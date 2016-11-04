function form_export_ini() {
    var label_out = {
        xtype: 'label',
        columnWidth: .70,
        margin: '0 0 5 5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;出口信息</span></h4>'
    }
    var relation_confirm_chk2 = Ext.create('Ext.form.field.Checkbox', {
        fieldLabel: '特殊关系确认',
        name: 'SPECIALRELATIONSHIP',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue) {
                    price_confirm_chk2.setReadOnly(false);
                }
                else {
                    price_confirm_chk2.setValue(false);
                    price_confirm_chk2.setReadOnly(true);
                }
            }
        }

    });
    var price_confirm_chk2 = Ext.create('Ext.form.field.Checkbox', {
        fieldLabel: '价格影响确认',
        name: 'PRICEIMPACT',
        readOnly: true
    });
    var fee_confirm_chk2 = Ext.create('Ext.form.field.Checkbox', {
        labelWidth: 125,
        fieldLabel: '支付特许权使用费确认',
        name: 'PAYPOYALTIES'
    });
    var chk_container2 = {
        columnWidth: .30,
        border: 2,
        height: 25,
        style: {
            borderColor: '#e9b477',
            borderStyle: 'solid'
        },
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [relation_confirm_chk2, price_confirm_chk2, fee_confirm_chk2]
    }
    //------------------------------------------------订单编号，委托类型，客户编号，经营单位，报关方式-----------------------------------------------
    var field_CODE2 = Ext.create('Ext.form.field.Text', {//订单编号
        name: 'CODE',
        fieldLabel: '订单编号',
        emptyText: '订单号自动生成',
        readOnly: true
    });
    //委托类型
    var store_ENTRUSTTYPENAME2 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: wtlx_js_data
    })
    var combo_ENTRUSTTYPENAME2 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ENTRUSTTYPENAME2',
        name: 'ENTRUSTTYPE',
        store: store_ENTRUSTTYPENAME2,
        fieldLabel: '委托类型',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        anyMatch: true,
        forceSelection: true,
        hideTrigger: true,
        triggerAction: 'all',
        tabIndex: 21,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.expand();
                }
            },
            select: function (cb, records) {
                bg_bj_sbdw_control(cb, 2);
            }
        },
        allowBlank: false,
        blankText: '委托类型不能为空!'
    })
    //客户编号
    var field_CUSNO2 = Ext.create('Ext.form.field.Text', {
        id: 'field_CUSNO2',
        name: 'CUSNO',
        tabIndex: 22,
        fieldLabel: '客户编号'
    });
    //经营单位
    var store_jydw2 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_jydw
    })
    var combo_jydw2 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_jydw2',
        name: 'BUSIUNITCODE',
        store: store_jydw2,
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        margin: 0,
        forceSelection: true,
        tabIndex: 23,
        anyMatch: true,
        minChars: 4,
        hideTrigger: true,
        listeners: {
            select: function (records) {
                field_BUSIUNITNAME2.setValue(records.rawValue.substr(0, records.rawValue.lastIndexOf('(')));
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
    var field_BUSIUNITNAME2 = Ext.create('Ext.form.field.Hidden', {
        name: 'BUSIUNITNAME'
    })
    //经营单位
    var field_jydw2 = {
        xtype: 'fieldcontainer',
        fieldLabel: '经营单位',
        layout: 'hbox',
        items: [combo_jydw2, {
            xtype: 'button', id: 'jydw_btn2', handler: function () {
                selectjydw(combo_jydw2, field_BUSIUNITNAME2);
            }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    //报关方式
    var store_DECLWAY2 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_bgfs
    })
    var combo_DECLWAY2 = Ext.create('Ext.form.field.ComboBox', {
        name: 'DECLWAY',
        hideTrigger: true,
        store: store_DECLWAY2,
        fieldLabel: '报关方式',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 25,
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
    var store_PACKKINDNAME2 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_bzzl
    })
    var combo_PACKKINDNAME2 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_PACKKINDNAME2',
        name: 'PACKKIND',
        hideTrigger: true,
        store: store_PACKKINDNAME2,
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 27,
        queryMode: 'local',
        margin: 0,
        listConfig: {
            maxHeight: 110
        },
        anyMatch: true,
        listeners: {
            select: function (cb_pack, records, eOpts) {
                if (Ext.getCmp("combo_PACKKINDNAME1")) {
                    Ext.getCmp("combo_PACKKINDNAME1").setValue(records[0].get("CODE"));
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
    var field_quanpackage2 = {
        xtype: 'fieldcontainer',
        fieldLabel: '件数/包装',
        layout: 'hbox',
        items: [{
            id: 'GOODSNUM2', name: 'GOODSNUM', xtype: 'numberfield', flex: .5, margin: 0, hideTrigger: true, anyMatch: true, tabIndex: 26,
            spinUpEnabled: false, spinDownEnabled: false, allowBlank: false, blankText: '不能为空!',
            listeners: {
                change: function (nf, newValue, oldValue, eOpts) {
                    if (Ext.getCmp("GOODSNUM1")) {
                        Ext.getCmp("GOODSNUM1").setValue(newValue);
                    }
                }
            }
        }, combo_PACKKINDNAME2]
    }
    //毛重/净重
    var field_weight2 = {
        xtype: 'fieldcontainer',
        fieldLabel: '毛重/净重',
        layout: 'hbox',
        items: [
            {
                id: 'GOODSGW2', name: 'GOODSGW', xtype: 'numberfield', flex: .5, margin: 0, allowBlank: false, blankText: '不能为空!', anyMatch: true, hideTrigger: true, tabIndex: 28, decimalPrecision: 4,
                spinUpEnabled: false, spinDownEnabled: false,
                listeners: {
                    focus: function (nf) {
                        nf.clearInvalid();
                    },
                    change: function (nf, newValue, oldValue, eOpts) {
                        if (Ext.getCmp("GOODSGW1")) {
                            Ext.getCmp("GOODSGW1").setValue(newValue);
                        }
                    }
                }
            },
            {
                id: 'GOODSNW2', name: 'GOODSNW', xtype: 'numberfield', flex: .5, margin: 0, hideTrigger: true, tabIndex: 29, anyMatch: true, decimalPrecision: 4,
                spinUpEnabled: false, spinDownEnabled: false,
                listeners: {
                    change: function (nf, newValue, oldValue, eOpts) {
                        if (Ext.getCmp("GOODSNW1")) {
                            Ext.getCmp("GOODSNW1").setValue(newValue);
                        }
                    }
                }
            }
        ]
    }
    //合同号
    var field_contractno2 = Ext.create('Ext.form.field.Text', {
        id: 'CONTRACTNO2',
        fieldLabel: '合同号',
        tabIndex: 30,
        name: 'CONTRACTNO'
    });
    //贸易方式
    var store_myfs2 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_myfs
    })
    var combo_myfs2 = Ext.create('Ext.form.field.ComboBox', {//贸易方式
        id: 'combo_myfs2',
        name: 'TRADEWAYCODES',
        store: store_myfs2,
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        anyMatch: true,
        forceSelection: true,
        tabIndex: 31,
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
        }
        , allowBlank: false,
        blankText: '贸易方式不能为空!'
    })
    //贸易方式
    var field_myfs2 = {
        xtype: 'fieldcontainer',
        fieldLabel: '贸易方式',
        forceSelection: true,
        layout: 'hbox',
        items: [combo_myfs2, {
            xtype: 'button', id: 'myfs_btn2', listeners: {
                click: function () { selectmyfs(combo_myfs2, field_ORDERREQUEST2); }
            }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    var field_FILINGNUMBER2 = Ext.create('Ext.form.field.Text', {
        tabIndex: 32,
        margin: 0,
        flex: .85,
        name: 'RECORDCODE'
    });
    var zcbah_container2 = {
        xtype: 'fieldcontainer',
        fieldLabel: '备案号',
        layout: 'hbox',
        items: [field_FILINGNUMBER2, {
            xtype: 'button',
            listeners: {
                //click: function () {
                //    selectmyfs(combo_myfs, field_TRADEWAYCODES);
                //}
            },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    //法检状况
    var chk_CHKLAWCONDITION2 = Ext.create('Ext.form.field.Checkbox', {
        tabIndex: 33,
        fieldLabel: '法检状况',
        name: 'LAWFLAG'
    })
    //通关单号
    var field_CLEARANCENO2 = Ext.create('Ext.form.field.Text', {
        fieldLabel: '通关单号',
        tabIndex: 34,
        name: 'CLEARANCENO'
    });
    //出口报关单
    var field_ASSOCIATEPEDECLNO2 = Ext.create('Ext.form.field.Text', {
        fieldLabel: '进口报关单',
        tabIndex: 35,
        name: 'ASSOCIATEPEDECLNO'
    });
    //报关申报单位
    var tf_bgsbdw2 = Ext.create('Ext.form.field.Text', {
        id: 'tf_bgsbdw2',
        readOnly: true,
        name: 'REPUNITCODE',
        margin: 0,
        flex: .85,
        allowBlank: false,
        blankText: '报关单位不能为空!'
    })
    var cont_bgsbdw2 = Ext.create('Ext.form.FieldContainer', {
        id: 'cont_bgsbdw2',
        fieldLabel: '报关申报单位',
        layout: 'hbox',
        items: [tf_bgsbdw2, {
            xtype: 'button', id: 'bgsbdw_btn2', listeners: {
                click: function () { bgsbdw_win(tf_bgsbdw2); }
            }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    })
    //报检申报单位   
    var tf_bjsbdw2 = Ext.create('Ext.form.field.Text', {
        id: 'tf_bjsbdw2',
        readOnly: true,
        name: 'INSPUNITCODE',
        margin: 0,
        flex: .85,
        allowBlank: false,
        blankText: '报检单位不能为空!'
    })
    var cont_bjsbdw2 = Ext.create('Ext.form.FieldContainer', {
        id: 'cont_bjsbdw2',
        fieldLabel: '报检申报单位',
        layout: 'hbox',
        items: [tf_bjsbdw2, { xtype: 'button', id: 'bjsbdw_btn2', listeners: { click: function () { bjsbdw_win(tf_bjsbdw2); } }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0 }]
    })
    //--------------------------------------------------------------------需求备注，业务状态---------------------------------------------------------
    //需求备注
    var field_ORDERREQUEST2 = Ext.create('Ext.form.field.Text', {
        id: 'field_ORDERREQUEST2',
        fieldLabel: '需求备注',
        tabIndex: 38,
        name: 'ORDERREQUEST'
    });
    var store_status2 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: orderstatus_js_data
    })

    var field_STATUS2 = Ext.create('Ext.form.field.ComboBox', {//业务状态
        id: 'field_status2',
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
        store: store_status2
    });
    formpanelout = Ext.create('Ext.form.Panel', {
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
        items: [{ layout: 'column', border: 0, items: [label_out, chk_container2] },
        { layout: 'column', height: 42, border: 0, items: [field_CODE2, combo_ENTRUSTTYPENAME2, field_CUSNO2, field_jydw2, combo_DECLWAY2] },
        { layout: 'column', height: 42, border: 0, items: [field_quanpackage2, field_weight2, field_contractno2, field_myfs2, zcbah_container2] },
        { layout: 'column', height: 42, border: 0, items: [chk_CHKLAWCONDITION2, field_CLEARANCENO2, field_ASSOCIATEPEDECLNO2, cont_bgsbdw2, cont_bjsbdw2] },
        { layout: 'column', height: 42, border: 0, items: [field_ORDERREQUEST2, field_STATUS2] },
         field_BUSIUNITNAME2
        ]
    })
}