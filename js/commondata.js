//委托类型
var wtlx_js_data = [{ "CODE": "01", "NAME": "报关单(01)" }, { "CODE": "02", "NAME": "报检单(02)" }, { "CODE": "03", "NAME": "报关/检单(03)" }];

//货物类型
var hwlx_js_data = [{ "CODE": "1", "NAME": "整箱(1)" }, { "CODE": "2", "NAME": "散货(2)" }];

//国内结转业务 进出口类型
var ietype_js_data = [{ CODE: '仅进口', NAME: '仅进口' }, { CODE: '仅出口', NAME: '仅出口' }, { CODE: '进/出口业务', NAME: '进/出口业务' }];

//业务类型静态数据
var common_data_busitype = [
    { "CODE": "10", "NAME": "空运出口" }, { "CODE": "11", "NAME": "空运进口" },
    { "CODE": "20", "NAME": "海运出口" }, { "CODE": "21", "NAME": "海运进口" },
    { "CODE": "30", "NAME": "陆运出口" }, { "CODE": "31", "NAME": "陆运进口" },
    { "CODE": "40", "NAME": "国内出口" }, { "CODE": "41", "NAME": "国内进口" },
    { "CODE": "50", "NAME": "特殊区域出口" }, { "CODE": "51", "NAME": "特殊区域进口" }];

//报关单管理 查询区域 查询条件4 /Common/condition4
var declarationsearch_js_condition4_data = [{ "NAME": "订单委托日期", "CODE": "SUBMITTIME" }, { "NAME": "申报完成", "CODE": "REPTIME" }];

//查询区域 查询条件1 /Common/loadcondition1
var search_js_condition1_data = [{ "NAME": "经营单位", "CODE": "BUSIUNITCODE" }, { "NAME": "申报关区", "CODE": "CUSTOMDISTRICTCODE" }, { "NAME": "进口口岸", "CODE": "PORTCODE" }, { "NAME": "申报方式", "CODE": "REPWAYID" }];
//查询区域 查询条件2 /Common/loadcondition2
var search_js_condition2_data = [{ "NAME": "订单编号", "CODE": "CODE" }, { "NAME": "客户编号", "CODE": "CUSNO" }, { "NAME": "分单号", "CODE": "DIVIDENO" }, { "NAME": "合同发票号", "CODE": "CONTRACTNO" }, { "NAME": "载货清单号", "CODE": "MANIFEST" }, { "NAME": "海关提运单号", "CODE": "SECONDLADINGBILLNO" }];
//查询区域 查询条件3 /Common/loadcondition3
var search_js_condition3_data = [{ "NAME": "报关状态", "CODE": "bgzt" }, { "NAME": "报检状态", "CODE": "bjzt" }];
//查询区域 查询条件4 /Common/loadcondition4
var search_js_condition4_data = [{ "NAME": "委托日期", "CODE": "SUBMITTIME" }, { "NAME": "订单开始时间", "CODE": "CSSTARTTIME" }];

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
    , { "CODE": 120, "NAME": "申报完成" }, { "CODE": 130, "NAME": "申报完结" }];

//查询的render方法
var orders_tatus = {
    '0': "草稿", '10': "已委托", '15': "已受理", '20': "制单中", '30': "制单完成", '40': "审核中", '50': "审核完成", '60': "待预录"
    , '70': "预录中", '80': "预录完成", '90': "审核校验完成", '100': "申报中", '105': "提前转关生成", '110': "提前申报完成"
    , '120': "申报完成", '130': "申报完结"
}

//1,查询条件：草稿、已委托、申报中、申报完结、未完结（差的是没有申报完结的范围）
//去掉订单状态：查询，列表展示；查询时，选的是报关状态，where条件就是报关状态=。。。
//保存的时候：除了修改status,还要修改报关报检状态