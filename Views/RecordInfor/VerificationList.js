var common_data_myfs = [];
var pgbar;

Ext.onReady(function () {

    Ext.Ajax.request({
        url: "/Common/Ini_Base_Data",
        params: { ParaType: 'verification' },
        success: function (response, opts) {
            var commondata = Ext.decode(response.responseText);
            common_data_myfs = commondata.myfs;//贸易方式

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
        fields: ["NAME"],
        data: [{ "NAME": "待比对" }, { "NAME": "比对中" }, { "NAME": "比对通过" }, { "NAME": "比对未通过" }]
    });

    //状态
    var s_combo_STATUS = Ext.create('Ext.form.field.ComboBox', {
        id: 's_combo_STATUS',
        store: store_STATUS,
        fieldLabel: '状态',
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

    var formpanel = Ext.create('Ext.form.Panel', {
        id: 'formpanel',
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.2,
            labelWidth: 70
        },
        items: [
        { layout: 'column', border: 0, margin: '5 0 0 0', items: [field_DECLARATIONCODE, s_combo_myfs, field_CONTRACTNO, s_combo_BUSITYPE, s_combo_STATUS] }
        ]
    });
}

function gridbind() {
    Ext.regModel('VERIFICATION', {
        fields: ['ID', 'DATADOURCE', 'DECLARATIONCODE', 'REPUNITCODE', 'KINDOFTAX', 'REPTIME', 'TRADEMETHOD', 'BUSIUNITCODE'
        , 'RECORDCODE', 'CREATETIME', 'STATUS', 'NOTE', 'CONTRACTNO', 'BUSITYPE', 'INOUTTYPE']
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
                    CONTRACTNO: Ext.getCmp('field_CONTRACTNO').getValue(), BUSITYPE: Ext.getCmp("s_combo_BUSITYPE").getValue(),
                    STATUS: Ext.getCmp("s_combo_STATUS").getValue()
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
            header: '状态', dataIndex: 'STATUS', width: 90, renderer: function (value, meta, record) {
                if (value == "比对未通过") {
                    meta.tdAttr = 'data-qtitle="<font color=red>未通过原因</font>" data-qtip="<font color=blue>' + record.get("NOTE") + '</font>"';
                }
                return value;
            }
        },
        { header: '合同号', dataIndex: 'CONTRACTNO', width: 110 },
        { header: '业务类型', dataIndex: 'BUSITYPE', width: 90 },
        { header: '进出类型', dataIndex: 'INOUTTYPE', width: 80 },
        { header: '报关单号', dataIndex: 'DECLARATIONCODE', width: 130 },
        { header: '申报单位代码', dataIndex: 'REPUNITCODE', width: 110 },
        { header: '征免性质', dataIndex: 'KINDOFTAX', width: 80 },
        {
            header: '申报日期', dataIndex: 'REPTIME', width: 100, renderer: function (value) {                
                if (value == null) { return value;}
                return value.substr(0, 10);
            }
        },
        { header: '贸易方式', dataIndex: 'TRADEMETHOD', width: 80 },
        //{ header: '经营单位代码', dataIndex: 'BUSIUNITCODE', width: 110 },
        { header: '账册号', dataIndex: 'RECORDCODE', width: 110 },
        { header: '类型', dataIndex: 'DATADOURCE', width: 60 },
        { header: '创建时间', dataIndex: 'CREATETIME', width: 130 }
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


function renderver(value, cellmeta, record, rowIndex, columnIndex, store) {
    //var rtn = "";
    //var dataindex = cellmeta.column.dataIndex;
    //switch (dataindex) {
    //    case "FLAG":
    //        var rec = store_flag.findRecord('CODE', value);
    //        if (rec) {
    //            rtn = rec.get("NAME");
    //        }
    //        break;
    //    case "OLDFILENAME":
    //        rtn = "<a href='" + record.get("FILEPATH") + "' target='_blank'>" + record.get("OLDFILENAME") + "</a>";
    //        break;
    //}
    //return rtn;
}

//重置
function Reset() {
    Ext.each(Ext.getCmp('formpanel').getForm().getFields().items, function (field) {
        field.reset();
    });
}

//查询
function Select() {
    pgbar.moveFirst();
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
                                pgbar.moveFirst();
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
                                pgbar.moveFirst();
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

//-----------------------------------------------------------------------win show-------------------------------------------------------------------------------

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
        id: 'NOTE ', name: 'NOTE ', fieldLabel: '<font color=red>未通过原因</font>', readOnly: true, value: recs[0].get("NOTE"), flex: 1
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
        minHeight: 170,
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

function grid_ini_detail(data) {
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