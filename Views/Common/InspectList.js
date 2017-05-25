//=======================================================JS init begin======================================================
var busitypeid = getQueryString("busitypeid"); var role = getQueryString("role");
var pgbar; var store_busitype, store_insptradeway, store_modifyflag;
var common_data_jydw = [], common_data_inspmyfs = [];

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
            common_data_inspmyfs = commondata.inspmyfs;//贸易方式

            initSearch();  //查询区域

            store_busitype = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_busitype });
            store_insptradeway = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_inspmyfs });
            store_modifyflag = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: modifyflag_data });

            var store = Ext.create('Ext.data.JsonStore', {
                fields: ['ID', 'CODE', 'ORDERCODE', 'INSPSTATUS', 'ISPRINT', 'APPROVALCODE', 'INSPECTIONCODE', 'CLEARANCECODE'
                    , 'TRADEWAY', 'ISNEEDCLEARANCE', 'LAWFLAG', 'STATUS', 'MODIFYFLAG', 'SHEETNUM'
                    , 'BUSITYPE', 'CUSNO', 'SUBMITTIME', 'BUSIUNITCODE', 'BUSIUNITNAME', 'CONTRACTNO'],
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
                            CONDITION4: Ext.getCmp('CONDITION4').getValue(),
                            VALUE4_1: Ext.Date.format(Ext.getCmp("CONDITION4_1").getValue(), 'Y-m-d H:i:s'), VALUE4_2: Ext.Date.format(Ext.getCmp("CONDITION4_2").getValue(), 'Y-m-d H:i:s')
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
                { header: '国检状态', dataIndex: 'INSPSTATUS', width: 80, locked: true },
                { header: '报检状态', dataIndex: 'STATUS', width: 80, locked: true, renderer: render },
                { header: '核准单号', dataIndex: 'APPROVALCODE', width: 140, locked: true },
                { header: '报检单号', dataIndex: 'INSPECTIONCODE', width: 140, locked: true, renderer: render },
                { header: '经营单位', dataIndex: 'BUSIUNITNAME', width: 200, locked: role == 'customer' },
                { header: '通关单号', dataIndex: 'CLEARANCECODE', width: 140 },
                { header: '监管方式', dataIndex: 'TRADEWAY', width: 160, renderer: render },
                { header: '打印标志', dataIndex: 'ISPRINT', width: 70, renderer: render },
                { header: '张数', dataIndex: 'SHEETNUM', width: 70 },
                { header: '删改单', dataIndex: 'MODIFYFLAG', width: 70, renderer: render },
                { header: '业务类型', dataIndex: 'BUSITYPE', width: 100, renderer: render, hidden: (busitypeid != '40-41' && busitypeid != '50-51') },
                { header: '是否需通关单', dataIndex: 'ISNEEDCLEARANCE', width: 80, renderer: render },
                { header: '是否法检', dataIndex: 'LAWFLAG', width: 60, renderer: render },
                { header: '委托时间', dataIndex: 'SUBMITTIME', width: 130 },
                { header: '合同发票号', dataIndex: 'CONTRACTNO', width: 120 },
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
    var inspsearch_condition2_data = [{ "NAME": "客户编号", "CODE": "CUSNO" }, { "NAME": "订单编号", "CODE": "ORDERCODE" }
            , { "NAME": "报检单号", "CODE": "INSPECTIONCODE" }, { "NAME": "核准单号", "CODE": "APPROVALCODE" }
            , { "NAME": "通关单号", "CODE": "CLEARANCECODE" }, { "NAME": "合同发票号", "CODE": "CONTRACTNOORDER" }
    ];

    var store_2 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: inspsearch_condition2_data
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
        margin: 0, value: 'CUSNO'
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

    var insp_condition3_data_dy = [{ "NAME": "已打印", "CODE": "1" }, { "NAME": "未打印", "CODE": "0" }];
    var insp_condition3_data_bj = [{ "NAME": "已完结", "CODE": "1" }, { "NAME": "未完结", "CODE": "0" }];

    var store_3 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "打印标志", "CODE": "DYBZ" }, { "NAME": "报检状态", "CODE": "BJZT" }]
    });    

    var combo_3 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION3',
        name: "CONDITION3",
        value: "BJZT",
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
                        store_3_1.loadData(insp_condition3_data_dy);
                    }
                    if (newValue == "BJZT") {
                        store_3_1.loadData(insp_condition3_data_bj);
                    }
                }
            }
    });
    var store_3_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: insp_condition3_data_bj
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

    var inspsearch_condition4_data =[{ "NAME": "订单委托日期", "CODE": "SUBMITTIME" }];

    var store_4 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: inspsearch_condition4_data
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
    Ext.getCmp("CONDITION3").setValue("BJZT");
    Ext.getCmp("CONDITION3_1").setValue("");
    Ext.getCmp("CONDITION4").setValue("SUBMITTIME");
    Ext.getCmp("CONDITION4_1").setValue("");
    Ext.getCmp("CONDITION4_2").setValue("");
}


function render(value, cellmeta, record, rowIndex, columnIndex, store) {
    var rtn = "";
    var dataindex = cellmeta.column.dataIndex;
    if (dataindex == "ISPRINT") {
        rtn = value == "0" ? "未打印" : "已打印";
    }
    if (dataindex == "MODIFYFLAG" && value) {
        var rec = store_modifyflag.findRecord('CODE', value);
        if (rec) {
            rtn = rec.get("NAME");
        }
    }
    if (dataindex == "INSPECTIONCODE" && value) {
        rtn = "<div style='color:red;cursor:pointer; text-decoration:underline;' onclick='showwinwj(\"" + record.get("ORDERCODE") + "\",\"" + escape(record.get("BUSITYPE")) + "\",\"" + record.get("CODE") + "\")'>" + value + "</div>";
    }
    if (dataindex == "BUSITYPE" && value) {
        var rec = store_busitype.findRecord('CODE', value);
        if (rec) {
            rtn = rec.get("NAME");
        }
    }
    if (dataindex == "TRADEWAY" && value) {
        var rec = store_insptradeway.findRecord('CODE', value);
        if (rec) {
            rtn = rec.get("NAME");
        }
    }
    if (dataindex == "STATUS") {
        rtn = orders_tatus[value];
    }
    if (dataindex == "LAWFLAG" || dataindex == "ISNEEDCLEARANCE") {
        rtn = value == "0" ? "否" : "是";
    }
    return rtn;
}

//查询
function Select() {
    pgbar.moveFirst();
}

//打开调阅信息
function showwinwj(ORDERCODE, BUSITYPE, PREINSPCODE) {
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


function ExportInsp() {

    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据导出中，请稍等..." });
    myMask.show();

    var data = {
        dec_insp_status: JSON.stringify(orderstatus_js_data), common_data_busitype: JSON.stringify(common_data_busitype),
        common_data_inspmyfs: JSON.stringify(common_data_inspmyfs), modifyflag_data: JSON.stringify(modifyflag_data),
        busitypeid: busitypeid, role: role,
        CONDITION1: Ext.getCmp('CONDITION1').getValue(), VALUE1: Ext.getCmp("CONDITION1_1").getValue(),
        CONDITION2: Ext.getCmp('CONDITION2').getValue(), VALUE2: Ext.getCmp("CONDITION2_1").getValue(),
        CONDITION3: Ext.getCmp('CONDITION3').getValue(), VALUE3: Ext.getCmp("CONDITION3_1").getValue(),
        CONDITION4: Ext.getCmp('CONDITION4').getValue(),
        VALUE4_1: Ext.Date.format(Ext.getCmp("CONDITION4_1").getValue(), 'Y-m-d H:i:s'), VALUE4_2: Ext.Date.format(Ext.getCmp("CONDITION4_2").getValue(), 'Y-m-d H:i:s')
    }

    Ext.Ajax.request({
        url: '/Common/ExportInspList',
        params: data,
        success: function (response, option) {
            var json = Ext.decode(response.responseText);
            if (json.success == false) {
                if (json.WebDownCount == 0) {
                    Ext.MessageBox.alert('提示', '记录为空！');
                } else {
                    Ext.MessageBox.alert('提示', '综合需求及性能，导出记录限制' + json.WebDownCount + '！');
                }
            } else {
                Ext.Ajax.request({
                    url: '/Common/DownloadFile',
                    method: 'POST',
                    params: Ext.decode(response.responseText),
                    form: 'exportform',
                    success: function (response, option) {
                    }
                });
            }
            myMask.hide();
        }
    });
}