using MvcPlatform.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Mvc;
using MvcPlatform.Models;
using Webdiyer.WebControls.Mvc;


namespace MvcPlatform.Controllers
{
    public class HomeController : Controller
    {
        int pagenum = 10;//每页数量

        [AllowAnonymous]
        public ActionResult Index()
        {
            string sql = "";
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();//新建字典
            DataTable dt_type = new DataTable(); DataTable dt_notice = new DataTable();
            DataTable dt_banner = new DataTable(); DataTable dt_infor_cate = new DataTable(); DataTable dt_infor = new DataTable();

            sql = "select id,name type from newscategory where pid is null order by sortindex,id";
            dt_type = DBMgr.GetDataTable(sql);

            sql = @"select *                        
                    from (
                      select a.id as type,a.sortindex,row_number() over (partition by a.id order by b.publishdate desc) numid,b.id
                        ,case when length(b.title)>50 then substr(b.title,1,50)||'...' else b.title end title
                        ,to_char(b.publishdate,'yyyy-mm-dd') as publishdate ,b.publishdate as publishdate_sort,to_char(b.updatetime,'yyyy-mm-dd hh24:mi') as updatetime 
                      from (select * from newscategory where pid is null) a
                           left join (
                                  select a.rootid,b.*
                                  from (
                                      select id ,CONNECT_BY_ROOT(ID) rootid
                                      from newscategory START WITH pid is null connect by prior id=pid
                                      ) a
                                      inner join (select * from web_notice where isinvalid=0 ) b on a.id=b.type
                                  order by a.rootid
                                  ) b on a.id=b.rootid
                    ) a
                    where numid<=6
                    order by a.sortindex,a.type,a.publishdate_sort desc";
            dt_notice = DBMgr.GetDataTable(sql);

            sql = @"select ID,IMGURL,LINKURL,DESCRIPTION,STATUS,FILENAME,SORTINDEX from web_banner WHERE STATUS='true' order by SORTINDEX";
            dt_banner = DBMgr.GetDataTable(sql);
            sql = @"select ID,ICON,NAME,DESCRIPTION from list_collect_infor_cate WHERE isinvalid=0 order by SORTINDEX";
            dt_infor_cate = DBMgr.GetDataTable(sql);

            sql = @"select a.NAME TYPENAME,b.*
                    from list_collect_infor_cate a 
                        left join (select * from list_collect_infor where isinvalid=0)b on a.id=b.rid_type 
                    WHERE a.isinvalid=0  order by a.SORTINDEX";
            dt_infor = DBMgr.GetDataTable(sql);

            dic.Add("dt_type", dt_type); dic.Add("dt_notice", dt_notice);
            dic.Add("dt_banner", dt_banner); dic.Add("dt_infor_cate", dt_infor_cate); dic.Add("dt_infor", dt_infor);

            //ViewBag.navigator = "关务云>>首页";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View(dic);
        }

        #region 更多IndexNotice
        public ActionResult IndexNotice()
        {
            string sql = "";
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();//新建字典
            DataTable dt_type = new DataTable();
            DataTable dt_notice = new DataTable();

            sql = "select distinct type,'' totalpage from web_notice where isinvalid=0 order by type";
            dt_type = DBMgr.GetDataTable(sql);
            for (int i = 0; i < dt_type.Rows.Count; i++)
			{
                dt_type.Rows[i]["totalpage"] = GetTotalPage(dt_type.Rows[i]["type"].ToString());
			}

            sql = @"select a.* 
                    from (
                          select row_number() over (partition by type order by type,updatetime desc) numid
                                 ,id,type,title,to_char(updatetime,'yyyy/mm/dd hh24:mi:ss') as updatetime 
                          from web_notice where isinvalid=0 
                          order by type,updatetime desc
                          ) a
                     where numid<={0}";
            dt_notice = DBMgr.GetDataTable(string.Format(sql, pagenum));

            dic.Add("dt_type", dt_type);
            dic.Add("dt_notice", dt_notice);

            ViewBag.navigator = "首页>>资讯动态";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);
            return View(dic);
        }

        public int GetTotalPage(string type)
        {
            int totalpage = 0;
            string sql = @"select max(numid) 
                            from (
                                  select row_number() over (partition by type order by type,updatetime desc) numid
                                         ,id,type,title,to_char(updatetime,'yyyy/mm/dd hh24:mi:ss') as updatetime 
                                  from web_notice where isinvalid=0 and type='{0}'
                                  order by type,updatetime desc
                                  ) a ";
            sql = string.Format(sql, type);
            DataTable dt_id = DBMgr.GetDataTable(sql);
            int maxid = Convert.ToInt32(dt_id.Rows[0][0].ToString());
            if (maxid % pagenum == 0) { totalpage = maxid / pagenum; }
            else { totalpage = (maxid / pagenum) + 1; }
            return totalpage;
        }

        public string GetInfor()
        {
            string cate = Request["cate"]; int curpage = Convert.ToInt32(Request["curpage"]); string type = Request["type"];
            int startnum = 0; int endnum = 0; string sql = ""; 
            int newpage = 0; int totalpage = 0;

            totalpage = GetTotalPage(type);

            if (cate == "pre") { startnum = (curpage - 2) * pagenum; endnum = (curpage - 1) * pagenum; newpage = (Convert.ToInt32(curpage) - 1); }
            if (cate == "next") { startnum = curpage * pagenum; endnum = (curpage + 1) * pagenum; newpage = (Convert.ToInt32(curpage) + 1); }
            if (cate == "first") { startnum = 0; endnum = 1 * pagenum; newpage = 1; }
            if (cate == "last") { startnum = (totalpage - 1) * pagenum; endnum = totalpage * pagenum; newpage = totalpage; }
            

            sql = @"select a.* 
                            from (
                                  select row_number() over (partition by type order by type,updatetime desc) numid
                                         ,id,type,title,to_char(updatetime,'yyyy/mm/dd hh24:mi:ss') as updatetime 
                                  from web_notice where isinvalid=0 and type='{0}'
                                  order by type,updatetime desc
                                  ) a 
                            where numid>{1} and numid<={2}";
            sql = string.Format(sql, type, startnum, endnum);
            DataTable dt = DBMgr.GetDataTable(sql);
            string result = JsonConvert.SerializeObject(dt);

            return "{\"resultdata\":" + result + ",\"newpage\":'" + newpage + "',\"totalpage\":'" + totalpage + "'}";
        }

        #endregion

        #region IndexNotice_M

        public string GetParentType()
        {
            DataTable dt_type = new DataTable();
            string sql = "select id,name type from newscategory where pid is null order by sortindex,id";
            dt_type = DBMgr.GetDataTable(sql);
            string result = JsonConvert.SerializeObject(dt_type);
            return "{\"Ptypedata\":" + result + "}";
        }

        //[HttpPost]
        public ActionResult IndexNotice_M(string typeid,string title, int id = 1)
        {
            //ViewBag.navigator = "关务云>>首页";
            ViewBag.IfLogin = !string.IsNullOrEmpty(HttpContext.User.Identity.Name);

            string strwhere = " isinvalid=0";
            if (!string.IsNullOrWhiteSpace(typeid))
            {
                strwhere += " and (type='" + typeid + "' or type in(select id  from newscategory START WITH pid ='" + typeid + "' connect by prior id=pid))";
            }
            if (!string.IsNullOrWhiteSpace(title))
            {
                strwhere += " and title like '%" + title + "%'";
            }

            int startIndex = (id - 1) * pagenum;
            int endIndex = id * pagenum;

            string sql = @"select *                                
                            from (
                                    select row_number() over (order by publishdate desc) numid,t.id
                                         ,case when length(t.title)>50 then substr(t.title,1,50)||'...' else t.title end title
                                         ,to_char(t.publishdate,'yyyy-mm-dd') as publishdate ,to_char(t.updatetime,'yyyy-mm-dd hh24:mi') as updatetime 
                                    from web_notice t where " + strwhere + 
                            @") a where numid>{0} and numid<={1}";
            sql = string.Format(sql, startIndex, endIndex);


            DataTable dt = DBMgr.GetDataTable(sql);
            List<Web_Notice> list_notice = new List<Web_Notice>();
            foreach (DataRow dr in dt.Rows)
            {
                Web_Notice web_notice = new Web_Notice();
                web_notice.ID = Int32.Parse(dr["id"].ToString());
                web_notice.Title = dr["title"].ToString();
                web_notice.PublishDate = dr["publishdate"].ToString();
                list_notice.Add(web_notice);

            }

            sql = "select count(*) from web_notice where" + strwhere;
            DataTable dt_count = DBMgr.GetDataTable(sql);
            int totalnum = Convert.ToInt32(dt_count.Rows[0][0].ToString());

            var model = new PagedList<Web_Notice>(list_notice, id, pagenum, totalnum);

            if (Request.IsAjaxRequest())
                return PartialView("IndexNotice_M_Part", model);
            return View(model);

        }

        public string GetChildrenType(string id)
        {
            string sql = string.Empty;

            string children = "[";
            sql = @"select t.* from NEWSCATEGORY t where  t.PID ='" +id + "' order by sortindex";

            DataTable dt = DBMgr.GetDataTable(sql);
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                string tmp_children = GetChildrenType(dr["id"].ToString());

                children += "{ID:'" + dr["id"] + "',NAME:'" + dr["NAME"] + "',PID:'" + dr["PID"] + "',leaf:'" + dr["ISLEAF"] + "',children:" + tmp_children + "}";

                if (i != dt.Rows.Count - 1)
                {
                    children += ",";
                }
                i++;
            }
            children += "]";
            return children;
        }

        #endregion

        [Filters.DecodeFilter]
        public ActionResult IndexNoticeDetail(string ID)
        {
            Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();//新建字典
            DataTable dt_notice = new DataTable();
            DataTable dt_notice_pre = new DataTable(); DataTable dt_notice_next = new DataTable();
            string type = "", typename = "", sql = "";

            sql = @"select type,(select name from newscategory where id=a.type) typename,title,content,attachment,to_char(a.publishdate,'yyyy-mm-dd') as publishdate 
                        ,to_char(updatetime,'yyyy/mm/dd hh24:mi:ss') as updatetime,REFERENCESOURCE 
                        ,0 iscollect
                    from web_notice a where isinvalid=0 and id='" + ID + "'";
            dt_notice = DBMgr.GetDataTable(sql);

            if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                JObject json_user = Extension.Get_UserInfo(HttpContext.User.Identity.Name);
                sql = @"select count(1) from list_collect_infor_BYUSER where type='news' and sysuserid='" + json_user.Value<string>("ID") + "' and rid='" + ID + "'";
                dt_notice.Rows[0]["iscollect"] = DBMgr.GetDataTable(sql).Rows[0][0];
            }


            dic.Add("dt_notice", dt_notice);
            type = dt_notice.Rows[0]["type"].ToString();typename = dt_notice.Rows[0]["typename"].ToString();

            dt_notice_pre = DBMgr.GetDataTable("select ID,title from web_notice where isinvalid=0 and type='" + type + "' and id<" + Convert.ToInt32(ID) + " and ROWNUM=1 order by publishdate desc");
            dic.Add("dt_notice_pre", dt_notice_pre);

            dt_notice_next = DBMgr.GetDataTable("select ID,title from web_notice where isinvalid=0 and type='" + type + "' and id>" + Convert.ToInt32(ID) + " and ROWNUM=1 order by publishdate");
            dic.Add("dt_notice_next", dt_notice_next);

            ViewBag.navigator = "资讯动态 > 分类：" + typename;
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
                                //result += "<li><a href=\"" + icon + dt2.Rows[j]["URL"] + "\">" + dt2.Rows[j]["NAME"] + "</a>";
                                //result += "<li><a href=\"" + dt2.Rows[j]["URL"] + "\">" + icon + dt2.Rows[j]["NAME"] + "</a>";

                                if ( dt2.Rows[j]["URL"].ToString().IndexOf("?")>0)
                                {
                                    result += "<li><a href=\""
                                        + dt2.Rows[j]["URL"].ToString().Substring(0, dt2.Rows[j]["URL"].ToString().IndexOf("?") + 1)
                                        + DecodeBase.Encrypt(dt2.Rows[j]["URL"].ToString().Substring(dt2.Rows[j]["URL"].ToString().IndexOf("?") + 1))
                                        + "\">" + icon + dt2.Rows[j]["NAME"] + "</a>";
                                }
                                else
                                {
                                    result += "<li><a href=\"" + dt2.Rows[j]["URL"] + "\">" + icon + dt2.Rows[j]["NAME"] + "</a>";
                                }

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
                                        //result += "<li><a href=\"" + dt3.Rows[k]["URL"] + "\">" + icon + dt3.Rows[k]["NAME"] + "</a></li>";
                                        if (dt3.Rows[k]["URL"].ToString().IndexOf("?") > 0)
                                        {
                                            result += "<li><a href=\""
                                                + dt3.Rows[k]["URL"].ToString().Substring(0, dt3.Rows[k]["URL"].ToString().IndexOf("?") + 1)
                                                + DecodeBase.Encrypt(dt3.Rows[k]["URL"].ToString().Substring(dt3.Rows[k]["URL"].ToString().IndexOf("?") + 1))
                                                + "\">" + icon + dt3.Rows[k]["NAME"] + "</a>";
                                        }
                                        else
                                        {
                                            result += "<li><a href=\"" + dt3.Rows[k]["URL"] + "\">" + icon + dt3.Rows[k]["NAME"] + "</a>";
                                        }
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



        public string Encrypt()//进行DES加密。
        {
            return DecodeBase.Encrypt(Request["para"].ToString());
        }
        public string Decrypt()//进行DES解密。
        {
            return DecodeBase.Decrypt(Request["para"].ToString());
        }
    }
}
