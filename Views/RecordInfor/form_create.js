function form_ini() {
    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;基础信息</span></h4>'
    }
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo, '->']
    })

    //账册号
    var store_RECORDINFOID = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME', 'ID'],
        data: common_data_recordid
    });
    var combo_RECORDINFOID = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_RECORDINFOID',
        name: 'RECORDINFOID',
        store: store_RECORDINFOID,
        hideTrigger: true,
        fieldLabel: '账册号',
        displayField: 'NAME',
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
            }
        },
        allowBlank: false,
        blankText: '账册号不能为空!'
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
        value: store_CUSTOMAREA.getCount() > 0 ? store_CUSTOMAREA.getAt(0).get("ID") : '',
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                }
            }
        },
        allowBlank: false,
        blankText: '备案关区不能为空!'
    });


    //报关行
    var store_CUSTOMER = Ext.create('Ext.data.JsonStore', {  //报关行combostore
        fields: ['CODE', 'NAME'],
        data: common_data_jydw
    })
    var combo_CUSTOMER = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_CUSTOMER',
        name: 'CUSTOMERCODE',
        store: store_CUSTOMER,
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        margin: 0,
        minChars: 4,
        forceSelection: true,
        anyMatch: true,
        hideTrigger: true,
        listeners: {
            focus: function (cb) {
                cb.clearInvalid();
            },
            select: function (records) {
                field_CUSTOMERNAME.setValue(records.rawValue.substr(0, records.rawValue.lastIndexOf('(')));
            }
        },
        flex: .85,
        allowBlank: false,
        blankText: '报关行不能为空!',
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    var field_CUSTOMERNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'CUSTOMERNAME'
    })


    //报关行
    var field_CUSTOMER = {
        xtype: 'fieldcontainer',
        fieldLabel: '报关行',
        layout: 'hbox',
        items: [combo_CUSTOMER, {
            id: 'CUSTOMER_btn', xtype: 'button', handler: function () {
                selectjydw(combo_CUSTOMER, field_CUSTOMERNAME);
            },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }

    //项号
    var field_ITEMNO = Ext.create('Ext.form.field.Text', {
        id: 'ITEMNO',
        name: 'ITEMNO',
        fieldLabel: '项号'
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
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand();
                }
            },
            change: function (field_paste, newValue, oldValue, eOpts) {
                if (newValue == "成品") {
                    $("#div_form_con").show();
                    if (!Ext.getCmp('formpanel_con')) {
                        form_ini_con();
                    }                       
                } else {
                    $("#div_form_con").hide();
                }
            },
            load: function () {
                alert(1);
            }
        },
        allowBlank: false,
        blankText: '项号属性不能为空!'
    });
    
    //HS编码
    var field_HSCODE = Ext.create('Ext.form.field.Text', {
        id: 'HSCODE',
        name: 'HSCODE',
        flex: 0.80, margin: 0
    });

    //附加码
    var field_ADDITIONALNO = Ext.create('Ext.form.field.Text', {
        id: 'ADDITIONALNO',
        name: 'ADDITIONALNO',
        flex: 0.20, margin: 0
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
        fieldLabel: '商品名称'
    });

    //规格型号
    var field_SPECIFICATIONSMODEL = Ext.create('Ext.form.field.Text', {
        id: 'SPECIFICATIONSMODEL',
        name: 'SPECIFICATIONSMODEL',
        fieldLabel: '规格型号'
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
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand();
                }
            }
        },
        listConfig: {
            maxHeight: 130
        },
        allowBlank: false,
        blankText: '成交单位不能为空!'
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
        flex: .33,
        value:'A',
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
        data:  [{ "CODE": 0, "NAME": "未打印" }, { "CODE": 1, "NAME": "已打印" }]
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


    //-----------------------------

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
    var field_CREATEMAN = Ext.create('Ext.form.field.Text', {
        id: 'CREATEMAN',
        name: 'CREATEMAN',
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
    var field_SUBMITMAN = Ext.create('Ext.form.field.Text', {
        id: 'SUBMITMAN',
        name: 'SUBMITMAN',
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
    var field_ACCEPTMAN = Ext.create('Ext.form.field.Text', {
        id: 'ACCEPTMAN',
        name: 'ACCEPTMAN',
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
    var field_PREMAN = Ext.create('Ext.form.field.Text', {
        id: 'PREMAN',
        name: 'PREMAN',
        fieldLabel: '预录人',
        readOnly: true,
        fieldStyle: 'background-color: #CECECE; background-image: none;'

    });

    var configItem = [
        { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_RECORDINFOID, combo_CUSTOMAREA, field_CUSTOMER] },
        { layout: 'column', height: 42, border: 0, items: [field_ITEMNO, combo_ITEMNOATTRIBUTE, hs_container, combo_UNIT] },
        { layout: 'column', height: 42, border: 0, items: [field_COMMODITYNAME, field_SPECIFICATIONSMODEL, combo_container] },
        //{ layout: 'column', border: 0, items: [test] },
        { layout: 'column', height: 42, border: 0, items: [textarea_container] },
        { layout: 'column', height: 42, border: 0, items: [field_CREATEDATE, field_CREATEMAN, field_SUBMITTIME, field_SUBMITMAN] },
        { layout: 'column', height: 42, border: 0, items: [field_ACCEPTTIME, field_ACCEPTMAN, field_PRETIME, field_PREMAN] },        
        field_CUSTOMERNAME
    ];
    
    formpanel = Ext.create('Ext.form.Panel', {
        id:'formpanel_id',
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

function form_ini_con() {
    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;成品单耗信息</span></h4>'
    }
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo, '->']
    })


    //对应料件序号
    var field_ITEMNO_CONSUME = Ext.create('Ext.form.field.Text', {
        id: 'ITEMNO_CONSUME',
        name: 'ITEMNO_CONSUME',
        fieldLabel: '对应料件序号', flex: .85, margin: 0
    });

    //报关行
    var field_ITEMNO_LJ = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [field_ITEMNO_CONSUME,
            {
                id: 'ITEMNO_CONSUME_btn', xtype: 'button', handler: function () {
                    //selectjydw(combo_CUSTOMER, field_CUSTOMERNAME);
                },
                text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
            }
        ]
    }

    //对应料件名称
    var field_ITEMNO_CONSUME_NAME = Ext.create('Ext.form.field.Text', {
        id: 'ITEMNO_CONSUME_NAME',
        name: 'ITEMNO_CONSUME_NAME',
        fieldLabel: '对应名称'
    });

    //对应料件规格
    var field_ITEMNO_CONSUME_SPEC = Ext.create('Ext.form.field.Text', {
        id: 'ITEMNO_CONSUME_SPEC',
        name: 'ITEMNO_CONSUME_SPEC',
        fieldLabel: '对应规格'
    });

    //对应料件计量单位
    var field_ITEMNO_CONSUME_UNIT = Ext.create('Ext.form.field.Text', {
        id: 'ITEMNO_CONSUME_UNIT',
        name: 'ITEMNO_CONSUME_UNIT',
        fieldLabel: '对应计量单位'
    });

    //单耗
    var field_CONSUME = Ext.create('Ext.form.field.Text', {
        id: 'CONSUME',
        name: 'CONSUME',
        fieldLabel: '单耗'
    });

    //损耗率
    var field_ATTRITIONRATE = Ext.create('Ext.form.field.Text', {
        id: 'ATTRITIONRATE',
        name: 'ATTRITIONRATE',
        fieldLabel: '损耗率'
    });

    var formpanel_con = Ext.create('Ext.form.Panel', {
        id:'formpanel_con',
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
            { layout: 'column', height: 42, border: 0, items: [field_ITEMNO_LJ, field_ITEMNO_CONSUME_NAME, field_ITEMNO_CONSUME_SPEC, field_ITEMNO_CONSUME_UNIT] },
            { layout: 'column', height: 42, border: 0, items: [field_CONSUME, field_ATTRITIONRATE] }
        ]
    });
    //=================================================
    var data_PRODUCTCONSUME= [];
    var store_PRODUCTCONSUME = Ext.create('Ext.data.JsonStore', {
        fields: ['ITEMNO_CONSUME', 'CONSUME', 'ATTRITIONRATE'],
        data: data_PRODUCTCONSUME
    });
    var w_tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: ['<span style="color:red">说明：双击列表项可对已添加的记录进行修改</span>',
               '->',
               {
                   text: '<span class="icon iconfont" style="font-size:10px">&#xe622;</span>&nbsp;保 存',
                   handler: function () {
                       //var recs = w_grid.getSelectionModel().getSelection();
                       //if (recs.length > 0) {
                       //    w_gridstore.remove(recs);
                       //}
                       //Ext.getCmp('btn_mode').setText('<span style="color:blue">新增模式</span>');
                       //rownum = -1;
                   }
               },
               {
                   text: '<span class="icon iconfont" style="font-size:10px">&#xe6d3;</span>&nbsp;删 除',
                   handler: function () {
                       //var recs = w_grid.getSelectionModel().getSelection();
                       //if (recs.length > 0) {
                       //    w_gridstore.remove(recs);
                       //}
                       //Ext.getCmp('btn_mode').setText('<span style="color:blue">新增模式</span>');
                       //rownum = -1;
                   }
               }]

    });
    var gridpanel_PRODUCTCONSUME = Ext.create('Ext.grid.Panel', {
        id: 'gridpanel_PRODUCTCONSUME',
        renderTo: 'div_form_con',
        store: store_PRODUCTCONSUME,
        height: 150,
        selModel: { selType: 'checkboxmodel' },
        enableColumnHide: false,
        tbar:w_tbar,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: 'ID', dataIndex: 'ID', hidden: true },
        { header: '对应料件序号', dataIndex: 'ITEMNO_CONSUME', width: 80 },
        { header: '对应料件名称', dataIndex: '', width: 130 },
        { header: '对应料件规格', dataIndex: '', width: 130 },
        { header: '对应料件计量单位', dataIndex: '', width: 80 },
        { header: '单耗', dataIndex: 'CONSUME', width: 80 },
        { header: '损耗率', dataIndex: 'ATTRITIONRATE', width: 80 }
        ],
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });

}


function form_ini_btn() {

    var bbar_r = '<div class="btn-group" role="group">'
                        + '<button type="button" onclick="Element_ini()" id="btn_cancelsubmit" class="btn btn-primary btn-sm"><i class="fa fa-angle-double-left"></i>&nbsp;撤回</button>'
                        + '<button type="button" onclick="" class="btn btn-primary btn-sm"><i class="fa fa-floppy-o"></i>&nbsp;保存</button>'
                        + '<button type="button" onclick="" id="btn_submitorder" class="btn btn-primary btn-sm"><i class="fa fa-hand-o-up"></i>&nbsp;提交申请</button></div>'

    var bbar_l = '<div class="btn-group">'
               + '<button type="button" onclick="" class="btn btn-primary btn-sm"><i class="fa fa-print"></i>&nbsp;申请表打印</button>'
           + '</div>';
    var bbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [bbar_l, '->', bbar_r]
    })

    formpanel = Ext.create('Ext.form.Panel', {
        renderTo: 'div_form_btn',
        border: 0,
        bbar: bbar
    });
}


function Element_ini() {
    var customarea = Ext.getCmp('combo_CUSTOMAREA').getValue();
    var hscode = Ext.getCmp('HSCODE').getValue() + Ext.getCmp('ADDITIONALNO').getValue();


    //==================================================================add field===================================================
    var label_busiinfo = {
        columnWidth: 1,
        xtype: 'label',
        margin: '0 0 5 5',
        html: '<div style="border-bottom-width: 2px; border-bottom-color: gray; border-bottom-style: dashed;padding-left:20px;"><h5><b>申报要素</b></h5></div>'
    }

    var field_PREMAN2 = Ext.create('Ext.form.field.Text', {
        id: 'PREMAN2',
        name: 'PREMAN2',
        fieldLabel: '1.品名',

    });

    var field_PREMAN3 = Ext.create('Ext.form.field.Text', {
        id: 'PREMAN3',
        name: 'PREMAN3',
        fieldLabel: '2.型号',

    });

    var field_PREMAN4 = Ext.create('Ext.form.field.Text', {
        id: 'PREMAN4',
        name: 'PREMAN4',
        fieldLabel: '3.描述',

    });

    var field_PREMAN5 = Ext.create('Ext.form.field.Text', {
        id: 'PREMAN5',
        name: 'PREMAN5',
        fieldLabel: '4.特殊',

    });

    var label_busiinfo_end = {
        columnWidth: 1,
        xtype: 'label',
        margin: '0 0 5 5',
        html: '<div style="border-bottom-width: 2px; border-bottom-color: gray; border-bottom-style: dashed;"></div>'
    }

    var configItem = [
    //{ layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_RECORDINFOID, combo_CUSTOMAREA, field_CUSTOMER] },
    //{ layout: 'column', height: 42, border: 0, items: [field_ITEMNO, combo_ITEMNOATTRIBUTE, hs_container, combo_UNIT] },
    //{ layout: 'column', height: 42, border: 0, items: [field_COMMODITYNAME, field_SPECIFICATIONSMODEL, combo_container] },
    { layout: 'column', height: 42, margin: '0 0 15 0', border: 0, items: [label_busiinfo] },//1
    { layout: 'column', height: 42, border: 0, items: [field_PREMAN2, field_PREMAN3, field_PREMAN4, field_PREMAN5] },//2
    { layout: 'column', height: 20, border: 0, items: [label_busiinfo_end] }//3,
    //{ layout: 'column', height: 42, border: 0, items: [textarea_container] },
    //{ layout: 'column', height: 42, border: 0, items: [field_CREATEDATE, field_CREATEMAN, field_SUBMITTIME, field_SUBMITMAN] },
    //{ layout: 'column', height: 42, border: 0, items: [field_ACCEPTTIME, field_ACCEPTMAN, field_PRETIME, field_PREMAN] },
    //field_CUSTOMERNAME
    ];

    var test = {
        id:'test',
        columnWidth: 1,
        xtype: 'fieldcontainer',
        items: configItem
    }

    //==============================================================================================================================



    //Ext.getCmp('test').add(configItem);
    Ext.getCmp('formpanel_id').items.insert(3, [field_PREMAN4,field_PREMAN5]);
    Ext.getCmp('formpanel_id').doLayout();
}
