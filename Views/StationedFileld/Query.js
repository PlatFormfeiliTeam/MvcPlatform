
//////////////////////////combox开始//////////////////////
function initQuery() {
   // var manifest_data = [{ "NAME": "已入库", "CODE": "1" }, { "NAME": "未入库", "CODE": "0" }];
    //if (busitypeid == '11') {
    //    search_js_condition3_data.push({ "NAME": "舱单入库", "CODE": "MANIFEST_STORAGE" });
    //    search_js_condition3_data.push({ "NAME": "物流状态", "CODE": "LOGISTICSSTATUS" });
    //}

    //if (busitypeid == '10' || busitypeid == '30') {
    //    search_js_condition2_data.push({ "NAME": "报关车号", "CODE": "DECLCARNO" });
    //}

    var store_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition1_StationField,
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
       // value: 'CUSNO'
    })
    var field_1_1 = Ext.create('Ext.form.field.Text', {
        id: 'CONDITION1_1',
        margin: 0,
        flex: .65,
    })
    var condition1 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_1, field_1_1]//combo_1_1
    }
    var store_2 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition2_StationField
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
        //value: 'PORTCODE',
        margin: 0,
        listeners:
        {
         change: function () {
            combo_2_1.reset();
            if (combo_2.getValue() == "PORTCODE") {
                combo_2_1.minChars = "1";
                store_2_1.loadData(common_data_sbgq);//common_data_jydw
            }
            if (combo_2.getValue() == "TRADEWAY") {
                combo_2_1.minChars = "1";
                store_2_1.loadData(common_data_myfs);
            }
            if (combo_2.getValue() == "BUSITYPE") {
                combo_2_1.minChars = "1";
                store_2_1.loadData(common_data_ywlx);
            }
            }
       }
    })

    var store_2_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"]
    });
    var combo_2_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION2_1',
        name: "NAME",
        margin: 0,
        store: store_2_1,
        displayField: 'NAME',
        valueField: "CODE",
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        forceSelection: true,
        flex: .65,
        listeners:
              {
                 change: function () {
                          if (combo_2_1.getValue() == null) {
                             combo_2_1.reset();
                                           }
                                    }
              }
    })
    var condition2 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_2, combo_2_1]//combo_1_1  field_2_1
    }
    var store_3 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition3_StationField,
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
        //value: 'STATUS',
        listeners: {
            change: function () {
                combo_3_1.reset();
                if (combo_3.getValue() == "STATUS") {
                    combo_3_1.minChars = "1";
                    store_3_1.loadData(common_data_StationFieldStatus);
                }
                if (combo_3.getValue() == "INSPFLAG") {
                    combo_3_1.minChars = "1";
                    store_3_1.loadData(common_data_YesOrNot);
                }
                if (combo_3.getValue() == "MANIFEST") {
                    combo_3_1.minChars = "1";
                    store_3_1.loadData(common_data_YesOrNot);
                }
            }
        }
    });
    var store_3_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"]
    });
    var combo_3_1 = Ext.create("Ext.form.ComboBox", {
        id: 'CONDITION3_1',
        store: store_3_1,
        displayField: 'NAME',
        valueField: "CODE",
        hideTrigger: true,
        margin: 0,
        flex: .65,
        anyMatch: true,
        // editable: true,
        forceSelection: true,
        queryMode: 'local',
        listeners:
         {
          change: function () {
              if (combo_3_1.getValue() == null) {
                  combo_3_1.reset();
              }
          }
        }
    });
    var condition3 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_3, combo_3_1]
    }
    var store_4 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition4_StationField
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
       // value: 'SUBMITTIME',
        editable: false
    })
            //初始化时间控件
            //var startTime = new Date();
            //startTime.setDate(startTime.getDate() - 7);

    var date_4_1 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION4_1',
        margin: 0,
        flex: .325,
        emptyText: '开始日期',
        format: 'Y-m-d',
       // value: startTime,
        listeners: {
            //focus: function (cb) {
            //    var con = Ext.Date.format(Ext.getCmp("CONDITION4_2").getValue(), 'Y-m-d');
            //    if (con != "") {
            //        var da_before = new Date(con);
            //        da_before.setFullYear(da_before.getFullYear() - 1);
            //        da_before.setDate(da_before.getDate() + 1);
            //        var da = new Date(con);

            //        cb.setMinValue(da_before); cb.setMaxValue(da);
            //    } else {
            //        //cb.reset();//没用
            //        cb.setMinValue(new Date("0000-01-01")); cb.setMaxValue(new Date("9999-12-31"));
            //    }
            //}
        }
    });

    //初始化时间控件
   // var endTime = new Date();

    var date_4_2 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION4_2',
        margin: 0,
        flex: .325,
        emptyText: '结束日期',
        format: 'Y-m-d',
       // value: endTime,
        listeners: {
            //focus: function (cb) {
            //    var con = Ext.Date.format(Ext.getCmp("CONDITION4_1").getValue(), 'Y-m-d');
            //    if (con != "") {
            //        var da_after = new Date(con);
            //        da_after.setFullYear(da_after.getFullYear() + 1);
            //        da_after.setDate(da_after.getDate() - 1);
            //        var da = new Date(con);

            //        cb.setMinValue(da); cb.setMaxValue(da_after);
            //    } else {
            //        //cb.reset();//没用
            //        cb.setMinValue(new Date("0000-01-01")); cb.setMaxValue(new Date("9999-12-31"));
            //    }
            //}
        }
    });
    var condition4 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: .28,
        items: [combo_4, date_4_1, date_4_2]
    }
    var store_5 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition1_StationField,
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
       // value: 'DIVIDENO'
        //listeners:
        //    {
        //        change: function () {
        //            combo_5_1.reset();
        //            if (combo_5.getValue() == "BUSIUNITCODE") {
        //                combo_5_1.minChars = "4";
        //                store_5_1.loadData(common_data_jydw);
        //            }
        //            if (combo_5.getValue() == "CUSTOMDISTRICTCODE" || combo_5.getValue() == "PORTCODE") {
        //                combo_5_1.minChars = "1";
        //                store_5_1.loadData(common_data_sbgq);
        //            }
        //            if (combo_5.getValue() == "REPWAYID") {
        //                combo_5_1.minChars = "1";
        //                store_5_1.loadData(common_data_sbfs);
        //            }
        //        }
        //    }
    })
    //var store_5_1 = Ext.create("Ext.data.JsonStore", {
    //    fields: ["CODE", "NAME"]
    //});
    var field_5_1 = Ext.create('Ext.form.field.Text', {
        id: 'CONDITION5_1',
        margin: 0,
        flex: .65
    })
    var condition5 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_5, field_5_1]//field_1_1  combo_5_1
    }
    var store_6 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition2_StationField
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
       // value: 'TRADEWAY',
        margin: 0,
        listeners:
        {
        change: function () {
              combo_6_1.reset();
            if (combo_6.getValue() == "PORTCODE") {
               combo_6_1.minChars = "1";
               store_6_1.loadData(common_data_sbgq);
              }
            if (combo_6.getValue() == "TRADEWAY") {
              combo_6_1.minChars = "1";
              store_6_1.loadData(common_data_myfs);
             }
           if (combo_6.getValue() == "BUSITYPE") {
              combo_6_1.minChars = "1";
              store_6_1.loadData(common_data_ywlx);
             }
           }
      }
    })

    var store_6_1 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"]
    });
    var combo_6_1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'CONDITION6_1',
        name: "NAME",
        margin: 0,
        store: store_6_1,
        displayField: 'NAME',
        valueField: "CODE",
        hideTrigger: true,
        anyMatch: true,
        queryMode: 'local',
        forceSelection: true,
        flex: .65,
        listeners:
        {
          change: function () {
              if (combo_6_1.getValue() == null) {
                  combo_6_1.reset();
              }
          }
        }
    })
    var condition6 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_6, combo_6_1]//combo_1_1 field_6_1
    }
    var store_7 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition3_StationField,
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
        //value: 'INSPFLAG',
        listeners: {
            change: function () {
                combo_7_1.reset();
                if (combo_7.getValue() == "STATUS") {
                    combo_7_1.minChars = "1";
                    store_7_1.loadData(common_data_StationFieldStatus);
                }
                if (combo_7.getValue() == "INSPFLAG") {
                    combo_7_1.minChars = "1";
                    store_7_1.loadData(common_data_YesOrNot);
                }
                if (combo_7.getValue() == "MANIFEST") {
                    combo_7_1.minChars = "1";
                    store_7_1.loadData(common_data_YesOrNot);
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
        anyMatch: true,
        // editable: false,
        queryMode: 'local',
        forceSelection: true,
        listeners:
         {
          change: function () {
              if (combo_7_1.getValue() == null) {
                  combo_7_1.reset();
              }
          }
         }
    });
    var condition7 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        items: [combo_7, combo_7_1]
    }
    var store_8 = Ext.create("Ext.data.JsonStore", {
        fields: ["CODE", "NAME"],
        data: search_js_condition4_StationField
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
        //value: 'ACCEPTTIME',
        editable: false
    })
    var date_8_1 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION8_1',
        margin: 0,
        flex: .325,
        emptyText: '开始日期',
        format: 'Y-m-d',
        listeners: {
            //focus: function (cb) {
            //    var con = Ext.Date.format(Ext.getCmp("CONDITION8_2").getValue(), 'Y-m-d');
            //    if (con != "") {
            //        var da_before = new Date(con);
            //        da_before.setFullYear(da_before.getFullYear() - 1);
            //        da_before.setDate(da_before.getDate() + 1);
            //        var da = new Date(con);

            //        cb.setMinValue(da_before); cb.setMaxValue(da);
            //    } else {
            //        //cb.reset();//没用
            //        cb.setMinValue(new Date("0000-01-01")); cb.setMaxValue(new Date("9999-12-31"));
            //    }
            //}
        }
    })
    var date_8_2 = Ext.create('Ext.form.field.Date', {
        id: 'CONDITION8_2',
        margin: 0,
        flex: .325,
        emptyText: '结束日期',
        format: 'Y-m-d',
        listeners: {
            //focus: function (cb) {
            //    var con = Ext.Date.format(Ext.getCmp("CONDITION8_1").getValue(), 'Y-m-d');
            //    if (con != "") {
            //        var da_after = new Date(con);
            //        da_after.setFullYear(da_after.getFullYear() + 1);
            //        da_after.setDate(da_after.getDate() - 1);
            //        var da = new Date(con);

            //        cb.setMinValue(da); cb.setMaxValue(da_after);
            //    } else {
            //        //cb.reset();//没用
            //        cb.setMinValue(new Date("0000-01-01")); cb.setMaxValue(new Date("9999-12-31"));
            //    }
            //}
        }
    })
    var condition8 = {
        xtype: 'fieldcontainer',
        layout: 'hbox',
        columnWidth: .28,
        items: [combo_8, date_8_1, date_8_2]
    }
    formpanel = Ext.create('Ext.form.Panel', {
        id: 'formpanel_Query',
        renderTo: 'div_form',
        fieldDefaults: {
            margin: '5',
            columnWidth: 0.24
        },
        items: [
        { layout: 'column', border: 0, items: [condition1, condition2, condition3, condition4] },
        { layout: 'column', border: 0, items: [condition5, condition6, condition7, condition8] }
        ]
    });
    //加载当前登录用户的默认查询条件
    //Ext.Ajax.request({//对公共基础数据发起一次请求
    //    url: "/Common/loadquerysetting",
    //    success: function (response, option) {
    //        var result = Ext.decode(response.responseText);
    //        if (result.data) {
    //            formpanel.getForm().setValues(result.data);
    //        }

    //        //初始化时间控件
    //        var before = new Date();
    //        before.setDate(before.getDate() - 3);
    //        var beforeday = Ext.Date.format(before, 'm/d/Y');

    //        var today = Ext.Date.format(new Date(), 'm/d/Y');

    //        if (Ext.getCmp("CONDITION4").getValue() == "SUBMITTIME") {
    //            Ext.getCmp("CONDITION4_1").setValue(beforeday); Ext.getCmp("CONDITION4_2").setValue(today);
    //        }
    //        if (Ext.getCmp("CONDITION8").getValue() == "SUBMITTIME") {
    //            Ext.getCmp("CONDITION8_1").setValue(beforeday); Ext.getCmp("CONDITION8_2").setValue(today);
    //        }
    //    }
    //});
}

//function initQueryCondition() {
//    CONDITION2.setValue("PORTCODE");
//    CONDITION6.setValue("TRADEWAY");

//    CONDITION3.setValue("STATUS");
//    CONDITION7.setValue("INSPFLAG");
//}

function Reset() {

    Ext.getCmp('CONDITION1').setValue("CUSNO");
    Ext.getCmp('CONDITION2').setValue("PORTCODE");
    Ext.getCmp('CONDITION3').setValue("STATUS");
    Ext.getCmp('CONDITION4').setValue("SUBMITTIME");

    Ext.getCmp('CONDITION5').setValue("DIVIDENO");
    Ext.getCmp('CONDITION6').setValue("TRADEWAY");
    Ext.getCmp('CONDITION7').setValue("INSPFLAG");
    Ext.getCmp('CONDITION8').setValue("ACCEPTTIME");

    var startTime = new Date();
    startTime.setDate(startTime.getDate() - 7);
    Ext.getCmp('CONDITION1_1').setValue("");
    Ext.getCmp('CONDITION2_1').setValue("");
    Ext.getCmp('CONDITION3_1').setValue("");
    Ext.getCmp('CONDITION4_1').setValue(startTime);//
    Ext.getCmp('CONDITION4_2').setValue(new Date());
    Ext.getCmp('CONDITION5_1').setValue("");
    Ext.getCmp('CONDITION6_1').setValue("");
    Ext.getCmp('CONDITION7_1').setValue("");
    Ext.getCmp('CONDITION8_1').setValue("");
    Ext.getCmp('CONDITION8_2').setValue("");
}

function SaveDefault() {
    var data = formpanel.getForm().getValues();
    Ext.Ajax.request({
        url: "/Common/SaveQuerySetting",
        params: { formdata: Ext.encode(data) },
        success: function (option, success, response) {
            if (option.responseText == '{success:true}') {
                Ext.MessageBox.alert('提示', '保存成功！');
            }
            else {
                Ext.MessageBox.alert('提示', '保存失败！');
            }
        }
    });
}

function del() {
    var sel = gridpanel.getSelectionModel().getSelection();
    if (sel.length == 0) {
        Ext.MessageBox.alert('提示', '请选择要删除的数据');
        return;
    }
    for (var i = 0; i < sel.length; i++)
    {
        var aaa = sel[i].data.STATUS;
        if (sel[i].data.STATUS != null && sel[i].data.STATUS != "已委托")
        {
            Ext.MessageBox.alert('提示','只能删除已委托的单子');
            return;
        }
    }
    var deleteData = Ext.encode(Ext.pluck(sel, 'data'));
    Ext.MessageBox.confirm("提示", "确定要删除所选择的数据吗？", function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: "/StationedFileld/DeleteOrder",
                params: { formdata: deleteData },
                success: function (response) {
                    var result = Ext.decode(response.responseText);
                    if (result.success) {
                        Ext.MessageBox.alert('提示', '删除成功！');
                        pgbar.moveFirst();
                    }
                    else {
                        Ext.MessageBox.alert('提示', result.msg);
                    }
                }
            });
        }
    });
}
