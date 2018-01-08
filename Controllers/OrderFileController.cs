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
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

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
            string bakpath = direc_pdf + @"/FileUpload/filereconginze/bak/";//备份原始文件目录
            if (!Directory.Exists(bakpath))
            {
                Directory.CreateDirectory(bakpath);
            }
            FileInfo fi = new FileInfo(direc_pdf + filepath);

            System.Uri Uri = new Uri("ftp://" + ConfigurationManager.AppSettings["FTPServer"] + ":" + ConfigurationManager.AppSettings["FTPPortNO"]);
            string UserName = ConfigurationManager.AppSettings["FTPUserName"];
            string Password = ConfigurationManager.AppSettings["FTPPassword"];
            FtpHelper ftp = new FtpHelper(Uri, UserName, Password);


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
            string newfilepath = "/44/" + ordercode + "/" + filepath.Substring(filepath.LastIndexOf(@"/") + 1);

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
                                                ,filesuffix,IETYPE,uploaduserid,uploadusername) 
                                            values(List_Attachment_Id.Nextval
                                                ,'{0}','{1}','{2}',sysdate,'{3}','{4}','{5}'
                                                ,'{6}','{7}','{8}','{9}')";
                    sql_insert = string.Format(sql_insert
                            , "/44/" + ordercode + "/" + filepath.Substring(filepath.LastIndexOf(@"/") + 1), originalname, "44", ordercode, fi.Length, "订单文件"
                            , filesuffix, dt_order.Rows[0]["BUSITYPE"].ToString() == "40" ? "仅出口" : "仅进口", json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"));
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
                                                ,filesuffix,IETYPE,uploaduserid,uploadusername)  
                                            values(List_Attachment_Id.Nextval
                                                ,'{0}','{1}','{2}',sysdate,'{3}','{4}','{5}'
                                                ,'{6}','{7}','{8}','{9}')";
                        sql_insert = string.Format(sql_insert
                                , "/44/" + dt_asOrder.Rows[0]["code"].ToString() + "/" + filepath.Substring(filepath.LastIndexOf(@"/") + 1), originalname, "44", dt_asOrder.Rows[0]["code"].ToString(), fi.Length, "订单文件"
                                , filesuffix, dt_asOrder.Rows[0]["BUSITYPE"].ToString() == "40" ? "仅出口" : "仅进口", json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"));
                        DBMgr.ExecuteNonQuery(sql_insert, conn);
                    }

                }
                else
                {
                    sql_insert = @"insert into LIST_ATTACHMENT (id
                                                ,filename,originalname,filetype,uploadtime,ordercode,sizes,filetypename
                                                ,filesuffix,uploaduserid,uploadusername)  
                                            values(List_Attachment_Id.Nextval
                                                ,'{0}','{1}','{2}',sysdate,'{3}','{4}','{5}'
                                                ,'{6}','{7}','{8}')";
                    sql_insert = string.Format(sql_insert
                            , "/44/" + ordercode + "/" + filepath.Substring(filepath.LastIndexOf(@"/") + 1), originalname, "44", ordercode, fi.Length, "订单文件"
                            , filesuffix, json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"));
                    DBMgr.ExecuteNonQuery(sql_insert, conn);
                }

                //关联成功 ，文件挪到自动上传到文件服务器的目录，并删除原始目录的文件、修改原始路径为服务器新路径
                DBMgr.ExecuteNonQuery("update list_filerecoginze set status='已关联',ordercode='" + ordercode + "',cusno='" + cusno
                    + "',filepath='/44/" + ordercode + "/" + filepath.Substring(filepath.LastIndexOf(@"/") + 1) + "' where id=" + id, conn);                

                bool res = ftp.UploadFile(direc_pdf + filepath, newfilepath, true);
                if (res)
                {
                    ot.Commit();

                    fi.CopyTo(bakpath + filepath.Substring(filepath.LastIndexOf(@"/") + 1));
                    fi.Delete();
                }
                else
                {
                    ot.Rollback();

                    DBMgr.ExecuteNonQuery("update list_filerecoginze set status='关联失败',ordercode='" + ordercode + "',cusno='" + cusno + "' where id=" + id);
                }

                //Submit(ordercode, json_user);//add 提交委托

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

        public string ExportFileRecoginze()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = QueryConditionFileRecoginze();
            sql = sql + " order by lf.TIMES desc";

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            DataTable dt_count = DBMgr.GetDataTable("select count(1) from (" + sql + ") a");
            int WebDownCount = Convert.ToInt32(ConfigurationManager.AppSettings["WebDownCount"]);
            if (Convert.ToInt32(dt_count.Rows[0][0]) > WebDownCount)
            {
                return "{success:false,WebDownCount:" + WebDownCount + "}";
            }

            DataTable dt = DBMgr.GetDataTable(sql);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();

            //添加一个导出成功sheet
            NPOI.SS.UserModel.ISheet sheet_S;
            string filename = "文件关联.xls";

            sheet_S = book.CreateSheet("文件关联");

            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("关联状态"); row1.CreateCell(1).SetCellValue("订单编号"); row1.CreateCell(2).SetCellValue("客户编号"); 
            row1.CreateCell(3).SetCellValue("创建时间"); row1.CreateCell(4).SetCellValue("创建人"); 

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["STATUS"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["ORDERCODE"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["TIMES"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["USERNAME"].ToString());
            }

            // 写入到客户端 
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //book.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", filename);

            return Extension.getPathname(filename, book);
        }

//        public static string Submit(string ordercode, JObject json_user)
//        {
//            string rtnstr = "{success:true}";
//            DataTable dt_order = DBMgr.GetDataTable("select * from list_order a where a.code='" + ordercode + "' and a.ISINVALID=0");

//            string busitype = dt_order.Rows[0]["BUSITYPE"].ToString();
//            string entrusttype = dt_order.Rows[0]["ENTRUSTTYPE"].ToString();


//            if (busitype == "30" || busitype == "31")//陆运业务
//            {
//                string status = "10", declstatus = "", inspstatus = "";
//                if (entrusttype == "01")
//                {
//                    if (dt_order.Rows[0]["DECLSTATUS"].ToString() != "")
//                    {
//                        if (Convert.ToInt32(dt_order.Rows[0]["DECLSTATUS"].ToString()) >= 10)
//                        {
//                            return rtnstr;
//                        }
//                    }
//                    declstatus = status;
//                }
//                if (entrusttype == "02")
//                {
//                    if (dt_order.Rows[0]["INSPSTATUS"].ToString() != "")
//                    {
//                        if (Convert.ToInt32(dt_order.Rows[0]["INSPSTATUS"].ToString()) >= 10)
//                        {
//                            return rtnstr;
//                        }
//                    }
//                    inspstatus = status;
//                }
//                if (entrusttype == "03")
//                {
//                    if (dt_order.Rows[0]["DECLSTATUS"].ToString() != "")
//                    {
//                        if (Convert.ToInt32(dt_order.Rows[0]["DECLSTATUS"].ToString()) >= 10)
//                        {
//                            return rtnstr;
//                        }
//                    }
//                    if (dt_order.Rows[0]["INSPSTATUS"].ToString() != "")
//                    {
//                        if (Convert.ToInt32(dt_order.Rows[0]["INSPSTATUS"].ToString()) >= 10)
//                        {
//                            return rtnstr;
//                        }
//                    }
//                    declstatus = status; inspstatus = status;
//                }

//                string sql = @"UPDATE LIST_ORDER SET STATUS='{1}',DECLSTATUS='{2}',INSPSTATUS='{3}',SUBMITUSERID='{4}',SUBMITUSERNAME='{5}'  
//                                    ,SUBMITTIME=sysdate,BUSIKIND='001',ORDERWAY='1'                                    
//                            WHERE CODE = '{0}' ";
//                sql = string.Format(sql, ordercode, status, declstatus, inspstatus, json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"));

//                int result = DBMgr.ExecuteNonQuery(sql);
//                if (result == 1)
//                {
//                    Extension.add_list_time(10, ordercode, json_user);//插入订单状态变更日志
//                }
//                else
//                {
//                    rtnstr = "{success:false}";
//                }
//            }
//            return rtnstr;

//        }


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
