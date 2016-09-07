function panel_file2_ini() {
    store_file2 = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'FILENAME', 'ORIGINALNAME', 'FILETYPE', 'FILETYPENAME', 'UPLOADTIME', 'SIZES', 'IETYPE'],
        listeners: {
            datachanged: function (sf_2, eOpts) {
                if (sf_2.find('IETYPE', '仅进口') >= 0 && Ext.getCmp('field_status3') && Ext.getCmp('field_status3').getValue() == 1) {
                    Ext.getCmp('field_status3').setValue(10);
                }
                if (sf_2.find('IETYPE', '仅进口') < 0 && Ext.getCmp('field_status3') && Ext.getCmp('field_status3').getValue() == 10) {
                    Ext.getCmp('field_status3').setValue(1);
                }
                if (sf_2.find('IETYPE', '仅出口') >= 0 && Ext.getCmp('field_status4') && Ext.getCmp('field_status4').getValue() == 1) {
                    Ext.getCmp('field_status4').setValue(10);
                }
                if (sf_2.find('IETYPE', '仅出口') < 0 && Ext.getCmp('field_status4') && Ext.getCmp('field_status4').getValue() == 10) {
                    Ext.getCmp('field_status4').setValue(1);
                }
            }
        }
    })
    var tmp2 = new Ext.XTemplate(
        '<tpl for=".">',
        '<div class="panel panel-default thumb-wrap fl" style="margin-top:5px;margin-left:5px;width:240px">',
        '<div class="panel-heading" style="padding-left:5px;padding-right:5px">{[values.ORIGINALNAME.substr(0,23)]}<div class="fr"><span class="glyphicon glyphicon-paperclip"></span></div></div>',
        '<div class="panel-body" style="padding-left:5px;">{FILETYPENAME}|',
        '<tpl>{[values.SIZES/1024 > 1024?Math.round(values.SIZES/(1024*1024))+"M":Math.round(values.SIZES/1024)+"K"]}</tpl>',
        '|{IETYPE}|{[values.UPLOADTIME.substr(0,values.UPLOADTIME.indexOf("T"))]}</div></div>',
        '</tpl>'
        )
    var fileview2 = Ext.create('Ext.view.View', {
        id: 'fileview2',
        store: store_file2,
        tpl: tmp2,
        itemSelector: 'div.thumb-wrap',
        multiSelect: true
    })
    panel_file2 = Ext.create('Ext.panel.Panel', {
        title: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;随附文件</span></h4>',
        border: 0,
        width: '62%',
        minHeight: 100,
        items: [fileview2]
    })
}