using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace MvcPlatform.Controllers
{

    public class BaseController : Controller
    {
        //
        // GET: /Base/



        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {

            //FormsAuthenticationTicket r = new FormsAuthenticationTicket(;
            string userdata=((FormsIdentity)HttpContext.User.Identity).Ticket.UserData;
           HttpRequestBase hrb = filterContext.HttpContext.Request;
           //Uri url=hrb.;
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            base.OnResultExecuting(filterContext);
        }

    }
}
    