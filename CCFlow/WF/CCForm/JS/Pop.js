//自定义url. ********************************************************************************************************
function SelfUrl(mapExt) {

    var tb = $("#TB_" + mapExt.AttrOfOper);
    if (tb.length == 0) {
        alert(mapExt.AttrOfOper + "字段删除了.");
        mapExt.Delete();
        return; //有可能字段被删除了.
    }

    //设置文本框只读.
    tb.attr('readonly', 'true');
    // tb.attr('disabled', 'true');

    //在文本框双击，绑定弹出. PopGroupList.htm的窗口. 
    tb.bind("click", function () { SelfUrl_Done(mapExt) });
}

function SelfUrl_Done(mapExt) {

    //获得主键.
    var pkval = GetPKVal();
    var url = mapExt.Tag;
    if (url.indexOf('?') == -1)
        url = url + "?PKVal=" + pkval;
    var title = mapExt.GetPara("Title");

    if (window.parent && window.parent.OpenBootStrapModal) {

        window.parent.OpenBootStrapModal(url, "eudlgframe", title, mapExt.H, mapExt.W,
         "icon-edit", true, function () {
             var iframe = document.getElementById("eudlgframe");
             if (iframe) {
                 var val = iframe.contentWindow.Btn_OK();
                 $("#TB_" + mapExt.AttrOfOper).val(val);

             }

         }, null, function () {
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
        "fit": true,
        "onUnselect": function (record) {
            Delete_FrmEleDB(mapExt.AttrOfOper, oid, record.No);
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
            "No": o.Tag1,
            "Name": o.Tag2
        });
    });
    $("#" + mapExt.AttrOfOper + "_mtags").mtags("loadData", initJsonData);
    //解项羽 这里需要相对路径.
    var localHref = GetLocalWFPreHref();
    var url = localHref + "/WF/CCForm/Pop/BranchesAndLeaf.htm?MyPK=" + mapExt.MyPK + "&oid=" + oid + "&m=" + Math.random();
    container.on("dblclick", function () {
        if (window.parent && window.parent.OpenBootStrapModal) {
            window.parent.OpenBootStrapModal(url, iframeId, title, width, height, "icon-edit", true, function () {
				var selectType = mapExt.GetPara("SelectType");
                //单选清空数据
                if (selectType == "0") {
                    //清空数据
                    Delete_FrmEleDBs(mapExt.FK_MapData, mapExt.AttrOfOper, oid);
                }
                var iframe = document.getElementById(iframeId);
                if (iframe) {
                    var selectedRows = iframe.contentWindow.selectedRows;
                    if ($.isArray(selectedRows)) {
                        var mtags = $("#" + mapExt.AttrOfOper + "_mtags")
                        mtags.mtags("loadData", selectedRows);
                        $("#TB_" + mapExt.AttrOfOper).val(mtags.mtags("getText"));
						// 单选复制当前表单
						if (selectType == "0" && selectedRows.length == 1) {
							ValSetter(mapExt.Tag4, selectedRows[0].No);
						}
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

function PopBranchesAndLeaf_Deal() {

}

function ValSetter(tag4, key) {
	if (!tag4 || !key) {
		return;
	}
	tag4 = tag4.replace(/@Key/g, key).replace(/~/g, "'");
	var dt = DBAccess.RunSQLReturnTable(tag4);
	GenerFullAllCtrlsVal(dt)
}

//树干模式.
function PopBranches(mapExt) {
    var target = $("#TB_" + mapExt.AttrOfOper);
    target.hide();

    var width = target.width();
    var height = target.height();
    var container = $("<div></div>");
    target.after(container);
    container.width(width);
    container.css("height", height);
    container.attr("id", mapExt.AttrOfOper + "_mtags");

    $("#" + mapExt.AttrOfOper + "_mtags").mtags({
        "fit": true,
        "onUnselect": function (record) {
            console.log("unselect: " + JSON.stringify(record));
            Delete_FrmEleDB(mapExt.AttrOfOper, oid, record.No);
        }
    });

    var width = mapExt.W;
    var height = mapExt.H;
    var iframeId = mapExt.MyPK + mapExt.FK_MapData;
    var title = mapExt.GetPara("Title");
    var oid = GetPKVal();
    //初始加载
    Refresh_Mtags(mapExt.FK_MapData, mapExt.AttrOfOper, oid);
    //这里需要相对路径.
    var localHref = GetLocalWFPreHref();
    var url = localHref + "/WF/CCForm/Pop/Branches.htm?MyPK=" + mapExt.MyPK + "&oid=" + oid + "&m=" + Math.random();
    container.on("dblclick", function () {
        if (window.parent && window.parent.OpenBootStrapModal) {
            window.parent.OpenBootStrapModal(url, iframeId, title, width, height, "icon-edit", true, function () {
                var selectType = mapExt.GetPara("SelectType");
                //单选清空数据
                if (selectType == "0") {
                    //清空数据
                    Delete_FrmEleDBs(mapExt.FK_MapData, mapExt.AttrOfOper, oid);
                }
                var iframe = document.getElementById(iframeId);
                if (iframe) {
                    var nodes = iframe.contentWindow.GetCheckNodes();
                    if ($.isArray(nodes)) {
                        $.each(nodes, function (i, node) {
                            SaveVal_FrmEleDB(mapExt.FK_MapData, mapExt.AttrOfOper, oid, node.No, node.Name);
                        });
                        //重新加载
                        Refresh_Mtags(mapExt.FK_MapData, mapExt.AttrOfOper, oid);
						// 单选复制当前表单
						if (selectType == "0" && nodes.length == 1) {
							ValSetter(mapExt.Tag4, nodes[0].No);
						}
                    }
                }
            }, null, function () {

            });
            return;
        }
    });
}

//删除数据.
function Delete_FrmEleDBs(FK_MapData, keyOfEn, oid) {
    var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
    frmEleDBs.Retrieve("FK_MapData", FK_MapData, "EleID", keyOfEn, "RefPKVal", oid);
    $.each(frmEleDBs, function (i, obj) {
        var frmEleDB = new Entity("BP.Sys.FrmEleDB");
        frmEleDB.MyPK = obj.MyPK
        frmEleDB.Delete();
    });
}
//删除数据.
function Delete_FrmEleDB(keyOfEn, oid, No) {
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = keyOfEn + "_" + oid + "_" + No;
    frmEleDB.Delete();
}
//设置值.
function SaveVal_FrmEleDB(fk_mapdata, keyOfEn, oid, val1, val2) {
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = keyOfEn + "_" + oid + "_" + val1;
    frmEleDB.FK_MapData = fk_mapdata;
    frmEleDB.EleID = keyOfEn;
    frmEleDB.RefPKVal = oid;
    frmEleDB.Tag1 = val1;
    frmEleDB.Tag2 = val2;
    if (frmEleDB.Update() == 0) {
        frmEleDB.Insert();
    }
}
//刷新
function Refresh_Mtags(FK_MapData, AttrOfOper, oid) {
    var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
    frmEleDBs.Retrieve("FK_MapData", FK_MapData, "EleID", AttrOfOper, "RefPKVal", oid);
    var initJsonData = [];
    $.each(frmEleDBs, function (i, o) {
        initJsonData.push({
            "No": o.Tag1,
            "Name": o.Tag2
        });
    });
    $("#" + AttrOfOper + "_mtags").mtags("loadData", initJsonData);
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
    var url = "";

    var host = window.location.href;

    if (host.indexOf('MyFlow') == -1)
        url = 'Pop/TableSearch.htm?FK_MapExt=' + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&PKVal=" + pkval + "&OID=" + pkval + "&KeyOfEn=" + mapExt.AttrOfOper;
    else
        url = './CCForm/Pop/TableSearch.htm?FK_MapExt=' + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&PKVal=" + pkval + "&OID=" + pkval + "&KeyOfEn=" + mapExt.AttrOfOper;

    if (window.parent && window.parent.OpenBootStrapModal) {
        window.parent.OpenBootStrapModal(url, "eudlgframe", mapExt.GetPara("Title"), mapExt.W, mapExt.H, "icon-edit", false, function () { }, null, function () {

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
    tb.bind("click", function () { PopGroupList_Done(mapExt) });
}

function PopGroupList_Done(mapExt) {

    //获得主键.
    var pkval = GetPKVal();

    //弹出这个url, 主要有高度宽度, 可以在  ReturnValCCFormPopValGoogle 上做修改.
    var local = window.location.href;
    var url = "";
    if (local.indexOf('MyFlow')== -1 )
        url = 'Pop/GroupList.htm?FK_MapExt=' + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&PKVal=" + pkval + "&OID=" + pkval + "&KeyOfEn=" + mapExt.AttrOfOper;
    else
        url = 'CCForm/Pop/GroupList.htm?FK_MapExt=' + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&PKVal=" + pkval + "&OID=" + pkval + "&KeyOfEn=" + mapExt.AttrOfOper;

    if (window.parent && window.parent.OpenBootStrapModal) {
        window.parent.OpenBootStrapModal(url, "eudlgframe", "导入数据", mapExt.H, mapExt.W, "icon-edit", true, function () {
            var iframe = document.getElementById("eudlgframe");
            if (iframe) {
                var savefn = iframe.contentWindow.Save;
                if (typeof savefn === "function") {
                    var selectVals = savefn();
                    $("#TB_" + mapExt.AttrOfOper).val(selectVals);
                }
				// 单选复制当前表单
				var selectType = mapExt.GetPara("SelectType");
				if (selectType == "0" && selectVals.length == 1) {
					ValSetter(mapExt.Tag4, selectVals[0]);
				}
            }
        }, null, function () {

        });
        return;
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