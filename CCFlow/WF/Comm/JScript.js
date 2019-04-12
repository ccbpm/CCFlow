/* Table 特效风格 */
function TROver(ctrl) {
    ctrl.style.backgroundColor = 'LightSteelBlue';
}

function RefMethod1(path, index, warning, target, ensName, keys) {
    if (warning == null || warning == '') {
    }
    else {
        if (confirm(warning) == false)
            return false;
    }

    var url = "../Comm/RefMethod.htm?Index=" + index + "&EnsName=" + ensName + keys;
    if (target == null || target == '')
        var b = WinOpen(url, 'remmed');
    else
        var a = WinOpen(url, target);
    return true;
}
function TROut(ctrl) {
    ctrl.style.backgroundColor = 'white';
    ctrl.style.borderWidth = '1px';
    ctrl.style.borderstyle = 'none none dotted none';
}

var s = null;
function TBOnfocus(ctrl) {
    //  background-color: #f4f4f4;
    s = ctrl.className;
    ctrl.style.borderColor = 'LightSteelBlue';
}

function TBOnblur(ctrl) {
    // ctrl.style.borderColor = 'white';
    ctrl.className = s;
}

/* 默认植问题 */
function OpenHelperTBNo(appPath, EnsName, ctl) {
    var url = appPath + '/Comm/DataHelp.htm?' + appPath + '/Comm/HelperOfTBNo.aspx?EnsName=' + EnsName;
    var str = window.showModalDialog(url, '', 'dialogHeight: 550px; dialogWidth:950px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no');
    if (str == undefined)
        return;
    if (str == null)
        return;
    ctl.value = str;
    return;
}
function To(url) {
    window.location.href = url;
}

//window.onerror = function () {
//    return true;
//}

function OpenItme3(webAppPath, className, url) {
    var url = webAppPath + "/Comm/" + 'Item3.aspx?EnName=' + className + url;
    var newWindow = window.open(url, 'card', 'width=700,top=50,left=50,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false');
    newWindow.focus();
    return;
}

function WinShowModalDialog(url, winName) {
    var v = window.showModalDialog(url, winName, 'scrollbars=yes;resizable=yes;center=yes;minimize:yes;maximize:yes;dialogHeight: 650px; dialogWidth: 850px; dialogTop: 100px; dialogLeft: 150px;');
    return;
}


function WinShowModalDialog(url, winName, w, h) {
    var explorer = window.navigator.userAgent;
    if (explorer.indexOf("Chrome") >= 0) {//谷歌的
        window.open(url, "sd", "scrollbars=yes,resizable=yes,center=yes,minimize=yes,maximize=yes,height= 600px,width= 550px, top=50px, left= 650px");
    }
    else {//IE,火狐
        var v = window.showModalDialog(url, '', 'scrollbars=yes;resizable=yes;center=yes;minimize:yes;maximize:yes;dialogHeight: 650px; dialogWidth: 550px; dialogTop: 50px; dialogLeft: 650px;');
    }
    return;
}
function WinShowModalDialog_Accepter(url) {//14.12.11   秦 选择器 注意参数的设置
    var explorer = window.navigator.userAgent;
    //    if (explorer.indexOf("Chrome") >= 0) {//谷歌的
    window.open(url, "sd", "scrollbars=yes,resizable=yes,center=yes,minimize=yes,resizable=no,maximize=yes,height= 600px,width= 550px, top=50px, left= 650px");
    //    }
    //    else {//IE,火狐
    //        var v = window.showModalDialog(url, '', 'scrollbars=yes;resizable=yes;center=yes;resizable:no;minimize:yes;maximize:yes;dialogHeight: 650px; dialogWidth: 550px; dialogTop: 50px; dialogLeft: 650px;');
    //    }
    return;
}
function ReturnVal(ctrl, url, winName) {
    //update by dgq 2013-4-12 判断有没有？
    if (ctrl && ctrl.value != "") {
        if (url.indexOf('?') > 0)
            url = url + '&CtrlVal=' + ctrl.value;
        else
            url = url + '?CtrlVal=' + ctrl.value;
    }
    //修改标题控制不进行保存
    if (typeof self.parent.TabFormExists != 'undefined') {
        var bExists = self.parent.TabFormExists();
        if (bExists) {
            self.parent.ChangTabFormTitleRemove();
        }
    }
    //OpenJbox();
    if (window.ActiveXObject) {//如果是IE浏览器，执行下列方法
        var v = window.showModalDialog(url, winName, 'scrollbars=yes;resizable=yes;center=yes;minimize:yes;maximize:yes;dialogHeight: 650px; dialogWidth: 850px; dialogTop: 100px; dialogLeft: 150px;');
        if (v == null || v == '' || v == 'NaN') {
            return;
        }
        ctrl.value = v;
    }
    else {//如果是chrome，执行下列方法a
        try {
            //OpenJbox();
            $.jBox("iframe:" + url, {
                title: '标题',
                width: 800,
                height: 350,
                buttons: { '确定': 'ok' },
                submit: function (v, h, f) {
                    var row = h[0].firstChild.contentWindow.getSelected();
                    ctrl.value = row.Name;
                }
            });
        } catch (e) {
            alert(e);
        }
    }
    //修改标题，失去焦点时进行保存
    if (typeof self.parent.TabFormExists != 'undefined') {
        var bExists = self.parent.TabFormExists();
        if (bExists) {
            self.parent.ChangTabFormTitle();
        }
    }
    return;
}

function WinOpen(url) {
    var newWindow = window.open(url, 'z', 'scroll:1;status:1;help:1;resizable:1;dialogWidth:680px;dialogHeight:420px');
    newWindow.focus();
    return;
}
function WinOpenAndBrowser(url) {
    var Sys = {};
    var ua = navigator.userAgent.toLowerCase();
    var s;
    var n;
    (s = ua.match(/msie ([\d.]+)/)) ? Sys.ie = s[1] :
            (s = ua.match(/firefox\/([\d.]+)/)) ? Sys.firefox = s[1] :
            (s = ua.match(/chrome\/([\d.]+)/)) ? Sys.chrome = s[1] :
            (s = ua.match(/opera.([\d.]+)/)) ? Sys.opera = s[1] :
            (s = ua.match(/version\/([\d.]+).*safari/)) ? Sys.safari = s[1] : 0;
    n = ua.match(/msie ([\d.]+)/);
    if (Sys.firefox) {
        var newWindow = window.open(url, 'z', 'scroll:1;status:1;help:1;resizable:1;dialogWidth:680px;dialogHeight:420px');
        newWindow.focus();
        return;
    }
    else if (Sys.chrome) {
        var newWindow = window.open(url, 'z', 'scroll:1;status:1;help:1;resizable:1;dialogWidth:680px;dialogHeight:420px');
        newWindow.focus();
        return;
    }
    else if (n == null) {
        var newWindow = window.open(url, 'z', 'scroll:1;status:1;help:1;resizable:1;dialogWidth:680px;dialogHeight:420px');
        newWindow.focus();
        return;
    }
    else {
        if (Sys.ie) {
            alert(encodeURI("此模式不支持IE10一下版本，如需使用请升级IE或者使用chrome浏览器，谢谢"));
        }
    }
}
function WinOpen(url, winName) {
    var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
    newWindow.focus();
    return;
}
function openAcc(url) {
    location.href = url;
}

/* ESC Key Down  */
function Esc() {
    return;

//    if (event.keyCode == 27)
//        window.close();
//    return true;
}

/************************************************ 校验类 top *********************************************************/
/* 用来验证 输入的是不是一个数字． onkeypress */
function VirtyInt(ctrl) {

}

function VirtyNum(ctrl) {

    if (event.keyCode == 190) {
        if (ctrl.value.indexOf('.') == -1) {
            return true;
        }
        else {
            return false;
        }
    }
    if (ctrl.value.indexOf('.') >= 0 && event.keyCode == 46)
        return false;
    if (ctrl.value.indexOf('-') >= 0 && event.keyCode == 45)
        return false;
    // alert(event.keyCode);
    if (event.keyCode >= 37 && event.keyCode <= 40)
        return true;

    if (event.keyCode >= 96 && event.keyCode <= 105)
        return false;
    if (event.keyCode == 8)
        return true;

    //   alert(event.keyCode);

    var txtval = ctrl.value;
    var key = event.keyCode;
    if ((key < 48 || key > 57) && key != 45 && key != 46) {
        event.keyCode = 0;
    }
    else {
        if (key == 45) {
            if (txtval.indexOf("-") != -1)
                event.keyCode = 0;
        }
        if (key == 46) {
            if (txtval.indexOf(".") != -1 || txtval.length == 0)
                event.keyCode = 0;
        }
    }
    //非0-9，并且不为逗号或减号
    if ((key < 48 || key > 57) && key != 45 && key != 46) {
        event.keyCode = 0;
    }
    //数字0-9
    if (event.keyCode >= 48 && event.keyCode <= 57)
        return true;

    if (event.keyCode == 229)
        return true;

    if (event.keyCode == 8 || event.keyCode == 190)
        return true;

    if (event.keyCode == 13)
        return true;

    if (event.keyCode == 46)
        return true;

    if (event.keyCode == 45)
        return true;
    return false;
}

function VirtyNum(ctrl, type) {

    if (event.keyCode == 190) {
        if (type == 'int')
            return false;
        else {
            if (ctrl.value.indexOf('.') == -1) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    if (type == 'int') {
        //负号只能有一个
        if (ctrl.value.indexOf('-') > 0 && event.keyCode == 45)
            return false;
        //不能包含小数点
        if (event.keyCode == 46)
            return false;
    }

    if (type == 'float') {
        if (ctrl.value.indexOf('.') > 0 && event.keyCode == 46)
            return false;
        if (ctrl.value.indexOf('-') > 0 && event.keyCode == 45)
            return false;
    }
    // alert(event.keyCode);
    if (event.keyCode >= 37 && event.keyCode <= 40)
        return true;

    if (event.keyCode >= 96 && event.keyCode <= 105)
        return false;
    if (event.keyCode == 8)
        return true;

    //   alert(event.keyCode);
    var txtval = ctrl.value;
    var key = event.keyCode;
    if ((key < 48 || key > 57) && key != 45 && key != 46) {
        event.keyCode = 0;
    }
    else {
        if (key == 45) {
            if (txtval.indexOf("-") != -1)
                event.keyCode = 0;
        }
        if (key == 46) {
            if (txtval.indexOf(".") != -1 || txtval.length == 0)
                event.keyCode = 0;
        }
    }
    //非0-9，
    if ((key < 48 || key > 57) && key != 45 && key != 46) {
        event.keyCode = 0;
    }
    //数字0-9
    if (event.keyCode >= 48 && event.keyCode <= 57)
        return true;

    if (event.keyCode == 229)
        return true;

    if (event.keyCode == 8 || event.keyCode == 190)
        return true;

    if (event.keyCode == 13)
        return true;

    if (event.keyCode == 46)
        return true;

    if (event.keyCode == 45)
        return true;
    return false;
}

function VirtyMoney(number) {

    number = number.replace(/\,/g, "");
    if (number == "")
        return "0.00";

    if (number < 0)
        return '-' + outputDollars(Math.floor(Math.abs(number) - 0) + '') + outputCents(Math.abs(number) - 0);
    else
        return outputDollars(Math.floor(number - 0) + '') + outputCents(number - 0);
}

function outputDollars(number) {
    if (number.length <= 3)
        return (number == '' ? '0' : number);
    else {
        var mod = number.length % 3;
        var output = (mod == 0 ? '' : (number.substring(0, mod)));
        for (i = 0; i < Math.floor(number.length / 3); i++) {
            if ((mod == 0) && (i == 0))
                output += number.substring(mod + 3 * i, mod + 3 * i + 3);
            else
                output += ',' + number.substring(mod + 3 * i, mod + 3 * i + 3);
        }
        return (output);
    }
}
function outputCents(amount) {
    amount = Math.round(((amount) - Math.floor(amount)) * 100);
    return (amount < 10 ? '.0' + amount : '.' + amount);
}
/************************************************ 校验类End *********************************************************/

/* 显示日期 */
function ShowDateTime(appPath, ctrl) {
    url = appPath + '/Comm/Pub/CalendarHelp.htm';
    val = window.showModalDialog(url, '', 'dialogHeight: 335px; dialogWidth: 340px; center: yes; help: no');
    if (val == undefined)
        return;
    ctrl.value = val;
}

/* 默认植问题 */
function DefaultVal1(appPath, ctrl, className, attrKey, empId) {
    if (event.button != 2)
        return;
    url = appPath + '/Comm/DataHelp.htm?' + appPath + '/Comm/HelperOfTB.aspx?EnsName=' + className + '&AttrKey=' + attrKey + '&empId=' + empId;
    str = ctrl.value;
    str = window.showModalDialog(url + '&Key=' + str, '', 'dialogHeight: 500px; dialogWidth:850px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no');
    if (str == undefined)
        return;
    ctrl.value = str;
}

/* 默认植问题　 */
function RefEns(appPath, ctrl, className, attrKey) {
    if (event.button != 2)
        return;
    url = appPath + '/Comm/DataHelp.htm?' + appPath + '/Comm/HelperOfTB.aspx?EnsName=' + className + '&AttrKey=' + attrKey + '&empId=' + empId;
    str = ctrl.value;
    str = window.showModalDialog(url + '&Key=' + str, '', 'dialogHeight: 500px; dialogWidth:850px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no');
    if (str == undefined)
        return;
    ctrl.value = str;
}

/* about cookice */
function GetCookieVal(offset) {
    var endstr = document.cookie.indexOf(";", offset);
    if (endstr == -1)
        endstr = document.cookie.length;
    return unescape(document.cookie.substring(offset, endstr));
}
// 得到cooke .如果是Null , 返回的val
function GetCookie(name, isNullReVal) {
    var arg = name + "=";
    var alen = arg.length;
    var clen = document.cookie.length;
    var I = 0;
    while (I < clen) {
        var j = I + alen;
        if (document.cookie.substring(I, j) == arg)
            return GetCookieVal(j);
        I = document.cookie.indexOf(" ", I) + 1;
        if (I == 0)
            break;
    }
    return isNullReVal;
}

// 设置cook
function SetCookie(name, value) {
    var argv = SetCookie.arguments;
    var argc = SetCookie.arguments.length;
    var expires = (argc > 2) ? argv[2] : null;
    var path = (argc > 3) ? argv[3] : null;
    var domain = (argc > 4) ? argv[4] : null;
    var secure = (argc > 5) ? argv[5] : false;

    document.cookie = name + "=" + escape(value) + ((expires == null) ? "" : ("; expires=" + expires.toGMTString())) +
    ((path == null) ? "" : ("; path=" + path)) + ((domain == null) ? "" : ("; domain=" + domain)) + ((secure == true) ? "; secure" : "");
}

function HalperOfDDL(appPath, EnsName, refkeyvalue, reftext, ddlID) {
    var url = ''; // appPath+"/Comm/DataHelp.htm?HelperOfDDL.aspx?EnsName="+EnsName+"&RefKey="+refkeyvalue+"&RefText="+reftext;
    // modified by ZhouYue 2013-05-20  changed "/DataHelp.htm..." to "../DataHelp.htm"
    if (appPath == '/')
        url = "/WF/Comm/DataHelp.htm?HelperOfDDL.aspx?EnsName=" + EnsName + "&RefKey=" + refkeyvalue + "&RefText=" + reftext;
    else
        url = appPath + "/Comm/DataHelp.htm?HelperOfDDL.aspx?EnsName=" + EnsName + "&RefKey=" + refkeyvalue + "&RefText=" + reftext;
    var str = window.showModalDialog(url, '', 'dialogHeight: 500px; dialogWidth:800px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no');
    SetDDLVal(ddlID, str);
}

function SetDDLVal(ddlID, val) {
    if (val == undefined)
        return;
    var ddl = document.getElementById(ddlID);
    var mylen = ddl.options.length - 1;
    while (mylen >= 0) {
        if (ddl.options[mylen].value == val) {
            ddl.options[mylen].selected = true;
        }
        mylen--;
    }
}

function onDDLSelectedMore(ddlID, MainEns, EnsName, refkeyvalue, reftext) {
    var url = '';
    url = "../Comm/DataHelp.htm?HelperOfDDL.aspx?EnsName=" + EnsName + "&RefKey=" + refkeyvalue + "&RefText=" + reftext + "&MainEns=" + MainEns + "&DDLID=" + ddlID;
    var str = window.showModalDialog(url, '', 'dialogHeight: 500px; dialogWidth:850px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no');
    ddlID = ddlID.replace('DDL_', '');
    if (str != null) {
        var hrf = window.location.href;
        hrf = hrf.replace(ddlID, 's');
        hrf = hrf + '&' + ddlID + '=' + str;
        window.location.href = hrf;
    }
}

function onkeydown() {
    if (window.event.srcElement.tagName = "TEXTAREA")
        return false;
    if (event.keyCode == 13)
        event.keyCode = 9;
}

function RSize() {

    if (document.body.scrollWidth > (window.screen.availWidth - 100)) {
        window.dialogWidth = (window.screen.availWidth - 100).toString() + "px"
    } else {
        window.dialogWidth = (document.body.scrollWidth + 50).toString() + "px"
    }

    if (document.body.scrollHeight > (window.screen.availHeight - 70)) {
        window.dialogHeight = (window.screen.availHeight - 50).toString() + "px"
    } else {
        window.dialogHeight = (document.body.scrollHeight + 115).toString() + "px"
    }
    window.dialogLeft = ((window.screen.availWidth - document.body.clientWidth) / 2).toString() + "px"
    window.dialogTop = ((window.screen.availHeight - document.body.clientHeight) / 2).toString() + "px"
}

function ReinitIframe(frmID, tdID) {
    try {

        var iframe = document.getElementById(frmID);
        var tdF = document.getElementById(tdID);

        iframe.height = iframe.contentWindow.document.body.scrollHeight;
        iframe.width = iframe.contentWindow.document.body.scrollWidth;

        if (tdF.width < iframe.width) {
            //alert(tdF.width +'  ' + iframe.width);
            tdF.width = iframe.width;
        } else {
            iframe.width = tdF.width;
        }

        tdF.height = iframe.height;
        return;

    } catch (ex) {

        return;
    }
    return;
}
/* 设置选框 cb1.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')"; */
function SetSelected(cb, ids) {
    //alert(ids);
    var arrmp = ids.split(',');
    var arrObj = document.all;
    var isCheck = false;
    if (cb.checked)
        isCheck = true;
    else
        isCheck = false;
    for (var i = 0; i < arrObj.length; i++) {
        if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
            for (var idx = 0; idx <= arrmp.length; idx++) {
                if (arrmp[idx] == '')
                    continue;
                var cid = arrObj[i].name + ',';
                var ctmp = arrmp[idx] + ',';
                if (cid.indexOf(ctmp) > 1) {
                    arrObj[i].checked = isCheck;
                    //                    alert(arrObj[i].name + ' is checked ');
                    //                    alert(cid + ctmp);
                }
            }
        }
    }
}


/* 输入的是否是字段类型 */
function IsDigit(s) {
    if (s.value == '' || s.value == ' ')
        return true;
    var patrn = new RegExp("^[a-zA-Z][a-zA-Z0-9_]*$");
    if (!patrn.exec(s.value)) {
        alert("请输入字母或数字，第一个字符必须是字母！")
        s.value = "";
        return false;
    }
    return true;
}

function parseVal2Float(ctrl, defVal) {
    /// <summary>转换指定ID控件值为float数值</summary>
    /// <param name="ctrl" type="String">控件ID</param>
    /// <param name="defVal" type="String">控件默认值（值不填时自动符此默认值）</param>
    var tb_Ctrl = document.getElementById(ctrl);
    if (tb_Ctrl) {
        if (tb_Ctrl.value == "" || tb_Ctrl.value.replace(/ /g, "") == "") {
            tb_Ctrl.value = !defVal || defVal == "" || defVal.replace(/ /g, "") == "" ? "0" : defVal;
        }

        return parseFloat(tb_Ctrl.value.replace(',', ''))
    }
    return 0;
}

function TBHelp(ctrl, appPath, attrKey, enName) {
    //alert("woshiduohangwenbenkuang:" + ctrl + "-" + appPath + "-" + attrKey + "-" + enName);
    //双击多行文本框  woshiduohangwenbenkuang:TB_WTMS-/-WTMS-ND1101

    var url = "/WF/Comm/HelperOfTBEUI.aspx?AttrKey=" + attrKey + "&WordsSort=3" + "&FK_MapData=" + enName + "&id=" + ctrl;
    var explorer = window.navigator.userAgent;
    var str = "";
    if (explorer.indexOf("Chrome") >= 0 || explorer.indexOf("Firefox") >= 0) {
        window.open(url, "sd" + Math.random().toString(), "left=200,height=500,top=150,width=600,location=yes,menubar=no,resizable=yes,scrollbars=yes,status=no,toolbar=no");
    }
    else {
        str = window.showModalDialog(url, "sd", "dialogHeight:500px;dialogWidth:600px;dialogTop:150px;dialogLeft:200px;center:no;help:no");
        if (str == undefined) return;
        $("*[id$=" + ctrl + "]").focus().val(str);
    }
}
function WorkCheckTBHelp(ctrl, op) {
    var url = "Comm/HelperOfTBEUI.aspx";
    var str = window.showModalDialog(url, 'sd', 'dialogHeight: 500px; dialogWidth:800px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no');
    if (str == undefined)
        return;

    document.getElementsByTagName("iframe")[0].contentDocument.body.children[0][5].innerHTML = str;
}
//数字签名
function SigantureAct(ele, UserNo, FK_MapData, KeyOfEn, WorkID) {
    if (ele) {
        var imgSrc = ele.src;
        //修改数据
        var json_data = {
            "method": "sigantureact",
            "imgSrc": imgSrc,
            "UserNo": UserNo,
            "FK_MapData": FK_MapData,
            "KeyOfEn": KeyOfEn,
            "WorkID": WorkID
        };
        if (imgSrc.indexOf(UserNo) > 0 || imgSrc.indexOf("UnName") > 0) {

        }
        else {

        }
        var localHref = GetLocalWFPreHref();
        //修改数据
        $.ajax({
            type: "get",
            url: localHref + "/WF/Comm/HelperOfSiganture.aspx",
            data: json_data,
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            beforeSend: function (XMLHttpRequest) {
                //ShowLoading();
            },
            success: function (data, textStatus) {
                ele.src = localHref + "/DataUser/Siganture/" + data + ".JPG";
            },
            complete: function (XMLHttpRequest, textStatus) {
                //HideLoading();
            },
            error: function () {
                //请求出错处理
                alert("数字签名出错，请联系管理员检查表单属性设置。");
            }
        });
    }
}

//获取WF之前路径
function GetLocalWFPreHref() {
    var url = window.location.href;
    if (url.indexOf('/WF/') >= 0) {
        var index = url.indexOf('/WF/');
        url = url.substring(0, index);
    }
    return url;
}
//function closeWins() {if (window.dialogArguments && window.dialogArguments.window) window.dialogArguments.window.location = window.dialogArguments.window.location;}
//window.onunload = closeWins;

function endWith(str, s) {
    if (s == null || s == "" || str.length == 0 || s.length > str.length)
        return false;
    if (str.substring(str.length - s.length) == s)
        return true;
    else
        return false;
    return true;
}

function startWith(str, s) {
    if (s == null || s == "" || str.length == 0 || s.length > str.length)
        return false;
    if (str.substr(0, s.length) == s)
        return true;
    else
        return false;
    return true;

}

function replaceAll(s1, s2, s3) {
    return s1.replace(new RegExp(s2, 'gm'), s3);
}

/**
 * AtPara=@key1=value1@key2=valu2...@keyN=valueN
 */
function GetAtPara(atPara, key) {
	if (typeof atPara != "string" || typeof key == "undefined" || key == "") {
		return undefined;
	}
	var reg = new RegExp("(^|@)" + key + "=([^@]*)(@|$)");
	var results = atPara.match(reg);
	if (results != null) {
		return unescape(results[2]);
	}
	return undefined;
}

/**
 * 输入验证firfox, ff浏览器不支持execCommand()
 */
function isFF() {
	return navigator.userAgent.indexOf("Firefox") > 0;
}
function valitationBefore(o, validateType) {
	if (isFF()) {
		var value = o.value;
		var flag = false;
		switch (validateType) {
			case "int" :
				flag = (!isNaN(value) && value % 1 === 0);
			break;
			case "float" :
			case "money":
			    if (value.indexOf("-") == 0 && value.length == 1)
			        break;
			    else {
			        flag = !isNaN(value);
			        break;
			    }
		}
		if (flag) {
		    return;
		}
	}
}

function valitationAfter(o, validateType) {
    var value = o.value;
    value = value.replace(/[^\d.-]/g, "");
    if (isFF()) {
        var flag = false;
        switch (validateType) {
            case "int":
                flag = (!isNaN(value) && value % 1 === 0);
                break;
            case "float":
            case "money":
                if (value.indexOf("-") == 0 && value.length == 1)
                    break;
                else {
                    flag = !isNaN(value);
                    break;
                }
        }
        if (!flag) {
            o.value = 0;
        }else
        o.value = value;
    } else {
        if (isNaN(value)) execCommand('undo');
        o.value = value;
    }
}

/**
 * 输入验证firfox, ff浏览器不支持execCommand()
 */

function limitLength(obj, length) {
   obj.value = obj.value.replace(/[^\d.-]/g, "");  //清除“数字”和“.”以外的字符 ;
    if (length != null && length != "" && length != "undefined") {
        if (obj.value.indexOf('.')>=0 && obj.value.split('.')[1].length > length) {
            obj.value = obj.value.substring(0, obj.value.length - 1);
            //obj.focus();
        }
    }
}

//类型为Money时输入设置
  function clearNoNum(obj){  
        //修复第一个字符是小数点 的情况.  
        if(obj.value !=''&& obj.value.substr(0,1) == '.'){  
            obj.value=0+obj.value;
        }
        if (obj.value == "")
            obj.value = 0.00;

        if (!/\./.test(obj.value)) //为整数字符串在末尾添加.00  
            obj.value += '.00';  

        obj.value = obj.value.replace(/^0*(0\.|[1-9])/, '$1');//解决 粘贴不生效  
        obj.value = obj.value.replace(/[^\d.]/g,"");  //清除“数字”和“.”以外的字符  
        obj.value = obj.value.replace(/\.{2,}/g,"."); //只保留第一个. 清除多余的       
        obj.value = obj.value.replace(".","$#$").replace(/\./g,"").replace("$#$",".");      
        obj.value = obj.value.replace(/^(\-)*(\d+)\.(\d\d).*$/,'$1$2.$3');//只能输入两个小数       
        if(obj.value.indexOf(".")< 0 && obj.value !=""){//以上已经过滤，此处控制的是如果没有小数点，首位不能为类似于 01、02的金额  
            if(obj.value.substr(0,1) == '0' && obj.value.length == 2){  
                obj.value= obj.value.substr(1,obj.value.length);      
            }  
        }      
    }
    function FormatMoney(obj, precision, separator) {
        if (precision == undefined || precision == null || precision == "")
            precision = 2;
        if (precision != 2)
            return;
        var val = formatNumber(obj.value, precision, separator);
        if (val != NaN)
            obj.value = val;

    }

    /** 
    * 将数值格式化成金额形式 
    * 
    * @param num 数值(Number或者String) 
    * @param precision 精度，默认不变
    * @param separator 分隔符，默认为逗号
    * @return 金额格式的字符串,如'1,234,567'，默认返回NaN
    * @type String 
    */
    function formatNumber(num, precision, separator) {
        if (precision != 2)
            return num;
        var parts;
        // 判断是否为数字
        if (!isNaN(parseFloat(num)) && isFinite(num)) {
            // 把类似 .5, 5. 之类的数据转化成0.5, 5, 为数据精度处理做准, 至于为什么
            // 不在判断中直接写 if (!isNaN(num = parseFloat(num)) && isFinite(num))
            // 是因为parseFloat有一个奇怪的精度问题, 比如 parseFloat(12312312.1234567119)
            // 的值变成了 12312312.123456713
            num = Number(num);
            // 处理小数点位数
            num = (typeof precision !== 'undefined' ? (Math.round(num * Math.pow(10, precision)) / Math.pow(10, precision)).toFixed(precision) : num).toString();
            // 分离数字的小数部分和整数部分
            parts = num.split('.');
            // 整数部分加[separator]分隔, 借用一个著名的正则表达式
            parts[0] = parts[0].toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1' + (separator || ','));

            return parts.join('.');
        }
        return 0;
    }
/* 文本框根据输入内容自适应高度
* @param                {HTMLElement}        输入框元素
* @param                {Number}                设置光标与输入框保持的距离(默认0)
* @param                {Number}                设置最大高度(可选)
*/
var autoTextarea = function (elem, extra, maxHeight) {
    extra = extra || 0;
    var isFirefox = !!document.getBoxObjectFor || 'mozInnerScreenX' in window,
    isOpera = !!window.opera && !!window.opera.toString().indexOf('Opera'),
            addEvent = function (type, callback) {
                    elem.addEventListener ?
                            elem.addEventListener(type, callback, false) :
                            elem.attachEvent('on' + type, callback);
            },
            getStyle = elem.currentStyle ? function (name) {
                    var val = elem.currentStyle[name];
 
                    if (name === 'height' && val.search(/px/i) !== 1) {
                            var rect = elem.getBoundingClientRect();
                            return rect.bottom - rect.top -
                                    parseFloat(getStyle('paddingTop')) -
                                    parseFloat(getStyle('paddingBottom')) + 'px';        
                    };
 
                    return val;
            } : function (name) {
                            return getComputedStyle(elem, null)[name];
            },
            minHeight = parseFloat(getStyle('height'));
 
    elem.style.resize = 'none';
 
    var change = function () {
            var scrollTop, height,
                    padding = 0,
                    style = elem.style;
 
            if (elem._length === elem.value.length) return;
            elem._length = elem.value.length;
 
            if (!isFirefox && !isOpera) {
                    padding = parseInt(getStyle('paddingTop')) + parseInt(getStyle('paddingBottom'));
            };
            scrollTop = document.body.scrollTop || document.documentElement.scrollTop;
 
            elem.style.height = minHeight + 'px';
            if (elem.scrollHeight > minHeight) {
                    if (maxHeight && elem.scrollHeight > maxHeight) {
                            height = maxHeight - padding+10;
                            style.overflowY = 'auto';
                    } else {
                            height = elem.scrollHeight - padding+10;
                            style.overflowY = 'hidden';
                    };
                    style.height = height + extra + 'px';
                    scrollTop += parseInt(style.height) - elem.currHeight;
                    document.body.scrollTop = scrollTop;
                    document.documentElement.scrollTop = scrollTop;
                    elem.currHeight = parseInt(style.height);
            };
    };
 
    addEvent('propertychange', change);
    addEvent('input', change);
    addEvent('focus', change);
    change();
};
    