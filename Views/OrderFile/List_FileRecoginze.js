var store_status;
var status_data = [{ "NAME": "未关联", "CODE": "未关联" }, { "NAME": "关联中", "CODE": "关联中" }, { "NAME": "已关联", "CODE": "已关联" }, { "NAME": "关联失败", "CODE": "关联失败" }];
var uploader;

Ext.onReady(function () {
    store_status = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: status_data });
    initSearch();
    gridbind();
    upload_ini();
});

function initSearch() {

    //企业编号
    var field_CUSNO = Ext.create('Ext.form.field.Text', {
        id: 'field_CUSNO',
        fieldLabel: '企业编号',
        name: 'CUSNO'
    });
    //订单编号
    var field_ORDERCODE = Ext.create('Ext.form.field.Text', {
        id: 'field_ORDERCODE',
        fieldLabel: '订单编号',
        name: 'ORDERCODE'
    });
    var s_combo_status = Ext.create('Ext.form.field.ComboBox', {
        id: 's_combo_status',
        editable: false,
        store: store_status,
        fieldLabel: '关联状态',
        displayField: 'NAME',
        name: 'FLAG',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local'
    });
    var start_date = Ext.create('Ext.form.field.Date', {
        id: 'start_date',
        name: 'start_date',
        format: 'Y-m-d',
        emptyText: '开始日期', flex: .5, margin: 0
    });
    var end_date = Ext.create('Ext.form.field.Date', {
        id: 'end_date',
        name: 'end_date',
        format: 'Y-m-d',
        emptyText: '结束日期', flex: .5, margin: 0
    });
    //创建时间
    var condate = Ext.create('Ext.form.FieldContainer', {
        fieldLabel: '创建时间',
        layout: 'hbox',
        //columnWidth: .25,
        items: [start_date, end_date]
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
        { layout: 'column', border: 0, margin: '5 0 0 0', items: [field_CUSNO, field_ORDERCODE, s_combo_status, condate] }
        ]
    });
}

function gridbind() {
    Ext.regModel('LISTFILE', {
        fields: ['ID', 'CUSNO', 'ORDERCODE', 'FILEPATH', 'FILENAME', 'USERID', 'USERNAME', 'TIMES', 'STATUS', 'CUSTOMERNAME', 'CUSTOMERCODE']
    });

    var store_filerecoginze = Ext.create('Ext.data.JsonStore', {
        model: 'LISTFILE',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/OrderFile/loadFileRecoginze',
            reader: {
                root: 'rows',
                type: 'json',
                totalProperty: 'total'
            }
        },
        autoLoad: true,
        listeners: {
            beforeload: function () {
                store_filerecoginze.getProxy().extraParams = {
                    OnlySelf: Ext.get('OnlySelfi').el.dom.className,
                    CUSNO: Ext.getCmp('field_CUSNO').getValue(), ORDERCODE: Ext.getCmp('field_ORDERCODE').getValue(),
                    STATUS: Ext.getCmp("s_combo_status").getValue(),
                    STARTDATE: Ext.Date.format(Ext.getCmp("start_date").getValue(), 'Y-m-d H:i:s'),
                    ENDDATE: Ext.Date.format(Ext.getCmp("end_date").getValue(), 'Y-m-d H:i:s')
                }
            }
        }
    });

    pgbar = Ext.create('Ext.toolbar.Paging', {
        id: 'pgbar',
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_filerecoginze,
        displayInfo: true
    })
    var gridpanel = Ext.create('Ext.grid.Panel', {
        id: "gridpanel",
        renderTo: "appConId",
        store: store_filerecoginze,
        height: 500,
        selModel: { selType: 'checkboxmodel' },
        bbar: pgbar,
        enableColumnHide: false,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: 'ID', dataIndex: 'ID', hidden: true },
        { header: '关联状态', dataIndex: 'STATUS', width: 70, renderer: renderpre},
        { header: '订单编号', dataIndex: 'ORDERCODE', width: 90 },
        { header: '客户编号', dataIndex: 'CUSNO', width: 110},
        { header: '文件名称', dataIndex: 'FILENAME', width: 180, renderer: renderpre },
        { header: '创建人', dataIndex: 'USERNAME', width: 130 },
        { header: '创建时间', dataIndex: 'TIMES', width: 120 }
        ],
        listeners:
        {
            'itemdblclick': function (view, record, item, index, e) {
                // opencenterwin("/OrderFile/List_FileRecoginzeDetail?ORDERCODE=" + record.get("ORDERCODE"), 1200, 600);
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
            var rec = store_status.findRecord('CODE', value);
            if (rec) {
                rtn = rec.get("NAME");
            }
            break;
        case "FILENAME":
            if (record.get("STATUS")=="已关联") {//服务器
                rtn = "<a href=\"/OrderFile/DownFile?filename=" + record.get("FILEPATH") + "&isupload=1\">" + value + "<\a>";
            } else {//本机
                rtn = "<a href=\"/OrderFile/DownFile?filename=" + record.get("FILEPATH") + "&isupload=\">" + value + "<\a>";
            }           
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
    Ext.getCmp("pgbar").moveFirst();
}

function changeOnlySelfStyle() {
    var OnlySelfDom = Ext.get('OnlySelfi');
    if (Ext.get('OnlySelfi').el.dom.className.replace(/(^\s*)|(\s*$)/g, "") == "fa fa-check-square-o") {
        OnlySelfDom.removeCls("fa fa-check-square-o")
        OnlySelfDom.addCls("fa fa-square-o");
    }
    else {
        OnlySelfDom.removeCls("fa fa-square-o")
        OnlySelfDom.addCls("fa fa-check-square-o");
    }
}

function upload_ini() {
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "正在上传，请稍等..." });

    uploader = new plupload.Uploader({
        runtimes: 'html5,flash,silverlight,html4',
        browse_button: 'upfiles', // you can pass an id...
        url: '/OrderFile/UploadFile',
        flash_swf_url: '/js/upload/Moxie.swf',
        silverlight_xap_url: '/js/upload/Moxie.xap',
        unique_names: true,
        filters: {
            max_file_size: '5000mb',
            mime_types: [
                { title: "Pdf files", extensions: "pdf" }
            ]
        }
    });
    uploader.init();
    uploader.bind('FilesAdded', function (up, files) {
        myMask.show();
        uploader.start();
    });
    uploader.bind('FileUploaded', function (up, file) {
    });
    uploader.bind('UploadComplete', function (up, files) {
        Ext.getCmp("pgbar").moveFirst();
        myMask.hide();
    });
}

function del() {
    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.Msg.alert("提示", "请选择删除记录!");
        return;
    }
    var ids = ""; var bf = false;
    Ext.each(recs, function (rec) {
        if (rec.get("STATUS") == "已关联") { bf = true; }
        ids += rec.get("ID") + ",";
    });
    if (bf) {
        Ext.Msg.alert("提示", "不能删除已关联的文件!");
        return;
    }
    ids = ids.substr(0, ids.length - 1);

    Ext.MessageBox.confirm("提示", "确定要删除所选择的记录吗？", function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/OrderFile/DeleteRecoginze',
                params: { ids: ids },
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

function openwin() {
    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要关联的文件！');
        return;
    }
    var data = "["; //var bf = false;
    for (var i = 0; i < recs.length; i++) {
        data += "{ID:'" + recs[i].get("ID") + "',FILEPATH:'" + recs[i].get("FILEPATH") + "'}";
        if (i != recs.length - 1) { data += ","; }

        //if (recs[i].get("STATUS") == "已关联") { bf = true;}
    }
    data += "']";

    //if (bf) {
    //    Ext.MessageBox.alert('提示', '已关联文件 不能 再次做关联！');
    //    return;
    //}

    var field_CUSNO = Ext.create('Ext.form.field.Text', {
        id: 'CUSNO', name: 'CUSNO', fieldLabel: '客户编号', labelWidth: 80, labelAlign: 'right', margin: 0
    });

    var field_ORDERCODE = Ext.create('Ext.form.field.Text', {
        id: 'ORDERCODE', name: 'ORDERCODE', fieldLabel: '订单编号', labelWidth: 80, labelAlign: 'right', margin: 0
    });

    var bbar_l = '<div class="btn-group">'
              + '<button type="button" onclick="save()" class="btn btn-primary btn-sm" id="btn_save"><i class="fa fa-floppy-o"></i>&nbsp;保存</button>'
           + '</div>';
    var bbar_r = '<div class="btn-group"><a href="javascript:reload(2);">上一条</a>&nbsp;&nbsp;<a href="javascript:reload(1);">下一条</a></div>';

    var topbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [field_CUSNO, field_ORDERCODE, bbar_l, '->', bbar_r]
    });

    var win = Ext.create("Ext.window.Window", {
        id: "win_d",
        title: '文件关联',
        tbar: topbar,
        width: 1000,
        height: 650,
        modal: true,
        items: [Ext.getCmp('form_panel'), {
            html: "<div id='pdfDiv' style='height: 100%;width: 100%;'></div>"
        }]
    });
    win.show();
    //document.getElementById('pdfPrintDiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="' + response.responseText + '"></embed>';
}

