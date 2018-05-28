// ********************** 根据关键字动态查询. ******************************** //
var oldValue = "";
var oid;
var highlightindex = -1;
function DoAnscToFillDiv(sender, e, tbid, fk_mapExt, dbSrc, dbType) {
    openDiv(sender, tbid);

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
            FullIt(oldValue, tbid, fk_mapExt, dbSrc, dbType);
            highlightindex = -1;
        }
    }
    else {
        if (e != oldValue) {
            $("#divinfo").empty();
            var url = GetLocalWFPreHref();

            var dataObj;
            //URL 获取数据源
            if (dbType == 1) {
                dbSrc = DealSQL(getDocOfSQLDeal(dbSrc), e, kvs);
                dataObj = DBAccess.RunDBSrc(dbSrc, 1);

                //JQuery 获取数据源
            } else if (dbType == 2) {
                dataObj = DBAccess.RunDBSrc(dbSrc, 2);
            } else {
                var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
                handler.AddPara("Key", e);
                handler.AddPara("FK_MapExt", fk_mapExt);
                handler.AddPara("KVs", kvs);
                var data = handler.DoMethodReturnString("HandlerMapExt");
                if (data.indexOf('err@') >= 0) {
                    alert(data);
                    $("#divinfo").hide();
                    return;
                }

                if (data == "") {
                    $("#divinfo").hide();
                    return;
                }
                dataObj = eval("(" + data + ")"); //转换为json对象 
            }


            highlightindex = -1;
            if (dataObj.Head.length == 0) {
                $("#divinfo").hide();
            }

            $.each(dataObj.Head, function (idx, item) {
                $("#divinfo").append("<div style='" + itemStyle + "' name='" + idx + "' onmouseover='MyOver(this)' onmouseout='MyOut(this)' onclick=\"ItemClick('" + sender.id + "','" + item.No + "','" + tbid + "','" + fk_mapExt + "','" + dbSrc + "','" + dbType + "');\" value='" + item.No + "'>" + item.No + '|' + item.Name + "</div>");
            });
            oldValue = e;
        }
    }
}

function FullIt(oldValue, tbid, fk_mapExt, dbSrc, dbType) {

    if (oid == null)
        oid = GetQueryString('OID');

    if (oid == null)
        oid = GetQueryString('WorkID');

    if (oid == null)
        oid = 0;

    //执行填充主表的控件.
    FullCtrl(oldValue, tbid, fk_mapExt, dbSrc, dbType);

    //执行个性化填充下拉框.
    FullCtrlDDL(oldValue, tbid, fk_mapExt, dbSrc, dbType);

    //执行填充从表.
    FullDtl(oldValue, fk_mapExt, dbSrc, dbType);

    //执行m2m 关系填充.
    FullM2M(oldValue, fk_mapExt, dbSrc, dbType);
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
    //update by dgq 修改路径
    //url = 'CCForm/FrmPopVal.aspx?FK_MapExt=' + fk_mapExt + '&RefPK=' + refEnPK + '&CtrlVal=' + ctrl.value;

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
    debugger
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

        //为表单元素反填值
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
                    //$(ctrl).val(returnValObj.Name);
                    setValForPopval(ctrl.id, dtlWin, returnValObj.Name);
                } else {
                    // ??????????
                    for (var property in returnValObj) {
                        //$('[id$=_' + property + ']').val(returnValObj[property]);

                        SetEleValByName(property, returnValObj[property]);
                    }

                    setValForPopval(ctrl.id, dtlWin, returnValObj.Name);
                }
            } else if (returnValSetObj[0].PopValWorkModel == "Group") { //分组模式
                frames["iframePopModalForm"].window.GetGroupReturnVal();
                //alert(returnValObj.Value + "|" + ctrl.id);
                //$(ctrl).val(returnValObj.Value);
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
                    // ??????????
                    for (var property in returnValObj) {
                        //$('[id$=_' + property + ']').val(returnValObj[property]);
                        SetEleValByName(property, returnValObj[property]);
                    }

                    setValForPopval(ctrl.id, dtlWin, returnValObj.Name);
                }
            } else if (returnValSetObj[0].PopValWorkModel == "SelfUrl") { //自定义URL
                //frames["iframePopModalForm"].window.GetTreeReturnVal();
                if (frames["iframePopModalForm"].window.GetReturnVal != undefined &&
                    typeof (frames["iframePopModalForm"].window.GetReturnVal) == "function") {
                    frames["iframePopModalForm"].window.GetReturnVal()
                }
                //$(ctrl).val(returnValObj.Value);
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
    var v = window.showModalDialog(url, 'wd', 'scrollbars=yes;resizable=yes;center=yes;minimize:yes;maximize:yes;dialogHeight: 650px; dialogWidth: 850px; dialogTop: 100px; dialogLeft: 150px;');
    if (v == null || v == '' || v == 'NaN') {
        return;
    }
    ctrl.value = v;
    // 填充.
    FullIt(oldValue, ctrl.id, fk_mapExt);
    return;
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


/* 自动填充 */
function DDLFullCtrl(e, ddlChild, fk_mapExt, dbSrc,dbType) {

    GenerPageKVs();
    var url = GetLocalWFPreHref();

    var dataObj;
    //URL 获取数据源
    if (dbType == 1) {
        dbSrc = DealSQL(getDocOfSQLDeal(dbSrc), e, kvs);
        dataObj = DBAccess.RunDBSrc(dbSrc, 1);

        //JQuery 获取数据源
    } else if (dbType == 2) {
        dataObj = DBAccess.RunDBSrc(dbSrc, 2);
    } else {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddPara("Key", e);
        handler.AddPara("FK_MapExt", fk_mapExt);
        handler.AddPara("KVs", kvs);
        var data = handler.DoMethodReturnString("HandlerMapExt");
        if (data.indexOf('err@') == 0) {
            alert(data);
            return;
        }
        dataObj = eval("(" + data + ")"); //转换为json对象 
    }

    for (var i in dataObj.Head) {
        if (typeof (i) == "function")
            continue;
        FullIt(e, ddlChild, fk_mapExt);
        return;
    }
}

/* 级联下拉框  param 传到后台的一些参数  例如从表的行数据 主表的字段值 如果param参数在，就不去页面中取KVS 了，PARAM 就是*/
function DDLAnsc(e, ddlChild, fk_mapExt, dbSrc, dbType, param) {
    if (e == null || e == "") {
        $("#" + ddlChild).empty();
        //无数据返回时，提示显示无数据，并将与此关联的下级下拉框也处理一遍，edited by liuxc,2015-10-22
        $("#" + ddlChild).append("<option value='' selected='selected' >无数据</option");
        var chg = $("#" + ddlChild).attr("onchange");

        $("#" + ddlChild).change();

        return;
    }

    GenerPageKVs();
    var url = GetLocalWFPreHref();
    if (param != undefined) {
        kvs = '';
    }

    var dataObj = "";
    //Url获取数据源
    if (dbType == 1) {
        dbSrc = DealSQL(getDocOfSQLDeal(dbSrc), e, kvs);
        dataObj = DBAccess.RunDBSrc(dbSrc, 1);

        //JQuery 获取数据源
    } else if (dbType == 2) {
        dataObj = DBAccess.RunDBSrc(dbSrc, 2);

        //SQL 获取数据源
    } else {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddPara("Key", e);
        handler.AddPara("FK_MapExt", fk_mapExt);
        handler.AddPara("KVs", kvs);
        if (param != undefined) {
            for (var pro in param) {
                if (pro == 'DoType')
                    continue;
                handler.AddPara(pro, param[pro]);
            }
        }
        var data = handler.DoMethodReturnString("HandlerMapExt");
        if (data.indexOf('err@') == 0) {
            alert(data);
            return;
        }
        dataObj = eval("(" + data + ")"); //转换为json对象.
    }


    // 这里要设置一下获取的外部数据.
    // var seleValOfOld = $("#" + ddlChild).selectedindex;
    // alert(seleValOfOld);
    // 获取原来选择值.

    var oldVal = null;
    var ddl = document.getElementById(ddlChild);
    var mylen = ddl.options.length - 1;
    while (mylen >= 0) {
        if (ddl.options[mylen].selected) {
            oldVal = ddl.options[mylen].value;
        }
        mylen--;
    }

    $("#" + ddlChild).empty();

    if (data == "" || dataObj == null) {
        //无数据返回时，提示显示无数据，并将与此关联的下级下拉框也处理一遍，edited by liuxc,2015-10-22
        $("#" + ddlChild).append("<option value='' selected='selected' >无数据</option");
        var chg = $("#" + ddlChild).attr("onchange");

        if (typeof chg == "function") {
            $("#" + ddlChild).change();
        }
        return;
    }


    $.each(dataObj.Head, function (idx, item) {
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
    if (isInIt == false) {
        //此处修改，去掉直接选中上次的结果，避免错误数据的产生，edited by liuxc,2015-10-22
        $("#" + ddlChild).prepend("<option value='' selected='selected' >*请选择</option");
        $("#" + ddlChild).val('');
        var chg = $("#" + ddlChild).attr("onchange");
        $("#" + ddlChild).change();

    }


}


function FullM2M(key, fk_mapExt, dbSrc, dbType) {
    GenerPageKVs();
    var url = GetLocalWFPreHref();
    var dataObj;
    if (dbType == 1) {
        dbSrc = DealSQL(getDocOfSQLDeal(dbSrc), e, kvs);
        dataObj = DBAccess.RunDBSrc(dbSrc, 1);

        //JQuery 获取数据源
    } else if (dbType == 2) {
        dataObj = DBAccess.RunDBSrc(dbSrc, 2);
    } else {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddPara("Key", e);
        handler.AddPara("FK_MapExt", fk_mapExt);
        handler.AddPara("KVs", kvs);
        handler.AddPara("DoTypeExt", "ReqM2MFullList");
        handler.AddPara("OID", oid);
        var data = handler.DoMethodReturnString("HandlerMapExt");
        if (data == "")
            return;

        if (data.indexOf('err@') == 0) {
            alert(data);
            return;
        }
        dataObj = eval("(" + data + ")"); //转换为json对象 	
    }
    for (var i in dataObj.Head) {
        if (typeof (i) == "function")
            continue;

        for (var k in dataObj.Head[i]) {
            var fullM2M = dataObj.Head[i][k];
            var frm = document.getElementById('F' + fullM2M);
            if (frm == null)
                continue;

            var src = frm.src;
            var idx = src.indexOf("&Key");
            if (idx == -1)
                src = src + '&Key=' + key + '&FK_MapExt=' + fk_mapExt;
            else
                src = src.substring(0, idx) + '&Key=' + key + '&FK_MapExt=' + fk_mapExt;
            frm.src = src;
        }
    }

}

//填充明细.
function FullDtl(key, fk_mapExt, dbSrc, dbType) {
    GenerPageKVs();
    var url = GetLocalWFPreHref();
    var dataObj;
    if (dbType == 1) {
        dbSrc = DealSQL(getDocOfSQLDeal(dbSrc), e, kvs);
        dataObj = DBAccess.RunDBSrc(dbSrc, 1);

        //JQuery 获取数据源
    } else if (dbType == 2) {
        dataObj = DBAccess.RunDBSrc(dbSrc, 2);
    } else {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddPara("Key", e);
        handler.AddPara("FK_MapExt", fk_mapExt);
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
        dataObj = eval("(" + data + ")"); //转换为json对象 	
    }

    for (var i in dataObj.Head) {
        if (typeof (i) == "function")
            continue;

        for (var k in dataObj.Head[i]) {
            var fullDtl = dataObj.Head[i][k];
            //  alert('您确定要填充从表吗?，里面的数据将要被删除。' + key + ' ID= ' + fullDtl);
            var frm = document.getElementById('F' + fullDtl);
            var src = frm.src;
            var idx = src.indexOf("&Key");
            if (idx == -1)
                src = src + '&Key=' + key + '&FK_MapExt=' + fk_mapExt;
            else
                src = src.substring(0, idx) + '&ss=d&Key=' + key + '&FK_MapExt=' + fk_mapExt;
            frm.src = src;
        }
    }

}

function FullCtrlDDL(key, ctrlIdBefore, fk_mapExt, dbSrc, dbType) {
    GenerPageKVs();
    var url = GetLocalWFPreHref();
    var dataObj;
    if (dbType == 1) {
        dbSrc = DealSQL(getDocOfSQLDeal(dbSrc), e, kvs);
        dataObj = DBAccess.RunDBSrc(dbSrc, 1);

        //JQuery 获取数据源
    } else if (dbType == 2) {
        dataObj = DBAccess.RunDBSrc(dbSrc, 2);
    } else {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddPara("Key", e);
        handler.AddPara("FK_MapExt", fk_mapExt);
        handler.AddPara("KVs", kvs);
        handler.AddPara("DoTypeExt", "ReqDDLFullList");
        var data = handler.DoMethodReturnString("HandlerMapExt");
        if (data == "")
            return;

        if (data.indexOf('err@') == 0) {
            alert(data);
            return;
        }
        dataObj = eval("(" + data + ")"); //转换为json对象 	
    }

    var beforeID = ctrlIdBefore.substring(0, ctrlIdBefore.indexOf('DDL_'));
    var endId = ctrlIdBefore.substring(ctrlIdBefore.lastIndexOf('_'));

    for (var i in dataObj.Head) {
        if (typeof (i) == "function")
            continue;

        for (var k in dataObj.Head[i]) {
            var fullDDLID = dataObj.Head[i][k];

            //alert(fullDDLID);
            FullCtrlDDLDB(key, fullDDLID, beforeID, endId, fk_mapExt, dbSrc, dbType);
        }
    }

}

function FullCtrlDDLDB(e, ddlID, ctrlIdBefore, endID, fk_mapExt, dbSrc, dbType) {
    GenerPageKVs();
    var url = GetLocalWFPreHref();

    var dataObj;
    if (dbType == 1) {
        dbSrc = DealSQL(getDocOfSQLDeal(dbSrc), e, kvs);
        dataObj = DBAccess.RunDBSrc(dbSrc, 1);

        //JQuery 获取数据源
    } else if (dbType == 2) {
        dataObj = DBAccess.RunDBSrc(dbSrc, 2);
    } else {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddPara("Key", e);
        handler.AddPara("FK_MapExt", fk_mapExt);
        handler.AddPara("KVs", kvs);
        handler.AddPara("DoTypeExt", "ReqDDLFullListDB");
        handler.AddPara("MyDDL", ddlID);
        var data = handler.DoMethodReturnString("HandlerMapExt");
        if (data.indexOf("err@") >= 0) {
            alert(data);
            return;
        }
        dataObj = eval("(" + data + ")"); //转换为json对象 	
    }

    endID = endID.replace('_', '');
    if (endID != parseInt(endID)) {
        endID = "";
    } else {
        endID = "_" + endID;
    }
    var id = ctrlIdBefore + "DDL_" + ddlID + "" + endID;
    $("*[id$=" + id + "]").empty();
    //$("#" + id).empty();
    var dataObj = eval("(" + data + ")"); //转换为json对象 

    $.each(dataObj.Head, function (idx, item) {
        $("*[id$=" + id + "]").append("<option value='" + item.No + "'>" + item.Name + "</option");
    });

}

//文本框自动填充
function FullCtrl(e, ctrlIdBefore, fk_mapExt, dbSrc, dbType) {
    e = escape(e);
    GenerPageKVs();

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

    var dataObj;
    if (dbType == 1) {
        dbSrc = DealSQL(getDocOfSQLDeal(dbSrc), e, kvs);
        dataObj = DBAccess.RunDBSrc(dbSrc, 1);

        //JQuery 获取数据源
    } else if (dbType == 2) {
        dataObj = DBAccess.RunDBSrc(dbSrc, 2);
    } else {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddPara("Key", e);
        handler.AddPara("FK_MapExt", fk_mapExt);
        handler.AddPara("KVs", kvs);
        handler.AddPara("DoTypeExt", "ReqCtrl");
        var data = handler.DoMethodReturnString("HandlerMapExt");
        if (data.indexOf("err@") >= 0) {
            alert(data);
            return;
        }
        dataObj = eval("(" + data + ")"); //转换为json对象 	
    }

    for (var i in dataObj.Head) {
        if (typeof (i) == "function")
            continue;

        for (var k in dataObj.Head[i]) {
            if (k == 'No' || k == 'Name')
                continue;

            //  alert(k + ' val= ' + dataObj.Head[i][k]);

            $("#" + beforeID + 'TB_' + k).val(dataObj.Head[i][k]);
            $("#" + beforeID + 'TB_' + k + endId).val(dataObj.Head[i][k]);

            $("#" + beforeID + 'DDL_' + k).val(dataObj.Head[i][k]);
            $("#" + beforeID + 'DDL_' + k + endId).val(dataObj.Head[i][k]);

            if (dataObj.Head[i][k] == '1') {
                $("#" + beforeID + 'CB_' + k).attr("checked", true);
                $("#" + beforeID + 'CB_' + k + endId).attr("checked", true);
            } else {
                $("#" + beforeID + 'CB_' + k).attr("checked", false);
                $("#" + beforeID + 'CB_' + k + endId).attr("checked", false);
            }
        }
    }
}

var itemStyle = 'padding:2px;color: #000000;background-color:white;width:100%;border-bottom: 1px solid #336699;';
var itemStyleOfS = 'padding:2px;color: #000000;background-color:green;width:100%';
function ItemClick(sender, val, tbid, fk_mapExt, dbSrc, dbType) {

    $("#divinfo").empty();
    $("#divinfo").css("display", "none");
    highlightindex = -1;
    oldValue = val;

    $("#" + tbid).val(oldValue);

    // 填充.
    FullIt(oldValue, tbid, fk_mapExt, dbSrc, dbType);
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

// 获取参数.
function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null)
        return unescape(r[2]);
    return null;
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
    if (!result) {//alert(tipInfo);
        $("[name=" + oInput + ']').addClass('errorInput');
    } else {
        $("[name=" + oInput + ']').removeClass('errorInput');
    }
    return result;
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
    var url = window.location.href;
    if (url.indexOf('/WF/') >= 0) {
        var index = url.indexOf('/WF/');
        url = url.substring(0, index);
    } else {
        url = "";
    }
    return url;
}


function getDocOfSQLDeal(dbSrc) {
    var webUser = new WebUser();
    dbSrc = dbSrc.replace("WebUser.No", webUser.No);
    dbSrc = dbSrc.replace("@WebUser.Name", webUser.Name);
    dbSrc = dbSrc.replace("@WebUser.FK_DeptNameOfFull", webUser.FK_DeptNameOfFull);
    dbSrc = dbSrc.replace("@WebUser.FK_DeptName", webUser.FK_DeptName);
    dbSrc = dbSrc.replace("@WebUser.FK_Dept", webUser.FK_Dept);
    return dbSrc;
}

function DealSQL(dbSrc, key, kvs) {

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

    }
    return dbSrc;
}
