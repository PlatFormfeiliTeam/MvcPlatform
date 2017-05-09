﻿//=======================================================JS init begin======================================================

//传参示例 /Common/DeclareList?busitypeid=11&role=customer   
var busitypeid = getQueryString("busitypeid"); var role = getQueryString("role");
var pgbar; var store_sbfs; var store_bgfs; var store_busitype;
var common_data_jydw = [], common_data_sbfs = [], common_data_bgfs = [];
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
            common_data_sbfs = commondata.sbfs;//申报方式  这个基础数据调取出来的目的就是为了列表页字段显示时render处理
            common_data_bgfs = commondata.bgfs;//报关方式  这个基础数据调取出来的目的就是为了列表页字段显示时render处理

            initSearch();  //查询区域

            if (busitypeid == '50-51') {
                Ext.getCmp('CONDITION2').setValue('DECLNO');
            }
            else {
                Ext.getCmp('CONDITION2').setValue('CUSNO');
            }
            store_sbfs = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_sbfs
            });
            store_bgfs = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_bgfs
            });
            store_busitype = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_busitype
            });
            var store = Ext.create('Ext.data.JsonStore', {
                fields: ['ID','CODE', 'ORDERCODE', 'CUSTOMSSTATUS', 'ISPRINT', 'SHEETNUM',
                        'DECLARATIONCODE', 'REPTIME', 'CONTRACTNO', 'GOODSNUM', 'GOODSNW', 'BLNO', 'TRANSNAME', 'VOYAGENO',
                        'BUSIUNITCODE', 'BUSIUNITNAME', 'PORTCODE', 'TRADEMETHOD', 'DECLWAY', 'DECLWAYNAME',
                        'BUSITYPE', 'CONTRACTNOORDER', 'REPWAYID', 'REPWAYNAME', 'TOTALNO', 'DIVIDENO','SECONDLADINGBILLNO',
                        'CUSNO', 'IETYPE', 'ASSOCIATENO', 'CORRESPONDNO', 'CUSTOMERNAME'],
                pageSize: 22,
                proxy: {
                    type: 'ajax',
                    url: '/Common/LoadDeclarationList',
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
                            CONDITION5: Ext.getCmp('CONDITION5').getValue(), VALUE5: Ext.getCmp("CONDITION5_1").getValue(),
                            CONDITION6: Ext.getCmp('CONDITION6').getValue(), VALUE6: Ext.getCmp("CONDITION6_1").getValue(),
                            CONDITION7: Ext.getCmp('CONDITION7').getValue(), VALUE7: Ext.getCmp("CONDITION7_1").getValue(),
                            CONDITION8: Ext.getCmp('CONDITION8').getValue(), VALUE8_1: Ext.Date.format(Ext.getCmp("CONDITION8_1").getValue(), 'Y-m-d H:i:s'), VALUE8_2: Ext.Date.format(Ext.getCmp("CONDITION8_2").getValue(), 'Y-m-d H:i:s')
                        }
                    }
                }
            })
            pgbar = Ext.create('Ext.toolbar.Paging', {
                displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                store: store,
                displayInfo: true
            })
            var gridpanel = Ext.create('Ext.grid.Panel', {
                id: 'declare_grid',
                store: store,
                height: 535,
                renderTo: 'appConId',
                selModel: { selType: 'checkboxmodel' },
                bbar: pgbar,
                columns: [
                { xtype: 'rownumberer', width: 45 },
                { header: 'ID', dataIndex: 'ID', sortable: true, hidden: true },
                { header: '海关状态', dataIndex: 'CUSTOMSSTATUS', width: 90, locked: true },
                { header: '合同发票号', dataIndex: 'CONTRACTNOORDER', width: 140 },
                { header: '总单号', dataIndex: 'TOTALNO', width: 100, hidden: (busitypeid != '10' && busitypeid != '11')},
                { header: '分单号', dataIndex: 'DIVIDENO', width: 100, hidden: (busitypeid != '10' && busitypeid != '11') },
                { header: '海运提单号', dataIndex: 'SECONDLADINGBILLNO', width: 140, hidden: (busitypeid != '20' && busitypeid != '21') },
                { header: '打印标志', dataIndex: 'ISPRINT', width: 70, renderer: render },
                { header: '报关单号', dataIndex: 'DECLARATIONCODE', width: 140, locked: true, renderer: render },
                { header: '委托单位', dataIndex: 'CUSTOMERNAME', width: 100, locked: role == 'supplier', hidden: role == 'customer' },
                { header: '申报日期', dataIndex: 'REPTIME', width: 140 },
                { header: '进/出', dataIndex: 'IETYPE', width: 80, hidden: busitypeid != '40-41' },
                { header: '两单关联号', dataIndex: 'ASSOCIATENO', width: 110, hidden: busitypeid != '40-41' },//两单关联号
                { header: '运输工具名称', dataIndex: 'TRANSNAME', width: 150, renderer: render },
                { header: '业务类型', dataIndex: 'BUSITYPE', width: 90, renderer: render },// 业务类型
                { header: '出口口岸', dataIndex: 'PORTCODE', width: 80 },
                { header: '提运单号', dataIndex: 'BLNO', width: 180 },
                { header: '申报方式', dataIndex: 'REPWAYNAME', width: 100, renderer: render },
                { header: '报关方式', dataIndex: 'DECLWAYNAME', width: 100, renderer: render },
                { header: '贸易方式', dataIndex: 'TRADEMETHOD', width: 80 },
                { header: '合同协议号', dataIndex: 'CONTRACTNO', width: 110 },
                { header: '件数', dataIndex: 'GOODSNUM', width: 60 },
                { header: '重量', dataIndex: 'GOODSNW', width: 60 },
                { header: '张数', dataIndex: 'SHEETNUM', width: 60 },
                { header: '多单关联号', dataIndex: 'CORRESPONDNO', width: 100, hidden: busitypeid != '40-41' },//多单关联号
                { header: '订单编号', dataIndex: 'ORDERCODE', width: 100 },
                { header: '经营单位', dataIndex: 'BUSIUNITNAME', width: 140, locked: role == 'customer' },
                { header: '客户编号', dataIndex: 'CUSNO', width: 125 }
                ],
                viewConfig: {
                    enableTextSelection: true
                }
            })
        }
    })
});

//=======================================================JS init end======================================================

function initSearch() {
    var store_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "经营单位", "CODE": "BUSIUNITCODE"},{"NAME": "申报方式", "CODE": "REPWAYNAME" }]
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
        flex: .35,
        listeners:
           {
               change: function (combo, newValue, oldValue, eOpts) {
                   combo_1_1.reset();
                   if (newValue == "REPWAYNAME") {
                       combo_1_1.minChars = 2;
                       store_1_1.loadData(common_data_sbfs);
                   }
                   if (newValue == "BUSIUNITCODE") {
                       combo_1_1.minChars = 4;
                       store_1_1.loadData(common_data_jydw);
                   }
               }
           }
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
    if (busitypeid == 10 || busitypeid == 11) {
        declarationsearch_js_condition2_data.push({ "NAME": "总单号", "CODE": "TOTALNO" });
        declarationsearch_js_condition2_data.push({ "NAME": "分单号", "CODE": "DIVIDENO" });
    }
    if (busitypeid == 20 || busitypeid == 21) {
        declarationsearch_js_condition2_data.push({ "NAME": "海关提单号", "CODE": "SECONDLADINGBILLNO" });
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
    var declarationsearch_js_condition3_data_hg = [{ "NAME": "已结关", "CODE": "已结关" }, { "NAME": "未结关", "CODE": "未结关" }];

    var store_3 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "打印标志", "CODE": "DYBZ" }, { "NAME": "海关状态", "CODE": "HGZT" }]
    });    

    var combo_3 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION3',
        name: "CONDITION3",
        value: "HGZT",
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
                    if (newValue == "HGZT") {
                        store_3_1.loadData(declarationsearch_js_condition3_data_hg);
                    }
                    if (newValue == "DYBZ") {
                        store_3_1.loadData(declarationsearch_js_condition3_data_dy);
                    }
                }
            }
    });
    var store_3_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: declarationsearch_js_condition3_data_hg
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




    var combo_5 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION5',
        name: "CONDITION5",
        value: "REPWAYNAME",
        store: store_1,
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        margin: 0,
        flex: .35,
        listeners:
          {
              change: function (combo, newValue, oldValue, eOpts) {
                  combo_5_1.reset();
                  if (newValue == "REPWAYNAME") {
                      combo_5_1.minChars = 2;
                      store_5_1.loadData(common_data_sbfs);
                  }
                  if (newValue == "BUSIUNITCODE") {
                      combo_5_1.minChars = 4;
                      store_5_1.loadData(common_data_jydw);
                  }
              }
          }
    })
    var store_5_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: common_data_sbfs
    });
    var combo_5_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION5_1',
        name: "NAME",
        margin: 0,
        store: store_5_1,
        displayField: 'NAME',
        valueField: "CODE",
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        forceSelection: true,
        minChars: 2,
        flex: .65
    })
    var condition5 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_5, combo_5_1]
    }

    var combo_6 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION6',
        name: "CONDITION6",
        store: store_2,
        value: "BLNO",
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        flex: .35,
        margin: 0,
    });
    var field_6_1 = Ext.create('Ext.form.field.Text', {
        id: 'CONDITION6_1',
        margin: 0,
        flex: .65
    })
    var condition6 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_6, field_6_1]
    }

    var combo_7 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION7',
        name: "CONDITION7",
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
                    combo_7_1.reset();
                    if (newValue == "HGZT") {
                        store_7_1.loadData(declarationsearch_js_condition3_data_hg);
                    }
                    if (newValue == "DYBZ") {
                        store_7_1.loadData(declarationsearch_js_condition3_data_dy);
                    }
                }
            }
    });
    var store_7_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: declarationsearch_js_condition3_data_dy
    });
    var combo_7_1 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION7_1',
        store: store_7_1,
        displayField: 'NAME',
        valueField: "CODE",
        hideTrigger: true,
        margin: 0,
        flex: .65,
        anyMatch: true,
        queryMode: 'local',
        editable: false
    });
    var condition7 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_7, combo_7_1]
    }

    var combo_8 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION8',
        name: 'CONDITION8',
        value: 'REPTIME',
        margin: 0,
        flex: .35,
        store: store_4,
        queryMode: 'local',
        displayField: 'NAME',
        valueField: "CODE",
        editable: false
    })
    var date_8_1 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION8_1',
        margin: 0,
        flex: .325,
        emptyText: '开始日期',
        format: 'Y-m-d'
    })
    var date_8_2 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION8_2',
        margin: 0,
        flex: .325,
        emptyText: '结束日期',
        format: 'Y-m-d'
    })
    var condition8 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: .34,
        items: [combo_8, date_8_1, date_8_2]
    }

    var formpanel = Ext.create('Ext.form.Panel', {
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.22
        },
        items: [
        { layout: 'column', border: 0, items: [condition1, condition2, condition3, condition4] },
         { layout: 'column', border: 0, items: [condition5, condition6, condition7, condition8] }
        ]
    });

}

//重置
function Reset() {
    Ext.getCmp("CONDITION1").setValue("BUSIUNITCODE");
    Ext.getCmp("CONDITION1_1").setValue("");
    Ext.getCmp("CONDITION2").setValue("CUSNO");
    Ext.getCmp("CONDITION2_1").setValue("");
    Ext.getCmp("CONDITION3").setValue("HGZT");
    Ext.getCmp("CONDITION3_1").setValue("");
    Ext.getCmp("CONDITION4").setValue("SUBMITTIME");
    Ext.getCmp("CONDITION4_1").setValue("");
    Ext.getCmp("CONDITION4_2").setValue("");

    Ext.getCmp("CONDITION5").setValue("REPWAYNAME");
    Ext.getCmp("CONDITION5_1").setValue("");
    Ext.getCmp("CONDITION6").setValue("BLNO");
    Ext.getCmp("CONDITION6_1").setValue("");
    Ext.getCmp("CONDITION7").setValue("DYBZ");
    Ext.getCmp("CONDITION7_1").setValue("");
    Ext.getCmp("CONDITION8").setValue("REPTIME");
    Ext.getCmp("CONDITION8_1").setValue("");
    Ext.getCmp("CONDITION8_2").setValue("");
}


function render(value, cellmeta, record, rowIndex, columnIndex, store) {
    var rtn = "";
    var dataindex = cellmeta.column.dataIndex;
    if (dataindex == "DECLARATIONCODE" && value) {
        rtn = "<div style='color:red;cursor:pointer; text-decoration:underline;' onclick='FileConsult(\"" + record.get("ORDERCODE") + "\",\"" + escape(record.get("BUSITYPE")) + "\",\"" + record.get("CODE") + "\")'>" + value + "</div>";
    }
    if (dataindex == "ISPRINT") {
        rtn = value == "0" ? "未打印" : "已打印";
    }
    if (dataindex == "REPWAYNAME" && value) {
        var rec = store_sbfs.findRecord('CODE', value);
        if (rec) {
            rtn = rec.get("NAME");
        }
    }
    if (dataindex == "DECLWAYNAME" && value) {
        var rec = store_bgfs.findRecord('CODE', value);
        if (rec) {
            rtn = rec.get("NAME");
        }
    }
    if (dataindex == "BUSITYPE" && value) {
        var rec = store_busitype.findRecord('CODE', value);
        if (rec) {
            rtn = rec.get("NAME");
        }
    }
    if (dataindex == "TRANSNAME") {
        var VOYAGENO = record.get("VOYAGENO");

        if (value == null && VOYAGENO == null) {
            rtn = '';
        }
        if (value == null && VOYAGENO != null) {
            rtn = '/' + VOYAGENO;
        }
        if (value != null && VOYAGENO == null) {
            rtn = value;
        }
        if (value != null && VOYAGENO != null) {
            rtn = value + '/' + VOYAGENO;
        }
    }
    return rtn;
}

//查询
function Select() {
    pgbar.moveFirst();
}

//打开调阅信息
function FileConsult(ORDERCODE, BUSITYPE, PREDECLCODE) {
    opencenterwin("/Common/FileConsult?source=declare&ORDERCODE=" + ORDERCODE + "&BUSITYPE=" + BUSITYPE + "&PREDECLCODE=" + PREDECLCODE, 1200, 900);
}

function ClickShowwinwj() {   //打开调阅信息
    var recs = Ext.getCmp("declare_grid").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要调阅的记录！');
        return;
    }
    opencenterwin("/Common/FileConsult?source=declare&ORDERCODE=" + recs[0].get("ORDERCODE") + "&BUSITYPE=" + recs[0].get("BUSITYPE") + "&PREDECLCODE=" + recs[0].get("CODE"), 1200, 900);
}

function MultiPrint() {
    var recs = Ext.getCmp("declare_grid").getSelectionModel().getSelection();
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
    opencenterwin("/Common/MultiPrint?source=declare&data=" + data, 1100, 700);
}




function ExportDecl() {

    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据导出中，请稍等..." });
    myMask.show();

    var data = {
        common_data_busitype: JSON.stringify(common_data_busitype),
        busitypeid: busitypeid, role: role,
        CONDITION1: Ext.getCmp('CONDITION1').getValue(), VALUE1: Ext.getCmp("CONDITION1_1").getValue(),
        CONDITION2: Ext.getCmp('CONDITION2').getValue(), VALUE2: Ext.getCmp("CONDITION2_1").getValue(),
        CONDITION3: Ext.getCmp('CONDITION3').getValue(), VALUE3: Ext.getCmp("CONDITION3_1").getValue(),
        CONDITION4: Ext.getCmp('CONDITION4').getValue(), VALUE4_1: Ext.Date.format(Ext.getCmp("CONDITION4_1").getValue(), 'Y-m-d H:i:s'), VALUE4_2: Ext.Date.format(Ext.getCmp("CONDITION4_2").getValue(), 'Y-m-d H:i:s'),
        CONDITION5: Ext.getCmp('CONDITION5').getValue(), VALUE5: Ext.getCmp("CONDITION5_1").getValue(),
        CONDITION6: Ext.getCmp('CONDITION6').getValue(), VALUE6: Ext.getCmp("CONDITION6_1").getValue(),
        CONDITION7: Ext.getCmp('CONDITION7').getValue(), VALUE7: Ext.getCmp("CONDITION7_1").getValue(),
        CONDITION8: Ext.getCmp('CONDITION8').getValue(), VALUE8_1: Ext.Date.format(Ext.getCmp("CONDITION8_1").getValue(), 'Y-m-d H:i:s'), VALUE8_2: Ext.Date.format(Ext.getCmp("CONDITION8_2").getValue(), 'Y-m-d H:i:s')

    }

    Ext.Ajax.request({
        url: '/Common/ExportDeclList',
        params: data,
        success: function (response, option) {
            var json = Ext.decode(response.responseText);
            if (json.success == false) {
                Ext.MessageBox.alert('提示', '综合需求及性能，导出记录限制' + json.WebDownCount + '！');
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