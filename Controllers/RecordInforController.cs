using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    [Authorize]
    public class RecordInforController : Controller
    {
        //
        // GET: /RecordInfor/
        int totalProperty = 0;
        public ActionResult Recordinfo_Detail()//备案信息
        {
            ViewBag.navigator = "备案管理>>备案信息";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult Create()//备案信息 create
        {
            ViewBag.navigator = "备案管理>>备案信息";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult Change()//备案信息 change
        {
            ViewBag.navigator = "备案管理>>备案信息";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult Recordinfo_Detail_SUMNUM()//申报数量
        {
            ViewBag.navigator = "备案管理>>申报数量";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        #region Recordinfo_Detail
        public string Query_RecordInfor(string type)
        {
            string where = "";
            if (!string.IsNullOrEmpty(Request["RECORDINFORID"]))
            {
                where += " and a.CODE='" + Request["RECORDINFORID"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["ITEMNO"]))
            {
                where += " and  b.ITEMNO='" + Request["ITEMNO"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["HSCODE"]))
            {
                where += " and  b.HSCODE='" + Request["HSCODE"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["OPTIONS"]) && type == "go")
            {
                where += " and  b.OPTIONS='" + Request["OPTIONS"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["STATUS"]) && type == "go")
            {
                where += " and  b.STATUS='" + Request["STATUS"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["ITEMNOATTRIBUTE"]))
            {
                where += " and  b.ITEMNOATTRIBUTE='" + Request["ITEMNOATTRIBUTE"] + "'";
            }
            return where;
        }

        /*all sql
         * string sql = @"select a.code,b.*
                        from cusdoc.sys_recordinfo a
                             inner join (  
                                     select aa.ID,aa.recordinfoid,aa.itemno,aa.hscode,aa.additionalno,aa.itemnoattribute 
                                            ,aa.commodityname,aa.specificationsmodel,aa.unit,aa.remark
                                            ,aa.options,aa.status,aa.customercode,aa.customername
                                     from sys_recordinfo_detail_task aa where aa.status<50 
                                     union
                                     select aa.ID,aa.recordinfoid,aa.itemno,aa.hscode,aa.additionalno,aa.itemnoattribute 
                                            ,aa.commodityname,aa.specificationsmodel,aa.unit,aa.remark 
                                            ,null options,null status,null customercode,null customername
                                     from cusdoc.sys_recordinfo_detail aa 
                                          left join (select * from sys_recordinfo_detail_task where status<50 and OPTIONS<>'A') bb on aa.id=bb.rid
                                     where bb.rid is null
                                ) b on a.id=b.recordinfoid ";*/
        public string loadRecordDetail()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string where = Query_RecordInfor("");

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string sql = @"select a.code,b.*
                            from cusdoc.sys_recordinfo a
                                 inner join (
                                         select aa.ID,aa.recordinfoid,aa.itemno,aa.hscode,aa.additionalno,aa.itemnoattribute 
                                                ,aa.commodityname,aa.specificationsmodel,aa.unit,aa.remark 
                                                ,null options,null status,null customercode,null customername
                                         from cusdoc.sys_recordinfo_detail aa
                                    ) b on a.id=b.recordinfoid ";
            if (Request["ERROR"].ToString() == "true")
            {
                sql = sql + " left join (select hscode from cusdoc.BASE_COMMODITYHS where enabled=1) c on b.hscode=c.hscode";
            }
            sql = sql + " where a.busiunit='" + json_user.Value<string>("CUSTOMERHSCODE") + "'" + where;

            if (Request["ERROR"].ToString() == "true")
            {
                sql = sql + " and c.hscode is null";
            }

            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "recordinfoid,itemno", "asc"));
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string loadRecordDetail_Go()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string where = Query_RecordInfor("go");

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string sql = @"select a.code,b.*
                            from cusdoc.sys_recordinfo a
                                 inner join (  
                                         select aa.ID,aa.recordinfoid,aa.itemno,aa.hscode,aa.additionalno,aa.itemnoattribute 
                                                ,aa.commodityname,aa.specificationsmodel,aa.unit,aa.remark
                                                ,aa.options,aa.status,aa.customercode,aa.customername
                                         from sys_recordinfo_detail_task aa where aa.status<50
                                    ) b on a.id=b.recordinfoid ";

            if (Request["ERROR"].ToString() == "true")
            {
                sql = sql + " left join (select hscode from cusdoc.BASE_COMMODITYHS where enabled=1) c on b.hscode=c.hscode";
            }
            sql = sql + " where a.busiunit='" + json_user.Value<string>("CUSTOMERHSCODE") + "'" + where;

            if (Request["ERROR"].ToString() == "true")
            {
                sql = sql + " and c.hscode is null";
            }

            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "options,status", "asc"));
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        #endregion

        public string GetElements()
        {
            string customarea = Request["customarea"].ToString(); string hscode = Request["hscode"].ToString();
            string sql = @"select regexp_substr(elements,'[^;]+',1,level,'i') elements 
                    from (select elements from cusdoc.BASE_COMMODITYHS where hscode='{0}' and yearid={1} and rownum<=1) t1
                    connect by level <= length(elements) - length(replace(elements,';',''))";
            sql = string.Format(sql, hscode, customarea);

            DataTable dt = DBMgrBase.GetDataTable(sql);
            string json = JsonConvert.SerializeObject(dt);
            return "{elements:" + json + "}";
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



    }
}
