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
    var webUser = new WebUser();

    var url = mapExt.Tag;
    if (url.indexOf('?') == -1)
        url = url + "?PKVal=" + pkval + "&UserNo=" + webUser.No;
    var title = mapExt.GetPara("Title");
        OpenBootStrapModal(url, "eudlgframe", title, mapExt.H, mapExt.W,
         "icon-edit", true, function () {
             var iframe = document.getElementById("eudlgframe");
             if (iframe) {
                 var val = iframe.contentWindow.Btn_OK();
                 $("#TB _" + mapExt.AttrOfOper).val(val);
                 FullIt(selectedRows[0].No, mapExt.MyPK, "TB_" + mapExt.AttrOfOper);
             }

         }, null, function () {
            
         });
      
}
//***************************************树干叶子模式*****************************************************************
function PopBranchesAndLeaf(mapExt, val) {
    var target = $("#TB_" + mapExt.AttrOfOper);
    var width = target.outerWidth();
    var height = target.height();
    target.hide();
    var container = $("<div></div>");
    target.after(container-100);
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
    var iframeId = mapExt.MyPK;
    var title = mapExt.GetPara("Title");
    var oid = GetPKVal();

    var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
    frmEleDBs.Retrieve("FK_MapData", mapExt.FK_MapData, "EleID", mapExt.AttrOfOper, "RefPKVal", oid);
    if (frmEleDBs.length == 0 && val != "")
        frmEleDBs = [{ "Tag1": "", "Tag2": val}];
    var initJsonData = [];
    $.each(frmEleDBs, function (i, o) {
        initJsonData.push({
            "No": o.Tag1,
            "Name": o.Tag2
        });
    });
    var mtags = $("#" + mapExt.AttrOfOper + "_mtags");
    mtags.mtags("loadData", initJsonData);
    $("#TB_" + mapExt.AttrOfOper).val(mtags.mtags("getText"));
    //解项羽 这里需要相对路径.
    var localHref = GetLocalWFPreHref();
    var url = localHref + "/WF/CCForm/Pop/BranchesAndLeaf.htm?MyPK=" + mapExt.MyPK + "&oid=" + oid + "&m=" + Math.random();
    container.on("click", function () {
        if (window.parent && window.parent.OpenBootStrapModal) {
            OpenBootStrapModal(url, iframeId, title, width, height, "icon-edit", true, function () {
                var selectType = mapExt.GetPara("SelectType");
                var iframe = document.getElementById(iframeId);
                if (iframe) {
                    var selectedRows = iframe.contentWindow.selectedRows;
                    if ($.isArray(selectedRows)) {
                        mtags.mtags("loadData", selectedRows);
                        $("#TB_" + mapExt.AttrOfOper).val(mtags.mtags("getText"));
                        // 单选复制当前表单
                        if (selectType == "0" && selectedRows.length == 1) {
                            FullIt(selectedRows[0].No, mapExt.MyPK, "TB_" + mapExt.AttrOfOper);
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
            return;
        }

    });

    return;
}

//***************************************树干模式.*****************************************************************
function PopBranches(mapExt, val) {
    var target = $("#TB_" + mapExt.AttrOfOper);
  
    var width = target.outerWidth();
    var height = target.height();
    target.hide();
    var container = $("<div></div>");
    target.after(container);
    container.width(width - 100);
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
    var iframeId = mapExt.MyPK;
    var title = mapExt.GetPara("Title");
    var oid = GetPKVal();
    //初始加载
    Refresh_Mtags(mapExt.FK_MapData, mapExt.AttrOfOper, oid, val);
    //这里需要相对路径.
    var localHref = GetLocalWFPreHref();
    var url = localHref + "/WF/CCForm/Pop/Branches.htm?MyPK=" + mapExt.MyPK + "&oid=" + oid + "&m=" + Math.random();
    container.on("click", function () {
        if (window.parent && window.parent.OpenBootStrapModal) {
            OpenBootStrapModal(url, iframeId, title, width, height, "icon-edit", true, function () {
                var selectType = mapExt.GetPara("SelectType");

                var iframe = document.getElementById(iframeId);
                if (iframe) {
                    var nodes = iframe.contentWindow.GetCheckNodes();
                    var nodeText;
                    if ($.isArray(nodes)) {
                        $.each(nodes, function (i, node) {
                            SaveVal_FrmEleDB(mapExt.FK_MapData, mapExt.AttrOfOper, oid, node.No, node.Name);

                        });
                        //重新加载
                        Refresh_Mtags(mapExt.FK_MapData, mapExt.AttrOfOper, oid, null);

                        // 单选复制当前表单
                        if (selectType == "0" && nodes.length == 1) {
                            ValSetter(mapExt.Tag4, nodes[0].No);
                            FullIt(nodes[0].No, mapExt.MyPK, "TB_" + mapExt.AttrOfOper);
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
            return;
        }
    });
}

/******************************************  表格查询 **********************************/
function PopTableSearch(mapExt) {
    var target = $("#TB_" + mapExt.AttrOfOper);
   
    var width = target.outerWidth();
    var height = target.height();
    target.hide();

    var container = $("<div></div>");
    target.after(container);
    container.width(width-100);
    //container.height(height);
    container.attr("id", mapExt.AttrOfOper + "_mtags");

    $("#" + mapExt.AttrOfOper + "_mtags").mtags({
        "fit": true,
        "onUnselect": function (record) {
            Delete_FrmEleDB(mapExt.AttrOfOper, oid, record.No);

        }
    });

    var width = mapExt.W;
    var height = mapExt.H;
    var iframeId = mapExt.MyPK;
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

    var mtags = $("#" + mapExt.AttrOfOper + "_mtags");
    mtags.mtags("loadData", initJsonData);
    $("#TB_" + mapExt.AttrOfOper).val(mtags.mtags("getText"));

    //解项羽 这里需要相对路径.
    var localHref = GetLocalWFPreHref();
    var url = localHref + "/WF/CCForm/Pop/TableSearch.htm?MyPK=" + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&OID=" + oid + "&KeyOfEn=" + mapExt.AttrOfOper;

    container.on("click", function () {
        if (window.parent && window.parent.OpenBootStrapModal) {
            OpenBootStrapModal(url, iframeId, mapExt.GetPara("Title"), mapExt.W, mapExt.H, "icon-edit", true, function () {
                var selectType = mapExt.GetPara("SelectType");
                var iframe = document.getElementById(iframeId);
                if (iframe) {
                    var selectedRows = iframe.contentWindow.selectedRows;
                    if ($.isArray(selectedRows)) {
                        mtags.mtags("loadData", selectedRows);
                        $("#TB_" + mapExt.AttrOfOper).val(mtags.mtags("getText"));
                        // 单选复制当前表单
                        if (selectType == "0" && selectedRows.length == 1) {
                            FullIt(selectedRows[0].No, mapExt.MyPK, "TB_" + mapExt.AttrOfOper);
                        }
                        //执行JS方法
                        var No = "";
                        if (selectedRows != null && $.isArray(selectedRows))
                            $.each(selectedRows, function (i, selectedRows) {
                                No += selectedRows.No + ",";
                            });
                        //执行JS
                        var backFunc = mapExt.Tag5;
                        if (backFunc != null && backFunc != "" && backFunc != undefined)
                            DBAccess.RunFunctionReturnStr(DealSQL(backFunc, No));
                    }
                }

            }, null, function () {

            }, "div_" + iframeId);
            return;
        }
    });
}

/******************************************  分组平铺列表 **********************************/

function PopGroupList(mapExt) {

    var target = $("#TB_" + mapExt.AttrOfOper);
    

    var width = target.outerWidth();
    var height = target.height();
    target.hide();
    var container = $("<div></div>");
    target.after(container);
    container.width(width-100);
    //container.height(height);
    container.attr("id", mapExt.AttrOfOper + "_mtags");

    $("#" + mapExt.AttrOfOper + "_mtags").mtags({
        "fit": true,
        "onUnselect": function (record) {
            Delete_FrmEleDB(mapExt.AttrOfOper, oid, record.No);

        }
    });

    var width = mapExt.W;
    var height = mapExt.H;
    var iframeId = mapExt.MyPK;
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

    var mtags = $("#" + mapExt.AttrOfOper + "_mtags");
    mtags.mtags("loadData", initJsonData);
    $("#TB_" + mapExt.AttrOfOper).val(mtags.mtags("getText"));

    //解项羽 这里需要相对路径.
    var localHref = GetLocalWFPreHref();
    var url = localHref + "/WF/CCForm/Pop/GroupList.htm?FK_MapExt=" + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&PKVal=" + oid + "&OID=" + oid + "&KeyOfEn=" + mapExt.AttrOfOper;

    container.on("click", function () {
        if (window.parent && window.parent.OpenBootStrapModal) {
            OpenBootStrapModal(url, iframeId, mapExt.GetPara("Title"), mapExt.W, mapExt.H, "icon-edit", true, function () {
                var selectType = mapExt.GetPara("SelectType");

                var iframe = document.getElementById(iframeId);
                if (iframe) {
                    var selectedRows = iframe.contentWindow.Save();
                    if ($.isArray(selectedRows)) {
                        mtags.mtags("loadData", selectedRows);
                        $("#TB_" + mapExt.AttrOfOper).val(mtags.mtags("getText"));
                        // 单选复制当前表单
                        if (selectType == "0" && selectedRows.length == 1) {
                            FullIt(selectedRows[0].No, mapExt.MyPK, "TB_" + mapExt.AttrOfOper);
                        }

                        //执行JS方法
                        var No = "";
                        if (selectedRows != null && $.isArray(selectedRows))
                            $.each(selectedRows, function (i, selectedRows) {
                                No += selectedRows.No + ",";
                            });
                        //执行JS
                        var backFunc = mapExt.Tag5;
                        if (backFunc != null && backFunc != "" && backFunc != undefined)
                            DBAccess.RunFunctionReturnStr(DealSQL(backFunc, No));
                    }
                }

            }, null, function () {

            }, "div_" + iframeId);
            return;
        }
    });

}

function ValSetter(tag4, key) {
    if (!tag4 || !key) {
        return;
    }
    tag4 = tag4.replace(/@Key/g, key).replace(/~/g, "'");
    var dt = DBAccess.RunDBSrc(tag4);
    GenerFullAllCtrlsVal(dt);
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
    $("#TB_" + keyOfEn).val('');
}
//删除数据.
function Delete_FrmEleDB(keyOfEn, oid, No) {
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = keyOfEn + "_" + oid + "_" + No;
    frmEleDB.Delete();
    $("#TB_" + keyOfEn).val('');
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
function Refresh_Mtags(FK_MapData, AttrOfOper, oid, val) {
    var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
    frmEleDBs.Retrieve("FK_MapData", FK_MapData, "EleID", AttrOfOper, "RefPKVal", oid);
    var initJsonData = [];
    if (frmEleDBs.length == 0 && val != null && val != "")
        frmEleDBs = [{ "Tag1": "", "Tag2": val}];
    $.each(frmEleDBs, function (i, o) {
        initJsonData.push({
            "No": o.Tag1,
            "Name": o.Tag2
        });
    });
    var mtags = $("#" + AttrOfOper + "_mtags");
    mtags.mtags("loadData", initJsonData);
    $("#TB_" + AttrOfOper).val(mtags.mtags("getText"));
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