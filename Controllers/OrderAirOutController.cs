using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class OrderAirOutController : Controller
    {
        //
        // GET: /OrderAirOut/

        public ActionResult Index()
        {
            ViewBag.navigator = "订单中心>>空运出口";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.navigator = "订单中心>>空运出口";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public string GetChk(string check_val)
        {
            return check_val == "on" ? "1" : "0";
        }

        public string GetName(string combin)
        {
            if (string.IsNullOrEmpty(combin))
            {
                return "";
            }
            else
            {
                int index = combin.LastIndexOf("(");
                return combin.Substring(0, index);
            }
        }
        public string GetCode(string combin)
        {
            if (string.IsNullOrEmpty(combin))
            {
                return "";
            }
            else
            {
                int start = combin.LastIndexOf("(");
                int end = combin.LastIndexOf(")");
                return combin.Substring(start + 1, end - start - 1);
            }
        }

        public string Save()
        {
            string filedata = Request["filedata"];
            string action = Request["action"];
            JObject json = (JObject)JsonConvert.DeserializeObject(Request["formdata"]);
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = "";
            string ordercode = string.Empty; bool IsSubmitAfterSave = false;
            if (Request["action"] + "" == "submit")
            {
                json.Remove("STATUS"); json.Remove("SUBMITTIME"); json.Remove("SUBMITUSERNAME"); json.Remove("SUBMITUSERID"); 
                json.Add("STATUS", 10);
                json.Add("SUBMITTIME", "sysdate");
                json.Add("SUBMITUSERNAME", json_user.Value<string>("REALNAME"));
                json.Add("SUBMITUSERID", json_user.Value<string>("ID"));
            }
            else
            {
                if (string.IsNullOrEmpty(json.Value<string>("SUBMITTIME")))//有可能提交以后再对部分字段进行修改后保存
                {
                    json.Remove("SUBMITTIME"); //委托时间  因为该字段需要取ORACLE的时间，而非系统时间 所以需要特殊处理,格式化时并没有加引号
                    json.Add("SUBMITTIME", "null");
                }
                else
                {
                    string submittime = json.Value<string>("SUBMITTIME");
                    json.Remove("SUBMITTIME");//委托时间  因为该字段需要取ORACLE的时间，而非系统时间 所以需要特殊处理
                    json.Add("SUBMITTIME", "to_date('" + submittime + "','yyyy-MM-dd HH24:mi:ss')");
                    IsSubmitAfterSave = true;
                }
            }

            if (json.Value<string>("ENTRUSTTYPE") == "01")
            {
                json.Add("DECLSTATUS", json.Value<string>("STATUS")); json.Add("INSPSTATUS", null);
            }
            if (json.Value<string>("ENTRUSTTYPE") == "02")
            {
                json.Add("DECLSTATUS", null); json.Add("INSPSTATUS", json.Value<string>("STATUS"));
            }
            if (json.Value<string>("ENTRUSTTYPE") == "03")
            {
                json.Add("DECLSTATUS", json.Value<string>("STATUS")); json.Add("INSPSTATUS", json.Value<string>("STATUS"));
            }

            if (string.IsNullOrEmpty(json.Value<string>("CODE")))//新增
            {
                ordercode = Extension.getOrderCode();
                sql = @"INSERT INTO LIST_ORDER (ID
                            ,BUSITYPE,CODE,CUSNO,BUSIUNITCODE,BUSIUNITNAME,CONTRACTNO
                            ,TOTALNO,DIVIDENO,TURNPRENO,GOODSNUM,ARRIVEDNO
                            ,CLEARANCENO,LAWFLAG,ENTRUSTTYPE,REPWAYID,CUSTOMAREACODE
                            ,REPUNITCODE,REPUNITNAME,DECLWAY,PORTCODE,INSPUNITCODE
                            ,INSPUNITNAME,ORDERREQUEST,CREATEUSERID,CREATEUSERNAME,STATUS
                            ,SUBMITUSERID,SUBMITUSERNAME,CUSTOMERCODE,CUSTOMERNAME,DECLCARNO
                            ,TRADEWAYCODES,GOODSGW,GOODSNW,PACKKIND,BUSIKIND
                            ,ORDERWAY,CLEARUNIT,CLEARUNITNAME,CREATETIME,SUBMITTIME,SPECIALRELATIONSHIP
                            ,PRICEIMPACT,PAYPOYALTIES,WEIGHTCHECK,ISWEIGHTCHECK,DECLSTATUS
                            ,INSPSTATUS,DOCSERVICECODE,CLEARREMARK,RECEIVERUNITCODE,RECEIVERUNITNAME   
                            ) 
                      VALUES (LIST_ORDER_id.Nextval
                            , '{0}','{1}','{2}','{3}','{4}','{5}'
                            ,'{6}','{7}','{8}','{9}','{10}'
                            ,'{11}','{12}','{13}','{14}','{15}'
                            , '{16}','{17}','{18}','{19}','{20}'
                            ,'{21}','{22}','{23}','{24}','{25}'
                            ,'{26}','{27}','{28}','{29}','{30}'
                            ,'{31}','{32}','{33}','{34}','{35}'
                            ,'{36}','{37}','{38}', sysdate,{39},'{40}'
                            ,'{41}','{42}','{43}','{44}','{45}'
                            ,'{46}','{47}','{48}','{49}','{50}')";
                sql = string.Format(sql
                            , "10", ordercode, json.Value<string>("CUSNO"), json.Value<string>("BUSIUNITCODE"), json.Value<string>("BUSIUNITNAME"), json.Value<string>("CONTRACTNO")
                            , json.Value<string>("TOTALNO"), json.Value<string>("DIVIDENO"), json.Value<string>("TURNPRENO"), json.Value<string>("GOODSNUM"), json.Value<string>("ARRIVEDNO")
                            , json.Value<string>("CLEARANCENO"), GetChk(json.Value<string>("LAWFLAG")), json.Value<string>("ENTRUSTTYPE"), json.Value<string>("REPWAYID"), json.Value<string>("CUSTOMAREACODE")
                            , GetCode(json.Value<string>("REPUNITCODE")), GetName(json.Value<string>("REPUNITCODE")), json.Value<string>("DECLWAY"), json.Value<string>("PORTCODE"), GetCode(json.Value<string>("INSPUNITCODE"))
                            , GetName(json.Value<string>("INSPUNITCODE")), json.Value<string>("ORDERREQUEST"), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"), json.Value<string>("STATUS")
                            , json.Value<string>("SUBMITUSERID"), json.Value<string>("SUBMITUSERNAME"), json.Value<string>("CUSTOMERCODE"), json.Value<string>("CUSTOMERNAME"), json.Value<string>("DECLCARNO")
                            , json.Value<string>("TRADEWAYCODES"), json.Value<string>("GOODSGW"), json.Value<string>("GOODSNW"), json.Value<string>("PACKKIND"), "001"
                            , "1", json.Value<string>("CLEARUNIT"), json.Value<string>("CLEARUNITNAME"), json.Value<string>("SUBMITTIME"), GetChk(json.Value<string>("SPECIALRELATIONSHIP"))
                            , GetChk(json.Value<string>("PRICEIMPACT")), GetChk(json.Value<string>("PAYPOYALTIES")), GetChk(json.Value<string>("WEIGHTCHECK")), GetChk(json.Value<string>("ISWEIGHTCHECK")), json.Value<string>("DECLSTATUS")
                            , json.Value<string>("INSPSTATUS"), json.Value<string>("DOCSERVICECODE"), json.Value<string>("CLEARREMARK"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME")
                        );
            }
            else//修改
            {
                ordercode = json.Value<string>("CODE");
                /*sql = @"UPDATE LIST_ORDER 
                        SET BUSITYPE='{1}',CUSNO='{2}',BUSIUNITCODE='{3}',BUSIUNITNAME='{4}',CONTRACTNO='{5}'
                            ,TOTALNO='{6}',DIVIDENO='{7}',TURNPRENO='{8}',GOODSNUM='{9}',ARRIVEDNO='{10}'
                            ,CLEARANCENO='{11}',LAWFLAG='{12}',ENTRUSTTYPE='{13}',REPWAYID='{14}',CUSTOMAREACODE='{15}'
                            ,REPUNITCODE='{16}',REPUNITNAME='{17}',DECLWAY='{18}',PORTCODE='{19}',INSPUNITCODE='{20}'
                            ,INSPUNITNAME='{21}' ,ORDERREQUEST='{22}',STATUS='{23}',SUBMITUSERID='{24}',SUBMITUSERNAME='{25}'
                            ,CUSTOMERCODE='{26}',CUSTOMERNAME='{27}',DECLCARNO='{28}',TRADEWAYCODES='{29}',SUBMITTIME={30}
                            ,GOODSGW='{31}',GOODSNW='{32}',PACKKIND='{33}',BUSIKIND='{34}',ORDERWAY='{35}'
                            ,CLEARUNIT='{36}',CLEARUNITNAME='{37}',SPECIALRELATIONSHIP='{38}',PRICEIMPACT='{39}',PAYPOYALTIES='{40}'
                            ,WEIGHTCHECK='{41}',ISWEIGHTCHECK='{42}',DOCSERVICECODE='{43}'
                        ";

                if (IsSubmitAfterSave == false)//提交之后保存，就不更新报关报检状态；
                {
                    sql += @",DECLSTATUS='{44}',INSPSTATUS='{45}'";
                }
                sql += @" WHERE CODE = '{0}'";
                */
                string allcol = @"CODE
                            ,BUSITYPE,CUSNO,BUSIUNITCODE,BUSIUNITNAME,CONTRACTNO
                            ,TOTALNO,DIVIDENO,TURNPRENO,GOODSNUM,ARRIVEDNO
                            ,CLEARANCENO,LAWFLAG,ENTRUSTTYPE,REPWAYID,CUSTOMAREACODE
                            ,REPUNITCODE,REPUNITNAME,DECLWAY,PORTCODE,INSPUNITCODE
                            ,INSPUNITNAME,ORDERREQUEST,STATUS,SUBMITUSERID,SUBMITUSERNAME
                            ,CUSTOMERCODE,CUSTOMERNAME,DECLCARNO,TRADEWAYCODES,SUBMITTIME
                            ,GOODSGW,GOODSNW,PACKKIND,BUSIKIND,ORDERWAY
                            ,CLEARUNIT,CLEARUNITNAME,SPECIALRELATIONSHIP,PRICEIMPACT,PAYPOYALTIES
                            ,WEIGHTCHECK,ISWEIGHTCHECK,DOCSERVICECODE,DECLSTATUS,INSPSTATUS
                            ,CLEARREMARK,RECEIVERUNITCODE,RECEIVERUNITNAME   
                            ";
                sql = Extension.getUpdateSql(allcol, ordercode, IsSubmitAfterSave);
                if (sql != "")
                {
                    sql = string.Format(sql, ordercode
                               , "10", json.Value<string>("CUSNO"), json.Value<string>("BUSIUNITCODE"), json.Value<string>("BUSIUNITNAME"), json.Value<string>("CONTRACTNO")
                               , json.Value<string>("TOTALNO"), json.Value<string>("DIVIDENO"), json.Value<string>("TURNPRENO"), json.Value<string>("GOODSNUM"), json.Value<string>("ARRIVEDNO")
                               , json.Value<string>("CLEARANCENO"), GetChk(json.Value<string>("LAWFLAG")), json.Value<string>("ENTRUSTTYPE"), json.Value<string>("REPWAYID"), json.Value<string>("CUSTOMAREACODE")
                               , GetCode(json.Value<string>("REPUNITCODE")), GetName(json.Value<string>("REPUNITCODE")), json.Value<string>("DECLWAY"), json.Value<string>("PORTCODE"), GetCode(json.Value<string>("INSPUNITCODE"))
                               , GetName(json.Value<string>("INSPUNITCODE")), json.Value<string>("ORDERREQUEST"), json.Value<string>("STATUS"), json.Value<string>("SUBMITUSERID"), json.Value<string>("SUBMITUSERNAME")
                               , json.Value<string>("CUSTOMERCODE"), json.Value<string>("CUSTOMERNAME"), json.Value<string>("DECLCARNO"), json.Value<string>("TRADEWAYCODES"), json.Value<string>("SUBMITTIME")
                               , json.Value<string>("GOODSGW"), json.Value<string>("GOODSNW"), json.Value<string>("PACKKIND"), "001", "1"
                               , json.Value<string>("CLEARUNIT"), json.Value<string>("CLEARUNITNAME"), GetChk(json.Value<string>("SPECIALRELATIONSHIP")), GetChk(json.Value<string>("PRICEIMPACT")), GetChk(json.Value<string>("PAYPOYALTIES"))
                               , GetChk(json.Value<string>("WEIGHTCHECK")), GetChk(json.Value<string>("ISWEIGHTCHECK")), json.Value<string>("DOCSERVICECODE"), json.Value<string>("DECLSTATUS"), json.Value<string>("INSPSTATUS")
                               , json.Value<string>("CLEARREMARK"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME")
                            );
                }              
            }
            if (sql != "")
            {
                int result = DBMgr.ExecuteNonQuery(sql);
                if (result == 1)
                {
                    //集装箱及报关车号列表更新
                    Extension.predeclcontainer_update(ordercode, json.Value<string>("CONTAINERTRUCK"));

                    //更新随附文件 
                    Extension.Update_Attachment(ordercode, filedata, json.Value<string>("ORIGINALFILEIDS"), json_user);

                    //插入订单状态变更日志
                    Extension.add_list_time(json.Value<Int32>("STATUS"), ordercode, json_user);
                    if (json.Value<Int32>("STATUS") > 10)
                    {
                        Extension.Insert_FieldUpdate_History(ordercode, json, json_user, "10");
                    }
                    return "{success:true,ordercode:'" + ordercode + "'}";
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


    }
}
