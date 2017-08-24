using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult ErrorPage()
        {
            //ViewBag.error = HttpContext.Application["error"];
            return View();
        }

    }
}
