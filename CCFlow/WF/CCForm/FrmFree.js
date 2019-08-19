
var isSigantureChecked = false;
function GenerFreeFrm(mapData, frmData) {

    //循环FrmRB
    for (var i in frmData.Sys_FrmRB) {
        var frmLab = frmData.Sys_FrmRB[i];
        var label = figure_Template_Rb(frmLab);
        $('#CCForm').append(label);
    }

    //循环MapAttr
    for (var mapAtrrIndex in frmData.Sys_MapAttr) {
        var mapAttr = frmData.Sys_MapAttr[mapAtrrIndex];
        if (mapAttr.UIContralType == 3) {
            if (mapAttr.UIIsEnable == 0)
                $('input[name=RB_' + mapAttr.KeyOfEn + ']').attr("disabled", "disabled");
            continue;
        }
        var eleHtml = figure_MapAttr_Template(mapAttr);
        $('#CCForm').append(eleHtml);
    }

    //循环FrmLab
    for (var i in frmData.Sys_FrmLab) {
        var frmLab = frmData.Sys_FrmLab[i];
        var label = figure_Template_Label(frmLab);
        $('#CCForm').append(label);
    }

    //循环FrmBtn
    for (var i in frmData.Sys_FrmBtn) {
        var frmBtn = frmData.Sys_FrmBtn[i];
        var btn = figure_Template_Btn(frmBtn);
        $('#CCForm').append(btn);
    }

    //循环Image
    for (var i in frmData.Sys_FrmImg) {
        var frmImg = frmData.Sys_FrmImg[i];
        var createdFigure = figure_Template_Image(frmImg);
        $('#CCForm').append(createdFigure);
    }

    //循环 Link
    for (var i in frmData.Sys_FrmLink) {
        var frmLink = frmData.Sys_FrmLink[i];
        var createdFigure = figure_Template_HyperLink(frmLink);
        $('#CCForm').append(createdFigure);
    }

    //循环 图片附件
    if (frmData.Sys_FrmImgAth.length > 0) {
        var imgSrc = "<input type='hidden' id='imgSrc'/>";
        $('#CCForm').append(imgSrc);
    }
    for (var i in frmData.Sys_FrmImgAth) {
        var frmImgAth = frmData.Sys_FrmImgAth[i];
        var createdFigure = figure_Template_ImageAth(frmImgAth);
        $('#CCForm').append(createdFigure);
    }


    //循环 附件
    for (var i in frmData.Sys_FrmAttachment) {
        var frmAttachment = frmData.Sys_FrmAttachment[i];
        if (frmAttachment.IsVisable == 0)
            continue;
        var createdFigure = figure_Template_Attachment(frmAttachment);
        $('#CCForm').append(createdFigure);
    }

    // 主表扩展(统计从表)
    var detailExt = {};
    // 装载公式 从表id -> 公式
    $.each(frmData.Sys_MapExt, function (i, o) {
        if (o.ExtType == "AutoFullDtlField") {	// 明细统计公式
            // 从表id.列.[Sum|Avg|Max|Min] -> CCFrm_CongBiaoCeShiDtl1.ShanJia.Avg
            var docs = o.Doc.split("\.");
            //判断是否显示大写
            var tag3 = o.Tag3;
            var DaXieAttrOfOper = "";
            if (tag3 == 1)
                DaXieAttrOfOper = o.Tag4;

            if (docs.length == 3) {
                var ext = {
                    "DtlNo": docs[0],
                    "FK_MapData": o.FK_MapData,
                    "AttrOfOper": o.AttrOfOper,
                    "DaXieAttrOfOper": DaXieAttrOfOper,
                    "Doc": o.Doc,
                    "DtlColumn": docs[1],
                    "exp": docs[2],
                    "Tag": o.Tag,
                    "Tag1": o.Tag1
                };

                if (!$.isArray(detailExt[ext.DtlNo])) {
                    detailExt[ext.DtlNo] = [];
                }
                detailExt[ext.DtlNo].push(ext);
                $(":input[name=TB_" + ext.AttrOfOper + "]").attr("readonly", true);
            }
        }
    });

    //循环 从表
    for (var i in frmData.Sys_MapDtl) {
        var frmMapDtl = frmData.Sys_MapDtl[i];
        var createdFigure = figure_Template_Dtl(frmMapDtl, detailExt[frmMapDtl.No]); // 根据从表id获取公式 从表id -> 公式
        $('#CCForm').append(createdFigure);
    }

    //循环线
    for (var i in frmData.Sys_FrmLine) {
        var frmLine = frmData.Sys_FrmLine[i];
        var createdConnector = connector_Template_Line(frmLine);
        $('#CCForm').append(createdConnector);
    }

    //循环Sys_MapFrame
    for (var i in frmData.Sys_MapFrame) {
        var frame = frmData.Sys_MapFrame[i];
        var alertMsgEle = figure_Template_IFrame(frame);
        $('#CCForm').append(alertMsgEle);
    }


    //循环组件 轨迹图 审核组件 子流程 子线程
    if (frmData["WF_FrmNodeComponent"] != null) {
        var wf_FrmNodeComponent = frmData["WF_FrmNodeComponent"][0];

        if (wf_FrmNodeComponent != null) {

            $('#CCForm').append(figure_Template_FigureFlowChart(wf_FrmNodeComponent, mapData));
            $('#CCForm').append(figure_Template_FigureFrmCheck(wf_FrmNodeComponent, mapData, frmData));
            $('#CCForm').append(figure_Template_FigureSubFlowDtl(wf_FrmNodeComponent, mapData));
            $('#CCForm').append(figure_Template_FigureThreadDtl(wf_FrmNodeComponent, mapData));
        }
    }

}


//初始化轨迹图
function figure_Template_FigureFlowChart(wf_node, mapData) {

    //轨迹图
    var sta = wf_node.FrmTrackSta;
    if (sta == 0 || sta == undefined)
        return $('');

    var x = wf_node.FrmTrack_X;
    var y = wf_node.FrmTrack_Y;
    var h = wf_node.FrmTrack_H;
    var w = wf_node.FrmTrack_W;

    var src = "./WorkOpt/OneWork/OneWork.htm?CurrTab=Track";
    src += '&FK_Flow=' + pageData.FK_Flow;
    src += '&FK_Node=' + pageData.FK_Node;
    src += '&WorkID=' + pageData.OID;
    src += '&FID=' + pageData.FID;
    var eleHtml = '<div id="divtrack' + wf_node.NodeID + '">' + "<iframe id='track" + wf_node.NodeID + "' style='width:" + w + "px;height=" + h + "px;'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y + 'px').css('left', x + 'px').css('width', w + 'px').css('height', h + 'px');

    return eleHtml;
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

//把FlowCtrls格式转化为. json. @Ctrl=FrmCheck,X=10,Y=100,H=90,W=39
function PreaseFlowCtrls(flowCtrls, ctrlID) {

    if (flowCtrls == "" || flowCtrls == null || flowCtrls == undefined)
        return null;

    //先用@符号分开.
    var at = flowCtrls.split('@');

    //遍历他.
    for (var i = 0; i < at.length; i++) {
        var str = at[i];
        if (str.indexOf(ctrlID) == -1)
            continue;

        //现在str 的格式为  Ctrl=FrmCheck,X=10,Y=100,H=90,W=39

        var strs = str.split(',');

        var jsonObj = {};
        $.each(strs, function (i, o) {
            var kv = o.split("=");
            if (kv.length == 2) {
                jsonObj[kv[0]] = kv[1];
            }
        });

        return jsonObj;

        //alert(jsonObj + " -> " + JSON.stringify(jsonObj));

    }
}

//子线程
function figure_Template_FigureThreadDtl(wf_node, mapData) {

    //FrmThreadSta Sta,FrmThread_X X,FrmThread_Y Y,FrmThread_H H,FrmThread_W
    var sta = wf_node.FrmThreadSta;
    if (sta == 0 || sta == '0' || sta == undefined)
        return $('');

    var x = wf_node.FrmThread_X;
    var y = wf_node.FrmThread_Y;
    var h = wf_node.FrmThread_H;
    var w = wf_node.FrmThread_W;
    if (sta == 0 || sta == '0')
        return $('');

    var src = "../WorkOpt/Thread.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData.OID;
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.OID;

    if (sta == 2) //只读
    {
        src += "&DoType=View";
    }
    else {
        fwcOnload = "onload= 'WC" + wf_node.NodeID + "load();'";
        $('body').append(addLoadFunction("WC" + wf_node.NodeID, "blur", "SaveDtl"));
    }

    src += "&r=q" + paras;
    var eleHtml = '<div id=DIVFT' + wf_node.NodeID + '>' + "<iframe id=FFT" + wf_node.NodeID + " style='width:100%;height:" + h + "px;'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);

    return eleHtml;
}

//子流程
function figure_Template_FigureSubFlowDtl(wf_node, mapData) {

    //@这里需要处理, 对于流程表单.
    if (sta == 0 || sta == "0" || sta == undefined)
        return $('');

    //SFSta Sta,SF_X X,SF_Y Y,SF_H H, SF_W W
    var sta = wf_node.SFSta;
    var x = wf_node.SF_X;
    var y = wf_node.SF_Y;
    var h = wf_node.SF_H;
    var w = wf_node.SF_W;

    var src = "../WorkOpt/SubFlow.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
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
    var eleHtml = '<div id=DIVWC' + wf_node.NodeID + '>' + "<iframe id=FSF" + wf_node.NodeID + " style='width:" + w + "px';height:" + h + "px'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);

    return eleHtml;
}


//初始化从表
function figure_Template_Dtl(frmDtl, ext) {


    var eleHtml = $("<DIV id='Fd" + frmDtl.No + "' style='position:absolute; left:" + frmDtl.X + "px; top:" + frmDtl.Y + "px; width:" + frmDtl.W + "px; height:" + frmDtl.H + "px;text-align: left;' >");
    var paras = this.pageData;


    var strs = "";
    for (var str in paras) {

        if (str == "EnsName" || str == "RefPKVal" || str == "IsReadonly")
            continue

        var val = paras[str];
        if (val == null || val == "null" || val == undefined || val == "undefined")
            continue;
        strs += "&" + str + "=" + paras[str];
    }
    var src = "";

    // alert(pageData);
    //alert(pageData.IsReadonly);

    if (frmDtl.ListShowModel == "0") {

        var dtlUrl = "Dtl2017";
        if (frmDtl.DtlVer == 1)
            dtlUrl = "Dtl2019";

        var local = window.location.href;
        if (local.indexOf('CCBill') != -1) {
            dtlUrl = '../CCForm/' + dtlUrl;
        }



        if (pageData.IsReadonly == "1") {
            src = dtlUrl + ".htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.OID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=1" + strs + "&m=" + Math.random();
        } else {
            src = dtlUrl + ".htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.OID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=0" + strs + "&m=" + Math.random();
        }

    } else if (frmDtl.ListShowModel == "1") {
        if (pageData.IsReadonly == "1") {
            src = "./DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.OID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=1" + strs + "&m=" + Math.random();
        } else {
            src = "./DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.OID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=0" + strs + "&m=" + Math.random();
        }
    }

    var eleIframe = '<iframe></iframe>';
    eleIframe = $("<iframe ID='Dtl_" + frmDtl.No + "' src='" + src +
                 "' frameborder=0  style='position:absolute;width:" + frmDtl.W + "px; height:" + frmDtl.H +
                 "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling='atuo' /></iframe>");
    if (pageData.IsReadonly) {

    } else {
        if (frmDtl.DtlSaveModel == 0) {
            eleHtml.append(addLoadFunction(frmDtl.No, "blur", "SaveDtl"));
            eleIframe.attr('onload', frmDtl.No + "load()");
        }
    }
    eleHtml.append(eleIframe);

    if (ext) {	// 表达式传入iframe(表达式为数组)
        eleIframe.load(function () {
            /*
            var iframeExp = $(this).contents().find(":input[id=formExt]").val();
            if (iframeExp == null || typeof iframeExp == "undefined" || iframeExp == "") {
            iframeExp = "[]";
            }
            iframeExp = JSON.parse(iframeExp);
            iframeExp.push(ext);
            */
            $(this).contents().find(":input[id=formExt]").val(JSON.stringify(ext));
            if (this.contentWindow && typeof this.contentWindow.parentStatistics === "function") {
                this.contentWindow.parentStatistics(ext);
            }
        });
    }

    //added by liuxc,2017-1-10,此处前台JS中增加变量DtlsLoadedCount记录明细表的数量，用于加载完全部明细表的判断
    var js = "";
    if (!pageData.IsReadonly) {
        js = "<script type='text/javascript' >";
        js += " function SaveDtl(dtl) { ";
        js += "   GenerPageKVs(); //调用产生kvs ";
        js += "\n   var iframe = document.getElementById('F' + dtl );";
        js += "   if(iframe && iframe.contentWindow){ ";
        js += "      iframe.contentWindow.SaveDtlData(); ";
        js += "   } ";
        js += " } ";
        js += " function SaveM2M(dtl) { ";
        js += "   document.getElementById('F' + dtl ).contentWindow.SaveM2M();";
        js += "} ";
        js += "</script>";
        eleHtml.append($(js));
    }
    return eleHtml;
}

//初始化框架
function figure_Template_IFrame(fram) {

    var eleHtml = $("<DIV id='Fd" + fram.MyPK + "' style='position:absolute; left:" + fram.X + "px; top:" + fram.Y + "px; width:" + fram.W + "px; height:" + fram.H + "px;text-align: left;' >");
    //获取框架的类型 0 自定义URL 1 地图开发 2流程轨迹表 3流程轨迹图
    var urlType = fram.UrlSrcType;
    var url = "";
    if (urlType == 0) {
        url = fram.URL;
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


    var eleIframe = '<iframe></iframe>';
    eleIframe = $("<iframe ID='Fdg" + fram.MyPK + "' src='" + url +
	                 "' frameborder=0  style='position:absolute;width:" + fram.W + "px; height:" + fram.H +
	                 "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");

    eleHtml.append(eleIframe);

    return eleHtml;
}

function figure_MapAttr_Template(mapAttr) {

    //根据不同的类型控件，生成html.
    var ele = figure_MapAttr_TemplateEle(mapAttr);

    ele += mapAttr.UIIsInput == 1 ? '<span style="color:red" class="mustInput" data-keyofen="' + mapAttr.KeyOfEn + '">*</span>' : "";

    var eleHtml = $('<div>' + ele + '</div>');
    var W = mapAttr.UIWidth;
    if (mapAttr.MyDataType == 6)
        if (W < 120) W = 120;
    if (mapAttr.MyDataType == 7)
        if (W < 160) W = 160;


    if (mapAttr.IsSigan == "4") {
        eleHtml.css('position', 'absolute').css('top', mapAttr.Y + 'px').css('left', mapAttr.X + 'px');
        eleHtml.css('z-index', '999');
        return eleHtml;
    }
    if (mapAttr.MyDataType != 4) {
        eleHtml.children(0).css('width', W).css('height', mapAttr.UIHeight).css("padding", "0px 6px").css("display", "inline");
    }


    eleHtml.css('position', 'absolute').css('top', mapAttr.Y + 'px').css('left', mapAttr.X + 'px');

    return eleHtml;
}

//升级表单元素 初始化文本框、日期、时间
function figure_MapAttr_TemplateEle(mapAttr) {

    var eleHtml = '';

    /***************** 隐藏的控件 *****************************/
    if (mapAttr.UIVisible == 0) {
        eleHtml = "<input type='hidden' class='form-control' id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "'></input>";
        return eleHtml;
    }

    /***************** 外键 *****************************/
    if (mapAttr.LGType == 2 && mapAttr.MyDataType == "1" && mapAttr.UIContralType == "1") {

        var enableAttr = "";
        if (mapAttr.UIIsEnable == 0) {
            enableAttr = "disabled='disabled'";
        }

        //判断外键是否为树形结构
        var uiBindKey = mapAttr.UIBindKey;
        if (uiBindKey != null && uiBindKey != undefined && uiBindKey != "") {
            var sfTable = new Entity("BP.Sys.SFTable");
            sfTable.SetPKVal(uiBindKey);
            var count = sfTable.RetrieveFromDBSources();
            if (count != 0 && sfTable.CodeStruct == "1") {
                return "<select  id='DDL_" + mapAttr.KeyOfEn + "' class='easyui-combotree' style='width:" + parseInt(mapAttr.UIWidth) * 2 + "px;height:28px'></select>";

            }
        }

        eleHtml = "<select style='padding:0px;' class='form-control' data-val='" + ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' id='DDL_" + mapAttr.KeyOfEn + "'  name='DDL_" + mapAttr.KeyOfEn + "' " + enableAttr + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";

        return eleHtml;
    }

    /***************** 外部数据源 *****************************/
    if (mapAttr.LGType != 2 && mapAttr.MyDataType == "1" && mapAttr.UIContralType == "1") {
        eleHtml = "<select style='padding:0px;' class='form-control' data-val='" + ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' id='DDL_" + mapAttr.KeyOfEn + "' name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
        return eleHtml;
    }

    /***************** 作为附件展示的控件. *****************************/
    if (mapAttr.UIContralType == 6) {
        var atParamObj = AtParaToJson(mapAttr.AtPara);
        if (atParamObj.AthRefObj != undefined) { //扩展设置为附件展示
            eleHtml += "<input class='tbAth form-control' type='hidden'   data-target='" + mapAttr.AtPara + "' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' >" + "</input>";
            defValue = defValue != undefined && defValue != '' ? defValue : '&nbsp;';
            if (defValue.indexOf('@AthCount=') == 0) {
                defValue = "附件" + "<span class='badge'>" + defValue.substring('@AthCount='.length, defValue.length) + "</span>个";
            } else {
                defValue = defValue;
            }
            eleHtml += "<div class='divAth' data-target='" + mapAttr.KeyOfEn + "'  id='DIV_" + mapAttr.KeyOfEn + "'>" + defValue + "</div>";
        }
        return eleHtml;
    }

    /***************** 其他类型的控件. *****************************/

    var str = '';
    var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
    var isInOneRow = false; //是否占一整行
    var islabelIsInEle = false; //
    eleHtml += '';

    //添加文本框 ，日期控件等
    //AppString
    if (mapAttr.MyDataType == "1") {

        //签字板
        if (mapAttr.UIContralType == "8") {
            //如果是图片签名，并且可以编辑
            var ondblclick = ""
            if (mapAttr.UIIsEnable == 1) {
                ondblclick = " ondblclick='figure_Template_HandWrite(\"" + mapAttr.KeyOfEn + "\",\"" + defValue + "\")'";
            }

            var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "'  value='" + defValue + "' type=hidden />";
            eleHtml += "<img src='" + defValue + "' " + ondblclick + " onerror=\"this.src='../../DataUser/Siganture/UnName.jpg'\"  style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
            return eleHtml;
        }

        //普通类型的单行文本.
        if (mapAttr.UIHeight <= 40) {

            //如果是图片签名，并且可以编辑
            if (mapAttr.IsSigan == "1" && mapAttr.UIIsEnable == 1) {
                //查找默认值
                var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' type=hidden />";
                //是否签过
                var sealData = new Entities("BP.Tools.WFSealDatas");
                sealData.Retrieve("OID", GetQueryString("OID"), "FK_Node", GetQueryString("FK_Node"), "SealData", GetQueryString("UserNo"));

                if (sealData.length > 0) {

                    //先判断是否存在签名图片
                    var handler = new HttpHandler("BP.WF.HttpHandler.WF");
                    handler.AddPara('no', GetQueryString("UserNo"));
                    data = handler.DoMethodReturnString("HasSealPic");

                    if (data.length > 0) {
                        eleHtml += data + html;
                    }
                    else {
                        eleHtml += "<img src='/DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='/DataUser/Siganture/Templete.JPG'\"  style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                    }
                    isSigantureChecked = true;
                }
                else {
                    eleHtml += "<img src='/DataUser/Siganture/siganture.jpg' onerror=\"this.src='/DataUser/Siganture/Templete.JPG'\" ondblclick='figure_Template_Siganture(\"" + mapAttr.KeyOfEn + "\",\"" + val + "\",\"0\")' style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                }
                return eleHtml;
            }
            //如果是图片盖章，并且可编辑
            if (mapAttr.IsSigan == "4" && mapAttr.UIIsEnable == 1) {
                //查找默认值
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + mapAttr.DefVal + "' type=hidden />";
                //是否签过
                var sealData = new Entities("BP.Tools.WFSealDatas");
                sealData.Retrieve("OID", GetQueryString("OID"), "FK_Node", GetQueryString("FK_Node"), "SealData", mapAttr.DefVal);

                if (sealData.length > 0) {
                    eleHtml += "<img src='/DataUser/Siganture/" + mapAttr.DefVal + ".jpg' style='border:0px;'  id='Img" + mapAttr.KeyOfEn + "' />" + html;
                    isSigantureChecked = true;
                }
                else {
                    eleHtml += "<img src='/DataUser/Siganture/siganture.jpg'  ondblclick='figure_Template_Siganture(\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.DefVal + "\",\"1\")' style='border:0px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                }
                //eleHtml += "<img src='../DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\" style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                return eleHtml;
            }
            //如果不可编辑，并且是图片名称
            if (mapAttr.IsSigan == "1") {
                var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
                var handler = new HttpHandler("BP.WF.HttpHandler.WF");
                handler.AddPara('no', val);
                data = handler.DoMethodReturnString("HasSealPic");
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' type=hidden />";
                if (data.length > 0) {
                    eleHtml += data + html;
                }
                else {
                    eleHtml += "<img src='/DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='/DataUser/Siganture/Templete.JPG'\" style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;

                }
                return eleHtml;
            }
            if (mapAttr.IsSigan == "4") {
                //var val = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + mapAttr.DefVal + "' type=hidden />";
                eleHtml += "<img src='/DataUser/Siganture/" + mapAttr.DefVal + ".jpg' onerror=\"this.src='/DataUser/Siganture/Templete.JPG'\"  style='border:0px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                //eleHtml += "<img src='../DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\" style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                return eleHtml;
            }

            eleHtml += "<input style='display:inline' class='form-control' maxlength=" + mapAttr.MaxLen + " name='TB_" + mapAttr.KeyOfEn + "'  id='TB_" + mapAttr.KeyOfEn + "' type='text' placeholder='" + (mapAttr.Tip || '') + "' />";

            return eleHtml;
        }

        //判断是否是富文本?
        if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {

            //如果是富文本就使用百度 UEditor

            if (mapAttr.UIIsEnable == "0") {
                //只读状态直接 div 展示富文本内容
                //eleHtml += "<script id='" + editorPara.id + "' name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";
                eleHtml = "<div class='richText' style='width:" + mapAttr.UIWidth + "px'>" + defValue + "</div>";
                return eleHtml;
            }

            document.BindEditorMapAttr = mapAttr; //存到全局备用

            //设置编辑器的默认样式
            var styleText = "text-align:left;font-size:12px;";
            styleText += "width:100%;";
            styleText += "height:" + mapAttr.UIHeight + "px;";
            //注意这里 name 属性是可以用来绑定表单提交时的字段名字的
            eleHtml += "<script id='editor' name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";
            return eleHtml;
        }

        //多行文本模式.
        eleHtml = "<textarea class='form-control' maxlength=" + mapAttr.MaxLen + " style='height:" + mapAttr.UIHeight + "px;' id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "' type='text' />";
        return eleHtml;
    }

    //日期类型.
    if (mapAttr.MyDataType == 6) { //AppDate
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1) {
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
        } else {
            enableAttr = "disabled='disabled'";
        }
        eleHtml = "<input class='form-control Wdate' type='text' " + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "' />";
        return eleHtml;
    }

    //日期时间类型.
    if (mapAttr.MyDataType == 7) { // AppDateTime = 7
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1) {
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
        } else {
            enableAttr = "disabled='disabled'";
        }
        eleHtml = "<input  class='form-control Wdate' type='text' " + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
        return eleHtml;
    }

    //checkbox 类型.
    if (mapAttr.MyDataType == 4) { // AppBoolean = 7


        eleHtml += "<div class='checkbox' ><label style='width:100%;' > <input " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox'  id='CB_" + mapAttr.KeyOfEn + "' name='CB_" + mapAttr.KeyOfEn + "'/>";
        eleHtml += mapAttr.Name + '</label></div>';
        return eleHtml;
    }

    //枚举下拉框.
    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
        if (mapAttr.UIContralType == 1) { //DDL
            //多选下拉框
            eleHtml += "<select style='padding:0px;'  class='form-control'  data-val='" + ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' id='DDL_" + mapAttr.KeyOfEn + "' name='DDL_" + mapAttr.KeyOfEn + "' >" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
        }
        return eleHtml;
    }

    // 浮点类型. AppDouble  AppFloat
    if (mapAttr.MyDataType == 5 || mapAttr.MyDataType == 3) {
        //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
        var defVal = mapAttr.DefVal;
        var bit;
        if (defVal != null && defVal !== "" && defVal.indexOf(".") >= 0)
            bit = defVal.substring(defVal.indexOf(".") + 1).length;

        eleHtml += "<input class='form-control' style='text-align:right;' onblur='valitationAfter(this, \"float\")' onkeydown='valitationBefore(this, \"float\")' onkeyup=" + '"' + "valitationAfter(this, 'float'); if(!(value.indexOf('-')==0&&value.length==1)&&isNaN(value));limitLength(this," + bit + ");" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'float'); if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
        return eleHtml;
    }

    // int 类型.
    if ((mapAttr.MyDataType == 2 && mapAttr.UIContralType == 0)) { //AppInt

        eleHtml += "<input class='form-control' style='text-align:right;' onblur='valitationAfter(this, \"int\")' onkeydown='valitationBefore(this, \"int\")' onkeyup=" + '"' + "valitationAfter(this, 'int'); if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'int'); if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' placeholder='" + (mapAttr.Tip || '') + "'/>";
        return eleHtml;
    }

    // 金额类型. AppMoney  AppRate
    if (mapAttr.MyDataType == 8) {
        //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
        var defVal = mapAttr.DefVal;
        var bit;
        if (defVal != null && defVal !== "" && defVal.indexOf(".") >= 0)
            bit = defVal.substring(defVal.indexOf(".") + 1).length;
        else
            bit = 2;

        eleHtml += "<input class='form-control ' style='text-align:right;'   onblur='valitationAfter(this, \"money\")' onkeydown='valitationBefore(this, \"money\")' onkeyup=" + '"' + "valitationAfter(this, 'money'); limitLength(this," + bit + ");FormatMoney(this, " + bit + ", ',');" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'money'); if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='0.00' placeholder='" + (mapAttr.Tip || '') + "'/>";
        return eleHtml;
    }

    alert(mapAttr.Name + '没有判断...');
}

//将#FF000000 转换成 #FF0000
function TranColorToHtmlColor(color) {
    if (color != undefined && color.indexOf('#') == 0 && color.length == 9) {
        color = color.substring(0, 7);
    }
    return color;
}

//FontStyle, FontWeight, IsBold, IsItalic
//fontStyle font-size:19;font-family:"Portable User Interface";font-weight:bolder;color:#FF0051; 为H5设计的，不用解析后面3个
function analysisFontStyle(ele, fontStyle, isBold, isItalic) {
    if (fontStyle != undefined && fontStyle.indexOf(':') > 0) {
        var fontStyleArr = fontStyle.split(';');
        $.each(fontStyleArr, function (i, fontStyleObj) {
            if (fontStyleObj.split(':')[0] == 'font-size')
                ele.css(fontStyleObj.split(':')[0], fontStyleObj.split(':')[1] + 'px');
            else if (fontStyleObj.split(':')[0] == 'color')
                ele.css(fontStyleObj.split(':')[0], TranColorToHtmlColor(fontStyleObj.split(':')[1]));
            else
                ele.css(fontStyleObj.split(':')[0], fontStyleObj.split(':')[1]);
        });
    }
    else {
        if (isBold == 1) {
            ele.css('font-weight', 'bold');
        }
        if (isItalic == 1) {
            ele.css('font-style', 'italic')
        }
    }
}

//升级表单元素 初始化Label
function figure_Template_Label(frmLab) {

    var eleHtml = "<label></label>";
    eleHtml = $(eleHtml);
    var text = frmLab.Text.replace(/@/g, "<br>");
    eleHtml.html(text);
    eleHtml.css('position', 'absolute').css('top', frmLab.Y + 'px').css('left', frmLab.X + 'px').css('font-size', frmLab.FontSize)
        .css('padding-top', '5px').css('color', TranColorToHtmlColor(frmLab.FontColor));
    // eleHtml.css('position', 'absolute').css('top', frmLab.Y).css('left', frmLab.X).css('padding-top', '5px').css('color', TranColorToHtmlColor(frmLab.FontColor));
    analysisFontStyle(eleHtml, frmLab.FontStyle, frmLab.isBold, frmLab.IsItalic);
    return eleHtml;
}

//升级表单元素 初始化Label
function figure_Template_Label_old(frmLab) {
    var eleHtml = '';
    eleHtml = '<label></label>'
    eleHtml = $(eleHtml);
    var text = frmLab.Text.replace(/@/g, "<br>");
    eleHtml.html(text);
    eleHtml.css('position', 'absolute').css('top', frmLab.Y).css('left', frmLab.X).css('font-size', frmLab.FontSize + 'px')
        .css('padding-top', '5px').css('color', TranColorToHtmlColor(frmLab.FontColor));
    analysisFontStyle(eleHtml, frmLab.FontStyle, frmLab.isBold, frmLab.IsItalic);
    return eleHtml;
}

//初始化按钮
function figure_Template_Btn(frmBtn) {
    var eleHtml = $('<div></div>');
    var btnId = frmBtn.BtnID;
    if (btnId == null || btnId == "")
        btnId = frmBtn.MyPK;
    var btnHtml = $("<input id='" + btnId + "' type='button' value='' >");
    btnHtml.val(frmBtn.Text).width(frmBtn.W).height(frmBtn.H).addClass('btn');
    var doc = frmBtn.EventContext;
    doc = doc.replace("~", "'");
    var eventType = frmBtn.EventType;
    if (eventType == 0) {//禁用
        btnHtml.attr('disabled', 'disabled').css('background', 'gray');
    } else if (eventType == 1) {//运行URL
        $.each(frmData.Sys_MapAttr, function (i, obj) {
            if (doc.indexOf('@' + obj.KeyOfEn) > 0) {
                //替换
                //url=  url.replace(new RegExp(/(：)/g), ':');
                //先这样吧
                doc = doc.replace('@' + obj.KeyOfEn, frmData.MainTable[0][obj.KeyOfEn]);
            }
        });
        var OID = GetQueryString("OID");
        if (OID == undefined || OID == "");
        OID = GetQueryString("OID");
        var FK_Node = GetQueryString("FK_Node");
        var FK_Flow = GetQueryString("FK_Flow");
        var webUser = new WebUser();
        var userNo = webUser.No;
        var SID = webUser.SID;
        if (SID == undefined)
            SID = "";
        if (doc.indexOf("?") == -1)
            doc = doc + "?1=1";
        doc = doc + "&OID=" + OID + "&FK_Node=" + FK_Node + "&FK_Flow=" + FK_Flow + "&UserNo=" + userNo + "&SID=" + SID;
        btnHtml.attr('onclick', "window.open('" + doc + "')");


    } else {//运行JS
        if (doc.indexOf("(") == -1)
            doc = doc + "()";
        btnHtml.attr('onclick', doc);
    }
    eleHtml.append(btnHtml);
    //别的一些属性先不加
    eleHtml.css('position', 'absolute').css('top', frmBtn.Y + 'px').css('left', frmBtn.X + 'px').width(frmBtn.W).height(frmBtn.H);
    return eleHtml;
}

//初始化单选按钮
function figure_Template_Rb(frmRb) {
    var eleHtml = '<div></div>';
    eleHtml = $(eleHtml);
    var childRbEle = $('<input id="RB_ChuLiFangShi2" type="radio"/>');
    var childLabEle = $('<label class="labRb"></label>');
    childLabEle.html(frmRb.Lab).attr('for', 'RB_' + frmRb.KeyOfEn + frmRb.IntKey).attr('name', 'RB_' + frmRb.KeyOfEn);

    childRbEle.val(frmRb.IntKey).attr('id', 'RB_' + frmRb.KeyOfEn + frmRb.IntKey).attr('name', 'RB_' + frmRb.KeyOfEn);
    //    if (frmRb.UIIsEnable == false)
    //        childRbEle.attr('disabled', 'disabled');

    var defVal = ConvertDefVal(frmData, '', frmRb.KeyOfEn);
    if (defVal == frmRb.IntKey) {
        childRbEle.attr("checked", "checked");
    }

    eleHtml.append(childRbEle).append(childLabEle);
    eleHtml.css('position', 'absolute').css('top', frmRb.Y + 'px').css('left', frmRb.X + 'px');
    return eleHtml;
}

//初始化超链接
function figure_Template_HyperLink(frmLin) {
    //URL @ 变量替换
    var url = frmLin.URL;
    $.each(frmData.Sys_MapAttr, function (i, obj) {
        if (url.indexOf('@' + obj.KeyOfEn) > 0) {
            //替换
            //url=  url.replace(new RegExp(/(：)/g), ':');
            //先这样吧
            url = url.replace('@' + obj.KeyOfEn, frmData.MainTable[0][obj.KeyOfEn]);
        }
    });

    var OID = GetQueryString("OID");
    if (OID == undefined || OID == "");
    OID = GetQueryString("OID");
    var FK_Node = GetQueryString("FK_Node");
    var FK_Flow = GetQueryString("FK_Flow");
    var webUser = new WebUser();
    var userNo = webUser.No;
    var SID = webUser.SID;
    if (SID == undefined)
        SID = "";
    if (url.indexOf("?") == -1)
        url = url + "?1=1";
    url = url + "&OID=" + OID + "&FK_Node=" + FK_Node + "&FK_Flow=" + FK_Flow + "&UserNo=" + userNo + "&SID=" + SID;

    var eleHtml = '<span></span>';
    eleHtml = $(eleHtml);

    var a = $("<a></a>");
    a.attr('href', url).attr('target', frmLin.Target).html(frmLin.Text);
    eleHtml.append(a);
    eleHtml.css('position', 'absolute')
        .css('top', frmLin.Y + 'px')
        .css('left', frmLin.X + 'px')
        .css('color', frmLin.FontColr)
        .css('fontsize', frmLin.FontSize)
        .css('font-family', frmLin.FontName);
    return eleHtml;
}


//初始化 IMAGE  只初始化了图片类型
function figure_Template_Image(frmImage) {
    var eleHtml = '';
    var imgSrc = "";
    if (frmImage.ImgAppType == 0) { //图片类型
        //数据来源为本地.
        var imgSrc = '';
        if (frmImage.ImgSrcType == 0) {
            //替换参数
            var frmPath = frmImage.ImgPath;
            frmPath = frmPath.replace('＠', '@');
            frmPath = frmPath.replace('@basePath', basePath);
            frmPath = frmPath.replace('@basePath', basePath);
            imgSrc = DealJsonExp(frmData.MainTable[0], frmPath);
        }

        //数据来源为指定路径.
        if (frmImage.ImgSrcType == 1) {
            var url = frmImage.ImgURL;
            url = url.replace('＠', '@');
            url = url.replace('@basePath', basePath);
            imgSrc = DealJsonExp(frmData.MainTable[0], url);
        }
        // 由于火狐 不支持onerror 所以 判断图片是否存在放到服务器端
        if (imgSrc == "" || imgSrc == null)
            imgSrc = "../../DataUser/ICON/CCFlow/LogBig.png";

        eleHtml = $('<div></div>');
        var a = $("<a></a>");
        var img = $("<img/>")
        img.attr("src", imgSrc).css('width', frmImage.W).css('height', frmImage.H).attr('onerror', "this.src='../../DataUser/ICON/CCFlow/LogBig.png'");
        if (frmImage.LinkURL != undefined && frmImage.LinkURL != '') {
            a.attr('href', frmImage.LinkTarget).attr('target', frmImage.LinkTarget).css('width', frmImage.W).css('height', frmImage.H);
            a.append(img);
            eleHtml.append(a);
        } else {
            eleHtml.append(img);
        }

        eleHtml.attr("id", frmImage.MyPK);
        eleHtml.css('position', 'absolute').css('top', frmImage.Y + 'px').css('left', frmImage.X + 'px').css('width', frmImage.W).css('height', frmImage.H); ;
    } else if (frmImage.ImgAppType == 3)//二维码  手机
    {


    } else if (frmImage.ImgAppType == 1) {//暂不解析
        //电子签章  写后台
    }
    return eleHtml;
}


//初始化 IMAGE 附件
function figure_Template_ImageAth(frmImageAth) {
    var isEdit = frmImageAth.IsEdit;
    var eleHtml = $("<div></div>");
    var img = $("<img class='pimg'/>");


    //判断权限
    var WhoIsPK = { OID: 0, FID: 1, PWorkID: 2, CWorkID: 3 };
    var fn = new Entity("BP.WF.Template.FrmNode");
    fn.SetPKVal(pageData.FK_MapData + "_" + pageData.FK_Node + "_" + pageData.FK_Flow);
    var count = fn.RetrieveFromDBSources();

    var refpkVal = pageData.OID;
    if (count != 0) {
        if (fn.WhoIsPK == WhoIsPK.FID || fn.WhoIsPK == WhoIsPK.PWorkID)
            refpkVal = pageData.FID
        var node = frmData.WF_Node[0];
        if ((node.FormType == 5 || node.FormType == 10) && fn.FrmSln == 2) {
            var imgAth = new Entity("BP.Sys.FrmUI.ExtImgAth");
            imgAth.SetPKVal(frmImageAth.MyPK + "_" + pageData.FK_Node);
            count = imgAth.RetrieveFromDBSources();
            if (count != 0)
                isEdit = ImgAth.IsEdit;

        }
    }
    var imgSrc = basePath + "/WF/Data/Img/LogH.PNG";
    //获取数据
    if (pageData.FK_MapData.indexOf("ND") != -1)
        imgSrc = basePath + "/DataUser/ImgAth/Data/" + frmImageAth.CtrlID + "_" + refpkVal + ".png";
    else
        imgSrc = basePath + "/DataUser/ImgAth/Data/" + pageData.FK_MapData + "_" + frmImageAth.CtrlID + "_" + refpkVal + ".png";
    //    if (frmData.Sys_FrmImgAthDB) {
    //        $.each(frmData.Sys_FrmImgAthDB, function (i, obj) {
    //            if (obj.MyPK == (frmImageAth.MyPK + '_' + pageData.WorkID)) {
    //                
    //            }
    //        });
    //    }
    //设计属性
    img.attr('id', 'Img' + frmImageAth.MyPK).attr('name', 'Img' + frmImageAth.MyPK);
    img.attr("src", imgSrc).attr('onerror', "this.src='" + basePath + "/WF/Admin/CCFormDesigner/Controls/DataView/AthImg.png'");
    img.css('width', frmImageAth.W).css('height', frmImageAth.H).css('padding', "0px").css('margin', "0px").css('border-width', "0px");
    //不可编辑
    if (isEdit == "1" && pageData.IsReadonly != "1") {
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
    eleHtml.css('position', 'absolute').css('top', frmImageAth.Y + 'px').css('left', frmImageAth.X + 'px');
    return eleHtml;
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

//初始化 附件
function figure_Template_Attachment(frmAttachment) {

    var eleHtml = '';
    var ath = frmAttachment;
    if (ath.UploadType == 0) { //单附件上传 L4204.
        return $('');
    }

    var src = "Ath.htm";
    var local = window.location.href;
    if (local.indexOf('CCBill') != -1) {
        src = '../CCForm/' + src;
    }

    if (pageData.IsReadonly == "1")
        src = src + "?PKVal=" + pageData.OID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1&FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow;
    else
        src = src + "?PKVal=" + pageData.OID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow;

    var fid = GetQueryString("FID");
    var pWorkID = GetQueryString("PWorkID");

    src += "&FID=" + fid;
    src += "&PWorkID=" + pWorkID;

    eleHtml += '<div>' + "<iframe style='width:" + ath.W + "px;height:" + ath.H + "px;' ID='Attach_" + ath.MyPK + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', ath.Y + 'px').css('left', ath.X + 'px').css('width', ath.W).css('height', ath.H);

    return eleHtml;
}

function connector_Template_Line(frmLine) {
    var eleHtml = '';
    eleHtml = '<table><tr><td></td></tr></table>';
    eleHtml = $(eleHtml).css('position', 'absolute').css('top', frmLine.Y1 + 'px').css('left', frmLine.X1 + 'px');
    eleHtml.find('td').css('padding', '0px')
    if (navigator.userAgent.indexOf('Firefox') >= 0) {
        eleHtml.find('td').css('padding', '0px')
        .css('width', Math.abs(frmLine.X1 - frmLine.X2) == 0 ? 1 : Math.abs(frmLine.X1 - frmLine.X2))
    .css('height', Math.abs(frmLine.Y1 - frmLine.Y2) == 0 ? 2 : Math.abs(frmLine.Y1 - frmLine.Y2))
        .css("background", frmLine.BorderColor);
    } else {
        eleHtml.find('td').css('padding', '0px')
        .css('width', Math.abs(frmLine.X1 - frmLine.X2) == 0 ? 0 : Math.abs(frmLine.X1 - frmLine.X2))
    .css('height', Math.abs(frmLine.Y1 - frmLine.Y2) == 0 ? 1 : Math.abs(frmLine.Y1 - frmLine.Y2))
        .css("background", frmLine.BorderColor);
    }

    return eleHtml;
}
