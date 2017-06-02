function subaccount_init() {
    Ext.regModel('User', { fields: ['ID', 'NAME', 'REALNAME', 'EMAIL', 'TELEPHONE', 'MOBILEPHONE', 'POSITIONID', 'SEX', 'ENABLED', 'CREATETIME', 'PARENTID', 'REMARK'] });
    store_user = Ext.create('Ext.data.JsonStore', {
        model: 'User',
        proxy: {
            type: 'ajax',
            url: '/Account/loaduserInfo',
            reader: {
                root: 'rows',
                type: 'json',
                totalProperty: 'total'
            }
        },
        autoLoad: true
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
                        Ext.Ajax.request({
                            url: '/Account/Delete',
                            params: { ID: recs[0].get("ID") },
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
                }, '->', '<span>说明：初始化密码后密码与登录账号相同</span>']
    })
    var pgbar = Ext.create('Ext.toolbar.Paging', {
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_user,
        displayInfo: true
    })
    gridpanel = Ext.create('Ext.grid.Panel', {
        // title: '子账户信息',
        tbar: tbar,
        // renderTo: 'appConId',
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