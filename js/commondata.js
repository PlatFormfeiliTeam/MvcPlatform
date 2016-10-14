//委托类型
var wtlx_js_data = [{ "CODE": "01", "NAME": "报关单(01)" }, { "CODE": "02", "NAME": "报检单(02)" }, { "CODE": "03", "NAME": "报关/检单(03)" }];

//货物类型
var hwlx_js_data =[{ "CODE": "1", "NAME": "整箱(1)" }, { "CODE": "2", "NAME": "散货(2)" }];

//查询区域 查询条件1 /Common/loadcondition1
var search_js_condition1_data = [{ "NAME": "经营单位", "CODE": "BUSIUNITCODE" }, { "NAME": "申报关区", "CODE": "CUSTOMDISTRICTCODE" }, { "NAME": "进口口岸", "CODE": "PORTCODE" }, { "NAME": "申报方式", "CODE": "REPWAYID" }];
//查询区域 查询条件2 /Common/loadcondition2
var search_js_condition2_data = [{ "NAME": "订单编号", "CODE": "CODE" }, { "NAME": "客户编号", "CODE": "CUSNO" }, { "NAME": "分单号", "CODE": "DIVIDENO" }, { "NAME": "合同发票号", "CODE": "CONTRACTNO" }, { "NAME": "载货清单号", "CODE": "MANIFEST" }];
//查询区域 查询条件3 /Common/loadcondition3
var search_js_condition3_data = [{ "NAME": "订单状态", "CODE": "ddzt" }, { "NAME": "报关状态", "CODE": "bgzt" }, { "NAME": "报检状态", "CODE": "bjzt" }, { "NAME": "预警状态", "CODE": "yjzt" }];
//查询区域 查询条件4 /Common/loadcondition4
var search_js_condition4_data = [{ "NAME": "委托日期", "CODE": "SUBMITTIME" }, { "NAME": "订单开始时间", "CODE": "CSSTARTTIME" }];

var search_js_condition3_orderstatus_data = [{ "CODE": "草稿", "NAME": "草稿" }, { "CODE": "订单待委托", "NAME": "订单待委托" },
    { "CODE": "订单待受理", "NAME": "订单待受理" }, { "CODE": "订单受理中", "NAME": "订单受理中" },
    { "CODE": "订单待交付", "NAME": "订单待交付" }, { "CODE": "订单已交付", "NAME": "订单已交付" },
    { CODE: '订单已作废', NAME: '订单已作废' }];

//查询区域 查询条件1 /Common/loadcondition3_1
var search_js_condition3_bgbjstatus_data = [{ "CODE": "50", "NAME": "制单中" }, { "CODE": "55", "NAME": "制单完成" }, { "CODE": "60", "NAME": "待审核" }
    , { "CODE": "65", "NAME": "审核已受理" }, { "CODE": "70", "NAME": "审核中（部分）" }, { "CODE": "71", "NAME": "审核中" }
    , { "CODE": "74", "NAME": "审核完成（部分）" }, { "CODE": "75", "NAME": "审核完成" }, { "CODE": "78", "NAME": "待预录" }
    , { "CODE": "80", "NAME": "预录已受理" }, { "CODE": "85", "NAME": "预录中(部分)" }, { "CODE": "86", "NAME": "预录中" }
    , { "CODE": "89", "NAME": "预录完成（部分）" }, { "CODE": "90", "NAME": "预录完成" }, { "CODE": "92", "NAME": "预录校验中" }
    , { "CODE": "93", "NAME": "预录校验完成（部分）" }, { "CODE": "95", "NAME": "预录校验完成" }, { "CODE": "100", "NAME": "申报中（部分）" }
    , { "CODE": "101", "NAME": "申报中" }, { "CODE": "102", "NAME": "申报退单" }, { "CODE": "103", "NAME": "提前申报完成" }
    , { "CODE": "104", "NAME": "申报完成（部分）" }, { "CODE": "105", "NAME": "申报完成" }, { "CODE": "106", "NAME": "申报完结（部分）" }
    , { "CODE": "107", "NAME": "申报完结" }];

//查询区域 查询条件1 /Common/loadcondition3_1
var search_js_condition3_yjstatus_data = [];

//国内结转业务 进出口类型
var ietype_js_data = [{ CODE: '仅进口', NAME: '仅进口' }, { CODE: '仅出口', NAME: '仅出口' }, { CODE: '进/出口业务', NAME: '进/出口业务' }];

//报关单管理 查询区域 查询条件4 /Common/condition4
var declarationsearch_js_condition4_data = [{ "NAME": "订单委托日期", "CODE": "SUBMITTIME" }, { "NAME": "申报时间", "CODE": "REPTIME" }];

//订单状态 create.cshtml
var orderstatus_js_data = [{ "CODE": 1, "NAME": "草稿" }, { "CODE": 10, "NAME": "文件已上传" }, { "CODE": 15, "NAME": "订单已委托" }
    , { "CODE": 20, "NAME": "订单已受理" }, { "CODE": 25, "NAME": "订单预审中" }, { "CODE": 30, "NAME": "文件处理中" }
    , { "CODE": 35, "NAME": "文件处理完成" }, { "CODE": 40, "NAME": "订单预审完成" }, { "CODE": 45, "NAME": "订单处理中" }
    , { "CODE": 50, "NAME": "制单中" }, { "CODE": 55, "NAME": "制单完成" }, { "CODE": 60, "NAME": "待审核" }
    , { "CODE": 65, "NAME": "审核已受理" }, { "CODE": 70, "NAME": "审核中(部分)" }, { "CODE": 71, "NAME": "审核中" }
    , { "CODE": 74, "NAME": "审核完成(部分)" }, { "CODE": 75, "NAME": "审核完成" }, { "CODE": 78, "NAME": "待预录" }
    , { "CODE": 80, "NAME": "预录已受理" }, { "CODE": 85, "NAME": "预录中(部分)" }, { "CODE": 86, "NAME": "预录中" }
    , { "CODE": 89, "NAME": "预录完成(部分)" }, { "CODE": 90, "NAME": "预录完成" }, { "CODE": 92, "NAME": "预录校验中" }
    , { "CODE": 93, "NAME": "预录校验完成(部分)" }, { "CODE": 95, "NAME": "预录校验完成" }, { "CODE": 100, "NAME": "申报中(部分)" }
    , { "CODE": 101, "NAME": "申报中" }, { "CODE": 102, "NAME": "申报退单" }, { "CODE": 103, "NAME": "提前申报完成" }
    , { "CODE": 104, "NAME": "申报完成(部分)" }, { "CODE": 105, "NAME": "申报完成" }, { "CODE": 106, "NAME": "申报完结(部分)" }
    , { "CODE": 107, "NAME": "申报完结" }, { "CODE": 110, "NAME": "订单已交付" }];

//查询的render方法
var orders_tatus = {
    '1': "草稿", '10': "文件已上传", '15': "订单已委托", '20': "订单已受理", '25': "订单预审中", '30': "文件处理中"
    , '35': "文件处理完成", '40': "订单预审完成", '45': "订单处理中", '50': "制单中", '55': "制单完成", '60': "待审核"
    , '65': "审核已受理", '70': "审核中(部分)", '71': "审核中", '73': '自审完成', '74': "审核完成(部分)", '75': "审核完成"
    , '78': "待预录", '80': "预录已受理", '85': "预录中(部分)", '86': "预录中", '89': "预录完成(部分)", '90': "预录完成"
    , '92': "预录校验中", '93': "预录校验完成(部分)", '95': "预录校验完成", '100': "申报中(部分)", '101': "申报中"
    , '102': "申报退单", '103': "提前申报完成", '104': "申报完成(部分)", '105': "申报完成", '106': "申报完结(部分)"
    , '107': "申报完结", '110': "订单已交付"
};

//业务类型静态数据
var common_data_busitype = [
    { "CODE": "10", "NAME": "空运出口" }, { "CODE": "11", "NAME": "空运进口" },
    { "CODE": "20", "NAME": "海运出口" }, { "CODE": "21", "NAME": "海运进口" },
    { "CODE": "30", "NAME": "陆运出口" }, { "CODE": "31", "NAME": "陆运进口" },
    { "CODE": "40", "NAME": "国内出口" }, { "CODE": "41", "NAME": "国内进口" },
    { "CODE": "50", "NAME": "特殊区域出口" }, { "CODE": "51", "NAME": "特殊区域进口" }];  