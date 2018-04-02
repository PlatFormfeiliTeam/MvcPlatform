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
                        form_ini_con();
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
                form_ini_con();
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

    var field_ORIGINALCOSTIDS = Ext.create('Ext.form.field.Hidden', { id: 'field_ORIGINALCOSTIDS', name: 'ORIGINALCOSTIDS' });


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
            field_BUSIUNITNAME, field_CUSTOMERNAME, field_CLEARUNITNAME, field_ORIGINALCOSTIDS
        ]
    });
}

function form_ini_con() {

    if (Ext.getCmp("formpanel_con")) {
        Ext.getCmp("formpanel_con").destroy();
    }

    var f_busitype = Ext.getCmp("combo_BUSITYPE").getValue();
    var f_entrusttype = Ext.getCmp("combo_ENTRUSTTYPE").getValue();

    var configItem = new Array();

    Ext.Ajax.request({
        url: "/OrderManager/Getele",
        params: { pagename: pagename, configtype: configtype, busitype: f_busitype, entrusttype: f_entrusttype, ordercode: ordercode },
        success: function (response, opts) {
            var json = Ext.decode(response.responseText);

            if (Ext.getCmp("formpanel_con")) {
                Ext.getCmp("formpanel_con").destroy();
            }

            if (json.fieldcolumn.length > 0) {

                var items_i = [];
                for (var i = 0; i < json.fieldcolumn.length; i++) {

                    switch (json.fieldcolumn[i].CONTROLTYPE) {
                        case "文本":
                            items_i.push(Ext.create('Ext.form.field.Text', { id: json.fieldcolumn[i].TABLECODE + '|' + json.fieldcolumn[i].FIELDCODE + '|' + 'text', name: json.fieldcolumn[i].TABLECODE + '|' + json.fieldcolumn[i].FIELDCODE + '|' + 'text', fieldLabel: json.fieldcolumn[i].NAME }));
                            break;
                        case "数字":
                            items_i.push(Ext.create('Ext.form.field.Number', { id: json.fieldcolumn[i].TABLECODE + '|' + json.fieldcolumn[i].FIELDCODE + '|' + 'number', name: json.fieldcolumn[i].TABLECODE + '|' + json.fieldcolumn[i].FIELDCODE + '|' + 'number', fieldLabel: json.fieldcolumn[i].NAME, hideTrigger: true }));
                            break;
                        case "日期":
                            items_i.push(Ext.create('Ext.form.field.Date', { id: json.fieldcolumn[i].TABLECODE + '|' + json.fieldcolumn[i].FIELDCODE + '|' + 'date', name: json.fieldcolumn[i].TABLECODE + '|' + json.fieldcolumn[i].FIELDCODE + '|' + 'date', fieldLabel: json.fieldcolumn[i].NAME, format: 'Y-m-d' }));
                            break;
                        case "下拉框":
                            if (json.fieldcolumn[i].SELECTCONTENT == null) {
                                items_i.push(Ext.create('Ext.form.field.Text', { id: json.fieldcolumn[i].TABLECODE + '|' + json.fieldcolumn[i].FIELDCODE + '|' + 'combox', name: json.fieldcolumn[i].TABLECODE + '|' + json.fieldcolumn[i].FIELDCODE + '|' + 'combox', fieldLabel: json.fieldcolumn[i].NAME }));

                            } else if (json.fieldcolumn[i].SELECTCONTENT.length <= 0) {
                                items_i.push(Ext.create('Ext.form.field.Text', { id: json.fieldcolumn[i].TABLECODE + '|' + json.fieldcolumn[i].FIELDCODE + '|' + 'combox', name: json.fieldcolumn[i].TABLECODE + '|' + json.fieldcolumn[i].FIELDCODE + '|' + 'combox', fieldLabel: json.fieldcolumn[i].NAME }));

                            } else {
                                var store_ss = Ext.create('Ext.data.JsonStore', { fields: ['NAME'] });
                                var arraylen = json.fieldcolumn[i].SELECTCONTENT.split(";");
                                for (var ai = 0; ai < arraylen.length ; ai++) {
                                    if (arraylen[ai]) {
                                        store_ss.insert(store_ss.data.length, { 'NAME': arraylen[ai] });
                                    }
                                }

                                items_i.push(Ext.create('Ext.form.field.ComboBox', {
                                    id: json.fieldcolumn[i].TABLECODE + '|' + json.fieldcolumn[i].FIELDCODE + '|' + 'combox', name: json.fieldcolumn[i].TABLECODE + '|' + json.fieldcolumn[i].FIELDCODE + '|' + 'combox', fieldLabel: json.fieldcolumn[i].NAME,
                                    store: store_ss,
                                    displayField: 'NAME', valueField: 'NAME', triggerAction: 'all', queryMode: 'local', hideTrigger: true, anyMatch: true,
                                    listeners: {
                                        focus: function (cb) {
                                            if (!cb.getValue()) {
                                                cb.clearInvalid();
                                                cb.store.clearFilter();
                                                cb.expand()
                                            };
                                        }
                                    }
                                }));
                            }
                            break;
                        default:
                            items_i.push(Ext.create('Ext.form.field.Text', { id: json.fieldcolumn[i].TABLECODE + '|' + json.fieldcolumn[i].FIELDCODE + '|' + 'text', name: json.fieldcolumn[i].TABLECODE + '|' + json.fieldcolumn[i].FIELDCODE + '|' + 'text', fieldLabel: json.fieldcolumn[i].NAME }));
                            break;

                    }
                    if (i == json.fieldcolumn.length - 1 || i % 4 == 3) {
                        configItem.push({ layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: items_i });
                        items_i = [];
                    }
                }

                if (configItem.length <= 0) { return; }

                var label_baseinfo = {
                    xtype: 'label',
                    margin: '5',
                    html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;可配信息</span></h4>'
                }
                var tbar = Ext.create('Ext.toolbar.Toolbar', {
                    items: [label_baseinfo, '->']
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
                    items: configItem
                });

                Ext.getCmp("formpanel_con").getForm().setValues(json.fieldcolumn_con);
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
    if (Ext.getCmp("formpanel_con")) {
        if (!Ext.getCmp('formpanel_con').getForm().isValid()) {
            return;
        }
    }

    var formdata = Ext.getCmp('formpanel_id').getForm().getValues();

    var formdata_con = "{}";
    if (Ext.getCmp("formpanel_con")) {
        formdata_con = Ext.encode(Ext.getCmp('formpanel_con').getForm().getValues());
    }

    var mask = new Ext.LoadMask(Ext.get(Ext.getBody()), { msg: "数据保存中，请稍等..." });

    mask.show();
    Ext.Ajax.request({
        url: "/OrderManager/Save_OrderM",
        params: { ordercode: ordercode, formdata_str: Ext.encode(formdata), formdata_con: formdata_con },
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
            //formpanelcontrol();//表单字段控制
        }
    });
}