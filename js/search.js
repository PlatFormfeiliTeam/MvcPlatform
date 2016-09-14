
//////////////////////////combox开始//////////////////////
function initSearch() {
    var store_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME", "QUANCODE"],
        data: search_js_condition1_data,
        listeners: { load: function () { store_1_1.load(); } }
    });
    var combo_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION1',
        name: "CONDITION1",
        store: store_1,
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        margin: 0,
        flex: .35,
        listeners:
            {
                change: function () {
                    combo_1_1.reset();
                    if (combo_1.getValue() == "BUSIUNITCODE") {
                        //如果是经营单位作为过滤条件，则CONDITION1_1绑定QUANCODE
                        combo_1_1.valueField = "QUANCODE";
                        store_1_1.loadData(common_data_jydw);
                    }
                    if (combo_1.getValue() == "CUSTOMDISTRICTCODE" || combo_1.getValue() == "PORTCODE") {
                        store_1_1.loadData(common_data_sbgq);
                    }
                    if (combo_1.getValue() == "REPWAYID") {
                        store_1_1.loadData(common_data_sbfs);
                    }
                }
            }
    })
    var store_1_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME", "QUANCODE"]
    });
    var combo_1_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION1_1',
        name: "NAME",
        margin: 0,
        store: store_1_1,
        displayField: 'NAME',
        valueField: "CODE",
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        forceSelection: true,
        flex: .65,
        listeners: {
            //focus: function () {
            //    combo_1_1.expand();
            //}
        }
    })
    var condition1 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_1, combo_1_1]
    }
    var store_2 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition2_data
    });
    var combo_2 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION2',
        name: "CONDITION2",
        store: store_2,
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        flex: .35,
        margin: 0
    })
    var field_2_1 = Ext.create('Ext.form.field.Text', {
        id: 'CONDITION2_1',
        margin: 0,
        flex: .65
    })
    var condition2 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_2, field_2_1]
    }
    var store_3 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition3_data,
        listeners: { load: function () { store_3_1.load(); } }
    });
    var combo_3 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION3',
        name: "CONDITION3",
        store: store_3,
        queryMode: 'local',
        editable: false,
        displayField: 'NAME',
        valueField: "CODE",
        margin: 0,
        flex: .35,
        listeners: {
            change: function () {
                combo_3_1.reset();
                if (combo_3.getValue() == "ddzt") {
                    store_3_1.loadData(search_js_condition3_orderstatus_data);
                }
                if (combo_3.getValue() == "bgzt" || combo_3.getValue() == "bjzt") {
                    store_3_1.loadData(search_js_condition3_bgbjstatus_data);
                }
                if (combo_3.getValue() == "yjzt") {
                    store_3_1.loadData(search_js_condition3_yjstatus_data);
                }
            }
        }
    });
    var store_3_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME", 'parentid']
    });
    var combo_3_1 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION3_1',
        store: store_3_1,
        displayField: 'NAME',
        valueField: "CODE",
        hideTrigger: true,
        margin: 0,
        flex: .65,
        //anyMatch: true,
        editable: false,
        queryMode: 'local'
    });
    var condition3 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_3, combo_3_1]
    }
    var store_4 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition4_data
    });
    var combo_4 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION4',
        name: 'CONDITION4',
        margin: 0,
        flex: .35,
        store: store_4,
        queryMode: 'local',
        displayField: 'NAME',
        valueField: "CODE",
        editable: false
    })
    var date_4_1 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION4_1',
        margin: 0,
        flex: .325,
        emptyText: '开始日期',
        format: 'Y-m-d'
    })
    var date_4_2 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION4_2',
        margin: 0,
        flex: .325,
        emptyText: '结束日期',
        format: 'Y-m-d'
    })
    var condition4 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: .34,
        items: [combo_4, date_4_1, date_4_2]
    }
    var store_5 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition1_data,
        listeners: {
            load: function () {
                store_5_1.load();
            }
        }
    });
    var combo_5 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION5',
        name: "CONDITION5",
        store: store_5,
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        margin: 0,
        flex: .35,
        listeners:
            {
                change: function () {
                    combo_5_1.reset();
                    if (combo_5.getValue() == "BUSIUNITCODE") {
                        store_5_1.loadData(common_data_jydw);
                    }
                    if (combo_5.getValue() == "CUSTOMDISTRICTCODE" || combo_5.getValue() == "PORTCODE") {
                        store_5_1.loadData(common_data_sbgq);
                    }
                    if (combo_5.getValue() == "REPWAYID") {
                        store_5_1.loadData(common_data_sbfs);
                    }
                }
            }
    })
    var store_5_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"]
    });
    var combo_5_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION5_1',
        margin: 0,
        store: store_5_1,
        displayField: 'NAME',
        valueField: "CODE",
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        forceSelection: true,
        flex: .65
    })
    var condition5 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_5, combo_5_1]
    }
    var store_6 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition2_data
    });
    var combo_6 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION6',
        name: "CONDITION6",
        store: store_6,
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        flex: .35,
        margin: 0
    })
    var field_6_1 = {
        id: 'CONDITION6_1',
        margin: 0,
        flex: .65,
        xtype: 'textfield'
    }
    var condition6 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_6, field_6_1]
    }
    var store_7 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition3_data,
        listeners: { load: function () { store_7_1.load(); } }
    });
    var combo_7 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION7',
        name: "CONDITION7",
        store: store_7,
        queryMode: 'local',
        editable: false,
        displayField: 'NAME',
        valueField: "CODE",
        margin: 0,
        flex: .35,
        listeners: {
            change: function () {
                combo_7_1.reset();
                if (combo_7.getValue() == "ddzt") {
                    store_7_1.loadData(search_js_condition3_orderstatus_data);
                }
                if (combo_7.getValue() == "bgzt" || combo_7.getValue() == "bjzt") {
                    store_7_1.loadData(search_js_condition3_bgbjstatus_data);
                }
                if (combo_7.getValue() == "yjzt") {
                    store_7_1.loadData(search_js_condition3_yjstatus_data);
                }
            }
        }
    });
    var store_7_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"]
    });
    var combo_7_1 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION7_1',
        name: "NAME",
        store: store_7_1,
        displayField: 'NAME',
        valueField: "CODE",
        hideTrigger: true,
        margin: 0,
        flex: .65,
        editable: false,
        queryMode: 'local'
    });
    var condition7 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_7, combo_7_1]
    }
    var store_8 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition4_data
    });
    var combo_8 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION8',
        margin: 0,
        flex: .35,
        name: "CONDITION8",
        store: store_8,
        queryMode: 'local',
        displayField: 'NAME',
        valueField: "CODE",
        editable: false
    })
    var date_8_1 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION8_1',
        margin: 0,
        flex: .325,
        emptyText: '开始日期',
        format: 'Y-m-d'
    })
    var date_8_2 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION8_2',
        margin: 0,
        flex: .325,
        emptyText: '结束日期',
        format: 'Y-m-d'
    })
    var condition8 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: .34,
        items: [combo_8, date_8_1, date_8_2]
    }
    formpanel = Ext.create('Ext.form.Panel', {
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.22
        },
        items: [
        { layout: 'column', border: 0, items: [condition1, condition2, condition3, condition4] },
        { layout: 'column', border: 0, items: [condition5, condition6, condition7, condition8] }
        ]
    });
    //加载当前登录用户的默认查询条件
    //formpanel.getForm().load({
    //    url: "/Common/loadquerysetting"
    //});
    Ext.Ajax.request({//对公共基础数据发起一次请求
        url: "/Common/loadquerysetting",
        success: function (response, option) {
            var result = Ext.decode(response.responseText); 
            if (result.data) {
                formpanel.getForm().setValues(result.data);
            }
        }
    });
}
function Reset() {
    Ext.getCmp("CONDITION1_1").setValue("");
    Ext.getCmp("CONDITION2_1").setValue("");
    Ext.getCmp("CONDITION3_1").setValue("");
    Ext.getCmp("CONDITION4_1").setValue("");
    Ext.getCmp("CONDITION4_2").setValue("");
    Ext.getCmp("CONDITION5_1").setValue("");
    Ext.getCmp("CONDITION6_1").setValue("");
    Ext.getCmp("CONDITION7_1").setValue("");
    Ext.getCmp("CONDITION8_1").setValue("");
    Ext.getCmp("CONDITION8_2").setValue("");
}