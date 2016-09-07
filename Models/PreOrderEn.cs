using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcPlatform.Models
{
    /// <summary>
    /// 订单预配实体
    /// </summary>
    [Serializable]
    public class PreOrderEn
    {
        private int id;
        /// <summary>
        /// 编号
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private int userId;
        /// <summary>
        /// 员工id
        /// </summary>
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        private string userName;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }


        private int areaId;
        /// <summary>
        /// 操作区域编号
        /// </summary>
        public int AreaId
        {
            get { return areaId; }
            set { areaId = value; }
        }
        private string areaName;
        /// <summary>
        /// 操作区域名字
        /// </summary>
        public string AreaName
        {
            get { return areaName; }
            set { areaName = value; }
        }
        private int preType;
        /// <summary>
        /// 预配类型编号，1客服2制单3审单4报关5报检6监管
        /// </summary>
        public int PreType
        {
            get { return preType; }
            set { preType = value; }
        }
        /// <summary>
        /// 预配类型名称
        /// </summary>
        public string PreTypeName { get; set; }

        private string  busiTypes;
        /// <summary>
        /// 业务类型编号集合，多个之间用逗号隔开
        /// </summary>
        public string BusiTypes
        {
            get { return busiTypes; }
            set { busiTypes = value; }
        }
        /// <summary>
        /// 业务类型名称集合，多个之间用逗号隔开
        /// </summary>
        public string BusiTypeNames { get; set; }

        private string customerIds;
        /// <summary>
        /// 委托单位编号，多个之间用逗号隔开
        /// </summary>
        public string CustomerIds
        {
            get { return customerIds; }
            set { customerIds = value; }
        }
        private string customerNames;
        /// <summary>
        /// 委托单位名字，多个之间用逗号隔开
        /// </summary>
        public string CustomerNames
        {
            get { return customerNames; }
            set { customerNames = value; }
        }
        private string busiUnitCodes;
        /// <summary>
        /// 经营单位编号，多个之间用逗号隔开
        /// </summary>
        public string BusiUnitCodes
        {
            get { return busiUnitCodes; }
            set { busiUnitCodes = value; }
        }
        private string busiUnitNames;
        /// <summary>
        /// 经营单位名字，多个之间用逗号隔开
        /// </summary>
        public string BusiUnitNames
        {
            get { return busiUnitNames; }
            set { busiUnitNames = value; }
        }
        
       
       




        private int enabled;
        /// <summary>
        /// 是否启用
        /// </summary>
        public int Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        private string enabled2;
        /// <summary>
        /// 是否启用
        /// </summary>
        public string Enabled2
        {
            get { return enabled2; }
            set { enabled2 = value; }
        }

        /// <summary>
        /// 备用字段
        /// </summary>
        public string Reserve { get; set; }
    }
}
