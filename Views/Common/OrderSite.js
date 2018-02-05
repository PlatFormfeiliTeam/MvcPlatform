var ordercode = getQueryString("ordercode"); var entrusttype = getQueryString("entrusttype");

var curuserRealname = ""; curuserId = "";
var uploader; var insp_uploader;
var file_decl = "[]"; var file_insp = "[]";

Ext.onReady(function () {
    form_ini_base();//表单初始化
   
    if (entrusttype == "01" || entrusttype == "03") {
        form_ini_decl();
    }
    if (entrusttype == "02" || entrusttype == "03") {
        form_ini_insp();
    }    
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

    //隐藏的字段
    var field_CODE = Ext.create('Ext.form.field.Hidden', {
        name: 'CODE'
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
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [container_CLEARREMARK] },
            field_CODE
        ]
    });
}

function form_ini_decl() {
    var label_declinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;现场报关</span></h4>'
    }
    var tbar_decl = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_declinfo]
    });

    //查验
    var chk_ISCHECK = {
        id: 'chk_ISCHECK',
        xtype: 'checkboxfield',
        tabIndex: 2, columnWidth: .10,
        fieldLabel: '查验',
        //boxLabel: '查验',
        name: 'ISCHECK',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue == true) {
                    field_DECLCHECKTIME.setValue(getNowDate());
                    field_DECLCHECKNAME.setValue(curuserRealname);
                    field_DECLCHECKID.setValue(curuserId);
                    field_CHECKREMARK.setValue("");
                    field_CHECKREMARK.setReadOnly(false);
                    field_CHECKREMARK.setFieldStyle('background-color: #FFFFFF; background-image: none;');

                    if (uploader == null) {
                        upload_ini();
                    }

                } else {
                    field_DECLCHECKTIME.setValue("");
                    field_DECLCHECKNAME.setValue("");
                    field_DECLCHECKID.setValue("");
                    field_CHECKREMARK.setValue("");
                    field_CHECKREMARK.setReadOnly(true);
                    field_CHECKREMARK.setFieldStyle('background-color: #CECECE; background-image: none;');

                    if (uploader) {
                        uploader.destroy();
                    }
                }
            }
        }
    };
    //查验时间
    var field_DECLCHECKTIME = Ext.create('Ext.form.field.Text', {
        id: 'field_DECLCHECKTIME',
        name: 'DECLCHECKTIME',
        fieldLabel: '查验时间', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //查验人员
    var field_DECLCHECKNAME = Ext.create('Ext.form.field.Text', {
        id: 'field_DECLCHECKNAME',
        name: 'DECLCHECKNAME',
        fieldLabel: '查验人员', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //查验备注
    var field_CHECKREMARK = Ext.create('Ext.form.field.Text', {
        id: 'field_CHECKREMARK',
        tabIndex: 3, 
        name: 'CHECKREMARK',
        fieldLabel: '查验备注', columnWidth: .35,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    var btn_checkpic = Ext.create('Ext.Button', {
        text: '上传查验图片', flex: .5, id: 'upfiles',
        handler: function () {
            if (!Ext.getCmp("chk_ISCHECK").getValue()) {
                Ext.MessageBox.alert("提示", "无查验标志，不能上传查验图片！");
                return false;
            }
        }
    });
    var btn_showcheckpic = Ext.create('Ext.Button', {
        text: '浏览查验图片', flex: .5,
        handler: function () {
            if (!Ext.getCmp("chk_ISCHECK").getValue()) {
                Ext.MessageBox.alert("提示", "没有查验图片!");
                return false;
            }
            if (file_decl.length <= 0) {
                Ext.MessageBox.alert("提示", "没有查验图片!");
                return false;
            }

            var strview = '';
            for (var i = 0; i < file_decl.length; i++) {
                strview += '<li><img src="' + url + '/file/' + file_decl[i]["FILENAME"] + '" data-original="' + url + '/file/' + file_decl[i]["FILENAME"] + '" alt="图片' + i + '"></li>';
            }
            $('#viewer').html(strview);
            $('#viewer').viewer('update');
            $('#viewer').viewer('show');
        }
    });
    var container_btn_decl = {
        columnWidth: .15,
        xtype: 'fieldcontainer',
        layout: 'hbox',
        //fieldLabel: '',
        items: [btn_checkpic, btn_showcheckpic]
    }


    //稽核
    var chk_AUDITFLAG = {
        id: 'chk_AUDITFLAG',
        xtype: 'checkboxfield',
        tabIndex: 4,
        fieldLabel: '稽核', columnWidth: .10,
        name: 'AUDITFLAG',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue == true) {
                    field_AUDITFLAGTIME.setValue(getNowDate());
                    field_AUDITFLAGNAME.setValue(curuserRealname);
                    field_AUDITFLAGID.setValue(curuserId);
                    field_AUDITCONTENT.setValue("");
                    field_AUDITCONTENT.setReadOnly(false);
                    field_AUDITCONTENT.setFieldStyle('background-color: #FFFFFF; background-image: none;');
                } else {
                    field_AUDITFLAGTIME.setValue("");
                    field_AUDITFLAGNAME.setValue("");
                    field_AUDITFLAGID.setValue("");
                    field_AUDITCONTENT.setValue("");
                    field_AUDITCONTENT.setReadOnly(true);
                    field_AUDITCONTENT.setFieldStyle('background-color: #CECECE; background-image: none;');
                }
            }
        }
    };
    //稽核时间
    var field_AUDITFLAGTIME = Ext.create('Ext.form.field.Text', {
        id: 'field_AUDITFLAGTIME',
        name: 'AUDITFLAGTIME',
        fieldLabel: '稽核时间', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //稽核人员
    var field_AUDITFLAGNAME = Ext.create('Ext.form.field.Text', {
        id: 'field_AUDITFLAGNAME',
        name: 'AUDITFLAGNAME',
        fieldLabel: '稽核人员', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //稽核内容
    var field_AUDITCONTENT = Ext.create('Ext.form.field.Text', {
        id: 'field_AUDITCONTENT',
        tabIndex: 5, 
        name: 'AUDITCONTENT', columnWidth: .35,
        fieldLabel: '稽核内容',
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });


    //现场报关
    var chk_SITEFLAG = {
        id: 'chk_SITEFLAG',
        xtype: 'checkboxfield',
        tabIndex: 6,
        fieldLabel: '现场报关', columnWidth: .10,
        name: 'SITEFLAG',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue == true) {
                    field_SITEAPPLYTIME.setValue(getNowDate());
                    field_SITEAPPLYUSERNAME.setValue(curuserRealname);
                    field_SITEAPPLYUSERID.setValue(curuserId);
                } else {
                    field_SITEAPPLYTIME.setValue("");
                    field_SITEAPPLYUSERNAME.setValue("");
                    field_SITEAPPLYUSERID.setValue("");
                }
            }
        }
    };
    //现场报关时间
    var field_SITEAPPLYTIME = Ext.create('Ext.form.field.Text', {
        id: 'field_SITEAPPLYTIME',
        name: 'SITEAPPLYTIME',
        fieldLabel: '现场报关时间', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //现场报关人员
    var field_SITEAPPLYUSERNAME = Ext.create('Ext.form.field.Text', {
        id: 'field_SITEAPPLYUSERNAMEE',
        name: 'SITEAPPLYUSERNAME',
        fieldLabel: '现场报关人员', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });


    //现场放行
    var chk_PASSFLAG = {
        id: 'chk_PASSFLAG',
        xtype: 'checkboxfield',
        tabIndex: 7,
        fieldLabel: '现场放行', columnWidth: .10,
        name: 'PASSFLAG',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue == true) {
                    field_SITEPASSTIME.setValue(getNowDate());
                    field_SITEPASSUSERNAME.setValue(curuserRealname);
                    field_SITEPASSUSERID.setValue(curuserId);
                } else {
                    field_SITEPASSTIME.setValue("");
                    field_SITEPASSUSERNAME.setValue("");
                    field_SITEPASSUSERID.setValue("");
                }
            }
        }
    };
    //现场放行时间
    var field_SITEPASSTIME = Ext.create('Ext.form.field.Text', {
        id: 'field_SITEPASSTIME',
        name: 'SITEPASSTIME',
        fieldLabel: '现场放行时间', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //现场放行人员
    var field_SITEPASSUSERNAME = Ext.create('Ext.form.field.Text', {
        id: 'field_SITEPASSUSERNAME',
        name: 'SITEPASSUSERNAME',
        fieldLabel: '现场放行人员', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    var field_DECLCHECKID = Ext.create('Ext.form.field.Hidden', { name: 'DECLCHECKID' });
    var field_AUDITFLAGID = Ext.create('Ext.form.field.Hidden', { name: 'AUDITFLAGID' });
    var field_SITEAPPLYUSERID = Ext.create('Ext.form.field.Hidden', { name: 'SITEAPPLYUSERID' });
    var field_SITEPASSUSERID = Ext.create('Ext.form.field.Hidden', { name: 'SITEPASSUSERID' });

    var formpanel_decl = Ext.create('Ext.form.Panel', {
        id: 'formpanel_decl',
        renderTo: 'div_form_decl',
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
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [chk_ISCHECK, field_DECLCHECKTIME, field_DECLCHECKNAME, field_CHECKREMARK, container_btn_decl] },
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [chk_AUDITFLAG, field_AUDITFLAGTIME, field_AUDITFLAGNAME, field_AUDITCONTENT] },
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [chk_SITEFLAG, field_SITEAPPLYTIME, field_SITEAPPLYUSERNAME] },
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [chk_PASSFLAG, field_SITEPASSTIME, field_SITEPASSUSERNAME] },
            field_DECLCHECKID, field_AUDITFLAGID, field_SITEAPPLYUSERID, field_SITEPASSUSERID
        ]
    });
}

function form_ini_insp() {
    var label_inspinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;现场报检</span></h4>'
    }
    var tbar_insp = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_inspinfo]
    });

    //查验
    var chk_INSPISCHECK = {
        id: 'chk_INSPISCHECK',
        xtype: 'checkboxfield',
        tabIndex: 8, columnWidth: .10,
        fieldLabel: '查验',
        //boxLabel: '查验',
        name: 'INSPISCHECK',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue == true) {
                    field_INSPCHECKTIME.setValue(getNowDate());
                    field_INSPCHECKNAME.setValue(curuserRealname);
                    field_INSPCHECKID.setValue(curuserId);
                    field_INSPCHECKREMARK.setValue("");
                    field_INSPCHECKREMARK.setReadOnly(false);
                    field_INSPCHECKREMARK.setFieldStyle('background-color: #FFFFFF; background-image: none;');

                    if (insp_uploader == null) {
                        insp_upload_ini();
                    }

                } else {
                    field_INSPCHECKTIME.setValue("");
                    field_INSPCHECKNAME.setValue("");
                    field_INSPCHECKID.setValue("");
                    field_INSPCHECKREMARK.setValue("");
                    field_INSPCHECKREMARK.setReadOnly(true);
                    field_INSPCHECKREMARK.setFieldStyle('background-color: #CECECE; background-image: none;');

                    if (insp_uploader) {
                        insp_uploader.destroy();
                    }
                }
            }
        }
    };
    //查验时间
    var field_INSPCHECKTIME = Ext.create('Ext.form.field.Text', {
        id: 'field_INSPCHECKTIME',
        name: 'INSPCHECKTIME',
        fieldLabel: '查验时间', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //查验人员
    var field_INSPCHECKNAME = Ext.create('Ext.form.field.Text', {
        id: 'field_INSPCHECKNAME',
        name: 'INSPCHECKNAME',
        fieldLabel: '查验人员', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //查验备注
    var field_INSPCHECKREMARK = Ext.create('Ext.form.field.Text', {
        id: 'field_INSPCHECKREMARK',
        tabIndex: 9, 
        name: 'INSPCHECKREMARK',
        fieldLabel: '查验备注', columnWidth: .35,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    var btn_inspcheckpic = Ext.create('Ext.Button', {
        text: '上传查验图片', flex: .5, id: 'inspupfiles',
        handler: function () {
            if (!Ext.getCmp("chk_INSPISCHECK").getValue()) {
                Ext.MessageBox.alert("提示", "无查验标志，不能上传查验图片！");
                return false;
            }
        }
    });
    var btn_showinspcheckpic = Ext.create('Ext.Button', {
        text: '浏览查验图片', flex: .5,
        handler: function () {
            if (!Ext.getCmp("chk_INSPISCHECK").getValue()) {
                Ext.MessageBox.alert("提示", "没有查验图片!");
                return false;
            }
            if (file_insp.length <= 0) {
                Ext.MessageBox.alert("提示", "没有查验图片!");
                return false;
            }

            var strview = '';
            for (var i = 0; i < file_insp.length; i++) {
                strview += '<li><img src="' + url + '/file/' + file_insp[i]["FILENAME"] + '" data-original="' + url + '/file/' + file_insp[i]["FILENAME"] + '" alt="图片' + i + '"></li>';
            }
            $('#viewer').html(strview);
            $('#viewer').viewer('update');
            $('#viewer').viewer('show');
        }
    });
    var container_btn_insp = {
        columnWidth: .15,
        xtype: 'fieldcontainer',
        layout: 'hbox',
        //fieldLabel: '',
        items: [btn_inspcheckpic, btn_showinspcheckpic]
    }


    //熏蒸
    var chk_ISFUMIGATION = {
        id: 'chk_ISFUMIGATION',
        xtype: 'checkboxfield',
        tabIndex: 10,
        fieldLabel: '熏蒸', columnWidth: .10,
        name: 'ISFUMIGATION',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue == true) {
                    field_FUMIGATIONTIME.setValue(getNowDate());
                    field_FUMIGATIONNAME.setValue(curuserRealname);
                    field_FUMIGATIONID.setValue(curuserId);
                } else {
                    field_FUMIGATIONTIME.setValue("");
                    field_FUMIGATIONNAME.setValue("");
                    field_FUMIGATIONID.setValue("");
                }
            }
        }
    };
    //熏蒸时间
    var field_FUMIGATIONTIME = Ext.create('Ext.form.field.Text', {
        id: 'field_FUMIGATIONTIME',
        name: 'FUMIGATIONTIME',
        fieldLabel: '熏蒸时间', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //熏蒸人员
    var field_FUMIGATIONNAME = Ext.create('Ext.form.field.Text', {
        id: 'field_FUMIGATIONNAME',
        name: 'FUMIGATIONNAME',
        fieldLabel: '熏蒸人员', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });


    //现场报检
    var chk_INSPSITEFLAG = {
        id: 'chk_INSPSITEFLAG',
        xtype: 'checkboxfield',
        tabIndex: 11,
        fieldLabel: '现场报检', columnWidth: .10,
        name: 'INSPSITEFLAG',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue == true) {
                    field_INSPSITEAPPLYTIME.setValue(getNowDate());
                    field_INSPSITEAPPLYUSERNAME.setValue(curuserRealname);
                    field_INSPSITEAPPLYUSERID.setValue(curuserId);
                } else {
                    field_INSPSITEAPPLYTIME.setValue("");
                    field_INSPSITEAPPLYUSERNAME.setValue("");
                    field_INSPSITEAPPLYUSERID.setValue("");
                }
            }
        }
    };
    //现场报检时间
    var field_INSPSITEAPPLYTIME = Ext.create('Ext.form.field.Text', {
        id: 'field_INSPSITEAPPLYTIME',
        name: 'INSPSITEAPPLYTIME',
        fieldLabel: '现场报检时间', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //现场报检人员
    var field_INSPSITEAPPLYUSERNAME = Ext.create('Ext.form.field.Text', {
        id: 'field_INSPSITEAPPLYUSERNAMEE',
        name: 'INSPSITEAPPLYUSERNAME',
        fieldLabel: '现场报检人员', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });


    //现场放行
    var chk_INSPPASSFLAG = {
        id: 'chk_INSPPASSFLAG',
        xtype: 'checkboxfield',
        tabIndex: 12,
        fieldLabel: '现场放行', columnWidth: .10,
        name: 'INSPPASSFLAG',
        listeners: {
            change: function (me, newValue, oldValue, eOpts) {
                if (newValue == true) {
                    field_INSPSITEPASSTIME.setValue(getNowDate());
                    field_INSPSITEPASSUSERNAME.setValue(curuserRealname);
                    field_INSPSITEPASSUSERID.setValue(curuserId);
                } else {
                    field_INSPSITEPASSTIME.setValue("");
                    field_INSPSITEPASSUSERNAME.setValue("");
                    field_INSPSITEPASSUSERID.setValue("");
                }
            }
        }
    };
    //现场放行时间
    var field_INSPSITEPASSTIME = Ext.create('Ext.form.field.Text', {
        id: 'field_INSPSITEPASSTIME',
        name: 'INSPSITEPASSTIME',
        fieldLabel: '现场放行时间', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });
    //现场放行人员
    var field_INSPSITEPASSUSERNAME = Ext.create('Ext.form.field.Text', {
        id: 'field_INSPSITEPASSUSERNAME',
        name: 'INSPSITEPASSUSERNAME',
        fieldLabel: '现场放行人员', columnWidth: .20,
        readOnly: true, fieldStyle: 'background-color: #CECECE; background-image: none;'
    });

    var field_INSPCHECKID = Ext.create('Ext.form.field.Hidden', { name: 'INSPCHECKID' });
    var field_FUMIGATIONID = Ext.create('Ext.form.field.Hidden', { name: 'FUMIGATIONID' });
    var field_INSPSITEAPPLYUSERID = Ext.create('Ext.form.field.Hidden', { name: 'INSPSITEAPPLYUSERID' });
    var field_INSPSITEPASSUSERID = Ext.create('Ext.form.field.Hidden', { name: 'INSPSITEPASSUSERID' });

    var formpanel_insp = Ext.create('Ext.form.Panel', {
        id: 'formpanel_insp',
        renderTo: 'div_form_insp',
        minHeight: 200,
        border: 0,
        tbar: tbar_insp,
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
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [chk_INSPISCHECK, field_INSPCHECKTIME, field_INSPCHECKNAME, field_INSPCHECKREMARK, container_btn_insp] },
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [chk_ISFUMIGATION, field_FUMIGATIONTIME, field_FUMIGATIONNAME] },
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [chk_INSPSITEFLAG, field_INSPSITEAPPLYTIME, field_INSPSITEAPPLYUSERNAME] },
            { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [chk_INSPPASSFLAG, field_INSPSITEPASSTIME, field_INSPSITEPASSUSERNAME] },
            field_INSPCHECKID, field_FUMIGATIONID, field_INSPSITEAPPLYUSERID, field_INSPSITEPASSUSERID
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
        id:'formpanel_btn',
        renderTo: 'div_form_btn',
        border: 0,
        bbar: bbar
    });
}

function loadform() {
    Ext.Ajax.request({
        url: "/Common/loadorder_site",
        params: { ordercode: ordercode },
        success: function (response, opts) {
            var data = Ext.decode(response.responseText);
            Ext.getCmp("formpanel_base").getForm().setValues(data.formdata);

            if (entrusttype == "01" || entrusttype == "03") {
                Ext.getCmp("formpanel_decl").getForm().setValues(data.formdata);

                if (data.formdata.SITEAPPLYTIME != "" && data.formdata.SITEAPPLYTIME != null) { Ext.getCmp("chk_SITEFLAG").setReadOnly(true); }
                if (data.formdata.SITEPASSTIME != "" && data.formdata.SITEPASSTIME != null) { Ext.getCmp("chk_PASSFLAG").setReadOnly(true); }
            }
            if (entrusttype == "02" || entrusttype == "03") {
                Ext.getCmp("formpanel_insp").getForm().setValues(data.formdata);

                if (data.formdata.INSPSITEAPPLYTIME != "" && data.formdata.INSPSITEAPPLYTIME != null) { Ext.getCmp("chk_INSPSITEFLAG").setReadOnly(true); }
                if (data.formdata.INSPSITEPASSTIME != "" && data.formdata.INSPSITEPASSTIME != null) { Ext.getCmp("chk_INSPPASSFLAG").setReadOnly(true); }
            }

            file_decl = data.filedata_decl;
            file_insp = data.filedata_insp;

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

    var formpanel_decl = "{}", formpanel_insp = "{}";
    if (entrusttype == "01" || entrusttype == "03") {
        formpanel_decl = Ext.encode(Ext.getCmp("formpanel_decl").getForm().getValues());
    }
    if (entrusttype == "02" || entrusttype == "03") {
        formpanel_insp = Ext.encode(Ext.getCmp("formpanel_insp").getForm().getValues());
    }

    //var filedata = Ext.encode(Ext.pluck(file_store.data.items, 'data'));

    var mask = new Ext.LoadMask(Ext.get(Ext.getBody()), { msg: "数据保存中，请稍等..." });
    mask.show();

    Ext.Ajax.request({
        url: "/Common/saveorder_site",
        params: { formpanel_base: formpanel_base, formpanel_decl: formpanel_decl, formpanel_insp: formpanel_insp },//, filedata: filedata
        success: function (response, option) {
            if (response.responseText) {
                mask.hide();
                var data = Ext.decode(response.responseText);
                if (data.success) {
                    ordercode = data.ordercode;
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

function upload_ini() {
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "正在上传，请稍等..." });

    uploader = new plupload.Uploader({
        runtimes: 'html5,flash,silverlight,html4',
        browse_button: 'upfiles', // you can pass an id...
        url: '/Common/UploadFile_site',
        flash_swf_url: '/js/upload/Moxie.swf',
        silverlight_xap_url: '/js/upload/Moxie.xap',
        unique_names: true,
        filters: {
            max_file_size: '5000mb',
            mime_types: [
                { title: "Image files", extensions: "jpg,gif,png" }
            ]
        },
        multipart_params: { ordercode: ordercode, filetype: "67", formpanel_decl: Ext.encode(Ext.getCmp("formpanel_decl").getForm().getValues()) }
    });
    uploader.init();
    uploader.bind('FilesAdded', function (up, files) {
        myMask.show();
        uploader.start();
    });
    uploader.bind('FileUploaded', function (up, file) {

    });
    uploader.bind('UploadComplete', function (up, files) {
        myMask.hide();
        loadform();
        Ext.MessageBox.alert("提示", "上传成功！");
    });
}

function insp_upload_ini() {
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "正在上传，请稍等..." });

    insp_uploader = new plupload.Uploader({
        runtimes: 'html5,flash,silverlight,html4',
        browse_button: 'inspupfiles', // you can pass an id...
        url: '/Common/UploadFile_site',
        flash_swf_url: '/js/upload/Moxie.swf',
        silverlight_xap_url: '/js/upload/Moxie.xap',
        unique_names: true,
        filters: {
            max_file_size: '5000mb',
            mime_types: [
                { title: "Image files", extensions: "jpg,gif,png" }
            ]
        },
        multipart_params: { ordercode: ordercode, filetype: "68", formpanel_insp: Ext.encode(Ext.getCmp("formpanel_insp").getForm().getValues()) }
    });
    insp_uploader.init();
    insp_uploader.bind('FilesAdded', function (up, files) {
        myMask.show();
        insp_uploader.start();
    });
    insp_uploader.bind('FileUploaded', function (up, file) {
    });
    insp_uploader.bind('UploadComplete', function (up, files) {
        myMask.hide();
        Ext.MessageBox.alert("提示", "上传成功！", function () {
            loadform();
        });
    });
}
