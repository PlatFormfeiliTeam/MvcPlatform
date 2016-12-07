function form_ini() {
    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;基础信息</span></h4>'
    }
    var store_notype = Ext.create('Ext.data.JsonStore', {
        fields: ['NUMBER', 'NAME'],
        data: [{ NUMBER: 1, NAME: '订单编号' }, { NUMBER: 2, NAME: '客户编号' }, { NUMBER: 3, NAME: '分单号' }]
    })
    var combo_notype = Ext.create('Ext.form.field.ComboBox', {//编号类型
        width: 75,
        value: 1,
        name: 'NUMBERTYPE',
        store: store_notype,
        displayField: 'NAME',
        valueField: 'NUMBER',
        triggerAction: 'all',
        forceSelection: true,
        queryMode: 'local',
        listeners: { focus: function () { combo_notype.expand(); } }
    })
    var tbar_r = '<div class="btn-group fl" role="group">'
              + '<button type="button" onclick="LoadOrderFromErp(31)" class="btn btn-primary btn-sm"><i class="fa fa-exchange fa-fw"></i>&nbsp;调用ERP</button>'
              + '<button type="button" onclick="LoadOrder()" class="btn btn-primary btn-sm"><i class="fa fa-search"></i>&nbsp;查询</button></div>';

    var field_noinput = Ext.create('Ext.form.field.Text', {
        name: 'NUMBER',
        id: 'NUMBER',
        tabIndex: 1
    })
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo, '->', combo_notype, field_noinput, tbar_r]
    })

    var field_CODE = Ext.create('Ext.form.field.Text', {//订单编号
        id: 'field_CODE',
        name: 'CODE',
        fieldLabel: '订单编号',
        readOnly: true,
        emptyText: '订单号自动生成'
    })

    var store_ENTRUSTTYPENAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: wtlx_js_data
    })

    var combo_ENTRUSTTYPENAME = Ext.create('Ext.form.field.ComboBox', {//委托类型
        id: 'combo_ENTRUSTTYPENAME',
        name: 'ENTRUSTTYPE',
        hideTrigger: true,
        store: store_ENTRUSTTYPENAME,
        fieldLabel: '委托类型',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 2,
        queryMode: 'local',
        anyMatch: true,
        listeners: {
            focus: function () { if (combo_ENTRUSTTYPENAME.getValue() == null) { combo_ENTRUSTTYPENAME.expand() }; },
            select: function (field, newValue) {
                wtlx_control();//委托类型对其他字段的控制
            }
        },
        allowBlank: false,
        blankText: '委托类型不能为空!'
    })

    var store_REPWAYNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbfs
    })
    var combo_REPWAYNAME = Ext.create('Ext.form.field.ComboBox', {//申报方式
        id: 'combo_REPWAYNAME',
        name: 'REPWAYID',
        store: store_REPWAYNAME,
        hideTrigger: true,
        fieldLabel: '申报方式',
        displayField: 'NAME',
        valueField: 'CODE',
        labelAlign: 'right',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 3,
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
        blankText: '申报方式不能为空!'
    })

    var store_CUSTOMDISTRICTNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbgq
    })

    var field_CUSTOMDISTRICTNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'CUSTOMDISTRICTNAME'
    })

    var combo_CUSTOMDISTRICTNAME = Ext.create('Ext.form.field.ComboBox', {//申报关区 这个数据比较多需要根据输入字符到后台动态模糊匹配
        name: 'CUSTOMAREACODE',
        store: store_CUSTOMDISTRICTNAME,
        fieldLabel: '申报关区',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        hideTrigger: true,
        forceSelection: true,
        anyMatch: true,
        minChars: 2,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                }
            }
        },
        tabIndex: 4,
        allowBlank: false,
        blankText: '申报关区不能为空!',
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    //报关申报单位
    var tf_bgsbdw = Ext.create('Ext.form.field.Text', {
        id: 'tf_bgsbdw',
        readOnly: true,
        name: 'REPUNITCODE',
        margin: 0,
        flex: .85, allowBlank: false,
        blankText: '报关单位不能为空!'
    })
    var cont_bgsbdw = Ext.create('Ext.form.FieldContainer', {
        id: 'cont_bgsbdw',
        fieldLabel: '报关申报单位',
        layout: 'hbox',
        items: [tf_bgsbdw, {
            id: 'bgsbdw_btn', xtype: 'button', handler: function () { bgsbdw_win(tf_bgsbdw); },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    })

    var store_DECLWAY = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_bgfs
    })
    var combo_DECLWAY = Ext.create('Ext.form.field.ComboBox', {//报关方式
        name: 'DECLWAY',
        hideTrigger: true,
        store: store_DECLWAY,
        fieldLabel: '报关方式',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 6,
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
        blankText: '报关方式不能为空!'
    })
    //委托人员id
    var field_SUBMITUSERID = Ext.create('Ext.form.field.Hidden', {
        id: 'field_SUBMITUSERID',
        name: 'SUBMITUSERID'
    });

    var field_SUBMITUSERNAME = Ext.create('Ext.form.field.Text', {//委托人员
        name: 'SUBMITUSERNAME',
        fieldLabel: '委托人员',
        readOnly: true
    });

    var field_SUBMITTIME = Ext.create('Ext.form.field.Text', {//委托时间
        name: 'SUBMITTIME',
        fieldLabel: '委托时间',
        readOnly: true
    });
    //报检申报单位
    var tf_bjsbdw = Ext.create('Ext.form.field.Text', {
        id: 'tf_bjsbdw',
        readOnly: true,
        name: 'INSPUNITCODE',
        margin: 0,
        flex: .85,
        allowBlank: false,
        blankText: '报检单位不能为空!'
    })
    var cont_bjsbdw = Ext.create('Ext.form.FieldContainer', {
        id: 'cont_bjsbdw',
        fieldLabel: '报检申报单位',
        layout: 'hbox',
        items: [tf_bjsbdw, { id: 'bjsbdw_btn', xtype: 'button', listeners: { click: function () { bjsbdw_win(tf_bjsbdw); } }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0 }]
    })

    var field_CREATEUSERNAME = Ext.create('Ext.form.field.Text', {//维护人员
        name: 'CREATEUSERNAME',
        fieldLabel: '维护人员',
        readOnly: true
    });
    var field_CREATETIME = Ext.create('Ext.form.field.Text', {//维护时间
        id: 'field_CREATETIME',
        name: 'CREATETIME',
        fieldLabel: '维护时间',
        readOnly: true
    });
    //业务状态
    var store_status = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: orderstatus_js_data
    })
    var field_STATUS = Ext.create('Ext.form.field.ComboBox', {//业务状态
        id: 'field_STATUS',
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
    var label_busiinfo = {
        columnWidth: .70,
        xtype: 'label',
        margin: '0 0 5 5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;业务信息</span></h4>'
    }
    var chk_2 = Ext.create('Ext.form.field.Checkbox', {
        fieldLabel: '价格影响确认',
        tabIndex: -1,
        readOnly: true,
        name: 'PRICEIMPACT'
    });
    var chk_spe_relation = Ext.create('Ext.form.field.Checkbox', {
        fieldLabel: '特殊关系确认',
        tabIndex: -1,
        name: 'SPECIALRELATIONSHIP',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue == true) {
                    chk_2.setReadOnly(false);
                } else {
                    chk_2.setValue(false);
                    chk_2.setReadOnly(true);
                }
            }
        }
    });
    var chk_3 = Ext.create('Ext.form.field.Checkbox', {
        labelWidth: 125,
        fieldLabel: '支付特许权使用费确认',
        tabIndex: -1,
        name: 'PAYPOYALTIES'
    });
    var chk_container = {
        columnWidth: .30,
        border: 2,
        height: 25,
        style: {
            borderColor: '#e9b477',
            borderStyle: 'solid'
        },
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [chk_spe_relation, chk_2, chk_3]
    }

    var field_CUSNO = Ext.create('Ext.form.field.Text', {//客户编号
        id: 'CUSNO',
        name: 'CUSNO',
        tabIndex: 9,
        fieldLabel: '客户编号'
    });

    //进口口岸
    var store_PORTNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbgq
    })
    var combo_PORTCODE = Ext.create('Ext.form.field.ComboBox', {//和申报关区一样 这个数据比较多需要根据输入字符到后台动态模糊匹配
        name: 'PORTCODE',
        store: store_PORTNAME,
        fieldLabel: '进口口岸',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        minChars: 2,
        hideTrigger: true,
        anyMatch: true,
        forceSelection: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                }
            },
            select: function (records) { field_PORTNAME.setValue(records.rawValue.substr(0, records.rawValue.lastIndexOf('('))); }
        },
        tabIndex: 10,
        allowBlank: false,
        blankText: '进口口岸不能为空!',
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })

    var field_PORTNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'PORTNAME'
    })

    //航次号LADINGBILLNO 变更为航次号 FILGHTNO by panhuaguo 2016-4-18
    var flightno = Ext.create('Ext.form.field.Text', {
        id: 'FILGHTNO',
        name: 'FILGHTNO',
        tabIndex: 12,
        enforceMaxLength: true,
        maxLength: 11,
        fieldLabel: '航次号'
    });
    var field_DIVIDENO = Ext.create('Ext.form.field.Text', {
        name: 'DIVIDENO',
        tabIndex: 13,
        fieldLabel: '分单号'
    });

    //经营单位combostore
    store_jydw = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_jydw
    })
    var combo_jydw = Ext.create('Ext.form.field.ComboBox', {//经营单位 这个数据比较多需要根据输入字符到后台动态模糊匹配,如果取不到点击添加按钮从总库进行选择，同时添加到自有客户库
        name: 'BUSIUNITCODE',
        store: store_jydw,
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        margin: 0,
        forceSelection: true,
        tabIndex: 11,
        anyMatch: true,
        hideTrigger: true,
        minChars: 4,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                }
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
    var field_BUSIUNITNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'BUSIUNITNAME'
    })
    var store_PACKKINDNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_bzzl
    })

    var combo_PACKKINDNAME = Ext.create('Ext.form.field.ComboBox', {//包装类型
        name: 'PACKKIND',
        hideTrigger: true,
        store: store_PACKKINDNAME,
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 15,
        margin: 0,
        flex: .5,
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
        listConfig: {
            maxHeight: 110
        }
    })

    //件数/包装
    var field_quanpackage = {
        xtype: 'fieldcontainer',
        fieldLabel: '件数/包装',
        layout: 'hbox',
        items: [{
            id: 'GOODSNUM', name: 'GOODSNUM', xtype: 'numberfield', tabIndex: 14, flex: .5, margin: 0, hideTrigger: true
        }, combo_PACKKINDNAME]
    }
    var field_weight = { //毛重/净重
        xtype: 'fieldcontainer',
        fieldLabel: '毛重/净重',
        layout: 'hbox',
        items: [{ id: 'GOODSGW', name: 'GOODSGW', xtype: 'numberfield', flex: .5, msgTarget: 'qtip', tabIndex: 16, margin: 0, allowBlank: false, blankText: '不能为空!', hideTrigger: true, decimalPrecision: 4 },
                { id: 'GOODSNW', name: 'GOODSNW', xtype: 'numberfield', flex: .5, msgTarget: 'qtip', tabIndex: 17, margin: 0, hideTrigger: true, decimalPrecision: 4 }]
    }

    //合同号
    var field_contractno = Ext.create('Ext.form.field.Text', {
        fieldLabel: '合同号',
        tabIndex: 18,
        name: 'CONTRACTNO'
    });

    //载货清单号
    var field_MANIFEST = Ext.create('Ext.form.field.Text', {
        tabIndex: 20,
        fieldLabel: '载货清单号',
        name: 'MANIFEST'
    });

    var store_myfs = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_myfs
    })
    //贸易方式可能多选
    var combo_myfs = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_myfs',
        name: 'TRADEWAYCODES',
        store: store_myfs,
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        anyMatch: true,
        forceSelection: true,
        tabIndex: 19,
        hideTrigger: true,
        multiSelect: false,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                }
            }
        },
        flex: 0.85,
        margin: 0,
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
      , allowBlank: false,
        blankText: '贸易方式不能为空!'
    })
    //贸易方式
    var field_myfs = {
        xtype: 'fieldcontainer',
        fieldLabel: '贸易方式',
        layout: 'hbox',
        items: [combo_myfs, {
            id: 'myfs_btn', xtype: 'button', handler: function () {
                selectmyfs(combo_myfs, field_ORDERREQUEST);
            },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }

    var store_mzbz = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_mzbz
    })

    var combo_mzbz = Ext.create('Ext.form.field.ComboBox', {//木质包装
        name: 'WOODPACKINGID',
        hideTrigger: true,
        store: store_mzbz,
        fieldLabel: '木质包装',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 21,
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
        listConfig: {
            maxHeight: 110
        },
        allowBlank: false,
        blankText: '木质包装不能为空!',
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })

    var field_CLEARANCENO = Ext.create('Ext.form.field.Text', {
        id: 'field_CLEARANCENO',
        tabIndex: 22,
        fieldLabel: '通关单号',
        name: 'CLEARANCENO'
    });
    //报关车号
    var field_CONTAINERTRUCK = Ext.create('Ext.form.field.Hidden', {
        id: 'field_CONTAINERTRUCK',
        name: 'CONTAINERTRUCK'
    })
    //报关车号 
    var field_bgch = Ext.create('Ext.form.field.Text', {
        name: 'DECLCARNO',
        id: 'DECLCARNO',
        tabIndex: 25,
        readOnly: true,
        allowBlank: true,
        margin: 0,
        flex: 0.85
    })
    var container_bgch = Ext.create('Ext.form.FieldContainer', {
        id: 'fieldbgch',
        fieldLabel: '报关车号',
        layout: 'hbox',
        items: [field_bgch, {
            id: 'bgch_button',
            xtype: 'button', handler: function () {
                win_container_truck.show();
            }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    })

    var chk_CHKLAWCONDITION = {
        xtype: 'checkboxfield',
        tabIndex: 23,
        fieldLabel: '法检标志',
        name: 'LAWFLAG'
    }

    //集装箱号
    var combo_containerno = Ext.create('Ext.form.field.Text', {
        xtype: 'textfield',
        name: 'CONTAINERNO',
        id: 'CONTAINERNO',
        tabIndex: 24,
        readOnly: true,
        allowBlank: true,
        margin: 0,
        flex: 0.85
    })
    var field_containerno = Ext.create('Ext.form.FieldContainer', {
        id: 'fieldcontainer1',
        fieldLabel: '集装箱号',
        layout: 'hbox',
        items: [combo_containerno, {
            id: 'container_btn', xtype: 'button', handler: function () {
                win_container_truck.show();
            },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    })
    //集装箱号
    var field_ContainerJson = Ext.create('Ext.form.field.Hidden', {
        name: 'ContainerJson',
        id: 'ContainerJson'
    })

    //需求备注
    var field_ORDERREQUEST = Ext.create('Ext.form.field.Text', {
        id: 'field_ORDERREQUEST',
        tabIndex: 26,
        fieldLabel: '需求备注',
        name: 'ORDERREQUEST'
    });

    //隐藏的字段
    var field_ID = Ext.create('Ext.form.field.Hidden', {
        name: 'ID'
    });

    var field_ORIGINALFILEIDS = Ext.create('Ext.form.field.Hidden', {
        id: 'field_ORIGINALFILEIDS',
        name: 'ORIGINALFILEIDS'
    });
    //单证服务单位
    var store_dzfwdw = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_dzfwdw
    })
    var combo_dzfwdw = Ext.create('Ext.form.field.ComboBox', {
        name: 'DOCSERVICECODE',
        store: store_dzfwdw,
        hideTrigger: true,
        fieldLabel: '单证服务单位',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        editable: false,
        value: 'GWYKS',
        queryMode: 'local',
        labelWidth: 80
    })
    //文件列表
    var bbar_r = '<div class="btn-group" role="group">'
                        + '<button type="button" onclick="orderBack();" id="btn_cancelsubmit" class="btn btn-primary btn-sm"><i class="fa fa-angle-double-left"></i>&nbsp;撤单</button>'
                        + '<button type="button" onclick="add_new(31)" class="btn btn-primary btn-sm"><i class="fa fa-plus fa-fw"></i>&nbsp;新增</button>'
                        + '<button type="button" onclick="copyorder(31)" class="btn btn-primary btn-sm"><i class="fa fa-files-o"></i>&nbsp;复制新增</button>'
                        + '<button type="button" onclick="save(\'save\',31)" class="btn btn-primary btn-sm" disabled><i class="fa fa-floppy-o"></i>&nbsp;保存</button>'
                        + '<button type="button" onclick="save(\'submit\',31)" id="btn_submitorder" class="btn btn-primary btn-sm"><i class="fa fa-hand-o-up"></i>&nbsp;提交委托</button></div>'
    var bbar_l;
    if (cur_usr.NAME == 'flddz001' || cur_usr.NAME == 'flddz002' || cur_usr.NAME == 'flddz003' || cur_usr.NAME == 'flddz004') {
        bbar_l = '<div class="btn-group">'
                      + '<button type="button" class="btn btn-primary btn-sm" id="pickfiles"><i class="fa fa-upload"></i>&nbsp;上传文件</button>'
                      + '<button type="button" onclick="browsefile()" class="btn btn-primary btn-sm"><i class="fa fa-exchange fa-fw"></i>&nbsp;浏览文件</button>'
                      + '<button type="button" onclick="removeFile()" class="btn btn-primary btn-sm" id="deletefile"><i class="fa fa-trash-o"></i>&nbsp;删除文件</button>'
                      + '<button type="button" onclick="printFile()" class="btn btn-primary btn-sm"><i class="fa fa-print"></i>&nbsp;打印文件</button>'
                  + '</div>';
    }
    else
    {
        bbar_l = '<div class="btn-group">'
               + '<button type="button" class="btn btn-primary btn-sm" id="pickfiles"><i class="fa fa-upload"></i>&nbsp;上传文件</button>'
               + '<button type="button" onclick="browsefile()" class="btn btn-primary btn-sm"><i class="fa fa-exchange fa-fw"></i>&nbsp;浏览文件</button>'
               + '<button type="button" onclick="removeFile()" class="btn btn-primary btn-sm" id="deletefile"><i class="fa fa-trash-o"></i>&nbsp;删除文件</button>'
           + '</div>';

    }

    var store_filetype = Ext.create('Ext.data.JsonStore', {
        fields: ['FILETYPEID', 'FILETYPENAME'],
        data: [{ FILETYPEID: '44', FILETYPENAME: '订单文件' },
               { FILETYPEID: '58', FILETYPENAME: '配舱单文件' }]

    })
    var combo_filetype = Ext.create('Ext.form.field.ComboBox', {//文件类型
        id: 'combo_filetype',
        name: 'FILETYPEID',
        store: store_filetype,
        fieldLabel: '文件类型',
        displayField: 'FILETYPENAME',
        valueField: 'FILETYPEID',
        editable: false,
        queryMode: 'local',
        labelWidth: 60,
        width: 150,
        value: '44'
    })

    //var field_FILETYPENAME = Ext.create('Ext.form.field.Hidden', {
    //    name: 'FILETYPENAME'
    //})
    var field_fileno1 = Ext.create('Ext.form.field.Text', {
        id: 'field_fileno1',
        labelWidth: 55,
        fieldLabel: '统一编号:',
        listeners: {
            specialkey: function (field, e) {
                // e.HOME, e.END, e.PAGE_UP, e.PAGE_DOWN,e.TAB, e.ESC, arrow keys: e.LEFT, e.RIGHT, e.UP, e.DOWN 
                if (e.getKey() == e.ENTER) {
                    var fileuniteno = field.getValue();
                    Ext.Ajax.request({
                        url: "/Common/LoadEnterpriseFile",
                        params: { fileuniteno: fileuniteno },
                        success: function (response, option) {
                            var data = Ext.decode(response.responseText);
                            if (data.success) {
                                Ext.MessageBox.alert("提示", "文件加载成功！", function () {
                                    field.setValue("");
                                    var filetype = Ext.getCmp('combo_filetype').getValue();
                                    var filetypename = Ext.getCmp('combo_filetype').getRawValue();
                                    var timestamp = Ext.Date.now();  //1351666679575  这个方法只是获取的时间戳
                                    var date = new Date(timestamp);

                                    Ext.each(data.data, function (item) {
                                        file_store.insert(file_store.data.length,
                                       { FILENAME: '/FileUpload/file/' + item.ORIGINALNAME, ORIGINALNAME: item.ORIGINALNAME, SIZES: item.SIZES, FILETYPENAME: filetypename, FILETYPE: filetype, UPLOADTIME: Ext.Date.format(date, 'Y-m-d H:i:s') });
                                    })
                                });
                            }
                            else {
                                Ext.MessageBox.alert("提示", "文件加载失败！");
                            }
                        }

                    });

                }
            }
        }
    });
    var bbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [combo_filetype, field_fileno1, bbar_l, '->', bbar_r]
    })
    formpanel = Ext.create('Ext.form.Panel', {
        renderTo: 'div_form',
        border: 0,
        minHeight: 350,
        tbar: tbar,
        bbar: bbar,
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
    { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_CODE, combo_ENTRUSTTYPENAME, combo_REPWAYNAME, combo_CUSTOMDISTRICTNAME, cont_bgsbdw] },
    { layout: 'column', height: 42, border: 0, items: [combo_DECLWAY, field_SUBMITTIME, field_SUBMITUSERNAME, field_STATUS, cont_bjsbdw] },
    { layout: 'column', height: 42, border: 0, items: [field_CREATEUSERNAME, field_CREATETIME, combo_dzfwdw] },
    { layout: 'column', border: 42, border: 0, items: [label_busiinfo, chk_container] },
    { layout: 'column', height: 42, border: 0, items: [field_CUSNO, combo_PORTCODE, field_jydw, flightno, field_DIVIDENO] },
    { layout: 'column', height: 42, border: 0, items: [field_quanpackage, field_weight, field_contractno, field_myfs, field_MANIFEST] },
    { layout: 'column', height: 42, border: 0, items: [combo_mzbz, field_CLEARANCENO, chk_CHKLAWCONDITION, field_containerno, container_bgch] },
    { layout: 'column', height: 42, border: 0, items: [field_ORDERREQUEST] },
    field_CUSTOMDISTRICTNAME, field_PORTNAME, field_BUSIUNITNAME, field_ID, field_ContainerJson, field_ORIGINALFILEIDS, field_CONTAINERTRUCK, field_SUBMITUSERID
        ]
    });
}