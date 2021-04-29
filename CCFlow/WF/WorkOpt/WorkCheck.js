
/*
目的是为了手机也可通用此脚本.
注意不要在手机端修改.
*/
var isCanSend = true; //是否可以发送？
var isChange = false;
var frmWorkCheck;


var checkParam = {
    FK_Flow: GetQueryString("FK_Flow"),
    FK_Node: GetQueryString("FK_Node"),
    WorkID: GetQueryString("WorkID"),
    FID: GetQueryString("FID"),
    IsReadonly: IsReadOnly(),
    IsCC: GetQueryString("IsCC")
};

//是否只读?
function IsReadOnly() {
    //如果是MyFlowView 或者是MyCC 就把该控件设置为只读的.
    var url = window.location.href;
    if (url.indexOf('MyViewGener') != -1 || url.indexOf('MyCC') != -1 || url.indexOf('MyFrm') != -1) {
        return true;
    }
    var val = GetQueryString("IsReadOnly") != null && GetQueryString("IsReadOnly") != undefined && GetQueryString("IsReadOnly") == "1" ? true : false;
    return val;
}
 

//审核组件页面初始化
$(function() {
    var FWCVer = null;
    if (FWCVer == null) {
        var node = new Entity("BP.WF.Node", checkParam.FK_Node);
        if (node != null && (node.FWCVer == 0 || node.FWCVer == "" || node.FWCVer == undefined))
            FWCVer = 0;
        else
            FWCVer = 1;

    }
    var checkData = WorkCheck_Init(FWCVer);

    //当前节点审核组件信息
    frmWorkCheck = checkData.WF_FrmWorkCheck[0];
    
    var tracks = checkData.Tracks;
    var aths = checkData.Aths;
    var SignType = checkData.SignType; //签名的人员 No,SignType 列, SignType=0 不签名, 1=图片签名, 2=电子签名。

    var _Html = '';

    //轨迹数据
    if (tracks.length != 0) {
        _Html += '<table style="width:100%">';
        $.each(tracks, function(idx, item) {
            _Html += WorkCheck_Parse(item, aths, frmWorkCheck, SignType, 1, true, FWCVer);
        });
        _Html += "</table>";

    }



    $("#WorkCheck").html(_Html);

    // $(window.parent.document).find("#FWC").css('height', $("#tbTracks").height() + 5);

    if ($("#WorkCheck_Doc").length > 0) {
        if (frmWorkCheck.FWCIsFullInfo == 1 && frmWorkCheck.FWCDefInfo && frmWorkCheck.FWCDefInfo.length > 0) {
            SaveWorkCheck(0);
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

function WorkCheck_Init(FWCVer) {
    var data;
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddUrlData()
    if (FWCVer == 0)
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
var map = {}; //求出来要输出的1 人1意见的最后数据.
//签批组件专用方法.
function GetWorkCheck_Node(checkData, keyOfEn, checkField, FWCVer) {
    //当前节点审核组件信息
    var frmWorkCheck = checkData.WF_FrmWorkCheck[0];
    var isShowCheck = false;
    if (checkField == keyOfEn && checkParam.IsReadonly != "1") {
        if ($("#TB_" + keyOfEn).length != 0 && $("#TB_" + keyOfEn).val().indexOf("," + checkParam.FK_Node) == -1)
            $("#TB_" + keyOfEn).val($("#TB_" + keyOfEn).val() + "," + checkParam.FK_Node);
        isShowCheck = true;
    }

    var tracks = checkData.Tracks;
    var aths = checkData.Aths;
    var SignType = checkData.SignType; //签名的人员 No,SignType 列, SignType=0 不签名, 1=图片签名, 2=电子签名。
    if (tracks.length == 0)
        return "";
    var _Html = '<table style="width:100%">';

    /*
 * 如果一个人有两个意见，在一个节点上，就需要显示最后一个意见.
 * 创建一个 Hasthable. Key=操作员的ID (EmpFrom),  Val = MyPK 的值.
 * 
 *  第一遍遍历，获取到这个 ht 
 * 
 */
    var tracksArr = unique(tracks);
    for (var i = 0; i < tracksArr.length; i++) {
        var track = tracksArr[i];
        if ($("#TB_" + keyOfEn).length != 0 && $("#TB_" + keyOfEn).val().indexOf("," + track.NodeID) == -1)
            continue;
        _Html += WorkCheck_Parse(track, aths, frmWorkCheck, SignType, 0, isShowCheck, FWCVer);
        //var empNo = track.EmpFrom;
        /*if (ht.包含  key = EmpNo ) //如果ht 有这个empNo.
          ht.Add(empNo, track.MyPK);
        else*/

        /* if (i == 0) {
             map[empNo] = track.MyPk;
         }
         //如果遍历map
         for (var prop in map) {
             //map.hasOwnProperty(prop) && 
             if (prop == empNo)
                 continue;
 
             map[empNo] = track.MyPk;
             break;
         }
     }
     // 根据ht 输出数据。
     for (var i = 0; i < tracks.length; i++) {
         var track = tracks[i];
         if ($("#TB_" + keyOfEn).length != 0 && $("#TB_" + keyOfEn).val().indexOf("," + track.NodeID) == -1)
             continue;
         _Html += WorkCheck_Parse(track, aths, frmWorkCheck, SignType, 0, isShowCheck, FWCVer);
         var mypk = track.MyPk;
 
         *//* if (ht 包含 myp == false)
         continue;*//*
       // 如果遍历map
       for (var prop in map) {
           //map.hasOwnProperty(prop) && 
           if (map[prop] == mypk || (mypk == null && map[prop] == null)) {
               _Html += WorkCheck_Parse(track, aths, frmWorkCheck, SignType, 0, isShowCheck, FWCVer);
               break;
           } else {
               continue;
           }
       }*/
    }

    _Html += "</table>";
    return _Html;

}
function WorkCheck_Parse(track, aths, frmWorkCheck, SignType, showNodeName, isShowCheck, FWCVer) {
    var _Html = "";
    //解析节点上传的附件
    var subaths = GetSubAths(track.NodeID, aths);

    //仅显示自己的审核意见
    if (frmWorkCheck.FWCMsgShow == "1" && track.NodeID == checkParam.FK_Node && track.IsDoc == false) {
        return true;
    }

    //var fwcs = new Entities("BP.WF.Template.FrmWorkChecks");
    //fwcs.Retrieve("NodeID", this.NodeID);

    // if (fwcs[0].FWCSta == 2)
    //   return true;

    _Html += "<tr>";
    if (showNodeName == 1) {
        //显示审核节点的信息/有可能是会签节点
        _Html += "<td " + (track.IsDoc ? ("id='tdnode_" + track.NodeID + "'") : "") + " rowspan='" + (subaths.length > 0 ? 3 : 2) + "' style='width:20%;border:1px solid #D6DDE6;'>";
        var nodeName = track.NodeName;
        nodeName = nodeName.replace('(会签)', '<br>(<font color=Gray>会签</font>)');
        _Html += nodeName;
        _Html += "</td>";
    }

    var isEditWorkCheck = false;
    if (track.IsDoc == "1" && (checkParam.IsReadonly == null || checkParam.IsReadonly == false) && isShowCheck == true)
        isEditWorkCheck = true;

    //可编辑的审核意见
    if (isEditWorkCheck == true) {

        _Html += "<td style='width:80%;border-bottom-style:none;border-color:#ddd;display:table-cell;' class='only-print-hidden'>";

        //是否启用附件上传
        if (frmWorkCheck.FWCAth == 1) {
            _Html += "<div style='float:right' id='uploaddiv' data-info='" + frmWorkCheck.FWCShowModel + "' onmouseover='UploadFileChange(this)'></div>";
        }
        if ("undefined" == typeof IsShowWorkCheckUsefulExpres) {
            IsShowWorkCheckUsefulExpres = true;
        }
        if (IsShowWorkCheckUsefulExpres == true)
            _Html += "<div style='float:right'><a onmouseover = 'UsefulExpresFlow(\"WorkCheck\",\"WorkCheck_Doc\");' ><span style='font-size:15px;'>常用短语</span>  <img alt='编辑常用审批语言.' src='../WF/Img/Btn/Edit.gif' /></a></div>";

        _Html += "<div style='float:left;width:100%;'>";
        var msg = track.Msg;
        if (msg == null || msg == undefined || msg == "")
            msg = "";
        else
            msg = msg.replace(/<BR>/g, '\t\n');

        _Html += "<textarea id='WorkCheck_Doc' maxlength='2000' placeholder='内容不能为空,请输入信息,或者使用常用短语选择,内容不超过2000字.' rows='3' style='color:blue;width:98%;border-style:solid;margin:5px; padding:5px;' onblur='SaveWorkCheck(0)' onkeydown='this.style.height=\"60px\";this.style.height=this.scrollHeight+\"px\";'>";
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
                if (fwcView != "" && str[i] == fwcView)
                    check = "checked = checked";
                else if (fwcView == "" && idx == 0)
                    check = "checked = checked";
                _Html += "<input type='radio' id='RB_FWCView_" + idx + "' name ='RB_FWCView' " + check + " onclick='SaveWorkCheck(0)' value='" + idx + "'/>" + str[i] + "&nbsp;&nbsp;&nbsp;";
                idx++;
            }
        }
    }//只读的审核意见
    else {

        _Html += '<td style="word-wrap: break-word;line-height:20px;padding:5px;font-color:green;border-bottom-style:none;border-color:#ddd" >';
        //显示退回原因
        var returnMsg = track.ActionType == 2 ? "退回原因：" : "";
        if (FWCVer == 1) {
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
                if (fwcView != "" && str[i] == fwcView)
                    check = "checked = checked";
                else if (fwcView == "" && idx == 0)
                    check = "checked = checked";
                _Html += "<input type='radio' id='RB_FWCView_" + idx + "' name ='RB_FWCView' " + check + " onclick='SaveWorkCheck(0)' value='" + idx + "' disabled/>" + str[i] + "&nbsp;&nbsp;&nbsp;";
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
            _Html += GetUserHandWriting(track, isEditWorkCheck, track.EmpFromT, track.EmpFrom);
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


            if (st.SignType == 2) {

                _Html += "<tr>";
                _Html += "<td style='text-align:left;height:35px;line-height:35px;'>" + track.DeptName + "<div style='float:right'>签名:"
                    + GetUserHandWriting(track, isEditWorkCheck, track.EmpFromT, track.EmpFrom);
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



function SaveWorkCheck(type) {
    var ToNodeName=  $("#DDL_ToNode option:selected").text()
    if ($("#WorkCheck_Doc").length == 0 || ToNodeName == "旁签辅签") {//审核组件只读
        isCanSend = true;
        return;
    }
    var doc = $("#WorkCheck_Doc").val();
    if (doc == "" && frmWorkCheck.SigantureEnabel == "2" && writeImg == "") {
        alert("请填写审核意见");
        isCanSend = false;
        return;
    }
    doc = doc.replace(/'/g, '');
    if (doc == "" && type != 0 && frmWorkCheck.SigantureEnabel == "2" && writeImg == "") {
        alert("请点击签字版签名");
        isCanSend = false;
        return;
    }
    if (checkParam.IsReadonly == true) {
        isCanSend = true;
        return;
    }
    if (checkParam.WorkID == null || checkParam.WorkID == undefined || checkParam.WorkID == 0) {
        isCanSend = true;
        return;
    }



    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddJson(checkParam);
    handler.AddPara("HandlerName", GetQueryString("HttpHandlerName"));
    handler.AddPara("Doc", doc);
    if (writeImg == null && writeImg == undefined)
        writeImg = "";
    else
        writeImg = writeImg.replace(/[+]/g, "~");
    handler.AddPara("WriteImg", writeImg);
    if ($("input[name='RB_FWCView']").length != 0) {
        handler.AddPara("FWCView", $("input[name='RB_FWCView']:checked")[0].nextSibling.nodeValue);
    }
    var data = handler.DoMethodReturnString("WorkCheck_Save");

    if (data.indexOf('err@') != -1) {
        alert(data);
        isCanSend = false;
        return;
    }

    if (data.length > 0) {
        $("#rdt").text(data);
    }
    isCanSend = true;
    return;
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
    if (webUser && webUser.CCBPMRunModel == 2)
        handler = new HttpHandler("BP.Cloud.HttpHandler.App");

    handler.AddPara('No', userNo);
    data = handler.DoMethodReturnString("HasSealPic");

    //如果不存在，就显示当前人的姓名
    if (data.length > 0)
        return userName;


    if (webUser && webUser.CCBPMRunModel == 2)
        return "<img src='../../DataUser/Siganture/"+webUser.OrgNo+"/" + userNo + ".jpg?m=" + Math.random() + "' title='" + userName + "' " + func + " style='height:40px;' border=0 alt='" + userNo + "' />";
    else
        return "<img src='../../DataUser/Siganture/" + userNo + ".jpg?m=" + Math.random() + "' title='" + userName + "' " + func + " style='height:40px;' border=0 alt='" + userNo + "' />";

}


//签字版
function GetUserHandWriting(track, isEditWorkCheck, userName, userNo) {
    //debugger;
    if (isEditWorkCheck == false) {
        if (track.WritImg == null || track.WritImg == "")
            return userName;
        return "<img src='" + track.WritImg.replace(/' '/, '') + "'  style='height:40px;' border=0  />";
    }
    writeImg = track.WritImg;
    var src = track.WritImg;
    if (writeImg == "" || writeImg == null) {
        src = "Siganture\\" + userNo + ".jpg";
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddPara("src", src);
        var data = handler.DoMethodReturnString("ImageDatabytes");
        if (data != "" && data.indexOf("err@") == -1)
            writeImg = "data:image/png;base64," + data;
        //将二进制流存入缓存中
        window.localStorage.setItem("writeImg", writeImg);
    }
    return "<img id='Img_WorkCheck' src='" + writeImg + "' onclick='openHandWriting()' onerror=\"this.src='../DataUser/Siganture/UnSiganture.jpg'\"  style='border:0px;height:40px;'  />";
}

function openHandWriting() {
    var url = "CCForm/HandWriting.htm?WorkID=" + checkParam.WorkID + "&FK_Flow=" + checkParam.FK_Flow + "&FK_Node=" + checkParam.FK_Node + "&WritType=WorkCheck";
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
    $.each(aths, function() {
        if (this.NodeID == nodeID) {
            subAths.push(this);
        }
    });

    //2.解析上传的附件
    var _Html = '';
    $.each(subAths, function() {
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

    html += "<a style='color:Blue; font-size:14;' href='javaScript:void(0)' onclick=\"AthDown('" + ath.FK_FrmAttachment + "','" + checkParam.WorkID + "','" + ath.MyPK + "', '" + ath.FK_MapData + "')\">" + ath.FileName;
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
            url = basePath + '/WF/CCForm/Handler.ashx?AttachPK=ND' + checkParam.FK_Node + '_FrmWorkCheck&DoType=MoreAttach&FK_Flow=' + checkParam.FK_Flow + '&PKVal=' + workid + "&FK_Node=" + GetQueryString("FK_Node");
        else {
            var currentPath = window.document.location.href;
            var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
            url = path + "WF/Ath/AttachmentUpload.do?FK_FrmAttachment=ND" + checkParam.FK_Node + "_FrmWorkCheck&FK_Flow=" + checkParam.FK_Flow + "&PKVal=" + checkParam.WorkID + "&FK_Node=" + GetQueryString("FK_Node");
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
            'onDialogOpen': function(a, b) {
            },
            'onQueueComplete': function(queueData) {
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
        Url = dynamicHandler + "?DoType=HttpHandler&DoMethod=" + doMethod + "&HttpHandlerName=" + httpHandlerName + "&FK_FrmAttachment=" + AttachPK + "&WorkID=" + checkParam.WorkID + "&PKVal=" + checkParam.WorkID + "&AttachPK=" + AttachPK + "&FK_Node=" + GetQueryString("FK_Node") + "&parasData=" + parasData + "&t=" + new Date().getTime();
    else {
        var currentPath = window.document.location.href;
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        Url = path + "WF/Ath/AttachmentUploadS.do/?FK_FrmAttachment=" + AttachPK + "&PKVal=" + checkParam.WorkID + "&AttachPK=" + AttachPK + "&parasData=" + parasData;
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
        beforeSend: function() {
            console.log("正在进行，请稍候");
        },
        success: function(responseStr) {
            GetNewUploadedAths(fileObj, fwcShowModel)
        },
        error: function(responseStr) {
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

    var Names = "|" + files.name + "|";

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddPara("Names", Names);
    handler.AddPara("AttachPK", checkParam.FK_Node + "_FrmWorkCheck");
    handler.AddJson(checkParam);
    var data = handler.DoMethodReturnString("WorkCheck_GetNewUploadedAths");
    if (data.indexOf('err@') != -1) {
        alert(data);
        console.log(data);
        return;
    }
    var naths = eval('(' + data + ')');


    if ($("#aths_" + checkParam.FK_Node).length == 0) {
        if ($("#WorkCheck_Doc").length > 0) {
            var tdid = "id='aths_" + checkParam.FK_Node + "'";
            var html = "<tr><td " + tdid + " style='word-wrap: break-word;'>";
            html += "<b>附件：</b>&nbsp;";
            html += "</td></tr>";

            $("#WorkCheck_Doc").parent().parent().parent().after(html)
        }
    }

    if (fwcShowModel != 0) {
        $("#tdnode_" + checkParam.FK_Node).attr("rowspan", "3");
    }

    $("#aths_" + checkParam.FK_Node).parent().removeAttr("style");

    $.each(naths, function() {
        $("#aths_" + checkParam.FK_Node).append(GetAthHtml(this));
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
function unique(arr) {
    var tracksArr = arr;
    for (var i = 0, len = tracksArr.length; i < len; i++) {
        for (var j = i + 1, len = tracksArr.length; j < len; j++) {
            if (tracksArr[i].EmpFrom === tracksArr[j].EmpFrom) {
                tracksArr.splice(j, 1);
                j--;        // 每删除一个数j的值就减1
                len--;      // j值减小时len也要相应减1（减少循环次数，节省性能）   
                // console.log(j,len)
            }
        }
    }
    return tracksArr;
}




