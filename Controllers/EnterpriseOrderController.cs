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
using LumiSoft.Net;
using LumiSoft.Net.POP3.Client;
using LumiSoft.Net.Mail;
using LumiSoft.Net.MIME;
using System.IO;
using MvcPlatform.MethodSvc;
using Oracle.ManagedDataAccess.Client;
using MvcPlatform.MethodSvc;

namespace MvcPlatform.Controllers
{
    [Authorize]
    [Filters.AuthFilter]
    public class EnterpriseOrderController : Controller
    {
        int totalProperty = 0;
        Bitmap bmp_Print = null;//报关单号条码(给打印事件用)
        string Declcode = "";//报关单号(给打印事件用)

        public ActionResult ProcessOrder()//委托任务
        {
            ViewBag.navigator = "客户服务>>文件委托";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult ProcessOrder_Data()//委托任务仅显示有数据的记录
        {
            ViewBag.navigator = "客户服务>>电子数据";
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
        public ActionResult EntOrder_Data_Detail()  //文件委托
        {
            ViewBag.navigator = "企业服务>>电子数据";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult EntOrderList()  //文件委托
        {
            ViewBag.navigator = "业务管理>>文件委托";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult BatchMaintain()
        {
            return View();
        }

        [Filters.DecodeFilter]
        public ActionResult ListOrder_Index(string busitypeid)
        {
            switch (busitypeid)
            {
                case "10":
                    ViewBag.navigator = "业务管理>>空运出口";
                    break;
                case "11":
                    ViewBag.navigator = "业务管理>>空运进口";
                    break;
                case "21":
                    ViewBag.navigator = "业务管理>>海运进口";
                    break;
                case "20":
                    ViewBag.navigator = "业务管理>>海运出口";
                    break;
                case "31":
                    ViewBag.navigator = "业务管理>>陆运进口";
                    break;
                case "30":
                    ViewBag.navigator = "业务管理>>陆运出口";
                    break;
                case "40-41":
                    ViewBag.navigator = "业务管理>>国内结转";
                    break;
                case "50-51":
                    ViewBag.navigator = "业务管理>>特殊区域";
                    break;
            }

            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult GoodsTrack()  //货况跟踪
        {
            //ViewBag.navigator = "货况跟踪";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult ListPreData()
        {
            ViewBag.navigator = "订单中心>>预录导入";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult ListPreDataDetail()
        {
            ViewBag.navigator = "订单中心>>预录导入";
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
            sql += @" and (t.printstatus=1 
                            or (
                                t.printstatus!=1 and (t.status is null or (t.status is not null and t.status>5))
                                )
                    )";
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
                where += " and b.BUSIUNITCODE='" + GetCode(Request["ENTERPRISENAME"]) + "'";
            }
            if (!string.IsNullOrEmpty(Request["CODE"]))//判断查询条件是否有值
            {
                where += " and instr(b.CUSNO,'" + Request["CODE"].ToString().Trim() + "')>0 ";
            }
            //if (!string.IsNullOrEmpty(Request["PRINTSTATUS"]))//判断查询条件是否有值
            //{
            //    where += " and  t.PRINTSTATUS='" + Request["PRINTSTATUS"] + "'";
            //}
            if (!string.IsNullOrEmpty(Request["STARTDATE"]))//如果开始时间有值
            {
                where += " and b.CREATETIME>=to_date('" + Request["STARTDATE"] + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            if (!string.IsNullOrEmpty(Request["ENDDATE"]))//如果结束时间有值
            {
                where += " and b.CREATETIME<=to_date('" + Request["ENDDATE"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            //string sql = @"select t.*,(select count(1) from list_attachment l where l.entid=t.ID ) FILENUM from ENT_ORDER t where t.FILEDECLAREUNITCODE='" + json_user.Value<string>("CUSTOMERHSCODE") + "'" + where;
            //2016/10/9 为了提升load效能 FILENUM获取修改为：
            string sql = @"select *  from LIST_CUSDATA_FL b  where b.cusno is not null and b.REPUNITCODE='" + json_user.Value<string>("CUSTOMERHSCODE") + "'" + where;
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "b.CREATETIME", "desc"));
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
                string sql = "select a.*,(case printstatus when '1' then 15 else (case when status is null then 10 else status end) end) as newstatus from ENT_ORDER a where a.ID = '" + ID + "'";
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
            string busiunitcode = json_user.Value<string>("CUSTOMERHSCODE");
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
                string action = Request["action"] + "";
                string status = action == "delegate" ? "10" : json_data.Value<string>("STATUS");
                string insert_sql = "";
                string update_sql = "";
                string sql = "";
                string ent_id = "";


                if (!string.IsNullOrEmpty(json_data.Value<string>("CODE")))
                {
                    string sql_check = "select * from ENT_ORDER where CODE='" + json_data.Value<string>("CODE") + "'";
                    if (!string.IsNullOrEmpty(json_data.Value<string>("ID")))
                    {
                        sql_check += " and ID !='" + json_data.Value<string>("ID") + "'";
                    }
                    DataTable dt_itemno = new DataTable();
                    dt_itemno = DBMgr.GetDataTable(sql_check);
                    if (dt_itemno.Rows.Count > 0) { return "{success:false,isrepeate:'Y'}"; }

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
                        sql = string.Format(insert_sql, ent_id, json_data.Value<string>("FILERECEVIEUNITCODE"), json_data.Value<string>("FILERECEVIEUNITNAME"),
                              json_data.Value<string>("FILEDECLAREUNITCODE"), json_data.Value<string>("FILEDECLAREUNITNAME"),
                              json_data.Value<string>("BUSITYPEID"), json_data.Value<string>("CUSTOMDISTRICTCODE"), json_data.Value<string>("CUSTOMDISTRICTNAME"),
                              json_data.Value<string>("REPWAYID"), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"),
                              json_user.Value<string>("CUSTOMERHSCODE"), json_user.Value<string>("CUSTOMERNAME"), json_data.Value<string>("REMARK"),
                              json_data.Value<string>("CODE"), json_data.Value<string>("CREATEMODE"), status, json_data.Value<string>("ISREADPDF"), json_data.Value<string>("TEMPLATENAME"));
                        DBMgr.ExecuteNonQuery(sql);
                        //更新随附文件
                        Extension.Update_Attachment_ForEnterprise(ent_id, filedata, json_data.Value<string>("ORIGINALFILEIDS"), json_user, "");
                    }
                    if (json_data.Value<string>("CREATEMODE") == "按文件")
                    {
                        JArray jarry = JsonConvert.DeserializeObject<JArray>(filedata);
                        foreach (JObject json in jarry)
                        {
                            sql = "select ENT_ORDER_ID.Nextval from dual";
                            ent_id = DBMgr.GetDataTable(sql).Rows[0][0] + "";//获取ID
                            sql = string.Format(insert_sql, ent_id, json_data.Value<string>("FILERECEVIEUNITCODE"), json_data.Value<string>("FILERECEVIEUNITNAME"),
                                  json_data.Value<string>("FILEDECLAREUNITCODE"), json_data.Value<string>("FILEDECLAREUNITNAME"),
                                  json_data.Value<string>("BUSITYPEID"), json_data.Value<string>("CUSTOMDISTRICTCODE"), json_data.Value<string>("CUSTOMDISTRICTNAME"),
                                  json_data.Value<string>("REPWAYID"), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"),
                                  json_user.Value<string>("CUSTOMERHSCODE"), json_user.Value<string>("CUSTOMERNAME"), json_data.Value<string>("REMARK"),
                                  json_data.Value<string>("CODE"), json_data.Value<string>("CREATEMODE"), status, json_data.Value<string>("ISREADPDF"), json_data.Value<string>("TEMPLATENAME"));
                            DBMgr.ExecuteNonQuery(sql);
                            //更新随附文件
                            Extension.Update_Attachment_ForEnterprise(ent_id, "[" + JsonConvert.SerializeObject(json) + "]", json_data.Value<string>("ORIGINALFILEIDS"), json_user, "");
                        }
                    }



                }
                else
                {
                    if (action == "ch")
                    {
                        update_sql = @"update ENT_ORDER  set status='5' where id='" + json_data.Value<string>("ID") + "'";
                        DBMgr.ExecuteNonQuery(update_sql);
                        return "{success:true}";
                    }

                    if (action == "delegate")
                    {
                        update_sql = @"update ENT_ORDER  set filerecevieunitcode='{1}',filerecevieunitname='{2}',filedeclareunitcode='{3}',
                            filedeclareunitname='{4}',busitypeid='{5}',customdistrictcode='{6}', customdistrictname='{7}',
                            repwayid='{8}',remark='{9}',code='{10}',status='{11}',templatename='{12}',submittime=sysdate where id='{0}'";
                    }
                    else
                    {
                        update_sql = @"update ENT_ORDER  set filerecevieunitcode='{1}',filerecevieunitname='{2}',filedeclareunitcode='{3}',
                        filedeclareunitname='{4}',busitypeid='{5}',customdistrictcode='{6}', customdistrictname='{7}',
                        repwayid='{8}',remark='{9}',code='{10}',status='{11}',templatename='{12}' where id='{0}'";
                    }
                    sql = string.Format(update_sql, json_data.Value<string>("ID"), json_data.Value<string>("FILERECEVIEUNITCODE"),
                    json_data.Value<string>("FILERECEVIEUNITNAME"), json_data.Value<string>("FILEDECLAREUNITCODE"),
                    json_data.Value<string>("FILEDECLAREUNITNAME"), json_data.Value<string>("BUSITYPEID"), json_data.Value<string>("CUSTOMDISTRICTCODE"),
                    json_data.Value<string>("CUSTOMDISTRICTNAME"), json_data.Value<string>("REPWAYID"), json_data.Value<string>("REMARK"), json_data.Value<string>("CODE"), status, json_data.Value<string>("TEMPLATENAME"));
                    DBMgr.ExecuteNonQuery(sql);
                    //更新随附文件
                    Extension.Update_Attachment_ForEnterprise(json_data.Value<string>("ID"), filedata, json_data.Value<string>("ORIGINALFILEIDS"), json_user, "");
                }
                return "{success:true}";
            }
            catch (Exception ex)
            {
                return "{success:false}";
            }
        }
        public string DeleteList(string recs)
        {
            bool flag = true; string UNITCODE = string.Empty;
            JArray ja = JArray.Parse(recs);
            string message = string.Empty;
            foreach (JToken jt in ja)
            {
                if (jt.Value<string>("NEWSTATUS") == "5")
                {
                    flag = Delete(jt.Value<string>("ID"));
                    if (!flag)
                    {
                        UNITCODE += jt.Value<string>("UNITCODE") + " ";
                    }
                }
                else
                {
                    message = "某些记录为非草稿状态，本次操作只删除草稿状态的记录！";
                }
            }
            if (UNITCODE != "")
            {

                message = "如下编号:" + UNITCODE + "的记录删除失败。";
                return "{success:false,message:'" + message + "'}";
            }
            else
            {
                return "{success:true,message:'" + message + "'}";
            }

        }
        public bool Delete(string id)
        {
            try
            {
                string sql = "";

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
            }
            catch (Exception)
            {

                return false;
            }
            return true;

        }
        public string loadOrderList()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string enterprisecode = json_user.Value<string>("CUSTOMERHSCODE");
            string sql = "select t.*,l.FILENUM ,(case printstatus when '1' then 15 else (case when status is null then 10 else status end) end) as newstatus " +
                       "from ENT_ORDER t left join (select entid,count(1) as FILENUM from list_attachment where entid is not null group by entid) l on t.ID=l.entid " +
                       "where t.enterprisecode=" + enterprisecode;
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
                where += " and instr(t.CODE,'" + Request["CODE"] + "')>0";
            }
            if (!string.IsNullOrEmpty(Request["STARTDATE"]))
            {
                where += " and t.SUBMITTIME>=to_date('" + Request["STARTDATE"] + "','yyyy-mm-dd hh24:mi:ss')";
            }
            if (!string.IsNullOrEmpty(Request["ENDDATE"]))
            {
                where += " and t.SUBMITTIME<=to_date('" + Request["ENDDATE"] + "','yyyy-mm-dd hh24:mi:ss')+1";
            }
            if (!string.IsNullOrEmpty(Request["STATUS"]))
            {
                string status = Request["STATUS"];
                switch (status)
                {
                    case "10":
                        where += " and (t.STATUS='" + Request["STATUS"] + "' or (t.STATUS is null and t.printstatus!='1' ))";
                        break;
                    case "15":
                        where += " and (t.STATUS='" + Request["STATUS"] + "' or  t.printstatus='1')";
                        break;
                    default:
                        where += " and t.STATUS='" + Request["STATUS"] + "'";
                        break;

                }

            }
            sql += where;
            IsoDateTimeConverter iso = new IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "CREATETIME desc,id ", "desc"));
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



            DataTable dtExcel = Extension.GetExcelData_Table(Server.MapPath(filepath),0);


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

            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string LoadId_index()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string where = string.Empty;
            if (!string.IsNullOrEmpty(Request["cx_value"]))
            {
                switch (Request["item_combo_value"])
                {

                    case "ddbh":
                        where += " and CODE='" + Request["cx_value"] + "'";
                        break;
                    case "khbh":
                        where += " and CUSNO='" + Request["cx_value"] + "'";
                        break;
                }

            }
            string item_combo_value = Request["item_combo_value"] + "";
            string cx_value = Request["cx_value"] + "";
            string sql = @"select * from LIST_ORDER where instr('" + Request["busitypeid"] + "',BUSITYPE)>0 and (BUSIUNITCODE='" + json_user.Value<string>("CUSTOMERHSCODE") + "' or CUSTOMERCODE='" + json_user.Value<string>("CUSTOMERCODE") + "') " + where;
            DataTable dt = DBMgr.GetDataTable(sql);
            var json = JsonConvert.SerializeObject(dt, iso);

            return json;
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
                    case "LOGISTICSSTATUS":
                        if (!string.IsNullOrEmpty(Request["VALUE1"]))
                        {
                            where += " and LOGISTICSSTATUS='" + Request["VALUE1"] + "'";
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
            if (!string.IsNullOrEmpty(Request["STARTDATE"]))
            {
                where += " and SUBMITTIME>=to_date('" + Request["STARTDATE"] + "','yyyy-mm-dd hh24:mi:ss')";
            }
            if (!string.IsNullOrEmpty(Request["ENDDATE"]))
            {
                where += " and SUBMITTIME<=to_date('" + Request["ENDDATE"] + "','yyyy-mm-dd hh24:mi:ss')+1";
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
            string sql_container = "select * from list_predeclcontainer where ordercode='" + dt_order.Rows[0]["CODE"].ToString() + "'";
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
                if (colName != "CODE")
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
            fieldList = "ID,ORDERCODE," + fieldList;
            fieldValueList = "LIST_GOOD_TRACK_ID.Nextval,'" + json_data.Value<string>("CODE") + "'," + fieldValueList;
            string sql_del = "delete from LIST_GOOD_TRACK where ordercode='" + json_data.Value<string>("CODE") + "'";
            int i = DBMgr.ExecuteNonQuery(sql_del);
            if (i >= 0)
            {
                DBMgr.ExecuteNonQuery("insert into LIST_GOOD_TRACK(" + fieldList + ") values(" + fieldValueList + ")");
                return "{success:true}";
            }

            return "{success:false}";
        }

        public string getStatusName(string curstatus, string dec_insp_status)
        {
            string statusname = "";
            JArray jarray = JArray.Parse(dec_insp_status);
            foreach (JObject json in jarray)
            {
                if (json.Value<string>("CODE") == curstatus) { statusname = json.Value<string>("NAME"); break; }
            }
            return statusname;
        }

        public string ExportList()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string where = QueryCondition();
            string dec_insp_status = Request["dec_insp_status"];
            string busitypeid = Request["busitypeid"];
            string busitype = "";

            switch (busitypeid)
            {
                case "11":
                    busitype = "空运进口";
                    break;
                case "10":
                    busitype = "空运出口";
                    break;
                case "21":
                    busitype = "海运进口";
                    break;
                case "20":
                    busitype = "海运出口";
                    break;
                case "31":
                    busitype = "陆运进口";
                    break;
                case "30":
                    busitype = "陆运出口";
                    break;
                case "40,41":
                    busitype = "国内";
                    break;
                case "50,51":
                    busitype = "特殊区域";
                    break;
            }

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string sql = @"select * from LIST_ORDER a  left join (select CODE,NAME||'('||CODE||')' REPWAYNAME from cusdoc.SYS_REPWAY where Enabled=1 and instr(busitype,'" + busitype + "')>0) b on a.REPWAYID=b.CODE "
                      + "where instr('" + Request["busitypeid"] + "',BUSITYPE)>0 and BUSIUNITCODE='" + json_user.Value<string>("CUSTOMERHSCODE") + "' " + where;

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
            string filename = "订单文件";

            #region 11 空进
            if (busitypeid == "11")//空进
            {
                sheet_S = book.CreateSheet("订单信息_空进"); filename = filename + "_空进.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("对应号");
                row1.CreateCell(4).SetCellValue("客户编号"); row1.CreateCell(5).SetCellValue("合同发票号"); row1.CreateCell(6).SetCellValue("总单号"); row1.CreateCell(7).SetCellValue("分单号");
                row1.CreateCell(8).SetCellValue("件数/重量"); row1.CreateCell(9).SetCellValue("打印状态"); row1.CreateCell(10).SetCellValue("申报关区"); row1.CreateCell(11).SetCellValue("进/出口岸");
                row1.CreateCell(12).SetCellValue("申报方式"); row1.CreateCell(13).SetCellValue("转关预录号"); row1.CreateCell(14).SetCellValue("法检");row1.CreateCell(15).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["REPNO"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CUSNO"].ToString());


                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["TOTALNO"].ToString());
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["DIVIDENO"].ToString());

                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(8).SetCellValue("");
                    }
                    rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["TURNPRENO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());
                    

                }
            }
            #endregion

            #region 10 空出
            if (busitypeid == "10")//空出
            {
                sheet_S = book.CreateSheet("订单信息_空出"); filename = filename + "_空出.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("对应号");
                row1.CreateCell(4).SetCellValue("客户编号"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("总单号"); row1.CreateCell(7).SetCellValue("分单号");
                row1.CreateCell(8).SetCellValue("件数/重量"); row1.CreateCell(9).SetCellValue("打印状态"); row1.CreateCell(10).SetCellValue("申报关区"); row1.CreateCell(11).SetCellValue("进/出口岸");
                row1.CreateCell(12).SetCellValue("申报方式"); row1.CreateCell(13).SetCellValue("运抵编号"); row1.CreateCell(14).SetCellValue("法检"); row1.CreateCell(15).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["REPNO"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["TOTALNO"].ToString());
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["DIVIDENO"].ToString());

                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(8).SetCellValue("");
                    }
                    rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["ARRIVEDNO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());
                }
            }
            #endregion

            #region 21 海进
            if (busitypeid == "21")//海进
            {
                sheet_S = book.CreateSheet("订单信息_海进"); filename = filename + "_海进.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("对应号");
                row1.CreateCell(4).SetCellValue("客户编号"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("打印状态"); row1.CreateCell(7).SetCellValue("国检提单号");
                row1.CreateCell(8).SetCellValue("海关提单号"); row1.CreateCell(9).SetCellValue("件数/重量"); row1.CreateCell(10).SetCellValue("申报关区"); row1.CreateCell(11).SetCellValue("进/出口岸");
                row1.CreateCell(12).SetCellValue("申报方式"); row1.CreateCell(13).SetCellValue("转关预录号"); row1.CreateCell(14).SetCellValue("法检"); row1.CreateCell(15).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["REPNO"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["FIRSTLADINGBILLNO"].ToString());

                    rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["SECONDLADINGBILLNO"].ToString());
                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(9).SetCellValue("");
                    }
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["TURNPRENO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());
                }
            }
            #endregion

            #region 20 海出
            if (busitypeid == "20")//海出
            {
                sheet_S = book.CreateSheet("订单信息_海出"); filename = filename + "_海出.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("对应号");
                row1.CreateCell(4).SetCellValue("客户编号"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("提单号"); row1.CreateCell(7).SetCellValue("打印状态");
                row1.CreateCell(8).SetCellValue("运抵编号"); row1.CreateCell(9).SetCellValue("件数/重量"); row1.CreateCell(10).SetCellValue("申报关区"); row1.CreateCell(11).SetCellValue("进/出口岸");
                row1.CreateCell(12).SetCellValue("申报方式"); row1.CreateCell(13).SetCellValue("转关预录号"); row1.CreateCell(14).SetCellValue("法检"); row1.CreateCell(15).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["REPNO"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["SECONDLADINGBILLNO"].ToString());
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");

                    rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["ARRIVEDNO"].ToString());
                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(9).SetCellValue("");
                    }
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["TURNPRENO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());
                }
            }
            #endregion

            #region 31 陆进
            if (busitypeid == "31")//陆进
            {
                sheet_S = book.CreateSheet("订单信息_陆进"); filename = filename + "_陆进.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("对应号");
                row1.CreateCell(4).SetCellValue("客户编号"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("分单号"); row1.CreateCell(7).SetCellValue("打印状态");
                row1.CreateCell(8).SetCellValue("件数/重量"); row1.CreateCell(9).SetCellValue("申报关区"); row1.CreateCell(10).SetCellValue("进/出口岸"); row1.CreateCell(11).SetCellValue("申报方式");
                row1.CreateCell(12).SetCellValue("法检"); row1.CreateCell(13).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["REPNO"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CUSNO"].ToString());
                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["DIVIDENO"].ToString());
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");

                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(8).SetCellValue("");
                    }
                    rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());
                }
            }
            #endregion

            #region 30 陆出
            if (busitypeid == "30")//陆出
            {
                sheet_S = book.CreateSheet("订单信息_陆出"); filename = filename + "_陆出.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("客户编号");
                row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("总单号"); row1.CreateCell(7).SetCellValue("分单号");
                row1.CreateCell(8).SetCellValue("打印状态"); row1.CreateCell(9).SetCellValue("件数/重量"); row1.CreateCell(10).SetCellValue("申报关区"); row1.CreateCell(11).SetCellValue("进/出口岸");
                row1.CreateCell(12).SetCellValue("申报方式"); row1.CreateCell(13).SetCellValue("运抵编号"); row1.CreateCell(14).SetCellValue("转关预录号"); row1.CreateCell(15).SetCellValue("法检");
                row1.CreateCell(16).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CUSNO"].ToString());
                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["TOTALNO"].ToString());
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["DIVIDENO"].ToString());

                    rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");
                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(9).SetCellValue("");
                    }
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["ARRIVEDNO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["TURNPRENO"].ToString());
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(16).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());

                }
            }
            #endregion

            #region 40,41 国内
            if (busitypeid == "40-41")//国内
            {
                sheet_S = book.CreateSheet("订单信息_国内"); filename = filename + "_国内.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("客户编号");
                row1.CreateCell(4).SetCellValue("对应号"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("件数/重量");
                row1.CreateCell(7).SetCellValue("打印状态"); row1.CreateCell(8).SetCellValue("申报关区"); row1.CreateCell(9).SetCellValue("申报方式"); row1.CreateCell(10).SetCellValue("法检");
                row1.CreateCell(11).SetCellValue("业务类型"); row1.CreateCell(12).SetCellValue("两单关联号"); row1.CreateCell(13).SetCellValue("多单关联号"); row1.CreateCell(14).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CUSNO"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["REPNO"].ToString());

                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(6).SetCellValue("");
                    }
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");

                    rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    if (dt.Rows[i]["BUSITYPE"].ToString() == "40")
                    {
                        rowtemp.CreateCell(11).SetCellValue("国内出口");
                    }
                    else if (dt.Rows[i]["BUSITYPE"].ToString() == "41")
                    {
                        rowtemp.CreateCell(11).SetCellValue("国内进口");
                    }
                    else
                    {
                        rowtemp.CreateCell(11).SetCellValue("");
                    }

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["ASSOCIATENO"].ToString());
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["CORRESPONDNO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());

                }
            }
            #endregion

            #region 50,51 特殊
            if (busitypeid == "50-51")//特殊
            {
                sheet_S = book.CreateSheet("订单信息_特殊"); filename = filename + "_特殊.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("对应号");
                row1.CreateCell(4).SetCellValue("客户编号"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("打印状态"); row1.CreateCell(7).SetCellValue("申报方式");
                row1.CreateCell(8).SetCellValue("件数/重量"); row1.CreateCell(9).SetCellValue("申报关区"); row1.CreateCell(10).SetCellValue("进/出口岸"); row1.CreateCell(11).SetCellValue("转关预录号");
                row1.CreateCell(12).SetCellValue("法检"); row1.CreateCell(13).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["REPNO"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID

                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(8).SetCellValue("");
                    }
                    rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["TURNPRENO"].ToString());

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());
                }
            }
            #endregion

            // 写入到客户端 
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //book.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", filename);

            return Extension.getPathname(filename, book);
        }

        #region
        public string mailImport()
        {
            string mailUserName = ConfigurationManager.AppSettings["mailUserName"];
            string mailPassword = ConfigurationManager.AppSettings["mailPassword"];
            string mailAddress = ConfigurationManager.AppSettings["mailAddress"];
            string sendAddress = Request["sendAddress"] + "";
            string filedata = string.Empty;
            JArray JA = new JArray();
            using (POP3_Client client = new POP3_Client())
            {

                client.Connect(mailAddress, 110, false);
                client.Login(mailUserName, mailPassword);//两个参数，前者为Email的账号，后者为Email的密码
                POP3_ClientMessageCollection messages = client.Messages;
                // Console.WriteLine("共{0}封邮件", messages.Count);

                foreach (POP3_ClientMessage message in messages)
                {


                    Mail_Message mime = Mail_Message.ParseFromByte(message.MessageToByte());

                    //string address = mime.Sender == null ? mime.ReturnPath.Address : mime.Sender.Address;//发件人地址
                    string address = string.Empty;//发件人地址 
                    if (mime.Sender != null)
                    {
                        address = mime.Sender.Address;
                    }
                    else if (mime.ReturnPath != null)
                    {
                        address = mime.ReturnPath.Address;
                    }

                    if (address == sendAddress)
                    {
                        JArray JA_file = new JArray();
                        var uploadPath = Server.MapPath("/FileUpload/file/");
                        foreach (MIME_Entity entity in mime.Attachments)
                        {


                            if (entity.ContentDisposition != null && entity.ContentDisposition.Param_FileName != null)
                            {
                                string remote = DateTime.Now.ToString("yyyy-MM-dd");
                                string ORIGINALNAME = entity.ContentDisposition.Param_FileName;
                                string NEWNAME = Guid.NewGuid().ToString() + ORIGINALNAME.Substring(ORIGINALNAME.IndexOf("."));
                                string filePath = "/FileUpload/file/" + NEWNAME;
                                string savePath = uploadPath + NEWNAME;
                                int size = ((MIME_b_SinglepartBase)entity.Body).Data.Length;
                                System.IO.File.WriteAllBytes(savePath, ((MIME_b_SinglepartBase)entity.Body).Data);
                                string fileContent = "{\"FILENAME\":\"" + filePath + "\",\"NEWNAME\":\"" + NEWNAME + "\",\"ORIGINALNAME\":\"" + ORIGINALNAME + "\",\"SIZES\":\"" + size + "\",\"UPLOADTIME\":\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\",\"FILETYPE\":\"44\"}";
                                JObject JO_file = JObject.Parse(fileContent);
                                JA_file.Add(JO_file);

                            }


                        }
                        filedata = JsonConvert.SerializeObject(JA_file);
                        JObject JO = JObject.Parse("{\"SUBJECT\":\"" + mime.Subject + "\",\"FILECOUNT\":\"" + mime.Attachments.Length + "\",\"FILEDATA\":" + filedata + "}");
                        JA.Add(JO);
                        message.MarkForDeletion();//删除邮件,测试时先不删除  
                    }

                }

            }
            string result = string.Empty;
            if (JA.Count == 0)
            {
                result = "{success:true}";
            }
            else
            {
                result = JsonConvert.SerializeObject(JA);
            }
            return result;

        }

        public string import()
        {
            try
            {
                string records = Request["records"];
                JArray ja_data = JArray.Parse(records);
                JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
                foreach (JObject item in ja_data)
                {

                    string filedata = JsonConvert.SerializeObject(item.Value<JArray>("FILEDATA"));
                    string insert_sql = @"insert into ENT_ORDER (id,createtime, unitcode,createid, createname,enterprisecode, enterprisename,status,isreadpdf,templatename,createmode) 
                                     values ('{0}',sysdate,(select fun_AutoQYBH(sysdate) from dual),'{1}','{2}', '{3}','{4}',5,0,'','按批次')";

                    string sql = "select ENT_ORDER_ID.Nextval from dual";
                    string ent_id = DBMgr.GetDataTable(sql).Rows[0][0] + "";//获取ID
                    sql = string.Format(insert_sql, ent_id, json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"), json_user.Value<string>("CUSTOMERHSCODE"), json_user.Value<string>("CUSTOMERNAME"));
                    DBMgr.ExecuteNonQuery(sql);
                    Extension.Update_Attachment_ForEnterprise(ent_id, filedata, "", json_user, "mailimport/" + DateTime.Now.ToString("yyyy-MM-dd"));
                }
                return "{success:true}";
            }
            catch (Exception)
            {

                return "{success:false}";
            }


        }
        #endregion
        public string load_entorder_Data_detail()
        {
            string result = string.Empty;
            string ID = Request["ID"]; //string busitype = Request["busitype"];
            string sql = string.Empty;
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            //申报数据
            string decl_data = "[]";
            string product_data = "[]";
            string file_data = "[]";

            sql = "select * from LIST_CUSDATA_FL where ID='" + ID + "'";
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
            result = "{decl_data:" + decl_data + ",product_data:" + product_data + ",file_data:" + file_data + "}";
            return result;

        }

        public string convertPdf()
        {
            string oldfilename = Request["filePath"] + "";
            string newfilename = "view_" + Guid.NewGuid() + ".pdf";
            string filePath = "D:/ftpserver" + oldfilename;
            string toPath = Server.MapPath("/Declare") + "/" + newfilename;
            bool print = true;

            PdfReader reader = new PdfReader(filePath);
            Document document = new Document(reader.GetPageSizeWithRotation(1));
            int n = reader.NumberOfPages;
            FileStream baos = new FileStream(toPath, FileMode.Create, FileAccess.Write);
            PdfCopy copy = new PdfCopy(document, baos);
            copy.ViewerPreferences = PdfWriter.HideToolbar | PdfWriter.HideMenubar;
            //往pdf中写入内容   
            document.Open();
            for (int i = 1; i <= n; i++)
            {
                PdfImportedPage page = copy.GetImportedPage(reader, i);
                copy.AddPage(page);
            }
            if (print)
            {
                PdfAction.JavaScript("myOnMessage();", copy);
                copy.AddJavaScript("this.print(true);function myOnMessage(aMessage) {app.alert('Test',2);} var msgHandlerObject = new Object();doc.onWillPrint = myOnMessage;this.hostContainer.messageHandler = msgHandlerObject;");
            }
            document.Close();
            reader.Close();

            return "{newfilepath:\"" + newfilename + "\"}";
        }

        public void updatePrintStatus()
        {
            string ID = Request["ID"];
            string sql = "update ENT_ORDER set PRINTSTATUS='1' where ID = '" + ID + "'";
            DBMgr.ExecuteNonQuery(sql);

        }
        public string LoadList_logistic(string totalno, string divdeno)
        {
            string sql = "select * from list_logisticsstatus where totalno='" + totalno + "' and divideno='" + divdeno + "'  order by operate_type,operate_date desc";
            DataTable dt = DBMgr.GetDataTable(sql);
            IsoDateTimeConverter iso = new IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string result_data = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + result_data + "}";

        }

        public string getLogisticStatus(string totalno, string divdeno)
        {
            MethodServiceClient msc = new MethodServiceClient();
            string result = msc.Update_Blno_Status(totalno, divdeno);
            return result;
        }


        #region 预录导入

        public string QueryConditionPreData()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string where = "";
            if (!string.IsNullOrEmpty(Request["CUSNO"]))
            {
                where += " and instr(lp.CUSNO,'" + Request["CUSNO"] + "')>0";
            }
            if (!string.IsNullOrEmpty(Request["FLAG"]))
            {
                where += " and lp.FLAG='" + Request["FLAG"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["STARTDATE"]))
            {
                where += " and lp.REPDATE>=to_date('" + Request["STARTDATE"] + "','yyyy-mm-dd hh24:mi:ss')";
            }
            if (!string.IsNullOrEmpty(Request["ENDDATE"]))
            {
                where += " and lp.REPDATE<=to_date('" + Request["ENDDATE"] + "','yyyy-mm-dd hh24:mi:ss')+1";
            }

            where += @" and lp.customercode ='" + json_user.Value<string>("CUSTOMERCODE") + "' ";

            string sql = @"select lp.*,lo.busitype,ld.STATUS,ld.CUSTOMSSTATUS,ld.ordercode,ld.code,lda.declarationcode 
                        from list_predata lp 
                            left join list_order lo on lp.cusno=lo.cusno and lo.ISINVALID=0 
                            left join list_declaration ld on lp.cusno=ld.cusno and ld.ISINVALID=0
                            left join list_declaration_after lda on ld.code=lda.code and lda.csid=1";

            sql += " where lp.ISINVALID=0 " + where;

            return sql;

        }

        public string loadpredata()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd";
            string sql = QueryConditionPreData();

            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "lp.CREATETIME", "DESC"));
            var json = JsonConvert.SerializeObject(dt, iso);

            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string getStatusCode(string name, JArray jarray)
        {
            string code = "";
            foreach (JObject json in jarray)
            {
                if (json.Value<string>("NAME").Substring(0, json.Value<string>("NAME").IndexOf("(")) == name)
                {
                    code = json.Value<string>("CODE");
                    break;
                }
            }
            return code;
        }
        public string getStatusCodebyname(string name, JArray jarray)
        {
            string code = "";
            foreach (JObject json in jarray)
            {
                if (json.Value<string>("NAME") == name)
                {
                    code = json.Value<string>("CODE");
                    break;
                }
            }
            return code;
        }

        public string ImportExcelData()
        {
            string formdata = Request["formdata"]; string action = Request["action"]; string cusno = Request["cusno"];
            JObject json_formdata = (JObject)JsonConvert.DeserializeObject(formdata);

            HttpPostedFileBase postedFile = Request.Files["UPLOADFILE"];//获取上传信息对象  
            string fileName = Path.GetFileName(postedFile.FileName);

            string newfile = @"~/FileUpload/PreData/" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + fileName;
            if (!Directory.Exists(Server.MapPath("~/FileUpload/PreData")))
            {
                Directory.CreateDirectory(Server.MapPath("~/FileUpload/PreData"));
            }
            postedFile.SaveAs(Server.MapPath(newfile));

            string result = "";
            if (json_formdata.Value<string>("RADIO_MODULE") == "1")
            {
                result = ModuleOne(newfile, fileName, action, cusno, json_formdata);
            }
            if (json_formdata.Value<string>("RADIO_MODULE") == "2")
            {
                result = ModuleTwo(newfile, fileName, action, cusno, json_formdata);
            }

            if (result != "{success:true}")//上传不成功，删除源文件
            {
                FileInfo fi = new FileInfo(Server.MapPath(newfile));
                if (fi.Exists)
                {
                    fi.Delete();
                }
            }

            return result;
        }

        //获取企业编号
        public string getcusno()
        {
            string cusno = "";

            string prefix = DateTime.Now.ToString("yyyyMMdd");
            OracleParameter[] parms = new OracleParameter[3];
            parms[0] = new OracleParameter("p_prefix", OracleDbType.NVarchar2, prefix, ParameterDirection.Input);
            parms[1] = new OracleParameter("p_type", OracleDbType.NVarchar2, "cusno", ParameterDirection.Input);
            parms[2] = new OracleParameter("p_increase", OracleDbType.Int32, ParameterDirection.Output);

            DBMgr.ExecuteNonQueryParm("PRO_Sequencegenerator_Web", parms);
            cusno = "CUS" + prefix + Convert.ToInt32(parms[2].Value.ToString()).ToString("0000");

            return cusno;
        }

        //作废旧的临时表
        public void ISINVALID_Predata(string cusno, OracleConnection conn)
        {
            DBMgr.ExecuteNonQuery("update list_predata set ISINVALID=1 where cusno='" + cusno + "'", conn);
            DBMgr.ExecuteNonQuery("update list_predata_sub set ISINVALID=1 where pcode='" + cusno + "'", conn);
        }

        //撤回：更新上传、撤回按钮调用
        public void cancel_pub(string cusno, OracleConnection conn)
        {
            DBMgr.ExecuteNonQuery(@"update list_decllist set ISINVALID=1
                                    where predeclcode=(select code from  list_declaration where ordercode=(select code from list_order where cusno='" + cusno + "' and ISINVALID=0) and ISINVALID=0)", conn);
            DBMgr.ExecuteNonQuery("update list_declaration set ISINVALID=1 where ordercode=(select code from list_order where cusno='" + cusno + "' and ISINVALID=0)", conn);
            DBMgr.ExecuteNonQuery("update list_order set ISINVALID=1 where cusno='" + cusno + "' and ISINVALID=0", conn);
        }

        public string ModuleOne(string newfile, string fileName, string action, string cusno, JObject json_formdata)
        {
            DataTable dtExcel = Extension.GetExcelData_Table(Server.MapPath(newfile),0);

            //Rows：表头11行+一行表体列名+至少一行表体数据；Columns：18列
            if (dtExcel == null || dtExcel.Rows.Count < 13 || dtExcel.Columns.Count != 17)
            {
                return "{success:false,error:'No Data'}";
            }
            //验证列名称
            int colcount = 0;
            for (int i = 0; i < dtExcel.Columns.Count; i++)
            {
                if (dtExcel.Columns[i].ColumnName == "报关单证录入") { colcount++; }
            }

            if (colcount == 0)
            {
                return "{success:false,error:'No Columns'}";
            }
            //====================================================================================================
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);


            //类型//地址//电话//客户全称
            string var1 = dtExcel.Rows[0][1].ToString(); string var2 = dtExcel.Rows[0][3].ToString(); string var3 = dtExcel.Rows[0][12].ToString(); string var4 = dtExcel.Rows[1][2].ToString();
            //客户地址//客户电话//发票号码//发票日期
            string var5 = dtExcel.Rows[1][7].ToString(); string var6 = dtExcel.Rows[1][14].ToString(); string var7 = dtExcel.Rows[2][2].ToString(); string var8 = dtExcel.Rows[2][5].ToString();
            //合同号码//合同日期//签约地//报关单号
            string var9 = dtExcel.Rows[2][8].ToString(); string var10 = dtExcel.Rows[2][12].ToString(); string var11 = dtExcel.Rows[2][15].ToString(); string var12 = dtExcel.Rows[3][2].ToString();
            //放行日期//退税日期//航次号//运输工具
            string var13 = dtExcel.Rows[3][5].ToString(); string var14 = dtExcel.Rows[3][9].ToString(); string var15 = dtExcel.Rows[3][12].ToString(); string var16 = dtExcel.Rows[4][2].ToString();

            //提运单号//出口日期//申报日期//贸易国
            string var17 = dtExcel.Rows[4][5].ToString(); string var18 = dtExcel.Rows[4][9].ToString(); string var19 = dtExcel.Rows[4][11].ToString(); string var20 = dtExcel.Rows[4][14].ToString();
            //经营单位//出口口岸//备案号//运输方式
            string var21 = dtExcel.Rows[5][2].ToString(); string var21_1 = dtExcel.Rows[5][4].ToString();
            string var22 = dtExcel.Rows[5][10].ToString(); string var23 = dtExcel.Rows[5][13].ToString(); string var24 = dtExcel.Rows[5][15].ToString();
            //发货单位//贸易方式//征免性质//运抵国
            string var25 = dtExcel.Rows[6][2].ToString(); string var25_1 = dtExcel.Rows[6][4].ToString();
            string var26 = dtExcel.Rows[6][10].ToString(); string var27 = dtExcel.Rows[6][13].ToString(); string var28 = dtExcel.Rows[7][3].ToString();
            //指运港//境内货源地//成交方式//运费
            string var29 = dtExcel.Rows[7][8].ToString(); string var30 = dtExcel.Rows[7][13].ToString(); string var31 = dtExcel.Rows[8][2].ToString();
            string var32 = dtExcel.Rows[8][5].ToString(); string var32_1 = dtExcel.Rows[8][6].ToString(); string var32_2 = dtExcel.Rows[8][7].ToString();

            //保费//杂费//许可证号//件数
            string var33 = dtExcel.Rows[8][9].ToString(); string var33_1 = dtExcel.Rows[8][10].ToString(); string var33_2 = dtExcel.Rows[8][11].ToString();
            string var34 = dtExcel.Rows[8][13].ToString(); string var34_1 = dtExcel.Rows[8][14].ToString(); string var34_2 = dtExcel.Rows[8][15].ToString();
            string var35 = dtExcel.Rows[9][2].ToString(); string var36 = dtExcel.Rows[9][5].ToString();
            //包装种类//毛重//净重//备注
            string var37 = dtExcel.Rows[9][9].ToString(); string var38 = dtExcel.Rows[9][11].ToString(); string var39 = dtExcel.Rows[9][14].ToString(); string var40 = dtExcel.Rows[10][2].ToString();
            //特殊关系确认//价格影响确认//支付特许权使用费确认//报关类型
            string var41 = dtExcel.Rows[10][8].ToString(); string var42 = dtExcel.Rows[10][10].ToString(); string var43 = dtExcel.Rows[10][13].ToString(); string var44 = dtExcel.Rows[10][15].ToString();
            if (var41 == "是") { var41 = "1"; } else if (var41 == "否") { var41 = "0"; } else { var41 = "9"; }
            if (var42 == "是") { var42 = "1"; } else if (var42 == "否") { var42 = "0"; } else { var42 = "9"; }
            if (var43 == "是") { var43 = "1"; } else if (var43 == "否") { var43 = "0"; } else { var43 = "9"; }


            //基础资料 获取code
            //string jsonstr = Extension.Ini_Base_Data(json_user, "predata", "");
            //JObject json = (JObject)JsonConvert.DeserializeObject(jsonstr);

            //贸易方式//出口口岸
            //string var26_code = getStatusCode(var26, json.Value<JArray>("myfs"));
            //string var22_code = getStatusCode(var22, json.Value<JArray>("sbgq"));
            //报关类型string var44_code = getStatusCode(var44, json.Value<JArray>("bgfs"));


            //企业编号            
            if (action == "add") { cusno = getcusno(); }

            //类型//合同号码//航次号//运输工具//提运单号//出口日期
            //申报日期//贸易国//出口口岸//备案号//运输方式
            //发货单位//贸易方式//征免性质//运抵国
            //指运港//境内货源地//成交方式//运费
            //保费//杂费
            //许可证号//件数//包装种类//毛重//净重
            //报关类型//文件路径//原始文件名//委托单位代码//委托单位名称
            //备注//特殊关系确认//价格影响确认//支付特许权使用费确认
            string sql = @"insert into list_predata(ID,ISINVALID,CREATETIME,FLAG,CUSNO
                            ,INOUTTYPE,CONTRACTNO,VOYAGENO,TRANSNAME,BLNO,INOUTDATE
                            ,REPDATE,TRADECOUNTRYNAME,PORTNAME,RECORDCODE,TRANSMODEL
                            ,BUSIUNITCODE,BUSIUNITNAME,TRADENAME,EXEMPTIONNAME,SECOUNTRYNAME
                            ,SEPORTNAME,SEPLACENAME,TRADETERMSNAME,FGCODE,FGNAME
                            ,FREIGHT,FGUNITNAME,IPCODE,IPNAME,INSURANCEPREMIUM
                            ,IPUNITNAME,AECODE,AENAME,ADDITIONALEXPENSES,AEUNITNAME
                            ,LICENSENO,GOODSNUM,PACKAGENAME,GOODSGW,GOODSNW
                            ,DECLWAY,FILEPATH,OLDFILENAME,CUSTOMERCODE,CUSTOMERNAME
                            ,REMARK,SPECIALRELATION,PRICEIMPACT,PAYPOYALTIES,DOCSERVICECODE
                            ,DOCSERVICENAME
                        ) VALUES ( LIST_PREDATA_ID.Nextval,0,sysdate,0,'" + cusno + @"'
                            ,'{0}','{1}','{2}','{3}','{4}',to_date('{5}','yyyy/mm/dd')
                            ,to_date('{6}','yyyy/mm/dd'),'{7}','{8}','{9}','{10}'
                            ,'{11}','{12}','{13}','{14}','{15}'
                            ,'{16}','{17}','{18}','{19}','{20}'
                            ,'{21}','{22}','{23}','{24}','{25}'
                            ,'{26}','{27}','{28}','{29}','{30}'
                            ,'{31}','{32}','{33}','{34}','{35}'
                            ,'{36}','{37}','{38}','{39}','{40}'
                            ,'{41}','{42}','{43}','{44}','{45}'
                            ,'{46}')";

            sql = string.Format(sql, var1, var9, var15, var16, var17, var18
                                , var19, var20, var22, var23, var24
                                , var25, var25_1, var26, var27, var28
                                , var29, var30, var31, var32.Substring(0, 1), var32.Substring(2)
                                , var32_1, var32_2, var33.Substring(0, 1), var33.Substring(2), var33_1
                                , var33_2, var34.Substring(0, 1), var34.Substring(2), var34_1, var34_2
                                , var35, var36, var37, var38, var39
                                , var44, newfile.Substring(1), fileName, json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME")
                                , var40, var41, var42, var43, json_formdata.Value<string>("DOCSERVICECODE")
                                , json_formdata.Value<string>("DOCSERVICENAME")
                                );

            OracleConnection conn = null;
            OracleTransaction ot = null;
            string result = "{success:true}";
            try
            {
                conn = DBMgr.getOrclCon();
                conn.Open();
                ot = conn.BeginTransaction();

                //更新，需要作废前一笔数据；正式表数据需要作废（撤回）
                if (action == "update")
                {
                    ISINVALID_Predata(cusno, conn);
                    cancel_pub(cusno, conn);
                }

                int recount = DBMgr.ExecuteNonQuery(sql, conn);
                if (recount > 0)
                {

                    //项号//商品编码//商品名称//商品规格//净重
                    //成交数量//法一数量//法二数量//成交单位//法一单位
                    //法二单位//原产国//总价//单价//币制
                    //征免//目的国//企业编号（外键）              

                    string sql_detail = @"insert into list_predata_sub(ID,ISINVALID,CREATEDATE,FLAG
                            ,ORDERNO,COMMODITYNO,ADDITIONALNO,COMMODITYCHNAME,SPECIFICATIONSMODEL,GOODSNW
                            ,CADQUANTITY,LEGALQUANTITY,SQUANTITY,CADUNITNAME,LEGALUNITNAME
                            ,SUNITNAME,COUNTRYORIGINNAME,TOTALPRICE,UNITPRICE,CURRENCYNAME
                            ,TAXPAIDNAME,DESTCOUNTRYNAME,PCODE
                        ) VALUES (LIST_PREDATA_SUB_ID.Nextval,0,sysdate,0
                            ,'{0}','{1}','{2}','{3}','{4}','{5}'
                            ,'{6}','{7}','{8}','{9}','{10}'
                            ,'{11}','{12}','{13}','{14}','{15}'
                            ,'{16}','{17}','{18}')";
                    /*暂时不用：//成交单位CODE//法一单位CODE//法二单位CODE//,CADUNITCODE,LEGALUNITCODE,SUNITCODE    
                    //JArray ja_unit = json.Value<JArray>("unit");//,'{19}','{20}','{21}' */

                    for (int j = 12; j < dtExcel.Rows.Count; j = j + 3)
                    {
                        if (dtExcel.Rows[j][1].ToString().Trim() == "")
                        {
                            break;
                        }

                        sql = string.Format(sql_detail
                            , dtExcel.Rows[j][0].ToString(), dtExcel.Rows[j][1].ToString().Substring(0, 8), dtExcel.Rows[j][1].ToString().Substring(8), dtExcel.Rows[j][2].ToString(), dtExcel.Rows[j][4].ToString(), dtExcel.Rows[j][8].ToString()
                            , dtExcel.Rows[j][9].ToString(), dtExcel.Rows[j + 1][9].ToString(), dtExcel.Rows[j + 2][9].ToString(), dtExcel.Rows[j][10].ToString(), dtExcel.Rows[j + 1][10].ToString()
                            , dtExcel.Rows[j + 2][10].ToString(), dtExcel.Rows[j][11].ToString(), dtExcel.Rows[j][12].ToString(), dtExcel.Rows[j][13].ToString(), dtExcel.Rows[j][14].ToString()
                            , dtExcel.Rows[j][15].ToString(), dtExcel.Rows[j][16].ToString(), cusno
                            );/*, getStatusCodebyname(dtExcel.Rows[j][10].ToString(), ja_unit), getStatusCodebyname(dtExcel.Rows[j + 1][10].ToString(), ja_unit)
                        , getStatusCodebyname(dtExcel.Rows[j + 2][10].ToString(), ja_unit)*/
                        DBMgr.ExecuteNonQuery(sql, conn);
                    }
                }

                if (action == "update")//更新上传 需要判断之前的数据已经提交
                {
                    if (GetDeclStatus(cusno) == "{success:false}")//再次及时判断一次报关状态
                    {
                        ot.Commit();
                    }
                    else
                    {
                        ot.Rollback();
                        result = "{success:false,error:'状态已经是预录中，不能更新数据'}";
                    }

                }
                else
                {
                    ot.Commit();
                }

            }
            catch (Exception ex)
            {
                ot.Rollback();
                result = "{success:false,error:'" + ex.Message + "'}";

            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public string ModuleTwo(string newfile, string fileName, string action, string cusno, JObject json_formdata)
        {
            DataTable dtExcel = Extension.GetExcelData_Table(Server.MapPath(newfile),0);

            //Rows：表头11行+一行表体列名+至少一行表体数据；Columns：18列
            if (dtExcel == null || dtExcel.Rows.Count < 12 || dtExcel.Columns.Count != 35)
            {
                return "No Data";
            }
            //验证列名称
            int colcount = 0;
            for (int i = 0; i < dtExcel.Columns.Count; i++)
            {
                if (dtExcel.Columns[i].ColumnName == "申报类别") { colcount++; }
            }

            if (colcount == 0)
            {
                return "No Columns";
            }
            //====================================================================================================
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            //申报类别//进口口岸//申报现场//手册号
            string var1 = dtExcel.Columns[2].ColumnName; string var2 = dtExcel.Rows[2][2].ToString(); string var3 = dtExcel.Rows[2][6].ToString(); string var4 = dtExcel.Rows[2][11].ToString();
            //合同协议号//申报日期//收发货单位//运输方式
            string var5 = dtExcel.Rows[2][16].ToString(); string var6 = dtExcel.Rows[2][21].ToString(); string var7 = dtExcel.Rows[3][2].ToString(); string var7_1 = dtExcel.Rows[3][4].ToString();
            string var8 = dtExcel.Rows[3][11].ToString();
            //运输工具//提运单号//消费使用单位//监管方式
            string var9 = dtExcel.Rows[3][16].ToString(); string var10 = dtExcel.Rows[3][21].ToString(); string var11 = dtExcel.Rows[4][2].ToString(); string var11_1 = dtExcel.Rows[4][4].ToString();
            string var12 = dtExcel.Rows[4][11].ToString();
            //征免性质//征税比例/结汇方式//随附单证//启运国/运抵国
            string var13 = dtExcel.Rows[4][16].ToString(); string var14 = dtExcel.Rows[4][21].ToString(); string var15 = dtExcel.Rows[5][2].ToString(); string var16 = dtExcel.Rows[5][11].ToString();

            //装货港/指运港//境内目的地/境内货源地//标记唛码及备注 //成交方式
            string var17 = dtExcel.Rows[5][16].ToString(); string var18 = dtExcel.Rows[5][21].ToString(); string var19 = dtExcel.Rows[6][2].ToString(); string var20 = dtExcel.Rows[6][11].ToString();
            //许可证号//贸易国//包装种类//毛重
            string var21 = dtExcel.Rows[6][16].ToString(); string var22 = dtExcel.Rows[6][21].ToString(); string var23 = dtExcel.Rows[7][11].ToString(); string var24 = dtExcel.Rows[7][16].ToString();
            //净重//集装箱号//件数//特殊关系确认
            string var25 = dtExcel.Rows[7][21].ToString(); string var26 = dtExcel.Rows[8][11].ToString(); string var27 = dtExcel.Rows[8][16].ToString(); string var28 = dtExcel.Rows[0][34].ToString();
            //价格影响确认//支付特许权使用确认
            string var29 = dtExcel.Rows[1][34].ToString(); string var30 = dtExcel.Rows[2][34].ToString();

            if (var28 == "是") { var28 = "1"; } else if (var28 == "否") { var28 = "0"; } else { var28 = "9"; }
            if (var29 == "是") { var29 = "1"; } else if (var29 == "否") { var29 = "0"; } else { var29 = "9"; }
            if (var30 == "是") { var30 = "1"; } else if (var30 == "否") { var30 = "0"; } else { var30 = "9"; }

            //基础资料 获取code
            //string jsonstr = Extension.Ini_Base_Data(json_user, "predata", "");
            //JObject json = (JObject)JsonConvert.DeserializeObject(jsonstr);


            //监管方式//进口口岸
            //string var12_code = getStatusCode(var12, json.Value<JArray>("myfs"));
            //string var2_code = getStatusCode(var2, json.Value<JArray>("sbgq"));

            //企业编号
            if (action == "add") { cusno = getcusno(); }

            //申报类别//进口口岸//手册号/合同协议号//申报日期//收发货单位
            //收发货单位//运输方式//运输工具//提运单号//消费使用单位
            //消费使用单位//监管方式//征免性质//征税比例/结汇方式//启运国/运抵国
            //装货港/指运港//境内目的地/境内货源地//标记唛码及备注 //成交方式//许可证号
            //贸易国//包装种类//毛重//净重//件数
            //特殊关系确认//价格影响确认//支付特许权使用确认//文件路径//原始文件名
            //委托单位代码//委托单位名称
            string sql = @"insert into list_predata(ID,ISINVALID,CREATETIME,FLAG,CUSNO
                            ,INOUTTYPE,PORTNAME,RECORDCODE,CONTRACTNO,REPDATE,BUSIUNITCODE
                            ,BUSIUNITNAME,TRANSMODEL,TRANSNAME,BLNO,CONSHIPPERCODE
                            ,CONSHIPPERNAME,TRADENAME,EXEMPTIONNAME,TAXRATE,SECOUNTRYNAME
                            ,SEPORTNAME,SEPLACENAME,REMARK,TRADETERMSNAME,LICENSENO
                            ,TRADECOUNTRYNAME,PACKAGENAME,GOODSGW,GOODSNW,GOODSNUM
                            ,SPECIALRELATION,PRICEIMPACT,PAYPOYALTIES,FILEPATH,OLDFILENAME
                            ,CUSTOMERCODE,CUSTOMERNAME,DOCSERVICECODE,DOCSERVICENAME                            
                        ) VALUES ( LIST_PREDATA_ID.Nextval,0,sysdate,0,'" + cusno + @"'
                            ,'{0}','{1}','{2}','{3}',to_date('{4}','yyyy/mm/dd'),'{5}'
                            ,'{6}','{7}','{8}','{9}','{10}'
                            ,'{11}','{12}','{13}','{14}','{15}'
                            ,'{16}','{17}','{18}','{19}','{20}'
                            ,'{21}','{22}','{23}','{24}','{25}'
                            ,'{26}','{27}','{28}','{29}','{30}'
                            ,'{31}','{32}','{33}','{34}'
                            )";
            sql = string.Format(sql, var1, var2, var4, var5, var6, var7
                               , var7_1, var8, var9, var10, var11
                               , var11_1, var12, var13, var14, var16
                               , var17, var18, var19, var20, var21
                               , var22, var23, var24, var25, var27
                               , var28, var29, var30, newfile.Substring(1), fileName
                               , json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), json_formdata.Value<string>("DOCSERVICECODE"), json_formdata.Value<string>("DOCSERVICENAME")
                               );

            OracleConnection conn = null;
            OracleTransaction ot = null;
            string result = "{success:true}";
            try
            {
                conn = DBMgr.getOrclCon();
                conn.Open();
                ot = conn.BeginTransaction();

                //更新，需要作废前一笔数据；正式表数据需要作废（撤回）
                if (action == "update")
                {
                    ISINVALID_Predata(cusno, conn);
                    cancel_pub(cusno, conn);
                }

                int recount = DBMgr.ExecuteNonQuery(sql, conn);
                if (recount > 0)
                {
                    //序号//手册项号//海关编码//附加编码//商品名称//规格型号
                    //申报数量//申报单位//法定数量//法定单位//第二法定数量
                    //第二法定单位//最终目的国//单价//金额//币制
                    //征免//版本//原产国//净重
                    string sql_detail = @"insert into list_predata_sub(ID,ISINVALID,CREATEDATE,FLAG
                                                ,ORDERNO,ITEMNO,COMMODITYNO,ADDITIONALNO,COMMODITYCHNAME,SPECIFICATIONSMODEL
                                                ,CADQUANTITY,CADUNITNAME,LEGALQUANTITY,LEGALUNITNAME,SQUANTITY
                                                ,SUNITNAME,DESTCOUNTRYNAME,UNITPRICE,TOTALPRICE,CURRENCYNAME
                                                ,TAXPAIDNAME,VERSIONNO,COUNTRYORIGINNAME,GOODSNW,PCODE
                                            ) VALUES (LIST_PREDATA_SUB_ID.Nextval,0,sysdate,0
                                                ,'{0}','{1}','{2}','{3}','{4}','{5}'
                                                ,'{6}','{7}','{8}','{9}','{10}'
                                                ,'{11}','{12}','{13}','{14}','{15}'
                                                ,'{16}','{17}','{18}','{19}','{20}')";
                    /*暂时不用：//成交单位CODE//法一单位CODE//法二单位CODE//,CADUNITCODE,LEGALUNITCODE,SUNITCODE    
                    JArray ja_unit = json.Value<JArray>("unit");//,'{21}','{22}','{23}' */

                    for (int j = 11; j < dtExcel.Rows.Count; j++)
                    {
                        if (dtExcel.Rows[j][3].ToString().Trim() == "")
                        {
                            break;
                        }

                        sql = string.Format(sql_detail
                            , dtExcel.Rows[j][0].ToString(), dtExcel.Rows[j][1].ToString(), dtExcel.Rows[j][3].ToString(), dtExcel.Rows[j][5].ToString(), dtExcel.Rows[j][6].ToString(), dtExcel.Rows[j][9].ToString()
                            , dtExcel.Rows[j][13].ToString(), dtExcel.Rows[j][14].ToString(), dtExcel.Rows[j][15].ToString(), dtExcel.Rows[j][16].ToString(), dtExcel.Rows[j][17].ToString()
                            , dtExcel.Rows[j][18].ToString(), dtExcel.Rows[j][19].ToString(), dtExcel.Rows[j][20].ToString(), dtExcel.Rows[j][21].ToString(), dtExcel.Rows[j][22].ToString()
                            , dtExcel.Rows[j][23].ToString(), dtExcel.Rows[j][24].ToString(), dtExcel.Rows[j][25].ToString(), dtExcel.Rows[j][28].ToString(), cusno
                            );/*, getStatusCodebyname(dtExcel.Rows[j][14].ToString(), ja_unit), getStatusCodebyname(dtExcel.Rows[j][16].ToString(), ja_unit)
                                            , getStatusCodebyname(dtExcel.Rows[j][18].ToString(), ja_unit)*/
                        DBMgr.ExecuteNonQuery(sql, conn);
                    }
                }
                if (action == "update")//更新上传 需要判断之前的数据已经提交
                {
                    if (GetDeclStatus(cusno) == "{success:false}")//再次及时判断一次报关状态
                    {
                        ot.Commit();
                    }
                    else
                    {
                        ot.Rollback();
                        result = "{success:false,error:'状态已经是预录中，不能更新数据'}";
                    }

                }
                else
                {
                    ot.Commit();
                }

            }
            catch (Exception ex)
            {
                ot.Rollback();
                result = "{success:false,error:'" + ex.Message + "'}";

            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public string DeletePreData()
        {
            string result = "{success:false}";
            //string str = "";

            try
            {
                string sql = "select * from LIST_PREDATA where ID in(" + Request["ids"] + ")";
                DataTable dt = DBMgr.GetDataTable(sql);

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["FLAG"] + "" == "0")
                    {
                        sql = "delete from LIST_PREDATA_SUB where PCODE='" + dr["CUSNO"] + "' and ISINVALID=0";
                        DBMgr.ExecuteNonQuery(sql);

                        sql = "delete from LIST_PREDATA where ID='" + dr["ID"] + "'";
                        DBMgr.ExecuteNonQuery(sql);

                        FileInfo fi = new FileInfo(Server.MapPath(@"~" + dr["FILEPATH"]));
                        if (fi.Exists)
                        {
                            fi.Delete();
                        }
                    }
                    //else
                    //{
                    //    str += "," + dr["CUSNO"];
                    //}
                }

                //if (str != "") { str = str.Substring(1); }
                //result = "{success:true,str:'" + str + "'}"; 
                result = "{success:true}";
            }
            catch (Exception ex)
            {


            }

            return result;
        }

        public string loadPreDataDetail()
        {
            string CUSNO = Request["CUSNO"];

            string result = string.Empty;
            string sql = string.Empty; string head_data = "{}"; string sub_data = "[]"; string declstatus = "";


            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd";

            sql = "select * from list_predata where ISINVALID=0 and cusno='" + CUSNO + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            head_data = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');

            sql = "select * from list_predata_sub where ISINVALID=0 and pcode='" + CUSNO + "' order by orderno";
            DataTable dt_sub = DBMgr.GetDataTable(sql);
            sub_data = JsonConvert.SerializeObject(dt_sub, iso);

            if (GetDeclStatus(CUSNO) == "{success:true}")//状态>=70了
            {
                declstatus = "1";
            }
            else
            {
                declstatus = "0";
            }

            result = "{head_data:" + head_data + ",sub_data:" + sub_data + ",declstatus:'" + declstatus + "'}";
            return result;
        }

        public string GetDeclStatus(string CUSNO)
        {
            string result = "{success:false}";

            string sql = @"select * from list_declaration where ISINVALID=0 and ordercode=(select code from list_order where ISINVALID=0 and cusno='{0}') and status>=70";
            sql = string.Format(sql, CUSNO);
            DataTable dt = new DataTable();
            dt = DBMgr.GetDataTable(sql);

            if (dt.Rows.Count > 0)
            {
                result = "{success:true}";
            }
            return result;
        }

        public string cancelPreData(string cusno)
        {
            OracleConnection conn = null;
            OracleTransaction ot = null;
            string result = "{success:true}";
            try
            {
                conn = DBMgr.getOrclCon();
                conn.Open();
                ot = conn.BeginTransaction();

                cancel_pub(cusno, conn);//作废正式表数据

                //临时表数据标记为未转化
                DBMgr.ExecuteNonQuery("update list_predata set flag=0 where cusno='" + cusno + "' and ISINVALID=0", conn);
                DBMgr.ExecuteNonQuery("update list_predata_sub set flag=0 where pcode='" + cusno + "' and ISINVALID=0", conn);


                if (GetDeclStatus(cusno) == "{success:false}")//再次及时判断一次报关状态
                {
                    ot.Commit();
                }
                else
                {
                    ot.Rollback();
                    result = "{success:false,error:'状态已经是预录中，不能撤回'}";
                }

            }
            catch (Exception ex)
            {
                ot.Rollback();
                result = "{success:false,error:'" + ex.Message + "'}";

            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public string SavePreDataToPrd()
        {
            string cusno = Request["cusno"];
            string result = "{success:true}";
            try
            {
                JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

                OracleParameter[] parms = new OracleParameter[4];
                parms[0] = new OracleParameter("p_cusno", OracleDbType.NVarchar2, cusno, ParameterDirection.Input);
                parms[1] = new OracleParameter("p_userid", OracleDbType.NVarchar2, json_user.Value<string>("ID"), ParameterDirection.Input);
                parms[2] = new OracleParameter("p_username", OracleDbType.NVarchar2, json_user.Value<string>("REALNAME"), ParameterDirection.Input);

                parms[3] = new OracleParameter("p_flag_parms", OracleDbType.Varchar2, 20, null, ParameterDirection.Output);//输出参数，字符串类型的，一定要设定大小

                DataTable dt = DBMgr.GetDataTableParm("Pro_SavePreDataToPrd", parms);
                string p_flag_parms = parms[3].Value.ToString();
                if (p_flag_parms == "N")
                {
                    result = "{success:false,error:'数据执行失败'}";
                }
            }
            catch (Exception ex)
            {
                result = "{success:false,error:'" + ex.Message + "'}";

            }
            return result;

        }

        public string Declare_Pre()
        {
            string result = "{success:false}";

            try
            {
                JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
                string cusno = Request["cusno"]; string ordercode = Request["ordercode"]; string declcode = Request["declcode"];
                string sql = "";

                MethodSvc.MethodServiceClient msc = new MethodSvc.MethodServiceClient();
                msc.ChangeStatus(100, ordercode, json_user.Value<int>("ID"), json_user.Value<string>("REALNAME"), declcode, 0);

                //更新task_bgd数据 
                sql = "update TASK_BGD set SBZT='1',SBSJHQ='1',SBSJ=sysdate,SBRY='{1}',RWRY='{1}' where DJBM='{0}'";
                sql = string.Format(sql, declcode, json_user.Value<string>("NAME"));
                DBMgr.ExecuteNonQuery(sql);

                //删除task_zt数据 
                DBMgr.ExecuteNonQuery("delete TASK_ZT where DJBM='" + declcode + "'");

                //插入task_zt数据 
                DataTable dt = new DataTable();
                dt = DBMgr.GetDataTable(@"select ld.DECLTYPE,ld.CURRENTCODE,lo.REPUNITCODE 
                                            from list_declaration ld 
                                                left join list_order lo on ld.ordercode=lo.code 
                                            where ld.code='" + declcode + "'");
                string DECLTYPE = "", CURRENTCODE = "", REPUNITCODE = "";
                if (dt.Rows.Count > 0)
                {
                    DECLTYPE = dt.Rows[0]["DECLTYPE"].ToString();
                    CURRENTCODE = dt.Rows[0]["CURRENTCODE"].ToString();
                    REPUNITCODE = dt.Rows[0]["REPUNITCODE"].ToString();
                }
                string SBKB = "";
                if (DECLTYPE == "1" || DECLTYPE == "3" || DECLTYPE == "5" || DECLTYPE == "7" || DECLTYPE == "9" || DECLTYPE == "11")
                {
                    SBKB = "0";
                }
                else if (DECLTYPE == "2" || DECLTYPE == "4" || DECLTYPE == "6" || DECLTYPE == "8" || DECLTYPE == "10" || DECLTYPE == "12" || DECLTYPE == "19")
                {
                    SBKB = "1";
                }
                else if (DECLTYPE == "13" || DECLTYPE == "15" || DECLTYPE == "17")
                {
                    SBKB = "2";
                }
                else if (DECLTYPE == "16" || DECLTYPE == "14" || DECLTYPE == "18")
                {
                    SBKB = "3";
                }

                sql = @"insert into TASK_ZT(DJBM,SBKB,ZCTY_NO,RWRY,RWSJ,KSCX,WCBZ,SBDW_NO) 
                            values('{0}','{1}','{2}','{3}',sysdate,'0','0','{4}')";
                sql = string.Format(sql, declcode, SBKB, CURRENTCODE, json_user.Value<string>("NAME"), REPUNITCODE);
                DBMgr.ExecuteNonQuery(sql);

                result = "{success:true}";
            }
            catch (Exception ex)
            {

            }
            return result;

        }

        #endregion



    }
}
