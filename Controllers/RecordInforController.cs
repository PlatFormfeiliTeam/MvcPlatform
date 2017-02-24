using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    [Authorize]
    public class RecordInforController : Controller
    {
        //
        // GET: /RecordInfor/

        public ActionResult Recordinfo_Detail()//备案信息
        {
            ViewBag.navigator = "备案管理>>备案信息";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult Recordinfo_Detail_SUMNUM()//申报数量
        {
            ViewBag.navigator = "备案管理>>申报数量";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View();
        }

        public string Ini_Record_Data()
        {
            JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
            string sql = "";

            string json_recordid = "[]";
            sql = @"select code,code||'('||bookattribute||')' as name from sys_recordinfo where busiunit= '" + json_user.Value<string>("CUSTOMERHSCODE") + "'";
            json_recordid = JsonConvert.SerializeObject(DBMgrBase.GetDataTable(sql));

            return "{recordid:" + json_recordid + "}";
        }



    }
}
