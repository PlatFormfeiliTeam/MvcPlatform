﻿
//=======================================================JS init begin======================================================

var gridpanel;
var pgbar;
var common_data_sbgq = [], common_data_sbfs = [], common_data_adminurl = "", common_data_jydw = [];
var store_busitype;
var store_STATUS = Ext.create('Ext.data.JsonStore', {
    fields: ['CODE', 'NAME'],
    data: [{ "CODE": 5, "NAME": "草稿" }, { "CODE": 10, "NAME": "已提交" }, { "CODE": 15, "NAME": "已受理" }]
});
Ext.onReady(function () {
    store_busitype = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_busitype
    });
    Ext.Ajax.request({
        url: "/Common/Ini_Base_Data",
        success: function (response, opts) {
            var commondata = Ext.decode(response.responseText);
            common_data_sbgq = commondata.sbgq;//申报关区   for win 窗口使用
            common_data_sbfs = commondata.sbfs;//申报方式   for win 窗口使用
            common_data_adminurl = commondata.adminurl;//文件服务器URL
            common_data_jydw = commondata.jydw;//经营单位
            //查询区域
            initSearch();
            gridpanelBind();
        }
    });
});

//=======================================================JS init end======================================================


function Open() {
    addwin("");
}

function initSearch() {
    var store_jydw = Ext.create('Ext.data.JsonStore', {  //报关行combostore
        fields: ['CODE', 'NAME'],
        data: common_data_jydw
    });

    //文件接收单位
    var field_FILERECEVIEUNIT_S = Ext.create('Ext.form.field.ComboBox', {
        id: 'FILERECEVIEUNIT_S',
        name: 'FILERECEVIEUNIT_S',
        fieldLabel: '文件接收单位',
        store: store_jydw,
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
        minChars: 4,
        hideTrigger: true,
        anyMatch: true,
        forceSelection: true
    })


    //文件申报单位
    var field_FILEDECLAREUNIT_S = Ext.create('Ext.form.field.ComboBox', {
        id: 'FILEDECLAREUNIT_S',
        name: 'FILEDECLAREUNIT_S',
        fieldLabel: '文件申报单位',
        store: store_jydw,
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
        minChars: 4,
        hideTrigger: true,
        anyMatch: true,
        forceSelection: true
    })



    //企业编号
    var field_CODE_S = Ext.create('Ext.form.field.Text', {
        id: 'CODE_S',
        fieldLabel: '企业编号',
        name: 'CODE_S',
        flex: .33
    });

    var start_date = Ext.create('Ext.form.field.Date', {
        id: 'start_date',
        format: 'Y-m-d',
        name: 'start_date',
        flex: .5, margin: 0
    })
    var end_date = Ext.create('Ext.form.field.Date', {
        id: 'end_date',
        format: 'Y-m-d',
        name: 'end_date',
        flex: .5, margin: 0
    })
    //创建时间
    var wt_date = Ext.create('Ext.form.FieldContainer', {
        fieldLabel: '提交日期',
        //labelWidth: 60,
        layout: 'hbox',
        columnWidth: .33,
        items: [start_date, end_date],
        //flex: .33
    })

    var combo_STATUS = Ext.create('Ext.form.field.ComboBox', {
        id: 'STATUS_S',
        fieldLabel: '状态',
        name: 'STATUS_S',
        store: store_STATUS,
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
        minChars: 1,
        hideTrigger: false,
        anyMatch: true,
        flex: .33
    })
    var formpanel = Ext.create('Ext.form.Panel', {
        id: 'formpanel',
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '0 5 10 5',
            columnWidth: 0.33, labelWidth: 80,
        },
        items: [
        { layout: 'column', columnWidth: 1, border: 0, margin: '5 0 0 0', items: [field_FILERECEVIEUNIT_S, field_FILEDECLAREUNIT_S, field_CODE_S] },
        { layout: 'column', border: 0, margin: '5 0 0 0', items: [wt_date, combo_STATUS] }
        ]
    });
}

function gridpanelBind() {
    var store_Trade = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'FILERECEVIEUNITCODE', 'FILERECEVIEUNITNAME', 'FILEDECLAREUNITCODE', 'FILEDECLAREUNITNAME',
            'BUSITYPEID', 'CUSTOMDISTRICTCODE', 'CUSTOMDISTRICTNAME', 'REPWAYID', 'NEWSTATUS',
             'CODE', 'CREATEID', 'CREATENAME', 'CREATETIME', 'ASSOCIATENO', 'ORDERCODE', 'ENTERPRISECODE', 'ENTERPRISENAME', 'ACCEPTID', 'ACCEPTNAME',
             'ACCEPTTIME', 'UNITCODE', 'CREATEMODE', 'REMARK', 'SUBMITTIME'],
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/EnterpriseOrder/loadOrderList',
            reader: {
                root: 'rows',
                type: 'json',
                totalProperty: 'total'
            }
        },
        autoLoad: true,
        listeners: {
            beforeload: function () {
                store_Trade.getProxy().extraParams = {
                    FILERECEVIEUNIT: Ext.getCmp('FILERECEVIEUNIT_S').getValue(),
                    FILEDECLAREUNIT: Ext.getCmp("FILEDECLAREUNIT_S").getValue(),
                    CODE: Ext.getCmp('CODE_S').getValue(),
                    STARTDATE: Ext.Date.format(Ext.getCmp("start_date").getValue(), 'Y-m-d H:i:s'),
                    ENDDATE: Ext.Date.format(Ext.getCmp("end_date").getValue(), 'Y-m-d H:i:s'),
                    STATUS: Ext.getCmp('STATUS_S').getValue()
                }
            }
        }
    })
    pgbar = Ext.create('Ext.toolbar.Paging', {
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_Trade,
        id: 'pgbar',
        displayInfo: true
    })
    gridpanel = Ext.create('Ext.grid.Panel', {
        renderTo: "appConId",
        store: store_Trade,
        height: 500,
        selModel: { selType: 'checkboxmodel' },
        bbar: pgbar,
        enableColumnHide: false,
        columns: [
        { xtype: 'rownumberer', width: 35, locked: true },
        { header: 'ID', dataIndex: 'ID', hidden: true, locked: true },
        { header: '文件统一编号', dataIndex: 'UNITCODE', width: 150, locked: true },
        { header: '企业|对应编号', dataIndex: 'CODE', width: 120, locked: true },
        { header: '文件接收单位', dataIndex: 'FILERECEVIEUNITNAME', width: 200, locked: true },
        { header: '文件申报单位', dataIndex: 'FILEDECLAREUNITNAME', width: 200, locked: true },
        { header: '业务类型', dataIndex: 'BUSITYPEID', width: 100, renderer: render, locked: true },
        { header: '申报关区', dataIndex: 'CUSTOMDISTRICTCODE', width: 70 },
        { header: '状态', dataIndex: 'NEWSTATUS', width: 70, renderer: render },
        { header: '提交时间', dataIndex: 'SUBMITTIME', width: 140, locked: true },
        { header: '备注', dataIndex: 'REMARK', width: 150 }
        ],
        listeners:
        {
            'itemdblclick': function (view, record, item, index, e) {
                opencenterwin("/EnterpriseOrder/BatchMaintain?ids=" + record.data.ID + "&unitcodes=" + record.data.UNITCODE, 1200, 800);
            }
        },
        viewConfig: {
            enableTextSelection: true
        }
    })
}

function render(value, cellmeta, record, rowIndex, columnIndex, store) {
    var rtn = "";
    var dataindex = cellmeta.column.dataIndex;
    switch (dataindex) {
        case "BUSITYPEID":
            var rec = store_busitype.findRecord('CODE', value);
            if (rec) {
                rtn = rec.get("NAME");
            }
            break;
        case "NEWSTATUS":
            var rec = store_STATUS.findRecord('CODE', value);
            if (rec) {
                rtn = rec.get("NAME");
            }

    }
    return rtn;
}

function Select() {
    Ext.getCmp('pgbar').moveFirst();
}

function Reset() {
    Ext.Array.each(Ext.getCmp('formpanel').getForm().getFields().items, function (field) {
        field.reset();
    });
}

function Delete() {
    var recs = gridpanel.getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要删除的记录！');
        return;
    }

    //if (recs[0].data.NEWSTATUS != '5') {
    //    Ext.MessageBox.alert('提示', '只能删除状态为草稿的记录！');
    //    return;
    //}
    Ext.MessageBox.confirm("提示", "确定要删除所选择的记录吗？", function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/EnterpriseOrder/DeleteList',
                params: { recs: Ext.encode(Ext.pluck(recs,'data')) },
                success: function (response, success, option) {
                    var res = Ext.decode(response.responseText);
                    if (res.success) {
                        Ext.MessageBox.alert('提示', '删除成功！' + res.message);
                        gridpanel.store.reload();
                    }
                    else {
                        Ext.MessageBox.alert('提示', '删除失败！' + res.message);
                    }
                }
            });
        }
    });
}

function openBatchWin() {
    var recs = gridpanel.getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要维护的记录！');
        return;
    }
    var plwhids = "";
    for (var i = 0; i < recs.length; i++) {
        plwhids += recs[i].data.ID + ',';
    }
    var unitcodes = "";
    for (var i = 0; i < recs.length; i++) {
        unitcodes += recs[i].data.UNITCODE + ',';
    }
    plwhids = plwhids.substr(0, plwhids.length - 1);
    unitcodes = unitcodes.substr(0, unitcodes.length - 1);
    opencenterwin("/EnterpriseOrder/BatchMaintain?ids=" + plwhids + "&unitcodes=" + unitcodes, 1200, 800);

}