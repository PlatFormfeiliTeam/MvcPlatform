using Aspose.Cells;
using iTextSharp.text;
using iTextSharp.text.pdf;
//using MvcPlatform.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
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
            JObject json_account = (JObject)JsonConvert.DeserializeObject(account);

            IDatabase db = SeRedis.redis.GetDatabase();
            string result = "";
            if (db.KeyExists(account))
            {
                //result = db.StringGet(account);
                db.KeyDelete(account);
            }
            //else
            //{
                //2016-08-02增加字段报关服务单位SCENEDECLAREID 报检服务单位SCENEINSPECTID 因为订单里面创建时取当前用户的默认值 故提前放到缓存
                //CUSTOMERID 这个字段在sysuser表中有
                string sql = @"select c.NAME as CUSTOMERNAME,c.HSCODE as CUSTOMERHSCODE,c.CIQCODE as CUSTOMERCIQCODE,c.CODE CUSTOMERCODE
                                    ,c.SCENEDECLAREID,c.SCENEINSPECTID,d.NAME as REPUNITNAME,e.NAME as INSPUNITNAME,c.ISRECEIVER,c.ISCUSTOMER
                                    ,c.DOCSERVICECOMPANY
                                    ,u.* 
                            from SYS_USER u 
                                 left join cusdoc.sys_customer c on u.customerid = c.id 
                                 left join cusdoc.base_company d on c.hscode=d.code
                                 left join cusdoc.base_company e on c.ciqcode=e.INSPCODE 
                            where lower(u.name) ='" + json_account.Value<string>("NAME").ToLower() + "' and lower(c.code)='" + json_account.Value<string>("CUSTOMERCODE").ToLower() + "'";
                DataTable dt = DBMgr.GetDataTable(sql);

                IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                string jsonstr = JsonConvert.SerializeObject(dt, iso);
                jsonstr = jsonstr.Replace("[", "").Replace("]", "");
                //db.StringSet(account, jsonstr);
                result = jsonstr;
            //}
            return (JObject)JsonConvert.DeserializeObject(result);
        }
        /*
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
                             c.SCENEDECLAREID,c.SCENEINSPECTID,d.NAME as REPUNITNAME,e.NAME as INSPUNITNAME,u.* from SYS_USER u 
                             left join cusdoc.sys_customer c on u.customerid = c.id 
                             left join cusdoc.base_company d on c.hscode=d.code
                             left join cusdoc.base_company e on c.ciqcode=e.INSPCODE where u.name ='" + account + "'";
                DataTable dt = DBMgr.GetDataTable(sql);
                IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
                iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                string jsonstr = JsonConvert.SerializeObject(dt, iso);
                jsonstr = jsonstr.Replace("[", "").Replace("]", "");
                db.StringSet(account, jsonstr);
                result = jsonstr;
            }
            return (JObject)JsonConvert.DeserializeObject(result);
        }*/

        public static bool Check_Customer(string customerid)//true:有权限，false无权限
        {
            DataTable dt = DBMgrBase.GetDataTable("select * from sys_customer where id='" + customerid + "'");
            if (dt.Rows[0]["SOCIALCREDITNO"].ToString() == "N")
            {
                return false;
            }
            return true;
        }

        //获取订单CODE
        public static string getOrderCode()
        {
            string code = string.Empty;
            try
            {;
                OracleParameter[] parms = new OracleParameter[3];
                parms[0] = new OracleParameter("p_prefix", OracleDbType.NVarchar2, 20, ParameterDirection.Input);
                parms[1] = new OracleParameter("p_type", OracleDbType.NVarchar2, 10, ParameterDirection.Input);
                parms[2] = new OracleParameter("p_increase", OracleDbType.Int32, ParameterDirection.Output);

                string prefix = getPrefix();
                parms[0].Value = prefix;
                parms[1].Value = "order";//O是订单 

                DBMgr.ExecuteNonQueryParm("p_sequencegenerator", parms);
                code = prefix + Convert.ToInt32(parms[2].Value.ToString()).ToString("00000"); 
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return code;

        }

        /*
        /// <summary> 
        /// 获取订单编号 
        /// </summary> 
        /// <param name="cmd"></param> 
        /// <returns></returns> 
        private static string getCode(OracleCommand cmd)
        {
            string code = "";
            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "p_sequencegenerator";
                OracleParameter[] parm = new OracleParameter[3];
                parm[0] = new OracleParameter("p_prefix", OracleDbType.NVarchar2, 20);
                parm[0].Direction = ParameterDirection.Input;
                parm[1] = new OracleParameter("p_type", OracleDbType.NVarchar2, 10);
                parm[1].Direction = ParameterDirection.Input;
                parm[2] = new OracleParameter("p_increase", OracleDbType.Int32);
                parm[2].Direction = ParameterDirection.Output;
                string prefix = getPrefix();
                parm[0].Value = prefix;
                parm[1].Value = "order";//O是订单 
                cmd.Parameters.AddRange(parm);

                int i = cmd.ExecuteNonQuery();
                code = prefix + parm[2].Value.ToInt().ToString("00000"); 
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return code;
        }*/
        /// <summary> 
        /// 获取前缀 
        /// </summary> 
        /// <returns></returns> 
        private static string getPrefix()
        {
            DateTime time = DateTime.Now;
            string code = "";
            code += time.Year.ToString().Substring(2);
            code += time.Month.ToString("00");
            code += time.Day.ToString("00");
            return code;
        }


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
        public static void Update_Attachment_ForEnterprise(string entorder_id, string filedata, string originalfileids, JObject json_user,string path)
        {
            if (!string.IsNullOrEmpty(filedata))
            {
                string webFilePath = ConfigurationManager.AppSettings["WebFilePath"];
                System.Uri Uri = new Uri("ftp://" + ConfigurationManager.AppSettings["FTPServer"] + ":" + ConfigurationManager.AppSettings["FTPPortNO"]);
                string UserName = ConfigurationManager.AppSettings["FTPUserName"];
                string Password = ConfigurationManager.AppSettings["FTPPassword"];
                FtpHelper ftp = new FtpHelper(Uri, UserName, Password);
                JArray jarry = JsonConvert.DeserializeObject<JArray>(filedata);
                string sql = "";
                string remote = path;
                if (remote == "")
                {
                    remote = DateTime.Now.ToString("yyyy-MM-dd");
                }
                
                IDatabase db = SeRedis.redis.GetDatabase(); 
                foreach (JObject json in jarry)
                {
                    if (string.IsNullOrEmpty(json.Value<string>("ID")))
                    {
                       
                        string filename = "/" + remote + "/" + json.Value<string>("NEWNAME");
                        string sizes = json.Value<string>("SIZES");
                        string filetypename = json.Value<string>("FILETYPENAME");
                        string extname = json.Value<string>("ORIGINALNAME").ToString().Substring(json.Value<string>("ORIGINALNAME").ToString().LastIndexOf('.') + 1);

                        if (json.Value<string>("ORIGINALNAME").IndexOf(".txt") > 0 || json.Value<string>("ORIGINALNAME").IndexOf(".TXT") > 0)
                        {
                            try
                            {

                                string[] split = json.Value<string>("NEWNAME").Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                                string oldName = webFilePath + json.Value<string>("NEWNAME");
                                string newName = webFilePath + split[0] + "_0." + split[1];

                                FileInfo fi = new FileInfo(oldName);
                                fi.MoveTo(Path.Combine(newName));
                                StreamReader sr = new StreamReader(newName, Encoding.GetEncoding("BIG5"));
                                String line;
                                FileStream fs = new FileStream(oldName, FileMode.Create);
                                while ((line = sr.ReadLine()) != null)
                                {
                                    byte[] dst = Encoding.UTF8.GetBytes(line);
                                    fs.Write(dst, 0, dst.Length);
                                    fs.WriteByte(13);
                                    fs.WriteByte(10);
                                }
                                fs.Flush();
                                fs.Close();
                            }
                            catch (Exception)
                            {

                                throw;
                            }

                        }

                        sql = @"insert into LIST_ATTACHMENT (id,filename,originalname,filetype,uploadtime,uploaduserid,customercode,entid,
                        sizes,filetypename,filesuffix) values(List_Attachment_Id.Nextval,'{0}','{1}','{2}',sysdate,{3},'{4}','{5}','{6}','{7}','{8}')";
                        sql = string.Format(sql, filename, json.Value<string>("ORIGINALNAME"), json.Value<string>("FILETYPE"), json_user.Value<string>("ID"), 
                        json_user.Value<string>("CUSTOMERCODE"), entorder_id, sizes, filetypename, extname);
                        DBMgr.ExecuteNonQuery(sql);

                        if (json.Value<string>("ORIGINALNAME").IndexOf(".txt") > 0 || json.Value<string>("ORIGINALNAME").IndexOf(".TXT") > 0)
                        {
                            db.ListRightPush("compal_sheet_topdf_queen", "{ENTID:'" + entorder_id + "',FILENAME:'" + filename + "'}");
                        }
                       
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
                   //移除转PDF的缓存
                    db.ListRemove("compal_sheet_topdf_queen", "{ENTID:'" + id + "',FILENAME:'" + dt.Rows[0]["FILENAME"].ToString()+ "'}");
                }
            }
        }
        
        //订单的状态在草稿、文件已上传、订单已委托 三个状态发生时记录到订单状态变更日志
        public static void add_list_time(int status, string ordercode, JObject json_user)
        {
            if (status != 0 && status != 10)
            {
                return;
            }

            string sql_search = @"select count(*) from list_times where code = '{0}' and status = '{1}'";
            string sql_insert = @"insert into list_times(id,code,userid,realname,status,times,type,ispause) values(list_times_id.nextval,'{0}','{1}','{2}','{3}',sysdate,'0','0')";

            string sql = ""; int i = 0; int CreateUserId = json_user.Value<Int32>("ID"); string CreateUserName = json_user.Value<string>("REALNAME");

             int[] status_array = new int[] { 0, 10 };
             foreach (int status_tmp in status_array)
             {
                 if (status >= status_tmp)
                 {
                     sql = string.Format(sql_search, ordercode, status_tmp);
                     i = Convert.ToInt32(DBMgr.GetDataTable(sql).Rows[0][0]);
                     if (i == 0)
                     {
                         sql = string.Format(sql_insert, ordercode, CreateUserId, CreateUserName, status_tmp);
                         DBMgr.ExecuteNonQuery(sql);
                     }
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
                        LAWFLAG:'法检状况',CLEARANCENO:'通关单号',ASSOCIATEPEDECLNO:'出口报关单',REPUNITCODE:'报关申报单位',INSPUNITCODE:'报检申报单位',ORDERREQUEST:'需求备注',CLEARREMARK:'结算备注'}";
                    break;
                case "10"://空运出口
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',TOTALNO:'总单号',DIVIDENO:'分单号',GOODSNUM:'件数',PACKKIND:'包装',GOODSGW:'毛重',GOODSNW:'净重',CONTRACTNO:'合同号',ARRIVEDNO:'运抵编号',TURNPRENO:'转关预录号',
                        CLEARANCENO:'通关单号',DECLCARNO:'报关车号',ORDERREQUEST:'需求备注',LAWFLAG:'法检状况',WEIGHTCHECK:'需重量确认',ISWEIGHTCHECK:'重量确认',CLEARREMARK:'结算备注'}";
                    break;
                case "11"://空运进口                    
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',TOTALNO:'总单号',GOODSNUM:'件数',PACKKIND:'包装',GOODSNW:'净重',CONTRACTNO:'合同号',TURNPRENO:'转关预录号',
                        CLEARANCENO:'通关单号',DECLCARNO:'报关车号',ORDERREQUEST:'需求备注',LAWFLAG:'法检状况',CLEARREMARK:'结算备注'}";
                    break;
                case "20"://海运出口       
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',PACKKIND:'包装',GOODSNW:'净重',CONTRACTNO:'合同号',SECONDLADINGBILLNO:'提单号',ARRIVEDNO:'运抵编号',
                        LAWFLAG:'法检状况',CLEARANCENO:'通关单号',CONTAINERNO:'集装箱号',DECLCARNO:'报关车号',TURNPRENO:'转关预录号',ORDERREQUEST:'需求备注',CLEARREMARK:'结算备注'}";
                    break;
                case "21"://海运进口     
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',PACKKIND:'包装',GOODSNW:'净重',CONTRACTNO:'合同号',FIRSTLADINGBILLNO:'国检提单号',SECONDLADINGBILLNO:'海关提单号',TRADEWAYCODES_ZS:'贸易方式',
                        TURNPRENO:'转关预录号',WOODPACKINGID:'木质包装',CLEARANCENO:'通关单号',LAWFLAG:'法检状况',CONTAINERNO:'集装箱号',DECLCARNO:'报关车号',ORDERREQUEST:'需求备注',CLEARREMARK:'结算备注'}";
                    break;
                case "30"://陆运出口       
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',FILGHTNO:'航次号',CONTRACTNO:'合同号',PACKKIND:'包装',GOODSNW:'净重',ARRIVEDNO:'运抵编号',LAWFLAG:'法检状况',
                        CLEARANCENO:'通关单号',CONTAINERNO:'集装箱号',DECLCARNO:'报关车号',TURNPRENO:'转关预录号',TOTALNO:'总单号',DIVIDENO:'分单号',ORDERREQUEST:'需求备注',CLEARREMARK:'结算备注'}";
                    break;
                case "31"://陆运进口     
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',FILGHTNO:'航次号',DIVIDENO:'分单号',GOODSNUM:'件数',PACKKIND:'包装',GOODSNW:'净重',CONTRACTNO:'合同号',MANIFEST:'载货清单号',
                        CLEARANCENO:'通关单号',LAWFLAG:'法检状况',CONTAINERNO:'集装箱号',DECLCARNO:'报关车号',ORDERREQUEST:'需求备注',CLEARREMARK:'结算备注'}";
                    break;
                case "50"://特殊区域出口                         
                case "51"://特殊区域进口      
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',PACKKIND:'包装',GOODSNW:'净重',CONTRACTNO:'合同号',TURNPRENO:'对方转关号',LAWFLAG:'法检状况',CLEARANCENO:'通关单号',
                        GOODSTYPEID:'货物类型',CONTAINERNO:'集装箱号',DECLCARNO:'报关车号',ORDERREQUEST:'需求备注',BUSITYPE:'业务类型',CLEARREMARK:'结算备注'}";
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

            string sql = "select * from list_attachmentdetail where ordercode='" + ordercode + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                ftp.DeleteFile(@"/" + dr["SOURCEFILENAME"] + "");
            }
            sql = "delete from list_attachmentdetail where ordercode='" + ordercode + "'";
            DBMgr.ExecuteNonQuery(sql);

            sql = "select * from list_attachment where ordercode='" + ordercode + "'";
            dt = DBMgr.GetDataTable(sql);
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

        //文件委托 合并打印处使用
        public static void MergePDFFiles_EntOrder(IList<string> fileList, string outMergeFile)
        {
            int rotation = 0;
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

                    newPage = writer.GetImportedPage(reader, j);
                    document.SetPageSize(new Rectangle(reader.GetPageSizeWithRotation(j).Width, reader.GetPageSizeWithRotation(j).Height));
                    document.NewPage();
                   // newPage = writer.GetImportedPage(reader, j);
                   // cb.AddTemplate(newPage, 0, 0);
                    rotation = reader.GetPageRotation(j);
                    switch (rotation)
                    {
                        case 90:
                            cb.AddTemplate(newPage, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(j).Height);
                            break;
                        case 180:
                            cb.AddTemplate(newPage, -1f, 0, 0, -1f, reader.GetPageSizeWithRotation(j).Width, reader.GetPageSizeWithRotation(j).Height);
                            break;
                        case 270:
                            cb.AddTemplate(newPage, 0, 1f, -1f, 0, reader.GetPageSizeWithRotation(j).Width, 0);
                            break;
                        default:
                            cb.AddTemplate(newPage, 1f, 0, 0, 1f, 0, 0);//等同于
                            break;
                    }
                }
            }
            document.Close();
        }

        public static string getUpdateSql(string allcol, string ordercode, bool IsSubmitAfterSave)
        {
            string sql = "";
            string sql_list = @"SELECT " + allcol + " FROM LIST_ORDER WHERE CODE = '" + ordercode + "'";

            DataTable dt_list = DBMgr.GetDataTable(sql_list);
            if (dt_list.Rows.Count == 1)
            {
                string colname;
                for (int i = 1; i < dt_list.Columns.Count; i++)
                {
                    colname = ""; colname = dt_list.Columns[i].ColumnName;

                    if (colname == "SUBMITTIME")
                    {
                        sql += colname + "={" + i + "},";
                    }
                    else if (colname == "ORDERREQUEST" || colname == "CLEARREMARK")//需求备注，结算备注 一直可以修改，不参与判断
                    {
                        sql += colname + "='{" + i + "}',";
                    }
                    else if (colname == "STATUS" || colname == "DECLSTATUS" || colname == "INSPSTATUS")
                    {
                        if (IsSubmitAfterSave == false)//委托之前保存，需更新状态；委托后保存，就不更新状态
                        {
                            sql += colname + "='{" + i + "}',";
                        }
                    }
                    else
                    {
                        if (IsSubmitAfterSave)//委托之后
                        {
                            if (colname == "SPECIALRELATIONSHIP" || colname == "PRICEIMPACT" || colname == "PAYPOYALTIES" || colname == "SPECIALRELATIONSHIP"
                                || colname == "LAWFLAG" || colname == "WEIGHTCHECK" || colname == "ISWEIGHTCHECK")//checkbox不勾选的时候，会赋默认值0
                            {
                                if ((dt_list.Rows[0][i] + "") == "" || (dt_list.Rows[0][i] + "") == "0")
                                {
                                    sql += colname + "='{" + i + "}',";
                                }
                            }
                            else
                            {
                                if ((dt_list.Rows[0][i] + "") == "")
                                {
                                    sql += colname + "='{" + i + "}',";
                                }
                            }
                           
                        }
                        else
                        {
                            sql += colname + "='{" + i + "}',";
                        }                       
                    }
                }
                sql = sql.Substring(0, sql.Length - 1); //去掉末尾逗号
                sql = @"UPDATE LIST_ORDER SET " + sql + " WHERE CODE = '{0}'";
            }
            return sql;
        }

        public static string getPathname(string filename, NPOI.HSSF.UserModel.HSSFWorkbook book)
        {
            string WebDownPath = ConfigurationManager.AppSettings["WebDownPath"];

            if (!Directory.Exists(WebDownPath))
                Directory.CreateDirectory(WebDownPath);

            string filename_s = DateTime.Now.ToString("yyMMddHHmmssfff") + ".xls";
            string path = WebDownPath + filename_s;

            FileStream fs = new FileStream(path, FileMode.Create);
            book.Write(fs);
            fs.Close();
            return "{filename_s:'" + filename_s + "',filename:'" + filename + "'}";
        }

        //基础资料 20170713
        public static string Ini_Base_Data(JObject json_user, string ParaType, string busitype)
        {
            string AdminUrl = ConfigurationManager.AppSettings["AdminUrl"];
            string ISRECEIVER = json_user.Value<string>("ISRECEIVER");//add 20180101

            IDatabase db = SeRedis.redis.GetDatabase();
            string sql = "";

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

            string json_unit = "[]";//单位 :公用
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

            if (ParaType == "recordinfo")//备案信息
            {
                //========================================备案信息============================================================================
                string json_recordid = "[]";//账册号
                sql = @"select id,code,code||'('||bookattribute||')' as name from sys_recordinfo where enabled=1 and busiunit= '" + json_user.Value<string>("CUSTOMERHSCODE") + "' order by id";
                json_recordid = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

                string json_customarea = "[]";//备案关区
                sql = @"select id,name,customarea,name||'('||customarea||')' as customareaname from cusdoc.base_year where customarea is not null and enabled=1 order by id";
                json_customarea = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

                //============================================================================================================================

                return "{jydw:" + json_jydw + ",recordid:" + json_recordid + ",unit:" + json_unit + ",customarea:" + json_customarea + "}";
            }

            string json_sbfs_all = "[]";//申报方式
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
            //if (db.KeyExists("common_data:dzfwdw"))
            //{
            //    json_dzfwdw = db.StringGet("common_data:dzfwdw");
            //}
            //else
            //{
            sql = @"select * from sys_customer where DOCSERVICECOMPANY=1";
            json_dzfwdw = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
            //db.StringSet("common_data:dzfwdw", json_dzfwdw);
            //}

            //委托单位or结算单位
            string json_wtdw = "[]";
            //if (db.KeyExists("common_data:wtdw"))
            //{
            //    json_wtdw = db.StringGet("common_data:wtdw");
            //}
            //else
            //{
            sql = @"select a.*,NAME||'('||CODE||')' CODENAME from sys_customer a where (ISCUSTOMER=1 or ISCOMPANY=1) and enabled=1";
            json_wtdw = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));
            //db.StringSet("common_data:wtdw", json_wtdw);
            //}

            //费用状态
            string json_fyzt= "[]";
            //if (db.KeyExists("common_data:fyzt"))
            //{
            //    json_fyzt = db.StringGet("common_data:fyzt");
            //}
            //else
            //{
                sql = @"select a.*,NAME||'('||CODE||')' CODENAME from finance_status a order by code";
                json_fyzt = JsonConvert.SerializeObject(DBMgr.GetDataTable(sql));
            //    db.StringSet("common_data:fyzt", json_fyzt);
            //}

            if (ParaType == "predata")//ListPreData.cshtml 导入用的
            {
                return "{sbgq:" + json_sbgq + ",bgfs:" + json_bgfs + ",myfs:" + json_myfs + ",unit:" + json_unit + ",dzfwdw:" + json_dzfwdw + "}";
            }

            if (ParaType == "verification")//VerificationList.cshtml 导入用的
            {
                return "{myfs:" + json_myfs + ",unit:" + json_unit + "}";
            }
            if (ParaType == "filerecoginze")//List_FileRecoginze.cshtml 导入用的
            {
                return "{jydw:" + json_jydw + "}";
            }

            if (ParaType == "CustomsService")//关务服务
            {
                return "{jydw:" + json_jydw + ",wtdw:" + json_wtdw + ",fyzt:" + json_fyzt + "}";
            }

            return "{jydw:" + json_jydw + ",sbfs_all:" + json_sbfs_all + ",sbfs:" + json_sbfs + ",sbgq:" + json_sbgq + ",bgfs:" + json_bgfs + ",bzzl:" + json_bzzl
                + ",myfs:" + json_myfs + ",containertype:" + json_containertype + ",containersize:" + json_containersize + ",truckno:" + json_truckno
                + ",relacontainer:" + json_relacontainer + ",mzbz:" + json_mzbz + ",jylb:" + json_jylb + ",json_sbkb:" + json_sbkb
                + ",inspbzzl:" + json_inspbzzl + ",adminurl:'" + AdminUrl + "',curuser:" + json_user
                + ",dzfwdw:" + json_dzfwdw + ",inspmyfs:" + json_inspmyfs + ",wtdw:" + json_wtdw + ",isreceiver:'" + ISRECEIVER + "'}";
        }


        public static DataTable GetExcelData_Table(string filePath,int sheetPoint)
        {
            Workbook book = new Workbook(filePath);
            //book.Open(filePath);
            Worksheet sheet = book.Worksheets[sheetPoint];
            Cells cells = sheet.Cells;
            DataTable dt_Import = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);//获取excel中的数据保存到一个datatable中
            return dt_Import;

        }


        /*
        public static string ImExcel_Verification_Data(DataTable dtExcel, DataTable dtExcel_sub, string datadource, JObject json_user)
        {
            if (dtExcel == null || dtExcel.Rows.Count <= 0)
            {
                return "{success:false,error:'导入资料为空'}";
            }
            if (dtExcel_sub == null || dtExcel_sub.Rows.Count <= 0)
            {
                return "{success:false,error:'导入资料为空'}";
            }
            //验证列名称
            string data = "";
            foreach (DataColumn column in dtExcel.Columns)
            {
                data = data + column.ColumnName.TrimEnd() + "/";
            }

            if (data != "报关单号/申报单位代码/征免性质/申报日期/贸易方式/经营单位代码/账册号/" || dtExcel.Columns.Count != 7)
            {
                return "{success:false,error:'列名不正确'}";
            }

            data = "";
            foreach (DataColumn column in dtExcel_sub.Columns)
            {
                data = data + column.ColumnName.TrimEnd() + "/";
            }

            if (data != "序号/项号/商品编号/商品名称/征免/成交数量/成交单位/币制/总价/报关单号/" || dtExcel_sub.Columns.Count != 10)
            {
                return "{success:false,error:'列名不正确'}";
            }
            //====================================================================================================
            string result = "{success:true,json:[]}";

            string status = "待比对";
            string createuserid = json_user.Value<string>("ID"), createusername = json_user.Value<string>("REALNAME");

            string declarationcode = "", repunitcode = "", kindoftax = "", reptime = "", trademethod = "", busiunitcode = "", recordcode = "";
            dtExcel.Columns.Add("ERRORMSG"); DataTable dt_error = dtExcel.Clone();
            string sql = ""; DataTable dt_exists = new DataTable();
            string sql_excel_insert = @"insert into list_verification(id 
                                ,datadource, declarationcode, repunitcode, kindoftax, reptime ,trademethod
                                , busiunitcode, recordcode, createuserid, createusername ,createtime, status
                            ) VALUES ( list_verification_id.Nextval
                                ,'{0}','{1}','{2}','{3}',to_date('{4}','yyyy/mm/dd'),'{5}'
                                ,'{6}','{7}','{8}','{9}',sysdate,'{10}'
                                )";
            string sql_excel_update = @"update list_verification set datadource='{0}',repunitcode='{2}',kindoftax='{3}',reptime=to_date('{4}','yyyy/mm/dd') ,trademethod='{5}'
                                            , busiunitcode='{6}', recordcode='{7}', createuserid='{8}', createusername='{9}' ,createtime=sysdate, status='{10}'
                                        where declarationcode='{1}'";

            string sql_sub = @"insert into list_verification_sub(id 
                                ,declarationcode, orderno, itemno, commodityno, commodityname, taxpaid
                                ,cadquantity, cadunit, currencycode, totalprice
                           ) VALUES ( list_verification_sub_id.Nextval
                                ,'{0}','{1}','{2}','{3}','{4}','{5}'
                                ,'{6}','{7}','{8}','{9}')";

            OracleConnection conn = null;
            OracleTransaction ot = null;
            conn = DBMgr.getOrclCon();

            for (int i = 0; i < dtExcel.Rows.Count; i++)
            {
                if (dtExcel.Rows[i]["报关单号"].ToString().Trim() == "")
                {
                    break;
                }

                //置空，以免影响下次判断
                declarationcode = ""; repunitcode = ""; kindoftax = ""; reptime = ""; trademethod = ""; busiunitcode = ""; recordcode = "";
                sql = ""; dt_exists.Clear();

                declarationcode = dtExcel.Rows[i]["报关单号"].ToString().Trim();
                repunitcode = dtExcel.Rows[i]["申报单位代码"].ToString().Trim();
                kindoftax = dtExcel.Rows[i]["征免性质"].ToString().Trim();
                reptime = dtExcel.Rows[i]["申报日期"].ToString().Trim();
                trademethod = dtExcel.Rows[i]["贸易方式"].ToString().Trim();
                busiunitcode = dtExcel.Rows[i]["经营单位代码"].ToString().Trim();
                recordcode = dtExcel.Rows[i]["账册号"].ToString().Trim();

                DataRow[] dr_array = dtExcel_sub.Select("报关单号='" + declarationcode + "'");
                if (dr_array.Length <= 0)
                {
                    dtExcel.Rows[i]["ERRORMSG"] = "没有表体数据，不能导入";
                    dt_error.ImportRow(dtExcel.Rows[i]);
                    continue;
                }

                //判断经营单位是否是当前账号的海关十位编码
                if (busiunitcode != json_user.Value<string>("CUSTOMERHSCODE"))
                {
                    dtExcel.Rows[i]["ERRORMSG"] = "经营单位与当前企业编码(" + json_user.Value<string>("CUSTOMERHSCODE") + ")不一致，不能导入";
                    dt_error.ImportRow(dtExcel.Rows[i]);
                    continue;
                }


                //判断存在性
                dt_exists = DBMgr.GetDataTable("select declarationcode,status from list_verification where declarationcode='" + declarationcode + "'");
                if (dt_exists.Rows.Count <= 0)
                {
                    //insert
                    sql = string.Format(sql_excel_insert, datadource, declarationcode, repunitcode, kindoftax, reptime, trademethod
                                            , busiunitcode, recordcode, createuserid, createusername, status
                                            );
                }
                else
                {
                    if (dt_exists.Rows[0]["STATUS"].ToString() == "待比对" || dt_exists.Rows[0]["STATUS"].ToString() == "比对未通过")
                    {
                        //update
                        sql = string.Format(sql_excel_update, datadource, declarationcode, repunitcode, kindoftax, reptime, trademethod
                                            , busiunitcode, recordcode, createuserid, createusername, status
                                            );
                    }
                    else
                    {
                        dtExcel.Rows[i]["ERRORMSG"] = "状态为 " + dt_exists.Rows[0]["STATUS"].ToString() + " ，不能导入";
                        dt_error.ImportRow(dtExcel.Rows[i]);
                        continue;

                    }
                }

                try
                {
                    conn.Open();
                    ot = conn.BeginTransaction();

                    int recount = DBMgr.ExecuteNonQuery(sql, conn);
                    if (recount > 0)
                    {
                        //插入前先删除
                        DBMgr.ExecuteNonQuery("delete list_verification_sub where declarationcode='" + declarationcode + "'", conn);
                        foreach (DataRow item in dr_array)
                        {
                            sql = string.Format(sql_sub
                                , item["报关单号"].ToString(), item["序号"].ToString(), item["项号"].ToString(), item["商品编号"].ToString(), item["商品名称"].ToString(), item["征免"].ToString()
                                , item["成交数量"].ToString(), item["成交单位"].ToString(), item["币制"].ToString(), item["总价"].ToString());
                            DBMgr.ExecuteNonQuery(sql, conn);
                        }

                    }

                    //commit 之前先判断
                    if (dt_exists.Rows.Count <= 0)//add
                    {
                        dt_exists.Clear();
                        dt_exists = DBMgr.GetDataTable("select declarationcode,status from list_verification where declarationcode='" + declarationcode + "'");
                        if (dt_exists.Rows.Count <= 0)
                        {
                            ot.Commit();
                        }
                        else
                        {
                            ot.Rollback();
                            dtExcel.Rows[i]["ERRORMSG"] = "报关单号已经存在，不能导入";
                            dt_error.ImportRow(dtExcel.Rows[i]);
                        }
                    }
                    else//update
                    {
                        dt_exists.Clear();
                        dt_exists = DBMgr.GetDataTable("select declarationcode,status from list_verification where declarationcode='" + declarationcode + "'");
                        if (dt_exists.Rows[0]["STATUS"].ToString() == "待比对" || dt_exists.Rows[0]["STATUS"].ToString() == "比对未通过")
                        {
                            ot.Commit();
                        }
                        else
                        {
                            ot.Rollback();
                            dtExcel.Rows[i]["ERRORMSG"] = "状态为 " + dt_exists.Rows[0]["STATUS"].ToString() + " ，不能导入";
                            dt_error.ImportRow(dtExcel.Rows[i]);
                        }
                    }

                }
                catch (Exception ex)
                {
                    ot.Rollback();
                    dtExcel.Rows[i]["ERRORMSG"] = "导入数据异常:" + ex.Message;
                    dt_error.ImportRow(dtExcel.Rows[i]);

                }
                finally
                {
                    conn.Close();
                }

            }

            if (dt_error.Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dt_error);
                result = "{success:true,json:" + json + "}";
            }

            return result;

        }
        */
        
        public static string ImExcel_Verification_Data(DataTable dtExcel, string datadource, JObject json_user)
        {
            if (dtExcel == null || dtExcel.Rows.Count <= 0)
            {
                return "{success:false,error:'导入资料为空'}";
            }
            //验证列名称
            string data = "";
            foreach (DataColumn column in dtExcel.Columns)
            {
                data = data + column.ColumnName.TrimEnd() + "/";
            }

            if (data != "报关单号/申报单位代码/征免性质/申报日期/贸易方式/经营单位代码/账册号/合同号/业务类型/进出类型/序号/项号/商品编号/商品名称/征免/成交数量/成交单位/币制/总价/")
            {
                return "{success:false,error:'列名不正确'}";
            }
            //====================================================================================================
            string result = "{success:true,json:[]}";

            string status = "待比对";
            string createuserid = json_user.Value<string>("ID"), createusername = json_user.Value<string>("REALNAME");
            string declarationcode = "", repunitcode = "", kindoftax = "", reptime = "", trademethod = "", busiunitcode = "", recordcode = "", contractno = "", busitype = "", inouttype = "";
            DataTable dt_array_right = dtExcel.Clone();//记载sub的正确数据
            ArrayList attlist = new ArrayList();
            
            dtExcel.Columns.Add("ERRORMSG"); DataTable dt_error = dtExcel.Clone();//记载错误信息            

            string sql = ""; DataTable dt_exists = new DataTable();
            string sql_excel_insert = @"insert into list_verification(id 
                                ,datadource, declarationcode, repunitcode, kindoftax, reptime ,trademethod
                                , busiunitcode, recordcode, createuserid, createusername ,createtime, status
                                , contractno, busitype, inouttype
                            ) VALUES ( list_verification_id.Nextval
                                ,'{0}','{1}','{2}','{3}',to_date('{4}','yyyy/mm/dd'),'{5}'
                                ,'{6}','{7}','{8}','{9}',sysdate,'{10}'
                                ,'{11}','{12}','{13}'
                                )";
            string sql_excel_update = @"update list_verification set datadource='{0}',repunitcode='{2}',kindoftax='{3}',reptime=to_date('{4}','yyyy/mm/dd') ,trademethod='{5}'
                                            , busiunitcode='{6}', recordcode='{7}', createuserid='{8}', createusername='{9}' ,createtime=sysdate, status='{10}'
                                            , contractno='{11}', busitype='{12}', inouttype='{13}'
                                        where declarationcode='{1}'";

            string sql_sub = @"insert into list_verification_sub(id 
                                ,declarationcode, orderno, itemno, commodityno, commodityname, taxpaid
                                ,cadquantity, cadunit, currencycode, totalprice
                           ) VALUES ( list_verification_sub_id.Nextval
                                ,'{0}','{1}','{2}','{3}','{4}','{5}'
                                ,'{6}','{7}','{8}','{9}')";

            OracleConnection conn = null;
            OracleTransaction ot = null;
            conn = DBMgr.getOrclCon();

            for (int i = 0; i < dtExcel.Rows.Count; i++)
            {
                if (dtExcel.Rows[i]["报关单号"].ToString().Trim() == "")
                {
                    break;
                }

                //置空，以免影响下次判断
                declarationcode = ""; repunitcode = ""; kindoftax = ""; reptime = ""; trademethod = ""; busiunitcode = ""; recordcode = "";
                sql = ""; dt_exists.Clear();

                declarationcode = dtExcel.Rows[i]["报关单号"].ToString().Trim();
                repunitcode = dtExcel.Rows[i]["申报单位代码"].ToString().Trim();
                kindoftax = dtExcel.Rows[i]["征免性质"].ToString().Trim();
                reptime = dtExcel.Rows[i]["申报日期"].ToString().Trim();
                trademethod = dtExcel.Rows[i]["贸易方式"].ToString().Trim();
                busiunitcode = dtExcel.Rows[i]["经营单位代码"].ToString().Trim();
                recordcode = dtExcel.Rows[i]["账册号"].ToString().Trim();
                contractno = dtExcel.Rows[i]["合同号"].ToString().Trim();
                busitype = dtExcel.Rows[i]["业务类型"].ToString().Trim();
                inouttype = dtExcel.Rows[i]["进出类型"].ToString().Trim();

                //----------------------------------------------------------------check data--------------------------------------------------------

                if (attlist.Contains(declarationcode))//重复的报关单号只做第一笔的主表数据
                {
                    continue;
                }

                attlist.Add(declarationcode);               

                //判断经营单位是否是当前账号的海关十位编码
                if (busiunitcode != json_user.Value<string>("CUSTOMERHSCODE"))
                {
                    dtExcel.Rows[i]["ERRORMSG"] = "经营单位与当前企业编码(" + json_user.Value<string>("CUSTOMERHSCODE") + ")不一致，不能导入";
                    dt_error.ImportRow(dtExcel.Rows[i]);
                    continue;
                }

                //判断存在性
                dt_exists = DBMgr.GetDataTable("select declarationcode,datadource,status from list_verification where declarationcode='" + declarationcode + "'");
                if (dt_exists.Rows.Count <= 0)
                {
                    //insert
                    sql = string.Format(sql_excel_insert, datadource, declarationcode, repunitcode, kindoftax, reptime, trademethod
                                            , busiunitcode, recordcode, createuserid, createusername, status
                                            , contractno, busitype, inouttype
                                            );
                }
                else
                {
                    if (dt_exists.Rows[0]["STATUS"].ToString() != "比对中")
                    {
                        if (datadource == "线下")//来源是线下的：可以替换线下的，不可以替换线上的
                        {
                            if (dt_exists.Rows[0]["DATADOURCE"].ToString() == "线下")
                            {
                                //update
                                sql = string.Format(sql_excel_update, datadource, declarationcode, repunitcode, kindoftax, reptime, trademethod
                                                    , busiunitcode, recordcode, createuserid, createusername, status
                                                    , contractno, busitype, inouttype
                                                    );
                            }
                            else
                            {
                                dtExcel.Rows[i]["ERRORMSG"] = "此笔报关单是线上数据 ，不能导入";
                                dt_error.ImportRow(dtExcel.Rows[i]);
                                continue;
                            }
                        }
                        else//来源是线上的：可以替换任何数据
                        {
                            //update
                            sql = string.Format(sql_excel_update, datadource, declarationcode, repunitcode, kindoftax, reptime, trademethod
                                                , busiunitcode, recordcode, createuserid, createusername, status
                                                , contractno, busitype, inouttype
                                                );
                        }
                        
                       
                    }
                    else
                    {
                        dtExcel.Rows[i]["ERRORMSG"] = "状态为 比对中，不能导入";
                        dt_error.ImportRow(dtExcel.Rows[i]);
                        continue;

                    }
                }
                //--------------------------------------------------------check data sub-----------------------------------------------------------------------------

                DataRow[] dr_array = dtExcel.Select("报关单号='" + declarationcode + "'");
                if (dr_array.Length <= 0)
                {
                    dtExcel.Rows[i]["ERRORMSG"] = "没有表体数据，不能导入";
                    dt_error.ImportRow(dtExcel.Rows[i]);
                    continue;
                }
                dt_array_right.Clear();

                int rowid = -1; string rowmsg = ""; int errorrow = 0;
                foreach (DataRow item in dr_array)
                {
                    rowid = -1; rowmsg = "";

                    if (item["序号"].ToString().Trim() == "") { rowmsg += "序号为空，不能导入;"; }
                    if (item["商品编号"].ToString().Trim() == "") { rowmsg += "商品编号为空，不能导入;"; }
                    if (item["商品名称"].ToString().Trim() == "") { rowmsg += "商品名称为空，不能导入;"; }
                    if (item["征免"].ToString().Trim() == "") { rowmsg += "征免为空，不能导入;"; }
                    if (item["成交数量"].ToString().Trim() == "") { rowmsg += "成交数量为空，不能导入;"; }
                    if (item["成交单位"].ToString().Trim() == "") { rowmsg += "成交单位为空，不能导入;"; }
                    if (item["成交单位"].ToString().Trim() == "") { rowmsg += "成交单位为空，不能导入;"; }
                    if (item["币制"].ToString().Trim() == "") { rowmsg += "币制为空，不能导入;"; }
                    if (item["总价"].ToString().Trim() == "") { rowmsg += "总价为空，不能导入;"; }

                    if (rowmsg != "")
                    {
                        errorrow++;
                        rowid = item.Table.Rows.IndexOf(item);
                        dtExcel.Rows[rowid]["ERRORMSG"] = rowmsg;
                        dt_error.ImportRow(dtExcel.Rows[rowid]);
                        continue;
                    }
                    dt_array_right.ImportRow(item);
                }

                if (errorrow == dr_array.Length)//表体全部有错，不能导入（错误信息已经写在每行了）
                {
                    continue;
                }

                //----------------------------------------------------------------执行数据库-----------------------------------------------------------------------------------
                try
                {
                    conn.Open();
                    ot = conn.BeginTransaction();

                    int recount = DBMgr.ExecuteNonQuery(sql, conn);
                    if (recount > 0)
                    {
                        //插入前先删除
                        DBMgr.ExecuteNonQuery("delete list_verification_sub where declarationcode='" + declarationcode + "'", conn);
                        foreach (DataRow item_right in dt_array_right.Rows)
                        {
                            sql = string.Format(sql_sub
                                , item_right["报关单号"].ToString(), item_right["序号"].ToString(), item_right["项号"].ToString(), item_right["商品编号"].ToString(), item_right["商品名称"].ToString(), item_right["征免"].ToString()
                                , item_right["成交数量"].ToString(), item_right["成交单位"].ToString(), item_right["币制"].ToString(), item_right["总价"].ToString());
                            DBMgr.ExecuteNonQuery(sql, conn);
                        }

                    }

                    //commit 之前先判断
                    if (dt_exists.Rows.Count <= 0)//add
                    {
                        dt_exists.Clear();
                        dt_exists = DBMgr.GetDataTable("select declarationcode,datadource,status from list_verification where declarationcode='" + declarationcode + "'");
                        if (dt_exists.Rows.Count <= 0)
                        {
                            ot.Commit();
                        }
                        else
                        {
                            ot.Rollback();
                            dtExcel.Rows[i]["ERRORMSG"] = "报关单号已经存在，不能导入";
                            dt_error.ImportRow(dtExcel.Rows[i]);
                        }
                    }
                    else//update
                    {
                        dt_exists.Clear();
                        dt_exists = DBMgr.GetDataTable("select declarationcode,datadource,status from list_verification where declarationcode='" + declarationcode + "'");

                        if (datadource == "线下")//来源是线下的：可以替换线下的，不可以替换线上的
                        {

                            if (dt_exists.Rows[0]["DATADOURCE"].ToString() == "线下")
                            {
                                if (dt_exists.Rows[0]["STATUS"].ToString() != "比对中")
                                {
                                    ot.Commit();
                                }
                                else
                                {
                                    ot.Rollback();
                                    dtExcel.Rows[i]["ERRORMSG"] = "状态为 比对中，不能导入";
                                    dt_error.ImportRow(dtExcel.Rows[i]);
                                }
                            }
                            else
                            {
                                ot.Rollback();
                                dtExcel.Rows[i]["ERRORMSG"] = "此笔报关单是线上数据 ，不能导入";
                                dt_error.ImportRow(dtExcel.Rows[i]);
                            }
                        }
                        else//来源是线上的：可以替换任何数据
                        {
                            ot.Commit();
                        }
                    }

                }
                catch (Exception ex)
                {
                    ot.Rollback();
                    dtExcel.Rows[i]["ERRORMSG"] = "导入数据异常:" + ex.Message;
                    dt_error.ImportRow(dtExcel.Rows[i]);

                }
                finally
                {
                    conn.Close();
                }

            }

            if (dt_error.Rows.Count > 0)
            {
                var json = JsonConvert.SerializeObject(dt_error);
                result = "{success:true,json:" + json + "}";
            }

            return result;

        }


        public static string Verification(string declarationcode_list, string DATADOURCE, JObject json_user)
        {
            string sql = "";
            if (DATADOURCE == "线上")
            {
                sql = @"select a.DECLARATIONCODE 报关单号,a.REPUNITCODE 申报单位代码,a.KINDOFTAX 征免性质,to_char(a.REPTIME,'yyyymmdd') 申报日期
                                   ,a.TRADEMETHOD 贸易方式,a.BUSIUNITCODE 经营单位代码,a.RECORDCODE 账册号 
                                   ,a.contractno 合同号 
                                   ,(select sb.name from list_order lo 
                                     left join cusdoc.sys_busitype sb on lo.busitype=sb.code and enabled=1
                                      where lo.code=(select ld.ordercode from list_declaration ld where ld.code=a.code)) 业务类型
                                   ,case when substr(a.declarationcode,9,1)='1' then '进口' when substr(a.declarationcode,9,1)='0' then '出口' else '' end 进出类型
                                   ,b.ORDERNO 序号,b.ITEMNO 项号,b.COMMODITYNO||b.ADDITIONALNO 商品编号,b.COMMODITYNAME 商品名称,b.TAXPAID 征免
                                   ,b.CADQUANTITY 成交数量,b.CADUNIT 成交单位,b.CURRENCYCODE 币制,b.TOTALPRICE 总价
                            from (select * from list_declaration_after where declarationcode in ({0}) and csid=1) a
                                 left join (select * from list_decllist_after where isinvalid=0)b on a.CODE=b.predeclcode and a.xzlb=b.xzlb";
            }

            if (DATADOURCE == "线下")
            {
                sql = @"select a.DECLARATIONCODE 报关单号,a.REPUNITCODE 申报单位代码,a.KINDOFTAX 征免性质,to_char(a.REPTIME,'yyyymmdd') 申报日期
                                   ,a.TRADEMETHOD 贸易方式,a.BUSIUNITCODE 经营单位代码,a.RECORDCODE 账册号 
                                   ,a.CONTRACTNO 合同号 
                                   ,a.BUSITYPE 业务类型
                                   ,a.INOUTTYPE 进出类型
                                   ,b.ORDERNO 序号,b.ITEMNO 项号,b.COMMODITYNO 商品编号,b.COMMODITYNAME 商品名称,b.TAXPAID 征免
                                   ,b.CADQUANTITY 成交数量,b.CADUNIT 成交单位,b.CURRENCYCODE 币制,b.TOTALPRICE 总价
                            from (select * from list_verification where declarationcode in ({0})) a
                                 left join list_verification_sub b on a.declarationcode=b.declarationcode";
            }
            sql = string.Format(sql, declarationcode_list);

            DataTable dt = DBMgr.GetDataTable(sql);
            string result = Extension.ImExcel_Verification_Data(dt, "线上", json_user);
            return result;
        }

    }
}