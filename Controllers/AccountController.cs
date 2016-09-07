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
        public ActionResult DoLogin(Models.User u)
        {
            //string username = Request["username"];
            //string password = Extension.ToSHA1(Request["password"]);
            string sql = "select * from sys_user where name = '" + u.NAME + "' and password = '" + Extension.ToSHA1(u.PASSWORD) + "'";
            DataTable dt = DBMgr.GetDataTable(sql);
            string msg = "";
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["TYPE"] + "" == "4")
                {
                    msg = "内部账号不允许登录!";
                }
                if (dt.Rows[0]["ENABLED"] + "" != "1")
                {
                    msg = "账号已停用!";
                }
                if (string.IsNullOrEmpty(msg))
                {
                    FormsAuthentication.SetAuthCookie(u.NAME, false);
                    Response.Redirect("/Home/Index");
                    // FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(username, true, 300);
                    //  FormsAuthentication.RedirectFromLoginPage(username, false);
                    //Session["user"] = username;
                    //msg = "success";
                }
            }
            else
            {
                msg = "账号/密码错误!";
            }
            return View();
        }
        public void SignOut()
        {
            FormsAuthentication.SignOut();
            Response.Redirect("/Account/Login");
        }
    }
}
