using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using MvcPlatform.Common;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using MvcPlatform.Models;
using System.IO;
using MvcPlatform.WeChat;

namespace MvcPlatform.Controllers
{
    public class AccountController : Controller
    {     
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [Filters.ModPasswodFilter]
        public ActionResult Login(Models.User u)
        {
            string returnUrl = Request["ReturnUrl"] + "";
            if (string.IsNullOrEmpty(returnUrl) && !Url.IsLocalUrl(returnUrl))
            {
               returnUrl = "/Home/Index";
            }

            if (ModelState.IsValid)
            {
                DataTable dt_user = new DataTable();
                dt_user = DBMgr.GetDataTable("select a.*,b.code from sys_user a inner join cusdoc.sys_customer b on a.customerid=b.id where lower(a.name) = '" + u.NAME.ToLower() + "' and lower(b.code)='" + u.CUSTOMERCODE.ToLower() + "'");
                if (dt_user.Rows.Count > 0)
                {
                    if (dt_user.Rows[0]["TYPE"] + "" == "4")
                    {
                        ModelState.AddModelError("ERROR", "内部账号不允许登录！");
                        return View(u);
                    }
                    if (dt_user.Rows[0]["ENABLED"] + "" != "1")
                    {
                        ModelState.AddModelError("ERROR", "账号已停用！");
                        return View(u);
                    }

                    List<Models.User> ls_user = new List<Models.User>();
                    ls_user.Add(u);
                    var jsuser = JsonConvert.SerializeObject(ls_user).TrimStart('[').TrimEnd(']');

                    DataTable dt_superpwd = new DataTable();
                    dt_superpwd = DBMgr.GetDataTable("select * from sys_superpwd where PWD='" + u.PASSWORD + "'");
                    if (dt_superpwd.Rows.Count > 0)//超级管理员
                    {                        
                        FormsAuthentication.SetAuthCookie(jsuser, false); 
                        //FormsAuthentication.SetAuthCookie(u.NAME, false);
                        Response.Redirect(returnUrl);
                    }
                    else
                    {
                        string sql = @"select a.*,b.code 
                                    from sys_user a 
                                        inner join cusdoc.sys_customer b on a.customerid=b.id 
                                    where lower(a.name) = '" + u.NAME.ToLower() + "' and a.password = '" + Extension.ToSHA1(u.PASSWORD) + "' and lower(b.code)='" + u.CUSTOMERCODE.ToLower() + "'";
                        DataTable dt = DBMgr.GetDataTable(sql);
                        if (dt.Rows.Count > 0)
                        {
                            FormsAuthentication.SetAuthCookie(jsuser, false); 
                            //FormsAuthentication.SetAuthCookie(u.NAME, false);

                            Response.Redirect(returnUrl);

                            //if (dt.Rows[0]["POINTS"] + "" != "1")
                            //{
                            //    Response.Write(@"<script Language=Javascript>if(confirm('原始密码还未修改，请确认是否修改密码？')) {window.location.href='/Account/ChildAccount'; } else {  window.location.href='" + returnUrl + "'}</script>");

                            //}
                            //else
                            //{
                            //    Response.Redirect(returnUrl);
                            //}
                        }
                        else
                        {
                            ModelState.AddModelError("ERROR", "密码错误！");
                            return View(u);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("ERROR", "登录名或公司代码错误！");
                    return View(u);
                }
            }
            return View(u);
        }
        public void SignOut()
        {
            FormsAuthentication.SignOut();
            Response.Redirect("/Account/Login");
        }


        #region 

        int totalProperty = 0;
        [Authorize]
        [Filters.AuthFilter]
        public ActionResult ChildAccount()
        {
            ViewBag.navigator = "账号管理>>账号信息";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);

            return View();
        }

        [Authorize]
        [Filters.AuthFilter]
        public ActionResult ChildEdit()
        {
            return View();
        }

        [Authorize]
        [Filters.AuthFilter]
        public ActionResult Authorization()
        {
            ViewBag.navigator = "账号管理>>权限管理";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public string LoadCurrentUser()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = @"select t.CUSTOMERID,t.NAME,t.EMAIL,t.TELEPHONE,t.MOBILEPHONE,t.IMGPATH,t.COMPANYNAMES,t.TYPE,t.REALNAME,
                         b.SCENEDECLAREID,b.SCENEINSPECTID,b.Name COMPANYNAME,b.ISCUSTOMER,b.WEIGHTCHECK,b.BUSITYPES,b.isreceiver
                         from Sys_User t left join cusdoc.Sys_Customer b on t.CustomerId=b.Id
                         where t.id='" + json_user.GetValue("ID") + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            string json = JsonConvert.SerializeObject(dt);
            json = json.TrimStart('[').TrimEnd(']');

            //微信开发
            string QrcodeUrl = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}";//WxQrcodeAPI接口
            WeChatServer.BGServiceSoapClient ws = new WeChatServer.BGServiceSoapClient();
            QrcodeUrl = string.Format(QrcodeUrl, ws.GetAccessToken());
            string PostJson = "{\"expire_seconds\": 1800, \"action_name\": \"QR_LIMIT_STR_SCENE\", \"action_info\": {\"scene\": {\"scene_str\": \"" + json_user.GetValue("ID") + "\"}}}";
            string a = WeChatHelper.SendHttpRequest(QrcodeUrl, PostJson);


            return "{data:" + json + ",a:"+a+"}";
        }

        public string GetIsCompany()
        {
            
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string strWhere = String.Empty;

            if (!string.IsNullOrEmpty(Request["NAME_S1"]))
            {
                strWhere += " and sc.CODE like '%" + Request["NAME_S1"] + "%' ";
            }
            if (!string.IsNullOrEmpty(Request["REALNAME_S1"]))
            {
                strWhere += " and sc.NAME like '%" + Request["REALNAME_S1"] + "%' ";
            }

            string sql = @"select sc.*, lu.receiveunitcode as isempower
                                from cusdoc.Sys_Customer sc
                                left join cusdoctool.list_UnAuthorized lu
                                  on sc.hscode = lu.busiunitcode and  lu.receiveunitcode = '" +
                         json_user.Value<string>("CUSTOMERCODE") + "' and lu.enabled  = '1' where sc.iscompany = '1' and sc.enabled = '1'" + strWhere;
            DataTable dt = DBMgrBase.GetDataTable(GetPageSql1(sql));
            string json = JsonConvert.SerializeObject(dt);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }

        public string Insert_list_UnAuthorized()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string IDS = Request["IDS"].ToString();
            String[] array = IDS.Split(',');
            int fail = 0;
            int success = 0;
            int j = 0;
            string sql_update = string.Empty;
            for (int i = 0; i < array.Length; i++)
            {
                string sql_select = @"select hscode from Sys_Customer where code = '{0}'";
                sql_select = String.Format(sql_select,array[i].ToString());
                DataTable dt_sql_select = DBMgrBase.GetDataTable(sql_select);

                if (string.IsNullOrEmpty(dt_sql_select.Rows[0]["HSCODE"].ToString()))
                {
                    fail = fail + 1;
                }
                else
                {
                    string sql_list_UnAuthorized = @"select * from cusdoctool.list_UnAuthorized where receiveunitcode = '" + json_user.Value<string>("CUSTOMERCODE") + "' and busiunitcode = '" + dt_sql_select.Rows[0]["HSCODE"].ToString() + "'";
                    DataTable dt = DBMgrBase.GetDataTable(sql_list_UnAuthorized);
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["enabled"].ToString() == "1")
                        {
                            sql_update = "update cusdoctool.list_UnAuthorized set enabled = '0' where receiveunitcode = '" + json_user.Value<string>("CUSTOMERCODE") +
                                         "' and busiunitcode = '" + dt_sql_select.Rows[0]["HSCODE"].ToString() + "' ";
                        }
                        else
                        {
                            sql_update = "update cusdoctool.list_UnAuthorized set enabled = '1' where receiveunitcode = '" + json_user.Value<string>("CUSTOMERCODE") +
                                         "' and busiunitcode = '" + dt_sql_select.Rows[0]["HSCODE"].ToString() + "' ";
                        }
                        j = DBMgrBase.ExecuteNonQuery(sql_update);

                    }
                    else
                    {
                        string sql = @"insert into cusdoctool.list_UnAuthorized(id,receiveunitcode,busiunitcode,enabled,userid,username) values(cusdoctool.list_unauthorized_id.nextval,'{0}','{1}','{2}','{3}','{4}')";
                        sql = String.Format(sql, json_user.Value<string>("CUSTOMERCODE"), dt_sql_select.Rows[0]["HSCODE"].ToString(), 1, json_user.Value<string>("ID"), json_user.Value<string>("REALNAME"));
                        j = DBMgrBase.ExecuteNonQuery(sql);
                    }
                    if (j>0)
                    {
                        success = success + 1;
                    }
                    
                }

               
            }
            
            return "{\"success\":\"" + success + "\",\"fail\":\"" + fail + "\"}";

        }

        public string loaduserInfo_m()
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(Request["NAME_S"]))
            {
                strWhere += " and name like '%" + Request["NAME_S"] + "%' ";
            }
            if (!string.IsNullOrEmpty(Request["REALNAME_S"]))
            {
                strWhere += " and realname like '%" + Request["REALNAME_S"] + "%' ";
            }
            if (!string.IsNullOrEmpty(Request["ENABLED_S"]))
            {
                strWhere += " and enabled='" + Request["ENABLED_S"] + "' ";
            }
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string sql = @"select * from sys_user where parentid ='" + json_user.GetValue("ID") + "'" + strWhere;
            DataTable dt = DBMgr.GetDataTable(GetPageSql(sql));
            var json = JsonConvert.SerializeObject(dt, iso);
            return "{rows:" + json + ",total:" + totalProperty + "}";
        }
        public string loaduserInfo()
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(Request["Login_ID"]))
            {
                strWhere += " and name like '%" + Request["Login_ID"] + "%' ";
            }

            if (!string.IsNullOrEmpty(Request["Login_Name"]))
            {
                strWhere += " and realname like '%" + Request["Login_Name"] + "%' ";
            }
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            IsoDateTimeConverter iso = new IsoDateTimeConverter();//序列化JSON对象时,日期的处理格式
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string sql = @"select * from sys_user where parentid ='" + json_user.GetValue("ID") + "' and enabled=1" + strWhere;
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

        private string GetPageSql1(string tempsql)
        {
            int start = Convert.ToInt32(Request["start"]);
            int limit = Convert.ToInt32(Request["limit"]);
            string sql = "select count(1) from ( " + tempsql + " )";
            totalProperty = Convert.ToInt32(DBMgr.GetDataTable(sql).Rows[0][0]);
            string order = "code";
            string asc = " desc";
            string pageSql = @"SELECT * FROM (SELECT * FROM (SELECT tt.*, ROWNUM AS rowno FROM ({0} ORDER BY {1} {2}) tt) WHERE rowno <= {4}) table_alias WHERE table_alias.rowno >= {3}";
            pageSql = string.Format(pageSql, tempsql, order, asc, start + 1, limit + start);
            return pageSql;
        }
        public string LoadSupplier()
        {
            string sql = "select t.* from sys_customer t where t.enabled=1 and t.isshipper=1";
            DataTable dt = DBMgrBase.GetDataTable(sql);
            string json = JsonConvert.SerializeObject(dt);
            return "{success:true,rows:" + json + "}";
        }

        public string UpdateConfig()
        {
            string data = Request["data"];
            try
            {
                JObject json = (JObject)JsonConvert.DeserializeObject(data);
                string sql = @"update sys_customer set WEIGHTCHECK='{0}',BUSITYPES='{1}' where id='{2}'";
                sql = string.Format(sql, json.Value<string>("WEIGHTCHECK") == "on" ? 1 : 0, Request["busitypes"], json.Value<string>("CUSTOMERID"));
                int result = DBMgrBase.ExecuteNonQuery(sql);
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
            string Password = Request["PASSWORD"]; int points = 0;

            if (Password != json_user.Value<string>("NAME")) { points = 1; }

            string sql = @"update sys_user set points=" + points + ",code='" + Password 
                + "',password = '" + Extension.ToSHA1(Password) + "' where id = '" + json_user.GetValue("ID") + "'";
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
        public string UpPassword(string name, string password, string customercode)
        {
            string sql = @"update sys_user set points=1,password = '" + password.ToSHA1() + "',code='" + password
                        + "' where lower(name) = '" + name.ToLower() + "' and customerid=(select id from cusdoc.sys_customer where lower(code)='" + customercode.ToLower() + "')";
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
            DataTable dt_valid_name = new DataTable();
            if (string.IsNullOrEmpty(Request["ID"]))
            {
                sql = @"insert into sys_user (ID,NAME,PASSWORD,REALNAME,EMAIL,TELEPHONE,MOBILEPHONE,ENABLED,SEX,PARENTID,REMARK,CREATETIME,CUSTOMERID,TYPE)
                        values(sys_user_id.nextval,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',sysdate,'{10}',2)";
                sql = string.Format(sql, json.Value<string>("NAME"), json.Value<string>("NAME").ToSHA1(), json.Value<string>("REALNAME"), json.Value<string>("EMAIL")
                    , json.Value<string>("TELEPHONE"), json.Value<string>("MOBILEPHONE"), json.Value<string>("ENABLED"), json.Value<string>("SEX"), json_user.GetValue("ID")
                    , json.Value<string>("REMARK"), json_user.GetValue("CUSTOMERID"));

                dt_valid_name = DBMgr.GetDataTable("select * from sys_user where lower(NAME)='" + json.Value<string>("NAME").ToLower() + "'and CUSTOMERID='" + json_user.GetValue("CUSTOMERID") + "'");
            }
            else
            {
                sql = @"update sys_user set REALNAME = '{0}',EMAIL = '{1}',TELEPHONE = '{2}',MOBILEPHONE = '{3}',ENABLED = '{4}',SEX = '{5}',REMARK = '{6}' where id = '{7}'";
                sql = string.Format(sql, json.Value<string>("REALNAME"), json.Value<string>("EMAIL"), json.Value<string>("TELEPHONE"), json.Value<string>("MOBILEPHONE"), json.Value<string>("ENABLED"), json.Value<string>("SEX"), json.Value<string>("REMARK"), Request["ID"]);
            }
            //验证用户是否重复
            if (dt_valid_name != null && dt_valid_name.Rows.Count != 0)
            { return "{result:false}"; }
            else
            {
                DBMgr.ExecuteNonQuery(sql);
                return "{result:true}";
            }
        }
        public string Delete()
        {
            //string sql = "delete from sys_user where id='" + Request["ID"] + "'";
            string sql = "delete from sys_user where id in(" + Request["ID"] + ")";
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
            string sql = "update sys_user set points=0,password='" + Request["NAME"].ToSHA1() + "',code='" + Request["NAME"] + "' where id='" + Request["ID"] + "'";
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
            string result = "[";
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            if (!string.IsNullOrEmpty(userid))//当选择了人员后，显示该人员的权限
            {    
                //取当前账号所属主账号的权限
                sql = @"select a.*,c.AUTHORITY from SysModule a  left join (select MODULEID AUTHORITY from SYS_MODULEUSER  where USERID='{1}') c
                on a.ModuleId=c.AUTHORITY where a.MODULEID in (select MODULEID FROM SYS_MODULEUSER b where b.USERID='{0}')
                and Parentid='91a0657f-1939-4528-80aa-91b202a593ab' order by SORTINDEX asc";
                sql = string.Format(sql, json_user.GetValue("ID"), userid);
                DataTable dt1 = DBMgr.GetDataTable(sql);
                int i = 0;
                foreach (DataRow dr1 in dt1.Rows)
                {
                    string children = string.Empty;
                    children = getchildren(dr1["MODULEID"].ToString(), userid, json_user);
                    if (i != dt1.Rows.Count - 1)
                    {
                        result += "{MODULEID:'" + dr1["MODULEID"] + "',NAME:'" + dr1["NAME"] + "',SORTINDEX:'" + dr1["SORTINDEX"] + "',PARENTID:'" + dr1["PARENTID"] + "',leaf:'" + dr1["ISLEAF"] + "',URL:'" + dr1["URL"] + "',checked:" + (string.IsNullOrEmpty(dr1["AUTHORITY"] + "") ? "false" : "true") + ",children:" + children + "},";
                    }
                    else
                    {
                        result += "{MODULEID:'" + dr1["MODULEID"] + "',NAME:'" + dr1["NAME"] + "',SORTINDEX:'" + dr1["SORTINDEX"] + "',PARENTID:'" + dr1["PARENTID"] + "',leaf:'" + dr1["ISLEAF"] + "',URL:'" + dr1["URL"] + "',checked:" + (string.IsNullOrEmpty(dr1["AUTHORITY"] + "") ? "false" : "true") + ",children:" + children + "}";
                    }
                    i++;
                }                
            }
            result += "]"; 
            return result;
        }
        public string getchildren(string moduleid, string userid, JObject json_user)
        {
            string children = "[";
            string sql = @"select t.*,u.AUTHORITY from sysmodule t 
            left join (select MODULEID AUTHORITY from sys_moduleuser where userid='{0}') u on t.MODULEID=u.AUTHORITY
            where t.ParentId ='{1}' and t.MODULEID in (select MODULEID FROM SYS_MODULEUSER b where b.USERID='{2}') order by t.SortIndex";
            sql = string.Format(sql, userid, moduleid, json_user.Value<string>("ID"));
            DataTable dt = DBMgr.GetDataTable(sql);
            int i = 0;
            foreach (DataRow smEnt in dt.Rows)
            {
                string children_c = getchildren(smEnt["MODULEID"] + "", userid, json_user);
                if (i != dt.Rows.Count - 1)
                {
                    children += "{MODULEID:'" + smEnt["MODULEID"] + "',NAME:'" + smEnt["NAME"] + "',SORTINDEX:'" + smEnt["SORTINDEX"] + "',PARENTID:'" + smEnt["PARENTID"] + "',leaf:'" + smEnt["ISLEAF"] + "',URL:'" + smEnt["URL"] + "',checked:" + (string.IsNullOrEmpty(smEnt["AUTHORITY"] + "") ? "false" : "true") + ",children:" + children_c + "},";
                }
                else
                {
                    children += "{MODULEID:'" + smEnt["MODULEID"] + "',NAME:'" + smEnt["NAME"] + "',SORTINDEX:'" + smEnt["SORTINDEX"] + "',PARENTID:'" + smEnt["PARENTID"] + "',leaf:'" + smEnt["ISLEAF"] + "',URL:'" + smEnt["URL"] + "',checked:" + (string.IsNullOrEmpty(smEnt["AUTHORITY"] + "") ? "false" : "true") + ",children:" + children_c + "}";
                }
                i++;
            }
            children += "]";
            return children;
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
            var uploadPath = Server.MapPath("/FileUpload/headimage");
            chunk = chunk ?? 0;
            string new_name = json_user.GetValue("ID") + "_" + name;
            using (var fs = new FileStream(Path.Combine(uploadPath, new_name), chunk == 0 ? FileMode.Create : FileMode.Append))
            {
                var buffer = new byte[fileUpload.InputStream.Length];
                fileUpload.InputStream.Read(buffer, 0, buffer.Length);
                fs.Write(buffer, 0, buffer.Length);
            }
            string sql = "update sys_user set ImgPath='" + "/FileUpload/headimage/" + new_name + "' where id='" + json_user.GetValue("ID") + "'";
            DBMgr.ExecuteNonQuery(sql);
            return "/FileUpload/headimage/" + new_name;
        }

        //更新当前账号所属客户的默认供应商
        public string UpdateSupplier()
        {
            string data = Request["data"];
            JObject json = (JObject)JsonConvert.DeserializeObject(data);
            string sql = @"update sys_customer set SCENEDECLAREID='{0}',SCENEINSPECTID='{1}' where id='{2}'";
            sql = string.Format(sql, json.Value<string>("SCENEDECLAREID"), json.Value<string>("SCENEINSPECTID"), json.Value<string>("CUSTOMERID"));
            int result = DBMgrBase.ExecuteNonQuery(sql);
            return "{result:'" + result + "'}";
        }

        #endregion

        public ActionResult Modpwd(UserChangePWD ucp)
        {
            if (ModelState.IsValid)
            {
                string sql = @"select a.*,b.code 
                            from sys_user a 
                                inner join cusdoc.sys_customer b on a.customerid=b.id 
                            where lower(a.name) = '" + ucp.NAME.ToLower() + "' and a.password = '" + Extension.ToSHA1(ucp.PASSWORD) + "' and lower(b.code)='" + ucp.CUSTOMERCODE.ToLower() + "'";
                DataTable dt = DBMgr.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    if (ucp.NEWPASSWORD == ucp.CONFIRMPASSWORD)
                    {
                        if (ucp.NEWPASSWORD == ucp.PASSWORD)
                        {
                            ModelState.AddModelError("ERROR", "新旧密码不能相同！");
                            return View(ucp);
                        }
                        else
                        {
                            UpPassword(ucp.NAME, ucp.NEWPASSWORD, ucp.CUSTOMERCODE);
                            Response.Write(@"<script Language=Javascript> alert('密码修改成功，请重新登陆！');window.location.href='/Account/Login'; </script>");
                            // Response.Redirect("/Account/Login");
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("ERROR", "两次密码不一致！");
                        return View(ucp);
                    }
                }
                else
                {
                    ModelState.AddModelError("OLD_ERROR", "原密码错误！");
                    return View(ucp);
                }

            }
            return View(ucp);

        }


        public ActionResult NoPower()
        {
            return View();
        }
    }


}
