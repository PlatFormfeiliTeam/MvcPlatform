using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcPlatform.Controllers
{

    public class BaseController : Controller
    {
        //
        // GET: /Base/



        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            RouteBase rb = filterContext.RouteData.Route;
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            base.OnResultExecuting(filterContext);
        }

    }
}
