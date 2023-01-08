//将v1版本表单元素转换为v2 杨玉慧  silverlight 自由表单转化为H5表单
function GenerFrm() {

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
    handler.AddUrlData();

    var data = handler.DoMethodReturnString("FrmView_Init"); //执行保存方法.

    if (data.indexOf('err@') == 0) {
        mui.alert(data);
        return;
    }
    jsonStr = data;
    var gengerWorkNode = {};
    var flow_Data;

    try {
        flow_Data = JSON.parse(data);
        frmData = flow_Data;
    } catch (err) {
        mui.alert("GenerFrm转换JSON失败:" + jsonStr);
        return;
    }
    //获取没有解析的外部数据源
    var uiBindKeys = flow_Data["UIBindKey"];
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
                    if (plant == 'CCFLOW') {
                        selectStatement = basePath + "/DataUser/SFTableHandler.ashx" + selectStatement;
                    } else {
                        selectStatement = basePath + "/DataUser/SFTableHandler" + selectStatement;
                    }
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
    //定义常用的变量.

    //设置标题.
    document.getElementById("title").innerHTML = "表单预览";


    BindFrm(frmData);


}
//绑定表单.
function BindFrm(frmData) {
    //分组信息.
    var Sys_GroupFields = frmData.Sys_GroupField;
    var Fk_MapData = frmData.Sys_MapData[0].No;

    //var node = frmData.WF_Node[0];

    //加入隐藏字段
    var mapAttrsHtml = "";
    $.grep(frmData.Sys_MapAttr, function (item) {
        return item.IsEnableInAPP == "0";

    }).forEach(function (attr) {
        var defval = ConvertDefVal(frmData.MainTable[0], attr.DefVal, attr.KeyOfEn);
        mapAttrsHtml += "<input type='hidden' id='TB_" + attr.KeyOfEn + "' name='TB_" + attr.KeyOfEn + "' value='" + defval + "' />";


    });
    $('#CCForm').append(mapAttrsHtml);
    //遍历循环生成 li
    var node = new Entity("BP.WF.Node");
    var fk_node = GetQueryString("FK_Node");
    if (fk_node != null && fk_node != undefined && fk_node != "0" && fk_node != "null") {
        node.SetPKVal(fk_node);
        node.Retrieve();
    }

    for (var i = 0; i < Sys_GroupFields.length; i++) {

        var gf = Sys_GroupFields[i];
        if (gf.CtrlType == 'FWC' && node.FWCSta == 0) {
            continue;
        }
        if (gf.CtrlType != 'Ath')
            mapAttrsHtml += "<div class=\"mui-table-view-divider\"><h5 style='color:black;'>" + gf.Lab + "</h5></div>";

        //附件类的控件.
        if (gf.CtrlType == 'Ath') {

            mapAttrsHtml += InitAth(frmData, gf);
            continue;
        }

        //明细表的控件.
        if (gf.CtrlType == 'Dtl') {
            mapAttrsHtml += InitDtl(frmData, gf);
            continue;
        }

        //字段类的控件.
        if (gf.CtrlType == '' || gf.CtrlType == null) {
            mapAttrsHtml += InitMapAttr(frmData.Sys_MapAttr, gf.OID);
            continue;
        }
    }
    //展显
    $(mapAttrsHtml).appendTo('#CCForm');
    //日期控件
    mui(".mui-input-row").on("tap", ".ccformdate", function () {
        var dDate = new Date();
        var optionsJson = this.getAttribute('data-options') || '{}';
        var ctrID = this.getAttribute('id');
        var options = JSON.parse(optionsJson);
        var picker = new mui.DtPicker(options);
        picker.show(function (rs) {
            var timestr = rs.text;
            $("#" + ctrID).html(timestr);
            picker.dispose();
        });
    });


    //为 DISABLED 的 TEXTAREA 加TITLE 
    var disabledTextAreas = $('#divCCForm textarea:disabled');
    $.each(disabledTextAreas, function (i, obj) {
        $(obj).attr('title', $(obj).val());
    })

    //根据NAME 设置ID的值
    var inputs = $('[name]');
    $.each(inputs, function (i, obj) {
        if ($(obj).attr("id") == undefined || $(obj).attr("id") == '') {
            $(obj).attr("id", $(obj).attr("name"));
        }
    });

    //mui(".mui-switch").switch();//注掉：开关按钮不可编辑

    var enName = frmData.Sys_MapData[0].No;

    try {
        ////加载JS文件
        var s = document.createElement('script');
        s.type = 'text/javascript';
        s.src = basePath+"/DataUser/JSLibData/" + enName + "_Self.js";
        var tmp = document.getElementsByTagName('script')[0];
        tmp.parentNode.insertBefore(s, tmp);
    }
    catch (err) {
    }

    if (frmData.Sys_FrmAttachment.length > 0) {
        try {
            var s = document.createElement('script');
            s.type = 'text/javascript';
            s.src = "./js/mui/js/feedback-page.js";
            var tmp = document.getElementsByTagName('script')[0];
            tmp.parentNode.insertBefore(s, tmp);
        }
        catch (err) {
        }
    }

    //设置默认值
    for (var j = 0; j < frmData.Sys_MapAttr.length; j++) {

        var mapAttr = frmData.Sys_MapAttr[j];

        //添加 label
        //如果是整行的需要添加  style='clear:both'.
        var defValue = ConvertDefVal(frmData.MainTable[0], mapAttr.DefVal, mapAttr.KeyOfEn);

        if ($('#TB_' + mapAttr.KeyOfEn).length == 1) {
            if (defValue == "1") {
                //判断是否存在mui-active这个类
                if ($("#SW_" + mapAttr.KeyOfEn).hasClass("mui-active") == false)
                    $("#SW_" + mapAttr.KeyOfEn).addClass("mui-active");
            } else {
                //判断是否存在mui-active这个类
                if ($("#SW_" + mapAttr.KeyOfEn).hasClass("mui-active") == true)
                    $("#SW_" + mapAttr.KeyOfEn).removeClass("mui-active");
            }
            $('#TB_' + mapAttr.KeyOfEn).val(defValue);
            $('#TB_' + mapAttr.KeyOfEn).html(defValue);//只读大文本放到div里
        }

        if ($('#DDL_' + mapAttr.KeyOfEn).length == 1) {
            $('#DDL_' + mapAttr.KeyOfEn).val(defValue);
        }

        if ($('#CB_' + mapAttr.KeyOfEn).length == 1) {
            if (defValue == "1")
                $('#CB_' + mapAttr.KeyOfEn).attr("checked", true);
            else
                $('#CB_' + mapAttr.KeyOfEn).attr("checked", false);
        }
    }
    //处理下拉框级联等扩展信息
    if (pageData.IsReadonly != "1") {

        // AfterBindEn_DealMapExt(frmData);
    }

    //设置为只读模式
    setFormEleDisabled();

}

function Change() {
    var btn = document.getElementById('Btn_Save');
    if (btn != null) {
        if (btn.value.valueOf('*') == -1)
            btn.value = btn.value + '*';
    }
}


function To(url) {
    //window.location.href = filterXSS(url);
    window.name = "dialogPage"; window.open(url, "dialogPage")
}

//公共方法
function AjaxService(param, callback, scope, levPath) {
    $.ajax({
        type: "GET", //使用GET或POST方法访问后台
        dataType: "text", //返回json格式的数据
        contentType: "application/json; charset=utf-8",
        url: MyFlow, //要访问的后台地址
        data: param, //要发送的数据
        async: true,
        cache: false,
        complete: function () { }, //AJAX请求完成时隐藏loading提示
        error: function (XMLHttpRequest, errorThrown) {
            callback(XMLHttpRequest);
        },
        success: function (msg) {//msg为返回的数据，在这里做数据绑定
            var data = msg;
            callback(data, scope);
        }
    });
}

function Do(warning, url) {
    if (window.confirm(warning) == false)
        return;
    SetHref(url);
}

//以下是软通写的
//初始化网页URL参数
function initPageParam() {
    pageData.FK_Flow = GetQueryString("FK_Flow");
    pageData.FK_Node = GetQueryString("FK_Node");
    pageData.FID = GetQueryString("FID") == null ? 0 : GetQueryString("FID");
    pageData.WorkID = GetQueryString("WorkID") == null ? GetQueryString("OID") : GetQueryString("WorkID");
    pageData.IsRead = GetQueryString("IsRead");
    pageData.T = GetQueryString("T");
    pageData.Paras = GetQueryString("Paras");
    pageData.IsReadOnly = "1"; //如果是IsReadOnly，就表示是查看页面，不是处理页面
    pageData.IsStartFlow = GetQueryString("IsStartFlow"); //是否是启动流程页面 即发起流程

    pageData.DoType1 = GetQueryString("DoType")//View
    pageData.IsReadonly = 1
}




//设置表单元素不可用
function setFormEleDisabled() {
    //文本框等设置为不可用
    $('#divCCForm textarea').attr('disabled', 'disabled');
    $('#divCCForm select').attr('disabled', 'disabled');
    /*$('#divCCForm input[type!=button]').attr('disabled', 'disabled');*/
}

//移交
//子线程
//子流程
//{"IsSuccess":true,"Msg":null,"ErrMsg":null,"List":null,"Data":2}
function getData(data, url, dataParam) {
    var jsonStr = '{"IsSuccess":true,"Msg":null,"ErrMsg":null,"List":null,"Data":2}';
    var data = JSON.parse(jsonStr);
    if (data.IsSuccess != true) {
        alert('返回参数失败，ErrMsg:' + data.ErrMsg + ";Msg:" + data.Msg + ";url:" + url);
    }
    return data;
}

var pageData = {};
var globalVarList = {};



window.onresize = function () {
    if (pageData.Col == 8) {
        if (jsonStr != undefined && jsonStr != '') {
            var frmData = JSON.parse(jsonStr);
            //设置CCFORM的表格宽度  
            if (document.body.clientWidth > 992) {//处于中屏时设置宽度最小值
                $('#CCForm').css('min-width', frmData.Sys_MapData[0].TableWidth);
            }
            else {
                $('#CCForm').css('min-width', 0);
            }
        }
    }
}

//逻辑类型、数据类型、控件类型
var FieldTypeS = { Normal: 0, Enum: 1, FK: 2, WinOpen: 3 },
    FormDataType = { AppString: 1, AppInt: 2, AppFloat: 3, AppBoolean: 4, AppDouble: 5, AppDate: 6, AppDateTime: 7, AppMoney: 8, AppRate: 9 },
    UIContralType = { TB: 0, DDL: 1, CheckBok: 2, RadioBtn: 3, MapPin: 4, MicHot: 5 };

//解析表单字段 MapAttr
function InitMapAttr(Sys_MapAttr, groupID) {

    var _html = "";
    $.grep(Sys_MapAttr, function (item) {
        return (item.IsEnableInAPP != 0 && item.UIVisible != 0) && item.GroupID == groupID;

    }).forEach(function (attr) {

        //图片签名
        if (attr.IsSigan == "1") {
            _html += "<div class='mui-input-row'>";
            _html += FormUtils.CreateSignPicture(attr);
            _html += "</div>";
            return;
        }
        //        _html += "<div class='mui-input-row'>";
        //加载其他数据控件
        switch (attr.LGType) {
            case FieldTypeS.Normal: //输出普通类型字段
                if (attr.UIContralType == UIContralType.DDL) {
                    //判断外部数据或WS类型.
                    _html += "<div class='mui-input-row'>";
                    _html += FormUtils.CreateDDLPK(attr);
                    break;
                }
                switch (attr.MyDataType) {
                    case FormDataType.AppString:
                        _html += FormUtils.CreateTBString(attr);
                        break;
                    case FormDataType.AppInt:
                        _html += "<div class='mui-input-row'>";
                        _html += FormUtils.CreateTBInt(attr);
                        break;
                    case FormDataType.AppFloat:
                    case FormDataType.AppDouble:
                    case FormDataType.AppMoney:
                        _html += "<div class='mui-input-row'>";
                        _html += FormUtils.CreateTBFloat(attr);
                        break;
                    case FormDataType.AppDate:
                        //日期\boolen型的不允许获取焦点，所以只能禁用
                        _html += "<div class='mui-input-row'>";
                        _html += FormUtils.CreateTBDate(attr);
                        break;
                    case FormDataType.AppDateTime:
                        //日期\boolen型的不允许获取焦点，所以只能禁用
                        _html += "<div class='mui-input-row'>";
                        _html += FormUtils.CreateTBDateTime(attr);
                        break;
                    case FormDataType.AppBoolean:
                        //日期\boolen型的不允许获取焦点，所以只能禁用
                        _html += "<div class='mui-input-row'>";
                        _html += FormUtils.CreateCBBoolean(attr);
                        break;
                }
                break;
            case FieldTypeS.Enum: //枚举值下拉框
                if (attr.Name.length >= 10) {

                    // var ctrl_ID = "RB_" + attr.KeyOfEn;
                    // if (attr.UIContralType == UIContralType.DDL) {
                    var ctrl_ID = "DDL_" + attr.KeyOfEn;
                    // }

                    _html += "<label  for=\"" + ctrl_ID + "\"><p style='padding-left: 5px;'>" + attr.Name + "</p></label>";
                    _html += "<div class='mui-input-row'>";
                    _html += "<select name=\"" + ctrl_ID + "\" id=\"" + ctrl_ID + "\"  " + (attr.UIIsEnable == "0" ? "disabled" : "") + " >";

                    _html += InitDDLOperation(frmData, attr, "");
                    _html += "</select>";
                } else {
                    _html += "<div class='mui-input-row'>";
                    _html += FormUtils.CreateDDLEnum(attr);

                }
                break;
            case FieldTypeS.FK: //外键表下拉框
                _html += "<div class='mui-input-row'>";
                _html += FormUtils.CreateDDLPK(attr);
                break;
            case FieldTypeS.WinOpen: //打开系统页面
                _html += "<div class='mui-input-row'>";
                switch (item.UIContralType) {
                    case UIContralType.MapPin: //地图定位
                        _html += FormUtils.CreateMapPin(attr);
                        break;
                    case UIContralType.MicHot: //语音控件
                        _html += FormUtils.CreateMicHot(attr);
                        break;
                }
                break;
        }
        _html += "</div>";
    });

    return _html;
}

var FormUtils = {
    CreateSignPicture: function (attr) {
        //图片签名+oitw "kyrw   \[i6514
        var val = ConvertDefVal(frmData.MainTable[0], attr.DefVal, attr.KeyOfEn);
        var html_Sign = "<label for=\"Sign_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label>";
        html_Sign += "<div align=\"left\">";
        html_Sign += "<img name=\"Sign_" + attr.KeyOfEn + "\" id=\"Sign_" + attr.KeyOfEn + "\" src='" + basePath + "/DataUser/Siganture/" + val + ".jpg' border=0 onerror=\"this.src='" + basePath+"/DataUser/Siganture/UnName.jpg'\"/>";
        //        html_Sign += defValue;
        html_Sign += "</div>";
        return html_Sign;
    },
    CreateTBString: function (attr) {
        var html_string = "";
        var strPlaceholder = "请输入";
        //启用二维码
        if (attr.IsEnableQrCode && attr.IsEnableQrCode == "1") {
            strPlaceholder = "通过扫一扫添加";
            Form_Ext_Function += "$('#Btn_" + attr.KeyOfEn + "').on('tap', function () { QrCodeToInput('TB_" + attr.KeyOfEn + "'); });"
            html_string = "<label for=\"TB_" + attr.KeyOfEn + "\">" + attr.Name + "</label>";
            html_string += "<div class=\"QrCodeBar ui-grid-a\">";
            html_string += "  <div class=\"ui-block-a\">";
            html_string += "      <input " + (attr.UIIsEnable == "0" ? "disabled" : "") + " type='text' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder=\"" + strPlaceholder + "\" />";
            html_string += "  </div>";
            html_string += "  <div class=\"ui-block-b\">";
            html_string += "      <div style='margin-top:12px;'>";
            html_string += "         <img id='Btn_" + attr.KeyOfEn + "' src='image/Field/scanQbar.png' width='29' height='24'/>";
            html_string += "      </div>";
            html_string += "  </div>";
            html_string += "</div>";
            return html_string;
        }

        //大文本备注信息解析
        if (attr.UIContralType == 60) {
            html_string += "<div class='' style='padding:11px 15px;line-height:1.1;'>";
            var filename = basePath + "/DataUser/CCForm/BigNoteHtmlText/" + attr.FK_MapData + ".htm";
            var htmlobj = $.ajax({ url: filename, async: false });
            var str = htmlobj.responseText;
            if (htmlobj.status == 404)
                str == "";
            html_string += str;
            html_string += "</div>";
            return html_string;
        }


        //多行文本
        if (attr.UIHeight > 30) {
            html_string += "<div class='' style='padding:11px 15px;line-height:1.1;'>";
            html_string += "<label for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label>";

            if (attr.AtPara && attr.AtPara.indexOf("@IsRichText=1") >= 0) {

                //如果富文本有数据，就用 div 展示html
                html_string += "<textarea wrap='virtual' onpropertychange= 'this.style.posHeight=this.scrollHeight' cols='40' style='overflow-y:visible;font-size:14px;width:100%;border:solid 1px gray;' rows=\"6\" placeholder=\"" + strPlaceholder + "\" name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\"></textarea>";
            } else {
                //非富文本或者 如果没有数据 就用textarea
                if (attr.UIIsEnable == "0")
                    html_string += "<div name='TB_" + attr.KeyOfEn + "' id='TB_" + attr.KeyOfEn + "' style='padding:5px;border:1px solid #d6dde6;font-size: 14px;line-height:22px;'></div>";
                else
                    html_string += "<textarea wrap='virtual' onpropertychange= 'this.style.posHeight=this.scrollHeight' cols='40' style='overflow-y:visible;font-size:14px;width:100%;border:solid 1px gray;' rows=\"6\" placeholder=\"" + strPlaceholder + "\" name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\"></textarea>";
            }
            return html_string;
        }

        //单行文本
        if (attr.UIIsInput == 1 && attr.UIIsEnable == 1) {
            html_string += "<div class='mui-input-row'>";
            html_string += "<label for=\"TB_" + attr.KeyOfEn + "\"  class='mustInput'><p>" + attr.Name + "</p></label>";
        } else {
            html_string += "<div class='mui-input-row'>";
            html_string += "<label for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label>";
        }

        if (attr.UIIsEnable == "0")
            html_string += "<input readonly='readonly' type='text' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder=\"" + strPlaceholder + "\" />";
        else
            html_string += "<input type='text' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder=\"" + strPlaceholder + "\" />";

        return html_string;
    },
    CreateTBInt: function (attr) {
        var inputHtml = "<label for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label>";
        if (attr.UIIsEnable == "0") {
            inputHtml += "<input readonly='readonly' type=\"number\" pattern=\"[0 - 9] * \"";
            inputHtml += " name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder='0' />";
        } else {
            inputHtml += "<input type=\"number\" pattern=\"[0 - 9] * \"";
            inputHtml += " name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder='0' />";
        }


        return inputHtml;
    },
    CreateTBFloat: function (attr) {

        var inputHtml = "<label for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label>"
        if (attr.UIIsEnable == "0") {
            inputHtml += "<input readonly='readonly' type =\"number\" name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder=\"0.00\" />\";"
        } else {
            inputHtml += "<input type =\"number\" name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder=\"0.00\" />\";"
        }
        return inputHtml;
    },
    CreateTBDate: function (attr) {

        var inputHtml = "<label for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label>";
        if (attr.UIIsEnable == "0") {
            inputHtml += "<input readonly='readonly' type='text' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" />";
        }
        else {
            inputHtml += "<input   type='text'  name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" />";
        }
        return inputHtml;
    },
    CreateTBDateTime: function (attr) {
        //Form_Ext_Function += "$('#TB_" + attr.KeyOfEn + "').datetimepicker({lang:'ch'});";
        var inputHtml = "<label for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label>";

        if (attr.UIIsEnable == "0") {
            inputHtml += "<input  name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\"  readonly='readonly' type='text' />";
        }
        else {
            inputHtml += "  <input name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\"  />";
        }
        return inputHtml;
    },
    CreateCBBoolean: function (attr) {
        var checkBoxVal = "";
        var keyOfEn = attr.KeyOfEn;
        var CB_Html = "";
        CB_Html += "  <label><p>" + attr.Name + "</p></label>";
        CB_Html += "  <input type='hidden'  id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' value='" + attr.DefVal + "'/>";
        CB_Html += "  <div class='mui-switch mui-switch-blue mui-switch-mini' id='SW_" + attr.KeyOfEn + "'>";
        CB_Html += "      <div class='mui-switch-handle'></div>";
        CB_Html += "  </div>";
        //CB_Html += "  <label><p>" + attr.Name + "</p></label>";
        //CB_Html += "  <input type='hidden' name='CB_" + keyOfEn + "' value='0'/>";
        //CB_Html += "  <div class='mui-switch mui-switch-blue mui-switch-mini mui-active'>";
        //CB_Html += "      <div class='mui-switch-handle'></div>";
        //CB_Html += "  </div>";
        //CB_Html += "  <input readonly='" + (attr.UIIsEnable == "0" ? "readonly" : "") + "' type=\"checkbox\" name=\"CB_" + keyOfEn + "\" id=\"CB_" + keyOfEn + "\" " + checkBoxVal + " />";
        return CB_Html;
    },
    CreateDDLEnum: function (attr) {
        //下拉框和单选都使用下拉框实现
        // var ctrl_ID = "RB_" + attr.KeyOfEn;
        // if (attr.UIContralType == UIContralType.DDL) {
        var ctrl_ID = "DDL_" + attr.KeyOfEn;
        // }

        var html_Select = "<label for=\"" + ctrl_ID + "\"><p>" + attr.Name + "</p></label>";
        html_Select += "<select name=\"" + ctrl_ID + "\" id=\"" + ctrl_ID + "\"  " + (attr.UIIsEnable == "0" ? "disabled" : "") + " >";

        html_Select += InitDDLOperation(frmData, attr, "");
        html_Select += "</select>";
        return html_Select;
    },
    CreateDDLPK: function (attr) {
        var html_Select = "<label for=\"DDL_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label>";
        html_Select += "<select name=\"DDL_" + attr.KeyOfEn + "\" id=\"DDL_" + attr.KeyOfEn + "\" readonly='" + (attr.UIIsEnable == "0" ? "readonly" : "") + "'>";

        html_Select += InitDDLOperation(frmData, attr, "");
        html_Select += "</select>&nbsp;&nbsp;";
        return html_Select;
    },
    CreateMapPin: function (attr) {
        var html_MapPin = "<label for=\"TB_" + attr.KeyOfEn + "\">" + attr.Name + "</label>";
        //展示内容
        html_MapPin += "<div align=\"left\">";
        if (this.Enable == false) {
            html_MapPin += "<img name=\"MapPin_" + attr.KeyOfEn + "\" id=\"MapPin_" + attr.KeyOfEn + "\" src='image/Field/ic_pindisabled.png' border=0  width=\"" + attr.UIWidth + "\" height=\"" + attr.UIHeight + "\" align='middle'/>";
        } else {
            html_MapPin += "<img onclick=\"GetMapLocationAddress('" + attr.KeyOfEn + "')\" name=\"MapPin_" + attr.KeyOfEn + "\" id=\"MapPin_" + attr.KeyOfEn + "\" src='image/Field/ic_pin.png' border=0 width=\"" + attr.UIWidth + "\" height=\"" + attr.UIHeight + "\" align='middle'/>";
        }
        html_MapPin += "<span onclick=\"OpenMapView('" + attr.KeyOfEn + "')\" style=\"margin-left:5px;\" name=\"LBL_" + attr.KeyOfEn + "\" id=\"LBL_" + attr.KeyOfEn + "\"></span>";
        html_MapPin += "</div>";
        //数据控件
        html_MapPin += "<input type='hidden' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" />";
        //地图定位
        return html_MapPin;
    },
    CreateMicHot: function (attr) {
        var html_MicHot = "<label for=\"TB_" + attr.KeyOfEn + "\">" + attr.Name + "</label>";
        var bDelete = this.Enable;
        //展示内容
        html_MicHot += "<div>";
        if (this.Enable == false) {
            html_MicHot += "<img align=\"left\" name=\"MicHot_" + attr.KeyOfEn + "\" id=\"MicHot_" + attr.KeyOfEn + "\" src='image/Field/microphonedisabled.png' border=0  width=\"" + attr.UIWidth + "\" height=\"" + attr.UIHeight + "\"/>";
        } else {
            html_MicHot += "<img align=\"left\" onclick=\"StartOpenRecord('" + attr.KeyOfEn + "')\" name=\"MicHot_" + attr.KeyOfEn + "\" ";
            html_MicHot += "id=\"MicHot_" + attr.KeyOfEn + "\" src='image/Field/microphonehot.png' border=0 width=\"" + attr.UIWidth + "\" height=\"" + attr.UIHeight + "\"/>";
        }
        html_MicHot += "<img src='image/Field/wx_startplay.gif' align='middle' style='display:none;' />";
        html_MicHot += "<div align=\"left\" style=\"margin-left:15px;float:left;\" name=\"Recorde_" + attr.KeyOfEn + "\" id=\"Recorde_" + attr.KeyOfEn + "\"></div>";
        html_MicHot += "</div><br /><br />";
        html_MicHot += "<div id=\"PanelRecords_" + attr.KeyOfEn + "\">";

        //获取历史语音
        var args = new RequestArgs();
        var keyOfEn = attr.KeyOfEn;

        html_MicHot += "</div>";
        //语音
        return html_MicHot;
    }
};


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
    if (data == undefined) {
        return operations;
    }
    $.each(data, function (i, obj) {
        operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
    });
    return operations;
}
//明细表
function InitDtl(frmData, gf) {
    var dtlHtml = "";
    if (frmData.Sys_MapDtl) {
        $.each(frmData.Sys_MapDtl, function (i, dtl) {
            if (gf.CtrlID == dtl.No) {
                if (dtl.MobileShowModel == undefined || dtl.MobileShowModel == 0) {
                    var func = "Dtl_ShowPage(\"" + dtl.No + "\",\"" + dtl.Name + "\")";
                    dtlHtml += "<div class='mui-table-view-cell'>";
                    dtlHtml += "	<a class='mui-navigate-right' data-title-type='native' href='javascript:" + func + "'><h5>" + dtl.Name
                        + "<span id='" + dtl.No + "_Count'></span></h5>";
                    dtlHtml += "		<p>点击查看详细</p>";
                    dtlHtml += "	</a>";
                    dtlHtml += "</div>";
                    return;
                }
                //列表模式展示
                if (dtl.MobileShowModel == 1) {
                    dtlHtml = GetDtlList(dtl.No);
                }
            }
        });
    }
    return dtlHtml;
}

var dtlExt = {};
//获取从表列表
function GetDtlList(dtlNo) {
    //获得mapdtl实体的基本信息.
    var hand = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    hand.AddPara("EnsName", dtlNo);
    hand.AddPara("RefPKVal", pageData.WorkID);
    hand.AddPara("FK_Node", pageData.FK_Node);
    hand.AddPara("IsReadonly", pageData.IsReadonly);
    mainData = hand.DoMethodReturnJSON("Dtl_Init");
    //主表数据，用于变量替换.
    var mainTable = mainData["MainTable"]; //主表数据.
    //从表信息.
    sys_MapDtl = mainData["Sys_MapDtl"][0]; //从表描述.
    sys_mapAttr = JSON.stringify(mainData["Sys_MapAttr"]); //从表字段.
    var sys_mapExtDtl = mainData["Sys_MapExt"]; //扩展信息.
    var dbDtl = mainData["DBDtl"]; //从表数据.

    if (!$.isArray(dtlExt[dtlNo])) {
        dtlExt[dtlNo] = [];
    }
    dtlExt[dtlNo].push(mainData);

    //存储消息

    var _Html = '<div class="mui-card" style="margin-bottom: 35px;">';
    _Html += '<ul class="mui-table-view">';
    for (var i = 0; i < dbDtl.length; i++) {
        _Html += '<li class="mui-table-view-cell mui-media">';
        _Html += '<a href="javascript:;">';
        _Html += "<button type='button' class='mui-btn mui-btn-success mui-btn-outlined' onclick='Dtl_InitPage(0,\"" + dtlNo + "\"," + dbDtl[i].OID + ")'>";
        _Html += '查看';
        _Html += '<span class="mui-iconmui-icon-search"></span>';
        _Html += '</button>';

        _Html += '<div class="mui-media-body">';
        _Html += dbDtl[i][sys_MapDtl.MobileShowField];
        _Html += ' </div>';
        _Html += '</a>';
        _Html += '</li>';
    }
    _Html += '</ul>';
    _Html += '</div>';


    return _Html;
}

//删除从表数据
function DeleteDtl(dtlNo, oid, obj) {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("FK_MapDtl", dtlNo);
    handler.AddPara("OID", oid);
    handler.DoMethodReturnString("Dtl_DeleteRow");
    //删除成功后，移除数据
    $(obj).parent().parent().remove();
}

//打开明细表
function Dtl_ShowPage(dtlNo, dtlName) {
    $("#frmDtlTitle").html(dtlName);
    $("#HD_CurDtl_No").val(dtlNo)
    Load_DtlInit();
    viewApi.go("#frmDtl");
}
//填充默认数据
function ConvertDefVal(mainTable, defVal, keyOfEn) {

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

    //通过MAINTABLE返回的参数
    for (var ele in mainTable) {
        if (keyOfEn == ele) {
            result = mainTable[ele];
            if (result == null) {
                result = "";
            }
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
            ele.css('font-style', 'italic')
        }
    }
}

function ImgAth(url, athMyPK) {
    var v = window.showModalDialog(url, 'ddf', 'dialogHeight: 650px; dialogWidth: 950px;center: yes; help: no');
    if (v == null)
        return;
    document.getElementById('Img' + athMyPK).setAttribute('src', v);
}

//初始化 IMAGE附件
function figure_Template_ImageAth(frmImageAth) {
    var isEdit = frmImageAth.IsEdit;
    var eleHtml = $("<div></div>");
    var img = $("<img/>");

    var imgSrc = basePath +  "/WF/Data/Img/LogH.PNG";
    //获取数据
    if (frmData.Sys_FrmImgAthDB) {
        $.each(frmData.Sys_FrmImgAthDB, function (i, obj) {
            if (obj.FK_FrmImgAth == frmImageAth.MyPK) {
                imgSrc = obj.FileFullName;
            }
        });
    }
    //设计属性
    img.attr('id', 'Img' + frmImageAth.MyPK).attr('name', 'Img' + frmImageAth.MyPK);
    img.attr("src", imgSrc).attr('onerror', "this.src='../../../Data/Img/LogH.PNG'");
    img.css('width', frmImageAth.W).css('height', frmImageAth.H).css('padding', "0px").css('margin', "0px").css('border-width', "0px");
    //不可编辑
    if (isEdit == "1") {
        var fieldSet = $("<fieldset></fieldset>");
        var length = $("<legend></legend>");
        var a = $("<a></a>");
        var url = basePath + "/WF/CCForm/ImgAth.aspx?W=" + frmImageAth.W + "&H=" + frmImageAth.H + "&FK_MapData=ND" + pageData.FK_Node + "&MyPK=" + pageData.WorkID + "&ImgAth=" + frmImageAth.MyPK;

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

//初始化 附件.
function figure_Template_Attachment(frmAttachment) {
    var eleHtml = '';
    var ath = frmAttachment;
    if (ath.UploadType == 0) {//单附件上传 L4204
        return $('');
    }
    var src = "";
    if (pageData.IsReadonly)
        src = "./CCForm/AttachmentUpload.htm?PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";
    else
        src = "./CCForm/AttachmentUpload.htm?PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK;

    eleHtml += '<div>' + "<iframe style='width:" + ath.W + "px;height:" + ath.H + "px;' ID='Attach_" + ath.MyPK + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', ath.Y).css('left', ath.X).css('width', ath.W).css('height', ath.H);

    return eleHtml;
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
    var href = GetHrefUrl();
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');
    if (frmDtl.DtlShowModel == "0") {
        if (pageData.IsReadOnly) {

            src = "./CCForm/Dtl.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=1&" + urlParam + "&Version=" + load.Version;
        } else {
            src = "./CCForm/Dtl.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=0&" + urlParam + "&Version=" + load.Version;
        }
    }
    else if (frmDtl.DtlShowModel == "1") {
        if (pageData.IsReadOnly)
            src = appPath + "WF/CCForm/DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=1" + strs;
        else
            src = appPath + "WF/CCForm/DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=0" + strs;

    }
    var eleIframe = '<iframe></iframe>';
    eleIframe = $("<iframe class='Fdtl' ID='F" + frmDtl.No + "' src='" + src +
        "' frameborder=0  style='position:absolute;width:" + frmDtl.W + "px; height:" + frmDtl.H +
        "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
    if (pageData.IsReadOnly) {

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
    var y = wf_node.FWC_Y;
    //    var h = wf_node.FWC_H;
    //    var w = wf_node.FWC_W;
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
    paras += '&IsReadonly=1';
    paras += '&csc=1' + Math.random();
    src += "&DoType=View";
    src += "&IsMobile=1";
    src += "&r1=" + Math.random() + paras;

    //暂时修改高度为500px.
    var eleHtml = '<div id="FFWC' + wf_node.NodeID + '">' + "<div style='padding: 2px; width: 100%;'><table id='tbTracks' style='border:1px solid #d6dde6;font-size:14px;padding: 0px; width: 100%;'></table></div>" + '</div>';
    eleHtml = $(eleHtml);

    return eleHtml;
}

//子流程
function figure_Template_FigureSubFlowDtl(wf_node) {
    var sta = wf_node.SFSta;
    var h = wf_node.SF_H;
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
    var eleHtml = '<div id=DIVWC' + wf_node.NodeID + '>' + "<iframe id=FSF" + wf_node.NodeID + " style='width:100%';height:" + h + "px'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);

    return eleHtml;
}

//初始化框架
function figure_Template_IFrame(fram) {
    var eleHtml = '';
    var src = dealWithUrl(fram.src) + "IsReadOnly=0";
    eleHtml = $('<div id="iframe' + fram.MyPK + '">' + '</div>');
    var iframe = $(+"<iframe  style='width:" + fram.W + "px; height:" + fram.H + "'     src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling='no'></iframe>");

    eleHtml.css('position', 'absolute').css('top', fram.Y).css('left', fram.X).css('width', fram.W).css('height', fram.H);
    return frameHtml;
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
                            params[i] = paramArr[0].substring(1) + "=" + frmData.MainTable[0][paramArr[1].substr('@WebUser.'.length)];
                        }
                        if (frmData.MainTable[0][paramArr[1].substr(1)] != undefined) {
                            params[i] = paramArr[0].substring(1) + "=" + frmData.MainTable[0][paramArr[1].substr(1)];
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
                        for (var ele in frmData.MainTable[0]) {
                            if (paramArr[0].substring(1) == ele) {
                                result = frmData.MainTable[0][ele];
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
var jsonStr = '';
var frmData = {};
//从MyFlowFree2017.htm 中拿过过的.
$(function () {

    initPageParam(); //初始化参数

    GenerFrm(); //表单数据.ajax

});

