
//阅读并关闭.
function ReadAndClose() {
    var msg = "";
    if ($("#FlowBBS_Doc").length == 1) {
        var doc = $("#FlowBBS_Doc").val();
        if ($("#FlowBBS_Doc").val() == null || $("#FlowBBS_Doc").val() == "" || $("#FlowBBS_Doc").val().trim().length == 0)
            doc = "已阅";
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
        handler.AddUrlData();
        handler.AddPara("FlowBBS_Doc", doc);
        msg = handler.DoMethodReturnString("FlowBBS_Save");
        if (msg.indexOf("err@") != -1) {
            alert(msg);
            return;
        }
    }

    if (window.parent != null && window.parent.WindowCloseReloadPage != null && typeof window.parent.WindowCloseReloadPage === "function") {
        window.parent.WindowCloseReloadPage(msg);
    } else {
        if (typeof WindowCloseReloadPage != 'undefined' && WindowCloseReloadPage instanceof Function)
            WindowCloseReloadPage(msg);
    }

    window.close();
}

function CloseWindow() {
    window.close();
}


var pageData = {};
var globalVarList = {};
var flowData = {};

//处理，表单没有加载完，就可以点击发送按钮.
var isLoadOk = false;

$(function () {

    if ("undefined" == typeof UserICon) {
        UserICon = '../DataUser/Siganture/';
    } else {
        UserICon = UserICon.replace("@basePath", basePath);
    }
    if ("undefined" == typeof UserIConExt) {
        UserIConExt = '.jpg';
    }

    //动态加载css样式
    if (webUser == null)
        webUser = new WebUser();
    var theme = webUser.Theme;
    if (theme == null || theme == undefined || theme == "")
        theme = "Default";

    //$('head').append('<link href="../DataUser/Style/CSS/' + theme + '.css" rel="stylesheet" type="text/css" />');
    //$('head').append('<link href="../DataUser/Style/MyFlow.css" rel="Stylesheet" />');

    //$('head').append('<link href="../DataUser/Style/CSS/' + theme + '.css" rel="stylesheet" type="text/css" />');
    $('head').append('<link href="../DataUser/Style/GloVarsCSS.css" rel="stylesheet" type="text/css" />');
    $('head').append('<link href="../DataUser/Style/MyFlow.css" rel="Stylesheet" />');


    initPageParam(); //初始化参数

    GenerWorkNode(); //表单数据.ajax


    if ($("#Message").html() == "") {
        $(".Message").hide();
    }

    $('#btnCloseMsg').bind('click', function () {
        $('.Message').hide();
    });

    $('#btnMsgModalOK').bind('click', function () {
        closeWindow();
    });


    $('#btnMsgModalOK1').bind('click', function () {

        //提示消息有错误，页面不跳转
        var msg = $("#msgModalContent").html();
        if (msg.indexOf("err@") == -1) {
            window.close();
        }
        else {
            setToobarEnable();
            $("#msgModal").modal("hidden");
        }

        if (window.parent != null && window.parent != undefined)
            window.parent.close();
        opener.window.focus();
    });
})

 

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


window.onload = function () {
    ResizeWindow();
    setToobarUnVisible();

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
    if (sealData.length > 0) {
        return;
    }
    else {
        sealData = new Entity("BP.Tools.WFSealData");
        sealData.MyPK = GetQueryString("WorkID") + "_" + GetQueryString("FK_Node") + "_" + val;
        sealData.OID = GetQueryString("WorkID");
        sealData.FK_Node = GetQueryString("FK_Node");
        sealData.SealData = val;
        sealData.Insert();
    }

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
function setHandWriteSrc(HandWriteID, imagePath) {
    imagePath = "../" + imagePath.substring(imagePath.indexOf("DataUser"));
    document.getElementById("Img" + HandWriteID).src = "";
    $("#Img" + HandWriteID).attr("src", imagePath);
    // document.getElementById("Img" + HandWriteID).src = imagePath;
    $("#TB_" + HandWriteID).val(imagePath);
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



//以下是软通写的
//初始化网页URL参数
function initPageParam() {
    pageData.FK_Flow = GetQueryString("FK_Flow");
    pageData.FK_Node = GetQueryString("FK_Node");
    pageData.FID = GetQueryString("FID") == null ? 0 : GetQueryString("FID");
    pageData.WorkID = GetQueryString("WorkID");
    pageData.OID = pageData.WorkID;
    pageData.Paras = GetQueryString("Paras");
    pageData.IsReadonly = 1; //如果是IsReadonly，就表示是查看页面，不是处理页面
}


//隐藏下方的功能按钮
function setToobarUnVisible() {
    //隐藏下方的功能按钮
    $('#bottomToolBar').css('display', 'none');
}


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


//调用后，就关闭刷新按钮.
function returnWorkWindowClose(data) {

    if (data == "" || data == "取消") {
        $('#returnWorkModal').modal('hide');
        setToobarEnable();
        return;
    }

    $('#returnWorkModal').modal('hide');
    //通过下发送按钮旁的下拉框选择下一个节点
    if (data.indexOf('SaveOK@') == 0) {
        //说明保存人员成功,开始调用发送按钮.
        var toNode = 0;
        //含有发送节点 且接收
        if ($('#DDL_ToNode').length > 0) {
            var selectToNode = $('#DDL_ToNode  option:selected').data();
            toNode = selectToNode.No;
        }

        execSend(toNode);
        //$('[name=Send]:visible').click();
        return;
    } else {//可以重新打开接收人窗口
        winSelectAccepter = null;
    }

    if (data.indexOf('err@') == 0 || data == "取消") {//发送时发生错误
        $('#Message').html(data);
        $('#MessageDiv').modal().show();
        return;
    }

    OptSuc(data);
}




//AtPara  @PopValSelectModel=0@PopValFormat=0@PopValWorkModel=0@PopValShowModel=0
function GepParaByName(name, atPara) {
    var params = atPara.split('@');
    var result = $.grep(params, function (value) {
        return value != '' && value.split('=').length == 2 && value.split('=')[0] == value;
    })
    return result;
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
                operations += "<option " + (obj.IntKey == defVal ? " selected = 'selected' " : "") + " value='" + mapAttr.DefVal + "'>-无(不选择)-</option>";

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
            //console.info(ele + "==" + flowData.MainTable[0][ele]);
            result = mainTable[ele];
            break;
        }
    }

    if (result != undefined && typeof (result) == 'string') {
        //result = result.replace(/｛/g, "{").replace(/｝/g, "}").replace(/：/g, ":").replace(/，/g, ",").replace(/【/g, "[").replace(/】/g, "]").replace(/；/g, ";").replace(/~/g, "'").replace(/‘/g, "'").replace(/‘/g, "'");
    }
    //console.info(defVal+"=="+keyOfEn+"=="+result);
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

// 杨玉慧
function GenerWorkNode() {

    var href = GetHrefUrl();
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyCC");
    handler.AddUrlData(urlParam);
    var data = handler.DoMethodReturnString("GenerWorkNode"); //执行保存方法.


    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    try {

        flowData = JSON.parse(data);

    } catch (err) {
        //console.log(data);
        alert(" GenerWorkNode转换JSON失败,请查看控制台日志,或者联系管理员.");
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
    var gfs = flowData.Sys_MapAttr;


    //设置标题.
    document.title = node.FlowName + ',' + node.Name; // "业务流程管理（BPM）平台";

    //帮助提醒
    HelpAlter();

    if (node.FormType == 11) {
        //获得配置信息.
        var frmNode = flowData["WF_FrmNode"];
        if (frmNode) {
            frmNode = frmNode[0];
            if (frmNode.FrmSln == 1)
                pageData.IsReadonly = 1
        }
    }
    //判断类型不同的类型不同的解析表单. 处理中间部分的表单展示.

    if (node.FormType == 5) {
        GenerTreeFrm(flowData); /*树形表单*/
        return;
    }

    if (node.FormType == 0 || node.FormType == 10) {
        $("#glyphicon").show();//显示换肤按钮
        GenerFoolFrm(flowData); //傻瓜表单.
    }

    /**if (node.FormType == 1) {
        Skip.addJs("./MyFlowFree2017.js");
        GenerFreeFrm(flowData);  //自由表单.
    }**/

    if (node.FormType == 12) {
        Skip.addJs("./CCForm/FrmDevelop.js");
        $('head').append('<link href="../DataUser/Style/MyFlowGenerDevelop.css" rel="Stylesheet" />');
        GenerDevelopFrm(flowData, flowData.Sys_MapData[0].No);
    }

    //2018.1.1 新增加的类型, 流程独立表单， 为了方便期间都按照自由表单计算了.
    if (node.FormType == 11) {
        if (flowData.WF_FrmNode[0] != null && flowData.WF_FrmNode[0] != undefined) {
            if (flowData.WF_FrmNode[0].FrmType == 0)
                GenerFoolFrm(flowData); //傻瓜表单.
            /**if (flowData.WF_FrmNode[0].FrmType == 1) {
                Skip.addJs("./MyFlowFree2017.js");
                GenerFreeFrm(flowData);
            }**/

            if (flowData.WF_FrmNode[0].FrmType == 8) {
                Skip.addJs("./CCForm/FrmDevelop.js");
                $('head').append('<link href="../DataUser/Style/MyFlowGenerDevelop.css" rel="Stylesheet" />');
                GenerDevelopFrm(flowData, flowData.WF_FrmNode[0].FK_Frm);
            }
        }

    }

    //公文表单
    if (node.FormType == 7) {
        var btnOffice = new Entity("BP.WF.Template.BtnLabExtWebOffice", pageData.FK_Node);
        if (btnOffice.WebOfficeFrmModel == 1)
            GenerFreeFrm(flowData);  //自由表单.
        else
            GenerFoolFrm(flowData); //傻瓜表单.
    }

    $.parser.parse("#CCForm");

    //单表单加载后执行.
    CCFormLoaded();

    //装载表单数据与修改表单元素风格.
    LoadFrmDataAndChangeEleStyle(flowData);

    AfterBindEn_DealMapExt(flowData);


    //初始化Sys_MapData
    var h = flowData.Sys_MapData[0].FrmH;
    var w = flowData.Sys_MapData[0].FrmW;

    //傻瓜表单的名称居中的问题
    if ($(".form-unit-title img").length > 0) {
        var width = $(".form-unit-title img")[0].width;
        $(".form-unit-title center h4 b").css("margin-left", "-" + width + "px");
    }

    $('#topContentDiv').width(w);
    $('.Bar').width(w + 15);
    $('#lastOptMsg').width(w + 15);

    ////2018.1.1 新增加的类型, 流程独立表单， 为了方便期间都按照自由表单计算了.
    //if (node.FormType == 11) {
    //    //获得配置信息.
    //    var frmNode = flowData["FrmNode"];
    //    if (frmNode) {
    //        frmNode = frmNode[0];
    //        if (frmNode.FrmSln == 1) {
    //            /*只读的方案.*/
    //            //alert("把表单设置为只读.");
    //           // SetFrmReadonly();
    //            //alert('ssssssssssss');
    //        }

    //        if (frmNode.FrmSln != 1)
    //            //处理下拉框级联等扩展信息
    //            AfterBindEn_DealMapExt(flowData);
    //    }
    //} else {
    //    //处理下拉框级联等扩展信息
    //    if (pageData.IsReadonly == null || pageData.IsReadonly == "0") {
    //        AfterBindEn_DealMapExt(flowData);
    //    }

    //}

    Common.MaxLengthError();

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

    ////加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题.
    var enName = flowData.Sys_MapData[0].No;
    var filespec = "../DataUser/JSLibData/" + pageData.FK_Flow + ".js";
    $.getScript(filespec);

    try {
        ////加载JS文件
        var s = document.createElement('script');
        s.type = 'text/javascript';
        s.src = "../DataUser/JSLibData/" + enName + "_Self.js";
        var tmp = document.getElementsByTagName('script')[0];
        tmp.parentNode.insertBefore(s, tmp);
    }
    catch (err) {

    }

    var jsSrc = '';
    try {


        var s = document.createElement('script');
        s.type = 'text/javascript';
        s.src = "../DataUser/JSLibData/" + enName + ".js";
        var tmp = document.getElementsByTagName('script')[0];
        tmp.parentNode.insertBefore(s, tmp);
    }
    catch (err) {

    }
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

    //公文解析
    if ($("#GovDocFile").length > 0) {
        Skip.addJs(ccbpmPath + "/WF/CCForm/Components/GovDocFile.js");
        LoadGovDocFile();
    }

    ////给富文本创建编辑器
    //if (document.BindEditorMapAttr) {
    //    var EditorDivs = $(".EditorClass");
    //    $.each(EditorDivs, function (i, EditorDiv) {
    //        var editorId = $(EditorDiv).attr("id");
    //        //给富文本 创建编辑器
    //        var editor = document.activeEditor = UM.getEditor(editorId, {
    //            'autoHeightEnabled': false,
    //            'fontsize': [10, 12, 14, 16, 18, 20, 24, 36],
    //            'initialFrameWidth': '100%'
    //        });
    //        var height = document.BindEditorMapAttr[i].UIHeight;
    //        $("#Td_" + document.BindEditorMapAttr[i].KeyOfEn).find('div[class = "edui-container"]').css("height", height);
    //        //$(".edui-container").css("height", height);

    //        if (editor) {

    //            editor.MaxLen = document.BindEditorMapAttr[i].MaxLen;
    //            editor.MinLen = document.BindEditorMapAttr[i].MinLen;
    //            editor.BindField = document.BindEditorMapAttr[i].KeyOfEn;
    //            editor.BindFieldName = document.BindEditorMapAttr[i].Name;

    //            //调整样式,让必选的红色 * 随后垂直居中
    //            $(editor.container).css({ "display": "inline-block", "margin-right": "4px", "vertical-align": "middle" });
    //        }
    //    })
    //}
    //给富文本创建编辑器
}


function resetData() {
    //装载表单数据与修改表单元素风格.
    LoadFrmDataAndChangeEleStyle(flowData);
}

function SetFrmReadonly() {


    $('#CCForm').find('input,textarea,select').attr('disabled', false);
    $('#CCForm').find('input,textarea,select').attr('readonly', true);
    $('#CCForm').find('input,textarea,select').attr('disabled', true);

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




function To(url) {
    window.name = "dialogPage"; window.open(url, "dialogPage")
}

function WinOpen(url, winName) {
    var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
    newWindow.focus();
    return;
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


