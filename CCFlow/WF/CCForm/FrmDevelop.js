/**
 * 开发者表单的解析
 * @param {any} mapData 表单属性
 * @param {any} fk_mapData 表单数据
 */
var currentURL = window.document.location.href;
var frmData;
function GenerDevelopFrm(wn,fk_mapData) {
    frmData = wn;
    $('head').append('<style type="text/css"> .form-control{display: inline; }</style >');

    var htmlContent = "";
    //数据库中查找
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_DevelopDesigner");
    handler.AddPara("FK_MapData", fk_mapData);
    htmlContent = handler.DoMethodReturnString("Designer_Init");
    
    if (htmlContent == "") {
        alert("开发者设计的表单内容丢失，请联系管理员");
        return;
    }
    if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 ||currentURL.indexOf("MyDict.htm") != -1)
        htmlContent = htmlContent.replace(new RegExp("../../../", 'gm'), "../../");
    else
        htmlContent = htmlContent.replace(new RegExp("../../../", 'gm'), "../");
    $("#CCForm").html(htmlContent);

    //解析表单中的数据
   
    //1.加载隐藏字段，
    var mapAttrs = frmData.Sys_MapAttr;
    var html = "";
    for (var i = 0; i < mapAttrs.length; i++) {
        var mapAttr = mapAttrs[i];
        if (mapAttr.UIVisible == 0 ) {
            $("#TB_" + mapAttr.KeyOfEn).hide();
            $("#DDL_" + mapAttr.KeyOfEn).hide();
            $("input[name=CB_" + mapAttr.KeyOfEn+"]").hide();
            $("input[name=RB_" + mapAttr.KeyOfEn+"]").hide();
            continue;
        }
            
        //设置字段的样式属性
        $('#TB_' + mapAttr.KeyOfEn).addClass(mapAttr.CSS);
        $('#RB_' + mapAttr.KeyOfEn).addClass(mapAttr.CSS);
        $('#DDL_' + mapAttr.KeyOfEn).addClass(mapAttr.CSS);
        $('#CB_' + mapAttr.KeyOfEn).addClass(mapAttr.CSS);
        if (mapAttr.UIContralType == 14 || mapAttr.UIContralType == 15 || mapAttr.UIContralType == 17)
            $('#TB_' + mapAttr.KeyOfEn).removeAttr("placeholder");

        //如果是时间控件
        if (mapAttr.MyDataType == 6 && (mapAttr.UIIsEnable != 0 && pageData.IsReadonly != "1")) {
            var frmDate = mapAttr.IsSupperText;
            var dateFmt = '';
            if (frmDate == 0) {
                dateFmt = "yyyy-MM-dd";
            } else if (frmDate == 3) {
                dateFmt = "yyyy-MM";
            } else if (frmDate == 6) {
                dateFmt = "MM-dd";
            }
            $('#TB_' + mapAttr.KeyOfEn).attr("onfocus", "WdatePicker({ dateFmt:'" + dateFmt + "' })");
            continue;

        }
        if (mapAttr.MyDataType == 7 && (mapAttr.UIIsEnable != 0 && pageData.IsReadonly != "1")) {
            var frmDate = mapAttr.IsSupperText;
            var dateFmt = '';
            if (frmDate == 1) {
                dateFmt = "yyyy-MM-dd HH:mm";
            } else if (frmDate == 2) {
                dateFmt = "yyyy-MM-dd HH:mm:ss";
            } else if (frmDate == 4) {
                dateFmt = "HH:mm";
            } else if (frmDate == 5) {
                dateFmt = "HH:mm:ss";
            } 
            
            $('#TB_' + mapAttr.KeyOfEn).attr("onfocus", "WdatePicker({ dateFmt:'" + dateFmt + "' })");
           
            continue;
        }

        //外部数据源、外键的选择列表
        if ((mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1)
            || (mapAttr.LGType == "2" && mapAttr.MyDataType == "1")) {
            var _html = InitDDLOperation(frmData, mapAttr, null);
            $("#DDL_" + mapAttr.KeyOfEn).empty();
            $("#DDL_" + mapAttr.KeyOfEn).append(_html);
            continue;
        }
      
        if (mapAttr.MyDataType == "1" && mapAttr.IsSigan == "1") {
            //隐藏该字段值
            $("#TB_" + mapAttr.KeyOfEn).hide();
            var html = "";
            var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
            var localHref = GetLocalWFPreHref();
            if (mapAttr.UIIsEnable == 1 && pageData.IsReadonly != 0) {
                //是否签过
                var sealData = new Entities("BP.Tools.WFSealDatas");
                sealData.Retrieve("OID", pageData.WorkID, "FK_Node", GetQueryString("FK_Node"), "SealData", GetQueryString("UserNo"));
                if (sealData.length > 0) {
                    html = "<img src='" + localHref + "/DataUser/Siganture/" + defValue + ".jpg' onerror=\"this.src='" + localHref+"/DataUser/Siganture/UnName.jpg'\"  style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />";
                    isSigantureChecked = true;
                }
                else {
                    html = "<img src='" + localHref + "/DataUser/Siganture/siganture.jpg' onerror=\"this.src='" + localHref + "/DataUser/Siganture/UnName.jpg'\" ondblclick='figure_Template_Siganture(\"" + mapAttr.KeyOfEn + "\",\"" + val + "\")' style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />";
                }
               

            } else {
              

                html = "<img src='" + localHref + "/DataUser/Siganture/" + val + ".jpg' onerror=\"this.src='" + localHref+"/DataUser/Siganture/siganture.jpg'\" style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />";
            }
            
            $("#TB_" + mapAttr.KeyOfEn).after(html);
            
        }

        //为复选框高级设置绑定事件
        if (mapAttr.MyDataType == 4 && mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
            $("#CB_" + mapAttr.KeyOfEn).attr("onchange", "clickEnable(this, \"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\",8)");
            continue;
        }
          //为单选按钮高级设置绑定事件
        if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {
            if (mapAttr.AtPara && mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
                if (mapAttr.UIContralType == 1)//枚举下拉框
                    $("#CB_" + mapAttr.KeyOfEn).attr("onchange", "changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\",8)");
                if (mapAttr.UIContralType == 3) { //枚举单选
                    var inputs =$("input[name='RB_"+mapAttr.KeyOfEn+"']");
                    $.each(inputs, function (i, target) {
                        $(target).attr("onchange","clickEnable( this ,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\",8)");
                    });
                   
                }
            }
            continue;
        } 

        if (mapAttr.MyDataType == 1) {
            if (mapAttr.UIContralType == 8)//手写签字版
            {
                var element = $("#Img" + mapAttr.KeyOfEn);
                var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
                var ondblclick = ""
                if (mapAttr.UIIsEnable == 1) {
                    ondblclick = " ondblclick='figure_Develop_HandWrite(\"" + mapAttr.KeyOfEn + "\",\"" + defValue + "\")'";
                }

                var html = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "'  value='" + defValue + "' type=hidden />";
                var eleHtml = "";
                if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
                    eleHtml = "<img src='" + defValue + "' " + ondblclick + " onerror=\"this.src='../../DataUser/Siganture/UnName.jpg'\"  style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                else
                    eleHtml = "<img src='" + defValue + "' " + ondblclick + " onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\"  style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />" + html;
                
                element.after(eleHtml);
                element.remove(); //移除Imge节点
                continue;
            }
            if (mapAttr.UIContralType == 4)//地图
            {
                var obj = $("#TB_" + mapAttr.KeyOfEn);
                //获取兄弟节点
                $(obj.prev()).attr("onclick", "figure_Template_Map('" + mapAttr.KeyOfEn + "','" + mapAttr.UIIsEnable + "')");
                continue;
            }
            if (mapAttr.UIContralType == 101)//评分
            {
                var scores = $(".simplestar");//获取评分的类
                $.each(scores, function (score, idx) {
                    $.each($(this).children("Img"), function () {
                        if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
                            $(this).attr("src", $(this).attr("src").replace("../../", "../"));
                       else
                            $(this).attr("src", $(this).attr("src").replace("../../", "./"));
                    });
                });
                continue;
            }
        }
    }


   

    //2.解析控件 从表、附件、附件图片、框架、地图、签字版、父子流程
    var frmDtls = frmData.Sys_MapDtl;
    for (var i = 0; i < frmDtls.length; i++) {
        var frmDtl = frmDtls[i];
        //根据data-key获取从表元素
        var element = $("Img[data-key=" + frmDtl.No + "]");
        if (element.length == 0)
            continue;
        figure_Develop_Dtl(element, frmDtl);

    }
    var aths = frmData.Sys_FrmAttachment;//附件
    for (var i = 0; i < aths.length; i++) {
        var ath = aths[i];
        //根据data-key获取从表元素
        var element = $("Img[data-key=" + ath.MyPK + "]");
        if (element.length == 0)
            continue;
        figure_Develop_Ath(element, ath);

    }

    //图片附件
    var athImgs = frmData.Sys_FrmImgAth;
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
        figure_Develop_ImageAth(element, athImg,fk_mapData);

    }

    //图片
    var imgs = frmData.Sys_FrmImg;
    for (var i = 0; i < imgs.length; i++) {
        var img = imgs[i];
        //根据data-key获取从表元素
        var element = $("Img[data-key=" + img.MyPK + "]");
        if (element.length == 0)
            continue;
        figure_Develop_Image(element, img);

    }
    var iframes = frmData.Sys_MapFrame;//框架
    for (var i = 0; i < iframes.length; i++) {
        var iframe = iframes[i];
        //根据data-key获取从表元素
        var element = $("Img[data-key=" + iframe.MyPK + "]");
        if (element.length == 0)
            continue;
        figure_Develop_IFrame(element, iframe);

    }
    if (frmData.WF_FrmNodeComponent != null && frmData.WF_FrmNodeComponent != undefined) {
        var nodeComponents = frmData.WF_FrmNodeComponent[0];//节点组件
        if (nodeComponents != null) {
            var elements = $("Img[data-key=" + nodeComponents.NodeID + "]");
            $.each(elements, function (i, element) {
                //父子流程
                if (element.getAttribute("data-type") == "SubFlow")
                    figure_Develop_FigureSubFlowDtl(nodeComponents, element);
                //如果有审核组件，增加审核组件的HTML
                if (element.getAttribute("data-type") == "WorkCheck")
                    figure_Develop_FigureFrmCheck(nodeComponents, element, frmData);
            })
            
        }
    }


}

//从表
function figure_Develop_Dtl(element, frmDtl, ext) {
    //$("<link href='../Comm/umeditor1.2.3-utf8/themes/default/css/umeditor.css' type = 'text/css' rel = 'stylesheet' />").appendTo("head");  
    //在Image元素下引入IFrame文件
    var src = "";
    if (frmDtl.ListShowModel == "0") {
        if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
            src = "../CCForm/Dtl2017.htm"
        else
            src = "./CCForm/Dtl2017.htm";
    }
        //表格模式
      
    if (frmDtl.ListShowModel == "1") {
        //卡片模式
        if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
            src = "../CCForm/DtlCard.htm"
        else
            src = "./CCForm/DtlCard.htm";
    }
        
    src = src + "?EnsName=" + frmDtl.No + "&RefPKVal=" + pageData.OID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=" + pageData.IsReadonly + "&Version=1";

    var W = element.width();
    var eleHtml = $("<div id='Fd" + frmDtl.No + "' style='width:" + W + "px; height:auto;' ></div>");

    var eleIframe = $("<iframe class= 'Fdtl' name='Dtl'  ID = 'Dtl_" + frmDtl.No + "' src = '" + src + "' frameborder=0  style='width:" + W + "px;"
        + "height: auto; text-align: left; '  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
    eleHtml.append(eleIframe);
    $(element).after(eleHtml);
    $(element).remove(); //移除Imge节点
}

//附件
function figure_Develop_Ath(element, ath) {
    var src = "";
    if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
        src = "../CCForm/Ath.htm?PKVal=" + pageData.OID;
    else
        src = "./CCForm/Ath.htm?PKVal=" + pageData.WorkID;
    src = src + "&PWorkID=" + GetQueryString("PWorkID") + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=" + pageData.IsReadonly + "&FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow;
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
function figure_Develop_ImageAth(element, frmImageAth,fk_mapData) {
    var isEdit = frmImageAth.IsEdit;
    var img = $("<img class='pimg'/>");

    var imgSrc = basePath + "/WF/Data/Img/LogH.PNG";

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
            imgSrc = DealJsonExp(frmData.MainTable[0], frmPath);
        }

        //数据来源为指定路径.
        if (frmImage.ImgSrcType == 1) {
            var url = frmImage.ImgURL;
            url = url.replace('＠', '@');
            url = url.replace('@basePath', basePath);
            imgSrc = DealJsonExp(frmData.MainTable[0], url);
        }
        var errorImg = "../DataUser/ICON/CCFlow/LogBig.png";
        if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
            errorImg = "../../DataUser/ICON/CCFlow/LogBig.png";
        // 由于火狐 不支持onerror 所以 判断图片是否存在放到服务器端
        if (imgSrc == "" || imgSrc == null) 
            imgSrc = errorImg;
            
        
            
        
       

        var a = $("<a></a>");
        var img = $("<img/>")

        img.attr("src", imgSrc).css('width', frmImage.W).css('height', frmImage.H).attr('onerror', "this.src='" + errorImg+"'");

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
    if (urlType == 2 || urlType == 3) {
        if (urlType == 2) { //轨迹表
            if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
                url = "../WorkOpt/OneWork/Table.htm";
            else
                url = "./WorkOpt/OneWork/Table.htm";
        } 
        if (urlType == 3) {//轨迹图
            if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
                url = "../WorkOpt/OneWork/TimeBase.htm";
            else
                url = "./WorkOpt/OneWork/TimeBase.htm";
        }
        url =url+"?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.OID + "&FID=" + pageData.FID;

    }
    
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
    var w = wf_node.SF_W;

    var src = "./WorkOpt/SubFlow.htm?s=2";
    if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
        src = "../WorkOpt/SubFlow.htm?s=2";

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
function figure_Develop_FigureFrmCheck(wf_node, element, frmData) {



    var sta = wf_node.FWCSta;
    if (sta == 0 || sta == undefined)
        return $('');

    var node = frmData.WF_Node;
    if (node != undefined)
        node = node[0];

    var frmNode = frmData.WF_FrmNode;
    if (frmNode != undefined)
        frmNode = frmNode[0];

    if (node == null)
        return $('');
    if (frmNode !=null && node.FormType == 5 && frmNode.IsEnableFWC == 0)
        return $('');

    var currentURL = window.location.href;

    var url = "./WorkOpt/";
    if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
        url = '../WorkOpt/';
    if (wf_node.FWCSta != 0) {
        if (wf_node.FWCVer == 0 || wf_node.FWCVer == "" || wf_node.FWCVer == undefined)
            pageData.FWCVer = 0;
        else
            pageData.FWCVer = 1;
    }

    var eleHtml = $("<div id='WorkCheck'>" + WorkCheck_InitPage()+"</div>");

    $(element).after(eleHtml);
    $(element).remove(); //移除SubFlow节点

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
function figure_Develop_HandWrite(HandWriteID, val) {
    var url = basePath + "/WF/CCForm/HandWriting.htm?WorkID=" + pageData.OID + "&FK_Node=" + pageData.FK_Node + "&KeyOfEn=" + HandWriteID;
    OpenBootStrapModal(url, "eudlgframe", '签字板', 400, 300, false);
}
//地图
function figure_Develop_Map(MapID, UIIsEnable) {
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
    $("#TB_" + HandWriteID).val(imagePath);
    $('#eudlg').dialog('close');
}



