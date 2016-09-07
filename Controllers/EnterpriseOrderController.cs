using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    public class EnterpriseOrderController : Controller
    {
        int totalProperty = 0;
        //
        // GET: /EnterpriseOrder/

        public ActionResult EntOrderList()  //文件委托
        {
            return View();
        }

        public ActionResult EnterpriseHome()//企业服务
        {
            return View();
        }

        #region 文件委托
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

        public string LoadList()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string where = "";
            if (!string.IsNullOrEmpty(Request["busitype"]))//判断查询条件是否有值
            {
                where += " and t.BUSITYPEID='" + Request["busitype"].ToString() + "'";
            }
            if (!string.IsNullOrEmpty(Request["FILERECEVIEUNIT"]))//判断查询条件是否有值
            {
                string FILERECEVIEUNIT = Request["FILERECEVIEUNIT"].ToString();
                where += " and t.FILERECEVIEUNITCODE='" + GetCode(FILERECEVIEUNIT) + "' and FILERECEVIEUNITNAME='" + GetName(FILERECEVIEUNIT) + "' ";
            }
            if (!string.IsNullOrEmpty(Request["FILEDECLAREUNIT"]))//判断查询条件是否有值
            {
                string FILEDECLAREUNIT = Request["FILEDECLAREUNIT"].ToString();
                where += " and t.FILEDECLAREUNITCODE='" + GetCode(FILEDECLAREUNIT) + "' and FILEDECLAREUNITNAME='" + GetName(FILEDECLAREUNIT) + "' ";
            }
            if (!string.IsNullOrEmpty(Request["CODE"]))//判断查询条件是否有值
            {
                string CODE = Request["CODE"].ToString().Trim();
                where += " and instr(t.CODE,'" + CODE + "')>0 ";
            }

            if (!string.IsNullOrEmpty(Request["STATUS_S"]))//判断查询条件是否有值
            {
                where += " and t.STATUS='" + Request["STATUS_S"].ToString().Trim() + "'";
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

            string sql = @"select t.ID, t.FILERECEVIEUNITCODE, t.FILERECEVIEUNITNAME, t.FILEDECLAREUNITCODE, t.FILEDECLAREUNITNAME, t.BUSITYPEID
                            , t.CUSTOMDISTRICTCODE, t.CUSTOMDISTRICTNAME, t.REPWAYID, t.STATUS
                            ,t.CODE, t.CREATEID, t.CREATENAME, t.CREATETIME, t.ASSOCIATENO, t.ORDERCODE, t.ENTERPRISECODE, t.ENTERPRISENAME, t.ACCEPTID, t.ACCEPTNAME
                            ,t.ACCEPTTIME, t.SUBMITTIME 
                        from ENT_ORDER t                
                        where ENTERPRISECODE='" + json_user.Value<string>("CUSTOMERCODE") + "' " + where;
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql,"CREATETIME","desc"));
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string loadform()
        {
            Object json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string ID = Request["ID"]; string busitype = Request["busitype"];

            DataTable dt;
            string data = "{}", filedata = "[]";
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            if (string.IsNullOrEmpty(ID))//如果为空、即新增的时候
            {
                string result = "{STATUS:5,BUSITYPEID:'" + busitype + "'}";
                return "{data:" + result + ",filedata:" + filedata + "}";
            }
            else
            {
                string sql = @"select id, code
                                    , case  when length(filerecevieunitname || '(' || filerecevieunitcode || ')')=2 then '' || '' else filerecevieunitname || '(' || filerecevieunitcode || ')' end as filerecevieunit
                                    , filerecevieunitname
                                    , case  when length(filedeclareunitname||'('||filedeclareunitcode||')')=2 then '' || '' else  filedeclareunitname||'('||filedeclareunitcode||')' end as filedeclareunit
                                    , filedeclareunitname
                                    , busitypeid, customdistrictcode, customdistrictname, repwayid, status, files, associateno
                                    , createid, createname, createtime, enterprisecode, enterprisename ,remark, code, isupload
                                from ENT_ORDER a where ID = '" + ID + "'";

                dt = DBMgr.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    data = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');
                    filedata = dt.Rows[0]["FILES"].ToString();
                }
                return "{data:" + data + ",filedata:" + filedata + "}";
            }
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

        public string Save()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string ID = Request["ID"].ToString();
            JObject json_data = (JObject)JsonConvert.DeserializeObject(Request["data"]);
            string filedata = Request["filedata"].ToString();

            if (Request["action"] + "" == "submit")
            {
                json_data.Remove("STATUS"); json_data.Add("STATUS", 10);
                json_data.Add("SUBMITTIME", "sysdate");
            }
            else
            {
                json_data.Add("SUBMITTIME", "null");
            }

            string AssociateNo = "";//两单关联号          
            string sql = "";

            string exe_desc = "";//订单保存时记录各订单的执行情况

            if (string.IsNullOrEmpty(ID))//新增
            {
                DataTable dtid = DBMgr.GetDataTable("select ENT_ORDER_ID.Nextval from dual");
                ID = dtid.Rows[0][0].ToString();

                //(select fun_AutoQYBH(sysdate) from dual)

                sql = @"insert into ENT_ORDER (id, code, filerecevieunitcode, filerecevieunitname, filedeclareunitcode, filedeclareunitname, busitypeid, customdistrictcode, customdistrictname, repwayid
                                                , status, files, associateno, createid, createname, createtime, enterprisecode, enterprisename,submittime, remark) 
                        values ('{15}','{18}','{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'
                                                ,'{8}','{9}','{10}','{11}','{12}',sysdate,'{13}','{14}',{16},'{17}')";
                sql = string.Format(sql, GetCode(json_data.Value<string>("FILERECEVIEUNIT")), GetName(json_data.Value<string>("FILERECEVIEUNIT")), GetCode(json_data.Value<string>("FILEDECLAREUNIT"))
                        , GetName(json_data.Value<string>("FILEDECLAREUNIT")), json_data.Value<string>("BUSITYPEID"), json_data.Value<string>("CUSTOMDISTRICTCODE")
                        , json_data.Value<string>("CUSTOMDISTRICTNAME"), json_data.Value<string>("REPWAYID"), json_data.Value<string>("STATUS"), filedata
                        , AssociateNo, json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME")
                        , ID, json_data.Value<string>("SUBMITTIME"), json_data.Value<string>("REMARK"), json_data.Value<string>("CODE")
                        );
            }
            else
            {
                sql = @"update ENT_ORDER  set filerecevieunitcode='{0}',filerecevieunitname='{1}',filedeclareunitcode='{2}',filedeclareunitname='{3}',busitypeid='{4}',customdistrictcode='{5}',
                            customdistrictname='{6}',repwayid='{7}',status='{8}' ,files='{9}',associateno='{10}',createid='{11}',
                            createname='{12}',createtime=sysdate,enterprisecode='{13}',enterprisename='{14}',submittime={16},remark='{17}',code='{18}' where id='{15}'";
                sql = string.Format(sql, GetCode(json_data.Value<string>("FILERECEVIEUNIT")), GetName(json_data.Value<string>("FILERECEVIEUNIT")), GetCode(json_data.Value<string>("FILEDECLAREUNIT"))
                        , GetName(json_data.Value<string>("FILEDECLAREUNIT")), json_data.Value<string>("BUSITYPEID"), json_data.Value<string>("CUSTOMDISTRICTCODE")
                        , json_data.Value<string>("CUSTOMDISTRICTNAME"), json_data.Value<string>("REPWAYID"), json_data.Value<string>("STATUS"), filedata
                        , AssociateNo, json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME")
                        , ID, json_data.Value<string>("SUBMITTIME"), json_data.Value<string>("REMARK"), json_data.Value<string>("CODE")
                        );
            }

            int order_res = DBMgr.ExecuteNonQuery(sql);
            if (order_res == 0)
            {
                exe_desc = "保存失败!";
            }

            return "{ID:'" + ID + "',result:'" + exe_desc + "'}";
        }

        public ActionResult Delete(string id)
        {
            string ids = id.TrimEnd(',');
            string msg = "删除失败！";

            string sql = @"delete from ENT_ORDER where id in (" + ids + ")";
            sql = string.Format(sql, id);

            int i = DBMgr.ExecuteNonQuery(sql);
            if (i > 0)
            {
                msg = "删除成功！";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
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

        #endregion

        public string IniChart()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = "";
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("date");
            dt.Columns.Add(dc);
            dc = new DataColumn("total");
            dt.Columns.Add(dc);
            dc = new DataColumn("submit");
            dt.Columns.Add(dc);
            DataTable tmp_dt;
            for (int i = 0; i < 7; i++)
            {
                DataRow dr = dt.NewRow();
                dr["date"] = DateTime.Now.AddDays(-i).ToShortDateString();
                sql = "select count(1) from ent_order where  to_char(CREATETIME, 'yyyy-MM-dd')=to_char(sysdate-" + i + ",'yyyy-MM-dd') and ENTERPRISECODE='" + json_user.Value<string>("CUSTOMERCODE") + "'";
                tmp_dt = DBMgr.GetDataTable(sql);
                dr["total"] = tmp_dt.Rows[0][0];
                sql = "select count(1) from ent_order where  to_char(SUBMITTIME, 'yyyy-MM-dd')=to_char(sysdate-" + i + ",'yyyy-MM-dd') and ENTERPRISECODE='" + json_user.Value<string>("CUSTOMERCODE") + "'";
                tmp_dt = DBMgr.GetDataTable(sql);
                dr["submit"] = tmp_dt.Rows[0][0];
                dt.Rows.Add(dr);
            }
            return "{result:" + JsonConvert.SerializeObject(dt) + "}";
        }

    }
}
