var common_data_jydw = [], common_data_wtdw = [];
var store_wtlx_CustomsService;

Ext.onReady(function () {
    Ext.Ajax.request({//对公共基础数据发起一次请求
        url: "/Common/Ini_Base_Data",
        params: { ParaType: 'CustomsService' },
        success: function (response, option) {
            var commondata = Ext.decode(response.responseText);
            common_data_jydw = commondata.jydw;//经营单位
            common_data_wtdw = commondata.wtdw;//委托单位

            store_wtlx_CustomsService = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: wtlx_js_data_CustomsService });

            initSearch_CusService();
            bindgrid();
        }
    });
});

function initSearch_CusService() {

    //经营单位
    var store_busiunitname = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_jydw
    });
    var s_combo_busiunitname = Ext.create('Ext.form.field.ComboBox', {
        id: 's_combo_busiunitname',
        name: 'BUSIUNITNAME',
        store: store_busiunitname,
        fieldLabel: '经营单位',tabIndex: 1,
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
        { layout: 'column', border: 0,  items: [s_combo_busiunitname, field_CUSNO, condate] },
        { layout: 'column', border: 0,  items: [combo_wtdw, field_CODE, condate2] }
        ]
    });
}

function bindgrid() {
    Ext.regModel('TRADE', {
        fields: ['ID', 'BUSITYPE', 'CREATETIME', 'SUBMITTIME', 'ENTRUSTTYPE', 'CUSTOMERCODE', 'CUSTOMERNAME', 'CLEARUNIT', 'CLEARUNITNAME'
                , 'BUSIUNITCODE', 'BUSIUNITNAME', 'CODE', 'CUSNO', 'DOREQUEST', 'CLEARREMARK', 'DLF', 'SJF', 'QTF', 'SUMF']
    })
    var store_Trade = Ext.create('Ext.data.JsonStore', {
        model: 'TRADE',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/OrderCustomsService/LoadList',
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
                    busitypeid: 70,
                    OnlySelf: Ext.get('OnlySelfi').el.dom.className,
                    BUSIUNITCODE: Ext.getCmp('s_combo_busiunitname').getValue(),CUSNO: Ext.getCmp('field_CUSNO').getValue(), 
                    start_date: Ext.Date.format(Ext.getCmp("start_date").getValue(), 'Y-m-d H:i:s'), end_date: Ext.Date.format(Ext.getCmp("end_date").getValue(), 'Y-m-d H:i:s'),
                    CUSTOMERCODE: Ext.getCmp('combo_wtdw').getValue(), CODE: Ext.getCmp('field_CODE').getValue(),
                    start_date2: Ext.Date.format(Ext.getCmp("start_date2").getValue(), 'Y-m-d H:i:s'), end_date2: Ext.Date.format(Ext.getCmp("end_date2").getValue(), 'Y-m-d H:i:s')
                }
            }
        }
    })
    Ext.tip.QuickTipManager.init();//初始化全局的QuickTips，为创建QuickTips做准备onclick="ShowList();"
    var pgbar = Ext.create('Ext.toolbar.Paging', {
        id: 'pgbar',
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_Trade,
        displayInfo: true
    })
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
        { header: '维护时间', dataIndex: 'SUBMITTIME', width: 120, locked: true },
        { header: '创建时间', dataIndex: 'CREATETIME', width: 120, locked: true },
        {
            header: '委托类型', dataIndex: 'ENTRUSTTYPE', width: 100, locked: true, renderer: function renderOrder(value, cellmeta, record, rowIndex, columnIndex, store) {
                var rtn = "";
                var rec = store_wtlx_CustomsService.findRecord('CODE', value);
                if (rec) {
                    rtn = rec.get("NAME");
                }
                return rtn;
            }
        },
        { header: '委托单位', dataIndex: 'CUSTOMERNAME', width: 130, locked: true },
        { header: '结算单位', dataIndex: 'CLEARUNITNAME', width: 130, locked: true },
        { header: '经营单位', dataIndex: 'BUSIUNITNAME', width: 130, locked: true },
        { header: '企业编号', dataIndex: 'CUSNO', width: 100 },
        { header: '订单编号', dataIndex: 'CODE', width: 100 },
        { header: '操作需求', dataIndex: 'DOREQUEST', width: 130 },
        { header: '结算备注', dataIndex: 'CLEARREMARK', width: 130 },
        { header: '代理费', dataIndex: 'DLF', width: 80 },
        { header: '输机费', dataIndex: 'SJF', width: 80 },
        { header: '其他费', dataIndex: 'QTF', width: 80 },
        { header: '合计', dataIndex: 'SUMF', width: 80 }
        ],
        listeners:
        {
            'itemdblclick': function (view, record, item, index, e) {
                opencenterwin("/OrderCustomsService/Create?ordercode=" + record.get("CODE"), 1600, 900);
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
    opencenterwin("/OrderCustomsService/Create", 1600, 900);
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
    Ext.getCmp("start_date").setValue("");
    Ext.getCmp("end_date").setValue("");
    Ext.getCmp("combo_wtdw").setValue("");
    Ext.getCmp("field_CODE").setValue("");
    Ext.getCmp("start_date2").setValue("");
    Ext.getCmp("end_date2").setValue("");
}

function ExportCustoms() {
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据导出中，请稍等..." });
    myMask.show();

    var data = {
        wtlx_cus: JSON.stringify(wtlx_js_data_CustomsService),
        busitypeid: 70,
        OnlySelf: Ext.get('OnlySelfi').el.dom.className,
        BUSIUNITCODE: Ext.getCmp('s_combo_busiunitname').getValue(), CUSNO: Ext.getCmp('field_CUSNO').getValue(),
        start_date: Ext.Date.format(Ext.getCmp("start_date").getValue(), 'Y-m-d H:i:s'), end_date: Ext.Date.format(Ext.getCmp("end_date").getValue(), 'Y-m-d H:i:s'),
        CUSTOMERCODE: Ext.getCmp('combo_wtdw').getValue(), CODE: Ext.getCmp('field_CODE').getValue(),
        start_date2: Ext.Date.format(Ext.getCmp("start_date2").getValue(), 'Y-m-d H:i:s'), end_date2: Ext.Date.format(Ext.getCmp("end_date2").getValue(), 'Y-m-d H:i:s')
    }

    Ext.Ajax.request({
        url: '/OrderCustomsService/ExportCustomsList',
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

function DeleteCustoms() {
    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要删除的记录！');
        return;
    }

    Ext.MessageBox.confirm("提示", "确定要删除所选择的记录吗？", function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/OrderCustomsService/DeleteCustoms',
                params: { ordercode: recs[0].get("CODE") },
                success: function (response, success, option) {
                    var res = Ext.decode(response.responseText);
                    if (res.success) {
                        Ext.MessageBox.alert('提示', '删除成功！', function () {
                            Ext.getCmp("pgbar").moveFirst();
                        });                       
                    }
                    else {
                        if (res.flag == "E") { Ext.MessageBox.alert('提示', '存在正在进行中的费用，不能删除！'); }
                        else {
                            Ext.MessageBox.alert('提示', '删除失败！');
                        }
                    }
                }
            });
        }
    });
}