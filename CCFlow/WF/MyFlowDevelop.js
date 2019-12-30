
var flowData = null;

function GenerDevelopFrm(wn, fk_mapData) {

    flowData = wn;

    $('head').append('<style type="text/css"> .form-control{display: inline; }</style >');

    //加载开发者表单的内容
    var filename = basePath + "/DataUser/CCForm/HtmlTemplateFile/" + fk_mapData + ".htm";
    var htmlobj = $.ajax({ url: filename, async: false });
    var htmlContent = "";
    if (htmlobj.status == 404) {
        //数据库中查找
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_DevelopDesigner");
        handler.AddPara("FK_MapData", fk_mapData);
        htmlContent = handler.DoMethodReturnString("Designer_Init");
    } else {
        htmlContent = htmlobj.responseText;
        if (htmlContent == null && htmlContent == "" && htmlContent == undefined) {
            htmlContent = "";
        }
    }
    if (htmlContent == "") {
        alert("开发者设计的表单内容丢失，请联系管理员");
        return;
    }

    htmlContent = htmlContent.replace(new RegExp("../../../", 'gm'), "../");
    $("#CCForm").html(htmlContent);

    //解析表单中的数据

    //1.加载隐藏字段，设置字段的宽度属性
    var mapAttrs = flowData.Sys_MapAttr;
    var html = "";
    for (var i = 0; i < mapAttrs.length; i++) {
        var mapAttr = mapAttrs[i];
        $('#TB_' + mapAttr.KeyOfEn).css('width', mapAttr.UIWidth);
        $('#CB_' + mapAttr.KeyOfEn).css('width', mapAttr.UIWidth);
        $('#RB_' + mapAttr.KeyOfEn).css('width', mapAttr.UIWidth);
        $('#DDL_' + mapAttr.KeyOfEn).css('width', mapAttr.UIWidth);

        if ((mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1)//外部数据源
            || (mapAttr.LGType == "2" && mapAttr.MyDataType == "1")) {
            var _html = InitDDLOperation(flowData, mapAttr, null);
            $("#DDL_" + mapAttr.KeyOfEn).empty();
            $("#DDL_" + mapAttr.KeyOfEn).append(_html);

        }

        if (mapAttr.UIVisible == 0 && $("#TB_" + mapAttr.KeyOfEn).length == 0) {
            var defval = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
            html = "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + defval + "' />";
            html = $(html);
            $('#CCForm').append(html);
            continue;
        }
        if (mapAttr.MyDataType == 1) {
            if (mapAttr.UIContralType == 8)//手写签字版
            {
                var element = $("#Img" + mapAttr.KeyOfEn);
                var defValue = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
                var ondblclick = ""
                if (mapAttr.UIIsEnable == 1) {
                    ondblclick = " ondblclick='figure_Template_HandWrite(\"" + mapAttr.KeyOfEn + "\",\"" + defValue + "\")'";
                }

                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "'  value='" + defValue + "' type=hidden />";
                var eleHtml = "<img src='" + defValue + "' " + ondblclick + " onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\"  style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                element.after(eleHtml);
                element.remove(); //移除Imge节点
            }
            if (mapAttr.UIContralType == 4)//地图
            {
                var obj = $("#TB_" + mapAttr.KeyOfEn);
                //获取兄弟节点
                $(obj.prev()).attr("onclick", "figure_Template_Map('" + mapAttr.KeyOfEn + "','" + mapAttr.UIIsEnable + "')");
            }
            if (mapAttr.UIContralType == 101)//评分
            {
                var scores = $(".simplestar");//获取评分的类
                $.each(scores, function (score, idx) {
                    $.each($(this).children("Img"), function () {
                        $(this).attr("src", $(this).attr("src").replace("../../", "./"));
                    });
                });

            }
        }
    }


    //外键、外部数据源增加选择项option
    var selects = $("select");
    $.each(selects, function (obj, i) {
        var _html = InitDDLOperation(flowData, mapAttr, null)
    })



    //2.解析控件 从表、附件、附件图片、框架、地图、签字版、父子流程
    var frmDtls = flowData.Sys_MapDtl;
    for (var i = 0; i < frmDtls.length; i++) {
        var frmDtl = frmDtls[i];
        //根据data-key获取从表元素
        var element = $("Img[data-key=" + frmDtl.No + "]");
        if (element.length == 0)
            continue;
        figure_Develop_Dtl(element, frmDtl);

    }
    var aths = flowData.Sys_FrmAttachment;//附件
    for (var i = 0; i < aths.length; i++) {
        var ath = aths[i];
        //根据data-key获取从表元素
        var element = $("Img[data-key=" + ath.MyPK + "]");
        if (element.length == 0)
            continue;
        figure_Develop_Ath(element, ath);

    }

    //图片附件
    var athImgs = flowData.Sys_FrmImgAth;
    if (athImgs.length > 0) {
        var imgSrc = "<input type='hidden' id='imgSrc'/>";
        $('#CCForm').append(imgSrc);
    }
    for (var i = 0; i < athImgs.length; i++) {
        var athImg = athImgs[i];
        //根据data-key获取从表元素
        var element = $("Img[data-key=" + athImg.MyPK + "]");
        if (element.length == 0)
            continue;
        figure_Develop_ImageAth(element, athImg, fk_mapData);

    }

    //图片
    var imgs = flowData.Sys_FrmImg;
    for (var i = 0; i < imgs.length; i++) {
        var img = imgs[i];
        //根据data-key获取从表元素
        var element = $("Img[data-key=" + img.MyPK + "]");
        if (element.length == 0)
            continue;
        figure_Develop_Image(element, img);

    }
    var iframes = flowData.Sys_MapFrame;//框架
    for (var i = 0; i < iframes.length; i++) {
        var iframe = iframes[i];
        //根据data-key获取从表元素
        var element = $("Img[data-key=" + iframe.MyPK + "]");
        if (element.length == 0)
            continue;
        figure_Develop_IFrame(element, iframe);

    }
    if (flowData.WF_FrmNodeComponent != null && flowData.WF_FrmNodeComponent != undefined) {
        var nodeComponents = flowData.WF_FrmNodeComponent[0];//节点组件
        if (nodeComponents != null) {
            var element = $("Img[data-key=" + nodeComponents.NodeID + "]");
            if (element.length != 0)
                figure_Develop_FigureSubFlowDtl(nodeComponents, element, fk_mapData);
            //如果有审核组件，增加审核组件的HTML
            var _html = figure_Develop_FigureFrmCheck(nodeComponents, flowData, flowData.Sys_MapData[0]);
            $("#CCForm").append(_html);

        }
    }

}

//从表
function figure_Develop_Dtl(element, frmDtl, ext) {
    //$("<link href='../Comm/umeditor1.2.3-utf8/themes/default/css/umeditor.css' type = 'text/css' rel = 'stylesheet' />").appendTo("head");  
    //在Image元素下引入IFrame文件
    var src = "";
    if (frmDtl.ListShowModel == "0")
        //表格模式
        src = "./CCForm/Dtl2017.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + pageData.OID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=" + pageData.IsReadonly + "Version=1";

    if (frmDtl.ListShowModel == "1")
        //卡片模式
        src = "./CCForm/DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + pageData.OID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=" + pageData.isReadonly + "&Version=1";

    var W = element.width();
    var eleHtml = $("<div id='Fd" + frmDtl.No + "' style='width:" + W + "px; height:auto;' ></div>");

    var eleIframe = $("<iframe class= 'Fdtl' ID = 'Dtl_" + frmDtl.No + "' src = '" + src + "' frameborder=0  style='width:" + W + "px;"
        + "height: auto; text-align: left; '  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
    eleHtml.append(eleIframe);
    $(element).after(eleHtml);
    $(element).remove(); //移除Imge节点
}

//附件
function figure_Develop_Ath(element, ath) {
    var src = "./CCForm/Ath.htm?PKVal=" + pageData.OID + "&PWorkID=" + GetQueryString("PWorkID") + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=" + pageData.isReadonly + "&FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow;

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
function figure_Develop_ImageAth(element, frmImageAth, fk_mapData) {
    var isEdit = frmImageAth.IsEdit;
    var img = $("<img class='pimg'/>");

    var imgSrc = "./Data/Img/LogH.PNG";

    var refpkVal = pageData.OID;
    //获取数据
    if (fk_mapData.indexOf("ND") != -1)
        imgSrc = basePath + "/DataUser/ImgAth/Data/" + frmImageAth.CtrlID + "_" + refpkVal + ".png";
    else
        imgSrc = basePath + "/DataUser/ImgAth/Data/" + fk_mapData + "_" + frmImageAth.CtrlID + "_" + refpkVal + ".png";

    //设计属性
    img.attr('id', 'Img' + frmImageAth.MyPK).attr('name', 'Img' + frmImageAth.MyPK);
    img.attr("src", imgSrc).attr('onerror', "this.src='" + basePath + "/WF/Admin/CCFormDesigner/Controls/DataView/AthImg.png'");
    img.css('width', element.width()).css('height', element.height()).css('padding', "0px").css('margin', "0px").css('border-width', "0px");
    //不可编辑
    if (isEdit == "1" && pageData.IsReadonly != 1) {
        var fieldSet = $("<fieldset style='display:inline'></fieldset>");
        var length = $("<legend></legend>");
        var a = $("<a></a>");
        var url = basePath + "/WF/CCForm/ImgAth.htm?W=" + frmImageAth.W + "&H=" + frmImageAth.H + "&FK_MapData=" + fk_mapData + "&RefPKVal=" + refpkVal + "&CtrlID=" + frmImageAth.CtrlID;

        a.attr('href', "javascript:ImgAth('" + url + "','" + frmImageAth.MyPK + "');").html("编辑");
        length.css('font-style', 'inherit').css('font-weight', 'bold').css('font-size', '12px').css('width', frmImageAth.W);

        fieldSet.append(length);
        length.append(a);
        fieldSet.append(img);
        $(element).after(fieldSet);

    } else {
        $(element).after(img);
    }
    $(element).remove(); //移除Imge节点
}

//图片附件编辑
function ImgAth(url, athMyPK) {
    var dgId = "iframDg";
    url = url + "&s=" + Math.random();

    OpenBootStrapModal(url, dgId, '图片附件', 900, 580, 'icon-new', false, function () {

    }, null, function () {
        //关闭也切换图片
        var imgSrc = $("#imgSrc").val();
        if (imgSrc != null && imgSrc != "")
            document.getElementById('Img' + athMyPK).setAttribute('src', imgSrc + "?t=" + Math.random());
        $("#imgSrc").val("");
    });
}

//初始化 IMAGE  只初始化了图片类型
function figure_Develop_Image(element, frmImage) {
    //解析图片
    if (frmImage.ImgAppType == 0) { //图片类型
        //数据来源为本地.
        var imgSrc = '';
        if (frmImage.ImgSrcType == 0) {
            //替换参数
            var frmPath = frmImage.ImgPath;
            frmPath = frmPath.replace('＠', '@');
            frmPath = frmPath.replace('@basePath', basePath);
            frmPath = frmPath.replace('@basePath', basePath);
            imgSrc = DealJsonExp(flowData.MainTable[0], frmPath);
        }

        //数据来源为指定路径.
        if (frmImage.ImgSrcType == 1) {
            var url = frmImage.ImgURL;
            url = url.replace('＠', '@');
            url = url.replace('@basePath', basePath);
            imgSrc = DealJsonExp(flowData.MainTable[0], url);
        }
        // 由于火狐 不支持onerror 所以 判断图片是否存在放到服务器端
        if (imgSrc == "" || imgSrc == null)
            imgSrc = "../DataUser/ICON/CCFlow/LogBig.png";

        var a = $("<a></a>");
        var img = $("<img/>")

        img.attr("src", imgSrc).css('width', frmImage.W).css('height', frmImage.H).attr('onerror', "this.src='../DataUser/ICON/CCFlow/LogBig.png'");

        if (frmImage.LinkURL != undefined && frmImage.LinkURL != '') {
            a.attr('href', frmImage.LinkTarget).attr('target', frmImage.LinkTarget).css('width', frmImage.W).css('height', frmImage.H);
            a.append(img);
            $(element).after(a);
        } else {
            $(element).after(img);
        }




        $(element).remove(); //移除Imge节点
    } else if (frmImage.ImgAppType == 3)//二维码  手机
    {


    } else if (frmImage.ImgAppType == 1) {//暂不解析
        //电子签章  写后台
    }
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
function figure_Develop_IFrame(element, frame) {

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
                                url = url.replace(paramArr[1], flowData.MainTable[0][paramArr[1].substr('@WebUser.'.length)]);
                            else
                                url = url.replace(paramArr[1], flowData.MainTable[0][paramArr[1].substr(1)]);
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
        var gwf = flowData.WF_GenerWorkFlow[0];
        if (gwf != null) {
            var atPara = gwf.AtPara;
            if (atPara != null && atPara != "") {
                atPara = atPara.replace(/@/g, '&');
                url = url + atPara;
            }
        }
    }
    if (urlType == 2) //轨迹表
        url = "./WorkOpt/OneWork/Table.htm?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.OID + "&FID=" + pageData.FID;
    if (urlType == 3)//轨迹图
        url = "./WorkOpt/OneWork/TimeBase.htm?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.OID + "&FID=" + pageData.FID;

    var eleHtml = $("<div id='Frame" + frame.MyPK + "' style='width:" + frame.W + "px; height:auto;' ></div>");

    var eleIframe = $("<iframe class= 'Fdtl' ID = 'Frame_" + frame.MyPK + "' src = '" + url + "' frameborder=0  style='width:" + frame.W + "px;"
        + "height: auto; text-align: left; '  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");


    eleHtml.append(eleIframe);
    $(element).after(eleHtml);
    $(element).remove(); //移除Frame节点

}


//子流程
function figure_Develop_FigureSubFlowDtl(wf_node, element) {

    //@这里需要处理, 对于流程表单.
    if (sta == 0 || sta == "0" || sta == undefined)
        return $('');

    var sta = wf_node.SFSta;
    var x = wf_node.SF_X;
    var y = wf_node.SF_Y;
    var h = wf_node.SF_H;
    var w = wf_node.SF_W;

    var src = "./WorkOpt/SubFlow.htm?s=2";
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

    var eleHtml = $("<div id=''SubFlow" + wf_node.NodeID + "' style='width:" + w + "px; height:auto;' ></div>");

    var eleIframe = $("<iframe class= 'Fdtl' ID = 'SubFlow_" + wf_node.NodeID + "' src = '" + src + "' frameborder=0  style='width:" + w + "px;"
        + "height: auto; text-align: left; '  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");


    eleHtml.append(eleIframe);
    $(element).after(eleHtml);
    $(element).remove(); //移除SubFlow节点

}


//审核组件
function figure_Develop_FigureFrmCheck(wf_node, flowData, mapData) {



    var sta = wf_node.FWCSta;
    if (sta == 0 || sta == undefined)
        return $('');

    var node = flowData.WF_Node;
    if (node != undefined)
        node = node[0];

    var frmNode = flowData.WF_FrmNode;
    if (frmNode != undefined)
        frmNode = frmNode[0];

    if (node == null || frmNode == null)
        return $('');
    if (node.FormType == 5 && frmNode.IsEnableFWC != 1)
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
        src = "./WorkOpt/WorkCheck.htm?s=2&IsReadonly=" + GetQueryString("IsReadonly");
    else
        src = "./WorkOpt/WorkCheck2019.htm?s=2&IsReadonly=" + GetQueryString("IsReadonly");
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



//双击签名
function figure_Develop_Siganture(SigantureID, val, type) {
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
        var src = '../DataUser/Siganture/' + val + '.jpg';    //新图片地址
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
function figure_Develop_HandWrite(HandWriteID, val) {
    var url = basePath + "/WF/CCForm/HandWriting.htm?WorkID=" + pageData.OID + "&FK_Node=" + pageData.FK_Node + "&KeyOfEn=" + HandWriteID;
    OpenBootStrapModal(url, "eudlgframe", '签字板', 400, 300, false);
}
//地图
function figure_Develop_Map(MapID, UIIsEnable) {
    var mainTable = flowData.MainTable[0];
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
    imagePath = "../" + imagePath.substring(imagePath.indexOf("DataUser"));
    document.getElementById("Img" + HandWriteID).src = "";
    $("#Img" + HandWriteID).attr("src", imagePath);
    $("#TB_" + HandWriteID).val(imagePath);
    $('#eudlg').dialog('close');
}

