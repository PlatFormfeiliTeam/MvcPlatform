using Aspose.Cells;
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
using System.Drawing;
using System.Drawing.Printing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using StackExchange.Redis;

namespace MvcPlatform.Controllers
{
    public class EnterpriseOrderController : Controller
    {
        int totalProperty = 0;
        Bitmap bmp_Print = null;//报关单号条码(给打印事件用)
        string Declcode = "";//报关单号(给打印事件用)

        public ActionResult ProcessOrder()//委托任务
        {
            ViewBag.navigator = "客户服务>>委托任务";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult ProcessOrder_Data()//委托任务仅显示有数据的记录
        {
            ViewBag.navigator = "客户服务>>委托任务_数据";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult ProcessServer()//委托服务
        {
            ViewBag.navigator = "客户服务>>委托服务";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult EntOrder_Detail()  //文件委托
        {
            ViewBag.navigator = "企业服务>>委托任务";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult EntOrderList()  //文件委托
        {
            ViewBag.navigator = "企业服务>>委托任务";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult BatchMaintain()
        {
            return View();
        }

        public ActionResult ListOrder_Index()  
        {
            //ViewBag.navigator = "企业服务>>委托任务";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult GoodsTrack()  //货况跟踪
        {
            //ViewBag.navigator = "货况跟踪";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        

        public string GetCode(string combin)
        {
            if (string.IsNullOrEmpty(combin))
            {
                return "";
            }
            else
            {
                int start = combin.LastIndexOf("(");
                int end = combin.LastIndexOf(")");
                return combin.Substring(start + 1, end - start - 1);
            }
        }
        public string GetName(string combin)
        {
            if (string.IsNullOrEmpty(combin))
            {
                return "";
            }
            else
            {
                int index = combin.LastIndexOf("(");
                return combin.Substring(0, index);
            }
        }

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
        //报关行角色查看订单暂存区数据时 加载申报单位与之对应的数据 
        public string LoadProcess()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string where = "";
            if (!string.IsNullOrEmpty(Request["ENTERPRISENAME"]))//判断查询条件是否有值
            {
                where += " and t.ENTERPRISECODE='" + GetCode(Request["ENTERPRISENAME"]) + "'";
            }
            if (!string.IsNullOrEmpty(Request["CODE"]))//判断查询条件是否有值
            {
                where += " and instr(t.CODE,'" + Request["CODE"].ToString().Trim() + "')>0 ";
            }
            if (!string.IsNullOrEmpty(Request["PRINTSTATUS"]))//判断查询条件是否有值
            {
                where += " and  t.PRINTSTATUS='" + Request["PRINTSTATUS"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["STARTDATE"]))//如果开始时间有值
            {
                where += " and t.SUBMITTIME>=to_date('" + Request["STARTDATE"] + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            if (!string.IsNullOrEmpty(Request["ENDDATE"]))//如果结束时间有值
            {
                where += " and t.SUBMITTIME<=to_date('" + Request["ENDDATE"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            //string sql = @"select t.*,(select count(1) from list_attachment l where l.entid=t.ID ) FILENUM from ENT_ORDER t where t.FILEDECLAREUNITCODE='" + json_user.Value<string>("CUSTOMERHSCODE") + "'" + where;
            //2016/10/9 为了提升load效能 FILENUM获取修改为：
            string sql = @"select t.*,l.FILENUM 
                            from ENT_ORDER t 
                                left join (select entid,count(1) as FILENUM from list_attachment where entid is not null group by entid) l on t.ID=l.entid
                            where l.FILENUM>0 and t.FILEDECLAREUNITCODE='" + json_user.Value<string>("CUSTOMERHSCODE") + "'" + where;
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "CREATETIME", "desc"));
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }
        public string LoadProcess_Data()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string where = "";
            if (!string.IsNullOrEmpty(Request["ENTERPRISENAME"]))//判断查询条件是否有值
            {
                where += " and t.ENTERPRISECODE='" + GetCode(Request["ENTERPRISENAME"]) + "'";
            }
            if (!string.IsNullOrEmpty(Request["CODE"]))//判断查询条件是否有值
            {
                where += " and instr(t.CODE,'" + Request["CODE"].ToString().Trim() + "')>0 ";
            }
            if (!string.IsNullOrEmpty(Request["PRINTSTATUS"]))//判断查询条件是否有值
            {
                where += " and  t.PRINTSTATUS='" + Request["PRINTSTATUS"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["STARTDATE"]))//如果开始时间有值
            {
                where += " and t.SUBMITTIME>=to_date('" + Request["STARTDATE"] + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            if (!string.IsNullOrEmpty(Request["ENDDATE"]))//如果结束时间有值
            {
                where += " and t.SUBMITTIME<=to_date('" + Request["ENDDATE"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            //string sql = @"select t.*,(select count(1) from list_attachment l where l.entid=t.ID ) FILENUM from ENT_ORDER t where t.FILEDECLAREUNITCODE='" + json_user.Value<string>("CUSTOMERHSCODE") + "'" + where;
            //2016/10/9 为了提升load效能 FILENUM获取修改为：
            string sql = @"select * 
                            from ENT_ORDER t 
                                left join  LIST_CUSDATA_FL b on t.code=b.cusno 
                            where b.cusno is not null and t.FILEDECLAREUNITCODE='" + json_user.Value<string>("CUSTOMERHSCODE") + "'" + where;
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "t.CREATETIME", "desc"));
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }
        //文件申报单位维度委托任务的批量打印功能
        public string BatchPrint()
        {
            string entids = Request["entids"];
            string[] id_array = entids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string output = Guid.NewGuid() + "";
            string sql = "";
            DataTable dt = null;
            IList<string> filelist = new List<string>();
            foreach (string entid in id_array)
            {
                sql = @"select t.*,instr('44515250',t.filetype) as typeindex from list_attachment t  where t.entid='" + entid + "' order by typeindex ASC";
                dt = DBMgr.GetDataTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    filelist.Add(ConfigurationManager.AppSettings["AdminUrl"] + "/file/" + dr["FILENAME"]);
                }
                sql = "update ent_order set printstatus=1 where id='" + entid + "'";
                DBMgr.ExecuteNonQuery(sql);
            }
            Extension.MergePDFFiles(filelist, Server.MapPath("~/Declare/") + output + ".pdf");
            return "/Declare/" + output + ".pdf";
        }

        public string load_entorder_detail()
        {
            string result = string.Empty;
            string ID = Request["ID"]; //string busitype = Request["busitype"];
            DataTable dt;
            string data = "{}";
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string sql = "select a.* from ENT_ORDER a where a.ID = '" + ID + "'";
            dt = DBMgr.GetDataTable(sql);
            data = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');
            //申报数据
            string decl_data = "[]";
            string product_data = "[]";
            string file_data = "[]";
            if (dt.Rows.Count > 0)
            {
                sql = "select * from LIST_CUSDATA_FL where cusno='" + dt.Rows[0]["CODE"] + "'";
                DataTable dt2 = DBMgr.GetDataTable(sql);
                decl_data = JsonConvert.SerializeObject(dt2, iso);
                if (dt2.Rows.Count > 0)
                {
                    sql = "select * from LIST_CUSDATA_SUB_FL where PCODE='" + dt2.Rows[0]["CUSNO"] + "'";
                    DataTable dt3 = DBMgr.GetDataTable(sql);
                    product_data = JsonConvert.SerializeObject(dt3, iso);
                }
                sql = "select * from list_attachment t where t.entid='" + ID + "'";
                file_data = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql), iso);
            }
            result = "{data:" + data + ",decl_data:" + decl_data + ",product_data:" + product_data + ",file_data:" + file_data + "}";
            return result;

        }
        public string loadform()
        {
            Object json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string ID = Request["ID"]; //string busitype = Request["busitype"];

            DataTable dt;
            string data = "{}", filedata = "[]";
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            if (string.IsNullOrEmpty(ID))//如果为空、即新增的时候
            {
                string result = "{STATUS:0}";//string result = "{STATUS:0,BUSITYPEID:'" + busitype + "'}";
                return "{data:" + result + ",filedata:" + filedata + "}";
            }
            else
            {
                string sql = "select a.* from ENT_ORDER a where a.ID = '" + ID + "'";
                dt = DBMgr.GetDataTable(sql);
                data = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');
                //随附文件
                sql = "select * from list_attachment t where t.entid='" + ID + "'";
                DataTable dt_detail = DBMgr.GetDataTable(sql);
                return "{data:" + data + ",filedata:" + JsonConvert.SerializeObject(dt_detail, iso) + "}";
            }
        }

        public string Ini_Base_Data_REPWAY()
        {
            string sql = "";
            string json_sbfs = "[]";//申报方式
            string busitype = Request["busitype"];

            sql = "select CODE,NAME||'('||CODE||')' NAME from SYS_REPWAY where Enabled=1 and instr(busitype,'" + busitype + "')>0";
            json_sbfs = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

            return "{sbfs:" + json_sbfs + "}";
        }
            public string Ini_Base_Data_TEMPLATENAME()
        { 
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string busiunitcode=json_user.Value<string>("CUSTOMERHSCODE");
            string sql = "select to_char(ID) ID,TEMPLATENAME from list_declaration_template where busiunitcode='" + busiunitcode + "'";
            string json_templatename = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
            return "{templatename:" + json_templatename + "}";
        }
            public ActionResult Upload_WebServer(int? chunk, string name)
            {
                var fileUpload = Request.Files[0];
                var uploadPath = Server.MapPath("/FileUpload/file");
                chunk = chunk ?? 0;
                using (var fs = new FileStream(Path.Combine(uploadPath, name), chunk == 0 ? FileMode.Create : FileMode.Append))
                {
                    var buffer = new byte[fileUpload.InputStream.Length];
                    fileUpload.InputStream.Read(buffer, 0, buffer.Length);
                    fs.Write(buffer, 0, buffer.Length);
                }
                return Content("chunk uploaded", "text/plain");
            }
            public FileResult DownFile(string filename, string ID)//ActionResult
            {
                string url = ""; byte[] bytes;
                if (ID != "")//文件在文件服务器上
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

            public string Save()
            {
                try
                {
                    JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
                    JObject json_data = (JObject)JsonConvert.DeserializeObject(Request["data"]);
                    string filedata = Request["filedata"] + "";
                    string action=Request["action"]+"";
                    string status =action=="delegate"?"10":json_data.Value<string>("STATUS");
                    string insert_sql = "";
                    string update_sql = "";
                    string sql = "";
                    string ent_id = "";


                    if (!string.IsNullOrEmpty(json_data.Value<string>("CODE")))
                    {
                        string sql_check="select * from ENT_ORDER where CODE='"+json_data.Value<string>("CODE")+"'";
                        if (!string.IsNullOrEmpty(json_data.Value<string>("ID")))
                        {
                            sql_check += " and ID !='" + json_data.Value<string>("ID") + "'";
                        }
                        DataTable dt_itemno = new DataTable();
                        dt_itemno = DBMgr.GetDataTable(sql_check);
                        if (dt_itemno.Rows.Count > 0) {  return "{success:false,isrepeate:'Y'}"; }

                    }
                    if (string.IsNullOrEmpty(json_data.Value<string>("ID")))//新增
                    {
                    insert_sql = @"insert into ENT_ORDER (id,createtime, unitcode,filerecevieunitcode, filerecevieunitname,
                    filedeclareunitcode,filedeclareunitname, busitypeid,customdistrictcode,customdistrictname,repwayid, 
                    createid, createname,enterprisecode, enterprisename,remark,code,createmode,status,isreadpdf,templatename) 
                    values ('{0}',sysdate,(select fun_AutoQYBH(sysdate) from dual),'{1}','{2}','{3}','{4}','{5}',
                     '{6}','{7}','{8}','{9}','{10}', '{11}','{12}','{13}','{14}','{15}','{16}',{17},'{18}')";
                        if (json_data.Value<string>("CREATEMODE") == "按批次")
                        {
                            sql = "select ENT_ORDER_ID.Nextval from dual";
                            ent_id = DBMgr.GetDataTable(sql).Rows[0][0] + "";//获取ID
                            sql = string.Format(insert_sql, ent_id, GetCode(json_data.Value<string>("FILERECEVIEUNITNAME")), GetName(json_data.Value<string>("FILERECEVIEUNITNAME")),
                                  GetCode(json_data.Value<string>("FILEDECLAREUNITNAME")), GetName(json_data.Value<string>("FILEDECLAREUNITNAME")),
                                  json_data.Value<string>("BUSITYPEID"), json_data.Value<string>("CUSTOMDISTRICTCODE"), json_data.Value<string>("CUSTOMDISTRICTNAME"),
                                  json_data.Value<string>("REPWAYID"), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"),
                                  json_user.Value<string>("CUSTOMERHSCODE"), json_user.Value<string>("CUSTOMERNAME"), json_data.Value<string>("REMARK"),
                                  json_data.Value<string>("CODE"), json_data.Value<string>("CREATEMODE"), status, json_data.Value<string>("ISREADPDF"), json_data.Value<string>("TEMPLATENAME"));
                            DBMgr.ExecuteNonQuery(sql);
                            //更新随附文件
                            Extension.Update_Attachment_ForEnterprise(ent_id, filedata, json_data.Value<string>("ORIGINALFILEIDS"), json_user);
                        }
                        if (json_data.Value<string>("CREATEMODE") == "按文件")
                        {
                            JArray jarry = JsonConvert.DeserializeObject<JArray>(filedata);
                            foreach (JObject json in jarry)
                            {
                                sql = "select ENT_ORDER_ID.Nextval from dual";
                                ent_id = DBMgr.GetDataTable(sql).Rows[0][0] + "";//获取ID
                                sql = string.Format(insert_sql, ent_id, GetCode(json_data.Value<string>("FILERECEVIEUNITNAME")), GetName(json_data.Value<string>("FILERECEVIEUNITNAME")),
                                      GetCode(json_data.Value<string>("FILEDECLAREUNITNAME")), GetName(json_data.Value<string>("FILEDECLAREUNITNAME")),
                                      json_data.Value<string>("BUSITYPEID"), json_data.Value<string>("CUSTOMDISTRICTCODE"), json_data.Value<string>("CUSTOMDISTRICTNAME"),
                                      json_data.Value<string>("REPWAYID"), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"),
                                      json_user.Value<string>("CUSTOMERHSCODE"), json_user.Value<string>("CUSTOMERNAME"), json_data.Value<string>("REMARK"),
                                      json_data.Value<string>("CODE"), json_data.Value<string>("CREATEMODE"), status, json_data.Value<string>("ISREADPDF"), json_data.Value<string>("TEMPLATENAME"));
                                DBMgr.ExecuteNonQuery(sql);
                                //更新随附文件
                                Extension.Update_Attachment_ForEnterprise(ent_id, "[" + JsonConvert.SerializeObject(json) + "]", json_data.Value<string>("ORIGINALFILEIDS"), json_user);
                            }
                        }



                    }
                    else//修改单独页面做
                    {
                        update_sql = @"update ENT_ORDER  set filerecevieunitcode='{1}',filerecevieunitname='{2}',filedeclareunitcode='{3}',
                    filedeclareunitname='{4}',busitypeid='{5}',customdistrictcode='{6}', customdistrictname='{7}',
                    repwayid='{8}',remark='{9}',code='{10}',status='{11}',templatename='{12}' where id='{0}'";
                        sql = string.Format(update_sql, json_data.Value<string>("ID"), GetCode(json_data.Value<string>("FILERECEVIEUNITNAME")),
                        GetName(json_data.Value<string>("FILERECEVIEUNITNAME")), GetCode(json_data.Value<string>("FILEDECLAREUNITNAME")),
                        GetName(json_data.Value<string>("FILEDECLAREUNITNAME")), json_data.Value<string>("BUSITYPEID"), json_data.Value<string>("CUSTOMDISTRICTCODE"),
                        json_data.Value<string>("CUSTOMDISTRICTNAME"), json_data.Value<string>("REPWAYID"), json_data.Value<string>("REMARK"), json_data.Value<string>("CODE"), status, json_data.Value<string>("TEMPLATENAME"));
                        DBMgr.ExecuteNonQuery(sql);
                        //更新随附文件
                        Extension.Update_Attachment_ForEnterprise(json_data.Value<string>("ID"), filedata, json_data.Value<string>("ORIGINALFILEIDS"), json_user);
                    }
                    return "{success:true}";
                }
                catch (Exception ex)
                {
                    return "{success:false}";
                }
            }

            public string Delete()
            {
                string id = Request["id"].ToString();
                string json = "{success:false}"; string sql = "";

                //删除订单随附文件
                System.Uri Uri = new Uri("ftp://" + ConfigurationManager.AppSettings["FTPServer"] + ":" + ConfigurationManager.AppSettings["FTPPortNO"]);
                string UserName = ConfigurationManager.AppSettings["FTPUserName"];
                string Password = ConfigurationManager.AppSettings["FTPPassword"];
                FtpHelper ftp = new FtpHelper(Uri, UserName, Password);
                sql = "select * from list_attachment where entid='" + id + "'";
                DataTable dt = DBMgr.GetDataTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    ftp.DeleteFile(dr["FILENAME"] + "");
                }

                sql = "delete from list_attachment where entid='" + id + "'";
                DBMgr.ExecuteNonQuery(sql);

                sql = "delete from ENT_ORDER where id = '" + id + "'";
                DBMgr.ExecuteNonQuery(sql);

                json = "{success:true}";
                return json;
            }
            public string loadOrderList()
            {
                JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
                string enterprisecode=json_user.Value<string>("CUSTOMERHSCODE");
                string sql="select t.*,l.FILENUM ,(case printstatus when '1' then 15 else (case when status is null then 10 else status end) end) as newstatus "+
                           "from ENT_ORDER t left join (select entid,count(1) as FILENUM from list_attachment where entid is not null group by entid) l on t.ID=l.entid "+
                           "where t.enterprisecode="+enterprisecode;
                string where = "";
                if (!string.IsNullOrEmpty(Request["FILERECEVIEUNIT"]))
                {
                    where += " and t.filerecevieunitcode='" + Request["FILERECEVIEUNIT"] + "'";
                }
                if (!string.IsNullOrEmpty(Request["FILEDECLAREUNIT"]))
                {
                    where += " and t.filedeclareunitcode='" + Request["FILEDECLAREUNIT"] + "'";
                }
                if (!string.IsNullOrEmpty(Request["CODE"]))
                {
                    where += " and instr(t.CODE,'"+Request["CODE"]+"')>0";
                }
                if (!string.IsNullOrEmpty(Request["STARTDATE"]))
                {
                    where += " and t.ENTERPRISECODE='"+ Request["ENTERPRISENAME"] + "'";
                }
                if (!string.IsNullOrEmpty(Request["ENDDATE"]))
                {
                    where += " and t.ENTERPRISECODE='"+ Request["ENTERPRISENAME"] + "'";
                }
                sql += where;
                IsoDateTimeConverter iso = new IsoDateTimeConverter();
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "CREATETIME", "desc"));
                var json = JsonConvert.SerializeObject(dt, iso);
                return "{rows:" + json + ",total:" + totalProperty + "}";
            
            
            }
        public FileResult ExportStu2()
        {
            string filepath = Request["filepath"];
            string ORIGINALNAME = Request["ORIGINALNAME"];
            DataTable dt = GetDataFromExcelByConn(filepath);


            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个导出成功sheet
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("sheet1");

            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                row1.CreateCell(j).SetCellValue(dt.Columns[j].ColumnName);
            }



            //将数据逐步写入sheet_S各个行
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    rowtemp.CreateCell(j).SetCellValue(dt.Rows[i][j].ToString());
                }
            }


            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", ORIGINALNAME.Substring(0, ORIGINALNAME.LastIndexOf(".")) + ".xls");

        }

        #region excel重组
        /// <summary>
        /// 读取Excel数据并重组
        /// </summary>
        /// <param name="hasTitle"></param>
        /// <returns></returns>
        private DataTable GetDataFromExcelByConn(string filepath, bool hasTitle = false)
        {
            /* OpenFileDialog openFile = new OpenFileDialog();
             openFile.Filter = "Excel文件(*.xls)|*.xls;*.xlsx";
             openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
             openFile.Multiselect = false;
             if (openFile.ShowDialog() == DialogResult.Cancel) return null;
             var filePath = openFile.FileName;
             string fileType = System.IO.Path.GetExtension(filePath);
             if (string.IsNullOrEmpty(fileType)) return null;
             * */



            DataTable dtExcel = ExcelToDatatalbe(Server.MapPath(filepath));


            /* string strConn = string.Format("Provider=Microsoft.Jet.OLEDB.{0}.0;" +
                             "Extended Properties=\"Excel {1}.0;HDR={2};IMEX=1;\";" +
                             "data source={3};",
                             (fileType == ".xls" ? 4 : 12), (fileType == ".xls" ? 8 : 12), (hasTitle ? "Yes" : "NO"), filePath);

             OleDbConnection conn = new OleDbConnection(strConn);
             conn.Open();

             DataTable dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });
             string strTableName = dtSheetName.Rows[0]["TABLE_NAME"].ToString();
             OleDbDataAdapter myCommand = null;
             string strExcel = "select * from [" + strTableName + "]";
             myCommand = new OleDbDataAdapter(strExcel, strConn);
             myCommand.Fill(ds);
             * */

            DataTable newdt = new DataTable();
            newdt.Columns.Add("BR", typeof(string));
            newdt.Columns.Add("BS", typeof(Int32));
            newdt.Columns.Add("BS1", typeof(Int32));
            for (int i = 0; i < dtExcel.Rows.Count; i++)
            {
                string bs = dtExcel.Rows[i]["REMARKS"].ToString();
                string[] bs1 = bs.Split(';');
                List<string> bsnew = new List<string>();
                for (int t1 = 0; t1 < bs1.Length; t1++)
                {
                    string bs2 = bs1[t1].ToString();
                    if (bs2.Contains("-"))
                    {
                        string bs21 = bs2.Split('-')[0].ToString();
                        string bs22 = bs2.Split('-')[1].ToString();
                        int bs211 = Convert.ToInt32(bs21.Split('(')[0].ToString());
                        int bs212 = Convert.ToInt32(bs22.Split('(')[0].ToString());
                        for (int t3 = 0; t3 < bs212 - bs211 + 1; t3++)
                        {
                            string newbs = (bs211 + t3) + "(" + bs21.Split('(')[1].ToString();
                            bsnew.Add(newbs);
                        }
                    }
                    else
                        bsnew.Add(bs2);
                }
                for (int t2 = 0; t2 < bsnew.Count(); t2++)
                {
                    string br = dtExcel.Rows[i]["PRECUSTOMS_CLEARANCE_ID"].ToString();
                    string bs3 = bsnew[t2].Split('(')[0].ToString();
                    string bs4 = bsnew[t2].Split('(')[1].ToString().Replace(")", "");
                    DataRow[] rows = newdt.Select("BR='" + br + "' and BS='" + bs3.ToString() + "'");
                    if (rows.Count() > 0)
                    {
                        DataRow drs = rows[0];
                        drs["BS1"] = (Convert.ToInt32(drs[2].ToString()) + Convert.ToInt32(bs4));
                    }
                    else
                    {
                        DataRow dr = newdt.NewRow();
                        dr["BR"] = dtExcel.Rows[i]["PRECUSTOMS_CLEARANCE_ID"].ToString();
                        dr["BS"] = bs3;
                        dr["BS1"] = bs4;
                        newdt.Rows.Add(dr);
                    }
                }
            }
            DataView dv = newdt.DefaultView;
            dv.Sort = " BR Asc,BS Asc";
            DataTable ndt = dv.ToTable();
            DataTable newbsdt = CreateNewDt(ndt);
            DataTable dt = CreateOldDt(dtExcel, newbsdt);
            return dt;

        }

        /// <summary>
        /// 重组原表，把原表中REMARKS格式重组为34061(30);34062(4)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="newdt"></param>
        /// <returns></returns>
        private DataTable CreateOldDt(DataTable dt, DataTable newdt)
        {
            string br = dt.Rows[dt.Rows.Count - 1]["PRECUSTOMS_CLEARANCE_ID"].ToString();
            //string bs=
            for (int i = dt.Rows.Count - 2; i > -1; i--)
            {
                if (br == dt.Rows[i]["PRECUSTOMS_CLEARANCE_ID"].ToString())
                {
                    dt.Rows.RemoveAt(i + 1);
                    if (i == 0)
                    {
                        DataRow[] rows = newdt.Select("BR='" + br + "' ");
                        if (rows.Count() > 0)
                        {
                            DataRow drs = rows[0];
                            dt.Rows[i]["REMARKS"] = (drs["BS"].ToString());
                        }
                    }
                }
                else
                {
                    DataRow[] rows = newdt.Select("BR='" + br + "' ");
                    if (rows.Count() > 0)
                    {
                        DataRow drs = rows[0];
                        dt.Rows[i + 1]["REMARKS"] = (drs["BS"].ToString());

                    }
                    br = dt.Rows[i]["PRECUSTOMS_CLEARANCE_ID"].ToString();
                }
            }

            return dt;
        }

      

       
        /// <summary>
        /// 重组临时表数据，组合后格式为：34061(30);34062(4)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable CreateNewDt(DataTable dt)
        {
            DataTable newdt = new DataTable();
            newdt.Columns.Add("BR", typeof(string));
            newdt.Columns.Add("BS", typeof(string));
            string br = "";
            string bs = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == 0)
                {
                    br = dt.Rows[i]["BR"].ToString();
                    bs = dt.Rows[i]["BS"].ToString() + "(" + dt.Rows[i]["BS1"].ToString() + ")" + ";";
                }
                else
                {
                    if (dt.Rows[i]["BR"].ToString() == dt.Rows[i - 1]["BR"].ToString())
                    {
                        bs += dt.Rows[i]["BS"].ToString() + "(" + dt.Rows[i]["BS1"].ToString() + ")" + ";";
                        if (i == dt.Rows.Count - 1)
                            newdt = ADDNewRow(newdt, br, bs);
                    }
                    else
                    {
                        newdt = ADDNewRow(newdt, br, bs);
                        br = dt.Rows[i]["BR"].ToString();
                        bs = dt.Rows[i]["BS"].ToString() + "(" + dt.Rows[i]["BS1"].ToString() + ")" + ";";
                        if (i == dt.Rows.Count - 1)
                            newdt = ADDNewRow(newdt, br, bs);
                    }
                }
            }
            return newdt;
        }


        /// <summary>
        /// 新增一条数据
        /// </summary>
        /// <param name="newdt"></param>
        /// <param name="br"></param>
        /// <param name="bs"></param>
        /// <returns></returns>
        private static DataTable ADDNewRow(DataTable newdt, string br, string bs)
        {
            DataRow dr = newdt.NewRow();
            dr["BR"] = br;
            dr["BS"] = bs.Substring(0, bs.Length - 1).ToString();
            newdt.Rows.Add(dr);
            return newdt;
        }

        public DataTable ExcelToDatatalbe(string filePath)
        {
            Workbook book = new Workbook(filePath);
            //book.Open(filePath);
            Worksheet sheet = book.Worksheets[0];
            Cells cells = sheet.Cells;
            DataTable dt_Import = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);//获取excel中的数据保存到一个datatable中
            return dt_Import;

        }
        public string DealCode()
        {
            string flag = string.Empty;
            string declcode = Request["code"] + "";

            DataTable dt_barcode = new DataTable();
            dt_barcode = DBMgr.GetDataTable("select * from sys_barcode where barcode='" + declcode + "' and createdate > sysdate - interval '7' day ");
            if (dt_barcode.Rows.Count > 0)
            {
                flag = "1";
            }
            else
            {

                DBMgr.ExecuteNonQuery("insert into sys_barcode(ID,BARCODE) values(SYS_BARCODE_id.Nextval,'" + declcode + "')");
            }

            return "{message:true,flag:\"" + flag + "\"}";
        }
        #endregion

        public string LoadList_index()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string where = QueryCondition();

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string sql = @"select * from LIST_ORDER where instr('" + Request["busitypeid"] + "',BUSITYPE)>0 and BUSIUNITCODE='" + json_user.Value<string>("CUSTOMERHSCODE") + "' " + where;
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "createtime", "desc"));
            var json = JsonConvert.SerializeObject(dt, iso);

            return "{rows:" + json + ",total:" + totalProperty +"}";
        }

        public string QueryCondition()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string where = "";

            if (!string.IsNullOrEmpty(Request["VALUE1"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION1"])
                {
                    case "DECLSTATUS":
                        if ((Request["VALUE1"] + "") == "草稿")  //草稿=草稿
                        {
                            where += " and DECLSTATUS=0 ";
                        }
                        if ((Request["VALUE1"] + "") == "已委托")  //已委托=已委托
                        {
                            where += " and DECLSTATUS=10 ";
                        }
                        if ((Request["VALUE1"] + "") == "申报中")  //申报中=申报中
                        {
                            where += " and DECLSTATUS=100 ";
                        }
                        if ((Request["VALUE1"] + "") == "申报完结")  //申报完结=申报完结
                        {
                            where += " and DECLSTATUS=130 ";
                        }
                        if ((Request["VALUE1"] + "") == "未完结")  //未完结
                        {
                            where += " and DECLSTATUS<130 ";
                        }
                        break;
                    case "INSPSTATUS":
                        if ((Request["VALUE1"] + "") == "草稿")  //草稿=草稿
                        {
                            where += " and INSPSTATUS=0 ";
                        }
                        if ((Request["VALUE1"] + "") == "已委托")  //已委托=已委托
                        {
                            where += " and INSPSTATUS=10 ";
                        }
                        if ((Request["VALUE1"] + "") == "申报中")  //申报中=申报中
                        {
                            where += " and INSPSTATUS=100 ";
                        }
                        if ((Request["VALUE1"] + "") == "申报完结")  //申报完结=申报完结
                        {
                            where += " and INSPSTATUS=130 ";
                        }
                        if ((Request["VALUE1"] + "") == "未完结")  //未完结
                        {
                            where += " and INSPSTATUS<130 ";
                        }
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE2"]))//判断查询条件2是否有值
            {
                switch (Request["CONDITION2"])
                {
                    case "CONTRACTNO":
                        where += " and CONTRACTNO like '%" + Request["VALUE2"] + "%' ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE3"]))//判断查询条件3是否有值
            {
                switch (Request["CONDITION3"])
                {
                    case "REPNO":
                        where += " and REPNO like '%" + Request["VALUE3"] + "%' ";
                        break;
                    case "DIVIDENO":
                        where += " and DIVIDENO like '%" + Request["VALUE3"] + "%' ";
                        break;
                    case "MANIFEST":
                        where += " and MANIFEST like '%" + Request["VALUE3"] + "%' ";
                        break;
                    case "SECONDLADINGBILLNO":
                        where += " and SECONDLADINGBILLNO like '%" + Request["VALUE3"] + "%' ";
                        break;
                }
            }

              if (!string.IsNullOrEmpty(Request["VALUE4"]))//判断查询条件2是否有值
            {
                switch (Request["CONDITION4"])
                {
                    case "CUSTOMAREACODE":
                        where += " and CUSTOMAREACODE = '" + Request["VALUE4"] + "' ";
                        break;
                    case "REPWAYID":
                        where += " and REPWAYID = '" + Request["VALUE4"] + "' ";
                        break;
                    case "PORTCODE":
                        where += " and PORTCODE = '" + Request["VALUE4"] + "' ";
                        break;
                }
            }
            where += " and ISINVALID=0 ";//?是否需要
            return where;
        }

        public string getTrack()
        {
            string busitypeid = Request["busitypeid"] + "";
            string id = Request["id"];
            string sql = string.Empty;
            sql = "select * from list_order a left join  LIST_GOOD_TRACK b on a.code=b.ordercode where a.busitype='" + busitypeid + "' and  a.id='" + id + "'";
            DataTable dt_order = DBMgr.GetDataTable(sql);
            IsoDateTimeConverter iso = new IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd";
            string json_order = JsonConvert.SerializeObject(dt_order, iso);
            string sql_container = "select * from list_predeclcontainer where ordercode='" + dt_order.Rows[0]["ORDERCODE"].ToString() + "'";
            DataTable dt_container = DBMgr.GetDataTable(sql_container);
            string json_container = JsonConvert.SerializeObject(dt_container, iso);

            return "{\"json_order\":" + json_order + ",\"json_container\":" + json_container + "}";
        
        }
        public string GoodsTrackSave()
        {
            JObject json_data = (JObject)JsonConvert.DeserializeObject(Request["data"]);
            string fieldList = string.Empty;
            string fieldValueList = string.Empty;
            foreach (JToken item in json_data.Values<JToken>())
	            {   
                    string colName = ((JProperty)item).Name;
                    string colValue = ((JProperty)item).Value.ToString();
                    if (colName!="CODE")
                    {
                         if (item.Next == null)
                        {
                             fieldList += colName;
                             fieldValueList += "'" + colValue + "'";
                        }
                        else
                        {
                             fieldList += colName + ",";
                             fieldValueList += "'" + colValue + "',";
                        }
                    }
                                 
	            }
             fieldList="ID,ORDERCODE,"+fieldList;
             fieldValueList = "LIST_GOOD_TRACK_ID.Nextval,'" + json_data.Value<string>("CODE") + "'," + fieldValueList; 
            string sql_del="delete from LIST_GOOD_TRACK where ordercode='"+json_data.Value<string>("CODE")+"'";
            int i=DBMgr.ExecuteNonQuery(sql_del);
            if (i >= 0)
            {
                DBMgr.ExecuteNonQuery("insert into LIST_GOOD_TRACK(" + fieldList + ") values(" + fieldValueList + ")");
                return "{success:true}";
            }

            return "{success:false}";
        }
    }
}
