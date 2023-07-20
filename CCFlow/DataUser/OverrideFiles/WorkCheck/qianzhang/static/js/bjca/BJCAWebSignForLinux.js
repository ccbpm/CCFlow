/* globals var */

var $_$WebSocketConnectState = false;
var $_$WebSign_WebSocketObj = null; // WebSocket class Object
var $_$WebSign_CurrentObj = null; // Current use class Object

var $_$SGD_SM3_RSA = 0x00010001; //基于SM3算法和RSA算法的签名
var $_$SGD_SHA1_RSA = 0x00010002; //基于SHA1算法和RSA算法的签名
var $_$SGD_SHA256_RSA = 0x00010004; //基于SHA256算法和RSA算法的签名
var $_$SGD_SM3_SM2 = 0x00020101; //基于SM2算法和SM3算法的签名

/**
 * 设置证书信任链
 */
function ESeaL_SetUserConfig() 
{
	var p7bs_len = $_$P7BS.length;
	var certs_len = $_$CERTS.length;
	if(p7bs_len <= 0 || certs_len <= 0) {
		return;
	}
	
	for(i = 0; i < p7bs_len; i++) {
		SetUserConfig_BJCAWebSign(6, $_$P7BS[i], function (strRes) {
			if (!strRes.retVal) {
				return;
			}
		}, null);		
	}
	
	for(i = 0; i < certs_len; i++) {
		SetUserConfig_BJCAWebSign(4, $_$CERTS[i], function (strRes) {
			if (!strRes.retVal) {
				return;
			}
		}, null);		
	}
}

/**
 * 成功回调函数
 */
function ESeaL_OKRtnFunc(retVal, cb) {
	if (typeof cb == "function") {
		var retObj = {
			retVal: retVal
		};
		cb(retObj);
	}
	return retVal;
}

/**
 * 失败回调函数
 */
function ESeaL_ErrorRtnFunc(retVal, cb) {
	if (typeof cb == "function") {
		var retObj = {
			retVal: retVal
		};
		cb(retObj);
	}
	return retVal;
}

/**
 * 设置在线签章地址，
 * @param iType 0：离线，1：国密在线，2：国办在线验证印章状态
 * @param url 服务器url
 */   
function ESeaL_SetSignTypeAndURL(iType, url, cb) {
	var ctx = null;
    if ($_$WebSign_WebSocketObj != null && $_$WebSign_WebSocketObj.SetSignTypeAndURL != undefined) {
        return $_$WebSign_WebSocketObj.SetSignTypeAndURL(iType, url, cb, ctx);
    } else {
        return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
    }
}

/**
 * 获取在线用户列表，
 * @param certid 用户certid
 */   
function $GetSealList(certid, cb) {
	var ctx = null;
    if ($_$WebSign_WebSocketObj != null && $_$WebSign_WebSocketObj.GetSealList != undefined) {
        return $_$WebSign_WebSocketObj.GetSealList(certid, cb, ctx);
    } else {
        return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
    }
}

/**
 * 获取在线用户列表，
 * @param certid 用户certid
 */   
function ESeaL_GetUserSealList(certid, cb) {
	$GetSealList(certid, function(ret){
		if(ret.retVal == "") {
			return ESeaL_ErrorRtnFunc("", cb);
		}
		//获取的格式为sealName1||sealNum1&&&sealName2||sealNum2&&&
		var strSealList = ret.retVal;
		var sealListArray = [];
		while (true) {
			var i = strSealList.indexOf("&&&");
			if (i <= 0) {
				break;
			}
			var strOneSeal = strSealList.substring(0, i);
			sealListArray.push(strOneSeal);
			var len = strSealList.length;
			strSealList = strSealList.substring(i + 3, len);
		}
		return ESeaL_OKRtnFunc(sealListArray, cb);
	});
}

/**
 * 获取印章图片
 * @param certid 用户certid
 * @param strOneSeal， 选中印章信息，格式：sealName||sealNum
 */
function ESeaL_GetStampPic(certid, strOneSeal, cb) {
	var ctx = null;
	var strName = strOneSeal.substring(0, strOneSeal.indexOf("||"));
    var strSealNum = strOneSeal.substring(strOneSeal.indexOf("||") + 2, strOneSeal.length);
			
	if($_$WebSign_WebSocketObj != null && $_$WebSign_WebSocketObj.ESeaL_GetStampPicForAll != undefined) {
		return $_$WebSign_WebSocketObj.ESeaL_GetStampPicForAll(certid, strSealNum, cb, ctx);
	} else {
        return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
    }
}

/**
 * 签章
 * @param strCertID:CertID
 * @param org_data 签名原文
 */
function ESeaL_Sign(strCertID, org_data, cb) {		
	// 签章
	var ctx = null;
    if($_$WebSign_WebSocketObj != null && $_$WebSign_WebSocketObj.ESeaL_Sign != undefined) {
        return $_$WebSign_WebSocketObj.ESeaL_Sign(strCertID, org_data, cb, ctx);
    } else {
        return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
    }
}

/**
 * 批量签章
 * @param strCertID:CertID
 * @param org_data 签名原文
 */
function ESeaL_BatchSign(strCertID, org_data, cb) {      
    // 签章
    var ctx = null;
    if($_$WebSign_WebSocketObj != null && $_$WebSign_WebSocketObj.ESeaL_BatchSign != undefined) {
        return $_$WebSign_WebSocketObj.ESeaL_BatchSign(strCertID, org_data, cb, ctx);
    } else {
        return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
    }
}

/**
 * 签章
 * @param orgData 签名原文
 * @param signdata 签名值
 */
function ESeal_Verify(orgdata, signdata, cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.Verify != undefined) {
		return $_$WebSign_CurrentObj.Verify(orgdata, signdata, cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}

/**
 * 签章
 * @param bVerifyResult 验证结果
 * @param signdata 签名值
 */
function ESeal_GetStampPicAfterVerified(bVerifyResult, signdata, cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.GetStampPicAfterVerified != undefined) {
		return $_$WebSign_CurrentObj.GetStampPicAfterVerified(bVerifyResult, signdata, cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}

/**
 * 撤章
 * @param certID:用户CertID
 * @param signature:签名值
 */
function ESeaL_RemoveSeal(certID, signature, cb) {	
	//判断是否登录
	SOF_IsLogin_BJCAWebSign(certID, function (ret) {
		if (!ret.retVal) {
			if (typeof cb == 'function') {
				var retObj = {retVal:false, isLogin:false};
				cb(retObj);
			}
			return;
		} 
		if ($_$WebSign_WebSocketObj != null && $_$WebSign_WebSocketObj.ESeaL_CheckRemoveRight != undefined) {
			//1、获取待签名数据
			$_$WebSign_WebSocketObj.ESeaL_CheckRemoveRight(certID, signature, function (ret) {
				if (ret.retVal == "true") {
					if (typeof cb == 'function') {
						var retObj = {retVal:ret.retVal, isLogin:true};
						cb(retObj);
					}
					return;
				} else {
					if (typeof cb == 'function') {
						var retObj = {retVal:ret.retVal, isLogin:true};
						cb(retObj);
					}
					return;
				}	
			});
		} else {
			return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
		}
	}, null);			
}

/**
 * 显示证书信息
 * @param signature:签名值
 */
function Eseal_ShowUserCerInfo(signature, cb) {
    if ($_$WebSign_WebSocketObj != null && $_$WebSign_WebSocketObj.ESeaL_ShowUserCerInfo != undefined) {
        return $_$WebSign_WebSocketObj.ESeaL_ShowUserCerInfo(signature, cb);
    } else {
        return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
    }	
}

/**
 * 显示签章时间
 * @param signature:签名值
 */
function Eseal_ShowSignTime(signature, cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.ShowSignTimeEx != undefined) {
		return $_$WebSign_CurrentObj.ShowSignTimeEx(signature, cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}

/**
 * 关于
 */
function ESeal_GetVersion(cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.GetVersion != undefined) {
		return $_$WebSign_CurrentObj.GetVersion(cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}
/**
 * 获取最近一次错误信息
 */
function ESeal_GetLastErrJS(cb) {
	var ctx = null;
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.GetLastErr != undefined) {
		return $_$WebSign_CurrentObj.GetLastErr(cb, ctx);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}

/**
 * 获取签章算法
 */
function ESeal_GetSignMethod(signdata, cb) {
    if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.ESeaL_GetSignMethod != undefined) {
        return $_$WebSign_CurrentObj.ESeaL_GetSignMethod(signdata, cb);
    } else {
        return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
    }
}

/**
 * 创建网页签章webSocket对象
 */
//webSocket client class
function CreateWebSocketObject_WebSign() {
    var o = new Object();
    o.ws_obj = null;
    o.ws_heartbeat_id = 0;
    o.ws_queue_id = 0; // call_cmd_id
    o.ws_queue_list = {}; // call_cmd_id callback queue
    o.ws_queue_ctx = {};
    o.xtx_version = "";
    o.load_websocket = function () {
        //说明，是否用TLS/SSL，根据url给的ws/wss及端口号来定，端口号固定(21051/21061)
        var ws_url;
        ws_url = "ws://127.0.0.1:21051/websign/"; //非TLS/SSL
        //ws_url = "wss://127.0.0.1:21061/xtxapp/";
        try {
            o.ws_obj = new WebSocket(ws_url);
        } catch (e) {
            //   console.log(e);
            return;
        }
        //   o.ws_queue_list["onUsbkeyChange"] = $OnUsbKeyChange;
        o.ws_obj.onopen = function (evt) {
            clearInterval(o.ws_heartbeat_id);
            //   o.callMethod("SOF_GetVersion", function(str){o.xtx_version = str.retVal;});
            o.ws_heartbeat_id = setInterval(function () {
                o.callMethod("ESeaL_GetGetVersion", function(str){});
            }, 10 * 1000);
            //    GetUserList($pushAllDropListBox);
            $_$WebSocketConnectState = true;
        };
        o.ws_obj.onclose = function (evt) {};
        o.ws_obj.onmessage = function (evt) {
            var res = "";
            if (evt.data != "") {
                res = JSON.parse(evt.data);
            }
            if (res['set-cookie']) {
                document.cookie = res['set-cookie'];
            }
            //登录失败
            //            if(res['loginError'])
            //           {
            //              alert(res['loginError']);
            //         }
            var call_cmd_id = res['call_cmd_id'];
            if (!call_cmd_id) {
                return;
            }
            var execFunc = o.ws_queue_list[call_cmd_id];
            if (typeof (execFunc) != 'function') {
                return;
            }
            var ctx = o.ws_queue_ctx[res['call_cmd_id']];
            ctx = ctx || {
                returnType: "string"
            };
            var ret;
            if (ctx.returnType == "bool") {
                ret = res.retVal == "true" ? true : false;
            } else if (ctx.returnType == "number") {
                ret = Number(res.retVal);
            } else {
                ret = res.retVal;
            }
            var retObj = {
                retVal: ret,
                ctx: ctx
            };
            execFunc(retObj);
            if (res['call_cmd_id'] != "onUsbkeyChange") {
                delete o.ws_queue_list[res['call_cmd_id']];
            }
            delete o.ws_queue_ctx[res['call_cmd_id']];
        };
        o.ws_obj.onerror = function (evt) {
            o.load_websocket();
        };
        return true;
    };
    o.sendMessage = function (sendMsg) {
        if (o.ws_obj.readyState == WebSocket.OPEN) {
            o.ws_obj.send(JSON.stringify(sendMsg));
        } else {
            console.log("Can't connect to WebSocket server!");
        }
    };
    o.callMethod = function (strMethodName, cb, ctx, returnType, argsArray) {
        o.ws_queue_id++;
        if (typeof (cb) == 'function') {
            o.ws_queue_list['i_' + o.ws_queue_id] = cb;
            ctx = ctx || {};
            ctx.returnType = returnType;
            o.ws_queue_ctx['i_' + o.ws_queue_id] = ctx;
        }
        var sendArray = {};
        sendArray['cookie'] = document.cookie;
        sendArray['websign_func_name'] = strMethodName;
        //get sessionid
        sendArray['call_cmd_id'] = 'i_' + o.ws_queue_id;
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
	
	o.Verify = function (plainstring, signDataString, cb, ctx) {
        var paramArray = [plainstring, signDataString];
        ctx = null;
        returnType = null;
        o.callMethod('ESeaL_Verify', cb, ctx, returnType, paramArray);
    };
    o.ESeaL_ShowUserCerInfo = function (signDataString, cb, ctx) {
        var paramArray = [signDataString];
        ctx = null;
        returnType = null;
        o.callMethod('ESeaL_ShowUserCerInfo', cb, ctx, returnType, paramArray);
    };
    o.ShowSignTimeEx = function (signDataString, cb, ctx) {
        var paramArray = [signDataString];
        ctx = null;
        returnType = null;
        o.callMethod('ESeaL_ShowSignTime', cb, ctx, returnType, paramArray);
    };
    o.GetVersion = function (cb, ctx) {
        var paramArray = "";
        ctx = null;
        returnType = null;
        o.callMethod('ESeaL_GetGetVersion', cb, ctx, returnType, paramArray);
    };
    o.ESeaL_CheckRemoveRight = function (certid, signDataString, cb, ctx) {
        var paramArray = [certid, signDataString];
        ctx = null;
        returnType = null;
        o.callMethod('ESeaL_CheckRemoveRight', cb, ctx, returnType, paramArray);
    };
    o.GetStampPicAfterVerified = function (bVerify, signature, cb, ctx) {
        var paramArray = [bVerify, signature];
        ctx = null;
        returnType = null;
        o.callMethod('ESeaL_GetStampPicAfterVerified', cb, ctx, returnType, paramArray);
    };
    o.GetLastErr = function (cb, ctx) {
        var paramArray = "";
        ctx = null;
        returnType = null;
        o.callMethod('ESeaL_GetLastErr', cb, ctx, returnType, paramArray);
    };
	o.SetSignTypeAndURL = function (iSignType, url, cb, ctx) {
        var paramArray = [iSignType, url];
        ctx = null;
        returnType = null;
        o.callMethod('ESeaL_SetSignTypeAndURL', cb, ctx, returnType, paramArray);
    };
	o.GetSealList = function (certid, cb, ctx) {
        var paramArray = [certid];
        ctx = null;
        returnType = null;
        o.callMethod('ESeaL_GetSealList', cb, ctx, returnType, paramArray);
    };
	o.ESeaL_GetStampPicForAll = function (certid, sealNum, cb, ctx) {
        var paramArray = [certid, sealNum];
        ctx = null;
        returnType = null;
        o.callMethod('ESeaL_GetStampPicForAll', cb, ctx, returnType, paramArray);
    };
	o.ESeaL_Sign = function (strCertID, plainstring, cb, ctx) {
        var paramArray = [strCertID, plainstring];
        ctx = null;
        returnType = null;
        o.callMethod('ESeaL_Sign', cb, ctx, returnType, paramArray);
    };
    o.ESeaL_BatchSign = function (strCertID, plainstring, cb, ctx) {
        var paramArray = [strCertID, plainstring];
        ctx = null;
        returnType = null;
        o.callMethod('ESeaL_BatchSign', cb, ctx, returnType, paramArray);
    };
    o.ESeaL_GetSignMethod = function (signDataString, cb, ctx) {
        var paramArray = [signDataString];
        ctx = null;
        returnType = null;
        o.callMethod('ESeaL_GetSignMethod', cb, ctx, returnType, paramArray);
    };
	
	 if (!o.load_websocket()) {
        return null;
    }

	return o;
}

(function $onLoadFunc() {
	$_$WebSign_WebSocketObj = CreateWebSocketObject_WebSign();
    if ($_$WebSign_WebSocketObj != null) {
        $_$WebSign_CurrentObj = $_$WebSign_WebSocketObj;
        return;
    }

	$_$WebSign_CurrentObj = null;
	//alert("初始化签章核心服务出错!");
	return;
})();



