var type = getQueryString("type"); var code = getQueryString("code"); var ordercode = getQueryString("ordercode"); var busitypeid = getQueryString("busitypeid");

var curuserRealname = ""; curuserId = "";

Ext.onReady(function () {
    form_ini_base();//表单初始化
    form_ini(type);

    //form_ini_btn();
    loadform(); //初始化表单信息,无论是新增还是修改

});

function form_ini_base() {
    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;业务信息</span></h4>'
    }
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo]
    });

    //结算备注
    var field_CLEARREMARK = Ext.create('Ext.form.field.Text', {
        id: 'field_CLEARREMARK',
        tabIndex: 1, flex: 1, margin: 0,
        name: 'CLEARREMARK'
    });
    var container_CLEARREMARK = {
        columnWidth: 1,
        xtype: 'fieldcontainer',
        layout: 'hbox',
        fieldLabel: '结算备注',
        items: [field_CLEARREMARK]
    }

    var formpanel_base = Ext.create('Ext.form.Panel', {
        id: 'formpanel_base',
        renderTo: 'div_form_base',
        minHeight: 80,
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
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [container_CLEARREMARK] }
        ]
    });
}

function form_ini(type) {
    var label_declinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;删改单维护</span></h4>'
    }
    var tbar_decl = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_declinfo]
    });

    //删单
    var chk_DEL_MODIFYFLAG = {
        id: 'chk_DEL_MODIFYFLAG',
        xtype: 'checkboxfield',
        tabIndex: 2, columnWidth: .10,
        fieldLabel: '删单',
        name: 'DEL_MODIFYFLAG',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue == true) {
                    field_DELORDERTIME.setValue(getNowDate());
                    field_DELORDERUSERNAME.setValue(curuserRealname);
                    field_DELORDERUSERID.setValue(curuserId);

                    Ext.getCmp('chk_MOD_MODIFYFLAG').setValue(false);
                    Ext.getCmp('chk_FIN_MODIFYFLAG').setValue(false);
                } else {
                    field_DELORDERTIME.setValue("");
                    field_DELORDERUSERNAME.setValue("");
                    field_DELORDERUSERID.setValue("");
                }
            }
        }
    };
    //删单时间
    var field_DELORDERTIME = Ext.create('Ext.form.field.Text', {
        id: 'field_DELORDERTIME',
        name: 'DELORDERTIME',
        fieldLabel: '删单时间', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //删单人员
    var field_DELORDERUSERNAME = Ext.create('Ext.form.field.Text', {
        id: 'field_DELORDERUSERNAME',
        name: 'DELORDERUSERNAME',
        fieldLabel: '删单人员', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });


    //改单
    var chk_MOD_MODIFYFLAG = {
        id: 'chk_MOD_MODIFYFLAG',
        xtype: 'checkboxfield',
        tabIndex: 4,
        fieldLabel: '改单', columnWidth: .10,
        name: 'MOD_MODIFYFLAG',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue == true) {
                    field_MODORDERTIME.setValue(getNowDate());
                    field_MODORDERUSERNAME.setValue(curuserRealname);
                    field_MODORDERUSERID.setValue(curuserId);

                    Ext.getCmp('chk_DEL_MODIFYFLAG').setValue(false);
                    Ext.getCmp('chk_FIN_MODIFYFLAG').setValue(false);
                } else {
                    field_MODORDERTIME.setValue("");
                    field_MODORDERUSERNAME.setValue("");
                    field_MODORDERUSERID.setValue("");
                }
            }
        }
    };
    //改单时间
    var field_MODORDERTIME = Ext.create('Ext.form.field.Text', {
        id: 'field_MODORDERTIME',
        name: 'MODORDERTIME',
        fieldLabel: '改单时间', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //改单人员
    var field_MODORDERUSERNAME = Ext.create('Ext.form.field.Text', {
        id: 'field_MODORDERUSERNAME',
        name: 'MODORDERUSERNAME',
        fieldLabel: '改单人员', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    //改单完成
    var chk_FIN_MODIFYFLAG = {
        id: 'chk_FIN_MODIFYFLAG',
        xtype: 'checkboxfield',
        tabIndex: 4,
        fieldLabel: '改单完成', columnWidth: .10,
        name: 'FIN_MODIFYFLAG',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue == true) {
                    field_MODFINISHTIME.setValue(getNowDate());
                    field_MODFINISHUSERNAME.setValue(curuserRealname);
                    field_MODFINISHUSERID.setValue(curuserId);

                    Ext.getCmp('chk_DEL_MODIFYFLAG').setValue(false);
                    Ext.getCmp('chk_MOD_MODIFYFLAG').setValue(false);
                } else {
                    field_MODFINISHTIME.setValue("");
                    field_MODFINISHUSERNAME.setValue("");
                    field_MODFINISHUSERID.setValue("");
                }
            }
        }
    };
    //改单完成时间
    var field_MODFINISHTIME = Ext.create('Ext.form.field.Text', {
        id: 'field_MODFINISHTIME',
        name: 'MODFINISHTIME',
        fieldLabel: '改单完成时间', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //改单完成人员
    var field_MODFINISHUSERNAME = Ext.create('Ext.form.field.Text', {
        id: 'field_MODFINISHUSERNAME',
        name: 'MODFINISHUSERNAME',
        fieldLabel: '改单完成人', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    var field_REMARK;
    //报关/报检单备注
    if (type == "dec") {//报关单备注        
        field_REMARK = Ext.create('Ext.form.field.Text', {
            id: 'field_DECLREMARK',
            tabIndex: 3,
            name: 'DECLREMARK',
            fieldLabel: '报关单备注', columnWidth: .50
        });
    }
    if (type == "insp") {//报检单备注        
        field_REMARK = Ext.create('Ext.form.field.Text', {
            id: 'field_INSPREMARK',
            tabIndex: 3,
            name: 'INSPREMARK',
            fieldLabel: '报检单备注', columnWidth: .50
        });
    }

    var field_DELORDERUSERID = Ext.create('Ext.form.field.Hidden', { name: 'DELORDERUSERID' });
    var field_MODORDERUSERID = Ext.create('Ext.form.field.Hidden', { name: 'MODORDERUSERID' });
    var field_MODFINISHUSERID = Ext.create('Ext.form.field.Hidden', { name: 'MODFINISHUSERID' });

    var formpanel = Ext.create('Ext.form.Panel', {
        id: 'formpanel',
        renderTo: 'div_form',
        minHeight: 200,
        border: 0,
        tbar: tbar_decl,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            //columnWidth: .20,
            labelAlign: 'right',
            labelSeparator: '',
            msgTarget: 'under',
            validateOnBlur: false,
            validateOnChange: false
        },
        items: [
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [chk_DEL_MODIFYFLAG, field_DELORDERTIME, field_DELORDERUSERNAME] },
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [chk_MOD_MODIFYFLAG, field_MODORDERTIME, field_MODORDERUSERNAME] },
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [chk_FIN_MODIFYFLAG, field_MODFINISHTIME, field_MODFINISHUSERNAME] },
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_REMARK] },
            field_DELORDERUSERID, field_MODORDERUSERID, field_MODFINISHUSERID
        ]
    });
}

function form_ini_btn() {

    var bbar_r = '<div class="btn-group" role="group">'
                        + '<button type="button" onclick="create_save()" id="btn_saveorder" class="btn btn-primary btn-sm"><i class="fa fa-hand-o-up"></i>&nbsp;保存</button>'
            + '</div>';

    var bbar = Ext.create('Ext.toolbar.Toolbar', {
        items: ['->', bbar_r]
    });

    var formpanel_btn = Ext.create('Ext.form.Panel', {
        id: 'formpanel_btn',
        renderTo: 'div_form_btn',
        border: 0,
        bbar: bbar
    });
}

function loadform() {
    Ext.Ajax.request({
        url: "/Common/loaddata_modify",
        params: { type: type, code: code, ordercode: ordercode },
        success: function (response, opts) {
            var data = Ext.decode(response.responseText);
            Ext.getCmp("formpanel_base").getForm().setValues(data.formdata);
            Ext.getCmp("formpanel").getForm().setValues(data.formdata_ini);

            if (data.formdata.RECEIVERUNITCODE == data.curuser.CUSTOMERCODE) {
                //$("#btn_saveorder").hide();
                if (!Ext.getCmp('formpanel_btn')) {
                    form_ini_btn();//保存之后加载数据，不需要创建
                }
            }
            curuserRealname = data.curuser.REALNAME; curuserId = data.curuser.ID;
        }
    });
}

function getNowDate() {
    var nd = new Date();
    var y = nd.getFullYear();
    var m = nd.getMonth() + 1;
    var d = nd.getDate();
    var h = nd.getHours();
    var mi = nd.getMinutes();
    var se = nd.getSeconds();

    if (m <= 9) m = "0" + m;
    if (d <= 9) d = "0" + d;

    if (h <= 9) h = "0" + h;
    if (mi <= 9) mi = "0" + mi;
    if (se <= 9) se = "0" + se;

    var curdate = y + "-" + m + "-" + d + " " + h + ":" + mi + ":" + se;
    return curdate;
}

function create_save() {
    var formpanel_base = Ext.encode(Ext.getCmp("formpanel_base").getForm().getValues());
    var formpanel = Ext.encode(Ext.getCmp("formpanel").getForm().getValues());

    var mask = new Ext.LoadMask(Ext.get(Ext.getBody()), { msg: "数据保存中，请稍等..." });
    mask.show();

    Ext.Ajax.request({
        url: "/Common/savemodify",
        params: { formpanel_base: formpanel_base, formpanel: formpanel, ordercode: ordercode, code: code, type: type, busitypeid: busitypeid },
        success: function (response, option) {
            if (response.responseText) {
                mask.hide();
                var data = Ext.decode(response.responseText);
                if (data.success) {
                    ordercode = data.ordercode; code = data.code; type = data.type; busitypeid = data.busitypeid;
                    Ext.MessageBox.alert("提示", "保存成功！", function () {
                        loadform();
                    });
                }
                else {
                    Ext.MessageBox.alert("提示", "保存失败！");
                }
            }
        }
    });
}