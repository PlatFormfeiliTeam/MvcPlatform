//委托类型
var wtlx_js_data = [{ "CODE": "01", "NAME": "报关单(01)" }, { "CODE": "02", "NAME": "报检单(02)" }, { "CODE": "03", "NAME": "报关/检单(03)" }];

//委托类型-关务服务
var wtlx_js_data_CustomsService = [{ "CODE": "05", "NAME": "企业备案（手册）" }, { "CODE": "06", "NAME": "减免税备案" }, { "CODE": "07", "NAME": "代理预录入" },
    { "CODE": "08", "NAME": "账册备案" }, { "CODE": "09", "NAME": "其他" }];
//费用类别-关务服务
var fylb_js_data = [{ "CODE": "DLF", "NAME": "代理费(DLF)" }, { "CODE": "SJF", "NAME": "输机费(SJF)" }, { "CODE": "QTF", "NAME": "其他费(QTF)" }];

//货物类型
var hwlx_js_data = [{ "CODE": "1", "NAME": "整箱(1)" }, { "CODE": "2", "NAME": "散货(2)" }];

//国内结转业务 进出口类型
var ietype_js_data = [{ CODE: '仅进口', NAME: '仅进口' }, { CODE: '仅出口', NAME: '仅出口' }, { CODE: '进/出口业务', NAME: '进/出口业务' }];

//业务类型静态数据
var common_data_busitype = [
    { "CODE": "10", "NAME": "空运出口", "CODENAME": "空运出口(10)" }, { "CODE": "11", "NAME": "空运进口", "CODENAME": "空运进口(11)" },
    { "CODE": "20", "NAME": "海运出口", "CODENAME": "海运出口(20)" }, { "CODE": "21", "NAME": "海运进口", "CODENAME": "海运进口(21)" },
    { "CODE": "30", "NAME": "陆运出口", "CODENAME": "陆运出口(30)" }, { "CODE": "31", "NAME": "陆运进口", "CODENAME": "陆运进口(31)" },
    { "CODE": "40", "NAME": "国内出口", "CODENAME": "国内出口(40)" }, { "CODE": "41", "NAME": "国内进口", "CODENAME": "国内进口(41)" },
    { "CODE": "50", "NAME": "特殊区域出口", "CODENAME": "特殊区域出口(50)" }, { "CODE": "51", "NAME": "特殊区域进口", "CODENAME": "特殊区域进口(51)" }];

var common_data_busitype_Domestic = [{ "CODE": "40", "NAME": "国内出口", "CODENAME": "国内出口(40)" }, { "CODE": "41", "NAME": "国内进口", "CODENAME": "国内进口(41)" }];
var common_data_busitype_Not_Domestic = [
    { "CODE": "10", "NAME": "空运出口", "CODENAME": "空运出口(10)" }, { "CODE": "11", "NAME": "空运进口", "CODENAME": "空运进口(11)" },
    { "CODE": "20", "NAME": "海运出口", "CODENAME": "海运出口(20)" }, { "CODE": "21", "NAME": "海运进口", "CODENAME": "海运进口(21)" },
    { "CODE": "30", "NAME": "陆运出口", "CODENAME": "陆运出口(30)" }, { "CODE": "31", "NAME": "陆运进口", "CODENAME": "陆运进口(31)" },
    { "CODE": "50", "NAME": "特殊区域出口", "CODENAME": "特殊区域出口(50)" }, { "CODE": "51", "NAME": "特殊区域进口", "CODENAME": "特殊区域进口(51)" }];

//报关单管理 查询区域 查询条件4 /Common/condition4
var declarationsearch_js_condition4_data = [{ "NAME": "订单委托日期", "CODE": "SUBMITTIME" }, { "NAME": "申报日期", "CODE": "REPTIME" }];

//查询区域 查询条件1 /Common/loadcondition1
var search_js_condition1_data = [{ "NAME": "经营单位", "CODE": "BUSIUNITCODE" }, { "NAME": "申报关区", "CODE": "CUSTOMDISTRICTCODE" }, { "NAME": "进口口岸", "CODE": "PORTCODE" }, { "NAME": "申报方式", "CODE": "REPWAYID" }];
//查询区域 查询条件2 /Common/loadcondition2
var search_js_condition2_data = [{ "NAME": "订单编号", "CODE": "CODE" }, { "NAME": "客户编号", "CODE": "CUSNO" }, { "NAME": "分单号", "CODE": "DIVIDENO" }
    , { "NAME": "合同发票号", "CODE": "CONTRACTNO" }, { "NAME": "载货清单号", "CODE": "MANIFEST" }, { "NAME": "海关提运单号", "CODE": "SECONDLADINGBILLNO" }
    , { "NAME": "报关单号", "CODE": "DECLARATIONCODE" }, { "NAME": "报检单号", "CODE": "INSPECTIONCODE" }, { "NAME": "通关单号", "CODE": "CLEARANCECODE" }
    ];
//查询区域 查询条件3 /Common/loadcondition3
var search_js_condition3_data = [{ "NAME": "报关状态", "CODE": "bgzt" }, { "NAME": "报检状态", "CODE": "bjzt" }];
//查询区域 查询条件4 /Common/loadcondition4
var search_js_condition4_data = [{ "NAME": "订单开始日期", "CODE": "CSSTARTTIME" }, { "NAME": "委托日期", "CODE": "SUBMITTIME" }
    , { "NAME": "报关查验日期", "CODE": "DECLCHECKTIME" }, { "NAME": "报检查验日期", "CODE": "INSPCHECKTIME" }
    , { "NAME": "报关稽核日期", "CODE": "AUDITFLAGTIME" }, { "NAME": "报检熏蒸日期", "CODE": "FUMIGATIONTIME" }
    , { "NAME": "现场报关日期", "CODE": "SITEAPPLYTIME" }, { "NAME": "现场报检日期", "CODE": "INSPSITEAPPLYTIME" }
    , { "NAME": "报关放行日期", "CODE": "SITEPASSTIME" }, { "NAME": "报检放行日期", "CODE": "INSPSITEPASSTIME" }
    ];

//查询区域 订单状态

//查询区域 报关报检状态 
var search_js_condition3_bgbjstatus_data = [{ "CODE": "草稿", "NAME": "草稿" }, { "CODE": "已委托", "NAME": "已委托" }
    , { "CODE": "申报中", "NAME": "申报中" }, { "CODE": "申报完结", "NAME": "申报完结" }, { "CODE": "未完结", "NAME": "未完结" }];

//create.cshtml 订单状态 
var orderstatus_js_data = [{ "CODE": 0, "NAME": "草稿" }, { "CODE": 10, "NAME": "已委托" }
    , { "CODE": 15, "NAME": "已受理" }, { "CODE": 20, "NAME": "制单中" }, { "CODE": 30, "NAME": "制单完成" }
    , { "CODE": 40, "NAME": "审核中" }, { "CODE": 50, "NAME": "审核完成" }, { "CODE": 60, "NAME": "待预录" }
    , { "CODE": 70, "NAME": "预录中" }, { "CODE": 80, "NAME": "预录完成" }, { "CODE": 90, "NAME": "审核校验完成" }
    , { "CODE": 100, "NAME": "申报中" }, { "CODE": 105, "NAME": "提前转关生成" }, { "CODE": 110, "NAME": "提前申报完成" }
    , { "CODE": 120, "NAME": "申报完成" }, { "CODE": 130, "NAME": "申报完结" }, { "CODE": 140, "NAME": "资料整理" }
    , { "CODE": 150, "NAME": "现场报关" }, { "CODE": 160, "NAME": "现场放行" }];

//查询的render方法
var orders_tatus = {
    '0': "草稿", '10': "已委托", '15': "已受理", '20': "制单中", '30': "制单完成", '40': "审核中", '50': "审核完成", '60': "待预录"
    , '70': "预录中", '80': "预录完成", '90': "审核校验完成", '100': "申报中", '105': "提前转关生成", '110': "提前申报完成"
    , '120': "申报完成", '130': "申报完结", '140': "资料整理", '150': "现场报关", '160': "现场放行"
}

//1,查询条件：草稿、已委托、申报中、申报完结、未完结（差的是没有申报完结的范围）
//去掉订单状态：查询，列表展示；查询时，选的是报关状态，where条件就是报关状态=。。。
//保存的时候：除了修改status,还要修改报关报检状态

//货况跟踪菜单项

var menu_js_data = [{ "NAME": "I跟踪", "URL": "http://www.igenzong.com/air/trace/", "busitypeid": "11" },
                    { "NAME": "I跟踪", "URL": "http://www.igenzong.com/air/trace/", "busitypeid": "10" },
                    { "NAME": "转关信息查询", "URL": "http://www.haiguan.info/onlinesearch/gateway/CabinOrder2.aspx", "busitypeid": "11" },
                    { "NAME": "转关信息查询", "URL": "http://www.haiguan.info/onlinesearch/gateway/CabinOrder2.aspx", "busitypeid": "10" },
                    { "NAME": "快件舱单确认信息查询", "URL": "http://www.haiguan.info/onlinesearch/gateway/FormHeadRtnAndEntryHead.aspx", "busitypeid": "11" },
                    { "NAME": "快件舱单确认信息查询", "URL": "http://www.haiguan.info/onlinesearch/gateway/FormHeadRtnAndEntryHead.aspx", "busitypeid": "10" },
                    { "NAME": "新舱单信息查询", "URL": "http://query.customs.gov.cn/MNFTQ/", "busitypeid": "11" },
                    { "NAME": "新舱单信息查询", "URL": "http://query.customs.gov.cn/MNFTQ/", "busitypeid": "10" },
                    { "NAME": "海勃", "URL": "http://www.hb56.com/Main.aspx", "busitypeid": "21" },
                    { "NAME": "海勃", "URL": "http://www.hb56.com/Main.aspx", "busitypeid": "20" },
                    { "NAME": "新舱单信息查询", "URL": "http://query.customs.gov.cn/MNFTQ/ ", "busitypeid": "21" },
                    { "NAME": "新舱单信息查询", "URL": "http://query.customs.gov.cn/MNFTQ/ ", "busitypeid": "20" }
]

var modifyflag_data = [{ "NAME": "正常", "CODE": "0" }, { "NAME": "删单", "CODE": "1" }, { "NAME": "改单", "CODE": "2" }, { "NAME": "改单完成", "CODE": "3" }];
var verstatus_data_search = [{ "NAME": "未比对", "CODE": "未比对" }, { "NAME": "比对中", "CODE": "比对中" }, { "NAME": "比对通过", "CODE": "比对通过" }, { "NAME": "比对未通过", "CODE": "比对未通过" }];

var logistic_status_data = [{ "NAME": "抽单完成", "CODE": ">=10" }, { "NAME": "抽单未完成", "CODE": "<10" },
                            { "NAME": "已派车", "CODE": ">=45" }, { "NAME": "未派车", "CODE": "<45" },
                            { "NAME": "运抵场站完成", "CODE": ">=70" }, { "NAME": "运抵场站未完成", "CODE": "<70" },
                            { "NAME": "送货完成", "CODE": ">=90" }, { "NAME": "送货未完成", "CODE": "<90" }];

//var logistic_status_data = [{ "NAME": "抽单完成", "CODE": ">='10'" },{ "NAME": "抽单未完成", "CODE": "<'10'" },
//                            { "NAME": "报关完成", "CODE": ">='30'" },{ "NAME": "报关未完成", "CODE": "<'30'" },
//                            { "NAME": "报检完成", "CODE": ">='40'" },{ "NAME": "报检未完成", "CODE": "<'40'" },
//                            { "NAME": "运输完成", "CODE": ">='70'" },{ "NAME": "运输未完成", "CODE": "<'70'" }];