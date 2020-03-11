
/*
目的是为了手机也可通用此脚本.
注意不要在手机端修改.
*/
var init;
var wcDesc;
var tks;
var aths;
var SignType; //签名的人员.有两个列  No, SignType,
var nodeid = GetQueryString("FK_Node");
var fk_flow = GetQueryString("FK_Flow");
var workid = GetQueryString("WorkID");
var fid = GetQueryString("FID");

var isCanSend = true; //是否可以发送？

//是否是手机端.
var isMobile = GetQueryString("IsMobile");

//是否只读？
var isReadonly = GetQueryString("isReadonly");
if (isReadonly != "1")
    isReadonly = "0";

var enName = GetQueryString("EnName");
var isChange = false;

$(function () {
    if (!workid || workid == "0") {
        workid = GetQueryString("OID");
    }
    InitPage();
});

function InitPage() {

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("WorkCheck_Init");
    if (data.indexOf('err@') != -1) {
        alert(data);
        return;
    }
    init = eval('(' + data + ')');
    wcDesc = init.wcDesc[0];
    tks = init.Tracks;
    aths = init.Aths;
    SignType = init.SignType; //签名的人员 No,SignType 列, SignType=0 不签名, 1=图片签名, 2=电子签名。

    var html = '';

    if (tks.length == 0) {
        html += '<tr style="background-color: #E2F6FB">';
        html += '<td>&nbsp;</td>';
        html += '</tr>';
        html += '<tr>';
        html += '<td style="word-wrap: break-word;line-height:30px;min-height:100px;">&nbsp;</td>';
        html += '</tr>';
    }

    $.each(tks, function () {

        var subaths = GetSubAths(this.NodeID);

        if (wcDesc.FWCMsgShow == "1" && this.NodeID == nodeid && this.IsDoc == false) {
            return true;
        }

        var fwcs = new Entities("BP.WF.Template.FrmWorkChecks");
        fwcs.Retrieve("NodeID", this.NodeID);

        if (fwcs[0].FWCSta == 2)
            return true;

        //自由模式
        html += "<tr>";

        var tdWidth = "120px";
        if (isMobile == "1")
            tdWidth = "20%;";

        html += "<td " + (this.IsDoc ? ("id='tdnode_" + this.NodeID + "'") : "") + " rowspan='" + (subaths.length > 0 ? 3 : 2) + "' style='width:" + tdWidth + ";border:1px solid #D6DDE6;'>";

        var nodeName = this.NodeName;
        nodeName = nodeName.replace('(会签)', '<br>(<font color=Gray>会签</font>)');
        html += nodeName;
        html += "</td>";
        //获取自定义常用短语
        var DuanYu = fwcs[0].FWCNewDuanYu;
        if (DuanYu != null && DuanYu != undefined) {

            var NewDuanYu = DuanYu.split("@");  
        } else {
            var NewDuanYu = "";
        }
        //审核意见
        if (this.IsDoc == "1" && isReadonly == false) {

            html += "<td>";

            if (wcDesc.FWCAth == 1) {
                html += "<div style='float:right' id='uploaddiv' onmouseover='UploadFileChange(this)'></div>";
            }

            html += "<div style='float:left;width:100%;'>";
            var msg = this.Msg;
            if (msg == null || msg == undefined || msg == "")
                msg = "同意";

            while (msg.indexOf('<BR>') >= 0) {
                msg = msg.replace('<BR>', '\t\n');
            }

            html += "<textarea id='WorkCheck_Doc' maxlength='2000' placeholder='内容不能为空,请输入信息,或者使用常用短语选择,内容不超过2000字.' rows='3' style='width:98%;border-style:solid;margin:5px; padding:5px;' onblur='SaveWorkCheck()' onkeydown='this.style.height=\"60px\";this.style.height=this.scrollHeight+\"px\";setIframeHeight();'>";
            html += msg;
            html += "</textarea>";

            //加入常用短语.
            html += "<br>";
            html += "<select id='DuanYu' onchange='SetDocVal();SaveWorkCheck();' >";
            html += "<option value=''>常用短语</option>";
            for (var i = 0; i < NewDuanYu.length; i++) {
                if (NewDuanYu[i] == "") {
                    continue;
                }
                html += "<option value='" + NewDuanYu[i] + "'>" + NewDuanYu[i] + "</option>";
            }
            html += "<option value='同意'>同意</option>";
            html += "<option value='同意办理'>同意办理</option>";
            html += "<option value='同意,请领导批示.'>同意,请领导批示.</option>";
            html += "<option value='情况属实报领导批准.'>情况属实报领导批准.</option>";
            html += "<option value='不同意'>不同意</option>";
            html += "</select><font color=Gray>内容不要超过2000字</font>";
            html += "</div>";
            html += "</td>";

            //加入立场判断
            if (wcDesc.FWCView != null && wcDesc.FWCView != "" && wcDesc.FWCView != undefined) {
                var fwcView = "";
                if (this.Tag.indexOf("@FWCView") != -1) {
                    var arr = this.Tag.split("@");
                    for (var i = 0; i < arr.length; i++) {
                        if (arr[i].indexOf("FWCView") == -1)
                            continue;
                        else {
                            fwcView = arr[i].replace("FWCView=", "");
                            break;
                        }
                    }
                }

                var str = wcDesc.FWCView.split(",");
                html += "<br>";
                html += "立场:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                var idx = 0;
                for (var i = 0; i < str.length; i++) {
                    if (str[i] == "")
                        continue;
                    var check = "";
                    if (fwcView != "" && idx == parseInt(fwcView))
                        check = "checked = checked";
                    else if (fwcView == "" && idx == 0)
                        check = "checked = checked";
                    html += "<input type='radio' id='RB_FWCView_" + idx + "' name ='RB_FWCView' " + check + " onclick='SaveWorkCheck()' value='" + idx + "'/>" + str[i] + "&nbsp;&nbsp;&nbsp;";
                    idx++;
                }
            }
        }
        else {

            html += '<td style="word-wrap: break-word;line-height:30px;margin:5px; padding:5px;font-color:green;" >';

            var returnMsg = this.ActionType == 2 ? "退回原因：" : "";
            html += '<font color=green>' + returnMsg + this.Msg + '</font>';
            //加入立场判断
            if (this.FWCView != null && this.FWCView != "" && this.FWCView!=undefined) {
                var fwcView = "";
                if (this.Tag.indexOf("@FWCView") != -1) {
                    var arr = this.Tag.split("@");
                    for (var i = 0; i < arr.length; i++) {
                        if (arr[i].indexOf("FWCView") == -1)
                            continue;
                        else {
                            fwcView = arr[i].replace("FWCView=", "");
                            break;
                        }
                    }
                }

                var str = this.FWCView.split(",");
                html += "<br>";
                html += "立场:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                var idx = 0;
                for (var i = 0; i < str.length; i++) {
                    if (str[i] == "")
                        continue;
                    var check = "";
                    if (fwcView != "" && idx == parseInt(fwcView))
                        check = "checked = checked";
                    else if (fwcView == "" && idx == 0)
                        check = "checked = checked";
                    html += "<input type='radio' id='RB_FWCView_" + idx + "' name ='RB_FWCView' " + check + " onclick='SaveWorkCheck()' value='" + idx + "' disabled/>" + str[i] + "&nbsp;&nbsp;&nbsp;";
                    idx++;
                }
            }


            html += '</td>';
        }

        html += '</tr>';

        //附件
        if (subaths.length > 0) {
            var tdid = this.IsDoc ? ("id='aths_" + this.NodeID + "'") : "";

            html += "<tr style='" + (subaths.length > 0 ? "" : "display:none;") + "'>";
            html += "<td " + tdid + " style='word-wrap: break-word;' colspan=2>";
            html += "<b>附件：</b>&nbsp;" + subaths;
            html += "</td>";
            html += "</tr>";
        }

        //输出签名,没有签名的要求.
        if (SignType == null || SignType == undefined) {

            var rdt = this.RDT.substring(0, 16);

            if (rdt == "") {
                var dt = new Date();
                rdt = dt.getFullYear() + "-" + (dt.getMonth() + 1) + "-" + dt.getDate();  // new Date().toString("yyyy-MM-dd HH:mm");
            }

            //签名，日期.
            html += "<tr>";
            html += "<td style='text-align:left;height:35px;line-height:35px;'><div style='float:left'><font color='Gray' >签名:</font>";

            if (wcDesc.SigantureEnabel == "0")
                html += GetUserSmallIcon(this.EmpFrom, this.EmpFromT);
            else
                html += GetUserSiganture(this.EmpFrom, this.EmpFromT);

            html += "</div>";


            html += "<div style='float:right'> ";
            html += "<font color='Gray'>日期:</font>" + rdt;
            html += "</div>";
            html += "</td>";

            html += "</tr>";

        } else {

            for (var idx = 0; idx < SignType.length; idx++) {

                var st = SignType[idx];
                if (st.No != this.EmpFrom)
                    continue;

                var rdt = this.RDT.substring(0, 16);


                if (st.SignType == 0 || st.SignType == 2 || st.SignType == null) {

                    html += "<tr>";
                    html += "<td style='text-align:left;height:35px;line-height:35px;'><div style='float:left'><font color='Gray' >签名:</font>"
                                    + GetUserSmallIcon(this.EmpFrom, this.EmpFromT) + '</div>'
                                    + "<div style='float:right' ><font color='Gray' >日期:</font>" + (this.IsDoc ? "<span id='rdt'>" : "") + rdt + (this.IsDoc ? "</span>" : "") + "</div></td>";
                    html += "</tr>";
                    break;
                }

                if (st.SignType == 1) {
                    html += "<tr>";
                    html += "<td style='text-align:left;height:35px;line-height:35px;'><div style='float:left'><font color='Gray' >签名:</font>"
                                    + GetUserSiganture(this.EmpFrom, this.EmpFromT) + '</div>'
                                    + " <div style='float:right' ><font color='Gray' >日期:</font>" + (this.IsDoc ? "<span id='rdt'>" : "") + rdt + (this.IsDoc ? "</span>" : "") + "</div></td>";
                    html += "</tr>";
                    break;
                }

                if (st.SignType == 2) {

                    html += "<tr>";
                    html += "<td style='text-align:left;height:35px;line-height:35px;'><div style='float:left'><font color='Gray' >签名:</font>"
                                    + GetUserSiganture(this.EmpFrom, this.EmpFromT) + '</div>'
                                    + " <div style='float:right' ><font color='Gray' >日期:</font>" + (this.IsDoc ? "<span id='rdt'>" : "") + rdt + (this.IsDoc ? "</span>" : "") + "</div></td>";
                    html += "</tr>";
                    //  alert('电子签名的逻辑尚未编写.');
                    break;
                }

                //如果是图片密码签名.
                if (st.SignType == 3) {

                    isCanSend = false; //设置不可以发送.
                    html += "<tr>";
                    html += "<td style='text-align:left;height:35px;line-height:35px;'><div style='float:left'><font color='Gray' >签名:</font>";

                    html += "<a href='WorkCheck_CheckPass();'>请输入签名</a>";

                    html += "</div>";

                    html += +" <div style='float:right' ><font color='Gray' >日期:</font>" + (this.IsDoc ? "<span id='rdt'>" : "") + rdt + (this.IsDoc ? "</span>" : "") + "</div></td>";
                    html += "</tr>";
                    break;
                }
            }
        }

        // GenerSiganture(SignType);

    });

    //如果是只读的,是多人处理模式,就把已经审核过的人员信息输出出来.
    if (isReadonly == 1) {
    }

    $("#tbTracks").append(html);

    $(window.parent.document).find("#FWC").css('height', $("#tbTracks").height() + 5);

    if ($("#WorkCheck_Doc").length > 0) {
        if (wcDesc.FWCIsFullInfo == 1 && wcDesc.FWCDefInfo && wcDesc.FWCDefInfo.length > 0) {
            SaveWorkCheck();
        }
    }

    $("textarea").trigger("keydown");

    if ($("#uploaddiv").length > 0) {
         var explorer = window.navigator.userAgent;
        var isIE = explorer.indexOf("compatible") > -1 && explorer.indexOf("MSIE") > -1; //判断是否IE<11浏览器
        if (((explorer.indexOf('MSIE') >= 0) && (explorer.indexOf('Opera') < 0) || (explorer.indexOf('Trident') >= 0)))
                AddUploadify("uploaddiv");
        else
            AddUploafFileHtm("uploaddiv");
    }

}

function SetDocVal() {

    var objS = document.getElementById("DuanYu");
    var val = objS.options[objS.selectedIndex].value;

    if (val == "")
        return;

    document.getElementById("WorkCheck_Doc").value = val;

}
function setIframeHeight() {
    $("#" + window.frameElement.getAttribute("id"), parent.document).height($("body").height() + 40);
}

function SaveWorkCheck() {

    if ($("#WorkCheck_Doc").length == 0)//审核组件只读
        return;

    var doc = $("#WorkCheck_Doc").val();

    if (isReadonly == true)
        return;

    var param = {
        FK_Flow: fk_flow,
        FK_Node: nodeid,
        WorkID: workid,
        FID: fid,
        Doc: doc,
        IsCC: GetQueryString("IsCC")
    };

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddJson(param);
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


function TBHelp(enName) {

    var explorer = window.navigator.userAgent;
    var str = "";
    var url = "../Comm/HelperOfTBEUI.aspx?EnsName=" + enName + "&AttrKey=WorkCheck_Doc&WordsSort=0" + "&FK_MapData=" + enName + "&id=WorkCheck_Doc";
    if (explorer.indexOf("Chrome") >= 0) {
        window.open(url, "sd", "left=200,height=500,top=150,width=600,location=yes,menubar=no,resizable=yes,scrollbars=yes,status=no,toolbar=no");
    }
    else {
        str = window.showModalDialog(url, 'sd', 'dialogHeight: 500px; dialogWidth:600px; dialogTop: 150px; dialogLeft: 200px; center: no; help: no');
        if (str == undefined)
            return;

        $("#WorkCheck_Doc").val(str);
        isChange = true;
    }
}

function AthDown(fk_ath, pkVal, delPKVal, fk_mapData, fk_flow, ath) {
    if (plant == "CCFlow")
        window.location.href = basePath + '/WF/CCForm/DownFile.aspx?DoType=Down&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_MapData=' + fk_mapData + '&Ath=' + ath;
    else {
        var currentPath = window.document.location.href;
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        Url = path + 'WF/Ath/downLoad.do?DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=' + fk_node + '&FK_Flow=' + fk_flow + '&FK_MapData=' + fk_mapData + '&Ath=' + ath;
        window.location.href = Url;
    }
    
    
}

function AthOpenOfiice(fk_ath, pkVal, delPKVal, FK_MapData, NoOfObj, FK_Node) {
    var date = new Date();
    var t = date.getFullYear() + "" + date.getMonth() + "" + date.getDay() + "" + date.getHours() + "" + date.getMinutes() + "" + date.getSeconds();
    var url = '../WebOffice/AttachOffice.aspx?DoType=EditOffice&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + "&FK_MapData=" + FK_MapData + "&NoOfObj=" + NoOfObj + "&FK_Node=" + FK_Node + "&T=" + t;
    window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
}

function AthOpenFileView(pkVal, delPKVal, FK_FrmAttachment, FK_FrmAttachmentExt, fk_flow, fk_node, workID, isCC) {
    var url = '../CCForm/FilesView.aspx?showType=view&DelPKVal=' + delPKVal + '&PKVal=' + pkVal + '&FK_FrmAttachment=' + FK_FrmAttachment + '&FK_FrmAttachmentExt=' + FK_FrmAttachmentExt + '&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node + '&WorkID=' + workID + '&IsCC=' + isCC;
    window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
}

function AthOpenView(pkVal, delPKVal, FK_FrmAttachment, FK_FrmAttachmentExt, FK_Flow, FK_Node, WorkID, IsCC) {
    var url = '../CCForm/FilesView.aspx?showType=view&DelPKVal=' + delPKVal + '&PKVal=' + pkVal + '&FK_FrmAttachment=' + FK_FrmAttachment + '&FK_FrmAttachmentExt=' + FK_FrmAttachmentExt + '&FK_Flow=' + FK_Flow + '&FK_Node=' + FK_Node + '&WorkID=' + WorkID + '&IsCC=' + IsCC;
    window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
}

function UploadFileChange(ctrl) {
    //ctrl.detachEvent('onblur', '');
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
    if (data.length > 0) {
        return userName;
    }
    else {
        return "<img src='../../DataUser/Siganture/" + userNo + ".jpg?m=" + Math.random() + "' title='" + userName + "' " + func + " style='height:40px;' border=0 onerror=\"src='../../DataUser/Siganture/Templete.jpg'\" />";
    }

}

function GetUserSmallIcon(userNo, userName) {

    return userName;

    //return "<img src='../../DataUser/UserIcon/" + userNo + "Smaller.png' border=0  style='height:15px;width:15px;padding-right:5px;vertical-align:middle;'  onerror=\"src='../../DataUser/UserIcon/DefaultSmaller.png'\" />" + userName;
}

function FindSubAths(nd) {
    var subAths = [];

    $.each(aths, function () {
        if (this.NodeID == nd) {
            subAths.push(this);
        }
    });

    return subAths;
}

function GetSubAths(nd) {
    var subAths = FindSubAths(nd);
    var html = '';

    $.each(subAths, function () {
        html += GetAthHtml(this);
    });

    return html;
}

function GetAthHtml(ath) {
    var html = "<div id='Ath_" + ath.MyPK + "' style='margin:5px; display:inline-block;'>";

    if (ath.CanDelete) {
        html += "<img alt='删除' align='middle' src='../Img/Btn/Delete.gif' onclick=\"DelWorkCheckAth('" + ath.MyPK + "')\" />&nbsp;&nbsp;";
    }

    html += "<a style='color:Blue; font-size:14;' href='javaScript:void(0)' onclick=\"AthDown('" + ath.FK_FrmAttachment + "','" + GetQueryString("WorkID") + "','" + ath.MyPK + "', '" + ath.FK_MapData + "')\">" + ath.FileName;
    html += "&nbsp;&nbsp;<img src='../Img/FileType/" + ath.FileExts + ".gif' border=0 onerror=\"src='../Img/FileType/Undefined.gif'\" /></a>";
    html += "&nbsp;&nbsp;</div>";

    return html;
}

function AddUploadify(divid) {
    if ($("#file_upload").length == 0) {
        var html = "<div id='file_upload-queue' class='uploadify-queue'></div>"
                             + "<div id='s' style='float:right;margin-right:10px;' >"
                             + "<input type='file' name='file_upload' id='file_upload' width='60' height='30' />"
                             + "</div>";

        $("#" + divid).append(html);
        var url = "";
        if (plant == 'CCFlow')
            url = basePath + '/WF/CCForm/Handler.ashx?AttachPK=ND' + nodeid + '_FrmWorkCheck&DoType=MoreAttach&EnsName=' + (enName ? enName : '') + '&FK_Flow=' + fk_flow + '&PKVal=' + workid + "&FK_Node=" + GetQueryString("FK_Node");
        else {
            var currentPath = window.document.location.href;
            var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
            url = path + "WF/Ath/AttachmentUpload.do?FK_FrmAttachment=ND" + nodeid + "_FrmWorkCheck&FK_Flow=" + fk_flow + "&PKVal=" + workid+"&FK_Node="+GetQueryString("FK_Node");
        }


        $('#file_upload').uploadify({
            'swf': '../Scripts/Jquery-plug/fileupload/uploadify.swf',
            'uploader':url,
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

                GetNewUploadedAths(queueData.files);
            },
            'removeCompleted': true
        });
    }
}

function AddUploafFileHtm(divid) {
 if ($("#file_upload").length == 0) {
    var html ="<div id='s' style='float:right;margin-right:10px;margin-top:5px;' >"
        + "<label id='realBtn' class='btn btn-info' style=''><input type='file' name='file' id='file' style='display:inline;left:-9999px;position:absolute;' onchange='UploadChange();' ><span>文件上传</span></label>"
        + "</div>";

    $("#" + divid).append(html);
   }
}

function UploadChange() {
        var fileObj = document.getElementById("file").files[0]; // js 获取文件对象
        if (typeof (fileObj) == "undefined" || fileObj.size <= 0) {
            alert("请选择上传的文件.");
            return;
        }
        var fileName = fileObj.name;

//            
//        if (realFileExts != "*.*" && realFileExts.indexOf(fileName.substr(fileName.lastIndexOf('.'))) == -1) {
//            alert("上传附件类型不正确，只能上传" + realFileExts);
//            return;
//        }

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
        var AttachPK = "ND"+ GetQueryString("FK_Node")+ "_FrmWorkCheck" ;
        var Url = "";
        var doMethod = "MoreAttach";
        var httpHandlerName = "BP.WF.HttpHandler.WF_CCForm";

        if (plant == 'CCFlow')
            Url = dynamicHandler + "?DoType=HttpHandler&DoMethod=" + doMethod + "&HttpHandlerName=" + httpHandlerName + "&FK_FrmAttachment=" + AttachPK + "&WorkID=" + workid + "&PKVal=" + workid + "&AttachPK=" + AttachPK + "&FK_Node=" + GetQueryString("FK_Node") + "&parasData=" + parasData + "&t=" + new Date().getTime();
        else {
            var currentPath = window.document.location.href;
            var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
            Url = path + "WF/Ath/AttachmentUploadS.do/?FK_FrmAttachment=" + AttachPK + "&PKVal=" + workid + "&AttachPK=" + AttachPK + "&parasData=" + parasData;
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
                GetNewUploadedAths(fileObj)
            },
            error: function (responseStr) {
                if (responseStr.indexOf('err@') != -1)
                    alert(responseStr);
            }
        });

        window.location.href = window.location.href;
    }

function GetNewUploadedAths(files) {
    var param = {
        showType: "WorkCheck_GetNewUploadedAths",
        Names: "|",
        AttachPK: nodeid + "_FrmWorkCheck",
        FK_Flow: fk_flow,
        FK_Node: nodeid,
        WorkID: workid
    };
    var Names ="|";
    for (var field in files) {
        Names += files[field].name + "|";
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddPara("Names", Names);
    handler.AddPara("AttachPK", "ND"+nodeid + "_FrmWorkCheck");
    handler.AddPara("FK_Flow", fk_flow);
    handler.AddPara("FK_Node", nodeid);
    handler.AddPara("WorkID", workid);
    var data = handler.DoMethodReturnString("WorkCheck_GetNewUploadedAths");
    if (data.indexOf('err@') != -1) {
        alert(data);
        return;
    }
    var naths = eval('(' + data + ')');
    aths = aths.concat(naths);

    if ($("#aths_" + nodeid).length == 0) {
        if ($("#WorkCheck_Doc").length > 0) {
            var tdid = "id='aths_" + nodeid + "'";
            var html = "<tr><td " + tdid + " style='word-wrap: break-word;'>";
            html += "<b>附件：</b>&nbsp;";
            html += "</td></tr>";

            $("#WorkCheck_Doc").parent("tr").after(html);
        }
    }

    if (wcDesc.FWCShowModel != 0) {
        $("#tdnode_" + nodeid).attr("rowspan", "3");
    }

    $("#aths_" + nodeid).parent().removeAttr("style");

    $.each(naths, function () {
        $("#aths_" + nodeid).append(GetAthHtml(this));
    });
    
}

//执行保存
function SaveDtlData() {
    SaveWorkCheck();
}

//为判断是否增加电子签章所用.
function IsCanSendWork() {
    if (isCanSend == false)
        return false;

    return true;
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

function WorkCheck_ChangePass() {

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddUrlData();
    handler.AddPara("FK_Emp", empNo);
    var data = handler.DoMethodReturnString("WorkCheck_ChangePass");

    if (data.indexOf('err@') == 0 || data.indexOf('info@') == 0) {
        alert(data);
        return;
    }

    delRow(row); //清空单个table tbody

    // 把返回的结果，重新绑定.
    var sas = JSON.parse(data);
    BindTable(sas);
}
