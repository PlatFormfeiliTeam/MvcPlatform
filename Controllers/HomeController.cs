using MvcPlatform.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            //ViewBag.navigator = "关务云>>首页";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public string Header()
        {
            string result = "<li><a href=\"/Home/Index\">首页</a></li>";
            if (string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {

            }
            else
            {
                JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
                string sql = @"select MODULEID,NAME,PARENTID,URL,SORTINDEX,IsLeaf from sysmodule t 
            where t.parentid='91a0657f-1939-4528-80aa-91b202a593ab' and t.MODULEID IN (select MODULEID FROM sys_moduleuser where userid='{0}')
            order by sortindex";
                sql = string.Format(sql, json_user.GetValue("ID"));
                DataTable dt1 = DBMgr.GetDataTable(sql);
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    result += "<li><a>" + dt1.Rows[i]["NAME"] + "</a>";

                    sql = @"select MODULEID,NAME,PARENTID,URL,SORTINDEX,IsLeaf from sysmodule t where t.parentid='{0}'
                and t.MODULEID IN (select MODULEID FROM sys_moduleuser where userid='{1}') order by sortindex";
                    sql = string.Format(sql, dt1.Rows[i]["MODULEID"], json_user.GetValue("ID"));
                    DataTable dt2 = DBMgr.GetDataTable(sql);
                    if (dt2.Rows.Count > 0)
                    {
                        result += "<ul>";
                        for (int j = 0; j < dt2.Rows.Count; j++)
                        {
                            if (string.IsNullOrEmpty(dt2.Rows[j]["URL"] + ""))
                            {
                                result += "<li><a>" + dt2.Rows[j]["NAME"] + "</a>";
                            }
                            else
                            {
                                result += "<li><a href=\"" + dt2.Rows[j]["URL"] + "\">" + dt2.Rows[j]["NAME"] + "</a>";
                            }
                            sql = @"select MODULEID,NAME,PARENTID,URL,SORTINDEX,IsLeaf from sysmodule t where t.parentid='{0}' 
                        and t.MODULEID IN (select MODULEID FROM sys_moduleuser where userid='{1}') order by sortindex";
                            sql = string.Format(sql, dt2.Rows[j]["MODULEID"], json_user.GetValue("ID"));
                            DataTable dt3 = DBMgr.GetDataTable(sql);
                            if (dt3.Rows.Count > 0)
                            {
                                result += "<ul>";
                                for (int k = 0; k < dt3.Rows.Count; k++)
                                {
                                    if (string.IsNullOrEmpty(dt3.Rows[k]["URL"] + ""))
                                    {
                                        result += "<li><a>" + dt3.Rows[k]["NAME"] + "</a></li>";
                                    }
                                    else
                                    {
                                        result += "<li><a href=\"" + dt3.Rows[k]["URL"] + "\">" + dt3.Rows[k]["NAME"] + "</a></li>";
                                    }
                                }
                                result += "</ul></li>";
                            }
                            else
                            {
                                result += "</li>";
                            }
                        }
                        result += "</ul></li>";
                    }
                    else
                    {
                        result += "</li>";
                    }
                }
            }
            return result;
        }
    }
}
