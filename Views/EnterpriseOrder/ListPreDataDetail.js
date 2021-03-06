﻿var cusno = getQueryString("CUSNO");
var flag_data = [{ "NAME": "未转化", "CODE": 0 }, { "NAME": "已转换", "CODE": 1 }, { "NAME": "转换错误", "CODE": 2 }];

Ext.onReady(function () {
    Ext.Ajax.request({
        url: "/EnterpriseOrder/loadPreDataDetail",
        params: { CUSNO: cusno },
        success: function (response, opts) {
            var data = Ext.decode(response.responseText);          
            
            predata(data);
            griddata(data);

            var panel = Ext.create('Ext.panel.Panel', {
                title: '报关单证录入',
                border: 0,
                layout: 'border',
                region: 'center',
                items: [Ext.getCmp("formpane_declare"), Ext.getCmp("grid")],
                //buttonAlign: 'center',
                buttons: [
                    {
                        text: '<i class="iconfont">&#xe633;</i>&nbsp;撤  回', id: 'btn_cancel', disabled: (data.declstatus == "0" && data.head_data.FLAG != "0")? false : true, handler: function () {
                            cancel();
                        }
                    }, '-',                   
                    {
                        text: '<i class="iconfont">&#xe628;</i>&nbsp;提  交', id: 'btn_submit', disabled: data.head_data.FLAG == "0" ? false : true, handler: function () {
                            save(cusno);
                        }
                    }
                ]
            });
            var viewport = new Ext.container.Viewport({
                layout: 'border',
                items: [panel]
            });

            $(window).unload(function () { window.opener.pgbar.moveFirst(); });
            
        }
    });
});

function predata(data) {
    
    var field_PORTNAME = Ext.create('Ext.form.field.Text', {
        name: 'PORTNAME',
        fieldLabel: '进/出口口岸',
        readOnly: true
    });
    var field_RECORDCODE = Ext.create('Ext.form.field.Text', {
        name: 'RECORDCODE',
        fieldLabel: '备案号',
        readOnly: true
    });
    var field_TRANSMODEL = Ext.create('Ext.form.field.Text', {
        name: 'TRANSMODEL',
        fieldLabel: '运输方式',
        readOnly: true
    });
    var field_TRANSNAME = Ext.create('Ext.form.field.Text', {
        name: 'TRANSNAME',
        fieldLabel: '运输工具名称',
        readOnly: true
    });
    var field_BLNO = Ext.create('Ext.form.field.Text', {
        name: 'BLNO',
        fieldLabel: '提运单号',
        readOnly: true
    });
    var field_BUSIUNITNAME = Ext.create('Ext.form.field.Text', {
        name: 'BUSIUNITNAME',
        fieldLabel: '收/发货单位',
        readOnly: true
    });
    var field_TRADENAME = Ext.create('Ext.form.field.Text', {
        name: 'TRADENAME',
        fieldLabel: '贸易方式',
        readOnly: true
    });
    var field_EXEMPTIONNAME = Ext.create('Ext.form.field.Text', {
        name: 'EXEMPTIONNAME',
        fieldLabel: '征免性质',
        readOnly: true
    });
    var field_LICENSENO = Ext.create('Ext.form.field.Text', {
        name: 'LICENSENO',
        fieldLabel: '许可证号',
        readOnly: true
    });
    var field_SECOUNTRYNAME = Ext.create('Ext.form.field.Text', {
        name: 'SECOUNTRYNAME',
        fieldLabel: '起运国/运抵国',
        readOnly: true
    });
    var field_SEPORTNAME = Ext.create('Ext.form.field.Text', {
        name: 'SEPORTNAME',
        fieldLabel: '装货港/指运港',
        readOnly: true
    });
    var field_SEPLACENAME = Ext.create('Ext.form.field.Text', {
        name: 'SEPLACENAME',
        fieldLabel: '目的地/货源地',
        readOnly: true
    });
    var field_APPROVALNO = Ext.create('Ext.form.field.Text', {
        name: 'APPROVALNO',
        fieldLabel: '批准文号',
        readOnly: true
    });

    var field_TRADETERMSNAME = Ext.create('Ext.form.field.Text', {
        name: 'TRADETERMSNAME',
        fieldLabel: '成交方式',
        readOnly: true
    });
    var field_FREIGHT = Ext.create('Ext.form.field.Text', {
        name: 'FREIGHT',
        fieldLabel: '运费',
        readOnly: true
    });
    var field_INSURANCEPREMIUM = Ext.create('Ext.form.field.Text', {
        name: 'INSURANCEPREMIUM',
        fieldLabel: '保费',
        readOnly: true
    });
    var field_CONTRACTNO = Ext.create('Ext.form.field.Text', {
        name: 'CONTRACTNO',
        fieldLabel: '合同协议号',
        readOnly: true
    });
    var field_GOODSNUM = Ext.create('Ext.form.field.Text', {
        name: 'GOODSNUM',
        fieldLabel: '件数',
        readOnly: true
    });
    var field_PACKAGENAME = Ext.create('Ext.form.field.Text', {
        name: 'PACKAGENAME',
        fieldLabel: '包装种类',
        readOnly: true
    });
    var field_GOODSGW = Ext.create('Ext.form.field.Text', {
        name: 'GOODSGW',
        fieldLabel: '毛重',
        readOnly: true
    });
    var field_GOODSNW = Ext.create('Ext.form.field.Text', {
        name: 'GOODSNW',
        fieldLabel: '净重',
        readOnly: true
    });
    var field_CONSHIPPERNAME = Ext.create('Ext.form.field.Text', {
        name: 'CONSHIPPERNAME',
        fieldLabel: '生产消费单位',
        readOnly: true
    });
    var field_DECLWAY = Ext.create('Ext.form.field.Text', {
        name: 'DECLWAY',
        fieldLabel: '报关方式',
        readOnly: true
    });
    var field_TRADECOUNTRYNAME = Ext.create('Ext.form.field.Text', {
        name: 'TRADECOUNTRYNAME',
        fieldLabel: '贸易国',
        readOnly: true
    });
    var field_CUSNO = Ext.create('Ext.form.field.Text', {
        id: 'field_CUSNO',
        name: 'CUSNO',
        fieldLabel: '企业编号',
        readOnly: true,
        fieldStyle: 'color: blue'
    });

    var store_flag = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: flag_data });
    var combo_flag = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_flag', name: 'FLAG', store: store_flag, fieldLabel: '转换状态',
        displayField: 'NAME', valueField: 'CODE', queryMode: 'local', editable: false, hiddenTrigger: true, readOnly: true,
        fieldStyle:'color: red'
    });

    var store_special = Ext.create('Ext.data.JsonStore', { fields: ['CODE', 'NAME'], data: [{ "NAME": "否", "CODE": 0 }, { "NAME": "是", "CODE": 1 }, { "NAME": "", "CODE": 9 }] });
    var combo_SPECIALRELATION = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_SPECIALRELATION', name: 'SPECIALRELATION', store: store_special, fieldLabel: '特殊关系',
        displayField: 'NAME', valueField: 'CODE', queryMode: 'local', editable: false, hiddenTrigger: true, readOnly: true
    });
    var combo_PRICEIMPACT = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_PRICEIMPACT', name: 'PRICEIMPACT', store: store_special, fieldLabel: '特殊价格',
        displayField: 'NAME', valueField: 'CODE', queryMode: 'local', editable: false, hiddenTrigger: true, readOnly: true
    });
    var combo_PAYPOYALTIES = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_PAYPOYALTIES', name: 'PAYPOYALTIES', store: store_special, fieldLabel: '特殊支付',
        displayField: 'NAME', valueField: 'CODE', queryMode: 'local', editable: false, hiddenTrigger: true, readOnly: true
    });

    var field_REMARK = Ext.create('Ext.form.field.Text', {
        name: 'REMARK',
        fieldLabel: '备注',
        readOnly: true
    });

    var field_INOUTTYPE = Ext.create('Ext.form.field.Text', {
        name: 'INOUTTYPE',
        fieldLabel: '申报类型',
        readOnly: true, fieldStyle: 'color: blue'
    });

    var formpane_declare = Ext.create('Ext.form.Panel', {
        id:'formpane_declare',
        region: 'north',
        height: 330,
        fieldDefaults: {
            margin: '5 5 10 0',
            labelWidth: 80,
            columnWidth: .25,
            labelSeparator: '',
            msgTarget: 'under',
            labelAlign: 'right',
            validateOnBlur: false,
            validateOnChange: false
        },
        items: [
                { layout: 'column', border: 0, items: [field_PORTNAME, field_RECORDCODE, field_TRANSMODEL, field_TRANSNAME] },
                { layout: 'column', border: 0, items: [field_BLNO, field_BUSIUNITNAME, field_TRADENAME, field_EXEMPTIONNAME] },
                { layout: 'column', border: 0, items: [field_LICENSENO, field_SECOUNTRYNAME, field_SEPORTNAME, field_SEPLACENAME] },
                { layout: 'column', border: 0, items: [field_TRADETERMSNAME, field_FREIGHT, field_INSURANCEPREMIUM, field_CONTRACTNO] },
                { layout: 'column', border: 0, items: [field_GOODSNUM, field_PACKAGENAME, field_GOODSGW, field_GOODSNW] },
                { layout: 'column', border: 0, items: [field_DECLWAY, field_INOUTTYPE, field_CUSNO, combo_flag] },
                { layout: 'column', border: 0, items: [field_CONSHIPPERNAME, combo_SPECIALRELATION, combo_PRICEIMPACT, combo_PAYPOYALTIES] },
                { layout: 'column', border: 0, items: [field_TRADECOUNTRYNAME, field_APPROVALNO, field_REMARK] }
        ]
    });
    Ext.getCmp("formpane_declare").getForm().setValues(data.head_data);
}

function griddata(data) {
    var store = Ext.create('Ext.data.JsonStore', {
        fields: ['ITEMNO','ORDERNO','COMMODITYNO','ADDITIONALNO','COMMODITYCHNAME','SPECIFICATIONSMODEL','GOODSNW'
                ,'CADQUANTITY','LEGALQUANTITY','SQUANTITY','CADUNITNAME','LEGALUNITNAME'
                ,'SUNITNAME','COUNTRYORIGINNAME','TOTALPRICE','UNITPRICE','CURRENCYNAME'
                ,'TAXPAIDNAME','DESTCOUNTRYNAME'],        
        data: data.sub_data
    })
    var grid = Ext.create('Ext.grid.Panel', {
        id:'grid',
        store: store,
        enableColumnHide: false,
        region: 'center',
        columns: [
        { xtype: 'rownumberer', width: 35 },
        { header: '序号', dataIndex: 'ORDERNO', width: 40, locked: true },
        { header: '项号', dataIndex: 'ITEMNO', width: 40, locked: true },
        { header: '商品编号', dataIndex: 'COMMODITYNO', width: 110, locked: true },
        { header: '附加编码', dataIndex: 'ADDITIONALNO', width: 60, locked: true },
        { header: '商品名称', dataIndex: 'COMMODITYCHNAME', width: 200, locked: true },
        { header: '商品规格', dataIndex: 'SPECIFICATIONSMODEL', width: 130 },
        { header: '净重', dataIndex: 'GOODSNW', width: 80 },
        { header: '成交数量', dataIndex: 'CADQUANTITY', width: 60 },
        { header: '成交单位', dataIndex: 'CADUNITNAME', width: 60 },
        { header: '法定数量', dataIndex: 'LEGALQUANTITY', width: 60 },
        { header: '法定单位', dataIndex: 'LEGALUNITNAME', width: 60 },
        { header: '法二数量', dataIndex: 'SQUANTITY', width: 60 },
        { header: '法二单位', dataIndex: 'SUNITNAME', width: 60 },  
        { header: '单价', dataIndex: 'UNITPRICE', width: 80 },
        { header: '总价', dataIndex: 'TOTALPRICE', width: 100 },
        { header: '币制', dataIndex: 'CURRENCYNAME', width: 40 },
        { header: '征免方式', dataIndex: 'TAXPAIDNAME', width: 100 },
        { header: '原产国', dataIndex: 'COUNTRYORIGINNAME', width: 100 },
        { header: '目的国', dataIndex: 'DESTCOUNTRYNAME', width: 100 }
        ]
    });
}

function cancel() {
    var mask = new Ext.LoadMask(Ext.get(Ext.getBody()), { msg: "撤回中，请稍等..." });

   
    Ext.MessageBox.confirm("提示", "确定要执行撤回操作吗？", function (btn) {
        if (btn == "yes") {
            mask.show();
            Ext.Ajax.request({
                url: '/EnterpriseOrder/cancelPreData',
                params: { cusno: Ext.getCmp("field_CUSNO").getValue() },
                success: function (response, options) {
                    mask.hide();
                    var json = Ext.decode(response.responseText);
                    if (json.success == true) {
                        Ext.MessageBox.alert("提示", "撤单成功！");
                        Ext.getCmp("combo_flag").setValue(0);
                        Ext.getCmp("btn_cancel").setDisabled(true);
                        Ext.getCmp("btn_submit").setDisabled(Ext.getCmp("combo_flag").getValue() == "0" ? false : true);
                    }
                    else {
                        Ext.MessageBox.alert("提示", "撤单失败：" + json.error);
                    }
                }
            });
        }
    });
}

function save(cusno) {
    
    var mask = new Ext.LoadMask(Ext.get(Ext.getBody()), { msg: "提交中，请稍等..." });

    mask.show();
    Ext.Ajax.request({
        url: "/EnterpriseOrder/SavePreDataToPrd",
        params: { cusno: cusno },
        success: function (response, option) {
            if (response.responseText) {
                mask.hide();
                var data = Ext.decode(response.responseText);
                if (data.success) {
                    Ext.MessageBox.alert("提示","提交成功！", function () {
                        Ext.getCmp("combo_flag").setValue(1);
                        Ext.getCmp("btn_cancel").setDisabled(false);
                        Ext.getCmp("btn_submit").setDisabled(true);
                    });
                }
                else {
                    Ext.MessageBox.alert("提示", "提交失败:" + data.error);
                }
            }
        }
    });
}