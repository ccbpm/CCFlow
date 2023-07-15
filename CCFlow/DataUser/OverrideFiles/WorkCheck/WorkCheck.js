$(function () {
    if (frmWorkCheck.SigantureEnabel == 3 || frmWorkCheck.SigantureEnabel == 4 || frmWorkCheck.SigantureEnabel == 5) {
        loadStamp_Init();
    }
})
/**
 * 调用各项目各自电子签字代码
 */
function Siganture() {

}

/**
 * 动态加载签章JS及初始化数据
 */
function loadStamp_Init() {
    Skip.addJs("/DataUser/OverrideFiles/WorkCheck/qianzhang/static/js/bjca/BJCAWebSign.js?ver=" + Math.random());
    Skip.addJs("/DataUser/OverrideFiles/WorkCheck/qianzhang/static/js/public/jquery-1.11.3.min.js?ver=" + Math.random());
    Skip.addJs("/DataUser/OverrideFiles/WorkCheck/qianzhang/static/js/public/echarts/echarts.min.js?ver=" + Math.random());
    Skip.addJs("/DataUser/OverrideFiles/WorkCheck/qianzhang/static/js/public/drag.js?ver=" + Math.random());
    Skip.addJs("/DataUser/OverrideFiles/WorkCheck/qianzhang/static/js/public/jquery.jqprint-0.3.js?ver=" + Math.random());
    Skip.addJs("/DataUser/OverrideFiles/WorkCheck/qianzhang/static/js/doc/lab.js?ver=" + Math.random());
    BWS_Init();
    if (typeof datas !="undefined" && datas.length != 0) {
        showSign();
    }
}
/**
 * 自定义签名按钮
 * @param {any} track 当前审核的信息
 * @param {any} isEditWorkCheck 是否是可以编辑的审核信息
 * @param {any} idx 当前的顺序
 */
function GetSigantureSelf(track, isEditWorkCheck, idx) {
    if (isEditWorkCheck == false) {
        if (track.WritImg != null && track.WritImg != "" && track.WriteStamp != null && track.WriteStamp != "") {
            datas.push({
                WriteImg: track.WritImg.replace(/' '/, ''),
                Idx: idx
            })
            stampIdx++;
        }
        return "";
    }
    retHtml += "<div class='verifyedgif20' id = 'verifyedgif20' style = 'position: relative;' ></div ><a href='javascript:positionSign(20,0)'>盖章</a><div > <div style='display:none'><textarea id = 'signatureData20' name = 'signatureData20' style = 'FONT-SIZE: 12pt; WIDTH: '100%;' COLOR: '#000000;' FONT-FAMILY: '仿宋_GB2312; HEIGHT: 155px' rows = '5'  cols = '75' placeholder = '显示签章值区域' readonly = 'readonly' ></textarea > </div ></div > ";
    return retHtml;
}
/**
 * 自定义签章按钮
 * @param {any} track 当前审核的信息
 * @param {any} isEditWorkCheck 是否是可以编辑的审核信息
 * @param {any} idx 当前的顺序
 */
function GetStamp(track, isEditWorkCheck, idx) {
    if (isEditWorkCheck == false) {
        if (track.WritImg != null && track.WritImg != "" && track.WriteStamp != null && track.WriteStamp != "") {
            datas.push({
                WriteStamp: track.WriteStamp.replace(/' '/, ''),
                Idx: idx
            })
            stampIdx++;
        }
        return "";
    }
    var retHtml = "<div class='verifyedgif10' id='verifyedgif10' style='position: relative;'></div><a href='javascript:positionSign(10,0)'>签字</a> <div style='display:none'><textarea id = 'signatureData10' name = 'signatureData10' style = 'FONT-SIZE: 12pt; WIDTH: '100%;' COLOR: '#000000;' FONT-FAMILY: '仿宋_GB2312; HEIGHT: 155px' rows = '5'  cols = '75' placeholder = '显示签名值区域' readonly = 'readonly' display='none' ></textarea > </div ></div >";
    return retHtml;
}
/**
 * 签名+签章
 * @param {any} track 当前审核的信息
 * @param {any} isEditWorkCheck 是否是可以编辑的审核信息
 * @param {any} idx 当前的顺序
 */
function GetSigantureStampSelf(track, isEditWorkCheck, idx) {
    if (isEditWorkCheck == false) {
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
/**
 * 签章回显
 **/
function showSign() {

    debugger;
    var i = 0;
    var j = 0;

    var showSignInterval = setInterval(function () {
        //debugger;
        if ($_$WebSocketConnectState) {
            debugger;
            i++;
            if (i > datas.length) {
                clearInterval(showSignInterval);
                return
            }
            var _Idx = datas[i - 1].Idx;
            $_$CurOrgData[i] = document.getElementById("WorkCheck_Doc" + _Idx).innerHTML;//"同意";//
            $_$CurSignData[i] = datas[i - 1][0];
            var orgdata = $_$CurOrgData[i];
            var signdata = datas[i - 1].WriteImg//$_$CurSignData[i][0];
            BWS_Verify(orgdata, signdata, function (ret) {
                if (ret.retVal) {
                    j++;
                    //显示验证后的印章图片
                    ESeaL_CreateSignMenu(j);
                    $_$CurSignMethod[j] = signMethodsDatas[j - 1];
                    var temp = $ShowSignCallback(j, ret.retVal);
                    document.getElementById("verifyedgif1" + _Idx).innerHTML = temp;
                    //禁用印章div右键菜单
                    document.getElementById("verifyedgif1" + _Idx).oncontextmenu = function () {
                        return false;
                    }
                }
            }, false);

            var signdata2 = datas[i - 1].WriteStamp;// $_$CurSignData[i][1];
            BWS_Verify(orgdata, signdata2, function (ret) {
                if (ret.retVal) {
                    //j++;
                    //显示验证后的印章图片
                    ESeaL_CreateSignMenu(j);
                    $_$CurSignMethod[j] = signMethodsDatas[j - 1];
                    var temp = $ShowSignCallback(j, ret.retVal);
                    document.getElementById("verifyedgif2" + _Idx).innerHTML = temp;
                    //禁用印章div右键菜单
                    document.getElementById("verifyedgif2" + _Idx).oncontextmenu = function () {
                        return false;
                    }
                }
            }, false);
        }
    }, 800);
}


 /**************************通用的方法*******************************/
 



/**
 * 调用各项目各自盖章代码
 */
function positionSign(stampID, docidx) {
    positionSign(stampID, idx);
}
//第一步，点击签章按钮，根据平台是win还是Linux决定是否弹窗
function positionSign(stampID, docidx) {
    //debugger;
    $_$CurStampID = stampID;
    if ($_$is_windows) {
        positionSignFunc(stampID, docidx);
    } else {
        showLogin(positionSignFunc(stampID, docidx));
    }
}
//签章
function positionSignFunc(stampID, docidx) {

    var orgdata = document.getElementById("WorkCheck_Doc" + docidx).value;
    var strSealID = $_$CurSealID[$_$CurStampID];
    var strCertID = $_$CurCertID[$_$CurStampID];
    if (orgdata == "") {
        alert("请输入原文！");
        return;
    }
    if (!$_$is_windows) {
        if (strCertID == "") {
            alert("请选择证书！");
            return;
        }
        if (strSealID == "") {
            alert("请选择印章！");
            return;
        }
    }
    $_$CurOrgData[$_$CurStampID] = orgdata;

    // 右键菜单
    // @param stampID 印章DIV的id
    ESeaL_CreateSignMenu($_$CurStampID);
    BWS_DirectSign(strCertID, strSealID, orgdata, function (ret) {
        if (ret.signData) {
            //显示验证后的印章图片
            var retObj = {
                retVal: ret.signData,
                picData: ret.picData
            };
            var temp = $DirectSignCallback(retObj);
            document.getElementById("verifyedgif" + $_$CurStampID).innerHTML = temp;
            //禁用印章div右键菜单
            document.getElementById("verifyedgif" + $_$CurStampID).oncontextmenu = function () {
                return false;
            }
            // console.log("done");
        }
    });

    $("#qm" + $_$CurStampID).css("display", "none");
}

//验章
function btnVerify(stampID) {
    // var orgdata = $_$CurOrgData[stampID];
    var orgdata = document.getElementById("originData1").value;
    $_$CurOrgData[stampID] = orgdata;
    var signdata = $_$CurSignData[stampID];
    if (signdata == "") {
        alert("请先签章！");
        return;
    }
    if (orgdata == "") {
        alert("原文不能为空！");
        return;
    }
    $_$CurStampID = stampID;
    BWS_Verify(orgdata, signdata, function (ret) {
        if (ret.retVal) {
            //显示验证后的印章图片
            var retObj = { retVal: ret.retVal };
            $VerifyCallback(retObj);
        }
    }, true);
}