
/*
说明:
1.  围绕该文件工作的有两个js文件，分别是。 FrmFool.js
2.  对于傻瓜表单自由表单的，展现方式不同以外其他的都相同.
3.  相同的部分写入到了该文件里，不同的部分分别在不同的两个js文件里.
4.  MapExt2016.js 文件是一个公用的文件，用于处理扩展业务逻辑的，它在多个地方别调用了.
*/
//初始化函数
$(function () {
    if(webUser == null)
        webUser = new WebUser();

    initPageParam(); //初始化参数.

    //时间轴的表单增加打印单据按钮
    var fromWorkOpt = GetQueryString("FromWorkOpt");
    if (fromWorkOpt == 2) {
        var PrintDocHtml = "<input type=button name='PrintDoc' value='打印单据' enable=true onclick='printDoc()' />";
        $("#topToolBar").append(PrintDocHtml);
    }
   

    //构造表单.
    GenerFrm(); //表单数据.

    if (parent != null && parent.document.getElementById('MainFrames') != undefined) {
        //计算高度，展示滚动条
        var height = $(parent.document.getElementById('MainFrames')).height() - 110;
        $('#topContentDiv').height(height);

        $(window).resize(function () {
            $("#CCForm").height($(window).height() - 150 + "px").css("overflow-y", "auto").css("scrollbar-face-color", "#fff"); ;
        });
    }

    
    $('#btnCloseMsg').bind('click', function () {
        $('.Message').hide();
    });

   
});
//打印单据
function printDoc() {
    WinOpen("../WorkOpt/PrintDoc.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.OID + "&FK_Flow=" + pageData.FK_Flow + "&s=" + Math.random()+"', 'dsdd'");
}



//设置不可以用.
function SetReadonly() {
    //设置保存按钮不可以用.
    $("#Btn_Save").attr("disabled", true);
    $('#CCForm').find('input,textarea,select').attr('disabled', false);
    $('#CCForm').find('input,textarea,select').attr('readonly', true);
    $('#CCForm').find('input,textarea,select').attr('disabled', true);
}

var frmData = null;
//将v1版本表单元素转换为v2 杨玉慧  silverlight 自由表单转化为H5表单.
function GenerFrm() {
    var isTest = GetQueryString("IsTest");
    var isReadonly = GetQueryString("IsReadonly");

    var href = GetHrefUrl();
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');

    //隐藏保存按钮.
    if (href.indexOf('&IsReadonly=1') > 1 || href.indexOf('&IsEdit=0') > 1) {
        $("#Btn").hide();
    }
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddUrlData(urlParam);
    handler.AddJson(pageData);
    var data = handler.DoMethodReturnString("FrmGener_Init");

    if (data.indexOf('err@') == 0) {
        alert('装载表单出错,请查看控制台console,或者反馈给管理员.');
        alert(data);
        console.log(data);
        return;
    }

    try {
        frmData = JSON.parse(data);
    }
    catch (err) {
        alert(" frmData数据转换JSON失败:" + data);
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
                    if (plant == "CCFLow")
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

    //获得sys_mapdata.
    var mapData = frmData["Sys_MapData"][0];

    //初始化Sys_MapData
    var h = mapData.FrmH;
    var w = mapData.FrmW;
    if (h <= 1200)
        h = 1200;
    // beign 设置表单标题和表单的布局
    document.title = mapData.Name;
    //$('#divCCForm').height(h);
    //$('#topContentDiv').height(h);
    $('#topContentDiv').width(w);
    $('.Bar').width(w + 15);
    var marginLeft = $('#topContentDiv').css('margin-left');
    if (undefined == marginLeft)
        marginLeft = '0px';
    marginLeft = marginLeft.replace('px', '');
    marginLeft = parseFloat(marginLeft.substr(0, marginLeft.length - 2)) + 50;
    $('#topContentDiv i').css('left', marginLeft.toString() + 'px');
    $('#CCForm').html('');

    //end

    // 加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题.
    jQuery.getScript("../../DataUser/JSLibData/" + mapData.No + "_Self.js");
    jQuery.getScript("../../DataUser/JSLibData/" + mapData.No + ".js");

    
   //加载开发者表单的内容
    //先判断DataUser/CCForm / HtmlTemplateFile / FK_MapData.htm是否存在
    var filename = basePath + "/DataUser/CCForm/HtmlTemplateFile/" + mapData.No + ".htm";
    var htmlobj = $.ajax({ url: filename, async: false });
    var htmlContent = "";
    if (htmlobj.status == 404) {
        //数据库中查找
        htmlContent = mapdata.HtmlTemplateFile;
    } else {
        htmlContent = htmlobj.responseText;
        if (htmlContent == null && htmlContent == "" && htmlContent == undefined) {
            htmlContent = "";
        }
    }
    if (htmlContent == "" || htmlContent == null ) {
        alert("开发者设计的表单内容丢失，请联系管理员");
        return;
    }

    $("#CCForm").html(htmlContent);

    //解析表单中的数据

    //1.加载隐藏字段，设置只读，赋初始值
    LoadFrmDataAndChangeEleStyle(frmData); 

    //2.解析控件 从表、附件、附件图片、框架、地图、签字版、父子流程
   var frmDtls = frmData.Sys_MapDtl;
    for (var i = 0; i < frmDtls.length;i++) {
        var frmDtl = frmDtls[i];
        //根据data-key获取从表元素
        var element = $("Img[data-key=" + frmDtl.No + "]");
        if (element.length == 0)
            continue;
        figure_Template_Dtl(element,frmDtl);

    }
    var aths = frmData.Sys_FrmAttachment;//附件
    for (var i = 0; i < aths.length; i++) {
        var ath = aths[i];
        //根据data-key获取从表元素
        var element = $("Img[data-key=" + ath.MyPK + "]");
        if (element.length == 0)
            continue;
        figure_Template_Ath(element,ath);

    }

    var athImgs = frmData.Sys_FrmImgAth;//从表附件 figure_Template_Ath
    for (var i = 0; i < athImgs.length; i++) {
        var athImg = athImgs[i];
        //根据data-key获取从表元素
        var element = $("Img[data-key=" + athImg.MyPK + "]");
        if (element.length == 0)
            continue;
        figure_Template_ImageAth(element,athImg);

    }
    var iframes = frmData.Sys_MapFrame;//框架
    for (var i = 0; i < iframes.length; i++) {
        var iframe = iframes[i];
        //根据data-key获取从表元素
        var element = $("Img[data-key=" + iframe.MyPK + "]");
        if (element.length == 0)
            continue;
        figure_Template_IFrame(element, iframe);

    }
    if (frmData.WF_FrmNodeComponent != null && frmData.WF_FrmNodeComponent != undefined) {
        var nodeComponents = frmData.WF_FrmNodeComponent[0];//节点组件
        if (nodeComponents != null) {
            var element = $("Img[data-key=" + nodeComponents.NodeID + "]");
            if (element.length != 0)
                figure_Template_FigureSubFlowDtl(nodeComponents, mapData, element);
            //如果有审核组件，增加审核组件的HTML
            var _html = figure_Template_FigureFrmCheck(nodeComponents, mapData, frmData);
            $("#CCForm").append(_html);

        }
    }
    


 

    
   
   

    

    
    //3.处理下拉框级联等扩展信息(小范围多选)
    AfterBindEn_DealMapExt(frmData);



    ShowTextBoxNoticeInfo();

    //textarea的高度自适应的设置
    var textareas = $("textarea");
    $.each(textareas, function (idex, item) {
        autoTextarea(item);
    });
   
   
}

//从表
function figure_Template_Dtl(element,frmDtl, ext) {
    //$("<link href='../Comm/umeditor1.2.3-utf8/themes/default/css/umeditor.css' type = 'text/css' rel = 'stylesheet' />").appendTo("head");  
    //在Image元素下引入IFrame文件
    var src = "";
    if (frmDtl.ListShowModel == "0")
        //表格模式
        src = "../../CCForm/Dtl2017.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + pageData.OID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=" + pageData.IsReadonly + "Version=1";

    if (frmDtl.ListShowModel == "1")
        //卡片模式
        src = "../../CCForm/DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + pageData.OID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=" + pageData.isReadonly + "&Version=1";

    var W =element.width();
    var eleHtml = $("<div id='Fd" + frmDtl.No + "' style='width:" + W + "px; height:auto;' ></div>");

    var eleIframe = $("<iframe class= 'Fdtl' ID = 'Dtl_" + frmDtl.No + "' src = '" + src + "' frameborder=0  style='width:" + W + "px;"
        + "height: auto; text-align: left; '  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
    eleHtml.append(eleIframe);
    $(element).after(eleHtml);
    $(element).remove(); //移除Imge节点
}

//附件
function figure_Template_Ath(element,ath) {
    var src = "../../CCForm/Ath.htm?PKVal=" + pageData.OID + "&PWorkID=" + GetQueryString("PWorkID") + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=" + pageData.isReadonly + "&FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow;

    var fid = GetQueryString("FID");
    var pWorkID = GetQueryString("PWorkID");

    src += "&FID=" + fid;
    src += "&PWorkID=" + pWorkID;
    var W = element.width();
    var eleHtml = $("<div id='Fd" + ath.MyPK + "' style='width:" + W + "px; height:auto;' ></div>");

    var eleIframe = $("<iframe class= 'Fdtl' ID = 'Attach_" + ath.MyPK + "' src = '" + src + "' frameborder=0  style='width:" + W + "px;"
        + "height: auto; text-align: left; '  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
    eleHtml.append(eleIframe);
    $(element).after(eleHtml);
    $(element).remove(); //移除Imge节点
}

//图片附件
function figure_Template_ImageAth(element,frmImageAth) {
    var isEdit = frmImageAth.IsEdit;
    var eleHtml = $("<div></div>");
    var img = $("<img class='pimg'/>");

    var imgSrc = basePath + "/WF/Data/Img/LogH.PNG";

    var refpkVal = pageData.OID;
    //获取数据
    if (pageData.FK_MapData.indexOf("ND") != -1)
        imgSrc = basePath + "/DataUser/ImgAth/Data/" + frmImageAth.CtrlID + "_" + refpkVal + ".png";
    else
        imgSrc = basePath + "/DataUser/ImgAth/Data/" + pageData.FK_MapData + "_" + frmImageAth.CtrlID + "_" + refpkVal + ".png";
   
    //设计属性
    img.attr('id', 'Img' + frmImageAth.MyPK).attr('name', 'Img' + frmImageAth.MyPK);
    img.attr("src", imgSrc).attr('onerror', "this.src='" + basePath + "/WF/Admin/CCFormDesigner/Controls/DataView/AthImg.png'");
    img.css('width', element.width()).css('height', element.height()).css('padding', "0px").css('margin', "0px").css('border-width', "0px");
    //不可编辑
    if (isEdit == "1" && pageData.IsReadonly != 1) {
        var fieldSet = $("<fieldset></fieldset>");
        var length = $("<legend></legend>");
        var a = $("<a></a>");
        var url = basePath + "/WF/CCForm/ImgAth.htm?W=" + frmImageAth.W + "&H=" + frmImageAth.H + "&FK_MapData=" + pageData.FK_MapData + "&RefPKVal=" + refpkVal + "&CtrlID=" + frmImageAth.CtrlID;

        a.attr('href', "javascript:ImgAth('" + url + "','" + frmImageAth.MyPK + "');").html("编辑");
        length.css('font-style', 'inherit').css('font-weight', 'bold').css('font-size', '12px');

        fieldSet.append(length);
        length.append(a);
        fieldSet.append(img);
        eleHtml.append(fieldSet);
    } else {
        eleHtml.append(img);
    }
   
    $(element).after(eleHtml);
    $(element).remove(); //移除Imge节点
}

//图片附件编辑
function ImgAth(url, athMyPK) {
    var dgId = "iframDg";
    url = url + "&s=" + Math.random();
    OpenEasyUiDialog(url, dgId, '图片附件', 900, 580, 'icon-new', false, function () {

    }, null, null, function () {
        //关闭也切换图片
        var imgSrc = $("#imgSrc").val();
        if (imgSrc != null && imgSrc != "")
            document.getElementById('Img' + athMyPK).setAttribute('src', imgSrc + "?t=" + Math.random());
        $("#imgSrc").val("");
    });
}
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

//初始化框架
function figure_Template_IFrame(element,frame) {

    //获取框架的类型 0 自定义URL 1 地图开发 2流程轨迹表 3流程轨迹图
    var urlType = frame.UrlSrcType;
    var url = "";
    if (urlType == 0) {
        url = frame.URL;
        if (url.indexOf('?') == -1)
            url += "?1=2";

        if (url.indexOf("@basePath") == 0)
            url = url.replace("@basePath", basePath);

        //1.处理URL需要的参数
        var pageParams = getQueryString();
        $.each(pageParams, function (i, pageParam) {
            var pageParamArr = pageParam.split('=');
            url = url.replace("@" + pageParamArr[0], pageParamArr[1]);
        });

        var src = url.replace(new RegExp(/(：)/g), ':');
        if (src.indexOf("?") > 0) {
            var params = getQueryStringFromUrl(src);
            if (params != null && params.length > 0) {
                $.each(params, function (i, param) {
                    if (param.indexOf('@') != -1) {//是需要替换的参数
                        paramArr = param.split('=');
                        if (paramArr.length == 2 && paramArr[1].indexOf('@') == 0) {
                            if (paramArr[1].indexOf('@WebUser.') == 0)
                                url = url.replace(paramArr[1], frmData.MainTable[0][paramArr[1].substr('@WebUser.'.length)]);
                            else
                                url = url.replace(paramArr[1], frmData.MainTable[0][paramArr[1].substr(1)]);
                        }
                    }
                });
            }
        }


        //1.拼接参数
        var paras = this.pageData;
        var strs = "";
        for (var str in paras) {
            if (str == "EnsName" || str == "RefPKVal" || str == "IsReadonly")
                continue
            else
                strs += "&" + str + "=" + paras[str];
        }

        url = url + strs + "&IsReadonly=0";

        //4.追加GenerWorkFlow AtPara中的参数
        var gwf = frmData.WF_GenerWorkFlow[0];
        if (gwf != null) {
            var atPara = gwf.AtPara;
            if (atPara != null && atPara != "") {
                atPara = atPara.replace(/@/g, '&');
                url = url + atPara;
            }
        }
    }
    if (urlType == 2) //轨迹表
        url = "../WorkOpt/OneWork/Table.htm?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.OID + "&FID=" + pageData.FID;
    if (urlType == 3)//轨迹图
        url = "../WorkOpt/OneWork/TimeBase.htm?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.OID + "&FID=" + pageData.FID;

    var eleHtml = $("<div id='Frame" + frame.MyPK + "' style='width:" + frame.W + "px; height:auto;' ></div>");

    var eleIframe = $("<iframe class= 'Fdtl' ID = 'Frame_" + frame.MyPK + "' src = '" + url + "' frameborder=0  style='width:" + frame.W + "px;"
        + "height: auto; text-align: left; '  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");


    eleHtml.append(eleIframe);
    $(element).after(eleHtml);
    $(element).remove(); //移除Frame节点

}


//子流程
function figure_Template_FigureSubFlowDtl(wf_node, mapData,element) {

    //@这里需要处理, 对于流程表单.
    if (sta == 0 || sta == "0" || sta == undefined)
        return $('');

    var sta = wf_node.SFSta;
    var w = wf_node.SF_W;

    var src = "../WorkOpt/SubFlow.htm?s=2";
    var paras = '';

    paras += "&FID=" + pageData.FID;
    paras += "&OID=" + pageData.OID;
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.OID;
    if (sta == 2 || pageData.IsReadonly == 1)//只读
    {
        src += "&DoType=View";
    }
    else {
        fwcOnload = "onload= 'WC" + wf_node.NodeID + "load();'";
        $('body').append(addLoadFunction("WC" + wf_node.NodeID, "blur", "SaveDtl"));
    }
    src += "&r=q" + paras;

    var eleHtml = $("<div id=''SubFlow" + wf_node.NodeID + "' style='width:100%; height:auto;' ></div>");

    var eleIframe = $("<iframe class= 'Fdtl' ID = 'SubFlow_" + wf_node.NodeID + "' src = '" + src + "' frameborder=0  style='width:" + w + "px;"
        + "height: auto; text-align: left; '  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");


    eleHtml.append(eleIframe);
    $(element).after(eleHtml);
    $(element).remove(); //移除SubFlow节点

}


//审核组件
function figure_Template_FigureFrmCheck(wf_node, mapData, frmData) {



    var sta = wf_node.FWCSta;
    if (sta == 0 || sta == undefined)
        return $('');

    var node = frmData.WF_Node;
    if (node != undefined)
        node = node[0];

    var frmNode = frmData.WF_FrmNode;
    if (frmNode != undefined)
        frmNode = frmNode[0];

    if (node == null || frmNode == null)
        return $('');
    if (node.FormType == 5 && frmNode.IsEnableFWC == 0)
        return $('');

    var pos = PreaseFlowCtrls(mapData.FlowCtrls, "FrmCheck");

    var x = 0, y = 0, h = 0, w = 0;
    if (pos == null) {
        x = wf_node.FWC_X;
        y = wf_node.FWC_Y;
        h = wf_node.FWC_H;
        w = wf_node.FWC_W;
    }

    if (pos != null) {
        x = parseFloat(pos.X);
        y = parseFloat(pos.Y);
        h = parseFloat(pos.H);
        w = parseFloat(pos.W);
    }
    if (x <= 10)
        x = 100;
    if (y <= 10)
        y = 100;

    if (h <= 10)
        h = 100;

    if (w <= 10)
        w = 300;

    var src = "";
    if (wf_node.FWCVer == 0 || wf_node.FWCVer == "" || wf_node.FWCVer == undefined)
        src = "../WorkOpt/WorkCheck.htm?s=2&IsReadonly=" + GetQueryString("IsReadonly");
    else
        src = "../WorkOpt/WorkCheck2019.htm?s=2&IsReadonly=" + GetQueryString("IsReadonly");
    var fwcOnload = "";
    var paras = '';

    var isReadonly = GetQueryString('IsReadonly');
    if (isReadonly != "1") {
        isReadonly = "0";
    }
    if (sta == 2)//只读
        isReadonly = "1";


    paras += "&FID=" + pageData["FID"];
    paras += "&WorkID=" + pageData["OID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;

    //  paras += '&WorkID=' + pageData.WorkID;
    if (sta == 2)//只读
    {
        // src += "&DoType=View";
    }
    else {
        fwcOnload = "onload= 'WC" + wf_node.NodeID + "load();'";
        $('body').append(addLoadFunction("WC" + wf_node.NodeID, "blur", "SaveDtl"));
    }
    src += "&r=q" + paras;

    var eleHtml = '<div >' + "<iframe style='width:100%' height=" + h + 800 + "' id='FWC' src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto ></iframe>" + '</div>';

    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y + 'px').css('left', x + 'px').css('width', w + 'px').css('height', h + 'px');
    return eleHtml;
}

var pageData = {};
//初始化网页URL参数
function initPageParam() {
    pageData.FK_Flow = GetQueryString("FK_Flow");
    pageData.FK_Node = GetQueryString("FK_Node");
    pageData.FID = GetQueryString("FID") == null ? 0 : GetQueryString("FID");

    var oid = GetQueryString("WorkID");
    if (oid == null)
        oid = GetQueryString("OID");
    if (oid == null)
        oid = 0;
    pageData.OID = oid;

    pageData.Paras = GetQueryString("Paras");
    var isReadonly = GetQueryString("IsReadonly");
    if (isReadonly == null || isReadonly == undefined || isReadonly == "" || isReadonly == "0")
        pageData.IsReadonly = 0;
    else
        pageData.IsReadonly = 1;
    pageData.IsStartFlow = GetQueryString("IsStartFlow"); //是否是启动流程页面 即发起流程

    pageData.DoType1 = GetQueryString("DoType")//View
    pageData.FK_MapData = GetQueryString("FK_MapData")//View


}



//设置表单元素不可用
function setFormEleDisabled() {
    //文本框等设置为不可用
    $('#divCCForm textarea').attr('disabled', 'disabled');
    $('#divCCForm select').attr('disabled', 'disabled');
    $('#divCCForm input[type!=button]').attr('disabled', 'disabled');
}







//初始化下拉列表框的OPERATION
function InitDDLOperation(frmData, mapAttr, defVal) {
    var operations = '';
    var data = frmData[mapAttr.KeyOfEn];
    if (data == undefined)
        data = frmData[mapAttr.UIBindKey];
    if (data == undefined) {
        //枚举类型的.
        if (mapAttr.LGType == 1) {
            var enums = frmData.Sys_Enum;
            enums = $.grep(enums, function (value) {
                return value.EnumKey == mapAttr.UIBindKey;
            });


            $.each(enums, function (i, obj) {
                operations += "<option " + (obj.IntKey == mapAttr.DefVal ? " selected='selected' " : "") + " value='" + obj.IntKey + "'>" + obj.Lab + "</option>";
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
function ConvertDefVal(frmData, defVal, keyOfEn) {
    //计算URL传过来的表单参数@TXB_Title=事件测试

    var result = defVal;

    //通过MAINTABLE返回的参数
    for (var ele in frmData.MainTable[0]) {
        if (keyOfEn == ele && frmData.MainTable[0] != '') {
            result = frmData.MainTable[0][ele];
            break;
        }
    }


    if (result != undefined && typeof (result) == 'string') {
        //result = result.replace(/｛/g, "{").replace(/｝/g, "}").replace(/：/g, ":").replace(/，/g, ",").replace(/【/g, "[").replace(/】/g, "]").replace(/；/g, ";").replace(/~/g, "'").replace(/‘/g, "'").replace(/‘/g, "'");
    }
    return result = unescape(result);
}

//获取表单数据
function getFormData(isCotainTextArea, isCotainUrlParam) {
    var formss = $('#divCCForm').serialize();
    var formArr = formss.split('&');
    var formArrResult = [];
    //获取CHECKBOX的值
    $.each(formArr, function (i, ele) {

        if (ele.split('=')[0].indexOf('CB_') == 0) {
            if ($('#' + ele.split('=')[0] + ':checked').length == 1) {
                ele = ele.split('=')[0] + '=1';
            } else {
                ele = ele.split('=')[0] + '=0';
            }
        }

        if (ele.split('=')[0].indexOf('DDL_') == 0) {

            var ctrlID = ele.split('=')[0];

            var item = $("#" + ctrlID).find("option:selected").text();

            var mystr = "TB_" + ctrlID.substring(4) + 'T=' + item;
            formArrResult.push(mystr);
        }


        formArrResult.push(ele);
    });



    //获取树形结构的表单值
    var combotrees = $(".easyui-combotree");
    $.each(combotrees, function (i, combotree) {
        var name = $(combotree).attr('id');
        var tree = $('#' + name).combotree('tree');
        //获取当前选中的节点
        var data = tree.tree('getSelected');
        if (data != null) {
            formArrResult.push(name + '=' + data.id);
            formArrResult.push(name + 'T=' + data.text);
        }
    });

    //获取表单中禁用的表单元素的值
    var disabledEles = $('#divCCForm :disabled');
    $.each(disabledEles, function (i, disabledEle) {
        var name = $(disabledEle).attr('name');
        switch (disabledEle.tagName.toUpperCase()) {
            case "INPUT":
                switch (disabledEle.type.toUpperCase()) {
                    case "CHECKBOX": //复选框
                        formArrResult.push(name + '=' + ($(disabledEle).is(':checked') ? 1 : 0));
                        break;
                    case "TEXT": //文本框
                    case "HIDDEN":
                        formArrResult.push(name + '=' + $(disabledEle).val());
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
                formArrResult.push(name + '=' + $(disabledEle).children('option:checked').val());
                break;

            //文本区域      
            case "TEXTAREA":
                formArrResult.push(name + '=' + $(disabledEle).val());
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
    return formdataResultStr;
}

//根据下拉框选定的值，弹出提示信息  绑定那个元素显示，哪个元素不显示  
function ShowNoticeInfo() {

    var rbs = frmData.Sys_FrmRB;
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
            //if (methodVal == value &&  obj.target.name.indexOf(drdlColName) == (obj.target.name.length - drdlColName.length)) {
            if (methodVal == value && (obj.target.name == drdlColName)) {
                //高级JS设置;  设置表单字段的  可用 可见 不可用 
                var fieldConfig = data[j].FieldsCfg;
                var fieldConfigArr = fieldConfig.split('@');
                for (var k = 0; k < fieldConfigArr.length; k++) {
                    var fieldCon = fieldConfigArr[k];
                    if (fieldCon != '' && fieldCon.split('=').length == 2) {
                        var fieldConArr = fieldCon.split('=');
                        var ele = $('[name$=' + fieldConArr[0] + ']');
                        if (ele.length == 0) {
                            continue;
                        }
                        var labDiv = undefined;
                        var eleDiv = undefined;
                        if (ele.css('display').toUpperCase() == "NONE") {
                            continue;
                        }

                        if (ele.parent().attr('class').indexOf('input-group') >= 0) {
                            labDiv = ele.parent().parent().prev();
                            eleDiv = ele.parent().parent();
                        } else {
                            labDiv = ele.parent().prev();
                            eleDiv = ele.parent();
                        }
                        switch (fieldConArr[1]) {
                            case "1": //可用
                                if (labDiv.css('display').toUpperCase() == "NONE" && ele[0].id.indexOf('DDL_') == 0) {
                                    needShowDDLids.push(ele[0].id);
                                }

                                labDiv.css('display', 'block');
                                eleDiv.css('display', 'block');
                                ele.removeAttr('disabled');


                                break;
                            case "2": //可见
                                if (labDiv.css('display').toUpperCase() == "NONE" && ele[0].id.indexOf('DDL_') == 0) {
                                    needShowDDLids.push(ele[0].id);
                                }

                                labDiv.css('display', 'block');
                                eleDiv.css('display', 'block');
                                break;
                            case "3": //不可见
                                labDiv.css('display', 'none');
                                eleDiv.css('display', 'none');
                                break;
                        }
                    }
                }
                //根据下拉列表的值选择弹出提示信息
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
                //$("#btnNoticeInfo").popover('show');
                //$('#btnNoticeInfo').trigger('click');
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

    var mapAttr = frmData.Sys_MapAttr;
    mapAttr = $.grep(mapAttr, function (attr) {
        var atParams = attr.AtPara;
        return atParams != undefined && AtParaToJson(atParams).Tip != undefined && AtParaToJson(atParams).Tip != '' && $('#TB_' + attr.KeyOfEn).length > 0 && $('#TB_' + attr.KeyOfEn).css('display') != 'none';
    })

    $.each(mapAttr, function (i, attr) {
        $('#TB_' + attr.KeyOfEn).bind('focus', function (obj) {

            var frmData = JSON.parse(jsonStr);
            var mapAttr = frmData.Sys_MapAttr;

            mapAttr = $.grep(mapAttr, function (attr) {
                return 'TB_' + attr.KeyOfEn == obj.target.id;
            })
            var atParams = AtParaToJson(mapAttr[0].AtPara);
            var noticeInfo = atParams.Tip;

            if (noticeInfo == undefined || noticeInfo == '')
                return;

            //noticeInfo = noticeInfo.replace(/\\n/g, '<br/>')

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
//必填项检查   名称最后是*号的必填  如果是选择框，值为'' 或者 显示值为 【*请选择】都算为未填 返回FALSE 检查必填项失败
function CheckBlanks() {
    var checkBlankResult = true;
    //获取所有的列名 找到带* 的LABEL mustInput
    //var lbs = $('[class*=col-md-1] label:contains(*)');
    var lbs = $('.mustInput');
    $.each(lbs, function (i, obj) {
        if ($(obj).parent().css('display') != 'none' && $(obj).parent().next().css('display')) {
            var keyofen = $(obj).data().keyofen
            var ele = $('[id$=_' + keyofen + ']');
            if (ele.length == 0)
                return;

            $.each(ele, function (i, obj) {
                var eleM = $(obj);
                switch (eleM[0].tagName.toUpperCase()) {
                    case "INPUT":
                        if (eleM.attr('type') == "text") {
                            if (eleM.val() == "") {
                                checkBlankResult = false;
                                eleM.addClass('errorInput');
                            } else {
                                eleM.removeClass('errorInput');
                            }
                        }
                        break;
                    case "SELECT":
                        if (eleM.val() == "" || eleM.children('option:checked').text() == "*请选择") {
                            checkBlankResult = false;
                            eleM.addClass('errorInput');
                        } else {
                            eleM.removeClass('errorInput');
                        }
                        break;
                    case "TEXTAREA":
                        if (eleM.val() == "") {
                            checkBlankResult = false;
                            eleM.addClass('errorInput');
                        } else {
                            eleM.removeClass('errorInput');
                        }
                        break;
                }
            });
        }
    });


    return checkBlankResult;
}

//正则表达式检查
function CheckReg() {
    var CheckRegResult = true;
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
                    CheckRegResult = false;
                } else {
                    $(obj).removeClass('errorInput');
                }
            }
        }
    });
    return CheckRegResult;
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
        var src = '/DataUser/Siganture/' + val + '.jpg';    //新图片地址
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
    var url = basePath+ "/WF/CCForm/HandWriting.htm?WorkID=" + pageData.OID + "&FK_Node=" + pageData.FK_Node + "&KeyOfEn=" + HandWriteID;
    OpenEasyUiDialogExt(url, '签字板', 400, 300, false);
}
//地图
function figure_Template_Map(MapID, UIIsEnable) {
    var mainTable = frmData.MainTable[0];
    var AtPara = "";
    //通过MAINTABLE返回的参数
    for (var ele in mainTable) {
        if (ele == "AtPara" && mainTable != '') {
            AtPara = mainTable[ele];
            break;
        }
    }

    var url = basePath + "/WF/CCForm/Map.htm?WorkID=" + pageData.WorkID + "&FK_Node=" + pageData.FK_Node + "&KeyOfEn=" + MapID + "&UIIsEnable=" + UIIsEnable + "&Paras=" + AtPara;
    OpenBootStrapModal(url, "eudlgframe", "地图", 800, 500, null, false, function () { }, null, function () {

    });
}
function setHandWriteSrc(HandWriteID, imagePath) {
    imagePath = "../../" + imagePath.substring(imagePath.indexOf("DataUser"));
    document.getElementById("Img" + HandWriteID).src = "";
    $("#Img" + HandWriteID).attr("src", imagePath);
    // document.getElementById("Img" + HandWriteID).src = imagePath;
    $("#TB_" + HandWriteID).val(imagePath);
    $('#eudlg').dialog('close');
}