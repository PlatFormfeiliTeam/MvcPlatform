function formpanel_ini() {
    var field_ID = Ext.create('Ext.form.field.Hidden', {
        name: 'ID'
    });
    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down" style="font-size:inherit"></i>&nbsp;基础信息</span></h4>'
    }
    var label_busiinfo = {
        xtype: 'label',
        margin: '0 0 5 5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down" style="font-size:inherit"></i>&nbsp;业务信息</span></h4>'
    }
    //订单编号
    var field_CODE = Ext.create('Ext.form.field.Text', {
        name: 'CODE',
        fieldLabel: '订单编号',
        readOnly: true
    })
    //委托类型 
    var store_ENTRUSTTYPENAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: [{ "CODE": "01", "NAME": "报关单(01)" }, { "CODE": "02", "NAME": "报检单(02)" }, { "CODE": "03", "NAME": "报关/检单(03)" }]
    })
    var field_ENTRUSTTYPE = Ext.create('Ext.form.field.ComboBox', {
        name: 'ENTRUSTTYPEID',
        store: store_ENTRUSTTYPENAME,
        fieldLabel: '委托类型',
        displayField: 'NAME',
        valueField: 'CODE',
        hideTrigger: true,
        readOnly: true
    })
    //申报方式
    var store_REPWAYNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbfs
    })
    var field_REPWAY = Ext.create('Ext.form.field.ComboBox', {
        name: 'REPWAYID',
        store: store_REPWAYNAME,
        hideTrigger: true,
        fieldLabel: '申报方式',
        displayField: 'NAME',
        valueField: 'CODE',
        readOnly: true
    })
    //申报关区
    var store_CUSTOMDISTRICTNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbgq
    })
    var field_CUSTOMDISTRICT = Ext.create('Ext.form.field.ComboBox', {
        name: 'CUSTOMDISTRICTCODE',
        store: store_CUSTOMDISTRICTNAME,
        fieldLabel: '申报关区',
        displayField: 'NAME',
        valueField: 'CODE',
        hideTrigger: true,
        readOnly: true
    })
    //报关申报单位
    var field_REPUNIT = Ext.create('Ext.form.field.Text', {
        name: 'REPUNITNAME',
        fieldLabel: '报关申报单位',
        readOnly: true
    })
    //报关方式 
    var store_DECLWAY = Ext.create('Ext.data.JsonStore', {  
        fields: ['CODE', 'NAME'],
        data: common_data_bgfs
    })
    var field_DECLWAY = Ext.create('Ext.form.field.ComboBox', {
        name: 'DECLWAY',
        hideTrigger: true,
        store: store_DECLWAY,
        fieldLabel: '报关方式',
        displayField: 'NAME',
        valueField: 'CODE',
        readOnly: true
    })
    //委托人员
    var field_SUBMITUSERNAME = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITUSERNAME',
        fieldLabel: '委托人员',
        readOnly: true
    })

    //委托时间
    var field_SUBMITTIME = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITTIME',
        fieldLabel: '委托时间',
        readOnly: true
    })

    //报检申报单位
    var field_INSPUNITNAME = Ext.create('Ext.form.field.Text', {
        name: 'INSPUNITNAME',
        fieldLabel: '报检申报单位',
        readOnly: true
    })

    //维护人员
    var field_CREATEUSERNAME = Ext.create('Ext.form.field.Text', {
        name: 'CREATEUSERNAME',
        fieldLabel: '维护人员',
        readOnly: true
    })

    //维护时间
    var field_CREATETIME = Ext.create('Ext.form.field.Text', {
        name: 'CREATETIME1',
        fieldLabel: '维护时间',
        readOnly: true
    });

    //业务状态 
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
        store: store_status,
        hiddenTrigger: true,
        readOnly: true        
    });
    //客户编号
    var field_CUSNO = Ext.create('Ext.form.field.Text', {
        name: 'CUSNO',
        fieldLabel: '客户编号',
        readOnly: true
    });

    //进口口岸
    var field_PORTCODE = Ext.create('Ext.form.field.Text', {
        name: 'PORTNAME',
        fieldLabel: '进/出口口岸',
        readOnly: true
    });

    //经营单位
    var field_BUSIUNIT = Ext.create('Ext.form.field.Text', {
        name: 'BUSIUNITNAME',
        fieldLabel: '经营单位',
        readOnly: true
    });

    //总单号
    var field_TOTALNO = Ext.create('Ext.form.field.Text', {
        name: 'TOTALNO',
        fieldLabel: '总单号',
        readOnly: true
    });

    //分单号
    var field_DIVIDENO = Ext.create('Ext.form.field.Text', {
        name: 'DIVIDENO',
        fieldLabel: '分单号',
        readOnly: true
    });

    //件数/包装
    var field_GOODSNUM = Ext.create('Ext.form.field.Text', {
        name: 'GOODSNUM',
        flex: .5,
        margin: 0,
        readOnly: true
    }); 
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
        margin: 0, 
        flex: .5,
        readOnly: true
    })
    var field_quanpackage = {
        xtype: 'fieldcontainer',
        fieldLabel: '件数/包装',
        layout: 'hbox',
        items: [field_GOODSNUM, combo_PACKKINDNAME]
    }

    //毛重/净重
    var field_weight = {
        xtype: 'fieldcontainer',
        fieldLabel: '毛重/净重',
        layout: 'hbox',
        items: [
            {
                id: 'GOODSGW', name: 'GOODSGW', xtype: 'numberfield', flex: .5, margin: 0, readOnly: true
            },
           {
               id: 'GOODSNW', name: 'GOODSNW', xtype: 'numberfield', flex: .5, margin: 0, readOnly: true
           }]
    }

    //合同号
    var field_contractno = Ext.create('Ext.form.field.Text', {
        name: 'CONTRACTNO',
        fieldLabel: '合同号',
        readOnly: true
    });

    //贸易方式
    var field_TRADEWAYCODES = Ext.create('Ext.form.field.Text', {
        name: 'TRADEWAYCODES',
        fieldLabel: '贸易方式',
        readOnly: true
    });

    //转关预录号
    var field_TURNPRENO = Ext.create('Ext.form.field.Text', {
        name: 'TURNPRENO',
        fieldLabel: '转关预录号',
        readOnly: true
    });

    //木质包装 
    var store_mzbz = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_mzbz
    })
   var field_WOODPACKING = Ext.create('Ext.form.field.ComboBox', {
        name: 'WOODPACKINGID',
        store: store_mzbz,
        fieldLabel: '木质包装',
        displayField: 'NAME',
        valueField: 'CODE', 
        hideTrigger: true,
        readOnly: true
    })
    //通关单号
    var field_CLEARANCENO = Ext.create('Ext.form.field.Text', {
        name: 'CLEARANCENO',
        fieldLabel: '通关单号',
        readOnly: true
    });

    //报关车号
    var field_DECLCARNO = Ext.create('Ext.form.field.Text', {
        name: 'DECLCARNO',
        fieldLabel: '报关车号',
        readOnly: true
    });

    //需求备注
    var field_CLEARREMARK = Ext.create('Ext.form.field.Text', {
        name: 'ENTRUSTREQUEST',
        fieldLabel: '需求备注',
        readOnly: true
    });

    //法检标志
    var field_LAWCONDITION = {
        xtype: 'checkboxfield',
        name: 'LAWCONDITION',
        fieldLabel: '法检标志',
        readOnly: true
    };
    formpanel = Ext.create('Ext.form.Panel', {
        border: false,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: .20,
            labelAlign: 'right',
            labelSeparator: '',
            msgTarget: 'under'
        },
        items: [
              { layout: 'column', border: 42, border: 0, items: [label_baseinfo] },
              { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_CODE, field_ENTRUSTTYPE, field_REPWAY, field_CUSTOMDISTRICT, field_REPUNIT] },
              { layout: 'column', height: 42, border: 0, items: [field_DECLWAY, field_SUBMITUSERNAME, field_SUBMITTIME, field_INSPUNITNAME, field_STATUS] },
              { layout: 'column', height: 42, border: 0, items: [field_CREATEUSERNAME, field_CREATETIME] },
              { layout: 'column', border: 42, border: 0, items: [label_busiinfo] },
              { layout: 'column', height: 42, border: 0, items: [field_CUSNO, field_PORTCODE, field_BUSIUNIT, field_TOTALNO, field_DIVIDENO] },
              { layout: 'column', height: 42, border: 0, items: [field_quanpackage, field_weight, field_contractno, field_TRADEWAYCODES, field_TURNPRENO] },
              { layout: 'column', height: 42, border: 0, items: [field_WOODPACKING, field_CLEARANCENO, field_DECLCARNO, field_CLEARREMARK, field_LAWCONDITION] },
              field_ID
        ]
    });
}