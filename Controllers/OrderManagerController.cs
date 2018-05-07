using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            ViewBag.navigator = "订单中心>>关务服务";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.navigator = "订单中心>>关务服务";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult ConfigItem()
        {
            ViewBag.navigator = "关务服务>>显示配置";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult BusitypeDetailBaseConfig()
        {
            ViewBag.navigator = "关务服务>>业务细项配置";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public string Ini_Base_Data_BUSIITEM()
        {
            string sql = "";
            string json_ywxx = "[]";//业务细项

            sql = @"select a.BUSIITEMCODE CODE,a.BUSIITEMNAME NAME,a.BUSIITEMNAME||'('||a.BUSIITEMCODE||')' CODENAME 
                    from web_customsconfig a 
                    where enable=1 and entrusttypecode='" + Request["entrusttype"] + "' order by a.BUSIITEMCODE";
            json_ywxx = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));

            return "{ywxx:" + json_ywxx + "}";
        }

        #region 关务服务2

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
            if (!string.IsNullOrEmpty(Request["entrusttype"]))//判断查询条件1是否有值
            {
                where += " and a.entrusttype='" + Request["entrusttype"].Trim() + "'";
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
            if (!string.IsNullOrEmpty(Request["busiitemcode"]))//判断查询条件1是否有值
            {
                where += " and a.busiitemcode='" + Request["busiitemcode"].Trim() + "'";
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

            if (!string.IsNullOrEmpty(Request["TEXTONE"]))
            {
                where += " and a.TEXTONE='" + Request["TEXTONE"].Trim() + "'";
            }
            if (!string.IsNullOrEmpty(Request["TEXTTWO"]))
            {
                where += " and a.TEXTTWO='" + Request["TEXTTWO"].Trim() + "'";
            }
            if (!string.IsNullOrEmpty(Request["NUMONE"]))
            {
                where += " and a.NUMONE='" + Request["NUMONE"].Trim() + "'";
            }
            if (!string.IsNullOrEmpty(Request["NUMTWO"]))
            {
                where += " and a.NUMTWO='" + Request["NUMTWO"].Trim() + "'";
            }
            if (!string.IsNullOrEmpty(Request["DATEONE"]))
            {
                where += " and a.DATEONE>=to_date('" + Request["DATEONE"] + "','yyyy-mm-dd hh24:mi:ss') ";
                //where += " and a.DATEONE>=to_date('" + Request["DATEONE"] + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            if (!string.IsNullOrEmpty(Request["DATETWO"]))
            {
                where += " and a.DATETWO>=to_date('" + Request["DATETWO"] + "','yyyy-mm-dd hh24:mi:ss') ";
                //where += " and a.DATETWO<=to_date('" + Request["DATETWO"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            if (!string.IsNullOrEmpty(Request["USERNAMEONE"]))
            {
                where += " and a.USERNAMEONE='" + Request["USERNAMEONE"].Trim() + "'";
            }
            if (!string.IsNullOrEmpty(Request["USERNAMETWO"]))
            {
                where += " and a.USERNAMETWO='" + Request["USERNAMETWO"].Trim() + "'";
            }


            if (Request["combo_ENABLED_S"] == "0")
            {
                    string a = Request["combo_ENABLED_S"];
                    where += " and a.SUBMITTIME is null";
            }
            else if (Request["combo_ENABLED_S"] == "1")
            {
                    where += " and a.SUBMITTIME is not null";
            }


            if (!string.IsNullOrEmpty(Request["field_clearremark"]))
            {
                where += "and a.CLEARREMARK like '%" + Request["field_clearremark"] + "%'";
            }

            if (Request["combo_clearremark"] == "0")
            {
                where += " and a.CLEARREMARK  is null";
            }
            else if (Request["combo_clearremark"] == "1")
            {
                where += " and a.CLEARREMARK is not null";
            }
            
            string sql = @"select a.ID,a.BUSITYPE,a.CREATETIME,a.SUBMITTIME,a.ENTRUSTTYPE,a.CUSTOMERCODE,a.CUSTOMERNAME,a.CLEARUNIT,a.CLEARUNITNAME 
                                ,a.BUSIUNITCODE,a.BUSIUNITNAME,a.CODE,a.CUSNO,a.DOREQUEST,a.CLEARREMARK
                                ,a.BUSIITEMCODE,a.BUSIITEMNAME
                                ,a.TEXTONE,a.TEXTTWO,a.NUMONE,a.NUMTWO,to_char(a.DATEONE,'yyyy-mm-dd') DATEONE,to_char(a.DATETWO,'yyyy-mm-dd') DATETWO
                                ,a.USERNAMEONE,a.USERIDONE,a.USERNAMETWO,a.USERIDTWO,fc.name,fc.getmoney 
                        from LIST_ORDER a 
                        left join (select nvl(fo.cost,0)-nvl(fo.paycost,0) as getmoney,fs.name,fo.code from finance_order fo left join finance_status fs on fo.status = fs.code) fc
                        on fc.code = a.code
                        where a.ISINVALID=0 and a.busitype='70'";

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

        public string CompleteOrderM()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string ordercode = Request["ordercode"]; string result = "{success:true}";

            string sql = @"select SUBMITTIME from list_order where code='" + ordercode + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            if (dt.Rows[0]["SUBMITTIME"].ToString() != "")
            {
                return "{success:false,flag:'E'}";
            }

            sql = @"update list_order set SUBMITTIME=sysdate,SUBMITUSERID='" + json_user.Value<string>("ID") + "',SUBMITUSERNAME='" + json_user.Value<string>("REALNAME") + "' where code='" + ordercode + "'";
            DBMgr.ExecuteNonQuery(sql);
            
            return result;
        }

        public string DeleteOrderM()
        {
            string ordercodes = Request["ordercodes"]; string result = "{success:true}";

            string sql = @"select SUBMITTIME from list_order where code in(" + ordercodes + ")";
            DataTable dt = DBMgr.GetDataTable(sql);
            bool bf = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["SUBMITTIME"].ToString() != "")
                {
                    bf = true;
                    break;
                }
            }

            if (bf)
            {
                return "{success:false,flag:'E'}";
            }

            sql = @"delete list_order where code in(" + ordercodes + ")";
            DBMgr.ExecuteNonQuery(sql);

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

        public string Getlabelname(string entrusttype, string busiitemcode)
        {
            string sql = string.Empty; 

            DataTable dt_field = new DataTable();
            sql = @"select originname,configname 
                    from web_customscost 
                    where entrusttypecode='" + entrusttype + "' and busiitemcode='" + busiitemcode + "'";
            dt_field = DBMgr.GetDataTable(sql);
            var fieldcolumn = JsonConvert.SerializeObject(dt_field);

            //-------------------------------------------------------------------------------------------------------------------

            return "{fieldcolumn:" + fieldcolumn + "}";
        }

        public string loadform_OrderM(string ordercode, string pagename, string configtype)
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = "";
            string result = "{}"; string formdata = "{}"; string cost_west = "[]"; string cost_east = "[]"; 
            string curuser = "{CUSTOMERCODE:'" + json_user.Value<string>("CUSTOMERCODE") + "',REALNAME:'" + json_user.Value<string>("REALNAME") + "',ID:'" + json_user.Value<string>("ID") + "'}";

            if (string.IsNullOrEmpty(ordercode))
            {

            }
            else
            {
                IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                sql = @"select CODE,BUSITYPE,ENTRUSTTYPE,CUSTOMERCODE,CUSTOMERNAME,BUSIUNITCODE
                            ,BUSIUNITNAME,CLEARUNIT,CLEARUNITNAME,CUSNO,CREATEUSERID                          
                            ,CREATEUSERNAME,CREATETIME,SUBMITUSERID,SUBMITUSERNAME,SUBMITTIME,DOREQUEST,CLEARREMARK,BUSIITEMCODE,BUSIITEMNAME
                            ,TEXTONE,TEXTTWO,NUMONE,NUMTWO,to_char(DATEONE,'yyyy-mm-dd') DATEONE,to_char(DATETWO,'yyyy-mm-dd') DATETWO,USERNAMEONE,USERIDONE,USERNAMETWO,USERIDTWO 
                        from list_order where code='" + ordercode + "'";
                DataTable dt = new DataTable();
                dt = DBMgr.GetDataTable(sql);
                formdata = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');

                sql = @"select a.BUILDMODE,a.SETTLEMENTUNIT,a.FEECODE,a.COST,a.CURRENCY,a.STATUS 
                            ,b.NAME FEENAME,b.NAME||'('||a.FEECODE||')' FEECODENAME
                            ,c.NAME STATUSNAME,c.NAME||'('||a.STATUS||')' STATUSCODENAME
                            ,d.NAME as CUSTOMERNAME
                            ,e.NAME as CURRENCYNAME
                        from finance_costdata a
                            left join finance_feelist b on a.FEECODE=b.CODE
                            left join finance_status c on a.STATUS=c.CODE
                            left join cusdoc.sys_customer d on a.SETTLEMENTUNIT=d.CODE
                            left join cusdoc.base_declcurrency e on a.CURRENCY=e.CODE
                        where paytype=1 and ordercode='" + ordercode + "'";
                cost_west = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));

                sql = @"select a.BUILDMODE,a.SETTLEMENTUNIT,a.FEECODE,a.COST,a.CURRENCY,a.STATUS 
                            ,b.NAME FEENAME,b.NAME||'('||a.FEECODE||')' FEECODENAME
                            ,c.NAME STATUSNAME,c.NAME||'('||a.STATUS||')' STATUSCODENAME
                            ,d.NAME as CUSTOMERNAME
                            ,e.NAME as CURRENCYNAME
                        from finance_costdata a
                            left join finance_feelist b on a.FEECODE=b.CODE
                            left join finance_status c on a.STATUS=c.CODE
                            left join cusdoc.sys_customer d on a.SETTLEMENTUNIT=d.CODE
                            left join cusdoc.base_declcurrency e on a.CURRENCY=e.CODE
                        where paytype=0 and ordercode='" + ordercode + "'";
                cost_east = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
            }
            result = "{formdata:" + formdata + ",curuser:" + curuser + ",cost_west:" + cost_west + ",cost_east:" + cost_east + "}";
            return result;
        }

        public string Save_OrderM()
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(Request["formdata"]);
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string sql = ""; string resultmsg = "{success:false}"; 
            string ordercode = string.Empty;
            try
            {
                List<string> sqllist = new List<string>();
                if (string.IsNullOrEmpty(Request["ordercode"]))//新增
                {
                    ordercode = Extension.getOrderCode();
                    sql = @"INSERT INTO LIST_ORDER (ID,
                            BUSITYPE,CODE,ENTRUSTTYPE,CUSTOMERCODE,CUSTOMERNAME,BUSIUNITCODE
                            ,BUSIUNITNAME,CLEARUNIT,CLEARUNITNAME,CUSNO,CREATEUSERID                          
                            ,CREATEUSERNAME,CREATETIME,DOREQUEST,CLEARREMARK,RECEIVERUNITCODE,RECEIVERUNITNAME
                            ,TEXTONE,TEXTTWO,NUMONE,NUMTWO,DATEONE
                            ,DATETWO,USERNAMEONE,USERIDONE,USERNAMETWO,USERIDTWO 
                            ,BUSIITEMCODE,BUSIITEMNAME
                        ) VALUES (LIST_ORDER_id.Nextval
                            ,'{0}','{1}','{2}','{3}','{4}','{5}'
                            ,'{6}','{7}','{8}','{9}','{10}'
                            ,'{11}',sysdate,'{12}','{13}','{14}','{15}'     
                            ,'{16}','{17}','{18}','{19}',to_date('{20}','yyyy-MM-dd HH24:mi:ss')
                            ,to_date('{21}','yyyy-MM-dd HH24:mi:ss'),'{22}','{23}','{24}','{25}'   
                            ,'{26}','{27}'                    
                            )";
                    sql = string.Format(sql
                            , "70", ordercode, json.Value<string>("ENTRUSTTYPE"), json.Value<string>("CUSTOMERCODE"), json.Value<string>("CUSTOMERNAME"), json.Value<string>("BUSIUNITCODE")
                            , json.Value<string>("BUSIUNITNAME"), json.Value<string>("CLEARUNIT"), json.Value<string>("CLEARUNITNAME"), json.Value<string>("CUSNO"), json_user.Value<string>("ID")
                            , json_user.Value<string>("REALNAME"), json.Value<string>("DOREQUEST"), json.Value<string>("CLEARREMARK"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME")
                            , json.Value<string>("TEXTONE"), json.Value<string>("TEXTTWO"), json.Value<string>("NUMONE"), json.Value<string>("NUMTWO"), json.Value<string>("DATEONE")
                            , json.Value<string>("DATETWO"), json.Value<string>("USERNAMEONE"), json.Value<string>("USERIDONE"), json.Value<string>("USERNAMETWO"), json.Value<string>("USERIDTWO")
                            , json.Value<string>("BUSIITEMCODE"), json.Value<string>("BUSIITEMNAME")
                       );
                }
                else//修改
                {
                    ordercode = Request["ordercode"];
                    sql = @"UPDATE LIST_ORDER SET BUSITYPE='{1}',ENTRUSTTYPE='{2}',CUSTOMERCODE='{3}',CUSTOMERNAME='{4}',BUSIUNITCODE='{5}'             
                            ,BUSIUNITNAME='{6}',CLEARUNIT='{7}',CLEARUNITNAME='{8}',CUSNO='{9}',DOREQUEST='{10}'         
                            ,CLEARREMARK='{11}',RECEIVERUNITCODE='{12}',RECEIVERUNITNAME='{13}',TEXTONE='{14}',TEXTTWO='{15}'
                            ,NUMONE='{16}',NUMTWO='{17}',DATEONE=to_date('{18}','yyyy-MM-dd HH24:mi:ss'),DATETWO=to_date('{19}','yyyy-MM-dd HH24:mi:ss'),USERNAMEONE='{20}'
                            ,USERIDONE='{21}',USERNAMETWO='{22}',USERIDTWO='{23}',BUSIITEMCODE='{24}',BUSIITEMNAME='{25}'                               
                        WHERE CODE='{0}'                          
                        ";
                    sql = string.Format(sql
                            , ordercode, "70", json.Value<string>("ENTRUSTTYPE"), json.Value<string>("CUSTOMERCODE"), json.Value<string>("CUSTOMERNAME"), json.Value<string>("BUSIUNITCODE")
                            , json.Value<string>("BUSIUNITNAME"), json.Value<string>("CLEARUNIT"), json.Value<string>("CLEARUNITNAME"), json.Value<string>("CUSNO"),json.Value<string>("DOREQUEST")
                            , json.Value<string>("CLEARREMARK"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), json.Value<string>("TEXTONE"), json.Value<string>("TEXTTWO")
                            , json.Value<string>("NUMONE"), json.Value<string>("NUMTWO"), json.Value<string>("DATEONE"), json.Value<string>("DATETWO"), json.Value<string>("USERNAMEONE")
                            , json.Value<string>("USERIDONE"), json.Value<string>("USERNAMETWO"), json.Value<string>("USERIDTWO"), json.Value<string>("BUSIITEMCODE"), json.Value<string>("BUSIITEMNAME")
                            );

                }
                if (sql != "")
                {
                    int i = DBMgr.ExecuteNonQuery(sql);
                    if (i > 0)
                    {
                        resultmsg = "{success:true,ordercode:'" + ordercode + "',entrusttype:'" + json.Value<string>("ENTRUSTTYPE") + "'}";
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return resultmsg;
        }

        public string ExportOrderM()
        {
            string common_data_entrust = Request["common_data_entrust"];
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = QueryCondition();
            sql += " order by a.createtime desc";
            string common_data_busi = Request["common_data_busi"];

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

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
            string filename = "关务服务.xls";

            sheet_S = book.CreateSheet("关务服务");

            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("业务完成"); row1.CreateCell(1).SetCellValue("创建时间"); row1.CreateCell(2).SetCellValue("业务类别"); row1.CreateCell(3).SetCellValue("业务细项"); 
            row1.CreateCell(4).SetCellValue("委托单位"); 
            row1.CreateCell(5).SetCellValue("结算单位");row1.CreateCell(6).SetCellValue("经营单位"); row1.CreateCell(7).SetCellValue("企业编号"); row1.CreateCell(8).SetCellValue("订单编号");            
            row1.CreateCell(9).SetCellValue("操作需求"); row1.CreateCell(10).SetCellValue("结算备注"); row1.CreateCell(11).SetCellValue("文本1"); row1.CreateCell(12).SetCellValue("文本2");
            row1.CreateCell(13).SetCellValue("数字1"); row1.CreateCell(14).SetCellValue("数字2"); row1.CreateCell(15).SetCellValue("日期1"); row1.CreateCell(16).SetCellValue("人员1");
            row1.CreateCell(17).SetCellValue("日期2"); row1.CreateCell(18).SetCellValue("人员2"); 

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["SUBMITTIME"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["CREATETIME"].ToString());
                rowtemp.CreateCell(2).SetCellValue(GetName(dt.Rows[i]["ENTRUSTTYPE"].ToString(),common_data_entrust));
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["BUSIITEMNAME"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CUSTOMERNAME"].ToString());

                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CLEARUNITNAME"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["CUSNO"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["CODE"].ToString());

                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["DOREQUEST"].ToString());
                rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["CLEARREMARK"].ToString());
                rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["TEXTONE"].ToString());
                rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["TEXTTWO"].ToString());

                rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["NUMONE"].ToString());
                rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["NUMTWO"].ToString());
                rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["DATEONE"].ToString());
                rowtemp.CreateCell(16).SetCellValue(dt.Rows[i]["USERNAMEONE"].ToString());

                rowtemp.CreateCell(17).SetCellValue(dt.Rows[i]["DATETWO"].ToString());
                rowtemp.CreateCell(18).SetCellValue(dt.Rows[i]["USERNAMETWO"].ToString());
            }


            // 写入到客户端 
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //book.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", filename);

            return Extension.getPathname(filename, book);
        }

        public string GetName(string curstatus, string dec_insp_status)
        {
            string statusname = "";
            JArray jarray = JArray.Parse(dec_insp_status);
            foreach (JObject json in jarray)
            {
                if (json.Value<string>("CODE") == curstatus) { statusname = json.Value<string>("NAME"); break; }
            }
            return statusname;
        }

        #endregion

        #region 显示配置

        public string LoadList_customscost()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string sql = QueryCondition_customscost();

            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "a.entrusttypecode,a.busiitemcode,a.ORIGINNAME", "asc"));
            var json = JsonConvert.SerializeObject(dt, iso);

            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string QueryCondition_customscost()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string where = "";
            if (!string.IsNullOrEmpty(Request["entrusttype"]))
            {
                where += " and a.entrusttypecode='" + Request["entrusttype"].Trim() + "'";
            }
            if (!string.IsNullOrEmpty(Request["busiitemcode"]))//判断查询条件1是否有值
            {
                where += " and a.busiitemcode like '%" + Request["busiitemcode"].Trim() + "%'";
            }

            string sql = "select a.* from web_customscost a where 1=1" + where;

            return sql;
        }

        public string DelCusCost()
        {
            string result = "{success:false}";
            try
            {
                string sql = @"delete from web_customscost where id in(" + Request["ids"] + ")";
                int i = DBMgr.ExecuteNonQuery(sql);
                if (i > 0)
                {
                    result = "{success:true}";
                }
            }
            catch (Exception ex)
            {
                result = "{success:false}";
            }

            return result;
        }

        public string Save_customscost()
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(Request["formdata"]);
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = "";

            string msg = Check_customscost(json);
            if (msg != "")
            {
                return "{success:false,msg:'" + msg + "'}";
            }

            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                sql = @"insert into web_customscost (Id
                                        ,entrusttypecode,entrusttypename,Busiitemcode,Busiitemname,Originname,Configname
                                        ,Createuserid,Createusername
                                        )
                        values (web_customscost_id.nextval
                                    ,'{0}','{1}','{2}','{3}','{4}','{5}'
                                    ,'{6}','{7}')";
                sql = string.Format(sql
                    , json.Value<string>("ENTRUSTTYPECODE"), json.Value<string>("ENTRUSTTYPENAME"), json.Value<string>("BUSIITEMCODE"), json.Value<string>("BUSIITEMNAME"), json.Value<string>("ORIGINNAME"), json.Value<string>("CONFIGNAME").Trim()
                    , json_user.Value<string>("ID"), json_user.Value<string>("REALNAME")
                    );
            }
            else
            {
                sql = @"update web_customscost set entrusttypecode='{1}',entrusttypename='{2}',busiitemcode='{3}',busiitemname='{4}',Originname='{5}'
                                    ,Configname='{6}',createuserid='{7}',createusername='{8}',remark='{9}' 
                        where id='{0}'";
                sql = string.Format(sql, json.Value<string>("ID")
                   , json.Value<string>("ENTRUSTTYPECODE"), json.Value<string>("ENTRUSTTYPENAME"), json.Value<string>("BUSIITEMCODE"), json.Value<string>("BUSIITEMNAME"), json.Value<string>("ORIGINNAME")
                   , json.Value<string>("CONFIGNAME").Trim(), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"),json.Value<string>("REMARK")
                   );
            }
            int i = DBMgr.ExecuteNonQuery(sql);
            if (i > 0)
            {
                return "{success:true}";
            }
            else
            {
                return "{success:false}";
            }

        }

        public string Check_customscost(JObject json)
        {
            string msg = "";

            string strWhere = "";
            if (json.Value<string>("ID") != "")
            {
                strWhere = " and t1.id not in ('" + json.Value<string>("ID") + "') ";
            }

            string sqlStr = "select * from WEB_CUSTOMSCOST t1 where t1.ENTRUSTTYPECODE='{0}' and t1.BUSIITEMCODE='{1}' and t1.ORIGINNAME='{2}' " + strWhere;
            sqlStr = string.Format(sqlStr, json.Value<string>("ENTRUSTTYPECODE"), json.Value<string>("BUSIITEMCODE"), json.Value<string>("ORIGINNAME"));
            DataTable dt = DBMgr.GetDataTable(sqlStr);
            if (dt.Rows.Count > 0)
            {
                msg = "不能重复！";
            }
            return msg;
        }

        public string ExportCusCost()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = QueryCondition_customscost();
            sql += " order by a.entrusttypecode,a.busiitemcode,a.ORIGINNAME asc";

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

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
            string filename = "显示配置.xls";

            sheet_S = book.CreateSheet("显示配置");

            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("业务类别编号"); row1.CreateCell(1).SetCellValue("业务类别名称"); row1.CreateCell(2).SetCellValue("业务细项编号"); row1.CreateCell(3).SetCellValue("业务细项名称");
            row1.CreateCell(4).SetCellValue("文本名称"); row1.CreateCell(5).SetCellValue("配置名称"); row1.CreateCell(6).SetCellValue("创建人"); 

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["ENTRUSTTYPECODE"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["ENTRUSTTYPENAME"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["BUSIITEMCODE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["BUSIITEMNAME"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["ORIGINNAME"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONFIGNAME"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["CREATEUSERNAME"].ToString());
            }
            // 写入到客户端 
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //book.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", filename);

            return Extension.getPathname(filename, book);
        }

        #endregion

        #region 业务细项配置

        public string LoadList_customsconfig()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string sql = QueryCondition_customsconfig();

            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "a.entrusttypecode,a.busiitemcode", "asc"));
            var json = JsonConvert.SerializeObject(dt, iso);

            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string QueryCondition_customsconfig()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string where = "";
            if (!string.IsNullOrEmpty(Request["entrusttype"]))
            {
                where += " and a.entrusttypecode='" + Request["entrusttype"].Trim() + "'";
            }
            if (!string.IsNullOrEmpty(Request["busiitemcode"]))
            {
                where += " and a.busiitemcode like '%" + Request["busiitemcode"].Trim() + "%'";
            }
            if (!string.IsNullOrEmpty(Request["busiitemname"]))
            {
                where += " and a.busiitemname like '%" + Request["busiitemname"].Trim() + "%'";
            }
            if (!string.IsNullOrEmpty(Request["enable"]))
            {
                where += " and a.enable='" + Request["enable"].Trim() + "'";
            }

            string sql = "select a.* from web_customsconfig a where 1=1" + where;

            return sql;
        }

        public string Save_customsconfig()
        {
            JObject json = (JObject)JsonConvert.DeserializeObject(Request["formdata"]);
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = "";
            
            string msg = Check_customsconfig(json);
            if (msg != "")
            {
                return "{success:false,msg:'" + msg + "'}";
            }

            if (string.IsNullOrEmpty(json.Value<string>("ID")))
            {
                sql = @"insert into web_customsconfig (ID,starttime,enable
                                    ,entrusttypecode,entrusttypename,busiitemcode,busiitemname,createuserid,createusername
                                    )
                        values (web_customsconfig_id.nextval,sysdate,1
                                    ,'{0}','{1}','{2}','{3}','{4}','{5}')";
                sql = string.Format(sql
                    , json.Value<string>("ENTRUSTTYPECODE"), json.Value<string>("ENTRUSTTYPENAME"), json.Value<string>("BUSIITEMCODE"), json.Value<string>("BUSIITEMNAME"), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME")
                    );
            }
            else
            {
                sql = @"update web_customsconfig set entrusttypecode='{1}',entrusttypename='{2}',busiitemcode='{3}',busiitemname='{4}',enable='{5}'
                                    ,createuserid='{6}',createusername='{7}',remark='{8}',starttime=sysdate where id='{0}'
                                    ";
                sql = string.Format(sql, json.Value<string>("ID")
                   , json.Value<string>("ENTRUSTTYPECODE"), json.Value<string>("ENTRUSTTYPENAME"), json.Value<string>("BUSIITEMCODE").Trim(), json.Value<string>("BUSIITEMNAME").Trim(), json.Value<string>("ENABLE")
                   , json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"), json.Value<string>("REMARK")
                   );
            }
            int i = DBMgr.ExecuteNonQuery(sql);
            if (i > 0)
            {
                return "{success:true}";
            }
            else
            {
                return "{success:false}";
            }

        }

        public string Check_customsconfig(JObject json)
        {
            string msg = "";

            string strWhere = "";
            if (json.Value<string>("ID") != "")
            {
                strWhere = " and t1.id not in ('" + json.Value<string>("ID") + "') ";
            }

            string sqlStr = @"select * from web_customsconfig t1 where t1.entrusttypecode='" + json.Value<string>("ENTRUSTTYPECODE") + "' and t1.busiitemcode='" + json.Value<string>("BUSIITEMCODE").Trim() + "'";
            sqlStr += strWhere;
            DataTable dt = DBMgr.GetDataTable(sqlStr);
            if (dt.Rows.Count > 0)
            {
                msg = "业务类型+业务细项代码重复！";
            }

            string sqlStr2 = @"select * from web_customsconfig t1 where t1.entrusttypecode='" + json.Value<string>("ENTRUSTTYPECODE") + "' and t1.busiitemname='" + json.Value<string>("BUSIITEMNAME").Trim() + "'";
            sqlStr2 += strWhere;
            DataTable dt2 = DBMgr.GetDataTable(sqlStr2);
            if (dt2.Rows.Count > 0)
            {
                msg = "业务类型+业务细项名称重复！";
            }

            return msg;
        }

        public string DelCusConfig()
        {
            string result = "{success:false}";
            try
            {
                string sql = @"delete from web_customsconfig where id in(" + Request["ids"] + ")";
                int i = DBMgr.ExecuteNonQuery(sql);
                if (i > 0)
                {
                    result = "{success:true}";
                }
            }
            catch (Exception ex)
            {
                result = "{success:false}";
            }

            return result;
        }

        public string EnableCusConfig()
        {
            string result = "{success:false}";
            try
            {
                JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

                string sql = @"update web_customsconfig set enable={1},enableuserid='{2}',enableusername='{3}' where id in({0})";
                sql = string.Format(sql, Request["ids"], Request["enable"], json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"));

                int i = DBMgr.ExecuteNonQuery(sql);
                if (i > 0)
                {
                    result = "{success:true}";
                }
            }
            catch (Exception ex)
            {
                result = "{success:false}";
            }

            return result;
        }

        public string ExportCusConfig()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = QueryCondition_customsconfig();
            sql += " order by a.entrusttypecode,a.busiitemcode asc";

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

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
            string filename = "业务细项.xls";

            sheet_S = book.CreateSheet("业务细项");

            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("业务类别编号"); row1.CreateCell(1).SetCellValue("业务类别名称"); row1.CreateCell(2).SetCellValue("业务细项编号"); row1.CreateCell(3).SetCellValue("业务细项名称"); 
            row1.CreateCell(4).SetCellValue("创建时间");row1.CreateCell(5).SetCellValue("是否启用"); row1.CreateCell(6).SetCellValue("创建人"); row1.CreateCell(7).SetCellValue("启禁用人"); 
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["ENTRUSTTYPECODE"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["ENTRUSTTYPENAME"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["BUSIITEMCODE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["BUSIITEMNAME"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["STARTTIME"].ToString());

                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["ENABLE"].ToString() == "1" ? "是" : "否");
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["CREATEUSERNAME"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["ENABLEUSERNAME"].ToString());
            }
            // 写入到客户端 
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //book.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", filename);

            return Extension.getPathname(filename, book);

        }

        #endregion

    }
}
