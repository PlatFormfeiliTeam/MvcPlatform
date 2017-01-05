using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    public class FrequentInforController : Controller
    {
        //
        // GET: /FrequentInfor/

        public ActionResult FrequentCode()
        {
            ViewBag.navigator = "常用信息 > 常用代码";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

    }
}
