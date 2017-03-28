//-----------------------------------------------------------------------------search------------------------------------------------------------------------
function initSearch() {
    //账册号
    var store_recordid = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_recordid
    });
    var s_combo_recordid = Ext.create('Ext.form.field.ComboBox', {
        id: 's_combo_recordid',
        editable: false,
        store: store_recordid,
        fieldLabel: '账册号',
        labelWidth: 50,
        width: 220,
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
        labelWidth: 50,
        name: 'ITEMNO',
        flex: .2
    });
    //HS编码
    var field_HSCODE = Ext.create('Ext.form.field.Text', {
        id: 'field_HSCODE',
        fieldLabel: 'HS编码',
        labelWidth: 50,
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
        labelWidth: 60,
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
        labelWidth: 60,
        displayField: 'NAME',
        name: 'STATUS',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local'
    });
    //非法码比对
    var chk_error = Ext.create('Ext.form.field.Checkbox', {
        id: 'chk_error',
        boxLabel: '非法码比对',
        name: 'error',
        minWidth: 80
    });
    var formpanel = Ext.create('Ext.form.Panel', {
        id: 'formpanel',
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5'
        },
        items: [
        { layout: 'column', border: 0, margin: '5 0 0 0', items: [s_combo_recordid, field_ITEMNO, field_HSCODE, s_combo_optionstatus, s_combo_status, chk_error] }
        ]
    });
}

function itemsbind() {
    Ext.regModel('RecrodDetail', {
        fields: ['ID', 'OPTIONS', 'STATUS', 'RECORDINFOID', 'CODE', 'ITEMNO', 'HSCODE', 'ADDITIONALNO', 'ITEMNOATTRIBUTE', 'COMMODITYNAME'
            , 'SPECIFICATIONSMODEL', 'UNIT', 'CUSTOMERCODE', 'CUSTOMERNAME', 'REMARK']
    });

    var store_RecrodDetail_lj = Ext.create('Ext.data.JsonStore', {
        model: 'RecrodDetail',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/RecordInfor/loadRecordDetail',
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
                    RECORDINFORID: Ext.getCmp('s_combo_recordid').getValue(), ITEMNO: Ext.getCmp("field_ITEMNO").getValue(), HSCODE: Ext.getCmp('field_HSCODE').getValue(),
                    OPTIONS: Ext.getCmp('s_combo_optionstatus').getValue(), STATUS: Ext.getCmp("s_combo_status").getValue(),
                    ERROR: Ext.getCmp('chk_error').getValue() == true ? "1" : "0"
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
        //{ header: '变动状态', dataIndex: 'OPTIONS', width: 110 },
        //{ header: '申请状态', dataIndex: 'STATUS', width: 110 },
        { header: '账册号', dataIndex: 'CODE', width: 130 },
        { header: '项号', dataIndex: 'ITEMNO', width: 80 },
        { header: 'HS编码', dataIndex: 'HSCODE', width: 130 },
        { header: '附加码', dataIndex: 'ADDITIONALNO', width: 80 },
        { header: '项号属性', dataIndex: 'ITEMNOATTRIBUTE', width: 80 },
        { header: '商品名称', dataIndex: 'COMMODITYNAME', width: 200 },
        { header: '规格型号', dataIndex: 'SPECIFICATIONSMODEL', width: 200 },
        { header: '成交单位', dataIndex: 'UNIT', width: 80, renderer: renderOrder },
        //{ header: '报关行', dataIndex: 'CUSTOMERNAME', width: 150 },
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
            url: '/RecordInfor/loadRecordDetail',
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
                    RECORDINFORID: Ext.getCmp('s_combo_recordid').getValue(), ITEMNO: Ext.getCmp("field_ITEMNO").getValue(), HSCODE: Ext.getCmp('field_HSCODE').getValue(),
                    OPTIONS: Ext.getCmp('s_combo_optionstatus').getValue(), STATUS: Ext.getCmp("s_combo_status").getValue(),
                    ERROR: Ext.getCmp('chk_error').getValue() == true ? "1" : "0"
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
        //{ header: '变动状态', dataIndex: 'OPTIONS', width: 110 },
        //{ header: '申请状态', dataIndex: 'STATUS', width: 110 },
        { header: '账册号', dataIndex: 'CODE', width: 130 },
        { header: '项号', dataIndex: 'ITEMNO', width: 80 },
        { header: 'HS编码', dataIndex: 'HSCODE', width: 130 },
        { header: '附加码', dataIndex: 'ADDITIONALNO', width: 80 },
        { header: '项号属性', dataIndex: 'ITEMNOATTRIBUTE', width: 80 },
        { header: '商品名称', dataIndex: 'COMMODITYNAME', width: 200 },
        { header: '规格型号', dataIndex: 'SPECIFICATIONSMODEL', width: 200 },
        { header: '成交单位', dataIndex: 'UNIT', width: 80, renderer: renderOrder },
        //{ header: '报关行', dataIndex: 'CUSTOMERNAME', width: 150 },
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
            {
                text: '<span class="icon iconfont" style="font-size:12px;color:#3071A9;">&#xe632;</span>&nbsp;<font color="#3071A9">修 改</font>',
                handler: function () { edit_task('gridpanel_lj_Go'); }
            },
               {
                   text: '<span class="icon iconfont" style="font-size:12px;color:#3071A9;">&#xe6d3;</span>&nbsp;<font color="#3071A9">删 除</font>',
                   handler: function () { delete_task('gridpanel_lj_Go', 'pgbar_lj_Go'); }
               },
               {
                   text: '<span class="icon iconfont" style="font-size:12px;color:#3071A9;">&#xe601;</span>&nbsp;<font color="#3071A9">申请表打印</font>',
                   handler: function () { print_task('gridpanel_lj_Go'); }
               }]

    });
    var store_RecrodDetail_lj_Go = Ext.create('Ext.data.JsonStore', {
        model: 'RecrodDetail',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/RecordInfor/loadRecordDetail_Go',
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
                    RECORDINFORID: Ext.getCmp('s_combo_recordid').getValue(), ITEMNO: Ext.getCmp("field_ITEMNO").getValue(), HSCODE: Ext.getCmp('field_HSCODE').getValue(),
                    OPTIONS: Ext.getCmp('s_combo_optionstatus').getValue(), STATUS: Ext.getCmp("s_combo_status").getValue(),
                    ERROR: Ext.getCmp('chk_error').getValue() == true ? "1" : "0"
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
        { header: '报关行', dataIndex: 'CUSTOMERNAME', width: 250 },
        { header: '备注', dataIndex: 'REMARK', width: 150 }
        ],
        listeners:
        {
            'itemdblclick': function (view, record, item, index, e) {
                edit_task('gridpanel_lj_Go');
            }
        },
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });


    var tbar_cp_Go = Ext.create('Ext.toolbar.Toolbar', {
        items: [
            {
                text: '<span class="icon iconfont" style="font-size:12px;color:#3071A9;">&#xe632;</span>&nbsp;<font color="#3071A9">修 改</font>',
                handler: function () { edit_task('gridpanel_cp_Go'); }
            },
               {
                   text: '<span class="icon iconfont" style="font-size:12px;color:#3071A9;">&#xe6d3;</span>&nbsp;<font color="#3071A9">删 除</font>',
                   handler: function () { delete_task('gridpanel_cp_Go', 'pgbar_cp_Go'); }
               },
               {
                   text: '<span class="icon iconfont" style="font-size:12px;color:#3071A9;">&#xe601;</span>&nbsp;<font color="#3071A9">申请表打印</font>',
                   handler: function () { print_task('gridpanel_cp_Go'); }
               }]

    });

    var store_RecrodDetail_cp_Go = Ext.create('Ext.data.JsonStore', {
        model: 'RecrodDetail',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/RecordInfor/loadRecordDetail_Go',
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
                    RECORDINFORID: Ext.getCmp('s_combo_recordid').getValue(), ITEMNO: Ext.getCmp("field_ITEMNO").getValue(), HSCODE: Ext.getCmp('field_HSCODE').getValue(),
                    OPTIONS: Ext.getCmp('s_combo_optionstatus').getValue(), STATUS: Ext.getCmp("s_combo_status").getValue(),
                    ERROR: Ext.getCmp('chk_error').getValue() == true ? "1" : "0"
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
        { header: '报关行', dataIndex: 'CUSTOMERNAME', width: 250 },
        { header: '备注', dataIndex: 'REMARK', width: 150 }
        ],
        listeners:
        {
            'itemdblclick': function (view, record, item, index, e) {
                edit_task('gridpanel_cp_Go');
            }
        },
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

function Open(type) {
    if (type == 'A') {//新增申请
        opencenterwin("/RecordInfor/Create", 1600, 900);
    } else {
        var Compentid = "";
        if (Ext.getCmp("tabpanel").getActiveTab().id == "tab_0") { Compentid = "gridpanel_lj"; }
        if (Ext.getCmp("tabpanel").getActiveTab().id == "tab_1") { Compentid = "gridpanel_cp"; }

        var recs = Ext.getCmp(Compentid).getSelectionModel().getSelection();
        if (recs.length != 1) {
            Ext.Msg.alert("提示", "请选择一笔记录!");
            return;
        }

        Ext.Ajax.request({
            url: '/RecordInfor/Repeat_Task',
            params: { id: recs[0].get("ID") },
            success: function (response, success, option) {
                var res = Ext.decode(response.responseText);

                if (res.success) {//判断是否存在正在申请的记录
                    Ext.MessageBox.alert('提示', "此项号正在申请中！");
                }
                else {
                    if (type == 'U') {//变更申请                   
                        opencenterwin("/RecordInfor/Change?rid=" + recs[0].get("ID"), 1600, 900);
                    }
                    if (type == 'D') {//删除申请
                        opencenterwin("/RecordInfor/Delete?rid=" + recs[0].get("ID"), 1600, 900);
                    }
                }

            }
        });        
    }    
}

function edit_task(Compentid) {
    var recs = Ext.getCmp(Compentid).getSelectionModel().getSelection();
    if (recs.length != 1) {
        Ext.Msg.alert("提示", "请选择一笔修改记录!");
        return;
    }

    if (recs[0].get("OPTIONS") == "A") {
        opencenterwin("/RecordInfor/Create?id=" + recs[0].get("ID"), 1600, 900);
    }
    if (recs[0].get("OPTIONS") == "U") {
        opencenterwin("/RecordInfor/Change?id=" + recs[0].get("ID"), 1600, 900);
    }
    if (recs[0].get("OPTIONS") == "D") {
        opencenterwin("/RecordInfor/Delete?id=" + recs[0].get("ID"), 1600, 900);
    }
}

function delete_task(Compentid, Compentid_pgbar) {
    var recs = Ext.getCmp(Compentid).getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.Msg.alert("提示", "请选择删除记录!");
        return;
    }
    var ids = ""; var bf = false;
    Ext.each(recs, function (rec) {
        if (rec.get("STATUS") != "0") { bf = true; }
        ids += rec.get("ID") + ",";
    });
    if (bf) {
        Ext.Msg.alert("提示", "只能删除状态为草稿的记录!");
        return;
    }
    ids = ids.substr(0, ids.length - 1);

    Ext.MessageBox.confirm("提示", "确定要删除所选择的记录吗？", function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/RecordInfor/Delete_Task',
                params: { ids: ids },
                success: function (response, success, option) {
                    var res = Ext.decode(response.responseText);
                    var msgs = "";
                    if (res.success) { msgs = "删除成功！"; }
                    else { msgs = "删除失败！"; }

                    Ext.MessageBox.alert('提示', msgs, function (btn) {
                        Ext.getCmp(Compentid_pgbar).moveFirst();
                    });
                }
            });
        }
    });
}

function print_task(Compentid) {
    var recs = Ext.getCmp(Compentid).getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.Msg.alert("提示", "请选择打印记录!");
        return;
    }
    var ids = ""; var bf = false;
    Ext.each(recs, function (rec) {
        if (rec.get("STATUS") == "0") { bf = true; }
        ids += rec.get("ID") + ",";
    });
    if (bf) {
        Ext.Msg.alert("提示", "状态为草稿 不可以打印，请重新选择需要打印的记录!");
        return;
    }
    ids = ids.substr(0, ids.length - 1);
    printitemno(ids);
    
}

function Export() {
    var recordid = Ext.getCmp('s_combo_recordid').getValue(); recordid = recordid == null ? "" : recordid;
    var itemno = Ext.getCmp('field_ITEMNO').getValue(); var hscode = Ext.getCmp('field_HSCODE').getValue();
    var options = Ext.getCmp('s_combo_optionstatus').getValue(); options = options == null ? "" : options;
    var status = Ext.getCmp('s_combo_status').getValue(); status = status == null ? "" : status;
    var error = Ext.getCmp('chk_error').getValue() == true ? "1" : "0";

    $('#e_options').val(JSON.stringify(optionstatus_js_data));
    $('#e_status').val(JSON.stringify(status_js_data));
    $('#e_unit').val(JSON.stringify(common_data_unit));

    var path = '/RecordInfor/Export?RECORDINFORID=' + recordid + '&ITEMNO=' + itemno + '&HSCODE=' + hscode + '&OPTIONS=' + options + '&STATUS=' + status + '&ERROR=' + error;
    $('#exportform').attr("action", path).submit();
}

//-----------------------------------------------------------------------------create or change-----------------------------------------------------------
function cancel() {
    var status = Ext.getCmp('combo_STATUS').getValue();
    var msg = "";
    if (status > 10) { msg = "当前状态为已受理，确认要执行撤回操作吗？"; }
    else { msg = "确定要执行撤单操作吗？"; }

    Ext.MessageBox.confirm("提示", msg, function (btn) {
        if (btn == "yes") {
            Ext.Ajax.request({
                url: '/RecordInfor/cancel',
                params: { id: id },
                success: function (response, options) {
                    var json = Ext.decode(response.responseText);
                    if (json.success == true) {
                        Ext.MessageBox.alert("提示", "撤单成功！");
                    }
                    else {
                        Ext.MessageBox.alert("提示", "撤单失败,订单状态已发生变化！");
                    }
                    loadform_record();
                }
            });
        }
    });

}

function Element_ini() {//61034200、52115100、85011099、74101100、41041111  

    if (Ext.getCmp('panel_ele_2')) {
        Ext.getCmp('panel_ele_2').destroy();
    }

    var label_busiinfo = {
        columnWidth: 1,
        xtype: 'label',
        margin: '0 0 5 5',
        html: '<div style="border-bottom-width: 2px; border-bottom-color: gray; border-bottom-style: dashed;padding-left:20px;"><h5><b>申报要素</b></h5></div>'
    }

    var label_busiinfo_end = {
        columnWidth: 1,
        xtype: 'label',
        margin: '0 0 5 5',
        html: '<div style="border-bottom-width: 2px; border-bottom-color: gray; border-bottom-style: dashed;"></div>'
    }

    var customarea = Ext.getCmp('combo_CUSTOMAREA').getValue();
    var hscode = Ext.getCmp('HSCODE').getValue();
    var additionalno = Ext.getCmp('ADDITIONALNO').getValue();

    var configItem = new Array();
    
    Ext.Ajax.request({
        url: "/RecordInfor/GetElements",
        params: { customarea: customarea, hscode: hscode, additionalno: additionalno, id: id },
        success: function (response, opts) {
            var json = Ext.decode(response.responseText);

            if (json.elements.length > 0) {
                configItem.push({ layout: 'column', height: 42, margin: '0 0 15 0', border: 0, items: [label_busiinfo] });

                var items_i; var id_pinming = "";
                for (var i = 0; i < json.elements.length; i = i + 4) {

                    //----------------------------------------------------------------------------------------------------------------------------------
                    if (id_pinming=="" && json.elements[i].ELEMENTS.indexOf(':') > 0) {
                        if (json.elements[i].ELEMENTS.substr(json.elements[i].ELEMENTS.indexOf(':') + 1) == "品名") { id_pinming = "field_ele_" + (i); }
                    }
                    if (id_pinming == "" && json.elements[i + 1].ELEMENTS.indexOf(':') > 0) {
                        if (json.elements[i + 1].ELEMENTS.substr(json.elements[i + 1].ELEMENTS.indexOf(':') + 1) == "品名") { id_pinming = "field_ele_" + (i + 1); }
                    }
                    if (id_pinming == "" && json.elements[i + 2].ELEMENTS.indexOf(':') > 0) {
                        if (json.elements[i + 2].ELEMENTS.substr(json.elements[i + 2].ELEMENTS.indexOf(':') + 1) == "品名") { id_pinming = "field_ele_" + (i + 2); }
                    }
                    if (id_pinming == "" && json.elements[i + 3].ELEMENTS.indexOf(':') > 0) {
                        if (json.elements[i + 3].ELEMENTS.substr(json.elements[i + 3].ELEMENTS.indexOf(':') + 1) == "品名") { id_pinming = "field_ele_" + (i + 3); }
                    }
                    //----------------------------------------------------------------------------------------------------------------------------

                    items_i = [];
                    items_i.push(Ext.create('Ext.form.Label', { id: 'label_ele_' + (i), name: 'label_ele_' + (i), text: json.elements[i].ELEMENTS, cls: "lab" }));
                    items_i.push(Ext.create('Ext.form.field.Text', { id: 'field_ele_' + (i), name: 'field_ele_' + (i), value: json.elements[i].DESCRIPTIONS, readOnly: Ext.getCmp('combo_STATUS').getValue() >= 10 }));//, fieldLabel: json.elements[i].ELEMENTS

                    if ((i + 1) < json.elements.length) {
                        items_i.push(Ext.create('Ext.form.Label', { id: 'label_ele_' + (i + 1), name: 'label_ele_' + (i + 1), text: json.elements[i + 1].ELEMENTS, cls: "lab" }));
                        items_i.push(Ext.create('Ext.form.field.Text', { id: 'field_ele_' + (i + 1), name: 'field_ele_' + (i + 1), value: json.elements[i + 1].DESCRIPTIONS, readOnly: Ext.getCmp('combo_STATUS').getValue() >= 10 }));//, fieldLabel: json.elements[i + 1].ELEMENTS
                    }
                    if ((i + 2) < json.elements.length) {
                        items_i.push(Ext.create('Ext.form.Label', { id: 'label_ele_' + (i + 2), name: 'label_ele_' + (i + 2), text: json.elements[i + 2].ELEMENTS, cls: "lab" }));
                        items_i.push(Ext.create('Ext.form.field.Text', { id: 'field_ele_' + (i + 2), name: 'field_ele_' + (i + 2), value: json.elements[i + 2].DESCRIPTIONS, readOnly: Ext.getCmp('combo_STATUS').getValue() >= 10 }));//, fieldLabel: json.elements[i + 2].ELEMENTS
                    }
                    if ((i + 3) < json.elements.length) {
                        items_i.push(Ext.create('Ext.form.Label', { id: 'label_ele_' + (i + 3), name: 'label_ele_' + (i + 3), text: json.elements[i + 3].ELEMENTS, cls: "lab" }));
                        items_i.push(Ext.create('Ext.form.field.Text', { id: 'field_ele_' + (i + 3), name: 'field_ele_' + (i + 3), value: json.elements[i + 3].DESCRIPTIONS, readOnly: Ext.getCmp('combo_STATUS').getValue() >= 10 }));//, fieldLabel: json.elements[i + 3].ELEMENTS
                    }

                    configItem.push({ layout: 'column', margin: '0 0 0 50', border: 0, items: items_i });
                }

                configItem.push({ layout: 'column', height: 42, margin: '0 0 0 0', border: 0, items: [label_busiinfo_end] }, Ext.create('Ext.form.field.Hidden', { id: 'jsonEle', name: 'jsonEle' })
                    , Ext.create('Ext.form.field.Hidden', { id: 'id_pinming', name: 'id_pinming', value: id_pinming })
                    );
                Ext.getCmp('jsonEle').setValue(response.responseText);//json.elements
                if (Ext.getCmp(id_pinming)) {
                    Ext.getCmp(id_pinming).setValue(Ext.getCmp('COMMODITYNAME').getValue());
                    Ext.getCmp(id_pinming).setReadOnly(true);
                }

                Ext.getCmp('panel_ele').add(Ext.create('Ext.panel.Panel', { id: 'panel_ele_2', columnWidth: 1, border: 0, items: configItem }));
            }
        }
    });
}

function form_ini_con() {
    var rownum = -1;//记录当前编辑的行号

    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;成品单耗信息</span></h4>'
    }
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo, '->']
    })


    //对应料件序号
    var store_ITEMNO_CONSUME = Ext.create('Ext.data.JsonStore', {  //报关行combostore
        storeId: 'myStore',
        fields: ['ITEMNO', 'COMMODITYNAME', 'SPECIFICATIONSMODEL', 'UNIT', 'UNITNAME']
    })

    var combo_ITEMNO_CONSUME = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ITEMNO_CONSUME',
        name: 'ITEMNO_CONSUME',
        store: store_ITEMNO_CONSUME,
        displayField: 'ITEMNO',
        valueField: 'ITEMNO',
        queryMode: 'local',
        margin: 0,
        minChars: 1,
        forceSelection: true,
        anyMatch: true,
        hideTrigger: true,
        listeners: {
            focus: function (cb) {
                cb.clearInvalid();
            },
            select: function (combo, records) {
                combo_ITEMNO_CONSUME.setValue(records[0].data.ITEMNO);
                field_ITEMNO_COMMODITYNAME.setValue(records[0].data.COMMODITYNAME);
                field_ITEMNO_SPECIFICATIONSMODEL.setValue(records[0].data.SPECIFICATIONSMODEL);
                field_ITEMNO_UNIT.setValue(records[0].data.UNIT);
                field_ITEMNO_UNITNAME.setValue(records[0].data.UNITNAME)
            }
        },
        flex: .85,
        allowBlank: false,
        blankText: '对应料件序号不能为空!'
    })

    //var field_ITEMNO_CONSUME = Ext.create('Ext.form.field.Text', {
    //    id: 'ITEMNO_CONSUME',
    //    name: 'ITEMNO_CONSUME',
    //    flex: .85, margin: 0,
    //    allowBlank: false,
    //    blankText: '对应料件序号不能为空!'
    //});

    var field_ITEMNO_LJ = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        fieldLabel: '对应料件序号',
        items: [combo_ITEMNO_CONSUME,
            {
                id: 'ITEMNO_CONSUME_btn', xtype: 'button', handler: function () {
                    selectitemno(Ext.getCmp('combo_RECORDINFOID').getValue(), combo_ITEMNO_CONSUME, field_ITEMNO_COMMODITYNAME, field_ITEMNO_SPECIFICATIONSMODEL, field_ITEMNO_UNITNAME, field_ITEMNO_UNIT);
                },
                text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
            }
        ]
    }

    //对应料件名称
    var field_ITEMNO_COMMODITYNAME = Ext.create('Ext.form.field.Text', {
        id: 'ITEMNO_COMMODITYNAME',
        name: 'ITEMNO_COMMODITYNAME',
        fieldLabel: '对应名称', readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;',
        allowBlank: false,
        blankText: '对应名称不能为空!'
    });

    //对应料件规格
    var field_ITEMNO_SPECIFICATIONSMODEL = Ext.create('Ext.form.field.Text', {
        id: 'ITEMNO_SPECIFICATIONSMODEL',
        name: 'ITEMNO_SPECIFICATIONSMODEL', readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;',
        fieldLabel: '对应规格'
    });

    //对应料件计量单位
    var field_ITEMNO_UNITNAME = Ext.create('Ext.form.field.Text', {
        id: 'ITEMNO_UNITNAME',
        name: 'ITEMNO_UNITNAME',
        fieldLabel: '对应计量单位', readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;',
        allowBlank: false,
        blankText: '对应计量单位不能为空!'
    });

    var field_ITEMNO_UNIT = Ext.create('Ext.form.field.Hidden', {
        name: 'ITEMNO_UNIT'
    })

    //单耗
    var field_CONSUME = Ext.create('Ext.form.field.Number', {
        id: 'CONSUME',
        name: 'CONSUME',
        fieldLabel: '单耗',
        allowBlank: false, hideTrigger: true,
        blankText: '单耗不能为空!'
    });

    //损耗率
    var field_ATTRITIONRATE = Ext.create('Ext.form.field.Text', {
        id: 'ATTRITIONRATE',
        name: 'ATTRITIONRATE',
        fieldLabel: '损耗率',
        allowBlank: false,
        blankText: '损耗率不能为空!'
    });

    var formpanel_con = Ext.create('Ext.form.Panel', {
        id: 'formpanel_con',
        renderTo: 'div_form_con',
        minHeight: 100,
        border: 0,
        tbar: tbar,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: .25,
            labelAlign: 'right',
            labelSeparator: '',
            msgTarget: 'under',
            validateOnBlur: false,
            validateOnChange: false
        },
        items: [
            { layout: 'column', height: 42, border: 0, margin: '5 0 0 0', items: [field_ITEMNO_LJ, field_ITEMNO_COMMODITYNAME, field_ITEMNO_SPECIFICATIONSMODEL, field_ITEMNO_UNITNAME] },
            { layout: 'column', height: 42, border: 0, items: [field_CONSUME, field_ATTRITIONRATE] },
            field_ITEMNO_UNIT
        ]
    });
    //=================================================
    var data_PRODUCTCONSUME = [];
    var store_PRODUCTCONSUME = Ext.create('Ext.data.JsonStore', {
        storeId: 'store_PRODUCTCONSUME',
        fields: ['ITEMNO_CONSUME', 'ITEMNO_COMMODITYNAME', 'ITEMNO_SPECIFICATIONSMODEL', 'ITEMNO_UNITNAME', 'ITEMNO_UNIT', 'CONSUME', 'ATTRITIONRATE'],
        data: data_PRODUCTCONSUME
    });
    
    var w_tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: ['<span style="color:red">说明：双击列表项可对已添加的记录进行修改</span>',
                {
                    text: '<span style="color:blue">新增模式</span>', id: 'btn_mode', handler: function () {
                        if (Ext.getCmp('btn_mode').getText() == '<span style="color:blue">编辑模式</span>') {
                            Ext.getCmp('btn_mode').setText('<span style="color:blue">新增模式</span>');
                            rownum = -1;
                        }
                    }
                },
               '->',
               {
                   text: '<span class="icon iconfont" style="font-size:10px">&#xe622;</span>&nbsp;保 存', id:'btn_pro_save',
                   handler: function () {
                       if (!formpanel_con.getForm().isValid()) {
                           return;
                       }

                       var formdata = formpanel_con.getForm().getValues();
                       if (rownum < 0) {//添加模式
                           store_PRODUCTCONSUME.insert(store_PRODUCTCONSUME.data.length, formdata);
                       }
                       else {//修改模式
                           var rec = store_PRODUCTCONSUME.getAt(rownum);
                           store_PRODUCTCONSUME.remove(rec);
                           store_PRODUCTCONSUME.insert(rownum, formdata);
                       }
                       formpanel_con.getForm().reset();
                       rownum = -1;
                       Ext.getCmp('btn_mode').setText('<span style="color:blue">新增模式</span>');
                   }
               },
               {
                   text: '<span class="icon iconfont" style="font-size:10px">&#xe6d3;</span>&nbsp;删 除', id: 'btn_pro_del',
                   handler: function () {
                       var recs = gridpanel_PRODUCTCONSUME.getSelectionModel().getSelection();
                       if (recs.length > 0) {
                           store_PRODUCTCONSUME.remove(recs);
                       }
                       Ext.getCmp('btn_mode').setText('<span style="color:blue">新增模式</span>');
                       rownum = -1;
                   }
               }]

    });
    var gridpanel_PRODUCTCONSUME = Ext.create('Ext.grid.Panel', {
        id: 'gridpanel_PRODUCTCONSUME',
        renderTo: 'div_form_con',
        store: store_PRODUCTCONSUME,
        minHeight: 150,
        selModel: { selType: 'checkboxmodel' },
        enableColumnHide: false,
        tbar: w_tbar,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: 'ID', dataIndex: 'ID', hidden: true },
        { header: '对应料件序号', dataIndex: 'ITEMNO_CONSUME', width: 80 },
        { header: '对应料件名称', dataIndex: 'ITEMNO_COMMODITYNAME', width: 130 },
        { header: '对应料件规格', dataIndex: 'ITEMNO_SPECIFICATIONSMODEL', width: 130 },
        { header: '对应料件计量单位', dataIndex: 'ITEMNO_UNITNAME', width: 80 },
        { header: '单耗', dataIndex: 'CONSUME', width: 80 },
        { header: '损耗率', dataIndex: 'ATTRITIONRATE', width: 80 }
        ],
        listeners: {
            itemdblclick: function (w_grid, record, item, index, e, eOpts) {
                rownum = index;
                formpanel_con.getForm().setValues(record.data);
                Ext.getCmp('btn_mode').setText('<span style="color:blue">编辑模式</span>');
            }
        },
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });

}

function printitemno(ids) {
    opencenterwin("/RecordInfor/PrintRecordDetail?ids=" + ids, 1600, 900);
}

function SetItemno_consume(recordid) {
    if (Ext.getCmp('formpanel_con')) {

        //清空成品对应信息
        Ext.each(Ext.getCmp('formpanel_con').getForm().getFields().items, function (field) {
            field.reset();
        });

        Ext.getCmp('gridpanel_PRODUCTCONSUME').store.removeAll();

        Ext.Ajax.request({
            url: "/RecordInfor/Ini_Base_Data_Itemno_Consume",
            params: { RECORDINFOID: recordid },
            success: function (response, opts) {
                var commondata = Ext.decode(response.responseText);
                Ext.getCmp('combo_ITEMNO_CONSUME').store.loadData(commondata.itemno_consume);//对应料件序号
            }
        });
    }
}
