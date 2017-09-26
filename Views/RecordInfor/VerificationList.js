var common_data_myfs = [], ommon_data_unit = [];

Ext.onReady(function () {

    Ext.Ajax.request({
        url: "/Common/Ini_Base_Data",
        params: { ParaType: 'verification' },
        success: function (response, opts) {
            var commondata = Ext.decode(response.responseText);
            common_data_myfs = commondata.myfs;//贸易方式
            common_data_unit = commondata.unit;//单位

            initSearch();
            gridbind();
        }
    });

});

function initSearch() {

    //报关单号
    var field_DECLARATIONCODE = Ext.create('Ext.form.field.Text', {
        id: 'field_DECLARATIONCODE',
        fieldLabel: '报关单号',
        name: 'DECLARATIONCODE'
    });
    var store_myfs = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: common_data_myfs,
        listeners: {
            load: function () {
                store_myfs.load();
            }
        }
    });
    //贸易方式
    var s_combo_myfs = Ext.create('Ext.form.field.ComboBox', {
        id: 's_combo_myfs',
        store: store_myfs,
        fieldLabel: '贸易方式',
        displayField: 'NAME',
        name: 'FLAG',
        valueField: 'CODE',
        triggerAction: 'all',
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.expand();
                    cb.store.clearFilter();
                }
            }
        },
        listConfig: {
            maxHeight: 150
        }
    });
    
    //合同号
    var field_CONTRACTNO = Ext.create('Ext.form.field.Text', {
        id: 'field_CONTRACTNO',
        fieldLabel: '合同号',
        name: 'CONTRACTNO'
    });

    var store_BUSITYPE = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: common_data_busitype
    });

    //业务类型
    var s_combo_BUSITYPE = Ext.create('Ext.form.field.ComboBox', {
        id: 's_combo_BUSITYPE',
        store: store_BUSITYPE,
        fieldLabel: '业务类型',
        displayField: 'NAME',
        name: 'BUSITYPE',
        valueField: 'NAME',
        triggerAction: 'all',
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.expand();
                    cb.store.clearFilter();
                }
            }
        },
        listConfig: {
            maxHeight: 150
        }
    });

    var store_STATUS = Ext.create("Ext.data.JsonStore", {
        fields: ["NAME", "CODE"],
        data: verstatus_data_search
    });

    //状态
    var s_combo_STATUS = Ext.create('Ext.form.field.ComboBox', {
        id: 's_combo_STATUS',
        store: store_STATUS,
        fieldLabel: '比对状态',
        displayField: 'NAME',
        name: 'STATUS',
        valueField: 'CODE',
        triggerAction: 'all',
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.expand();
                    cb.store.clearFilter();
                }
            }
        },
        listConfig: {
            maxHeight: 150
        }
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

    var formpanel = Ext.create('Ext.form.Panel', {
        id: 'formpanel',
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.25,
            labelWidth: 70
        },
        items: [
        { layout: 'column', border: 0, margin: '5 0 0 0', items: [field_DECLARATIONCODE, s_combo_myfs, field_CONTRACTNO, s_combo_BUSITYPE] },
        { layout: 'column', border: 0, margin: '5 0 0 0', items: [s_combo_STATUS, s_date_container] }
        ]
    });
}

function gridbind() {
    Ext.regModel('VERIFICATION', {
        fields: ['ID', 'DATADOURCE', 'DECLARATIONCODE', 'REPUNITCODE', 'KINDOFTAX', 'REPTIME', 'TRADEMETHOD', 'BUSIUNITCODE'
        , 'RECORDCODE', 'CREATETIME', 'STATUS', 'NOTE', 'CONTRACTNO', 'BUSITYPE', 'INOUTTYPE','CUSTOMSSTATUS']
    });

    var store_verification = Ext.create('Ext.data.JsonStore', {
        model: 'VERIFICATION',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/RecordInfor/loadverification',
            reader: {
                root: 'rows',
                type: 'json',
                totalProperty: 'total'
            }
        },
        autoLoad: true,
        listeners: {
            beforeload: function () {
                store_verification.getProxy().extraParams = {
                    DECLARATIONCODE: Ext.getCmp('field_DECLARATIONCODE').getValue(), TRADEMETHOD: Ext.getCmp("s_combo_myfs").getValue(),
                    CONTRACTNO: Ext.getCmp('field_CONTRACTNO').getValue(), BUSITYPE: Ext.getCmp('s_combo_BUSITYPE').getValue(),
                    STATUS: Ext.getCmp('s_combo_STATUS').getValue(),
                    DATE_START: Ext.Date.format(Ext.getCmp("s_date_start").getValue(), 'Y-m-d H:i:s'), DATE_END: Ext.Date.format(Ext.getCmp("s_date_end").getValue(), 'Y-m-d H:i:s')
                }
            }
        }
    });
    Ext.tip.QuickTipManager.init();
    pgbar = Ext.create('Ext.toolbar.Paging', {
        id: 'pgbar',
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_verification,
        displayInfo: true
    });

    var gridpanel = Ext.create('Ext.grid.Panel', {
        id: "gridpanel",
        renderTo: "appConId",
        store: store_verification,
        height: 500,
        selModel: { selType: 'checkboxmodel' },
        bbar: pgbar,
        enableColumnHide: false,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: 'ID', dataIndex: 'ID', hidden: true },
        {
            header: '比对状态', dataIndex: 'STATUS', width: 90, renderer: function (value, meta, record) {
                if (value == "比对未通过") {
                    meta.tdAttr = 'data-qtitle="<font color=red>未通过原因</font>" data-qtip="<font color=blue>' + record.get("NOTE") + '</font>"';
                }
                return value;
            }
        },
        { header: '海关状态', dataIndex: 'CUSTOMSSTATUS', width: 90 },
        { header: '合同号', dataIndex: 'CONTRACTNO', width: 110 },
        { header: '业务类型', dataIndex: 'BUSITYPE', width: 90 },
        { header: '进出类型', dataIndex: 'INOUTTYPE', width: 65 },
        { header: '报关单号', dataIndex: 'DECLARATIONCODE', width: 150 },
        { header: '申报单位代码', dataIndex: 'REPUNITCODE', width: 100 },
        { header: '征免性质', dataIndex: 'KINDOFTAX', width: 65 },
        {
            header: '申报日期', dataIndex: 'REPTIME', width: 90, renderer: function (value) {                
                if (value == null) { return value;}
                return value.substr(0, 10);
            }
        },
        { header: '贸易方式', dataIndex: 'TRADEMETHOD', width: 65 },
        //{ header: '经营单位代码', dataIndex: 'BUSIUNITCODE', width: 110 },
        { header: '账册号', dataIndex: 'RECORDCODE', width: 110 },
        {
            header: '类型', dataIndex: 'DATADOURCE', width: 60, renderer: function (value, meta, record) {
                if (value == "线下") {
                    return '<font color=blue>' + value + '</font>';
                }
                return value;
            }
        },
        { header: '创建时间', dataIndex: 'CREATETIME', width: 140 }
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

function Reset() {
    Ext.each(Ext.getCmp('formpanel').getForm().getFields().items, function (field) {
        field.reset();
    });
}

function Select() {
    Ext.getCmp("pgbar").moveFirst();
}

function importfile() {
    var modulelable = Ext.create('Ext.form.Label', {
        id: 'modulelable',
        name: 'modulelable',
        html: '<a href="/FileUpload/VerificationData.xlsx" style="cursor:pointer">模板下载</a>',
        cls: "lab", flex: .1
    });

    var uploadfile = Ext.create('Ext.form.field.File', {
        id: 'UPLOADFILE', name: 'UPLOADFILE', fieldLabel: '导入数据', labelAlign: 'right', msgTarget: 'under'
        , margin: '15 5 15 5', anchor: '90%', buttonText: '浏览文件', regex: /.*(.xls|.xlsx)$/, regexText: '只能上传xls,xlsx文件'
        , allowBlank: false, blankText: '文件不能为空!', flex: .9
    });

    var field_CUS = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [uploadfile, modulelable]
    }

    var formpanel_upload = Ext.create('Ext.form.Panel', {
        id: 'formpanel_upload', height: 135,
        buttonAlign: 'center',
        items: [field_CUS],
        buttons: [{
            text: '确认上传',
            handler: function () {
                if (Ext.getCmp('formpanel_upload').getForm().isValid()) {
                    
                    Ext.getCmp('formpanel_upload').getForm().submit({
                        url: '/RecordInfor/ImExcel_Verification',
                        waitMsg: '数据导入中...',
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            var json = result.json; var msg = "";
                            if (json.length>0) {
                                msg = "操作完成";
                            } else {
                                msg = "保存成功";
                            }

                            Ext.Msg.alert('提示', msg, function () {
                                Ext.getCmp("pgbar").moveFirst();
                                //Ext.getCmp('win_upload').close();
                                if (json.length > 0) {
                                    errorwin(json);
                                }
                            });
                        },
                        failure: function (form, action) {//失败要做的事情 
                            var result = Ext.decode(action.response.responseText);
                            var errormsg = result.error;

                            Ext.MessageBox.alert("提示", errormsg, function () {
                                Ext.getCmp("pgbar").moveFirst();
                            });
                        }
                    });

                }
            }
        }]
    });

    var win_upload = Ext.create("Ext.window.Window", {
        id: "win_upload",
        title: '预录导入',
        width: 600,
        height: 170,
        modal: true,
        items: [Ext.getCmp('formpanel_upload')]
    });
    win_upload.show();
}

function VeriList() {
    var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要比对的记录！');
        return;
    }
    var datadource = recs[0].get("DATADOURCE");
    var bf = false; var bf_type = false;//var jsondata="[]";
    Ext.each(recs, function (rec) {
        if (rec.get("STATUS") == "比对中") { bf = true; }
        if (rec.get("DATADOURCE") != datadource) { bf_type = true; }
        //jsondata.push({ "DECLARATIONCODE": rec.get("DECLARATIONCODE"), "DATADOURCE": rec.get("DATADOURCE") });
    });
    if (bf) {
        Ext.Msg.alert("提示", "比对中的记录 不能 发送核销比对!");
        return;
    }
    if (bf_type) {
        Ext.Msg.alert("提示", "线上、线下的数据需要分开发送核销比对!");
        return;
    }

    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据保存中，请稍等..." });
    myMask.show();

    //var jsondata_str = Ext.encode(jsondata);
    var declarationcode_list = Ext.encode(Ext.pluck(Ext.pluck(recs, 'data'), 'DECLARATIONCODE'));

    Ext.Ajax.request({
        url: '/RecordInfor/VeriList',        
        params: { declarationcode_list: declarationcode_list, datadource: datadource },//params: { jsondata_str: jsondata_str },
        success: function (response, option) {
            myMask.hide();
            var result = Ext.decode(response.responseText);
            if (result.success) {
                var json = result.json; var msg = "";
                if (json.length > 0) {
                    msg = "操作完成";
                } else {
                    msg = "保存成功";
                }

                Ext.Msg.alert('提示', msg, function () {
                    Ext.getCmp("pgbar").moveFirst();
                    if (json.length > 0) {
                        errorwin(json);
                    }
                });
            }
            else {
                var result = Ext.decode(response.responseText);
                var errormsg = result.error;
                Ext.MessageBox.alert("提示", errormsg, function () {
                    Ext.getCmp("pgbar").moveFirst();
                });
            }


        }
    });

}

function DeleteVeri() {
    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.Msg.alert("提示", "请选择删除记录!");
        return;
    }
    var declcodes = ""; var bf = false;
    Ext.each(recs, function (rec) {
        if (rec.get("STATUS") == null || rec.get("STATUS") == "比对中") { bf = true; }
        declcodes += "'" + rec.get("DECLARATIONCODE") + "',";
    });
    if (bf) {
        Ext.Msg.alert("提示", "未必对或比对中的记录不能删除!");
        return;
    }
    declcodes = declcodes.substr(0, declcodes.length - 1);

    Ext.MessageBox.confirm("提示", "确定要删除所选择的记录吗？", function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/RecordInfor/DeleteVeri',
                params: { declcodes: declcodes },
                success: function (response, success, option) {
                    var res = Ext.decode(response.responseText);
                    var msgs = "";
                    if (res.success) { msgs = "删除成功！"; }
                    else { msgs = "删除失败！"; }

                    Ext.MessageBox.alert('提示', msgs, function (btn) {
                        Ext.getCmp("pgbar").moveFirst();
                    });
                }
            });
        }
    });
}

//-----------------------------------------------------------------------win show-------------------------------------------------------------------------------

function Open() {
    var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
    if (recs.length != 1) {
        Ext.Msg.alert("提示", "请选择一笔记录!");
        return;
    }

    form_ini_detail();
    grid_ini_detail();

    var bbar = '<div class="btn-group" role="group">'
                    + '<button type="button" onclick="VeriList_D()" id="btn_VeriList_D" class="btn btn-primary btn-sm"><i class="fa fa-send"></i>&nbsp;核销比对</button>'
                + '</div>';
    var bbar = Ext.create('Ext.toolbar.Toolbar', {
        items: ['->', bbar, '->']
    });

    var win = Ext.create("Ext.window.Window", {
        id: "win_d",
        title: '<font style="font-size:14px;">核销比对信息</font>',
        width: 1000,
        height: 550,
        modal: true,
        bbar: bbar,
        items: [Ext.getCmp('f_formpanel'), Ext.getCmp('gridpanel_d')]
    });
    win.show();

    Ext.Ajax.request({
        url: "/RecordInfor/loadVerificationDetail_D",
        params: { declartioncode: recs[0].get("DECLARATIONCODE"), status: recs[0].get("STATUS") },
        success: function (response, opts) {
            var data = Ext.decode(response.responseText);

            Ext.getCmp("f_formpanel").getForm().setValues(recs[0].data);
            Ext.getCmp('gridpanel_d').store.loadData(data.verisubdata);

            if (recs[0].get("STATUS") != "比对未通过") {
                Ext.getCmp("NOTE").hide();
            }

            if (recs[0].get("DATADOURCE") == "线下") {
                Ext.Array.each(Ext.getCmp("f_formpanel").getForm().getFields().items, function (item) {
                    if (item.id != "DATADOURCE" && item.id != "CUSTOMSSTATUS" && item.id != "CREATETIME" && item.id != "STATUS" && item.id != "NOTE") {
                        item.setReadOnly(false);
                    }
                });
                document.getElementById("btn_VeriList_D").style.visibility = "visible";
                Ext.getCmp("btn_gird_add").show();
                Ext.getCmp("btn_gird_del").show();
            } else {
                Ext.Array.each(Ext.getCmp("f_formpanel").getForm().getFields().items, function (item) {
                    item.setFieldStyle('background-color: #CECECE; background-image: none;');
                    item.setReadOnly(true);
                });
                document.getElementById("btn_VeriList_D").style.visibility = "hidden";
                Ext.getCmp("btn_gird_add").hide();
                Ext.getCmp("btn_gird_del").hide();
            }

        }
    });
}

function form_ini_detail() {

    var field_DECLARATIONCODE = Ext.create('Ext.form.field.Text', { id: 'DECLARATIONCODE', name: 'DECLARATIONCODE', fieldLabel: '报关单号', allowBlank: false, blankText: '报关单号不能为空!' });

    var field_REPUNITCODE = Ext.create('Ext.form.field.Text', { id: 'REPUNITCODE', name: 'REPUNITCODE', fieldLabel: '申报单位代码' });

    var field_KINDOFTAX = Ext.create('Ext.form.field.Text', { id: 'KINDOFTAX', name: 'KINDOFTAX', fieldLabel: '征免性质' });

    var field_REPTIME = Ext.create('Ext.form.field.Text', { id: 'REPTIME', name: 'REPTIME', fieldLabel: '申报日期' });

    var field_TRADEMETHOD = Ext.create('Ext.form.field.Text', { id: 'TRADEMETHOD', name: 'TRADEMETHOD', fieldLabel: '贸易方式' });

    var field_BUSIUNITCODE = Ext.create('Ext.form.field.Text', { id: 'BUSIUNITCODE', name: 'BUSIUNITCODE', fieldLabel: '经营单位代码', allowBlank: false, blankText: '报关单号不能为空!' });

    var field_RECORDCODE = Ext.create('Ext.form.field.Text', { id: 'RECORDCODE', name: 'RECORDCODE', fieldLabel: '账册号' });
    
    var field_DATADOURCE = Ext.create('Ext.form.field.Text', {
        id: 'DATADOURCE', name: 'DATADOURCE', fieldLabel: '类型',
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none; color: blue;'
    });

    var field_CUSTOMSSTATUS = Ext.create('Ext.form.field.Text', {
        id: 'CUSTOMSSTATUS', name: 'CUSTOMSSTATUS', fieldLabel: '海关状态',
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    var field_CONTRACTNO = Ext.create('Ext.form.field.Text', { id: 'CONTRACTNO', name: 'CONTRACTNO', fieldLabel: '合同号' });

    var field_BUSITYPE = Ext.create('Ext.form.field.Text', { id: 'BUSITYPE', name: 'BUSITYPE', fieldLabel: '业务类型' });

    var field_INOUTTYPE = Ext.create('Ext.form.field.Text', { id: 'INOUTTYPE', name: 'INOUTTYPE', fieldLabel: '进出类型' });
    
    var field_CREATETIME = Ext.create('Ext.form.field.Text', {
        id: 'CREATETIME', name: 'CREATETIME', fieldLabel: '创建时间',
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    var field_STATUS = Ext.create('Ext.form.field.Text', {
        id: 'STATUS', name: 'STATUS', fieldLabel: '状态', 
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    var field_NOTE = Ext.create('Ext.form.field.Text', {
        id: 'NOTE', name: 'NOTE', fieldLabel: '<font color=red>未通过原因</font>', flex: 1, 
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none; color: blue;'
    });

    var f_NOTE_container = { xtype: 'fieldcontainer', layout: 'hbox', margin: 0, columnWidth: .5, items: [field_NOTE] };

    var f_formpanel = Ext.create('Ext.form.Panel', {
        id: 'f_formpanel',
        minHeight: 180,
        border: 0,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: .25,
            labelAlign: 'right',
            labelSeparator: ''
        },
        items: [
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_DECLARATIONCODE, field_REPUNITCODE, field_KINDOFTAX, field_REPTIME] },
                { layout: 'column', height: 42, border: 0, items: [field_TRADEMETHOD, field_BUSIUNITCODE, field_RECORDCODE, field_CONTRACTNO] },
                { layout: 'column', height: 42, border: 0, items: [field_BUSITYPE, field_INOUTTYPE, field_CUSTOMSSTATUS, field_DATADOURCE] },
                { layout: 'column', height: 42, border: 0, items: [field_CREATETIME, field_STATUS, f_NOTE_container] }
        ]
    });
}

function grid_ini_detail() {
    var data_verisub = [];
    var store_d = Ext.create('Ext.data.JsonStore', {
        storeId: 'store_d',
        fields: ['DECLARATIONCODE', 'ORDERNO', 'ITEMNO', 'COMMODITYNO', 'COMMODITYNAME', 'TAXPAID', 'CADQUANTITY', 'CADUNIT', 'CURRENCYCODE', 'TOTALPRICE'],
        data: data_verisub
    });

    var g_tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [
            '->',
            {
                text: '<span class="icon iconfont" style="font-size:10px">&#xe622;</span>&nbsp;添 加', id: 'btn_gird_add',
                handler: function () {
                    var p = {
                        DECLARATIONCODE: '', ORDERNO: '', ITEMNO: '', COMMODITYNO: '', COMMODITYNAME: '', TAXPAID: '', CADQUANTITY: '', CADUNIT: '', CURRENCYCODE: '', TOTALPRICE: ''
                    };
                    store_d.insert(store_d.data.length, p);
                }
            },
            {
                text: '<span class="icon iconfont" style="font-size:10px">&#xe6d3;</span>&nbsp;删 除', id: 'btn_gird_del',
                handler: function () {
                    var recs = grid.getSelectionModel().getSelection();
                    if (recs.length > 0) {
                        store_d.remove(recs);
                    }
                }
            }
        ]
    });

    var grid = Ext.create('Ext.grid.Panel', {
        id: 'gridpanel_d',
        store: store_d,
        height: 300,
        tbar: g_tbar,
        selModel: { selType: 'checkboxmodel' },
        enableColumnHide: false,
        columns: [
            { xtype: 'rownumberer', width: 35 },
            { header: '序号', dataIndex: 'ORDERNO', width: 40, editor: { xtype: "numberfield", decimalPrecision: 0, selectOnFocus: true } },
            { header: '项号', dataIndex: 'ITEMNO', width: 40, editor: { xtype: "numberfield", decimalPrecision: 0, selectOnFocus: true } },
            { header: '商品编号', dataIndex: 'COMMODITYNO', width: 110, editor: { xtype: "numberfield", decimalPrecision: 0, selectOnFocus: true } },
            { header: '商品名称', dataIndex: 'COMMODITYNAME', width: 200, editor: { xtype: "textfield", selectOnFocus: true } },
            { header: '征免', dataIndex: 'TAXPAID', width: 100, editor: { xtype: "textfield", selectOnFocus: true } },
            { header: '成交数量', dataIndex: 'CADQUANTITY', width: 60, editor: { xtype: "textfield", selectOnFocus: true } },
            { header: '成交单位', dataIndex: 'CADUNIT', width: 60, editor: { xtype: "textfield", selectOnFocus: true } },
            { header: '币制', dataIndex: 'CURRENCYCODE', width: 40, editor: { xtype: "textfield", selectOnFocus: true } },
            { header: '总价', dataIndex: 'TOTALPRICE', width: 100, editor: { xtype: "textfield", selectOnFocus: true } }
        ],
        listeners:{
            beforeedit: {//控制是否可以编辑
                fn: function (editor, e, eOpts) {
                    if (Ext.getCmp("DATADOURCE").getValue() == "线上") {
                        return false;
                    }
                    return true;
                }
            }
        },
        viewConfig: {
            enableTextSelection: true
        },
        plugins: [
            Ext.create('Ext.grid.plugin.CellEditing', {//Ext.create('Ext.grid.plugin.RowEditing', {
                clicksToEdit: 1
            })
        ],
        forceFit: true
    });
}

function VeriList_D() {
    if (!Ext.getCmp('f_formpanel').getForm().isValid()) {
        return;
    }
    var formdata = Ext.encode(Ext.getCmp('f_formpanel').getForm().getValues());
    var verisubdata = Ext.encode(Ext.pluck(Ext.data.StoreManager.lookup('store_d').data.items, 'data'));

    var mask = new Ext.LoadMask(Ext.get(Ext.getBody()), { msg: "数据保存中，请稍等..." });

    mask.show();
    Ext.Ajax.request({
        url: "/RecordInfor/VeriList_D_Verification",
        params: { formdata: formdata, verisubdata: verisubdata },
        success: function (response, option) {
            if (response.responseText) {
                mask.hide();
                var result = Ext.decode(response.responseText);
                if (result.success) {
                    var json = result.json; var msg = "";
                    if (json.length > 0) {
                        msg = "操作完成";
                    } else {
                        msg = "保存成功";
                    }

                    Ext.Msg.alert('提示', msg, function () {
                        Ext.getCmp("pgbar").moveFirst();
                        if (json.length > 0) {
                            errorwin(json);
                        } else {
                            Ext.getCmp("STATUS").setValue("待比对");
                        }
                    });
                }
                else {
                    var result = Ext.decode(response.responseText);
                    var errormsg = result.error;
                    Ext.MessageBox.alert("提示", errormsg, function () {
                        Ext.getCmp("pgbar").moveFirst();
                    });
                }
            }
        }
    });

}



/*
function Open() {
    var recs = Ext.getCmp('gridpanel').getSelectionModel().getSelection();
    if (recs.length != 1) {
        Ext.Msg.alert("提示", "请选择一笔记录!");
        return;
    }
    form_ini_detail(recs);
    grid_ini_detail();

    var win = Ext.create("Ext.window.Window", {
        id: "win_d",
        title: '<font style="font-size:14px;">核销比对信息</font>',
        width: 1000,
        height: 500,
        modal: true,
        items: [Ext.getCmp('f_formpanel'), Ext.getCmp('gridpanel_d')]
    });
    win.show();
}

function form_ini_detail(recs) {

    var field_DECLARATIONCODE = Ext.create('Ext.form.field.Text', {
        id: 'DECLARATIONCODE', name: 'DECLARATIONCODE', fieldLabel: '报关单号', readOnly: true, value: recs[0].get("DECLARATIONCODE")
    });
    var field_REPUNITCODE = Ext.create('Ext.form.field.Text', {
        id: 'REPUNITCODE', name: 'REPUNITCODE', fieldLabel: '申报单位代码', readOnly: true, value: recs[0].get("REPUNITCODE")
    });
    var field_KINDOFTAX = Ext.create('Ext.form.field.Text', {
        id: 'KINDOFTAX', name: 'KINDOFTAX', fieldLabel: '征免性质', readOnly: true, value: recs[0].get("KINDOFTAX")
    });
    var field_REPTIME = Ext.create('Ext.form.field.Text', {
        id: 'REPTIME', name: 'REPTIME', fieldLabel: '申报日期', readOnly: true, value: recs[0].get("REPTIME") == null ? recs[0].get("REPTIME") : recs[0].get("REPTIME").substr(0, 10)
    });
    var field_TRADEMETHOD = Ext.create('Ext.form.field.Text', {
        id: 'TRADEMETHOD', name: 'TRADEMETHOD', fieldLabel: '贸易方式', readOnly: true, value: recs[0].get("TRADEMETHOD")
    });
    var field_BUSIUNITCODE = Ext.create('Ext.form.field.Text', {
        id: 'BUSIUNITCODE', name: 'BUSIUNITCODE', fieldLabel: '经营单位代码', readOnly: true, value: recs[0].get("BUSIUNITCODE")
    });
    var field_RECORDCODE = Ext.create('Ext.form.field.Text', {
        id: 'RECORDCODE', name: 'RECORDCODE', fieldLabel: '账册号', readOnly: true, value: recs[0].get("RECORDCODE")
    });
    var field_DATADOURCE = Ext.create('Ext.form.field.Text', {
        id: 'DATADOURCE', name: 'DATADOURCE', fieldLabel: '类型', readOnly: true, value: recs[0].get("DATADOURCE")
    });
    var field_STATUS = Ext.create('Ext.form.field.Text', {
        id: 'STATUS', name: 'STATUS', fieldLabel: '状态', readOnly: true, value: recs[0].get("STATUS")
    });
    var field_CREATETIME = Ext.create('Ext.form.field.Text', {
        id: 'CREATETIME', name: 'CREATETIME', fieldLabel: '创建时间', readOnly: true, value: recs[0].get("CREATETIME")
    });
    var field_NOTE = Ext.create('Ext.form.field.Text', {
        id: 'NOTE ', name: 'NOTE ', fieldLabel: '<font color=red>未通过原因</font>', flex: 1, readOnly: true, value: recs[0].get("NOTE")
    });
    field_NOTE.setFieldStyle({ color: 'blue' });

    var f_NOTE_container = {
        xtype: 'fieldcontainer',
        layout: 'hbox', margin: 0,
        columnWidth: .5,
        items: [field_NOTE]
    }   

    var f_formpanel = Ext.create('Ext.form.Panel', {
        id: 'f_formpanel',
        minHeight: 140,
        border: 0,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: .25,
            labelAlign: 'right',
            labelSeparator: ''
        },
        items: [
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_DECLARATIONCODE, field_REPUNITCODE, field_KINDOFTAX, field_REPTIME] },
                { layout: 'column', height: 42, border: 0, items: [field_TRADEMETHOD, field_BUSIUNITCODE, field_RECORDCODE,field_CREATETIME ] },
                { layout: 'column', height: 42, border: 0, items: [field_DATADOURCE, field_STATUS, f_NOTE_container] }
        ]
    });

    if (field_STATUS.getValue() != "比对未通过") {
        field_NOTE.hide();
    }     
}

function grid_ini_detail() {
    Ext.regModel('VerificationDetail', {
        fields: ['DECLARATIONCODE', 'ORDERNO', 'ITEMNO', 'COMMODITYNO', 'COMMODITYNAME', 'TAXPAID'
             , 'CADQUANTITY', 'CADUNIT', 'CURRENCYCODE', 'TOTALPRICE']
    });

    var store_d = Ext.create('Ext.data.JsonStore', {
        model: 'VerificationDetail',
        pageSize: 10,
        proxy: {
            type: 'ajax',
            url: '/RecordInfor/loadVerificationDetail_D',
            reader: {
                root: 'rows',
                type: 'json',
                totalProperty: 'total'
            }
        },
        autoLoad: true,
        listeners: {
            beforeload: function () {
                store_d.getProxy().extraParams.declartioncode = Ext.getCmp("DECLARATIONCODE").getValue();
                store_d.getProxy().extraParams.status = Ext.getCmp("STATUS").getValue();
            }
        }
    });

    var pgbar_d = Ext.create('Ext.toolbar.Paging', {
        id: 'pgbar_d',
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_d,
        displayInfo: true
    })

    var grid = Ext.create('Ext.grid.Panel', {
        id: 'gridpanel_d',
        store: store_d,
        height: 300,
        bbar: pgbar_d,
        enableColumnHide: false,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: '序号', dataIndex: 'ORDERNO', width: 40 },
        { header: '项号', dataIndex: 'ITEMNO', width: 40 },
        { header: '商品编号', dataIndex: 'COMMODITYNO', width: 110 },
        { header: '商品名称', dataIndex: 'COMMODITYNAME', width: 200 },
        { header: '征免', dataIndex: 'TAXPAID', width: 100 },
        { header: '成交数量', dataIndex: 'CADQUANTITY', width: 60 },
        { header: '成交单位', dataIndex: 'CADUNIT', width: 60 },
        { header: '币制', dataIndex: 'CURRENCYCODE', width: 40 },
        { header: '总价', dataIndex: 'TOTALPRICE', width: 100 }//,
        //{ header: '报关单号', dataIndex: 'DECLARATIONCODE', width: 130 }
        ],
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });
}
*/

//----------------------------------------------------------------------export win----------------------------------------------------------------------------------
function ExportWin() {
    var bbar_r = '<div class="btn-group" role="group">'
                       + '<form id="exportform_type" name="form" enctype="multipart/form-data" method="post" style="display:inline-block">'
                       + '<button onclick="ExportReport_ver()" type="button" id="btn_Export" class="btn btn-primary btn-sm"><i class="fa fa-level-down"></i>&nbsp;导出</button></form>'
                  + '</div>';

    var bbar = Ext.create('Ext.toolbar.Toolbar', {
        items: ['->', bbar_r]
    });

    //导出格式
    var field_report = Ext.create('Ext.form.RadioGroup', {
        id: 'field_report',
        labelAlign: "right",
        fieldLabel: '导出格式',
        columns: 2,
        vertical: true,
        items: [{ boxLabel: "昆山区内", name: 'rbg_report', inputValue: "0", checked: true }, { boxLabel: "昆山区外", name: 'rbg_report', inputValue: "1" }]
    });

    var formpanel_d = Ext.create('Ext.form.Panel', {
        id: 'formpanel_report',
        border: 0,
        bbar: bbar,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: 1,
            labelAlign: 'right',
            labelSeparator: ''
        },
        items: [{ layout: 'column', height: 42, border: 0, margin: '5 0 0 0', items: [field_report] }]
    });

    var win = Ext.create("Ext.window.Window", {
        id: "win_report",
        //title: '下载报表',
        width: 500,
        height: 120,
        modal: true,
        items: [Ext.getCmp('formpanel_report')]
    });
    win.show();
}


function ExportReport_ver() {
    var myMask = new Ext.LoadMask(Ext.getCmp("win_report"), { msg: "数据导出中，请稍等..." });
    myMask.show();

    var data = {
        UNIT: JSON.stringify(common_data_unit), modifyflag_data: JSON.stringify(modifyflag_data),
        DECLARATIONCODE: Ext.getCmp('field_DECLARATIONCODE').getValue(), TRADEMETHOD: Ext.getCmp("s_combo_myfs").getValue(),
        CONTRACTNO: Ext.getCmp('field_CONTRACTNO').getValue(), BUSITYPE: Ext.getCmp('s_combo_BUSITYPE').getValue(),
        STATUS: Ext.getCmp('s_combo_STATUS').getValue(),
        DATE_START: Ext.Date.format(Ext.getCmp("s_date_start").getValue(), 'Y-m-d'), DATE_END: Ext.Date.format(Ext.getCmp("s_date_end").getValue(), 'Y-m-d'),
        RBGTYPE: Ext.getCmp("field_report").getValue().rbg_report
    }

    Ext.Ajax.request({
        url: '/RecordInfor/ExportReport_ver',
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
                    form: 'exportform_type',
                    success: function (response, option) {
                    }
                });
            }
            myMask.hide();
        }
    });
}