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
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                }
            },
            change: function (combo, newValue, oldValue, eOpts) {
                SetItemno_consume(newValue);
            }
        },
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
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                }
            },
            change: function (field_paste, newValue, oldValue, eOpts) {
                if (newValue == "成品") {
                    if (!Ext.getCmp('formpanel_con') && !Ext.getCmp('gridpanel_PRODUCTCONSUME')) {
                        form_ini_con();
                    }
                    Ext.getCmp('formpanel_con').show();
                    Ext.getCmp('gridpanel_PRODUCTCONSUME').show();

                    SetItemno_consume(Ext.getCmp('combo_RECORDINFOID').getValue());//对应料件序号下拉
                } else {
                   if (Ext.getCmp('formpanel_con')) {
                       Ext.getCmp('formpanel_con').hide();
                   }
                   if (Ext.getCmp('gridpanel_PRODUCTCONSUME')) {
                       Ext.getCmp('gridpanel_PRODUCTCONSUME').hide();
                   }
                }
            }
        },
        fieldStyle: 'background-color: #CECECE; background-image: none;'
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
        value: 'U',
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
        value: 0,
        fieldStyle: 'background-color: #CECECE; background-image: none;'
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
        layout: 'hbox', flex: .5,
        items: [combo_CUSTOMER, {
            id: 'CUSTOMER_btn', xtype: 'button', handler: function () {
                selectjydw(combo_CUSTOMER, field_CUSTOMERNAME);
            },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }

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
                if (newValue != oldValue) {
                    if (i != 0) { hscode = ""; }

                    Ext.getCmp('HSCODE').reset();
                    Ext.getCmp('HSCODE').store.removeAll();

                    Ext.Ajax.request({
                        url: "/RecordInfor/GetCommodityHS",
                        params: { CUSTOMAREA: this.lastValue },
                        success: function (response, opts) {
                            var commondata = Ext.decode(response.responseText);
                            Ext.getCmp('HSCODE').store.loadData(commondata.hscode);

                            Ext.getCmp('HSCODE').setValue(hscode);
                        }
                    });
                    i++;
                    Element_ini();
                }
            }
        },
        allowBlank: false,
        blankText: '备案关区不能为空!'
    });

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //HS编码left
    var field_HSCODE_LEFT = Ext.create('Ext.form.field.Text', { id: 'HSCODE_LEFT', name: 'HSCODE_LEFT', readOnly: true, fieldLabel: 'HS编码', flex: .5, fieldStyle: 'background-color: #CECECE; background-image: none;' });
    //附加码left
    var field_ADDITIONALNO_LEFT = Ext.create('Ext.form.field.Text', { id: 'ADDITIONALNO_LEFT', name: 'ADDITIONALNO_LEFT', readOnly: true, fieldLabel: 'HS附加码', flex: .5, fieldStyle: 'background-color: #CECECE; background-image: none;' });

    var field_1_LEFT = { columnWidth: 1, layout: 'hbox', border: 0, items: [field_HSCODE_LEFT, field_ADDITIONALNO_LEFT] };

    //商品名称
    var field_COMMODITYNAME_LEFT = Ext.create('Ext.form.field.Text', { id: 'COMMODITYNAME_LEFT', name: 'COMMODITYNAME_LEFT', fieldLabel: '商品名称', readOnly: true, flex: .5, fieldStyle: 'background-color: #CECECE; background-image: none;' });
    //规格型号
    var field_SPECIFICATIONSMODEL_LEFT = Ext.create('Ext.form.field.Text', { id: 'SPECIFICATIONSMODEL_LEFT', name: 'SPECIFICATIONSMODEL_LEFT', fieldLabel: '规格型号', readOnly: true, flex: .5, fieldStyle: 'background-color: #CECECE; background-image: none;' });

    var field_2_LEFT = { columnWidth: 1, layout: 'hbox', border: 0, items: [field_COMMODITYNAME_LEFT, field_SPECIFICATIONSMODEL_LEFT] };

    //成交单位
    var store_UNIT_LEFT = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME', 'CODENAME'],
        data: common_data_unit
    });
    var combo_UNIT_LEFT = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_UNIT_LEFT',
        name: 'UNIT_LEFT',
        store: store_UNIT_LEFT,
        fieldLabel: '成交单位',
        displayField: 'CODENAME',
        valueField: 'CODE',
        hideTrigger: true,
        triggerAction: 'all',
        forceSelection: true,
        queryMode: 'local',
        anyMatch: true,
        readOnly: true, flex: .5, fieldStyle: 'background-color: #CECECE; background-image: none;',
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand();
                }
            }
        }
    });


    var field_3_LEFT = { columnWidth: 0.5, layout: 'hbox', border: 0, items: [combo_UNIT_LEFT] };//columnWidth:1

    var panel_left = Ext.create('Ext.panel.Panel', {
        id: 'panel_left',
        columnWidth: 0.50, margin: '0 2 0 0',
        border: 1, title: '<font style="font-weight:200px; font-size:14px;color:blue;"><center>变动前</center><font>',
        items: [
            { columnWidth: 1, layout: 'column', border: 0, height: 42, margin: '5 0 0 0', items: [field_1_LEFT] },
            { columnWidth: 1, layout: 'column', border: 0, height: 42, items: [field_2_LEFT] },
            { columnWidth: 1, layout: 'column', border: 0, height: 42, items: [field_3_LEFT] }
        ]
    });


    //HS编码right
    //var field_HSCODE = Ext.create('Ext.form.field.Text', {
    //    id: 'HSCODE', name: 'HSCODE', fieldLabel: 'HS编码', flex: .5,
    //    listeners: {
            //change: function (field_paste, newValue, oldValue, eOpts) {
            //    if (newValue != oldValue) {
            //        Element_ini();                    
            //    }
            //    if (newValue != Ext.getCmp('HSCODE_LEFT').getValue()) {
            //        this.setFieldStyle({ color: 'blue' });
            //    } else {
            //        this.setFieldStyle({ color: 'black' });
            //    }
            //}
    //    },
    //    allowBlank: false, blankText: 'HS编码不能为空!'
    //});
    var store_HSCODE = Ext.create('Ext.data.JsonStore', {
        fields: ['HSCODE']
    });
    var combo_HSCODE = Ext.create('Ext.form.field.ComboBox', {
        id: 'HSCODE', fieldLabel: 'HS编码', flex: .5, margin: 0, minChars: 4,
        name: 'HSCODE', maxLength: 8, minLength: 8, enforceMaxLength: true,
        store: store_HSCODE,
        hideTrigger: true,
        displayField: 'HSCODE',
        valueField: 'HSCODE',
        triggerAction: 'all',
        forceSelection: true,
        queryMode: 'local',
        anyMatch: true,
        listeners: {
            focus: function (cb) {
                cb.clearInvalid();
            },
            change: function (combo, newValue, oldValue, eOpts) {
                if (newValue != oldValue) {
                    if (j != 0) { extracode = ""; }

                    Ext.getCmp('ADDITIONALNO').reset();
                    Ext.getCmp('ADDITIONALNO').store.removeAll();

                    Ext.Ajax.request({
                        url: "/RecordInfor/GetCommodityEXTRA",
                        params: { CUSTOMAREA: Ext.getCmp('combo_CUSTOMAREA').getValue(), HSCODE: this.lastValue },
                        success: function (response, opts) {
                            var commondata = Ext.decode(response.responseText);
                            Ext.getCmp('ADDITIONALNO').store.loadData(commondata.extra);

                            Ext.getCmp('ADDITIONALNO').setValue(extracode);
                        }
                    });
                    j++;
                    Element_ini();

                }
                if (newValue != Ext.getCmp('HSCODE_LEFT').getValue()) {
                    this.setFieldStyle({ color: 'blue' });
                } else {
                    this.setFieldStyle({ color: 'black' });
                }
            }
        },
        allowBlank: false,
        blankText: 'HS编码不能为空!'
    });

    //附加码right
    //var field_ADDITIONALNO = Ext.create('Ext.form.field.Text', {
    //    id: 'ADDITIONALNO', name: 'ADDITIONALNO', fieldLabel: 'HS附加码', flex: .5,
    //    maxLength: 2, minLength: 2, enforceMaxLength: true, minLengthText: '附加码为2位！',
    //    listeners: {
    //        change: function (field_paste, newValue, oldValue, eOpts) {
    //            if (newValue != oldValue) {
    //                Element_ini();
    //            }
                
    //            if (newValue != Ext.getCmp('ADDITIONALNO_LEFT').getValue()) {
    //                this.setFieldStyle({ color: 'blue' });
    //            } else {
    //                this.setFieldStyle({ color: 'black' });
    //            }
    //        }
    //    },
    //    allowBlank: false, blankText: '附加码不能为空!'
    //});
    var store_ADDITIONALNO = Ext.create('Ext.data.JsonStore', {
        fields: ['EXTRACODE']
    });
    var combo_ADDITIONALNO = Ext.create('Ext.form.field.ComboBox', {
        id: 'ADDITIONALNO', fieldLabel: 'HS附加码', flex: .5, minChars: 1,
        name: 'ADDITIONALNO', maxLength: 2, minLength: 2, enforceMaxLength: true, minLengthText: '附加码为2位！',
        store: store_ADDITIONALNO,
        hideTrigger: true,
        displayField: 'EXTRACODE',
        valueField: 'EXTRACODE',
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
            change: function (combo, newValue, oldValue, eOpts) {
                if (newValue != oldValue) {
                    Element_ini();
                }
                if (newValue != Ext.getCmp('ADDITIONALNO_LEFT').getValue()) {
                    this.setFieldStyle({ color: 'blue' });
                } else {
                    this.setFieldStyle({ color: 'black' });
                }
            }
        },
        allowBlank: false,
        blankText: '附加码不能为空!'
    });
    var field_1_right = { columnWidth: 1, layout: 'hbox', border: 0, items: [combo_HSCODE, combo_ADDITIONALNO] };


    //商品名称
    var field_COMMODITYNAME = Ext.create('Ext.form.field.Text', {
        id: 'COMMODITYNAME',
        name: 'COMMODITYNAME',
        fieldLabel: '商品名称', flex: .5,
        listeners: {
            change: function (field_paste, newValue, oldValue, eOpts) {
                if (Ext.getCmp('panel_ele_2')) {
                    var id_pm = Ext.getCmp('id_pinming').getValue();
                    if (id_pm != "") {
                        if (Ext.getCmp(id_pm)) { Ext.getCmp(id_pm).setValue(newValue); }
                    }
                }

                if (newValue != Ext.getCmp('COMMODITYNAME_LEFT').getValue()) {
                    this.setFieldStyle({ color: 'blue' });
                } else {
                    this.setFieldStyle({ color: 'black' });
                }

            }
        },
        allowBlank: false,
        blankText: '商品名称不能为空!'
    });

    //规格型号
    var field_SPECIFICATIONSMODEL = Ext.create('Ext.form.field.Text', {
        id: 'SPECIFICATIONSMODEL',
        name: 'SPECIFICATIONSMODEL',
        fieldLabel: '规格型号', flex: .5,
        listeners: {
            change: function (field_paste, newValue, oldValue, eOpts) {
                if (newValue != Ext.getCmp('SPECIFICATIONSMODEL_LEFT').getValue()) {
                    this.setFieldStyle({ color: 'blue' });
                } else {
                    this.setFieldStyle({ color: 'black' });
                }
            }
        }
    });

    var field_2_right = { columnWidth: 1, layout: 'hbox', border: 0, items: [field_COMMODITYNAME, field_SPECIFICATIONSMODEL] };

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
        anyMatch: true,flex: .5,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand();
                }
            },
            change: function (field_paste, newValue, oldValue, eOpts) {
                if (newValue != Ext.getCmp('combo_UNIT_LEFT').getValue()) {
                    this.setFieldStyle({ color: 'blue' });
                } else {
                    this.setFieldStyle({ color: 'black' });
                }
            }
        },
        listConfig: {
            maxHeight: 130
        },
        allowBlank: false,
        blankText: '成交单位不能为空!'
    });
    

    var field_3_right = { columnWidth: 0.5, layout: 'hbox', border: 0, items: [combo_UNIT] };//columnWidth: 1

    var panel_right = Ext.create('Ext.panel.Panel', {
        id: 'panel_right',
        columnWidth: 0.50,
        border: 1, title: '<font style="font-weight:200px; font-size:14px;color:blue;"><center>变动后</center><font>',
        items: [
            { columnWidth: 1, layout: 'column', border: 0, margin: '5 0 0 0', height: 42, items: [field_1_right] },
            { columnWidth: 1, layout: 'column', border: 0, height: 42, items: [field_2_right] },
            { columnWidth: 1, layout: 'column', border: 0, height: 42, items: [field_3_right] }
        ]
    });
    
    var panel_ele = Ext.create('Ext.panel.Panel', {//申报要素
        id: 'panel_ele',
        columnWidth: 1,
        border: 0,
        items: []
    });

    var panel_compare = Ext.create('Ext.panel.Panel', {
        id: 'panel_compare',
        columnWidth: 1,
        border: 0,
        items: [
            { layout: 'column', border: 0, margin: '5 0 15 0', items: [panel_left, panel_right] },
            { layout: 'column', border: 0, items: [panel_ele] }
        ]
    });

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
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


    var configItem = [
        { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_RECORDINFOID, field_ITEMNO, combo_ITEMNOATTRIBUTE, combo_OPTIONS] },
        { layout: 'column', height: 42, border: 0, items: [combo_STATUS, combo_ISPRINT_APPLY, field_CUSTOMER, combo_CUSTOMAREA] },
        { layout: 'column', border: 0, items: [panel_compare] },
        { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [textarea_container] },
        { layout: 'column', height: 42, border: 0, items: [field_CREATEDATE, field_CREATENAME, field_SUBMITTIME, field_SUBMITNAME] },
        { layout: 'column', height: 42, border: 0, items: [field_ACCEPTTIME, field_ACCEPTNAME, field_PRETIME, field_PRENAME] },
        { layout: 'column', height: 42, border: 0, items: [field_REPTIME, field_REPNAME, field_FINISHTIME, field_FINISHNAME] },
        field_CREATEID, field_SUBMITID, field_ACCEPTID, field_PREID, field_REPID, field_FINISHID, field_CUSTOMERNAME, field_RID
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
                        + '<button type="button" onclick="printitemno(' + id + ')" id="btn_print" class="btn btn-primary btn-sm"><i class="fa fa-print"></i>&nbsp;申请表打印</button>'
                        + '<button type="button" onclick="create_save(\'save\')" id="btn_save" class="btn btn-primary btn-sm"><i class="fa fa-floppy-o"></i>&nbsp;保存</button>'
                        + '<button type="button" onclick="create_save(\'submit\')" id="btn_submitorder" class="btn btn-primary btn-sm"><i class="fa fa-hand-o-up"></i>&nbsp;提交申请</button></div>'

    var bbar_l = '<div class="btn-group">'
            + '<button type="button" onclick="cancel()" id="btn_cancelsubmit" class="btn btn-primary btn-sm"><i class="fa fa-angle-double-left"></i>&nbsp;撤回</button>'
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

function create_save(action) {

    if (action == 'submit') {

        if (!Ext.getCmp('formpanel_id').getForm().isValid()) {
            return;
        }

        var validate = "";
        if (!Ext.getCmp('panel_ele_2')) {
            validate += "申报要素不存在，请重新输入HS编码、备案关区！<br />";
        }

        if (Ext.getCmp('combo_ITEMNOATTRIBUTE').getValue() == "成品") {
            if (Ext.data.StoreManager.lookup('store_PRODUCTCONSUME').data.items.length <= 0) {
                validate += "成品单耗信息为空！<br />";
            }
        }

        if (validate) {
            Ext.MessageBox.alert("提示", validate);
            return;
        }

    }

    var formdata = Ext.encode(Ext.getCmp('formpanel_id').getForm().getValues());
    var productconsume = [];
    if (Ext.getCmp('combo_ITEMNOATTRIBUTE').getValue() == "成品") {
        productconsume = Ext.encode(Ext.pluck(Ext.data.StoreManager.lookup('store_PRODUCTCONSUME').data.items, 'data'));
    }

    var mask = new Ext.LoadMask(Ext.get(Ext.getBody()), { msg: "数据保存中，请稍等..." });

    mask.show();
    Ext.Ajax.request({
        url: "/RecordInfor/Change_Save",
        params: { id: id, formdata: formdata, productconsume: productconsume, action: action },
        success: function (response, option) {
            if (response.responseText) {
                mask.hide();
                var data = Ext.decode(response.responseText);
                if (data.success) {
                    id = data.id;
                    Ext.MessageBox.alert("提示", action == 'submit' ? "提交成功！" : "保存成功！", function () {
                        loadform_record();
                    });
                }
                else {
                    if (data.isgoing == "Y") { Ext.MessageBox.alert("提示", "此项号正在申请中!"); }
                    else {
                        Ext.MessageBox.alert("提示", action == 'submit' ? "提交失败！" : "保存失败！");
                    }
                }
            }
        }
    });
}

function loadform_record() {
    Ext.Ajax.request({
        url: "/RecordInfor/loadrecord_change",
        params: { id: id, rid: rid },
        success: function (response, opts) {
            var data = Ext.decode(response.responseText);
            hscode = data.formdata.HSCODE; extracode = data.formdata.ADDITIONALNO;//二次联动，前一个赋值后，调用此值 

            Ext.getCmp("formpanel_id").getForm().setValues(data.formdata);
            
            if (Ext.getCmp('gridpanel_PRODUCTCONSUME')) {
                Ext.getCmp('gridpanel_PRODUCTCONSUME').store.loadData(data.productsonsumedata);
            }

            formpanelcontrol();//表单字段控制
        }
    });
}

function formpanelcontrol() {
    var status = Ext.getCmp('combo_STATUS').getValue();
    document.getElementById("btn_save").disabled = status >= 10; //保存 
    document.getElementById("btn_cancelsubmit").disabled = (status == 0 || status == 50);//撤回:只有草稿才不可以撤回  
    document.getElementById("btn_submitorder").disabled = status >= 10;//提交申请
    document.getElementById("btn_print").disabled = status < 10;//打印

    Ext.Array.each(Ext.getCmp("formpanel_id").getForm().getFields().items, function (item) {
        if (item.fieldStyle != 'background-color: #CECECE; background-image: none;') {
            item.setReadOnly(status >= 10);
        }
    });

    if (Ext.getCmp('panel_ele_2')) {
        var id_pm = Ext.getCmp('id_pinming').getValue();
        if (id_pm != "") {
            if (Ext.getCmp(id_pm)) { Ext.getCmp(id_pm).setReadOnly(true); }
        }
    }

    //下面是表单控件涉及的弹窗选择按钮
    Ext.getCmp('CUSTOMER_btn').setDisabled(status >= 10); //报关行     
    if (Ext.getCmp("ITEMNO_CONSUME_btn")) { Ext.getCmp("ITEMNO_CONSUME_btn").setDisabled(status >= 10) }//对应料件序号

    //成品单耗 保存删除按钮
    if (Ext.getCmp("btn_pro_save")) { Ext.getCmp("btn_pro_save").setDisabled(status >= 10); }
    if (Ext.getCmp("btn_pro_del")) { Ext.getCmp("btn_pro_del").setDisabled(status >= 10); }


    if (Ext.getCmp('HSCODE').getValue() != Ext.getCmp('HSCODE_LEFT').getValue()) { Ext.getCmp('HSCODE').setFieldStyle({ color: 'blue' }); }
    else { Ext.getCmp('HSCODE').setFieldStyle({ color: 'black' }); }

    if (Ext.getCmp('ADDITIONALNO').getValue() != Ext.getCmp('ADDITIONALNO_LEFT').getValue()) { Ext.getCmp('ADDITIONALNO').setFieldStyle({ color: 'blue' }); }
    else { Ext.getCmp('ADDITIONALNO').setFieldStyle({ color: 'black' }); }

    if (Ext.getCmp('COMMODITYNAME').getValue() != Ext.getCmp('COMMODITYNAME_LEFT').getValue()) { Ext.getCmp('COMMODITYNAME').setFieldStyle({ color: 'blue' }); }
    else { Ext.getCmp('COMMODITYNAME').setFieldStyle({ color: 'black' }); }

    if (Ext.getCmp('SPECIFICATIONSMODEL').getValue() != Ext.getCmp('SPECIFICATIONSMODEL_LEFT').getValue()) { Ext.getCmp('SPECIFICATIONSMODEL').setFieldStyle({ color: 'blue' }); }
    else { Ext.getCmp('SPECIFICATIONSMODEL').setFieldStyle({ color: 'black' }); }

    if (Ext.getCmp('combo_UNIT').getValue() != Ext.getCmp('combo_UNIT_LEFT').getValue()) { Ext.getCmp('combo_UNIT').setFieldStyle({ color: 'blue' }); }
    else { Ext.getCmp('combo_UNIT').setFieldStyle({ color: 'black' }); }

}

