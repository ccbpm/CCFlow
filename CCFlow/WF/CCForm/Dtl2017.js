
//公共方法执行保存,删除，新增
function AjaxServiceGener(param, callback, scope) {
    // 1=自由表单模式 2=傻瓜表单模式 不需要自动保存
    if (param.DoType != "Dtl_DeleteRow" && (window.EditModel == 1 || window.EditModel == 2)) {
        return;
    }
    //DoType换为DoMethod
    var method = param["DoType"];
    delete param["DoType"];
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddJson(param);
    var data = handler.DoMethodReturnString(method);
    if (data.indexOf("err@") != -1) {
        alert('保存数据出错' + data);
        return;
    }

    if (saveAll) {
        alerdaySaveCount++;
    } else {
        callback(data, scope);
    }

    if (!saveAll || (saveAll && alerdaySaveCount == needSaveCount)) {//保存所有数据
        $('#Msg').hide();
        $('#ContentDtlDiv').show();
        alerdaySaveCount = 0;
        needSaveCount = 0;
        saveAll = false;

        InitPage();
        BindMapExt();
    }
}

//保存数据
function saveRow(index) {

    pageData.saveRowCurrent.count = 1;
    pageData.saveRowCurrent.overCount = 0;
    //获取序号的值
    pageData.saveRowCurrent.rowIndex = index;
    //让界面的值=DAT 的值
    setTrDataByData(index);
    var trData = $($('table tbody tr')[index]).data().data;
    var rowData = trData;
    rowData.DoType = "Dtl_SaveRow";
    rowData.FK_MapDtl = GetQueryString("EnsName");
    rowData.RefPKVal = GetQueryString("RefPKVal");
    rowData.FK_Node = GetQueryString("FK_Node");
    rowData.FK_MapData = GetQueryString("FK_MapData");
    rowData.FID = GetQueryString("FID");
    rowData.WorkID = GetQueryString("WorkID");
    rowData.PWorkID = GetQueryString("PWorkID");
    var dbDataRow = $.grep(pageData.DBDtl, function (val) { return val.OID == rowData.OID; });
    if (rowData.OID != 0 && dbDataRow.length == 1 && JSON.stringify(dbDataRow[0].RowData) == JSON.stringify(trData)) {
        //值未改变，无需存盘
        return;
    }

    $('#Msg').show();
    $('#ContentDtlDiv').hide();
    AjaxServiceGener(rowData, SaveRowAfter, this);

}

//保存后执行
function SaveRowAfter(data) {
    pageData.saveRowCurrent.overCount = pageData.saveRowCurrent.overCount + 1;
    if (data.indexOf('err@') > -1) {
        alert('保存失败！' + data);
        return;
    }

    var returnRowData = JSON.parse(data);
    //更新表格的DATA
    var updateTr = $($('table tbody tr')[pageData.saveRowCurrent.rowIndex]);
    //更新 pageData.DBDTL
    var updateData = $.grep(pageData.DBDtl, function (val) { return val.OID == returnRowData.OID });
    if (updateData.length == 0) { //新增的  直接PUSH
        var rowData = {};
        pageData.DBDtl.push({ OID: rowData.OID, RowData: returnRowData });
        updateTr.data().data = returnRowData;
        //新增数据  将移除改成删除
        updateTr.find('.delRow').attr('src', '../Img/Btn/delete.png');

        updateTr.find('.Ath').attr('href', "javascript:alert('ss')");


    } else {
        for (var obj in updateData[0].RowData) {//更新值
            updateData[0].RowData[obj] = returnRowData[obj];
        }
        updateTr.data().data = updateData[0].RowData;
    }

    //保存后刷新行数据
    setTdDataByTrData(pageData.saveRowCurrent.rowIndex);

    pageData.saveRowCurrent.rowIndex = -1;

    return;
}
//插入行.
function insertRow(EditModel) {

    //判断插入行的模式 =0行模式. 1=自由表单卡片模式 2=傻瓜表单卡片模式.
    if (EditModel != 0 && window.parent) {
        var ensName = GetQueryString("EnsName");
        var refPKVal = GetQueryString("RefPKVal");
        var FK_MapData = GetQueryString("FK_MapData");
        var FID = GetQueryString("FID");
        var FK_Node = GetQueryString("FK_Node");
        window.parent.DtlFrm(ensName, refPKVal, '0', EditModel, InitPage, FK_MapData, FK_Node, FID, false, sys_MapDtl.H);
        return;
    }

    if ($('#dtlDiv div table tbody tr').length == 1) {
        var firstTr = $('#dtlDiv div table tbody tr')[0];
        if ($(firstTr).data().data == undefined) {
            $('#dtlDiv div table tbody').html('')//无数据时，新增的时候先删除 无记录行
            statisticsFlag = false; // 清空无记录行(统计行) 则需要重新生成统计行
        }
    }
    var insertTr = $('<tr></tr>');
    var threadTr = $('#dtlDiv div table thead tr');
    var newRowIndex = $('#dtlDiv div table tbody tr').length;
    if (statisticsFlag && newRowIndex > 0) {	// 统计行不计
        newRowIndex--;
    }

    var threadTh = getUseHeadTh();
    $.each(threadTh, function (k, threadThObj) {

        if ($(threadThObj).data().colname != undefined) {
            var o = $(threadThObj);
            var tmplate = figure_MapAttr_Template(o.data());
            if (o.data().UIIsInput == 1) {
                tmplate.attr('class', "mustInput");
            }
            var mapAttr = o.data();
            //枚举字段
            //枚举复选框
            if (mapAttr.LGType == 1 && ((mapAttr.MyDataType == "1" && mapAttr.UIContralType == "2")
                //枚举单选按钮
                || (mapAttr.MyDataType == "2" && mapAttr.UIContralType == "3"))) {
                $.each(tmplate, function (idx, obj) {
                    if (obj.nodeName != "LABEL") {
                        $(obj).attr('id', $(obj).attr('id') + '_' + newRowIndex);
                        $(obj).attr('name', $(obj).attr('name') + '_' + newRowIndex);
                        $(obj).data({ KeyOfEn: o.data().KeyOfEn });
                    }

                });
                var td = $('<td></td>');
                if (o.data().UIVisible == 0)
                    td = $('<td style="display:none"></td>');
            } else {
                tmplate.data({ KeyOfEn: o.data().KeyOfEn });
                tmplate.attr('id', tmplate.attr('name') + "_" + newRowIndex);
                if (tmplate.attr('name') != undefined && tmplate.attr('name').indexOf('TB_') == 0) {
                    $(tmplate).bind('blur', function (obj) {
                        $(obj.target).parent().parent().data().data[$(obj.target).data().KeyOfEn] =
                            $(obj.target).val();
                    });
                } else if (tmplate.attr('name') != undefined && tmplate.attr('name').indexOf('DDL_') == 0) {
                    $(tmplate).bind('change', function (obj) {
                        $(obj.target).parent().parent().data().data[$(obj.target).data().KeyOfEn] =
                            $(obj.target).val();
                    });


                    //对于只读的下拉框做如下处理  为只读的下拉框赋值
                    if (o.data().UIIsEnable == "0") {
                        var readOnlyDdlText = '无';
                        var readOnlyDdlVal = '';
                        if (workNodeData.Blank[0][o.data().KeyOfEn + "TEXT"] != undefined) {
                            readOnlyDdlText = workNodeData.Blank[0][o.data().KeyOfEn + "TEXT"];
                        }
                        if (workNodeData.Blank[0][o.data().KeyOfEn] != undefined) {
                            readOnlyDdlVal = workNodeData.Blank[0][o.data().KeyOfEn];
                        }
                        var option = $('<option select="selected" value="' + readOnlyDdlVal + '">' + readOnlyDdlText + '</option>')
                        $(tmplate).children().remove();
                        $(tmplate).append(option);
                    }
                } else if (tmplate.attr('name') == undefined && tmplate.find('[name^=CB_]').length == 1) {
                    $(tmplate).find('[name^=CB_]').bind('change', function (obj) {
                        $(obj.target).parent().parent().parent().data().data[$(obj.target).data().KeyOfEn] =
                            $(tmplate).find('[name^=CB_]')[0].checked == true ? 1 : 0;
                    });
                }
            }
            //枚举值单选按钮

            var radioEles = $.grep(tmplate, function (obj) {
                return $(obj).attr('name') == 'RB_' + o.data().KeyOfEn + '_' + newRowIndex;
            })


            //处理复选框  设置表格中数据的值
            var textVal = workNodeData.Blank[0][o.data().KeyOfEn];
          
            var ckEle = tmplate.find('[name=CB_' + o.data().KeyOfEn + ']');
            if (ckEle.length == 1) {
                var ckId = 'CB_' + o.data().KeyOfEn + '_' + newRowIndex;
                ckEle.attr('id', ckId);
                tmplate.find('label').attr('for', ckId);
                if (textVal == "1") {
                    ckEle[0].checked = true;
                } else {
                    ckEle[0].checked = false;
                }
            } else if (ckEle.length > 1) {
                textVal = "," + textVal + ",";
                $.each(ckEle, function () {
                    if (textVal.indexOf("," + $(this).val() + ",") != -1)
                        $(this).attr("checked", true);
                    else
                        $(this).attr("checked", false);
                });
            } else if (radioEles.length > 1) {
                $.each(radioEles, function () {
                    if (textVal == $(this).val())
                        $(this).attr("checked", true);
                    else
                        $(this).attr("checked", false);
                });

            }
            else {
                tmplate.val(textVal);
            }

            var td = $('<td></td>');
            if (o.data().UIVisible == 0)
                td = $('<td style="display:none"></td>');
            td.append(tmplate);
            insertTr.append(td);

        } else if ($(threadThObj).data().coltype == 'SN') {

            insertTr.append($('<td>' + (newRowIndex + 1) + '</td>'));

        } else {

        }
    });



    var trData = { data: {} };
    $.each(workNodeData.Sys_MapAttr, function (k, mapAttr) {
        var defVal = undefined;
        //设置新增时的默认值
        if (workNodeData.Blank[0][mapAttr.KeyOfEn] == undefined) {
            defVal = mapAttr.DefVal;
        } else {
            defVal = workNodeData.Blank[0][mapAttr.KeyOfEn];
        }

        if (mapAttr.DefValType == 0 && mapAttr.DefVal == "10002" && (defVal == "10002" || defVal == "10002.0000"))
            defVal = "";
        trData.data[mapAttr.KeyOfEn] = defVal;
    })

    trData.data["OID"] = 0;
    insertTr.data(trData);
    insertTr.append($('<td> <a href="#" onclick="deleteRow(this)" >移除</a></td>'));



    // 生成统计行
    if (window.columnExp && !statisticsFlag) {
        addStatisticsRow();
    }
    if (statisticsFlag) {
        // 将数据行插入到统计行前
        var statisticsRow = $("#dtlDiv div table tbody").find("tr").last();
        insertTr.insertBefore(statisticsRow);
        //
        bindStatistics();
    } else {
        $('#dtlDiv div table tbody').append(insertTr);
    }


    var formExt = $("#formExt").val();
    var extObj = null;
    try {
        if (formExt && formExt != "") {
            extObj = JSON.parse(formExt);
        }
        if (extObj) {
            parentStatistics(extObj);
        }

    } catch (e) {
    }


    setTdDataByTrData(newRowIndex);

    insertTr.bind('mouseleave', mouseLeaveTrFun);
    setIframeHeight();

    AfterBindEn_DealMapExt(insertTr, newRowIndex);
}

//行最后的保存按钮
function saveTrRow(obj) {

    var thisRowIndex = parseInt($($(obj).parent().parent().children()[0]).text()) - 1;
    saveRow(thisRowIndex);
    saveAll = false;
    $('#Msg').hide();
}


var saveAll = false;
var needSaveCount = 0;
var alerdaySaveCount = 0;
var isChange = true;
function SetChange(val) {
    isChange = val;
}
var isDelRowAlert = false;

//从表全部数据保存 还有问题 需要调整
function SaveAll() {
    // 1=自由表单模式 2=傻瓜表单模式 不需要自动保存
    if (window.EditModel == 1 || window.EditModel == 2) {
        return;
    }
    /**
    * 页面失去焦点之后, 无法重新加载?
    * 从表生成后如果有统计, 并不自动计算, 所以在初始化完成后触发onblur事件(页面自动计算设计之初就是onblur事件)
    */
    //console.log("已禁用SaveAll");
    //return;

    if (isDelRowAlert == true) {
        isDelRowAlert = false;
        return;
    }

    if (GetQueryString("IsReadonly") == 1)
        return;

    if (isChange == false) {
        return;
    }

    //判断是否有必填项
    var formCheckResult = true;

    if (checkBlanks() == false) {
        formCheckResult = false;
    }

    if (checkReg() == false) {
        formCheckResult = false;
    }

    if (formCheckResult == false) {
        //alert("请检查表单必填项和正则表达式");
        return;
    }


    //循环表格
    var trs = $('.table.wupop tbody tr');
    for (var i = 0; i < trs.length; i++) {
        var obj = trs[i];
        var sn = $($(obj).children()[0]).text();
        if (sn == null || sn == "" || isNaN(sn)) {
            continue;
        }

        needSaveCount = trs.length;
        saveAll = true;
        var index = i;

        //让界面的值=DAT 的值
        setTrDataByData(index);

        var trData = $($('table tbody tr')[index]).data().data;
        var rowData = trData;
        if ($($('table tbody tr')[index]).data().customRowType == "statistics") {
            needSaveCount--;
            continue;
        }
        rowData.DoType = "Dtl_SaveRow";
        rowData.FK_MapDtl = GetQueryString("EnsName");
        rowData.RefPKVal = GetQueryString("RefPKVal");
        rowData.FK_Node = GetQueryString("FK_Node");
        rowData.FK_MapData = GetQueryString("FK_MapData");
        rowData.FID = GetQueryString("FID");
        rowData.WorkID = GetQueryString("WorkID");
        rowData.PWorkID = GetQueryString("PWorkID");
        rowData.RowIndex = index;//pop用到，传到后台
        var dbDataRow = $.grep(pageData.DBDtl, function (val) { return val.OID == rowData.OID; });
        if (rowData.OID != 0 && dbDataRow.length == 1 && JSON.stringify(dbDataRow[0].RowData) == JSON.stringify(trData)) {
            //值未改变，无需存盘
            alerdaySaveCount++;
            continue;
        }

        if (rowData.OID != 0) {
            AjaxServiceGener(rowData, SaveRowAfter, this);
            continue;
        }

        //开始执行存盘.
        var isEdit = false;
        for (var pro in workNodeData.Blank[0]) {
            if (rowData[pro] != undefined && rowData[pro] != workNodeData.Blank[0][pro])
                isEdit = true;
           
        }

        if (isEdit)//改变了，入库
            AjaxServiceGener(rowData, SaveRowAfter, this);
       
        else {
            //未录入值  无需存盘
            alerdaySaveCount++;
            continue;
        }
    };  //循环表格

    if (!saveAll || (saveAll && alerdaySaveCount == needSaveCount)) {//保存所有数据
        $('#Msg').hide();
        $('#ContentDtlDiv').show();
        alerdaySaveCount = 0;
        needSaveCount = 0;
        saveAll = false;

        InitPage();
        BindMapExt();
    }
}

function updateRow(OID, EditModel) {
    if (window.parent) {
        var ensName = GetQueryString("EnsName");
        var refPKVal = GetQueryString("RefPKVal");
        var FK_MapData = GetQueryString("FK_MapData");
        var fid = GetQueryString("FID");
        if (fid == "" || fid == undefined)
            fid = 0
        var FK_Node = GetQueryString("FK_Node");
        window.parent.DtlFrm(ensName, refPKVal, OID, EditModel, InitPage, FK_MapData, FK_Node, fid, true);
    }
}
function deleteRow(obj) {

    isDelRowAlert = true;
    var result = parent.window.confirm('确定要删除吗?');
    if (result == false)
        return;


    var rowData = $(obj).parent().parent().data().data;
    var rowCurrentIndex = parseInt($($(obj).parent().parent().children()[0]).text()) - 1;
    pageData.currentRowIndex = rowCurrentIndex;
    if (rowData.OID > 0) {
        rowData.DoType = "Dtl_DeleteRow";
        rowData.FK_MapDtl = GetQueryString("EnsName");
        //row.FK_MapData = "ND101";
        rowData.RefPKVal = GetQueryString("RefPKVal");
        currentRowIndex = -1;

        $('#Msg').show();
        AjaxServiceGener(rowData, deleteRowAfter, this);
        $('#Msg').hide();

    } else { //移除  新增的 尚未入库
        var tr = $(obj).parent().parent();
        updateTableSn(tr);
        tr.remove();
        triggerStatistics(); // 重新计算
        var formExt = $("#formExt").val();
        var extObj = null;
        try {
            if (formExt && formExt != "") {
                extObj = JSON.parse(formExt);
            }
            if (extObj) {
                parentStatistics(extObj);
            }
        } catch (e) {
        }

    }
}

function deleteRowAfter(data) {
    if (data.indexOf('err') > -1) {
        alert('删除失败！');
    } else {
        //
        //InitPage();//刷新页面
        //删除数据
        var tr = $('table tbody tr')[pageData.currentRowIndex];
        var delOid = $(tr).data().data.OID;
        var delRow = $.grep(pageData.DBDtl, function (val) { return val.OID == delOid });
        if (delRow.length == 1) {
            delRow.OID = -1;
            delRow.RowData = undefined;
        }


        updateTableSn(tr);
        tr = $('table tbody tr')[pageData.currentRowIndex];
        $(tr).remove();
        pageData.currentRowIndex = -1;
        triggerStatistics(); // 重新计算
        return;
    }
}

//更新表格序号
function updateTableSn(tr) {
    tr = $(tr).next();
    while (tr.length == 1) {
        var snTd = $(tr.children('td')[0]);
        var sn = snTd.text();
        if (!isNaN(parseInt(sn))) {
            snTd.text(parseInt(sn) - 1);
        }
        tr = $(tr).next();
    }
}



/**
 * 获得控件的值，不管是cb,tb,ddl 都可以获取到.
 * @param {any} ctrlID
 */

//获得控件的值.
function ReqDtlCtrlVal(ctrlID) {

    var ctrl = ReqDtlCtrl(ctrlID);
    if (ctrl == null || ctrl == undefined || ctrl.length == 0) {
        alert("列名错误:" + ctrlID);
        return "";
    }
    var val = ctrl.val();
    return val;
}

/**
 * 获得控件, 不需要加前缀, 不需要idx字段。
 * @param {控件ID,比如:XingMing } ctrlID
 */
//获得控件.
function ReqDtlCtrl(ctrlID) {

    var ctrl = $("#TB_" + ctrlID + "_" + curRowIndex);

    if (ctrl.length == 0)
        ctrl = $("#DDL_" + ctrlID + "_" + curRowIndex);
    else
        return ctrl;

    if (ctrl.length == 0)
        ctrl = $("#CB_" + ctrlID + "_" + curRowIndex);
    else
        return ctrl;

    return ctrl;
}

/**
 * 
 * @param {控件ID,比如:XingMing } ctrlID
 * @param {any} val
 */
//设置控件的值.
function SetDtlCtrlVal(ctrlID, val) {

    var ctrl = $("#TB_" + ctrlID + "_" + curRowIndex);
    if (ctrl.length != 0) {
        $("#TB_" + ctrlID + "_" + curRowIndex).val(val);
        return;
    }

    ctrl = $("#DDL_" + ctrlID + "_" + curRowIndex);
    if (ctrl.length != 0) {
        $("#DDL_" + ctrlID + "_" + curRowIndex).val(val);
        return;
    }

    ctrl = $("#CB_" + ctrlID + "_" + curRowIndex);
    if (ctrl.length == 0) {
        alert("执行方法： SetCtrlVal， 列名：" + ctrlID + " 不存在, val=" + val + ". 请F12检查是否正确.");
        return;
    }

    if (val >= 1 || val == true)
        ctrl.prop('checked', true);
    else
        ctrl.prop('checked', false);
    return;
}
//計算日期間隔
function CalculateRDT(StarRDT, EndRDT, RDTRadio) {

    var res = "";
    var demoRDT;
    demoRDT = StarRDT.split("-");
    StarRDT = new Date(demoRDT[0] + '-' + demoRDT[1] + '-' + demoRDT[2]);  //转换为yyyy-MM-dd格式
    demoRDT = EndRDT.split("-");
    EndRDT = new Date(demoRDT[0] + '-' + demoRDT[1] + '-' + demoRDT[2]);
    res = parseInt((EndRDT - StarRDT) / 1000 / 60 / 60 / 24); //把相差的毫秒数转换为天数
    res = res + 1;
    //判断结束日期是否早于开始日期
    if (parseInt(EndRDT / 1000 / 60 / 60 / 24) < parseInt(StarRDT / 1000 / 60 / 60 / 24)) {
        alert("结束日期不能早于开始日期");
        res = "";
    }
    else {
        //当包含节假日的时候
        if (RDTRadio == 0) {
            var holidayEn = new Entity("BP.Sys.GloVar");
            holidayEn.No = "Holiday";
            if (holidayEn.RetrieveFromDBSources() == 1) {
                var holidays = holidayEn.Val.split(",");
                res = res - (holidays.length - 1);
                //检查计算的天数
                if (res <= 0) {
                    alert("请假时间内均为节假日");
                    res = "";
                }
            }
        }
    }
    return res;

}

function GetMapExtsGroup(mapExts) {
    var map = {};
    var mypk = "";
    //对mapExt进行分组，根据AttrOfOper
    $.each(mapExts, function (i, mapExt) {
        //不是操作字段不解析
        if (mapExt.AttrOfOper == "")
            return true;
        if (mapExt.ExtType == "DtlImp"
            || mapExt.MyPK.indexOf(mapExt.FK_MapData + '_Table') >= 0
            || mapExt.MyPK.indexOf('PageLoadFull') >= 0
            || mapExt.ExtType == 'StartFlow'
            || mapExt.ExtType == 'AutoFullDLL'
            || mapExt.ExtType == 'ActiveDDLSearchCond'
            || mapExt.ExtType == 'AutoFullDLLSearchCond')
            return true;

        mypk = mapExt.FK_MapData + "_" + mapExt.AttrOfOper;
        if (!map[mypk])
            map[mypk] = [mapExt];
        else
            map[mypk].push(mapExt);
    });
    var res = [];
    Object.keys(map).forEach(key => {
        res.push({
            attrKey: key,
            data: map[key],
        })
    });
    console.log(res);
    return map;
}