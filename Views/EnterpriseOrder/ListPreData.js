var common_data_jydw = [], common_data_dzfwdw = [];
var store_flag;
var flag_data = [{ "NAME": "未转化", "CODE": "0" }, { "NAME": "已转换", "CODE": "1" }, { "NAME": "转换错误", "CODE": "2" }];
var pgbar;

Ext.onReady(function () {

    Ext.Ajax.request({
        url: "/Common/Ini_Base_Data",
        success: function (response, opts) {
            var commondata = Ext.decode(response.responseText);
            common_data_jydw = commondata.jydw;//经营单位
            common_data_dzfwdw = commondata.dzfwdw;//单证服务单位

            store_flag = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: flag_data });

            initSearch();
            gridbind();
        }
    });

});

function initSearch() {
    var s_combo_flag = Ext.create('Ext.form.field.ComboBox', {
        id: 's_combo_flag',
        editable: false,
        store: store_flag,
        fieldLabel: '转换状态',
        displayField: 'NAME',
        name: 'FLAG',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local'
    });
    //企业编号
    var field_CUSNO = Ext.create('Ext.form.field.Text', {
        id: 'field_CUSNO',
        fieldLabel: '企业编号',
        name: 'CUSNO'
    });
    var start_date = Ext.create('Ext.form.field.Date', {
        id: 'start_date',
        format: 'Y-m-d',
        fieldLabel: '申报日期从'
    })
    var end_date = Ext.create('Ext.form.field.Date', {
        id: 'end_date',
        fieldLabel: '到',
        format: 'Y-m-d'
    })

    var formpanel = Ext.create('Ext.form.Panel', {
        id: 'formpanel',
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.25,
            labelWidth: 70
        },
        items: [
        { layout: 'column', border: 0, margin: '5 0 0 0', items: [field_CUSNO, s_combo_flag, start_date, end_date] }
        ]
    });
}

function gridbind() {
    Ext.regModel('LISTPREDATA', {
        fields: ['ID', 'CUSNO', 'CONTRACTNO', 'REPDATE', 'RECORDCODE', 'INOUTTYPE', 'TRADECODE', 'TRADENAME'
            , 'GOODSNUM', 'GOODSGW', 'GOODSNW', 'OLDFILENAME', 'FLAG', 'CREATETIME', 'FILEPATH', 'CUSTOMERCODE', 'CUSTOMERNAME'
            , 'STATUS', 'CUSTOMSSTATUS', 'DECLARATIONCODE','ORDERCODE','CODE','BUSITYPE'
        ]
    });

    var store_predata = Ext.create('Ext.data.JsonStore', {
        model: 'LISTPREDATA',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/EnterpriseOrder/loadpredata',
            reader: {
                root: 'rows',
                type: 'json',
                totalProperty: 'total'
            }
        },
        autoLoad: true,
        listeners: {
            beforeload: function () {
                store_predata.getProxy().extraParams = {
                    CUSNO: Ext.getCmp('field_CUSNO').getValue(), FLAG: Ext.getCmp("s_combo_flag").getValue(),
                    STARTDATE: Ext.Date.format(Ext.getCmp("start_date").getValue(), 'Y-m-d H:i:s'),
                    ENDDATE: Ext.Date.format(Ext.getCmp("end_date").getValue(), 'Y-m-d H:i:s')
                }
            }
        }
    });

    pgbar = Ext.create('Ext.toolbar.Paging', {
        id: 'pgbar',
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_predata,
        displayInfo: true
    })
    var gridpanel = Ext.create('Ext.grid.Panel', {
        id: "gridpanel",
        renderTo: "appConId",
        store: store_predata,
        height: 500,
        selModel: { selType: 'checkboxmodel' },
        bbar: pgbar,
        enableColumnHide: false,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: 'ID', dataIndex: 'ID', hidden: true },
        { header: '报关状态', dataIndex: 'STATUS', width: 100, renderer: renderpre, locked: true },
        { header: '海关状态', dataIndex: 'CUSTOMSSTATUS', width: 90, locked: true },
        { header: '报关单号', dataIndex: 'DECLARATIONCODE', width: 140, locked: true, renderer: renderpre },
        { header: '对应号', dataIndex: 'CUSNO', width: 130, locked: true },
        { header: '合同号', dataIndex: 'CONTRACTNO', width: 110 },
        { header: '申报日期', dataIndex: 'REPDATE', width: 110},
        { header: '备案号', dataIndex: 'RECORDCODE', width: 110 },
        { header: '进出口类型', dataIndex: 'INOUTTYPE', width: 80 },
        { header: '贸易方式', dataIndex: 'TRADENAME', width: 110 },
        { header: '件数', dataIndex: 'GOODSNUM', width: 110 },
        { header: '毛重', dataIndex: 'GOODSGW', width: 110 },
        { header: '净重', dataIndex: 'GOODSNW', width: 110 },
        { header: '转换标记', dataIndex: 'FLAG', width: 110, renderer: renderpre },
        { header: '创建时间', dataIndex: 'CREATETIME', width: 150 },
        { header: '文件名称', dataIndex: 'OLDFILENAME', width: 300, renderer: renderpre }
        ],
        listeners:
        {
            'itemdblclick': function (view, record, item, index, e) {
                opencenterwin("/EnterpriseOrder/ListPreDataDetail?CUSNO=" + record.get("CUSNO"), 1200, 600);
            }
        },
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });
}


function renderpre(value, cellmeta, record, rowIndex, columnIndex, store) {
    var rtn = "";
    var dataindex = cellmeta.column.dataIndex;
    switch (dataindex) {
        case "STATUS":
            if (value) {
                rtn = orders_tatus[value];
            }
            break;
        case "DECLARATIONCODE":
            if (value) {
                rtn = "<div style='color:red;cursor:pointer; text-decoration:underline;' onclick='FileConsult_Pre(\"" + record.get("CUSNO") + "\",\"" + record.get("ORDERCODE") + "\",\"" + record.get("CODE") + "\",\"" + record.get("BUSITYPE") + "\")'>" + value + "</div>";
            }
            break;
        case "FLAG":
            var rec = store_flag.findRecord('CODE', value);
            if (rec){
                rtn = rec.get("NAME");
            }
            break;
        case "OLDFILENAME":
            rtn = "<a href='" + record.get("FILEPATH") + "' target='_blank'>" + record.get("OLDFILENAME") + "</a>";
            break;
    }
    return rtn;
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

function readfile() {
    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要调阅的记录！');
        return;
    }
    FileConsult_Pre(recs[0].get("CUSNO"), recs[0].get("ORDERCODE"), recs[0].get("CODE"), recs[0].get("BUSITYPE"));
}

function FileConsult_Pre(CUSNO, ORDERCODE, PREDECLCODE, BUSITYPE) {
    opencenterwin("/Common/FileConsult_Pre?CUSNO=" + CUSNO + "&ORDERCODE=" + ORDERCODE + "&BUSITYPE=" + BUSITYPE + "&PREDECLCODE=" + PREDECLCODE, 1200, 900);
}

function multiprint_pre() {
    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要打印的记录！');
        return;
    }
    var data = "[";
    for (var i = 0; i < recs.length; i++) {
        if (i == recs.length - 1) {
            data += "{BUSITYPE:'" + recs[i].get("BUSITYPE") + "',CODE:'" + recs[i].get("CODE") + "'}]"
        }
        else {
            data += "{BUSITYPE:'" + recs[i].get("BUSITYPE") + "',CODE:'" + recs[i].get("CODE") + "'},"
        }
    }
    opencenterwin("/Common/MultiPrint?menuxml=dec_mul_pre&source=declare&data=" + data, 1100, 700);
}

function declare_pre() {
    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要申报的记录！');
        return;
    }

    if (recs[0].get("STATUS") == null) {
        Ext.MessageBox.alert('提示', '报关状态为空，无法申报！');
        return;
    }
    if (parseInt(recs[0].get("STATUS")) < 80) {
        Ext.MessageBox.alert('提示', '报关状态小于预录完成，无法申报！');
        return;
    }
    if (parseInt(recs[0].get("STATUS")) >= 100) {
        Ext.MessageBox.alert('提示', '此单已经申报！');
        return;
    }

    Ext.MessageBox.confirm("提示", "确定要申报所选择的记录吗？", function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/EnterpriseOrder/Declare_Pre',
                params: { cusno: recs[0].get("CUSNO"), ordercode: recs[0].get("ORDERCODE"), declcode: recs[0].get("CODE") },
                success: function (response, success, option) {
                    var res = Ext.decode(response.responseText);
                    if (res.success) {
                        Ext.MessageBox.alert('提示', '申报成功！', function () {
                            pgbar.moveFirst();
                        });
                    }
                    else {
                        Ext.MessageBox.alert('提示', '申报失败！');
                    }
                }
            });
        }
    });
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

        var cusno = recs[0].get("CUSNO");
        Ext.Ajax.request({
            url: '/EnterpriseOrder/GetDeclStatus',
            params: { CUSNO: cusno },
            success: function (response, success, option) {
                var res = Ext.decode(response.responseText);
                if (res.success) {
                    Ext.MessageBox.alert('提示', '当前数据状态已经为预录，不可修改！');
                    return;
                } else {
                    importexcel(action, cusno);
                }
            }
        });
    }    
}

function importexcel(action,cusno) {
    var radio_module = Ext.create('Ext.form.RadioGroup', {
        name: "RADIO_MODULE", id: "RADIO_MODULE", fieldLabel: '模板类型', labelAlign: 'right', anchor: '90%', margin: '15 5 15 5',
        items: [
            { boxLabel: "<a href='/FileUpload/PreData_One.xlsx'><b>模板一</b></a>", name: 'RADIO_MODULE', inputValue: '1', checked: true },
            { boxLabel: "<a href='/FileUpload/PreData_Two.xlsx'><b>模板二</b></a>", name: 'RADIO_MODULE', inputValue: '2' }
        ]
    });

    var uploadfile = Ext.create('Ext.form.field.File', {
        id: 'UPLOADFILE', name: 'UPLOADFILE', fieldLabel: '导入数据', labelAlign: 'right', msgTarget: 'under'
        , margin: '15 5 15 5', anchor: '90%', buttonText: '浏览文件', regex: /.*(.xls|.xlsx)$/, regexText: '只能上传xls,xlsx文件'
        , allowBlank: false, blankText: '文件不能为空!'
    });

    //单证服务单位
    var store_dzfwdw = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_dzfwdw
    });

    var field_DOCSERVICENAME = Ext.create('Ext.form.field.Hidden', {
        name: 'DOCSERVICENAME', value: store_dzfwdw.findRecord('CODE', 'GWYKS').get("NAME")
    });

    var combo_dzfwdw = Ext.create('Ext.form.field.ComboBox', {
        id: "DOCSERVICECODE", name: 'DOCSERVICECODE', margin: '15 5 15 5', anchor: '90%', labelAlign: 'right',
        store: store_dzfwdw,
        fieldLabel: '单证服务单位',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        value: 'GWYKS',
        queryMode: 'local',
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.expand();
                    cb.store.clearFilter();
                }
            },
            select: function (records) {
                field_DOCSERVICENAME.setValue(records.rawValue);
            }
        }
    });

    var formpanel_upload = Ext.create('Ext.form.Panel', {
        id: 'formpanel_upload', height: 170,
        buttonAlign: 'center',
        items: [radio_module, combo_dzfwdw, uploadfile
            , field_DOCSERVICENAME
        ],
        buttons: [{
            text: '确认上传',
            handler: function () {
                if (Ext.getCmp('formpanel_upload').getForm().isValid()) {

                    var formdata = Ext.encode(Ext.getCmp('formpanel_upload').getForm().getValues());

                    Ext.getCmp('formpanel_upload').getForm().submit({
                        url: '/EnterpriseOrder/ImportExcelData',
                        params: { formdata: formdata, action: action, cusno: cusno },
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
        height: 205,
        modal: true,
        items: [Ext.getCmp('formpanel_upload')]
    });
    win_upload.show();
}

function deletedata() {
    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要删除的记录！');
        return;
    }

    var ids = ""; var bf = false;
    Ext.each(recs, function (rec) {
        if (rec.get("FLAG") != "0") { bf = true; }
        ids += rec.get("ID") + ",";
    });
    if (bf) {
        Ext.MessageBox.alert('提示', '只可以删除未转化的数据！');
        return;
    }
    ids = ids.substr(0, ids.length - 1);

    Ext.MessageBox.confirm("提示", "确定要删除所选择的记录吗？", function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/EnterpriseOrder/DeletePreData',
                params: { ids: ids},
                success: function (response, success, option) {
                    var res = Ext.decode(response.responseText);
                    if (res.success) {
                        Ext.MessageBox.alert('提示', '删除成功！', function () {
                            pgbar.moveFirst();
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