/**
 * xm-select通用的单选，多选
 * @param {any} popType 弹出框类型
 * @param {any} mapAttr 字段属性
 * @param {any} mapExt 扩展属性
 * @param {any} frmData 表单数据
 */
function CommPop(popType, mapAttr, mapExt, frmData, mapExts, targetID) {

    targetID = targetID == null || targetID == undefined ? mapAttr.KeyOfEn : targetID;
    if (mapAttr.UIIsEnable == 0 || isReadonly == true) {
        //只显示
        return;
    }
    //单选还是多选
    var selectType = mapExt.GetPara("SelectType");
    selectType = selectType == null || selectType == undefined || selectType == "" ? 1 : selectType;
    var pkVal = GetQueryString("WorkID");
    pkVal = pkVal == null || pkVal == undefined || pkVal == 0 ? GetQueryString("OID") : pkVal;

    //选中的值
    var selects = new Entities("BP.Sys.FrmEleDBs");
    selects.Retrieve("FK_MapData", mapExt.FK_MapData, "EleID", mapExt.AttrOfOper, "RefPKVal", pkVal);
    var data = [];
    //获取实体信息
    var ens = [];
    if (popType == "PopBranches") {
        $("#TB_" + targetID).hide();
        $("#TB_" + targetID).after("<div id='mapExt_" + targetID + "' style='width:99%'></div>")
        xmSelectTree("mapExt_" + targetID, mapExt, selects, popType, selectType);
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
        ens = GetDataTableByDB(mapExt.Tag2, mapExt.DBType, mapExt.FK_DBSrc, null,mapExt,"Tag2");
    }
       
    //如果是分组的时候处理
    if (popType == "PopGroupList") {
        //获取分组信息
        var groups = GetDataTableByDB(mapExt.Tag1, mapExt.DBType, mapExt.FK_DBSrc,null,mapExt,"Tag1");
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
        if(data.length!=0)
            $.each(ens, function (i, item) {
                data.push({
                    No: item.No,
                    Name: item.Name,
                    selected: IsHaveSelect(item.No, selects)
                })
            })
      
    }
    data = data == null ? [] : data;
    $("#TB_" + targetID).hide();
    $("#TB_" + targetID).after("<div id='mapExt_" + targetID + "' style='width:99%'></div>")
    layui.use('xmSelect', function () {
        var xmSelect = layui.xmSelect;
        xmSelect.render({
            el: "#mapExt_" + targetID,
            prop: {
                name: 'Name',
                value: 'No',
            },
            paging: data.length > 15 ? true : false,
            data: data,
            autoRow:true,
            radio: selectType==1 ? false : true,
            clickClose: selectType == 1 ? false : true,
            toolbar: { show: selectType == 1 ? true : false },
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
function CommPopDialog(poptype, mapAttr, mapExt, pkval, frmData, baseUrl, mapExts, targetID) {
    targetID = targetID == null || targetID == undefined ? mapAttr.KeyOfEn : targetID;
    if (pkval == null || pkval == undefined) {
        pkval = GetQueryString("WorkID");
        if (pkval == null || pkval == undefined)
            pkval = GetQueryString("OID");
    }
    var target = $("#TB_" + targetID);
    target.hide();
    var container = $("<div class='mtags-container'style='width:99%'></div>");
    var mtagsId = targetID + "_mtags";
    container.attr("id", mtagsId);
    target.after(container);
    $("#" + mtagsId).mtags({
        "fit": true,
        "FK_MapData": mapExt.FK_MapData,
        "KeyOfEn": mapExt.AttrOfOper,
        "RefPKVal": pkval,
        "elemId": mtagsId,
        "onUnselect": function (target,record) {
            Delete_FrmEleDB(mapExt.AttrOfOper, pkval, record.No);
            var elemId = mapExt.AttrOfOper + "_mtags";
            $("#TB_" + mapExt.AttrOfOper).val($("#" + elemId).mtags("getText"));
        }
    });

    $("#" + mtagsId).mtags("loadData", GetInitJsonData(mapExt, pkval,''));
    target.val($("#" + mtagsId).mtags("getText"));

    var OpenPopType = GetPara(mapExt.AtPara, "OpenPopType");
    if (OpenPopType && OpenPopType == 1) {
        var btnLab = GetPara(mapExt.AtPara, "BtnLab");
        btnLab = btnLab == null || btnLab == undefined || btnLab == "" ? "查找" : btnLab;
        var element = $("<div class='layui-col-xs1'><button type=button class='layui-btn layui-btn-primary layui-btn-sm'>" + btnLab + "</button></div>");
        container.append(element);
        $(container.children()[0]).removeClass("layui-col-xs12").addClass("layui-col-xs10");
        element.on("click", function () {
            OpenPopFunction(mapExt, mapExts, mtagsId, target, targetID, pkval, poptype, frmData, baseUrl);
        });
        return;
    }
    $("#" + mtagsId).on('dblclick', function () {
        OpenPopFunction(mapExt, mapExts, mtagsId, target, targetID, pkval, poptype, frmData, baseUrl);
    })
}
function OpenPopFunction(mapExt, mapExts, mtagsId, target, targetID, pkval, poptype, frmData, baseUrl) {
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
            url = DealExp(mapExt.Tag);
            if (url.indexOf('?') == -1)
                url = url + "?PKVal=" + pkval + "&UserNo=" + webUser.No;
            break;
        default: break;
    }
    var dlgWidth = mapExt.W;
    var dlgHeight = mapExt.H;
    if (dlgWidth > window.innerWidth || dlgWidth < window.innerWidth / 2)
        dlgWidth = window.innerWidth * 4 / 5;
    if (dlgHeight > window.innerHeight || dlgHeight < window.innerHeight / 2)
        dlgHeight = 50;
    else
        dlgHeight = dlgHeight / window.innerHeight * 100;
    if (window.parent && window.parent.OpenLayuiDialog)
        window.OpenLayuiDialog(url, mapExt.Title, dlgWidth, dlgHeight, "auto", false, true, true, function () {
            CloseLayuiDialogFunc(mapExt, mapExts, mtagsId, target, targetID, pkval);
        })
    else
        OpenLayuiDialog(url, mapExt.Title, dlgWidth, dlgHeight, "auto", false, true, true, function () {
            CloseLayuiDialogFunc(mapExt, mapExts, mtagsId, target, targetID, pkval);
        })
}
function CloseLayuiDialogFunc(mapExt, mapExts, mtagsId, target, targetID, pkval) {
    //获取选择的值，存储展示
    var selectType = mapExt.GetPara("SelectType");
    var iframe = $(window.frames["dlg"]).find("iframe");
    if (iframe.length > 0) {
       // debugger
        var selectedRows = iframe[0].contentWindow.selectedRows;
        if (selectedRows == undefined || selectedRows.length == 0) {
            if (typeof iframe[0].contentWindow.GetCheckNodes != 'undefined' && typeof iframe[0].contentWindow.GetCheckNodes == "function")
                selectedRows = iframe[0].contentWindow.GetCheckNodes();
            if (typeof iframe[0].contentWindow.Btn_OK != 'undefined' && typeof iframe[0].contentWindow.Btn_OK == "function")
                selectedRows = iframe[0].contentWindow.Btn_OK();
        }
        if ($.isArray(selectedRows)) {
            //保存selectedRows的信息
            SaveFrmEleDBs(selectedRows, mapExt.AttrOfOper, mapExt, pkval);
            var mtags = $("#" + mtagsId);
            mtags.mtags("loadData", selectedRows);
            target.val(mtags.mtags("getText"));
            // 单选复制当前表单
            //if (selectType == "0" && selectedRows.length == 1) {
            //    FullIt(selectedRows[0].No, mapExt.MyPK, targetId);
            //}
            var No = "";
            if (selectedRows != null && $.isArray(selectedRows))
                $.each(selectedRows, function (i, selectedRow) {
                    if (i == 0)
                        No += selectedRow.No;
                    else
                        No += "," + selectedRow.No;
                });
            FullIt(No, mapExt.MyPK, "TB_" + targetID);
            var attrMyPK = mapExt.FK_MapData + "_" + mapExt.AttrOfOper;
            if (mapExts[attrMyPK] == undefined || mapExts[attrMyPK].length == 0) {
                //执行JS
                var backFunc = mapExt.Tag5;
                if (backFunc != null && backFunc != "" && backFunc != undefined)
                    DBAccess.RunFunctionReturnStr(DealSQL(backFunc, No));
            }
            //else {
            //    $.each(mapExts[attrMyPK], function (idx, mapExt1) {
            //        var mapExtN = new Entity("BP.Sys.MapExt", mapExt1);
            //        mapExtN.MyPK = mapExt1.MyPK;
            //        //填充其他控件
            //        switch (mapExtN.ExtType) {
            //            case "FullData": //填充其他控件
            //                DDLFullCtrl(No.substring(0, No.length - 1), mapExtN.AttrOfOper, mapExtN.MyPK);

            //                break;
            //        }
            //    });
            //    //执行JS
            //    var backFunc = mapExt.Tag5;
            //    if (backFunc != null && backFunc != "" && backFunc != undefined)
            //        DBAccess.RunFunctionReturnStr(DealSQL(backFunc, No));
            //}
        }
    }
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

    var json = GetDataTableByDB(treeUrl, mapExt.DBType, mapExt.FK_DBSrc, rootNo,mapExt,'Tag2');
    var data = TreeJson(json, rootNo, frmEleDBs);
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
                    var json = GetDataTableByDB(url, mapExt.DBType, mapExt.FK_DBSrc, val,mapExt,"Tag1");
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
                        var json = GetDataTableByDB(url, mapExt.DBType, mapExt.FK_DBSrc, item.No,mapExt,"Tag2");
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
function TreeJson(jsonArray, parentNo, frmEleDBs) {
 
    var jsonTree = [];
    if (jsonArray.length > 0) {
        $.each(jsonArray, function (i, o) {
            if (o.ParentNo == parentNo && parentNo == "0") {
                jsonTree.push({
                    "No": o.No,
                    "Name": o.Name,
                    "selected": IsSelect(frmEleDBs,o.No),
                    "children": findChildren(jsonArray, o.No, frmEleDBs)
                });
                return false;
            }
            if (o.No == parentNo) {
                jsonTree.push({
                    "No": o.No,
                    "Name": o.Name,
                    "selected": IsSelect(frmEleDBs, o.No),
                    "children": findChildren(jsonArray, o.No, frmEleDBs)
                });
                return false;
            }
        })
    }
    return jsonTree;
}
function findChildren(jsonArray, parentNo, frmEleDBs) {
    var children = [];
    $.each(jsonArray, function (i, child) {
        if (parentNo == child.ParentNo)
            children.push({
                "No": child.No,
                "Name": child.Name,
                "selected": IsSelect(frmEleDBs, child.No),
                "children": []
            });
    });
   
    return children;
}
function IsSelect(frmEleDBs,no) {
    if (frmEleDBs == null || frmEleDBs.length == 0)
        return false;
    var dbs = $.grep(frmEleDBs, function (item) {
        return item.Tag1 == no;
    });
    if (dbs.length > 0)
        return true;
    return false;
}
