using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcPlatform.Models
{
    public class Web_Notice
    {
        [Display(Name = "Web_Notice.ID")]
        public int ID { get; set; }


        [Display(Name = "类别ID")]
        [MaxLength(20)]
        public string Type { get; set; }


        [Display(Name = "标题")]
        [MaxLength(200)]
        public string Title { get; set; }


        [Display(Name = "内容")]
        public string Content { get; set; }


        [Display(Name = "发布日期")]
        public DateTime PublishDate { get; set; }

        
        [Display(Name = "本文来源")]
        [MaxLength(20)]
        public string ReferenceSource { get; set; }

    }
}