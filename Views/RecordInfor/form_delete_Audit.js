function form_ini_Audit() {
    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;基础信息</span></h4>'
    }
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo, '->']
    })

    //委托企业
    var store_enterprise = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_jydw
    });
    var combo_enterprise = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_enterprise',
        name: 'ENTERPRISE', readOnly: true,
        store: store_enterprise,
        fieldLabel: '委托企业',
        displayField: 'NAME',
        minChars: 4,
        valueField: 'CODE',
        triggerAction: 'all',
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local'
    });

    var field_RECORDINFONAME = Ext.create('Ext.form.field.Text', {
        id: 'RECORDINFONAME',
        name: 'RECORDINFONAME', readOnly: true,
        fieldLabel: '账册号',
        fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    var field_RECORDINFOID = Ext.create('Ext.form.field.Hidden', { id: 'RECORDINFOID', name: 'RECORDINFOID' });

    //备案关区
    var store_CUSTOMAREA = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'NAME', 'CUSTOMAREA', 'CUSTOMAREANAME'],
        data: common_data_customarea
    });
    var combo_CUSTOMAREA = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_CUSTOMAREA',
        name: 'CUSTOMAREA', readOnly: true,
        store: store_CUSTOMAREA,
        fieldLabel: '备案关区',
        displayField: 'CUSTOMAREANAME',
        valueField: 'ID',
        triggerAction: 'all',
        forceSelection: true,
        queryMode: 'local',
        anyMatch: true,
        hideTrigger: true,
        listeners: {
            change: function (field_paste, newValue, oldValue, eOpts) {
                if (newValue != oldValue) {
                    Element_ini();
                }
            }
        }
    });


    //报关行
    var store_CUSTOMER = Ext.create('Ext.data.JsonStore', {  //报关行combostore
        fields: ['CODE', 'NAME'],
        data: common_data_jydw
    })
    var combo_CUSTOMER = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_CUSTOMER',
        name: 'CUSTOMERCODE', readOnly: true,
        fieldLabel: '报关行',
        store: store_CUSTOMER,
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        margin: 0,
        minChars: 4,
        anyMatch: true,
        hideTrigger: true
    });

    //项号
    var field_ITEMNO = Ext.create('Ext.form.field.Text', {
        id: 'ITEMNO',
        name: 'ITEMNO',
        fieldLabel: '项号',
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    //项号属性
    var store_ITEMNOATTRIBUTE = Ext.create('Ext.data.JsonStore', {
        fields: ['NAME'],
        data: [{ "NAME": "料件" }, { "NAME": "成品" }]
    });
    var combo_ITEMNOATTRIBUTE = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ITEMNOATTRIBUTE',
        name: 'ITEMNOATTRIBUTE',
        store: store_ITEMNOATTRIBUTE,
        fieldLabel: '项号属性',
        displayField: 'NAME',
        valueField: 'NAME',
        triggerAction: 'all',
        forceSelection: true,
        queryMode: 'local',
        anyMatch: true,
        listeners: {
            change: function (field_paste, newValue, oldValue, eOpts) {
                if (newValue == "成品") {
                    form_ini_con_Audit();
                }
            }
        },
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    //HS编码
    var field_HSCODE = Ext.create('Ext.form.field.Text', {
        id: 'HSCODE',
        name: 'HSCODE',
        flex: 0.75, margin: 0,
        listeners: {
            change: function (field_paste, newValue, oldValue, eOpts) {
                if (newValue != oldValue) {
                    Element_ini();
                }
            }
        },
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    //附加码
    var field_ADDITIONALNO = Ext.create('Ext.form.field.Text', {
        id: 'ADDITIONALNO',
        name: 'ADDITIONALNO',
        flex: 0.25, margin: 0,
        maxLength: 2,
        minLength: 2,
        enforceMaxLength: true,
        minLengthText: '2位！',
        listeners: {
            change: function (field_paste, newValue, oldValue, eOpts) {
                if (newValue != oldValue) {
                    Element_ini();
                }
            }
        },
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    var hs_container = {
        columnWidth: .25,
        xtype: 'fieldcontainer',
        layout: 'hbox',
        fieldLabel: 'HS编码/附加码',
        items: [field_HSCODE, field_ADDITIONALNO]
    }

    //商品名称
    var field_COMMODITYNAME = Ext.create('Ext.form.field.Text', {
        id: 'COMMODITYNAME',
        name: 'COMMODITYNAME',
        fieldLabel: '商品名称',
        listeners: {
            change: function (field_paste, newValue, oldValue, eOpts) {
                if (Ext.getCmp('panel_ele_2')) {
                    var id_pm = Ext.getCmp('id_pinming').getValue();
                    if (id_pm != "") {
                        if (Ext.getCmp(id_pm)) { Ext.getCmp(id_pm).setValue(newValue); }
                    }
                }
            }
        },
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    //规格型号
    var field_SPECIFICATIONSMODEL = Ext.create('Ext.form.field.Text', {
        id: 'SPECIFICATIONSMODEL',
        name: 'SPECIFICATIONSMODEL',
        fieldLabel: '规格型号',
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    //成交单位
    var store_UNIT = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME', 'CODENAME'],
        data: common_data_unit
    });
    var combo_UNIT = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_UNIT',
        name: 'UNIT',
        store: store_UNIT,
        fieldLabel: '成交单位',
        displayField: 'CODENAME',
        valueField: 'CODE',
        hideTrigger: true,
        triggerAction: 'all',
        forceSelection: true,
        queryMode: 'local',
        anyMatch: true,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });


    //变动状态
    var store_OPTIONS = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: optionstatus_js_data
    });
    var combo_OPTIONS = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_OPTIONS',
        name: 'OPTIONS',
        store: store_OPTIONS,
        fieldLabel: '<font color=red>变动状态</font>',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
        readOnly: true,
        editable: false, 
        fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    //状态
    var store_STATUS = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: status_js_data
    });
    var combo_STATUS = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_STATUS',
        name: 'STATUS',
        store: store_STATUS,
        fieldLabel: '状态',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
        readOnly: true,
        editable: false, flex: 0.5, margin: 0,
        fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    //打印状态
    var store_ISPRINT_APPLY = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: [{ "CODE": 0, "NAME": "未打印" }, { "CODE": 1, "NAME": "已打印" }]
    });
    var combo_ISPRINT_APPLY = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ISPRINT_APPLY',
        name: 'ISPRINT_APPLY',
        store: store_ISPRINT_APPLY,
        fieldLabel: '打印状态',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
        readOnly: true,
        editable: false, flex: 0.5, margin: 0,
        fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    var status_container = {
        columnWidth: .25,
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_STATUS, combo_ISPRINT_APPLY]
    }

    //-----------------------------

    //备注
    var field_REMARK = Ext.create('Ext.form.field.Text', {
        id: 'REMARK',
        name: 'REMARK', readOnly: true,
        fieldLabel: '备注',
        flex: .50


    });
    //修改原因
    var field_MODIFYREASON = Ext.create('Ext.form.field.Text', {
        id: 'MODIFYREASON',
        name: 'MODIFYREASON', readOnly: true,
        fieldLabel: '<font color=red>修改原因</font>',
        fieldStyle: 'border-color:red;',
        flex: .50
    });
    var textarea_container = {
        columnWidth: 1,
        xtype: 'fieldcontainer',
        layout: 'hbox', margin: 0,
        items: [field_REMARK, field_MODIFYREASON]
    }

    //维护时间
    var field_CREATEDATE = Ext.create('Ext.form.field.Text', {
        id: 'CREATEDATE',
        name: 'CREATEDATE',
        fieldLabel: '维护时间',
        readOnly: true,
        fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //维护人
    var field_CREATENAME = Ext.create('Ext.form.field.Text', {
        id: 'CREATENAME',
        name: 'CREATENAME',
        fieldLabel: '维护人',
        readOnly: true,
        fieldStyle: 'background-color: #CECECE; background-image: none;'

    });
    //提交时间
    var field_SUBMITTIME = Ext.create('Ext.form.field.Text', {
        id: 'SUBMITTIME',
        name: 'SUBMITTIME',
        fieldLabel: '提交时间',
        readOnly: true,
        fieldStyle: 'background-color: #CECECE; background-image: none;'

    });
    //提交人
    var field_SUBMITNAME = Ext.create('Ext.form.field.Text', {
        id: 'SUBMITNAME',
        name: 'SUBMITNAME',
        fieldLabel: '提交人',
        readOnly: true,
        fieldStyle: 'background-color: #CECECE; background-image: none;'

    });
    //受理时间
    var field_ACCEPTTIME = Ext.create('Ext.form.field.Text', {
        id: 'ACCEPTTIME',
        name: 'ACCEPTTIME',
        fieldLabel: '受理时间',
        readOnly: true,
        fieldStyle: 'background-color: #CECECE; background-image: none;'

    });
    //受理人
    var field_ACCEPTNAME = Ext.create('Ext.form.field.Text', {
        id: 'ACCEPTNAME',
        name: 'ACCEPTNAME',
        fieldLabel: '受理人',
        readOnly: true,
        fieldStyle: 'background-color: #CECECE; background-image: none;'

    });
    //预录时间
    var field_PRETIME = Ext.create('Ext.form.field.Text', {
        id: 'PRETIME',
        name: 'PRETIME',
        fieldLabel: '预录时间',
        readOnly: true,
        fieldStyle: 'background-color: #CECECE;background-image: none;'

    });
    //预录人
    var field_PRENAME = Ext.create('Ext.form.field.Text', {
        id: 'PRENAME',
        name: 'PRENAME',
        fieldLabel: '预录人',
        readOnly: true,
        fieldStyle: 'background-color: #CECECE; background-image: none;'

    });

    //申报时间
    var field_REPTIME = Ext.create('Ext.form.field.Text', {
        id: 'REPTIME', name: 'REPTIME', fieldLabel: '申报时间', readOnly: true, fieldStyle: 'background-color: #CECECE;background-image: none;'
    });
    //申报人
    var field_REPNAME = Ext.create('Ext.form.field.Text', {
        id: 'REPNAME', name: 'REPNAME', fieldLabel: '申报人', readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //确认完成时间
    var field_FINISHTIME = Ext.create('Ext.form.field.Text', {
        id: 'FINISHTIME', name: 'FINISHTIME', fieldLabel: '确认时间', readOnly: true, fieldStyle: 'background-color: #CECECE;background-image: none;'
    });
    //确认完成人
    var field_FINISHNAME = Ext.create('Ext.form.field.Text', {
        id: 'FINISHNAME', name: 'FINISHNAME', fieldLabel: '确认人', readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    var field_CREATEID = Ext.create('Ext.form.field.Hidden', { name: 'CREATEID' });
    var field_SUBMITID = Ext.create('Ext.form.field.Hidden', { name: 'SUBMITID' });
    var field_ACCEPTID = Ext.create('Ext.form.field.Hidden', { name: 'ACCEPTID' });
    var field_PREID = Ext.create('Ext.form.field.Hidden', { name: 'PREID' });
    var field_REPID = Ext.create('Ext.form.field.Hidden', { name: 'REPID' });
    var field_FINISHID = Ext.create('Ext.form.field.Hidden', { name: 'FINISHID' });

    var field_RID = Ext.create('Ext.form.field.Hidden', { name: 'RID', id: 'RID' });


    var panel_ele = Ext.create('Ext.panel.Panel', {
        id: 'panel_ele',
        columnWidth: 1,
        border: 0,
        items: []
    });

    var configItem = [
        { layout: 'column', height: 42, border: 0, margin: '5 0 0 0', items: [combo_enterprise, combo_CUSTOMAREA, combo_CUSTOMER, combo_OPTIONS] },
        { layout: 'column', height: 42, border: 0, items: [field_RECORDINFONAME, field_ITEMNO, combo_ITEMNOATTRIBUTE, status_container] },
        { layout: 'column', height: 42, border: 0, items: [field_COMMODITYNAME, field_SPECIFICATIONSMODEL, hs_container, combo_UNIT] },
        { layout: 'column', border: 0, items: [panel_ele] },
        { layout: 'column', height: 42, border: 0, items: [textarea_container] },
        { layout: 'column', height: 42, border: 0, items: [field_CREATEDATE, field_CREATENAME, field_SUBMITTIME, field_SUBMITNAME] },
        { layout: 'column', height: 42, border: 0, items: [field_ACCEPTTIME, field_ACCEPTNAME, field_PRETIME, field_PRENAME] },
        { layout: 'column', height: 42, border: 0, items: [field_REPTIME, field_REPNAME, field_FINISHTIME, field_FINISHNAME] },
        field_RECORDINFOID, field_CREATEID, field_SUBMITID, field_ACCEPTID, field_PREID, field_REPID, field_FINISHID, field_RID
    ];

    var formpanel = Ext.create('Ext.form.Panel', {
        id: 'formpanel_id',
        renderTo: 'div_form',
        minHeight: 250,
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
        items: configItem
    });
}