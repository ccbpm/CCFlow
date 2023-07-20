/*bjca_zhzx 2021-2-19
 接口层（注：所有接口都是异步调用）
*/
var $_$is_windows = true;//平台(win/linux)
var $_$is_login = false;//是否登录
var $_$bPos = true;//是否可拖动印章，true：不可拖拽印章；false：可拖拽印章

var $_$CurStampID = -1;//印章页面ID
var $_$CurContainerName = "";//签名容器
var $_$CurOrgData = [];//签名原文
var $_$CurSignData = [];//签名值
var $_$CurSignMethod = [];//签名算法
var $_$CurSealID = [];//印章ID，仅对信创平台有效
var $_$CurCertID = [];//证书ID，仅对信创平台有效

//原文DIV前缀，页面原文的DIV需要用“前缀+印章页面ID”的方式命名，否则无法使用右键验签
var $_$OrgDataContainerNameHead = "originData";

//印章偏移信息，仅对拖拽签章有效
var $_$CurDragPosX = 0;
var $_$CurDragPosY = 0;

//印章页面结构信息
var $_$Seal_Pic_Main_Div_Name = "";
var $_$Seal_Pic_Div_Name = "";
var $_$Seal_Img_Name = "";

//信创平台证书选择框
var userInfoDialogStr = '<div class="cert-info-modal">'
    +'<div class="win-modal-title">请选择证书和印章</div>'
    +'<div class="win-modal-cont">'
      +'<div class="win-modal-ele" id="certEle">'
        +'<div class="win-modal-cont-left">证书ID</div>'
        +'<div class="win-modal-cont-right">'
          +'<select id="certSelect" class="info-select"></select>'
        +'</div>'
      +'</div>'
      +'<div class="win-modal-ele" id="pinEle">'
        +'<div class="win-modal-cont-left">输入密码</div>'
        +'<div class="win-modal-cont-right">'
          +'<input type="password" name="" class="info-input" id="pinInput" placeholder="请输入证书PIN码">'
        +'</div>'
      +'</div>'
      +'<div class="win-modal-ele" id="sealEle" style="display: none;">'
        +'<div class="win-modal-cont-left">印章ID</div>'
        +'<div class="win-modal-cont-right">'
          +'<select id="sealSelect" class="info-select"></select>'
        +'</div>'
      +'</div>'
    +'</div>'
    +'<div class="win-modal-bottom">'
      +'<button class="win-modal-btn confirm-btn" onclick="pinConfirm()" id="pinConfirmBtn" style="display: block;">登录</button>'
      +'<button class="win-modal-btn confirm-btn" onclick="infoConfirm()" id="infoConfirmBtn" style="display: none;">确定</button>'
      +'<button class="win-modal-btn cancel-btn" onclick="infoCancel()">取消</button>'
    +'</div>'
  +'</div>';

//信创平台证书选择框回调
var userInfoDialogCallback = null;

//等待提示框
var loadingViewStr = '<div class="hint-modal">'
      +'<img class="hint-img" src="data:image/gif;base64,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"><br />'
      +'<span class="hint-txt">正在操作，请等待……</span>'
    +'</div>';

/******************************初始化接口******************************/

/**
 * 引入不同平台的js
 */
function initJs(){
		//常量文件
	script = document.createElement("script");
	script.src = "/DataUser/OverrideFiles/WorkCheck/qianzhang/static/js/bjca/config.js";
	document.getElementsByTagName("head")[0].appendChild(script);
		//回调函数文件，用户可根据需求配置
	script = document.createElement("script");
	script.src = "/DataUser/OverrideFiles/WorkCheck/qianzhang/static/js/bjca/BJCAWebSignCallback.js";
	document.getElementsByTagName("head")[0].appendChild(script);
		//拖拽签章文件，需要拖拽功能的时候引入
	script = document.createElement("script");
	script.src = "/DataUser/OverrideFiles/WorkCheck/qianzhang/static/js/bjca/DragSeal.js";
	document.getElementsByTagName("head")[0].appendChild(script);
		//右键菜单文件，需要使用右键执行验证签章、撤销签章等功能的时候引入
	script = document.createElement("script");
	script.src = "/DataUser/OverrideFiles/WorkCheck/qianzhang/static/js/bjca/PopMenu.js";
	document.getElementsByTagName("head")[0].appendChild(script);

	if($_$is_windows){
		//windows
		script = document.createElement("script");
		script.src = "/DataUser/OverrideFiles/WorkCheck/qianzhang/static/js/bjca/BJCAWebSignForWindows.js";
		document.getElementsByTagName("head")[0].appendChild(script);
	}else{		
		//信创
		script = document.createElement("script");
		script.src = "/DataUser/OverrideFiles/WorkCheck/qianzhang/static/js/bjca/BJCAWebSignWithinXTXSAB.js";
		document.getElementsByTagName("head")[0].appendChild(script);
		
		script = document.createElement("script");
		script.src = "../Scripts/qianzhang/static/js/bjca/BJCAWebSignForLinux.js";
		document.getElementsByTagName("head")[0].appendChild(script);

		var loadingView = document.createElement("div");
		loadingView.setAttribute("id", "loadingView");
		loadingView.className = "win-modal";

		document.getElementsByTagName('body')[0].appendChild(loadingView);
		document.getElementById("loadingView").innerHTML = loadingViewStr;

		var userCertInfo = document.createElement("div");
		userCertInfo.setAttribute("id", "userCertInfo");
		userCertInfo.className = "win-modal";

		document.getElementsByTagName('body')[0].appendChild(userCertInfo);

		document.getElementById("userCertInfo").innerHTML = userInfoDialogStr;
	}
}

/**
 * 初始化
 * @param UserListID 用户列表选择框id
 */
function BWS_Init() {
	
	// $_$is_windows = false;
	$_$is_windows = (navigator.platform == "Win32") || (navigator.platform == "Windows");
	initJs();
	setTimeout(function(){
		//设置签章类型和url，0：离线，1：国密在线，2：国办在线验证印章状态
		ESeaL_SetSignTypeAndURL($_$WEBSIGN_CONFIG.$_$sign_type, $_$WEBSIGN_CONFIG.$_$url, function(ret){return;});
	}, 500);

	if(!$_$is_windows){	
		//初始化方法中调用接口需要加等待时间
		setTimeout(function(){
			ESeaL_SetUserConfig();//先设置可信任根；
		}, 500);
	}
}

/******************************通用接口******************************/
/**
 * 批量签章
 * @param strCertID 证书id
 * @param strSealID 印章id
 * @param orgdata 原文(多个原文以json格式拼接)
 * @param cb 回调函数。成功返回签章值signData(多个签章数据以json格式拼接)和印章图片picData，失败返回空
 */
function BWS_BatchSign(strCertID, strSealID, orgdata, cb)
{
	if($_$is_windows) {//windows
		ESeal_GetStampPic(function(ret){
			if(ret.retVal == "") {
				//获取错误信息，并弹框显示
				$GetLastErrJS();
				return ESeaL_ErrorRtnFunc("", cb);
			} else if(ret.retVal == $_$METHOD_NOT_EXIST) {
				//当前接口不存在，客户端版本不匹配
				alert($_$METHOD_NOT_EXIST);
				return ESeaL_ErrorRtnFunc("", cb);
			} else {
				//返回印章图片数据
				ESeaL_BatchSign(orgdata, function(ret2){
					if(ret2.retVal == "") {
						//获取错误信息，并弹框显示
						$GetLastErrJS();
						return ESeaL_ErrorRtnFunc("", cb);
					} else if(ret2.retVal == $_$METHOD_NOT_EXIST) {
						//当前接口不存在，客户端版本不匹配
						alert($_$METHOD_NOT_EXIST);
						return ESeaL_ErrorRtnFunc("", cb);
					} else {
						//返回签章数据
						if (typeof cb == 'function') {
							var retObj = {signData:ret2.retVal, picData:ret.retVal};
							cb(retObj);
						}
						return;
					}					
				});
			}
		});
	} else {//信创
		document.getElementById("loadingView").style.display = "block";
		SOF_IsLogin_BJCAWebSign(strCertID, function (retLogin) {
			if (retLogin.retVal) {
				ESeaL_GetStampPic(strCertID, strSealID, function (ret) {
					if (ret.retVal == "") {
						document.getElementById("loadingView").style.display = "none";
						//获取错误信息，并弹框显示
						$GetLastErrJS();
						return ESeaL_ErrorRtnFunc("", cb);
					} else if(ret.retVal == $_$METHOD_NOT_EXIST) {
						document.getElementById("loadingView").style.display = "none";
						//当前接口不存在，客户端版本不匹配
						alert($_$METHOD_NOT_EXIST);
						return ESeaL_ErrorRtnFunc("", cb);
					} else {
						//批量签章
						ESeaL_BatchSign(strCertID, orgdata, function(ret2){
							document.getElementById("loadingView").style.display = "none";
							if(ret2.retVal == "") {
								//获取错误信息，并弹框显示
								$GetLastErrJS();
								return ESeaL_ErrorRtnFunc("", cb);
							} else if(ret2.retVal == $_$METHOD_NOT_EXIST) {
								//当前接口不存在，客户端版本不匹配
								alert($_$METHOD_NOT_EXIST);
								return ESeaL_ErrorRtnFunc("", cb);
							} else {
								//返回签章数据
								if (typeof cb == 'function') {
									var retObj = {signData:ret2.retVal, picData:ret.retVal};
									cb(retObj);
								}
								return;	
							}	
						});
					}
				});
			} else {
				document.getElementById("loadingView").style.display = "none";
				alert("请先登录！");
				return ESeaL_ErrorRtnFunc("", cb);
			}
		}, null);
	}
}

/**
 * 直接签章
 * @param strCertID 证书id
 * @param strSealID 印章id
 * @param orgdata 原文
 * @param cb 回调函数。成功返回签章值signData和印章图片picData，失败返回空
 */
function BWS_DirectSign(strCertID, strSealID, orgdata, cb)
{
	//debugger
	if($_$is_windows) {//windows
		ESeal_GetStampPic(function(ret){
			if(ret.retVal == "") {
				//获取错误信息，并弹框显示
				$GetLastErrJS();
				return ESeaL_ErrorRtnFunc("", cb);
			} else if(ret.retVal == $_$METHOD_NOT_EXIST) {
				//当前接口不存在，客户端版本不匹配
				alert($_$METHOD_NOT_EXIST);
				return ESeaL_ErrorRtnFunc("", cb);
			} else {
				//返回印章图片数据
				ESeaL_Sign(orgdata, function(ret2){
					if(ret2.retVal == "") {
						//获取错误信息，并弹框显示
						$GetLastErrJS();
						return ESeaL_ErrorRtnFunc("", cb);
					} else if(ret2.retVal == $_$METHOD_NOT_EXIST) {
						//当前接口不存在，客户端版本不匹配
						alert($_$METHOD_NOT_EXIST);
						return ESeaL_ErrorRtnFunc("", cb);
					} else {
						// console.log("BWS_Sign:"+ret2.retVal);
						//返回签章数据
						if (typeof cb == 'function') {
							var retObj = {signData:ret2.retVal, picData:ret.retVal};
							cb(retObj);
						}
					}
					return;				
				});
			}

		});
	} else {//信创
		document.getElementById("loadingView").style.display = "block";
		SOF_IsLogin_BJCAWebSign(strCertID, function (retLogin) {
			if (retLogin.retVal) {
				ESeaL_GetStampPic(strCertID, strSealID, function (ret) {
					if (ret.retVal == "") {
						document.getElementById("loadingView").style.display = "none";
						//获取错误信息，并弹框显示
						$GetLastErrJS();
						return ESeaL_ErrorRtnFunc("", cb);
					} else if(ret.retVal == $_$METHOD_NOT_EXIST) {
						document.getElementById("loadingView").style.display = "none";
						//当前接口不存在，客户端版本不匹配
						alert($_$METHOD_NOT_EXIST);
						return ESeaL_ErrorRtnFunc("", cb);
					} else {
						ESeaL_Sign(strCertID, orgdata, function(ret2){
							document.getElementById("loadingView").style.display = "none";
							if(ret2.retVal == "") {
								//获取错误信息，并弹框显示
								$GetLastErrJS();
								return ESeaL_ErrorRtnFunc("", cb);
							}
							if (typeof cb == 'function') {
								var retObj = {signData:ret2.retVal, picData:ret.retVal};
								cb(retObj);
							}
							return;					
						});
					}
				});
			} else {
				document.getElementById("loadingView").style.display = "none";
				alert("请先登录！");
				return ESeaL_ErrorRtnFunc("", cb);
			}
		}, null);
	}
}

/**
 * 签章
 * @param strCertID 证书id
 * @param orgdata 原文
 * @param cb 回调函数。成功返回签章值signData，失败返回空
 */
function BWS_Sign(strCertID, orgdata, cb)
{	
	if($_$is_windows) {//windows
		ESeaL_Sign(orgdata, function(ret2){
			if(ret2.retVal == "") {
				//获取错误信息，并弹框显示
				$GetLastErrJS();
				return ESeaL_ErrorRtnFunc("", cb);
			} else if(ret2.retVal == $_$METHOD_NOT_EXIST) {
				//当前接口不存在，客户端版本不匹配
				alert($_$METHOD_NOT_EXIST);
				return ESeaL_ErrorRtnFunc("", cb);
			} else {
				//返回签章值
				return ESeaL_OKRtnFunc(ret2.retVal, cb);
			}			
		});
	} else {//信创
		document.getElementById("loadingView").style.display = "block";
		SOF_IsLogin_BJCAWebSign(strCertID, function (retLogin) {
			if (retLogin.retVal) {
				ESeaL_Sign(strCertID, orgdata, function(ret2){
					document.getElementById("loadingView").style.display = "none";
					if(ret2.retVal == "") {
						//获取错误信息，并弹框显示
						$GetLastErrJS();
						return ESeaL_ErrorRtnFunc("", cb);
					}
					//返回签章值
					return ESeaL_OKRtnFunc(ret2.retVal, cb);				
				});
			} else {
				document.getElementById("loadingView").style.display = "none";
				alert("请先登录！");
				return ESeaL_ErrorRtnFunc("", cb);
			}
		}, null);
	}
}

/**
 * 验证
 * @param orgdata 原文
 * @param signdata 签章值
 * @param cb 回调函数。成功返回验证后的印章图片，失败返回空
 */
function BWS_Verify(orgdata, signdata, cb, bAlert)
{
	//debugger;
	var sInfo;
	var method_name = $_$CurSignMethod[$_$CurStampID];
	if($_$is_windows) {//windows平台
		//验证
		var sVerifyResult;
		ESeal_Verify(orgdata, signdata, function(ret){
			if (ret.retVal == $_$METHOD_NOT_EXIST) {
				//当前接口不存在，客户端版本不匹配
				alert($_$METHOD_NOT_EXIST);
				return ESeaL_ErrorRtnFunc("", cb);
			} else if (ret.retVal != 0) {
				//验证成功
				sVerifyResult = "true";
				sInfo = "验证成功，数据有效！\r\n" + "签名算法：" + method_name;
			} else {
				//验证失败
				sVerifyResult = "false";
				sInfo = "验证失败，数据无效！\r\n" + "签名算法：" + method_name;
			}
			//获取验证后的印章图片
			// console.log(sVerifyResult);
			ESeal_GetStampPicAfterVerifiedEx(sVerifyResult, signdata, function(ret2){ 
				if (ret2.retVal == $_$METHOD_NOT_EXIST) {
					//当前接口不存在，客户端版本不匹配
					alert($_$METHOD_NOT_EXIST);
					return ESeaL_ErrorRtnFunc("", cb);
				} else if (ret2.retVal != "") {
					//弹出验证信息
					if(bAlert) {
						alert(sInfo);
					}
					// console.log(sInfo);
					//返回印章图片
					return ESeaL_OKRtnFunc(ret2.retVal, cb);
				} else {
					//获取错误信息，并弹框显示
					$GetLastErrJS();
					return ESeaL_ErrorRtnFunc("", cb);
				}		
			})
		});
	} else {//信创平台
		//验证
		ESeal_Verify(orgdata, signdata, function(ret){
			if (ret.retVal == $_$METHOD_NOT_EXIST) {
				//当前接口不存在，客户端版本不匹配
				alert($_$METHOD_NOT_EXIST);
				return ESeaL_ErrorRtnFunc("", cb);
			} else if (ret.retVal == "true") {
				//验证成功
				sInfo = "验证成功，数据有效！\r\n" + "签名算法：" + method_name;
			} else {
				//验证失败
				sInfo = "验证失败，数据无效！\r\n" + "签名算法：" + method_name;
			}
			//获取验证后的印章图片
			ESeal_GetStampPicAfterVerified(ret.retVal, signdata, function(ret2){ 
				if (ret2.retVal == $_$METHOD_NOT_EXIST) {
					//当前接口不存在，客户端版本不匹配
					alert($_$METHOD_NOT_EXIST);
					return ESeaL_ErrorRtnFunc("", cb);
				} else if (ret2.retVal != "") {
					//弹出验证信息
					if(bAlert) {
						alert(sInfo);
					}
					//返回印章图片
					return ESeaL_OKRtnFunc(ret2.retVal, cb);
				} else {
					//获取错误信息，并弹框显示
					$GetLastErrJS();
					return ESeaL_ErrorRtnFunc("", cb);
				}		
			})
		});
	}
}

/**
 * 获取印章图片
 * @param strCertID 证书id
 * @param strSealID 印章id
 * @param cb 回调函数。成功返回印章图片picData，失败返回空
 */
function BWS_GetStampPic(strCertID, strSealID, cb)
{
	if($_$is_windows) {//windows
		ESeal_GetStampPic(function(ret){
			if(ret.retVal == "") {
				//获取错误信息，并弹框显示
				$GetLastErrJS();
				return ESeaL_ErrorRtnFunc("", cb);
			} else if(ret.retVal == $_$METHOD_NOT_EXIST) {
				//当前接口不存在，客户端版本不匹配
				alert($_$METHOD_NOT_EXIST);
				return ESeaL_ErrorRtnFunc("", cb);
			} else {
				//返回印章图片
				return ESeaL_OKRtnFunc(ret.retVal, cb);
			}

		});
	} else {//信创
		ESeaL_GetStampPic(strCertID, strSealID, function (ret) {
			if (ret.retVal == "") {
				//获取错误信息，并弹框显示
				$GetLastErrJS();
				return ESeaL_ErrorRtnFunc("", cb);
			} else if(ret.retVal == $_$METHOD_NOT_EXIST) {
				//当前接口不存在，客户端版本不匹配
				alert($_$METHOD_NOT_EXIST);
				return ESeaL_ErrorRtnFunc("", cb);
			} else {
				//返回印章图片
				return ESeaL_OKRtnFunc(ret.retVal, cb);
			}
		});
	}
}

/**
 * 撤销签章
 * @param signdata 签章值
 * @param strCertID 证书id 
 * @param cb 回调函数。成功返回true，失败返回false
 */
function BWS_Remove(signdata, strCertID, cb)
{
	// console.log("BWS_Remove:"+signdata);
	if($_$is_windows) {
		//windows平台
		//检查撤章权限
		ESeaL_RemoveSeal(signdata, function(ret){ 
			if(ret.retVal == "") {
				//获取错误信息，并弹框显示
				$GetLastErrJS();
				return ESeaL_ErrorRtnFunc(false, cb);
			} else if (ret.retVal == $_$METHOD_NOT_EXIST) {
				//当前接口不存在，客户端版本不匹配
				alert($_$METHOD_NOT_EXIST);
				return ESeaL_ErrorRtnFunc(false, cb);
			} else if(ret.retVal == "true") {
				//允许撤章
				return ESeaL_OKRtnFunc(true, cb);
			} else {
				//不允许撤章，弹出错误信息
				alert(ret.retVal); 
				return ESeaL_ErrorRtnFunc($_$REMOVE_NOT_ALLOWED, cb);
			}			
		});
	} else {
		//信创平台
		//检查撤章权限
		ESeaL_RemoveSeal(strCertID, signdata, function(ret){
			if(ret.isLogin) {
				//当前Ukey已登录
				if(ret.retVal == "") {
					//获取错误信息，并弹框显示
					$GetLastErrJS();
					return ESeaL_ErrorRtnFunc(false, cb);
				}  else if (ret.retVal == $_$METHOD_NOT_EXIST) {
					//当前接口不存在，客户端版本不匹配
					alert($_$METHOD_NOT_EXIST);
					return ESeaL_ErrorRtnFunc(false, cb);
				} else if (ret.retVal == "true") {
					//允许撤章
					return ESeaL_OKRtnFunc(true, cb);
				} else {
					//不允许撤章，弹出错误信息
					alert(ret.retVal); 
					return ESeaL_ErrorRtnFunc($_$REMOVE_NOT_ALLOWED, cb);
				}
			} else {
				//当前Ukey还没有登录，需要先登录
				alert("请先登录！");
				return ESeaL_ErrorRtnFunc($_$NOT_LOGIN, cb);
			}
			
		});
	}
}

/**
 * 获取签章时间
 * @param signdata 签章值
 * @param cb 回调函数。成功返回签章时间，失败返回空
 */
function BWS_GetSignTime(signdata, cb)
{
	Eseal_ShowSignTime(signdata, function(ret){
		if(ret.retVal == "") {
			//获取错误信息，并弹框显示
			$GetLastErrJS();
			return ESeaL_ErrorRtnFunc("", cb);
		} else if (ret.retVal == $_$METHOD_NOT_EXIST) {
			//当前接口不存在，客户端版本不匹配
			alert($_$METHOD_NOT_EXIST);
			return ESeaL_ErrorRtnFunc("", cb);
		} else {
			//显示签章时间
			//alert(ret.retVal); 
			return ESeaL_OKRtnFunc(ret.retVal, cb);
		}
	});
}

/**
 * 获取证书信息
 * @param signdata 签章值
 * @param cb 回调函数。成功返回证书信息，失败返回空
 */
function BWS_GetCertInfo(signdata, cb){
	Eseal_ShowUserCerInfo(signdata, function(ret){
		if(ret.retVal == "") {
			//获取错误信息，并弹框显示
			$GetLastErrJS();
			return ESeaL_ErrorRtnFunc("", cb);
		} else if (ret.retVal == $_$METHOD_NOT_EXIST) {
			//当前接口不存在，客户端版本不匹配
			alert($_$METHOD_NOT_EXIST);
			return ESeaL_ErrorRtnFunc("", cb);
		} else {
			//显示证书信息
			//alert(ret.retVal);
			return ESeaL_OKRtnFunc(ret.retVal, cb);
		}
	});
}

/**
 * 获取关于信息
 * @param cb 回调函数。成功返回签章时间，失败返回空
 */
function BWS_GetVersion(cb)
{
	ESeal_GetVersion(function(ret){
		if(ret.retVal == "") {
			//获取错误信息，并弹框显示
			$GetLastErrJS();
			return ESeaL_ErrorRtnFunc("", cb);
		} else if (ret.retVal == $_$METHOD_NOT_EXIST) {
			//当前接口不存在，客户端版本不匹配
			alert($_$METHOD_NOT_EXIST);
			return ESeaL_ErrorRtnFunc("", cb);
		} else {
			//显示证书信息
			//alert(ret.retVal);
			return ESeaL_OKRtnFunc(ret.retVal, cb);
		}
	});
}

/**
 * 获取签章算法
 * @param signdata 签章值
 * @param cb 回调函数。成功返回签章算法，失败返回空
 */
function BWS_GetSignMethod(signdata, cb)
{
//debugger
	if($_$is_windows) {//windows
		ESeal_GetSignMethod(function(ret){
			if(ret.retVal == "") {
				//获取错误信息，并弹框显示
				$GetLastErrJS();
				return ESeaL_ErrorRtnFunc("", cb);
			} else if(ret.retVal == $_$METHOD_NOT_EXIST) {
				//当前接口不存在，客户端版本不匹配
				alert($_$METHOD_NOT_EXIST);
				return ESeaL_ErrorRtnFunc("", cb);
			} else {
				//返回签名算法
				return ESeaL_OKRtnFunc(ret.retVal, cb);
			}

		});
	} else {//信创
		// console.log(signdata);
		ESeal_GetSignMethod(signdata, function(ret){
		// console.log(ret);
		if(ret.retVal == "") {
			//获取错误信息，并弹框显示
			$GetLastErrJS();
			return ESeaL_ErrorRtnFunc("", cb);
		} else if (ret.retVal == $_$METHOD_NOT_EXIST) {
			//当前接口不存在，客户端版本不匹配
			alert($_$METHOD_NOT_EXIST);
			return ESeaL_ErrorRtnFunc("", cb);
		} else {
			//返回签名算法
			return ESeaL_OKRtnFunc(ret.retVal, cb);
		}	
	});
	}
}

/******************************信创接口******************************/
/**
 * 获取最近一次错误信息并弹框显示
 */
function $GetLastErrJS() {
	ESeal_GetLastErrJS(function (ret) {
			if (ret.retVal == "") {
				alert("获取错误信息失败");
			} else if (ret.retVal == $_$METHOD_NOT_EXIST) {
				//当前接口不存在，客户端版本不匹配
				alert($_$METHOD_NOT_EXIST);
			} else {
				alert(ret.retVal);
			}
		});
}

/**
 * 登录
 * @param strCertID 证书id 
 * @param strPin 证书id对应的pin码
 * @param cb 回调函数。成功返回true，失败返回false
 */
function BWS_Login(strCertID, strPin, cb){
	VerifyUserPIN_BJCAWebSign(strCertID, strPin, function(strRes) {
		if (!strRes.retVal) {
		//	alert("校验PIN码失败");
			return ESeaL_ErrorRtnFunc("false", cb);
		} else {
		//	alert("校验PIN码成功!\r\n");
			$_$is_login = true;
			return ESeaL_OKRtnFunc("true", cb);
		}
	});
}

/**
 * 获取用户列表，并填充UserListObj指向的select框
 * @param UserListObj 用户列表select框对象 
 * @param cb 回调函数。成功返回true，失败返回false
 */
function BWS_GetUserList(UserListObj, cb){
	//先清空select列表
	UserListObj.options.length = 0;
	
	//获取用户证书列表
	GetUserList_BJCAWebSign(function(ret){
		console.log("GetUserList_BJCAWebSign ret："+ret.retVal);
		if(ret.retVal == "") {
		    alert("没有识别到证书，请插入ukey！");
			return ESeaL_ErrorRtnFunc("false", cb);
		} else {
			var strUserList = ret.retVal;
   			var allListArray = [];
    		while (true) {
        		var i = strUserList.indexOf("&&&");
        		if (i <= 0 ) {
            		break;
        		}
        		var strOneUser = strUserList.substring(0, i);
        		var strName = strOneUser.substring(0, strOneUser.indexOf("||"));
        		var strCertID = strOneUser.substring(strOneUser.indexOf("||") + 2, strOneUser.length);
        		UserListObj.options.add(new Option(strName, strCertID));

        		var len = strUserList.length;
        		strUserList = strUserList.substring(i + 3,len);
    		}

			console.log("GetUserList_BJCAWebSign end");
			return ESeaL_OKRtnFunc("true", cb);
		}
	});
}

/**
 * 获取印章列表，并填充SealListObj指向的select框
 * @param strCertID 证书id 
 * @param SealListObj 印章列表select框对象
 * @param cb 回调函数。成功返回true，失败返回false
 */
function BWS_GetSealList(strCertID, SealListObj, cb){
	if(!$_$is_login) {
		alert("请先登录！");
		return ESeaL_ErrorRtnFunc(false, cb);
	}
	//先清空select列表
	SealListObj.options.length = 0;
	
	//获取印章列表
	ESeaL_GetUserSealList(strCertID, function(ret){
		if(ret.retVal == "") {
			//获取错误信息，并弹框显示
			$GetLastErrJS();
			return ESeaL_ErrorRtnFunc("false", cb);
		} else if (ret.retVal == $_$METHOD_NOT_EXIST) {
			//当前接口不存在，客户端版本不匹配
			alert($_$METHOD_NOT_EXIST);
			return ESeaL_ErrorRtnFunc("false", cb);
		} else {
			//返回印章列表
			var sealListArray = ret.retVal;
			var seallist_length = sealListArray.length;
			if (seallist_length == 0) {
				alert("未能检测到印章！");
				return ESeaL_ErrorRtnFunc("false", cb);				
			}
			//填充印章列表选择框
			var i;
			for (i = 0; i < sealListArray.length; i++) {
				var certObj = sealListArray[i];
				var objItem = new Option(certObj, certObj);
				SealListObj.options.add(objItem);
			}
			return ESeaL_OKRtnFunc("true", cb);
		}
	});
}

//填充印章列表
function setSealList() {
  //获取印章列表并显示
  var sealSelectObj = document.getElementById("sealSelect");
  if (sealSelectObj) {
    console.log("BWS_GetSealList begin");
    BWS_GetSealList($_$CurCertID[$_$CurStampID], sealSelectObj, function(ret){
      if(ret.retVal == "true") {  
        document.getElementById("pinConfirmBtn").style.display = "none";
        document.getElementById("infoConfirmBtn").style.display = "block";
        document.getElementById("certEle").style.display = "none";
        document.getElementById("pinEle").style.display = "none";
        document.getElementById("sealEle").style.display = "block";  
      }
    });
  } else {
    alert("不识别的对象");
  }
}

var $_$isSign = 1;//1：签章，2：撤章

//第二步，弹登录窗口
function showLogin(cb, isSign) { 
  var certSelectObj = document.getElementById("certSelect");
  if (certSelectObj) {
  	userInfoDialogCallback = cb;
  	if (!isSign) {
      isSign = 1;
    }
  	$_$isSign = isSign;
    BWS_GetUserList(certSelectObj, function(ret) {
      if(ret.retVal == "true") {    
        document.getElementById("pinConfirmBtn").style.display = "block";
        document.getElementById("infoConfirmBtn").style.display = "none";
        document.getElementById("certEle").style.display = "block";
        document.getElementById("pinEle").style.display = "block";
        document.getElementById("sealEle").style.display = "none";
        document.getElementById("userCertInfo").style.display = "block";
        document.getElementById("pinInput").value = "";
      } 
    });
  } else {
    alert("不识别的对象");
  }
}

//第三步，输入pin码后登录
function pinConfirm() {
  var strCertID =  document.getElementById("certSelect").value;
  //var strCertName =  document.getElementById("certSelect").innerHTML;
  var index = document.getElementById("certSelect").selectedIndex;
  var strCertName = document.getElementById("certSelect").options[index].text;
  var strPin = document.getElementById("pinInput").value;
  if(strCertID == "") {
    alert("请先选择证书！");
    return;
  }

  if(strCertName != "易签盾证书") {
    if(strPin == "") {
      alert("请先输入密码！");
      return;
    }
  }
  
  BWS_Login(strCertID, strPin, function(ret){
    if(ret.retVal == "true") {    
      alert("校验PIN码成功!\r\n");

      //判断是否是易签盾
      if(strCertName == "易签盾证书") {
        GetLastLoginCertID_BJCAWebSign(function(ret){
          if(ret.retVal) {    
            $_$CurCertID[$_$CurStampID] = ret.retVal;
            // console.log("YQD certid:"+$_$CurCertID[$_$CurStampID]);
            if ($_$isSign == 1) {
          	  setSealList();
            } else {
          	  document.getElementById("userCertInfo").style.display = "none";
		      if (typeof userInfoDialogCallback == 'function') {
			    userInfoDialogCallback();
		      }
            }
          } else {
            alert("获取易签盾证书ID失败");
          }
        });
      } else {
        $_$CurCertID[$_$CurStampID] = strCertID;
        if ($_$isSign == 1) {
      	  setSealList();
        } else {
      	  document.getElementById("userCertInfo").style.display = "none";
	      if (typeof userInfoDialogCallback == 'function') {
		    userInfoDialogCallback();
	      }
        }
      }
    } else {
      alert("校验PIN码失败");
    }
  });
}

//第四步，选择印章之后执行签章
function infoConfirm() {
  $_$CurSealID[$_$CurStampID] = document.getElementById("sealSelect").value;
  document.getElementById("userCertInfo").style.display = "none";
  if (typeof userInfoDialogCallback == 'function') {
	userInfoDialogCallback();
  }
  // btnSign($_$CurStampID);
}
  
//关闭弹窗
function infoCancel() {
  document.getElementById("userCertInfo").style.display = "none";
}
