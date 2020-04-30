//点击右边的下载标签.
var webUser = new WebUser();
function Down2018(fk_ath, pkVal, delPKVal) {
    if (plant == "CCFlow")
        window.location.href = basePath+'/WF/CCForm/DownFile.aspx?DoType=Down&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node='+FK_Node+'&FK_Flow='+FK_Flow+'&FK_MapData='+FK_MapData+'&Ath='+Ath;
    else {
        var currentPath = window.document.location.href;
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        Url = path + 'WF/Ath/downLoad.do?DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=' + FK_Node + '&FK_Flow=' + FK_Flow + '&FK_MapData=' + FK_MapData + '&Ath=' + Ath;
        window.location.href = Url;
    }
    
}


function DownZip() {

    var httphandle = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    httphandle.AddUrlData();

    var data = httphandle.DoMethodReturnString("AttachmentUpload_DownZip");

    if (data.indexOf('err@') == 0) {
        alert(data); //如果是异常，就提提示.
        return;
    }

    if (data.indexOf('url@') == 0) {

        data = data.replace('url@', ''); //如果返回url，就直接转向.

        var i = data.indexOf('\DataUser');
        var str = '/' + data.substring(i);
        str = str.replace('\\\\', '\\');
        if (plant != 'CCFlow') {
            var currentPath = window.document.location.href;
            var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
            str = path + str;
        } else {
            str = basePath + str;
        }
        window.location.href = str;

    }

}

//删除附件.
function Del(fk_ath, pkVal, delPKVal) {

    if (window.confirm('您确定要删除吗？ ') == false)
        return;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("DelPKVal", delPKVal);
    var data = handler.DoMethodReturnString("AttachmentUpload_Del");
    numOfAths--;
    window.location.href = window.location.href;
}

//附件图片显示
function FileShowPic(athDesc, dbs) {
    realFileExts = "*.gif,*.jpg,*.jepg,*.jpeg,*.bmp,*.png,*.tif,*.gsp";
    var _Html = "";
    var outerdiv = $("#outerdiv", parent.document);
    var innerdiv = $("#innerdiv", parent.document);
    var bigimg = $("#bigimg", parent.document);
    for (var i = 0; i < dbs.length; i++) {
        var db = dbs[i];
        var url = GetFileStream(db.MyPK, db.FK_FrmAttachment);
        _Html += "<div id='" + db.MyPK + "' class='image-item' style='background-image: url(&quot;" + url + "&quot;);'>";
        if ((athDesc.DeleteWay == AthDeleteWay.DelAll) || ((athDesc.DeleteWay == AthDeleteWay.DelSelf) && (db.Rec == WebUser.No)))
            _Html += "<div class='image-close' onclick='Del(\"" + db.FK_FrmAttachment + "\",\"" + PKVal + "\",\"" + db.MyPK + "\")'>X</div>";
        _Html += "<div style ='width: 100%; height: 100%;' class='athImg' ></div>"; 
        _Html += "<div class='image-name' id = 'name-0-0' > ";
        if (athDesc.IsDownload == 0)
            _Html += "<p style = 'text-align:center;width:63.4px;margin:0;padding:0' >" + db.FileName + "</p>";
        else
            _Html += "<p style = 'text-align:center;width:63.4px;margin:0;padding:0' ><a href=\"javascript:Down2018('" + FK_FrmAttachment + "','" + PKVal + "','" + db.MyPK + "')\">" + db.FileName.split(".")[0]+"</a></p>";
        _Html += "</div>";
        _Html += "</div>";
    }
    //可以上传附件，增加上传附件按钮
    if (athDesc.IsUpload == true && IsReadonly != "1") {
        _Html += "<div class='image-item space'><input type='file' id='file' onchange='UploadChange();'></div>";
    }
    $("#FilePic").html(_Html);
    
    $(".athImg").on("click", function () {
        var _this = $(this); //将当前的pimg元素作为_this传入函数  
        var src = _this.parent().css("background-image").replace("url(\"", "").replace("\")", "")
        imgShow(outerdiv, innerdiv, bigimg, src);
    });
}

function imgShow(outerdiv, innerdiv, bigimg, src) {
    //var src = _this.attr("src"); //获取当前点击的pimg元素中的src属性  
    bigimg.attr("src", src); //设置#bigimg元素的src属性  

    /*获取当前点击图片的真实大小，并显示弹出层及大图*/
    $("<img/>").attr("src", src).load(function () {
        var windowW = $(window).width(); //获取当前窗口宽度  
        var windowH = $(window).height(); //获取当前窗口高度  
        var realWidth = this.width; //获取图片真实宽度  
        var realHeight = this.height; //获取图片真实高度  
        var imgWidth, imgHeight;
        var scale = 0.8; //缩放尺寸，当图片真实宽度和高度大于窗口宽度和高度时进行缩放  

        if (realHeight > windowH * scale) {//判断图片高度  
            imgHeight = windowH * scale; //如大于窗口高度，图片高度进行缩放  
            imgWidth = imgHeight / realHeight * realWidth; //等比例缩放宽度  
            if (imgWidth > windowW * scale) {//如宽度扔大于窗口宽度  
                imgWidth = windowW * scale; //再对宽度进行缩放  
            }
        } else if (realWidth > windowW * scale) {//如图片高度合适，判断图片宽度  
            imgWidth = windowW * scale; //如大于窗口宽度，图片宽度进行缩放  
            imgHeight = imgWidth / realWidth * realHeight; //等比例缩放高度  
        } if (realHeight > windowH * scale) {
            imgWidth = windowH * scale;
            imgHeight = windowH * scale;
        } else {//如果图片真实高度和宽度都符合要求，高宽不变  
            imgWidth = realWidth;
            imgHeight = realHeight;
        }
        bigimg.css("width", imgWidth); //以最终的宽度对图片缩放  

        var w = (windowW - imgWidth) / 2; //计算图片与窗口左边距  
        var h = (windowH - imgHeight) / 2; //计算图片与窗口上边距  
        innerdiv.css({ "top": h, "left": w }); //设置#innerdiv的top和left属性  
        outerdiv.fadeIn("fast"); //淡入显示#outerdiv及.pimg  
    });

    outerdiv.click(function () {//再次点击淡出消失弹出层  
        $(this).fadeOut("fast");
    });
}  

//文件下载
function GetFileStream(mypk,FK_FrmAttachment) {
    if (plant == "CCFlow") {
        var Url = './DownFile.aspx?DoType=Down&MyPK=' + mypk + '&FK_FrmAttachment=' + FK_FrmAttachment;
    } else {
        //按照数据流模式下载。
        var currentPath = window.document.location.href;
        var path = currentPath.substring(0, currentPath.indexOf('/CCMobile') + 1);
        var Url = path + "WF/Ath/downLoad.do?MyPK=" + mypk + "&FK_FrmAttachment=" + FK_FrmAttachment;
    }

    return Url;
}


//解析附件扩张字段
function InitAthMapAttrOfCtrlFool(db, mapAttr) {
    var defValue = "";
    if (db == !"")
        defValue = GetPara(mapAttr.Name, db.AtPara)
    var eleHtml = '';

    //外部数据源类型.
    if (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1) {

        if (mapAttr.UIIsEnable == 0) {
            var ctrl = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' type=hidden  class='form-control' type='text'/>";
            defValue = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn + "T");

            if (defValue == '' || defValue == null)
                defValue = '无';

            ctrl += "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "Text'  value='" + defValue + "' disabled='disabled'   type='text'/>";
            return ctrl;
        }


        return "<select id='DDL_" + mapAttr.KeyOfEn + "' name='DDL_" + mapAttr.KeyOfEn + "'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(mapAttr, defValue) + "</select>";
    }

    //外键类型.
    if (mapAttr.LGType == "2" && mapAttr.MyDataType == "1") {

        var data = flowData[mapAttr.UIBindKey];
        //枚举类型.
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";


        return "<select id='DDL_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(mapAttr, defValue) + "</select>";
    }

    //添加文本框 ，日期控件等.
    //AppString
    if (mapAttr.MyDataType == "1") {  //不是外键

        if (mapAttr.UIHeight <= 40) //普通的文本框.
            return "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'   type='text'/>";

        if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {

            var eleHtml = "";
            //如果是富文本就使用百度 UEditor
            if (mapAttr.UIIsEnable == "0") {
                //只读状态直接 div 展示富文本内容                
                eleHtml += "<div class='richText' style='width:99%;margin-right:2px'>" + defValue + "</div>";

            } else {
                document.BindEditorMapAttr = mapAttr; //存到全局备用

                //设置编辑器的默认样式
                var styleText = "text-align:left;font-size:12px;";
                styleText += "width:100%;";
                styleText += "height:" + mapAttr.UIHeight + "px;";
                //注意这里 name 属性是可以用来绑定表单提交时的字段名字的 id 是特殊约定的.
                eleHtml += "<script id='editor'  name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";

            }

            eleHtml = "<div style='white-space:normal;'>" + eleHtml + "</div>";
            return eleHtml
        }

        //普通的大块文本.
        return "<textarea maxlength=" + mapAttr.MaxLen + "  style='height:" + mapAttr.UIHeight + "px;width:100%;' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' type='text'  " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + " />"
    }

    //日期类型.
    if (mapAttr.MyDataType == 6) {
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return "<input type='text' " + enableAttr + " value='" + defValue + "' style='width:120px;' class='form-control' id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    //时期时间类型.
    if (mapAttr.MyDataType == 7) {

        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return "<input  type='text'  value='" + defValue + "' style='width:140px;' class='form-control' " + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' />";
    }

    // boolen 类型.
    if (mapAttr.MyDataType == 4) {  // AppBoolean = 7

        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        //CHECKBOX 默认值
        var checkedStr = '';
        if (checkedStr != "true" && checkedStr != '1') {
            checkedStr = ' checked="checked" ';
        }

        return "<label ><input " + enableAttr + " " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' id='CB_" + mapAttr.KeyOfEn + "'  name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + " /> </label>";
    }

    //枚举类型.
    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";
        if (mapAttr.UIContralType == 1)
        //return "<select " + enableAttr + "  id='DDL_" + mapAttr.KeyOfEn + "' class='form-control' >" + InitDDLOperation(flowData, mapAttr, defValue) + "</select>";
            return "<select id='DDL_" + mapAttr.KeyOfEn + "' name='DDL_" + mapAttr.KeyOfEn + "' class='form-control'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitAthDDLOperation(mapAttr, defValue) + "</select>";
        if (mapAttr.UIContralType == 3) {
            //横向排列
            var RBShowModel = 3;
            if (mapAttr.AtPara.indexOf("@RBShowModel=3") == -1)
                RBShowModel = 0;
            return InitRBShowContent(mapAttr, defValue, RBShowModel, enableAttr);

        }
    }

    // AppDouble  AppFloat
    if (mapAttr.MyDataType == 5 || mapAttr.MyDataType == 3) {
        return "<input  value='" + defValue + "' style='text-align:right;width:80px;'class='form-control'  onkeyup=" + '"' + "if(isNaN(value)) execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    if ((mapAttr.MyDataType == 2)) { //AppInt
        var enableAttr = '';
        if (mapAttr.UIIsEnable != 1) {
            enableAttr = "disabled='disabled'";
        }

        //alert(defValue);

        return "<input  value='0' style='text-align:right;width:80px;' class='form-control' onkeyup=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    //AppMoney  AppRate
    if (mapAttr.MyDataType == 8) {
        return "<input  value='" + defValue + "' style='text-align:right;width:80px;' class='form-control' onkeyup=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    alert(mapAttr.Name + "的类型没有判断.");
    return;
}

//初始化下拉列表框的OPERATION
function InitAthDDLOperation(mapAttr, defVal) {

    var operations = '';

    //外键类型的.
    if (mapAttr.LGType == 2) {

        if (flowData[mapAttr.KeyOfEn] != undefined) {

            $.each(flowData[mapAttr.KeyOfEn], function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });

        }

        if (flowData[mapAttr.UIBindKey] != undefined) {

            $.each(flowData[mapAttr.UIBindKey], function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
        }
        return operations;
    }

    //枚举类型的.
    if (mapAttr.LGType == 1) {
        var enums = new Entities("BP.Sys.SysEnums");
        enums.Retrieve("EnumKey", mapAttr.UIBindKey);


        $.each(enums, function (i, obj) {
            operations += "<option " + (obj.IntKey == defVal ? " selected='selected' " : "") + " value='" + obj.IntKey + "'>" + obj.Lab + "</option>";
        });
        return operations;
    }


    //外部数据源类型 FrmGener.js.InitDDLOperation
    if (mapAttr.LGType == 0) {

        //如果是一个函数.
        var fn;
        try {
            if (mapAttr.UIBindKey) {
                fn = eval(mapAttr.UIBindKey);
            }
        } catch (e) {
            // alert(e);
        }

        if (typeof fn == "function") {
            $.each(fn.call(), function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
            return operations;
        }

        if (typeof CommonHandler == "function") {
            CommonHandler.call("", mapAttr.UIBindKey, function (data) {
                GenerBindDDL("DDL_" + mapAttr.KeyOfEn, data, "No", "Name");
            })
            return "";
        }

        if (mapAttr.UIIsEnable == 0) {

            alert('不可编辑');
            operations = "<option  value='" + defVal + "'>" + defVal + "</option>";
            return operations;
        }

        if (flowData[mapAttr.KeyOfEn] != undefined) {
            $.each(flowData[mapAttr.KeyOfEn], function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
            return operations;
        }

        if (flowData[mapAttr.UIBindKey] != undefined) {

            $.each(flowData[mapAttr.UIBindKey], function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
            return operations;
        }
        return "";
        //   alert('没有获得约定的数据源.');
        alert('没有获得约定的数据源..' + mapAttr.KeyOfEn + " " + mapAttr.UIBindKey);
    }

    alert(mapAttr.LGType + "没有判断.");
}

function InitRBShowContent(mapAttr, defValue, RBShowModel, enableAttr) {
    var rbHtml = "";
    var enums = new Entities("BP.Sys.SysEnums");
    enums.Retrieve("EnumKey", mapAttr.UIBindKey);
    enums = $.grep(enums, function (value) {
        return value.EnumKey == mapAttr.UIBindKey;
    });
    $.each(enums, function (i, obj) {
        if (RBShowModel == 3)
        //<input  " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' id='CB_" + mapAttr.KeyOfEn + "'  name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + " /> &nbsp;" + mapAttr.Name + "</label</div>";
            rbHtml += "<label><input " + enableAttr + " " + (obj.IntKey == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "' />&nbsp;" + obj.Lab + "</label>";
        else
            rbHtml += "<label><input " + enableAttr + " " + (obj.IntKey == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "'  />&nbsp;" + obj.Lab + "</label><br/>";
    });
    return rbHtml;
}

function GetPara(key, AtPara) {
    var atPara = AtPara;
    if (typeof atPara != "string" || typeof key == "undefined" || key == "") {
        return "";
    }
    var reg = new RegExp("(^|@)" + key + "=([^@]*)(@|$)");
    var results = atPara.match(reg);
    if (results != null) {
        return unescape(results[2]);
    }
    return "";
}

//必填项检查   名称最后是*号的必填  如果是选择框，值为'' 或者 显示值为 【*请选择】都算为未填 返回FALSE 检查必填项失败
function checkBlanks() {
    var checkBlankResult = true;
    //获取所有的列名 找到带* 的LABEL mustInput
    //var lbs = $('[class*=col-md-1] label:contains(*)');
    var lbs = $('.mustInput'); //获得所有的class=mustInput的元素.
    $.each(lbs, function (i, obj) {
        if ($(obj).parent().css('display') != 'none' && $(obj).parent().next().css('display')) {
            var keyofen = $(obj).data().keyofen;

            var ele = $('[id$=_' + keyofen + ']');
            if (ele.length == 1) {
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
                        if (ele.val() == "" || ele.children('option:checked').text() == "*请选择") {
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
            }
        }
    });


    //2.对 UMEditor 中的必填项检查
    if (document.activeEditor != null && document.activeEditor.$body != null) {

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

function setIframeHeight() {
    var h = $("body").height();
    if ($("body").height() < 100) {
        h = 100;
    }
    $("#" + window.frameElement.getAttribute("id"), parent.document).height(h + 40);
}



   