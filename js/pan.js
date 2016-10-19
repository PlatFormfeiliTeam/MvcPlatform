
//无数据则返回空字符串
function isNull(value) {
    if (value == null || value == "null" || value == undefined) {
        return '';
    } else {
        return value;
    }
}
function opencenterwin(url, width, height) {
    var iWidth = width ? width : "1000", iHeight = height ? height : "600";
    var iTop = (window.screen.availHeight - 30 - iHeight) / 2; //获得窗口的垂直位置;
    var iLeft = (window.screen.availWidth - 10 - iWidth) / 2; //获得窗口的水平位置; 
    window.open(url, '', 'height=' + iHeight + ',,innerHeight=' + iHeight + ',width=' + iWidth + ',innerWidth=' + iWidth + ',top=' + iTop + ',left=' + iLeft + ',toolbar=yes,menubar=yes, location=yes,scrollbars=yes,resizable=yes');
}
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}



//ENTER实现TAB功能
var maximumformpaneltabIndexi = 1;
var isreturn = false;
function getNextFocus(nextItem, nextIndex) {
    maximumformpaneltabIndexi++;
    if (nextItem == undefined) {
        if (maximumformpaneltabIndexi > 100) {
            (Ext.ComponentQuery.query("component[tabIndex=1]")[0]).focus(false, 200);
        } else {
            var nextIndex2 = (Number(nextIndex) + Number(1));
            var nextItem2 = Ext.ComponentQuery.query("component[tabIndex=" + nextIndex2 + "]")[0];
            getNextFocus(nextItem2, nextIndex2);
        }
    } else {
        maximumformpaneltabIndexi = nextItem.tabIndex;
        if (nextItem.disabled == true || nextItem.readOnly == true) {
            var nextIndex2 = (Number(nextIndex) + Number(1));
            var nextItem2 = Ext.ComponentQuery.query("component[tabIndex=" + nextIndex2 + "]")[0];
            getNextFocus(nextItem2, nextIndex2);
        } else {
            nextItem.focus(false, 200);
        }
    }
}
document.onkeydown = function () {
    if (event.keyCode == 13) {
        var currIndex = document.activeElement.tabIndex; // 当前获取焦点的对象
        if (isNull(currIndex) == '') {
            return;
        }
        var currItems = Ext.ComponentQuery.query("component[tabIndex=" + currIndex + "]");
        if (currItems != undefined) {
            currItem = currItems[0];
            if (currItem != undefined) {
                if (isNull(currItem['value']) == '' || currItem.isValid() == false) {
                    currItem.reset();
                }
            }
        }
        var nextIndex = (Number(currIndex) + Number(1));
        var nextItem = Ext.ComponentQuery.query("component[tabIndex=" + nextIndex + "]")[0];
        getNextFocus(nextItem, nextIndex);
    }
}

//删除文件
function removeFile() {
    var records = Ext.getCmp('w_fileview').getSelectionModel().getSelection();
    if (records.length == 0) {
        Ext.MessageBox.alert("提示", "请选择要删除的记录！");
        return
    }
    Ext.MessageBox.confirm('提示', '确定要删除选择的记录吗？', function (btn) {
        if (btn == 'yes') {
            Ext.getCmp('w_fileview').store.remove(records);
        }
    })
}
//浏览文件
function browsefile() {
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
    if (!records[0].get("ID")) {
        win.show();
        document.getElementById('fileViewDiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="/FileUpload/file/' + records[0].get("ORIGINALNAME") + '"></embed>';
    }
    else {
        win.show();
        document.getElementById('fileViewDiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="' + common_data_adminurl + '\/file/' + records[0].get("FILENAME") + '"></embed>';
    }
}

//显示报关申报单位列表选择窗体 modal显示
function bgsbdw_win(component) {//传入需要赋值的控件
    var tb_bgsbdw = Ext.create('Ext.toolbar.Toolbar', {
        items: [
            { xtype: 'textfield', id: 'bgsbdw_Name_s', fieldLabel: '单位名称', labelAlign: 'right' },
            { xtype: 'textfield', id: 'bgsbdw_Code_s', fieldLabel: '海关编码', labelAlign: 'right' },
            {
                text: '<i class="fa fa-search"></i>&nbsp;查询', handler: function () {
                    pb_bgsbdw.moveFirst();
                }
            }
        ]
    })
    var store_bgsbdw = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'CODE', 'NAME', 'INSPCODE', 'SOCIALCREDITNO'],
        proxy: {
            url: '/Common/selectbgsbdw',
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
                store_bgsbdw.getProxy().extraParams = {
                    Name: Ext.getCmp('bgsbdw_Name_s').getValue(),
                    Code: Ext.getCmp('bgsbdw_Code_s').getValue()
                }
            }
        }
    })
    var pb_bgsbdw = Ext.create('Ext.toolbar.Paging', {
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_bgsbdw,
        displayInfo: true
    })

    var grid_bgsbdw = Ext.create('Ext.grid.Panel', {
        store: store_bgsbdw,
        selModel: { selType: 'checkboxmodel' },
        region: 'center',
        tbar: tb_bgsbdw,
        bbar: pb_bgsbdw,
        columns: [
    { xtype: 'rownumberer', width: 35 },
    { header: 'ID', dataIndex: 'ID', hidden: true },
    { header: '单位名称', dataIndex: 'NAME', flex: 1 },
    { header: '海关代码', dataIndex: 'CODE', width: 120 },
    { header: '报检代码', dataIndex: 'INSPCODE', width: 120 },
    { header: '社会信用', dataIndex: 'SOCIALCREDITNO', width: 100 }],
        listeners: {
            itemdblclick: function (gd, record, item, index, e, eOpts) {
                component.setValue(record.get("NAME") + "(" + record.get("CODE") + ")");
                win.close();
            }
        }
    });
    var win = Ext.create("Ext.window.Window", {
        title: '报关申报单位选择',
        width: 700,
        height: 570,
        modal: true,
        items: [grid_bgsbdw],
        layout: 'border',
        buttonAlign: 'center',
        buttons: [{
            text: '<i class="fa fa-check-square-o"></i>&nbsp;确定', handler: function () {
                var recs = grid_bgsbdw.getSelectionModel().getSelection();
                if (recs.length > 0) {
                    component.setValue(recs[0].get("NAME") + "(" + recs[0].get("CODE") + ")");
                    win.close();
                }
            }
        }, {
            text: '<i class="fa fa-times"></i>&nbsp;取消', handler: function () {
                win.close();
            }
        }]
    });
    win.show();
}
function bjsbdw_win(component) {//传入需要赋值的控件
    var tb_bjsbdw = Ext.create('Ext.toolbar.Toolbar', {
        items: [
            { xtype: 'textfield', id: 'bjsbdw_Name_s', fieldLabel: '单位名称', labelAlign: 'right' },
            { xtype: 'textfield', id: 'bjsbdw_Code_s', fieldLabel: '报检编码', labelAlign: 'right' },
            {
                text: '<i class="fa fa-search"></i>&nbsp;查询', handler: function () {
                    pb_bjsbdw.moveFirst();
                }
            }
        ]
    })
    var store_bjsbdw = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'CODE', 'NAME', 'INSPCODE', 'SOCIALCREDITNO'],
        proxy: {
            url: '/Common/selectbjsbdw',
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
                store_bjsbdw.getProxy().extraParams = {
                    Name: Ext.getCmp('bjsbdw_Name_s').getValue(),
                    INSPCODE: Ext.getCmp('bjsbdw_Code_s').getValue()
                }
            }
        }
    })
    var pb_bjsbdw = Ext.create('Ext.toolbar.Paging', {
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_bjsbdw,
        displayInfo: true
    })

    var grid_bjsbdw = Ext.create('Ext.grid.Panel', {
        store: store_bjsbdw,
        selModel: { selType: 'checkboxmodel' },
        region: 'center',
        tbar: tb_bjsbdw,
        bbar: pb_bjsbdw,
        columns: [
    { xtype: 'rownumberer', width: 35 },
    { header: 'ID', dataIndex: 'ID', hidden: true },
    { header: '单位名称', dataIndex: 'NAME', flex: 1 },
    { header: '海关代码', dataIndex: 'CODE', width: 120 },
    { header: '报检代码', dataIndex: 'INSPCODE', width: 120 },
    { header: '社会信用', dataIndex: 'SOCIALCREDITNO', width: 100 }],
        listeners: {
            itemdblclick: function (gd, record, item, index, e, eOpts) {
                component.setValue(record.get("NAME") + "(" + record.get("INSPCODE") + ")");
                win.close();
            }
        }
    });
    var win = Ext.create("Ext.window.Window", {
        title: '报检申报单位选择',
        width: 700,
        height: 570,
        modal: true,
        items: [grid_bjsbdw],
        layout: 'border',
        buttonAlign: 'center',
        buttons: [{
            text: '<i class="fa fa-check-square-o"></i>&nbsp;确定', handler: function () {
                var recs = grid_bjsbdw.getSelectionModel().getSelection();
                if (recs.length > 0) {
                    component.setValue(recs[0].get("NAME") + "(" + recs[0].get("INSPCODE") + ")");
                    win.close();
                }
            }
        }, {
            text: '<i class="fa fa-times"></i>&nbsp;取消', handler: function () {
                win.close();
            }
        }]
    });
    win.show();
}

//创建报关车号
function createUptBgch(pb_bgsbdw) {

    var bgchname = Ext.create('Ext.form.field.Text', {
        margin: "10 0 10 0",
        id: 'bgchname',
        fieldLabel: '报关车号',
        labelAlign: 'right',
        allowBlank: false,
        blankText: '不能为空!',
        msgTarget: 'under'
    });

    var bkhname = Ext.create('Ext.form.field.Text', {
        margin: "10 0 10 0",
        id: 'bkhname',
        fieldLabel: '白卡号',
        labelAlign: 'right',
        allowBlank: false,
        blankText: '不能为空!',
        msgTarget: 'under'
    });

    var store_cdcode = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        //data: declarationjydw_js_condition_data
        proxy: {
            type: 'ajax',
            url: '/Common/cdbh',
            reader: { type: 'json', root: 'rows' }
        },
        autoLoad: true
    });
    var bgchWin = Ext.create("Ext.window.Window", {
        id: "bgchWin",
        title: "报关车号维护",
        width: 300,
        height: 170,
        modal: true,
        buttonAlign: 'center',
        items: [
            bgchname, bkhname
        ],
        buttons: [{
            text: '<i class="fa fa-check-square-o"></i>&nbsp;确定', handler: function () {
                if (Ext.getCmp('bgchname').isValid() == false || Ext.getCmp('bkhname').isValid() == false) {
                    return;
                }
                Ext.Ajax.request({
                    url: "/Common/updatebgch",
                    //params: { 'bgchname': Ext.getCmp('bgchname').getValue().replace(/(^\s*)|(\s*$)/g, ""), 'bkhname': Ext.getCmp('bkhname').getValue().replace(/(^\s*)|(\s*$)/g, ""), 'cdcode': Ext.getCmp('cdcode').getValue().replace(/(^\s*)|(\s*$)/g, "") },
                    params: { 'bgchname': Ext.getCmp('bgchname').getValue().replace(/(^\s*)|(\s*$)/g, ""), 'bkhname': Ext.getCmp('bkhname').getValue().replace(/(^\s*)|(\s*$)/g, "") },
                    success: function (option, success, response) {
                        var data = Ext.decode(option.responseText);
                        if (data.success == true) {
                            pb_bgsbdw.moveFirst();
                            bgchWin.close();
                        } else if (data.success == "bgch") {
                            Ext.MessageBox.alert("提示", "报关车号重复！");
                        } else if (data.success == "bkh") {
                            Ext.MessageBox.alert("提示", "白卡号重复！");
                        }
                        else {
                            Ext.MessageBox.alert("提示", "添加失败！");
                        }
                    }
                });
            }
        }, {
            text: '<i class="fa fa-times"></i>&nbsp;取消', handler: function () {
                bgchWin.close();
            }
        }]
    });
    bgchWin.show();
};

//显示报关申报单位列表选择窗体 modal显示
function bgch_win(store_bgch_com, combo_bgch_com, common) {//传入需要赋值的控件
    var tb_bgsbdw = Ext.create('Ext.toolbar.Toolbar', {
        items: [
            { xtype: 'textfield', id: 'bgch_Name_s', fieldLabel: '报关车号', labelAlign: 'right' },
            { xtype: 'textfield', id: 'bgsbdw_Name_s', fieldLabel: '所属公司名称', labelAlign: 'right' },
            {
                text: '<i class="fa fa-search"></i>&nbsp;查询', handler: function () {
                    pb_bgsbdw.moveFirst();
                }
            },
            {
                xtype: 'button', text: '<span class="glyphicon glyphicon glyphicon-plus"></span>&nbsp;添加', handler: function () {
                    createUptBgch(pb_bgsbdw);
                }
            }
        ]
    });

    var store_bgsbdw = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'LICENSE', 'NAME', 'WHITECARD', 'MOTORCADE'],
        proxy: {
            url: '/Common/selectbgch',
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
                store_bgsbdw.getProxy().extraParams = {
                    NAME: Ext.getCmp('bgsbdw_Name_s').getValue(),
                    LICENSE: Ext.getCmp('bgch_Name_s').getValue()
                }
            }
        }
    })
    var pb_bgsbdw = Ext.create('Ext.toolbar.Paging', {
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_bgsbdw,
        displayInfo: true
    })

    var grid_bgsbdw = Ext.create('Ext.grid.Panel', {
        store: store_bgsbdw,
        selModel: { selType: 'checkboxmodel' },
        region: 'center',
        tbar: tb_bgsbdw,
        bbar: pb_bgsbdw,
        columns: [
            { xtype: 'rownumberer', width: 35 },
            { header: 'ID', dataIndex: 'ID', hidden: true },
            { header: '报关车号', dataIndex: 'LICENSE', width: 150 },
            { header: '白卡号', dataIndex: 'WHITECARD', width: 150 },
            { header: '所属公司名称', dataIndex: 'NAME', flex: 1 },
            { header: '车队编号', dataIndex: 'MOTORCADE', width: 150 }
        ],
        listeners: {
            itemdblclick: function (gd, record, item, index, e, eOpts) {
                if (record.get("MOTORCADE") == null || record.get("MOTORCADE") == undefined || record.get("MOTORCADE") == "") {
                    if (common == 'common') {
                        store_bgch_com.insert(0, { CODE: record.get("WHITECARD"), NAMECODE: record.get("LICENSE") + "(" + record.get("WHITECARD") + ")" });
                        combo_bgch_com.setValue(record.get("WHITECARD"));
                    } else {
                        store_bgch_com.insert(0, { CODE: record.get("LICENSE"), NAME: record.get("LICENSE") + "(" + record.get("WHITECARD") + ")" });
                        combo_bgch_com.setValue(record.get("LICENSE"));
                    }
                    win.close();
                    return;
                }
                Ext.Ajax.request({
                    url: "/Common/updateCybgch",
                    params: { 'DECLARATIONCARID': record.get("ID"), 'MOTORCADE': record.get("MOTORCADE") },
                    success: function (option, success, response) {
                        var data = Ext.decode(option.responseText);
                        if (data.success == true) {
                            store_bgch_com.load();
                            if (common == 'common') {
                                combo_bgch_com.setValue(record.get("WHITECARD"));
                            } else {
                                combo_bgch_com.setValue(record.get("LICENSE"));
                            }
                            win.close();
                        }
                        else {
                            Ext.MessageBox.alert("提示", "添加失败！");
                        }
                    }
                });
            }
        }
    });
    var win = Ext.create("Ext.window.Window", {
        title: '报关车号选择',
        width: 700,
        height: 570,
        modal: true,
        items: [grid_bgsbdw],
        layout: 'border',
        buttonAlign: 'center',
        buttons: [
            {
                text: '<i class="fa fa-check-square-o"></i>&nbsp;确定', handler: function () {
                    var recs = grid_bgsbdw.getSelectionModel().getSelection();
                    if (recs.length > 0) {
                        if (recs[0].data.MOTORCADE == null || recs[0].data.MOTORCADE == undefined || recs[0].data.MOTORCADE == "") {
                            if (common == 'common') {
                                store_bgch_com.insert(0, { CODE: recs[0].data.WHITECARD, NAMECODE: recs[0].data.LICENSE + "(" + recs[0].data.WHITECARD + ")" });
                                combo_bgch_com.setValue(recs[0].data.WHITECARD);
                            } else {
                                store_bgch_com.insert(0, { CODE: recs[0].data.LICENSE, NAME: recs[0].data.LICENSE + "(" + recs[0].data.WHITECARD + ")" });
                                combo_bgch_com.setValue(recs[0].data.LICENSE);
                            }
                            win.close();
                            return;
                        }
                        Ext.Ajax.request({
                            url: "/Common/updateCybgch",
                            params: { 'DECLARATIONCARID': recs[0].data.ID, 'MOTORCADE': recs[0].data.MOTORCADE },
                            success: function (option, success, response) {
                                var data = Ext.decode(option.responseText);
                                if (data.success == true) {
                                    store_bgch_com.load();
                                    if (common == 'common') {
                                        combo_bgch_com.setValue(recs[0].data.WHITECARD);
                                    } else {
                                        combo_bgch_com.setValue(recs[0].data.LICENSE);
                                    }
                                    win.close();
                                }
                                else {
                                    Ext.MessageBox.alert("提示", "添加失败！");
                                }
                            }
                        });
                    }

                }
            },
            {
                text: '<i class="fa fa-times"></i>&nbsp;取消', handler: function () {
                    win.close();
                }
            }

        ]
    });

    win.show();
}
//经营单位选择窗体
function selectjydw(cb_jydw, field_quanname) {//传入需要赋值的控件
    var tb_jydw = Ext.create('Ext.toolbar.Toolbar', {
        items: [
           { xtype: 'textfield', fieldLabel: '经营单位', labelWidth: 100, labelAlign: 'right', id: 'NAME_jydw_s' },
           {
               text: '<i class="fa fa-search"></i>&nbsp;查询', handler: function () {
                   pb_jydw.moveFirst();
               }
           }
        ]
    })
    var store_jydw = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'CODE', 'NAME'],
        proxy: {
            url: '/Common/LoadJydw',
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
                store_jydw.getProxy().extraParams = {
                    NAME: Ext.getCmp('NAME_jydw_s').getValue()
                }
            }
        }
    })
    var pb_jydw = Ext.create('Ext.toolbar.Paging', {
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_jydw,
        displayInfo: true
    })
    var grid_jydw = Ext.create('Ext.grid.Panel', {
        store: store_jydw,
        selModel: { selType: 'checkboxmodel' },
        region: 'center',
        tbar: tb_jydw,
        bbar: pb_jydw,
        columns: [
                    { xtype: 'rownumberer', width: 50 },
                    { header: 'ID', dataIndex: 'ID', hidden: true },
                    { header: '经营单位代码', dataIndex: 'CODE', width: 300 },
                    { header: '经营单位名称', dataIndex: 'NAME', flex: 1 }],
        listeners: {
            itemdblclick: function (gd, record, item, index, e, eOpts) {
                cb_jydw.setValue(record.get("CODE"));
                field_quanname.setValue(record.get("NAME"));
                win_jydw.close();
            }
        }
    });
    var win_jydw = Ext.create("Ext.window.Window", {
        title: '经营单位选择',
        width: 700,
        height: 570,
        modal: true,
        items: [grid_jydw],
        layout: 'border',
        buttonAlign: 'center',
        buttons: [{
            text: '<i class="fa fa-check-square-o"></i>&nbsp;确定', handler: function () {
                var recs = grid_jydw.getSelectionModel().getSelection();
                if (recs.length > 0) {
                    cb_jydw.setValue(recs[0].get("CODE"));
                    field_quanname.setValue(recs[0].get("NAME"));
                    win_jydw.close();
                }
            }
        }, {
            text: '<i class="fa fa-times"></i>&nbsp;取消', handler: function () {
                win_jydw.close();
            }
        }]
    });
    win_jydw.show();
}
//贸易方式选择窗体
function selectmyfs(cb_myfs, tradeway_m) {//传入需要赋值的控件tradeway_m 表示备注信息
    var tb_myfs = Ext.create('Ext.toolbar.Toolbar', {
        items: [
           { xtype: 'textfield', fieldLabel: '贸易方式', labelWidth: 100, labelAlign: 'right', id: 'NAME_myfs_s' },
           {
               text: '<i class="fa fa-search"></i>&nbsp;查询', handler: function () {
                   pb_myfs.moveFirst();
               }
           }
        ]
    })
    var sm_selrecs = new Array();
    var store_myfs = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'CODE', 'NAME'],
        proxy: {
            url: '/Common/LoadMyfs',
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
                store_myfs.getProxy().extraParams = { NAME: Ext.getCmp('NAME_myfs_s').getValue() };
            },
            load: function () {
                var recs = new Array();
                Ext.each(sm_selrecs, function (str) {
                    var rec = store_myfs.findRecord('CODE', str);
                    if (rec) {
                        recs.push(rec);
                    }
                })
                sm.select(recs);
            }
        }
    })
    var pb_myfs = Ext.create('Ext.toolbar.Paging', {
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_myfs,
        displayInfo: true
    })
    var grid_myfs = Ext.create('Ext.grid.Panel', {
        store: store_myfs,
        selModel: { selType: 'checkboxmodel', mode: 'SIMPLE' },
        region: 'center',
        tbar: tb_myfs,
        bbar: pb_myfs,
        columns: [{ xtype: 'rownumberer', width: 50 },
                  { header: 'ID', dataIndex: 'ID', hidden: true },
                  { header: '贸易方式代码', dataIndex: 'CODE', width: 300 },
                  { header: '贸易方式名称', dataIndex: 'NAME', flex: 1 }],
        listeners: {
            itemdblclick: function (gd, record, item, index, e, eOpts) {
                cb_myfs.setValue(record.get("CODE"));
                var remark = tradeway_m.getValue();
                if (remark) {
                    var start = remark.indexOf('贸易方式<@');
                    var end = remark.indexOf('>@');
                    var sub = remark.substring(start, end + 2);
                    tradeway_m.setValue(remark.replace(sub, ''));
                }
                win_myfs.close();
            },
            select: function (gd, record, index, eOpts) {
                if (Ext.Array.indexOf(sm_selrecs, record.get('CODE')) < 0) {
                    sm_selrecs.push(record);
                }
            },
            deselect: function (gd, record, index, eOpts) {
                Ext.Array.remove(sm_selrecs, record);
            }
        }
    });
    var sm = grid_myfs.getSelectionModel();
    var win_myfs = Ext.create("Ext.window.Window", {
        title: '贸易方式选择',
        width: 700,
        height: 570,
        modal: true,
        items: [grid_myfs],
        layout: 'border',
        buttonAlign: 'center',
        buttons: [{
            text: '<i class="fa fa-check-square-o"></i>&nbsp;确定', handler: function () {
                if (sm_selrecs.length > 0) {
                    var codes = "";
                    var remark = tradeway_m.getValue();
                    if (remark) { 
                        var start = remark.indexOf('贸易方式<@');
                        var end = remark.indexOf('>@');
                        var sub = remark.substring(start, end + 2);
                        tradeway_m.setValue(remark.replace(sub, ''));
                    }
                    Ext.each(sm_selrecs, function (rec) {
                        codes += codes ? "|" + rec.get("NAME") : rec.get("NAME");
                    })
                    cb_myfs.setValue(sm_selrecs[0].get("CODE"));
                    //如果贸易方式超过一个,需要将贸易方式写入备注信息  2016-10-18 by panhuaguo
                    if (sm_selrecs.length > 1) {
                        tradeway_m.setValue(tradeway_m.getValue() + '贸易方式<@' + codes + ">@");
                    }
                    win_myfs.close();
                }
                else {
                    Ext.MessageBox.alert('提示', '请选择对应的贸易方式！');
                }
            }
        }, {
            text: '<i class="fa fa-times"></i>&nbsp;取消', handler: function () {
                win_myfs.close();
            }
        }]
    });
    win_myfs.show();
}

function setHistoryUrl(url) {
    try {
        if (!!window.ActiveXObject) {
            window.location.href = url;
        } else {
            var stateObject = {};
            var title = "";
            var newUrl = url;
            history.pushState(stateObject, title, newUrl);
        }
    } catch (err) {
    }
    finally {
    }
}

try {
    window.addEventListener('popstate', function (event) {
        history.go(0);
    });
} catch (err) {
}
finally {
}

//验证该账号是否开通了ERP接口
function validateErpCompetence() {
    var erpCode = 0;
    Ext.Ajax.request({
        url: "/Common/validateErpCompetence",
        async: false,
        success: function (response, option) {
            var data = response.responseText;
            if (data != undefined && data != null && data != '') {
                erpCode = 1;
            }
        }
    });
    return erpCode;
}

//从企业ERP获取订单信息
function LoadOrderFromErp(busitype) {
    if (!Ext.getCmp('NUMBER').getValue()) {
        Ext.MessageBox.alert("提示", "请输入需要导入的订单编号！");
        return;
    }
    //当页面是编辑模式时,需验证输入的客户编号和当前客户编号相同
    if (ordercode && Ext.getCmp('CUSNO').getValue() != Ext.getCmp('NUMBER').getValue()) {
        Ext.MessageBox.alert("提示", "请输入相同的客户编号！");
        return;
    }

    //2016/9/26 add heguiqin：如果订单中存在重复的客户编号需要进行确认
    Ext.Ajax.request({
        url: "/Common/OperateIdRepeate",
        params: { operateid: Ext.String.trim(Ext.getCmp('NUMBER').getValue()) },
        success: function (response, option) {
            var json = Ext.decode(response.responseText);
            if (parseInt(json.result) > 0) {
                Ext.MessageBox.confirm('提示', '该订单编号已经存在，确定要继续导入吗？', function (btn) {
                    if (btn == "yes") {
                        importorderFromErp();
                    }
                })
            }
            else {
                importorderFromErp();
            }
        }
    });
}

function importorderFromErp() {
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "数据查询中,请稍候..." });
    myMask.show();
    Ext.Ajax.request({
        url: "/Common/GetOrderFromErp",
        params: { operationid: Ext.String.trim(Ext.getCmp('NUMBER').getValue()), busitype: busitype },
        success: function (response, success, option) {
            var data = Ext.decode(response.responseText);
            if (data.success) {  //erp上存在该业务编号
                Ext.MessageBox.alert("提示", "数据导入成功！");
                formpanel.getForm().setValues(data.data);
                if (Ext.getCmp('combo_jydw').getValue()) { //导入时如果经营单位有值 就给简称字段赋值
                    var rec = Ext.getCmp('combo_jydw').store.findRecord('CODE', Ext.getCmp('combo_jydw').getValue());
                }
                if (Ext.getCmp('combo_PORTCODE') && Ext.getCmp('combo_PORTCODE').getValue()) {//如果进出口岸有值就给portname赋值
                    var rec = Ext.getCmp('combo_PORTCODE').store.findRecord('CODE', Ext.getCmp('combo_PORTCODE').getValue());
                    Ext.getCmp('field_PORTNAME').setValue(rec.get("NAME"));
                }
                Ext.Ajax.request({
                    url: "/Common/getErpfile",
                    params: { id: Ext.String.trim(Ext.getCmp('NUMBER').getValue()), busitype: busitype },
                    success: function (response, success, option) {
                        var data = Ext.decode(response.responseText);

                        var timestamp = Ext.Date.now();  //1351666679575  这个方法只是获取的时间戳
                        var date = new Date(timestamp);

                        Ext.each(data.rows, function (item) {
                            file_store.insert(file_store.data.length, { FILENAME: '/FileUpload/file/' + item.MAINNAME.substr(item.MAINNAME.lastIndexOf("CustomsFile") + 11), ORIGINALNAME: item.MAINNAME.substr(item.MAINNAME.lastIndexOf("CustomsFile") + 11), SIZES: item.FILE_SIZE, FILETYPENAME: "订单文件", FILETYPE: "44", UPLOADTIME: Ext.Date.format(date, 'Y-m-d H:i:s') });
                        })
                    }
                });
            }
            else {
                Ext.MessageBox.alert("提示", "对应的订单号在ERP上无记录！");
            }
            myMask.hide();
        }
    });
}

//2016-6-2 针对以前追加查询做改进，弹出grid窗体，录入或者粘贴多个分单号进行查询
function seniorsearch(pagebar_p) {
    if (Ext.getCmp('win_seniorsearch')) {
        Ext.getCmp('win_seniorsearch').expand();
        return;
    }
    var fieldarray = ['DIVIDENO'];
    var store_seniorsearch = Ext.create('Ext.data.JsonStore', {
        fields: fieldarray
    })
    var field_paste = Ext.create('Ext.form.field.Text', {
        fieldLabel: '<i class="fa fa-clipboard" aria-hidden="true"></i>&nbsp;粘贴区',
        labelWidth: 60,
        listeners: {
            change: function (field_paste, newValue, oldValue, eOpts) {
                if (newValue) {
                    var len = newValue.split(" "); //获取行数   
                    var trStr;
                    for (var i = 0; i < len.length ; i++) {
                        store_seniorsearch.insert(store_seniorsearch.data.length, { 'DIVIDENO': len[i] });
                    }
                    field_paste.setValue("");
                }
            }
        }
    });
    var tbar_senior = Ext.create('Ext.toolbar.Toolbar', {
        items: [{
            text: '<i class="fa fa-plus fa-fw"></i>&nbsp;添加', handler: function () {
                store_seniorsearch.insert(store_seniorsearch.data.length, {});
            }
        }, '-', {
            text: '<i class="fa fa-trash-o"></i>&nbsp;删除', handler: function () {
                var recs = grid_seniorsearch.getSelectionModel().getSelection();
                store_seniorsearch.remove(recs);
            }
        }, '-', {
            text: '<i class="fa fa-eraser" aria-hidden="true"></i>&nbsp;清空', handler: function () {
                store_seniorsearch.removeAll();
            }
        }, '->', field_paste]
    });
    var grid_seniorsearch = Ext.create('Ext.grid.Panel', {
        tbar: tbar_senior,
        plugins: [Ext.create('Ext.grid.plugin.CellEditing', {
            clicksToEdit: 1
        })],
        columnLines: true,
        store: store_seniorsearch,
        selModel: { selType: 'checkboxmodel' },
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: '分单号', dataIndex: 'DIVIDENO', width: 130, field: { xtype: 'textfield' } }
        ]
    })
    var win_seniorsearch = Ext.create("Ext.window.Window", {
        id: 'win_seniorsearch',
        title: "高级查询",
        width: 500,
        height: 400,
        layout: "fit",
        collapsible: true,
        // modal: true,
        items: [grid_seniorsearch],
        buttonAlign: 'center',
        buttons: [{
            text: '<i class="fa fa-search"></i>&nbsp;查 询', handler: function () {
                seniorcondition = "";
                store_seniorsearch.each(
                    function (item) {
                        seniorcondition += item.get("DIVIDENO") + ",";
                    });
                var OnlySelfDom = Ext.get('OnlySelfi');
                OnlySelfDom.removeCls("fa fa-check-square-o");
                OnlySelfDom.addCls("fa fa-square-o");
                var pg_bar = Ext.getCmp(pagebar_p);
                pg_bar.moveFirst();
                win_seniorsearch.collapse();
            }
        }]
    });
    win_seniorsearch.show();
}
//非国内业务表单加载数据的方法 by panhuaguo 2016-08-19 封装至此 
function loadform() {
    Ext.Ajax.request({
        url: "/Common/loadorder",
        params: { ordercode: ordercode, copyordercode: copyordercode, busitype: busitype },
        success: function (response, opts) {
            var data = Ext.decode(response.responseText);
            formpanel.getForm().setValues(data.formdata);
            file_store.loadData(data.filedata);
            if (data.formdata.TRADEWAYCODES1) {
                Ext.getCmp('combo_myfs').setValue(data.formdata.TRADEWAYCODES1.split(','));
            }
            repunitcode = data.formdata.REPUNITNAME != null ? (data.formdata.REPUNITNAME + '(' + data.formdata.REPUNITCODE + ')') : "";
            Ext.getCmp('tf_bgsbdw').setValue(repunitcode);
            inspunitcode = data.formdata.INSPUNITNAME != null ? (data.formdata.INSPUNITNAME + '(' + data.formdata.INSPUNITCODE + ')') : "";
            Ext.getCmp('tf_bjsbdw').setValue(inspunitcode);
            //如果是修改需要将随附文件的ID拼接成字符串 赋值到
            var fileids = "";
            Ext.each(file_store.getRange(), function (rec) {
                fileids += rec.get("ID") + ",";
            });
            Ext.getCmp('field_ORIGINALFILEIDS').setValue(fileids);
            if (!win_container_truck) {
                ini_container_truck();//初始化集装箱和报关车号选择界面
            }
            else {
                Ext.getCmp('w_grid').store.loadData(Ext.decode(Ext.getCmp('field_CONTAINERTRUCK').getValue()));
            }
            formcontrol();//表单字段控制
            //add 2016/10/9 add
            if (ordercode == "" && copyordercode == "") {//编辑或复制新增时，直接以上面的formdata赋值为准，新增则需要抓取值
                if (Ext.getCmp('WEIGHTCHECK')) {
                    if (data.WEIGHTCHECK == "1") {
                        Ext.getCmp('WEIGHTCHECK').setValue(true);
                        Ext.getCmp('ISWEIGHTCHECK').setReadOnly(false);
                    } else {
                        Ext.getCmp('WEIGHTCHECK').setValue(false);

                        Ext.getCmp('ISWEIGHTCHECK').setValue(false);
                        Ext.getCmp('ISWEIGHTCHECK').setReadOnly(true);
                    }
                }
            }
        }
    });
}
//非国内业务表单控制逻辑  by panhuaguo 2016-08-19 封装至此 
function formcontrol() {
    var status = Ext.getCmp('field_STATUS').getValue();
    document.getElementById("pickfiles").disabled = status >= 10;
    document.getElementById("deletefile").disabled = status >= 10; //删除按钮  --提交后不允许删除setVisibilityMode
    document.getElementById("btn_cancelsubmit").disabled = status != 10;//撤单按钮  只有在提交后受理前才可以撤单
    document.getElementById("btn_submitorder").disabled = status >= 10;//提交按钮

    //以前是要加状态判断,考虑到撤单后页面字段状态需要重新控制 故去掉status>=15条件 by panhuaguo 2016-08-29
    //2016-08-30测试发现field_STATUS这个控件在默认只读的情况,循环表单控件的时候会将其只读属性去掉，所以要过滤掉这个控件 自始至终这个控件应该是只读的
    Ext.Array.each(formpanel.getForm().getFields().items, function (item) {
        if (item.value != "" && item.value != null && item.value != undefined && item.id != 'field_ENTRUSTREQUEST' && item.id != "field_STATUS" && item.id != "tf_bjsbdw" && item.id != "tf_bgsbdw") {
            item.setReadOnly(status >= 10);
        }
    });

    //下面是表单控件涉及的弹窗选择按钮
    Ext.getCmp('jydw_btn').setDisabled(status >= 10); //经营单位        
    Ext.getCmp("myfs_btn").setDisabled(status >= 10);//贸易方式 
    Ext.getCmp('bgsbdw_btn').setDisabled(status >= 10);//报关申报单位 
    Ext.getCmp('bjsbdw_btn').setDisabled(status >= 10); //报检申报单位 

    //集装箱号和报关车号不是所有的业务下都有这些控件，所以需要判断一下
    if (Ext.getCmp('container_btn')) {
        if (Ext.getCmp("CONTAINERNO").getValue()) {
            Ext.getCmp('container_btn').setDisabled(status >= 10);//集装箱号 
        }
    }
    if (Ext.getCmp('declcarno_btn')) {
        if (Ext.getCmp("DECLCARNO").getValue()) {
            Ext.getCmp('declcarno_btn').setDisabled(status >= 10);//报关车号  
        }
    }

    if (status < 10) {
        upload_ini();
    }
    //2016-10-18增加判断,因为提交表单后页面并没有刷新,虽然上传按钮禁用了,但uploader对象并没有销毁,还是能上传 10表示已委托 
    if (status >= 10 && uploader) {
        uploader.destroy();
    }
    wtlx_control();//委托类型对其他字段的控制
}
//非国内业务上传控件初始化方法  by panhuaguo  2016-08-19
function upload_ini() {
    uploader = new plupload.Uploader({
        runtimes: 'html5,flash,silverlight,html4',
        browse_button: 'pickfiles', // you can pass an id...
        url: '/Common/UploadFile',
        flash_swf_url: '/js/upload/Moxie.swf',
        silverlight_xap_url: '/js/upload/Moxie.xap',
        unique_names: true,
        filters: {
            max_file_size: '5000mb',
            mime_types: [
                { title: "Image files", extensions: "*" },
                { title: "Zip files", extensions: "zip,rar" }
            ]
        }
    });
    uploader.init();
    uploader.bind('FilesAdded', function (up, files) {
        uploader.start();
    });
    uploader.bind('FileUploaded', function (up, file) {
        var filetype = Ext.getCmp('combo_filetype').getValue();
        var filetypename = Ext.getCmp('combo_filetype').getRawValue();

        var timestamp = Ext.Date.now();  //1351666679575  这个方法只是获取的时间戳
        var date = new Date(timestamp);

        file_store.insert(file_store.data.length,
       { FILENAME: '/FileUpload/file/' + file.target_name, ORIGINALNAME: file.target_name, SIZES: file.size, FILETYPENAME: filetypename, FILETYPE: filetype, UPLOADTIME: Ext.Date.format(date, 'Y-m-d H:i:s') });
    });
}
//非国内业务 委托类型对其他字段的控制逻辑 by panhuaguo  2016-08-19
function wtlx_control() {
    if (Ext.getCmp('combo_ENTRUSTTYPENAME').getValue() == '01')//仅报关
    {
        Ext.getCmp('tf_bgsbdw').enable();
        Ext.getCmp('tf_bgsbdw').setValue(repunitcode);
        Ext.getCmp('cont_bgsbdw').enable();
        Ext.getCmp('tf_bjsbdw').setValue('');
        Ext.getCmp('tf_bjsbdw').disable();
        Ext.getCmp('cont_bjsbdw').disable();
    }
    if (Ext.getCmp('combo_ENTRUSTTYPENAME').getValue() == '02') {
        Ext.getCmp('tf_bjsbdw').enable();
        Ext.getCmp('tf_bjsbdw').setValue(inspunitcode);
        Ext.getCmp('cont_bjsbdw').enable();
        Ext.getCmp('tf_bgsbdw').setValue('');
        Ext.getCmp('tf_bgsbdw').disable();
        Ext.getCmp('cont_bgsbdw').disable();
        Ext.getCmp('field_CLEARANCENO').setDisabled(true);
        Ext.getCmp('field_CLEARANCENO').setValue('');
    }
    if (Ext.getCmp('combo_ENTRUSTTYPENAME').getValue() == '03') {
        Ext.getCmp('tf_bgsbdw').enable();
        Ext.getCmp('tf_bgsbdw').setValue(repunitcode);
        Ext.getCmp('cont_bgsbdw').enable();
        Ext.getCmp('tf_bjsbdw').enable();
        Ext.getCmp('tf_bjsbdw').setValue(inspunitcode);
        Ext.getCmp('cont_bjsbdw').enable();
        //委托类型是报关报检时、屏蔽通关单号字段
        Ext.getCmp('field_CLEARANCENO').setDisabled(true);
        Ext.getCmp('field_CLEARANCENO').setValue('');
    }
}
//非国内业务 随附文件展示控件 by panhuaguo 2016-08-19  1  调用此方法时注意将file_store声明为全局  2 父页面渲染的html  ID  div_panel
function panel_file_ini() {
    file_store = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'FILENAME', 'ORIGINALNAME', 'FILETYPE', 'FILETYPENAME', 'UPLOADTIME', 'SIZES', 'IETYPE']
    })
    var tmp = new Ext.XTemplate(
         '<tpl for=".">',
        '<div class="panel panel-default thumb-wrap fl" style="margin-top:5px;margin-left:5px;width:240px">',
        '<div class="panel-heading" style="padding-left:5px;padding-right:5px">{[values.ORIGINALNAME.substr(0,23)]}<div class="fr"><span class="glyphicon glyphicon-paperclip"></span></div></div>',
        '<div class="panel-body" style="padding-left:5px;">{FILETYPENAME}|',
        '<tpl>{[values.SIZES/1024 > 1024?Math.round(values.SIZES/(1024*1024))+"M":Math.round(values.SIZES/1024)+"K"]}</tpl>',
        //'|{[values.UPLOADTIME.substr(0,values.UPLOADTIME.indexOf("T"))]}</div></div>',
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
        title: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;随附文件</span></h4>',
        renderTo: "div_panel",
        border: 0,
        width: '62%',
        minHeight: 100,
        items: [fileview]
    })
}

//非国内订单订单前端保存方法 封装 by panhuaguo 2016-08-19
function save(action, busitype) {

    if (action == 'submit') {
        var validate = "";

        var mz = Ext.getCmp('GOODSGW'); var jz = Ext.getCmp('GOODSNW');
        if (Ext.typeOf(mz) != "undefined" && Ext.typeOf(jz) != "undefined") {
            if (!Ext.isEmpty(mz.getValue()) && !Ext.isEmpty(jz.getValue()) && Number(jz.getValue()) > Number(mz.getValue())) {
                validate = "净重小于应等于毛重！";
            }
        }

        if (validate) {
            Ext.MessageBox.alert("提示", validate);
            return;
        }

        if (!formpanel.getForm().isValid()) {
            return;
        }

        if (file_store.data.items.length == 0) { //如果是提交委托,必须上传文件 
            Ext.MessageBox.alert('提示', '请上传文件！');
            return;
        }
    }
    var url = "";
    switch (busitype) {
        case 10:
            url = "/OrderAirOut/Save";
            break;
        case 11:
            url = "/OrderAirIn/Save";
            break;
        case 20:
            url = "/OrderSeaOut/Save";
            break;
        case 21:
            url = "/OrderSeaIn/Save";
            break;
        case 30:
            url = "/OrderLandOut/Save";
            break;
        case 31:
            url = "/OrderLandIn/Save";
            break;
        case 50:
            url = "/OrderSpecial/Save";
            break;
    }
    var formdata = Ext.encode(formpanel.getForm().getValues());
    var filedata = Ext.encode(Ext.pluck(file_store.data.items, 'data'));
    var mask = new Ext.LoadMask(Ext.get(Ext.getBody()), { msg: "数据保存中，请稍等..." });
    mask.show();
    Ext.Ajax.request({
        url: url,
        params: { ordercode: ordercode, formdata: formdata, filedata: filedata, action: action },
        success: function (response, option) {
            if (response.responseText) {
                mask.hide();
                var data = Ext.decode(response.responseText);
                if (data.success) {
                    ordercode = data.ordercode;
                    Ext.MessageBox.alert("提示", action == 'submit' ? "提交成功！" : "保存成功！", function () {
                        loadform();
                    });
                }
                else {
                    Ext.MessageBox.alert("提示", action == 'submit' ? "提交失败！" : "保存失败！");
                }
            }
        }
    });
}
//撤单  非国内订单提交后执行撤单操作统一执行此方法 by panhuaguo 2016-08-23
function orderBack() {
    Ext.MessageBox.confirm("提示", "确定要执行撤单操作吗？", function (btn) {
        if (btn == "yes") {
            Ext.Ajax.request({
                url: '/Common/CancelSubmit',
                params: { ordercode: Ext.getCmp("field_CODE").getValue() },
                success: function (response, options) {
                    var json = Ext.decode(response.responseText);
                    if (json.success == true) {
                        Ext.MessageBox.alert("提示", "撤单成功！");
                        loadform();
                    }
                    else {
                        Ext.MessageBox.alert("提示", "撤单失败,订单状态已发生变化！");
                    }
                }
            });
        }
    });
}
//非国内业务 订单编辑页 点新增按钮时 统一调用此方法
function add_new(busitype) {
    if (busitype == 10) {
        window.location.href = "/OrderAirOut/Create";
    }
    if (busitype == 11) {
        window.location.href = "/OrderAirIn/Create";
    }
    if (busitype == 20) {
        window.location.href = "/OrderSeaOut/Create";
    }
    if (busitype == 21) {
        window.location.href = "/OrderSeaIn/Create";
    }
    if (busitype == 30) {
        window.location.href = "/OrderLandOut/Create";
    }
    if (busitype == 31) {
        window.location.href = "/OrderLandIn/Create";
    }
    if (busitype == 50) {
        window.location.href = "/OrderSpecial/Create";
    }
}

//非国内业务复制新增时统一调用此方法  by panhuaguo 2018-8-30
function copyorder(busitype) {
    if (Ext.getCmp('field_CODE') && Ext.getCmp('field_CODE').getValue()) {
        if (busitype == 10) {
            window.location.href = "/OrderAirOut/Create?copyordercode=" + Ext.getCmp('field_CODE').getValue();
        }
        if (busitype == 11) {
            window.location.href = "/OrderAirIn/Create?copyordercode=" + Ext.getCmp('field_CODE').getValue();
        }
        if (busitype == 20) {
            window.location.href = "/OrderSeaOut/Create?copyordercode=" + Ext.getCmp('field_CODE').getValue();
        }
        if (busitype == 21) {
            window.location.href = "/OrderSeaIn/Create?copyordercode=" + Ext.getCmp('field_CODE').getValue();
        }
        if (busitype == 30) {
            window.location.href = "/OrderLandOut/Create?copyordercode=" + Ext.getCmp('field_CODE').getValue();
        }
        if (busitype == 31) {
            window.location.href = "/OrderLandIn/Create?copyordercode=" + Ext.getCmp('field_CODE').getValue();
        }
        if (busitype == 50) {
            window.location.href = "/OrderSpecial/Create?copyordercode=" + Ext.getCmp('field_CODE').getValue();
        }
    }
}

function ini_container_truck() {
    var rownum = -1;//记录当前编辑的行号
    var w_store_relacontainer = Ext.create("Ext.data.JsonStore", {
        fields: ["CONTAINERSIZE", "CONTAINERTYPE", "FORMATNAME", "CONTAINERHS"],
        data: common_data_relacontainer
    });
    var w_ontainerno = Ext.create('Ext.form.field.Text', {
        name: 'CONTAINERNO',
        margin: '10',
        columnWidth: .33,
        fieldLabel: '集装箱号',
        maxLength: 11,
        minLength: 11,
        msgTarget: 'under',
        tabIndex: 1,
        enforceMaxLength: true



    });
    var w_eleshut = Ext.create('Ext.form.field.Text', {
        name: 'ELESHUT',
        margin: '10',
        columnWidth: .33,
        tabIndex: 2,
        fieldLabel: '电子关锁号'
    });
    var w_weight = Ext.create('Ext.form.field.Number', {
        name: 'CONTAINERWEIGHT',
        margin: '10',
        columnWidth: .34,
        fieldLabel: '自重',
        tabIndex: 3,
        hideTrigger: true

    });
    var w_store_containertype = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "MERGENAME", "CONTAINERCODE"],
        data: common_data_containertype
    });
    //为了确定集装箱的FORMATNAME 和HSCODE需要关联另外一张表RELA_CONTAINER(关联集装箱表),需要基于base_containertype 这个表的CODE字段
    //和base_containersize和这个表的CODE字段，来确定RELA_CONTAINER具体哪一条记录
    var w_combo_containertype = Ext.create("Ext.form.ComboBox", {
        name: "CONTAINERTYPE",
        fieldLabel: "集装箱类别",
        margin: '10',
        tabIndex: 4,
        forceSelection: true,
        store: w_store_containertype,
        columnWidth: .33,
        displayField: 'MERGENAME',
        valueField: "MERGENAME",
        queryMode: 'local',
        anyMatch: true
    });
    var w_store_containersize = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "CONTAINERSIZE", "DECLSIZE", "MERGENAME"],
        data: common_data_containersize
    });
    var w_combo_containersize = Ext.create("Ext.form.ComboBox", {
        name: "CONTAINERSIZE",
        fieldLabel: "集装箱尺寸",
        margin: '10',
        store: w_store_containersize,
        columnWidth: .33,
        displayField: 'MERGENAME',
        tabIndex: 5,
        valueField: "CONTAINERSIZE",//集装箱中文尺寸如50尺 CONTAINERSIZE 对应基础数据的NAME  为了给grid添加记录方便 特更名为CONTAINERSIZE
        queryMode: 'local',
        anyMatch: true
    });
    var w_field_declsize = Ext.create('Ext.form.field.Hidden', {
        name: 'CONTAINERSIZEE'//集装箱英文报关尺寸 如S  L
    });
    var w_store_truckno = Ext.create("Ext.data.JsonStore", {
        fields: ["LICENSE", 'MERGENAME', "UNITNO", "WHITECARD"],
        data: common_data_truckno
    });
    var w_combo_truckno = Ext.create("Ext.form.ComboBox", {
        name: "CDCARNAME",
        forceSelection: true,
        store: w_store_truckno,
        margin: '10',
        fieldLabel: "报关车号",
        displayField: 'MERGENAME',
        valueField: 'LICENSE',
        tabIndex: 6,
        columnWidth: .34,
        hideTrigger: true,
        queryMode: 'local',
        anyMatch: true
    });
    var w_field_unitno = Ext.create('Ext.form.field.Hidden', {
        name: 'UNITNO'//车队信息
    });
    var w_field_cdcarno = Ext.create('Ext.form.field.Hidden', {
        name: 'CDCARNO'//白卡号
    });
    var w_field_formatname = Ext.create('Ext.form.field.Hidden', {
        name: 'FORMATNAME' //集装箱规格名称
    });
    var w_field_hscode = Ext.create('Ext.form.field.Hidden', {
        name: 'HSCODE' //集装箱HSCODE
    });
    var w_formpanel = Ext.create('Ext.form.Panel', {
        region: 'center',
        items: [
            { layout: 'column', height: 52, border: 0, items: [w_ontainerno, w_eleshut, w_weight, ] },
            { layout: 'column', border: 0, items: [w_combo_containertype, w_combo_containersize, w_combo_truckno] },
            w_field_declsize, w_field_unitno, w_field_cdcarno, w_field_formatname, w_field_hscode
        ]
    });
    var data_containertruck = [];
    if (Ext.getCmp('field_CONTAINERTRUCK') && Ext.getCmp('field_CONTAINERTRUCK').getValue()) {
        data_containertruck = Ext.decode(Ext.getCmp('field_CONTAINERTRUCK').getValue());
    }
    var w_gridstore = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'ORDERCODE', 'CONTAINERORDER', 'CONTAINERNO', 'CONTAINERSIZEE', 'CONTAINERSIZE', 'CONTAINERWEIGHT', 'CONTAINERTYPE',
            'HSCODE', 'CONTSPECIFICATIONS', 'FORMATNAME', 'CDCARNO', 'CDCARNAME', 'UNITNO', 'ELESHUT'],
        data: data_containertruck
    })
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
                text: '<span class="icon iconfont" style="font-size:10px">&#xe606;</span>&nbsp;删 除',
                handler: function () {
                    var recs = w_grid.getSelectionModel().getSelection();
                    if (recs.length > 0) {
                        w_gridstore.remove(recs);
                    }
                    Ext.getCmp('btn_mode').setText('<span style="color:blue">新增模式</span>');
                    rownum = -1;
                }
            },
            {
                text: '<span class="icon iconfont" style="font-size:10px">&#xe60b;</span>&nbsp;保 存',
                handler: function () {
                    var msg = "";
                    if (!w_ontainerno.getValue() && !w_combo_truckno.getValue()) {
                        msg = "集装箱号和报关车号不能同时为空!";
                    }
                    if (msg) {
                        Ext.MessageBox.alert("提示", msg);
                        return;
                    }
                    if (w_combo_containersize.getValue() && w_combo_containertype.getValue()) {
                        var rec_type = w_combo_containertype.findRecordByValue(w_combo_containertype.getValue());
                        var rec_size = w_combo_containersize.findRecordByValue(w_combo_containersize.getValue());
                        //通过containertype code 和containersize code 确定w_store_relacontainer的选项
                        w_store_relacontainer.filterBy(function (record) {
                            return record.get('CONTAINERTYPE') == rec_type.get("CODE") && record.get('CONTAINERSIZE') == rec_size.get("CODE");
                        });
                        if (w_store_relacontainer.getAt(0)) {//有可能通过两种CODE匹配不到关联集装箱 所以需要判断一下
                            var rec = w_store_relacontainer.getAt(0);
                            w_field_formatname.setValue(rec.get("FORMATNAME"));
                            w_field_hscode.setValue(rec.get("CONTAINERHS"));
                        }
                        w_field_declsize.setValue(rec_size.get("DECLSIZE"));
                        w_store_relacontainer.clearFilter();
                    }
                    if (w_combo_truckno.getValue()) {
                        var rec_truckno = w_combo_truckno.findRecordByValue(w_combo_truckno.getValue());
                        w_field_unitno.setValue(rec_truckno.get("UNITNO"));
                        w_field_cdcarno.setValue(rec_truckno.get("WHITECARD"));
                    }

                    var formdata = w_formpanel.getForm().getValues();
                    if (rownum < 0) {//添加模式
                        w_gridstore.insert(w_gridstore.data.length, formdata);
                    }
                    else {//修改模式
                        var rec = w_gridstore.getAt(rownum);
                        w_gridstore.remove(rec);
                        w_gridstore.insert(rownum, formdata);
                    }
                    w_formpanel.getForm().reset();
                    rownum = -1;
                    Ext.getCmp('btn_mode').setText('<span style="color:blue">新增模式</span>');
                }
            }]
    })
    var w_grid = Ext.create('Ext.grid.Panel', {
        id: 'w_grid',
        tbar: w_tbar,
        store: w_gridstore,
        selModel: { selType: 'checkboxmodel' },
        height: 200,
        region: 'south',
        columns: [
            { header: '集装箱号', dataIndex: 'CONTAINERNO', width: 100 },
            { header: '尺寸', dataIndex: 'CONTAINERSIZE', width: 50 },
            { header: '规格', dataIndex: 'CONTAINERSIZEE', width: 50 },
            { header: '自重', dataIndex: 'CONTAINERWEIGHT', width: 60 },
            { header: '集装箱种类', dataIndex: 'CONTAINERTYPE', width: 100 },
            { header: 'HS编码', dataIndex: 'HSCODE', width: 90 },
            { header: '集装箱规格', dataIndex: 'FORMATNAME', width: 100 },
            { header: '报关车号', dataIndex: 'CDCARNO', width: 85 },//实际显示的是白卡号
            { header: '报关车名', dataIndex: 'CDCARNAME', width: 85 },//中文牌照信息
            { header: '车队名称', dataIndex: 'UNITNO', flex: 1 },
            { header: '电子关锁号', dataIndex: 'ELESHUT', width: 90 }
        ],
        listeners: {
            itemdblclick: function (w_grid, record, item, index, e, eOpts) {
                rownum = index;
                w_formpanel.getForm().setValues(record.data);
                //反向编辑时需要通过FORMATNAME确定CONTAINERTYPE
                //var rec_ctype = w_store_containertype.findRecord('MERGENAME', record.get('CONTAINERTYPE'));
                //w_combo_containertype.setValue(rec_ctype.get("CODE"));
                Ext.getCmp('btn_mode').setText('<span style="color:blue">编辑模式</span>');
            }
        }
    });
    win_container_truck = Ext.create("Ext.window.Window", {
        title: "集装箱&报关车号维护",
        width: 1150,
        height: 400,
        closeAction: 'hide',
        layout: "border",
        modal: true,
        items: [w_formpanel, w_grid],
        buttonAlign: 'center',
        buttons: [{
            text: '<i class="fa fa-search"></i>&nbsp;确 定', handler: function () {
                if (w_gridstore.data.length > 0) {//将列表数据序列化成数组保存至订单表CONTAINERTRUCK字段                
                    var detaildata = Ext.encode(Ext.pluck(w_gridstore.data.items, 'data'));
                    Ext.getCmp('field_CONTAINERTRUCK').setValue(detaildata);
                    Ext.each(w_gridstore.getRange(), function (rec) {  //找到第一条有报关车牌号的信息
                        if (rec.get("CDCARNAME") && Ext.getCmp('DECLCARNO')) {
                            Ext.getCmp('DECLCARNO').setValue(rec.get("CDCARNAME"));
                            return false;
                        }
                    });
                    Ext.each(w_gridstore.getRange(), function (rec) {  //找到第一条有集装箱号的记录
                        if (rec.get("CONTAINERNO") && Ext.getCmp('CONTAINERNO')) {
                            Ext.getCmp('CONTAINERNO').setValue(rec.get("CONTAINERNO"));
                            return false;
                        }
                    });
                }
                else {
                    Ext.getCmp('DECLCARNO').setValue("");
                    if (Ext.getCmp('CONTAINERNO')) {
                        Ext.getCmp('CONTAINERNO').setValue("");
                    }
                    Ext.getCmp('field_CONTAINERTRUCK').setValue("");
                }
                win_container_truck.close();
            }
        }]
    });
}

//非国内业务 列表页删除功能
function DeleteNotGuoNei() {
    var recs = gridpanel.getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要删除的记录！');
        return;
    }

    var bf = false;
    if (recs[0].data.ENTRUSTTYPE == "01") {
        if (recs[0].data.DECLSTATUS != '0') { bf = true; }
    }
    if (recs[0].data.ENTRUSTTYPE == "02") {
        if (recs[0].data.INSPSTATUS != '0') { bf = true; }
    }
    if (recs[0].data.ENTRUSTTYPE == "03") {
        if (recs[0].data.DECLSTATUS != '0' || recs[0].data.INSPSTATUS != '0') { bf = true; }
    }

    if (bf) {
        Ext.MessageBox.alert('提示', '已委托的订单不能删除！');
        return;
    }

    Ext.MessageBox.confirm("提示", "确定要删除所选择的记录吗？", function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/Common/Delete',
                params: { ordercode: recs[0].get("CODE") },
                success: function (response, success, option) {
                    var res = Ext.decode(response.responseText);
                    if (res.success) {
                        Ext.MessageBox.alert('提示', '删除成功！');
                        store_Trade.load();
                    }
                    else {
                        Ext.MessageBox.alert('提示', '删除失败！');
                    }
                }
            });
        }
    });
}


//查询界面 grid的列方法
function renderOrder(value, cellmeta, record, rowIndex, columnIndex, store) {
    var rtn = "";
    var dataindex = cellmeta.column.dataIndex;
    switch (dataindex) {
        case "DECLSTATUS":
        case "INSPSTATUS":
            if (record.get("ISINVALID") == "1") {
                rtn = "已作废";
            }
            else {
                rtn = orders_tatus[value];
            }
            break;
        case "BUSITYPE":
            var rec = store_busitype.findRecord('CODE', value);
            if (rec) {
                rtn = rec.get("NAME");
            }
            break;
        case "REPWAYID":
            var rec = store_sbfs.findRecord('CODE', value);
            if (rec) {
                rtn = rec.get("NAME");
            }
            break;
        case "LAWFLAG":
            rtn = value ? "有" : "无";
            break;
        case "GOODSNUM":
            if (record.get("GOODSNUM")) {
                rtn = record.get("GOODSNUM") + '/' + record.get("GOODSGW");
            }
            break;
        case "WOODPACKINGID"://木质包装
            var rec = store_mzbz.findRecord('CODE', value);
            if (rec) {
                rtn = rec.get("NAME");
            }
            break;
    }
    return rtn;
}

function openwin(type) {
    var recs = gridpanel.getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要维护的记录！');
        return;
    }
    var plwhids = "";
    for (var i = 0; i < recs.length; i++) {
        plwhids += recs[i].data.ID + ',';
    }
    plwhids = plwhids.substr(0, plwhids.length - 1);
    opencenterwin("/Common/BatchMaintain?ids=" + plwhids + "&type=" + type, 1200, 600);
}
