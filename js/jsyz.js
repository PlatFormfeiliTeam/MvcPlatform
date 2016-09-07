Ext.apply(Ext.form.VTypes, {
    phoneMobile: function (val, field) {
        var phone = /^0\d{2,3}-\d{7,8}$/;
        var mobile = /^1[3|4|5|7|8]\d{9}$/;
        return (phone.test(val)) || (mobile.test(val));
    },
    phoneMobileText: '电话号码格式不对',
    double: function (val, field) {
        var reg = /^[-\+]?\d+(\.\d+)?$/;
        return (reg.test(val));
    },
    doubleText: '输入的重量格式不正确',
    certainlength11: function (val, field) {
        var reg = /^.{11}$/;
        return (reg.test(val));
    },
    certainlength11Text: '请输入11位字符！',
    certainlength18: function (val, field) {
        var reg = /^.{18}$/;
        return (reg.test(val));
    },
    certainlength18Text: '请输入18位字符！',
    gwnwrule: function (val, field) {
        var mz = Ext.getCmp('GOODSGW').getValue();
        var jz = Ext.getCmp('GOODSNW').getValue();
        if (!Ext.isEmpty(mz) && !Ext.isEmpty(jz) && Number(jz) > Number(mz)) {
            //Ext.MessageBox.alert('提示', '净重小于等于毛重！');
            return false;
        } else {
            return true;
        }
    },
    gwnwruleText: '净重小于等于毛重！',
    gwnwrule1: function (val, field) {
        var mz = Ext.getCmp('GOODSGW1').getValue();
        var jz = Ext.getCmp('GOODSNW1').getValue();
        if (!Ext.isEmpty(mz) && !Ext.isEmpty(jz) && Number(jz) > Number(mz)) {
            //Ext.MessageBox.alert('提示', '净重小于等于毛重！');
            return false;
        } else {
            return true;
        }
    },
    gwnwrule1Text: '净重小于等于毛重！',

    //空运总单号特殊验证：第11位等于4到10位跟7的取余
    kyzdhcertainlength11: function (val, field) {
        var reg = /^.{11}$/;
        var reg2 = /^[0-9]*$/;
        if (reg.test(val) && reg2.test(val)) {
            if ((val.substr(3, 7) % 7) == val.substr(10, 1)) {
                return true;
            }
        }
        return false;
    },
    kyzdhcertainlength11Text: '请输入正确的11位字符！',

    certainlength16: function (val, field) {
        var reg = /^.{16}$/;
        return (reg.test(val));
    },
    certainlength16Text: '请输入16位字符！'


})
