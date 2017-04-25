var store_Tool_added, store_type;
Ext.onReady(function () {

    var model_Type = Ext.define('model_Type', {
        extend: 'Ext.data.Model',
        fields: ["ID", "TYPENAME", "TYPEID"]
    });
    store_type = Ext.create('Ext.data.JsonStore', {
        model: 'model_Type'
    });

    var tmp = new Ext.XTemplate(
   '<tpl for=".">',
   '<div class="panel panel-default thumb-wrap fl" style="margin-top:5px;margin-left:5px;width:95px;text-align:center;cursor:pointer">',
   '<div class="panel-body" style="text-align:center;padding:0px"><img src="{[values.ICON]}"/></div>',
   '<div class="panel-heading" style=" text-align:center;padding:5px 10px;">{[values.NAME]}</div>',
   '</div>',
   '</tpl>',
   '<div class="panel panel-default fl" style="margin-top:5px;margin-left:5px;width:95px;height:90px;text-align:center">',
   '<span style="margin:8px 20px;display:block;cursor:pointer" onclick="addItem()"><i class="iconfont" style="font-size:50px;">&#xe60d;</i></span>',
   '</div>'
   )
    var model_Tool = Ext.define('model_Tool', {
        extend: 'Ext.data.Model',
        fields: ["RID", "ICON", "NAME", "URL", "TYPENAME", "CREATEID", "CREATEDATE", "ISINVALID"]
    });
    store_Tool_added = Ext.create('Ext.data.JsonStore', {
        model: 'model_Tool',
        proxy: {
            type: 'ajax',
            url: '/FrequentInfor/GetTools',
            reader: {
                root: 'rows',
                type: 'json'
            }
        },
        autoLoad: true,
        listeners: {
            datachanged: function (store, eOpts) {
                if (store.data.length == 0) {
                    if (Ext.getCmp("add_view")) { Ext.getCmp("add_view").hide(); }

                } else {
                    if (Ext.getCmp("add_view")) { Ext.getCmp("add_view").show(); }
                }
            }


        }

    });

    var fileview = Ext.create('Ext.view.View', {
        store: store_Tool_added,
        tpl: tmp,
        itemSelector: 'div.thumb-wrap',
        multiSelect: false,
        listeners: {
            selectionchange: function (thisView, selections) {
                // alert(selections[0].data.URL);
                window.location = selections[0].data.URL;
            }
        }


    })


    var panel_top = Ext.create('Ext.panel.Panel', {
        title: '<i class="icon iconfont" aria-hidden="true">&#xe6e4</i>&nbsp;常用工具',
        renderTo: "div_form",
        border: 0,
        items: [fileview]
    });


    var model_News = Ext.define('model_News', {
        extend: 'Ext.data.Model',
        fields: ["ID", "RID", "TITLE", "PUBLISHDATE", "ISINVALID"]
    });
    var store_News = Ext.create('Ext.data.JsonStore', {
        model: 'model_News',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/FrequentInfor/GetNews',
            reader: {
                root: 'rows',
                type: 'json',
                totalProperty: 'total'
            }
        },
        autoLoad: true

    });
    var pgbar = Ext.create('Ext.toolbar.Paging', {
        displayMsg: '显示 {0} - {1} 条,共计 {2} 条',
        store: store_News,
        id: 'pgbar',
        displayInfo: true
    })

    var tbar = Ext.create('Ext.toolbar.Toolbar', {
        items: [

               {
                   text: '<span class="icon iconfont" style="font-size:12px;color:#3071A9;">&#xe6d3;</span>&nbsp;<font color="#3071A9">取 消 收 藏</font>',
                   handler: function () { delete_News(); }
               }]

    });
    var grid_new = Ext.create('Ext.grid.Panel', {
        id: "grid_new",
        store: store_News,
        height: 500,
        selModel: { selType: 'checkboxmodel' },
        tbar: tbar,
        bbar: pgbar,
        enableColumnHide: false,
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: 'ID', dataIndex: 'ID', hidden: true },
        { header: '标题', dataIndex: 'TITLE', flex: 0.8, renderer: render },
        { header: '发布时间', dataIndex: 'PUBLISHDATE', flex: 0.1 },
        ],
        listeners:
        {
            //'itemdblclick': function (view, record, item, index, e) {
            //    javascript: window.open("/Home/IndexNoticeDetail?id=" + record.data.RID, "_blank")
            //}
        },
        viewConfig: {
            enableTextSelection: true
        }
    })


    var panel_bottom = Ext.create('Ext.panel.Panel', {
        title: '<i class="fa fa-hand-o-right" aria-hidden="true"></i>&nbsp;收藏新闻',
        renderTo: "appConId",
        border: 0,
        items: [grid_new]
    })
});

function addItem() {


    var model_Tool = Ext.define('model_Tool', {
        extend: 'Ext.data.Model',
        fields: ["RID", "ICON", "NAME", "URL", "TYPENAME", "CREATEID", "CREATEDATE", "ISINVALID"]
    });
    var tmp_tool = new Ext.XTemplate(
              '<tpl for=".">',
              '<div id="tpl_div_{[values.RID]}" class="panel panel-default thumb-wrap fl" style="margin-top:5px;margin-left:5px;width:95px;text-align:center;cursor:pointer">',
              '<div class="panel-body" style="text-align:center;padding:0px"><img src="{[values.ICON]}"/></div>',
              '<div class="panel-heading" style=" text-align:center;padding:5px 10px;">{[values.NAME]}</div>',
              '</div>',
              '</tpl>');
    var tmp_tool_add = new Ext.XTemplate(
              '<div style="padding-top:5px;width:100%;height:30px;background:#d9e4ec;text-align:center;color:#7a8693">已添加收藏</div>',
              '<tpl for=".">',
              '<div id="tpl_div_{[values.RID]}" class="panel panel-default thumb-wrap fl" style="margin-top:5px;margin-left:5px;width:95px;text-align:center;cursor:pointer">',
              '<div class="panel-body" style="text-align:center;padding:0px"><img src="{[values.ICON]}"/></div>',
              '<div class="panel-heading" style=" text-align:center;padding:5px 10px;">{[values.NAME]}</div>',
              '</div>',
              '</tpl>',
              '<div style="padding-top:5px;width:100%;height:30px;background:#d9e4ec;text-align:center;color:#7a8693;float:left">未添加工具</div>');

    var text_tmp;
    var add_view = new Ext.view.View({
        id: "add_view",
        tpl: tmp_tool_add,
        store: store_Tool_added,
        width: '100%',
        itemSelector: 'div.thumb-wrap',
        multiSelect: false,
        listeners: {
            selectionchange: function (thisView, selections) {
                // window.location = selections[0].data.URL;
            },
            itemmouseenter: function (thisView, record, item, index, e) {
                text_tmp = item.childNodes[1].innerHTML;
                item.childNodes[1].innerHTML = "<font color='#3071A9'><b>删除</b></font>";
                Ext.get("tpl_div_" + record.data.RID).dom.className = "panel panel-default thumb-wrap x-item-selected fl";
            },
            itemmouseleave: function (thisView, record, item, index, e) {

                item.childNodes[1].innerHTML = text_tmp;
                Ext.get("tpl_div_" + record.data.RID).dom.className = "panel panel-default thumb-wrap fl";
            },
            itemclick: function (thisView, record, item, index, e) {
                var typeid = store_type.findRecord("TYPENAME", record.data.TYPENAME).data.TYPEID;
                Ext.getCmp("view_" + typeid).store.insert(Ext.getCmp("view_" + typeid).store.data.length, record.data);
                var num = Ext.getCmp("view_" + typeid).store.data.length;
                Ext.getCmp("fs_" + typeid).setTitle(record.data.TYPENAME + "(" + num + ")");
                Ext.Ajax.request({
                    url: '/FrequentInfor/ManageTools?act=del',
                    params: record.data,
                    success: function (response, opts) {
                        var result = Ext.decode(response.responseText);
                        if (result.success) {
                            //Ext.MessageBox.alert("提示", "移除成功！");
                            store_Tool_added.remove(record);
                        }

                    }
                });
            }
        }
    })
    var fieldSet_tool = [add_view];
    if (store_Tool_added.data.length == 0) {
        if (Ext.getCmp("add_view")) { Ext.getCmp("add_view").hide(); }

    } else {
        if (Ext.getCmp("add_view")) { Ext.getCmp("add_view").show(); }
    }
    Ext.Ajax.request({
        url: '/FrequentInfor/GetToolsGroupByType',
        success: function (response, opts) {
            var Tool_data = Ext.decode(response.responseText);
            store_type.loadData(Tool_data);
            for (var i = 0; i < Tool_data.length; i++) {
                var store_Tool_tmp = Ext.create('Ext.data.JsonStore', { model: 'model_Tool' });
                var el_id = Tool_data[i].TYPEID;
                var data_item = Ext.decode(Tool_data[i].ITEM);
                store_Tool_tmp.loadData(data_item);
                var set_tmp = new Ext.form.FieldSet({
                    title: Tool_data[i].TYPENAME + "(" + data_item.length + ")",
                    id: 'fs_' + el_id,
                    width: '99%',
                    border: '1 0 0 0',
                    style: 'padding:0 0',
                    margin: 5,
                    items: [
                        new Ext.view.View({
                            store: store_Tool_tmp,
                            tpl: tmp_tool,
                            id: "view_" + el_id,
                            itemSelector: 'div.thumb-wrap',
                            multiSelect: false,
                            listeners: {
                                selectionchange: function (thisView, selections) {
                                    // window.location = selections[0].data.URL;
                                },
                                itemmouseenter: function (thisView, record, item, index, e) {
                                    //text_tmp = item.childNodes[1].innerText;
                                    text_tmp = item.childNodes[1].innerHTML;
                                    item.childNodes[1].innerHTML = "<font color='#3071A9'><b>添加</b></font>";
                                    //item.childNodes[1].innerText = "添加";
                                    Ext.get("tpl_div_" + record.data.RID).dom.className = "panel panel-default thumb-wrap x-item-selected fl";
                                },
                                itemmouseleave: function (thisView, record, item, index, e) {
                                    // item.select(record);
                                    //alert(record.data.ID);
                                    // alert(item.childNodes[1].innerText);
                                    //item.childNodes[1].innerText = text_tmp;
                                    item.childNodes[1].innerHTML = text_tmp;
                                    Ext.get("tpl_div_" + record.data.RID).dom.className = "panel panel-default thumb-wrap fl";
                                },
                                itemclick: function (thisView, record, item, index, e) {

                                    thisView.store.remove(record);


                                    var typeid = store_type.findRecord("TYPENAME", record.data.TYPENAME).data.TYPEID;
                                    var num = Ext.getCmp("view_" + typeid).store.data.length;
                                    Ext.getCmp("fs_" + typeid).setTitle(record.data.TYPENAME + "(" + num + ")");
                                    var rec = store_Tool_added.findRecord('RID', record.data.RID);
                                    if (!rec) {
                                        store_Tool_added.insert(store_Tool_added.data.length, record.data);
                                        Ext.Ajax.request({
                                            url: '/FrequentInfor/ManageTools?act=add',
                                            params: record.data,
                                            success: function (response, opts) {
                                                var result = Ext.decode(response.responseText);
                                                if (result.success) {  // Ext.MessageBox.alert("提示", "添加成功！");
                                                }
                                            }
                                        });
                                    }

                                }
                            }
                        })
                    ]
                });
                fieldSet_tool.push(set_tmp);
            }

            var win = Ext.create('Ext.window.Window', {
                title: '常用收藏',
                height: 800,
                width: 840,
                border: 0,
                bodyStyle: 'background:white;',
                layout: 'column',
                overflowY: 'scroll',
                modal: true,
                items: fieldSet_tool
            }).show();

        }

    });







}

function delete_News() {
    var records = Ext.getCmp("grid_new").getSelectionModel().getSelection();
    if (records.length == 0) {
        Ext.MessageBox.alert("提示", "请选择要取消的记录！");
        return;
    }
    var rids = "";
    for (var i = 0; i < records.length; i++) {
        if (i != records.length - 1) {
            rids += records[i].data.RID + ',';
        }
        else {
            rids += records[i].data.RID;
        }

    }
    Ext.Ajax.request({
        url: '/FrequentInfor/ManageNews?act=del',
        params: { rid: rids },
        success: function (response, opts) {
            var result = Ext.decode(response.responseText);
            if (result.success) {
                Ext.MessageBox.alert("提示", "取消收藏成功！");
                Ext.getCmp("pgbar").moveFirst();
            }

        }
    });
}
function render(value, cellmeta, record, rowIndex, columnIndex, store) {
    var rtn = "";
    var dataindex = cellmeta.column.dataIndex;
    switch (dataindex) {
        case "TITLE":
            rtn = "<a href='/Home/IndexNoticeDetail?id=" + record.get("RID") + "' target='_blank'>" + record.get("TITLE") + "</a>";
            break;
    }
    return rtn;
}