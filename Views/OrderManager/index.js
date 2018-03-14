var common_data_jydw = [], common_data_wtdw = [];
var store_wtlx_CustomsService;

Ext.onReady(function () {
    Ext.Ajax.request({//对公共基础数据发起一次请求
        url: "/Common/Ini_Base_Data",
        params: { ParaType: 'OrderManeger' },
        success: function (response, option) {
            var commondata = Ext.decode(response.responseText);
            common_data_jydw = commondata.jydw;//经营单位
            common_data_wtdw = commondata.wtdw;//委托单位

            store_wtlx_CustomsService = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: wtlx_js_data_CustomsService });

            initSearch_OrderM();
            //bindgrid();
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
        fieldLabel: '经营单位', tabIndex: 1,
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
        fieldLabel: '企业编号', tabIndex: 2,
        name: 'CUSNO'
    });

    var start_date = Ext.create('Ext.form.field.Date', { id: 'start_date', name: 'start_date', format: 'Y-m-d', emptyText: '开始日期', tabIndex: 3, flex: .5, margin: 0 });
    var end_date = Ext.create('Ext.form.field.Date', { id: 'end_date', name: 'end_date', format: 'Y-m-d', emptyText: '结束日期', tabIndex: 4, flex: .5, margin: 0 });
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
        fieldLabel: '委托单位', tabIndex: 5,
        displayField: 'CODENAME',
        valueField: 'CODE',
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local'
    });


    //订单编号
    var field_CODE = Ext.create('Ext.form.field.Text', {
        id: 'field_CODE',
        fieldLabel: '订单编号', tabIndex: 6,
        name: 'CODE'
    });

    var start_date2 = Ext.create('Ext.form.field.Date', { id: 'start_date2', name: 'start_date2', format: 'Y-m-d', emptyText: '开始日期', tabIndex: 7, flex: .5, margin: 0 });
    var end_date2 = Ext.create('Ext.form.field.Date', { id: 'end_date2', name: 'end_date2', format: 'Y-m-d', emptyText: '结束日期', tabIndex: 8, flex: .5, margin: 0 });
    var condate2 = Ext.create('Ext.form.FieldContainer', { fieldLabel: '维护日期', layout: 'hbox', columnWidth: .33, items: [start_date2, end_date2] });


    var formpanel = Ext.create('Ext.form.Panel', {
        id: 'formpanel',
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.33,
            labelWidth: 80
        },
        items: [
        { layout: 'column', border: 0, items: [s_combo_busiunitname, field_CUSNO, condate] },
        { layout: 'column', border: 0, items: [combo_wtdw, field_CODE, condate2] }
        ]
    });
}
