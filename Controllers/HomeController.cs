using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    public class HomeController : Controller
    {
          
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.navigator = "关务云>>首页";
            return View();
        }  
    }
}
