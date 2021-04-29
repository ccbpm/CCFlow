//自定义url. ********************************************************************************************************
function SelfUrl(mapExt,targetId,index,oid) {
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
    tb.bind("click", function () { SelfUrl_Done(mapExt,targetId,index,oid) });
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
    
        OpenBootStrapModal(url, "eudlgframe", title, mapExt.H, mapExt.W,
         "icon-edit", true, function () {
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
    if (index == null || index == undefined) {
        container.attr("id", mapExt.AttrOfOper + "_mtags");
        mtagsId = mapExt.AttrOfOper + "_mtags";
    }
    else {
        container.attr("id", mapExt.AttrOfOper + "_mtags_" + index);
        mtagsId = mapExt.AttrOfOper + "_mtags_" + index;
    }
    var mtags = $("#" + mtagsId);
    if (oid == null || oid == undefined)
        oid = GetPKVal();

    //是否可以手工录入
    var isEnter = mapExt.GetPara("IsEnter");
    isEnter = isEnter != null && isEnter != undefined && isEnter == "1"? true : false;
    var title = mapExt.GetPara("Title");
    mtags.mtags({
        "fit": true,
        "FK_MapData": mapExt.FK_MapData,
        "KeyOfEn": mapExt.AttrOfOper,
        "RefPKVal": oid,
        "IsEnter": isEnter,
        "Title": title == null || title == "" ? "选择" :title,
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
            clickEvent(mapExt, targetId, objtr, url, mtagsId, target,oid)
        });
    else
        $("#" + mtagsId + "_Button").bind("click", function () {
            clickEvent(mapExt, targetId, objtr, url, mtagsId, target,oid);
        });

}

function GetInitJsonData(mapExt, oid, val) {
    var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
    frmEleDBs.Retrieve("FK_MapData", mapExt.FK_MapData, "EleID", mapExt.AttrOfOper, "RefPKVal", oid);
    if (frmEleDBs.length == 0 && val != "")
        frmEleDBs = [{ "Tag1": "", "Tag2": val }];
    var initJsonData = [];
    $.each(frmEleDBs, function (i, o) {
        initJsonData.push({
            "No": o.Tag1,
            "Name": o.Tag2,
        });
    });
    return initJsonData;
}
function clickEvent(mapExt, targetId, objtr, url, mtagsId, target,oid) {
    var width = mapExt.W;
    var height = mapExt.H;
    var iframeId = mapExt.MyPK;
    var title = mapExt.GetPara("Title");
    if (window.parent && window.parent.OpenBootStrapModal) {
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


        window.parent.OpenBootStrapModal(url + "&AtParas=" + paras, iframeId, title, width, height, "icon-edit", true, function () {
            var selectType = mapExt.GetPara("SelectType");
            var iframe = window.parent.frames[iframeId];
            if (iframe) {
                var selectedRows = iframe.selectedRows;
                if (iframe.Save)
                        selectedRows = iframe.Save();
                if ($.isArray(selectedRows)) {

                    var mtags = $("#" + mtagsId);
                    mtags.mtags("loadData", GetInitJsonData(mapExt, oid,""));
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
        return;
    }

}
//***************************************树干模式.*****************************************************************
function PopBranches(mapExt, val, targetId, index,oid,objtr) {
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
    if (index == null || index == undefined) {
        container.attr("id", mapExt.AttrOfOper + "_mtags");
        mtagsId = mapExt.AttrOfOper + "_mtags";
    }
    else {
        container.attr("id", mapExt.AttrOfOper + "_mtags_" + index);
        mtagsId = mapExt.AttrOfOper + "_mtags_" + index;
    }
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

    if (window.parent && window.parent.OpenBootStrapModal) {
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
            window.parent.OpenBootStrapModal(url+"&AtParas="+paras, iframeId, title, width, height, "icon-edit", true, function () {
                var selectType = mapExt.GetPara("SelectType");
                var iframe = window.parent.frames[iframeId];
                
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
            return;
        }
}

/******************************************  表格查询 **********************************/
function PopTableSearch(mapExt,val, targetId, index, oid,objtr) {
    
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
   
    
    if (index == null || index == undefined) {
        container.attr("id", mapExt.AttrOfOper + "_mtags");
        mtagsId = mapExt.AttrOfOper + "_mtags";
    }
    else {
        container.attr("id", mapExt.AttrOfOper + "_mtags_" + index);
        mtagsId = mapExt.AttrOfOper + "_mtags_" + index;
    }
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
            clickEvent(mapExt, targetId, objtr, url, mtagsId, target,oid)
        });
    else
        $("#" + mtagsId + "_Button").bind("click", function () {
            clickEvent(mapExt, targetId, objtr, url, mtagsId, target,oid);
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
    if (index == null || index == undefined) {
        container.attr("id", mapExt.AttrOfOper + "_mtags");
        mtagsId = mapExt.AttrOfOper + "_mtags";
    }
    else {
        container.attr("id", mapExt.AttrOfOper + "_mtags_" + index);
        mtagsId = mapExt.AttrOfOper + "_mtags_" + index;
    }
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
    if (index == null || index == undefined) {
        container.attr("id", mapExt.AttrOfOper + "_mtags");
        mtagsId = mapExt.AttrOfOper + "_mtags";
    }
    else {
        container.attr("id", mapExt.AttrOfOper + "_mtags_" + index);
        mtagsId = mapExt.AttrOfOper + "_mtags_" + index;
    }
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
    if (index == null || index == undefined) {
        container.attr("id", mapExt.AttrOfOper + "_mtags");
        mtagsId = mapExt.AttrOfOper + "_mtags";
    }
    else {
        container.attr("id", mapExt.AttrOfOper + "_mtags_" + index);
        mtagsId = mapExt.AttrOfOper + "_mtags_" + index;
    }
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
    if (index == null || index == undefined) {
        container.attr("id", mapExt.AttrOfOper + "_mtags");
        mtagsId = mapExt.AttrOfOper + "_mtags";
    }
    else {
        container.attr("id", mapExt.AttrOfOper + "_mtags_" + index);
        mtagsId = mapExt.AttrOfOper + "_mtags_" + index;
    }
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
       "Title": title == null || title==""?"选择":title,
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


function ValSetter(tag4, key,dbType,dbSource) {
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


/**
 * 获取页面数据
 * */
function getPageData() {
    var formss = $('#divCCForm').serialize();
    var params = "";
    var formArr = formss.split('&');
    var formArrResult = [];
    $.each(formArr, function (i, ele) {
        if (ele.split('=')[0].indexOf('CB_') == 0) {
            //如果ID获取不到值，Name获取到值为复选框多选
            var targetId = ele.split('=')[0];
            if ($('#' + targetId).length == 1) {
                if ($('#' + targetId + ':checked').length == 1) {
                    ele = targetId.replace("CB_","") + '=1';
                } else {
                    ele = targetId.replace("CB_", "") + '=0';
                }
                params += "@" + ele;
            }
        } else if (ele.split('=')[0].indexOf('DDL_') == 0) {
            var ctrlID = ele.split('=')[0];
            var item = $("#" + ctrlID).children('option:checked').text();
            var mystr = ctrlID.replace("DDL_", "") + 'T=' + item;
            params += "@" + mystr;
            params += "@" + ele.replace("DDL_","");
        } else {
            params += "@" + ele.replace("TB_", "");
        }

    });


    
    //获取表单中禁用的表单元素的值
    var disabledEles = $('#divCCForm :disabled');
    $.each(disabledEles, function (i, disabledEle) {

        var name = $(disabledEle).attr('id');

        switch (disabledEle.tagName.toUpperCase()) {

            case "INPUT":
                switch (disabledEle.type.toUpperCase()) {
                    case "CHECKBOX": //复选框
                        params += "@" + name.replace("CB_","") + '=' + $(disabledEle).is(':checked') ? 1 : 0;
                       
                        break;
                    case "TEXT": //文本框
                    case "HIDDEN":
                        params += "@"+name.replace("TB_","") + '=' + $(disabledEle).val();
                        break;
                    case "RADIO": //单选钮
                        name = $(disabledEle).attr('name');
                        var eleResult = name + '=' + $('[name="' + name + '"]:checked').val();
                        params += "@" + eleResult.replace("RB_","");
                        break;
                }
                break;
            //下拉框            
            case "SELECT":
                var tbID = name.replace("DDL_", "TB_") + 'T';
                if ($("#" + tbID).length == 1)
                    params += "@" + tbID.replace("DDL_", "") + '=' + $(disabledEle).children('option:checked').text();
                    
                break;

            //文本区域                    
            case "TEXTAREA":
                params += "@" + name.replace("TB_", "") + '=' + $(disabledEle).val()
                break;
        }
    });

    return params;

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