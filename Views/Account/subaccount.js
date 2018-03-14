function subaccount_init() {
    Ext.regModel('User', { fields: ['ID', 'NAME', 'REALNAME', 'EMAIL', 'TELEPHONE', 'MOBILEPHONE', 'POSITIONID', 'SEX', 'ENABLED', 'CREATETIME', 'PARENTID', 'REMARK'] });
    store_user = Ext.create('Ext.data.JsonStore', {
        model: 'User',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/Account/loaduserInfo_m',
            reader: {
                root: 'rows',
                type: 'json',
                totalProperty: 'total'
            }
        },
        autoLoad: true,
        listeners: {
            beforeload: function (store, options) {
                store_user.getProxy().extraParams = Ext.getCmp('formpanel_sub_search').getForm().getValues();
            }
        }
    })
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [{
            text: '<i class="fa fa-plus fa-fw"></i>&nbsp;添加', handler: function () {
                opencenterwin("/Account/ChildEdit", 800, 400);
            }
        },
                {
                    text: '<i class="fa fa-pencil-square-o"></i>&nbsp;修改', handler: function () {
                        var recs = gridpanel.getSelectionModel().getSelection();
                        if (recs.length == 0) {
                            Ext.MessageBox.alert('提示', '请选择需要修改的记录！');
                            return;
                        }
                        opencenterwin("/Account/ChildEdit?ID=" + recs[0].get("ID"), 800, 400);
                    }
                },
                {
                    text: '<i class="fa fa-trash-o"></i>&nbsp;删除', handler: function () {
                        var recs = gridpanel.getSelectionModel().getSelection();
                        if (recs.length == 0) {
                            Ext.MessageBox.alert('提示', '请选择需要删除的记录！');
                            return;
                        }

                        var ids = "";
                        Ext.each(recs, function (rec) {
                            ids = ids + rec.get("ID") + ",";
                        });
                        ids = ids.substr(0, ids.length - 1);

                        Ext.Ajax.request({
                            url: '/Account/Delete',
                            params: { ID: ids },//recs[0].get("ID")
                            success: function (response, option) {
                                var data = Ext.decode(response.responseText);
                                if (data.result == true) {
                                    Ext.MessageBox.alert('提示', '删除成功！');
                                    store_user.load();
                                }
                                else {
                                    Ext.MessageBox.alert('提示', '删除失败！');
                                }
                            }
                        });
                    }
                },
                {
                    text: '<i class="fa fa-key"></i>&nbsp;初始化密码', handler: function () {
                        var recs = gridpanel.getSelectionModel().getSelection();
                        if (recs.length == 0) {
                            Ext.MessageBox.alert('提示', '请选择需要操作的记录！');
                            return;
                        }
                        Ext.Ajax.request({
                            url: '/Account/InitialPsd',
                            params: { ID: recs[0].get("ID"), NAME: recs[0].get("NAME") },
                            success: function (response, option) {
                                var data = Ext.decode(response.responseText);
                                if (data.result == true) {
                                    Ext.MessageBox.alert('提示', '密码初始化成功！');
                                    store_user.load();
                                }
                                else {
                                    Ext.MessageBox.alert('提示', '密码初始化失败！');
                                }
                            }
                        });
                    }
                },
                '<span style="color:blue;">说明：初始化密码后密码与登录账号相同</span>'
                , '->'
                , {
                    text: '<i class="fa fa-search"></i>&nbsp;查 询', handler: function () {
                        Ext.getCmp("pgbar_sub").moveFirst();
                    }
                }
                , {
                    text: '<i class="fa fa-refresh"></i>&nbsp;重 置', handler: function () {
                        Ext.each(Ext.getCmp('formpanel_sub_search').getForm().getFields().items, function (field) {
                            field.reset();
                        });
                    }
                }
        ]
    });
    var pgbar = Ext.create('Ext.toolbar.Paging', {
        id: 'pgbar_sub',
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_user,
        displayInfo: true
    })
    gridpanel = Ext.create('Ext.grid.Panel', {
        tbar: tbar,
        region: 'center',
        store: store_user,
        height: 443,
        selModel: { selType: 'checkboxmodel' },
        bbar: pgbar,
        columns: [
            { xtype: 'rownumberer', width: 35 },
            { header: 'ID', dataIndex: 'ID', hidden: true },
            { header: '登录账户', dataIndex: 'NAME', width: 130 },
            { header: '姓名', dataIndex: 'REALNAME', width: 90 },
            { header: '邮箱', dataIndex: 'EMAIL', width: 200 },
            { header: '电话', dataIndex: 'TELEPHONE', width: 130 },
            { header: '手机', dataIndex: 'MOBILEPHONE', width: 120 },
            {
                header: '状态', dataIndex: 'ENABLED', width: 60, renderer: function (value) {
                    if (value == '1') {
                        return '启用';
                    } else {
                        return '停用';
                    };
                }
            },
           { header: '创建时间', dataIndex: 'CREATETIME', width: 150 },
           { header: '备注', dataIndex: 'REMARK', flex: 1 }
        ]
    })
}

function init_search() {
    var txtNAME = Ext.create('Ext.form.field.Text', { id: 'NAME_S', name: 'NAME_S', fieldLabel: '登录账号' });
    var txtREALNAME = Ext.create('Ext.form.field.Text', { id: 'REALNAME_S', name: 'REALNAME_S', fieldLabel: '姓名' });

    var store_ENABLED_S = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: [{ "CODE": 0, "NAME": "停用" }, { "CODE": 1, "NAME": "启用" }]
    });
    var combo_ENABLED_S = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ENABLED_S',
        name: 'ENABLED_S',
        store: store_ENABLED_S,
        queryMode: 'local',
        anyMatch: true,
        fieldLabel: '是否启用',
        displayField: 'NAME',
        valueField: 'CODE'
    });

    var formpanel_sub_search = Ext.create('Ext.form.Panel', {
        id: 'formpanel_sub_search',
        region: 'north',
        border: 0,
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.25,
            labelWidth: 70
        },
        items: [
        { layout: 'column', border: 0, items: [txtNAME, txtREALNAME, combo_ENABLED_S] }
        ]
    });
}