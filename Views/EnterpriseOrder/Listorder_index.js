//=======================================================JS init begin======================================================
var busitypeid;
var store_busitype; var store_sbfs;
var win_logistic;
Ext.onReady(function () {
    busitypeid = getQueryString_new("busitypeid");
    var busitype = "";
    var columns_order = [];
    switch (busitypeid) {
        case "10":
            busitype = "空运出口";
            columns_order = [{ xtype: 'rownumberer', width: 35 },
              { header: 'ID', dataIndex: 'ID', hidden: true, locked: true },
              { header: '报关状态', dataIndex: 'DECLSTATUS', width: 90, renderer: renderOrder, locked: true },
              { header: '报检状态', dataIndex: 'INSPSTATUS', width: 90, renderer: renderOrder, locked: true },
              { header: '合同号', dataIndex: 'CONTRACTNO', width: 100, locked: true },
              { header: '总单号', dataIndex: 'TOTALNO', width: 90 },//需要确定具体字段
              { header: '分单号', dataIndex: 'DIVIDENO', width: 90 },//需要确定具体字段
              { header: '件数/重量', dataIndex: 'GOODSNUM', width: 65, renderer: renderOrder },//该字段需要拼接
              { header: '申报关区', dataIndex: 'CUSTOMAREACODE', width: 60 },//需要显示编码
              { header: '进/出口岸', dataIndex: 'PORTCODE', width: 70 },//需要显示编码
              { header: '申报方式', dataIndex: 'REPWAYID', width: 100, renderer: renderOrder },
              { header: '运抵编号', dataIndex: 'ARRIVEDNO', width: 140 },//需要确定具体字段
              { header: '法检', dataIndex: 'LAWFLAG', width: 60, renderer: renderOrder },
              { header: '受理时间', dataIndex: 'SUBMITTIME', width: 130, locked: true },
              { header: '订单编号', dataIndex: 'CODE' },
              { header: '委托人员', dataIndex: 'SUBMITUSERNAME', width: 80, locked: true },
              { header: '报关申报单位', dataIndex: 'REPUNITNAME', width: 180, locked: true }];
            break;
        case "11":
            busitype = "空运进口";
            columns_order = [{ xtype: 'rownumberer', width: 35 },
                { header: 'ID', dataIndex: 'ID', hidden: true, locked: true },
                { header: '报关状态', dataIndex: 'DECLSTATUS', width: 70, renderer: renderOrder, locked: true },
                { header: '报检状态', dataIndex: 'INSPSTATUS', width: 70, renderer: renderOrder, locked: true },
                { header: '物流状态', dataIndex: 'LOGISTICSNAME', width: 80, renderer: renderLogistic, locked: true },
                { header: '对应号', dataIndex: 'REPNO', width: 100, locked: true },
                { header: '合同发票号', dataIndex: 'CONTRACTNO', width: 100, locked: true },
                { header: '件数/重量', dataIndex: 'GOODSNUM', width: 65, renderer: renderOrder, locked: true },//该字段需要拼接
                { header: '总单号', dataIndex: 'TOTALNO', width: 90 },//需要确定具体字段
                { header: '分单号', dataIndex: 'DIVIDENO', width: 90 },//需要确定具体字段
                { header: '申报关区', dataIndex: 'CUSTOMAREACODE', width: 70 },//需要显示编码
                { header: '进/出口岸', dataIndex: 'PORTCODE', width: 70 },//需要显示编码
                { header: '申报方式', dataIndex: 'REPWAYID', width: 100, renderer: renderOrder },
                { header: '转关预录号', dataIndex: 'TURNPRENO', width: 130 },//需要确定具体字段
                { header: '法检', dataIndex: 'LAWFLAG', width: 60, renderer: renderOrder },
                { header: '受理时间', dataIndex: 'SUBMITTIME', width: 130, locked: true },
                { header: '订单编号', dataIndex: 'CODE', width: 100 },
                { header: '委托人员', dataIndex: 'SUBMITUSERNAME', width: 70, locked: true },
                { header: '报关申报单位', dataIndex: 'REPUNITNAME', width: 180, locked: true }];
            break;
        case "20":
            busitype = "海运出口";
            columns_order = [{ xtype: 'rownumberer', width: 35 },
                { header: 'ID', dataIndex: 'ID', hidden: true, locked: true },
                { header: '报关状态', dataIndex: 'DECLSTATUS', width: 80, renderer: renderOrder, locked: true },
                { header: '报检状态', dataIndex: 'INSPSTATUS', width: 80, renderer: renderOrder, locked: true },
                { header: '合同号', dataIndex: 'CONTRACTNO', width: 140, locked: true },
                { header: '提单号', dataIndex: 'SECONDLADINGBILLNO', width: 110, locked: true },
                { header: '运抵编号', dataIndex: 'ARRIVEDNO', width: 150 },
                { header: '件数/重量', dataIndex: 'GOODSNUM', width: 90, renderer: renderOrder },//该字段需要拼接
                { header: '申报关区', dataIndex: 'CUSTOMAREACODE', width: 60 },//需要显示编码
                { header: '进/出口岸', dataIndex: 'PORTCODE', width: 70 },//需要显示编码
                { header: '申报方式', dataIndex: 'REPWAYID', width: 100, renderer: renderOrder },
                { header: '转关预录号', dataIndex: 'TURNPRENO', width: 70 },//需要确定具体字段
                { header: '法检', dataIndex: 'LAWFLAG', width: 60, renderer: renderOrder },
                { header: '受理时间', dataIndex: 'SUBMITTIME', width: 130, locked: true },
                { header: '订单编号', dataIndex: 'CODE', width: 100 },
              { header: '委托人员', dataIndex: 'SUBMITUSERNAME', width: 80, locked: true },
              { header: '报关申报单位', dataIndex: 'REPUNITNAME', width: 180, locked: true }];
            break;
        case "21":
            busitype = "海运进口";
            columns_order = [{ xtype: 'rownumberer', width: 35 },
                { header: 'ID', dataIndex: 'ID', hidden: true, locked: true },
                { header: '报关状态', dataIndex: 'DECLSTATUS', width: 90, renderer: renderOrder, locked: true },
                { header: '报检状态', dataIndex: 'INSPSTATUS', width: 90, renderer: renderOrder, locked: true },
                { header: '合同号', dataIndex: 'CONTRACTNO', width: 100, locked: true },
                { header: '国检提单号', dataIndex: 'FIRSTLADINGBILLNO', width: 120 },//需要确定具体字段
                { header: '海关提单号', dataIndex: 'SECONDLADINGBILLNO', width: 120 },//需要确定具体字段
                { header: '件数/重量', dataIndex: 'GOODSNUM', width: 65, renderer: renderOrder },//该字段需要拼接
                { header: '申报关区', dataIndex: 'CUSTOMAREACODE', width: 60 },//需要显示编码
                { header: '进/出口岸', dataIndex: 'PORTCODE', width: 70 },//需要显示编码
                { header: '申报方式', dataIndex: 'REPWAYID', width: 110, renderer: renderOrder },
                { header: '转关预录号', dataIndex: 'TURNPRENO', width: 130 },
                { header: '法检', dataIndex: 'LAWFLAG', width: 60, renderer: renderOrder },
                { header: '受理时间', dataIndex: 'SUBMITTIME', width: 130, locked: true },
                { header: '订单编号', dataIndex: 'CODE', width: 100 },
              { header: '委托人员', dataIndex: 'SUBMITUSERNAME', width: 80, locked: true },
              { header: '报关申报单位', dataIndex: 'REPUNITNAME', width: 180, locked: true }];
            break;
        case "30":
            busitype = "陆运出口";
            columns_order = [{ xtype: 'rownumberer', width: 35 },
                { header: 'ID', dataIndex: 'ID', hidden: true, locked: true },
                { header: '报关状态', dataIndex: 'DECLSTATUS', width: 90, locked: true, renderer: renderOrder },
                { header: '报检状态', dataIndex: 'INSPSTATUS', width: 90, locked: true, renderer: renderOrder },
                { header: '合同发票号', dataIndex: 'CONTRACTNO', width: 100, locked: true },
                { header: '总单号', dataIndex: 'TOTALNO', width: 90 },//需要确定具体字段
                { header: '分单号', dataIndex: 'DIVIDENO', width: 110, locked: true },//需要确定具体字段
                { header: '件数/重量', dataIndex: 'GOODSNUM', width: 90, renderer: renderOrder },
                { header: '申报关区', dataIndex: 'CUSTOMDISTRICTCODE', width: 70 },//需要显示编码
                { header: '进/出口岸', dataIndex: 'PORTCODE', width: 70 },//需要显示编码
                { header: '申报方式', dataIndex: 'REPWAYID', width: 100, renderer: renderOrder },
                { header: '运抵编号', dataIndex: 'ARRIVEDNO', width: 130 },//需要确定具体字段
                { header: '转关预录号', dataIndex: 'TURNPRENO', width: 130 },//需要确定具体字段
                { header: '法检', dataIndex: 'LAWFLAG', width: 60, renderer: renderOrder },
                { header: '受理时间', dataIndex: 'SUBMITTIME', width: 130, locked: true },
                { header: '订单编号', dataIndex: 'CODE', width: 100 },
              { header: '委托人员', dataIndex: 'SUBMITUSERNAME', width: 80, locked: true },
              { header: '报关申报单位', dataIndex: 'REPUNITNAME', width: 180, locked: true }];
            $("#logistics").hide();
            break;
        case "31":
            busitype = "陆运进口";
            columns_order = [{ xtype: 'rownumberer', width: 35 },
               { header: 'ID', dataIndex: 'ID', sortable: true, hidden: true, locked: true },
               { header: '报关状态', dataIndex: 'DECLSTATUS', width: 90, renderer: renderOrder, locked: true },
               { header: '报检状态', dataIndex: 'INSPSTATUS', width: 90, renderer: renderOrder, locked: true },
               { header: '合同号', dataIndex: 'CONTRACTNO', width: 100, locked: true },
               { header: '分单号', dataIndex: 'DIVIDENO', width: 110 },
               { header: '件数/重量', dataIndex: 'GOODSNUM', width: 90, renderer: renderOrder },//该字段需要拼接
               { header: '申报关区', dataIndex: 'CUSTOMAREACODE', width: 70 },//需要显示编码
               { header: '进/出口岸', dataIndex: 'PORTCODE', width: 70 },//需要显示编码
               { header: '申报方式', dataIndex: 'REPWAYID', width: 100, renderer: renderOrder },
               { header: '法检', dataIndex: 'LAWFLAG', width: 60, renderer: renderOrder },
               { header: '受理时间', dataIndex: 'SUBMITTIME', width: 130, locked: true },
               { header: '订单编号', dataIndex: 'CODE', width: 100 },
              { header: '委托人员', dataIndex: 'SUBMITUSERNAME', width: 80, locked: true },
              { header: '报关申报单位', dataIndex: 'REPUNITNAME', width: 180, locked: true }];
            $("#logistics").hide();
            break;
        case "40-41":
            busitype = "国内";
            columns_order = [{ xtype: 'rownumberer', width: 35 },
               { header: 'ID', dataIndex: 'ID', hidden: true, locked: true },
               { header: '报关状态', dataIndex: 'DECLSTATUS', width: 70, renderer: renderOrder, locked: true },
               { header: '报检状态', dataIndex: 'INSPSTATUS', width: 70, renderer: renderOrder, locked: true },
               { header: '合同发票号', dataIndex: 'CONTRACTNO', width: 130, locked: true },
               { header: '件数/重量', dataIndex: 'GOODSNUM', width: 80, renderer: renderOrder, locked: true },//该字段需要拼接
               { header: '申报关区', dataIndex: 'CUSTOMAREACODE', width: 70 },//需要显示编码
               { header: '申报方式', dataIndex: 'REPWAYID', width: 110, renderer: renderOrder },
               { header: '法检', dataIndex: 'LAWFLAG', width: 40, renderer: renderOrder },
               { header: '业务类型', dataIndex: 'BUSITYPE', width: 70, renderer: renderOrder },
               { header: '两单关联号', dataIndex: 'ASSOCIATENO', width: 120 },
               { header: '受理时间', dataIndex: 'SUBMITTIME', width: 130, locked: true },
               { header: '多单关联号', dataIndex: 'CORRESPONDNO', width: 110 },
               { header: '订单编号', dataIndex: 'CODE', width: 100 },
              { header: '委托人员', dataIndex: 'SUBMITUSERNAME', width: 80, locked: true },
              { header: '报关申报单位', dataIndex: 'REPUNITNAME', width: 180, locked: true }];
            $("#logistics").hide();
            break;
        case "50-51":
            busitype = "特殊区域";
            columns_order = [{ xtype: 'rownumberer', width: 35, locked: true },
                { header: 'ID', dataIndex: 'ID', width: 110, locked: true, hidden: true },
                { header: '报关状态', dataIndex: 'DECLSTATUS', width: 90, locked: true, renderer: renderOrder },
                { header: '报检状态', dataIndex: 'INSPSTATUS', locked: true, width: 90, renderer: renderOrder },
                { header: '合同发票号', dataIndex: 'CONTRACTNO', locked: true, width: 100 },
                { header: '申报方式', dataIndex: 'REPWAYID', width: 100, renderer: renderOrder },
                { header: '件数/重量', dataIndex: 'GOODSNUM', width: 100, renderer: renderOrder },
                { header: '申报关区', dataIndex: 'CUSTOMAREACODE', width: 70 },
                { header: '进/出口岸', dataIndex: 'PORTCODE', width: 70 },
                { header: '转关预录号', dataIndex: 'TURNPRENO', width: 150 },
                { header: '法检', dataIndex: 'LAWFLAG', width: 60, renderer: renderOrder },
                { header: '受理时间', dataIndex: 'SUBMITTIME', width: 130, locked: true },
                { header: '订单编号', dataIndex: 'CODE', width: 100 },
              { header: '委托人员', dataIndex: 'SUBMITUSERNAME', width: 80, locked: true },
              { header: '报关申报单位', dataIndex: 'REPUNITNAME', width: 180, locked: true }
            ];
            $("#logistics").hide();
            break;
    }

    var model_fields = ['ID', 'ENTRUSTTYPE', 'DECLSTATUS', 'INSPSTATUS', 'CODE', 'CUSNO', 'PORTCODE', 'TURNPRENO', 'SUBMITTIME',
                        'BUSIUNITNAME', 'BUSIUNITCODE', 'CONTRACTNO', 'TOTALNO', 'DIVIDENO', 'REPWAYID', 'GOODSNUM',
                        'GOODSGW', 'CUSTOMAREACODE', 'LAWFLAG', 'ISINVALID', 'BUSITYPE', 'PRINTSTATUS', 'STATUS', 'REPNO', 'ASSOCIATENO',
                       'CORRESPONDNO', 'FIRSTLADINGBILLNO', 'SECONDLADINGBILLNO', 'ARRIVEDNO', 'SUBMITUSERNAME', 'REPUNITNAME', 'LOGISTICSSTATUS'];
    Ext.define('ORDERLIST', {
        extend: 'Ext.data.Model',
        fields: model_fields
    });

    Ext.Ajax.request({
        url: "/Common/Ini_Base_Data",
        params: {
            busitype: busitype
        },
        success: function (response, opts) {
            var commondata = Ext.decode(response.responseText);
            common_data_sbgq = commondata.sbgq;//申报关区
            common_data_sbfs = commondata.sbfs;//申报方式
            //查询区域

            initSearch();
            store_busitype = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_busitype
            });
            store_sbfs = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_sbfs
            });
            var store_Trade = Ext.create('Ext.data.JsonStore', {
                model: 'ORDERLIST',
                pageSize: 20,
                proxy: {
                    type: 'ajax',
                    url: '/EnterpriseOrder/LoadList_index',
                    reader: {
                        root: 'rows',
                        type: 'json',
                        totalProperty: 'total'
                    }
                },
                autoLoad: true,
                listeners: {
                    beforeload: function () {
                        store_Trade.getProxy().extraParams = {
                            busitypeid: busitypeid,
                            CONDITION1: Ext.getCmp('CONDITION1').getValue(), VALUE1: Ext.getCmp("CONDITION1_1").getValue(),
                            CONDITION2: Ext.getCmp('CONDITION2').getValue(), VALUE2: Ext.getCmp("CONDITION2_1").getValue(),
                            CONDITION3: Ext.getCmp('CONDITION3').getValue(), VALUE3: Ext.getCmp("CONDITION3_1").getValue(),
                            CONDITION4: Ext.getCmp('CONDITION4').getValue(), VALUE4: Ext.getCmp("CONDITION4_1").getValue(),
                            STARTDATE: Ext.Date.format(Ext.getCmp("start_date").getValue(), 'Y-m-d H:i:s'),
                            ENDDATE: Ext.Date.format(Ext.getCmp("end_date").getValue(), 'Y-m-d H:i:s'),

                        }
                    }
                }
            })
            pgbar = Ext.create('Ext.toolbar.Paging', {
                displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                store: store_Trade,
                id: 'pgbar',
                displayInfo: true
            })
            gridpanel = Ext.create('Ext.grid.Panel', {
                renderTo: "appConId",
                store: store_Trade,
                height: 500,
                selModel: { selType: 'checkboxmodel' },
                bbar: pgbar,
                enableColumnHide: false,
                columns: columns_order,
                listeners:
                {
                    'cellclick': function (view, td, cellindex, record, tr, rowindex, e, opt) {
                        var dataindex = gridpanel.columns[cellindex].dataIndex;
                        if (dataindex == 'LOGISTICSNAME') {
                            var totalno = record.data.TOTALNO;
                            var divdeno = record.data.DIVIDENO;
                          
                            if (record.data.LOGISTICSSTATUS == null || record.data.LOGISTICSSTATUS == "") {
                                Ext.Ajax.request({
                                    url: '/EnterpriseOrder/getLogisticStatus',
                                    params: { totalno: totalno, divdeno: divdeno },
                                    success: function (response, option) {
                                        store_Trade.getAt(rowindex).set('LOGISTICSNAME', response.responseText);
                                        store_Trade.getAt(rowindex).commit();
                                    }

                                });
                            }
                            if (win_logistic) {
                                win_logistic.close();
                            }
                            showLogisticStatus(totalno, divdeno);
                        }
                    }
                },
                viewConfig: {
                    enableTextSelection: true
                }
            });
        }
    });


});

//=======================================================JS init end======================================================

function initSearch() {
    var store_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "报关状态", "CODE": "DECLSTATUS" }, { "NAME": "报检状态", "CODE": "INSPSTATUS" }, { "NAME": "物流状态", "CODE": "LOGISTICSSTATUS" }]
    });
    var combo_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION1',
        name: "CONDITION1",
        value: "DECLSTATUS",
        store: store_1,
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        margin: 0,
        flex: .35,
        listeners: {
            change: function () {
                combo_1_1.reset();
                if (combo_1.getValue() == "LOGISTICSSTATUS") {
                    store_1_1.loadData(logistic_status_data);
                }
                else {
                    store_1_1.loadData(search_js_condition3_bgbjstatus_data);
                }
            }
        }
    })
    var store_1_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition3_bgbjstatus_data
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
        editable: false,
        forceSelection: true,
        minChars: 1,
        flex: .65
    })
    var condition1 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_1, combo_1_1]
    }
    var store_2 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "合同发票号", "CODE": "CONTRACTNO" }]
    });
    var combo_2 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION2',
        name: "CONDITION2",
        store: store_2,
        value: "CONTRACTNO",
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        flex: .40,
        margin: 0,
    });
    var field_2_1 = Ext.create('Ext.form.field.Text', {
        id: 'CONDITION2_1',
        margin: 0,
        flex: .60
    })
    var condition2 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_2, field_2_1]
    }

    var store_3 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "对应号", "CODE": "REPNO" }, { "NAME": "分单号", "CODE": "DIVIDENO" }, { "NAME": "载货清单号", "CODE": "MANIFEST" }, { "NAME": "海关提单号", "CODE": "SECONDLADINGBILLNO" }]
    });
    var combo_3 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION3',
        name: "CONDITION3",
        store: store_3,
        value: "REPNO",
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        flex: .40,
        margin: 0,
    });
    var field_3_1 = Ext.create('Ext.form.field.Text', {
        id: 'CONDITION3_1',
        margin: 0,
        flex: .60
    })
    var condition3 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_3, field_3_1]
    }

    var store_4 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "申报关区", "CODE": "CUSTOMAREACODE" }, { "NAME": "申报方式", "CODE": "REPWAYID" }, { "NAME": "进出口岸", "CODE": "PORTCODE" }]
    });
    var combo_4 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION4',
        name: "CONDITION4",
        value: "CUSTOMAREACODE",
        store: store_4,
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        margin: 0,
        flex: .35,
        listeners:
          {
              change: function () {
                  combo_4_1.reset();
                  if (combo_4.getValue() == "CUSTOMAREACODE") {
                      combo_4_1.minChars = "2";
                      store_4_1.loadData(common_data_sbgq);
                  }
                  if (combo_4.getValue() == "REPWAYID") {
                      combo_4_1.minChars = "1";
                      store_4_1.loadData(common_data_sbfs);
                  }
                  if (combo_4.getValue() == "PORTCODE") {
                      combo_4_1.minChars = "2";
                      store_4_1.loadData(common_data_sbgq);
                  }
              }
          }
    })
    var store_4_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: common_data_sbgq
    });
    var combo_4_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION4_1',
        name: "NAME",
        margin: 0,
        store: store_4_1,
        displayField: 'NAME',
        valueField: "CODE",
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        forceSelection: true,
        minChars: 2,
        flex: .65
    })
    var condition4 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_4, combo_4_1]
    }
   
    var start_date = Ext.create('Ext.form.field.Date', {
        id: 'start_date',
        format: 'Y-m-d',
        name: 'start_date',
        flex: .5, margin: 0
    })
    var end_date = Ext.create('Ext.form.field.Date', {
        id: 'end_date',
        format: 'Y-m-d',
        name: 'end_date',
        flex: .5, margin: 0
    })
    //创建时间
    var condition5 = Ext.create('Ext.form.FieldContainer', {
        fieldLabel: '受理日期',
        labelWidth: 60,
        layout: 'hbox',
        columnWidth: .24,
        items: [start_date, end_date],
        //flex: .33
    })



    var store_6 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        //data: declarationsearch_js_condition4_data
    });

    var formpanel = Ext.create('Ext.form.Panel', {
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.19
        },
        items: [
        { layout: 'column', border: 0, items: [condition1, condition2, condition3, condition4,condition5] },
        ]
    });

}
//重置
function Reset() {
    Ext.getCmp("CONDITION1").setValue("DECLSTATUS");
    Ext.getCmp("CONDITION1_1").setValue("");
    Ext.getCmp("CONDITION2").setValue("CONTRACTNO");
    Ext.getCmp("CONDITION2_1").setValue("");
    Ext.getCmp("CONDITION3").setValue("REPNO");
    Ext.getCmp("CONDITION3_1").setValue("");
    Ext.getCmp("CONDITION4").setValue("CUSTOMAREACODE");
    Ext.getCmp("CONDITION4_1").setValue("");
    Ext.getCmp("start_date").setValue("");
    Ext.getCmp("end_date").setValue("");

}
function Select() {
    Ext.getCmp('pgbar').moveFirst();
}
function openrwindow(url, width, height) {
    var iWidth = width ? width : "1000", iHeight = height ? height : "600";
    var iTop = (window.screen.availHeight - 30 - iHeight) / 2; //获得窗口的垂直位置;
    var iLeft = (window.screen.availWidth - 10 - iWidth) / 2; //获得窗口的水平位置; 
    window.open(url, '', 'height=' + iHeight + ',,innerHeight=' + iHeight + ',width=' + iWidth + ',innerWidth=' + iWidth + ',top=' + iTop + ',left=' + iLeft + ',location=yes,scrollbars=yes');
}
function Open() {
    var recs = gridpanel.getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要维护的记录！');
        return;
    }
    var id = recs[0].data.ID
    var index = gridpanel.store.indexOf(recs[0]);
    var currentPage = gridpanel.store.currentPage;

    var menuxml = "";
    switch (recs[0].get("BUSITYPE")) {
        case "11":
            menuxml = "ent_airin";
            break;
        case "10":
            menuxml = "ent_airout";
            break;
        case "21":
            menuxml = "ent_seain";
            break;
        case "20":
            menuxml = "ent_seaout";
            break;
        case "31":
            menuxml = "ent_landin";
            break;
        case "30":
            menuxml = "ent_landout";
            break;
        case "40": case "41":
            menuxml = "ent_domestic";
            break;
        case "50": case "51":
            menuxml = "ent_special";
            break;
    }

    openrwindow("/EnterpriseOrder/GoodsTrack?menuxml=" + menuxml + "&busitypeid=" + busitypeid + "&id=" + id + "&rowIndex=" + index + "&currentPage=" + currentPage, 1200, 800);
}
function ViewsEnterprise() {
    var recs = gridpanel.getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要查看的记录！');
        return;
    }

    var menuxml = "";
    switch (recs[0].get("BUSITYPE")) {
        case "11":
            menuxml = "ent_airin";
            break;
        case "10":
            menuxml = "ent_airout";
            break;
        case "21":
            menuxml = "ent_seain";
            break;
        case "20":
            menuxml = "ent_seaout";
            break;
        case "31":
            menuxml = "ent_landin";
            break;
        case "30":
            menuxml = "ent_landout";
            break;
        case "40": case "41":
            menuxml = "ent_domestic";
            break;
        case "50": case "51":
            menuxml = "ent_special";
            break;
    }

    if (busitypeid == "40-41") {
        opencenterwin("/OrderDomestic/OrderView?menuxml=" + menuxml + "&Role=enterprise&OrderId=" + recs[0].get("ID") + "&OrderCode=" + recs[0].get("CODE") + "&busitypeid=" + recs[0].get("BUSITYPE"), 1500, 800);
    }
    else {
        opencenterwin("/Common/OrderView?menuxml=" + menuxml + "&Role=enterprise&OrderId=" + recs[0].get("ID") + "&ordercode=" + recs[0].get("CODE") + "&busitypeid=" + recs[0].get("BUSITYPE"), 1200, 800);
    }

}
function Export() {
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据导出中，请稍等..." });
    myMask.show();

    var data = {
        dec_insp_status: JSON.stringify(orderstatus_js_data),
        busitypeid: busitypeid,
        CONDITION1: Ext.getCmp('CONDITION1').getValue(), VALUE1: Ext.getCmp("CONDITION1_1").getValue(),
        CONDITION2: Ext.getCmp('CONDITION2').getValue(), VALUE2: Ext.getCmp("CONDITION2_1").getValue(),
        CONDITION3: Ext.getCmp('CONDITION3').getValue(), VALUE3: Ext.getCmp("CONDITION3_1").getValue(),
        CONDITION4: Ext.getCmp('CONDITION4').getValue(), VALUE4: Ext.getCmp("CONDITION4_1").getValue()

    }
    Ext.Ajax.request({
        url: '/EnterpriseOrder/ExportList',
        params: data,
        success: function (response, option) {
            var json = Ext.decode(response.responseText);
            if (json.success == false) {
                Ext.MessageBox.show('提示', '综合需求及性能，导出记录限制' + json.WebDownCount + '！');
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
function showLogisticStatus(totalno,divdeno)
{
    Ext.define('LOGISTICSTATUS', {
        extend: 'Ext.data.Model',
        fields: ['ID', 'MSG', 'OPERATER', 'OPERATE_TYPE', 'OPERATE_RESULT', 'OPERATE_DATE']
    });
    var store_logistic = Ext.create('Ext.data.JsonStore', {
        model: 'LOGISTICSTATUS',
        groupField: 'OPERATE_TYPE',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/EnterpriseOrder/LoadList_logistic',
            reader: {
                root: 'rows',
                type: 'json'
            }
        },
        autoLoad: true,
        listeners: {
            beforeload: function () {
                store_logistic.getProxy().extraParams = {
                    totalno:totalno,
                    divdeno:divdeno
                }
            }
        }
    });


   
    var tab_0_store, tab_1_store, tab_2_store, tab_3_store;
    store_logistic.load(function () {
        var data_kazt;
        var data1 = store_logistic.getGroups("报关申报状态") == undefined ? [] : store_logistic.getGroups("报关申报状态").children
        var data2 = store_logistic.getGroups("转关申报状态") == undefined ? [] : store_logistic.getGroups("转关申报状态").children
        data_kazt = data1.concat(data2);
         tab_0_store = Ext.create("Ext.data.JsonStore", {
            fields: ['ID', 'MSG', 'OPERATER', 'OPERATE_TYPE', 'OPERATE_RESULT', 'OPERATE_DATE'],
            data: store_logistic.getGroups("抽单状态")==undefined?[]:store_logistic.getGroups("抽单状态").children
        });
         tab_1_store = Ext.create("Ext.data.JsonStore", {
             fields: ['ID', 'MSG', 'OPERATER', 'OPERATE_TYPE', 'OPERATE_RESULT', 'OPERATE_DATE'],
             //data: store_logistic.getGroups("抽单状态") == undefined ? [] : store_logistic.getGroups("抽单状态").children
             data:data_kazt
         });
         tab_1_store.sort([
                    {
                        property: 'OPERATE_DATE',
                        direction: 'DESC'
                    }
         ]);
         tab_2_store = Ext.create("Ext.data.JsonStore", {
            fields: ['ID', 'MSG', 'OPERATER', 'OPERATE_TYPE', 'OPERATE_RESULT', 'OPERATE_DATE'],
            data: store_logistic.getGroups("报检状态") == undefined ? [] : store_logistic.getGroups("报检状态").children
        });
         tab_3_store = Ext.create("Ext.data.JsonStore", {
            fields: ['ID', 'MSG', 'OPERATER', 'OPERATE_TYPE', 'OPERATE_RESULT', 'OPERATE_DATE'],
            data: store_logistic.getGroups("运输状态") == undefined ? [] : store_logistic.getGroups("运输状态").children
        });



         var columns_logistic = [
            { header: 'ID', dataIndex: 'ID', hidden: true},
            { header: '提示信息', dataIndex: 'MSG' },
            { header: '操作人', dataIndex: 'OPERATER' },
            { header: '状态类型', dataIndex: 'OPERATE_TYPE'},
            { header: '状态值', dataIndex: 'OPERATE_RESULT'},
            { header: '时间', dataIndex: 'OPERATE_DATE',width:160}
         ]
         tab_0_gridpanel = Ext.create('Ext.grid.Panel', {
             store: tab_0_store,
             enableColumnHide: false,
             columns: columns_logistic,
             height: 400,
             width: 585,
             overflowY: 'auto',
             viewConfig: {
                 enableTextSelection: true
             }
         });
         tab_1_gridpanel = Ext.create('Ext.grid.Panel', {
             store: tab_1_store,
             enableColumnHide: false,
             columns: columns_logistic,
             height: 400,
             width: 585,
             overflowY: 'auto',
             viewConfig: {
                 enableTextSelection: true
             }
         });
         tab_2_gridpanel = Ext.create('Ext.grid.Panel', {
             store: tab_2_store,
             enableColumnHide: false,
             columns: columns_logistic,
             height: 400,
             width: 585,
             overflowY: 'auto',
             viewConfig: {
                 enableTextSelection: true
             }
         });
         tab_3_gridpanel = Ext.create('Ext.grid.Panel', {
             store: tab_3_store,
             enableColumnHide: false,
             columns: columns_logistic,
             height: 400,
             width: 585,
             overflowY: 'auto',
             viewConfig: {
                 enableTextSelection: true
             }
         });

         var items = [{ title: '抽单状态', id: "tab_0", items: [tab_0_gridpanel] }, { title: '口岸状态', id: "tab_1", items: [tab_1_gridpanel] },
                      { title: '报检状态', id: "tab_2", items: [tab_2_gridpanel] }, { title: '运输状态', id: "tab_3", items: [tab_3_gridpanel] }];
         var tabpanel = Ext.create('Ext.tab.Panel', {
             id: 'tabpanel',
             items: items,
         });

         win_logistic = Ext.create('Ext.window.Window', {
             title: '物流状态',
             height: 400,
             width: 600,
             layout: 'fit',
             items: [tabpanel]
         }).show();

    });
    

 
}

function renderLogistic(value, cellmeta, record, rowIndex, columnIndex, store) {
    var rtn = "";
    var dataindex = cellmeta.column.dataIndex;
    switch (dataindex) {
        case "LOGISTICSNAME":
            if (value) {
                rtn = "<a style='color:blue'>" + value + "</a>";
            }
            break;
    }
    return rtn;
}