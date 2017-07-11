function form_user_init() {
    var field_image = Ext.create('Ext.Img', {
        id: 'user_img',
        src: '',
        width: 150,
        height: 150,
        margin: '10 10'
        // renderTo: Ext.getBody()http://www.sencha.com/img/20110215-feat-html5.png
    });
    var btn_upload = Ext.create('Ext.Button', {
        id: 'img_upload',
        text: '上传图片',
        handler: function () {

        }
    });
    var account = Ext.create('Ext.form.field.Text', {
        fieldLabel: '登录账号',
        name: 'NAME',
        readOnly: true
    })
    var name = Ext.create('Ext.form.field.Text', {
        fieldLabel: '姓名',
        name: 'REALNAME'
    })
    var email = Ext.create('Ext.form.field.Text', {
        fieldLabel: '邮箱',
        name: 'EMAIL',
    })
    var phone = Ext.create('Ext.form.field.Text', {
        fieldLabel: '工作电话',
        name: 'TELEPHONE',
    })
    var mobile = Ext.create('Ext.form.field.Text', {
        fieldLabel: '移动电话',
        name: 'MOBILEPHONE',
    })
    var company = Ext.create('Ext.form.field.Text', {
        fieldLabel: '公司名称',
        name: 'COMPANYNAME',
        readOnly: true
    })
    //待删除
    field_ID = Ext.create('Ext.form.field.Hidden', {
        id: 'user_id',
        name: 'ID'
    });
    field_TYPE = Ext.create('Ext.form.field.Hidden', {
        name: 'TYPE'
    });
    field_PASSWORD = Ext.create('Ext.form.field.Hidden', {
        name: 'PASSWORD'
    });
    formpanel = Ext.create('Ext.form.Panel', {
        border: 1, columnWidth: 0.50, title: '<center>基本资料</center>',
        fieldDefaults: {
            margin: '10 10',
            width: 500
        },
        items: [field_image, btn_upload, account, name, email, phone, mobile, company, field_ID, field_TYPE, field_PASSWORD],
        buttonAlign: 'center',
        buttons: [{
       
            text: '<i class="fa fa-floppy-o"></i>&nbsp;保存', handler: function () {
                var formdata = formpanel.getForm().getValues();
                Ext.Ajax.request({
                    url: "/Account/Update",
                    params: { json: Ext.encode(formdata) },
                    success: function (option, success, response) {
                        var data = Ext.decode(option.responseText);
                        if (data.result == 1) {
                            Ext.MessageBox.alert('提示', "保存成功！");
                        }
                        else {
                            Ext.MessageBox.alert('提示', "保存失败！");
                        }
                    }
                });
            }
        }]
    });

    //=============================================================================================================================================
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
    form_psd = Ext.create('Ext.form.Panel', {
        id: 'form_psd', margin: '0 0 0 3', //margin: '175 0 0 5',
        columnWidth: 0.50, border: 1, title: '<center>修改密码</center>',
        height:470,
        fieldDefaults: {
            columnWidth: .80, labelStyle: 'font-weight:bold',
            inputType: 'password',
            msgTarget: 'under'
        },
        items: [
            { layout: 'column', margin: '15 10', height: 67, border: 0, items: [field_ori] },
            { layout: 'column', margin: '5 10', height: 67, border: 0, items: [field_newpsd1] },
            { layout: 'column', margin: '5 10', height: 67, border: 0, items: [field_newpsd2] }
        ],
        buttonAlign: 'center',
        buttons: [{
                text: '<i class="fa fa-pencil-square-o" aria-hidden="true"></i>&nbsp;修改密码', handler: function () {
                    if (form_psd.getForm().isValid()) {

                        if (field_ori.getValue() == field_newpsd1.getValue()) {
                            field_newpsd1.markInvalid('原始密码与新密码不可相同!');
                            return;
                        }

                        Ext.Ajax.request({//先验证原始密码
                            url: "/Account/ValidPassword",
                            params: { PASSWORD: field_ori.getValue() },
                            success: function (response, option) {
                                var data = Ext.decode(response.responseText);
                                if (!data.success) {
                                    field_ori.markInvalid('原始密码输入不正确!');
                                }
                                else {
                                    Ext.Ajax.request({
                                        url: "/Account/UpdatePassword",
                                        params: { PASSWORD: field_newpsd1.getValue() },
                                        success: function (response, option) {
                                            var data = Ext.decode(response.responseText);
                                            Ext.MessageBox.alert('提示', data.result ? '密码更新成功！' : '密码更新失败！');
                                        }
                                    });
                                }
                            }
                        });
                    }
                }
            }]
    });

    //===================================================================================================================================================
    formuser_panel = Ext.create('Ext.panel.Panel', {
        id: 'formuser_panel',
        border: 0,
        items: [
          { layout: 'column', border: 0, margin: '5 0 15 0', items: [formpanel, form_psd] }
        ]
    });


}


