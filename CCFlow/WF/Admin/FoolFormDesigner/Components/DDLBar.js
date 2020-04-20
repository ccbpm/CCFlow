﻿$(function () {
    var fk_mapdata = GetQueryString("FK_MapData");
    var groupID = GetQueryString("GroupField");
    if (groupID == null)
        groupID = 1;
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





//var oNode = null, thePlugins = 'component';
//dialog.oncancel = function () {
//    if (UE.plugins[thePlugins].editdom) {
//        delete UE.plugins[thePlugins].editdom;
//    }
//};
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
        case 0:
            break;
        case 1:
            break;
        case 2:
            break;
        case 3:
            break;
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


 function Save() {
    //保存控件
    var frmComponent = $("#changBar").val();
    switch (parseInt(frmComponent)) {
        case 4: //地图控件
            ExtMap();
            break;
        case 5://录音控件
            break;
        case 6: //字段附件
            break;
        case 7:
            break;
        case 8://签字版
            ExtHandWriting();
            break;
        case 9://超链接
            ExtLink();
            break;
        case 10://文本
            break;
        case 11://图片
            break;
        case 12://图片附件
            break;
        case 13://身份证
            ExtIDCard();
            break;
        case 14://签批组件
            break;
        case 15://评论组件
            break;
        case 16://系统定位
            MapAttrFixed();
            break;
        case 17:// 公文字号
            ExtDocWord();
            break;
        case 18:
            break;
        case 50://流程进度图
            ExtJobSchedule();
            break;
        case 60://大文本
            ExtBigNoteHtmlText();
            break;
        case 70://独立附件
            break;
        case 80://从表
            break;
        case 101://评分控件
            ExtScore();
            break;

    }
}

//地图
function ExtMap() {
    var name = window.prompt('请输入地图名称:\t\n比如:中国地图', '地图');
    if (name == null || name == undefined)
        return;

    var frmID = fk_mapdata;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtMap();
        return;
    }

    //获得ID.
    var id = StrToPinYin(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        ExtMap();
        return;
    }

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 4; //地图.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; //
    mapAttr.UIWidth = 800;//宽度
    mapAttr.UIHeight = 500;//高度
    mapAttr.Insert(); //插入字段.

    var mapAttr1 = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 0;
    mapAttr1.MyPK = frmID + "_AtPara";
    mapAttr1.FK_MapData = frmID;
    mapAttr1.KeyOfEn = "AtPara";
    mapAttr1.UIVisible = 0;
    mapAttr1.Name = "AtPara";
    mapAttr1.MyDataType = 1;
    mapAttr1.LGType = 0;
    mapAttr1.ColSpan = 1; //
    mapAttr1.UIWidth = 100;
    mapAttr1.UIHeight = 23;
    mapAttr1.Insert(); //插入字段

    mapAttr.Retrieve();
    //var url = './../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtMap&MyPK=' + mapAttr.MyPK;
    var _html = "<div style='text-align:left;padding-left:0px' id='Map_" + mapAttr.KeyOfEn + "' data-type='Map' data-key='" + mapAttr.MyPK + "' leipiplugins='component'>";
    _html += "<input type='button' name='select' value='选择'  style='background: #fff;color: #545454;font - size: 12px;padding: 4px 15px;margin: 5px 3px 5px 3px;border - radius: 3px;border: 1px solid #d2cdcd;'/>";
    _html += "<input type = text style='width:200px' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' />";
    _html += "</div>";
    editor.execCommand('insertHtml', _html);

}

//公文字号
function ExtDocWord() {
    var en = new Entity("BP.Sys.MapAttr");
    en.SetPKVal(fk_mapdata + "_DocWord");
    if (en.RetrieveFromDBSources() == 1) {
        alert("该表单DocWord字段已经存在，公文字号默认的字段DocWord,请确认该字段是否为公文字段");
        return;
    }


    var mypk = fk_mapdata + "_DocWord";
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 17; //公文字号.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = fk_mapdata;
    mapAttr.KeyOfEn = "DocWord";
    mapAttr.Name = "公文字号";
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; //
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();
    var _Html = "<input type='text'  id='TB_DocWord' name='TB_DocWord' data-key='DocWord' data-name='公文字号' data-type='DocWord'   leipiplugins='component' style='width:98%'/>";
    editor.execCommand('insertHtml', _Html);
}
//身份证
function ExtIDCard() {
    var IDCard = [{ No: "IDCardName", Name: "姓名" }, { No: "IDCardNo", Name: '身份证号' }, { No: "IDCardAddress", Name: "地址" }];
    var frmID = GetQueryString("FK_MapData");
    for (var i = 0; i < IDCard.length; i++) {
        var mapAttr = new Entity("BP.Sys.MapAttr");
        mapAttr.SetPKVal(frmID + "_" + IDCard[i].No);
        if (mapAttr.RetrieveFromDBSources() == 0) {
            mapAttr.FK_MapData = frmID;
            mapAttr.KeyOfEn = IDCard[i].No;
            mapAttr.Name = IDCard[i].Name;
            mapAttr.GroupID = groupID;
            mapAttr.UIContralType = 13; //身份证号.
            mapAttr.MyDataType = 1;
            if (IDCard[i].No == "IDCardAddress")
                mapAttr.ColSpan = 3; //单元格.
            else
                mapAttr.ColSpan = 1;
            mapAttr.TextColSpan = 1;
            mapAttr.LGType = 0; //文本
            mapAttr.UIIsEnable = 0; //不可编辑
            mapAttr.UIIsInput = 1;//必填
            mapAttr.UIWidth = 150;
            mapAttr.UIHeight = 23;
            mapAttr.Insert(); //插入字段.
        } else {
            alert("字段" + IDCard[i].No + "已存在，请变更表单中的" + mapAttr.Name + "的编号");

        }

    }
    alert("创建成功");
}

//系统定位（需要创建唯一)
function MapAttrFixed() {
    var mapAttr = new Entity("BP.Sys.FrmUI.MapAttrFixed");
    mapAttr.SetPKVal(GetQueryString("FK_MapData") + "_Fixed");
    if (mapAttr.RetrieveFromDBSources() == 0) {
        mapAttr.FK_MapData = GetQueryString("FK_MapData");
        mapAttr.KeyOfEn = "Fixed";
        mapAttr.Name = "系统定位";
        mapAttr.GroupID = 1;
        mapAttr.UIContralType = 16; //系统定位
        mapAttr.MyDataType = 1;
        mapAttr.ColSpan = 1;
        mapAttr.TextColSpan = 1;
        mapAttr.LGType = 0; //文本
        mapAttr.UIIsEnable = 0; //不可编辑
        mapAttr.UIIsInput = 0;
        mapAttr.UIWidth = 150;
        mapAttr.UIHeight = 23;
        mapAttr.Insert(); //插入字段.
        alert("创建成功");
    } else {
        alert("表单" + GetQueryString("FK_MapData") + "已经存在系统定位按钮，不能重复创建");
        return;
    }
    ens.Retrieve("FK_MapData", GetQueryString("FK_MapData"), "Text", "系统定位");

}

//附件.
function ExtAth() {

    var name = window.prompt('请输入附件名称:\t\n比如:报送材料、报销资料', '附件');
    if (name == null || name == undefined)
        return;

    var frmID = GetQueryString("FK_MapData");
    // alert(frmID + imgName);
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtAth();
        return;
    }

    //获得ID.
    var id = StrToPinYin(name);

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        return;
    }
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.GroupID = groupID;
    mapAttr.UIContralType = 6; //附件控件.
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 3; //
    mapAttr.TextColSpan = 1; //
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.IsEnableInAPP = 0;
    mapAttr.Insert(); //插入字段.

    mapAttr.Retrieve();

    var en = new Entity("BP.Sys.FrmAttachment");
    en.MyPK = frmID + "_" + id;
    en.FK_MapData = frmID;
    en.NoOfObj = id;
    en.GroupID = mapAttr.GroupID; //设置分组列.
    en.Name = name;
    en.UploadType = 1; //多附件.
    en.IsVisable = 0; //让其不可见.
    en.CtrlWay = 4; // 按流程WorkID计算
    en.SetPara("IsShowMobile", 1);
    en.Insert(); //插入到数据库.
    window.location.href = "../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&MyPK=" + en.MyPK;
}

//超链接.
function ExtLink() {

    var name = window.prompt('请输入超链接名称:\t\n比如:我的连接、点击这里打开', '我的连接');
    if (name == null || name == undefined)
        return;

    var frmID = GetQueryString("FK_MapData");
    // alert(frmID + imgName);
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtLink();
        return;
    }

    //获得ID.
    var id = StrToPinYin(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        ExtLink();
        return;
    }

    var link = window.prompt('请输入超链地址:\t\n比如:https://gitee.com/opencc', 'https://gitee.com/opencc');
    if (link == null || link == undefined)
        return;

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.SetPara("Url", link.replace(/@/g, '$'));
    mapAttr.UIContralType = 9; //超链接.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.GroupID = groupID;
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 0; //
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.Tag1 = "_blank"; //打开目标.
    mapAttr.Tag2 = link; // 超链接地址.
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();
    window.location.href = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtLink&MyPK=" + mapAttr.MyPK;
}

//评分控件
function ExtScore() {

    var name = window.prompt('请输入评分事项名称:\t\n比如:快递速度，服务水平', '评分事项');
    if (name == null || name == undefined)
        return;

    var frmID = GetQueryString("FK_MapData");
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtScore();
        return;
    }

    //获得ID.
    var id = StrToPinYin(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        ExtScore();
        return;
    }

    var score = window.prompt('请设定总分:\t\n比如:5，10', '5');
    if (score == null || score == undefined)
        return;

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 101; //评分控件.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.GroupID = groupID;
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; //
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.Tag2 = score; // 总分
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();
    window.location.href = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtScore&MyPK=" + mapAttr.MyPK;
}
//大块文本
function ExtBigNoteHtmlText() {

    var frmID = GetQueryString("FK_MapData");
    //增加大文本
    if (window.confirm('您确认要创建吗？') == false)
        return;
    window.location.href = "./EditFExtContral/60.BigNoteHtmlText.htm?FrmID=" + frmID;
}

//手写签名版.
function ExtHandWriting() {

    var name = window.prompt('请输入签名版名称:\t\n比如:签字版、签名', '签字版');
    if (name == null || name == undefined)
        return;

    var frmID = GetQueryString("FK_MapData");
    // alert(frmID + imgName);
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtHandWriting();
        return;
    }

    //获得ID.
    var id = StrToPinYin(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        ExtHandWriting();
        return;
    }

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 8; //手写签名版.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.GroupID = groupID;
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; //
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();
    window.location.href = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtHandWriting&MyPK=" + mapAttr.MyPK;
}

//流程进度图.
function ExtJobSchedule() {

    var name = "流程进度图";
    var id = "JobSchedule";
    var frmID = GetQueryString("FK_MapData");

    //获得ID.
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert("已经存在，一个表单仅仅允许有一个流程进度图.");
        return;
    }

    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 50; //流程进度图.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.GroupID = groupID;
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 4; //
    mapAttr.UIWidth = 0;
    mapAttr.UIHeight = 100;
    mapAttr.Idx = 0;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();
    window.location.href = "../../Comm/EnOnly.htm?EnName=BP.WF.Template.ExtJobSchedule&MyPK=" + mapAttr.MyPK;

}
