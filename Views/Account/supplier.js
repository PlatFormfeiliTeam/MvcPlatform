function supplier_ini() {
    var store_decl_supplier = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'NAME'],
        proxy: {
            type: 'ajax',
            url: '/Account/LoadSupplier',
            reader: {
                root: 'rows',
                type: 'json'
            }
        },
        autoLoad: true
    })
    var combo_decl_supplier = Ext.create('Ext.form.field.ComboBox', { //现场报关服务单位
        fieldLabel: '默认报关服务单位',
        labelWidth: 130,
        store: store_decl_supplier,
        displayField: 'NAME',
        valueField: 'ID',
        name:'SCENEDECLAREID',
        editable: false,
        width: 350
    })
    var store_insp_supplier = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'NAME'],
        proxy: {
            type: 'ajax',
            url: '/Account/LoadSupplier',
            reader: {
                root: 'rows',
                type: 'json'
            }
        },
        autoLoad: true
    })
    var combo_insp_supplier = Ext.create('Ext.form.field.ComboBox', { //现场报关服务单位
        fieldLabel: '默认报检服务单位',
        labelWidth: 130,
        store: store_insp_supplier,
        displayField: 'NAME',
        valueField: 'ID',
        name: 'SCENEINSPECTID',
        editable: false,
        width: 350
    })
    var field_userid = Ext.create('Ext.form.field.Hidden', {
        name: 'ID'
    });
    var field_customerid = Ext.create('Ext.form.field.Hidden', {
        name: 'CUSTOMERID'
    });
    form_supplier = Ext.create('Ext.form.Panel', {
        id:'form_supplier',
        border: 0,
        fieldDefaults: {
            margin: '10 10'
        },
        items: [combo_decl_supplier, combo_insp_supplier, field_userid, field_customerid],
        buttonAlign: 'center',
        buttons: [{
            text: '<i class="fa fa-floppy-o"></i>&nbsp;保存', handler: function () {
                Ext.Ajax.request({
                    url: "/Account/UpdateSupplier",
                    params: { data: Ext.encode(form_supplier.getForm().getValues()) },
                    success: function (response, opts) {
                        var data = Ext.decode(response.responseText);
                        if (data.result == 1) {
                            Ext.MessageBox.alert('提示', '保存成功！');
                        }
                        else {
                            Ext.MessageBox.alert('提示', '保存失败！');
                        }
                    }
                });
            }
        }]
    })
}