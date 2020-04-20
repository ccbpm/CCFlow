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
    html += "<select id='changBar' onchange='changeOption()' >";

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
    if (frmType == 0)//傻瓜表单
        html += "<option value=" + FrmComponents.BigText + ">&nbsp;&nbsp;&nbsp;&nbsp;大块Html说明文字引入</option>";

    html += "<option value=null  disabled='disabled'>+流程组件</option>";
    html += "<option value=" + FrmComponents.SignCheck + ">&nbsp;&nbsp;&nbsp;&nbsp;签批组件</option>";
    html += "<option value=" + FrmComponents.FlowBBS + ">&nbsp;&nbsp;&nbsp;&nbsp;评论（抄送）组件</option>";
    html += "<option value=" + FrmComponents.DocWord + ">&nbsp;&nbsp;&nbsp;&nbsp;公文字号</option>";
    html += "<option value=" + FrmComponents.JobSchedule + ">&nbsp;&nbsp;&nbsp;&nbsp;流程进度图</option>";

    html += "<option value=null  disabled='disabled'>+移动端控件</option>";
    html += "<option value=" + FrmComponents.Fiexed + ">&nbsp;&nbsp;&nbsp;&nbsp;系统定位</option>";

    html += "</select >";

    if (frmType != 8)
        html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";

    html += "</div>";

    document.getElementById("bar").innerHTML = html;

    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");

}
function changeOption() {
    
    //var nodeID = GetQueryString("FK_Node");
    //var en = new Entity("BP.WF.Template.NodeSimple", nodeID);
    //flowNo = en.FK_Flow;
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = 0;
    if (index > 1) {
        optionKey = sele[index].value
    }
    var roleName = "";
    
    switch (parseInt(optionKey)) {
        case FrmComponents.Map:
            roleName = "4.Map.htm";
            break;
        case FrmComponents.MicHot:
            roleName = "5.MicHot.htm";
            break;
        case FrmComponents.AthShow:
            roleName = "6.AthShow.htm";
            break;
        case FrmComponents.MobilePhoto:
            roleName = "7.MobilePhoto.htm";
            break;
        case FrmComponents.HandWriting:
            roleName = "8.HandWriting.htm";
            break;
        case FrmComponents.HyperLink:
            roleName = "9.HyperLink.htm";
            break;
        case FrmComponents.Lab:
            roleName = "10.Lab.htm";
            break;
        case FrmComponents.FrmImg:
            roleName = "11.FrmImg.htm";
            break;
        case FrmComponents.FrmImgAth:
            roleName = "12.FrmImgAth.htm";
            break;
        case FrmComponents.IDCard:
            roleName = "13.IDCard.htm";
            break;
        case FrmComponents.SignCheck:
            roleName = "14.SignCheck.htm";
            break;
        case FrmComponents.FlowBBS:
            roleName = "15.FlowBBS.htm";
            break;
        case FrmComponents.Fiexed:
            roleName = "16.Fiexed.htm";
            break;
        case FrmComponents.DocWord:
            roleName = "17.DocWord.htm";
            break;
        case FrmComponents.JobSchedule:
            roleName = "50.JobSchedule.htm";
            break;
        case FrmComponents.BigText:
            roleName = "60.BigText.htm";
            break;
        case FrmComponents.Ath:
            roleName = "70.Ath.htm";
            break;
        case FrmComponents.Dtl:
            roleName = "80.Dtl.htm";
            break;
        case FrmComponents.Score:
            roleName = "101.Score.htm";
            break;
        default:
            roleName = "4.Map.htm";
            break;
    }
    window.location.href = roleName;
}

