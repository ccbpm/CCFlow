
var frmData = null;
function GenerFoolFrm(mapData, frmData) {

    frmData = frmData;
    var Sys_GroupFields = frmData.Sys_GroupField;

    var node = frmData.WF_Node;
    if (node != undefined)
        node = node[0];

    var tableWidth = 800; //  w - 40;
    var html = "<table style='width:" + tableWidth + "px;' >";
    var frmName = mapData.Name;

    //html += "<tr>";
    //html += "<td colspan=4 ><div style='float:left' ><img src='../../DataUser/ICON/LogBiger.png'  style='height:50px;' /></div><div style='float:right;padding:10px;bordder:none;width:70%;' ><center><h4><b>" + frmName + "</b></h4></center></div></td>";
    //html += "</tr>";

    //遍历循环生成 listview
    for (var i = 0; i < Sys_GroupFields.length; i++) {

        var gf = Sys_GroupFields[i];

        //从表..
        if (gf.CtrlType == 'Dtl') {


            var dtls = frmData.Sys_MapDtl;

            for (var k = 0; k < dtls.length; k++) {

                var dtl = dtls[k];

                if (dtl.No != gf.CtrlID)
                    continue;

                html += "<tr>";
                html += "  <th colspan=4>" + gf.Lab + "</th>";
                html += "</tr>";

                html += "<tr>";
                html += "  <td colspan='4' >";

                html += Ele_Dtl(dtl);

                html += "  </td>";
                html += "</tr>";
            }
            continue;
        }


        //附件类的控件.
        if (gf.CtrlType == 'Ath') {

            html += "<tr>";
            html += "  <th colspan=4>" + gf.Lab + "</th>";
            html += "</tr>";


            html += "<tr>";
            html += "  <td colspan='4'>";

            html += Ele_Attachment(frmData, gf);

            html += "  </td>";
            html += "</tr>";

            continue;
        }

        //审核组件,有节点信息,并且当前节点状态不是禁用的,就可以显示.
        if (gf.CtrlType == 'FWC' && node && node.FWCSta != 0) {

            html += "<tr>";
            html += "  <th colspan=4>" + gf.Lab + "</th>";
            html += "</tr>";

            html += "<tr>";
            html += "  <td colspan='4' >";

            html += Ele_FrmCheck(node);

            // html += figure_Template_FigureFrmCheck(node);

            html += "  </td>";
            html += "</tr>";

            continue;
        }

        //字段类的控件.
        if (gf.CtrlType == '' || gf.CtrlType == null) {

            html += "<tr>";
            html += "  <th colspan=4>" + gf.Lab + "</th>";
            html += "</tr>";

            html += InitMapAttr(frmData.Sys_MapAttr, frmData, gf.OID);
            continue;
        }
    }

    html += "</table>";

    //加入隐藏控件.
    for (var attr in frmData.Sys_MapAttr) {
        if (attr.UIVisable == 0) {
            var defval = ConvertDefVal(frmData, attr.DefVal, attr.KeyOfEn);
            html += "<input type='hidden' id='TB_" + attr.KeyOfEn + "' name='TB_" + attr.KeyOfEn + "' value='" + defval + "' />";
        }
    }

    $('#CCForm').html("").append(html);
}


//审核组件
function Ele_FrmCheck(wf_node) {

    //审核组键FWCSta Sta,FWC_X X,FWC_Y Y,FWC_H H, FWC_W W from WF_Node
    var sta = wf_node.FWCSta;

    var h = wf_node.FWC_H + 1000;

    var isReadonly = GetQueryString('IsReadonly');
    if (isReadonly != "1") {
        isReadonly = "0";
    }
    if (sta == 2)//只读
        isReadonly = "1";


    var src = "../WorkOpt/WorkCheck.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["OID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.OID;
    paras += "&IsReadonly=" + isReadonly;
    //alert(paras);

    src += "&r=q" + paras;
    var eleHtml = "<iframe width='100%' height='" + h + "' id='FWC' src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=no ></iframe>";
    return eleHtml;
}

//初始化 附件
function figure_Template_Attachment(frmData, gf) {

    var ath = frmData.Sys_FrmAttachment[0];
    if (ath == null)
        return "没有找到附件定义，请与管理员联系。";

    var eleHtml = '';
    //    if (ath.UploadType == 0) { //单附件上传 L4204
    //        return '';
    //    }
    var src = "";
    if (pageData.IsReadonly)
        src = "AttachmentUpload.htm?PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";
    else
        src = "AttachmentUpload.htm?PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK;

    eleHtml += "<iframe style='width:100%;height:" + ath.H + "px;' ID='Attach_" + ath.MyPK + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';

    return eleHtml;
}


//解析表单字段 MapAttr.
function InitMapAttr(Sys_MapAttr, frmData, groupID) {

    var html = "";
    var isDropTR = true;
    for (var i = 0; i < Sys_MapAttr.length; i++) {

        var attr = Sys_MapAttr[i];

        if (attr.GroupID != groupID || attr.UIVisible == 0)
            continue;

        var enable = attr.UIIsEnable == "1" ? "" : " ui-state-disabled";
        var defval = ConvertDefVal(frmData, attr.DefVal, attr.KeyOfEn);

        var lab = "";
        if (attr.UIContralType == 0)
            lab = "<label for='TB_" + attr.KeyOfEn + "' class='" + (attr.UIIsInput == 1 ? "mustInput" : "") + "'>" + attr.Name + "</label>";

        if (attr.UIContralType == 1)
            lab = "<label for='DDL_" + attr.KeyOfEn + "' class='" + (attr.UIIsInput == 1 ? "mustInput" : "") + "'>" + attr.Name + "</label>";

        if (attr.UIIsInput == 1 && attr.UIIsEnable == 1) {
            lab += " <span style='color:red' class='mustInput' data-keyofen='" + attr.KeyOfEn + "' >*</span>";
        }

        //        if (item.UIContralType == 2)
        //            lab = "<label for='CB_" + item.KeyOfEn + "' >" + item.Name + "</label>";

        //线性展示并且colspan=3
        if (attr.ColSpan == 3 || (attr.ColSpan == 4 && attr.UIHeight < 40)) {
            isDropTR = true;
            html += "<tr>";
            html += "<td  class='FDesc' style='width:120px;'>" + lab + "</td>";
            html += "<td ColSpan=3>";
            html += InitMapAttrOfCtrl(attr, enable, defval);
            html += "</td>";
            html += "</tr>";
            continue;
        }

        //线性展示并且colspan=4
        if (attr.ColSpan == 4) {
            isDropTR = true;
            html += "<tr>";
            html += "<td ColSpan='4'>" + lab + "</br>";
            html += InitMapAttrOfCtrl(attr, enable, defval);
            html += "</td>";
            html += "</tr>";
            continue;
        }

        if (isDropTR == true) {
            html += "<tr>";
            html += "<td class='FDesc' style='width:120px;'>" + lab + "</td>";
            html += "<td class='FContext'  >";
            html += InitMapAttrOfCtrl(attr, enable, defval);
            html += "</td>";
            isDropTR = !isDropTR;
            continue;
        }

        if (isDropTR == false) {
            html += "<td class='FDesc' style='width:120px;'>" + lab + "</td>";
            html += "<td class='FContext'>";
            html += InitMapAttrOfCtrl(attr, enable, defval);
            html += "</td>";
            html += "</tr>";
            isDropTR = !isDropTR;
            continue;
        }
    }
    return html;
}

function InitMapAttrOfCtrl(mapAttr) {

    var str = '';
    var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);

    var isInOneRow = false; //是否占一整行
    var islabelIsInEle = false; //

    var eleHtml = '';

    //外部数据源类型.
    if (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == "1") {

        //枚举类型.
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        return "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
    }

    //外键类型.
    if (mapAttr.LGType == "2" && mapAttr.MyDataType == "1") {

        //枚举类型.
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        return "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
    }

    //外部数据类型.
    if (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContral == 1) {

        //枚举类型.
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        return "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
    }


    //添加文本框 ，日期控件等
    //AppString
    if (mapAttr.MyDataType == "1" && mapAttr.UIContralType == 0) {  //不是外键

        if (mapAttr.UIHeight <= 40) //普通的文本框.
        {
            var enableAttr = '';
            if (mapAttr.UIIsEnable == 0)
                enableAttr = "disabled='disabled'";

            return "<input maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "' style='width:100%;height:23px;' type='text'  " + enableAttr + " />";
        }

        if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {

            var eleHtml = "";
            //如果是富文本就使用百度 UEditor
            if (mapAttr.UIIsEnable == "0") {
                //只读状态直接 div 展示富文本内容
                //eleHtml += "<script id='" + editorPara.id + "' name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";
                //eleHtml += "<div class='richText' style='width:" + mapAttr.UIWidth + "px'>" + defValue + "</div>";
                eleHtml += "<div class='richText'>" + defValue + "</div>";
            } else {

                document.BindEditorMapAttr = mapAttr; //存到全局备用.

                //设置编辑器的默认样式
                var styleText = "text-align:left;font-size:12px;";
                styleText += "width:100%;";
                styleText += "height:" + mapAttr.UIHeight + "px;";
                //注意这里 name 属性是可以用来绑定表单提交时的字段名字的
                eleHtml += "<script id='editor' name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";
            }

            eleHtml = "<div style='white-space:normal;'>" + eleHtml + "</div>";
            return eleHtml
        }

        //普通的大块文本.
        return "<textarea maxlength=" + mapAttr.MaxLen + " style='height:" + mapAttr.UIHeight + "px;width:100%;' name='TB_" + mapAttr.KeyOfEn + "' type='text'  " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + " />"
    }

    //日期类型.
    if (mapAttr.MyDataType == 6) {
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return "<input " + enableAttr + " style='width:80px;' name='TB_" + mapAttr.KeyOfEn + "' type='text' />";
    }

    //时期时间类型.
    if (mapAttr.MyDataType == 7) {

        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return "<input  type='text'  style='width:120px;' " + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' />";
    }

    // boolen 类型.
    if (mapAttr.MyDataType == 4) {  // AppBoolean = 7

        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        //CHECKBOX 默认值
        var checkedStr = '';
        if (checkedStr != "true" && checkedStr != '1') {
            checkedStr = ' checked="checked" ';
        }

        checkedStr = ConvertDefVal(frmData, '', mapAttr.KeyOfEn);

        return "<input " + enableAttr + " " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' id='CB_" + mapAttr.KeyOfEn + "'  name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + " /><label for='CB_" + mapAttr.KeyOfEn + "' >" + mapAttr.Name + "</label>";
    }

    //枚举类型.
    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        return "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
    }

    // AppDouble  AppFloat
    if (mapAttr.MyDataType == 5 || mapAttr.MyDataType == 3) {

        var enableAttr = '';
        if (mapAttr.UIIsEnable != 1)
            enableAttr = "disabled='disabled'";

        // alert(mapAttr.KeyOfEn);
        return "<input style='text-align:right;width:80px;'  onkeyup=" + '"' + "if(isNaN(value)) execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    if ((mapAttr.MyDataType == 2)) { //AppInt
        var enableAttr = '';
        if (mapAttr.UIIsEnable != 1) {
            enableAttr = "disabled='disabled'";
        }

        return "<input style='text-align:right;width:80px;' onkeyup=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    //AppMoney  AppRate
    if (mapAttr.MyDataType == 8) {
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1) {

        } else {
            enableAttr = "disabled='disabled'";
        }
        return "<input style='text-align:right;width:80px;' onkeyup=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    alert(mapAttr.Name + "的类型没有判断.");
    return;
}


//初始化 附件
function Ele_Attachment(workNode, gf) {

    var ath = workNode.Sys_FrmAttachment[0];
    if (ath == null)
        return "没有找到附件定义，请与管理员联系。";

    var eleHtml = '';
    //    if (ath.UploadType == 0) { //单附件上传 L4204
    //        return '';
    //    }

    var pkval = GetQueryString("WorkID");
    if (pkval == undefined)
        pkval = GetQueryString("OID");

    var src = "";
    if (pageData.IsReadonly)
        src = "AttachmentUpload.htm?PKVal=" + pkval + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";
    else
        src = "AttachmentUpload.htm?PKVal=" + pkval + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK;

    eleHtml += "<iframe style='width:100%;height:" + ath.H + "px;' ID='Attach_" + ath.MyPK + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';

    return eleHtml;
}


var appPath = "../../";
var DtlsCount = " + dtlsCount + "; //应该加载的明细表数量

//初始化从表
function Ele_Dtl(frmDtl) {
    var src = "";
    var href = window.location.href;
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');
    urlParam = urlParam.replace('EnsName=' + frmDtl.FK_MapData, '');
    urlParam = urlParam.replace('&RefPKVal=' + GetQueryString('RefPKVal'), '');

    urlParam = "";
    //alert(urlParam);

    var refPK = GetQueryString('OID');
    if (refPK == null)
        refPK = GetQueryString('WorkID');

    var isReadonly = GetQueryString("IsReadonly");
    if (isReadonly == "null" || isReadonly == "0" || isReadonly == null || isReadonly == undefined)
        isReadonly = "0";
    else
        isReadonly = "1";

    if (frmDtl.ListShowModel == "0") {
        src = "Dtl.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + refPK + "&IsReadonly=" + isReadonly + "&" + urlParam + "&Version=1";
    }
    else if (frmDtl.ListShowModel == "1") {
        src = "DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + refPK + "&IsReadonly=" + isReadonly + "&" + urlParam + "&Version=1";
    }

    return "<iframe style='width:100%;height:" + frmDtl.H + "px;' ID='" + frmDtl.No + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
}