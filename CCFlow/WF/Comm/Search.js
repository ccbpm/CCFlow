/******************************************工具栏查询条件、操作按钮的解析**********************************************************/
//初始化数据.
function InitToolbar() {

    //创建处理器.
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Comm");
    handler.AddUrlData();  //增加参数.
    //获得map基本信息.
    mapBase = handler.DoMethodReturnJSON("Search_MapBaseInfo");

    document.title = mapBase.EnDesc;

    //获得查询信息，包含了查询数据表.
    var data = handler.DoMethodReturnJSON("Search_SearchAttrs");
    var IsTableShow = false;
    //绑定外键枚举查询条件.
    var attrs = data["Attrs"];

    //search通用的查询条件，关键字查询、字符串查询、数值型查询、外键/枚举的查询
    //计算查询条件的数量

    //字符串的查询
    var searchFields = mapBase.SearchFields;
    if (searchFields == null || searchFields == "" || searchFields == undefined)
        searchFields = "";
    var stringFields = searchFields == "" ? [] : searchFields.split("@");

    //数字型的查询
    var searchFieldsOfNum = mapBase.SearchFieldsOfNum;
    if (searchFieldsOfNum == null || searchFieldsOfNum == "" || searchFieldsOfNum == undefined)
        searchFieldsOfNum = "";
    var intFields = searchFieldsOfNum == "" ? [] : searchFieldsOfNum.split("@");

    //查询条件的数据合
    var len = 0;
    var _html = "<div class='layui-form-item'>";
    //关键字的查询
    if (stringFields.length == 0 && mapBase.IsShowSearchKey == "1") {
        var keyLabel = cfg.KeyLabel;
        if (keyLabel == null || keyLabel == undefined || keyLabel == "")
            keyLabel = "关键字";
        var keyPlaceholder = cfg.KeyPlaceholder;
        if (keyPlaceholder == null || keyPlaceholder == undefined)
            keyPlaceholder = "";
        _html += GetInputHtml(keyLabel, "Key", ur.SearchKey, keyPlaceholder);
        len = len + 1;
    }
    if (stringFields.length != 0) {
        $.each(stringFields, function (i, field) {
            if (field == "")
                return true;
            var strs = field.split("=");
            if (strs.length < 2 || strs[0] == "" || strs[1] == "")
                return true;

            if (len != 0 && len % 4 == 0) {
                _html += "</div>";
                _html += "<div class='layui-form-item'>";
            }
            //存入查询条件
            fields.push(strs[1]);
            //获取查询的历史数据
            fieldV = ur.GetPara(strs[1]);
            if (fieldV == null || fieldV == undefined)
                fieldV = "";
            //增加字符串的查询
            _html += GetInputHtml(strs[0], strs[1], fieldV);
            len++;
        })
    }
    //数字型的查询
    if (intFields.length != 0) {
        $.each(intFields, function (i, field) {
            if (field == "")
                return true;
            var strs = field.split("=");
            if (strs.length < 2 || strs[0] == "" || strs[1] == "")
                return true;

            if (len != 0 && len % 4 == 0) {
                _html += "</div>";
                _html += "<div class='layui-form-item'>";
            }
            //存入查询条件
            fields.push(strs[1]);
            //获取查询的历史数据
            fieldV = ur.GetPara(strs[1]);
            if (fieldV == null || fieldV == undefined)
                fieldV = "";
            fieldV = ur.GetPara(strs[1]);
            if (fieldV == null || fieldV == undefined || fieldV == "") {
                val1 = "";
                val2 = "";
            } else {
                val1 = fieldV.split(',')[0];
                val2 = fieldV.split(',')[1];
            }
            //增加字符串的查询
            _html += GetInputPartHtml(strs[0], strs[1], val1, val2);
            len++;
        })
    }
    //时间的查询条件
    if (mapBase.DTSearchWay != 0) {
        if (len != 0 && len % 4 == 0) {
            _html += "</div>";
            _html += "<div class='layui-form-item'>";
        }
        var dataInfo = "yyyy-MM-dd";
        if (mapBase.DTSearchWay != 1)
            dataInfo = "yyyy-MM-dd HH:mm";
        _html += GetDateInputPart(mapBase.DTSearchLable, ur.DTFrom, ur.DTTo, dataInfo);
        len = len + 1;
    }

    //增加下拉、枚举的
    if (attrs.length > 0) {
        var json = AtParaToJson(ur.Vals);
        $.each(attrs, function (i, attr) {
            if (len != 0 && len % 4 == 0) {
                _html += "</div>";
                _html += "<div class='layui-form-item'>";
            }
            _html += GetDDLHtml(attr.Name, attr.Field, data, attr);
            len = len + 1;
        })
    }
    //增加按钮的操作
    if (len != 0 && len % 4 == 0) {
        _html += "</div>";
        _html += "<div class='layui-form-item'>";
    }
    _html += "<div class='layui-inline'>";
    //查询
    _html += "<button type='button' class='layui-btn layui-btn-radius  layui-btn-sm toolbar' id='Search'>查询</button>";

    //自定义按钮
    var btnLab1 = cfg.BtnLab1;
    if (btnLab1 != null && btnLab1 != undefined && btnLab1 != "")
        _html += "<button type='button' class='layui-btn layui-btn-radius layui-btn-sm toolbar' id='BtnLab1'>" + btnLab1 + "</button>";
    var btnLab2 = cfg.BtnLab2;
    if (btnLab2 != null && btnLab2 != undefined && btnLab2 != "")
        _html += "<button  type='button' class='layui-btn layui-btn-radius layui-btn-sm toolbar' id='BtnLab2'>" + btnLab2 + "</button>";

    //新增
    if (mapBase.IsInsert.toString().toLowerCase() == "true")
        _html += "<button type='button' class='layui-btn layui-btn-radius layui-btn-sm toolbar' id='Add'>新建</button>";

    //导出
    if (mapBase.IsExp.toString().toLowerCase() == "true")
        _html += "<button type='button' class='layui-btn layui-btn-radius  layui-btn-sm toolbar' id='Exp'>导出</button>";

    //导入
    if (mapBase.IsImp.toString().toLowerCase() == "true")
        _html += "<button type='button' class='layui-btn layui-btn-radius layui-btn-sm toolbar' id='Imp'>导入</button>";
    //设置
    if (new WebUser().IsAdmin == true)
        _html += "<button type='button' class='layui-btn layui-btn-radius  layui-btn-sm toolbar' id='Setting'>设置</button>";
    _html += "</div>";
    _html += "</div>";

    $("#toolbar").append(_html);
    layui.form.render();
    //为查询外键赋值.
    for (var i = 0; i < attrs.length; i++) {
        var attr = attrs[i];
        var selectVal = json[attr.Field];
        if (GetQueryString(attr.Field) != null && GetQueryString(attr.Field) != undefined)
            selectVal = GetQueryString(attr.Field);

        if (selectVal == undefined || selectVal == "")
            selectVal = "all";
        //判断是否有级联关系
        var myPK = "ActiveDDL_" + ensName + "_" + attr.Field;
        var mapExt = new Entity("BP.Sys.MapExt");
        mapExt.SetPKVal(myPK);
        var isExist = mapExt.RetrieveFromDBSources();
        //处理级联关系
        if (isExist == 1) {
            var ddlPerant = $("#DDL_" + mapExt.AttrOfOper);
            var ddlChild = $("#DDL_" + mapExt.AttrsOfActive);
            if (ddlPerant != null && ddlChild != null) {
                ddlPerant.attr("onchange", "DDLRelation(this.value,\'" + "DDL_" + mapExt.AttrsOfActive + "\', \'" + mapExt.MyPK + "\',\'" + ddlPerant.val() + "\')");

            }
        }

        $("#DDL_" + attr.Field).val(selectVal);
    }
	$("#TB_Key").val(GetQueryString("Key"));
    layui.form.render();
}

function GetInputHtml(labName, key, val, keyPlaceholder) {
    var _html = "";
    _html += "<div class='layui-inline'>";
    _html += "<label class='layui-form-label'>" + labName + ":</label >"
    _html += "<div class='layui-input-inline'>";
    _html += "<input type='text' id='TB_" + key + "' class='layui-input' name='TB_" + key + "' value='" + val + "' placeholder='" + keyPlaceholder+"'>";
    _html += "</div>";
    _html += "</div>";
    return _html;
}
function GetInputPartHtml(labName, key, val1, val2) {
    var _html = "";
    _html += "<div class='layui-inline'>";
    _html += "<label class='layui-form-label'>" + labName + ":</label >"
    _html += "<div class='layui-input-inline'  style='width:100px'><input type='text' id='TB_" + key + "_0' class='layui-input' name='TB_" + key + "' value='" + val1 + "'/></div>";
    _html += "<div class='layui-input-inline'style='width:5px'>-</div>";
    _html += "<div class='layui-input-inline' style='width:100px'><input type='text' id='TB_" + key + "_1' class='layui-input' name='TB_" + key + "' value='" + val2 + "'/></div>";
    _html += "</div>";
    return _html;
}

function GetDateInputPart(labName, val1, val2, dateInfo) {
    var _html = "";
    _html += "<div class='layui-inline'>";
    _html += "<label class='layui-form-label'>" + labName + ":</label >"
    _html += "<div class='layui-input-inline' style='width:100px'><input type='text' id='TB_DTFrom' class='layui-input ccdate' name='TB_DTFrom'placeholder='开始时间' data-info='" + dateInfo + "' value='" + val1 + "'/></div>";
    _html += "<div class='layui-input-inline'style='width:5px'>-</div>";
    _html += "<div class='layui-input-inline' style='width:100px'><input type='text' id='TB_DTTo' class='layui-input ccdate' name='TB_DTTo' placeholder='结束时间' data-info='" + dateInfo + "' value='" + val2 + "'/></div>";
    _html += "</div>";
    return _html;
}

function GetDDLHtml(labName, key, formData, attr) {
    var _html = "";
    _html += "<div class='layui-inline'>";
    _html += "<label class='layui-form-label'>" + labName + ":</label >"
    _html += "<div class='layui-input-inline'>";
    _html += "<select  id='DDL_" + key + "'name='DDL_" + key + "'>";
    _html += InitDDLOperation(formData, attr, "");
    _html += "</select>";
    _html += "</div>";
    _html += "</div>";
    return _html;
}

//初始化下拉列表框的OPERATION
function InitDDLOperation(frmData, mapAttr, defVal) {

    var operations = "";
    operations += "<option value='all' >全部</option>";

    var ens = frmData[mapAttr.Field];
    if (ens == null) {
        ens = [{ 'IntKey': 0, 'Lab': '否' }, { 'IntKey': 1, 'Lab': '是' }];
    }
    for (var i = 0; i < ens.length; i++) {

        var en = ens[i];

        if (en.No == undefined)
            operations += "<option value='" + en.IntKey + "'>" + en.Lab + "</option>";
        else
            operations += "<option value='" + en.No + "'>" + en.Name + "</option>";
    }
    return operations;
}
/******************************************工具栏查询条件、操作按钮的解析**********************************************************/

/******************************************获取查询列表字段的信息**********************************************************/
/**
 * 获取查询table的列集合处理
 * @param {any} data 数据集合
 * @param {any} thrMultiTitle 三级表头的内容
 * @param {any} secMultiTitle 二级表头的内容
 * @param {any} ColorSet 列字段颜色显示
 * @param {any} attrs 显示列的集合
 * @param {any} sortColumns 排序的字段
 * * @param {any} focusField 焦点字段，超链接显示
 * * @param {any} isBatch 是否是批处理
 */


//判断是否是多级表头
var isThrHeader = false; //是否是三级表头
var isSecHeader = false;//是否是二级表头
var richAttrs = [];
var dtMs = null;
function GetColoums(data, thrMultiTitle, secMultiTitle, colorSet, sortColumns, focusField, isBatch) {

    var hideAttrs = cfield.Attrs;
    var attrs = data.Attrs;
    dtMs = data["dtM"];

    var sysMapData = data["Sys_MapData"][0];
    sysMapData = new Entity("BP.Sys.MapData", sysMapData); //把他转化成entity.
    enPK = sysMapData.GetPara("PK");

    if (attrs == undefined) {
        alert('没有取得属性.');
        return;
    }

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


    if (isBatch && isBatch == true) {
        fieldColumns = {
            type: 'checkbox',
            field: '',
            align: 'center',
            width: 50,
            rowspan: isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1,

        };
    } else {
        fieldColumns = {
            title: `序<i class="layui-icon analyseCols">&#xe625;</i>`,
            field: '',
            align: 'center',
            width: 80,
            rowspan: isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1,
            
            templet: function (d) {
                return pageSize * (pageIdx - 1) + d.LAY_TABLE_INDEX + 1;    // 返回每条的序号： 每页条数 *（当前页 - 1 ）+ 序号

            }
        };
    }
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
        var sortable = false;
        if (sortColumns != null && sortColumns != "")
            sortable = sortColumns.indexOf(field) != -1 ? true : false;
        if (width < 60) {
            width = 60;
        }
        if (field == "Title") {
            width = 230;
        }

        if (attr.UIContralType == 1) {
            if (width == null || width == "" || width == undefined)
                width = 180;
            field = field + "Text";
            fieldColumns = {
                field: field,
                title: title,
                fixed: false,
                minWidth: width,
                sort: sortable,
                hide: hideAttrs.indexOf(","+field+",")!=-1,
                rowspan: keyRowSpan

            };
            AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, keyRowSpan);
            continue;
        }

        if (attr.UIContralType == 2) {
            fieldColumns = {
                field: field,
                title: title,
                minWidth: width,
                fixed: false,
                sort: sortable,
                rowspan: keyRowSpan,
                hide: hideAttrs.indexOf(","+field + ",") != -1,
                templet: function (row) {
                    var val = "";
                    if (row[this.field] == "0")
                        val = "否";
                    if (row[this.field] == "1")
                        val = "是";
                    return val;

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
                hide: hideAttrs.indexOf("," + field + ",") != -1,
                rowspan: keyRowSpan,
                templet: function (row) {
                    var val = row[this.field];
                    if (val == "")
                        return val;
                    val = htmlDecodeByRegExp(val);
                    return "<div>" + val + "</div>";

                }
            };
            AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, keyRowSpan);
            continue;
        }

        if (attr.MyDataType == "6") {
            fieldColumns = {
                field: field,
                title: title,
                width: width,
                fixed: false,
                sort: sortable,
                rowspan: keyRowSpan,
                hide: hideAttrs.indexOf("," + field + ",") != -1,
                templet: function (row) {
                    var val = row[this.field];
                    if (val == null || val == undefined || val == "")
                        return "";
                    val = FormatDate(new Date(val), "yyyy-MM-dd");
                    return val;

                }
            };
            AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, keyRowSpan);
            continue;
        }


        fieldColumns = {
            field: field,
            title: title,
            minWidth: width,
            fixed: false,
            sortable: true,
            sort: sortable,
            hide: hideAttrs.indexOf("," + field + ",") != -1,
            align: field.toLowerCase() == "icon" ? "center" : "left",
            templet: function (row) {
                //如果这个字段是Icon
                if (this.field.toLowerCase() == "icon" && row[this.field] != "") {
                    return "<i  class='" + row[this.field] + "'></i>";
                }
                if (this.field == focusField) {
                    var pkval = row[enPK];
                    var paras = "&" + enPK + "=" + pkval;
                    for (var i = 0; i < attrs.length; i++) {
                        var attr = attrs[i];
                        if (attr.UIContralType == 1)
                            paras += "&" + attr.KeyOfEn + "=" + row[attr.KeyOfEn];
                    }
                    var rowstr = JSON.stringify(row);
                    rowstr = encodeURIComponent(rowstr);
                    return "<a href='#' style='color:#3959c1' onclick='OpenEn(\"" + pkval + "\",\"" + paras + "\",1,\"" + rowstr + "\")'>" + row[this.field] + "</a>";
                }
                var fieldColor = [];
                if (colorSet.indexOf("@" + this.field + ":") != -1) {
                    fieldColor = getFieldColor(colorSet, this.field);
                }
                if (fieldColor.length == 0)
                    return row[this.field];
                for (var i = 0; i < fieldColor.length; i++) {
                    var color = fieldColor[i];
                    if (color.From <= row[this.field] && color.To >= row[this.field])
                        return "<div style='width:20px;height:20px;text-align:center;background:" + color.Color + "'>" + row[this.field] + "</div>";
                }
            }

        };
        AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, keyRowSpan);
    }
    if (thrcolspan.field != undefined)
        threeColumns.push(thrcolspan);
    //获取屏幕的长宽
    var W = document.body.clientWidth - 40;
    var H = document.body.clientHeight - 40;
    var entityAthType = sysMapData.GetPara("BPEntityAthType");
    if (dtMs.length > 0 || entityAthType == 1 || entityAthType == 2) {
        fieldColumns = {
            field: '_operate',
            title: '操作',
            minWidth: 200,
            align: 'center',
            rowspan: isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1,
            templet: function (row) {
                return rowbar(dtMs, row);
            }
        }
        AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1);
    }

    var isEnableOpen = cfg.GetPara("IsEnableOpenICON");
    //添加显示详情列
    if (isEnableOpen == "1") {
        //加入操作下载文件
        fieldColumns = {
            field: 'viewDetail',
            title: '详细信息',
            minWidth: 80,
            align: 'center',
            rowspan: isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1,
            templet: function (row) {
                var pkval = row[enPK];
                var paras = "&" + enPK + "=" + pkval;
                for (var i = 0; i < attrs.length; i++) {
                    var attr = attrs[i];
                    if (attr.UIContralType == 1)
                        paras += "&" + attr.KeyOfEn + "=" + row[attr.KeyOfEn];
                }
                var rowstr = JSON.stringify(row);
                rowstr = encodeURIComponent(rowstr);
                return "<a href='#' onclick='OpenEn(\"" + pkval + "\",\"" + paras + "\",1,\"" + rowstr + "\")'>详情</a>";
            }
        }
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
 * 解析行事件
 * @param {any} dtMs
 * @param {any} entityAthType
 */
function rowbar(dtMs, row) {
    var _html = '';
    $.each(dtMs, function (idx, dtM) {
        //根据行中的数据显示
        var isShowForEnsCondtion = dtM.IsShowForEnsCondtion;
        if (isShowForEnsCondtion != null && isShowForEnsCondtion != "") {
            var strs = isShowForEnsCondtion.split("@");
            var isShow = true;
            $.each(strs, function (i, obj) {
                var items = obj.split("=");
                if (row[items[0]] != items[1]) {
                    isShow = false;
                    return true;
                }
            });
            if (isShow == false)
                return true;
        }
        _html += '<a class="layui-btn layui-btn-xs" lay-event="' + dtM.Title + '">' + dtM.Title + '</a>';
    });
    if (row.MyFileName != null && row.MyFileName!=undefined && row.MyFileName != "")
        _html += '<a class="layui-btn layui-btn-xs" lay-event="downloadFile">下载</a>';
    if (_html == "") _html = "-";
  return _html;
}
/**
 * 处理行操作
 * @param {any} dtm
 * @param {any} row
 */
function DealRowBarOper(dtm, row) {
    var pkval = row[enPK];
    var warning = dtm.Warning;
    var refMethodType = parseInt(dtm.RefMethodType);
    var url = dtm.Url;
    url = url.replace("@WorkID", pkval);
    url = url.replace("@OID", pkval);
    url = url.replace("@PKVal", pkval);
    url = dtm.Url + pkval;
    //获取屏幕的宽
    var W = document.body.clientWidth - 40;
    switch (refMethodType) {
        case RefMethodType.LinkeWinOpen:
            WinOpenIt(url, dtm.Title);
            break;
        case RefMethodType.LinkModel:
        case RefMethodType.RightFrameOpen:
           OpenLayuiDialog(url,dtm.Title ,W ,100,"r", false);
            break;
        case RefMethodType.Func:
            if (dtm.FunPara == true || dtm.FunPara == "true")
                OpenLayuiDialog(url, dtm.Title,W,100,"r", false);
            else {
                if (warning == null || warning == "null" || warning == "") {
                    warning = "确定要执行吗？";
                }
                else {
                    warning = warning.replace(/,\s+/g, ",");
                    warning = warning.replace(/\s+/g, "\r\n");
                }
                openFun(warning,url, dtm.Title);

            }

            break;
        case RefMethodType.TabOpen:
            if (window.top && window.top.vm) {
                window.top.vm.openTab(dtm.Title, url);
                break;
            }
            OpenLayuiDialog(url, dtm.Title, W, 100, "r", false);
            break;
        default: break;
    }
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
function GetAttrKeyRowSpan(keyOfEn) {
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
function AddSecOrThrColumn(keyOfEn, keyRowSpan, secondColumns, threeColumns) {
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

/******************************************获取查询列表字段的信息**********************************************************/
//执行查询.
function Search() {

    //保存查询条件.
    var ensName = GetQueryString("EnsName");
    ur = GetUserRegedit();
    if ($("#TB_Key") != null && $("#TB_Key").val() != "")
        ur.SearchKey = $("#TB_Key").val();
    else
        ur.SearchKey = "";

    //增加字段查询
    var val = "";
    for (var i = 0; i < fields.length; i++) {
        var field = fields[i];
        var strs = $("input[name='TB_" + field + "']");
        if (strs.length == 1) {
            ur.SetPara(field, $("#TB_" + field).val());
        } else {
            if ($("#TB_" + field + "_0").val() == "" && $("#TB_" + field + "_1").val() == "")
                ur.SetPara(field, "");
            else
                ur.SetPara(field, $("#TB_" + field + "_0").val() + "," + $("#TB_" + field + "_1").val());
        }


    }

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

    ur.FK_Emp = webUser.No;
    ur.CfgKey = "SearchAttrs";
    ur.Vals = str;
    ur.FK_MapData = ensName;
    ur.SetPara("RecCount", count);
    var i = ur.Update();
    pageIdx = 1;
    var tableData = SearchData();
    //分页
    layui.table.reload('tableSearch', { data: tableData["DT"] });
    renderLaypage();
}


function SearchData(orderBy, orderWay) {
    //ur = GetUserRegedit()
    //创建处理器.
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Comm");
    handler.AddUrlData()
    handler.AddPara("PageIdx", pageIdx);
    handler.AddPara("PageSize", pageSize);
    if (orderBy != null && orderBy != undefined)
        ur.OrderBy = orderBy;
    if (orderWay != null && orderWay != undefined)
        ur.OrderWay = orderWay;
    ur.Update();
    //查询集合
    var data = handler.DoMethodReturnString("Search_SearchIt");
    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }


    data = JSON.parse(data);


    //当前用户页面信息.

    ur.MyPK = webUser.No + "_" + ensName + "_SearchAttrs";
    ur.SetPKVal(ur.MyPK);
    ur.RetrieveFromDBSources();
    count = ur.GetPara("RecCount");
    if (count % pageSize != 0)
        pages = parseInt(count / pageSize) + 1;
    else
        pages = parseInt(count / pageSize);

    if (pages == 0) pages = 1;



    //设置查询总数居的合计、平均等信息
    var heji = data["Search_HeJi"];
    var _html = "";
    if (heji != null && heji != undefined) {
        $.each(heji, function (i, item) {
            var val = item.Value;
            if (val == null || val == "" || val == undefined)
                val = 0;
            if (item.Type == "Sum")
                _html += item.Field + "总合计：" + val + " ";
            if (item.Type == "Avg")
                _html += item.Field + "总平均：" + val + " ";
        });
    }


    $("#JsResult").html("").html(_html);
    return data;
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
    if (str.length === 0) {
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



function OpenEn(pk, paras, flag, row) {

    if (row != null && row != undefined && row != "")
        row = JSON.parse(decodeURIComponent(row));

    var str = GetHrefUrl();
    //当前URL参数
    var currUrl = str.substring(str.indexOf('?') + 1); // window.location.search.substring(1);

    //去除URL中的参数 EnsName、 PKVal
    var currUrls = currUrl.split("&");
    currUrl = "";
    $.each(currUrls, function (i, param) {
        if (param.indexOf("EnsName") >= 0 || param.indexOf("PKVal") >= 0)
            return true;

        currUrl += "&" + param;
    });

    var ensName = GetQueryString("EnsName");
    var enName = ensName.substring(0, ensName.length - 1);
    //考虑兼容旧版本.
    var url = cfg.GetPara("WinOpenUrl");
    if (url && url.length > 4) {
        cfg.Url = url;
        cfg.Update();
    }

    url = cfg.UrlExt;
    var urlOpenType = cfg.SearchUrlOpenType;

    if (urlOpenType == 0 || urlOpenType == undefined)
        url = "./RefFunc/En.htm?EnName=" + mapBase.EnName + "&PKVal=" + pk + currUrl;

    if (urlOpenType == 1)
        url = "./RefFunc/EnOnly.htm?EnName=" + mapBase.EnName + "&PKVal=" + pk + currUrl;

    if (urlOpenType == 2)
        url = "../CCForm/FrmGener.htm?FK_MapData=" + GetQueryString("EnsName") + "&PKVal=" + pk + currUrl;

    if (urlOpenType == 3)
        url = "../CCForm/FrmGener.htm?FK_MapData=" + GetQueryString("EnsName") + "&PKVal=" + pk + currUrl;

    if (urlOpenType == 9) {
        if (url.indexOf('?') == -1)
            url = url + "?1=1";
        if (url.indexOf('FrmID') != -1)
            url = url + "&WorkID=" + pk + "&OID=" + pk;
    }

    //url中包含@符号需要进行替换
    if (url.indexOf("@") != -1) {
        url = DealJsonExp(row, url);
    }

    var winCardW = cfg.WinCardW;
    var windowW = document.body.clientWidth;
    if (winCardW == 0)
        windowW = windowW * 3 / 4;
    if (winCardW == 1)
        windowW = windowW * 1 / 2;
    if (winCardW == 3)
        windowW = windowW * 1 / 4;


    var openModel = cfg.OpenModel;

    if (openModel == 0) {
        if (cfg.IsRefreshParentPage == 1)
            OpenLayuiDialog(url, mapBase.EnDesc + ' : 详细', windowW, 100, "r", true);
        else
            OpenLayuiDialog(url, mapBase.EnDesc + ' : 详细', windowW, 100, "r", false);
    } else {
        var windowObj = window.open(url);
        if (flag == 0 || cfg.IsRefreshParentPage == 1) {
            var loop = setInterval(function () {
                if (windowObj.closed) {
                    clearInterval(loop);
                    SearchData();
                    Paginator(pages);

                }

            }, 1000);
        }
    }


}


function New() {
    OpenEn("", "", 0, null);
}

function Exp() {

    if (plant != 'CCFlow')
        downLoadExcel(basePath + "/WF/Export/Search_Exp?type=search&EnsName=" + GetQueryString("EnsName"));
    else {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Comm");
        handler.AddPara("EnsName", GetQueryString("EnsName"));
        //查询集合
        var data = handler.DoMethodReturnString("Search_Exp");
        var url = "";
        if (data.indexOf('err@') == 0) {
            alert(data);
            return;
        }
        data = basePath + data;
        window.open(data);
    }
}

///导入.
function Imp() {

    var ensName = GetQueryString("EnsName");
    var en = new Entity("BP.Sys.EnCfg", ensName);
    var url = "./Sys/ImpData.htm?EnsName=" + GetQueryString("EnsName") + "&m=" + Math.random();
    if (en.ImpFuncUrl != '')
        url = en.ImpFuncUrl;

    //获取屏幕的长宽
    var W = document.body.clientWidth - 80;
    OpenLayuiDialog(url, '导入数据', W, 100, "r", true);
}

//设置
function Setting() {
    //先判断是否有该笔数据.
    var ensName = GetQueryString("EnsName");
    var en = new Entity("BP.Sys.EnCfg");
    en.SetPKVal(ensName);
    var i = en.RetrieveFromDBSources();
    if (i == 0) {
        en.No = ensName;
        en.Insert();
    }
    var url = "./RefFunc/En.htm?EnName=BP.Sys.EnCfg&No=" + GetQueryString("EnsName") + "&m=" + Math.random();
    //获取屏幕的长宽
    var W = document.body.clientWidth - 340;
    OpenLayuiDialog(url, "", W, 100, "r", true);
}

function openFun(warning, url, title) {
    var W = document.body.clientWidth - 40;
    if (confirm(warning)) {
        OpenLayuiDialog(url, title, W, 100, "r", false);
    }
}


function downLoadFile(PKVal) {
    if (plant == "CCFlow")
        SetHref('../CCForm/DownFile.aspx?DoType=EntityFile_Load&DelPKVal=' + PKVal + '&EnsName=' + GetQueryString("EnsName"));
    else {
        var currentPath = GetHrefUrl();
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        Url = path + '/WF/Ath/EntityFileLoad.do?DelPKVal=' + PKVal + '&EnsName=' + GetQueryString("EnsName");
        SetHref(url);
    }
}
//执行方法
function executeFunction(jsString, label) {
    jsString = jsString.replace(/~/g, "'");
    if (jsString.indexOf("/") != -1) {
        var W = document.body.clientWidth - 40;
        OpenLayuiDialog(jsString, label, W, 100, "r", false);
    } else {
        if (jsString.indexOf('(') == -1)
            cceval(jsString + "()");
        else
            cceval(jsString);
    }
}

/* 级联下拉框  param 传到后台的一些参数  例如从表的行数据 主表的字段值 如果param参数在，就不去页面中取KVS 了，PARAM 就是*/
function DDLRelation(selectVal, ddlChild, fk_mapExt, param) {
    if (selectVal == "all") {
        $("#" + ddlChild).empty();
        //无数据返回时，提示显示无数据，并将与此关联的下级下拉框也处理一遍，edited by liuxc,2015-10-22
        $("#" + ddlChild).append("<option value='all' selected='selected' >全部</option");
        var chg = $("#" + ddlChild).attr("onchange");

        $("#" + ddlChild).change();

        return;
    }

    var mapExt = new Entity("BP.Sys.MapExt", fk_mapExt);

    //处理参数问题
    if (param != undefined) {
        kvs = '';
    }
    var dbSrc = mapExt.Doc;
    if (param != undefined) {
        for (var pro in param) {
            if (pro == 'DoType')
                continue;
            dbSrc = dbSrc.replace("@" + pro, param[pro]);
        }
    }

    var dataObj = GenerDB(dbSrc, selectVal, mapExt.DBType); //获得数据源.

    // 这里要设置一下获取的外部数据.
    // 获取原来选择值.
    var oldVal = null;
    var ddl = document.getElementById(ddlChild);

    if (ddl == null) {
        alert(ddlChild + "丢失,或者该字段被删除.");
        return;
    }

    var mylen = ddl.options.length - 1;
    while (mylen >= 0) {
        if (ddl.options[mylen].selected) {
            oldVal = ddl.options[mylen].value;
        }
        mylen--;
    }

    //清空级联字段
    $("#" + ddlChild).empty();

    //查询数据为空时为级联字段赋值
    if (dataObj == null || dataObj.length == 0) {
        //无数据返回时，提示显示无数据，并将与此关联的下级下拉框也处理一遍，edited by liuxc,2015-10-22
        $("#" + ddlChild).append("<option value='' selected='selected' >无数据</option");
        var chg = $("#" + ddlChild).attr("onchange");

        if (typeof chg == "function") {
            $("#" + ddlChild).change();
        }
        return;
    }

    //不为空的时候赋值
    $("#" + ddlChild).append("<option value='all' >全部</option>");
    $.each(dataObj, function (idx, item) {
        var no = item.No;
        if (no == undefined)
            no = item.NO;

        var name = item.Name;
        if (name == undefined)
            name = item.NAME;

        $("#" + ddlChild).append("<option value='" + no + "'>" + name + "</option");

    });

    var isInIt = false;
    mylen = ddl.options.length - 1;
    while (mylen >= 0) {
        if (ddl.options[mylen].value == oldVal) {
            ddl.options[mylen].selected = true;
            isInIt = true;
            break;
        }
        mylen--;
    }
    if (isInIt == false) {
        //此处修改，去掉直接选中上次的结果，避免错误数据的产生，edited by liuxc,2015-10-22
        //$("#" + ddlChild).prepend("<option value='' selected='selected' >*请选择</option");
        $("#" + ddlChild).val('');
        //增加默认选择第一条数据
        ddl.options[0].selected = true;
        var chg = $("#" + ddlChild).attr("onchange");
        $("#" + ddlChild).change();

    }
}
//打开窗口
function WinOpenIt(url, winName) {
    var newWindow = window.open(url, winName, 'height=1200,width=1000,top=' + (window.screen.availHeight - 800) / 2 + ',left=' + (window.screen.availWidth - 1030) / 2 + ',scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
    newWindow.focus();
    return;
}

