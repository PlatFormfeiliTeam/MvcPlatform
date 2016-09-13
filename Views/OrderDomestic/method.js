function initTabPanel() {
    tabpanel = Ext.create('Ext.tab.Panel', {
        items: [{ title: '原始订单', items: [topbar, formpanelhead, formpanelin, formpanelout, toolbar1, panel_file1] }
        ],
        renderTo: 'div_tab',
        listeners: {
            tabchange: function (tabPanel, newCard, oldCard, eOpts) {
                oldCard.items.remove(topbar);
                oldCard.items.remove(toolbar1);
                newCard.items.insert(0, topbar);
                newCard.items.insert(4, toolbar1);
                newCard.doLayout();
                //2016-06-21发现工具栏迁移后IE下面上传控件功能异常 为此需要根据订单状态将上传按钮重新初始化一次
                //四个订单的状态除了在草稿和文件已上传 后面所有的状态是一致的 所以只需要取原始订单的状态
                //var status = Ext.getCmp('field_status1') ? Ext.getCmp('field_status1').getValue() : Ext.getCmp('field_status2').getValue();
                var status = formpanelin.isVisible() ? Ext.getCmp('field_status1').getValue() : Ext.getCmp('field_status2').getValue();                
                button_control(status);
            }
        }
    });
}

//添加或者修改加载页面
function loadform() {
    Ext.Ajax.request({
        url: "/OrderDomestic/loadform",
        params: { ordercode: ordercode },
        success: function (response, opts) {
            var data = Ext.decode(response.responseText);
            formpanelhead.getForm().setValues(data.data1.IETYPE ? data.data1 : data.data2);
            Ext.getCmp("field_ordercodes").setValue(data.ORDERCODES);//记录所有已经存在的订单号,方便改变进出口类型时进行比对         
            repunitcode = data.data1.REPUNITCODE.length != 2 ? data.data1.REPUNITCODE : data.data2.REPUNITCODE;//初始化报关报检单位
            inspunitcode = data.data1.INSPUNITCODE.length != 2 ? data.data1.INSPUNITCODE : data.data2.INSPUNITCODE;
            
            formpanelin.getForm().setValues(data.data1);
            form_control(data.data1, 1);
            readonly_init(formpanelin, formpanelhead, 1);
           
            formpanelout.getForm().setValues(data.data2);
            form_control(data.data2, 2);
            readonly_init(formpanelout, formpanelhead, 2);//设置控件只读

            store_file1.loadData(data.filedata1);//加载附件列表数据 
            //记录原始文件ID 
            fileids1 = "";
            Ext.each(store_file1.data.items, function (rec) {
                fileids1 += rec.get("ID") + ",";
            })
            
            //记录当前的订单状态,方便进行按钮及其他控制 取原始订单某一个订单的状态即可
            var status = formpanelin.isVisible() ? Ext.getCmp('field_status1').getValue() : Ext.getCmp('field_status2').getValue();
            button_control(status);//按钮的控制

            if (data.data1.CORRESPONDNO || data.data2.CORRESPONDNO) {//如果存在多单关联号
                if (tabpanel.items.length == 1) {//如果没有初始化关联订单的界面 先进行初始化 
                    form_head2_ini();
                    form2_import_ini();
                    form2_export_ini();
                    panel_file2_ini();
                    tabpanel.add({ title: '关联订单', closable: true, items: [formpanelhead2, formpanelin2, formpanelout2, panel_file2] });
                    var task = new Ext.util.DelayedTask(function () {  //---------延时操作-------------   //doSomething,如：隐藏loading效果

                        formpanelhead2.getForm().setValues(data.data3.IETYPE ? data.data3 : data.data4);

                        formpanelin2.getForm().setValues(data.data3);
                        form_control(data.data3, 3);
                        readonly_init(formpanelin2, formpanelhead2, 3);//设置控件只读

                        formpanelout2.getForm().setValues(data.data4);
                        form_control(data.data4, 4);
                        readonly_init(formpanelout2, formpanelhead2, 4);//设置控件只读

                        store_file2.loadData(data.filedata2);//加载附件列表数据 

                        button_control(status);//按钮的控制

                        fileids2 = "";
                        Ext.each(store_file2.data.items, function (rec) {
                            fileids2 += rec.get("ID") + ",";
                        })
                    });
                    task.delay(1000); //因为经营单位选项数据加载 需延时1秒
                }
                if (tabpanel.items.length == 2) {//如果已经初始化了 
                    data.data3.IETYPE ? formpanelhead2.getForm().setValues(data.data3) : formpanelhead2.getForm().setValues(data.data4);

                    formpanelin2.getForm().setValues(data.data3);
                    form_control(data.data3, 3);
                    readonly_init(formpanelin2, formpanelhead2, 3);//设置控件只读

                    formpanelout2.getForm().setValues(data.data4);
                    form_control(data.data4, 4);
                    readonly_init(formpanelout2, formpanelhead2, 4);//设置控件只读

                    store_file2.loadData(data.filedata2);//加载附件列表数据 
                    fileids2 = "";
                    Ext.each(store_file2.data.items, function (rec) {
                        fileids2 += rec.get("ID") + ",";
                    })
                }
            }
        }
    });
}

function form_control(formdata, index) {
    if (Ext.getCmp('combo_myfs' + index)) {//贸易方式     
        if (formdata.TRADEWAYCODES1) {
            Ext.getCmp('combo_myfs' + index).setValue(formdata.TRADEWAYCODES1.split(','));
        }
    }
    if (Ext.getCmp("combo_ENTRUSTTYPENAME" + index)) {//委托类型对报关报检申报单位的控制
        bg_bj_sbdw_control(Ext.getCmp("combo_ENTRUSTTYPENAME" + index), index);
    }
}

//设置控件只读
function readonly_init(formpanel_tmp, formhead_tmp, index) {
    if (Ext.getCmp('field_status' + index)) {
        var status = Ext.getCmp('field_status' + index).getValue();

        Ext.Array.each(formpanel_tmp.getForm().getFields().items, function (item) {
            if (item.value != "" && item.value != null && item.value != undefined && item.id != 'field_ENTRUSTREQUEST' + index && item.id != "field_status" + index) {
                item.setReadOnly(status >= 15);
            }
        });
        Ext.Array.each(formhead_tmp.getForm().getFields().items, function (item) {
            if (item.value != "" && item.value != null && item.value != undefined) {
                item.setReadOnly(status >= 15);
            }
        });

        Ext.getCmp('jydw_btn' + index).setDisabled(status >= 15);
        Ext.getCmp("myfs_btn" + index).setDisabled(status >= 15);

        if (Ext.getCmp('tf_bgsbdw' + index)) {
            if (Ext.getCmp('tf_bjsbdw' + index).getValue()) {
                Ext.getCmp('bgsbdw_btn' + index).setDisabled(status >= 15);
            }            
        }
        if (Ext.getCmp('tf_bjsbdw' + index)) {
            if (Ext.getCmp('tf_bjsbdw' + index).getValue()) {
                Ext.getCmp('bjsbdw_btn' + index).setDisabled(status >= 15);
            }
        }
    }
}

function button_control(status) {
    if (status >= 15) {
        Ext.Ajax.request({//上传文件按钮除了基本的控制外，还有一种情形就是当后台开启上传权限的数量，即使提交还是可以上传的16050508667
            url: "/OrderDomestic/AdditionFile",
            params: { ORDERCODE: ordercode },
            success: function (response, option) {
                var json = Ext.decode(response.responseText);
                if (parseInt(json.result) > 0) {
                    upload_ini();
                    document.getElementById("pickfiles").disabled = false;
                }
                else {
                    document.getElementById("pickfiles").disabled = true;
                    if (uploader) {
                        uploader.destroy();
                    }
                }
            }
        })
    } 
    if (status < 15) {
        upload_ini(); //未提交时才初始化上传控件 
    }
    //上传按钮---已受理后正常情况下不允许上传文件   
    document.getElementById("deletefile").disabled = status >= 15; //删除按钮  --提交后不允许删除setVisibilityMode
    document.getElementById("btn_cancelsubmit").disabled = status != 15;//撤单按钮  只有在提交后受理前才可以撤单
    document.getElementById("btn_addlinkorder").disabled = status >= 15;//新增关联订单
    document.getElementById("btn_submitorder").disabled = status >= 15;//提交按钮     
}

function bg_bj_sbdw_control(cb, index) {
    var bgsbdw_input = Ext.getCmp('tf_bgsbdw' + index);
    var bgsbdw_container = Ext.getCmp('cont_bgsbdw' + index);
    var bjsbdw_input = Ext.getCmp('tf_bjsbdw' + index);
    var bjsbdw_container = Ext.getCmp('cont_bjsbdw' + index);
    if (cb.getValue() == '01')//仅报关 repunitcode, inspunitcode
    {
        bgsbdw_input.enable();
        bgsbdw_input.setValue(repunitcode);
        bgsbdw_container.enable();
        bjsbdw_input.setValue('');
        bjsbdw_input.disable();
        bjsbdw_container.disable();
    }
    if (cb.getValue() == '02') {
        bjsbdw_input.enable();
        bjsbdw_input.setValue(inspunitcode);
        bjsbdw_container.enable();
        bgsbdw_input.setValue('');
        bgsbdw_input.disable();
        bgsbdw_container.disable();
    }
    if (cb.getValue() == '03') {
        bgsbdw_input.enable();
        bgsbdw_input.setValue(repunitcode);
        bgsbdw_container.enable();
        bjsbdw_input.enable();
        bjsbdw_input.setValue(inspunitcode);
        bjsbdw_container.enable();
    }
}

//加载表单从Erp  by panhuaguo 2016-03-24
function LoadOrderErp() {
    if (!Ext.getCmp('NUMBER').getValue()) {
        Ext.MessageBox.alert("提示", "请输入需要导入的编号！");
        return;
    }
    if (ordercode) { //如果是编辑状态执行导入前需要判断是否和当前客户订单编号是否一致, 
        var isValid = false;
        var cusno = Ext.getCmp('NUMBER').getValue();
        if (formpanelin && Ext.getCmp("field_CUSNO1").getValue() == cusno) {
            isValid = true;
        }
        if (formpanelout && Ext.getCmp("field_CUSNO2").getValue() == cusno) {
            isValid = true;
        }
        if (formpanelin2 && Ext.getCmp("field_CUSNO3").getValue() == cusno) {
            isValid = true;
        }
        if (formpanelout2 && Ext.getCmp("field_CUSNO4").getValue() == cusno) {
            isValid = true;
        }
        if (!isValid) {
            Ext.MessageBox.alert("提示", "订单编辑时导入的客户编号必须和当前订单客户编号一致！");
            return;
        }
    }
    //如果订单中存在重复的客户编号需要进行确认
    Ext.Ajax.request({
        url: "/OrderDomestic/OperateIdRepeate",
        params: { operateid: Ext.String.trim(Ext.getCmp('NUMBER').getValue()) },
        success: function (response, option) {
            var json = Ext.decode(response.responseText);
            if (parseInt(json.result) > 0) {
                Ext.MessageBox.confirm('提示', '该订单编号已经存在，确定要继续导入吗？', function (btn) {
                    if (btn == "yes") {
                        importorder();
                    }
                })
            }
            else {
                importorder();
            }
        }
    });
}

function importorder() {
    var mask = new Ext.LoadMask(Ext.get(tabpanel.getEl()), { msg: "数据导入中，请稍等..." });
    mask.show();
    Ext.Ajax.request({
        url: "/OrderDomestic/GetOrderCodeErp",
        params: { operateid: Ext.String.trim(Ext.getCmp('NUMBER').getValue()) },
        success: function (response, option) {
            if (Ext.decode(response.responseText)) {
                var data = Ext.decode(response.responseText);
                if (data.result == "true") {
                    //相同则判断订单状态是草稿或者文件已上传时直接覆盖界面数据；如果已委托的订单，则只修改空白字段
                    var status = Ext.getCmp('field_status1') ? Ext.getCmp('field_status1').getValue() : Ext.getCmp('field_status2').getValue();
                    if (status < 15) {
                        formpanelhead.getForm().setValues(data.data1.CUSNO ? data.data1 : data.data2);
                        formpanelin.getForm().setValues(data.data1);
                        formpanelout.getForm().setValues(data.data2);
                    }
                    else {
                        update_special_field_byerp(data.data1, 1);
                        update_special_field_byerp(data.data2, 2);
                    }
                    form_control(data.data1, 1);
                    form_control(data.data2, 2);
                    if (data.data3.CUSNO || data.data4.CUSNO) {//如果存在关联订单
                        if (tabpanel.items.length == 1) {
                            form_head2_ini();
                            form2_import_ini();
                            form2_export_ini();
                            panel_file2_ini();
                            tabpanel.add({ title: '关联订单', closable: true, items: [formpanelhead2, formpanelin2, formpanelout2, panel_file2] });
                        }
                        var task = new Ext.util.DelayedTask(function () { //需要在此做一些延迟处理 有可能界面还未初始化完成直接赋值可能会失败
                            if (status < 15) {
                                formpanelhead2.getForm().setValues(data.data3);
                                formpanelin2.getForm().setValues(data.data3);
                                formpanelout2.getForm().setValues(data.data4);
                            }
                            else {
                                update_special_field_byerp(data.data3, 3);
                                update_special_field_byerp(data.data4, 4);
                            }
                            form_control(data.data3, 3);
                            form_control(data.data4, 4);
                            get_orderfile_fromerp(mask);
                        });
                        task.delay(500);
                    }
                    else {//如果不存在关联订单
                        get_orderfile_fromerp(mask);
                    }
                }
                else {
                    Ext.MessageBox.alert("提示", "对应的订单号在ERP上无记录！");
                }
            }
        }
    });
}

function get_orderfile_fromerp(mask) {
    var status = Ext.getCmp('field_status1') ? Ext.getCmp('field_status1').getValue() : Ext.getCmp('field_status2').getValue();
    //提交后的订单,再次从ERP获取信息时，如果已提交不需要重新加载文件   
    if (status < 15) {
        Ext.Ajax.request({
            url: "/OrderDomestic/getErpfile_kunshan",
            params: { 'operation_id': Ext.String.trim(Ext.getCmp('NUMBER').getValue()) },
            success: function (response, option) {
                var data = Ext.decode(response.responseText);
                store_file1.removeAll(); //获取ERP文件  
                Ext.each(data.filedata1, function (item) {
                    store_file1.insert(store_file1.data.length, { FILENAME: '/FileUpload/file/' + item.MAINNAME.substr(item.MAINNAME.lastIndexOf("CustomsFile") + 11), ORIGINALNAME: item.MAINNAME.substr(item.MAINNAME.lastIndexOf("CustomsFile") + 11), SIZES: item.FILE_SIZE, FILETYPENAME: "订单文件", FILETYPE: "44", IETYPE: '仅进口' });
                })
                Ext.each(data.filedata2, function (item) {
                    store_file1.insert(store_file1.data.length, { FILENAME: '/FileUpload/file/' + item.MAINNAME.substr(item.MAINNAME.lastIndexOf("CustomsFile") + 11), ORIGINALNAME: item.MAINNAME.substr(item.MAINNAME.lastIndexOf("CustomsFile") + 11), SIZES: item.FILE_SIZE, FILETYPENAME: "订单文件", FILETYPE: "44", IETYPE: '仅出口' });
                })
                if (store_file2) {
                    store_file2.removeAll();
                    Ext.each(data.filedata3, function (item) {
                        store_file2.insert(store_file2.data.length, { FILENAME: '/FileUpload/file/' + item.MAINNAME.substr(item.MAINNAME.lastIndexOf("CustomsFile") + 11), ORIGINALNAME: item.MAINNAME.substr(item.MAINNAME.lastIndexOf("CustomsFile") + 11), SIZES: item.FILE_SIZE, FILETYPENAME: "订单文件", FILETYPE: "44", IETYPE: '仅进口' });
                    })
                    Ext.each(data.filedata4, function (item) {
                        store_file2.insert(store_file2.data.length, { FILENAME: '/FileUpload/file/' + item.MAINNAME.substr(item.MAINNAME.lastIndexOf("CustomsFile") + 11), ORIGINALNAME: item.MAINNAME.substr(item.MAINNAME.lastIndexOf("CustomsFile") + 11), SIZES: item.FILE_SIZE, FILETYPENAME: "订单文件", FILETYPE: "44", IETYPE: '仅出口' });
                    })
                } 
                mask.hide();
                Ext.MessageBox.alert("提示", "数据导入成功！");
            }
        }); 
    }
    else {
        mask.hide();
        Ext.MessageBox.alert("提示", "数据导入成功！");
    }
}

function upload_ini() {
    uploader = new plupload.Uploader({
        runtimes: 'html5,flash,silverlight,html4',
        browse_button: 'pickfiles', // you can pass an id...
        url: '/OrderDomestic/Upload_WebServer',
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
        var cb_filetype = toolbar1.items.items[0];
        var filetype = cb_filetype.getValue();
        var filetypename = cb_filetype.getRawValue();
        var ietype = toolbar1.items.items[1].getValue();
        if (tabpanel.getActiveTab().title == "原始订单") {
            if (ietype == "进/出口业务") {
                store_file1.insert(store_file1.data.length,
               { FILENAME: '/FileUpload/file/' + file.target_name, ORIGINALNAME: file.target_name, SIZES: file.size, FILETYPENAME: filetypename, FILETYPE: filetype, IETYPE: '仅进口' });
                store_file1.insert(store_file1.data.length,
               { FILENAME: '/FileUpload/file/' + file.target_name, ORIGINALNAME: file.target_name, SIZES: file.size, FILETYPENAME: filetypename, FILETYPE: filetype, IETYPE: '仅出口' });
            }
            else {
                store_file1.insert(store_file1.data.length,
               { FILENAME: '/FileUpload/file/' + file.target_name, ORIGINALNAME: file.target_name, SIZES: file.size, FILETYPENAME: filetypename, FILETYPE: filetype, IETYPE: ietype });
            }
        }
        else {
            if (ietype == "进/出口业务") {
                store_file2.insert(store_file2.data.length,
               { FILENAME: '/FileUpload/file/' + file.target_name, ORIGINALNAME: file.target_name, SIZES: file.size, FILETYPENAME: filetypename, FILETYPE: filetype, IETYPE: '仅进口' });
                store_file2.insert(store_file2.data.length,
               { FILENAME: '/FileUpload/file/' + file.target_name, ORIGINALNAME: file.target_name, SIZES: file.size, FILETYPENAME: filetypename, FILETYPE: filetype, IETYPE: '仅出口' });
            }
            else {
                store_file2.insert(store_file2.data.length,
                { FILENAME: '/FileUpload/file/' + file.name, ORIGINALNAME: file.name, SIZES: file.size, FILETYPENAME: filetypename, FILETYPE: filetype, IETYPE: ietype });
            }
        }
    });
}

function browsefile() {
    var tab = tabpanel.getActiveTab();
    var fileview = tab.title == "原始订单" ? Ext.getCmp('fileview1') : Ext.getCmp('fileview2');
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
    if (!records[0].get("ID")) {
        win.show();
        document.getElementById('fileViewDiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="/FileUpload/file/' + records[0].get("ORIGINALNAME") + '"></embed>';
    }
    else {
        win.show();
        document.getElementById('fileViewDiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="' + AdminUrl + '\/file/' + records[0].get("FILENAME") + '"></embed>';
    }
}

function clearWin() {
    window.location.href = "/OrderDomestic/Create";
}

//新增关联业务
function addGlyw() {
    if (tabpanel.items.length == 2) {
        Ext.MessageBox.alert('提示', '最多只能添加一组关联订单！');
    }
    else {
        form_head2_ini();
        form2_import_ini();
        form2_export_ini();
        panel_file2_ini();
        tabpanel.add({ title: '关联订单', closable: true, closeAction: 'destroy', items: [formpanelhead2, formpanelin2, formpanelout2, panel_file2] });
        Ext.Ajax.request({
            url: "/OrderDomestic/loadform",
            success: function (response, option) {
                tabpanel.setActiveTab(1);
                var data = Ext.decode(response.responseText);
                formpanelhead2.getForm().setValues(data.data3);
                formpanelin2.getForm().setValues(data.data3);
                form_control(data.data3, 3);
                formpanelout2.getForm().setValues(data.data4);
                form_control(data.data4, 4);
            }
        })
    }
}

//删除文件
function removeFile() {
    var tab = tabpanel.getActiveTab();
    var fileview = tab.title == "原始订单" ? Ext.getCmp('fileview1') : Ext.getCmp('fileview2');
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

function order_cancel_submit() {//撤单,防止页面停留后后台已经受理，所以操作订单的时候需要到后台重新获取一次数据
    Ext.Ajax.request({
        url: '/OrderDomestic/GetOrder_ByCode',
        params: { ordercode: ordercode },
        success: function (response, opts) {
            var json = Ext.decode(response.responseText);
            if (json.STATUS > 15) {
                Ext.MessageBox.alert("提示", "已受理的订单不能执行撤单操作！");
                return;
            }
            if (json.STATUS == 15 && json.ORDERWAY == "1") {//线上订单已委托，执行撤单功能    
                Ext.MessageBox.confirm("提示", "确定要执行撤单操作吗？", function (btn) {
                    if (btn == "yes") {
                        Ext.Ajax.request({
                            url: '/OrderDomestic/Order_Cancel_Submit',
                            params: { ordercode: ordercode },
                            success: function (response, options) {
                                var json = Ext.decode(response.responseText);
                                if (json.success == true) {
                                    Ext.MessageBox.alert("提示", "撤单成功！");
                                    loadform();
                                }
                                else {
                                    Ext.MessageBox.alert("提示", "撤单失败！");
                                }
                            }
                        });
                    }
                });
            }
        }
    });
}

//保存
function save(action) {
    //提交后的订单如果开启的文件上传按钮，且已经上传了 
    var status = Ext.getCmp('field_status1') ? Ext.getCmp('field_status1').getValue() : Ext.getCmp('field_status2').getValue();
    if (status >= 15) {
        if ((store_file1.getModifiedRecords().length > 0) || (store_file2 && store_file2.getModifiedRecords().length > 0)) {
            Ext.getCmp("file_addition").setValue(ordercode);
        }
    }
    var data_head1 = Ext.encode(formpanelhead.getForm().getValues());
    var data1 = "{}";
    if (formpanelin && formpanelin.isVisible()) {
        data1 = Ext.encode(formpanelin.getForm().getValues());
    }
    var data2 = "{}";
    if (formpanelout && formpanelout.isVisible()) {
        data2 = Ext.encode(formpanelout.getForm().getValues());
    }
    var data_head2 = "{}";
    if (tabpanel.items.length == 2) {
        data_head2 = Ext.encode(formpanelhead2.getForm().getValues());
    }
    var data3 = "{}";
    if (tabpanel.items.length == 2 && formpanelin2) {
        data3 = Ext.encode(formpanelin2.getForm().getValues());
    }
    var data4 = "{}";
    if (tabpanel.items.length == 2 && formpanelout2) {
        data4 = Ext.encode(formpanelout2.getForm().getValues());
    }
    var file_data1 = Ext.encode(Ext.pluck(store_file1.data.items, 'data'));

    var file_data2 = "[]";
    if (tabpanel.items.length == 2 && store_file2) {
        file_data2 = Ext.encode(Ext.pluck(store_file2.data.items, 'data'));
    }
    var validate = "";
    if (formpanelin && formpanelin.isVisible() && formpanelout && formpanelout.isVisible()) {
        var wtlx_tmp1 = Ext.getCmp("combo_ENTRUSTTYPENAME1");
        var wtlx_tmp2 = Ext.getCmp("combo_ENTRUSTTYPENAME2");
        if (wtlx_tmp1.getValue() == '02' || wtlx_tmp1.getValue() == '03') {
            if (wtlx_tmp2.getValue() == '02' || wtlx_tmp2.getValue() == '03') {
                validate = "原始订单不允许同时存在报检的委托类型!";
            }
        }
    }
    if (tabpanel.items.length == 2) {
        if (formpanelin2 && formpanelout2) {
            var wtlx_tmp3 = Ext.getCmp("combo_ENTRUSTTYPENAME3");
            var wtlx_tmp4 = Ext.getCmp("combo_ENTRUSTTYPENAME4");
            if (wtlx_tmp3.getValue() == '02' || wtlx_tmp3.getValue() == '03') {
                if (wtlx_tmp4.getValue() == '02' || wtlx_tmp4.getValue() == '03') {
                    validate = "关联订单不允许同时存在报检的委托类型!";
                }
            }
        }
    }
    if (formpanelin && formpanelin.isVisible()) {
        if (Ext.getCmp("GOODSNW1").getValue() > Ext.getCmp("GOODSGW1").getValue()) {
            validate = "原始订单净重不能大于毛重!";
        }
    }
    if (tabpanel.items.length == 2) {
        if (formpanelin2) {
            if (Ext.getCmp("GOODSNW3").getValue() > Ext.getCmp("GOODSGW3").getValue()) {
                validate = "关联订单净重不能大于毛重!";
            }
        }
    }
    if (validate) {
        Ext.MessageBox.alert("提示", validate);
        return;
    }
    var mask = new Ext.LoadMask(Ext.get(tabpanel.getEl()), { msg: "数据保存中，请稍等..." });
    mask.show();
    Ext.Ajax.request({
        url: "/OrderDomestic/Save",
        params: { ordercode: ordercode, data_head1: data_head1, data1: data1, data2: data2, data_head2: data_head2, data3: data3, data4: data4, file_data1: file_data1, originalids1: fileids1, file_data2: file_data2, originalids2: fileids2, action: action },
        success: function (response, option) {
            if (response.responseText) {
                mask.hide();
                var data = Ext.decode(response.responseText);
                if (data.result) {
                    Ext.MessageBox.alert("提示", data.result);
                }
                else {
                    Ext.MessageBox.alert("提示", action == 'submit' ? "提交成功！" : "保存成功！", function () {
                        ordercode = data.ordercode;
                        setHistoryUrl("/OrderDomestic/Create?ordercode=" + data.ordercode);
                        loadform();
                    });
                }
            }
        }
    });
}

function submit() {
    if (Ext.getCmp('field_status1').getValue() >= 15) {//15表示订单已委托    
        Ext.MessageBox.alert('提示', '已委托的订单不能再次提交！');
        return;
    }
    if (store_file1.data.items.length == 0) { //如果是提交委托,必须上传文件 
        Ext.MessageBox.alert('提示', '原始订单未上传随附文件！');
        return;
    }
    if (tabpanel.items.length == 2 && store_file2 && store_file2.data.items.length == 0) {
        Ext.MessageBox.alert('提示', '关联订单未上传随附文件！');
        return;
    }
    //保存时不验证，提交委托时验证, formpanelin2
    var msg = "";
    if (!formpanelhead.getForm().isValid()) {
        msg = "原始订单表头验证未通过！";
    }
    if (formpanelin.isVisible() && !formpanelin.getForm().isValid()) {
        msg = "原始订单进口数据验证未通过！";
    }
    if (formpanelout.isVisible() && !formpanelout.getForm().isValid()) {
        msg = "原始订单出口数据验证未通过！";
    }
    if (tabpanel.items.length == 2 && !formpanelhead2.getForm().isValid()) {
        msg = "关联订单表头数据验证未通过！";
    }
    if (tabpanel.items.length == 2 && !formpanelin2.getForm().isValid()) {// && formpanelout2.isVisible()
        msg = "关联订单进口数据验证未通过！";
    }
    if (tabpanel.items.length == 2 && !formpanelout2.getForm().isValid()) {// && formpanelout2.isVisible()
        msg = "关联订单出口数据验证未通过！";
    }
    if (msg) {
        Ext.MessageBox.alert('提示', msg);
        return
    }
    save("submit");
}

function update_special_field_byerp(data, index) {
    //可能发生变化的字段 件数GOODSNUM 毛重 GOODSGW 合同号 CONTRACTNO 其他字段提交前都是必填的
    if (Ext.getCmp("GOODSNUM" + index) && !Ext.getCmp("GOODSNUM" + index).getValue()) {
        Ext.getCmp("GOODSNUM" + index).setValue(data.GOODSNUM);
    }
    if (Ext.getCmp("GOODSGW" + index) && !Ext.getCmp("GOODSGW" + index).getValue()) {
        Ext.getCmp("GOODSGW" + index).setValue(data.GOODSGW);
    }
    if (Ext.getCmp("CONTRACTNO" + index) && !Ext.getCmp("CONTRACTNO" + index).getValue()) {
        Ext.getCmp("CONTRACTNO" + index).setValue(data.CONTRACTNO);
    }
}
