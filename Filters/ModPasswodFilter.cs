using MvcPlatform.Common;
using MvcPlatform.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Filters
{
    public class ModPasswodFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            User u = (User)filterContext.ActionParameters["u"];
            UserChangePWD ucp = new UserChangePWD();
            ucp.NAME = u.NAME;
            ucp.PASSWORD = u.PASSWORD;
            bool valid = filterContext.Controller.ValidateRequest;

            if (valid)
            {
                DataTable dt_user = new DataTable();
                dt_user = DBMgr.GetDataTable("select * from sys_user where name = '" + u.NAME + "'");
                if (dt_user.Rows.Count > 0)
                {

                    if (dt_user.Rows[0]["TYPE"] + "" != "4" && dt_user.Rows[0]["ENABLED"] + "" == "1")
                    {

                        DataTable dt_superpwd = new DataTable();
                        dt_superpwd = DBMgr.GetDataTable("select * from sys_superpwd where PWD='" + u.PASSWORD + "'");
                        if (dt_superpwd.Rows.Count <= 0)//超级管理员
                        {
                            if (dt_user.Rows[0]["POINTS"] + "" != "1")
                            {
                              

                                //filterContext.Result = new RedirectResult("/Home/Modpwd");
                                //ViewEngineCollection vec = new ViewEngineCollection();
                                //RazorViewEngine razorViewEngine=new RazorViewEngine();
                                //razorViewEngine.ViewLocationFormats = new[] { "~/Views/Home/Modpwd.cshtml" };
                                //vec.Add(razorViewEngine);
                                filterContext.Result = new ViewResult
                                {
                                    ViewName = "Modpwd",
                                    ViewData = new ViewDataDictionary<UserChangePWD>(ucp)
                                   // ViewEngineCollection = vec
                                };
                                
                                
                            }

                            if (ucp.PASSWORD != null)
                            {
                                string sql = "select * from sys_user where name = '" + u.NAME + "' and password = '" + Extension.ToSHA1(u.PASSWORD) + "'";
                                DataTable dt = DBMgr.GetDataTable(sql);
                                if (dt.Rows.Count <= 0)
                                {
                                    ucp.PASSWORD = string.Empty;

                                } 
                            }
                            
                           
                        }
                    }
                }

            }

        }

    }
}