//拖拽签章

/**
 * 获取当前标签相对于页面的横向坐标
 * @param e 标签对象
 */
function getLeft(e) {
	var offset = e.offsetLeft;
	if (e.offsetParent != null)
		offset += getLeft(e.offsetParent);
	return offset;
}

/**
 * 获取当前标签相对于页面的纵向坐标
 * @param e 标签对象
 */
function getTop(e) {
	var offset = e.offsetTop;
	if (e.offsetParent != null)
		offset += getTop(e.offsetParent);
	return offset;
}

/**
 * 获取签章偏移量，仅对拖拽签章有效
 */
function $getDragSignPosition() {
	var dragSignPosition = {
		X: $_$CurDragPosX,
		Y: $_$CurDragPosY
	}
	return dragSignPosition;
}

/**
 * 获取拖拽印章图片
 * @param stampID 印章DIV的id
 */
function $DragSeal(stampID, strCertID, strSealID) {
	BWS_GetStampPic(strCertID, strSealID, function (ret) {
		if (ret.retVal == "") {
			//ie11以下ie浏览器，若签章失败必须清空当前div，否则会影响前一个div
			deleteSigDiv(stampID);
		//	$GetLastErrJS();
			return;
		}
		var sealObj = document.getElementById($_$Seal_Img_Name);
		sealObj.src = "data:image/gif;base64," + ret.retVal;
		// $_$sealImg.src = "data:image/gif;base64," + ret.retVal;
		var sealDivObj = document.getElementById($_$Seal_Pic_Div_Name);
		sealDivObj.style.display = "block";
		var parMainObj = document.getElementById($_$Seal_Pic_Main_Div_Name);
		parMainObj.style.display = "block";
		$_$bPos = false;

		if($_$is_windows) {
			ESeal_GetStampPicWH(function(ret){
				document.getElementById($_$Seal_Img_Name).style.width = ret.width;
				document.getElementById($_$Seal_Pic_Div_Name).style.width = ret.width;
			})
		}
	});
}

/**
 * 删除页面中的印章元素
 * @param stampID 印章DIV的id
 */
function deleteSigDiv(stampID) {
	var sealPicMainDivName = "sealPicMain" + stampID;
	var sealObj = document.getElementById(sealPicMainDivName);
	if (sealObj != null)
		sealObj.parentNode.removeChild(sealObj);
}

/**
 * 拖拽印章
 * @param strCertID 证书ID，仅对信创平台有效
 * @param strSealID 印章ID，仅对信创平台有效
 * @param orgdata 原文
 * @param stampID 印章DIV的id
 * @param container_name 签名容器
 */
function BWS_DragSign(strCertID, strSealID, orgdata, stampID, container_name)
{	
	$_$CurStampID = stampID;
	$_$CurOrgData[stampID] = orgdata;
	$_$CurCertID[stampID] = strCertID;
	$_$CurSealID[stampID] = strSealID;

	//清空Main div
	deleteSigDiv(stampID);

	//Main div
	$_$Seal_Pic_Main_Div_Name = "sealPicMain" + stampID;
	var sealPicMainDiv = document.createElement("div");
	sealPicMainDiv.setAttribute("id", $_$Seal_Pic_Main_Div_Name);
	sealPicMainDiv.style.position = "relative";
	sealPicMainDiv.className = $_$Seal_Pic_Main_Div_Name;
	if (container_name) {
		$_$CurContainerName = container_name;
		var containerDiv = document.getElementById($_$CurContainerName);
		if (containerDiv) {
			containerDiv.appendChild(sealPicMainDiv);
		}
	} else {
		container_name = "BODY";
		$_$CurContainerName = container_name;
		document.body.appendChild(sealPicMainDiv);
	}

	//印章div
	$_$Seal_Pic_Div_Name = "sealPicDiv" + stampID;
	var sealDiv = document.createElement("div");
	sealDiv.setAttribute("id", $_$Seal_Pic_Div_Name);
	sealDiv.className = $_$Seal_Pic_Div_Name;
	sealDiv.style.position = "relative";
	sealDiv.style.left = "100px";
	sealDiv.style.top = "100px";
	sealDiv.style.display = "none";
	document.getElementById($_$Seal_Pic_Main_Div_Name).appendChild(sealDiv);

	//图片
	$_$Seal_Img_Name = "sealImg" + stampID;
	var img = document.createElement("img");
	img.setAttribute("id", $_$Seal_Img_Name);
	document.getElementById($_$Seal_Pic_Div_Name).appendChild(img);

	//拖拽签章
	$DragSeal(stampID, strCertID, strSealID, function(ret){
		if (typeof cb == 'function') {
			var retObj = {retVal:ret.value};
			cb(retObj);
		}
	});
	
	document.ondragstart = function () {
		return false;
	};

	return;
}

/**
 * 鼠标拖动印章
 */
document.onmousemove = function (event) {
	event = event || window.event;

	if ($_$bPos)
		return;

	var sealPic = window.document.getElementById($_$Seal_Pic_Div_Name);
	var sealImg = window.document.getElementById($_$Seal_Img_Name);
	var mainDIV = window.document.getElementById($_$Seal_Pic_Main_Div_Name);
	var containerDIV = $_$CurContainerName;
	if (containerDIV == "" || mainDIV == null || sealPic == null || sealImg == null /*|| $_$sealImg.src == ""*/)
		return;

	//luoxing  因适配高dpi显示，图片的像素可能会比实际需要展示的像素大，div的像素大于内部img对象的显示会产生部分页面区域被覆盖无法点击
	//var sealPicWidth = $_$sealImg.width;
	//var sealPicHeight = $_$sealImg.height;
	//sealPic.style.width = sealPicWidth + "px";
	//sealPic.style.height = sealPicHeight + "px";

	var containerObj = window.document.getElementById(containerDIV);
	if (containerObj == null)
		return;

	sealPic.style.display = "block";
	//鼠标在印章div的外面，加1，从而定位
	var pixelLeft = document.body.scrollLeft + event.clientX + 1;
	var pixelTop = document.body.scrollTop + event.clientY + 1;
	
	var t = containerObj.offsetTop; //获取该元素对应父容器的上边距
    var l = containerObj.offsetLeft; //对应父容器的上边距
    //判断是否有父容器，如果存在则累加其边距
    while (containerObj = containerObj.offsetParent) {//等效 obj = obj.offsetParent;while (obj != undefined)
        t += containerObj.offsetTop; //叠加父容器的上边距
        l += containerObj.offsetLeft; //叠加父容器的左边距
    }
	sealPic.style.left = pixelLeft - l + "px";
	sealPic.style.top = pixelTop - t + "px";

};

/**
 * 鼠标点击盖章
 */
document.onmousedown = function (event) {
	event = event || window.event;
	if (oMenu) {
		var srcObj = event.srcElement ? event.srcElement : event.target;
        var srcObjName = srcObj.id.substr(0, 5);
        if (srcObjName != "liObj") {
            oMenu.style.display = "none";
		}
	}
	if (event.button == 1 || event.button == 0) {
		//ie11 是0，ie9 是1
		if ($_$bPos)
			return;

		var evtObj = event.srcElement ? event.srcElement : event.target;
		var sealPic = window.document.getElementById($_$Seal_Pic_Div_Name);
		var sealImg = window.document.getElementById($_$Seal_Img_Name);
		var mainDIV = window.document.getElementById($_$Seal_Pic_Main_Div_Name);
		var containerDIV = $_$CurContainerName;
		if (containerDIV == null || mainDIV == null || sealPic == null || sealImg == null /*|| $_$sealImg.src == ""*/)
			return;

		var posX;
		var posY;

		// luoxing 获取浏览器版本
		var explorer = window.navigator.userAgent.toLowerCase();
		var explorer_obj;
		if (explorer.indexOf("firefox") >= 0) {
			var ver = explorer.match(/firefox\/([\d.]+)/)[1];
			explorer_obj ={ type: "Firefox", version: ver };
		}
		else
		{
			explorer_obj ={ type: "", version: ver };			
		}

		var containerObj = window.document.getElementById(containerDIV);
		if (containerObj == null)
			return;

		//坐标补偿
		// if (event.x == undefined)  这个在IE下可能造成获取坐标错误，待检验浏览器兼容性
        if (event.pageX)
		{
			var sealPosX =  event.pageX;
			var sealPosY =  event.pageY;
		}else
		{
			var sealPosX =  event.x;
			var sealPosY =  event.y;
		}
		
		var scroll_top = 0;
		if (document.documentElement && document.documentElement.scrollTop) {
	        scroll_top = document.documentElement.scrollTop;
	    }
	    else if (document.body) {
	        scroll_top = document.body.scrollTop;
	    }
		
		posX = document.body.scrollLeft + sealPosX - containerObj.offsetLeft - mainDIV.offsetLeft;
//		if (explorer_obj.type == "Firefox")
//		{
//			posY = sealPosY - containerObj.offsetTop - mainDIV.offsetTop;
//		}else
//		{
		posY = sealPosY + scroll_top - containerObj.offsetTop - mainDIV.offsetTop;
//		}
		
		sealPic.style.left = posX + "px";
		sealPic.style.top = posY + "px";
		
		var ele = document.getElementById($_$CurContainerName);
		if (ele != null) {
			//标签相对页面左上角的绝对坐标
			var rtElementLeft, rtElementTop;
			rtElementLeft = getLeft(ele);
			rtElementTop = getTop(ele);
			$_$CurDragPosX = posX + rtElementLeft;
			$_$CurDragPosY = posY + rtElementTop;
		}

		if (containerDIV != "BODY") {
			var containerObj = window.document.getElementById(containerDIV);
			if (containerObj != null) {
				//容器相对页面左上角的绝对坐标
				containerObj = document.getElementById(containerDIV);
				var containerElementLeft,
				containerElementTop;
				containerElementLeft = getLeft(containerObj);
				containerElementTop = getTop(containerObj);

				//再计算相对坐标
				$_$CurDragPosX -= containerElementLeft;
				$_$CurDragPosY -= containerElementTop;
			}
		}
		$_$bPos = true;

		BWS_Sign($_$CurCertID[$_$CurStampID], $_$CurOrgData[$_$CurStampID], $DragSignCallback);
		
		//document.getElementById('sealPicDiv').style.display = 'none';
		//document.getElementById('sealPicDiv').style.backgroundImage = "";
	} else if (event.button == 2) {
		//右键取消
		if ($_$bPos)
			return;
		//document.getElementById($_$Seal_Pic_Div_Name).style.display = "none";
		deleteSigDiv($_$CurStampID);
		$_$bPos = true;
		//	document.getElementById($_$Seal_Pic_Div_Name).style.backgroundImage = "";
	}
};