﻿//=======================================================JS init begin======================================================

//传参示例 /Common/DeclareList?busitypeid=11&module=ddzx&role=customer

var pgbar; var store_sbfs; var store_bgfs; var store_busitype; var store_modifyflag; var store_EXP;
var common_data_jydw = [], common_data_sbfs = [], common_data_bgfs = [], common_data_sbgq = [], socialcreditno;

Ext.onReady(function () {
    Ext.Ajax.request({
        url: "/Common/Ini_Base_Data",
        success: function (response, opts) {
            var commondata = Ext.decode(response.responseText)
            common_data_jydw = commondata.jydw;//申报单位  查询区要用
            common_data_sbfs = commondata.sbfs_all;//申报方式  这个基础数据调取出来的目的就是为了列表页字段显示时render处理
            common_data_bgfs = commondata.bgfs;//报关方式  这个基础数据调取出来的目的就是为了列表页字段显示时render处理
            common_data_sbgq = commondata.sbgq;//申报关区 进口口岸

            initSearch();  //查询区域

            store_sbfs = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_sbfs });
            store_bgfs = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_bgfs });
            store_busitype = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_busitype_Domestic });
            store_modifyflag = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: modifyflag_data });

            gridpanelBind();
        }
    })



});

//=======================================================JS init end=============================================================

function initSearch() {
    var declarationsearch_js_condition3_data_hg = [{ "NAME": "已结关", "CODE": "已结关" }, { "NAME": "未结关", "CODE": "未结关" }];
    var comboxsearch_js_condition = [{ "NAME": "申报单位", "CODE": "REPUNITCODE" }, { "NAME": "进出口岸", "CODE": "PORTCODE" }, { "NAME": "关联企业", "CODE": "BUSIUNITCODE_ASS" }];

    var store_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: comboxsearch_js_condition
    });
    var combo_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION1',
        name: "CONDITION1",
        value: "REPUNITCODE",
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
                   if (combo_1.getValue() == "REPUNITCODE") {
                       combo_1_1.minChars = "4";
                       store_1_1.loadData(common_data_jydw);
                   }
                   if (combo_1.getValue() == "PORTCODE") {
                       combo_1_1.minChars = "1";
                       store_1_1.loadData(common_data_sbgq);
                   }
                   if (combo_1.getValue() == "BUSIUNITCODE_ASS") {
                       combo_1_1.minChars = "4";
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
    var declarationsearch_js_condition2_data = [{ "NAME": "对应号", "CODE": "REPNO" }, { "NAME": "提运单号", "CODE": "BLNO" }
            , { "NAME": "订单编号", "CODE": "ORDERCODE" }, { "NAME": "报关单号", "CODE": "DECLNO" }
            , { "NAME": "合同协议号", "CODE": "CONTRACTNO" }, { "NAME": "运输工具名称", "CODE": "TRANSNAME" }, { "NAME": "合同发票号", "CODE": "CONTRACTNOORDER" }];
    
    var store_2 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: declarationsearch_js_condition2_data
    });
    var combo_2 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION2',
        name: "CONDITION2",
        store: store_2,
        value: "DECLNO",
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
        data: [{ "NAME": "业务类型", "CODE": "BUSITYPE" }, { "NAME": "海关状态", "CODE": "HGZT" }, { "NAME": "删改单", "CODE": "SGD" }, { "NAME": "比对状态", "CODE": "VERSTATUS" }]
    });
    var combo_3 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION3',
        name: "CONDITION3",
        value: "BUSITYPE",
        store: store_3,
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        margin: 0,
        flex: .35,
        listeners:
           {
               change: function () {
                   combo_3_1.reset();
                   if (this.getValue() == "BUSITYPE") {
                       store_3_1.loadData(common_data_busitype_Domestic);
                   }                  
                   if (this.getValue() == "HGZT") {
                       store_3_1.loadData(declarationsearch_js_condition3_data_hg);
                   }
                   if (this.getValue() == "SGD") {
                       store_3_1.loadData(modifyflag_data);
                   }
                   if (this.getValue() == "VERSTATUS") {
                       store_3_1.loadData(verstatus_data_search);
                   }
               }
           }
    })
    var store_3_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: common_data_busitype_Domestic
    });
    var combo_3_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION3_1',
        name: "NAME",
        margin: 0,
        store: store_3_1,
        displayField: 'NAME',
        valueField: "CODE",
        hideTrigger: false,
        anyMatch: true,
        queryMode: 'local',
        forceSelection: true,
        minChars: 1,
        flex: .65
    })
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
        items: [combo_4, date_4_1, date_4_2]
    }

    

//新增加

    var store_5 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: comboxsearch_js_condition
    });
    var combo_5 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION5',
        name: "CONDITION5",
        value: "PORTCODE",
        store: store_5,
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        margin: 0,
        flex: .35,
        listeners:
          {
              change: function () {
                  combo_5_1.reset();
                  if (this.getValue() == "REPUNITCODE") {
                      combo_5_1.minChars = "4";
                      store_5_1.loadData(common_data_jydw);
                  }
                  if (this.getValue() == "PORTCODE") {
                      combo_5_1.minChars = "1";
                      store_5_1.loadData(common_data_sbgq);
                  }
                  if (this.getValue() == "BUSIUNITCODE_ASS") {
                      combo_5_1.minChars = "4";
                      store_5_1.loadData(common_data_jydw);
                  }
              }
          }
    })
    var store_5_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: common_data_sbgq
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
        minChars: 4,
        flex: .65
    })
    var condition5 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_5, combo_5_1]
    }
    var store_6 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: declarationsearch_js_condition2_data
    });
    var combo_6 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION6',
        name: "CONDITION6",
        store: store_6,
        value: "CONTRACTNO",
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

    var store_7 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "业务类型", "CODE": "BUSITYPE" }, { "NAME": "海关状态", "CODE": "HGZT" }, { "NAME": "删改单", "CODE": "SGD" }, { "NAME": "比对状态", "CODE": "VERSTATUS" }]
    });
    var combo_7 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION7',
        name: "CONDITION7",
        value: "HGZT",
        store: store_7,
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        margin: 0,
        flex: .35,
        listeners:
           {
               change: function () {
                   combo_7_1.reset();
                   if (this.getValue() == "BUSITYPE") {
                       store_7_1.loadData(common_data_busitype_Domestic);
                   }
                   if (this.getValue() == "HGZT") {
                       store_7_1.loadData(declarationsearch_js_condition3_data_hg);
                   }
                   if (this.getValue() == "SGD") {
                       store_7_1.loadData(modifyflag_data);
                   }
                   if (this.getValue() == "VERSTATUS") {
                       store_7_1.loadData(verstatus_data_search);
                   }
               }
           }
    })
    var store_7_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: declarationsearch_js_condition3_data_hg
    });
    var combo_7_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION7_1',
        name: "NAME",
        margin: 0,
        store: store_7_1,
        displayField: 'NAME',
        valueField: "CODE",
        hideTrigger: false,
        anyMatch: true,
        queryMode: 'local',
        forceSelection: true,
        minChars: 1,
        flex: .65
    })
    var condition7 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_7, combo_7_1]
    }

    var store_8 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: declarationsearch_js_condition4_data
    });
    var combo_8 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION8',
        name: 'CONDITION8',
        value: 'REPTIME',
        margin: 0,
        flex: .35,
        store: store_8,
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
        items: [combo_8, date_8_1, date_8_2]
    }

    var formpanel = Ext.create('Ext.form.Panel', {
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.25
        },
        items: [
        { layout: 'column', border: 0, items: [condition1, condition2, condition3, condition4] },
        { layout: 'column', border: 0, items: [condition5, condition6, condition7, condition8] }

        ]
    });

}

function gridpanelBind() {
    var store = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'PREDECLCODE', 'DECLARATIONCODE', 'CUSTOMSSTATUS', 'CODE', 'MODIFYFLAG', 'REPTIME', 'TRANSNAME', 'BUSIUNITCODE', 'CUSTOMERNAME', 'IETYPE',
                'BUSIUNITNAME', 'PORTCODE', 'BLNO', 'REPWAYID', 'REPWAYNAME', 'DECLWAY', 'DECLWAYNAME', 'TRADEMETHOD', 'CONTRACTNO', 'GOODSNUM',
                'GOODSNW', 'GOODSGW', 'SHEETNUM', 'ORDERCODE', 'CUSNO', 'ASSOCIATENO', 'CORRESPONDNO', 'BUSITYPE', 'CONTRACTNOORDER', 'REPUNITNAME'
                , 'ORDERCODE_ASS', 'BUSIUNITCODE_ASS', 'BUSIUNITNAME_ASS', 'VERSTATUS', 'NOTE','INSPSTATUS', 'INSPECTIONCODE'],
        pageSize: 22,
        proxy: {
            type: 'ajax',
            url: '/Common/LoadDeclarationList_E_Domestic',
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
                    CONDITION1: Ext.getCmp('CONDITION1').getValue(), VALUE1: Ext.getCmp("CONDITION1_1").getValue(),
                    CONDITION2: Ext.getCmp('CONDITION2').getValue(), VALUE2: Ext.getCmp("CONDITION2_1").getValue(),
                    CONDITION3: Ext.getCmp('CONDITION3').getValue(), VALUE3: Ext.getCmp("CONDITION3_1").getValue(),
                    CONDITION4: Ext.getCmp('CONDITION4').getValue(),
                    VALUE4_1: Ext.Date.format(Ext.getCmp("CONDITION4_1").getValue(), 'Y-m-d H:i:s'),
                    VALUE4_2: Ext.Date.format(Ext.getCmp("CONDITION4_2").getValue(), 'Y-m-d H:i:s'),

                    CONDITION5: Ext.getCmp('CONDITION5').getValue(), VALUE5: Ext.getCmp("CONDITION5_1").getValue(),
                    CONDITION6: Ext.getCmp('CONDITION6').getValue(), VALUE6: Ext.getCmp("CONDITION6_1").getValue(),
                    CONDITION7: Ext.getCmp('CONDITION7').getValue(), VALUE7: Ext.getCmp("CONDITION7_1").getValue(),
                    CONDITION8: Ext.getCmp('CONDITION8').getValue(),
                    VALUE8_1: Ext.Date.format(Ext.getCmp("CONDITION8_1").getValue(), 'Y-m-d H:i:s'),
                    VALUE8_2: Ext.Date.format(Ext.getCmp("CONDITION8_2").getValue(), 'Y-m-d H:i:s')
                }
            },
            load: function () {
                socialcreditno = store.getProxy().getReader().rawData.socialcreditno;
                if (socialcreditno == "true") {
                    document.getElementById("btn_VerificationList").disabled = false;
                    Ext.getCmp('declare_grid').columns[2].setVisible(true);
                } else {
                    document.getElementById("btn_VerificationList").disabled = true;
                    Ext.getCmp('declare_grid').columns[2].setVisible(false);
                }
            }
        }
    });

    Ext.tip.QuickTipManager.init();
    pgbar = Ext.create('Ext.toolbar.Paging', {
        id: 'declare_grid_pgbar',
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store,
        displayInfo: true
    })

    var columns_old;
    
        columns_old = [
        { xtype: 'rownumberer', width: 35 },
        { header: 'ID', dataIndex: 'ID', sortable: true, hidden: true },
        { header: '海关状态', dataIndex: 'CUSTOMSSTATUS', width: 90, locked: true },
        
        {
            header: '比对状态', dataIndex: 'VERSTATUS', width: 90, locked: true, renderer: function (value, meta, record) {
                if (value == "比对未通过") {
                    meta.tdAttr = 'data-qtitle="<font color=red>未通过原因</font>" data-qtip="<font color=blue>' + record.get("NOTE") + '</font>"';
                }
                return value;
            }
        },
        { header: '报关单号', dataIndex: 'DECLARATIONCODE', width: 140, locked: true, renderer: render },
        { header: '报检单号', dataIndex: 'INSPECTIONCODE', width: 140, locked: true },
        { header: '报检状态', dataIndex: 'INSPSTATUS', width: 90, locked: true, renderer: renderinspstatus },
        { header: '申报单位', dataIndex: 'REPUNITNAME', width: 200 },
        { header: '合同发票号', dataIndex: 'CONTRACTNOORDER', width: 140, locked: true, },
        { header: '申报日期', dataIndex: 'REPTIME', width: 140 },
        { header: '运输工具名称', dataIndex: 'TRANSNAME', width: 150, renderer: render },
        { header: '业务类型', dataIndex: 'BUSITYPE', width: 90, renderer: render },// 业务类型
        { header: '进出口岸', dataIndex: 'PORTCODE', width: 80 },
        { header: '提运单号', dataIndex: 'BLNO', width: 180 },
        { header: '申报方式', dataIndex: 'REPWAYNAME', width: 100, renderer: render },
        { header: '报关方式', dataIndex: 'DECLWAYNAME', width: 100, renderer: render },
        { header: '贸易方式', dataIndex: 'TRADEMETHOD', width: 80 },
        { header: '合同协议号', dataIndex: 'CONTRACTNO', width: 110 },
        { header: '件数', dataIndex: 'GOODSNUM', width: 60 },
        { header: '重量', dataIndex: 'GOODSGW', width: 60 },
        { header: '张数', dataIndex: 'SHEETNUM', width: 60 },
        { header: '删改单', dataIndex: 'MODIFYFLAG', width: 60, renderer: render },
        { header: '订单编号', dataIndex: 'ORDERCODE', width: 100 },
        { header: '客户编号', dataIndex: 'CUSNO', width: 125 },
        { header: '关联企业', dataIndex: 'BUSIUNITNAME_ASS', width: 200 }
        ];
    

    var gridpanel = Ext.create('Ext.grid.Panel', {
        id: 'declare_grid',
        store: store,
        height: 535,
        renderTo: 'appConId',
        selModel: { selType: 'checkboxmodel' },
        bbar: pgbar,
        columns: columns_old,
        viewConfig: {
            enableTextSelection: true
        }
    })
}
//报检状态返回数值
function renderinspstatus(value, cellmeta, record, rowIndex, columnIndex, store) {
    var rtn = "";
    if (value == 50) {
        rtn = "审核完成";
    }
    if (value == 100) {
        rtn = "报检申报";
    }
    if (value == 115) {
        rtn = "申报退单";
    }
    if (value == 155) {
        rtn = "报检查验";
    } if (value == 160) {
        rtn = "报检放行";
    }
    return rtn;
}
function render(value, cellmeta, record, rowIndex, columnIndex, store) {
    var rtn = "";
    var dataindex = cellmeta.column.dataIndex;
    if (dataindex == "DECLARATIONCODE" && value) {
        rtn = "<div style='color:red;cursor:pointer; text-decoration:underline;' onclick='FileConsult(\"" + record.get("ORDERCODE") + "\",\"" + escape(record.get("BUSITYPE")) + "\",\"" + record.get("CODE") + "\")'>" + value + "</div>";
    }
    if (dataindex == "MODIFYFLAG") {
        var rec = store_modifyflag.findRecord('CODE', value);
        if (rec) {
            rtn = rec.get("NAME");
        }
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

//重置
function Reset() {
    Ext.getCmp("CONDITION1").setValue("REPUNITCODE");
    Ext.getCmp("CONDITION1_1").setValue("");
    Ext.getCmp("CONDITION2").setValue("DECLNO");
    Ext.getCmp("CONDITION2_1").setValue("");
    Ext.getCmp("CONDITION3").setValue("BUSITYPE");
    Ext.getCmp("CONDITION3_1").setValue("");
    Ext.getCmp("CONDITION4").setValue("SUBMITTIME");
    Ext.getCmp("CONDITION4_1").setValue("");
    Ext.getCmp("CONDITION4_2").setValue("");
    //
    Ext.getCmp("CONDITION5").setValue("PORTCODE");
    Ext.getCmp("CONDITION5_1").setValue("");
    Ext.getCmp("CONDITION6").setValue("CONTRACTNO");
    Ext.getCmp("CONDITION6_1").setValue("");
    Ext.getCmp("CONDITION7").setValue("HGZT");
    Ext.getCmp("CONDITION7_1").setValue("");
    Ext.getCmp("CONDITION8").setValue("REPTIME");
    Ext.getCmp("CONDITION8_1").setValue("");
    Ext.getCmp("CONDITION8_2").setValue("");
}

//打开调阅信息
function FileConsult(ORDERCODE, BUSITYPE, PREDECLCODE) {
    opencenterwin("/Common/FileConsult_E?menuxml=dec_domestic&source=declare&ORDERCODE=" + ORDERCODE + "&BUSITYPE=" + BUSITYPE + "&PREDECLCODE=" + PREDECLCODE, 1200, 900);
}

function ExportDecl() {
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据导出中，请稍等..." });
    myMask.show();

    var data = {
        common_data_busitype_Domestic: JSON.stringify(common_data_busitype_Domestic), modifyflag_data: JSON.stringify(modifyflag_data),
        CONDITION1: Ext.getCmp('CONDITION1').getValue(), VALUE1: Ext.getCmp("CONDITION1_1").getValue(),
        CONDITION2: Ext.getCmp('CONDITION2').getValue(), VALUE2: Ext.getCmp("CONDITION2_1").getValue(),
        CONDITION3: Ext.getCmp('CONDITION3').getValue(), VALUE3: Ext.getCmp("CONDITION3_1").getValue(),
        CONDITION4: Ext.getCmp('CONDITION4').getValue(),
        VALUE4_1: Ext.Date.format(Ext.getCmp("CONDITION4_1").getValue(), 'Y-m-d H:i:s'),
        VALUE4_2: Ext.Date.format(Ext.getCmp("CONDITION4_2").getValue(), 'Y-m-d H:i:s'),

        CONDITION5: Ext.getCmp('CONDITION5').getValue(), VALUE5: Ext.getCmp("CONDITION5_1").getValue(),
        CONDITION6: Ext.getCmp('CONDITION6').getValue(), VALUE6: Ext.getCmp("CONDITION6_1").getValue(),
        CONDITION7: Ext.getCmp('CONDITION7').getValue(), VALUE7: Ext.getCmp("CONDITION7_1").getValue(),
        CONDITION8: Ext.getCmp('CONDITION8').getValue(),
        VALUE8_1: Ext.Date.format(Ext.getCmp("CONDITION8_1").getValue(), 'Y-m-d H:i:s'),
        VALUE8_2: Ext.Date.format(Ext.getCmp("CONDITION8_2").getValue(), 'Y-m-d H:i:s')

    }

    Ext.Ajax.request({
        url: '/Common/ExportDeclList_E_Domestic',
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



function ExportDeclFile(arg,judge) {
    var recs
    if (arg == 'select') {
        recs = Ext.getCmp('declare_grid').getSelectionModel().getSelection();
        if (recs.length == 0) {
            Ext.MessageBox.alert('提示', '请选择需要导出的记录！');
            return;
        }
        //var codelist = Ext.encode(Ext.pluck(Ext.pluck(recs, 'data'), 'CODE'));
        var codelist = Ext.encode(Ext.pluck(recs, 'data'));

        var formtemp = new Ext.form.BasicForm(Ext.get('exportfileform'));
        formtemp.submit({
            waitTitle: '请稍后...',
            waitMsg: '正在下载,请稍后...',
            url: '/Common/ExportDeclFile',
            method: 'post',
            params: { codelist: codelist ,judge:judge},
            success: function (form, action) {
                window.location.href = url + action.result.url;
            },
            failure: function (form, action) {

                Ext.MessageBox.alert('提示', '下载失败，请确认文件是否确实存在！');
            }

        });
    }
    else {

        Ext.MessageBox.confirm("提示", "全部导出时可能因为文件过多而下载缓慢，确定导出吗？", function (btn) {
            if (btn == 'yes') {

                    var formtemp = new Ext.form.BasicForm(Ext.get('exportfileform'));
                    formtemp.submit({
                        timeout: 600000,
                        waitTitle: '请稍后...',
                        waitMsg: '正在下载,请稍后...',
                        url: '/Common/ExpDeclarationList_E_Domestic_all',
                        method: 'post',
                        params: {
                            CONDITION1: Ext.getCmp('CONDITION1').getValue(), VALUE1: Ext.getCmp("CONDITION1_1").getValue(),
                            CONDITION2: Ext.getCmp('CONDITION2').getValue(), VALUE2: Ext.getCmp("CONDITION2_1").getValue(),
                            CONDITION3: Ext.getCmp('CONDITION3').getValue(), VALUE3: Ext.getCmp("CONDITION3_1").getValue(),
                            CONDITION4: Ext.getCmp('CONDITION4').getValue(),
                            VALUE4_1: Ext.Date.format(Ext.getCmp("CONDITION4_1").getValue(), 'Y-m-d H:i:s'),
                            VALUE4_2: Ext.Date.format(Ext.getCmp("CONDITION4_2").getValue(), 'Y-m-d H:i:s'),

                            CONDITION5: Ext.getCmp('CONDITION5').getValue(), VALUE5: Ext.getCmp("CONDITION5_1").getValue(),
                            CONDITION6: Ext.getCmp('CONDITION6').getValue(), VALUE6: Ext.getCmp("CONDITION6_1").getValue(),
                            CONDITION7: Ext.getCmp('CONDITION7').getValue(), VALUE7: Ext.getCmp("CONDITION7_1").getValue(),
                            CONDITION8: Ext.getCmp('CONDITION8').getValue(),
                            VALUE8_1: Ext.Date.format(Ext.getCmp("CONDITION8_1").getValue(), 'Y-m-d H:i:s'),
                            VALUE8_2: Ext.Date.format(Ext.getCmp("CONDITION8_2").getValue(), 'Y-m-d H:i:s'),
                            judge:judge

                        },
                        success: function (form, action) {
                            window.location.href = url + action.result.url;
                        },
                        failure: function (form, action) {
                            Ext.MessageBox.alert('提示', '下载失败，请确认文件是否确实存在！');
                        }
                    });
             

            }
        });

    }



}

function openrwindow(url, width, height) {
    var iWidth = width ? width : "1000", iHeight = height ? height : "600";
    iWidth = iWidth > window.screen.availWidth ? window.screen.availWidth - 20 : iWidth
    iHeight = iHeight > window.screen.availHeight ? window.screen.availHeight - 120 : iHeight
    var iTop = (window.screen.availHeight - 30 - iHeight) / 2; //获得窗口的垂直位置;
    var iLeft = (window.screen.availWidth - 10 - iWidth) / 2; //获得窗口的水平位置; 
    window.open(url, '', 'height=' + iHeight + ',,innerHeight=' + iHeight + ',width=' + iWidth + ',innerWidth=' + iWidth + ',top=' + iTop + ',left=' + iLeft + ',location=yes,scrollbars=yes');
}
function Searchstatus() {
    var recs = Ext.getCmp('declare_grid').getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要维护的记录！');
        return;
    }
    var id = recs[0].data.ID
    var index = Ext.getCmp('declare_grid').store.indexOf(recs[0]);
    var currentPage = Ext.getCmp('declare_grid').store.currentPage
    openrwindow("/Common/ClearanceStatus?menuxml=ent_dec_cle_domestic&id=" + id + "&rowIndex=" + index + "&currentPage=" + currentPage, 1200, 800);
}

function VerificationList() {

    var recs = Ext.getCmp('declare_grid').getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要比对的记录！');
        return;
    }
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据保存中，请稍等..." });
    myMask.show();

    var declarationcode_list = Ext.encode(Ext.pluck(Ext.pluck(recs, 'data'), 'DECLARATIONCODE'));
    //var predeclcode_list = Ext.encode(Ext.pluck(Ext.pluck(recs, 'data'), 'CODE'));
    Ext.Ajax.request({
        url: '/Common/dec_Verification',
        params: { declarationcode_list: declarationcode_list },//, predeclcode_list: predeclcode_list
        success: function (response, option) {
            myMask.hide();
            var result = Ext.decode(response.responseText);
            if (result.success) {
                var json = result.json; var msg = "";
                if (json.length > 0) {
                    msg = "操作完成";
                } else {
                    msg = "保存成功";
                }

                Ext.Msg.alert('提示', msg, function () {
                    pgbar.moveFirst();
                    if (json.length > 0) {
                        errorwin(json);
                    }
                });
            }
            else {
                var result = Ext.decode(response.responseText);
                var errormsg = result.error;
                Ext.MessageBox.alert("提示", errormsg, function () {
                    pgbar.moveFirst();
                });
            }


        }
    });

}