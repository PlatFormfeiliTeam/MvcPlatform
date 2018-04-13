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

            initSearch_CustomsCost();
            bindgrid();
        }
    });
});

function initSearch_CustomsCost() {

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
            },
            change: function (combo, newValue, oldValue, eOpts) {
                combo_ENTRUSTTYPE_S.reset();
                Ext.Ajax.request({
                    url: "/OrderManager/Ini_Base_Data_BUSIITEM",
                    params: { busitype: newValue },
                    success: function (response, opts) {
                        var commondata = Ext.decode(response.responseText);//业务细项
                        store_ENTRUSTTYPE_S.loadData(commondata.ywxx);
                    }
                });

            }
        }
    });


    //业务细项
    var store_ENTRUSTTYPE_S = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME', 'CODENAME']
    });

    var combo_ENTRUSTTYPE_S = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ENTRUSTTYPE_S',
        name: 'ENTRUSTTYPE',
        store: store_ENTRUSTTYPE_S,
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
            { layout: 'column', border: 0, items: [combo_BUSITYPE_S, combo_ENTRUSTTYPE_S] }
        ]
    });

}

function bindgrid() {
    var store_Trade = Ext.create('Ext.data.JsonStore', {
        fields: ['BUSITYPECODE', 'BUSITYPENAME', 'BUSIITEMCODE', 'BUSIITEMNAME', 'ORIGINNAME', 'CONFIGNAME', 'CREATEUSERID', 'CREATEUSERNAME', 'ID'],
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/OrderManager/LoadList_customscost',
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
                    busitypeid: Ext.getCmp('combo_BUSITYPE_S').getValue(), entrusttype: Ext.getCmp('combo_ENTRUSTTYPE_S').getValue()
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
                { header: '文本名称', dataIndex: 'ORIGINNAME', width: 200 },
                { header: '配置名称', dataIndex: 'CONFIGNAME', width: 200 },
                { header: '创建人', dataIndex: 'CREATEUSERNAME', width: 150 },
        ],
        listeners:
        {
            'itemdblclick': function (view, record, item, index, e) {
                EditCusCost();
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
    Ext.getCmp("combo_ENTRUSTTYPE_S").setValue("");
}

function form_ini_win(recs) {

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
            change: function (combo, newValue, oldValue, eOpts) {
                combo_BUSIITEMCODE.reset();
                Ext.Ajax.request({
                    url: "/OrderManager/Ini_Base_Data_BUSIITEM",
                    params: { busitype: newValue },
                    success: function (response, opts) {
                        var commondata = Ext.decode(response.responseText);//业务细项
                        store_BUSIITEM.loadData(commondata.ywxx);

                        var rec = store_BUSIITEM.findRecord('CODE', recs.BUSIITEMCODE);
                        if (!rec) {
                            combo_BUSIITEMCODE.setValue("");//编辑页赋值
                        } else {
                            combo_BUSIITEMCODE.setValue(recs.BUSIITEMCODE);//编辑页赋值
                        }
                    }
                });

            },
            select: function (records) { field_BUSITYPENAME.setValue(records.rawValue.substr(0, records.rawValue.lastIndexOf('('))); }
        },
        allowBlank: false,
        blankText: '业务类型不可为空!'
    });
    var field_BUSITYPENAME = Ext.create('Ext.form.field.Hidden', { id: 'field_BUSITYPENAME', name: 'BUSITYPENAME' });

    //业务细项
    var store_BUSIITEM = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME', 'CODENAME']
    });

    var combo_BUSIITEMCODE = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_BUSIITEMCODE',
        name: 'BUSIITEMCODE',
        store: store_BUSIITEM,
        fieldLabel: '业务细项',//tabIndex: 3
        displayField: 'CODENAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
        hideTrigger: true,
        forceSelection: true,
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
            select: function (records) { field_BUSIITEMNAME.setValue(records.rawValue.substr(0, records.rawValue.lastIndexOf('('))); }
        },
        allowBlank: false,
        blankText: '业务细项不可为空!'
    });
    var field_BUSIITEMNAME = Ext.create('Ext.form.field.Hidden', { id: 'field_BUSIITEMNAME', name: 'BUSIITEMNAME' });

    //适用页面
    var store_ORIGINNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: [{ "CODE": "文本1", "NAME": "文本1" }, { "CODE": "文本2", "NAME": "文本2" }
        , { "CODE": "数字1", "NAME": "数字1" }, { "CODE": "数字2", "NAME": "数字2" }
        , { "CODE": "日期1", "NAME": "日期1" }, { "CODE": "日期2", "NAME": "日期2" }
        , { "CODE": "人员1", "NAME": "人员1" }, { "CODE": "人员2", "NAME": "人员2" }]
    });

    var combo_ORIGINNAME = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ORIGINNAME',
        name: 'ORIGINNAME',
        store: store_ORIGINNAME,
        queryMode: 'local',
        displayField: 'NAME',
        valueField: 'CODE',
        anyMatch: true,
        fieldLabel: '文本名称',
        hideTrigger: true,
        forceSelection: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                };
            }
        },
        allowBlank: false,
        blankText: '文本名称不可为空!'
    });

    var field_CONFIGNAME = Ext.create('Ext.form.field.Text', {
        id: 'CONFIGNAME',
        name: 'CONFIGNAME',
        fieldLabel: '配置名称',
        allowBlank: false,
        blankText: '配置名称不可为空!',
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
                { layout: 'column', height: 42, border: 0, items: [combo_BUSIITEMCODE] },
                { layout: 'column', height: 42, border: 0, items: [combo_ORIGINNAME] },
                { layout: 'column', height: 42, border: 0, items: [field_CONFIGNAME] },
                field_id, field_BUSITYPENAME, field_BUSIITEMNAME
        ],
        buttonAlign: 'center',
        buttons: [{
            text: '<i class="fa fa-floppy-o"></i>&nbsp;保存', handler: function () {
                if (!Ext.getCmp('formpanel_Win').getForm().isValid()) {
                    return;
                }

                var formdata = Ext.encode(Ext.getCmp('formpanel_Win').getForm().getValues());
                Ext.Ajax.request({
                    url: "/OrderManager/Save_customscost",
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
    form_ini_win(recs);

    if (ID != "") {   
        Ext.getCmp('formpanel_Win').getForm().setValues(recs);
    }
    var win = Ext.create("Ext.window.Window", {
        id: "win_d",
        title: '业务细项',
        width: 500,
        height: 240,
        modal: true,
        items: [Ext.getCmp('formpanel_Win')]
    });
    win.show();
}

function AddCusCost() {
    form_ini_config("", null);
}

function EditCusCost() {
    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length != 1) {
        Ext.MessageBox.alert('提示', '请选择一笔需要修改的记录！');
        return;
    }
    form_ini_config(recs[0].get("ID"), recs[0].data);
}

function DelCusCost() {
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
                url: '/OrderManager/DelCusCost',
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

function ExportCusCost() {
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据导出中，请稍等..." });
    myMask.show();

    var data = {
        busitypeid: Ext.getCmp('combo_BUSITYPE_S').getValue(), entrusttype: Ext.getCmp('combo_ENTRUSTTYPE_S').getValue()
    }

    Ext.Ajax.request({
        url: '/OrderManager/ExportCusCost',
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