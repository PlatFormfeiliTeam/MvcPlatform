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
        border: 0,
        fieldDefaults: {
            margin: '10 10',
            width: 350
        },
        items: [field_image, btn_upload, account, name, email, phone, mobile, company, field_ID, field_TYPE, field_PASSWORD],
        buttonAlign: 'center',
        buttons: [{
            text: '<i class="fa fa-floppy-o"></i>&nbsp;保存', handler: function () {
                var formdata = formpanel.getForm().getValues();
                Ext.Ajax.request({
                    url: "/AccountManagement/Update",
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
        }, {
            text: '<i class="fa fa-pencil-square-o" aria-hidden="true"></i>&nbsp;修改密码', handler: function () {
                modifypsd();
            }
        }]
    })
}