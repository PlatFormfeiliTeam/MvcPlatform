using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    [Authorize]
    [Filters.AuthFilter]
    public class OrderManagerController : Controller
    {
        int totalProperty = 0;
        //
        // GET: /OrderManager/

        public ActionResult Index()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            ViewBag.navigator = "订单中心>>关务服务2";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.navigator = "订单中心>>关务服务2";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public string Ini_Base_Data_BUSIITEM()
        {
            string sql = "";
            string json_ywxx = "[]";//业务细项

            sql = @"select a.BUSIITEMCODE CODE,a.BUSIITEMNAME NAME,a.BUSIITEMNAME||'('||a.BUSIITEMCODE||')' CODENAME 
                    from web_customsconfig a 
                    where enable=1 and busitypecode='" + Request["busitype"] + "' order by a.BUSIITEMCODE";
            json_ywxx = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));

            return "{ywxx:" + json_ywxx + "}";
        }

        public string LoadList()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string sql = QueryCondition();

            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "a.createtime", "desc"));
            var json = JsonConvert.SerializeObject(dt, iso);

            return "{rows:" + json + ",total:" + totalProperty + "}";//",json_senior:" + json_senior +
        }

        public string QueryCondition()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string where = "";
            if (!string.IsNullOrEmpty(Request["BUSIUNITCODE"]))//判断查询条件1是否有值
            {
                where += " and a.BUSIUNITCODE='" + Request["BUSIUNITCODE"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["CUSNO"]))//判断查询条件1是否有值
            {
                where += " and a.CUSNO='" + Request["CUSNO"].Trim() + "'";
            }
            if (!string.IsNullOrEmpty(Request["busitypeid"]))//判断查询条件1是否有值
            {
                where += " and a.busitype='" + Request["busitypeid"].Trim() + "'";
            }
            if (!string.IsNullOrEmpty(Request["start_date"]))//如果开始时间有值
            {
                where += " and a.CREATETIME>=to_date('" + Request["start_date"] + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            if (!string.IsNullOrEmpty(Request["end_date"]))//如果结束时间有值
            {
                where += " and a.CREATETIME<=to_date('" + Request["end_date"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
            }

            if (!string.IsNullOrEmpty(Request["CUSTOMERCODE"]))//判断查询条件1是否有值
            {
                where += " and a.CUSTOMERCODE='" + Request["CUSTOMERCODE"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["CODE"]))//判断查询条件1是否有值
            {
                where += " and a.CODE='" + Request["CODE"].Trim() + "'";
            }
            if (!string.IsNullOrEmpty(Request["entrusttype"]))//判断查询条件1是否有值
            {
                where += " and a.entrusttype='" + Request["entrusttype"].Trim() + "'";
            }
            if (!string.IsNullOrEmpty(Request["start_date2"]))//如果开始时间有值
            {
                where += " and a.SUBMITTIME>=to_date('" + Request["start_date2"] + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            if (!string.IsNullOrEmpty(Request["end_date2"]))//如果结束时间有值
            {
                where += " and a.SUBMITTIME<=to_date('" + Request["end_date2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
            }

            if ((Request["OnlySelf"] + "").Trim() == "fa fa-check-square-o")
            {
                where += " and (a.CREATEUSERID = " + json_user.Value<string>("ID") + " or a.submitusername='" + json_user.Value<string>("REALNAME") + "') ";
            }

            string sql = @"select a.ID,a.BUSITYPE,a.CREATETIME,a.SUBMITTIME,a.ENTRUSTTYPE,a.CUSTOMERCODE,a.CUSTOMERNAME,a.CLEARUNIT,a.CLEARUNITNAME 
                                ,a.BUSIUNITCODE,a.BUSIUNITNAME,a.CODE,a.CUSNO,a.DOREQUEST,a.CLEARREMARK
                                ,b.busiitemname ENTRUSTTYPENAME
                        from LIST_ORDER a 
                            left join web_customsconfig b on a.busitype=b.busitypecode and a.entrusttype=b.busiitemcode
                        where a.ISINVALID=0 and b.enable=1";

            string rolestr = "";
            rolestr = " a.receiverunitcode='" + json_user.Value<string>("CUSTOMERCODE") + "'";

            /*if (json_user.Value<string>("ISRECEIVER") == "1")//接单单位
            {
                string rec = " a.receiverunitcode='" + json_user.Value<string>("CUSTOMERCODE") + "'";

                if (rolestr == "") { rolestr = rec; }
                else { rolestr = rolestr + " or " + rec; }
            }

            if (json_user.Value<string>("ISCUSTOMER") == "1")//委托单位
            {
                string cus = " a.customercode='" + json_user.Value<string>("CUSTOMERCODE") + "'";

                if (rolestr == "") { rolestr = cus; }
                else { rolestr = rolestr + " or " + cus; }
            }

            if (json_user.Value<string>("DOCSERVICECOMPANY") == "1")//单证服务单位
            {
                string doc = " a.docservicecode='" + json_user.Value<string>("CUSTOMERCODE") + "'";

                if (rolestr == "") { rolestr = doc; }
                else { rolestr = rolestr + " or " + doc; }
            }*/

            sql += " and (" + rolestr + ") " + where;

            return sql;
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

        public string Getele(string pagename, string configtype, string busitype, string entrusttype, string ordercode)
        {
            string sql = string.Empty; string fieldcolumn = "[]"; string fieldcolumn_con = "{}";
            string result = "{fieldcolumn:" + fieldcolumn + ",fieldcolumn_con:" + fieldcolumn_con + "}"; 

            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            DataTable dt = new DataTable();
            sql = @"select * from web_pageconfig where enabled=1 and pagename='{0}' and configcontent='{1}' and customercode='{2}'";
            sql = string.Format(sql, pagename, "业务类型=" + busitype + ";业务细项=" + entrusttype, json_user.Value<string>("CUSTOMERCODE"));
            dt = DBMgr.GetDataTable(sql);

            if (dt == null) { return result; }
            if (dt.Rows.Count != 1) { return result; }
            int parentid = Convert.ToInt32(dt.Rows[0]["ID"].ToString() == "" ? "0" : dt.Rows[0]["ID"].ToString());

            DataTable dt_field = new DataTable();
            sql = @"select name,controltype,selectcontent,tablecode,fieldcode 
                    from web_pageconfig_detail 
                    where enabled=1 and configtype='" + configtype + "' and parentid=" + parentid + " order by orderno";
            dt_field = DBMgr.GetDataTable(sql);
            fieldcolumn = JsonConvert.SerializeObject(dt_field);

            //-------------------------------------------------------------------------------------------------------------------
            if (ordercode == "")//为空,则不需要赋值
            {
                result = "{fieldcolumn:" + fieldcolumn + ",fieldcolumn_con:" + fieldcolumn_con + "}";
                return result;
            }

            string ParTableName = "LIST_ORDER";

            DataTable dt_con = new DataTable();
            dt_con.Rows.Add(dt_con.NewRow());

            if (dt_field != null)
            {
                string fieldname = "";
                for (int fi = 0; fi < dt_field.Rows.Count; fi++)
                {
                    fieldname = dt_field.Rows[fi]["TABLECODE"].ToString() + "|" + dt_field.Rows[fi]["FIELDCODE"].ToString();
                    switch (dt_field.Rows[fi]["CONTROLTYPE"].ToString())
                    {
                        case "文本":
                            dt_con.Columns.Add(fieldname + "|" + "text", typeof(string));
                            break;
                        case "数字":
                            dt_con.Columns.Add(fieldname + "|" + "number", typeof(string));
                            break;
                        case "日期":
                            dt_con.Columns.Add(fieldname + "|" + "date", typeof(string));
                            break;
                        case "下拉框":
                            dt_con.Columns.Add(fieldname + "|" + "combox", typeof(string));
                            break;
                        default:
                            dt_con.Columns.Add(fieldname + "|" + "text", typeof(string));
                            break;
                    }

                    sql = @"select ";
                    if (dt_field.Rows[fi]["CONTROLTYPE"].ToString() == "日期")//sql = "to_char(" + dt_field.Rows[fi]["FIELDCODE"].ToString() + ",'yyyy-mm-dd hh24:mi')";                        
                    {
                        sql = sql + "to_char(" + dt_field.Rows[fi]["FIELDCODE"].ToString() + ",'yyyy-mm-dd')";
                    }
                    else
                    {
                        sql = sql + dt_field.Rows[fi]["FIELDCODE"].ToString();
                    }
                    sql = sql + " from " + dt_field.Rows[fi]["TABLECODE"].ToString();

                    if (dt_field.Rows[fi]["TABLECODE"].ToString().ToUpper() == ParTableName.ToUpper())//跟主表同一张表
                    {
                        sql = sql + " where code='" + ordercode + "'";
                    }
                    else
                    {
                        sql = sql + " where ordercode='" + ordercode + "'";
                    }
                    
                    DataTable dt_fieldvalue = DBMgr.GetDataTable(sql);

                    if (dt_fieldvalue == null) { continue; }
                    if (dt_fieldvalue.Rows.Count <= 0) { continue; }
                    dt_con.Rows[0][dt_con.Columns.Count - 1] = dt_fieldvalue.Rows[0][0].ToString();
                }
            }
            fieldcolumn_con = JsonConvert.SerializeObject(dt_con).TrimStart('[').TrimEnd(']');
            //-------------------------------------------------------------------------------------------------------------------

            result = "{fieldcolumn:" + fieldcolumn + ",fieldcolumn_con:" + fieldcolumn_con + "}";
            return result;
        }

        public string loadform_OrderM(string ordercode, string pagename, string configtype)
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = "";
            string result = "{}"; string formdata = "{}"; 
            if (string.IsNullOrEmpty(ordercode))
            {

            }
            else
            {
                IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                sql = @"select CODE,BUSITYPE,ENTRUSTTYPE,CUSTOMERCODE,CUSTOMERNAME,BUSIUNITCODE
                            ,BUSIUNITNAME,CLEARUNIT,CLEARUNITNAME,CUSNO,CREATEUSERID                          
                            ,CREATEUSERNAME,CREATETIME,SUBMITUSERID,SUBMITUSERNAME,SUBMITTIME,DOREQUEST,CLEARREMARK
                        from list_order where code='" + ordercode + "'";
                DataTable dt = new DataTable();
                dt = DBMgr.GetDataTable(sql);
                formdata = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');                
            }
            result = "{formdata:" + formdata + "}";
            return result;
        }

        public string Save_OrderM()
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(Request["formdata_str"]);
            JObject json_con = (JObject)JsonConvert.DeserializeObject(Request["formdata_con"]);
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string sql = ""; string resultmsg = "{success:false}"; string ParTableName = "LIST_ORDER";
            string ordercode = string.Empty;
            try
            {
                List<string> sqllist = new List<string>();
                if (string.IsNullOrEmpty(Request["ordercode"]))//新增
                {
                    ordercode = Extension.getOrderCode();
                    sql = @"INSERT INTO " + ParTableName + @" (ID,
                            BUSITYPE,CODE,ENTRUSTTYPE,CUSTOMERCODE,CUSTOMERNAME,BUSIUNITCODE
                            ,BUSIUNITNAME,CLEARUNIT,CLEARUNITNAME,CUSNO,CREATEUSERID                          
                            ,CREATEUSERNAME,CREATETIME,SUBMITUSERID,SUBMITUSERNAME,SUBMITTIME,DOREQUEST,CLEARREMARK
                            ,RECEIVERUNITCODE,RECEIVERUNITNAME
                        ) VALUES (LIST_ORDER_id.Nextval
                            ,'{0}','{1}','{2}','{3}','{4}','{5}'
                            ,'{6}','{7}','{8}','{9}','{10}'
                            ,'{11}',sysdate,'{12}','{13}',sysdate,'{14}','{15}'
                            ,'{16}','{17}'
                            )";
                    sql = string.Format(sql
                            , json.Value<string>("BUSITYPE"), ordercode, json.Value<string>("ENTRUSTTYPE"), json.Value<string>("CUSTOMERCODE"), json.Value<string>("CUSTOMERNAME"), json.Value<string>("BUSIUNITCODE")
                            , json.Value<string>("BUSIUNITNAME"), json.Value<string>("CLEARUNIT"), json.Value<string>("CLEARUNITNAME"), json.Value<string>("CUSNO"), json_user.Value<string>("ID")
                            , json_user.Value<string>("REALNAME"), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"), json.Value<string>("DOREQUEST"), json.Value<string>("CLEARREMARK")
                            , json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME")
                       );
                }
                else//修改
                {
                    ordercode = Request["ordercode"];
                    sql = @"UPDATE " + ParTableName + @" SET BUSITYPE='{1}',ENTRUSTTYPE='{2}',CUSTOMERCODE='{3}',CUSTOMERNAME='{4}',BUSIUNITCODE='{5}'             
                            ,BUSIUNITNAME='{6}',CLEARUNIT='{7}',CLEARUNITNAME='{8}',CUSNO='{9}',SUBMITUSERID='{10}'          
                            ,SUBMITUSERNAME='{11}',SUBMITTIME=sysdate,DOREQUEST='{12}',CLEARREMARK='{13}',RECEIVERUNITCODE='{14}',RECEIVERUNITNAME='{15}'                              
                        WHERE CODE='{0}'                          
                        ";
                    sql = string.Format(sql
                            , ordercode, json.Value<string>("BUSITYPE"), json.Value<string>("ENTRUSTTYPE"), json.Value<string>("CUSTOMERCODE"), json.Value<string>("CUSTOMERNAME"), json.Value<string>("BUSIUNITCODE")
                            , json.Value<string>("BUSIUNITNAME"), json.Value<string>("CLEARUNIT"), json.Value<string>("CLEARUNITNAME"), json.Value<string>("CUSNO"), json_user.Value<string>("ID")
                            , json_user.Value<string>("REALNAME"), json.Value<string>("DOREQUEST"), json.Value<string>("CLEARREMARK"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME")
                            );

                }
                if (sql != "")
                {
                    int ii = DBMgr.ExecuteNonQuery(sql);
                    if (ii == 1)
                    {
                        //-----------------------------------------------------------可配信息sql拼接-------------------------------------------------------------------------------
                        string tablename = "", fieldname = "", typename = "";  DataTable dt = new DataTable();
                        string all_tablename = ParTableName.ToUpper() + ";";

                        foreach (JProperty jp in (JToken)json_con)
                        {
                            tablename = jp.Name.Substring(0, jp.Name.IndexOf('|'));
                            fieldname = jp.Name.Substring(jp.Name.IndexOf('|') + 1, jp.Name.LastIndexOf('|') - 1 - jp.Name.IndexOf('|'));
                            typename = jp.Name.Substring(jp.Name.LastIndexOf('|') + 1);

                            if (tablename.ToUpper() == ParTableName.ToUpper())//跟主表同一张表
                            {
                                if (typename == "date")
                                {
                                    sql = @"update " + tablename + " set " + fieldname + "=to_date('" + jp.Value.ToString() + "','yyyy-MM-dd HH24:mi:ss') where code='" + ordercode + "'";
                                }
                                else
                                {
                                    sql = @"update " + tablename + " set " + fieldname + "='" + jp.Value.ToString() + "' where code='" + ordercode + "'";
                                }
                                
                                sqllist.Add(sql);

                            }
                            else if (all_tablename.Contains(tablename.ToUpper()))//非主表，但是是前面字段用过的表
                            {
                                if (typename == "date")
                                {
                                    sql = @"update " + tablename + " set " + fieldname + "=to_date('" + jp.Value.ToString() + "','yyyy-MM-dd HH24:mi:ss') where ordercode='" + ordercode + "'";
                                }
                                else
                                {
                                    sql = @"update " + tablename + " set " + fieldname + "='" + jp.Value.ToString() + "' where ordercode='" + ordercode + "'";
                                }

                                sqllist.Add(sql);
                            }
                            else
                            {
                                dt = DBMgr.GetDataTable(@"select code from " + tablename + " where ordercode='" + ordercode + "'");
                                if (dt != null)
                                {
                                    if (dt.Rows.Count <= 0)
                                    {
                                        if (typename == "date")
                                        {
                                            sql = @"insert into " + tablename + " (id,ordercode," + fieldname + ") VALUES (" + tablename + "_id.Nextval,'" + ordercode + "',to_date('" + jp.Value.ToString() + ",'yyyy-MM-dd HH24:mi:ss'))";
                                        }
                                        else
                                        {
                                            sql = @"insert into " + tablename + " (id,ordercode," + fieldname + ") VALUES (" + tablename + "_id.Nextval,'" + ordercode + "','" + jp.Value.ToString() + "')";
                                        }
                                    }
                                    else
                                    {
                                        if (typename == "date")
                                        {
                                            sql = @"update " + tablename + " set " + fieldname + "='to_date(" + jp.Value.ToString() + "','yyyy-MM-dd HH24:mi:ss') where ordercode='" + ordercode + "'";
                                        }
                                        else
                                        {
                                            sql = @"update " + tablename + " set " + fieldname + "='" + jp.Value.ToString() + "' where ordercode='" + ordercode + "'";
                                        }
                                        
                                    }
                                    sqllist.Add(sql);
                                    all_tablename = all_tablename + tablename.ToUpper() + ";";
                                }
                            }
                        }
                        //--------------------------------------------------------------------------------------------------------------------------------------------------------------

                        OracleConnection conn = null;
                        OracleTransaction ot = null;
                        conn = DBMgr.getOrclCon();
                        try
                        {
                            conn.Open();
                            ot = conn.BeginTransaction();
                            for (int i = 0; i < sqllist.Count; i++)
                            {
                                DBMgr.ExecuteNonQuery(sqllist[i], conn);
                            }
                            ot.Commit();
                            resultmsg = "{success:true,ordercode:'" + ordercode + "'}";
                        }
                        catch (Exception ex)
                        {
                            ot.Rollback();
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {

            }
            return resultmsg;
        }

    }
}
