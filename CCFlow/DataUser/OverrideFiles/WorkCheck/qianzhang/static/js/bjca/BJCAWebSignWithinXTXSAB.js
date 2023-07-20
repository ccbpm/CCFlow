/*--------------------------------------------------------------------------  
 *
 * BJCA Adaptive Javascript, Version SAB(Support All Browsers :))
 * This script support bjca client version 2.0 and later
 * Author:BJCA-zys
 *--------------------------------------------------------------------------*/

/* globals var */
var $_$softCertListID_BJCAWebSign = ""; // Soft CertListID, Set by SetUserCertList
var $_$hardCertListID_BJCAWebSign = ""; // USBKeyCertListID, Set by SetUserCertList
var $_$allCertListID_BJCAWebSign = "";  // All CertListID, Set by SetUserCertList
var $_$loginCertID_BJCAWebSign = "";    // logined CertID, Set by SetAutoLogoutParameter
var $_$logoutFunc_BJCAWebSign = null;   // logout Function, Set by SetAutoLogoutParameter
var $_$onUsbKeyChangeCallBackFunc_BJCAWebSign = null; //custom onUsbkeyChange callback function
var $_$XTXAlert_BJCAWebSign = null;     // alert custom function
var $_$WebSocketObj_BJCAWebSign = null; // WebSocket class Object
var $_$CurrentObj_BJCAWebSign = null;   // Current use class Object

// const var
var CERT_TYPE_HARD = 1;
var CERT_TYPE_SOFT = 2;
var CERT_TYPE_ALL  = 3;

// set auto logout parameters
function SetAutoLogoutParameter_BJCAWebSign(strCertID, logoutFunc) 
{
    $_$loginCertID_BJCAWebSign = strCertID;
    $_$logoutFunc_BJCAWebSign = logoutFunc;
    return;
}

function SetLogoutFunction_BJCAWebSign(logoutFunc) 
{
    $_$logoutFunc_BJCAWebSign = logoutFunc;
}

/*
*删除数组指定下标或指定对象 
*/ 
Array.prototype.remove = function(obj) { 
 for ( var i = 0; i < this.length; i++) { 
 var temp = this[i]; 
 if (!isNaN(obj)) { 
 temp = i; 
 } 
 if (temp == obj) { 
 for ( var j = i; j < this.length; j++) { 
 this[j] = this[j + 1]; 
 } 
 this.length = this.length - 1; 
 } 
 } 
} 

function $popDropListBoxAll_BJCAWebSign(strListID)
{
    var objListID = eval(strListID);
    if (objListID == undefined) {
        return;
    }
    var i, n = objListID.length;
    for(i = 0; i < n; i++) {
        objListID.remove(0);
    }
    
    objListID = null;
}

function $pushOneDropListBox_BJCAWebSign(userListArray, strListID) 
{
    var objListID = eval(strListID);
    if (objListID == undefined) {
        return;
    }
    
    var i;
    for (i = 0; i < userListArray.length; i++) {
        var certObj = userListArray[i];
        var objItem = new Option(certObj.certName, certObj.certID);
        objListID.options.add(objItem);
    }
    
    objListID = null;
    
    return;
}

function $pushAllDropListBox_BJCAWebSign(certUserListObj_BJCAWebSign) 
{
    if ($_$hardCertListID_BJCAWebSign != "") {
        $popDropListBoxAll_BJCAWebSign($_$hardCertListID_BJCAWebSign);
    }
    if ($_$softCertListID_BJCAWebSign != "") {
        $popDropListBoxAll_BJCAWebSign($_$softCertListID_BJCAWebSign);
    }
    
    if ($_$allCertListID_BJCAWebSign != "") {
        $popDropListBoxAll_BJCAWebSign($_$allCertListID_BJCAWebSign);
    }
    
    var strUserList = certUserListObj_BJCAWebSign.retVal;
    var allListArray = []
    while (true) {
        var i = strUserList.indexOf("&&&");
        if (i <= 0 ) {
            break;
        }
        var strOneUser = strUserList.substring(0, i);
        var strName = strOneUser.substring(0, strOneUser.indexOf("||"));
        var strCertID = strOneUser.substring(strOneUser.indexOf("||") + 2, strOneUser.length);
        allListArray.push({certName:strName, certID:strCertID});
        
        if ($_$hardCertListID_BJCAWebSign != "") {
            GetDeviceType_BJCAWebSign(strCertID, function(retObj) {
                if (retObj.retVal == "HARD") {
                    $pushOneDropListBox_BJCAWebSign([retObj.ctx], $_$hardCertListID_BJCAWebSign);
                }
            }, {certName:strName, certID:strCertID});
        }
        
        if ($_$softCertListID_BJCAWebSign != "") {
            GetDeviceType_BJCAWebSign(strCertID, function(retObj) {
                if (retObj.retVal == "SOFT") {
                    $pushOneDropListBox_BJCAWebSign([retObj.ctx], $_$softCertListID_BJCAWebSign);
                }
            }, {certName:strName, certID:strCertID});
        }
        var len = strUserList.length;
        strUserList = strUserList.substring(i + 3,len);
    }

    if ($_$allCertListID_BJCAWebSign != "") {
        $pushOneDropListBox_BJCAWebSign(allListArray, $_$allCertListID_BJCAWebSign);
    }
}

function $myAutoLogoutCallBack_BJCAWebSign(retObj)
{
    if (retObj.retVal.indexOf($_$loginCertID) <= 0) {
        $_$logoutFunc_BJCAWebSign();
    }
}

//usbkey change default callback function
function $OnUsbKeyChange_BJCAWebSign() 
{
    GetUserList_BJCAWebSign($pushAllDropListBox_BJCAWebSign);
    if (typeof $_$onUsbKeyChangeCallBackFunc_BJCAWebSign == 'function') {
        $_$onUsbKeyChangeCallBackFunc_BJCAWebSign();
    }
    if ($_$loginCertID_BJCAWebSign != "" && typeof $_$logoutFunc_BJCAWebSign == 'function') {
        GetUserList_BJCAWebSign($myAutoLogoutCallBack_BJCAWebSign);
    }
}

//webSocket client class
function CreateWebSocketObject_BJCAWebSign() {
    
    var o = new Object();
    o.ws_obj = null;
    o.ws_heartbeat_id = 0;
    o.ws_queue_id = 0; // call_cmd_id
    o.ws_queue_list = {};  // call_cmd_id callback queue
    o.ws_queue_ctx = {};
    o.xtx_version = "";
    
    o.load_websocket = function() {

        //说明，是否用TLS/SSL，根据url给的ws/wss及端口号来定，端口号固定(21051/21061)
        var ws_url;
        ws_url = "ws://127.0.0.1:21051/xtxapp/"; //非TLS/SSL
        //ws_url = "wss://127.0.0.1:21061/xtxapp/";
        try {
            o.ws_obj = new WebSocket(ws_url); 
        } catch (e) {
        //    console.log(e);
            return ;
        }
        
        o.ws_queue_list["onUsbkeyChange"] = $OnUsbKeyChange_BJCAWebSign;
        
        o.ws_obj.onopen = function(evt) { 
            clearInterval(o.ws_heartbeat_id);
            o.callMethod("SOF_GetVersion", function(str){o.xtx_version = str.retVal;});
            o.ws_heartbeat_id = setInterval(function () {
             //   o.callMethod("SOF_GetVersion", function(str){});
            }, 10 * 1000);
           // GetUserList_BJCAWebSign($pushAllDropListBox_BJCAWebSign);
        }; 
        
        o.ws_obj.onclose = function(evt) { 
           
        }; 
        
        o.ws_obj.onmessage = function(evt) { 
            
            var res = JSON.parse(evt.data);  
            if(res['set-cookie']){
                document.cookie = res['set-cookie'];
            }

            //登录失败
            if(res['loginError'])
            {
                alert(res['loginError']);
            }

            var call_cmd_id = res['call_cmd_id'];
            if(!call_cmd_id)
            {
                return;
            }

            var execFunc = o.ws_queue_list[call_cmd_id];
            if(typeof(execFunc) != 'function')
            {
                return;
            }
      
            var ctx = o.ws_queue_ctx[res['call_cmd_id']];
            ctx = ctx || {returnType:"string"};

            var ret;
            if (ctx.returnType == "bool"){
                ret = res.retVal == "true" ? true : false;
            } 
            else if (ctx.returnType == "number"){
                ret = Number(res.retVal);
            } 
            else{
                ret = res.retVal;
            }     
            var retObj = {retVal:ret, ctx:ctx};

            execFunc(retObj);

            if (res['call_cmd_id'] != "onUsbkeyChange") {
                delete o.ws_queue_list[res['call_cmd_id']];
            }
            delete o.ws_queue_ctx[res['call_cmd_id']];
        
        }; 
        
        o.ws_obj.onerror = function(evt) { 
            o.load_websocket();
        };
        
        return true;
    };
    
    o.sendMessage = function(sendMsg) {
        if (o.ws_obj.readyState == WebSocket.OPEN) {
            o.ws_obj.send(JSON.stringify(sendMsg));
        } else {
            console.log("Can't connect to WebSocket server!");
        }
    };
    
    o.callMethod = function(strMethodName, cb, ctx, returnType, argsArray) {
        o.ws_queue_id++;
        if (typeof(cb) == 'function'){
            o.ws_queue_list['i_' + o.ws_queue_id] = cb;
            ctx = ctx || {};
            ctx.returnType = returnType;           
            o.ws_queue_ctx['i_' + o.ws_queue_id] = ctx;
        }
        
        var sendArray = {};
        sendArray['cookie'] = document.cookie;
        sendArray['xtx_func_name'] = strMethodName;
        //get sessionid
        
        sendArray['call_cmd_id'] = 'i_' + o.ws_queue_id ;
        if (o.xtx_version >= "2.16") {
            sendArray['URL'] = window.location.href;
        }
        
        if (arguments.length > 4) {
            for (var i = 1; i <= argsArray.length; i++) {
                var strParam = "param_" + i;
                sendArray[strParam] = argsArray[i - 1];
            }
			sendArray["param"] = argsArray;
        }
        
        if (o.ws_obj.readyState == WebSocket.OPEN) {
            o.sendMessage(sendArray)
        } else if (o.ws_obj.readyState != WebSocket.CONNECTING) {
            o.load_websocket();
            setTimeout(o.sendMessage(sendArray), 500);
        }
    };
	
	o.SOF_IsLogin_BJCAWebSign = function(strCertID, cb, ctx){
        var paramArray = [strCertID];
        o.callMethod('SOF_IsLogin', cb, ctx, "bool",paramArray);
    }

    o.SetSignMethod_BJCAWebSign = function(sign_algo, cb, ctx){
        var paramArray = [sign_algo];
        o.callMethod('SOF_SetSignMethod', cb, ctx, "string",paramArray);
    }

    o.GetUserList_BJCAWebSign = function(cb, ctx) {
        o.callMethod('SOF_GetUserList', cb, ctx, "string");
    };

	o.SetUserConfig_BJCAWebSign = function(type, strConfig,cb, ctx) {
		var paramArray = [type,strConfig];
        o.callMethod('SetUserConfig', cb, ctx, "bool",paramArray);
    };

    o.ExportUserSignCert_BJCAWebSign = function(strCertID, cb, ctx) {
        var paramArray = [strCertID];
        o.callMethod('SOF_ExportUserCert', cb, ctx, "string", paramArray);
    };

    o.VerifyUserPIN_BJCAWebSign = function(strCertID, strUserPIN, cb, ctx) {
        var paramArray = [strCertID, strUserPIN];
        o.callMethod('SOF_Login', cb, ctx, "bool", paramArray);
    };

    o.GetCertInfo_BJCAWebSign = function(strCert, Type, cb, ctx) {
        var paramArray = [strCert, Type];
        o.callMethod('SOF_GetCertInfo', cb, ctx, "string", paramArray);
    };

	o.SignDataBase64_BJCAWebSign = function(strCertID, strInData, cb, ctx) {
        var paramArray = [strCertID, strInData];
        o.callMethod('SOF_SignDataBase64', cb, ctx, "string", paramArray);
    };
      
    o.GetDeviceType_BJCAWebSign = function(strCertID, cb, ctx) {
        var paramArray = [strCertID, 7];
        o.callMethod('GetDeviceInfo', cb, ctx, "string", paramArray);
    }

    o.GetLastLoginCertID_BJCAWebSign = function(cb, ctx) {
        o.callMethod('SOF_GetLastLoginCertID', cb, ctx, "string");
    };
 
    if (!o.load_websocket()) {
        return null;
    }
    
    return o;
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////// 对外提供的接口

function $myErrorRtnFunc_BJCAWebSign(retVal, cb, ctx)
{
    if (typeof cb == 'function') {
        var retObj = {retVal:retVal, ctx:ctx};
        cb(retObj);
    }
    
    return retVal;
}
//get user list
function GetUserList_BJCAWebSign(cb, ctx) {
    if ($_$CurrentObj_BJCAWebSign != null) {
        return $_$CurrentObj_BJCAWebSign.GetUserList_BJCAWebSign(cb, ctx);
    } else {
        return $myErrorRtnFunc_BJCAWebSign("", cb, ctx);
    }
}

// set user cert list id
function SetUserCertList_BJCAWebSign(strListID, certType) 
{
    if (arguments.length == 1) {
        $_$hardCertListID_BJCAWebSign = strListID;
    } else {
        if (certType == CERT_TYPE_HARD) {
            $_$hardCertListID_BJCAWebSign = strListID;
        }
        if (certType == CERT_TYPE_SOFT) {
            $_$softCertListID_BJCAWebSign = strListID;
        }
        if (certType == CERT_TYPE_ALL) {
            $_$allCertListID_BJCAWebSign = strListID;
        }
    }
    GetUserList_BJCAWebSign($pushAllDropListBox_BJCAWebSign);
    
    return;
}

//verify user pin
function VerifyUserPIN_BJCAWebSign(strCertID, strUserPIN, cb, ctx) {
    if ($_$CurrentObj_BJCAWebSign != null) {
        return $_$CurrentObj_BJCAWebSign.VerifyUserPIN_BJCAWebSign(strCertID, strUserPIN, cb, ctx);
    } else {
        return $myErrorRtnFunc_BJCAWebSign(false, cb, ctx);
    }
}

//sign data
function SignDataBase64_BJCAWebSign(strCertID, strInData, cb, ctx) {
    if ($_$CurrentObj_BJCAWebSign != null) {
        return $_$CurrentObj_BJCAWebSign.SignDataBase64_BJCAWebSign(strCertID, strInData, cb, ctx);
    } else {
        return $myErrorRtnFunc_BJCAWebSign("", cb, ctx);
    }
}

//get device type return SOFT or HARD
function GetDeviceType_BJCAWebSign(strCertID, cb, ctx) {
    if ($_$CurrentObj_BJCAWebSign != null && $_$CurrentObj_BJCAWebSign.GetDeviceType_BJCAWebSign != undefined) {
        return $_$CurrentObj_BJCAWebSign.GetDeviceType_BJCAWebSign(strCertID, cb, ctx);
    } else {
        return $myErrorRtnFunc_BJCAWebSign("HARD", cb, ctx);
    }   
}

function SetSignMethod_BJCAWebSign(sign_algo,cb, ctx){
    if ($_$CurrentObj_BJCAWebSign != null) {
        return $_$CurrentObj_BJCAWebSign.SetSignMethod_BJCAWebSign(sign_algo,cb, ctx);
    } 
    return "";
}

function SOF_IsLogin_BJCAWebSign(strCertID,cb, ctx){
    if ($_$CurrentObj_BJCAWebSign != null) {
        return $_$CurrentObj_BJCAWebSign.SOF_IsLogin_BJCAWebSign(strCertID,cb, ctx);
    } 
    return "";
}

function ExportUserSignCert_BJCAWebSign(strCertID,cb, ctx){
    if ($_$CurrentObj_BJCAWebSign != null) {
        return $_$CurrentObj_BJCAWebSign.ExportUserSignCert_BJCAWebSign(strCertID,cb, ctx);
    } 
    return "";
}

function GetCertInfo_BJCAWebSign(strCert, Type,cb, ctx){
    if ($_$CurrentObj_BJCAWebSign != null) {
        return $_$CurrentObj_BJCAWebSign.GetCertInfo_BJCAWebSign(strCert, Type,cb, ctx);
    } 
    return "";
}

function SetUserConfig_BJCAWebSign(type, strConfig,cb, ctx){
    if ($_$CurrentObj_BJCAWebSign != null) {
        return $_$CurrentObj_BJCAWebSign.SetUserConfig_BJCAWebSign(type, strConfig,cb, ctx);
    } 
    return "";
}

function GetLastLoginCertID_BJCAWebSign(cb, ctx) {
    if ($_$CurrentObj_BJCAWebSign != null) {
        return $_$CurrentObj_BJCAWebSign.GetLastLoginCertID_BJCAWebSign(cb, ctx);
    } else {
        return $myErrorRtnFunc_BJCAWebSign("", cb, ctx);
    }
}

(function() {
	$_$WebSocketObj_BJCAWebSign = CreateWebSocketObject_BJCAWebSign();
    if ($_$WebSocketObj_BJCAWebSign != null) {
        $_$CurrentObj_BJCAWebSign = $_$WebSocketObj_BJCAWebSign;
        return;
    } 

    $_$CurrentObj_BJCAWebSign = null;
    
   $XTXAlert_BJCAWebSign("检查证书应用环境出错!");
    return;
})();
