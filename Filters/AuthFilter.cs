using MvcPlatform.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace MvcPlatform.Filters
{
    public class AuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (pubfunc(filterContext))
            {
                Uri url = filterContext.HttpContext.Request.Url;
                string pathandquery = url.AbsolutePath, xmlstr = "", queryString = "";
               
                //先解密
                if (url.Query != "") { 
                    queryString = DecodeBase.Decrypt(url.Query.Substring(1));
                    if (queryString == "") { queryString = url.Query.Substring(1); }//解密是空的话，根本就没加密，没加密就还原
                }

                if (queryString != "") { pathandquery = pathandquery + "?" + queryString; }//带入数据库值
                
                //带入xml值
                if (queryString.Contains("menuxml"))
                {
                    xmlstr = url.AbsolutePath + "?" + queryString.Substring(0, queryString.IndexOf("&"));
                }
                else
                {
                    xmlstr = url.AbsolutePath;
                }

                XDocument doc = XDocument.Load("http://" + url.Authority + "/FileUpload/SubMenuFile.xml");
                string str = "";

                //foreach (XElement e in doc.Root.Descendants("submenu"))//doc.Root.Elements("submenu")
                //{
                //    if (e.Value.Equals(xmlstr))
                //    {
                //        str = e.Parent.Element("menu").Value;
                //        break;
                //    }
                //}


                var text1 = doc.Descendants("rootmenu")
                   .Where(p =>
                   {
                       //if (p.Element("submenu").Value.Equals(xmlstr))
                       //{
                       //    str = p.Element("menu").Value;
                       //    return true;
                       //}
                       //return false;

                       foreach (var item in p.Elements("submenu"))
                       {
                           if (item.Value.Equals(xmlstr))
                           {
                               str = item.Parent.Element("menu").Value; break;
                           }
                       }

                       if (str != "") { return true; }
                       return false;

                   }).ToList();

                if (str != "") { pathandquery = str; }

                JObject json_user = Extension.Get_UserInfo(filterContext.HttpContext.User.Identity.Name);
                string sql = @"select count(1) from sysmodule t where t.MODULEID IN (select MODULEID FROM sys_moduleuser where userid='{0}') and lower(t.url)=lower('{1}')";
                sql = string.Format(sql, json_user.GetValue("ID"), pathandquery);
                DataTable dt = DBMgr.GetDataTable(sql);
                if (dt.Rows[0][0].ToString() == "0")//无权限
                {
                    filterContext.Result = new RedirectResult("/Account/NoPower");
                }

                //----------------------------------------------------
                //20180103临时加上权限：因 委托单位 跟 接单单位 共用一个界面，只是控制新增按钮而已，接单单位可以新增，委托单位不可以
                if (queryString == "" && (xmlstr.ToLower() == "/orderairout/create" || xmlstr.ToLower() == "/orderairin/create" || xmlstr.ToLower() == "/orderlandout/create"
                    || xmlstr.ToLower() == "/orderlandin/create" || xmlstr.ToLower() == "/orderseaout/create" || xmlstr.ToLower() == "/orderseain/create"
                    || xmlstr.ToLower() == "/orderdomestic/create" || xmlstr.ToLower() == "/orderspecial/create"))//代表的是新增界面
                {
                    JObject jsonu = Extension.Get_UserInfo(filterContext.HttpContext.User.Identity.Name);
                    if (json_user.Value<string>("ISRECEIVER") != "1")
                    {
                        filterContext.Result = new RedirectResult("/Account/NoPower");
                    }
                }
                //----------------------------------------------------
            }
        }

        private bool pubfunc(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                return false;
            }
            if (filterContext.ActionDescriptor.ActionName == "CurrentUser" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Common")//当前用户不做验证
            {
                return false;
            }
            if (filterContext.HttpContext.Request.Url.AbsolutePath == "/Home/Index")
            {
                return false;
            }
            if (filterContext.HttpContext.Request.Url.AbsolutePath == "/")
            {
                return false;
            }

            if (filterContext.HttpContext.Request.Url.AbsolutePath == "/Common/UploadFile")
            {
                return false;
            }
            if (filterContext.HttpContext.Request.Url.AbsolutePath == "/OrderFile/UploadFile")
            {
                return false;
            }
            if (filterContext.HttpContext.Request.Url.AbsolutePath == "/OrderDomestic/Upload_WebServer")
            {
                return false;
            }
            if (filterContext.HttpContext.Request.Url.AbsolutePath == "/EnterpriseOrder/Upload_WebServer")
            {
                return false;
            }

            if (filterContext.HttpContext.Request.Url.AbsolutePath == "/RecordInfor/PrintRecordDetail")
            {
                return false;
            }

            if (filterContext.HttpContext.Request.Url.AbsolutePath == "/Common/DownloadFile")
            {
                return false;
            }
            if (filterContext.HttpContext.Request.Url.AbsolutePath == "/EnterpriseOrder/DownFile")
            {
                return false;
            }
            if (filterContext.HttpContext.Request.Url.AbsolutePath == "/OrderFile/DownFile")
            {
                return false;
            }
            if (filterContext.HttpContext.Request.Url.AbsolutePath == "/Home/IndexNoticeDetail")//常用收藏，首页，更多
            {
                return false;
            }
            return true;
        }


    }
}