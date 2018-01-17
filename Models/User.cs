using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcPlatform.Models
{
    public class User
    {
        //[Required]
        //[EmailAddress]
        //[StringLength(150)]
       // [Display(Name = "Email:")]
        //[Remote("doesEmailExist", "User")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "用户名不能为空")]
        public string NAME { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "密码不能为空")]
        public string PASSWORD { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "公司代码不能为空")]
        public string CUSTOMERCODE { get; set; }
    }
}