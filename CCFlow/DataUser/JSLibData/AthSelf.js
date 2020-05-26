//附件上传与查看, 可以重写。

//在线预览，如果需要连接其他的文件预览查看器，就需要在这里重写该方法.
function AthView(fk_ath, pkVal, delPKVal) {

    if (plant == "CCFlow")
        window.location.href = basePath + '/WF/CCForm/DownFile.aspx?DoType=Down&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=' + FK_Node + '&FK_Flow=' + FK_Flow + '&FK_MapData=' + FK_MapData + '&Ath=' + Ath;
    else {
        var currentPath = window.document.location.href;
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        Url = path + 'WF/Ath/downLoad.do?DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=' + FK_Node + '&FK_Flow=' + FK_Flow + '&FK_MapData=' + FK_MapData + '&Ath=' + Ath;
        window.location.href = Url;
    }
}

//文件上传成功后,要激活的事件,用户进行二次开发比如：把ftp文件转化pdf进行预览.
function AfterAthUploadOver(frmID,pkVal,guid) {

    return;

  //  var url = "http://127.0.0.1:8012/addTask?url=ftp://xxx/test.txt";
    var url = "http://127.0.0.1:8012/addTask?id=" + pkVal;
    // window.location.href = url;
    $("#feeds").load(url, { limit: 25 }, function () {
        //alert("The last 25 entries in the feed have been loaded");
    });

    //var athDB = new Entity('BP.Sys.FrmAttachmentDB');
    //athDB.SetPKVal(pkVal);
    //ath.Retrurn();
}


