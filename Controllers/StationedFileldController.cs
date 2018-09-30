﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using MvcPlatform.Common;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.IO;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;  //2007的版本及以上
using NPOI.XSSF.Util;

namespace MvcPlatform.Controllers
{
    [Authorize]
    [Filters.AuthFilter]
    public class StationedFileldController : Controller
    {
        int totalProperty = 0;
        private static readonly object objDelete = new object();
        private static readonly object objMaintance = new object();

        //
        // GET: /StationedFileld/

        public ActionResult EntryStationFileld()
        {
            ViewBag.navigator = "订单中心>>驻厂服务";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult AddStationField()//
        {
            ViewBag.navigator = "订单中心>>驻厂服务>>新增/修改";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult WebSiteQuery()//
        {
            ViewBag.navigator = "订单中心>>网站速查";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        private string getQueryCondition()
        {
            //Request["start"]
            string condition = string.Empty;

            if (Request["VALUE4_1"].ToString() != string.Empty)
            {
                condition += " and " + Request["CONDITION4"].ToString() + ">=to_date('" + Request["VALUE4_1"].ToString() + " 00:00:01','yyyy/mm/dd hh24:mi:ss')";
            }
            if (Request["VALUE4_2"].ToString() != string.Empty)
            {
                condition += " and " + Request["CONDITION4"].ToString() + "<=to_date('" + Request["VALUE4_2"].ToString() + " 23:59:59','yyyy/mm/dd hh24:mi:ss')";
            }
            if (Request["VALUE8_1"].ToString() != string.Empty)
            {
                condition += " and " + Request["CONDITION8"].ToString() + ">=to_date('" + Request["VALUE8_1"].ToString() + " 00:00:01','yyyy/mm/dd hh24:mi:ss')";
            }
            if (Request["VALUE8_2"].ToString() != string.Empty)
            {
                condition += " and " + Request["CONDITION8"].ToString() + "<=to_date('" + Request["VALUE8_2"].ToString() + " 23:59:59','yyyy/mm/dd hh24:mi:ss')";
            }
            //////
            if (Request["VALUE2"].ToString() != string.Empty)
            {

                condition += " and " + Request["CONDITION2"].ToString() + "='" + Request["VALUE2"].ToString() + "'";
            }
            if (Request["VALUE6"].ToString() != string.Empty)
            {

                condition += " and " + Request["CONDITION6"].ToString() + "='" + Request["VALUE6"].ToString() + "'";
            }
            //////
            if (Request["VALUE3"].ToString() != string.Empty)
            {

                condition += " and " + Request["CONDITION3"].ToString() + "='" + Request["VALUE3"].ToString() + "'";
            }
            if (Request["VALUE7"].ToString() != string.Empty)
            {

                condition += " and " + Request["CONDITION7"].ToString() + "='" + Request["VALUE7"].ToString() + "'";
            }
            //////
            if (Request["VALUE1"].ToString() != string.Empty)
            {
                if (Request["CONDITION1"].ToString() == "CUSNO")//企业编号模糊查询
                {
                    condition += " and " + Request["CONDITION1"].ToString() + " like '%" + Request["VALUE1"].ToString().Trim() + "%'";
                }
                else
                {
                    condition += " and " + Request["CONDITION1"].ToString() + "='" + Request["VALUE1"].ToString().Trim() + "'";
                }
            }
            if (Request["VALUE5"].ToString() != string.Empty)
            {
                if (Request["CONDITION5"].ToString() == "CUSNO")//企业编号模糊查询
                {
                    condition += " and " + Request["CONDITION5"].ToString() + " like '%" + Request["VALUE5"].ToString().Trim() + "%'";
                }
                else
                {
                    condition += " and " + Request["CONDITION5"].ToString() + "='" + Request["VALUE5"].ToString().Trim() + "'";
                }
            }
            return condition;
        }

        private string getQueryCondition2(string con)
        {
            JObject jsonCondition = (JObject)JsonConvert.DeserializeObject(con);
            //Request["start"]
            string condition = string.Empty;
            /////////
            if (jsonCondition.Value<string>("VALUE4_1") != string.Empty && jsonCondition.Value<string>("VALUE4_1") != "null")
            {
                condition += " and " + jsonCondition.Value<string>("CONDITION4") + ">=to_date('" + jsonCondition.Value<string>("VALUE4_1") + " 00:00:01','yyyy/mm/dd hh24:mi:ss')";
            }
            if (jsonCondition.Value<string>("VALUE4_2") != string.Empty && jsonCondition.Value<string>("VALUE4_2") != "null")
            {
                condition += " and " + jsonCondition.Value<string>("CONDITION4") + "<=to_date('" + jsonCondition.Value<string>("VALUE4_2") + " 23:59:59','yyyy/mm/dd hh24:mi:ss')";
            }
            if (jsonCondition.Value<string>("VALUE8_1") != string.Empty && jsonCondition.Value<string>("VALUE8_1") != "null")
            {
                condition += " and " + jsonCondition.Value<string>("CONDITION8") + ">=to_date('" + jsonCondition.Value<string>("VALUE8_1") + " 00:00:01','yyyy/mm/dd hh24:mi:ss')";
            }
            if (jsonCondition.Value<string>("VALUE8_2") != string.Empty && jsonCondition.Value<string>("VALUE8_2") != "null")
            {
                condition += " and " + jsonCondition.Value<string>("CONDITION8") + "<=to_date('" + jsonCondition.Value<string>("VALUE8_2") + " 23:59:59','yyyy/mm/dd hh24:mi:ss')";
            }
            //////
            if (jsonCondition.Value<string>("VALUE2") != string.Empty && jsonCondition.Value<string>("VALUE2") != "null")
            {

                condition += " and " +jsonCondition.Value<string>("CONDITION2") + "='" +jsonCondition.Value<string>("VALUE2") + "'";
            }
            if (jsonCondition.Value<string>("VALUE6") != string.Empty && jsonCondition.Value<string>("VALUE6") != "null")
            {

                condition += " and " +jsonCondition.Value<string>("CONDITION6") + "='" + jsonCondition.Value<string>("VALUE6") + "'";
            }
            //////
            if (jsonCondition.Value<string>("VALUE3") != string.Empty && jsonCondition.Value<string>("VALUE3") != "null")
            {

                condition += " and " +jsonCondition.Value<string>("CONDITION3") + "='" + jsonCondition.Value<string>("VALUE3") + "'";
            }
            if (jsonCondition.Value<string>("VALUE7") != string.Empty && jsonCondition.Value<string>("VALUE7") != "null")
            {

                condition += " and " +jsonCondition.Value<string>("CONDITION7") + "='" + jsonCondition.Value<string>("VALUE7") + "'";
            }
            ////////
            if (jsonCondition.Value<string>("VALUE1") != string.Empty && jsonCondition.Value<string>("VALUE1") != "null")//VALUE1
            {
                if (jsonCondition.Value<string>("CONDITION1") == "CUSNO")//企业编号模糊查询
                {
                    condition += " and " + jsonCondition.Value<string>("CONDITION1") + " like '%" + jsonCondition.Value<string>("VALUE1").Trim() + "%'";
                }
                else
                {
                    condition += " and " + jsonCondition.Value<string>("CONDITION1") + "='" + jsonCondition.Value<string>("VALUE1").Trim() + "'";
                }
            }
            if (jsonCondition.Value<string>("VALUE5") != string.Empty && jsonCondition.Value<string>("VALUE5") != "null")
            {
                if (jsonCondition.Value<string>("CONDITION5") == "CUSNO")//企业编号模糊查询
                {
                    condition += " and " + jsonCondition.Value<string>("CONDITION5") + " like '%" + jsonCondition.Value<string>("VALUE5").Trim() + "%'";
                }
                else
                {
                    condition += " and " + jsonCondition.Value<string>("CONDITION5") + "='" + jsonCondition.Value<string>("VALUE5").Trim() + "'";
                }
            }

            return condition;
        }



        private string getListSql()
        {
            string strSql = @"select h.CODE,h.SUBMITTIME,sta.name as STATUS,
case h.INSPFLAG when '0' then '否(0)' when '1' then '是(1)' end as INSPFLAG,
case h.MANIFEST when '0' then '否(0)' when '1' then '是(1)' end as MANIFEST,
h.CUSNO,h.CONTRACTNO,h.TOTALNO,h.DIVIDENO,h.GOODSNUM||'/'||h.GOODGW as GOODSNUM,busi.name as BUSITYPE,cus.name as PORTCODE,
trade.name as TRADEWAY,h.REMARK,h.DECLCODEQTY,h.DECLARATIONCODE,h.BUSIUNITNAME,h.SHIPPINGAGENT, h.INSPREMARK, h.COMMODITYNUM,
h.ACCEPTTIME,h.MOENDTIME,h.COENDTIME, h.RECOENDTIME,h.REPSTARTTIME,h.REPENDTIME,h.PASSTIME 
from RESIDENT_ORDER h
left join SYS_STATUS sta on sta.code=h.status
left join cusdoc.sys_busitype busi on busi.code=h.BUSITYPE
left join cusdoc.BASE_CUSTOMDISTRICT cus on cus.code=h.PORTCODE
left join cusdoc.BASE_DECLTRADEWAY trade on trade.code=h.TRADEWAY ";

            return strSql;
        }
        private string getSql_listExcel()
        {
            string strSql = @"select h.CODE as 订单编号,
h.SUBMITTIME as 委托时间,
h.SUBMITUSERNAME as 委托人,
sta.name  as 状态,
case h.INSPFLAG when '0' then '否(0)' when '1' then '是(1)' end  as 报检状态,
case h.MANIFEST when '0' then '否(0)' when '1' then '是(1)' end  as 舱单,
h.CUSNO as 企业编号,
h.CONTRACTNO as 合同号,
h.TOTALNO as 总单号,
h.DIVIDENO as 分单号,
h.GOODSNUM as 件数,
h.GOODGW as 毛重,
busi.name  as 业务类型,
cus.name  as 进出境关别,
trade.name  as 监管方式,
h.REMARK as 备注,
h.DECLCODEQTY as 报关单套数,
h.DECLARATIONCODE as 报关单号,
h.BUSIUNITNAME as 经营单位名称,
h.SHIPPINGAGENT as 货物代理,
h.INSPREMARK as 报检备注, 
h.COMMODITYNUM as 商品项数,
h.ACCEPTTIME as 受理时间,
h.ACCEPTUSERNAME as 受理人,
h.MOENDTIME as 制单完成时间,
h.MOENDNAME as 制单完成人,
h.COENDTIME as 审单完成时间, 
h.COENDNAME as 审单完成人,
h.RECOENDTIME as 复审完成时间,
h.RECOENDNAME as 复审完成人,
h.REPSTARTTIME as 申报时间,
h.REPSTARTNAME as 申报人,
h.REPENDTIME as 申报完成时间,
h.REPENDNAME as 申报完成人,
h.PASSTIME as 通关放行时间 ,
h.PASSNAME as 通关放行人
from RESIDENT_ORDER h
left join SYS_STATUS sta on sta.code=h.status
left join cusdoc.sys_busitype busi on busi.code=h.BUSITYPE
left join cusdoc.BASE_CUSTOMDISTRICT cus on cus.code=h.PORTCODE
left join cusdoc.BASE_DECLTRADEWAY trade on trade.code=h.TRADEWAY ";

            return strSql;
        }

        private string getSql_QDExcel()
        {
            string strSql = @"select

h.SUBMITTIME as 委托时间,
d.DECLARATIONCODE as 报关单号,
h.CONTRACTNO as 合同号,
h.TOTALNO ||'_'|| h.DIVIDENO as 总单号分单号,
h.GOODSNUM as 件数,
h.GOODGW as 毛重,
h.SHIPPINGAGENT as 货物代理

from RESIDENT_ORDER h
left join RESIDENT_DECLARATION d on h.code=d.ordercode
 ";

            return strSql;
        }

        public string LoadList()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            //string sql = QueryCondition();

//            string strSql = @"select h.CODE,h.SUBMITTIME,sta.name as STATUS,
//case h.INSPFLAG when '0' then '否(0)' when '1' then '是(1)' end as INSPFLAG,
//case h.MANIFEST when '0' then '否(0)' when '1' then '是(1)' end as MANIFEST,
//h.CUSNO,h.CONTRACTNO,h.TOTALNO,h.DIVIDENO,h.GOODSNUM||'/'||h.GOODGW as GOODSNUM,busi.name as BUSITYPE,cus.name as PORTCODE,
//trade.name as TRADEWAY,h.REMARK,h.DECLCODEQTY,h.DECLARATIONCODE,h.BUSIUNITNAME,h.SHIPPINGAGENT, h.INSPREMARK, h.COMMODITYNUM,
//h.ACCEPTTIME,h.MOENDTIME,h.COENDTIME, h.RECOENDTIME,h.REPSTARTTIME,h.REPENDTIME,h.PASSTIME 
//from RESIDENT_ORDER h
//left join SYS_STATUS sta on sta.code=h.status
//left join cusdoc.sys_busitype busi on busi.code=h.BUSITYPE
//left join cusdoc.BASE_CUSTOMDISTRICT cus on cus.code=h.PORTCODE
//left join cusdoc.BASE_DECLTRADEWAY trade on trade.code=h.TRADEWAY 
//
//where 1=1 " + getQueryCondition()+" and RECEIVERUNITCODE='" +json_user.Value<string>("CUSTOMERCODE")+"' order by h.SUBMITTIME desc";
            string strSql = getListSql() + @" where 1=1 " + getQueryCondition() + " and RECEIVERUNITCODE='" + json_user.Value<string>("CUSTOMERCODE") + "' order by h.SUBMITTIME desc";

            DataTable dt = GetData(strSql);
            var json = JsonConvert.SerializeObject(dt, iso);

            //List<string> listSqls = new List<string>();
            //object obj = Session["listSqls"];
            //if (obj == null)
            //{
            //    listSqls.Add(strSql);
            //    Session["listSqls"] = listSqls;
            //}
            //else
            //{
            //   listSqls = (List<string>)obj;
            //   listSqls.Add(strSql);
            //   Session["listSqls"] = listSqls;
            //}

            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        private DataTable GetData(string strSql)
        {
           int start = Convert.ToInt32(Request["start"]);
           int limit = Convert.ToInt32(Request["limit"]);

            start = start / limit + 1;

          //  int start = 1 + Convert.ToInt32(Request["start"]);
            string sql = "select count(1) from ( " + strSql + " )";
            totalProperty = Convert.ToInt32(DBMgr.GetDataTable(sql).Rows[0][0]);

            return DBMgr.GetDataTable(GetQuerySqlPage(strSql, start, limit)); 
        }
        private string GetQuerySqlPage(string strSql, int pageIndex, int pageSize)
        {
            string sql = @"SELECT * FROM (SELECT ROWNUM AS rowno, t.* FROM ( " + strSql + @" ) t WHERE ROWNUM <= " + pageIndex * pageSize + " ) table_alias  WHERE table_alias.rowno >  " + (pageIndex - 1) * pageSize;
            return sql;
        }

        public string Create_Save()
        {
            try
            {

                JObject jsonOrderdata = (JObject)JsonConvert.DeserializeObject(Request["formOrderdata"]);
                JObject jsonOrderTimedata = (JObject)JsonConvert.DeserializeObject(Request["formOrderTimedata"]);
                JArray jarryDeclData = (JArray)JsonConvert.DeserializeObject(Request["formDeclData"]);
                JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

                string code = jsonOrderdata.Value<string>("CODE");//

                string flag = string.Empty;

                string DECLCODEQTY = string.Empty;
                string DECLARATIONCODE = string.Empty;
                if (jarryDeclData.Count > 0)
                {
                    DECLCODEQTY = jarryDeclData.Count.ToString();
                    if (jarryDeclData.Count == 1)
                    {
                        DECLARATIONCODE = jarryDeclData[0].Value<string>("DECLARATIONCODE");
                    }
                    else
                    {
                        DECLARATIONCODE = jarryDeclData[0].Value<string>("DECLARATIONCODE") + "..."; ;
                    }
                }
                string status = string.Empty;

                List<string> listSqls = new List<string>();
                string strSql = string.Empty;
                if (code == string.Empty)
                {
                    code = Extension.getOrderCode();
                    flag = "add";
                }
                else
                {
                    flag = "modify";

                    strSql = "delete from RESIDENT_ORDER where code='" + code + "'";
                    listSqls.Add(strSql);
                    strSql = "delete from RESIDENT_DECLARATION where ORDERCODE='" + code + "'";
                    listSqls.Add(strSql);
                }
                //保存业务信息
                string BUSIUNITNAME = jsonOrderdata.Value<string>("BUSIUNITNAME");
                string MANIFEST = jsonOrderdata.Value<string>("MANIFEST");
                string INSPFLAG = jsonOrderdata.Value<string>("INSPFLAG");
                if (MANIFEST == "on" || MANIFEST == "true" || MANIFEST == "1")
                {
                    MANIFEST = "1";
                }
                else
                {
                    MANIFEST = "0";
                }

                if (INSPFLAG == "on" || INSPFLAG == "true" || INSPFLAG == "1")
                {
                    INSPFLAG = "1";
                }
                else
                {
                    INSPFLAG = "0";
                }

                if (BUSIUNITNAME.Contains("("))
                {
                    BUSIUNITNAME = BUSIUNITNAME.Split('(')[0];
                }
                strSql = @"insert into RESIDENT_ORDER (code,cusno,busitype,tradeway,portcode,busiunitcode,busiunitname,goodsnum,goodgw,contractno,
TOTALNO,DIVIDENO,MANIFEST,INSPFLAG,REMARK,RECEIVERUNITCODE,RECEIVERUNITNAME,CREATETIME,DECLCODEQTY,DECLARATIONCODE,SHIPPINGAGENT, INSPREMARK, COMMODITYNUM) 
            values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}',sysdate,'{17}','{18}','{19}','{20}','{21}')";
                strSql = string.Format(strSql, code, jsonOrderdata.Value<string>("CUSNO").Trim(), jsonOrderdata.Value<string>("BUSITYPE"), jsonOrderdata.Value<string>("TRADEWAY2")
                    , jsonOrderdata.Value<string>("PORTCODE"), jsonOrderdata.Value<string>("BUSIUNITCODE"), BUSIUNITNAME
                    , jsonOrderdata.Value<string>("GOODSNUM2"), jsonOrderdata.Value<string>("GOODGW2")
                    , jsonOrderdata.Value<string>("CONTRACTNO").Trim(), jsonOrderdata.Value<string>("TOTALNO").Trim(), jsonOrderdata.Value<string>("DIVIDENO").Trim()
                    , MANIFEST, INSPFLAG, jsonOrderdata.Value<string>("REMARK2").Trim()
                    , json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), DECLCODEQTY, DECLARATIONCODE
                    , jsonOrderdata.Value<string>("SHIPPINGAGENT").Trim(), jsonOrderdata.Value<string>("INSPREMARK").Trim(), jsonOrderdata.Value<string>("COMMODITYNUM"));
                listSqls.Add(strSql);

                //  strSql="update RESIDENT_ORDER set ";
                strSql = string.Empty;
                string sysdate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                status = "10";//已委托
                if (jsonOrderTimedata.Value<string>("SUBMITTIME") != "null" && jsonOrderTimedata.Value<string>("SUBMITTIME") != "")//
                {
                    strSql += " SUBMITTIME=to_date('" + jsonOrderTimedata.Value<string>("SUBMITTIME") + "','yyyy/mm/dd hh24:mi:ss'),SUBMITUSERID='" + jsonOrderTimedata.Value<string>("SUBMITUSERID") + "',SUBMITUSERNAME='" + jsonOrderTimedata.Value<string>("SUBMITUSERNAME") + "', ";
                }
                else
                {
                    strSql += " SUBMITTIME=to_date('" + sysdate + "','yyyy/mm/dd hh24:mi:ss'),SUBMITUSERID='" + json_user.Value<string>("ID") + "',SUBMITUSERNAME='" + json_user.Value<string>("REALNAME") + "', ";
                }
                if (jsonOrderTimedata.Value<string>("ACCEPTTIME") != "null" && jsonOrderTimedata.Value<string>("ACCEPTTIME") != "")
                {
                    status = "15";//已受理
                    strSql += " ACCEPTTIME=to_date('" + jsonOrderTimedata.Value<string>("ACCEPTTIME") + "','yyyy/mm/dd hh24:mi:ss'),ACCEPTUSERID='" + jsonOrderTimedata.Value<string>("ACCEPTUSERID") + "',ACCEPTUSERNAME='" + jsonOrderTimedata.Value<string>("ACCEPTUSERNAME") + "', ";
                }
                if (jsonOrderTimedata.Value<string>("MOENDTIME") != "null" && jsonOrderTimedata.Value<string>("MOENDTIME") != "")
                {
                    status = "30";//制单完成
                    strSql += " MOENDTIME=to_date('" + jsonOrderTimedata.Value<string>("MOENDTIME") + "','yyyy/mm/dd hh24:mi:ss'),MOENDID='" + jsonOrderTimedata.Value<string>("MOENDID") + "',MOENDNAME='" + jsonOrderTimedata.Value<string>("MOENDNAME") + "', ";
                }
                if (jsonOrderTimedata.Value<string>("COENDTIME") != "null" && jsonOrderTimedata.Value<string>("COENDTIME") != "")
                {
                    status = "50";//审核完成
                    strSql += " COENDTIME=to_date('" + jsonOrderTimedata.Value<string>("COENDTIME") + "','yyyy/mm/dd hh24:mi:ss'),COENDID='" + jsonOrderTimedata.Value<string>("COENDID") + "',COENDNAME='" + jsonOrderTimedata.Value<string>("COENDNAME") + "', ";
                }
                if (jsonOrderTimedata.Value<string>("RECOENDTIME") != "null" && jsonOrderTimedata.Value<string>("RECOENDTIME") != "")
                {
                    status = "55";//复审完成
                    strSql += " RECOENDTIME=to_date('" + jsonOrderTimedata.Value<string>("RECOENDTIME") + "','yyyy/mm/dd hh24:mi:ss'),RECOENDID='" + jsonOrderTimedata.Value<string>("RECOENDID") + "',RECOENDNAME='" + jsonOrderTimedata.Value<string>("RECOENDNAME") + "', ";
                }
                if (jsonOrderTimedata.Value<string>("REPSTARTTIME") != "null" && jsonOrderTimedata.Value<string>("REPSTARTTIME") != "")
                {
                    status = "100";//已申报
                    strSql += " REPSTARTTIME=to_date('" + jsonOrderTimedata.Value<string>("REPSTARTTIME") + "','yyyy/mm/dd hh24:mi:ss'),REPSTARTID='" + jsonOrderTimedata.Value<string>("REPSTARTID") + "',REPSTARTNAME='" + jsonOrderTimedata.Value<string>("REPSTARTNAME") + "', ";
                }
                if (jsonOrderTimedata.Value<string>("REPENDTIME") != "null" && jsonOrderTimedata.Value<string>("REPENDTIME") != "")
                {
                    status = "120";//申报完成
                    strSql += " REPENDTIME=to_date('" + jsonOrderTimedata.Value<string>("REPENDTIME") + "','yyyy/mm/dd hh24:mi:ss'),REPENDID='" + jsonOrderTimedata.Value<string>("REPENDID") + "',REPENDNAME='" + jsonOrderTimedata.Value<string>("REPENDNAME") + "', ";
                }
                if (jsonOrderTimedata.Value<string>("PASSTIME") != "null" && jsonOrderTimedata.Value<string>("PASSTIME") != "")
                {
                    status = "160";//通关放行
                    strSql += " PASSTIME=to_date('" + jsonOrderTimedata.Value<string>("PASSTIME") + "','yyyy/mm/dd hh24:mi:ss'),PASSID='" + jsonOrderTimedata.Value<string>("PASSID") + "',PASSNAME='" + jsonOrderTimedata.Value<string>("PASSNAME") + "', ";
                }
                if (strSql != string.Empty)
                {
                    //strSql = strSql.Trim();
                    //strSql = strSql.Substring(0, strSql.Length - 1);
                    strSql = "update RESIDENT_ORDER set " + strSql + " status=" + status + " where code='" + code + "'";
                    listSqls.Add(strSql);
                }

                //保存表体  
                int count = 1;
                foreach (JObject obj in jarryDeclData)
                {
                    strSql = @"insert into RESIDENT_DECLARATION (ordercode,declarationcode,tradeway,sheetnum,modifyflag,goodsnum,goodgw,customsstatus,createtime,tradewayNAME,no) 
values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',sysdate,'{8}','{9}')";
                    strSql = string.Format(strSql, code, obj.Value<string>("DECLARATIONCODE"), obj.Value<string>("TRADEWAY"), obj.Value<string>("SHEETNUM")
                        , obj.Value<string>("MODIFYFLAG"), obj.Value<string>("GOODSNUM"), obj.Value<string>("GOODGW"), obj.Value<string>("CUSTOMSSTATUS")
                        , obj.Value<string>("TRADEWAYNAME"), count);
                    listSqls.Add(strSql);
                    count++;
                }
                DBMgr.ExecuteNonQueryBatch(listSqls);
                //保存成功后在重新拉一下数据
                return loadrecord(code);
            }
            catch (Exception ex)
            {
                return "{success:false,msg:'保存失败：" + ex.Message + "'}";
            }
        }

        public string loadrecord(string ordercode="")
        {
            if (ordercode == "")
            {
                ordercode = Request["ordercode"];
            }
            if (ordercode == "")
            {
                return "{success:false,msg:'订单编号为空'}";
            }

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string formOrderData = "{}";
            string formDeclData = "[]";

            string strSql = @"select code,cusno,busitype,tradeway as TRADEWAY2,portcode,busiunitcode,busiunitname,busiunitnum
,goodsnum as GOODSNUM2,goodgw as GOODGW2,contractno,totalno,divideno,manifest,status,inspflag,remark as REMARK2,submittime,submituserid,submitusername,accepttime,acceptuserid,acceptusername,moendtime,moendid,moendname,coendtime,coendid,coendname,recoendtime,recoendid,recoendname,repstarttime,repstartid,repstartname,rependtime
,rependid,rependname,passtime,passid,passname,receiverunitcode,receiverunitname,createtime,declcodeqty,declarationcode,
SHIPPINGAGENT, INSPREMARK, COMMODITYNUM  from RESIDENT_ORDER where code='" + ordercode + "'";
            formOrderData = JsonConvert.SerializeObject(DBMgr.GetDataTable(strSql), iso).TrimStart('[').TrimEnd(']');

            strSql = "select * from RESIDENT_DECLARATION where ordercode='"+ordercode+"' order by NO";
            formDeclData = JsonConvert.SerializeObject(DBMgr.GetDataTable(strSql), iso);

            return "{success:true,formOrderData:" + formOrderData + ",formDeclData:" + formDeclData + "}";
        }

        public string DeleteOrder()
        {
            try
            {
                JArray orderData = (JArray)JsonConvert.DeserializeObject(Request["formdata"]);
                List<string> listSqls = new List<string>();
                string strSql = string.Empty;

                string code = string.Empty;
                if (orderData.Count > 0)
                {
                    foreach (JObject obj in orderData)
                    {
                        code += "'" + obj.Value<string>("CODE") + "',";

                        strSql = "delete from RESIDENT_ORDER where code='" + obj.Value<string>("CODE") + "'";
                        listSqls.Add(strSql);
                        strSql = @"delete from RESIDENT_DECLARATION d 
where d.ordercode='" + obj.Value<string>("CODE") + "'";
                        listSqls.Add(strSql);
                    }
                    code = code.Substring(0, code.Length - 1);
                    lock (objDelete)
                    {
                        strSql = "select code from RESIDENT_ORDER where code in (" + code + ") and status<>10";
                        DataTable dt = DBMgr.GetDataTable(strSql);
                        if (dt.Rows.Count > 0)
                        {
                            return "{success:false,msg:'保存失败：有单子状态大于已委托，不可删除'}";
                        }
                        DBMgr.ExecuteNonQueryBatch(listSqls);
                        return "{success:true}";
                    }
                }
                else
                {
                    return "{success:false,msg:'保存失败：未找到要删除的数据'}";
                }
            }
            catch (Exception ex)
            {
                return "{success:false,msg:'保存失败：" + ex.Message + "'}";
            }
        }

        public string getServerTime()
        {
            return "{success:true,time:'"+System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"'}";
        }

        public string batchMaintance(string codes, string id)
        {
            try
            {
                string status = "";
                string time = "";
                string userid = "";
                string username = "";
                switch (id)
                {
                    case "maintance_ACCEPTTIME"://受理时间
                        status = "15";
                        userid = "ACCEPTUSERID";
                        username = "ACCEPTUSERNAME";
                        time = "ACCEPTTIME";
                        break;
                    case "maintance_MOENDTIME"://制单完成时间
                        status = "30";
                        userid = "MOENDID";
                        username = "MOENDNAME";
                        time = "MOENDTIME";
                        break;
                    case "maintance_COENDTIME"://审核完成时间
                        status = "50";
                        userid = "COENDID";
                        username = "COENDNAME";
                        time = "COENDTIME";
                        break;
                    case "maintance_RECOENDTIME"://复审完成时间
                        status = "55";
                        userid = "RECOENDID";
                        username = "RECOENDNAME";
                        time = "RECOENDTIME";
                        break;
                    case "maintance_REPSTARTTIME"://申报时间
                        status = "100";
                        userid = "REPSTARTID";
                        username = "REPSTARTNAME";
                        time = "REPSTARTTIME";
                        break;
                    case "maintance_REPENDTIME"://申报完成时间
                        status = "120";
                        userid = "REPENDID";
                        username = "REPENDNAME";
                        time = "REPENDTIME";
                        break;
                    case "maintance_PASSTIME"://通关放行时间
                        status = "160";
                        userid = "PASSID";
                        username = "PASSNAME";
                        time = "PASSTIME";
                        break;

                }

                List<string> listSqls = getMainSqls(codes, status, time,userid, username);
                int successQty = 0;
                lock (objMaintance)
                {
                    successQty=DBMgr.ExecuteNonQueryBatch(listSqls);
                }

                return "{success:true,successQty:" + successQty + "}";
            }
            catch (Exception ex)
            {
                return "{success:false,msg:'保存失败：" + ex.Message + "'}";
            }
        }
        private List<string> getMainSqls(string codes, string status, string time, string userid, string username)
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string[] listCodes = codes.Split(',');

            List<string> listSqls = new List<string>();
            string strSql = string.Empty;
            foreach (string code in listCodes)
            {
                strSql = "update RESIDENT_ORDER set status={0} where code='{1}' and {2} is null and nvl(status,0)<{3}";
                strSql = string.Format(strSql,status,code,time,status);
                listSqls.Add(strSql);

                strSql = "update RESIDENT_ORDER set {0}=sysdate,{1}='{2}',{3}='{4}' where code='{5}' and {6} is null ";
                strSql = string.Format(strSql, time, userid, json_user.Value<string>("ID"), username, json_user.Value<string>("REALNAME"),code,time);
                listSqls.Add(strSql);
            }
            return listSqls;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="type">类型 0：列表导出 1：代理清单导出</param>
        /// <returns></returns>
        public ActionResult ExportExcel(string type,string condition)
        {
            string fileName = string.Empty;
            switch (type)
            {
                case "0":
                    fileName = "驻厂服务列表导出_" + System.DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
                    break;
                case "1":
                    fileName = "驻厂服务代理清单导出_" + System.DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
                    break;
            }
            //XSSFWorkbook workbook = new XSSFWorkbook();
            //XSSFSheet sheet1 = (XSSFSheet)workbook.CreateSheet("驻厂服务");
            //XSSFRow row1 = (XSSFRow)sheet1.CreateRow(0);

           XSSFWorkbook workbook=  getExcelData(type, condition);

            Models.NpoiMemoryStream ms = new Models.NpoiMemoryStream();
            //MemoryStream ms = new MemoryStream();
            ms.AllowClose = false;
            workbook.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;
            // ms.Seek(0, SeekOrigin.Begin);
            // ms.Position = 0;

            workbook.Close();

            return File(ms, "application/vnd.ms-excel", fileName); 
        }

        private XSSFWorkbook getExcelData(string type, string condition)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            XSSFSheet sheet1 = (XSSFSheet)workbook.CreateSheet("驻厂服务");
            //XSSFRow row1 = (XSSFRow)sheet1.CreateRow(0);

            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string strSql =string.Empty;
            switch (type)
            {
                case "0"://列表
                    strSql = getSql_listExcel() + @" where 1=1 " + getQueryCondition2(condition) + " and RECEIVERUNITCODE='" + json_user.Value<string>("CUSTOMERCODE") + "' order by h.SUBMITTIME desc";
                    break;
                case "1"://代理清单
                    strSql = getSql_QDExcel() + @" where 1=1 " + getQueryCondition2(condition) + " and RECEIVERUNITCODE='" + json_user.Value<string>("CUSTOMERCODE") + "' order by h.SUBMITTIME desc";
                    break;
            }
            int qty = Convert.ToInt32(DBMgr.GetDataTable("select count(1) from ( " + strSql + " )").Rows[0][0]);

            int index = 0;
            int pageSize = 5000;
            int num = qty / pageSize;//求整
            int mod = qty % pageSize;//求余
            if (mod > 0)
            {
                num += 1;
            }
            int lastCount = index ;
            XSSFRow row1;
            for (int i = 1; i <= num; i++)
            {
                DataTable dt = DBMgr.GetDataTable(GetQuerySqlPage(strSql,i,pageSize));

                if (i == 1)
                {
                    row1 = (XSSFRow)sheet1.CreateRow(index);
                    for (int j = 1; j < dt.Columns.Count; j++)
                    {
                        sheet1.SetColumnWidth(j - 1, 4000);
                        row1.CreateCell(j-1).SetCellValue(dt.Columns[j].ColumnName);
                    }
                    index++;
                }

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    row1 = (XSSFRow)sheet1.CreateRow(index);
                    for (int k = 1; k < dt.Columns.Count; k++)
                    {
                        row1.CreateCell(k-1).SetCellValue(dt.Rows[j][dt.Columns[k].ColumnName].ToString());
                    }
                    index++;
                }
            }
            return workbook;
        }


    }
}
