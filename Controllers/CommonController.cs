using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using iTextSharp.text;
using iTextSharp.text.pdf;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    [Authorize]
    public class CommonController : Controller
    {
        int totalProperty = 0;
        string AdminUrl = ConfigurationManager.AppSettings["AdminUrl"];

        public ActionResult DeclareList()//报关单管理
        {
            return View();
        }
        public ActionResult FileConsult()//文件调阅
        {
            return View();
        }

        public ActionResult MultiPrint()//批量打印
        {
            return View();
        }
        public ActionResult InspectList()//报检单管理
        {
            return View();
        }

        //登录后显示菜单栏 by heguiqin 2016-08-25
        public string Header()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = @"select MODULEID,NAME,PARENTID,URL,SORTINDEX,IsLeaf from sysmodule t 
            where t.parentid='91a0657f-1939-4528-80aa-91b202a593ab' and t.MODULEID IN (select MODULEID FROM sys_moduleuser where userid='{0}')
            order by sortindex";
            sql = string.Format(sql, json_user.GetValue("ID"));
            DataTable dt1 = DBMgr.GetDataTable(sql);
            string result = "";
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                result += "<li><a href=\"" + dt1.Rows[i]["URL"] + "\">" + dt1.Rows[i]["NAME"] + "</a>";

                sql = @"select MODULEID,NAME,PARENTID,URL,SORTINDEX,IsLeaf from sysmodule t where t.parentid='{0}'
                and t.MODULEID IN (select MODULEID FROM sys_moduleuser where userid='{1}') order by sortindex";
                sql = string.Format(sql, dt1.Rows[i]["MODULEID"], json_user.GetValue("ID"));
                DataTable dt2 = DBMgr.GetDataTable(sql);
                if (dt2.Rows.Count > 0)
                {
                    result += "<ul>";
                    for (int j = 0; j < dt2.Rows.Count; j++)
                    {
                        result += "<li><a href=\"" + dt2.Rows[j]["URL"] + "\">" + dt2.Rows[j]["NAME"] + "</a>";

                        sql = @"select MODULEID,NAME,PARENTID,URL,SORTINDEX,IsLeaf from sysmodule t where t.parentid='{0}' 
                        and t.MODULEID IN (select MODULEID FROM sys_moduleuser where userid='{1}') order by sortindex";
                        sql = string.Format(sql, dt2.Rows[j]["MODULEID"], json_user.GetValue("ID"));
                        DataTable dt3 = DBMgr.GetDataTable(sql);
                        if (dt3.Rows.Count > 0)
                        {
                            result += "<ul>";
                            for (int k = 0; k < dt3.Rows.Count; k++)
                            {
                                result += "<li><a href=\"" + dt3.Rows[k]["URL"] + "\">" + dt3.Rows[k]["NAME"] + "</a></li>";

                            }
                            result += "</ul></li>";
                        }
                        else
                        {
                            result += "</li>";
                        }
                    }
                    result += "</ul></li>";
                }
                else
                {
                    result += "</li>";
                }
            }
            return result;
        }
        //登录后显示顶部当前用户中文名称 by heguiqin 2016-08-25
        public string CurrentUser()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            return json_user.GetValue("REALNAME") + "";
        }

        //订单列表页加载方法 by panhuaguo 2016-08-25
        public string LoadList()
        {

            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string where = "";
            if (!string.IsNullOrEmpty(Request["seniorsearch"] + ""))
            {
                string[] seniorarray = Request["seniorsearch"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                where += " and (";
                for (int i = 0; i < seniorarray.Length; i++)
                {
                    if (i != seniorarray.Length - 1)
                    {
                        where += "instr(DIVIDENO,'" + seniorarray[i] + "')>0 or ";
                    }
                    else
                    {
                        where += "instr(DIVIDENO,'" + seniorarray[i] + "')>0)";
                    }
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE1"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION1"])
                {
                    case "BUSIUNITCODE"://经营单位
                        where += " and BUSIUNITCODE='" + Request["VALUE1"] + "' ";
                        break;
                    case "CUSTOMDISTRICTCODE"://申报关区
                        where += " and CUSTOMAREACODE='" + Request["VALUE1"] + "' ";
                        break;
                    case "PORTCODE"://进口口岸
                        where += " and PORTCODE='" + Request["VALUE1"] + "' ";
                        break;
                    case "REPWAYID"://申报方式
                        where += " and REPWAYID='" + Request["VALUE1"] + "' ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE2"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION2"])
                {
                    case "CODE"://订单编号
                        where += " and instr(CODE,'" + Request["VALUE2"].Trim() + "')>0 ";
                        break;
                    case "CUSNO"://客户编号
                        where += " and instr(CUSNO,'" + Request["VALUE2"].Trim() + "')>0 ";
                        break;
                    case "DIVIDENO"://分单号
                        where += " and instr(DIVIDENO,'" + Request["VALUE2"].Trim() + "')>0 ";
                        break;
                    case "CONTRACTNO"://合同发票号
                        where += " and instr(CONTRACTNO,'" + Request["VALUE2"].Trim() + "')>0  ";
                        break;
                    case "MANIFEST"://载货清单号
                        where += " and instr(MANIFEST,'" + Request["VALUE2"].Trim() + "')>0  ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE3"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION3"])
                {
                    case "ddzt"://订单状态                      
                        if ((Request["VALUE3"] + "") == "草稿")  //草稿=草稿
                        {
                            where += " and STATUS='1' ";
                        }
                        if ((Request["VALUE3"] + "") == "订单待委托")   //订单待委托 < 已委托
                        {
                            where += " and STATUS < 15 ";
                        }
                        if ((Request["VALUE3"] + "") == "订单待受理") //已委托 <= 订单待受理 < 已受理
                        {
                            where += " and STATUS >= 15 and STATUS < 20 ";
                        }
                        if ((Request["VALUE3"] + "") == "订单受理中")  //已受理 <= 订单受理中 < 已交付
                        {
                            where += " and STATUS >= 20 and STATUS < 110 ";
                        }
                        if ((Request["VALUE3"] + "") == "订单待交付")  //已委托 <= 订单待交付 < 已交付
                        {
                            where += " and STATUS >= 15 and STATUS < 110 ";
                        }
                        if ((Request["VALUE3"] + "") == "订单已交付")  //订单已交付 = 已交付
                        {
                            where += " and STATUS='110' ";
                        }
                        if ((Request["VALUE3"] + "") == "订单已作废")
                        {
                            where += " and ISINVALID='1' ";
                        }
                        break;
                    case "bgzt":
                        where += " and DECLSTATUS='" + Request["VALUE3"] + "' ";
                        break;
                    case "bjzt":
                        where += " and INSPSTATUS='" + Request["VALUE3"] + "' ";
                        break;
                }
            }
            switch (Request["CONDITION4"])
            {
                case "SUBMITTIME"://委托日期 
                    if (!string.IsNullOrEmpty(Request["VALUE4_1"]))//如果开始时间有值
                    {
                        where += " and SUBMITTIME>=to_date('" + Request["VALUE4_1"] + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE4_2"]))//如果结束时间有值
                    {
                        where += " and SUBMITTIME<=to_date('" + Request["VALUE4_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    break;
                case "CSSTARTTIME"://订单开始时间
                    if (!string.IsNullOrEmpty(Request["VALUE4_1"]))//如果开始时间有值
                    {
                        where += " and CREATETIME>=to_date('" + Request["VALUE4_1"] + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE4_2"]))//如果结束时间有值
                    {
                        where += " and CREATETIME<=to_date('" + Request["VALUE4_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    break;
            }
            if (!string.IsNullOrEmpty(Request["VALUE5"]))//判断查询条件5是否有值
            {
                switch (Request["CONDITION5"])
                {
                    case "BUSIUNITCODE"://经营单位
                        where += " and BUSIUNITCODE='" + Request["VALUE5"] + "' ";
                        break;
                    case "CUSTOMDISTRICTCODE"://申报关区
                        where += " and CUSTOMAREACODE='" + Request["VALUE5"] + "' ";
                        break;
                    case "PORTCODE"://进口口岸
                        where += " and PORTCODE='" + Request["VALUE5"] + "' ";
                        break;
                    case "REPWAYID"://申报方式
                        where += " and REPWAYID='" + Request["VALUE5"] + "' ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE6"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION6"])
                {
                    case "CODE"://订单编号
                        where += " and instr(CODE,'" + Request["VALUE6"].Trim() + "')>0 ";
                        break;
                    case "CUSNO"://客户编号
                        where += " and instr(CUSNO,'" + Request["VALUE6"].Trim() + "')>0 ";
                        break;
                    case "DIVIDENO"://分单号
                        where += " and instr(DIVIDENO,'" + Request["VALUE6"].Trim() + "')>0 ";
                        break;
                    case "CONTRACTNO"://合同发票号
                        where += " and instr(CONTRACTNO,'" + Request["VALUE6"].Trim() + "')>0  ";
                        break;
                    case "MANIFEST"://载货清单号
                        where += " and instr(MANIFEST,'" + Request["VALUE6"].Trim() + "')>0  ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE7"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION7"])
                {
                    case "ddzt"://订单状态
                        //草稿=草稿
                        if ((Request["VALUE7"] + "") == "草稿")
                        {
                            where += " and STATUS='1' ";
                        }
                        //订单待委托 < 已委托
                        if ((Request["VALUE7"] + "") == "订单待委托")
                        {
                            where += " and STATUS < 15 ";
                        }
                        //已委托 <= 订单待受理 < 已受理
                        if ((Request["VALUE7"] + "") == "订单待受理")
                        {
                            where += " and STATUS >= 15 and STATUS < 20 ";
                        }
                        //已受理 <= 订单受理中 < 已交付
                        if ((Request["VALUE7"] + "") == "订单受理中")
                        {
                            where += " and STATUS >= 20 and STATUS < 110 ";
                        }
                        //已委托 <= 订单待交付 < 已交付
                        if ((Request["VALUE7"] + "") == "订单待交付")
                        {
                            where += " and STATUS >= 15 and STATUS < 110 ";
                        }
                        //订单已交付 = 已交付
                        if ((Request["VALUE7"] + "") == "订单已交付")
                        {
                            where += " and STATUS='110' ";
                        }
                        if ((Request["VALUE7"] + "") == "订单已作废")
                        {
                            where += " and ISINVALID='1' ";
                        }
                        break;
                    case "bgzt":
                        where += " and DECLSTATUS='" + Request["VALUE7"] + "' ";
                        break;
                    case "bjzt":
                        where += " and INSPSTATUS='" + Request["VALUE7"] + "' ";
                        break;
                }
            }
            switch (Request["CONDITION8"])
            {
                case "SUBMITTIME"://委托日期 
                    if (!string.IsNullOrEmpty(Request["VALUE8_1"]))//如果开始时间有值
                    {
                        where += " and SUBMITTIME>=to_date('" + Request["VALUE8_1"] + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE8_2"]))//如果结束时间有值
                    {
                        where += " and SUBMITTIME<=to_date('" + Request["VALUE8_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    break;
                case "CSSTARTTIME"://订单开始时间
                    if (!string.IsNullOrEmpty(Request["VALUE8_1"]))//如果开始时间有值
                    {
                        where += " and CREATETIME>=to_date('" + Request["VALUE8_1"] + "','yyyy-mm-dd hh24:mi:ss')' ";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE8_2"]))//如果结束时间有值
                    {
                        where += " and CREATETIME<=to_date('" + Request["VALUE8_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    break;
            }

            if ((Request["VALUE3"] + "") != "订单已作废" && (Request["VALUE7"] + "") != "订单已作废")//在不查询已作废的订单情形下，皆显示正常的订单
            {
                where += " and ISINVALID='0' ";
            }

            if ((Request["OnlySelf"] + "").Trim() == "fa fa-check-square-o")
            {
                where += " and CREATEUSERID = " + json_user.Value<string>("ID") + " ";
            }
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 BUSISHORTNAME,
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            /*string sql = @"select ID, CODE,ENTRUSTTYPEID,CUSNO,PORTCODE,PORTNAME,BUSIUNITNAME,FIRSTLADINGBILLNO,SECONDLADINGBILLNO,
                BUSITYPE,CORRESPONDNO,LADINGBILLNO,ARRIVEDNO,CUSTOMERNAME,CONTRACTNO,TOTALNO,DIVIDENO,TURNPRENO,                
                GOODSNUM || '/'|| GOODSGW  GOODSNUMGOODSNW,GOODSGW,REPWAYID, GOODSWEIGHT,CUSTOMDISTRICTCODE,
                CUSTOMDISTRICTNAME,BUSISHORTNAME,ISINVALID,LAWCONDITION,STATUS,DECLSTATUS,INSPSTATUS,    
                ASSOCIATENO,createtime CREATEDATE,SUBMITTIME from LIST_ORDER where instr('" + Request["busitypeid"] + "',BUSITYPE)>0 and customercode='" + json_user.Value<string>("CUSTOMERCODE") + "' " + where;
             */
            string sql = @"select * from LIST_ORDER where instr('" + Request["busitypeid"] + "',BUSITYPE)>0 and customercode='" + json_user.Value<string>("CUSTOMERCODE") + "' " + where;
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "createtime", "desc"));
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        //基础资料 by heguiqin 2016-08-25
        public string Ini_Base_Data()
        {
            IDatabase db = SeRedis.redis.GetDatabase();
            string sql = "";
            string json_sbfs = "[]";//申报方式
            string busitype = Request["busitype"];
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            sql = "select CODE,NAME||'('||CODE||')' NAME from SYS_REPWAY where Enabled=1 and instr(busitype,'" + busitype + "')>0";
            if (busitype == "空运进口")
            {
                if (db.KeyExists("common_data_sbfs:kj"))
                {
                    json_sbfs = db.StringGet("common_data_sbfs:kj");
                }
                else
                {
                    json_sbfs = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                    db.StringSet("common_data_sbfs:kj", json_sbfs);
                }
            }
            if (busitype == "空运出口")
            {
                if (db.KeyExists("common_data_sbfs:kc"))
                {
                    json_sbfs = db.StringGet("common_data_sbfs:kc");
                }
                else
                {
                    json_sbfs = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                    db.StringSet("common_data_sbfs:kc", json_sbfs);
                }
            }
            if (busitype == "陆运进口")
            {
                if (db.KeyExists("common_data_sbfs:lj"))
                {
                    json_sbfs = db.StringGet("common_data_sbfs:lj");
                }
                else
                {
                    json_sbfs = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                    db.StringSet("common_data_sbfs:lj", json_sbfs);
                }
            }
            if (busitype == "陆运出口")
            {
                if (db.KeyExists("common_data_sbfs:lc"))
                {
                    json_sbfs = db.StringGet("common_data_sbfs:lc");
                }
                else
                {
                    json_sbfs = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                    db.StringSet("common_data_sbfs:lc", json_sbfs);
                }
            }
            if (busitype == "海运进口")
            {
                if (db.KeyExists("common_data_sbfs:hj"))
                {
                    json_sbfs = db.StringGet("common_data_sbfs:hj");
                }
                else
                {
                    json_sbfs = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                    db.StringSet("common_data_sbfs:hj", json_sbfs);
                }
            }
            if (busitype == "海运出口")
            {
                if (db.KeyExists("common_data_sbfs:hc"))
                {
                    json_sbfs = db.StringGet("common_data_sbfs:hc");
                }
                else
                {
                    json_sbfs = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                    db.StringSet("common_data_sbfs:hc", json_sbfs);
                }
            }
            if (busitype == "特殊区域")
            {
                if (db.KeyExists("common_data_sbfs:ts"))
                {
                    json_sbfs = db.StringGet("common_data_sbfs:ts").ToString();
                }
                else
                {
                    json_sbfs = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                    db.StringSet("common_data_sbfs:ts", json_sbfs);
                }
            }
            if (busitype == "国内")
            {
                if (db.KeyExists("common_data_sbfs:gn"))
                {
                    json_sbfs = db.StringGet("common_data_sbfs:gn");
                }
                else
                {
                    json_sbfs = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                    db.StringSet("common_data_sbfs:gn", json_sbfs);
                }
            }

            string json_sbgq = "[]";//申报关区 进口口岸 
            if (db.KeyExists("common_data:sbgq"))
            {
                json_sbgq = db.StringGet("common_data:sbgq");
            }
            else
            {
                sql = "select CODE,NAME||'('||CODE||')' NAME from BASE_CUSTOMDISTRICT  where ENABLED=1 ORDER BY CODE";
                json_sbgq = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:sbgq", json_sbgq);
            }

            string json_jydw = "";//经营单位 
            if (db.KeyExists("jydw:" + json_user.Value<string>("CUSTOMERID")))
            {
                json_jydw = db.StringGet("jydw:" + json_user.Value<string>("CUSTOMERID"));
            }
            else
            {
                //2016-6-2 梁总提出一个改进 如果某一个经营单位 客户先 添加到自己的库了，但后来总库里面禁用了，则客户自己的库中也要禁用掉
                sql = @"SELECT T.* 
                        FROM (
                                SELECT a.CUSTOMERID, a.companychname||'('||a.companyenname||')' NAME ,a.companychname SHORTNAME, a.companyenname CODE,b.incode QUANCODE,b.name QUANNAME 
                                FROM USER_RENAME_COMPANY a 
                                    left join BASE_COMPANY b on a.companyid = b.id 
                                where b.incode is not null and a.companyenname is not null and b.enabled=1
                            ) T 
                            WHERE  T.CUSTOMERID = '" + json_user.Value<string>("CUSTOMERID") + "'";
                json_jydw = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("jydw:" + json_user.Value<string>("CUSTOMERID"), json_jydw);
            }
            string json_bgfs = "[]";//报关方式 
            if (db.KeyExists("common_data:bgfs"))
            {
                json_bgfs = db.StringGet("common_data:bgfs");
            }
            else
            {
                sql = "select CODE,NAME||'('||CODE||')' NAME  from SYS_DECLWAY where enabled=1 order by id asc";
                json_bgfs = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:bgfs", json_bgfs);
            }

            string json_bzzl = "[]";//包装种类 
            if (db.KeyExists("common_data:bzzl"))
            {
                json_bzzl = db.StringGet("common_data:bzzl");
            }
            else
            {
                sql = "select CODE,NAME||'('||CODE||')' NAME from base_Packing";
                json_bzzl = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:bzzl", json_bzzl);
            }

            string json_myfs = "[]";//贸易方式 
            if (db.KeyExists("common_data:myfs"))
            {
                json_myfs = db.StringGet("common_data:myfs");
            }
            else
            {
                sql = @"select ID,CODE,NAME||'('||CODE||')' NAME from BASE_DECLTRADEWAY WHERE enabled=1";
                json_myfs = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:myfs", json_myfs);
            }

            string json_containertype = "[]";//集装箱类型 
            if (db.KeyExists("common_data:containertype"))
            {
                json_containertype = db.StringGet("common_data:containertype");
            }
            else
            {
                sql = "select CODE,NAME||'('||CONTAINERCODE||')' as MERGENAME,CONTAINERCODE from BASE_CONTAINERTYPE where enabled=1";
                json_containertype = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:containertype", json_containertype);
            }

            string json_containersize = "[]";//集装箱尺寸 
            if (db.KeyExists("common_data:containersize"))
            {
                json_containersize = db.StringGet("common_data:containersize");
            }
            else
            {
                sql = "select CODE,NAME as CONTAINERSIZE,NAME||'('||DECLSIZE||')' as MERGENAME,DECLSIZE from BASE_CONTAINERSIZE where enabled=1";
                json_containersize = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:containersize", json_containersize);
            }

            string json_truckno = "[]";//报关车号 
            if (db.KeyExists("common_data:truckno"))
            {
                json_truckno = db.StringGet("common_data:truckno");
            }
            else
            {
                sql = @"select t.license, t.license||'('||t.whitecard||')' as MERGENAME,t.whitecard,t1.NAME||'('|| t1.CODE||')' as UNITNO from sys_declarationcar t
                left join base_motorcade t1 on t.motorcade=t1.code where t.enabled=1";
                json_truckno = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:truckno", json_truckno);
            }

            string json_relacontainer = "[]";//关联集装箱信息  通过选择集装箱类型的CODE和集装箱尺寸的CODE,会自动匹配关联集装箱信息
            if (db.KeyExists("common_data:relacontainer"))
            {
                json_relacontainer = db.StringGet("common_data:relacontainer");
            }
            else
            {
                sql = @"select CONTAINERSIZE,CONTAINERTYPE,FORMATNAME,CONTAINERHS from rela_container t where t.enabled=1";
                json_relacontainer = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:relacontainer", json_relacontainer);
            }
            //木质包装
            string json_mzbz = "[]";
            if (db.KeyExists("common_data:mzbz"))
            {
                json_mzbz = db.StringGet("common_data:mzbz");
            }
            else
            {
                sql = @"select CODE,NAME||'('||CODE||')' NAME from SYS_WOODPACKING";
                json_mzbz = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:mzbz", json_mzbz);
            }
            //20160918 add 暂时用在报检单展示    检验类别、申报库别
            //检验类别
            string json_jylb = "[]";
            if (db.KeyExists("common_data:jylb"))
            {
                json_jylb = db.StringGet("common_data:jylb");
            }
            else
            {
                sql = @"select CODE,NAME||'('||CODE||')' NAME from SYS_INSPTYPE";
                json_jylb = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:jylb", json_jylb);
            }
            //申报库别
            string json_sbkb = "[]";
            if (db.KeyExists("common_data:sbkb"))
            {
                json_sbkb = db.StringGet("common_data:sbkb");
            }
            else
            {
                sql = @"select CODE,NAME||'('||CODE||')' NAME from SYS_REPORTLIBRARY";
                json_sbkb = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:sbkb", json_sbkb);
            }
            //报检包装种类 
            string json_inspbzzl = "[]";
            if (db.KeyExists("common_data:inspbzzl"))
            {
                json_inspbzzl = db.StringGet("common_data:inspbzzl");
            }
            else
            {
                sql = @"select CODE,NAME||'('||CODE||')' NAME from BASE_INSPPACKAGE";
                json_inspbzzl = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:inspbzzl", json_inspbzzl);
            }

            return "{jydw:" + json_jydw + ",sbfs:" + json_sbfs + ",sbgq:" + json_sbgq + ",bgfs:" + json_bgfs + ",bzzl:" + json_bzzl 
                + ",myfs:" + json_myfs + ",containertype:" + json_containertype + ",containersize:" + json_containersize + ",truckno:" + json_truckno
                + ",relacontainer:" + json_relacontainer + ",mzbz:" + json_mzbz + ",jylb:" + json_jylb + ",json_sbkb:" + json_sbkb 
                + ",inspbzzl:" + json_inspbzzl + "}";
        }

        //查询条件默认值 by heguiqin 2016-08-26
        public string loadquerysetting()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            //首先判断该用户先前有没有设置查询条件
            string sql = "select * from CONFIG_QUERYSETTING where UserId='" + json_user.Value<string>("ID") + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                string json = (JsonConvert.SerializeObject(DBMgr.GetDataTable(sql))).TrimStart('[').TrimEnd(']');
                return "{success:true,data:" + json + "}";
            }
            else
            {
                return "{rows:''}";
            }
        }

        //分页 by heguiqin 2016-08-26
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

        //为了基础数据新增的
        private string GetPageSqlBase(string tempsql, string order, string asc)
        {
            int start = Convert.ToInt32(Request["start"]);
            int limit = Convert.ToInt32(Request["limit"]);
            string sql = "select count(1) from ( " + tempsql + " )";
            totalProperty = Convert.ToInt32(DBMgrBase.GetDataTable(sql).Rows[0][0]);
            string pageSql = @"SELECT * FROM ( SELECT tt.*, ROWNUM AS rowno FROM ({0} ORDER BY {1} {2}) tt WHERE ROWNUM <= {4}) table_alias WHERE table_alias.rowno >= {3}";
            pageSql = string.Format(pageSql, tempsql, order, asc, start + 1, limit + start);
            return pageSql;
        }

        //删除报关单及其对应文件信息 by heguiqin 2016-08-26
        public string Delete()
        {
            string result = "";
            result = Extension.deleteorder(Request["ordercode"] + "");
            return result;
        }

        //报关申报单位开创加载资料
        public string selectbgsbdw()
        {
            string where = "";
            if (!string.IsNullOrEmpty(Request["Name"]))
            {
                where += " and Name like '%" + Request["Name"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["Code"]))
            {
                where += " and Code like '%" + Request["Code"] + "%'";
            }
            string sql = "SELECT * FROM BASE_COMPANY WHERE  CODE IS NOT NULL AND ENABLED=1  " + where;
            DataTable dt = DBMgrBase.GetDataTable(GetPageSqlBase(sql, "CREATEDATE", "desc"));
            string json = JsonConvert.SerializeObject(dt); ;
            return "{total:" + totalProperty + ",rows:" + json + "}";
        }

        //报检申报单位开窗加载资料
        public string selectbjsbdw()
        {
            string where = "";
            if (!string.IsNullOrEmpty(Request["Name"]))
            {
                where += " and Name like '%" + Request["Name"] + "%'";
            }
            if (!string.IsNullOrEmpty(Request["INSPCODE"]))
            {
                where += " and INSPCODE like '%" + Request["INSPCODE"] + "%'";
            }
            string sql = "SELECT * FROM BASE_COMPANY WHERE  INSPCODE IS NOT NULL AND ENABLED=1  " + where;
            DataTable dt = DBMgrBase.GetDataTable(GetPageSqlBase(sql, "CREATEDATE", "desc"));
            string json = JsonConvert.SerializeObject(dt); ;
            return "{total:" + totalProperty + ",rows:" + json + "}";
        }

        //经营单位选择窗体弹出时加载基础库数据
        public JsonResult LoadJydw()
        {
            string sql = "";
            DataTable dt;
            sql = "SELECT ID,INCODE CODE,NAME FROM BASE_COMPANY WHERE ENABLED=1 AND (INCODE LIKE '%{0}%' OR NAME LIKE '%{0}%')";
            sql = string.Format(sql, (Request["NAME"] + "").ToUpper());
            dt = DBMgrBase.GetDataTable(GetPageSqlBase(sql, "CREATEDATE", "desc"));
            var data1 = (from B in dt.AsEnumerable()
                         select new
                         {
                             ID = B["ID"],
                             CODE = B["CODE"],
                             NAME = B["NAME"],
                         }).AsQueryable();
            var json5 = new { total = totalProperty, rows = data1 };
            return Json(json5, JsonRequestBehavior.AllowGet);
        }

        //经营单位开窗后，选择资料后 更新用户库
        public string UpdateRenameCompany()
        {
            //ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(AppUtil.RedisIp);
            //IDatabase db = redis.GetDatabase();
            IDatabase db = SeRedis.redis.GetDatabase();
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string IDS = Request["IDS"]; //更新经营单位简称表 
            string CODES = Request["CODES"];
            string NAMES = Request["NAMES"];
            string sql = @"select * from user_rename_company where CUSTOMERID ='" + json_user.Value<string>("CUSTOMERID") + "' and companyid = '" + IDS + "'";
            DataTable dt = DBMgrBase.GetDataTable(sql);
            string res_json = "";
            if (dt.Rows.Count == 0)
            {
                sql = @"INSERT INTO USER_RENAME_COMPANY (ID,CUSTOMERID,COMPANYID,COMPANYCHNAME,COMPANYENNAME,CREATEDATE) 
                            VALUES (USER_RENAME_COMPANY_id.Nextval,'{0}','{1}','{2}','{3}',sysdate)";
                sql = string.Format(sql, json_user.Value<string>("CUSTOMERID"), IDS, NAMES, CODES);
                DBMgrBase.ExecuteNonQuery(sql);

                //更新redis,同步最新经营单位简称 这里记得需要和Ini_Base_Data的数据结构保持一致 update by panhuaguo 2016-07-29         
                sql = @"SELECT T.* 
                        FROM (
                            SELECT a.CUSTOMERID, a.companychname||'('||a.companyenname||')' NAME ,a.companychname SHORTNAME, a.companyenname CODE,b.incode QUANCODE,b.name QUANNAME 
                            FROM USER_RENAME_COMPANY a 
                                left join BASE_COMPANY b on a.companyid = b.id  
                            where b.incode is not null and a.companyenname is not null and b.enabled=1
                            ) T 
                      WHERE  T.CUSTOMERID = '" + json_user.Value<string>("CUSTOMERID") + "'";
                var json = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

                db.StringSet("jydw:" + json_user.Value<string>("CUSTOMERID"), json);
                res_json = "{CODE:'" + CODES + "',NAME:'" + NAMES + "',SHORTNAME:'" + NAMES + "',QUANCODE:'" + CODES + "',QUANNAME:'" + NAMES + "',data:" + json + "}";
            }
            else//如果简称库已经存在
            {
                string json = db.StringGet("jydw:" + json_user.Value<string>("CUSTOMERID"));
                res_json = "{CODE:'" + dt.Rows[0]["COMPANYENNAME"] + "',NAME:'" + dt.Rows[0]["COMPANYCHNAME"] + "',SHORTNAME:'" + dt.Rows[0]["COMPANYCHNAME"] + "',QUANCODE:'" + CODES + "',QUANNAME:'" + NAMES + "',data:" + json + "}";
            }
            return res_json;
        }

        //贸易方式开窗加载资料
        public string LoadMyfs()
        {
            string sql = @"select ID,CODE,NAME from BASE_DECLTRADEWAY WHERE CODE LIKE '%{0}%' OR NAME LIKE '%{0}%'";
            sql = string.Format(sql, Request["NAME"] + "");
            var json = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(GetPageSqlBase(sql, "CODE", "asc")));
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        //文件上传到web服务器
        public ActionResult UploadFile(int? chunk, string name)
        {
            var fileUpload = Request.Files[0];
            var uploadPath = Server.MapPath("/FileUpload/file");
            chunk = chunk ?? 0;
            using (var fs = new FileStream(Path.Combine(uploadPath, name), chunk == 0 ? FileMode.Create : FileMode.Append))
            {
                var buffer = new byte[fileUpload.InputStream.Length];
                fileUpload.InputStream.Read(buffer, 0, buffer.Length);
                fs.Write(buffer, 0, buffer.Length);
            }
            return Content("chunk uploaded", "text/plain");
        }

        //非国内订单订单编辑页在首次加载或者修改时封装的方法 by panhuaguo 20160729   国内订单要复杂很多，单独放在其自身控制器里面
        public string loadorder()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string ordercode = Request["ordercode"];
            string copyordercode = Request["copyordercode"];
            DataTable dt;
            string bgsb_unit = "";
            string bjsb_unit = "";
            string result = "{}";
            if (string.IsNullOrEmpty(ordercode))//如果订单号为空、即新增的时候
            {
                string sql = "select * from base_company where CODE='" + json_user.Value<string>("CUSTOMERHSCODE") + "' AND ENABLED=1 AND ROWNUM=1";//根据海关的10位编码查询申报单位
                dt = DBMgrBase.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    bgsb_unit = dt.Rows[0]["NAME"] + "";
                }
                sql = "select * from base_company where INSPCODE='" + json_user.Value<string>("CUSTOMERCIQCODE") + "' AND ENABLED=1 AND ROWNUM=1";//根据海关的10位编码查询申报单位
                dt = DBMgrBase.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    bjsb_unit = dt.Rows[0]["NAME"] + "";
                }
                if (string.IsNullOrEmpty(copyordercode))//如果不是复制新增
                {
                    string formdata = "{STATUS:1,REPUNITNAME:'" + bgsb_unit + "',REPUNITCODE:'" + json_user.Value<string>("CUSTOMERHSCODE") + "',INSPUNITNAME:'" + bjsb_unit + "',INSPUNITCODE:'" + json_user.Value<string>("CUSTOMERCIQCODE") + "'}";
                    result = "{formdata:" + formdata + ",filedata:[]}";
                }
                else//如果是复制新增
                {
                    sql = @"select t.*,'' CONTAINERTRUCK from LIST_ORDER t where t.CODE = '" + copyordercode + "' and rownum=1";
                    dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        dt.Rows[0]["CODE"] = DBNull.Value; dt.Rows[0]["STATUS"] = "1";
                        dt.Rows[0]["CREATEUSERID"] = DBNull.Value; dt.Rows[0]["CREATEUSERNAME"] = DBNull.Value;
                        dt.Rows[0]["SUBMITTIME"] = DBNull.Value; dt.Rows[0]["CREATETIME"] = DBNull.Value;
                        dt.Rows[0]["SUBMITUSERNAME"] = DBNull.Value; dt.Rows[0]["SUBMITUSERID"] = DBNull.Value;
                        dt.Rows[0]["CONTAINERNO"] = DBNull.Value; dt.Rows[0]["DECLCARNO"] = DBNull.Value;
                        //报关、报检申报单位
                        dt.Rows[0]["REPUNITNAME"] = bgsb_unit; dt.Rows[0]["REPUNITCODE"] = json_user.Value<string>("CUSTOMERHSCODE");
                        dt.Rows[0]["INSPUNITNAME"] = bjsb_unit; dt.Rows[0]["INSPUNITCODE"] = json_user.Value<string>("CUSTOMERCIQCODE");
                        //件数和重量也要清空
                        dt.Rows[0]["GOODSNUM"] = DBNull.Value; dt.Rows[0]["PACKKIND"] = DBNull.Value;
                        dt.Rows[0]["GOODSGW"] = DBNull.Value; dt.Rows[0]["GOODSNW"] = DBNull.Value;
                        string formdata = JsonConvert.SerializeObject(dt).TrimStart('[').TrimEnd(']');
                        result = "{formdata:" + formdata + ",filedata:[]}";
                    }
                }
            }
            else //如果订单号不为空
            {
                //订单基本信息 CONTAINERTRUCK 这个字段本身不属于list_order表,虚拟出来存储集装箱和报关车号记录,是个数组形式的字符串
                string sql = @"select t.*,'' CONTAINERTRUCK from LIST_ORDER t where t.CODE = '" + Request["ordercode"] + "' and rownum=1";
                dt = DBMgr.GetDataTable(sql);
                IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                sql = "select * from list_predeclcontainer t where t.ordercode='" + dt.Rows[0]["CODE"] + "' order by containerorder";
                DataTable dt_container = DBMgr.GetDataTable(sql);
                dt.Rows[0]["CONTAINERTRUCK"] = JsonConvert.SerializeObject(dt_container);
                string formdata = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');
                //订单随附文件
                sql = @"select * from LIST_ATTACHMENT where instr(ordercode,'{0}') >0 
                      and ((filetype=44 or filetype=58) or ( filetype=57 AND confirmstatus = 1 )) and (abolishstatus is null or abolishstatus=0)";
                sql = string.Format(sql, ordercode);
                dt = DBMgr.GetDataTable(sql);
                string filedata = JsonConvert.SerializeObject(dt, iso);
                result = "{formdata:" + formdata + ",filedata:" + filedata + "}";
            }
            return result;
        }

        //撤单  非国内订单提交后执行撤单操作统一执行此方法 by panhuaguo 2016-08-23
        public string CancelSubmit()
        {
            string ordercode = Request["ordercode"];
            //1先判断订单的状态有没有发生变化，比如是否受理
            string sql = "select * from list_order where code='" + ordercode + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            string result = "";
            if (dt.Rows[0]["STATUS"] + "" != "15")
            {
                result = "{success:false}";
            }
            else
            {
                sql = "delete from list_times where code='" + ordercode + "' and status = '15'";//删除订单状态变更日志信息
                DBMgr.ExecuteNonQuery(sql);
                sql = "update list_order set STATUS = '10' ,SUBMITUSERNAME='',SUBMITTIME='' where code='" + ordercode + "'";
                DBMgr.ExecuteNonQuery(sql);
                result = "{success:true}";
            }
            return result;
        }

        /*报关单管理 列表页展示*/
        public string LoadDeclarationList()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string where = "";
            string role = Request["role"];
            string busitypeid = Request["busitypeid"];
            if (!string.IsNullOrEmpty(Request["VALUE1"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION1"])
                {
                    case "BUSIUNITCODE"://经营单位
                        where += " and ort.BUSIUNITNAME='" + Request["VALUE1"] + "' ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE2"]))//判断查询条件2是否有值
            {
                switch (Request["CONDITION2"])
                {
                    case "CUSNO"://客户编号
                        where += " and instr(ort.CUSNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "BLNO"://提运单号
                        where += " and instr(det.BLNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "ORDERCODE"://订单编号
                        where += " and instr(det.ORDERCODE,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "DECLCARNO"://报关车号
                        where += " and instr(ort.DECLCARNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "TRANSNAME"://运输工具名称
                        where += " and instr(det.TRANSNAME,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "DECLNO"://报关单号
                        where += " and instr(det.DECLARATIONCODE,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "CONTRACTNO"://合同协议号
                        where += " and instr(det.CONTRACTNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE3"]))//判断查询条件3是否有值
            {
                switch (Request["CONDITION3"])
                {
                    case "DYBZ"://打印标志
                        where += " and det.ISPRINT='" + Request["VALUE3"] + "' ";
                        break;
                }
            }
            switch (Request["CONDITION4"])
            {
                case "SUBMITTIME"://订单委托日期 
                    if (!string.IsNullOrEmpty(Request["VALUE4_1"]))//如果开始时间有值
                    {
                        where += " and ort.SUBMITTIME>=to_date('" + Request["VALUE4_1"] + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE4_2"]))//如果结束时间有值
                    {
                        where += " and ort.SUBMITTIME<=to_date('" + Request["VALUE4_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    break;
                case "REPTIME"://申报时间
                    if (!string.IsNullOrEmpty(Request["VALUE4_1"]))//如果开始时间有值
                    {
                        where += " and det.REPSTARTTIME>=to_date('" + Request["VALUE4_1"] + "','yyyy-mm-dd hh24:mi:ss') "; 
                        //" and det.REPTIME>=to_date('" + Request["VALUE4_1"] + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE4_2"]))//如果结束时间有值
                    {
                        where += " and det.REPSTARTTIME<=to_date('" + Request["VALUE4_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') "; 
                        //" and det.REPTIME<=to_date('" + Request["VALUE4_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    break;
            }
            if (role == "supplier") //如果是现场服务角色
            {
                where += @" and cus.SCENEDECLAREID ='" + json_user.Value<string>("CUSTOMERID") + "' ";
            }//如果是企业服务
            if (role == "enterprise")
            {
                where += @" and ort.BUSIUNITCODE IN 
                    (SELECT b.incode QUANCODE FROM USER_RENAME_COMPANY a left join BASE_COMPANY b on a.companyid = b.id 
                    where b.incode is not null and a.companyenname is not null and a.customerid = " + json_user.Value<string>("CUSTOMERID") + ") ";
            }
            if (role == "customer")//如果角色是客户
            {
                where += @" and ort.customercode ='" + json_user.Value<string>("CUSTOMERCODE") + "' ";
            }           
           
            //2016-6-24 更新报关单列表显示逻辑 根据报关单对应的订单【DECLPDF】即报关单是否已关联好PDF文件，作为显示的条件 国内业务不需要去判断关联订单，因为打这两个标志的时候已经判断了           
            //DECL_TRANSNAME 预制报关单的运输工具名称
            //运输工具名称的显示需要更改为一下逻辑：根据草单中的申报库别 如果是13或者17 运输工具名称取预制报关单里面的。否则取草单的运输工具名称
            /*string sql = @"select det.ID,det.PREDECLCODE,det.DECLARATIONCODE,det.CODE,ort.CUSTOMERNAME ,det.REPFINISHTIME, det.CUSTOMSSTATUS ,   
                         det.ISPRINT,det.CONTRACTNO,det.GOODSNUM,det.GOODSNW,det.SHEETNUM,det.ORDERCODE,det.STARTTIME CREATEDATE,
                         det.BUSITYPE BUSITYPE,det.TRANSNAME DECL_TRANSNAME,
                         prt.TRANSNAME,prt.BUSIUNITCODE, prt.PORTCODE, prt.BLNO, prt.DECLTYPE, 
                         ort.REPWAYID ,ort.REPWAYID REPWAYNAME,ort.DECLWAY ,ort.DECLWAY DECLWAYNAME,ort.TRADEWAYCODES ,
                         ort.CUSNO ,ort.IETYPE,ort.ASSOCIATENO,ort.CORRESPONDNO,ort.BUSIUNITNAME                                                                          
                         from list_declaration det 
                         left join list_predeclaration prt  on det.predeclcode = prt.predeclcode 
                         left join list_order ort on prt.ordercode = ort.code 
                         where (ort.DECLPDF =1 or ort.PREPDF=1) and det.isinvalid=0 and instr('" + busitypeid + "',det.busitype)>0 " + where;*/
            string sql = @"select det.ID,det.DECLARATIONCODE,det.CODE,ort.CUSTOMERNAME ,det.REPENDTIME REPFINISHTIME, det.CUSTOMSSTATUS ,   
                         det.CONTRACTNO,det.GOODSNUM,det.GOODSNW,det.SHEETNUM,det.ORDERCODE,det.COSTARTTIME CREATEDATE,
                         det.TRANSNAME DECL_TRANSNAME, det.ISPRINT,
                         det.TRANSNAME,det.BUSIUNITCODE, det.PORTCODE, det.BLNO, det.DECLTYPE, 
                         ort.REPWAYID ,ort.REPWAYID REPWAYNAME,ort.DECLWAY ,ort.DECLWAY DECLWAYNAME,ort.TRADEWAYCODES ,
                         ort.CUSNO ,ort.IETYPE,ort.ASSOCIATENO,ort.CORRESPONDNO,ort.BUSIUNITNAME,ort.BUSITYPE, 
                         cus.SCENEDECLAREID                                                                          
                         from list_declaration det 
                              left join list_order ort on det.ordercode = ort.code 
                              left join sys_customer cus on ort.customercode = cus.code 
                         where (ort.DECLPDF =1 or ort.PREPDF=1) and det.isinvalid=0 and instr('" + busitypeid + "',ort.BUSITYPE)>0 " + where;
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "CREATETIME", "desc"));
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        /*通过查询条件获取具体的报关单海关状态数据 list_receiptstatus 这个表是预制报关单海关状态回执表
         目前有两个地方在抓取 我显示时按照时间进行升序，如果有重复的状态名称,取时间早的那个时间值
         by panhuaguo 2016-6-3*/
        public string LoadCustomsReceipt()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string sql = @"select * from (select l.DECLSTATUS,min(l.TIMES) TIMES from list_receiptstatus l where l.DECLSTATUS is not null and 
                         l.code = '" + Request["bgdcode"] + "' group by l.DECLSTATUS) a order by a.times asc ";
            DataTable dt = DBMgr.GetDataTable(sql);
            string json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + "}";
        }

        #region FileConsult 文件调阅 测试单号：16063955942

        //文件调阅功能菜单
        public string ConsultInfo()
        {
            string ordercode = Request["ordercode"];
            string id = Request["id"];
            string sql = "";
            DataTable dt;
            string result = "";
            if (id == "-1")  //1 根据委托类型加载业务单据   01 报关单  02  报检单  03  报关报检
            {
                sql = "select * from list_order where code='" + ordercode + "'";
                dt = DBMgr.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    string entrusttypeid = dt.Rows[0]["ENTRUSTTYPEID"] + "";
                    switch (entrusttypeid)
                    {
                        case "01":
                            result += "[{id:'order',typename:'委托',leaf:false},{id:'declare',typename:'报关'}]";
                            break;
                        case "02":
                            result += "[{id:'order',typename:'委托',leaf:false},{id:'declare',typename:'报检'}]";
                            break;
                        case "03":
                            result += "[{id:'order',typename:'委托',leaf:false},{id:'declare',typename:'报关'},{id:'inspect',typename:'报检'}]";
                            break;
                    }
                }
                return result;
            }
            //44 订单文件  58 配仓单文件  57  转关单文件  这三种类型都属于订单业务下
            if (id == "order")
            {
                sql = @"select f.FILETYPEID,f.FILETYPENAME  from sys_filetype f 
                      where (f.FILETYPEID='44' or f.FILETYPEID='58' or f.FILETYPEID='57') 
                      and f.FILETYPEID IN (select t.FILETYPE from List_Attachment t where t.ordercode='" + ordercode + "') order by f.FILETYPEID asc ";
                dt = DBMgr.GetDataTable(sql);
                int i = 0;
                result += "[";
                foreach (DataRow dr in dt.Rows)
                {
                    if (i != dt.Rows.Count - 1)
                    {
                        result += "{id:'" + dr["FILETYPEID"] + "',typename:'" + dr["FILETYPENAME"] + "'},";
                    }
                    else
                    {
                        result += "{id:'" + dr["FILETYPEID"] + "',typename:'" + dr["FILETYPENAME"] + "'}";
                    }
                    i++;
                }
                result += "]";
                return result;
            }
            if (id == "44")
            {
                int i = 0;
                sql = @"select t.* from list_attachment t where t.filetype='44' and t.splitstatus=1 and t.ordercode='" + ordercode + "'";
                dt = DBMgr.GetDataTable(sql);
                result += "[";
                foreach (DataRow dr in dt.Rows)
                {
                    if (i != dt.Rows.Count - 1)
                    {
                        result += "{id:'44_" + (i + 1) + "',typename:'订单文件_" + (i + 1) + "',fileid:'" + dr["ID"] + "',url:'" + AdminUrl + "/file/" + dr["FILENAME"] + "'},";
                    }
                    else
                    {
                        result += "{id:'44_" + (i + 1) + "',typename:'订单文件_" + (i + 1) + "',fileid:'" + dr["ID"] + "',url:'" + AdminUrl + "/file/" + dr["FILENAME"] + "'}";
                    }
                    i++;
                }
                result += "]";
                return result;
            }
            if (id.LastIndexOf("44_") >= 0)
            {
                int i = 0;
                sql = @"select t.*,f.FILETYPENAME,t.rowid from list_attachmentdetail t left join  sys_filetype f on t.filetypeid=f.filetypeid
                                where t.ordercode='" + ordercode + "' order by f.filetypeid asc";
                dt = DBMgr.GetDataTable(sql);
                result += "[";
                foreach (DataRow dr in dt.Rows)
                {
                    if (i != dt.Rows.Count - 1)
                    {
                        result += "{id:'" + dr["FILETYPEID"] + "',typename:'" + dr["FILETYPENAME"] + "',filename:'" + dr["FILENAME"] + "',leaf:true,url:'" + AdminUrl + "/file/" + dr["SOURCEFILENAME"] + "'},";
                    }
                    else
                    {
                        result += "{id:'" + dr["FILETYPEID"] + "',typename:'" + dr["FILETYPENAME"] + "',filename:'" + dr["FILENAME"] + "',leaf:true,url:'" + AdminUrl + "/file/" + dr["SOURCEFILENAME"] + "'}";
                    }
                    i++;
                }
                result += "]";
                return result;
            }
            if (id == "declare")//报关下面涉及的文件类型有 63  报关单(提前)  61  报关单
            {
                sql = @"select f.FILETYPEID,f.FILETYPENAME  from sys_filetype f where
                       f.FILETYPEID IN (select t.FILETYPE from List_Attachment t where  (t.FILETYPE='63' or t.FILETYPE='61') and t.ordercode='" + ordercode + "') order by f.FILETYPEID asc ";
                dt = DBMgr.GetDataTable(sql);
                int i = 0;
                result += "[";
                foreach (DataRow dr in dt.Rows)
                {
                    if (i != dt.Rows.Count - 1)
                    {
                        result += "{id:'" + dr["FILETYPEID"] + "',typename:'" + dr["FILETYPENAME"] + "'},";
                    }
                    else
                    {
                        result += "{id:'" + dr["FILETYPEID"] + "',typename:'" + dr["FILETYPENAME"] + "'}";
                    }
                    i++;
                }
                result += "]";
                return result;
            }
            if (id == "61")//报关单下面显示所有的报关单号 每个报关单号下可能会有一个或者多个报关单文件 236920161696176849
            {
                int i = 0;
                sql = @"select CODE,DECLARATIONCODE from list_declaration t where t.ordercode='" + ordercode + "' order by starttime";
                dt = DBMgr.GetDataTable(sql);
                result += "[";
                foreach (DataRow dr in dt.Rows)
                {
                    if (i != dt.Rows.Count - 1)
                    {
                        result += "{id:'" + dr["CODE"] + "',typename:'" + dr["DECLARATIONCODE"] + "'},";
                    }
                    else
                    {
                        result += "{id:'" + dr["CODE"] + "',typename:'" + dr["DECLARATIONCODE"] + "'}";
                    }
                    i++;
                }
                result += "]";
                return result;

            }
            if (id == "63")//报关单提前 16063955942这个订单有报关单提前数据  2016-6-7暂定取出所有的该订单下所有的报关单提前文件
            {
                int i = 0;
                sql = @"select t.* from list_attachment t where t.filetype='63' and t.ordercode='" + ordercode + "' order by  uploadtime asc";
                dt = DBMgr.GetDataTable(sql);//G16055233755001  这个预支报关单号下面有两个报关单文件
                result += "[";
                foreach (DataRow dr in dt.Rows)
                {
                    if (i != dt.Rows.Count - 1)
                    {
                        result += "{id:'" + id + "_" + (i + 1) + "',typename:'报关单提前文件_" + (i + 1) + "',fileid:'" + dr["ID"] + "',leaf:true,url:'" + AdminUrl + "/file/" + dr["FILENAME"] + "'},";
                    }
                    else
                    {
                        result += "{id:'" + id + "_" + (i + 1) + "',typename:'报关单提前文件_" + (i + 1) + "',fileid:'" + dr["ID"] + "',leaf:true,url:'" + AdminUrl + "/file/" + dr["FILENAME"] + "'}";
                    }
                    i++;
                }
                result += "]";
                return result;

            }
            if (id.Length == 15 && id.Substring(0, 1) == "G")//如果节点是预制报关单号 则加载其下所有的文件
            {
                int i = 0;
                sql = @"select t.* from list_attachment t where t.filetype='61' and t.declcode='" + id + "' order by pgindex asc,uploadtime asc";
                dt = DBMgr.GetDataTable(sql);//G16055233755001  这个预支报关单号下面有两个报关单文件
                result += "[";
                foreach (DataRow dr in dt.Rows)
                {
                    if (i != dt.Rows.Count - 1)
                    {
                        result += "{id:'" + id + "_" + (i + 1) + "',typename:'报关单文件_" + (i + 1) + "',fileid:'" + dr["ID"] + "',leaf:true,url:'" + AdminUrl + "/file/" + dr["FILENAME"] + "'},";
                    }
                    else
                    {
                        result += "{id:'" + id + "_" + (i + 1) + "',typename:'报关单文件_" + (i + 1) + "',fileid:'" + dr["ID"] + "',leaf:true,url:'" + AdminUrl + "/file/" + dr["FILENAME"] + "'}";
                    }
                    i++;
                }
                result += "]";
                return result;
            }
            //报检入口  涉及的文件类型有 62 报检单 121  报检核准单
            if (id == "inspect")
            {
                sql = @"select f.FILETYPEID,f.FILETYPENAME  from sys_filetype f where
                       f.FILETYPEID IN (select t.FILETYPE from List_Attachment t where  (t.FILETYPE='62' or t.FILETYPE='121') and t.ordercode='" + ordercode + "') order by f.FILETYPEID asc ";
                dt = DBMgr.GetDataTable(sql);
                int i = 0;
                result += "[";
                foreach (DataRow dr in dt.Rows)
                {
                    if (i != dt.Rows.Count - 1)
                    {
                        result += "{id:'" + dr["FILETYPEID"] + "',typename:'" + dr["FILETYPENAME"] + "'},";
                    }
                    else
                    {
                        result += "{id:'" + dr["FILETYPEID"] + "',typename:'" + dr["FILETYPENAME"] + "'}";
                    }
                    i++;
                }
                result += "]";
                return result;
            }
            //显示报检业务的下的所有报检单 一个订单可能会有多个报检单，一个报检单号会对应多个报检单文件
            //报检单和报检核准单都是挂在预制报检单list_inspection 这个表下面
            if (id == "62")
            {
                int i = 0;
                sql = @"select CODE,INSPECTIONCODE from list_inspection t where t.ordercode='" + ordercode + "' and t.INSPECTIONCODE is not null order by starttime";
                dt = DBMgr.GetDataTable(sql);
                result += "[";
                foreach (DataRow dr in dt.Rows)
                {
                    if (i != dt.Rows.Count - 1)
                    {
                        result += "{id:'" + id + "_" + dr["CODE"] + "',typename:'" + dr["INSPECTIONCODE"] + "'},";
                    }
                    else
                    {
                        result += "{id:'" + id + "_" + dr["CODE"] + "',typename:'" + dr["INSPECTIONCODE"] + "'}";
                    }
                    i++;
                }
                result += "]";
                return result;
            }
            if (id == "121")
            {
                int i = 0;
                sql = @"select CODE,APPROVALCODE from list_inspection t where t.ordercode='" + ordercode + "' and t.APPROVALCODE is not null order by starttime";
                dt = DBMgr.GetDataTable(sql);
                result += "[";
                foreach (DataRow dr in dt.Rows)
                {
                    if (i != dt.Rows.Count - 1)
                    {
                        result += "{id:'" + id + "_" + dr["CODE"] + "',typename:'" + dr["APPROVALCODE"] + "'},";
                    }
                    else
                    {
                        result += "{id:'" + id + "_" + dr["CODE"] + "',typename:'" + dr["APPROVALCODE"] + "'}";
                    }
                    i++;
                }
                result += "]";
                return result;
            }
            if (id.LastIndexOf("62_J") >= 0)//显示报检核准单
            {
                int i = 0;
                sql = @"select t.* from list_attachment t where t.filetype='62' and t.ORDERCODE='" + ordercode + "' and t.INSPCODE='" + id.Replace("62_", "") + "' order by pgindex asc,uploadtime asc";
                dt = DBMgr.GetDataTable(sql);
                result += "[";
                foreach (DataRow dr in dt.Rows)
                {
                    if (i != dt.Rows.Count - 1)
                    {
                        result += "{id:'" + id + "_" + (i + 1) + "',typename:'报检单文件_" + (i + 1) + "',fileid:'" + dr["ID"] + "',leaf:true,url:'" + AdminUrl + "/file/" + dr["FILENAME"] + "'},";
                    }
                    else
                    {
                        result += "{id:'" + id + "_" + (i + 1) + "',typename:'报检单文件_" + (i + 1) + "',fileid:'" + dr["ID"] + "',leaf:true,url:'" + AdminUrl + "/file/" + dr["FILENAME"] + "'}";
                    }
                    i++;
                }
                result += "]";
                return result;
            }
            if (id.LastIndexOf("121_J") >= 0)//显示报检核准单
            {
                int i = 0;
                sql = @"select t.* from list_attachment t where t.filetype='121' and t.ORDERCODE='" + ordercode + "' and t.INSPCODE='" + id.Replace("121_", "") + "' order by pgindex asc,uploadtime asc";
                dt = DBMgr.GetDataTable(sql);
                result += "[";
                foreach (DataRow dr in dt.Rows)
                {
                    if (i != dt.Rows.Count - 1)
                    {
                        result += "{id:'" + id + "_" + (i + 1) + "',typename:'报检核准单文件_" + (i + 1) + "',fileid:'" + dr["ID"] + "',leaf:true,url:'" + AdminUrl + "/file/" + dr["FILENAME"] + "'},";
                    }
                    else
                    {
                        result += "{id:'" + id + "_" + (i + 1) + "',typename:'报检核准单文件_" + (i + 1) + "',fileid:'" + dr["ID"] + "',leaf:true,url:'" + AdminUrl + "/file/" + dr["FILENAME"] + "'}";
                    }
                    i++;
                }
                result += "]";
                return result;
            }
            //报检单号：J16052035234001
            return "";
        }

        public string SinglePrint()//单个文件打印
        {
            string fileid = Request["fileid"];//文件ID
            string printtype = Request["printtype"];//打印类型
            string busitype = Request["busitype"];//业务类型
            string printtmp = Request["printtmp"];//打印模板
            string sql = "select t.* from List_Attachment t where t.id='" + fileid + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            string output = Guid.NewGuid() + "";
            IList<string> filelist = new List<string>();
            if (printtype == "standardprint")//如果是标准打印
            {
                string predeclcode = (dt.Rows[0]["DECLCODE"] + "");
                predeclcode = predeclcode.Substring(0, predeclcode.Length - 3);

                /*sql = "select t.*, t.rowid from list_predeclaration t where t.predeclcode='" + predeclcode + "'";*/
                sql = "select t.*, t.rowid from list_declaration t where t.code='" + predeclcode + "'";

                DataTable dt_pre = DBMgr.GetDataTable(sql);
                //报关单标准打印的时候用户必须在前端选择多个打印模板
                string[] tmp_array = printtmp.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (tmp_array.Length > 0)
                {
                    foreach (string tmpname in tmp_array)
                    {
                        string outpath = AddBackground(dt.Rows[0]["FILENAME"] + "", tmpname, busitype, dt_pre.Rows[0]["DECLTYPE"] + "");
                        filelist.Add(outpath);
                    }
                    UpdatePrintInfo("list_declaration", dt.Rows[0]["DECLCODE"] + "");
                }
                else
                {
                    filelist.Add(AdminUrl + "/file/" + dt.Rows[0]["FILENAME"]);
                    UpdatePrintInfo("list_declaration", dt.Rows[0]["DECLCODE"] + "");
                }
            }
            else
            {
                filelist.Add(AdminUrl + "/file/" + dt.Rows[0]["FILENAME"]);
                UpdatePrintInfo("list_declaration", dt.Rows[0]["DECLCODE"] + "");
            }
            MergePDFFiles(filelist, Server.MapPath("~/Declare/") + output + ".pdf");
            return "/Declare/" + output + ".pdf";
        }

        //给报关单文件增加背景图片 by panhuaguo 2016-04-19  
        //修改需求 如果申报关区是昆山综保2369的话 使用进境出境作为报关单背景图 by panhuaguo 2016-08-15
        //修改需求 依据申报库别进行判断
        public string AddBackground(string filename, string printtmp, string busitype, string decltype)
        {
            string outname = Guid.NewGuid() + "";
            //
            Image img = null;
            if (busitype == "11" || busitype == "21" || busitype == "31" || busitype == "41" || busitype == "51")
            {
                if (decltype == "11" || decltype == "17")
                {
                    img = Image.GetInstance(Server.MapPath("/FileUpload/进境备案清单简化.jpg"));
                }
                else
                {
                    if (printtmp == "海关作业联")
                    {
                        img = Image.GetInstance(Server.MapPath("/FileUpload/进口-海关作业联.png"));
                    }
                    if (printtmp == "企业留存联")
                    {
                        img = Image.GetInstance(Server.MapPath("/FileUpload/进口-企业留存联.png"));
                    }
                    if (printtmp == "海关核销联")
                    {
                        img = Image.GetInstance(Server.MapPath("/FileUpload/进口-海关核销联.png"));
                    }
                }
            }
            else
            {
                if (decltype == "12" || decltype == "18")
                {
                    img = Image.GetInstance(Server.MapPath("/FileUpload/出境备案清单简化.jpg"));
                }
                else
                {
                    if (printtmp == "海关作业联")
                    {
                        img = Image.GetInstance(Server.MapPath("/FileUpload/出口-海关作业联.png"));
                    }
                    if (printtmp == "企业留存联")
                    {
                        img = Image.GetInstance(Server.MapPath("/FileUpload/出口-企业留存联.png"));
                    }
                    if (printtmp == "海关核销联")
                    {
                        img = Image.GetInstance(Server.MapPath("/FileUpload/出口-海关核销联.png"));
                    }
                }
            }
            string destFile = Server.MapPath("~/Declare/") + outname + ".pdf";
            FileStream stream = new FileStream(destFile, FileMode.Create, FileAccess.ReadWrite);

            Uri url = new Uri(AdminUrl + "/file/" + filename);
            PdfReader reader = new PdfReader(url);
            PdfStamper stamper = new PdfStamper(reader, stream);    //read pdf stream 
            var imgHeight = img.Height;
            var imgWidth = img.Width;  //调整图片大小，使之适合A4
            if (img.Height > iTextSharp.text.PageSize.A4.Height)
            {
                imgHeight = Convert.ToInt32(iTextSharp.text.PageSize.A4.Height);
            }
            if (img.Width > iTextSharp.text.PageSize.A4.Width)
            {
                imgWidth = Convert.ToInt32(iTextSharp.text.PageSize.A4.Width);
            }
            img.ScaleToFit(imgWidth, imgHeight);
            img.Alignment = Image.UNDERLYING;
            if (decltype == "11" || decltype == "12" || decltype == "17" || decltype == "18")
            {
                img.SetAbsolutePosition(-10, -6);
            }
            else
            {
                img.SetAbsolutePosition(0, 0);
            }
            int totalPage = reader.NumberOfPages;
            for (int current = 1; current <= totalPage; current++)
            {
                var canvas = stamper.GetUnderContent(current);
                var page = stamper.GetImportedPage(reader, current);
                canvas.AddImage(img);
            }
            stamper.Close();
            reader.Close();
            return "http://localhost:8012/Declare/" + outname + ".pdf";
        }

        public void UpdatePrintInfo(string tablename, string code)
        {
            string sql = @"update " + tablename + " set PRINTNUM = PRINTNUM+1,ISPRINT = 1,PRINTTIME=sysdate where CODE='" + code + "'";
            DBMgr.ExecuteNonQuery(sql);
        }

        //pdf文件合并
        protected void MergePDFFiles(IList<string> fileList, string outMergeFile)
        {
            PdfReader reader;
            Document document = new Document();  // Define the output place, and add the document to the stream          
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outMergeFile, FileMode.Create));
            document.Open();// Open document         
            // writer.AddJavaScript("this.print(true);", true);
            PdfContentByte cb = writer.DirectContent; // PDF ContentByte         
            PdfImportedPage newPage;   // PDF import page  
            for (int i = 0; i < fileList.Count; i++)
            {
                Uri url = new Uri(fileList[i]);
                reader = new PdfReader(url);
                int iPageNum = reader.NumberOfPages;
                for (int j = 1; j <= iPageNum; j++)
                {
                    document.NewPage();
                    newPage = writer.GetImportedPage(reader, j);
                    cb.AddTemplate(newPage, 0, 0);
                }
            }
            document.Close();
        }

        #endregion

        #region MutiPrint 批量打印

        //报关单列表页--选择多个报关单进行批量打印时，需要统计根据预制报关单号或者预制报检单号统计出 【61报关单、63报关单提前、62报检单、121核准单】的文件数量
        public string GetFileNums()
        {
            string data = Request["data"];//父页面选取的预制报关单或者报检单
            string source = Request["source"];
            string bgdnum = "";
            string bgdtqnum = "";
            string bjdnum = "";
            string bjhzdnum = "";
            string sql = "";
            string codes = "";
            JArray ja = JArray.Parse(data);
            for (int j = 0; j < ja.Count(); j++)
            {
                if (j == ja.Count - 1)
                {
                    codes += ja[j].Value<string>("CODE");
                }
                else
                {
                    codes += ja[j].Value<string>("CODE") + ",";
                }
            }
            if (source == "declare")
            {
                sql = "select count(1) RECS from list_attachment t where instr('" + codes + "',t.declcode)>0 and t.filetype=61";
                bgdnum = DBMgr.GetDataTable(sql).Rows[0]["RECS"] + "";
                sql = "select count(1) RECS from list_attachment t where instr('" + codes + "',t.declcode)>0 and t.filetype=63";
                bgdtqnum = DBMgr.GetDataTable(sql).Rows[0]["RECS"] + "";
            }
            if (source == "inspect")
            {
                sql = "select count(1) RECS from list_attachment t where instr('" + codes + "',t.inspcode)>0 and t.filetype=62";
                bjdnum = DBMgr.GetDataTable(sql).Rows[0]["RECS"] + "";
                sql = "select count(1) RECS from list_attachment t where instr('" + codes + "',t.inspcode)>0 and t.filetype=121";
                bjhzdnum = DBMgr.GetDataTable(sql).Rows[0]["RECS"] + "";
            }
            return "{bgdnum:'" + bgdnum + "',bgdtqnum:'" + bgdtqnum + "',bjdnum:'" + bjdnum + "',bjhzdnum:'" + bjhzdnum + "'}";
        }

        /*报关报检单列表页面批量打印by panhuaguo 2016-04-15*/
        public string batchprint()
        {
            string printmode = Request["printmode"];//报关单打印模式  分为标准打印，套打打印
            string filetype = Request["filetype"];//所选择的参与批量打印的文档类型44,61,62
            string printtmp = Request["printtmp"];//模型又分很多联,有可能选择多联一起打印
            JArray jarray = (JArray)JsonConvert.DeserializeObject(Request["data"]);//报关单或者报检单列表选择的记录数据

            IList<string> filelist = new List<string>();
            string sql = "";
            DataTable dt = null;
            foreach (JObject json in jarray)     //通过选择的规则进行多文件合并，展示后自动打印 
            {
                if (filetype == "61")//报关单
                {
                    sql = "select t.* from List_Attachment t where t.DECLCODE='" + json.Value<string>("CODE") + "' and upper(t.filesuffix)='PDF' and t.filetype=61";
                    dt = DBMgr.GetDataTable(sql);//一个报关单号可能对应多个报关文件
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (printmode == "biaozhun")//如果是标准打印
                        {

                            string[] tmp_array = printtmp.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);//用户可能会在前端选择多个打印模板
                            foreach (string tmpname in tmp_array)
                            {
                                string outpath = AddBackground(dr["FILENAME"] + "", tmpname, json.Value<string>("BUSITYPE"), json.Value<string>("DECLTYPE"));
                                filelist.Add(outpath);
                            }
                        }
                        else  //如果是在带格式的纸张上输出，则不需要追加背景
                        {
                            filelist.Add(AdminUrl + "/file/" + dr["FILENAME"]);
                        }
                    }
                    if (dt.Rows.Count > 0)
                    {
                        UpdatePrintInfo("list_declaration", json.Value<string>("CODE"));
                    }
                }
                if (filetype == "63")
                {
                    sql = "select * from List_Attachment where DECLCODE='" + json.Value<string>("CODE") + "' and upper(filesuffix)='PDF' and filetype=63";
                    dt = DBMgr.GetDataTable(sql);
                    foreach (DataRow dr in dt.Rows)//报关单提前只能在套打模式下打印 故不需要给PDF追加背景
                    {
                        filelist.Add(AdminUrl + "/file/" + dr["FILENAME"]);
                    }
                    if (dt.Rows.Count > 0)
                    {
                        UpdatePrintInfo("list_declaration", json.Value<string>("CODE") + "");
                    }
                }
                if (filetype == "62" || filetype == "121")
                {
                    sql = "select * from List_Attachment where INSPCODE='" + json.Value<string>("CODE") + "' and upper(filesuffix)='PDF' and filetype=" + filetype;
                    dt = DBMgr.GetDataTable(sql);
                    foreach (DataRow dr in dt.Rows)//报关单提前只能在套打模式下打印 故不需要给PDF追加背景
                    {
                        filelist.Add(AdminUrl + "/file/" + dr["FILENAME"]);
                    }
                    if (dt.Rows.Count > 0)
                    {
                        UpdatePrintInfo("list_inspection", json.Value<string>("CODE") + "");
                    }
                }
            }
            string output = "";
            if (filelist.Count > 0)
            {
                output = Guid.NewGuid() + ".pdf";
                MergePDFFiles(filelist, Server.MapPath("~/Declare/") + output);
            }
            return "{newname:'" + output + "'}";
        }

        #endregion

        /*报检单管理 列表页展示*/
        public string LoadInspectionList()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string where = "";
            string role = Request["role"]; string busitypeid = Request["busitypeid"];

            if (!string.IsNullOrEmpty(Request["VALUE1"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION1"])
                {
                    case "BUSIUNITCODE"://经营单位
                        where += " and lo.BUSIUNITCODE ='" + Request["VALUE1"] + "' ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE2"]))//判断查询条件2是否有值
            {
                switch (Request["CONDITION2"])
                {
                    case "CUSNO"://客户编号
                        where += " and instr(lo.CUSNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    //case "BLNO"://提运单号
                    //    where += " and instr(lp.LADINGNO,'" + Request["VALUE2"] + "')>0 ";
                    //    break;
                    case "ORDERCODE"://订单编号
                        where += " and instr(li.ORDERCODE,'" + Request["VALUE2"] + "')>0 ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE3"]))//判断查询条件3是否有值
            {
                switch (Request["CONDITION3"])
                {
                    case "DYBZ"://打印标志
                        where += " and li.ISPRINT='" + Request["VALUE3"] + "' ";
                        break;
                }
            }
            switch (Request["CONDITION4"])
            {
                case "SUBMITTIME"://订单委托日期 
                    if (!string.IsNullOrEmpty(Request["VALUE4_1"]))//如果开始时间有值
                    {
                        where += " and lo.SUBMITTIME>=to_date('" + Request["VALUE4_1"] + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE4_2"]))//如果结束时间有值
                    {
                        where += " and lo.SUBMITTIME<=to_date('" + Request["VALUE4_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    break;
                //case "REPTIME"://申报时间
                //    if (!string.IsNullOrEmpty(Request["VALUE4_1"]))//如果开始时间有值
                //    {
                //        where += " and li.REPTIME>=to_date('" + Request["VALUE4_1"] + "','yyyy-mm-dd hh24:mi:ss') ";
                //    }
                //    if (!string.IsNullOrEmpty(Request["VALUE4_2"]))//如果结束时间有值
                //    {
                //        where += " and li.REPTIME<=to_date('" + Request["VALUE4_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                //    }
                //    break;
            }
            if (role == "supplier") //如果是现场服务
            {
                where += @" and cus.SCENEINSPECTID ='" + json_user.Value<string>("CUSTOMERID") + "' ";
            }
            if (role == "enterprise") //如果是企业服务
            {
                where += @" and lo.BUSIUNITCODE IN (
                                    SELECT b.incode QUANCODE FROM USER_RENAME_COMPANY a 
                                    left join BASE_COMPANY b on a.companyid = b.id 
                                    where b.incode is not null and a.companyenname is not null and a.customerid = " + json_user.Value<string>("CUSTOMERID") + ") ";
            }
            if (role == "customer")
            {
                where += @" and lo.customercode ='" + json_user.Value<string>("CUSTOMERCODE") + "' ";
            }
            //申报库别基础表sys_REPORTLIBRARY
            /*string sql = @"SELECT li.ID,li.CODE,li.PREINSPCODE, li.APPROVALCODE,li.INSPECTIONCODE,li.REPFINISHTIME
                            ,li.BUSITYPE,li.ORDERCODE,lo.CUSNO,li.ISPRINT,li.ISFORCELAW,li.GOODSNUM,li.CUSTOMSSTATUS
                            ,lp.UPCNNAME,lp.INSPTYPE,lp.ENTRYPORT,lp.TRANSTOOL,lp.LADINGNO,lp.TOTALNO,lp.TRADE,lp.CONTRACTNO,lp.FOBPORT,lp.FOBPORTNAME
                            ,lo.WOODPACKINGID,lo.INSPUNITNAME
                            ,bi.NAME PACKAGETYPENAME,sr.NAME DECLTYPENAME,cus.SCENEINSPECTID    
                         FROM list_inspection li 
                         LEFT JOIN list_preinspection lp ON li.preinspcode = lp.preinspcode
                         LEFT JOIN list_order lo ON li.ordercode = lo.code 
                         left join sys_customer cus on ort.customercode=cus.code 
                         left join base_insppackage bi on lp.packagetype=bi.code
                         left join sys_REPORTLIBRARY sr on sr.code=lp.DECLTYPE 
                         WHERE li.STATUS >=103 and INSTR('" + busitypeid + "',li.busitype)>0 " + where;*/

            /* 报检单 暂时没新增的字段
,li.REPFINISHTIME,li.ISFORCELAW
,lp.UPCNNAME,lp.INSPTYPE,lp.ENTRYPORT,lp.TRANSTOOL,lp.LADINGNO,lp.TOTALNO,lp.TRADE,lp.CONTRACTNO,lp.FOBPORT,lp.FOBPORTNAME,lp.PACKAGETYPE,lp.DECLTYPE 
*/
            string sql = @"SELECT li.ID,li.CODE,li.ORDERCODE,li.INSPSTATUS,li.ISPRINT, li.APPROVALCODE,li.INSPECTIONCODE
                              ,lo.WOODPACKINGID,lo.INSPUNITNAME,lo.BUSITYPE,lo.GOODSNUM,lo.CUSNO
                              ,cus.SCENEINSPECTID    
                            FROM list_inspection li 
                                 LEFT JOIN list_order lo ON li.ordercode = lo.code 
                                 left join sys_customer cus on lo.customercode=cus.code 
                            WHERE li.STATUS >=103  and INSTR('" + busitypeid + "',lo.busitype)>0 " + where;
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "CREATETIME", "DESC"));//DBMgr.GetDataTable(GetPageSql(sql, "REPFINISHTIME", "DESC"));    //排序字段修改
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string LoadInspectReceipt()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string sql = @"select l.TIMES ,l.STATUS from list_inspreceiptstatus l  
                         where l.code = '" + Request["bjdcode"] + "' order by l.times asc ";
            DataTable dt = DBMgr.GetDataTable(sql);
            string json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + "}";
        }

        #region ERP 导入

        //验证该账号是否开通了ERP接口
        public string validateErpCompetence()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            int customerId = !string.IsNullOrEmpty(json_user.Value<string>("CUSTOMERID")) ? Convert.ToInt32(json_user.Value<string>("CUSTOMERID")) : 0;
            string erpCode = "";
            if (customerId > 0)
            {
                string erpCodeSql = "select INTERFACECODE from sys_customer where id = " + customerId;
                DataTable erpCodeDt = DBMgr.GetDataTable(erpCodeSql);
                if (erpCodeDt.Rows[0]["INTERFACECODE"] != null)
                {
                    erpCode = (erpCodeDt.Rows[0]["INTERFACECODE"]).ToString();
                }
            }
            return erpCode;
        }

        //非国内业务根据业务编号(客户自己的编号)从客户ERP获取订单信息  by panhuaguo 2016-08-19  封装出来 不用到每个业务的控制器里面去调取
        public string GetOrderFromErp()
        {
            string operationid = Request["operationid"];
            string busitype = Request["busitype"];
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            DataTable dt;
            string sql = "";
            switch (busitype)
            {
                case "10":
                    sql = @"select '10' BUSITYPE,d.CUSTOM_CODE CUSTOMDISTRICTCODE,'' CUSTOMDISTRICTNAME,d.SB_CUSTOM_CODE PORTCODE ,'' PORTNAME , 
                    '' BUSIUNITCODE ,'' BUSIUNITNAME ,'' BUSISHORTCODE ,'' BUSISHORTNAME,'{0}' CUSTOMERCODE,'{1}' CUSTOMERNAME,'{0}' CLEARUNIT,                       
                    '{1}' CLEARUNITNAME,d.MBL TOTALNO ,d.HBL DIVIDENO  ,d.PIECES GOODSNUM ,d.WEIGHT GOODSGW ,d.OPERATION_ID CUSNO,  
                    (select CHINNAME from Crm_Enterprise c where c.enterpriseid=d.Customer_code) CHINNAME,
                    (select case when forekeyin_no = '0' then '' else forekeyin_no end from IFM_AIRI_PORT_CHANGE where hbl=d.hbl and mbl = d.mbl) TURNPRENO ,
                     d.customer_invoice_no CONTRACTNO from OPS_AIRE_HEAD d  where d.operation_id = '{2}'";
                    break;
                case "11"://空进
                    sql = @"select '11' BUSITYPE,d.Cusom_code CUSTOMDISTRICTCODE,'' BUSIUNITCODE,'' BUSIUNITNAME,'' BUSISHORTCODE,'' BUSISHORTNAME,
                          '{0}' CUSTOMERCODE,'{1}' CUSTOMERNAME,'{0}' CLEARUNIT,'{1}' CLEARUNITNAME,'001' BUSIKIND,d.MBL TOTALNO,d.HBL DIVIDENO,
                          d.PIECES GOODSNUM,d.WEIGHT GOODSGW,'1' ORDERWAY,d.OPERATION_ID CUSNO,
                          (select CHINNAME from Crm_Enterprise c where c.enterpriseid=d.Customer_code) CHINNAME,                       
                          (select case when forekeyin_no = '0' then '' else forekeyin_no end from IFM_AIRI_PORT_CHANGE where hbl=d.hbl and mbl = d.mbl) TURNPRENO,                          
                          d.customer_invoice_no CONTRACTNO from OPS_AIRI_ASN d where d.operation_id = '{2}'";
                    break;
                case "20"://海出
                    sql = @"select  '20' BUSITYPE,d.SB_CUSTOM CUSTOMDISTRICTCODE,'' CUSTOMDISTRICTNAME,'' BUSIUNITCODE,'' BUSIUNITNAME, 
                    '' BUSISHORTCODE,'' BUSISHORTNAME,'{0}' CUSTOMERCODE,'{1}' CUSTOMERNAME,'{0}' CLEARUNIT,'{1}' CLEARUNITNAME, 
                    '001' BUSIKIND,d.FIRST_SHIP SHIPNAME  ,d.FIRST_VOYAGES FILGHTNO,d.BOOK_BILL_NUMBER SECONDLADINGBILLNO,
                    d.PIECES_TOTAL GOODSNUM,d.WEIGHT_TOTAL GOODSGW  ,d.PACK_CODE PACKKIND  ,'1' ORDERWAY ,d.OPERATION_ID CUSNO,
                    d.CUSTOMER_FP_NO CONTRACTNO ,(select CHINNAME from Crm_Enterprise c where c.enterpriseid=d.Customer_code) CHINNAME     
                    from OPS_SEAO_HEAD d  where d.OPERATION_ID = '{2}'";
                    break;
                case "21":
                    sql = @"select  '21' BUSITYPE,d.CUSOM_CODE CUSTOMDISTRICTCODE,'' CUSTOMDISTRICTNAME,'' BUSIUNITCODE,'' BUSIUNITNAME,
                          '' BUSISHORTCODE,'' BUSISHORTNAME,'{0}' CUSTOMERCODE,'{1}' CUSTOMERNAME,'{0}' CLEARUNIT,'{1}' CLEARUNITNAME,                         
                          d.SECOND_SHIP SHIPNAME,d.SECOND_VOYAGES FILGHTNO,d.FIRST_BILL FIRSTLADINGBILLNO,d.main_no MAIN_NO, 
                          d.SECOND_BILL SECONDLADINGBILLNO ,d.PIECES_TOTAL GOODSNUM,d.WEIGHT_TOTAL GOODSGW,d.PACK_CODE PACKKIND,d.OPERATION_ID CUSNO,
                          (select CHINNAME from Crm_Enterprise c where c.enterpriseid=d.Customer_code) CHINNAME
                          from OPS_SEAI_ASN_HEAD d  where d.OPERATION_ID = '{2}'";
                    break;
                case "50":
                case "51":
                    sql = @"select d.DECLARE_CUSTOM CUSTOMDISTRICTCODE,CUSTOM_CODE PORTCODE  ,CONSIGN_CODE BUSIUNITCODE,PIECES GOODSNUM,  
                    WEIGHT GOODSGW,PACK_TYPE  PACKKIND,'' BUSITYPE,'1' ORDERWAY ,OPERATION_ID CUSNO ,INV_NO CONTRACTNO ,'' CUSTOMDISTRICTNAME,'' BUSIUNITNAME,  
                    '' BUSISHORTCODE ,'' BUSISHORTNAME ,'{0}' CUSTOMERCODE ,'{0}' CLEARUNIT,'003' BUSIKIND ,'{1}' CUSTOMERNAME,'{1}' CLEARUNITNAME,DECLARE_MODE,     
                    '' REPWAYID,(select CHINNAME from Crm_Enterprise c where c.enterpriseid=d.CONSIGN_CODE) CHINNAME  from ops_ts_head d   where d.operation_id = '{2}'";
                    break;
            }
            sql = string.Format(sql, json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), operationid);
            DataSet ds = DBMgrERP.GetDataSet(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];//通过ERP的经营单位中文全名到本系统数据库进行匹配,加载本地经营单位                
                sql = "select id,INCODE,NAME from base_company where translate(name,'（）','()') = '" + dt.Rows[0]["CHINNAME"].ToString().Replace('（', '(').Replace('）', ')') + "'";
                DataTable dtt = DBMgrBase.GetDataTable(sql);
                if (dtt.Rows.Count > 0)
                {
                    dt.Rows[0]["BUSIUNITCODE"] = dtt.Rows[0]["INCODE"] + "";
                    dt.Rows[0]["BUSIUNITNAME"] = dtt.Rows[0]["NAME"] + "";
                }
                if (busitype == "21") //有主拼单号，根据主拼单号（main_no）查询该主拼单的二程提单号写入到海关提单号(二程提单号)中
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["MAIN_NO"] + ""))
                    {
                        sql = "select SECOND_BILL from OPS_SEAI_ASN_HEAD where OPERATION_ID = '" + dt.Rows[0]["MAIN_NO"] + "'";
                        DataSet ds_t = DBMgrERP.GetDataSet(sql);
                        dt.Rows[0]["SECONDLADINGBILLNO"] = ds_t.Tables[0].Rows[0]["SECOND_BILL"] + "";
                    }
                }
                //特殊业务时需要根据根据报关方式 确定申报方式和业务类型  
                if (busitype == "50" || busitype == "51")
                {
                    if (dt.Rows[0]["DECLARE_MODE"] + "" == "PE")
                    {
                        dt.Rows[0]["REPWAYID"] = "008";
                        dt.Rows[0]["BUSITYPE"] = "50";
                    }
                    if (dt.Rows[0]["DECLARE_MODE"] + "" == "CE")
                    {
                        dt.Rows[0]["REPWAYID"] = "007";
                        dt.Rows[0]["BUSITYPE"] = "50";
                    }
                    if (dt.Rows[0]["DECLARE_MODE"] + "" == "PI")
                    {
                        dt.Rows[0]["REPWAYID"] = "002";
                        dt.Rows[0]["BUSITYPE"] = "51";
                    }
                    if (dt.Rows[0]["DECLARE_MODE"] + "" == "CI")
                    {
                        dt.Rows[0]["REPWAYID"] = "001";
                        dt.Rows[0]["BUSITYPE"] = "51";
                    }
                }
            }
            else
            {
                return "{success:false}";
            }
            string json = JsonConvert.SerializeObject(dt);
            json = json.TrimStart('[').TrimEnd(']');
            return "{success:true,data:" + json + "}";
        }

        //获取ERP订单对应的文件到本地
        public string getErpfile(string id)
        {
            System.Uri Uri = new Uri("ftp://" + ConfigurationManager.AppSettings["ERPServer"] + ":" + ConfigurationManager.AppSettings["ERPPortNO"]);
            string UserName = ConfigurationManager.AppSettings["ERPUserName"];
            string Password = ConfigurationManager.AppSettings["ERPPassword"];
            FMService.FMService1SoapClient fm = new FMService.FMService1SoapClient();

            string type = Request["type"];
            DataSet ds = null;

            if (string.IsNullOrEmpty(type) || type == "qt")
            {
                ds = fm.ReturnMainFile(id);
            }
            else
            {
                string landTable = "";
                if (type == "landIn")
                {
                    landTable = "OPS_LI_HEAD";
                }
                else if (type == "landOut")
                {
                    landTable = "OPS_LE_HEAD";
                }
                string landSql = "select ol.operation_id from " + landTable + " ol where ol.tracking_no like '%" + id + "%'";
                DataSet landDs = DBMgrERP.GetDataSet(landSql);
                DataTable landDt = new DataTable();
                landDt = landDs.Tables[0];
                string landJson = landDt.Rows[0]["OPERATION_ID"].ToString();
                ds = fm.ReturnMainFile(landJson);
            }

            DataTable dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                FtpHelper ftp = new FtpHelper(Uri, UserName, Password);
                string remoteFilePath = (dr["PATH"] + "").Replace(@"http://172.20.70.98:7003/Document", "");
                string localFilePath = Server.MapPath("~/") + "FileUpload/file/" + remoteFilePath.Substring(remoteFilePath.LastIndexOf("CustomsFile") + 11);
                bool d = ftp.DownloadFile(remoteFilePath, localFilePath);
            }
            var json = JsonConvert.SerializeObject(dt);
            return "{rows:" + json + "}";
        }

        #endregion

    }
}
