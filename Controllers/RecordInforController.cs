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

        public string Create_Save()
        {
            string action = Request["action"];            
            JObject json = (JObject)JsonConvert.DeserializeObject(Request["formdata"]);
            JObject productconsume = (JObject)JsonConvert.DeserializeObject(Request["productconsume"]);
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string sql = "";
            string id = string.Empty; 
            if (Request["action"] + "" == "submit")
            {
                json.Remove("STATUS"); json.Remove("SUBMITTIME"); json.Remove("SUBMITUSERNAME");
                json.Add("STATUS", 10); json.Add("SUBMITTIME", "sysdate"); json.Add("SUBMITUSERNAME", json_user.Value<string>("REALNAME"));
            }

            if (string.IsNullOrEmpty(json.Value<string>("ID")))//新增
            {
                sql = "select SYS_RECORDINFO_DETAIL_TASK_ID.Nextval from dual";
                id = DBMgr.GetDataTable(sql).Rows[0][0] + "";//获取ID
                sql = @"INSERT INTO SYS_RECORDINFO_DETAIL_TASK (ID,
                          id, 
rid, 
recordinfoid, 
itemno, 
hscode, 
additionalno, 
itemnoattribute, 
commodityname, 
specificationsmodel, 
unit, 
remark, 
modifyreason, 
createman, 
createdate, 
options, 
status, 
customercode, 
customername, 
submitman, 
submittime, 
acceptman, 
accepttime, 
preman, 
pretime, 
finishman, 
finishtime, 
isprint_apply, 
isprint_accept, 
customarea                        
                        ) VALUES (LIST_ORDER_id.Nextval
                            ,'{0}','{1}','{2}','{3}','{4}','{5}'
                            ,'{6}','{7}','{8}','{9}','{10}'
                            ,'{11}','{12}','{13}','{14}','{15}'
                            ,'{16}','{17}','{18}','{19}','{20}'
                            ,'{21}','{22}','{23}','{24}','{25}'
                            ,'{26}','{27}','{28}','{29}','{30}'
                            ,'{31}','{32}','{33}','{34}','{35}'
                            ,'{36}','{37}','{38}',sysdate,'{39}','{40}'
                            ,'{41}',{42},'{43}','{44}','{45}'
                            )";

                /*id = Extension.getOrderCode();
                sql = @"INSERT INTO LIST_ORDER (ID,
                            BUSITYPE,CODE,CUSNO,BUSIUNITCODE,BUSIUNITNAME,CONTRACTNO
                            ,TOTALNO,DIVIDENO,TURNPRENO,GOODSNUM,WOODPACKINGID
                            ,CLEARANCENO,LAWFLAG,ENTRUSTTYPE,REPWAYID,CUSTOMAREACODE
                            ,REPUNITCODE,REPUNITNAME,DECLWAY,PORTCODE,INSPUNITCODE
                            ,INSPUNITNAME,ORDERREQUEST,CREATEUSERID,CREATEUSERNAME,STATUS
                            ,SUBMITUSERID,SUBMITUSERNAME,CUSTOMERCODE,CUSTOMERNAME,DECLCARNO
                            ,TRADEWAYCODES,GOODSGW,GOODSNW,PACKKIND,BUSIKIND
                            ,ORDERWAY,CLEARUNIT,CLEARUNITNAME,CREATETIME,SPECIALRELATIONSHIP,PRICEIMPACT
                            ,PAYPOYALTIES,SUBMITTIME,DECLSTATUS,INSPSTATUS,DOCSERVICECODE                        
                        ) VALUES (LIST_ORDER_id.Nextval
                            ,'{0}','{1}','{2}','{3}','{4}','{5}'
                            ,'{6}','{7}','{8}','{9}','{10}'
                            ,'{11}','{12}','{13}','{14}','{15}'
                            ,'{16}','{17}','{18}','{19}','{20}'
                            ,'{21}','{22}','{23}','{24}','{25}'
                            ,'{26}','{27}','{28}','{29}','{30}'
                            ,'{31}','{32}','{33}','{34}','{35}'
                            ,'{36}','{37}','{38}',sysdate,'{39}','{40}'
                            ,'{41}',{42},'{43}','{44}','{45}'
                            )";
                sql = string.Format(sql
                        , "11", ordercode, json.Value<string>("CUSNO"), json.Value<string>("BUSIUNITCODE"), json.Value<string>("BUSIUNITNAME"), json.Value<string>("CONTRACTNO")
                        , json.Value<string>("TOTALNO"), json.Value<string>("DIVIDENO"), json.Value<string>("TURNPRENO"), json.Value<string>("GOODSNUM"), json.Value<string>("WOODPACKINGID")
                        , json.Value<string>("CLEARANCENO"), GetChk(json.Value<string>("LAWFLAG")), json.Value<string>("ENTRUSTTYPE"), json.Value<string>("REPWAYID"), json.Value<string>("CUSTOMAREACODE")
                        , GetCode(json.Value<string>("REPUNITCODE")), GetName(json.Value<string>("REPUNITCODE")), json.Value<string>("DECLWAY"), json.Value<string>("PORTCODE"), GetCode(json.Value<string>("INSPUNITCODE"))
                        , GetName(json.Value<string>("INSPUNITCODE")), json.Value<string>("ORDERREQUEST"), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"), json.Value<string>("STATUS")
                        , json.Value<string>("SUBMITUSERID"), json.Value<string>("SUBMITUSERNAME"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), json.Value<string>("DECLCARNO")
                        , json.Value<string>("TRADEWAYCODES"), json.Value<string>("GOODSGW"), json.Value<string>("GOODSNW"), json.Value<string>("PACKKIND"), "001"
                        , "1", json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), GetChk(json.Value<string>("SPECIALRELATIONSHIP")), GetChk(json.Value<string>("PRICEIMPACT"))
                        , GetChk(json.Value<string>("PAYPOYALTIES")), json.Value<string>("SUBMITTIME"), json.Value<string>("DECLSTATUS"), json.Value<string>("INSPSTATUS"), json.Value<string>("DOCSERVICECODE")
                   );*/
            }
            else//修改
            {
                id = json.Value<string>("ID");
                /*
                string allcol = @"CODE
                            ,BUSITYPE,CUSNO,BUSIUNITCODE,BUSIUNITNAME,CONTRACTNO 
                            ,TOTALNO,DIVIDENO,TURNPRENO,GOODSNUM,WOODPACKINGID 
                            ,CLEARANCENO,LAWFLAG,ENTRUSTTYPE,REPWAYID,CUSTOMAREACODE 
                            ,REPUNITCODE,REPUNITNAME,DECLWAY,PORTCODE,INSPUNITCODE 
                            ,INSPUNITNAME,ORDERREQUEST,STATUS,SUBMITUSERID,SUBMITUSERNAME 
                            ,CUSTOMERCODE,CUSTOMERNAME,DECLCARNO,TRADEWAYCODES,GOODSGW 
                            ,GOODSNW,PACKKIND,BUSIKIND,ORDERWAY,CLEARUNIT 
                            ,CLEARUNITNAME,SPECIALRELATIONSHIP, PRICEIMPACT,PAYPOYALTIES,SUBMITTIME
                            ,DOCSERVICECODE,DECLSTATUS,INSPSTATUS
                            ";
                sql = Extension.getUpdateSql(allcol, ordercode, IsSubmitAfterSave);
                if (sql != "")
                {
                    sql = string.Format(sql
                            , ordercode, "11", json.Value<string>("CUSNO"), json.Value<string>("BUSIUNITCODE"), json.Value<string>("BUSIUNITNAME"), json.Value<string>("CONTRACTNO")
                            , json.Value<string>("TOTALNO"), json.Value<string>("DIVIDENO"), json.Value<string>("TURNPRENO"), json.Value<string>("GOODSNUM"), json.Value<string>("WOODPACKINGID")
                            , json.Value<string>("CLEARANCENO"), GetChk(json.Value<string>("LAWFLAG")), json.Value<string>("ENTRUSTTYPE"), json.Value<string>("REPWAYID"), json.Value<string>("CUSTOMAREACODE")
                            , GetCode(json.Value<string>("REPUNITCODE")), GetName(json.Value<string>("REPUNITCODE")), json.Value<string>("DECLWAY"), json.Value<string>("PORTCODE"), GetCode(json.Value<string>("INSPUNITCODE"))
                            , GetName(json.Value<string>("INSPUNITCODE")), json.Value<string>("ORDERREQUEST"), json.Value<string>("STATUS"), json.Value<string>("SUBMITUSERID"), json.Value<string>("SUBMITUSERNAME")
                            , json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), json.Value<string>("DECLCARNO"), json.Value<string>("TRADEWAYCODES"), json.Value<string>("GOODSGW")
                            , json.Value<string>("GOODSNW"), json.Value<string>("PACKKIND"), "001", "1", json_user.Value<string>("CUSTOMERCODE")
                            , json_user.Value<string>("CUSTOMERNAME"), GetChk(json.Value<string>("SPECIALRELATIONSHIP")), GetChk(json.Value<string>("PRICEIMPACT")), GetChk(json.Value<string>("PAYPOYALTIES")), json.Value<string>("SUBMITTIME")
                            , json.Value<string>("DOCSERVICECODE"), json.Value<string>("DECLSTATUS"), json.Value<string>("INSPSTATUS")
                            );
                }*/
            }
            if (sql != "")
            {
                int result = DBMgr.ExecuteNonQuery(sql);
                if (result == 1)
                {
                    //集装箱及报关车号列表更新
                    //Extension.predeclcontainer_update(ordercode, json.Value<string>("CONTAINERTRUCK"));
                    ////更新随附文件 
                    //Extension.Update_Attachment(ordercode, filedata, json.Value<string>("ORIGINALFILEIDS"), json_user);

                    ////插入订单状态变更日志
                    //Extension.add_list_time(json.Value<Int32>("STATUS"), ordercode, json_user);
                    //if (json.Value<Int32>("STATUS") > 10)
                    //{
                    //    Extension.Insert_FieldUpdate_History(ordercode, json, json_user, "11");
                    //}
                    return "{success:true,ordercode:'" + id + "'}";
                }
                else
                {
                    return "{success:false}";
                }
            }
            else
            {
                return "{success:false}";
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



    }
}
