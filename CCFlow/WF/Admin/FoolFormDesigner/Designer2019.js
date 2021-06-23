var webUser;
var pageData = {};
var workModel = GetQueryString("WorkModel");
if (workModel == null || workModel == undefined)
    workModel = 0;

//页面启动函数.
$(function () {

    //导入关联表单按钮
    $("#RefDict").hide();

    //装载风格.
    LoadCss();

    //单据属性按钮
    if (workModel == 0 || workModel == 1)
        $("#FrmillBtn").hide();

    if (workModel == 3)
        $("#RefDict").show();


    $("#state").css("left", ($("#Btn_Save").position().left - 150 - 34) + "px");

    $(".wrapper-dropdown-2").on("mousedown", function (e) {
        var v_id = $(e.target).attr("id");
        var dd = new DropDown($("#" + v_id + ""));
    });

    $(".wrapper-dropdown-2").click(function () {
        // all dropdowns
        //$('.wrapper-dropdown-2').removeClass('active');
    });
    $(document).click(function () {
        // all dropdowns
        $('.wrapper-dropdown-2').removeClass('active');
    });


    webUser = new WebUser();
    pageData.fk_mapdata = GetQueryString("FK_MapData");
    pageData.FK_Flow = GetQueryString("FK_Flow");
    pageData.FK_Node = GetQueryString("FK_Node");

    //如果SAAS模式.
    if (webUser.CCBPMRunModel == 2) {
        $("#StyletDfine").hide();
        $("#GloValStyles").hide();
        $("#BatchAddF").hide();
        $("#MobileFrm").hide();
    }

    //pageData.IsReadonly = 1;

    $("#Msg").html("<img src=../../Img/loading.gif />&nbsp;正在加载,请稍后......");

    var nodeID = GetQueryString("FK_Node");
    if (nodeID == null || nodeID == undefined)
        nodeID = 0;
    if (nodeID == 0)
        $("#FrmNodeComponent").hide();

    //初始化groupID.
    var fk_mapData = GetQueryString("FK_MapData");
    var isF = GetQueryString("IsFirst"); //是否第一次加载?

    var hander = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
    hander.Clear();
    hander.AddPara("IsFirst", isF);
    hander.AddPara("FK_MapData", fk_mapData);
    hander.AddPara("FK_Flow", GetQueryString("FK_Flow"));
    hander.AddPara("FK_Node", GetQueryString("FK_Node"));
    var data = hander.DoMethodReturnString("Designer_Init");

    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    if (data.indexOf('url@') == 0) {
        data = data.replace('url@', '');
        window.location.href = data;
        return;
    }

    //里面有三个对象. Sys_MapAttr, Sys_GroupField, Sys_MapData
    data = JSON.parse(data);

    //拼接 TABLE
    //按分组拼接
    var groupFields = data.Sys_GroupField;
    //  var tbody = $('<tbody class=FoolFrmBody ></tbody>');
    //var tbody = $('');

    var sys_MapData = data.Sys_MapData[0];

    var frmName = sys_MapData.Name;
    var tableCol = sys_MapData.TableCol;

    //  GenerBindEnumKey("DDL_TableCol", "TableCol", "No", "Name", sys_MapData.TableCol);

    $("#DDL_TableCol").val(tableCol);


    if (tableCol == 0)
        tableCol = 4;
    else if (tableCol == 1)
        tableCol = 6;
    else if (tableCol == 2)
        tableCol = 3;
    else
        tableCol = 4;

    var html = "<table class='FoolFrmTable' >";

    // 生成标题部分.
    html += "<tr class='FoolFrmTitleTR' >";
    html += " <td   class='FoolFrmTitleTD' colspan=" + tableCol + ">";
    html += "   <div class='FoolFrmTitleIcon' style='float:left;margin-top:1px'  > <img src='../../../DataUser/ICON/LogBiger.png' style='height:50px;' /></div >";
    html += "   <div class='FoolFrmTitleLable' style='float:right;margin-top:8px' >" + frmName + "</div>";
    html += "</td>";
    html += "</tr>";

    //alert(html);

    for (var k = 0; k < groupFields.length; k++) {

        var groupObj = groupFields[k];
        //附件类的控件.
        if (groupObj.CtrlType == 'Ath') {
            //获取附件的主键
            var MyPK = groupObj.CtrlID;
            if (MyPK == "")
                continue;

            //创建附件描述信息.
            var ath = new Entity("BP.Sys.FrmAttachment");
            ath.MyPK = groupObj.CtrlID;
            if (ath.RetrieveFromDBSources() == 0)
                continue;
            if (ath.IsVisable == "0" || ath.NoOfObj == "FrmWorkCheck")
                continue;
        }
        //生成工具栏.
        html += GenerGroupTR(groupObj, tableCol, data);

        //生成内容.
        html += GenerGroupContext(groupObj, data, tableCol);

        //过滤attrs
        var mapAttrs = $.grep(data.Sys_MapAttr, function (val) { return val.GroupID == groupObj.OID; });
        if (tableCol == 4 || tableCol == 6)
            html += InitMapAttr(mapAttrs, tableCol);

        if (tableCol == 3)
            html += InitThreeColMapAttr(mapAttrs, tableCol);
        continue;
    }

    html += "</table>";
    // alert(html);
    // tbody.append($(html) );

    $(".NewChild").hide();
    //contentTable
    $('#contentTable').children().remove();
    // $('#contentTable').append($(html));
    $('#contentTable').html(html);

    if ($("#WorkCheck").length == 1)
        loadScript("../../WorkOpt/WorkCheck.js");

    if ($("#FlowBBS").length == 1)
        loadScript("../../WorkOpt/FlowBBS.js");

    if (data.Sys_FrmAttachment.length != 0) {
        Skip.addJs("../../CCForm/Ath.js");
        Skip.addJs("../../CCForm/JS/FileUpload/fileUpload.js");
        $('head').append("<link href='../../CCForm/JS/FileUpload/css/fileUpload.css' rel='stylesheet' type='text/css' />");
    }

    $.each(data.Sys_FrmAttachment, function (idex, ath) {
        AthTable_Init(ath, "Div_" + ath.MyPK);
    });


    var mapAttrs = data.Sys_MapAttr;
    //解析设置表单字段联动显示与隐藏.
    for (var i = 0; i < mapAttrs.length; i++) {

        var mapAttr = mapAttrs[i];
        if (mapAttr.UIVisible == 0)
            continue;

        // 增加icon .
        AddICON(mapAttr);

        if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {  // AppInt Enum
            if (mapAttr.AtPara != null && mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
                if (mapAttr.UIContralType == 1) {
                    /*启用了显示与隐藏.*/
                    var ddl = $("#DDL_" + mapAttr.KeyOfEn);
                    //初始化页面的值
                    var nowKey = ddl.val();
                    if (nowKey == undefined || nowKey == "")
                        continue;

                    setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey);

                }
                if (mapAttr.UIContralType == 3) {
                    /*启用了显示与隐藏.*/
                    var nowKey = $('input[name="RB_' + mapAttr.KeyOfEn + '"]:checked').val();
                    if (nowKey == undefined || nowKey == "")
                        continue;
                    setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey);
                }
            }
        }
    }

    $("#Msg").html("");
    ResizeWindow();


    //设置表单选择的值.
    $("DDL_TableCol").selectedIndex = sys_MapData.TableCol;
});

function AddICON(attr) {

    var icon = "icon-calendar";

    //alert(attr.MyDataType);

    var icon = attr.ICON;
    if (attr.MyDataType == 6 || attr.MyDataType == 7)
        icon = "icon-calendar";
    if (icon == ""|| icon=="0") {
        $('#DIV_' + attr.KeyOfEn).removeClass("ccbpm-input-group");
        return;
    }

    if (icon) {
        var html = "";
        // html += '<div class="input-group-Designer">';
        html += ' <i class="' + icon + '"></i>';

        $('#DIV_' + attr.KeyOfEn).prepend(html);
    }
    // $('#TB_' + attr.KeyOfEn).after('</div>');
    return;


    //  if (attr.MyDataType == 7 || attr.MyDataType == 8) {

    //日期类型的.
    SetICON("TB_" + attr.KeyOfEn, "icon-calendar");
    return;

    //    alert(attr.KeyOfEn);
    //   return;
    // }

    // alert(attr.KeyOfEn);
    // alert(attr);

    // SetICON("TB_" + attr.KeyOfEn, "icon-calendar");

    //如果是 textBox. 
    if (attr.UIContralType == 0 &&
        (attr.MyDataType == 7 || attr.MyDataType == 8)) {

        //日期类型的.
        SetICON("TB_" + attr.KeyOfEn, "icon-calendar");

        alert(attr.KeyOfEn);

    }
    return;

    //如果是 textBox. 
    if (attr.UIContralType == 0) {

    }

    SetICON("TB_Tel", "icon-phone");
    SetICON("TB_Email", "icon-envelope-letter");
    SetICON("TB_Addr", "icon-location-pin");

    SetICON("TB_SQR", "icon-user");
    SetICON("TB_SQRQ", "icon-calendar");
}

function SetICON(id, icon) {



}

function DDL_TableCol_Change() {

    var col = $("#DDL_TableCol").val();

    var frmID = GetQueryString("FK_MapData");

    var en = new Entity("BP.WF.Template.MapFrmFool", frmID);
    en.TableCol = col;
    en.Update();

    window.location.href = window.location.href;
}

function GenerGroupTR(groupEn, tabCol, data) {

    var lab = groupEn.Lab;
    if (lab == "")
        lab = "编辑";

    //左边的按钮.
    var leftBtn = "<div style='text-align:left; float:left'>";

    var midBtn = "";
    var isShow = true;
    var tdId = "";
    switch (groupEn.CtrlType) {
        case "Ath":
            if (groupEn.CtrlID == "") {
                isShow = false;
                break;
            }
            //创建附件描述信息.
            var frmattachments = data.Sys_FrmAttachment;
            var ath = null;
            for (var i = 0; i < frmattachments.length; i++) {
                if (frmattachments[i].MyPK == groupEn.CtrlID) {
                    ath = frmattachments[i];
                    break;
                }

            }
            if (ath == null) {
                isShow = false;
                break;
            }
            if (ath.IsVisable == "0") {
                //如果是节点字段附件的时候就隐藏
                var mapAttr = $.grep(data.Sys_MapAttr, function (obj, i) {
                    if (obj.MyPK == groupEn.CtrlID)
                        return obj;
                });
                if (mapAttr.length != 0) {
                    isShow = false;
                    break;
                }
            }
            tdId = "id='THAth_" + ath.MyPK+"'";
            leftBtn += "<div title='编辑附件属性'  style='cursor: pointer' onclick=\"javascript:EditAth('" + groupEn.CtrlID + "')\" >附件:" + lab + "</div>";
            break;
        case "Frame":
            leftBtn += "<div title='编辑框架属性'  style='cursor: pointer' onclick=\"javascript:EditFrame('" + groupEn.CtrlID + "')\" >" + lab + "</div>";
            break;
        case "SubFlow":
            leftBtn += "<div title='编辑子流程属性'  style='cursor: pointer' onclick=\"javascript:EditSubFlow('" + groupEn.CtrlID + "')\" >" + lab + "</div>";
            break;
        case "Dtl":

            leftBtn += "<div title='编辑从表属性' style='cursor: pointer' onclick=\"javascript:EditDtl('" + groupEn.CtrlID + "')\" >从表属性:" + lab + "</div>";

            //中间连接.
            midBtn = "";
            midBtn += '<span style="cursor: pointer" onclick="javascript:AddFForDtl(\'' + groupEn.CtrlID + '\');" ><img src="../../Img/Btn/New.gif" border="0/"><font >插入列</font></span>';
            //  midBtn += "<a title='点击编辑属性' href=\"javascript:AddFForDtl(" + groupEn.CtrlID + ");\" ><img src='../../Img/Btn/New.gif' border=0 >插入列</a>";
            //midBtn += '<a href="javascript:document.getElementById(\'F' + groupEn.CtrlID + '\').contentWindow.AddFGroup(\'' + groupEn.CtrlID + '\');"><img src="../../Img/Btn/New.gif" border="0/">插入列组</a>';
            //midBtn += '<a href="javascript:document.getElementById(\'F'+ groupEn.CtrlID + '\').contentWindow.CopyF(\'' +  groupEn.CtrlID + '\');"><img src="../../Img/Btn/Copy.gif" border="0/">复制列</a>';
            midBtn += "";
            tdId = "id='THDtl_" + groupEn.CtrlID+"'" ;
            break;
        case "FWC":
            leftBtn += "<div title='点击编辑属性' style='cursor: pointer' onclick=\"javascript:FrmNodeComponent()\" >审核审批</div>";
            break;
        default:
            leftBtn += "<div title='点击编辑属性' style='cursor: pointer' onclick=\"javascript:GroupField('" + groupEn.EnName + "','" + groupEn.OID + "')\" >" + lab + "</div>";
            break;
    }
    if (isShow == true) {
        leftBtn += "</div>";


        //右边的按钮都一样.
        var rightBtn = "<div class='cs-order'>" + midBtn;
        rightBtn += "<a href=\"javascript:GroupFieldDoUp('" + groupEn.OID + "' )\"  class='easyui-linkbutton l-btn l-btn-plain'  data-options='iconCls:icon-up,plain:true' ><span class='l-btn-left'><span class='l-btn-text'><span class='l-btn-empty icon-up'>&nbsp;</span></span></span></a>";
        rightBtn += "<a href=\"javascript:GroupFieldDoDown('" + groupEn.OID + "');\" class='easyui-linkbutton l-btn l-btn-plain' data-options='iconCls:icon-down,plain:true'><span class='l-btn-left'><span class='l-btn-text'><span class='l-btn-empty icon-down'>&nbsp;</span></span></span></a>";
        rightBtn += "</div>";

        var html = "<tr class='FoolFrmGroupBarTR' >";
        html += "<td class='FoolFrmGroupBarTD' colspan='" + tabCol +"' " + tdId+ ">";

        html += leftBtn + rightBtn;

        html += "</td>";
        html += "</tr>";
    }

    return html;
}


function AddFForDtl(fk_mapdtl) {

    var url = 'FieldTypeList.htm?FK_MapData=' + fk_mapdtl + '&inlayer=1&s=' + Math.random();
    OpenEasyUiDialog(url, "eudlgframe", "插入列", 800, 500, "icon-edit", true, null, null, null, function () {

        var frm = document.getElementById("F" + fk_mapdtl);
        frm.src = frm.src;

        // window.location.href = window.location.href;
        //ReloadDtlFrame();
    });
}

// 生成内容.
function GenerGroupContext(groupEn, data, tableCol) {

    switch (groupEn.CtrlType) {

        case "Ath":
            if (groupEn.CtrlID == "")
                return "";
            var athEn = $.grep(data.Sys_FrmAttachment, function (ath) { return ath.MyPK == groupEn.CtrlID });

            if (athEn.length == 0) {
                return "<tr  class='FoolFrmFieldTR'  ><td id='Ath_" + athEn.MyPK + "' colspan=" + tableCol + " style='width:100%;height:200px;' > 附件[" + groupEn.CtrlID + "]丢失</td></tr>";
            }
            if (athEn[0].IsVisable == "0")
                return "";

            return "<tr  class='FoolFrmFieldTR'  ><td id='Ath_" + athEn.MyPK + "' colspan=" + tableCol + " style='width:" + athEn[0].H + "px;'><div id='Div_" + athEn[0].MyPK + "'></div></td></tr>";

            break;
        case "SubFlow":

            var flowNo = GetQueryString("FK_Flow");
            var nodeID = GetQueryString("FK_Node");

            var src = "../../WorkOpt/SubFlow.htm?WorkID=0&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;

            var compent = new Entity("BP.WF.Template.FrmSubFlow", nodeID);

            var frameDocs = "<iframe ID='F_SubFlow_" + nodeID + "' frameborder=0 style='padding:0px;border:0px;width:100%;height:" + compent.SF_H + "px;'  leftMargin='0'  topMargin='0' src='" + src + "'  scrolling='auto'  /></iframe>";
            return "<tr  class='FoolFrmFieldTR'  ><td colspan=" + tableCol + " id='SubFlow" + nodeID + "' style='width:300px;'>" + frameDocs + "</td></tr>";

            break;
        case "Frame":
            var frameEn = $.grep(data.Sys_MapFrame, function (frame) { return frame.MyPK == groupEn.CtrlID });
            if (frameEn.length == 0) {
                return "<tr class='FoolFrmFieldTR'  ><td colspan=" + tableCol + " style='width:100%;height:200px;' > 框架[" + groupEn.CtrlID + "]丢失</td></tr>";
            }

            var src = frameEn[0].URL;
            if (src.indexOf('?') == -1)
                src = frameEn[0].URL + "?PKVal=0&FK_MapData=" + pageData.fk_mapdata + "&MapFrame=" + frameEn[0].MyPK;
            else
                src = frameEn[0].URL + "&PKVal=0&FK_MapData=" + pageData.fk_mapdata + "&MapFrame=" + frameEn[0].MyPK;

            var frameUrl = "<iframe ID='F" + frameEn[0].MyPK + "' frameborder=0 style='padding:0px;border:0px;width:100%;height:" + frameEn[0].H + "px;'  leftMargin='0'  topMargin='0' src='" + src + "'  scrolling='auto'  /></iframe>";
            return "<tr class='FoolFrmFieldTR' ><td colspan=" + tableCol + " id='TD" + frameEn[0].MyPK + "' style='width:" + frameEn[0].H + "px;'>" + frameUrl + "</td></tr>";

            break;
        case "FWC":
            return "<tr class='FoolFrmFieldTR' ><td colspan=" + tableCol + " style='width:100%;' ><div id='WorkCheck'></div></td></tr>";
        case "Dtl":

            var dtl = $.grep(data.Sys_MapDtl, function (dtl) { return dtl.No == groupEn.CtrlID });

            if (dtl.length == 1) {
                var src = "MapDtlDe.htm?DoType=Edit&FK_MapData=" + pageData.fk_mapdata + "&FK_MapDtl=" + dtl[0].No;
                var frameUrl = "<iframe ID='F" + dtl[0].No + "' frameborder=0 style='padding:0px;border:0px;width:100%;'  leftMargin='0'  topMargin='0' src='" + src + "'  scrolling='auto'  /></iframe>";
                return "<tr  class='FoolFrmFieldTR' ><td colspan=" + tableCol + " id='Dtl_" + dtl[0].No + "'>" + frameUrl + "</td></tr>";
            } else {
                html = "<tr  class='FoolFrmFieldTR' ><td colspan=" + tableCol + "  id='Dtl_" + dtl[0].No + "' style='width:100%;height:200px;' > 从表[" + groupEn.CtrlID + "]丢失</td></tr>";
                return html;
            }
            break;
        default:
            return "";
    }

    return html;
}

//解析表单是三列的情况
function InitThreeColMapAttr(Sys_MapAttr, tableCol) {
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
        if (attr.UIVisible == 0)
            continue;
        rowSpan = attr.RowSpan;
        if (rowSpan == 0) {
            rowSpan = 1;
            attr.RowSpan = 1;
        }
        colSpan = attr.ColSpan;
        if (colSpan == 0) {
            colSpan = 1;
            attr.ColSpan = 1;
        }

        textColSpan = attr.TextColSpan;
        if (textColSpan == 0) {
            textColSpan = 1;
            attr.TextColSpan = 1;
        }


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
            html += "<tr  class='FoolFrmFieldTR' >";
            html += "<td  colspan='" + tableCol + "' class='FoolFrmFieldCtrl' style='text-align:left:height:auto'>" + str + "</td>";
            html += "</tr>";
            isDropTR = true;
            UseColSpan = 0;
            continue;
        }
        //跨列设置(显示的是文本)
        if (colSpan == 0) {

            if (textColSpan == tableCol) {
                html += "<td  class='FoolFrmFieldName' rowSpan=" + rowSpan + " colspan=" + textColSpan + ">" + GenerLabel(attr) + "</td>";
                isDropTR = true;
                continue;
            }
            //线性展示都跨一个单元格
            //换行的情况
            if (isDropTR == true) {
                html += "<tr  class='FoolFrmFieldTR' >";
                UseColSpan = 0;
                UseColSpan += colSpan + textColSpan;
                html += "<td class='FoolFrmFieldName' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colspan=" + textColSpan + ">" + GenerLabel(attr) + "</td>";

                if (UseColSpan == tableCol) {
                    isDropTR = true;
                } else {
                    isDropTR = false;
                }
                continue;
            }

            if (isDropTR == false) {
                UseColSpan += colSpan + textColSpan;
                html += "<td class='FoolFrmFieldName' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colspan=" + textColSpan + ">" + GenerLabel(attr) + "</td>";
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
            html += "<tr  class='FoolFrmFieldTR' >";
            html += "<td  colspan='" + colSpan + "' rowSpan=" + rowSpan + " class='FoolFrmFieldName' style='text-align:left'>" + GenerLabel(attr) + "</br>";
            html += InitMapAttrOfCtrlFool(attr);
            html += "</td>";
            html += "</tr>";
            isDropTR = true;
            UseColSpan = 0;
            continue;
        }

        //换行的情况
        if (isDropTR == true) {
            html += "<tr  class='FoolFrmFieldTR' >";
            UseColSpan = 0;

            UseColSpan += colSpan;
            html += "<td  id='TD_" + attr.KeyOfEn + "' class='FoolFrmFieldName' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colspan=" + colSpan + " class='tdSpan'>" + GenerLabel(attr) + "<br/>";
            html += InitMapAttrOfCtrlFool(attr);
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
            html += "<td  id='TD_" + attr.KeyOfEn + "' class='FoolFrmFieldName' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colspan=" + colSpan + " class='tdSpan'>" + GenerLabel(attr) + "<br/>";
            html += InitMapAttrOfCtrlFool(attr);
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


//解析表单字段 MapAttr.(表单4列/6列)
function InitMapAttr(Sys_MapAttr, tableCol) {
    var html = "";
    var isDropTR = true;
    //跨行问题定义的字段
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

    var lab = "";
    var rowSpan = 1;
    var colSpan = 1;
    var textColSpan = 1;
    var textWidth = "15%";
    var colWidth = "15%";

    //记录一行已占用的列输
    var UseColSpan = 0;
    var IsMiddle = false;

    //跨行问题
    for (var i = 0; i < Sys_MapAttr.length; i++) {
        var attr = Sys_MapAttr[i];
        if (attr.UIVisible == 0)
            continue;

        rowSpan = attr.RowSpan;
        colSpan = attr.ColSpan;
        textColSpan = attr.TextColSpan;
        if (tableCol == 4) {
            if (colSpan == 1)
                colWidth = "35%";
            else if (colSpan == 2)
                colWidth = "50%";
            else if (colSpan == 3)
                colWidth = "85%";
            textWidth = 15 * parseInt(textColSpan) + "%";
        } else {
            colWidth = 25 * parseInt(colSpan) + "%";
            textWidth = 8 * parseInt(textColSpan) + "%";
        }
        //大文本备注信息 独占一行
        if (attr.UIContralType == 60) {
            //获取文本信息
            var filename = basePath + "/DataUser/CCForm/BigNoteHtmlText/" + attr.FK_MapData + ".htm?r=" + Math.random();
            var htmlobj = $.ajax({ url: filename, async: false });
            var str = htmlobj.responseText;
            if (htmlobj.status == 404)
                str = filename + "这个文件不存在，请联系管理员";
            html += "<tr class='FoolFrmFieldTR'>";
            html += "<td  colspan='" + tableCol + "' class='FoolFrmFieldCtrl' style='text-align:left:height:auto'><a href='#' onclick='EditBigText(\"" + attr.MyPK + "\",\"" + attr.FK_MapData + "\")'>" + str + "</a></td>";
            html += "</tr>";
            isDropTR = true;
            UseColSpan = 0;
            continue;
        }
        //跨列设置(显示的是文本)
        if (colSpan == 0) {

            if (textColSpan >= tableCol) {
                textColSpn = tableCol;
                rowSpan = 1;
                html += "<td  class='FoolFrmFieldName' rowSpan=" + rowSpan + " colspan=" + textColSpan + ">" + GenerLabel(attr) + "</td>";
                isDropTR = true;
                continue;
            }
            //线性展示都跨一个单元格
            //换行的情况
            if (isDropTR == true) {
                html += "<tr class='FoolFrmFieldTR'>";
                UseColSpan = 0;
                luColSpan = 0;
                if (IsShowLeft == true) {
                    UseColSpan += colSpan + textColSpan + ruColSpan;
                    lRowSpan = rowSpan;
                    luColSpan += colSpan + textColSpan;
                    html += "<td class='FoolFrmFieldName' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colspan=" + textColSpan + ">" + GenerLabel(attr) + "</td>";
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
                        html += "<td  class='" + labClass + "' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colspan=" + textColSpan + ">" + lab + "</td>";
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
                        html += "<td class='FoolFrmFieldName' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colspan=" + textColSpan + ">" + GenerLabel(attr) + "</td>";
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
        //解析占一行的情况
        if (colSpan == tableCol) {
            rowSpan = 1;
            html += "<tr  class='FoolFrmFieldTR' >";
            html += "<td  colspan='" + colSpan + "' rowSpan=" + rowSpan + " class='FoolFrmFieldName' style='text-align:left'>" + GenerLabel(attr) + "</td>";
            html += "</tr>";
            html += "<tr class='FoolFrmFieldTR' >";
            html += "<td  id='TD_" + attr.KeyOfEn + "' colspan='" + colSpan + "' rowSpan=" + rowSpan + " class='FoolFrmFieldCtrl' style='text-align:left'>";
            html += InitMapAttrOfCtrlFool(attr);
            html += "</td>";
            html += "</tr>";
            isDropTR = true;
            UseColSpan = 0;
            continue;
        }
        var sumColSpan = colSpan + textColSpan;
        if (sumColSpan == tableCol) {
            isDropTR = true;
            UseColSpan = 0;
            rowSpan = 1;
            html += "<tr class='FoolFrmFieldTR' >";
            html += "<td  id='TD_" + attr.KeyOfEn + "' class='FoolFrmFieldName' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colspan=" + textColSpan + " class='tdSpan'>" + GenerLabel(attr) + "</td>";
            html += "<td  class='FoolFrmFieldCtrl' id='TD_" + attr.KeyOfEn + "'  style='width:" + colWidth + ";' colspan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
            html += InitMapAttrOfCtrlFool(attr);
            html += "</td>";
            html += "</tr>";
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
                html += "<td  id='TD_" + attr.KeyOfEn + "' class='FoolFrmFieldName' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colspan=" + textColSpan + " class='tdSpan'>" + GenerLabel(attr) + "</td>";
                html += "<td  class='FoolFrmFieldCtrl' id='TD_" + attr.KeyOfEn + "'  style='width:" + colWidth + ";' colspan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
                html += InitMapAttrOfCtrlFool(attr);
                html += "</td>";
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
                    luColSpan = colSpan + textColSpan;
                    lRowSpan = rowSpan;
                    html += "<td  id='TD_" + attr.KeyOfEn + "' class='FoolFrmFieldName' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colspan=" + textColSpan + " class='tdSpan'>" + GenerLabel(attr) + "</td>";
                    html += "<td  class='FoolFrmFieldCtrl' id='TD_" + attr.KeyOfEn + "'  style='width:" + colWidth + ";' colspan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
                    html += InitMapAttrOfCtrlFool(attr);
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
                    html += "<td  id='TD_" + attr.KeyOfEn + "' class='FoolFrmFieldName' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colspan=" + textColSpan + " class='tdSpan'>" + GenerLabel(attr) + "</td>";
                    html += "<td  class='FoolFrmFieldCtrl' id='TD_" + attr.KeyOfEn + "'  style='width:" + colWidth + ";' colspan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
                    html += InitMapAttrOfCtrlFool(attr);
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
    return html;
}


function InitMapAttrOfCtrlFool(mapAttr) {
    var elemHtml = "";
    var ccsCtrl = " class='form-control' ";
    //  mapAttr.CCSCtrl = "HongSeTEXTBOXFengGe";
    if (mapAttr.CSSCtrl != "")
        ccsCtrl = " class='" + mapAttr.CSSCtrl + "'";

    if (mapAttr.MyDataType == "1" && mapAttr.UIContralType == 2) {
        var rbHtmls = "";
        var ses = new Entities("BP.Sys.SysEnums");
        ses.Retrieve("EnumKey", mapAttr.UIBindKey, "IntKey");

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
            if (se.IntKey == mapAttr.DefVal)
                checked = " checked=true";

            rbHtmls += "<label style='font-weight:normal;'><input " + ccsCtrl + " type=checkbox name='CB_" + mapAttr.KeyOfEn + "' id='CB_" + mapAttr.KeyOfEn + "_" + se.IntKey + "' value='" + se.IntKey + "' " + checked + " onclick='clickEnable( this ,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")' />" + se.Lab + " </label>&nbsp;" + br;
        }

        return "<div id='DIV_" + mapAttr.KeyOfEn + "'>" + rbHtmls + "</div>";
    }
    if (mapAttr.MyDataType == "1") {
        //字段附件
        if (mapAttr.UIContralType == 6) {
            return "<div " + ccsCtrl + " style='text-align:left;padding-left:10px' id='athModel_" + mapAttr.KeyOfEn + "'><label>请点击[" + mapAttr.Name + "]执行上传</label></div>";
        }
        //写字板
        if (mapAttr.UIContralType == 8) {
            return "<img  src='../../../DataUser/Siganture/admin.jpg' " + ccsCtrl + " style='border:0px;height:" + mapAttr.UIHeight + "px;' id='Img" + mapAttr.KeyOfEn + "' />";
        }
        //地图控件
        if (mapAttr.UIContralType == 4) {
            var eleHtml = "<div style='text-align:left;padding-left:0px' id='athModel_" + mapAttr.KeyOfEn + "' data-type='1'>";
            eleHtml += "<input type='button' name='select' value='选择' " + ccsCtrl + " />";
            eleHtml += "<input type=text " + ccsCtrl + " style='width:75%' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' />";
            eleHtml += "</div>";
            return eleHtml;
        }
        //身份证
        if (mapAttr.UIContralType == 13 && mapAttr.KeyOfEn == "IDCardAddress") {
            var eleHtml = "<div style='text-align:left;padding-left:0px'  data-type='1'>";
            eleHtml += "<input type=text " + ccsCtrl + " style='width:75% !important;display:inline;' class='form-control' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'/>";
            eleHtml += "<label class='image-local' style='margin-left:5px'><input type='file' accept='image/png,image/bmp,image/jpg,image/jpeg' style='width:25% !important;display:none' onchange='GetIDCardInfo()'/>上传身份证</label>";
            eleHtml += "</div>";
            return eleHtml;
        }
        //评分
        if (mapAttr.UIContralType == 101) {
            var eleHtml = "<div style='text-align:left;padding-left:0px'  data-type='1'>";
            eleHtml += "<span class='simplestar'>";

            var num = mapAttr.Tag2;
            for (var i = 0; i < num; i++) {

                eleHtml += "<img src='../../Style/Img/star_2.png' />";
            }
            eleHtml += "&nbsp;&nbsp;<span " + ccsCtrl + " class='score-tips' style='vertical-align: middle;color:#ff6600;font: 12px/1.5 tahoma,arial,\"Hiragino Sans GB\",宋体,sans-serif;'><strong>" + num + "  分</strong></span>";
            eleHtml += "</span></div>";
            return eleHtml;
        }

        //   if (mapAttr.Name == '保存')
        //alert(mapAttr.UIContralType);

        //超链接
        if (mapAttr.UIContralType == 9) {
            var btn = "<a " + ccsCtrl + " id='Link_" + mapAttr.KeyOfEn + "' href='" + mapAttr.Tag2 + "' target='" + mapAttr.Tag1 + "' name='Link_" + mapAttr.KeyOfEn + "' >" + mapAttr.Name + "</a>";
            return btn;
        }

        //按钮
        if (mapAttr.UIContralType == 18) {
            var btn = "<input type='button' " + ccsCtrl + "  id='Btn_" + mapAttr.KeyOfEn + "' name='Btn_" + mapAttr.KeyOfEn + "' value='" + mapAttr.Name + "' onclick=''/>";
            return btn;
        }

        //工作进度图
        if (mapAttr.UIContralType == 50) {
            return "<img  src='./Img/JobSchedule.png'  " + ccsCtrl + " style='border:0px;height:" + mapAttr.UIHeight + "px;width:100%;' id='Img" + mapAttr.KeyOfEn + "' />";
        }

        if (mapAttr.UIContralType == 16) {
            var eleHtml = "<div style='text-align:left;padding-left:0px' >";
            eleHtml += "<input type='button' " + ccsCtrl + " name='select' value='系统定位' />";
            eleHtml += "</div>";
            return eleHtml;
        }

        if (mapAttr.UIHeight <= 40)
            return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'><input " + ccsCtrl + " class='form-control' style='width:95%;' value='" + mapAttr.DefVal + "' maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "'placeholder='" + (mapAttr.Tip || '') + "' type='text' " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + "/></div>";

        if (mapAttr.UIHeight > 23) {
            var uiHeight = mapAttr.UIHeight;
            return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'> <textarea " + ccsCtrl + " class='form-control' maxlength=" + mapAttr.MaxLen + " style='height:" + uiHeight + "px;width:100%;' name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "'placeholder='" + (mapAttr.Tip || '') + "' type='text' " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + "/></div>";
        }

        return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'> <input " + ccsCtrl + " class='form-control' maxlength=" + mapAttr.MaxLen + "  value='" + mapAttr.DefVal + "' name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "'placeholder='" + (mapAttr.Tip || '') + "' type='text' " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + " /></div>";
    }

    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 0) {
        return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'><input " + ccsCtrl + "  value='0' style='text-align:right;'  onkeyup=" + '"' + "valitationAfter(this, 'int');if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'int');if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "'placeholder='" + (mapAttr.Tip || '') + "'/></div>";
    }

    if (mapAttr.MyDataType == 5 || mapAttr.MyDataType == 3) {
        var attrdefVal = mapAttr.DefVal;
        var bit;
        if (attrdefVal != null && attrdefVal !== "" && attrdefVal.indexOf(".") >= 0)
            bit = attrdefVal.substring(attrdefVal.indexOf(".") + 1).length;

        return "<input " + ccsCtrl + " value='0.00' style='text-align:right;'  onkeyup=" + '"' + "valitationAfter(this, 'float');if(isNaN(value)) execCommand('undo');limitLength(this," + bit + ");" + '"' + " onafterpaste=" + '"' + " valitationAfter(this, 'float');if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
    }

    if (mapAttr.MyDataType == 6 || mapAttr.MyDataType == 7) {
        //生成中间的部分.
        var enableAttr = '';
        var dateFmt = "yyyy-MM-dd"; //日期格式.
        if (mapAttr.MyDataType == 7)
            dateFmt = "yyyy-MM-dd HH:mm";

        if (mapAttr.UIIsEnable == 1) {
            enableAttr = '  onfocus="WdatePicker({dateFmt:' + "'" + dateFmt + "'})" + '";';
        } else {
            enableAttr = "disabled='disabled' ";
        }
        return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'> <input " + ccsCtrl + "  class='form-control Wdate' style='width:96%;' maxlength=" + mapAttr.MaxLen + " value='" + mapAttr.DefVal + "'  type='text' " + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "'/></div>";
    }
    if (mapAttr.MyDataType == 8) {
        //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
        var attrdefVal = mapAttr.DefVal;
        var bit;
        if (attrdefVal != null && attrdefVal !== "" && attrdefVal.indexOf(".") >= 0)
            bit = attrdefVal.substring(attrdefVal.indexOf(".") + 1).length;
        else
            bit = 2;
        return "<div id='DIV_" + mapAttr.KeyOfEn + "' class='ccbpm-input-group'><input value='0.00' " + ccsCtrl + " style='text-align:right;' onkeyup=" + '"' + "valitationAfter(this, 'money');limitLength(this," + bit + "); FormatMoney(this, " + bit + ", ',')" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'money');if(isNaN(value))execCommand('undo');" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' value='0.00' placeholder='" + (mapAttr.Tip || '') + "'/></div>";
    }


    if (mapAttr.MyDataType == 4) {
        if (mapAttr.UIIsEnable == 0) {
            enableAttr = "disabled='disabled'";
        } else {
            enableAttr = "";
        }
        return "<div class='checkbox' id='DIV_" + mapAttr.KeyOfEn + "'><label for='CB_" + mapAttr.KeyOfEn + "' >" + mapAttr.Name + "</label><input " + ccsCtrl + " " + (mapAttr.DefVal == 1 ? "checked='checked'" : "") + " type='checkbox' " + enableAttr + " name='CB_" + mapAttr.KeyOfEn + "' id='CB_" + mapAttr.KeyOfEn + "' value='" + mapAttr.Name + "' />&nbsp;</div>";
    }


    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {
        if (mapAttr.UIContralType == 1) { //下拉框

            var ses = GetSysEnums(mapAttr.UIBindKey);

            var operations = "";
            $.each(ses, function (i, obj) {
                operations += "<option  value='" + obj.IntKey + "'>" + obj.Lab + "</option>";
            });

            return "<div id='DIV_" + mapAttr.KeyOfEn + "'><select " + ccsCtrl + " name='DDL_" + mapAttr.KeyOfEn + "' id='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + "  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + operations + "</select></div>";

        } else if (mapAttr.UIContralType == 3) { //单选按钮

            var rbHtmls = "";
            var ses = GetSysEnums(mapAttr.UIBindKey);

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

                rbHtmls += "<label style='font-weight:normal;'><input " + ccsCtrl + " type=radio name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + se.IntKey + "' value='" + se.IntKey + "' " + checked + " onclick='clickEnable( this ,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")' />" + se.Lab + " </label>&nbsp;" + br;
            }

            return "<div id='DIV_" + mapAttr.KeyOfEn + "'>" + rbHtmls + "</div>";
        }
    }
}

function GetSysEnums(enumKey) {

    if (webUser.CCBPMRunModel == 0 || webUser.CCBPMRunModel == 1) {
        var ses = new Entities("BP.Sys.SysEnums");
        ses.Retrieve("EnumKey", enumKey, "IntKey");
        return ses;
    }

    var ses = new Entities("BP.Cloud.Sys.SysEnums");
    ses.Retrieve("RefPK", webUser.OrgNo + "_" + enumKey, "IntKey");
    return ses;
}

function GenerLabel(attr) {

    var fk_mapdata = GetQueryString("FK_MapData");

    var tdUp = "<a href=\"javascript:Up('" + attr.MyPK + "','1');\" class='easyui-linkbutton l-btn l-btn-plain' data-options='iconCls:icon-left,plain:true'  alt='向左动顺序' ><span class='l-btn-left'><span class='l-btn-text'><span class='l-btn-empty icon-left'>&nbsp;</span></span></span></a>";
    var tdDown = "<a href=\"javascript:Down('" + attr.MyPK + "','1');\" class='easyui-linkbutton l-btn l-btn-plain' data-options='iconCls:icon-right,plain:true' alt='向右动顺序' ><span class='l-btn-left'><span class='l-btn-text'><span class='l-btn-empty icon-right'>&nbsp;</span></span></span></a>";
    var ccsLab = "";
    if (attr.CSSLabel != "")
        ccsLab = " class='" + attr.CSSLabel + "'";
    if (attr.LGType == 0 && attr.UIContralType == 1) {
        return tdUp + "<span " + ccsLab+" style='cursor: pointer' onclick=\"javascript:EditTableSQL('" + attr.MyPK + "','" + attr.KeyOfEn + "');\" >" + attr.Name + "</span>" + tdDown;
    }

    if (attr.LGType == 0) {
        return tdUp + "<span " + ccsLab +" style='cursor: pointer' onclick=\"javascript:Edit('" + attr.MyPK + "','" + attr.MyDataType + "','" + attr.GroupID + "','" + attr.LGType + "','" + attr.UIContralType + "');\" >" + attr.Name + "</span>" + tdDown;
    }

    if (attr.LGType == 1)
        return tdUp + "<span " + ccsLab +" style='cursor: pointer' onclick=\"javascript:EditEnum('" + attr.FK_MapData + "','" + attr.MyPK + "','" + attr.KeyOfEn + "');\" >" + attr.Name + "</span>" + tdDown;

    if (attr.LGType == 2)
        return tdUp + "<span " + ccsLab +" style='cursor: pointer' onclick=\"javascript:EditTable('" + attr.FK_MapData + "','" + attr.MyPK + "','" + attr.KeyOfEn + "');\" >" + attr.Name + "</span>" + tdDown;
}

//动态的装载css.
function LoadCss() {
    // 动态加载css
    var loadStyle = function (url) {
        var link = document.createElement('link');
        link.rel = "stylesheet";
        link.type = "text/css";
        link.href = url;
        var head = document.getElementsByTagName("head")[0];
        head.appendChild(link);
    };

    // css加载
    var url = "../../../DataUser/Style/FoolFrmStyle/Default.css?ref=11" + Math.random();
    loadStyle(url);
    url = "../../../DataUser/Style/GloVarsCSS.css?ref=11" + Math.random();
    loadStyle(url);

    $('head').children(':last').attr({
        rel: "stylesheet",
        type: 'text/css',
        href: url,
    });
}

function FrmWithAdd() {
    var frmID = GetQueryString("FK_MapData");
    var en = new Entity("BP.Sys.MapData", frmID);
    en.FrmW = parseInt(en.FrmW) + 10;
    en.Update();
    window.location.href = window.location.href;
}

function FrmWithCut() {

    var frmID = GetQueryString("FK_MapData");
    var en = new Entity("BP.Sys.MapData", frmID);
    en.FrmW = parseInt(en.FrmW) - 10;
    en.Update();
    window.location.href = window.location.href;

}