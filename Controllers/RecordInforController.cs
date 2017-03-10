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
                                         from sys_recordinfo_detail_task aa 
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

            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "b.id", "desc"));
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        #endregion

        public string GetElements()
        {
            string customarea = Request["customarea"].ToString(); string hscode = Request["hscode"].ToString(); 
            string additionalno = Request["additionalno"].ToString(); string id = Request["id"].ToString();
            string sql = string.Empty; string json = "[]";

            string flag = "";
            if (id == "") { flag = "1"; }//查总库
            else
            {
                sql = @"select * from sys_recordinfo_detail_task where id='" + id + "'";
                DataTable dt = DBMgr.GetDataTable(sql);
                string hscode_database = dt.Rows[0]["HSCODE"].ToString(); string customarea_database = dt.Rows[0]["CUSTOMAREA"].ToString(); string additionalno_database = dt.Rows[0]["ADDITIONALNO"].ToString();
                if (hscode_database != hscode || customarea_database != customarea || additionalno_database != additionalno)//修改了这两个字段
                {
                    flag = "1";//查总库
                }
            }

            if (flag == "1")//查总库
            {
                sql = @"select regexp_substr(elements,'[^;]+',1,level,'i') elements,'' descriptions 
                    from (select elements from cusdoc.BASE_COMMODITYHS where hscode='{0}' and yearid={1} and extracode='{2}') t1
                    connect by level <= length(elements) - length(replace(elements,';',''))";
                sql = string.Format(sql, hscode, customarea, additionalno);
                json = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
            }
            else
            {
                sql = "select functiontype as elements,descriptions from SYS_ELEMENTS where rid='" + id + "' order by sno";
                json = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
            }           

            return "{elements:" + json + "}";
        }

        public string cancel_create()
        {
            string id = Request["id"];
            string result = "{success:false}"; string sql = "";

            sql = "select * from sys_recordinfo_detail_task where id ='" + id + "'";
            DataTable dt = DBMgr.GetDataTable(sql);

            if (Convert.ToInt32(dt.Rows[0]["STATUS"] + "") == 0)
            {
                return result;
            }

            sql = "update sys_recordinfo_detail_task set STATUS = 0,SUBMITID=null,SUBMITTIME=null,SUBMITNAME=null where id ='" + id + "'";
            DBMgr.ExecuteNonQuery(sql);

            result = "{success:true}";
            return result;
        }

        public string loadrecord_create()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string id = Request["id"];
            string sql = "";
            string result = "{}";
            if (!string.IsNullOrEmpty(id))
            {
                IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                sql = @"select * from sys_recordinfo_detail_task where id='" + id + "'";
                string formdata = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql), iso).TrimStart('[').TrimEnd(']');

                //成品单耗
                sql = "select * from SYS_PRODUCTCONSUME where rid='" + id + "' order by id desc";
                string productsonsumedata = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql), iso);

                result = "{formdata:" + formdata + ",productsonsumedata:" + productsonsumedata + "}";
            }

            return result;            
        }

        public string Create_Save()
        {
            string action = Request["action"];            
            JObject json = (JObject)JsonConvert.DeserializeObject(Request["formdata"]);           
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string sql = ""; string resultmsg = "{success:false}";
            //验证项号是否重复----------------------------------------------------------------------------------------
            if (json.Value<string>("ITEMNO") != null)
            {
                sql = "select itemno from SYS_RECORDINFO_DETAIL_TASK where STATUS<50 and RECORDINFOID='{0}' and ITEMNO='{1}' and ITEMNOATTRIBUTE='{2}'";
                if (!string.IsNullOrEmpty(Request["id"])) { sql = sql + " and ID!='{3}'"; }
                sql += " union select itemno from cusdoc.SYS_RECORDINFO_DETAIL where enabled=1 and RECORDINFOID='{0}' and ITEMNO='{1}' and ITEMNOATTRIBUTE='{2}'";

                sql = string.Format(sql, json.Value<string>("RECORDINFOID"), json.Value<string>("ITEMNO"), json.Value<string>("ITEMNOATTRIBUTE"), Request["id"]);
                DataTable dt_itemno = new DataTable();
                dt_itemno = DBMgr.GetDataTable(sql);
                if (dt_itemno.Rows.Count > 0) { resultmsg = "{success:false,isrepeate:'Y'}"; return resultmsg; }
            }           
            //-----------------------------------------------------------------------------------------------------------


            string id = string.Empty;
            if (Request["action"] + "" == "submit")
            {
                json.Remove("STATUS"); json.Add("STATUS", 10);
                json.Remove("SUBMITTIME"); json.Add("SUBMITTIME", "sysdate");
                json.Remove("SUBMITNAME"); json.Add("SUBMITNAME", json_user.Value<string>("REALNAME"));
                json.Remove("SUBMITID"); json.Add("SUBMITID", json_user.Value<string>("ID"));
            }
            else
            {
                json.Remove("SUBMITTIME"); //委托时间  
                json.Add("SUBMITTIME", "null");
            }

            if (string.IsNullOrEmpty(Request["id"]))//新增
            {
                sql = "select SYS_RECORDINFO_DETAIL_TASK_ID.Nextval from dual";
                id = DBMgr.GetDataTable(sql).Rows[0][0] + "";//获取ID
                sql = @"INSERT INTO SYS_RECORDINFO_DETAIL_TASK (ID
                        ,RECORDINFOID,ITEMNO,HSCODE,ADDITIONALNO,ITEMNOATTRIBUTE
                        ,COMMODITYNAME,SPECIFICATIONSMODEL,UNIT,REMARK,MODIFYREASON
                        ,CREATEID,CREATENAME,CREATEDATE,OPTIONS,STATUS,CUSTOMERCODE
                        ,CUSTOMERNAME,SUBMITID,SUBMITNAME,SUBMITTIME,CUSTOMAREA                       
                        ) VALUES ('{0}'
                            ,'{1}','{2}','{3}','{4}','{5}'
                            ,'{6}','{7}','{8}','{9}','{10}'
                            ,'{11}','{12}',sysdate,'{13}','{14}','{15}'
                            ,'{16}','{17}','{18}',{19},'{20}'
                            )";
                sql = string.Format(sql, id
                    , json.Value<string>("RECORDINFOID"), json.Value<string>("ITEMNO"), json.Value<string>("HSCODE"), json.Value<string>("ADDITIONALNO"), json.Value<string>("ITEMNOATTRIBUTE")
                    , json.Value<string>("COMMODITYNAME"), json.Value<string>("SPECIFICATIONSMODEL"), json.Value<string>("UNIT"), json.Value<string>("REMARK"), json.Value<string>("MODIFYREASON")
                    , json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"), 'A', json.Value<string>("STATUS"), json.Value<string>("CUSTOMERCODE")
                    , json.Value<string>("CUSTOMERNAME"), json.Value<string>("SUBMITID"), json.Value<string>("SUBMITNAME"), json.Value<string>("SUBMITTIME"), json.Value<string>("CUSTOMAREA")
                    );
            }
            else//修改
            {
                id = Request["id"];
                sql = @"UPDATE SYS_RECORDINFO_DETAIL_TASK SET RECORDINFOID='{1}',ITEMNO='{2}',HSCODE='{3}',ADDITIONALNO='{4}',ITEMNOATTRIBUTE='{5}' 
                            ,COMMODITYNAME='{6}',SPECIFICATIONSMODEL='{7}',UNIT='{8}',REMARK='{9}',MODIFYREASON='{10}'
                            ,OPTIONS='{11}',STATUS='{12}',CUSTOMERCODE='{13}',CUSTOMERNAME='{14}',SUBMITID='{15}'
                            ,SUBMITNAME='{16}',SUBMITTIME={17},CUSTOMAREA='{18}'
                        WHERE ID={0}";
                sql = string.Format(sql, id
                    , json.Value<string>("RECORDINFOID"), json.Value<string>("ITEMNO"), json.Value<string>("HSCODE"), json.Value<string>("ADDITIONALNO"), json.Value<string>("ITEMNOATTRIBUTE")
                    , json.Value<string>("COMMODITYNAME"), json.Value<string>("SPECIFICATIONSMODEL"), json.Value<string>("UNIT"), json.Value<string>("REMARK"), json.Value<string>("MODIFYREASON")
                    , 'A', json.Value<string>("STATUS"), json.Value<string>("CUSTOMERCODE"), json.Value<string>("CUSTOMERNAME"), json.Value<string>("SUBMITID")
                    , json.Value<string>("SUBMITNAME"), json.Value<string>("SUBMITTIME"), json.Value<string>("CUSTOMAREA")
                    );
            }
            if (sql != "")
            {
                int result = DBMgr.ExecuteNonQuery(sql);
                if (result == 1)
                {
                    update_elements(json, json_user, id);//申报要素  
                    update_productconsume(json, json_user, id);//成品单耗

                    resultmsg = "{success:true,id:'" + id + "'}";
                }
            }
            return resultmsg;
        }

        //申报要素  
        public string update_elements(JObject json, JObject json_user, string id)
        {
            string sql = "";
            //先清空
            sql = @"delete SYS_ELEMENTS where RID='" + id + "'";
            DBMgr.ExecuteNonQuery(sql);

            if (json.Value<string>("jsonEle") != null)
            {
                //在插入
                string jsonEle = json.Value<string>("jsonEle").Substring(("{elements:").Length);
                jsonEle = jsonEle.Substring(0, jsonEle.Length - 1);
                JArray je = (JArray)JsonConvert.DeserializeObject(jsonEle);
                for (int i = 0; i < je.Count; i++)
                {
                    sql = @"insert into SYS_ELEMENTS(ID,RECORDINFOID,ITEMNO,ITEMNOATTRIBUTE,SNO,FUNCTIONTYPE,DESCRIPTIONS,CREATEMAN,CREATEDATE,RID) 
                            values(SYS_ELEMENTS_id.Nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}',sysdate,'{7}')";
                    sql = string.Format(sql, json.Value<string>("RECORDINFOID"), json.Value<string>("ITEMNO"), json.Value<string>("ITEMNOATTRIBUTE"), i, je[i].Value<string>("ELEMENTS")
                        , json.Value<string>("field_ele_" + i), json_user.Value<string>("ID"), id);
                    DBMgr.ExecuteNonQuery(sql);
                }
            }           
            return "success";
        }

        //成品单耗
        public string update_productconsume(JObject json, JObject json_user, string id)
        {
            string sql = "";
            //先清空
            sql = @"delete SYS_PRODUCTCONSUME where RID='" + id + "'";
            DBMgr.ExecuteNonQuery(sql);

            if (json.Value<string>("ITEMNOATTRIBUTE") == "成品") 
            {
                //在插入
                JArray ja = (JArray)JsonConvert.DeserializeObject(Request["productconsume"]);
                for (int j = 0; j < ja.Count; j++)
                {
                    sql = @"insert into SYS_PRODUCTCONSUME(ID,RECORDINFOID,ITEMNO,ITEMNO_CONSUME,ITEMNO_COMMODITYNAME,ITEMNO_SPECIFICATIONSMODEL,ITEMNO_UNIT,
                                        ITEMNO_UNITNAME,CONSUME,ATTRITIONRATE,CREATEMAN,CREATEDATE,RID) 
                                    values(SYS_PRODUCTCONSUME_id.Nextval,'{0}','{1}','{2}','{3}','{4}','{5}'
                                    ,'{6}','{7}','{8}','{9}',sysdate,'{10}')";
                    sql = string.Format(sql, json.Value<string>("RECORDINFOID"), json.Value<string>("ITEMNO"), ja[j].Value<string>("ITEMNO_CONSUME"), ja[j].Value<string>("ITEMNO_COMMODITYNAME"), ja[j].Value<string>("ITEMNO_SPECIFICATIONSMODEL"), ja[j].Value<string>("ITEMNO_UNIT")
                        , ja[j].Value<string>("ITEMNO_UNITNAME"), ja[j].Value<string>("CONSUME"), ja[j].Value<string>("ATTRITIONRATE"), json_user.Value<string>("ID"), id
                        );
                    DBMgr.ExecuteNonQuery(sql);
                }
            }
            return "success";
        }

        public string Delete_Task()
        {
            string ids = Request["ids"];
            string result = "{success:false}"; string sql = "";

            bool bf = false;
            sql = "select * from sys_recordinfo_detail_task where id in(" + ids + ")";
            DataTable dt = DBMgr.GetDataTable(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToInt32(dt.Rows[0]["STATUS"] + "") != 0)
                {
                    bf = true;
                    break;
                }
            }

            if (bf) { return result; }

            sql = "delete from SYS_ELEMENTS where rid in(" + ids + ")";
            DBMgr.ExecuteNonQuery(sql);

            sql = "delete from SYS_PRODUCTCONSUME where rid in(" + ids + ")";
            DBMgr.ExecuteNonQuery(sql);

            sql = "delete from sys_recordinfo_detail_task where id in(" + ids + ")";
            DBMgr.ExecuteNonQuery(sql);

            result = "{success:true}";

            return result;
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

        //================================================================================================//
        public ActionResult PrintRecordDetail()//备案信息 change
        {
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public string GetPrintDetail()
        {
            string ids = Request["id"];
            string id = ids.IndexOf(",") == -1 ? ids : ids.Substring(0, ids.IndexOf(","));
            string ITEMNOATTRIBUTE = string.Empty;
            string sql = string.Empty; string sql_cp = string.Empty;
            IsoDateTimeConverter iso = new IsoDateTimeConverter();
            string sql_recordinfo = "select a.itemnoattribute,(select code from cusdoc.SYS_RECORDINFO where id=a.recordinfoid) recordcode from SYS_RECORDINFO_DETAIL_TASK a where id=" + id;
            DataTable dt_recordinfo = DBMgr.GetDataTable(sql_recordinfo);
            string json_recordinfo = JsonConvert.SerializeObject(dt_recordinfo, iso);
            ITEMNOATTRIBUTE = dt_recordinfo.Rows[0]["ITEMNOATTRIBUTE"].ToString();
            sql = "select a.itemno,a.hscode,a.commodityname,b.ele,a.options,(select name from cusdoc.base_declproductunit where enabled=1 and code=a.unit) unit," +
                      "(select name from cusdoc.base_declproductunit where enabled=1 and code=c.legalunit) legalunit,(select name from cusdoc.base_declproductunit where enabled=1 and code=c.secondunit) secondunit " +
                      " from SYS_RECORDINFO_DETAIL_TASK  A left join " +
                       "(select RID,listagg(to_char(FUNCTIONTYPE||':'||DESCRIPTIONS),'<br/>') within group(order by sno) as ELE from SYS_ELEMENTS  GROUP BY RID) B on A.Id=B.RID " +
                       "left join cusdoc.base_commodityhs  C on A.HSCODE=C.HSCODE AND A.CUSTOMAREA=C.YEARID AND A.Additionalno=C.Extracode " +
                       "  where A.ID in(" + ids + ") order by a.itemno";
            string json = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql), iso);

            if (ITEMNOATTRIBUTE == "成品")
            {
                sql_cp = "select a.ITEMNO,a.HSCODE,a.COMMODITYNAME,a.OPTIONS,(select name from cusdoc.base_declproductunit where enabled=1 and code=a.unit) UNIT,ITEMNO_CONSUME,ITEMNO_COMMODITYNAME,ITEMNO_SPECIFICATIONSMODEL,ITEMNO_UNITNAME," +
                         "CONSUME,ATTRITIONRATE from SYS_RECORDINFO_DETAIL_TASK  A left join SYS_PRODUCTCONSUME B  on A.ID=B.RID WHERE A.ID IN (" + ids + ") order by a.itemno";
                string json_cp = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql_cp), iso);
                return "{jsonrows:" + json + ",jsonrows_cp:" + json_cp + ",json_recordinfo:" + json_recordinfo + "}";

            }

            return "{jsonrows:" + json + ",json_recordinfo:" + json_recordinfo + "}";


        }
        //================================================================================================//

    }
}
