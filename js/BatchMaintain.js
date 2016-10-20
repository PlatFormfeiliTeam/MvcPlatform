function Batchform_ini() {

    var label_baseinfo = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;数据信息</span></h4>'
    }
    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo]
    })

    //申报关区
    var store_CUSTOMDISTRICTNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbgq
    })
    var combo_CUSTOMDISTRICTNAME = Ext.create('Ext.form.field.ComboBox', {//申报关区 这个数据比较多需要根据输入字符到后台动态模糊匹配
        id: 'combo_CUSTOMDISTRICTNAME',
        name: 'CUSTOMAREACODE',
        store: store_CUSTOMDISTRICTNAME,
        fieldLabel: '申报关区',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        minChars: 1,
        hideTrigger: true,
        forceSelection: true,
        anyMatch: true,
        allowBlank: false,
        blankText: '申报关区不能为空!',
        listeners: {
            focus: function (cb) {
                cb.clearInvalid();
            }
        },
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    });

    //总单号
    var field_TOTALNO = Ext.create('Ext.form.field.Text', {
        id: 'field_TOTALNO',
        name: 'TOTALNO',
        enforceMaxLength: true,
        maxLength: 11,
        fieldLabel: '总单号'
    });
    //转关预录号
    var field_TURNPRENO = Ext.create('Ext.form.field.Text', {
        fieldLabel: '转关预录号',
        id: 'field_TURNPRENO',
        name: 'TURNPRENO',
        enforceMaxLength: true,
        maxLength: 16
    });
    //木质包装
    var store_mzbz = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_mzbz
    })
    var combo_mzbz = Ext.create('Ext.form.field.ComboBox', {
        name: 'WOODPACKINGID',
        id: 'combo_mzbz',
        store: store_mzbz,
        fieldLabel: '木质包装',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        hideTrigger: true,
        forceSelection: true,
        anyMatch: true,
        listeners: {
            focus: function (cb) {
                if (!cb.getValue()) {
                    cb.clearInvalid();
                    cb.expand();
                    cb.store.clearFilter();
                }
            }
        },
        allowBlank: false,
        blankText: '木质包装不能为空!',
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })
    //需求备注
    var field_ORDERREQUEST = Ext.create('Ext.form.field.Text', {
        fieldLabel: '备注',
        name: 'ORDERREQUEST',
        id: 'field_ORDERREQUEST'
    });
    //报关车号
    var field_CONTAINERTRUCK = Ext.create('Ext.form.field.Hidden', {
        id: 'field_CONTAINERTRUCK',
        name: 'CONTAINERTRUCK'
    })
    var field_bgch = Ext.create('Ext.form.field.Text', {
        name: 'DECLCARNO',
        id: 'DECLCARNO',
        tabIndex: 25,
        readOnly: true,
        allowBlank: true,
        margin: 0,
        flex: 0.85
    });
    var container_bgch = Ext.create('Ext.form.FieldContainer', {
        id: 'fieldbgch',
        fieldLabel: '报关车号',
        layout: 'hbox',
        items: [field_bgch, {
            id: 'bgch_button',
            xtype: 'button', id: 'declcarno_btn', handler: function () {
                win_container_truck.show();
            }, text: '<span class="glyphicon glyphicon-search"></span>', flex: .15, margin: 0
        }]
    })

    //进出口岸
    var store_PORTNAME = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: common_data_sbgq
    })
    var combo_PORTCODE = Ext.create('Ext.form.field.ComboBox', {
        name: 'PORTCODE',
        id: 'combo_PORTCODE',
        store: store_PORTNAME,
        fieldLabel: '进出口岸',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        minChars: 1,
        hideTrigger: true,
        anyMatch: true,
        forceSelection: true,
        listeners: {
            focus: function (cb) {
                cb.clearInvalid();
            }
        },
        listConfig: {
            maxHeight: 110,
            getInnerTpl: function () {
                return '<div>{NAME}</div>';
            }
        }
    })

    //船名
    var field_SHIPNAME = Ext.create('Ext.form.field.Text', {
        fieldLabel: '船名',
        name: 'SHIPNAME',
        id: 'field_SHIPNAME'
    });
    //航次
    var field_FILGHTNO = Ext.create('Ext.form.field.Text', {
        fieldLabel: '航次',
        name: 'FILGHTNO',
        id: 'field_FILGHTNO'
    });
    //提单号
    var field_LADINGBILLNO = Ext.create('Ext.form.field.Text', {
        name: 'SECONDLADINGBILLNO',
        fieldLabel: '提单号',
        id: 'field_LADINGBILLNO'
    });
    //运抵编号
    var field_ARRIVEDNO = Ext.create('Ext.form.field.Text', {
        name: 'ARRIVEDNO',
        id: 'field_ARRIVEDNO',
        maxLength: 18,
        minLength: 18,
        emptyText: '请填写18位运抵编号',
        fieldLabel: '运抵编号'
    });
    //载货清单号
    var field_MANIFEST = Ext.create('Ext.form.field.Text', {
        fieldLabel: '载货清单号',
        name: 'MANIFEST',
        id: 'field_MANIFEST'
    });
    //转关预录号 对方转关号
    var field_TURNPRENO_TWO = Ext.create('Ext.form.field.Text', {
        fieldLabel: '对方转关号',
        name: 'TURNPRENO',
        id: 'field_TURNPRENO_TWO',
        enforceMaxLength: true,
        maxLength: 16
    });
    //法检状况
    var chk_CHKLAWCONDITION = Ext.create('Ext.form.field.Checkbox', {
        tabIndex: 16,
        fieldLabel: '法检状况',
        name: 'LAWFLAG',
        id: 'chk_CHKLAWCONDITION'
    })

    var itemsArray = new Array();

    //空运进口
    if (type == "KYJK") {
        itemsArray = [
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_CUSTOMDISTRICTNAME] },
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_TOTALNO, field_TURNPRENO, combo_mzbz] },
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [container_bgch, field_ORDERREQUEST] }
                , field_CONTAINERTRUCK
        ]
    }
    //空运出口
    if (type == "KYCK") {
        itemsArray = [
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_CUSTOMDISTRICTNAME] },
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_TOTALNO, combo_PORTCODE, container_bgch] },
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_ORDERREQUEST] }
                , field_CONTAINERTRUCK
        ]
    }
    //海运进口
    if (type == "HYJK") {
        itemsArray = [
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_CUSTOMDISTRICTNAME] },
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_TURNPRENO, combo_mzbz, combo_PORTCODE] },
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [container_bgch, field_ORDERREQUEST] }
                , field_CONTAINERTRUCK
        ]
    }
    //海运出口
    if (type == "HYCK") {
        itemsArray = [
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_CUSTOMDISTRICTNAME] },
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_PORTCODE, field_SHIPNAME, field_FILGHTNO] },
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_LADINGBILLNO, field_ARRIVEDNO, container_bgch] },
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_ORDERREQUEST] }
                , field_CONTAINERTRUCK
        ]
    }
    //陆运进口
    if (type == "LYJK") {
        itemsArray = [
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_CUSTOMDISTRICTNAME] },
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_mzbz, field_LADINGBILLNO, field_MANIFEST] },
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [container_bgch, field_ORDERREQUEST] }
                , field_CONTAINERTRUCK
        ]
    }
    //陆运出口
    if (type == "LYCK") {
        itemsArray = [
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_CUSTOMDISTRICTNAME] },
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_PORTCODE, field_ARRIVEDNO, container_bgch] },
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_ORDERREQUEST] }
                , field_CONTAINERTRUCK
        ]
    }
    //特殊区域
    if (type == "TSQY") {
        itemsArray = [
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_CUSTOMDISTRICTNAME] },
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [field_TURNPRENO_TWO, container_bgch, field_ORDERREQUEST] }
                , field_CONTAINERTRUCK
        ]
    }
    //国内结转
    if (type == "GNJZ") {
        itemsArray = [
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [combo_CUSTOMDISTRICTNAME] },
                { layout: 'column', height: 42, margin: '5 0 0 0', border: 0, items: [chk_CHKLAWCONDITION, field_ORDERREQUEST] }
        ]
    }

    //数据信息
    formpanel = Ext.create('Ext.form.Panel', {
        renderTo: 'div_form_left',
        minHeight: 350,
        border: 0,
        tbar: tbar,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: .33,
            labelAlign: 'right',
            labelSeparator: '',
            msgTarget: 'under',
            validateOnBlur: false,
            validateOnChange: false
        },
        items: itemsArray,
        buttons: [
            {
                text: '保存', handler: function () {
                    /*var recs = gridpanel.getSelectionModel().getSelection();
                    if (recs.length == 0) {
                        Ext.MessageBox.alert('提示', '请选择需要维护的记录！');
                        return;
                    }
                    plwhids = "";
                    for (var i = 0; i < recs.length; i++) {
                        plwhids += recs[i].data.ID + ',';
                    }
                    plwhids = plwhids.substr(0, plwhids.length - 1);
                    var formdata = formpanel.getForm().getValues();
                    var json = Ext.encode(Ext.pluck(store.data.items, 'data'));
                    Ext.Ajax.request({
                        url: "/Common/BatchOrderUpdate",
                        params: {
                            formdata: Ext.encode(formdata),
                            ids: plwhids,
                            jsonStringWJ: json
                        },
                        success: function (option, success, response) {
                            var data = Ext.decode(option.responseText);
                            if (data.success == true) {
                                store_Trade.load();
                                //Ext.getCmp('w_grid').store.loadData(Ext.decode(Ext.getCmp('field_CONTAINERTRUCK').getValue()));
                                Ext.MessageBox.alert("提示", "保存成功！");
                            } else {
                                Ext.MessageBox.alert("提示", "未保存信息！");
                            }
                        }
                    })*/

                }
            }, {
                text: '关闭', handler: function () {
                    window.close();
                }
            }
        ]
    });
    ini_container_truck();//初始化集装箱和报关车号选择界面
}

function Batchpanel_file_ini() {
    //批量维护文件
    var label_baseinfo_file = {
        xtype: 'label',
        margin: '5',
        html: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;随附文件</span></h4>'
    }
    var bbar_r_file = '<div><div class="btn-group" role="group">'
                    + '<button type="button" style="float:left; border: 1px solid transparent;border-radius:0px 0px 0px 0px;" id="pickfiles" class="btn-primary btn-sm"><i class="fa fa-upload"></i>&nbsp;上传文件</button>'
                    + '<button type="button" onclick="browsefile()" class="btn btn-primary btn-sm"><i class="fa fa-exchange fa-fw"></i>&nbsp;浏览文件</button>'
                    + '<button type="button" onclick="removeFile()" class="btn btn-primary btn-sm"><i class="fa fa-trash-o"></i>&nbsp;删除文件</button></div></div>'

    var store_filetype = Ext.create('Ext.data.JsonStore', {
        fields: ['FILETYPEID', 'FILETYPENAME'],
        data: [{ FILETYPEID: '57', FILETYPENAME: '转关单文件' },
               { FILETYPEID: '58', FILETYPENAME: '配舱单文件' }]
    });

    var combo_filetype = Ext.create('Ext.form.field.ComboBox', {//文件类型
        id: 'combo_filetype',
        name: 'FILETYPEID',
        store: store_filetype,
        fieldLabel: '文件类型',
        displayField: 'FILETYPENAME',
        valueField: 'FILETYPEID',
        editable: false,
        queryMode: 'local',
        labelWidth: 60,
        width: 150,
        value: '58'
    });

    var tbar_file = Ext.create('Ext.toolbar.Toolbar', {
        items: [label_baseinfo_file, '->', combo_filetype, bbar_r_file]
    });

    file_store = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'FILENAME', 'ORIGINALNAME', 'FILETYPE', 'FILETYPENAME', 'UPLOADTIME', 'SIZES', 'IETYPE']
    })
    var tmp = new Ext.XTemplate(
         '<tpl for=".">',
        '<div class="panel panel-default thumb-wrap fl" style="margin-top:5px;margin-left:5px;width:240px">',
        '<div class="panel-heading" style="padding-left:5px;padding-right:5px">{[values.ORIGINALNAME.substr(0,23)]}<div class="fr"><span class="glyphicon glyphicon-paperclip"></span></div></div>',
        '<div class="panel-body" style="padding-left:5px;">{FILETYPENAME}|',
        '<tpl>{[values.SIZES/1024 > 1024?Math.round(values.SIZES/(1024*1024))+"M":Math.round(values.SIZES/1024)+"K"]}</tpl>',
        '|{[values.UPLOADTIME]}</div></div>',
        '</tpl>'
        )
    var fileview = Ext.create('Ext.view.View', {
        id: 'w_fileview',
        store: file_store,
        tpl: tmp,
        itemSelector: 'div.thumb-wrap',
        multiSelect: true
    })
    var panel = Ext.create('Ext.panel.Panel', {
        renderTo: "div_form_right",
        tbar: tbar_file,
        border: 0,
        minHeight: 350,
        items: [fileview]
    })

}

//设置数据信息区域可用
function loadbatchform() {
    var recs = gridpanel.getSelectionModel().getSelection();    
    
    var bf = false; var a = 0, b = 0, c = 0, d = 0, e = 0, f = 0, g = 0, h = 0, i = 0;

    //空运进口
    if (type == 'KYJK') {
        //申报关区//报关车号//备注//总单号//转关预录号//木质包装
        Ext.getCmp('combo_CUSTOMDISTRICTNAME').setDisabled(false); Ext.getCmp('declcarno_btn').setDisabled(false); Ext.getCmp('field_ORDERREQUEST').setDisabled(false);
        Ext.getCmp('field_TOTALNO').setDisabled(false); Ext.getCmp('field_TURNPRENO').setDisabled(false); Ext.getCmp('combo_mzbz').setDisabled(false);

        for (var i = 0; i < recs.length; i++) {

            if (recs[i].data.ENTRUSTTYPE == "01") { if (recs[i].data.DECLSTATUS != '0') { bf = true; } }
            if (recs[i].data.ENTRUSTTYPE == "02") { if (recs[i].data.INSPSTATUS != '0') { bf = true; } }
            if (recs[i].data.ENTRUSTTYPE == "03") { if (recs[i].data.DECLSTATUS != '0' || recs[i].data.INSPSTATUS != '0') { bf = true; } }

            if (bf) {
                if (isNull(recs[i].data.CUSTOMAREACODE)) { a = a + 1; }//申报关区
                if (isNull(recs[i].data.DECLCARNO)) { b = b + 1; }//报关车号
                if (isNull(recs[i].data.ORDERREQUEST)) { c = c + 1; }//备注
                if (isNull(recs[i].data.TOTALNO)) { d = d + 1; }//总单号
                if (isNull(recs[i].data.TURNPRENO)) { e = e + 1; }//转关预录号
                if (isNull(recs[i].data.WOODPACKINGID)) { f = f + 1; }//木质包装
            }
            if (a > 0) { Ext.getCmp('combo_CUSTOMDISTRICTNAME').setDisabled(true); }//申报关区
            if (b > 0) { Ext.getCmp('declcarno_btn').setDisabled(true); }//报关车号
            if (c > 0) { Ext.getCmp('field_ORDERREQUEST').setDisabled(true); }//备注
            if (d > 0) { Ext.getCmp('field_TOTALNO').setDisabled(true); }//总单号
            if (e > 0) { Ext.getCmp('field_TURNPRENO').setDisabled(true); }//转关预录号
            if (f > 0) { Ext.getCmp('combo_mzbz').setDisabled(true); }//木质包装
        }
    }

    ////空运出口
    //if (type == 'KYCK') {
    //    //申报关区
    //    combo_CUSTOMDISTRICTNAME.setDisabled(false);
    //    //报关车号
    //    combo_bgch.setDisabled(false);
    //    //备注
    //    field_ORDERREQUEST.setDisabled(false);
    //    //总单号
    //    field_TOTALNO.setDisabled(false);
    //    //进出口岸
    //    combo_PORTCODE.setDisabled(false);
    //    for (var i = 0; i < recs.length; i++) {
    //        if (recs[i].data.STATUS != '1' && recs[i].data.STATUS != '10') {
    //            //申报关区
    //            if (isNull(recs[i].data.CUSTOMDISTRICTNAME)) {
    //                combo_CUSTOMDISTRICTNAME.setDisabled(true);
    //            }
    //            //报关车号
    //            if (isNull(recs[i].data.DECLCARNO)) {
    //                combo_bgch.setDisabled(true);
    //            }
    //            //备注
    //            if (isNull(recs[i].data.ORDERREQUEST)) {
    //                field_ORDERREQUEST.setDisabled(true);
    //            }
    //            //总单号
    //            if (isNull(recs[i].data.TOTALNO)) {
    //                field_TOTALNO.setDisabled(true);
    //            }
    //            //进出口岸
    //            if (isNull(recs[i].data.PORTCODE)) {
    //                combo_PORTCODE.setDisabled(true);
    //            }
    //        }
    //    }
    //}

    ////海运进口
    //if (type == 'HYJK') {
    //    //申报关区
    //    combo_CUSTOMDISTRICTNAME.setDisabled(false);
    //    //报关车号
    //    combo_bgch.setDisabled(false);
    //    //备注
    //    field_ORDERREQUEST.setDisabled(false);
    //    //转关预录号
    //    field_TURNPRENO.setDisabled(false);
    //    //木质包装
    //    combo_mzbz.setDisabled(false);
    //    //进出口岸
    //    combo_PORTCODE.setDisabled(false);
    //    for (var i = 0; i < recs.length; i++) {
    //        if (recs[i].data.STATUS != '1' && recs[i].data.STATUS != '10') {
    //            //申报关区
    //            if (isNull(recs[i].data.CUSTOMDISTRICTNAME)) {
    //                combo_CUSTOMDISTRICTNAME.setDisabled(true);
    //            }
    //            //报关车号
    //            if (isNull(recs[i].data.DECLCARNO)) {
    //                combo_bgch.setDisabled(true);
    //            }
    //            //备注
    //            if (isNull(recs[i].data.ORDERREQUEST)) {
    //                field_ORDERREQUEST.setDisabled(true);
    //            }
    //            //转关预录号
    //            if (isNull(recs[i].data.TURNPRENO)) {
    //                field_TURNPRENO.setDisabled(true);
    //            }
    //            //木质包装
    //            if (isNull(recs[i].data.WOODPACKINGNAME)) {
    //                combo_mzbz.setDisabled(true);
    //            }
    //            //进出口岸
    //            if (isNull(recs[i].data.PORTCODE)) {
    //                combo_PORTCODE.setDisabled(true);
    //            }
    //        }
    //    }
    //}

    ////海运出口
    //if (type == 'HYCK') {
    //    //申报关区
    //    combo_CUSTOMDISTRICTNAME.setDisabled(false);
    //    //报关车号
    //    combo_bgch.setDisabled(false);
    //    //备注
    //    field_ORDERREQUEST.setDisabled(false);
    //    //进出口岸
    //    combo_PORTCODE.setDisabled(false);
    //    field_SHIPNAME.setDisabled(false);
    //    field_FILGHTNO.setDisabled(false);
    //    field_LADINGBILLNO.setDisabled(false);
    //    field_ARRIVEDNO.setDisabled(false);

    //    for (var i = 0; i < recs.length; i++) {
    //        if (recs[i].data.STATUS != '1' && recs[i].data.STATUS != '10') {
    //            //申报关区
    //            if (isNull(recs[i].data.CUSTOMDISTRICTNAME)) {
    //                combo_CUSTOMDISTRICTNAME.setDisabled(true);
    //            }
    //            //报关车号
    //            if (isNull(recs[i].data.DECLCARNO)) {
    //                combo_bgch.setDisabled(true);
    //            }
    //            //备注
    //            if (isNull(recs[i].data.ORDERREQUEST)) {
    //                field_ORDERREQUEST.setDisabled(true);
    //            }
    //            //进出口岸
    //            if (isNull(recs[i].data.PORTCODE)) {
    //                combo_PORTCODE.setDisabled(true);
    //            }
    //            if (isNull(recs[i].data.SHIPNAME)) {
    //                field_SHIPNAME.setDisabled(true);
    //            }
    //            if (isNull(recs[i].data.FILGHTNO)) {
    //                field_FILGHTNO.setDisabled(true);
    //            }
    //            if (isNull(recs[i].data.SECONDLADINGBILLNO)) {
    //                field_LADINGBILLNO.setDisabled(true);
    //            }
    //            if (isNull(recs[i].data.ARRIVEDNO)) {
    //                field_ARRIVEDNO.setDisabled(true);
    //            }
    //        }
    //    }
    //}

    ////陆运进口
    //if (type == 'LYJK') {
    //    //申报关区
    //    combo_CUSTOMDISTRICTNAME.setDisabled(false);
    //    //报关车号
    //    combo_bgch.setDisabled(false);
    //    //备注
    //    field_ORDERREQUEST.setDisabled(false);
    //    combo_mzbz.setDisabled(false);
    //    field_LADINGBILLNO.setDisabled(false);
    //    field_MANIFEST.setDisabled(false);

    //    for (var i = 0; i < recs.length; i++) {
    //        if (recs[i].data.STATUS != '1' && recs[i].data.STATUS != '10') {
    //            //申报关区
    //            if (isNull(recs[i].data.CUSTOMDISTRICTNAME)) {
    //                combo_CUSTOMDISTRICTNAME.setDisabled(true);
    //            }
    //            //报关车号
    //            if (isNull(recs[i].data.DECLCARNO)) {
    //                combo_bgch.setDisabled(true);
    //            }
    //            //备注
    //            if (isNull(recs[i].data.ORDERREQUEST)) {
    //                field_ORDERREQUEST.setDisabled(true);
    //            }
    //            if (isNull(recs[i].data.WOODPACKINGID)) {
    //                combo_mzbz.setDisabled(true);
    //            }
    //            if (isNull(recs[i].data.SECONDLADINGBILLNO)) {
    //                field_LADINGBILLNO.setDisabled(true);
    //            }
    //            if (isNull(recs[i].data.MANIFEST)) {
    //                field_MANIFEST.setDisabled(true);
    //            }
    //        }
    //    }
    //}

    ////陆运出口
    //if (type == 'LYCK') {
    //    //申报关区
    //    combo_CUSTOMDISTRICTNAME.setDisabled(false);
    //    //报关车号
    //    combo_bgch.setDisabled(false);
    //    //备注
    //    field_ORDERREQUEST.setDisabled(false);
    //    combo_PORTCODE.setDisabled(false);
    //    field_ARRIVEDNO.setDisabled(false);

    //    for (var i = 0; i < recs.length; i++) {
    //        if (recs[i].data.STATUS != '1' && recs[i].data.STATUS != '10') {
    //            //申报关区
    //            if (isNull(recs[i].data.CUSTOMDISTRICTNAME)) {
    //                combo_CUSTOMDISTRICTNAME.setDisabled(true);
    //            }
    //            //报关车号
    //            if (isNull(recs[i].data.DECLCARNO)) {
    //                combo_bgch.setDisabled(true);
    //            }
    //            //备注
    //            if (isNull(recs[i].data.ORDERREQUEST)) {
    //                field_ORDERREQUEST.setDisabled(true);
    //            }
    //            if (isNull(recs[i].data.PORTCODE)) {
    //                combo_PORTCODE.setDisabled(true);
    //            }
    //            if (isNull(recs[i].data.ARRIVEDNO)) {
    //                field_ARRIVEDNO.setDisabled(true);
    //            }
    //        }
    //    }
    //}

    ////特殊区域
    //if (type == 'TSQY') {
    //    //申报关区
    //    combo_CUSTOMDISTRICTNAME.setDisabled(false);
    //    //报关车号
    //    combo_bgch.setDisabled(false);
    //    //备注
    //    field_ORDERREQUEST.setDisabled(false);
    //    field_TURNPRENO_TWO.setDisabled(false);

    //    for (var i = 0; i < recs.length; i++) {
    //        if (recs[i].data.STATUS != '1' && recs[i].data.STATUS != '10') {
    //            //申报关区
    //            if (isNull(recs[i].data.CUSTOMDISTRICTNAME)) {
    //                combo_CUSTOMDISTRICTNAME.setDisabled(true);
    //            }
    //            //报关车号
    //            if (isNull(recs[i].data.DECLCARNO)) {
    //                combo_bgch.setDisabled(true);
    //            }
    //            //备注
    //            if (isNull(recs[i].data.ORDERREQUEST)) {
    //                field_ORDERREQUEST.setDisabled(true);
    //            }
    //            if (isNull(recs[i].data.TURNPRENO)) {
    //                field_TURNPRENO_TWO.setDisabled(true);
    //            }
    //        }
    //    }
    //}

    ////国内结转
    //if (type == 'GNJZ') {
    //    //申报关区
    //    combo_CUSTOMDISTRICTNAME.setDisabled(false);
    //    //备注
    //    field_ORDERREQUEST.setDisabled(false);
    //    for (var i = 0; i < recs.length; i++) {
    //        if (recs[i].data.STATUS != '1' && recs[i].data.STATUS != '10') {
    //            //申报关区
    //            if (isNull(recs[i].data.CUSTOMDISTRICTNAME)) {
    //                combo_CUSTOMDISTRICTNAME.setDisabled(true);
    //            }
    //            //备注
    //            if (isNull(recs[i].data.ORDERREQUEST)) {
    //                field_ORDERREQUEST.setDisabled(true);
    //            }
    //        }
    //    }
    //}

}

function formcontrol() {
    var status = Ext.getCmp('field_STATUS').getValue();
    document.getElementById("pickfiles").disabled = status >= 10;
    document.getElementById("deletefile").disabled = status >= 10; //删除按钮  --提交后不允许删除setVisibilityMode

    if (status < 10) {
        upload_ini();
    }
}
