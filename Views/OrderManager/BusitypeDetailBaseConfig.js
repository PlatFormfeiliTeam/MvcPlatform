var common_data_jydw = [], common_data_wtdw = [], common_data_busi = [];
var store_busitype; var columns_order = [];
Ext.onReady(function () {
    Ext.Ajax.request({//对公共基础数据发起一次请求
        url: "/Common/Ini_Base_Data",
        params: { ParaType: 'OrderManager' },
        success: function (response, option) {
            var commondata = Ext.decode(response.responseText);
            common_data_busi = commondata.busi;//业务类型
            
            store_busitype = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME', 'CODENAME'], data: common_data_busi });

            initSearch_CustomsConfig();
            bindgrid();
        }
    });
});

function initSearch_CustomsConfig() {

    //业务类型
    //var store_busitype = Ext.create('Ext.data.JsonStore', {
    //    fields: ['CODE', 'NAME', 'CODENAME'],
    //    data: common_data_busi
    //});

    var combo_BUSITYPE_S = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_BUSITYPE_S',
        name: 'BUSITYPEID',
        store: store_busitype,
        fieldLabel: '业务类型',//tabIndex: 3
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
            }//,
            //change: function (combo, newValue, oldValue, eOpts) {
            //    combo_ENTRUSTTYPE_S.reset();
            //    Ext.Ajax.request({
            //        url: "/OrderManager/Ini_Base_Data_BUSIITEM",
            //        params: { busitype: newValue },
            //        success: function (response, opts) {
            //            var commondata = Ext.decode(response.responseText);//业务细项
            //            store_ENTRUSTTYPE_S.loadData(commondata.ywxx);
            //        }
            //    });

            //}
        }
    });


    //业务细项
    //var store_ENTRUSTTYPE_S = Ext.create('Ext.data.JsonStore', {
    //    fields: ['CODE', 'NAME']
    //});

    //var combo_ENTRUSTTYPE_S = Ext.create('Ext.form.field.ComboBox', {
    //    id: 'combo_ENTRUSTTYPE_S',
    //    name: 'ENTRUSTTYPE',
    //    store: store_ENTRUSTTYPE_S,
    //    fieldLabel: '业务细项',//tabIndex: 3
    //    displayField: 'NAME',
    //    valueField: 'CODE',
    //    triggerAction: 'all',
    //    queryMode: 'local',
    //    hideTrigger: true,
    //    anyMatch: true,
    //    queryMode: 'local',
    //    listeners: {
    //        focus: function (cb) {
    //            if (!cb.getValue()) {
    //                cb.clearInvalid();
    //                cb.store.clearFilter();
    //                cb.expand()
    //            };
    //        }
    //    }
    //});

    var field_BUSIITEMCODE_S = Ext.create('Ext.form.field.Text', {
        id: 'field_BUSIITEMCODE_S',
        name: 'BUSIITEMCODE',
        fieldLabel: '业务细项代码'
    });

    var field_BUSIITEMNAME_S = Ext.create('Ext.form.field.Text', {
        id: 'field_BUSIITEMNAME_S',
        name: 'BUSIITEMNAME',
        fieldLabel: '业务细项名称'
    });

    //启禁用
    var store_ENABLE_S = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: [{ "CODE": 0, "NAME": "否" }, { "CODE": 1, "NAME": "是" }]
    });
    var combo_ENABLE_S = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ENABLE_S',
        name: 'ENABLE',
        store: store_ENABLE_S,
        queryMode: 'local',
        anyMatch: true,
        hideTrigger: true,
        fieldLabel: '是否启用',
        displayField: 'NAME',
        valueField: 'CODE',
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

    var formpanel = Ext.create('Ext.form.Panel', {
        id: 'formpanel',
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.25,
            labelWidth: 80
        },
        items: [
            { layout: 'column', border: 0, items: [combo_BUSITYPE_S, field_BUSIITEMCODE_S, field_BUSIITEMNAME_S, combo_ENABLE_S] }// combo_ENTRUSTTYPE_S,
        ]
    });

}

function bindgrid() {
    var store_Trade = Ext.create('Ext.data.JsonStore', {
        fields: ['BUSITYPECODE', 'BUSITYPENAME', 'BUSIITEMCODE', 'BUSIITEMNAME', 'STARTTIME', 'ENABLE', 'CREATEUSERNAME', 'ENABLEUSERNAME', 'REMARK', 'ID'],
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/OrderManager/LoadList_customsconfig',
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
                    busitypeid: Ext.getCmp('combo_BUSITYPE_S').getValue(), //entrusttype: Ext.getCmp('combo_ENTRUSTTYPE_S').getValue(),
                    busiitemcode: Ext.getCmp('field_BUSIITEMCODE_S').getValue(),
                    busiitemname: Ext.getCmp('field_BUSIITEMNAME_S').getValue(),
                    enable: Ext.getCmp('combo_ENABLE_S').getValue()
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
        columns: [
                { xtype: 'rownumberer', width: 35 },
                { header: 'ID', dataIndex: 'ID', hidden: true, locked: true },
                { header: '业务类型编号', dataIndex: 'BUSITYPECODE', width: 130 },
                { header: '业务类型名称', dataIndex: 'BUSITYPENAME', width: 150 },
                { header: '业务细项编号', dataIndex: 'BUSIITEMCODE', width: 130 },
                { header: '业务细项名称', dataIndex: 'BUSIITEMNAME', width: 150 },
                { header: '创建时间', dataIndex: 'STARTTIME', width: 160 },
                {
                    header: '是否启用', dataIndex: 'ENABLE', width: 95, renderer: function gridrender(value, cellmeta, record, rowIndex, columnIndex, stroe) {                        
                        return value == "1" ? '<span class="icon iconfont" style="font-size:12px;color:blue;">&#xe628;</span>' : '<span class="icon iconfont" style="font-size:12px;color:red;">&#xe634;</span>';
                    }
                },
                { header: '创建人', dataIndex: 'CREATEUSERNAME', width: 160 },
                { header: '启禁用人', dataIndex: 'ENABLEUSERNAME', width: 160 }
        ],
        listeners:
        {
            'itemdblclick': function (view, record, item, index, e) {
                EditCusConfig();
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

function Reset() {
    Ext.getCmp("combo_BUSITYPE_S").setValue("");
    //Ext.getCmp("combo_ENTRUSTTYPE_S").setValue("");
    Ext.getCmp("field_BUSIITEMCODE_S").setValue("");
    Ext.getCmp("field_BUSIITEMNAME_S").setValue("");
    Ext.getCmp("combo_ENABLE_S").setValue("");
}

function form_ini_win() {

    var field_id = Ext.create('Ext.form.field.Hidden', { id: 'ID', name: 'ID' });

    var combo_BUSITYPE = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_BUSITYPE',
        name: 'BUSITYPECODE',
        store: store_busitype,
        fieldLabel: '业务类型',//tabIndex: 3
        displayField: 'CODENAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        forceSelection: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                };
            },
            select: function (records) { field_BUSITYPENAME.setValue(records.rawValue.substr(0, records.rawValue.lastIndexOf('('))); }
        },
        allowBlank: false,
        blankText: '业务类型不可为空!',
    });
    var field_BUSITYPENAME = Ext.create('Ext.form.field.Hidden', { id: 'field_BUSITYPENAME', name: 'BUSITYPENAME' });


    var field_BUSIITEMCODE = Ext.create('Ext.form.field.Text', {
        id: 'field_BUSIITEMCODE',
        name: 'BUSIITEMCODE',
        fieldLabel: '业务细项代码',
        allowBlank: false,
        blankText: '业务细项代码不可为空!',
    });

    var field_BUSIITEMNAME = Ext.create('Ext.form.field.Text', {
        id: 'field_BUSIITEMNAME',
        name: 'BUSIITEMNAME',
        fieldLabel: '业务细项名称',
        allowBlank: false,
        blankText: '业务细项名称不可为空!',
    });

    //启禁用
    var store_ENABLE = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: [{ "CODE": 0, "NAME": "否" }, { "CODE": 1, "NAME": "是" }]
    });
    var combo_ENABLE = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ENABLE',
        name: 'ENABLE',
        store: store_ENABLE,
        queryMode: 'local',
        anyMatch: true,
        hideTrigger: true,
        forceSelection: true,
        fieldLabel: '是否启用',
        displayField: 'NAME',
        valueField: 'CODE',
        value:0,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                };
            }
        },
        hidden: true
    });

    //修改原因
    var field_REMARK = Ext.create('Ext.form.field.Text', {
        id: 'field_REMARK',
        name: 'REMARK',
        fieldLabel: '修改原因',
        hidden: true
    });

    var formpanel_Win = Ext.create('Ext.form.Panel', {
        id: 'formpanel_Win',
        minHeight: 150,
        border: 0,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: 1,
            labelAlign: 'right',
            labelSeparator: '',
            msgTarget: 'under'
        },
        items: [
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_BUSITYPE] },
                { layout: 'column', height: 42, border: 0, items: [field_BUSIITEMCODE] },
                { layout: 'column', height: 42, border: 0, items: [field_BUSIITEMNAME] },
                { layout: 'column', height: 42, border: 0, items: [combo_ENABLE] },
                { layout: 'column', height: 42, border: 0, items: [field_REMARK] },
                field_id, field_BUSITYPENAME
        ],
        buttonAlign: 'center',
        buttons: [{
            text: '<i class="fa fa-floppy-o"></i>&nbsp;保存', handler: function () {
                if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {
                    return;
                }

                var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());
                Ext.Ajax.request({
                    url: "/OrderManager/Save_customsconfig",
                    params: { formdata: formdata },
                    success: function (option, success, response) {
                        var data = Ext.decode(option.responseText);
                        if (data.success) {
                            Ext.MessageBox.alert("提示","保存成功！", function () {
                                Ext.getCmp("pgbar").moveFirst();
                                Ext.getCmp("win_d").close();
                            });
                        }
                        else {
                            Ext.MessageBox.alert("提示", "保存失败:" + data.msg);
                        }
                    }
                });
            }
        }]
    });
}

function form_ini_config(ID, recs) {
    form_ini_win();

    if (ID != "") {       
        Ext.getCmp('combo_ENABLE').hidden = false;
        Ext.getCmp('combo_ENABLE').allowBlank = false;
        Ext.getCmp('combo_ENABLE').blankText = '是否启用不可为空!';

        Ext.getCmp('field_REMARK').hidden = false;
        Ext.getCmp('field_REMARK').allowBlank = false;
        Ext.getCmp('field_REMARK').blankText = '修改原因不可为空!';

        Ext.getCmp('formpanel_Win').getForm().setValues(recs);
    }
    var win = Ext.create("Ext.window.Window", {
        id: "win_d",
        title: '业务细项',
        width: 500,
        height: 280,
        modal: true,
        items: [Ext.getCmp('formpanel_Win')]
    });
    win.show();
}

function AddCusConfig() {
    form_ini_config("", null);
}

function EditCusConfig() {
    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length != 1) {
        Ext.MessageBox.alert('提示', '请选择一笔需要修改的记录！');
        return;
    }
    form_ini_config(recs[0].get("ID"), recs[0].data);
}

function DelCusConfig() {
    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要删除的记录！');
        return;
    }
    var ids = "";
    Ext.each(recs, function (rec) {
        ids += rec.get("ID") + ",";
    });
    ids = ids.substr(0, ids.length - 1);

    Ext.MessageBox.confirm("提示", "确定要删除所选择的记录吗？", function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/OrderManager/DelCusConfig',
                params: { ids: ids },
                success: function (response, success, option) {
                    var res = Ext.decode(response.responseText);
                    if (res.success) {
                        Ext.MessageBox.alert('提示', '删除成功！', function () {
                            Ext.getCmp("pgbar").moveFirst();
                        });
                    }
                    else {
                        Ext.MessageBox.alert('提示', '删除失败！');
                    }
                }
            });
        }
    });
}

function EnableCusConfig(enable) {
    var enablemsg = "";
    if (enable == 1) {
        enablemsg = "启用";
    } else {
        enablemsg = "禁用";
    }

    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要' + enablemsg + '的记录！');
        return;
    }
    var ids = "";
    Ext.each(recs, function (rec) {
        ids += rec.get("ID") + ",";
    });
    ids = ids.substr(0, ids.length - 1);

    Ext.MessageBox.confirm("提示", "确定要" + enablemsg + "所选择的记录吗？", function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/OrderManager/EnableCusConfig',
                params: { ids: ids, enable: enable },
                success: function (response, success, option) {
                    var res = Ext.decode(response.responseText);
                    if (res.success) {
                        Ext.MessageBox.alert('提示', enablemsg + '成功！', function () {
                            Ext.getCmp("pgbar").moveFirst();
                        });
                    }
                    else {
                        Ext.MessageBox.alert('提示', enablemsg + '失败！');
                    }
                }
            });
        }
    });
}

function ExportCusConfig() {
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据导出中，请稍等..." });
    myMask.show();

    var data = {
        busitypeid: Ext.getCmp('combo_BUSITYPE_S').getValue(), //entrusttype: Ext.getCmp('combo_ENTRUSTTYPE_S').getValue(),
        busiitemcode: Ext.getCmp('field_BUSIITEMCODE_S').getValue(),
        busiitemname: Ext.getCmp('field_BUSIITEMNAME_S').getValue(),
        enable: Ext.getCmp('combo_ENABLE_S').getValue()
    }

    Ext.Ajax.request({
        url: '/OrderManager/ExportCusConfig',
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