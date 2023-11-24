/*目的是为了手机也可通用此脚本.*/
var init;
var wcDesc;
var tks;
var aths;
var SignType; //签名的人员.有两个列  No, SignType,
var nodeid = GetQueryString("FK_Node");
var fk_flow = GetQueryString("FK_Flow");
var workid = GetQueryString("WorkID");
var fid = GetQueryString("FID");

//是否是手机端.
var isMobile = GetQueryString("IsMobile");

//是否只读？
var isReadonly = GetQueryString("IsReadonly");
if (isReadonly != "1")
    isReadonly = "0";
var enName = GetQueryString("EnName");

function getWorkCheck() {
    var node = new Entity("BP.WF.Node", nodeid);
    var data;
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddJson(pageData);
    handler.AddPara("IsReadonly", isReadonly);

    if (node.FWCVer == 0 || node.FWCVer == "" || node.FWCVer == undefined)
        data = handler.DoMethodReturnString("WorkCheck_Init");
    else
        data = handler.DoMethodReturnString("WorkCheck_Init2019");
    if (data.indexOf('err@') != -1) {
        alert(data);
        return;
    }

    init = cceval('(' + data + ')');
    wcDesc = init.WF_FrmWorkCheck[0];
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

        //仅显示自己的审核意见
        if (wcDesc.FWCMsgShow == "1" && this.NodeID == nodeid && this.IsDoc == false) {
            return true;
        }
        //自由模式
        html += "<tr>";

        var tdWidth = "120px";
        if (isMobile == "1")
            tdWidth = "20%;";

        html += "<td " + (this.IsDoc ? ("id='tdnode_" + this.NodeID + "'") : "") + " rowspan='" + (subaths.length > 0 ? 3 : 2) + "' style='padding-left:14px;width:" + tdWidth + ";border:1px solid #D6DDE6;'>";

        var nodeName = this.NodeName;
        nodeName = nodeName.replace('(会签)', '<br>(<font>会签</font>)');
        html += nodeName;
        html += "</td>";

        var fwcs = new Entities("BP.WF.Template.FrmWorkChecks");
        fwcs.Retrieve("NodeID", this.NodeID);
        //获取自定义常用短语
        var DuanYu = fwcs[0].FWCNewDuanYu;
        if (DuanYu != null && DuanYu != undefined) {

            var NewDuanYu = DuanYu.split("@");
        } else {
            var NewDuanYu = "";
        }
        var isEditWorkCheck = this.IsDoc == "1" && isReadonly == false ? true : false;
        //审核意见
        if (isEditWorkCheck == true) {

            html += "<td>";

            html += "<div style='float:left;width:97%;'>";
            var msg = this.Msg;
            if (msg == null)
                msg = "";

            while (msg.indexOf('<BR>') >= 0) {
                msg = msg.replace('<BR>', '\t\n');
            }

            html += "<textarea id='WorkCheck_Doc' maxlength='2000' placeholder='内容不能为空,请输入信息,或者使用常用短语选择,内容不超过2000字.' rows='4' style='font-size:14px;overflow:auto;width:98%;border:thin solid #CCCCCC;margin:5px; padding:5px;'  onkeydown='this.style.height=\"60px\";this.style.height=\"60px\";'>";
            html += msg;
            html += "</textarea>";


            //加入常用短语.
            html += "<div style='float:right'>"
            html += "<select id='DuanYu' onchange='SetDocVal();SaveWorkCheck();' style='border:1px solid black;padding-left: 1px'>";
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
            html += "</select>";
            html += "</div>";
            if (wcDesc.FWCAth == 1) {
                html += "<div style='float:left' id='uploaddiv' onmouseover='UploadFileChange(this)'></div>";
            }

            html += "</div>";

            html += "</td>";
        }
        else {
            var returnMsg = this.ActionType == 2 ? "退回原因：" : "";
            var val = this.Msg.split("WorkCheck@");
            if (val.length == 2)
                this.Msg = val[1];

            html += '<td style="word-wrap: break-word;line-height:30px;margin:5px; padding:5px;font-color:green;" >';
            html += '<font color=green>' + this.Msg + '</font>';
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

        //输出签名.
        if (SignType == null || SignType == undefined) {

            var rdt = this.RDT.substring(0, 16);
            if (rdt == "") {
                var dt = new Date();
                rdt = dt.getFullYear() + "-" + (dt.getMonth() + 1) + "-" + dt.getDate();  // new Date().toString("yyyy-MM-dd HH:mm");
            }
            //签名，日期.
            html += "<tr>";
            html += "<td style='border-bottom:1px solid #d6dde6;text-align:left;height:35px;line-height:35px;'><div style='float:left'><font>签名:</font>";

            if (wcDesc.SigantureEnabel == "0")
                html += GetUserSmallIcon(this.EmpFrom, this.EmpFromT);
            else if (wcDesc.SigantureEnabel == "1")
                html += GetUserSiganture(this.EmpFrom, this.EmpFromT);
            else if (wcDesc.SigantureEnabel == "2")
                html += GetUserHandWriting(this, isEditWorkCheck, this.EmpFromT);
            html += "</div>";

            html += "<div style='float:right;margin-right:20px'> ";
            html += "<font>日期:</font>" + rdt;
            html += "</div>";
            html += "</td>";

            html += "</tr>";

        } else {

            for (var idx = 0; idx < SignType.length; idx++) {

                var st = SignType[idx];
                if (st.No != this.EmpFrom)
                    continue;


                if (st.SignType == 0 || st.SignType == 2 || st.SignType == null) {

                    var rdt = this.RDT.substring(0, 16);

                    html += "<tr>";
                    html += "<td style='border-bottom:1px solid #d6dde6;text-align:left;height:35px;line-height:35px;'><div style='float:left'><font>签名:</font>"
                        + GetUserSmallIcon(this.EmpFrom, this.EmpFromT) + '</div>'
                        + "<div style='float:right;margin-right:20px' ><font>日期:</font>" + (this.IsDoc ? "<span id='rdt'>" : "") + rdt + (this.IsDoc ? "</span>" : "") + "</div></td>";
                    html += "</tr>";
                    break;
                }

                if (st.SignType == 1) {
                    html += "<tr>";
                    html += "<td style='border-bottom:1px solid #d6dde6;text-align:left;height:35px;line-height:35px;'><div style='float:left'>签名:"
                        + GetUserSiganture(this.EmpFrom, this.EmpFromT) + '</div>'
                        + " <div style='float:right;margin-right:20px' >日期:" + (this.IsDoc ? "<span id='rdt'>" : "") + this.RDT + (this.IsDoc ? "</span>" : "") + "</div></td>";
                    html += "</tr>";
                    break;
                }

                if (st.SignType == 2) {

                    html += "<tr>";
                    html += "<td style='border-bottom:1px solid #d6dde6;text-align:left;height:35px;line-height:35px;'><div style='float:left'>签名:"
                        + GetUserSiganture(this.EmpFrom, this.EmpFromT) + '</div>'
                        + " <div style='float:right;margin-right:20px' >日期:" + (this.IsDoc ? "<span id='rdt'>" : "") + this.RDT + (this.IsDoc ? "</span>" : "") + "</div></td>";
                    html += "</tr>";

                    //  alert('电子签名的逻辑尚未编写.');
                    break;
                }
            }
        }

    });

    $("#tbTracks").append(html);

    if ($("#WorkCheck_Doc").length > 0) {
        if (wcDesc.FWCIsFullInfo == 1 && wcDesc.FWCDefInfo && wcDesc.FWCDefInfo.length > 0) {
            SaveWorkCheck();
        }
    }
    $("textarea").trigger("keydown");

    if ($("#uploaddiv").length > 0) {
        AddUploafFileHtm("uploaddiv");
    }
    //签字版点击的操作
    $("#Img_WorkCheck").on("tap", function () {
        $('head').append('<link href="./Scripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />');
        Skip.addJs("./Scripts/bootstrap/js/bootstrap.min.js");
        Skip.addJs("./Scripts/bootstrap/BootstrapUIDialog.js");
        var url = "WorkOpt/HandWriting.htm?WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&FK_Node=" + pageData.FK_Node + "&WritType=WorkCheck";
        OpenBootStrapModal(url, "eudlgframe", "签字版", 400, 240, "icon-edit", false);
    });

}



//保存审核信息
function SaveWorkCheck() {

    var doc = $("#WorkCheck_Doc").val();
    if (isReadonly == true)
        return true;
    if (wcDesc.SigantureEnabel == "2" && writeImg == "") {
        alert("请点击签字版签名");
        return false;
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_WorkOpt");
    handler.AddPara("FK_Flow", fk_flow);
    handler.AddPara("FK_Node", nodeid);
    handler.AddPara("WorkID", workid);
    handler.AddPara("FID", fid);
    handler.AddPara("Doc", doc);
    handler.AddPara("IsCC", GetQueryString("IsCC"));
    handler.AddPara("WriteImg", writeImg.replace(/[+]/g, "~"));
    handler.AddPara("HandlerName", GetQueryString("HttpHandlerName"));
    var data = handler.DoMethodReturnString("WorkCheck_Save");
    if (data.indexOf('err@') != -1) {
        alert(data);
        return false;
    }

    if (data.length > 0) {
        $("#rdt").text(data);
    }
    return true;
}

//删除附件
function DelWorkCheckAth(athPK) {
    isChange = false;
    if (confirm("确定要删除所选文件吗？")) {

        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddPara("MyPK", athPK);
        var msg = handler.DoMethodReturnString("DelWorkCheckAttach");
        if (msg == "true") {
            isChange = true;

            $("#Ath_" + athPK).remove();
        }
        if (msg == "false") {
            mui.toast("删除失败。");
        }
    }
}


//上传附件     
function UploadFileChange(ctrl) {
    $(ctrl).unbind("onblur");
    isChange = false;
}

//签名
function GetUserSiganture(userNo, userName) {
    var func = " oncontextmenu='return false;' ondragstart='return false;'  onselectstart='return false;' onselect='document.selection.empty();'";
    if (webUser && webUser.CCBPMRunModel == 2)
        return "<img src='../../DataUser/Siganture/"+webUser.OrgNo+"/" + userNo + ".jpg?m=" + Math.random() + "' title='" + userName + "' " + func + " style='height:40px;' border=0 onerror=\"src='../../DataUser/Siganture/UnName.jpg'\" />";
    else
        return "<img src='../../DataUser/Siganture/" + userNo + ".jpg?m=" + Math.random() + "' title='" + userName + "' " + func + " style='height:40px;' border=0 onerror=\"src='../../DataUser/Siganture/UnName.jpg'\" />";
}

var writeImg = "";
function GetUserHandWriting(track, isEditWorkCheck, userName) {
    if (isEditWorkCheck == false) {
        if (track.WritImg == null || track.WritImg == "")
            return userName;
        return "<img src='" + track.WritImg.replace(/' '/, '') + "'  style='height:40px;' border=0  />";
    }
    writeImg = track.WritImg;
    //将二进制流存入缓存中
    window.localStorage.setItem("writeImg", writeImg);
    return "<img id='Img_WorkCheck' src='" + track.WritImg + "' onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\"  style='border:0px;height:40px;'  />";
}

//获取人员图像
function GetUserSmallIcon(userNo, userName) {
    return userName;

}
//解析附件 begin
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
        html += "<img alt='删除' align='middle' src='./Img/Btn/Delete.gif' onclick=\"DelWorkCheckAth('" + ath.MyPK + "')\" />&nbsp;&nbsp;";
    }

    html += "<a style='color:Blue; font-size:14;'  href='javaScript:void(0)' onclick=\"AthDown('" + ath.FK_FrmAttachment + "','" + GetQueryString("WorkID") + "','" + ath.MyPK + "', '" + ath.FK_MapData + "')\">" + ath.FileName;
    html += "&nbsp;&nbsp;<img src='./Img/FileType/" + ath.FileExts + ".gif' border=0 onerror=\"src='../Img/FileType/Undefined.gif'\" /></a>";
    html += "&nbsp;&nbsp;</div>";

    return html;
}
//解析附件 end

//附件内容加载
function AddUploafFileHtm(divid) {
    if ($("#file_upload").length == 0) {
        var html = "<div id='s' style='margin-left:10px;margin-top:5px;' >"
            + "<label id='realBtn' style='border-radius: 0px !important;color:White !important; background-color:#5bc0de;border-color:#46b8da;  display: inline-block;" +
            "margin - bottom: 0; font - weight: @btn-font - weight;text - align: center;vertical - align: middle;touch - action: manipulation;cursor: pointer; background - image: none;border: 1px solid transparent;white - space: nowrap;'><input type='file' name='file' id='file' style='display:inline;left:-9999px;position:absolute;' onchange='UploadChange();' ><span>文件上传</span></label>"
            + "</div>";

        $("#" + divid).append(html);
    }
}

//附件上传
function UploadChange() {
    var fileObj = document.getElementById("file").files[0]; // js 获取文件对象
    if (typeof (fileObj) == "undefined" || fileObj.size <= 0) {
        alert("请选择上传的文件.");
        return;
    }
    var fileName = fileObj.name;

    var formData = new FormData();
    var name = $("input").val();
    formData.append("file", fileObj);
    formData.append("name", name);
    var AttachPK = "ND" + GetQueryString("FK_Node") + "_FrmWorkCheck";
    var Url = "";
    var doMethod = "MoreAttach";
    var httpHandlerName = "BP.WF.HttpHandler.WF_CCForm";

    if (plant == 'CCFlow')
        Url = dynamicHandler + "?DoType=HttpHandler&DoMethod=" + doMethod + "&HttpHandlerName=" + httpHandlerName + "&FK_FrmAttachment=" + AttachPK + "&WorkID=" + workid + "&PKVal=" + workid + "&AttachPK=" + AttachPK + "&FK_Node=" + GetQueryString("FK_Node") + "&t=" + new Date().getTime();
    else {
        var currentPath = GetHrefUrl();
        
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        Url = path + "WF/Ath/AttachmentUploadS.do/?FK_FrmAttachment=" + AttachPK + "&PKVal=" + workid + "&AttachPK=" + AttachPK;
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

    Reload();
}
//附件下载
function AthDown(fk_ath, pkVal, delPKVal, fk_mapData, fk_flow, ath) {
    if (plant == "CCFlow")
        SetHref( basePath + '/CCMobile/CCForm/DownFile.aspx?DoType=Down&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_MapData=' + fk_mapData + '&Ath=' + ath);
    else {
        var currentPath = GetHrefUrl();
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        Url = path + 'WF/Ath/downLoad.do?DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=' + fk_node + '&FK_Flow=' + fk_flow + '&FK_MapData=' + fk_mapData + '&Ath=' + ath;
        SetHref(url);
    }


}

//重新加载附件数据
function GetNewUploadedAths(files) {
    var Names = "|";
    for (var field in files) {
        Names += files[field].name + "|";
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddPara("Names", Names);
    handler.AddPara("AttachPK", "ND" + nodeid + "_FrmWorkCheck");
    handler.AddPara("FK_Flow", fk_flow);
    handler.AddPara("FK_Node", nodeid);
    handler.AddPara("WorkID", workid);
    var data = handler.DoMethodReturnString("WorkCheck_GetNewUploadedAths");
    if (data.indexOf('err@') != -1) {
        alert(data);
        return;
    }


    var naths = cceval('(' + data + ')');
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


function SetDocVal() {

    var objS = document.getElementById("DuanYu");
    var val = objS.options[objS.selectedIndex].value;

    if (val == "")
        return;

    document.getElementById("WorkCheck_Doc").value = val;

}

//执行保存
function SaveDtlData() {
    SaveWorkCheck();

}



//为判断是否增加电子签章所用.
function IsCanSendWork() {
    return true;
}