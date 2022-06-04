//点击右边的下载标签.
function Down(fk_ath, pkVal, delPKVal) {
    SetHref('AttachmentUpload.aspx?DoType=Down&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=<%=FK_Node %>&FK_Flow = <%=FK_Flow %>&FK_MapData=<%=FK_MapData %>&Ath=<%=Ath %>');
}

//点击文件名称执行的下载.
function Down2017(mypk) {

    //$("#Msg").html("<img src=../Img/loading.gif />正在加载,请稍后......");


    //组织url.
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("MyPK", mypk);
    var data = handler.DoMethodReturnString("AttachmentUpload_Down");

    if (data.indexOf('err@') == 0) {
        alert(data); //如果是异常，就提提示.
        return;
    }

    if (data.indexOf('url@') == 0) {

        data = data.replace('url@', ''); //如果返回url，就直接转向.

        var i = data.indexOf('\DataUser');
        var str = '/' + data.substring(i);
        str = str.replace('\\\\', '\\');
        window.open(str, "_blank", "width=800, height=600,toolbar=yes");
        return;
    }
    if (data.indexOf("fromdb") > -1) {
        url = Handler + "?DoType=AttachmentDownFromByte&MyPK=" + mypk + "&m=" + Math.random();
        $('<form action="' + url + '" method="post"></form>').appendTo('body').submit().remove();
    }
    return;
}


/* 一下的方法从网上找到的，都不适用 . */

function Down3(str) {

    alert(str);
    var a;
    a = window.open(str, "_blank", "width=0, height=0,status=0");
    a.document.execCommand("SaveAs");
    a.close();
}

function Down2(imgURL) {

    var oPop = window.open(imgURL, "", "width=1, height=1, top=5000, left=5000");

    for (; oPop.document.readyState != "complete"; ) {
        if (oPop.document.readyState == "complete")
            break;
    }

    oPop.document.execCommand("SaveAs");
    oPop.close();

}

function Down(url) {

    var $eleForm = $("<form method='get'></form>");

    $eleForm.attr("action", url);

    $(document.body).append($eleForm);

    //提交表单，实现下载
    $eleForm.submit();
}

function downloadFile(url) {
    try {
        var elemIF = document.createElement("iframe");
        elemIF.src = url;
        elemIF.style.display = "none";
        document.body.appendChild(elemIF);
    } catch (e) {

    }
}

function DownZip() {

    var httphandle = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    httphandle.AddUrlData();

    var data = httphandle.DoMethodReturnString("AttachmentUpload_DownZip");

    if (data.indexOf('err@') == 0) {
        alert(data); //如果是异常，就提提示.
        console.log(data);
        return;
    }

    if (data.indexOf('url@') == 0) {

        data = data.replace('url@', ''); //如果返回url，就直接转向.

        var i = data.indexOf('\DataUser');
        var str = '/' + data.substring(i);
        str = str.replace('\\\\', '\\');

        window.open(str, "_blank", "width=800, height=600,toolbar=yes");
        return;
    }
    alert(data);
}

//删除附件.
function Del(fk_ath, pkVal, delPKVal) {

    if (window.confirm('您确定要删除吗？ ') == false)
        return;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("DelPKVal", delPKVal);
    var data = handler.DoMethodReturnString("AttachmentUpload_Del");

    alert(data);

    Reload();
}
   