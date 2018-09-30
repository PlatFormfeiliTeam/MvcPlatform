function Export() {

    var Export_list = Ext.create('Ext.button.Button', {
        text: '列表导出',
        id: 'Export_list',
        flex: 1,
        margin: '10 15 10 20',
        height: 30,
        margin: '10',
        handler: function () {
            ExportExcel('0');
        },
    })

    var Export_QD = Ext.create('Ext.button.Button', {
        text: '代理清单导出',
        id: 'Export_QD',
        flex: 1,
        margin: '10 20 10 15',
        height: 30,
        margin: '10',
        handler: function () {
            ExportExcel('1');
        },
    })

    //var Export_list = Ext.create('Ext.form.Label', {
    //    //text: '列表导出',
    //    html: '<a href="/StationedFileld/ExportExcel?type=0" style="font-size:16px;">列表导出</a>',
    //    id: 'Export_list',
    //    flex: 1,
    //    margin: '10 0 0 40',
    //    //height: 30,
    //    //margin: '10',
    //    //handler: function () {
    //    //    ExportExcel('0');
    //    //},
    //})

    //var Export_QD = Ext.create('Ext.form.Label', {
    //    //text: '代理清单导出',
    //    html: '<a href="~/Home/download" style="font-size:16px;">代理清单导出</a>',
    //    id: 'Export_QD',
    //    flex: 1,
    //    margin: '10 0 0 0',
    //    //height: 30,
    //    //margin: '10',
    //    //handler: function () {
    //    //    ExportExcel('1');
    //    //},
    //})

    //var Export_list = '<div> <a href="~/Home/download">列表导出</a> </div>';
    //var Export_QD = '<div> <a href="~/Home/download">代理清单导出</a> </div>';

    var Export_grid = Ext.create('Ext.form.Panel', {
        id: 'Export_grid',
        layout: {
            type: 'vbox',
            align: 'stretch'
        },
        items: [
            { layout: 'hbox', border: 0, margin: '20 0 0 0', items: [Export_list, Export_QD] },
        ],
    });

    var Export_window = Ext.create("Ext.window.Window", {
        id: 'Export_window',
        title: "导出",
        width: 400,
        height: 150,
        layout: "fit",
        //collapsible: true,
        items: [Export_grid],//destroy( Ext.Component this, Object eOpts )
        listeners: {
            //destroy: function (th, eOpts) {
            //    pgbar.doRefresh();
            //}
        }
    });
    Export_window.show();

}

function ExportExcel(type) {
    //var condition = Ext.encode(formpanel.getForm().getValues());
    var condition = getJsonCondition();
    window.location.href = "/StationedFileld/ExportExcel?type=" + type + "&condition=" + condition;
    Ext.MessageBox.alert("提示", "正在导出，如无反应请耐心等待，无需重复点击");
    //Ext.Ajax.request({
    //    url: "/StationedFileld/ExportExcel",
    //   // method: 'POST',
    //    params: { type: type },
    //    success: function (response, opts) {
    //        var aa = 0;
    //    }
    //});
}

