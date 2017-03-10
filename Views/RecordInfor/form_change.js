function form_ini() {
    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;基础信息</span></h4>'
    }
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo, '->']
    });

    //账册号
    var store_RECORDINFOID = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME', 'ID'],
        data: common_data_recordid
    });
    var combo_RECORDINFOID = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_RECORDINFOID',
        name: 'RECORDINFOID',
        store: store_RECORDINFOID,
        fieldLabel: '账册号',
        displayField: 'NAME',
        valueField: 'ID',
        triggerAction: 'all',
        forceSelection: true,
        queryMode: 'local',
        anyMatch: true,
        readOnly: true, 
        fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    //项号
    var field_ITEMNO = Ext.create('Ext.form.field.Text', {
        id: 'ITEMNO',
        name: 'ITEMNO',
        fieldLabel: '项号',
        readOnly: true, 
        fieldStyle: 'background-color: #CECECE; background-image: none;'
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
        readOnly: true,
        fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    //备案关区
    var store_CUSTOMAREA = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'NAME', 'CUSTOMAREA', 'CUSTOMAREANAME'],
        data: common_data_customarea
    });
    var combo_CUSTOMAREA = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_CUSTOMAREA',
        name: 'CUSTOMAREA',
        store: store_CUSTOMAREA,
        fieldLabel: '备案关区',
        displayField: 'CUSTOMAREANAME',
        valueField: 'ID',
        triggerAction: 'all',
        forceSelection: true,
        queryMode: 'local',
        anyMatch: true, 
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                }
            },
            change: function (field_paste, newValue, oldValue, eOpts) {
                //if (newValue != oldValue) {
                //    Element_ini();
                //}
            }
        },
        allowBlank: false,
        blankText: '备案关区不能为空!'
    });

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //HS编码left
    var field_HSCODE_left = Ext.create('Ext.form.field.Text', { id: 'HSCODE_left', name: 'HSCODE_left', readOnly: true, fieldLabel: 'HS编码', flex: .5 });
    //附加码left
    var field_ADDITIONALNO_left = Ext.create('Ext.form.field.Text', { id: 'ADDITIONALNO_left', name: 'ADDITIONALNO_left', readOnly: true, fieldLabel: 'HS附加码', flex: .5, margin: 0 });

    var field_1_left = { columnWidth: 1, xtype: 'fieldcontainer', layout: 'hbox', margin: 0, items: [field_HSCODE_left, field_ADDITIONALNO_left] };




    var con_container_left = {
        columnWidth: .5,
        xtype: 'fieldcontainer',
        layout: 'column',
        items: [
            { columnWidth: 1, layout: 'column', border: 0, height: 42, margin: '5 0 0 0', items: [field_1_left] }
        ]
    }

    //HS编码right
    var field_HSCODE = Ext.create('Ext.form.field.Text', {
        id: 'HSCODE', name: 'HSCODE', fieldLabel: 'HS编码', flex: .5,
        listeners: {
            change: function (field_paste, newValue, oldValue, eOpts) {
                //if (newValue != Ext.getCmp('HSCODE_left').getValue()) {
                //    Element_ini();
                //}
            }
        },
        allowBlank: false, blankText: 'HS编码不能为空!'
    });

    //附加码right
    var field_ADDITIONALNO = Ext.create('Ext.form.field.Text', {
        id: 'ADDITIONALNO', name: 'ADDITIONALNO', fieldLabel: 'HS附加码', flex: .5, margin: 0,
        maxLength: 2, minLength: 2, enforceMaxLength: true, minLengthText: '附加码为2位！',
        listeners: {
            change: function (field_paste, newValue, oldValue, eOpts) {
                //if (newValue != Ext.getCmp('ADDITIONALNO_left').getValue()) {
                //    Element_ini();
                //}
            }
        },
        allowBlank: false, blankText: '附加码不能为空!'
    });

    var field_1_right = { columnWidth: 1, xtype: 'fieldcontainer', layout: 'hbox', margin: 0, items: [field_HSCODE, field_ADDITIONALNO] };


    var con_container_right = {
        columnWidth: .5,
        xtype: 'fieldcontainer',
        layout: 'column',
        items: [
            { columnWidth: 1, layout: 'column', border: 0, height: 42, margin: '5 0 0 0', items: [field_1_right] }
        ]
    }

    var panel_ele = Ext.create('Ext.panel.Panel', {//申报要素
        id: 'panel_ele',
        columnWidth: 1,
        border: 0,
        items: []
    });

    var panel_compare = Ext.create('Ext.panel.Panel', {
        id: 'panel_compare',
        columnWidth: 1,
        border: 1,
        items: [
            { layout: 'column', border: 0, items: [con_container_left, con_container_right] },
            { layout: 'column', border: 0, items: [panel_ele] }
        ]
    });

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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
        flex: .33,
        value: 'A',
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
        editable: false,
        flex: .33,
        value: 0,
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
        editable: false,
        flex: .33,
        value: 0,
        fieldStyle: 'background-color: #CECECE; background-image: none;'
    });


    var combo_container = {
        columnWidth: .50,
        xtype: 'fieldcontainer',
        layout: 'hbox', margin: 0,
        items: [combo_OPTIONS, combo_STATUS, combo_ISPRINT_APPLY]
    }
    //备注
    var field_REMARK = Ext.create('Ext.form.field.Text', {
        id: 'REMARK',
        name: 'REMARK',
        fieldLabel: '备注',
        flex: .50


    });
    //修改原因
    var field_MODIFYREASON = Ext.create('Ext.form.field.Text', {
        id: 'MODIFYREASON',
        name: 'MODIFYREASON',
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
    
    var field_CREATEID = Ext.create('Ext.form.field.Hidden', { name: 'CREATEID' });
    var field_SUBMITID = Ext.create('Ext.form.field.Hidden', { name: 'SUBMITID' });
    var field_PREID = Ext.create('Ext.form.field.Hidden', { name: 'PREID' });
    var field_FINISHID = Ext.create('Ext.form.field.Hidden', { name: 'FINISHID' });

    var configItem = [
        { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_RECORDINFOID, field_ITEMNO, combo_ITEMNOATTRIBUTE, combo_CUSTOMAREA] },
        { layout: 'column', border: 0, items: [panel_compare] },
        { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [textarea_container] },
        { layout: 'column', height: 42, border: 0, items: [combo_container] },
        { layout: 'column', height: 42, border: 0, items: [field_CREATEDATE, field_CREATENAME, field_SUBMITTIME, field_SUBMITNAME] },
        { layout: 'column', height: 42, border: 0, items: [field_ACCEPTTIME, field_ACCEPTNAME, field_PRETIME, field_PRENAME] },
        field_CREATEID, field_SUBMITID, field_PREID, field_FINISHID//field_CUSTOMERNAME, 
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

function form_ini_btn() {

    var bbar_r = '<div class="btn-group" role="group">'
                        + '<button type="button" onclick="" id="btn_cancelsubmit" class="btn btn-primary btn-sm"><i class="fa fa-angle-double-left"></i>&nbsp;撤回</button>'
                        + '<button type="button" onclick="" id="btn_save" class="btn btn-primary btn-sm"><i class="fa fa-floppy-o"></i>&nbsp;保存</button>'
                        + '<button type="button" onclick="" id="btn_submitorder" class="btn btn-primary btn-sm"><i class="fa fa-hand-o-up"></i>&nbsp;提交申请</button></div>'

    var bbar_l = '<div class="btn-group">'
               + '<button type="button" onclick="" id="btn_print" class="btn btn-primary btn-sm"><i class="fa fa-print"></i>&nbsp;申请表打印</button>'
           + '</div>';
    var bbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [bbar_l, '->', bbar_r]
    })

    var formpanel_btn = Ext.create('Ext.form.Panel', {
        renderTo: 'div_form_btn',
        border: 0,
        bbar: bbar
    });
}


function loadform_record() {
   
}

