//自定义url. ********************************************************************************************************
function SelfUrl(mapExt) {

    var tb = $("#TB_" + mapExt.AttrOfOper);
    if (tb.length == 0)
        return; //有可能字段被删除了.

    //设置文本框只读.
    tb.attr('readonly', 'true');
    // tb.attr('disabled', 'true');

    //在文本框双击，绑定弹出. PopGroupList.htm的窗口. 
    tb.bind("click", function () { SelfUrl_Done(mapExt) });
}

function SelfUrl_Done(mapExtJson) {

    //获得主键.
    var pkval = GetPKVal();
    var url = mapExt.Tag;
    if (url.indexOf('?') == -1)
        url = url + "?PKVal=" + pkval;
    var title = mapExt.GetPara("Title");

    if (window.parent && window.parent.OpenBootStrapModal) {
        window.parent.OpenBootStrapModal(url, "eudlgframe", title, mapExt.H, mapExt.W, "icon-edit", false, function () { }, null, function () {
            //location = location;
        });
        return;
    }
}

//树干叶子模式.
function PopBranchesAndLeaf(mapExt) {
	var target = $("#TB_" + mapExt.AttrOfOper);
	target.hide();

	var width = target.width();
	var height = target.height();
	var container = $("<div></div>");
	target.after(container);
	container.width(width);
	container.height(height);
	container.attr("id", mapExt.AttrOfOper + "_mtags");

	$("#" + mapExt.AttrOfOper + "_mtags").mtags({
		"fit" : true,
		"onUnselect" : function (record) {
			console.log("unselect: " + JSON.stringify(record));
		}
	});

	var width = mapExt.W;
	var height = mapExt.H;
	var iframeId = mapExt.MyPK + mapExt.FK_MapData;
	var title = mapExt.GetPara("Title");
	var oid = GetPKVal(); 
	
	var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
	frmEleDBs.Retrieve("FK_MapData", mapExt.FK_MapData, "EleID", mapExt.AttrOfOper, "RefPKVal", oid);
	var initJsonData = [];
	$.each(frmEleDBs, function (i, o) {
		initJsonData.push({
			"No" : o.Tag1,
			"Name" : o.Tag2
		});
	});
	$("#" + mapExt.AttrOfOper + "_mtags").mtags("loadData", initJsonData);
	//解项羽 这里需要相对路径.
	var url = "../CCForm/Pop/BranchesAndLeaf.htm?MyPK=" + mapExt.MyPK + "&oid=" + oid + "&m=" + Math.random();
	container.on("dblclick", function () {
		if (window.parent && window.parent.OpenBootStrapModal) {
			window.parent.OpenBootStrapModal(url, iframeId, title, width, height, "icon-edit", true, function () {
				var iframe = document.getElementById(iframeId);
				if (iframe) {
					var selectedRows = iframe.contentWindow.selectedRows;
					if ($.isArray(selectedRows)) {
						var mtags = $("#" + mapExt.AttrOfOper + "_mtags")
						mtags.mtags("loadData", selectedRows);
						$("#TB_" + mapExt.AttrOfOper).val(mtags.mtags("getText"));
					}
				}
			}, null, function () {
				
			});
			return;
		}
		//OpenEasyUiDialog(url, iframeId, title, width, height, undefined, true, function () {
		//	var iframe = document.getElementById(iframeId);
		//	if (iframe) {
		//		var selectedRows = iframe.contentWindow.selectedRows;
		//		if ($.isArray(selectedRows)) {
		//			var mtags = $("#" + mapExt.AttrOfOper + "_mtags")
		//			mtags.mtags("loadData", selectedRows);
		//			$("#TB_" + mapExt.AttrOfOper).val(mtags.mtags("getText"));
		//		}
		//	}
		//	return true;
		//});
	});

	return;
    var tb = $("#" + mapExt.AttrOfOper);

    //设置文本框只读.
    tb.attr('readonly', 'true');
    tb.attr('disabled', 'true');
    tb.attr("onclick", "alert('ssssssssssssss');");
    return;

    //  alert(tb);
    // 把文本框的内容，按照逗号分开, 并且块状显示. 右上角出现删除叉叉.

    // 文本框尾部出现选择的图标.
    icon = "glyphicon glyphicon-tree-deciduous";
    var eleHtml = ' <div class="input-group form_tree">' + tb.parent().html()
    eleHtml += '<span class="input-group-addon" onclick="PopBranchesAndLeaf_Deal(this,' + "'" + mapExt.MyPK + "','" + mapExt.FK_MapData + "', " + mapExt.W + "," + mapExt.H + ",'" + GepParaByName("Title", mapExt.AtPara) + "');" + '"><span class="' + icon + '"></span></span></div>';
    tb.parent().html(eleHtml);


   // tb.parent().html(eleHtml);
   // alert(eleHtml);

    //在文本框双击，绑定弹出. DeptEmpModelAdv.htm的窗口.
    tb.attr("onclick", "alert('ssssssssssssss');");

//    tb.attr("onclick", "ShowHelpDiv('TB_" + mapExt.AttrOfOper + "','','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "','returnvalccformpopval');");
    //窗口返回值的时候，重新计算文本块.
}

function PopBranchesAndLeaf_Deal()
{

}

//树干模式.
function PopBranches(mapExt) {

    var tb = $("#TB_" + mapExt.AttrOfOper);

    //设置文本框只读.
    tb.attr('readonly', 'true');
    tb.attr('disabled', 'true');

    // 把文本框的内容，按照逗号分开, 并且块状显示. 右上角出现删除叉叉.

    // 文本框尾部出现选择的图标.
    icon = "glyphicon glyphicon-tree-deciduous";
    var eleHtml = ' <div class="input-group form_tree">' + tb.parent().html()
    eleHtml += '<span class="input-group-addon" onclick="' + "ReturnValCCFormPopValGoogle('TB_" + mapExt.AttrOfOper + "','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "', " + mapExt.W + "," + mapExt.H + ",'" + GepParaByName("Title", mapExt.AtPara) + "');" + '"><span class="' + icon + '"></span></span></div>';
    tb.parent().html(eleHtml);

    //在文本框双击，绑定弹出. DeptEmpModelAdv.htm的窗口.
  //  tb.attr("onclick", "alert('ssssssssssssss');");

    //    tb.attr("onclick", "ShowHelpDiv('TB_" + mapExt.AttrOfOper + "','','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "','returnvalccformpopval');");
    //窗口返回值的时候，重新计算文本块.

}


/******************************************  表格查询 **********************************/
function PopTableSearch(mapExt) {

    var tb = $("#TB_" + mapExt.AttrOfOper);
    if (tb.length == 0) { 
        mapExt.Delete(); //把他删除掉.
        return;
    }

    //设置文本框只读.
    tb.attr('readonly', 'true');
    // tb.attr('disabled', 'true');

    //在文本框双击，绑定弹出. PopGroupList.htm的窗口. 
    tb.bind("click", function () { PopTableSearch_Done(mapExt) });
}

function PopTableSearch_Done(mapExt) {

    //获得主键.
    var pkval = GetPKVal();

    //弹出这个url, 主要有高度宽度, 可以在  ReturnValCCFormPopValGoogle 上做修改.
    var url = 'Pop/TableSearch.htm?FK_MapExt=' + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&PKVal=" + pkval + "&OID=" + pkval + "&KeyOfEn=" + mapExt.AttrOfOper;

    if (window.parent && window.parent.OpenBootStrapModal) {
        window.parent.OpenBootStrapModal(url, "eudlgframe", mapExt.GetPara("Title"), mapExt.H, mapExt.W, "icon-edit", false, function () { }, null, function () {
           
            // location = location;

        });
        return;
    }
}


/******************************************  分组列表 **********************************/

function PopGroupList(mapExt) {

    var tb = $("#TB_" + mapExt.AttrOfOper);
    if (tb.length == 0)
        return; //有可能字段被删除了.

    //设置文本框只读.
    tb.attr('readonly', 'true');
   // tb.attr('disabled', 'true');

    //在文本框双击，绑定弹出. PopGroupList.htm的窗口. 
    tb.bind("click", function (){ PopGroupList_Done(mapExt)} );
}

function PopGroupList_Done(mapExt) {

    //获得主键.
    var pkval = GetPKVal();

    //弹出这个url, 主要有高度宽度, 可以在  ReturnValCCFormPopValGoogle 上做修改.
    var url = 'Pop/GroupList.htm?FK_MapExt=' + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&PKVal="+pkval+"&OID=" + pkval + "&KeyOfEn=" + mapExt.AttrOfOper;

    if (window.parent && window.parent.OpenBootStrapModal) {
        window.parent.OpenBootStrapModal(url, "eudlgframe", "导入数据", mapExt.H, mapExt.W, "icon-edit", true, function () {
			var iframe = document.getElementById("eudlgframe");
			if (iframe) {
				var savefn = iframe.contentWindow.Save;
				if (typeof savefn === "function") {
					var selectVals = savefn();
					$("#TB_" + mapExt.AttrOfOper).val(selectVals);
				}
			}
		}, null, function () {
			
        });
        return;
    }
}
