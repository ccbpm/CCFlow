

/**
* 从表附件 图片附件 ,表格附件只显示上传按钮
* @param athchment 附件属性
* @param athDivID 生成的附件信息追加的位置
*/
function DtlAthTable_Init(athchment, athDivID, refPKVal) {
    //附件显示
    InitDtlAthPage(athchment, athDivID, refPKVal);
}
/**
* 初始化附件列表信息
* @param athDivID 生成的附件信息追加的位置
*/
function InitDtlAthPage(athchmentMyPK, athDivID, refPKVal) {
    var athchment = new Entity("BP.Sys.FrmAttachment", athchmentMyPK);
    //文件类型指定的任意文件，表格展示
    if (athchment.FileType == 0) {
        var index = athDivID.replace("Div_" + athchmentMyPK + "_", "");
        var _html ="<a href='javaScript:void(0);' onclick='OpenDtlAth(this,\""+athchmentMyPK+"\")' style='margin-left:20px' titile='附件'> <i class='fa fa-upload' aria-hidden='true'></a>"
        $("#" + athDivID).after(_html);
        $("#" + athDivID).remove();
        return;
    }

    //1.请求后台数据
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddUrlData();
    handler.AddPara("RefOID", refPKVal);
    handler.AddPara("FK_FrmAttachment", athchment.MyPK);
    handler.AddPara("FK_MapData", athchment.FK_MapData);
    var data = handler.DoMethodReturnString("Ath_Init");
    if (data.indexOf('err@') == 0) {
        //执行方法报错
        alert(data);
        console.log(data);
        return;
    }

    if (data.indexOf('url@') == 0) {
        var url = data.replace('url@', '');
        SetHref(url);
        return;
    }

    data = JSON.parse(data);
    athDesc = data["AthDesc"][0]; // 附件属性
    var dbs = data["DBAths"];  // 附件列表数据
    //图片显示
    $("#" + athDivID).html(FileShowPic_Dtl(athDesc, dbs, athDivID, refPKVal,athchment.FK_MapData));
    $.each($("#" + athDivID+" .athImg"), function (index, item) {
        $(item).on("click", function () {
            var _this = $(this); //将当前的pimg元素作为_this传入函数  
            var src = _this.parent().css("background-image").replace("url(\"", "").replace("\")", "")
            imgDtlShow(this, src);
        });
    })    
}

/**
* 附件图片显示的方式
* @param athDesc 附件属性
* @param dbs 附件列表
*/
function FileShowPic_Dtl(athDesc, dbs,athDivID,refPKVal,fk_mapData) {
    var exts = athDesc.Exts || "";
    if (exts.indexOf("*.*") != -1 || exts == "")
        exts = "image/gif,image/jpg,image/jepg,image/jpeg,image/bmp,image/png,image/tif,image/gsp";

    var _Html = "<form id='Form_" + athDesc.MyPK + "' enctype='multipart/form-data' method='post'>";
    for (var i = 0; i < dbs.length; i++) {
        var db = dbs[i];
        var url = GetFileStream(db.MyPK, db.FK_FrmAttachment);
        _Html += "<div id='" + db.MyPK + "' class='image-item athInfo' style='background-image: url(&quot;" + url + "&quot;);width:135px !important'>";
        if ((athDesc.DeleteWay == 1) || ((athDesc.DeleteWay == 2) && (db.Rec == webUser.No)))
            _Html += "<div class='image-close' onclick='Del(\"" + db.MyPK + "\",\"" + db.FK_FrmAttachment + "\",\"" + athDivID + "\",\"" + fk_mapData +"\")'>X</div>";
        _Html += "<div style ='width: 100%; height: 100%;' class='athImg' ></div>";
        _Html += "<div class='image-name' id = 'name-0-0' > ";
       /* if (athDesc.IsDownload == 0)
            _Html += "<p style = 'text-align:center;width:63.4px;margin:0;padding:0;overflow:hidden;text-overflow: ellipsis;white-space: nowrap' >" + db.FileName + "</p>";
        else
            _Html += "<p style = 'text-align:center;width:63.4px;margin:0;padding:0;overflow:hidden;text-overflow: ellipsis;white-space: nowrap' ><a href=\"javascript:Down2018('" + db.MyPK + "');\" title='" + db.FileName.split(".")[0] + "'>" + db.FileName.split(".")[0] + "</a></p>";*/
        _Html += "</div>";
        _Html += "</div>";
    }
    //可以上传附件，增加上传附件按钮
    if (athDesc.IsUpload == true && pageData.IsReadonly != "1") {

        _Html += "<div class='image-item space' style='width: 135px !important'><input type='file' id='file_" + athDivID + "'name='file_" + athDesc.MyPK + "' accept='" + exts + "' onchange='UploadChangeAth(\"" + athDesc.MyPK + "\",\"" + athDivID + "\",\"" + refPKVal + "\"," + athDesc.FileMaxSize + "," + athDesc.TopNumOfUpload + "," + dbs.length + ",\"" + fk_mapData+"\");'></div>";
    }
    _Html += "</form>";

    return _Html;

}

//文件数据流
function GetFileStream(mypk, FK_FrmAttachment) {
    var Url = "";
    if (plant == "CCFlow") {
        Url = basePath + "/WF/Comm/Handler.ashx?DoType=HttpHandler&DoMethod=AttachmentUpload_Down&HttpHandlerName=BP.WF.HttpHandler.WF_CCForm&WorkID=" + GetQueryString("WorkID") + "&FK_Node=" + GetQueryString("FK_Node") + "&MyPK=" + mypk;
    } else {
        //按照数据流模式下载。
        Url = basePath + "/WF/Ath/downLoad.do?MyPK=" + mypk + "&FK_FrmAttachment=" + FK_FrmAttachment;
    }

    return Url;
}

/**
* 附件下载
* @param fk_ath  附件的属性
* @param MyPK 上传附件数据的信息主键
*/
function Down2018(mypk) {

    var nodeID = GetQueryString("FK_Node");
    var workID = GetQueryString("WorkID");

    var url = "";
    if (plant == "CCFlow") {
        SetHref(basePath + "/WF/Comm/Handler.ashx?DoType=HttpHandler&DoMethod=AttachmentUpload_Down&HttpHandlerName=BP.WF.HttpHandler.WF_CCForm&WorkID=" + workID + "&FK_Node=" + nodeID + "&MyPK=" + mypk);
        return;
    }


    var currentPath = GetHrefUrl();
    var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
    url = path + 'WF/Ath/downLoad.do?MyPK=' + mypk + "&WorkID=" + workID + "&FK_Node=" + nodeID;
    if(typeof filterXSS === 'function'){
        url = filterXSS(url);
    }else {
        url = url.replace(/<\/?[^>]+>/gi, '')
        .replace(/[(]/g, '')
        .replace(/->/g, '_')
    }
    if (IEVersion() < 11) {
        window.open(url);
        return;
    }
    var link = document.createElement('a');
    link.setAttribute("download", "");
    link.href = url;
    link.click();

}



/**
* 删除附件
* @param delPKVal
*/
function Del(delPKVal, fk_framAttachment, athDivID, fk_mapData) {
    if (window.confirm('您确定要删除吗？ ') == false)
        return;
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("DelPKVal", delPKVal);
    var data = handler.DoMethodReturnString("AttachmentUpload_Del");
    if (data.indexOf("err@") != -1) {
        alert(data);
        console.log(data);
        return;
    }
    //获取
    InitDtlAthPage(fk_framAttachment, athDivID, fk_mapData);
}

/**
 * 图片附件上传
 */
function UploadChangeAth(fk_frmAttachment, athDivID, refPKVal, fileSize, fileMaxNum, uploadLen, fk_mapData) {
    var element = $("#file_" + athDivID);
    if (element.length == 0)
        return;
    var fileObj = element.val();
    if (typeof (fileObj) == "undefined" || fileObj.size <= 0) {
        alert("请选择上传的文件.");
        return;
    }

    if (uploadLen + 1 > fileMaxNum) {
        alert("超过了最大上传数量");
        return;
    }
    var file = element[0].files[0];
    if (file.size > fileSize * 1000) {
        alert("上传附件大小的最大限制是" + fileSize + "KB");
        return;
    }
  
    var uploadUrl = "";
    if (plant == 'CCFlow')
        uploadUrl = basePath + '/WF/CCForm/Handler.ashx?AttachPK=' + fk_frmAttachment + '&DoType=MoreAttach&FK_Flow=' + pageData.FK_Flow + '&PKVal=' + refPKVal;
    else {
        uploadUrl = basePath + "/WF/Ath/AttachmentUploadS.do?FK_FrmAttachment=" + fk_frmAttachment + '&FK_Flow=' + pageData.FK_Flow + "&PKVal=" + refPKVal;
    }
    uploadUrl += "&WorkID=" + pageData.WorkID;
    uploadUrl += "&FID=" + pageData.FID;
    uploadUrl += "&FK_Node=" + pageData.FK_Node;
    uploadUrl += "&PWorkID=" + GetQueryString("PWorkID");
    uploadUrl += "&FK_MapData=" + fk_mapData;
    //提交数据
    var option = {
        url: uploadUrl,
        type: 'POST',
        dataType: 'json',
        headers: { "ClientCallMode": "ajax" },
        success: function (data) {
            if (data != null && data.indexOf("err@") != -1) {
                alert(data.replace("err@", ""));
                return;
            }
            InitDtlAthPage(fk_frmAttachment, athDivID, refPKVal, fk_mapData);
        },
        error: function (xhr, status, err) {
            if (xhr.responseText != null && xhr.responseText.indexOf("err@") != -1) {
                alert(xhr.responseText);
                return;
            }
            InitDtlAthPage(fk_frmAttachment, athDivID, refPKVal, fk_mapData);
        }

    };

    $("form").ajaxSubmit(option);
    return false;


}
/**
 *图片附件预览
 * @param {any} obj
 */
function imgDtlShow(obj, src) {
    if (src == null || src == undefined)
        src = obj.src;
    var img = new Image();
    img.src = src;
    img.onload = () => {
        var height = img.height + 50; //获取图片高度
        if (height > window.innerHeight - 150)
            height = window.innerHeight - 150;
        var width = img.width; //获取图片宽度
        var imgHtml = "<div style='text-align:center'><img src='" + src + "' /></div>";
        //弹出层
        window.parent.layer.open({
            type: 1,
            shade: 0.8,
            offset: 'auto',
            area: ['80%', '80%'],
            shadeClose: true,//点击外围关闭弹窗
            scrollbar: false,//不现实滚动条
            title: "",
            closeBtn: 1,
            content: imgHtml, //捕获的元素，注意：最好该指定的元素要存放在body最外层，否则可能被其它的相对元素所影响  
            cancel: function () {
                //layer.msg('捕获就是从页面已经存在的元素上，包裹layer的结构', { time: 5000, icon: 6 });  
            }
        });
    }

}

//打开附件.
function OpenDtlAth(obj, athMyPK) {
    debugger
    var index = $(obj).parent().parent().attr("data-id");
    var dtlOID = $(obj).parent().parent().data().data.OID || 0;
    var workID = GetQueryString("RefPKVal");
    var dtlName = GetQueryString("EnsName");
    var fk_node = GetQueryString("FK_Node");
    var fk_flow = GetQueryString("FK_Flow");
    var FFK_MapData = GetQueryString("FK_MapData");
    var IsReadonly = GetQueryString("IsReadonly");
    athMyPK = athMyPK || dtlName + "_AthMDtl";
    var pkVal = dtlOID == 0 ? workID + "_" + index : dtlOID;
    url = basePath + "/WF/CCForm/Ath.htm?IsBTitle=1&PKVal=" + pkVal + "&Ath=AthMDtl&FK_MapData=" + GetQueryString("EnsName") + "&AthPK=" + athMyPK + "&WorkID=" + workID + "&FK_Node=" + fk_node + "&FK_Flow=" + fk_flow + "&FFK_MapData=" + FFK_MapData + "&IsReadonly=" + IsReadonly;
    var AthNum = 0;
   
    if (typeof parent.OpenLayuiDialog != "undefined") {
        var W = parent.window.innerWidth / 2;
        parent.OpenLayuiDialog(url, "从表多附件", W, 70, "auto", false, false, false, null, function () {
            //关闭窗口事件
            AthNum = $("#athModel_" + dtlName + "_AthMDtl").find("a").length;
            var rowCurrentIndex = parseInt($($(index).parent().parent().children()[0]).text()) - 1;
            $("#Ath_" + rowCurrentIndex).html(AthNum);
            $("#TB_AthNum_" + rowCurrentIndex).val(AthNum);

        });
        return;
    }
    OpenBootStrapModal(url, "从表多附件", window.innerWidth / 2, 70, "auto", false, false, false, null, function () {
        //关闭窗口事件
        AthNum = $("#athModel_" + dtlName + "_AthMDtl").find("a").length;
        var rowCurrentIndex = parseInt($($(index).parent().parent().children()[0]).text()) - 1;
        $("#Ath_" + rowCurrentIndex).html(AthNum);
        $("#TB_AthNum_" + rowCurrentIndex).val(AthNum);

    });
}