mui.init();
function InitAth(frmData, gf) {
    var aths = frmData.Sys_FrmAttachment;
    var ath;
    for (var i = 0; i < aths.length; i++) {
        if (aths[i].MyPK == gf.CtrlID) {
            ath = aths[i];
            break;
        }
    }
    if (ath == null) {
        return "";
    }



    ////判断是否设置了附件权限
    //var node = frmData.WF_Node[0];
    //var frmNode = frmData["FrmNode"];
    //if (node.FormType == 11 && frmNode != null) {
    //    frmNode = frmNode[0];
    //    if (frmNode.FrmSln == 1) {
    //        pageData.IsReadOnly = 1;
    //    }
    //    //自定义权限
    //    if (frmNode.FrmSln == 2) {
    //        var myPk = ath.MyPk + "_" + node.No;
    //        var NodeAth = new Entity("BP.Sys.FrmAttachment", myPk);
    //        var count = NodeAth.RetrieveFromDBSources();
    //        if (count != 0)
    //            ath = NodeAth;
    //    }
    //}


    var athDBs = frmData.Sys_FrmAttachmentDB;

    if (ath.IsVisable == false) {
        if (GetPara(ath.AtPara, "IsShowMobile") == "1") {
            //说明这是字段附件，根据字段的属性设置
            var attr = GetMapAttr(ath.MyPK);
            if (attr.UIVisible == 0)
                return "";
        } else
            return "";
    }


    var html = "";
    html += "<div class=\"mui-table-view-divider\"><h5 style='color:black;'>" + gf.Lab + "</h5></div>";

    html += '<div id="feedback" class="feedback" style="padding-left:5px;padding-right:5px">';
    html += "<div id='image-list-" + gf.CtrlID + "' name='image-list' class='row image-list'></div>";
    html += "</div>";
    return html;

}

function GetMapAttr(myPK) {
    var attrs = frmData.Sys_MapAttr;
    for (var i = 0; i < attrs.length; i++) {
        if (attrs[i].MyPK == myPK)
            return attrs[i];
    }
}

var IsReadonly;
//获取上传的文件集合
function GetAllAttachments(ath) {
    var jsonString;
    IsReadonly = GetQueryString("IsReadonly");
    //获取上传的附件文件
    //    var CCForm = url.substring(0, url.lastIndexOf('/') + 1) + "CCForm/ProcessRequest.do";
    var FK_FrmAttachment = ath.MyPK;
    var FK_MapData = ath.FK_MapData;
    //    var noOfObj = athPK.substr(athPK.lastIndexOf("_") + 1);
    var href = GetHrefUrl();
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    var FK_Flow = GetQueryString("FK_Flow");
    var FID = GetQueryString("FID");
    var FK_Node = GetQueryString("FK_Node");
    var NodeID = GetQueryString("NodeID");
    var WorkID = GetQueryString("WorkID");
    //获取上传附件列表的信息及权限信息
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("FK_FrmAttachment", FK_FrmAttachment);
    handler.AddPara("FK_MapData", FK_MapData);
    handler.AddPara("FK_Flow", FK_Flow);
    handler.AddPara("FID", FID);
    handler.AddPara("PKVal", WorkID);
    handler.AddPara("FK_Node", FK_Node);
    handler.AddPara("NodeID", NodeID);
    handler.AddPara("WorkID", WorkID);
    var data = handler.DoMethodReturnString("Ath_Init");
    if (data.indexOf('err@') == 0) {
        mui.alert(data);
        return;
    }


    data = JSON.parse(data);

    var athDesc = data["AthDesc"][0];

    var dbs = data["DBAths"];

    return dbs;
}

//判断上传文件的后缀名是否是Img类型
function IsImgeExt(ext) {
    ext = ext.replace(".", "").toLowerCase();
    switch (ext) {
        case "gif":
        case "jpg":
        case "jepg":
        case "jpeg":
        case "bmp":
        case "png":
        case "tif":
        case "gsp":
        case "mov":
        case "psd":
        case "tiff":
        case "wmf":
            return true;
        default:
            return false;
    }

}
//上传文件
function uploadFile(fileObj, FK_FrmAttachment) {
    //form表单序列话

    var parasData = $("form").serialize();
    var formData = new FormData();
    var name = $("input").val();
    formData.append("file", fileObj);
    formData.append("name", name);
    var pkval = pageData.WorkID;
    var WorkID = pageData.WorkID;

    var doMethod = "MoreAttach";
    var httpHandlerName = "BP.WF.HttpHandler.WF_CCForm";
    if (plant == 'CCFlow') {
        Url = dynamicHandler + "?DoType=HttpHandler&DoMethod=" + doMethod + "&HttpHandlerName=" + httpHandlerName + "&FK_FrmAttachment=" + FK_FrmAttachment + "&WorkID=" + WorkID + "&PKVal=" + pkval + "&AttachPK=" + FK_FrmAttachment + "&t=" + new Date().getTime();
    } else {
        var currentPath = GetHrefUrl();
        var path = currentPath.substring(0, currentPath.indexOf('/CCMobile') + 1);
        var Url = path + "WF/Ath/AttachmentUploadS.do?FK_FrmAttachment=" + FK_FrmAttachment + "&PKVal=" + pkval;
    }




    var IsUpSuccess = false;
    $.ajax({
        url: Url,
        type: 'POST',
        data: formData,
        async: false,
        // 告诉jQuery不要去处理发送的数据
        processData: false,
        // 告诉jQuery不要去设置Content-Type请求头
        contentType: false,
        beforeSend: function () {
            console.log("正在进行，请稍候");
        },
        success: function (responseStr) {
            IsUpSuccess = true;
        },
        error: function (responseStr) {

        }
    });
    return IsUpSuccess;
}

//文件下载
function downLoad(mypk) {
    if (plant == "CCFlow")
        var Url = 'CCForm/DownFile.aspx?DoType=Down&MyPK=' + mypk + '&PKVal=' + mypk;
    else {
        //按照数据流模式下载。
        var currentPath = GetHrefUrl();
        var path = currentPath.substring(0, currentPath.indexOf('/CCMobile') + 1);
        var Url = path + "WF/Ath/downLoad.do?MyPK=" + mypk + "&PKVal=" + mypk;
    }

    SetHref(Url);
    //    return Url;
    //    //按照数据流模式下载。
    //    var currentPath = GetHrefUrl();
    //    var path = currentPath.substring(0, currentPath.indexOf('/CCMobile') + 1);
    //    var Url = path + "/CCMobile/CCForm/downloadFile.do?MyPK=" + mypk + "&PKVal=" + mypk;
    //window.location.href = filterXSS(url);
}

//文件下载
function GetFileStream(mypk) {
    if (plant == "CCFlow") {
        var Url = 'CCForm/DownFile.aspx?DoType=Down&MyPK=' + mypk + '&PKVal=' + mypk;
    } else {
        //按照数据流模式下载。
        var currentPath = GetHrefUrl();
        var path = currentPath.substring(0, currentPath.indexOf('/CCMobile') + 1);
        var Url = path + "WF/Ath/downLoad.do?MyPK=" + mypk + "&PKVal=" + mypk;
    }

    return Url;
}