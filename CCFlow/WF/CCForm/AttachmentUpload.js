﻿//点击右边的下载标签.
function Down(fk_ath, pkVal, delPKVal) {
    window.location.href = 'AttachmentUpload.aspx?DoType=Down&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=<%=FK_Node %>&FK_Flow = <%=FK_Flow %>&FK_MapData=<%=FK_MapData %>&Ath=<%=Ath %>';
}

//点击文件名称执行的下载.
function Down2017(mypk) {

    //$("#Msg").html("<img src=../Img/loading.gif />正在加载,请稍后......");

    //组织url.
    var url = Handler + "?DoType=AttachmentUpload_Down&MyPK="+mypk+"&m=" + Math.random();

    $.ajax({
        type: 'post',
        async: true,
        url: url,
        dataType: 'html',
        success: function (data) {

            if (data.indexOf('err@') == 0) {
                alert(data); //如果是异常，就提提示.
                return;
            }

            if (data.indexOf('url@') == 0) {

                data = data.replace('url@', ''); //如果返回url，就直接转向.

                var i = data.indexOf('\DataUser');
                var str = '/' + data.substring(i);

                window.location.href = str;
                return;
            }
            alert(data);
            return;
        }
    });
}