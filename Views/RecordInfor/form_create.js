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
        fields: ['CODE', 'NAME', 'ID'],
        //data: common_data_recordid
    });
    var combo_CUSTOMAREA = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_CUSTOMAREA',
        name: 'CUSTOMAREA',
        store: store_CUSTOMAREA,
        fieldLabel: '备案关区',
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
        blankText: '备案关区不能为空!'
    });


    //报关行
    var store_CUSTOMER = Ext.create('Ext.data.JsonStore', {  //报关行combostore
        fields: ['CODE', 'NAME'],
        //data: common_data_jydw
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
        fieldLabel: '变动状态',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        queryMode: 'local',
        anyMatch: true,
        readOnly: true,
        flex: .33,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                }
            }
        }
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
        forceSelection: true,
        queryMode: 'local',
        anyMatch: true,
        readOnly: true,
        flex: .33,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                }
            }
        }
    });
    //打印状态
    var store_ISPRINT_APPLY = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data:  [{ "CODE": "0", "NAME": "未打印" }, { "CODE": "1", "NAME": "已打印" }]
    });
    var combo_ISPRINT_APPLY = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ISPRINT_APPLY',
        name: 'ISPRINT_APPLY',
        store: store_ISPRINT_APPLY,
        fieldLabel: '打印状态',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        queryMode: 'local',
        anyMatch: true,
        readOnly: true,
        flex: .33,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                }
            }
        }
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
        fieldLabel: '维护时间'
    });
    //维护人
    var field_CREATEMAN = Ext.create('Ext.form.field.Text', {
        id: 'CREATEMAN',
        name: 'CREATEMAN',
        fieldLabel: '维护人'
    });
    //提交时间
    var field_SUBMITTIME = Ext.create('Ext.form.field.Text', {
        id: 'SUBMITTIME',
        name: 'SUBMITTIME',
        fieldLabel: '提交时间'
    });
    //提交人
    var field_SUBMITMAN = Ext.create('Ext.form.field.Text', {
        id: 'SUBMITMAN',
        name: 'SUBMITMAN',
        fieldLabel: '提交人'
    });
    //受理时间
    var field_ACCEPTTIME = Ext.create('Ext.form.field.Text', {
        id: 'ACCEPTTIME',
        name: 'ACCEPTTIME',
        fieldLabel: '受理时间'
    });
    //受理人
    var field_ACCEPTMAN = Ext.create('Ext.form.field.Text', {
        id: 'ACCEPTMAN',
        name: 'ACCEPTMAN',
        fieldLabel: '受理人'
    });
    //预录时间
    var field_PRETIME = Ext.create('Ext.form.field.Text', {
        id: 'PRETIME',
        name: 'PRETIME',
        fieldLabel: '预录时间'
    });
    //预录人
    var field_PREMAN = Ext.create('Ext.form.field.Text', {
        id: 'PREMAN',
        name: 'PREMAN',
        fieldLabel: '预录人'
    });


    var bbar_r = '<div class="btn-group" role="group">'
                        + '<button type="button" onclick="orderBack();" id="btn_cancelsubmit" class="btn btn-primary btn-sm"><i class="fa fa-angle-double-left"></i>&nbsp;撤单</button>'
                        + '<button type="button" onclick="add_new(11)" class="btn btn-primary btn-sm"><i class="fa fa-plus fa-fw"></i>&nbsp;新增</button>'
                        + '<button type="button" onclick="copyorder(11)" class="btn btn-primary btn-sm"><i class="fa fa-files-o"></i>&nbsp;复制新增</button>'
                        + '<button type="button" onclick="save(\'save\',11)" class="btn btn-primary btn-sm"><i class="fa fa-floppy-o"></i>&nbsp;保存</button>'
                        + '<button type="button" onclick="save(\'submit\',11)" id="btn_submitorder" class="btn btn-primary btn-sm"><i class="fa fa-hand-o-up"></i>&nbsp;提交委托</button></div>'

    var bbar_l = '<div class="btn-group">'
               + '<button type="button" class="btn btn-primary btn-sm" id="pickfiles"><i class="fa fa-upload"></i>&nbsp;上传文件</button>'
               + '<button type="button" onclick="browsefile()" class="btn btn-primary btn-sm"><i class="fa fa-exchange fa-fw"></i>&nbsp;浏览文件</button>'
               + '<button type="button" onclick="removeFile()" class="btn btn-primary btn-sm" id="deletefile"><i class="fa fa-trash-o"></i>&nbsp;删除文件</button>'
               + '<button type="button" onclick="printFile()" class="btn btn-primary btn-sm"><i class="fa fa-print"></i>&nbsp;打印文件</button>'
           + '</div>';
    var bbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [bbar_l, '->', bbar_r]
    })


    formpanel = Ext.create('Ext.form.Panel', {
        renderTo: 'div_form',
        minHeight: 350,
        border: 0,
        tbar: tbar,
        bbar: bbar,
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
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_RECORDINFOID, combo_CUSTOMAREA, field_CUSTOMER] },
            { layout: 'column', height: 42, border: 0, items: [field_ITEMNO, combo_ITEMNOATTRIBUTE, hs_container,combo_UNIT] },
            { layout: 'column', height: 42, border: 0, items: [field_COMMODITYNAME, field_SPECIFICATIONSMODEL, combo_container] },
            { layout: 'column', height: 42, border: 0, items: [textarea_container] },
            { layout: 'column', height: 42, border: 0, items: [field_CREATEDATE, field_CREATEMAN, field_SUBMITTIME, field_SUBMITMAN] },
            { layout: 'column', height: 42, border: 0, items: [field_ACCEPTTIME, field_ACCEPTMAN, field_PRETIME, field_PREMAN] },

        /*{ layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_CODE, combo_ENTRUSTTYPENAME, combo_REPWAYNAME, combo_CUSTOMDISTRICTNAME, cont_bgsbdw] },
        { layout: 'column', height: 42, border: 0, items: [combo_DECLWAY, field_SUBMITUSERNAME, field_SUBMITTIME, field_STATUS, cont_bjsbdw] },
        { layout: 'column', height: 42, border: 0, items: [field_CREATEUSERNAME, field_CREATETIME, combo_dzfwdw] },
        { layout: 'column', border: 42, border: 0, items: [label_busiinfo, chk_container] },
        { layout: 'column', height: 42, border: 0, items: [field_CUSNO, combo_PORTCODE, field_jydw, field_TOTALNO, field_DIVIDENO] },
        { layout: 'column', height: 42, border: 0, items: [field_quanpackage, field_weight, field_contractno, field_myfs, field_TURNPRENO] },
        { layout: 'column', height: 42, border: 0, items: [combo_mzbz, field_CLEARANCENO, container_bgch, field_ORDERREQUEST, chk_CHKLAWCONDITION] },*/
        field_CUSTOMERNAME
        ]
    });
}