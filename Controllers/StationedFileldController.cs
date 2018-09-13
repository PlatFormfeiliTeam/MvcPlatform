using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    public class StationedFileldController : Controller
    {
        //
        // GET: /StationedFileld/

        public ActionResult EntryStationFileld()
        {
            ViewBag.navigator = "现场服务>>驻场管理";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

    }
}
