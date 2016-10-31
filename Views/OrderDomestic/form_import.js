function form_import_ini() {
    var label_in = {
        xtype: 'label',
        columnWidth: .70,
        margin: '0 0 5 5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;进口信息</span></h4>'
    }
    var relation_confirm_chk1 = Ext.create('Ext.form.field.Checkbox', {
        fieldLabel: '特殊关系确认',
        name: 'SPECIALRELATIONSHIP',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue) {
                    price_confirm_chk1.setReadOnly(false);
                }
                else {
                    price_confirm_chk1.setValue(false);
                    price_confirm_chk1.setReadOnly(true);
                }
            }
        }
    });
    var price_confirm_chk1 = Ext.create('Ext.form.field.Checkbox', {
        fieldLabel: '价格影响确认',
        name: 'PRICEIMPACT',
        readOnly: true
    });
    var fee_confirm_chk1 = Ext.create('Ext.form.field.Checkbox', {
        labelWidth: 125,
        fieldLabel: '支付特许权使用费确认',
        name: 'PAYPOYALTIES'
    });
    var chk_container1 = {
        columnWidth: .30,
        border: 2,
        height: 25,
        style: {
            borderColor: '#e9b477',
            borderStyle: 'solid'
        },
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [relation_confirm_chk1, price_confirm_chk1, fee_confirm_chk1]
    }
    //根据"双单关联号"可以查出四单关联的另两单的信息，用“BUSITYPE”区分，加载
    //------------------------------------------------订单编号，委托类型，客户编号，经营单位，报关方式-----------------------------------------------------------------
    //订单编号
    var field_CODE = Ext.create('Ext.form.field.Text', {
        id: 'code1',
        name: 'CODE',
        fieldLabel: '订单编号',
        emptyText: '订单号自动生成',
        readOnly: true
    });
    //委托类型[{ "CODE": "01", "NAME": "报关单(01)" }, { "CODE": "02", "NAME": "报检单(02)" }, { "CODE": "03", "NAME": "报关/检单(03)" }]
    var store_ENTRUSTTYPENAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: wtlx_js_data
    })
    var combo_ENTRUSTTYPENAME = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ENTRUSTTYPENAME1',
        name: 'ENTRUSTTYPE',
        store: store_ENTRUSTTYPENAME,
        fieldLabel: '委托类型',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        hideTrigger: true,
        forceSelection: true,
        tabIndex: 5,
        anyMatch: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.expand();
                }
            },
            select: function (cb, records) {
                bg_bj_sbdw_control(cb, 1);
            }
        },
        allowBlank: false,
        blankText: '委托类型不能为空!'
    })
    //客户编号
    var field_CUSNO = Ext.create('Ext.form.field.Text', {
        id: 'field_CUSNO1',
        name: 'CUSNO',
        tabIndex: 6,
        fieldLabel: '客户编号'
    });
    //经营单位
    var store_jydw = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_jydw
    })
    var combo_jydw = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_jydw1',
        name: 'BUSIUNITCODE',
        store: store_jydw,
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        margin: 0,
        forceSelection: true,
        tabIndex: 7,
        anyMatch: true,
        minChars: 4,
        hideTrigger: true,
        listeners: {
            select: function (records) {
                field_BUSIUNITNAME.setValue(records.rawValue.substr(0, records.rawValue.lastIndexOf('(')));
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
    var field_BUSIUNITNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'BUSIUNITNAME'
    })
    var field_jydw = { //经营单位
        xtype: 'fieldcontainer',
        fieldLabel: '经营单位',
        layout: 'hbox',
        items: [combo_jydw, {
            xtype: 'button', id: 'jydw_btn1', handler: function () {
                selectjydw(combo_jydw, field_BUSIUNITNAME);
            }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    var store_DECLWAY = Ext.create('Ext.data.JsonStore', { //报关方式
        fields: ['CODE', 'NAME'],
        data: common_data_bgfs
    })
    var combo_DECLWAY = Ext.create('Ext.form.field.ComboBox', {
        name: 'DECLWAY',
        hideTrigger: true,
        store: store_DECLWAY,
        fieldLabel: '报关方式',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 8,
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
    //件数/包装，
    var store_PACKKINDNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_bzzl
    })
    var combo_PACKKINDNAME = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_PACKKINDNAME1',
        name: 'PACKKIND',
        hideTrigger: true,
        store: store_PACKKINDNAME,
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 10,
        queryMode: 'local',
        margin: 0,
        anyMatch: true,
        listConfig: {
            maxHeight: 110
        },
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand();
                }
            },
            select: function (cb_pack, records, eOpts) {
                if (Ext.getCmp("combo_PACKKINDNAME2")) {
                    Ext.getCmp("combo_PACKKINDNAME2").setValue(records[0].get("CODE"));
                }
            }
        },
        flex: .5
    })
    //件数/包装
    var field_quanpackage = {
        xtype: 'fieldcontainer',
        fieldLabel: '件数/包装',
        layout: 'hbox',
        items: [
            {
                id: 'GOODSNUM1', name: 'GOODSNUM', xtype: 'numberfield', flex: .5, margin: 0, hideTrigger: true, anyMatch: true, tabIndex: 9,
                spinUpEnabled: false, spinDownEnabled: false,
                listeners: {
                    change: function (nf, newValue, oldValue, eOpts) {
                        if (Ext.getCmp("GOODSNUM2")) {
                            Ext.getCmp("GOODSNUM2").setValue(newValue);
                        }
                    }
                }
            }, combo_PACKKINDNAME]
    }
    //毛重/净重
    var field_weight = {
        xtype: 'fieldcontainer',
        fieldLabel: '毛重/净重',
        layout: 'hbox',
        items:
        [
            {
                id: 'GOODSGW1', name: 'GOODSGW', xtype: 'numberfield', flex: .5, margin: 0, allowBlank: false, blankText: '不能为空!', hideTrigger: true, decimalPrecision: 4, tabIndex: 11,
                spinUpEnabled: false, spinDownEnabled: false,
                listeners: {
                    focus: function (nf) {
                        nf.clearInvalid();
                    },
                    change: function (nf, newValue, oldValue, eOpts) {
                        if (Ext.getCmp("GOODSGW2")) {
                            Ext.getCmp("GOODSGW2").setValue(newValue);
                        }
                    }
                }
            },
            {
                id: 'GOODSNW1', name: 'GOODSNW', xtype: 'numberfield', flex: .5, margin: 0, hideTrigger: true, decimalPrecision: 4, tabIndex: 12,
                spinUpEnabled: false, spinDownEnabled: false,
                listeners: {
                    change: function (nf, newValue, oldValue, eOpts) {
                        if (Ext.getCmp("GOODSNW2")) {
                            Ext.getCmp("GOODSNW2").setValue(newValue);
                        }
                    }
                }
            }
        ]
    }
    //合同号
    var field_contractno = Ext.create('Ext.form.field.Text', {
        id: 'CONTRACTNO1',
        fieldLabel: '合同号',
        tabIndex: 13,
        name: 'CONTRACTNO'
    });
    //贸易方式combostore
    var store_myfs = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_myfs
    })
    var combo_myfs = Ext.create('Ext.form.field.ComboBox', {//贸易方式
        id: 'combo_myfs1',
        name: 'TRADEWAYCODES',
        store: store_myfs,
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        anyMatch: true,
        forceSelection: true,
        tabIndex: 14,
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
            maxHeight: 110,
            minWidth: 160,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        },
        allowBlank: false,
        blankText: '贸易方式不能为空!'
        // ,multiSelect: true
    })
    //贸易方式
    var field_myfs = {
        xtype: 'fieldcontainer',
        fieldLabel: '贸易方式',
        layout: 'hbox',
        items: [combo_myfs, {
            xtype: 'button', id: 'myfs_btn1',
            listeners: { click: function () { selectmyfs(combo_myfs, field_ORDERREQUEST); } },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    var field_FILINGNUMBER = Ext.create('Ext.form.field.Text', {
        tabIndex: 15,
        margin: 0,
        flex: .85,
        name: 'RECORDCODE'
    });
    var zcbah_container = {
        xtype: 'fieldcontainer',
        fieldLabel: '备案号',
        layout: 'hbox',
        items: [field_FILINGNUMBER, {
            xtype: 'button',
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    //----------------------------------------------法检状况，通关单号，出口报关单，报关申报单位，报检申报单位-----------------------------------------
    //法检状况
    var chk_CHKLAWCONDITION = Ext.create('Ext.form.field.Checkbox', {
        tabIndex: 16,
        fieldLabel: '法检状况',
        name: 'LAWFLAG'
    })
    //通关单号
    var field_CLEARANCENO = Ext.create('Ext.form.field.Text', {
        fieldLabel: '通关单号',
        tabIndex: 17,
        name: 'CLEARANCENO'
    });
    //出口报关单
    var field_ASSOCIATEPEDECLNO = Ext.create('Ext.form.field.Text', {
        fieldLabel: '出口报关单',
        name: 'ASSOCIATEPEDECLNO',
        tabIndex: 18
    });
    //报关申报单位
    var tf_bgsbdw1 = Ext.create('Ext.form.field.Text', {
        id: 'tf_bgsbdw1',
        readOnly: true,
        name: 'REPUNITCODE',
        margin: 0,
        flex: .85,
        allowBlank: false,
        blankText: '报关单位不能为空!'
    })
    var cont_bgsbdw = Ext.create('Ext.form.FieldContainer', {
        id: 'cont_bgsbdw1',
        fieldLabel: '报关申报单位',
        layout: 'hbox',
        items: [tf_bgsbdw1, {
            xtype: 'button', id: 'bgsbdw_btn1', listeners: {
                click: function () { bgsbdw_win(tf_bgsbdw1); }
            }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    })
    //报检申报单位   
    var tf_bjsbdw1 = Ext.create('Ext.form.field.Text', {
        id: 'tf_bjsbdw1',
        readOnly: true,
        name: 'INSPUNITCODE',
        margin: 0,
        flex: .85,
        allowBlank: false,
        blankText: '报检单位不能为空!'
    })
    var cont_bjsbdw = Ext.create('Ext.form.FieldContainer', {
        id: 'cont_bjsbdw1',
        fieldLabel: '报检申报单位',
        layout: 'hbox',
        items: [tf_bjsbdw1, { xtype: 'button', id: 'bjsbdw_btn1', listeners: { click: function () { bjsbdw_win(tf_bjsbdw1); } }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0 }]
    })
    //--------------------------------------------------------------------需求备注，业务状态-----------------------------------------------------------------
    //需求备注
    var field_ORDERREQUEST = Ext.create('Ext.form.field.Text', {
        id: 'field_ORDERREQUEST1',
        fieldLabel: '需求备注',
        tabIndex: 20,
        name: 'ORDERREQUEST'
    });
    var store_status = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: orderstatus_js_data
    })
    var field_STATUS = Ext.create('Ext.form.field.ComboBox', {//业务状态
        id: 'field_status1',
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
        store: store_status
    });
    var field_DECLSTATUS1 = Ext.create('Ext.form.field.Hidden', {
        id: 'field_DECLSTATUS1',
        name: 'DECLSTATUS'
    })
    var field_INSPSTATUS1 = Ext.create('Ext.form.field.Hidden', {
        id:'field_INSPSTATUS1',
        name: 'INSPSTATUS'
    })    
    formpanelin = Ext.create('Ext.form.Panel', {
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
                { layout: 'column', border: 0, items: [label_in, chk_container1] },
                { layout: 'column', height: 42, border: 0, items: [field_CODE, combo_ENTRUSTTYPENAME, field_CUSNO, field_jydw, combo_DECLWAY] },
                { layout: 'column', height: 42, border: 0, items: [field_quanpackage, field_weight, field_contractno, field_myfs, zcbah_container] },
                { layout: 'column', height: 42, border: 0, items: [chk_CHKLAWCONDITION, field_CLEARANCENO, field_ASSOCIATEPEDECLNO, cont_bgsbdw, cont_bjsbdw] },
                { layout: 'column', height: 42, border: 0, items: [field_ORDERREQUEST, field_STATUS] },
                field_BUSIUNITNAME, field_DECLSTATUS1, field_INSPSTATUS1
        ]
    })
}