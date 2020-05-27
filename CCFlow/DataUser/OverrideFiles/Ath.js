/*说明:

1, 该js文件被引入到 /CCForm/Ath.htm 文件里.
2, 您可以在这里做二次开发，重新相关的事件来完成个性化的业务逻辑处理.
    比如:您可以重现上传按钮事件，在上传按钮事件里写入自己的方法.
3, 这里用到了,南京在上传附件前需要先选择附件列表，然后上传.
  
*/

/*
当选择文件上传的按钮的时候激活的事件,
可以用于转向其他的页面，处理文件上传业务逻辑. 

*/
function OnUploadClick() {


    return true;


    return false; //就是关闭不弹出窗口.

}
//附件上传与查看, 可以重写。

//在线预览，如果需要连接其他的文件预览查看器，就需要在这里重写该方法.
function AthViewOverWrite_Del(fk_ath, pkVal, delPKVal) {

   alert(fk_ath + pkVal + delPKVal + ":AthViewOverWrite执行成功.");

}

//文件上传成功后,要激活的事件,用户进行二次开发比如：把ftp文件转化pdf进行预览.
//mypks=多个用逗号分开.
function AfterAthUploadOver(frmID, pkVal, mypks) {

    return;

    //执行成功.
    alert("AfterAthUploadOver执行成功:" + frmID + pkVal + mypks);
    return;
    //var url = "http://127.0.0.1:8012/addTask?url="+mypks;

    var url = "http://127.0.0.1:8012/addTask?id=" + mypks;
    $("#feeds").load(url, { limit: 25 }, function () {

    });


}