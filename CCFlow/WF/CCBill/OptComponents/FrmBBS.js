
new Vue({
    el: '#FrmBBSlist',
    data: {
        flowNodes: [],
        expandAll: false,
        loadingDialog: false,
        webuser: '',
        isReadonly:false
    },
    methods: {
        ///保存.
        Save: function () {
            var input = $("#reply-input")
            var en = new Entity("BP.CCBill.FrmBBS");
            en.Name = input.val();
            en.WorkID = GetQueryString("WorkID");
            en.FrmID = GetQueryString("FrmID");
            en.ParentNo = "0";
            en.Insert();

            input.val('');
            layer.msg("回复成功", { time: 1000 }, function () {
                window.location.reload();
            });
        },
        ///答复.
      
        Delete: function(id) {
        var en = new Entity("BP.CCBill.FrmBBS", id);
            en.Delete();
            layer.msg("删除成功", { time: 1000 }, function () {
                window.location.reload();
            });
        },
        Repay: function (index,No) {
            var noid = No;
            var tag = "<textarea id = 'Retext" + index +"' style = 'height: 120px; width: 100%;' placeholder = '期待您的回复！' spellcheck = 'false' ></textarea>";
            tag += "<a onclick='SaveAsReply(" + index + ")' id='Renum" + index + "' data-noid=" + No +"  class='layui-btn layui-btn-sm top10'>回复</a>";
            tag += "<a onclick='Closeq(" + index +")'  class='layui-btn layui-btn-primary layui-btn-sm top10'>取消回复</a>";
            tag += "<a  onclick='UploadFile(" + index +")' class='layui-btn layui-btn-primary layui-btn-sm top10'>上传附件</a>";
            tag += "<input id='" + index + "' name='" + index+"' type='file' style='visibility: hidden'>"
            var num = index
            var a = $('.huifu').eq(num).html(tag);
          
        },
        downloadFile: function (frmBBS) {
            var path = frmBBS.MyFilePath;
            path = basePath + "/" + path.substr(path.indexOf("DataUser"), path.length);
            var link = document.createElement('a');
            link.setAttribute("download", "");
            link.href = path;
            link.click();

            var x = new XMLHttpRequest();
            x.open("GET", url, true);
            x.responseType = 'blob';
            x.onload = function (e) { download(x.response, frmBBS.MyFileName , "image/gif"); }
            x.send();

        }
    },
    mounted: function () {

        // fix firefox bug
        document.body.ondrop = function (event) {
            event.preventDefault();
            event.stopPropagation();
        }


        var workID = GetQueryString("WorkID");
        var frmID = GetQueryString("FrmID");

        //查询出来数据.
        var ens = new Entities("BP.CCBill.FrmBBSs");
        ens.Retrieve("WorkID", workID, "ParentNo",0);
        systems = obj2arr(ens)
        for (var i = 0; i < systems.length; i++) {
            var en = systems[i];
            
            en.children = [];
            var ensk = new Entities("BP.CCBill.FrmBBSs");
            ensk.Retrieve("WorkID", workID, 'ParentNo', en.No);
            childModules = obj2arr(ensk)
            console.log(workID);
            en.children = childModules
        }
        
        this.flowNodes = systems;
        var webUser = new WebUser();         
        this.webuser = webUser.Name;
        var isRead = GetQueryString("IsReadonly");
        if (isRead == null || isRead == undefined || isRead == "" || isRead == "0")
            this.isReadonly = false;
        else
            this.isReadonly = true;

    }
})

function Init() {

    var workID = GetQueryString("WorkID");
    var frmID = GetQueryString("FrmID");

    //查询出来数据.
    var ens = new Entities("BP.CCBill.FrmBBSs");
    ens.Retrieve("WorkID", workID, "RDT");
    
    for (var i = 0; i < ens.length; i++) {

        var en = ens[i];
        var rdt = en.RDT;
        var name = en.Name;
        var parnt = en.ParentNo;
    }

    //给头部赋值.
    var dictEn = new Entity(frmID, workID);
    $("#TB_No").html(dictEn.BillNo);
    $("#TB_Name").html(dictEn.Title);

}
function Closeq(index) {
    var num = index
    console.log(index);
    var a = $('.huifu').eq(num).html('');
}

function UploadFile(index) {
    document.getElementById(index+"").click();
}
function obj2arr(obj) {
    delete obj.Paras
    delete obj.ensName
    delete obj.length
    var arr = []
    for (var key in obj) {
        if (Object.hasOwnProperty.call(obj, key)) {
            arr.push(obj[key]);
        }
    }
    return arr
}


///答复.
function SaveAsReply(index) {
    parentNo = $('#Renum' + index).data('noid')
    var en = new Entity("BP.CCBill.FrmBBS");
    en.Name = $("#Retext" + index).val();
    console.log(en.Name);
    en.ParentNo = parentNo;
    en.WorkID = GetQueryString("WorkID");
    en.FrmID = GetQueryString("FrmID");
    en.Insert();
    var file = $("#"+index);
    var fileObj = file[0].files[0]; // js 获取文件对象
    if (typeof (fileObj) == "undefined") {
        layer.msg("回复成功", { time: 1000 }, function () {
            window.location.reload();
        });
        return;
    }
    var handler = new HttpHandler("BP.CCBill.WF_CCBill_OptComponents");
    handler.AddPara("file", fileObj);
    handler.AddPara("No", en.No);
    var data = handler.DoMethodReturnString("FrmBBs_UploadFile");
    if (data.indexOf("err@") != -1) {
        layer.alert(data);
        return;
    }
    layer.msg("回复成功", { time: 1000 }, function () {
        window.location.reload();
    });

}
