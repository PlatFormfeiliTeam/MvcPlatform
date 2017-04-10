using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    public class FrequentInforController : Controller
    {
        int totalProperty = 0;
        //
        // GET: /FrequentInfor/

        
        public ActionResult BaseCommodityHS()
        {
            ViewBag.navigator = "基础信息 > HS编码";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult DownList()
        {
            ViewBag.navigator = "其它信息 > 常用下载";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

          [Authorize]
        public ActionResult CollectInfor()
        {
            ViewBag.navigator = "常用工具 > 收藏信息";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult BaseCommodityHSDetail(string id)//HS编码 详细界面
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();//新建字典
            DataTable dt_CommodityHSDetail = new DataTable();

            string sql = @"select((SELECT name from base_ProductUnit where code =  t.LEGALUNIT) ||'/'||(SELECT name from base_ProductUnit where code =  t.SECONDUNIT))as LEGALUNITNAME, HSCODE||EXTRACODE AS HSCODEEXTRACODE, t.* from BASE_COMMODITYHS t where t.ID = " + id;
            dt_CommodityHSDetail = DBMgrBase.GetDataTable(sql);
            dic.Add("dt_CommodityHSDetail", dt_CommodityHSDetail);

            ViewBag.navigator = "基础信息 ><a href='/FrequentInfor/BaseCommodityHS'> HS编码</a> > 详细";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View(dic);
        }

        public ActionResult BaseInspHS()
        {
            ViewBag.navigator = "基础信息 > 国检代码";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }


        #region HS编码
        public string LoadBaseDeclhsclass()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string sql = @"select * from BASE_DECLHSCLASS ";
            DataTable dt = DBMgrBase.GetDataTable(GetPageSqlBase(sql, "createtime", "desc"));
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string LoadBaseCommodityhs(string HSCODEEXTRACODE, string NAME)
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string classcode = Request["classcode"]; string type = Request["type"];
            string sql = "", where = "";
            if (!string.IsNullOrEmpty(HSCODEEXTRACODE))
            {

                where += " and t.HSCODE like '%" + HSCODEEXTRACODE + "%'";

            }
            if (!string.IsNullOrEmpty(NAME))
            {
                where += " and t.NAME like '%" + NAME + "%'";
            }
            if (!string.IsNullOrEmpty(classcode) && type == "1")
            {
                where += " and t.CLASSCODE = " + classcode;
            }
            sql = @"select (SELECT name from base_ProductUnit where code =  t.LEGALUNIT) as LEGALUNITNAME
                        ,(SELECT name from base_ProductUnit where code =  t.SECONDUNIT) as SECONDUNITNAME
                        , HSCODE||EXTRACODE AS HSCODEEXTRACODE, t.* 
                    from BASE_COMMODITYHS t where 1=1" + where;
            DataTable dt = DBMgrBase.GetDataTable(GetPageSqlBase(sql, "createdate", "desc"));

            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        #endregion

        public string LoadBaseInsphs(string HSCODE, string HSNAME)
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string where = string.Empty;
            if (!string.IsNullOrEmpty(HSCODE))
            {
                where += " AND bi.HSCODE LIKE '%" + HSCODE + "%' ";
            }
            if (!string.IsNullOrEmpty(HSNAME))
            {
                where += "AND bi.HSNAME LIKE '%" + HSNAME + "%' ";
            }

            string sql = @"SELECT bi.*, su.NAME AS CREATENAME, su2.NAME AS STOPNAME, bp.NAME AS UNITNAME 
                        FROM BASE_INSPHS bi 
                            LEFT JOIN SYS_USER su ON bi.CREATEMAN = su.ID 
                            LEFT JOIN SYS_USER su2 ON bi.STOPMAN = su2.ID 
                            LEFT JOIN BASE_PRODUCTUNIT bp ON bi.LEGALUNIT = bp.CODE 
                        WHERE 1=1 " + where;

            DataTable dt = DBMgrBase.GetDataTable(GetPageSqlBase(sql, "bi.createdate", "desc"));
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";


        }

        private string GetPageSqlBase(string tempsql, string order, string asc)
        {
            int start = Convert.ToInt32(Request["start"]);
            int limit = Convert.ToInt32(Request["limit"]);
            string sql = "select count(1) from ( " + tempsql + " )";
            totalProperty = Convert.ToInt32(DBMgrBase.GetDataTable(sql).Rows[0][0]);
            string pageSql = @"SELECT * FROM ( SELECT tt.*, ROWNUM AS rowno FROM ({0} ORDER BY {1} {2}) tt WHERE ROWNUM <= {4}) table_alias WHERE table_alias.rowno >= {3}";
            pageSql = string.Format(pageSql, tempsql, order, asc, start + 1, limit + start);
            return pageSql;
        }
        //分页 by heguiqin 2016-08-26
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

        public string GetToolsGroupByType()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
           // string sql = "select * from list_collect_infor_BYUSER a left join list_collect_infor b on a.rid=b.id where a.sysuserid='" + json_user.Value<string>("ID")+ "'";

          //  string sql_gropByType = "select  DISTINCT TYPE ,'' ITEM from list_collect_infor order by TYPE ";
            string sql_gropByType = " SELECT a.*,row_number() over (order by TYPENAME desc)  TYPEID FROM(select  DISTINCT TYPE  TYPENAME,'' ITEM from list_collect_infor) A";
            DataTable dt = DBMgr.GetDataTable(sql_gropByType);
            for (int i = 0; i < dt.Rows.Count; i++)
			{
                string sql = "select ID RID,ICON,NAME,URL,TYPE TYPENAME,CREATEID,CREATEDATE,ISINVALID from list_collect_infor where TYPE='" + dt.Rows[i]["TYPENAME"].ToString() + "' AND id not in "+
                    "(select RID from list_collect_infor_BYUSER where sysuserid='" + json_user.Value<string>("ID") + "')";
                //string sql = "select * from list_collect_infor";)";
                dt.Rows[i]["ITEM"] = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
            }
            
            IsoDateTimeConverter iso = new IsoDateTimeConverter();
            string json_tools = JsonConvert.SerializeObject(dt,iso);
            return json_tools;
        }


        public string GetTools()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = "select a.*,b.ICON,b.NAME,b.URL,b.TYPE TYPENAME,b.CREATEID,b.CREATEDATE,b.ISINVALID  from list_collect_infor_BYUSER a left join list_collect_infor b on a.rid=b.id where a.TYPE='tool' and a.sysuserid='" + json_user.Value<string>("ID") + "'";
            //string sql = "select * from list_collect_infor";
            DataTable dt = DBMgr.GetDataTable(sql);
            IsoDateTimeConverter iso = new IsoDateTimeConverter();
            string json_tools = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json_tools + ",total:0}";
        }
        public string ManageTools()
        {
            string sql = string.Empty;
            string action = Request["act"] + "";
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string rid = Request["RID"] + "";
             if (action=="add")
            {
                string sql_query = "select * from list_collect_infor_BYUSER where TYPE='tool' and  RID='" + rid + "'";
                DataTable dt = DBMgr.GetDataTable(sql_query);
                if (dt.Rows.Count!=0)
                {
                  return "{success:false}";
                }

            sql = "insert into list_collect_infor_BYUSER(ID,SYSUSERID,TYPE,CREATEID,CREATEDATE,RID) values(LIST_COLLECT_INFOR_BYUSER_ID.Nextval,'{0}','tool','{1}',sysdate,'{2}')";
            sql=string.Format(sql, json_user.Value<string>("ID"), json_user.Value<string>("ID"), rid);
           
            }
            else
            {
                sql = "delete from  list_collect_infor_BYUSER where TYPE='tool' and  RID='" + rid + "' and SYSUSERID='" + json_user.Value<string>("ID") + "'";
            
            }
            DBMgr.ExecuteNonQuery(sql);
            return "{success:true}";
        
        }
        public string GetNews()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = "select a.*,b.TITLE,to_char(b.PUBLISHDATE,'yyyy-mm-dd') as PUBLISHDATE,b.ISINVALID from list_collect_infor_BYUSER a left join web_notice b on a.rid=b.id where a.TYPE='news' and a.sysuserid='" + json_user.Value<string>("ID") + "'";
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "publishdate","desc"));
            IsoDateTimeConverter iso = new IsoDateTimeConverter();
            string json_news = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json_news + ",total:" + totalProperty + "}";
        }
        public string ManageNews()
        {
            string sql = string.Empty;
            string action = Request["act"] + "";
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string rid = Request["RID"] + "";
            if (action == "add")
            {
                string sql_query = "select * from list_collect_infor_BYUSER where TYPE='news' and  RID='" + rid + "'";
                DataTable dt = DBMgr.GetDataTable(sql_query);
                if (dt.Rows.Count != 0)
                {
                    return "{success:false}";
                }
                sql = "insert into list_collect_infor_BYUSER(ID,SYSUSERID,TYPE,CREATEID,CREATEDATE,RID) values(LIST_COLLECT_INFOR_BYUSER_ID.Nextval,'{0}','news','{1}',sysdate,'{2}')";
                sql = string.Format(sql, json_user.Value<string>("ID"), json_user.Value<string>("ID"), rid);

            }
            else
            {
                sql = "delete from  list_collect_infor_BYUSER where TYPE='news' and  RID in (" + rid + ") and SYSUSERID='" + json_user.Value<string>("ID") + "'";

            }
            DBMgr.ExecuteNonQuery(sql);
            return "{success:true}";

        }
    }
}
