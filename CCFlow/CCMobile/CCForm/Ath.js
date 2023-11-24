/*****
 * 移动端附件信息显示使用的JS方法
 */
mui.init();
/**
 * 附件信息初始化
 * @param {any} frmData 主表表单集合
 * @param {any} gf 分组信息
 * @param {any} isZDMobile 是否折叠
 * @param {any} dtlData 从表表单信息
 * @param {any} type 类别 0 主表 1从表
 * @param {any} refPKVal
 */
function InitAth(frmData, gf, isZDMobile, dtlData, type, refPKVal) {
    dtlData = dtlData || {};
    type = type || 0;
    refPKVal = refPKVal || pageData.WorkID;
    var aths = frmData.Sys_FrmAttachment;
    var ath;
    if (type == 0) {
        $.each(frmData.Sys_FrmAttachment, function (i, item) {
            if (item.MyPK == gf.CtrlID) {
                ath = item;
                return false;
            }
        })
    }
    if (type == 1) {
        $.each(dtlData.Sys_FrmAttachment, function (i, item) {
            if (item.MyPK == gf.CtrlID) {
                ath = item;
                return false;
            }
        })
    }
    if (ath == null || ath == undefined)
        return "";

    var isShowMobile = GetPara(ath.AtPara, "IsShowMobile")||"1";
    if (isShowMobile == "0")
        return "";


    //判断是否设置了附件权限
    var attrMyPK = ath.MyPK;
    if (frmData.WF_Node != null && frmData.WF_Node != undefined) {
        var node = frmData.WF_Node[0];
        var frmNode = frmData["WF_FrmNode"];
        if (frmNode != null && (node.FormType == 11 || node.FormType == 5 || (frmData.WF_Flow != undefined && frmData.WF_Flow[0].FlowDevModel == 1))) {
            frmNode = frmNode[0];
            if (frmNode.FrmSln == 1) {
                pageData.IsReadOnly = 1;
            }
            //自定义权限
            if (frmNode.FrmSln == 2) {
                var myPK = ath.MyPK + "_" + node.NodeID;
                if (type == 1)
                    myPK = ath.FK_MapData + "_" + node.NodeID + "_AthMDtl";
                var nodeAth = new Entity("BP.Sys.FrmAttachment");
                nodeAth.SetPKVal(myPK);
                var count = nodeAth.RetrieveFromDBSources();
                if (count != 0) {
                    ath = nodeAth;
                }
            }
        }
    }

    var athDBs = [];
    if (type == 1 || dtlData)
        athDBs = dtlData.Sys_FrmAttachmentDB;
    else
        athDBs = frmData.Sys_FrmAttachmentDB;

    if (ath.IsVisable == false && type == 0) {
        if (GetPara(ath.AtPara, "IsShowMobile") == "1") {
            //说明这是字段附件，根据字段的属性设置
            var attr = GetMapAttr(attrMyPK);
            if (attr != undefined && attr.UIVisible == 0)
                return "";
        } else
            return "";
    }


    var html = "";
    if (isZDMobile == false) {
        html += "<div class=\"mui-table-view-divider\"><h5 style='color:black;'>" + gf.Lab + "</h5></div>";

        html += '<div id="feedback" class="feedback" style="padding-left:5px;padding-right:5px">';
        if (type == 1)
            html += "<div id='image-list-" + ath.MyPK + "_" + refPKVal + "' data-refpk='" + refPKVal + "' name='image-list' class='row image-list'></div>";
        else
            html += "<div id='image-list-" + ath.MyPK + "' data-refpk='" + refPKVal + "' name='image-list' class='row image-list'></div>";

        html += "</div>";
    }
    if (isZDMobile == true) {
        html += "<li class='mui-table-view-cell mui-collapse mui-active'><a class='mui-navigate-right' href='#'>" + gf.Lab + "</a>";
        html += "<div class='mui-collapse-content' style='margin-right:-65px'>";
        html += '<div id="feedback" class="feedback" style="padding-left:5px;padding-right:5px">';
        if (type == 1)
            html += "<div id='image-list-" + ath.MyPK + "_" + refPKVal + "' data-refpk='" + refPKVal + "' name='image-list' class='row image-list'></div>";
        else
            html += "<div id='image-list-" + ath.MyPK + "' data-refpk='" + refPKVal + "' name='image-list' class='row image-list'></div>";
        html += "</div>";
        html += "</div>";
        html += "</li>";
    }

    return html;

}

function InitEleAth(frmData, gf, fk_Mapdata, keyOfEn) {
    var frmImgs = $.grep(frmData.Sys_FrmImgAth, function (item, i) {
        if (item.FK_MapData == fk_Mapdata && item.CtrlID == keyOfEn)
            return item;
    });
    if (frmImgs[0] == null) {
        return "";
    }
    var ath = frmImgs[0];

    var isShowMobile = "1";
    if (isShowMobile == null || isShowMobile == "" || isShowMobile == undefined || isShowMobile == "0")
        return "";


    //判断是否设置了附件权限
    var attrMyPK = ath.MyPK;
    if (frmData.WF_Node != null && frmData.WF_Node != undefined) {
        var node = frmData.WF_Node[0];
        var frmNode = frmData["WF_FrmNode"];
        if (frmNode != null && (node.FormType == 11 || node.FormType == 5 || (frmData.WF_Flow != undefined && frmData.WF_Flow[0].FlowDevModel == 1))) {
            frmNode = frmNode[0];
            if (frmNode.FrmSln == 1) {
                pageData.IsReadOnly = 1;
            }
            //自定义权限
            if (frmNode.FrmSln == 2) {
                var myPK = ath.MyPK + "_" + node.NodeID;
                var nodeAth = new Entity("BP.Sys.FrmUI.FrmImgAth");
                nodeAth.SetPKVal(myPK);
                var count = nodeAth.RetrieveFromDBSources();
                if (count != 0) {
                    ath = nodeAth;

                }

            }
        }
    }



    var athDBs = frmData.Sys_FrmImgAthDB;

    if (ath.IsVisable == false) {
        if (GetPara(ath.AtPara, "IsShowMobile") == "1") {
            //说明这是字段附件，根据字段的属性设置
            var attr = GetMapAttr(attrMyPK);
            if (attr != undefined && attr.UIVisible == 0)
                return "";
        } else
            return "";
    }


    var html = "";
    //html += "<div class=\"mui-table-view-divider\"><h5 style='color:black;'>" + gf.Lab + "</h5></div>";

    html += '<div id="feedback" class="feedback" style="padding-left:5px;padding-right:5px">';

    html += "<div id='image-list-" + ath.MyPK + "'data-refpk='" + pageData.WorkID + "' name='img-list' class='row image-list'></div>";
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
function GetAllAttachments(ath, imgId) {
    var jsonString;
    IsReadonly = GetQueryString("IsReadonly");
    //获取上传的附件文件
    var FK_FrmAttachment = ath.MyPK;
    var FK_MapData = ath.FK_MapData;
    var FK_Flow = pageData.FK_Flow;
    var FID = pageData.FID;
    var FK_Node = pageData.FK_Node;
    var NodeID = GetQueryString("NodeID");
    var refPKVal = $("#" + imgId).attr("data-refpk") || pageData.WorkID;
    //获取上传附件列表的信息及权限信息
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("FK_FrmAttachment", FK_FrmAttachment);
    handler.AddPara("FK_MapData", FK_MapData);
    handler.AddPara("FK_Flow", FK_Flow);
    handler.AddPara("FID", FID);
    handler.AddPara("PKVal", refPKVal);
    handler.AddPara("FK_Node", FK_Node);
    handler.AddPara("NodeID", NodeID);
    handler.AddPara("WorkID", refPKVal);
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
function uploadFile(fileObj, FK_FrmAttachment, imgId) {
    //form表单序列话

    var parasData = $("form").serialize();
    var formData = new FormData();
    var name = $("input").val();
    for (var i = 0; i < fileObj.length; i++) {
        if (fileObj[i] != null) {
            formData.append("file", fileObj[i]);
            //numOfAths++;
            ////判断附件上传最大数量
            //if (topNumOfUpload < numOfAths) {
            //    alert("您最多上传[" + topNumOfUpload + "]个附件");
            //    return;
            //}
        }
    }
    formData.append("name", name);
    var refPKVal = $("#" + imgId).attr("data-refpk") || pageData.WorkID;
    var pkval = refPKVal;
    var WorkID = refPKVal;

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
            console.log(responseStr);
            if (responseStr.status == 200)
                IsUpSuccess = true;
        }
    });
    return IsUpSuccess;
}
function previewShow(src) {

    $('#preview img').attr("src", src);
    $('#preview').show();
}
//文件下载
function downLoad(mypk, filePath) {
    const _src = $(`#${mypk}`).css('backgroundImage').split("(")[1].split(")")[0].replace(/\"/g, "");//获取当前点击的pimg元素中的src属性
    console.log(`🚀 :: _src`, _src);
    const srcArr = _src.split('/');
    var Url = "";
    if (plant == "CCFlow")
        Url = basePath + "/WF/Comm/ProcessRequest?DoType=HttpHandler&DoMethod=AttachmentUpload_Down&HttpHandlerName=BP.WF.HttpHandler.WF_CCForm&WorkID=" + GetQueryString("WorkID") + "&FK_Node=" + GetQueryString("FK_Node") + "&MyPK=" + mypk;

    else {
        //按照数据流模式下载。
        var currentPath = GetHrefUrl();
        var path = currentPath.substring(0, currentPath.indexOf('/CCMobile') + 1);
        Url = path + "WF/Ath/downLoad.do?MyPK=" + mypk + "&PKVal=" + mypk;
    }

    let i = filePath.indexOf('\DataUser');
    let str = '/' + filePath.substring(i);
    console.log('str', str);
    filePath = str.replace(/\\/g, "/")
    console.log('filePath ', filePath);

    if (CommonConfig.IsOnlinePreviewOfAth == true) {
        if (filePath.endsWith('.jpg') || filePath.endsWith('.png') || filePath.endsWith('.jpeg') || filePath.endsWith('.bmp')) {

            previewShow(_src);
            return;
        }
        //配置的在线预览的方式，待处理.
        var host = window.location.protocol + "//" + window.location.host;
        var url = host + filePath;
        //url = 'http://localhost:2296/DataUser/UploadFile/ND18201/838/6456dd46-04ec-4843-a057-31351053cd42.混合 - 副本.docx';
        url = encodeURIComponent(base64Encode(url));
        //预览文件服务器.
        var fileServerHost = CommonConfig.PreviewPathOfAth;
        var viewUrl = fileServerHost + '/onlinePreview?url=' + url;
        //window.location.href = fileServerHost + '/onlinePreview?url=' + url;
        mui.openWindow({
            url: '/CCMobile/CCForm/AttachmentViewOnL.htm?viewUrl=' + viewUrl,
            id: 'mphone'
        });

        return;
    }
   
    if (filePath.endsWith('.jpg') || filePath.endsWith('.png') || filePath.endsWith('.jpeg') || filePath.endsWith('.bmp')){
   
        previewShow(_src);
        return;
    }

    SetHref(url);
}
function base64Encode(input) {
    let _keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    let output = "";
    let chr1, chr2, chr3, enc1, enc2, enc3, enc4;
    let i = 0;
    input = this.utf8_encode(input);
    while (i < input.length) {
        chr1 = input.charCodeAt(i++);
        chr2 = input.charCodeAt(i++);
        chr3 = input.charCodeAt(i++);
        enc1 = chr1 >> 2;
        enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
        enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
        enc4 = chr3 & 63;
        if (isNaN(chr2)) {
            enc3 = enc4 = 64;
        } else if (isNaN(chr3)) {
            enc4 = 64;
        }
        output = output +
            _keyStr.charAt(enc1) + _keyStr.charAt(enc2) +
            _keyStr.charAt(enc3) + _keyStr.charAt(enc4);
    }
    return output;
}
function utf8_encode(input) {
    input = input.replace(/\r\n/g, "\n");
    let utftext = "";
    for (let n = 0; n < input.length; n++) {
        let c = input.charCodeAt(n);
        if (c < 128) {
            utftext += String.fromCharCode(c);
        } else if ((c > 127) && (c < 2048)) {
            utftext += String.fromCharCode((c >> 6) | 192);
            utftext += String.fromCharCode((c & 63) | 128);
        } else {
            utftext += String.fromCharCode((c >> 12) | 224);
            utftext += String.fromCharCode(((c >> 6) & 63) | 128);
            utftext += String.fromCharCode((c & 63) | 128);
        }

    }
    return utftext;
}
//文件下载
function GetFileStream(mypk) {
    if (plant == "CCFlow") {
        Url = basePath + "/WF/Comm/ProcessRequest?DoType=HttpHandler&DoMethod=AttachmentUpload_Down&HttpHandlerName=BP.WF.HttpHandler.WF_CCForm&WorkID=" + GetQueryString("WorkID") + "&FK_Node=" + GetQueryString("FK_Node") + "&MyPK=" + mypk;
    } else {
        //按照数据流模式下载。
        var currentPath = GetHrefUrl();
        var path = currentPath.substring(0, currentPath.indexOf('/CCMobile') + 1);
        var Url = path + "WF/Ath/downLoad.do?MyPK=" + mypk + "&PKVal=" + mypk;
    }

    return Url;
}