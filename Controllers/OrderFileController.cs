using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
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
                if (dt.Rows[i]["STATUS"].ToString() == "已关联" || dt.Rows[i]["STATUS"].ToString() == "关联中")
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

        public string SaveRecoginze()
        {
            string id = Request["id"]; string cusno = Request["cusno"]; string ordercode = Request["ordercode"];
            string result = "{success:false}"; string sql = "";

            sql = "select * from list_filerecoginze where id =" + id;
            DataTable dt = DBMgr.GetDataTable(sql);
            if (dt == null)
            {
                return result;
            }
            if (dt.Rows.Count != 1)
            {
                return result;
            }

            if (dt.Rows[0]["STATUS"].ToString() == "已关联")//已经关联
            {
                return "{success:true,flag:'Y'}";
            }
            //----------------------------------------------------------------------------------------------------------------------
            //--list_filerecoginze
            string filepath = dt.Rows[0]["FILEPATH"].ToString();
            string originalname = dt.Rows[0]["FILENAME"].ToString(); string filesuffix = originalname.Substring(originalname.LastIndexOf(".") + 1).ToUpper();
            string direc_pdf = Request.PhysicalApplicationPath;
            FileInfo fi = new FileInfo(direc_pdf + filepath);

            DataTable dt_order = new DataTable();

            //根据识别出的订单号，查询是否存在此订单
            sql = "select * from list_order a where a.ISINVALID=0";
            if (ordercode != "") { sql += " and a.code='" + ordercode + "'"; }
            if (cusno != "") { sql += " and a.cusno='" + cusno + "'"; }
            dt_order = DBMgr.GetDataTable(sql);
            if (dt_order == null) { return "{success:true,flag:'E'}"; }
            if (dt_order.Rows.Count <= 0) { return "{success:true,flag:'E'}"; }

            //如果为空的话，再次赋值
            if (ordercode == "") { ordercode = dt_order.Rows[0]["CODE"].ToString(); }
            if (cusno == "") { cusno = dt_order.Rows[0]["CUSNO"].ToString(); }
            string associateno = dt_order.Rows[0]["ASSOCIATENO"].ToString();
            OracleConnection conn = null;
            OracleTransaction ot = null;
            conn = DBMgr.getOrclCon();
            try
            {
                conn.Open();
                ot = conn.BeginTransaction();
                string sql_insert = "";

                if (associateno != "")//两单关联
                {
                    sql_insert = @"insert into LIST_ATTACHMENT (id
                                                ,filename,originalname,filetype,uploadtime,ordercode,sizes,filetypename
                                                ,filesuffix,IETYPE) 
                                            values(List_Attachment_Id.Nextval
                                                ,'{0}','{1}','{2}',sysdate,'{3}','{4}','{5}'
                                                ,'{6}','{7}')";
                    sql_insert = string.Format(sql_insert
                            , "/44/" + ordercode + "/" + filepath.Substring(filepath.LastIndexOf(@"/") + 1), originalname, "44", ordercode, fi.Length, "订单文件"
                            , filesuffix, dt_order.Rows[0]["BUSITYPE"].ToString() == "40" ? "仅出口" : "仅进口");
                    DBMgr.ExecuteNonQuery(sql_insert, conn);

                    DataTable dt_asOrder = new DataTable();
                    if (associateno != "")//两单关联
                    {
                        dt_asOrder = DBMgr.GetDataTable("select * from list_order a where a.ISINVALID=0 and ASSOCIATENO='" + associateno + "' and code!='" + ordercode + "'");
                    }

                    if (dt_asOrder == null)
                    {

                    }

                    else if (dt_asOrder.Rows.Count < 0)
                    {

                    }
                    else
                    {
                        sql_insert = @"insert into LIST_ATTACHMENT (id
                                                ,filename,originalname,filetype,uploadtime,ordercode,sizes,filetypename
                                                ,filesuffix,IETYPE) 
                                            values(List_Attachment_Id.Nextval
                                                ,'{0}','{1}','{2}',sysdate,'{3}','{4}','{5}'
                                                ,'{6}','{7}')";
                        sql_insert = string.Format(sql_insert
                                , "/44/" + dt_asOrder.Rows[0]["code"].ToString() + "/" + filepath.Substring(filepath.LastIndexOf(@"/") + 1), originalname, "44", dt_asOrder.Rows[0]["code"].ToString(), fi.Length, "订单文件"
                                , filesuffix, dt_asOrder.Rows[0]["BUSITYPE"].ToString() == "40" ? "仅出口" : "仅进口");
                        DBMgr.ExecuteNonQuery(sql_insert, conn);
                    }

                }
                else
                {
                    sql_insert = @"insert into LIST_ATTACHMENT (id
                                                ,filename,originalname,filetype,uploadtime,ordercode,sizes,filetypename
                                                ,filesuffix) 
                                            values(List_Attachment_Id.Nextval
                                                ,'{0}','{1}','{2}',sysdate,'{3}','{4}','{5}'
                                                ,'{6}')";
                    sql_insert = string.Format(sql_insert
                            , "/44/" + ordercode + "/" + filepath.Substring(filepath.LastIndexOf(@"/") + 1), originalname, "44", ordercode, fi.Length, "订单文件"
                            , filesuffix);
                    DBMgr.ExecuteNonQuery(sql_insert, conn);
                }

                //关联成功 ，文件挪到自动上传到文件服务器的目录，并删除原始目录的文件、修改原始路径为服务器新路径
                DBMgr.ExecuteNonQuery("update list_filerecoginze set status='已关联',ordercode='" + ordercode + "',cusno='" + cusno
                    + "',filepath='/44/" + ordercode + "/" + filepath.Substring(filepath.LastIndexOf(@"/") + 1) + "' where id=" + id, conn);
                ot.Commit();

                fi.CopyTo(direc_pdf + @"/FileUpload/file/" + filepath.Substring(filepath.LastIndexOf(@"/") + 1));
                fi.Delete();

                result = "{success:true}";
            }
            catch (Exception ex)
            {
                ot.Rollback();
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public string selectorder()
        {
            string where = "";
            if (!string.IsNullOrEmpty(Request["BUSIUNIT"]))
            {
                where += " and (busiunitcode like '%" + Request["BUSIUNIT"] + "%' or busiunitname like '%" + Request["BUSIUNIT"] + "%')";
            }
            if (!string.IsNullOrEmpty(Request["BUSITYPE"]))
            {
                where += " and busitype='" + Request["BUSITYPE"] + "'";
            }
            string sql = "select id,code,cusno,associateno,busitype,busiunitcode,busiunitname from list_order where isinvalid=0 " + where;
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "createtime", "desc"));
            string json = JsonConvert.SerializeObject(dt); ;
            return "{total:" + totalProperty + ",rows:" + json + "}";
        }

        public string filedetail()
        {
            string ordercode = Request["ordercode"]; 
            //订单随附文件
            string sql = @"select * from LIST_ATTACHMENT where instr(ordercode,'{0}') >0 
                      and ((filetype=44 or filetype=58) or ( filetype=57 AND confirmstatus = 1 )) and (abolishstatus is null or abolishstatus=0)";
            sql = string.Format(sql, ordercode);
            DataTable dt = DBMgr.GetDataTable(sql);

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string filedata = JsonConvert.SerializeObject(dt, iso);


            string result = "{filedata:" + filedata + "}";
            return result;
        }
        

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
