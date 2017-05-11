

function bfile() {

    var fileview = Ext.getCmp('fileviewmail');
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
    document.getElementById('fileViewDiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="' + records[0].get("FILENAME") + '"></embed>';
}
function MailImport(ID) {
    mail_form_ini();

    var win = Ext.create("Ext.window.Window", {
        id: "mailWin",
        title: '邮件导入',
        width: 900,
        height: 700,
        modal: true,
        items: [Ext.getCmp('formpanel_address'),Ext.getCmp('gridpanel_mail'), Ext.getCmp('panelfilemail')],
    });
    win.on("close", function () {
        Ext.getCmp('pgbar').moveFirst();
    });
    win.show();
}

function mail_form_ini() {

    var field_sendAddress = Ext.create('Ext.form.field.Text', {
        id: 'sendAddress',
        fieldLabel: '发件人',
        regex: /^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+(.[a-zA-Z0-9_-])+/,
        regexText: '不是正确的邮箱地址！',
        name: 'sendAddress',
        emptyText: '请输入发件人邮箱地址',
        allowBlank: false,
        blankText: '邮箱地址不能为空!'
    });
    var field_recAddress = Ext.create('Ext.form.field.Text', {
        id: 'recAddress',
        fieldLabel: '收件人',
        name: 'recAddress',
        value: mailServer,
        readOnly: true,
    });
    var formpanel_address = Ext.create('Ext.form.Panel', {
        id: 'formpanel_address',
        border: 0,
        minWidth: 800,
        fieldDefaults: {
            margin: '0 5 10 0',
            labelWidth: 80,
            columnWidth: .45,
            labelSeparator: '',
            msgTarget: 'under',
            labelAlign: 'right'
        },
        items: [
            {
                layout: 'column', height: 42, border: 0, margin: '5 0 0 0', items: [field_sendAddress, field_recAddress, {
                    xtype: 'button', id: 'btn_filerecevieunit', handler: function () {
                        if (!Ext.getCmp('formpanel_address').getForm().isValid()) {
                            return;
                        } 
                        Ext.Ajax.request({
                            url: "/EnterpriseOrder/mailImport",
                            params: { sendAddress: Ext.getCmp("sendAddress").getValue() },
                            success: function (response, opts) {
                                var result = Ext.decode(response.responseText);
                                if (result.success) {
                                    Ext.MessageBox.alert("提示", "暂时没有可导入的邮件！");
                                }
                                else
                                {
                                    store_mail.removeAll();
                                    store_mail.insert(store_mail.data.length, result);
                                }
                                
                            }
                        });
                    }, text: '导入'
                }]
            },
        ]
    })




    var data_mail = [];
    var store_mail = Ext.create('Ext.data.JsonStore', {
        storeId: 'store_mail',
        fields: ['SUBJECT', 'FILECOUNT','FILEDATA'],
        data: data_mail
    });
    var w_tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: ['<span style="color:red">说明：勾选下方对应的记录，点击生成完成委托文件的导入</span>',
               '->',
               {
                   text: '<span class="icon iconfont" style="font-size:10px">&#xe622;</span>&nbsp;生成', id: 'btn_pro_save',
                   handler: function () {
                       var records = Ext.getCmp("gridpanel_mail").getSelectionModel().getSelection();
                       var data = Ext.encode(Ext.pluck(records,'data'));
                       Ext.Ajax.request({
                           url: "/EnterpriseOrder/import",
                           params: { records: data},
                           success: function (response, option) {
                               var data = Ext.decode(response.responseText);
                               if (data.success) {
                                   Ext.MessageBox.alert("提示", "生成成功！", function () {
                                   });
                               }
                               else {
                                
                                       Ext.MessageBox.alert("提示", "生成失败！");
                               }
                           }

                       });
                   }
               }
               //,{
               //    text: '<span class="icon iconfont" style="font-size:10px">&#xe6d3;</span>&nbsp;删 除', id: 'btn_pro_del',
               //    handler: function () {
               //        var recs = gridpanel_mail.getSelectionModel().getSelection();
               //        if (recs.length > 0) {
               //            store_mail.remove(recs);
               //        }
               //    }
               //}
        ]

    });

    var gridpanel_mail = Ext.create('Ext.grid.Panel', {
        id: 'gridpanel_mail',
        store: store_mail,
        minHeight: 300,
        selModel: { selType: 'checkboxmodel' },
        enableColumnHide: false,
        tbar: w_tbar,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: 'ID', dataIndex: 'ID', hidden: true },
        { header: '邮件主题', dataIndex: 'SUBJECT', width: 80 },
        { header: '文件数量', dataIndex: 'FILECOUNT', width: 80 }

        
        ],
        listeners: {
            select: function (ts, record, index, eOpts) {
                Ext.getCmp('fileviewmail').store.removeAll();
                Ext.getCmp('fileviewmail').store.insert(Ext.getCmp('fileviewmail').store.data.length,record.data.FILEDATA);
            }
        },
        viewConfig: {
            enableTextSelection: true
        },
        forceFit: true
    });

    var bbar_l = '<div class="btn-group">'
           + '<button type="button" onclick="bfile()" class="btn btn-primary btn-sm"><i class="fa fa-exchange fa-fw"></i>&nbsp;浏览文件</button>'
           //+ '<button type="button" onclick="removeFile()" class="btn btn-primary btn-sm" id="deletefile"><i class="fa fa-trash-o"></i>&nbsp;删除文件</button>'
       + '</div>';

    var toolbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [bbar_l]
    })

    var storefilemail = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'FILENAME', 'NEWNAME', 'ORIGINALNAME', 'UPLOADTIME', 'SIZES', 'FILETYPE']
    });

    var tmp = new Ext.XTemplate(
    '<tpl for=".">',
    '<div class="panel panel-default thumb-wrap fl" style="margin-top:5px;margin-left:5px;width:240px">',
    //'<div class="panel-heading" style="padding-left:5px;padding-right:5px">{[values.ORIGINALNAME.substr(0,23)]}<div class="fr"><span class="glyphicon glyphicon-paperclip"></span></div></div>',
    '<div class="panel-heading" style="padding-left:5px;padding-right:5px">{[values.ORIGINALNAME.substr(0,23)]}<div class="fr"><a href="/EnterpriseOrder/DownFile?filename={[values.FILENAME]}&ID={[values.ID]}" style="cursor:pointer"><span class="glyphicon glyphicon-download-alt"></span></a></div></div>',
    '<div class="panel-body" style="padding-left:5px;">',
    '<tpl>{[values.SIZES/1024 > 1024?Math.round(values.SIZES/(1024*1024))+"M":Math.round(values.SIZES/1024)+"K"]}</tpl>',
    '|{[values.UPLOADTIME]}</div></div>',
    '</tpl>'
    )
    var fileviewmail = Ext.create('Ext.view.View', {
        id: 'fileviewmail',
        store: storefilemail,
        tpl: tmp,
        itemSelector: 'div.thumb-wrap',
        multiSelect: true
    })
    var panelfile = Ext.create('Ext.panel.Panel', {
        id: 'panelfilemail',
        tbar: toolbar,
        border: 0,
        width: '100%',
        minHeight: 500,
        items: [fileviewmail]
    })

}