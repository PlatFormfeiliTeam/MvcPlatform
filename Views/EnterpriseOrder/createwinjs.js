﻿/*****************************************************win 窗口 begin ********************************************/
function addwin(ID, busitype) {
    form_ini();
    var win = Ext.create("Ext.window.Window", {
        id: "wjcsWin",
        title: '委托信息',
        width: 820,
        height: 620,
        modal: true,
        items: [Ext.getCmp('formpanel_u'), Ext.getCmp('panelfile')],
    });
    win.show();
    loadform(ID, busitype);
}

function form_ini() {

    //文件接收单位
    var field_FILERECEVIEUNIT = Ext.create('Ext.form.field.Text', {
        id:'field_FILERECEVIEUNIT',
        readOnly: true,
        name: 'FILERECEVIEUNIT',
        margin: 0,
        flex: .90,
        allowBlank: false,
        blankText: '文件接收单位不能为空!'
    })
    var field_FILERECEVIEUNITNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'FILERECEVIEUNITNAME'
    })
    var cont_wjjsdw = Ext.create('Ext.form.FieldContainer', {
        fieldLabel: '文件接收单位',
        layout: 'hbox',
        items: [field_FILERECEVIEUNIT,
            {
                xtype: 'button', id: 'btn_filerecevieunit', listeners: {
                    click: function () { bgsbdw_win(field_FILERECEVIEUNIT); }
                }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .10, margin: 0
            }
        ]
    })

    //文件申报单位
    var field_FILEDECLAREUNIT = Ext.create('Ext.form.field.Text', {
        id:'field_FILEDECLAREUNIT',
        name: 'FILEDECLAREUNIT',
        readOnly: true,
        margin: 0,
        flex: .90,
    });
    var field_FILEDECLAREUNITNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'FILEDECLAREUNITNAME'
    })
    var cont_wjsbdw = Ext.create('Ext.form.FieldContainer', {
        fieldLabel: '文件申报单位',
        layout: 'hbox',
        items: [field_FILEDECLAREUNIT,
            {
                xtype: 'button', id: 'btn_filedeclareunit', listeners: {
                    click: function () { bgsbdw_win(field_FILEDECLAREUNIT); }
                }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .10, margin: 0
            }
        ]
    })

    //业务类型
    var store_busitype = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_busitype
    });
    var combo_BUSITYPE = Ext.create('Ext.form.field.ComboBox', {
        id:'combo_BUSITYPE',
        name: 'BUSITYPEID',
        store: store_busitype,
        fieldLabel: '业务类型',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        readOnly: true,
        forceSelection: true,//选中后，修改时，是否需要必须选中
        queryMode: 'local',
        anyMatch: true,
        allowBlank: false,
        blankText: '业务类型不能为空!'
    });

    //申报关区
    var store_CUSTOMDISTRICTNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbgq
    })
    var combo_CUSTOMDISTRICTNAME = Ext.create('Ext.form.field.ComboBox', {//申报关区 这个数据比较多需要根据输入字符到后台动态模糊匹配
        name: 'CUSTOMDISTRICTCODE',
        store: store_CUSTOMDISTRICTNAME,
        fieldLabel: '申报关区',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        minChars: 2,
        hideTrigger: true,
        //forceSelection: true,
        anyMatch: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                }
            },
            select: function (cb, records, eOpts) {
                field_CUSTOMDISTRICTNAME.setValue(cb.getRawValue().substr(0, cb.getRawValue().lastIndexOf('(')));
            }
        },
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    var field_CUSTOMDISTRICTNAME = Ext.create('Ext.form.field.Hidden', {
        name: 'CUSTOMDISTRICTNAME'
    })

    //申报方式
    var store_REPWAYNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbfs
    })
    var combo_REPWAYNAME = Ext.create('Ext.form.field.ComboBox', {
        name: 'REPWAYID',
        store: store_REPWAYNAME,
        fieldLabel: '申报方式',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        queryMode: 'local',
        anyMatch: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.store.clearFilter();
                    cb.expand()
                };
            }
        }
    })

    var store_status = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: [{ "CODE": 5, "NAME": "未提交" }, { "CODE": 10, "NAME": "已提交" }, { "CODE": 15, "NAME": "已受理" }]
    })
    var field_STATUS = Ext.create('Ext.form.field.ComboBox', {//业务状态
        id: 'field_status1',
        name: 'STATUS',
        valueField: 'CODE',
        displayField: 'NAME',
        fieldLabel: '状态',
        queryMode: 'local',
        editable: false,
        hiddenTrigger: true,
        readOnly: true,
        labelWidth: 80,
        value: 5,
        store: store_status
    });

    //备注
    var field_REMARK = Ext.create('Ext.form.field.Text', {
        id: 'field_REMARK1',
        fieldLabel: '需求备注',
        name: 'REMARK'
    });

    //企业编号
    var field_CODE = Ext.create('Ext.form.field.Text', {
        id: 'CODE',
        fieldLabel: '企业编号',
        name: 'CODE',
        allowBlank: false,
        blankText: '企业编号不能为空!'
    });

    var field_ISUPLOAD = Ext.create('Ext.form.field.Hidden', {
        id: 'field_ISUPLOAD',
        name: 'ISUPLOAD'
    })

    var bbar_l = '<div class="btn-group">'
           + '<button type="button" class="btn btn-primary btn-sm" id="pickfiles"><i class="fa fa-upload"></i>&nbsp;上传文件</button>'
           + '<button type="button" onclick="browsefile()" class="btn btn-primary btn-sm"><i class="fa fa-exchange fa-fw"></i>&nbsp;浏览文件</button>'
           + '<button type="button" onclick="removeFile()" class="btn btn-primary btn-sm" id="deletefile"><i class="fa fa-trash-o"></i>&nbsp;删除文件</button>'
       + '</div>';
    var bbar_r = '<div class="btn-group">'
    + '<button type="button" onclick="save()" class="btn btn-primary btn-sm" id="btn_saveorder"><i class="fa fa-floppy-o"></i>&nbsp;保存</button>'
    + '<button type="button" onclick="submit()" class="btn btn-primary btn-sm" id="btn_submitorder"><i class="fa fa-hand-o-up"></i>&nbsp;传送</button>'
    + '</div>';

    var buttombar = Ext.create('Ext.toolbar.Toolbar', {
        items: [bbar_l, '->', bbar_r]
    })

    var formpanel_u = Ext.create('Ext.form.Panel', {
        id: 'formpanel_u',
        bbar: buttombar,
        border: 0,
        minWidth: 800,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: .50,
            labelSeparator: '',
            msgTarget: 'under',
            labelAlign: 'right',
            validateOnBlur: false,
            validateOnChange: false
        },
        items: [
            { layout: 'column', height: 42, border: 0, margin: '5 0 0 0', items: [cont_wjjsdw, cont_wjsbdw] },
            { layout: 'column', height: 42, border: 0, items: [combo_BUSITYPE, combo_REPWAYNAME] },
            { layout: 'column', height: 42, border: 0, items: [combo_CUSTOMDISTRICTNAME, field_CODE] },
            { layout: 'column', height: 42, border: 0, items: [field_REMARK, field_STATUS] },
            field_FILERECEVIEUNITNAME, field_FILEDECLAREUNITNAME, field_CUSTOMDISTRICTNAME, field_ISUPLOAD
        ]
    })

    var storefile = Ext.create('Ext.data.JsonStore', {
        fields: ['FILENAME', 'NEWNAME', 'ORIGINALNAME', 'UPLOADTIME', 'SIZES']
    });

    var tmp = new Ext.XTemplate(
        '<tpl for=".">',
        '<div class="panel panel-default thumb-wrap fl" style="margin-top:5px;margin-left:5px;width:240px">',
        '<div class="panel-heading" style="padding-left:5px;padding-right:5px">{[values.ORIGINALNAME.substr(0,23)]}<div class="fr"><span class="glyphicon glyphicon-paperclip"></span></div></div>',
        '<div class="panel-body" style="padding-left:5px;">',
        '<tpl>{[values.SIZES/1024 > 1024?Math.round(values.SIZES/(1024*1024))+"M":Math.round(values.SIZES/1024)+"K"]}</tpl>',
        '|{[values.UPLOADTIME]}</div></div>',
        '</tpl>'
        )
    var fileview = Ext.create('Ext.view.View', {
        id: 'fileview1',
        store: storefile,
        tpl: tmp,
        itemSelector: 'div.thumb-wrap',
        multiSelect: true
    })
    var panelfile = Ext.create('Ext.panel.Panel', {
        id: 'panelfile',
        border: 0,
        width: '100%',
        minHeight: 500,
        items: [fileview]
    })
}

function loadform(ID, busitype) {
    Ext.Ajax.request({
        url: "/EnterpriseOrder/loadform",
        params: { ID: ID, busitype: busitype },
        success: function (response, opts) {
            var data = Ext.decode(response.responseText);

            Ext.getCmp('formpanel_u').getForm().setValues(data.data);
            Ext.getCmp('fileview1').store.loadData(data.filedata);//加载附件列表数据

            var status = Ext.getCmp('field_status1').getValue();
            button_control(status);//按钮的控制
        }
    });
}

function button_control(status) {
    if (status >= 10) {
        document.getElementById("pickfiles").disabled = true;
    } else {
        upload_ini(); //未提交时才初始化上传控件
    }

    document.getElementById("deletefile").disabled = status >= 10; //删除按钮
    document.getElementById("btn_saveorder").disabled = status >= 10;//保存按钮
    document.getElementById("btn_submitorder").disabled = status >= 10;//提交按钮
}

function upload_ini() {
    var uploader = new plupload.Uploader({
        runtimes: 'html5,flash,silverlight,html4',
        browse_button: 'pickfiles', // you can pass an id...
        url: '/EnterpriseOrder/Upload_WebServer',
        flash_swf_url: '/js/upload/Moxie.swf',
        silverlight_xap_url: '/js/upload/Moxie.xap',
        unique_names: true,
        filters: {
            max_file_size: '5000mb',
            mime_types: [
                { title: "Image files", extensions: "*" },
                { title: "Zip files", extensions: "zip,rar" }
            ]
        }
    });
    uploader.init();
    uploader.bind('FilesAdded', function (up, files) {
        uploader.start();
    });

    uploader.bind('FileUploaded', function (up, file) {        
        var curtime = GetCurTime(); 
        Ext.getCmp('fileview1').store.insert(Ext.getCmp('fileview1').store.data.length
            , { FILENAME: '/FileUpload/file/' + file.target_name, NEWNAME: file.target_name, ORIGINALNAME: file.name, SIZES: file.size, UPLOADTIME: curtime });
    });
}

//删除文件
function removeFile() {
    var fileview = Ext.getCmp('fileview1');
    var records = fileview.getSelectionModel().getSelection();
    if (records.length == 0) {
        Ext.MessageBox.alert("提示", "请选择要删除的记录！");
        return
    }
    Ext.MessageBox.confirm('提示', '确定要删除选择的记录吗？', function (btn) {
        if (btn == 'yes') {
            var records = fileview.getSelectionModel().getSelection();
            fileview.store.remove(records);
        }
    })
}

function browsefile() {

    var fileview = Ext.getCmp('fileview1');
    var records = fileview.getSelectionModel().getSelection();
    if (records.length == 0) {
        Ext.MessageBox.alert("提示", "请选择要浏览的记录！");
        return
    }
    var win = Ext.create("Ext.window.Window", {
        title: "文件预览",
        width: 800,
        height: 600,
        layout: "fit",
        modal: true,
        closeAction: "destroy",
        items: [{
            html: "<div id='fileViewDiv' style='height: 100%;width: 100%;'></div>"
        }]
    });

    win.show();
    if (Ext.getCmp('field_ISUPLOAD').getValue() == "1") {//文件已经到另一台server上
        document.getElementById('fileViewDiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="' + AdminUrl + '\/file' + records[0].get("FILENAME") + '"></embed>';
    }
    else {
        document.getElementById('fileViewDiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="' + records[0].get("FILENAME") + '"></embed>';
    }
}

function save(action) {
    var data = Ext.encode(Ext.getCmp('formpanel_u').getForm().getValues());
    var filedata = Ext.encode(Ext.pluck(Ext.getCmp('fileview1').store.data.items, 'data'));

    var validate = "";
    if (validate) {
        Ext.MessageBox.alert("提示", validate);
        return;
    }

    var mask = new Ext.LoadMask(Ext.getBody(), { msg: "数据保存中，请稍等..." });
    mask.show();
    Ext.Ajax.request({
        url: "/EnterpriseOrder/Save",
        params: { ID: ID, data: data, filedata: filedata, action: action },
        success: function (response, option) {
            if (response.responseText) {
                mask.hide();
                var data = Ext.decode(response.responseText);
                if (data.result) {
                    Ext.MessageBox.alert("提示", data.result);
                }
                else {
                    Ext.MessageBox.alert("提示", action == 'submit' ? "提交成功！" : "保存成功！", function () {
                        ID = data.ID;

                        if (action == "submit") {
                            /*
                            Ext.getCmp('field_status1').setValue(10);
                            button_control(Ext.getCmp('field_status1').getValue());//按钮的控制
                            */
                            Ext.getCmp('wjcsWin').close();
                        }
                        Select();//重新加载查询列表页
                    });
                }
            }
        }
    });
}

function submit() {
    if (Ext.getCmp('field_status1').getValue() == 15) {//15表示订单已委托
        Ext.MessageBox.alert('提示', '已传送的文件不能再次传送！');
        return;
    }
    if (Ext.getCmp('fileview1').store.data.items.length == 0) { //如果是提交,必须上传文件
        Ext.MessageBox.alert('提示', '传送前需上传随附文件！');
        return;
    }

    //保存时不验证，提交委托时验证
    var msg = "";

    if (!Ext.getCmp('formpanel_u').getForm().isValid()) {
        msg = "数据验证未通过！";
    }

    if (msg) {
        Ext.MessageBox.alert('提示', msg);
        return
    }

    var bf = true;
    if (Ext.getCmp('combo_BUSITYPE').getValue() == "40" || Ext.getCmp('combo_BUSITYPE').getValue() == "41") {
        if (Ext.getCmp('field_FILERECEVIEUNIT').getValue() == Ext.getCmp('field_FILEDECLAREUNIT').getValue()) {
            bf = false;
        }
    }

    if (bf) {
        save("submit");
    } else {
        Ext.MessageBox.confirm('提示', '文件接收单位、文件申报单位一致,确定要传送吗？', function (btn) {
            if (btn == 'yes') {
                save("submit");
            }
        })
    }

}

function GetCurTime() {
    var nd = new Date();

    var y = nd.getFullYear();
    var m = nd.getMonth() + 1;
    var d = nd.getDate();
    var h = nd.getHours();
    var mi = nd.getMinutes();
    var s = nd.getSeconds();

    if (m <= 9) m = "0" + m;
    if (d <= 9) d = "0" + d;
    if (h <= 9) h = "0" + h;
    if (mi <= 9) mi = "0" + mi;
    if (s <= 9) s = "0" + s;

    var cdate = y + "-" + m + "-" + d + " " + h + ":" + mi + ":" + s;
    return cdate;
}

/*****************************************************win 窗口 end ********************************************/