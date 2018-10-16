function Import() {

    var modulelable = Ext.create('Ext.form.Label', {
        id: 'modulelable',
        name: 'modulelable',
        html: '<a href="/FileUpload/驻厂服务导入模板.xlsx" style="cursor:pointer">导入模板下载</a>',
       // cls: "lab", flex: .1
    });

    var Import_Panel = Ext.create('Ext.form.Panel', {
       // title: '导入',
        //  width: 400,
        id: 'Import_Panel',
        name: 'Import_Panel',
        bodyPadding: 20,
        items: [{
            xtype: 'filefield',
            name: 'import',
            fieldLabel: '导入文件',
            labelWidth: 60,
            msgTarget: 'side',
            allowBlank: false,
            anchor: '100%',
            buttonText: '选择文件',
            validator: function (value) {
                // 文件类型判断
                var arrType = value.split('.');
                var docType = arrType[arrType.length - 1].toLowerCase();
                if (docType != 'xlsx') {
                    return '文件类型必须为xlsx';
                } else {
                    return true;
                }
            }
        }, modulelable],
        buttons: [{
            text: '导入',
            handler: function () {
                var form = Ext.getCmp('Import_Panel').getForm();
                if (form.isValid()) {
                    form.submit({
                        url: "/StationedFileld/Import_Excel",
                        waitMsg: '导入中，请稍等。。',
                        success: function (fm, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert('提示', result.msg);
                        },
                        failure: function (fm, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert("错误提示", result.msg);
                        }
                    });
                }
            }
        }]
    });

    var Import_window = Ext.create("Ext.window.Window", {
        id: 'Import_window',
        title: "导入",
        width: 400,
        height: 130,
        layout: "fit",
        //collapsible: true,
        items: [Import_Panel],//destroy( Ext.Component this, Object eOpts )
        listeners: {
            destroy: function (th, eOpts) {
                pgbar.moveFirst();//moveFirst(); doRefresh
            }
        }
    });
    Import_window.show();
}
