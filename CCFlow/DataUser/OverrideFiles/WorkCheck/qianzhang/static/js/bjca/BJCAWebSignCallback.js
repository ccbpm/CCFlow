/*bjca_gangj 2021-4-25
 回调层，用户可根据需求修改
*/

/**
 * 直接签章回调
 */
function $DirectSignCallback(ret) {
	//debugger;
	alert("签名成功！");
	$_$CurSignData[$_$CurStampID] = ret.retVal;
	var stampID = $_$CurStampID;
	document.getElementById("signatureData" + $_$CurStampID).value = ret.retVal;
	$_$Seal_Pic_Main_Div_Name = "sealPicMain" + stampID;
	$_$Seal_Pic_Div_Name = "sealPicDiv" + stampID;
	$_$Seal_Img_Name = "sealImg" + stampID;
	BWS_GetSignMethod($_$CurSignData[$_$CurStampID], function(ret) {
		$_$CurSignMethod[$_$CurStampID] = ret.retVal;
	})
	var temp = "<div id='sealPicMain" + stampID + "' class='sealPicMain" + stampID + "' style='position: relative; display: block;'><div id='sealPicDiv" + stampID + "'><img id='sealImg" + stampID + "' src='data:image/gif;base64,"+ret.picData+"' /></div></div>";
	return temp;
}

/**
 * 回显签章回调
 */
function $ShowSignCallback(stampID, picData) {
	var temp = "<div id='sealPicMain" + stampID + "' class='sealPicMain" + stampID + "' style='position: relative; display: block;'><div id='sealPicDiv" + stampID + "'><img id='sealImg" + stampID + "' src='data:image/gif;base64,"+picData+"' /></div></div>";
	return temp;
}

/**
 * 拖拽签章回调
 */
function $DragSignCallback(ret) {
	if (ret.retVal == "") {
		$GetLastErrJS();
		document.getElementById($_$Seal_Pic_Div_Name).style.display = "none";
	} else {
		document.getElementById($_$Seal_Pic_Div_Name).style.display = "block";
		document.getElementById("signatureData" + $_$CurStampID).value = ret.retVal;
		alert("签名成功！");
		console.log($getDragSignPosition());
		$_$CurSignData[$_$CurStampID] = ret.retVal;
		BWS_GetSignMethod($_$CurSignData[$_$CurStampID], function(ret) {
			$_$CurSignMethod[$_$CurStampID] = ret.retVal;
		})
	}
}

/**
 * 撤销签章回调
 */
function $RemoveCallback(ret,$_$CurStampID) {
	$_$Seal_Pic_Div_Name = "sealPicDiv" + $_$CurStampID;
	$_$Seal_Img_Name = "sealImg" + $_$CurStampID;
	if (ret.retVal == "") {
		$GetLastErrJS();
	} else if (ret.retVal == $_$REMOVE_NOT_ALLOWED || ret.retVal == $_$NOT_LOGIN) {
		return;
	} else {
		document.getElementById($_$Seal_Pic_Div_Name).style.display = "none";
		if ($_$CurStampID) {
			if (document.getElementById("signatureData" + $_$CurStampID)) {
				document.getElementById("signatureData" + $_$CurStampID).value = "";
			}
		}
	}
}

/**
 * 验证签章回调
 */
function $VerifyCallback(ret,$_$CurStampID) {
	$_$Seal_Pic_Div_Name = "sealPicDiv" + $_$CurStampID;
	$_$Seal_Img_Name = "sealImg" + $_$CurStampID;
	if (document.getElementById($_$Seal_Img_Name)) {
		var sealObj = document.getElementById($_$Seal_Img_Name);
		sealObj.src = "data:image/gif;base64," + ret.retVal;
	}
}