using System.Collections.Generic;
using System.Data;
using MvcPlatform.Models;
using MvcPlatform.Common;
using System;
using System.Web.Mvc;

namespace MvcPlatform.Common
{
    /// <summary>
    /// 创建订单时调用，获得预配信息
    /// </summary>
    public class CreateOrderPre : Controller
    {
        /// <summary>
        /// 创建订单预配信息
        /// </summary>
        public int CreateOrderPreMatch(Models.OrderEn order)
        {
            DataTable dt = null;
            int num = 0;
            string str = string.Empty;

            List<PreOrderEn> list = new List<PreOrderEn>();
            if (!string.IsNullOrEmpty(order.BusiType) && !string.IsNullOrEmpty(order.BusiUnitCode) && !string.IsNullOrEmpty(order.CustomerCode))
            {
                string sel = @"select * from config_preorder where (instr(','||busitypes||',',','||'" + order.BusiType +
                    "'||',')>0 or busitypes is null) and  (instr(','||customerids||',',','||'" + order.CustomerCode +
                    "'||',')>0 or customerids is null) and (instr(','||busiunitcodes||',',','||'" + order.BusiUnitCode +
                    "'||',')>0 or busiunitcodes is null) and enabled=1";
                dt = DBMgr.GetDataTable(sel);
            }
            string cs = string.Empty, mo = string.Empty, co = string.Empty, decl = string.Empty, insp = string.Empty, regu = string.Empty;
            if (dt == null)
            {
                return 0;
            }
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow p in dt.Rows)
                {
                    switch (Convert.ToInt16(p["PreType"]))
                    {
                        case (int)PostEnum.CS:
                            cs += p["UserId"] + ",";
                            break;
                        case (int)PostEnum.MO:
                            mo += p["UserId"] + ",";
                            break;
                        case (int)PostEnum.CO:
                            co += p["UserId"] + ",";
                            break;
                        case (int)PostEnum.DECL:
                            decl += p["UserId"] + ",";
                            break;
                        case (int)PostEnum.INSP:
                            insp += p["UserId"] + ",";
                            break;
                        case (int)PostEnum.REGU:
                            regu += p["UserId"] + ",";
                            break;
                    }
                }
            }
            string ins = @"insert into list_cspond(id,ordercode,precs,premo,preco,predecl,preinsp,preregu,priority,isback,status,correspondenceno,associateno,busitype)
                values(list_cspond_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')";
            ins = string.Format(ins, order.Code, cs, mo, co, decl, insp, regu, order.Priority, 0, order.Status, order.CorrespondNo, order.AssociateNo, order.BusiType);
            num = DBMgr.ExecuteNonQuery(ins);
            return num;
        }

        /// <summary>
        /// 更新修改 list_times
        /// </summary>
        /// <param name="order"></param>
        public void CreateOrdertimes(Models.OrderEn order)
        {
            //更新修改 list_times
            if (order.Status != 1 && order.Status != 10 && order.Status != 15)
            {
                return;
            }
            string ins = "";
            int i = 0;
            //如果是创建的时候直接委托或直接上传文件：
            if (order.Status == 10)
            {
                //文件上传
                ins = @"select count(*) from list_times where code = '{0}' and status = '{1}'";
                ins = string.Format(ins, order.Code, 1);
                i = Convert.ToInt32(DBMgr.GetDataTable(ins).Rows[0][0]);
                if (i == 0)
                {
                    //插入草稿
                    ins = "insert into list_times(id,code,userid,realname,status,times,type,ispause) values(list_times_id.nextval,'{0}','{1}','{2}','{3}',sysdate,'1','0')";
                    ins = string.Format(ins, order.Code, order.CreateUserId, order.CreateUserName, 1);
                   DBMgr.ExecuteNonQuery(ins);
                }
            }
            if (order.Status == 15)
            {
                //提交委托
                ins = @"select count(*) from list_times where code = '{0}' and status = '{1}'";
                ins = string.Format(ins, order.Code, 1);
                i = Convert.ToInt32(DBMgr.GetDataTable(ins).Rows[0][0]);
                if (i == 0)
                {
                    //插入草稿
                    ins = "insert into list_times(id,code,userid,realname,status,times,type,ispause) values(list_times_id.nextval,'{0}','{1}','{2}','{3}',sysdate,'1','0')";
                    ins = string.Format(ins, order.Code, order.CreateUserId, order.CreateUserName, 1);
                    DBMgr.ExecuteNonQuery(ins);
                }

                ins = @"select count(*) from list_times where code = '{0}' and status = '{1}'";
                ins = string.Format(ins, order.Code, 10);
                i = Convert.ToInt32(DBMgr.GetDataTable(ins).Rows[0][0]);
                if (i == 0)
                {
                    //插入文件已上传
                    ins = "insert into list_times(id,code,userid,realname,status,times,type,ispause) values(list_times_id.nextval,'{0}','{1}','{2}','{3}',sysdate,'1','0')";
                    ins = string.Format(ins, order.Code, order.CreateUserId, order.CreateUserName, 10);
                    DBMgr.ExecuteNonQuery(ins);
                }
            }

            //插入当前
            ins = @"select count(*) from list_times where code = '{0}' and status = '{1}'";
            ins = string.Format(ins, order.Code, order.Status);
            i = Convert.ToInt32(DBMgr.GetDataTable(ins).Rows[0][0]);
            if (i == 0)
            {
                ins = "insert into list_times(id,code,userid,realname,status,times,type,ispause) values(list_times_id.nextval,'{0}','{1}','{2}','{3}',sysdate,'1','0')";
                ins = string.Format(ins, order.Code, order.CreateUserId, order.CreateUserName, order.Status);
                DBMgr.ExecuteNonQuery(ins);
            }
        }


    }
}
