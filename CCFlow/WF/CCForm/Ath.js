//点击右边的下载标签.
function Down2018(fk_ath, pkVal, delPKVal) {
    if (plant == "CCFlow")
        window.location.href = 'DownFile.aspx?DoType=Down&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=<%=FK_Node %>&FK_Flow = <%=FK_Flow %>&FK_MapData=<%=FK_MapData %>&Ath=<%=Ath %>';
    else {
        var currentPath = window.document.location.href;
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        Url = 'downLoad.do?DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + '&FK_Node=' + FK_Node + '&FK_Flow=' + FK_Flow + '&FK_MapData=' + FK_MapData + '&Ath=' + Ath;
        window.location.href = Url;
    }

}

//点击文件名称执行的下载.
function Down2017(mypk) {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("MyPK", mypk);
    var data = handler.DoMethodReturnString("AttachmentUpload_Down");
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
        }
        var a = document.getElementById("downPdf");
        a.href = str;
        a.download = "11.txt";
        a.click();

        return;
    }
    if (data.indexOf("fromdb") > -1) {
        url = Handler + "?DoType=AttachmentDownFromByte&MyPK=" + mypk + "&m=" + Math.random();
        $('<form action="' + url + '" method="post"></form>').appendTo('body').submit().remove();
    }
    return;


}


function sa() {

    if (window.frames["hrong"].document.readyState != "complete")

        setTimeout("sa()", 100);

    else

        window.frames["hrong"].document.execCommand('SaveAs');

}

/* 一下的方法从网上找到的，都不适用 . */

function Down3(str) {

    var a;
    a = window.open(str, "_blank", "width=0, height=0,status=0");
    a.document.execCommand("SaveAs");
    a.close();
}

function Down2(imgURL) {

    var oPop = window.open(imgURL, "", "width=1, height=1, top=5000, left=5000");

    for (; oPop.document.readyState != "complete"; ) {
        if (oPop.document.readyState == "complete")
            break;
    }

    oPop.document.execCommand("SaveAs");
    oPop.close();

}

function Down(url) {

    var $eleForm = $("<form method='get'></form>");

    $eleForm.attr("action", url);

    $(document.body).append($eleForm);

    //提交表单，实现下载
    $eleForm.submit();
}

function downloadFile(url) {
    try {
        var elemIF = document.createElement("iframe");
        elemIF.src = url;
        elemIF.style.display = "none";
        document.body.appendChild(elemIF);
    } catch (e) {

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
        }
        window.open(str, "_blank", "width=800, height=600,toolbar=yes");
        return;
    }

}

//删除附件.
function Del(fk_ath, pkVal, delPKVal) {

    if (window.confirm('您确定要删除吗？ ') == false)
        return;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("DelPKVal", delPKVal);
    var data = handler.DoMethodReturnString("AttachmentUpload_Del");

    window.location.href = window.location.href;
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


   