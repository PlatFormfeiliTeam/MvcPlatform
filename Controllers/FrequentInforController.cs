using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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

        //为了基础数据新增的
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

    }
}
