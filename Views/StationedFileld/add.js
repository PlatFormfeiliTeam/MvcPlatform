function form_ini() {

    //业务状态
    var store_STATUS = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_status
    });
    var combo_STATUS = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_STATUS',
        name: 'STATUS',
        hideTrigger: true,
        store: store_STATUS,
        fieldLabel: '业务状态',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        //tabIndex: 1,
        //minChars: '5',
        queryMode: 'local',
        anyMatch: true,
        //columnWidth: .25,
        labelWidth: 60,
        //margin: 0,
        //flex: 1,
        readOnly:true,
        fieldStyle: 'background-color: #CECECE; background-image: none;',
    });

    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;业务信息</span></h4>'
    }
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo, '->', combo_STATUS]
    });

    //订单编号
    var field_CODE = Ext.create('Ext.form.field.Text', {
        id: 'field_CODE',
        name: 'CODE',
        fieldLabel: '订单编号',
        readOnly: true,
        flex: 1,
        emptyText: '订单号自动生成',
        columnWidth: .25,
        labelWidth: 60,
    });

    //经营单位
    var store_BUSIUNITCODE = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_jydw
    });
    var combo_BUSIUNITCODE = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_BUSIUNITCODE',
        name: 'BUSIUNITCODE',
        hideTrigger: true,
        store: store_BUSIUNITCODE,
        fieldLabel: '经营单位',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 1,
        minChars:'5',
        queryMode: 'local',
        anyMatch: true,
        columnWidth: .25,
        labelWidth: 60,
        margin: 0,
        flex: 1,
        listeners: {
            blur: function (th, e, eOpts) {
                field_BUSIUNITNAME.setValue(th.rawValue);
            },
            change: function () {
                 if (combo_BUSIUNITCODE.getValue() == null) {
                     combo_BUSIUNITCODE.reset();
                     field_BUSIUNITNAME.reset();
                  }
              }           
        }
    });
    //经营单位名称
    var field_BUSIUNITNAME = Ext.create('Ext.form.field.Text', {
        id: 'field_BUSIUNITNAME',
        name: 'BUSIUNITNAME',
        hidden:true,
    });

    //经营单位
    var field_BUSIUNITCODE_BUSIUNITNAME = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: 0.25,
        items: [combo_BUSIUNITCODE, field_BUSIUNITNAME]
    }

    //企业编号
    var field_CUSNO = Ext.create('Ext.form.field.Text', {
        id: 'CUSNO',
        name: 'CUSNO',
        tabIndex: 2,
        flex: 1,
        fieldLabel: '企业编号',
        columnWidth: .25,
        labelWidth: 60,
    });

    //业务类型
    var store_busitype = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_busitype
    });
    var combo_busitype = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_busitype',
        name: 'BUSITYPE',
        hideTrigger: true,
        store: store_busitype,
        fieldLabel: '业务类型',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 3,
        minChars: '1',
        queryMode: 'local',
        anyMatch: true,
        columnWidth: .25,
        labelWidth: 60,
        listeners:{
          change: function () {
              if (combo_busitype.getValue() == null) {
                  combo_busitype.reset();
              }
          }
        }
    });

    //进出境关别
    var store_portcode = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbgq
    });
    var combo_portcode = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_portcode',
        name: 'PORTCODE',
        hideTrigger: true,
        store: store_portcode,
        fieldLabel: '进出境关别',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 4,
        queryMode: 'local',
        anyMatch: true,
        columnWidth: .25,
        labelWidth: 60,
        minChars: '1',
        listeners:{
          change: function () {
              if (combo_portcode.getValue() == null) {
                  combo_portcode.reset();
              }
          }
        }
    });

    //监管方式
    var store_TRADEWAY2 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_myfs
    });
    var combo_TRADEWAY2 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_TRADEWAY2',
        name: 'TRADEWAY2',
        hideTrigger: true,
        store: store_TRADEWAY2,
        fieldLabel: '监管方式',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 5,
        queryMode: 'local',
        anyMatch: true,
        columnWidth: .25,
        labelWidth: 60,
        minChars: '1',
        listeners: {
            change: function () {
                if (combo_TRADEWAY2.getValue() == null) {
                    combo_TRADEWAY2.reset();
                }
            }
        }
    });

    //总单号
    var field_TOTALNO = Ext.create('Ext.form.field.Text', {
        id: 'TOTALNO',
        name: 'TOTALNO',
        tabIndex: 6,
        flex: 1,
        fieldLabel: '总单号',
        columnWidth: .25,
        labelWidth: 60,
    });

    //分单号
    var field_DIVIDENO = Ext.create('Ext.form.field.Text', {
        id: 'DIVIDENO',
        name: 'DIVIDENO',
        tabIndex: 7,
        flex: 1,
        fieldLabel: '分单号',
        columnWidth: .25,
        labelWidth: 60,
    });

    //合同发票号
    var field_CONTRACTNO = Ext.create('Ext.form.field.Text', {
        id: 'CONTRACTNO',
        name: 'CONTRACTNO',
        tabIndex: 8,
        flex: 1,
        fieldLabel: '合同发票号',
        columnWidth: .25,
        labelWidth: 60,
    });

    //备注
    var field_REMARK2 = Ext.create('Ext.form.field.Text', {
        id: 'REMARK2',
        name: 'REMARK2',
        tabIndex: 9,
        flex: 1,
        fieldLabel: '备注',
        columnWidth: .25,
        labelWidth: 60,
    });

    //件数
    var field_GOODSNUM2 = Ext.create('Ext.form.field.Number', {
        id: 'GOODSNUM2',
        name: 'GOODSNUM2',
        tabIndex: 10,
        flex: 0.5,
        fieldLabel: '件数/毛重',
        margin: 0,
        // columnWidth: .4,
        labelWidth: 60,
        allowDecimals: false,
        hideTrigger :true,
    });
    //毛重
    var field_GOODGW2 = Ext.create('Ext.form.field.Number', {
        id: 'GOODGW2',
        name: 'GOODGW2',
        margin: 0,
        tabIndex: 11,
        flex: 0.5,
        hideTrigger: true,
    });
    //毛重件数
    var field_GOODSNUM_GOODGW = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: 0.25,
        items: [field_GOODSNUM2, field_GOODGW2]
    }

    //舱单
    var chk_MANIFEST = Ext.create('Ext.form.field.Checkbox', {
        id: 'MANIFEST',
        name: 'MANIFEST',
        //tabIndex: 12,
        flex: 1,
        fieldLabel: '舱单',
        columnWidth: .1,
        labelWidth: 60,
    });

    //报检标志
    var chk_INSPFLAG = Ext.create('Ext.form.field.Checkbox', {
        id: 'INSPFLAG',
        name: 'INSPFLAG',
        //tabIndex: 13,
        flex: 1,
        fieldLabel: '报检标志',
        columnWidth: .1,
        labelWidth: 60,
    });


    formpanel_form = Ext.create('Ext.form.Panel', {
        id: "formpanel_form",
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
           { layout: 'column', height: 30, margin: '5 0 0 0', border: 0, items: [field_CODE, field_BUSIUNITCODE_BUSIUNITNAME, field_CUSNO, combo_busitype] },//combo_TRADEWAY
           { layout: 'column', height: 30, border: 0, items: [combo_portcode, combo_TRADEWAY2, field_TOTALNO, field_DIVIDENO] },
           { layout: 'column', height: 30, border: 0, items: [field_CONTRACTNO, field_REMARK2,field_GOODSNUM_GOODGW, chk_MANIFEST, chk_INSPFLAG] },
        ]
    });
}

function form_ini_decl() {
    var rownum = -1;//记录当前编辑的行号

    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        //html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;报关单信息</span></h4>'
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;报关单信息</span></h4>'
    }
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        margin: '0 0 5 0',
        items: [label_baseinfo, '->',
            {
                text: '<span class="icon iconfont" style="font-size:10px">&#xe6d3;</span>&nbsp;移 除',
                id: 'btn_del',
                handler: function () {
                    var sel = gridpanel_declGrid.getSelectionModel().getSelection();
                    if (sel.length == 0) {
                        Ext.MessageBox.alert('提示','请选择要移除的行');
                        return;
                    }
                    Ext.MessageBox.confirm("提示", "确定要移除所选择的记录吗？", function (btn) {
                        if (btn == 'yes') {
                            store_declGrid.remove(sel);
                            //Ext.getCmp('btn_mode').setText('<span style="color:blue">新增模式</span>');
                            //rownum = -1;
                            formpanel_decl.getForm().reset();
                            rownum = -1;
                        }
                    });
                }
            }]
    })
    //===============================================

    var data_declGrid = [];
    var store_declGrid = Ext.create('Ext.data.JsonStore', {
        storeId: 'store_declGrid',
        fields: ['ORDERCODE','DECLARATIONCODE', 'TRADEWAY', 'TRADEWAYNAME', 'SHEETNUM', 'MODIFYFLAG', 'GOODSNUM', 'GOODGW', 'CUSTOMSSTATUS', 'CREATETIME', 'REMARK'],
        data: data_declGrid
    });

    var gridpanel_declGrid = Ext.create('Ext.grid.Panel', {
        id: 'gridpanel_declGrid',
        renderTo: 'div_form_decl',
        store: store_declGrid,
        minHeight: 200,
        maxHeight: 200,
        //margin: '5 0 0 0',
        //    flex: 1,
       // autoScroll: true,
       // scroll:'both',
        selModel: { selType: 'checkboxmodel' },
        enableColumnHide: false,
        tbar: tbar,
        columns: [
       // { xtype: 'rownumberer', width: 35 },
     //   { header: 'ID', dataIndex: 'ID', hidden: true },
     //   { header: '订单号', dataIndex: 'ORDERCODE', width: 130 },
        { header: '报关单号', dataIndex: 'DECLARATIONCODE', width: 130 },
        { header: '监管方式', dataIndex: 'TRADEWAY', hidden: true },
         { header: '监管方式', dataIndex: 'TRADEWAYNAME', width: 130 },
        { header: '张数', dataIndex: 'SHEETNUM', width: 130 },
        { header: '删改单', dataIndex: 'MODIFYFLAG', hidden: true },
        { header: '件数', dataIndex: 'GOODSNUM', width: 100 },
        { header: '毛重', dataIndex: 'GOODGW', width: 100 },
        { header: '海关状态', dataIndex: 'CUSTOMSSTATUS', width: 130 },
        { header: '备注', dataIndex: 'REMARK', width: 130 },
   //     { header: '创建时间', dataIndex: 'CREATETIME', hidden: true },
        ],
        listeners: {
            itemdblclick: function (w_grid, record, item, index, e, eOpts) {
                rownum = index;
                formpanel_decl.getForm().setValues(record.data);
               // Ext.getCmp('btn_mode').setText('<span style="color:blue">编辑模式</span>');
            }
        },
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });

    //================================================================
    //报关单号
    var field_DECLARATIONCODE = Ext.create('Ext.form.field.Text', {
        id: 'DECLARATIONCODE',
        name: 'DECLARATIONCODE',
        tabIndex: 12,
       // flex: 0.5,
        fieldLabel: '报关单号',
        // columnWidth: .25,
        labelWidth: 60,
        maxLength: 18,
        minLength: 18,
        msgTarget: 'side',
    });

    //监管方式中文
    var field_TRADEWAYNAME = Ext.create('Ext.form.field.Text', {
        id: 'TRADEWAYNAME',
        name: 'TRADEWAYNAME',
        hidden:true,
    });
    //监管方式
    var store_TRADEWAY = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_myfs
    });
    var combo_TRADEWAY = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_TRADEWAY',
        name: 'TRADEWAY',
        hideTrigger: true,
        store: store_TRADEWAY,
        fieldLabel: '监管方式',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        queryMode: 'local',
        anyMatch: true,
        columnWidth: .25,
        labelWidth: 60,
        flex: 1,
        margin: 0,
        tabIndex: 13,
        minChars: '1',
        listeners: {
            blur: function (th, e, eOpts) {
                field_TRADEWAYNAME.setValue(th.rawValue);
            },
            change: function () {
                if (combo_TRADEWAY.getValue() == null) {
                    combo_TRADEWAY.reset();
                    field_TRADEWAYNAME.reset();
                }
            }
        }

    });
    //监管方式
    var field_TRADEWAY_TRADEWAYNAME = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: 0.25,
        items: [combo_TRADEWAY, field_TRADEWAYNAME]
    }

    //张数
    var field_SHEETNUM = Ext.create('Ext.form.field.Number', {
        id: 'SHEETNUM',
        name: 'SHEETNUM',
        tabIndex: 14,
        // flex: 0.5,
        fieldLabel: '张数',
        // columnWidth: .25,
        labelWidth: 60,
        allowDecimals: false,
        hideTrigger: true,
    });
    //删改单
    var store_MODIFYFLAG = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_Modify
    });
    var combo_MODIFYFLAG = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_MODIFYFLAG',
        name: 'MODIFYFLAG',
        hideTrigger: true,
        store: store_MODIFYFLAG,
        fieldLabel: '删改单',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 15,
        queryMode: 'local',
        anyMatch: true,
       // columnWidth: .25,
        labelWidth: 60,
        minChars: '1',
        listeners: {
            change: function () {
                if (combo_MODIFYFLAG.getValue() == null) {
                    combo_MODIFYFLAG.reset();
                }
            }
        }
    });
    //海关状态
    var store_CUSTOMSSTATUS = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_CUSTOMSSTATUS
    });
    var combo_CUSTOMSSTATUS = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_CUSTOMSSTATUS',
        name: 'CUSTOMSSTATUS',
        hideTrigger: true,
        store: store_CUSTOMSSTATUS,
        fieldLabel: '海关状态',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 16,
        queryMode: 'local',
        anyMatch: true,
        // columnWidth: .25,
        labelWidth: 60,
        minChars: '1',
        listeners: {
            change: function () {
                if (combo_CUSTOMSSTATUS.getValue() == null) {
                    combo_CUSTOMSSTATUS.reset();
                }
            }
        }
    });
    //件数
    var field_GOODSNUM = Ext.create('Ext.form.field.Number', {
        id: 'GOODSNUM',
        name: 'GOODSNUM',
        tabIndex: 17,
       // flex: 0.5,
        fieldLabel: '件数',
        // columnWidth: .25,
        labelWidth: 60,
        allowDecimals: false,
        hideTrigger: true,
    });

    //毛重
    var field_GOODGW = Ext.create('Ext.form.field.Number', {
        id: 'GOODGW',
        name: 'GOODGW',
        tabIndex: 18,
      //  flex: 0.5,
          fieldLabel: '毛重',
        // columnWidth: .25,
          labelWidth: 60,
          hideTrigger: true,
    });
    //备注
    var field_REMARK = Ext.create('Ext.form.field.Text', {
        id: 'REMARK',
        name: 'REMARK',
        tabIndex: 19,
        //  flex: 0.5,
        fieldLabel: '备注',
        // columnWidth: .25,
        labelWidth: 60,
        listeners: {
            specialkey: function (field, e) {
                if (e.getKey() == e.ENTER)
                {
                    if (formpanel_decl.getForm().isValid()) {

                        var formdata = formpanel_decl.getForm().getValues();
                        if (rownum < 0) {
                            store_declGrid.insert(store_declGrid.data.length, formdata);
                        }
                        else {//修改模式
                            var rec = store_declGrid.getAt(rownum);
                            store_declGrid.remove(rec);
                            store_declGrid.insert(rownum, formdata);
                        }
                        //
                        formpanel_decl.getForm().reset();
                        rownum = -1;
                        Ext.getCmp('GOODGW2').focus();
                        //field_DECLARATIONCODE.focus();//combo_TRADEWAY  field_DECLARATIONCODE field_GOODGW2
                    }
                }
            }
        }
    });


    var formpanel_decl = Ext.create('Ext.form.Panel', {
        id: 'formpanel_decl',
        renderTo: 'div_form_decl',
        minHeight: 80,
        border: 0,
       // tbar: tbar,
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
            { layout: 'column', height: 30, border: 0, margin: '10 0 0 0', items: [field_DECLARATIONCODE, field_TRADEWAY_TRADEWAYNAME, field_SHEETNUM, combo_MODIFYFLAG] },
            { layout: 'column', height: 30, border: 0, items: [combo_CUSTOMSSTATUS, field_GOODSNUM, field_GOODGW, field_REMARK] },
        ]
    });


}

function form_ini_btn() {

    var bbar_r = '<div class="btn-group" role="group">'
                        + '<button type="button" onclick="create_save()" id="btn_submitorder" class="btn btn-primary btn-sm"><i class="fa fa-hand-o-up"></i>&nbsp;保存</button>'
                         + '<button type="button" onclick="CopyAdd()" id="btn_CopyAdd" class="btn btn-primary btn-sm"><i class="fa fa-hand-o-up"></i>&nbsp;复制新增</button>'
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

function form_ini_time() {
    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;时间信息</span></h4>'
    }
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo]
    });

    var txtSUBMITTIME = Ext.create('Ext.form.field.Text', {
        id: 'SUBMITTIME',
        name: 'SUBMITTIME',
        fieldLabel: '委托时间',
        labelWidth: 60,
        readOnly: true,
        margin: 0,
        flex: .7,
        fieldStyle: 'background-color: #CECECE; background-image: none;',
        //listeners: {
        //    render:function(p){
        //        p.getEl().on('dblclick', function () {
        //            if (txtSUBMITTIME.getValue() == null || txtSUBMITTIME.getValue()=="") {
        //                txtSUBMITTIME.setValue(Ext.Date.format(new Date(), 'Y-m-d H:i:s')); //new Date()
        //                txtSUBMITUSERNAME.setValue(common_data_curuser.REALNAME);
        //                txtSUBMITUSERID.setValue(common_data_curuser.ID);
        //            }
        //        });
        //    }
        //}
    });
    var txtSUBMITUSERNAME = Ext.create('Ext.form.field.Text', {
        id: 'SUBMITUSERNAME',
        name: 'SUBMITUSERNAME',
        readOnly: true,
        margin: 0,
        flex: .3,
        fieldStyle: 'background-color: #CECECE; background-image: none;',
    });
    var txtSUBMITUSERID = Ext.create('Ext.form.field.Text', {
        id: 'SUBMITUSERID',
        name: 'SUBMITUSERID',
        //margin: 0,
        //flex: .3,
        hidden: true,
    });
    //委托时间
    var date_SUBMIT = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: 0.25,
        items: [txtSUBMITTIME, txtSUBMITUSERNAME, txtSUBMITUSERID]
    }

    ////////////////////////
    //var flagACCEPTTIME = 1;//0 代表可删除 1代表不可删除
    var txtACCEPTTIME = Ext.create('Ext.form.field.Text', {
        id: 'ACCEPTTIME',
        name: 'ACCEPTTIME',
        fieldLabel: '受理时间',
        labelWidth: 60,
        readOnly: true,
        margin: 0,
        flex: .7,
        listeners: {
            render: function (p) {
                p.getEl().on('dblclick', function () {
                    if (txtACCEPTTIME.getValue() == null || txtACCEPTTIME.getValue() == "") {
                        getServerTime('ACCEPTTIME');
                        //txtACCEPTTIME.setValue(Ext.Date.format(new Date(), 'Y-m-d H:i:s')); //new Date()
                        txtACCEPTUSERNAME.setValue(common_data_curuser.REALNAME);
                        txtACCEPTUSERID.setValue(common_data_curuser.ID);
                        flagACCEPTTIME = 0;
                    }
                })
            },
            specialkey: function (field, e) {
                if (e.keyCode == 46) {
                    //46 代表del键
                    if (flagACCEPTTIME == 0) {
                        txtACCEPTTIME.setValue(''); //new Date()
                        txtACCEPTUSERNAME.setValue('');
                        txtACCEPTUSERID.setValue('');
                    }
                }
            }
        }
    });
    var txtACCEPTUSERNAME = Ext.create('Ext.form.field.Text', {
        id: 'ACCEPTUSERNAME',
        name: 'ACCEPTUSERNAME',
        margin: 0,
        readOnly: true,
        flex: .3,
    });
    var txtACCEPTUSERID = Ext.create('Ext.form.field.Text', {
        id: 'ACCEPTUSERID',
        name: 'ACCEPTUSERID',
        //margin: 0,
        //flex: .3,
        hidden: true,
    });
    //受理时间
    var date_ACCEPTTIME = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: 0.25,
        items: [txtACCEPTTIME, txtACCEPTUSERNAME, txtACCEPTUSERID]
    }

    ///////////////////////////
   // var flagMOENDTIME = 1;//0 代表可删除 1代表不可删除
    var txtMOENDTIME = Ext.create('Ext.form.field.Text', {
        id: 'MOENDTIME',
        name: 'MOENDTIME',
        fieldLabel: '制单完成',
        labelWidth: 60,
        readOnly: true,
        margin: 0,
        flex: .7,
        listeners: {
            render: function (p) {
                p.getEl().on('dblclick', function () {
                    if (txtMOENDTIME.getValue() == null || txtMOENDTIME.getValue() == "") {
                        getServerTime('MOENDTIME');
                       // txtMOENDTIME.setValue(Ext.Date.format(new Date(), 'Y-m-d H:i:s')); //new Date()
                        txtMOENDNAME.setValue(common_data_curuser.REALNAME);
                        txtMOENDID.setValue(common_data_curuser.ID);
                        flagMOENDTIME = 0;
                    }
                });
            },
            specialkey: function (field, e) {
                if (e.keyCode == 46) {
                    //46 代表del键
                    if (flagMOENDTIME == 0) {
                        txtMOENDTIME.setValue(''); //new Date()
                        txtMOENDNAME.setValue('');
                        txtMOENDID.setValue('');
                    }
                }
            }
        }
    });
    var txtMOENDNAME = Ext.create('Ext.form.field.Text', {
        id: 'MOENDNAME',
        name: 'MOENDNAME',
        readOnly: true,
        margin: 0,
        flex: .3,
    });
    var txtMOENDID = Ext.create('Ext.form.field.Text', {
        id: 'MOENDID',
        name: 'MOENDID',
        //margin: 0,
        //flex: .3,
        hidden: true,
    });
    //制单完成
    var date_MOENDTIME = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: 0.25,
        items: [txtMOENDTIME, txtMOENDNAME, txtMOENDID]
    }

    ///////////////////////////
   // var flagCOENDTIME = 1;//0 代表可删除 1代表不可删除
    var txtCOENDTIME = Ext.create('Ext.form.field.Text', {
        id: 'COENDTIME',
        name: 'COENDTIME',
        fieldLabel: '审核完成',
        labelWidth: 60,
        readOnly: true,
        margin: 0,
        flex: .7,
        listeners: {
            render: function (p) {
                p.getEl().on('dblclick', function () {
                    if (txtCOENDTIME.getValue() == null || txtCOENDTIME.getValue() == "") {
                        getServerTime('COENDTIME');
                       // txtCOENDTIME.setValue(Ext.Date.format(new Date(), 'Y-m-d H:i:s')); //new Date()
                        txtCOENDNAME.setValue(common_data_curuser.REALNAME);
                        txtCOENDID.setValue(common_data_curuser.ID);
                        flagCOENDTIME = 0;
                    }
                });
            },
            specialkey: function (field, e) {
                if (e.keyCode == 46) {
                    //46 代表del键
                    if (flagCOENDTIME == 0) {
                        txtCOENDTIME.setValue(''); //new Date()
                        txtCOENDNAME.setValue('');
                        txtCOENDID.setValue('');
                    }
                }
            }
        }
    });
    var txtCOENDNAME = Ext.create('Ext.form.field.Text', {
        id: 'COENDNAME',
        name: 'COENDNAME',
        readOnly: true,
        margin: 0,
        flex: .3,
    });
    var txtCOENDID = Ext.create('Ext.form.field.Text', {
        id: 'COENDID',
        name: 'COENDID',
        //margin: 0,
        //flex: .3,
        hidden: true,
    });
    //审核完成
    var date_COENDTIME = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: 0.25,
        items: [txtCOENDTIME, txtCOENDNAME, txtCOENDID]
    }

    ///////////////////////////
    //var flagRECOENDTIME = 1;//0 代表可删除 1代表不可删除
    var txtRECOENDTIME = Ext.create('Ext.form.field.Text', {
        id: 'RECOENDTIME',
        name: 'RECOENDTIME',
        fieldLabel: '复审完成',
        labelWidth: 60,
        readOnly: true,
        margin: 0,
        flex: .7,
        listeners: {
            render: function (p) {
                p.getEl().on('dblclick', function () {
                    if (txtRECOENDTIME.getValue() == null || txtRECOENDTIME.getValue() == "") {
                        getServerTime('RECOENDTIME');
                        //txtRECOENDTIME.setValue(Ext.Date.format(new Date(), 'Y-m-d H:i:s')); //new Date()
                        txtRECOENDNAME.setValue(common_data_curuser.REALNAME);
                        txtRECOENDID.setValue(common_data_curuser.ID);
                        flagRECOENDTIME = 0;
                    }
                });
            },
            specialkey: function (field, e) {
                if (e.keyCode == 46) {
                    //46 代表del键
                    if (flagRECOENDTIME == 0) {
                        txtRECOENDTIME.setValue(''); //new Date()
                        txtRECOENDNAME.setValue('');
                        txtRECOENDID.setValue('');
                    }
                }
            }
        }
    });
    var txtRECOENDNAME = Ext.create('Ext.form.field.Text', {
        id: 'RECOENDNAME',
        name: 'RECOENDNAME',
        readOnly: true,
        margin: 0,
        flex: .3,
    });
    var txtRECOENDID = Ext.create('Ext.form.field.Text', {
        id: 'RECOENDID',
        name: 'RECOENDID',
        //margin: 0,
        //flex: .3,
        hidden: true,
    });
    //复审完成
    var date_RECOENDTIME = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: 0.25,
        items: [txtRECOENDTIME, txtRECOENDNAME, txtRECOENDID]
    }

    ///////////////////////////
    //var flagREPSTARTTIME = 1;//0 代表可删除 1代表不可删除
    var txtREPSTARTTIME = Ext.create('Ext.form.field.Text', {
        id: 'REPSTARTTIME',
        name: 'REPSTARTTIME',
        fieldLabel: '申报时间',
        labelWidth: 60,
        readOnly: true,
        margin: 0,
        flex: .7,
        listeners: {
            render: function (p) {
                p.getEl().on('dblclick', function () {
                    if (txtREPSTARTTIME.getValue() == null || txtREPSTARTTIME.getValue() == "") {
                        getServerTime('REPSTARTTIME');
                       // txtREPSTARTTIME.setValue(Ext.Date.format(new Date(), 'Y-m-d H:i:s')); //new Date()
                        txtREPSTARTNAME.setValue(common_data_curuser.REALNAME);
                        txtREPSTARTID.setValue(common_data_curuser.ID);
                        flagREPSTARTTIME = 0;
                    }
                });
            },
            specialkey: function (field, e) {
                if (e.keyCode == 46) {
                    //46 代表del键
                    if (flagREPSTARTTIME == 0) {
                        txtREPSTARTTIME.setValue(''); //new Date()
                        txtREPSTARTNAME.setValue('');
                        txtREPSTARTID.setValue('');
                    }
                }
            }
        }
    });
    var txtREPSTARTNAME = Ext.create('Ext.form.field.Text', {
        id: 'REPSTARTNAME',
        name: 'REPSTARTNAME',
        readOnly: true,
        margin: 0,
        flex: .3,
    });
    var txtREPSTARTID = Ext.create('Ext.form.field.Text', {
        id: 'REPSTARTID',
        name: 'REPSTARTID',
        //margin: 0,
        //flex: .3,
        hidden: true,
    });
    //申报时间
    var date_REPSTARTTIME = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: 0.25,
        items: [txtREPSTARTTIME, txtREPSTARTNAME, txtREPSTARTID]
    }

    ///////////////////////////
    //var flagREPENDTIME = 1;//0 代表可删除 1代表不可删除
    var txtREPENDTIME = Ext.create('Ext.form.field.Text', {
        id: 'REPENDTIME',
        name: 'REPENDTIME',
        fieldLabel: '申报完成',
        labelWidth: 60,
        readOnly: true,
        margin: 0,
        flex: .7,
        listeners: {
            render: function (p) {
                p.getEl().on('dblclick', function () {
                    if (txtREPENDTIME.getValue() == null || txtREPENDTIME.getValue() == "") {
                        getServerTime('REPENDTIME');
                        //txtREPENDTIME.setValue(Ext.Date.format(new Date(), 'Y-m-d H:i:s')); //new Date()
                        txtREPENDNAME.setValue(common_data_curuser.REALNAME);
                        txtREPENDID.setValue(common_data_curuser.ID);
                        flagREPENDTIME = 0;
                    }
                });
            },
            specialkey: function (field, e) {
                if (e.keyCode == 46) {
                    //46 代表del键
                    if (flagREPENDTIME == 0) {
                        txtREPENDTIME.setValue(''); //new Date()
                        txtREPENDNAME.setValue('');
                        txtREPENDID.setValue('');
                    }
                }
            }
        }
    });
    var txtREPENDNAME = Ext.create('Ext.form.field.Text', {
        id: 'REPENDNAME',
        name: 'REPENDNAME',
        readOnly: true,
        margin: 0,
        flex: .3,
    });
    var txtREPENDID = Ext.create('Ext.form.field.Text', {
        id: 'REPENDID',
        name: 'REPENDID',
        //margin: 0,
        //flex: .3,
        hidden: true,
    });
    //申报完成
    var date_REPENDTIME = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: 0.25,
        items: [txtREPENDTIME, txtREPENDNAME, txtREPENDID]
    }

    ///////////////////////////
    //var flagPASSTIME = 1;//0 代表可删除 1代表不可删除
    var txtPASSTIME = Ext.create('Ext.form.field.Text', {
        id: 'PASSTIME',
        name: 'PASSTIME',
        fieldLabel: '通关放行',
        labelWidth: 60,
        readOnly: true,
        margin: 0,
        flex: .7,
        listeners: {
            render: function (p) {
                p.getEl().on('dblclick', function () {
                    if (txtPASSTIME.getValue() == null || txtPASSTIME.getValue() == "") {
                        getServerTime('PASSTIME');
                       // txtPASSTIME.setValue(Ext.Date.format(new Date(), 'Y-m-d H:i:s')); //new Date()
                        txtPASSNAME.setValue(common_data_curuser.REALNAME);
                        txtPASSID.setValue(common_data_curuser.ID);
                        flagPASSTIME = 0;
                    }
                });
            },
            specialkey: function (field, e) {
                if (e.keyCode == 46) {
                    //46 代表del键
                    if (flagPASSTIME == 0) {
                        txtPASSTIME.setValue(''); //new Date()
                        txtPASSNAME.setValue('');
                        txtPASSID.setValue('');
                    }
                }
            }
        }
    });
    var txtPASSNAME = Ext.create('Ext.form.field.Text', {
        id: 'PASSNAME',
        name: 'PASSNAME',
        readOnly: true,
        margin: 0,
        flex: .3,
    });
    var txtPASSID = Ext.create('Ext.form.field.Text', {
        id: 'PASSID',
        name: 'PASSID',
        //margin: 0,
        //flex: .3,
        hidden: true,
    });
    //通关放行
    var date_PASSTIME = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: 0.25,
        items: [txtPASSTIME, txtPASSNAME, txtPASSID]
    }

    ///////////////////////////

    formpanel_time = Ext.create('Ext.form.Panel', {
        id: "formpanel_time",
        renderTo: 'div_form_time',
        minHeight: 110,
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
           { layout: 'column', height: 30, border: 0, margin: '5 0 0 0', items: [date_SUBMIT, date_ACCEPTTIME, date_MOENDTIME, date_COENDTIME] },
           { layout: 'column', height: 30, border: 0, items: [date_RECOENDTIME, date_REPSTARTTIME, date_REPENDTIME, date_PASSTIME] }

        ]
    });
}

function resetTimeFlag() {
    flagACCEPTTIME = 1;//0 代表可删除 1代表不可删除
    flagMOENDTIME = 1;//0 代表可删除 1代表不可删除
    flagCOENDTIME = 1;//0 代表可删除 1代表不可删除
    flagRECOENDTIME = 1;//0 代表可删除 1代表不可删除
    flagREPSTARTTIME = 1;//0 代表可删除 1代表不可删除
    flagREPENDTIME = 1;//0 代表可删除 1代表不可删除
    flagPASSTIME = 1;//0 代表可删除 1代表不可删除
}

function create_save() {
    if (!Ext.getCmp('formpanel_form').getForm().isValid()) {
        return;
    }
    var formOrderdata = Ext.encode(Ext.getCmp('formpanel_form').getForm().getValues());
    var formOrderTimedata = Ext.encode(Ext.getCmp('formpanel_time').getForm().getValues());
    var formDeclData = Ext.encode(Ext.pluck(Ext.data.StoreManager.lookup('store_declGrid').data.items, 'data'));

    //if (Ext.getCmp('SUBMITTIME').getValue() == null || Ext.getCmp('SUBMITTIME').getValue() == "") {
    //    getServerTime('SUBMITTIME');
    //    // txtPASSTIME.setValue(Ext.Date.format(new Date(), 'Y-m-d H:i:s')); //new Date()
    //    Ext.getCmp('SUBMITUSERNAME').setValue(common_data_curuser.REALNAME);
    //    Ext.getCmp('SUBMITUSERID').setValue(common_data_curuser.ID);
    //}

    var mask = new Ext.LoadMask(Ext.get(Ext.getBody()), { msg: "数据保存中，请稍等..." });
    mask.show();
    Ext.Ajax.request({
        url: "/StationedFileld/Create_Save",
        params: { formOrderdata: formOrderdata, formOrderTimedata: formOrderTimedata, formDeclData: formDeclData },
        success: function (response, option) {
            if (response.responseText) {
                mask.hide();
                var data = Ext.decode(response.responseText);
                if (data.success) {
                   // ordercode = data.ordercode;
                    //var msg = "保存成功";
                    //if (data.detail == "f") {
                    //    msg = "业务信息保存成功，费用明细保存失败！";
                    //}

                    Ext.getCmp("formpanel_form").getForm().setValues(data.formOrderData);
                    Ext.getCmp("formpanel_time").getForm().setValues(data.formOrderData);

                    Ext.getCmp('gridpanel_declGrid').store.loadData(data.formDeclData);

                    //Ext.getCmp('SUBMITTIME').setValue(data.SUBMITTIME);
                    //Ext.getCmp('SUBMITUSERNAME').setValue(data.SUBMITUSERNAME);
                    //Ext.getCmp('SUBMITUSERID').setValue(data.SUBMITUSERID);

                    resetTimeFlag();
                    Ext.MessageBox.alert("提示", "保存成功", function () {
                        //loadform_CusService();
                    });

                }
                else {
                    Ext.MessageBox.alert("提示", data.msg);
                }
            }
        }
    });

}

function loadform_CusService() {

    if (ordercode == null || ordercode == ""){
        return;
    }
    Ext.Ajax.request({
        url: "/StationedFileld/loadrecord",
        params: { ordercode: ordercode },
        success: function (response, opts) {
            var data = Ext.decode(response.responseText);

            Ext.getCmp("formpanel_form").getForm().setValues(data.formOrderData);
            Ext.getCmp("formpanel_time").getForm().setValues(data.formOrderData);

            Ext.getCmp('gridpanel_declGrid').store.loadData(data.formDeclData);

        }
    });
}

function getServerTime(time) {
    Ext.Ajax.request({
        url: "/StationedFileld/getServerTime",
      //  params: { ordercode: ordercode },
        success: function (response, opts) {
            var data = Ext.decode(response.responseText);
            //txtACCEPTTIME.setValue(Ext.Date.format(new Date(), 'Y-m-d H:i:s')); //new Date()
            Ext.getCmp(time).setValue(data.time)

        }
    });
}

function CopyAdd() {
    var formOrderdata = Ext.getCmp('formpanel_form').getForm().getValues() ;
    Ext.getCmp('formpanel_form').getForm().reset();

    // var aa = formOrderdata.BUSIUNITCODE; field_BUSIUNITNAME
    Ext.getCmp('combo_BUSIUNITCODE').setValue(formOrderdata.BUSIUNITCODE);
    Ext.getCmp('field_BUSIUNITNAME').setValue(formOrderdata.BUSIUNITNAME);
    Ext.getCmp('combo_busitype').setValue(formOrderdata.BUSITYPE);
    Ext.getCmp('combo_portcode').setValue(formOrderdata.PORTCODE);
    Ext.getCmp('combo_TRADEWAY2').setValue(formOrderdata.TRADEWAY2);

    Ext.getCmp('formpanel_time').getForm().reset();
    Ext.data.StoreManager.lookup('store_declGrid').removeAll();
}



