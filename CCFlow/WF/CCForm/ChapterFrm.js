/**
 * 傻瓜表单的解析
 * @param {any} wn
 */
var frmData;
function GenerFoolFrm(wn, isComPare) {

    if (isComPare == null || isComPare == undefined || isComPare == "")
        isComPare = false;
    frmData = wn;
    var mapData = frmData.Sys_MapData[0];           //表单属性
    var tableCol = getTableCol(mapData.TableCol);   //表单列数
    var frmShowType = mapData.FrmShowType;          //表单展示方式 普通方式、页签方式
    var Sys_GroupFields = frmData.Sys_GroupField;   //分组信息
    var frmName = mapData.Name;                     //表单名称
    var node = frmData.WF_Node;                     //节点属性
    node = node != null && node != undefined ? node[0] : null;
    frmShowType = frmShowType == null || frmShowType == undefined || frmShowType == "" ? 0 : frmShowType;
    var _html = "";
    //表头
    _html += "<div class='layui-row FoolFrmTitle'>";
    _html += "<div class='layui-col-xs12'>";
    _html += "<div class='FoolFrmTitleLable' style='float:right;margin-top:8px' >" + frmName + "</div>";
    var imgsrc = "../DataUser/ICON/LogBiger.png";
    if (GetHrefUrl().indexOf("/CCBill") != -1 || GetHrefUrl().indexOf("CCForm") != -1)
        imgsrc = "../../DataUser/ICON/LogBiger.png";
    if (GetHrefUrl().indexOf("/AdminFrm.htm") != -1)
        imgsrc = "../../../DataUser/ICON/LogBiger.png";

    //_html += "<div class='FoolFrmTitleIcon' style='float:left;margin-top:1px'  > <img src='" + imgsrc + "' style='height:50px;' /></div >";
    _html += "</div>";
    _html += "</div>";
    //普通方式
    if (frmShowType == 0)
        _html += ShowFoolByTable(frmData, tableCol, Sys_GroupFields, node, isComPare);
    // 页签显示
    if (frmShowType == 1)
        _html += ShowFoolByTab(frmData, tableCol, Sys_GroupFields, node, isComPare);

    $("#CCForm").html(_html);


    //表格附件
    if (frmData.Sys_FrmAttachment && frmData.Sys_FrmAttachment.length != 0) {
        $.each(frmData.Sys_FrmAttachment, function (idex, ath) {
            if ($("#Div_" + ath.MyPK).length == 1)
                AthTable_Init(ath, "Div_" + ath.MyPK);
        });
    }


    //字段附件
    var aths = $(".athModel");
    $.each(aths, function (idx, ath) {
        //获取ID
        var name = $(ath).attr('id');
        var keyOfEn = name.replace("athModel_", "");
        $("#Lab_" + keyOfEn).html("<div style='text-align:left'>" + $("#Lab_" + keyOfEn).text() + "</div>");
    });


};


/**
 * 普通方式的解析
 */
function ShowFoolByTable(frmData, tableCol, Sys_GroupFields, node, isComPare) {
    var _html = "";
    var gfLabHtml = "";
    Sys_GroupFields.forEach(function (gf) {
        var ctrlType = gf.CtrlType;
        ctrlType = ctrlType == null ? "" : ctrlType;
        if (ctrlType == "FWC")
            gfLabHtml = "<div class='layui-row FoolFrmGroupBar' id='Group_FWC'>"
        else
            gfLabHtml = "<div class='layui-row FoolFrmGroupBar' id='Group_" + gf.CtrlID + "'>"
        gfLabHtml += "<div class='layui-col-xs12'>";
        gfLabHtml += gf.Lab;
        gfLabHtml += "</div>";
        gfLabHtml += "</div>";

        switch (ctrlType) {
            case "Ath": //附件
                if (gf.CtrlID == "")
                    break;
                if (frmData.Sys_FrmAttachment == undefined || frmData.Sys_FrmAttachment.length == 0)
                    break;
                //创建附件描述信息.
                var aths = $.grep(frmData.Sys_FrmAttachment, function (ath) { return ath.MyPK == gf.CtrlID });
                var ath = aths.length > 0 ? aths[0] : null;

                //附件分组不显示或者是审核组件中的附件
                if (ath != null && (ath.IsVisable == "0" || ath.NoOfObj == "FrmWorkCheck"))
                    break;
                //增加附件分组
                _html += gfLabHtml;
                _html += "<div class='layui-row'>"
                _html += "<div class='layui-col-xs12'>";
                if (ath == null)
                    _html += "附件" + gf.CtrlID + "信息丢失";
                else
                    _html += "<div id='Div_" + ath.MyPK + "' name='Ath'></div>";
                _html += "</div>";
                _html += "</div>";
                break;
            case "Dtl"://从表
                if (frmData.Sys_MapDtl == undefined || frmData.Sys_MapDtl.length == 0)
                    break;
                var dtls = $.grep(frmData.Sys_MapDtl, function (dtl) {
                    return dtl.No == gf.CtrlID && dtl.IsView != 0;
                });
                var dtl = dtls.length > 0 ? dtls[0] : null;
                if (dtl == null)
                    break;
                _html += gfLabHtml;
                _html += "<div class='layui-row'>"
                _html += "<div class='layui-col-xs12'>";
                if (dtl == null)
                    _html += "从表" + gf.CtrlID + "信息丢失";
                else
                    _html += "<div id='Dtl_" + dtl.No + "' name='dtl'>" + Ele_Dtl(dtl, isComPare) + "</div>";
                _html += "</div>";
                _html += "</div>";
                break;
            case "FWC"://审核组件
                if (node == null || node.FWCSta == 0)
                    break;
                if (GetHrefUrl().indexOf("AdminFrm.htm") != -1)
                    break;
                //如何有签批字段就不解析

                _html += gfLabHtml;
                _html += "<div class='layui-row'>"
                _html += "<div class='layui-col-xs12'>";
                _html += "<div id='WorkCheck'></div>";
                _html += "</div>";
                _html += "</div>";
                break;
            case "Frame"://框架
                _html += gfLabHtml;
                _html += "<div class='layui-row'>"
                _html += "<div class='layui-col-xs12'>";
                _html += Ele_Frame(frmData, gf);
                _html += "</div>";
                _html += "</div>";
                break;
            case "SubFlow"://父子流程
                Skip.addJs("./WorkOpt/SubFlow.js");
                _html += gfLabHtml;
                _html += "<div class='layui-row'>"
                _html += "<div class='layui-col-xs12'>";
                //说明是累加表单.
                if (gf.FrmID.indexOf(node.NodeID) == -1) {

                    var myNodeID = gf.FrmID.substring(2);
                    var myNode = new Entity("BP.WF.Node", myNodeID);
                    _html += "<div id='SubFlow'>" + SubFlow_Init(myNode) + "</div>";
                }
                else {
                    _html += "<div id='SubFlow'>" + SubFlow_Init(node) + "</div>";
                }
                _html += "</div>";
                _html += "</div>";
                break;
            default://普通字段
                if (gf.ShowType == 2) //0显示 1 PC折叠 2 隐藏
                    break;
                _html += gfLabHtml;
                _html += InitMapAttr(frmData, tableCol, gf.OID);
                break;
        }
    });
    return _html;
}
/**
    * Tab页签的方式显示
    */
function ShowFoolByTab(frmData, tableCol, Sys_GroupFields, node, isComPare) {
    var _html = "";
    _html += '<div class="layui-tab layui-tab-brief" lay-filter="Fool">';
    _html += '<ul class="layui-tab-title">';
    var i = 0;
    $.each(Sys_GroupFields, function (i, gf) {
        if (i == 0)
            _html += "<li class='layui-this'>" + gf.Lab + "</li>";
        else
            _html += "<li>" + gf.Lab + "</li>";
    });

    _html += '</ul>';
    _html += '<div class="layui-tab-content">';

    $.each(Sys_GroupFields, function (i, gf) {
        var contHtml = "";
        if (i == 0)
            contHtml += "<div class='layui-tab-item layui-show'>";
        else
            contHtml += "<div class='layui-tab-item'>";
        var ctrlType = gf.CtrlType;
        ctrlType = ctrlType == null ? "" : ctrlType;
        switch (ctrlType) {
            case "Ath": //附件
                if (gf.CtrlID == "")
                    break;
                //创建附件描述信息.
                var aths = $.grep(frmData.Sys_FrmAttachment, function (ath) { return ath.MyPK == gf.CtrlID });
                var ath = aths.length > 0 ? aths[0] : null;

                //附件分组不显示或者是审核组件中的附件
                if (ath != null && (ath.IsVisable == "0" || ath.NoOfObj == "FrmWorkCheck"))
                    break;
                //增加附件分组
                contHtml += "<div class='layui-row'>"
                contHtml += "<div class='layui-col-xs12'>";
                if (ath == null)
                    contHtml += "附件" + gf.CtrlID + "信息丢失";
                else
                    contHtml += "<div id='Div_" + ath.MyPK + "' name='Ath'></div>";
                contHtml += "</div>";
                contHtml += "</div>";
                break;
            case "Dtl"://从表
                var dtls = $.grep(frmData.Sys_MapDtl, function (dtl) {
                    return dtl.No == gf.CtrlID && dtl.IsView != 0;
                });
                var dtl = dtls.length > 0 ? dtls[0] : null;
                if (dtl == null)
                    break;
                contHtml += "<div class='layui-row'>"
                contHtml += "<div class='layui-col-xs12'>";
                if (dtl == null)
                    contHtml += "从表" + gf.CtrlID + "信息丢失";
                else
                    contHtml += "<div id='Dtl_" + dtl.No + "' name='dtl'>" + Ele_Dtl(dtl, isComPare) + "</div>";
                contHtml += "</div>";
                contHtml += "</div>";
                break;
            case "FWC"://审核组件
                if (node == null || node.FWCSta == 0)
                    break;
                if (GetHrefUrl().indexOf("AdminFrm.htm") != -1)
                    break;
                //如何有签批字段就不解析

                contHtml += "<div class='layui-row'>"
                contHtml += "<div class='layui-col-xs12'>";
                contHtml += "<div id='WorkCheck'></div>";
                contHtml += "</div>";
                contHtml += "</div>";
                break;
            case "Frame"://框架
                contHtml += "<div class='layui-row'>"
                contHtml += "<div class='layui-col-xs12'>";
                contHtml += Ele_Frame(frmData, gf);
                contHtml += "</div>";
                contHtml += "</div>";
                break;
            case "SubFlow"://父子流程
                Skip.addJs("./WorkOpt/SubFlow.js");
                contHtml += gfLabHtml;
                contHtml += "<div class='layui-row'>"
                contHtml += "<div class='layui-col-xs12'>";
                //说明是累加表单.
                if (gf.FrmID.indexOf(node.NodeID) == -1) {

                    var myNodeID = gf.FrmID.substring(2);
                    var myNode = new Entity("BP.WF.Node", myNodeID);
                    contHtml += "<div id='SubFlow'>" + SubFlow_Init(myNode) + "</div>";
                }
                else {
                    contHtml += "<div id='SubFlow'>" + SubFlow_Init(node) + "</div>";
                }
                contHtml += "</div>";
                contHtml += "</div>";
                break;
            default://普通字段
                if (gf.ShowType == 2) //0显示 1 PC折叠 2 隐藏
                    break;
                contHtml += InitMapAttr(frmData, tableCol, gf.OID);
                break;
        }
        contHtml += "</div>";
        _html += contHtml;
    });
    _html += '</div > ';
    _html += '</div>';
    return _html;
}
/**
    * 解析基本字段
    * @param {any} groupID
    */
function InitMapAttr(frmData, tableCol, groupID) {
    var Sys_MapAttr = $.grep(frmData.Sys_MapAttr, function (mapAttr) { return mapAttr.GroupID == groupID; });
    var html = "";
    var isDropTR = true;
    var colSpan = 1;
    var LabelColSpan = 1;
    var textWidth = "";
    var colWidth = "";
    var useColSpan = 0;

    for (var i = 0; i < Sys_MapAttr.length; i++) {
        var attr = Sys_MapAttr[i];
        if (attr.UIVisible == 0)
            continue;
        //单元格和标签占的列数
        colSpan = attr.ColSpan;
        LabelColSpan = attr.LabelColSpan;
        //单元格和标签占的列数对应的class
        colWidth = getColSpanClass(colSpan, tableCol);
        textWidth = getLabelColSpanClass(LabelColSpan, tableCol);
        //大文本备注信息 独占一行
        if (attr.UIContralType == 60) {
            if (isDropTR == false) {
                html += "</div>";
                isDropTR = true;
            }
            textWidth = getLabelColSpanClass(tableCol, tableCol);
            //获取文本信息
            var filename = basePath + "/DataUser/CCForm/BigNoteHtmlText/" + attr.FK_MapData + ".htm?r=" + Math.random();
            var htmlobj = $.ajax({ url: filename, async: false });
            var str = htmlobj.responseText;
            if (htmlobj.status == 404)
                str = filename + "这个文件不存在，请联系管理员";
            html += "<div class='layui-row FoolFrmFieldRow'>";
            html += "<div  class='" + textWidth + " FoolFrmFieldLabel'>" + str + "</div>";
            html += "</div>";
            isDropTR = true;
            continue;
        }
        //跨列设置(显示的是文本)
        if (colSpan == 0) {

            if (LabelColSpan >= tableCol) {
                if (isDropTR == false) {
                    html += "</div>";
                    isDropTR = true;
                }
                textWidth = getLabelColSpanClass(tableCol, tableCol);
                html += "<div class='layui-row FoolFrmFieldRow'>";
                html += "<div  class='" + textWidth + " FoolFrmFieldLabel'>" + GetLab(attr, frmData) + "</div>";
                html += "</div>";
                isDropTR = true;
                continue;
            }
            //线性展示都跨一个单元格
            //换行的情况
            if (isDropTR == true) {
                useColSpan = LabelColSpan;
                html += "<div class='layui-row FoolFrmFieldRow'>";
                html += "<div  class='" + textWidth + " FoolFrmFieldLabel'>" + GetLab(attr, frmData) + "</div>";
                if (useColSpan == tableCol) {
                    isDropTR = true;
                    html += "</div>";
                }
                else
                    isDropTR = false;
                continue;
            }

            if (isDropTR == false) {
                useColSpan += LabelColSpan;
                if (useColSpan > tableCol) {
                    useColSpan = LabelColSpan;
                    //自动换行
                    html += "</div>";
                    html += "<div class='layui-row FoolFrmFieldRow'>";

                }
                if (attr.UIContralType == 18)
                    html += "<div  class='" + textWidth + " '>" + GetLab(attr, frmData) + "</div>";
                else
                    html += "<div  class='" + textWidth + " FoolFrmFieldLabel'>" + GetLab(attr, frmData) + "</div>";
                if (useColSpan == tableCol) {
                    isDropTR = true;
                    html += "</div>";
                }
                else
                    isDropTR = false;
                continue;
            }
        }
        //解析占一行的情况
        if (colSpan == tableCol) {
            if (isDropTR == false) {
                html += "</div>";
                isDropTR = true;
            }
            useColSpan = 0;
            //自动换行
            html += "<div class='layui-row FoolFrmFieldRow'>";
            html += "<div class='" + colWidth + " FoolFrmFieldLabel'>" + GetLab(attr, frmData) + "</div>";
            if (attr.UIContralType != 12)
                html += "<div class='" + colWidth + " FoolFrmFieldInput' id='TD_" + attr.KeyOfEn + "' >" + InitMapAttrOfCtrlFool(frmData, attr) + "</div>";
            html += "</div>"
            continue;
        }
        var sumColSpan = colSpan + LabelColSpan;
        if (sumColSpan >= tableCol) {
            if (isDropTR == false) {
                html += "</div>";
                isDropTR = true;
            }
            useColSpan = 0;
            if (sumColSpan > tableCol)
                colWidth = getColSpanClass(tableCol - LabelColSpan, tableCol)
            //colWidth = textWidth.replace("layui-col-md", "").replace(" layui-col-xs4");
            //colWidth = "layui-col-md" + (12 - parseInt(colWidth)) +" layui-col-xs8" ;
            html += "<div class='layui-row FoolFrmFieldRow'>";
            if (attr.MyDataType == 1 && attr.LGType == 0 && (attr.IsSupperText == 1 || attr.UIHeight > 40))
                html += "<div  class='" + textWidth + " FoolFrmFieldLabel' style='height:" + attr.UIHeight + "px'>" + GetLab(attr, frmData) + "</div>";
            else
                html += "<div  class='" + textWidth + " FoolFrmFieldLabel'>" + GetLab(attr, frmData) + "</div>";
            if (attr.UIContralType != 12)
                html += "<div  class='" + colWidth + " FoolFrmFieldInput' id='TD_" + attr.KeyOfEn + "'>" + InitMapAttrOfCtrlFool(frmData, attr) + "</div>";
            html += "</div>";
            continue;
        }

        //换行的情况
        if (isDropTR == true) {
            useColSpan = LabelColSpan + colSpan;
            html += "<div class='layui-row FoolFrmFieldRow'>";
            html += "<div  class='" + textWidth + " FoolFrmFieldLabel'>" + GetLab(attr, frmData) + "</div>";
            if (attr.UIContralType != 12)
                html += "<div  class='" + colWidth + " FoolFrmFieldInput' id='TD_" + attr.KeyOfEn + "'>" + InitMapAttrOfCtrlFool(frmData, attr) + "</div>";
            if (useColSpan >= tableCol) {
                isDropTR = true;
                html += "</div>";
            }
            else
                isDropTR = false;
            continue;

        }
        if (isDropTR == false) {
            useColSpan += LabelColSpan + colSpan;
            if (useColSpan > tableCol) {
                useColSpan = LabelColSpan + colSpan;
                //自动换行
                html += "</div>";
                html += "<div class='layui-row FoolFrmFieldRow'>";
                html += "<div  class='" + textWidth + " FoolFrmFieldLabel'>" + GetLab(attr, frmData) + "</div>";
                if (attr.UIContralType != 12)
                    html += "<div  class='" + colWidth + " FoolFrmFieldInput' id='TD_" + attr.KeyOfEn + "'>" + InitMapAttrOfCtrlFool(frmData, attr) + "</div>";
            } else {
                html += "<div  class='" + textWidth + " FoolFrmFieldLabel'>" + GetLab(attr, frmData) + "</div>";
                if (attr.UIContralType != 12)
                    html += "<div  class='" + colWidth + " FoolFrmFieldInput' id='TD_" + attr.KeyOfEn + "'>" + InitMapAttrOfCtrlFool(frmData, attr) + "</div>";
            }
            if (useColSpan == tableCol) {
                isDropTR = true;
                html += "</div>";
            }
            else
                isDropTR = false;
            continue;
        }
    }
    if (isDropTR == false)
        html += "</div>";
    return html;
}
/**
    * 获取单元格显示的内容
    * @param {any} mapAttr
    */
function InitMapAttrOfCtrlFool(frmData, mapAttr) {
    var ccsCtrl = mapAttr.CSSCtrl;
    ccsCtrl = ccsCtrl == null || ccsCtrl == undefined || ccsCtrl == "0" ? "" : ccsCtrl;

    var eleHtml = "";

    //下拉框 外键和外部数据源
    if ((mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1)
        || (mapAttr.LGType == "2" && mapAttr.MyDataType == "1")) {
        var css = "";
        if (mapAttr.LGType == "0")
            css = "class='ddl-ext'";
        return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'><select id = 'DDL_" + mapAttr.KeyOfEn + "' name = 'DDL_" + mapAttr.KeyOfEn + "' " + css + " lay-filter='" + mapAttr.KeyOfEn + "'  > " + InitDDLOperation(frmData, mapAttr, "") + "</select></div>";

    }
    //枚举 单选枚举和下拉框枚举
    if (mapAttr.LGType == 1) {
        var ses = frmData[mapAttr.KeyOfEn];
        if (ses == undefined)
            ses = frmData[mapAttr.UIBindKey];
        if (ses == undefined) {
            //枚举类型的.
            if (mapAttr.LGType == 1) {
                ses = frmData.Sys_Enum;
                ses = $.grep(ses, function (value) {
                    return value.EnumKey == mapAttr.UIBindKey;
                });
            }

        }
        if (mapAttr.UIContralType == 1) {//下拉框显示
            var operations = "";
            $.each(ses, function (i, obj) {
                operations += "<option  value='" + obj.IntKey + "'>" + obj.Lab + "</option>";
            });
            return "<div id='DIV_" + mapAttr.KeyOfEn + "'><select " + ccsCtrl + " name='DDL_" + mapAttr.KeyOfEn + "'  id='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + "  lay-filter='" + mapAttr.KeyOfEn + "' >" + operations + "</select></div>";
        }
        if (mapAttr.UIContralType == 2) {//复选框
            var rbHtmls = "";
            //显示方式,默认为 0=横向展示 3=横向.. 
            var RBShowModel = 0;
            if (mapAttr.AtPara.indexOf('@RBShowModel=3') >= 0)
                RBShowModel = 3;
            for (var i = 0; i < ses.length; i++) {
                var se = ses[i];
                var br = "";
                if (RBShowModel == 0)
                    br = "<br>";
                var checked = "";
                //if (se.IntKey == mapAttr.DefVal)
                //    checked = " checked=true ";
                rbHtmls += "<input " + ccsCtrl + " type=checkbox name='CB_" + mapAttr.KeyOfEn + "' id='CB_" + mapAttr.KeyOfEn + "_" + se.IntKey + "' value='" + se.IntKey + "' " + checked + " lay-filter='" + mapAttr.KeyOfEn + "'  class='mcheckbox'  value='" + se.IntKey + "' title='" + se.Lab + "'/>";
            }
            return "<div id='DIV_" + mapAttr.KeyOfEn + "'>" + rbHtmls + "</div>";
        }

        if (mapAttr.UIContralType == 3) {//单选按钮显示
            var rbHtmls = "";
            //显示方式,默认为 0=横向展示 3=横向.. 
            var RBShowModel = 0;
            if (mapAttr.AtPara.indexOf('@RBShowModel=3') >= 0)
                RBShowModel = 3;
            for (var i = 0; i < ses.length; i++) {
                var se = ses[i];
                var br = "";
                if (RBShowModel == 0)
                    br = "<br>";
                var checked = "";
                if (se.IntKey == mapAttr.DefVal)
                    checked = " checked=true ";
                rbHtmls += "<input " + ccsCtrl + " type=radio name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + se.IntKey + "' value='" + se.IntKey + "' " + checked + " lay-filter='" + mapAttr.KeyOfEn + "'   title='" + se.Lab + "'/>" + br;
            }
            return "<div id='DIV_" + mapAttr.KeyOfEn + "'>" + rbHtmls + "</div>";
        }
    }

    //普通字段
    if (mapAttr.LGType == 0) {
        switch (parseInt(mapAttr.MyDataType)) {
            case 1://普通文本
                //获取到当前字段值
                var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
                switch (parseInt(mapAttr.UIContralType)) {
                    case 4: //地图
                        //如果是地图，并且可以编辑
                        var eleHtml = "<div style='text-align:left;padding-left:0px' id='athModel_" + mapAttr.KeyOfEn + "' data-type='1'>";
                        if (mapAttr.UIIsEnable == 1 && isReadonly == false) {
                            eleHtml += "<button type='button' class='layui-btn layui-btn-sm layui-btn-primary' style='height:38px' name='select' onclick='figure_Template_Map(\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.UIIsEnable + "\")'>选择</button>";
                            eleHtml += "<input type = text class='layui-input' style='width:75%;display:inline' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' />";
                        } else {
                            eleHtml += "<button type='button' name='select'class='layui-btn layui-btn-sm layui-btn-primary' style='height:38px' onclick='figure_Template_Map(\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.UIIsEnable + "\")'>选择</button>";
                            eleHtml += "<input type = text  class='layui-input' style='width:75%;display:inline' readonly='readonly' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' />";
                        }
                        eleHtml += "</div>";
                        return eleHtml;
                    case 6://字段附件
                        return getFieldAth(mapAttr,frmData.Sys_FrmAttachment);

                    case 8://写字板
                        var imgPath = "../";
                        if (currentURL.indexOf("CCBill") != -1 || currentURL.indexOf("CCForm") != -1)
                            imgPath = "../../";
                        if (currentURL.indexOf("AdminFrm.htm") != -1)
                            imgPath = "../../../";
                        var imgSrc = imgPath + "DataUser/Siganture/UnName.jpg";
                        //如果是图片签名，并且可以编辑
                        var ondblclick = ""
                        if (mapAttr.UIIsEnable == 1) {
                            ondblclick = " ondblclick='figure_Template_HandWrite(\"" + mapAttr.KeyOfEn + "\",\"" + val + "\")'";
                        }
                        val = imgPath + val.substring(val.indexOf("DataUser"));
                        var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' type=hidden />";
                        eleHtml += "<img src='" + val + "' " + ondblclick + " onerror=\"this.src='" + imgSrc + "'\"  style='border:0px;height:" + mapAttr.UIHeight + "px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                        return eleHtml;

                    case 9://超链接
                        return "<a  class='" + ccsCtrl + "' id='Link_" + mapAttr.KeyOfEn + "' href='" + mapAttr.Tag2 + "' target='" + mapAttr.Tag1 + "' name='Link_" + mapAttr.KeyOfEn + "' >" + mapAttr.Name + "</a>";
                    case 13://身份证
                        if (mapAttr.KeyOfEn == "IDCardAddress") {
                            eleHtml = "<div style='text-align:left;padding-left:0px'  data-type='1'>";
                            eleHtml += "<input type=text class='" + ccsCtrl + " layui-input' style='width:75% !important;display:inline;' class='form-control' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'/>";
                            eleHtml += "<label class='image-local' style='margin-left:5px'><input type='file' accept='image/png,image/bmp,image/jpg,image/jpeg' style='width:25% !important;display:none' onchange='GetIDCardInfo(event)'/>上传身份证</label>";
                            eleHtml += "</div>";
                            return eleHtml;
                        }
                        return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'> <input class='" + ccsCtrl + " layui-input'  maxlength=" + mapAttr.MaxLen + "  value='" + mapAttr.DefVal + "' name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "'placeholder='" + (mapAttr.Tip || '') + "' type='text' " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + " /></div>";
                        break;
                    case 16://系统定位
                        eleHtml = "<div style='text-align:left;padding-left:0px' >";
                        eleHtml += "<input type='button' class='" + ccsCtrl + "' name='select' value='系统定位' />";
                        eleHtml += "</div>";
                        return eleHtml;

                    //return "<input type='button' class='" + ccsCtrl + "'  id='Btn_" + mapAttr.KeyOfEn + "' name='Btn_" + mapAttr.KeyOfEn + "' value='" + mapAttr.Name + "' onclick=''/>";
                    case 50://工作进度
                        return " <div id ='JobSchedule' class='DashbCon'></div>";
                    case 101://评分标准
                        val = val == null || val == undefined || val == "" ? 0 : val;

                        //如果编辑
                        var eleHtml = "<div class='score-star' style='text-align:left;padding-left:3px;height:30px;margin-top:10px' data-type='1' id='SC_" + mapAttr.KeyOfEn + "' >";
                        if (mapAttr.UIIsEnable == 1)
                            eleHtml += "<span class='score-simplestar' id='Star_" + mapAttr.KeyOfEn + "'>";
                        else
                            eleHtml += "<span class='score-simplestar'>";
                        var num = mapAttr.Tag2;
                        var baseUrl = "./";
                        if (currentURL.indexOf("CCForm") != -1 || currentURL.indexOf("CCBill") != -1)
                            baseUrl = "../";
                        if (currentURL.indexOf("AdminFrm.htm") != -1)
                            baseUrl = "../../";
                        for (var i = 0; i < val; i++) {

                            eleHtml += "<img src='" + baseUrl + "Style/Img/star_2.png' />";
                        }
                        for (var j = 0; j < num - val; j++) {

                            eleHtml += "<img src='" + baseUrl + "Style/Img/star_1.png' />";
                        }
                        eleHtml += "&nbsp;&nbsp;<span class='score-tips' id='SP_" + mapAttr.KeyOfEn + "' style='vertical-align: middle;color:#ff6600;font: 12px/1.5 tahoma,arial,\"Hiragino Sans GB\",宋体,sans-serif;'><strong>" + val + "  分</strong></span>";
                        eleHtml += "<input id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' type=hidden /></span>";
                        eleHtml += "</div>";
                        return eleHtml;
                    case 110: //公文组件
                        eleHtml = "<div style='text-align:left;padding-left:0px' >";
                        eleHtml += "<input type='text'  class='" + ccsCtrl + "' id='TB_" + mapAttr.KeyOfEn + "'name='TB_" + mapAttr.KeyOfEn + "' style='display:none'/>";
                        eleHtml += "<input type='button'  class='" + ccsCtrl + "' id='" + mapAttr.FK_MapData + "_" + mapAttr.KeyOfEn + "' name='AthSingle' value='" + mapAttr.Name + "' />";
                        eleHtml += "</div>";
                        return eleHtml;
                    default:
                        //判断是不是富文本编辑器
                        if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {
                            if (mapAttr.UIIsEnable == "0" || isReadonly == true) {
                                //使用div展示
                                var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
                                defValue = defValue.replace(/white-space: nowrap;/g, "");
                                return "<div style='margin:9px 0px 9px 15px'>" + defValue + "</div>";
                            }
                            if (richTextType == "tinymce")
                                return "<textarea maxlength=" + mapAttr.MaxLen + "  style='height:" + mapAttr.UIHeight + "px;width:100%;' id='TB_" + mapAttr.KeyOfEn + "'  class='rich'/>";
                            var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
                            //设置一个默认高度
                            if (mapAttr.UIHeight < 180) {
                                mapAttr.UIHeight = 180;
                            }
                            //设置编辑器的默认样式
                            var styleText = "text-align:left;font-size:12px;";
                            styleText += "width:100%;";
                            var height = parseInt(mapAttr.UIHeight) - 54;

                            styleText += "height:" + height + "px;";
                            //注意这里 name 属性是可以用来绑定表单提交时的字段名字的 id 是特殊约定的.
                            return "<script class='EditorClass' id='editor_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";
                        }
                        //判断是不是大块文本
                        if (mapAttr.IsSupperText == 1 || mapAttr.UIHeight > 40) {
                            return "<textarea class='layui-textarea'  id='TB_" + mapAttr.KeyOfEn + "' type='text'  " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + "></textarea>"
                        }
                        var baseUrl = "../";
                        if (currentURL.indexOf("AdminFrm.htm") != -1)
                            baseUrl = "../../../";
                        if (currentURL.indexOf("CCBill") != -1 || currentURL.indexOf("CCForm") != -1)
                            baseUrl = "../../";

                        if (mapAttr.IsSigan == "1" && mapAttr.UIIsEnable == 1 && isReadonly != 0) {
                            var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'  value='" + defValue + "' type=hidden />";
                            //是否签过
                            var sealData = new Entities("BP.Tools.WFSealDatas");
                            sealData.Retrieve("OID", pageData.OID, "FK_Node", GetQueryString("FK_Node"), "SealData", GetQueryString("UserNo"));

                            if (sealData.length > 0) {
                                eleHtml += "<img src='" + baseUrl + "DataUser/Siganture/" + defValue + ".jpg' alt='" + defValue + "'  style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                                isSigantureChecked = true;
                            }
                            else {
                                eleHtml += "<img src='" + baseUrl + "DataUser/Siganture/siganture.jpg'   ondblclick='figure_Template_Siganture(\"" + mapAttr.KeyOfEn + "\",\"" + defValue + "\")' style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                            }
                            return eleHtml;
                        }
                        //如果不可编辑，并且是图片名称
                        if (mapAttr.IsSigan == "1") {
                            var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
                            var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'  value='" + val + "' type=hidden />";
                            eleHtml += "<img src='" + baseUrl + "DataUser/Siganture/" + val + ".jpg' alt='" + val + "' style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                            return eleHtml;
                        }

                        return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'> <input class='" + ccsCtrl + " layui-input'  maxlength=" + mapAttr.MaxLen + "  value='" + mapAttr.DefVal + "' name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "'placeholder='" + (mapAttr.Tip || '') + "' type='text' " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + " /></div>";
                }
                break;
            case 2://整数
                return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'><input class='" + ccsCtrl + " layui-input'  value='0' style='text-align:right;'  onkeyup=" + '"' + "valitationAfter(this, 'int');if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'int');if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "'placeholder='" + (mapAttr.Tip || '') + "'/></div>";
            case 4:  //复选框
                if (mapAttr.UIIsEnable == 0) {
                    enableAttr = "disabled='disabled'";
                } else {
                    enableAttr = "";
                }
                return "<div class='checkbox' id='DIV_" + mapAttr.KeyOfEn + "'><label for='CB_" + mapAttr.KeyOfEn + "' ></label><input type='checkbox' name='CB_" + mapAttr.KeyOfEn + "' id='CB_" + mapAttr.KeyOfEn + "' lay-skin='switch' lay-text='是|否' " + (mapAttr.DefVal == 1 ? "checked = 'checked'" : "") + enableAttr + " lay-filter='" + mapAttr.KeyOfEn + "' value='1'></div>";
            case 3://浮点
            case 5://双精度
                var attrdefVal = mapAttr.DefVal;
                var bit;
                if (attrdefVal != null && attrdefVal !== "" && attrdefVal.indexOf(".") >= 0)
                    bit = attrdefVal.substring(attrdefVal.indexOf(".") + 1).length;
                else
                    bit = 2;
                return "<input class='" + ccsCtrl + " layui-input'  value='0.00'  onkeyup=" + '"' + "valitationAfter(this, 'float');if(isNaN(value)) execCommand('undo');limitLength(this," + bit + ");" + '"' + " onafterpaste=" + '"' + " valitationAfter(this, 'float');if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
            case 6://日期类型
            case 7://时间类型
                //生成中间的部分.
                var enableAttr = '';
                var frmDate = mapAttr.IsSupperText; //获取日期格式
                var dateFmt = '';
                var dateType = "";
                if (frmDate == 0) {
                    dateFmt = "yyyy-MM-dd";
                    dateType = "date"
                } else if (frmDate == 1) {
                    dateFmt = "yyyy-MM-dd HH:mm";
                    dateType = "datetime";
                } else if (frmDate == 2) {
                    dateFmt = "yyyy-MM-dd HH:mm:ss";
                    dateType = "datetime";
                } else if (frmDate == 3) {
                    dateFmt = "yyyy-MM";
                    dateType = "month";
                } else if (frmDate == 4) {
                    dateFmt = "HH:mm";
                    dateType = "time";
                } else if (frmDate == 5) {
                    dateFmt = "HH:mm:ss";
                    dateType = "time";
                } else if (frmDate == 6) {
                    dateFmt = "MM-dd";
                    dateType = "date";
                }
                else if (frmDate == 7) {
                    dateFmt = "yyyy";
                    dateType = "year";
                }
                if (mapAttr.UIIsEnable == 0)
                    enableAttr = "disabled='disabled' ";



                return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'><i class='input-icon layui-icon layui-icon-date'></i> <input class='" + ccsCtrl + " ccdate layui-input'  data-info='" + dateFmt + "' data-type='" + dateType + "' maxlength=" + mapAttr.MaxLen + " value='" + mapAttr.DefVal + "'  type='text' " + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "'/></div>";
            case 8://金额
                //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数

                var attrdefVal = mapAttr.DefVal;
                var bit;
                if (attrdefVal != null && attrdefVal !== "" && attrdefVal.indexOf(".") >= 0)
                    bit = attrdefVal.substring(attrdefVal.indexOf(".") + 1).length;
                else
                    bit = 2;
                return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'><input value='0.00' class='" + ccsCtrl + " layui-input' style='text-align:right;' onkeyup=" + '"' + "valitationAfter(this, 'money');limitLength(this," + bit + "); FormatMoney(this, " + bit + ", ',')" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'money');if(isNaN(value))execCommand('undo');" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' value='0.00' placeholder='" + (mapAttr.Tip || '') + "'/></div>";
            default: break;

        }
    }

}
/**
* 获取字段标签
* @param {any} attr
* @param {any} frmData
*/
function GetLab(attr, frmData) {
    var lab = "";
    var forID = "TB_" + attr.KeyOfEn;
    var contralType = attr.UIContralType;

    var ccsLab = attr.CSSLabel;
    ccsLab = ccsLab == null || ccsLab == undefined || ccsLab == "0" ? "" : ccsLab;
    switch (parseInt(contralType)) {
        case 0://文本
        case 1://下拉框
        case 2://复选框
        case 3://单选按钮
        case 4://地图
        case 6://字段附件
        case 8://手写签字版
        case 13://身份证号
        case 50: //流程进度图
        case 101://评分
            if (contralType == 1)//外键下拉框
                forID = "DDL_" + attr.KeyOfEn;
            if (contralType == 3)//枚举
                forID = "RB_" + attr.KeyOfEn;
            if (contralType == 2)//枚举复选框
                forID = "CB_" + attr.KeyOfEn;
            ccsLab += " layui-form-label"
            if (attr.UIIsInput == 1 && attr.UIIsEnable == 1) {
                lab = " <span style='color:red' class='mustInput' data-keyofen='" + attr.KeyOfEn + "' >*</span>";
                ccsLab += " mustInput ";
            }

            lab += "<label class='" + ccsLab + "' id='Lab_" + attr.KeyOfEn + "' for='" + forID + "' >" + attr.Name + "</label>";
            return lab;

        case 9://超链接
            var url = attr.Tag2;
            //替换URL中的参数
            var pageParams = getQueryString();
            $.each(pageParams, function (i, pageParam) {
                var pageParamArr = pageParam.split('=');
                url = url.replace("@" + pageParamArr[0], pageParamArr[1]);
            });

            //替换表单中的参数
            $.each(frmData.Sys_MapAttr, function (i, obj) {
                if (url != null && url.indexOf('@' + obj.KeyOfEn) > 0) {
                    url = url.replace('@' + obj.KeyOfEn, frmData.MainTable[0][obj.KeyOfEn]);
                }
            });
            url = url.indexOf("?") == -1 ? url + "?1=1" : url;

            if (url.indexOf("Search.htm") != -1)
                url = url + "&FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&UserNo=" + webUser.No + "&Token=" + webUser.Token;
            else
                url = url + "&UserNo=" + webUser.No + "&Token=" + webUser.Token;

            if (url.indexOf('OID=') == -1)
                url += "&OID=" + frmData.MainTable[0].OID;
            if (url.indexOf('WorkID=') == -1)
                url += "&WorkID=" + frmData.MainTable[0].OID;

            if (url.indexOf('FrmID=') == -1)
                url += "&FrmID=" + attr.FK_MapData;

            return '<span ><a href="' + url + '" target="_blank">' + attr.Name + '</a></span>';
        case 11://图片（只显示）
            //获取图片控件的信息
            var frmImg = new Entity("BP.Sys.FrmUI.ExtImg");
            frmImg.SetPKVal(attr.MyPK);
            var count = frmImg.RetrieveFromDBSources();
            if (count == 0) {
                layer.alert("主键为" + attr.MyPK + "名称为" + attr.Name + "的图片控件信息丢失，请联系管理员111");
                return "";
            }
            //解析图片
            if (frmImg.ImgAppType == 0) { //图片类型
                //数据来源为本地.
                var imgSrc = '';
                if (frmImg.ImgSrcType == 0) {
                    //替换参数
                    var frmPath = frmImg.ImgPath;
                    frmPath = frmPath.replace('＠', '@');
                    frmPath = frmPath.replace('@basePath', basePath);
                    frmPath = frmPath.replace('@basePath', basePath);
                    imgSrc = DealJsonExp(frmData.MainTable[0], frmPath);
                }

                //数据来源为指定路径.
                if (frmImg.ImgSrcType == 1) {
                    var url = frmImg.ImgURL;
                    url = url.replace('＠', '@');
                    url = url.replace('@basePath', basePath);
                    imgSrc = DealJsonExp(frmData.MainTable[0], url);
                }
                // 由于火狐 不支持onerror 所以 判断图片是否存在放到服务器端
                if (imgSrc == "" || imgSrc == null)
                    imgSrc = "../DataUser/ICON/CCFlow/LogBig.png";

                var style = "text-align:center;";
                if (attr.UIWidth == 0)
                    style += "width:100%;";
                else
                    style += "width:" + attr.UIWidth + "px;";

                if (attr.UIHeight == 0)
                    style += "Height:100%;";
                else
                    style += "Height:" + attr.UIHeight + "px;";
                return "<img src='" + imgSrc + "' style='" + style + "'  />";

            }
            break;
        case 12://单图片附件（可以上传)
            //获取图片控件的信息
            if (frmData.Sys_FrmImgAth == undefined || frmData.Sys_FrmImgAth.length == 0)
                return;
            var frmImgs = $.grep(frmData.Sys_FrmImgAth, function (item, i) {
                return item.MyPK == attr.MyPK;
            });
            if (frmImgs.length == 0) {
                alert("主键为" + attr.MyPK + "名称为" + attr.Name + "的图片控件信息丢失，请联系管理员");
                return "";
            }

            var frmImg = frmImgs[0];
            var imgSrc = basePath + "/DataUser/ICON/CCFlow/LogBig.png";

            //获取数据
            if (frmImg.FK_MapData.indexOf("ND") != -1)
                imgSrc = basePath + "/DataUser/ImgAth/Data/" + frmImg.CtrlID + "_" + pageData.WorkID + ".png";
            else
                imgSrc = basePath + "/DataUser/ImgAth/Data/" + frmImg.FK_MapData + "_" + frmImg.CtrlID + "_" + pageData.WorkID + ".png";

            var _html = "";
            if (frmImg.IsEdit == "1" && pageData.IsReadonly != "1") {
                var url = dynamicHandler + "?DoType=HttpHandler&DoMethod=FrmImgAthDB_Upload&HttpHandlerName=BP.WF.HttpHandler.WF_CCForm&FK_MapData=" + frmData.Sys_MapData[0].No + "&CtrlID=" + frmImg.CtrlID + "&RefPKVal=" + pageData.WorkID
                _html += "<div>";
                _html += "<fieldset>";
                _html += "<legend style='margin-bottom:0px'>";
                _html += ' <div class="layui-btn layui-btn-primary" id="editimg" data-ref="' + frmImg.MyPK + '" data-info="' + url + '">修改图片</div >';
                _html += "</legend>";
                _html += "<img  id='Img" + frmImg.MyPK + "' name='Img" + frmImg.MyPK + "' src='" + imgSrc + "' onerror=\"this.src='" + basePath + "/DataUser/ICON/CCFlow/LogBig.png'\" style='width:" + frmImg.W + "px;height:" + frmImg.H + "px;' onclick='imgShow(this)'/>";
                _html += "</fieldset>";
                _html += "</div>";
                return _html;
            } else {
                _html += "<div>";
                _html += "<img id='Img" + frmImg.MyPK + "' name='Img" + frmImg.MyPK + "' src='" + imgSrc + "' \"this.src='" + basePath + "/DataUser/ICON/CCFlow/LogBig.png'\" style='width:" + frmImg.W + "px;height:" + frmImg.H + "px;' onclick='imgShow(this)'/>";
                _html += "</div>";
                return _html;
            }
            break;
        case 18://按钮
            return Gener_Btn(frmData, attr);
        default:
            return "";
    }
}
//初始化 框架
function Ele_Frame(frmData, gf) {
    var frame = null;
    try {
        frame = new Entity("BP.Sys.MapFrame", gf.CtrlID);
    } catch (e) {
        layer.alert("没有找到框架的定义，请与管理员联系。");
        return;
    }

    if (frame == null)
        return;

    //获取框架的类型 0 自定义URL 1 地图开发 2流程轨迹表 3流程轨迹图
    var urlType = frame.UrlSrcType;

    var eleHtml = '';
    var url = "";
    if (urlType == 0) {
        url = frame.URL;
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

        //3.替换表单中的参数
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

        //4.追加GenerWorkFlow AtPara中的参数
        var gwf = frmData.WF_GenerWorkFlow;
        if (gwf != null && gwf != undefined) {
            gwf = gwf[0];
            var atPara = gwf.AtPara;
            if (atPara != null && atPara != "") {
                atPara = atPara.replace(/@/g, '&');
                url = url + atPara;
            }
        }
    }
    var baseUrl = "./";
    if (currentURL.indexOf("AdminFrm.htm") != -1)
        baseUrl = "../../";
    if (currentURL.indexOf("CCBill") != -1 || currentURL.indexOf("CCForm") != -1)
        baseUrl = "../";
    if (urlType == 2) //轨迹表
        url = baseUrl + "WorkOpt/OneWork/Table.htm?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.WorkID + "&FID=" + pageData.FID;
    if (urlType == 3)//轨迹图
        url = baseUrl + "WorkOpt/OneWork/TimeBase.htm?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.WorkID + "&FID=" + pageData.FID;

    eleHtml += "<iframe style='width:100%;height:" + frame.H + "px;' ID='" + frame.MyPK + "'    src='" + url + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    return eleHtml;
}
//初始化从表
function Ele_Dtl(frmDtl, isComPare) {
    if (isComPare == true)
        return "";
    var src = "";
    var href = GetHrefUrl();
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');

    var ensName = frmDtl.No;
    if (ensName == undefined) {
        layer.alert('系统错误,请找管理员联系');
        return;
    }
    var baseUrl = "./CCForm/";
    if (currentURL.indexOf("AdminFrm.htm") != -1)
        baseUrl = "../../CCForm/";
    if (currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
        baseUrl = "../CCForm/";
    if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("FrmDBVer.htm") != -1 || currentURL.indexOf("DtlFrm.htm") != -1)
        baseUrl = "./";

    //表格模式
    if (frmDtl.ListShowModel == "0")
        src = baseUrl + "Dtl2017.htm?1=1";
    if (frmDtl.ListShowModel == "1")
        src = baseUrl + "DtlCard.htm?1=1";
    if (frmDtl.ListShowModel == "2") {
        if (frmDtl.UrlDtl == null || frmDtl.UrlDtl == undefined || frmDtl.UrlDtl == "")
            return "从表" + frmDtl.Name + "没有设置URL,请在" + frmDtl.FK_MapData + "_Self.js中解析";
        src = basePath + "/" + frmDtl.UrlDtl;
        if (src.indexOf("?") == -1)
            src += "?1=1";
    }

    var refpk = this.pageData.WorkID;
    if (refpk == undefined)
        refpk = pageData.OID;
    src += "&EnsName=" + frmDtl.No + "&RefPKVal=" + refpk + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=" + (isReadonly == true ? 1 : 0) + "&" + urlParam + "&Version=1&FrmType=0";
    src = src.replace('//', '/');

    return "<iframe style='width:100%;height:100%' name='Dtl' ID='Frame_" + frmDtl.No + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>";
}

/**
 * 按钮组件的解析
 * @param {any} frmData
 * @param {any} mapAttr
 */
function Gener_Btn(frmData, mapAttr) {
    var btnId = mapAttr.KeyOfEn;
    if (btnId == null || btnId == undefined || btnId == "")
        btnId = mapAttr.MyPK;

    var doc = mapAttr.Tag;
    doc = doc.replace("~", "'");
    var eventType = mapAttr.UIIsEnable;
    var onclick = "";
    if (eventType == 0) //禁用
        onclick = "disabled='disabled' style='background:gray;'";

    if (eventType == 1) { //运行URL
        var attrs = frmData.Sys_MapAttr;
        for (var i = 0; i < attrs.length; i++) {
            var attr = attrs[i];
            if (doc.indexOf('@' + attr.KeyOfEn) > 0)
                doc = doc.replace('@' + attr.KeyOfEn, frmData.MainTable[0][obj.KeyOfEn]);
        }

        var oid = GetQueryString("OID");
        if (oid == undefined || oid == "");
        oid = GetQueryString("WorkID");
        var FK_Node = GetQueryString("FK_Node");
        var FK_Flow = GetQueryString("FK_Flow");
        var userNo = webUser.No;
        var SID = webUser.Token;
        if (SID == undefined)
            SID = "";
        if (doc.indexOf("?") == -1)
            doc = doc + "?1=1";
        doc = doc + "&OID=" + oid + "&FK_Node=" + FK_Node + "&FK_Flow=" + FK_Flow + "&UserNo=" + userNo + "&Token=" + SID;
        onclick = "onclick='window.open(\"" + doc + ")'";
    }

    ////运行URL
    if (eventType == 2) {
        if (doc.indexOf("(") == -1)
            doc = doc + "()";
        doc = doc.replace(/~/g, "'");
        onclick = 'onclick="' + doc + '"';
    }
    return "<button type='button' class='layui-btn layui-btn-primary layui-btn-sm' id='" + btnId + "' type='button' " + onclick + ">" + mapAttr.Name + "</button>";
}
/**
    * 获取表单显示的列数
    * @param {any} tableColType
    */
function getTableCol(tableColType) {
    switch (tableColType) {
        case 0:
            return 4;
        case 1:
            return 6;
        case 2:
            return 3;
        default:
            return 4;
    }
}

/**
* 获取字段占的列数
* @param {any} colSpan
* @param {any} tabCol
*/
function getColSpanClass(colSpan, tabCol) {
    if (tabCol == 4) {
        switch (colSpan) {
            case 1:
                return "layui-col-md4 layui-col-xs8";
            case 2:
                return "layui-col-md6 layui-col-xs8";
            case 3:
                return "layui-col-md10 layui-col-xs8";
            case 4:
                return "layui-col-md12 layui-col-xm12";
            default:
                return "layui-col-md4 layui-col-xs8";
        }
    }
    if (tabCol == 6) {
        switch (colSpan) {
            case 1:
                return "layui-col-xs3";
            case 2:
                return "layui-col-xs4";
            case 3:
                return "layui-col-xs7";
            case 4:
                return "layui-col-xs8";
            case 5:
                return "layui-col-xs11";
            case 6:
                return "layui-col-xs12";
            default:
                return "layui-col-xs3";
        }
    }
}
/**
    * 获取标签占的列数
    * @param {any} LabelColSpan
    * @param {any} tabCol
    */
function getLabelColSpanClass(LabelColSpan, tabCol) {
    if (tabCol == 4) {
        switch (LabelColSpan) {
            case 1:
                return "layui-col-md2 layui-col-xs4";
            case 2:
                return "layui-col-md6 layui-col-xs4";
            case 3:
                return "layui-col-md8 layui-col-xs4";
            case 4:
                return "layui-col-md12 layui-col-xs12";
            default:
                return "layui-col-md2 layui-col-xs4";
        }
    }
    if (tabCol == 6) {
        switch (LabelColSpan) {
            case 1:
                return "layui-col-xs1";
            case 2:
                return "layui-col-xs4";
            case 3:
                return "layui-col-xs5";
            case 4:
                return "layui-col-xs8";
            case 5:
                return "layui-col-xs9";
            case 6:
                return "layui-col-xs12";
            default:
                return "layui-col-xs1";
        }
    }
}
