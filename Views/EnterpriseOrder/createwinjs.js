/*****************************************************win 窗口 begin ********************************************/
var repwayidcode;

function addwin(ID) {
    form_ini();
    var win = Ext.create("Ext.window.Window", {
        id: "wjcsWin",
        title: '委托信息',
        width: 820,
        height: 480,
        modal: true,
        items: [Ext.getCmp('formpanel_u'), Ext.getCmp('panelfile')],
    });
    win.show();
    loadform(ID);
    upload_ini();
}

function form_ini() {
    //统一编号
    var field_UNITCODE = Ext.create('Ext.form.field.Text', {
        id: 'UNITCODE',
        fieldLabel: '统一编号',
        name: 'UNITCODE',
        readOnly: true,
        emptyText: '统一编号自动生成'
    });

    //生成方式
    var store_createmode = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: [{ "CODE": "按批次", "NAME": "按批次" }, { "CODE": "按文件", "NAME": "按文件" }]
    });
    var combo_CREATEMODE = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_CREATEMODE',
        name: 'CREATEMODE',
        store: store_createmode,
        fieldLabel: '生成方式',
        displayField: 'NAME',
        queryMode: 'local',
        valueField: 'CODE',
        triggerAction: 'all',
        value: '按批次',
        editable: false
    });


    //文件接收单位
    var field_FILERECEVIEUNIT = Ext.create('Ext.form.field.Text', {
        id: 'field_FILERECEVIEUNIT',
        readOnly: true,
        name: 'FILERECEVIEUNITNAME',
        margin: 0,
        flex: .90
    })
    var cont_wjjsdw = Ext.create('Ext.form.FieldContainer', {
        fieldLabel: '文件接收单位',
        layout: 'hbox',
        items: [field_FILERECEVIEUNIT,
            {
                xtype: 'button', id: 'btn_filerecevieunit', handler: function () {
                    bgsbdw_win(field_FILERECEVIEUNIT);
                }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .10, margin: 0
            }
        ]
    })

    //文件申报单位
    var field_FILEDECLAREUNIT = Ext.create('Ext.form.field.Text', {
        id: 'field_FILEDECLAREUNIT',
        name: 'FILEDECLAREUNITNAME',
        readOnly: true,
        margin: 0,
        flex: .90,
    });
    var field_ID = Ext.create('Ext.form.field.Hidden', {
        name: 'ID'
    })
    var field_ORIGINALFILEIDS = Ext.create('Ext.form.field.Hidden', {
        id: 'field_ORIGINALFILEIDS',
        name: 'ORIGINALFILEIDS'
    });
    var cont_wjsbdw = Ext.create('Ext.form.FieldContainer', {
        fieldLabel: '文件申报单位',
        layout: 'hbox',
        items: [field_FILEDECLAREUNIT,
            {
                xtype: 'button', id: 'btn_filedeclareunit', handler: function () {
                    bgsbdw_win(field_FILEDECLAREUNIT);
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
        id: 'combo_BUSITYPE',
        name: 'BUSITYPEID',
        store: store_busitype,
        fieldLabel: '业务类型',
        displayField: 'NAME',
        valueField: 'CODE',
        triggerAction: 'all',
        editable: false,
        queryMode: 'local',
        listeners: {
            change: function (combo, records) {
                combo_REPWAYNAME.reset();
                Ext.Ajax.request({
                    url: "/EnterPriseOrder/Ini_Base_Data_REPWAY",
                    params: { busitype: combo_BUSITYPE.getRawValue() },
                    success: function (response, opts) {
                        var commondata = Ext.decode(response.responseText);
                        common_data_sbfs = commondata.sbfs;//申报方式
                        store_REPWAYNAME.loadData(common_data_sbfs);

                        var rec = store_REPWAYNAME.findRecord('CODE', repwayidcode);
                        if (!rec) {
                            repwayidcode = "";//找不到为空
                        }
                        combo_REPWAYNAME.setValue(repwayidcode);//编辑页赋值
                    }
                });
            }
        }
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
    //备注
    var field_REMARK = Ext.create('Ext.form.field.Text', {
        id: 'field_REMARK1',
        fieldLabel: '备注',
        name: 'REMARK'
    });

    //企业编号
    var field_CODE = Ext.create('Ext.form.field.Text', {
        fieldLabel: '企业编号',
        name: 'CODE'
    });
    var bbar_l = '<div class="btn-group">'
           + '<button type="button" class="btn btn-primary btn-sm" id="pickfiles"><i class="fa fa-upload"></i>&nbsp;上传文件</button>'
           + '<button type="button" onclick="browsefile()" class="btn btn-primary btn-sm"><i class="fa fa-exchange fa-fw"></i>&nbsp;浏览文件</button>'
           + '<button type="button" onclick="removeFile()" class="btn btn-primary btn-sm" id="deletefile"><i class="fa fa-trash-o"></i>&nbsp;删除文件</button>'
       + '</div>';
    var bbar_r = '<div class="btn-group">'
    + '<button type="button" onclick="save()" class="btn btn-primary btn-sm" id="btn_saveorder"><i class="fa fa-floppy-o"></i>&nbsp;保存</button>'
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
            labelAlign: 'right'
        },
        items: [
            { layout: 'column', height: 42, border: 0, margin: '5 0 0 0', items: [field_UNITCODE, combo_CREATEMODE] },
            { layout: 'column', height: 42, border: 0, items: [cont_wjjsdw, cont_wjsbdw] },
            { layout: 'column', height: 42, border: 0, items: [combo_BUSITYPE, combo_REPWAYNAME] },
            { layout: 'column', height: 42, border: 0, items: [combo_CUSTOMDISTRICTNAME, field_CODE] },
            { layout: 'column', height: 42, border: 0, items: [field_REMARK] },
            field_CUSTOMDISTRICTNAME, field_ID, field_ORIGINALFILEIDS
        ]
    })

    var storefile = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'FILENAME', 'NEWNAME', 'ORIGINALNAME', 'UPLOADTIME', 'SIZES']
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
function loadform(ID) {
    Ext.Ajax.request({
        url: "/EnterpriseOrder/loadform",
        params: { ID: ID },
        success: function (response, opts) {
            var data = Ext.decode(response.responseText);
            repwayidcode = data.data.REPWAYID;//二次联动，前一个赋值后，调用此值 
            Ext.getCmp('formpanel_u').getForm().setValues(data.data);
            //文件接收单位
            if (Ext.getCmp('field_FILERECEVIEUNIT').getValue()) {
                Ext.getCmp('field_FILERECEVIEUNIT').setValue(Ext.getCmp('field_FILERECEVIEUNIT').getValue() + '(' + data.data.FILERECEVIEUNITCODE + ')');
            }
            //文件申报单位
            if (Ext.getCmp('field_FILEDECLAREUNIT').getValue()) {
                Ext.getCmp('field_FILEDECLAREUNIT').setValue(Ext.getCmp('field_FILEDECLAREUNIT').getValue() + '(' + data.data.FILEDECLAREUNITCODE + ')');
            }
            //文件明细
            Ext.getCmp('fileview1').store.loadData(data.filedata);//加载附件列表数据
            //如果是修改需要将随附文件的ID拼接成字符串 赋值到
            var fileids = "";
            Ext.each(Ext.getCmp('fileview1').store.getRange(), function (rec) {
                fileids += rec.get("ID") + ",";
            });
            Ext.getCmp('field_ORIGINALFILEIDS').setValue(fileids);
        }
    });
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
            max_file_size: '500mb',
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
        var timestamp = Ext.Date.now();  //1351666679575  这个方法只是获取的时间戳
        var date = new Date(timestamp);
        Ext.getCmp('fileview1').store.insert(Ext.getCmp('fileview1').store.data.length
            , { FILENAME: '/FileUpload/file/' + file.target_name, NEWNAME: file.target_name, ORIGINALNAME: file.name, SIZES: file.size, UPLOADTIME: Ext.Date.format(date, 'Y-m-d H:i:s') });
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
        width: 1000,
        height: 600,
        layout: "fit",
        modal: true,
        closeAction: "destroy",
        items: [{
            html: "<div id='fileViewDiv' style='height: 100%;width: 100%;'></div>"
        }]
    });
    win.show();
    if (records[0].get("ID")) {//文件已经到另一台server上
        // 2016-09-23/E1C1637027_sheet.txt  records[0].get("FILENAME")             
        //var position = records[0].get("FILENAME").lastIndexOf(".");
        //var suffix = records[0].get("FILENAME").substr(position + 1);
        // if (suffix == 'pdf' || suffix == 'PDF') {
        document.getElementById('fileViewDiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="' + common_data_adminurl + '\/file' + records[0].get("FILENAME") + '"></embed>';
        //}
        //else {
        //    var newpath = records[0].get("FILENAME").substr(0, position + 1) + 'pdf';
        //    document.getElementById('fileViewDiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="' + common_data_adminurl + '\/file' + newpath + '"></embed>';
        //}
    }
    else {
        document.getElementById('fileViewDiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="' + records[0].get("FILENAME") + '"></embed>';
    }
}

function save(action) {
    if (Ext.getCmp('formpanel_u').getForm().isValid()) {
        if (Ext.getCmp('fileview1').store.data.items.length == 0) { //如果是提交,必须上传文件
            Ext.MessageBox.alert('提示', '随附文件不能为空！');
            return;
        }
        var data = Ext.encode(Ext.getCmp('formpanel_u').getForm().getValues());
        var filedata = Ext.encode(Ext.pluck(Ext.getCmp('fileview1').store.data.items, 'data'));
        var mask = new Ext.LoadMask(Ext.getBody(), { msg: "数据保存中，请稍等..." });
        mask.show();
        Ext.Ajax.request({
            url: "/EnterpriseOrder/Save",
            params: { data: data, filedata: filedata },
            success: function (response, option) {
                mask.hide();
                var data = Ext.decode(response.responseText);
                if (data.success) {
                    Ext.MessageBox.alert("提示", "保存成功！", function () {
                        Ext.getCmp('wjcsWin').close();
                        Ext.getCmp('pgbar').moveFirst();
                    });
                }
                else {
                    Ext.MessageBox.alert("提示", "保存失败！");
                }
            }

        });
    }
}

/*****************************************************win 窗口 end ********************************************/