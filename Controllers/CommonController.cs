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
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;

namespace MvcPlatform.Controllers
{
    [Authorize]
    public class CommonController : Controller
    {
        int totalProperty = 0;
        string AdminUrl = ConfigurationManager.AppSettings["AdminUrl"];

        public ActionResult DeclareList()//报关单管理
        {

            switch (Request["busitypeid"].ToString())
            {
                case "11":
                    ViewBag.navigator = "空进订单>>报关单管理";
                    break;
                case "10":
                    ViewBag.navigator = "空出订单>>报关单管理";
                    break;
                case "21":
                    ViewBag.navigator = "海进订单>>报关单管理";
                    break;
                case "20":
                    ViewBag.navigator = "海出订单>>报关单管理";
                    break;
                case "31":
                    ViewBag.navigator = "陆进订单>>报关单管理";
                    break;
                case "30":
                    ViewBag.navigator = "陆出订单>>报关单管理";
                    break;
                case "40-41":
                    ViewBag.navigator = "国内订单>>报关单管理";
                    break;
                case "50-51":
                    ViewBag.navigator = "特殊区域订单>>报关单管理";
                    break;
                default:
                    break;
            }
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult FileConsult()//文件调阅
        {
            return View();
        }
        public ActionResult FileConsult_E()//文件调阅
        {
            return View();
        }

        public ActionResult MultiPrint()//批量打印
        {
            return View();
        }
        public ActionResult OrderView()
        {
            return View();
        }
        public ActionResult OrderTrack()
        {
            return View();
        }
        public ActionResult InspectList()//报检单管理
        {
            switch (Request["busitypeid"].ToString())
            {
                case "11":
                    ViewBag.navigator = "空进订单>>报检单管理";
                    break;
                case "10":
                    ViewBag.navigator = "空出订单>>报检单管理";
                    break;
                case "21":
                    ViewBag.navigator = "海进订单>>报检单管理";
                    break;
                case "20":
                    ViewBag.navigator = "海出订单>>报检单管理";
                    break;
                case "31":
                    ViewBag.navigator = "陆进订单>>报检单管理";
                    break;
                case "30":
                    ViewBag.navigator = "陆出订单>>报检单管理";
                    break;
                case "40-41":
                    ViewBag.navigator = "国内订单>>报检单管理";
                    break;
                case "50-51":
                    ViewBag.navigator = "特殊区域订单>>报检单管理";
                    break;
                default:
                    break;
            }
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult BatchMaintain()
        {
            return View();
        }
        public ActionResult DeclareList_Enterprise()
        {
            ViewBag.navigator = "通关管理>>通关信息";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
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
            string where = QueryCondition();

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string sql = @"select * from LIST_ORDER a left join LIST_GOOD_TRACK b on a.CODE=b.ORDERCODE where instr('" + Request["busitypeid"] + "',BUSITYPE)>0 and customercode='" + json_user.Value<string>("CUSTOMERCODE") + "' " + where;
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "createtime", "desc"));
            var json = JsonConvert.SerializeObject(dt, iso);

            var json_senior = "[]";
            if (!string.IsNullOrEmpty(Request["seniorsearch"] + ""))
            {
                DataTable dtall = DBMgr.GetDataTable(sql);
                DataTable dt_senior = new DataTable();
                dt_senior.Columns.Add("DIVIDENO", typeof(string)); dt_senior.Columns.Add("ISYN", typeof(int));

                DataRow[] dr; DataRow dr_n;
                string[] seniorarray = Request["seniorsearch"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < seniorarray.Length; i++)
                {
                    dr = dtall.Select("DIVIDENO='" + seniorarray[i] + "'");//"DIVIDENO like '%" + seniorarray[i] + "%'"
                    dr_n = dt_senior.NewRow();
                    dr_n["DIVIDENO"] = seniorarray[i]; dr_n["ISYN"] = dr.Length > 0 ? 1 : 0;
                    dt_senior.Rows.Add(dr_n);
                }
                json_senior = JsonConvert.SerializeObject(dt_senior, iso);
            }

            return "{rows:" + json + ",total:" + totalProperty + ",json_senior:" + json_senior + "}";
        }

        public string QueryCondition()
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
                        //where += "instr(DIVIDENO,'" + seniorarray[i] + "')>0 or ";
                        where += "DIVIDENO='" + seniorarray[i] + "' or ";
                    }
                    else
                    {
                        //where += "instr(DIVIDENO,'" + seniorarray[i] + "')>0)";
                        where += "DIVIDENO='" + seniorarray[i] + "')";
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
                    case "SECONDLADINGBILLNO"://海关提运单号
                        where += " and instr(SECONDLADINGBILLNO,'" + Request["VALUE2"].Trim() + "')>0  ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE3"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION3"])
                {
                    case "bgzt":
                        if ((Request["VALUE3"] + "") == "草稿")  //草稿=草稿
                        {
                            where += " and DECLSTATUS=0 ";
                        }
                        if ((Request["VALUE3"] + "") == "已委托")  //已委托=已委托
                        {
                            where += " and DECLSTATUS=10 ";
                        }
                        if ((Request["VALUE3"] + "") == "申报中")  //申报中=申报中
                        {
                            where += " and DECLSTATUS=100 ";
                        }
                        if ((Request["VALUE3"] + "") == "申报完结")  //申报完结=申报完结
                        {
                            where += " and DECLSTATUS=130 ";
                        }
                        if ((Request["VALUE3"] + "") == "未完结")  //未完结
                        {
                            where += " and DECLSTATUS<130 ";
                        }
                        break;
                    case "bjzt":
                        if ((Request["VALUE3"] + "") == "草稿")  //草稿=草稿
                        {
                            where += " and INSPSTATUS=0 ";
                        }
                        if ((Request["VALUE3"] + "") == "已委托")  //已委托=已委托
                        {
                            where += " and INSPSTATUS=10 ";
                        }
                        if ((Request["VALUE3"] + "") == "申报中")  //申报中=申报中
                        {
                            where += " and INSPSTATUS=100 ";
                        }
                        if ((Request["VALUE3"] + "") == "申报完结")  //申报完结=申报完结
                        {
                            where += " and INSPSTATUS=130 ";
                        }
                        if ((Request["VALUE3"] + "") == "未完结")  //未完结
                        {
                            where += " and INSPSTATUS<130 ";
                        }
                        break;
                    case "MANIFEST_STORAGE":
                        if ((Request["VALUE3"] + "") == "1")
                        {
                            where += " and b.MANIFEST_STORAGE=1 ";
                        }
                        if ((Request["VALUE3"] + "") == "0")
                        {
                            where += " and (b.MANIFEST_STORAGE=0 or b.MANIFEST_STORAGE is null) ";
                        }
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
                    case "SECONDLADINGBILLNO"://海关提运单号
                        where += " and instr(SECONDLADINGBILLNO,'" + Request["VALUE6"].Trim() + "')>0  ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE7"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION7"])
                {
                    case "bgzt":
                        if ((Request["VALUE7"] + "") == "草稿")  //草稿=草稿
                        {
                            where += " and DECLSTATUS=0 ";
                        }
                        if ((Request["VALUE7"] + "") == "已委托")  //已委托=已委托
                        {
                            where += " and DECLSTATUS=10 ";
                        }
                        if ((Request["VALUE7"] + "") == "申报中")  //申报中=申报中
                        {
                            where += " and DECLSTATUS=100 ";
                        }
                        if ((Request["VALUE7"] + "") == "申报完结")  //申报完结=申报完结
                        {
                            where += " and DECLSTATUS=130 ";
                        }
                        if ((Request["VALUE7"] + "") == "未完结")  //未完结
                        {
                            where += " and DECLSTATUS<130 ";
                        }
                        break;
                    case "bjzt":
                        if ((Request["VALUE7"] + "") == "草稿")  //草稿=草稿
                        {
                            where += " and INSPSTATUS=0 ";
                        }
                        if ((Request["VALUE7"] + "") == "已委托")  //已委托=已委托
                        {
                            where += " and INSPSTATUS=10 ";
                        }
                        if ((Request["VALUE7"] + "") == "申报中")  //申报中=申报中
                        {
                            where += " and INSPSTATUS=100 ";
                        }
                        if ((Request["VALUE7"] + "") == "申报完结")  //申报完结=申报完结
                        {
                            where += " and INSPSTATUS=130 ";
                        }
                        if ((Request["VALUE7"] + "") == "未完结")  //未完结
                        {
                            where += " and INSPSTATUS<130 ";
                        }
                        break;
                    case "MANIFEST_STORAGE":
                        if ((Request["VALUE7"] + "") == "1")  //草稿=草稿
                        {
                            where += " and b.MANIFEST_STORAGE=1 ";
                        }
                        if ((Request["VALUE7"] + "") == "0")  //已委托=已委托
                        {
                            where += " and (b.MANIFEST_STORAGE=0 or b.MANIFEST_STORAGE is null) ";
                        }
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
            if ((Request["OnlySelf"] + "").Trim() == "fa fa-check-square-o")
            {
                where += " and (CREATEUSERID = " + json_user.Value<string>("ID") + " or submitusername='" + json_user.Value<string>("REALNAME") + "') ";
            }

            where += " and ISINVALID=0 ";
            return where;
        }

        //基础资料 by heguiqin 2016-08-25
        public string Ini_Base_Data()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            IDatabase db = SeRedis.redis.GetDatabase();
            string sql = ""; string ParaType = Request["ParaType"];

            string json_jydw = "";//经营单位 :公用
            if (db.KeyExists("common_data:jydw"))
            {
                json_jydw = db.StringGet("common_data:jydw");
            }
            else
            {
                sql = "SELECT CODE,NAME||'('||CODE||')' NAME FROM BASE_COMPANY where CODE is not null and enabled=1";
                json_jydw = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:jydw", json_jydw);
            }

            if (ParaType == "recordinfo")//备案信息
            {
                //========================================备案信息============================================================================
                string json_recordid = "[]";//账册号
                sql = @"select id,code,code||'('||bookattribute||')' as name from sys_recordinfo where enabled=1 and busiunit= '" + json_user.Value<string>("CUSTOMERHSCODE") + "' order by id";
                json_recordid = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

                string json_unit = "[]";//单位
                if (db.KeyExists("common_data:unit"))
                {
                    json_unit = db.StringGet("common_data:unit");
                }
                else
                {
                    sql = @"select code,name,code||'('||name||')' as codename from base_declproductunit where enabled=1 order by code";
                    json_unit = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                    db.StringSet("common_data:unit", json_unit);
                }

                string json_customarea = "[]";//备案关区
                sql = @"select id,name,customarea,name||'('||customarea||')' as customareaname from cusdoc.base_year where customarea is not null and enabled=1 order by id";
                json_customarea = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

                //============================================================================================================================

                return "{jydw:" + json_jydw + ",recordid:" + json_recordid + ",unit:" + json_unit + ",customarea:" + json_customarea + "}";
            }

            string json_sbfs_all = "[]";//申报关区 进口口岸 
            if (db.KeyExists("common_data:sbfs_all"))
            {
                json_sbfs_all = db.StringGet("common_data:sbfs_all");
            }
            else
            {
                sql = "select CODE,NAME||'('||CODE||')' NAME from SYS_REPWAY where Enabled=1 ORDER BY CODE";
                json_sbfs_all = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:sbfs_all", json_sbfs_all);
            }

            #region 申报方式
            string json_sbfs = "[]";//申报方式
            string busitype = Request["busitype"];
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
            #endregion

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
            //报检监管方式（贸易方式）
            string json_inspmyfs = "[]";//贸易方式 
            if (db.KeyExists("common_data:inspmyfs"))
            {
                json_inspmyfs = db.StringGet("common_data:inspmyfs");
            }
            else
            {
                sql = @"select ID,CODE,NAME||'('||CODE||')' NAME from BASE_TRADEWAY WHERE enabled=1";
                json_inspmyfs = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:inspmyfs", json_inspmyfs);
            }
            //单证服务单位
            string json_dzfwdw = "[]";
            if (db.KeyExists("common_data:dzfwdw"))
            {
                json_dzfwdw = db.StringGet("common_data:dzfwdw");
            }
            else
            {
                sql = @"select * from sys_customer where DOCSERVICECOMPANY=1";
                json_dzfwdw = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
                db.StringSet("common_data:dzfwdw", json_dzfwdw);
            }            

            return "{jydw:" + json_jydw + ",sbfs_all:" + json_sbfs_all + ",sbfs:" + json_sbfs + ",sbgq:" + json_sbgq + ",bgfs:" + json_bgfs + ",bzzl:" + json_bzzl
                + ",myfs:" + json_myfs + ",containertype:" + json_containertype + ",containersize:" + json_containersize + ",truckno:" + json_truckno
                + ",relacontainer:" + json_relacontainer + ",mzbz:" + json_mzbz + ",jylb:" + json_jylb + ",json_sbkb:" + json_sbkb
                + ",inspbzzl:" + json_inspbzzl + ",adminurl:'" + AdminUrl + "',curuser:" + Extension.Get_UserInfo(HttpContext.User.Identity.Name)
                + ",dzfwdw:" + json_dzfwdw + ",inspmyfs:" + json_inspmyfs + "}";
        }

        /*保存查询条件设置 by panhuaguo 2016-01-17*/
        public string SaveQuerySetting()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string formdata = Request["formdata"];
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            //首先判断该用户先前有没有设置查询条件
            string sql = "select * from CONFIG_QUERYSETTING where UserId='" + json_user.Value<string>("ID") + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                sql = @"update CONFIG_QUERYSETTING set CONDITION1='{0}',CONDITION2='{1}',CONDITION3='{2}',CONDITION4='{3}',CONDITION5='{4}'
                        ,CONDITION6='{5}',CONDITION7='{6}',CONDITION8='{7}',updatetime=sysdate where UserId = '{8}'";
                sql = string.Format(sql, json.Value<string>("CONDITION1"), json.Value<string>("CONDITION2"), json.Value<string>("CONDITION3"), json.Value<string>("CONDITION4")
                    , json.Value<string>("CONDITION5"), json.Value<string>("CONDITION6"), json.Value<string>("CONDITION7"), json.Value<string>("CONDITION8"), json_user.Value<string>("ID"));
            }
            else
            {
                sql = @"insert into CONFIG_QUERYSETTING(ID,USERNAME,USERID,CONDITION1,CONDITION2,CONDITION3,CONDITION4,CONDITION5,CONDITION6,CONDITION7,CONDITION8,updatetime)
                        values (CONFIG_QUERYSETTING_ID.Nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',sysdate)";
                sql = string.Format(sql
                        , json_user.Value<string>("NAME"), json_user.Value<string>("ID"), json.Value<string>("CONDITION1"), json.Value<string>("CONDITION2"), json.Value<string>("CONDITION3")
                        , json.Value<string>("CONDITION4"), json.Value<string>("CONDITION5"), json.Value<string>("CONDITION6"), json.Value<string>("CONDITION7"), json.Value<string>("CONDITION8"));
            }
            int i = DBMgr.ExecuteNonQuery(sql);
            return i > 0 ? "{success:true}" : "{}";
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
            string result = "{success:false}";
            string sql = "select * from LIST_ORDER where code='" + Request["ordercode"] + "'";
            DataTable dt = DBMgr.GetDataTable(sql);

            bool bf = false;
            string status = dt.Rows[0]["STATUS"] + "" == "" ? "0" : dt.Rows[0]["STATUS"] + "";
            string declstatus = dt.Rows[0]["DECLSTATUS"] + "" == "" ? "0" : dt.Rows[0]["DECLSTATUS"] + "";
            string inspstatus = dt.Rows[0]["INSPSTATUS"] + "" == "" ? "0" : dt.Rows[0]["INSPSTATUS"] + "";

            if (status != "0" || declstatus != "0" || inspstatus != "0")
            {
                bf = true;
            }

            if (bf) { return result; }

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
            sql = "SELECT ID,CODE,NAME FROM BASE_COMPANY WHERE ENABLED=1 AND (CODE LIKE '%{0}%' OR NAME LIKE '%{0}%')";
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

        //备案信息 项号选择窗体弹出时加载基础库数据
        public string LoadItemno()
        {
            string sql = "select a.*,b.name as unitname from sys_recordinfo_detail a left join base_declproductunit b on a.unit=b.code WHERE a.recordinfoid={0} and a.itemnoattribute='料件' and a.commodityname LIKE '%{1}%'";
            sql = string.Format(sql, Request["RECORDINFOID"] + "", Request["NAME"] + "");
            var json = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(GetPageSqlBase(sql, "itemno", "asc")));
            return "{rows:" + json + ",total:" + totalProperty + "}";
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
            string sql = "";
            string result = "{}";
            if (string.IsNullOrEmpty(ordercode))//如果订单号为空、即新增的时候
            {
                if (string.IsNullOrEmpty(copyordercode))//如果不是复制新增
                {
                    //add 2016/10/9 by heguiqin
                    string WEIGHTCHECK = "";
                    sql = "select * from Sys_Customer where ID='" + json_user.Value<string>("CUSTOMERID") + "' AND instr(BUSITYPES,'" + Request["busitype"] + "')>0 AND ENABLED=1 AND ROWNUM=1";//根据客户ID，查询需自审，需重量确认
                    dt = DBMgrBase.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        WEIGHTCHECK = dt.Rows[0]["WEIGHTCHECK"].ToString();
                    }
                    //end
                    string formdata = "{STATUS:0,WEIGHTCHECK:'" + WEIGHTCHECK + "'}";
                    result = "{formdata:" + formdata + ",filedata:[]}";
                }
                else//如果是复制新增
                {
                    sql = @"select t.ENTRUSTTYPE,t.REPWAYID,t.CUSTOMAREACODE,t.DECLWAY,t.PORTCODE,t.REPUNITCODE,t.REPUNITNAME,t.INSPUNITCODE,t.INSPUNITNAME,
                    t.TRADEWAYCODES,'' CONTAINERTRUCK from LIST_ORDER t where t.CODE = '" + copyordercode + "' and rownum=1";
                    dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        string formdata = JsonConvert.SerializeObject(dt).TrimStart('[').TrimEnd(']');
                        result = "{formdata:" + formdata + ",filedata:[]}";
                    }
                }
            }
            else //如果订单号不为空
            {
                //订单基本信息 CONTAINERTRUCK 这个字段本身不属于list_order表,虚拟出来存储集装箱和报关车号记录,是个数组形式的字符串
                sql = @"select t.*,'' CONTAINERTRUCK from LIST_ORDER t where t.CODE = '" + Request["ordercode"] + "' and rownum=1";
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
            int status = dt.Rows[0]["STATUS"] + "" == "" ? 0 : Convert.ToInt32(dt.Rows[0]["STATUS"] + "");
            int declstatus = dt.Rows[0]["DECLSTATUS"] + "" == "" ? 0 : Convert.ToInt32(dt.Rows[0]["DECLSTATUS"] + "");
            int inspstatus = dt.Rows[0]["INSPSTATUS"] + "" == "" ? 0 : Convert.ToInt32(dt.Rows[0]["INSPSTATUS"] + "");

            string result = "";

            if (status != 10 || (status == 10 && (declstatus > 10 || inspstatus > 10)))//if (dt.Rows[0]["STATUS"] + "" != "10")
            {
                result = "{success:false}";
            }
            else
            {
                sql = "delete from list_times where code='" + ordercode + "' and status = '10'";//删除订单状态变更日志信息
                DBMgr.ExecuteNonQuery(sql);
                sql = @"update list_order set STATUS = '0'
                        ,DECLSTATUS=case when DECLSTATUS is null then null else '0' end
                        ,INSPSTATUS=case when INSPSTATUS is null then null else '0' end
                        ,SUBMITUSERNAME='',SUBMITTIME='' where code='" + ordercode + "'";
                DBMgr.ExecuteNonQuery(sql);
                result = "{success:true}";
            }
            return result;
        }

        /*报关单管理 列表页展示*/
        public string LoadDeclarationList()
        {
            string sql = QueryConditionDecl();
            DataTable dt = null;           
            try
            {
                dt = DBMgr.GetDataTable(GetPageSql(sql, "CREATETIME", "desc"));  
            }
            catch (Exception ex)
            {

            }
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string QueryConditionDecl()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string busitypeid = Request["busitypeid"];

            string where = "";
            string role = Request["role"];
            if (!string.IsNullOrEmpty(Request["VALUE1"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION1"])
                {
                    case "BUSIUNITCODE"://经营单位
                        where += " and lda.BUSIUNITCODE='" + Request["VALUE1"] + "' ";
                        break;
                    case "REPWAYNAME"://申报方式
                        where += " and ort.REPWAYID='" + Request["VALUE1"] + "' ";
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
                        where += " and instr(lda.BLNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "ORDERCODE"://订单编号
                        where += " and instr(det.ORDERCODE,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "DECLCARNO"://报关车号
                        where += " and instr(ort.DECLCARNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "TRANSNAME"://运输工具名称
                        where += " and (instr(lda.TRANSNAME,'" + Request["VALUE2"] + "')>0 or instr(lda.VOYAGENO,'" + Request["VALUE2"] + "')>0)";
                        break;
                    case "DECLNO"://报关单号
                        where += " and instr(lda.DECLARATIONCODE,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "CONTRACTNO"://合同协议号
                        where += " and instr(lda.CONTRACTNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "CONTRACTNOORDER"://合同发票号
                        where += " and instr(ort.CONTRACTNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "TOTALNO"://总单号
                        where += " and instr(ort.TOTALNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "DIVIDENO"://分单号
                        where += " and instr(ort.DIVIDENO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "SECONDLADINGBILLNO"://海关提单号
                        where += " and instr(ort.SECONDLADINGBILLNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "PRETRANSNAME"://船名、预录号
                        where += " and instr(det.TRANSNAME,'" + Request["VALUE2"] + "')>0 ";
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
                    case "HGZT"://海关状态
                        if (Request["VALUE3"] == "已结关") { where += " and det.CUSTOMSSTATUS='已结关' "; }
                        if (Request["VALUE3"] == "未结关") { where += " and det.CUSTOMSSTATUS!='已结关' "; }
                        break;
                    case "SGD"://删改单
                        where += " and det.modifyflag='" + Request["VALUE3"] + "' ";
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
                        where += " and lda.REPTIME>=to_date('" + Request["VALUE4_1"] + "','yyyy-mm-dd hh24:mi:ss') ";//REPSTARTTIME
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE4_2"]))//如果结束时间有值
                    {
                        where += " and lda.REPTIME<=to_date('" + Request["VALUE4_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";//REPSTARTTIME
                    }
                    break;
            }


            if (!string.IsNullOrEmpty(Request["VALUE5"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION5"])
                {
                    case "BUSIUNITCODE"://经营单位
                        where += " and lda.BUSIUNITCODE='" + Request["VALUE5"] + "' ";
                        break;
                    case "REPWAYNAME"://申报方式
                        where += " and ort.REPWAYID='" + Request["VALUE5"] + "' ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE6"]))//判断查询条件2是否有值
            {
                switch (Request["CONDITION6"])
                {
                    case "CUSNO"://客户编号
                        where += " and instr(ort.CUSNO,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "BLNO"://提运单号
                        where += " and instr(lda.BLNO,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "ORDERCODE"://订单编号
                        where += " and instr(det.ORDERCODE,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "DECLCARNO"://报关车号
                        where += " and instr(ort.DECLCARNO,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "TRANSNAME"://运输工具名称
                        where += " and (instr(lda.TRANSNAME,'" + Request["VALUE6"] + "')>0 or instr(lda.VOYAGENO,'" + Request["VALUE6"] + "')>0)";
                        break;
                    case "DECLNO"://报关单号
                        where += " and instr(lda.DECLARATIONCODE,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "CONTRACTNO"://合同协议号
                        where += " and instr(lda.CONTRACTNO,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "CONTRACTNOORDER"://合同发票号
                        where += " and instr(ort.CONTRACTNO,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "TOTALNO"://总单号
                        where += " and instr(ort.TOTALNO,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "DIVIDENO"://分单号
                        where += " and instr(ort.DIVIDENO,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "SECONDLADINGBILLNO"://海关提单号
                        where += " and instr(ort.SECONDLADINGBILLNO,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "PRETRANSNAME"://船名、预录号
                        where += " and instr(det.TRANSNAME,'" + Request["VALUE6"] + "')>0 ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE7"]))//判断查询条件3是否有值
            {
                switch (Request["CONDITION7"])
                {
                    case "DYBZ"://打印标志
                        where += " and det.ISPRINT='" + Request["VALUE7"] + "' ";
                        break;
                    case "HGZT"://海关状态
                        if (Request["VALUE7"] == "已结关") { where += " and det.CUSTOMSSTATUS='已结关' "; }
                        if (Request["VALUE7"] == "未结关") { where += " and det.CUSTOMSSTATUS!='已结关' "; }
                        break;
                    case "SGD"://删改单
                        where += " and det.modifyflag='" + Request["VALUE7"] + "' ";
                        break;
                }
            }
            switch (Request["CONDITION8"])
            {
                case "SUBMITTIME"://订单委托日期 
                    if (!string.IsNullOrEmpty(Request["VALUE8_1"]))//如果开始时间有值
                    {
                        where += " and ort.SUBMITTIME>=to_date('" + Request["VALUE8_1"] + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE8_2"]))//如果结束时间有值
                    {
                        where += " and ort.SUBMITTIME<=to_date('" + Request["VALUE8_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    break;
                case "REPTIME"://申报完成时间
                    if (!string.IsNullOrEmpty(Request["VALUE8_1"]))//如果开始时间有值
                    {
                        where += " and lda.REPTIME>=to_date('" + Request["VALUE8_1"] + "','yyyy-mm-dd hh24:mi:ss') ";//REPSTARTTIME
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE8_2"]))//如果结束时间有值
                    {
                        where += " and lda.REPTIME<=to_date('" + Request["VALUE8_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";//REPSTARTTIME
                    }
                    break;
            }


            if (role == "supplier") //如果是现场服务角色
            {
                where += @" and cus.SCENEDECLAREID ='" + json_user.Value<string>("CUSTOMERID") + "' ";
            }
            if (role == "customer")//如果角色是客户
            {
                where += @" and ort.customercode ='" + json_user.Value<string>("CUSTOMERCODE") + "' ";
            }

//            string sql = @"select det.ID,det.DECLARATIONCODE,det.CODE,ort.CUSTOMERNAME ,det.REPENDTIME REPFINISHTIME, det.CUSTOMSSTATUS ,   
//                           det.CONTRACTNO,det.GOODSNUM,det.GOODSNW,det.SHEETNUM,det.ORDERCODE,det.COSTARTTIME CREATEDATE,
//                           det.TRANSNAME,det.VOYAGENO,det.ISPRINT,
//                           det.BUSIUNITCODE, det.PORTCODE, det.BLNO, det.DECLTYPE, 
//                           ort.REPWAYID ,ort.REPWAYID REPWAYNAME,ort.DECLWAY ,ort.DECLWAY DECLWAYNAME,ort.TRADEWAYCODES ,
//                           ort.CUSNO ,ort.IETYPE,ort.ASSOCIATENO,ort.CORRESPONDNO,ort.BUSIUNITNAME,ort.BUSITYPE, 
//                           cus.SCENEDECLAREID,ort.CONTRACTNO CONTRACTNOORDER ,ort.CREATETIME                                                                      
//                           from list_declaration det 
//                                left join list_order ort on det.ordercode = ort.code 
//                                left join cusdoc.sys_customer cus on ort.customercode = cus.code 
//                                left join (
//                                      select ASSOCIATENO from list_order l inner join list_declaration i on l.code=i.ordercode 
//                                      where l.ASSOCIATENO is not null and l.isinvalid=0 and i.isinvalid=0 and (i.STATUS!=130 and i.STATUS!=110)          
//                                      ) a on ort.ASSOCIATENO=a.ASSOCIATENO 
//                                left join (select ordercode from list_declaration ld where ld.isinvalid=0 and ld.STATUS!=130 and ld.STATUS!=110) b on det.ordercode=b.ordercode
//                           where (det.STATUS=130 or det.STATUS=110) and det.isinvalid=0 and ort.isinvalid=0 and instr('" + busitypeid + "',ort.BUSITYPE)>0 " + where
//                      + " and a.ASSOCIATENO is null and b.ordercode is  null ";


            string sql = @"select det.ID,det.CODE,det.ORDERCODE, det.CUSTOMSSTATUS ,det.ISPRINT,det.SHEETNUM,det.modifyflag,det.transname as pretransname,
                              lda.declarationcode,to_char(lda.reptime,'yyyy-mm-dd') reptime,lda.contractno,lda.goodsnum,lda.goodsnw,lda.blno,
                              lda.transname,lda.voyageno,lda.portcode,
                              lda.trademethod,lda.declkind DECLWAY,lda.declkind DECLWAYNAME,  
                              ort.BUSITYPE,ort.CONTRACTNO CONTRACTNOORDER,ort.REPWAYID,ort.REPWAYID REPWAYNAME,ort.CUSNO,
                              ort.IETYPE,ort.ASSOCIATENO,ort.CORRESPONDNO,ort.customercode,ort.CUSTOMERNAME,ort.CREATETIME, 
                              ort.busiunitcode,ort.busiunitname,ort.totalno,ort.divideno,ort.secondladingbillno, 
                              cus.SCENEDECLAREID                                                             
                        from list_declaration det     
                            left join list_order ort on det.ordercode = ort.code 
                            left join cusdoc.sys_customer cus on ort.customercode = cus.code 
                            left join list_declaration_after lda on det.code=lda.code and lda.csid=1
                            left join (select ordercode from list_declaration ld where ld.isinvalid=0 and ld.STATUS!=130 and ld.STATUS!=110) a on det.ordercode=a.ordercode
                            left join (
                                  select ASSOCIATENO from list_order l inner join list_declaration i on l.code=i.ordercode 
                                  where l.ASSOCIATENO is not null and l.isinvalid=0 and i.isinvalid=0 and (i.STATUS!=130 and i.STATUS!=110)          
                                  ) b on ort.ASSOCIATENO=b.ASSOCIATENO     
                        where (det.STATUS=130 or det.STATUS=110) and det.isinvalid=0 and ort.isinvalid=0 
                               and instr('" + busitypeid + "',ort.BUSITYPE)>0 " + where +
                      @" and a.ordercode is null 
                               and b.ASSOCIATENO is null";
            return sql;

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

            string ordercode = Request["ordercode"];string role = Request["role"];
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
                    string entrusttypeid = dt.Rows[0]["ENTRUSTTYPE"] + "";
                    if (role == "enterprise")
                    {
                        switch (entrusttypeid)
                        {
                            //case "01":
                            //    result += "[{id:'declare',typename:'报关'}]";
                            //    break;
                            //case "02":
                            //    result += "[{id:'declare',typename:'报检'}]";
                            //    break;
                            //case "03":
                            //    result += "[{id:'declare',typename:'报关'},{id:'inspect',typename:'报检'}]";
                            //    break;
                            case "01":
                                result += "[{id:'order',typename:'委托',leaf:false},{id:'declare',typename:'报关'}]";
                                break;
                            case "02":
                                result += "[{id:'order',typename:'委托',leaf:false}]";
                                break;
                            case "03":
                                result += "[{id:'order',typename:'委托',leaf:false},{id:'declare',typename:'报关'}]";
                                break;
                        }
                    }
                    else
                    {
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
                    if (role == "enterprise")
                    {
                       if (i != dt.Rows.Count - 1)
                        {
                            result += "{id:'44_" + (i + 1) + "',typename:'订单文件_" + (i + 1) + "',fileid:'" + dr["ID"] + "',leaf:true,url:'" + AdminUrl + "/file/" + dr["FILENAME"] + "'},";
                        }
                        else
                        {
                            result += "{id:'44_" + (i + 1) + "',typename:'订单文件_" + (i + 1) + "',fileid:'" + dr["ID"] + "',leaf:true,url:'" + AdminUrl + "/file/" + dr["FILENAME"] + "'}";
                        }
                    }
                    else
                    {
                        if (i != dt.Rows.Count - 1)
                        {
                            result += "{id:'44_" + (i + 1) + "',typename:'订单文件_" + (i + 1) + "',fileid:'" + dr["ID"] + "',url:'" + AdminUrl + "/file/" + dr["FILENAME"] + "'},";
                        }
                        else
                        {
                            result += "{id:'44_" + (i + 1) + "',typename:'订单文件_" + (i + 1) + "',fileid:'" + dr["ID"] + "',url:'" + AdminUrl + "/file/" + dr["FILENAME"] + "'}";
                        }
                    
                    }
                   
                    i++;
                }
                result += "]";
                return result;
            }
            if (id.LastIndexOf("44_") >= 0 && role != "enterprise")
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
                sql = @"select CODE,DECLARATIONCODE from list_declaration t where t.ordercode='" + ordercode + "' order by COSTARTTIME";
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
                sql = @"select CODE,INSPECTIONCODE from list_inspection t where t.ordercode='" + ordercode + "' and t.INSPECTIONCODE is not null order by COSTARTTIME"; //starttime
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
                sql = @"select CODE,APPROVALCODE from list_inspection t where t.ordercode='" + ordercode + "' and t.APPROVALCODE is not null order by COSTARTTIME";//starttime
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
            string top = Request["top"]; string right = Request["right"]; string left = Request["left"]; string buttom = Request["buttom"];
            string sql = "select t.* from List_Attachment t where t.id='" + fileid + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            string output = Guid.NewGuid() + "";
            IList<string> filelist = new List<string>();
            string role = Request["role"];
            if (printtype == "standardprint")//如果是标准打印
            {
                //报关单标准打印的时候用户必须在前端选择多个打印模板 单证二期已经删除了草单表,报关单标准打印已经摒弃了申报库别的判断 by panhuaguo2016-12-13
                string[] tmp_array = printtmp.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (tmp_array.Length > 0)
                {
                    foreach (string tmpname in tmp_array)
                    {
                        string outpath = AddBackground(dt.Rows[0]["FILENAME"] + "", tmpname, busitype, "", top, right, buttom, left);
                        filelist.Add(outpath);
                    }
                    UpdatePrintInfo("list_declaration", dt.Rows[0]["DECLCODE"] + "", role);
                }
                else
                {
                    filelist.Add(AdminUrl + "/file/" + dt.Rows[0]["FILENAME"]);
                    UpdatePrintInfo("list_declaration", dt.Rows[0]["DECLCODE"] + "", role);
                }
            }
            else//套打打印
            {
                filelist.Add(AdminUrl + "/file/" + dt.Rows[0]["FILENAME"]);
                UpdatePrintInfo("list_declaration", dt.Rows[0]["DECLCODE"] + "", role);
            }
            string result = string.Empty;
            if (filelist.Count > 1)
            {
                MergePDFFiles(filelist, Server.MapPath("~/Declare/") + output + ".pdf");
                result = "/Declare/" + output + ".pdf";
            }
            else
            {
                result = filelist[0];
                System.Uri Uri = new Uri("ftp://" + ConfigurationManager.AppSettings["FTPServer"] + ":" + ConfigurationManager.AppSettings["FTPPortNO"]);
                string UserName = ConfigurationManager.AppSettings["FTPUserName"];
                string Password = ConfigurationManager.AppSettings["FTPPassword"];
                FtpHelper ftp = new FtpHelper(Uri, UserName, Password);
                bool file = ftp.DownloadFile("/" + dt.Rows[0]["FILENAME"] + "", ConfigurationManager.AppSettings["DeclareFile"] + output + ".pdf");
            }
            //将报关单打印任务推送至缓存
            IDatabase db = SeRedis.redis.GetDatabase();
            db.ListLeftPush("fileprint", "{user:'" + HttpContext.User.Identity.Name + "',filename:'" + output + ".pdf'}");
            return result;
        }

        public string Loadwatermark()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = "select t.* from config_watermark t where t.CUSTOMER='" + json_user.Value<string>("CUSTOMERID") + "'";
            DataTable dt = new DataTable(); string formdata = "{}";
            dt = DBMgr.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                formdata = JsonConvert.SerializeObject(dt).TrimStart('[').TrimEnd(']');
            }
            else
            {
                formdata = "{\"POSITIONWEBTOP\":0,\"POSITIONWEBRIGHT\":0,\"POSITIONWEBBUTTOM\":0,\"POSITIONWEBLEFT\":0}";
            }

            return "{formdata:" + formdata + "}";
        }

        //给报关单文件增加背景图片 by panhuaguo 2016-04-19  
        //修改需求 如果申报关区是昆山综保2369的话 使用进境出境作为报关单背景图 by panhuaguo 2016-08-15
        //修改需求 依据申报库别进行判断
        public string AddBackground(string filename, string printtmp, string busitype, string decltype, string top, string right, string buttom, string left)
        {
            string outname = Guid.NewGuid() + "";

            int top_int = Convert.ToInt32(top == "" ? "0" : top); int right_int = Convert.ToInt32(right == "" ? "0" : right);
            int buttom_int = Convert.ToInt32(buttom == "" ? "0" : buttom); int left_int = Convert.ToInt32(left == "" ? "0" : left);

            Image img = null;
            if (busitype == "11" || busitype == "21" || busitype == "31" || busitype == "41" || busitype == "51")
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
            string destFile = Server.MapPath("~/Declare/") + outname + ".pdf";
            FileStream stream = new FileStream(destFile, FileMode.Create, FileAccess.ReadWrite);

            Uri url = new Uri(AdminUrl + "/file/" + filename);
            PdfReader reader = new PdfReader(url);

            iTextSharp.text.Rectangle psize = reader.GetPageSize(1);
            var imgWidth = psize.Width + right_int;
            var imgHeight = psize.Height - top_int + buttom_int;
            img.ScaleAbsolute(imgWidth, imgHeight);
            img.SetAbsolutePosition(0 + left_int, 0 - buttom_int);//坐标是从左下角开始算的，注意 

            PdfStamper stamper = new PdfStamper(reader, stream);    //read pdf stream 
            int totalPage = reader.NumberOfPages;
            for (int current = 1; current <= totalPage; current++)
            {
                var canvas = stamper.GetUnderContent(current);
                var page = stamper.GetImportedPage(reader, current);
                canvas.AddImage(img);
            }
            stamper.Close();
            reader.Close();

            //记下坐标
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            DataTable dt = new DataTable(); string sql = "";
            sql = "select t.* from config_watermark t where t.CUSTOMER='" + json_user.Value<string>("CUSTOMERID") + "'";
            dt = DBMgr.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                sql = @"update config_watermark set POSITIONWEBTOP={1},POSITIONWEBRIGHT={2},POSITIONWEBBUTTOM={3},POSITIONWEBLEFT={4} 
                        where CUSTOMER='{0}'";
            }
            else
            {
                sql = @"insert into config_watermark(CUSTOMER,POSITIONWEBTOP,POSITIONWEBRIGHT,POSITIONWEBBUTTOM,POSITIONWEBLEFT) 
                        values('{0}',{1},{2},{3},{4})";
            }
            sql = string.Format(sql, json_user.Value<string>("CUSTOMERID"), top_int, right_int, buttom_int, left_int);
            DBMgr.ExecuteNonQuery(sql);

            return "http://" + Request.Url.Authority + "/Declare/" + outname + ".pdf";
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
                //if (decltype == "11" || decltype == "17")
                //{
                //    img = Image.GetInstance(Server.MapPath("/FileUpload/进境备案清单简化.jpg"));
                //}
                //else
                //{
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
                //}
            }
            else
            {
                //if (decltype == "12" || decltype == "18")
                //{
                //    img = Image.GetInstance(Server.MapPath("/FileUpload/出境备案清单简化.jpg"));
                //}
                //else
                //{
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
                //}
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
            //if (decltype == "11" || decltype == "12" || decltype == "17" || decltype == "18")
            //{
            //    img.SetAbsolutePosition(-10, -6);
            //}
            //else
            //{
            img.SetAbsolutePosition(0, 0);
            //}
            int totalPage = reader.NumberOfPages;
            for (int current = 1; current <= totalPage; current++)
            {
                var canvas = stamper.GetUnderContent(current);
                var page = stamper.GetImportedPage(reader, current);
                canvas.AddImage(img);
            }
            stamper.Close();
            reader.Close();
            return "http://" + Request.Url.Authority + "/Declare/" + outname + ".pdf";
        }

        public void UpdatePrintInfo(string tablename, string code, string role)
        {
            if(role=="enterprise")
            {
           
            }
            else
            {
             string sql = @"update " + tablename + " set PRINTNUM = PRINTNUM+1,ISPRINT = 1,PRINTTIME=sysdate where CODE='" + code + "'";
            DBMgr.ExecuteNonQuery(sql);
            }
            
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
        //自动打印
        //private string ConvertPDFToPDF(string filePath, bool print)
        //{
        //    string result = Guid.NewGuid().ToString();
        //    PdfReader reader = new PdfReader(filePath);
        //    Document document = new Document(reader.GetPageSizeWithRotation(1));
        //    int n = reader.NumberOfPages;
        //    FileStream baos = new FileStream(ConfigurationManager.AppSettings["DeclareFile"] + result + ".pdf", FileMode.Create, FileAccess.Write);
        //    PdfCopy copy = new PdfCopy(document, baos);
        //    copy.ViewerPreferences = PdfWriter.HideToolbar | PdfWriter.HideMenubar;
        //    //往pdf中写入内容     
        //    document.Open();
        //    for (int i = 1; i <= n; i++)
        //    {
        //        PdfImportedPage page = copy.GetImportedPage(reader, i);
        //        copy.AddPage(page);
        //    }
        //    if (print)
        //    {
        //        // PdfAction.JavaScript("myOnMessage();", copy);
        //        //copy.AddJavaScript("this.print(true);function myOnMessage(aMessage) {app.alert('Test',2);} var msgHandlerObject = new Object();doc.onWillPrint = myOnMessage;this.hostContainer.messageHandler = msgHandlerObject;");
        //        copy.AddJavaScript("this.print(true);");
        //    }
        //    document.Close();
        //    return @"http://192.168.252.8:8012/Declare/" + result + ".pdf";

        //}

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
                        UpdatePrintInfo("list_declaration", json.Value<string>("CODE"),"");
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
                        UpdatePrintInfo("list_declaration", json.Value<string>("CODE") + "","");
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
                        UpdatePrintInfo("list_inspection", json.Value<string>("CODE") + "","");
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
            string sql = QueryConditionInsp();

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "CREATETIME", "DESC"));
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string QueryConditionInsp()
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
                    case "ORDERCODE"://订单编号
                        where += " and instr(li.ORDERCODE,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "INSPECTIONCODE"://报检单号
                        where += " and instr(li.INSPECTIONCODE,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "APPROVALCODE"://核准单号
                        where += " and instr(li.APPROVALCODE,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "CLEARANCECODE"://通关单号
                        where += " and instr(li.CLEARANCECODE,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "CONTRACTNOORDER"://合同发票号
                        where += " and instr(lo.CONTRACTNO,'" + Request["VALUE2"] + "')>0 ";
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
                    case "BJZT"://报检状态
                        if (Request["VALUE3"] == "1") { where += " and li.STATUS = 130 "; }
                        if (Request["VALUE3"] == "0") { where += " and li.STATUS < 130 "; }
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
            }
            if (role == "supplier") //如果是现场服务
            {
                where += @" and cus.SCENEINSPECTID ='" + json_user.Value<string>("CUSTOMERID") + "' ";
            }
            if (role == "customer")
            {
                where += @" and lo.customercode ='" + json_user.Value<string>("CUSTOMERCODE") + "' ";
            }

            string sql = @"select li.ID,li.CODE,li.ORDERCODE,li.INSPSTATUS,li.ISPRINT,li.APPROVALCODE,li.INSPECTIONCODE,li.CLEARANCECODE
                                ,li.TRADEWAY,li.ISNEEDCLEARANCE,li.LAWFLAG,li.STATUS,li.MODIFYFLAG ,li.SHEETNUM
                                ,lo.CUSNO,lo.CREATETIME,lo.SUBMITTIME,lo.BUSIUNITCODE ,lo.BUSIUNITNAME ,lo.CONTRACTNO ,lo.BUSITYPE
                                ,cus.SCENEINSPECTID 
                            from list_inspection li
                                left join list_order lo on li.ordercode = lo.code 
                                left join cusdoc.sys_customer cus on lo.customercode=cus.code 
                            where li.isinvalid=0 and lo.isinvalid=0 and instr('" + busitypeid + "',lo.busitype)>0 " + where;

            return sql;
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
                DataTable erpCodeDt = DBMgrBase.GetDataTable(erpCodeSql);
                if (erpCodeDt.Rows[0]["INTERFACECODE"] != null)
                {
                    erpCode = (erpCodeDt.Rows[0]["INTERFACECODE"]).ToString();
                }
            }
            return erpCode;
        }
        //客户通过客户订单编号从ERP导入数据时需要进行判断有无重复,如果有需要确认是否继续导入  梁 2016-5-14
        public string OperateIdRepeate()
        {
            DataTable dt;
            string sql = "select * from list_order where cusno='" + Request["operateid"] + "'";
            dt = DBMgr.GetDataTable(sql);
            return "{result:'" + dt.Rows.Count + "'}";
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
                    sql = @"select '10' BUSITYPE,d.CUSTOM_CODE CUSTOMAREACODE,'' CUSTOMDISTRICTNAME,d.SB_CUSTOM_CODE PORTCODE ,'' PORTNAME , 
                    '' BUSIUNITCODE ,'' BUSIUNITNAME ,'{0}' CUSTOMERCODE,'{1}' CUSTOMERNAME,'{0}' CLEARUNIT,                       
                    '{1}' CLEARUNITNAME,d.MBL TOTALNO ,d.HBL DIVIDENO  ,d.PIECES GOODSNUM ,d.WEIGHT GOODSGW ,d.OPERATION_ID CUSNO,  
                    (select CHINNAME from Crm_Enterprise c where c.enterpriseid=d.Customer_code) CHINNAME,
                    (select case when forekeyin_no = '0' then '' else forekeyin_no end from IFM_AIRI_PORT_CHANGE where hbl=d.hbl and mbl = d.mbl) TURNPRENO ,
                     d.customer_invoice_no CONTRACTNO from OPS_AIRE_HEAD d  where d.operation_id = '{2}'";
                    break;
                case "11"://空进
                    sql = @"select '11' BUSITYPE,d.Cusom_code CUSTOMAREACODE,'' BUSIUNITCODE,'' BUSIUNITNAME,
                          '{0}' CUSTOMERCODE,'{1}' CUSTOMERNAME,'{0}' CLEARUNIT,'{1}' CLEARUNITNAME,'001' BUSIKIND,d.MBL TOTALNO,d.HBL DIVIDENO,
                          d.PIECES GOODSNUM,d.WEIGHT GOODSGW,'1' ORDERWAY,d.OPERATION_ID CUSNO,
                          (select CHINNAME from Crm_Enterprise c where c.enterpriseid=d.Customer_code) CHINNAME,                       
                          (select case when forekeyin_no = '0' then '' else forekeyin_no end from IFM_AIRI_PORT_CHANGE where hbl=d.hbl and mbl = d.mbl) TURNPRENO,                          
                          d.customer_invoice_no CONTRACTNO from OPS_AIRI_ASN d where d.operation_id = '{2}'";
                    break;
                case "20"://海出
                    sql = @"select  '20' BUSITYPE,d.SB_CUSTOM CUSTOMAREACODE,'' CUSTOMDISTRICTNAME,'' BUSIUNITCODE,'' BUSIUNITNAME, 
                    '{0}' CUSTOMERCODE,'{1}' CUSTOMERNAME,'{0}' CLEARUNIT,'{1}' CLEARUNITNAME, 
                    '001' BUSIKIND,d.FIRST_SHIP SHIPNAME  ,d.FIRST_VOYAGES FILGHTNO,d.BOOK_BILL_NUMBER SECONDLADINGBILLNO,
                    d.PIECES_TOTAL GOODSNUM,d.WEIGHT_TOTAL GOODSGW  ,d.PACK_CODE PACKKIND  ,'1' ORDERWAY ,d.OPERATION_ID CUSNO,
                    d.CUSTOMER_FP_NO CONTRACTNO ,(select CHINNAME from Crm_Enterprise c where c.enterpriseid=d.Customer_code) CHINNAME     
                    from OPS_SEAO_HEAD d  where d.OPERATION_ID = '{2}'";
                    break;
                case "21":
                    sql = @"select  '21' BUSITYPE,d.CUSOM_CODE CUSTOMAREACODE,'' CUSTOMDISTRICTNAME,'' BUSIUNITCODE,'' BUSIUNITNAME,
                          '{0}' CUSTOMERCODE,'{1}' CUSTOMERNAME,'{0}' CLEARUNIT,'{1}' CLEARUNITNAME,                         
                          d.SECOND_SHIP SHIPNAME,d.SECOND_VOYAGES FILGHTNO,d.FIRST_BILL FIRSTLADINGBILLNO,d.main_no MAIN_NO, 
                          d.SECOND_BILL SECONDLADINGBILLNO ,d.PIECES_TOTAL GOODSNUM,d.WEIGHT_TOTAL GOODSGW,d.PACK_CODE PACKKIND,d.OPERATION_ID CUSNO,
                          (select CHINNAME from Crm_Enterprise c where c.enterpriseid=d.Customer_code) CHINNAME
                          from OPS_SEAI_ASN_HEAD d  where d.OPERATION_ID = '{2}'";
                    break;
                case "30"://陆运
                    sql = @"select '30' BUSITYPE,d.Custom_code CUSTOMAREACODE ,d.DECLARE_CUSTOME PORTCODE ,'' CUSTOMDISTRICTNAME,
                          '' BUSIUNITCODE ,'' BUSIUNITNAME,'{0}' CUSTOMERCODE,'{1}' CUSTOMERNAME, 
                          '{0}' CLEARUNIT,'{1}' CLEARUNITNAME,d.PACK_MODE PACKKIND,d.DECLAREPIECE GOODSNUM,d.WEIGHT GOODSGW,
                          d.TRACKING_NO CUSNO,(select CHINNAME from Crm_Enterprise c where c.enterpriseid=d.Customer_code) CHINNAME                    
                          from OPS_LE_HEAD d where d.TRACKING_NO = '{2}'";
                    break;
                case "31":
                    sql = @"select  '31' BUSITYPE,d.Custom_code CUSTOMAREACODE,d.JJ_PORT PORTCODE ,lao.TRUCK_ASSIGN_NO MANIFEST,   
                        '' CUSTOMDISTRICTNAME,'' BUSIUNITCODE ,'' BUSIUNITNAME ,   
                        '{0}' CUSTOMERCODE,'{1}' CUSTOMERNAME,'{0}' CLEARUNIT,'{1}' CLEARUNITNAME,'001' BUSIKIND,d.PACK_MODE PACKKIND,  
                        d.PIECES GOODSNUM,d.WEIGHT GOODSGW,d.TRACKING_NO CUSNO, 
                        (select CHINNAME from Crm_Enterprise c where c.enterpriseid=d.Customer_code) CHINNAME   
                        from OPS_LI_HEAD d left join OPS_LI_ASSIGN_OP lao on d.tracking_no=lao.tracking_no where d.TRACKING_NO = '{2}'";
                    break;
                case "50":
                case "51":
                    sql = @"select d.DECLARE_CUSTOM CUSTOMAREACODE,CUSTOM_CODE PORTCODE  ,CONSIGN_CODE BUSIUNITCODE,PIECES GOODSNUM,  
                    WEIGHT GOODSGW,PACK_TYPE  PACKKIND,'' BUSITYPE,'1' ORDERWAY ,OPERATION_ID CUSNO ,INV_NO CONTRACTNO ,'' CUSTOMDISTRICTNAME,'' BUSIUNITNAME,  
                    '{0}' CUSTOMERCODE ,'{0}' CLEARUNIT,'003' BUSIKIND ,'{1}' CUSTOMERNAME,'{1}' CLEARUNITNAME,DECLARE_MODE,     
                    '' REPWAYID,(select CHINNAME from Crm_Enterprise c where c.enterpriseid=d.CONSIGN_CODE) CHINNAME  from ops_ts_head d   where d.operation_id = '{2}'";
                    break;
            }
            sql = string.Format(sql, json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME"), operationid);
            //DataSet ds = DBMgrERP.GetDataSet(sql);
            dt = DBMgrERP.GetDataTable(sql);
            if (dt.Rows.Count > 0)//(ds != null && ds.Tables.Count > 0)
            {
                //dt = ds.Tables[0];//通过ERP的经营单位中文全名到本系统数据库进行匹配,加载本地经营单位                
                sql = "select id,CODE,NAME from base_company where translate(name,'（）','()') = '" + dt.Rows[0]["CHINNAME"].ToString().Replace('（', '(').Replace('）', ')') + "'";
                DataTable dtt = DBMgrBase.GetDataTable(sql);
                if (dtt.Rows.Count > 0)
                {
                    dt.Rows[0]["BUSIUNITCODE"] = dtt.Rows[0]["CODE"] + "";
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
        //根据文件统一编号从文件服务器获取文件信息和文件实体
        public string LoadEnterpriseFile()
        {
            string fileuniteno = (Request["fileuniteno"] + "").Trim();
            string sql = "select * from ent_order where UNITCODE='" + fileuniteno + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                sql = "select FILENAME,ORIGINALNAME,SIZES from list_attachment where entid='" + dt.Rows[0]["ID"] + "'";
                DataTable dt_detail = DBMgr.GetDataTable(sql);
                //FTP信息 从文件服务器DOWNLOAD到WEB虚拟目录
                System.Uri Uri = new Uri("ftp://" + ConfigurationManager.AppSettings["FTPServer"] + ":" + ConfigurationManager.AppSettings["FTPPortNO"]);
                string UserName = ConfigurationManager.AppSettings["FTPUserName"];
                string Password = ConfigurationManager.AppSettings["FTPPassword"];
                FtpHelper ftp = new FtpHelper(Uri, UserName, Password);
                string localpath = "";
                string remotepath = "";
                string[] filename = null;
                foreach (DataRow dr in dt_detail.Rows)
                {
                    filename = (dr["FILENAME"] + "").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    localpath = ConfigurationManager.AppSettings["WebFilePath"] + filename[filename.Length - 1];
                    remotepath = dr["FILENAME"] + "";
                    ftp.DownloadFile(remotepath, localpath);
                    dr["ORIGINALNAME"] = filename[filename.Length - 1];
                }
                return "{success:true,data:" + JsonConvert.SerializeObject(dt_detail) + "}";
            }
            return "{success:false}";
        }


        // 加载批量维护订单
        public string LoadBatchMaintain()
        {
            string sql = @"select t.* from list_order t where t.id in (" + Request["ids"] + ")";
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "createtime", "desc"));
            var json = JsonConvert.SerializeObject(dt);
            return "{rows:" + json + "}";
        }

        public string BatchMaintainSave()
        {
            string resultmsg = "";
            string ordercodes = Request["ordercodes"]; string filedata = Request["filedata"];

            try
            {
                JObject json = (JObject)JsonConvert.DeserializeObject(Request["formdata"]);
                JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

                string sql = ""; bool IsCONTAINERTRUCK = false; string[] ordercodelist = ordercodes.Split(',');

                bool bf = true;
                string sql_search = "select * from list_order where code in ('" + ordercodes.Replace(",", "','") + "')";
                DataTable dt_search = DBMgr.GetDataTable(sql_search);

                foreach (JProperty jp in (JToken)json)
                {
                    if (jp.Name != "CONTAINERTRUCK" && jp.Value.ToString() != "")
                    {
                        if (jp.Name != "ORDERREQUEST")//备注可以修改
                        {
                            foreach (DataRow dr in dt_search.Rows)
                            {
                                if ((dr[jp.Name] + "") != "") { bf = false; break; }//只要有一个单号的值不为空，就不拼sql
                            }
                        }

                        if (bf)
                        {
                            sql += jp.Name + "='" + jp.Value.ToString() + "',";
                        }
                    }
                    if (jp.Name == "CONTAINERTRUCK" && jp.Value.ToString() != "")
                    {
                        IsCONTAINERTRUCK = true;
                    }
                }
                if (sql != "")
                {
                    sql = sql.Substring(0, sql.Length - 1);
                    sql = "update list_order set " + sql + " where code in ('" + ordercodes.Replace(",", "','") + "')";
                    DBMgr.ExecuteNonQuery(sql);
                }

                if (IsCONTAINERTRUCK) //集装箱及报关车号列表更新
                {
                    for (int i = 0; i < ordercodelist.Length; i++)
                    {
                        Extension.predeclcontainer_update(ordercodelist[i], json.Value<string>("CONTAINERTRUCK"));
                    }
                }
                //更新随附文件 
                for (int i = 0; i < ordercodelist.Length; i++)
                {
                    Extension.Update_Attachment(ordercodelist[i], filedata, "", json_user);//json.Value<string>("ORIGINALFILEIDS")
                }
                resultmsg = "{success:true}";
            }
            catch (Exception ex)
            {
                resultmsg = "{success:false}";
            }
            return resultmsg;

        }

        public string loadOrderView()
        {
            string sql = @"select t.* from LIST_ORDER t where t.CODE = '" + Request["ordercode"] + "' and rownum=1";
            DataTable dt = DBMgr.GetDataTable(sql);
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string result = JsonConvert.SerializeObject(dt, iso);
            return "{success:true,data:" + result.TrimStart('[').TrimEnd(']') + "}";
        }

        public string LoadDeclaration()
        {
            string sql = @"select ID,DECLARATIONCODE,GOODSGW,GOODSNW,GOODSNUM,SHEETNUM,BUSITYPE,CUSTOMSSTATUS,TRADECODE,COMMODITYNUM from list_declaration  
                         WHERE ordercode='" + Request["ORDERCODE"] + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            var json = JsonConvert.SerializeObject(dt);

            return "{rows:" + json + "}";
        }

        public string LoadInspection()
        {
            string sql = @"SELECT li.ID,li.CODE, li.APPROVALCODE,li.INSPECTIONCODE,lo.INSPUNITNAME,lo.WOODPACKINGID,li.ORDERCODE,lo.CUSNO,li.ISPRINT,lo.GOODSNUM,lo.BUSITYPE,lo.INSPUNITNAME,lo.CONTRACTNO,
                          lo.TOTALNO,li.INSPECTIONCODE
                         FROM list_inspection li  LEFT JOIN list_order lo ON li.ordercode = lo.code   
                         WHERE li.ORDERCODE='" + Request["ordercode"] + "'";
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            DataTable dt = DBMgr.GetDataTable(sql);
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + "}";
        }

        public string LoadOrderTrack()
        {
            string ordercode = Request["ordercode"];
            string dec_insp_status = Request["dec_insp_status"];
            //根据订单号获取当前状态信息和所有状态变更的时间
            string sql = @"select * from list_order t where t.code='" + ordercode + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            string status_cur = dt.Rows[0]["STATUS"] + "";
            string entrusttypeid = dt.Rows[0]["ENTRUSTTYPE"] + "";//委托类型 01 报关单  02报检单  03报关报检
            //类型（1订单、2报关草单、3报检草单、4预制报关单、5预制报检单）
            //查询订单已经走过的节点lt.ID, lt.CODE, lt.TIMES, lt.STATUS, lt.ISPAUSE, lt.REASON, tb2.VALUE AS STATUS_NAMEAND (lt.STATUS <= 45 OR lt.STATUS >= 110) 
            sql = @"SELECT lt.TIMES,lt.STATUS FROM list_times lt WHERE lt.CODE = '" + ordercode + "' AND lt.TYPE = 1 ORDER BY lt.STATUS asc, lt.TIMES asc";
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            dt = DBMgr.GetDataTable(sql);
            string decltrack = "[]";
            if (entrusttypeid == "01" || entrusttypeid == "03")
            {
                decltrack = Declare_Inspect_Track(ordercode, "declare", dec_insp_status);
            }
            string insptrack = "[]";
            if (entrusttypeid == "02" || entrusttypeid == "03")
            {
                insptrack = Declare_Inspect_Track(ordercode, "inspection", dec_insp_status);
            }
            return "{entrusttypeid:'" + entrusttypeid + "',status:'" + status_cur + "',rows:" + JsonConvert.SerializeObject(dt, iso) + ",declare:" + decltrack + ",insptrack:" + insptrack + "}";
        }
        public string Declare_Inspect_Track(string ordercode, string source, string dec_insp_status)
        {
            string sql = "";
            int total = 0;
            int status = 0;
            DataTable dt = null;
            DataTable dt_times = null;
            string declnos = "";//保存所有的预制报关_预制报检单号
            if (source == "declare")
            {
                sql = @"select * from list_declaration ld where ordercode='" + ordercode + "'";
                dt = DBMgr.GetDataTable(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    declnos += dt.Rows[i]["CODE"];
                }
                total = dt.Rows.Count;
            }
            else
            {
                sql = @"select * from list_inspection ld where ordercode='" + ordercode + "'";
                dt = DBMgr.GetDataTable(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    declnos += dt.Rows[i]["CODE"];
                }
                total = dt.Rows.Count;
            }
            DataTable dt_cus = new DataTable();
            DataColumn dc = new DataColumn("total");
            dt_cus.Columns.Add(dc);
            dc = new DataColumn("finished");
            dt_cus.Columns.Add(dc);
            dc = new DataColumn("exception");
            dt_cus.Columns.Add(dc);
            dc = new DataColumn("times");
            dt_cus.Columns.Add(dc);
            dc = new DataColumn("status");
            dt_cus.Columns.Add(dc);
            dc = new DataColumn("name");
            dt_cus.Columns.Add(dc);
            //string dec_insp_status = "[{CODE: 25,NAME:'预审中'}, {CODE: 40,NAME: '预审完成'},{CODE: 45,NAME:'制单已受理'},{CODE: 50,NAME:'制单中'},{CODE: 55,NAME:'制单完成'},{CODE: 60,NAME:'待审核'},{CODE: 65,NAME:'审核已受理'},{CODE:70,NAME:'审核中'},{CODE:75,NAME:'审核完成'},{CODE:78,NAME:'待预录'},{CODE:80,NAME:'预录已受理'},{CODE:85,NAME:'预录中'},{CODE:90,NAME:'预录完成'},{CODE:95,NAME:'预录校验完成'},{CODE:100,NAME:'申报中'},{CODE:105,NAME:'申报完成'},{CODE:110,NAME:'申报完结'}]";
            JArray jarray = JArray.Parse(dec_insp_status);
            foreach (JObject json in jarray)
            {
                DataRow dr_cus = dt_cus.NewRow();
                status = json.Value<int>("CODE");
                int finished = 0; int exception = 0;

                foreach (DataRow dr in dt.Rows)//循环预制报关_报检单
                {
                    int declstatus = Convert.ToInt32(dr["STATUS"] + "");//预制报关单当前状态
                    if (declstatus == status && (dr["ISPAUSE"] + "") == "1")//先判断是否异常 暂存了 预录是否暂存（0正常，1暂存）AND lt.TYPE = 4 
                    {
                        exception++;
                    }
                    else if (declstatus >= status)
                    {
                        finished++;
                    }
                }
                dr_cus["total"] = total; dr_cus["finished"] = finished; dr_cus["exception"] = exception;
                sql = @"SELECT lt.TIMES,lt.STATUS FROM list_times lt WHERE instr('" + declnos + "',lt.CODE)>0 and lt.STATUS='" + status + "' order by lt.TIMES desc";
                dt_times = DBMgr.GetDataTable(sql);
                if (dt_times.Rows.Count > 0)
                {
                    dr_cus["times"] = dt_times.Rows[0]["TIMES"];
                }

                dr_cus["status"] = json.Value<string>("CODE");
                dr_cus["name"] = json.Value<string>("NAME");
                dt_cus.Rows.Add(dr_cus);
            }
            return JsonConvert.SerializeObject(dt_cus);
        }

        public string PdfPrint()
        {
            try
            {
                Uri url = new Uri(ConfigurationManager.AppSettings["AdminUrl"].ToString() + "/file/" + Request["filename"]);
                string path = ConfigurationManager.AppSettings["WebFilePath"].ToString();
                PdfReader pdfReader = new PdfReader(url);
                string newname = Guid.NewGuid().ToString();

                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(path + newname + ".pdf", FileMode.Create, FileAccess.Write, FileShare.None));
                BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\simhei.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);//获取系统的字体  
                iTextSharp.text.Font font = new iTextSharp.text.Font(baseFont, 12);

                //Phrase p = new Phrase(Request["ordercode"] + "  " + Request["repwayname"], font);
                Chunk chunk = new Chunk(Request["ordercode"] + " " + Request["repwayname"] + " " + Request["createtime"], font); chunk.SetBackground(BaseColor.WHITE);
                Phrase p = new Phrase();
                p.Add(chunk);
                //设置块的背景色           
                PdfContentByte over = pdfStamper.GetOverContent(1);//PdfContentBye类，用来设置图像和文本的绝对位置  
                ColumnText.ShowTextAligned(over, Element.ALIGN_CENTER, p, pdfReader.GetPageSize(1).Width - 150, pdfReader.GetPageSize(1).Height - 20, 0);
                pdfStamper.Close();
                string sql = "";
                if (Request["type"] + "" != "gn")//如果是非国内订单
                {
                    sql = "update list_order set printstatus=1 where code='" + Request["ordercode"] + "'";
                }
                else
                {
                    sql = "select * from list_order where code='" + Request["ordercode"] + "'";
                    DataTable dt = DBMgr.GetDataTable(sql);
                    if (!string.IsNullOrEmpty(dt.Rows[0]["CORRESPONDNO"] + ""))
                    {
                        sql = "update list_order set printstatus=1 where CORRESPONDNO='" + dt.Rows[0]["CORRESPONDNO"] + "'";
                    }
                    else if (!string.IsNullOrEmpty(dt.Rows[0]["ASSOCIATENO"] + ""))
                    {
                        sql = "update list_order set printstatus=1 where ASSOCIATENO='" + dt.Rows[0]["ASSOCIATENO"] + "'";
                    }
                    else
                    {
                        sql = "update list_order set printstatus=1 where code='" + Request["ordercode"] + "'";
                    }
                }
                DBMgr.ExecuteNonQuery(sql);
                return newname + ".pdf";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public string ExportList()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string where = QueryCondition();
            string dec_insp_status = Request["dec_insp_status"]; string busitypeid = Request["busitypeid"]; string busitype = "";

            switch (busitypeid)
            {
                case "11":
                    busitype = "空运进口";
                    break;
                case "10":
                    busitype = "空运出口";
                    break;
                case "21":
                    busitype = "海运进口";
                    break;
                case "20":
                    busitype = "海运出口";
                    break;
                case "31":
                    busitype = "陆运进口";
                    break;
                case "30":
                    busitype = "陆运出口";
                    break;
                case "40,41":
                    busitype = "国内";
                    break;
                case "50,51":
                    busitype = "特殊区域";
                    break;
            }

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string sql = "select a.*,b.REPWAYNAME from LIST_ORDER a "
                    + " left join (select CODE,NAME||'('||CODE||')' REPWAYNAME from cusdoc.SYS_REPWAY where Enabled=1 and instr(busitype,'" + busitype + "')>0) b on a.REPWAYID=b.CODE "
                    + " where instr('" + busitypeid + "',a.BUSITYPE)>0 and a.customercode='" + json_user.Value<string>("CUSTOMERCODE") + "' " + where;

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
            string filename = "订单文件";

            #region 11 空进
            if (busitypeid == "11")//空进
            {
                sheet_S = book.CreateSheet("订单信息_空进"); filename = filename + "_空进.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("客户编号");
                row1.CreateCell(4).SetCellValue("经营单位"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("总单号"); row1.CreateCell(7).SetCellValue("分单号"); 
                row1.CreateCell(8).SetCellValue("件数/重量"); row1.CreateCell(9).SetCellValue("打印状态");row1.CreateCell(10).SetCellValue("申报关区"); row1.CreateCell(11).SetCellValue("进/出口岸");
                row1.CreateCell(12).SetCellValue("申报方式");row1.CreateCell(13).SetCellValue("转关预录号");row1.CreateCell(14).SetCellValue("法检"); row1.CreateCell(15).SetCellValue("委托时间");                 

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["TOTALNO"].ToString());
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["DIVIDENO"].ToString());

                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(8).SetCellValue("");
                    }
                    rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["TURNPRENO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["SUBMITTIME"].ToString());
                }
            }
            #endregion

            #region 10 空出
            if (busitypeid == "10")//空出
            {
                sheet_S = book.CreateSheet("订单信息_空出"); filename = filename + "_空出.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("客户编号");
                row1.CreateCell(4).SetCellValue("经营单位"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("总单号"); row1.CreateCell(7).SetCellValue("分单号");
                row1.CreateCell(8).SetCellValue("件数/重量"); row1.CreateCell(9).SetCellValue("打印状态"); row1.CreateCell(10).SetCellValue("申报关区"); row1.CreateCell(11).SetCellValue("进/出口岸");
                row1.CreateCell(12).SetCellValue("申报方式"); row1.CreateCell(13).SetCellValue("运抵编号"); row1.CreateCell(14).SetCellValue("法检"); row1.CreateCell(15).SetCellValue("委托时间");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["TOTALNO"].ToString());
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["DIVIDENO"].ToString());

                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(8).SetCellValue("");
                    }
                    rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["ARRIVEDNO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["SUBMITTIME"].ToString());
                }
            }
            #endregion

            #region 21 海进
            if (busitypeid == "21")//海进
            {
                sheet_S = book.CreateSheet("订单信息_海进"); filename = filename + "_海进.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("客户编号");
                row1.CreateCell(4).SetCellValue("经营单位"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("打印状态"); row1.CreateCell(7).SetCellValue("国检提单号");
                row1.CreateCell(8).SetCellValue("海关提单号"); row1.CreateCell(9).SetCellValue("件数/重量"); row1.CreateCell(10).SetCellValue("申报关区"); row1.CreateCell(11).SetCellValue("进/出口岸");
                row1.CreateCell(12).SetCellValue("申报方式"); row1.CreateCell(13).SetCellValue("转关预录号"); row1.CreateCell(14).SetCellValue("法检"); row1.CreateCell(15).SetCellValue("委托时间");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["FIRSTLADINGBILLNO"].ToString());

                    rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["SECONDLADINGBILLNO"].ToString());
                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(9).SetCellValue("");
                    }
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["TURNPRENO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["SUBMITTIME"].ToString());
                }
            }
            #endregion

            #region 20 海出
            if (busitypeid == "20")//海出
            {
                sheet_S = book.CreateSheet("订单信息_海出"); filename = filename + "_海出.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("客户编号");
                row1.CreateCell(4).SetCellValue("经营单位"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("提单号"); row1.CreateCell(7).SetCellValue("打印状态");
                row1.CreateCell(8).SetCellValue("运抵编号"); row1.CreateCell(9).SetCellValue("件数/重量"); row1.CreateCell(10).SetCellValue("申报关区"); row1.CreateCell(11).SetCellValue("进/出口岸");
                row1.CreateCell(12).SetCellValue("申报方式"); row1.CreateCell(13).SetCellValue("转关预录号"); row1.CreateCell(14).SetCellValue("法检"); row1.CreateCell(15).SetCellValue("委托时间");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["SECONDLADINGBILLNO"].ToString());
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");

                    rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["ARRIVEDNO"].ToString());
                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(9).SetCellValue("");
                    }
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["TURNPRENO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["SUBMITTIME"].ToString());
                }
            }
            #endregion

            #region 31 陆进
            if (busitypeid == "31")//陆进
            {
                sheet_S = book.CreateSheet("订单信息_陆进"); filename = filename + "_陆进.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("客户编号");
                row1.CreateCell(4).SetCellValue("经营单位"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("分单号"); row1.CreateCell(7).SetCellValue("打印状态");
                row1.CreateCell(8).SetCellValue("件数/重量"); row1.CreateCell(9).SetCellValue("申报关区"); row1.CreateCell(10).SetCellValue("进/出口岸"); row1.CreateCell(11).SetCellValue("申报方式");
                row1.CreateCell(12).SetCellValue("法检"); row1.CreateCell(13).SetCellValue("委托时间");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["DIVIDENO"].ToString());
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");

                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(8).SetCellValue("");
                    }
                    rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["SUBMITTIME"].ToString());
                }
            }
            #endregion

            #region 30 陆出
            if (busitypeid == "30")//陆出
            {
                sheet_S = book.CreateSheet("订单信息_陆出"); filename = filename + "_陆出.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("客户编号");
                row1.CreateCell(4).SetCellValue("经营单位"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("总单号"); row1.CreateCell(7).SetCellValue("分单号");
                row1.CreateCell(8).SetCellValue("打印状态"); row1.CreateCell(9).SetCellValue("件数/重量"); row1.CreateCell(10).SetCellValue("申报关区"); row1.CreateCell(11).SetCellValue("进/出口岸");
                row1.CreateCell(12).SetCellValue("申报方式"); row1.CreateCell(13).SetCellValue("运抵编号"); row1.CreateCell(14).SetCellValue("转关预录号"); row1.CreateCell(15).SetCellValue("法检");
                row1.CreateCell(16).SetCellValue("委托时间");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["TOTALNO"].ToString());
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["DIVIDENO"].ToString());

                    rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");
                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(9).SetCellValue("");
                    }
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["ARRIVEDNO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["TURNPRENO"].ToString());
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");

                    rowtemp.CreateCell(16).SetCellValue(dt.Rows[i]["SUBMITTIME"].ToString());
                }
            }
            #endregion

            #region 40,41 国内
            if (busitypeid == "40,41")//国内
            {
                sheet_S = book.CreateSheet("订单信息_国内"); filename = filename + "_国内.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("客户编号");
                row1.CreateCell(4).SetCellValue("经营单位"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("件数/重量"); row1.CreateCell(7).SetCellValue("打印状态");
                row1.CreateCell(8).SetCellValue("申报关区"); row1.CreateCell(9).SetCellValue("申报方式"); row1.CreateCell(10).SetCellValue("法检"); row1.CreateCell(11).SetCellValue("业务类型");
                row1.CreateCell(12).SetCellValue("两单关联号"); row1.CreateCell(13).SetCellValue("委托时间"); row1.CreateCell(14).SetCellValue("多单关联号");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(6).SetCellValue("");
                    }
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");

                    rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    if (dt.Rows[i]["BUSITYPE"].ToString()=="40")
                    {
                        rowtemp.CreateCell(11).SetCellValue("国内出口");
                    }
                    else if (dt.Rows[i]["BUSITYPE"].ToString() == "41")
                    {
                        rowtemp.CreateCell(11).SetCellValue("国内进口");
                    }
                    else
                    {
                        rowtemp.CreateCell(11).SetCellValue("");
                    }                   

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["ASSOCIATENO"].ToString());
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["SUBMITTIME"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["CORRESPONDNO"].ToString());
                    
                }
            }
            #endregion

            #region 50,51 特殊
            if (busitypeid == "50,51")//特殊
            {
                sheet_S = book.CreateSheet("订单信息_特殊"); filename = filename + "_特殊.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("客户编号");
                row1.CreateCell(4).SetCellValue("合同号"); row1.CreateCell(5).SetCellValue("经营单位"); row1.CreateCell(6).SetCellValue("打印状态"); row1.CreateCell(7).SetCellValue("申报方式");
                row1.CreateCell(8).SetCellValue("件数/重量"); row1.CreateCell(9).SetCellValue("申报关区"); row1.CreateCell(10).SetCellValue("进/出口岸"); row1.CreateCell(11).SetCellValue("转关预录号"); 
                row1.CreateCell(12).SetCellValue("法检"); row1.CreateCell(13).SetCellValue("委托时间");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CUSNO"].ToString());    
                
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());//REPWAYID

                    if (dt.Rows[i]["GOODSNUM"].ToString() != "")
                    {
                        rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString() + "/" + dt.Rows[i]["GOODSGW"].ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(8).SetCellValue("");
                    }
                    rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["CUSTOMAREACODE"].ToString());
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["TURNPRENO"].ToString());                
                   
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["SUBMITTIME"].ToString());
                }
            }
            #endregion

            // 写入到客户端 
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //book.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", filename);

            return Extension.getPathname(filename, book);
        }

        public string getStatusName(string curstatus, string dec_insp_status)
        {
            string statusname = "";
            JArray jarray = JArray.Parse(dec_insp_status);
            foreach (JObject json in jarray)
            {
                if (json.Value<string>("CODE") == curstatus) { statusname = json.Value<string>("NAME"); break; }
            }
            return statusname;
        }

        public string ExportDeclList()
        {
            string common_data_busitype = Request["common_data_busitype"]; string busitypeid = Request["busitypeid"]; string modifyflag_data = Request["modifyflag_data"];
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string sql = QueryConditionDecl();
            sql = sql + " order by CREATETIME desc";
            DataTable dt = new DataTable();
            try
            {
                DataTable dt_count = DBMgr.GetDataTable("select count(1) from (" + sql + ") a");

                int WebDownCount = Convert.ToInt32(ConfigurationManager.AppSettings["WebDownCount"]);
                if (Convert.ToInt32(dt_count.Rows[0][0]) > WebDownCount)
                {
                    return "{success:false,WebDownCount:" + WebDownCount + "}";
                }

                dt = DBMgr.GetDataTable(sql);
            }
            catch (Exception ex)
            {
                
            }

            if (dt.Rows.Count <= 0)
            {
                return "{success:false,WebDownCount:0}";
            }
                      
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();//创建Excel文件的对象            
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("报关单信息");//添加一个导出成功sheet           
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0); //给sheet1添加第一行的头部标题


            #region 空运进出口
            if (busitypeid == "10" || busitypeid == "11")
            {
                row1.CreateCell(0).SetCellValue("海关状态"); row1.CreateCell(1).SetCellValue("报关单号"); row1.CreateCell(2).SetCellValue("经营单位"); row1.CreateCell(3).SetCellValue("合同发票号");
                row1.CreateCell(4).SetCellValue("总单号"); row1.CreateCell(5).SetCellValue("分单号"); row1.CreateCell(6).SetCellValue("申报日期"); row1.CreateCell(7).SetCellValue("运输工具名称"); 
                row1.CreateCell(8).SetCellValue("业务类型"); row1.CreateCell(9).SetCellValue("出口口岸");row1.CreateCell(10).SetCellValue("提运单号"); row1.CreateCell(11).SetCellValue("申报方式");
                row1.CreateCell(12).SetCellValue("报关方式"); row1.CreateCell(13).SetCellValue("贸易方式");row1.CreateCell(14).SetCellValue("合同协议号"); row1.CreateCell(15).SetCellValue("件数");
                row1.CreateCell(16).SetCellValue("重量"); row1.CreateCell(17).SetCellValue("张数"); row1.CreateCell(18).SetCellValue("删改单"); row1.CreateCell(19).SetCellValue("订单编号"); 
                row1.CreateCell(20).SetCellValue("客户编号");
                

                //将数据逐步写入sheet_S各个行
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["CUSTOMSSTATUS"].ToString());
                    rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["DECLARATIONCODE"].ToString());
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CONTRACTNOORDER"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["TOTALNO"].ToString());
                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["DIVIDENO"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["REPTIME"].ToString());
                    if (dt.Rows[i]["TRANSNAME"].ToString() == "" && dt.Rows[i]["VOYAGENO"].ToString() == "")
                    {
                        rowtemp.CreateCell(7).SetCellValue("");

                    }
                    if (dt.Rows[i]["TRANSNAME"].ToString() == "" && dt.Rows[i]["VOYAGENO"].ToString() != "")
                    {
                        rowtemp.CreateCell(7).SetCellValue("/" + dt.Rows[i]["VOYAGENO"].ToString());

                    }
                    if (dt.Rows[i]["TRANSNAME"].ToString() != "" && dt.Rows[i]["VOYAGENO"].ToString() == "")
                    {
                        rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["TRANSNAME"].ToString());

                    }
                    if (dt.Rows[i]["TRANSNAME"].ToString() != "" && dt.Rows[i]["VOYAGENO"].ToString() != "")
                    {
                        rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["TRANSNAME"].ToString() + "/" + dt.Rows[i]["VOYAGENO"].ToString());

                    }
                    rowtemp.CreateCell(8).SetCellValue(getStatusName(dt.Rows[i]["BUSITYPE"].ToString(), common_data_busitype));
                    rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["BLNO"].ToString());//REPWAYID
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());
                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["DECLWAYNAME"].ToString());
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["TRADEMETHOD"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString());
                    rowtemp.CreateCell(16).SetCellValue(dt.Rows[i]["GOODSNW"].ToString());
                    rowtemp.CreateCell(17).SetCellValue(dt.Rows[i]["SHEETNUM"].ToString());
                    rowtemp.CreateCell(18).SetCellValue(getStatusName(dt.Rows[i]["MODIFYFLAG"].ToString(), modifyflag_data));
                    rowtemp.CreateCell(19).SetCellValue(dt.Rows[i]["ORDERCODE"].ToString());
                    rowtemp.CreateCell(20).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                }
            }
            #endregion

            #region 海运进出口
            if (busitypeid == "20" || busitypeid == "21")
            {
                row1.CreateCell(0).SetCellValue("海关状态"); row1.CreateCell(1).SetCellValue("报关单号"); row1.CreateCell(2).SetCellValue("经营单位"); row1.CreateCell(3).SetCellValue("合同发票号");
                row1.CreateCell(4).SetCellValue("海运提单号"); row1.CreateCell(5).SetCellValue("申报日期"); row1.CreateCell(6).SetCellValue("运输工具名称");row1.CreateCell(7).SetCellValue("业务类型"); 
                row1.CreateCell(8).SetCellValue("出口口岸"); row1.CreateCell(9).SetCellValue("提运单号"); row1.CreateCell(10).SetCellValue("申报方式");row1.CreateCell(11).SetCellValue("报关方式"); 
                row1.CreateCell(12).SetCellValue("贸易方式"); row1.CreateCell(13).SetCellValue("合同协议号"); row1.CreateCell(14).SetCellValue("件数");row1.CreateCell(15).SetCellValue("重量");
                row1.CreateCell(16).SetCellValue("张数"); row1.CreateCell(17).SetCellValue("删改单"); row1.CreateCell(18).SetCellValue("订单编号"); row1.CreateCell(19).SetCellValue("客户编号");


                //将数据逐步写入sheet_S各个行
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["CUSTOMSSTATUS"].ToString());
                    rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["DECLARATIONCODE"].ToString());
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CONTRACTNOORDER"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["SECONDLADINGBILLNO"].ToString());
                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["REPTIME"].ToString());
                    if (dt.Rows[i]["TRANSNAME"].ToString() == "" && dt.Rows[i]["VOYAGENO"].ToString() == "")
                    {
                        rowtemp.CreateCell(6).SetCellValue("");

                    }
                    if (dt.Rows[i]["TRANSNAME"].ToString() == "" && dt.Rows[i]["VOYAGENO"].ToString() != "")
                    {
                        rowtemp.CreateCell(6).SetCellValue("/" + dt.Rows[i]["VOYAGENO"].ToString());

                    }
                    if (dt.Rows[i]["TRANSNAME"].ToString() != "" && dt.Rows[i]["VOYAGENO"].ToString() == "")
                    {
                        rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["TRANSNAME"].ToString());

                    }
                    if (dt.Rows[i]["TRANSNAME"].ToString() != "" && dt.Rows[i]["VOYAGENO"].ToString() != "")
                    {
                        rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["TRANSNAME"].ToString() + "/" + dt.Rows[i]["VOYAGENO"].ToString());

                    }
                    rowtemp.CreateCell(7).SetCellValue(getStatusName(dt.Rows[i]["BUSITYPE"].ToString(), common_data_busitype));
                    rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());
                    rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["BLNO"].ToString());//REPWAYID
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["DECLWAYNAME"].ToString());
                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["TRADEMETHOD"].ToString());
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString());
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["GOODSNW"].ToString());
                    rowtemp.CreateCell(16).SetCellValue(dt.Rows[i]["SHEETNUM"].ToString());
                    rowtemp.CreateCell(17).SetCellValue(getStatusName(dt.Rows[i]["MODIFYFLAG"].ToString(), modifyflag_data));
                    rowtemp.CreateCell(18).SetCellValue(dt.Rows[i]["ORDERCODE"].ToString());
                    rowtemp.CreateCell(19).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                }
            }
            #endregion

            #region 陆运进出口
            if (busitypeid == "30" || busitypeid == "31")
            {
                row1.CreateCell(0).SetCellValue("海关状态"); row1.CreateCell(1).SetCellValue("报关单号"); row1.CreateCell(2).SetCellValue("经营单位"); row1.CreateCell(3).SetCellValue("合同发票号");
                row1.CreateCell(4).SetCellValue("申报日期"); row1.CreateCell(5).SetCellValue("运输工具名称"); row1.CreateCell(6).SetCellValue("业务类型"); row1.CreateCell(7).SetCellValue("出口口岸");
                row1.CreateCell(8).SetCellValue("提运单号"); row1.CreateCell(9).SetCellValue("申报方式"); row1.CreateCell(10).SetCellValue("报关方式"); row1.CreateCell(11).SetCellValue("贸易方式");
                row1.CreateCell(12).SetCellValue("合同协议号"); row1.CreateCell(13).SetCellValue("件数"); row1.CreateCell(14).SetCellValue("重量"); row1.CreateCell(15).SetCellValue("张数");
                row1.CreateCell(16).SetCellValue("删改单"); row1.CreateCell(17).SetCellValue("订单编号"); row1.CreateCell(18).SetCellValue("客户编号");

                //将数据逐步写入sheet_S各个行
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["CUSTOMSSTATUS"].ToString());
                    rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["DECLARATIONCODE"].ToString());
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CONTRACTNOORDER"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["REPTIME"].ToString());
                    if (dt.Rows[i]["TRANSNAME"].ToString() == "" && dt.Rows[i]["VOYAGENO"].ToString() == "")
                    {
                        rowtemp.CreateCell(5).SetCellValue("");

                    }
                    if (dt.Rows[i]["TRANSNAME"].ToString() == "" && dt.Rows[i]["VOYAGENO"].ToString() != "")
                    {
                        rowtemp.CreateCell(5).SetCellValue("/" + dt.Rows[i]["VOYAGENO"].ToString());

                    }
                    if (dt.Rows[i]["TRANSNAME"].ToString() != "" && dt.Rows[i]["VOYAGENO"].ToString() == "")
                    {
                        rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["TRANSNAME"].ToString());

                    }
                    if (dt.Rows[i]["TRANSNAME"].ToString() != "" && dt.Rows[i]["VOYAGENO"].ToString() != "")
                    {
                        rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["TRANSNAME"].ToString() + "/" + dt.Rows[i]["VOYAGENO"].ToString());

                    }
                    rowtemp.CreateCell(6).SetCellValue(getStatusName(dt.Rows[i]["BUSITYPE"].ToString(), common_data_busitype));
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());
                    rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["BLNO"].ToString());//REPWAYID
                    rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["DECLWAYNAME"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["TRADEMETHOD"].ToString());
                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["GOODSNW"].ToString());
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["SHEETNUM"].ToString()); 
                    rowtemp.CreateCell(16).SetCellValue(getStatusName(dt.Rows[i]["MODIFYFLAG"].ToString(), modifyflag_data));
                    rowtemp.CreateCell(17).SetCellValue(dt.Rows[i]["ORDERCODE"].ToString());
                    rowtemp.CreateCell(18).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                }
            }
            #endregion

            #region 国内进出口
            if (busitypeid == "40-41")
            {
                row1.CreateCell(0).SetCellValue("海关状态"); row1.CreateCell(1).SetCellValue("报关单号");row1.CreateCell(2).SetCellValue("经营单位");row1.CreateCell(3).SetCellValue("合同发票号");
                row1.CreateCell(4).SetCellValue("申报日期"); row1.CreateCell(5).SetCellValue("进/出"); row1.CreateCell(6).SetCellValue("两单关联号"); row1.CreateCell(7).SetCellValue("运输工具名称");                
                row1.CreateCell(8).SetCellValue("业务类型"); row1.CreateCell(9).SetCellValue("出口口岸"); row1.CreateCell(10).SetCellValue("提运单号"); row1.CreateCell(11).SetCellValue("申报方式");
                row1.CreateCell(12).SetCellValue("报关方式"); row1.CreateCell(13).SetCellValue("贸易方式"); row1.CreateCell(14).SetCellValue("合同协议号"); row1.CreateCell(15).SetCellValue("件数");
                row1.CreateCell(16).SetCellValue("重量"); row1.CreateCell(17).SetCellValue("张数"); row1.CreateCell(18).SetCellValue("多单关联号"); row1.CreateCell(19).SetCellValue("删改单");
                row1.CreateCell(20).SetCellValue("订单编号"); row1.CreateCell(21).SetCellValue("客户编号");

                //将数据逐步写入sheet_S各个行
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["CUSTOMSSTATUS"].ToString());
                    rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["DECLARATIONCODE"].ToString());
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CONTRACTNOORDER"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["REPTIME"].ToString());
                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["IETYPE"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["ASSOCIATENO"].ToString());
                    if (dt.Rows[i]["TRANSNAME"].ToString() == "" && dt.Rows[i]["VOYAGENO"].ToString() == "")
                    {
                        rowtemp.CreateCell(7).SetCellValue("");

                    }
                    if (dt.Rows[i]["TRANSNAME"].ToString() == "" && dt.Rows[i]["VOYAGENO"].ToString() != "")
                    {
                        rowtemp.CreateCell(7).SetCellValue("/" + dt.Rows[i]["VOYAGENO"].ToString());

                    }
                    if (dt.Rows[i]["TRANSNAME"].ToString() != "" && dt.Rows[i]["VOYAGENO"].ToString() == "")
                    {
                        rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["TRANSNAME"].ToString());

                    }
                    if (dt.Rows[i]["TRANSNAME"].ToString() != "" && dt.Rows[i]["VOYAGENO"].ToString() != "")
                    {
                        rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["TRANSNAME"].ToString() + "/" + dt.Rows[i]["VOYAGENO"].ToString());

                    }
                    rowtemp.CreateCell(8).SetCellValue(getStatusName(dt.Rows[i]["BUSITYPE"].ToString(), common_data_busitype));
                    rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["BLNO"].ToString());//REPWAYID
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());
                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["DECLWAYNAME"].ToString());
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["TRADEMETHOD"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString());
                    rowtemp.CreateCell(16).SetCellValue(dt.Rows[i]["GOODSNW"].ToString());
                    rowtemp.CreateCell(17).SetCellValue(dt.Rows[i]["SHEETNUM"].ToString());
                    rowtemp.CreateCell(18).SetCellValue(dt.Rows[i]["CORRESPONDNO"].ToString());
                    rowtemp.CreateCell(19).SetCellValue(getStatusName(dt.Rows[i]["MODIFYFLAG"].ToString(), modifyflag_data));
                    rowtemp.CreateCell(20).SetCellValue(dt.Rows[i]["ORDERCODE"].ToString());
                    rowtemp.CreateCell(21).SetCellValue(dt.Rows[i]["CUSNO"].ToString());                 

                }
            }
            #endregion

            #region 特殊区域
            if (busitypeid == "50-51")
            {
                row1.CreateCell(0).SetCellValue("海关状态"); row1.CreateCell(1).SetCellValue("报关单号"); row1.CreateCell(2).SetCellValue("经营单位"); row1.CreateCell(3).SetCellValue("合同发票号");
                row1.CreateCell(4).SetCellValue("申报日期"); row1.CreateCell(5).SetCellValue("运输工具名称");row1.CreateCell(6).SetCellValue("业务类型"); row1.CreateCell(7).SetCellValue("出口口岸"); 
                row1.CreateCell(8).SetCellValue("提运单号"); row1.CreateCell(9).SetCellValue("申报方式"); row1.CreateCell(10).SetCellValue("报关方式"); row1.CreateCell(11).SetCellValue("贸易方式");
                row1.CreateCell(12).SetCellValue("合同协议号"); row1.CreateCell(13).SetCellValue("件数");row1.CreateCell(14).SetCellValue("重量"); row1.CreateCell(15).SetCellValue("张数") ;
                row1.CreateCell(16).SetCellValue("删改单"); row1.CreateCell(17).SetCellValue("订单编号"); row1.CreateCell(18).SetCellValue("客户编号");

                //将数据逐步写入sheet_S各个行
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["CUSTOMSSTATUS"].ToString());
                    rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["DECLARATIONCODE"].ToString());
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CONTRACTNOORDER"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["REPTIME"].ToString());
                    if (dt.Rows[i]["TRANSNAME"].ToString() == "" && dt.Rows[i]["VOYAGENO"].ToString() == "")
                    {
                        rowtemp.CreateCell(5).SetCellValue("");

                    }
                    if (dt.Rows[i]["TRANSNAME"].ToString() == "" && dt.Rows[i]["VOYAGENO"].ToString() != "")
                    {
                        rowtemp.CreateCell(5).SetCellValue("/" + dt.Rows[i]["VOYAGENO"].ToString());

                    }
                    if (dt.Rows[i]["TRANSNAME"].ToString() != "" && dt.Rows[i]["VOYAGENO"].ToString() == "")
                    {
                        rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["TRANSNAME"].ToString());

                    }
                    if (dt.Rows[i]["TRANSNAME"].ToString() != "" && dt.Rows[i]["VOYAGENO"].ToString() != "")
                    {
                        rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["TRANSNAME"].ToString() + "/" + dt.Rows[i]["VOYAGENO"].ToString());

                    }
                    rowtemp.CreateCell(6).SetCellValue(getStatusName(dt.Rows[i]["BUSITYPE"].ToString(), common_data_busitype));
                    rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());
                    rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["BLNO"].ToString());//REPWAYID
                    rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["DECLWAYNAME"].ToString());
                    rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["TRADEMETHOD"].ToString());
                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["GOODSNW"].ToString());
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["SHEETNUM"].ToString());
                    rowtemp.CreateCell(16).SetCellValue(getStatusName(dt.Rows[i]["MODIFYFLAG"].ToString(), modifyflag_data));
                    rowtemp.CreateCell(17).SetCellValue(dt.Rows[i]["ORDERCODE"].ToString());
                    rowtemp.CreateCell(18).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                }
            }
            #endregion

            // 写入到客户端 
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //book.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", "报关单文件.xls");

            return Extension.getPathname("报关单文件.xls", book);
        }
        
        public string LoadDeclarationList_E()
        {
            string sql = QueryConditionDecl_E();          

            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "CREATETIME", "desc"));
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string QueryConditionDecl_E()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string where = "";
            if (!string.IsNullOrEmpty(Request["VALUE1"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION1"])
                {
                    case "REPUNITCODE"://申报单位
                        where += " and lda.REPUNITCODE='" + Request["VALUE1"] + "' ";
                        break;
                    case "PORTCODE"://进出口岸
                        where += " and lda.PORTCODE='" + Request["VALUE1"] + "' ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE5"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION5"])
                {
                    case "REPUNITCODE"://申报单位
                        where += " and lda.REPUNITCODE='" + Request["VALUE5"] + "' ";
                        break;
                    case "PORTCODE"://进出口岸
                        where += " and lda.PORTCODE='" + Request["VALUE5"] + "' ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE2"]))//判断查询条件2是否有值
            {
                switch (Request["CONDITION2"])
                {
                    case "REPNO"://对应号
                        where += " and instr(ort.REPNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "BLNO"://提运单号
                        where += " and instr(lda.BLNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "ORDERCODE"://订单编号
                        where += " and instr(det.ORDERCODE,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "DECLCARNO"://报关车号
                        where += " and instr(ort.DECLCARNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "TRANSNAME"://运输工具名称
                        where += " and (instr(lda.transname,'" + Request["VALUE2"] + "')>0 or instr(lda.voyageno,'" + Request["VALUE2"] + "')>0) ";
                        break;
                    case "DECLNO"://报关单号
                        where += " and instr(lda.DECLARATIONCODE,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "CONTRACTNO"://合同协议号
                        where += " and instr(lda.CONTRACTNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                    case "CONTRACTNOORDER"://合同发票号
                        where += " and instr(ort.CONTRACTNO,'" + Request["VALUE2"] + "')>0 ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE6"]))//判断查询条件2是否有值
            {
                switch (Request["CONDITION6"])
                {
                    case "REPNO"://对应号
                        where += " and instr(ort.REPNO,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "BLNO"://提运单号
                        where += " and instr(lda.BLNO,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "ORDERCODE"://订单编号
                        where += " and instr(det.ORDERCODE,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "DECLCARNO"://报关车号
                        where += " and instr(ort.DECLCARNO,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "TRANSNAME"://运输工具名称
                        where += " and (instr(lda.transname,'" + Request["VALUE6"] + "')>0 or instr(lda.voyageno,'" + Request["VALUE6"] + "')>0) ";
                        break;
                    case "DECLNO"://报关单号
                        where += " and instr(lda.DECLARATIONCODE,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "CONTRACTNO"://合同协议号
                        where += " and instr(lda.CONTRACTNO,'" + Request["VALUE6"] + "')>0 ";
                        break;
                    case "CONTRACTNOORDER"://合同发票号
                        where += " and instr(ort.CONTRACTNO,'" + Request["VALUE6"] + "')>0 ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE3"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION3"])
                {
                    case "BUSITYPE"://业务类型
                        where += " and ort.BUSITYPE='" + Request["VALUE3"] + "' ";
                        break;
                    case "HGZT"://海关状态
                        if (Request["VALUE3"] == "已结关") { where += " and det.CUSTOMSSTATUS='已结关' "; }
                        if (Request["VALUE3"] == "未结关") { where += " and det.CUSTOMSSTATUS!='已结关' "; }
                        break;
                    case "SGD"://删改单
                        where += " and det.modifyflag='" + Request["VALUE3"] + "' ";
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE7"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION7"])
                {
                    case "BUSITYPE"://业务类型
                        where += " and ort.BUSITYPE='" + Request["VALUE7"] + "' ";
                        break;
                    case "HGZT"://海关状态
                        if (Request["VALUE7"] == "已结关") { where += " and det.CUSTOMSSTATUS='已结关' "; }
                        if (Request["VALUE7"] == "未结关") { where += " and det.CUSTOMSSTATUS!='已结关' "; }
                        break;
                    case "SGD"://删改单
                        where += " and det.modifyflag='" + Request["VALUE7"] + "' ";
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
                case "REPTIME"://申报日期
                    if (!string.IsNullOrEmpty(Request["VALUE4_1"]))//如果开始时间有值
                    {
                        where += " and lda.REPTIME>=to_date('" + Request["VALUE4_1"] + "','yyyy-mm-dd hh24:mi:ss') ";//REPSTARTTIME
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE4_2"]))//如果结束时间有值
                    {
                        where += " and lda.REPTIME<=to_date('" + Request["VALUE4_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";//REPSTARTTIME
                    }
                    break;
            }
            switch (Request["CONDITION8"])
            {
                case "SUBMITTIME"://订单委托日期 
                    if (!string.IsNullOrEmpty(Request["VALUE8_1"]))//如果开始时间有值
                    {
                        where += " and ort.SUBMITTIME>=to_date('" + Request["VALUE8_1"] + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE8_2"]))//如果结束时间有值
                    {
                        where += " and ort.SUBMITTIME<=to_date('" + Request["VALUE8_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
                    }
                    break;
                case "REPTIME"://申报日期
                    if (!string.IsNullOrEmpty(Request["VALUE8_1"]))//如果开始时间有值
                    {
                        where += " and lda.REPTIME>=to_date('" + Request["VALUE8_1"] + "','yyyy-mm-dd hh24:mi:ss') ";//REPSTARTTIME
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE8_2"]))//如果结束时间有值
                    {
                        where += " and lda.REPTIME<=to_date('" + Request["VALUE8_2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";//REPSTARTTIME
                    }
                    break;
            }
            where += @" and lda.BUSIUNITCODE ='" + json_user.Value<string>("CUSTOMERHSCODE") + "' ";

//            string sql = @"select det.ID,det.DECLARATIONCODE,det.CODE,ort.CUSTOMERNAME ,det.REPENDTIME REPFINISHTIME, det.CUSTOMSSTATUS ,   
//                           det.CONTRACTNO,det.GOODSNUM,det.GOODSNW,det.SHEETNUM,det.ORDERCODE,det.COSTARTTIME CREATEDATE,
//                           det.TRANSNAME,det.VOYAGENO,det.ISPRINT,
//                           det.BUSIUNITCODE, det.PORTCODE, det.BLNO, det.DECLTYPE, 
//                           ort.REPWAYID ,ort.REPWAYID REPWAYNAME,ort.DECLWAY ,ort.DECLWAY DECLWAYNAME,ort.TRADEWAYCODES ,
//                           ort.CUSNO ,ort.IETYPE,ort.ASSOCIATENO,ort.CORRESPONDNO,ort.BUSIUNITNAME,ort.BUSITYPE, 
//                           cus.SCENEDECLAREID,ort.CONTRACTNO CONTRACTNOORDER ,ort.CREATETIME,ort.REPUNITNAME,ort.REPNO                                                                      
//                           from list_declaration det 
//                                left join list_order ort on det.ordercode = ort.code 
//                                left join cusdoc.sys_customer cus on ort.customercode = cus.code 
//                                left join (
//                                      select ASSOCIATENO from list_order l inner join list_declaration i on l.code=i.ordercode 
//                                      where l.ASSOCIATENO is not null and l.isinvalid=0 and i.isinvalid=0 and (i.STATUS!=130 and i.STATUS!=110)          
//                                      ) a on ort.ASSOCIATENO=a.ASSOCIATENO 
//                                left join (select ordercode from list_declaration ld where ld.isinvalid=0 and ld.STATUS!=130 and ld.STATUS!=110) b on det.ordercode=b.ordercode
//                           where (det.STATUS=130 or det.STATUS=110) and det.isinvalid=0 and ort.isinvalid=0 " + where
//                     + " and a.ASSOCIATENO is null and b.ordercode is  null ";

            string sql = @"select det.ID,det.CODE,det.ORDERCODE, det.CUSTOMSSTATUS ,det.SHEETNUM,det.modifyflag,
                              lda.declarationcode,to_char(lda.reptime,'yyyy-mm-dd') reptime,lda.contractno,lda.goodsnum,lda.goodsnw,lda.blno,
                              lda.transname,lda.voyageno,lda.busiunitcode,lda.busiunitname,lda.portcode,
                              lda.trademethod,lda.declkind DECLWAY,lda.declkind DECLWAYNAME,lda.REPUNITNAME,
                              ort.BUSITYPE,ort.CONTRACTNO CONTRACTNOORDER,ort.REPWAYID,ort.REPWAYID REPWAYNAME,ort.CUSNO,
                              ort.IETYPE,ort.ASSOCIATENO,ort.CORRESPONDNO,ort.customercode,ort.CUSTOMERNAME,ort.CREATETIME, 
                              ort.REPNO    
                           from list_declaration det 
                                left join list_order ort on det.ordercode = ort.code 
								left join list_declaration_after lda on det.code=lda.code and lda.csid=1
                                left join (select ordercode from list_declaration ld where ld.isinvalid=0 and ld.STATUS!=130 and ld.STATUS!=110) a on det.ordercode=a.ordercode
                                left join (
                                      select ASSOCIATENO from list_order l inner join list_declaration i on l.code=i.ordercode 
                                      where l.ASSOCIATENO is not null and l.isinvalid=0 and i.isinvalid=0 and (i.STATUS!=130 and i.STATUS!=110)    
									   ) b on ort.ASSOCIATENO=b.ASSOCIATENO 
                           where (det.STATUS=130 or det.STATUS=110) and det.isinvalid=0 and ort.isinvalid=0 " + where +
                        @"  and a.ordercode is null 
                             and b.ASSOCIATENO is null ";
            return sql;

        }
        
        public string ExportDeclList_E()
        {
            string common_data_busitype = Request["common_data_busitype"]; string modifyflag_data = Request["modifyflag_data"];
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string sql = QueryConditionDecl_E();
            sql = sql + " order by CREATETIME desc";

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
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("报关单信息");

            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("海关状态"); row1.CreateCell(1).SetCellValue("报关单号"); row1.CreateCell(2).SetCellValue("合同发票号"); row1.CreateCell(3).SetCellValue("申报单位");
            row1.CreateCell(4).SetCellValue("申报日期"); row1.CreateCell(5).SetCellValue("运输工具名称"); row1.CreateCell(6).SetCellValue("业务类型"); row1.CreateCell(7).SetCellValue("出口口岸");
            row1.CreateCell(8).SetCellValue("提运单号"); row1.CreateCell(9).SetCellValue("申报方式"); row1.CreateCell(10).SetCellValue("报关方式"); row1.CreateCell(11).SetCellValue("贸易方式");
            row1.CreateCell(12).SetCellValue("合同协议号"); row1.CreateCell(13).SetCellValue("件数"); row1.CreateCell(14).SetCellValue("重量"); row1.CreateCell(15).SetCellValue("张数");
            row1.CreateCell(16).SetCellValue("订单编号"); row1.CreateCell(17).SetCellValue("客户编号"); row1.CreateCell(18).SetCellValue("进/出"); row1.CreateCell(19).SetCellValue("两单关联号");
            row1.CreateCell(20).SetCellValue("多单关联号"); row1.CreateCell(21).SetCellValue("删改单"); 



            //将数据逐步写入sheet_S各个行
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["CUSTOMSSTATUS"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["DECLARATIONCODE"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CONTRACTNOORDER"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["REPUNITNAME"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["REPTIME"].ToString());
                if (dt.Rows[i]["TRANSNAME"].ToString() == "" && dt.Rows[i]["VOYAGENO"].ToString() == "")
                {
                    rowtemp.CreateCell(5).SetCellValue("");

                }
                if (dt.Rows[i]["TRANSNAME"].ToString() == "" && dt.Rows[i]["VOYAGENO"].ToString() != "")
                {
                    rowtemp.CreateCell(5).SetCellValue("/" + dt.Rows[i]["VOYAGENO"].ToString());

                }
                if (dt.Rows[i]["TRANSNAME"].ToString() != "" && dt.Rows[i]["VOYAGENO"].ToString() == "")
                {
                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["TRANSNAME"].ToString());

                }
                if (dt.Rows[i]["TRANSNAME"].ToString() != "" && dt.Rows[i]["VOYAGENO"].ToString() != "")
                {
                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["TRANSNAME"].ToString() + "/" + dt.Rows[i]["VOYAGENO"].ToString());

                }
                rowtemp.CreateCell(6).SetCellValue(getStatusName(dt.Rows[i]["BUSITYPE"].ToString(), common_data_busitype));
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["PORTCODE"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["BLNO"].ToString());
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["REPWAYNAME"].ToString());
                rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["DECLWAYNAME"].ToString());//REPWAYID
                rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["TRADEMETHOD"].ToString());
                rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["GOODSNUM"].ToString());
                rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["GOODSNW"].ToString());
                rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["SHEETNUM"].ToString());
                rowtemp.CreateCell(16).SetCellValue(dt.Rows[i]["ORDERCODE"].ToString());
                rowtemp.CreateCell(17).SetCellValue(dt.Rows[i]["CUSNO"].ToString());
                rowtemp.CreateCell(18).SetCellValue(dt.Rows[i]["IETYPE"].ToString());
                rowtemp.CreateCell(19).SetCellValue(dt.Rows[i]["ASSOCIATENO"].ToString());
                rowtemp.CreateCell(20).SetCellValue(dt.Rows[i]["CORRESPONDNO"].ToString());
                rowtemp.CreateCell(21).SetCellValue(getStatusName(dt.Rows[i]["MODIFYFLAG"].ToString(), modifyflag_data));
            }


            // 写入到客户端 
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //book.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", "报关单文件.xls");

            return Extension.getPathname("报关单文件.xls", book);
        }
                
        public FileResult DownloadFile()
        {
            string WebDownPath = ConfigurationManager.AppSettings["WebDownPath"];
            string path = WebDownPath + Request["filename_s"];

            FileInfo fi = new FileInfo(path);
            if (fi.Exists)
            {
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(Request["filename"]));// + fi.Name
                Response.AddHeader("Content-Length", fi.Length.ToString());
                Response.ContentType = "application/octet-stream";
                Response.Filter.Close();
                Response.WriteFile(path);
                Response.Flush();
                Response.Close();
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                Response.End();
            }

           return new FileStreamResult(Response.OutputStream, "application/vnd.ms-excel");
        }


        public string ExportInspList()
        {
            string dec_insp_status = Request["dec_insp_status"]; string common_data_busitype = Request["common_data_busitype"];
            string common_data_inspmyfs = Request["common_data_inspmyfs"]; string modifyflag_data = Request["modifyflag_data"];
            //string busitypeid = Request["busitypeid"];//导出暂时不用区分业务类型

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string sql = QueryConditionInsp();
            sql = sql + " order by CREATETIME desc";
            DataTable dt = new DataTable();
            try
            {
                DataTable dt_count = DBMgr.GetDataTable("select count(1) from (" + sql + ") a");

                int WebDownCount = Convert.ToInt32(ConfigurationManager.AppSettings["WebDownCount"]);
                if (Convert.ToInt32(dt_count.Rows[0][0]) > WebDownCount)
                {
                    return "{success:false,WebDownCount:" + WebDownCount + "}";
                }

                dt = DBMgr.GetDataTable(sql);
            }
            catch (Exception ex)
            {

            }

            if (dt.Rows.Count <= 0)
            {
                return "{success:false,WebDownCount:0}";
            }

            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();//创建Excel文件的对象            
            NPOI.SS.UserModel.ISheet sheet_S = book.CreateSheet("报检单信息");//添加一个导出成功sheet           
            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0); //给sheet1添加第一行的头部标题

            row1.CreateCell(0).SetCellValue("国检状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("核准单号"); row1.CreateCell(3).SetCellValue("报检单号");
            row1.CreateCell(4).SetCellValue("经营单位"); row1.CreateCell(5).SetCellValue("通关单号"); row1.CreateCell(6).SetCellValue("监管方式"); row1.CreateCell(7).SetCellValue("打印标志");
            row1.CreateCell(8).SetCellValue("张数"); row1.CreateCell(9).SetCellValue("删改单"); row1.CreateCell(10).SetCellValue("业务类型"); row1.CreateCell(11).SetCellValue("是否需通关单");
            row1.CreateCell(12).SetCellValue("是否法检"); row1.CreateCell(13).SetCellValue("委托时间"); row1.CreateCell(14).SetCellValue("合同发票号"); row1.CreateCell(15).SetCellValue("订单编号");
            row1.CreateCell(16).SetCellValue("客户编号");


            //将数据逐步写入sheet_S各个行
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["INSPSTATUS"].ToString());
                rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["STATUS"].ToString(),dec_insp_status));
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["APPROVALCODE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["INSPECTIONCODE"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CLEARANCECODE"].ToString());
                rowtemp.CreateCell(6).SetCellValue(getStatusName(dt.Rows[i]["TRADEWAY"].ToString(), common_data_inspmyfs));
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["ISPRINT"].ToString() == "0" ? "未打印" : "已打印");
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["SHEETNUM"].ToString());
                rowtemp.CreateCell(9).SetCellValue(getStatusName(dt.Rows[i]["MODIFYFLAG"].ToString(), modifyflag_data));
                rowtemp.CreateCell(10).SetCellValue(getStatusName(dt.Rows[i]["BUSITYPE"].ToString(), common_data_busitype));
                rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["ISNEEDCLEARANCE"].ToString() == "0" ? "否" : "是");
                rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "0" ? "否" : "是");
                rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["SUBMITTIME"].ToString());
                rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["ORDERCODE"].ToString());
                rowtemp.CreateCell(16).SetCellValue(dt.Rows[i]["CUSNO"].ToString());
            }

            // 写入到客户端 
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //book.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", "报关单文件.xls");

            return Extension.getPathname("报检单文件.xls", book);
        }

    }
}
