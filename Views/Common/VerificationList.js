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

    var formpanel = Ext.create('Ext.form.Panel', {
        id: 'formpanel',
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.25,
            labelWidth: 70
        },
        items: [
        { layout: 'column', border: 0, margin: '5 0 0 0', items: [field_DECLARATIONCODE, s_combo_myfs] }
        ]
    });
}

function gridbind() {
    Ext.regModel('VERIFICATION', {
        fields: ['ID', 'DATADOURCE', 'DECLARATIONCODE', 'REPUNITCODE', 'KINDOFTAX', 'REPTIME', 'TRADEMETHOD', 'BUSIUNITCODE'
        , 'RECORDCODE', 'CREATETIME', 'STATUS']
    });

    var store_verification = Ext.create('Ext.data.JsonStore', {
        model: 'VERIFICATION',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/Common/loadverification',
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
                    DECLARATIONCODE: Ext.getCmp('field_DECLARATIONCODE').getValue(), TRADEMETHOD: Ext.getCmp("s_combo_myfs").getValue()
                }
            }
        }
    });

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
        { header: '状态', dataIndex: 'STATUS', width: 100 },
        { header: '报关单号', dataIndex: 'DECLARATIONCODE', width: 130 },
        { header: '申报单位代码', dataIndex: 'REPUNITCODE', width: 110 },
        { header: '征免性质', dataIndex: 'KINDOFTAX', width: 110 },
        {
            header: '申报日期', dataIndex: 'REPTIME', width: 110, renderer: function (value) {
                return value.substr(0, 10);
            }
        },
        { header: '贸易方式', dataIndex: 'TRADEMETHOD', width: 110 },
        { header: '经营单位代码', dataIndex: 'BUSIUNITCODE', width: 110 },
        { header: '账册号', dataIndex: 'RECORDCODE', width: 110 },        
        { header: '类型', dataIndex: 'DATADOURCE', width: 100 },
        { header: '创建时间', dataIndex: 'CREATETIME', width: 130 }
        ],
        listeners:
        {
            'itemdblclick': function (view, record, item, index, e) {
                //opencenterwin("/Common/VerificationListDetail?DECLARATIONCODE=" + record.get("DECLARATIONCODE"), 1000, 500);
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
                        url: '/Common/ImExcel_Verification',
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

function errorwin(jsondata) {
    var store_error = Ext.create('Ext.data.JsonStore', {
        fields: ['报关单号', '申报单位代码', '征免性质', '申报日期', '贸易方式', '经营单位代码', '账册号', 'ERRORMSG'],
        data: jsondata
    })
    var griderror = Ext.create('Ext.grid.Panel', {
        id: 'griderror',
        store: store_error,
        enableColumnHide: false,
        height: 300,
        columns: [
         { xtype: 'rownumberer', width: 35 },
        { header: '报关单号', dataIndex: '报关单号', width: 110 },
        { header: '申报单位代码', dataIndex: '申报单位代码', width: 100 },
        { header: '征免性质', dataIndex: '征免性质', width: 80 },
        { header: '申报日期', dataIndex: '申报日期', width: 100 },
        { header: '贸易方式', dataIndex: '贸易方式', width: 80 },
        { header: '经营单位代码', dataIndex: '经营单位代码', width: 100 },
        { header: '账册号', dataIndex: '账册号', width: 110 },
        { header: '<font color="red"><b>错误信息</b></font>', dataIndex: 'ERRORMSG', width: 200 },
        ]
    });

    var win_error = Ext.create("Ext.window.Window", {
        id: "win_error",
        title: '<font style="font-size:14px;">错误信息</font>',
        width: 1000,
        height: 350,
        modal: true,
        items: [Ext.getCmp('griderror')]
    });
    win_error.show();

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
        id: 'REPTIME', name: 'REPTIME', fieldLabel: '申报日期', readOnly: true, value: recs[0].get("REPTIME").substr(0, 10)
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
                { layout: 'column', height: 42, border: 0, items: [field_TRADEMETHOD, field_BUSIUNITCODE, field_RECORDCODE, field_DATADOURCE] },
                { layout: 'column', height: 42, border: 0, items: [field_STATUS, field_CREATETIME] }
        ]
    });
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
            url: '/Common/loadVerificationDetail_D',
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
        { header: '总价', dataIndex: 'TOTALPRICE', width: 100 },
        { header: '报关单号', dataIndex: 'DECLARATIONCODE', width: 130 }
        ],
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });
}