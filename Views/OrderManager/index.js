var common_data_jydw = [], common_data_wtdw = [], common_data_busi = [];
var store_busitype; var columns_order = [];
Ext.onReady(function () {
    Ext.Ajax.request({//对公共基础数据发起一次请求
        url: "/Common/Ini_Base_Data",
        params: { ParaType: 'OrderManager' },
        success: function (response, option) {
            var commondata = Ext.decode(response.responseText);
            common_data_jydw = commondata.jydw;//经营单位
            common_data_wtdw = commondata.wtdw;//委托单位
            common_data_busi = commondata.busi;//业务类型
            
            store_busitype = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME', 'CODENAME'], data: common_data_busi });

            initSearch_OrderM();
            bindgrid();
        }
    });
});

function initSearch_OrderM() {
    //经营单位
    var store_busiunitname = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_jydw
    });
    var s_combo_busiunitname = Ext.create('Ext.form.field.ComboBox', {
        id: 's_combo_busiunitname',
        name: 'BUSIUNITNAME',
        store: store_busiunitname,
        fieldLabel: '经营单位', //tabIndex: 1,
        displayField: 'NAME',
        minChars: 4,
        valueField: 'CODE',
        triggerAction: 'all',
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local'
    });

    //企业编号
    var field_CUSNO = Ext.create('Ext.form.field.Text', {
        id: 'field_CUSNO',
        fieldLabel: '企业编号', //tabIndex: 2,
        name: 'CUSNO'
    });

    //业务类型
    //var store_busitype = Ext.create('Ext.data.JsonStore', {
    //    fields: ['CODE', 'NAME', 'CODENAME'],
    //    data: common_data_busi
    //});

    var combo_BUSITYPE = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_BUSITYPE',
        name: 'BUSITYPEID',
        store: store_busitype,
        fieldLabel: '业务类型',//tabIndex: 3
        displayField: 'CODENAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
        minChars: 1,
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                };
            },
            change: function (combo, newValue, oldValue, eOpts) {
                combo_ENTRUSTTYPE.reset();
                Ext.Ajax.request({
                    url: "/OrderManager/Ini_Base_Data_BUSIITEM",
                    params: { busitype: newValue },
                    success: function (response, opts) {
                        var commondata = Ext.decode(response.responseText);//业务细项
                        store_ENTRUSTTYPE.loadData(commondata.ywxx);
                    }
                });
            }
        }
    });

    var start_date = Ext.create('Ext.form.field.Date', {
        id: 'start_date', name: 'start_date', format: 'Y-m-d', emptyText: '开始日期', flex: .5, margin: 0,//tabIndex: 3,
        listeners: {
            focus: function (cb) {
                var con = Ext.Date.format(Ext.getCmp("end_date").getValue(), 'Y-m-d');
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
    });
    var end_date = Ext.create('Ext.form.field.Date', {
        id: 'end_date', name: 'end_date', format: 'Y-m-d', emptyText: '结束日期', flex: .5, margin: 0,//tabIndex: 4,
        listeners: {
            focus: function (cb) {
                var con = Ext.Date.format(Ext.getCmp("start_date").getValue(), 'Y-m-d');
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
    });
    var condate = Ext.create('Ext.form.FieldContainer', { fieldLabel: '创建日期', layout: 'hbox', columnWidth: .33, items: [start_date, end_date] });

    //委托单位
    var store_wtdw = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME', 'CODENAME'],
        data: common_data_wtdw
    })
    var combo_wtdw = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_wtdw',
        name: 'CUSTOMERCODE',
        store: store_wtdw,
        fieldLabel: '委托单位', //tabIndex: 5,
        displayField: 'CODENAME',
        valueField: 'CODE',
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local'
    });


    //订单编号
    var field_CODE = Ext.create('Ext.form.field.Text', {
        id: 'field_CODE',
        fieldLabel: '订单编号', //tabIndex: 6,
        name: 'CODE'
    });

    //业务细项
    var store_ENTRUSTTYPE = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME']//,data: common_data_ywxx
    });

    var combo_ENTRUSTTYPE = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ENTRUSTTYPE',
        name: 'ENTRUSTTYPE',
        store: store_ENTRUSTTYPE,
        fieldLabel: '业务细项',//tabIndex: 3
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
        minChars: 1,
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                };
            }
        }
    });

    var start_date2 = Ext.create('Ext.form.field.Date', {
        id: 'start_date2', name: 'start_date2', format: 'Y-m-d', emptyText: '开始日期', flex: .5, margin: 0,//tabIndex: 7,
        listeners: {
            focus: function (cb) {
                var con = Ext.Date.format(Ext.getCmp("end_date2").getValue(), 'Y-m-d');
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
    });
    var end_date2 = Ext.create('Ext.form.field.Date', {
        id: 'end_date2', name: 'end_date2', format: 'Y-m-d', emptyText: '结束日期', flex: .5, margin: 0,//tabIndex: 8,
        listeners: {
            focus: function (cb) {
                var con = Ext.Date.format(Ext.getCmp("start_date2").getValue(), 'Y-m-d');
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
    });
    var condate2 = Ext.create('Ext.form.FieldContainer', { fieldLabel: '业务完成', layout: 'hbox', columnWidth: .33, items: [start_date2, end_date2] });


    var formpanel = Ext.create('Ext.form.Panel', {
        id: 'formpanel',
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.22,
            labelWidth: 80
        },
        items: [
        { layout: 'column', border: 0, items: [s_combo_busiunitname, field_CUSNO,combo_BUSITYPE, condate] },
        { layout: 'column', border: 0, items: [combo_wtdw, field_CODE, combo_ENTRUSTTYPE, condate2] }
        ]
    });

    //初始化时间控件
    var before = new Date();
    before.setDate(before.getDate() - 3);
    var beforeday = Ext.Date.format(before, 'm/d/Y');

    var today = Ext.Date.format(new Date(), 'm/d/Y');

    Ext.getCmp("start_date").setValue(beforeday); Ext.getCmp("end_date").setValue(today);
    
}

function bindgrid() {
    columns_order = [
                { xtype: 'rownumberer', width: 35 },
                { header: 'ID', dataIndex: 'ID', hidden: true, locked: true },
                { header: '业务完成', dataIndex: 'SUBMITTIME', width: 130, locked: true },
                { header: '创建时间', dataIndex: 'CREATETIME', width: 130, locked: true },
                {
                    header: '业务类型', dataIndex: 'BUSITYPE', width: 90, renderer: function renderOrder(value, cellmeta, record, rowIndex, columnIndex, store) {
                        var rtn = "";
                        if (value) {
                            var rec = store_busitype.findRecord('CODE', value);
                            if (rec) {
                                rtn = rec.get("NAME");
                            }
                        }
                        return rtn;
                    }
                },
                { header: '业务细项', dataIndex: 'ENTRUSTTYPENAME', width: 100, locked: true },
                { header: '委托单位', dataIndex: 'CUSTOMERNAME', width: 130, locked: true },
                { header: '结算单位', dataIndex: 'CLEARUNITNAME', width: 130, locked: true },
                { header: '经营单位', dataIndex: 'BUSIUNITNAME', width: 130, locked: true },
                { header: '企业编号', dataIndex: 'CUSNO', width: 100 },
                { header: '订单编号', dataIndex: 'CODE', width: 100 },
                { header: '操作需求', dataIndex: 'DOREQUEST', width: 130 },
                { header: '结算备注', dataIndex: 'CLEARREMARK', width: 130 }
    ];

    //Ext.regModel('TRADE', {
    //    fields: ['ID', 'BUSITYPE', 'CREATETIME', 'SUBMITTIME', 'ENTRUSTTYPE', 'CUSTOMERCODE', 'CUSTOMERNAME', 'CLEARUNIT', 'CLEARUNITNAME'
    //            , 'BUSIUNITCODE', 'BUSIUNITNAME', 'CODE', 'CUSNO', 'DOREQUEST', 'CLEARREMARK', 'ENTRUSTTYPENAME']
    //});
    var store_Trade = Ext.create('Ext.data.JsonStore', {
        //model: 'TRADE',
        fields: ['ID', 'BUSITYPE', 'CREATETIME', 'SUBMITTIME', 'ENTRUSTTYPE', 'CUSTOMERCODE', 'CUSTOMERNAME', 'CLEARUNIT', 'CLEARUNITNAME'
                , 'BUSIUNITCODE', 'BUSIUNITNAME', 'CODE', 'CUSNO', 'DOREQUEST', 'CLEARREMARK', 'ENTRUSTTYPENAME'],
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/OrderManager/LoadList',
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
                    OnlySelf: Ext.get('OnlySelfi').el.dom.className,
                    BUSIUNITCODE: Ext.getCmp('s_combo_busiunitname').getValue(), CUSNO: Ext.getCmp('field_CUSNO').getValue(), busitypeid: Ext.getCmp('combo_BUSITYPE').getValue(),
                    start_date: Ext.Date.format(Ext.getCmp("start_date").getValue(), 'Y-m-d H:i:s'), end_date: Ext.Date.format(Ext.getCmp("end_date").getValue(), 'Y-m-d H:i:s'),
                    CUSTOMERCODE: Ext.getCmp('combo_wtdw').getValue(), CODE: Ext.getCmp('field_CODE').getValue(), entrusttype: Ext.getCmp('combo_ENTRUSTTYPE').getValue(),
                    start_date2: Ext.Date.format(Ext.getCmp("start_date2").getValue(), 'Y-m-d H:i:s'), end_date2: Ext.Date.format(Ext.getCmp("end_date2").getValue(), 'Y-m-d H:i:s')
                }
            }
        }
    });
    Ext.tip.QuickTipManager.init();//初始化全局的QuickTips，为创建QuickTips做准备onclick="ShowList();"
    var pgbar = Ext.create('Ext.toolbar.Paging', {
        id: 'pgbar',
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_Trade,
        displayInfo: true
    });
    //显示
    var gridpanel = Ext.create('Ext.grid.Panel', {
        id: 'gridpanel',
        renderTo: "appConId",
        store: store_Trade,
        height: 500,
        selModel: { selType: 'checkboxmodel' },
        bbar: pgbar,
        enableColumnHide: false,
        columns: columns_order,
        listeners:
        {
            'itemdblclick': function (view, record, item, index, e) {
                opencenterwin("/OrderManager/Create?ordercode=" + record.get("CODE"), 1600, 900);
            }
        },
        viewConfig: {
            enableTextSelection: true
        }
    });
}

function Select() {
    Ext.getCmp("pgbar").moveFirst();
}

function Open() {
    opencenterwin("/OrderManager/Create", 1600, 900);
}

function EditOrderM() {
    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length != 1) {
        Ext.MessageBox.alert('提示', '请选择一笔需要修改的记录！');
        return;
    }

    opencenterwin("/OrderManager/Create?ordercode=" + recs[0].get("CODE"), 1600, 900);
}

function changeOnlySelfStyle() {
    var OnlySelfDom = Ext.get('OnlySelfi');
    if (Ext.get('OnlySelfi').el.dom.className.replace(/(^\s*)|(\s*$)/g, "") == "fa fa-check-square-o") {
        OnlySelfDom.removeCls("fa fa-check-square-o")
        OnlySelfDom.addCls("fa fa-square-o");
    }
    else {
        OnlySelfDom.removeCls("fa fa-square-o")
        OnlySelfDom.addCls("fa fa-check-square-o");
    }
}

function Reset() {
    Ext.getCmp("s_combo_busiunitname").setValue("");
    Ext.getCmp("field_CUSNO").setValue("");
    Ext.getCmp("combo_BUSITYPE").setValue("");
    Ext.getCmp("start_date").setValue("");
    Ext.getCmp("end_date").setValue("");
    Ext.getCmp("combo_wtdw").setValue("");
    Ext.getCmp("field_CODE").setValue("");
    Ext.getCmp("combo_ENTRUSTTYPE").setValue("");
    Ext.getCmp("start_date2").setValue("");
    Ext.getCmp("end_date2").setValue("");
}

function DeleteOrderM() {

}