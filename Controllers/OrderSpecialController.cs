using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    [Authorize]
    [Filters.AuthFilter]
    public class OrderSpecialController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.navigator = "订单中心>>特殊区域订单";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult Create()
        {
            ViewBag.navigator = "订单中心>>特殊区域订单";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
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
        public string GetChk(string check_val)
        {
            return check_val == "on" ? "1" : "0";
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
                if (string.IsNullOrEmpty(json.Value<string>("SUBMITTIME")))
                {
                    json.Remove("SUBMITTIME"); //委托时间  因为该字段需要取ORACLE的时间，而非系统时间 所以需要特殊处理,格式化时并没有加引号
                    json.Add("SUBMITTIME", "null");
                }
                else//有可能提交以后再对部分字段进行修改后保存
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
                            ,TURNPRENO,GOODSNUM,CLEARANCENO,LAWFLAG,ENTRUSTTYPE
                            ,REPWAYID,CUSTOMAREACODE,REPUNITCODE,REPUNITNAME,DECLWAY
                            ,PORTCODE,INSPUNITCODE,INSPUNITNAME,ORDERREQUEST,CREATEUSERID
                            ,CREATEUSERNAME,STATUS,SUBMITUSERID,SUBMITUSERNAME,CUSTOMERCODE
                            ,CUSTOMERNAME,DECLCARNO,TRADEWAYCODES,GOODSGW,GOODSNW
                            ,PACKKIND,CREATETIME,SUBMITTIME,GOODSTYPEID,CONTAINERNO,ORDERWAY
                            ,SPECIALRELATIONSHIP,PRICEIMPACT,PAYPOYALTIES,BUSIKIND,CLEARUNIT
                            ,CLEARUNITNAME,DECLSTATUS,INSPSTATUS,DOCSERVICECODE
                            )
                      VALUES ( LIST_ORDER_id.Nextval,
                            '{0}','{1}','{2}','{3}','{4}','{5}'
                            ,'{6}','{7}','{8}','{9}','{10}'
                            ,'{11}','{12}','{13}', '{14}','{15}'
                            ,'{16}','{17}','{18}','{19}','{20}'
                            ,'{21}','{22}','{23}','{24}','{25}'
                            ,'{26}','{27}','{28}','{29}','{30}'
                            ,'{31}',sysdate,{32},'{33}','{34}','{35}'
                            ,'{36}','{37}','{38}','{39}','{40}'
                            ,'{41}','{42}','{43}','{44}'
                            )";
                sql = string.Format(sql
                        , json.Value<string>("BUSITYPE"), ordercode, json.Value<string>("CUSNO"), json.Value<string>("BUSIUNITCODE"), json.Value<string>("BUSIUNITNAME"), json.Value<string>("CONTRACTNO")
                        , json.Value<string>("TURNPRENO"), json.Value<string>("GOODSNUM"), json.Value<string>("CLEARANCENO"), GetChk(json.Value<string>("LAWFLAG")), json.Value<string>("ENTRUSTTYPE")
                        , json.Value<string>("REPWAYID"), json.Value<string>("CUSTOMAREACODE"), GetCode(json.Value<string>("REPUNITCODE")), GetName(json.Value<string>("REPUNITCODE")), json.Value<string>("DECLWAY")
                        , json.Value<string>("PORTCODE"), GetCode(json.Value<string>("INSPUNITCODE")), GetName(json.Value<string>("INSPUNITCODE")), json.Value<string>("ORDERREQUEST"), json_user.Value<string>("ID")
                        , json_user.Value<string>("REALNAME"), json.Value<string>("STATUS"), json.Value<string>("SUBMITUSERID"), json.Value<string>("SUBMITUSERNAME"), json_user.Value<string>("CUSTOMERCODE")
                        , json_user.Value<string>("CUSTOMERNAME"), json.Value<string>("DECLCARNO"), json.Value<string>("TRADEWAYCODES"), json.Value<string>("GOODSGW"), json.Value<string>("GOODSNW")
                        , json.Value<string>("PACKKIND"), json.Value<string>("SUBMITTIME"), json.Value<string>("GOODSTYPEID"), json.Value<string>("CONTAINERNO")
                        , "1", GetChk(json.Value<string>("SPECIALRELATIONSHIP")), GetChk(json.Value<string>("PRICEIMPACT")), GetChk(json.Value<string>("PAYPOYALTIES")), "002"
                        , json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), json.Value<string>("DECLSTATUS"), json.Value<string>("INSPSTATUS"), json.Value<string>("DOCSERVICECODE")
                       );
            }
            else//修改
            {
                ordercode = json.Value<string>("CODE");
                /*sql = @"UPDATE LIST_ORDER 
                        SET BUSITYPE='{1}',BUSIKIND='{2}',CUSNO='{3}',BUSIUNITCODE='{4}',BUSIUNITNAME='{5}'
                            ,CONTRACTNO='{6}',TURNPRENO='{7}',GOODSNUM='{8}',CLEARANCENO='{9}',LAWFLAG='{10}'
                            ,ENTRUSTTYPE='{11}',REPWAYID='{12}',CUSTOMAREACODE='{13}',REPUNITCODE='{14}',REPUNITNAME='{15}'
                            ,DECLWAY='{16}',PORTCODE='{17}',INSPUNITCODE='{18}',INSPUNITNAME='{19}',ORDERREQUEST='{20}'
                            ,STATUS='{21}',SUBMITUSERID='{22}',SUBMITUSERNAME='{23}',CUSTOMERCODE='{24}',CUSTOMERNAME='{25}'
                            ,DECLCARNO='{26}',TRADEWAYCODES='{27}',SUBMITTIME={28},GOODSGW='{29}',GOODSNW='{30}'
                            ,PACKKIND='{31}',GOODSTYPEID='{32}',CONTAINERNO='{33}',ORDERWAY='{34}'
                            ,SPECIALRELATIONSHIP='{35}',PRICEIMPACT='{36}',PAYPOYALTIES='{37}',CLEARUNIT='{38}',CLEARUNITNAME='{39}',DOCSERVICECODE='{40}'  
                            ";

                if (IsSubmitAfterSave == false)//提交之后保存，就不更新报关报检状态；
                {
                    sql += @",DECLSTATUS='{41}',INSPSTATUS='{42}'";
                }
                sql += @" WHERE CODE = '{0}'";
                */
                string allcol = @"CODE 
                                ,BUSITYPE,BUSIKIND,CUSNO,BUSIUNITCODE,BUSIUNITNAME 
                                ,CONTRACTNO,TURNPRENO,GOODSNUM,CLEARANCENO,LAWFLAG 
                                ,ENTRUSTTYPE,REPWAYID,CUSTOMAREACODE,REPUNITCODE,REPUNITNAME 
                                ,DECLWAY,PORTCODE,INSPUNITCODE,INSPUNITNAME,ORDERREQUEST 
                                ,STATUS,SUBMITUSERID,SUBMITUSERNAME,CUSTOMERCODE,CUSTOMERNAME 
                                ,DECLCARNO,TRADEWAYCODES,SUBMITTIME,GOODSGW,GOODSNW 
                                ,PACKKIND,GOODSTYPEID,CONTAINERNO,ORDERWAY 
                                ,SPECIALRELATIONSHIP,PRICEIMPACT,PAYPOYALTIES,CLEARUNIT,CLEARUNITNAME,DOCSERVICECODE 
                                ,DECLSTATUS,INSPSTATUS
                                ";
                sql = Extension.getUpdateSql(allcol, ordercode, IsSubmitAfterSave);
                if (sql != "")
                {
                    sql = string.Format(sql
                            , ordercode, json.Value<string>("BUSITYPE"), "002", json.Value<string>("CUSNO"), json.Value<string>("BUSIUNITCODE"), json.Value<string>("BUSIUNITNAME")
                            , json.Value<string>("CONTRACTNO"), json.Value<string>("TURNPRENO"), json.Value<string>("GOODSNUM"), json.Value<string>("CLEARANCENO"), GetChk(json.Value<string>("LAWFLAG"))
                            , json.Value<string>("ENTRUSTTYPE"), json.Value<string>("REPWAYID"), json.Value<string>("CUSTOMAREACODE"), GetCode(json.Value<string>("REPUNITCODE")), GetName(json.Value<string>("REPUNITCODE"))
                            , json.Value<string>("DECLWAY"), json.Value<string>("PORTCODE"), GetCode(json.Value<string>("INSPUNITCODE")), GetName(json.Value<string>("INSPUNITCODE")), json.Value<string>("ORDERREQUEST")
                            , json.Value<string>("STATUS"), json.Value<string>("SUBMITUSERID"), json.Value<string>("SUBMITUSERNAME"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME")
                            , json.Value<string>("DECLCARNO"), json.Value<string>("TRADEWAYCODES"), json.Value<string>("SUBMITTIME"), json.Value<string>("GOODSGW"), json.Value<string>("GOODSNW")
                            , json.Value<string>("PACKKIND"), json.Value<string>("GOODSTYPEID"), json.Value<string>("CONTAINERNO"), "1"
                            , GetChk(json.Value<string>("SPECIALRELATIONSHIP")), GetChk(json.Value<string>("PRICEIMPACT")), GetChk(json.Value<string>("PAYPOYALTIES")), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME")
                            , json.Value<string>("DOCSERVICECODE"), json.Value<string>("DECLSTATUS"), json.Value<string>("INSPSTATUS")
                         );
                }
            }
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
                    Extension.Insert_FieldUpdate_History(ordercode, json, json_user, json.Value<string>("BUSITYPE"));
                }
                return "{success:true,ordercode:'" + ordercode + "'}";
            }
            else
            {
                return "{success:false}";
            }
        }

    }
}
