function form2_export_ini() {
    var label_out4 = {
        xtype: 'label',
        columnWidth: .70,
        margin: '0 0 5 5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;出口信息</span></h4>'
    }
    var relation_confirm_chk4 = Ext.create('Ext.form.field.Checkbox', {
        fieldLabel: '特殊关系确认',
        name: 'SPECIALRELATIONSHIP',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue) {
                    price_confirm_chk4.setReadOnly(false);
                }
                else {
                    price_confirm_chk4.setValue(false);
                    price_confirm_chk4.setReadOnly(true);
                }
            }
        }
    });
    var price_confirm_chk4 = Ext.create('Ext.form.field.Checkbox', {
        fieldLabel: '价格影响确认',
        name: 'PRICEIMPACT',
        readOnly: true
    });
    var fee_confirm_chk4 = Ext.create('Ext.form.field.Checkbox', {
        labelWidth: 125,
        fieldLabel: '支付特许权使用费确认',
        name: 'PAYPOYALTIES'
    });
    var chk_container4 = {
        columnWidth: .30,
        border: 2,
        height: 25,
        style: {
            borderColor: '#e9b477',
            borderStyle: 'solid'
        },
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [relation_confirm_chk4, price_confirm_chk4, fee_confirm_chk4]
    }
    //------------------------------------------------订单编号，委托类型，客户编号，经营单位，报关方式-----------------------------------------------
    var field_CODE4 = Ext.create('Ext.form.field.Text', {//订单编号
        id: 'code4',
        name: 'CODE',
        fieldLabel: '订单编号',
        emptyText: '订单号自动生成',
        readOnly: true
    });
    //委托类型
    var store_ENTRUSTTYPENAME4 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: wtlx_js_data
    })
    var combo_ENTRUSTTYPENAME4 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ENTRUSTTYPENAME4',
        name: 'ENTRUSTTYPE',
        store: store_ENTRUSTTYPENAME4,
        fieldLabel: '委托类型',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        anyMatch: true,
        forceSelection: true,
        hideTrigger: true,
        triggerAction: 'all',
        tabIndex: 61,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.expand();
                }
            },
            select: function (cb, records) {
                bg_bj_sbdw_control(cb, 4);
            }
        },
        allowBlank: false,
        blankText: '委托类型不能为空!'
    })
    //客户编号
    var field_CUSNO4 = Ext.create('Ext.form.field.Text', {
        id: 'field_CUSNO4',
        name: 'CUSNO',
        tabIndex: 62,
        fieldLabel: '客户编号'
    });
    //经营单位
    var store_jydw4 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_jydw
    })
    var combo_jydw4 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_jydw4',
        name: 'BUSIUNITCODE',
        store: store_jydw4,
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        margin: 0,
        forceSelection: true,
        tabIndex: 63,
        anyMatch: true,
        minChars: 4,
        hideTrigger: true,
        listeners: {
            select: function (records) {
                field_BUSIUNITNAME4.setValue(records.rawValue.substr(0, records.rawValue.lastIndexOf('(')));
            },
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                }
            }
        },
        flex: .85,
        allowBlank: false,
        blankText: '经营单位不能为空!',
        listConfig: {
            maxHeight: 150
        }
    })
    var field_BUSIUNITNAME4 = Ext.create('Ext.form.field.Hidden', {
        name: 'BUSIUNITNAME'
    })
    //经营单位
    var field_jydw4 = {
        xtype: 'fieldcontainer',
        fieldLabel: '经营单位',
        layout: 'hbox',
        items: [combo_jydw4, {
            xtype: 'button', id: 'jydw_btn4', handler: function () {
                selectjydw(combo_jydw4, field_BUSIUNITNAME4);
            }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    //报关方式
    var store_DECLWAY4 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_bgfs
    })
    var combo_DECLWAY4 = Ext.create('Ext.form.field.ComboBox', {
        name: 'DECLWAY',
        hideTrigger: true,
        store: store_DECLWAY4,
        fieldLabel: '报关方式',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 64,
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
        blankText: '报关方式不能为空!'
    })
    //-------------------------------------------------------件数/包装，毛重/净重，合同号，贸易方式，账册备案号--------------------------------------
    //件数/包装，包装combostore
    var store_PACKKINDNAME4 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_bzzl
    })
    var combo_PACKKINDNAME4 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_PACKKINDNAME4',
        name: 'PACKKIND',
        hideTrigger: true,
        store: store_PACKKINDNAME4,
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        margin: 0,
        listConfig: {
            maxHeight: 110
        },
        forceSelection: true,
        tabIndex: 66,
        queryMode: 'local',
        anyMatch: true,
        listeners: {
            select: function (cb_pack, records, eOpts) {
                if (Ext.getCmp("combo_PACKKINDNAME3")) {
                    Ext.getCmp("combo_PACKKINDNAME3").setValue(records[0].get("CODE"));
                }
            },
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand();
                }
            }
        },
        flex: .5
    })
    //件数/包装
    var field_quanpackage4 = {
        xtype: 'fieldcontainer',
        fieldLabel: '件数/包装',
        layout: 'hbox',
        items: [{
            id: 'GOODSNUM4', name: 'GOODSNUM', xtype: 'numberfield', flex: .5, margin: 0, hideTrigger: true, anyMatch: true, tabIndex: 65,
            spinUpEnabled: false, spinDownEnabled: false,allowBlank: false, blankText: '不能为空!',
            listeners: {
                change: function (nf, newValue, oldValue, eOpts) {
                    if (Ext.getCmp("GOODSNUM3")) {
                        Ext.getCmp("GOODSNUM3").setValue(newValue);
                    }
                },
                focus: function (nf) {
                    nf.clearInvalid();
                }
            }
        }, combo_PACKKINDNAME4]
    }
    //毛重/净重
    var field_weight4 = {
        xtype: 'fieldcontainer',
        fieldLabel: '毛重/净重',
        layout: 'hbox',
        items: [
            {
                id: 'GOODSGW4', name: 'GOODSGW', xtype: 'numberfield', flex: .5, margin: 0, allowBlank: false, blankText: '不能为空!', anyMatch: true, hideTrigger: true, tabIndex: 67, decimalPrecision: 4,
                spinUpEnabled: false, spinDownEnabled: false,
                listeners: {
                    focus: function (nf) {
                        nf.clearInvalid();
                    },
                    change: function (nf, newValue, oldValue, eOpts) {
                        if (Ext.getCmp("GOODSGW3")) {
                            Ext.getCmp("GOODSGW3").setValue(newValue);
                        }
                    }
                }
            },
            {
                id: 'GOODSNW4', name: 'GOODSNW', xtype: 'numberfield', flex: .5, margin: 0, hideTrigger: true, tabIndex: 69, anyMatch: true, decimalPrecision: 4,
                spinUpEnabled: false, spinDownEnabled: false,
                listeners: {
                    change: function (nf, newValue, oldValue, eOpts) {
                        if (Ext.getCmp("GOODSNW3")) {
                            Ext.getCmp("GOODSNW3").setValue(newValue);
                        }
                    }
                }
            }
        ]
    }
    //合同号
    var field_contractno4 = Ext.create('Ext.form.field.Text', {
        id: 'CONTRACTNO4',
        fieldLabel: '合同号',
        tabIndex: 70,
        name: 'CONTRACTNO'
    });
    //贸易方式
    var store_myfs4 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_myfs
    })
    var combo_myfs4 = Ext.create('Ext.form.field.ComboBox', {//贸易方式
        id: 'combo_myfs4',
        name: 'TRADEWAYCODES',
        store: store_myfs4,
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        anyMatch: true,
        forceSelection: true,
        tabIndex: 71,
        hideTrigger: true,
        multiSelect: false,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand();
                }
            }
        },
        flex: 0.85,
        margin: 0,
        listConfig: {
            maxHeight: 110, minWidth: 160,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        },
        allowBlank: false,
        blankText: '贸易方式不能为空!'
    })
    //贸易方式
    var field_myfs4 = {
        xtype: 'fieldcontainer',
        fieldLabel: '贸易方式',
        forceSelection: true,
        layout: 'hbox',
        items: [combo_myfs4, {
            xtype: 'button', id: 'myfs_btn4', listeners: {
                click: function () { selectmyfs(combo_myfs4, field_ORDERREQUEST4); }
            }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    var field_FILINGNUMBER4 = Ext.create('Ext.form.field.Text', {
        tabIndex: 72,
        margin: 0,
        flex: .85,
        name: 'RECORDCODE'
    });
    var zcbah_container4 = {
        xtype: 'fieldcontainer',
        fieldLabel: '备案号',
        layout: 'hbox',
        items: [field_FILINGNUMBER4, {
            xtype: 'button',
            listeners: {
                //click: function () {
                //    selectmyfs(combo_myfs, field_TRADEWAYCODES);
                //}
            },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    //----------------------------------------------法检状况，通关单号，进口报关单，报关申报单位，报检申报单位---------------------------------------
    //法检状况
    var chk_CHKLAWCONDITION4 = Ext.create('Ext.form.field.Checkbox', {
        tabIndex: 73,
        fieldLabel: '法检状况',
        name: 'LAWFLAG'
    })
    //通关单号
    var field_CLEARANCENO4 = Ext.create('Ext.form.field.Text', {
        fieldLabel: '通关单号',
        tabIndex: 74,
        name: 'CLEARANCENO'
    });
    //出口报关单
    var field_ASSOCIATEPEDECLNO4 = Ext.create('Ext.form.field.Text', {
        fieldLabel: '进口报关单',
        tabIndex: 75,
        name: 'ASSOCIATEPEDECLNO'
    });
    //报关申报单位 
    var tf_bgsbdw4 = Ext.create('Ext.form.field.Text', {
        id: 'tf_bgsbdw4',
        readOnly: true,
        name: 'REPUNITCODE',
        margin: 0,
        flex: .85,
        allowBlank: false,
        blankText: '报关单位不能为空!'
    })
    var cont_bgsbdw4 = Ext.create('Ext.form.FieldContainer', {
        id: 'cont_bgsbdw4',
        fieldLabel: '报关申报单位',
        layout: 'hbox',
        items: [tf_bgsbdw4, {
            xtype: 'button', id: 'bgsbdw_btn4', listeners: {
                click: function () { bgsbdw_win(tf_bgsbdw4); }
            }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    })
    //报检申报单位   
    var tf_bjsbdw4 = Ext.create('Ext.form.field.Text', {
        id: 'tf_bjsbdw4',
        readOnly: true,
        name: 'INSPUNITCODE',
        margin: 0,
        flex: .85,
        allowBlank: false,
        blankText: '报检单位不能为空!'
    })
    var cont_bjsbdw4 = Ext.create('Ext.form.FieldContainer', {
        id: 'cont_bjsbdw4',
        fieldLabel: '报检申报单位',
        layout: 'hbox',
        items: [tf_bjsbdw4, { xtype: 'button', id: 'bjsbdw_btn4', listeners: { click: function () { bjsbdw_win(tf_bjsbdw4); } }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0 }]
    })
    //需求备注
    var field_ORDERREQUEST4 = Ext.create('Ext.form.field.Text', {
        id: 'field_ORDERREQUEST4',
        fieldLabel: '需求备注',
        tabIndex: 76,
        name: 'ORDERREQUEST'
    });
    var store_status4 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: orderstatus_js_data
    })
    var field_STATUS4 = Ext.create('Ext.form.field.ComboBox', {//业务状态
        id: 'field_status4',
        name: 'STATUS',
        valueField: 'CODE',
        displayField: 'NAME',
        fieldLabel: '业务状态',
        queryMode: 'local',
        editable: false,
        hiddenTrigger: true,
        readOnly: true,
        labelWidth: 80,
        value: 0,
        store: store_status4
    });

    var field_CUSTOMERNAME4 = Ext.create('Ext.form.field.Hidden', { name: 'CUSTOMERNAME' });
    var field_CLEARUNITNAME4 = Ext.create('Ext.form.field.Hidden', { name: 'CLEARUNITNAME' });

    //委托单位
    var store_wtdw4 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_wtdw
    })
    var combo_wtdw4 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_wtdw4',
        name: 'CUSTOMERCODE',
        store: store_wtdw4,
        hideTrigger: true,
        fieldLabel: '委托单位', forceSelection: true,
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
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
                if (Ext.getCmp('combo_jsdw4').getValue() == "") {
                    Ext.getCmp('combo_jsdw4').setValue(newValue);
                }
                field_CUSTOMERNAME4.setValue(combo.rawValue);
            }
        },
        allowBlank: false,
        blankText: '委托单位不能为空!'
    });

    //结算单位
    var store_jsdw4 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_wtdw
    })
    var combo_jsdw4 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_jsdw4',
        name: 'CLEARUNIT',
        store: store_jsdw4,
        hideTrigger: true,
        fieldLabel: '结算单位', forceSelection: true,
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
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
                field_CLEARUNITNAME4.setValue(combo.rawValue);
            }
        },
        allowBlank: false,
        blankText: '结算单位不能为空!'
    });

    //结算备注
    var field_CLEARREMARK4 = Ext.create('Ext.form.field.Text', {
        id: 'field_CLEARREMARK4',
        tabIndex: 23,
        fieldLabel: '结算备注',
        name: 'CLEARREMARK'
    });


    formpanelout2 = Ext.create('Ext.form.Panel', {
        border: 0,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: .20,
            labelSeparator: '',
            msgTarget: 'under',
            labelAlign: 'right',
            validateOnBlur: false,
            validateOnChange: false
        },
        items: [{ layout: 'column', border: 0, items: [label_out4, chk_container4] },
        { layout: 'column', height: 42, border: 0, items: [field_CODE4, combo_ENTRUSTTYPENAME4, field_CUSNO4, field_jydw4, combo_DECLWAY4] },
        { layout: 'column', height: 42, border: 0, items: [field_quanpackage4, field_weight4, field_contractno4, field_myfs4, zcbah_container4] },
        { layout: 'column', height: 42, border: 0, items: [chk_CHKLAWCONDITION4, field_CLEARANCENO4, field_ASSOCIATEPEDECLNO4, cont_bgsbdw4, cont_bjsbdw4] },
        { layout: 'column', height: 42, border: 0, items: [field_ORDERREQUEST4, field_STATUS4, combo_wtdw4, combo_jsdw4, field_CLEARREMARK4] },
         field_BUSIUNITNAME4, field_CUSTOMERNAME4, field_CLEARUNITNAME4
        ]
    })
}