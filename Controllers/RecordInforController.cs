using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using StackExchange.Redis;
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
    [Filters.AuthFilter]
    public class RecordInforController : Controller
    {
        //
        // GET: /RecordInfor/
        int totalProperty = 0;
        public ActionResult Recordinfo_Detail()//账册信息
        {
            ViewBag.navigator = "备案管理>>账册信息";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult Create()//账册信息 create
        {
            ViewBag.navigator = "备案管理>>账册信息";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult Change()//账册信息 change
        {
            ViewBag.navigator = "备案管理>>账册信息";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult Delete()//账册信息 delete
        {
            ViewBag.navigator = "备案管理>>账册信息";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult Recordinfo_Detail_SUMNUM()//申报数量
        {
            ViewBag.navigator = "备案管理>>申报数量";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult Recordinfo_Detail_SUM_Detail()//申报数量 明细
        {
            ViewBag.navigator = "备案管理>>申报数量";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult Recordinfo_Detail_Audit()//账册审核
        {
            ViewBag.navigator = "客户服务>>账册审核";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult Create_Audit()//账册审核 create
        {
            ViewBag.navigator = "备案管理>>账册审核";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult Change_Audit()//账册审核 change
        {
            ViewBag.navigator = "备案管理>>账册审核";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult Delete_Audit()//账册审核 delete
        {
            ViewBag.navigator = "备案管理>>账册审核";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult PrintRecordDetail()//打印界面
        {
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult VerificationList()//核销比对
        {
            ViewBag.navigator = "备案管理>>核销比对";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }


        #region Recordinfo_Detail
        public string Query_RecordInfor(string type)
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string sql = "", where = "";
            if (!string.IsNullOrEmpty(Request["RECORDINFORID"]))
            {
                where += " and a.CODE='" + Request["RECORDINFORID"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["ITEMNO"]))
            {
                where += " and  b.ITEMNO='" + Request["ITEMNO"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["HSCODE"]))
            {
                where += " and  b.HSCODE='" + Request["HSCODE"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["OPTIONS"]) && type == "go")
            {
                where += " and  b.OPTIONS='" + Request["OPTIONS"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["STATUS"]) && type == "go")
            {
                where += " and  b.STATUS='" + Request["STATUS"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["ITEMNOATTRIBUTE"]))
            {
                where += " and  b.ITEMNOATTRIBUTE='" + Request["ITEMNOATTRIBUTE"] + "'";
            }
            if (type == "go")
            {
                sql = @"select a.code,b.*,c.elements
                        from (select * from cusdoc.sys_recordinfo where enabled=1) a
                            inner join (  
                                    select aa.ID,aa.recordinfoid,aa.itemno,aa.hscode,aa.additionalno,aa.itemnoattribute 
                                        ,aa.commodityname,aa.specificationsmodel,aa.unit,aa.remark
                                        ,aa.options,aa.status,aa.customercode,aa.customername
                                    from sys_recordinfo_detail_task aa 
                                    ) b on a.id=b.recordinfoid 
                            left join (
                                    select hscode,extracode,elements from cusdoc.BASE_COMMODITYHS 
                                    where enabled=1 and yearid=(select id from  cusdoc.base_year where customarea='2300' and enabled=1 and rownum=1)
                                    ) c on b.hscode=c.hscode and b.additionalno=c.extracode";
            }
            else
            {
                sql = @"select a.code,b.*,c.elements
                        from (select * from cusdoc.sys_recordinfo where enabled=1) a
                            inner join (
                                    select aa.ID,aa.recordinfoid,aa.itemno,aa.hscode,aa.additionalno,aa.itemnoattribute 
                                        ,aa.commodityname,aa.specificationsmodel,aa.unit,aa.remark 
                                        ,null options,null status,null customercode,null customername
                                    from cusdoc.sys_recordinfo_detail aa
                                    ) b on a.id=b.recordinfoid 
                            left join (
                                    select hscode,extracode,elements from cusdoc.BASE_COMMODITYHS 
                                    where enabled=1 and yearid=(select id from  cusdoc.base_year where customarea='2300' and enabled=1 and rownum=1)
                                    ) c on b.hscode=c.hscode and b.additionalno=c.extracode";                
            }

            sql = sql + " where a.busiunit='" + json_user.Value<string>("CUSTOMERHSCODE") + "'" + where;

            if (Request["ERROR"].ToString() == "1")
            {
                sql = sql + " and (c.hscode is null or c.extracode is null)";
            }

            return sql;
        }

        public string loadRecordDetail()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string sql = Query_RecordInfor("");
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "recordinfoid,itemno", "asc"));

            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string loadRecordDetail_Go()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string sql = Query_RecordInfor("go");
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "b.id", "desc"));

            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string Delete_Task()
        {
            string ids = Request["ids"];
            string result = "{success:false}"; string sql = "";

            bool bf = false;
            sql = "select * from sys_recordinfo_detail_task where id in(" + ids + ")";
            DataTable dt = DBMgr.GetDataTable(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToInt32(dt.Rows[0]["STATUS"] + "") != 0)
                {
                    bf = true;
                    break;
                }
            }

            if (bf) { return result; }

            sql = "delete from SYS_ELEMENTS where rid in(" + ids + ")";
            DBMgr.ExecuteNonQuery(sql);

            sql = "delete from SYS_PRODUCTCONSUME where rid in(" + ids + ")";
            DBMgr.ExecuteNonQuery(sql);

            sql = "delete from sys_recordinfo_detail_task where id in(" + ids + ")";
            DBMgr.ExecuteNonQuery(sql);

            result = "{success:true}";

            return result;
        }

        public string Repeat_Task()//判断是否存在正在申请的记录
        {
            string id = Request["id"];
            string result = "{success:false}"; string sql = "";

            sql = "select * from sys_recordinfo_detail_task where status<50 and rid='" + id + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                result = "{success:true}";
            }
            return result;
        }

        public string Export()
        {
            string sql = "";string e_options=Request["e_options"];string e_status=Request["e_status"];string e_unit=Request["e_unit"];
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            string filename = "账册信息.xls";

            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            sql = Query_RecordInfor("");
            sql = sql + " order by recordinfoid,itemno";

            DataTable dt_count = DBMgr.GetDataTable("select count(1) from (" + sql + ") a");
            int WebDownCount = Convert.ToInt32(ConfigurationManager.AppSettings["WebDownCount"]);
            if (Convert.ToInt32(dt_count.Rows[0][0]) > WebDownCount)
            {
                return "{success:false,WebDownCount:" + WebDownCount + "}";
            }

            DataTable dt = DBMgr.GetDataTable(sql);
            DataRow[] dr_lj = dt.Select("itemnoattribute='料件'"); DataRow[] dr_cp = dt.Select("itemnoattribute='成品'");

            NPOI.SS.UserModel.ISheet sheet_lj = book.CreateSheet("料件");
            NPOI.SS.UserModel.IRow row1 = sheet_lj.CreateRow(0);
            row1.CreateCell(0).SetCellValue("账册号"); row1.CreateCell(1).SetCellValue("项号"); row1.CreateCell(2).SetCellValue("HS编码"); row1.CreateCell(3).SetCellValue("附加码");
            row1.CreateCell(4).SetCellValue("项号属性"); row1.CreateCell(5).SetCellValue("商品名称"); row1.CreateCell(6).SetCellValue("规格型号"); row1.CreateCell(7).SetCellValue("成交单位");
            row1.CreateCell(8).SetCellValue("备注"); row1.CreateCell(9).SetCellValue("申报要素");

            for (int i = 0; i < dr_lj.Length; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_lj.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dr_lj[i]["CODE"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dr_lj[i]["ITEMNO"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dr_lj[i]["HSCODE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dr_lj[i]["ADDITIONALNO"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dr_lj[i]["ITEMNOATTRIBUTE"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dr_lj[i]["COMMODITYNAME"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dr_lj[i]["SPECIFICATIONSMODEL"].ToString());
                rowtemp.CreateCell(7).SetCellValue(GetName(dr_lj[i]["UNIT"].ToString(), e_unit));
                rowtemp.CreateCell(8).SetCellValue(dr_lj[i]["REMARK"].ToString());
                rowtemp.CreateCell(9).SetCellValue(dr_lj[i]["ELEMENTS"].ToString());
            }

            NPOI.SS.UserModel.ISheet sheet_cp = book.CreateSheet("成品");
            NPOI.SS.UserModel.IRow row2 = sheet_cp.CreateRow(0);
            row2.CreateCell(0).SetCellValue("账册号"); row2.CreateCell(1).SetCellValue("项号"); row2.CreateCell(2).SetCellValue("HS编码"); row2.CreateCell(3).SetCellValue("附加码");
            row2.CreateCell(4).SetCellValue("项号属性"); row2.CreateCell(5).SetCellValue("商品名称"); row2.CreateCell(6).SetCellValue("规格型号"); row2.CreateCell(7).SetCellValue("成交单位");
            row2.CreateCell(8).SetCellValue("备注"); row2.CreateCell(9).SetCellValue("申报要素");

            for (int i = 0; i < dr_cp.Length; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_cp.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dr_cp[i]["CODE"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dr_cp[i]["ITEMNO"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dr_cp[i]["HSCODE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dr_cp[i]["ADDITIONALNO"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dr_cp[i]["ITEMNOATTRIBUTE"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dr_cp[i]["COMMODITYNAME"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dr_cp[i]["SPECIFICATIONSMODEL"].ToString());
                rowtemp.CreateCell(7).SetCellValue(GetName(dr_cp[i]["UNIT"].ToString(), e_unit));
                rowtemp.CreateCell(8).SetCellValue(dr_cp[i]["REMARK"].ToString());
                rowtemp.CreateCell(9).SetCellValue(dr_cp[i]["ELEMENTS"].ToString());
            }

            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            sql = Query_RecordInfor("go");
            sql = sql + " order by b.id desc";

            DataTable dt_go = DBMgr.GetDataTable(sql);
            DataRow[] dr_lj_go = dt_go.Select("itemnoattribute='料件'"); DataRow[] dr_cp_go = dt_go.Select("itemnoattribute='成品'");

            NPOI.SS.UserModel.ISheet sheet_lj_go = book.CreateSheet("料件_申请");
            NPOI.SS.UserModel.IRow row3 = sheet_lj_go.CreateRow(0);
            row3.CreateCell(0).SetCellValue("变动状态"); row3.CreateCell(1).SetCellValue("申请状态"); row3.CreateCell(2).SetCellValue("账册号"); row3.CreateCell(3).SetCellValue("项号");
            row3.CreateCell(4).SetCellValue("HS编码"); row3.CreateCell(5).SetCellValue("附加码"); row3.CreateCell(6).SetCellValue("项号属性"); row3.CreateCell(7).SetCellValue("商品名称");
            row3.CreateCell(8).SetCellValue("规格型号"); row3.CreateCell(9).SetCellValue("成交单位"); row3.CreateCell(10).SetCellValue("备注"); row3.CreateCell(11).SetCellValue("申报要素");

            for (int i = 0; i < dr_lj_go.Length; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_lj_go.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(GetName(dr_lj_go[i]["OPTIONS"].ToString(), e_options));
                rowtemp.CreateCell(1).SetCellValue(GetName(dr_lj_go[i]["STATUS"].ToString(), e_status));
                rowtemp.CreateCell(2).SetCellValue(dr_lj_go[i]["CODE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dr_lj_go[i]["ITEMNO"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dr_lj_go[i]["HSCODE"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dr_lj_go[i]["ADDITIONALNO"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dr_lj_go[i]["ITEMNOATTRIBUTE"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dr_lj_go[i]["COMMODITYNAME"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dr_lj_go[i]["SPECIFICATIONSMODEL"].ToString());
                rowtemp.CreateCell(9).SetCellValue(GetName(dr_lj_go[i]["UNIT"].ToString(), e_unit));
                rowtemp.CreateCell(10).SetCellValue(dr_lj_go[i]["REMARK"].ToString());
                rowtemp.CreateCell(11).SetCellValue(dr_lj_go[i]["ELEMENTS"].ToString());
            }

            NPOI.SS.UserModel.ISheet sheet_cp_go = book.CreateSheet("成品_申请");
            NPOI.SS.UserModel.IRow row4 = sheet_cp_go.CreateRow(0);
            row4.CreateCell(0).SetCellValue("变动状态"); row4.CreateCell(1).SetCellValue("申请状态"); row4.CreateCell(2).SetCellValue("账册号"); row4.CreateCell(3).SetCellValue("项号");
            row4.CreateCell(4).SetCellValue("HS编码"); row4.CreateCell(5).SetCellValue("附加码"); row4.CreateCell(6).SetCellValue("项号属性"); row4.CreateCell(7).SetCellValue("商品名称");
            row4.CreateCell(8).SetCellValue("规格型号"); row4.CreateCell(9).SetCellValue("成交单位"); row4.CreateCell(10).SetCellValue("备注"); row4.CreateCell(11).SetCellValue("申报要素");

            for (int i = 0; i < dr_cp_go.Length; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_cp_go.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(GetName(dr_cp_go[i]["OPTIONS"].ToString(), e_options));
                rowtemp.CreateCell(1).SetCellValue(GetName(dr_cp_go[i]["STATUS"].ToString(),e_status));
                rowtemp.CreateCell(2).SetCellValue(dr_cp_go[i]["CODE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dr_cp_go[i]["ITEMNO"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dr_cp_go[i]["HSCODE"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dr_cp_go[i]["ADDITIONALNO"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dr_cp_go[i]["ITEMNOATTRIBUTE"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dr_cp_go[i]["COMMODITYNAME"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dr_cp_go[i]["SPECIFICATIONSMODEL"].ToString());
                rowtemp.CreateCell(9).SetCellValue(GetName(dr_cp_go[i]["UNIT"].ToString(), e_unit));
                rowtemp.CreateCell(10).SetCellValue(dr_cp_go[i]["REMARK"].ToString());
                rowtemp.CreateCell(11).SetCellValue(dr_cp_go[i]["ELEMENTS"].ToString());
            }
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // 写入到客户端 
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //book.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", filename);

            return Extension.getPathname(filename, book);
        }

        public string loadRecrodInfo()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string sql = @"select * from cusdoc.sys_recordinfo where busiunit='" + json_user.Value<string>("CUSTOMERHSCODE") + "'";

            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "ENABLED", "desc"));
            var json = JsonConvert.SerializeObject(dt);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string RecordSet_task()
        {
            string ids = Request["ids"]; string type = Request["type"];
            string result = "{success:false}"; string sql = "";

            sql = "update sys_recordinfo set ENABLED=" + type + " where id in(" + ids + ")";
            DBMgrBase.ExecuteNonQuery(sql);

            result = "{success:true}";

            return result;
        }

        #endregion

        #region Recorninfo_Common

        public string Ini_Base_Data_Itemno_Consume()
        {
            string sql = "";
            string json_itemno_consume = "[]";//对应料件序号

            sql = @"select a.*,b.name as unitname from sys_recordinfo_detail a 
                        left join base_declproductunit b on a.unit=b.code WHERE a.recordinfoid={0} and a.itemnoattribute='料件'
                    order by a.itemno";
            sql = string.Format(sql, Request["RECORDINFOID"] + "");

            json_itemno_consume = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

            return "{itemno_consume:" + json_itemno_consume + "}";
        }

        public string cancel()
        {
            string id = Request["id"];
            string result = "{success:false}"; string sql = "";

            sql = "select * from sys_recordinfo_detail_task where id ='" + id + "'";
            DataTable dt = DBMgr.GetDataTable(sql);

            if (Convert.ToInt32(dt.Rows[0]["STATUS"] + "") == 0)
            {
                return result;
            }

            sql = @"update sys_recordinfo_detail_task set STATUS = 0,SUBMITID=null,SUBMITTIME=null,SUBMITNAME=null 
                        ,ACCEPTTIME=null,ACCEPTID=null,ACCEPTNAME=null,PRETIME=null,PREID=null,PRENAME=null
                        ,REPTIME=null,REPID=null,REPNAME=null,FINISHTIME=null,FINISHID=null,FINISHNAME=null 
                    where id ='" + id + "'";
            DBMgr.ExecuteNonQuery(sql);

            result = "{success:true}";
            return result;
        }

        public string GetElements()
        {
            string customarea = Request["customarea"].ToString(); string hscode = Request["hscode"].ToString(); 
            string additionalno = Request["additionalno"].ToString(); string id = Request["id"].ToString();
            string sql = string.Empty; string json = "[]";

            string flag = "";
            if (id == "") { flag = "1"; }//查总库
            else
            {
                sql = @"select * from sys_recordinfo_detail_task where id='" + id + "'";
                DataTable dt = DBMgr.GetDataTable(sql);
                string hscode_database = dt.Rows[0]["HSCODE"].ToString(); string customarea_database = dt.Rows[0]["CUSTOMAREA"].ToString(); string additionalno_database = dt.Rows[0]["ADDITIONALNO"].ToString();
                if (hscode_database != hscode || customarea_database != customarea || additionalno_database != additionalno)//修改了这两个字段
                {
                    flag = "1";//查总库
                }
            }

            if (flag == "1")//查总库
            {
                sql = @"select regexp_substr(elements,'[^;]+',1,level,'i') elements,'' descriptions 
                    from (select elements from cusdoc.BASE_COMMODITYHS where hscode='{0}' and yearid={1} and extracode='{2}') t1
                    connect by level <= length(elements) - length(replace(elements,';',''))";
                sql = string.Format(sql, hscode, customarea, additionalno);
                json = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
            }
            else
            {
                sql = "select functiontype as elements,descriptions from SYS_ELEMENTS where rid='" + id + "' order by sno";
                json = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
            }           

            return "{elements:" + json + "}";
        }

        //申报要素  
        public string update_elements(JObject json, JObject json_user, string id)
        {
            string sql = "";
            //先清空
            sql = @"delete SYS_ELEMENTS where RID='" + id + "'";
            DBMgr.ExecuteNonQuery(sql);

            if (json.Value<string>("jsonEle") != null)
            {
                //在插入
                string jsonEle = json.Value<string>("jsonEle").Substring(("{elements:").Length);
                jsonEle = jsonEle.Substring(0, jsonEle.Length - 1);
                JArray je = (JArray)JsonConvert.DeserializeObject(jsonEle);
                for (int i = 0; i < je.Count; i++)
                {
                    sql = @"insert into SYS_ELEMENTS(ID,RECORDINFOID,ITEMNO,ITEMNOATTRIBUTE,SNO,FUNCTIONTYPE,DESCRIPTIONS,CREATEMAN,CREATEDATE,RID) 
                            values(SYS_ELEMENTS_id.Nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}',sysdate,'{7}')";
                    sql = string.Format(sql, json.Value<string>("RECORDINFOID"), json.Value<string>("ITEMNO"), json.Value<string>("ITEMNOATTRIBUTE"), i, je[i].Value<string>("ELEMENTS")
                        , json.Value<string>("field_ele_" + i), json_user.Value<string>("ID"), id);
                    DBMgr.ExecuteNonQuery(sql);
                }
            }
            return "success";
        }

        //成品单耗
        public string update_productconsume(JObject json, JObject json_user, string id)
        {
            string sql = "";
            //先清空
            sql = @"delete SYS_PRODUCTCONSUME where RID='" + id + "'";
            DBMgr.ExecuteNonQuery(sql);

            if (json.Value<string>("ITEMNOATTRIBUTE") == "成品")
            {
                //在插入
                JArray ja = (JArray)JsonConvert.DeserializeObject(Request["productconsume"]);
                for (int j = 0; j < ja.Count; j++)
                {
                    sql = @"insert into SYS_PRODUCTCONSUME(ID,RECORDINFOID,ITEMNO,ITEMNO_CONSUME,ITEMNO_COMMODITYNAME,ITEMNO_SPECIFICATIONSMODEL,ITEMNO_UNIT,
                                        ITEMNO_UNITNAME,CONSUME,ATTRITIONRATE,CREATEMAN,CREATEDATE,RID) 
                                    values(SYS_PRODUCTCONSUME_id.Nextval,'{0}','{1}','{2}','{3}','{4}','{5}'
                                    ,'{6}','{7}','{8}','{9}',sysdate,'{10}')";
                    sql = string.Format(sql, json.Value<string>("RECORDINFOID"), json.Value<string>("ITEMNO"), ja[j].Value<string>("ITEMNO_CONSUME"), ja[j].Value<string>("ITEMNO_COMMODITYNAME"), ja[j].Value<string>("ITEMNO_SPECIFICATIONSMODEL"), ja[j].Value<string>("ITEMNO_UNIT")
                        , ja[j].Value<string>("ITEMNO_UNITNAME"), ja[j].Value<string>("CONSUME"), ja[j].Value<string>("ATTRITIONRATE"), json_user.Value<string>("ID"), id
                        );
                    DBMgr.ExecuteNonQuery(sql);
                }
            }
            return "success";
        }
        
        #endregion

        #region form_create

        public string GetCommodityHS()
        {
            string json_hscode = "[]";
            string sql = @"select distinct hscode from BASE_COMMODITYHS where yearid= '" + Request["CUSTOMAREA"] + "' order by hscode";
            json_hscode = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
            return "{hscode:" + json_hscode + "}";
        }
        public string GetCommodityEXTRA()
        {
            string json_extra = "[]";
            string sql = @"select EXTRACODE from BASE_COMMODITYHS where yearid= '" + Request["CUSTOMAREA"] + "' and hscode= '" + Request["HSCODE"] + "' order by EXTRACODE";
            json_extra = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
            return "{extra:" + json_extra + "}";
        }

        public string loadrecord_create()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string id = Request["id"]; string copyid = Request["copyid"];
            string sql = "";
            string result = "{}"; string formdata = "{}"; string productsonsumedata = "[]";
            if (string.IsNullOrEmpty(id))
            {
                if (string.IsNullOrEmpty(copyid))
                {
                    string RECORDINFOID = "", CUSTOMAREA = ""; 
                    //账册号
                    sql = @"select id from cusdoc.sys_recordinfo where enabled=1 and busiunit= '" + json_user.Value<string>("CUSTOMERHSCODE") + "' order by id";
                    DataTable dt_temp = DBMgr.GetDataTable(sql);
                    if (dt_temp.Rows.Count > 0) { RECORDINFOID = dt_temp.Rows[0][0].ToString(); }

                    //备案关区
                    sql = @"select id from cusdoc.base_year where customarea is not null and enabled=1 order by id";
                    DataTable dt_temp2 = DBMgr.GetDataTable(sql);
                    if (dt_temp2.Rows.Count > 0) { CUSTOMAREA = dt_temp2.Rows[0][0].ToString(); }

                    formdata = "{ITEMNOATTRIBUTE:'料件',STATUS:0,OPTIONS:'A',ISPRINT_APPLY:0,RECORDINFOID:" + RECORDINFOID + ",CUSTOMAREA:" + CUSTOMAREA + "}";
                }
                else//如果是复制新增
                {
                    IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                    iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                    sql = @"select RECORDINFOID,CUSTOMAREA,CUSTOMERCODE,CUSTOMERNAME,ITEMNOATTRIBUTE from sys_recordinfo_detail_task where id='" + copyid + "'";
                    formdata = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql), iso).TrimStart('[').TrimEnd(']');
                }
            }
            else
            {
                IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                sql = @"select * from sys_recordinfo_detail_task where id='" + id + "'";
                formdata = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql), iso).TrimStart('[').TrimEnd(']');

                //成品单耗
                sql = "select * from SYS_PRODUCTCONSUME where rid='" + id + "' order by id desc";
                productsonsumedata = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql), iso);

            }
            result = "{formdata:" + formdata + ",productsonsumedata:" + productsonsumedata + "}";
            return result;            
        }

        public string Create_Save()
        {
            string action = Request["action"];            
            JObject json = (JObject)JsonConvert.DeserializeObject(Request["formdata"]);           
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string sql = ""; string resultmsg = "{success:false}";
            /*
            //验证项号是否重复----------------------------------------------------------------------------------------
            if (json.Value<string>("ITEMNO") != null)
            {
                sql = "select itemno from SYS_RECORDINFO_DETAIL_TASK where STATUS<50 and RECORDINFOID='{0}' and ITEMNO='{1}' and ITEMNOATTRIBUTE='{2}'";
                if (!string.IsNullOrEmpty(Request["id"])) { sql = sql + " and ID!='{3}'"; }
                sql += " union select itemno from cusdoc.SYS_RECORDINFO_DETAIL where enabled=1 and RECORDINFOID='{0}' and ITEMNO='{1}' and ITEMNOATTRIBUTE='{2}'";

                sql = string.Format(sql, json.Value<string>("RECORDINFOID"), json.Value<string>("ITEMNO"), json.Value<string>("ITEMNOATTRIBUTE"), Request["id"]);
                DataTable dt_itemno = new DataTable();
                dt_itemno = DBMgr.GetDataTable(sql);
                if (dt_itemno.Rows.Count > 0) { resultmsg = "{success:false,isrepeate:'Y'}"; return resultmsg; }
            }           
            //-----------------------------------------------------------------------------------------------------------
            */

            //验证项号是否是最大值----------------------------------------------------------------------------------------

            //新增无条件；修改：先判断项号是否修改，若没修改，不做验证；若修改，除去本笔记录做验证；

            bool bf = false;
            if (!string.IsNullOrEmpty(Request["id"]))
            {
                sql = "select itemno,ITEMNOATTRIBUTE,RECORDINFOID from SYS_RECORDINFO_DETAIL_TASK where ID='{0}'"; sql = string.Format(sql, Request["id"]);
                DataTable dt_itemno_e = DBMgr.GetDataTable(sql);
                if (dt_itemno_e.Rows[0][0].ToString() == json.Value<string>("ITEMNO") && dt_itemno_e.Rows[0][1].ToString() == json.Value<string>("ITEMNOATTRIBUTE")
                     && dt_itemno_e.Rows[0][2].ToString() == json.Value<string>("RECORDINFOID"))
                {
                    bf = true;//修改：先判断项号、项号属性、账册号是否修改，若没修改，不做验证；
                }
            }
            if (bf == false)
            {
                sql = "select itemno from SYS_RECORDINFO_DETAIL_TASK where STATUS<50 and RECORDINFOID='{0}' and ITEMNO>=to_number('{1}') and ITEMNOATTRIBUTE='{2}'";
                if (!string.IsNullOrEmpty(Request["id"])) { sql = sql + " and ID!='{3}'"; }
                sql += " union select itemno from cusdoc.SYS_RECORDINFO_DETAIL where enabled=1 and RECORDINFOID='{0}' and ITEMNO>=to_number('{1}') and ITEMNOATTRIBUTE='{2}'";

                sql = string.Format(sql, json.Value<string>("RECORDINFOID"), json.Value<string>("ITEMNO"), json.Value<string>("ITEMNOATTRIBUTE"), Request["id"]);
                DataTable dt_itemno = new DataTable();
                dt_itemno = DBMgr.GetDataTable(sql);
                if (dt_itemno.Rows.Count > 0) { resultmsg = "{success:false,ismax:'Y'}"; return resultmsg; }
            }
            //-----------------------------------------------------------------------------------------------------------

            string id = string.Empty;
            if (Request["action"] + "" == "submit")
            {
                json.Remove("STATUS"); json.Add("STATUS", 10);
                json.Remove("SUBMITTIME"); json.Add("SUBMITTIME", "sysdate");
                json.Remove("SUBMITNAME"); json.Add("SUBMITNAME", json_user.Value<string>("REALNAME"));
                json.Remove("SUBMITID"); json.Add("SUBMITID", json_user.Value<string>("ID"));
            }
            else
            {
                json.Remove("SUBMITTIME"); //委托时间  
                json.Add("SUBMITTIME", "null");
            }

            if (string.IsNullOrEmpty(Request["id"]))//新增
            {
                sql = "select SYS_RECORDINFO_DETAIL_TASK_ID.Nextval from dual";
                id = DBMgr.GetDataTable(sql).Rows[0][0] + "";//获取ID
                sql = @"INSERT INTO SYS_RECORDINFO_DETAIL_TASK (ID
                        ,RECORDINFOID,ITEMNO,HSCODE,ADDITIONALNO,ITEMNOATTRIBUTE
                        ,COMMODITYNAME,SPECIFICATIONSMODEL,UNIT,REMARK,CUSTOMAREA
                        ,CREATEID,CREATENAME,CREATEDATE,OPTIONS,STATUS,CUSTOMERCODE
                        ,CUSTOMERNAME,SUBMITID,SUBMITNAME,SUBMITTIME                       
                        ) VALUES ('{0}'
                            ,'{1}','{2}','{3}','{4}','{5}'
                            ,'{6}','{7}','{8}','{9}','{10}'
                            ,'{11}','{12}',sysdate,'{13}','{14}','{15}'
                            ,'{16}','{17}','{18}',{19}
                            )";
                sql = string.Format(sql, id
                    , json.Value<string>("RECORDINFOID"), json.Value<string>("ITEMNO"), json.Value<string>("HSCODE"), json.Value<string>("ADDITIONALNO"), json.Value<string>("ITEMNOATTRIBUTE")
                    , json.Value<string>("COMMODITYNAME"), json.Value<string>("SPECIFICATIONSMODEL"), json.Value<string>("UNIT"), json.Value<string>("REMARK"), json.Value<string>("MODIFYREASON")
                    , json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"), 'A', json.Value<string>("STATUS"), json.Value<string>("CUSTOMERCODE")
                    , json.Value<string>("CUSTOMERNAME"), json.Value<string>("SUBMITID"), json.Value<string>("SUBMITNAME"), json.Value<string>("SUBMITTIME"), json.Value<string>("CUSTOMAREA")
                    );
            }
            else//修改
            {
                id = Request["id"];
                sql = @"UPDATE SYS_RECORDINFO_DETAIL_TASK SET RECORDINFOID='{1}',ITEMNO='{2}',HSCODE='{3}',ADDITIONALNO='{4}',ITEMNOATTRIBUTE='{5}' 
                            ,COMMODITYNAME='{6}',SPECIFICATIONSMODEL='{7}',UNIT='{8}',REMARK='{9}',CUSTOMAREA='{10}'
                            ,OPTIONS='{11}',STATUS='{12}',CUSTOMERCODE='{13}',CUSTOMERNAME='{14}',SUBMITID='{15}'
                            ,SUBMITNAME='{16}',SUBMITTIME={17}
                        WHERE ID={0}";
                sql = string.Format(sql, id
                    , json.Value<string>("RECORDINFOID"), json.Value<string>("ITEMNO"), json.Value<string>("HSCODE"), json.Value<string>("ADDITIONALNO"), json.Value<string>("ITEMNOATTRIBUTE")
                    , json.Value<string>("COMMODITYNAME"), json.Value<string>("SPECIFICATIONSMODEL"), json.Value<string>("UNIT"), json.Value<string>("REMARK"), json.Value<string>("CUSTOMAREA")
                    , 'A', json.Value<string>("STATUS"), json.Value<string>("CUSTOMERCODE"), json.Value<string>("CUSTOMERNAME"), json.Value<string>("SUBMITID")
                    , json.Value<string>("SUBMITNAME"), json.Value<string>("SUBMITTIME")
                    );
            }
            if (sql != "")
            {
                int result = DBMgr.ExecuteNonQuery(sql);
                if (result == 1)
                {
                    update_elements(json, json_user, id);//申报要素  
                    update_productconsume(json, json_user, id);//成品单耗

                    resultmsg = "{success:true,id:'" + id + "'}";
                }
            }
            return resultmsg;
        }

        #endregion

        #region form_change

        public string loadrecord_change()
        {
            string id = Request["id"]; string rid = Request["rid"];
            string sql = ""; DataTable dt = new DataTable();
            string result = "{}"; string formdata = "{}"; string productsonsumedata = "[]";
            if (string.IsNullOrEmpty(id))
            {
                if (!string.IsNullOrEmpty(rid))//如果是变动申请
                {
                    IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                    iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                    string CUSTOMAREA = "";
                    //备案关区
                    sql = @"select id from cusdoc.base_year where customarea is not null and enabled=1 order by id";
                    DataTable dt_temp = DBMgrBase.GetDataTable(sql);
                    if (dt_temp.Rows.Count > 0) { CUSTOMAREA = dt_temp.Rows[0][0].ToString(); }


                    sql = @"select '{0}' RID,RECORDINFOID,ITEMNO,ITEMNOATTRIBUTE,{1} CUSTOMAREA
                                ,HSCODE,ADDITIONALNO,COMMODITYNAME,SPECIFICATIONSMODEL,UNIT
                                ,HSCODE HSCODE_LEFT,ADDITIONALNO ADDITIONALNO_LEFT,COMMODITYNAME COMMODITYNAME_LEFT,SPECIFICATIONSMODEL SPECIFICATIONSMODEL_LEFT,UNIT UNIT_LEFT   
                            from sys_recordinfo_detail where id='{0}'";
                    sql = string.Format(sql, rid, CUSTOMAREA);
                    dt = DBMgrBase.GetDataTable(sql);
                    formdata = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');
                }
            }
            else
            {
                IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                sql = @"select * from sys_recordinfo_detail_task where id='" + id + "'";
                dt = DBMgr.GetDataTable(sql);

                formdata = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');

                //成品单耗
                sql = "select * from SYS_PRODUCTCONSUME where rid='" + id + "' order by id desc";
                productsonsumedata = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql), iso);

            }
            result = "{formdata:" + formdata + ",productsonsumedata:" + productsonsumedata + "}";
            return result;
        }

        public string Change_Save()
        {
            string action = Request["action"];
            JObject json = (JObject)JsonConvert.DeserializeObject(Request["formdata"]);
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string sql = ""; string resultmsg = "{success:false}";

            string id = string.Empty;
            if (Request["action"] + "" == "submit")
            {
                json.Remove("STATUS"); json.Add("STATUS", 10);
                json.Remove("SUBMITTIME"); json.Add("SUBMITTIME", "sysdate");
                json.Remove("SUBMITNAME"); json.Add("SUBMITNAME", json_user.Value<string>("REALNAME"));
                json.Remove("SUBMITID"); json.Add("SUBMITID", json_user.Value<string>("ID"));
            }
            else
            {
                json.Remove("SUBMITTIME"); //委托时间  
                json.Add("SUBMITTIME", "null");
            }

            if (string.IsNullOrEmpty(Request["id"]))//新增
            {
                //验证是否存在同比的记录正在申请中--------------------------------------------------------------
                sql = "select * from sys_recordinfo_detail_task where status<50 and rid='" + json.Value<string>("RID") + "'";
                DataTable dt = DBMgr.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    resultmsg = "{success:false,isgoing:'Y'}";
                    return resultmsg;
                }
                //-------------------------------------------------------------------------------------------------


                sql = "select SYS_RECORDINFO_DETAIL_TASK_ID.Nextval from dual";
                id = DBMgr.GetDataTable(sql).Rows[0][0] + "";//获取ID
                sql = @"INSERT INTO SYS_RECORDINFO_DETAIL_TASK (ID
                        ,RECORDINFOID,ITEMNO,HSCODE,ADDITIONALNO,ITEMNOATTRIBUTE
                        ,COMMODITYNAME,SPECIFICATIONSMODEL,UNIT,REMARK,MODIFYREASON
                        ,CREATEID,CREATENAME,CREATEDATE,OPTIONS,STATUS,CUSTOMERCODE
                        ,CUSTOMERNAME,SUBMITID,SUBMITNAME,SUBMITTIME,CUSTOMAREA
                        ,HSCODE_LEFT,ADDITIONALNO_LEFT,COMMODITYNAME_LEFT,SPECIFICATIONSMODEL_LEFT,UNIT_LEFT
                        ,RID                       
                        ) VALUES ('{0}'
                            ,'{1}','{2}','{3}','{4}','{5}'
                            ,'{6}','{7}','{8}','{9}','{10}'
                            ,'{11}','{12}',sysdate,'{13}','{14}','{15}'
                            ,'{16}','{17}','{18}',{19},'{20}'
                            ,'{21}','{22}','{23}','{24}','{25}'
                            ,'{26}'
                            )";
                sql = string.Format(sql, id
                    , json.Value<string>("RECORDINFOID"), json.Value<string>("ITEMNO"), json.Value<string>("HSCODE"), json.Value<string>("ADDITIONALNO"), json.Value<string>("ITEMNOATTRIBUTE")
                    , json.Value<string>("COMMODITYNAME"), json.Value<string>("SPECIFICATIONSMODEL"), json.Value<string>("UNIT"), json.Value<string>("REMARK"), json.Value<string>("MODIFYREASON")
                    , json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"), 'U', json.Value<string>("STATUS"), json.Value<string>("CUSTOMERCODE")
                    , json.Value<string>("CUSTOMERNAME"), json.Value<string>("SUBMITID"), json.Value<string>("SUBMITNAME"), json.Value<string>("SUBMITTIME"), json.Value<string>("CUSTOMAREA")
                    , json.Value<string>("HSCODE_LEFT"), json.Value<string>("ADDITIONALNO_LEFT"), json.Value<string>("COMMODITYNAME_LEFT"), json.Value<string>("SPECIFICATIONSMODEL_LEFT"), json.Value<string>("UNIT_LEFT")
                    , json.Value<string>("RID")
                    );
            }
            else//修改
            {
                id = Request["id"];
                sql = @"UPDATE SYS_RECORDINFO_DETAIL_TASK SET HSCODE='{1}',ADDITIONALNO='{2}',COMMODITYNAME='{3}',SPECIFICATIONSMODEL='{4}',UNIT='{5}'
                            ,CUSTOMERCODE='{6}',CUSTOMERNAME='{7}',CUSTOMAREA='{8}',REMARK='{9}',MODIFYREASON='{10}'
                            ,OPTIONS='{11}',STATUS='{12}',SUBMITID='{13}',SUBMITNAME='{14}',SUBMITTIME={15}
                        WHERE ID={0}";
                sql = string.Format(sql, id
                    , json.Value<string>("HSCODE"), json.Value<string>("ADDITIONALNO"), json.Value<string>("COMMODITYNAME"), json.Value<string>("SPECIFICATIONSMODEL"), json.Value<string>("UNIT")
                    , json.Value<string>("CUSTOMERCODE"), json.Value<string>("CUSTOMERNAME"), json.Value<string>("CUSTOMAREA"), json.Value<string>("REMARK"), json.Value<string>("MODIFYREASON")
                    , 'U', json.Value<string>("STATUS"), json.Value<string>("SUBMITID"), json.Value<string>("SUBMITNAME"), json.Value<string>("SUBMITTIME")                    
                    );
            }
            if (sql != "")
            {
                int result = DBMgr.ExecuteNonQuery(sql);
                if (result == 1)
                {
                    update_elements(json, json_user, id);//申报要素  
                    update_productconsume(json, json_user, id);//成品单耗

                    resultmsg = "{success:true,id:'" + id + "'}";
                }
            }
            return resultmsg;
        }

        #endregion

        #region form_delete

        public string loadrecord_delete()
        {
            string id = Request["id"]; string rid = Request["rid"];
            string sql = ""; DataTable dt = new DataTable();
            string result = "{}"; string formdata = "{}"; string productsonsumedata = "[]";
            if (string.IsNullOrEmpty(id))
            {
                if (!string.IsNullOrEmpty(rid))//如果是删除申请
                {
                    IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                    iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                    sql = @"select '{0}' RID,RECORDINFOID,ITEMNO,ITEMNOATTRIBUTE,HSCODE,ADDITIONALNO,COMMODITYNAME,SPECIFICATIONSMODEL,UNIT  
                            from sys_recordinfo_detail where id='{0}'";
                    sql = string.Format(sql, rid);
                    dt = DBMgrBase.GetDataTable(sql);
                    formdata = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');
                }
            }
            else
            {
                IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                sql = @"select * from sys_recordinfo_detail_task where id='" + id + "'";
                dt = DBMgr.GetDataTable(sql);

                formdata = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');

                //成品单耗
                sql = "select * from SYS_PRODUCTCONSUME where rid='" + id + "' order by id desc";
                productsonsumedata = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql), iso);

            }
            result = "{formdata:" + formdata + ",productsonsumedata:" + productsonsumedata + "}";
            return result;
        }

        public string Delete_Save()
        {
            string action = Request["action"];
            JObject json = (JObject)JsonConvert.DeserializeObject(Request["formdata"]);
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string sql = ""; string resultmsg = "{success:false}";

            string id = string.Empty;
            if (Request["action"] + "" == "submit")
            {
                json.Remove("STATUS"); json.Add("STATUS", 10);
                json.Remove("SUBMITTIME"); json.Add("SUBMITTIME", "sysdate");
                json.Remove("SUBMITNAME"); json.Add("SUBMITNAME", json_user.Value<string>("REALNAME"));
                json.Remove("SUBMITID"); json.Add("SUBMITID", json_user.Value<string>("ID"));
            }
            else
            {
                json.Remove("SUBMITTIME"); //委托时间  
                json.Add("SUBMITTIME", "null");
            }

            if (string.IsNullOrEmpty(Request["id"]))//新增
            {
                //验证是否存在同比的记录正在申请中--------------------------------------------------------------
                sql = "select * from sys_recordinfo_detail_task where status<50 and rid='" + json.Value<string>("RID") + "'";
                DataTable dt = DBMgr.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    resultmsg = "{success:false,isgoing:'Y'}";
                    return resultmsg;
                }
                //-------------------------------------------------------------------------------------------------


                sql = "select SYS_RECORDINFO_DETAIL_TASK_ID.Nextval from dual";
                id = DBMgr.GetDataTable(sql).Rows[0][0] + "";//获取ID
                sql = @"INSERT INTO SYS_RECORDINFO_DETAIL_TASK (ID
                        ,RECORDINFOID,ITEMNO,HSCODE,ADDITIONALNO,ITEMNOATTRIBUTE
                        ,COMMODITYNAME,SPECIFICATIONSMODEL,UNIT,REMARK,MODIFYREASON
                        ,CREATEID,CREATENAME,CREATEDATE,OPTIONS,STATUS,CUSTOMERCODE
                        ,CUSTOMERNAME,SUBMITID,SUBMITNAME,SUBMITTIME,CUSTOMAREA
                        ,RID                       
                        ) VALUES ('{0}'
                            ,'{1}','{2}','{3}','{4}','{5}'
                            ,'{6}','{7}','{8}','{9}','{10}'
                            ,'{11}','{12}',sysdate,'{13}','{14}','{15}'
                            ,'{16}','{17}','{18}',{19},'{20}'
                            ,'{21}'
                            )";
                sql = string.Format(sql, id
                    , json.Value<string>("RECORDINFOID"), json.Value<string>("ITEMNO"), json.Value<string>("HSCODE"), json.Value<string>("ADDITIONALNO"), json.Value<string>("ITEMNOATTRIBUTE")
                    , json.Value<string>("COMMODITYNAME"), json.Value<string>("SPECIFICATIONSMODEL"), json.Value<string>("UNIT"), json.Value<string>("REMARK"), json.Value<string>("MODIFYREASON")
                    , json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"), 'D', json.Value<string>("STATUS"), json.Value<string>("CUSTOMERCODE")
                    , json.Value<string>("CUSTOMERNAME"), json.Value<string>("SUBMITID"), json.Value<string>("SUBMITNAME"), json.Value<string>("SUBMITTIME"), json.Value<string>("CUSTOMAREA")
                    , json.Value<string>("RID")
                    );
            }
            else//修改
            {
                id = Request["id"];
                sql = @"UPDATE SYS_RECORDINFO_DETAIL_TASK SET CUSTOMERCODE='{1}',CUSTOMERNAME='{2}',CUSTOMAREA='{3}',REMARK='{4}',MODIFYREASON='{5}'
                            ,OPTIONS='{6}',STATUS='{7}',SUBMITID='{8}',SUBMITNAME='{9}',SUBMITTIME={10} 
                        WHERE ID={0}";
                sql = string.Format(sql, id                    
                    , json.Value<string>("CUSTOMERCODE"), json.Value<string>("CUSTOMERNAME"), json.Value<string>("CUSTOMAREA"), json.Value<string>("REMARK"), json.Value<string>("MODIFYREASON")
                    , 'D', json.Value<string>("STATUS"), json.Value<string>("SUBMITID"), json.Value<string>("SUBMITNAME"), json.Value<string>("SUBMITTIME")
                    );
            }
            if (sql != "")
            {
                int result = DBMgr.ExecuteNonQuery(sql);
                if (result == 1)
                {
                    update_elements(json, json_user, id);//申报要素  
                    update_productconsume(json, json_user, id);//成品单耗

                    resultmsg = "{success:true,id:'" + id + "'}";
                }
            }
            return resultmsg;
        }

        #endregion

        #region Recordinfor_Audit
        public string GetRecordidByEnterprise()
        {
            string json_recordid = "[]";//账册号
            string sql = @"select id,code,code||'('||bookattribute||')' as name from sys_recordinfo where enabled=1 and busiunit= '" + Request["EnterpriseHSCOCDE"] + "'";
            json_recordid = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
            return "{recordid:" + json_recordid + "}";
        }

        public string Query_RecordInfor_Audit()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string sql = ""; string where = "";
            if (!string.IsNullOrEmpty(Request["ENTERPRISECODE"]))
            {
                where += " and a.busiunit='" + Request["ENTERPRISECODE"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["RECORDINFORID"]))
            {
                where += " and a.CODE='" + Request["RECORDINFORID"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["ITEMNO"]))
            {
                where += " and  b.ITEMNO='" + Request["ITEMNO"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["HSCODE"]))
            {
                where += " and  b.HSCODE='" + Request["HSCODE"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["ITEMNOATTRIBUTE"]))
            {
                where += " and  b.ITEMNOATTRIBUTE='" + Request["ITEMNOATTRIBUTE"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["OPTIONS"]))
            {
                where += " and  b.OPTIONS='" + Request["OPTIONS"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["STATUS"]))
            {
                where += " and  b.STATUS='" + Request["STATUS"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["DATE_START"]))//如果开始时间有值
            {
                where += " and b.SUBMITTIME>=to_date('" + Request["DATE_START"] + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            if (!string.IsNullOrEmpty(Request["DATE_END"]))//如果结束时间有值
            {
                where += " and b.SUBMITTIME<=to_date('" + Request["DATE_END"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            sql = @"select a.code,a.busiunit,b.*,c.name busiunitname 
                            from (select * from cusdoc.sys_recordinfo where enabled=1) a
                                 inner join (  
                                         select aa.ID,aa.recordinfoid,aa.itemno,aa.hscode,aa.additionalno,aa.itemnoattribute 
                                                ,aa.commodityname,aa.specificationsmodel,aa.unit,aa.remark
                                                ,aa.options,aa.status,aa.customercode,aa.customername,aa.submittime 
                                         from sys_recordinfo_detail_task aa where aa.status>=10 
                                    ) b on a.id=b.recordinfoid 
                                left join (select code,name from cusdoc.base_company where code is not null and enabled=1) c on a.busiunit=c.code";
            sql = sql + " where b.customercode='" + json_user.Value<string>("CUSTOMERHSCODE") + "'" + where;
            return sql;
        }

        public string loadRecordDetail_Audit_Go()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string sql = Query_RecordInfor_Audit();
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "b.id", "desc"));

            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string Export_Audit()
        {
            string sql = ""; string e_options = Request["e_options"]; string e_status = Request["e_status"]; string e_unit = Request["e_unit"];
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            string filename = "账册信息.xls";            

            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            sql = Query_RecordInfor_Audit();         
            sql = sql + " order by b.id desc";

            DataTable dt_count = DBMgr.GetDataTable("select count(1) from (" + sql + ") a");
            int WebDownCount = Convert.ToInt32(ConfigurationManager.AppSettings["WebDownCount"]);
            if (Convert.ToInt32(dt_count.Rows[0][0]) > WebDownCount)
            {
                return "{success:false,WebDownCount:" + WebDownCount + "}";
            }

            DataTable dt_go = DBMgr.GetDataTable(sql);
            DataRow[] dr_lj_go = dt_go.Select("itemnoattribute='料件'"); DataRow[] dr_cp_go = dt_go.Select("itemnoattribute='成品'");

            NPOI.SS.UserModel.ISheet sheet_lj_go = book.CreateSheet("料件_申请");
            NPOI.SS.UserModel.IRow row3 = sheet_lj_go.CreateRow(0);
            row3.CreateCell(0).SetCellValue("变动状态"); row3.CreateCell(1).SetCellValue("申请状态"); row3.CreateCell(2).SetCellValue("账册号"); row3.CreateCell(3).SetCellValue("项号");
            row3.CreateCell(4).SetCellValue("HS编码"); row3.CreateCell(5).SetCellValue("附加码"); row3.CreateCell(6).SetCellValue("项号属性"); row3.CreateCell(7).SetCellValue("商品名称");
            row3.CreateCell(8).SetCellValue("规格型号"); row3.CreateCell(9).SetCellValue("成交单位"); row3.CreateCell(10).SetCellValue("委托企业"); row3.CreateCell(11).SetCellValue("提交时间");
            row3.CreateCell(12).SetCellValue("备注");

            for (int i = 0; i < dr_lj_go.Length; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_lj_go.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(GetName(dr_lj_go[i]["OPTIONS"].ToString(), e_options));
                rowtemp.CreateCell(1).SetCellValue(GetName(dr_lj_go[i]["STATUS"].ToString(), e_status));
                rowtemp.CreateCell(2).SetCellValue(dr_lj_go[i]["CODE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dr_lj_go[i]["ITEMNO"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dr_lj_go[i]["HSCODE"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dr_lj_go[i]["ADDITIONALNO"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dr_lj_go[i]["ITEMNOATTRIBUTE"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dr_lj_go[i]["COMMODITYNAME"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dr_lj_go[i]["SPECIFICATIONSMODEL"].ToString());
                rowtemp.CreateCell(9).SetCellValue(GetName(dr_lj_go[i]["UNIT"].ToString(), e_unit));
                rowtemp.CreateCell(10).SetCellValue(dr_lj_go[i]["BUSIUNITNAME"].ToString());
                rowtemp.CreateCell(11).SetCellValue(dr_lj_go[i]["SUBMITTIME"].ToString());
                rowtemp.CreateCell(12).SetCellValue(dr_lj_go[i]["REMARK"].ToString());
            }

            NPOI.SS.UserModel.ISheet sheet_cp_go = book.CreateSheet("成品_申请");
            NPOI.SS.UserModel.IRow row4 = sheet_cp_go.CreateRow(0);
            row4.CreateCell(0).SetCellValue("变动状态"); row4.CreateCell(1).SetCellValue("申请状态"); row4.CreateCell(2).SetCellValue("账册号"); row4.CreateCell(3).SetCellValue("项号");
            row4.CreateCell(4).SetCellValue("HS编码"); row4.CreateCell(5).SetCellValue("附加码"); row4.CreateCell(6).SetCellValue("项号属性"); row4.CreateCell(7).SetCellValue("商品名称");
            row4.CreateCell(8).SetCellValue("规格型号"); row4.CreateCell(9).SetCellValue("成交单位"); row4.CreateCell(10).SetCellValue("委托企业"); row4.CreateCell(11).SetCellValue("提交时间");
            row4.CreateCell(12).SetCellValue("备注");

            for (int i = 0; i < dr_cp_go.Length; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_cp_go.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(GetName(dr_cp_go[i]["OPTIONS"].ToString(), e_options));
                rowtemp.CreateCell(1).SetCellValue(GetName(dr_cp_go[i]["STATUS"].ToString(), e_status));
                rowtemp.CreateCell(2).SetCellValue(dr_cp_go[i]["CODE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dr_cp_go[i]["ITEMNO"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dr_cp_go[i]["HSCODE"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dr_cp_go[i]["ADDITIONALNO"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dr_cp_go[i]["ITEMNOATTRIBUTE"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dr_cp_go[i]["COMMODITYNAME"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dr_cp_go[i]["SPECIFICATIONSMODEL"].ToString());
                rowtemp.CreateCell(9).SetCellValue(GetName(dr_cp_go[i]["UNIT"].ToString(), e_unit));
                rowtemp.CreateCell(10).SetCellValue(dr_cp_go[i]["BUSIUNITNAME"].ToString());
                rowtemp.CreateCell(11).SetCellValue(dr_cp_go[i]["SUBMITTIME"].ToString());
                rowtemp.CreateCell(12).SetCellValue(dr_cp_go[i]["REMARK"].ToString());
            }
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // 写入到客户端 
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //book.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", filename);

            return Extension.getPathname(filename, book);
        }

        #endregion


        #region form_Audit

        public string loadrecord_Audit()
        {
            string id = Request["id"];
            string sql = "";

            string result = "{}"; string formdata = "{}"; string productsonsumedata = "[]";

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            //委托信息
            sql = @"select a.*,b.busiunit enterprise,b.code||'('||b.bookattribute||')' recordinfoname
                    from sys_recordinfo_detail_task a 
                        left join (select * from cusdoc.sys_recordinfo where enabled=1) b on a.recordinfoid=b.id 
                    where a.id='" + id + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            formdata = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');

            //成品单耗
            sql = "select * from SYS_PRODUCTCONSUME where rid='" + id + "' order by id desc";
            productsonsumedata = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql), iso);

            result = "{formdata:" + formdata + ",productsonsumedata:" + productsonsumedata + "}";
            return result;
        }

        public string Save_Audit()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            int id = Convert.ToInt32(Request["id"].ToString()); int status = Convert.ToInt32(Request["status"]);
            string options = Request["options"]; int nextid = 0; int flag_parms;

            string resultmsg = "{success:false,id:'" + id + "'}";

            string sql = "";
            if (options == "A")
            {
                sql = "select SYS_RECORDINFO_DETAIL_ID.Nextval from dual";
                nextid = Convert.ToInt32(DBMgrBase.GetDataTable(sql).Rows[0][0] + "");
            }

            OracleParameter[] parms = new OracleParameter[7];
            parms[0] = new OracleParameter("p_id", OracleDbType.Int32, ParameterDirection.Input);parms[1] = new OracleParameter("p_status", OracleDbType.Int32, ParameterDirection.Input);
            parms[2] = new OracleParameter("p_nextid", OracleDbType.Int32, ParameterDirection.Input); parms[3] = new OracleParameter("p_options", OracleDbType.NVarchar2, 20, ParameterDirection.Input);
            parms[4] = new OracleParameter("p_userid", OracleDbType.Int32, ParameterDirection.Input); parms[5] = new OracleParameter("p_username", OracleDbType.NVarchar2, 50, ParameterDirection.Input);
            parms[6] = new OracleParameter("p_flag_parms", OracleDbType.Int32, ParameterDirection.Output);

            parms[0].Value = id; parms[1].Value = status; parms[2].Value = nextid; parms[3].Value = options;
            parms[4].Value = json_user.Value<string>("ID"); parms[5].Value = json_user.Value<string>("REALNAME");

            DBMgr.ExecuteNonQueryParm("Pro_Save_Audit", parms);
            flag_parms = Convert.ToInt32(parms[6].Value.ToString());

            if (flag_parms == 0)
            {
                resultmsg = "{success:true,id:'" + id + "'}";
            }
            return resultmsg;

           
        }

        #endregion

        #region Recordinfo_SUM

        public string Query_RecordDetail_SUM()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string sql = ""; string where = "";
            if (!string.IsNullOrEmpty(Request["RECORDINFORID"]))
            {
                where += " and a.recordcode='" + Request["RECORDINFORID"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["ITEMNO"]))
            {
                where += " and b.ITEMNO='" + Request["ITEMNO"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["DATE_START"]))//如果开始时间有值
            {
                where += " and a.reptime>=to_date('" + Request["DATE_START"] + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            if (!string.IsNullOrEmpty(Request["DATE_END"]))//如果结束时间有值
            {
                where += " and a.reptime<=to_date('" + Request["DATE_END"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            if (!string.IsNullOrEmpty(Request["INOUT_TYPE"]))
            {
                where += " and substr(a.declarationcode,9,1)='" + Request["INOUT_TYPE"] + "'";
            }

            sql = @"select aa.recordcode,aa.itemno,aa.internaltype,aa.internaltypename,aa.trademethod,aa.commodityname,aa.currency
                           ,bb.isproductname ITEMNOATTRIBUTE,aa.cadunit,sum(aa.cadquantity) as cadquantity,sum(aa.totalprice) as totalprice 
                    from(
                        select substr(a.declarationcode,9,1) internaltype
             	                ,case when substr(a.declarationcode,9,1)='1' then '进口' when substr(a.declarationcode,9,1)='0' then '出口' else '' end internaltypename
                                ,a.trademethod,a.recordcode,to_number(b.itemno) itemno,b.cadquantity,b.cadunit,b.commodityname,b.currency,b.totalprice,a.reptime 
                        from list_declaration_after a 
                            inner join list_decllist_after b on a.code=b.predeclcode and a.xzlb=b.xzlb 
                            inner join list_declaration c on a.code=c.code 
                        where a.csid=1 and (c.modifyflag<>1 or (c.modifyflag=1 and c.customsstatus<>'删单或异常')) 
                            and a.busiunitcode='" + json_user.Value<string>("CUSTOMERHSCODE") + "'" + where
                    + @") aa
                        left join cusdoc.base_booksdata bb on aa.trademethod=bb.trade and aa.internaltypename=bb.isinportname
                    group by aa.recordcode,aa.itemno,aa.internaltype,aa.internaltypename,aa.trademethod,bb.isproductname,aa.commodityname,aa.currency,aa.cadunit";

            return sql;
        }

        public string loadRecordDetail_SUM()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            string sql = Query_RecordDetail_SUM();
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "aa.recordcode,aa.itemno,aa.internaltype", "asc"));

            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string Query_RecordDetail_SUM_D()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = ""; string where = "";

            if (!string.IsNullOrEmpty(Request["f_field_declartioncode"]))
            {
                where += " and a.declarationcode='" + Request["f_field_declartioncode"] + "'";
            }

            //=====================================================================================================
            if (!string.IsNullOrEmpty(Request["f_field_recordid"]))
            {
                where += " and a.recordcode='" + Request["f_field_recordid"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["f_field_ITEMNO"]))
            {
                where += " and b.itemno='" + Request["f_field_ITEMNO"] + "'";
            }
            where += " and b.commodityname='" + Request["f_field_COMMODITYNAME"] + "'"
                    + " and a.trademethod='" + Request["f_field_TRADEMETHOD"] + "'"
                    + " and b.cadunit='" + Request["f_combo_UNIT"] + "'"
                    + " and b.currency='" + Request["f_field_CURRENCY"] + "'";

            if (!string.IsNullOrEmpty(Request["f_date_start"]))//如果开始时间有值
            {
                where += " and a.reptime>=to_date('" + Request["f_date_start"] + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            if (!string.IsNullOrEmpty(Request["f_date_end"]))//如果结束时间有值
            {
                where += " and a.reptime<=to_date('" + Request["f_date_end"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
            }

            sql = @"select aa.recordcode,aa.itemno,aa.internaltype,aa.internaltypename,aa.trademethod,aa.commodityname,aa.currency
                           ,bb.isproductname ITEMNOATTRIBUTE,aa.cadunit,cadquantity,aa.totalprice
                           ,aa.reptime,aa.declarationcode,aa.legalquantity,aa.legalunit,aa.repunitname,aa.commodityno,aa.transmodel
                           ,(select ee.Name from cusdoc.base_Transport ee where aa.transmodel=ee.code) as transmodelname
                           ,aa.customsstatus,aa.modifyflag,aa.dataconfirm 
                    from(
                          select substr(a.declarationcode,9,1) internaltype
             	                    ,case when substr(a.declarationcode,9,1)='1' then '进口' when substr(a.declarationcode,9,1)='0' then '出口' else '' end internaltypename
                                 ,a.trademethod,a.recordcode,b.itemno,b.cadquantity,b.cadunit,b.commodityno,b.commodityname,b.currency,b.totalprice
                                 ,a.reptime,a.declarationcode,b.legalquantity,b.legalunit,a.repunitname,a.transmodel
                                 ,c.customsstatus,c.modifyflag,c.dataconfirm 
                          from list_declaration_after a 
                                inner join list_decllist_after b on a.code=b.predeclcode and a.xzlb=b.xzlb 
                                inner join list_declaration c on a.code=c.code 
                          where a.csid=1 and (c.modifyflag<>1 or (c.modifyflag=1 and c.customsstatus<>'删单或异常'))
                                and a.busiunitcode='" + json_user.Value<string>("CUSTOMERHSCODE") + "'" + where
                    + @") aa
                      left join cusdoc.base_booksdata bb on aa.trademethod=bb.trade and aa.internaltypename=bb.isinportname "
                    + @" where aa.internaltypename='" + Request["f_field_inout_type"] + "'";

            if (!string.IsNullOrEmpty(Request["f_field_ITEMNOATTRIBUTE"]))
            {
                sql += " and bb.isproductname='" + Request["f_field_ITEMNOATTRIBUTE"] + "'";
            }
            //=====================================================================================================

            return sql;
        }

        public string loadRecordDetail_SUM_D()
        {
            string sql = Query_RecordDetail_SUM_D();
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "aa.reptime desc,aa.declarationcode", "desc"));

            IsoDateTimeConverter iso = new IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd";      
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string Export_SUM()
        {
            string sql = ""; string UNIT = Request["UNIT"];
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            string filename = "申报数量.xls";

            sql = Query_RecordDetail_SUM();
            sql = sql + " order by aa.recordcode,aa.itemno,aa.internaltype";

            DataTable dt_count = DBMgr.GetDataTable("select count(1) from (" + sql + ") a");
            int WebDownCount = Convert.ToInt32(ConfigurationManager.AppSettings["WebDownCount"]);
            if (Convert.ToInt32(dt_count.Rows[0][0]) > WebDownCount)
            {
                return "{success:false,WebDownCount:" + WebDownCount + "}";
            }

            DataTable dt = DBMgr.GetDataTable(sql);

            NPOI.SS.UserModel.ISheet sheet = book.CreateSheet("申报数量");

            NPOI.SS.UserModel.IRow row1 = sheet.CreateRow(0);
            row1.CreateCell(0).SetCellValue("账册号"); row1.CreateCell(1).SetCellValue("项号"); row1.CreateCell(2).SetCellValue("进出类型"); row1.CreateCell(3).SetCellValue("贸易方式");
            row1.CreateCell(4).SetCellValue("项号属性"); row1.CreateCell(5).SetCellValue("商品名称"); row1.CreateCell(6).SetCellValue("成交数量"); row1.CreateCell(7).SetCellValue("成交总价");
            row1.CreateCell(8).SetCellValue("成交单位"); row1.CreateCell(9).SetCellValue("币别");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["RECORDCODE"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["ITEMNO"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["INTERNALTYPENAME"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["TRADEMETHOD"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["ITEMNOATTRIBUTE"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["COMMODITYNAME"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["CADQUANTITY"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["TOTALPRICE"].ToString());
                rowtemp.CreateCell(8).SetCellValue(GetName(dt.Rows[i]["CADUNIT"].ToString(), UNIT));
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["CURRENCY"].ToString());
            }
            
            // 写入到客户端 
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //book.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", filename);

            return Extension.getPathname(filename, book);
        }

        public string Export_SUM_D()
        {
            string sql = ""; string UNIT = Request["UNIT"]; string modifyflag_data = Request["modifyflag_data"];
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            string filename = "申报数量明细.xls";

            sql = Query_RecordDetail_SUM_D();
            sql = sql + " order by aa.reptime desc,aa.declarationcode desc";

            DataTable dt_count = DBMgr.GetDataTable("select count(1) from (" + sql + ") a");
            int WebDownCount = Convert.ToInt32(ConfigurationManager.AppSettings["WebDownCount"]);
            if (Convert.ToInt32(dt_count.Rows[0][0]) > WebDownCount)
            {
                return "{success:false,WebDownCount:" + WebDownCount + "}";
            }

            DataTable dt = DBMgr.GetDataTable(sql);
            NPOI.SS.UserModel.ISheet sheet = book.CreateSheet("申报数量明细");
            NPOI.SS.UserModel.IRow row1 = sheet.CreateRow(0);
            row1.CreateCell(0).SetCellValue("申报单位"); row1.CreateCell(1).SetCellValue("海关状态"); row1.CreateCell(2).SetCellValue("报关单号"); row1.CreateCell(3).SetCellValue("成交数量");
            row1.CreateCell(4).SetCellValue("成交总价"); row1.CreateCell(5).SetCellValue("申报时间"); row1.CreateCell(6).SetCellValue("HS编码"); row1.CreateCell(7).SetCellValue("法定数量");
            row1.CreateCell(8).SetCellValue("法定单位"); row1.CreateCell(9).SetCellValue("运输方式"); row1.CreateCell(10).SetCellValue("删改单"); row1.CreateCell(11).SetCellValue("数据确认");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["REPUNITNAME"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["CUSTOMSSTATUS"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["DECLARATIONCODE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CADQUANTITY"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["TOTALPRICE"].ToString());
                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["REPTIME"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["COMMODITYNO"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["LEGALQUANTITY"].ToString());
                rowtemp.CreateCell(8).SetCellValue(GetName(dt.Rows[i]["LEGALUNIT"].ToString(), UNIT));
                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["TRANSMODELNAME"].ToString());
                rowtemp.CreateCell(10).SetCellValue(GetName(dt.Rows[i]["MODIFYFLAG"].ToString(), modifyflag_data));
                rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["DATACONFIRM"].ToString() == "2" ? "是" : "否");
            }

            // 写入到客户端 
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //book.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", filename);

            return Extension.getPathname(filename, book);
        }

        public string DownReport_detail()
        {
            string UNIT = Request["UNIT"]; string busitype = Request["busitype"]; string modifyflag_data = Request["modifyflag_data"];
            string DATE_START = Request["DATE_START"]; string DATE_END = Request["DATE_END"];
            string INOUT_TYPE = Request["INOUT_TYPE"]; string RBGTYPE = Request["RBGTYPE"]; string RECORDINFORID = Request["RECORDINFORID"];
            

            int WebDownCount = Convert.ToInt32(ConfigurationManager.AppSettings["WebDownCount"]);
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            string filename = "统一数据表.xls";

            OracleParameter[] parms = new OracleParameter[9];

            parms[0] = new OracleParameter("WebDownCount", OracleDbType.Int32, WebDownCount, ParameterDirection.Input);
            parms[1] = new OracleParameter("busiunitcode", OracleDbType.NVarchar2, json_user.Value<string>("CUSTOMERHSCODE"), ParameterDirection.Input);
            parms[2] = new OracleParameter("rbgtype", OracleDbType.NVarchar2, RBGTYPE, ParameterDirection.Input);
            parms[3] = new OracleParameter("inout_type", OracleDbType.NVarchar2, INOUT_TYPE, ParameterDirection.Input);
            parms[4] = new OracleParameter("date_start", OracleDbType.NVarchar2, DATE_START, ParameterDirection.Input);
            parms[5] = new OracleParameter("date_end", OracleDbType.NVarchar2, DATE_END, ParameterDirection.Input);
            parms[6] = new OracleParameter("recordinforid", OracleDbType.NVarchar2, RECORDINFORID, ParameterDirection.Input);

            parms[7] = new OracleParameter("p_flag_parms", OracleDbType.Varchar2, 20, null, ParameterDirection.Output);//输出参数，字符串类型的，一定要设定大小
            parms[8] = new OracleParameter("rescur", OracleDbType.RefCursor, ParameterDirection.Output);

            DataTable dt = DBMgr.GetDataTableParm("Pro_RecordDetail_Report", parms);
            string p_flag_parms = parms[7].Value.ToString();

            if (p_flag_parms == "N")
            {
                return "{success:false,WebDownCount:" + WebDownCount + "}";
            }

            if (RBGTYPE == "0")//昆山区内
            {
                NPOI.SS.UserModel.ISheet sheet = book.CreateSheet("昆山区内");
                NPOI.SS.UserModel.IRow row1 = sheet.CreateRow(0);
                row1.CreateCell(0).SetCellValue("合同号"); row1.CreateCell(1).SetCellValue("业务类型"); row1.CreateCell(2).SetCellValue("进出类型"); row1.CreateCell(3).SetCellValue("申报日期");
                row1.CreateCell(4).SetCellValue("报关单号"); row1.CreateCell(5).SetCellValue("贸易方式"); row1.CreateCell(6).SetCellValue("序号"); row1.CreateCell(7).SetCellValue("项号属性");
                row1.CreateCell(8).SetCellValue("项号"); row1.CreateCell(9).SetCellValue("品名"); row1.CreateCell(10).SetCellValue("规格型号"); row1.CreateCell(11).SetCellValue("成交数量"); 
                row1.CreateCell(12).SetCellValue("成交单位"); row1.CreateCell(13).SetCellValue("成交金额");row1.CreateCell(14).SetCellValue("币制"); row1.CreateCell(15).SetCellValue("海关状态");
                row1.CreateCell(16).SetCellValue("账册号"); row1.CreateCell(17).SetCellValue("删改单"); row1.CreateCell(18).SetCellValue("数据确认"); row1.CreateCell(19).SetCellValue("比对状态");
                row1.CreateCell(20).SetCellValue("未通过原因"); 
               

                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        NPOI.SS.UserModel.IRow rowtemp = sheet.CreateRow(i + 1);
                        rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                        rowtemp.CreateCell(1).SetCellValue(GetName(dt.Rows[i]["BUSITYPE"].ToString(), busitype));
                        rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["INTERNALTYPENAME"].ToString());
                        rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["REPTIME"].ToString());
                        rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["DECLARATIONCODE"].ToString());
                        rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["TRADEMETHOD"].ToString());
                        rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["ORDERNO"].ToString());
                        rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["ITEMNOATTRIBUTE"].ToString());
                        rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["ITEMNO"].ToString());
                        rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["COMMODITYNAME"].ToString());
                        rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["SPECIFICATIONSMODEL"].ToString());
                        rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["CADQUANTITY"].ToString());
                        rowtemp.CreateCell(12).SetCellValue(GetName(dt.Rows[i]["CADUNIT"].ToString(), UNIT));
                        rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["TOTALPRICE"].ToString());
                        rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["CURRENCY"].ToString());
                        rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["CUSTOMSSTATUS"].ToString());
                        rowtemp.CreateCell(16).SetCellValue(dt.Rows[i]["RECORDCODE"].ToString());
                        rowtemp.CreateCell(17).SetCellValue(GetName(dt.Rows[i]["MODIFYFLAG"].ToString(), modifyflag_data));
                        rowtemp.CreateCell(18).SetCellValue(dt.Rows[i]["DATACONFIRM"].ToString() == "2" ? "是" : "否");
                        rowtemp.CreateCell(19).SetCellValue(dt.Rows[i]["VERSTATUS"].ToString());
                        rowtemp.CreateCell(20).SetCellValue(dt.Rows[i]["NOTE"].ToString());
                    }
                }
            }

            if (RBGTYPE == "1")//昆山区外
            {
                NPOI.SS.UserModel.ISheet sheet = book.CreateSheet("昆山区外");
                NPOI.SS.UserModel.IRow row1 = sheet.CreateRow(0);
                row1.CreateCell(0).SetCellValue("备案号"); row1.CreateCell(1).SetCellValue("企业内部编号"); row1.CreateCell(2).SetCellValue("报关单号"); row1.CreateCell(3).SetCellValue("申报日期");
                row1.CreateCell(4).SetCellValue("退单日期"); row1.CreateCell(5).SetCellValue("贸易方式"); row1.CreateCell(6).SetCellValue("成品料件标志"); row1.CreateCell(7).SetCellValue("毛重(公斤)");
                row1.CreateCell(8).SetCellValue("净重(公斤)"); row1.CreateCell(9).SetCellValue("序号"); row1.CreateCell(10).SetCellValue("项号"); row1.CreateCell(11).SetCellValue("料号");
                row1.CreateCell(12).SetCellValue("商品名称"); row1.CreateCell(13).SetCellValue("规格型号"); row1.CreateCell(14).SetCellValue("申报数量"); row1.CreateCell(15).SetCellValue("单位");
                row1.CreateCell(16).SetCellValue("法定数量"); row1.CreateCell(17).SetCellValue("法定单位"); row1.CreateCell(18).SetCellValue("第二法定数量"); row1.CreateCell(19).SetCellValue("第二法定单位");
                row1.CreateCell(20).SetCellValue("原产国"); row1.CreateCell(21).SetCellValue("币制"); row1.CreateCell(22).SetCellValue("单价"); row1.CreateCell(23).SetCellValue("总价");
                row1.CreateCell(24).SetCellValue("折合美元总价"); row1.CreateCell(25).SetCellValue("汇率"); row1.CreateCell(26).SetCellValue("备注1"); row1.CreateCell(27).SetCellValue("备注2");
                row1.CreateCell(28).SetCellValue("备注3"); row1.CreateCell(29).SetCellValue("比对状态"); row1.CreateCell(30).SetCellValue("未通过原因"); 


                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        NPOI.SS.UserModel.IRow rowtemp = sheet.CreateRow(i + 1);
                        rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["RECORDCODE"].ToString());
                        rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["CONTRACTNO"].ToString());
                        rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["DECLARATIONCODE"].ToString());
                        rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["REPTIME"].ToString());
                        //rowtemp.CreateCell(4).SetCellValue("");
                        rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["TRADEMETHOD"].ToString());
                        if (dt.Rows[i]["ITEMNOATTRIBUTE"].ToString() == "料件")
                        {
                            rowtemp.CreateCell(6).SetCellValue("3");
                        }
                        else if (dt.Rows[i]["ITEMNOATTRIBUTE"].ToString() == "成品")
                        {
                            rowtemp.CreateCell(6).SetCellValue("4");
                        }
                        else
                        {
                            rowtemp.CreateCell(6).SetCellValue("");
                        }
                        //rowtemp.CreateCell(7).SetCellValue("");
                        //rowtemp.CreateCell(8).SetCellValue("");
                        rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["ORDERNO"].ToString());
                        rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["ITEMNO"].ToString());
                        //rowtemp.CreateCell(11).SetCellValue("");
                        rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["COMMODITYNAME"].ToString());
                        rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["SPECIFICATIONSMODEL"].ToString());
                        rowtemp.CreateCell(14).SetCellValue(dt.Rows[i]["CADQUANTITY"].ToString());
                        rowtemp.CreateCell(15).SetCellValue(dt.Rows[i]["CADUNIT"].ToString());//GetName(dt.Rows[i]["CADUNIT"].ToString(), UNIT)

                        rowtemp.CreateCell(16).SetCellValue(dt.Rows[i]["LEGALQUANTITY"].ToString());
                        rowtemp.CreateCell(17).SetCellValue(dt.Rows[i]["LEGALUNIT"].ToString());
                        rowtemp.CreateCell(18).SetCellValue(dt.Rows[i]["SQUANTITY"].ToString());
                        rowtemp.CreateCell(19).SetCellValue(dt.Rows[i]["SUNIT"].ToString());

                        if (dt.Rows[i]["INTERNALTYPENAME"].ToString()=="进口")
                        {
                            rowtemp.CreateCell(20).SetCellValue(dt.Rows[i]["COUNTRYORIGINCODE"].ToString());
                        }
                        else if (dt.Rows[i]["INTERNALTYPENAME"].ToString() == "出口")
                        {
                            rowtemp.CreateCell(20).SetCellValue(dt.Rows[i]["DESTCOUNTRYCODE"].ToString());
                        }
                        else
                        {
                            rowtemp.CreateCell(20).SetCellValue("");
                        }
                        
                        rowtemp.CreateCell(21).SetCellValue(dt.Rows[i]["CURRENCY"].ToString());
                        rowtemp.CreateCell(22).SetCellValue(dt.Rows[i]["UNITPRICE"].ToString());
                        rowtemp.CreateCell(23).SetCellValue(dt.Rows[i]["TOTALPRICE"].ToString());
                        //rowtemp.CreateCell(24).SetCellValue("");
                        //rowtemp.CreateCell(25).SetCellValue("");
                        //rowtemp.CreateCell(26).SetCellValue("");
                        //rowtemp.CreateCell(27).SetCellValue("");
                        //rowtemp.CreateCell(28).SetCellValue("");
                        rowtemp.CreateCell(29).SetCellValue(dt.Rows[i]["VERSTATUS"].ToString());
                        rowtemp.CreateCell(30).SetCellValue(dt.Rows[i]["NOTE"].ToString());
                    }
                }
            }

            
            // 写入到客户端 
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //book.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", filename);

            return Extension.getPathname(filename, book);
        }

        #endregion

        public string GetName(string value, string datasource)
        {
            string name = "";

            JArray jarray = JArray.Parse(datasource);
            foreach (JObject json in jarray)
            {
                if (json.Value<string>("CODE") == value) { name = json.Value<string>("NAME"); break; }
            }

            return name;
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

        
        #region recordinfo print

        public string GetPrintDetail()
        {
            string ids = Request["id"];
            string id = ids.IndexOf(",") == -1 ? ids : ids.Substring(0, ids.IndexOf(","));
            string ITEMNOATTRIBUTE = string.Empty;
            string sql = string.Empty; string sql_cp = string.Empty;
            IsoDateTimeConverter iso = new IsoDateTimeConverter();
            string sql_recordinfo = "select a.itemnoattribute,(select code from cusdoc.SYS_RECORDINFO where enabled=1 and id=a.recordinfoid) recordcode from SYS_RECORDINFO_DETAIL_TASK a where id=" + id;
            DataTable dt_recordinfo = DBMgr.GetDataTable(sql_recordinfo);
            string json_recordinfo = JsonConvert.SerializeObject(dt_recordinfo, iso);
            ITEMNOATTRIBUTE = dt_recordinfo.Rows[0]["ITEMNOATTRIBUTE"].ToString();
            sql = "select a.itemno,a.hscode,a.commodityname,b.ele,a.options,(select name from cusdoc.base_declproductunit where enabled=1 and code=a.unit) unit," +
                      "(select name from cusdoc.base_declproductunit where enabled=1 and code=c.legalunit) legalunit,(select name from cusdoc.base_declproductunit where enabled=1 and code=c.secondunit) secondunit " +
                      " from SYS_RECORDINFO_DETAIL_TASK  A left join " +
                       "(select RID,listagg(to_char(FUNCTIONTYPE||':'||DESCRIPTIONS),'<br/>') within group(order by sno) as ELE from SYS_ELEMENTS  GROUP BY RID) B on A.Id=B.RID " +
                       "left join cusdoc.base_commodityhs  C on A.HSCODE=C.HSCODE AND A.CUSTOMAREA=C.YEARID AND A.Additionalno=C.Extracode " +
                       "  where A.ID in(" + ids + ") order by a.itemno";
            string json = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql), iso);

            if (ITEMNOATTRIBUTE == "成品")
            {
                sql_cp = "select a.ITEMNO,a.HSCODE,a.COMMODITYNAME,a.OPTIONS,(select name from cusdoc.base_declproductunit where enabled=1 and code=a.unit) UNIT,ITEMNO_CONSUME,ITEMNO_COMMODITYNAME,ITEMNO_SPECIFICATIONSMODEL,ITEMNO_UNITNAME," +
                         "CONSUME,ATTRITIONRATE from SYS_RECORDINFO_DETAIL_TASK  A left join SYS_PRODUCTCONSUME B  on A.ID=B.RID WHERE A.ID IN (" + ids + ") order by a.itemno";
                string json_cp = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql_cp), iso);
                return "{jsonrows:" + json + ",jsonrows_cp:" + json_cp + ",json_recordinfo:" + json_recordinfo + "}";

            }

            return "{jsonrows:" + json + ",json_recordinfo:" + json_recordinfo + "}";


        }        
        #endregion


        #region LIST_VERIFICATION 核销比对

        public string check_ver()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            return Extension.Check_Customer(json_user.Value<string>("CUSTOMERID")).ToString().ToLower();
        }
        /*
        public string ImExcel_Verification()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            HttpPostedFileBase postedFile = Request.Files["UPLOADFILE"];//获取上传信息对象  
            string fileName = Path.GetFileName(postedFile.FileName);

            string newfile = @"~/FileUpload/Verification/" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + fileName;
            if (!Directory.Exists(Server.MapPath("~/FileUpload/Verification")))
            {
                Directory.CreateDirectory(Server.MapPath("~/FileUpload/Verification"));
            }
            postedFile.SaveAs(Server.MapPath(newfile));

            string result = "";
            DataTable dtExcel = Extension.GetExcelData_Table(Server.MapPath(newfile), 0);
            DataTable dtExcel_sub = Extension.GetExcelData_Table(Server.MapPath(newfile), 1);

            result = Extension.ImExcel_Verification_Data(dtExcel, dtExcel_sub, "线下", json_user);

            if (result != "{success:true,json:[]}")//上传不成功，删除源文件
            {
                FileInfo fi = new FileInfo(Server.MapPath(newfile));
                if (fi.Exists)
                {
                    fi.Delete();
                }
            }

            return result;
        }
         */

        public string ImExcel_Verification()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            HttpPostedFileBase postedFile = Request.Files["UPLOADFILE"];//获取上传信息对象  
            string fileName = Path.GetFileName(postedFile.FileName);

            string newfile = @"~/FileUpload/Verification/" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + fileName;
            if (!Directory.Exists(Server.MapPath("~/FileUpload/Verification")))
            {
                Directory.CreateDirectory(Server.MapPath("~/FileUpload/Verification"));
            }
            postedFile.SaveAs(Server.MapPath(newfile));

            string result = "";
            DataTable dtExcel = Extension.GetExcelData_Table(Server.MapPath(newfile), 0);
            result = Extension.ImExcel_Verification_Data(dtExcel, "线下", json_user);

            if (result != "{success:true,json:[]}")//上传不成功，删除源文件
            {
                FileInfo fi = new FileInfo(Server.MapPath(newfile));
                if (fi.Exists)
                {
                    fi.Delete();
                }
            }

            return result;
        }

        /*public string QueryConditionVerification()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string where = @" where lv.BUSIUNITCODE ='" + json_user.Value<string>("CUSTOMERHSCODE") + "' ";

            if (!string.IsNullOrEmpty(Request["DECLARATIONCODE"]))
            {
                where += " and lv.DECLARATIONCODE='" + Request["DECLARATIONCODE"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["TRADEMETHOD"]))
            {
                where += " and lv.TRADEMETHOD='" + Request["TRADEMETHOD"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["CONTRACTNO"]))
            {
                where += " and lv.CONTRACTNO='" + Request["CONTRACTNO"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["BUSITYPE"]))
            {
                where += " and lv.BUSITYPE='" + Request["BUSITYPE"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["STATUS"]))
            {
                where += " and lv.STATUS='" + Request["STATUS"] + "'";
            }

            string sql = @"select lv.* from list_verification lv " + where;

            return sql;

        }*/

        public string QueryConditionVerification()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string where = " 1=1";
            if (!string.IsNullOrEmpty(Request["DECLARATIONCODE"]))
            {
                where += " and aaa.DECLARATIONCODE='" + Request["DECLARATIONCODE"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["TRADEMETHOD"]))
            {
                where += " and aaa.TRADEMETHOD='" + Request["TRADEMETHOD"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["CONTRACTNO"]))
            {
                where += " and aaa.CONTRACTNO='" + Request["CONTRACTNO"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["BUSITYPE"]))
            {
                where += " and aaa.BUSITYPE='" + Request["BUSITYPE"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["STATUS"]))
            {
                where += " and aaa.STATUS='" + Request["STATUS"] + "'";
            }

            string sql_ver = "", sql_decl = "", sql = "";

            sql_ver = @"select lv.id,lv.datadource,lv.declarationcode,lv.repunitcode,lv.kindoftax
                        ,to_char(lv.reptime,'yyyy-mm-dd') reptime,lv.trademethod,lv.busiunitcode,lv.recordcode,lv.createtime
                        ,lv.status,lv.note,lv.contractno,lv.busitype
                        ,lv.inouttype      
                    from list_verification lv 
                    where lv.busiunitcode='{0}'";
            sql_ver = string.Format(sql_ver, json_user.Value<string>("CUSTOMERHSCODE"));

            sql_decl = @"select aa.* 
                    from (
                        select null id, N'线上' datadource,lda.declarationcode,lda.repunitcode,lda.kindoftax 
                            ,to_char(lda.reptime,'yyyy-mm-dd') reptime,lda.trademethod,lda.busiunitcode,lda.recordcode,null createtime
                            , null status,null note,lda.contractno,sb.name busitype
                            ,case when substr(lda.declarationcode,9,1)='1' then N'进口' when substr(lda.declarationcode,9,1)='0' then N'出口' else N'' end inouttype  
                        from list_declaration det 
                            left join list_order ort on det.ordercode = ort.code 
						    left join list_declaration_after lda on det.code=lda.code and lda.csid=1
                            left join (select ordercode from list_declaration ld where ld.isinvalid=0 and ld.STATUS!=130 and ld.STATUS!=110) a on det.ordercode=a.ordercode
                            left join (
                                    select ASSOCIATENO from list_order l inner join list_declaration i on l.code=i.ordercode 
                                    where l.ASSOCIATENO is not null and l.isinvalid=0 and i.isinvalid=0 and (i.STATUS!=130 and i.STATUS!=110)    
								    ) b on ort.ASSOCIATENO=b.ASSOCIATENO 
                             left join cusdoc.sys_busitype sb on ort.busitype=sb.code and enabled=1           
                        where (det.STATUS=130 or det.STATUS=110) and det.isinvalid=0 and ort.isinvalid=0 and lda.busiunitcode ='{0}'
                            and a.ordercode is null 
                            and b.ASSOCIATENO is null
                        ) aa 
                            left join (select * from list_verification where busiunitcode='{0}') bb on aa.declarationcode=bb.declarationcode
                    where bb.declarationcode is null
                    ";
            sql_decl = string.Format(sql_decl, json_user.Value<string>("CUSTOMERHSCODE"));
            sql = @"select * from (" + sql_ver + " union " + sql_decl + ") aaa where " + where;

            return sql;

        }

        public string loadverification()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string sql = QueryConditionVerification();

            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "declarationcode", "desc"));
            var json = JsonConvert.SerializeObject(dt, iso);

            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string loadVerificationDetail_D()
        {
            string declartioncode = Request["declartioncode"]; string status = Request["status"];

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd";

            string sql = "";
            if (status != "")
            {
                sql = @"select * from list_verification_sub where declarationcode='" + declartioncode + "'";
            }
            else
            {
                sql = @"select a.DECLARATIONCODE,b.ORDERNO,b.ITEMNO,b.COMMODITYNO||b.ADDITIONALNO COMMODITYNO,b.COMMODITYNAME,b.TAXPAID
                                   ,b.CADQUANTITY,b.CADUNIT,b.CURRENCYCODE,b.TOTALPRICE
                            from (select * from list_declaration_after where declarationcode='{0}' and csid=1) a
                                 left join (select * from list_decllist_after where isinvalid=0 and xzlb in('报关单','报关单解析'))b on a.CODE=b.predeclcode";
                sql = string.Format(sql, declartioncode);
            }
            

            DataTable dt_sub = DBMgr.GetDataTable(GetPageSql(sql, "orderno", "asc"));
            var json = JsonConvert.SerializeObject(dt_sub, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        #endregion
    }
}
