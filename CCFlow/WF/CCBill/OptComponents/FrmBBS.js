
var myvue=new Vue({
    el: '#FrmBBSlist',
    data: {
        flowNodes: [],
        expandAll: false,
        loadingDialog: false,
        webuser: '',
        workID: null,
        frmID: null,
        isReadonly:false
    },
    methods: {
        ///保存.
        Save: function () {
            var input = $("#reply-input")
            var en = new Entity("BP.CCBill.FrmBBS");
            en.Name = input.val();
            en.WorkID = this.workID; 
            en.FrmID = GetQueryString("FrmID");
            en.ParentNo = "0";
            en.Insert();

            input.val('');
            layer.msg("提交成功", { time: 1000 }, function () {
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
            var tag = "<textarea id = 'Retext" + index +"' style = 'height: 40px; width: 100%;' placeholder = '期待您的回复！' spellcheck = 'false' ></textarea>";
            tag += "<a onclick='SaveAsReply(" + index + ")' id='Renum" + index + "' data-noid=" + No +"  class='layui-btn layui-btn-sm top10'>回复</a>";
            tag += "<a onclick='Closeq(" + index +")'  class='layui-btn layui-btn-primary layui-btn-sm top10'>取消</a>";
            tag += "<a  onclick='UploadFile(" + index +")' class='layui-btn layui-btn-primary layui-btn-sm top10'>上传附件</a>";
            tag += "<input id='" + index + "' name='" + index + "' type='file' onchange='fileChange(event," + index + ")' style='visibility: hidden'>"
            var num = index
            var a = $('.huifu').eq(num).html(tag);
          
        },
        downloadFile: function (frmBBS) {
            var path = frmBBS.MyFilePath;
            path = basePath + "/" + path.substr(path.indexOf("DataUser"), path.length);
            SetHref(path);

        }
    },
    mounted: function () {

        // fix firefox bug
        document.body.ondrop = function (event) {
            event.preventDefault();
            event.stopPropagation();
        }

          this.workID = GetQueryString("WorkID");
        if (this.workID == null)
            this.workID = GetQueryString("No");
        if (this.workID == null)
            this.workID = GetQueryString("MyPK");



        var frmID = GetQueryString("FrmID");

        //查询出来数据.
        var ens = new Entities("BP.CCBill.FrmBBSs");
        ens.Retrieve("WorkID", this.workID, "ParentNo", 0);

        systems = obj2arr(ens)
        for (var i = 0; i < systems.length; i++) {
            var en = systems[i];
            
            en.children = [];
            var ensk = new Entities("BP.CCBill.FrmBBSs");
            ensk.Retrieve("WorkID", this.workID, 'ParentNo', en.No);
            childModules = obj2arr(ensk)
          //  console.log(workID);
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
 
function Closeq(index) {
    var num = index
    console.log(index);
    var a = $('.huifu').eq(num).html('');
}

function fileChange(e, index) {
    var e = e || window.event;
    //获取 文件 个数 取消的时候使用
    var files = e.target.files;
    if (files.length > 0) {
        SaveAsReply(index);
    }
}
function UploadFile(index) {
    document.getElementById(index).click();
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
    en.WorkID = myvue.workID; 
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
