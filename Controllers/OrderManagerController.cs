﻿using MvcPlatform.Common;
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

            string sql = @"select a.ID,a.BUSITYPE,a.CREATETIME,a.SUBMITTIME,a.ENTRUSTTYPE,a.CUSTOMERCODE,a.CUSTOMERNAME,a.CLEARUNIT,a.CLEARUNITNAME 
                                ,a.BUSIUNITCODE,a.BUSIUNITNAME,a.CODE,a.CUSNO,a.DOREQUEST,a.CLEARREMARK
                                ,b.busiitemname ENTRUSTTYPENAME
                                ,a.TEXTONE,a.TEXTTWO,a.NUMONE,a.NUMTWO,to_char(a.DATEONE,'yyyy-mm-dd') DATEONE,to_char(a.DATETWO,'yyyy-mm-dd') DATETWO
                                ,a.USERNAMEONE,a.USERIDONE,a.USERNAMETWO,a.USERIDTWO 
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

        public string CompleteOrderM()
        {
            string ordercode = Request["ordercode"]; string result = "{success:true}";

            string sql = @"select SUBMITTIME from list_order where code='" + ordercode + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            if (dt.Rows[0]["SUBMITTIME"].ToString() != "")
            {
                return "{success:false,flag:'E'}";
            }

            sql = @"update list_order set SUBMITTIME=sysdate where code='" + ordercode + "'";
            DBMgr.ExecuteNonQuery(sql);
            
            return result;
        }

        public string DeleteOrderM()
        {
            string ordercode = Request["ordercode"]; string result = "{success:true}";

            string sql = @"select SUBMITTIME from list_order where code='" + ordercode + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            if (dt.Rows[0]["SUBMITTIME"].ToString() != "")
            {
                return "{success:false,flag:'E'}";
            }

            sql = @"delete list_order where code='" + ordercode + "'";
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

        public string Getlabelname(string busitype, string entrusttype)
        {
            string sql = string.Empty; 

            DataTable dt_field = new DataTable();
            sql = @"select originname,configname 
                    from web_customscost 
                    where busitypecode='" + busitype + "' and busiitemcode='" + entrusttype + "'";
            dt_field = DBMgr.GetDataTable(sql);
            var fieldcolumn = JsonConvert.SerializeObject(dt_field);

            //-------------------------------------------------------------------------------------------------------------------

            return "{fieldcolumn:" + fieldcolumn + "}";
        }

        public string loadform_OrderM(string ordercode, string pagename, string configtype)
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = "";
            string result = "{}"; string formdata = "{}";
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
                            ,CREATEUSERNAME,CREATETIME,SUBMITUSERID,SUBMITUSERNAME,SUBMITTIME,DOREQUEST,CLEARREMARK
                            ,TEXTONE,TEXTTWO,NUMONE,NUMTWO,to_char(DATEONE,'yyyy-mm-dd') DATEONE,to_char(DATETWO,'yyyy-mm-dd') DATETWO,USERNAMEONE,USERIDONE,USERNAMETWO,USERIDTWO 
                        from list_order where code='" + ordercode + "'";
                DataTable dt = new DataTable();
                dt = DBMgr.GetDataTable(sql);
                formdata = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');                
            }
            result = "{formdata:" + formdata + ",curuser:" + curuser + "}";
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
                        ) VALUES (LIST_ORDER_id.Nextval
                            ,'{0}','{1}','{2}','{3}','{4}','{5}'
                            ,'{6}','{7}','{8}','{9}','{10}'
                            ,'{11}',sysdate,'{12}','{13}','{14}','{15}'     
                            ,'{16}','{17}','{18}','{19}',to_date('{20}','yyyy-MM-dd HH24:mi:ss')
                            ,to_date('{21}','yyyy-MM-dd HH24:mi:ss'),'{22}','{23}','{24}','{25}'                       
                            )";
                    sql = string.Format(sql
                            , json.Value<string>("BUSITYPE"), ordercode, json.Value<string>("ENTRUSTTYPE"), json.Value<string>("CUSTOMERCODE"), json.Value<string>("CUSTOMERNAME"), json.Value<string>("BUSIUNITCODE")
                            , json.Value<string>("BUSIUNITNAME"), json.Value<string>("CLEARUNIT"), json.Value<string>("CLEARUNITNAME"), json.Value<string>("CUSNO"), json_user.Value<string>("ID")
                            , json_user.Value<string>("REALNAME"), json.Value<string>("DOREQUEST"), json.Value<string>("CLEARREMARK"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME")
                            , json.Value<string>("TEXTONE"), json.Value<string>("TEXTTWO"), json.Value<string>("NUMONE"), json.Value<string>("NUMTWO"), json.Value<string>("DATEONE")
                            , json.Value<string>("DATETWO"), json.Value<string>("USERNAMEONE"), json.Value<string>("USERIDONE"), json.Value<string>("USERNAMETWO"), json.Value<string>("USERIDTWO")
                       );
                }
                else//修改
                {
                    ordercode = Request["ordercode"];
                    sql = @"UPDATE LIST_ORDER SET BUSITYPE='{1}',ENTRUSTTYPE='{2}',CUSTOMERCODE='{3}',CUSTOMERNAME='{4}',BUSIUNITCODE='{5}'             
                            ,BUSIUNITNAME='{6}',CLEARUNIT='{7}',CLEARUNITNAME='{8}',CUSNO='{9}',DOREQUEST='{10}'         
                            ,CLEARREMARK='{11}',RECEIVERUNITCODE='{12}',RECEIVERUNITNAME='{13}',TEXTONE='{14}',TEXTTWO='{15}'
                            ,NUMONE='{16}',NUMTWO='{17}',DATEONE=to_date('{18}','yyyy-MM-dd HH24:mi:ss'),DATETWO=to_date('{19}','yyyy-MM-dd HH24:mi:ss'),USERNAMEONE='{20}'
                            ,USERIDONE='{21}',USERNAMETWO='{22}',USERIDTWO='{23}'                               
                        WHERE CODE='{0}'                          
                        ";
                    sql = string.Format(sql
                            , ordercode, json.Value<string>("BUSITYPE"), json.Value<string>("ENTRUSTTYPE"), json.Value<string>("CUSTOMERCODE"), json.Value<string>("CUSTOMERNAME"), json.Value<string>("BUSIUNITCODE")
                            , json.Value<string>("BUSIUNITNAME"), json.Value<string>("CLEARUNIT"), json.Value<string>("CLEARUNITNAME"), json.Value<string>("CUSNO"),json.Value<string>("DOREQUEST")
                            , json.Value<string>("CLEARREMARK"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), json.Value<string>("TEXTONE"), json.Value<string>("TEXTTWO")
                            , json.Value<string>("NUMONE"), json.Value<string>("NUMTWO"), json.Value<string>("DATEONE"), json.Value<string>("DATETWO"), json.Value<string>("USERNAMEONE")
                            , json.Value<string>("USERIDONE"), json.Value<string>("USERNAMETWO"), json.Value<string>("USERIDTWO")
                            );

                }
                if (sql != "")
                {
                    int i = DBMgr.ExecuteNonQuery(sql);
                    if (i > 0)
                    {
                        resultmsg = "{success:true,ordercode:'" + ordercode + "'}";
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
            string filename = "关务服务2.xls";

            sheet_S = book.CreateSheet("关务服务2"); 

            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("业务完成"); row1.CreateCell(1).SetCellValue("创建时间"); row1.CreateCell(2).SetCellValue("业务细项"); row1.CreateCell(3).SetCellValue("委托单位"); row1.CreateCell(4).SetCellValue("结算单位");
            row1.CreateCell(5).SetCellValue("经营单位"); row1.CreateCell(6).SetCellValue("业务类型"); row1.CreateCell(7).SetCellValue("企业编号"); row1.CreateCell(8).SetCellValue("订单编号");
            row1.CreateCell(9).SetCellValue("操作需求"); row1.CreateCell(10).SetCellValue("结算备注"); row1.CreateCell(11).SetCellValue("文本1"); row1.CreateCell(12).SetCellValue("文本2");
            row1.CreateCell(13).SetCellValue("数字1"); row1.CreateCell(14).SetCellValue("数字2"); row1.CreateCell(15).SetCellValue("日期1"); row1.CreateCell(16).SetCellValue("人员1");
            row1.CreateCell(17).SetCellValue("日期2"); row1.CreateCell(18).SetCellValue("人员2"); 

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["SUBMITTIME"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["CREATETIME"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["ENTRUSTTYPENAME"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CUSTOMERNAME"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CLEARUNITNAME"].ToString());

                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                rowtemp.CreateCell(6).SetCellValue(getName(dt.Rows[i]["BUSITYPE"].ToString(), common_data_busi));
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

        public string getName(string curstatus, string dec_insp_status)
        {
            string statusname = "";
            JArray jarray = JArray.Parse(dec_insp_status);
            foreach (JObject json in jarray)
            {
                if (json.Value<string>("CODE") == curstatus) { statusname = json.Value<string>("NAME"); break; }
            }
            return statusname;
        }

    }
}