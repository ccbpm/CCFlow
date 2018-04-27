//点击右边的下载标签.
function Down2018(fk_ath, pkVal, delPKVal) {
	if(plant == "CCFlow")
		window.location.href = 'DownFile.aspx?DoType=Down&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=<%=FK_Node %>&FK_Flow = <%=FK_Flow %>&FK_MapData=<%=FK_MapData %>&Ath=<%=Ath %>';
	else{
		var currentPath = window.document.location.href;
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        Url = 'downLoad.do?DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=<%=FK_Node %>&FK_Flow = <%=FK_Flow %>&FK_MapData=<%=FK_MapData %>&Ath=<%=Ath %>';
        window.location.href=Url;
	}
		
}

//点击文件名称执行的下载.
function Down2017(mypk) {
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
        if (plant != 'CCFlow') {
            var currentPath = window.document.location.href;
            var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
            str = path + str;
        }
       // var a = window.open(str, "_blank", "width=800, height=600,toolbar=yes");
      // window.frames["hrong"].location.href   =   str;
        //window.frames["hrong"].src = str;
      // sa();

       //js方法  
 
                var a = document.getElementById("downPdf");  
                a.href=str;  
                a.download="11.txt";  
                a.click();  
         

        //var fileURL = window.open(str, "_blank", "height=0,width=0,toolbar=no,menubar=no,scrollbars=no,resizable=on,location=no,status=no");
        //f/ileURL.document.execCommand("SaveAs");
        //fileURL.window.close();
        //fileURL.close();

        return;
    }
    if (data.indexOf("fromdb") > -1) {
        url = Handler + "?DoType=AttachmentDownFromByte&MyPK=" + mypk + "&m=" + Math.random();
        $('<form action="' + url + '" method="post"></form>').appendTo('body').submit().remove();
    }
    return;


}


function   sa()   

{   

       if(window.frames["hrong"].document.readyState!="complete")   

            setTimeout("sa()",   100);   

      else   

         window.frames["hrong"].document.execCommand('SaveAs');   

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
        return;
    }

    if (data.indexOf('url@') == 0) {

        data = data.replace('url@', ''); //如果返回url，就直接转向.

        var i = data.indexOf('\DataUser');
        var str = '/' + data.substring(i);
        str = str.replace('\\\\', '\\');
        if (plant != 'CCFlow') {
            var currentPath = window.document.location.href;
            var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
            str = path + str;
        }
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

    window.location.href = window.location.href;
}
   