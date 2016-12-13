function panel_file_ini() {
    var store_filetype1 = Ext.create('Ext.data.JsonStore', {
        fields: ['FILETYPEID', 'FILETYPENAME'],
        data: [{ FILETYPEID: '44', FILETYPENAME: '订单文件' },
            { FILETYPEID: '58', FILETYPENAME: '配舱单文件' }]
    })
    var combo_filetype1 = Ext.create('Ext.form.field.ComboBox', {//文件类型
        name: 'FILETYPEID',
        store: store_filetype1,
        //fieldLabel: '文件类型',
        displayField: 'FILETYPENAME',
        valueField: 'FILETYPEID',
        queryMode: 'local',
        //labelWidth: 50,
        labelAlign: 'right',
        width: 75,
        value: '44',
        editable: false
    })
    var store_ietype1 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: ietype_js_data
    })
    var combo_ietype1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ietype1',
        store: store_ietype1,
        labelAlign: 'right',
        //fieldLabel: '进出口类型',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        editable: false,
        //labelWidth: 60,
        width: 90,
        value: '进/出口业务'
    })
    var field_fileno1 = Ext.create('Ext.form.field.Text', {
        id: 'field_fileno1',
        labelWidth: 55,
        fieldLabel: '统一编号',
        width: 158,
        listeners: {
            specialkey: function (field, e) {
                // e.HOME, e.END, e.PAGE_UP, e.PAGE_DOWN,e.TAB, e.ESC, arrow keys: e.LEFT, e.RIGHT, e.UP, e.DOWN 
                if (e.getKey() == e.ENTER) {
                    var fileuniteno = field.getValue();
                    Ext.Ajax.request({
                        url: "/Common/LoadEnterpriseFile",
                        params: { fileuniteno: fileuniteno },
                        success: function (response, option) {
                            var data = Ext.decode(response.responseText);
                            if (data.success) {
                                Ext.MessageBox.alert("提示", "文件加载成功！", function () {
                                    field.setValue("");
                                    var cb_filetype = toolbar1.items.items[0];
                                    var filetype = cb_filetype.getValue();
                                    var filetypename = cb_filetype.getRawValue();
                                    var ietype = toolbar1.items.items[1].getValue();
                                    var timestamp = Ext.Date.now();  //1351666679575  这个方法只是获取的时间戳
                                    var date = new Date(timestamp);

                                    if (tabpanel.getActiveTab().title == "原始订单") {
                                        if (ietype == "进/出口业务") {
                                            Ext.each(data.data, function (item) {
                                                store_file1.insert(store_file1.data.length,
                                                { FILENAME: '/FileUpload/file/' + item.ORIGINALNAME, ORIGINALNAME: item.ORIGINALNAME, SIZES: item.SIZES, FILETYPENAME: filetypename, FILETYPE: filetype, IETYPE: '仅进口', UPLOADTIME: Ext.Date.format(date, 'Y-m-d H:i:s') });
                                                store_file1.insert(store_file1.data.length,
                                                { FILENAME: '/FileUpload/file/' + item.ORIGINALNAME, ORIGINALNAME: item.ORIGINALNAME, SIZES: item.SIZES, FILETYPENAME: filetypename, FILETYPE: filetype, IETYPE: '仅出口', UPLOADTIME: Ext.Date.format(date, 'Y-m-d H:i:s') });
                                            })
                                        }
                                        else {
                                            Ext.each(data.data, function (item) {
                                                store_file1.insert(store_file1.data.length,
                                               { FILENAME: '/FileUpload/file/' + item.ORIGINALNAME, ORIGINALNAME: item.ORIGINALNAME, SIZES: item.SIZES, FILETYPENAME: filetypename, FILETYPE: filetype, IETYPE: ietype, UPLOADTIME: Ext.Date.format(date, 'Y-m-d H:i:s') });
                                            })
                                        }
                                    }
                                    else {
                                        if (ietype == "进/出口业务") {
                                            Ext.each(data.data, function (item) {
                                                store_file2.insert(store_file2.data.length,
                                               { FILENAME: '/FileUpload/file/' + item.ORIGINALNAME, ORIGINALNAME: item.ORIGINALNAME, SIZES: item.SIZES, FILETYPENAME: filetypename, FILETYPE: filetype, IETYPE: '仅进口', UPLOADTIME: Ext.Date.format(date, 'Y-m-d H:i:s') });
                                                store_file2.insert(store_file2.data.length,
                                               { FILENAME: '/FileUpload/file/' + item.ORIGINALNAME, ORIGINALNAME: item.ORIGINALNAME, SIZES: item.SIZES, FILETYPENAME: filetypename, FILETYPE: filetype, IETYPE: '仅出口', UPLOADTIME: Ext.Date.format(date, 'Y-m-d H:i:s') });
                                            })
                                        }
                                        else {
                                            Ext.each(data.data, function (item) {
                                                store_file2.insert(store_file2.data.length,
                                                { FILENAME: '/FileUpload/file/' + item.ORIGINALNAME, ORIGINALNAME: item.ORIGINALNAME, SIZES: item.SIZES, FILETYPENAME: filetypename, FILETYPE: filetype, IETYPE: ietype, UPLOADTIME: Ext.Date.format(date, 'Y-m-d H:i:s') });
                                            })
                                        }
                                    }
                                });
                            }
                            else {
                                Ext.MessageBox.alert("提示", "文件加载失败！");
                            }
                        }

                    });

                }
            }
        }
    });
    var bbar_r = '<div class="btn-group">'
        + '<button type="button" onclick="order_cancel_submit()" class="btn btn-primary btn-sm" id="btn_cancelsubmit"><i class="fa fa-angle-double-left"></i>&nbsp;撤单</button>'
        + '<button type="button" onclick="addGlyw()" class="btn btn-primary btn-sm" id="btn_addlinkorder"><i class="fa fa-plus fa-fw"></i>&nbsp;新增关联业务</button>'
        + '<button type="button" onclick="clearWin()" class="btn btn-primary btn-sm" id="btn_createorder"><i class="fa fa-plus fa-fw"></i>&nbsp;新增</button>'
        + '<button type="button" onclick="save()" class="btn btn-primary btn-sm"><i class="fa fa-floppy-o"></i>&nbsp;保存</button>'
        + '<button type="button" onclick="submit()" class="btn btn-primary btn-sm" id="btn_submitorder"><i class="fa fa-hand-o-up"></i>&nbsp;提交委托</button>'
        + '</div>';
    var bbar_l;
    if (cur_usr.NAME == 'flddz001' || cur_usr.NAME == 'flddz002' || cur_usr.NAME == 'flddz003' || cur_usr.NAME == 'flddz004') {
          bbar_l = '<div class="btn-group">'
                    + '<button type="button" class="btn btn-primary btn-sm" id="pickfiles"><i class="fa fa-upload"></i>&nbsp;上传文件</button>'
                    + '<button type="button" onclick="browsefile()" class="btn btn-primary btn-sm"><i class="fa fa-exchange fa-fw"></i>&nbsp;浏览文件</button>'
                    + '<button type="button" onclick="removeFile()" class="btn btn-primary btn-sm" id="deletefile"><i class="fa fa-trash-o"></i>&nbsp;删除文件</button>'
                    + '<button type="button" onclick="printFile(\'gn\')" class="btn btn-primary btn-sm"><i class="fa fa-print"></i>&nbsp;打印文件</button>'
                + '</div>';
    }
    else {
          bbar_l = '<div class="btn-group">'
                    + '<button type="button" class="btn btn-primary btn-sm" id="pickfiles"><i class="fa fa-upload"></i>&nbsp;上传文件</button>'
                    + '<button type="button" onclick="browsefile()" class="btn btn-primary btn-sm"><i class="fa fa-exchange fa-fw"></i>&nbsp;浏览文件</button>'
                    + '<button type="button" onclick="removeFile()" class="btn btn-primary btn-sm" id="deletefile"><i class="fa fa-trash-o"></i>&nbsp;删除文件</button>'
                + '</div>';
    }
    toolbar1 = Ext.create('Ext.toolbar.Toolbar', {
        items: [combo_filetype1, combo_ietype1, field_fileno1, bbar_l,'->', bbar_r]
    })
    store_file1 = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'FILENAME', 'ORIGINALNAME', 'FILETYPE', 'FILETYPENAME', 'UPLOADTIME', 'SIZES', 'IETYPE'] 
    })
    var tmp1 = new Ext.XTemplate(
        '<tpl for=".">',
        '<div class="panel panel-default thumb-wrap fl" style="margin-top:5px;margin-left:5px;width:240px">',
        '<div class="panel-heading" style="padding-left:5px;padding-right:5px">{[values.ORIGINALNAME.substr(0,23)]}<div class="fr"><span class="glyphicon glyphicon-paperclip"></span></div></div>',
        '<div class="panel-body" style="padding-left:5px;">{FILETYPENAME}|',
        '<tpl>{[values.SIZES/1024 > 1024?Math.round(values.SIZES/(1024*1024))+"M":Math.round(values.SIZES/1024)+"K"]}</tpl>',        
        '|{IETYPE}|{[values.UPLOADTIME]}</div></div>',
        '</tpl>'
        )
    var fileview1 = Ext.create('Ext.view.View', {
        id: 'fileview1',
        store: store_file1,
        tpl: tmp1,
        itemSelector: 'div.thumb-wrap',
        multiSelect: true
    })
    panel_file1 = Ext.create('Ext.panel.Panel', {
        title: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;随附文件</span></h4>',
        border: 0,
        width: '62%',
        minHeight: 100,
        items: [fileview1]
    })
}