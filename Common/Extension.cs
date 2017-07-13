using iTextSharp.text;
using iTextSharp.text.pdf;
//using MvcPlatform.Models;
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
                        LAWFLAG:'法检状况',CLEARANCENO:'通关单号',ASSOCIATEPEDECLNO:'出口报关单',REPUNITCODE:'报关申报单位',INSPUNITCODE:'报检申报单位',ORDERREQUEST:'需求备注'}";
                    break;
                case "10"://空运出口
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',TOTALNO:'总单号',DIVIDENO:'分单号',GOODSNUM:'件数',PACKKIND:'包装',GOODSGW:'毛重'',GOODSNW:'净重',CONTRACTNO:'合同号',ARRIVEDNO:'运抵编号',TURNPRENO:'转关预录号',
                        CLEARANCENO:'通关单号',DECLCARNO:'报关车号',ORDERREQUEST:'需求备注',LAWFLAG:'法检状况',WEIGHTCHECK:'需重量确认',ISWEIGHTCHECK:'重量确认'}";
                    break;
                case "11"://空运进口                    
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',TOTALNO:'总单号',GOODSNUM:'件数',PACKKIND:'包装',GOODSNW:'净重',CONTRACTNO:'合同号',TURNPRENO:'转关预录号',
                        CLEARANCENO:'通关单号',DECLCARNO:'报关车号',ORDERREQUEST:'需求备注',LAWFLAG:'法检状况'}";
                    break;
                case "20"://海运出口       
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',PACKKIND:'包装',GOODSNW:'净重',CONTRACTNO:'合同号',SECONDLADINGBILLNO:'提单号',ARRIVEDNO:'运抵编号',
                        LAWFLAG:'法检状况',CLEARANCENO:'通关单号',CONTAINERNO:'集装箱号',DECLCARNO:'报关车号',TURNPRENO:'转关预录号',ORDERREQUEST:'需求备注'}";
                    break;
                case "21"://海运进口     
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',PACKKIND:'包装',GOODSNW:'净重',CONTRACTNO:'合同号',FIRSTLADINGBILLNO:'国检提单号',SECONDLADINGBILLNO:'海关提单号',TRADEWAYCODES_ZS:'贸易方式',
                        TURNPRENO:'转关预录号',WOODPACKINGID:'木质包装',CLEARANCENO:'通关单号',LAWFLAG:'法检状况',CONTAINERNO:'集装箱号',DECLCARNO:'报关车号',ORDERREQUEST:'需求备注'}";
                    break;
                case "30"://陆运出口       
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',FILGHTNO:'航次号',CONTRACTNO:'合同号',PACKKIND:'包装',GOODSNW:'净重',ARRIVEDNO:'运抵编号',LAWFLAG:'法检状况',
                        CLEARANCENO:'通关单号',CONTAINERNO:'集装箱号',DECLCARNO:'报关车号',TURNPRENO:'转关预录号',TOTALNO:'总单号',DIVIDENO:'分单号',ORDERREQUEST:'需求备注'}";
                    break;
                case "31"://陆运进口     
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',FILGHTNO:'航次号',DIVIDENO:'分单号',GOODSNUM:'件数',PACKKIND:'包装',GOODSNW:'净重',CONTRACTNO:'合同号',MANIFEST:'载货清单号',
                        CLEARANCENO:'通关单号',LAWFLAG:'法检状况',CONTAINERNO:'集装箱号',DECLCARNO:'报关车号',ORDERREQUEST:'需求备注'}";
                    break;
                case "50"://特殊区域出口                         
                case "51"://特殊区域进口      
                    JsonFieldComments = @"{SPECIALRELATIONSHIP:'特殊关系确认',PRICEIMPACT:'价格影响确认',PAYPOYALTIES:'支付特许权使用费确认',
                        CUSNO:'客户编号',PACKKIND:'包装',GOODSNW:'净重',CONTRACTNO:'合同号',TURNPRENO:'对方转关号',LAWFLAG:'法检状况',CLEARANCENO:'通关单号',
                        GOODSTYPEID:'货物类型',CONTAINERNO:'集装箱号',DECLCARNO:'报关车号',ORDERREQUEST:'需求备注',BUSITYPE:'业务类型'}";
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

        public static void MergePDFFiles(IList<string> fileList, string outMergeFile)
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
                    else if (colname == "ORDERREQUEST")//需求备注一直可以修改，不参与判断
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

            if (ParaType == "predata")//ListPreData.cshtml 导入用的
            {
                return "{sbgq:" + json_sbgq + ",bgfs:" + json_bgfs + ",myfs:" + json_myfs + "}";
            }

            return "{jydw:" + json_jydw + ",sbfs_all:" + json_sbfs_all + ",sbfs:" + json_sbfs + ",sbgq:" + json_sbgq + ",bgfs:" + json_bgfs + ",bzzl:" + json_bzzl
                + ",myfs:" + json_myfs + ",containertype:" + json_containertype + ",containersize:" + json_containersize + ",truckno:" + json_truckno
                + ",relacontainer:" + json_relacontainer + ",mzbz:" + json_mzbz + ",jylb:" + json_jylb + ",json_sbkb:" + json_sbkb
                + ",inspbzzl:" + json_inspbzzl + ",adminurl:'" + AdminUrl + "',curuser:" + json_user
                + ",dzfwdw:" + json_dzfwdw + ",inspmyfs:" + json_inspmyfs + "}";
        }

    }
}