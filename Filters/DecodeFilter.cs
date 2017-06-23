using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;
using MvcPlatform.Common;

namespace MvcPlatform.Filters
{
    public class DecodeFilter : ActionFilterAttribute
    {
      
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
          
            HttpRequestBase req = filterContext.HttpContext.Request;
            string queryString = req.Url.Query.Substring(1);//AbsoluteUri.Substring(req.Url.AbsoluteUri.IndexOf('?') + 1);

            //queryString = HttpUtility.UrlDecode(queryString);
            queryString = DecodeBase.Decrypt(queryString);

             string path=req.FilePath;
             NameValueCollection QS = ParseQueryString(queryString);
             ParameterDescriptor[] pds = filterContext.ActionDescriptor.GetParameters();
             filterContext.ActionParameters.Clear();
            
             foreach (var pd in pds)
             {
               string ParameterValue= QS.Get(pd.ParameterName);
               string typeName = pd.ParameterType.Name;
               
               filterContext.ActionParameters.Add(pd.ParameterName,Convert.ChangeType(ParameterValue,pd.ParameterType));
             }
        }

        public static NameValueCollection ParseQueryString(string url)
        {
            NameValueCollection cPar = new NameValueCollection();
            url = url.StartsWith("?") ? url.Substring(1) : url;
            string[] pairs = url.Split(new char[] { '&' });
            int tmpIndex;
            foreach (string pair in pairs)
            {
                if (string.IsNullOrEmpty(pair))
                    continue;
                tmpIndex = pair.IndexOf('=');
                if (tmpIndex == -1)
                    continue;
                cPar.Add(pair.Substring(0, tmpIndex), pair.Substring(tmpIndex + 1));
            }
            return cPar;
        }



    }
}