
/*
说明:
1.  围绕该文件工作的有两个js文件，分别是。 FrmFool.js
2.  对于傻瓜表单自由表单的，展现方式不同以外其他的都相同.
3.  相同的部分写入到了该文件里，不同的部分分别在不同的两个js文件里.
4.  MapExt2021.js 文件是一个公用的文件，用于处理扩展业务逻辑的，它在多个地方别调用了.
*/
var isReadonly = false;
var IsSave = false;//是否已经保存
//初始化函数
$(function () {
    //增加css样式
    $('head').append('<link href="../../DataUser/Style/GloVarsCSS.css" rel="stylesheet" type="text/css" />');
    initPageParam(); //初始化参数.
    if (pageData.IsNew == 0) //数据编辑字段
        IsSave = true;
    //隐藏保存按钮.
    if (GetHrefUrl().indexOf('&IsReadonly=1') > 1
        || GetHrefUrl().indexOf('&IsEdit=0') > 1) {
        isReadonly = true;
        $("#Save").hide();
        $("#SaveAndClose").hide();
    }
    //构造表单.
    GenerFrm(); //表单数据.
    $("#ToolBar .layui-btn").on('click', function () {
        switch (this.name) {
            case "Save":
                Save(false);
                break;
            case "SaveAndAdd":
                Save(true);
                break;
            case "Delete":
                DeleteDtlFrm();
                break;
            case "Close":
                CloseIt();
                break;
        }
    });
});

/**
 * 网页页面参数获取
 */
var pageData = {};
var richTextType = getConfigByKey("RichTextType", 'tinymce');

function initPageParam() {
    pageData.FK_Flow = GetQueryString("FK_Flow");
    pageData.FK_Node = GetQueryString("FK_Node");
    pageData.FID = GetQueryString("FID") == null ? 0 : GetQueryString("FID");

    var oid = GetQueryString("WorkID");
    if (oid == null)
        oid = GetQueryString("OID");
    oid = oid == null || oid == undefined || oid == "" ? 0 : oid;
    pageData.OID = oid;
    pageData.Paras = GetQueryString("Paras");
    pageData.IsReadonly = GetQueryString("IsReadonly"); //如果是IsReadonly，就表示是查看页面，不是处理页面
    pageData.IsStartFlow = GetQueryString("IsStartFlow"); //是否是启动流程页面 即发起流程
    pageData.DoType1 = GetQueryString("DoType");
    pageData.FK_MapData = GetQueryString("FK_MapData");
    pageData.IsNew = GetQueryString("IsNew")||"0";
}

/**
 * 表单数据获取
 */
var frmData = null;
function GenerFrm() {

    var href = GetHrefUrl();
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');

   
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddUrlData(urlParam);
    handler.AddJson(pageData);
    var data = handler.DoMethodReturnString("DtlFrm_Init");

    if (data.indexOf('err@') == 0) {
        layer.alert('装载表单出错,请查看控制台console,或者反馈给管理员.'+'<br/>'+data);
        console.log(data);
        return;
    }

    if (data.indexOf('url@') == 0) {
        data = data.replace('url@', '');
        SetHref(data);
        return;
    }
    try {
        frmData = JSON.parse(data);
    }
    catch (err) {
        layer.alert("frmData数据转换JSON失败(请查看控制台console):" + data);
        console.log(data);
        return;
    }

    //获取没有解析的外部数据源
    var uiBindKeys = frmData["UIBindKey"];
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
                    if (plant == "CCFlow")
                        selectStatement = basePath + "/DataUser/SFTableHandler.ashx" + selectStatement;
                    else
                        selectStatement = basePath + "/DataUser/SFTableHandler" + selectStatement;
                    operdata = DBAccess.RunDBSrc(selectStatement, 1);
                }
                //JavaScript获取外部数据源
                if (srcType == 6) {
                    operdata = DBAccess.RunDBSrc(sfTable.FK_Val, 2);
                }
                frmData[uiBindKeys[i].No] = operdata;
            }
        }

    }
    //处理附件的问题 
    if (frmData.Sys_FrmAttachment.length != 0) {
        Skip.addJs("./Ath.js");
        Skip.addJs("./JS/FileUpload/fileUpload.js");
        Skip.addJs("../Scripts/jquery-form.js");
        Skip.addJs("../../DataUser/OverrideFiles/Ath.js");
        $('head').append("<link href='./JS/FileUpload/css/fileUpload.css' rel='stylesheet' type='text/css' />");
    }

    //解析表单数据
    $('head').append('<link href="../../DataUser/Style/FoolFrmStyle/Default.css" rel="stylesheet" type="text/css" />');
    Skip.addJs("./FrmFool.js?ver=" + Math.random());
    GenerFoolFrm(frmData);

    //获得sys_mapdata.
    var mapData = frmData["Sys_MapData"][0];

    //初始化Sys_MapData
    var w = mapData.FrmW;
 
    //表单名称.
    document.title = mapData.Name;
    $('#ContentDiv').width(w);
    $('#ContentDiv').css("margin-left", "auto").css("margin-right", "auto");

    var enName = mapData.No;

    loadScript("../../DataUser/JSLibData/" + enName + "_Self.js?t=" + Math.random());

    //星级评分事件
    setScore(isReadonly);

    //如果是富文本编辑器
    if ($(".rich").length > 0) {
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
            tinymce: '../Scripts/layui/ext/tinymce/tinymce'
        }).use('tinymce', function () {
            var tinymce = layui.tinymce;
            $(".rich").each(function (i, item) {
                var id = item.id;
                tinymce.render({
                    elem: "#" + id
                    , height: 200
                    , images_upload_url: images_upload_url
                    , paste_data_images: true
                });
            })

        });

    }

    //3.装载表单数据与修改表单元素风格.
    LoadFrmDataAndChangeEleStyle(frmData);
    layui.form.render();

    //4.解析表单的扩展功能
    AfterBindEn_DealMapExt(frmData);

    $.each($(".ccdate"), function (i, item) {
        var format = $(item).attr("data-info");
        if (format.indexOf("HH") != -1) {
            layui.laydate.render({
                elem: '#' + item.id,
                format: $(item).attr("data-info"), //可任意组合
                type: 'datetime',
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
 * 保存一条从表数据
 * @param {any} isSaveAndNew 是不是保存且新增
 */
function Save(isSaveAndNew) {
    //正在保存弹出层
    var index = layer.msg('正在保存，请稍后..', {
        icon: 16
        , shade: 0.01
    });
    $("[name=Dtl]").each(function (i, obj) {
        var contentWidow = obj.contentWindow;
        if (contentWidow != null && contentWidow.SaveAll != undefined && typeof (contentWidow.SaveAll) == "function") {
            contentWidow.SaveAll();
        }
    });
    //监听提交
    layui.form.on('submit(Save)', function (data) {
        //保存信息
        var formData = getFormData(data.field);
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddPara("IsForDtl", 1);
        handler.AddPara("EnsName", GetQueryString("EnsName"));
        handler.AddPara("RefPKVal", GetQueryString("RefPKVal"));
        handler.AddPara("OID", GetQueryString("OID"));
        for (var key in formData) {
            handler.AddPara(key, encodeURIComponent(formData[key]));
        }
        var data = handler.DoMethodReturnString("FrmGener_Save");
        layer.close(index);
        if (data.indexOf("err@") != -1) {
            layer.alert(data);
            return;
        }

        //layer.alert("数据保存成功");

        if (isSaveAndNew == false) {
            layer.alert("数据保存成功");
            //SetHref(GetHrefUrl() + "&IsSave=true");
            IsSave = true;
            return false;
        }
        IsSave = false;
        var url = "DtlFrm.htm?EnsName=" + GetQueryString("EnsName") + "&RefPKVal=" + GetQueryString("RefPKVal") + "&OID=0&IsNew=1";
        SetHref(url);
        return false;
    });

}







//设置表单元素不可用
function setFormEleDisabled() {
    //文本框等设置为不可用
    $('#divCCForm textarea').attr('disabled', 'disabled');
    $('#divCCForm select').attr('disabled', 'disabled');
    $('#divCCForm input[type!=button]').attr('disabled', 'disabled');
}


var pageData = {};


//回填扩展字段的值
function SetAth(data) {
    var atParamObj = $('#iframeAthForm').data();
    var tbId = atParamObj.tbId;
    var divId = atParamObj.divId;
    var athTb = $('#' + tbId);
    var athDiv = $('#' + divId);

    $('#athModal').modal('hide');
    //不存在或来自于viewWorkNodeFrm
    if (atParamObj != undefined && atParamObj.IsViewWorkNode != 1 && divId != undefined && tbId != undefined) {
        if (atParamObj.AthShowModel == "1") {
            athTb.val(data.join('*'));
            athDiv.html(data.join(';&nbsp;'));
        } else {
            athTb.val('@AthCount=' + data.length);
            athDiv.html("附件<span class='badge' >" + data.length + "</span>个");
        }
    } else {
        $('#athModal').removeClass('in');
    }
    $('#athModal').hide();
    var ifs = $("iframe[id^=track]").contents();
    if (ifs.length > 0) {
        for (var i = 0; i < ifs.length; i++) {
            $(ifs[i]).find(".modal-backdrop").hide();
        }
    }
}

//查看页面的附件展示  查看页面调用
function ShowViewNodeAth(athLab, atParamObj, src) {
    var athForm = $('iframeAthForm');
    var athModal = $('athModal');
    var athFormTitle = $('#athModal .modal-title');
    athFormTitle.text("上传附件：" + athLab);
    athModal.modal().show();
}


function SaveDtlData() {
    if (IsChange == false)
        return;

    Save();
}


/**
 * 删除从表数据
 */
function DeleteDtlFrm() {
    layer.confirm('您确定要删除吗?', function (index) {
        layer.close(index);
        IsSave = true;
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddPara("EnsName", GetQueryString("EnsName"));
        handler.AddPara("RefPKVal", GetQueryString("RefPKVal"));
        handler.AddPara("OID", GetQueryString("OID"));
        handler.AddPara("FK_Node", GetQueryString("FK_Node"));
        var data = handler.DoMethodReturnString("DtlFrm_Delete");
        if (data.indexOf('err@') == 0) {
            layer.alert(data);
            return;
        }
        layer.alert(data);
        CloseIt();
        return;
    })
   
}
/**
 * 关闭弹出窗
 */
function CloseIt() {
    dtlFrm_Delete();
    var index = parent.layer.getFrameIndex(window.name);
    parent.layer.close(index);
}

function dtlFrm_Delete() {
    if (IsSave == false) {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddPara("EnsName", GetQueryString("EnsName"));
        handler.AddPara("RefPKVal", GetQueryString("RefPKVal"));
        handler.AddPara("OID", GetQueryString("OID"));
        handler.AddPara("FK_Node", GetQueryString("FK_Node"));
        var data = handler.DoMethodReturnString("DtlFrm_Delete");
        if (data.indexOf('err@') == 0) {
            layer.alert(data);
            return;
        }
    }
}
