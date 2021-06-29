

function GenerFoolFrm(wn) {
    debugger 
    flowData = wn;

    //初始化Sys_MapData
    var h = flowData.Sys_MapData[0].FrmH;
    var w = flowData.Sys_MapData[0].FrmW;
    var frmShowType = flowData.Sys_MapData[0].FrmShowType; //0 普通方式 1 页签方式
    if (frmShowType == null || frmShowType == undefined || frmShowType == "")
        frmShowType = 0;
    var node = flowData.WF_Node[0];
    var tableCol = flowData.Sys_MapData[0].TableCol;
    if (tableCol == 0)
        tableCol = 4;
    else if (tableCol == 1)
        tableCol = 6;
    else if (tableCol == 2)
        tableCol = 3;
    else
        tableCol = 4;

    $('#CCForm').html('');

    var tableWidth = w - 40;
    var html = "<table style='width:100%;' class='FoolFrmTable' >";

    var frmName = flowData.Sys_MapData[0].Name;
    var Sys_GroupFields = flowData.Sys_GroupField;

    html += "<tr  class='FoolFrmTitleTR' >";
    html += "<td colspan='" + tableCol + "' class='FoolFrmTitleTD'>";

    html += " <div style='padding:10px;font-size: 18px;text-align:center' class='FoolFrmTitleIcon'  >";
    html += " <img src='../DataUser/ICON/LogBiger.png'  style='height:50px; float:left;margin-top:-8px;' />";
    html += " </div>";

    html += " <div id='FoolFrmTitleLable' style='float:right;margin-top:8px' >";
    html += frmName;
    html += "  </div>";
    html += "</td>";

    html += "</tr>";
    if (frmShowType == 1) {
        html += "</table>";
        html += "<div class='tabbable' ><ul class='nav nav-tabs' id='tabDiv'>";

        for (var i = 0; i < Sys_GroupFields.length; i++) {
            var gf = Sys_GroupFields[i];
            if (i == 0)
                html += "<li class='active'><a data-toggle='tab' href='#" + gf.OID + "'>" + gf.Lab + "</a></li>";
            else
                html += "<li><a data-toggle='tab' href='#" + gf.OID + "'>" + gf.Lab + "</a></li>";
        }

        html += "</ul>";

        //增加表情中的内容
        html += "<div class='tab-content'>";
        for (var i = 0; i < Sys_GroupFields.length; i++) {
            var gf = Sys_GroupFields[i];
            if (i == 0)
                html += "<div id='" + gf.OID + "' class='tab-pane fade active in'>";
            else
                html += "<div id='" + gf.OID + "' class='tab-pane fade'>";
            html += '<form class="form-horizontal" role="form">';
            html += "<table style='width:100%;' >";
            //从表..
            if (gf.CtrlType == 'Dtl') {
                var dtls = flowData.Sys_MapDtl;

                for (var k = 0; k < dtls.length; k++) {

                    var dtl = dtls[k];
                    if (dtl.No != gf.CtrlID)
                        continue;
                    if (dtl.IsView == "0")
                        continue;
                    html += "<tr>";
                    html += "  <td colspan='" + tableCol + "'  >";

                    html += Ele_Dtl(dtl);

                    html += "  </td>";
                    html += "</tr>";
                }
                html += "</table>";
                html += "</form>";
                html += "</div>";
                continue;
            }


            //附件类的控件.
            if (gf.CtrlType == 'Ath') {

                //获取附件的主键
                var MyPK = gf.CtrlID;
                if (MyPK == "")
                    continue;
                //创建附件描述信息.
                var aths = $.grep(flowData.Sys_FrmAttachment, function (ath) { return ath.MyPK == gf.CtrlID });
                var ath = aths.length > 0 ? aths[0] : null;
                var athInfo = "";
                if (ath == null)
                    athInfo = "附件" + gf.CtrlID + "信息丢失";
                else
                    athInfo = "<div id='Div_" + ath.MyPK + "'></div>";

                if (ath != null && (ath.IsVisable == "0" || ath.NoOfObj == "FrmWorkCheck"))
                    continue;
                html += "<tr class='FoolFrmFieldTR' >";
                html += "  <td colspan='" + tableCol + "' class='FoolFrmFieldTD'  >";
                html += athInfo;
                html += "  </td>";
                html += "</tr>";
                html += "</table>";
                html += "</form>";
                html += "</div>";
                continue;
            }


            //框架类的控件.
            if (gf.CtrlType == 'Frame') {
                html += "<tr class='FoolFrmFieldTR' >";
                html += "  <td colspan='" + tableCol + "' class='FoolFrmFieldTD'  >";
                html += Ele_Frame(flowData, gf);
                html += "  </td>";
                html += "</tr>";
                html += "</table>";
                html += "</form>";
                html += "</div>";
                continue;
            }

            //审核组件..
            if (gf.CtrlType == 'FWC' && node.FWCSta != 0) {
                if (window.document.location.href.indexOf("AdminFrm.htm") != -1)
                    continue;

                html += "<tr class='FoolFrmFieldTR' >";
                html += "  <td colspan='" + tableCol + "' class='FoolFrmFieldTD' >";

                html += Ele_FrmCheck(node);

                html += "  </td>";
                html += "</tr>";
                html += "</table>";
                html += "</form>";
                html += "</div>";
                continue;
            }


            //字段类的控件.
            if (gf.CtrlType == '' || gf.CtrlType == null) {
                if (tableCol == 4 || tableCol == 6)
                    html += InitMapAttr(flowData.Sys_MapAttr, flowData, gf.OID, tableCol);
                else if (tableCol == 3)
                    html += InitThreeColMapAttr(flowData.Sys_MapAttr, flowData, gf.OID, tableCol);
                html += "</table>";
                html += "</form>";
                html += "</div>";
                continue;
            }

            //父子流程
            if (gf.CtrlType == 'SubFlow') {

                html += "<tr class='FoolFrmFieldTR' >";
                html += "  <td colspan='" + tableCol + "' class='FoolFrmFieldTD' >";
                //html += Ele_SubFlow(node);
                //引入SubFlow.js
                Skip.addJs(ccbpmPath + "/WF/WorkOpt/SubFlow.js");
                html += SubFlow_Init(node);

                html += "  </td>";
                html += "</tr>";
                html += "</table>";
                html += "</form>";
                html += "</div>";

                continue;
            }

        }

        html += "</div>";
    }
    if (frmShowType == 0) {
        //遍历循环生成 listview
        for (var i = 0; i < Sys_GroupFields.length; i++) {

            var gf = Sys_GroupFields[i];

            //从表..
            if (gf.CtrlType == 'Dtl') {
                var dtls = flowData.Sys_MapDtl;
                var mydtls = $.grep(dtls, function (dtl) {
                    return dtl.No == gf.CtrlID && dtl.IsView != 0;
                })

                if (mydtls.length == 0)
                    continue;
                var dtl = mydtls[0];
                html += "<tr class='FoolFrmGroupBarTR' >";
                html += "  <th class='FoolFrmGroupBarTD'  id='THDtl_" + gf.CtrlID + "' colspan='" + tableCol + "' >";
                html += "   <div style='float:left;display:inline'  class='FoolFrmGroupBarTD'>" + gf.Lab + "</div>";
                html += "   <div style='float:right;display:inline' class='FoolFrmGroupBarTD'><img title='全屏显示' style='width: 30px; padding: 0px 5px; cursor: pointer;' src='./Img/Full.png' onclick='WindowOpenDtl(\"" + gf.CtrlID + "\")' /></div>";

                html += "  </th>";
                html += "</tr>";
                html += "<tr class='FoolFrmFieldTR' >";
                html += "  <td id='Dtl_" + dtl.No + "' colspan='" + tableCol + "' class='FoolFrmFieldCtrl' >";

                html += Ele_Dtl(dtl);

                html += "  </td>";
                html += "</tr>";
                continue;
            }


            //附件类的控件.
            if (gf.CtrlType == 'Ath') {
                if (gf.CtrlID == "")
                    continue;
                //创建附件描述信息.
                var aths = $.grep(flowData.Sys_FrmAttachment, function (ath) { return ath.MyPK == gf.CtrlID });
                var ath = aths.length > 0 ? aths[0] : null;
                var athInfo = "";
                if (ath == null) {
                    athInfo = "附件" + gf.CtrlID + "信息丢失";
                    continue;
                }
                else
                    athInfo = "<div id='Div_" + ath.MyPK + "' name='Ath'></div>";

                if (ath != null && (ath.IsVisable == "0" || ath.NoOfObj == "FrmWorkCheck"))
                    continue;
                html += "<tr class='FoolFrmFieldTR' >";
                html += "  <td id='THAth_" + ath.MyPK + "' colspan='" + tableCol + "'  class='FoolFrmGroupBarTD' >" + gf.Lab + "</td>";
                html += "</tr>";
                html += "<tr class='FoolFrmFieldTR' >";
                html += "<td id='Ath_" + ath.MyPK + "' colspan='" + tableCol + "' class='FoolFrmFieldCtrl'>";
                html += athInfo;
                html += "  </td>";
                html += "</tr>";
                continue;
            }


            //框架类的控件.
            if (gf.CtrlType == 'Frame') {

                html += "<tr class='FoolFrmGroupBarTR' >";
                html += "  <td colspan='" + tableCol + "'  class='FoolFrmGroupTD'>" + gf.Lab + "</td>";
                html += "</tr>";
                html += "<tr class='FoolFrmFieldTR' >";
                html += "  <td colspan='" + tableCol + "' class='FoolFrmFieldCtrl'>";
                html += Ele_Frame(flowData, gf);
                html += "  </td>";
                html += "</tr>";
                continue;
            }

            //审核组件..
            if (gf.CtrlType == 'FWC' && node.FWCSta != 0) {
                if (node.FormType == 10 && gf.FrmID != 'ND' + node.NodeID)
                    continue;

                html += "<tr class='FoolFrmFieldTR' id='WorkCheck_Group'>";
                html += "  <th colspan='" + tableCol + "'  class='FoolFrmGroupBarTD'>" + gf.Lab + "</th>";
                html += "</tr>";

                html += "<tr class='FoolFrmFieldTR' >";
                html += "  <td colspan='" + tableCol + "' class='FoolFrmFieldCtrl'>";

                html += Ele_FrmCheck(node);

                html += "  </td>";
                html += "</tr>";

                continue;
            }


            //字段类的控件.
            if (gf.CtrlType == '' || gf.CtrlType == null) {
                if (gf.ShowType != 2) {
                    html += "<tr class='FoolFrmFieldTR' >";
                    html += "  <th colspan='" + tableCol + "' class='FoolFrmGroupBarTD' >" + gf.Lab + "</th>";
                    html += "</tr>";
                }
                if (tableCol == 4 || tableCol == 6)
                    html += InitMapAttr(flowData.Sys_MapAttr, flowData, gf.OID, tableCol);
                else if (tableCol == 3)
                    html += InitThreeColMapAttr(flowData.Sys_MapAttr, flowData, gf.OID, tableCol);
                continue;
            }

            //父子流程
            if (gf.CtrlType == 'SubFlow') {
                html += "<tr class='FoolFrmFieldTR' >";
                html += "  <th colspan='" + tableCol + "'  class='FoolFrmGroupBarTD' >" + gf.Lab + "</th>";
                html += "</tr>";

                html += "<tr class='FoolFrmFieldTR' >";
                html += "  <td colspan='" + tableCol + "' class='FoolFrmFieldCtrl'>";

                //html += Ele_SubFlow(node);
                Skip.addJs(ccbpmPath + "/WF/WorkOpt/SubFlow.js");


                //说明是累加表单.
                if (gf.FrmID.indexOf(node.NodeID) == -1) {

                    var myNodeID = gf.FrmID.substring(2);
                    var myNode = new Entity("BP.WF.Node", myNodeID);
                    html += "<div id='SubFlow'>" + SubFlow_Init(myNode) + "</div>";
                }
                else {
                    html += "<div id='SubFlow'>" + SubFlow_Init(node) + "</div>";
                }

                html += "  </td>";
                html += "</tr>";

                continue;
            }
        }
    }

    html += "</table>";
    if (flowData.Sys_FrmImgAth.length > 0) {
        html += "<input type='hidden' id='imgSrc'/>";
    }

    $('#CCForm').html(html);

    //表格附件
    $.each(flowData.Sys_FrmAttachment, function (idex, ath) {
        if ($("#Div_" + ath.MyPK).length == 1)
            AthTable_Init(ath, "Div_" + ath.MyPK);
    });

    //字段附件
    var aths = $(".athModel");
    $.each(aths, function (idx, ath) {

        //获取ID
        var name = $(ath).attr('id');
        var keyOfEn = name.replace("athModel_", "");
        $("#Lab_" + keyOfEn).html("<div style='text-align:left'>" + $("#Lab_" + keyOfEn).text() + "</div>");
    });


}

//解析表单是三列的情况
function InitThreeColMapAttr(Sys_MapAttr, flowData, groupID, tableCol) {
    var html = "";
    var isDropTR = true;

    var lab = "";
    var colSpan = 1;
    var rowSpan = 1;
    var textColSpan = 1;
    var textWidth = "33%";
    var colWidth = "33%";

    //记录一行已占用的列输
    var UseColSpan = 0;
    var IsMiddle = false;
    //跨行问题
    for (var i = 0; i < Sys_MapAttr.length; i++) {
        var attr = Sys_MapAttr[i];

        if (attr.GroupID != groupID || attr.UIVisible == 0 || attr.UIContralType == 16)
            continue;


        rowSpan = attr.RowSpan;
        colSpan = attr.ColSpan;
        textColSpan = attr.TextColSpan;


        colWidth = 33 * parseInt(colSpan) + "%";
        textWidth = 33 * parseInt(textColSpan) + "%";

        //大文本备注信息 独占一行
        if (attr.UIContralType == 60) {
            //获取文本信息
            var filename = basePath + "/DataUser/CCForm/BigNoteHtmlText/" + attr.FK_MapData + ".htm";
            var htmlobj = $.ajax({ url: filename, async: false });
            var str = htmlobj.responseText;
            if (htmlobj.status == 404)
                str = filename + "这个文件不存在，请联系管理员";
            html += "<tr class='FoolFrmFieldTR' >";
            html += "<td  ColSpan='" + tableCol + "' class='FoolFrmFieldCtrl' style='text-align:left:height:auto'>" + str + "</td>";
            html += "</tr>";
            isDropTR = true;
            UseColSpan = 0;
            continue;
        }

        //解析Lab 1、文本类型、DDL类型、RB类型、扩张（图片、附件、超链接）
        lab = GetLab(flowData, attr);

        //跨列设置(显示的是文本)
        if (colSpan == 0) {

            if (textColSpan == tableCol) {
                rowSpan = 1;
                html += "<td  class='FoolFrmFieldName' rowSpan=" + rowSpan + " colSpan=" + textColSpan + ">" + lab + "</td>";
                isDropTR = true;
                continue;
            }
            //线性展示都跨一个单元格
            //换行的情况
            if (isDropTR == true) {
                html += "<tr class='FoolFrmFieldTR' >";
                UseColSpan = 0;
                UseColSpan += colSpan + textColSpan;
                html += "<td class='FoolFrmFieldName' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colSpan=" + textColSpan + ">" + lab + "</td>";

                if (UseColSpan == tableCol) {
                    isDropTR = true;
                } else {
                    isDropTR = false;
                }
                continue;
            }

            if (isDropTR == false) {
                UseColSpan += colSpan + textColSpan;
                html += "<td class='FoolFrmFieldName' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colSpan=" + textColSpan + ">" + lab + "</td>";
                if (UseColSpan == tableCol) {
                    html += "</tr>";
                    isDropTR = true;
                } else {
                    isDropTR = false;
                }
                continue;
            }
        }
        //解析占一行的情况
        if (colSpan == tableCol) {
            html += "<tr class='FoolFrmFieldTR' >";
            html += "<td  ColSpan='" + colSpan + "' rowSpan=" + rowSpan + " class='FoolFrmFieldName' style='text-align:left'>" + lab + "</br>";
            html += InitMapAttrOfCtrlFool(flowData, attr);
            html += "</td>";
            html += "</tr>";
            isDropTR = true;
            UseColSpan = 0;
            continue;
        }

        //换行的情况
        if (isDropTR == true) {
            html += "<tr class='FoolFrmFieldTR' >";
            UseColSpan = 0;

            UseColSpan += colSpan;
            html += "<td  id='TD_" + attr.KeyOfEn + "' class='FoolFrmFieldName' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " ColSpan=" + colSpan + " class='tdSpan'>" + lab + "<br/>";
            html += InitMapAttrOfCtrlFool(flowData, attr);
            html += "</td>";
            if (UseColSpan == tableCol) {
                isDropTR = true;
            } else {
                isDropTR = false;
            }


            continue;
        }

        if (isDropTR == false) {

            UseColSpan += colSpan;
            html += "<td  id='TD_" + attr.KeyOfEn + "' class='FoolFrmFieldName' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " ColSpan=" + colSpan + " class='tdSpan'>" + lab + "<br/>";
            html += InitMapAttrOfCtrlFool(flowData, attr);
            html += "</td>";

            if (UseColSpan == tableCol) {
                html += "</tr>";
                isDropTR = true;
            } else {
                isDropTR = false;
            }


            continue;
        }
    }

    return html;
}

//解析表单字段 MapAttr.
function InitMapAttr(Sys_MapAttr, flowData, groupID, tableCol) {

    var html = "";
    var isDropTR = true;
    //右侧跨行
    var IsShowRight = true; // 是否显示右侧列
    var rRowSpan = 0; //跨的行数
    var ruRowSpan = 0; //已近解析的行数
    var ruColSpan = 0; //该跨行总共跨的列数

    //左侧跨行
    var IsShowLeft = true; // 是否显示左侧列
    var lRowSpan = 0; //跨的行数
    var luRowSpan = 0; //已近解析的行数
    var luColSpan = 0; //该跨行总共跨的列数

    //记录一行已占用的列输
    var UseColSpan = 0;

    //跨列的字段
    var colSpan = 1;
    var textColSpan = 2;
    var textWidth = "15%";
    var colWidth = 35;

    var lab = "";

    //跨行问题
    for (var i = 0; i < Sys_MapAttr.length; i++) {

        var attr = Sys_MapAttr[i];

        if (attr.GroupID != groupID || attr.UIVisible == 0 || attr.UIContralType == 16)
            continue;

        //赋值
        rowSpan = parseInt(attr.RowSpan);
        colSpan = parseInt(attr.ColSpan);
        textColSpan = parseInt(attr.TextColSpan);
        if (tableCol == 4) {
            colWidth = 35 * parseInt(colSpan) + "%";
            textWidth = 15 * parseInt(textColSpan) + "%";
        } else {
            colWidth = 23 * parseInt(colSpan) + "%";
            textWidth = 10 * parseInt(textColSpan) + "%";
        }

        //大文本备注信息 独占一行
        if (attr.UIContralType == 60) {
            //获取文本信息
            var filename = basePath + "/DataUser/CCForm/BigNoteHtmlText/" + attr.FK_MapData + ".htm";
            var htmlobj = $.ajax({ url: filename, async: false });
            var str = htmlobj.responseText;
            if (htmlobj.status == 404)
                str = filename + "这个文件不存在，请联系管理员";
            html += "<tr class='FoolFrmFieldTR' >";
            html += "<td  ColSpan='" + tableCol + "' class='FoolFrmFieldCtrl' style='text-align:left:height:auto'>" + str + "</td>";
            html += "</tr>";
            isDropTR = true;
            UseColSpan = 0;
            continue;
        }

        var labClass = "FoolFrmFieldName";
        if (attr.UIContralType == 18)
            labClass = "FDesc";

        //解析Lab 1、文本类型、DDL类型、RB类型、扩张（图片、附件、超链接）
        lab = GetLab(flowData, attr);
        if (colSpan == 0) {
            //占一行
            if (textColSpan >= tableCol) {
                if (isDropTR == false) {
                    var unUseColSpan = tableCol - UseColSpan;
                    html += "<td colspan=" + unUseColSpan + "></td>";
                    html += "</tr>";
                }
                isDropTR = true;
                rowSpan = 1;
                html += "<tr class='FoolFrmFieldTR' >";

                html += "<td  colSpan=" + tableCol + " rowSpan=" + rowSpan + " class='" + labClass + "' style='text-align:left'>" + lab + "</br>";
                html += "</tr>";
                continue;

            }
            //线性展示都跨一个单元格
            if (isDropTR == true) {
                html += "<tr class='FoolFrmFieldTR' >";
                UseColSpan = 0;
                luColSpan = 0;
                if (IsShowLeft == true) {
                    UseColSpan += colSpan + textColSpan + ruColSpan;
                    lRowSpan = rowSpan;
                    luColSpan += colSpan + textColSpan;
                    html += "<td  class='" + labClass + "' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colSpan=" + textColSpan + ">" + lab + "</td>";
                    if (rowSpan != 1) {
                        IsShowLeft = false;
                    }

                }
                if (UseColSpan >= tableCol) {
                    ruRowSpan++;
                    isDropTR = true;
                } else {
                    isDropTR = false;
                }

                //复位右侧信息
                if (ruRowSpan == rRowSpan) {
                    ruRowSpan = 0;
                    luRowSpan = 0;
                    rRowSpan = 0;
                    IsShowRight = true;
                    if (rowSpan == 1)
                        luColSpan = 0;
                    ruColSpan = 0;
                }

                if (IsShowRight == false && (UseColSpan == tableCol)) {
                    html += "</tr>";
                    isDropTR = true;
                    UseColSpan = ruColSpan;

                }
                continue;
            }

            if (isDropTR == false) {
                ruColSpan = 0;
                if (IsShowRight == true) {
                    UseColSpan += colSpan + textColSpan;
                    if (UseColSpan > tableCol) {
                        //需要换行，补齐缺失的空格
                        var count = tableCol - (UseColSpan - colSpan - textColSpan);
                        for (var k = 0; k < count; k++) {
                            html += "<td class='FoolFrmFieldCtrl'></td>";
                        }

                        html += "</tr>";
                        html += "<tr>";
                        UseColSpan = colSpan + textColSpan + ruColSpan;
                        lRowSpan = rowSpan;
                        luColSpan = colSpan + textColSpan;
                        html += "<td  class='" + labClass + "' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colSpan=" + textColSpan + ">" + lab + "</td>";
                        if (rowSpan != 1) {
                            IsShowLeft = false;
                        }
                        if (UseColSpan >= tableCol) {
                            ruRowSpan++;
                            isDropTR = true;
                        } else {
                            isDropTR = false;
                        }

                        //复位右侧信息
                        if (ruRowSpan == rRowSpan) {
                            ruRowSpan = 0;
                            luRowSpan = 0;
                            rRowSpan = 0;
                            IsShowRight = true;
                            if (rowSpan == 1)
                                luColSpan = 0;
                            ruColSpan = 0;
                        }

                        if (IsShowRight == false && (UseColSpan == tableCol)) {
                            html += "</tr>";
                            isDropTR = true;
                            UseColSpan = ruColSpan;

                        }
                        continue;

                    } else {
                        rRowSpan = rowSpan;
                        ruColSpan += colSpan + textColSpan;
                        html += "<td  class='" + labClass + "' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colSpan=" + textColSpan + ">" + lab + "</td>";
                        if (UseColSpan == tableCol) {
                            isDropTR = true;
                            if (rowSpan != 1) {
                                ruRowSpan++;
                            }
                        }
                        if (rowSpan != 1) {
                            IsShowRight = false;
                            lRowSpan = rowSpan;
                        }
                    }

                }

                if (UseColSpan == tableCol) {
                    luRowSpan++;
                    html += "</tr>";
                }

                //复位左侧信息
                if (luRowSpan == lRowSpan) {
                    luRowSpan = 0;
                    ruRowSpan = 0;
                    lRowSpan = 0;
                    IsShowLeft = true;
                    ruColSpan = 0;

                }

                if (IsShowLeft == false && (UseColSpan == tableCol)) {
                    html += "<tr class='FoolFrmFieldTR' >";
                    UseColSpan = 0;
                    isDropTR = false;
                    UseColSpan = luColSpan;
                }
                continue;
            }

        }

        //线性展示并且colspan=4
        if (colSpan >= tableCol) {
            if (isDropTR == false) {
                var unUseColSpan = tableCol - UseColSpan;
                html += "<td colspan=" + unUseColSpan + "></td>";
                html += "</tr>";
            }

            isDropTR = true;
            rowSpan = 1;

            html += "<tr class='FoolFrmFieldTR' >";
            html += "<td  ColSpan='" + colSpan + "' rowSpan=" + rowSpan + "  class='" + labClass + "' style='text-align:left'>" + lab + "</br>";
            html += "</tr>";
            html += "<tr class='FoolFrmFieldTR' >";
            html += "<td  id='TD_" + attr.KeyOfEn + "' ColSpan='" + colSpan + "' rowSpan=" + rowSpan + " class='FoolFrmFieldCtrl' style='text-align:left'>";

            html += InitMapAttrOfCtrlFool(flowData, attr);

            html += "</td>";
            html += "</tr>";
            continue;
        }
        var sumColSpan = colSpan + textColSpan;
        if (sumColSpan == tableCol) {
            if (isDropTR == false) {
                var unUseColSpan = tableCol - UseColSpan;
                html += "<td colspan=" + unUseColSpan + "></td>";
                html += "</tr>";
            }
            rowSpan = 1;
            isDropTR = true;
            html += "<tr class='FoolFrmFieldTR' >";
            html += "<td  id='TD_" + attr.KeyOfEn + "'  class='" + labClass + "' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " ColSpan=" + textColSpan + " class='tdSpan'>" + lab + "</td>";
            if (attr.IsSigan == "5") 
                html += "<td  class='FoolFrmFieldCtrl' id='TD_" + attr.KeyOfEn + "'  style='text-align: center;width:" + colWidth + ";' ColSpan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
            else
                html += "<td  class='FoolFrmFieldCtrl' id='TD_" + attr.KeyOfEn + "'  style='width:" + colWidth + ";' ColSpan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
            html += InitMapAttrOfCtrlFool(flowData, attr);
            html += "</td>";
            html += "</tr>";
            isDropTR = true;
            continue;
        }

        //换行的情况
        if (isDropTR == true) {
            html += "<tr class='FoolFrmFieldTR' >";
            UseColSpan = 0;
            luColSpan = 0;
            if (IsShowLeft == true) {
                UseColSpan += colSpan + textColSpan + ruColSpan;
                lRowSpan = rowSpan;
                luColSpan += colSpan + textColSpan;
                if (attr.MyDataType == 4) {
                    colSpan = colSpan + textColSpan;
                    colWidth = (parseInt(colSpan) * 23 + 10 * parseInt(textColSpan)) + "%";
                } else {
                    html += "<td  id='TD_" + attr.KeyOfEn + "'  class='" + labClass + "' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " ColSpan=" + textColSpan + " class='tdSpan'>" + lab + "</td>";
                }
                if (attr.IsSigan == "5") {
                    html += "<td  class='FoolFrmFieldCtrl' id='TD_" + attr.KeyOfEn + "'  style='text-align: center;width:" + colWidth + ";' ColSpan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
                }
                else
                    html += "<td  class='FoolFrmFieldCtrl' id='TD_" + attr.KeyOfEn + "'  style='width:" + colWidth + ";' ColSpan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
                html += InitMapAttrOfCtrlFool(flowData, attr);
                html += "</td>";
                if (rowSpan != 1) {
                    IsShowLeft = false;
                }

            }
            if (UseColSpan == tableCol) {
                ruRowSpan++;
                isDropTR = true;

            } else {
                isDropTR = false;
            }

            //复位右侧信息
            if (ruRowSpan == rRowSpan) {
                ruRowSpan = 0;
                luRowSpan = 0;
                rRowSpan = 0;
                IsShowRight = true;
                if (rowSpan == 1)
                    luColSpan = 0;
                ruColSpan = 0;
            }


            if (IsShowRight == false && (UseColSpan == tableCol)) {
                html += "</tr>";
                isDropTR = true;
                UseColSpan = ruColSpan;

            }

            continue;
        }

        if (isDropTR == false) {
            ruColSpan = 0;
            if (IsShowRight == true) {
                UseColSpan += colSpan + textColSpan;
                if (UseColSpan > tableCol) {
                    //需要换行，补齐缺失的空格
                    var count = tableCol - (UseColSpan - colSpan - textColSpan);
                    for (var k = 0; k < count; k++) {
                        html += "<td class='FoolFrmFieldCtrl'></td>";
                    }

                    html += "</tr>";
                    html += "<tr>";
                    UseColSpan = colSpan + textColSpan + ruColSpan;
                    lRowSpan = rowSpan;
                    luColSpan = colSpan + textColSpan;
                    if (attr.MyDataType == 4) {
                        colSpan = colSpan + textColSpan;
                        colWidth = (parseInt(colSpan) * 23 + 10 * parseInt(textColSpan)) + "%";
                    } else {
                        html += "<td  id='TD_" + attr.KeyOfEn + "'  class='" + labClass + "' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " ColSpan=" + textColSpan + " class='tdSpan'>" + lab + "</td>";
                    }
                    if (attr.IsSigan == "5") 
                        html += "<td  class='FoolFrmFieldCtrl' id='TD_" + attr.KeyOfEn + "'  style='text-align: center;width:" + colWidth + ";' ColSpan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
                    else
                        html += "<td  class='FoolFrmFieldCtrl' id='TD_" + attr.KeyOfEn + "'  style='width:" + colWidth + ";' ColSpan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
                    html += InitMapAttrOfCtrlFool(flowData, attr);
                    html += "</td>";
                    if (rowSpan != 1) {
                        IsShowLeft = false;
                    }
                    if (UseColSpan >= tableCol) {
                        ruRowSpan++;
                        isDropTR = true;
                    } else {
                        isDropTR = false;
                    }

                    //复位右侧信息
                    if (ruRowSpan == rRowSpan) {
                        ruRowSpan = 0;
                        rRowSpan = 0;
                        IsShowRight = true;
                        if (rowSpan == 1)
                            luColSpan = 0;
                        ruColSpan = 0;

                    }


                    if (IsShowRight == false && (UseColSpan == tableCol)) {
                        html += "</tr>";
                        isDropTR = true;
                        UseColSpan = ruColSpan;

                    }

                    continue;

                } else {
                    rRowSpan = rowSpan;
                    ruColSpan += colSpan + textColSpan;
                    if (attr.MyDataType == 4) {
                        colSpan = colSpan + textColSpan;
                        width = 35 * parseInt(colSpan) + "%";
                    } else {
                        html += "<td  id='TD_" + attr.KeyOfEn + "'  class='" + labClass + "' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " ColSpan=" + textColSpan + " class='tdSpan'>" + lab + "</td>";
                    }
                    if (attr.IsSigan == "5") 
                        html += "<td  class='FoolFrmFieldCtrl' id='TD_" + attr.KeyOfEn + "'  style='text-align: center;width:" + colWidth + ";' ColSpan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
                    else
                        html += "<td  class='FoolFrmFieldCtrl' id='TD_" + attr.KeyOfEn + "'  style='width:" + colWidth + ";' ColSpan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
                    html += InitMapAttrOfCtrlFool(flowData, attr);
                    html += "</td>";
                    if (UseColSpan == tableCol) {
                        isDropTR = true;
                        if (rowSpan != 1) {
                            ruRowSpan++;
                        }
                    }
                    if (rowSpan != 1) {
                        IsShowRight = false;
                        lRowSpan = rowSpan;
                    }
                }

            }

            if (UseColSpan == tableCol) {
                luRowSpan++;
                html += "</tr>";
            }

            //复位左侧信息
            if (luRowSpan == lRowSpan) {
                luRowSpan = 0;
                ruRowSpan = 0;
                lRowSpan = 0;
                IsShowLeft = true;
                ruColSpan = 0;

            }

            if (IsShowLeft == false && (UseColSpan == tableCol)) {
                html += "<tr class='FoolFrmFieldTR' >";
                UseColSpan = 0;
                isDropTR = false;
                UseColSpan = luColSpan;
            }
            continue;
        }

    }

    if (isDropTR == false) {
        var unUseColSpan = tableCol - UseColSpan;
        html += "<td colspan=" + unUseColSpan + "></td>";
        html += "</tr>";
    }
    return html;
}

function Gener_Btn(flowData, mapAttr) {

    var eleHtml = $('<div></div>');
    var btnId = mapAttr.KeyOfEn;
    if (btnId == null || btnId == "")
        btnId = mapAttr.MyPK;

    var doc = mapAttr.Tag;
    doc = doc.replace("~", "'");
    var eventType = mapAttr.UIIsEnable;
    var onclick = "";
    var style = ""
    if (eventType == 0) { //禁用
        onclick = "disabled='disabled' style='background:gray;'";
    }

    if (eventType == 1) { //运行URL

        var attrs = flowData.Sys_MapAttr;

        for (var i = 0; i < attrs.length; i++) {
            var attr = attrs[i];

            if (doc.indexOf('@' + attr.KeyOfEn) > 0) {
                doc = doc.replace('@' + attr.KeyOfEn, frmData.MainTable[0][obj.KeyOfEn]);
            }
        }

        var OID = GetQueryString("OID");
        if (OID == undefined || OID == "");
        OID = GetQueryString("OID");
        var FK_Node = GetQueryString("FK_Node");
        var FK_Flow = GetQueryString("FK_Flow");
        var webUser = new WebUser();
        var userNo = webUser.No;
        var SID = webUser.SID;
        if (SID == undefined)
            SID = "";
        if (doc.indexOf("?") == -1)
            doc = doc + "?1=1";
        doc = doc + "&OID=" + pageData.WorkID + "&FK_Node=" + FK_Node + "&FK_Flow=" + FK_Flow + "&UserNo=" + userNo + "&SID=" + SID;
        onclick = "onclick='window.open(\"" + doc + ")'";
    }

    ////运行URL
    if (eventType == 2) {
        if (doc.indexOf("(") == -1)
            doc = doc + "()";
        onclick = "onclick='" + doc + "'";
    }
    return "<input id='" + btnId + "' type='button' value='" + mapAttr.Name + "'  class='btn' " + onclick + ">";
}


function Gener_Link(flowData, mapAttr) {

    var eleHtml = $('<div></div>');
    var btnId = mapAttr.KeyOfEn;
    if (btnId == null || btnId == "")
        btnId = mapAttr.MyPK;

    var doc = mapAttr.Tag;
    doc = doc.replace("~", "'");
    var eventType = mapAttr.UIIsEnable;
    var onclick = "";
    var style = ""
    if (eventType == 0) { //禁用
        onclick = "disabled='disabled' style='background:gray;'";
    }

    if (eventType == 1) { //运行URL

        var attrs = flowData.Sys_MapAttr;

        for (var i = 0; i < attrs.length; i++) {
            var attr = attrs[i];

            if (doc.indexOf('@' + attr.KeyOfEn) > 0) {
                doc = doc.replace('@' + attr.KeyOfEn, frmData.MainTable[0][obj.KeyOfEn]);
            }
        }

        var OID = GetQueryString("OID");
        if (OID == undefined || OID == "");
        OID = GetQueryString("OID");
        var FK_Node = GetQueryString("FK_Node");
        var FK_Flow = GetQueryString("FK_Flow");
        var webUser = new WebUser();
        var userNo = webUser.No;
        var SID = webUser.SID;
        if (SID == undefined)
            SID = "";
        if (doc.indexOf("?") == -1)
            doc = doc + "?1=1";
        doc = doc + "&OID=" + pageData.WorkID + "&FK_Node=" + FK_Node + "&FK_Flow=" + FK_Flow + "&UserNo=" + userNo + "&SID=" + SID;
        onclick = "onclick='window.open(\"" + doc + ")'";
    }

    ////运行URL
    if (eventType == 2) {
        if (doc.indexOf("(") == -1)
            doc = doc + "()";
        onclick = "onclick='" + doc + "'";
    }
    return "<input id='" + btnId + "' type='button' value='" + mapAttr.Name + "'  class='btn' " + onclick + ">";
}


function InitMapAttrOfCtrlFool(flowData, mapAttr) {

    //如果是按钮.
    if (mapAttr.UIContralType == 18)
        return Gener_Btn(flowData, mapAttr);

    //如果是连接.
    if (mapAttr.UIContralType == 9)
        return Gener_Link(flowData, mapAttr);

    var str = '';
    var defValue = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);

    var isInOneRow = false; //是否占一整行
    var islabelIsInEle = false; //
    var eleHtml = '';

    //外部数据源类型.
    if (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1) {

        //判断外键是否为树形结构
        if (mapAttr.AtPara != null && mapAttr.AtPara.indexOf("@CodeStruct=1") != -1)
            return "<select  id='DDL_" + mapAttr.KeyOfEn + "' class='easyui-combotree' style='height:28px;width:60%'></select>";

        if (mapAttr.UIIsEnable == 0) {
            var ctrl = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' type=hidden  class='form-control' type='text'/>";

            defValue = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn + "T");

            if (defValue == '' || defValue == null)
                defValue = '无';

            ctrl += "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "T'  value='" + defValue + "' disabled='disabled'   class='form-control' type='text' style='width:100%'/>";
            return ctrl;
        }

        return "<select id='DDL_" + mapAttr.KeyOfEn + "' class='form-control'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(flowData, mapAttr, defValue) + "</select>";
    }

    //外键类型.
    if (mapAttr.LGType == "2" && mapAttr.MyDataType == "1") {

        var data = flowData[mapAttr.UIBindKey];
        //是否可编辑
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        //判断外键是否为树形结构
        if (mapAttr.AtPara.indexOf("@CodeStruct=1") != -1)
            return "<select  id='DDL_" + mapAttr.KeyOfEn + "' class='easyui-combotree' style='height:28px;width:60%'></select>";

        return "<select id='DDL_" + mapAttr.KeyOfEn + "' class='form-control'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(flowData, mapAttr, defValue) + "</select>";
    }

    if (mapAttr.MyDataType == "1" && mapAttr.UIContralType == "2") {//枚举复选框

        var rbHtmls = "";
        var ses = flowData[mapAttr.KeyOfEn];
        if (ses == undefined)
            ses = flowData[mapAttr.UIBindKey];
        if (ses == undefined) {
            //枚举类型的.
            if (mapAttr.LGType == 1) {
                ses = flowData.Sys_Enum;
                ses = $.grep(ses, function (value) {
                    return value.EnumKey == mapAttr.UIBindKey;
                });
            }

        }
        var enableAttr = "";
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        //显示方式,默认为横向展示.
        var RBShowModel = 0;
        if (mapAttr.AtPara.indexOf('@RBShowModel=0') > 0)
            RBShowModel = 1;

        for (var i = 0; i < ses.length; i++) {
            var se = ses[i];

            var br = "";
            if (RBShowModel == 1)
                br = "<br>";

            var checked = "";
            if ("," + defValue + ",".indexOf("," + se.IntKey + ",") == true)
                checked = " checked=true";

            rbHtmls += "<label style='font-weight:normal;'><input type=checkbox name='CB_" + mapAttr.KeyOfEn + "' id='CB_" + mapAttr.KeyOfEn + "_" + se.IntKey + "' value='" + se.IntKey + "' " + checked + enableAttr + " onclick='clickEnable( this ,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")' />" + se.Lab + " </label>&nbsp;" + br;
        }
        return rbHtmls;
    }

    //添加文本框 ，日期控件等.
    //AppString
    if (mapAttr.MyDataType == "1") {  //不是外键

        //附件.
        if (mapAttr.UIContralType == 6) {

            //获取上传附件列表的信息及权限信息
            var nodeID = pageData.FK_Node;
            var no = nodeID.toString().substring(nodeID.toString().length - 2);
            var IsStartNode = 0;
            if (no == "01")
                IsStartNode = 1;

            //创建附件描述信息. 
            var mypk = mapAttr.MyPK;

            //获取附件显示的格式
            var athShowModel = GetPara(mapAttr.AtPara, "AthShowModel");

            var ath = new Entity("BP.Sys.FrmAttachment");
            ath.MyPK = mypk;
            if (ath.RetrieveFromDBSources() == 0) {
                alert("没有找到附件属性,请联系管理员");
                return;
            }
            var noOfObj = mypk.replace(mapAttr.FK_MapData + "_", "");
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
            handler.AddPara("WorkID", pageData.WorkID);
            handler.AddPara("FID", pageData.FID);
            handler.AddPara("FK_Node", nodeID);
            handler.AddPara("FK_Flow", pageData.FK_Flow);
            handler.AddPara("IsStartNode", IsStartNode);
            handler.AddPara("PKVal", pageData.WorkID);
            handler.AddPara("Ath", noOfObj);
            handler.AddPara("FK_MapData", mapAttr.FK_MapData);
            handler.AddPara("FromFrm", mapAttr.FK_MapData);
            handler.AddPara("FK_FrmAttachment", mypk);
            data = handler.DoMethodReturnString("Ath_Init");

            if (data.indexOf('err@') == 0) {
                alert(data);
                return;
            }

            if (data.indexOf('url@') == 0) {
                var url = data.replace('url@', '');
                window.location.href = url;
                return;
            }
            data = JSON.parse(data);
            var dbs = data["DBAths"];
            var athDesc = data["AthDesc"][0];
            if (dbs.length == 0) {
                if (athDesc.IsUpload == 1 || pageData.IsReadOnly == 0)
                    return "<div style='text-align:left;padding-left:10px' id='athModel_" + mapAttr.KeyOfEn + "'><label>请点击[" + mapAttr.Name + "]执行上传</label></div>";
                else
                    return "<div style='text-align:left;padding-left:10px' id='athModel_" + mapAttr.KeyOfEn + "' class='athModel'><label>附件(0)</label></div>";
            }
            var eleHtml = "";
            if (athShowModel == "" || athShowModel == 0)
                return "<div style='text-align:left;padding-left:10px' id='athModel_" + mapAttr.KeyOfEn + "' data-type='0'><label >附件(" + dbs.length + ")</label></div>";

            eleHtml = "<div style='text-align:left;padding-left:10px' id='athModel_" + mapAttr.KeyOfEn + "' data-type='1'>";

            var workID = GetQueryString("WorkID");
            for (var i = 0; i < dbs.length; i++) {
                var db = dbs[i];
                eleHtml += "<label><a style='font-weight:normal;font-size:12px'  href=\"javascript:Down2018('" + db.MyPK + "','" + workID + "')\"><img src='./Img/FileType/" + db.FileExts + ".gif' />" + db.FileName + "</a></label>&nbsp;&nbsp;&nbsp;"
            }
            eleHtml += "</div>";
            return eleHtml;
        }
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
            eleHtml += "<img src='" + val + "' " + ondblclick + " onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\"  style='border:0px;height:" + mapAttr.UIHeight + "px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
            return eleHtml;
        }

        //评分控件
        if (mapAttr.UIContralType == "101") {
            //查找默认值
            var val = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
            if (val == "" || val == undefined || val == null) {
                val = 0;
            }
            //如果编辑
            var eleHtml = "<div class='score-star' style='text-align:left;padding-left:3px;height:30px;margin-top:10px' data-type='1' id='SC_" + mapAttr.KeyOfEn + "' >";
            if (mapAttr.UIIsEnable == 1) {
                eleHtml += "<span class='score-simplestar' id='Star_" + mapAttr.KeyOfEn + "'>";
            } else {
                eleHtml += "<span class='score-simplestar'>";
            }
            var num = mapAttr.Tag2;
            for (var i = 0; i < val; i++) {

                eleHtml += "<img src='Style/Img/star_2.png' />";
            }
            for (var j = 0; j < num - val; j++) {

                eleHtml += "<img src='Style/Img/star_1.png' />";
            }
            eleHtml += "&nbsp;&nbsp;<span class='score-tips' id='SP_" + mapAttr.KeyOfEn + "' style='vertical-align: middle;color:#ff6600;font: 12px/1.5 tahoma,arial,\"Hiragino Sans GB\",宋体,sans-serif;'><strong>" + val + "  分</strong></span>";
            eleHtml += "<input id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' type=hidden /></span>";
            eleHtml += "</div>";
            return eleHtml;
        }
        //  地图
        if (mapAttr.UIContralType == "4") {
            //查找默认值
            var val = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
            //如果是地图，并且可以编辑
            var eleHtml = "<div style='text-align:left;padding-left:0px' id='athModel_" + mapAttr.KeyOfEn + "' data-type='1'>";
            if (mapAttr.UIIsEnable == 1) {
                eleHtml += "<input type='button' name='select' value='选择' onclick='figure_Template_Map(\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.UIIsEnable + "\")'/>";
                eleHtml += "<input type = text style='width:75%' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' />";
            } else {
                eleHtml += "<input type='button' name='select' value='选择' onclick='figure_Template_Map(\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.UIIsEnable + "\")'/>";
                eleHtml += "<input type = text style='width:75%' readonly='readonly' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' />";
            }
            eleHtml += "</div>";
            return eleHtml;
        }

        //身份证
        if (mapAttr.UIContralType == 13 && mapAttr.KeyOfEn == "IDCardAddress") {
            var eleHtml = "<div style='text-align:left;padding-left:0px'  data-type='1'>";
            eleHtml += "<input type = text style='width:75% !important;display:inline;' class='form-control' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'/>";
            eleHtml += "<label class='image-local' style='margin-left:5px'><input type='file' accept='image/png,image/bmp,image/jpg,image/jpeg' style='width:25% !important;display:none' onchange='GetIDCardInfo(event)'/>上传身份证</label>";
            eleHtml += "</div>";
            return eleHtml;
        }

        //进度条
        if (mapAttr.UIContralType == "50") {
            return "<div id='JobSchedule' >JobSchedule</div>";
        }


        if (mapAttr.UIHeight <= 40) //普通的文本框.
        {
            if (mapAttr.IsSigan == "1") {
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' type=hidden />";
                var val = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
                if (webUser.CCBPMRunModel == 2)
                    val = webUser.OrgNo + "/";
                return "<img alt='" + val + "' src='../DataUser/Siganture/" + val + UserIConExt + "'  style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
            }
            if (mapAttr.IsSigan == "5") {
                //查找默认值
                var val = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
                //如果是图片签名，并且可以编辑
                var ondblclick = ""
                if (mapAttr.UIIsEnable == 1) {
                    ondblclick = " ondblclick='figure_Template_HandWrite(\"" + mapAttr.KeyOfEn + "\",\"" + val + "\")'";
                }

                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' type=hidden />";
                eleHtml += "<img src='" + val + "' " + ondblclick + " onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\"  style='border:0px;height:50px;width:50%' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                return eleHtml;
            }
            if (mapAttr.IsSecret)
                return "<div  id='TDIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'><input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "'  class='form-control' type='password' placeholder='" + (mapAttr.Tip || '') + "' style='width:100%'/></div>";
            else
                return "<div  id='TDIV_" + mapAttr.KeyOfEn + "'  class='ccbpm-input-group'><input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "'  class='form-control' type='text' placeholder='" + (mapAttr.Tip || '') + "'  style='width:100%'/></div>";
        }

        if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {

            var eleHtml = "";
            //如果是富文本就使用百度 UEditor
            if (mapAttr.UIIsEnable == "0" || pageData.IsReadonly == "1") {
                //只读状态直接 div 展示富文本内容
                defValue = defValue.replace(/white-space: nowrap;/g, "");
                eleHtml += "<div class='richText' style='width:99%;margin-right:2px'>" + defValue + "</div>";

            } else {
                //设置一个默认高度
                if (mapAttr.UIHeight < 180) {
                    mapAttr.UIHeight = 180;
                }

                document.BindEditorMapAttr.push(mapAttr); //存到全局备用

                //设置编辑器的默认样式
                var styleText = "text-align:left;font-size:12px;";
                styleText += "width:100%;";
                var height = parseInt(mapAttr.UIHeight) - 54;

                styleText += "height:" + height + "px;";
                //注意这里 name 属性是可以用来绑定表单提交时的字段名字的 id 是特殊约定的.
                eleHtml += "<script class='EditorClass' id='editor_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";

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
        var frmDate = mapAttr.IsSupperText;//获取日期格式
        var dateFmt = '';
        if (frmDate == 0) {
            dateFmt = "yyyy-MM-dd";
        } else if (frmDate == 3) {
            dateFmt = "yyyy-MM";
        } else if (frmDate == 6) {
            dateFmt = "MM-dd";
        }

        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'" + dateFmt + "'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return " <input type='text' " + enableAttr + " value='" + defValue + "'  class='form-control Wdate' id='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "' />";
    }

    //时期时间类型.
    if (mapAttr.MyDataType == 7) {

        var enableAttr = '';
        var frmDate = mapAttr.IsSupperText; //获取日期格式
        var dateFmt = '';
        if (frmDate == 1) {
            dateFmt = "yyyy-MM-dd HH:mm";
        } else if (frmDate == 2) {
            dateFmt = "yyyy-MM-dd HH:mm:ss";
        } else if (frmDate == 4) {
            dateFmt = "HH:mm";
        } else if (frmDate == 5) {
            dateFmt = "HH:mm:ss";
        }
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'" + dateFmt + "'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return " <input  type='text'  value='" + defValue + "'  class='form-control Wdate' " + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
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

        var tip = "";
        if (mapAttr.Tip != "" && mapAttr.Tip != null)
            tip = "<span style='color: #C0C0C0;'>(" + mapAttr.Tip + ")</span>";

        return "<label ><input " + enableAttr + " " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' id='CB_" + mapAttr.KeyOfEn + "'  name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + " onchange='changeCBEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'/> &nbsp;" + mapAttr.Name + tip + "</label>";
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

        var type = " type='text'";
        if (mapAttr.IsSecret)
            type = " type='password'";
        return "<input onfocus='removeplaceholder(this," + bit + ");' onblur='addplaceholder(this," + bit + ");' value='" + defValue + "' style='text-align:right;'class='form-control'  onkeyup=" + '"' + "valitationAfter(this, 'float');if(isNaN(value)) execCommand('undo');limitLength(this," + bit + ");" + '"' + " onafterpaste=" + '"' + " valitationAfter(this, 'float');if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + type + "  id='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
    }

    //AppInt 整数类型.
    if ( mapAttr.MyDataType == 2 ) { 
        var enableAttr = '';
        if (mapAttr.UIIsEnable != 1) {
            enableAttr = "disabled='disabled'";
        }

        var type = " type='text'";
        if (mapAttr.IsSecret)
            type = " type='password'";

        return "<input  onfocus='removeplaceholder(this,0);' onblur='addplaceholder(this,0);' value='" + defValue + "' style='text-align:right;' class='form-control' onkeyup=" + '"' + "limitLength(this," + bit + ");valitationAfter(this, 'int');if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'int');if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + type + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "'placeholder='" + (mapAttr.Tip || '') + "'/>";
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

        var type = " type='text'";
        if (mapAttr.IsSecret)
            type = " type='password'";

        return "<input value='" + defValue + "' style='text-align:right;' class='form-control' onfocus='removeplaceholder(this," + bit + ");' onblur='addplaceholder(this," + bit + ");numberFormat (this, " + bit + ") ' onkeyup=" + '"' +
            "limitLength(this," + bit + ");" + '"' +
            " onafterpaste=" + '"' + "valitationAfter(this, 'money');if(isNaN(value))execCommand('undo');" + '"' +
            " maxlength=" + mapAttr.MaxLen / 2 + type + "  id='TB_" + mapAttr.KeyOfEn + "' value='0.00' placeholder='" + (mapAttr.Tip || '') + "'/>";

        //return "<input value='" + defValue + "' style='text-align:right;' class='form-control' onkeyup=" + '"' + "valitationAfter(this, 'money');limitLength(this," + bit + "); FormatMoney(this, " + bit + ", ',')" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'money');if(isNaN(value))execCommand('undo');" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' value='0.00' placeholder='" + (mapAttr.Tip || '') + "'/>";
    }

    alert(mapAttr.Name + "的类型没有判断.");
    return;
}


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

    if (wf_node.FWCSta != 0) {
        if (wf_node.FWCVer == 0 || wf_node.FWCVer == "" || wf_node.FWCVer == undefined)
            pageData.FWCVer = 0;
        else
            pageData.FWCVer = 1;

    }

    return "<div id='WorkCheck'></div>";

}

//子流程
function Ele_SubFlow(wf_node) {
    var sta = wf_node.SFSta;
    var h = wf_node.SF_H + 100;

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
    if (sta == 2 || pageData.IsReadonly == 1)//只读
    {
        src += "&DoType=View";
    }
    src += "&r=q" + paras;
    if (h == 0)
        h = 400;
    var eleHtml = "<iframe id=FSF" + wf_node.NodeID + " style='width:100%;height:" + h + "px'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=no></iframe>";

    return eleHtml;
}


//初始化 框架
function Ele_Frame(flowData, gf) {
    var frame = new Entity("BP.Sys.MapFrame", gf.CtrlID);

    if (frame == null)
        return "没有找到框架的定义，请与管理员联系。";

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
        var gwf = flowData.WF_GenerWorkFlow[0];
        if (gwf != null) {
            var atPara = gwf.AtPara;
            if (atPara != null && atPara != "") {
                atPara = atPara.replace(/@/g, '&');
                url = url + atPara;
            }
        }
    }

    if (urlType == 2) //轨迹表
        url = "./WorkOpt/OneWork/Table.htm?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.WorkID + "&FID=" + pageData.FID;
    if (urlType == 3)//轨迹图
        url = "./WorkOpt/OneWork/TimeBase.htm?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.WorkID + "&FID=" + pageData.FID;

    eleHtml += "<iframe style='width:100%;height:" + frame.H + "px;' ID='" + frame.MyPK + "'    src='" + url + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    return eleHtml;
}


////初始化 附件
//function Ele_Attachment(flowData, gf, node, ath) {

//    var eleHtml = '';
//    var nodeID = GetQueryString("FK_Node");
//    var url = "";
//    url += "&WorkID=" + GetQueryString("WorkID");
//    url += "&FK_Node=" + nodeID;
//    url += "&FK_Flow=" + node.FK_Flow;
//    url += "&FormType=" + node.FormType; //表单类型，累加表单，傻瓜表单，自由表单.
//    var no = node.NodeID.toString().substring(node.NodeID.toString().length - 2);
//    var IsStartNode = 0;
//    if (no == "01")
//        url += "&IsStartNode=" + 1; //是否是开始节点

//    var isReadonly = false;
//    //if (gf.FrmID.indexOf(nodeID) == -1)
//    //    isReadonly = true;

//    if (isReadonly == false) {
//        var strRD = GetQueryString("IsReadonly");
//        if (strRD == 1)
//            isReadonly = true;
//    }

//    var athPK = gf.CtrlID;
//    var noOfObj = athPK.replace(gf.FrmID + "_", "");

//    var src = "";

//    //这里的连接要取 FK_MapData的值.
//    src = "./CCForm/Ath.htm?PKVal=" + pageData.WorkID + "&PWorkID=" + GetQueryString("PWorkID") + "&FID=" + pageData["FID"] + "&Ath=" + noOfObj + "&FK_MapData=ND" + node.NodeID + "&FromFrm=" + gf.FrmID + "&FK_FrmAttachment=" + athPK + url + "&M=" + Math.random();
//    if (isReadonly == true)
//        src += "&IsReadOnly=1";

//    //自定义表单模式.
//    if (ath.AthRunModel == 2) {
//        src = "../DataUser/OverrideFiles/Ath.htm?PKVal=" + pageData.WorkID + "&PWorkID=" + GetQueryString("PWorkID") + "&FID=" + pageData["FID"] + "&Ath=" + noOfObj + "&FK_MapData=" + gf.FrmID + "&FK_FrmAttachment=" + athPK + url + "&M=" + Math.random();
//    }

//    eleHtml += "<iframe style='width:100%;height:" + ath.H + "px;' id='Ath1' name='Ath1'  src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
//    return eleHtml;
//}

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
            src = "./CCForm/" + dtlUrl + ".htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=1&" + urlParam + "&Version=1&FrmType=0";
        } else {
            src = "./CCForm/" + dtlUrl + ".htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=0&" + urlParam + "&Version=1&FrmType=0";
        }
    }
    else if (frmDtl.ListShowModel == "1") {
        //卡片模式
        if (pageData.IsReadonly) {
            src = "./CCForm/DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=1&" + urlParam + "&Version=1&FrmType=0";
        } else {
            src = "./CCForm/DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=0&" + urlParam + "&Version=1&FrmType=0";
        }
    }
    return "<iframe style='width:100%;height:100%' name='Dtl' ID='IFrame_" + frmDtl.No + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
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
            rbHtml += "<label><input " + enableAttr + " " + (obj.IntKey == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "' " + onclickEvent + " />&nbsp;" + obj.Lab + "</label>";
        else
            rbHtml += "<label><input " + enableAttr + " " + (obj.IntKey == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "' " + onclickEvent + "/>&nbsp;" + obj.Lab + "</label><br/>";
    });
    return rbHtml;
}


//解析傻瓜表单的字段lab
function GetLab(flowData, attr) {
    var lab = "";
    var forID = "TB_" + attr.KeyOfEn;
    var contralType = attr.UIContralType;
    if (contralType == 1) {//外键下拉框
        forID = "DDL_" + attr.KeyOfEn;
    }
    if (contralType == 3) {//枚举
        forID = "RB_" + attr.KeyOfEn;
    }
    if (contralType == 2) {//枚举复选框
        forID = "CB_" + attr.KeyOfEn;
    }
    var ccsLab = "";
    if (attr.CSSLabel != "")
        ccsLab = " class='" + attr.CSSLabel + "'";

    //文本框，下拉框，单选按钮
    if (contralType == 0 || contralType == 1 || contralType == 2 || contralType == 3 || contralType == 4 || contralType == 13 || contralType == 8 || contralType == 50 || contralType == 101) {
        if (attr.UIIsInput == 1 && attr.UIIsEnable == 1) {
            lab = " <span style='color:red' class='mustInput' data-keyofen='" + attr.KeyOfEn + "' >*</span>";
        }
        lab += "<label " + ccsLab+" id='Lab_" + attr.KeyOfEn + "' for='" + forID + "' class='" + (attr.UIIsInput == 1 ? "mustInput" : "") + "' >" + attr.Name + "</label>";

        return lab;
    }
    //附件控件
    if (contralType == 6) {
        var mypk = attr.MyPK;
        //创建附件描述信息.
        var ath = $.grep(flowData.Sys_FrmAttachment, function (obj, idx) {
            if (obj.MyPK == mypk)
                return obj;
        });
        if (ath.length == 0) {
            alert("没有找到附件属性,请联系管理员");
            return;
        }

        //附件的url
        var eleHtml = '';
        var nodeID = GetQueryString("FK_Node");
        var url = "";
        url += "&WorkID=" + pageData.WorkID;
        url += "&FK_Node=" + nodeID;
        url += "&FK_Flow=" + pageData.FK_Flow;
        var no = nodeID.toString().substring(nodeID.toString().length - 2);
        var IsStartNode = 0;
        if (no == "01")
            url += "&IsStartNode=" + 1; //是否是开始节点

        var isReadonly = pageData.IsReadonly;

        if (isReadonly == false) {
            var strRD = GetQueryString("IsReadonly");
            if (strRD == 1)
                isReadonly = true;
        }

        var noOfObj = mypk.replace(attr.FK_MapData + "_", "");
        var src = "";

        //这里的连接要取 FK_MapData的值.
        src = "./CCForm/Ath.js?PKVal=" + pageData.WorkID + "&FID=" + pageData["FID"] + "&Ath=" + noOfObj + "&FK_MapData=" + attr.FK_MapData + "&FromFrm=" + attr.FK_MapData + "&FK_FrmAttachment=" + mypk + url + "&M=" + Math.random();
        if (isReadonly == true)
            src += "&IsReadOnly=1";

        //自定义表单模式.
        if (ath[0].AthRunModel == 2) {
            src = "../DataUser/OverrideFiles/Ath.htm?PKVal=" + pageData.WorkID + "&FID=" + pageData["FID"] + "&Ath=" + noOfObj + "&FK_MapData=" + attr.FK_MapData + "&FK_FrmAttachment=" + mypk + url + "&M=" + Math.random();
        }
        lab = "<label " + ccsLab +" id='Lab_" + attr.KeyOfEn + "' for='athModel_" + attr.KeyOfEn + "'><div style='text-align:left'><a href='javaScript:void(0)' onclick='OpenAth(\"" + attr.Name + "\",\"" + attr.KeyOfEn + "\",\"" + attr.MyPK + "\",\"" + attr.AtPara + "\",\"" + attr.FK_MapData + "\",0)' style='text-align:left'>" + attr.Name + "<image src='./Img/Tree/Dir.gif'></image></a></div></label>";
        return lab;
    }

    //超链接
    if (contralType == 9) {
        //URL @ 变量替换
        var url = attr.Tag2;

        //替换URL中的参数
        var pageParams = getQueryString();
        $.each(pageParams, function (i, pageParam) {
            var pageParamArr = pageParam.split('=');
            url = url.replace("@" + pageParamArr[0], pageParamArr[1]);
        });

        //替换表单中的参数
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

        if (url.indexOf("Search.htm") != -1)
            url = url + "&FK_Node=" + FK_Node + "&FK_Flow=" + FK_Flow + "&UserNo=" + userNo + "&SID=" + SID;
        else
            url = url + "&UserNo=" + userNo + "&SID=" + SID;

        eleHtml = '<span ><a href="' + url + '" target="_blank">' + attr.Name + '</a></span>';

        return eleHtml;

    }


    //图片
    if (contralType == 11) {

        //获取图片控件的信息
        var frmImg = new Entity("BP.Sys.FrmUI.ExtImg");
        frmImg.SetPKVal(attr.MyPK);
        var count = frmImg.RetrieveFromDBSources();
        if (count == 0) {
            alert("主键为" + attr.MyPK + "名称为" + attr.Name + "的图片控件信息丢失，请联系管理员");
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
                imgSrc = DealJsonExp(flowData.MainTable[0], frmPath);
            }

            //数据来源为指定路径.
            if (frmImg.ImgSrcType == 1) {
                var url = frmImg.ImgURL;
                url = url.replace('＠', '@');
                url = url.replace('@basePath', basePath);
                imgSrc = DealJsonExp(flowData.MainTable[0], url);
            }
            // 由于火狐 不支持onerror 所以 判断图片是否存在放到服务器端
            if (imgSrc == "" || imgSrc == null)
                imgSrc = "../DataUser/ICON/CCFlow/LogBig.png";

            //＠basePath
            //alert(imgSrc);

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
        return "";

    }
    //图片附件
    if (contralType == 12) {
        //获取图片控件的信息
        var frmImgs = $.grep(flowData.Sys_FrmImgAth, function (item, i) {
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
            var url = basePath + "/WF/CCForm/ImgAth.htm?W=" + frmImg.W + "&H=" + frmImg.H + "&FK_MapData=" + flowData.Sys_MapData[0].No + "&RefPKVal=" + pageData.WorkID + "&CtrlID=" + frmImg.CtrlID;
            _html += "<div>";
            _html += "<fieldset>";
            _html += "<legend style='margin-bottom:0px'>";
            _html += "<a href='javaScript:void(0)' onclick='ImgAth(\"" + url + "\",\"" + frmImg.MyPK + "\");'>编辑</a>";
            _html += "</legend>";
            _html += "<img class='pimg' id='Img" + frmImg.MyPK + "' name='Img" + frmImg.MyPK + "' src='" + imgSrc + "' onerror=\"this.src='" + basePath + "/DataUser/ICON/CCFlow/LogBig.png'\" style='width:" + frmImg.W + "px;height:" + frmImg.H + "px;'/>";
            _html += "</fieldset>";
            _html += "</div>";
            return _html;
        } else {
            _html += "<div>";
            _html += "<img class='pimg' id='Img" + frmImg.MyPK + "' name='Img" + frmImg.MyPK + "' src='" + imgSrc + "' \"this.src='" + basePath + "/DataUser/ICON/CCFlow/LogBig.png'\" style='width:" + frmImg.W + "px;height:" + frmImg.H + "px;'/>";
            _html += "</div>";
            return _html;
        }
    }

    //按钮
    if (contralType == 18) {
        return "";
    }

    return lab;
}


