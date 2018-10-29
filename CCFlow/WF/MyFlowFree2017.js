var flowData = null;
var IsCC = false;
var isSigantureChecked = false;
function GenerFreeFrm(wn) {

    flowData = wn;

    $('#CCForm').html('');
    //循环MapAttr
    for (var mapAtrrIndex in flowData.Sys_MapAttr) {
        var mapAttr = flowData.Sys_MapAttr[mapAtrrIndex];
        var eleHtml = figure_MapAttr_Template(mapAttr);
        $('#CCForm').append(eleHtml);
    }

    //循环FrmLab
    for (var i in flowData.Sys_FrmLab) {
        var frmLab = flowData.Sys_FrmLab[i];
        var label = figure_Template_Label(frmLab);
        $('#CCForm').append(label);
    }
    //循环FrmRB
    for (var i in flowData.Sys_FrmRB) {
        var frmLab = flowData.Sys_FrmRB[i];
        var label = figure_Template_RB(frmLab);
        $('#CCForm').append(label);
    }

    //循环FrmBtn
    for (var i in flowData.Sys_FrmBtn) {
        var frmBtn = flowData.Sys_FrmBtn[i];
        var btn = figure_Template_Btn(frmBtn);
        $('#CCForm').append(btn);
    }

    //循环Image
    for (var i in flowData.Sys_FrmImg) {
        var frmImg = flowData.Sys_FrmImg[i];
        var createdFigure = figure_Template_Image(frmImg);
        $('#CCForm').append(createdFigure);
    }

    //循环 Link
    for (var i in flowData.Sys_FrmLink) {
        var frmLink = flowData.Sys_FrmLink[i];
        var createdFigure = figure_Template_HyperLink(frmLink);
        $('#CCForm').append(createdFigure);
    }
    //循环 图片附件
    if (flowData.Sys_FrmImgAth.length > 0) {
        var imgSrc = "<input type='hidden' id='imgSrc'/>";
        $('#CCForm').append(imgSrc);
    }
    for (var i in flowData.Sys_FrmImgAth) {
        var frmImgAth = flowData.Sys_FrmImgAth[i];
        var createdFigure = figure_Template_ImageAth(frmImgAth);
        $('#CCForm').append(createdFigure);
    }
    //循环 附件
    for (var i in flowData.Sys_FrmAttachment) {
        var frmAttachment = flowData.Sys_FrmAttachment[i];
        var createdFigure = figure_Template_Attachment(frmAttachment);
        $('#CCForm').append(createdFigure);
    }

    //循环 从表
    for (var i in flowData.Sys_MapDtl) {
        var frmMapDtl = flowData.Sys_MapDtl[i];
        var createdFigure = figure_Template_Dtl(frmMapDtl);
        $('#CCForm').append(createdFigure);
    }

    //循环线
    for (var i in flowData.Sys_FrmLine) {
        var frmLine = flowData.Sys_FrmLine[i];
        var createdConnector = connector_Template_Line(frmLine);
        $('#CCForm').append(createdConnector);
    }

    //循环组件 轨迹图 审核组件 子流程 子线程
    var wf_FrmNodeComponent = flowData["WF_FrmNodeComponent"][0];
    $('#CCForm').append(figure_Template_FigureFlowChart(wf_FrmNodeComponent));
    $('#CCForm').append(figure_Template_FigureFrmCheck(wf_FrmNodeComponent));
    $('#CCForm').append(figure_Template_FigureSubFlowDtl(wf_FrmNodeComponent));
    $('#CCForm').append(figure_Template_FigureThreadDtl(wf_FrmNodeComponent));
}

function figure_MapAttr_Template(mapAttr) {


    //根据不同的类型控件，生成html.
    var ele = figure_MapAttr_TemplateEle(mapAttr);

    ele += mapAttr.UIIsInput == 1 ? '<span style="color:red" class="mustInput" data-keyofen="' + mapAttr.KeyOfEn + '" >*</span>' : "";

    var eleHtml = $('<div>' + ele + '</div>');
    var W = mapAttr.UIWidth;
    if (mapAttr.MyDataType == 6)
        if (W < 120) W = 120;
    if (mapAttr.MyDataType == 7)
        if (W < 160) W = 160;

    if (mapAttr.MyDataType != 4) {
        eleHtml.children(0).css('width', W).css('height', mapAttr.UIHeight).css("padding", "0px 12px");  
     }
    
    eleHtml.css('position', 'absolute').css('top', mapAttr.Y).css('left', mapAttr.X);

    return eleHtml;
}

//升级表单元素 初始化文本框、日期、时间
function figure_MapAttr_TemplateEle(mapAttr) {

    var eleHtml = '';

    /***************** 隐藏的控件 (在 CCForm/FrmEnd.js 处理.) *****************************/
    if (mapAttr.UIVisible == 0) {
        return "";
    }

    /***************** 外键 *****************************/
    if (mapAttr.LGType == 2 && mapAttr.MyDataType == "1" && mapAttr.UIContralType == "1") {
        eleHtml = "<select  style='padding:0px;' id='DDL_" + mapAttr.KeyOfEn + "' class='form-control'  >" + InitDDLOperation(flowData, mapAttr) + "</select>";
        return eleHtml;
    }

    /***************** 外部数据源 *****************************/
    if (mapAttr.LGType == 0 && mapAttr.MyDataType == "1" && mapAttr.UIContralType == "1") {
        eleHtml = "<select  style='padding:0px;'  id='DDL_" + mapAttr.KeyOfEn + "' class='form-control' >" + InitDDLOperation(flowData, mapAttr, "") + "</select>";
        return eleHtml;
    }

    /***************** 作为附件展示的控件. *****************************/
    if (mapAttr.UIContralType == 6) {
        var atParamObj = AtParaToJson(mapAttr.AtPara);
        if (atParamObj.AthRefObj != undefined) { //扩展设置为附件展示
            eleHtml += "<input type='hidden' class='tbAth' class='form-control' data-target='" + mapAttr.AtPara + "' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' >" + "</input>";
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
    var isInOneRow = false; //是否占一整行
    var islabelIsInEle = false; //
    eleHtml += '';

    //添加文本框 ，日期控件等
    //AppString
    if (mapAttr.MyDataType == "1") {

        //普通类型的单行文本.
        if (mapAttr.UIHeight <= 40) {

            //如果是图片签名，并且可以编辑
            if (mapAttr.IsSigan == "1" && mapAttr.UIIsEnable == 1) {
                //查找默认值
                var val = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' type=hidden />";
                //是否签过
                var sealData = new Entities("BP.Tools.WFSealDatas");
                sealData.Retrieve("OID", GetQueryString("WorkID"), "FK_Node", GetQueryString("FK_Node"), "SealData", GetQueryString("UserNo"));

                if (sealData.length > 0) {
                    eleHtml += "<img src='../DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\"  style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                    isSigantureChecked = true;
                }
                else {
                    eleHtml += "<img src='../DataUser/Siganture/siganture.jpg' onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\" ondblclick='figure_Template_Siganture(\"" + mapAttr.KeyOfEn + "\",\"" + val + "\")' style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                } 
                return eleHtml;
            }
            //如果不可编辑，并且是图片名称
            if (mapAttr.IsSigan == "1") {
                var val = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' type=hidden />";
                eleHtml += "<img src='../DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='../DataUser/Siganture/siganture.jpg'\" style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                return eleHtml;
            }


            eleHtml += "<input class='form-control' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' type='text' placeholder='" + (mapAttr.Tip || '') + "' />";

            return eleHtml;
        }

        //判断是否是富文本?
        if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {

            //如果是富文本就使用百度 UEditor

            if (mapAttr.UIIsEnable == "0") {
                //只读状态直接 div 展示富文本内容
                //eleHtml += "<script id='" + editorPara.id + "' name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";
                var defValue = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
                eleHtml = "<div class='richText' style='style='width:99%;'>" + defValue + "</div>";
                return eleHtml;
            }

            document.BindEditorMapAttr = mapAttr; //存到全局备用

            //设置编辑器的默认样式.
            var styleText = "text-align:left;font-size:12px;";
            styleText += "width:100%;";
            styleText += "height:" + mapAttr.UIHeight + "px;";
            //注意这里 name 属性是可以用来绑定表单提交时的字段名字的 editor是特殊约定的.
            eleHtml += "<script id='editor' name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'></script>";
            // eleHtml += "<script id='editor' name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";

            return eleHtml;
        }

        //多行文本模式.
        eleHtml = "<textarea maxlength=" + mapAttr.MaxLen + " style='height:" + mapAttr.UIHeight + "px;' id='TB_" + mapAttr.KeyOfEn + "' type='text' />";
        return eleHtml;
    }

    //日期类型.
    if (mapAttr.MyDataType == 6) { //AppDate
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1) {
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
        }

        eleHtml = "<input  type='text' class='form-control Wdate' " + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' />";
        return eleHtml;
    }

    //日期时间类型.
    if (mapAttr.MyDataType == 7) { // AppDateTime = 7
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1) {
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
        }
        eleHtml = "<input type='text' class='form-control Wdate'  " + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' />";
        return eleHtml;
    }

    //checkbox 类型.
    if (mapAttr.MyDataType == 4) { // AppBoolean = 7

        //CHECKBOX 默认值
        var checkedStr = '';
        if (checkedStr != "true" && checkedStr != '1') {
            checkedStr = ' checked="checked" '
        }
        checkedStr = ConvertDefVal(flowData, '', mapAttr.KeyOfEn);
        eleHtml += "<div class='checkbox' ><label><input " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox'  id='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + "/>";
        eleHtml += mapAttr.Name + '</label></div>';
        return eleHtml;
    }

    //枚举下拉框.
    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
        if (mapAttr.UIContralType == 1) { //DDL
            //多选下拉框.
            eleHtml += "<select  style='padding:0px;' id='DDL_" + mapAttr.KeyOfEn + "' class='form-control'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(flowData, mapAttr, "") + "</select>";
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

        eleHtml += "<input style='text-align:right;' class='form-control' onkeyup=" + '"' + "if(!(value.indexOf('-')==0&&value.length==1)&&isNaN(value))execCommand('undo');limitLength(this," + bit + ");" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' value='0.00'/>";
        return eleHtml;
    }

    // int 类型.
    if ((mapAttr.MyDataType == 2 && mapAttr.UIContralType == 0)) { //AppInt
        eleHtml += "<input style='text-align:right;' class='form-control' onkeyup=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' />";
        return eleHtml;
    }

    // 金额类型. AppMoney  AppRate
    if (mapAttr.MyDataType == 8) {

        eleHtml += "<input style='text-align:right;' class='form-control' onkeyup=" + '"' + "if(!(value.indexOf('-')==0&&value.length==1)&&isNaN(value))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' value='0.00'/>";
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
            ele.css(fontStyleObj.split(':')[0], TranColorToHtmlColor(fontStyleObj.split(':')[1]));
        });
    }
    else {

        if (isBold == 1) {
            ele.css('font-weight', 'bold');
        }

        if (isItalic == 1) {
            ele.css('font-style', 'italic');
        }
    }
}

//升级表单元素 初始化Label
function figure_Template_Label(frmLab) {
    var eleHtml = '';
    eleHtml = "<label id='" + frmLab.MyPK + "' ></label>";

    eleHtml = $(eleHtml);
    var text = frmLab.Text == null ? "" : frmLab.Text.replace(/@/g, "<br>");
    eleHtml.html(text);
    eleHtml.css('position', 'absolute').css('top', frmLab.Y -5).css('left', frmLab.X).css('font-size', frmLab.FontSize)
        .css('padding-top', '5px').css('color', TranColorToHtmlColor(frmLab.FontColr));
    analysisFontStyle(eleHtml, frmLab.FontStyle, frmLab.isBold, frmLab.IsItalic);
    return eleHtml;
}

//初始化按钮
function figure_Template_Btn(frmBtn) {
    var eleHtml = $('<div></div>');
    var btnHtml = $('<input type="button" value="">');
    btnHtml.val(frmBtn.Text).width(frmBtn.W).height(frmBtn.H).addClass('btn');
    var doc = frmBtn.EventContext;
    doc = (doc == null ? "" : doc.replace(/~/g, "'"));
    var eventType = frmBtn.EventType;
    if (eventType == 0 || pageData.IsReadOnly=="1") {//禁用
        btnHtml.attr('disabled', 'disabled').css('background', 'gray');
    } else if (eventType == 1) {//运行Exe文件. 运行JS
        $.each(flowData.Sys_MapAttr, function (i, obj) {
            if (doc != null && doc.indexOf('@' + obj.KeyOfEn) > 0) {
                //替换
                //url=  url.replace(new RegExp(/(：)/g), ':');
                //先这样吧
                doc = doc.replace('@' + obj.KeyOfEn, flowData.MainTable[0][obj.KeyOfEn]);
            }
        });

        var OID = GetQueryString("OID");
        if (OID == undefined || OID == "");
        OID = GetQueryString("WorkID");
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
    } else {
        if (doc.indexOf("(") == -1)
            doc = doc + "()";
        btnHtml.attr('onclick', doc);
    }
    eleHtml.append(btnHtml);
    //别的一些属性先不加
    eleHtml.css('position', 'absolute').css('top', frmBtn.Y).css('left', frmBtn.X).width(frmBtn.W).height(frmBtn.H);
    return eleHtml;
}

//初始化单选按钮
function figure_Template_RB(frmRb) {
    var MyPK = frmRb.FK_MapData + "_" + frmRb.KeyOfEn;
    var mapAttr = new Entity("BP.Sys.MapAttr", MyPK);

    var eleHtml = '<div></div>';
    eleHtml = $(eleHtml);
    var childRbEle = $('<input id="RB_ChuLiFangShi2" type="radio" />');
    var childLabEle = $('<label class="labRb"></label>');
    childLabEle.html(frmRb.Lab).attr('for', 'RB_' + frmRb.KeyOfEn + "_" + frmRb.IntKey).attr('name', 'RB_' + frmRb.KeyOfEn);

    childRbEle.val(frmRb.IntKey).attr('id', 'RB_' + frmRb.KeyOfEn + "_" + frmRb.IntKey).attr('name', 'RB_' + frmRb.KeyOfEn);
    if (mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0)
        childRbEle.attr("onclick", "clickEnable( this ,\"" + frmRb.FK_MapData + "\",\"" + frmRb.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")");

    if (frmRb.UIIsEnable == false)
        childRbEle.attr('disabled', 'disabled');

    var defVal = ConvertDefVal(flowData, '', frmRb.KeyOfEn);
    if (defVal == frmRb.IntKey) {
        childRbEle.attr("checked", "checked");
    }

    eleHtml.append(childRbEle).append(childLabEle);
    eleHtml.css('position', 'absolute').css('top', frmRb.Y).css('left', frmRb.X);
    return eleHtml;
}

//初始化超链接
function figure_Template_HyperLink(frmLin) {
    //URL @ 变量替换
    var url = frmLin.URL;
    $.each(flowData.Sys_MapAttr, function (i, obj) {
        if (url != null && url.indexOf('@' + obj.KeyOfEn) > 0) {
            //替换
            //url=  url.replace(new RegExp(/(：)/g), ':');
            //先这样吧
            url = url.replace('@' + obj.KeyOfEn, flowData.MainTable[0][obj.KeyOfEn]);
        }
    });

    var OID = GetQueryString("OID");
    if (OID == undefined || OID == "");
    OID = GetQueryString("WorkID");
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
        .css('top', frmLin.Y)
        .css('left', frmLin.X)
        .css('color', frmLin.FontColr)
        .css('fontsize', frmLin.FontSize)
        .css('font-family', frmLin.FontName);
    return eleHtml;
}


//初始化 IMAGE  只初始化了图片类型
function figure_Template_Image(frmImage) {
    var eleHtml = '';
    if (frmImage.ImgAppType == 0) {//图片类型
        //数据来源为本地.
        var imgSrc = '';
        if (frmImage.ImgSrcType == 0) {
            if (frmImage.ImgPath && frmImage.ImgPath.indexOf(";") < 0)
                imgSrc = frmImage.ImgPath;
            else
                imgSrc = frmImage.ImgPath;

        }
        //数据来源为指定路径.
        if (frmImage.ImgSrcType == 1) {
            //图片路径不为默认值
            if (frmImage.ImgURL && frmImage.ImgURL.indexOf(";") < 0)
                imgSrc = frmImage.ImgURL;
            else
                imgSrc = frmImage.ImgURL;

        }
        // 由于火狐 不支持onerror 所以 判断图片是否存在放到服务器端
        if (imgSrc == "" || imgSrc == null)  //|| !File.Exists(Server.MapPath("~/" + imgSrc)))  //
            imgSrc = "../DataUser/ICON/CCFlow/LogBig.png";

        eleHtml = $('<div></div>');
        var a = $("<a></a>");
        var img = $("<img/>")
        // img.attr("src", imgSrc).css('width', frmImage.W).css('height', frmImage.H);
        img.attr("src", imgSrc).css('width', frmImage.W).css('height', frmImage.H).attr('onerror', "this.src='../DataUser/ICON/CCFlow/LogBig.png'");

        if (frmImage.LinkURL != undefined && frmImage.LinkURL != '') {
            a.attr('href', frmImage.LinkTarget).attr('target', frmImage.LinkTarget).css('width', frmImage.W).css('height', frmImage.H);
            a.append(img);
            eleHtml.append(a);
        } else {
            eleHtml.append(img);
        }

        eleHtml.attr("id", frmImage.MyPK);
        eleHtml.css('position', 'absolute').css('top', frmImage.Y).css('left', frmImage.X).css('width', frmImage.W).css('height', frmImage.H); ;
    } else if (frmImage.ImgAppType == 3)//二维码  手机
    {


    } else if (frmImage.ImgAppType == 1) {//暂不解析
        //电子签章  写后台
    }
    return eleHtml;
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
        if(imgSrc!=null && imgSrc!="")
            document.getElementById('Img' + athMyPK).setAttribute('src', imgSrc);
    });
}

//初始化 IMAGE附件
function figure_Template_ImageAth(frmImageAth) {
    var isEdit = frmImageAth.IsEdit;
    var eleHtml = $("<div></div>");
    var img = $("<img/>");

    var imgSrc = basePath + "/WF/Data/Img/LogH.PNG";
    //获取数据
    if (flowData.Sys_FrmImgAthDB) {
        $.each(flowData.Sys_FrmImgAthDB, function (i, obj) {
            if (obj.MyPK == (frmImageAth.MyPK + '_' + pageData.WorkID)) {
                imgSrc = basePath + obj.FileFullName;
            }
        });
    }
    //设计属性
    img.attr('id', 'Img' + frmImageAth.MyPK).attr('name', 'Img' + frmImageAth.MyPK);
    img.attr("src", imgSrc).attr('onerror', "this.src='" + basePath + "/WF/Admin/CCFormDesigner/Controls/DataView/AthImg.png'");
    img.css('width', frmImageAth.W).css('height', frmImageAth.H).css('padding', "0px").css('margin', "0px").css('border-width', "0px");
    //不可编辑
    if (isEdit == "1" && pageData.IsReadonly!="1") {
        var fieldSet = $("<fieldset></fieldset>");
        var length = $("<legend></legend>");
        var a = $("<a></a>");
        var url = basePath + "/WF/CCForm/ImgAth.htm?W=" + frmImageAth.W + "&H=" + frmImageAth.H + "&FK_MapData=" + flowData.Sys_MapData[0].No + "&MyPK=" + pageData.WorkID + "&ImgAth=" + frmImageAth.MyPK;

        a.attr('href', "javascript:ImgAth('" + url + "','" + frmImageAth.MyPK + "');").html("编辑");
        length.css('font-style', 'inherit').css('font-weight', 'bold').css('font-size', '12px');

        fieldSet.append(length);
        length.append(a);
        fieldSet.append(img);
        eleHtml.append(fieldSet);
    } else {
        eleHtml.append(img);
    }
    eleHtml.css('position', 'absolute').css('top', frmImageAth.Y).css('left', frmImageAth.X);
    return eleHtml;
}

//初始化 附件
function figure_Template_Attachment(frmAttachment) {
    var eleHtml = '';
    var ath = frmAttachment;
    if (ath.UploadType == 0) { //单附件上传 L4204
        return $('');
    }
    var src = "";
    if (pageData.IsReadonly)
        src = "./CCForm/Ath.htm?PKVal=" + pageData.WorkID + "&FID=" + pageData["FID"] + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1&FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow;
    else
        src = "./CCForm/Ath.htm?PKVal=" + pageData.WorkID + "&FID=" + pageData["FID"] + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow;

    eleHtml += '<div>' + "<iframe style='width:" + ath.W + "px;height:" + ath.H + "px;' ID='Attach_" + ath.MyPK + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', ath.Y).css('left', ath.X).css('width', ath.W).css('height', ath.H);

    return eleHtml;
}

function connector_Template_Line(frmLine) {
    var eleHtml = '';
    eleHtml = '<table><tr><td></td></tr></table>';
    eleHtml = $(eleHtml).css('position', 'absolute').css('top', frmLine.Y1).css('left', frmLine.X1);
    if (navigator.userAgent.indexOf('Firefox') >= 0) {
        eleHtml.find('td').css('padding', '0px')
        .css('width', Math.abs(frmLine.X1 - frmLine.X2) == 0 ? 1 : Math.abs(frmLine.X1 - frmLine.X2))
    .css('height', Math.abs(frmLine.Y1 - frmLine.Y2) == 0 ? 2: Math.abs(frmLine.Y1 - frmLine.Y2))
        .css("background", frmLine.BorderColor);
    } else {
        eleHtml.find('td').css('padding', '0px')
        .css('width', Math.abs(frmLine.X1 - frmLine.X2) == 0 ? 0 : Math.abs(frmLine.X1 - frmLine.X2))
    .css('height', Math.abs(frmLine.Y1 - frmLine.Y2) == 0 ? 1 : Math.abs(frmLine.Y1 - frmLine.Y2))
        .css("background", frmLine.BorderColor);
    }

    return eleHtml;
}

//初始化从表
function figure_Template_Dtl(frmDtl) {
    var eleHtml = $("<DIV id='Fd" + frmDtl.No + "' style='position:absolute; left:" + frmDtl.X + "px; top:" + frmDtl.Y + "px; width:" + frmDtl.W + "px; height:" + frmDtl.H + "px;text-align: left;' >");
    var paras = this.pageData;
    var strs = "";
    for (var str in paras) {
        if (str == "EnsName" || str == "RefPKVal" || str == "IsReadonly")
            continue
        else
            strs += "&" + str + "=" + paras[str];
    }
    var src = "";
    var href = window.location.href;
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');
    if (frmDtl.ListShowModel == "0") {
        //表格模式
        if (pageData.IsReadonly) {
            src = "./CCForm/Dtl.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=1&" + urlParam + "&Version=1";
        } else {
            src = "./CCForm/Dtl.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=0&" + urlParam + "&Version=1";
        }
    } else if (frmDtl.ListShowModel == "1") {
        //卡片模式
        if (pageData.IsReadonly) {
            src = "./CCForm/DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=1&" + urlParam + "&Version=1";
        } else {
            src = "./CCForm/DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=0&" + urlParam + "&Version=1";
        }
    }

    var eleIframe = '<iframe></iframe>';
    eleIframe = $("<iframe class='Fdtl' ID='F" + frmDtl.No + "' src='" + src +
                 "' frameborder=0  style='position:absolute;width:" + frmDtl.W + "px; height:" + frmDtl.H +
                 "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
    if (pageData.IsReadonly) {

    } else {
        if (frmDtl.DtlSaveModel == 0) {
            eleHtml.append(addLoadFunction(frmDtl.No, "blur", "SaveDtl"));
            eleIframe.attr('onload', frmDtl.No + "load()");
        }
    }
    eleHtml.append(eleIframe);
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

//初始化轨迹图
function figure_Template_FigureFlowChart(wf_node) {

    //轨迹图
    var sta = wf_node.FrmTrackSta;
    var x = wf_node.FrmTrack_X;
    var y = wf_node.FrmTrack_Y;
    var h = wf_node.FrmTrack_H;
    var w = wf_node.FrmTrack_W;

    if (sta == 0) {
        return $('');
    }

    if (sta == undefined) {
        return;
    }

    var src = "./WorkOpt/OneWork/OneWork.htm?CurrTab=Track";
    src += '&FK_Flow=' + pageData.FK_Flow;
    src += '&FK_Node=' + pageData.FK_Node;
    src += '&WorkID=' + pageData.WorkID;
    src += '&FID=' + pageData.FID;
    var eleHtml = '<div id="divtrack' + wf_node.NodeID + '">' + "<iframe id='track" + wf_node.NodeID + "' style='width:" + w + "px;height=" + h + "px;'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);

    return eleHtml;
}

//审核组件
function figure_Template_FigureFrmCheck(wf_node) {

    //审核组键FWCSta Sta,FWC_X X,FWC_Y Y,FWC_H H, FWC_W W from WF_Node
    var sta = wf_node.FWCSta;
    var x = wf_node.FWC_X;

    if (x <= 20)
        x = 200;

    var y = wf_node.FWC_Y;
    if (y <= 20)
        y = 500;

    var h = wf_node.FWC_H;
    var w = wf_node.FWC_W;
    if (sta == 0)
        return $('');

    var src = "./WorkOpt/WorkCheck.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["WorkID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.WorkID;
    if (sta == 2)//只读
    {
        src += "&DoType=View";
    }
    else {
        fwcOnload = "onload= 'WC" + wf_node.NodeID + "load();'";
        $('body').append(addLoadFunction("WC" + wf_node.NodeID, "blur", "SaveDtl"));
    }
    src += "&r=q" + paras;

    var eleHtml = '<div >' + "<iframe style='width:100%' height=" + h + "' id='FWC' src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto ></iframe>" + '</div>';

    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);
    return eleHtml;
}

//子线程
function figure_Template_FigureThreadDtl(wf_node) {

    //FrmThreadSta Sta,FrmThread_X X,FrmThread_Y Y,FrmThread_H H,FrmThread_W
    var sta = wf_node.FrmThreadSta;
    var x = wf_node.FrmThread_X;
    var y = wf_node.FrmThread_Y;
    var h = wf_node.FrmThread_H;
    var w = wf_node.FrmThread_W;
    if (sta == 0 || sta == '0')
        return $('');

    var src = "./WorkOpt/Thread.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["WorkID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.WorkID;

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
function figure_Template_FigureSubFlowDtl(wf_node) {
    //SFSta Sta,SF_X X,SF_Y Y,SF_H H, SF_W W
    var sta = wf_node.SFSta;
    var x = wf_node.SF_X;
    var y = wf_node.SF_Y;
    var h = wf_node.SF_H;
    var w = wf_node.SF_W;
    if (sta == 0)
        return $('');

    var src = "./WorkOpt/SubFlow.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["WorkID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.WorkID;
    if (sta == 2)//只读
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

//初始化框架
function figure_Template_IFrame(fram) {
    var eleHtml = '';
    var src = dealWithUrl(fram.src) + "IsReadonly=0";
    eleHtml = $('<div id="iframe' + fram.MyPK + '">' + '</div>');
    var iframe = $(+"<iframe  style='width:" + fram.W + "px; height:" + fram.H + "'     src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling='no'></iframe>");

    eleHtml.css('position', 'absolute').css('top', fram.Y).css('left', fram.X).css('width', fram.W).css('height', fram.H);
    return frameHtml;
}
