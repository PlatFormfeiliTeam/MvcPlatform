

var common_data_jydw = [], common_data_unit = [];
var store_unit, store_optionstatus, store_status;//中文所需
var gridpanel_lj, gridpanel_cp, gridpanel_lj_Go, gridpanel_cp_Go;

Ext.onReady(function () {
    Ext.Ajax.request({
        url: "/Common/Ini_Base_Data",
        params: { ParaType: 'recordinfo' },
        success: function (response, opts) {
            var commondata = Ext.decode(response.responseText);
            common_data_recordid = commondata.recordid;//账册号
            common_data_unit = commondata.unit;//单位

            store_unit = Ext.create('Ext.data.JsonStore', {
                fields: ['CODE', 'NAME'],
                data: common_data_unit
            });

            initSearch_Sum();
            itemsbind_Sum();
        }
    });
});



function initSearch_Sum() {
    //账册号
    var s_store_recordid = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_recordid
    });
    var s_combo_recordid = Ext.create('Ext.form.field.ComboBox', {
        id: 's_combo_recordid',
        editable: false,
        store: s_store_recordid,
        fieldLabel: '账册号',
        displayField: 'NAME',
        name: 'recordid',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local'
    });
    //项号
    var s_field_ITEMNO = Ext.create('Ext.form.field.Text', {
        id: 's_field_ITEMNO',
        fieldLabel: '项号'
    });

    //申报日期
    var s_date_start = Ext.create('Ext.form.field.Date', {
        id: 's_date_start',
        margin: 0,
        emptyText: '开始日期', flex: .5,
        format: 'Y-m-d'
    })
    var s_date_end = Ext.create('Ext.form.field.Date', {
        id: 's_date_end',
        margin: 0,
        emptyText: '结束日期', flex: .5,
        format: 'Y-m-d'
    })
    var s_date_container = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        fieldLabel: '申报日期',
        columnWidth: .25,
        items: [s_date_start, s_date_end]
    }

    //进出类型
    var s_store_inout_type = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: [{ "CODE": "1", "NAME": "进口" }, { "CODE": "0", "NAME": "出口" }]
    });
    var s_combo_inout_type = Ext.create('Ext.form.field.ComboBox', {
        id: 's_combo_inout_type',
        editable: false,
        store: s_store_inout_type,
        fieldLabel: '进出类型',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local'
    });
    var formpanel = Ext.create('Ext.form.Panel', {
        id: 'formpanel',
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5', labelWidth: 60, columnWidth: .25,
        },
        items: [
        { layout: 'column', border: 0, margin: '5 0 0 0', items: [s_combo_recordid, s_field_ITEMNO, s_combo_inout_type, s_date_container] }
        ]
    });
}

function itemsbind_Sum() {
    Ext.regModel('RecrodDetail_SUM', {
        fields: ['RECORDCODE', 'ITEMNO', 'INTERNALTYPE', 'INTERNALTYPENAME', 'TRADEMETHOD', 'COMMODITYNAME'
            , 'CURRENCY', 'CADUNIT', 'CADQUANTITY', 'ITEMNOATTRIBUTE', 'TOTALPRICE']
    });

    var store_sum = Ext.create('Ext.data.JsonStore', {
        model: 'RecrodDetail_SUM',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/RecordInfor/loadRecordDetail_SUM',
            reader: {
                root: 'rows',
                type: 'json',
                totalProperty: 'total'
            }
        },
        autoLoad: true,
        listeners: {
            beforeload: function () {
                store_sum.getProxy().extraParams = {
                    RECORDINFORID: Ext.getCmp('s_combo_recordid').getValue(), ITEMNO: Ext.getCmp("s_field_ITEMNO").getValue(),
                    DATE_START: Ext.Date.format(Ext.getCmp("s_date_start").getValue(), 'Y-m-d H:i:s'), DATE_END: Ext.Date.format(Ext.getCmp("s_date_end").getValue(), 'Y-m-d H:i:s'),
                    INOUT_TYPE: Ext.getCmp('s_combo_inout_type').getValue()
                }
            }
        }
    });

    var pgbar = Ext.create('Ext.toolbar.Paging', {
        id: 'pgbar',
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_sum,
        displayInfo: true
    })
    var gridpanel = Ext.create('Ext.grid.Panel', {
        id: 'gridpanel',
        store: store_sum,
        renderTo: 'appConId',
        height: 500,
        selModel: { selType: 'checkboxmodel' },
        bbar: pgbar,
        enableColumnHide: false,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: '进出类型', dataIndex: 'INTERNALTYPE', width: 80, hidden: true },
        { header: '账册号', dataIndex: 'RECORDCODE', width: 110 },
        { header: '项号', dataIndex: 'ITEMNO', width: 80 },
        { header: '进出类型', dataIndex: 'INTERNALTYPENAME', width: 70 },
        { header: '贸易方式', dataIndex: 'TRADEMETHOD', width: 80 },
        { header: '项号属性', dataIndex: 'ITEMNOATTRIBUTE', width: 70 },
        { header: '商品名称', dataIndex: 'COMMODITYNAME', width: 150 },
        { header: '成交数量', dataIndex: 'CADQUANTITY', width: 80 },
        { header: '成交总价', dataIndex: 'TOTALPRICE', width: 90 },
        { header: '成交单位', dataIndex: 'CADUNIT', width: 80, renderer: renderOrder },
        { header: '币别', dataIndex: 'CURRENCY', width: 80 }
        ],
        listeners:
        {
            'itemdblclick': function (view, record, item, index, e) {
                Open();
            }
        },
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });
}

function Select() {
    Ext.getCmp('pgbar').moveFirst();
}

function Reset() {
    Ext.getCmp("s_combo_recordid").setValue("");
    Ext.getCmp("s_field_ITEMNO").setValue("");
    Ext.getCmp("s_date_start").setValue("");
    Ext.getCmp("s_date_end").setValue("");
    Ext.getCmp("s_combo_inout_type").setValue("");
}

function Open() {
    var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
    if (recs.length != 1) {
        Ext.Msg.alert("提示", "请选择一笔记录!");
        return;
    }
    form_ini_detail(recs);
    grid_ini_detail();
    var win = Ext.create("Ext.window.Window", {
        title: '申报明细',
        width: 900,
        height: 550,
        modal: true,
        items: [Ext.getCmp('f_formpanel'), Ext.getCmp('gridpanel_d')]
    });
    win.show();
}

function form_ini_detail(recs) {
    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;汇总信息</span></h4>'
    }
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo, '->']
    })

    //账册号
    var f_field_recordid = Ext.create('Ext.form.field.Text', {
        id: 'f_field_recordid', name: 'f_field_recordid', fieldLabel: '账册号', readOnly: true, value: recs[0].get("RECORDCODE")
    });


    //项号
    var f_field_ITEMNO = Ext.create('Ext.form.field.Text', {
        id: 'f_field_ITEMNO', name: 'f_field_ITEMNO', fieldLabel: '项号', readOnly: true, value: recs[0].get("ITEMNO")
    });

    //项号属性
    var f_field_ITEMNOATTRIBUTE = Ext.create('Ext.form.field.Text', {
        id: 'f_field_ITEMNOATTRIBUTE', name: 'f_field_ITEMNOATTRIBUTE', fieldLabel: '项号属性', readOnly: true, value: recs[0].get("ITEMNOATTRIBUTE")
    });

    //商品名称
    var f_field_COMMODITYNAME = Ext.create('Ext.form.field.Text', {
        id: 'f_field_COMMODITYNAME', name: 'f_field_COMMODITYNAME', fieldLabel: '商品名称', readOnly: true, value: recs[0].get("COMMODITYNAME")

    });

    //进出类型
    var f_field_inout_type = Ext.create('Ext.form.field.Text', {
        id: 'f_field_inout_type', name: 'f_field_inout_type', fieldLabel: '进(出)', readOnly: true, value: recs[0].get("INTERNALTYPENAME")
    });

    //贸易方式
    var f_field_TRADEMETHOD = Ext.create('Ext.form.field.Text', {
        id: 'f_field_TRADEMETHOD', name: 'f_field_TRADEMETHOD', fieldLabel: '贸易方式', readOnly: true, value: recs[0].get("TRADEMETHOD")
    });


    //成交单位
    var f_store_UNIT = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME', 'CODENAME'],
        data: common_data_unit
    });
    var f_combo_UNIT = Ext.create('Ext.form.field.ComboBox', {
        id: 'f_combo_UNIT', name: 'f_combo_UNIT', readOnly: true,
        store: f_store_UNIT,
        fieldLabel: '成交单位',
        displayField: 'CODENAME',
        valueField: 'CODE',
        hideTrigger: true,
        triggerAction: 'all',
        forceSelection: true,
        queryMode: 'local',
        anyMatch: true, value: recs[0].get("CADUNIT")
    });

    //成交数量
    var f_field_CADQUANTITY = Ext.create('Ext.form.field.Text', {
        id: 'f_field_CADQUANTITY', name: 'f_field_CADQUANTITY', fieldLabel: '成交数量', readOnly: true, value: recs[0].get("CADQUANTITY")
    });

    //成交总价
    var f_field_TOTALPRICE = Ext.create('Ext.form.field.Text', {
        id: 'f_field_TOTALPRICE', name: 'f_field_TOTALPRICE', fieldLabel: '成交总价', readOnly: true, value: recs[0].get("TOTALPRICE")
    });

    //币别
    var f_field_CURRENCY = Ext.create('Ext.form.field.Text', {
        id: 'f_field_CURRENCY', name: 'f_field_CURRENCY', fieldLabel: '币别', readOnly: true, value: recs[0].get("CURRENCY")
    });

    //申报日期
    var f_date_start = Ext.create('Ext.form.field.Date', {
        id: 'f_date_start', name: 'f_date_start', flex: .5, readOnly: true,
        format: 'Y-m-d', value: Ext.getCmp('s_date_start').getValue()
    });
    var f_date_end = Ext.create('Ext.form.field.Date', {
        id: 'f_date_end', name: 'f_date_end', flex: .5, readOnly: true,
        format: 'Y-m-d', value: Ext.getCmp('s_date_end').getValue()
    });
    var f_date_container = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        fieldLabel: '申报日期', margin: 0,
        columnWidth: .5,
        items: [f_date_start, f_date_end]
    }

    var f_formpanel = Ext.create('Ext.form.Panel', {
        id: 'f_formpanel',
        minHeight: 170,
        border: 0,
        tbar: tbar,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: .25,
            labelAlign: 'right',
            labelSeparator: ''
        },
        items: [
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [f_field_recordid, f_field_ITEMNO, f_date_container] },
                { layout: 'column', height: 42, border: 0, items: [f_field_COMMODITYNAME, f_field_inout_type, f_field_TRADEMETHOD, f_field_ITEMNOATTRIBUTE] },
                { layout: 'column', height: 42, border: 0, items: [f_field_CADQUANTITY, f_field_TOTALPRICE, f_combo_UNIT, f_field_CURRENCY] }
        ]
    });
}

function grid_ini_detail() {

    var label_baseinfo_d = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;明细信息</span></h4>'
    }
    var tbar_d = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo_d, '->']
    })

    Ext.regModel('RecrodDetail_SUM_D', {
        fields: ['DECLARATIONCODE', 'CUSTOMSSTATUS', 'REPUNITNAME', 'REPTIME', 'COMMODITYNO'
            , 'CADQUANTITY', 'LEGALUNIT', 'LEGALQUANTITY', 'TRANSMODEL', 'TRANSMODELNAME', 'TOTALPRICE']
    });

    var store_sum_d = Ext.create('Ext.data.JsonStore', {
        model: 'RecrodDetail_SUM_D',
        pageSize: 10,
        proxy: {
            type: 'ajax',
            url: '/RecordInfor/loadRecordDetail_SUM_D',
            reader: {
                root: 'rows',
                type: 'json',
                totalProperty: 'total'
            }
        },
        autoLoad: true,
        listeners: {
            beforeload: function () {
                store_sum_d.getProxy().extraParams = Ext.getCmp('f_formpanel').getForm().getValues();
            }
        }
    });

    var pgbar_d = Ext.create('Ext.toolbar.Paging', {
        id: 'pgbar_d',
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_sum_d,
        displayInfo: true
    })
    var gridpanel_d = Ext.create('Ext.grid.Panel', {
        id: 'gridpanel_d',
        store: store_sum_d,
        height: 350,
        tbar: tbar_d,
        bbar: pgbar_d,
        enableColumnHide: false,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: '申报单位', dataIndex: 'REPUNITNAME', width: 250 },
        { header: '海关状态', dataIndex: 'CUSTOMSSTATUS', width: 80 },
        { header: '报关单号', dataIndex: 'DECLARATIONCODE', width: 180 },
        { header: '成交数量', dataIndex: 'CADQUANTITY', width: 80 },
        { header: '成交总价', dataIndex: 'TOTALPRICE', width: 90 },
        { header: '申报时间', dataIndex: 'REPTIME', width: 100 },
        { header: 'HS编码', dataIndex: 'COMMODITYNO', width: 100 },
        { header: '法定数量', dataIndex: 'LEGALQUANTITY', width: 75 },
        { header: '法定单位', dataIndex: 'LEGALUNIT', width: 75, renderer: renderOrder },
        { header: '运输方式', dataIndex: 'TRANSMODELNAME', width: 150 }
        ],
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });
}

function Export_SUM() {
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据导出中，请稍等..." });
    myMask.show();

    var data = {
        UNIT: JSON.stringify(common_data_unit),
        RECORDINFORID: Ext.getCmp('s_combo_recordid').getValue(), ITEMNO: Ext.getCmp("s_field_ITEMNO").getValue(),
        DATE_START: Ext.Date.format(Ext.getCmp("s_date_start").getValue(), 'Y-m-d H:i:s'), DATE_END: Ext.Date.format(Ext.getCmp("s_date_end").getValue(), 'Y-m-d H:i:s'),
        INOUT_TYPE: Ext.getCmp('s_combo_inout_type').getValue()
    }

    Ext.Ajax.request({
        url: '/RecordInfor/Export_SUM',
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