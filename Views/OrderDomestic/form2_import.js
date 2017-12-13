function form2_import_ini() {
    var label3_in = {
        xtype: 'label',
        columnWidth: .70,
        margin: '0 0 5 5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;进口信息</span></h4>'
    }
    var relation_confirm_chk3 = Ext.create('Ext.form.field.Checkbox', {
        fieldLabel: '特殊关系确认',
        name: 'SPECIALRELATIONSHIP',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue) {
                    price_confirm_chk3.setReadOnly(false);
                }
                else {
                    price_confirm_chk3.setValue(false);
                    price_confirm_chk3.setReadOnly(true);
                }
            }
        }
    });
    var price_confirm_chk3 = Ext.create('Ext.form.field.Checkbox', {
        fieldLabel: '价格影响确认',
        name: 'PRICEIMPACT',
        readOnly: true
    });
    var fee_confirm_chk3 = Ext.create('Ext.form.field.Checkbox', {
        labelWidth: 125,
        fieldLabel: '支付特许权使用费确认',
        name: 'PAYPOYALTIES'
    });
    var chk_container3 = {
        columnWidth: .30,
        border: 2,
        height: 25,
        style: {
            borderColor: '#e9b477',
            borderStyle: 'solid'
        },
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [relation_confirm_chk3, price_confirm_chk3, fee_confirm_chk3]
    }
    //根据"双单关联号"可以查出四单关联的另两单的信息，用“BUSITYPE”区分，加载
    //------------------------------------------------订单编号，委托类型，客户编号，经营单位，报关方式-----------------------------------------------------------------
    //订单编号
    var field_CODE3 = Ext.create('Ext.form.field.Text', {
        id: 'code3',
        name: 'CODE',
        fieldLabel: '订单编号',
        emptyText: '订单号自动生成',
        readOnly: true
    });
    //委托类型
    var store_ENTRUSTTYPENAME3 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: wtlx_js_data
    })
    var combo_ENTRUSTTYPENAME3 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ENTRUSTTYPENAME3',
        name: 'ENTRUSTTYPE',
        store: store_ENTRUSTTYPENAME3,
        fieldLabel: '委托类型',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        hideTrigger: true,
        forceSelection: true,
        tabIndex: 43,
        anyMatch: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.expand();
                }
            },
            select: function (cb, records) {
                bg_bj_sbdw_control(cb, 3);
            }
        },
        allowBlank: false,
        blankText: '委托类型不能为空!'
    })
    //客户编号
    var field_CUSNO3 = Ext.create('Ext.form.field.Text', {
        id: 'field_CUSNO3',
        name: 'CUSNO',
        tabIndex: 44,
        fieldLabel: '客户编号'
    });
    //经营单位
    var store_jydw3 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_jydw
    })
    var combo_jydw3 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_jydw3',
        name: 'BUSIUNITCODE',
        store: store_jydw3,
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        margin: 0,
        forceSelection: true,
        tabIndex: 45,
        anyMatch: true,
        minChars: 4,
        hideTrigger: true,
        listeners: {
            select: function (records) {
                field_BUSIUNITNAME3.setValue(records.rawValue.substr(0, records.rawValue.lastIndexOf('(')));
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
    var field_BUSIUNITNAME3 = Ext.create('Ext.form.field.Hidden', {
        name: 'BUSIUNITNAME'
    })
    var field_jydw3 = { //经营单位
        xtype: 'fieldcontainer',
        fieldLabel: '经营单位',
        layout: 'hbox',
        items: [combo_jydw3, {
            xtype: 'button', id: 'jydw_btn3', handler: function () {
                selectjydw(combo_jydw3, field_BUSIUNITNAME3);
            }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    var store_DECLWAY3 = Ext.create('Ext.data.JsonStore', { //报关方式
        fields: ['CODE', 'NAME'],
        data: common_data_bgfs
    })
    var combo_DECLWAY3 = Ext.create('Ext.form.field.ComboBox', {
        name: 'DECLWAY',
        hideTrigger: true,
        store: store_DECLWAY3,
        fieldLabel: '报关方式',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 46,
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
    //件数/包装 
    var store_PACKKINDNAME3 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_bzzl
    })
    var combo_PACKKINDNAME3 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_PACKKINDNAME3',
        name: 'PACKKIND',
        hideTrigger: true,
        store: store_PACKKINDNAME3,
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 48,
        margin: 0,
        listConfig: {
            maxHeight: 110
        },
        queryMode: 'local',
        anyMatch: true,
        listeners: {
            select: function (cb_pack, records, eOpts) {
                if (Ext.getCmp("combo_PACKKINDNAME4")) {
                    Ext.getCmp("combo_PACKKINDNAME4").setValue(records[0].get("CODE"));
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
    var field_quanpackage3 = {
        xtype: 'fieldcontainer',
        fieldLabel: '件数/包装',
        layout: 'hbox',
        items: [{
            id: 'GOODSNUM3', name: 'GOODSNUM', xtype: 'numberfield', flex: .5, margin: 0, hideTrigger: true, anyMatch: true, tabIndex: 47,
            spinUpEnabled: false, spinDownEnabled: false, allowBlank: false, blankText: '不能为空!',
            listeners: {
                change: function (nf, newValue, oldValue, eOpts) {
                    if (Ext.getCmp("GOODSNUM4")) {
                        Ext.getCmp("GOODSNUM4").setValue(newValue);
                    }
                },
                focus: function (nf) {
                    nf.clearInvalid();
                }
            }
        }, combo_PACKKINDNAME3]
    }
    //毛重/净重
    var field_weight3 = {
        xtype: 'fieldcontainer',
        fieldLabel: '毛重/净重',
        layout: 'hbox',
        items:
        [
            {
                id: 'GOODSGW3', name: 'GOODSGW', xtype: 'numberfield', flex: .5, margin: 0, allowBlank: false, blankText: '不能为空!', hideTrigger: true, decimalPrecision: 4, tabIndex: 49,
                spinUpEnabled: false, spinDownEnabled: false,
                listeners: {
                    focus: function (nf) {
                        nf.clearInvalid();
                    },
                    change: function (nf, newValue, oldValue, eOpts) {
                        if (Ext.getCmp("GOODSGW4")) {
                            Ext.getCmp("GOODSGW4").setValue(newValue);
                        }
                    }
                }
            },
            {
                id: 'GOODSNW3', name: 'GOODSNW', xtype: 'numberfield', flex: .5, margin: 0, hideTrigger: true, decimalPrecision: 4, tabIndex: 50,
                spinUpEnabled: false, spinDownEnabled: false,
                listeners:
                {
                    change: function (nf, newValue, oldValue, eOpts) {
                        if (Ext.getCmp("GOODSNW4")) {
                            Ext.getCmp("GOODSNW4").setValue(newValue);
                        }
                    }
                }
            }
        ]
    }
    //合同号
    var field_contractno3 = Ext.create('Ext.form.field.Text', {
        id: 'CONTRACTNO3',
        fieldLabel: '合同号',
        tabIndex: 51,
        name: 'CONTRACTNO'
    });
    //贸易方式 
    var store_myfs3 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_myfs
    })
    var combo_myfs3 = Ext.create('Ext.form.field.ComboBox', {//贸易方式
        id: 'combo_myfs3',
        name: 'TRADEWAYCODES',
        store: store_myfs3,
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        anyMatch: true,
        forceSelection: true,
        tabIndex: 52,
        hideTrigger: true,
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
    var field_myfs3 = {
        xtype: 'fieldcontainer',
        fieldLabel: '贸易方式',
        layout: 'hbox',
        items: [combo_myfs3, {
            xtype: 'button', id: 'myfs_btn3',
            listeners: { click: function () { selectmyfs(combo_myfs3, field_ORDERREQUEST3); } },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    var field_FILINGNUMBER3 = Ext.create('Ext.form.field.Text', {
        tabIndex: 53,
        margin: 0,
        flex: .85,
        name: 'RECORDCODE'
    });
    var zcbah_container3 = {
        xtype: 'fieldcontainer',
        fieldLabel: '备案号',
        layout: 'hbox',
        items: [field_FILINGNUMBER3, {
            xtype: 'button',
            listeners: {
                //click: function () {
                //    selectmyfs(combo_myfs, field_TRADEWAYCODES);
                //}
            },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    //----------------------------------------------法检状况，通关单号，出口报关单，报关申报单位，报检申报单位-----------------------------------------
    //法检状况
    var chk_CHKLAWCONDITION3 = Ext.create('Ext.form.field.Checkbox', {
        tabIndex: 54,
        fieldLabel: '法检状况',
        name: 'LAWFLAG'
    })
    //通关单号
    var field_CLEARANCENO3 = Ext.create('Ext.form.field.Text', {
        fieldLabel: '通关单号',
        tabIndex: 55,
        name: 'CLEARANCENO'
    });
    //出口报关单
    var field_ASSOCIATEPEDECLNO3 = Ext.create('Ext.form.field.Text', {
        fieldLabel: '出口报关单',
        name: 'ASSOCIATEPEDECLNO',
        tabIndex: 56
    });
    //报关申报单位 
    var tf_bgsbdw3 = Ext.create('Ext.form.field.Text', {
        id: 'tf_bgsbdw3',
        readOnly: true,
        name: 'REPUNITCODE',
        margin: 0,
        flex: .85,
        allowBlank: false,
        blankText: '报关单位不能为空!'
    })
    var cont_bgsbdw3 = Ext.create('Ext.form.FieldContainer', {
        id: 'cont_bgsbdw3',
        fieldLabel: '报关申报单位',
        layout: 'hbox',
        items: [tf_bgsbdw3, {
            xtype: 'button', id: 'bgsbdw_btn3', listeners: {
                click: function () { bgsbdw_win(tf_bgsbdw3); }
            }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    })
    //报检申报单位   
    var tf_bjsbdw3 = Ext.create('Ext.form.field.Text', {
        id: 'tf_bjsbdw3',
        readOnly: true,
        name: 'INSPUNITCODE',
        margin: 0,
        flex: .85,
        allowBlank: false,
        blankText: '报检单位不能为空!'
    })
    var cont_bjsbdw3 = Ext.create('Ext.form.FieldContainer', {
        id: 'cont_bjsbdw3',
        fieldLabel: '报检申报单位',
        layout: 'hbox',
        items: [tf_bjsbdw3, { xtype: 'button', id: 'bjsbdw_btn3', listeners: { click: function () { bjsbdw_win(tf_bjsbdw3); } }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0 }]
    })
    //--------------------------------------------------------------------需求备注，业务状态-----------------------------------------------------------------
    //需求备注
    var field_ORDERREQUEST3 = Ext.create('Ext.form.field.Text', {
        id: 'field_ORDERREQUEST3',
        fieldLabel: '需求备注',
        tabIndex: 59,
        name: 'ORDERREQUEST'
    });
    var store_status3 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: orderstatus_js_data
    })
    var field_STATUS3 = Ext.create('Ext.form.field.ComboBox', {//业务状态
        id: 'field_status3',
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
        store: store_status3
    });

    var field_CUSTOMERNAME3 = Ext.create('Ext.form.field.Hidden', { name: 'CUSTOMERNAME' });
    var field_CLEARUNITNAME3 = Ext.create('Ext.form.field.Hidden', { name: 'CLEARUNITNAME' });

    //委托单位
    var store_wtdw3 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_wtdw
    })
    var combo_wtdw3 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_wtdw3',
        name: 'CUSTOMERCODE',
        store: store_wtdw3,
        hideTrigger: true,
        fieldLabel: '委托单位',
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
                if (Ext.getCmp('combo_jsdw3').getValue() == "") {
                    Ext.getCmp('combo_jsdw3').setValue(newValue);
                }
                field_CUSTOMERNAME3.setValue(combo.rawValue);
            }
        },
        allowBlank: false,
        blankText: '委托单位不能为空!'
    });

    //结算单位
    var store_jsdw3 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_wtdw
    })
    var combo_jsdw3 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_jsdw3',
        name: 'CLEARUNIT',
        store: store_jsdw3,
        hideTrigger: true,
        fieldLabel: '结算单位',
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
                field_CLEARUNITNAME3.setValue(combo.rawValue);
            }
        },
        allowBlank: false,
        blankText: '结算单位不能为空!'
    });

    //结算备注
    var field_CLEARREMARK3 = Ext.create('Ext.form.field.Text', {
        id: 'field_CLEARREMARK3',
        tabIndex: 23,
        fieldLabel: '结算备注',
        name: 'CLEARREMARK'
    });


    formpanelin2 = Ext.create('Ext.form.Panel', {
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
        items: [
                { layout: 'column', border: 0, items: [label3_in, chk_container3] },
                { layout: 'column', height: 42, border: 0, items: [field_CODE3, combo_ENTRUSTTYPENAME3, field_CUSNO3, field_jydw3, combo_DECLWAY3] },
                { layout: 'column', height: 42, border: 0, items: [field_quanpackage3, field_weight3, field_contractno3, field_myfs3, zcbah_container3] },
                { layout: 'column', height: 42, border: 0, items: [chk_CHKLAWCONDITION3, field_CLEARANCENO3, field_ASSOCIATEPEDECLNO3, cont_bgsbdw3, cont_bjsbdw3] },
                { layout: 'column', height: 42, border: 0, items: [field_ORDERREQUEST3, field_STATUS3, combo_wtdw3, combo_jsdw3, field_CLEARREMARK3] },
                field_BUSIUNITNAME3, field_CUSTOMERNAME3, field_CLEARUNITNAME3
        ]
    })
}