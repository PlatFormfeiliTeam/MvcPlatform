function initSearch_Audit() {
    //企业编号
    var store_enterprise = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_jydw
    });
    var s_combo_enterprise = Ext.create('Ext.form.field.ComboBox', {
        id: 's_combo_enterprise',        
        store: store_enterprise,
        fieldLabel: '企业编号',
        //width: 220,
        displayField: 'NAME',
        name: 'NAME',
        minChars:4,
        valueField: 'CODE',
        triggerAction: 'all',
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        listeners: {
            blur: function () {
                Ext.getCmp('s_combo_recordid').reset();
                Ext.getCmp('s_combo_recordid').store.removeAll();

                Ext.Ajax.request({
                    url: "/RecordInfor/GetRecordidByEnterprise",
                    params: { EnterpriseHSCOCDE: this.lastValue },
                    success: function (response, opts) {
                        var commondata = Ext.decode(response.responseText);
                        Ext.getCmp('s_combo_recordid').store.loadData(commondata.recordid);//对应企业的账册号
                    }
                });
            }
            //change: function (combo, newValue, oldValue, eOpts) {
            //    Ext.getCmp('s_combo_recordid').reset();              
            //    Ext.getCmp('s_combo_recordid').store.removeAll();
            //    Ext.Ajax.request({
            //        url: "/RecordInfor/GetRecordidByEnterprise",
            //        params: { EnterpriseHSCOCDE: newValue },
            //        success: function (response, opts) {
            //            var commondata = Ext.decode(response.responseText);
            //            Ext.getCmp('s_combo_recordid').store.loadData(commondata.recordid);//对应企业的账册号
            //        }
            //    });
            //}
        }
    });

    //账册号
    var store_recordid = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME']
    });
    var s_combo_recordid = Ext.create('Ext.form.field.ComboBox', {
        id: 's_combo_recordid',
        editable: false,
        store: store_recordid,
        fieldLabel: '账册号',
        //width: 220,
        displayField: 'NAME',
        name: 'recordid',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local'
    });
    //项号
    var field_ITEMNO = Ext.create('Ext.form.field.Text', {
        id: 'field_ITEMNO',
        fieldLabel: '项号',
        name: 'ITEMNO',
    });
    //HS编码
    var field_HSCODE = Ext.create('Ext.form.field.Text', {
        id: 'field_HSCODE',
        fieldLabel: 'HS编码',
        name: 'HSCODE'
    });
    //变动状态
    var store_optionstatus = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: optionstatus_js_data
    });
    var s_combo_optionstatus = Ext.create('Ext.form.field.ComboBox', {
        id: 's_combo_optionstatus',
        editable: false,
        store: store_optionstatus,
        fieldLabel: '变动状态',
        displayField: 'NAME',
        name: 'OPTIONS',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local'
    });
    //申请状态
    var store_status = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: status_js_data
    });
    var s_combo_status = Ext.create('Ext.form.field.ComboBox', {
        id: 's_combo_status',
        editable: false,
        store: store_status,
        fieldLabel: '申请状态',
        displayField: 'NAME',
        name: 'STATUS',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local'
    });

    var date_start = Ext.create('Ext.form.field.Date', {
        id: 'date_start',
        margin: 0,
        emptyText: '开始日期',flex: .5,
        format: 'Y-m-d'
    })
    var date_end = Ext.create('Ext.form.field.Date', {
        id: 'date_end',
        margin: 0,
        emptyText: '结束日期', flex: .5,
        format: 'Y-m-d'
    })
    var date_container = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        fieldLabel: '提交日期',
        columnWidth: .25,
        items: [date_start, date_end]
    }

    var formpanel = Ext.create('Ext.form.Panel', {
        id: 'formpanel',
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5', labelWidth: 60, columnWidth: .25,
        },
        items: [
        { layout: 'column', border: 0, margin: '5 0 0 0', items: [s_combo_enterprise, s_combo_recordid, field_ITEMNO, field_HSCODE] },
        { layout: 'column', border: 0, items: [s_combo_optionstatus, s_combo_status, date_container] }
        ]
    });
}

function itemsbind_Audit() {
    Ext.regModel('RecrodDetail', {
        fields: ['ID', 'OPTIONS', 'STATUS', 'RECORDINFOID', 'CODE', 'ITEMNO', 'HSCODE', 'ADDITIONALNO', 'ITEMNOATTRIBUTE', 'COMMODITYNAME'
            , 'SPECIFICATIONSMODEL', 'UNIT', 'REMARK', 'BUSIUNIT', 'BUSIUNITNAME']
    });

    var store_RecrodDetail_lj = Ext.create('Ext.data.JsonStore', {
        model: 'RecrodDetail',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/RecordInfor/loadRecordDetail_Audit',
            reader: {
                root: 'rows',
                type: 'json',
                totalProperty: 'total'
            }
        },
        autoLoad: true,
        listeners: {
            beforeload: function () {
                store_RecrodDetail_lj.getProxy().extraParams = {
                    ITEMNOATTRIBUTE: '料件',
                    ENTERPRISECODE: Ext.getCmp('s_combo_recordid').getValue(), RECORDINFORID: Ext.getCmp('s_combo_enterprise').getValue(),
                    ITEMNO: Ext.getCmp("field_ITEMNO").getValue(), HSCODE: Ext.getCmp('field_HSCODE').getValue(),
                    OPTIONS: Ext.getCmp('s_combo_optionstatus').getValue(), STATUS: Ext.getCmp("s_combo_status").getValue(),
                    DATE_START: Ext.Date.format(Ext.getCmp("date_start").getValue(), 'Y-m-d H:i:s'), DATE_END: Ext.Date.format(Ext.getCmp("date_end").getValue(), 'Y-m-d H:i:s')
                }
            },
            load: function () {
                var total_lj = store_RecrodDetail_lj.getProxy().getReader().rawData.total;
                Ext.getCmp("tabpanel").items.items[0].setTitle("料件(" + total_lj + ")");
            }
        }
    });

    var pgbar_lj = Ext.create('Ext.toolbar.Paging', {
        id: 'pgbar_lj',
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_RecrodDetail_lj,
        displayInfo: true
    })
    gridpanel_lj = Ext.create('Ext.grid.Panel', {
        id: "gridpanel_lj",
        store: store_RecrodDetail_lj,
        height: 500,
        selModel: { selType: 'checkboxmodel' },
        bbar: pgbar_lj,
        enableColumnHide: false,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: 'ID', dataIndex: 'ID', hidden: true },
        { header: '账册号', dataIndex: 'CODE', width: 130 },
        { header: '项号', dataIndex: 'ITEMNO', width: 80 },
        { header: 'HS编码', dataIndex: 'HSCODE', width: 130 },
        { header: '附加码', dataIndex: 'ADDITIONALNO', width: 80 },
        { header: '项号属性', dataIndex: 'ITEMNOATTRIBUTE', width: 80 },
        { header: '商品名称', dataIndex: 'COMMODITYNAME', width: 200 },
        { header: '规格型号', dataIndex: 'SPECIFICATIONSMODEL', width: 200 },
        { header: '成交单位', dataIndex: 'UNIT', width: 80, renderer: renderOrder },
        { header: '企业名称', dataIndex: 'BUSIUNITNAME', width: 250 },
        { header: '备注', dataIndex: 'REMARK', width: 150 }
        ],
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });
    var store_RecrodDetail_cp = Ext.create('Ext.data.JsonStore', {
        model: 'RecrodDetail',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/RecordInfor/loadRecordDetail_Audit',
            reader: {
                root: 'rows',
                type: 'json',
                totalProperty: 'total'
            }
        },
        autoLoad: true,
        listeners: {
            beforeload: function () {
                store_RecrodDetail_cp.getProxy().extraParams = {
                    ITEMNOATTRIBUTE: '成品',
                    ENTERPRISECODE: Ext.getCmp('s_combo_recordid').getValue(), RECORDINFORID: Ext.getCmp('s_combo_enterprise').getValue(),
                    ITEMNO: Ext.getCmp("field_ITEMNO").getValue(), HSCODE: Ext.getCmp('field_HSCODE').getValue(),
                    OPTIONS: Ext.getCmp('s_combo_optionstatus').getValue(), STATUS: Ext.getCmp("s_combo_status").getValue(),
                    DATE_START: Ext.Date.format(Ext.getCmp("date_start").getValue(), 'Y-m-d H:i:s'), DATE_END: Ext.Date.format(Ext.getCmp("date_end").getValue(), 'Y-m-d H:i:s')
                }
            },
            load: function () {
                var total_cp = store_RecrodDetail_cp.getProxy().getReader().rawData.total;
                Ext.getCmp("tabpanel").items.items[1].setTitle("成品(" + total_cp + ")");
            }
        }
    });

    var pgbar_cp = Ext.create('Ext.toolbar.Paging', {
        id: 'pgbar_cp',
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_RecrodDetail_cp,
        displayInfo: true
    })
    gridpanel_cp = Ext.create('Ext.grid.Panel', {
        id: "gridpanel_cp",
        store: store_RecrodDetail_cp,
        height: 500,
        selModel: { selType: 'checkboxmodel' },
        bbar: pgbar_cp,
        enableColumnHide: false,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: 'ID', dataIndex: 'ID', hidden: true },
        { header: '账册号', dataIndex: 'CODE', width: 130 },
        { header: '项号', dataIndex: 'ITEMNO', width: 80 },
        { header: 'HS编码', dataIndex: 'HSCODE', width: 130 },
        { header: '附加码', dataIndex: 'ADDITIONALNO', width: 80 },
        { header: '项号属性', dataIndex: 'ITEMNOATTRIBUTE', width: 80 },
        { header: '商品名称', dataIndex: 'COMMODITYNAME', width: 200 },
        { header: '规格型号', dataIndex: 'SPECIFICATIONSMODEL', width: 200 },
        { header: '成交单位', dataIndex: 'UNIT', width: 80, renderer: renderOrder },
        { header: '企业名称', dataIndex: 'BUSIUNITNAME', width: 250 },
        { header: '备注', dataIndex: 'REMARK', width: 150 }
        ],
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });

    //====================================================================Go==================================================================
    var tbar_lj_Go = Ext.create('Ext.toolbar.Toolbar', {
        items: [
            //{
            //    text: '<span class="icon iconfont" style="font-size:12px;color:#3071A9;">&#xe632;</span>&nbsp;<font color="#3071A9">修 改</font>',
            //    handler: function () { edit_task('gridpanel_lj_Go'); }
            //},
            //   {
            //       text: '<span class="icon iconfont" style="font-size:12px;color:#3071A9;">&#xe6d3;</span>&nbsp;<font color="#3071A9">删 除</font>',
            //       handler: function () { delete_task('gridpanel_lj_Go', 'pgbar_lj_Go'); }
            //   },
            //   {
            //       text: '<span class="icon iconfont" style="font-size:12px;color:#3071A9;">&#xe601;</span>&nbsp;<font color="#3071A9">申请表打印</font>',
            //       handler: function () { print_task('gridpanel_lj_Go'); }
            //   }
        ]

    });
    var store_RecrodDetail_lj_Go = Ext.create('Ext.data.JsonStore', {
        model: 'RecrodDetail',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/RecordInfor/loadRecordDetail_Audit_Go',
            reader: {
                root: 'rows',
                type: 'json',
                totalProperty: 'total'
            }
        },
        autoLoad: true,
        listeners: {
            beforeload: function () {
                store_RecrodDetail_lj_Go.getProxy().extraParams = {
                    ITEMNOATTRIBUTE: '料件',
                    ENTERPRISECODE: Ext.getCmp('s_combo_recordid').getValue(), RECORDINFORID: Ext.getCmp('s_combo_enterprise').getValue(),
                    ITEMNO: Ext.getCmp("field_ITEMNO").getValue(), HSCODE: Ext.getCmp('field_HSCODE').getValue(),
                    OPTIONS: Ext.getCmp('s_combo_optionstatus').getValue(), STATUS: Ext.getCmp("s_combo_status").getValue(),
                    DATE_START: Ext.Date.format(Ext.getCmp("date_start").getValue(), 'Y-m-d H:i:s'), DATE_END: Ext.Date.format(Ext.getCmp("date_end").getValue(), 'Y-m-d H:i:s')
                }
            },
            load: function () {
                var total_lj_Go = store_RecrodDetail_lj_Go.getProxy().getReader().rawData.total;
                Ext.getCmp("tabpanel").items.items[2].setTitle("料件_申请中(" + total_lj_Go + ")");
            }
        }
    });

    var pgbar_lj_Go = Ext.create('Ext.toolbar.Paging', {
        id: 'pgbar_lj_Go',
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_RecrodDetail_lj_Go,
        displayInfo: true
    })
    gridpanel_lj_Go = Ext.create('Ext.grid.Panel', {
        id: 'gridpanel_lj_Go',
        store: store_RecrodDetail_lj_Go,
        height: 500,
        selModel: { selType: 'checkboxmodel' },
        tbar: tbar_lj_Go,
        bbar: pgbar_lj_Go,
        enableColumnHide: false,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: 'ID', dataIndex: 'ID', hidden: true },
        { header: '变动状态', dataIndex: 'OPTIONS', width: 110, renderer: renderOrder },
        { header: '申请状态', dataIndex: 'STATUS', width: 110, renderer: renderOrder },
        { header: '账册号', dataIndex: 'CODE', width: 130 },
        { header: '项号', dataIndex: 'ITEMNO', width: 80 },
        { header: 'HS编码', dataIndex: 'HSCODE', width: 130 },
        { header: '附加码', dataIndex: 'ADDITIONALNO', width: 80 },
        { header: '项号属性', dataIndex: 'ITEMNOATTRIBUTE', width: 80 },
        { header: '商品名称', dataIndex: 'COMMODITYNAME', width: 150 },
        { header: '规格型号', dataIndex: 'SPECIFICATIONSMODEL', width: 200 },
        { header: '成交单位', dataIndex: 'UNIT', width: 80, renderer: renderOrder },
        { header: '委托企业', dataIndex: 'BUSIUNITNAME', width: 250 },
        { header: '备注', dataIndex: 'REMARK', width: 150 }
        ],
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });


    var tbar_cp_Go = Ext.create('Ext.toolbar.Toolbar', {
        items: [
            //{
            //    text: '<span class="icon iconfont" style="font-size:12px;color:#3071A9;">&#xe632;</span>&nbsp;<font color="#3071A9">修 改</font>',
            //    handler: function () { edit_task('gridpanel_cp_Go'); }
            //},
            //   {
            //       text: '<span class="icon iconfont" style="font-size:12px;color:#3071A9;">&#xe6d3;</span>&nbsp;<font color="#3071A9">删 除</font>',
            //       handler: function () { delete_task('gridpanel_cp_Go', 'pgbar_cp_Go'); }
            //   },
            //   {
            //       text: '<span class="icon iconfont" style="font-size:12px;color:#3071A9;">&#xe601;</span>&nbsp;<font color="#3071A9">申请表打印</font>',
            //       handler: function () { print_task('gridpanel_cp_Go'); }
            //   }
        ]

    });

    var store_RecrodDetail_cp_Go = Ext.create('Ext.data.JsonStore', {
        model: 'RecrodDetail',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/RecordInfor/loadRecordDetail_Audit_Go',
            reader: {
                root: 'rows',
                type: 'json',
                totalProperty: 'total'
            }
        },
        autoLoad: true,
        listeners: {
            beforeload: function () {
                store_RecrodDetail_cp_Go.getProxy().extraParams = {
                    ITEMNOATTRIBUTE: '成品',
                    ENTERPRISECODE: Ext.getCmp('s_combo_recordid').getValue(), RECORDINFORID: Ext.getCmp('s_combo_enterprise').getValue(),
                    ITEMNO: Ext.getCmp("field_ITEMNO").getValue(), HSCODE: Ext.getCmp('field_HSCODE').getValue(),
                    OPTIONS: Ext.getCmp('s_combo_optionstatus').getValue(), STATUS: Ext.getCmp("s_combo_status").getValue(),
                    DATE_START: Ext.Date.format(Ext.getCmp("date_start").getValue(), 'Y-m-d H:i:s'), DATE_END: Ext.Date.format(Ext.getCmp("date_end").getValue(), 'Y-m-d H:i:s')
                }
            },
            load: function () {
                var total_cp_Go = store_RecrodDetail_cp_Go.getProxy().getReader().rawData.total;
                Ext.getCmp("tabpanel").items.items[3].setTitle("成品_申请中(" + total_cp_Go + ")");
            }
        }
    });

    var pgbar_cp_Go = Ext.create('Ext.toolbar.Paging', {
        id: 'pgbar_cp_Go',
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_RecrodDetail_cp_Go,
        displayInfo: true
    })
    gridpanel_cp_Go = Ext.create('Ext.grid.Panel', {
        id: 'gridpanel_cp_Go',
        store: store_RecrodDetail_cp_Go,
        height: 500,
        selModel: { selType: 'checkboxmodel' },
        tbar: tbar_cp_Go,
        bbar: pgbar_cp_Go,
        enableColumnHide: false,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: 'ID', dataIndex: 'ID', hidden: true },
        { header: '变动状态', dataIndex: 'OPTIONS', width: 110, renderer: renderOrder },
        { header: '申请状态', dataIndex: 'STATUS', width: 110, renderer: renderOrder },
        { header: '账册号', dataIndex: 'CODE', width: 130 },
        { header: '项号', dataIndex: 'ITEMNO', width: 80 },
        { header: 'HS编码', dataIndex: 'HSCODE', width: 130 },
        { header: '附加码', dataIndex: 'ADDITIONALNO', width: 80 },
        { header: '项号属性', dataIndex: 'ITEMNOATTRIBUTE', width: 80 },
        { header: '商品名称', dataIndex: 'COMMODITYNAME', width: 150 },
        { header: '规格型号', dataIndex: 'SPECIFICATIONSMODEL', width: 200 },
        { header: '成交单位', dataIndex: 'UNIT', width: 80, renderer: renderOrder },
        { header: '委托企业', dataIndex: 'BUSIUNITNAME', width: 250 },
        { header: '备注', dataIndex: 'REMARK', width: 150 }
        ],
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });
}

function Select() {
    Ext.getCmp('pgbar_lj').moveFirst(); Ext.getCmp('pgbar_cp').moveFirst();
    Ext.getCmp('pgbar_lj_Go').moveFirst(); Ext.getCmp('pgbar_cp_Go').moveFirst();
}

function Reset() {
    Ext.each(Ext.getCmp('formpanel').getForm().getFields().items, function (field) {
        field.reset();
    });
}