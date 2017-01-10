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

    }
}
