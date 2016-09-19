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
    public class OrderLandOutController : Controller
    {
        //
        // GET: /OrderLandOut/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
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
            string ordercode = string.Empty;
            if (Request["action"] + "" == "submit")
            {
                json.Remove("STATUS"); json.Remove("SUBMITTIME"); json.Remove("SUBMITUSERNAME"); json.Remove("SUBMITUSERID");
                json.Add("STATUS", 15);
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
                }
            }
            if (string.IsNullOrEmpty(json.Value<string>("CODE")))//新增
            {
                ordercode = Extension.getOrderCode();
                sql = @"INSERT INTO LIST_ORDER (ID
                        ,BUSITYPE,CODE,CUSNO,BUSIUNITCODE,BUSIUNITNAME,CONTRACTNO
                        ,TOTALNO,DIVIDENO,TURNPRENO,GOODSNUM,WOODPACKINGID
                        ,CLEARANCENO,LAWFLAG,ENTRUSTTYPE,REPWAYID,CUSTOMAREACODE
                        ,REPUNITCODE,REPUNITNAME,DECLWAY,PORTCODE,INSPUNITCODE
                        ,INSPUNITNAME,ENTRUSTREQUEST,CREATEUSERID,CREATEUSERNAME,STATUS
                        ,SUBMITUSERID,SUBMITUSERNAME,CUSTOMERCODE,CUSTOMERNAME,DECLCARNO
                        ,TRADEWAYCODES,GOODSGW,GOODSNW,PACKKIND,CREATETIME,SUBMITTIME
                        ,GOODSTYPEID,ARRIVEDNO,CONTAINERNO,TRADEWAYCODES1,BUSIKIND
                        ,ORDERWAY,SPECIALRELATIONSHIP,PRICEIMPACT,PAYPOYALTIES,FILGHTNO
                        ,CLEARUNIT,CLEARUNITNAME 
                    ) VALUES (LIST_ORDER_id.Nextval
                        ,'{0}','{1}','{2}','{3}', '{4}','{5}'
                        ,'{6}','{7}','{8}','{9}','{10}'
                        ,'{11}','{12}','{13}','{14}','{15}'
                        ,'{16}','{17}','{18}','{19}','{20}'
                        ,'{21}','{22}','{23}','{24}','{25}'
                        ,'{26}','{27}','{28}','{29}','{30}'
                        ,'{31}','{32}','{33}','{34}', sysdate,{35}
                        ,'{36}','{37}','{38}','{39}','{40}'
                        ,'{41}','{42}','{43}','{44}','{45}'
                        ,'{46}','{47}'
                    )";
                sql = string.Format(sql
                        , "30", ordercode, json.Value<string>("CUSNO"), json.Value<string>("BUSIUNITCODE"), json.Value<string>("BUSIUNITNAME"), json.Value<string>("CONTRACTNO")
                        , json.Value<string>("TOTALNO"), json.Value<string>("DIVIDENO"), json.Value<string>("TURNPRENO"), json.Value<string>("GOODSNUM"), json.Value<string>("WOODPACKINGID")
                        , json.Value<string>("CLEARANCENO"), GetChk(json.Value<string>("LAWFLAG")), json.Value<string>("ENTRUSTTYPE"), json.Value<string>("REPWAYID"), json.Value<string>("CUSTOMAREACODE")
                        , GetCode(json.Value<string>("REPUNITCODE")), GetName(json.Value<string>("REPUNITCODE")), json.Value<string>("DECLWAY"), json.Value<string>("PORTCODE"), GetCode(json.Value<string>("INSPUNITCODE"))
                        , GetName(json.Value<string>("INSPUNITCODE")), json.Value<string>("ENTRUSTREQUEST"), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"), json.Value<string>("STATUS")
                        , json.Value<string>("SUBMITUSERID"), json.Value<string>("SUBMITUSERNAME"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), json.Value<string>("DECLCARNO")
                        , json.Value<string>("TRADEWAYCODES"), json.Value<string>("GOODSGW"), json.Value<string>("GOODSNW"), json.Value<string>("PACKKIND"), json.Value<string>("SUBMITTIME")
                        , json.Value<string>("GOODSTYPEID"), json.Value<string>("ARRIVEDNO"), json.Value<string>("CONTAINERNO"), json.Value<string>("TRADEWAYCODES1"), "001"
                        , "1", GetChk(json.Value<string>("SPECIALRELATIONSHIP")), GetChk(json.Value<string>("PRICEIMPACT")), GetChk(json.Value<string>("PAYPOYALTIES")), json.Value<string>("FILGHTNO")
                        , json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME")
                    );
            }
            else//修改
            {
                ordercode = json.Value<string>("CODE");
                sql = @"UPDATE LIST_ORDER SET BUSITYPE='{1}',CUSNO='{2}',BUSIUNITCODE='{3}',BUSIUNITNAME='{4}',CONTRACTNO='{5}'
                            ,TOTALNO='{6}', DIVIDENO='{7}',TURNPRENO='{8}',GOODSNUM='{9}',WOODPACKINGID='{10}'
                            ,CLEARANCENO='{11}',LAWFLAG='{12}',ENTRUSTTYPE='{13}',REPWAYID='{14}',CUSTOMAREACODE='{15}'
                            ,REPUNITCODE='{16}',REPUNITNAME='{17}',DECLWAY='{18}',PORTCODE='{19}',INSPUNITCODE='{20}'
                            ,INSPUNITNAME='{21}',ENTRUSTREQUEST='{22}',STATUS='{23}',SUBMITUSERID='{24}',SUBMITUSERNAME='{25}'
                            ,CUSTOMERCODE='{26}',CUSTOMERNAME='{27}',DECLCARNO='{28}',TRADEWAYCODES='{29}',SUBMITTIME={30}
                            ,GOODSGW='{31}',GOODSNW='{32}',PACKKIND='{33}',GOODSTYPEID = '{34}',ARRIVEDNO = '{35}'
                            ,CONTAINERNO = '{36}',TRADEWAYCODES1 = '{37}',BUSIKIND='{38}',ORDERWAY='{39}',SPECIALRELATIONSHIP='{40}'
                            ,PRICEIMPACT='{41}',PAYPOYALTIES='{42}',FILGHTNO='{43}',CLEARUNIT='{44}',CLEARUNITNAME='{45}'
                             WHERE CODE = '{0}'";
                sql = string.Format(sql, ordercode
                        , "30", json.Value<string>("CUSNO"), json.Value<string>("BUSIUNITCODE"), json.Value<string>("BUSIUNITNAME"), json.Value<string>("CONTRACTNO")
                        , json.Value<string>("TOTALNO"), json.Value<string>("DIVIDENO"), json.Value<string>("TURNPRENO"), json.Value<string>("GOODSNUM"), json.Value<string>("WOODPACKINGID")
                        , json.Value<string>("CLEARANCENO"), GetChk(json.Value<string>("LAWFLAG")), json.Value<string>("ENTRUSTTYPE"), json.Value<string>("REPWAYID"), json.Value<string>("CUSTOMAREACODE")
                        , GetCode(json.Value<string>("REPUNITCODE")), GetName(json.Value<string>("REPUNITCODE")), json.Value<string>("DECLWAY"), json.Value<string>("PORTCODE"), GetCode(json.Value<string>("INSPUNITCODE"))
                        , GetName(json.Value<string>("REPUNITCODE")), json.Value<string>("ENTRUSTREQUEST"), json.Value<string>("STATUS"), json.Value<string>("SUBMITUSERID"), json.Value<string>("SUBMITUSERNAME")
                        , json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), json.Value<string>("DECLCARNO"), json.Value<string>("TRADEWAYCODES"), json.Value<string>("SUBMITTIME")
                        , json.Value<string>("GOODSGW"), json.Value<string>("GOODSNW"), json.Value<string>("PACKKIND"), json.Value<string>("GOODSTYPEID"), json.Value<string>("ARRIVEDNO")
                        , json.Value<string>("CONTAINERNO"), json.Value<string>("TRADEWAYCODES1"), "001", "1", GetChk(json.Value<string>("SPECIALRELATIONSHIP"))
                        , GetChk(json.Value<string>("PRICEIMPACT")), GetChk(json.Value<string>("PAYPOYALTIES")), json.Value<string>("FILGHTNO"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME")
                        );
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
                if (json.Value<Int32>("STATUS") > 15)
                {
                    Extension.Insert_FieldUpdate_History(ordercode, json, json_user, "30");
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
