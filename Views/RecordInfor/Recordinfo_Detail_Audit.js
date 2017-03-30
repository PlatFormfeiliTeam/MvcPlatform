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
            , 'SPECIFICATIONSMODEL', 'UNIT', 'REMARK', 'BUSIUNIT', 'BUSIUNITNAME','SUBMITTIME']
    });
    Ext.tip.QuickTipManager.init();

    
    //====================================================================Go==================================================================
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
                    ENTERPRISECODE: Ext.getCmp('s_combo_enterprise').getValue(), RECORDINFORID: Ext.getCmp('s_combo_recordid').getValue(),
                    ITEMNO: Ext.getCmp("field_ITEMNO").getValue(), HSCODE: Ext.getCmp('field_HSCODE').getValue(),
                    OPTIONS: Ext.getCmp('s_combo_optionstatus').getValue(), STATUS: Ext.getCmp("s_combo_status").getValue(),
                    DATE_START: Ext.Date.format(Ext.getCmp("date_start").getValue(), 'Y-m-d H:i:s'), DATE_END: Ext.Date.format(Ext.getCmp("date_end").getValue(), 'Y-m-d H:i:s')
                }
            },
            load: function () {
                var total_lj_Go = store_RecrodDetail_lj_Go.getProxy().getReader().rawData.total;
                Ext.getCmp("tabpanel").items.items[0].setTitle("料件_申请(" + total_lj_Go + ")");
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
        bbar: pgbar_lj_Go,
        enableColumnHide: false,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: 'ID', dataIndex: 'ID', hidden: true },
        { header: '变动状态', dataIndex: 'OPTIONS', width: 80, renderer: renderOrder },
        { header: '申请状态', dataIndex: 'STATUS', width: 80, renderer: renderOrder },
        { header: '账册号', dataIndex: 'CODE', width: 130 },
        { header: '项号', dataIndex: 'ITEMNO', width: 80 },
        { header: 'HS编码', dataIndex: 'HSCODE', width: 100 },
        { header: '附加码', dataIndex: 'ADDITIONALNO', width: 60 },
        { header: '项号属性', dataIndex: 'ITEMNOATTRIBUTE', width: 80 },
        { header: '商品名称', dataIndex: 'COMMODITYNAME', width: 150, renderer: ViewAll },
        { header: '规格型号', dataIndex: 'SPECIFICATIONSMODEL', width: 150, renderer: ViewAll },
        { header: '成交单位', dataIndex: 'UNIT', width: 80, renderer: renderOrder },
        { header: '委托企业', dataIndex: 'BUSIUNITNAME', width: 200, renderer: ViewAll },
        { header: '提交时间', dataIndex: 'SUBMITTIME', width: 150 }
        ],
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
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
                    ENTERPRISECODE: Ext.getCmp('s_combo_enterprise').getValue(), RECORDINFORID: Ext.getCmp('s_combo_recordid').getValue(),
                    ITEMNO: Ext.getCmp("field_ITEMNO").getValue(), HSCODE: Ext.getCmp('field_HSCODE').getValue(),
                    OPTIONS: Ext.getCmp('s_combo_optionstatus').getValue(), STATUS: Ext.getCmp("s_combo_status").getValue(),
                    DATE_START: Ext.Date.format(Ext.getCmp("date_start").getValue(), 'Y-m-d H:i:s'), DATE_END: Ext.Date.format(Ext.getCmp("date_end").getValue(), 'Y-m-d H:i:s')
                }
            },
            load: function () {
                var total_cp_Go = store_RecrodDetail_cp_Go.getProxy().getReader().rawData.total;
                Ext.getCmp("tabpanel").items.items[1].setTitle("成品_申请(" + total_cp_Go + ")");
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
        bbar: pgbar_cp_Go,
        enableColumnHide: false,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: 'ID', dataIndex: 'ID', hidden: true },
        { header: '变动状态', dataIndex: 'OPTIONS', width: 80, renderer: renderOrder },
        { header: '申请状态', dataIndex: 'STATUS', width: 80, renderer: renderOrder },
        { header: '账册号', dataIndex: 'CODE', width: 130 },
        { header: '项号', dataIndex: 'ITEMNO', width: 80 },
        { header: 'HS编码', dataIndex: 'HSCODE', width: 100 },
        { header: '附加码', dataIndex: 'ADDITIONALNO', width: 60 },
        { header: '项号属性', dataIndex: 'ITEMNOATTRIBUTE', width: 80 },
        { header: '商品名称', dataIndex: 'COMMODITYNAME', width: 150, renderer: ViewAll },
        { header: '规格型号', dataIndex: 'SPECIFICATIONSMODEL', width: 150, renderer: ViewAll },
        { header: '成交单位', dataIndex: 'UNIT', width: 80, renderer: renderOrder },
        { header: '委托企业', dataIndex: 'BUSIUNITNAME', width: 200, renderer: ViewAll },
        { header: '提交时间', dataIndex: 'SUBMITTIME', width: 150 }
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
    Ext.getCmp("s_combo_enterprise").setValue("");
    Ext.getCmp("s_combo_recordid").setValue("");
    Ext.getCmp("field_ITEMNO").setValue("");
    Ext.getCmp("field_HSCODE").setValue("");
    Ext.getCmp("s_combo_optionstatus").setValue("");
    Ext.getCmp("s_combo_status").setValue("");
    Ext.getCmp("date_start").setValue("");
    Ext.getCmp("date_end").setValue("");
}

function Export_Audit() {
    var enterprisecode = Ext.getCmp('s_combo_enterprise').getValue(); enterprisecode = enterprisecode == null ? "" : enterprisecode;
    var recordid = Ext.getCmp('s_combo_recordid').getValue(); recordid = recordid == null ? "" : recordid;
    var itemno = Ext.getCmp('field_ITEMNO').getValue(); var hscode = Ext.getCmp('field_HSCODE').getValue();
    var options = Ext.getCmp('s_combo_optionstatus').getValue(); options = options == null ? "" : options;
    var status = Ext.getCmp('s_combo_status').getValue(); status = status == null ? "" : status;
    var date_start = Ext.Date.format(Ext.getCmp("date_start").getValue(), 'Y-m-d H:i:s'); var date_end = Ext.Date.format(Ext.getCmp("date_end").getValue(), 'Y-m-d H:i:s');

    $('#e_options').val(JSON.stringify(optionstatus_js_data));
    $('#e_status').val(JSON.stringify(status_js_data));
    $('#e_unit').val(JSON.stringify(common_data_unit));

    var path = '/RecordInfor/Export_Audit?ENTERPRISECODE=' + enterprisecode + '&RECORDINFORID=' + recordid + '&ITEMNO=' + itemno
        + '&HSCODE=' + hscode + '&OPTIONS=' + options + '&STATUS=' + status + '&DATE_START=' + date_start + '&DATE_END=' + date_end;
    $('#exportform').attr("action", path).submit();

   
}