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
        listeners: { focus: function () { combo_notype.expand(); } },
    })

    //是否显示“调用ERP按钮”

    var tbar_r = '<div class="btn-group fl" role="group">'
           + '<button type="button" onclick="LoadOrderFromErp(11)" class="btn btn-primary btn-sm"><i class="fa fa-exchange fa-fw"></i>&nbsp;调用ERP</button>'
           + '<button type="button" onclick="LoadOrder()" class="btn btn-primary btn-sm"><i class="fa fa-search"></i>&nbsp;查询</button></div>';

    var field_noinput = Ext.create('Ext.form.field.Text', {
        name: 'NUMBER',
        id: 'NUMBER',
        tabIndex: 1
    });
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo, '->', combo_notype, field_noinput, tbar_r]
    })
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
        data: wtlx_js_data
    })
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
        tabIndex: 2,
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
    })
    //申报方式
    var store_REPWAYNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbfs
    })
    var combo_REPWAYNAME = Ext.create('Ext.form.field.ComboBox', {
        name: 'REPWAYID',
        store: store_REPWAYNAME,
        hideTrigger: true,
        fieldLabel: '申报方式',
        displayField: 'NAME',
        valueField: 'CODE',
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
            //select: function (field, newValue) {
            //    LoadButtonStyle();
            //}
        },
        allowBlank: false,
        blankText: '申报方式不能为空!'
    })
    //申报关区
    var store_CUSTOMDISTRICTNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbgq
    })
    var combo_CUSTOMDISTRICTNAME = Ext.create('Ext.form.field.ComboBox', {//申报关区 这个数据比较多需要根据输入字符到后台动态模糊匹配
        name: 'CUSTOMAREACODE',
        store: store_CUSTOMDISTRICTNAME,
        fieldLabel: '申报关区',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        minChars: 2,
        hideTrigger: true,
        forceSelection: true,
        anyMatch: true,
        tabIndex: 4,
        allowBlank: false,
        blankText: '申报关区不能为空!',
        listeners: {
            focus: function (cb) {
                cb.clearInvalid();
            }
        },
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    var field_CUSTOMDISTRICTNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'CUSTOMDISTRICTNAME'
    })
    //报关申报单位
    var tf_bgsbdw = Ext.create('Ext.form.field.Text', {
        id: 'tf_bgsbdw',
        readOnly: true,
        name: 'REPUNITCODE',
        margin: 0,
        flex: .85,
    })
    var cont_bgsbdw = Ext.create('Ext.form.FieldContainer', {
        fieldLabel: '报关申报单位',
        id: 'cont_bgsbdw',
        layout: 'hbox',
        items: [tf_bgsbdw, {
            id: 'bgsbdw_btn', xtype: 'button', handler: function () { bgsbdw_win(tf_bgsbdw); },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    })
    //报关方式
    var store_DECLWAY = Ext.create('Ext.data.JsonStore', {
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
            },
            beforequery: function (e) {
                var queryItem = e.query;
                var i;
                var result = '';
                for (i = 0; i < queryItem.length; i++) {
                    code = queryItem.charCodeAt(i);
                    if (code >= 65281 && code < 65373)
                        result += String.fromCharCode(queryItem.charCodeAt(i) - 65248);
                    else result += queryItem.charAt(i);
                }
                e.query = result;
            }
        },
        allowBlank: false,
        blankText: '报关方式不能为空!',
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    //委托人员
    var field_SUBMITUSERNAME = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITUSERNAME',
        fieldLabel: '委托人员',
        readOnly: true
    });
    //委托时间
    var field_SUBMITTIME = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITTIME1',
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
    })
    var cont_bjsbdw = Ext.create('Ext.form.FieldContainer', {
        fieldLabel: '报检申报单位',
        id: 'cont_bjsbdw',
        layout: 'hbox',
        items: [tf_bjsbdw, {
            id: 'bjsbdw_btn', xtype: 'button', handler: function () { bjsbdw_win(tf_bjsbdw); },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    })
    //维护人员
    var field_CREATEUSERNAME = Ext.create('Ext.form.field.Text', {
        name: 'CREATEUSERNAME',
        fieldLabel: '维护人员',
        readOnly: true
    });
    //维护时间
    var field_CREATETIME = Ext.create('Ext.form.field.Text', {
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
        value: 1,
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
    //客户编号
    var field_CUSNO = Ext.create('Ext.form.field.Text', {
        id: 'CUSNO',
        name: 'CUSNO',
        tabIndex: 8,
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
        hideTrigger: true,
        anyMatch: true,
        minChars: 2,
        forceSelection: true,
        listeners: {
            focus: function (cb) {
                cb.clearInvalid();
            },
            select: function (records) { field_PORTNAME.setValue(records.rawValue.substr(0, records.rawValue.lastIndexOf('('))); }
        },
        tabIndex: 9,
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
    var store_jydw = Ext.create('Ext.data.JsonStore', {  //经营单位combostore
        fields: ['CODE', 'NAME', 'QUANCODE', 'QUANNAME', 'SHORTNAME'],
        data: common_data_jydw
    })
    var combo_jydw = Ext.create('Ext.form.field.ComboBox', {//经营单位 这个数据比较多需要根据输入字符到后台动态模糊匹配,如果取不到点击添加按钮从总库进行选择，同时添加到自有客户库
        id: 'combo_jydw',
        name: 'BUSIUNITCODE',
        store: store_jydw,
        displayField: 'NAME',
        valueField: 'QUANCODE',
        queryMode: 'local',
        margin: 0,
        minChars: 2,
        forceSelection: true,
        tabIndex: 10,
        anyMatch: true,
        hideTrigger: true,
        listeners: {
            focus: function (cb) {
                cb.clearInvalid();
            },
            select: function (combo, records) {
                field_BUSIUNITNAME.setValue(records[0].get("QUANNAME"));
                field_BUSISHORTCODE.setValue(records[0].get("CODE"));
                field_BUSISHORTNAME.setValue(records[0].get("SHORTNAME"));
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
    })
    var field_BUSISHORTCODE = Ext.create('Ext.form.field.Hidden', {
        id: 'field_BUSISHORTCODE',
        name: 'BUSISHORTCODE'
    })
    var field_BUSISHORTNAME = Ext.create('Ext.form.field.Hidden', {
        id: 'field_BUSISHORTNAME',
        name: 'BUSISHORTNAME'
    })
    //总单号
    var field_TOTALNO = Ext.create('Ext.form.field.Text', {
        name: 'TOTALNO',
        tabIndex: 11,
        enforceMaxLength: true,
        maxLength: 11,
        fieldLabel: '总单号'
    });
    //分单号
    var field_DIVIDENO = Ext.create('Ext.form.field.Text', {
        name: 'DIVIDENO',
        tabIndex: 12,
        enforceMaxLength: true,
        maxLength: 18,
        fieldLabel: '分单号',
        allowBlank: false,
        blankText: '分单号不能为空!'
    });
    //经营单位
    var field_jydw = {
        xtype: 'fieldcontainer',
        fieldLabel: '经营单位',
        layout: 'hbox',
        items: [combo_jydw, {
            id: 'jydw_btn', xtype: 'button', handler: function () {
                selectjydw(combo_jydw, field_BUSIUNITNAME, field_BUSISHORTCODE, field_BUSISHORTNAME);
            },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    //包装combostore
    var store_PACKKINDNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_bzzl
    })
    var combo_PACKKINDNAME = Ext.create('Ext.form.field.ComboBox', {
        name: 'PACKKIND',
        hideTrigger: true,
        store: store_PACKKINDNAME,
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 14,
        queryMode: 'local',
        anyMatch: true,
        margin: 0,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.expand();
                    cb.store.clearFilter();
                }
            }
        },
        flex: .5,
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    //件数/包装
    var field_quanpackage = {
        xtype: 'fieldcontainer',
        fieldLabel: '件数/包装',
        layout: 'hbox',
        items: [{
            id: 'GOODSNUM', name: 'GOODSNUM', xtype: 'numberfield', tabIndex: 13, flex: .5, margin: 0, hideTrigger: true
        }, combo_PACKKINDNAME]
    }
    var field_weight = { //毛重/净重
        xtype: 'fieldcontainer',
        fieldLabel: '毛重/净重',
        layout: 'hbox',
        items: [{ id: 'GOODSGW', name: 'GOODSGW', xtype: 'numberfield', flex: .5, msgTarget: 'qtip', tabIndex: 15, margin: 0, allowBlank: false, blankText: '不能为空!', hideTrigger: true, decimalPrecision: 4 },
                { id: 'GOODSNW', name: 'GOODSNW', xtype: 'numberfield', flex: .5, msgTarget: 'qtip', tabIndex: 16, margin: 0, hideTrigger: true, decimalPrecision: 4 }]
    }
    //合同号
    var field_contractno = Ext.create('Ext.form.field.Text', {
        tabIndex: 17,
        fieldLabel: '合同号',
        name: 'CONTRACTNO'
    });
    var store_myfs = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_myfs
    })
    var combo_myfs = Ext.create('Ext.form.field.ComboBox', {//贸易方式
        id: 'combo_myfs',
        store: store_myfs,
        displayField: 'CODE',
        valueField: 'CODE',
        queryMode: 'local',
        anyMatch: true,
        forceSelection: true,
        tabIndex: 18,
        hideTrigger: true,
        multiSelect: false,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.expand();
                    cb.store.clearFilter();
                }
            },
            select: function (records) {
                field_TRADEWAYCODES1.setValue(records.rawValue);
                field_TRADEWAYCODES.setValue(records.rawValue.substr(0, 4));
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
    var field_TRADEWAYCODES = Ext.create('Ext.form.field.Hidden', {
        name: 'TRADEWAYCODES'
    });
    var field_TRADEWAYCODES1 = Ext.create('Ext.form.field.Hidden', {
        name: 'TRADEWAYCODES1'
    });
    //贸易方式
    var field_myfs = {
        xtype: 'fieldcontainer',
        fieldLabel: '贸易方式',
        layout: 'hbox',
        items: [combo_myfs, {
            id: 'myfs_btn', xtype: 'button', handler: function () {
                selectmyfs(combo_myfs, field_TRADEWAYCODES, field_TRADEWAYCODES1);
            },
            text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    }
    //转关预录号
    var field_TURNPRENO = Ext.create('Ext.form.field.Text', {
        tabIndex: 19,
        fieldLabel: '转关预录号',
        name: 'TURNPRENO',
        enforceMaxLength: true,
        maxLength: 16
    });
    //木质包装
    var store_mzbz = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_mzbz
    });
    var combo_mzbz = Ext.create('Ext.form.field.ComboBox', {
        name: 'WOODPACKINGID',
        store: store_mzbz,
        fieldLabel: '木质包装',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        hideTrigger: true,
        forceSelection: true,
        tabIndex: 20,
        anyMatch: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.expand();
                    cb.store.clearFilter();
                }
            }
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
    //通关单号
    var field_CLEARANCENO = Ext.create('Ext.form.field.Text', {
        id: 'field_CLEARANCENO',
        tabIndex: 21,
        fieldLabel: '通关单号',
        name: 'CLEARANCENO'
    });
    //报关车号
    var field_CONTAINERTRUCK = Ext.create('Ext.form.field.Hidden', {
        id: 'field_CONTAINERTRUCK',
        name: 'CONTAINERTRUCK'
    })
    var field_bgch = Ext.create('Ext.form.field.Text', {
        name: 'DECLCARNO',
        id: 'DECLCARNO',
        tabIndex: 25,
        readOnly: true,
        allowBlank: true,
        margin: 0,
        flex: 0.85
    });
    var container_bgch = Ext.create('Ext.form.FieldContainer', {
        id: 'fieldbgch',
        fieldLabel: '报关车号',
        layout: 'hbox',
        items: [field_bgch, {
            id: 'bgch_button',
            xtype: 'button', id: 'declcarno_btn', handler: function () {
                win_container_truck.show();
            }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    })
    //需求备注
    var field_CLEARREMARK = Ext.create('Ext.form.field.Text', {
        tabIndex: 23,
        fieldLabel: '需求备注',
        name: 'ENTRUSTREQUEST'
    });
    //法检状况
    var chk_CHKLAWCONDITION = {
        xtype: 'checkboxfield',
        tabIndex: 24,
        fieldLabel: '法检状况',
        name: 'LAWCONDITION'
    }
    //隐藏的字段
    field_ID = Ext.create('Ext.form.field.Hidden', {
        name: 'ID'
    });
    var field_ORIGINALFILEIDS = Ext.create('Ext.form.field.Hidden', {
        id: 'field_ORIGINALFILEIDS',
        name: 'ORIGINALFILEIDS'
    });
    //文件列表
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
           + '</div>';
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
    var field_fileno1 = Ext.create('Ext.form.field.Text', {
        id: 'field_fileno1',
        labelWidth: 70,
        fieldLabel: '文件统一编号',
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
        minHeight: 350,
        border: 0,
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
        { layout: 'column', height: 42, border: 0, items: [combo_DECLWAY, field_SUBMITUSERNAME, field_SUBMITTIME, field_STATUS, cont_bjsbdw] },
        { layout: 'column', height: 42, border: 0, items: [field_CREATEUSERNAME, field_CREATETIME] },
        { layout: 'column', border: 42, border: 0, items: [label_busiinfo, chk_container] },
        { layout: 'column', height: 42, border: 0, items: [field_CUSNO, combo_PORTCODE, field_jydw, field_TOTALNO, field_DIVIDENO] },
        { layout: 'column', height: 42, border: 0, items: [field_quanpackage, field_weight, field_contractno, field_myfs, field_TURNPRENO] },
        { layout: 'column', height: 42, border: 0, items: [combo_mzbz, field_CLEARANCENO, container_bgch, field_CLEARREMARK, chk_CHKLAWCONDITION] },
        field_CUSTOMDISTRICTNAME, field_PORTNAME, field_BUSIUNITNAME, field_BUSISHORTCODE, field_BUSISHORTNAME,
        field_ID, field_TRADEWAYCODES, field_TRADEWAYCODES1,
        field_CONTAINERTRUCK, field_ORIGINALFILEIDS
        ]
    });
}