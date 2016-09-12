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
    public class OrderAirOutController : Controller
    {
        //
        // GET: /OrderAirOut/

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
                json.Remove("STATUS"); json.Remove("SUBMITTIME"); json.Remove("SUBMITUSERNAME"); json.Remove("SUBMITUSERID"); json.Remove("SUBMITUSERPHONE");
                json.Add("STATUS", 15);
                json.Add("SUBMITTIME", "sysdate");
                json.Add("SUBMITUSERNAME", json_user.Value<string>("REALNAME"));
                json.Add("SUBMITUSERID", json_user.Value<string>("ID"));
                json.Add("SUBMITUSERPHONE", json_user.Value<string>("TELEPHONE") + "|" + json_user.Value<string>("MOBILEPHONE"));
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
                sql = @"INSERT INTO LIST_ORDER (
                      ID,BUSITYPE,CODE,CUSNO,BUSIUNITCODE,BUSIUNITNAME,CONTRACTNO,TOTALNO,DIVIDENO,TURNPRENO,GOODSNUM ,
                      GOODSWEIGHT,ARRIVEDNO,CLEARANCENO,LAWCONDITION,ENTRUSTTYPEID,REPWAYID,CUSTOMDISTRICTCODE,
                      CUSTOMDISTRICTNAME,REPUNITCODE,REPUNITNAME,DECLWAY,PORTCODE,PORTNAME,INSPUNITCODE,INSPUNITNAME,
                      ENTRUSTREQUEST,CREATEUSERID,CREATEUSERNAME,STATUS,SUBMITUSERID,SUBMITUSERNAME,SUBMITUSERPHONE,
                      CSPHONE,CUSTOMERCODE,CUSTOMERNAME,DECLCARNO,TRADEWAYCODES,TRADEWAYCODES1,GOODSGW,GOODSNW,PACKKIND,
                      BUSIKIND,ORDERWAY,CLEARUNIT,CLEARUNITNAME,CREATETIME,SUBMITTIME,BUSISHORTCODE,SPECIALRELATIONSHIP,
                      PRICEIMPACT,PAYPOYALTIES,BUSISHORTNAME,SCENEDECLAREID,SCENEINSPECTID,
                      SELFCHECK,WEIGHTCHECK,ISWEIGHTCHECK,CHECKEDGOODSNUM,CHECKEDWEIGHT) 
                      VALUES (LIST_ORDER_id.Nextval, '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}', 
                      '{12}','{13}','{14}','{15}', '{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}',
                      '{27}','{28}',  '{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}', 
                      '{38}','{39}','{40}','{41}','{42}','{43}','{44}', sysdate,{45},'{46}','{47}','{48}','{49}','{50}',
                      '{51}','{52}','{53}','{54}','{55}','{56}','{57}')";
                sql = string.Format(sql, "10", ordercode, json.Value<string>("CUSNO"), json.Value<string>("BUSIUNITCODE"), json.Value<string>("BUSIUNITNAME"),
                      json.Value<string>("CONTRACTNO"), json.Value<string>("TOTALNO"), json.Value<string>("DIVIDENO"), json.Value<string>("TURNPRENO"),
                      json.Value<string>("GOODSNUM"), json.Value<string>("GOODSWEIGHT"), json.Value<string>("ARRIVEDNO"), json.Value<string>("CLEARANCENO"),
                      GetChk(json.Value<string>("LAWCONDITION")), json.Value<string>("ENTRUSTTYPEID"), json.Value<string>("REPWAYID"), json.Value<string>("CUSTOMDISTRICTCODE"),
                      json.Value<string>("CUSTOMDISTRICTNAME"), GetCode(json.Value<string>("REPUNITCODE")), GetName(json.Value<string>("REPUNITCODE")),
                      json.Value<string>("DECLWAY"), json.Value<string>("PORTCODE"), json.Value<string>("PORTNAME"), GetCode(json.Value<string>("INSPUNITCODE")),
                      GetName(json.Value<string>("INSPUNITCODE")), json.Value<string>("ENTRUSTREQUEST"), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"),
                      json.Value<string>("STATUS"), json.Value<string>("SUBMITUSERID"), json.Value<string>("SUBMITUSERNAME"), json.Value<string>("SUBMITUSERPHONE"),
                      json.Value<string>("CSPHONE"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), json.Value<string>("DECLCARNO"),
                      json.Value<string>("TRADEWAYCODES"), json.Value<string>("TRADEWAYCODES1"), json.Value<string>("GOODSGW"), json.Value<string>("GOODSNW"),
                      json.Value<string>("PACKKIND"), "001", "1", json.Value<string>("CLEARUNIT"), json.Value<string>("CLEARUNITNAME"), json.Value<string>("SUBMITTIME"),
                      json.Value<string>("BUSISHORTCODE"), GetChk(json.Value<string>("SPECIALRELATIONSHIP")), GetChk(json.Value<string>("PRICEIMPACT")),
                      GetChk(json.Value<string>("PAYPOYALTIES")), json.Value<string>("BUSISHORTNAME"), json_user.Value<string>("SCENEDECLAREID"),
                      json_user.Value<string>("SCENEINSPECTID"), GetChk(json.Value<string>("SELFCHECK")), GetChk(json.Value<string>("WEIGHTCHECK")),
                      GetChk(json.Value<string>("ISWEIGHTCHECK")), json.Value<string>("CHECKEDGOODSNUM"), json.Value<string>("CHECKEDWEIGHT"));
            }
            else//修改
            {
                ordercode = json.Value<string>("CODE");
                sql = @"UPDATE LIST_ORDER SET BUSITYPE='{1}',CUSNO='{2}',BUSIUNITCODE='{3}',BUSIUNITNAME='{4}',CONTRACTNO='{5}',
                      TOTALNO='{6}',DIVIDENO='{7}',TURNPRENO='{8}',GOODSNUM='{9}' ,GOODSWEIGHT='{10}',ARRIVEDNO='{11}',CLEARANCENO='{12}',
                      LAWCONDITION='{13}',ENTRUSTTYPEID='{14}',REPWAYID='{15}',CUSTOMDISTRICTCODE='{16}',CUSTOMDISTRICTNAME='{17}',REPUNITCODE='{18}',
                      REPUNITNAME='{19}',DECLWAY='{20}',PORTCODE='{21}',PORTNAME='{22}',INSPUNITCODE='{23}',INSPUNITNAME='{24}' ,ENTRUSTREQUEST='{25}',
                      CSPHONE='{26}',SUBMITUSERPHONE='{27}',STATUS='{28}',SUBMITUSERID='{29}',SUBMITUSERNAME='{30}',CUSTOMERCODE='{31}',
                      CUSTOMERNAME='{32}',DECLCARNO='{33}',TRADEWAYCODES='{34}',TRADEWAYCODES1='{35}',SUBMITTIME={36},GOODSGW='{37}',
                      GOODSNW='{38}',PACKKIND='{39}',BUSIKIND='{40}',ORDERWAY='{41}' ,CLEARUNIT='{42}',CLEARUNITNAME='{43}',
                      BUSISHORTCODE='{44}',SPECIALRELATIONSHIP='{45}',PRICEIMPACT='{46}',PAYPOYALTIES='{47}',BUSISHORTNAME='{48}' ,
                      SELFCHECK='{49}',WEIGHTCHECK='{50}',ISWEIGHTCHECK='{51}',CHECKEDGOODSNUM='{52}',CHECKEDWEIGHT='{53}' WHERE CODE = '{0}'";
                sql = string.Format(sql, ordercode, "10", json.Value<string>("CUSNO"), json.Value<string>("BUSIUNITCODE"), json.Value<string>("BUSIUNITNAME"),
                      json.Value<string>("CONTRACTNO"), json.Value<string>("TOTALNO"), json.Value<string>("DIVIDENO"), json.Value<string>("TURNPRENO"),
                      json.Value<string>("GOODSNUM"), json.Value<string>("GOODSWEIGHT"), json.Value<string>("ARRIVEDNO"), json.Value<string>("CLEARANCENO"),
                      GetChk(json.Value<string>("LAWCONDITION")), json.Value<string>("ENTRUSTTYPEID"), json.Value<string>("REPWAYID"),
                      json.Value<string>("CUSTOMDISTRICTCODE"), json.Value<string>("CUSTOMDISTRICTNAME"), GetCode(json.Value<string>("REPUNITCODE")),
                      GetName(json.Value<string>("REPUNITCODE")), json.Value<string>("DECLWAY"), json.Value<string>("PORTCODE"), json.Value<string>("PORTNAME"),
                      GetCode(json.Value<string>("INSPUNITCODE")), GetName(json.Value<string>("INSPUNITCODE")), json.Value<string>("ENTRUSTREQUEST"),
                      json.Value<string>("CSPHONE"), json.Value<string>("SUBMITUSERPHONE"), json.Value<string>("STATUS"), json.Value<string>("SUBMITUSERID"),
                      json.Value<string>("SUBMITUSERNAME"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"),
                      json.Value<string>("DECLCARNO"), json.Value<string>("TRADEWAYCODES"), json.Value<string>("TRADEWAYCODES1"), json.Value<string>("SUBMITTIME"),
                      json.Value<string>("GOODSGW"), json.Value<string>("GOODSNW"), json.Value<string>("PACKKIND"), "001", "1", json.Value<string>("CLEARUNIT"),
                      json.Value<string>("CLEARUNITNAME"), json.Value<string>("BUSISHORTCODE"), GetChk(json.Value<string>("SPECIALRELATIONSHIP")),
                      GetChk(json.Value<string>("PRICEIMPACT")), GetChk(json.Value<string>("PAYPOYALTIES")), json.Value<string>("BUSISHORTNAME"), GetChk(json.Value<string>("SELFCHECK")),
                      GetChk(json.Value<string>("WEIGHTCHECK")), GetChk(json.Value<string>("ISWEIGHTCHECK")), json.Value<string>("CHECKEDGOODSNUM"), json.Value<string>("CHECKEDWEIGHT"));
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
                    Extension.Insert_FieldUpdate_History(ordercode, json, json_user, "10");
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
