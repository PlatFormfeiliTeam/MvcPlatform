using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;
using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using MvcPlatform.Models;
using Newtonsoft.Json.Linq;
using System.IO;

namespace MvcPlatform.Controllers
{
    public class AccountManagementController : Controller
    {
        //
        // GET: /AccountManagement/
        int totalProperty = 0;
        public ActionResult ChildAccount()
        {
            return View();
        }
        public ActionResult ChildEdit()
        {
            return View();
        }
        public ActionResult Authorization()
        {
            return View();
        }
        public string LoadCurrentUser()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = @"select t.CUSTOMERID,t.NAME,t.EMAIL,t.TELEPHONE,t.MOBILEPHONE,t.IMGPATH,t.COMPANYNAMES,t.TYPE,t.REALNAME,
                         b.SCENEDECLAREID,b.SCENEINSPECTID,b.Name COMPANYNAME,b.ISCUSTOMER,b.SELFCHECK,b.WEIGHTCHECK,b.BUSITYPES
                         from Sys_User t left join Sys_Customer b on t.CustomerId=b.Id
                         where t.id='" + json_user.GetValue("ID") + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            string json = JsonConvert.SerializeObject(dt);
            json = json.TrimStart('[').TrimEnd(']');
            return "{data:" + json + "}";
        }
        public string loaduserInfo()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd hh:mm:ss";
            string sql = @"select * from sys_user where parentid ='" + json_user.GetValue("ID") + "'";
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql));
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }
        private string GetPageSql(string tempsql)
        {
            int start = Convert.ToInt32(Request["start"]);
            int limit = Convert.ToInt32(Request["limit"]);
            string sql = "select count(1) from ( " + tempsql + " )";
            totalProperty = Convert.ToInt32(DBMgr.GetDataTable(sql).Rows[0][0]);
            string order = "CREATETIME";
            string asc = " desc";
            string pageSql = @"SELECT * FROM (SELECT * FROM (SELECT tt.*, ROWNUM AS rowno FROM ({0} ORDER BY {1} {2}) tt) WHERE rowno <= {4}) table_alias WHERE table_alias.rowno >= {3}";
            pageSql = string.Format(pageSql, tempsql, order, asc, start + 1, limit + start);
            return pageSql;
        }
        public string LoadSupplier()
        {
            string sql = "select t.* from sys_customer t where t.enabled=1 and t.isshipper=1";
            DataTable dt = DBMgr.GetDataTable(sql);
            string json = JsonConvert.SerializeObject(dt);
            return "{success:true,rows:" + json + "}";
        }

        public string UpdateConfig()
        {
            string data = Request["data"];
            try
            {
                JObject json = (JObject)JsonConvert.DeserializeObject(data);
                string sql = @"update sys_customer set SELFCHECK='{0}',WEIGHTCHECK='{1}',BUSITYPES='{2}' where id='{3}'";
                sql = string.Format(sql, json.Value<string>("SELFCHECK") == "on" ? 1 : 0, json.Value<string>("WEIGHTCHECK") == "on" ? 1 : 0, Request["busitypes"], json.Value<string>("CUSTOMERID"));
                int result = DBMgr.ExecuteNonQuery(sql);
                return "{result:'" + result + "'}";
            }
            catch (Exception ex)
            {
                return "{result: 0}";
            }
        }
        //账户编辑
        public string Update()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string formdata = Request["json"];
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            string sql = @"update sys_user set REALNAME = '{0}',EMAIL = '{1}',TELEPHONE = '{2}',MOBILEPHONE = '{3}' where ID = '" + json_user.GetValue("ID") + "'";
            sql = string.Format(sql, json.Value<string>("REALNAME"), json.Value<string>("EMAIL"), json.Value<string>("TELEPHONE"), json.Value<string>("MOBILEPHONE"));
            int result = DBMgr.ExecuteNonQuery(sql);
            return "{result:'" + result + "'}";
        }

        public string UpdatePassword()  //密码修改
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string Password = Request["PASSWORD"];
            string sql = @"update sys_user set password = '" + Password.ToSHA1() + "' where id = '" + json_user.GetValue("ID") + "'";
            int i = DBMgr.ExecuteNonQuery(sql);
            if (i > 0)
            {
                return "{result:true}";
            }
            else
            {
                return "{result:false}";
            }
        }

        public string ValidPassword()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string psd = Request["PASSWORD"];
            string sql = @"select * from  sys_user  where id = '" + json_user.GetValue("ID") + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            psd = psd.Trim().ToSHA1();
            if (psd == dt.Rows[0]["PASSWORD"] + "")
            {
                return "{success:true}";
            }
            else
            {
                return "{success:false}";
            }
        }

        public string SelectEdit()
        {
            string ID = Request["ID"];
            if (!string.IsNullOrEmpty(ID))
            {
                string sql = @"select ID, NAME,PASSWORD,REALNAME,EMAIL,TELEPHONE,MOBILEPHONE,ENABLED,SEX,POSITIONID,REMARK from sys_user where id = '" + ID + "'";
                DataTable dt = DBMgr.GetDataTable(sql);
                string json2 = JsonConvert.SerializeObject(dt);
                json2 = json2.TrimStart('[').TrimEnd(']');
                return "{success:true,data:" + json2 + "}";
            }
            return "";
        }
        public string Save()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = string.Empty;
            string formdata = Request["json"];
            JObject json = (JObject)JsonConvert.DeserializeObject(formdata);
            if (string.IsNullOrEmpty(Request["ID"]))
            {
                sql = @"insert into sys_user (ID,NAME,PASSWORD,REALNAME,EMAIL,TELEPHONE,MOBILEPHONE,ENABLED,SEX,PARENTID,REMARK,CREATETIME,CUSTOMERID,TYPE)
                        values(sys_user_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',sysdate,'{10}',2)";
                sql = string.Format(sql, json.Value<string>("NAME"), json.Value<string>("NAME").ToSHA1(), json.Value<string>("REALNAME"), json.Value<string>("EMAIL"), json.Value<string>("TELEPHONE"), json.Value<string>("MOBILEPHONE"), json.Value<string>("ENABLED"), json.Value<string>("SEX"), json_user.GetValue("ID"), json.Value<string>("REMARK"), json_user.GetValue("CustomerId"));
            }
            else
            {
                sql = @"update sys_user set REALNAME = '{0}',EMAIL = '{1}',TELEPHONE = '{2}',MOBILEPHONE = '{3}',ENABLED = '{4}',SEX = '{5}',REMARK = '{6}' where id = '{7}'";
                sql = string.Format(sql, json.Value<string>("REALNAME"), json.Value<string>("EMAIL"), json.Value<string>("TELEPHONE"), json.Value<string>("MOBILEPHONE"), json.Value<string>("ENABLED"), json.Value<string>("SEX"), json.Value<string>("REMARK"), Request["ID"]);
            }
            DBMgr.ExecuteNonQuery(sql);
            return "{result:true}";
        }
        public string Delete()
        {
            string sql = "delete from sys_user where id='" + Request["ID"] + "'";
            try
            {
                DBMgr.ExecuteNonQuery(sql);
                return "{result:true}";
            }
            catch (Exception ex)
            {
                return "{result:false}";
            }
        }

        //初始化密码
        public string InitialPsd()
        {
            string sql = "update sys_user set password='" + Request["NAME"].ToSHA1() + "' where id='" + Request["ID"] + "'";
            try
            {
                DBMgr.ExecuteNonQuery(sql);
                return "{result:true}";
            }
            catch (Exception ex)
            {
                return "{result:false}";
            }
        }

        public string selectModel()
        {
            string userid = Request["userid"];
            string sql = "";
            string json = "";
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            //RedisClient redisClient = new RedisClient(AppUtil.RedisIp, Int32.Parse(AppUtil.RedisPort));//redis服务IP和端口
            //if (redisClient.Exists("Authority_" + userid) == 1)//判断REDIS是否存在某一个KEY值，如果存在返回1 不存在返回0
            //{
            //    json = redisClient.Get<string>("Authority_" + userid);
            //}
            //// redisClient.Set("WebSite_" + userid, result);
            //else
            //{
                
                if (!string.IsNullOrEmpty(userid))//当选择了人员后，显示该人员的权限
                {
                    sql = @"select a.MODULEID,a.NAME,a.ISLEAF,a.URL,a.PARENTID,a.SORTINDEX,
                                                          (select  b.ModuleId  from SYS_MODULEUSER b where a.MODULEID=b.MODULEID AND B.USERID='{0}' and rownum=1) as CHECKED from sysmodule a                             
                                                          where PARENTID ='{1}' order by SORTINDEX";
                    sql = string.Format(sql, userid, Request["id"]);
                    DataTable ents = DBMgr.GetDataTable(sql);

                    DataTable moduleIdDt;

                    string moduleIdSql = "select MODULEID from SYS_MODULEUSER where userid = " + json_user.GetValue("ID");
                    moduleIdDt = DBMgr.GetDataTable(moduleIdSql);

                    Sysmodule obj = new Sysmodule();
                    obj.id = "91a0657f-1939-4528-80aa-91b202a593ab";
                    obj = GetTree(obj, moduleIdDt);
                    json = JsonConvert.SerializeObject(obj.children, Formatting.None);
                    json = json.Replace("check", "checked");

                    
                }
           // }
            return json;
        }


        private Sysmodule GetTree(Sysmodule obj, DataTable moduleIdDt)
        {
            try
            {
                string strSQL = @"select a.MODULEID as id,a.NAME,a.ISLEAF,a.URL,a.PARENTID,a.SORTINDEX,
                                      (select  b.ModuleId  from SYS_MODULEUSER b where a.MODULEID=b.MODULEID AND B.USERID='" + Request["userid"] + "' and rownum=1) as CHECKED from sysmodule a  " +
                                      "where PARENTID ='" + obj.id + "' order by SORTINDEX";
                DataTable dtNext = DBMgr.GetDataTable(strSQL);
                obj.children = new List<Sysmodule>();

                foreach (DataRow dr in dtNext.Rows)
                {
                    //过滤掉主账号没有的ModuleId
                    int count = 0;
                    foreach (DataRow mdr in moduleIdDt.Rows)
                    {
                        if (mdr["MODULEID"].ToString() == dr["id"].ToString())
                        {
                            count += 1;
                        }
                    }
                    if (count == 0)
                    {
                        continue;
                    }

                    Sysmodule st = new Sysmodule();
                    st.id = dr["id"].ToString();
                    st.name = dr["name"].ToString();
                    st.ParentID = dr["PARENTID"].ToString();
                    st.check = string.IsNullOrEmpty(dr["CHECKED"].ToString()) ? false : true;
                    // 递归调用
                    st = this.GetTree(st, moduleIdDt);
                    st.leaf = dr["ISLEAF"] + "";
                    obj.children.Add(st);
                }
                return obj;
            }
            catch (Exception e)
            {
                //log.Error(e.Message + e.StackTrace);
                return obj;
            }
        }


        public string AuthorizationSave()
        {
            string moduleids = Request["moduleids"];
            string userid = Request["userid"];
            string sql = @"DELETE FROM SYS_MODULEUSER WHERE USERID = '{0}'";
            sql = string.Format(sql, userid);
            DBMgr.ExecuteNonQuery(sql);
            string[] ids = moduleids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string moduleid in ids)
            {
                sql = @"insert into SYS_MODULEUSER (USERID,MODULEID) values ('{0}','{1}')";
                sql = string.Format(sql, userid, moduleid);
                DBMgr.ExecuteNonQuery(sql);
            }
            return "{success:true}";
        }

        public string Upload(int? chunk, string name)
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            var fileUpload = Request.Files[0];
            var uploadPath = Server.MapPath("/FileUpload/file");
            chunk = chunk ?? 0;
            string new_name = json_user.GetValue("ID") + "_" + name;
            using (var fs = new FileStream(Path.Combine(uploadPath, new_name), chunk == 0 ? FileMode.Create : FileMode.Append))
            {
                var buffer = new byte[fileUpload.InputStream.Length];
                fileUpload.InputStream.Read(buffer, 0, buffer.Length);
                fs.Write(buffer, 0, buffer.Length);
            }
            string sql = "update sys_user set ImgPath='" + "/FileUpload/file/" + new_name + "' where id='" + json_user.GetValue("ID") + "'";
            DBMgr.ExecuteNonQuery(sql);
            return "/FileUpload/file/" + new_name;
        }

        //更新当前账号所属客户的默认供应商
        public string UpdateSupplier()
        {
            string data = Request["data"];
            JObject json = (JObject)JsonConvert.DeserializeObject(data);
            string sql = @"update sys_customer set SCENEDECLAREID='{0}',SCENEINSPECTID='{1}' where id='{2}'";
            sql = string.Format(sql, json.Value<string>("SCENEDECLAREID"), json.Value<string>("SCENEINSPECTID"), json.Value<string>("CUSTOMERID"));
            int result = DBMgr.ExecuteNonQuery(sql);
            return "{result:'" + result + "'}";
        }



    }
}
