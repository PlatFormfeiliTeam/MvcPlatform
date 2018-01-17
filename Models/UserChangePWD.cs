using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcPlatform.Models
{
    public class UserChangePWD
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "用户名不能为空")]
        public string NAME { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "密码不能为空")]
        public string PASSWORD { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "新密码不能为空")]
        public string NEWPASSWORD { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "确认密码不能为空")]
        public string CONFIRMPASSWORD { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "公司代码不能为空")]
        public string CUSTOMERCODE { get; set; }
    }
}