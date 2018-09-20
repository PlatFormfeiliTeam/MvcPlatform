using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using MvcPlatform.Common;
using System.Data;

namespace MvcPlatform.Controllers
{
    public class StationedFileldController : Controller
    {
        int totalProperty = 0;
        private readonly object objDelete = new object();
        //
        // GET: /StationedFileld/

        public ActionResult EntryStationFileld()
        {
            ViewBag.navigator = "现场服务>>驻场管理";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult AddStationField()
        {
            ViewBag.navigator = "现场服务>>驻场管理>>新增";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        private string getQueryCondition()
        {
            //Request["start"]
            string condition = string.Empty;
            if (Request["VALUE1"].ToString() != string.Empty)
            {

                condition += " and " + Request["CONDITION1"].ToString() + "='" + Request["VALUE1"].ToString() + "'";
            }
            if (Request["VALUE5"].ToString() != string.Empty)
            {

                condition += " and " + Request["CONDITION5"].ToString() + "='" + Request["VALUE5"].ToString() + "'";
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
            //
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
            return condition;
        }

        public string LoadList()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            //string sql = QueryCondition();
            string strSql = @"select h.CODE,h.SUBMITTIME,sta.name as STATUS,
case h.INSPFLAG when '0' then '否(0)' when '1' then '是(1)' end as INSPFLAG,
case h.MANIFEST when '0' then '否(0)' when '1' then '是(1)' end as MANIFEST,
h.CUSNO,h.CONTRACTNO,h.TOTALNO,h.DIVIDENO,h.GOODSNUM||'/'||h.GOODGW as GOODSNUM,busi.name as BUSITYPE,cus.name as PORTCODE,
trade.name as TRADEWAY,h.REMARK,h.DECLCODEQTY,h.DECLARATIONCODE,h.BUSIUNITNAME,h.ACCEPTTIME,h.MOENDTIME,h.COENDTIME, h.RECOENDTIME,h.REPSTARTTIME,h.REPENDTIME,h.PASSTIME 
from RESIDENT_ORDER h
left join SYS_STATUS sta on sta.code=h.status
left join cusdoc.sys_busitype busi on busi.code=h.BUSITYPE
left join cusdoc.BASE_CUSTOMDISTRICT cus on cus.code=h.PORTCODE
left join cusdoc.BASE_DECLTRADEWAY trade on trade.code=h.TRADEWAY 

where 1=1 " + getQueryCondition()+" and RECEIVERUNITCODE='" +json_user.Value<string>("CUSTOMERCODE")+"' order by h.SUBMITTIME desc";
            DataTable dt = GetData(strSql);
            var json = JsonConvert.SerializeObject(dt, iso);

            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        private DataTable GetData(string strSql)
        {
            int start = 1 + Convert.ToInt32(Request["start"]);
            int limit = Convert.ToInt32(Request["limit"]);
            string sql = "select count(1) from ( " + strSql + " )";
            totalProperty = Convert.ToInt32(DBMgr.GetDataTable(sql).Rows[0][0]);

            return GetQuerySqlPage(strSql, start, limit);
        }
        private DataTable GetQuerySqlPage(string strSql, int pageIndex, int pageSize)
        {
            string sql = @"SELECT * FROM (SELECT ROWNUM AS rowno, t.* FROM ( " + strSql + @" ) t WHERE ROWNUM <= " + pageIndex * pageSize + " ) table_alias  WHERE table_alias.rowno >  " + (pageIndex - 1) * pageSize;
            return DBMgr.GetDataTable(sql);
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
TOTALNO,DIVIDENO,MANIFEST,INSPFLAG,REMARK,RECEIVERUNITCODE,RECEIVERUNITNAME,CREATETIME,DECLCODEQTY,DECLARATIONCODE) 
            values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}',sysdate,'{17}','{18}')";
                strSql = string.Format(strSql, code, jsonOrderdata.Value<string>("CUSNO"), jsonOrderdata.Value<string>("BUSITYPE"), jsonOrderdata.Value<string>("TRADEWAY2")
                    , jsonOrderdata.Value<string>("PORTCODE"), jsonOrderdata.Value<string>("BUSIUNITCODE"), BUSIUNITNAME
                    , jsonOrderdata.Value<string>("GOODSNUM2"), jsonOrderdata.Value<string>("GOODGW2")
                    , jsonOrderdata.Value<string>("CONTRACTNO"), jsonOrderdata.Value<string>("TOTALNO"), jsonOrderdata.Value<string>("DIVIDENO")
                    , MANIFEST, INSPFLAG, jsonOrderdata.Value<string>("REMARK2")
                    , json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), DECLCODEQTY, DECLARATIONCODE);
                listSqls.Add(strSql);

                //  strSql="update RESIDENT_ORDER set ";
                strSql = string.Empty;

                status = "10";//已委托
                if (jsonOrderTimedata.Value<string>("SUBMITTIME") != "null" && jsonOrderTimedata.Value<string>("SUBMITTIME") != "")//
                {
                    strSql += " SUBMITTIME=to_date('" + jsonOrderTimedata.Value<string>("SUBMITTIME") + "','yyyy/mm/dd hh24:mi:ss'),SUBMITUSERID='" + jsonOrderTimedata.Value<string>("SUBMITUSERID") + "',SUBMITUSERNAME='" + jsonOrderTimedata.Value<string>("SUBMITUSERNAME") + "', ";
                }
                else
                {
                    strSql += " SUBMITTIME=sysdate,SUBMITUSERID='" + json_user.Value<string>("ID") + "',SUBMITUSERNAME='" + json_user.Value<string>("REALNAME") + "', ";
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
                return "{success:true,code:'" + code + "'}";
            }
            catch (Exception ex)
            {
                return "{success:false,msg:'保存失败：" + ex.Message + "'}";
            }
        }

        public string loadrecord()
        {
            string ordercode = Request["ordercode"];
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string formOrderData = "{}";
            string formDeclData = "[]";

            string strSql = "select * from RESIDENT_ORDER where code='"+ordercode+"'";
            formOrderData = JsonConvert.SerializeObject(DBMgr.GetDataTable(strSql), iso).TrimStart('[').TrimEnd(']');

            strSql = "select * from RESIDENT_DECLARATION where ordercode='"+ordercode+"' order by NO";
            formDeclData = JsonConvert.SerializeObject(DBMgr.GetDataTable(strSql), iso);

            return "{formOrderData:" + formOrderData + ",formDeclData:" + formDeclData + "}";
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




    }
}
