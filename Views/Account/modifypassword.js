function modifypsd() {
    Ext.apply(Ext.form.field.VTypes, {
        password: function (val, field) {
            if (field.initialPassField) {
                var pwd = field.up('form').down('#' + field.initialPassField);
                return (val == pwd.getValue());
            }
            return true;
        },
        passwordText: '两次输入的新密码不一致!'
    });
    var field_ori = Ext.create('Ext.form.field.Text', {
        fieldLabel: '原始密码',
        name: 'PASSWORD',
        allowBlank: false,
        blankText: '原始密码不能为空！'
    })
    var field_newpsd1 = Ext.create('Ext.form.field.Text', {
        fieldLabel: '新密码',
        name: 'NEWPASSWORD1',
        allowBlank: false,
        blankText: '密码设置不能为空！',
        id: 'pass',
        listeners: {
            validitychange: function (field) {
                field_newpsd2.validate();
            },
            blur: function (field) {//field.next()
                field_newpsd2.validate();
            }
        }
    })
    var field_newpsd2 = Ext.create('Ext.form.field.Text', {
        fieldLabel: '确认密码',
        vtype: 'password',
        name: 'NEWPASSWORD2',
        allowBlank: false,
        blankText: '密码设置不能为空！',
        initialPassField: 'pass' // id of the initial password field
    })
    var form_psd = Ext.create('Ext.form.Panel', {
        border: 0,
        region: 'center',
        fieldDefaults: {
            columnWidth: .80,
            labelAlign: 'top',
            labelStyle: 'font-weight:bold',
            width: 250,
            inputType: 'password',
            msgTarget: 'under'
        },
        items: [
            { layout: 'column', margin: '5 10', height: 67, border: 0, items: [field_ori] },
            { layout: 'column', margin: '5 10', height: 67, border: 0, items: [field_newpsd1] },
            { layout: 'column', margin: '5 10', height: 67, border: 0, items: [field_newpsd2] }
        ]
    });
    var win_modifypsd = Ext.create("Ext.window.Window", {
        title: '修改密码',
        width: 400,
        height: 320,
        modal: true,
        items: [form_psd],
        layout: 'border',
        buttonAlign: 'center',
        buttons: [{
            text: '<i class="fa fa-check-square-o"></i>&nbsp;确定', handler: function () {
                if (form_psd.getForm().isValid()) {
                    Ext.Ajax.request({//先验证原始密码
                        url: "/Account/ValidPassword",
                        params: {PASSWORD: field_ori.getValue() },
                        success: function (response, option) {
                            var data = Ext.decode(response.responseText);
                            if (!data.success) {
                                field_ori.markInvalid('原始密码输入不正确!');
                            }
                            else {
                                Ext.Ajax.request({
                                    url: "/Account/UpdatePassword",
                                    params: {PASSWORD: field_newpsd1.getValue() },
                                    success: function (response, option) {
                                        var data = Ext.decode(response.responseText);
                                        Ext.MessageBox.alert('提示', data.result ? '密码更新成功！' : '密码更新失败！');
                                        win_modifypsd.close();
                                    }
                                });
                            }
                        }
                    });
                }
            }
        }]
    });
    win_modifypsd.show();
}