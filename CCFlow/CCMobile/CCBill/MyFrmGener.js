$(function () {
    var workID = GetQueryString("WorkID");
    workID = workID == null || workID == undefined || workID == "" ? 0 : workID;
    if (workID == 0) {
        //生成WorkID
        var handler = new HttpHandler("BP.CCBill.WF_CCBill");
        handler.AddUrlData();
        var workID = handler.DoMethodReturnString("MyDict_CreateBlankDictID");
        if (workID.indexOf('err@') > -1) {
            layer.alert(workID);
            return;
        }

        // window.location.reload();
        // Reload();
        Reload();
    }
    InitParam();
    ToolBar_Init(2);
    //相关功能
    RefMethod();
});
var pageData = {};
function InitParam() {
    pageData.IsReadonly = GetQueryString("IsReadonly");
    pageData.WorkID = GetQueryString("WorkID");
    pageData.FK_MapDataa = GetQueryString("FrmID");
    pageData.FrmID = GetQueryString("FrmID");
}
/**
 * 初始化按钮的操作
 */
function ToolBar_Init(entityType) {
    var handler = new HttpHandler("BP.CCBill.WF_CCBill_API");
    handler.AddPara("FrmID", GetQueryString("FrmID"));
    handler.AddPara("IsReadonly", GetQueryString("IsReadonly"));
    handler.AddPara("IsMobile", 1);
    var data = handler.DoMethodReturnString("CCFrom_ToolBar_Init");
    if (data.indexOf("err@") != -1) {
        layer.alert(data);
        return;
    }
    data = JSON.parse(data);
    
    $(".mui-page").css("bottom", "0px");
    $(".mui-pages").css("bottom", "0px");
    if (data.length == 0) {
        $("#bottomToolBar").hide();
        return;
    }
       
   
    var bottombar = $('#bottomToolBar');
    var popoverBar = $('#popoverBar');
    var barcount = 0;
    var methodNo = GetQueryString("MethodNo");
    methodNo = methodNo == null || methodNo == undefined ? "" : methodNo;
    var pworkid = GetQueryString("PWorkID");
    pworkid = pworkid == null || pworkid == undefined ? 0 : pworkid;
    $.each(data, function (i, btn) {
        barcount++;
        //增加按钮操作
        if (barcount == 4) {
            bottombar.append('<a class="mui-tab-item" href="#Popover">更多</a>');
            barcount++;
        }
        if (barcount < 4)
            bottombar.append("<a class='mui-tab-item' id='" + btn.BtnID + "' name='" + btn.BtnID + "' href='#' >" + btn.BtnLab + "</ a>");

        if (barcount > 4)
            popoverBar.append("<li class='mui-table-view-cell'><a id='" + btn.BtnID + "' name='" + btn.BtnID + "' href='#' >" + btn.BtnLab + "</ a></li>");
        $("#" + btn.BtnID).on("tap", function () {
            switch (this.name) {
                case "New":
                    if (methodNo != "" && pworkid != 0) {
                        window.location.href = "./Opt/GotoLink.htm?FrmID=" + GetQueryString("FrmID") + "&MethodNo=" + methodNo + "&WorkID=" + pworkid + "&DoType=Bill";
                        return;
                    }
                    var handler = new HttpHandler("BP.CCBill.WF_CCBill");
                    handler.AddUrlData();
                    var data = "";
                    if (entityType == 1)
                        data = handler.DoMethodReturnString("MyBill_CreateBlankBillID");
                    else
                        data = handler.DoMethodReturnString("MyDict_CreateBlankDictID");
                    if (data != null && data != undefined && data.indexOf('err@') > 0) {
                        layer.alert(data);
                        return;
                    }
                    if (entityType == 1)
                        window.location.href = 'MyBill.htm?FrmID=' + GetQueryString("FrmID") + "&WorkID=" + data;
                    else
                        window.location.href = 'MyDict.htm?FrmID=' + GetQueryString("FrmID") + "&WorkID=" + data;

                    break;
                case "Save":
                    //保存信息
                    //保存前事件
                    if (typeof beforeSave != 'undefined' && beforeSave() instanceof Function)
                        if (beforeSave() == false)
                            return false;
                    //必填项和正则表达式检查.
                    if (checkBlanks() == false) {
                        mui.alert("请输入必填项！");
                        return false;
                    }

                    if (checkReg() == false) {
                        mui.alert("正则验证错误，请检查边框变红字段！");
                        return false;
                    }
                    var params = getFormData(true, true, "divCCForm", false);
                    var handler = new HttpHandler("BP.CCBill.WF_CCBill");
                    handler.AddUrlData();
                    handler.AddJson(params);
                    var data = "";
                    if (entityType == 1)
                        data = handler.DoMethodReturnString("MyBill_SaveIt");
                    else
                        data = handler.DoMethodReturnString("MyDict_SaveIt");
                    this.innerHTML = "<i class='iconfont icon-baocun'></i>保存";
                    if (data.indexOf("err@") != -1) {
                        mui.alert(data);
                        return false;
                    }
                    alert("保存成功");
                    return false;

                    break;
                case "Submit":
                    //保存信息
                    //发送提交前
                    if (typeof beforeSend != 'undefined' && beforeSend instanceof Function)
                        if (beforeSend() == false)
                            return false; 

                    var params = getFormData(true, true, "divCCForm", false);
                    handler.AddUrlData();
                    handler.AddJson(params);
                    var data = "";
                    if (entityType == 1)
                        data = handler.DoMethodReturnString("MyBill_Submit");
                    else
                        data = handler.DoMethodReturnString("MyDict_Submit");
                    this.innerHTML = "<i class='iconfont icon-baocun'></i>提交";
                    if (data.indexOf("err@") != -1) {
                        alert(data);
                        return false;
                    }
                    layer.alert("提交成功");
                    break;
                case "Delete":
                    if (window.confirm("'您确定要删除吗?'") == false)
                        return;
                    var handler = new HttpHandler("BP.CCBill.WF_CCBill");
                    handler.AddUrlData();

                    //增加删除前事件
                    if (typeof beforeDelete != 'undefined' && beforeDelete instanceof Function)
                        if (beforeDelete() == false)
                            return false;

                    var data = "";
                    if (entityType == 1)
                        data = handler.DoMethodReturnString("MyBill_Delete");
                    else
                        data = handler.DoMethodReturnString("MyDict_Delete");
                    if (data.indexOf("err@") != -1) {
                        layer.alert(data);
                        return;
                    }
                    if (window.parent && (window.parent.location.href.indexOf("SearchDict") != -1
                        || window.parent.location.href.indexOf("SearchBill") != -1)) {
                        window.parent.reload();
                        //关闭该弹出层
                        window.parent.layer.close(layer.index);
                    } else {
                        if (entityType == 1)
                            window.location.href = 'SearchBill.htm?FrmID=' + GetQueryString("FrmID");
                        else
                            window.location.href = 'SearchDict.htm?FrmID=' + GetQueryString("FrmID");
                    }

                    break;
                case "DataVer":
                    var url = "./OptComponents/DataVer.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");

                    window.location.href = filterXSS(url);
                    break;
                case "Search":
                    if (entityType == 1) {
                        var url = "SearchBill.htm?FrmID=" + GetQueryString("FrmID");
                        if (methodNo != "")
                            url += "&MethodNo=" + methodNo;
                        if (pworkid != 0)
                            url += "&PWorkID=" + pworkid;
                            window.location.href = filterXSS(url);
                    }

                    else
                        window.location.href = "SearchDict.htm?FrmID=" + GetQueryString("FrmID");

                    break;
                case "Group":
                    var url = "Group.htm?FrmID=" + GetQueryString("FrmID");
                    if (methodNo != "")
                        url += "&MethodNo=" + methodNo;
                    if (pworkid != 0)
                        url += "&PWorkID=" + pworkid;
                    window.location.href = filterXSS(url);
                    break;
                case "Print":
                    var type = $(this).data("type");
                    var url = "";
                    if (type == "HTML") {
                        url = "../WorkOpt/Packup.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID") + "&SourceType=Bill&FileType=htm";
                        OpenLayuiDialog(url, "打印ZIP", window.innerWidth / 2, Window.innerHeight / 2, "auto");
                        break;
                    }
                    if (type == "PDF") {
                        PrintPDF();
                        break;
                    }
                    if (type == "RTF") {
                        url = "../WorkOpt/PrintDoc.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID") + "&SourceType=Bill";
                        OpenLayuiDialog(url, "打印RTF", window.innerWidth / 2, 50, "auto");
                        break;
                    }
                    if (type == "CCWord") {
                        url = "../WorkOpt/PrintDoc.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID") + "&SourceType=Bill";
                        OpenLayuiDialog(url, "打印CCWord", window.innerWidth / 2, 50, "auto");
                        break;
                    }
                    if (type == "ZIP") {
                        url = "../WorkOpt/Packup.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID") + "&SourceType=Bill&FileType=zip";
                        OpenLayuiDialog(url, "打印ZIP", window.innerWidth / 2, 50, "auto");
                        break;
                    }
                    break;
                case "dictFlow":
                    break;
                case "Setting":
                    var url = "../Comm/RefFunc/En.htm?EnName=BP.CCBill.FrmDict&PKVal=" + GetQueryString("FrmID");
                    OpenLayuiDialog(url, "设置", window.innerWidth * 4 / 5, 80, "auto");
                    break;
            }
        })

        

    });
}


function openPage(method) {

    if (method.MethodModel === "Bill")
        method.Docs = "./Opt/Bill.htm?FrmID=" + method.Tag1 + "&MethodNo=" + method.No + "&WorkID=" + GetQueryString("WorkID") + "&From=Dict";

    //如果是一个方法.
    if (method.MethodModel === "Func") {
        if (method.IsHavePara == 0) {
            doMethod(method);
            return;
        }
       method.Docs = "./Opt/DoMethodPara.htm?FrmID=" + method.FrmID + "&No=" + method.No + "&WorkID=" + GetQueryString("WorkID");
    }
        

    if (method.MethodModel === "FrmBBS")
        method.Docs = "./OptComponents/FrmBBS.htm?FrmID=" + method.FrmID + "&No=" + method.No + "&WorkID=" + GetQueryString("WorkID");
   

    if (method.MethodModel === "QRCode")
        method.Docs = "./OptComponents/QRCode.htm?FrmID=" + method.FrmID + "&MethodNo=" + method.No + "&WorkID=" + GetQueryString("WorkID");
 

    //单个实体发起的流程汇总.
    if (method.MethodModel === "SingleDictGenerWorkFlows")
        method.Docs = "./OptOneFlow/SingleDictGenerWorkFlows.htm?FrmID=" + method.FrmID + "&No=" + method.No + "&MethodNo=" + method.No + "&WorkID=" + GetQueryString("WorkID");
    //修改基础数据的的流程.
    if (method.MethodModel === "FlowBaseData") {
        //通过找个方法 window.open(method.Docs);

        var url = "./OptOneFlow/FlowBaseData.htm?WorkID=" + GetQueryString("WorkID");
        url += "&FrmID=" + GetQueryString("FrmID");
        url += "&MethodNo=" + method.No;
        url += "&FlowNo=" + method.FlowNo;
        method.Docs = url;
    }

    //其他业务流程.
    if (method.MethodModel == "FlowEtc") {

        var url = "./OptOneFlow/FlowEtc.htm?WorkID=" + GetQueryString("WorkID");
        url += "&FrmID=" + GetQueryString("FrmID");
        url += "&MethodNo=" + method.No; // GetQueryString("MethodNo");
        url += "&FlowNo=" + method.FlowNo;

        method.Docs = url;
    }

    //数据版本.
    if (method.MethodModel == "DataVer") {
        method.Docs = "./OptComponents/DataVer.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");
    }

    //日志.
    if (method.MethodModel == "DictLog") {
        method.Docs = "./OptComponents/DictLog.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");
    }

    //超链接.
    if (method.MethodModel == "Link") {
        if (method.UrlExt.indexOf('?') > 0)
            method.Docs = method.UrlExt + "&FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");
        else
            method.Docs = method.UrlExt + "?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");
    }

    if (method.Docs === "") {

        var url = method.UrlExt;
        if (url === "") {
            alert("没有解析的Url-MethodModel:" + method.MethodModel + " - " + method.Mark);
            return;
        }
        if (url.indexOf('?') > 0)
            method.Docs = url + "&FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");
        else
            method.Docs = url + "?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID");
    }

    window.location.href = filterXSS(method.Docs);
}
//必填项检查   名称最后是*号的必填  如果是选择框，值为'' 或者 显示值为 【*请选择】都算为未填 返回FALSE 检查必填项失败
function checkBlanks() {
    var checkBlankResult = true;
    //获取所有的列名 找到带* 的LABEL mustInput

    var lbs = $('.mustInput');
    $.each(lbs, function (i, obj) {
        var parentObj = $(obj).parent().parent();
        if (parentObj && parentObj.css('display') != 'none') {
            var keyOfEn = $(obj).attr("data-keyofen");
            if (keyOfEn != null) {
                var item = $("#TB_" + keyOfEn);
                if (item.length != 0) {
                    var val = item.val();
                    if ($("#" + keyOfEn + "_mtags").length != 0) {
                        var count = $("#" + keyOfEn + "_mtags").find(".ccflow-tag");
                        if (count == 0) {
                            checkBlankResult = false;
                            item.addClass('errorInput');
                        } else {
                            item.removeClass('errorInput');
                        }
                    } else {
                        if (item.val() == "") {
                            checkBlankResult = false;
                            item.addClass('errorInput');
                        } else {
                            item.removeClass('errorInput');
                        }
                    }

                    return true;
                }

                item = $("#DDL_" + keyOfEn);
                if (item.length != 0) {
                    if (item.val() == "" || item.val() == -1 || item.children('option:checked').text() == "*请选择") {
                        checkBlankResult = false;
                        item.addClass('errorInput');
                    } else {
                        item.removeClass('errorInput');
                    }
                    return true;
                }

            }

        }
    });

    return checkBlankResult;
}

//正则表达式检查
function checkReg() {
    var checkRegResult = true;
    var regInputs = $('.CheckRegInput');
    $.each(regInputs, function (i, obj) {
        var name = obj.name;
        var mapExtData = $(obj).data();
        if (mapExtData.Doc != undefined) {
            var regDoc = mapExtData.Doc.replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}').replace(/，/g, ',');
            var tag1 = mapExtData.Tag1;
            if ($(obj).val() != undefined && $(obj).val() != '') {

                var result = CheckRegInput(name, regDoc, tag1);
                if (!result) {
                    $(obj).addClass('errorInput');
                    checkRegResult = false;
                } else {
                    $(obj).removeClass('errorInput');
                }
            }
        }
    });

    return checkRegResult;
}
function doMethod(methodFunc) {
    var data = "";
    switch (methodFunc.MethodDocTypeOfFunc) {
        case 0://执行SQL
            var handler = new HttpHandler("BP.CCBill.WF_CCBill");
            handler.AddPara("MyPK", methodFunc.No);
            handler.AddPara("FrmID", methodFunc.FrmID);
            handler.AddPara("WorkID", GetQueryString("WorkID"));
            handler.AddPara("WorkIDs", GetQueryString("WorkIDs"));
            data = handler.DoMethodReturnString("DoMethod_ExeSQL"); //执行SQLs
            mui.alert(data);
            break;
        case 1://执行JavaScript
            Skip.addJs('../../DataUser/JSLibData/Method/' + methodFunc.No + '.js');
            data = DBAccess.RunFunctionReturnStr(methodFunc.MethodID);
            mui.alert(data);
            break;
        case 2://URL模式
            var url = methodFunc.Tag1;
            if (url.indexOf('?') == -1)
                url += "?1=1";
            url += "&MethodName=" + methodFunc.MethodID + "&FrmID=" + methodFunc.FrmID + "&WorkID=" + GetQueryString("WorkID") + "&WorkIDs=" + GetQueryString("WorkIDs");
            var data = DBAccess.RunUrlReturnString(url);
            mui.alert(data);
            break;
        default:
            mui.alert("还没有增加" + methodFunc.MethodDocTypeOfFunc + "类型的判断");
            break;
    }
}
/**
 * 相关功能
 */
function RefMethod() {
    //获得数据源.
    var handler = new HttpHandler("BP.CCBill.WF_CCBill_Admin");
    handler.AddUrlData();
    var ds = handler.DoMethodReturnJSON("Method_Init");
    var groups = ds["Groups"];
    var methods = ds["Methods"];
    for (var i = 0; i < groups.length; i++) {
        var group = groups[i];
        group.open = true;
        group.children = methods.filter(function (item) {
            return group.No === item.GroupID
        });
    }

    //解析显示的功能
    var _html = "";
    $.each(groups, function (i, group) {
        var li = $('<li class="mui-table-view-cell mui-collapse"></li>');
        li.append('<a class="mui-navigate-right" href="#">' + group.Name + '</a>');
        var ul = $(' <ul class="mui-table-view mui-table-view-chevron"></ul>');
        li.append(ul);
        $("#list").append(li);
        $.each(group.children, function (idx, item) {
            var ulli = $('<li class="mui-table-view-cell mui-menu"></li>');
            ulli.append('<a class="" href="javaScript:void()">' + item.Name+'</a>');
            ulli.data(item);
            ul.append(ulli);
        });
    });
    $(".mui-menu").on("tap", function () {
        var menu = $(this).data();
        openPage(menu);
    })
}
/**
 * 获取表单数据
 * @param {any} dataJson 表单数据JSON集合
 */
//获取表单数据
function getFormData(isCotainTextArea, isCotainUrlParam, formID, isDtl) {
    if (window.editor) {
        $("textarea[name='" + editor.srcElement.attr("name") + "']").val(editor.html());
    }

    var formss = $('#' + formID).serialize();
    if (formss == "")
        return {};

    var formArr = "\"" + formss.replace(/=/g, "\":\"");
    var stringObj = "{" + formArr.replace(/&/g, "\",\"") + "\"}";
    var formArrResult = JSON.parse(stringObj);
    haseExistStr = "";
    //获取CHECKBOX的值
    for (var key in formArrResult) {
        var attrName = key.replace("CB_", "");
        if ($("#SW_" + attrName).hasClass("mui-active") == true) {
            formArrResult["CB_" + attrName] = 1;
            continue;
        }

        if (key.indexOf("CB_") == 0) {
            //可能是复选框
            var ckboxs = $("input[name='" + key + "']");
            if (ckboxs.length == 1) {
                if ($('#' + key + ':checked').length == 1) {
                    formArrResult[key] = 1;
                } else {
                    formArrResult[key] = 0;
                }
            } else {
                var vals = [];
                $.each($("input[name='" + key + "']:checked"), function (i, item) {
                    vals.push($(item).val());
                });
                formArrResult[key] = vals.join(",");
            }

            continue;
        }


        if (key.indexOf('DDL_') == 0) {
            var item = $("#" + key).children('option:checked').text();
            var mystr = '';
            //如果是从表，需要获取后缀
            if (isDtl == true) {
                var before = key.substring(0, key.lastIndexOf("_"));
                var after = key.substring(key.lastIndexOf("_"));
                var keyT = before.replace("DDL_", "TB_") + 'T' + after;
                mystr = keyT + "=" + item;
                formArrResult[keyT] = item;
                //formArrResult.push(ele);
                haseExistStr += keyT + ",";
            } else {
                //mystr = key.replace("DDL_", "TB_") + 'T=' + item;
                var keyT = key.replace("DDL_", "TB_") + 'T'
                formArrResult[keyT] = item;
                //formArrResult.push(ele);
                haseExistStr += keyT + ",";
            }

        }


    }

    //复选框checkbox未选中时序列化时不包含的添加
    var checkBoxs = $('input[type=checkbox]');
    $.each(checkBoxs, function (i, checkBox) {
        //@浙商银行
        var name = $(checkBox).attr("name");
        if ($("input[name='" + name + "']:checked").length == 0) {
            formArrResult[name] = 0;
        }
    });

    //获取表单中禁用的表单元素的值
    var disabledEles = $('#' + formID + ' :disabled');
    $.each(disabledEles, function (i, disabledEle) {
        var name = $(disabledEle).attr('name');
        if (name == null || name == undefined || name == "")
            return true;
        switch (disabledEle.tagName.toUpperCase()) {
            case "INPUT":
                switch (disabledEle.type.toUpperCase()) {
                    case "CHECKBOX": //复选框
                        formArrResult[name] = encodeURIComponent($(disabledEle).is(':checked') ? 1 : 0);
                        break;
                    case "TEXT": //文本框
                    case "NUMBER":
                        if (haseExistStr.indexOf("," + tbID + ",") == -1) {
                            formArrResult[name] = encodeURIComponent($(disabledEle).val());
                        }
                        break;
                    case "RADIO": //单选钮
                        var eleResult = name + '=' + $('[name="' + name + ':checked"]').val();
                        if (!$.inArray(formArrResult, eleResult)) {
                            formArrResult.push();
                        }
                        break;
                }
                break;
            //下拉框      
            case "SELECT":
                var tbID = name.replace("DDL_", "TB_") + 'T';
                if ($("#" + tbID).length == 1) {
                    if (haseExistStr.indexOf("," + tbID + ",") == -1) {
                        formArrResult[tbID] = $(disabledEle).children('option:checked').text();
                        haseExistStr += tbID + ",";
                    }
                }
                formArrResult[name] = $(disabledEle).children('option:checked').val();
                break;
            case "TEXTAREA":
                formArrResult[name] = encodeURIComponent($(disabledEle).val());
                break;
        }
    });
    //获取树形结构的表单值
    var combotrees = $(".easyui-combotree");
    $.each(combotrees, function (i, combotree) {
        var name = $(combotree).attr('id');
        var tree = $('#' + name).combotree('tree');
        //获取当前选中的节点
        var data = tree.tree('getSelected');
        if (data != null) {
            formArrResult[name] = data.id;
            formArrResult[name + "T"] = data.text;
        }
    });

    return formArrResult;
}

function keyDown(e) {
    e.preventDefault();
    var currKey = 0, e = e || event || window.event;
    currKey = e.keyCode || e.which || e.charCode;
    if (currKey == 83 && (e.ctrlKey || e.metaKey)) {

    }
    return true;
}

function SearchBill() {
    var url = "SearchBill.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = filterXSS(url);
}


function DraftBox() {
    var url = "Draft.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = filterXSS(url);
}

function RefBill(frmID) {
    //关联单据

    var W = document.body.clientWidth - 40;
    var H = document.body.clientHeight - 40;
    var url = "Opt/RefBill.htm?PFrmID=" + frmID + "&WorkID=" + GetQueryString("WorkID") + "&FrmID=" + GetQueryString("FrmID");
    OpenBootStrapModal(url, "eudlgframe", "关联单据", W, H, "icon-property", null, null, null, function () {
        Reload();
    }, null, "black");
}

//查看关联单据的信息
function ShowRefBillInfo(frmID) {
    var workID = frmData.MainTable[0].PWorkID;
    var url = "MyBill.htm?WorkID=" + workID + "&FrmID=" + frmID + "&FK_MapData=" + frmID;
    var W = document.body.clientWidth - 40;
    var H = document.body.clientHeight - 40;

    OpenBootStrapModal(url, "eudlgframe", "关联单据信息", W, H, "icon-property", null, null, null, null, null, "black");
}


function StartFlow() {
    alert('尚未完成.');
}


function PrintPDF() {
    var W = document.body.clientWidth - 40;
    var H = document.body.clientHeight - 40;
    $("#Btn_PrintPdf").val("PDF打印中...");
    $("#Btn_PrintPdf").attr("disabled", true);
    var _html = document.getElementById("divCurrentForm").innerHTML;
    _html = _html.replace("height: " + $("#topContentDiv").height() + "px", "");
    _html = _html.replace("height: " + $("#contentDiv").height() + "px", "");
    _html = _html.replace("height: " + $("#divCCForm").height() + "px", "");

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddPara("html", _html);
    handler.AddPara("FrmID", GetQueryString("FrmID"));
    handler.AddPara("WorkID", GetQueryString("WorkID"));
    handler.AddPara("SourceType", "Bill");
    var data = handler.DoMethodReturnString("Packup_Init");
    if (data.indexOf("err@") != -1) {
        alert(data);
    } else {
        $("#Btn_PrintPdf").val("PDF打印成功");
        $("#Btn_PrintPdf").attr("disabled", false);
        $("#Btn_PrintPdf").val("打印pdf");
        var urls = JSON.parse(data);
        for (var i = 0; i < urls.length; i++) {
            if (urls[i].No == "pdf") {
                window.open(urls[i].Name);
                break;
            }
        }
    }
}



