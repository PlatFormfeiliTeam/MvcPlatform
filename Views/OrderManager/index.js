var common_data_jydw = [], common_data_wtdw = [], common_data_entrust = [];
var store_entrust;
Ext.onReady(function () {
    Ext.Ajax.request({//对公共基础数据发起一次请求
        url: "/Common/Ini_Base_Data",
        params: { ParaType: 'OrderManager' },
        success: function (response, option) {
            var commondata = Ext.decode(response.responseText);
            common_data_jydw = commondata.jydw;//经营单位
            common_data_wtdw = commondata.wtdw;//委托单位
            common_data_entrust = commondata.entrust;//业务类别
            
            store_entrust = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME', 'CODENAME'], data: common_data_entrust });

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

    var combo_ENTRUST = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ENTRUST',
        name: 'ENTRUSTTYPECODE',
        store: store_entrust,
        fieldLabel: '业务类别',//tabIndex: 3
        displayField: 'CODENAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
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
                combo_BUSIITEMCODE.reset();
                Ext.Ajax.request({
                    url: "/OrderManager/Ini_Base_Data_BUSIITEM",
                    params: { entrusttype: newValue },
                    success: function (response, opts) {
                        var commondata = Ext.decode(response.responseText);//业务细项
                        store_BUSIITEMCODE.loadData(commondata.ywxx);

                        change_ini_label();
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
    var store_BUSIITEMCODE = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME', 'CODENAME']//,data: common_data_ywxx
    });

    var combo_BUSIITEMCODE = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_BUSIITEMCODE',
        name: 'BUSIITEMCODE',
        store: store_BUSIITEMCODE,
        fieldLabel: '业务细项',//tabIndex: 3
        displayField: 'CODENAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
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
                change_ini_label();
            }
        }
    });


    var start_date2 = Ext.create('Ext.form.field.Date', {
        id: 'start_date2', name: 'start_date2', format: 'Y-m-d', emptyText: '开始日期', flex: .5, margin: 0,width:'35%',//tabIndex: 7,
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
        id: 'end_date2', name: 'end_date2', format: 'Y-m-d', emptyText: '结束日期', flex: .5, margin: 0,width:'35%',//tabIndex: 8,
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

    //var check_box = Ext.create('Ext.form.field.Checkbox', {
    //    margin:'0',
    //    defaultType: 'checkboxfield', name: 'ischeck',
    //    inputValue: '1',
    //    id: 'ischeck'
    //});
    var store_ENABLED_S = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: [{ "CODE": 0, "NAME": "业务未完成" }, { "CODE": 1, "NAME": "业务已完成" }]
    });

    var combo_ENABLED_S = Ext.create('Ext.form.field.ComboBox', {
        margin: '0',
        id: 'combo_ENABLED_S',
        name: 'combo_ENABLED_S',
        store: store_ENABLED_S,
        queryMode: 'local',
        anyMatch: true,
        displayField: 'NAME',
        valueField: 'CODE',
        width: '30%',
        emptyText: '是否完成',
    });

    var condate2 = Ext.create('Ext.form.FieldContainer', { fieldLabel: '业务完成', layout: 'hbox', columnWidth: .33, items: [start_date2, end_date2, combo_ENABLED_S] });


    var formpanel = Ext.create('Ext.form.Panel', {
        id: 'formpanel',
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.22,
            labelWidth: 80
        },
        items: [
        { layout: 'column', border: 0, items: [s_combo_busiunitname, field_CUSNO, combo_ENTRUST, condate] },
        { layout: 'column', border: 0, items: [combo_wtdw, field_CODE, combo_BUSIITEMCODE, condate2] }
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
    var store_Trade = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'BUSITYPE', 'CREATETIME', 'SUBMITTIME', 'ENTRUSTTYPE', 'CUSTOMERCODE', 'CUSTOMERNAME', 'CLEARUNIT', 'CLEARUNITNAME'
                , 'BUSIUNITCODE', 'BUSIUNITNAME', 'CODE', 'CUSNO', 'DOREQUEST', 'CLEARREMARK', 'BUSIITEMCODE', 'BUSIITEMNAME'
                , 'TEXTONE', 'TEXTTWO', 'NUMONE', 'NUMTWO', 'DATEONE', 'DATETWO', 'USERNAMEONE', 'USERNAMETWO','NAME','GETMONEY'],
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
                    BUSIUNITCODE: Ext.getCmp('s_combo_busiunitname').getValue(), CUSNO: Ext.getCmp('field_CUSNO').getValue(), entrusttype: Ext.getCmp('combo_ENTRUST').getValue(),
                    start_date: Ext.Date.format(Ext.getCmp("start_date").getValue(), 'Y-m-d H:i:s'), end_date: Ext.Date.format(Ext.getCmp("end_date").getValue(), 'Y-m-d H:i:s'),
                    CUSTOMERCODE: Ext.getCmp('combo_wtdw').getValue(), CODE: Ext.getCmp('field_CODE').getValue(), busiitemcode: Ext.getCmp('combo_BUSIITEMCODE').getValue(),
                    start_date2: Ext.Date.format(Ext.getCmp("start_date2").getValue(), 'Y-m-d H:i:s'), end_date2: Ext.Date.format(Ext.getCmp("end_date2").getValue(), 'Y-m-d H:i:s'),
                    combo_ENABLED_S: Ext.getCmp('combo_ENABLED_S').getValue()
                }
                var TEXTONE = ""; var TEXTTWO = "";var NUMONE = ""; var NUMTWO = "";                
                var DATEONE = ""; var DATETWO = ""; var USERNAMEONE = ""; var USERNAMETWO = "";

                if (Ext.getCmp('win_seniorOrderM')) {
                    TEXTONE = Ext.getCmp('field_TEXTONE').getValue(); TEXTTWO = Ext.getCmp('field_TEXTTWO').getValue();
                    NUMONE = Ext.getCmp('field_NUMONE').getValue(); NUMTWO = Ext.getCmp('field_NUMTWO').getValue();
                    DATEONE = Ext.Date.format(Ext.getCmp('field_DATEONE').getValue(), 'Y-m-d H:i:s'); DATETWO = Ext.Date.format(Ext.getCmp('field_DATETWO').getValue(), 'Y-m-d H:i:s');
                    USERNAMEONE = Ext.getCmp('field_USERNAMEONE').getValue(); USERNAMETWO = Ext.getCmp('field_USERNAMETWO').getValue();
                }
                var new_params = {
                    TEXTONE: TEXTONE, TEXTTWO: TEXTTWO,NUMONE: NUMONE, NUMTWO: NUMTWO,                    
                    DATEONE: DATEONE, DATETWO: DATETWO,USERNAMEONE: USERNAMEONE, USERNAMETWO: USERNAMETWO                    
                }
                Ext.apply(store_Trade.proxy.extraParams, new_params);

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
        columns: [
                { xtype: 'rownumberer', width: 35 },
                { header: 'ID', dataIndex: 'ID', hidden: true, locked: true },
                { header: '业务完成', dataIndex: 'SUBMITTIME', width: 130, locked: true },
                { header: '创建时间', dataIndex: 'CREATETIME', width: 130, locked: true },
                {
                    header: '业务类别', dataIndex: 'ENTRUSTTYPE', width: 120, locked: true, renderer: function renderOrder(value, cellmeta, record, rowIndex, columnIndex, store) {
                        var rtn = "";
                        if (value) {
                            var rec = store_entrust.findRecord('CODE', value);
                            if (rec) {
                                rtn = rec.get("NAME");
                            }
                        }
                        return rtn;
                    }
                },
                { header: '业务细项', dataIndex: 'BUSIITEMNAME', width: 120, locked: true },
                { header: '委托单位', dataIndex: 'CUSTOMERNAME', width: 130, locked: true },
                { header: '结算单位', dataIndex: 'CLEARUNITNAME', width: 130, locked: true },
                { header: '经营单位', dataIndex: 'BUSIUNITNAME', width: 130, locked: true },
                { header: '企业编号', dataIndex: 'CUSNO', width: 100 },
                { header: '订单编号', dataIndex: 'CODE', width: 100 },
                { header: '应收费用状态', dataIndex: 'NAME', width: 100 },
                { header: '毛利', dataIndex: 'GETMONEY', width: 100 },
                { header: '操作需求', dataIndex: 'DOREQUEST', width: 130 },
                { header: '结算备注', dataIndex: 'CLEARREMARK', width: 130 },
                { header: '文本1', dataIndex: 'TEXTONE', width: 100 },
                { header: '文本2', dataIndex: 'TEXTTWO', width: 100 },
                { header: '数字1', dataIndex: 'NUMONE', width: 100 },
                { header: '数字2', dataIndex: 'NUMTWO', width: 100 },
                { header: '日期1', dataIndex: 'DATEONE', width: 100 },
                { header: '人员1', dataIndex: 'USERNAMEONE', width: 100 },
                { header: '日期2', dataIndex: 'DATETWO', width: 100 },
                { header: '人员2', dataIndex: 'USERNAMETWO', width: 100 }
        ],
        listeners:
        {
            'itemdblclick': function (view, record, item, index, e) {
                opencenterwin("/OrderManager/Create?ordercode=" + record.get("CODE") + "&busiitemcode=" + record.get("BUSIITEMCODE"), 1600, 900);
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

    opencenterwin("/OrderManager/Create?ordercode=" + recs[0].get("CODE") + "&busiitemcode=" + recs[0].get("BUSIITEMCODE"), 1600, 900);
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
    Ext.getCmp("combo_ENTRUST").setValue("");
    Ext.getCmp("start_date").setValue("");
    Ext.getCmp("end_date").setValue("");
    Ext.getCmp("combo_wtdw").setValue("");
    Ext.getCmp("field_CODE").setValue("");
    Ext.getCmp("combo_BUSIITEMCODE").setValue("");
    Ext.getCmp("start_date2").setValue("");
    Ext.getCmp("end_date2").setValue("");
    Ext.getCmp("combo_ENABLED_S").setValue("");
}

function seniorOrderM(pgbar) {
    if (Ext.getCmp('win_seniorOrderM')) {
        Ext.getCmp('win_seniorOrderM').expand();
        return;
    }

    var field_TEXTONE = Ext.create('Ext.form.field.Text', {
        id: 'field_TEXTONE',
        name: 'TEXTONE',//tabIndex: 5,        
        fieldLabel: '文本1'
    });
    var field_TEXTTWO = Ext.create('Ext.form.field.Text', {
        id: 'field_TEXTTWO',
        name: 'TEXTTWO',//tabIndex: 5,        
        fieldLabel: '文本2'
    });

    var field_NUMONE = Ext.create('Ext.form.field.Number', {
        id: 'field_NUMONE',
        name: 'NUMONE', hideTrigger: true,//tabIndex: 5,        
        fieldLabel: '数字1'
    });
    var field_NUMTWO = Ext.create('Ext.form.field.Number', {
        id: 'field_NUMTWO',
        name: 'NUMTWO', hideTrigger: true,//tabIndex: 5,        
        fieldLabel: '数字2'
    });

    //日期1
    var field_DATEONE = Ext.create('Ext.form.field.Date', {
        id: 'field_DATEONE',
        name: 'DATEONE', format: 'Y-m-d',
        fieldLabel: '日期1',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue != "") {
                    field_USERNAMEONE.setValue(curuserRealname);
                    field_USERIDONE.setValue(curuserId);
                } else {
                    field_USERNAMEONE.setValue("");
                    field_USERIDONE.setValue("");
                }
            }
        }
    });
    //人员1
    var field_USERNAMEONE = Ext.create('Ext.form.field.Text', {
        id: 'field_USERNAMEONE',
        name: 'USERNAMEONE',
        fieldLabel: '人员1'
    });
    var field_USERIDONE = Ext.create('Ext.form.field.Hidden', { name: 'USERIDONE' });

    //日期2
    var field_DATETWO = Ext.create('Ext.form.field.Date', {
        id: 'field_DATETWO',
        name: 'DATETWO', format: 'Y-m-d',
        fieldLabel: '日期2',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue != "") {
                    field_USERNAMETWO.setValue(curuserRealname);
                    field_USERIDTWO.setValue(curuserId);
                } else {
                    field_USERNAMETWO.setValue("");
                    field_USERIDTWO.setValue("");
                }
            }
        }
    });
    //人员2
    var field_USERNAMETWO = Ext.create('Ext.form.field.Text', {
        id: 'field_USERNAMETWO',
        name: 'USERNAMETWO',
        fieldLabel: '人员2'
    });
    var field_USERIDTWO = Ext.create('Ext.form.field.Hidden', { name: 'USERIDTWO' });


    var f_formpanel = Ext.create('Ext.form.Panel', {
        id: "f_formpanel",
        minHeight: 140,
        border: 0,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: .25,
            labelAlign: 'right',
            labelSeparator: '',
            msgTarget: 'under',
            validateOnBlur: false,
            validateOnChange: false
        },
        items: [
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_TEXTONE, field_NUMONE, field_DATEONE, field_USERNAMEONE] },
            { layout: 'column', height: 42, border: 0, items: [field_TEXTTWO, field_NUMTWO, field_DATETWO, field_USERNAMETWO] },
            field_USERIDONE, field_USERIDTWO
        ]
    });

    var win_seniorOrderM = Ext.create("Ext.window.Window", {
        id: 'win_seniorOrderM',
        title: "高级查询",
        width: 1000,
        height: 150,
        collapsible: true,
        //modal: true,
        items: [f_formpanel],
        buttonAlign: 'center',
        buttons: [{
            text: '<i class="fa fa-search"></i>&nbsp;查 询', handler: function () {               
                Ext.getCmp("pgbar").moveFirst();
                win_seniorOrderM.collapse();
            }
        }]
    });
    win_seniorOrderM.show();
    change_ini_label();
}

function change_ini_label() {
    if (Ext.getCmp('win_seniorOrderM')) {
        Ext.getCmp("field_TEXTONE").setFieldLabel("文本1"); Ext.getCmp("field_TEXTTWO").setFieldLabel("文本2");
        Ext.getCmp("field_NUMONE").setFieldLabel("数字1"); Ext.getCmp("field_NUMTWO").setFieldLabel("数字2");
        Ext.getCmp("field_DATEONE").setFieldLabel("日期1"); Ext.getCmp("field_DATETWO").setFieldLabel("日期2");
        Ext.getCmp("field_USERNAMEONE").setFieldLabel("人员1"); Ext.getCmp("field_USERNAMETWO").setFieldLabel("人员2");

        var f_entrusttype = Ext.getCmp("combo_ENTRUST").getValue();
        var f_busiitemcode = Ext.getCmp("combo_BUSIITEMCODE").getValue();

        Ext.Ajax.request({
            url: "/OrderManager/Getlabelname",
            params: { entrusttype: f_entrusttype, busiitemcode: f_busiitemcode },
            success: function (response, opts) {
                var json = Ext.decode(response.responseText);
                var jsonobj = json.fieldcolumn;

                for (var i = 0; i < jsonobj.length; i++) {
                    switch (jsonobj[i].ORIGINNAME) {
                        case "文本1":
                            Ext.getCmp("field_TEXTONE").setFieldLabel(jsonobj[i].CONFIGNAME);
                            break;
                        case "文本2":
                            Ext.getCmp("field_TEXTTWO").setFieldLabel(jsonobj[i].CONFIGNAME);
                            break;
                        case "数字1":
                            Ext.getCmp("field_NUMONE").setFieldLabel(jsonobj[i].CONFIGNAME);
                            break;
                        case "数字2":
                            Ext.getCmp("field_NUMTWO").setFieldLabel(jsonobj[i].CONFIGNAME);
                            break;
                        case "日期1":
                            Ext.getCmp("field_DATEONE").setFieldLabel(jsonobj[i].CONFIGNAME);
                            break;
                        case "日期2":
                            Ext.getCmp("field_DATETWO").setFieldLabel(jsonobj[i].CONFIGNAME);
                            break;
                        case "人员1":
                            Ext.getCmp("field_USERNAMEONE").setFieldLabel(jsonobj[i].CONFIGNAME);
                            break;
                        case "人员2":
                            Ext.getCmp("field_USERNAMETWO").setFieldLabel(jsonobj[i].CONFIGNAME);
                            break;
                        default:
                            break;
                    }
                }

            }
        });

    }
}

function CompleteOrderM() {
    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要操作的记录！');
        return;
    }

    if (recs[0].get("SUBMITTIME") != "" && recs[0].get("SUBMITTIME") != null) {
        Ext.MessageBox.alert('提示', '订单业务已完成,不能再次完成！');
        return;
    }

    Ext.MessageBox.confirm("提示", "确定要业务完成吗？", function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/OrderManager/CompleteOrderM',
                params: { ordercode: recs[0].get("CODE") },
                success: function (response, success, option) {
                    var res = Ext.decode(response.responseText);
                    if (res.success) {
                        Ext.MessageBox.alert('提示', '业务完成成功！', function () {
                            Ext.getCmp("pgbar").moveFirst();
                        });
                    }
                    else {
                        if (res.flag == "E") { Ext.MessageBox.alert('提示', '订单业务已完成,不能再次完成！'); }
                        else {
                            Ext.MessageBox.alert('提示', '业务完成失败！');
                        }
                    }
                }
            });
        }
    });
}

function DeleteOrderM() {
    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要删除的记录！');
        return;
    }

    var bf = false; ordercodes = "";
    Ext.each(recs, function (rec) {
        if (rec.get("SUBMITTIME") != "" && rec.get("SUBMITTIME") != null) {
            bf = true;
        } else {
            ordercodes += "'" + rec.get("CODE") + "',";
        }
    });
    if (bf) {
        Ext.MessageBox.alert('提示', '订单业务已完成,不能删除！');
        return;
    }
    ordercodes = ordercodes.substr(0, ordercodes.length - 1);

    Ext.MessageBox.confirm("提示", "确定要删除所选择的记录吗？", function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/OrderManager/DeleteOrderM',
                params: { ordercodes: ordercodes },
                success: function (response, success, option) {
                    var res = Ext.decode(response.responseText);
                    if (res.success) {
                        Ext.MessageBox.alert('提示', '删除成功！', function () {
                            Ext.getCmp("pgbar").moveFirst();
                        });
                    }
                    else {
                        if (res.flag == "E") { Ext.MessageBox.alert('提示', '订单业务已完成,不能删除！'); }
                        else {
                            Ext.MessageBox.alert('提示', '删除失败！');
                        }
                    }
                }
            });
        }
    });
}

function ExportOrderM() {
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据导出中，请稍等..." });
    myMask.show();

    var TEXTONE = ""; var TEXTTWO = ""; var NUMONE = ""; var NUMTWO = "";
    var DATEONE = ""; var DATETWO = ""; var USERNAMEONE = ""; var USERNAMETWO = "";

    if (Ext.getCmp('win_seniorOrderM')) {
        TEXTONE = Ext.getCmp('field_TEXTONE').getValue(); TEXTTWO = Ext.getCmp('field_TEXTTWO').getValue();
        NUMONE = Ext.getCmp('field_NUMONE').getValue(); NUMTWO = Ext.getCmp('field_NUMTWO').getValue();
        DATEONE = Ext.Date.format(Ext.getCmp('field_DATEONE').getValue(), 'Y-m-d H:i:s'); DATETWO = Ext.Date.format(Ext.getCmp('field_DATETWO').getValue(), 'Y-m-d H:i:s');
        USERNAMEONE = Ext.getCmp('field_USERNAMEONE').getValue(); USERNAMETWO = Ext.getCmp('field_USERNAMETWO').getValue();
    }

    var data = {
        common_data_entrust: JSON.stringify(common_data_entrust),
        OnlySelf: Ext.get('OnlySelfi').el.dom.className,
        BUSIUNITCODE: Ext.getCmp('s_combo_busiunitname').getValue(), CUSNO: Ext.getCmp('field_CUSNO').getValue(), entrusttype: Ext.getCmp('combo_ENTRUST').getValue(),
        start_date: Ext.Date.format(Ext.getCmp("start_date").getValue(), 'Y-m-d H:i:s'), end_date: Ext.Date.format(Ext.getCmp("end_date").getValue(), 'Y-m-d H:i:s'),
        CUSTOMERCODE: Ext.getCmp('combo_wtdw').getValue(), CODE: Ext.getCmp('field_CODE').getValue(), busiitemcode: Ext.getCmp('combo_BUSIITEMCODE').getValue(),
        start_date2: Ext.Date.format(Ext.getCmp("start_date2").getValue(), 'Y-m-d H:i:s'), end_date2: Ext.Date.format(Ext.getCmp("end_date2").getValue(), 'Y-m-d H:i:s'),
        TEXTONE: TEXTONE, TEXTTWO: TEXTTWO, NUMONE: NUMONE, NUMTWO: NUMTWO,
        DATEONE: DATEONE, DATETWO: DATETWO, USERNAMEONE: USERNAMEONE, USERNAMETWO: USERNAMETWO
    }

    Ext.Ajax.request({
        url: '/OrderManager/ExportOrderM',
        method: 'POST',
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