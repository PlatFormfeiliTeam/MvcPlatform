using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MvcPlatform
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
          //  BundleConfig.RegisterBundles(BundleTable.Bundles);
          //  AuthConfig.RegisterAuth();
        }


        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception objErr = Server.GetLastError().GetBaseException();
            
            string errortime = "发生时间:" + System.DateTime.Now.ToString();
            string erroraddr = "发生异常页: " + Request.Url.ToString();
            string errorinfo = "异常信息: " + objErr.Message;
            string errorsource = "错误源:" + objErr.Source;
            string errortrace = "堆栈信息:" + objErr.StackTrace;

            string error = errortime + "<br>" + erroraddr + "<br>" + errorinfo + "<br>" + errorsource + "<br>" + errortrace + "<br>" + "--------------------------------------<br>";
            Server.ClearError();
            //Application["error"] = error;

           
            //独占方式，因为文件只能由一个进程写入.
            System.IO.StreamWriter writer = null;
            try
            {
                lock (this)
                {
                    // 写入日志
                    string year = DateTime.Now.Year.ToString();
                    string month = DateTime.Now.Month.ToString();
                    string path = Server.MapPath("~/ErrorLog/") + year + "/" + month;

                    if (!System.IO.Directory.Exists(path)) //如果目录不存在则创建
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }

                    string filename = DateTime.Now.Day.ToString() + ".txt";
                    System.IO.FileInfo file = new System.IO.FileInfo(path + "/" + filename);
                    writer = new System.IO.StreamWriter(file.FullName, true);

                    string ip = "请求地址:" + Request.Url.PathAndQuery;
                    string log = "----------------------------------------------------------------------------------------------------------\r\n"
                        + errortime + "\r\n" + erroraddr + "\r\n" + ip + "\r\n" + errorinfo + "\r\n" + errorsource + "\r\n" + errortrace + "\r\n";
                    writer.WriteLine(log);
                }
            }
            finally
            {
                if (writer != null)
                    writer.Close();

            } 


            Response.Redirect("~/Error/ErrorPage");
        }


    }
}