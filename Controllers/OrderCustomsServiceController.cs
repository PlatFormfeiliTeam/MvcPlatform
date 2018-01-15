using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
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
    public class OrderCustomsServiceController : Controller
    {
        int totalProperty = 0;
        //
        // GET: /OrderCustomsService/

        public ActionResult Index()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            ViewBag.navigator = "订单中心>>关务服务";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.navigator = "订单中心>>关务服务";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        #region 关务服务

        public string LoadList()
        {
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string sql = QueryCondition();

            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql, "a.createtime", "desc"));
            var json = JsonConvert.SerializeObject(dt, iso);

            return "{rows:" + json + ",total:" + totalProperty + "}";//",json_senior:" + json_senior +
        }

        public string QueryCondition()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string where = "";
            if (!string.IsNullOrEmpty(Request["BUSIUNITCODE"]))//判断查询条件1是否有值
            {
                where += " and a.BUSIUNITCODE='" + Request["BUSIUNITCODE"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["CUSNO"]))//判断查询条件1是否有值
            {
                where += " and a.CUSNO='" + Request["CUSNO"].Trim() + "'";
            }
            if (!string.IsNullOrEmpty(Request["start_date"]))//如果开始时间有值
            {
                where += " and a.CREATETIME>=to_date('" + Request["start_date"] + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            if (!string.IsNullOrEmpty(Request["end_date"]))//如果结束时间有值
            {
                where += " and a.CREATETIME<=to_date('" + Request["end_date"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
            }

            if (!string.IsNullOrEmpty(Request["CUSTOMERCODE"]))//判断查询条件1是否有值
            {
                where += " and a.CUSTOMERCODE='" + Request["CUSTOMERCODE"] + "'";
            }
            if (!string.IsNullOrEmpty(Request["CODE"]))//判断查询条件1是否有值
            {
                where += " and a.CODE='" + Request["CODE"].Trim() + "'";
            }
            if (!string.IsNullOrEmpty(Request["start_date2"]))//如果开始时间有值
            {
                where += " and a.SUBMITTIME>=to_date('" + Request["start_date2"] + "','yyyy-mm-dd hh24:mi:ss') ";
            }
            if (!string.IsNullOrEmpty(Request["end_date2"]))//如果结束时间有值
            {
                where += " and a.SUBMITTIME<=to_date('" + Request["end_date2"].Replace("00:00:00", "23:59:59") + "','yyyy-mm-dd hh24:mi:ss') ";
            }     
            
            if ((Request["OnlySelf"] + "").Trim() == "fa fa-check-square-o")
            {
                where += " and (CREATEUSERID = " + json_user.Value<string>("ID") + " or submitusername='" + json_user.Value<string>("REALNAME") + "') ";
            }

            string sql = @"select a.ID,a.BUSITYPE,a.CREATETIME,a.SUBMITTIME,a.ENTRUSTTYPE,a.CUSTOMERCODE,a.CUSTOMERNAME,a.CLEARUNIT,a.CLEARUNITNAME 
                                ,a.BUSIUNITCODE,a.BUSIUNITNAME,a.CODE,a.CUSNO,a.DOREQUEST,a.CLEARREMARK
                                ,nvl(b.DLF,0) DLF,nvl(b.SJF,0) SJF,nvl(b.QTF,0) QTF,(nvl(b.DLF,0)+nvl(b.SJF,0)+nvl(b.QTF,0))SUMF
                        from LIST_ORDER a 
                            left join (
                                     select * from (select ordercode,FEECODE, cost from finance_costdata) pivot (sum(cost) for FEECODE in ('DLF' DLF, 'SJF' SJF, 'QTF' QTF))
                                    ) b on a.code=b.ordercode
                        where a.ISINVALID=0 and a.BUSITYPE='" + Request["busitypeid"] + "'";

            string rolestr = "";
            rolestr = " a.receiverunitcode='" + json_user.Value<string>("CUSTOMERCODE") + "'";

            /*if (json_user.Value<string>("ISRECEIVER") == "1")//接单单位
            {
                string rec = " a.receiverunitcode='" + json_user.Value<string>("CUSTOMERCODE") + "'";

                if (rolestr == "") { rolestr = rec; }
                else { rolestr = rolestr + " or " + rec; }
            }

            if (json_user.Value<string>("ISCUSTOMER") == "1")//委托单位
            {
                string cus = " a.customercode='" + json_user.Value<string>("CUSTOMERCODE") + "'";

                if (rolestr == "") { rolestr = cus; }
                else { rolestr = rolestr + " or " + cus; }
            }

            if (json_user.Value<string>("DOCSERVICECOMPANY") == "1")//单证服务单位
            {
                string doc = " a.docservicecode='" + json_user.Value<string>("CUSTOMERCODE") + "'";

                if (rolestr == "") { rolestr = doc; }
                else { rolestr = rolestr + " or " + doc; }
            }*/

            sql += " and (" + rolestr + ") " + where;

            return sql;
        }

        public string loadrecord_create()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string ordercode = Request["ordercode"];
            string sql = "";
            string result = "{}"; string formdata = "{}"; string costdata = "[]";
            if (string.IsNullOrEmpty(ordercode))
            {
                
            }
            else
            {
                IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                sql = @"select CODE,ENTRUSTTYPE,CUSTOMERCODE,CUSTOMERNAME,BUSIUNITCODE
                            ,BUSIUNITNAME,CLEARUNIT,CLEARUNITNAME,CUSNO,CREATEUSERID                          
                            ,CREATEUSERNAME,CREATETIME,SUBMITUSERID,SUBMITUSERNAME,SUBMITTIME,DOREQUEST,CLEARREMARK
                        from list_order where code='" + ordercode + "'";
                formdata = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql), iso).TrimStart('[').TrimEnd(']');

                //费用明细
                sql = @"select a.*,(select name||'('||code||')' from finance_feelist where code=a.feecode and ISENABLED=0) FEENAME
                            ,(select name from finance_status where code=a.status) STATUSNAME
                        from finance_costdata a where a.ordercode='" + ordercode + "' order by a.id";
                costdata = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql), iso);
            }
            result = "{formdata:" + formdata + ",costdata:" + costdata + "}";
            return result;
        }

        public string Create_Save()
        {
            string action = Request["action"];
            JObject json = (JObject)JsonConvert.DeserializeObject(Request["formdata"]);
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);

            string sql = ""; string resultmsg = "{success:false}";
            string ordercode = string.Empty;

            if (string.IsNullOrEmpty(Request["ordercode"]))//新增
            {
                ordercode = Extension.getOrderCode();
                sql = @"INSERT INTO LIST_ORDER (ID,
                            BUSITYPE,CODE,ENTRUSTTYPE,CUSTOMERCODE,CUSTOMERNAME,BUSIUNITCODE
                            ,BUSIUNITNAME,CLEARUNIT,CLEARUNITNAME,CUSNO,CREATEUSERID                          
                            ,CREATEUSERNAME,CREATETIME,SUBMITUSERID,SUBMITUSERNAME,SUBMITTIME,DOREQUEST,CLEARREMARK
                            ,RECEIVERUNITCODE,RECEIVERUNITNAME
                        ) VALUES (LIST_ORDER_id.Nextval
                            ,'{0}','{1}','{2}','{3}','{4}','{5}'
                            ,'{6}','{7}','{8}','{9}','{10}'
                            ,'{11}',sysdate,'{12}','{13}',sysdate,'{14}','{15}'
                            ,'{16}','{17}'
                            )";
                sql = string.Format(sql
                        , "70", ordercode, json.Value<string>("ENTRUSTTYPE"), json.Value<string>("CUSTOMERCODE"), json.Value<string>("CUSTOMERNAME"), json.Value<string>("BUSIUNITCODE")
                        , json.Value<string>("BUSIUNITNAME"), json.Value<string>("CLEARUNIT"), json.Value<string>("CLEARUNITNAME"), json.Value<string>("CUSNO"), json_user.Value<string>("ID")
                        , json_user.Value<string>("REALNAME"), json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"), json.Value<string>("DOREQUEST"), json.Value<string>("CLEARREMARK")
                        , json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME")
                   );
            }
            else//修改
            {
                ordercode = Request["ordercode"];
                sql = @"UPDATE LIST_ORDER SET BUSITYPE='{1}',ENTRUSTTYPE='{2}',CUSTOMERCODE='{3}',CUSTOMERNAME='{4}',BUSIUNITCODE='{5}'             
                            ,BUSIUNITNAME='{6}',CLEARUNIT='{7}',CLEARUNITNAME='{8}',CUSNO='{9}',SUBMITUSERID='{10}'          
                            ,SUBMITUSERNAME='{11}',SUBMITTIME=sysdate,DOREQUEST='{12}',CLEARREMARK='{13}',RECEIVERUNITCODE='{14}',RECEIVERUNITNAME='{15}'                              
                        WHERE CODE='{0}'                          
                        ";

                sql = string.Format(sql
                        , ordercode, "70", json.Value<string>("ENTRUSTTYPE"), json.Value<string>("CUSTOMERCODE"), json.Value<string>("CUSTOMERNAME"), json.Value<string>("BUSIUNITCODE")
                        , json.Value<string>("BUSIUNITNAME"), json.Value<string>("CLEARUNIT"), json.Value<string>("CLEARUNITNAME"), json.Value<string>("CUSNO"), json_user.Value<string>("ID")
                        , json_user.Value<string>("REALNAME"), json.Value<string>("DOREQUEST"), json.Value<string>("CLEARREMARK"), json_user.Value<string>("CUSTOMERCODE"), json_user.Value<string>("CUSTOMERNAME")
                        );
            }

            int result = DBMgr.ExecuteNonQuery(sql);
            if (result == 1)
            {
                //费用明细
                string costmsg = update_costdata(json, json_user, ordercode, json.Value<string>("ORIGINALCOSTIDS"));
                if (costmsg == "success")//明细插入成功
                {
                    resultmsg = "{success:true,ordercode:'" + ordercode + "'}";
                }
                else
                {
                    resultmsg = "{success:true,ordercode:'" + ordercode + "',detail:'f'}";
                }
            }

            return resultmsg;
        }

        public string update_costdata(JObject jsonorder, JObject json_user, string ordercode, string originalfileids)
        {
            string msg = "success";
            OracleConnection conn = null;
            OracleTransaction ot = null;
            conn = DBMgr.getOrclCon();
            try
            {
                conn.Open();
                ot = conn.BeginTransaction();

                //在插入 应收
                JArray jarry = (JArray)JsonConvert.DeserializeObject(Request["costdata"]);

                string sql = "";
                bool bf = false;//标记是否有明细，false代表没有：一个是jarry有数据，二个是删除时 状态已经大于 生成费用 ，没删除掉的

                foreach (JObject json in jarry)
                {
                    bf = true;
                    if (string.IsNullOrEmpty(json.Value<string>("ID")))
                    {
                        sql = @"insert into finance_costdata(paytype,buildmode,createtime,currency,costtype,id   
                                                        ,ordercode,feecode,cost,createman,settlementunit,status                                                     
                                                        ) 
                                                values ('0','手动',sysdate,'142','0',FINANCE_COSTDATA_ID.Nextval
                                                        ,'{0}','{1}',{2},'{3}','{4}',{5}
                                                        )";
                        sql = string.Format(sql
                            , ordercode, json.Value<string>("FEECODE"), json.Value<string>("COST"), json_user.Value<string>("NAME"), jsonorder.Value<string>("CLEARUNIT"), json.Value<int>("STATUS")
                            );
                        DBMgr.ExecuteNonQuery(sql, conn);
                    }
                    else//如果ID已经存在  说明是已经存在的记录,不需要做任何处理
                    {
                        originalfileids = originalfileids.Replace(json.Value<string>("ID") + ",", "");
                    }
                }

                //在前端移除的随附文件记录  
                string[] idarray = originalfileids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string id in idarray)
                {
                    sql = @"select * from finance_costdata where ordercode='" + ordercode + "' and ID='" + id + "'";
                    DataTable dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count == 1)
                    {
                        if (Convert.ToInt32(dt.Rows[0]["STATUS"].ToString()) <= 10)
                        {
                            sql = @"delete from finance_costdata where ordercode='" + ordercode + "' and ID='" + id + "'";
                            DBMgr.ExecuteNonQuery(sql, conn);
                        }
                        else
                        {
                            bf = true;
                        }
                    }
                }


                if (bf == false)//false没有明细记录
                {
                    sql = @"delete from finance_order where code='" + ordercode + "'";
                    DBMgr.ExecuteNonQuery(sql, conn);
                }
                else
                {
                    sql = "select count(1) from finance_order where code='" + ordercode + "'";
                    string isExistOrd = DBMgr.GetDataTable(sql).Rows[0][0].ToString();
                    if (isExistOrd == "0")//不存在 
                    {
                        sql = @"insert into finance_order (code,status,cost,paystatus,paycost
                                    ,calrecvflag,calpayflag,container_stand,container_natural, totaldeclcode 
                                    ,totalsheetnum) 
                            values ('{0}',0,0,0,0
                                    ,1,1,0,0,0
                                    ,0)";
                        sql = string.Format(sql, ordercode);
                        DBMgr.ExecuteNonQuery(sql, conn);
                    }

                    //更细汇总数据
                    sql = @"update finance_order set status=(select min(status) from finance_costdata where ordercode='{0}' and paytype='0')
                        ,cost=(select sum(cost) from finance_costdata where ordercode='{0}' and paytype='0') 
                    where code='{0}'";
                    sql = string.Format(sql, ordercode);
                    DBMgr.ExecuteNonQuery(sql, conn);
                }
                ot.Commit();
            }
            catch (Exception ex)
            {
                ot.Rollback();
                msg = "false";
            }
            finally
            {
                conn.Close();
            }
            return msg;
        }

        public string ExportCustomsList()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = QueryCondition();
            sql += " order by a.createtime desc";

            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式 
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

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
            string filename = "关联服务.xls";

            sheet_S = book.CreateSheet("关联服务");

            NPOI.SS.UserModel.IRow row1 = sheet_S.CreateRow(0);
            row1.CreateCell(0).SetCellValue("维护时间"); row1.CreateCell(1).SetCellValue("创建时间"); row1.CreateCell(2).SetCellValue("委托类型"); row1.CreateCell(3).SetCellValue("委托单位"); row1.CreateCell(4).SetCellValue("结算单位");
            row1.CreateCell(5).SetCellValue("经营单位"); row1.CreateCell(6).SetCellValue("企业编号"); row1.CreateCell(7).SetCellValue("订单编号"); row1.CreateCell(8).SetCellValue("操作需求");
            row1.CreateCell(9).SetCellValue("结算备注"); row1.CreateCell(10).SetCellValue("代理费"); row1.CreateCell(11).SetCellValue("输机费"); row1.CreateCell(12).SetCellValue("其他费");
            row1.CreateCell(13).SetCellValue("合计"); 

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet_S.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(dt.Rows[i]["SUBMITTIME"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dt.Rows[i]["CREATETIME"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dt.Rows[i]["ENTRUSTTYPE"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dt.Rows[i]["CUSTOMERNAME"].ToString());
                rowtemp.CreateCell(4).SetCellValue(dt.Rows[i]["CLEARUNITNAME"].ToString());

                rowtemp.CreateCell(5).SetCellValue(dt.Rows[i]["BUSIUNITNAME"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dt.Rows[i]["CUSNO"].ToString());
                rowtemp.CreateCell(7).SetCellValue(dt.Rows[i]["CODE"].ToString());
                rowtemp.CreateCell(8).SetCellValue(dt.Rows[i]["DOREQUEST"].ToString());

                rowtemp.CreateCell(9).SetCellValue(dt.Rows[i]["CLEARREMARK"].ToString());
                rowtemp.CreateCell(10).SetCellValue(dt.Rows[i]["DLF"].ToString());
                rowtemp.CreateCell(11).SetCellValue(dt.Rows[i]["SJF"].ToString());
                rowtemp.CreateCell(12).SetCellValue(dt.Rows[i]["QTF"].ToString());

                rowtemp.CreateCell(13).SetCellValue(dt.Rows[i]["SUMF"].ToString());
            }
            // 写入到客户端 
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //book.Write(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", filename);

            return Extension.getPathname(filename, book);

        }

        public string DeleteCustoms()
        {
            string ordercode = Request["ordercode"]; string result = "{success:true}";

            bool bf = false;
            string sql = @"select * from finance_costdata where ordercode='" + ordercode + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToInt32(dt.Rows[0]["STATUS"].ToString()) > 10)
                {
                    bf = true;
                    break;
                }
            }
            if (bf)//费用明细 中 费用状态大于生成费用，不能删除
            {
                return "{success:false,flag:'E'}";
            }

             OracleConnection conn = null;
            OracleTransaction ot = null;
            conn = DBMgr.getOrclCon();
            try
            {
                conn.Open();
                ot = conn.BeginTransaction();

                sql = @"delete from finance_order where code='" + ordercode + "'";
                DBMgr.ExecuteNonQuery(sql, conn);

                sql = @"delete from finance_costdata where ordercode='" + ordercode + "'";
                DBMgr.ExecuteNonQuery(sql, conn);

                sql = @"update list_order set ISINVALID=1 where code='" + ordercode + "'";
                DBMgr.ExecuteNonQuery(sql, conn);

                ot.Commit();                 
            }
            catch (Exception ex)
            {
                ot.Rollback();
                result = "{success:false}";
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        #endregion


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
