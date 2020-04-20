$(function () {

    jQuery.getScript(basePath + "/WF/Admin/Admin.js")
        .done(function () {
            /* 耶，没有问题，这里可以干点什么 */
            //alert('ok');
        })
        .fail(function () {
            /* 靠，马上执行挽救操作 */
            //alert('err');
        });
});


var optionKey = 0;
function InitBar(optionKey, frmType) {

    var webUser = new WebUser();

    var html = "<div style='padding:5px' >表单组件: ";
    html += "<select id='changBar' >";

    html += "<option value=null  disabled='disabled'>+通用组件</option>";

    html += "<option value=" + FrmComponents.FrmImg + ">&nbsp;&nbsp;&nbsp;&nbsp;装饰类图片</option>";
    html += "<option value=" + FrmComponents.FrmImgAth + " >&nbsp;&nbsp;&nbsp;&nbsp;图片附件 </option>";
    html += "<option value=" + FrmComponents.IDCard + " >&nbsp;&nbsp;&nbsp;&nbsp;身份证 </option>";
    html += "<option value=" + FrmComponents.AthShow + " >&nbsp;&nbsp;&nbsp;&nbsp;字段附件</option>";
    html += "<option value=" + FrmComponents.HyperLink + " >&nbsp;&nbsp;&nbsp;&nbsp;超链接 </option>";
    html += "<option value=" + FrmComponents.HandWriting + " >&nbsp;&nbsp;&nbsp;&nbsp;写字板</option>";
    html += "<option value=" + FrmComponents.Score + ">&nbsp;&nbsp;&nbsp;&nbsp;评分控件</option>";
    html += "<option value=" + FrmComponents.Ath + ">&nbsp;&nbsp;&nbsp;&nbsp;独立附件</option>";
    html += "<option value=" + FrmComponents.Dtl + ">&nbsp;&nbsp;&nbsp;&nbsp;从表</option>";
    if(frmType == 0)//傻瓜表单
    html += "<option value=" + FrmComponents.BigText + ">&nbsp;&nbsp;&nbsp;&nbsp;大块Html说明文字引入</option>";

    html += "<option value=null  disabled='disabled'>+流程组件</option>";
    html += "<option value=" + FrmComponents.SignCheck + ">&nbsp;&nbsp;&nbsp;&nbsp;签批组件</option>";
    html += "<option value=" + FrmComponents.FlowBBS + ">&nbsp;&nbsp;&nbsp;&nbsp;评论（抄送）组件</option>";
    html += "<option value=" + FrmComponents.DocWord + ">&nbsp;&nbsp;&nbsp;&nbsp;公文字号</option>";
    html += "<option value=" + FrmComponents.JobSchedule + ">&nbsp;&nbsp;&nbsp;&nbsp;流程进度图</option>";

    html += "<option value=null  disabled='disabled'>+移动端控件</option>";
    html += "<option value=" + FrmComponents.Fiexed + ">&nbsp;&nbsp;&nbsp;&nbsp;系统定位</option>";

    html += "</select >";

    if (frmType!=8)
        html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
  
    html += "</div>";

    document.getElementById("bar").innerHTML = html;


}
