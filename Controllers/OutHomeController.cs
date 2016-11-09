using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

    }
}
