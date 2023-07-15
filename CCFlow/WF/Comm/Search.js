/******************************************工具栏查询条件、操作按钮的解析**********************************************************/
//初始化数据.
var treeKey = [];
var dllKey = [];
var xmlSelectKey = [];
var cbKey = [];
function InitToolbar(type) {

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

    //扩展属性
    var mapExts = data["Sys_MapExt"];
    mapExts = mapExts || [];

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
        if (mapBase.DTSearchWay == 2)
            dataInfo = "yyyy-MM-dd HH:mm";
        if (mapBase.DTSearchWay == 3)
            dataInfo = "yyyy-MM";
        if (mapBase.DTSearchWay == 4)
            dataInfo = "yyyy";

        _html += GetDateInputPart(mapBase.DTSearchWay, mapBase.DTSearchLabel, ur.DTFrom, ur.DTTo, dataInfo);
        len = len + 1;
    }
    var json = AtParaToJson(ur.Vals);
	var isSelectMore = cfg.IsSelectMore;
    //增加下拉、枚举的
    if (attrs.length > 0) {
        $.each(attrs, function (i, attr) {
            if (len != 0 && len % 4 == 0) {
                _html += "</div>";
                _html += "<div class='layui-form-item'>";
            }
            //复选框
            if (attr.UIContralType == 2) {
                var selectVal = json[attr.Field]||"0";
                _html += GetCheckBoxHtml(attr.Name, attr.Field, selectVal);
                len = len + 1;
                cbKey.push(attr.Field);
                return true;
            }
            if (attr.IsTree == "0") {
				 if (isSelectMore == 0) {
                    dllKey.push(attr.Field);
                    _html += GetDDLHtml(attr.Name, attr.Field, data, attr);
                    len = len + 1;
                    return true;
                }
                if (mapExts.length == 0) { //解析成多选
                    xmlSelectKey.push(attr.Field);
                    _html += GetXmlSelectHtml(attr.Name, attr.Field);
                } else {
                    //判断是不是存在级联关系，存在selext下拉框
                    var items = $.grep(mapExts, function (obj) {
                        return obj.AttrOfOper == attr.Field || obj.AttrsOfActive == attr.Field;
                    })
                    if (items.length > 0) {
                        dllKey.push(attr.Field);
                        _html += GetDDLHtml(attr.Name, attr.Field, data, attr);
                    }
                    else {
                        xmlSelectKey.push(attr.Field);
                        _html += GetXmlSelectHtml(attr.Name, attr.Field);
                    }

                }

                len = len + 1;
                return true;
            }
            if (attr.IsTree == "1") {
                treeKey.push(attr.Field);
                _html += GetXmlSelectHtml(attr.Name, attr.Field);
                len = len + 1;
                return true;
            }
        })
    }
    //增加按钮的操作
    if (len != 0 && len % 4 == 0) {
        _html += "</div>";
        _html += "<div class='layui-form-item'>";
    }
    _html += "<div class='layui-inline layui-toolbar'>";
    if (type == "search" || type == "batch") {
        //查询
        _html += "<button type='button' class='layui-btn layui-btn-radius  layui-btn-sm toolbar' id='Search'>查询</button>";

        //自定义按钮
        var btnLab1 = cfg.BtnLab1;
        if (btnLab1 != null && btnLab1 != undefined && btnLab1 != "")
            _html += "<button type='button' class='layui-btn layui-btn-radius layui-btn-sm toolbar' id='BtnLab1'>" + btnLab1 + "</button>";
        var btnLab2 = cfg.BtnLab2;
        if (btnLab2 != null && btnLab2 != undefined && btnLab2 != "")
            _html += "<button  type='button' class='layui-btn layui-btn-radius layui-btn-sm toolbar' id='BtnLab2'>" + btnLab2 + "</button>";

        var btnLab3 = cfg.BtnLab3;
        if (btnLab3 != null && btnLab3 != undefined && btnLab3 != "")
            _html += "<button  type='button' class='layui-btn layui-btn-radius layui-btn-sm toolbar' id='BtnLab3'>" + btnLab3 + "</button>";
        //新增
        if (mapBase.IsInsert.toString().toLowerCase() == "true")
            _html += "<button type='button' class='layui-btn layui-btn-radius layui-btn-sm toolbar' id='Add'>新建</button>";
        if (type == "batch" && mapBase.IsDelete.toString().toLowerCase() == "true")
            _html += "<button type='button' class='layui-btn layui-btn-radius layui-btn-sm toolbar' id='Delete'>删除</button>";
        //导出
        if (cfg.IsExp == 1 || mapBase.IsExp.toString().toLowerCase() == "true")
            _html += "<button type='button' class='layui-btn layui-btn-radius  layui-btn-sm toolbar' id='Exp'>导出</button>";

        //导入
        if (mapBase.IsImp.toString().toLowerCase() == "true")
            _html += "<button type='button' class='layui-btn layui-btn-radius layui-btn-sm toolbar' id='Imp'>导入</button>";
        //设置
        if (new WebUser().IsAdmin == true)
            _html += "<button type='button' class='layui-btn layui-btn-radius  layui-btn-sm toolbar' id='Setting'>设置</button>";

    }
    if (type == "group") {
        //分析
        _html += "<button type='button' class='layui-btn layui-btn-radius  layui-btn-sm toolbar' id='Search'>分析</button>";
        //设置
        if (new WebUser().IsAdmin == true)
            _html += "<button type='button' class='layui-btn layui-btn-radius  layui-btn-sm toolbar' id='Setting'>设置</button>";

        //查询
        //_html += "<button type='button' class='layui-btn layui-btn-radius  layui-btn-sm toolbar' id='ToSearch'>查询</button>";

    }
    _html += "</div>";
    _html += "</div>";

    $("#toolbar").append(_html);
    layui.form.render();
    layui.dropdown.render({
        elem: '#Exp',
        trigger: 'click',
        data: [
            { title: '导出到Excel文件', id: "ExpExcel" },
            { title: '导出到Csv文件', id: "ExpCsv" }
        ],
        click: function (data, oThis) {
            if (data.id == "ExpExcel")
                Exp(0, type);
            if (data.id == "ExpCsv")
                Exp(1, type);
        }
    })

    $.each(xmlSelectKey, function (i, field) {
        var selectVal = json[field];
        var urlVal = GetQueryString(field);
        if (urlVal != null && urlVal != undefined)
            selectVal = GetQueryString(field);
        selectVal = selectVal || "all";
        selectVal = "," + selectVal + ",";
        var items = data[field];
        var selectData = [];
        $.each(items, function (idx, item) {
            if (typeof item.No == "undefined") {
                selectData.push({
                    name: item.Lab,
                    value: item.IntKey,
                    selected: selectVal.indexOf("," + item.IntKey + ",") != -1 ? true : false
                })
            }
            else {
                selectData.push({
                    name: item.Name,
                    value: item.No,
                    selected: selectVal.indexOf("," + item.No + ",") != -1 ? true : false
                })
            }
        })
        xmSelect.render({
            el: "#XmlSelect_" + field,
            autoRow: true,
            radio: false,
            clickClose: false,
            model: { label: { type: 'text' } },
            data: selectData,
            toolbar: {
                show: true,
				list: ['ALL', 'CLEAR', {
                    name: '确定',
                    icon: 'layui-icon layui-icon-ok',
                    key: "#XmlSelect_" + field,
                    method(event) {
                        //$("xm-select").css("border-color", "");
                        //$(".xm-icon-expand").removeClass("xm-icon-expand");
                        //$(".xm-body").addClass("dis");
                        $(this.key).find("xm-select").click();
                    }
                }]

            },
        });
    })
    //树形结构
    $.each(treeKey, function (i, field) {
        var parentNo = 1;
        if (field.toLowerCase().includes("dept"))
            parentNo = webUser.FK_Dept;
        if (webUser.CCBPMRunModel === 1)
            parentNo = webUser.OrgNo;
        var treeData = findChildren(data[field], parentNo);
        xmSelect.render({
            el: "#XmlSelect_" + field,
            autoRow: true,
            radio: true,
            clickClose: true,
            model: { label: { type: 'text' } },
            tree: {
                show: true,
                showLine: true,
                strict: false,
                clickCheck: false,
                expandedKeys: true
            },
            data: treeData,
            toolbar: {
                show: true,

            }
        });
    })

    //时间类型
    if ($(".ccdate").length > 0) {
        $.each($(".ccdate"), function (i, item) {
            var format = $(item).attr("data-info");
            var type = "date";
            if (format == "yyyy-MM-dd HH:mm")
                type = "datetime";
            if (format == "yyyy-MM")
                type = "month";
            if (format == "yyyy")
                type = "year";
            layui.laydate.render({
                elem: '#' + item.id, //指定元素
                format: format,
                type: type,
                done: function (value, date, endDate) {
                    //判断结束时间不能小于开始时间
                    if(value == "")
                        return;
                    //比对的时间字段值
                    var operVal = $('#TB_DTFrom').val();
                    var msg = "";
                    var searchLabel = '开始时间';
                    if (value < operVal && operVal != "") {
                        msg = "所选日期不能小于[" + searchLabel + "]对应的日期时间";
                    }

                    if (msg != "") {
                        layer.alert(msg);
                        value = "";
                    }

                    $(this.elem).val(value);
                }
            });
        })
    }
    //级联外键赋值.
    $.each(dllKey, function (i, field) {
        var selectVal = json[field];
        var urlVal = GetQueryString(field);
        if (urlVal != null && urlVal != undefined)
            selectVal = GetQueryString(field);
        selectVal = selectVal || "all";
        var exts = $.grep(mapExts, function (obj) {
            return obj.AttrOfOper == field;
        });
        $("#DDL_" + field).val(selectVal);
        if (exts.length == 0)
            return true;
        var ddlPerant = $("#DDL_" + mapExt.AttrOfOper);
        var ddlChild = $("#DDL_" + mapExt.AttrsOfActive);
        if (ddlPerant != null && ddlChild != null) {
            ddlPerant.attr("onchange", "DDLRelation(this.value,\'" + "DDL_" + exts[0].AttrsOfActive + "\', \'" + exts[0].MyPK + "\',\'" + selectVal + "\')");

        }

    })

    $("#TB_Key").val(GetQueryString("Key"));
    layui.form.render();


}
/**
 * 点击下三角选择列的显示隐藏
 */
function dropDownLayuiCols(attrs) {
    if (isSecHeader == true || isThrHeader == true)
        return;
    var h = $(document).height() - $("#toolbar").height() - 150;
    var _html = GetDropdownHtml(attrs, h);
    //筛选列的显示
    var inst = layui.dropdown.render({
        elem: '.analyseCols',
        content: _html,
        style: 'height: ' + h + 'px; box-shadow: 1px 1px 30px rgb(0 0 0 / 12%);border: 1px solid #d2d2d2;'
        , ready: function () {
            inst.reload({
                show: true //重载即显示
                , content: GetDropdownHtml(attrs, h),
            });
            layui.form.render();
        }

    })

}
function GetDropdownHtml(attrs, h) {
    var _html = '<div id="laytable-col" class="layui-form"><ul class="layui-table-col" style="height:' + h + 'px;">';
    var cols = $("div[lay-id='tableSearch']").find(".layui-table-header").find("th");
    $.each(attrs, function (idx, attr) {
        if (attr.UIVisible == 0
            || attr.KeyOfEn == "OID"
            || attr.KeyOfEn == "WorkID"
            || attr.KeyOfEn == "NodeID"
            || attr.KeyOfEn == "MyNum"
            || attr.KeyOfEn == "MyPK") {

            return true;
        }
        var col = cols[idx + 1];//排除序号字段或者复选框节点
        if (pageType == "batch")
            col = cols[idx + 2];
        var data = $(col).data() || {};
        var isHide = false;
        if ($(col).hasClass("layui-hide") == true)
            isHide = true;
        else
            isHide = false;

        data.key = data.key || "";
        var key = data.key.replace(vtable.config.index + "-", "");
        _html += '<li><input type="checkbox" name="' + data.field + '" data-key="' + key + '" data-parentkey="' + (data.parentKey || "") + '" lay-skin="primary" ' + (isHide ? "" : "checked") + ' title="' + attr.Name + '" lay-filter="LAY_TABLE_TOOL_COLS"></li>';

    })
    _html += '</ul></div>';
    return _html;
}
function GetInputHtml(labName, key, val, keyPlaceholder) {
    var _html = "";
    _html += "<div class='layui-inline'>";
    _html += "<label class='layui-form-label'>" + labName + ":</label >"
    _html += "<div class='layui-input-inline'>";
    _html += "<input type='text' id='TB_" + key + "' class='layui-input' name='TB_" + key + "' value='" + val + "' placeholder='" + keyPlaceholder + "'>";
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

function GetDateInputPart(dtSearchWay, labName, val1, val2, dateInfo) {
    var _html = "";
    _html += "<div class='layui-inline'>";
    _html += "<label class='layui-form-label'>" + labName + ":</label >"
    if (dtSearchWay == 1 || dtSearchWay == 2) {
        _html += "<div class='layui-input-inline' style='width:120px'><i class='input-icon layui-icon layui-icon-date'></i><input type='text' id='TB_DTFrom' class='layui-input ccdate' name='TB_DTFrom'placeholder='开始时间' data-info='" + dateInfo + "' value='" + val1 + "'/></div>";
        _html += "<div class='layui-input-inline'style='width:5px'>-</div>";
        _html += "<div class='layui-input-inline' style='width:120px'><i class='input-icon layui-icon layui-icon-date'></i><input type='text' id='TB_DTTo' class='layui-input ccdate' name='TB_DTTo' placeholder='结束时间' data-info='" + dateInfo + "' value='" + val2 + "'/></div>";
    }
    if (dtSearchWay == 3 || dtSearchWay == 4) {
        _html += "<div class='layui-input-inline'><i class='input-icon layui-icon layui-icon-date'></i><input type='text' id='TB_DTFrom' class='layui-input ccdate' name='TB_DTFrom'placeholder='' data-info='" + dateInfo + "' value='" + val1 + "'/></div>";

    }
    _html += "</div>";
    return _html;
}
function GetCheckBoxHtml(labName, key, val) {
    var _html = "";
    _html += "<div class='layui-inline'>";
   // _html += "<label class='layui-form-label'>" + labName + ":</label >"
    _html += "<div class='layui-input-inline'>";
    var checkStr = val == "1" ? "checked" : "";
    _html += "<input type='checkbox' name='CB_" + key + "' id='CB_" + key + "' title='" + labName + "' lay-skin='primary' "+checkStr+">";
    _html += "</div>";
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

function GetXmlSelectHtml(labName, key) {
    var _html = "";
    _html += "<div class='layui-inline'>";
    _html += "<label class='layui-form-label'>" + labName + ":</label >"
    _html += "<div class='layui-input-inline'>";
    _html += "<div  id='XmlSelect_" + key + "'name='XmlSelect_" + key + "'>";
    _html += "</div>";
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

	colorSet = colorSet || "";
    colorSet = colorSet.replace(/@/g, "@");   
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
    var foramtFunc = cfg.ForamtFunc;
    if (foramtFunc == null || foramtFunc == undefined || foramtFunc == "")
        foramtFunc = "";

    if (isBatch && isBatch == true) {
        fieldColumns = {
            type: 'checkbox',
            field: '',
            align: 'center',
            width: 50,
            rowspan: isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1,

        };
        AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1);
    }
    var title = `序`;
    if (isSecHeader == false && isThrHeader == false)
        title = `序<i class="layui-icon analyseCols">&#xe625;</i>`;
    fieldColumns = {
        title: title,
        field: '',
        align: 'center',
        width: 80,
        rowspan: isThrHeader == true ? 3 : isSecHeader == true ? 2 : 1,

        templet: function (d) {
            return pageSize * (pageIdx - 1) + d.LAY_INDEX;    // 返回每条的序号： 每页条数 *（当前页 - 1 ）+ 序号

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
        var sortable = false;
        if (sortColumns != null && sortColumns != "")
            sortable = sortColumns.indexOf(field) != -1 ? true : false;
        if (width < 100) {
            width = 100;
        }
        if (field == "Title") {
            width = 230;
        }

        if (attr.UIContralType == 1) {
            if (width == null || width == "" || width == undefined)
                width = 120;
            field = field + "Text";
            fieldColumns = {
                field: field,
                title: title,
                fixed: false,
                minWidth: width,
                sort: sortable,
                hide: hideAttrs.indexOf("," + field + ",") != -1,
                rowspan: keyRowSpan,
           		templet: function (row) {
                    var formatter = "";
                    var val = row[this.field];
                    if (typeof val == 'undefined') {
                        val = row[this.field.substring(0, this.field.length-4)];
                    }
                    var field = this.field.substring(0, this.field.length - 4);
                    if (foramtFunc != "" && foramtFunc.indexOf(field + "@") != -1) {
                        formatter = foramtFunc.substring(foramtFunc.indexOf(field + "@"));
                        formatter = formatter.substring(0, formatter.indexOf(";"));
                        var strs = formatter.split("@");
                        if (strs.length == 2) {
                            val = eval(strs[1] + "('" + row[field] + "','" + JSON.stringify(row) + "')");
                        }
                    }
                   if (drillFields.indexOf(field + ",") != -1 && typeof Drill == "function") {
                        var _html = cceval("Drill('" + field + "','" + val + "','" + JSON.stringify(row) + "')");
                        return _html;
                    }
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
                minWidth: width,
                fixed: false,
                sort: sortable,
                rowspan: keyRowSpan,
                hide: hideAttrs.indexOf("," + field + ",") != -1,
                templet: function (row) {
                    var val = "";
                    if (row[this.field] == "0")
                        val = "否";
                    if (row[this.field] == "1")
                        val = "是";
                    if (drillFields.indexOf(this.field + ",") != -1 && typeof Drill == "function") {
                        var _html = cceval("Drill('" + this.field + "','" + val + "','" + JSON.stringify(row) + "')");
                        return _html;
                    }
                    return val;

                }
            };
            AddColumn(fieldColumns, firstColumns, secondColumns, threeColumns, keyRowSpan);
            continue;
        }

        if (width == null || width == "" || width == undefined)
            width = 100;
        if (attr.IsRichText == "1") {
            richAttrs.push(attr.KeyOfEn);
            fieldColumns = {
                field: field,
                title: title,
                minWidth: width,
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
                minWidth: width,
                fixed: false,
                sort: sortable,
                rowspan: keyRowSpan,
                hide: hideAttrs.indexOf("," + field + ",") != -1,
                templet: function (row) {
                    var val = row[this.field];
                    if (val == null || val == undefined || val == "")
                        return "";
                    val = FormatDate(new Date(val), "yyyy-MM-dd");
                    if (drillFields.indexOf(this.field + ",") != -1 && typeof Drill == "function") {
                        var _html = cceval("Drill('" + this.field + "','" + val + "','" + JSON.stringify(row) + "')");
                        return _html;
                    }
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
            rowspan: keyRowSpan,
            templet: function (row) {
            	//如果这个字段是Icon
                if (this.field.toLowerCase() == "icon" && row[this.field] != "") {
                    return "<i  class='" + row[this.field] + "'></i>";
                }
                var val = row[this.field];
                if (val == null)
                    val = "";
                var formatter = "";

                if (foramtFunc != "" && foramtFunc.indexOf(this.field + "@") != -1) {
                    formatter = foramtFunc.substring(foramtFunc.indexOf(this.field + "@"));
                    formatter = formatter.substring(0, formatter.indexOf(";"));
                    var strs = formatter.split("@");
                    if (strs.length == 2) {
                        val = eval(strs[1] + "('" + val + "','" + JSON.stringify(row) + "')");
                    }
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
                    return "<a href='#' style='color:#3959c1' onclick='OpenEn(\"" + pkval + "\",\"" + paras + "\",1,\"" + rowstr + "\")'>" + val + "</a>";
                }
                if (drillFields.indexOf(this.field + ",") != -1 && typeof Drill == "function") {
                    var _html = cceval("Drill('" + this.field + "','" + val + "','" + JSON.stringify(row) + "')");
                    return _html;
                }
                var fieldColor = [];
                if (colorSet.indexOf("@" + this.field + ":") != -1) {
                    fieldColor = getFieldColor(colorSet, this.field);
                }
                if (fieldColor.length == 0)
                    return val + "";
                for (var i = 0; i < fieldColor.length; i++) {
                    var color = fieldColor[i];
                    if (color.From <= row[this.field] && color.To >= row[this.field])
                        return "<div style='width:20px;height:20px;text-align:center;background:" + color.Color + ";color:white'>" + val + "</div>";
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
    if (dtMs.length > 0 || entityAthType == 1 || entityAthType == 2
        || (cfg.EnBtnLab1 && cfg.EnBtnLab1 != "") || (cfg.EnBtnLab2 && cfg.EnBtnLab2 != "")) {
        var len = dtMs.length;
        if (cfg.EnBtnLab1 && cfg.EnBtnLab1 != "")
            len++;
        if (cfg.EnBtnLab2 && cfg.EnBtnLab2 != "")
            len++;
        fieldColumns = {
            field: '',
            title: '操作',
            minWidth: len * 100 + 50,
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
        if (dtM.Icon != "" && dtM.Icon != null)
            _html += '<a class="layui-btn layui-btn-xs" lay-event="' + dtM.Title + '"><i class="' + dtM.Icon + '"></i>&nbsp;&nbsp;' + dtM.Title + '</a>';
        else
            _html += '<a class="layui-btn layui-btn-xs" lay-event="' + dtM.Title + '">' + dtM.Title + '</a>';
    });
    if (row.MyFileName != null && row.MyFileName != undefined && row.MyFileName != "")
        _html += '<a class="layui-btn layui-btn-xs" lay-event="downloadFile">下载</a>';
    var btnLab1 = cfg.EnBtnLab1;
    if (btnLab1 != null && btnLab1 != undefined && btnLab1 != "")
        _html += '<a class="layui-btn layui-btn-xs" lay-event="Btn_En1">' + btnLab1 + '</a>';
    var btnLab2 = cfg.EnBtnLab2;
    if (btnLab2 != null && btnLab2 != undefined && btnLab2 != "")
        _html += '<a class="layui-btn layui-btn-xs" lay-event="Btn_En2">' + btnLab2 + '</a>';
    if (_html != "") {
        $("#Btns").append(_html);
    }

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
            OpenLayuiDialog(url, dtm.Title, W, 100, "r", false);
            break;
        case RefMethodType.Func:
            if (dtm.FunPara == true || dtm.FunPara == "true")
                OpenLayuiDialog(url, dtm.Title, W, 100, "r", false);
            else {
                if (warning == null || warning == "null" || warning == "") {
                    warning = "确定要执行吗？";
                }
                else {
                    warning = warning.replace(/,\s+/g, ",");
                    warning = warning.replace(/\s+/g, "\r\n");
                }
                openFun(warning, url, dtm.Title);

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
function Search(pageType) {
    pageType = pageType || "search";
    SearchCondtion();
    pageIdx = 1;
    var tableData = SearchData(pageType);
    //分页
    layui.table.reload('tableSearch', { data: transferHtmlData(tableData["DT"]) });
    dropDownLayuiCols(tableData.Attrs);
    renderLaypage();
}

function SearchCondtion() {
   
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
    $.each(cbKey, function (i, key) {
        if ($("#CB_" + key).is(':checked')==true)
            str += "@" + key + "=1";
        else
            str += "@" + key + "=0";
    })
    $.each(dllKey, function (i, key) {
            str += "@" + key + "=" + $("#DDL_" + key).val();
    });
    $.each(xmlSelectKey, function (i, key) {
        var val = xmSelect.get('#XmlSelect_' + key, true).getValue('value');
        str += "@" + key + "=" + val.join(",");
    })

    $.each(treeKey, function (i, key) {
        var val = xmSelect.get('#XmlSelect_' + key, true).getValue('value');
        str += "@" + key + "=" + val.join(",");
    });

    ur.FK_Emp = webUser.No;
    ur.CfgKey = "SearchAttrs";
   
    ur.Vals = str;
    ur.FK_MapData = ensName;
    ur.SetPara("RecCount", count);
    var i = ur.Update();

}

function SearchData(pageType, orderBy, orderWay) {
    pageType = pageType || "search";
    //ur = GetUserRegedit()
    SearchCondtion();
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Comm");
    handler.AddUrlData();
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

    if (pageType == "batch") {
        batchData.forEach(function (item) {
            data["DT"].forEach(function (obj) {
                if (obj[enPK] == item[enPK]) {
                    obj.LAY_CHECKED = true;
                    return false;
                }
            })
        })
    }

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
                _html += item.Field + "(总合计)：" + val + " ";
            if (item.Type == "Avg")
                _html += item.Field + "(总平均)：" + val + " ";
        });
    }


    $("#JsResult").html("").html(_html);
    return data;
}

var searchTableData = [];
function transferHtmlData(tableData) {
    tableData = JSON.parse(filterXSS(JSON.stringify(tableData)))
    var val = "";
    if (richAttrs.length != 0) {
        $.each(tableData, function (i, item) {
            richAttrs.forEach(key => {
                val = item[key];
                if (val != "") {

                    // val = htmlEncodeByRegExp(val);
                    // val = val.replace(/<[^>]+>/g, "")
                    item[key] = filterXSS(val);
                    console.log(item[key])
                }
            })
        });
    }
    searchTableData = tableData;
    return tableData;
}
function htmlEncodeByRegExp(str) {
    var s = '';
    if (str == null || str == undefined)
        return "";
    if (str && str.length === 0) {
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
    if (str == null || str == undefined)
        return "";
    if (str && str.length === 0) {
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


function OpenEn(pk, paras, flag, row, obj, openType) {
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
		var tName = "详细";
        if(openType == 'New'){
            tName = "新建";
        }
        if(openType == 'Edit'){
            tName = "编辑";
        }
        var isRefresh = cfg.IsRefreshParentPage == 1 ? true : false;
        if (isRefresh) 
            OpenLayuiDialog(url, mapBase.EnDesc + ' : ' + tName, windowW, 100, "r", false, false, false, null, function () {
                if(pk== null || pk == undefined || pk=="" ){
                    SearchData();
                    return;
                }
				ChangeTableData(pk,enName,obj);
            });
        else
            OpenLayuiDialog(url, mapBase.EnDesc + ' : ' + tName, windowW, 100, "r", false);
    } else {
        var windowObj = window.open(url);
        if (flag == 0 || cfg.IsRefreshParentPage == 1) {
            var loop = setInterval(function () {
                if (windowObj.closed) {
                    clearInterval(loop);
                    SearchData();

                }

            }, 1000);
        }
    }


}

function ChangeTableData(pkVal, enName, obj) {
    if (typeof obj != "undefined") {
        //根据PK获取到改行的最新信息
        var en = new Entity(enName);
        en.SetPKVal(pkVal);
        en.RetrieveFromDBSources();

        searchTableData.forEach(item => {
            if (item[enPK] == pkVal) {
                if (richAttrs.length != 0) {
                    richAttrs.forEach(key => {
                        var val = item[key];
                        if (val != "") {
                            en[key] = filterXSS(val);
                            console.log(item[key])
                        }
                    });
                }
                for (var key in item)
                    item[key] = en[key];
                if (typeof obj != "undefined") {
                    obj.update(item);
                }
                return;
            }
        });
    }
    
    if (typeof obj === "undefined") {
        pageIdx = 1;
        var data = SearchData("search");
        //获取列
        var tableData = transferHtmlData(data["DT"]);
        layui.table.reload('tableSearch', { data: tableData });
    }
        
}

function New() {
    OpenEn("", "", 0, null, "New");
}

function Exp(type, pageType) {
    var searchData = [];
    if (pageType == "search") {
        SearchCondtion();
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Comm");
        handler.AddPara("EnsName", GetQueryString("EnsName"));
        //查询集合
        var data = handler.DoMethodReturnString("Search_Exp");
        if (data.indexOf("err@") == 0) {
            layer.alert(data);
            return;
        }
        searchData = JSON.parse(data);
        var richtext = "," + richAttrs.join(",") + ","
        $.each(searchData, function (i, item) {
            for (var key in item) {
                if (item[key] == null || item[key] == undefined)
                    item[key] = "";
                if (type == 0)
                    item[key] = item[key] + "";
                if (richtext.indexOf("," + key + ",") != -1) {
                    var val = item[key];
                    val = replaceAll(val, "<[^>]+>", "");
                    val = replaceAll(val, "&[^;]+;", "");
                    item[key] = val;
                }
            }

        })
    }
    if (pageType == "batch") {
        searchData = batchData;
    }
    if (searchData.length == 0) {
        layer.alert("没有需要导出的数据");
        return "";
    }
    if (type == 0) {
        layui.table.exportFile("tableSearch", searchData, 'xls');
    }

    if (type == 1)
        layui.table.exportFile("tableSearch", searchData, 'csv');
    return;


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
        SetHref(basePath + '/WF/Comm/ProcessRequest?DoType=HttpHandler&HttpHandlerName=BP.WF.HttpHandler.WF_CommEntity&DoMethod=EntityFile_Load&DelPKVal=' + PKVal + '&EnsName=' + GetQueryString("EnsName"));
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

//执行删除
function Delete() {
   
    if (batchData.length == 0) {
        layer.alert("请选择要删除的行");
        return;
    }
    if (confirm("确定要删除选择的行吗") == false)
        return;
    var deleteRow = batchData;
    //执行删除操作
    var enName = ensName.substring(0, ensName.length - 1);
    $.each(deleteRow, function (idx, item) {
        var en = new Entity(enName);
        en.Delete(enPK, item[enPK]);
    })
    Search("batch");
    batchData = [];
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
    var newWindow = window.open(url);
    newWindow.focus();
    return;
}

//树形结构
function findChildren(jsonArray, parentNo) {
    var appendToTree = function (treeToAppend, o) {
        $.each(treeToAppend, function (i, child) {
            if (o.value == child.ParentNo)
                o.children.push({
                    "value": child.No,
                    "name": child.Name,
                    "children": []
                });
        });

        $.each(o.children, function (i, o) {
            appendToTree(jsonArray, o);
        });

    };

    var jsonTree = [];
    var jsonchildTree = [];
    if (jsonArray.length > 0 && typeof parentNo !== "undefined") {
        $.each(jsonArray, function (i, o) {
            if (o.No == parentNo) {
                jsonchildTree.push(o);
                jsonTree.push({
                    "value": o.No,
                    "name": o.Name,
                    "children": []
                });
            }
        });

        $.each(jsonTree, function (i, o) {
            appendToTree(jsonArray, o);
        });

    }

    function _(treeArray) {
        $.each(treeArray, function (i, o) {
            if ($.isArray(o.children)) {
                if (o.children.length == 0) {
                    o.children = undefined;
                } else {
                    _(o.children);
                }
            }
        });
    }
    _(jsonTree);
    return jsonTree;
}
//禁用回车功能
$(window).keydown(function (e) {

    var key = window.event ? e.keyCode : e.which;

    if (!!key && key.toString() == "13") {

        return false;

    }

});

