function from_init_empower() {
   
    var tbar1 = Ext.create('Ext.toolbar.Toolbar', {
        items: [                
                {
                    text: '<i class="fa fa-key"></i>&nbsp;授权配置', handler: function () {
                        var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
                        if (recs.length == 0) {
                            Ext.MessageBox.alert('提示', '请选择需要操作的记录！');
                            return;
                        }
                        var ids = "";
                        Ext.each(recs, function (rec) {
                            ids = ids + rec.get("CODE") + ",";
                        });

                        ids = (ids.substring(ids.length - 1) == ',') ? ids.substring(0, ids.length - 1) : ids;
                        
                        Ext.Ajax.request({
                            url: '/Account/Insert_list_UnAuthorized',
                            params: { IDS: ids },
                            success: function (response, option) {
                                var data = Ext.decode(response.responseText);
                                Ext.MessageBox.alert('提示', '修改成功' + data.success + '条！失败' + data.fail + '条（无海关十位编码）!');
                                Ext.getCmp("pgbar").moveFirst();
                                //    Ext.getCmp("pgbar").moveFirst();
                                //if (data.result == true) {
                                //    Ext.MessageBox.alert('提示', '修改成功！');
                                //    Ext.getCmp("pgbar").moveFirst();
                                //}
                                //else {
                                //    Ext.MessageBox.alert('提示', '修改失败！');
                                //}
                            }
                        });
                    }
                },

                , '->'
                , {
                    text: '<i class="fa fa-search"></i>&nbsp;查 询', handler: function () {
                        Ext.getCmp("pgbar").moveFirst();
                    }
                }
                , {
                    text: '<i class="fa fa-refresh"></i>&nbsp;重 置', handler: function () {
                        Ext.each(Ext.getCmp('formpanel_sub_search1').getForm().getFields().items, function (field) {
                            field.reset();
                        });
                    }
                }
        ]
    });


        var customerdatabase = Ext.create('Ext.data.JsonStore',
            {
                fields: ['CODE', 'NAME', 'ID','ISEMPOWER','ISENABLED'],
                pageSize: 20,
                proxy: {
                    type: 'ajax',
                    url: '/Account/GetIsCompany',
                    reader: {
                        root: 'rows',
                        type: 'json',
                        totalProperty: 'total'
                    }
                },
                autoLoad: true,
                listeners: {
                    beforeload: function (store, options) {
                        customerdatabase.getProxy().extraParams = Ext.getCmp('formpanel_sub_search1').getForm().getValues();
                    }
                }
            });
        var pgbar = Ext.create('Ext.toolbar.Paging',
            {
                id: 'pgbar',
                displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
                store: customerdatabase,
                displayInfo: true
            });
        var gridpanel = Ext.create('Ext.grid.Panel',
            {
                tbar: tbar1,
                id: 'gridpanel',
                store: customerdatabase,
                region: 'center',
                height: 610,
                selModel: { selType: 'checkboxmodel' },               
                bbar: pgbar,
                columns: [
                    { xtype: 'rownumberer', width: 35 },
                    { header: '客户代码', dataIndex: 'CODE', width: '25%' },
                    { header: '客户名称', dataIndex: 'NAME', width:190 },
                    { header: '是否授权', dataIndex: 'ISEMPOWER', width: 190,renderer:gridrender },
                    { header: 'ID', dataIndex: 'ID', width: 200, hidden: true }
                ],
                listeners:
                {
                    'itemdblclick': function (view, record, item, index, e) {
                        
                    }
                },
                viewConfig: {
                    enableTextSelection: true
                }
            });


        function gridrender(value, cellmeta, record, rowIndex, columnIndex, stroe) {
            
            var dataindex = cellmeta.column.dataIndex;
            var str = "";
            if (dataindex != null ) {
                
            }

            switch (dataindex) {
                case "ISEMPOWER":
                    str = value != null ? '<span class="icon iconfont" style="font-size:12px;color:blue;">&#xe628;</span>' : '<span class="icon iconfont" style="font-size:12px;color:red;">&#xe634;</span>';
            break;
        }
        return str;
    }


    var txtNAME1 = Ext.create('Ext.form.field.Text', { id: 'NAME_S1', name: 'NAME_S1', fieldLabel: '客户代码' });
    var txtREALNAME1 = Ext.create('Ext.form.field.Text', { id: 'REALNAME_S1', name: 'REALNAME_S1', fieldLabel: '客户名称' });
    var formpanel_sub_search = Ext.create('Ext.form.Panel', {
        id: 'formpanel_sub_search1',
        region: 'north',
        border: 0,
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.25,
            labelWidth: 70
        },
        items: [
            { layout: 'column', border: 0, items: [txtNAME1, txtREALNAME1] }
        ]
    });
    
    formempower_panel = Ext.create('Ext.panel.Panel',
        {
            id: 'formempower_panel',
            labelAlign: "center",
            border: 0,
            height: 650,
            items: [formpanel_sub_search, gridpanel]
        });

    
        
    
}