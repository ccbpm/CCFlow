
/*
目的是为了手机也可通用此脚本.
注意不要在手机端修改.
*/
var isCanSend = true; //是否可以发送？
var isChange = false;
var frmWorkCheck;

//审核组件页面初始化
$(function () {

    var checkData = WorkCheck_Init();

    //当前节点审核组件信息
    frmWorkCheck = checkData.WF_FrmWorkCheck[0];

    var tracks = checkData.Tracks;
    var aths = checkData.Aths;
    var SignType = checkData.SignType; //签名的人员 No,SignType 列, SignType=0 不签名, 1=图片签名, 2=电子签名。

    var _Html = '<table style="width:100%">';

    //轨迹数据
    if (tracks.length == 0) {
        _Html += '<tr style="background-color: #E2F6FB">';
        _Html += '<td>&nbsp;</td>';
        _Html += '</tr>';
        _Html += '<tr>';
        _Html += '<td style="word-wrap: break-word;line-height:30px;min-height:100px;">&nbsp;</td>';
        _Html += '</tr>';
    }

    $.each(tracks, function (idx, item) {
        _Html += WorkCheck_Parse(item, aths, frmWorkCheck, SignType, 1, true);
    });
    _Html += "</table>";

    $("#WorkCheck").html(_Html);

    // $(window.parent.document).find("#FWC").css('height', $("#tbTracks").height() + 5);

    if ($("#WorkCheck_Doc").length > 0) {
        if (frmWorkCheck.FWCIsFullInfo == 1 && frmWorkCheck.FWCDefInfo && frmWorkCheck.FWCDefInfo.length > 0) {
            SaveWorkCheck();
        }
    }

    //$("textarea").trigger("keydown");

    if ($("#uploaddiv").length > 0) {
        var explorer = window.navigator.userAgent;
        if (((explorer.indexOf('MSIE') >= 0) && (explorer.indexOf('Opera') < 0) || (explorer.indexOf('Trident') >= 0)))
            AddUploadify("uploaddiv", frmWorkCheck.FWCShowModel);
        else
            AddUploafFileHtm("uploaddiv", frmWorkCheck.FWCShowModel);
    }
    return _Html;

});

function WorkCheck_Init() {
    var data;
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddUrlData()
    if (pageData.FWCVer == 0)
        data = handler.DoMethodReturnString("WorkCheck_Init");
    else
        data = handler.DoMethodReturnString("WorkCheck_Init2019");  //显示反复节点审核的信息

    if (data.indexOf('err@') != -1) {
        alert(data);
        console.log(data);
        return;
    }
    //审核组件的数据集合
    return JSON.parse(data);

}

function GetWorkCheck_Node(checkData, keyOfEn, checkField) {
    //当前节点审核组件信息
    var frmWorkCheck = checkData.WF_FrmWorkCheck[0];
    var isShowCheck = false;
    if (checkField == keyOfEn && pageData.IsReadonly != "1") {
        if ($("#TB_" + keyOfEn).length != 0 && $("#TB_" + keyOfEn).val().indexOf("," + pageData.FK_Node) == -1)
            $("#TB_" + keyOfEn).val($("#TB_" + keyOfEn).val() + "," + pageData.FK_Node);
        isShowCheck = true;
    }


    var tracks = checkData.Tracks;
    var aths = checkData.Aths;
    var SignType = checkData.SignType; //签名的人员 No,SignType 列, SignType=0 不签名, 1=图片签名, 2=电子签名。
    if (tracks.length == 0)
        return "";
    var _Html = '<table style="width:100%">';

    for (var i = 0; i < tracks.length; i++) {
        var track = tracks[i];
        if ($("#TB_" + keyOfEn).length != 0 && $("#TB_" + keyOfEn).val().indexOf("," + track.NodeID) == -1)
            continue;
        _Html += WorkCheck_Parse(track, aths, frmWorkCheck, SignType, 0, isShowCheck);
    }
    _Html += "</table>";
    return _Html;

}
function WorkCheck_Parse(track, aths, frmWorkCheck, SignType, showNodeName, isShowCheck) {
    var _Html = "";
    //解析节点上传的附件
    var subaths = GetSubAths(track.NodeID, aths);

    //仅显示自己的审核意见
    if (frmWorkCheck.FWCMsgShow == "1" && track.NodeID == pageData.FK_Node && track.IsDoc == false) {
        return true;
    }

    //var fwcs = new Entities("BP.WF.Template.FrmWorkChecks");
    //fwcs.Retrieve("NodeID", this.NodeID);

    // if (fwcs[0].FWCSta == 2)
    //   return true;

    _Html += "<tr>";
    if (showNodeName == 1) {
        //显示审核节点的信息/有可能是会签节点
        _Html += "<td " + (track.IsDoc ? ("id='tdnode_" + track.NodeID + "'") : "") + " rowspan='" + (subaths.length > 0 ? 3 : 2) + "' style='width:120px;border:1px solid #D6DDE6;'>";
        var nodeName = track.NodeName;
        nodeName = nodeName.replace('(会签)', '<br>(<font color=Gray>会签</font>)');
        _Html += nodeName;
        _Html += "</td>";
    }

    var isEditWorkCheck = false;
    if (track.IsDoc == "1" && (pageData.IsReadonly == null || pageData.IsReadonly == false) && isShowCheck == true)
        isEditWorkCheck = true;

    //可编辑的审核意见
    if (isEditWorkCheck == true) {

        _Html += "<td style='width:100%;border-bottom-style:none;border-color:#ddd;display:table-cell;' class='only-print-hidden'>";

        //是否启用附件上传
        if (frmWorkCheck.FWCAth == 1) {
            _Html += "<div style='float:right' id='uploaddiv' data-info='" + frmWorkCheck.FWCShowModel + "' onmouseover='UploadFileChange(this)'></div>";
        }
        _Html += "<div style='float:right'><a onmouseover = 'AddCommUseWord(\"" + pageData.FK_Node + "\",\"WorkCheck\",\"WorkCheck_Doc\");' ><span style='font-size:15px;'>常用短语</span>  <img alt='编辑常用审批语言.' src='../WF/Img/Btn/Edit.gif' /></a></div>";

        _Html += "<div style='float:left;width:100%;'>";
        var msg = track.Msg;
        if (msg == null || msg == undefined || msg == "")
            msg = "";
        else
            msg = msg.replace(/<BR>/g, '\t\n');

        _Html += "<textarea id='WorkCheck_Doc' maxlength='2000' placeholder='内容不能为空,请输入信息,或者使用常用短语选择,内容不超过2000字.' rows='3' style='color:blue;width:98%;border-style:solid;margin:5px; padding:5px;' onblur='SaveWorkCheck()' onkeydown='this.style.height=\"60px\";this.style.height=this.scrollHeight+\"px\";'>";
        _Html += msg;
        _Html += "</textarea>";
        _Html += "<br>";

        _Html += "</div>";
        _Html += "</td>";

        //3.加入立场判断
        if (frmWorkCheck.FWCView != null && frmWorkCheck.FWCView != "" && frmWorkCheck.FWCView != undefined) {
            var fwcView = "";
            if (track.Tag.indexOf("@FWCView") != -1) {
                var arr = track.Tag.split("@");
                for (var i = 0; i < arr.length; i++) {
                    if (arr[i].indexOf("FWCView") == -1)
                        continue;
                    else {
                        fwcView = arr[i].replace("FWCView=", "");
                        break;
                    }
                }
            }

            var str = frmWorkCheck.FWCView.split(",");
            _Html += "<br>";
            _Html += "立场:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            var idx = 0;
            for (var i = 0; i < str.length; i++) {
                if (str[i] == "")
                    continue;
                var check = "";
                if (fwcView != "" && idx == parseInt(fwcView))
                    check = "checked = checked";
                else if (fwcView == "" && idx == 0)
                    check = "checked = checked";
                _Html += "<input type='radio' id='RB_FWCView_" + idx + "' name ='RB_FWCView' " + check + " onclick='SaveWorkCheck()' value='" + idx + "'/>" + str[i] + "&nbsp;&nbsp;&nbsp;";
                idx++;
            }
        }
    }//只读的审核意见
    else {

        _Html += '<td style="word-wrap: break-word;line-height:20px;padding:5px;font-color:green;border-bottom-style:none;border-color:#ddd" >';
        //显示退回原因
        var returnMsg = track.ActionType == 2 ? "退回原因：" : "";
        if (pageData.FWCVer == 1) {
            var val = track.Msg.split("WorkCheck@");
            if (val.length == 2)
                track.Msg = val[1];
        }
        _Html += '<font color=green>' + returnMsg + track.Msg + '</font>';
        //加入立场判断
        if (track.FWCView != null && track.FWCView != "" && track.FWCView != undefined) {
            var fwcView = "";
            if (track.Tag.indexOf("@FWCView") != -1) {
                var arr = track.Tag.split("@");
                for (var i = 0; i < arr.length; i++) {
                    if (arr[i].indexOf("FWCView") == -1)
                        continue;
                    else {
                        fwcView = arr[i].replace("FWCView=", "");
                        break;
                    }
                }
            }

            var str = track.FWCView.split(",");
            _Html += "<br>";
            _Html += "立场:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            var idx = 0;
            for (var i = 0; i < str.length; i++) {
                if (str[i] == "")
                    continue;
                var check = "";
                if (fwcView != "" && idx == parseInt(fwcView))
                    check = "checked = checked";
                else if (fwcView == "" && idx == 0)
                    check = "checked = checked";
                _Html += "<input type='radio' id='RB_FWCView_" + idx + "' name ='RB_FWCView' " + check + " onclick='SaveWorkCheck()' value='" + idx + "' disabled/>" + str[i] + "&nbsp;&nbsp;&nbsp;";
                idx++;
            }
        }


        _Html += '</td>';
    }

    _Html += '</tr>';

    //附件
    if (subaths.length > 0) {
        var tdid = track.IsDoc ? ("id='aths_" + track.NodeID + "'") : "";

        _Html += "<tr style='" + (subaths.length > 0 ? "" : "display:none;") + "'>";
        _Html += "<td " + tdid + " style='word-wrap: break-word;' colspan=2>";
        _Html += "<b>附件：</b>&nbsp;" + subaths;
        _Html += "</td>";
        _Html += "</tr>";
    }

    //输出签名,没有签名的要求.
    if (SignType == null || SignType == undefined) {



        //签名，日期.
        //_Html += "<tr>";
        if (track.RDT == "")
            _Html += "<td style='text-align:right;width:100%;border-top-style:none;border-color:#ddd;display:table-cell;' class='only-print-hidden'>";
        else
            _Html += "<td style='text-align:right;border-top-style:none;border-color:#ddd'>";

        if (frmWorkCheck.SigantureEnabel == "0")
            _Html += track.EmpFromT;
        else if (frmWorkCheck.SigantureEnabel == "1")
            _Html += GetUserSiganture(track.EmpFrom, track.EmpFromT);
        else if (frmWorkCheck.SigantureEnabel == "2")
            _Html += GetUserHandWriting(track, isEditWorkCheck, track.EmpFromT);

        var rdt = track.RDT.substring(0, 16);
        if (rdt == "") {
            var dt = new Date();
            rdt = dt.getFullYear() + "-" + (dt.getMonth() + 1) + "-" + dt.getDate();  // new Date().toString("yyyy-MM-dd HH:mm");
        }
        _Html += "(" + rdt + ")";
        _Html += "</td>";

        _Html += "</tr>";

    } else {

        for (var idx = 0; idx < SignType.length; idx++) {

            var st = SignType[idx];
            if (st.No != track.EmpFrom)
                continue;

            var rdt = track.RDT.substring(0, 16);


            if (st.SignType == 0 || st.SignType == 2 || st.SignType == null) {

                _Html += "<tr>";
                _Html += "<td style='text-align:left;height:35px;line-height:35px;'>" + track.DeptName + "<div style='float:right'>签名:"
                    + track.EmpFromT
                    + "日期:" + (track.IsDoc ? "<span id='rdt'>" : "") + rdt + (track.IsDoc ? "</span>" : "") + "</div></td>";
                _Html += "</tr>";
                break;
            }
            //图片签名
            if (st.SignType == 1) {
                _Html += "<tr>";
                _Html += "<td style='text-align:left;height:35px;line-height:35px;'>" + track.DeptName + "<div style='float:right'>签名:"
                    + GetUserSiganture(track.EmpFrom, track.EmpFromT)
                    + "日期:" + (track.IsDoc ? "<span id='rdt'>" : "") + rdt + (track.IsDoc ? "</span>" : "") + "</div></td>";
                _Html += "</tr>";
                break;
            }
            //写字板
            if (st.SignType == 2) {
                _Html += "<tr>";
                _Html += "<td style='text-align:left;height:35px;line-height:35px;'>" + track.DeptName + "<div style='float:right'>签名:"
                    + GetUserSiganture(track.EmpFrom, track.EmpFromT)
                    + "日期:" + (track.IsDoc ? "<span id='rdt'>" : "") + rdt + (track.IsDoc ? "</span>" : "") + "</div></td>";
                _Html += "</tr>";
                break;
            }

            if (st.SignType == 2) {

                _Html += "<tr>";
                _Html += "<td style='text-align:left;height:35px;line-height:35px;'>" + track.DeptName + "<div style='float:right'>签名:"
                    + GetUserHandWriting(track, isEditWorkCheck, track.EmpFromT);
                + "日期:" + (track.IsDoc ? "<span id='rdt'>" : "") + rdt + (track.IsDoc ? "</span>" : "") + "</div></td>";
                _Html += "</tr>";
                //  alert('电子签名的逻辑尚未编写.');
                break;
            }

            //如果是图片密码签名.
            if (st.SignType == 3) {

                isCanSend = false; //设置不可以发送.
                _Html += "<tr>";
                _Html += "<td style='text-align:left;height:35px;line-height:35px;'>" + track.DeptName + "<div style='float:right'>签名:";

                _Html += "<a href='WorkCheck_CheckPass();'>请输入签名</a>";

                //_Html += "</div>";

                _Html += +"日期:" + (track.IsDoc ? "<span id='rdt'>" : "") + rdt + (track.IsDoc ? "</span>" : "") + "</div></td>";
                _Html += "</tr>";
                break;
            }
        }
    }
    return _Html;
}

function SetDocVal() {

    var objS = document.getElementById("DuanYu");
    var val = objS.options[objS.selectedIndex].value;

    if (val == "")
        return;
    if ($("#WorkCheck_Doc").length == 1) {
        $("#WorkCheck_Doc").val(val);
    }


}

function SaveWorkCheck() {

    if ($("#WorkCheck_Doc").length == 0)//审核组件只读
        return;

    if (frmWorkCheck.SigantureEnabel == "2" && writeImg == "") {
        alert("请点击签字版签名");
        return;
    }

    var doc = $("#WorkCheck_Doc").val();
    if (doc == "") {
        alert("请填写审核意见");
        return;
    }
    doc = doc.replace(/'/g, '');

    if (pageData.IsReadonly == true)
        return;
    if (pageData.WorkID == null || pageData.WorkID == undefined || pageData.WorkID == 0)
        return;

    var param = {
        FK_Flow: pageData.FK_Flow,
        FK_Node: pageData.FK_Node,
        WorkID: pageData.WorkID,
        FID: pageData.FID,
        Doc: doc,
        IsCC: GetQueryString("IsCC")
    };

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddJson(param);
    if (writeImg == null && writeImg == undefined)
        writeImg = "";
    else
        writeImg = writeImg.replace(/[+]/g, "~");
    handler.AddPara("WriteImg", writeImg);
    var data = handler.DoMethodReturnString("WorkCheck_Save");

    if (data.indexOf('err@') != -1) {
        alert(data);
        return;
    }

    if (data.length > 0) {
        $("#rdt").text(data);
    }
}

function DelWorkCheckAth(athPK) {
    isChange = false;

    if (confirm("确定要删除所选文件吗？") == false)
        return;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("MyPK", athPK);
    var msg = handler.DoMethodReturnString("DelWorkCheckAttach");
    if (msg == "true") {
        isChange = true;
        $("#Ath_" + athPK).remove();
    }
    if (msg == "false") {
        alert("删除失败。");
    }
}

function AthDown(fk_ath, pkVal, delPKVal, fk_mapData, fk_flow, ath) {
    if (plant == "CCFlow") {
        window.location.href = basePath + '/WF/CCForm/DownFile.aspx?DoType=Down&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_MapData=' + fk_mapData + '&Ath=' + ath;
        return;
    }

    var currentPath = window.document.location.href;
    var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
    Url = path + 'WF/Ath/downLoad.do?DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=' + fk_node + '&FK_Flow=' + fk_flow + '&FK_MapData=' + fk_mapData + '&Ath=' + ath;
    window.location.href = Url;
}



function UploadFileChange(ctrl) {
    $(ctrl).unbind("onblur");
    isChange = false;
}

function GetUserSiganture(userNo, userName) {
    var func = " oncontextmenu='return false;' ondragstart='return false;'  onselectstart='return false;' onselect='document.selection.empty();'";
    //先判断，是否存在签名图片
    var handler = new HttpHandler("BP.WF.HttpHandler.WF");
    handler.AddPara('No', userNo);
    data = handler.DoMethodReturnString("HasSealPic");

    //如果不存在，就显示当前人的姓名
    if (data.length > 0)
        return userName;

    return "<img src='../../DataUser/Siganture/" + userNo + ".jpg?m=" + Math.random() + "' title='" + userName + "' " + func + " style='height:40px;' border=0 alt='" + userNo + "' />";
}

var writeImg = "";
//签字版
function GetUserHandWriting(track, isEditWorkCheck, userName) {
    if (isEditWorkCheck == false) {
        if (track.WritImg == null || track.WritImg == "")
            return userName;
        return "<img src='" + track.WritImg.replace(/' '/, '') + "'  style='height:40px;' border=0  />";
    }
    writeImg = track.WritImg;
    return "<img id='Img_WorkCheck' src='" + track.WritImg + "' onclick='openHandWriting()' onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\"  style='border:0px;height:40px;'  />";
}

function openHandWriting() {
    var url = "CCForm/HandWriting.htm?WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&FK_Node=" + pageData.FK_Node + "&WritType=WorkCheck";
    OpenEasyUiDialogExt(url, '签字板', 400, 300, false);
}

/**
 * 获取该节点上传的附件
 * @param {any} nodeID 当前节点ID
 * @param {any} aths 审核组件上传的附件
 */
function GetSubAths(nodeID, aths) {
    var subAths = [];

    //1.获取节点上传的附件
    $.each(aths, function () {
        if (this.NodeID == nodeID) {
            subAths.push(this);
        }
    });

    //2.解析上传的附件
    var _Html = '';
    $.each(subAths, function () {
        _Html += GetAthHtml(this);
    });

    return _Html;
}

function GetAthHtml(ath) {
    var html = "<div id='Ath_" + ath.MyPK + "' style='margin:5px; display:inline-block;'>";
    var src = './';
    if (window.location.href.indexOf("/CCForm") != -1)
        src = '../';
    if (ath.CanDelete == "1" || ath.CanDelete == true) {
        html += "<img alt='删除' align='middle' src='" + src + "Img/Btn/Delete.gif' onclick=\"DelWorkCheckAth('" + ath.MyPK + "')\" />&nbsp;&nbsp;";
    }

    html += "<a style='color:Blue; font-size:14;' href='javaScript:void(0)' onclick=\"AthDown('" + ath.FK_FrmAttachment + "','" + pageData.WorkID + "','" + ath.MyPK + "', '" + ath.FK_MapData + "')\">" + ath.FileName;
    html += "&nbsp;&nbsp;<img src='" + src + "Img/FileType/" + ath.FileExts + ".gif' border=0 onerror=\"src='" + src + "Img/FileType/Undefined.gif'\" /></a>";
    html += "&nbsp;&nbsp;</div>";

    return html;
}


function AddUploadify(divid, fwcShowModel) {
    if ($("#file_upload").length == 0) {
        var html = "<div id='file_upload-queue' class='uploadify-queue'></div>"
            + "<div id='s' style='float:right;margin-right:10px;' >"
            + "<input type='file' name='file_upload' id='file_upload' width='60' height='30' />"
            + "</div>";

        $("#" + divid).append(html);
        var url = "";
        if (plant == 'CCFlow')
            url = basePath + '/WF/CCForm/Handler.ashx?AttachPK=ND' + pageData.FK_Node + '_FrmWorkCheck&DoType=MoreAttach&FK_Flow=' + pageData.FK_Flow + '&PKVal=' + workid + "&FK_Node=" + GetQueryString("FK_Node");
        else {
            var currentPath = window.document.location.href;
            var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
            url = path + "WF/Ath/AttachmentUpload.do?FK_FrmAttachment=ND" + pageData.FK_Node + "_FrmWorkCheck&FK_Flow=" + pageData.FK_Flow + "&PKVal=" + pageData.WorkID + "&FK_Node=" + GetQueryString("FK_Node");
        }


        $('#file_upload').uploadify({
            'swf': '../Scripts/Jquery-plug/fileupload/uploadify.swf',
            'uploader': url,
            'auto': true,
            'fileTypeDesc': '请选择上传文件',
            'buttonText': '上传附件',
            //                    'hideButton': true,
            'width': 60,
            'fileTypeExts': '*.*',
            'height': 18,
            'multi': true,
            'queueSizeLimit': 999,
            'onDialogOpen': function (a, b) {
            },
            'onQueueComplete': function (queueData) {
                isChange = true;

                GetNewUploadedAths(queueData.files, fwcShowModel);
            },
            'removeCompleted': true
        });
    }
}

function AddUploafFileHtm(divid, fwcShowModel) {
    if ($("#file_upload").length == 0) {
        var html = "<div id='s' style='float:right;margin-right:10px;margin-top:5px;' >"
            + "<label id='realBtn' class='btn btn-info' style=''><input type='file' name='file' id='file' style='display:inline;left:-9999px;position:absolute;' onchange='UploadChange(" + fwcShowModel + ");' ><span>文件上传</span></label>"
            + "</div>";

        $("#" + divid).append(html);
    }
}

function UploadChange(fwcShowModel) {
    var fileObj = document.getElementById("file").files[0]; // js 获取文件对象
    if (typeof (fileObj) == "undefined" || fileObj.size <= 0) {
        alert("请选择上传的文件.");
        return;
    }
    var fileName = fileObj.name;


    //form表单序列话
    var parasData = $("form").serialize();
    //form表单序列化时调用了encodeURLComponent方法将数据编码了
    parasData = decodeURIComponent(parasData, true);
    parasData = decodeURIComponent(parasData, true);
    parasData = parasData.replace(/&/g, '@');
    parasData = parasData.replace(/TB_/g, '');
    parasData = parasData.replace(/RB_/g, '');
    parasData = parasData.replace(/CB_/g, '');
    parasData = parasData.replace(/DDL_/g, '');

    var formData = new FormData();
    var name = $("input").val();
    formData.append("file", fileObj);
    formData.append("name", name);
    var AttachPK = "ND" + GetQueryString("FK_Node") + "_FrmWorkCheck";
    var Url = "";
    var doMethod = "MoreAttach";
    var httpHandlerName = "BP.WF.HttpHandler.WF_CCForm";

    if (plant == 'CCFlow')
        Url = dynamicHandler + "?DoType=HttpHandler&DoMethod=" + doMethod + "&HttpHandlerName=" + httpHandlerName + "&FK_FrmAttachment=" + AttachPK + "&WorkID=" + pageData.WorkID + "&PKVal=" + pageData.WorkID + "&AttachPK=" + AttachPK + "&FK_Node=" + GetQueryString("FK_Node") + "&parasData=" + parasData + "&t=" + new Date().getTime();
    else {
        var currentPath = window.document.location.href;
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        Url = path + "WF/Ath/AttachmentUploadS.do/?FK_FrmAttachment=" + AttachPK + "&PKVal=" + pageData.WorkID + "&AttachPK=" + AttachPK + "&parasData=" + parasData;
    }

    Url += "&FID=" + GetQueryString("FID");
    Url += "&FK_Node=" + GetQueryString("FK_Node");
    Url += "&PWorkID=" + GetQueryString("PWorkID");
    Url += "&FK_MapData=" + GetQueryString("FK_MapData");
    //获取分组
    var sort = $("#Sort").val();
    if (sort != null && sort != "" && sort != undefined)
        Url += "&Sort=" + sort;


    $.ajax({
        url: Url,
        type: 'POST',
        data: formData,
        async: false,
        xhrFields: {
            withCredentials: true
        },
        crossDomain: true,
        // 告诉jQuery不要去处理发送的数据
        processData: false,
        // 告诉jQuery不要去设置Content-Type请求头
        contentType: false,
        beforeSend: function () {
            console.log("正在进行，请稍候");
        },
        success: function (responseStr) {
            GetNewUploadedAths(fileObj, fwcShowModel)
        },
        error: function (responseStr) {
            if (responseStr.indexOf('err@') != -1)
                alert(responseStr);
        }
    });

}
/**
 * 显示上传的附件
 * @param {any} files
 */
function GetNewUploadedAths(files, fwcShowModel) {
    var params = {
        AttachPK: pageData.FK_Node + "_FrmWorkCheck",
        FK_Flow: pageData.FK_Flow,
        FK_Node: pageData.FK_Node,
        WorkID: pageData.WorkID
    };
    var Names = "|" + files.name + "|";

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddPara("Names", Names);
    handler.AddJson(params);
    var data = handler.DoMethodReturnString("WorkCheck_GetNewUploadedAths");
    if (data.indexOf('err@') != -1) {
        alert(data);
        console.log(data);
        return;
    }
    var naths = eval('(' + data + ')');


    if ($("#aths_" + pageData.FK_Node).length == 0) {
        if ($("#WorkCheck_Doc").length > 0) {
            var tdid = "id='aths_" + pageData.FK_Node + "'";
            var html = "<tr><td " + tdid + " style='word-wrap: break-word;'>";
            html += "<b>附件：</b>&nbsp;";
            html += "</td></tr>";

            $("#WorkCheck_Doc").parent().parent().parent().after(html)
        }
    }

    if (fwcShowModel != 0) {
        $("#tdnode_" + pageData.FK_Node).attr("rowspan", "3");
    }

    $("#aths_" + pageData.FK_Node).parent().removeAttr("style");

    $.each(naths, function () {
        $("#aths_" + pageData.FK_Node).append(GetAthHtml(this));
    });

}

//当用户点击签名图片的时候，弹出窗体让其输入密码.
function WorkCheck_CheckPass() {

    var pass = window.prompt('请输入签名密码，初始化密码为123，您可以修改该密码.', "");
    if (pass == undefined || pass == "")
        return;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddPara("SPass", pass);
    var data = handler.DoMethodReturnString("WorkCheck_CheckPass");

    if (data.indexOf('err@') == 0 || data.indexOf('info@') == 0) {
        alert(data);
        return;
    }

    //让其可以发送.
    isCanSend = true;
    //签名成功后，就需要把图片显示出来.

}




