function initSearch() {
    var store_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "报关状态", "CODE": "DECLSTATUS" }, { "NAME": "报检状态", "CODE": "INSPSTATUS" }]
    });
    var combo_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION1',
        name: "CONDITION1",
        value: "DECLSTATUS",
        store: store_1,
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        margin: 0,
        flex: .35
    })
    var store_1_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition3_bgbjstatus_data
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
        editable: false,
        forceSelection: true,
        minChars: 1,
        flex: .65
    })
    var condition1 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_1, combo_1_1]
    }
    var store_2 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "合同发票号", "CODE": "CONTRACTNO" }]
    });
    var combo_2 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION2',
        name: "CONDITION2",
        store: store_2,
        value: "CONTRACTNO",
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
        data: [{ "NAME": "对应号", "CODE": "REPNO" }, { "NAME": "分单号", "CODE": "DIVIDENO" }, { "NAME": "载货清单号", "CODE": "MANIFEST" }, { "NAME": "海关提运单号", "CODE": "SECONDLADINGBILLNO" }]
    });
    var combo_3 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION3',
        name: "CONDITION3",
        store: store_3,
        value: "REPNO",
        displayField: 'NAME',
        valueField: "CODE",
        editable: false,
        queryMode: 'local',
        flex: .35,
        margin: 0,
    });
    var field_3_1 = Ext.create('Ext.form.field.Text', {
        id: 'CONDITION3_1',
        margin: 0,
        flex: .65
    })
    var condition3 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_3, field_3_1]
    }

    var store_4 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: [{ "NAME": "申报关区", "CODE": "CUSTOMAREACODE" }, { "NAME": "申报方式", "CODE": "REPWAYID" }, { "NAME": "进出口岸", "CODE": "PORTCODE" }]
    });
    var combo_4 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION4',
        name: "CONDITION4",
        value: "CUSTOMAREACODE",
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
                  if (combo_4.getValue() == "CUSTOMAREACODE") {
                      combo_4_1.minChars = "2";
                      store_4_1.loadData(common_data_sbgq);
                  }
                  if (combo_4.getValue() == "REPWAYID") {
                      combo_4_1.minChars = "1";
                      store_4_1.loadData(common_data_sbfs);
                  }
                  if (combo_4.getValue() == "PORTCODE") {
                      combo_4_1.minChars = "2";
                      store_4_1.loadData(common_data_sbgq);
                  }
              }
          }
    })
    var store_4_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: common_data_sbgq
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
        minChars: 2,
        flex: .65
    })
    var condition4 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_4, combo_4_1]
    }
   



    var store_6 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        //data: declarationsearch_js_condition4_data
    });

    var formpanel = Ext.create('Ext.form.Panel', {
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.25
        },
        items: [
        { layout: 'column', border: 0, items: [condition1,condition2, condition3,condition4] },
        ]
    });

}

//重置
function Reset() {
    Ext.getCmp("CONDITION1").setValue("DECLSTATUS");
    Ext.getCmp("CONDITION1_1").setValue("");
    Ext.getCmp("CONDITION2").setValue("CONTRACTNO");
    Ext.getCmp("CONDITION2_1").setValue("");
    Ext.getCmp("CONDITION3").setValue("REPNO");
    Ext.getCmp("CONDITION3_1").setValue("");
    Ext.getCmp("CONDITION4").setValue("CUSTOMAREACODE");
    Ext.getCmp("CONDITION4_1").setValue("");
}