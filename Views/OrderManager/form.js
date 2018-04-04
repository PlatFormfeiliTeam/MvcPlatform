function form_ini() {
    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;基础信息</span></h4>'
    }
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo]
    });

    //订单编号
    var field_CODE = Ext.create('Ext.form.field.Text', {
        id: 'field_CODE',
        name: 'CODE',
        fieldLabel: '订单编号',
        readOnly: true,
        emptyText: '订单号自动生成'
    });
    
    //企业编号
    var field_CUSNO = Ext.create('Ext.form.field.Text', {
        id: 'CUSNO',
        name: 'CUSNO',//tabIndex: 5,        
        fieldLabel: '企业编号'
    });

    //业务类型
    var store_busitype = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME', 'CODENAME'],
        data: common_data_busi
    });
    var combo_BUSITYPE = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_BUSITYPE',
        name: 'BUSITYPE',
        store: store_busitype,
        fieldLabel: '业务类型',//tabIndex: 3
        displayField: 'CODENAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                };
            },
            change: function (combo, newValue, oldValue, eOpts) {
                combo_ENTRUSTTYPE.reset();
                Ext.Ajax.request({
                    url: "/OrderManager/Ini_Base_Data_BUSIITEM",
                    params: { busitype: newValue },
                    success: function (response, opts) {
                        var commondata = Ext.decode(response.responseText);//业务细项
                        store_ENTRUSTTYPE.loadData(commondata.ywxx);

                        var entrusttype = Ext.getCmp("combo_ENTRUSTTYPE").getValue();
                        var rec = store_ENTRUSTTYPE.findRecord('CODE', entrusttype);
                        if (!rec) {
                            combo_ENTRUSTTYPE.setValue("");//编辑页赋值
                        } else {
                            combo_ENTRUSTTYPE.setValue(entrusttype);//编辑页赋值
                        }
                        change_ini_label();
                    }
                });
            }
        },
        allowBlank: false,
        blankText: '业务类型不能为空!'
    });

    //业务细项
    var store_ENTRUSTTYPE = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME']//,data: common_data_ywxx
    });

    var combo_ENTRUSTTYPE = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ENTRUSTTYPE',
        name: 'ENTRUSTTYPE',
        store: store_ENTRUSTTYPE,
        fieldLabel: '业务细项',//tabIndex: 3
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                };
            },
            change: function (combo, newValue, oldValue, eOpts) {
                change_ini_label();
            }
        },
        allowBlank: false,
        blankText: '业务细项不能为空!'
    });

    var field_CUSTOMERNAME = Ext.create('Ext.form.field.Hidden', { id: 'field_CUSTOMERNAME', name: 'CUSTOMERNAME' });
    var field_BUSIUNITNAME = Ext.create('Ext.form.field.Hidden', { id: 'field_BUSIUNITNAME', name: 'BUSIUNITNAME' });
    var field_CLEARUNITNAME = Ext.create('Ext.form.field.Hidden', { id: 'field_CLEARUNITNAME', name: 'CLEARUNITNAME' });

    //委托单位
    var store_wtdw = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_wtdw
    })
    var combo_wtdw = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_wtdw',
        name: 'CUSTOMERCODE',
        store: store_wtdw,
        hideTrigger: true,
        fieldLabel: '委托单位', forceSelection: true,
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local', tabIndex: 2,
        anyMatch: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand();
                }
            },
            change: function (combo, newValue, oldValue, eOpts) {
                if (Ext.getCmp('combo_jsdw').getValue() == "" || Ext.getCmp('combo_jsdw').getValue() == null) {
                    Ext.getCmp('combo_jsdw').setValue(newValue);
                }
                field_CUSTOMERNAME.setValue(combo.rawValue);
            }
        },
        allowBlank: false,
        blankText: '委托单位不能为空!'
    });

    var store_jydw = Ext.create('Ext.data.JsonStore', {  //经营单位combostore
        fields: ['CODE', 'NAME'],
        data: common_data_jydw
    });
    var combo_jydw = Ext.create('Ext.form.field.ComboBox', {//经营单位 这个数据比较多需要根据输入字符到后台动态模糊匹配,如果取不到点击添加按钮从总库进行选择，同时添加到自有客户库
        id: 'combo_jydw',
        name: 'BUSIUNITCODE',
        store: store_jydw,
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        margin: 0,
        minChars: 4,
        forceSelection: true,
        tabIndex: 3,
        anyMatch: true,
        hideTrigger: true,
        listeners: {
            focus: function (cb) {
                cb.clearInvalid();
            },
            select: function (records) {
                field_BUSIUNITNAME.setValue(records.rawValue.substr(0, records.rawValue.lastIndexOf('(')));
            }
        },
        flex: .85,
        allowBlank: false,
        blankText: '经营单位不能为空!',
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    

    //经营单位
    var field_jydw = {
        xtype: 'fieldcontainer',
        fieldLabel: '经营单位', columnWidth: .5,
        layout: 'hbox',
        items: [combo_jydw, {
            id: 'jydw_btn', xtype: 'button', handler: function () {
                selectjydw(combo_jydw, field_BUSIUNITNAME);
            },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .05, margin: 0
        }]
    }

    //结算单位
    var store_jsdw = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_wtdw
    })
    var combo_jsdw = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_jsdw',
        name: 'CLEARUNIT',
        store: store_jsdw,
        hideTrigger: true,
        fieldLabel: '结算单位', forceSelection: true, flex: .95,
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all', tabIndex: 4,
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
            change: function (combo, newValue, oldValue, eOpts) {
                field_CLEARUNITNAME.setValue(combo.rawValue);
            }
        },
        allowBlank: false,
        blankText: '结算单位不能为空!'
    });

    
    //创建人员
    var field_CREATEUSERNAME = Ext.create('Ext.form.field.Text', {
        name: 'CREATEUSERNAME',
        fieldLabel: '创建人员',
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //创建时间
    var field_CREATETIME = Ext.create('Ext.form.field.Text', {
        id: 'field_CREATETIME',
        name: 'CREATETIME',
        fieldLabel: '创建时间',
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    //业务完成人员
    var field_SUBMITUSERNAME = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITUSERNAME',
        fieldLabel: '完成人员',
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //业务完成时间
    var field_SUBMITTIME = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITTIME',
        fieldLabel: '完成时间',
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    //操作需求
    var field_DOREQUEST = Ext.create('Ext.form.field.Text', {
        id: 'field_DOREQUEST',
        flex: 1, margin: 0, tabIndex: 6,
        name: 'DOREQUEST'
    });
    var container_DOREQUEST = {
        columnWidth: 1,
        xtype: 'fieldcontainer',
        layout: 'hbox',
        fieldLabel: '操作需求',
        items: [field_DOREQUEST]
    }

    //结算备注
    var field_CLEARREMARK = Ext.create('Ext.form.field.Text', {
        id: 'field_CLEARREMARK',
        flex: 1, margin: 0, tabIndex: 7,
        name: 'CLEARREMARK'
    });
    var container_CLEARREMARK = {
        columnWidth: 1,
        xtype: 'fieldcontainer',
        layout: 'hbox',
        fieldLabel: '结算备注',
        items: [field_CLEARREMARK]
    }

    var label_goinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;可配信息</span></h4>'
    }

    var field_TEXTONE = Ext.create('Ext.form.field.Text', {
        id: 'field_TEXTONE',
        name: 'TEXTONE',//tabIndex: 5,        
        fieldLabel: '文本1'
    });
    var field_TEXTTWO = Ext.create('Ext.form.field.Text', {
        id: 'field_TEXTTWO',
        name: 'TEXTTWO',//tabIndex: 5,        
        fieldLabel: '文本2'
    });

    var field_NUMONE = Ext.create('Ext.form.field.Number', {
        id: 'field_NUMONE',
        name: 'NUMONE', hideTrigger: true,//tabIndex: 5,        
        fieldLabel: '数字1'
    });
    var field_NUMTWO = Ext.create('Ext.form.field.Number', {
        id: 'field_NUMTWO',
        name: 'NUMTWO', hideTrigger: true,//tabIndex: 5,        
        fieldLabel: '数字2'
    });

    //日期1
    var field_DATEONE = Ext.create('Ext.form.field.Date', {
        id: 'field_DATEONE',
        name: 'DATEONE', format: 'Y-m-d',
        fieldLabel: '日期1',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue != "") {
                    field_USERNAMEONE.setValue(curuserRealname);
                    field_USERIDONE.setValue(curuserId);
                } else {
                    field_USERNAMEONE.setValue("");
                    field_USERIDONE.setValue("");
                }
            }
        }
    });
    //人员1
    var field_USERNAMEONE = Ext.create('Ext.form.field.Text', {
        id: 'field_USERNAMEONE',
        name: 'USERNAMEONE',
        fieldLabel: '人员1',
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    var field_USERIDONE = Ext.create('Ext.form.field.Hidden', { name: 'USERIDONE' });

    //日期2
    var field_DATETWO = Ext.create('Ext.form.field.Date', {
        id: 'field_DATETWO',
        name: 'DATETWO', format: 'Y-m-d',
        fieldLabel: '日期2',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue != "") {
                    field_USERNAMETWO.setValue(curuserRealname);
                    field_USERIDTWO.setValue(curuserId);
                } else {
                    field_USERNAMETWO.setValue("");
                    field_USERIDTWO.setValue("");
                }
            }
        }
    });
    //人员2
    var field_USERNAMETWO = Ext.create('Ext.form.field.Text', {
        id: 'field_USERNAMETWO',
        name: 'USERNAMETWO',
        fieldLabel: '人员2',
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    var field_USERIDTWO = Ext.create('Ext.form.field.Hidden', { name: 'USERIDTWO' });


    formpanel = Ext.create('Ext.form.Panel', {
        id: "formpanel_id",
        renderTo: 'div_form',
        minHeight: 150,
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
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_CODE, field_CUSNO, combo_BUSITYPE, combo_ENTRUSTTYPE] },
            { layout: 'column', height: 42, border: 0, items: [field_jydw, combo_wtdw, combo_jsdw] },
            { layout: 'column', height: 42, border: 0, items: [field_CREATEUSERNAME, field_CREATETIME, field_SUBMITUSERNAME, field_SUBMITTIME] },
            { layout: 'column', height: 42, border: 0, items: [container_DOREQUEST] },
            { layout: 'column', height: 42, border: 0, items: [container_CLEARREMARK] },
            { layout: 'column', height: 42, margin: '0 0 15 0', border: 0, items: [label_goinfo] },
            { layout: 'column', height: 42, border: 0, items: [field_TEXTONE, field_NUMONE, field_DATEONE, field_USERNAMEONE] },
            { layout: 'column', height: 42, border: 0, items: [field_TEXTTWO, field_NUMTWO, field_DATETWO, field_USERNAMETWO] },            
            field_BUSIUNITNAME, field_CUSTOMERNAME, field_CLEARUNITNAME, field_USERIDONE, field_USERIDTWO
        ]
    });
}

function change_ini_label() {
    Ext.getCmp("field_TEXTONE").setFieldLabel("文本1"); Ext.getCmp("field_TEXTTWO").setFieldLabel("文本2");
    Ext.getCmp("field_NUMONE").setFieldLabel("数字1"); Ext.getCmp("field_NUMTWO").setFieldLabel("数字2");
    Ext.getCmp("field_DATEONE").setFieldLabel("日期1"); Ext.getCmp("field_DATETWO").setFieldLabel("日期2");
    Ext.getCmp("field_USERNAMEONE").setFieldLabel("人员1"); Ext.getCmp("field_USERNAMETWO").setFieldLabel("人员2");

    var f_busitype = Ext.getCmp("combo_BUSITYPE").getValue();
    var f_entrusttype = Ext.getCmp("combo_ENTRUSTTYPE").getValue();

    Ext.Ajax.request({
        url: "/OrderManager/Getlabelname",
        params: { busitype: f_busitype, entrusttype: f_entrusttype },
        success: function (response, opts) {
            var json = Ext.decode(response.responseText);
            var jsonobj = json.fieldcolumn;

            for (var i = 0; i < jsonobj.length; i++) {
                switch (jsonobj[i].ORIGINNAME) {
                    case "文本1":
                        Ext.getCmp("field_TEXTONE").setFieldLabel(jsonobj[i].CONFIGNAME);
                        break;
                    case "文本2":
                        Ext.getCmp("field_TEXTTWO").setFieldLabel(jsonobj[i].CONFIGNAME);
                        break;
                    case "数字1":
                        Ext.getCmp("field_NUMONE").setFieldLabel(jsonobj[i].CONFIGNAME);
                        break;
                    case "数字2":
                        Ext.getCmp("field_NUMTWO").setFieldLabel(jsonobj[i].CONFIGNAME);
                        break;
                    case "日期1":
                        Ext.getCmp("field_DATEONE").setFieldLabel(jsonobj[i].CONFIGNAME);
                        break;
                    case "日期2":
                        Ext.getCmp("field_DATETWO").setFieldLabel(jsonobj[i].CONFIGNAME);
                        break;
                    case "人员1":
                        Ext.getCmp("field_USERNAMEONE").setFieldLabel(jsonobj[i].CONFIGNAME);
                        break;
                    case "人员2":
                        Ext.getCmp("field_USERNAMETWO").setFieldLabel(jsonobj[i].CONFIGNAME);
                        break;
                    default:
                        break;
                }
            }
            
        }
    });
}

function form_ini_btn() {

    var bbar_r = '<div class="btn-group" role="group">'
                        + '<button type="button" onclick="Save_OrderM()" id="btn_submitorder" class="btn btn-primary btn-sm"><i class="fa fa-hand-o-up"></i>&nbsp;保存</button>'
            + '</div>';
    var bbar = Ext.create('Ext.toolbar.Toolbar', {
        items: ['->', bbar_r]
    });

    var formpanel_btn = Ext.create('Ext.form.Panel', {
        renderTo: 'div_form_btn',
        border: 0,
        bbar: bbar
    });
}

function Save_OrderM() {
    if (!Ext.getCmp('formpanel_id').getForm().isValid()) {
        return;
    }

    var formdata = Ext.encode(Ext.getCmp('formpanel_id').getForm().getValues());
    
    var mask = new Ext.LoadMask(Ext.get(Ext.getBody()), { msg: "数据保存中，请稍等..." });

    mask.show();
    Ext.Ajax.request({
        url: "/OrderManager/Save_OrderM",
        params: { ordercode: ordercode, formdata: formdata },
        success: function (response, option) {
            if (response.responseText) {
                mask.hide();
                var data = Ext.decode(response.responseText);
                if (data.success) {
                    ordercode = data.ordercode; 
                    Ext.MessageBox.alert("提示", "保存成功！", function () {
                        loadform_OrderM();
                    });
                }
                else {
                    Ext.MessageBox.alert("提示", "保存失败！");
                }
            }
        }
    });
}

function loadform_OrderM() {
    Ext.Ajax.request({
        url: "/OrderManager/loadform_OrderM",
        params: { ordercode: ordercode },
        success: function (response, opts) {
            var data = Ext.decode(response.responseText);

            Ext.getCmp("formpanel_id").getForm().setValues(data.formdata);

            if (data.formdata.SUBMITTIME != "" && data.formdata.SUBMITTIME != null) {
                document.getElementById("btn_submitorder").disabled = true;
            } else {
                document.getElementById("btn_submitorder").disabled = false;
            }

            curuserRealname = data.curuser.REALNAME; curuserId = data.curuser.ID;
        }
    });
}