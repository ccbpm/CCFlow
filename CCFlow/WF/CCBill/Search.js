/**
* 配置下拉框数据
* @param frmData
* @param mapAttr
* @param defVal
*/
function InitDDLOperation(frmData, mapAttr, defVal) {
    var operations = [];
    operations.push({
        name: "全部",
        value: "all"
    });
    var ens = frmData[mapAttr.Field];
    if (ens == null) {
        operations.push({
            name: "否",
            value: "0"
        });
        operations.push({
            name: "是",
            value: "1"
        });
    }
    ens.forEach(function (en) {
        if (en.No == undefined)
            if (en.IntKey == undefined) {
                operations.push({
                    name: en.Name,
                    value: en.BH,
                    selected: en.BH == defVal ? true : false
                });
            } else {
                operations.push({
                    name: en.Lab,
                    value: en.IntKey,
                    selected: en.IntKey == defVal ? true : false
                });
            }
           
        else
            operations.push({
                name: en.Name,
                value: en.No,
                selected: en.No == defVal ? true : false
            });
    })
    return operations;
}
/**
 * 获取查询table的列集合处理
 * @param {any} thrMultiTitle 三级表头的内容
 * @param {any} secMultiTitle 二级表头的内容
 * @param {any} ColorSet 列字段颜色显示
 * @param {any} attrs 显示列的集合
 * @param {any} sortColumns 排序的字段
 */

//判断是否是多级表头
var isThrHeader = false; //是否是三级表头
var isSecHeader = false;//是否是二级表头
function GetColoums(thrMultiTitle, secMultiTitle, colorSet, sortColumns, openModel, openTitle, entityType) {
    var handler = new HttpHandler("BP.CCBill.WF_CCBill");
    handler.AddPara("FrmID", frmID);
    var data = handler.DoMethodReturnString("Search_MapAttr");
    if (data.indexOf("err@") != -1) {
        layer.alert(data);
        return;
    }
    var attrs = JSON.parse(data);

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

    var fieldColumns = {
        type: 'checkbox',
        rowspan: isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1

    };
    AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1);
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
        var sortable = true;
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
                    return "<a href=\"javascript:OpenIt('" + row.OID + "','" + frmID + "'," + openModel + ",'" + openTitle + "'," + entityType + ")\"><img src=" + icon + " border=0 width='14px;' />" + row[this.field] + "</a>";
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
                style: {
                    css: { "white-space": "nowrap", "word-break": "keep-all", "width": "100%" }
                },
                templet: function (row) {
                    var val = row[this.field + "Text"];
                    if (val == undefined || val == null)
                        val = row[this.field + "T"];

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
                    if (row[this.field] == "0") return "否";
                    if (row[this.field] == "1") return "是";
                }
            };
            AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, keyRowSpan);
            continue;
        }


        if (width == null || width == "" || width == undefined)
            width = 100;
        fieldColumns = {
            field: field,
            title: title,
            minWidth: attr.Width,
            fixed: false,
            sort: sortable,
            rowspan: keyRowSpan,

            templet: function (row) {
                var fieldColor = [];
                if (colorSet.indexOf("@" + this.field + ":") != -1) {
                    fieldColor = getFieldColor(colorSet, this.field);
                }
                if (fieldColor.length == 0)
                    return row[this.field];
                for (var i = 0; i < fieldColor.length; i++) {
                    var color = fieldColor[i];
                    if (color.From <= row[this.field] && color.To >= row[this.field]) {
                        var stylecss = "height: 22px;line-height: 22px;padding: 0 5px;font-size: 12px;color: #fff;white-space: nowrap;border-radius: 2px;text-align:center;";
                        return '<div style="' + stylecss + 'background-color:' + color.Color + ';">' + row[this.field] + '</div>'
                    }
                }
            }

        };
        AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, keyRowSpan);
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
function SearchData() {
    var handler = new HttpHandler("BP.CCBill.WF_CCBill");
    handler.AddUrlData()
    handler.AddPara("PageIdx", pageIdx);
    handler.AddPara("PageSize", pageSize);

    if (orderBy != null && orderBy != undefined)
        ur.OrderBy = orderBy;
    if (orderWay != null && orderWay != undefined)
        ur.OrderWay = orderWay;
    ur.Update();

    //查询集合
    var data = handler.DoMethodReturnString("Search_Init");
    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    data = JSON.parse(data);

    ur = new Entity("BP.Sys.UserRegedit");
    ur.MyPK = webUser.No + frmID + "_SearchAttrs";
    ur.RetrieveFromDBSources();

    return data["DT"];
}
/**
 * 打开新页面的方式
 * @param {any} workid 实体的WorkID
 * @param {any} frmID 实体的表单ID
 * @param {any} openModel 打开方式 //0=新窗口打开 1=在本窗口打开 2=弹出窗口打开,关闭后不刷新列表 3=弹出窗口打开,关闭刷新
 * @param {any} title 标题
 */
function OpenIt(workid, frmID, openModel, title, entityType) {


    var url = "";
    url = "MyDictFrameWork.htm?WorkID=" + workid + "&FrmID=" + frmID + "&FK_MapData=" + frmID;
    if (entityType == 1)
        url = "MyBill.htm?WorkID=" + workid + "&FrmID=" + frmID + "&FK_MapData=" + frmID;

    /* if (window.parent && window.parent.location.href != window.location.href) {
         url = "../CCBill/" + url;
         window.top.vm.openTab(title, url);
     } else {*/

    // OpenLayuiDialog(url, "", window.innerWidth / 2, false, false, true);
    OpenLayuiDialog(url, "", 90000, 0, null, false);

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
    $("select[name^='DDL_']").each(function () {
        var id = $(this).attr("id");
        id = id.replace("DDL_", "");
        str += "@" + id + "=" + $(this).val();
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
    layui.table.reload('lay_table_dict', { data: tableData });
    renderLaypage();
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
                "From": parseInt(ts[0].replace("From=", "")),
                "To": parseInt(ts[1].replace("To=", "")),
                "Color": ts[2].replace("Color=", "")
            });
        }



    }
    return fieldColor;
}