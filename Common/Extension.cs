using MvcPlatform.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MvcPlatform.Common
{
    public static class Extension
    {
        public static string ToSHA1(this string value)
        {
            string result = string.Empty;
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] array = sha1.ComputeHash(Encoding.Unicode.GetBytes(value));
            for (int i = 0; i < array.Length; i++)
            {
                result += array[i].ToString("x2");
            }
            return result;
        }
        public static JObject Get_UserInfo(string account)
        { 
            IDatabase db = SeRedis.redis.GetDatabase();
            string result = "";
            if (db.KeyExists(account))
            {
                result = db.StringGet(account);
            } 
            else 
            {
                //2016-08-02增加字段报关服务单位SCENEDECLAREID 报检服务单位SCENEINSPECTID 因为订单里面创建时取当前用户的默认值 故提前放到缓存
                //CUSTOMERID 这个字段在sysuser表中有
                string sql = @"select c.NAME as CUSTOMERNAME,c.HSCODE as CUSTOMERHSCODE,c.CIQCODE as CUSTOMERCIQCODE,c.CODE CUSTOMERCODE,
                             c.SCENEDECLAREID,c.SCENEINSPECTID,u.* from SYS_USER u 
                             left join sys_customer c on u.customerid = c.id where u.name ='" + account + "'";
                DataTable dt = DBMgr.GetDataTable(sql);
                IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                string jsonstr = JsonConvert.SerializeObject(dt, iso);
                jsonstr = jsonstr.Replace("[", "").Replace("]", "");
                db.StringSet(account, jsonstr); 
                result = jsonstr;
            }
            return (JObject)JsonConvert.DeserializeObject(result);
        }

        //获取订单CODE
        public static string getOrderCode()
        {
            string code = string.Empty;
            try
            {
                string sql = "", sql1 = "";
                sql = "update sys_code set code=code+1";
                DBMgr.ExecuteNonQuery(sql);

                sql1 = "select code,sysdate from sys_code";
                DataTable dt = DBMgr.GetDataTable(sql1);

                code = (Convert.ToDateTime(dt.Rows[0]["sysdate"].ToString()).ToString("yyyyMMdd")).Substring(2);
                code += Convert.ToInt32(dt.Rows[0]["code"].ToString()).ToString("00000");

            }
            catch (Exception e)
            {
                throw e;
            }

            return code;

        }

        /*
string sql = "", sql1 = "";

sql = "select sys_code_id.nextval from dual";
string code = string.Empty;
try
{
    DataTable dt = DBMgr.GetDataTable(sql);
    int CodeId = int.Parse(dt.Rows[0][0].ToString());
    sql1 = "select YEARMONTH, code from sys_code where id=" + CodeId;
    DataTable dt1 = DBMgr.GetDataTable(sql1);
    while (dt1.Rows.Count <= 0)
    {
        dt = DBMgr.GetDataTable(sql);
        CodeId = int.Parse(dt.Rows[0][0].ToString());
        sql1 = "select YEARMONTH, code from sys_code where id=" + CodeId;
        dt1 = DBMgr.GetDataTable(sql1);
    }
    code = dt1.Rows[0][0].ToString() + dt1.Rows[0][1].ToString();
}
catch (Exception e)
{
    throw e;
}
return code;
 */

        /*
        private static readonly object syncRoot = new object();
        public string GetCode1(CodeTypeEnum codeType)
        {
            DBSession db = new DBSession();
            db.BeginTransaction();
            string code = "";

            lock (syncRoot)
            {
                try
                {
                    string sql = "update sys_code set code=code+1";
                    db.Execute(sql);

                    sql = "select code,sysdate from sys_code";
                    DataTable dt = db.Query(sql);

                    db.Dispose();
                    code += createCode(Convert.ToDateTime(dt.Rows[0]["sysdate"].ToString()));
                    code += Convert.ToInt32(dt.Rows[0]["code"].ToString()).ToString("00000");
                    return code;
                }
                catch (Exception ex)
                {
                    db.Rollback();
                    return ex.Message;
                }
            }


        }
        private string createCode(DateTime time)
        {
            string code = "";
            code += time.Year.ToString().Substring(2);
            code += time.Month.ToString("00");
            code += time.Day.ToString("00");
            return code;
        } 
        */

        //集装箱及报关车号列表更新
        public static void predeclcontainer_update(string ordercode, string containertruck)
        {

            DBMgr.ExecuteNonQuery("delete from list_predeclcontainer where ORDERCODE = '" + ordercode + "'");
            if (!string.IsNullOrEmpty(containertruck))
            {
                JArray ja = (JArray)JsonConvert.DeserializeObject(containertruck);
                for (int i = 0; i < ja.Count; i++)
                {
                    string sql = @"insert into list_predeclcontainer(ID,ORDERCODE,CONTAINERORDER,CONTAINERNO,CONTAINERSIZE,CONTAINERSIZEE,CONTAINERWEIGHT,
                    CONTAINERTYPE,HSCODE,FORMATNAME,CDCARNO,CDCARNAME,UNITNO,ELESHUT) values(LIST_PREDECLCONTAINER_id.Nextval,'{0}','{1}','{2}','{3}',
                    '{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')";
                    sql = string.Format(sql, ordercode, i, ja[i].Value<string>("CONTAINERNO"), ja[i].Value<string>("CONTAINERSIZE"), ja[i].Value<string>("CONTAINERSIZEE"),
                    ja[i].Value<string>("CONTAINERWEIGHT"), ja[i].Value<string>("CONTAINERTYPE"), ja[i].Value<string>("HSCODE"), ja[i].Value<string>("FORMATNAME"),
                    ja[i].Value<string>("CDCARNO"), ja[i].Value<string>("CDCARNAME"), ja[i].Value<string>("UNITNO"), ja[i].Value<string>("ELESHUT"));
                    DBMgr.ExecuteNonQuery(sql);
                }
            }
        }

        //订单新增或者更新时对随附文件表的操作  非国内业务都会用到  封装by panhuaguo 2016-08-03
        //originalfileids 这个字符串存储的是订单修改时原始随附文件id用逗号分隔
        public static void Update_Attachment(string ordercode, string filedata, string originalfileids, JObject json_user)
        {
            if (!string.IsNullOrEmpty(filedata))
            {
                System.Uri Uri = new Uri("ftp://" + ConfigurationManager.AppSettings["FTPServer"] + ":" + ConfigurationManager.AppSettings["FTPPortNO"]);
                string UserName = ConfigurationManager.AppSettings["FTPUserName"];
                string Password = ConfigurationManager.AppSettings["FTPPassword"];
                FtpHelper ftp = new FtpHelper(Uri, UserName, Password);
                JArray jarry = JsonConvert.DeserializeObject<JArray>(filedata);
                string sql = "";
                foreach (JObject json in jarry)
                {
                    if (string.IsNullOrEmpty(json.Value<string>("ID")))
                    {
                        string filename = "/" + json.Value<string>("FILETYPE") + "/" + ordercode + "/" + json.Value<string>("ORIGINALNAME");
                        string sizes = json.Value<string>("SIZES");
                        string filetypename = json.Value<string>("FILETYPENAME");
                        string extname = json.Value<string>("ORIGINALNAME").ToString().Substring(json.Value<string>("ORIGINALNAME").ToString().LastIndexOf('.') + 1);

                        sql = @"insert into LIST_ATTACHMENT (id,filename,originalname,filetype,uploadtime,uploaduserid,customercode,ordercode,
                                          sizes,filetypename,filesuffix,IETYPE) values(List_Attachment_Id.Nextval,'{0}','{1}','{2}',sysdate,{3},'{4}','{5}','{6}','{7}','{8}','{9}')";
                        sql = string.Format(sql, filename, json.Value<string>("ORIGINALNAME"), json.Value<string>("FILETYPE"), json_user.Value<string>("ID"), json_user.Value<string>("CUSTOMERCODE"), ordercode, sizes, filetypename, extname, json.Value<string>("IETYPE"));
                        DBMgr.ExecuteNonQuery(sql);
                    }
                    else//如果ID已经存在  说明是已经存在的记录,不需要做任何处理
                    {
                        originalfileids = originalfileids.Replace(json.Value<string>("ID") + ",", "");
                    }
                }
                //从数据库和文档库删除在前端移除的随附文件记录  
                string[] idarray = originalfileids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string id in idarray)
                {
                    sql = @"select * from LIST_ATTACHMENT where ID='" + id + "'";
                    DataTable dt = DBMgr.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        ftp.DeleteFile(dt.Rows[0]["FILENAME"] + "");
                    }
                    sql = @"delete from LIST_ATTACHMENT where ID='" + id + "'";
                    DBMgr.ExecuteNonQuery(sql);
                }
            }
        }
        
        //订单的状态在草稿、文件已上传、订单已委托 三个状态发生时记录到订单状态变更日志
        public static void add_list_time(int status, string ordercode, JObject json_user)
        {
            int[] status_array = new int[] { 1, 10, 15 };
            foreach (int status_tmp in status_array)
            {
                if (status >= status_tmp)
                {
                    OrderEn entTimes = new OrderEn();
                    entTimes.Status = status_tmp;
                    entTimes.Code = ordercode;
                    entTimes.CreateUserId = json_user.Value<Int32>("ID");
                    entTimes.CreateUserName = json_user.Value<string>("REALNAME");
                    CreateOrderPre creaTimes = new CreateOrderPre();
                    creaTimes.CreateOrdertimes(entTimes);
                }
            }
        }

        //提交后订单修改时记录字段信息变更情况
        public static void Insert_FieldUpdate_History(string ordercode, JObject json_new, JObject json_user, string busitype)
        {
            //国内订单已受理后前端为空时能修改的字段
            string JsonFieldComments = "";
            switch (busitype)
            {
                case "40"://国内出口
                case "41"://国内进口
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',GOODSNUM:'件数',PACKKIND:'包装',GOODSGW:'毛重',GOODSNW:'净重',CONTRACTNO:'合同号',RECORDCODE:'备案号',
                        LAWFLAG:'法检状况',CLEARANCENO:'通关单号',ASSOCIATEPEDECLNO:'出口报关单',REPUNITCODE:'报关申报单位',INSPUNITCODE:'报检申报单位',ENTRUSTREQUEST:'需求备注'}";
                    break;
                case "10"://空运出口
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',TOTALNO:'总单号',DIVIDENO:'分单号',GOODSNUM:'件数',PACKKIND:'包装',GOODSGW:'毛重'',GOODSNW:'净重',CONTRACTNO:'合同号',ARRIVEDNO:'运抵编号',TURNPRENO:'转关预录号',
                        CLEARANCENO:'通关单号',DECLCARNO:'报关车号',ENTRUSTREQUEST:'需求备注',LAWFLAG:'法检状况',WEIGHTCHECK:'需重量确认',ISWEIGHTCHECK:'重量确认'}";
                    break;
                case "11"://空运进口                    
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',TOTALNO:'总单号',GOODSNUM:'件数',PACKKIND:'包装',GOODSNW:'净重',CONTRACTNO:'合同号',TURNPRENO:'转关预录号',
                        CLEARANCENO:'通关单号',DECLCARNO:'报关车号',ENTRUSTREQUEST:'需求备注',LAWFLAG:'法检状况'}";
                    break;
                case "20"://海运出口       
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',PACKKIND:'包装',GOODSNW:'净重',CONTRACTNO:'合同号',SECONDLADINGBILLNO:'提单号',ARRIVEDNO:'运抵编号',
                        LAWFLAG:'法检状况',CLEARANCENO:'通关单号',CONTAINERNO:'集装箱号',DECLCARNO:'报关车号',TURNPRENO:'转关预录号',ENTRUSTREQUEST:'需求备注'}";
                    break;
                case "21"://海运进口     
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',PACKKIND:'包装',GOODSNW:'净重',CONTRACTNO:'合同号',FIRSTLADINGBILLNO:'国检提单号',SECONDLADINGBILLNO:'海关提单号',TRADEWAYCODES_ZS:'贸易方式',
                        TURNPRENO:'转关预录号',WOODPACKINGID:'木质包装',CLEARANCENO:'通关单号',LAWFLAG:'法检状况',CONTAINERNO:'集装箱号',DECLCARNO:'报关车号',ENTRUSTREQUEST:'需求备注'}";
                    break;
                case "30"://陆运出口       
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',FILGHTNO:'航次号',CONTRACTNO:'合同号',PACKKIND:'包装',GOODSNW:'净重',ARRIVEDNO:'运抵编号',LAWFLAG:'法检状况',
                        CLEARANCENO:'通关单号',CONTAINERNO:'集装箱号',DECLCARNO:'报关车号',TURNPRENO:'转关预录号',TOTALNO:'总单号',DIVIDENO:'分单号',ENTRUSTREQUEST:'需求备注'}";
                    break;
                case "31"://陆运进口     
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',FILGHTNO:'航次号',DIVIDENO:'分单号',GOODSNUM:'件数',PACKKIND:'包装',GOODSNW:'净重',CONTRACTNO:'合同号',MANIFEST:'载货清单号',
                        CLEARANCENO:'通关单号',LAWFLAG:'法检状况',CONTAINERNO:'集装箱号',DECLCARNO:'报关车号',ENTRUSTREQUEST:'需求备注'}";
                    break;
                case "50"://特殊区域出口                         
                case "51"://特殊区域进口      
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',PACKKIND:'包装',GOODSNW:'净重',CONTRACTNO:'合同号',TURNPRENO:'对方转关号',LAWFLAG:'法检状况',CLEARANCENO:'通关单号',
                        GOODSTYPEID:'货物类型',CONTAINERNO:'集装箱号',DECLCARNO:'报关车号',ENTRUSTREQUEST:'需求备注',BUSITYPE:'业务类型'}";
                    break;
            }

            string sql = "select * from list_order where CODE = '" + ordercode + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string ori_json = JsonConvert.SerializeObject(dt, iso).TrimStart('[').TrimEnd(']');
            JObject json_key = JsonConvert.DeserializeObject<JObject>(JsonFieldComments);
            JObject json_ori = JsonConvert.DeserializeObject<JObject>(ori_json);
            foreach (JProperty jp in json_key.Properties())
            {
                if (jp.Name == "SPECIALRELATIONSHIP" || jp.Name == "PRICEIMPACT" || jp.Name == "PAYPOYALTIES" || jp.Name == "SPECIALRELATIONSHIP" || jp.Name == "LAWFLAG"
                    || jp.Name == "WEIGHTCHECK" || jp.Name == "ISWEIGHTCHECK")
                {
                    if (!json_ori.Value<bool>(jp.Name) && json_new.Value<string>(jp.Name) == "on")
                    {
                        sql = @"insert into list_updatehistory(id,ORDERCODE,USERID,UPDATETIME,NEWFIELD,NAME,CODE,FIELD,FIELDNAME,TYPE) values
                        (LIST_UPDATEHISTORY_ID.nextval,'{0}','{1}',sysdate,'{2}','{3}','{4}','{5}','{6}','1')";
                        sql = string.Format(sql, ordercode, json_user.Value<string>("ID"), json_new.Value<string>(jp.Name), json_user.Value<string>("NAME"), ordercode, jp.Name, jp.Value);
                        DBMgr.ExecuteNonQuery(sql);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(json_ori.Value<string>(jp.Name)) && !string.IsNullOrEmpty(json_new.Value<string>(jp.Name)))
                    {
                        sql = @"insert into list_updatehistory(id,ORDERCODE,USERID,UPDATETIME,NEWFIELD,NAME,CODE,FIELD,FIELDNAME,TYPE) values
                        (LIST_UPDATEHISTORY_ID.nextval,'{0}','{1}',sysdate,'{2}','{3}','{4}','{5}','{6}','1')";
                        sql = string.Format(sql, ordercode, json_user.Value<string>("ID"), json_new.Value<string>(jp.Name), json_user.Value<string>("NAME"), ordercode, jp.Name, jp.Value);
                        DBMgr.ExecuteNonQuery(sql);
                    }
                }
            }
        }

        //订单删除 公共方法 by panhuaguo 2016-08-30  含国内业务
        public static string deleteorder(string ordercode)
        {
            string json = "{success:false}";
            //删除订单随附文件
            System.Uri Uri = new Uri("ftp://" + ConfigurationManager.AppSettings["FTPServer"] + ":" + ConfigurationManager.AppSettings["FTPPortNO"]);
            string UserName = ConfigurationManager.AppSettings["FTPUserName"];
            string Password = ConfigurationManager.AppSettings["FTPPassword"];
            FtpHelper ftp = new FtpHelper(Uri, UserName, Password);
            string sql = "select * from list_attachment where ordercode='" + ordercode + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                ftp.DeleteFile(dr["FILENAME"] + "");
            }
            sql = "delete from list_attachment where ordercode='" + ordercode + "'";
            DBMgr.ExecuteNonQuery(sql);
            //删除list_times信息
            sql = "delete from list_times where code='" + ordercode + "'";
            DBMgr.ExecuteNonQuery(sql);
            //删除集装箱信息
            sql = "delete from list_predeclcontainer where ordercode='" + ordercode + "'";
            DBMgr.ExecuteNonQuery(sql);
            //删除订单信息
            sql = "delete from  list_order where code='" + ordercode + "'";
            DBMgr.ExecuteNonQuery(sql);
            json = "{success:true}";
            return json;
        }

    }
}