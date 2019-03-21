
var flowData = null;

function GenerFoolFrm(wn) {

    flowData = wn;

    //初始化Sys_MapData
    var h = flowData.Sys_MapData[0].FrmH;
    var w = flowData.Sys_MapData[0].FrmW;
    var node = flowData.WF_Node[0];

    $('#CCForm').html('');

    var tableWidth = w - 40;
    var html = "<table style='width:100%;' >";

    var frmName = flowData.Sys_MapData[0].Name;
    var Sys_GroupFields = flowData.Sys_GroupField;

    html += "<tr>";
    html += "<td colspan=4 ><div style='float:left' ><img src='../DataUser/ICON/LogBiger.png'  style='height:50px;' /></div><div style='float:right;padding:10px;bordder:none;width:70%;' ><center><h4><b>" + frmName + "</b></h4></center></div></td>";
    //  html += "<td colspan=2 ></td>";
    html += "</tr>";

    //遍历循环生成 listview
    for (var i = 0; i < Sys_GroupFields.length; i++) {

        var gf = Sys_GroupFields[i];

        //从表..
        if (gf.CtrlType == 'Dtl' ) {

            html += "<tr>";
            html += "  <th colspan=4>" + gf.Lab + "</th>";
            html += "</tr>";

            var dtls = flowData.Sys_MapDtl;

            for (var k = 0; k < dtls.length; k++) {

                var dtl = dtls[k];
                if (dtl.No != gf.CtrlID)
                    continue;

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
            html += "  <td colspan='4' >";

            html += Ele_Attachment(flowData, gf,node);

            html += "  </td>";
            html += "</tr>";

            continue;
        }


        //框架类的控件.
        if (gf.CtrlType == 'Frame') {
           
            html += "<tr>";
            html += "  <th colspan=4>" + gf.Lab + "</th>";
            html += "</tr>";

            html += "<tr>";
            html += "  <td colspan='4' >";

            html += Ele_Frame(flowData, gf);

            html += "  </td>";
            html += "</tr>";

            continue;
        }

        //审核组件..
        if (gf.CtrlType == 'FWC' && node.FWCSta != 0) {

            html += "<tr>";
            html += "  <th colspan=4>" + gf.Lab + "</th>";
            html += "</tr>";

            html += "<tr>";
            html += "  <td colspan='4' >";

            html += Ele_FrmCheck(node);

            html += "  </td>";
            html += "</tr>";

            continue;
        }


        //字段类的控件.
        if (gf.CtrlType == '' || gf.CtrlType == null) {

            html += "<tr>";
            html += "  <th colspan=4>" + gf.Lab + "</th>";
            html += "</tr>";

            html += InitMapAttr(flowData.Sys_MapAttr, flowData, gf.OID);
            continue;
        }

        //父子流程
        if (gf.CtrlType == 'SubFlow') {
            html += "<tr>";
            html += "  <th colspan=4>" + gf.Lab + "</th>";
            html += "</tr>";

            html += "<tr>";
            html += "  <td colspan='4' >";

            html += Ele_SubFlow(node);

            html += "  </td>";
            html += "</tr>";

            continue;
        }
    }

    html += "</table>";

    $('#CCForm').html(html);

}

//解析表单字段 MapAttr.
function InitMapAttr(Sys_MapAttr, flowData, groupID) {

    var html = "";
    var isDropTR = true;
    for (var i = 0; i < Sys_MapAttr.length; i++) {

        var attr = Sys_MapAttr[i];

        if (attr.GroupID != groupID || attr.UIVisible == 0)
            continue;

        var enable = attr.UIIsEnable == "1" ? "" : " ui-state-disabled";
        var defval = ConvertDefVal(flowData, attr.DefVal, attr.KeyOfEn);

        var lab = "";
        if (attr.UIContralType == 0 || attr.UIContralType == 8)
            lab = "<label id='Lab_" + attr.KeyOfEn + "' for='TB_" + attr.KeyOfEn + "' class='" + (attr.UIIsInput == 1 ? "mustInput" : "") + "'>" + attr.Name + "</label>";

        if (attr.UIContralType == 1)
            lab = "<label id='Lab_" + attr.KeyOfEn + "' for='DDL_" + attr.KeyOfEn + "' class='" + (attr.UIIsInput == 1 ? "mustInput" : "") + "'>" + attr.Name + "</label>";

        if (attr.UIIsInput == 1 && attr.UIIsEnable == 1) {
            lab += " <span style='color:red' class='mustInput' data-keyofen='" + attr.KeyOfEn + "' >*</span>";
        }

        if (attr.UIContralType == 3)
            lab = "<label id='Lab_" + attr.KeyOfEn + "' for='RB_" + attr.KeyOfEn + "' class='" + (attr.UIIsInput == 1 ? "mustInput" : "") + "'>" + attr.Name + "</label>";

        //线性展示并且colspan=3
        if (attr.ColSpan == 3 || (attr.ColSpan == 4 && attr.UIHeight < 40)) {
            isDropTR = true;
            html += "<tr>";
            if (attr.MyDataType != 4 && attr.UIContralType !="9")
                html += "<td  class='FDesc' style='width:120px;'>" + lab + "</td>";
            if (attr.MyDataType != 4 && attr.UIContralType != "9")
                html += "<td  class='FDesc' id='Td_" + attr.KeyOfEn + "' ColSpan=3 >";
            else
                html += "<td  class='FDesc' id='Td_" + attr.KeyOfEn + "' ColSpan=4 >";

            html += InitMapAttrOfCtrlFool(flowData, attr, enable, defval);
            html += "</td>";
            html += "</tr>";
            continue;
        }

        //线性展示并且colspan=4
        if (attr.ColSpan == 4) {
            isDropTR = true;
            html += "<tr>";
            html += "<td  id='Td_" + attr.KeyOfEn + "' ColSpan='4'>" + lab + "</br>";
            html += InitMapAttrOfCtrlFool(flowData, attr, enable, defval);
            html += "</td>";
            html += "</tr>";
            continue;
        }

        if (isDropTR == true) {
            html += "<tr>";
            if (attr.UIContralType != "9") {
                html += "<td class='FDesc' style='width:120px;'>" + lab + "</td>";
                html += "<td id='Td_" + attr.KeyOfEn + "' class='FContext'  >";
            } else {
                html += "<td id='Td_" + attr.KeyOfEn + "' class='FContext' ColSpan=2 >";
            }
            html += InitMapAttrOfCtrlFool(flowData, attr, enable, defval);
            html += "</td>";
            isDropTR = !isDropTR;
            continue;
        }

        if (isDropTR == false) {
            if (attr.UIContralType != "9") {
                html += "<td class='FDesc' style='width:120px;'>" + lab + "</td>";
                html += "<td id='Td_" + attr.KeyOfEn + "' class='FContext'  >";
            } else {
                html += "<td id='Td_" + attr.KeyOfEn + "' class='FContext' ColSpan=2 >";
            }
            html += InitMapAttrOfCtrlFool(flowData, attr, enable, defval);
            html += "</td>";
            html += "</tr>";
            isDropTR = !isDropTR;
            continue;
        }
    }

    if (isDropTR == false) {
        html += "<td class='FDesc' ColSpan='2'></td>";
        html += "</tr>";
    }

    return html;
}


function InitMapAttrOfCtrlFool(flowData, mapAttr) {

    var str = '';
    var defValue = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);

    var isInOneRow = false; //是否占一整行
    var islabelIsInEle = false; //
    var eleHtml = '';

    //外部数据源类型.
    if (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1) {

        if (mapAttr.UIIsEnable == 0) {
            var ctrl = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' type=hidden  class='form-control' type='text'/>";

            defValue = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn + "T");

            if (defValue == '' || defValue == null)
                defValue = '无';

            ctrl += "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "Text'  value='" + defValue + "' disabled='disabled'   class='form-control' type='text'/>";
            return ctrl;
        }

        return "<select id='DDL_" + mapAttr.KeyOfEn + "' class='form-control'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(flowData, mapAttr, defValue) + "</select>";
    }

    //外键类型.
    if (mapAttr.LGType == "2" && mapAttr.MyDataType == "1") {

        var data = flowData[mapAttr.UIBindKey];
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
        return "<select id='DDL_" + mapAttr.KeyOfEn + "' class='form-control'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(flowData, mapAttr, defValue) + "</select>";
    }

    //添加文本框 ，日期控件等.
    //AppString
    if (mapAttr.MyDataType == "1") {  //不是外键

        //签字板
        if (mapAttr.UIContralType == "8") {
            //查找默认值
            var val = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
            //如果是图片签名，并且可以编辑
            var ondblclick = ""
            if (mapAttr.UIIsEnable == 1) {
                ondblclick = " ondblclick='figure_Template_HandWrite(\"" + mapAttr.KeyOfEn + "\",\"" + val + "\")'";
            }

            var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' type=hidden />";
            eleHtml += "<img src='" + val + "' " + ondblclick + " onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\"  style='border:0px;width:" + mapAttr.UIWidth + "px;height:" + mapAttr.UIHeight + "px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
            return eleHtml;
        }
        //超链接
        if (mapAttr.UIContralType == "9") {
            //URL @ 变量替换
            var url = GetPara(mapAttr.AtPara, "Url").replace(/[$]/g,'@');
            $.each(flowData.Sys_MapAttr, function (i, obj) {
                if (url != null && url.indexOf('@' + obj.KeyOfEn) > 0) {
                    url = url.replace('@' + obj.KeyOfEn, flowData.MainTable[0][obj.KeyOfEn]);
                }
            });

            var OID = GetQueryString("OID");
            if (OID == undefined || OID == "");
            OID = GetQueryString("WorkID");
            var FK_Node = GetQueryString("FK_Node");
            var FK_Flow = GetQueryString("FK_Flow");
            var webUser = new WebUser();
            var userNo = webUser.No;
            var SID = webUser.SID;
            if (SID == undefined)
                SID = "";
            if (url.indexOf("?") == -1)
                url = url + "?1=1";

            if (url.indexOf("SearchBS.htm") != -1)
                url = url + "&FK_Node=" + FK_Node + "&FK_Flow=" + FK_Flow + "&UserNo=" + userNo + "&SID=" + SID;
            else
                url = url + "&OID=" + OID + "&FK_Node=" + FK_Node + "&FK_Flow=" + FK_Flow + "&UserNo=" + userNo + "&SID=" + SID;

            eleHtml = '<span ><a href="' + url + '" target="_blank">' + mapAttr.Name + '</a></span>';
           
            return eleHtml;
        }
        if (mapAttr.UIHeight <= 40) //普通的文本框.
        {
            if (mapAttr.IsSigan == "1") {
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' type=hidden />";
                var val = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
                return "<img src='../DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\" style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
            }

            //alert(mapAttr.IsSigan);

            return "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "'  class='form-control' type='text' placeholder='" + (mapAttr.Tip || '') + "'/>";
        }

        if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {

            var eleHtml = "";
            //如果是富文本就使用百度 UEditor
            if (mapAttr.UIIsEnable == "0") {
                //只读状态直接 div 展示富文本内容                
                eleHtml += "<div class='richText' style='width:99%;margin-right:2px'>" + defValue + "</div>";

            } else {
                document.BindEditorMapAttr = mapAttr; //存到全局备用

                //设置编辑器的默认样式
                var styleText = "text-align:left;font-size:12px;";
                styleText += "width:100%;";
                styleText += "height:" + mapAttr.UIHeight + "px;";
                //注意这里 name 属性是可以用来绑定表单提交时的字段名字的 id 是特殊约定的.
                eleHtml += "<script id='editor'  name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";

                //eleHtml += "<script id='editor' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";

            }

            eleHtml = "<div style='white-space:normal;'>" + eleHtml + "</div>";
            return eleHtml
        }

        //普通的大块文本.
        return "<textarea maxlength=" + mapAttr.MaxLen + "  class='form-control' style='height:" + mapAttr.UIHeight + "px;width:100%;' id='TB_" + mapAttr.KeyOfEn + "' type='text'  " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + " />"
    }

    //日期类型.
    if (mapAttr.MyDataType == 6) {
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return " <input type='text' " + enableAttr + " value='" + defValue + "' style='width:120px;' class='form-control Wdate' id='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "' />";
    }

    //时期时间类型.
    if (mapAttr.MyDataType == 7) {

        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return " <input  type='text'  value='" + defValue + "' style='width:160px;' class='form-control Wdate' " + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
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

        checkedStr = ConvertDefVal(flowData, '', mapAttr.KeyOfEn);

        var tip= "";
        if (mapAttr.Tip != "" && mapAttr.Tip != null)
            tip = "<span style='color: #C0C0C0;'>(" + mapAttr.Tip + ")</span>";


        return "<label ><input " + enableAttr + " " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' id='CB_" + mapAttr.KeyOfEn + "'  name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + " /> &nbsp;" + mapAttr.Name +tip+"</label>";
    }

    //枚举类型.
    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";
        if (mapAttr.UIContralType == 1)
        //return "<select " + enableAttr + "  id='DDL_" + mapAttr.KeyOfEn + "' class='form-control' >" + InitDDLOperation(flowData, mapAttr, defValue) + "</select>";
            return "<select id='DDL_" + mapAttr.KeyOfEn + "' class='form-control'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")' value='" + defValue + "'>" + InitDDLOperation(flowData, mapAttr, defValue) + "</select>";
        if (mapAttr.UIContralType == 3) {
            //横向排列
            var RBShowModel = 3;
            if (mapAttr.AtPara.indexOf("@RBShowModel=3") == -1)
                RBShowModel = 0;
            return InitRBShowContent(flowData, mapAttr, defValue, RBShowModel, enableAttr);

        }
    }

    // AppDouble  AppFloat
    if (mapAttr.MyDataType == 5 || mapAttr.MyDataType == 3) {
        //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
        var attrdefVal = mapAttr.DefVal;
        var bit;
        if (attrdefVal != null && attrdefVal !== "" && attrdefVal.indexOf(".") >= 0)
            bit = attrdefVal.substring(attrdefVal.indexOf(".") + 1).length;

        return "<input  value='" + defValue + "' style='text-align:right;width:125px;'class='form-control'  onkeyup=" + '"' + "valitationAfter(this, 'float');if(isNaN(value)) execCommand('undo');limitLength(this," + bit + ");" + '"' + " onafterpaste=" + '"' + " valitationAfter(this, 'float');if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
    }

    if ((mapAttr.MyDataType == 2)) { //AppInt
        var enableAttr = '';
        if (mapAttr.UIIsEnable != 1) {
            enableAttr = "disabled='disabled'";
        }

        //alert(defValue);

        return "<input  value='" + defValue + "' style='text-align:right;width:125px;' class='form-control' onkeyup=" + '"' + "valitationAfter(this, 'int');if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'int');if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "'placeholder='" + (mapAttr.Tip || '') + "'/>";
    }

    //AppMoney  AppRate
    if (mapAttr.MyDataType == 8) {
        //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
        var attrdefVal = mapAttr.DefVal;
        var bit;
        if (attrdefVal != null && attrdefVal !== "" && attrdefVal.indexOf(".") >= 0)
            bit = attrdefVal.substring(attrdefVal.indexOf(".") + 1).length;
        else
            bit = 2;
        return "<input  value='" + defValue + "' style='text-align:right;width:125px;' class='form-control' onkeyup=" + '"' + "valitationAfter(this, 'money');if(isNaN(value))execCommand('undo');;limitLength(this," + bit + ");" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'money');if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' value='0.00' placeholder='" + (mapAttr.Tip || '') + "'/>";
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

    var Script = frmRB.Script;
    //解析执行js脚本
    if (Script != null && Script != "" && Script != undefined)
        DBAccess.RunDBSrc(Script, 2);
    //解决字段隐藏显示.
    var cfgs = frmRB.FieldsCfg;

    //解决为其他字段设置值.
    var setVal = frmRB.SetVal;
    if (setVal) {
        var strs = setVal.split('@');

        for (var i = 0; i < strs.length; i++) {
            var str = strs[i];
            if (str == "")
                continue;
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
    if (ctrl.length > 0)
        ctrl.parent('tr').show();

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

var flowData = {};

//初始化 IMAGE附件
function Ele_ImgAth(frmImageAth) {

    var isEdit = frmImageAth.IsEdit;
    var eleHtml = $("<div></div>");
    var img = $("<img/>");

    var imgSrc = "/WF/Data/Img/LogH.PNG";
    //获取数据
    if (flowData.Sys_FrmImgAthDB) {
        $.each(flowData.Sys_FrmImgAthDB, function (i, obj) {
            if (obj.FK_FrmImgAth == frmImageAth.MyPK) {
                imgSrc = obj.FileFullName;
            }
        });
    }
    //设计属性
    img.attr('id', 'Img' + frmImageAth.MyPK).attr('name', 'Img' + frmImageAth.MyPK);
    img.attr("src", imgSrc).attr('onerror', "this.src='/WF/Data/Img/LogH.PNG'");
    img.css('width', frmImageAth.W).css('height', frmImageAth.H).css('padding', "0px").css('margin', "0px").css('border-width', "0px");
    //不可编辑
    if (isEdit == "1") {
        var fieldSet = $("<fieldset></fieldset>");
        var length = $("<legend></legend>");
        var a = $("<a></a>");
        var url = "./CCForm/ImgAth.htm?W=" + frmImageAth.W + "&H=" + frmImageAth.H + "&FK_MapData=ND" + pageData.FK_Node + "&MyPK=" + pageData.WorkID + "&ImgAth=" + frmImageAth.MyPK;

        a.attr('href', "javascript:ImgAth('" + url + "','" + frmImageAth.MyPK + "');").html("编辑");
        length.css('font-style', 'inherit').css('font-weight', 'bold').css('font-size', '12px');

        fieldSet.append(length);
        length.append(a);
        fieldSet.append(img);
        eleHtml.append(fieldSet);
    } else {
        eleHtml.append(img);
    }
    eleHtml.css('position', 'absolute').css('top', frmImageAth.Y).css('left', frmImageAth.X);
    return eleHtml;
}


//审核组件
function Ele_FrmCheck(wf_node) {

    //审核组键FWCSta Sta,FWC_X X,FWC_Y Y,FWC_H H, FWC_W W from WF_Node
    var sta = wf_node.FWCSta;

    var h = wf_node.FWC_H + 1300;
    var src = "./WorkOpt/WorkCheck.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["WorkID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.WorkID;
    if (sta == 2)//只读
    {
        src += "&DoType=View";
    }
    src += "&r=q" + paras;

    if (h == 0)
        h = 400;

    var eleHtml = "<iframe width='100%' height='" + h + "' id='FWC' src='" + src + "'";
    eleHtml += " frameborder=0  leftMargin='0'  topMargin='0' scrolling=no ></iframe>";
    return eleHtml;
}

//子流程
function Ele_SubFlow(wf_node) {
    //SFSta Sta,SF_X X,SF_Y Y,SF_H H, SF_W W
    var sta = wf_node.SFSta;
    var h = wf_node.SF_H+1300;

    if (sta == 0)
        return $('');

    var src = "./WorkOpt/SubFlow.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["WorkID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.WorkID;
    if (sta == 2)//只读
    {
        src += "&DoType=View";
    }
    src += "&r=q" + paras;
    if (h == 0)
        h = 400;
    var eleHtml =  "<iframe id=FSF" + wf_node.NodeID + " style='width:100%;height:" + h + "px'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>";

    return eleHtml;
}


//初始化 框架
function Ele_Frame(flowData, gf) {
    var frame = new Entity("BP.Sys.MapFrame", gf.CtrlID);

    if (frame == null)
        return "没有找到框架的定义，请与管理员联系。";

    var eleHtml = '';

    var url = frame.URL;
    if (url.indexOf('?') == -1)
        url += "?1=2";

    if (url.indexOf("@basePath") == 0)
        url = url.replace("@basePath", basePath);

    //2.替换@参数
    var pageParams = getQueryString();
    $.each(pageParams, function (i, pageParam) {
        var pageParamArr = pageParam.split('=');
        url = url.replace("@" + pageParamArr[0], pageParamArr[1]);
    });

    var src = url.replace(new RegExp(/(：)/g), ':');
    if (src.indexOf("?") > 0) {
        var params = getQueryStringFromUrl(src);
        if (params != null && params.length > 0) {
            $.each(params, function (i, param) {
                if (param.indexOf('@') != -1) {//是需要替换的参数
                    paramArr = param.split('=');
                    if (paramArr.length == 2 && paramArr[1].indexOf('@') == 0) {
                        if (paramArr[1].indexOf('@WebUser.') == 0)
                            url = url.replace(paramArr[1], flowData.MainTable[0][paramArr[1].substr('@WebUser.'.length)]);
                        else
                            url = url.replace(paramArr[1], flowData.MainTable[0][paramArr[1].substr(1)]);
                    }
                }
            });
        }
    }


    //处理URL需要的参数
    //1.拼接参数
    var paras = this.pageData;
    var strs = "";
    for (var str in paras) {
        if (str == "EnsName" || str == "RefPKVal" || str == "IsReadonly")
            continue
        else
            strs += "&" + str + "=" + paras[str];
    }


    url = url + strs + "&IsReadonly=0";

    eleHtml += "<iframe style='width:100%;height:" + frame.H + "px;' ID='" + frame.MyPK + "'    src='" + url + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    return eleHtml;
}


//初始化 附件
function Ele_Attachment(flowData, gf, node) {

    var eleHtml = '';
    var nodeID = GetQueryString("FK_Node");
    var url = "";
    url += "&WorkID=" + GetQueryString("WorkID");
    url += "&FK_Node=" + nodeID;
    url += "&FK_Flow=" + node.FK_Flow;
    url += "&FormType=" + node.FormType; //表单类型，累加表单，傻瓜表单，自由表单.
    var no = node.NodeID.toString().substring(node.NodeID.toString().length - 2);
    var IsStartNode = 0;
    if(no=="01") 
     url += "&IsStartNode=" + 1; //是否是开始节点

    var isReadonly = false;
    if (gf.FrmID.indexOf(nodeID) == -1)
        isReadonly = true;

        //创建附件描述信息.
    var ath = new Entity("BP.Sys.FrmAttachment", gf.CtrlID);

    var athPK = gf.CtrlID;
    var noOfObj = athPK.replace(gf.FrmID + "_", "");

    var src = "";

    //这里的连接要取 FK_MapData的值.
    src = "./CCForm/Ath.htm?PKVal=" + pageData.WorkID + "&FID=" + pageData["FID"] + "&Ath=" + noOfObj + "&FK_MapData=ND" + node.NodeID + "&FromFrm="+ gf.FrmID + "&FK_FrmAttachment=" + athPK + url+"&M="+Math.random();

    //自定义表单模式.
    if (ath.AthRunModel == 2) {
        src = "../DataUser/OverrideFiles/Ath.htm?PKVal=" + pageData.WorkID + "&FID=" + pageData["FID"] + "&Ath=" + noOfObj + "&FK_MapData=" + gf.FrmID + "&FK_FrmAttachment=" + athPK + url + "&M=" + Math.random();
    }

    eleHtml += "<iframe style='width:100%;height:" + ath.H + "px;' id='Ath1' name='Ath1'  src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
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

    var ensName = frmDtl.No;
    if (ensName == undefined) {
        alert('系统错误,请找管理员联系');
        return;
    }

    if (frmDtl.ListShowModel == "0") {

        var dtlUrl = "Dtl2017";
        if (frmDtl.DtlVer == 1)
            dtlUrl = "Dtl2019";

        //表格模式
        if (pageData.IsReadonly) {
            src = "./CCForm/" + dtlUrl + ".htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=1&" + urlParam + "&Version=1";
        } else {
            src = "./CCForm/" + dtlUrl + ".htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=0&" + urlParam + "&Version=1";
        }
    }
    else if (frmDtl.ListShowModel == "1") {
        //卡片模式
        if (pageData.IsReadonly) {
            src = "./CCForm/DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=1&" + urlParam + "&Version=1";
        } else {
            src = "./CCForm/DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=0&" + urlParam + "&Version=1";
        }
    }
    return "<iframe style='width:100%;height:" + frmDtl.H + "px;' ID='F" + frmDtl.No + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
}

function InitRBShowContent(flowData, mapAttr, defValue, RBShowModel, enableAttr) {
    var rbHtml = "";
    var enums = flowData.Sys_Enum;
    enums = $.grep(enums, function (value) {
        return value.EnumKey == mapAttr.UIBindKey;
    });
    $.each(enums, function (i, obj) {
        var onclickEvent = "";
        if (mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
            onclickEvent = "onclick='clickEnable( this ,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'";
        }
        if (RBShowModel == 3)
        //<input  " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' id='CB_" + mapAttr.KeyOfEn + "'  name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + " /> &nbsp;" + mapAttr.Name + "</label</div>";
            rbHtml += "<label><input " + enableAttr + " " + (obj.IntKey == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "' "+onclickEvent+" />&nbsp;" + obj.Lab + "</label>";
        else
            rbHtml += "<label><input " + enableAttr + " " + (obj.IntKey == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "' " + onclickEvent + "/>&nbsp;" + obj.Lab + "</label><br/>";
    });
    return rbHtml;
}


   
 