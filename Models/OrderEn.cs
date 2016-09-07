using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MvcPlatform.Models
{
    public class OrderEn
    {
       
        /// <summary>
        /// 订单id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        [DisplayName("订单编号")]
        public string Code { get; set; }
        /// <summary> 
        /// 业务类型 
        /// </summary> 
        [DisplayName("业务类型")]
        public string BusiType { get; set; }
        /// <summary>
        /// 业务类型名称
        /// </summary>
        [DisplayName("业务类型名称")]
        public string BusiName { get; set; }
        /// <summary> 
        /// 客户编号(企业编号) 
        /// </summary> 
        [DisplayName("业务类型名称")]
        public string CusNo { get; set; }
        /// <summary>
        /// 所属客户
        /// </summary>
        [DisplayName("所属客户")]
        public string CustomerCode { get; set; }
        /// <summary>
        /// 委托单位名称
        /// </summary>
        [DisplayName("委托单位名称")]
        public string CustomerName { get; set; }
        /// <summary> 
        /// 经营单位代码
        /// </summary> 

        [DisplayName("经营单位代码")]
        public string BusiUnitCode { get; set; }
        /// <summary>
        /// 经营单位名字
        /// </summary>
        [DisplayName("经营单位名字")]
        public string BusiUnitName { get; set; }
        /// <summary>
        /// 业务类别
        /// </summary>

        [DisplayName("业务类别")]
        public string BusiKind { get; set; }
        /// <summary>
        /// 合同发票号
        /// </summary>
        [DisplayName("合同发票号")]
        public string ContractNo { get; set; }

        /// <summary>
        /// 总单号
        /// </summary>
        [DisplayName("总单号")]
        public string TotalNo { get; set; }
        /// <summary>
        /// 分单号
        /// </summary>
        [DisplayName("分单号")]
        public string DivideNo { get; set; }
        /// <summary> 
        /// 转关预录号 
        /// </summary> 
        [DisplayName("转关预录号")]
        public string TurnPreNo { get; set; }
        /// <summary>
        /// 申报单位
        /// </summary>
        [DisplayName("申报单位代码")]
        public string RepUnitCode { get; set; }
        [DisplayName("申报单位名称")]
        public string RepUnitName { get; set; }
        /// <summary>
        /// 报检单位
        /// </summary>
        [DisplayName("报检单位代码")]
        public string InspUnitCode { get; set; }
        [DisplayName("报检单位名称")]
        public string InspUnitName { get; set; }
        /// <summary>
        /// 木质包装
        /// </summary>
        [DisplayName("木质包装")]
        public string WoodPackingId { get; set; }
        /// <summary>
        /// 法检状况（是否）
        /// </summary>
        [DisplayName("法检状况")]
        public int LawCondition { get; set; }
        /// <summary>
        /// 件数
        /// </summary>
        [DisplayName("件数")]
        public int GoodsNum { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        [DisplayName("重量")]
        public decimal GoodsWeight { get; set; }
        /// <summary>
        /// 毛重
        /// </summary>
        [DisplayName("毛重")]
        public decimal GoodsGW { get; set; }
        /// <summary>
        /// 净重
        /// </summary>
        [DisplayName("净重")]
        public decimal GoodsNw { get; set; }
        /// <summary>
        /// 货物类型
        /// </summary>
        [DisplayName("货物类型代码")]
        public string GoodsTypeId { get; set; }
        [DisplayName("货物类型名称")]
        public string GoodsTypeName { get; set; }
        /// <summary>
        /// 集装箱号
        /// </summary>
        // [DisplayName("集装箱号")]
        // public string ContainerNo { get; set; }
        /// <summary>
        /// 载货清单
        /// </summary>
        [DisplayName("载货清单")]
        public string Manifest { get; set; }
        /// <summary>
        /// 检疫监管单号
        /// </summary>
        [DisplayName("检疫监管单号")]
        public string RegNo { get; set; }
        /// <summary>
        /// 申报方式ID
        /// </summary>
        [DisplayName("申报方式ID")]
        public string RepWayId { get; set; }

        /// <summary>
        /// 申报方式
        /// </summary>
        [DisplayName("申报方式")]
        public string RepwayName { get; set; }
        /// <summary>
        /// 申报关区代码
        /// </summary>
        [DisplayName("申报关区代码")]
        public string CustomDistrictCode { get; set; }
        /// <summary>
        /// 申报关区名称
        /// </summary>
        [DisplayName("申报关区名称")]
        public string CustomDistrictName { get; set; }
        /// <summary>
        /// 报关方式
        /// </summary>
        [DisplayName("报关方式")]
        public string DeclWay { get; set; }

        /// <summary>
        /// 进出口口岸代码
        /// </summary>
        [DisplayName("进出口口岸代码")]
        public string PortCode { get; set; }
        /// <summary>
        /// 进出口口岸名称
        /// </summary>
        [DisplayName("进出口口岸名称")]
        public string PortName { get; set; }
        /// <summary>
        /// 一程提单
        /// </summary>
        [DisplayName("一程提单")]
        public string FirstLadingBillNo { get; set; }
        /// <summary>
        /// 二程提单
        /// </summary>
        [DisplayName("二程提单")]
        public string SecondLadingBillNo { get; set; }
        /// <summary>
        /// 是否二次转关
        /// </summary>
        [DisplayName("是否二次转关")]
        public int SecondTransit { get; set; }
        /// <summary>
        /// 报关单号
        /// </summary>
        [DisplayName("报关单号")]
        public string DeclarationNo { get; set; }
        /// <summary>
        /// 委托要求
        /// </summary>
        [DisplayName("委托要求")]
        public string EntrustRequest { get; set; }
        /// <summary>
        /// 委托类型
        /// </summary>
        [DisplayName("委托类型")]
        public string EntrustTypeId { get; set; }
        /// <summary>
        /// 委托类型名称
        /// </summary>
        [DisplayName("委托类型名称")]
        public string EntrustTypeName { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [DisplayName("创建人")]
        public int CreateUserId { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        [DisplayName("创建人名称")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 下单（委托）人
        /// </summary>
        [DisplayName("下单（委托）人")]
        public int SubmitUserId { get; set; }
        /// <summary>
        /// 委托人名称
        /// </summary>
        [DisplayName("委托人名称")]
        public string SubmitUserName { get; set; }
        /// <summary>
        /// 委托人电话
        /// </summary>
          [DisplayName("委托人电话")]
        public string SubmitUserPhone { get; set; }
        /// <summary>
        /// 委托时间
        /// </summary>
        [DisplayName("委托时间")]
        public DateTime SubmitTime { get; set; }
        /// <summary>
        /// 接单（受理）人
        /// </summary>
        [DisplayName("接单（受理）人")]
        public int CSACCEPTUSERID { get; set; }
        /// <summary>
        /// 受理人名称
        /// </summary>
        [DisplayName("受理人名称")]
        public string AcceptUserName { get; set; }
        /// <summary>
        /// 受理时间
        /// </summary>
        [DisplayName("受理时间")]
        public DateTime AcceptTime { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        [DisplayName("订单状态")]
        public int Status { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        [DisplayName("订单状态名称")]
        public string StatusName { get; set; }
        /// <summary>
        /// 报关单状态
        /// </summary>
        [DisplayName("报关单状态")]
        public int DeclStatus { get; set; }
        /// <summary>
        /// 报关状态名称
        /// </summary>
        [DisplayName("报关状态名称")]
        public string DeclStatusName { get; set; }
        /// <summary>
        /// 报检单状态
        /// </summary>
        [DisplayName("报检单状态")]
        public int InspStatus { get; set; }
        /// <summary>
        /// 报检单状态
        /// </summary>
        [DisplayName("报检单状态名称")]
        public string InspStatusName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 船名
        /// </summary>
        [DisplayName("船名")]
        public string ShipName { get; set; }
        /// <summary>
        /// 航次
        /// </summary>
        [DisplayName("航次")]
        public string FilghtNo { get; set; }
        /// <summary>
        /// 提单号
        /// </summary>
        [DisplayName("提单号")]
        public string LadingBillNo { get; set; }
        /// <summary>
        /// 贸易方式（多个贸易方式组合）
        /// </summary>
        [DisplayName("贸易方式")]
        public string TradeWayCodes { get; set; }
        /// <summary>
        /// 贸易方式1
        /// </summary>
        [DisplayName("贸易方式1")]
        public string TradeWayCodes1 { get; set; }

        /// <summary>
        /// 接单方式
        /// </summary>
        [DisplayName("接单方式")]
        public string OrderWay { get; set; }
        /// <summary>
        /// 通关单号
        /// </summary>
        [DisplayName("通关单号")]
        public string ClearanceNo { get; set; }
        /// <summary>
        /// 计算单位
        /// </summary>
        [DisplayName("计算单位")]
        public string ClearUnit { get; set; }
        /// <summary>
        /// 包装种类
        /// </summary>
        [DisplayName("包装种类")]
        public string PackKind { get; set; }
        /// <summary>
        /// 包装类别名称
        /// </summary>
        [DisplayName("包装类别名称")]
        public string PackKindName { get; set; }
        /// <summary>
        /// 运抵编号
        /// </summary>
        [DisplayName("运抵编号")]
        public string ArrivedNo { get; set; }
        /// <summary>
        /// 路陆监管号
        /// </summary>
        [DisplayName("路陆监管号")]
        public string LandRegNo { get; set; }

        /// <summary>
        /// 客服指令
        /// </summary>
        [DisplayName("客服指令")]
        public string CSRequest { get; set; }
        /// <summary>
        /// 结算备注
        /// </summary>
        [DisplayName("结算备注")]
        public string ClearRemark { get; set; }
        /// <summary>
        /// 客服
        /// </summary>
        [DisplayName("客服")]
        public int CSId { get; set; }
        /// <summary>
        /// 客服名称
        /// </summary>
        [DisplayName("客服名称")]
        public string CSName { get; set; }
        /// <summary>
        /// 客服电话
        /// </summary>
          [DisplayName("客服电话")]
        public string CSPhone { get; set; }
        /// <summary>
        /// 客服认领时间
        /// </summary>
        [DisplayName("客服认领时间")]
        public DateTime CSAcceptTime { get; set; }
        /// <summary>
        /// 客服操作时间
        /// </summary>
        [DisplayName("客服操作时间")]
        public DateTime CSInputTime { get; set; }
        /// <summary>
        /// 客服提交时间
        /// </summary>
        [DisplayName("客服提交时间")]
        public DateTime CSSubmitTime { get; set; }
        /// <summary>
        /// 制单人ID
        /// </summary>
        [DisplayName("制单人")]
        public int MOId { get; set; }
        /// <summary>
        /// 制单人名称
        /// </summary>
        [DisplayName("制单人名称")]
        public string MOName { get; set; }
        /// <summary>
        /// 制单认领时间
        /// </summary>
        [DisplayName("制单认领时间")]
        public DateTime MOAcceptTime { get; set; }
        /// <summary>
        /// 制单提交时间
        /// </summary>
        [DisplayName("制单完成时间")]
        public DateTime MOSubmitTime { get; set; }
        /// <summary>
        /// 审单
        /// </summary>
        [DisplayName("审单")]
        public int COId { get; set; }
        /// <summary>
        /// 认领制单人
        /// </summary>
        [DisplayName("认领制单人")]
        public int MoAcceptUserid { get; set; }
        /// <summary>
        /// 认领审单人
        /// </summary>
        [DisplayName("认领审单人")]
        public int CoAcceptUserid { get; set; }
        /// <summary>
        /// 审单名称
        /// </summary>
        [DisplayName("审单名称")]
        public string COName { get; set; }
        /// <summary>
        /// 审单认领时间
        /// </summary>
        [DisplayName("审单认领时间")]
        public DateTime COAcceptTime { get; set; }

        /// <summary>
        /// 审单提交时间
        /// </summary>
        [DisplayName("审单提交时间")]
        public DateTime COSubmitTime { get; set; }
        /// <summary>
        /// 预录管理开始时间
        /// </summary>
        [DisplayName("预录管理开始时间")]
        public DateTime PerAcceptTime { get; set; }
        /// <summary>
        /// 预录管理完成时间
        /// </summary>
        [DisplayName("预录管理完成时间")]
        public DateTime PerSubmitTime { get; set; }
        /// <summary>
        /// 是否作废
        /// </summary>
        [DisplayName("是否作废")]
        public int Isinvalid { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        [DisplayName("优先级")]
        public int Priority { get; set; }
        /// <summary>
        /// 优先级名称
        /// </summary>
        [DisplayName("优先级名称")]
        public string PriorityName { get; set; }
        /// <summary>
        /// 业务对应号，四单关联
        /// </summary>
        [DisplayName("业务对应号")]
        public string CorrespondNo { get; set; }
        /// <summary>
        /// 关联编号，两单关联
        /// </summary>
        [DisplayName("关联编号")]
        public string AssociateNo { get; set; }
        /// <summary>
        /// 内部类型
        /// </summary>
        [DisplayName("内部类型")]
        public string internaltype { get; set; }
        /// <summary>
        /// 关联报关单号
        /// </summary>
        [DisplayName("关联报关单号")]
        public string AssociatePedeclNo { get; set; }
        /// <summary>
        /// 关联贸易方式
        /// </summary>
        [DisplayName("关联贸易方式")]
        public string ASSOCIATETRADEWAY { get; set; }
        /// <summary>
        /// 是否暂存
        /// </summary>
        [DisplayName("是否暂存")]
        public int IsPause { get; set; }

        //以下为非数据库表字段//////////////////////////////////////
        /// <summary>
        /// 申报时间
        /// </summary>
        public DateTime DeclarationTime { get; set; }
        public string OtherCustomsNo { get; set; }

        /// <summary>
        /// 备用字段
        /// </summary>
        public int Backup { get; set; }
        /// <summary>
        /// 制单时长
        /// </summary>
        [Description("制单时长")]
        public int TimeLong { get; set; }
        /// <summary>
        /// 有无差错
        /// </summary>
        [Description("有无差错")]
        public string Is_Error { get; set; }
        /// <summary>
        /// 差错字段数
        /// </summary>
        [Description("差错字段数")]
        public string Error_Num { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        [Description("部门名称")]
        public string DeptName { get; set; }
        /// <summary>
        /// 申报备案号
        /// </summary>
        [Description("申报备案号")]
        public string FilingNumber { get; set; }
        /// <summary>
        /// 集装箱号
        /// </summary>
        [DisplayName("集装箱号")]
        public string ContainerNo { get; set; }

        /// <summary>
        /// 报关车号
        /// </summary>
        [DisplayName("报关车号")]
        public string DECLCARNO { get; set; }
        /// <summary>
        /// 归属地
        /// </summary>
        [DisplayName("归属地")]
        public string Attribution { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int orderNO { get; set; }


        /// <summary>
        /// 预制报关单套数
        /// </summary>
        public int DeclSetNum { get; set; }
        /// <summary>
        /// 预制报检单套数
        /// </summary>
        public int InspSetNum { get; set; }
        /// <summary>
        /// 预制报关单张数
        /// </summary>
        public int DeclSheetNum { get; set; }
        /// <summary>
        /// 预制报检单张数
        /// </summary> 
        public int InspSheetNum { get; set; }
        /// <summary>
        /// 报关草单套数
        /// </summary>
        public int PreDeclSetNum { get; set; }
        /// <summary>
        /// 报检草单套数
        /// </summary>
        public int PreInspSetNum { get; set; }




        #region 业务处理需要的字段，数据库中没有改字段

        /// <summary>
        /// 制单开始时间
        /// </summary>
        public DateTime MOStartTime { get; set; }
        //预警字段
        /// <summary>
        /// 预警颜色
        /// </summary>
        public string waring_Color { get; set; }
        /// <summary>
        /// 预警启用条件
        /// </summary>
        public string warning_Condition { get; set; }
        /// <summary>
        /// 预警判断内容
        /// </summary>
        public string warning_Content { get; set; }
        /// <summary>
        /// 实际结果
        /// </summary>
        public string warning_Result { get; set; }
        /// <summary>
        /// 预警提示
        /// </summary>
        public string waring_Prompt { get; set; }
        #endregion
    }
}