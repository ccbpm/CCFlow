var $_$WebSocketConnectState = false;

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
 * 获取印章图片
 */
function ESeal_GetStampPic(cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.GetStampPicForAllBrowser != undefined) {
		return $_$WebSign_CurrentObj.GetStampPicForAllBrowser(cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}

/**
 * 获取印章图片宽高
 */
function ESeal_GetStampPicWH(cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.GetPicWidthPx != undefined) {
		return $_$WebSign_CurrentObj.GetPicWidthPx(cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}

}

/**
 * 获取签章算法
 */
function ESeal_GetSignMethod(cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.GetSignMethod != undefined) {
		return $_$WebSign_CurrentObj.GetSignMethod(cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}

/**
 * 设置签章类型和url（type可为空，url）
 */
function ESeaL_SetSignTypeAndURL(iType, webserviceUrl, cb) {
	if(webserviceUrl == "" || webserviceUrl == null) {
		return ESeaL_ErrorRtnFunc("", cb);
	}
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.SetWebServiceURL != undefined) {
		return $_$WebSign_CurrentObj.SetWebServiceURL(webserviceUrl, cb, null);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}

/**
 * 签章
 * @param orgData:原文
 */
function ESeaL_Sign(orgData, cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.SignForAllBrowser != undefined) {
		return $_$WebSign_CurrentObj.SignForAllBrowser(orgData, cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}

/**
 * 批量签章
 * @param orgData:原文
 */
function ESeaL_BatchSign(orgData, cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.BatchSignForAllBrowser != undefined) {
		return $_$WebSign_CurrentObj.BatchSignForAllBrowser(orgData, cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}

/**
 * 验章
 * @param org_data 原文
 * @param signdata 签章值
 */
function ESeal_Verify(org_data, signdata, cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.Verify != undefined) {
		return $_$WebSign_CurrentObj.Verify(org_data, signdata, cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}

/**
 * 验章
 */
function ESeal_GetStampPicAfterVerified(cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.GetStampPicAfterVerified != undefined) {
		return $_$WebSign_CurrentObj.GetStampPicAfterVerified(cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}

/**
 * 验章
 */
function ESeal_GetStampPicAfterVerifiedEx(sVerify, sSignature, cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.GetStampPicAfterVerifiedEx != undefined) {
		return $_$WebSign_CurrentObj.GetStampPicAfterVerifiedEx(sVerify, sSignature, cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}

/**
 * 撤章
 * @param signdata:签章值
 */
function ESeaL_CheckRemoveRight(signdata, cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.CheckRemoveRight != undefined) {
		return $_$WebSign_CurrentObj.CheckRemoveRight(signdata, cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}
/**
 * 显示证书信息
 * @param signdata:原文
 */
function Eseal_ShowUserCerInfo(signdata, cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.ShowSignerCertInfoEx != undefined) {
		return $_$WebSign_CurrentObj.ShowSignerCertInfoEx(signdata, cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}
/**
 * 显示签章时间
 * @param signdata:签章值
 */
function Eseal_ShowSignTime(signdata, cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.ShowSignTimeEx != undefined) {
		return $_$WebSign_CurrentObj.ShowSignTimeEx(signdata, cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}
/**
 * 撤销签章
 * @param signdata:签章值
 */
function ESeaL_RemoveSeal(signdata, cb) {
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.CheckRemoveRight != undefined) {
		return $_$WebSign_CurrentObj.CheckRemoveRight(signdata, cb);
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
	if ($_$WebSign_CurrentObj != null && $_$WebSign_CurrentObj.GetLastErr != undefined) {
		return $_$WebSign_CurrentObj.GetLastErr(cb);
	} else {
		return ESeaL_ErrorRtnFunc($_$METHOD_NOT_EXIST, cb);
	}
}

function $checkBrowserISIE() {
	return !!window.ActiveXObject || "ActiveXObject" in window ? true : false;
}

/**
 * IE11下注册监听函数
 * @param strObjName 控件对象名称
 * @param eventName 事件名称
 * @param callbackFunName 回调方法名称
 */
function $AttachForIE11Event(strObjName, eventName, callbackFunName) {
	var handler = document.createElement("script");
	handler.setAttribute("for", strObjName);
	handler.setAttribute("event", eventName);
	handler.appendChild(document.createTextNode(callbackFunName));
	document.body.appendChild(handler);
}
/**
 * 加载一个控件
 * @param CLSID 控件的CLSID
 * @param ctlName 控件对象名称
 * @param testFuncName 测试方法名称
 * @param addEvent 是否增加回调函数
 * @param OnSignCallbackFun 签章回调函数
 * @param OnSignCallbackFunString 签章回调函数字符串
 * @param OnVerifyCallbackFun 验章回调函数
 * @param OnVerifyCallbackFunString 验章回调函数字符串
 * @param OnSignRemovedCallbackFun 撤章回调函数
 * @param OnSignRemovedCallbackFunString 撤章回调函数字符串
 */
function $LoadControl(
	CLSID,
	ctlName,
	testFuncName,
	addEvent,
	OnSignCallbackFun,
	OnSignCallbackFunString,
	OnVerifyCallbackFun,
	OnVerifyCallbackFunString,
	OnSignRemovedCallbackFun,
	OnSignRemovedCallbackFunString) {
	var pluginDiv = document.getElementById("pluginDiv" + ctlName);
	if (pluginDiv) {
		//return true;
		document.body.removeChild(pluginDiv);
		pluginDiv.innerHTML = "";
		pluginDiv = null;
	}
	pluginDiv = document.createElement("div");
	pluginDiv.id = "pluginDiv" + ctlName;
	document.body.appendChild(pluginDiv);
	try {
		if ($checkBrowserISIE()) {
			// IE
			if (window.navigator.platform == "Win32")
				//codeBase="BJCAWebSign.CAB#version=4,1,0,0"
				pluginDiv.innerHTML =
					'<object id="' +
					ctlName +
					'" classid="CLSID:' +
					CLSID +
					'" codeBase="BJCAWebSign.CAB" style="POSITION: absolute; TOP: 10px; LEFT: 10px;"> <PARAM NAME="Visible" VALUE="false"> </object>';
			else
				pluginDiv.innerHTML =
					'<object id="' +
					ctlName +
					'" classid="CLSID:' +
					CLSID +
					'" codeBase="BJCAWebSignX64.CAB" style="POSITION: absolute; TOP: 10px; LEFT: 10px;"> <PARAM NAME="Visible" VALUE="false"> </object>';
			if (addEvent) {
				var clt = eval(ctlName);
				if (clt.attachEvent) {
					clt.attachEvent("OnSign", OnSignCallbackFun);
					clt.attachEvent("OnVerify", OnVerifyCallbackFun);
					clt.attachEvent("OnSignRemoved", OnSignRemovedCallbackFun);
				} else {
					// IE11 not support attachEvent, and addEventListener do not work well, so addEvent ourself
					$AttachForIE11Event(ctlName, "OnSign", OnSignCallbackFunString);
					$AttachForIE11Event(ctlName, "OnVerify", OnVerifyCallbackFunString);
					$AttachForIE11Event(ctlName, "OnSignRemoved", OnSignRemovedCallbackFunString);
					//clt.addEventListener("OnUsbKeyChange", $OnUsbKeyChange, false);
				}
			}
		} else {
			//luoxing 不适用npapi调用
			return false;
		}
		if (testFuncName != null && testFuncName != "") {
			if (eval(ctlName + "." + testFuncName) == undefined) {
				document.body.removeChild(pluginDiv);
				pluginDiv.innerHTML = "";
				pluginDiv = null;
				return false;
			}
		}
		return true;
	} catch (e) {
		document.body.removeChild(pluginDiv);
		pluginDiv.innerHTML = "";
		pluginDiv = null;
		return false;
	}
}
/**
 * 创建网页签章控件对象,定义控件对象中的各个方法
 * @param objectIDString 提示框显示的信息内容
 * @param OnSignCallbackFun 签章回调函数
 * @param OnSignCallbackFunString 签章回调函数字符串
 * @param OnVerifyCallbackFun 验章回调函数
 * @param OnVerifyCallbackFunString 验章回调函数字符串
 * @param OnSignRemovedCallbackFun 撤章回调函数
 * @param OnSignRemovedCallbackFunString 撤章回调函数字符串
 */
function CreateAppObject_WebSign(
	objectIDString,
	OnSignCallbackFun,
	OnSignCallbackFunString,
	OnVerifyCallbackFun,
	OnVerifyCallbackFunString,
	OnSignRemovedCallbackFun,
	OnSignRemovedCallbackFunString) {
	var bOK = $LoadControl(
			"820390E5-1C07-483D-AEED-6A0EDF640AA2",
			objectIDString,
			null,
			false,
			OnSignCallbackFun,
			OnSignCallbackFunString,
			OnVerifyCallbackFun,
			OnVerifyCallbackFunString,
			OnSignRemovedCallbackFun,
			OnSignRemovedCallbackFunString);
	if (!bOK) {
		return null;
	}
	console.log("CreateAppObject_WebSign ok");
	$_$WebSocketConnectState = true;
	var o = new Object();
	var clt = eval(objectIDString);
	o.Sign = function (plainstring, cb) {
		var ret = clt.Sign(plainstring);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.Verify = function (plainstring, signDataString, cb) {
		var ret = clt.Verify(plainstring, signDataString);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	/*
	o.ConvertSampleSeal = function() {
	return clt.RemoveSign();
	};
	 */
	o.SetCtrlPos = function (x, y, cb) {
		var ret = clt.SetCtrlPos(x, y);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.SetOffsetPos = function (posRelativeElementIDString, x, y, cb) {
		var ret = clt.SetOffsetPos(posRelativeElementIDString, x, y);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.SetDisplayRect = function (left, top, width, height, cb) {
		var ret = clt.SetDisplayRect(left, top, width, height);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.ShowLastVerifyResult = function (cb) {
		var ret = clt.ShowLastVerifyResult();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.GetSignature = function (cb) {
		var ret = clt.GetSignature();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.SetSignature = function (signValue, cb) {
		var ret = clt.SetSignature(signValue);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.IsSigned = function (cb) {
		var ret = clt.IsSigned();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.SetVisible = function (bVisible, cb) {
		var ret = clt.SetVisible(bVisible);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.GetVisible = function (cb) {
		var ret = clt.GetVisible();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.IsKeyReady = function (cb) {
		var ret = clt.IsKeyReady();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.SignFormFields = function (formname, elementname, bsilence, cb) {
		var ret = clt.SignFormFields(formname, elementname, bsilence);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.VerifyFormFields = function (cb) {
		var ret = clt.VerifyFormFields();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.GetSignTime = function (cb) {
		var ret = clt.GetSignTime();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.GetStampPicAfterVerified = function (cb) {
		var ret = clt.GetStampPicAfterVerified();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.GetStampPicAfterVerifiedEx = function (sVerify, sSignature, cb) {
		var ret = clt.GetStampPicAfterVerifiedEx(sVerify, sSignature);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	// liyuan for support online signature
	o.SetWebServiceURL = function (url, cb) {
		var ret = clt.SetWebServiceURL(url);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.ShowSignerCertInfo = function (cb) {
		var ret = clt.ShowSignerCertInfo();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.RemoveESign = function (bAlertMsg, cb) {
		var ret = clt.RemoveESign(bAlertMsg);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.ShowSignerCertInfoEx = function (strSignature, cb) {
		var ret = clt.ShowSignerCertInfoEx(strSignature);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.AboutBox = function (cb) {
		var ret = "";
		clt.AboutBox();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.GetStampPicFromUKey = function (strCertID, cb) {
		var ret = clt.GetStampPicFromUKey(strCertID);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.ShowSignTimeEx = function (strSignature, cb) {
		var ret = clt.ShowSignTimeEx(strSignature);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.CheckRemoveRight = function (strSignature, cb) {
		var ret = clt.CheckRemoveRight(strSignature);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.SignEx = function (strCertID, plainstring, bAlertMsg, cb) {
		var ret = clt.SignEx(strCertID, plainstring, bAlertMsg);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.GetVersion = function (cb) {
		var ret = clt.GetVersion();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.GetLastErr = function (cb) {
		var ret = clt.GetLastErr();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.GetSignMethod = function (cb) {
		var ret = clt.GetSignMethod();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.GetStampPicForAllBrowser = function (cb) {
		var ret = clt.GetStampPicForAllBrowser();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.SignForAllBrowser = function (plainstring, cb) {
		var ret = clt.SignForAllBrowser(plainstring);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.GetPicWidthPx = function (cb) {
		var ret = clt.GetPicWidthPx();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.GetPicHeightPx = function (cb) {
		var ret = clt.GetPicHeightPx();
		return ESeaL_OKRtnFunc(ret, cb);
	};
	o.BatchSignForAllBrowser = function (plainstring, cb) {
		var ret = clt.BatchSignForAllBrowser(plainstring);
		return ESeaL_OKRtnFunc(ret, cb);
	};
	
	return o;
}
/**
 * 创建网页签章webSocket对象
 */
//webSocket client class
function CreateWebSocketObject_WebSign() {

	var o = new Object();

	var config = {};
	var config_index = 0;
	//var socket;
	o.socket = new WebSocket("ws://127.0.0.1:21051/websign/");
	o.socket.onopen = function (evt) { $_$WebSocketConnectState = true; };

	o.socket.onmessage = function (evt) {		
		var res = "";
		if (evt.data != "") {
			res = JSON.parse(evt.data);
		}
		var call_cmd_id = res['call_cmd_id'];
		if (!call_cmd_id) {
			// 低版本IE不支持日志，需要调试时自行打开
			//console.error("server return error", evt.data);
			return;
		}

		var callback = config[call_cmd_id];
		if (callback && typeof callback == "function") {
			callback(res);
		}

		delete config[call_cmd_id];
	}

	o.callMethod = function (strMethodName, callback, argsArray) {
		config_index++;
		if (callback) {
			config[config_index] = callback;
		}

		var sendArray = {};
		sendArray['xtx_func_name'] = strMethodName;
		sendArray['call_cmd_id'] = config_index;
		sendArray['ocx_id'] = "{820390E5-1C07-483D-AEED-6A0EDF640AA2}";

		if (arguments.length > 1) {
			sendArray["param"] = argsArray;
		}
		o.socket.send(JSON.stringify(sendArray));
	}

	o.Verify = function (sData, sSignature, callback) {
		o.callMethod("Verify", callback, [sData, "BSTR", sSignature, "BSTR"]);
	};

	o.SetWebServiceURL = function (sWSURL, callback) {
		o.callMethod("SetWebServiceURL", callback, [sWSURL, "BSTR"]);
	};

	o.GetStampPicAfterVerified = function (callback) {
		o.callMethod("GetStampPicAfterVerified", callback);
	};
	
	o.GetStampPicAfterVerifiedEx = function (sVerify, sSignature, callback) {
		o.callMethod("GetStampPicAfterVerifiedEx", callback, [sVerify, "BSTR", sSignature, "BSTR"]);
	};

	o.ShowSignerCertInfoEx = function (sSignature, callback) {
		o.callMethod("ShowSignerCertInfoEx", callback, [sSignature, "BSTR"]);
	};

	o.ShowSignTimeEx = function (sSignature, callback) {
		o.callMethod("ShowSignTimeEx", callback, [sSignature, "BSTR"]);
	};

	o.CheckRemoveRight = function (sSignature, callback) {
		o.callMethod("CheckRemoveRight", callback, [sSignature, "BSTR"]);
	};

	//BSTR SignEx(BSTR sCertID, BSTR sData, boolean bAlertMsg);
	o.SignEx = function (sCertID, sData, bAlertMsg, callback) {
		o.callMethod("SignEx", callback, [sCertID, "BSTR", sData, "BSTR", bAlertMsg, "boolean"]);
	};

	//BSTR GetVersion();
	o.GetVersion = function (callback) {
		o.callMethod("GetVersion", callback);
	};

	//BSTR GetLastErr();
	o.GetLastErr = function (callback) {
		o.callMethod("GetLastErr", callback);
	};

	//BSTR GetSignMethod();
	o.GetSignMethod = function (callback) {
		o.callMethod("GetSignMethod", callback);
	};

	//long SetSignMethod(BSTR sSignMethod);
	o.SetSignMethod = function (sSignMethod, callback) {
		o.callMethod("SetSignMethod", callback, [sSignMethod, "BSTR"]);
	};

	// BSTR GetStampPicForAllBrowser();
	o.GetStampPicForAllBrowser = function (callback) {
		o.callMethod("GetStampPicForAllBrowser", callback);
	};

	// BSTR SignForAllBrowser(BSTR sData);
	o.SignForAllBrowser = function (sData, callback) {
		o.callMethod("SignForAllBrowser", callback, [sData, "BSTR"]);
	};

	o.GetPicWidthPx = function (callback) {
		o.callMethod("GetPicWidthPx", callback);
	};

	o.GetPicHeightPx = function (callback) {
		o.callMethod("GetPicHeightPx", callback);
	};
	
	o.BatchSignForAllBrowser = function (sData, callback) {
		o.callMethod("BatchSignForAllBrowser", callback, [sData, "BSTR"]);
	};
	
	return o;
}

(function $onLoadFunc() {
	// liyuan，优先按照IE加载控件的方式
	// console.log("$onLoadFunc in");
	$_$WebSign_AppObj = CreateAppObject_WebSign("SignObj", null, null, null, null, null, null);
	if ($_$WebSign_AppObj != null) {
		console.log("CreateAppObject_WebSign success");
		$_$WebSign_CurrentObj = $_$WebSign_AppObj;
		return;
	}

	// liyuan，如果按照IE加载控件的方式失败，则按照websocket方式加载控件
	$_$WebSign_WebSocketObj = CreateWebSocketObject_WebSign();
	if ($_$WebSign_WebSocketObj != null) {
		// console.log(" CreateWebSocketObject_WebSign success");
		$_$WebSign_CurrentObj = $_$WebSign_WebSocketObj;
		return;
	}

	$_$WebSign_CurrentObj = null;
	//$XTXAlert("检查签章核心服务出错!");
	alert("初始化签章核心服务出错!");
	return;
})();