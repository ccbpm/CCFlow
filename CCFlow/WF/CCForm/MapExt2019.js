// ********************** 根据关键字动态查询. ******************************** //
var oldValue = "";
var oid;
var highlightindex = -1;
function DoAnscToFillDiv(sender, selectVal, tbid, fk_mapExt, TBModel) {
    //openDiv(sender, tbid);
    var mapExt = new Entity("BP.Sys.MapExt", fk_mapExt);
    var myEvent = window.event || arguments[0];
    var myKeyCode = myEvent.keyCode;
    // 获得ID为divinfo里面的DIV对象 .  
    var autoNodes = $("#autoComplete").children("div");
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
            $("#autodiv").remove();
            oldValue = strs[0];

            // 填充.
            FullIt(oldValue, mapExt.MyPK, tbid);
            highlightindex = -1;
        }
    }
    else {
        if (selectVal != oldValue) {
            $("#autodiv").remove();
            //获得对象.
            //var dataObj = GenerDB(mapExt.Tag4, selectVal, mapExt.DBType, mapExt.FK_DBSrc);
            //if ($.isEmptyObject(dataObj)) {
            //    //$("#divinfo").hide();
            //    return;
            //}

            //简洁模式
            if (TBModel == "Simple") {
                $("#" + tbid).after("<div id='autodiv'><div id='autoComplete' style='position:absolute;z-index:999'></div>");
                $.each(dataObj, function (idx, item) {

                    var no = item.No;
                    if (no == undefined)
                        no = item.NO;

                    var name = item.Name;
                    if (name == undefined)
                        name = item.NAME;


                    var left = $("#autodiv").offset().left;
                    $("#autoComplete").css("left", left + "px");
                    $("#autoC omplete").append("<div style='" + itemStyle + "' name='" + idx + "' onmouseover='MyOver(this)' onmouseout='MyOut(this)' onclick=\"ItemClick('" + sender.id + "','" + no + "','" + tbid + "','" + fk_mapExt + "');\" value='" + no + "'>" + no + '|' + name + "</div></div>");
                });

            }

            //表格模式
            if (TBModel == "Table")
                showDataGrid(tbid, selectVal, mapExt);

            oldValue = selectVal;

            document.onclick = function () {
                $("#autodiv").remove();
            }

        }

    }
}
/**
 * 获取数据的方法
 * @param {any} dbSrc 请求数据集合的内容
 * @param {any} dbType 请求数据的集合了类型 SQL,函数,URL
 * @param {any} dbSource 如果是SQL的时，SQL的查询来源，本地，外部数据源
 * @param {any} keyVal 选择替换的值
 */
function GetDataTableByDB(dbSrc, dbType, dbSource, keyVal,mapExt,field) {
    // debugger
    if (dbSrc == null || dbSrc == undefined || dbSrc == "")
        return null;
    if (dbType == 0) {
        var mypk = mapExt.MyPK;
        mapExt = new Entity("BP.Sys.MapExt", mapExt);
        mapExt.MyPK = mypk;
        //增加表单上的
        var paras = getPageData() + "@&Key=" + keyVal;
        var pkval = GetQueryString("WorkID") || GetQueryString("OID");
        var data = mapExt.DoMethodReturnString("GetDataTableByField", field, paras, null, pkval);
        if (data.indexOf("err@") != -1) {
            alert(data);
            return null;
        }
        var dataObj = JSON.parse(data);
        return dataObj;
    }
    //处理sql，url参数.
    dbSrc = dbSrc.replace(/~/g, "'");
    if (keyVal != null) {
        if (dbType == 0)
            keyVal = keyVal.replace(/'/g, '');

        dbSrc = dbSrc.replace(/@Key/g, keyVal);
        dbSrc = dbSrc.replace(/@key/g, keyVal);
        dbSrc = dbSrc.replace(/@KEY/g, keyVal);
    }
    dbSrc = DealExp(dbSrc, null, false);
    //获取数据源.
    dataObj = DBAccess.RunDBSrc(dbSrc, dbType, dbSource);
    return dataObj;
}


/**
* 文本自动完成表格展示
*/
function showDataGrid(tbid, selectVal, mapExtMyPK) {
    var mapExt = new Entity("BP.Sys.MapExt", mapExtMyPK);
    var dataObj = GetDataTableByDB(mapExt.Tag4, mapExt.DBType, mapExt.FK_DBSrc, selectVal,mapExt,"Tag4");
    var columns = mapExt.Tag3;
    $("#divInfo").remove();
    $("#" + tbid).after("<div style='position:relative;z-index:999;box-shadow: 0 2px 4px rgba(0, 0, 0, 0.12);width:100%' id='divInfo'><table class='layui-hide' style='width:100%;' id='autoTable' lay-filter='autoTable'></table></div>");

    var searchTableColumns = [{
        field: "",
        title: "序号",
        templet: function (d) {
            return d.LAY_TABLE_INDEX + 1;    // 返回每条的序号： 每页条数 *（当前页 - 1 ）+ 序号

        }

    }];

    //显示列的中文名称.
    if (typeof columns == "string" && columns != "") {
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
    } else {
        searchTableColumns.push({
            field: "No",
            title: "编号"

        });
        searchTableColumns.push({
            field: "Name",
            title: "名称"

        });
    }
    //debugger
    var ispagination = dataObj.length > 20 ? true : false;
    layui.use('table', function () {
        var table = layui.table;
        table.render({
            elem: "#autoTable",
            id: "autoTable",
            cols: [searchTableColumns],
            data: dataObj
        })
        //监听行单击事件（双击事件为：rowDouble）
        table.on('row(autoTable)', function (obj) {
            var data = obj.data;
            $("#" + tbid).val(data.No);
            $("#divInfo").remove();
            FullIt(data.No, mapExt.MyPK, tbid);

        });
    })

}

function isLegalName(name) {
    if (!name) {
        return false;
    }
    return name.match(/^[a-zA-Z\$_][a-zA-Z\d\$_]*$/);
}



function openDiv(e, tbID) {


    if (document.getElementById("divinfo").style.display == "none") {

        var txtObject = e; //  document.getElementById(tbID);
        var orgObject = document.getElementById("divinfo");
        var rect = getoffset(txtObject);
        var t = rect[0] + 22;
        var l = rect[1];

        orgObject.style.top = t + 'px';
        orgObject.style.left = l + 'px';
        orgObject.style.width = "";
        orgObject.style.border = "1px solid rgb(51, 102, 153)";
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

/* 内置的Pop自动返回值. */
function ReturnValCCFormPopVal(ctrl, fk_mapExt, refEnPK, width, height, title) {
    var wfpreHref = GetLocalWFPreHref();
    url = wfpreHref + '/WF/CCForm/PopVal.htm?FK_MapExt=' + fk_mapExt + '&RefPK=' + refEnPK + '&CtrlVal=' + ctrl.value;
    var v = window.showModalDialog(url, 'opp', 'scrollbars=yes;resizable=yes;center=yes;minimize:yes;maximize:yes;dialogHeight: ' + (height || 600) + 'px; dialogWidth: ' + (width || 850) + 'px; dialogTop: 100px; dialogLeft: 150px;');
    if (v == null || v == '' || v == 'NaN') {
        return;
    }
    ctrl.value = v;
    ctrl.value = v;
    return;
}

//根据Name设置元素的值  分为 tb,ddl,rd
function SetEleValByName(eleName, val) {
    var ele = $('[name$=_' + eleName + ']');
    if (ele != undefined && ele.length > 0) {
        switch (ele[0].tagName.toUpperCase()) {
            case "INPUT":
                switch (ele[0].type.toUpperCase()) {
                    case "CHECKBOX": //复选框  0:false  1:true
                        val.indexOf('1') >= 0 ? $(ele).attr('checked', true) : $(ele).attr('checked', false);
                        break;
                    case "TEXT": //文本框
                        $(ele).val(val);
                        break;
                    case "RADIO": //单选钮
                        $(ele).attr('checked', false);
                        $('[name=RB_' + eleName + '][value=' + val + ']').attr('checked', true);
                        break;
                    case "HIDDEN":
                        $(ele).val(val);
                        break;
                }
                break;
            //下拉框   
            case "SELECT":
                $(ele).val(val);
                break;
            //文本区域   
            case "TEXTAREA":
                $(ele).val(val);
                break;
        }
    }
}

function PopFullCtrl(val1, val2) {
    alert(val1, val2);
}

/* 内置的Pop自动返回值. google 版 软通*/

function ReturnValCCFormPopValGoogle(ctrl, fk_mapExt, refEnPK, width, height, title, formData, dtlNo, extType) {
    //设置摸态框的宽度和高度
    $('#returnPopValModal .modal-dialog').height(height);
    $('#returnPopValModal .modal-dialog').width(width);
    $('#returnPopValModal .modal-dialog').css('margin-left', 'auto');
    $('#returnPopValModal .modal-dialog').css('margin-right', 'auto');

    //ctrl = $('#' + ctrl);
    if (typeof ctrl == "string") {
        ctrl = document.getElementById(ctrl);
    }
    var wfpreHref = GetLocalWFPreHref();
    var fd;

    var dtlWin = dtlNo ? document.getElementById("F" + dtlNo).contentWindow : null;

    if (formData) {
        fd = formData;
    }
    else {
        fd = getFormData(false, false);
    }

    var url = "";

    if (extType == "PopVal" || extType == undefined)
        url = wfpreHref + '/WF/CCForm/PopVal.htm?FK_MapExt=' + fk_mapExt + '&RefPK=' + refEnPK + '&CtrlVal=' + ctrl.value + "&FormData=" + escape(fd) + "&m=" + Math.random();

    if (extType == "PopFullCtrl")
        url = wfpreHref + '/WF/CCForm/PopFullCtrl.htm?FK_MapExt=' + fk_mapExt + '&RefPK=' + refEnPK + '&CtrlVal=' + ctrl.value + "&FormData=" + escape(fd) + "&m=" + Math.random();

    //杨玉慧 模态框 先用这个.
    $('#returnPopValModal .modal-header h4').text("请选择：" + $(ctrl).parent().parent().prev().text());
    $('#iframePopModalForm').attr("src", url); //绑定连接.
    $('#btnPopValOK').unbind('click');
    $('#btnPopValOK').bind('click', function () {

        //$(ctrl).val("");
        setValForPopval(ctrl.id, dtlWin, "");

        //为表单元素反填值。
        var returnValSetObj = frames["iframePopModalForm"].window.pageSetData;
        var returnValObj = frames["iframePopModalForm"].window.returnVal;

        //设置值.
        if (extType == "PopFullCtrl") {
            PopFullCtrl(returnValSetObj, returnValObj);
            return;
        }

        if (returnValSetObj != null && returnValObj != null) {
            if (returnValSetObj[0].PopValWorkModel == "Tree" || returnValSetObj[0].PopValWorkModel == "TreeDouble") { //树模式 分组模式
                frames["iframePopModalForm"].window.GetTreeReturnVal();
                if (returnValSetObj[0].PopValFormat == "OnlyNo") {
                    setValForPopval(ctrl.id, dtlWin, returnValObj.No);
                } else if (returnValSetObj[0].PopValFormat == "OnlyName") {
                    setValForPopval(ctrl.id, dtlWin, returnValObj.Name);
                } else {
                    //
                    for (var property in returnValObj) {

                        SetEleValByName(property, returnValObj[property]);
                    }

                    setValForPopval(ctrl.id, dtlWin, returnValObj.Name);
                }
            } else if (returnValSetObj[0].PopValWorkModel == "Group") { //分组模式
                frames["iframePopModalForm"].window.GetGroupReturnVal();
                setValForPopval(ctrl.id, dtlWin, returnValObj.Value);
            } else if (returnValSetObj[0].PopValWorkModel == "TableOnly" ||
                returnValSetObj[0].PopValWorkModel == "TablePage") { //表格模式
                if (returnValSetObj[0].PopValFormat == "OnlyNo") {
                    $(ctrl).val(returnValObj.No);
                    setValForPopval(ctrl.id, dtlWin, returnValObj.No);
                } else if (returnValSetObj[0].PopValFormat == "OnlyName") {
                    //$(ctrl).val(returnValObj.Name);
                    setValForPopval(ctrl.id, dtlWin, returnValObj.Name);
                } else {
                    for (var property in returnValObj) {
                        SetEleValByName(property, returnValObj[property]);
                    }

                    setValForPopval(ctrl.id, dtlWin, returnValObj.Name);
                }
            } else if (returnValSetObj[0].PopValWorkModel == "SelfUrl") { //自定义URL
                if (frames["iframePopModalForm"].window.GetReturnVal != undefined &&
                    typeof (frames["iframePopModalForm"].window.GetReturnVal) == "function") {
                    frames["iframePopModalForm"].window.GetReturnVal()
                }
                setValForPopval(ctrl.id, dtlWin, returnValObj.Value);
            }
        } else {
            if (frames["iframePopModalForm"].window.returnValue != undefined) {
                var Value = frames["iframePopModalForm"].window.returnValue;
            }
            //$(ctrl).val(Value);
            setValForPopval(ctrl.id, dtlWin, Value);
        }

        //把树等都变成不显示 解决点击一个后另一个会把原来的先显示一下的问题
        $(frames["iframePopModalForm"].window.document.getElementById('poptablew')).css('display', 'none');
        $(frames["iframePopModalForm"].window.document.getElementById('main')).css('display', 'none');
        $(frames["iframePopModalForm"].window.document.getElementById('orgjstree')).css('display', 'none');
        $(frames["iframePopModalForm"].window.document.getElementById('groupTable')).css('display', 'none');

        // $(".jstree-clicked").removeClass("jstree-clicked");

    });
    $('#btnPopValCancel').unbind('click');
    $('#btnPopValCancel').bind('click', function () {
        //把树等都变成不显示 解决点击一个后另一个会把原来的先显示一下的问题
        $(frames["iframePopModalForm"].window.document.getElementById('poptablew')).css('display', 'none');
        $(frames["iframePopModalForm"].window.document.getElementById('main')).css('display', 'none');
        $(frames["iframePopModalForm"].window.document.getElementById('orgjstree')).css('display', 'none');
        $(frames["iframePopModalForm"].window.document.getElementById('groupTable')).css('display', 'none');
    })
    $('#returnPopValModal').modal().show();
    //修改标题，失去焦点时进行保存
    if (typeof self.parent.TabFormExists != 'undefined') {
        var bExists = self.parent.TabFormExists();
        if (bExists) {
            self.parent.ChangTabFormTitle();
        }
    }
    return;
}

/* 设置控件值，仅用在主表单/明细表（且为iframe内的）中设置控件值，目前仅用于Popval弹窗设置返回值 */
function setValForPopval(id, dtlWin, val) {
    if (dtlWin && dtlWin.SetTextboxValue) {
        dtlWin.SetTextboxValue(id, val);
    }
    else {
        $("#" + id).val(val);
    }
}

/*  ReturnValTBFullCtrl */
function ReturnValTBFullCtrl(ctrl, fk_mapExt) {
    var wfPreHref = GetLocalWFPreHref();
    var url = wfPreHref + '/WF/CCForm/FrmReturnValTBFullCtrl.aspx?CtrlVal=' + ctrl.value + '&FK_MapExt=' + fk_mapExt;
    var v = window.OpenLayuiDialog(url, 'wd', 'scrollbars=yes;resizable=yes;center=yes;minimize:yes;maximize:yes;dialogHeight: 650px; dialogWidth: 850px; dialogTop: 100px; dialogLeft: 150px;');
    if (v == null || v == '' || v == 'NaN') {
        return;
    }
    ctrl.value = v;
    // 填充.
    FullIt(oldValue, ctrl.id, fk_mapExt);
    return;
}




/* 自动填充 */
function DDLFullCtrl(selectVal, ddlChild, fk_mapExt) {

    FullIt(selectVal, fk_mapExt, ddlChild);
}

/* 级联下拉框  param 传到后台的一些参数  例如从表的行数据 主表的字段值 如果param参数在，就不去页面中取KVS 了，PARAM 就是*/
function DDLAnsc(selectVal, ddlChild, fk_mapExt, param) {

    //1.初始值为空或者NULL时，相关联的字段没有数据显示
    if (selectVal == null || selectVal == "") {
        $("#" + ddlChild).empty();
        //无数据返回时，提示显示无数据，并将与此关联的下级下拉框也处理一遍，edited by liuxc,2015-10-22
        $("#" + ddlChild).append("<option value='' selected='selected' ></option");
        var chg = $("#" + ddlChild).attr("onchange");

        $("#" + ddlChild).change();

        return;
    }
    if (selectVal == "all") {
        $("#" + ddlChild).empty();
        //无数据返回时，提示显示无数据，并将与此关联的下级下拉框也处理一遍，edited by liuxc,2015-10-22
        $("#" + ddlChild).append("<option value='all' selected='selected' >全部</option");
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
        $("#" + ddlChild).append("<option value='' selected='selected' ></option");
        var chg = $("#" + ddlChild).attr("onchange");

        if (typeof chg == "function") {
            $("#" + ddlChild).change();
        }
        return;
    }

    //不为空的时候赋值
    $.each(dataObj, function (idx, item) {

        //  alert(item[idx][1]);
        //console.log(item);
        //return;

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


//隐藏自动填充的DIV
function hiddenDiv() {
    $("#divinfo").empty();
    $("#divinfo").css("display", "none");
    $("#autodiv").remove();
}
var itemStyle = 'padding:2px;color: #000000;background-color:white;width:100%;border-bottom: 1px solid #336699;';
var itemStyleOfS = 'padding:2px;color: #000000;background-color:green;width:100%';
function ItemClick(sender, val, tbid, fk_mapExt) {

    $("#autodiv").remove();
    //$("#divinfo").css("display", "none");
    highlightindex = -1;
    oldValue = val;

    $("#" + tbid).val(oldValue);

    // 填充.
    FullIt(oldValue, fk_mapExt, tbid);

}

function MyOver(sender) {
    if (highlightindex != -1) {
        $("#autoComplete").children("div").eq(highlightindex).css("background-color", "white");
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

//输入检查
function txtTest_Onkeyup(ele, filter, message) {
    if (ele == null) return;
    var re = filter;
    if (typeof (filter) == "string") {
        re = new RegExp(filter);
    }
    var format = re.test(ele.value);
    if (!format) {
        ele.value = "";
        alert(message);
    }
}
//输入检查
function EleInputCheck(ele, filter, message) {
    if (ele == null) return;
    if (CheckInput(ele.value, filter) == true) {
        ele.title = "";
        ele.style.border = "1";
        ele.style.backgroundColor = "White";
        ele.style.borderBottomColor = "Black";
    }
    else {
        ele.title = message;
        ele.style.border = "2";
        ele.style.backgroundColor = "#FFDEAD";
        ele.style.borderBottomColor = "Red";
    }
}
function EleInputCheck2(ele, filter, message) {
    if (ele == null) return;

    var reg = new RegExp(filter);
    var isPass = reg.test(ele.value);
    if (isPass == true) {
        ele.title = "";
        ele.style.border = "1";
        ele.style.backgroundColor = "White";
        ele.style.borderBottomColor = "Black";
    }
    else {
        ele.title = message;
        ele.style.border = "2";
        ele.style.backgroundColor = "#FFDEAD";
        ele.style.borderBottomColor = "Red";
    }
}

function EleSubmitCheck(ele, message) {
    ele.title = message;
    ele.style.border = "2";
    ele.style.backgroundColor = "#FFDEAD";
    ele.style.borderBottomColor = "Red";
}

//保存检查
function EleSubmitCheck(ele, filter, message) {
    if (ele == null) return;
    if (CheckInput(ele.value, filter) == true) {
        ele.title = "";
        ele.className = "TB";
        return true;
    }
    else {
        ele.title = message;
        ele.style.border = "2";
        ele.style.backgroundColor = "#FFDEAD";
        ele.style.borderBottomColor = "Red";
        return false;
    }
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

/**
* 填充其他控件
* @param selectVal 选中的值
* @param refPK mapExt关联的主键
* @param elementId 元素ID
*/
function FullIt(selectVal, refPK, elementId) {
    if (oid == null)
        oid = GetQueryString('OID');

    if (oid == null)
        oid = GetQueryString('WorkID');

    if (oid == null)
        oid = GetQueryString("RefPKVal");

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
    FullCtrl(selectVal, elementId, mapExt);

    //执行个性化填充下拉框，比如填充ddl下拉框的范围.
    FullCtrlDDL(selectVal, elementId, mapExt);

    //执行填充从表.
    FullDtl(selectVal, mapExt);

    //执行确定后执行的JS
    var backFunc = mapExt.Tag2;
    if (backFunc != null && backFunc != "" && backFunc != undefined)
        DBAccess.RunFunctionReturnStr(DealSQL(backFunc, selectVal));
}


/**
*  填充主表数据信息.
*/
function FullCtrl(selectVal, ctrlIdBefore, mapExt) {
    var dbSrc = mapExt.Doc;
    if (dbSrc == null || dbSrc == "") {
        // alert("没有配置填充主表的信息");
        return;
    }

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
    var isDtlField = endId !== "" ? true : false;

    var dataObj = GenerDB(dbSrc, selectVal, mapExt.DBType, mapExt.FK_DBSrc, isDtlField);

    TableFullCtrl(dataObj, ctrlIdBefore, beforeID, endId);

    //如果含有FullDataDtl也需要处理
    var mapExts = new Entities("BP.Sys.MapExts");
    mapExts.Retrieve("FK_MapData", mapExt.FK_MapData, "AttrOfOper", mapExt.AttrOfOper, "ExtType", "FullDataDtl");
    for (var i = 0; i < mapExts.length; i++) {
        var item = mapExts[i];
        var dbSrc = item.Doc;
        if (dbSrc != null && dbSrc != "") {
            var dataObj = GenerDB(dbSrc, selectVal, item.DBType, item.FK_DBSrc, isDtlField);

            TableFullCtrl(dataObj, ctrlIdBefore, beforeID, endId);
        }

    }
}

function TableFullCtrl(dataObj, ctrlIdBefore, beforeID, endId) {

    if ($.isEmptyObject(dataObj)) {
        // alert('系统错误不应该查询不到数据:'+dbSrc);
        return;
    }

    var data = dataObj[0]; //获得这一行数据.
    if (typeof frmMapAttrs != "undefined" && frmMapAttrs != null && frmMapAttrs.length != 0)
        data = DealDataTableColName(data, frmMapAttrs);
    else
        data = DealDataTableColName(data, mapAttrs);
    //遍历属性，给属性赋值.
    var valID;
    var tbs;
    var selects;
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
            if (tbs == undefined || tbs == "") {
                if (endId != "") {
                    //获取所在的行
                    tbs = $("#" + ctrlIdBefore).parent().parent().find("input")
                }
                else
                    tbs = $('input');
            }
            if (selects == undefined || selects == "") {
                if (endId != "") {
                    //获取所在的行
                    selects = $("#" + ctrlIdBefore).parent().parent().find("select")
                }
                else
                    selects = $('select');
            }

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

/**填充下拉框信息**/
function FullCtrlDDL(selectVal, ctrlID, mapExt) {

    var doc = mapExt.Tag;
    if (doc == "" || doc == null)
        return;
    if (mapExt.DBType == 0) {
        //按照SQL
        var dbs = GetDataTableByDB(src, mapExt.DBType, mapExt.FK_DBSrc, selectVal, mapExt, "Tag");
        for (var key in dbs)
            GenerBindDDL("DDL_" + key, dbs[key]);
        layui.form.render("select");
        return;
    }
    var dbSrcs = doc.split('$'); //获得集合.

    for (var i = 0; i < dbSrcs.length; i++) {

        var dbSrc = dbSrcs[i];
        if (dbSrc == "" || dbSrc.length == 0)
            continue;
        var ctrlID = dbSrc.substring(0, dbSrc.indexOf(':'));
        var src = dbSrc.substring(dbSrc.indexOf(':') + 1);

        var db = GenerDB(src, selectVal, mapExt.DBType, mapExt.FK_DBSrc); //获得数据源.

        //重新绑定下拉框.
        GenerBindDDL("DDL_" + ctrlID, db);
    }
    layui.form.render("select");
}
//填充明细.
function FullDtl(selectVal, mapExt) {
    if (mapExt.Tag1 == "" || mapExt.Tag1 == null)
        return;

    var dbType = mapExt.DBType;
    var dbSrc = mapExt.Tag1;
    var url = GetLocalWFPreHref();
    var dataObj;

    if (dbType == 3) {

        dbSrc = DealSQL(DealExp(dbSrc), e, kvs);
        dataObj = DBAccess.RunDBSrc(dbSrc, 1);

        //JQuery 获取数据源
    } else if (dbType == 2) {
        dataObj = DBAccess.RunDBSrc(dbSrc, 2);
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

    for (var i in dataObj.Head) {
        if (typeof (i) == "function")
            continue;

        for (var k in dataObj.Head[i]) {
            var fullDtl = dataObj.Head[i][k];
            //  alert('您确定要填充从表吗?，里面的数据将要被删除。' + key + ' ID= ' + fullDtl);
            var frm = document.getElementById('Dtl_' + fullDtl);

            var src = frm.src;
            if (src != undefined || src != null) {
                var idx = src.indexOf("&Key");
                if (idx == -1)
                    src = src + '&Key=' + selectVal + '&FK_MapExt=' + mapExt.MyPK;
                else
                    src = src.substring(0, idx) + '&ss=d&Key=' + selectVal + '&FK_MapExt=' + mapExt.MyPK;
                frm.src = src;
            }
        }
    }
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

/**处理SQL语句/函数/**/
function GenerDB(dbSrc, selectVal, dbType, dbSource, isDtlField) {


    //处理sql，url参数.
    dbSrc = dbSrc.replace(/~/g, "'");
    if (dbType == 0 && typeof selectval === 'string')
        selectVal = selectVal.replace(/'/g, '');

    dbSrc = dbSrc.replace(/@Key/g, selectVal);
    dbSrc = dbSrc.replace(/@key/g, selectVal);
    dbSrc = dbSrc.replace(/@KEY/g, selectVal);

    dbSrc = DealExp(dbSrc, null, isDtlField);
    dbSrc = DealSQL(dbSrc, selectVal, kvs);

    //获取数据源.
    dataObj = DBAccess.RunDBSrc(dbSrc, dbType, dbSource);
    return dataObj;
}

function DealSQL(dbSrc, key, kvs) {

    if (dbSrc.indexOf('@') == -1)
        return dbSrc;

    dbSrc = dbSrc.replace(/~/g, "'");

    dbSrc = dbSrc.replace(/@Key/g, key);
    dbSrc = dbSrc.replace(/@key/g, key);
    dbSrc = dbSrc.replace(/@KEY/g, key);

    dbSrc = dbSrc.replace(/@Val/g, key);
    dbSrc = dbSrc.replace(/\n/g, "");
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

    return dbSrc;
}
function isLegalName(name) {
    if (!name) {
        return false;
    }
    return name.match(/^[a-zA-Z\$_][a-zA-Z\d\$_]*$/);
}

/**
 * 获取页面数据
 * */
function getPageData() {
    var formss = $('#divCCForm').serialize() || "";
    var params = "";
    var formArr = formss.split('&');
    var formArrResult = [];
    $.each(formArr, function (i, ele) {
        if (ele.split('=')[0].indexOf('CB_') == 0) {
            //如果ID获取不到值，Name获取到值为复选框多选
            var targetId = ele.split('=')[0];
            if ($('#' + targetId).length == 1) {
                if ($('#' + targetId + ':checked').length == 1) {
                    ele = targetId.replace("CB_", "") + '=1';
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
            params += "@" + ele.replace("DDL_", "");
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
                        params += "@" + name.replace("CB_", "") + '=' + $(disabledEle).is(':checked') ? 1 : 0;

                        break;
                    case "TEXT": //文本框
                    case "HIDDEN":
                        params += "@" + name.replace("TB_", "") + '=' + $(disabledEle).val();
                        break;
                    case "RADIO": //单选钮
                        name = $(disabledEle).attr('name');
                        var eleResult = name + '=' + $('[name="' + name + '"]:checked').val();
                        params += "@" + eleResult.replace("RB_", "");
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

