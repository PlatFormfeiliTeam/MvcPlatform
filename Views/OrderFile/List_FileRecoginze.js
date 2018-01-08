//var common_data_jydw = [];
var store_status; var store_busitype;
var status_data = [{ "NAME": "未关联", "CODE": "未关联" }, { "NAME": "关联中", "CODE": "关联中" }, { "NAME": "已关联", "CODE": "已关联" }, { "NAME": "关联失败", "CODE": "关联失败" }];
var uploader;

var rowIndex, currentPage, pageSize, max_page;
var store_size, store_Trade;

Ext.onReady(function () {
    store_status = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: status_data });
    store_busitype = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_busitype });

    initSearch();
    gridbind();
    upload_ini();

});
/*
Ext.onReady(function () {
    Ext.Ajax.request({
        url: "/Common/Ini_Base_Data",
        params: { ParaType: 'filerecoginze' },
        success: function (response, opts) {
            var commondata = Ext.decode(response.responseText);
            common_data_jydw = commondata.jydw;//经营单位

            store_status = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: status_data });
            store_busitype = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: common_data_busitype });

            initSearch();
            gridbind();
            upload_ini();
        }
    });
});
*/

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
        { header: '订单编号', dataIndex: 'ORDERCODE', width: 90, renderer: renderpre },
        { header: '客户编号', dataIndex: 'CUSNO', width: 110},
        { header: '文件名称', dataIndex: 'FILENAME', width: 180, renderer: renderpre },
        { header: '创建人', dataIndex: 'USERNAME', width: 130 },
        { header: '创建时间', dataIndex: 'TIMES', width: 120 }
        ],
        listeners:
        {
            'itemdblclick': function (view, record, item, index, e) {
                openwin();
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
        case "ORDERCODE":
            if (value) {// color:red;text-decoration:underline;
                rtn = "<div style='color:red;cursor:pointer;' onclick='Filedetail(\"" + record.get("ORDERCODE") + "\",\"" + record.get("CUSNO") + "\")'>" + value + "</div>";
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
        if (rec.get("STATUS") == "已关联" || rec.get("STATUS") == "关联中") { bf = true; }
        ids += rec.get("ID") + ",";
    });
    if (bf) {
        Ext.Msg.alert("提示", "已关联或关联中的文件不能删除!");
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

function Filedetail(ordercode, cusno) {

    if (cusno == "null") { cusno = ""; }

    var file_store = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'FILENAME', 'ORIGINALNAME', 'FILETYPE', 'FILETYPENAME', 'UPLOADTIME', 'SIZES', 'IETYPE']
    })
    var tmp = new Ext.XTemplate(
         '<tpl for=".">',
        '<div class="panel panel-default thumb-wrap fl" style="margin-top:5px;margin-left:5px;width:240px">',
        '<div class="panel-heading" style="padding-left:5px;padding-right:5px">{[values.ORIGINALNAME.substr(0,23)]}<div class="fr"><span class="glyphicon glyphicon-paperclip"></span></div></div>',
        '<div class="panel-body" style="padding-left:5px;">{FILETYPENAME}|',
        '<tpl>{[values.SIZES/1024 > 1024?Math.round(values.SIZES/(1024*1024))+"M":Math.round(values.SIZES/1024)+"K"]}</tpl>',
        '|{[values.UPLOADTIME]}</div></div>',
        '</tpl>'
        )
    var fileview = Ext.create('Ext.view.View', {
        id: 'w_fileview',
        store: file_store,
        tpl: tmp,
        itemSelector: 'div.thumb-wrap',
        multiSelect: true
    })
    var panel = Ext.create('Ext.panel.Panel', {
        border: 0, tbar: topbar,
        minHeight: 100,
        items: [fileview]
    });

    var field_CUSNO_fd = Ext.create('Ext.form.field.Text', {
        id: 'CUSNO_fd', name: 'CUSNO_fd', fieldLabel: '客户编号', labelWidth: 80, labelAlign: 'right', readOnly: true, value: cusno, margin: 0
    });

    var field_ORDERCODE_fd = Ext.create('Ext.form.field.Text', {
        id: 'ORDERCODE_fd', name: 'ORDERCODE_fd', fieldLabel: '订单编号', labelWidth: 80, labelAlign: 'right', readOnly: true, value: ordercode, margin: 0
    });

    var bbar_r = '<div class="btn-group">'
               + '<button type="button" onclick="browsefile_fd()" class="btn btn-primary btn-sm"><i class="fa fa-exchange fa-fw"></i>&nbsp;浏览文件</button>'
               + '</div>';

    var topbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [field_ORDERCODE_fd, field_CUSNO_fd, '->', bbar_r]
    });

    var win_file = Ext.create("Ext.window.Window", {
        id: "win_file",
        title: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;随附文件</span></h4>',
        tbar: topbar,
        width: 1000,
        height: 500,
        modal: true,
        items: [panel]
    });

    win_file.show();

    Ext.Ajax.request({
        url: "/OrderFile/filedetail",
        params: { ordercode: ordercode},
        success: function (response, opts) {
            var data = Ext.decode(response.responseText);
            file_store.loadData(data.filedata);
        }
    });
}

//浏览文件
function browsefile_fd() {
    var records = Ext.getCmp('w_fileview').getSelectionModel().getSelection();
    if (records.length == 0) {
        Ext.MessageBox.alert("提示", "请选择要浏览的记录！");
        return
    }
    var win = Ext.create("Ext.window.Window", {
        title: "文件预览",
        width: 800,
        height: 600,
        layout: "fit",
        modal: true,
        closeAction: "destroy",
        items: [{
            html: "<div id='fileViewDiv' style='height: 100%;width: 100%;'></div>"
        }]
    });

    win.show();
    document.getElementById('fileViewDiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="' + url + '/file/' + records[0].get("FILENAME") + '"></embed>';
}

function Export() {
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据导出中，请稍等..." });
    myMask.show();

    var data = {
        OnlySelf: Ext.get('OnlySelfi').el.dom.className,
        CUSNO: Ext.getCmp('field_CUSNO').getValue(), ORDERCODE: Ext.getCmp('field_ORDERCODE').getValue(),
        STATUS: Ext.getCmp("s_combo_status").getValue(),
        STARTDATE: Ext.Date.format(Ext.getCmp("start_date").getValue(), 'Y-m-d H:i:s'),
        ENDDATE: Ext.Date.format(Ext.getCmp("end_date").getValue(), 'Y-m-d H:i:s')
    }

    Ext.Ajax.request({
        url: '/OrderFile/ExportFileRecoginze',
        method: 'POST',
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
                    form: 'exportform',
                    success: function (response, option) {
                    }
                });
            }
            myMask.hide();
        }
    });
}

//----------------------------------------------------------------------------------------关联文件------------------------------------------------------------------------------------
function openwin() {
    var recs = Ext.getCmp("gridpanel").getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要关联的文件！');
        return;
    }

    var id = recs[0].data.ID;

    var field_ID_win = Ext.create('Ext.form.field.Hidden', { id: 'ID_win', name: 'ID_win' });

    var field_CUSNO_win = Ext.create('Ext.form.field.Text', {
        id: 'CUSNO_win', name: 'CUSNO_win', fieldLabel: '客户编号', labelWidth: 80, labelAlign: 'right', margin: 0
    });

    var field_ORDERCODE_win = Ext.create('Ext.form.field.Text', {
        id: 'ORDERCODE_win', name: 'ORDERCODE_win', fieldLabel: '订单编号', labelWidth: 80, labelAlign: 'right', margin: 0
    });

    var field_STATUS_win = Ext.create('Ext.form.field.Text', {
        id: 'STATUS_win', name: 'STATUS_win', fieldLabel: '关联状态', labelWidth: 80, labelAlign: 'right', margin: 0,
        readOnly: true, fieldStyle: 'color:blue',width:160
    });

    var bbar_l = '<div class="btn-group">'
                + '<button type="button" onclick="save()" id="btn_save" class="btn btn-primary btn-sm"><i class="fa fa-floppy-o"></i>&nbsp;关联</button>'
                + '</div>';
    var bbar_r = '<div class="btn-group"><a href="javascript:reload(2);">上一条</a>&nbsp;&nbsp;<a href="javascript:reload(1);">下一条</a></div>';

    var topbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [field_ID_win, field_CUSNO_win, field_ORDERCODE_win
            , { id: 'btn_search', xtype: 'button', text: '<span class="glyphicon glyphicon-search"></span>', handler: function () { searchinfor(); } }
            , field_STATUS_win
            , bbar_l, '->', '<div id="divcount" style="color:blue;"></div>', '->', bbar_r]
    });

    var html = '<div id="pdfdiv" style="width:100%;height:100%"></div>';
    var panel = Ext.create('Ext.panel.Panel', {
        region: 'center',
        html: html,
        id: 'panel_file'
    });

    var win = Ext.create("Ext.window.Window", {
        id: "win_d",
        title: '文件关联',
        tbar: topbar,
        width: 1000,
        height: 650,
        modal: true,
        items: [panel]
    });

    rowIndex = Ext.getCmp('gridpanel').store.indexOf(recs[0]);
    currentPage = Ext.getCmp('gridpanel').store.currentPage;

    pageSize = parseInt(Ext.getCmp('pgbar').store.pageSize);
    store_size = parseInt(Ext.getCmp('pgbar').store.data.length);

   
    var model_fields = ['ID', 'CUSNO', 'ORDERCODE', 'FILEPATH', 'FILENAME', 'USERID', 'USERNAME', 'TIMES', 'STATUS', 'CUSTOMERNAME', 'CUSTOMERCODE'];
    Ext.define('FileReList', { extend: 'Ext.data.Model', fields: model_fields });

    store_Trade = Ext.create('Ext.data.JsonStore', { model: 'FileReList', pageSize: pageSize });
    store_Trade.proxy = Ext.getCmp('pgbar').store.getProxy();
    store_Trade.proxy.extraParams.start = pageSize * (parseInt(currentPage) - 1);;
    store_Trade.load(function (records, operation, success) {
        max_page = (store_Trade.totalCount % pageSize) == 0 ? parseInt(store_Trade.totalCount / pageSize) : parseInt(store_Trade.totalCount / pageSize) + 1;
    });
    win.show();

    setHeight();
    Ext.getCmp("win_d").on('resize', function () { setHeight(); });

    var recCurrent = Ext.getCmp('pgbar').store.findRecord('ID', id);
    loadform_data(recCurrent);
}

function loadform_data(rec) {
    Ext.getCmp('panel_file').hide();

    Ext.getCmp('ID_win').setValue(rec.data.ID);
    Ext.getCmp('ORDERCODE_win').setValue(rec.data.ORDERCODE);
    Ext.getCmp('CUSNO_win').setValue(rec.data.CUSNO);
    Ext.getCmp('STATUS_win').setValue(rec.data.STATUS);
    if (rec.data.STATUS == "已关联") {
        document.getElementById('pdfdiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="' + url + '/file/' + rec.data.FILEPATH + '"></embed>';
    } else {
        document.getElementById('pdfdiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="' + rec.data.FILEPATH + '"></embed>';
    }
    document.getElementById("btn_save").disabled = (rec.data.STATUS == "关联中" || rec.data.STATUS == "已关联");
    document.getElementById('divcount').innerHTML = '当前页：' + currentPage + '，第 ' + (rowIndex + 1) + ' 笔';

    Ext.getCmp('panel_file').show();
}

function setHeight() {
    var V_height = Ext.getCmp('win_d').getSize().height;
    Ext.get('pdfdiv').setHeight(V_height - 70);
}

function reload(action) {
    Ext.getCmp('panel_file').hide();

    if (action == "1") {
        rowIndex = eval(parseInt(rowIndex) + 1);
        if (rowIndex >= store_size) {
            rowIndex = 0;
            currentPage = parseInt(currentPage) + 1;
            startrow = pageSize * (parseInt(currentPage) - 1);

            if (currentPage == max_page + 1) {
                currentPage = currentPage - 1;
                rowIndex = store_size - 1;
                Ext.MessageBox.alert('提示', '当前为尾页！', function () {
                    Ext.getCmp('panel_file').show();
                });
                return;
            }
            store_Trade.proxy.extraParams.start = startrow;
            store_Trade.load(function (records, operation, success) {
                var recs = records[rowIndex];
                loadform_data(recs);
                store_size = records.length;
            });
        }
        else {
            if (currentPage == max_page + 1) {
                //currentPage = currentPage - 1;
                rowIndex = store_size;
                Ext.MessageBox.alert('提示', '当前为尾页！', function () {
                    Ext.getCmp('panel_file').show();
                });
            }
            else {
                loadform_data(store_Trade.data.items[rowIndex]);
            }
        }
    } else {

        rowIndex = eval(parseInt(rowIndex) - 1);
        if (rowIndex < 0) {
            rowIndex = pageSize - 1;
            currentPage = parseInt(currentPage) - 1;
            startrow = pageSize * (parseInt(currentPage) - 1) < 0 ? 0 : pageSize * (parseInt(currentPage) - 1);


            if (currentPage == 0) {
                currentPage = 1
                Ext.MessageBox.alert('提示', '当前为首页！', function () {
                    Ext.getCmp('panel_file').show();
                });
                rowIndex = 0;
                return;
            }
            store_Trade.proxy.extraParams.start = startrow;
            store_Trade.load(function (records, operation, success) {
                var recs = records[rowIndex];
                loadform_data(recs);
                store_size = records.length;
            });
        }
        else {
            if (currentPage == 0) {
                currentPage = 1
                Ext.MessageBox.alert('提示', '当前为首页！', function () {
                    Ext.getCmp('panel_file').show();
                });
            }
            else {
                loadform_data(store_Trade.data.items[rowIndex]);
            }

        }
    }
}

function searchinfor() {
   /*
    //经营单位
    var store_jydw = Ext.create('Ext.data.JsonStore', {  
        fields: ['CODE', 'NAME'],
        data: common_data_jydw
    });

    //经营单位
    var field_busiunit_s = Ext.create('Ext.form.field.ComboBox', {
        id: 'field_busiunit_s',
        name: 'BUSIUNIT',
        fieldLabel: '经营单位',
        store: store_jydw,
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
        minChars: 4,
        hideTrigger: true,
        anyMatch: true,
        forceSelection: true, labelWidth: 80,width:350, labelAlign: 'right', margin: 0
    });
    */

    var field_busiunit_s = Ext.create('Ext.form.field.Text', {
        id: 'field_busiunit_s', name: 'BUSIUNIT', fieldLabel: '经营单位', labelWidth: 80, width: 250, labelAlign: 'right', margin: 0
    });

    //业务类型
    var combo_busitype_s = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_busitype_s',
        name: 'BUSITYPE',
        store: store_busitype,
        fieldLabel: '业务类型',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        labelWidth: 80, labelAlign: 'right', margin: 0,//hideTrigger: true, 
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

    var tbar_order = Ext.create('Ext.toolbar.Toolbar', {
        items: [field_busiunit_s, combo_busitype_s,
            { text: '<i class="fa fa-search"></i>&nbsp;查询', handler: function () { pgbar_order.moveFirst(); } }
        ]
    })
    var store_order = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'CODE', 'CUSNO', 'BUSITYPE', 'BUSIUNITCODE','BUSIUNITNAME'],
        proxy: {
            url: '/OrderFile/selectorder',
            type: 'ajax',
            reader: {
                type: 'json',
                root: 'rows',
                totalProperty: 'total'
            }
        },
        pageSize: 20,
        autoLoad: true,
        listeners: {
            beforeload: function () {
                store_order.getProxy().extraParams = {
                    BUSIUNIT: Ext.getCmp('field_busiunit_s').getValue(),
                    BUSITYPE: Ext.getCmp('combo_busitype_s').getValue()
                }
            }
        }
    })
    var pgbar_order = Ext.create('Ext.toolbar.Paging', {
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_order,
        displayInfo: true
    })

    var grid_order = Ext.create('Ext.grid.Panel', {
        store: store_order,
        selModel: { selType: 'checkboxmodel' },
        region: 'center',
        tbar: tbar_order,
        bbar: pgbar_order,
        columns: [
            { xtype: 'rownumberer', width: 35 },
            { header: 'ID', dataIndex: 'ID', hidden: true },
            { header: '订单编号', dataIndex: 'CODE', width: 100 },
            { header: '企业编号', dataIndex: 'CUSNO', width: 120 },
            {
                header: '业务类型', dataIndex: 'BUSITYPE', width: 90, renderer: function render(value, cellmeta, record, rowIndex, columnIndex, store) {
                    var rec = store_busitype.findRecord('CODE', value);
                    if (rec) { return rec.get("NAME"); }
                    return "";
                }
            },
            { header: '经营单位', dataIndex: 'BUSIUNITCODE', width: 110 },
            { header: '经营单位', dataIndex: 'BUSIUNITNAME', flex:1 }            
        ],
        listeners: {
            itemdblclick: function (gd, record, item, index, e, eOpts) {
                Ext.getCmp("CUSNO_win").setValue(record.get("CUSNO"));
                Ext.getCmp("ORDERCODE_win").setValue(record.get("CODE"));
                win_order.close();
            }
        },
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });
    var win_order = Ext.create("Ext.window.Window", {
        title: '关联订单业务编号选择',
        width: 700,
        height: 570,
        modal: true,
        items: [grid_order],
        layout: 'border',
        buttonAlign: 'center',
        buttons: [{
            text: '<i class="fa fa-check-square-o"></i>&nbsp;确定', handler: function () {
                var recs = grid_order.getSelectionModel().getSelection();
                if (recs.length > 0) {
                    Ext.getCmp("CUSNO_win").setValue(recs[0].get("CUSNO"));
                    Ext.getCmp("ORDERCODE_win").setValue(recs[0].get("CODE"));
                    win_order.close();
                }
            }
        }, {
            text: '<i class="fa fa-times"></i>&nbsp;取消', handler: function () {
                win_order.close();
            }
        }],
        listeners: {
            "show": function () {
                Ext.getCmp('panel_file').hide();
            },
            "close": function () {
                Ext.getCmp('panel_file').show();
            }
        }
    });
    win_order.show();
}

function save() {
    var id = Ext.getCmp('ID_win').getValue();
    var cusno = $.trim(Ext.getCmp('CUSNO_win').getValue());
    var ordercode = $.trim(Ext.getCmp('ORDERCODE_win').getValue());

    if (cusno == "" && ordercode == "") {
        Ext.Msg.alert("提示", "关联文件时，客户编号、订单编号不能都为空!");
        return;
    }
    Ext.getCmp('panel_file').hide();
    var myMask = new Ext.LoadMask(Ext.getCmp("win_d"), { msg: "关联中，请稍等..." });
    myMask.show();

    Ext.Ajax.request({
        url: '/OrderFile/SaveRecoginze',
        params: { id: id, cusno: cusno, ordercode: ordercode },
        success: function (response, success, option) {
            var data = Ext.decode(response.responseText);
            myMask.hide();

            var msg="";
            if (data.success) { msg = "关联成功"; }
            else {
                if (data.flag == "Y") { msg = "该文件已经关联"; }
                else if (data.flag == "E") { msg = "订单不存在"; }
                else { msg = "关联失败"; }
            }

            Ext.MessageBox.alert("提示", msg, function () {
                //刷新前一页Ext.getCmp('pgbar').store.load 用Ext.getCmp("pgbar").moveFirst();的话，没法写load完成后的获取新数据
                Ext.getCmp('pgbar').store.load(function (records, operation, success) {
                    var recCurrent = Ext.getCmp('pgbar').store.findRecord('ID', id);
                    loadform_data(recCurrent);//刷新当前界面
                    store_Trade.load();//刷新当前上一条下一条的store
                });
            });

        }
    });
}