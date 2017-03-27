/*****************************************************win 窗口 begin ********************************************/
var repwayidcode;

function addwin(ID) {
    form_ini();
    var win = Ext.create("Ext.window.Window", {
        id: "wjcsWin",
        title: '委托信息',
        width: 820,
        height: 550,
        modal: true,
        items: [ Ext.getCmp('panelfile')],//Ext.getCmp('formpanel_u'),
    });
    win.show();
   // loadform(ID);
    upload_ini();
}

function form_ini() {
    var bbar_l = '<div class="btn-group">'
           + '<button type="button" class="btn btn-primary btn-sm" id="pickfiles"><i class="fa fa-upload"></i>&nbsp;上传文件</button>'
           + '<button type="button" onclick="browsefile()" class="btn btn-primary btn-sm"><i class="fa fa-exchange fa-fw"></i>&nbsp;浏览文件</button>'
           + '<button type="button" onclick="removeFile()" class="btn btn-primary btn-sm" id="deletefile"><i class="fa fa-trash-o"></i>&nbsp;删除文件</button>'
       + '</div>';
    var bbar_r = '<div class="btn-group">'
    + '<button type="button" onclick="save(\'按文件\')" class="btn btn-primary btn-sm" id="btn_saveorder"><i class="fa fa-floppy-o"></i>&nbsp;按文件逐个生成</button>'
    + '<button type="button" onclick="save(\'按批次\')" class="btn btn-primary btn-sm" id="btn_saveorder"><i class="fa fa-floppy-o"></i>&nbsp;按选择批次生成</button>'
    + '</div>';
    var bbar_m = '<span style="color:blue">说明：批次生成 CTRL+鼠标单击</span>';
    var buttombar = Ext.create('Ext.toolbar.Toolbar', {
        items: [bbar_l,bbar_m, '->', bbar_r]
    })


    var storefile = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'FILENAME', 'NEWNAME', 'ORIGINALNAME', 'UPLOADTIME', 'SIZES', 'FILETYPE']
    });

    var tmp = new Ext.XTemplate(
        '<tpl for=".">',
        '<div class="panel panel-default thumb-wrap fl" style="margin-top:5px;margin-left:5px;width:240px">',
        //'<div class="panel-heading" style="padding-left:5px;padding-right:5px">{[values.ORIGINALNAME.substr(0,23)]}<div class="fr"><span class="glyphicon glyphicon-paperclip"></span></div></div>',
        '<div class="panel-heading" style="padding-left:5px;padding-right:5px">{[values.ORIGINALNAME.substr(0,23)]}<div class="fr"><a href="/EnterpriseOrder/DownFile?filename={[values.FILENAME]}&ID={[values.ID]}" style="cursor:pointer"><span class="glyphicon glyphicon-download-alt"></span></a></div></div>',
        '<div class="panel-body" style="padding-left:5px;">',
        '<tpl>{[values.SIZES/1024 > 1024?Math.round(values.SIZES/(1024*1024))+"M":Math.round(values.SIZES/1024)+"K"]}</tpl>',
        '|{[values.UPLOADTIME]}</div></div>',
        '</tpl>'
        )
    var fileview = Ext.create('Ext.view.View', {
        id: 'fileview1',
        store: storefile,
        tpl: tmp,
        itemSelector: 'div.thumb-wrap',
        multiSelect: true
    })
    var panelfile = Ext.create('Ext.panel.Panel', {
        id: 'panelfile',
        tbar: buttombar,
        border: 0,
        width: '100%',
        minHeight: 500,
        items: [fileview]
    })
}

function upload_ini() {
    var uploader = new plupload.Uploader({
        runtimes: 'html5,flash,silverlight,html4',
        browse_button: 'pickfiles', // you can pass an id... 
        url: '/EnterpriseOrder/Upload_WebServer',
        flash_swf_url: '/js/upload/Moxie.swf',
        silverlight_xap_url: '/js/upload/Moxie.xap',
        unique_names: true,
        filters: {
            max_file_size: '500mb',
            mime_types: [
                { title: "Image files", extensions: "*" },
                { title: "Zip files", extensions: "zip,rar" }
            ]
        }
    });
    uploader.init();
    uploader.bind('FilesAdded', function (up, files) {
        uploader.start();
    });
    uploader.bind('FileUploaded', function (up, file) {
        var timestamp = Ext.Date.now();  //1351666679575  这个方法只是获取的时间戳
        var date = new Date(timestamp);
        Ext.getCmp('fileview1').store.insert(Ext.getCmp('fileview1').store.data.length
            , { FILENAME: '/FileUpload/file/' + file.target_name, NEWNAME: file.target_name, ORIGINALNAME: file.name, SIZES: file.size, UPLOADTIME: Ext.Date.format(date, 'Y-m-d H:i:s'),FILETYPE:44});
    });
}

//删除文件
function removeFile() {
    var fileview = Ext.getCmp('fileview1');
    var records = fileview.getSelectionModel().getSelection();
    if (records.length == 0) {
        Ext.MessageBox.alert("提示", "请选择要删除的记录！");
        return
    }
    Ext.MessageBox.confirm('提示', '确定要删除选择的记录吗？', function (btn) {
        if (btn == 'yes') {
            var records = fileview.getSelectionModel().getSelection();
            fileview.store.remove(records);
        }
    })
}

function browsefile() {

    var fileview = Ext.getCmp('fileview1');
    var records = fileview.getSelectionModel().getSelection();
    if (records.length == 0) {
        Ext.MessageBox.alert("提示", "请选择要浏览的记录！");
        return
    }
    var win = Ext.create("Ext.window.Window", {
        title: "文件预览",
        width: 1000,
        height: 600,
        layout: "fit",
        modal: true,
        closeAction: "destroy",
        items: [{
            html: "<div id='fileViewDiv' style='height: 100%;width: 100%;'></div>"
        }]
    });
    win.show();
    if (records[0].get("ID")) {
        document.getElementById('fileViewDiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="' + common_data_adminurl + '\/file' + records[0].get("FILENAME") + '"></embed>';
    }
    else {
        document.getElementById('fileViewDiv').innerHTML = '<embed id="pdf" width="100%" height="100%" src="' + records[0].get("FILENAME") + '"></embed>';
    }
}

function save(action) {
        if (Ext.getCmp('fileview1').store.data.items.length == 0) { //如果是提交,必须上传文件
            Ext.MessageBox.alert('提示', '随附文件不能为空！');
            return;
        }
        var filedata = Ext.encode(Ext.pluck(Ext.getCmp('fileview1').store.data.items, 'data'));
        if (action=="按批次") {
            var fileview = Ext.getCmp('fileview1');
            var records = fileview.getSelectionModel().getSelection();
            if (records.length == 0) {
                Ext.MessageBox.alert("提示", "请勾选要生成的文件记录，可按住CTRL键多选！");
                return
            } else {
                filedata = Ext.encode(Ext.pluck(records, 'data'));
                Ext.getCmp('fileview1').store.remove(records);

            }
        }
        
        var mask = new Ext.LoadMask(Ext.getBody(), { msg: "数据保存中，请稍等..." });
        mask.show();
        Ext.Ajax.request({
            url: "/EnterpriseOrder/Save",
            params: { action: action, filedata: filedata },
            success: function (response, option) {
                mask.hide();
                var data = Ext.decode(response.responseText);
                if (data.success) {
                    Ext.MessageBox.alert("提示", "保存成功！", function () {
                        if(action=="按文件")
                        {
                         Ext.getCmp('wjcsWin').close();
                        }
                          Ext.getCmp('pgbar').moveFirst();
                       
                    });
                }
                else {
                    Ext.MessageBox.alert("提示", "保存失败！");
                }
            }

        });
    
}

/*****************************************************win 窗口 end ********************************************/