//右键菜单

/******************************通用接口******************************/
/**
 * 验章
 */
function $Verify($_$CurStampID) {
	oMenu.style.display = "none";
	// 获取orgdata方法1：直接从签名之后储存的数据中获取，对签章之后修改的原文无效
	// var orgdata = $_$CurOrgData[$_$CurStampID];

	// 获取orgdata方法2：通过“前缀+印章页面ID”的方式获取，这种方式要求如果页面有多个原文，所有原文所在的容器ID都要以“前缀+印章页面ID”的方式命名
	var orgdata;
	var OrgDataContainerName = $_$OrgDataContainerNameHead + $_$CurStampID;
	if (document.getElementById(OrgDataContainerName)) {
		orgdata = document.getElementById(OrgDataContainerName).value;
	} else {
		return;
	}

	// 获取orgdata方法3：对于已集成用户，如果原文容器ID命名不是“前缀+印章页面ID”的方式，可以存入数组中
	// var OrgDataContainerArr = ["originData1", "originData2", "originData3", "originData4", "originData5"];
	// var OrgDataContainerName = OrgDataContainerArr[$_$CurStampID - 1];

	if (document.getElementById(OrgDataContainerName)) {
		orgdata = document.getElementById(OrgDataContainerName).value;
	} else {
		return;
	}

  	$_$CurOrgData[$_$CurStampID] = orgdata;
	var signdata = $_$CurSignData[$_$CurStampID];
	BWS_Verify(orgdata, signdata, function(ret){
		var retObj = {retVal:ret.retVal};
		$VerifyCallback(retObj,$_$CurStampID);
	}, true);
}

/**
 * 撤销签章
 */
function $RemoveSeal($_$CurStampID) {
  oMenu.style.display = "none";
  if ($_$is_windows) {
    $RemoveSealFunc($_$CurStampID);
  } else {
    showLogin($RemoveSealFunc($_$CurStampID), 2);
  }

	$("#qm"+$_$CurStampID).css("display","");
}

function $RemoveSealFunc($_$CurStampID) {

  var signdata = $_$CurSignData[$_$CurStampID];
  var certID = $_$CurCertID[$_$CurStampID];
  BWS_Remove(signdata, certID, function(ret){
	var retObj = {retVal:ret.retVal};
	$RemoveCallback(retObj,$_$CurStampID);
  });
}

/**
 * 获取证书信息
 */
function $ShowUserCerInfo() {
	oMenu.style.display = "none";
	var signdata = $_$CurSignData[$_$CurStampID];
	BWS_GetCertInfo(signdata, function(ret){
		if(ret.retVal) {
			//显示证书信息
			alert(ret.retVal);
		}
	});
}

/**
 * 获取签章时间
 */
function $ShowSignTime() {
	oMenu.style.display = "none";
	var signdata = $_$CurSignData[$_$CurStampID];
	BWS_GetSignTime(signdata, function(ret){
		if(ret.retVal) {
			//显示签章时间
			alert(ret.retVal);
		}
	});
}

/**
 * 获取关于信息
 */
function $GetVersion() {
	oMenu.style.display = "none";
	BWS_GetVersion(function(ret){
		alert("BJCA网页签章V" + ret.retVal);
	});
}

/**
 * 删除之前的右键菜单
 */
function deleteSigMenuDiv() {
	var sigMenuDivName = "sigMenu";
	var my = document.getElementById(sigMenuDivName);
	if (my != null)
		my.parentNode.removeChild(my);
}

/**
 * 创建右键菜单
 */
var oMenu;
function ESeaL_CreateSignMenu($_$CurStampID) {
	deleteSigMenuDiv();

	var sigMenuDiv = document.createElement("div");
	var sigMenuDivName = "sigMenu";
	sigMenuDiv.setAttribute("id", sigMenuDivName);
	sigMenuDiv.className = "contextMenu";
	//.getElementById("qm"+$_$CurStampID)
	document.body.appendChild(sigMenuDiv);
	var ulObj = document.createElement("ul");
	var ulObjID = "ulo";
	ulObj.setAttribute("id", ulObjID);
	document.getElementById(sigMenuDivName).appendChild(ulObj); //在提示框div中添加标题栏对象title
	/*
	var liObj = document.createElement("li");
	liObj.innerHTML = "签章";
	document.getElementById(ulObjID).appendChild(liObj);//在提示框div中添加标题栏对象title
	 */
	var liObj2 = document.createElement("li");
	liObj2.setAttribute("id", "liObj2");
	liObj2.innerHTML = "验章";
	document.getElementById(ulObjID).appendChild(liObj2); //在提示框div中添加标题栏对象title
	var liObj3 = document.createElement("li");
	liObj3.setAttribute("id", "liObj3");
	liObj3.innerHTML = "证书信息";
	document.getElementById(ulObjID).appendChild(liObj3); //在提示框div中添加标题栏对象title
	var liObj4 = document.createElement("li");
	liObj4.setAttribute("id", "liObj4");
	liObj4.innerHTML = "签章时间";
	document.getElementById(ulObjID).appendChild(liObj4); //在提示框div中添加标题栏对象title
	var liObj5 = document.createElement("li");
	liObj5.setAttribute("id", "liObj5");
	liObj5.innerHTML = "撤销签章";
	document.getElementById(ulObjID).appendChild(liObj5); //在提示框div中添加标题栏对象title
	var liObj6 = document.createElement("li");
	liObj6.setAttribute("id", "liObj6");
	liObj6.innerHTML = "关于";
	document.getElementById(ulObjID).appendChild(liObj6); //在提示框div中添加标题栏对象title
	oMenu = document.getElementById(sigMenuDivName); //"picDragMenu"
	var aUl = oMenu.getElementsByTagName("ul");
	var aLi = oMenu.getElementsByTagName("li");
	var showTimer = (hideTimer = null);
	var i = 0;
	var maxWidth = (maxHeight = 0);
	var aDoc = [document.documentElement.offsetWidth, document.documentElement.offsetHeight];
	oMenu.style.display = "none";
	/*oMenu.onmouseover = function () {
		oMenu.style.display = "block";
	};*/
	// 这里如果判断移出，会在移入的时候触发一次移出，导致菜单消失。
	// 不判断移出，需要用户手动点击了菜单或页面其他位置菜单才消失。
	// 所以暂不判断移出。2020-3-6 zc
	//oMenu.onmouseout = function () {
	//	oMenu.style.display = "none";
	//};
	//var Seal_Pic_Main_Div_Name = "";
	//var Seal_Pic_Div_Name = "";
	//var Signature_Element_Name = "";
	for (i = 0; i < aLi.length; i++) {
		//为含有子菜单的li加上箭头
		aLi[i].getElementsByTagName("ul")[0] && (aLi[i].className = "sub");
		//鼠标移入
        aLi[i].onmouseover = function() {
            var oThis = this;

            var oUl = oThis.getElementsByTagName("ul");
            //鼠标移入样式
            oThis.className += " active";
            //显示子菜单
            if (oUl[0]) {

                clearTimeout(hideTimer);
                showTimer = setTimeout(function() {

                    for (i = 0; i < oThis.parentNode.children.length; i++) {
                        oThis.parentNode.children[i].getElementsByTagName("ul")[0] && (oThis.parentNode.children[i].getElementsByTagName("ul")[0].style.display = "none");
                    }

                    oUl[0].style.display = "block";
                    oUl[0].style.top = oThis.offsetTop + "px";
                    oUl[0].style.left = oThis.offsetWidth + "px";
                    setWidth(oUl[0]);
                    //最大显示范围					
                    maxWidth = aDoc[0] - oUl[0].offsetWidth;
                    maxHeight = aDoc[1] - oUl[0].offsetHeight;
                    //防止溢出
                    maxWidth < getOffset.left(oUl[0]) && (oUl[0].style.left = -oUl[0].clientWidth + "px");
                    maxHeight < getOffset.top(oUl[0]) && (oUl[0].style.top = -oUl[0].clientHeight + oThis.offsetTop + oThis.clientHeight + "px")
                }, 300);
            }
        };
        //鼠标移出	
        aLi[i].onmouseout = function() {
            var oThis = this;
            var oUl = oThis.getElementsByTagName("ul");
            //鼠标移出样式
            oThis.className = oThis.className.replace(/\s?active/, "");
            clearTimeout(showTimer);
            hideTimer = setTimeout(function() {
                for (i = 0; i < oThis.parentNode.children.length; i++) {
                    oThis.parentNode.children[i].getElementsByTagName("ul")[0] && (oThis.parentNode.children[i].getElementsByTagName("ul")[0].style.display = "none");
                }
            }, 300);
        };
		//鼠标点击
		aLi[i].onclick = function () {

			var oThis = this;
			if ($_$WebSign_CurrentObj) {
				if (oThis.innerText == "签章" || oThis.innerHTML == "签章") {
					BWS_DirectSign($_$CurCertID[$_$CurStampID], $_$CurSealID[$_$CurStampID], $_$CurOrgData[$_$CurStampID], $DragSignCallback);
				} else if (oThis.innerText == "验章" || oThis.innerHTML == "验章") {
					$Verify($_$CurStampID);
				} else if (oThis.innerText == "证书信息" || oThis.innerHTML == "证书信息") {
					$ShowUserCerInfo();
				} else if (oThis.innerText == "签章时间" || oThis.innerHTML == "签章时间") {
					$ShowSignTime();
				} else if (oThis.innerText == "撤销签章" || oThis.innerHTML == "撤销签章") {
					$RemoveSeal($_$CurStampID);
				} else if (oThis.innerText == "关于" || oThis.innerHTML == "关于") {
					$GetVersion();
				}
			}
		};
	}
	//自定义右键菜单
    document.oncontextmenu = function(event) {
        var event = event || window.event;
        var srcObj = event.srcElement ? event.srcElement : event.target;
        var srcObjName = srcObj.id.substr(0, 7);
        var stampID = srcObj.id.substr(7);
        if (stampID != "") {
            if (srcObjName == "sealImg") {
                if ($_$bPos) {
                	$_$CurStampID = stampID;
                    oMenu.style.display = "block";
                    /*获取当前鼠标右键按下后的位置，据此定义菜单显示的位置*/
                    var rightedge = document.body.clientWidth - event.clientX;
                    var bottomedge = document.body.clientHeight - event.clientY;
                    /*如果从鼠标位置到容器右边的空间小于菜单的宽度，就定位菜单的左坐标（Left）为当前鼠标位置向左一个菜单宽度*/
                    if (rightedge < oMenu.offsetWidth) oMenu.style.left = document.body.scrollLeft + event.clientX - oMenu.offsetWidth + "px";
                    else
                        /*否则，就定位菜单的左坐标为当前鼠标位置*/
                        oMenu.style.left = document.body.scrollLeft + event.clientX + "px";
                    /*如果从鼠标位置到容器下边的空间小于菜单的高度，就定位菜单的上坐标（Top）为当前鼠标位置向上一个菜单高度*/
                    if (bottomedge < oMenu.offsetHeight) oMenu.style.top = document.body.scrollTop + event.clientY - oMenu.offsetHeight + "px";
                    else
                        /*否则，就定位菜单的上坐标为当前鼠标位置*/
                        oMenu.style.top = document.body.scrollTop + event.clientY + "px";
                    /*
                    oMenu.style.top = event.y-2 + "px";
                    oMenu.style.left = event.x-2 + "px";
                    */
                    setWidth(aUl[0]);
                    liObj2.style.display = "none";
                    liObj3.style.display = "none";
                    liObj4.style.display = "none";
                    var signdata = $_$CurSignData[stampID];
                    if (signdata) {
                        liObj2.style.display = "block";
                        liObj3.style.display = "block";
                        liObj4.style.display = "block";
                    }
                    //最大显示范围
                    maxWidth = aDoc[0] - oMenu.offsetWidth;
                    maxHeight = aDoc[1] - oMenu.offsetHeight;
                    //防止菜单溢出
                    //oMenu.offsetTop > maxHeight && (oMenu.style.top = maxHeight + "px");
                    //oMenu.offsetLeft > maxWidth && (oMenu.style.left = maxWidth + "px");
                    oMenu.target = srcObj.id;
                } else {
                    bSignReady = false;
                    document.getElementById(sealDivName).style.display = 'none';
                }
            }
        }
        return false;
    };
	//点击隐藏菜单
	/*document.onmousedown = function (event) {
		var event = event || window.event;
        var srcObj = event.srcElement ? event.srcElement : event.target;
        var srcObjName = srcObj.id.substr(0, 5);
        if (srcObjName != "liObj") {
            oMenu.style.display = "none";
		}
	};*/
	//取li中最大的宽度, 并赋给同级所有li
	function setWidth(obj) {
		maxWidth = 0;
		for (i = 0; i < obj.children.length; i++) {
			var oLi = obj.children[i];
			var iWidth = oLi.clientWidth - parseInt(oLi.currentStyle ? oLi.currentStyle["paddingLeft"] : getComputedStyle(oLi, null)["paddingLeft"]) * 2;
			if (iWidth > maxWidth)
				maxWidth = iWidth;
		}
		for (i = 0; i < obj.children.length; i++)
			obj.children[i].style.width = maxWidth + "px";
	}
}
