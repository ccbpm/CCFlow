// ********************** 根据关键字动态查询. ******************************** //
var oldValue = "";
var oid;
var highlightindex = -1;
function DoAnscToFillDiv(sender, selectVal, tbid, fk_mapExt, dbSrc, dbType) {
    openDiv(sender, tbid);
    var mapExt = new Entity("BP.Sys.MapExt", fk_mapExt);
    var myEvent = window.event || arguments[0];
    var myKeyCode = myEvent.keyCode;
    // 获得ID为divinfo里面的DIV对象 .  
    var autoNodes = $("#divinfo").children("div");
    if (myKeyCode == 38) {
        if (highlightindex != -1) {
            autoNodes.eq(highlightindex).css("background-color", "white");
            autoNodes.eq(highlightindex).css("color", "Black");
            if (highlightindex == 0) {
                highlightindex = autoNodes.length - 1;
            }
            else {
                highlightindex--;
            }
        }
        else {
            highlightindex = autoNodes.length - 1;
        }
        autoNodes.eq(highlightindex).css("background-color", "rgb(40, 132, 250)");
        autoNodes.eq(highlightindex).css("color", "white");
    }
    else if (myKeyCode == 40) {
        if (highlightindex != -1) {
            autoNodes.eq(highlightindex).css("background-color", "white");
            autoNodes.eq(highlightindex).css("color", "black");
            highlightindex++;
        }
        else {
            highlightindex++;
        }

        if (highlightindex == autoNodes.length) {
            autoNodes.eq(autoNodes.length).css("background-color", "white");
            autoNodes.eq(autoNodes.length).css("color", "black");
            highlightindex = 0;
        }
        autoNodes.eq(highlightindex).css("background-color", "rgb(40, 132, 250)");
        autoNodes.eq(highlightindex).css("color", "white");
    }
    else if (myKeyCode == 13) {
        if (highlightindex != -1) {

            //获得选中的那个的文本值
            var textInputText = autoNodes.eq(highlightindex).text();
            var strs = textInputText.split('|');
            autoNodes.eq(highlightindex).css("background-color", "white");
            $("#" + tbid).val(strs[0]);
            $("#divinfo").hide();
            oldValue = strs[0];

            // 填充.
            FullIt(oldValue, mapExt.MyPK, tbid);
            highlightindex = -1;
        }
    }
    else {
        if (selectVal != oldValue) {
            $("#divinfo").empty();
            //获得对象.
            var mapExt = new Entity("BP.Sys.MapExt", fk_mapExt);
            var dataObj = GenerDB(mapExt.Doc, selectVal, mapExt.DBType, mapExt.FK_DBSrc);

            if ($.isEmptyObject(dataObj)) {
                $("#divinfo").hide();
                return;
            }
            
            //简洁模式
            if (TBModel == "Simple"){
                $.each(dataObj, function (idx, item) {
                    var no = item.No;
                    if (no == undefined)
                        no = item.NO;

                    var name = item.Name;
                    if (name == undefined)
                        name = item.NAME;


                    $("#divinfo").append("<div style='" + itemStyle + "' name='" + idx + "' onmouseover='MyOver(this)' onmouseout='MyOut(this)' onclick=\"ItemClick('" + sender.id + "','" + no + "','" + tbid + "','" + fk_mapExt + "');\" value='" + no + "'>" + no + '|' + name + "</div>");
                });

            }

            //表格模式
            if(TBModel=="Table")
                showDataGrid(sender, tbid, dataObj, mapExt); 

            oldValue = selectVal;

        }

    }
}

function showDataGrid(sender, tbid, dataObj, mapExt) {
    //加载bootStrap的js
    if (GetHrefUrl().indexOf("CCForm") != -1) {
        $('head').append('<link href="../Scripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />');
        $('head').append('<link href="../Scripts/bootstrap/bootstrap-table/src/bootstrap-table.css" rel="stylesheet" type="text/css" />');
        Skip.addJs("../Scripts/bootstrap/bootstrap-table/src/bootstrap-table.js");

    } else {
        $('head').append('<link href="./Scripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />');
        $('head').append('<link href="./Scripts/bootstrap/bootstrap-table/src/bootstrap-table.css" rel="stylesheet" type="text/css" />');
        Skip.addJs("./Scripts/bootstrap/bootstrap-table/src/bootstrap-table.js");
    }
    var columns = mapExt.Tag3;
    $("#divinfo").append("<table id='viewGrid'></table>");
    //取消DIV的宽度
    document.getElementById('divinfo').style.width = "";
    //显示行号的添加
    var searchTableColumns = [{
        formatter: function (value, row, index) {
            return index + 1;
        }
    }];

    //显示列的中文名称
    if (typeof columns == "string") {

        $.each(columns.split(","), function (i, o) {
            var exp = o.split("=");
            var field;
            var title;
            if (exp.length == 1) {
                field = title = exp[0];
            } else if (exp.length == 2) {
                field = exp[0];
                title = exp[1];
            }
            if (!isLegalName(field)) {
                return true;
            }
            searchTableColumns.push({
                field: field,
                title: title
            });
        });

        var options = {
            striped: true,
            cache: false,
            showHeader: true,
            sortOrder: "asc",
            strictSearch: true,
            minimumCountColumns: 2,
            clickToSelect: true,
            sortable: false,
            cardView: false,
            detailView: false,
            uniqueId: "No",
            columns: searchTableColumns
        };
        //选中行的操作
        options.onClickRow = function (row, element) {
            $("#divinfo").empty();
            $("#divinfo").css("display", "none");
            highlightindex = -1;
            $("#" + tbid).val(row.No);
            FullIt(row.No, mapExt.MyPK, tbid);
        };

        $('#viewGrid').bootstrapTable(options);
        $('#viewGrid').bootstrapTable("load", dataObj);
    }
}

function isLegalName(name) {
    if (!name) {
        return false;
    }
    return name.match(/^[a-zA-Z\$_][a-zA-Z\d\$_]*$/);
}

//填充其他的控件.
function FullIt(selectVal, refPK, elementId,type) {

    if (oid == null)
        oid = GetQueryString('OID');

    if (oid == null)
        oid = GetQueryString('WorkID');

    if (oid == null) {
        oid = 0;
        return;
    }

    var mypk = "";
    if (refPK.indexOf("FullData") != -1)
        mypk = refPK;
    else
        mypk = refPK + "_FullData";

    //获得对象.
    var mapExt = new Entity("BP.Sys.MapExt");
    mapExt.SetPKVal(mypk);
    var i = mapExt.RetrieveFromDBSources();

    //没有填充其他控件
    if (i == 0)
        return;

    
    //生成关键字.
    GenerPageKVs();

    //执行填充主表的控件.
    FullCtrl(selectVal, elementId, mapExt,type);

    //执行个性化填充下拉框，比如填充ddl下拉框的范围.
    FullCtrlDDL(selectVal, elementId, mapExt);

    //执行填充从表.
    FullDtl(selectVal, mapExt);

    //执行确定后执行的JS
    var backFunc = mapExt.Tag2;
    if (backFunc != null && backFunc != "" && backFunc != undefined) {
        var oid = GetQueryString("OID")
        if (backFunc.indexOf("@OID") != -1 && oid==null) {
            //获取OID的值
            var oid = elementId.replace("TB_", "").replace("DDL_", "").replace("CB_", "").replace(mapExt.AttrOfOper + "_", "");
            backFunc = backFunc.replace(/@OID/g, oid);
        }
        backFunc = DealSQL(backFunc, selectVal);
        DBAccess.RunFunctionReturnStr(backFunc); 
    }
    


}
function openDiv(e, tbID) {


    if (document.getElementById("divinfo").style.display == "none") {

        var txtObject = e; //  document.getElementById(tbID);
        var orgObject = document.getElementById("divinfo");
        var rect = getoffset(txtObject);
        var t = rect[0] + 29;
        var l = rect[1];

        orgObject.style.top = t + 'px';
        orgObject.style.left = l + 'px';
        orgObject.style.display = "block";
        txtObject.focus();
    }
}
function getoffset(e) {
    var t = e.offsetTop;
    var l = e.offsetLeft;
    while (e = e.offsetParent) {
        if (e.id == 'divCCForm') {
            break;
        }

        t += e.offsetTop;
        l += e.offsetLeft;
    }
    var rec = new Array(1);
    rec[0] = t;
    rec[1] = l;
    return rec
}

var kvs = null;
function GenerPageKVs() {
    var ddls = null;
    ddls = parent.document.getElementsByTagName("select");
    kvs = "";
    for (var i = 0; i < ddls.length; i++) {
        var id = ddls[i].name;

        if (id.indexOf('DDL_') == -1) {
            continue;
        }
        var myid = id.substring(id.indexOf('DDL_') + 4);
        kvs += '~' + myid + '=' + ddls[i].value;
    }

    ddls = document.getElementsByTagName("select");
    for (var i = 0; i < ddls.length; i++) {
        var id = ddls[i].name;

        if (id.indexOf('DDL_') == -1) {
            continue;
        }
        var myid = id.substring(id.indexOf('DDL_') + 4);
        kvs += '~' + myid + '=' + ddls[i].value;
    }
    return kvs;
}


/* 自动填充  type: 0 主表 1 从表*/

function DDLFullCtrl(selectVal, ddlChild, fk_mapExt,type) {
    FullIt(selectVal, fk_mapExt, ddlChild,type);
}

/* 级联下拉框  param 传到后台的一些参数  例如从表的行数据 主表的字段值 如果param参数在，就不去页面中取KVS 了，PARAM 就是*/
function DDLAnsc(selectVal, ddlChild, fk_mapExt, param) {

    //1.初始值为空或者NULL时，相关联的字段没有数据显示
    if (selectVal == null || selectVal == "") {
        $("#" + ddlChild).empty();
        //无数据返回时，提示显示无数据，并将与此关联的下级下拉框也处理一遍，edited by liuxc,2015-10-22
        $("#" + ddlChild).append("<option value='' selected='selected' >无数据</option");
        var chg = $("#" + ddlChild).attr("onchange");

        $("#" + ddlChild).change();

        return;
    }

    GenerPageKVs();

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

    var dataObj = GenerDB(dbSrc, selectVal, mapExt.DBType, mapExt.FK_DBSrc); //获得数据源.

    // 这里要设置一下获取的外部数据.
    // 获取原来选择值.
    var oldVal = null;
    var ddl = document.getElementById(ddlChild);
    if (ddl == null) return;

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

        if (chg != null && chg != undefined && (chg.indexOf("DDLAnsc") == 0 || chg.indexOf("DDLFullCtrl") == 0)) {
            $("#" + ddlChild).change();
        }
        return;
    }

    //不为空的时候赋值
    $.each(dataObj, function (idx, item) {
        $("#" + ddlChild).append("<option value='" + item.No + "'>" + item.Name + "</option");
        
        
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
    if(isInIt == false){
	$("#" + ddlChild).val(dataObj[0].No);
	var chg = $("#" + ddlChild).attr("onchange");
        if (chg != null && chg != undefined && (chg.indexOf("DDLAnsc") == 0 || chg.indexOf("DDLFullCtrl")==0))
            $("#" + ddlChild).change();
    }
 
}

//填充明细.
function FullDtl(selectVal, mapExt) {
    if (mapExt.Tag1 == "" || mapExt.Tag1 == null)
        return;

    var dbType = mapExt.DBType;
    var dbSrc = mapExt.Tag1;
    var url = GetLocalWFPreHref();
    var dataObj;

    if (dbType == 1) {

        dbSrc = DealSQL(DealExp(dbSrc), e, kvs);
        dataObj = DBAccess.RunDBSrc(dbSrc, 1,mapExt.FK_DBSrc);

        //JQuery 获取数据源
    } else if (dbType == 2) {
        dataObj = DBAccess.RunDBSrc(dbSrc, 2,mapExt.FK_DBSrc);
    } else {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddPara("Key", selectVal);
        handler.AddPara("FK_MapExt", mapExt.MyPK);
        handler.AddPara("KVs", kvs);
        handler.AddPara("DoTypeExt", "ReqDtlFullList");
        handler.AddPara("OID", oid);
        var data = handler.DoMethodReturnString("HandlerMapExt");
        if (data == "")
            return;

        if (data.indexOf('err@') == 0) {
            alert(data);
            return;
        }
        dataObj = cceval("(" + data + ")"); //转换为json对象
    }
}
function FullCtrlDDL(selectVal, ctrlID, mapExt) {

    if (mapExt.Tag == "" || mapExt.Tag == null)
        return;

    var dbSrcs = mapExt.Tag.split('$'); //获得集合.
    for (var i = 0; i < dbSrcs.length; i++) {

        var dbSrc = dbSrcs[i];
        if (dbSrc == "" || dbSrc.length == 0)
            continue;
        var ctrlID = dbSrc.substring(0, dbSrc.indexOf(':'));
        var src = dbSrc.substring(dbSrc.indexOf(':') + 1);

        var db = GenerDB(src, selectVal, mapExt.DBType, mapExt.FK_DBSrc); //获得数据源.

        //重新绑定下拉框.
        $("#DDL_" + ctrlID).empty();
        if (db && db.length != 0) {
            $("#DDL_" + ctrlID).append("<option value='' selected='selected' >无数据</option");
            $("#DDL_" + ctrlID).attr("onchange");
            return;
        }
    }
}


function GenerDB(dbSrc, selectVal, dbType,dbSource) {


    //处理sql，url参数.
    dbSrc = dbSrc.replace(/~/g, "'");

    dbSrc = dbSrc.replace(/@Key/g, selectVal);
    dbSrc = dbSrc.replace(/@key/g, selectVal);
    dbSrc = dbSrc.replace(/@KEY/g, selectVal);
    dbSrc = DealExp(dbSrc);
    dbSrc = DealSQL(dbSrc, selectVal, kvs);

    //获取数据源.
    dataObj = DBAccess.RunDBSrc(dbSrc, dbType,dbSource);
    return dataObj;
}



//主表数据的填充.
function FullCtrl(selectVal, ctrlIdBefore, mapExt,type) {

    var dbSrc = mapExt.Doc;
    if (dbSrc == null || dbSrc == "") {
        //alert("没有配置填充主表的信息");
        return;
    }
    var dataObj = GenerDB(dbSrc, selectVal, mapExt.DBType, mapExt.FK_DBSrc);

    TableFullCtrl(dataObj, ctrlIdBefore,type);
    //如果含有FullDataDtl也需要处理
    var mapExts = new Entities("BP.Sys.MapExts");
    mapExts.Retrieve("FK_MapData", mapExt.FK_MapData, "AttrOfOper", mapExt.AttrOfOper, "ExtType", "FullDataDtl");
    for (var i = 0; i < mapExts.length; i++) {
        var item = mapExts[i];
        var dbSrc = item.Doc;
        if (dbSrc != null && dbSrc != "") {
            var dataObj = GenerDB(dbSrc, selectVal, item.DBType, item.FK_DBSrc);

            TableFullCtrl(dataObj, ctrlIdBefore,type);
        }

    }
}

function TableFullCtrl(dataObj, ctrlIdBefore,type) {

    type = type == undefined || type == null ? 0 : type;
    if ($.isEmptyObject(dataObj)) {
        return;
    }
    var data = dataObj[0]; //获得这一行数据.
   

    //针对主表或者从表的文本框自动填充功能，需要确定填充的ID
    var beforeID = null;
    var endId = null;

    // 根据ddl 与 tb 不同。
    if (ctrlIdBefore.indexOf('DDL_') > 1) {
        beforeID = ctrlIdBefore.substring(0, ctrlIdBefore.indexOf('DDL_'));
        endId = ctrlIdBefore.substring(ctrlIdBefore.lastIndexOf('_'));
    } else {
        beforeID = ctrlIdBefore.substring(0, ctrlIdBefore.indexOf('TB_'));
        endId = ctrlIdBefore.substring(ctrlIdBefore.lastIndexOf('_'));
    }
   
   
    if (frmMapAttrs != null && frmMapAttrs.length != 0 && type == 0) {
        data = DealDataTableColName(data, frmMapAttrs);
    } else {
        data = DealDataTableColName(data, dtlmapAttrs);
    }

    //遍历属性，给属性赋值.
    var valID;
    for (var key in data) {

        var val = data[key];
        valID = $("#" + beforeID + "TB_" + key);
        if (valID.length == 1) {
            valID.val(val);
            continue;
        }
        valID = $("#" + beforeID + "TB_" + key + endId);
        if (valID.length == 1) {
            valID.val(val);
            continue;
        }

        valID = $("#" + beforeID + "DDL_" + key)
        if (valID.length == 1) {
            valID.val(val);
            continue;
        }
        valID = $("#" + beforeID + "DDL_" + key + endId);
        if (valID.length == 1) {
            valID.val(val);
            continue;
        }

        valID = $("#" + beforeID + 'CB_' + key);
        if (valID.length == 1) {
            if (val == '1') {
                valID.attr("checked", true);
            } else {
                valID.attr("checked", false);

            }
            continue;
        }
        valID = $("#" + beforeID + 'CB_' + key + endId);
        if (valID.length == 1) {
            if (val == '1') {
                valID.attr("checked", true);
            } else {
                valID.attr("checked", false);

            }
            continue;
        }

        //获取表单中所有的字段
        if (valID.length == 0) {
            var tbs = $('input');
            $.each(tbs, function (i, tb) {
                var name = $(tb).attr("id");
                if (name == null || name == undefined)
                    return false;
                if (name.toUpperCase().indexOf(key) >= 0) {
                    if (name.indexOf("TB_") == 0)
                        $("#" + name).val(val);
                    if (name.indexOf("CB_")) {
                        if (val == '1') {
                            $("#" + name).attr("checked", true);
                        } else {
                            $("#" + name).attr("checked", false);

                        }
                    }
                    return false;
                }
            });

            var selects = $('select');
            $.each(selects, function (i, select) {
                var name = $(select).attr("id");
                if (name.toUpperCase().indexOf(key) >= 0) {
                    $("#" + name).val(val);
                    return false;
                }
            });
        }
    }
}

var itemStyle = 'padding:2px;color: #000000;width:100%;border-bottom: 1px solid #c8c7cc;';
var itemStyleOfS = 'padding:2px;color: #000000;background-color:green;width:100%';
function ItemClick(sender, val, tbid, fk_mapExt) {

    $("#divinfo").empty();
    $("#divinfo").css("display", "none");
    highlightindex = -1;
    oldValue = val;

    $("#" + tbid).val(oldValue);

    // 填充.
    FullIt(oldValue, fk_mapExt, tbid);
}

function MyOver(sender) {
    if (highlightindex != -1) {
        $("#divinfo").children("div").eq(highlightindex).css("background-color", "white");
    }

    highlightindex = $(sender).attr("name");
    $(sender).css("background-color", "rgb(40, 132, 250)");
    $(sender).css("color", "white");
}

function MyOut(sender) {
    $(sender).css("background-color", "white");
    $(sender).css("color", "black");
}

function hiddiv() {
    $("#divinfo").css("display", "none");
}

//通过正则表达式检测
function CheckInput(oInput, filter) {
    var re = filter;
    if (typeof (filter) == "string") {
        re = new RegExp(filter);
    }
    return re.test(oInput);
}
//正则表达式检查
function CheckRegInput(oInput, filter, tipInfo) {
    var mapExt = $('#' + oInput).data();
    var filter = mapExt.Doc.replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}');
    var oInputVal = $("[name=" + oInput + ']').val();
    var result = true;
    if (oInput != '') {
        var re = filter;
        if (typeof (filter) == "string") {
            if (filter.indexOf('/') == 0) {
                filter = filter.substr(1, filter.length - 2);
            }

            re = new RegExp(filter);
        } else {
            re = filter;
        }
        result = re.test(oInputVal);
    }
    if (!result) {
        mui.alert(tipInfo);
        if ($('#' + oInput).attr("type") == "number")
            $("#" + oInput).val(0);
        else
            $("#" + oInput).val("");
        $("#" + oInput).trigger("change");
        return result;
    }
    //NumEnterLimit($("#" + oInput));
    return result;
}

//输入限制
function NumEnterLimit(element) {
    var oInputVal = element.val();
    //判断是否存在最大值，最小值的判断
    var minVal = element.attr("data-min");
    if (minVal == undefined || minVal == "") {
        element.css("border", "none");
        element.removeClass('compareClass');
        return true;
    }


    if (oInputVal < parseInt(minVal)) {
        mui.alert("不能小于最小值" + minVal);
        element.addClass('compareClass');
        var cssText = element.attr("style") + ";border:1px solid red !important;"
        element.css("cssText", cssText);
        return false;
    }
    var maxVal = element.attr("data-max");
    if (maxVal == undefined || maxVal == "") {
        element.removeClass('compareClass');
        element.css("border", "none");
        return true;
    }

    if (oInputVal > parseInt(maxVal)) {
        var cssText = element.attr("style") + ";border:1px solid red !important;"
        element.css("cssText", cssText);
        element.addClass('compareClass');
        mui.alert("不能大于最大值" + maxVal);


        return false;
    }
    element.css("border", "none");
    element.removeClass('compareClass');
    return true;
}



function TB_ClickNum(ele, defVal) {
    if (ele == null) return;
    if (ele.value == "" && typeof (defVal) != 'undefined') {
        ele.value = defVal;
        return;
    }

    //赋值
    if (ele.value == "0") ele.value = "";
    if (ele.value == "0.00") ele.value = "";
    //判断小数点数
    var pointNum = ele.value.split('.');
    if (pointNum) {
        if (pointNum.length > 2) {
            ele.value = pointNum[0] + "." + pointNum[1];
        }
    }
}

//获取WF之前路径
function GetLocalWFPreHref() {
    var url = GetHrefUrl();
    if (url.indexOf('/WF/') >= 0) {
        var index = url.indexOf('/WF/');
        url = url.substring(0, index);
    } else {
        url = "";
    }
    return url;
}


function DealSQL(dbSrc, key, kvs) {

    dbSrc = dbSrc.replace(/~/g, "'");

    dbSrc = dbSrc.replace(/@Key/g, key);
    dbSrc = dbSrc.replace(/@Val/g, key);

    var oid = GetQueryString("OID");
    if (oid != null) {
        dbSrc = dbSrc.replace("@OID", oid);
    }

    if (kvs != null && kvs != "" && dbSrc.indexOf("@") >= 0) {

        var strs = kvs.split("[~]", -1);
        for (var i = 0; i < strs.length; i++) {
            var s = strs[i];
            if (s == null || s == "" || s.indexOf("=") == -1)
                continue;
            var mykv = s.split("[=]", -1);
            dbSrc = dbSrc.replace("@" + mykv[0], mykv[1]);
            if (dbSrc.indexOf("@") == -1)
                break;
        }
    }

    if (dbSrc.indexOf("@") >= 0) {
        alert('系统配置错误有一些变量没有找到:' + dbSrc);
    }

    return dbSrc;
}
