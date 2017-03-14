function initSearch() {
    var store_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "业务类型", "CODE": "BUSITYPE" }, { "NAME": "申报单位", "CODE": "REPUNITCODE" }, { "NAME": "进出口岸", "CODE": "PORTCODE" }]
    });
    var combo_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION1',
        name: "CONDITION1",
        value: "BUSITYPE",
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
                   if (combo_1.getValue() == "BUSITYPE") {
                       combo_1_1.minChars = "1"; 
                       store_1_1.loadData(common_data_busitype);
                   }
                   if (combo_1.getValue() == "REPUNITCODE") {
                       combo_1_1.minChars = "4";
                       store_1_1.loadData(common_data_jydw);
                   }
                   if (combo_1.getValue() == "PORTCODE") {
                       combo_1_1.minChars = "1";
                       store_1_1.loadData(common_data_sbgq);
                   }
               }
           }
    })
    var store_1_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: common_data_busitype
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
        minChars: 1,
        flex: .65
    })
    var condition1 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_1, combo_1_1]
    }
    var declarationsearch_js_condition2_data = [{ "NAME": "对应号", "CODE": "REPNO" }, { "NAME": "提运单号", "CODE": "BLNO" }
            , { "NAME": "订单编号", "CODE": "ORDERCODE" }, { "NAME": "报关单号", "CODE": "DECLNO" }
            , { "NAME": "合同协议号", "CODE": "CONTRACTNO" }, { "NAME": "运输工具名称", "CODE": "TRANSNAME" }, { "NAME": "合同发票号", "CODE": "CONTRACTNOORDER" }];
    
    var store_2 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: declarationsearch_js_condition2_data
    });
    var combo_2 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION2',
        name: "CONDITION2",
        store: store_2,
        value: "DECLNO",
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        flex: .35,
        margin: 0,
    });
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
        data: declarationsearch_js_condition4_data
    });
    var combo_3 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION3',
        name: 'CONDITION3',
        value: 'SUBMITTIME',
        margin: 0,
        flex: .35,
        store: store_3,
        queryMode: 'local',
        displayField: 'NAME',
        valueField: "CODE",
        editable: false
    })
    var date_3_1 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION3_1',
        margin: 0,
        flex: .325,
        emptyText: '开始日期',
        format: 'Y-m-d'
    })
    var date_3_2 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION3_2',
        margin: 0,
        flex: .325,
        emptyText: '结束日期',
        format: 'Y-m-d'
    })
    var condition3 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: .4,
        items: [combo_3, date_3_1, date_3_2]
    }

//新增加

    var store_4 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "业务类型", "CODE": "BUSITYPE" }, { "NAME": "申报单位", "CODE": "REPUNITCODE" }, { "NAME": "进出口岸", "CODE": "PORTCODE" }]
    });
    var combo_4 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION4',
        name: "CONDITION4",
        value: "REPUNITCODE",
        store: store_4,
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        margin: 0,
        flex: .35,
        listeners:
          {
              change: function () {
                  combo_4_1.reset();
                  if (combo_4.getValue() == "BUSITYPE") {
                      combo_4_1.minChars = "1";
                      store_4_1.loadData(common_data_busitype);
                  }
                  if (combo_4.getValue() == "REPUNITCODE") {
                      combo_4_1.minChars = "4";
                      store_4_1.loadData(common_data_jydw);
                  }
                  if (combo_4.getValue() == "PORTCODE") {
                      combo_4_1.minChars = "1";
                      store_4_1.loadData(common_data_sbgq);
                  }
              }
          }
    })
    var store_4_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: common_data_jydw
    });
    var combo_4_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION4_1',
        name: "NAME",
        margin: 0,
        store: store_4_1,
        displayField: 'NAME',
        valueField: "CODE",
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        forceSelection: true,
        minChars: 4,
        flex: .65
    })
    var condition4 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_4, combo_4_1]
    }
    var store_5 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: declarationsearch_js_condition2_data
    });
    var combo_5 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION5',
        name: "CONDITION5",
        store: store_5,
        value: "CONTRACTNO",
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        flex: .35,
        margin: 0,
    });
    var field_5_1 = Ext.create('Ext.form.field.Text', {
        id: 'CONDITION5_1',
        margin: 0,
        flex: .65
    })
    var condition5 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_5, field_5_1]
    }



    var store_6 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: declarationsearch_js_condition4_data
    });
    var combo_6 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION6',
        name: 'CONDITION6',
        value: 'REPTIME',
        margin: 0,
        flex: .35,
        store: store_6,
        queryMode: 'local',
        displayField: 'NAME',
        valueField: "CODE",
        editable: false
    })
    var date_6_1 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION6_1',
        margin: 0,
        flex: .325,
        emptyText: '开始日期',
        format: 'Y-m-d'
    })
    var date_6_2 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION6_2',
        margin: 0,
        flex: .325,
        emptyText: '结束日期',
        format: 'Y-m-d'
    })
    var condition6 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: .4,
        items: [combo_6, date_6_1, date_6_2]
    }

    var formpanel = Ext.create('Ext.form.Panel', {
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.3
        },
        items: [
        { layout: 'column', border: 0, items: [condition1, condition2, condition3] },
        { layout: 'column', border: 0, items: [condition4, condition5, condition6] }

        ]
    });

}

//重置
function Reset() {
    Ext.getCmp("CONDITION1").setValue("BUSITYPE");
    Ext.getCmp("CONDITION1_1").setValue("");
    Ext.getCmp("CONDITION2").setValue("DECLNO");
    Ext.getCmp("CONDITION2_1").setValue("");
    Ext.getCmp("CONDITION3").setValue("SUBMITTIME");
    Ext.getCmp("CONDITION3_1").setValue("");
    Ext.getCmp("CONDITION3_2").setValue("");
    //
    Ext.getCmp("CONDITION4").setValue("REPUNITCODE");
    Ext.getCmp("CONDITION4_1").setValue("");
    Ext.getCmp("CONDITION5").setValue("CONTRACTNO");
    Ext.getCmp("CONDITION5_1").setValue("");
    Ext.getCmp("CONDITION6").setValue("REPTIME");
    Ext.getCmp("CONDITION6_1").setValue("");
    Ext.getCmp("CONDITION6_2").setValue("");
}