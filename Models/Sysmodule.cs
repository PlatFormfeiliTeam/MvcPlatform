using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPlatform.Models
{
    public class Sysmodule
    {
        public string MODULEID { get; set; }
        public string id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        public string ParentID { get; set; }
        public string leaf { get; set; }

        public string iconCls { get; set; }

        public bool check { get; set; }
        /// <summary>
        /// 集装箱序号
        /// </summary>
        public string URL { get; set; }
        public List<Sysmodule> children { get; set; }
    }
}