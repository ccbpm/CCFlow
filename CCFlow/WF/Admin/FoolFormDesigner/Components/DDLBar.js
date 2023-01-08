var localHref = GetLocalWFPreHref();
var frmType = GetQueryString("FrmType");
var fk_mapData = GetQueryString("FK_MapData");
var groupID = 1;

$(function () {

    jQuery.getScript(localHref + "/WF/Admin/Admin.js")
        .done(function () {

        })
        .fail(function () {

        });
    if (frmType == 8) {
        jQuery.getScript(localHref + "/WF/Admin/DevelopDesigner/js/ueditor/dialogs/internal.js")
            .done(function () { })
            .fail(function () { });
        jQuery.getScript(localHref + "/WF/Admin/DevelopDesigner/DialogCtr/Components.js")
            .done(function () { })
            .fail(function () { });
    }

});

var optionKey = 0;
function InitBar(optionKey) {

    var html = "<div style='padding:5px' >表单组件: ";
    html += "<select id='changBar' onchange='changeOption()' >";

    html += "<option value=null  disabled='disabled'>+通用组件</option>";

    html += "<option value=" + FrmComponents.FrmImg + ">&nbsp;&nbsp;&nbsp;&nbsp;装饰类图片</option>";
    html += "<option value=" + FrmComponents.FrmImgAth + " >&nbsp;&nbsp;&nbsp;&nbsp;图片附件 </option>";
    html += "<option value=" + FrmComponents.IDCard + " >&nbsp;&nbsp;&nbsp;&nbsp;身份证 </option>";
    html += "<option value=" + FrmComponents.AthShow + " >&nbsp;&nbsp;&nbsp;&nbsp;字段附件</option>";
    html += "<option value=" + FrmComponents.Ath + ">&nbsp;&nbsp;&nbsp;&nbsp;独立附件(表格模式展示)</option>";
    html += "<option value=" + FrmComponents.HyperLink + " >&nbsp;&nbsp;&nbsp;&nbsp;超链接 </option>";
    html += "<option value=" + FrmComponents.Btn + ">&nbsp;&nbsp;&nbsp;&nbsp;按钮</option>";
    html += "<option value=" + FrmComponents.HandWriting + " >&nbsp;&nbsp;&nbsp;&nbsp;写字板</option>";
    html += "<option value=" + FrmComponents.Score + ">&nbsp;&nbsp;&nbsp;&nbsp;评分控件</option>";
    html += "<option value=" + FrmComponents.Dtl + ">&nbsp;&nbsp;&nbsp;&nbsp;从表</option>";
    html += "<option value=" + FrmComponents.Frame + ">&nbsp;&nbsp;&nbsp;&nbsp;框架</option>";
    if (frmType == 0)//傻瓜表单
        html += "<option value=" + FrmComponents.BigText + ">&nbsp;&nbsp;&nbsp;&nbsp;大块Html说明文字引入</option>";

    html += "<option value=null  disabled='disabled'>+流程组件</option>";
    html += "<option value=" + FrmComponents.SignCheck + ">&nbsp;&nbsp;&nbsp;&nbsp;签批组件</option>";
    html += "<option value=" + FrmComponents.FlowBBS + ">&nbsp;&nbsp;&nbsp;&nbsp;评论（抄送）组件</option>";
    html += "<option value=" + FrmComponents.GovDocFile + ">&nbsp;&nbsp;&nbsp;&nbsp;公文正文组件</option>";
    html += "<option value=" + FrmComponents.DocWord + ">&nbsp;&nbsp;&nbsp;&nbsp;发文字号</option>";
    html += "<option value=" + FrmComponents.DocWordReceive + ">&nbsp;&nbsp;&nbsp;&nbsp;收文字号</option>";
    html += "<option value=" + FrmComponents.WorkCheck + ">&nbsp;&nbsp;&nbsp;&nbsp;审核组件</option>";
    if (frmType ==8)
    html += "<option value=" + FrmComponents.SubFlow + ">&nbsp;&nbsp;&nbsp;&nbsp;父子流程组件</option>";
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
        case FrmComponents.WorkCheck:
            roleName = "140.WorkCheck.htm";
            break;
        case FrmComponents.FlowBBS:
            roleName = "15.FlowBBS.htm";
            break;
        case FrmComponents.Fiexed:
            roleName = "16.Fiexed.htm";
            break;
        case FrmComponents.GovDocFile: //公文正文组件.
            roleName = "110.GovDocFile.htm";
            break;
        case FrmComponents.DocWord:
            roleName = "17.DocWord.htm";
            break;
        case FrmComponents.DocWordReceive:
            roleName = "170.DocWordReceive.htm";
            break;
        case FrmComponents.Btn:
            roleName = "18.Btn.htm";
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
        case FrmComponents.Frame:
            roleName = "90.Frame.htm";
            break;
        case FrmComponents.Score:
            roleName = "101.Score.htm";
            break;
        case FrmComponents.SubFlow:
            roleName = "120.SubFlow.htm";
            break;
        default:
            roleName = "4.Map.htm";
            break;
    }

    SetHref("./" + roleName + "?FK_MapData=" + GetQueryString("FK_MapData") + "&FrmType=" + frmType);
}


function Save() {
    //保存控件
    var frmComponent = $("#changBar").val();
    switch (parseInt(frmComponent)) {
        case 4: //地图控件
            return ExtMap();
            break;
        case 5://录音控件
            break;
        case 6: //字段附件
            return ExtAth();
        case 7:
            break;
        case 8://签字版
            return ExtHandWriting();
        case 9://超链接
            return ExtLink();
        case 10://文本
            break;
        case 11://图片
            return ExtImg();
        case 12://图片附件
            return ExtImgAth();
        case 13://身份证
            return ExtIDCard();
        case 14://签批组件
            return ExtWorkCheck();
        case 140://审核组件
            var mypk = GetQueryString("FK_Node");
            if (frmType != 8 && (mypk == null || mypk == undefined)) {
                SetHref('../../Comm/EnOnly.htm?EnName=BP.WF.Template.NodeWorkCheck&PKVal=' + mypk + '&tab=审核组件');
                return;
            }

            if (mypk == null || mypk == undefined) {
                var _html = "<img src='../CCFormDesigner/Controls/DataView/FrmCheck.png' style='width:67%;height:200px'  leipiplugins='component'  data-type='WorkCheck'/>"
                editor.execCommand('insertHtml', _html);
                return;
            }
            var node = new Entity("BP.WF.WF_Node", mypk);
            node.FWCSta = 1;
            node.Update;
            var _html = "<img src='../CCFormDesigner/Controls/DataView/FrmCheck.png' style='width:67%;height:200px'  leipiplugins='component'  data-type='WorkCheck'/>"
            editor.execCommand('insertHtml', _html);
            return;
        case 15://评论组件
            return ExtFlowBBS();
        case 16://系统定位
            return MapAttrFixed();
        case 17:// 发文字号
            return ExtDocWord();
        case 170:// 收文字号
            return DocWordReceive();
        case 18://按钮
            return ExtBtn();
            break;
        case 50://流程进度图
            return ExtJobSchedule();
        case 60://大文本
            return ExtBigNoteHtmlText();
        case 70://独立附件
            return MultiAth();
            break;
        case 80://从表
            return CreateDtl();
            break;
        case 90: //框架
            return CreateFrame();
            break;
        case 101: //评分控件
            return ExtScore();
        case 110: //公文正文组件
            return ExtGovDocFile();
        case 111: //打印组件
            return PrintRTF();
        case 120: //公文正文组件
            return SubFlow();
        default:
            break;
    }
    return "";
}

function SubFlow() {
    return GetHtmlByMapAttrAndFrmComponent(null, 120)
}

//地图
function ExtMap() {

    var name = promptGener('请输入地图名称:\t\n比如:中国地图', '地图');
    if (name == null || name == undefined || name.trim() == "")
        return "";

    var frmID = fk_mapData;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtMap();
        return "";
    }

    //获得ID.
    var id = StrToPinYin(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        ExtMap();
        return "";
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
    if (frmType != 8)
        SetHref('../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtMap&MyPK=' + mapAttr.MyPK);
    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(mapAttr, 4)
    }
}


//公文发文字号
function ExtDocWord() {

    var en = new Entity("BP.Sys.MapAttr");
    en.SetPKVal(fk_mapData + "_DocWord");
    if (en.RetrieveFromDBSources() == 1) {
        alert("该表单 DocWord 字段已经存在，发文字号默认的字段 DocWord ,请确认该字段是否为公文字段");
        return "";
    }

    var mypk = fk_mapData + "_DocWord";
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 17; //发文字号.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = fk_mapData;
    mapAttr.KeyOfEn = "DocWord";
    mapAttr.Name = "发文字号";
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; //
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 23;
    mapAttr.Insert(); //插入字段.
    if (frmType != 8)
        SetHref("../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrDocWord&MyPK=" + mapAttr.MyPK);

    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(mapAttr, 17);
    }
}
/**
 * 收文字号
 */
function DocWordReceive() {
    var en = new Entity("BP.Sys.MapAttr");
    en.SetPKVal(fk_mapData + "_DocWordReceive");
    if (en.RetrieveFromDBSources() == 1) {
        alert("该表单 DocWordReceive 字段已经存在，收文字号默认的字段 DocWordReceive,请确认该字段是否为公文字段");
        return "";
    }

    var mypk = fk_mapData + "_DocWordReceive";
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 170; //收文字号.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = fk_mapData;
    mapAttr.KeyOfEn = "DocWordReceive";
    mapAttr.Name = "收文字号";
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; //
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 23;
    mapAttr.Insert(); //插入字段.
    if (frmType != 8)
        SetHref("../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrDocWordReceive&MyPK=" + mapAttr.MyPK);
    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(mapAttr, 170)
    }

}

/**
 * 在线wps编辑
 */
function ExtGovDocFile() {

    var name = promptGener('请输入在线编辑组件的名称:\t\n比如:正文', '');
    if (name == null || name == undefined || name.trim() == "")
        return "";

    var frmID = fk_mapData;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtGovDocFile();
        return "";
    }

    //获得ID.
    var id = StrToPinYin(name);

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        return "";
    }
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.GroupID = 1;
    mapAttr.UIContralType = 110; 
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 3; //
    mapAttr.LabelColSpan = 1; //
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 50;
    mapAttr.IsEnableInAPP = 1;
    mapAttr.Insert(); //插入字段.
    if (frmType != 8)
        SetHref("../../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttGovDocFile&MyPK=" + mapAttr.MyPK);
    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(mapAttr, 110)
    }
}

//签批组件
function ExtWorkCheck() {
    var name = promptGener('请输入签批组件的名称:\t\n比如:办公室意见、拟办意见', '');
    if (name == null || name == undefined || name.trim() == "")
        return "";

    var frmID = fk_mapData;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtWorkCheck();
        return "";
    }

    //获得ID.
    var id = StrToPinYin(name);

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        return "";
    }
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.GroupID = 1;
    mapAttr.UIContralType = 14; //签批意见
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 3; //
    mapAttr.LabelColSpan = 1; //
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 50;
    mapAttr.IsEnableInAPP = 1;
    mapAttr.Insert(); //插入字段.
    if (frmType != 8)
        SetHref("../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrCheck&MyPK=" + mapAttr.MyPK);
    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(mapAttr, 14)
    }

}
//评论组件
function ExtFlowBBS() {

    var mypk = fk_mapData + "_FlowBBS";
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert("字段FlowBBS已存在，一个表单中只能存在一个评论字段，请查看表单中是否已经存在评论组件");
        return "";
    }
    mapAttr.FK_MapData = fk_mapData;
    mapAttr.KeyOfEn = "FlowBBS";
    mapAttr.Name = "评论组件";
    mapAttr.GroupID = 1;
    mapAttr.UIContralType = 15; //评论组件
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 3;
    mapAttr.LabelColSpan = 1;
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 50;
    mapAttr.IsEnableInAPP = 1;
    mapAttr.Insert(); //插入字段.
    if (frmType != 8)
        SetHref("../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrFlowBBS&MyPK=" + mapAttr.MyPK);
    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(mapAttr, 15)
    }

}


//身份证
function ExtIDCard() {
    var IDCard = [{ No: "IDCardName", Name: "姓名" }, { No: "IDCardNo", Name: '身份证号' }, { No: "IDCardAddress", Name: "地址" }];
    var frmID = fk_mapData;
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
            mapAttr.LabelColSpan = 1;
            mapAttr.LGType = 0; //文本
            mapAttr.UIIsEnable = 0; //不可编辑
            mapAttr.UIIsInput = 1;//必填
            mapAttr.UIWidth = 150;
            mapAttr.UIHeight = 23;
            mapAttr.Insert(); //插入字段.
        } else {
            alert("字段" + IDCard[i].No + "已存在，请变更表单中的" + mapAttr.Name + "的编号");
            return "";

        }

    }
    if (frmType != 8)
        SetHref("../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrCard&MyPK=" + frmID + "_IDCardNo");
    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(mapAttr, 13)
    }
}

//系统定位（需要创建唯一)
function MapAttrFixed() {
    var mapAttr = new Entity("BP.Sys.FrmUI.MapAttrFixed");
    mapAttr.SetPKVal(fk_mapData + "_Fixed");
    if (mapAttr.RetrieveFromDBSources() == 0) {
        mapAttr.FK_MapData = fk_mapData;
        mapAttr.KeyOfEn = "Fixed";
        mapAttr.Name = "系统定位";
        mapAttr.GroupID = groupID;
        mapAttr.UIContralType = 16; //系统定位
        mapAttr.MyDataType = 1;
        mapAttr.ColSpan = 1;
        mapAttr.LabelColSpan = 1;
        mapAttr.LGType = 0; //文本
        mapAttr.UIIsEnable = 0; //不可编辑
        mapAttr.UIIsInput = 0;
        mapAttr.UIWidth = 150;
        mapAttr.UIHeight = 23;
        mapAttr.Insert(); //插入字段.
        alert("创建成功");
    } else {
        alert("表单" + GetQueryString("FK_MapData") + "已经存在系统定位按钮，不能重复创建");
        return "";
    }
    if (frmType != 8)
        SetHref("../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrFixed&MyPK=" + mapAttr.MyPK);
    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(mapAttr, 16)
    }

}

//附件.
function ExtAth() {
    var frmID = fk_mapData;

    var name = promptGener('请输入附件名称:\t\n比如:报送材料、报销资料', '附件');
    if (name == null || name == undefined || name.trim() == "") {
        alert("字段附件的名称不能为空");
        ExtAth();
        return "";
    }


    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + ']的附件已经存在.');
        ExtAth();
        return "";
    }
    //获得ID.
    var id = StrToPinYin(name);
    var id = promptGener('请输入附件编号:\t\n比如:BSCL、BXZL', id);
    if (id == null || id == undefined || id.trim() == "") {
        alert("字段附件的编号不能为空");
        ExtAth();
        return "";
    }


    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称为：[' + name + ']，编号为[' + id + ']的附件已经存在.');
        ExtAth();
        return "";
    }
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.GroupID = groupID;
    mapAttr.UIContralType = 6; //附件控件.
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 3; //
    mapAttr.LabelColSpan = 1; //
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 70;
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
    if (frmType != 8)
        SetHref("../../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&MyPK=" + en.MyPK);
    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(mapAttr, 6)
    }
}

//超链接.
function ExtLink() {

    var name = promptGener('请输入超链接名称:\t\n比如:我的连接、点击这里打开', '我的连接');
    if (name == null || name == undefined || name.trim() == "")
        return "";

    var frmID = fk_mapData;
    // alert(frmID + imgName);
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtLink();
        return "";
    }

    //获得ID.
    var id = StrToPinYin(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        ExtLink();
        return "";
    }

    var link = promptGener('请输入超链地址:\t\n比如:https://gitee.com/opencc', 'https://gitee.com/opencc');
    if (link == null || link == undefined)
        return "";

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
    if (frmType != 8)
        SetHref("../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtLink&MyPK=" + mapAttr.MyPK);
    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(mapAttr, 9)
    }
}

//评分控件
function ExtScore() {

    var name = promptGener('请输入评分事项名称:\t\n比如:快递速度，服务水平', '评分事项');
    if (name == null || name == undefined || name.trim() == "")
        return "";

    var frmID = fk_mapData;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtScore();
        return "";
    }

    //获得ID.
    var id = StrToPinYin(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        ExtScore();
        return "";
    }

    var score = promptGener('请设定总分:\t\n比如:5，10', '5');
    if (score == null || score == undefined)
        return "";

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
    if (frmType != 8)
        SetHref("../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtScore&MyPK=" + mapAttr.MyPK);
    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(mapAttr, 101)
    }
}
//大块文本
function ExtBigNoteHtmlText() {
    var name = promptGener('请输入字段名','大块提示文本');
    if (name == null || name == undefined || name.trim() == "")
        return "";

    var frmID = fk_mapData;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtLink();
        return "";
    }

    //获得ID.
    var id = StrToPinYin(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        ExtBigNoteHtmlText();
        return "";
    }

    
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 60; //大块文本.
    mapAttr.FK_MapData = frmID;
    mapAttr.Name = name;
    mapAttr.KeyOfEn = id;
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 4; //
    mapAttr.UIWidth = 0;
    mapAttr.UIHeight = 100;
    mapAttr.Idx = 0;
    mapAttr.Insert(); //插入字段.
    SetHref("../EditFExtContral/60.BigNoteHtmlText.htm?FrmID=" + fk_mapData + "&KeyOfEn=" + id);
}

//手写签名版.
function ExtHandWriting() {

    var name = promptGener('请输入签名版名称:\t\n比如:签字版、签名', '签字版');
    if (name == null || name == undefined || name.trim() == "")
        return "";

    var frmID = fk_mapData;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtHandWriting();
        return "";
    }

    //获得ID.
    var id = StrToPinYin(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        ExtHandWriting();
        return "";
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
    mapAttr.UIWidth = 70;
    mapAttr.UIHeight = 50;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();
    if (frmType != 8)
        SetHref("../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtHandWriting&MyPK=" + mapAttr.MyPK);
    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(mapAttr, 8)
    }
}

//按钮
function ExtBtn() {

    var name = promptGener('请输入按钮名称:\t\n比如:保存、发送');
    if (name == null || name == undefined || name.trim() == "")
        return "";

    var frmID = fk_mapData;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtBtn();
        return "";
    }

    //获得ID.
    var id = StrToPinYin(name);

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        return "";
    }

    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.GroupID = groupID;
    mapAttr.UIContralType = 18; //按钮
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 0; //
    mapAttr.LabelColSpan = 1; //
    mapAttr.IsEnableInAPP = 0;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();

    if (frmType != 8) {
        SetHref("../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmBtn&MyPK=" + en.MyPK);
        return;
    }

    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(mapAttr, 18)
    }
}

//流程进度图.
function ExtJobSchedule() {

    var name = "流程进度图";
    var id = "JobSchedule";
    var frmID = fk_mapData;

    //获得ID.
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert("已经存在，一个表单仅仅允许有一个流程进度图.");
        return "";
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
    if (frmType != 8)
        SetHref("../../../Comm/EnOnly.htm?EnName=BP.WF.Template.ExtJobSchedule&MyPK=" + mapAttr.MyPK);
    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(mapAttr, 50)
    }

}

//图片组件.
function ExtImg() {

    var name = promptGener('请输入图片名称:\t\n比如:肖像、头像、ICON、地图位置', '肖像');
    if (name == null || name == undefined || name.trim() == "")
        return "";

    var frmID = fk_mapData;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtImg();
        return "";
    }

    //获得ID.
    var id = StrToPinYin(name);

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        return "";
    }
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.GroupID = groupID;
    mapAttr.UIContralType = 11; //FrmImg 类型的控件
    mapAttr.MyDataType = 1;
    mapAttr.ColSpan = 0; //单元格.
    mapAttr.LGType = 0;
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();

    var en = new Entity("BP.Sys.FrmUI.ExtImg");
    en.MyPK = frmID + "_" + id;
    en.FK_MapData = frmID;
    en.KeyOfEn = id;

    en.ImgAppType = 0; //图片.
    en.FK_MapData = frmID;
    en.GroupID = mapAttr.GroupID; //设置分组列.
    en.Name = name;
    en.UIWidth = 150;
    en.UIHeight = 170;
    en.Insert(); //插入到数据库.
    if (frmType != 8)
        SetHref("../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtImg&MyPK=" + en.MyPK);
    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(en, 11)
    }
}


//图片附件.
function ExtImgAth() {

    var name = promptGener('请输入图片名称:\t\n比如:肖像、头像、ICON', '肖像');
    if (name == null || name == undefined || name.trim() == "")
        return "";

    var frmID = fk_mapData;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtImgAth();
        return "";
    }

    //获得ID.
    var id = StrToPinYin(name);

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        return "";
    }
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.GroupID = groupID;
    mapAttr.UIContralType = 12; //FrmImgAth 类型的控件.
    mapAttr.MyDataType = 1;
    mapAttr.ColSpan = 0; //单元格.
    mapAttr.LGType = 0;
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();

    var en = new Entity("BP.Sys.FrmUI.FrmImgAth");
    en.MyPK = frmID + "_" + id;
    en.FK_MapData = frmID;
    en.CtrlID = id;
    en.Name = name;
    en.GroupID = mapAttr.GroupID; //设置分组列.

    en.Insert(); //插入到数据库.
    if (frmType != 8)
        SetHref("../../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmImgAth&MyPK=" + en.MyPK);
    if (frmType == 8) {
        return GetHtmlByMapAttrAndFrmComponent(en, 12);
    }
}

function MultiAth() {
    var val = prompt('请输入附件ID:(要求是字母数字下划线，非中文等特殊字符.)', 'Ath1');
    if (val == null) {
        return "";
    }

    if (val.trim() == '') {
        alert('附件ID不能为空，请重新输入！');
        return "";
    }

    //秦 18.11.16
    if (!CheckID(val)) {
        alert("编号不符合规则");
        return "";
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
    handler.AddPara("FK_MapData", fk_mapData);
    handler.AddPara("AthNo", val);
    var data = handler.DoMethodReturnString("Designer_AthNew");

    if (data.indexOf('err@') == 0) {
        alert(data);
        return "";
    }
    if (frmType != 8)
        SetHref('../../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&FK_MapData=' + fk_mapData + '&MyPK=' + data);
    if (frmType == 8)
        return GetHtmlByMapAttrAndFrmComponent(data, 70);
}


function CreateDtl() {
    var val = prompt('请输入从表ID，要求表单唯一。', fk_mapData + 'Dtl1');

    if (val == null || val.trim() == "") {
        return;
    }

    //秦 18.11.16
    if (!CheckID(val)) {
        alert("编号不符合规则");
        return;
    }

    if (val == '' || val.trim() == "") {
        alert('请输入从表ID不能为空，请重新输入！');
        CreateDtl(fk_mapData);
        return;
    }

    var en = new Entity("BP.Sys.MapDtl");
    en.No = val;
    if (en.RetrieveFromDBSources() == 1) {
        alert("已经存在:" + val);
        return;
    }
    en.FK_Node = 0;
    en.PTable = en.No;
    en.Name = "从表" + en.No;
    en.FK_MapData = fk_mapData;
    en.H = 300;
    en.Insert();
    var data = en.DoMethodReturnString("IntMapAttrs");
     

    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }
    if (frmType != 8)
        SetHref('../../../Comm/En.htm?EnName=BP.WF.Template.Frm.MapDtlExt&FK_MapData=' + fk_mapData + '&No=' + en.No);
    if (frmType == 8)
        return GetHtmlByMapAttrAndFrmComponent( en.No, 80);
}

function CreateFrame() {
    var alert = "\t\n1.为了更好的支持应用扩展,ccform可以用iFrame的地图、流程轨迹图、轨迹表的展示。";
    alert += "\t\n2.在创建一个框架后，在框架属性里设置。";
    alert += "\t\n3.请输入框架ID,要求是字母数字下划线，非中文等特殊字符。";

    var val = promptGener('新建框架:' + alert, 'Frame1');

    if (val == null || val.trim() == "") {
        return "";
    }

    if (val == '') {
        alert('框架ID不能为空，请重新输入！');
        CreateFrame(fk_mapData);
        return;
    }

    var en = new Entity("BP.Sys.MapFrame");
    en.MyPK = fk_mapData + "_" + val;
    if (en.IsExits() == true) {
        alert("该编号[" + val + "]已经存在");
        return "";
    }

    en.FK_MapData = fk_mapData;
    en.Name = "我的框架" + val;
    en.FrameURL = 'MapFrameDefPage.htm';
    en.H = 200;
    en.W = 200;
    en.X = 100;
    en.Y = 100;
    en.Insert();
    if (frmType != 8)
        SetHref('../../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapFrameExt&FK_MapData=' + fk_mapData + '&MyPK=' + en.MyPK);

    if (frmType == 8)
        return GetHtmlByMapAttrAndFrmComponent(en, 90);
}

