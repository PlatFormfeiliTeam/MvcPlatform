function form_ini() {
    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;业务信息</span></h4>'
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
    //委托类型
    var store_ENTRUSTTYPENAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: wtlx_js_data_CustomsService
    });
    var combo_ENTRUSTTYPENAME = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ENTRUSTTYPENAME',
        name: 'ENTRUSTTYPE',
        hideTrigger: true,
        store: store_ENTRUSTTYPENAME,
        fieldLabel: '委托类型',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 1,
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
            select: function (field, newValue) {
                wtlx_control();//委托类型对其他字段的控制
            }
        },
        allowBlank: false,
        blankText: '委托类型不能为空!'
    });   


    var field_CUSTOMERNAME = Ext.create('Ext.form.field.Hidden', { name: 'CUSTOMERNAME' });
    var field_CLEARUNITNAME = Ext.create('Ext.form.field.Hidden', { name: 'CLEARUNITNAME' });

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
    var field_BUSIUNITNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'BUSIUNITNAME'
    });

    //经营单位
    var field_jydw = {
        xtype: 'fieldcontainer',
        fieldLabel: '经营单位',
        layout: 'hbox',
        items: [combo_jydw, {
            id: 'jydw_btn', xtype: 'button', handler: function () {
                selectjydw(combo_jydw, field_BUSIUNITNAME);
            },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
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
        fieldLabel: '结算单位', forceSelection: true,
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

    //企业编号
    var field_CUSNO = Ext.create('Ext.form.field.Text', {
        id: 'CUSNO',
        name: 'CUSNO',
        tabIndex: 5,
        fieldLabel: '企业编号'
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

    //维护人员
    var field_SUBMITUSERNAME = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITUSERNAME',
        fieldLabel: '维护人员',
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //维护时间
    var field_SUBMITTIME = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITTIME',
        fieldLabel: '维护时间',
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    //操作需求
    var field_DOREQUEST = Ext.create('Ext.form.field.Text', {
        id: 'field_DOREQUEST',
        tabIndex: 23, flex: 1, margin: 0, tabIndex: 6,
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
        tabIndex: 23, flex: 1, margin: 0, tabIndex: 7,
        name: 'CLEARREMARK'
    });
    var container_CLEARREMARK = {
        columnWidth: 1,
        xtype: 'fieldcontainer',
        layout: 'hbox',
        fieldLabel: '结算备注',
        items: [field_CLEARREMARK]
    }
    var field_ORIGINALCOSTIDS = Ext.create('Ext.form.field.Hidden', {
        id: 'field_ORIGINALCOSTIDS',
        name: 'ORIGINALCOSTIDS'
    });
    
    formpanel = Ext.create('Ext.form.Panel', {
        id: "formpanel_id",
        renderTo: 'div_form',
        minHeight: 150,
        border: 0,
        tbar: tbar,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: .20,
            labelAlign: 'right',
            labelSeparator: '',
            msgTarget: 'under',
            validateOnBlur: false,
            validateOnChange: false
        },
        items: [
        { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_CODE, combo_ENTRUSTTYPENAME, combo_wtdw, field_jydw, combo_jsdw] },
        { layout: 'column', height: 42, border: 0, items: [field_CUSNO, field_CREATEUSERNAME, field_CREATETIME, field_SUBMITUSERNAME, field_SUBMITTIME] },
        { layout: 'column', height: 42, border: 0, items: [container_DOREQUEST] },
        { layout: 'column', height: 42, border: 0, items: [container_CLEARREMARK] },
        field_BUSIUNITNAME, field_CUSTOMERNAME, field_CLEARUNITNAME, field_ORIGINALCOSTIDS
        ]
    });
}

function form_ini_con() {
    var rownum = -1;//记录当前编辑的行号

    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;费用明细</span></h4>'
    }
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo, '->']
    })

    //================================================================

    //费用类别
    var store_fylb = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: fylb_js_data
    })
    var combo_fylb = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_fylb',
        name: 'FEECODE',
        store: store_fylb,
        hideTrigger: true,
        fieldLabel: '费用类别', forceSelection: true,
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
            select: function (records) {
                field_FEENAME.setValue(records.rawValue);
            },
            change: function (combo, newValue, oldValue, eOpts) {
                field_FEENAME.setValue(combo.rawValue);
            }
        },
        allowBlank: false,
        blankText: '费用类别不能为空!'
    });

    var field_FEENAME = Ext.create('Ext.form.field.Hidden', { id: 'field_FEENAME', name: 'FEENAME' });

    //金额
    var field_COST = Ext.create('Ext.form.field.Number', {
        id: 'COST',
        name: 'COST',
        fieldLabel: '金额', hideTrigger: true, //decimalPrecision: 2,
        allowBlank: false,
        blankText: '金额不能为空!',
        listeners: {
            "specialkey": function (field, e) {
                if (e.keyCode == 13) {
                    if (!formpanel_con.getForm().isValid()) {
                        return;
                    }
                    if (Number(field_COST.getValue()) <= 0) {
                        Ext.MessageBox.alert("提示", "金额必须大于0！");
                        return;
                    }
                    var formdata = formpanel_con.getForm().getValues();
                    if (rownum < 0) {//添加模式
                        store_COSTDATA.insert(store_COSTDATA.data.length, formdata);
                    }
                    else {//修改模式
                        var rec = store_COSTDATA.getAt(rownum);
                        store_COSTDATA.remove(rec);
                        store_COSTDATA.insert(rownum, formdata);
                    }
                    formpanel_con.getForm().reset();
                    rownum = -1;
                    Ext.getCmp('btn_mode').setText('<span style="color:blue">新增模式</span>');
                }
            }
        }
    });

    //费用状态
    var store_status = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_fyzt
    })
    var field_STATUS = Ext.create('Ext.form.field.ComboBox', {
        id: 'field_STATUS',
        name: 'STATUS',
        valueField: 'CODE',
        displayField: 'NAME',
        fieldLabel: '费用状态',
        queryMode: 'local',
        editable: false,
        hiddenTrigger: true,
        readOnly: true,
        value: 10,
        store: store_status,
        listeners: {
            change: function (combo, newValue, oldValue, eOpts) {
                field_STATUSNAME.setValue(combo.rawValue);
            }
        },
    });
    var field_STATUSNAME = Ext.create('Ext.form.field.Hidden', { id: 'field_STATUSNAME', name: 'STATUSNAME', value: field_STATUS.rawValue });

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
        items: [
            { layout: 'column', height: 42, border: 0, margin: '5 0 0 0', items: [combo_fylb, field_COST, field_STATUS] }
            , field_FEENAME, field_STATUSNAME
        ]
    });
    //=================================================
    var data_COSTDATA = [];
    var store_COSTDATA = Ext.create('Ext.data.JsonStore', {
        storeId: 'store_COSTDATA',
        fields: ['ORDERCODE', 'ID', 'FEECODE', 'FEENAME', 'COST', 'STATUS', 'STATUSNAME'],
        data: data_COSTDATA
    });

    var w_tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: ['<span style="color:red">说明：双击列表项可对已添加的记录进行修改</span>',
                {
                    text: '<span style="color:blue">新增模式</span>', id: 'btn_mode', handler: function () {
                        if (Ext.getCmp('btn_mode').getText() == '<span style="color:blue">编辑模式</span>') {
                            Ext.getCmp('btn_mode').setText('<span style="color:blue">新增模式</span>');
                            rownum = -1;
                        }
                    }
                },
               '->',
               /*{
                   text: '<span class="icon iconfont" style="font-size:10px">&#xe622;</span>&nbsp;保 存', id: 'btn_pro_save',
                   handler: function () {
                       if (!formpanel_con.getForm().isValid()) {
                           return;
                       }
                       if (Number(field_COST.getValue()) <= 0) {
                           Ext.MessageBox.alert("提示", "金额必须大于0！");
                           return;
                       }
                       var formdata = formpanel_con.getForm().getValues();
                       if (rownum < 0) {//添加模式
                           store_COSTDATA.insert(store_COSTDATA.data.length, formdata);
                       }
                       else {//修改模式
                           var rec = store_COSTDATA.getAt(rownum);
                           store_COSTDATA.remove(rec);
                           store_COSTDATA.insert(rownum, formdata);
                       }
                       formpanel_con.getForm().reset();
                       rownum = -1;
                       Ext.getCmp('btn_mode').setText('<span style="color:blue">新增模式</span>');
                   }
               },*/
               {
                   text: '<span class="icon iconfont" style="font-size:10px">&#xe6d3;</span>&nbsp;移 除', id: 'btn_pro_del',
                   handler: function () {
                       var recs = gridpanel_costdata.getSelectionModel().getSelection();
                       if (recs.length == 0) {
                           Ext.MessageBox.alert('提示', '请选择需要移除的记录！');
                           return;
                       }

                       Ext.MessageBox.confirm("提示", "确定要移除所选择的记录吗？", function (btn) {
                           if (btn == 'yes') {
                               if (recs[0].data.STATUS > 10) {
                                   Ext.MessageBox.alert("提示", "只能移除费用状态为<span style='color:blue'>生成费用</span>的记录！");
                                   return;
                               }

                               store_COSTDATA.remove(recs);
                               Ext.getCmp('btn_mode').setText('<span style="color:blue">新增模式</span>');
                               rownum = -1;
                           }
                       });
                   }
               }]
    });
    var gridpanel_costdata = Ext.create('Ext.grid.Panel', {
        id: 'gridpanel_costdata',
        renderTo: 'div_form_con',
        store: store_COSTDATA,
        minHeight: 150,
        selModel: { selType: 'checkboxmodel' },
        enableColumnHide: false,
        tbar: w_tbar,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: 'ID', dataIndex: 'ID', hidden: true },
        { header: 'ORDERCODE', dataIndex: 'ORDERCODE', hidden: true },
        { header: 'FEECODE', dataIndex: 'FEECODE', hidden: true },
        { header: '费用名称', dataIndex: 'FEENAME', width: 130 },
        { header: '金额', dataIndex: 'COST', width: 130 },
        { header: 'STATUS', dataIndex: 'STATUS', hidden: true },
        { header: '费用状态', dataIndex: 'STATUSNAME', width: 130 }
        ],
        listeners: {
            itemdblclick: function (w_grid, record, item, index, e, eOpts) {
                rownum = index;
                formpanel_con.getForm().setValues(record.data);
                Ext.getCmp('btn_mode').setText('<span style="color:blue">编辑模式</span>');
            }
        },
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });    
}

function form_ini_btn() {

    var bbar_r = '<div class="btn-group" role="group">'
                        + '<button type="button" onclick="create_save()" id="btn_submitorder" class="btn btn-primary btn-sm"><i class="fa fa-hand-o-up"></i>&nbsp;保存</button>'
            + '</div>';

    //var bbar_l = '<div class="btn-group">'
    //             + '<button type="button" onclick="copyadd_save()" class="btn btn-primary btn-sm"><i class="fa fa-plus fa-fw"></i>&nbsp;新增</button>'
    //        + '</div>';
    var bbar = Ext.create('Ext.toolbar.Toolbar', {
        items: ['->', bbar_r]//bbar_l, 
    });

    var formpanel_btn = Ext.create('Ext.form.Panel', {
        renderTo: 'div_form_btn',
        border: 0,
        bbar: bbar
    });
}

function create_save() {    

    if (!Ext.getCmp('formpanel_id').getForm().isValid()) {
        return;
    }

    //var validate = "";        
    //if (Ext.data.StoreManager.lookup('store_COSTDATA').data.items.length <= 0) {
    //    validate += "费用明细为空！<br />";
    //}       

    //if (validate) {
    //    Ext.MessageBox.alert("提示", validate);
    //    return;
    //}    

    var formdata = Ext.encode(Ext.getCmp('formpanel_id').getForm().getValues());
    var costdata = Ext.encode(Ext.pluck(Ext.data.StoreManager.lookup('store_COSTDATA').data.items, 'data'));

    var mask = new Ext.LoadMask(Ext.get(Ext.getBody()), { msg: "数据保存中，请稍等..." });

    mask.show();
    Ext.Ajax.request({
        url: "/OrderCustomsService/Create_Save",
        params: { ordercode: ordercode, formdata: formdata, costdata: costdata },
        success: function (response, option) {
            if (response.responseText) {
                mask.hide();
                var data = Ext.decode(response.responseText);
                if (data.success) {
                    ordercode = data.ordercode;
                    var msg = "保存成功";
                    if (data.detail == "f") {
                        msg = "业务信息保存成功，费用明细保存失败！";
                    }
                    Ext.MessageBox.alert("提示", msg, function () {
                        loadform_CusService();
                    });

                }
                else {
                    Ext.MessageBox.alert("提示", "保存失败！");
                }
            }
        }
    });
}

function loadform_CusService() {
    Ext.Ajax.request({
        url: "/OrderCustomsService/loadrecord_create",
        params: { ordercode: ordercode },
        success: function (response, opts) {
            var data = Ext.decode(response.responseText);

            Ext.getCmp("formpanel_id").getForm().setValues(data.formdata);
            Ext.getCmp('gridpanel_costdata').store.loadData(data.costdata);

            var fileids = "";
            Ext.each(data.costdata, function (rec) {
                fileids += rec["ID"] + ",";
            });
            Ext.getCmp('field_ORIGINALCOSTIDS').setValue(fileids);
           
        }
    });
}