using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    [Authorize]
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

        public ActionResult Recordinfo_Detail_Audit()//账册审核
        {
            ViewBag.navigator = "客户服务>>账册审核";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult PrintRecordDetail()//打印界面
        {
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }


        #region Recordinfo_Detail
        public string Query_RecordInfor(string type)
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string sql="",where = "";
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
                sql = @"select a.code,b.*
                            from cusdoc.sys_recordinfo a
                                 inner join (  
                                         select aa.ID,aa.recordinfoid,aa.itemno,aa.hscode,aa.additionalno,aa.itemnoattribute 
                                                ,aa.commodityname,aa.specificationsmodel,aa.unit,aa.remark
                                                ,aa.options,aa.status,aa.customercode,aa.customername
                                         from sys_recordinfo_detail_task aa 
                                    ) b on a.id=b.recordinfoid ";
            }
            else
            {
                sql = @"select a.code,b.*
                            from cusdoc.sys_recordinfo a
                                 inner join (
                                         select aa.ID,aa.recordinfoid,aa.itemno,aa.hscode,aa.additionalno,aa.itemnoattribute 
                                                ,aa.commodityname,aa.specificationsmodel,aa.unit,aa.remark 
                                                ,null options,null status,null customercode,null customername
                                         from cusdoc.sys_recordinfo_detail aa
                                    ) b on a.id=b.recordinfoid ";                
            }

            if (Request["ERROR"].ToString() == "1")
            {
                sql = sql + " left join (select hscode from cusdoc.BASE_COMMODITYHS where enabled=1) c on b.hscode=c.hscode";
            }
            sql = sql + " where a.busiunit='" + json_user.Value<string>("CUSTOMERHSCODE") + "'" + where;

            if (Request["ERROR"].ToString() == "1")
            {
                sql = sql + " and c.hscode is null";
            }

            return sql;
        }

        /*all sql
         * string sql = @"select a.code,b.*
                        from cusdoc.sys_recordinfo a
                             inner join (  
                                     select aa.ID,aa.recordinfoid,aa.itemno,aa.hscode,aa.additionalno,aa.itemnoattribute 
                                            ,aa.commodityname,aa.specificationsmodel,aa.unit,aa.remark
                                            ,aa.options,aa.status,aa.customercode,aa.customername
                                     from sys_recordinfo_detail_task aa where aa.status<50 
                                     union
                                     select aa.ID,aa.recordinfoid,aa.itemno,aa.hscode,aa.additionalno,aa.itemnoattribute 
                                            ,aa.commodityname,aa.specificationsmodel,aa.unit,aa.remark 
                                            ,null options,null status,null customercode,null customername
                                     from cusdoc.sys_recordinfo_detail aa 
                                          left join (select * from sys_recordinfo_detail_task where status<50 and OPTIONS<>'A') bb on aa.id=bb.rid
                                     where bb.rid is null
                                ) b on a.id=b.recordinfoid ";*/
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

        public FileResult Export()
        {
            string sql = "";string e_options=Request["e_options"];string e_status=Request["e_status"];string e_unit=Request["e_unit"];
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            string filename = "账册信息.xls";

            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            sql = Query_RecordInfor("");
            sql = sql + " order by recordinfoid,itemno";

            DataTable dt = DBMgr.GetDataTable(sql);
            DataRow[] dr_lj = dt.Select("itemnoattribute='料件'"); DataRow[] dr_cp = dt.Select("itemnoattribute='成品'");

            NPOI.SS.UserModel.ISheet sheet_lj = book.CreateSheet("料件");
            NPOI.SS.UserModel.IRow row1 = sheet_lj.CreateRow(0);
            row1.CreateCell(0).SetCellValue("账册号"); row1.CreateCell(1).SetCellValue("项号"); row1.CreateCell(2).SetCellValue("HS编码"); row1.CreateCell(3).SetCellValue("附加码");
            row1.CreateCell(4).SetCellValue("项号属性"); row1.CreateCell(5).SetCellValue("商品名称"); row1.CreateCell(6).SetCellValue("规格型号"); row1.CreateCell(7).SetCellValue("成交单位");
            row1.CreateCell(8).SetCellValue("备注");

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
            }

            NPOI.SS.UserModel.ISheet sheet_cp = book.CreateSheet("成品");
            NPOI.SS.UserModel.IRow row2 = sheet_cp.CreateRow(0);
            row2.CreateCell(0).SetCellValue("账册号"); row2.CreateCell(1).SetCellValue("项号"); row2.CreateCell(2).SetCellValue("HS编码"); row2.CreateCell(3).SetCellValue("附加码");
            row2.CreateCell(4).SetCellValue("项号属性"); row2.CreateCell(5).SetCellValue("商品名称"); row2.CreateCell(6).SetCellValue("规格型号"); row2.CreateCell(7).SetCellValue("成交单位");
            row2.CreateCell(8).SetCellValue("备注");

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
            row3.CreateCell(8).SetCellValue("规格型号"); row3.CreateCell(9).SetCellValue("成交单位"); row3.CreateCell(10).SetCellValue("备注");

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
            }

            NPOI.SS.UserModel.ISheet sheet_cp_go = book.CreateSheet("成品_申请");
            NPOI.SS.UserModel.IRow row4 = sheet_cp_go.CreateRow(0);
            row4.CreateCell(0).SetCellValue("变动状态"); row4.CreateCell(1).SetCellValue("申请状态"); row4.CreateCell(2).SetCellValue("账册号"); row4.CreateCell(3).SetCellValue("项号");
            row4.CreateCell(4).SetCellValue("HS编码"); row4.CreateCell(5).SetCellValue("附加码"); row4.CreateCell(6).SetCellValue("项号属性"); row4.CreateCell(7).SetCellValue("商品名称");
            row4.CreateCell(8).SetCellValue("规格型号"); row4.CreateCell(9).SetCellValue("成交单位"); row4.CreateCell(10).SetCellValue("备注");

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
            }
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", filename);
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

            sql = "update sys_recordinfo_detail_task set STATUS = 0,SUBMITID=null,SUBMITTIME=null,SUBMITNAME=null where id ='" + id + "'";
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

        public string loadrecord_create()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string id = Request["id"]; string copyid = Request["copyid"];
            string sql = "";
            string result = "{}"; string formdata = "{}"; string productsonsumedata = "[]";
            if (string.IsNullOrEmpty(id))
            {
                if (string.IsNullOrEmpty(copyid))//如果是复制新增
                {
                    formdata = "{ITEMNOATTRIBUTE:'料件',STATUS:0,OPTIONS:'A',ISPRINT_APPLY:0}";
                }
                else
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
                        ,COMMODITYNAME,SPECIFICATIONSMODEL,UNIT,REMARK,MODIFYREASON
                        ,CREATEID,CREATENAME,CREATEDATE,OPTIONS,STATUS,CUSTOMERCODE
                        ,CUSTOMERNAME,SUBMITID,SUBMITNAME,SUBMITTIME,CUSTOMAREA                       
                        ) VALUES ('{0}'
                            ,'{1}','{2}','{3}','{4}','{5}'
                            ,'{6}','{7}','{8}','{9}','{10}'
                            ,'{11}','{12}',sysdate,'{13}','{14}','{15}'
                            ,'{16}','{17}','{18}',{19},'{20}'
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
                            ,COMMODITYNAME='{6}',SPECIFICATIONSMODEL='{7}',UNIT='{8}',REMARK='{9}',MODIFYREASON='{10}'
                            ,OPTIONS='{11}',STATUS='{12}',CUSTOMERCODE='{13}',CUSTOMERNAME='{14}',SUBMITID='{15}'
                            ,SUBMITNAME='{16}',SUBMITTIME={17},CUSTOMAREA='{18}'
                        WHERE ID={0}";
                sql = string.Format(sql, id
                    , json.Value<string>("RECORDINFOID"), json.Value<string>("ITEMNO"), json.Value<string>("HSCODE"), json.Value<string>("ADDITIONALNO"), json.Value<string>("ITEMNOATTRIBUTE")
                    , json.Value<string>("COMMODITYNAME"), json.Value<string>("SPECIFICATIONSMODEL"), json.Value<string>("UNIT"), json.Value<string>("REMARK"), json.Value<string>("MODIFYREASON")
                    , 'A', json.Value<string>("STATUS"), json.Value<string>("CUSTOMERCODE"), json.Value<string>("CUSTOMERNAME"), json.Value<string>("SUBMITID")
                    , json.Value<string>("SUBMITNAME"), json.Value<string>("SUBMITTIME"), json.Value<string>("CUSTOMAREA")
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
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string id = Request["id"]; string rid = Request["rid"];
            string sql = ""; DataTable dt = new DataTable();
            string result = "{}"; string formdata = "{}"; string productsonsumedata = "[]";
            if (string.IsNullOrEmpty(id))
            {
                if (!string.IsNullOrEmpty(rid))//如果是变动申请
                {
                    IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                    iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                    sql = @"select '{0}' RID,RECORDINFOID,ITEMNO,ITEMNOATTRIBUTE
                                ,HSCODE,ADDITIONALNO,COMMODITYNAME,SPECIFICATIONSMODEL,UNIT
                                ,HSCODE HSCODE_LEFT,ADDITIONALNO ADDITIONALNO_LEFT,COMMODITYNAME COMMODITYNAME_LEFT,SPECIFICATIONSMODEL SPECIFICATIONSMODEL_LEFT,UNIT UNIT_LEFT   
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
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
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
            string sql = @"select id,code,code||'('||bookattribute||')' as name from sys_recordinfo where busiunit= '" + Request["EnterpriseHSCOCDE"] + "'";
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
                            from cusdoc.sys_recordinfo a
                                 inner join (  
                                         select aa.ID,aa.recordinfoid,aa.itemno,aa.hscode,aa.additionalno,aa.itemnoattribute 
                                                ,aa.commodityname,aa.specificationsmodel,aa.unit,aa.remark
                                                ,aa.options,aa.status,aa.customercode,aa.customername,aa.submittime 
                                         from sys_recordinfo_detail_task aa 
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

        public FileResult Export_Audit()
        {
            string sql = ""; string e_options = Request["e_options"]; string e_status = Request["e_status"]; string e_unit = Request["e_unit"];
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            string filename = "账册信息.xls";            

            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            sql = Query_RecordInfor_Audit();         
            sql = sql + " order by b.id desc";

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
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", filename);
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
            string sql_recordinfo = "select a.itemnoattribute,(select code from cusdoc.SYS_RECORDINFO where id=a.recordinfoid) recordcode from SYS_RECORDINFO_DETAIL_TASK a where id=" + id;
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
    }
}
