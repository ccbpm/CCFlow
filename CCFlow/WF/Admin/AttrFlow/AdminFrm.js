

function CloseWindow() {
    window.close();
}
//定义全局的变量
var pageData = {};//全局的参数变量
var flowData = {}; // 流程数据
var isReadonly = false;//表单方案是只读时的变化
if (typeof webUser == "undefined" || webUser == null)
    webUser = new WebUser();

var UserICon = getConfigByKey("UserICon", '../../../DataUser/Siganture/'); //获取签名图片的地址
var UserIConExt = getConfigByKey("UserIConExt", '.jpg');  //签名图片的默认后缀
var richTextType = getConfigByKey("RichTextType", 'tinymce');
$(function () {
    UserICon = UserICon.replace("@basePath", basePath);

    //增加css样式
    $('head').append('<link href="../../../DataUser/Style/GloVarsCSS.css" rel="stylesheet" type="text/css" />');

    //初始化表单参数
    initPageData();

    //初始化表单数据
    GenerWorkNode();
})

/**
 * 初始化表单数据
 */
function initPageData() {
    pageData = {
        FK_Flow: GetQueryString("FK_Flow"),
        FK_Node: GetQueryString("FK_Node"),
        FID: GetQueryString("FID") == null ? 0 : GetQueryString("FID"),
        WorkID: GetQueryString("WorkID"),
        OID: pageData.WorkID,
        Paras: GetQueryString("Paras"),
        IsReadonly: 0,
        IsStartFlow: GetQueryString("IsStartFlow"),
        IsMobile: IsMobile()//是不是移动端
    }
}
/**
 * 获取表单数据
 */
function GenerWorkNode() {
    var index = layer.load(0, { shade: false }); //0代表加载的风格，支持0-2
    var href = GetHrefUrl();
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData(urlParam);
    var data = handler.DoMethodReturnString("GenerWorkNode"); //执行保存方法.

    if (data.indexOf('err@') == 0) {
        layer.alert(data);
        console.log(data);
        layer.close(index);
        return;
    }

    try {

        flowData = JSON.parse(data);

    } catch (err) {
        layer.alert(" GenerWorkNode转换JSON失败,请查看控制台日志,或者联系管理员.");
        console.log(flowData);
        layer.close(index);
        return;
    }
    if (webUser == null)
        webUser = new WebUser();
    //处理附件的问题 
    if (flowData.Sys_FrmAttachment.length != 0) {
        Skip.addJs("../../CCForm/Ath.js");
        Skip.addJs("../../CCForm/JS/FileUpload/fileUpload.js");
        Skip.addJs("../../Scripts/jquery-form.js");
        Skip.addJs("../../../DataUser/OverrideFiles/Ath.js");
        $('head').append("<link href='../../CCForm/JS/FileUpload/css/fileUpload.css' rel='stylesheet' type='text/css' />");
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
    //修改网页标题.
    document.title = node.FlowName + ',' + node.Name;
    //增加提示信息
    ShowWorkReturnTip();
    //解析表单
    BindFrm();
    //加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题.
    var enName = flowData.Sys_MapData[0].No;
    loadScript("../../../DataUser/JSLibData/" + pageData.FK_Flow + ".js?t=" + Math.random());
    loadScript("../../../DataUser/JSLibData/" + enName + "_Self.js?t=" + Math.random());
    loadScript("../../../DataUser/JSLibData/" + enName + ".js?t=" + Math.random());

    layer.close(index);
}

/**
 * 解析表单数据
 */
function BindFrm() {
    var node = flowData.WF_Node[0];
    var flow = flowData.WF_Flow[0];
    var flowDevModel = GetPara(flow.AtPara, "FlowDevModel");
    flowDevModel = flowDevModel == null || flowDevModel == undefined || flowDevModel == "" ? 0 : parseInt(flowDevModel);
    var isFool = true;
    var frmNode = flowData["WF_FrmNode"];
    var flow = flowData["WF_Flow"];
    if ((flow && flow[0].FlowDevModel == 1 || node.FormType == 11) && frmNode != null && frmNode != undefined) {
        frmNode = frmNode[0];
        if (frmNode.FrmSln == 1) {
            isReadonly = true;
        }
    }
    //根据流程设计模式解析
    switch (flowDevModel) {
        case FlowDevModel.Prefessional: //专业模式 
            //根据节点的表单方案解析
            switch (parseInt(node.FormType)) {
                case 0: //傻瓜表单
                case 10://累加表单
                    $('head').append('<link href="../../../DataUser/Style/FoolFrmStyle/Default.css" rel="stylesheet" type="text/css" />');
                    Skip.addJs("../../CCForm/FrmFool.js?ver=" + Math.random());
                    GenerFoolFrm(flowData);
                    break;
                case 12://开发者表单
                    $('head').append('<link href="../../../DataUser/Style/ccbpm.css" rel="stylesheet" type="text/css" />');
                    $('head').append('<link href="../../../DataUser/Style/MyFlowGenerDevelop.css" rel="Stylesheet" />');
                    Skip.addJs("../../CCForm/FrmDevelop2021.js?ver=1");
                    GenerDevelopFrm(flowData, flowData.Sys_MapData[0].No);
                    isFool = false;
                    break;
                case 5://树形表单
                    Skip.addJs("../../MyFlowTree.js?ver=1");
                    Skip.addJs("../../Scripts/vue.js?ver=1");
                    $("#AdminFrm").hide();
                    $("#app").show();
                    break;
                case 11://表单库单表单
                    var mapData = flowData.Sys_MapData[0];
                    if (frmNode != null && frmNode != undefined) {
                        if (mapData.FrmType == 0) { //傻瓜表单
                            $('head').append('<link href="../../../DataUser/Style/FoolFrmStyle/Default.css" rel="stylesheet" type="text/css" />');
                            Skip.addJs("../../CCForm/FrmFool.js?ver=" + Math.random());
                            GenerFoolFrm(flowData);
                        }
                        if (mapData.FrmType == 8) {//开发者表单
                            Skip.addJs("../../CCForm/FrmDevelop2021.js?ver=" + Math.random());
                            $('head').append('<link href="../../../DataUser/Style/ccbpm.css" rel="stylesheet" type="text/css" />');
                            $('head').append('<link href="../../../DataUser/Style/MyFlowGenerDevelop.css" rel="Stylesheet" />');
                            GenerDevelopFrm(flowData, flowData.Sys_MapData[0].No);
                            isFool = false;
                        }
                    }
                    break;
                default:
                    layer.alert("节点表单方案:[" + node.FormType + "]不存在或者暂未解析，请联系管理员")
                    break;
            }
            break;
        case FlowDevModel.JiJian://极简模式(傻瓜表单)
            var mapData = flowData.Sys_MapData[0];
            if (mapData.FrmType == 0) { //傻瓜表单
                $('head').append('<link href="../../../DataUser/Style/FoolFrmStyle/Default.css" rel="stylesheet" type="text/css" />');
                Skip.addJs("../../CCForm/FrmFool.js?ver=" + Math.random());
                GenerFoolFrm(flowData);
            }
            if (mapData.FrmType == 8) {//开发者表单
                Skip.addJs("../../CCForm/FrmDevelop2021.js?ver=" + Math.random());
                $('head').append('<link href="../../../DataUser/Style/ccbpm.css" rel="stylesheet" type="text/css" />');
                $('head').append('<link href="../../../DataUser/Style/MyFlowGenerDevelop.css" rel="Stylesheet" />');
                GenerDevelopFrm(flowData, flowData.Sys_MapData[0].No);
                isFool = false;
            }

            break;
        case FlowDevModel.FoolTruck://累加模式
            Skip.addJs("../../CCForm/FrmFool.js?ver=" + Math.random());
            GenerFoolFrm(flowData); //傻瓜表单.
            break;
        case FlowDevModel.RefOneFrmTree://表单库单表单
            if (frmNode != null && frmNode != undefined) {
                // frmNode = frmNode[0];
                if (frmNode.FrmType == 0) { //傻瓜表单
                    Skip.addJs("../../CCForm/FrmFool.js?ver=" + Math.random());
                    GenerFoolFrm(flowData);
                }
                if (frmNode.FrmType == 8) {//开发者表单
                    Skip.addJs("../../CCForm/FrmDevelop2021.js?ver=" + Math.random());
                    $('head').append('<link href="../../../DataUser/Style/MyFlowGenerDevelop.css" rel="Stylesheet" />');
                    GenerDevelopFrm(flowData, flowData.Sys_MapData[0].No);
                    isFool = false;
                }
            }
            break;
        case FlowDevModel.FrmTree://表单库多表单
            Skip.addJs("../../MyFlowTree.js?ver=1");
            Skip.addJs("../../Scripts/vue.js?ver=1");
            $("#AdminFrm").hide();
            $("#app").show();
            break;
        default:
            layer.alert("流程设计模式:[" + getFlowDevModelText(flow.FlowDevModel) + "]不存在或者暂未解析，请联系管理员")
            break;
    }


    //调整页面宽度
    var w = flowData.Sys_MapData[0].FrmW;//设置的页面宽度
    //傻瓜表单的名称居中的问题
    if ($(".form-unit-title img").length > 0) {
        var width = $(".form-unit-title img")[0].width;
        $(".form-unit-title center h4 b").css("margin-left", "-" + width + "px");
    }
    if (isFool == true && pageData.IsMobile == false) {
        $('#ContentDiv').width(w);
        $('#ContentDiv').css("margin-left", "auto").css("margin-right", "auto");
    }



    //如果是富文本编辑器
    if ($(".rich").length > 0) {// && richTextType == "tinymce"
        var images_upload_url = "";
        var directory = "ND" + pageData.FK_Flow;
        var handlerUrl = "";
        if (plant == "CCFlow")
            handlerUrl = basePath + "/WF/Comm/Handler.ashx";
        else
            handlerUrl = basePath + "/WF/Comm/Sys/ProcessRequest.do";

        images_upload_url = handlerUrl + '?DoType=HttpHandler&DoMethod=RichUploadFile';
        images_upload_url += '&HttpHandlerName=BP.WF.HttpHandler.WF_Comm_Sys&Directory=' + directory;
        layui.extend({
            tinymce: '../../Scripts/layui/ext/tinymce/tinymce'
        }).use('tinymce', function () {
            var tinymce = layui.tinymce;
            $(".rich").each(function (i, item) {
                var id = item.id;
                tinymce.render({
                    elem: "#" + id
                    , height: 200
                    , images_upload_url: images_upload_url
                });
            })

        });

    }
    //if ($(".EditorClass").length > 0 && richTextType == "ueditor") {
    //    $('head').append('<link href="./Comm/umeditor1.2.3-utf8/themes/default/css/umeditor.css" type="text/css" rel="stylesheet">');
    //    Skip.addJs("../../Comm/umeditor1.2.3-utf8/third-party/template.min.js?Version=" + Math.random());
    //    Skip.addJs("../../Comm/umeditor1.2.3-utf8/umeditor.config.js?Version=" + Math.random());
    //    Skip.addJs("../../Comm/umeditor1.2.3-utf8/umeditor.js?Version=" + Math.random());
    //    Skip.addJs("../../Comm/umeditor1.2.3-utf8/lang/zh-cn/zh-cn.js?Version=" + Math.random());
    //    $.each($(".EditorClass"), function (i, EditorDiv) {
    //        var editorId = $(EditorDiv).attr("id");
    //        //给富文本 创建编辑器
    //        var editor = document.activeEditor = UM.getEditor(editorId, {
    //            'autoHeightEnabled': false, //是否自动长高
    //            'fontsize': [10, 12, 14, 16, 18, 20, 24, 36],
    //            'initialFrameWidth': '98%'
    //        });
    //        var mapAttr = $(EditorDiv).data();
    //        var height = mapAttr.UIHeight
    //        $("#Td_" + mapAttr.KeyOfEn).find('div[class = "edui-container"]').css("height", height);

    //        if (editor) {

    //            editor.MaxLen = mapAttr.MaxLen;
    //            editor.MinLen = mapAttr.MinLen;
    //            editor.BindField = mapAttr.KeyOfEn;
    //            editor.BindFieldName = mapAttr.Name;

    //            //调整样式,让必选的红色 * 随后垂直居中
    //            $(editor.container).css({ "display": "inline-block", "margin-right": "10px", "vertical-align": "middle" });
    //        }
    //    })
    //}

    //3.装载表单数据与修改表单元素风格.
    LoadFrmDataAndChangeEleStyle(flowData);
    if ((flow && flow[0].FlowDevModel == 1 || node.FormType == 11) && frmNode != null && frmNode != undefined) {
        if (frmNode.FrmSln == 1)
            /*只读的方案.*/
            SetFrmReadonly();

    }
    layui.form.render();

    //星级评分事件
    setScore(isReadonly);

    //4.解析表单的扩展功能
    AfterBindEn_DealMapExt(flowData);

    $.each($(".ccdate"), function (i, item) {
        var format = $(item).attr("data-info");
        var type = $(item).attr("data-type");
        if (format.indexOf("HH") != -1) {
            layui.laydate.render({
                elem: '#' + item.id,
                format: $(item).attr("data-info"), //可任意组合
                type: type,
                trigger: 'click',
                ready: function (date) {
                    var now = new Date();
                    var mm = "";
                    if (now.getMinutes() < 10)
                        mm = "0" + now.getMinutes();
                    else
                        mm = now.getMinutes();

                    var ss = "";
                    if (now.getSeconds() < 10)
                        ss = "0" + now.getSeconds();
                    else
                        ss = now.getSeconds();

                    this.dateTime.hours = now.getHours();
                    this.dateTime.minutes = mm;
                    this.dateTime.seconds = ss;
                },
                change: function (value, date, endDate) {
                    $('.laydate-btns-confirm').click();
                },
                done: function (value, date, endDate) {
                    var data = $(this.elem).data();
                    $(this.elem).val(value);
                    if (data && data.ReqDay != null && data.ReqDay != undefined)
                        ReqDays(data.ReqDay);
                }
            });
        } else {
            layui.laydate.render({
                elem: '#' + item.id,
                format: $(item).attr("data-info"), //可任意组合
                done: function (value, date, endDate) {
                    var data = $(this.elem).data();
                    $(this.elem).val(value);
                    if (data && data.ReqDay != null && data.ReqDay != undefined)
                        ReqDays(data.ReqDay);
                }
            });
        }

    })

}

/**
 *保存表单数据 
 */
function Save(saveType) {

    //正在保存弹出层
    var index = layer.msg('正在保存，请稍后..', {
        icon: 16
        , shade: 0.01
    });
    if (checkBlanks() == false) {
        layer.alert("必填项不能为空");
        return;
    }

    //保存从表数据
    $("[name=Dtl]").each(function (i, obj) {
        var contentWidow = obj.contentWindow;
        if (contentWidow != null && contentWidow.SaveAll != undefined && typeof (contentWidow.SaveAll) == "function") {
            contentWidow.SaveAll();
        }
    });
    //审核组件
    if ($("#WorkCheck_Doc").length == 1) {
        //保存审核信息
        SaveWorkCheck();
    }

    //保存前事件
    if (typeof beforeSave != 'undefined' && beforeSave(saveType) instanceof Function)
        if (beforeSave(saveType) == false) {
            layer.close(index);
            return;
        }

    //监听提交
    layui.form.on('submit(Save)', function (data) {
        //保存信息
        var formData = getFormData(data.field);
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddUrlData();
        for (var key in formData) {
            handler.AddPara(key, encodeURIComponent(formData[key]));
        }
        var data = handler.DoMethodReturnString("Save");
        layer.close(index);
        if (data.indexOf("err@") != -1) {
            layer.alert(data);
        }

        //if (typeof isSaveOnly != undefined && isSaveOnly == true) {

        //}else
        //    layer.alert("数据保存成功");

        return false;
    });

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
//流程设计模式.
var FlowDevModel = {
    //专业模式
    Prefessional: 0,
    //极简模式（傻瓜表单）
    JiJian: 1,
    //累加模式
    FoolTruck: 2,
    //绑定单表单
    RefOneFrmTree: 3,
    //绑定多表单
    FrmTree: 4,
    //SDK表单
    SDKFrm: 5,
    /// 嵌入式表单
    SelfFrm: 6,
    /// 物联网流程
    InternetOfThings: 7,
    /// 决策树流程
    Tree: 8
}
function getFlowDevModelText(model) {
    switch (model) {
        case FlowDevModel.Prefessional:
            return "专业模式";
        case FlowDevModel.JiJian:
            return "专业模式";
        case FlowDevModel.FoolTruck:
            return "累加模式";
        case FlowDevModel.RefOneFrmTree:
            return "绑定表单库的单表单";
        case FlowDevModel.FrmTree:
            return "绑定表单库的多表单";
        case FlowDevModel.SDKFrm:
            return "SDK表单";
        case FlowDevModel.SelfFrm:
            return "嵌入式表单";
        case FlowDevModel.InternetOfThings:
            return "物联网流程";
        case FlowDevModel.Tree:
            return "决策树流程";
        default:
            return model;
    }
}


/**
 * 增加退回
 */
function ShowWorkReturnTip() {

    //显示退回消息
    if (flowData.AlertMsg.length != 0) {
        var _html = "";
        $.each(flowData.AlertMsg, function (i, item) {
            if (item.Title == "退回信息")
                _html += "<div style='padding: 10px 0px 0px 10px;line-height: 24px;color:red'>";
            else
                _html += "<div style='padding: 10px 0px 0px 10px;line-height: 24px;'>";
            _html += (i + 1) + "." + item.Title + "<br/>";
            _html += item.Msg;
            _html += "</div>";
        });
        var h = window.innerHeight - 240;
        //退回消息
        layer.open({
            type: 1,
            skin: '', //加上边框
            area: ['420px', h + 'px'], //宽高
            content: _html
        });
    }
}

function SaveDtlAll() {
    return true;
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
                    if (item.val() == "") {
                        checkBlankResult = false;
                        item.addClass('errorInput');
                    } else {
                        item.removeClass('errorInput');
                    }
                    return true;
                }

                item = $("#DDL_" + keyOfEn);
                if (item.length != 0) {
                    if (item.val() == "" || item.val() == null || item.val() == -1 || item.children('option:checked').text() == "*请选择") {
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





