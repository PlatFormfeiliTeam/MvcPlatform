using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    [Authorize]
    [Filters.AuthFilter]
    public class OrderFileController : Controller
    {
        int totalProperty = 0;
        //
        // GET: /OrderFile/

        public ActionResult List_FileRecoginze()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            ViewBag.navigator = "订单中心>>文件关联";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        #region 文件关联

        public string QueryConditionFileRecoginze()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string where = "";
            if (!string.IsNullOrEmpty(Request["CUSNO"]))
            {
                where += " and instr(lf.CUSNO,'" + Request["CUSNO"] + "')>0";
            }
            if (!string.IsNullOrEmpty(Request["ORDERCODE"]))
            {
                where += " and instr(lf.ORDERCODE,'" + Request["ORDERCODE"] + "')>0";
            }
            if (!string.IsNullOrEmpty(Request["STATUS"]))
            {
                where += " and lf.STATUS='" + Request["STATUS"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["STARTDATE"]))
            {
                where += " and lf.TIMES>=to_date('" + Request["STARTDATE"] + "','yyyy-mm-dd hh24:mi:ss')";
            }
            if (!string.IsNullOrEmpty(Request["ENDDATE"]))
            {
                where += " and lf.TIMES<=to_date('" + Request["ENDDATE"] + "','yyyy-mm-dd hh24:mi:ss')+1";
            }
            if ((Request["OnlySelf"] + "").Trim() == "fa fa-check-square-o")
            {
                where += " and lf.USERID = " + json_user.Value<string>("ID");
            }

            string sql = @"select lf.* from list_filerecoginze lf where lf.customercode ='{0}'" + where;
            sql = string.Format(sql, json_user.Value<string>("CUSTOMERCODE"));

            return sql;

        }

        public string loadFileRecoginze()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string sql = QueryConditionFileRecoginze();

            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "lf.TIMES", "DESC"));
            var json = JsonConvert.SerializeObject(dt, iso);

            return "{rows:" + json + ",total:" + totalProperty + "}";
        }


        public ActionResult UploadFile(int? chunk, string name)
        {
            var fileUpload = Request.Files[0];
            var uploadPath = Server.MapPath("/FileUpload/filereconginze");
            chunk = chunk ?? 0;
            using (var fs = new FileStream(Path.Combine(uploadPath, name), chunk == 0 ? FileMode.Create : FileMode.Append))
            {
                var buffer = new byte[fileUpload.InputStream.Length];
                fileUpload.InputStream.Read(buffer, 0, buffer.Length);
                fs.Write(buffer, 0, buffer.Length);
            }
            create_save(name, Path.GetFileName(fileUpload.FileName));
            return Content("chunk uploaded", "text/plain");
        }

        public string create_save(string name, string originalname)
        {
            string sql = ""; string resultmsg = "{success:false}";
            try
            {
                JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

                sql = @"INSERT INTO LIST_FILERECOGINZE (ID
                        ,FILEPATH,FILENAME,USERID,USERNAME,TIMES,STATUS,CUSTOMERCODE
                        ,CUSTOMERNAME                                           
                        ) VALUES (LIST_FILERECOGINZE_ID.Nextval
                            ,'{0}','{1}','{2}','{3}',sysdate,'{4}','{5}'
                            ,'{6}'
                            )";
                sql = string.Format(sql, "/FileUpload/filereconginze/" + name, originalname, json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"), "未关联", json_user.Value<string>("CUSTOMERCODE")
                    , json_user.Value<string>("CUSTOMERNAME"));

                int i = DBMgr.ExecuteNonQuery(sql);
                if (i > 0)
                {
                    resultmsg = "{success:true}";
                }
            }
            catch (Exception ex)
            {

            }

            return resultmsg;
        }

        public FileResult DownFile(string filename, string isupload)//ActionResult
        {
            string url = ""; byte[] bytes;
            if (isupload == "1")//文件在文件服务器上
            {
                url = ConfigurationManager.AppSettings["AdminUrl"] + "/file" + filename;
                WebClient wc = new WebClient();
                bytes = wc.DownloadData(url);
                wc.Dispose();
            }
            else//企业端上传的文件，未保存前还在当前服务器上
            {
                url = Server.MapPath(filename);
                FileStream fs = new FileStream(url, FileMode.Open);
                bytes = new byte[(int)fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
            }
            MemoryStream ms = new MemoryStream(bytes);
            return File(ms, "application/octet-stream", Path.GetFileName(filename));
        }

        public string DeleteRecoginze()
        {
            string ids = Request["ids"];
            string result = "{success:false}"; string sql = "";

            bool bf = false;
            sql = "select * from list_filerecoginze where id in(" + ids + ")";
            DataTable dt = DBMgr.GetDataTable(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["STATUS"].ToString() == "已关联")
                {
                    bf = true;
                    break;
                }
            }

            if (bf) { return result; }

            foreach (DataRow dr in dt.Rows)
            {
                FileInfo fi = new FileInfo(Request.PhysicalApplicationPath + dr["FILEPATH"].ToString());
                fi.Delete();
            }
            sql = "delete from list_filerecoginze where id in(" + ids + ")";
            DBMgr.ExecuteNonQuery(sql);

            result = "{success:true}";

            return result;
        }

        /* //订单随附文件
                sql = @"select * from LIST_ATTACHMENT where instr(ordercode,'{0}') >0 
                      and ((filetype=44 or filetype=58) or ( filetype=57 AND confirmstatus = 1 )) and (abolishstatus is null or abolishstatus=0)";
                sql = string.Format(sql, ordercode);
                dt = DBMgr.GetDataTable(sql);
                string filedata = JsonConvert.SerializeObject(dt, iso);


                result = "{formdata:" + formdata + ",filedata:" + filedata + "}";*/

        #endregion

        private string GetPageSql(string tempsql, string order, string asc)
        {
            int start = Convert.ToInt32(Request["start"]);
            int limit = Convert.ToInt32(Request["limit"]);
            string sql = "select count(1) from ( " + tempsql + " )";
            totalProperty = Convert.ToInt32(DBMgr.GetDataTable(sql).Rows[0][0]);
            string pageSql = @"SELECT * FROM ( SELECT tt.*, ROWNUM AS rowno FROM ({0} ORDER BY {1} {2}) tt WHERE ROWNUM <= {4}) table_alias WHERE table_alias.rowno >= {3}";
            pageSql = string.Format(pageSql, tempsql, order, asc, start + 1, limit + start);
            return pageSql;
        }

    }
}
