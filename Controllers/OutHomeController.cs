using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPlatform.Common;
using Newtonsoft.Json.Linq;

namespace MvcPlatform.Controllers
{
    public class OutHomeController : Controller
    {
        //
        // GET: /OutHome/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View(); 
        }
        public string CurrentUser()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            if (json_user!=null)
            {
            return json_user.GetValue("REALNAME") + "";
            }
            return "sginOut";
        }


    }
}
