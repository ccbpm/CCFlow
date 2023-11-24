
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

var writeImg = "";
//是否只读?
function IsReadOnly() {
    //如果是MyFlowView 或者是MyCC 就把该控件设置为只读的.
    var url = GetHrefUrl();
    if (url.indexOf('MyViewGener') != -1 || url.indexOf('MyCC') != -1 || url.indexOf('MyFrm') != -1) {
        return 1;
    }
    var val = GetQueryString("IsReadOnly") != null && GetQueryString("IsReadOnly") != undefined && GetQueryString("IsReadOnly") == "1" ? true : false;
    return val;
}


//审核组件页面初始化
function NodeWorkCheck_Init() {
    if ($("#WorkCheck").length == 0) {
        $("#Group_FWC").hide();
        return;
    }
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
    var SignType = checkData.SignType; //签名的人员 No,SignType 列, SignType=0 不签名, 1=图片签名, 2=电子签名。

    var _Html = '';

    if (tracks.length == 0) {
        $("#Group_FWC").hide();
        $("#WorkCheck_Group").hide();
    }

    //轨迹数据
    if (tracks.length != 0) {
        _Html += '<table style="width:100%">';
        $.each(tracks, function (idx, item) {
            if (frmWorkCheck.SigantureEnabel == 3 || frmWorkCheck.SigantureEnabel == 4 || frmWorkCheck.SigantureEnabel == 5)
                _Html += WorkCheck_Stamp_Parse(item, frmWorkCheck, 1, true, FWCVer, idx);
            else
                _Html += WorkCheck_Parse(item, frmWorkCheck, SignType, 1, true, FWCVer);

        });
        _Html += "</table>";
    }



    $("#WorkCheck").html(_Html);
    if ($("#WorkCheck_Doc").length > 0) {
        if (frmWorkCheck.FWCIsFullInfo == 1 && frmWorkCheck.FWCDefInfo && frmWorkCheck.FWCDefInfo.length > 0) {
            SaveWorkCheck(0);
        }
    }
    if ($("#uploaddiv").length > 0) {
        /*var explorer = window.navigator.userAgent;
        if (((explorer.indexOf('MSIE') >= 0) && (explorer.indexOf('Opera') < 0) || (explorer.indexOf('Trident') >= 0)))
            AddUploadify("uploaddiv", frmWorkCheck.FWCShowModel);
        else*/
        AddUploafFileHtm("uploaddiv", frmWorkCheck.FWCShowModel);
    }
};

function WorkCheck_Init(FWCVer) {
    var data;
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddUrlData();

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
    var data = JSON.parse(data);
    frmWorkCheck = data.WF_FrmWorkCheck[0];
    if (typeof ccbpmPath == "undefined")
        ccbpmPath = basePath;
    loadScript(ccbpmPath + "/DataUser/OverrideFiles/WorkCheck/WorkCheck.js?t=" + Math.random());
    return data;

}
var map = {}; //求出来要输出的1 人1意见的最后数据.
//签批组件专用方法.
function GetWorkCheck_Node(checkData, keyOfEn, checkField, FWCVer) {
    //当前节点审核组件信息
    frmWorkCheck = checkData.WF_FrmWorkCheck[0];
    var isShowCheck = false;
    if (checkField == keyOfEn && checkParam.IsReadonly != "1") {
        if ($("#TB_" + keyOfEn).length != 0 && $("#TB_" + keyOfEn).val().indexOf("," + checkParam.FK_Node) == -1)
            $("#TB_" + keyOfEn).val($("#TB_" + keyOfEn).val() + "," + checkParam.FK_Node);
        isShowCheck = true;
    }

    var tracks = checkData.Tracks;
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
        if (frmWorkCheck.SigantureEnabel == 3 || frmWorkCheck.SigantureEnabel == 4 || frmWorkCheck.SigantureEnabel == 5)
            _Html += WorkCheck_Stamp_Parse(track, frmWorkCheck, 0, isShowCheck, FWCVer, i);
        else
            _Html += WorkCheck_Parse(track, frmWorkCheck, SignType, 0, isShowCheck, FWCVer);
    }

    _Html += "</table>";
    return _Html;

}
function WorkCheck_Parse(track, frmWorkCheck, SignType, showNodeName, isShowCheck, FWCVer) {

    //解析节点上传的附件
    var subaths = GetSubAths(track.NodeID, frmWorkCheck);

    //仅显示自己的审核意见
    if (frmWorkCheck.FWCMsgShow == "1" && track.NodeID == checkParam.FK_Node && track.IsDoc == false) {
        return true;
    }

    var isEditWorkCheck = false;
    if (track.IsDoc == "1" && (checkParam.IsReadonly == null || checkParam.IsReadonly == false) && isShowCheck == true)
        isEditWorkCheck = true;

    var _Html = "<tr>";
    if (isEditWorkCheck == false && getConfigByKey("IsShowComplteCheckIcon", false) == true)
        _Html += "<td  style='border: 1px solid #D6DDE6;background-image: url(Img/banjie.png);background-repeat: no-repeat;background-position:center;background-size:75px'>";
    else
        _Html += "<td  style='border: 1px solid #D6DDE6;'>";

    if (showNodeName == 1) {
        //显示审核节点的信息/有可能是会签节点
        _Html += "<div " + (track.IsDoc ? ("id='tdnode_" + track.NodeID + "'") : "") + " style='font-weight:bold'>";
        var nodeName = track.NodeName;
        nodeName = nodeName.replace('(会签)', '<br>(<font color=Gray>会签</font>)');
        _Html += nodeName;
        _Html += "</div>";
    }
    //_Html += "<div style='border: 1px solid #D6DDE6;border-bottom: none;border-top: none;'>";




    //可编辑的审核意见
    if (isEditWorkCheck == true) {

        _Html += "<div class=''>";

        //是否启用附件上传
        if (frmWorkCheck.FWCAth == 1) {
            _Html += "<div style='float:right' id='uploaddiv' data-info='" + frmWorkCheck.FWCShowModel + "' onmouseover='UploadFileChange(this)'></div>";
        }


        _Html += "<div style='width:100%;'>";
        var msg = track.Msg;
        if (msg == null || msg == undefined || msg == "")
            msg = "";
        else
            msg = msg.replace(/<BR>/g, '\t\n');

        _Html += "<textarea id='WorkCheck_Doc' maxlength='2000' placeholder='内容不能为空,请输入信息,或者使用常用短语选择,内容不超过2000字.' rows='3' style='color:blue;width:98%;border-style:solid;margin:5px; padding:5px;' onblur='SaveWorkCheck(0)'>";
        _Html += msg;
        _Html += "</textarea>";
        _Html += "<br>";

        _Html += "</div>";
        _Html += "</div>";

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

        _Html += '<div style="word-wrap: break-word;line-height:20px;padding:5px;padding-left:50px;" >';
        //显示退回原因
        var returnMsg = (track.ActionType == 2 || track.ActionType == 201) ? "退回原因：" : "";
        if (FWCVer == 1) {
            var val = track.Msg.split("WorkCheck@");
            if (val.length == 2)
                track.Msg = val[1];
        }
        _Html += "<font color='#999'>" + returnMsg + track.Msg + "</font>";
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
        _Html += '</div>';
    }
    //_Html += '</td>';
    //_Html += '</tr>';



    //输出签名,没有签名的要求.
    if (SignType == null || SignType == undefined) {

        //签名，日期.
        //_Html += "<tr style='border: 1px solid #D6DDE6;border-top: none;'>";
        if (track.RDT == "")
            _Html += "<div style='text-align:right;width:100%;padding-right:5px' class=''>";
        else
            _Html += "<div style='text-align:right;padding-right:5px'>";
        if (isEditWorkCheck == true && getConfigByKey("IsShowWorkCheckUsefulExpres", true) == true)
            _Html += "<div style='float:left'><a onclick = 'UsefulExpresFlow(\"WorkCheck\",\"WorkCheck_Doc\");' ><span style='font-size:15px;'>常用短语</span>  <img alt='编辑常用审批语言.' src='" + basePath + "/WF/Img/Btn/Edit.gif' /></a></div>";

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
        _Html += "</div>";

        //_Html += "</tr>";

    } else {

        for (var idx = 0; idx < SignType.length; idx++) {

            var st = SignType[idx];
            if (st.No != track.EmpFrom)
                continue;

            var rdt = track.RDT.substring(0, 16);


            if (st.SignType == 0 || st.SignType == 2 || st.SignType == null) {

                // _Html += "<tr style='border: 1px solid #D6DDE6;border-top: none;'>";
                _Html += "<div style='text-align:left;height:35px;line-height:35px;'>" + track.DeptName + "<div style='float:right'>签名:"
                    + track.EmpFromT
                    + "日期:" + (track.IsDoc ? "<span id='rdt'>" : "") + rdt + (track.IsDoc ? "</span>" : "") + "</div></div>";
                //_Html += "</tr>";
                break;
            }
            //图片签名
            if (st.SignType == 1) {
                //_Html += "<tr>";
                _Html += "<div style='text-align:left;height:35px;line-height:35px;'>" + track.DeptName + "<div style='float:right'>签名:"
                    + GetUserSiganture(track.EmpFrom, track.EmpFromT)
                    + "日期:" + (track.IsDoc ? "<span id='rdt'>" : "") + rdt + (track.IsDoc ? "</span>" : "") + "</div></div>";
                // _Html += "</tr>";
                break;
            }


            if (st.SignType == 2) {

                // _Html += "<tr>";
                _Html += "<div style='text-align:left;height:35px;line-height:35px;'>" + track.DeptName + "<div style='float:right'>签名:"
                    + GetUserHandWriting(track, isEditWorkCheck, track.EmpFromT, track.EmpFrom);
                + "日期:" + (track.IsDoc ? "<span id='rdt'>" : "") + rdt + (track.IsDoc ? "</span>" : "") + "</div></div>";
                //_Html += "</tr>";
                //  alert('电子签名的逻辑尚未编写.');
                break;
            }

            //如果是图片密码签名.
            if (st.SignType == 3) {

                isCanSend = false; //设置不可以发送.
                //_Html += "<tr>";
                _Html += "<div style='text-align:left;height:35px;line-height:35px;'>" + track.DeptName + "<div style='float:right'>签名:";

                _Html += "<a href='WorkCheck_CheckPass();'>请输入签名</a>";

                //_Html += "</div>";

                _Html += +"日期:" + (track.IsDoc ? "<span id='rdt'>" : "") + rdt + (track.IsDoc ? "</span>" : "") + "</div></div>";
                // _Html += "</tr>";
                break;
            }
        }
    }
    _Html += "</td>";
    _Html += "</tr>";

    //附件
    if (subaths.length > 0) {
        var tdid = track.IsDoc ? ("id='aths_" + track.NodeID + "'") : "";

        _Html += "<tr style='" + (subaths.length > 0 ? "" : "display:none;") + "'>";
        _Html += "<td " + tdid + " style='word-wrap: break-word;' colspan=2>";
        _Html += "<b>附件：</b>&nbsp;" + subaths;
        _Html += "</td>";
        _Html += "</tr>";
    }
    return _Html;
}
function WorkCheck_Stamp_Parse(track, frmWorkCheck, showNodeName, isShowCheck, FWCVer, idx) {

    idx = parseInt(idx) + 1;
    //解析节点上传的附件
    var subaths = GetSubAths(track.NodeID, frmWorkCheck);

    //仅显示自己的审核意见
    if (frmWorkCheck.FWCMsgShow == "1" && track.NodeID == checkParam.FK_Node && track.IsDoc == false) {
        return true;
    }

    var isEditWorkCheck = false;
    if (track.IsDoc == "1" && (checkParam.IsReadonly == null || checkParam.IsReadonly == false) && isShowCheck == true)
        isEditWorkCheck = true;

    var _Html = "<tr>";
    if (isEditWorkCheck == false && getConfigByKey("IsShowComplteCheckIcon", false) == true)
        _Html += "<td  style='border: 1px solid #D6DDE6;background-image: url(Img/banjie.png);background-repeat: no-repeat;background-position:center;background-size:75px'>";
    else
        _Html += "<td  style='border: 1px solid #D6DDE6;'>";

    if (showNodeName == 1) {
        //显示审核节点的信息/有可能是会签节点
        _Html += "<div " + (track.IsDoc ? ("id='tdnode_" + track.NodeID + "'") : "") + " style='font-weight:bold'>";
        var nodeName = track.NodeName;
        nodeName = nodeName.replace('(会签)', '<br>(<font color=Gray>会签</font>)');
        _Html += nodeName;
        _Html += "</div>";
    }
    //_Html += "<div style='border: 1px solid #D6DDE6;border-bottom: none;border-top: none;'>";




    //可编辑的审核意见
    if (isEditWorkCheck == true) {

        _Html += "<div class=''>";

        //是否启用附件上传
        if (frmWorkCheck.FWCAth == 1) {
            _Html += "<div style='float:right' id='uploaddiv' data-info='" + frmWorkCheck.FWCShowModel + "' onmouseover='UploadFileChange(this)'></div>";
        }


        _Html += "<div style='width:100%;'>";
        var msg = track.Msg;
        if (msg == null || msg == undefined || msg == "")
            msg = "";
        else
            msg = msg.replace(/<BR>/g, '\t\n');

        _Html += "<textarea id='WorkCheck_Doc0' name='WorkCheck_Doc'  maxlength='2000' placeholder='内容不能为空,请输入信息,或者使用常用短语选择,内容不超过2000字.' rows='3' style='color:blue;width:98%;border-style:solid;margin:5px; padding:5px;' onblur='SaveWorkCheck(0)'>";
        _Html += msg;
        _Html += "</textarea>";
        _Html += "<br>";

        _Html += "</div>";
        _Html += "</div>";

    }//只读的审核意见
    else {

        _Html += '<div style="word-wrap: break-word;line-height:20px;padding:5px;padding-left:50px;" >';
        //显示退回原因
        var returnMsg = track.ActionType == 2 ? "退回原因：" : "";
        if (FWCVer == 1) {
            var val = track.Msg.split("WorkCheck@");
            if (val.length == 2)
                track.Msg = val[1];
        }
        _Html += "<font color='#999'><div id='WorkCheck_Doc" + idx + "' name='WorkCheck_Doc'>" + returnMsg + track.Msg + "</div></font><div class='verifyedgif1" + idx + "' id='verifyedgif1" + idx + "' style='position: relative;'></div><div class='verifyedgif2" + idx + "' id='verifyedgif2" + idx + "' style='position: relative;'></div>";

        _Html += '</div>';
    }
    //_Html += '</td>';
    //_Html += '</tr>';



    //输出签名,没有签名的要求.

    //签名，日期.
    //_Html += "<tr style='border: 1px solid #D6DDE6;border-top: none;'>";
    if (track.RDT == "")
        _Html += "<div style='text-align:right;width:100%;padding-right:5px' class=''>";
    else
        _Html += "<div style='text-align:right;padding-right:5px'>";
    if (isEditWorkCheck == true && getConfigByKey("IsShowWorkCheckUsefulExpres", true) == true)
        _Html += "<div style='float:left'><a onmouseover = 'UsefulExpresFlow(\"WorkCheck\",\"WorkCheck_Doc\");' ><span style='font-size:15px;'>常用短语</span>  <img alt='编辑常用审批语言.' src='../WF/Img/Btn/Edit.gif' /></a></div>";
    //debugger

    //电子签名
    if (frmWorkCheck.SigantureEnabel == "3")
        _Html += GetSiganture(idx);
    //盖章
    else if (frmWorkCheck.SigantureEnabel == "4")
        _Html += GetStamp(idx);
    //电子签名+盖章
    else if (frmWorkCheck.SigantureEnabel == "5")
        _Html += GetSigantureStamp(track, isEditWorkCheck, idx);
    var rdt = track.RDT.substring(0, 16);
    if (rdt == "") {
        var dt = new Date();
        rdt = dt.getFullYear() + "-" + (dt.getMonth() + 1) + "-" + dt.getDate();  // new Date().toString("yyyy-MM-dd HH:mm");
    }
    _Html += "(" + rdt + ")";
    _Html += "</div>";

    //_Html += "</tr>";


    _Html += "</td>";
    _Html += "</tr>";
    //附件
    if (subaths.length > 0) {
        var tdid = track.IsDoc ? ("id='aths_" + track.NodeID + "'") : "";

        _Html += "<tr style='" + (subaths.length > 0 ? "" : "display:none;") + "'>";
        _Html += "<td " + tdid + " style='word-wrap: break-word;' colspan=2>";
        _Html += "<b>附件：</b>&nbsp;" + subaths;
        _Html += "</td>";
        _Html += "</tr>";
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



function SetDocVal() {

    var objS = document.getElementById("DuanYu");
    var val = objS.options[objS.selectedIndex].value;

    if (val == "")
        return;
    if ($("#WorkCheck_Doc").length == 1) {
        $("#WorkCheck_Doc").val(val);
    }


}


/**
 * 保存审核组件
 * @param {any} type
 */
function SaveWorkCheck(type) {
    if ($("#WorkCheck_Doc").length == 0) {
        if ($("#WorkCheck_Doc0").length == 0) {
            isCanSend = true;
            return;
        }
    }

    var doc = "";
    if ($("#WorkCheck_Doc").length == 1)
        doc = $("#WorkCheck_Doc").val();
    if ($("#WorkCheck_Doc0").length == 1)
        doc = $("#WorkCheck_Doc0").val();
    if (doc == "" && frmWorkCheck.SigantureEnabel == "2" && writeImg == "") {
        alert("请填写审核意见");
        isCanSend = false;
        return;
    }
    var signatureData1 = "";
    if (frmWorkCheck.SigantureEnabel == "4" || frmWorkCheck.SigantureEnabel == "5") {
        signatureData1 = $("#signatureData10").val();
        if (signatureData1 == "" && type != 0) {
            alert("请点击盖章按钮进行盖章");
            isCanSend = false;
            return;
        }
    }
    var signatureData2 = "";
    if (frmWorkCheck.SigantureEnabel == "3" || frmWorkCheck.SigantureEnabel == "5") {
        signatureData2 = $("#signatureData20").val();
        if (signatureData2 == "" && type != 0) {
            alert("请点击签名按钮进行签名");
            isCanSend = false;
            return;
        }
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
    if (signatureData1 == null && signatureData1 == undefined)
        signatureData1 = "";
    else
        signatureData1 = encodeURIComponent(signatureData1);//signatureData1.replace(/[+]/g, "~");
    handler.AddPara("WriteStamp", signatureData1);
    if (frmWorkCheck.SigantureEnabel == "2") {
        writeImg = writeImg || "";
        handler.AddPara("WriteImg", encodeURIComponent(writeImg));
    }
    if (frmWorkCheck.SigantureEnabel == "3" || frmWorkCheck.SigantureEnabel == "5") {
        signatureData2 = signatureData2 || "";
        signatureData2 = encodeURIComponent(signatureData2);// signatureData2.replace(/[+]/g, "~");

        handler.AddPara("WriteImg", signatureData2);
    }

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
        SetHref(basePath + "/WF/Comm/ProcessRequest?DoType=HttpHandler&DoMethod=AttachmentUpload_Down&HttpHandlerName=BP.WF.HttpHandler.WF_CCForm&WorkID=" + pkVal + "&FK_Node=" + fk_ath.replace("_FrmWorkCheck", "") + "&MyPK=" + delPKVal);


        return;
    }

    var currentPath = GetHrefUrl();
    var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
    Url = path + 'WF/Ath/downLoad.do?DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=' + fk_node + '&FK_Flow=' + fk_flow + '&FK_MapData=' + fk_mapData + '&Ath=' + ath;
    SetHref(url);
}



function UploadFileChange(ctrl) {
    $(ctrl).unbind("onblur");
    isChange = false;
}

function GetSiganture() {
    return "<a href='javascirpt:Siganture()'>签字</a>";
}
function GetStamp() {
    return "<a href='javascirpt:Stamp()'>盖章</a>";
}
function GetSigantureStamp() {
    return "<a href='javascirpt:Siganture()'>签字</a>   <a href='javascirpt:Stamp()'>盖章</a>";
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
        return "<img src='../../DataUser/Siganture/" + webUser.OrgNo + "/" + userNo + ".jpg?m=" + Math.random() + "' title='" + userName + "' " + func + " style='height:40px;' border=0 alt='" + userNo + "' />";
    else
        return "<img src='../../DataUser/Siganture/" + userNo + ".jpg?m=" + Math.random() + "' title='" + userName + "' " + func + " style='height:40px;' border=0 alt='" + userNo + "' />";

}


//签字版
function GetUserHandWriting(track, isEditWorkCheck, userName, userNo) {
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
    return "<img id='Img_WorkCheck' src='" + writeImg + "' onclick='openHandWriting()' onerror=\"this.src='../DataUser/Siganture/Siganture.jpg'\"  style='border:0px;height:40px;'  />";
}
/**
 * 电子签名
 * @param {any} track
 * @param {any} isEditWorkCheck
 * @param {any} idx
 */
function GetSiganture(track, isEditWorkCheck, idx) {
    if (typeof GetSigantureSelf != "undefined" && typeof GetSigantureSelf == "function")
        return GetSigantureSelf();
    var retHtml = "<div class='verifyedgif2' id='verifyedgif2' style='position: relative;'></div><a href='javascript:positionSign(2)'>签字</a> <textarea id = 'signatureData2' name = 'signatureData2' style = 'FONT-SIZE: 12pt; WIDTH: '100%;' COLOR: '#000000;' FONT-FAMILY: '仿宋_GB2312; HEIGHT: 155px' rows = '5'  cols = '75' placeholder = '显示签名值区域' readonly = 'readonly' ></textarea > </div >";
    return retHtml;
}
/**
 * 电子签章
 * @param {any} track
 * @param {any} isEditWorkCheck
 * @param {any} idx
 */
function GetStamp(track, isEditWorkCheck, idx) {
    if (typeof GetStampSelf != "undefined" && typeof GetStampSelf == "function")
        return GetStampSelf();
    var retHtml = "<div class='verifyedgif1' id = 'verifyedgif1' style = 'position: relative;' ></div ><a href='javascript:positionSign(1)'>盖章</a><div > <textarea id = 'signatureData1' name = 'signatureData1' style = 'FONT-SIZE: 12pt; WIDTH: '100%;' COLOR: '#000000;' FONT-FAMILY: '仿宋_GB2312; HEIGHT: 155px' rows = '5'  cols = '75' placeholder = '显示签章值区域' readonly = 'readonly' ></textarea > </div > ";
    return retHtml;
}
var stampIdx = 0
/**
 * 电子签名+签章
 * @param {any} track
 * @param {any} isEditWorkCheck
 * @param {any} idx
 */
function GetSigantureStamp(track, isEditWorkCheck, idx) {
    if (typeof GetSigantureStampSelf != "undefined" && typeof GetSigantureStampSelf == "function")
        return GetSigantureStampSelf(track, isEditWorkCheck, idx);

    if (isEditWorkCheck == false) {
        var html = "";

        if (track.WritImg != null && track.WritImg != "" && track.WriteStamp != null && track.WriteStamp != "") {
            datas.push({
                WriteImg: track.WritImg.replace(/' '/, ''),
                WriteStamp: track.WriteStamp.replace(/' '/, ''),
                Idx: idx
            })
            stampIdx++;
        }
        return "";
    }


    var retHtml = "<div class='verifyedgif10' id='verifyedgif10' style='position: relative;'></div><a href='javascript:positionSign(10,0)'>签字</a> <div style='display:none'><textarea id = 'signatureData10' name = 'signatureData10' style = 'FONT-SIZE: 12pt; WIDTH: '100%;' COLOR: '#000000;' FONT-FAMILY: '仿宋_GB2312; HEIGHT: 155px' rows = '5'  cols = '75' placeholder = '显示签名值区域' readonly = 'readonly' display='none' ></textarea > </div ></div >";
    retHtml += "<div class='verifyedgif20' id = 'verifyedgif20' style = 'position: relative;' ></div ><a href='javascript:positionSign(20,0)'>盖章</a><div > <div style='display:none'><textarea id = 'signatureData20' name = 'signatureData20' style = 'FONT-SIZE: 12pt; WIDTH: '100%;' COLOR: '#000000;' FONT-FAMILY: '仿宋_GB2312; HEIGHT: 155px' rows = '5'  cols = '75' placeholder = '显示签章值区域' readonly = 'readonly' ></textarea > </div ></div > ";
    return retHtml;
}


function openHandWriting() {
    var url = "CCForm/HandWriting.htm?WorkID=" + checkParam.WorkID + "&FK_Flow=" + checkParam.FK_Flow + "&FK_Node=" + checkParam.FK_Node + "&WritType=WorkCheck";
    OpenLayuiDialog(url, '签字板', 400, 60, "auto", false);
}

/**
 * 获取该节点上传的附件
 * @param {any} nodeID 当前节点ID
 * @param {any} aths 审核组件上传的附件
 */
function GetSubAths(nodeID, frmWorkCheck) {

    //1.获取节点上传的附件
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddPara("WorkID", checkParam.WorkID);
    handler.AddPara("FK_Node", nodeID);
    var data = handler.DoMethodReturnString("WorkCheck_GetNewUploadedAths");
    if (data.indexOf('err@') != -1) {
        alert(data);
        console.log(data);
        return;
    }
    var naths = cceval('(' + data + ')');

    //2.解析上传的附件
    var _Html = '';
    debugger
    $.each(naths, function () {
        if (frmWorkCheck.FWCMsgShow == "1") {
            if (this.Rec === webUser.No)
                _Html += GetAthHtml(this);
        }
        else
            _Html += GetAthHtml(this);
    });

    return _Html;
}

function GetAthHtml(ath) {
    var html = "<div id='Ath_" + ath.MyPK + "' style='margin:5px; display:inline-block;'>";
    var src = './';
    if (GetHrefUrl().indexOf("/CCForm") != -1)
        src = '../';
    if (ath.CanDelete == "1" || ath.CanDelete == true) {
        html += "<img alt='删除' align='middle' src='" + src + "Img/Btn/Delete.gif' onclick=\"DelWorkCheckAth('" + ath.MyPK + "')\" />&nbsp;&nbsp;";
    }

    html += "<a style='color:Blue; font-size:14;' href='javaScript:void(0)' onclick=\"AthDown('" + ath.FK_FrmAttachment + "','" + checkParam.WorkID + "','" + ath.MyPK + "', '" + ath.FK_MapData + "')\">" + ath.FileName;
    html += "&nbsp;&nbsp;<img src='" + src + "Img/FileType/" + ath.FileExts + ".gif' border=0 onerror=\"src='" + src + "Img/FileType/Undefined.gif'\" /></a>";
    html += "&nbsp;&nbsp;</div>";

    return html;
}


/*function AddUploadify(divid, fwcShowModel) {
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
            var currentPath = GetHrefUrl();
            var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
            url = path + "WF/Ath/AttachmentUpload.do?FK_FrmAttachment=ND" + checkParam.FK_Node + "_FrmWorkCheck&FK_Flow=" + checkParam.FK_Flow + "&PKVal=" + checkParam.WorkID + "&FK_Node=" + GetQueryString("FK_Node");
        }


        $('#file_upload').uploadify({
            'swf': '../Scripts/fileupload/uploadify.swf',
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
}*/

function AddUploafFileHtm(divid, fwcShowModel) {
    if ($("#file_upload").length == 0) {
        var html = "<div id='s' style='float:right;margin-right:10px;margin-top:5px;' >"
            + "<label id='realBtn' class='btn btn-info' style=''><input type='file' name='file' id='file' style='display:inline;left:-9999px;position:absolute;' onchange='UploadChange(" + fwcShowModel + ");' ><span><i class='layui-icon'>&#xe67c;</i>文件上传</span></label>"
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
        Url = dynamicHandler + "?DoType=HttpHandler&DoMethod=" + doMethod + "&HttpHandlerName=" + httpHandlerName + "&FK_FrmAttachment=" + AttachPK + "&WorkID=" + checkParam.WorkID + "&PKVal=" + checkParam.WorkID + "&AttachPK=" + AttachPK + "&FK_Node=" + GetQueryString("FK_Node") + "&t=" + new Date().getTime();
    else {
        var currentPath = GetHrefUrl();
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        Url = path + "WF/Ath/AttachmentUploadS.do/?FK_FrmAttachment=" + AttachPK + "&PKVal=" + checkParam.WorkID + "&AttachPK=" + AttachPK;
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
        dataType: 'html',
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
    var naths = cceval('(' + data + ')');


    if ($("#aths_" + checkParam.FK_Node).length == 0) {
        if ($("#WorkCheck_Doc").length > 0) {
            var tdid = "id='aths_" + checkParam.FK_Node + "'";
            var html = "<tr><td " + tdid + " style='word-wrap: break-word;'>";
            html += "<b>附件：</b>&nbsp;";
            html += "</td></tr>";

            $("#WorkCheck_Doc").parent().parent().parent().parent().after(html)
        }
    }

    if (fwcShowModel != 0) {
        $("#tdnode_" + checkParam.FK_Node).attr("rowspan", "3");
    }

    $("#aths_" + checkParam.FK_Node).parent().removeAttr("style");

    $.each(naths, function () {
        if (Names.toLowerCase().indexOf("|" + this.FileName.toLowerCase() + "|") == -1)
            return true;
        $("#aths_" + checkParam.FK_Node).append(GetAthHtml(this));
    });

}

//当用户点击签名图片的时候，弹出窗体让其输入密码.
function WorkCheck_CheckPass() {

    var pass = promptGener('请输入签名密码，初始化密码为123，您可以修改该密码.', "");
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
            if (tracksArr[i].EmpFrom === tracksArr[j].EmpFrom
                && tracksArr[i].NodeID === tracksArr[j].NodeID) {
                tracksArr.splice(i, 1);
                j--; // 每删除一个数j的值就减1
                len--; // j值减小时len也要相应减1（减少循环次数，节省性能）
            }
        }
    }
    return tracksArr;
}

/**加载完后的事件 */
window.onload = function () {
    if (frmWorkCheck && (frmWorkCheck.SigantureEnabel == 3 || frmWorkCheck.SigantureEnabel == 4 || frmWorkCheck.SigantureEnabel == 5)) {
        if (typeof loadStamp_Init == "function")
            loadStamp_Init();
    }

}




