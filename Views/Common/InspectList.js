//=======================================================JS init begin======================================================
var busitypeid = getQueryString("busitypeid"); var role = getQueryString("role");
var pgbar; var store_busitype;//var store_mzbz, store_jylb, store_sbkb, store_inspbzzl;
var common_data_jydw = [];//var common_data_mzbz = [], common_data_jylb = [], common_data_sbkb = [], common_data_inspbzzl = [];

var busitype = "";
switch (busitypeid) {
    case "10":
        busitype = "空运出口"; break;
    case "11":
        busitype = "空运进口"; break;
    case "20":
        busitype = "海运出口"; break;
    case "21":
        busitype = "海运进口"; break;
    case "30":
        busitype = "陆运出口"; break;
    case "31":
        busitype = "陆运进口"; break;
    case "40-41":
        busitype = "国内"; break;
    case "50-51":
        busitype = "特殊区域"; break;
}
Ext.onReady(function () {

    Ext.Ajax.request({
        url: "/Common/Ini_Base_Data",
        params: {
            busitype: busitype
        },
        success: function (response, opts) {
            var commondata = Ext.decode(response.responseText)
            common_data_jydw = commondata.jydw;//经营单位  查询区要用
            //common_data_mzbz = commondata.mzbz;//木质包装
            //common_data_jylb = commondata.jylb;//检验类别
            //common_data_sbkb = commondata.sbkb;//申报库别
            //common_data_inspbzzl = commondata.inspbzzl;//报检包装种类

            initSearch();  //查询区域

            if (busitypeid == '50-51') {
                Ext.getCmp('CONDITION2').setValue('DECLNO');
            }
            else {
                Ext.getCmp('CONDITION2').setValue('CUSNO');
            }

            store_busitype = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_busitype });
            ////木质包装
            //store_mzbz = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_mzbz });
            ////检验类别
            //store_jylb = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_jylb });
            ////申报库别
            //store_sbkb = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_sbkb });
            ////报检包装种类
            //store_inspbzzl = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_inspbzzl });

            var store = Ext.create('Ext.data.JsonStore', {
                fields: ['ID', 'CODE', 'INSPSTATUS', 'ISPRINT', 'APPROVALCODE', 'INSPECTIONCODE', 'REPFINISHTIME',
                    'UPCNNAME', 'INSPTYPE', 'ENTRYPORT', 'TRANSTOOL', 'LADINGNO', 'TOTALNO', 'TRADE', 'CONTRACTNO', 'INSPUNITNAME', 'BUSITYPE',
                    'ISFORCELAW', 'WOODPACKINGID', 'GOODSNUM', 'PACKAGETYPE', 'DECLTYPE', 'ORDERCODE', 'CUSNO', 'FOBPORT', 'FOBPORTNAME'],
                pageSize: 22,
                proxy: {
                    type: 'ajax',
                    url: '/Common/LoadInspectionList',
                    reader: {
                        root: 'rows',
                        type: 'json',
                        totalProperty: 'total'
                    }
                },
                autoLoad: true,
                listeners: {
                    beforeload: function () {
                        store.getProxy().extraParams = {
                            busitypeid: busitypeid, role: role,
                            CONDITION1: Ext.getCmp('CONDITION1').getValue(), VALUE1: Ext.getCmp("CONDITION1_1").getValue(),
                            CONDITION2: Ext.getCmp('CONDITION2').getValue(), VALUE2: Ext.getCmp("CONDITION2_1").getValue(),
                            CONDITION3: Ext.getCmp('CONDITION3').getValue(), VALUE3: Ext.getCmp("CONDITION3_1").getValue(),
                            CONDITION4: Ext.getCmp('CONDITION4').getValue(), VALUE4_1: Ext.Date.format(Ext.getCmp("CONDITION4_1").getValue(), 'Y-m-d H:i:s'), VALUE4_2: Ext.Date.format(Ext.getCmp("CONDITION4_2").getValue(), 'Y-m-d H:i:s'),
                        }
                    }
                }
            })
            pgbar = Ext.create('Ext.toolbar.Paging', {
                displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                store: store,
                displayInfo: true
            })
            var col_array = [
                { xtype: 'rownumberer', width: 35 },
                { header: 'ID', dataIndex: 'ID', width: 110, sortable: true, hidden: true },
                { header: '国检状态', dataIndex: 'INSPSTATUS', width: 90, locked: true, renderer: render },
                { header: '打印标志', dataIndex: 'ISPRINT', width: 70, locked: true, renderer: render },
                { header: '核放单号', dataIndex: 'APPROVALCODE', width: 160, locked: true },
                { header: '报检单号', dataIndex: 'INSPECTIONCODE', width: 150, locked: true, renderer: render },
                //{ header: '申报完成时间', dataIndex: 'REPFINISHTIME', width: 130 },
                { header: '业务类型', dataIndex: 'BUSITYPE', width: 100, renderer: render },
                //{ header: '收货单位', dataIndex: 'UPCNNAME', width: 170 },
                //{ header: '检验类别', dataIndex: 'INSPTYPE', width: 100, renderer: render },
                //{ header: '入境口岸', dataIndex: 'ENTRYPORT', width: 80 },
                //{ header: '运输工具', dataIndex: 'TRANSNAME', width: 80 },
                //{ header: '提运单号', dataIndex: 'LADINGNO', width: 130 },
                //{ header: '贸易方式', dataIndex: 'TRADEWAYCODES', width: 80 },
                //{ header: '合同号', dataIndex: 'CONTRACTNO', width: 100 },
                //{ header: '报检单位', dataIndex: 'INSPUNITNAME', width: 200 },
                //{ header: '是否法检', dataIndex: 'ISFORCELAW', width: 60, renderer: render },
                //{ header: '木质包装', dataIndex: 'WOODPACKINGID', width: 80, renderer: render },
                //{ header: '包装数量', dataIndex: 'GOODSNUM', width: 80 },
                //{ header: '包装种类', dataIndex: 'PACKAGETYPE', width: 80, renderer: render },
                //{ header: '申报库别', dataIndex: 'DECLTYPE', width: 120, renderer: render },
                { header: '订单编号', dataIndex: 'ORDERCODE', width: 120 },
                { header: '客户编号', dataIndex: 'CUSNO', width: 130 }
            ]
            var gridpanel = Ext.create('Ext.grid.Panel', {
                id: 'inspect_grid',
                store: store,
                height: 535,
                selModel: { selType: 'checkboxmodel' },
                bbar: pgbar,
                renderTo: 'appConId',
                columns: col_array,
                viewConfig: {
                    enableTextSelection: true
                }
            })
        }
    });
})

//=======================================================JS init end======================================================

function initSearch() {
    var store_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "经营单位", "CODE": "BUSIUNITCODE" }]
    });
    var combo_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION1',
        name: "CONDITION1",
        value: "BUSIUNITCODE",
        store: store_1,
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        margin: 0,
        flex: .35
    })
    var store_1_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: common_data_jydw
    });
    var combo_1_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION1_1',
        name: "NAME",
        margin: 0,
        store: store_1_1,
        displayField: 'NAME',
        valueField: "CODE",
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        forceSelection: true,
        minChars: 4,
        flex: .65
    })
    var condition1 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_1, combo_1_1]
    }
    var declarationsearch_js_condition2_data = [{ "NAME": "客户编号", "CODE": "CUSNO" }, { "NAME": "提运单号", "CODE": "BLNO" }
            , { "NAME": "订单编号", "CODE": "ORDERCODE" }, { "NAME": "报关单号", "CODE": "DECLNO" }
            , { "NAME": "合同协议号", "CODE": "CONTRACTNO" }, { "NAME": "运输工具名称", "CODE": "TRANSNAME" }, { "NAME": "合同发票号", "CODE": "CONTRACTNOORDER" }];
    if (busitypeid == 10 || busitypeid == 30) {  //如果是空运出口，或者是陆运出口
        declarationsearch_js_condition2_data.push({ "NAME": "报关车号", "CODE": "DECLCARNO" });
    }
    var store_2 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: declarationsearch_js_condition2_data
    });
    var combo_2 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION2',
        name: "CONDITION2",
        store: store_2,
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        flex: .35,
        margin: 0,
    });
    var field_2_1 = Ext.create('Ext.form.field.Text', {
        id: 'CONDITION2_1',
        margin: 0,
        flex: .65
    })
    var condition2 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_2, field_2_1]
    }

    var declarationsearch_js_condition3_data_dy = [{ "NAME": "已打印", "CODE": "1" }, { "NAME": "未打印", "CODE": "0" }];

    var store_3 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "打印标志", "CODE": "DYBZ" }]
    });    

    var combo_3 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION3',
        name: "CONDITION3",
        value: "DYBZ",
        store: store_3,
        queryMode: 'local',
        editable: false,
        displayField: 'NAME',
        valueField: "CODE",
        margin: 0,
        flex: .35,
        listeners:
            {
                change: function (combo, newValue, oldValue, eOpts) {
                    combo_3_1.reset();
                    if (newValue == "DYBZ") {
                        store_3_1.loadData(declarationsearch_js_condition3_data_dy);
                    }
                }
            }
    });
    var store_3_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: declarationsearch_js_condition3_data_dy
    });
    var combo_3_1 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION3_1',
        store: store_3_1,
        displayField: 'NAME',
        valueField: "CODE",
        hideTrigger: true,
        margin: 0,
        flex: .65,
        anyMatch: true,
        queryMode: 'local',
        editable: false
    });
    var condition3 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_3, combo_3_1]
    }
    var store_4 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: declarationsearch_js_condition4_data
    });
    var combo_4 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION4',
        name: 'CONDITION4',
        value: 'SUBMITTIME',
        margin: 0,
        flex: .35,
        store: store_4,
        queryMode: 'local',
        displayField: 'NAME',
        valueField: "CODE",
        editable: false
    })
    var date_4_1 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION4_1',
        margin: 0,
        flex: .325,
        emptyText: '开始日期',
        format: 'Y-m-d'
    })
    var date_4_2 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION4_2',
        margin: 0,
        flex: .325,
        emptyText: '结束日期',
        format: 'Y-m-d'
    })
    var condition4 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: .34,
        items: [combo_4, date_4_1, date_4_2]
    }

    var formpanel = Ext.create('Ext.form.Panel', {
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.22
        },
        items: [
        { layout: 'column', border: 0, items: [condition1, condition2, condition3, condition4] }
        ]
    });

}

//重置
function Reset() {
    Ext.getCmp("CONDITION1").setValue("BUSIUNITCODE");
    Ext.getCmp("CONDITION1_1").setValue("");
    Ext.getCmp("CONDITION2").setValue("CUSNO");
    Ext.getCmp("CONDITION2_1").setValue("");
    Ext.getCmp("CONDITION3").setValue("DYBZ");
    Ext.getCmp("CONDITION3_1").setValue("");
    Ext.getCmp("CONDITION4").setValue("SUBMITTIME");
    Ext.getCmp("CONDITION4_1").setValue("");
    Ext.getCmp("CONDITION4_2").setValue("");
}


function render(value, cellmeta, record, rowIndex, columnIndex, store) {
    var rtn = "";
    var dataindex = cellmeta.column.dataIndex;
    if (dataindex == "INSPSTATUS" && value) {
        rtn = "<div style='color:red;cursor:pointer; text-decoration:underline;' onclick='showinspect_receipt(\"" + record.get("CODE") + "\")'>" + value + "</div>";
    }
    if (dataindex == "ISPRINT") {
        rtn = value == "0" ? "未打印" : "已打印";
    }
    if (dataindex == "INSPECTIONCODE" && value) {
        //rtn = "<div style='color:red;cursor:pointer; text-decoration:underline;' onclick='showwinwj(\"" + record.get("ID") + "\",\"" + record.get("ORDERCODE") + "\",\"" + record.get("CODE") + "\",TYPE=61)'>" + value + "</div>";
        rtn = "<div style='color:red;cursor:pointer; text-decoration:underline;' onclick='showwinwj(\"" + record.get("ORDERCODE") + "\",\"" + escape(record.get("BUSITYPE")) + "\",\"" + record.get("CODE") + "\")'>" + value + "</div>";
    }
    if (dataindex == "BUSITYPE" && value) {
        var rec = store_busitype.findRecord('CODE', value);
        if (rec) {
            rtn = rec.get("NAME");
        }
    }
    //if (dataindex == "WOODPACKINGID") {//木质包装
    //    var rec = store_mzbz.findRecord('CODE', value);
    //    if (rec) {
    //        rtn = rec.get("NAME");
    //    }
    //}
    //if (dataindex == "INSPTYPE") {//检验类别    
    //    var rec = store_jylb.findRecord('CODE', value);
    //    if (rec) {
    //        rtn = rec.get("NAME");
    //    }
    //}
    //if (dataindex == "PACKAGETYPE") {//包装种类
    //    var rec = store_inspbzzl.findRecord('CODE', value);
    //    if (rec) {
    //        rtn = rec.get("NAME");
    //    }
    //}
    //if (dataindex == "DECLTYPE") {//申报库别
    //    var rec = store_sbkb.findRecord('CODE', value);
    //    if (rec) {
    //        rtn = rec.get("NAME");
    //    }
    //}
    //if (dataindex == "ISFORCELAW") {
    //    rtn = value == "0" ? "否" : "是";
    //}      
    return rtn;
}

//查询
function Select() {
    pgbar.moveFirst();
}

//打开调阅信息
function showwinwj(ORDERCODE, BUSITYPE, PREINSPCODE) {
    //opencenterwin("/Common/FileConsult?ID=" + ID + "&ORDERCODE=" + ORDERCODE + "&DECLCODE=" + DECLCODE + "&TYPE=" + TYPE, 1200, 900);
    opencenterwin("/Common/FileConsult?source=inspect&ORDERCODE=" + ORDERCODE + "&BUSITYPE=" + BUSITYPE + "&PREINSPCODE=" + PREINSPCODE, 1200, 900);
}

//打开调阅信息
function ClickShowwinwj() {
    var recs = Ext.getCmp('inspect_grid').getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要调阅的记录！');
        return;
    }
    opencenterwin("/Common/FileConsult?source=inspect&ORDERCODE=" + recs[0].get("ORDERCODE") + "&BUSITYPE=" + recs[0].get("BUSITYPE") + "&PREINSPCODE=" + recs[0].get("CODE"), 1200, 900);
}

//显示国检状态回执记录
function showinspect_receipt(code_bjd) {
    var store_inspect = Ext.create('Ext.data.JsonStore', {
        fields: ['TIMES', 'STATUS'],
        proxy: {
            type: 'ajax',
            url: '/Common/LoadInspectReceipt?bjdcode=' + code_bjd,
            reader: {
                root: 'rows',
                type: 'json'
            }
        },
        autoLoad: true
    })
    var grid_inspect = Ext.create('Ext.grid.Panel', {
        title: '国检状态回执',
        store: store_inspect,
        height: 100,
        columns: [
            { xtype: 'rownumberer', width: 35 },
            { header: '时间', dataIndex: 'TIMES', width: 130 },
            { header: '国检状态', dataIndex: 'STATUS', flex: 1 }
        ]
    })
    var win_inspect_status = Ext.create("Ext.window.Window", {
        title: "",
        width: 400,
        height: 400,
        layout: "fit",
        modal: true,
        items: [grid_inspect]
    });
    win_inspect_status.show();
}

function MultiPrint() {
    var recs = Ext.getCmp("inspect_grid").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要打印的记录！');
        return;
    }
    var data = "[";
    for (var i = 0; i < recs.length; i++) {
        if (i == recs.length - 1) {
            data += "{BUSITYPE:'" + recs[i].get("BUSITYPE") + "',CODE:'" + recs[i].get("CODE") + "'}]"
        }
        else {
            data += "{BUSITYPE:'" + recs[i].get("BUSITYPE") + "',CODE:'" + recs[i].get("CODE") + "'},"
        }
    }
    opencenterwin("/Common/MultiPrint?source=inspect&data=" + data, 1100, 700);
}