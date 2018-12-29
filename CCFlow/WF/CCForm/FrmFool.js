
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


    //表单联动设置
    Set_Frm_Enable(frmData);

}


function Set_Frm_Enable(frmData) {
    var mapAttrs = frmData.Sys_MapAttr;
    //解析设置表单字段联动显示与隐藏.
    for (var i = 0; i < mapAttrs.length; i++) {

        var mapAttr = mapAttrs[i];
        if (mapAttr.UIVisible == 0)
            continue;

        if (mapAttr.LGType != 1)
            continue;

        if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {  // AppInt Enum
            if (mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
                if (mapAttr.UIContralType == 1) {
                    /*启用了显示与隐藏.*/
                    var ddl = $("#DDL_" + mapAttr.KeyOfEn);
                    //初始化页面的值
                    var nowKey = ddl.val();


                    setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey);

                }
                if (mapAttr.UIContralType == 3) {
                    /*启用了显示与隐藏.*/
                    var rb = $("#RB_" + mapAttr.KeyOfEn);
                    var nowKey = $('input[name="RB_' + mapAttr.KeyOfEn + '"]:checked').val();
                    setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey);

                }
            }
        }

    }
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
        if (attr.UIContralType == 0 || attr.UIContralType == 8)
            lab = "<label id=Lab_"+attr.KeyOfEn + "'  for='TB_" + attr.KeyOfEn + "' class='" + (attr.UIIsInput == 1 ? "mustInput" : "") + "'>" + attr.Name + "</label>";

        if (attr.UIContralType == 1)
            lab = "<label id=Lab_" + attr.KeyOfEn + "' for='DDL_" + attr.KeyOfEn + "' class='" + (attr.UIIsInput == 1 ? "mustInput" : "") + "'>" + attr.Name + "</label>";

        if (attr.UIIsInput == 1 && attr.UIIsEnable == 1) {
            lab += " <span style='color:red' class='mustInput' data-keyofen='" + attr.KeyOfEn + "' >*</span>";
        }

        if (attr.UIContralType == 3)
            lab = "<label id=Lab_" + attr.KeyOfEn + "' for='RB_" + attr.KeyOfEn + "' class='" + (attr.UIIsInput == 1 ? "mustInput" : "") + "'>" + attr.Name + "</label>";

        //线性展示并且colspan=3
        if (attr.ColSpan == 3 || (attr.ColSpan == 4 && attr.UIHeight < 40)) {
            isDropTR = true;
            html += "<tr>";
            html += "<td  class='FDesc' style='width:120px;'>" + lab + "</td>";
            html += "<td id='Td_" + attr.KeyOfEn + "' ColSpan=3>";
            html += InitMapAttrOfCtrl(attr, enable, defval);
            html += "</td>";
            html += "</tr>";
            continue;
        }

        //线性展示并且colspan=4
        if (attr.ColSpan == 4) {
            isDropTR = true;
            html += "<tr>";
            html += "<td id='Td_" + attr.KeyOfEn + "' ColSpan='4'>" + lab + "</br>";
            html += InitMapAttrOfCtrl(attr, enable, defval);
            html += "</td>";
            html += "</tr>";
            continue;
        }

        if (isDropTR == true) {
            html += "<tr>";
            html += "<td class='FDesc' style='width:120px;'>" + lab + "</td>";
            html += "<td id='Td_" + attr.KeyOfEn + "' class='FContext'  >";
            html += InitMapAttrOfCtrl(attr, enable, defval);
            html += "</td>";
            isDropTR = !isDropTR;
            continue;
        }

        if (isDropTR == false) {
            html += "<td class='FDesc' style='width:120px;'>" + lab + "</td>";
            html += "<td id='Td_" + attr.KeyOfEn + "' class='FContext'>";
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

        //判断外键是否为树形结构
        var uiBindKey = mapAttr.UIBindKey;
        if (uiBindKey != null && uiBindKey != undefined && uiBindKey != "") {
            var sfTable = new Entity("BP.Sys.SFTable");
            sfTable.SetPKVal(uiBindKey);
            var count = sfTable.RetrieveFromDBSources();
            if (count!=0 && sfTable.CodeStruct == "1") {
                return "<select  id='DDL_" + mapAttr.KeyOfEn + "' class='easyui-combotree' style='width:" + parseInt(mapAttr.UIWidth) * 2 + "px;height:28px'></select>";
            }
        }

        return "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
    }

    //外部数据类型.
    if (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1) {

        //枚举类型.
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        if (mapAttr.UIContralType == 1)
            return "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
        if (mapAttr.UIContralType == 3) {
            //横向排列
            var RBShowModel = 3;
            if (mapAttr.AtPara.indexOf("@RBShowModel=3") == -1)
                RBShowModel = 0;
            return InitRBShowContent(frmData, mapAttr, defValue, RBShowModel, enableAttr);

        }
    }


    //添加文本框 ，日期控件等
    //AppString
    if (mapAttr.MyDataType == "1" && mapAttr.UIContralType == 0) {  //不是外键

        if (mapAttr.UIHeight <= 40) //普通的文本框.
        {
            //如果是图片签名，并且可以编辑
            if (mapAttr.IsSigan == "1" && mapAttr.UIIsEnable == 1) {
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' value='" + defValue + "' type=hidden />";
                //是否签过
                var sealData = new Entities("BP.Tools.WFSealDatas");
                sealData.Retrieve("OID", GetQueryString("WorkID"), "FK_Node", GetQueryString("FK_Node"), "SealData", GetQueryString("UserNo"));

                if (sealData.length > 0) {
                    eleHtml += "<img src='../../DataUser/Siganture/" + defValue + ".jpg' onerror=\"this.src='../../DataUser/Siganture/UnName.jpg'\"  style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                    isSigantureChecked = true;
                }
                else {
                    eleHtml += "<img src='../../DataUser/Siganture/siganture.jpg' onerror=\"this.src='../../DataUser/Siganture/UnName.jpg'\" ondblclick='figure_Template_Siganture(\"" + mapAttr.KeyOfEn + "\",\"" + defValue + "\")' style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                }
                return eleHtml;
            }
            //如果不可编辑，并且是图片名称
            if (mapAttr.IsSigan == "1") {
                var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' type=hidden />";
                eleHtml += "<img src='../../DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='../../DataUser/Siganture/siganture.jpg'\" style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                return eleHtml;
            }

            var enableAttr = '';
            if (mapAttr.UIIsEnable == 0)
                enableAttr = "disabled='disabled'";

            return "<input maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "' style='width:100%;height:23px;' type='text'  " + enableAttr + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
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
    if (mapAttr.MyDataType == "1" && mapAttr.UIContralType == 8) {
        //如果是图片签名，并且可以编辑
        var ondblclick = ""
        if (mapAttr.UIIsEnable == 1) {
            ondblclick = " ondblclick='figure_Template_HandWrite(\"" + mapAttr.KeyOfEn + "\",\"" + defValue + "\")'";
        }

        var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' value='" + defValue + "' type=hidden />";
        eleHtml += "<img src='" + defValue + "' " + ondblclick + " onerror=\"this.src='../../DataUser/Siganture/UnName.jpg'\"  style='border:0px;width:" + mapAttr.UIWidth + "px;height:" + mapAttr.UIHeight + "px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
        return eleHtml;
    }

    //日期类型.
    if (mapAttr.MyDataType == 6) {
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return "<input " + enableAttr + " style='width:120px;' name='TB_" + mapAttr.KeyOfEn + "' type='text' class='Wdate'   placeholder='" + (mapAttr.Tip || '') + "'/>";
    }

    //时期时间类型.
    if (mapAttr.MyDataType == 7) {

        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return "<input class='Wdate'  type='text'  style='width:160px;' " + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "' />";
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
        //枚举类型.
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        if (mapAttr.UIContralType == 1)
            return "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
        if (mapAttr.UIContralType == 3) {
            //横向排列
            var RBShowModel = 3;
            if (mapAttr.AtPara.indexOf("@RBShowModel=3") == -1)
                RBShowModel = 0;
            return InitRBShowContent(frmData, mapAttr, defValue, RBShowModel, enableAttr);
        }

        /*if (mapAttr.UIIsEnable == 1)
        enableAttr = "";
        else
        enableAttr = "disabled='disabled'";

        return "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
        */
    }

    // AppDouble  AppFloat
    if (mapAttr.MyDataType == 5 || mapAttr.MyDataType == 3) {

        var enableAttr = '';
        if (mapAttr.UIIsEnable != 1)
            enableAttr = "disabled='disabled'";

        //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
        var defVal = mapAttr.DefVal;
        var bit;
        if (defVal != null && defVal !== "" && defVal.indexOf(".") >= 0)
            bit = defVal.substring(defVal.indexOf(".") + 1).length;

        // alert(mapAttr.KeyOfEn);
        return "<input style='text-align:right;width:125px;'  onkeyup=" + '"' + "if(!(value.indexOf('-')==0&&value.length==1)&&isNaN(value)) execCommand('undo');limitLength(this," + bit + ");" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
    }

    if ((mapAttr.MyDataType == 2)) { //AppInt
        var enableAttr = '';
        if (mapAttr.UIIsEnable != 1) {
            enableAttr = "disabled='disabled'";
        }

        return "<input style='text-align:right;width:125px;' onkeyup=" + '"' + "if(!(value.indexOf('-')==0&&value.length==1)&&isNaN(value)) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
    }

    //AppMoney  AppRate
    if (mapAttr.MyDataType == 8) {
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1) {

        } else {
            enableAttr = "disabled='disabled'";
        }
        return "<input style='text-align:right;width:125px;' onkeyup=" + '"' + "if(isNaN(value))execCommand('undo');" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' value='0.00' placeholder='" + (mapAttr.Tip || '') + "'/>";
    }

    alert(mapAttr.Name + "的类型没有判断.");
    return;
}

//记录改变字段样式 不可编辑，不可见
var mapAttrs = [];
function changeEnable(obj, FK_MapData, KeyOfEn, AtPara) {
    if (AtPara.indexOf('@IsEnableJS=1') >= 0) {
        var selecedval = $(obj).children('option:selected').val();  //弹出select的值.
        cleanAll();
        setEnable(FK_MapData, KeyOfEn, selecedval);
    }
}
function clickEnable(obj, FK_MapData, KeyOfEn, AtPara) {
    if (AtPara.indexOf('@IsEnableJS=1') >= 0) {
        var selectVal = $(obj).val();
        cleanAll();
        setEnable(FK_MapData, KeyOfEn, selectVal);
    }
}

//清空所有的设置
function cleanAll() {
    for (var i = 0; i < mapAttrs.length; i++) {
        SetCtrlShow(mapAttrs[i]);
        SetCtrlEnable(mapAttrs[i]);
        CleanCtrlVal(mapAttrs[i]);
    }

}
//启用了显示与隐藏.
function setEnable(FK_MapData, KeyOfEn, selectVal) {
    var pkval = FK_MapData + "_" + KeyOfEn + "_" + selectVal;
    var frmRB = new Entity("BP.Sys.FrmRB", pkval);


    //解决字段隐藏显示.
    var cfgs = frmRB.FieldsCfg;

    //解决为其他字段设置值.
    var setVal = frmRB.SetVal;
    if (setVal) {
        var strs = setVal.split('@');

        for (var i = 0; i < strs.length; i++) {

            var str = strs[i];
            var kv = str.split('=');

            var key = kv[0];
            var value = kv[1];
            SetCtrlVal(key, value);
            mapAttrs.push(key);

        }
    }
    //@Title=3@OID=2@RDT=1@FID=3@CDT=2@Rec=1@Emps=3@FK_Dept=2@FK_NY=3
    if (cfgs) {

        var strs = cfgs.split('@');

        for (var i = 0; i < strs.length; i++) {

            var str = strs[i];
            var kv = str.split('=');

            var key = kv[0];
            var sta = kv[1];

            if (sta == 0)
                continue; //什么都不设置.


            if (sta == 1) {  //要设置为可编辑.
                SetCtrlShow(key);
                SetCtrlEnable(key);
            }

            if (sta == 2) { //要设置为不可编辑.
                SetCtrlShow(key);
                SetCtrlUnEnable(key);
                mapAttrs.push(key);
            }

            if (sta == 3) { //不可见.
                SetCtrlHidden(key);
                mapAttrs.push(key);
            }

        }


    }


}

//设置是否可以用?
function SetCtrlEnable(key) {

    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.removeAttr("disabled");
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        ctrl.removeAttr("disabled");
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {
        ctrl.removeAttr("disabled");
    }

    ctr = document.getElementsByName('RB_' + key);
    if (ctrl != null) {
        var ses = new Entities("BP.Sys.SysEnums");
        ses.Retrieve("EnumKey", key);
        for (var i = 0; i < ses.length; i++)
            $("#RB_" + key + "_" + ses[i].IntKey).removeAttr("disabled");
    }
}
function SetCtrlUnEnable(key) {

    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.attr("disabled", "true");
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        ctrl.attr("disabled", "disabled");
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {

        ctrl.attr("disabled", "disabled");
    }

    ctrl = $("#RB_" + key);
    if (ctrl != null) {
        $('input[name=RB_' + key + ']').attr("disabled", "disabled");
        //ctrl.attr("disabled", "disabled");
    }
}
//设置隐藏?
function SetCtrlHidden(key) {
    ctrl = $("#Lab_" + key);
    if (ctrl.length > 0)
        ctrl.parent('tr').hide();

    var ctrl = $("#Td_" + key);
    if (ctrl.length > 0) {
        ctrl.parent('tr').hide();
    }


}
//设置显示?
function SetCtrlShow(key) {
    var ctrl = $("#Td_" + key);
    if (ctrl.length > 0) {
        ctrl.parent('tr').show();
    }

    ctrl = $("#Lab_" + key);
    if (ctrl.length > 0) {
        ctrl.parent('tr').show();
    }
   

}

//设置值?
function SetCtrlVal(key, value) {
    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.val(value);
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        ctrl.val(value);
        // ctrl.attr("value",value);
        //$("#DDL_"+key+" option[value='"+value+"']").attr("selected", "selected");
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {
        ctrl.val(value);
        ctrl.attr('checked', true);
    }

    ctrl = $("#RB_" + key + "_" + value);
    if (ctrl.length > 0) {
        var checkVal = $('input:radio[name=RB_' + key + ']:checked').val();
        document.getElementById("RB_" + key + "_" + checkVal).checked = false;
        document.getElementById("RB_" + key + "_" + value).checked = true;
        // ctrl.attr('checked', 'checked');
    }
}

//清空值?
function CleanCtrlVal(key) {
    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.val('');
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        //ctrl.attr("value",'');
        ctrl.val('');
        // $("#DDL_"+key+" option:first").attr('selected','selected');
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {
        ctrl.attr('checked', false); ;
    }

    ctrl = $("#RB_" + key + "_" + 0);
    if (ctrl.length > 0) {
        ctrl.attr('checked', true);
    }
}


//初始化 附件
function Ele_Attachment(workNode, gf) {

    var eleHtml = '';
    var nodeID = GetQueryString("FK_Node");
    var url = "";
    url += "&WorkID=" + GetQueryString("WorkID");
    url += "&FK_Node=" + GetQueryString("FK_Node");
    url += "&FK_Flow=" + GetQueryString("FK_Flow");
    url += "&FormType=" + GetQueryString("FormType"); //表单类型，累加表单，傻瓜表单，自由表单.

    var nodeID = GetQueryString("FK_Node");
    var no = nodeID.substring(nodeID.length - 2);
    var IsStartNode = 0;
    if (no == "01")
        url += "&IsStartNode=1"; //是否是开始节点

    var isReadonly = false;
    if (gf.FrmID.indexOf(nodeID) == -1)
        isReadonly = true;


    //创建附件描述信息.
    var ath = new Entity("BP.Sys.FrmAttachment", gf.CtrlID);

    var athPK = gf.CtrlID;
    var noOfObj = athPK.replace(gf.FrmID + "_", "");

    var src = "";
    
    //这里的连接要取 FK_MapData的值.
    src = "Ath.htm?PKVal=" + GetQueryString("PKVal") + "&Ath=" + noOfObj + "&FK_MapData=" + GetQueryString("FK_MapData") + "&FromFrm=" + gf.FrmID + "&FK_FrmAttachment=" + athPK + url;

    //自定义表单模式.
    if (ath.AthRunModel == 2) {
        src = "../../DataUser/OverrideFiles/Ath.htm?PKVal=" + GetQueryString("PKVal") + "&Ath=" + noOfObj + "&FK_MapData=" + gf.FrmID + "&FK_FrmAttachment=" + athPK + url;
    }

    eleHtml += "<iframe style='width:100%;height:" + ath.H + "px;' ID='Attach_" + gf.CtrlID + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    return eleHtml;

    /*

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
    src = "Ath.htm?PKVal=" + pkval + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";
    else
    src = "Ath.htm?PKVal=" + pkval + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK;

    eleHtml += "<iframe style='width:100%;height:" + ath.H + "px;' ID='Attach_" + ath.MyPK + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';

    return eleHtml; */
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
        src = "Dtl.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + refPK + "&IsReadonly=" + isReadonly + "&FK_MapData=" + frmDtl.FK_MapData + "&" + urlParam + "&Version=1";
    }
    else if (frmDtl.ListShowModel == "1") {
        src = "DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + refPK + "&IsReadonly=" + isReadonly + "&FK_MapData=" + frmDtl.FK_MapData + "&" + urlParam + "&Version=1";
    }

    return "<iframe style='width:100%;height:" + frmDtl.H + "px;' ID='" + frmDtl.No + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
}

function InitRBShowContent(flowData, mapAttr, defValue, RBShowModel, enableAttr) {
    var rbHtml = "";
    var enums = flowData.Sys_Enum;
    enums = $.grep(enums, function (value) {
        return value.EnumKey == mapAttr.UIBindKey;
    });
    $.each(enums, function (i, obj) {
        if (RBShowModel == 3)
        //<input  " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' id='CB_" + mapAttr.KeyOfEn + "'  name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + " /> &nbsp;" + mapAttr.Name + "</label</div>";
            rbHtml += "<label><input " + enableAttr + " " + (obj.IntKey == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "' />&nbsp;" + obj.Lab + "</label>";
        else
            rbHtml += "<label><input " + enableAttr + " " + (obj.IntKey == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "'  />&nbsp;" + obj.Lab + "</label><br/>";
    });
    return rbHtml;
}

