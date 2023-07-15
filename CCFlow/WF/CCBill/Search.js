var dblist = null;
/**
 * 是否自动填充数据
 * @param {any} mapAttr
 */
function isHaveAutoFull(mapAttr) {
    if (mapExts == null || mapExts == undefined)
        return false;
    var isHave = false;
    $.each(mapExts, function (idex, mapExt) {
        if ((mapExt.AttrOfOper == mapAttr.Field || mapExt.AttrsOfActive == mapAttr.Field)
            && mapExt.ExtType == "AutoFullDLLSearchCond") {
            isHave = true;
            return false;
        }
    })
    if (isHave)
        return true;
    return false;
}
/**
 * 是否有联动数据
 * @param {any} mapAttr
 */
function isHaveActiveDDLSearchCond(mapAttr) {
    if (mapExts == null || mapExts == undefined)
        return false;
    var isHave = false;
    $.each(mapExts, function (idex, mapExt) {
        if ((mapExt.AttrOfOper == mapAttr.Field || mapExt.AttrsOfActive == mapAttr.Field)
            && mapExt.ExtType == "ActiveDDLSearchCond") {
            isHave = true;
            return false;
        }
    })
    if (isHave)
        return true;
    return false;
}
/**
* 配置下拉框数据
* @param frmData
* @param mapAttr
* @param defVal
*/
function InitDDLOperation(frmData, mapAttr, defVal, ddlShowWays, selectSearch) {
	const defaultValue = defVal;
    defVal = "," + defVal + ",";
    var operations = [];
    var isAutoFull = isHaveAutoFull(mapAttr);
    var isActiveDDL = isHaveActiveDDLSearchCond(mapAttr);
    if (isAutoFull == false && isActiveDDL == false)
        operations.push({
            name: "全部",
            value: "all"
        });
    var ens = frmData[mapAttr.Field];
    if (ens == null || ens == undefined) {
        operations.push({
            name: "否",
            value: "0"
        });
        operations.push({
            name: "是",
            value: "1"
        });
    } else {
        ens.forEach(function (en) {
            if (en.No == undefined)
                if (en.IntKey == undefined) {
                    operations.push({
                        name: en.Name,
                        value: en.BH,
                        selected: defVal.indexOf(","+en.BH+",")!=-1 ? true : false
                    });
                } else {
                    operations.push({
                        name: en.Lab,
                        value: en.IntKey,
                        selected: defVal.indexOf("," + en.IntKey + ",") != -1? true : false
                    });
                }

            else
                operations.push({
                    name: en.Name,
                    value: en.No,
                    selected: defVal.indexOf("," + en.No + ",") != -1 ? true : false
                });
        })
    }

	defVal = defaultValue;
    if ((isAutoFull == true || isActiveDDL == true) && defVal == 'all') {
        defVal = operations[0].value;

    }

    var showWay = ddlShowWays[mapAttr.Field];
    showWay = showWay == null || showWay == undefined || showWay == "" ? "0" : showWay;
    var isRadioSelect = 1;
    var ss = showWay.split("_");
    if (ss.length == 2) {
        showWay = ss[0];
        isRadioSelect = ss[1];
    } else {
        showWay = ss[0];
        isRadioSelect = 1;
    }
    if (showWay == 0 && (isAutoFull == true || isActiveDDL == true)) {
        showWay = 2;
        isRadioSelect = 1;
    }
    selectSearch.push({
        key: mapAttr.Field,
        label: mapAttr.Name,
        value: defVal,
        showWay: showWay, //0下拉 1平铺
        operations: operations,
        isRadioSelect: isRadioSelect
    });

    return selectSearch;
}
/**
 * 获取查询table的列集合处理
 * @param {any} thrMultiTitle 三级表头的内容
 * @param {any} secMultiTitle 二级表头的内容
 * @param {any} ColorSet 列字段颜色显示
 * @param {any} attrs 显示列的集合
 * @param {any} sortColumns 排序的字段
 * @param {any} openModel 行打开方式
 * @param {any} openTitle 弹窗的标题
 * @param {any} entityType 当前表单是实体类，单据，数据源
 * @param {any} isBatch 是否可以批处理
 */

//判断是否是多级表头
var isThrHeader = false; //是否是三级表头
var isSecHeader = false;//是否是二级表头
var richAttrs = [];
function GetColoums(thrMultiTitle, secMultiTitle, colorSet, sortColumns, openModel, openTitle, entityType,isBatch) {
    var foramtFunc = mapData.ForamtFunc;
    foramtFunc = foramtFunc == null || foramtFunc == undefined ? "" : foramtFunc;
    var handler = new HttpHandler("BP.CCBill.WF_CCBill");
    handler.AddPara("FrmID", frmID);
    var data = handler.DoMethodReturnString("Search_MapAttr");
    if (data.indexOf("err@") != -1) {
        layer.alert(data);
        return;
    }
    data = JSON.parse(data);
    var attrs = data.Attrs;
    var sys_enums = data.Sys_Enum;
    var methods = data.Frm_Method;
    if (thrMultiTitle == null || thrMultiTitle == undefined)
        thrMultiTitle = "";
    if (thrMultiTitle != "")
        isThrHeader = true;

    if (secMultiTitle == null || secMultiTitle == undefined)
        secMultiTitle = "";
    if (isThrHeader == false && secMultiTitle != "")
        isSecHeader = true;

    var firstColumns = new Array(); //一级菜单
    var secondColumns = new Array(); //二级菜单
    var threeColumns = new Array(); //三级菜单
    var fieldColumns = {};
    //判断查询列表是不是有其他删除，集合操作，无不显示复选框
    if (isHaveDelOper == true || isHaveSeachOper == true || isBatch == true) {
        fieldColumns = {
            type: 'checkbox',
            rowspan: isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1

        };
        AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1);
    } else {
        fieldColumns = {
            title: '序',
            field: '',
            align: 'center',
            width: 50,
            rowspan: isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1,
            templet: function (d) {
                return pageSize * (pageIdx - 1) + d.LAY_TABLE_INDEX + 1;    // 返回每条的序号： 每页条数 *（当前页 - 1 ）+ 序号

            }
        };
        AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1);
    }
    
    
    for (var i = 0; i < attrs.length; i++) {
        var attr = attrs[i];

        if (attr.UIVisible == 0
            || attr.KeyOfEn == "OID"
            || attr.KeyOfEn == "WorkID"
            || attr.KeyOfEn == "NodeID"
            || attr.KeyOfEn == "MyNum"
            || attr.KeyOfEn == "MyPK") {
            keyOfEn = attr.KeyOfEn
            continue;
        }


        var keyRowSpan = GetAttrKeyRowSpan(attr.KeyOfEn, secMultiTitle, thrMultiTitle);
        if (keyRowSpan == 3 && thrcolspan.field != undefined) {
            threeColumns.push(thrcolspan);
            thrcolspan = {};
        }

        //是否增加二级或者三级分组
        if ((isSecHeader == true && keyRowSpan == 1)
            || isThrHeader == true && keyRowSpan != 3)
            AddSecOrThrColumn(attr.KeyOfEn, keyRowSpan, secondColumns, threeColumns, secMultiTitle, thrMultiTitle);

        var field = attr.KeyOfEn;
        var title = attr.Name;
        var width = attr.Width;
        var sortable = false;
        if (sortColumns != null && sortColumns != "")
            sortable = sortColumns.indexOf(field) != -1 ? true : false;

        if (field == "BillState") {
            fieldColumns = {
                field: field,
                title: title,
                minWidth: attr.Width,
                fixed: false,
                sort: sortable,
                rowspan: keyRowSpan,
                templet: function (row) {
                    return GetBillState(row[this.field]);
                }
            };
            AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, keyRowSpan);
            continue;
        }

        if (field == "Title") {
            fieldColumns = {
                field: field,
                title: title,
                minWidth: attr.Width,
                fixed: false,
                sort: sortable,
                rowspan: keyRowSpan,
                templet: function (row) {
                    var icon = GenerICON(false, row.BillState);
                    var rowstr = JSON.stringify(row);
                    rowstr = encodeURIComponent(rowstr);
                    return "<a href=\"javascript:OpenIt('" + row.OID + "'," + entityType + "," + row.BillState + ",'" + rowstr + "')\"><img src=" + icon + " border=0 width='14px;' />" + row[this.field] + "</a>";
                }
            };
            AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, keyRowSpan);
            continue;
        }
        //枚举
        if (attr.MyDataType == 2 && attr.LGType == 1) {
            fieldColumns = {
                field: field,
                title: title,
                fixed: false,
                minWidth: width,
                sort: sortable,
                rowspan: keyRowSpan,
                uibindkey: attr.UIBindKey,
                templet: function (row) {
                    if (row[this.field] == -1)
                        return "无";
                    if (foramtFunc.indexOf(this.field + "@") != -1) {
                        formatter = foramtFunc.substring(foramtFunc.indexOf(this.field + "@"));
                        formatter = formatter.substring(0, formatter.indexOf(";"));
                        var strs = formatter.split("@");
                        if (strs.length == 2) {
                            val = cceval(strs[1] + "('" + row[this.field] + "')");
                            return val;
                        }
                    }
                    var val = row[this.field + "Text"];
                    if ((val == undefined || val == "") && entityType == 100) {
                        //获取外键对应的文本值
                        val = GetDDLText(this.field, row[this.field], this.uibindkey, data);
                    }
                    if (val == undefined || val == null)
                        return row[this.field];
                    else
                        return val;
                }
            };
            AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, keyRowSpan);
            continue;
        }
        //枚举多选
        if (attr.MyDataType == 1 && attr.LGType == 1 && attr.UIContralType == 2) {
            fieldColumns = {
                field: field,
                title: title,
                fixed: false,
                minWidth: width,
                sort: sortable,
                rowspan: keyRowSpan,
                uibindkey: attr.UIBindKey,
                templet: function (row) {
                    var val = row[this.field];
                    if (val == -1 || val == "")
                        return "无";
                    if (foramtFunc.indexOf(this.field + "@") != -1) {
                        formatter = foramtFunc.substring(foramtFunc.indexOf(this.field + "@"));
                        formatter = formatter.substring(0, formatter.indexOf(";"));
                        var strs = formatter.split("@");
                        if (strs.length == 2) {
                            val = cceval(strs[1] + "('" + row[this.field] + "')");
                            return val;
                        }
                    }
                    var bindkey = this.uibindkey
                    var enums = $.grep(sys_enums, function (item) {
                        return item.EnumKey == bindkey;
                    });
                    if (enums.length == 0)
                        return val;
                    val = val + ",";
                    var str = [];
                    $.each(enums, function (i, item) {
                        if (val.indexOf(item.IntKey + ",") != -1)
                            str.push(item.Lab);
                    })
                    return str.join(",");
                }
            };
            AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, keyRowSpan);
            continue;
        }
        if (attr.UIContralType == 1 || attr.UIContralType == 3) {
            if (width == null || width == "" || width == undefined)
                width = 180;

            fieldColumns = {
                field: field,
                title: title,
                fixed: false,
                minWidth: width,
                sort: sortable,
                rowspan: keyRowSpan,
                uibindKey: attr.UIBindKey,
                style: {
                    css: { "white-space": "nowrap", "word-break": "keep-all", "width": "100%" }
                },
                templet: function (row) {
                    var val = row[this.field + "Text"];
                    if (val == undefined || val == null)
                        val = row[this.field + "T"];
                    if ((val == undefined || val == "") && row[this.field] != "" && entityType == 100) {
                        //获取外键对应的文本值
                        val = GetDDLText(this.field, row[this.field], this.uibindKey, data);
                    }
                    if (val == undefined || val == null)
                        return row[this.field];
                    else
                        return val;
                }
            };
            AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, keyRowSpan);
            continue;
        }
        if (attr.UIContralType == 2) {
            fieldColumns = {
                field: field,
                title: title,
                minWidth: attr.Width,
                fixed: false,
                sort: sortable,
                rowspan: keyRowSpan,
                templet: function (row) {
                    var val = "";
                    if (row[this.field] == "0")
                        val = "否";
                    if (row[this.field] == "1")
                        val = "是";

                    return FieldColorSet(colorSet, this.field, row[this.field], val,row)

                }
            };
            AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, keyRowSpan);
            continue;
        }

        if (width == null || width == "" || width == undefined)
            width = 100;
        if (attr.IsRichText == "1") {
            richAttrs.push(attr);
            fieldColumns = {
                field: field,
                title: title,
                width: width,
                fixed: false,
                sort: sortable,
                rowspan: keyRowSpan,
                templet: function (row) {
                    var val = row[this.field];
                    if (val == "")
                        return val;
                    val = htmlDecodeByRegExp(val);
                    return "<div style='margin:9px 0px 9px 15px'>" + val + "</div>";

                }
            };
            AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, keyRowSpan);
            continue;
        }


        fieldColumns = {
            field: field,
            title: title,
            minWidth: attr.Width,
            fixed: false,
            sort: sortable,
            rowspan: keyRowSpan,
            templet: function (row) {
                var val = row[this.field];
                if (val == null)
                    val = "";

                if (foramtFunc.indexOf(this.field + "@") != -1) {
                    formatter = foramtFunc.substring(foramtFunc.indexOf(this.field + "@"));
                    formatter = formatter.substring(0, formatter.indexOf(";"));
                    var strs = formatter.split("@");
                    if (strs.length == 2) {
                        val = cceval(strs[1] + "('" + val + "')");
                    }
                }

                return FieldColorSet(colorSet, this.field, val, val,row);

            }

        };
        AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, keyRowSpan);
    }
    fieldColumns = {
        title: '操作',
        field: 'Oper',
        align: 'center',
        minWidth: 80,
        rowspan: isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1,
        templet: function (row) {
            var _html = "";
            var rowstr = JSON.stringify(row);
            rowstr = encodeURIComponent(rowstr);
            /*if (row.BillState == 100 || entityType == 100) {
                _html += "<a href='javascript:void(0)'onclick='OpenIt(\"" + row.OID + "\"," + entityType + "," + row.BillState + ",\"" + rowstr + "\")'style='color:blue'>详情</a>";
            }
            else
                _html += "<a href='javascript:void(0)'onclick='OpenIt(\"" + row.OID + "\"," + entityType + "," + row.BillState + ")'style='color:blue'>编辑</a>";*/
            //增加其他的方法
            $.each(methods, function (idx, method) {
                _html += "<span style='padding: 0px 3px; color:#ccc'>|</span><a href='javascript:void(0)'onclick='DoMethod(\"" + method.No + "\",\"" + row.OID + "\",\"" + rowstr + "\")'style='color:blue'>" + method.Name + "</a>";
            })
            if (isHaveDelOper == true)
                _html += "<span style='padding: 0px 3px; color:#ccc'>|</span><a href='javascript:void(0)'onclick='DeleteIt(\"" + row.OID + "\"," + entityType + ")' style='color:red'>删除</a>";
            return _html;
        }
    };
    if (dblist != null && dblist.DBType == 2 && methods.length == 0 && isHaveDelOper == false) {

    } else {
        AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1);
    }

    if (thrcolspan.field != undefined)
        threeColumns.push(thrcolspan);

    var columns = new Array();
    if (isThrHeader == false && isSecHeader == false)
        columns[0] = firstColumns;
    if (isSecHeader == true) {
        columns.push(secondColumns);
        columns.push(firstColumns);

    }

    if (isThrHeader == true) {
        columns.push(threeColumns);
        columns.push(secondColumns);
        columns.push(firstColumns);
    }
    return columns;
}
/**
 * 查询数据
 */
function SearchData(key, val) {
    var handler = new HttpHandler("BP.CCBill.WF_CCBill");
    handler.AddUrlData()
    handler.AddPara("PageIdx", pageIdx);
    handler.AddPara("PageSize", pageSize);
    if (key != null && key != undefined && key != "")
        handler.AddPara(key, val);
    if (orderBy != null && orderBy != undefined)
        ur.OrderBy = orderBy;
    if (orderWay != null && orderWay != undefined)
        ur.OrderWay = orderWay;
    ur.Update();

    //查询集合
    var data;
    if (mapData.EntityType == 100)
        data = handler.DoMethodReturnString("SearchDB_Init");
    else
        data = handler.DoMethodReturnString("Search_Init");

    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    data = JSON.parse(data);

    ur = new Entity("BP.Sys.UserRegedit");
    ur.MyPK = webUser.No + frmID + "_SearchAttrs";
    ur.RetrieveFromDBSources();

    return transferHtmlData(data["DT"]);
}
/**
 * 打开新页面的方式
 * @param {any} workid 实体的WorkID
 * @param {any} frmID 实体的表单ID
 * @param {any} openModel 打开方式 //0=新窗口打开 1=在本窗口打开 2=弹出窗口打开,关闭后不刷新列表 3=弹出窗口打开,关闭刷新
 * @param {any} title 标题
 */
function OpenIt(workid, entityType, billstate, row, isOpenAdd) {
    if (row != null && row != undefined && row != "")
        row = JSON.parse(decodeURIComponent(row));

    var IsReadonly = 0;
    IsReadonly = billstate == 100 ? 1 : 0;
    var url = "";
    var frmID = GetQueryString("FrmID");
    //页面打开方式
    var urlOpenType = mapData.SearchDictOpenType;
    var url = "";
    if (urlOpenType == null || urlOpenType == undefined
        || urlOpenType == 0 || urlOpenType == 2) {
        url = "MyDictFrameWork.htm?WorkID=" + workid + "&FrmID=" + frmID + "&FK_MapData=" + frmID + "&IsReadonly=" + IsReadonly;
        if (entityType == 1)
            url = "MyBill.htm?WorkID=" + workid + "&FrmID=" + frmID + "&FK_MapData=" + frmID + "&IsReadonly=" + IsReadonly;
    }

    if (urlOpenType == 1)
        url = "MyDict.htm?WorkID=" + workid + "&FrmID=" + frmID + "&FK_MapData=" + frmID + "&IsReadonly=" + IsReadonly;

    if (urlOpenType == 9) {
        url = mapData.UrlExt;
        if (url.indexOf('?') == -1)
            url = url + "?1=1";
        if (url.indexOf('FrmID') != -1)
            url = url + "&WorkID=" + workid + "&OID=" + workid;

        if (url.indexOf("@") != -1) {
            url = DealJsonExp(row, url);
        }
    }

    if (mapData.RowOpenModel == 0) {
        if (urlOpenType != 9)
            url = basePath + "/WF/CCBill/" + url;
        if (top.vm != null)
            top.vm.openTab(mapData.Name, url);
        else
            window.open(url);
        return;
    }
    if (mapData.RowOpenModel == 3)
        OpenLayuiDialog(url, "", 90000, 0, null, true);
    else {
        if (isOpenAdd == true)
            OpenLayuiDialog(url, "", 90000, 0, null, true);
        else
            OpenLayuiDialog(url, "", 90000, 0, null, false);
    }

    return;
}
/**
 * 查询数据
 */
function Search() {
    if ($("#TB_Key") != null && $("#TB_Key").val() != "")
        ur.SearchKey = $("#TB_Key").val();
    else
        ur.SearchKey = "";

    //设置查询时间.
    if ($("#TB_DTFrom").length == 1)
        ur.DTFrom = $("#TB_DTFrom").val();

    if ($("#TB_DTTo").length == 1)
        ur.DTTo = $("#TB_DTTo").val();

    //获得外键的查询条件,存储里面去.
    var str = "";
   
    $.each(searchData["selectSearch"], function (i, item) {
        if (item.showWay == 0 && item.isRadioSelect == 0) {
            var val = xmSelect.get('#XmlSelect_' + item.key, true).getValue('value');
            if (val.join(",").indexOf("all") != -1)
                str += "@" + item.key + "=all";
            else
                str += "@" + item.key + "=" + val.join(",");
        } else {
            str += "@" + item.key + "=" + $("#DDL_" + item.key).val();
        }
       
           

    });
    $.each(searchData["inputSearch"], function (i, item) {
        if (item.key == "key")
            return true;
        var strs = $("input[name='TB_" + item.key + "']");
        if (strs.length == 1) {
            ur.SetPara(item.key, $("#TB_" + item.key).val());
        } else {
            if ($("#TB_" + item.key + "_0").val() == "" && $("#TB_" + item.key + "_1").val() == "")
                ur.SetPara(item.key, "");
            else
                ur.SetPara(item.key, $("#TB_" + item.key + "_0").val() + "," + $("#TB_" + item.key + "_1").val());
        }
    })

    ur.FK_Emp = webUser.No;
    ur.CfgKey = "SearchAttrs";
    ur.Vals = str;
    ur.FK_MapData = frmID;
    ur.Update();
    pageIdx = 1;
    tableData = SearchData();

    if ($("#lay_table_bill").length != 0)
        layui.table.reload('lay_table_bill', { data: tableData });
    else
        layui.table.reload('lay_table_dict', { data: tableData });
    renderLaypage();
}
/**
 * tab时间的选择的查询
 * @param {any} selectVal
 * @param {any} selectType
 * @param {any} obj
 * @param {any} type
 */
function SearchByDate(type, selectVal, selectType, obj) {
    //去掉选择的节点的class
    if (obj != null) {
        if (selectType == "year")
            //修改其他年度的情况
            $(".layui-year").removeClass("layui-a-this");
        else
            $(obj).siblings().removeClass("layui-a-this");

        $(obj).addClass("layui-a-this");
        $("a[name=" + selectVal + "]").addClass("layui-a-this");

    }
    if (selectType != null && selectType != undefined) {
        if (selectType == "year")
            searchData.dateTabSearch.value[0] = selectVal;
        if (selectType == "month")
            searchData.dateTabSearch.value[1] = selectVal;
        if (selectType == "jidu")
            searchData.dateTabSearch.value[2] = selectVal;
    }
    var dateValue = searchData.dateTabSearch.value;
    //设置时间段
    if (type == "year") {
        selectVal = selectVal == null || selectVal == undefined ? dateValue[0] : selectVal;
        ur.DTFrom = selectVal + "-01-01";
        ur.DTTo = selectVal + "-12-31";
    }
    if (type == "month") {
        selectVal = selectVal == null || selectVal == undefined ? dateValue[1] : selectVal;
        selectVal = selectVal.replace("月", "");
        var month = parseInt(selectVal) < 10 ? "0" + selectVal : selectVal;
        var year = searchData.dateTabSearch.value[0];
        var date = new Date(parseInt(year), parseInt(month), 0)
        var days = date.getDate();
        ur.DTFrom = year + "-" + month + "-01";
        ur.DTTo = year + "-" + month + "-" + days;
    }
    if (type == "jidu") {
        selectVal = selectVal == null || selectVal == undefined ? dateValue[2] : selectVal;
        var jidu = 0;
        if (selectVal == "二季度")
            jidu = 1;
        if (selectVal == "三季度")
            jidu = 2;
        if (selectVal == "四季度")
            jidu = 3;
        var year = searchData.dateTabSearch.value[0];
        var beginMonth = jidu * 3 + 1;
        var endMonth = (jidu + 1) * 3;
        var date = new Date(parseInt(year), parseInt(endMonth), 0)
        var days = date.getDate();
        endMonth = endMonth < 10 ? "0" + endMonth : "" + endMonth;
        beginMonth = beginMonth < 10 ? "0" + beginMonth : "" + beginMonth;
        ur.DTFrom = year + "-" + beginMonth + "-01";
        ur.DTTo = year + "-" + endMonth + "-" + days;
    }
    Search();
}
/**
 * 根据下拉框执行
 * @param {any} ddlKey
 * @param {any} ddlVal
 */
function SearchBySelect(ddlKey, ddlVal) {
    $("#DDL_" + ddlKey).val(ddlVal);
    Search();
}
/**
   * 把列增加到对应的数组中
   * @param column 列信息
   * @param firstColumns 一级表头数组
   * @param secondColumns 二级表头数组
   * @param threeColumns 三级表头数组
   * @param columnIdx 隶属那个表头
   */
function AddColumn(column, firstColumns, secondColumns, threeColumns, columnIdx) {
    if (columnIdx == 1)
        firstColumns.push(column);
    if (columnIdx == 2)
        secondColumns.push(column);
    if (columnIdx == 3)
        threeColumns.push(column);

}

/**
 * 获取数据字段跨的行数
 * @param keyOfEn
 */
function GetAttrKeyRowSpan(keyOfEn, secMultiTitle, thrMultiTitle) {
    //一级表头
    if (isThrHeader == false && isSecHeader == false)
        return 1;
    //先判断是否是隶属于二级表头下的字段
    if (secMultiTitle.indexOf("," + keyOfEn + ",") != -1)
        return 1;
    //是否是隶属于三级表头下的字段
    if (thrMultiTitle.indexOf("," + keyOfEn + ",") != -1)
        return 2;
    return isThrHeader == true ? 3 : 2;

}
/**
 * 增加二级或三级表头的分组
 * @param keyOfEn
 * @param keyRowSpan
 * @param secondColumns
 * @param threeColumns
 */
var curSecGroup = "";
var curTreGroup = "";
var thrcolspan = {};
var isSecChange = false;
function AddSecOrThrColumn(keyOfEn, keyRowSpan, secondColumns, threeColumns, secMultiTitle, thrMultiTitle) {
    //计算二级表头分组
    var secfilds = getMutliFile(keyOfEn, secMultiTitle);
    var secRowSpan = 1;
    if (isSecHeader == true && keyRowSpan == 1)
        secRowSpan = 1;
    if (isThrHeader == true && keyRowSpan == 1) {
        if (thrMultiTitle.indexOf("," + secfilds[0] + ",") == -1)
            secRowSpan = 2;
        else
            secRowSpan = 1;
    }
    if (isThrHeader == true && keyRowSpan == 2)
        secRowSpan = 0;
    //增加二级表头
    if (secRowSpan == 1) {
        var colspan = secfilds.length - 1;
        if (curSecGroup == "" || curSecGroup != secfilds[0]) {
            curSecGroup = secfilds[0];
            isSecChange = true;
            secondColumns.push({
                title: secfilds[0],
                field: '',
                align: 'center',
                colspan: colspan,
                rowspan: secRowSpan
            });
        } else {
            isSecChange = false;
        }
        //增加三级表头
        if (isThrHeader == true && secRowSpan == 1) {
            var filds = getMutliFile(secfilds[0], thrMultiTitle);
            if (curTreGroup == "" || curTreGroup != filds[0]) {
                //增加三级表头
                if (curTreGroup != "" && curTreGroup != filds[0]) {
                    threeColumns.push(thrcolspan);
                    thrcolspan = {};
                }
                thrcolspan = {
                    title: filds[0],
                    field: '',
                    align: 'center',
                    colspan: colspan,
                }

                curTreGroup = filds[0];
            } else {
                if (isSecChange == true)
                    thrcolspan.colspan = thrcolspan.colspan + colspan;
            }


        }
    }
    //三级表头
    if (secRowSpan == 2) {
        if (thrcolspan.title != undefined) {
            threeColumns.push(thrcolspan);
            thrcolspan = {};
        }
        if (curTreGroup == "" || curTreGroup != secfilds[0]) {
            curTreGroup = secfilds[0];
            threeColumns.push({
                title: secfilds[0],
                field: '',
                align: 'center',
                colspan: secfilds.length - 1,
                rowspan: secRowSpan
            });
        }
    }

    if (secRowSpan == 0) {
        filds = getMutliFile(keyOfEn, thrMultiTitle);
        if (curTreGroup == "" || curTreGroup != filds[0]) {
            //增加三级表头
            if (curTreGroup != "" && curTreGroup != filds[0]) {
                threeColumns.push(thrcolspan);
            }
            thrcolspan = {
                title: filds[0],
                field: '',
                align: 'center',
                colspan: 1,
            };
            curTreGroup = filds[0];
        } else {
            thrcolspan.colspan = thrcolspan.colspan + 1;
        }


    }

}
function getMutliFile(keyOfEn, multi) {
    var fields = multi.split(";");
    for (var i = 0; i < fields.length; i++) {
        var str = fields[i];
        if (str == "")
            continue;
        if (str.indexOf("," + keyOfEn + ",") == -1)
            continue;
        var strs = str.substring(0, str.length - 1).split(",");
        return strs;
    }
    return "";
}
/**
 * 获取单据，实体状态对应的图标
 * @param {any} isCanDo
 * @param {any} BillState
 */
function GenerICON(isCanDo, BillState) {

    if (BillState == 3)
        icon = "./Img/BillState/Complete.png";  //已经完成.
    else if (BillState == 2)
        icon = "./Img/BillState/Runing.png"; //运行中.
    else if (BillState == 5)
        icon = "./Img/BillState/ReturnSta.png"; //退回.
    else
        icon = "./Img/BillState/Etc.png"; //其他.

    if (isCanDo == true && BillState != 3)
        icon = "./Img/BillState/Todo.png"; //其他.

    return icon;
}

/**
* 获取字段的设置
* @param colorSet  颜色总体设置
* @param keyOfEn 字段
*/
function getFieldColor(colorSet, keyOfEn) {
    var fieldColor = [];
    var colorSets = colorSet.split('@');
    for (var i = 0; i < colorSets.length; i++) {
        if (colorSets[i] == "")
            continue;
        var strs = colorSets[i].split(':');
        if (strs.length == 0 || strs.length == 1)
            continue;
        if (strs[0] != keyOfEn)
            continue;
        var ss = strs[1].split(';');
        for (var k = 0; k < ss.length; k++) {
            if (ss[k] == "")
                continue;
            var ts = ss[k].split(',');
            if (ts.length < 3) {
                alert('字段' + keyOfEn + '范围颜色设置格式错误');
                break;
            }

            fieldColor.push({
                "From": ts[0].replace("From=", ""),
                "To": ts[1].replace("To=", ""),
                "Color": ts[2].replace("Color=", "")
            });
        }



    }
    return fieldColor;
}

/**
 * 获取字段颜色
 * @param {any} colorSet
 * @param {any} field
 * @param {any} val
 * @param {any} valText
 */
function FieldColorSet(colorSet, field, val, valText, rowData) {
    var fieldColor = [];
    if (colorSet.indexOf("@" + field + ":") != -1) {
        fieldColor = getFieldColor(colorSet, field);
    }
    if (fieldColor.length == 0)
        return valText;
    var reg = /^[0-9]+.?[0-9]*/;
    for (var i = 0; i < fieldColor.length; i++) {
        var color = fieldColor[i];
        if (color.Color.indexOf("_")==0)
            color.Color = rowData[color.Color.substring(1)];
        if (color.From == 0 && color.To == 0) {
            var stylecss = "padding: 0 5px;font-size: 14px;white-space: nowrap;border-radius: 2px;text-align:center;";
            return '<div style="' + stylecss + 'background-color:' + color.Color + ';">' + valText + '</div>';
        }
        //说明是字符串，需要修改
        if (reg.test(color.From) == false && reg.test(color.To) == false && (color.From == valText || color.To == valText)) {
            var stylecss = "padding: 0 5px;font-size: 14px;white-space: nowrap;border-radius: 2px;text-align:center;";
            return '<div style="' + stylecss + 'background-color:' + color.Color + ';">' + valText + '</div>';
        }
        if (reg.test(color.From) == true && reg.test(color.To) == true
            && parseInt(color.From) <= val && parseInt(color.To) >= val) {
            var stylecss = "padding: 0 5px;font-size: 14px;white-space: nowrap;border-radius: 2px;text-align:center;";
            return '<div style="' + stylecss + 'background-color:' + color.Color + ';">' + valText + '</div>';
        }
    }
    return valText;
}

/**
 * 打开链接
 * @param {any} no
 * @param {any} source
 */
function OpenLink(no, source) {
    var enName = "BP.CCBill.Template.CollectionLink";
    if (source == "Method")
        enName = "BP.CCBill.Template.MethodLink";
    var link = new Entity(enName, no);
    var url = link.Tag1;
    if (url == null || url == undefined || url == "") {
        layer.alert("自定义按钮/超链接的内容为空，请联系管理员");
        return;
    }
    url = url.indexOf("?") == -1 ? url + "?1=1" : url;
    url = url.replace(/@FrmID/g, frmID);
    url = url.replace(/@FK_MapData/g, frmID);
    if(url.indexOf("FrmID")==-1)
        url += "&FrmID=" + frmID;
    url = DealExp(url);
    var w = link.PopWidth || window.innerWidth * 2/3;
    if (w < window.innerWidth * 2 / 3)
        w = window.innerWidth * 2 / 3;
    if (link.RefMethodType == 0) {//0=模态窗口打开@1=新窗口打开@2=右侧窗口打开@4=转到新页面
        OpenLayuiDialog(url, link.Name, w, null, "r", false);
        return;
    }
    window.top.vm.openTab(link.Name, url);
}
/**
 * 列表集合的方法操作
 * @param {any} no
 * @param {any} source
 */
function OpenFunc(no, source) {
    var enName = "BP.CCBill.Template.CollectionFunc";
    if (source == "Method")
        enName = "BP.CCBill.Template.MethodFunc";
    var func = new Entity(enName, no);

    var checkStatus = layui.table.checkStatus("lay_table_dict");
    if (checkStatus.data.length == 0) {
        layer.alert("请选择" + func.Name + "的行");
        return;
    }
    layer.confirm('确定要' + func.Name + '选择的数据吗?', function (index) {
        layer.close(index);
        var workids = [];
        for (var i = 0; i < checkStatus.data.length; i++) {
            workids.push(checkStatus.data[i]["OID"]);
        }
        workids = workids.join(",");
        //执行方法.
        var isHaveAttr = false;
        var attrs = new Entities("BP.Sys.MapAttrs", "FK_MapData", func.MyPK);
        if (attrs.length > 0)
            isHaveAttr = true;


        //带有参数的方法.
        if (isHaveAttr == true) {
            var url = "./Opt/DoMethodPara.htm?No=" + func.MethodID + "&WorkIDs=" + workids + "&FrmID=" + frmID;
            OpenLayuiDialog(url, func.Name, window.innerWidth * 2 / 3, null, 'r', true)
            return;
        }

        var url = "./Opt/DoMethod.htm?No=" + func.MethodID + "&WorkIDs=" + workids + "&FrmID=" + frmID;
        OpenLayuiDialog(url, func.Name, window.innerWidth * 2 / 3, null, 'r', true)

    });
}
/**
 * 批量发起单据信息
 * @param {any} no
 * @param {any} source
 */
function OpenBill(no, source) {

    var enName = "BP.CCBill.Template.MethodBill";
    var bill = new Entity(enName, no);
    var billFrm = bill.Tag1;
    if (billFrm == null || billFrm == undefined || billFrm == "") {
        layer.alert("批量发起的单据为空，请联系管理员");
        return;
    }
    var checkStatus = layui.table.checkStatus("lay_table_dict");
    if (checkStatus.data.length == 0) {
        layer.alert("请选择" + bill.Name + "操作的行");
        return;
    }
    //发起单据流程
    layer.confirm('确定要' + bill.Name + '选择的数据吗?', function (index) {
        layer.close(index);
        var workids = [];
        for (var i = 0; i < checkStatus.data.length; i++) {
            workids.push(checkStatus.data[i]["OID"]);
        }
        workids = workids.join(",");
        var handler = new HttpHandler("BP.CCBill.WF_CCBill");
        handler.AddPara("FrmID", billFrm);
        handler.AddPara("FromFrmID", bill.FrmID);
        handler.AddPara("MethodNo", no);
        handler.AddPara("WorkIDs", workids);
        var data = handler.DoMethodReturnString("MyDict_DoBill_Start");
        if (data.indexOf("err@") != -1) {
            layer.alert(data);
            return;
        }
        OpenLayuiDialog(data, "", window.innerWidth * 2 / 3, null, 'r', true)

    });
}
/**
 * 发起流程
 * @param {any} no
 * @param {any} source
 */
function OpenFlow(no, source) {

    var enName = "BP.CCBill.Template.CollectionFlowBatch";
    var flowM = new Entity(enName, no);
    var flowNo = flowM.Tag1;
    if (flowNo == null || flowNo == undefined || flowNo == "") {
        layer.alert("批量发起的流程编号为空，请联系管理员");
        return;
    }
    var checkStatus = layui.table.checkStatus("lay_table_dict");
    if (checkStatus.data.length == 0) {
        layer.alert("请选择" + flowM.Name + "操作的行");
        return;
    }
    //发起单据流程
    layer.confirm('确定要' + flowM.Name + '选择的数据吗?', function (index) {
        layer.close(index);
        var workids = [];
        for (var i = 0; i < checkStatus.data.length; i++) {
            workids.push(checkStatus.data[i]["OID"]);
        }
        workids = workids.join(",");
        var handler = new HttpHandler("BP.CCBill.WF_CCBill");
        handler.AddPara("FK_Flow", flowNo);
        handler.AddPara("FromFrmID", flowM.FrmID);
        handler.AddPara("MethodNo", no);
        handler.AddPara("WorkIDs", workids);
        var data = handler.DoMethodReturnString("MyDict_DoFlowBatchBaseData_StartFlow");
        if (data.indexOf("err@") != -1) {
            layer.alert(data);
            return;
        }
        data = data.replace("../", "/WF/");
        window.top.vm.openTab(flowM.Name, basePath+data);

    });
}

/**
 * 新增实体类流程
 * @param {any} no
 * @param {any} source
 */
function OpenFlowEntity(no, source) {

    var enName = "BP.CCBill.Template.Collection";
    var flowM = new Entity(enName, no);
    var flowNo = flowM.FlowNo;
    if (flowNo == null || flowNo == undefined || flowNo == "") {
        layer.alert("新增实体类流程编号不能为空，请联系管理员");
        return;
    }
    var menuNo = flowM.FrmID + "_" + flowNo;
    var url = "/WF/CCBill/Opt/StartFlowByNewEntity.htm?FK_Flow=" + flowNo + "&MenuNo=" + menuNo;
    window.top.vm.openTab(flowM.Name, url);
}
/**
 * 删除选择的列数据
 * @param {any} oid
 * @param {any} entityType
 */
function DeleteIt(oid, entityType) {
    layer.confirm('确定要删除改行数据信息吗?', function (index) {

        var handler = new HttpHandler("BP.CCBill.WF_CCBill");
        handler.AddPara("FrmID", GetQueryString("FrmID"));
        handler.AddPara("WorkID", oid);
        var data = null;
        if (entityType == 1)
            data = handler.DoMethodReturnString("MyBill_Delete");
        if (entityType == 2)
            data = handler.DoMethodReturnString("MyDict_Delete");
        if (data.indexOf('err@') == 0) {
            layer.alert(data);
            return;
        }
        pageIdx = 1;
        var tableData = SearchData();
        layui.table.reload('lay_table_dict', { data: tableData });
        renderLaypage();
        layui.laypage.render();
        layer.close(index);

    });
}
/**
 * 执行操作中的方法
 * @param {any} methodNo
 * @param {any} workid
 */
function DoMethod(methodNo, workid, jsonStr) {
    jsonStr = jsonStr || "";
    jsonStr = decodeURIComponent(jsonStr);
    if (jsonStr != "")
        jsonStr = JSON.parse(jsonStr);
    var method = new Entity("BP.CCBill.Template.Method", methodNo);
    if (method.MethodModel === "Bill")
        method.Docs = "./Opt/Bill.htm?FrmID=" + method.Tag1 + "&MethodNo=" + method.No + "&WorkID=" + workid + "&From=Dict";
    //如果是一个方法.
    if (method.MethodModel === "Func") {
        if (method.IsHavePara == 0) {
            Skip.addJs("../../DataUser/JSLibData/Method/" + method.No + ".js");
            DBAccess.RunFunctionReturnStr(method.MethodID + "(" + workid + ")");
            return;
        }
        method.Docs = "./Opt/DoMethod.htm?FrmID=" + method.FrmID + "&No=" + method.No + "&WorkID=" + workid + "&From=Search";

    }

    if (method.MethodModel === "FrmBBS")
        method.Docs = "./OptComponents/FrmBBS.htm?FrmID=" + method.FrmID + "&No=" + method.No + "&WorkID=" + workid;
    if (method.MethodModel === "QRCode")
        method.Docs = "./OptComponents/QRCode.htm?FrmID=" + method.FrmID + "&MethodNo=" + method.No + "&WorkID=" + workid;

    //单个实体发起的流程汇总.
    if (method.MethodModel === "SingleDictGenerWorkFlows")
        method.Docs = "./OptOneFlow/SingleDictGenerWorkFlows.htm?FrmID=" + method.FrmID + "&No=" + method.No + "&MethodNo=" + method.No + "&WorkID=" + workid;

    //修改基础数据的的流程.
    if (method.MethodModel === "FlowBaseData") {
        var url = "./OptOneFlow/FlowBaseData.htm?WorkID=" + workid;
        url += "&FrmID=" + method.FrmID;
        url += "&MethodNo=" + method.No;
        url += "&FlowNo=" + method.FlowNo;
        method.Docs = url;
    }

    //其他业务流程.
    if (method.MethodModel == "FlowEtc") {

        var url = "./OptOneFlow/FlowEtc.htm?WorkID=" + workid;
        url += "&FrmID=" + method.FrmID;
        url += "&MethodNo=" + method.No;
        url += "&FlowNo=" + method.FlowNo;

        method.Docs = url;

    }

    //数据版本.
    if (method.MethodModel == "DataVer") {
        method.Docs = "./OptComponents/DataVer.htm?FrmID=" + method.FrmID + "&WorkID=" + workid;
    }

    //日志.
    if (method.MethodModel == "DictLog") {
        method.Docs = "./OptComponents/DictLog.htm?FrmID=" + method.FrmID + "&WorkID=" + workid;
    }

    //超链接.
    if (method.MethodModel == "Link") {
        method.Tag1 = method.Tag1.replace(/@FrmID/g, method.FrmID);
        method.Tag1 = method.Tag1.replace(/@FK_MapData/g, method.FrmID);
        method.Tag1 = method.Tag1.replace(/@OID/g, workid);
        method.Tag1 = method.Tag1.replace(/@WorkID/g, workid);
        if (method.Tag1.indexOf('?') == -1)
            method.Docs = method.Tag1 + "?1=1";
        else
            method.Docs = method.Tag1;

        method.Docs = DealJsonExp(jsonStr, method.Docs);

        if (method.Tag1.indexOf('FrmID') == -1)
            method.Docs += "&FrmID=" + method.FrmID;
        if (method.Tag1.indexOf('WorkID') == -1)
            method.Docs += "&WorkID=" + workid;
    }

    if (method.Docs === "") {

        var url = method.UrlExt;
        if (url === "") {
            layer.alert("没有解析的Url-MethodModel:" + method.MethodModel + " - " + method.Mark);
            return;
        }
        url = DealJsonExp(jsonStr, url);
        if (url.indexOf('?') > 0)
            method.Docs = url + "&FrmID=" + method.FrmID + "&WorkID=" + workid;
        else
            method.Docs = url + "?FrmID=" + method.FrmID + "&WorkID=" + workid;
    }

    if (method.MethodModel === "Func") {
        OpenLayuiDialog(method.Docs, method.Name, window.innerWidth / 2, 50, "auto");
    }
    else {
        var refmethodType = method.RefMethodType;
        if (refmethodType == 0 || refmethodType == 1) {//模态窗打开
            var h = method.PopHeight == 0 ? 70 : method.PopHeight;
            var w = method.PopWidth == 0 ? window.innerWidth / 2 : method.PopWidth;
            OpenLayuiDialog(method.Docs, method.Name, w, h, "auto");
            return;
        }
        if (refmethodType == 2) { //新页面打开
            window.top.vm.openTab(method.Name, method.Docs);
            return;
        }
        if (refmethodType == 3) {//侧滑打开
            var w = method.PopWidth == 0 ? window.innerWidth / 2 : method.PopWidth;
            OpenLayuiDialog(method.Docs, method.Name, w, 100, "r");
            return;
        }
        if (refmethodType == 4) {//转新页面
            window.open(method.Docs);
            return;
        }
        
    }

}

function GetBillState(BillState) {
    if (BillState == 0)
        return "空白";

    if (BillState == 1)
        return "草稿";

    if (BillState == 2)
        return "编辑中";

    if (BillState == 100)
        return "归档";

    return BillState;
}

function GetDDLText(field, val, uibindKey, data) {
    //获取这个字段对应的值
    if (uibindKey == null || uibindKey == undefined || uibindKey == "")
        return "";
    var options = data[uibindKey];
    if (options == null || options == undefined) {
        var enums = data.Sys_Enum;
        if (enums.length > 0) {
            var text = "";
            $.each(enums, function (i, item) {
                if (item.EnumKey == uibindKey && item.IntKey == val) {
                    text = item.Lab;
                    return false;
                }
            })
            if (text != "")
                return text;
        }
        return "";
    }

    var item = $.grep(options, function (option) {
        return option.No == val;
    });
    if (item.length == 0)
        return "";
    return item[0].Name;
}
function transferHtmlData(tableData) {
    var val = "";
    if (richAttrs.length != 0) {
        $.each(tableData, function (i, item) {
            richAttrs.forEach(attr => {
                val = item[attr.KeyOfEn];
                if (val != "") {

                    val = htmlEncodeByRegExp(val);
                    val = val.replace(/<[^>]+>/g, "")
                    item[attr.KeyOfEn] = val;
                }
            })
        });
    }
    return tableData;
}

function htmlEncodeByRegExp(str) {
    var s = '';
    if (str==null || str==undefined ||str.length === 0) {
        return '';
    }
    s = str.replace(/&/g, '&amp;');
    s = s.replace(/</g, '&lt;');
    s = s.replace(/>/g, '&gt;');
    s = s.replace(/ /g, '&nbsp;');
    s = s.replace(/\'/g, '&#39;');
    s = s.replace(/\"/g, '&quot;');
    return s;
}

function htmlDecodeByRegExp(str) {
    var s = '';
    if (str.length === 0) {
        return '';
    }
    s = str.replace(/&amp;/g, '&');
    s = s.replace(/&lt;/g, '<');
    s = s.replace(/&gt;/g, '>');
    s = s.replace(/&nbsp;/g, ' ');
    s = s.replace(/&#39;/g, '\'');
    s = s.replace(/&quot;/g, '\"');
    return s;
}