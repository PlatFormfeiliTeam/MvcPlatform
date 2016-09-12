function sysconfig_ini() {
    var field_id = Ext.create('Ext.form.field.Hidden', {
        name: 'ID'
    });
    var field_customerid = Ext.create('Ext.form.field.Hidden', {
        name: 'CUSTOMERID'
    });
    //业务类型静态数据
    var common_data_busitype = [
        { CODE: '10', NAME: '空运出口' }, { CODE: '11', NAME: '空运进口' }, { CODE: '20', NAME: '海运出口' }, { CODE: '21', NAME: '海运进口' }, { CODE: '30', NAME: '陆运出口' }, { CODE: '31', NAME: '陆运进口' }, { CODE: '40', NAME: '国内出口' }, { CODE: '41', NAME: '国内进口' }, { CODE: '50', NAME: '特殊区域出口' }, { CODE: '51', NAME: '特殊区域进口' }];
    var items = [];
    Ext.each(common_data_busitype, function (busitype) {
        items.push({ boxLabel: busitype.NAME, inputValue: busitype.CODE, name: 'BUSITYPES' })
    })
    var cbg_busitype = Ext.create('Ext.form.CheckboxGroup', {
        id: 'cb_group_busitype',
        margin: '10 0 10 10',
        name: 'BUSITYPES',
        fieldLabel: '业务类型',
        columns: 4,
        items: items
    })
    var cbg_selfcheck = Ext.create('Ext.form.field.Checkbox', {
        margin: '10 0 10 10',
        fieldLabel: '需自审',
        name: 'SELFCHECK'
    })
    var cbg_needweightconfirm = Ext.create('Ext.form.field.Checkbox', {
        margin: '10 0 10 10',
        fieldLabel: '需重量确认',
        name: 'WEIGHTCHECK'
    })
    form_sysconfig = Ext.create('Ext.form.Panel', {
        items: [cbg_busitype, cbg_selfcheck, cbg_needweightconfirm, field_id, field_customerid],
        buttonAlign: 'center',
        buttons: [{
            text: '<i class="fa fa-floppy-o"></i>&nbsp;保存', handler: function () {
                var busitypes = "";
                Ext.each(cbg_busitype.getChecked(), function (chkitem) {
                    busitypes += chkitem.inputValue + ",";
                }); 
                Ext.Ajax.request({
                    url: "/AccountManagement/UpdateConfig",
                    params: { data: Ext.encode(form_sysconfig.getForm().getValues()), busitypes: busitypes },
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