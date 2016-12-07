
function form_head2_ini() {
    //进出口类型
    var store_BUSITYPE2 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: ietype_js_data
    })
    var combo_BUSITYPE2 = Ext.create('Ext.form.field.ComboBox', {
        id: 'IETYPE2',
        name: 'IETYPE',
        editable: false,
        store: store_BUSITYPE2,
        hideTrigger: true,
        fieldLabel: '进出口类型',
        displayField: 'NAME',
        valueField: 'CODE',
        tabIndex: 39,
        queryMode: 'local',
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand();
                }
            }, 
            change: function (cb, newValue, oldValue, eOpts) {
                if (cb.getValue() == "仅进口") {
                    formpanelin2.setVisible(true);
                    formpanelout2.setVisible(false);
                }
                if (cb.getValue() == "仅出口") {
                    formpanelin2.setVisible(false);
                    formpanelout2.setVisible(true);
                }
                if (cb.getValue() == "进/出口业务") {
                    formpanelin2.setVisible(true);
                    formpanelout2.setVisible(true);
                }
            }
        },
        labelWidth: 80,
        allowBlank: false,
        value: '进/出口业务',
        blankText: '进出口类型不能为空!'
    })
    //申报方式
    var store_REPWAYNAME2 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbfs
    })
    var combo_REPWAYNAME2 = Ext.create('Ext.form.field.ComboBox', {
        name: 'REPWAYID',
        store: store_REPWAYNAME2,
        hideTrigger: true,
        fieldLabel: '申报方式',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        forceSelection: true,
        tabIndex: 40,
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
        labelWidth: 80,
        allowBlank: false,
        blankText: '申报方式不能为空!'
    })
    //申报关区
    var store_CUSTOMDISTRICTNAME2 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbgq
    })
    var combo_CUSTOMDISTRICTNAME2 = Ext.create('Ext.form.field.ComboBox', {//申报关区 这个数据比较多需要根据输入字符到后台动态模糊匹配
        name: 'CUSTOMAREACODE',
        store: store_CUSTOMDISTRICTNAME2,
        fieldLabel: '申报关区',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        hideTrigger: true,
        forceSelection: true,
        tabIndex: 41,
        anyMatch: true,
        allowBlank: false,
        minChars: 2,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                }
            },
            select: function (cb, records, eOpts) {
                field_CUSTOMDISTRICTNAME2.setValue(cb.getRawValue().substr(0, cb.getRawValue().lastIndexOf('(')));
            }
        },
        blankText: '申报关区不能为空!',
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    var field_CUSTOMDISTRICTNAME2 = Ext.create('Ext.form.field.Hidden', {
        name: 'CUSTOMDISTRICTNAME'
    })
    //委托时间
    var field_SUBMITTIME2 = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITTIME',
        fieldLabel: '委托时间',
        readOnly: true
    });
    //委托人员
    var field_SUBMITUSERNAME2 = Ext.create('Ext.form.field.Text', {
        name: 'SUBMITUSERNAME',
        fieldLabel: '委托人员',
        readOnly: true
    });
    //维护人员
    var field_CREATEUSERNAME2 = Ext.create('Ext.form.field.Text', {
        name: 'CREATEUSERNAME',
        fieldLabel: '维护人员',
        readOnly: true
    });
    //维护时间
    var field_CREATETIME2 = Ext.create('Ext.form.field.Text', {
        name: 'CREATETIME',
        fieldLabel: '维护时间',
        readOnly: true
    });
    //单证服务单位
    var store_dzfwdw2 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_dzfwdw
    })
    var combo_dzfwdw2 = Ext.create('Ext.form.field.ComboBox', {
        name: 'DOCSERVICECODE',
        store: store_dzfwdw2,
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
    formpanelhead2 = Ext.create('Ext.form.Panel', {
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
                { layout: 'column', height: 42, border: 0, margin: '5 0 0 0', items: [combo_BUSITYPE2, combo_REPWAYNAME2, combo_CUSTOMDISTRICTNAME2, field_SUBMITTIME2, field_SUBMITUSERNAME2] },
                { layout: 'column', height: 42, border: 0, items: [field_CREATEUSERNAME2, field_CREATETIME2, combo_dzfwdw2] },
        field_CUSTOMDISTRICTNAME2]
    })
}