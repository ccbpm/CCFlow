/**
 * xm-select通用的单选，多选
 * @param {any} popType 弹出框类型
 * @param {any} mapAttr 字段属性
 * @param {any} mapExt 扩展属性
 * @param {any} frmData 表单数据
 */
function CommPop(popType, mapAttr, mapExt, frmData, mapExts) {
  
    if (mapAttr.UIIsEnable == 0 || isReadonly == true) {
        //只显示
        return;
    }
    //单选还是多选
    var selectType = mapExt.SelectType;
    selectType = selectType == null || selectType == undefined || selectType == "" ? 1 : selectType;
    var pkVal = pageData.WorkID;
    pkVal = pkVal == undefined || pkVal == 0 ? pageData.OID : pkVal;

    //选中的值
    var selects = new Entities("BP.Sys.FrmEleDBs");
    selects.Retrieve("FK_MapData", mapExt.FK_MapData, "EleID", mapExt.AttrOfOper, "RefPKVal", pkVal);
    var data = [];
    //获取实体信息
    var ens = [];
    if (popType == "PopBranches") {
        $("#TB_" + mapAttr.KeyOfEn).hide();
        $("#TB_" + mapAttr.KeyOfEn).after("<div id='mapExt_" + mapAttr.KeyOfEn + "' style='width:99%'></div>")
        xmSelectTree("mapExt_" + mapAttr.KeyOfEn, mapExt, selects, popType, selectType);
        return;
    }
    if (popType == "PopBindEnum") {
        ens = new Entities("BP.Sys.SysEnums");
        ens.Retrieve("EnumKey", mapExt.Tag2);
        $.each(ens, function (i, item) {
            data.push({
                No: item.IntKey,
                Name: item.Lab,
                selected: IsHaveSelect(item.IntKey, selects)
            })
        })
    }
    else if (popType == "PopBindSFTable") {
        var en = new Entity("BP.Sys.SFTable", mapExt.Tag2);
        ens = en.DoMethodReturnJSON("GenerDataOfJson");
    } else {
        ens = GetDataTableByDB(mapExt.Tag2, mapExt.DBType, mapExt.FK_DBSrc, null);
    }
       
    //如果是分组的时候处理
    if (popType == "PopGroupList") {
        //获取分组信息
        var groups = GetDataTableByDB(mapExt.Tag1, mapExt.DBType, mapExt.FK_DBSrc);
        var myidx = 0;
        var oOfEn = "";
        for (var obj in ens[0]) {
            if (myidx == 2) {
                oOfEn = obj;
                break;
            }
            myidx++;
        }

        myidx = 0;
        var oOfGroup;
        for (var obj in groups[0]) {
            if (myidx == 0) {
                oOfGroup = obj;
                break;
            }
            myidx++;
        }
       
        groups.forEach(function (group) {
            var children = [];
            $.each(ens, function (i,item) {
                if (item[oOfEn] == group[oOfGroup]) {
                    children.push({
                        No: item.No,
                        Name: item.Name,
                        selected: IsHaveSelect(item.No, selects)
                    })
                }
            });

            data.push({
                Name: group.Name,
                No: group[oOfGroup],
                children: children,
                disabled: children.length==0?true:false
            })
        })
    } else {
        if(data.length==0)
            $.each(ens, function (i, item) {
                data.push({
                    No: item.No,
                    Name: item.Name,
                    selected: IsHaveSelect(item.No, selects)
                })
            })
      
    }
    data = data == null ? [] : data;
    $("#TB_" + mapAttr.KeyOfEn).hide();
    $("#TB_" + mapAttr.KeyOfEn).after("<div id='mapExt_" + mapAttr.KeyOfEn + "' style='width:99%'></div>")
    layui.use('xmSelect', function () {
        var xmSelect = layui.xmSelect;
        xmSelect.render({
            el: "#mapExt_" + mapAttr.KeyOfEn,
            prop: {
                name: 'Name',
                value: 'No',
            },
            paging: data.length > 15 ? true : false,
            data: data,
            autoRow:true,
            radio: selectType==1 ? false : true,
            clickClose: selectType == 1 ? false : true,
            click: function () {
                alert("sdfdf");
            },
            on: function (data) {
                var arr = data.arr;
                var vals = [];
                var valTexts = [];
                var elID = data.el.replace("#mapExt", "TB");
                if (arr.length == 0) {
                    $("#" + elID).val("");
                } else {
                    $.each(arr, function (i, obj) {
                        vals[i] = obj.No;
                        valTexts[i] = obj.Name;
                    })

                    $("#" + elID).val(valTexts.join(","));
                }
               
                SaveFrmEleDBs(arr, elID.replace("TB_", ""), mapExt);
               //填充其他控件
                FullIt(vals.join(","), mapExt.MyPK, elID);
                //确定后执行的方法
                //执行JS
                var backFunc = mapExt.Tag5;
                if (backFunc != null && backFunc != "" && backFunc != undefined)
                    DBAccess.RunFunctionReturnStr(DealSQL(backFunc, vals.join(",")));
            }
        })
    });
}
/**
 * 通用的POP弹出框
 * @param {any} poptype 树干叶子，树干，表格查询，自定义URL的弹出
 * @param {any} mapAttr 字段属性
 * @param {any} mapExt  扩展属性
 * @param {any} frmData 表单数据
 */
function CommPopDialog(poptype, mapAttr, mapExt, pkval, frmData, baseUrl, mapExts) {
    if (pkval == null || pkval == undefined) {
        pkval = GetQueryString("WorkID");
        if (pkval == null || pkval == undefined)
            pkval = GetQueryString("OID");
    }
    var target = $("#TB_" + mapAttr.KeyOfEn);
    target.hide();
    var container = $("<div class='mtags-container'style='width:99%'></div>");
    var mtagsId = mapExt.AttrOfOper + "_mtags";
    container.attr("id", mtagsId);
    target.after(container);
    $("#" + mtagsId).mtags({
        "fit": true,
        "FK_MapData": mapExt.FK_MapData,
        "KeyOfEn": mapExt.AttrOfOper,
        "RefPKVal": pkval,
        "onUnselect": function (record) {
            Delete_FrmEleDB(mapExt.AttrOfOper, pkval, record.No);

        }
    });

    $("#" + mtagsId).mtags("loadData", GetInitJsonData(mapExt, pkval,''));
    target.val($("#" + mtagsId).mtags("getText"));

    $("#" + mtagsId).on('dblclick', function () {
        debugger;
        var url = "";
        switch (poptype) {
            case "PopBranchesAndLeaf": //树干叶子模式.
                url = baseUrl + "Pop/BranchesAndLeaf.htm?MyPK=" + mapExt.MyPK + "&oid=" + pkval + "&m=" + Math.random();
                break;
            case "PopBranches": //树干简单模式.
                url = baseUrl + "Pop/Branches.htm?MyPK=" + mapExt.MyPK + "&oid=" + pkval + "&m=" + Math.random();
                break;
            case "PopTableSearch": //表格查询.
                url = baseUrl + "Pop/TableSearch.htm?MyPK=" + mapExt.MyPK + "&oid=" + pkval + "&m=" + Math.random();
                break;
            case "PopSelfUrl":
                url = mapExt.Tag;
                if (url.indexOf('?') == -1)
                    url = url + "?PKVal=" + pkval + "&UserNo=" + webUser.No;
                break;
            default: break;
        }
        var dlgWidth = mapExt.W;
        var dlgHeight = mapExt.H;
        if (dlgWidth > window.innerWidth || dlgWidth < window.innerWidth / 2)
            dlgWidth = window.innerWidth / 2;
        if (dlgHeight > window.innerHeight || dlgHeight < window.innerHeight / 2)
            dlgHeight = 50;
        else
            dlgHeight = dlgHeight / window.innerHeight *100;

        OpenLayuiDialog(url, mapExt.Title, dlgWidth, dlgHeight, "auto", false, true, true,function () {
            //获取选择的值，存储展示
            var selectType = mapExt.GetPara("SelectType");
            var iframe = $(window.frames["dlg"]).find("iframe");
            if (iframe.length > 0) {
                debugger
                var selectedRows = iframe[0].contentWindow.selectedRows;
                if (selectedRows == undefined || selectedRows.length==0)
                    selectedRows =iframe[0].contentWindow.GetCheckNodes()
                if ($.isArray(selectedRows)) {
                    //保存selectedRows的信息
                    SaveFrmEleDBs(selectedRows, mapExt.AttrOfOper, mapExt);
                    var mtags = $("#" + mtagsId);
                    mtags.mtags("loadData", selectedRows);
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

                    var attrMyPK = mapExt.FK_MapData + "_" + mapExt.AttrOfOper;
                    if (mapExts[attrMyPK] == undefined || mapExts[attrMyPK].length == 0) {
                        //执行JS
                        var backFunc = mapExt.Tag5;
                        if (backFunc != null && backFunc != "" && backFunc != undefined)
                            DBAccess.RunFunctionReturnStr(DealSQL(backFunc, No));
                    }
                    else {
                        $.each(mapExts[attrMyPK], function (idx, mapExt1) {
                            var mapExtN = new Entity("BP.Sys.MapExt", mapExt1);
                            mapExtN.MyPK = mapExt1.MyPK;
                            //填充其他控件
                            switch (mapExtN.ExtType) {
                                case "FullData": //填充其他控件
                                    DDLFullCtrl(No.substring(0, No.length - 1), mapExtN.AttrOfOper, mapExtN.MyPK);
                                        
                                    break;
                            }
                        });
                        //执行JS
                        var backFunc = mapExt.Tag5;
                        if (backFunc != null && backFunc != "" && backFunc != undefined)
                            DBAccess.RunFunctionReturnStr(DealSQL(backFunc, No));
                    }
                }
            }
        })
    })
}

function xmSelectTree(eleID, mapExt, frmEleDBs, type, selectType) {

    //获取根目录
    //跟节点编号.
    var rootNo = mapExt.Doc;
    if (rootNo == "@WebUser.FK_Dept") {
        rootNo = webUser.FK_Dept;
    }
    if (rootNo == "@WebUser.OrgNo") {
        rootNo = webUser.OrgNo;
    }
    if (rootNo == null || rootNo == undefined) {
        rootNo = "0"
    }

    var treeUrl = mapExt.Tag2.replace(/~/g, "'");
    var treeTag1 = mapExt.Tag1;
    if (treeUrl == "") {
        alert('配置错误:查询数据源，初始化树的数据源不能为空。');
        return;
    }
   
    var json = GetDataTableByDB(treeUrl, mapExt.DBType, mapExt.FK_DBSrc, rootNo);
    var data = TreeJson(json, rootNo);
    layui.use('xmSelect', function () {
        var xmSelect = layui.xmSelect;
        var tree = xmSelect.render({
            el: "#" + eleID,
            prop: {
                name: 'Name',
                value: 'No',
            },
            autoRow: true,
            filterable: true,
            remoteSearch: true,
            radio: selectType == 1 ? false : true,
            clickClose: selectType == 1 ? false : true,
            remoteMethod: function (val, cb, show) {
                //这里如果val为空, 则不触发搜索
                if (!val) {
                    return cb(data);
                }
                setTimeout(function () {
                    var url = mapExt.Tag1.replace(/~/g, "'") + "";
                    var json = GetDataTableByDB(url, mapExt.DBType, mapExt.FK_DBSrc, val);
                    //var data = findChildren(json, item.value);
                    cb(json);

                }, 500)
            },
            tree: {
                show: true,
                showFolderIcon: true,
                showLine: true,
                lazy: true,
                strict: false,
                clickCheck: false,
                load: function (item, cb) {
                    setTimeout(function () {
                        var url = mapExt.Tag2.replace(/~/g, "'") + "";
                        var json = GetDataTableByDB(url, mapExt.DBType, mapExt.FK_DBSrc, item.No);
                        var data = findChildren(json, item.No);
                        cb(data);

                    }, 500)
                }
            },
            on: function (data) {
                var arr = data.arr;
                var vals = [];
                var valTexts = [];
                var elID = data.el.replace("#mapExt", "TB");
                if (arr.length == 0) {
                    $("#" + elID).val("");
                } else {
                    $.each(arr, function (i, obj) {
                        vals[i] = obj.No;
                        valTexts[i] = obj.Name;
                    })

                    $("#" + elID).val(valTexts.join(","));
                }

                SaveFrmEleDBs(arr, elID.replace("TB_", ""), mapExt);
                //填充其他控件
                FullIt(vals.join(","), mapExt.MyPK, elID);
                //确定后执行的方法
                //执行JS
                var backFunc = mapExt.Tag5;
                if (backFunc != null && backFunc != "" && backFunc != undefined)
                    DBAccess.RunFunctionReturnStr(DealSQL(backFunc, vals.join(",")));
            },
            height: 'auto',
            data() {
                return data;
            }
        });
        if (frmEleDBs && frmEleDBs.length > 0) {
            var vals = [];
            $.each(frmEleDBs, function (i, item) {
                vals.push({
                    Name: item.Tag2,
                    No: item.Tag1
                })
            })
            tree.setValue(vals);
        }
    });
}

/**
 * 保存EleDB
 * @param {any} rows
 */
function SaveFrmEleDBs(rows,keyOfEn,mapExt) {
  debugger
    var pkVal = pageData.WorkID;
    pkVal = pkVal == undefined || pkVal == 0 ? pageData.OID : pkVal;
    //删除
    var ens = new Entities("BP.Sys.FrmEleDBs");
    ens.Delete("FK_MapData", mapExt.FK_MapData, "EleID", keyOfEn, "RefPKVal", pkVal);
    //保存
    $.each(rows, function (i, row) {
        var frmEleDB = new Entity("BP.Sys.FrmEleDB");
        frmEleDB.MyPK = keyOfEn + "_" + pkVal + "_" + row.No;
        frmEleDB.FK_MapData = mapExt.FK_MapData;
        frmEleDB.EleID = keyOfEn;
        frmEleDB.RefPKVal = pkVal;
        frmEleDB.Tag1 = row.No;
        frmEleDB.Tag2 = row.Name;
        frmEleDB.Insert();
    })
}
/**
 * 删除保存的数据
 * @param {any} keyOfEn
 * @param {any} oid
 * @param {any} No
 */
function Delete_FrmEleDB(keyOfEn, oid, No) {
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = keyOfEn + "_" + oid + "_" + No;
    frmEleDB.Delete();
}

function GetInitJsonData(mapExt, refPKVal, val) {
    var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
    frmEleDBs.Retrieve("FK_MapData", mapExt.FK_MapData, "EleID", mapExt.AttrOfOper, "RefPKVal", refPKVal);
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

//树形结构
function TreeJson(jsonArray, parentNo) {
 
    var jsonTree = [];
    if (jsonArray.length > 0) {
        $.each(jsonArray, function (i, o) {
            if (o.ParentNo == parentNo && parentNo == "0") {
                jsonTree.push({
                    "No": o.No,
                    "Name": o.Name,
                    "children": findChildren(jsonArray, o.No)
                });
                return false;
            }
            if (o.No == parentNo) {
                jsonTree.push({
                    "No": o.No,
                    "Name": o.Name,
                    "children": findChildren(jsonArray, o.No)
                });
                return false;
            }
        })
    }
    return jsonTree;
}
function findChildren(jsonArray, parentNo) {
    var children = [];
    $.each(jsonArray, function (i, child) {
        if (parentNo == child.ParentNo)
            children.push({
                "No": child.No,
                "Name": child.Name,
                "children": []
            });
    });
   
    return children;
}
