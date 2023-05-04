//自定义url. ********************************************************************************************************
function SelfUrl(mapExt, targetId, index, oid) {
    if (targetId == null || targetId == undefined)
        targetId = "TB_" + mapExt.AttrOfOper;

    var tb = $("#" + targetId);
    if (tb.length == 0) {
        alert(mapExt.AttrOfOper + "字段删除了.");
        mapExt.Delete();
        return; //有可能字段被删除了.
    }

    //设置文本框只读.
    tb.attr('readonly', 'true');
    // tb.attr('disabled', 'true');

    //在文本框双击，绑定弹出. PopGroupList.htm的窗口. 
    tb.bind("click", function () {
        SelfUrl_Done(mapExt, this.id, index, oid)
    });
}

function SelfUrl_Done(mapExt, targetId, index, pkval) {

    //获得主键.
    if (pkval == null || pkval == undefined)
        pkval = GetPKVal();
    var webUser = new WebUser();

    var url = mapExt.Tag;
    if (url.indexOf('?') == -1)
        url = url + "?PKVal=" + pkval + "&UserNo=" + webUser.No;
    var title = mapExt.GetPara("Title");
    var width = mapExt.W;
    var height = mapExt.H;

    if (width > window.innerWidth || width < window.innerWidth / 2)
        width = window.innerWidth * 4 / 5;
    if (height > window.innerHeight || height < window.innerHeight / 2)
        height = 250;
    else
        height = height / window.innerHeight * 100;

    if (window.parent && typeof window.parent.OpenLayuiDialog == "function") {
        window.parent.OpenLayuiDialog(url, title, width, 80, "auto", false, true, true, function () {
            var iframe = $(window.parent.frames["dlg"]).find("iframe");
            if (iframe.length > 0) {
                iframe = iframe[0].contentWindow;
                if (typeof iframe.Btn_OK == "function") {
                    var val = iframe.Btn_OK;
                    $("#" + targetId).val(val);
                    FullIt(val, mapExt.MyPK, targetId);
                }
            }
        })
        return;
    }
    OpenBootStrapModal(url, "eudlgframe", title, mapExt.H, mapExt.W,"icon-edit", true, function () {
        var iframe = document.getElementById("eudlgframe");
        if (iframe) {
            var val = iframe.contentWindow.Btn_OK();
            $("#" + targetId).val(val);
            FullIt(val, mapExt.MyPK, targetId);
        }

    }, null, function () {

    });

}
//***************************************树干叶子模式*****************************************************************
function PopBranchesAndLeaf(mapExt, val, targetId, index, oid, objtr) {

    var mtagsId;
    if (targetId == null || targetId == undefined)
        targetId = "TB_" + mapExt.AttrOfOper;

    var target = $("#" + targetId);

    var width = target.outerWidth();
    var height = target.outerHeight();
    target.hide();
    var container = $("<div></div>");
    target.after(container);
    container.width(width);
    container.css("min-height", height);
    if (index == null || index == undefined)
        mtagsId = mapExt.AttrOfOper + "_mtags";
    else
        mtagsId = mapExt.AttrOfOper + "_mtags_" + index;

    if ($("#" + mtagsId).length != 0)
        $("#" + mtagsId).remove();
    container.attr("id", mtagsId);
    var mtags = $("#" + mtagsId);
    if (oid == null || oid == undefined)
        oid = GetPKVal();

    //是否可以手工录入
    var isEnter = mapExt.GetPara("IsEnter");
    isEnter = isEnter != null && isEnter != undefined && isEnter == "1" ? true : false;
    var title = mapExt.GetPara("Title");
    mtags.mtags({
        "fit": true,
        "FK_MapData": mapExt.FK_MapData,
        "KeyOfEn": mapExt.AttrOfOper,
        "RefPKVal": oid,
        "IsEnter": isEnter,
        "Title": title == null || title == "" ? "选择" : title,
        "onUnselect": function (record) {
            Delete_FrmEleDB(mapExt.AttrOfOper, oid, record.No);
            console.log("unselect: " + JSON.stringify(record));
        }
    });



    mtags.mtags("loadData", GetInitJsonData(mapExt, oid, val));
    $("#" + targetId).val(mtags.mtags("getText"));
    //解项羽 这里需要相对路径.
    var localHref = GetLocalWFPreHref();
    var url = localHref + "/WF/CCForm/Pop/BranchesAndLeaf.htm?MyPK=" + mapExt.MyPK + "&oid=" + oid + "&m=" + Math.random();
    if (isEnter == false)
        container.on("dblclick", function () {
            clickEvent(mapExt, targetId, objtr, url, mtagsId, target, oid)
        });
    else
        $("#" + mtagsId + "_Button").bind("click", function () {
            clickEvent(mapExt, targetId, objtr, url, mtagsId, target, oid);
        });

}

function GetInitJsonData(mapExt, oid, val) {
    var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
    frmEleDBs.Retrieve("FK_MapData", mapExt.FK_MapData, "EleID", mapExt.AttrOfOper, "RefPKVal", oid);
    /*if (frmEleDBs.length == 0 && val != "")
        frmEleDBs = [{ "Tag1": "", "Tag2": val }];*/
    var initJsonData = [];
    $.each(frmEleDBs, function (i, o) {
        initJsonData.push({
            "No": o.Tag1,
            "Name": o.Tag2,
        });
    });
    return initJsonData;
}
function clickEvent(mapExt, targetId, objtr, url, mtagsId, target, oid) {
    var width = mapExt.W;
    var height = mapExt.H;
    var iframeId = mapExt.MyPK;
    var title = mapExt.GetPara("Title");

    if (width > window.innerWidth || width < window.innerWidth / 2)
        width = window.innerWidth * 4 / 5;
    if (height > window.innerHeight || height < window.innerHeight / 2)
        height = 250;
    else
        height = height / window.innerHeight * 100;

    //参数传递
    var data = "";
    var paras = "";
    if (objtr == "" || objtr == null || objtr == undefined) {
        //获取表单中字段的数据
        paras = getPageData();
    }
    else {
        data = $(objtr).data().data;
        Object.keys(data).forEach(function (key) {
            if (key == "OID" || key == "FID" || key == "Rec" || key == "RefPK" || key == "RDT") { }
            else {
                paras += "@" + key + "=" + data[key];
            }
        });
    }

    if (window.parent && typeof window.parent.OpenLayuiDialog == "function") {
        window.parent.OpenLayuiDialog(url + "&AtParas=" + paras, title, width, 80, "auto", false, true, true, function () {
            var selectType = mapExt.GetPara("SelectType");
            var iframe = $(window.parent.frames["dlg"]).find("iframe");
            if (iframe.length > 0) {
                iframe = iframe[0].contentWindow;
                var selectedRows = iframe.selectedRows;
                if (iframe.Save)
                    selectedRows = iframe.Save();
                if ($.isArray(selectedRows)) {

                    var mtags = $("#" + mtagsId);
                    mtags.mtags("loadData", GetInitJsonData(mapExt, oid, ""));
                    target.val(mtags.mtags("getText"));
                    // 单选复制当前表单
                    if (selectType == "0" && selectedRows.length == 1) {
                        FullIt(selectedRows[0].No, mapExt.MyPK, targetId);
                    }
                    var No = "";
                    if (selectedRows != null && $.isArray(selectedRows))
                        $.each(selectedRows, function (i, selectedRow) {
                            No += selectedRow.No + ",";
                        });
                    //执行JS
                    var backFunc = mapExt.Tag5;
                    if (backFunc != null && backFunc != "" && backFunc != undefined)
                        DBAccess.RunFunctionReturnStr(DealSQL(backFunc, No));

                }
            }
        });
    } else {
        window.OpenBootStrapModal(url + "&AtParas=" + paras, iframeId, title, width, height, "icon-edit", true, function () {
            var selectType = mapExt.GetPara("SelectType");
            var iframe = window.frames[iframeId];
            if (iframe) {
                var selectedRows = iframe.selectedRows;
                if (iframe.Save)
                    selectedRows = iframe.Save();
                if ($.isArray(selectedRows)) {

                    var mtags = $("#" + mtagsId);
                    mtags.mtags("loadData", GetInitJsonData(mapExt, oid, ""));
                    target.val(mtags.mtags("getText"));
                    // 单选复制当前表单
                    if (selectType == "0" && selectedRows.length == 1) {
                        FullIt(selectedRows[0].No, mapExt.MyPK, targetId);
                    }
                    var No = "";
                    if (selectedRows != null && $.isArray(selectedRows))
                        $.each(selectedRows, function (i, selectedRow) {
                            No += selectedRow.No + ",";
                        });
                    //执行JS
                    var backFunc = mapExt.Tag5;
                    if (backFunc != null && backFunc != "" && backFunc != undefined)
                        DBAccess.RunFunctionReturnStr(DealSQL(backFunc, No));

                }
            }
        }, null, function () {

        }, "div_" + iframeId);
    }

}
//***************************************树干模式.*****************************************************************
function PopBranches(mapExt, val, targetId, index, oid, objtr) {
    var mtagsId;
    if (targetId == null || targetId == undefined)
        targetId = "TB_" + mapExt.AttrOfOper;

    var target = $("#" + targetId);

    var width = target.outerWidth();
    var height = target.outerHeight();
    target.hide();
    var container = $("<div></div>");
    target.after(container);
    container.width(width);
    container.css("min-height", height);
    container.attr("id", mapExt.AttrOfOper + "_mtags");
    var mtags;
    if (index == null || index == undefined)
        mtagsId = mapExt.AttrOfOper + "_mtags";
    else
        mtagsId = mapExt.AttrOfOper + "_mtags_" + index;

    if ($("#" + mtagsId).length != 0)
        $("#" + mtagsId).remove();
    container.attr("id", mtagsId);

    var mtags = $("#" + mtagsId);
    if (oid == null || oid == undefined)
        oid = GetPKVal();

    //是否可以手工录入
    var isEnter = mapExt.GetPara("IsEnter");
    isEnter = isEnter != null && isEnter != undefined && isEnter == "1" ? true : false;
    var title = mapExt.GetPara("Title");
    mtags.mtags({
        "fit": true,
        "FK_MapData": mapExt.FK_MapData,
        "KeyOfEn": mapExt.AttrOfOper,
        "RefPKVal": oid,
        "IsEnter": isEnter,
        "Title": title == null || title == "" ? "选择" : title,
        "onUnselect": function (record) {
            console.log("unselect: " + JSON.stringify(record));
            Delete_FrmEleDB(mapExt.AttrOfOper, oid, record.No);

        }
    });

    //初始加载
    mtags.mtags("loadData", GetInitJsonData(mapExt, oid, val));
    $("#" + targetId).val(mtags.mtags("getText"));


    //这里需要相对路径.
    var localHref = GetLocalWFPreHref();
    var url = localHref + "/WF/CCForm/Pop/Branches.htm?MyPK=" + mapExt.MyPK + "&oid=" + oid + "&m=" + Math.random();
    if (isEnter == false)
        container.on("dblclick", function () {
            clickBranchesEvent(mapExt, targetId, objtr, url, mtagsId, target, oid)
        });
    else
        $("#" + mtagsId + "_Button").bind("click", function () {
            clickBranchesEvent(mapExt, targetId, objtr, url, mtagsId, target, oid);
        });

}

function clickBranchesEvent(mapExt, targetId, objtr, url, mtagsId, target, oid) {
    var width = mapExt.W;
    var height = mapExt.H;
    var iframeId = mapExt.MyPK;
    var title = mapExt.GetPara("Title");
    if (width > window.innerWidth || width < window.innerWidth / 2)
        width = window.innerWidth * 4 / 5;
    if (height > window.innerHeight || height < window.innerHeight / 2)
        height = 50;
    else
        height = height / window.innerHeight * 100;
    //传递参数
    var data = "";
    var paras = "";
    if (objtr == "" || objtr == null || objtr == undefined) {
        //获取表单中字段的数据
        paras = getPageData();
    }
    else {
        data = $(objtr).data().data;
        Object.keys(data).forEach(function (key) {
            if (key == "OID" || key == "FID" || key == "Rec" || key == "RefPK" || key == "RDT") { }
            else {
                paras += "@" + key + "=" + data[key];
            }
        });
    }
    if (window.parent && typeof window.parent.OpenLayuiDialog == "function") {
        window.parent.OpenLayuiDialog(url + "&AtParas=" + paras, title, width, 80, "auto", false, true, true, function () {
            var selectType = mapExt.GetPara("SelectType");
            var iframe = $(window.parent.frames["dlg"]).find("iframe");
            if (iframe.length > 0) {
                iframe = iframe[0].contentWindow;
                //删除保存的数据
                var initJsonData = [];
                initJsonData = Delete_FrmEleDBs(mapExt.FK_MapData, mapExt.AttrOfOper, oid, initJsonData);
                var nodes = iframe.GetCheckNodes();

                mtags = $("#" + mtagsId);

                if ($.isArray(nodes)) {
                    $.each(nodes, function (i, node) {
                        initJsonData.push({
                            "No": node.No,
                            "Name": node.Name
                        });
                    });

                    mtags.mtags("loadData", initJsonData);
                    $("#" + targetId).val(mtags.mtags("getText"));

                    // 单选复制当前表单
                    if (selectType == "0" && nodes.length == 1) {
                        FullIt(nodes[0].No, mapExt.MyPK, targetId);
                    }

                    //执行JS方法
                    var No = "";
                    if (nodes != null && $.isArray(nodes))
                        $.each(nodes, function (i, nodes) {
                            No += nodes.No + ",";
                        });
                    //执行JS
                    var backFunc = mapExt.Tag5;
                    if (backFunc != null && backFunc != "" && backFunc != undefined)
                        DBAccess.RunFunctionReturnStr(DealSQL(backFunc, No));
                }
            }
        })
    } else {
        window.OpenBootStrapModal(url + "&AtParas=" + paras, iframeId, title, width, height, "icon-edit", true, function () {
            var selectType = mapExt.GetPara("SelectType");
            var iframe = window.frames[iframeId];

            if (iframe) {
                //删除保存的数据
                var initJsonData = [];
                initJsonData = Delete_FrmEleDBs(mapExt.FK_MapData, mapExt.AttrOfOper, oid, initJsonData);
                var nodes = iframe.GetCheckNodes();

                mtags = $("#" + mtagsId);

                if ($.isArray(nodes)) {
                    $.each(nodes, function (i, node) {
                        initJsonData.push({
                            "No": node.No,
                            "Name": node.Name
                        });
                    });

                    mtags.mtags("loadData", initJsonData);
                    $("#" + targetId).val(mtags.mtags("getText"));

                    // 单选复制当前表单
                    if (selectType == "0" && nodes.length == 1) {
                        FullIt(nodes[0].No, mapExt.MyPK, targetId);
                    }

                    //执行JS方法
                    var No = "";
                    if (nodes != null && $.isArray(nodes))
                        $.each(nodes, function (i, nodes) {
                            No += nodes.No + ",";
                        });
                    //执行JS
                    var backFunc = mapExt.Tag5;
                    if (backFunc != null && backFunc != "" && backFunc != undefined)
                        DBAccess.RunFunctionReturnStr(DealSQL(backFunc, No));
                }
            }
        }, null, function () {

        }, "div_" + iframeId);

    }

}

/******************************************  表格查询 **********************************/
function PopTableSearch(mapExt, val, targetId, index, oid, objtr) {

    var mtagsId;
    if (targetId == null || targetId == undefined)
        targetId = "TB_" + mapExt.AttrOfOper;

    var target = $("#" + targetId);

    var width = target.outerWidth();
    var height = target.outerHeight();
    target.hide();

    var container = $("<div></div>");
    target.after(container);
    container.width(width);


    if (index == null || index == undefined)
        mtagsId = mapExt.AttrOfOper + "_mtags";
    else
        mtagsId = mapExt.AttrOfOper + "_mtags_" + index;

    if ($("#" + mtagsId).length != 0)
        $("#" + mtagsId).remove();
    container.attr("id", mtagsId);

    if (oid == null || oid == undefined)
        oid = GetPKVal();

    //是否可以手工录入
    var isEnter = mapExt.GetPara("IsEnter");
    isEnter = isEnter != null && isEnter != undefined && isEnter == "1" ? true : false;
    var title = mapExt.GetPara("Title");
    var mtags = $("#" + mtagsId);
    mtags.mtags({
        "fit": true,
        "FK_MapData": mapExt.FK_MapData,
        "KeyOfEn": mapExt.AttrOfOper,
        "RefPKVal": oid,
        "IsEnter": isEnter,
        "Title": title == null || title == "" ? "选择" : title,
        "onUnselect": function (record) {
            Delete_FrmEleDB(mapExt.AttrOfOper, oid, record.No);

        }
    });

    mtags.mtags("loadData", GetInitJsonData(mapExt, oid, val));
    target.val(mtags.mtags("getText"));

    //解项羽 这里需要相对路径.
    var localHref = GetLocalWFPreHref();
    var url = localHref + "/WF/CCForm/Pop/TableSearch.htm?MyPK=" + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&OID=" + oid + "&KeyOfEn=" + mapExt.AttrOfOper;

    if (isEnter == false)
        container.on("dblclick", function () {
            clickEvent(mapExt, targetId, objtr, url, mtagsId, target, oid)
        });
    else
        $("#" + mtagsId + "_Button").bind("click", function () {
            clickEvent(mapExt, targetId, objtr, url, mtagsId, target, oid);
        });

}

/******************************************  分组平铺列表 **********************************/

function PopGroupList(mapExt, targetId, index, oid) {

    var mtagsId;
    if (targetId == null || targetId == undefined)
        targetId = "TB_" + mapExt.AttrOfOper;

    var target = $("#" + targetId);


    var width = target.outerWidth();
    var height = target.outerHeight();
    target.hide();
    var container = $("<div></div>");
    target.after(container);
    container.width(width);
    container.height(height);
    if (index == null || index == undefined)
        mtagsId = mapExt.AttrOfOper + "_mtags";
    else
        mtagsId = mapExt.AttrOfOper + "_mtags_" + index;

    if ($("#" + mtagsId).length != 0)
        $("#" + mtagsId).remove();
    container.attr("id", mtagsId);
    if (oid == null || oid == undefined)
        oid = GetPKVal();
    var mtags = $("#" + mtagsId);

    //是否可以手工录入
    var isEnter = mapExt.GetPara("IsEnter");
    isEnter = isEnter != null && isEnter != undefined && isEnter == "1" ? true : false;
    var title = mapExt.GetPara("Title");
    mtags.mtags({
        "fit": true,
        "FK_MapData": mapExt.FK_MapData,
        "KeyOfEn": mapExt.AttrOfOper,
        "RefPKVal": oid,
        "IsEnter": isEnter,
        "Title": title == null || title == "" ? "选择" : title,
        "onUnselect": function (record) {
            Delete_FrmEleDB(mapExt.AttrOfOper, oid, record.No);

        }
    });
    mtags.mtags("loadData", GetInitJsonData(mapExt, oid, ""));
    target.val(mtags.mtags("getText"));

    //解项羽 这里需要相对路径.
    var localHref = GetLocalWFPreHref();
    var url = localHref + "/WF/CCForm/Pop/GroupList.htm?FK_MapExt=" + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&PKVal=" + oid + "&OID=" + oid + "&KeyOfEn=" + mapExt.AttrOfOper;
    if (isEnter == false)
        container.on("dblclick", function () {
            clickEvent(mapExt, targetId, null, url, mtagsId, target, oid)
        });
    else
        $("#" + mtagsId + "_Button").bind("click", function () {
            clickEvent(mapExt, targetId, null, url, mtagsId, target, oid);
        });


}


/******************************************  绑定外键-外部数据源 **********************************/

function PopBindSFTable(mapExt, targetId, index, oid) {

    var mtagsId;
    if (targetId == null || targetId == undefined)
        targetId = "TB_" + mapExt.AttrOfOper;

    var target = $("#" + targetId);

    var width = target.outerWidth();
    var height = target.outerHeight();
    target.hide();
    var container = $("<div></div>");
    target.after(container);
    container.width(width);
    if (index == null || index == undefined)
        mtagsId = mapExt.AttrOfOper + "_mtags";
    else
        mtagsId = mapExt.AttrOfOper + "_mtags_" + index;

    if ($("#" + mtagsId).length != 0)
        $("#" + mtagsId).remove();
    container.attr("id", mtagsId);
    var mtags = $("#" + mtagsId);
    var oid = GetPKVal();

    //是否可以手工录入
    var isEnter = mapExt.GetPara("IsEnter");
    isEnter = isEnter != null && isEnter != undefined && isEnter == "1" ? true : false;
    var title = mapExt.GetPara("Title");
    mtags.mtags({
        "fit": true,
        "FK_MapData": mapExt.FK_MapData,
        "KeyOfEn": mapExt.AttrOfOper,
        "RefPKVal": oid,
        "IsEnter": isEnter,
        "Title": title == null || title == "" ? "选择" : title,
        "onUnselect": function (record) {
            Delete_FrmEleDB(mapExt.AttrOfOper, oid, record.No);

        }
    });


    mtags.mtags("loadData", GetInitJsonData(mapExt, oid, ""));
    target.val(mtags.mtags("getText"));

    //解项羽 这里需要相对路径.
    var localHref = GetLocalWFPreHref();
    var url = localHref + "/WF/CCForm/Pop/BindSFTable.htm?FK_MapExt=" + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&PKVal=" + oid + "&OID=" + oid + "&KeyOfEn=" + mapExt.AttrOfOper;

    if (isEnter == false)
        container.on("dblclick", function () {
            clickEvent(mapExt, targetId, null, url, mtagsId, target, oid)
        });
    else
        $("#" + mtagsId + "_Button").bind("click", function () {
            clickEvent(mapExt, targetId, null, url, mtagsId, target, oid);
        });
}



/******************************************  绑定外键-单表数据源 PopTableList **********************************/

function PopTableList(mapExt, targetId, index, oid) {

    var mtagsId;
    if (targetId == null || targetId == undefined)
        targetId = "TB_" + mapExt.AttrOfOper;

    var target = $("#" + targetId);

    var width = target.outerWidth();
    var height = target.outerHeight();
    target.hide();
    var container = $("<div></div>");
    target.after(container);
    container.width(width);
    if (index == null || index == undefined)
        mtagsId = mapExt.AttrOfOper + "_mtags";
    else
        mtagsId = mapExt.AttrOfOper + "_mtags_" + index;

    if ($("#" + mtagsId).length != 0)
        $("#" + mtagsId).remove();
    container.attr("id", mtagsId);
    var mtags = $("#" + mtagsId);

    if (oid == null || oid == undefined)
        oid = GetPKVal();

    //是否可以手工录入
    var isEnter = mapExt.GetPara("IsEnter");
    isEnter = isEnter != null && isEnter != undefined && isEnter == "1" ? true : false;
    var title = mapExt.GetPara("Title");
    mtags.mtags({
        "fit": true,
        "FK_MapData": mapExt.FK_MapData,
        "KeyOfEn": mapExt.AttrOfOper,
        "RefPKVal": oid,
        "IsEnter": isEnter,
        "Title": title == null || title == "" ? "选择" : title,
        "onUnselect": function (record) {
            Delete_FrmEleDB(mapExt.AttrOfOper, oid, record.No);

        }
    });

    mtags.mtags("loadData", GetInitJsonData(mapExt, oid, ""));
    target.val(mtags.mtags("getText"));

    //解项羽 这里需要相对路径.
    var localHref = GetLocalWFPreHref();
    var url = localHref + "/WF/CCForm/Pop/TableList.htm?FK_MapExt=" + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&PKVal=" + oid + "&OID=" + oid + "&KeyOfEn=" + mapExt.AttrOfOper;

    if (isEnter == false)
        container.on("dblclick", function () {
            clickEvent(mapExt, targetId, null, url, mtagsId, target, oid)
        });
    else
        $("#" + mtagsId + "_Button").bind("click", function () {
            clickEvent(mapExt, targetId, null, url, mtagsId, target, oid);
        });
}






/******************************************  绑定枚举 **********************************/

function PopBindEnum(mapExt, targetId, index, oid) {

    var mtagsId;
    if (targetId == null || targetId == undefined)
        targetId = "TB_" + mapExt.AttrOfOper;

    var target = $("#" + targetId);

    var width = target.outerWidth();
    var height = target.outerHeight();
    target.hide();
    var container = $("<div></div>");
    target.after(container);
    container.width(width);
    if (index == null || index == undefined)
        mtagsId = mapExt.AttrOfOper + "_mtags";
    else
        mtagsId = mapExt.AttrOfOper + "_mtags_" + index;

    if ($("#" + mtagsId).length != 0)
        $("#" + mtagsId).remove();
    container.attr("id", mtagsId);
    var mtags = $("#" + mtagsId);

    if (oid == null || oid == undefined)
        oid = GetPKVal();

    //是否可以手工录入
    var isEnter = mapExt.GetPara("IsEnter");
    isEnter = isEnter != null && isEnter != undefined && isEnter == "1" ? true : false;
    var title = mapExt.GetPara("Title");
    mtags.mtags({
        "fit": true,
        "FK_MapData": mapExt.FK_MapData,
        "KeyOfEn": mapExt.AttrOfOper,
        "RefPKVal": oid,
        "IsEnter": isEnter,
        "Title": title == null || title == "" ? "选择" : title,
        "onUnselect": function (record) {
            Delete_FrmEleDB(mapExt.AttrOfOper, oid, record.No);

        }
    });

    mtags.mtags("loadData", GetInitJsonData(mapExt, oid, ""));
    target.val(mtags.mtags("getText"));

    //解项羽 这里需要相对路径.
    var localHref = GetLocalWFPreHref();
    var url = localHref + "/WF/CCForm/Pop/BindEnum.htm?FK_MapExt=" + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&PKVal=" + oid + "&OID=" + oid + "&KeyOfEn=" + mapExt.AttrOfOper;

    if (isEnter == false)
        container.on("dblclick", function () {
            clickEvent(mapExt, targetId, null, url, mtagsId, target, oid)
        });
    else
        $("#" + mtagsId + "_Button").bind("click", function () {
            clickEvent(mapExt, targetId, null, url, mtagsId, target, oid);
        });
}


function ValSetter(tag4, key, dbType, dbSource) {
    if (!tag4 || !key) {
        return;
    }
    tag4 = tag4.replace(/@Key/g, key).replace(/~/g, "'");
    tag4 = tag4.replace(/@key/g, key).replace(/~/g, "'");
    tag4 = tag4.replace(/@KEY/g, key).replace(/~/g, "'");

    var dt = DBAccess.RunDBSrc(tag4, dbType, dbSource);
    GenerFullAllCtrlsVal(dt);
}

//删除数据.
function Delete_FrmEleDBs(FK_MapData, keyOfEn, oid, initJsonData) {
    var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
    frmEleDBs.Retrieve("FK_MapData", FK_MapData, "EleID", keyOfEn, "RefPKVal", oid);
    $.each(frmEleDBs, function (i, obj) {
        if (obj.Tag5 != "1") {
            var frmEleDB = new Entity("BP.Sys.FrmEleDB");
            frmEleDB.MyPK = obj.MyPK
            frmEleDB.Delete();
        } else {
            initJsonData.push({
                "No": obj.Tag1,
                "Name": obj.Tag2
            });
        }

    });
    $("#TB_" + keyOfEn).val('');
    return initJsonData;
}

//删除数据.
function Delete_FrmEleDB(keyOfEn, oid, No) {
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = keyOfEn + "_" + oid + "_" + No;
    frmEleDB.Delete();
    //$("#TB_" + keyOfEn).val(target.getText);
}
//设置值.
function SaveVal_FrmEleDB(fk_mapdata, keyOfEn, oid, val1, val2, tag5) {
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = keyOfEn + "_" + oid + "_" + val1;
    frmEleDB.FK_MapData = fk_mapdata;
    frmEleDB.EleID = keyOfEn;
    frmEleDB.RefPKVal = oid;
    frmEleDB.Tag1 = val1;
    frmEleDB.Tag2 = val2;
    frmEleDB.Tag5 = tag5;
    if (frmEleDB.Update() == 0) {
        frmEleDB.Insert();
    }
}





//获取WF之前路径
function GetLocalWFPreHref() {
    var url = GetHrefUrl();
    if (url.indexOf('/WF/') >= 0) {
        var index = url.indexOf('/WF/');
        url = url.substring(0, index);
    }
    return url;
}