using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
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
    public class CustomerOrderController : Controller
    {
        int totalProperty = 0;

        //
        // GET: /CustomerOrder/

        [Filters.DecodeFilter]
        public ActionResult ListOrder_Index_Cus(string busitypeid)
        {
            switch (busitypeid)
            {
                case "10":
                    ViewBag.navigator = "业务中心>>空运出口";
                    break;
                case "11":
                    ViewBag.navigator = "业务中心>>空运进口";
                    break;
                case "21":
                    ViewBag.navigator = "业务中心>>海运进口";
                    break;
                case "20":
                    ViewBag.navigator = "业务中心>>海运出口";
                    break;
                case "31":
                    ViewBag.navigator = "业务中心>>陆运进口";
                    break;
                case "30":
                    ViewBag.navigator = "业务中心>>陆运出口";
                    break;
                case "40-41":
                    ViewBag.navigator = "业务中心>>国内结转";
                    break;
                case "50-51":
                    ViewBag.navigator = "业务中心>>特殊区域";
                    break;
            }

            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public string LoadList_index_cus()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string sql = QueryCondition();
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "createtime", "desc"));
            var json = JsonConvert.SerializeObject(dt, iso);

            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string QueryCondition()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string busitypeid = Request["busitypeid"];
            string where = "";

            if (!string.IsNullOrEmpty(Request["VALUE1"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION1"])
                {
                    case "DECLSTATUS":
                        if ((Request["VALUE1"] + "") == "草稿")  //草稿=草稿
                        {
                            where += " and DECLSTATUS=0 ";
                        }
                        if ((Request["VALUE1"] + "") == "已委托")  //已委托=已委托
                        {
                            where += " and DECLSTATUS=10 ";
                        }
                        if ((Request["VALUE1"] + "") == "申报中")  //申报中=申报中
                        {
                            where += " and DECLSTATUS=100 ";
                        }
                        if ((Request["VALUE1"] + "") == "申报完结")  //申报完结=申报完结
                        {
                            where += " and DECLSTATUS=130 ";
                        }
                        if ((Request["VALUE1"] + "") == "未完结")  //未完结
                        {
                            where += " and DECLSTATUS<130 ";
                        }
                        break;
                    case "INSPSTATUS":
                        if ((Request["VALUE1"] + "") == "草稿")  //草稿=草稿
                        {
                            where += " and INSPSTATUS=0 ";
                        }
                        if ((Request["VALUE1"] + "") == "已委托")  //已委托=已委托
                        {
                            where += " and INSPSTATUS=10 ";
                        }
                        if ((Request["VALUE1"] + "") == "申报中")  //申报中=申报中
                        {
                            where += " and INSPSTATUS=100 ";
                        }
                        if ((Request["VALUE1"] + "") == "申报完成")  //申报完成=申报完成
                        {
                            where += " and INSPSTATUS>=120 ";
                        }
                        if ((Request["VALUE1"] + "") == "未完成")  //未完结
                        {
                            where += " and INSPSTATUS<120 ";
                        }
                        break;
                    case "LOGISTICSSTATUS":
                        if (!string.IsNullOrEmpty(Request["VALUE1"]))
                        {
                            where += " and to_number(nvl(LOGISTICSSTATUS,0))" + Request["VALUE1"];
                        }
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE2"]))//判断查询条件2是否有值
            {
                switch (Request["CONDITION2"])
                {
                    case "CONTRACTNO":
                        where += " and CONTRACTNO like '%" + Request["VALUE2"] + "%' ";
                        break;
                    case "REPNO":
                        where += " and REPNO like '%" + Request["VALUE2"] + "%' ";
                        break;
                    case "DIVIDENO":
                        where += " and DIVIDENO like '%" + Request["VALUE2"] + "%' ";
                        break;
                    case "MANIFEST":
                        where += " and MANIFEST like '%" + Request["VALUE2"] + "%' ";
                        break;
                    case "SECONDLADINGBILLNO":
                        where += " and SECONDLADINGBILLNO like '%" + Request["VALUE2"] + "%' ";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(Request["VALUE3"]))//判断查询条件2是否有值
            {
                switch (Request["CONDITION3"])
                {
                    case "CUSTOMAREACODE":
                        where += " and CUSTOMAREACODE = '" + Request["VALUE3"] + "' ";
                        break;
                    case "REPWAYID":
                        where += " and REPWAYID = '" + Request["VALUE3"] + "' ";
                        break;
                    case "PORTCODE":
                        where += " and PORTCODE = '" + Request["VALUE3"] + "' ";
                        break;
                }
            }

            switch (Request["CONDITION4"])
            {
                case "SUBMITTIME":
                    if (!string.IsNullOrEmpty(Request["VALUE4_1"]))
                    {
                        where += " and SUBMITTIME>=to_date('" + Request["VALUE4_1"] + "','yyyy-mm-dd hh24:mi:ss')";
                    }
                    if (!string.IsNullOrEmpty(Request["VALUE4_2"]))
                    {
                        where += " and SUBMITTIME<=to_date('" + Request["VALUE4_2"] + "','yyyy-mm-dd hh24:mi:ss')+1";
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(Request["VALUE5"]))//判断查询条件1是否有值
            {
                switch (Request["CONDITION5"])
                {
                    case "DECLSTATUS":
                        if ((Request["VALUE5"] + "") == "草稿")  //草稿=草稿
                        {
                            where += " and DECLSTATUS=0 ";
                        }
                        if ((Request["VALUE5"] + "") == "已委托")  //已委托=已委托
                        {
                            where += " and DECLSTATUS=10 ";
                        }
                        if ((Request["VALUE5"] + "") == "申报中")  //申报中=申报中
                        {
                            where += " and DECLSTATUS=100 ";
                        }
                        if ((Request["VALUE5"] + "") == "申报完结")  //申报完结=申报完结
                        {
                            where += " and DECLSTATUS=130 ";
                        }
                        if ((Request["VALUE5"] + "") == "未完结")  //未完结
                        {
                            where += " and DECLSTATUS<130 ";
                        }
                        break;
                    case "INSPSTATUS":
                        if ((Request["VALUE5"] + "") == "草稿")  //草稿=草稿
                        {
                            where += " and INSPSTATUS=0 ";
                        }
                        if ((Request["VALUE5"] + "") == "已委托")  //已委托=已委托
                        {
                            where += " and INSPSTATUS=10 ";
                        }
                        if ((Request["VALUE5"] + "") == "申报中")  //申报中=申报中
                        {
                            where += " and INSPSTATUS=100 ";
                        }
                        if ((Request["VALUE5"] + "") == "申报完成")  //申报完成=申报完成
                        {
                            where += " and INSPSTATUS>=120 ";
                        }
                        if ((Request["VALUE5"] + "") == "未完成")  //未完结
                        {
                            where += " and INSPSTATUS<120 ";
                        }
                        break;
                    case "LOGISTICSSTATUS":
                        if (!string.IsNullOrEmpty(Request["VALUE5"]))
                        {
                            where += " and to_number(nvl(LOGISTICSSTATUS,0))" + Request["VALUE5"];
                        }
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request["VALUE6"]))//判断查询条件2是否有值
            {
                switch (Request["CONDITION6"])
                {
                    case "CONTRACTNO":
                        where += " and CONTRACTNO like '%" + Request["VALUE6"] + "%' ";
                        break;
                    case "REPNO":
                        where += " and REPNO like '%" + Request["VALUE6"] + "%' ";
                        break;
                    case "DIVIDENO":
                        where += " and DIVIDENO like '%" + Request["VALUE6"] + "%' ";
                        break;
                    case "MANIFEST":
                        where += " and MANIFEST like '%" + Request["VALUE2"] + "%' ";
                        break;
                    case "SECONDLADINGBILLNO":
                        where += " and SECONDLADINGBILLNO like '%" + Request["VALUE6"] + "%' ";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(Request["VALUE7"]))//判断查询条件2是否有值
            {
                switch (Request["CONDITION7"])
                {
                    case "CUSTOMAREACODE":
                        where += " and CUSTOMAREACODE = '" + Request["VALUE7"] + "' ";
                        break;
                    case "REPWAYID":
                        where += " and REPWAYID = '" + Request["VALUE7"] + "' ";
                        break;
                    case "PORTCODE":
                        where += " and PORTCODE = '" + Request["VALUE7"] + "' ";
                        break;
                }
            }

            where += " and ISINVALID=0 ";
            string sql = @"select * from LIST_ORDER where instr('" + busitypeid + "',BUSITYPE)>0 and CUSTOMERCODE='" + json_user.Value<string>("CUSTOMERCODE") + "' " + where;
            return sql;
        }


        public string ExportList()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string dec_insp_status = Request["dec_insp_status"]; string common_data_sbfs = Request["common_data_sbfs"];
            string busitypeid = Request["busitypeid"];

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string sql = QueryCondition();
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
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("对应号");
                row1.CreateCell(4).SetCellValue("客户编号"); row1.CreateCell(5).SetCellValue("合同发票号"); row1.CreateCell(6).SetCellValue("总单号"); row1.CreateCell(7).SetCellValue("分单号");
                row1.CreateCell(8).SetCellValue("件数/重量"); row1.CreateCell(9).SetCellValue("打印状态"); row1.CreateCell(10).SetCellValue("申报关区"); row1.CreateCell(11).SetCellValue("进/出口岸");
                row1.CreateCell(12).SetCellValue("申报方式"); row1.CreateCell(13).SetCellValue("转关预录号"); row1.CreateCell(14).SetCellValue("法检"); row1.CreateCell(15).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["REPNO"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CUSNO"].ToString());


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

                    rowtemp.CreateCell(12).SetCellValue(getStatusName(dt.Rows[i]["REPWAYID"].ToString(), common_data_sbfs));
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["TURNPRENO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());


                }
            }
            #endregion

            #region 10 空出
            if (busitypeid == "10")//空出
            {
                sheet_S = book.CreateSheet("订单信息_空出"); filename = filename + "_空出.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("对应号");
                row1.CreateCell(4).SetCellValue("客户编号"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("总单号"); row1.CreateCell(7).SetCellValue("分单号");
                row1.CreateCell(8).SetCellValue("件数/重量"); row1.CreateCell(9).SetCellValue("打印状态"); row1.CreateCell(10).SetCellValue("申报关区"); row1.CreateCell(11).SetCellValue("进/出口岸");
                row1.CreateCell(12).SetCellValue("申报方式"); row1.CreateCell(13).SetCellValue("运抵编号"); row1.CreateCell(14).SetCellValue("法检"); row1.CreateCell(15).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["REPNO"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

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

                    rowtemp.CreateCell(12).SetCellValue(getStatusName(dt.Rows[i]["REPWAYID"].ToString(), common_data_sbfs));
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["ARRIVEDNO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());
                }
            }
            #endregion

            #region 21 海进
            if (busitypeid == "21")//海进
            {
                sheet_S = book.CreateSheet("订单信息_海进"); filename = filename + "_海进.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("对应号");
                row1.CreateCell(4).SetCellValue("客户编号"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("打印状态"); row1.CreateCell(7).SetCellValue("国检提单号");
                row1.CreateCell(8).SetCellValue("海关提单号"); row1.CreateCell(9).SetCellValue("件数/重量"); row1.CreateCell(10).SetCellValue("申报关区"); row1.CreateCell(11).SetCellValue("进/出口岸");
                row1.CreateCell(12).SetCellValue("申报方式"); row1.CreateCell(13).SetCellValue("转关预录号"); row1.CreateCell(14).SetCellValue("法检"); row1.CreateCell(15).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["REPNO"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

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

                    rowtemp.CreateCell(12).SetCellValue(getStatusName(dt.Rows[i]["REPWAYID"].ToString(), common_data_sbfs));
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["TURNPRENO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());
                }
            }
            #endregion

            #region 20 海出
            if (busitypeid == "20")//海出
            {
                sheet_S = book.CreateSheet("订单信息_海出"); filename = filename + "_海出.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("对应号");
                row1.CreateCell(4).SetCellValue("客户编号"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("提单号"); row1.CreateCell(7).SetCellValue("打印状态");
                row1.CreateCell(8).SetCellValue("运抵编号"); row1.CreateCell(9).SetCellValue("件数/重量"); row1.CreateCell(10).SetCellValue("申报关区"); row1.CreateCell(11).SetCellValue("进/出口岸");
                row1.CreateCell(12).SetCellValue("申报方式"); row1.CreateCell(13).SetCellValue("转关预录号"); row1.CreateCell(14).SetCellValue("法检"); row1.CreateCell(15).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["REPNO"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

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

                    rowtemp.CreateCell(12).SetCellValue(getStatusName(dt.Rows[i]["REPWAYID"].ToString(), common_data_sbfs));
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["TURNPRENO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());
                }
            }
            #endregion

            #region 31 陆进
            if (busitypeid == "31")//陆进
            {
                sheet_S = book.CreateSheet("订单信息_陆进"); filename = filename + "_陆进.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("对应号");
                row1.CreateCell(4).SetCellValue("客户编号"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("分单号"); row1.CreateCell(7).SetCellValue("打印状态");
                row1.CreateCell(8).SetCellValue("件数/重量"); row1.CreateCell(9).SetCellValue("申报关区"); row1.CreateCell(10).SetCellValue("进/出口岸"); row1.CreateCell(11).SetCellValue("申报方式");
                row1.CreateCell(12).SetCellValue("法检"); row1.CreateCell(13).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["REPNO"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CUSNO"].ToString());
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
                    rowtemp.CreateCell(11).SetCellValue(getStatusName(dt.Rows[i]["REPWAYID"].ToString(), common_data_sbfs));

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());
                }
            }
            #endregion

            #region 30 陆出
            if (busitypeid == "30")//陆出
            {
                sheet_S = book.CreateSheet("订单信息_陆出"); filename = filename + "_陆出.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("客户编号");
                row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("总单号"); row1.CreateCell(7).SetCellValue("分单号");
                row1.CreateCell(8).SetCellValue("打印状态"); row1.CreateCell(9).SetCellValue("件数/重量"); row1.CreateCell(10).SetCellValue("申报关区"); row1.CreateCell(11).SetCellValue("进/出口岸");
                row1.CreateCell(12).SetCellValue("申报方式"); row1.CreateCell(13).SetCellValue("运抵编号"); row1.CreateCell(14).SetCellValue("转关预录号"); row1.CreateCell(15).SetCellValue("法检");
                row1.CreateCell(16).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CUSNO"].ToString());
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

                    rowtemp.CreateCell(12).SetCellValue(getStatusName(dt.Rows[i]["REPWAYID"].ToString(), common_data_sbfs));
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["ARRIVEDNO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["TURNPRENO"].ToString());
                    rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(16).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());

                }
            }
            #endregion

            #region 40,41 国内
            if (busitypeid == "40-41")//国内
            {
                sheet_S = book.CreateSheet("订单信息_国内"); filename = filename + "_国内.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("客户编号");
                row1.CreateCell(4).SetCellValue("对应号"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("件数/重量");
                row1.CreateCell(7).SetCellValue("打印状态"); row1.CreateCell(8).SetCellValue("申报关区"); row1.CreateCell(9).SetCellValue("申报方式"); row1.CreateCell(10).SetCellValue("法检");
                row1.CreateCell(11).SetCellValue("业务类型"); row1.CreateCell(12).SetCellValue("两单关联号"); row1.CreateCell(13).SetCellValue("多单关联号"); row1.CreateCell(14).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CUSNO"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["REPNO"].ToString());

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
                    rowtemp.CreateCell(9).SetCellValue(getStatusName(dt.Rows[i]["REPWAYID"].ToString(), common_data_sbfs));
                    rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    if (dt.Rows[i]["BUSITYPE"].ToString() == "40")
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
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["CORRESPONDNO"].ToString());
                    rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());

                }
            }
            #endregion

            #region 50,51 特殊
            if (busitypeid == "50-51")//特殊
            {
                sheet_S = book.CreateSheet("订单信息_特殊"); filename = filename + "_特殊.xls";

                NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
                row1.CreateCell(0).SetCellValue("报关状态"); row1.CreateCell(1).SetCellValue("报检状态"); row1.CreateCell(2).SetCellValue("订单编号"); row1.CreateCell(3).SetCellValue("对应号");
                row1.CreateCell(4).SetCellValue("客户编号"); row1.CreateCell(5).SetCellValue("合同号"); row1.CreateCell(6).SetCellValue("打印状态"); row1.CreateCell(7).SetCellValue("申报方式");
                row1.CreateCell(8).SetCellValue("件数/重量"); row1.CreateCell(9).SetCellValue("申报关区"); row1.CreateCell(10).SetCellValue("进/出口岸"); row1.CreateCell(11).SetCellValue("转关预录号");
                row1.CreateCell(12).SetCellValue("法检"); row1.CreateCell(13).SetCellValue("委托人员");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(getStatusName(dt.Rows[i]["DECLSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(1).SetCellValue(getStatusName(dt.Rows[i]["INSPSTATUS"].ToString(), dec_insp_status));
                    rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["CODE"].ToString());
                    rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["REPNO"].ToString());
                    rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CUSNO"].ToString());

                    rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                    rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["PRINTSTATUS"].ToString() == "1" ? "已打印" : "未打印");
                    rowtemp.CreateCell(7).SetCellValue(getStatusName(dt.Rows[i]["REPWAYID"].ToString(), common_data_sbfs));

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

                    rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["LAWFLAG"].ToString() == "1" ? "有" : "无");
                    rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["SUBMITUSERNAME"].ToString());
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
    }
}
