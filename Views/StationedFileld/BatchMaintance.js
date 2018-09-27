function maintance() {

    var recs = gridpanel.getSelectionModel().getSelection();
    if (recs.length == 0) {
        Ext.MessageBox.alert('提示', '请选择需要维护的记录！');
        return;
    }

    var maintance_ACCEPTTIME = Ext.create('Ext.button.Button', {
        text: '受理时间',
        id: 'maintance_ACCEPTTIME',
        flex: 0.5,
        height: 30,
        margin: '10',
        handler: function () {
            maintance_save(maintance_ACCEPTTIME);
        }, 
    })
    var maintance_MOENDTIME = Ext.create('Ext.button.Button', {
        text: '制单完成时间',
        id: 'maintance_MOENDTIME',
        flex: 0.5,
        height: 30,
        margin: '10',
        handler: function () {
            maintance_save(maintance_MOENDTIME);
        },
    })

    var maintance_COENDTIME = Ext.create('Ext.button.Button', {
        text: '审核完成时间',
        id: 'maintance_COENDTIME',
        flex: 0.5,
        height: 30,
        margin: '10',
        handler: function () {
            maintance_save(maintance_COENDTIME);
        },
    })
    var maintance_RECOENDTIME = Ext.create('Ext.button.Button', {
        text: '复审完成时间',
        id: 'maintance_RECOENDTIME',
        flex: 0.5,
        height: 30,
        margin: '10',
        handler: function () {
            maintance_save(maintance_RECOENDTIME);
        },
    })

    var maintance_REPSTARTTIME = Ext.create('Ext.button.Button', {
        text: '申报时间',
        id: 'maintance_REPSTARTTIME',
        flex: 0.5,
        height: 30,
        margin: '10',
        handler: function () {
            maintance_save(maintance_REPSTARTTIME);
        },
    })
    var maintance_REPENDTIME = Ext.create('Ext.button.Button', {
        text: '申报完成时间',
        id: 'maintance_REPENDTIME',
        flex: 0.5,
        height: 30,
        margin: '10',
        handler: function () {
            maintance_save(maintance_REPENDTIME);
        },
    })

    var maintance_PASSTIME = Ext.create('Ext.button.Button', {
        text: '通关放行时间',
        id: 'maintance_PASSTIME',
        flex: 1,
        margin: '10 10 10 10',
        height: 30,
        margin: '10',
        handler: function () {
            maintance_save(maintance_PASSTIME);
        },
    })

    var maintance_grid = Ext.create('Ext.form.Panel', {
        id: 'maintance_grid',
        layout: {
            type: 'vbox',
            align: 'stretch'
        },
        items: [
            { layout: 'hbox', border: 0, margin: '30 0 0 0', items: [maintance_ACCEPTTIME, maintance_RECOENDTIME] },
            { layout: 'hbox', border: 0,  items: [maintance_MOENDTIME, maintance_REPSTARTTIME] },
            { layout: 'hbox', border: 0,  items: [maintance_COENDTIME, maintance_REPENDTIME] },
            { layout: 'hbox', border: 0,  items: [maintance_PASSTIME] },
        ],
    });

    var maintance_window = Ext.create("Ext.window.Window", {
        id: 'maintance_window',
        title: "批量维护",
        width: 500,
        height: 300,
        layout: "fit",
        //collapsible: true,
        items: [maintance_grid],//destroy( Ext.Component this, Object eOpts )
        listeners: {
            destroy: function (th, eOpts) {
                pgbar.doRefresh();
            }
        }
    });
    maintance_window.show();
}

function maintance_save(id) {
    var recs = gridpanel.getSelectionModel().getSelection();
    var codes = "";
    for (var i = 0; i < recs.length; i++) {
        codes += recs[i].data.CODE + ',';
    }
    codes = codes.substr(0, codes.length - 1);

    var mask = new Ext.LoadMask(Ext.get(maintance_window), { msg: "数据保存中，请稍等..." });//Ext.getBody()
    mask.show();

    Ext.Ajax.request({
        url: "/StationedFileld/batchMaintance",
        params: { codes: codes,id:id.id },
        success: function (response, opts) {
            mask.hide();
            var data = Ext.decode(response.responseText);
            if (data.success) {
                //Ext.MessageBox.alert("提示", "保存成功\r\n选中个数为：" + recs.length + "\t成功个数为：" + data.successQty);
                Ext.MessageBox.alert("提示", "保存成功");
               // pgbar.doRefresh();
            }
            else {
                Ext.MessageBox.alert("提示", data.msg);
            }
        }
    });
}