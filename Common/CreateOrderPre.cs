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
