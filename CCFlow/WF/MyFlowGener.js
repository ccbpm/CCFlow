

//从MyFlowFree2017.htm 中拿过过的.

var pageData = {};
var globalVarList = {};
var flowData = {};
document.BindEditorMapAttr = [];
var webUser = new WebUser();

//处理，表单没有加载完，就可以点击发送按钮.
var isLoadOk = false;
debugger
var UserICon = getConfigByKey("UserICon", '../DataUser/Siganture/'); //获取签名图片的地址
var UserIConExt = getConfigByKey("UserIConExt", '.jpg');  //签名图片的默认后缀
$(function () {
    UserICon = UserICon.replace("@basePath", basePath);

    //动态加载css样式
    if (webUser == null)
        webUser = new WebUser();
    var theme = webUser.Theme;
    if (theme == null || theme == undefined || theme == "")
        theme = "Default";

    $('head').append('<link href="../DataUser/Style/MyFlow.css" rel="Stylesheet" />');

    $('head').append('<link href="../DataUser/Style/FoolFrmStyle/Default.css" rel="stylesheet" type="text/css" />');
    $('head').append('<link href="../DataUser/Style/GloVarsCSS.css" rel="stylesheet" type="text/css" />');

    initPageParam(); //初始化参数

    GenerWorkNode(); //表单数据.ajax

    if ($("#Message").html() == "") {
        $(".Message").hide();
    }
    $('#btnCloseMsg').bind('click', function () {
        $('.Message').hide();
    });

})


//从表在新建或者在打开行的时候，如果 EditModel 配置了使用卡片的模式显示一行数据的时候，就调用此方法.
function DtlFrm(ensName, refPKVal, pkVal, frmType, InitPage, H) {
    // model=1 自由表单, model=2傻瓜表单.
    var pathName = document.location.pathname;
    var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
    var projectName = pathName.length > 3 ? pathName.substring(1, 3) : "";
    var wWidth = $(window).width();
    var wHeight = $(window).height();
    if (wWidth > 1200) {
        wWidth = 1000;
    }
    if (H < 600 || H == undefined) {
        wHeight = 600;
    } else {
        wHeight = H;
    }

    if (projectName == "WF") {
        projectName = "";
    }

    if (plant == "JFlow")
        projectName = basePath;
    var url = basePath + '/WF/CCForm/DtlFrm.htm?EnsName=' + ensName + '&RefPKVal=' + refPKVal + "&FrmType=" + frmType + '&OID=' + pkVal;

    if (typeof ((parent && parent.OpenBootStrapModal) || OpenBootStrapModal) === "function") {
        ((parent && parent.OpenBootStrapModal) || OpenBootStrapModal)(url, "editSubGrid", '编辑', wWidth, wHeight, "icon-property", false, function () { }, null, function () {
            if (typeof InitPage === "function") {
                InitPage.call();
            } else {
                alert("请手动刷新表单");
            }
        }, "editSubGridDiv");
    } else {
        window.open(url);
    }
}
//从表在新建或者在打开行的时候，如果 EditModel 配置了使用卡片的模式显示一行数据的时候，就调用此方法. // IsSave 弹出页面关闭时是否要删除从表
function DtlFrm(ensName, refPKVal, pkVal, frmType, InitPage, FK_MapData, FK_Node, FID, IsSave, H) {
    // model=1 自由表单, model=2傻瓜表单.
    var pathName = document.location.pathname;
    var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
    if (projectName.startsWith("/WF")) {
        projectName = "";
    }
    if (H == undefined || H < 600)
        H = 600;
    if (H > 1000)
        H = 1000;

    var url = projectName + '/WF/CCForm/DtlFrm.htm?EnsName=' + ensName + '&RefPKVal=' + refPKVal + "&FrmType=" + frmType + '&OID=' + pkVal + "&FK_MapData=" + FK_MapData + "&FK_Node=" + FK_Node + "&FID=" + FID + "&IsSave=" + IsSave;
    if (typeof ((parent && parent.OpenBootStrapModal) || OpenBootStrapModal) === "function") {
        OpenBootStrapModal(url, "editSubGrid", '编辑', 1000, H, "icon-property", false, function () { }, null, function () {
            if (typeof InitPage === "function") {
                InitPage.call();
            } else {
                alert("请手动刷新表单");
            }
        }, "editSubGridDiv", null, false);
    } else {
        window.open(url);
    }
}
//单表单加载需要执行的函数
function CCFormLoaded() {
    if (parent != null && parent.document.getElementById('MainFrames') != undefined) {
        //计算高度，展示滚动条
        var height = $(parent.document.getElementById('MainFrames')).height() - 110;
        //$('#topContentDiv').height(height);

        $(window).resize(function () {
            //$("#CCForm").height($(window).height() - 115 + "px").css("overflow-y", "auto").css("scrollbar-face-color", "#fff"); 
            $("#divCCForm").height($(window).height() - 100 + "px").css("overflow-y", "auto");
        });
    }
    else {
        //新加
        //计算高度，展示滚动条
        var height = $("#divCCForm").height($(window).height() - 57 + "px").css("overflow-y", "auto");

        $(window).resize(function () {
            $("#divCCForm").height($(window).height() - 57 + "px").css("overflow-y", "auto");
        });
    }

    SetHegiht();
    //打开表单检查正则表达式
    if (typeof FormOnLoadCheckIsNull != 'undefined' && FormOnLoadCheckIsNull instanceof Function) {
        FormOnLoadCheckIsNull();
    }
}

//设置底部工具栏
function SetBottomTooBar() {
    //设置toolbar
    var toolBar = document.getElementById("bottomToolBar");
    if (toolBar) {
        document.getElementById("bottomToolBar").style.display = "";
    }

}

window.onload = function () {
    setToobarUnVisible();
    ResizeWindow();
};

$(function () {
    $('#HelpAlterDiv').on('hide.bs.modal', function () {

        //保存用户的帮助指引信息操作
        var mypk = webUser.No + "_ND" + pageData.FK_Node + "_HelpAlert"
        var userRegedit = new Entity("BP.Sys.UserRegedit");
        userRegedit.SetPKVal(mypk);
        var count = userRegedit.RetrieveFromDBSources();
        if (count == 0) {
            //保存数据
            userRegedit.FK_Emp = webUser.No;
            userRegedit.FK_MapData = "ND" + pageData.FK_Node;
            userRegedit.Insert();
        }
    })
});

function HelpAlter() {
    var node = flowData.WF_Node[0];
    //判断该节点是否启用了帮助提示 0 禁用 1 启用 2 强制提示 3 选择性提示
    var btnLab = new Entity("BP.WF.Template.BtnLab", node.NodeID);
    if (btnLab.HelpRole != 0) {
        var count = 0;
        if (btnLab.HelpRole == 3) {
            var mypk = webUser.No + "_ND" + node.NodeID + "_HelpAlert";
            var userRegedit = new Entity("BP.Sys.UserRegedit");
            userRegedit.SetPKVal(mypk);
            count = userRegedit.RetrieveFromDBSources();
        }

        if (btnLab.HelpRole == 2 || (count == 0 && btnLab.HelpRole == 3)) {
            var filename = basePath + "/DataUser/CCForm/HelpAlert/" + node.NodeID + ".htm";
            var htmlobj = $.ajax({ url: filename, async: false });
            if (htmlobj.status == 404)
                return;
            var str = htmlobj.responseText;
            if (str != null && str != "" && str != undefined) {
                $('#HelpAlter').html("").append(str);
                $('#HelpAlterDiv').modal().show();
            }
        }
    }
}


//双击签名
function figure_Template_Siganture(SigantureID, val, type) {

    //先判断，是否存在签名图片
    var handler = new HttpHandler("BP.WF.HttpHandler.WF");
    handler.AddPara('no', val);
    data = handler.DoMethodReturnString("HasSealPic");

    //如果不存在，就显示当前人的姓名
    if (data.length > 0 && type == 0) {
        $("#TB_" + SigantureID).before(data);
        var obj = document.getElementById("Img" + SigantureID);
        var impParent = obj.parentNode; //获取img的父对象
        impParent.removeChild(obj);
    }
    else {
        var src = UserICon + val + UserIConExt;    //新图片地址
        document.getElementById("Img" + SigantureID).src = src;
    }
    isSigantureChecked = true;

    var sealData = new Entities("BP.Tools.WFSealDatas");
    sealData.Retrieve("OID", GetQueryString("WorkID"), "FK_Node", GetQueryString("FK_Node"), "SealData", GetQueryString("UserNo"));
    if (sealData.length > 0)
        return;

    sealData = new Entity("BP.Tools.WFSealData");
    sealData.MyPK = GetQueryString("WorkID") + "_" + GetQueryString("FK_Node") + "_" + val;
    sealData.OID = GetQueryString("WorkID");
    sealData.FK_Node = GetQueryString("FK_Node");
    sealData.SealData = val;
    sealData.Insert();
}

//签字板
function figure_Template_HandWrite(HandWriteID, val) {
    var url = "CCForm/HandWriting.htm?WorkID=" + pageData.WorkID + "&FK_Node=" + pageData.FK_Node + "&KeyOfEn=" + HandWriteID;
    OpenEasyUiDialogExt(url, '签字板', 400, 300, false);
}
//地图
function figure_Template_Map(MapID, UIIsEnable) {
    var mainTable = flowData.MainTable[0];
    var AtPara = "";
    //通过MAINTABLE返回的参数
    for (var ele in mainTable) {
        if (ele == "AtPara" && mainTable != '') {
            AtPara = mainTable[ele];
            break;
        }
    }

    var url = "CCForm/Map.htm?WorkID=" + pageData.WorkID + "&FK_Node=" + pageData.FK_Node + "&KeyOfEn=" + MapID + "&UIIsEnable=" + UIIsEnable + "&Paras=" + AtPara;
    OpenBootStrapModal(url, "eudlgframe", "地图", 800, 500, null, false, function () { }, null, function () {

    });
}
function setHandWriteSrc(HandWriteID, imagePath, type) {
    if (type == 0) {
        imagePath = "../" + imagePath.substring(imagePath.indexOf("DataUser"));
        document.getElementById("Img" + HandWriteID).src = "";
        $("#Img" + HandWriteID).attr("src", imagePath);
        $("#TB_" + HandWriteID).val(imagePath);
    }
    if (type == 1) {
        $("#Img_" + HandWriteID).attr("src", imagePath);
        if ("undefined" != typeof writeImg)
            writeImg = imagePath;
    }

    $('#eudlg').dialog('close');
}
//然浏览器最大化.
function ResizeWindow() {
    if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen     
        var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽     
        var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高     
        window.moveTo(0, 0);           //把window放在左上角     
        window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh     
    }
}
function OpenCC() {
    var url = $("#CC_Url").val();
    var v = window.showModalDialog(url, 'cc', 'scrollbars=yes;resizable=yes;center=yes;minimize:yes;maximize:yes;dialogHeight: 650px; dialogWidth: 850px; dialogTop: 100px; dialogLeft: 150px;');
    if (v == '1')
        return true;
    return false;
}

//原有的
function OpenOfiice(fk_ath, pkVal, delPKVal, FK_MapData, NoOfObj, FK_Node) {
    var date = new Date();
    var t = date.getFullYear() + "" + date.getMonth() + "" + date.getDay() + "" + date.getHours() + "" + date.getMinutes() + "" + date.getSeconds();

    var url = 'WebOffice/AttachOffice.htm?DoType=EditOffice&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + "&FK_MapData=" + FK_MapData + "&NoOfObj=" + NoOfObj + "&FK_Node=" + FK_Node + "&T=" + t;
    window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
}


//初始化网页URL参数
function initPageParam() {
    pageData.FK_Flow = GetQueryString("FK_Flow");
    pageData.FK_Node = GetQueryString("FK_Node");
    pageData.FID = GetQueryString("FID") == null ? 0 : GetQueryString("FID");
    pageData.WorkID = GetQueryString("WorkID");
    pageData.OID = pageData.WorkID;
    pageData.Paras = GetQueryString("Paras");
    pageData.IsReadonly = 0;
    pageData.IsStartFlow = GetQueryString("IsStartFlow"); //是否是启动流程页面 即发起流程
    pageData.DoType1 = GetQueryString("DoType")//View
    
}


//隐藏下方的功能按钮
function setToobarUnVisible() {
    //隐藏下方的功能按钮
    $('#bottomToolBar').css('display', 'none');
}

////隐藏下方的功能按钮
//function setToobarDisiable() {
//    //隐藏下方的功能按钮
//    $('.Bar input').css('background', 'gray');
//    $('.Bar input').attr('disabled', 'disabled');
//}

//function setToobarEnable() {
//    //隐藏下方的功能按钮
//    $('.Bar input').css('background', '');
//    $('.Bar input').removeAttr('disabled');
//}

function CheckMinMaxLength() {

    return true;

    var editor = document.activeEditor;
    if (editor) {
        var wordslen = editor.getContent().length,
            msg = "";

        if (wordslen > editor.MaxLen || wordslen < editor.MinLen) {
            msg += '@' + editor.BindFieldName + ' , 输入的值长度必须在:' + editor.MinLen + ', ' + editor.MaxLen + '之间. 现在输入是:' + wordslen;
        }

        if (msg != "") {
            alert(msg);
            return false;
        }
    }
    return true;
}

//保存 0单保存 1发送的保存
function Save(saveType) {
    //正在保存弹出层
    var index = layer.msg('正在保存，请稍后..', {
        icon: 16
        , shade: 0.01
    });
    //保存从表数据
    $("[name=Dtl]").each(function (i, obj) {
        var contentWidow = obj.contentWindow;
        if (contentWidow != null && contentWidow.SaveAll != undefined && typeof (contentWidow.SaveAll) == "function") {
            IsSaveTrue = contentWidow.SaveAll();
        }
    });
    //审核组件
    if ($("#WorkCheck_Doc").length == 1) {
        //保存审核信息
        SaveWorkCheck();
    }

    //保存前事件
    if (typeof beforeSave != 'undefined' && beforeSave(saveType) instanceof Function)
        if (beforeSave(saveType) == false)
            return false;

    //判断是否有保存按钮，如果有就需要安全性检查，否则就不执行，这种情况在会签下，发送的时候不做检查。
    var btn = document.getElementById('Btn_Save');
    if (btn != null) {
        //检查最小最大长度.
        var f = CheckMinMaxLength();
        if (f == false)
            return false;
    }
    var msg = checkAths();
    if (msg != "") {
        alert(msg);
        return false;
    }

    //必填项和正则表达式检查
    var formCheckResult = true;

    if (checkBlanks() == false) {
        formCheckResult = false;
    }

    if (checkReg() == false) {
        formCheckResult = false;
    }

    if (formCheckResult == false) {
        // alert("请检查表单必填项和正则表达式");
        alert("请检查表单必填项");
        return false;
    }

    setToobarDisiable();

    //判断是否启用审核组件
    var iframe = document.getElementById("FWC");
    if (iframe)
        iframe.contentWindow.SaveWorkCheck();

    //树形表单保存
    if (flowData) {
        var node = flowData.WF_Node[0];
        //   alert(node.FormType);
        if (node && node.FormType == 5) {
            if (OnTabChange("btnsave") == true) {
                //判断内容是否保存到待办
                var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
                handler.AddPara("FK_Flow", pageData.FK_Flow);
                handler.AddPara("FK_Node", pageData.FK_Node);
                handler.AddPara("WorkID", pageData.WorkID);
                handler.AddPara("SaveType", saveType);
                handler.DoMethodReturnString("SaveFlow_ToDraftRole");
            }
            setToobarEnable();
            return;
        }
    }

    var params = getFormData(true, true);

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    $.each(params.split("&"), function (i, o) {
        var param = o.split("=");
        if (param.length == 2 && validate(param[1])) {
            handler.AddPara(param[0], param[1]);
        } else {
            handler.AddPara(param[0], "");
        }
    });
    var data = handler.DoMethodReturnString("Save"); //执行保存方法.

    // alert(data);

    layer.close(index);//关闭正在保存
    setToobarEnable();

    //刷新 从表的IFRAME
    var dtls = $('.Fdtl');
    $.each(dtls, function (i, dtl) {
        $(dtl).attr('src', $(dtl).attr('src'));
    });

    //提示信息.
    DealErrMsg(data);
    if (msgFieldly != "") {
        //友好提示.
        $("#divFieldly").html(msgFieldly);
        $("#divFieldly").css("color", "red");
        $("#divFieldly").show();
        $('#MessageDiv').modal().show();
    }
    if (msgTech != "") {
        var tech = "<a href='javascript: void(0)' onclick='msgTchClick()' ><img src='../../WF/img/Message24.png' height='20' width='20'/>技术信息</a>";
        $("#divTech").html(tech); //技术要显示的信息
        $("#divTech").show();
    }

    /* if (data.indexOf('保存成功') != 0) {
         $('#Message').html(data.substring(4, data.length));
         $('#MessageDiv').modal().show();
     }*/


}




//初始化下拉列表框的OPERATION
function InitDDLOperation(flowData, mapAttr, defVal) {
    if (mapAttr.UIIsEnable == "0" || pageData.IsReadonly == "1")
        return "";
    var operations = '';
    var data = flowData[mapAttr.KeyOfEn];
    if (data == undefined)
        data = flowData[mapAttr.UIBindKey];
    if (data == undefined) {
        //枚举类型的.
        if (mapAttr.LGType == 1) {
            var enums = flowData.Sys_Enum;
            enums = $.grep(enums, function (value) {
                return value.EnumKey == mapAttr.UIBindKey;
            });

            if (mapAttr.DefVal == -1)
                operations += "<option " + (defVal==-1 ? " selected = 'selected' " : "") + " value='" + mapAttr.DefVal + "'>-无(不选择)-</option>";

            $.each(enums, function (i, obj) {
                operations += "<option " + (obj.IntKey == defVal ? " selected='selected' " : "") + " value='" + obj.IntKey + "'>" + obj.Lab + "</option>";
            });
        }
        return operations;
    }
    $.each(data, function (i, obj) {
        operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
    });
    return operations;
}


//填充默认数据
function ConvertDefVal(flowData, defVal, keyOfEn) {
    //计算URL传过来的表单参数@TXB_Title=事件测试

    var pageParams = getQueryString();
    var pageParamObj = {};
    $.each(pageParams, function (i, pageParam) {
        if (pageParam.indexOf('@') == 0) {
            var pageParamArr = pageParam.split('=');
            pageParamObj[pageParamArr[0].substring(1, pageParamArr[0].length)] = pageParamArr[1];
        }
    });
    var result = defVal;
    var mainTable = flowData.MainTable[0];
    //通过MAINTABLE返回的参数
    for (var ele in mainTable) {
        if (keyOfEn == ele && mainTable != '') {
            result = mainTable[ele];
            break;
        }
    }

    var result = unescape(result);

    if (result == "null")
        result = "";

    return result;
}

function isExistArray(arrys, no) {
    for (var i = 0; i < arrys.length; i++) {
        if (arrys[i].split('=')[0] == no)
            return i;
    }
    return -1;
}

//获取表单数据
function getFormData(isCotainTextArea, isCotainUrlParam) {

    var formss = $('#divCCForm').serialize();

    var formArr = formss.split('&');
    var formArrResult = [];
    var haseExistStr = ",";
    var mcheckboxs = "";
    //获取树形结构的表单值
    var combotrees = $(".easyui-combotree");
    $.each(combotrees, function (i, combotree) {
        var name = $(combotree).attr('id');
        var tree = $('#' + name).combotree('tree');
        //获取当前选中的节点
        var data = tree.tree('getSelected');
        if (data != null) {
            formArrResult.push(name + '=' + data.id);
            formArrResult.push(name.replace("DDL_", "TB_") + 'T=' + data.text);
            haseExistStr += name.replace("DDL_", "TB_") + "T" + ",";
        }
    });
    $.each(formArr, function (i, ele) {
        if (ele.split('=')[0].indexOf('CB_') == 0) {
            //如果ID获取不到值，Name获取到值为复选框多选
            var targetId = ele.split('=')[0];
            if ($('#' + targetId).length == 1) {
                if ($('#' + targetId + ':checked').length == 1) {
                    ele = targetId + '=1';
                } else {
                    ele = targetId + '=0';
                }
                formArrResult.push(ele);
            } else {

                if (mcheckboxs.indexOf(targetId + ",") == -1) {
                    mcheckboxs += targetId + ",";
                    var str = "";
                    $("input[name='" + targetId + "']:checked").each(function (index, item) {
                        if ($("input[name='" + targetId + "']:checked").length - 1 == index) {
                            str += $(this).val();
                        } else {
                            str += $(this).val() + ",";
                        }
                    });

                    formArrResult.push(targetId + '=' + str);
                }
            }

        }
        if (ele.split('=')[0].indexOf('DDL_') == 0) {

            var ctrlID = ele.split('=')[0];

            var item = $("#" + ctrlID).children('option:checked').text();

            var mystr = '';
            mystr = ctrlID.replace("DDL_", "TB_") + 'T=' + item;
            formArrResult.push(mystr);
            formArrResult.push(ele);
            haseExistStr += ctrlID.replace("DDL_", "TB_") + "T" + ",";
        }
        if (ele.split('=')[0].indexOf('RB_') == 0) {
            formArrResult.push(ele);
        }

    });

    $.each(formArr, function (i, ele) {
        var ctrID = ele.split('=')[0];
        if (ctrID.indexOf('TB_') == 0) {
            if (haseExistStr.indexOf("," + ctrID + ",") == -1) {
                formArrResult.push(ele);
                haseExistStr += ctrID + ",";
            }


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
                        formArrResult.push(name + '=' + encodeURIComponent(($(disabledEle).is(':checked') ? 1 : 0)));
                        break;
                    case "TEXT": //文本框
                    case "HIDDEN":
                        formArrResult.push(name + '=' + encodeURIComponent($(disabledEle).val()));
                        break;
                    case "RADIO": //单选钮
                        name = $(disabledEle).attr('name');
                        var eleResult = name + '=' + $('[name="' + name + '"]:checked').val();
                        if ($.inArray(eleResult, formArrResult) == -1) {
                            formArrResult.push(eleResult);
                        }
                        break;
                }
                break;
            //下拉框            
            case "SELECT":
                formArrResult.push(name + '=' + encodeURIComponent($(disabledEle).children('option:checked').val()));
                var tbID = name.replace("DDL_", "TB_") + 'T';
                if ($("#" + tbID).length == 1) {
                    if (haseExistStr.indexOf("," + tbID + ",") == -1) {
                        formArrResult.push(tbID + '=' + $(disabledEle).children('option:checked').text());
                        haseExistStr += tbID + ",";
                    }
                }
                break;

            //文本区域                    
            case "TEXTAREA":
                formArrResult.push(name + '=' + encodeURIComponent($(disabledEle).val()));
                break;
        }
    });



    if (!isCotainTextArea) {
        formArrResult = $.grep(formArrResult, function (value) {
            return value.split('=').length == 2 ? value.split('=')[1].length <= 50 : true;
        });
    }

    formss = formArrResult.join('&');
    var dataArr = [];
    //加上URL中的参数
    if (pageData != undefined && isCotainUrlParam) {
        var pageDataArr = [];
        for (var data in pageData) {
            pageDataArr.push(data + '=' + pageData[data]);
        }
        dataArr.push(pageDataArr.join('&'));
    }
    if (formss != '')
        dataArr.push(formss);
    var formData = dataArr.join('&');

    //为了复选框  合并一下值  复选框的值以  ，号分割
    //用& 符号截取数据
    var formDataArr = formData.split('&');

    var formDataResultObj = {};
    $.each(formDataArr, function (i, formDataObj) {
        //计算出等号的INDEX
        var indexOfEqual = formDataObj.indexOf('=');
        var objectKey = formDataObj.substr(0, indexOfEqual);
        var objectValue = formDataObj.substr(indexOfEqual + 1);
        if (formDataResultObj[objectKey] == undefined) {
            formDataResultObj[objectKey] = objectValue;
        } else {
            formDataResultObj[objectKey] = formDataResultObj[objectKey] + ',' + objectValue;
        }
    });

    var formdataResultStr = '';
    for (var ele in formDataResultObj) {
        formdataResultStr = formdataResultStr + ele + '=' + formDataResultObj[ele] + '&';
    }

    // 处理没有选择的文本框.
    //获得checkBoxIDs 格式为: CB_IsXX,CB_IsYY,
    var ids = GenerCheckNames();

    if (ids) {
        var scores = ids.split(",");
        var arrLength = scores.length;
        for (var i = 0; i < arrLength; i++) {
            var field = scores[i];
            var index = formdataResultStr.indexOf(field);
            if (index == -1) {
                if ($("input[name='" + field + "'").length == 1)
                    formdataResultStr += '&' + field + '=0';
                else
                    formdataResultStr += '&' + field + '= ';
            }
        }
    }

    formdataResultStr = formdataResultStr.replace('&&', '&');

    return formdataResultStr;
}


//获得所有的checkbox 的id组成一个string用逗号分开, 以方便后台接受的值保存.
function GenerCheckIDs() {

    var checkBoxIDs = "";
    var arrObj = document.all;

    for (var i = 0; i < arrObj.length; i++) {

        if (arrObj[i].type != 'checkbox')
            continue;

        var cid = arrObj[i].id;
        if (cid == null || cid == "" || cid == '')
            continue;

        checkBoxIDs += arrObj[i].id + ',';
    }
    return checkBoxIDs;
}



//根据下拉框选定的值，弹出提示信息  绑定那个元素显示，哪个元素不显示  
function ShowNoticeInfo() {
    var rbs = flowData.Sys_FrmRB;
    data = rbs;
    $("input[type=radio],select").bind('change', function (obj) {
        var needShowDDLids = [];
        var methodVal = obj.target.value;

        for (var j = 0; j < data.length; j++) {
            var value = data[j].IntKey;
            var noticeInfo = data[j].Tip;
            var drdlColName = data[j].KeyOfEn;

            if (obj.target.tagName == "SELECT") {
                drdlColName = 'DDL_' + drdlColName;
            } else {
                drdlColName = 'RB_' + drdlColName;
            }
            if (methodVal == value && (obj.target.name == drdlColName)) {

                // 增加提示信息
                if (noticeInfo == undefined || noticeInfo.trim() == '') {
                    break;
                }
                noticeInfo = noticeInfo.replace(/\\n/g, '<br/>')
                var selectText = '';
                if (obj.target.tagName.toUpperCase() == 'INPUT' && obj.target.type.toUpperCase() == 'RADIO') {//radio button
                    selectText = obj.target.nextSibling.textContent;
                } else {//select
                    selectText = $(obj.target).find("option:selected").text();
                }
                $($('#div_NoticeInfo .popover-title span')[0]).text(selectText);
                $('#div_NoticeInfo .popover-content').html(noticeInfo);

                var top = obj.target.offsetHeight;
                var left = obj.target.offsetLeft;
                var current = obj.target.offsetParent;
                while (current !== null) {
                    left += current.offsetLeft;
                    top += current.offsetTop;
                    current = current.offsetParent;
                }


                if (obj.target.tagName.toUpperCase() == 'INPUT' && obj.target.type.toUpperCase() == 'RADIO') {//radio button
                    left = left - 40;
                    top = top + 10;
                }
                if (top - $('#div_NoticeInfo').height() - 30 < 0) {
                    //让提示框在下方展示
                    $('#div_NoticeInfo').removeClass('top');
                    $('#div_NoticeInfo').addClass('bottom');
                    top = top;
                } else {
                    $('#div_NoticeInfo').removeClass('bottom');
                    $('#div_NoticeInfo').addClass('top');
                    top = top - $('#div_NoticeInfo').height() - 30;
                }
                $('#div_NoticeInfo').css('top', top);
                $('#div_NoticeInfo').css('left', left);
                $('#div_NoticeInfo').css('display', 'block');

                break;
            }
        }

        $.each(needShowDDLids, function (i, ddlId) {
            $('#' + ddlId).change();
        });
    });


    $('#span_CloseNoticeInfo').bind('click', function () {
        $('#div_NoticeInfo').css('display', 'none');
    })

    $("input[type=radio]:checked,select").change();
    $('#span_CloseNoticeInfo').click();
}

//给出文本框输入提示信息
function ShowTextBoxNoticeInfo() {
    var mapAttr = flowData.Sys_MapAttr;
    mapAttr = $.grep(mapAttr, function (attr) {
        var atParams = attr.AtPara;
        return atParams != undefined && AtParaToJson(atParams).Tip != undefined && AtParaToJson(atParams).Tip != '' && $('#TB_' + attr.KeyOfEn).length > 0 && $('#TB_' + attr.KeyOfEn).css('display') != 'none';
    })

    $.each(mapAttr, function (i, attr) {
        $('#TB_' + attr.KeyOfEn).bind('focus', function (obj) {
            var mapAttr = flowData.Sys_MapAttr;

            mapAttr = $.grep(mapAttr, function (attr) {
                return 'TB_' + attr.KeyOfEn == obj.target.id;
            })
            var atParams = AtParaToJson(mapAttr[0].AtPara);
            var noticeInfo = atParams.Tip;

            if (noticeInfo == undefined || noticeInfo == '')
                return;

            $($('#div_NoticeInfo .popover-title span')[0]).text(mapAttr[0].Name);
            $('#div_NoticeInfo .popover-content').html(noticeInfo);

            var top = obj.target.offsetHeight;
            var left = obj.target.offsetLeft;
            var current = obj.target.offsetParent;
            while (current !== null) {
                left += current.offsetLeft;
                top += current.offsetTop;
                current = current.offsetParent;
            }

            if (top - $('#div_NoticeInfo').height() - 30 < 0) {
                //让提示框在下方展示
                $('#div_NoticeInfo').removeClass('top');
                $('#div_NoticeInfo').addClass('bottom');
                top = top;
            } else {
                $('#div_NoticeInfo').removeClass('bottom');
                $('#div_NoticeInfo').addClass('top');
                top = top - $('#div_NoticeInfo').height() - 30;
            }
            $('#div_NoticeInfo').css('top', top);
            $('#div_NoticeInfo').css('left', left);
            $('#div_NoticeInfo').css('display', 'block');
        });
    })
}

//检查附件数量.
function checkAths() {
    // 不支持火狐浏览器。
    if ("undefined" != typeof AthParams && AthParams.AthInfo != undefined) {
        var aths = document.getElementsByName("Ath");
        for (var i = 0; i < aths.length; i++) {
            var athment = aths[i].id.replace("Div_", "");
            if (AthParams.AthInfo[athment] != undefined && AthParams.AthInfo[athment].length > 0) {
                var athInfo = AthParams.AthInfo[athment][0];
                var minNum = athInfo[0];
                var maxNum = athInfo[1];
                var athNum = $("#Div_" + athment + " table tbody .athInfo").length;
                if (athNum.length == 0)
                    athNum = $("#Div_" + athment + " .athInfo").length;

                if (athNum < minNum)
                    return athment + "上传附件数量不能小于" + minNum;;
                if (athNum > maxNum)
                    return athment + "您最多上传[" + maxNum + "]个附件";
            }
        }
    }
    return "";

}


//必填项检查   名称最后是*号的必填  如果是选择框，值为'' 或者 显示值为 【*请选择】都算为未填 返回FALSE 检查必填项失败
function checkBlanks() {

    var checkBlankResult = true;
    //获得所有的class=mustInput的元素.
    var lbs = $('.mustInput');

    $.each(lbs, function (i, obj) {

        if ($(obj).parent().css('display') != 'none' && (($(obj).parent().next().css('display')) != 'none' || ($(obj).siblings("textarea").css('display')) != 'none')) {
        } else {
            return;
        }

        var keyofen = $(obj).data().keyofen;
        if (keyofen == undefined)
            return;
        var ele = $("#TB_" + keyofen);
        if (ele.length == 0)
            ele = $("#DDL_" + keyofen);
        if (ele.length == 0)
            ele = $("#CB_" + keyofen);
        if (ele.length == 0) {
            var val = $("input[name='RB_" + keyofen + "']:checked").val();
            if (val == -1 || val == undefined) {
                $("input[name$='RB_" + keyofen + "']").parent().parent().addClass('errorInput');
            } else {
                $("input[name$='RB_" + keyofen + "']").parent().parent().removeClass('errorInput');
            }
            return;
        }


        switch (ele[0].tagName.toUpperCase()) {
            case "INPUT":
                if (ele.attr('type') == "text") {
                    if (ele.val() == "") {
                        checkBlankResult = false;
                        ele.addClass('errorInput');
                    } else {
                        ele.removeClass('errorInput');
                    }
                }


                break;
            case "SELECT":
                if (ele.val() == "" || ele.val() == -1 || ele.children('option:checked').text() == "*请选择") {
                    checkBlankResult = false;
                    ele.addClass('errorInput');
                } else {
                    ele.removeClass('errorInput');
                }
                break;
            case "TEXTAREA":
                if (ele.val() == "") {
                    checkBlankResult = false;
                    ele.addClass('errorInput');
                } else {
                    ele.removeClass('errorInput');
                }
                break;
        }



    });


    //2.对 UMEditor 中的必填项检查
    if (document.activeEditor != null && document.activeEditor.$body != null) {
        /* #warning 这个地方有问题.*/

        //        var ele = document.activeEditor.$body;
        //        if (ele != null && document.activeEditor.getPlainTxt().trim() === "") {
        //            checkBlankResult = false;
        //            ele.addClass('errorInput');
        //        } else {
        //            ele.removeClass('errorInput');
        //        }
    }

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

function SaveDtlAll() {
    return true;
}

// 杨玉慧
function GenerWorkNode() {

    var href = window.location.href;
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData(urlParam);
    var data = handler.DoMethodReturnString("GenerWorkNode"); //执行保存方法.

    if (data.indexOf('err@') == 0) {
        alert(data);
        console.log(data);
        return;
    }

    try {

        flowData = JSON.parse(data);

    } catch (err) {
        alert(" GenerWorkNode转换JSON失败,请查看控制台日志,或者联系管理员.");
        console.log(flowData);
        return;
    }

    //处理附件的问题 
    if (flowData.Sys_FrmAttachment.length != 0) {
        Skip.addJs("./CCForm/Ath.js");
        Skip.addJs("./CCForm/JS/FileUpload/fileUpload.js");
        Skip.addJs("./Scripts/jquery-form.js");
        Skip.addJs("../DataUser/OverrideFiles/Ath.js");
        $('head').append("<link href='./CCForm/JS/FileUpload/css/fileUpload.css' rel='stylesheet' type='text/css' />");
    }


    //获取没有解析的外部数据源
    var uiBindKeys = flowData["UIBindKey"];
    if (uiBindKeys.length != 0) {
        //获取外部数据源 handler/JavaScript
        var operdata;
        for (var i = 0; i < uiBindKeys.length; i++) {
            var sfTable = new Entity("BP.Sys.SFTable", uiBindKeys[i].No);
            var srcType = sfTable.SrcType;
            if (srcType != null && srcType != "") {
                //Handler 获取外部数据源
                if (srcType == 5) {
                    var selectStatement = sfTable.SelectStatement;
                    if (plant == 'CCFlow')
                        selectStatement = basePath + "/DataUser/SFTableHandler.ashx" + selectStatement;
                    else
                        selectStatement = basePath + "/DataUser/SFTableHandler/" + selectStatement;
                    operdata = DBAccess.RunDBSrc(selectStatement, 1);
                }
                //JavaScript获取外部数据源
                if (srcType == 6) {
                    operdata = DBAccess.RunDBSrc(sfTable.FK_Val, 2);
                }
                flowData[uiBindKeys[i].No] = operdata;
            }
        }

    }

    var node = flowData.WF_Node[0];


    //设置标题.
    document.title = node.FlowName + ',' + node.Name;


    //循环之前的提示信息.
    var info = "";
    var title = ""
    for (var i = 0; i < flowData.AlertMsg.length; i++) {
        var alertMsg = flowData.AlertMsg[i];
        var alertMsgEle = figure_Template_MsgAlert(alertMsg, i);
        title = alertMsg.Title;
        if (title.indexOf("请求加签") > 0) {
            $('#flowInfo').append(alertMsgEle);

        } else {
            $('#Message').append(alertMsgEle);
            $('#Message').append($('<hr/>'));
        }
    }

    if (flowData.AlertMsg.length != 0 && title.indexOf("请求加签") < 0) {
        $('#MessageDiv').modal().show();
    }

    //帮助提醒
    HelpAlter();

    //文本框的提示信息
    //ShowTextBoxNoticeInfo();



    //if (node.FormType == 11) {
    //    //获得配置信息.
    //    var frmNode = flowData["FrmNode"];
    //    if (frmNode) {
    //        frmNode = frmNode[0];
    //        if (frmNode.FrmSln == 1)
    //            pageData.IsReadonly = 1
    //    }
    //}
    //判断类型不同的类型不同的解析表单. 处理中间部分的表单展示.
    var isDevelopForm = false;
    if (node.FormType == 5) {
        GenerTreeFrm(flowData); /*树形表单*/
        return;
    }

    if (node.FormType == 0 || node.FormType == 10) {
        $("#glyphicon").show();//显示换肤按钮
        Skip.addJs("./MyFlowFool2017.js?ver=1");
        GenerFoolFrm(flowData); //傻瓜表单.
    }

    if (node.FormType == 1) {
        Skip.addJs("./MyFlowFree2017.js?ver=1");
        GenerFreeFrm(flowData);  //自由表单.
    }

    if (node.FormType == 12) {
        Skip.addJs("./CCForm/FrmDevelop.js?ver=1");
        $('head').append('<link href="../DataUser/Style/MyFlowGenerDevelop.css" rel="Stylesheet" />');
        GenerDevelopFrm(flowData, flowData.Sys_MapData[0].No);
        isDevelopForm = true;
    }


    //2018.1.1 新增加的类型, 流程独立表单， 为了方便期间都按照自由表单计算了.
    if (node.FormType == 11) {
        if (flowData.WF_FrmNode[0] != null && flowData.WF_FrmNode[0] != undefined)
            if (flowData.WF_FrmNode[0].FrmType == 0) {
                Skip.addJs("./MyFlowFool2017.js?ver=1");
                GenerFoolFrm(flowData); //傻瓜表单.
            }

        if (flowData.WF_FrmNode[0].FrmType == 1) {
            Skip.addJs("./MyFlowFree2017.js?ver=1");
            GenerFreeFrm(flowData);
        }

        if (flowData.WF_FrmNode[0].FrmType == 8) {
            Skip.addJs("./CCForm/FrmDevelop.js?ver=1");
            $('head').append('<link href="../DataUser/Style/MyFlowGenerDevelop.css" rel="Stylesheet" />');
            GenerDevelopFrm(flowData, flowData.WF_FrmNode[0].FK_Frm);
            isDevelopForm = true;
        }

    }

    /* //公文表单
     if (node.FormType == 7) {
         var btnOffice = new Entity("BP.WF.Template.BtnLabExtWebOffice", pageData.FK_Node);
         if (btnOffice.WebOfficeFrmModel == 1)
             GenerFreeFrm(flowData);  //自由表单.
         else
             GenerFoolFrm(flowData); //傻瓜表单.
     }*/

    $.parser.parse("#CCForm");



    //单表单加载后执行.
    CCFormLoaded();

    //装载表单数据与修改表单元素风格.
    LoadFrmDataAndChangeEleStyle(flowData);

    //初始化Sys_MapData
    var h = flowData.Sys_MapData[0].FrmH;
    var w = flowData.Sys_MapData[0].FrmW;

    //傻瓜表单的名称居中的问题
    if ($(".form-unit-title img").length > 0) {
        var width = $(".form-unit-title img")[0].width;
        $(".form-unit-title center h4 b").css("margin-left", "-" + width + "px");
    }
    if (isDevelopForm == false)
        $('#topContentDiv').width(w);
    $('.Bar').width(w + 15);
    $('#lastOptMsg').width(w + 15);

    //2018.1.1 新增加的类型, 流程独立表单， 为了方便期间都按照自由表单计算了.
    var frmNode = flowData["WF_FrmNode"];
    var flow = flowData["WF_Flow"];
    if ((flow && flow[0].FlowDevModel==1 || node.FormType == 11) && frmNode != null && frmNode != undefined) {
        frmNode = frmNode[0];
        if (frmNode.FrmSln == 1) {
            /*只读的方案.*/
            SetFrmReadonly();
            pageData.IsReadonly = 1;
        }
    }
    AfterBindEn_DealMapExt(flowData);


    var marginLeft = $('#topContentDiv').css('margin-left');
    marginLeft = marginLeft.replace('px', '');

    marginLeft = parseFloat(marginLeft.substr(0, marginLeft.length - 2)) + 50;
    $('#topContentDiv i').css('left', marginLeft.toString() + 'px');
    //原有的

    //textarea的高度自适应的设置
    if (node.FormType != 1) {
        var textareas = $("textarea");
        $.each(textareas, function (idex, item) {
            autoTextarea(item);
        });
    }


    //为 DISABLED 的 TEXTAREA 加TITLE 
    var disabledTextAreas = $('#divCCForm textarea:disabled');
    $.each(disabledTextAreas, function (i, obj) {
        $(obj).attr('title', $(obj).val());
    })

    //加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题.
    var enName = flowData.Sys_MapData[0].No;
    loadScript("../DataUser/JSLibData/" + pageData.FK_Flow + ".js?t=" + Math.random());
    loadScript("../DataUser/JSLibData/" + enName + "_Self.js?t=" + Math.random());
    loadScript("../DataUser/JSLibData/" + enName + ".js?t=" + Math.random());

    //星级评分事件
    var scoreDiv = $(".score-star");
    $.each(scoreDiv, function (idex, item) {
        var divId = $(item).attr("id");
        var KeyOfEn = divId.substring(3);//获取字段值
        $("#Star_" + KeyOfEn + " img").click(function () {
            var index = $(this).index() + 1;
            $("#Star_" + KeyOfEn + " img:lt(" + index + ")").attr("src", "Style/Img/star_2.png");
            $("#SP_" + KeyOfEn + " strong").html(index + "  分");
            $("#TB_" + KeyOfEn).val(index);//给评分的隐藏input赋值
            index = index - 1;
            $("#Star_" + KeyOfEn + " img:gt(" + index + ")").attr("src", "Style/Img/star_1.png");
        });
    });



    $(".pimg").on("dblclick", function () {
        var _this = $(this); //将当前的pimg元素作为_this传入函数  
        imgShow("#outerdiv", "#innerdiv", "#bigimg", _this);
    });


    //给富文本创建编辑器
    if (document.BindEditorMapAttr) {
        Skip.addJs("./Comm/umeditor1.2.3-utf8/third-party/template.min.js?Version=" + Math.random());
        Skip.addJs("./Comm/umeditor1.2.3-utf8/umeditor.config.js?Version=" + Math.random());
        Skip.addJs("./Comm/umeditor1.2.3-utf8/umeditor.js?Version=" + Math.random());
        Skip.addJs("./Comm/umeditor1.2.3-utf8/lang/zh-cn/zh-cn.js?Version=" + Math.random());
        var EditorDivs = $(".EditorClass");
        $.each(EditorDivs, function (i, EditorDiv) {
            var editorId = $(EditorDiv).attr("id");
            //给富文本 创建编辑器
            var editor = document.activeEditor = UM.getEditor(editorId, {
                'autoHeightEnabled': false,
                'fontsize': [10, 12, 14, 16, 18, 20, 24, 36],
                'initialFrameWidth': '100%'
            });
            var height = document.BindEditorMapAttr[i].UIHeight;
            $("#Td_" + document.BindEditorMapAttr[i].KeyOfEn).find('div[class = "edui-container"]').css("height", height);
            //$(".edui-container").css("height", height);

            if (editor) {

                editor.MaxLen = document.BindEditorMapAttr[i].MaxLen;
                editor.MinLen = document.BindEditorMapAttr[i].MinLen;
                editor.BindField = document.BindEditorMapAttr[i].KeyOfEn;
                editor.BindFieldName = document.BindEditorMapAttr[i].Name;

                //调整样式,让必选的红色 * 随后垂直居中
                $(editor.container).css({ "display": "inline-block", "margin-right": "4px", "vertical-align": "middle" });
            }
        })
    }
    //给富文本创建编辑器

    //公文解析
    if ($("#GovDocFile").length > 0) {
        Skip.addJs(ccbpmPath + "/WF/CCForm/Components/GovDocFile.js");
        LoadGovDocFile();
    }


    //关联面板
    var mapData = flowData.Sys_MapData[0];
    //关联面板对应的焦点字段
    var field = mapData.RefBlurField;
    if (mapData.RefWorkModel != 0) {
        InitRefPanel(mapData);
        if (field != null && field != undefined && field != "") {
            var item = $("#TB_" + field);
            if (item.length == 0)
                item = $("#DDL_" + field);
            if (item.length != 0) {
                var $events = item.data("events");
                item.on("blur", InitRefPanel(mapData))
            }
        }

    }




}

function InitRefPanel(mapData) {
    var _html = "";
    switch (parseInt(mapData.RefWorkModel)) {
        case 0: //禁用
            break;
        case 1://静态html
            $("#refPanel").html(mapData.RefHtml);
            break;
        case 2://静态URL
            _html = DBAccess.RunDBSrc(mapData.RefUrl);
            $("#refPanel").html(_html);
            break;
        case 3://动态URL
            var url = mapData.RefUrl;
            url = url.replace("@OID", pageData.WorkID).replace("@WorkID", pageData.WorkID);
            url = DealExp(url);
            _html = "<iframe style='width:100%;height:100px'    src='" + url + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=no></iframe>";
            $("#refPanel").html(_html);
            break;
        case 4://动态HTML脚本
            var refHtml = mapData.RefHtml;
            refHtml = refHtml.replace("@OID", pageData.WorkID).replace("@WorkID", pageData.WorkID);
            refHtml = DealExp(urlrefHtml);
            $("#refPanel").html(refHtml);
            break;
        default: break;
    }
}

//图片附件编辑
function ImgAth(url, athMyPK) {
    var dgId = "iframDg";
    url = url + "&s=" + Math.random();
    OpenEasyUiDialog(url, dgId, '图片附件', 900, 580, 'icon-new', false, function () {

    }, null, null, function () {
        //关闭也切换图片
        //var obj = document.getElementById(dgId);
        //var win =(obj.contentWindow || obj.contentDocument); 
        var imgSrc = $("#imgSrc").val();
        if (imgSrc != null && imgSrc != "")
            document.getElementById('Img' + athMyPK).setAttribute('src', imgSrc + "?t=" + Math.random());
        $("#imgSrc").val("");
    });
}
function resetData() {
    //装载表单数据与修改表单元素风格.
    LoadFrmDataAndChangeEleStyle(flowData);
}

function SetFrmReadonly() {


    $('#CCForm').find('input,textarea,select').attr('disabled', false);
    $('#CCForm').find('input,textarea,select').attr('readonly', true);
    $('#CCForm').find('input,textarea,select').attr('disabled', true);
    if ($("#WorkCheck_Doc").length == 1) {
        $("#WorkCheck_Doc").removeAttr("readonly");
        $("#WorkCheck_Doc").removeAttr("disabled");
    }

    $('#Btn_Save').attr('disabled', true);
}

function sel(n, KeyOfEn, FK_MapData) {
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = KeyOfEn + "_" + (pageData.WorkID || pageData.OID || "") + "_" + n;
    frmEleDB.FK_MapData = FK_MapData;
    frmEleDB.EleID = KeyOfEn;
    frmEleDB.RefPKVal = (pageData.WorkID || pageData.OID || "");
    frmEleDB.Tag1 = n;
    if (frmEleDB.Update() == 0) {
        frmEleDB.Insert();
    }
}

function unsel(n, KeyOfEn) {
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = KeyOfEn + "_" + (pageData.WorkID || pageData.OID || "") + "_" + n;
    frmEleDB.Delete();
}
// V
function getMapExt(Sys_MapExt, KeyOfEn) {
    var ext = {};
    for (var p in Sys_MapExt) {
        if (KeyOfEn == Sys_MapExt[p].AttrOfOper) {
            ext = Sys_MapExt[p];
            break;
        }
    }
    return ext;
}

function addLoadFunction(id, eventName, method) {
    var js = "";
    js = "<script type='text/javascript' >";
    js += "function F" + id + "load() { ";
    js += "if (document.all) {";
    js += "document.getElementById('F" + id + "').attachEvent('on" + eventName + "',function(event){" + method + "('" + id + "');});";
    js += "} ";

    js += "else { ";
    js += "document.getElementById('F" + id + "').contentWindow.addEventListener('" + eventName + "',function(event){" + method + "('" + id + "');}, false); ";
    js += "} }";

    js += "</script>";
    return $(js);
}

var appPath = "../../";
var DtlsCount = " + dtlsCount + "; //应该加载的明细表数量

function figure_Template_MsgAlert(msgAlert, i) {
    var eleHtml = $('<div></div>');
    var titleSpan = $('<span class="titleAlertSpan"> ' + (parseInt(i) + 1) + "&nbsp;&nbsp;&nbsp;" + msgAlert.Title + '</span>');
    var msgDiv = $('<div>' + msgAlert.Msg + '</div>');
    eleHtml.append(titleSpan).append(msgDiv)
    return eleHtml;
}

//处理URL，MainTable URL 参数 替换问题
function dealWithUrl(src) {
    var src = fram.URL.replace(new RegExp(/(：)/g), ':');
    var params = '&FID=' + pageData.FID;
    params += '&WorkID=' + pageData.WorkID;
    if (src.indexOf("?") > 0) {
        var params = getQueryStringFromUrl(src);
        if (params != null && params.length > 0) {
            $.each(params, function (i, param) {
                if (param.indexOf('@') == 0) {//是需要替换的参数
                    paramArr = param.split('=');
                    if (paramArr.length == 2 && paramArr[1].indexOf('@') == 0) {
                        if (paramArr[1].indexOf('@WebUser.') == 0) {
                            params[i] = paramArr[0].substring(1) + "=" + flowData.MainTable[0][paramArr[1].substr('@WebUser.'.length)];
                        }
                        if (flowData.MainTable[0][paramArr[1].substr(1)] != undefined) {
                            params[i] = paramArr[0].substring(1) + "=" + flowData.MainTable[0][paramArr[1].substr(1)];
                        }

                        //使用URL中的参数
                        var pageParams = getQueryString();
                        var pageParamObj = {};
                        $.each(pageParams, function (i, pageParam) {
                            if (pageParam.indexOf('@') == 0) {
                                var pageParamArr = pageParam.split('=');
                                pageParamObj[pageParamArr[0].substring(1, pageParamArr[0].length)] = pageParamArr[1];
                            }
                        });
                        var result = "";
                        //通过MAINTABLE返回的参数
                        for (var ele in flowData.MainTable[0]) {
                            if (paramArr[0].substring(1) == ele) {
                                result = flowData.MainTable[0][ele];
                                break;
                            }
                        }
                        //通过URL参数传过来的参数
                        for (var pageParam in pageParamObj) {
                            if (pageParam == paramArr[0].substring(1)) {
                                result = pageParamObj[pageParam];
                                break;
                            }
                        }

                        if (result != '') {
                            params[i] = paramArr[0].substring(1) + "=" + unescape(result);
                        }
                    }
                }
            });
            src = src.substr(0, src.indexOf('?')) + "?" + params.join('&');
        }
    }
    else {
        src += "?q=1";
    }
    return src;
}

var colVisibleJsonStr = ''
document.BindEditorMapAttr = [];
/*
公共的工作处理器js. 
1. 该js的方法都是从各个类抽取出来的.
2. MyFlowFool.htm, MyFlowFree.htm, MyFlowSelfForm.htm 引用它.
3. 用于处理流程业务逻辑，表单业务逻辑.
*/


/* 打开公文表单 */
function OpenOffice(isEdit) {
    var url = "./WorkOpt/DocWord.htm?WorkID=" + GetQueryString("WorkID") + "&FK_Flow=" + GetQueryString("FK_Flow") + "&FK_Node=" + GetQueryString("FK_Node");
    WinOpen(url);
    return;
}

// 检查审核组件,是否加盖了电子签章？
function CheckFWC() {

    var frm = document.getElementById('FWC');
    if (frm == null || frm == undefined)
        return true;

    return frm.contentWindow.IsCanSendWork();
}





function To(url) {
    window.name = "dialogPage"; window.open(url, "dialogPage")
}

function WinOpen(url, winName) {
    var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
    newWindow.focus();
    return;
}

//. 保存嵌入式表单. add 2015-01-22 for GaoLing.
function SaveSelfFrom() {

    // 不支持火狐浏览器。
    var frm = document.getElementById('SelfForm');
    if (frm == null) {
        alert('系统错误,没有找到SelfForm的ID.');
    }
    //执行保存.
    return frm.contentWindow.Save();
}

function SendSelfFrom() {

    if (SaveSelfFrom() == false) {
        alert('表单保存失败，不能发送。');
        return false;
    }
    return true;
}

function SetHegiht() {
    var screenHeight = document.documentElement.clientHeight;
    var messageHeight = $('#Message').height();
    var topBarHeight = 40;
    var infoHeight = $('#flowInfo').height();
    var allHeight = messageHeight + topBarHeight + infoHeight;
    try {
        var BtnWord = $("#BtnWord").val();
        if (BtnWord == 2)
            allHeight = allHeight + 30;

        var frmHeight = $("#FrmHeight").val();
        if (frmHeight == NaN || frmHeight == "" || frmHeight == null)
            frmHeight = 0;
        if (screenHeight > parseFloat(frmHeight) + allHeight) {
            $("#TDWorkPlace").height(screenHeight - allHeight - 10);
        }
        else {
            $("#TDWorkPlace").height(parseFloat(frmHeight) + allHeight - 10);
        }
    }
    catch (e) {
    }
}


function Change() {
    var btn = document.getElementById('Btn_Save');
    if (btn != null) {
        if (btn.value.valueOf('*') == -1)
            btn.value = btn.value + '*';
    }
}

//加签回复
function AskForRe(fk_flow, fk_node, workID, fid) {
    var url = "WorkOpt/AskForRe.htm?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workID + "&FID=" + fid;
    var iframeId = "askForRe";
    var dlgTitle = "加签回复";
    var dlgWidth = "800";
    var dlgHeight = "400";
    var showCloseBtn = true;
    OpenBootStrapModal(url, iframeId, dlgTitle, dlgWidth, dlgHeight, showCloseBtn);
}