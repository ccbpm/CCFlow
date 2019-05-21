
var frmData = null;
function GenerFoolFrm(mapData, frmData) {

    frmData = frmData;
    var Sys_GroupFields = frmData.Sys_GroupField;

    var node = frmData.WF_Node;
    if (node != undefined)
        node = node[0];

    var frmNode = frmData.WF_FrmNode;
    if (frmNode != undefined)
        frmNode = frmNode[0];

    var tableWidth = 800; //  w - 40;
    var html = "<table style='width:" + tableWidth + "px;' >";
    var frmName = mapData.Name;
    html += "<tr>";
    html += "<td colspan=4 class='TitleFDesc' ><div style='float:left' ><img src='../../DataUser/ICON/LogBiger.png'  style='height:50px;' /></div><div class='form-unit-title' style='float:right;padding:10px;bordder:none;width:70%;font-size: 18px;' ><center><h4><b>" + frmName + "</b></h4></center></div></td>";
    html += "</tr>";


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
                html += "  <th colspan=4 class='form-unit'>" + gf.Lab + "</th>";
                html += "</tr>";

                html += "<tr>";
                html += "  <td colspan='4' class='FDesc'>";

                html += Ele_Dtl(dtl);

                html += "  </td>";
                html += "</tr>";
            }
            continue;
        }


        //附件类的控件.
        if (gf.CtrlType == 'Ath') {

            //获取附件的主键
            var MyPK = gf.CtrlID;
            if (MyPK == "")
                continue;
            //创建附件描述信息.
            var ath = new Entity("BP.Sys.FrmAttachment");
            ath.MyPK = gf.CtrlID;
            if (ath.RetrieveFromDBSources() == 0)
                continue;
            if (ath.IsVisable == "0")
                continue;

            html += "<tr>";
            html += "  <th colspan=4 class='form-unit'>" + gf.Lab + "</th>";
            html += "</tr>";


            html += "<tr>";
            html += "  <td colspan='4' class='FDesc'>";

            html += Ele_Attachment(frmData, gf);

            html += "  </td>";
            html += "</tr>";

            continue;
        }

        //框架类的控件.
        if (gf.CtrlType == 'Frame') {

            html += "<tr>";
            html += "  <th colspan=4 class='form-unit'>" + gf.Lab + "</th>";
            html += "</tr>";

            html += "<tr>";
            html += "  <td colspan='4' class='FDesc'>";

            html += Ele_Frame(frmData, gf);

            html += "  </td>";
            html += "</tr>";

            continue;
        }

        //审核组件,有节点信息,并且当前节点状态不是禁用的,就可以显示.
        if (gf.CtrlType == 'FWC' && node && node.FWCSta != 0) {
            if (node.FormType != 5 || (node.FormType == 5 && frmNode && frmNode.IsEnableFWC == 1)) {
                html += "<tr>";
                html += "  <th colspan=4 class='form-unit'>" + gf.Lab + "</th>";
                html += "</tr>";

                html += "<tr>";
                html += "  <td colspan='4' class='FDesc'>";

                html += Ele_FrmCheck(node);

                html += "  </td>";
                html += "</tr>";

                continue;
            }
        }

        //字段类的控件.
        if (gf.CtrlType == '' || gf.CtrlType == null) {

            html += "<tr>";
            html += "  <th colspan=4 class='form-unit'>" + gf.Lab + "</th>";
            html += "</tr>";

            html += InitMapAttr(frmData.Sys_MapAttr, frmData, gf.OID);
            continue;
        }
    }

    html += "</table>";

    //加入隐藏控件.
    var mapAttrs = frmData.Sys_MapAttr;
    for (var i = 0; i < mapAttrs.length; i++) {
        var attr = mapAttrs[i];
        if (attr["UIVisible"] == 0) {
            var defval = ConvertDefVal(frmData, attr["DefVal"], attr["KeyOfEn"]);
            html += "<input type='hidden' id='TB_" + attr["KeyOfEn"] + "' name='TB_" + attr["KeyOfEn"] + "' value='" + defval + "' />";
        }
    }

    $('#CCForm').html("").append(html);

    if (pageData.IsReadonly == "0")
    //表单联动设置
        Set_Frm_Enable(frmData);

    //处理附件的问题
    var aths = $(".athModel");
    $.each(aths, function (idx, ath) {
        //获取ID
        var name = $(ath).attr('id');
        var keyOfEn = name.replace("athModel_", "");
        $("#Lab_" + keyOfEn).html("<div style='text-align:left'>" + $("#Lab_" + keyOfEn).text() + "</div>");
    });

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
                    if (nowKey != null || nowKey != undefined)
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


    src += "&r=q" + paras;
    var eleHtml = "<iframe width='100%' height='" + h + "' id='FWC' src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=no ></iframe>";
    return eleHtml;
}



//解析表单字段 MapAttr.
function InitMapAttr(Sys_MapAttr, frmData, groupID) {

    var html = "";

    //跨行问题，1.记录是否跨行 2.已经跨了几行 3.跨的行数
    var isShowTdLeft = true;
    var haveDropRowLeft = 0;
    var recordRowLeft = 0;

    var isShowTdRight = true;
    var haveDropRowRight = 1;
    var recordRowRight = 1;

    var isDropTR = true;

    //跨列的字段
    var colSpan = 1;
    var textColSpan = 2;
    var textWidth = "15%";
    var width = "15%";


    var lab = "";
    for (var i = 0; i < Sys_MapAttr.length; i++) {

        var attr = Sys_MapAttr[i];

        if (attr.GroupID != groupID || attr.UIVisible == 0)
            continue;

        //解析Lab 1、文本类型、DDL类型、RB类型、扩张（图片、附件、超链接）
        lab = GetLab(frmData, attr);

        //赋值
        rowSpan = attr.RowSpan;
        colSpan = attr.ColSpan;
        textColSpan = attr.TextColSpan;
        textWidth = 15 * parseInt(textColSpan)+"%";
        width = 15 * parseInt(colSpan)+"%";

        //单元格为0的情况
        if (colSpan == 0) {
            //占一行
            if (textColSpan == 4) {
                isDropTR = true;
                html += "<tr>";
                html += "<td  ColSpan='4' rowSpan=" + rowSpan + " class='LabelFDesc' style='text-align:left'>" + lab + "</br>";
                html += "</tr>";
                continue;
            }
            //线性展示都跨一个单元格
            if (isDropTR == true) {
                html += "<tr >";
                if (isShowTdLeft == true) {
                   
                    recordRowRight = rowSpan;
                    haveDropRowLeft = 0;
                    html += "<td class='LabelFDesc' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colSpan=" + textColSpan + ">" + lab + "</td>";
                    if (rowSpan != 1)
                        isShowTdLeft = false;
                }
                isDropTR = !isDropTR;

                haveDropRowRight++;
                if (haveDropRowRight == recordRowRight) {
                    haveDropRowRight = 0;
                    recordRowRight = 1;
                    isShowTdRight = true;
                }

                if (isShowTdRight == false) {
                    html += "</tr>";
                    isDropTR = true;
                }

                continue;
            }

            if (isDropTR == false) {
                if (isShowTdRight == true) {
                    recordRowLeft = rowSpan;
                    haveDropRowRight = 0;
                    html += "<td class='LabelFDesc' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " colSpan=" + textColSpan + ">" + lab + "</td>";
                    if (rowSpan != 1)
                        isShowTdLeft = false;
                }
                isDropTR = !isDropTR;
                html += "</tr>";
                haveDropRowLeft++;

                if (haveDropRowLeft == recordRowLeft) {
                    haveDropRowLeft = 0;
                    recordRowLeft = 1;
                    isShowTdLeft = true;
                }

                if (isShowTdLeft == false) {
                    html += "<tr>";
                    isDropTR = false;
                }

                continue;
            }

        }


        //线性展示并且colspan=4
        if (colSpan == 4) {
            isDropTR = true;
            html += "<tr>";
            html += "<td  ColSpan='4' rowSpan=" + rowSpan + " class='LabelFDesc' style='text-align:left'>" + lab + "</br>";
            html += "</tr>";
            html += "<tr>";
            html += "<td  id='Td_" + attr.KeyOfEn + "' ColSpan='4' rowSpan=" + rowSpan + " class='FDesc' style='text-align:left'>";
            html += InitMapAttrOfCtrl(attr);
            html += "</td>";
            html += "</tr>";
            continue;
        }

        if ((colSpan == 3 && textColSpan == 1)
            || (colSpan == 2 && textColSpan == 2)
            || (colSpan == 1 && textColSpan == 3)) {

            isDropTR = true;
            html += "<tr >";
            html += "<td  id='Td_" + attr.KeyOfEn + "' class='LabelFDesc' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " ColSpan=" + textColSpan + " class='tdSpan'>" + lab + "</td>";
            html += "<td  class='FDesc' id='Td_" + attr.KeyOfEn + "'  style='width:" + width + ";' ColSpan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
            html += InitMapAttrOfCtrl(attr);
            html += "</td>";
            html += "</tr>";
            isDropTR = true;
            continue;
        }

        //换行的情况
        if (isDropTR == true) {
            html += "<tr >";
            if (isShowTdLeft == true) {
                recordRowLeft = rowSpan;
                haveDropRowLeft = 0;
                html += "<td  id='Td_" + attr.KeyOfEn + "' class='LabelFDesc' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " ColSpan=" + textColSpan + " class='tdSpan'>" + lab + "</td>";
                html += "<td  class='FDesc' id='Td_" + attr.KeyOfEn + "'  style='width:" + width + ";' ColSpan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
                html += InitMapAttrOfCtrl(attr);
                html += "</td>";
                if (rowSpan != 1)
                    isShowTdLeft = false;
            }

            isDropTR = !isDropTR;

            haveDropRowRight++;
            if (haveDropRowRight == recordRowRight) {
                haveDropRowRight = 0;
                recordRowRight = 1;
                isShowTdRight = true;
            }

            if (isShowTdRight == false) {
                html += "</tr>";
                isDropTR = true;
            }

            continue;
        }

        if (isDropTR == false) {
            if (isShowTdRight == true) {
                recordRowRight = rowSpan;
                haveDropRowRight = 0;
                html += "<td  id='Td_" + attr.KeyOfEn + "' class='LabelFDesc' style='width:" + textWidth + ";' rowSpan=" + rowSpan + " ColSpan=" + textColSpan + " class='tdSpan'>" + lab + "</td>";
                html += "<td  class='FDesc' id='Td_" + attr.KeyOfEn + "'  style='width:" + width + ";' ColSpan=" + colSpan + " rowSpan=" + rowSpan + " class='tdSpan'>";
                html += InitMapAttrOfCtrl(attr);
                html += "</td>";

            }
            isDropTR = !isDropTR;
            if (rowSpan != 1)
                isShowTdRight = false;

            html += "</tr>";
            haveDropRowLeft++;

            if (haveDropRowLeft == recordRowLeft) {
                haveDropRowLeft = 0;
                recordRowLeft = 1;
                isShowTdLeft = true;
            }

            if (isShowTdLeft == false) {
                html += "<tr>";
                isDropTR = false;
            }
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

        return "<select style='width:100%' name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
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
            if (count != 0 && sfTable.CodeStruct == "1") {
                return "<select  id='DDL_" + mapAttr.KeyOfEn + "' class='easyui-combotree' style='height:28px;width:60%'></select>";
            }
        }

        return "<select style='width:100%' name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
    }

    //外部数据类型.
    if (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1) {

        //枚举类型.
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        if (mapAttr.UIContralType == 1)
            return "<select style='width:100%' name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
        if (mapAttr.UIContralType == 3) {
            //横向排列
            var RBShowModel = 3;
            if (mapAttr.AtPara.indexOf("@RBShowModel=3") == -1)
                RBShowModel = 0;
            return InitRBShowContent(frmData, mapAttr, defValue, RBShowModel, enableAttr);

        }
    }

    if (mapAttr.MyDataType == "1") {

        //附件
        if (mapAttr.UIContralType == "6") {

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
            handler.AddPara("WorkID", pageData.OID);
            handler.AddPara("FID", pageData.FID);
            handler.AddPara("FK_Node", nodeID);
            handler.AddPara("FK_Flow", pageData.FK_Flow);
            handler.AddPara("IsStartNode", IsStartNode);
            handler.AddPara("PKVal", pageData.OID);
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
            for (var i = 0; i < dbs.length; i++) {
                var db = dbs[i];
                eleHtml += "<label><a style='font-weight:normal;font-size:12px' href=\"javascript:Down2018('" + mypk + "','" + pageData.WorkID + "','" + db.MyPK + "','" + pageData.FK_Flow + "','" + pageData.FK_Node + "','" + mapAttr.FK_MapData + "','" + mypk + "')\"><img src='../Img/FileType/" + db.FileExts + ".gif' />" + db.FileName + "</a></label>&nbsp;&nbsp;&nbsp;"
            }
            eleHtml += "</div>";
            return eleHtml;
        }
       
    }

    //添加文本框 ，日期控件等
    //AppString
    if (mapAttr.MyDataType == "1" && mapAttr.UIContralType == 0) {  //不是外键

        if (mapAttr.UIHeight <= 40) //普通的文本框.
        {
            //如果是图片签名，并且可以编辑
            if (mapAttr.IsSigan == "1" && mapAttr.UIIsEnable == 1 && pageData.IsReadonly != 0) {
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

            return "<input maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "' style='width:100%;height:28px;' type='text'  " + enableAttr + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
        }

        if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {

            var eleHtml = "";
            //如果是富文本就使用百度 UEditor
            if (mapAttr.UIIsEnable == "0" || pageData.IsReadonly == 1) {
                //只读状态直接 div 展示富文本内容
                defValue = defValue.replace(/white-space: nowrap;/g, "");   
                eleHtml += "<div class='richText'>" + defValue + "</div>";
            } else {

                document.BindEditorMapAttr = mapAttr; //存到全局备用.

                //设置编辑器的默认样式
                var styleText = "text-align:left;font-size:12px;";
                styleText += "width:100%;";
                var height = parseInt(mapAttr.UIHeight) - 54;
                styleText += "height:" + height + "px;";
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
        if (mapAttr.UIIsEnable == 1 && pageData.IsReadonly ==0) {
            ondblclick = " ondblclick='figure_Template_HandWrite(\"" + mapAttr.KeyOfEn + "\",\"" + defValue + "\")'";
        }

        var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' value='" + defValue + "' type=hidden />";
        if (defValue.indexOf("../DataUser") != 0)
            defValue = defValue.replace("../DataUser","../../DataUser");
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

        return "<input " + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' type='text' class='Wdate' style='width:100%'  placeholder='" + (mapAttr.Tip || '') + "'/>";
    }

    //时期时间类型.
    if (mapAttr.MyDataType == 7) {

        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return "<input style='width:100%' class='Wdate'  type='text'  " + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "' />";
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

        return "<input " + enableAttr + " " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox'   id='CB_" + mapAttr.KeyOfEn + "'  name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + " /><label for='CB_" + mapAttr.KeyOfEn + "' >" + mapAttr.Name + "</label>";
    }

    //枚举类型.
    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
        //枚举类型.
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        if (mapAttr.UIContralType == 1)
            return "<select style='width:100%' name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable == 1 ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
        if (mapAttr.UIContralType == 3) {
            //横向排列
            var RBShowModel = 3;
            if (mapAttr.AtPara.indexOf("@RBShowModel=3") == -1)
                RBShowModel = 0;
            return InitRBShowContent(frmData, mapAttr, defValue, RBShowModel, enableAttr);
        }

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
        return "<input style='text-align:right;width:100%'  onkeyup=" + '"' + "if(!(value.indexOf('-')==0&&value.length==1)&&isNaN(value)) execCommand('undo');limitLength(this," + bit + ");" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
    }

    if ((mapAttr.MyDataType == 2)) { //AppInt
        var enableAttr = '';
        if (mapAttr.UIIsEnable != 1) {
            enableAttr = "disabled='disabled'";
        }

        return "<input style='text-align:right;;width:100%' onkeyup=" + '"' + "if(!(value.indexOf('-')==0&&value.length==1)&&isNaN(value)) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
    }

    //AppMoney  AppRate
    if (mapAttr.MyDataType == 8) {
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1) {

        } else {
            enableAttr = "disabled='disabled'";
        }

        //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
        var defVal = mapAttr.DefVal;
        var bit;
        if (defVal != null && defVal !== "" && defVal.indexOf(".") >= 0)
            bit = defVal.substring(defVal.indexOf(".") + 1).length;
        else
            bit = 2;

        return "<input style='text-align:right;width:100%' onkeyup=" + '"' + "if(isNaN(value))execCommand('undo');limitLength(this," + bit + ");" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' value='0.00' placeholder='" + (mapAttr.Tip || '') + "'/>";
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
        ctrl.addClass("form-control");
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        ctrl.removeAttr("disabled");
        ctrl.addClass("form-control");
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {
        ctrl.removeAttr("disabled");
        ctrl.addClass("form-control");
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
        ctrl.removeClass("form-control");
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        ctrl.attr("disabled", "disabled");
        ctrl.removeClass("form-control");
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {

        ctrl.attr("disabled", "disabled");
        ctrl.removeClass("form-control");
    }

    ctrl = $("#RB_" + key);
    if (ctrl != null) {
        $('input[name=RB_' + key + ']').attr("disabled", "disabled");
      
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

//初始化 框架
function Ele_Frame(frmData, gf) {
    var frame = new Entity("BP.Sys.MapFrame", gf.CtrlID);
    if (frame == null)
        return "没有找到框架的定义，请与管理员联系。";

    var eleHtml = '';
    //获取框架的类型 0 自定义URL 1 地图开发 2流程轨迹表 3流程轨迹图
    var urlType = frame.UrlSrcType;
    var url = "";
    if (urlType == 0) {
        url = frame.URL;
        if (url.indexOf('?') == -1)
            url += "?1=2";

        if (url.indexOf("@basePath") == 0)
            url = url.replace("@basePath", basePath);

        //处理URL需要的参数
        //1.替换@参数
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
                                url = url.replace(paramArr[1], frmData.MainTable[0][paramArr[1].substr('@WebUser.'.length)]);
                            else
                                url = url.replace(paramArr[1], frmData.MainTable[0][paramArr[1].substr(1)]);
                        }
                    }
                });
            }
        }

        //2.拼接参数
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
        var gwf = frmData.WF_GenerWorkFlow[0];
        if (gwf != null) {
            var atPara = gwf.AtPara;
            if (atPara != null && atPara != "") {
                atPara = atPara.replace(/@/g, '&');
                url = url + atPara;
            }
        }
    }
    if (urlType == 2) //轨迹表
        url = "../WorkOpt/OneWork/Table.htm?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.OID + "&FID=" + pageData.FID;
    if (urlType == 3)//轨迹图
        url = "../WorkOpt/OneWork/TimeBase.htm?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.OID + "&FID=" + pageData.FID;

    eleHtml += "<iframe style='width:100%;height:" + frame.H + "px;' ID='" + frame.MyPK + "'    src='" + url + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    return eleHtml;
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
    if (nodeID != null && nodeID.length>2) {
        var no = nodeID.substring(nodeID.length - 2);
        var IsStartNode = 0;
        if (no == "01")
            url += "&IsStartNode=1"; //是否是开始节点
    }

    var isReadonly = false;
    if (gf.FrmID.indexOf(nodeID) == -1)
        isReadonly = true;


    //创建附件描述信息.
    var ath = new Entity("BP.Sys.FrmAttachment", gf.CtrlID);

    var athPK = gf.CtrlID;
    var noOfObj = athPK.replace(gf.FrmID + "_", "");

    var src = "";

    var athUrl = "Ath.htm";
    var local = window.location.href;
    if (local.indexOf('CCBill') != -1) {
        athUrl = '../CCForm/' + athUrl;
    }


    //这里的连接要取 FK_MapData的值.
    src = athUrl+"?PKVal=" + pageData.OID + "&Ath=" + noOfObj + "&FK_MapData=" + GetQueryString("FK_MapData") + "&FromFrm=" + gf.FrmID + "&FK_FrmAttachment=" + athPK + url + "&M=" + Math.random();

    //自定义表单模式.
    if (ath.AthRunModel == 2) {
        src = "../../DataUser/OverrideFiles/Ath.htm?PKVal=" + pageData.OID + "&Ath=" + noOfObj + "&FK_MapData=" + gf.FrmID + "&FK_FrmAttachment=" + athPK + url + "&M=" + Math.random();
    }

    eleHtml += "<iframe style='width:100%;height:" + ath.H + "px;' ID='Attach_" + gf.CtrlID + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
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

    var dtlUrl = "Dtl2017";
    if (frmDtl.DtlVer == 1)
        dtlUrl = "Dtl2019";

    var local = window.location.href;
    if (local.indexOf('CCBill') != -1) {
        dtlUrl = '../CCForm/' + dtlUrl;
    }


    if (frmDtl.ListShowModel == "0") {
        src = dtlUrl + ".htm?EnsName=" + frmDtl.No + "&RefPKVal=" + refPK + "&IsReadonly=" + isReadonly + "&FK_MapData=" + frmDtl.FK_MapData + "&" + urlParam + "&Version=1";
    }
    else if (frmDtl.ListShowModel == "1") {
        src = "DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + refPK + "&IsReadonly=" + isReadonly + "&FK_MapData=" + frmDtl.FK_MapData + "&" + urlParam + "&Version=1";
    }

    return "<iframe style='width:100%;height:" + frmDtl.H + "px;' ID='Dtl_" + frmDtl.No + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
}

function InitRBShowContent(frmData, mapAttr, defValue, RBShowModel, enableAttr) {
    var rbHtml = "";
    var enums = frmData.Sys_Enum;
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
function Ath_Init(mypk, FK_MapData) {
    var nodeID = pageData.FK_Node;
    var no = nodeID.toString().substring(nodeID.toString().length - 2);
    var IsStartNode = 0;
    if (no == "01")
        IsStartNode = 1;

    var noOfObj = mypk.replace(FK_MapData + "_", "");
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("WorkID", pageData.WorkID);
    handler.AddPara("FID", pageData.FID);
    handler.AddPara("FK_Node", nodeID);
    handler.AddPara("FK_Flow", pageData.FK_Flow);
    handler.AddPara("IsStartNode", IsStartNode);
    handler.AddPara("PKVal", pageData.WorkID);
    handler.AddPara("Ath", noOfObj);
    handler.AddPara("FK_MapData", FK_MapData);
    handler.AddPara("FromFrm", FK_MapData);
    handler.AddPara("FK_FrmAttachment", mypk);
    data = handler.DoMethodReturnString("Ath_Init");
    return data;
}
//弹出附件
function OpenAth(url, title, keyOfEn, athMyPK,atPara, FK_MapData) {
    var H = document.body.clientHeight - 240;

    OpenBootStrapModal(url, "eudlgframe", title, frmData.Sys_MapData[0].FrmW, H, "icon-property", null, null, null, function () {


        //获取附件显示的格式
        var athShowModel = GetPara(atPara, "AthShowModel");

        var ath = new Entity("BP.Sys.FrmAttachment");
        ath.MyPK = athMyPK;
        if (ath.RetrieveFromDBSources() == 0) {
            alert("没有找到附件属性,请联系管理员");
            return;
        }
        var data = Ath_Init(athMyPK, FK_MapData)

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
        if (dbs.length == 0) {
            $("#athModel_" + keyOfEn).html("<label>请点击[" + title + "]执行上传</label>");
            return;
        }

        var eleHtml = "";
        if (athShowModel == "" || athShowModel == 0) {
            $("#athModel_" + keyOfEn).html("<label >附件(" + dbs.length + ")</label>");
            return;
        }

        for (var i = 0; i < dbs.length; i++) {
            var db = dbs[i];
            eleHtml += "<label><a style='font-weight:normal;font-size:12px'  href=\"javascript:Down2018('" + athMyPK + "','" + pageData.WorkID + "','" + db.MyPK + "','" + pageData.FK_Flow + "','" + pageData.FK_Node + "','" + FK_MapData + "')\"><img src='../Img/FileType/" + db.FileExts + ".gif' />" + db.FileName + "</a></label>&nbsp;&nbsp;&nbsp;"
        }
        $("#athModel_" + keyOfEn).html(eleHtml);
    }, null, "black", true);

}

var webUser = new WebUser();
function Down2018(fk_ath, pkVal, delPKVal, FK_Flow, FK_Node, FK_MapData, Ath) {
    if (plant == "CCFlow")
        window.location.href = basePath + '/WF/CCForm/DownFile.aspx?DoType=Down&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=' + FK_Node + '&FK_Flow=' + FK_Flow + '&FK_MapData=' + FK_MapData + '&Ath=' + Ath;
    else {
        var currentPath = window.document.location.href;
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        Url = path + 'WF/Ath/downLoad.do?DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=' + FK_Node + '&FK_Flow=' + FK_Flow + '&FK_MapData=' + FK_MapData + '&Ath=' + Ath;
        window.location.href = Url;
    }

}



//解析傻瓜表单的字段lab
function GetLab(frmData, attr) {
    var lab = "";
    var lab = "";
    var forID = "TB_" + attr.KeyOfEn;
    var contralType = attr.UIContralType;
    if (contralType == 1) {//外键下拉框
        forID = "DDL_" + attr.KeyOfEn;
    }
    if (contralType == 3) {//枚举
        forID = "RB_" + attr.KeyOfEn;
    }
    //文本框，下拉框，单选按钮
    if (contralType == 0 || contralType == 1 || contralType == 3 || contralType == 8) {
        lab = "<label id='Lab_" + attr.KeyOfEn + "' for='" + forID + "' class='" + (attr.UIIsInput == 1 ? "mustInput" : "") + "'>" + attr.Name + "</label>";
        if (attr.UIIsInput == 1 && attr.UIIsEnable == 1) {
            lab += " <span style='color:red' class='mustInput' data-keyofen='" + attr.KeyOfEn + "' >*</span>";
        }
        return lab;
    }
    //附件控件
    if (contralType == 6) {
        //创建附件描述信息.
        var mypk = attr.MyPK;
        var ath = new Entity("BP.Sys.FrmAttachment");
        ath.MyPK = mypk;
        if (ath.RetrieveFromDBSources() == 0) {
            alert("没有找到附件属性,请联系管理员");
            return;
        }

        //附件的url
        var eleHtml = '';
        var nodeID = pageData.FK_Node;
        var url = "";
        url += "&WorkID=" + pageData.WorkID;
        url += "&FK_Node=" + nodeID;
        url += "&FK_Flow=" + pageData.FK_Flow;
        var no = nodeID.toString().substring(nodeID.toString().length - 2);
        var IsStartNode = 0;
        if (no == "01")
            url += "&IsStartNode=" + 1; //是否是开始节点

        var isReadonly = false;
        if (attr.FK_MapData.indexOf(nodeID) == -1)
            isReadonly = true;
        var noOfObj = mypk.replace(attr.FK_MapData + "_", "");
        var src = "";

        //这里的连接要取 FK_MapData的值.
        src = "../CCForm/Ath.htm?PKVal=" + pageData.WorkID + "&FID=" + pageData["FID"] + "&Ath=" + noOfObj + "&FK_MapData=" + attr.FK_MapData + "&FromFrm=" + attr.FK_MapData + "&FK_FrmAttachment=" + mypk + url + "&M=" + Math.random();
        //自定义表单模式.
        if (ath.AthRunModel == 2) {
            src = "../../DataUser/OverrideFiles/Ath.htm?PKVal=" + pageData.WorkID + "&FID=" + pageData["FID"] + "&Ath=" + noOfObj + "&FK_MapData=" + attr.FK_MapData + "&FK_FrmAttachment=" + mypk + url + "&M=" + Math.random();
        }
        lab = "<label id='Lab_" + attr.KeyOfEn + "' for='athModel_" + attr.KeyOfEn + "'><div style='text-align:left'><a href='javaScript:void(0)' onclick='OpenAth(\"" + src + "\",\"" + attr.Name + "\",\"" + attr.KeyOfEn + "\",\"" + attr.MyPK + "\",\"" + attr.AtPara + "\",\"" + attr.FK_MapData + "\")' style='text-align:left'>" + attr.Name + "<image src='../Img/Tree/Dir.gif'></image></a></div></label>";
        return lab;
    }

    //超链接
    if (contralType == 9) {
        //URL @ 变量替换
        var url = attr.Tag2;
        $.each(frmData.Sys_MapAttr, function (i, obj) {
            if (url != null && url.indexOf('@' + obj.KeyOfEn) > 0) {
                url = url.replace('@' + obj.KeyOfEn, frmData.MainTable[0][obj.KeyOfEn]);
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
        if (frmImg.ImgAppType == 0) {//图片类型
            //数据来源为本地.
            var webUser = new WebUser();
            var imgSrc = '';
            if (frmImg.ImgSrcType == 0) {
                //替换参数
                var frmPath = frmImg.ImgPath;
                frmPath = frmPath.replace('@basePath', basePath);
                imgSrc = DealJsonExp(frmData.MainTable[0], frmPath);

            }
            //数据来源为指定路径.
            if (frmImg.ImgSrcType == 1) {
                var url = frmImg.ImgURL.replace('@basePath', basePath);
                imgSrc = DealJsonExp(frmData.MainTable[0], url);
            }
            // 由于火狐 不支持onerror 所以 判断图片是否存在放到服务器端
            if (imgSrc == "" || imgSrc == null)
                imgSrc = "../../DataUser/ICON/CCFlow/LogBig.png";

            return "<img src='" + imgSrc + "' style='width:100%;height:100%' onerror=\"this.src='../../DataUser/ICON/CCFlow/LogBig.png'\" />";

        }
        return "";

    }

    return lab;
}

