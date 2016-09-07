using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcPlatform.Models
{
    /// <summary>
    /// 岗位
    /// </summary>
    public enum PostEnum
    {
        //1客服2制单3审单4报关5报检6监管
        /// <summary>
        /// 客服
        /// </summary>
        CS = 1,
        /// <summary>
        /// 制单
        /// </summary>
        MO,
        /// <summary>
        /// 审单
        /// </summary>
        CO,
        /// <summary>
        /// 报关
        /// </summary>
        DECL,
        /// <summary>
        /// 报检
        /// </summary>
        INSP,
        /// <summary>
        /// 监管
        /// </summary>
        REGU
    }
}
