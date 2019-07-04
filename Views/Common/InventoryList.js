//=======================================================JS init begin======================================================

var busitypeid = getQueryString_new("busitypeid"); var role = getQueryString_new("role");
var pgbar; var store_sbfs; var store_bgfs; var store_busitype; var store_modifyflag; 
var common_data_jydw = [], common_data_sbfs = [], common_data_bgfs = [], common_data_sbgq = [];
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
    var bl_title = "提运单号";
    if (busitypeid == "10" || busitypeid == "11")
        bl_title = "总分单号";
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
            common_data_sbgq = commondata.sbgq;//申报关区 进口口岸
            initSearch();  //查询区域
            Ext.getCmp('CONDITION2').setValue('CUSNO');
            store_sbfs = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_sbfs });
            store_bgfs = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_bgfs });
            store_busitype = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_busitype });
            store_modifyflag = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: modifyflag_data });

            var store = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE','VERIFYDECLFLAGNAME', 'INVENTORYIOCHECKSTATUSNAME', 'INVENTORYCODE', 'BUSIUNITNAME', 'REPSTARTTIME', 'PORTNAME',
                        'SUPERVISEMODENAME', 'DECLFLAGNAME', 'DECLTYPENAME', 'MODIFYFLAG', 'ORDERCODE', 'RELATIONBUSIUNITNAME',
                        'CUSNO', 'SECONDLADINGBILLNO', 'COMPANYINSIDENO', 'SUBMITTIME', 'BUSITYPE'],
                pageSize: 22,
                proxy: {
                    type: 'ajax',
                    url: '/Common/LoadInventoryList',
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
            });

            Ext.tip.QuickTipManager.init();
            pgbar = Ext.create('Ext.toolbar.Paging', {
                id: 'inventory_grid_pgbar',
                displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                store: store,
                displayInfo: true
            });
            var gridpanel = Ext.create('Ext.grid.Panel', {
                id: 'inventory_grid',
                store: store,
                height: 535,
                renderTo: 'appConId',
                selModel: { selType: 'checkboxmodel' },
                bbar: pgbar,
                columns: [
                { xtype: 'rownumberer', width: 45 },
                { header: 'ID', dataIndex: 'ID', sortable: true, hidden: true },
                { header: '业务类型', dataIndex: 'BUSITYPE', sortable: true, hidden: true },
                { header: '预制核注清单编号', dataIndex: 'CODE', sortable: true, hidden: true },
                { header: '核扣标志', dataIndex: 'VERIFYDECLFLAGNAME', width: 90, locked: true },
                { header: '进出卡扣标志', dataIndex: 'INVENTORYIOCHECKSTATUSNAME', width: 90, locked: true },
                { header: '清单编号', dataIndex: 'INVENTORYCODE', width: 140, renderer: render },
                { header: '经营单位', dataIndex: 'BUSIUNITNAME', width: 150 },
                { header: '合同号', dataIndex: 'COMPANYINSIDENO', width: 100 },
                { header: '打印标志', dataIndex: 'ISPRINT', width: 70 },
                { header: '申报日期', dataIndex: 'REPSTARTTIME', width: 140 },
                { header: '进出口岸', dataIndex: 'PORTNAME', width: 80 },
                { header: bl_title, dataIndex: 'SECONDLADINGBILLNO', width: 180 },
                { header: '监管方式', dataIndex: 'SUPERVISEMODENAME', width: 100},
                { header: '报关标志', dataIndex: 'DECLFLAGNAME', width: 100},
                { header: '报关类型', dataIndex: 'DECLTYPENAME', width: 80 },
                { header: '删改单', dataIndex: 'MODIFYFLAG', width: 60, renderer: render},
                { header: '订单编号', dataIndex: 'ORDERCODE', width: 100 },
                { header: '客户编号', dataIndex: 'CUSNO', width: 125 },
                { header: '关联收发货人', dataIndex: 'RELATIONBUSIUNITNAME', width: 125, hidden: (busitypeid != "50-51" && busitypeid != "40-41") }//海陆空不显示
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
        data: [{ "NAME": "经营单位", "CODE": "BUSIUNITCODE" }, { "NAME": "报关类型", "CODE": "DECLTYPE" }]
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
                   if (newValue == "BUSIUNITCODE") {
                       combo_1_1.minChars = 4;
                       store_1_1.loadData(common_data_jydw);
                   }
                   if (newValue == "DECLTYPE") {
                       combo_1_1.minChars = 1;
                       store_1_1.loadData(decltype_data);
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
    var declarationsearch_js_condition2_data = [{ "NAME": "订单编号", "CODE": "ORDERCODE" }, { "NAME": "客户编号", "CODE": "CUSNO" },
        { "NAME": "清单编号", "CODE": "INVENTORYCODE" }, { "NAME": "对应报关单号", "CODE": "CORRDECLFORMNO" }, { "预录入统一编号": "客户编号", "CODE": "PREUNITYCODE" }];

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


    var store_3 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "核扣标志", "CODE": "VERIFYDECLFLAG" }, { "NAME": "进出卡扣标志", "CODE": "INVENTORYIOCHECKSTATUS" },
            { "NAME": "报关标志", "CODE": "DECLFLAG" }, { "NAME": "删改单标志", "CODE": "MODIFYFLAG" }]
    });    

    var combo_3 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION3',
        name: "CONDITION3",
        value: "VERIFYDECLFLAG",
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
                    if (newValue == "VERIFYDECLFLAG") {
                        store_3_1.loadData(verifydeclflag_data);
                    }
                    if (newValue == "INVENTORYIOCHECKSTATUS") {
                        store_3_1.loadData(inventoryiocheckstatus_data);
                    }
                    if (newValue == "DECLFLAG") {
                        store_3_1.loadData(declflag_data);
                    }
                    if (newValue == "MODIFYFLAG") {
                        store_3_1.loadData(modifyflag_data);
                    }
                }
            }
    });
    var store_3_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: verifydeclflag_data
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
        //editable: false
        forceSelection: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                }
            }
        }
    });
    var condition3 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_3, combo_3_1]
    }
    var store_4 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "订单委托日期", "CODE": "SUBMITTIME" }, { "NAME": "申报日期", "CODE": "REPSTARTTIME" }]
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
        format: 'Y-m-d',
        listeners: {
            focus: function (cb) {
                var con = Ext.Date.format(Ext.getCmp("CONDITION4_2").getValue(), 'Y-m-d');
                if (con != "") {
                    var da_before = new Date(con);
                    da_before.setFullYear(da_before.getFullYear() - 1);
                    da_before.setDate(da_before.getDate() + 1);
                    var da = new Date(con);

                    cb.setMinValue(da_before); cb.setMaxValue(da);
                } else {
                    //cb.reset();//没用
                    cb.setMinValue(new Date("0000-01-01")); cb.setMaxValue(new Date("9999-12-31"));
                }
            }
        }
    })
    var date_4_2 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION4_2',
        margin: 0,
        flex: .325,
        emptyText: '结束日期',
        format: 'Y-m-d',
        listeners: {
            focus: function (cb) {
                var con = Ext.Date.format(Ext.getCmp("CONDITION4_1").getValue(), 'Y-m-d');
                if (con != "") {
                    var da_after = new Date(con);
                    da_after.setFullYear(da_after.getFullYear() + 1);
                    da_after.setDate(da_after.getDate() - 1);
                    var da = new Date(con);

                    cb.setMinValue(da); cb.setMaxValue(da_after);
                } else {
                    //cb.reset();//没用
                    cb.setMinValue(new Date("0000-01-01")); cb.setMaxValue(new Date("9999-12-31"));
                }
            }
        }
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
        value: "DECLTYPE",
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
                  if (newValue == "BUSIUNITCODE") {
                      combo_5_1.minChars = 4;
                      store_5_1.loadData(common_data_jydw);
                  }
                  if (newValue == "DECLTYPE") {
                      combo_5_1.minChars = 1;
                      store_5_1.loadData(decltype_data);
                  }
              }
          }
    })
    var store_5_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: decltype_data
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
        minChars: 1,
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
        value: "INVENTORYCODE",
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
        value: "INVENTORYIOCHECKSTATUS",
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
                    if (newValue == "VERIFYDECLFLAG") {
                        store_7_1.loadData(verifydeclflag_data);
                    }
                    if (newValue == "INVENTORYIOCHECKSTATUS") {
                        store_7_1.loadData(inventoryiocheckstatus_data);
                    }
                    if (newValue == "DECLFLAG") {
                        store_7_1.loadData(declflag_data);
                    }
                    if (newValue == "MODIFYFLAG") {
                        store_7_1.loadData(modifyflag_data);
                    }
                }
            }
    });
    var store_7_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: inventoryiocheckstatus_data
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
        //editable: false
        forceSelection: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                }
            }
        }
    });
    var condition7 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_7, combo_7_1]
    }

    var combo_8 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION8',
        name: 'CONDITION8',
        value: 'REPSTARTTIME',
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
        format: 'Y-m-d',
        listeners: {
            focus: function (cb) {
                var con = Ext.Date.format(Ext.getCmp("CONDITION8_2").getValue(), 'Y-m-d');
                if (con != "") {
                    var da_before = new Date(con);
                    da_before.setFullYear(da_before.getFullYear() - 1);
                    da_before.setDate(da_before.getDate() + 1);
                    var da = new Date(con);

                    cb.setMinValue(da_before); cb.setMaxValue(da);
                } else {
                    //cb.reset();//没用
                    cb.setMinValue(new Date("0000-01-01")); cb.setMaxValue(new Date("9999-12-31"));
                }
            }
        }
    })
    var date_8_2 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION8_2',
        margin: 0,
        flex: .325,
        emptyText: '结束日期',
        format: 'Y-m-d',
        listeners: {
            focus: function (cb) {
                var con = Ext.Date.format(Ext.getCmp("CONDITION8_1").getValue(), 'Y-m-d');
                if (con != "") {
                    var da_after = new Date(con);
                    da_after.setFullYear(da_after.getFullYear() + 1);
                    da_after.setDate(da_after.getDate() - 1);
                    var da = new Date(con);

                    cb.setMinValue(da); cb.setMaxValue(da_after);
                } else {
                    //cb.reset();//没用
                    cb.setMinValue(new Date("0000-01-01")); cb.setMaxValue(new Date("9999-12-31"));
                }
            }
        }
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

    //初始化时间控件
    var before = new Date();
    before.setDate(before.getDate() - 3);
    var beforeday = Ext.Date.format(before, 'm/d/Y');

    var today = Ext.Date.format(new Date(), 'm/d/Y');

    if (Ext.getCmp("CONDITION4").getValue() == "SUBMITTIME") {
        Ext.getCmp("CONDITION4_1").setValue(beforeday); Ext.getCmp("CONDITION4_2").setValue(today);
    }
    if (Ext.getCmp("CONDITION8").getValue() == "SUBMITTIME") {
        Ext.getCmp("CONDITION8_1").setValue(beforeday); Ext.getCmp("CONDITION8_2").setValue(today);
    }

}

//重置
function Reset() {
    Ext.getCmp("CONDITION1").setValue("BUSIUNITCODE");
    Ext.getCmp("CONDITION1_1").setValue("");
    Ext.getCmp("CONDITION2").setValue("ORDERCODE");
    Ext.getCmp("CONDITION2_1").setValue("");
    Ext.getCmp("CONDITION3").setValue("VERIFYDECLFLAG");
    Ext.getCmp("CONDITION3_1").setValue("");
    Ext.getCmp("CONDITION4").setValue("SUBMITTIME");
    Ext.getCmp("CONDITION4_1").setValue("");
    Ext.getCmp("CONDITION4_2").setValue("");

    Ext.getCmp("CONDITION5").setValue("DECLTYPE");
    Ext.getCmp("CONDITION5_1").setValue("");
    Ext.getCmp("CONDITION6").setValue("INVENTORYCODE");
    Ext.getCmp("CONDITION6_1").setValue("");
    Ext.getCmp("CONDITION7").setValue("INVENTORYIOCHECKSTATUS");
    Ext.getCmp("CONDITION7_1").setValue("");
    Ext.getCmp("CONDITION8").setValue("REPSTARTTIME");
    Ext.getCmp("CONDITION8_1").setValue("");
    Ext.getCmp("CONDITION8_2").setValue("");
}





//查询
function Select() {
    pgbar.moveFirst();
}

function render(value, cellmeta, record, rowIndex, columnIndex, store) {
    var rtn = "";
    var dataindex = cellmeta.column.dataIndex;
    if (dataindex == "INVENTORYCODE" && value) {
        rtn = "<div style='color:red;cursor:pointer; text-decoration:underline;' onclick='FileConsult(\"" + record.get("ORDERCODE") + "\",\"" + escape(record.get("BUSITYPE")) + "\",\"" + record.get("CODE") + "\")'>" + value + "</div>";
    }
    if (dataindex == "ISPRINT") {
        rtn = value == "0" ? "未打印" : "已打印";
    }
    if (dataindex == "MODIFYFLAG") {
        var rec = store_modifyflag.findRecord('CODE', value);
        if (rec) {
            rtn = rec.get("NAME");
        }
    }
   
  
    return rtn;
}

//打开调阅信息
function FileConsult(ORDERCODE, BUSITYPE, PREDECLCODE) {
    var menuxml = "";
    switch (BUSITYPE) {
        case "11":
            menuxml = "dec_airin";
            break;
        case "10":
            menuxml = "dec_airout";
            break;
        case "21":
            menuxml = "dec_seain";
            break;
        case "20":
            menuxml = "dec_seaout";
            break;
        case "31":
            menuxml = "dec_landin";
            break;
        case "30":
            menuxml = "dec_landout";
            break;
        case "40": case "41":
            menuxml = "dec_domestic";
            break;
        case "50": case "51":
            menuxml = "dec_special";
            break;
    }
    opencenterwin("/Common/FileConsult?menuxml=" + menuxml + "&source=declare&ORDERCODE=" + ORDERCODE + "&BUSITYPE=" + BUSITYPE + "&PREDECLCODE=" + PREDECLCODE, 1200, 900);
}

function ClickShowwinwj() {   //打开调阅信息
    var recs = Ext.getCmp("inventory_grid").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要调阅的记录！');
        return;
    }
    var menuxml = "";
    switch (recs[0].get("BUSITYPE")) {
        case "11":
            menuxml = "dec_airin";
            break;
        case "10":
            menuxml = "dec_airout";
            break;
        case "21":
            menuxml = "dec_seain";
            break;
        case "20":
            menuxml = "dec_seaout";
            break;
        case "31":
            menuxml = "dec_landin";
            break;
        case "30":
            menuxml = "dec_landout";
            break;
        case "40": case "41":
            menuxml = "dec_domestic";
            break;
        case "50": case "51":
            menuxml = "dec_special";
            break;
    }

    opencenterwin("/Common/FileConsult?menuxml=" + menuxml + "&source=declare&ORDERCODE=" + recs[0].get("ORDERCODE") + "&BUSITYPE=" + recs[0].get("BUSITYPE") + "&PREDECLCODE=" + recs[0].get("CODE"), 1200, 900);
}

function MultiPrint() {
    var recs = Ext.getCmp("inventory_grid").getSelectionModel().getSelection();
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

    var menuxml = "";
    switch (recs[0].get("BUSITYPE")) {
        case "11":
            menuxml = "invt_mul_airin";
            break;
        case "10":
            menuxml = "invt_mul_airout";
            break;
        case "21":
            menuxml = "invt_mul_seain";
            break;
        case "20":
            menuxml = "invt_mul_seaout";
            break;
        case "31":
            menuxml = "invt_mul_landin";
            break;
        case "30":
            menuxml = "invt_mul_landout";
            break;
        case "40": case "41":
            menuxml = "invt_mul_domestic";
            break;
        case "50": case "51":
            menuxml = "invt_mul_special";
            break;
    }

    opencenterwin("/Common/MultiPrint?menuxml=" + menuxml + "&source=invt&data=" + data, 1100, 700);
}

function openrwindow(url, width, height) {
    var iWidth = width ? width : "1000", iHeight = height ? height : "600";
    iWidth = iWidth > window.screen.availWidth ? window.screen.availWidth - 20 : iWidth
    iHeight = iHeight > window.screen.availHeight ? window.screen.availHeight - 120 : iHeight
    var iTop = (window.screen.availHeight - 30 - iHeight) / 2; //获得窗口的垂直位置;
    var iLeft = (window.screen.availWidth - 10 - iWidth) / 2; //获得窗口的水平位置; 
    window.open(url, '', 'height=' + iHeight + ',,innerHeight=' + iHeight + ',width=' + iWidth + ',innerWidth=' + iWidth + ',top=' + iTop + ',left=' + iLeft + ',location=yes,scrollbars=yes');
}

function ExportInvt() {

    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据导出中，请稍等..." });
    myMask.show();

    var data = {
        common_data_busitype: JSON.stringify(common_data_busitype), modifyflag_data: JSON.stringify(modifyflag_data),
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
        url: '/Common/ExportInvtList',
        params: data,
        success: function (response, option) {
            var json = Ext.decode(response.responseText);
            if (json.success == false) {
                if (json.WebDownCount==0) {
                    Ext.MessageBox.alert('提示', '记录为空！');
                }else{
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

