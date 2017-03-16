﻿using MvcPlatform.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPlatform.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            string sql = "";
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();//新建字典
            DataTable dt_type = new DataTable();
            DataTable dt_notice = new DataTable();

            sql = "select distinct type from web_notice where isinvalid=0 order by type";
            dt_type = DBMgr.GetDataTable(sql);

            sql = @"select a.* 
                    from (
                          select row_number() over (partition by type order by type,updatetime desc) numid
                                 ,id,type,title,to_char(updatetime,'yyyy/mm/dd hh24:mi:ss') as updatetime 
                          from web_notice where isinvalid=0 
                          order by type,updatetime desc
                          ) a
                    where numid<=6";
            dt_notice = DBMgr.GetDataTable(sql);           
            
            dic.Add("dt_type", dt_type);
            dic.Add("dt_notice", dt_notice);

            //ViewBag.navigator = "关务云>>首页";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View(dic);
        }

        public ActionResult IndexNotice()
        {
            string sql = "";
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();//新建字典
            DataTable dt_type = new DataTable();
            DataTable dt_notice = new DataTable();

            sql = "select distinct type from web_notice where isinvalid=0 order by type";
            dt_type = DBMgr.GetDataTable(sql);

            sql = @"select a.* 
                    from (
                          select row_number() over (partition by type order by type,updatetime desc) numid
                                 ,id,type,title,to_char(updatetime,'yyyy/mm/dd hh24:mi:ss') as updatetime 
                          from web_notice where isinvalid=0 
                          order by type,updatetime desc
                          ) a
                     where numid<=3";
            dt_notice = DBMgr.GetDataTable(sql);

            dic.Add("dt_type", dt_type);
            dic.Add("dt_notice", dt_notice);
            ViewBag.navigator = "首页>>资讯动态";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View(dic);
        }

        public ActionResult IndexNoticeDetail(string ID)
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();//新建字典
            DataTable dt_notice = new DataTable();
            DataTable dt_notice_pre = new DataTable(); DataTable dt_notice_next = new DataTable();
            string type = "";

            dt_notice = DBMgr.GetDataTable("select type,title,content,attachment,to_char(updatetime,'yyyy/mm/dd hh24:mi:ss') as updatetime from web_notice where isinvalid=0 and id='" + ID + "'");
            dic.Add("dt_notice", dt_notice);
            type = dt_notice.Rows[0]["type"].ToString();

            dt_notice_pre = DBMgr.GetDataTable("select ID,title from web_notice where isinvalid=0 and type='" + type + "' and id<" + Convert.ToInt32(ID) + " and ROWNUM=1 order by id desc");
            dic.Add("dt_notice_pre", dt_notice_pre);

            dt_notice_next = DBMgr.GetDataTable("select ID,title from web_notice where isinvalid=0 and type='" + type + "' and id>" + Convert.ToInt32(ID) + " and ROWNUM=1 order by id");
            dic.Add("dt_notice_next", dt_notice_next);

            ViewBag.navigator = "资讯动态 > 分类：" + type;
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View(dic);
        }

        public string Header()
        {
            string result = "<li><a href=\"/Home/Index\"><i class=\"icon iconfont\">&#xe62e;</i>&nbsp;&nbsp;首页</a></li>";
            if (string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {

            }
            else
            {
                JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
                string sql = @"select MODULEID,NAME,PARENTID,URL,SORTINDEX,IsLeaf,ICON from sysmodule t 
                where t.parentid='91a0657f-1939-4528-80aa-91b202a593ab' and t.MODULEID IN (select MODULEID FROM sys_moduleuser where userid='{0}')
                order by sortindex";
                sql = string.Format(sql, json_user.GetValue("ID"));
                DataTable dt1 = DBMgr.GetDataTable(sql);
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    string icon = string.Empty;
                    if (!string.IsNullOrEmpty(dt1.Rows[i]["ICON"] + ""))
                    {
                        icon = "<i class=\"icon iconfont\">&#x" + dt1.Rows[i]["ICON"] + ";</i>&nbsp;&nbsp;";
                    }
                    result += "<li><a>" + icon + dt1.Rows[i]["NAME"] + "</a>";
                    sql = @"select MODULEID,NAME,PARENTID,URL,SORTINDEX,IsLeaf,ICON from sysmodule t where t.parentid='{0}'
                    and t.MODULEID IN (select MODULEID FROM sys_moduleuser where userid='{1}') order by sortindex";
                    sql = string.Format(sql, dt1.Rows[i]["MODULEID"], json_user.GetValue("ID"));
                    DataTable dt2 = DBMgr.GetDataTable(sql);
                    if (dt2.Rows.Count > 0)
                    {
                        result += "<ul>";
                        for (int j = 0; j < dt2.Rows.Count; j++)
                        {
                            icon = string.Empty;
                            if (!string.IsNullOrEmpty(dt2.Rows[j]["ICON"] + ""))
                            {
                                icon = "<i class=\"icon iconfont\">&#x" + dt2.Rows[j]["ICON"] + ";</i>&nbsp;&nbsp;";
                            }
                            if (string.IsNullOrEmpty(dt2.Rows[j]["URL"] + ""))
                            {
                                result += "<li><a>" + icon + dt2.Rows[j]["NAME"] + "</a>";
                            }
                            else
                            {
                                result += "<li><a href=\"" + icon + dt2.Rows[j]["URL"] + "\">" + dt2.Rows[j]["NAME"] + "</a>";
                            }
                            sql = @"select MODULEID,NAME,PARENTID,URL,SORTINDEX,IsLeaf,ICON from sysmodule t where t.parentid='{0}' 
                            and t.MODULEID IN (select MODULEID FROM sys_moduleuser where userid='{1}') order by sortindex";
                            sql = string.Format(sql, dt2.Rows[j]["MODULEID"], json_user.GetValue("ID"));
                            DataTable dt3 = DBMgr.GetDataTable(sql);
                            if (dt3.Rows.Count > 0)
                            {
                                result += "<ul>";
                                for (int k = 0; k < dt3.Rows.Count; k++)
                                {
                                    icon = string.Empty;
                                    if (!string.IsNullOrEmpty(dt3.Rows[k]["ICON"] + ""))
                                    {
                                        icon = "<i class=\"icon iconfont\">&#x" + dt3.Rows[k]["ICON"] + ";</i>&nbsp;&nbsp;";
                                    }
                                    if (string.IsNullOrEmpty(dt3.Rows[k]["URL"] + ""))
                                    {
                                        result += "<li><a>" + icon + dt3.Rows[k]["NAME"] + "</a></li>";
                                    }
                                    else
                                    {
                                        result += "<li><a href=\"" + dt3.Rows[k]["URL"] + "\">" + icon + dt3.Rows[k]["NAME"] + "</a></li>";
                                    }
                                }
                                result += "</ul></li>";
                            }
                            else
                            {
                                result += "</li>";
                            }
                        }
                        result += "</ul></li>";
                    }
                    else
                    {
                        result += "</li>";
                    }
                }
            }
            return result;
        }
    }
}
