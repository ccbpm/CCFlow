//自定义url. ********************************************************************************************************
function SelfUrl(mapExt, mapAttr) {
    if (mapAttr.UIVisible == 0)
        return;
    var tb = $("#TB_" + mapExt.AttrOfOper);
    if (tb.length == 0) {
        alert(mapExt.AttrOfOper + "字段删除了.");
        mapExt.Delete();
        return; //有可能字段被删除了.
    }

    //设置文本框只读.
    tb.attr('readonly', 'true');

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

    //暂时未处理
}

/******************************************  树干枝叶模式 **********************************/
function PopBranchesAndLeaf(mapExt, mapAttr) {
    if (mapAttr.UIVisible == 0) {
        return;
    }
    var target = $("#TB_" + mapExt.AttrOfOper);
    target.hide();
    var parentTarget = target.parent();
    var oid = GetPKVal();

    if (mapAttr.UIIsEnable != 0 && mapAttr.UIVisible != 0) {
        //增加a标签
        var localHref = GetLocalWFPreHref();
        var url = localHref + "/CCMobile/CCForm/Pop/BranchesAndLeaf.htm?MyPK=" + mapExt.MyPK + "&oid=" + oid + "&m=" + Math.random();
        var aLink = $('<a class="mui-navigate-right" href="#branchesAndLeaf" ></a>');
        aLink.on('tap', function () {
            initBranchesLPage(mapExt, oid,0);
        });
        aLink.append($('<p style="margin-right:30px;margin-top:10px;min-width:160px;text-align:right;">请选择</p>'));
        parentTarget.append(aLink);
    }

    //增加显示选择结果行
    var width = target.width();
    var height = target.height();
    var container = $("<div class='mui-table-view-cell'></div>");
    parentTarget.after(container);
    container.attr("id", mapExt.AttrOfOper + "_mtags");

    $("#" + mapExt.AttrOfOper + "_mtags").mtags({
        "fit": true,
        "onUnselect": function (record) {
            DeleteFrmEleDB(mapExt.AttrOfOper, oid, record.No);
            var mtags = $("#" + mapExt.AttrOfOper + "_mtags i");
            var len = mtags.length;
            var RemoveFunc = mapExt.GetPara("RemoveFunc");

            if (RemoveFunc) {
                if (RemoveFunc.indexOf("(") == -1) {
                    RemoveFunc = RemoveFunc + "('" + record.No + "','" + len + "')";
                } else {
                    var para = record.No + "','" + len;
                    RemoveFunc = replaceAll(RemoveFunc, "Key", para);
                    RemoveFunc = replaceAll(RemoveFunc, "~", "'");
                }
                //调用移除函数
                DBAccess.RunDBSrc(RemoveFunc, mapExt.DBType,mapExt.FK_DBSrc);
            }
            console.log("unselect: " + JSON.stringify(record));
        }
    });

    //初始加载
    Refresh_Mtags(mapExt.FK_MapData, mapExt.AttrOfOper, oid, mapAttr);
    return;

}
/******************************************  树干模式 **********************************/
function PopBranches(mapExt, mapAttr) {
    if (mapAttr.UIVisible == 0) {
        return;
    }
    var target = $("#TB_" + mapExt.AttrOfOper);
    target.hide();
    var parentTarget = target.parent();
    var oid = GetPKVal();
    if (mapAttr.UIIsEnable != 0 && mapAttr.UIVisible != 0) {
        //增加a标签
        var aLink = $('<a class="mui-navigate-right" href="#branches" ></a>');
        aLink.on('tap', function () {
            initBranchesPage(mapExt, oid,0);
        });
        aLink.append($('<p style="margin-right:30px;margin-top:10px;min-width:160px;text-align:right;">请选择</p>'));
        parentTarget.append(aLink);
    }
    //增加显示选择结果行
    var width = target.width();
    var height = target.height();
    var container = $("<div class='mui-table-view-cell'></div>");
    parentTarget.after(container);
    container.attr("id", mapExt.AttrOfOper + "_mtags");

    $("#" + mapExt.AttrOfOper + "_mtags").mtags({
        "fit": true,
        "onUnselect": function (record) {
            console.log("unselect: " + JSON.stringify(record));
            DeleteFrmEleDB(mapExt.AttrOfOper, oid, record.No);

            var mtags = $("#" + mapExt.AttrOfOper + "_mtags i");
            var len = mtags.length;
            var RemoveFunc = mapExt.GetPara("RemoveFunc");

            if (RemoveFunc) {
                if (RemoveFunc.indexOf("(") == -1) {
                    RemoveFunc = RemoveFunc + "('" + record.No + "','" + len + "')";
                } else {
                    var para = record.No + "','" + len;
                    RemoveFunc = replaceAll(RemoveFunc, "Key", para);
                    RemoveFunc = replaceAll(RemoveFunc, "~", "'");
                }
                //调用移除函数
                DBAccess.RunDBSrc(RemoveFunc, mapExt.DBType,mapExt.FK_DBSrc);
            }
        }
    });
    //初始加载
    Refresh_Mtags(mapExt.FK_MapData, mapExt.AttrOfOper, oid, mapAttr);

}

/******************************************  表格查询 **********************************/
function PopTableSearch(mapExt, mapAttr) {
    if (mapAttr.UIVisible == 0) {
        return;
    }
    var target = $("#TB_" + mapExt.AttrOfOper);
    target.hide();
    var parentTarget = target.parent();
    var oid = GetPKVal();
    if (mapAttr.UIIsEnable != 0 && mapAttr.UIVisible != 0) {
        //增加a标签
        var aLink = $('<a class="mui-navigate-right" href="#tableSearch" ></a>');
        aLink.on('tap', function () {
            if (GetHrefUrl().indexOf("CCForm") != -1) {
                $('head').append('<link href="../Scripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />');
                $('head').append('<link href="../Scripts/bootstrap/bootstrap-table/src/bootstrap-table.css" rel="stylesheet" type="text/css" />');
                Skip.addJs("../Scripts/bootstrap/bootstrap-table/src/bootstrap-table.js");

            } else {
                $('head').append('<link href="./Scripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />');
                $('head').append('<link href="./Scripts/bootstrap/bootstrap-table/src/bootstrap-table.css" rel="stylesheet" type="text/css" />');
                Skip.addJs("./Scripts/bootstrap/bootstrap-table/src/bootstrap-table.js");
            }

            initTableSPage(mapExt, oid,0);
        });
        aLink.append($('<p style="margin-right:30px;margin-top:10px;min-width:160px;text-align:right;">请选择</p>'));
        parentTarget.append(aLink);
    }
    //增加显示选择结果行
    var width = target.width();
    var height = target.height();
    var container = $("<div class='mui-table-view-cell'></div>");
    parentTarget.after(container);
    container.attr("id", mapExt.AttrOfOper + "_mtags");


    $("#" + mapExt.AttrOfOper + "_mtags").mtags({
        "fit": true,
        "onUnselect": function (record) {
            DeleteFrmEleDB(mapExt.AttrOfOper, oid, record.No);
        }
    });

    var width = mapExt.W;
    var height = mapExt.H;
    var iframeId = mapExt.MyPK + mapExt.FK_MapData;
    var title = mapExt.GetPara("Title");
    var oid = GetPKVal();

    //初始加载
    Refresh_Mtags(mapExt.FK_MapData, mapExt.AttrOfOper, oid, mapAttr);

   document.getElementById("TSDone").addEventListener('onclick', function () {
        //获取
        var nos = $("#" + mapExt.AttrOfOper + "_mtags").mtags("getValue");
        var backFunc = mapExt.Tag5;
        if (backFunc != null && backFunc != "" && backFunc != undefined)
            DBAccess.RunFunctionReturnStr(DealSQL(backFunc, nos));
   }, { passive: false })

}

/******************************************  文本自动填充 **********************************/
function TBFullCtrl(mapExt, mapAttr,objID,type) {
    if (mapAttr.UIVisible == 0) {
        return;
    }
    objID = objID == null || objID == undefined ? "TB_" + mapExt.AttrOfOper : objID;
    var target = $("#" + objID);
    var parentTarget = target.parent();
    var oid = GetPKVal();
    
    if (pageData.IsReadonly == "0") {
        //增加a标签
        target.attr("style", "padding-right:30px");
        var aLink = $('<a class="mui-navigate-right" href="#tbFullCtrl" ></a>');
        aLink.on('tap', function () {

            if (GetHrefUrl().indexOf("CCForm") != -1) {
                $('head').append('<link href="../Scripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />');
                $('head').append('<link href="../Scripts/bootstrap/bootstrap-table/src/bootstrap-table.css" rel="stylesheet" type="text/css" />');
                Skip.addJs("../Scripts/bootstrap/bootstrap-table/src/bootstrap-table.js");

            } else {
                $('head').append('<link href="./Scripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />');
                $('head').append('<link href="./Scripts/bootstrap/bootstrap-table/src/bootstrap-table.css" rel="stylesheet" type="text/css" />');
                Skip.addJs("./Scripts/bootstrap/bootstrap-table/src/bootstrap-table.js");
            }
            initTBFullCtrlPage(mapExt, mapAttr, oid, objID,type);
            //alert('ss');
        });
    }
    parentTarget.append(aLink);
   
}

/******************************************  公共处理扩展列的方法 **********************************/

//刷新
function Refresh_Mtags(FK_MapData, AttrOfOper, oid, mapAttr) {
    
    if (frmEleDBs == null) {
        frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
        frmEleDBs.Retrieve("FK_MapData", FK_MapData, "RefPKVal", oid);
    }
  
    var initJsonData = [];
    $.each(frmEleDBs, function (i, o) {
        if (o.EleID == AttrOfOper) {
            initJsonData.push({
                "No": o.Tag1,
                "Name": o.Tag2,
                "POP_Value": o.Tag3
            });
        }
        
    });
    var mtags = $("#" + AttrOfOper + "_mtags")
    mtags.mtags("loadData", initJsonData);

    //给隐藏的控件赋值
    $("#TB_" + AttrOfOper).val(mtags.mtags("getText"));

    if (mapAttr.UIIsEnable == 0 || mapAttr.UIVisible == 0) {
        var divcontainer = mtags.find(".ccflow-input-span-container");
        var spans = divcontainer.children("span");
        $.each(spans, function (i, span) {
            span.innerHTML = span.innerText;
        });
        $("#TB_" + AttrOfOper).attr("placeholder", "");
    }
}

//删除扩展数据.
function DeleteFrmEleDB(keyOfEn, oid, No) {
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = keyOfEn + "_" + oid + "_" + No;
    frmEleDB.Delete();
}

//保存扩展数据.
function SaveFrmEleDB(fk_mapdata, keyOfEn, oid, val1, val2, val3) {
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = keyOfEn + "_" + oid + "_" + val1;
    frmEleDB.FK_MapData = fk_mapdata;
    frmEleDB.EleID = keyOfEn;
    frmEleDB.RefPKVal = oid;
    frmEleDB.Tag1 = val1;
    frmEleDB.Tag2 = val2;
    frmEleDB.Tag3 = val3;
    //if (frmEleDB.Update() == 0) {
    //    frmEleDB.Insert();
    //}
    frmEleDB.Save();
}

//自动填充其他控件
function ValSetter(tag4, key, DBType,dbSrc) {
    if (!tag4 || !key) {
        return;
    }
    tag4 = tag4.replace(/@Key/g, key).replace(/~/g, "'");
    var dt = DBAccess.RunDBSrc(tag4, DBType,dbSrc);
    GenerFullAllCtrlsVal(dt);
}


//改变完成状态
function changeDoneState(count, id) {
    var value = count ? "完成(" + count + ")" : "完成";
    var done = document.getElementById(id);
    done.innerHTML = value;
    if (count) {
        if (done.classList.contains("mui-disabled")) {
            done.classList.remove("mui-disabled");
        }
    } else {
        if (!done.classList.contains("mui-disabled")) {
            //done.classList.add("mui-disabled");
        }
    }
}

//页面退回时给扩展字段赋值
function DealFrmEleDB(id) {
    var selectedRows = global.selectedRows;
    if ($.isArray(selectedRows) && selectedRows.length > 0) {
        var attrOfOper = global.AttrOfOper ? global.AttrOfOper : mapExt.AttrOfOper;
        var ctrlID = "TB_" + attrOfOper;
        var mtags = $("#" + attrOfOper + "_mtags")
        mtags.mtags("loadData", selectedRows);
        $("#" + ctrlID).val(mtags.mtags("getText"));
        // 执行,url
        if (global.selectType == "0" && selectedRows.length == 1 && mapExt.DBType == 1) {
            ValSetter(mapExt.Tag4, selectedRows[0].No, mapExt.DBType,mapExt.FK_DBSrc);
        }
        // 执行,function
        if (mapExt.DBType == 2) {
            var strJson = JSON.stringify(selectedRows);
            var tag4 = mapExt.Tag4.replace(/@Key/g, strJson).replace(/~/g, "'");
            DBAccess.RunDBSrc(tag4, mapExt.DBType,mapExt.FK_DBSrc);
        }
    } else {
        if (id == "tbFullCtrl") {
            //没有选择的时候，搜索框的内容显示到页面上
            $("#TB_" + mapExt.AttrOfOper).val($("#TB_TS_Key").val());
            $("#TB_" + mapExt.AttrOfOper).show()
        }
    }
}

/******************************************  分组列表 **********************************/

function PopGroupList(mapExt, mapAttr) {
    if (mapAttr.UIVisible == 0) {
        return;
    }
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
    var local = GetHrefUrl();
    var url = "";
    if (local.indexOf('MyFlow') == -1)
        url = 'Pop/GroupList.htm?FK_MapExt=' + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&PKVal=" + pkval + "&OID=" + pkval + "&KeyOfEn=" + mapExt.AttrOfOper;
    else
        url = 'CCForm/Pop/GroupList.htm?FK_MapExt=' + mapExt.MyPK + "&FK_MapData=" + mapExt.FK_MapData + "&PKVal=" + pkval + "&OID=" + pkval + "&KeyOfEn=" + mapExt.AttrOfOper;

    //暂时未处理
}

//获取WF之前路径
function GetLocalWFPreHref() {
    var url = GetHrefUrl();
    if (url.indexOf('/CCMobile/') >= 0) {
        var index = url.indexOf('/CCMobile/');
        url = url.substring(0, index);
    }
    return url;
}