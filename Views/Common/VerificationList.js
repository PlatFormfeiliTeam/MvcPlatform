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
                    field_DECLARATIONCODE: Ext.getCmp('field_DECLARATIONCODE').getValue(), TRADEMETHOD: Ext.getCmp("s_combo_myfs").getValue()
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
        { header: '报关单号', dataIndex: 'DECLARATIONCODE', width: 130, locked: true },
        { header: '贸易方式', dataIndex: 'TRADEMETHOD', width: 110, locked: true },
        { header: '申报单位代码', dataIndex: 'REPUNITCODE', width: 110 },
        { header: '征免性质', dataIndex: 'KINDOFTAX', width: 110 },
        { header: '申报日期', dataIndex: 'REPTIME', width: 110 },        
        { header: '经营单位代码', dataIndex: 'BUSIUNITCODE', width: 110 },
        { header: '账册号', dataIndex: 'RECORDCODE', width: 110 },
        { header: '创建时间', dataIndex: 'CREATETIME', width: 130 },
        { header: '状态', dataIndex: 'STATUS', width: 100 },
        { header: '类型', dataIndex: 'DATADOURCE', width: 100}
        ],
        listeners:
        {
            'itemdblclick': function (view, record, item, index, e) {
                //opencenterwin("/Common/ListVerificationDetail?CUSNO=" + record.get("CUSNO"), 1200, 600);
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

function importfile(action) {
    
    if (action == "add") {
        importexcel(action, "");
    }

    if (action == "update") {
        var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
        if (recs.length != 1) {
            Ext.MessageBox.alert('提示', '请选择需要修改一笔的记录！');
            return;
        }

        if (recs[0].get("STATUS") != "待比对" && recs[0].get("STATUS") != "比对未通过") {
            Ext.MessageBox.alert('提示', '更新导入，只可选择 待比对、比对未通过 的记录！');
            return;
        }

        importexcel(action, declarationcode);

        //var DECLARATIONCODE = recs[0].get("DECLARATIONCODE");
        //Ext.Ajax.request({
        //    url: '/EnterpriseOrder/GetDeclStatus',
        //    params: { DECLARATIONCODE: DECLARATIONCODE },
        //    success: function (response, success, option) {
        //        var res = Ext.decode(response.responseText);
        //        if (res.success) {
        //            Ext.MessageBox.alert('提示', '比对中或比对通过的，不可修改！');
        //            return;
        //        } else {
        //            importexcel(action, DECLARATIONCODE);
        //        }
        //    }
        //});
    }
}

function importexcel(action, declarationcode) {
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

                    //var formdata = Ext.encode(Ext.getCmp('formpanel_upload').getForm().getValues());

                    Ext.getCmp('formpanel_upload').getForm().submit({
                        url: '/Common/ImExcel_Verification',
                        params: { action: action, declarationcode: declarationcode },//formdata: formdata, 
                        waitMsg: '数据导入中...',
                        success: function (form, action) {
                            Ext.Msg.alert('提示', '保存成功', function () {
                                pgbar.moveFirst();
                                //Ext.getCmp('win_upload').close();
                            });
                        },
                        failure: function (form, action) {//失败要做的事情 
                            Ext.MessageBox.alert("提示", "保存失败", function () { });
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

