var FieldTypeS = { Normal: 0, Enum: 1, FK: 2, WinOpen: 3 },
    FormDataType = { AppString: 1, AppInt: 2, AppFloat: 3, AppBoolean: 4, AppDouble: 5, AppDate: 6, AppDateTime: 7, AppMoney: 8, AppRate: 9 },
    UIContralType = { TB: 0, DDL: 1, CheckBok: 2, RadioBtn: 3, MapPin: 4, MicHot: 5 };
//解析表单字段 MapAttr
function InitDtlMapAttr(Sys_MapAttr, groupID, frmData, isReadonly, type, dtlIndx) {
    isReadonly = isReadonly || "0";
    var _html = "";

    $.grep(Sys_MapAttr, function (item) {
        if (type == 'dtl' || type == 'refmethod' || type == 'dtls')
            return item.IsEnableInAPP != 0 && item.UIVisible != 0;
        else
            return item.IsEnableInAPP != 0 && item.UIVisible != 0 && (item.GroupID == groupID || GetPara(item.AtPara, "GroupName") == groupID);

    }).forEach(function (attr) {

        //图片签名
        if (attr.IsSigan == "1") {
            _html += "<div class='mui-input-row'>";
            _html += FormUtils.CreateSignPicture(attr);
            _html += "</div>";
            return;
        }

        //加载其他数据控件
        switch (attr.LGType) {
            case FieldTypeS.Normal: //输出普通类型字段
                if (attr.UIContralType == UIContralType.DDL) {
                    //判断外部数据或WS类型. 
                    _html += "<div class='mui-input-row'>";
                    _html += DtlFormUtils.CreateDDLPK(attr, frmData, isReadonly, type, dtlIndx);
                    break;
                }
                switch (attr.MyDataType) {
                    case FormDataType.AppString:
                        _html += DtlFormUtils.CreateTBString(attr, frmData, isReadonly, type, dtlIndx);
                        break;
                    case FormDataType.AppInt:
                        _html += "<div class='mui-input-row'>";
                        _html += DtlFormUtils.CreateTBInt(attr, frmData, isReadonly, type, dtlIndx);
                        break;
                    case FormDataType.AppFloat:
                    case FormDataType.AppDouble:
                    case FormDataType.AppMoney:
                        _html += "<div class='mui-input-row'>";
                        _html += DtlFormUtils.CreateTBFloat(attr, frmData, isReadonly, type, dtlIndx);
                        break;
                    case FormDataType.AppDate:
                        //日期\boolen型的不允许获取焦点，所以只能禁用
                        _html += "<div class='mui-input-row'>";
                        _html += DtlFormUtils.CreateTBDate(attr, frmData, isReadonly, type, dtlIndx);
                        break;
                    case FormDataType.AppDateTime:
                        //日期\boolen型的不允许获取焦点，所以只能禁用
                        _html += "<div class='mui-input-row'>";
                        _html += DtlFormUtils.CreateTBDateTime(attr, frmData, isReadonly, type, dtlIndx);
                        break;
                    case FormDataType.AppBoolean:
                        //日期\boolen型的不允许获取焦点，所以只能禁用
                        _html += "<div class='mui-input-row'>";
                        _html += DtlFormUtils.CreateCBBoolean(attr, frmData, isReadonly, type, dtlIndx);
                        break;
                }
                break;
            case FieldTypeS.Enum: //枚举值下拉框
                if (attr.Name.length >= 10) {

                    // var ctrl_ID = "RB_" + attr.KeyOfEn;
                    // if (attr.UIContralType == UIContralType.DDL) {
                    var ctrl_ID = "DDL_" + attr.KeyOfEn;
                    if (type == 'dtls')
                        ctrl_ID += "_" + dtlIndex;

                    _html += "<label  for=\"" + ctrl_ID + "\"><p style='padding-left: 5px;'>" + attr.Name + "</p></label>";
                    _html += "<div class='mui-input-row'>";
                    _html += "<select name=\"" + ctrl_ID + "\" id=\"" + ctrl_ID + "\"  " + (attr.UIIsEnable == "0" ? "disabled" : "") + " >";

                    _html += InitDDLOperation1(frmData, attr, "", frmData, isReadonly);
                    _html += "</select>";

                } else {
                    _html += "<div class='mui-input-row'>";
                    _html += DtlFormUtils.CreateDDLEnum(attr, frmData, isReadonly, type, dtlIndx);
                }

                break;
            case FieldTypeS.FK: //外键表下拉框
                if (attr.Name.length >= 10) {
                    _html += DtlFormUtils.CreateDDLPK(attr, frmData, isReadonly, type, dtlIndx);
                } else {
                    _html += "<div class='mui-input-row'>";
                    _html += DtlFormUtils.CreateDDLPK(attr, frmData, isReadonly, type, dtlIndx);
                }

                break;
            case FieldTypeS.WinOpen: //打开系统页面
                _html += "<div class='mui-input-row'>";
                switch (item.UIContralType) {
                    case UIContralType.MapPin: //地图定位
                        _html += DtlFormUtils.CreateMapPin(attr, frmData, isReadonly, type, dtlIndx);
                        break;
                    case UIContralType.MicHot: //语音控件
                        _html += DtlFormUtils.CreateMicHot(attr, frmData, isReadonly, type, dtlIndx);
                        break;
                }
                break;
        }
        _html += "</div>";
    });

    return _html;
}

var DtlFormUtils = {
    CreateSignPicture: function (attr, frmData, isReadonly, type, dtlIndx) {
        //图片签名
        var elementId = "Sign_" + attr.KeyOfEn;
        if (type == "dtls")
            elementId = "Sign_" + dtlIndx + "_" + attr.KeyOfEn;
        var val = ConvertDefVal(frmData, attr.DefVal, attr.KeyOfEn);
        var html_Sign = "<label for=\"" + elementId + "\"><p>" + attr.Name + "</p></label>";
        html_Sign += "<div align=\"left\">";
        html_Sign += "<img name=\"" + elementId + "\" id=\"" + elementId + "\" src='../DataUser/Siganture/" + val + ".jpg' border=0 onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\"/>";
        //        html_Sign += defValue;
        html_Sign += "</div>";
        return html_Sign;
    },
    CreateTBString: function (attr, frmData, isReadonly, type, dtlIndx) {
        var html_string = "";
        var strPlaceholder = "请输入";
        if (attr.UIIsEnable == "0")
            strPlaceholder = "";
        var elementId = "TB_" + attr.KeyOfEn;
        if (type == "dtls")
            elementId = "TB_" + dtlIndx + "_" + attr.KeyOfEn;

        var mustInput = attr.UIIsInput == 1 || (type == "en" && attr.MinLen > 0) ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + attr.KeyOfEn + '" >*</span>' : "";
        //启用二维码
        if (attr.IsEnableQrCode && attr.IsEnableQrCode == "1") {
            html_string += "<div class='mui-input-row'>";
            strPlaceholder = "通过扫一扫添加";
            Form_Ext_Function += "$('#Btn_" + attr.KeyOfEn + "').on('tap', function () { QrCodeToInput('" + elementId + "'); });"
            html_string += "<label for=\"" + elementId + "\">" + attr.Name + "</label>";
            html_string += "<div class=\"QrCodeBar ui-grid-a\">";
            html_string += "  <div class=\"ui-block-a\">";
            html_string += "      <input " + (attr.UIIsEnable == "0" ? "disabled" : "") + " type='text' name=\"" + elementId + "\" id=\"" + elementId + "\" placeholder=\"" + strPlaceholder + "\" />";
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
            html_string += "<label for=\"" + elementId + "\"><p>" + attr.Name + "</p></label>";

            //if (attr.AtPara && attr.AtPara.indexOf("@IsRichText=1") >= 0) {

            //    //如果富文本有数据，就用 div 展示html
            //    //html_string += "<div name='TB_" + attr.KeyOfEn + "' id='TB_" + attr.KeyOfEn + "' style='padding:5px;border:1px solid #d6dde6;font-size: 14px;line-height:22px;'></div>";
            //    html_string += "<textarea wrap='virtual' onpropertychange= 'this.style.posHeight=this.scrollHeight' cols='40' style='font-size:14px;width:100%;border:solid 1px gray;' rows=\"5\" placeholder=\"" + strPlaceholder + "\" name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\"></textarea>";
            //} else {
            //非富文本或者 如果没有数据 就用textarea
            if (attr.UIIsEnable == "0" || isReadonly == "1")
                html_string += "<div name='" + elementId + "' id='" + elementId + "' style='padding:5px;border:1px solid #d6dde6;font-size: 14px;line-height:22px;'></div>";

            else
                html_string += "<textarea wrap='virtual' onpropertychange= 'this.style.posHeight=this.scrollHeight' cols='40' style='font-size:14px;width:100%;border:solid 1px gray;' rows=\"5\" placeholder=\"" + strPlaceholder + "\" name=\"" + elementId + "\" id=\"" + elementId + "\"></textarea>";
            //}
            return html_string;
        }

        //单行文本
        if ((attr.UIIsInput == 1 || (type == "en" && attr.MinLen > 0)) && attr.UIIsEnable == 1) {
            html_string += "<div class='mui-input-row'>";
            html_string += "<label for=\"" + elementId + "\"  class='mustInput' ><p>" + attr.Name + mustInput + "</p></label>";
        } else {
            html_string += "<div class='mui-input-row'>";
            html_string += "<label for=\"" + elementId + "\" ><p>" + attr.Name + mustInput + "</p></label>";
        }

        if (attr.UIIsEnable == "0" || isReadonly == "1") {
            html_string += "<input  style='background-color:#fff ' readonly='readonly' type='text' name='" + elementId + "' id='" + elementId + "' placeholder='" + strPlaceholder + "' />";
        } else {
            html_string += "<input  style='background-color:#fff' type='text' name=\"" + elementId + "\" id=\"" + elementId + "\" placeholder=\"" + strPlaceholder + "\" />";
        }
        return html_string;
    },
    CreateTBInt: function (attr, frmData, isReadonly, type, dtlIndx) {
        var elementId = "TB_" + attr.KeyOfEn;
        if (type == "dtls")
            elementId = "TB_" + dtlIndx + "_" + attr.KeyOfEn;

        var mustInput = attr.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + attr.KeyOfEn + '" >*</span>' : "";

        var inputHtml = "<label style='background-color:#fff' for=\"" + elementId + "\"><p>" + attr.Name + mustInput + "</p></label>";

        if (attr.UIIsEnable == "0" || isReadonly == "1")
            inputHtml += "<input  style='background-color:#fff ' readonly='readonly' type='text' name='" + elementId + "' id='" + elementId + "'/>";

        else {
            inputHtml += "<input style='background-color:#fff' type=\"number\" pattern=\"/^\d+$/\"";
            inputHtml += " name=\"" + elementId + "\" id=\"" + elementId + "\" placeholder='0' />";
        }


        return inputHtml;
    },
    CreateTBFloat: function (attr, frmData, isReadonly, type, dtlIndx) {
        var elementId = "TB_" + attr.KeyOfEn;
        if (type == "dtls")
            elementId = "TB_" + dtlIndx + "_" + attr.KeyOfEn;

        var attrdefVal = attr.DefVal;
        var bit;
        if (attrdefVal != null && attrdefVal !== "" && attrdefVal.indexOf(".") >= 0)
            bit = attrdefVal.substring(attrdefVal.indexOf(".") + 1).length;
        else
            bit = 2;
        var event = "";
        if (attr.UIIsEnable == "1") {
            if (attr.MyDataType == 8)
                event = " onkeyup=\"valitationAfter(this, 'money');limitLength(this," + bit + "); FormatMoney(this, " + bit + ", ',')\"";
            else
                event = " onkeyup=\"valitationAfter(this, 'float');if(isNaN(value)) execCommand('undo');limitLength(this," + bit + ");\"" + " onafterpaste=\"valitationAfter(this, 'float');if(isNaN(value))execCommand('undo');\"";
        }
        var mustInput = attr.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + attr.KeyOfEn + '" >*</span>' : "";
        var inputHtml = "<label style='background-color:#fff' for=\"" + elementId + "\"><p>" + attr.Name + mustInput + "</p></label>";
        if (attr.UIIsEnable == "0" || isReadonly == "1")
            inputHtml += "<input  style='background-color:#fff ' readonly='readonly' type='text' name='" + elementId + "' id='" + elementId + "'/>";
        else
            inputHtml += "<input style = 'backgroud-color:#fff' type =\"number\" " + event + "  name=\"" + elementId + "\" id=\"" + elementId + "\" placeholder=\"0.00\"  />";
        return inputHtml;
    },
    CreateTBDate: function (attr, frmData, isReadonly, type, dtlIndx) {
        var elementId = "TB_" + attr.KeyOfEn;
        if (type == "dtls")
            elementId = "TB_" + dtlIndx + "_" + attr.KeyOfEn;
        var labID = "LAB_" + attr.KeyOfEn;
        if (type == "dtls")
            labID = "LAB_" + dtlIndx + "_" + attr.KeyOfEn;

        var mustInput = attr.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + attr.KeyOfEn + '" >*</span>' : "";
        var inputHtml = "<label for=\"" + elementId + "\"><p>" + attr.Name + mustInput + "</p></label>";
        if (attr.UIIsEnable == "0" || isReadonly == 1) {
            inputHtml += "<input readonly='readonly' type='text' name=\"" + elementId + "\" id=\"" + elementId + "\" />";
        }
        else {
            inputHtml += "<a class='mui-navigate-right'>";
            inputHtml += "  <span name=\"" + labID + "\" id=\"" + labID + "\" data-options='{\"type\":\"date\"}' class='mui-pull-right ccformdate' style='padding-top:10px;'><p>请选择日期</p></span>";
            inputHtml += "</a>";
            inputHtml += "<input  type='hidden' name=\"" + elementId + "\" id=\"" + elementId + "\" />";
        }
        return inputHtml;
    },
    CreateTBDateTime: function (attr, frmData, isReadonly, type, dtlIndx) {
        var elementId = "TB_" + attr.KeyOfEn;
        if (type == "dtls")
            elementId = "TB_" + dtlIndx + "_" + attr.KeyOfEn;
        var labID = "LAB_" + attr.KeyOfEn;
        if (type == "dtls")
            labID = "LAB_" + dtlIndx + "_" + attr.KeyOfEn;

        var mustInput = attr.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + attr.KeyOfEn + '" >*</span>' : "";
        var inputHtml = "<label for=\"" + elementId + "\"><p>" + attr.Name + mustInput + "</p></label>";

        if (attr.UIIsEnable == "0" || isReadonly == 1) {
            inputHtml += "<input name=\"" + elementId + "\" id=\"" + elementId + "\" readonly='readonly' type='text' />";
        }
        else {
            inputHtml += "<a class='mui-navigate-right'>";
            inputHtml += " <label name=\"" + labID + "\" id=\"" + labID + "\" data-options='{\"type\":\"datetime\"}' class='mui-pull-right ccformdate' style='padding-top:10px;'>请选择时间</label>";
            inputHtml += "</a>";
            inputHtml += "<input  type='hidden' name=\"" + elementId + "\" id=\"" + elementId + "\" />";
        }
        return inputHtml;
    },
    CreateCBBoolean: function (attr, frmData, isReadonly, type, dtlIndx) {
        var elementId = "CB_" + attr.KeyOfEn;
        if (type == "dtls")
            elementId = "CB_" + dtlIndx + "_" + attr.KeyOfEn;
        var switchId = "SW_" + attr.KeyOfEn;
        if (type == "dtls")
            switchId = "SW_" + dtlIndx + "_" + attr.KeyOfEn;

        var checkBoxVal = "";
        var keyOfEn = attr.KeyOfEn;
        var CB_Html = "";
        CB_Html += "  <label><p>" + attr.Name + "</p></label>";
        CB_Html += "  <input type='hidden'  id='" + elementId + "' name='" + elementId + "' value='" + attr.DefVal + "'/>";
        if (attr.UIIsEnable == "0" || isReadonly == "1")
            CB_Html += "  <div class='mui-switch mui-switch-blue mui-switch-mini  mui-disabled' id='" + switchId + "'>";
        else
            CB_Html += "  <div class='mui-switch mui-switch-blue mui-switch-mini' id='" + switchId + "'>";
        CB_Html += "      <div class='mui-switch-handle'></div>";
        CB_Html += "  </div>";
        return CB_Html;


        //var checkBoxVal = "";
        //var keyOfEn = attr.KeyOfEn;
        //var CB_Html = "";
        //CB_Html += "  <label><p>" + attr.Name + "</p></label>";
        //CB_Html += "  <input type='hidden' name='CB_" + keyOfEn + "' value='0'/>";
        //CB_Html += "  <div class='mui-switch mui-switch-blue mui-switch-mini mui-active'>";
        //CB_Html += "      <div class='mui-switch-handle'></div>";
        //CB_Html += "  </div>";
        ////CB_Html += "  <input readonly='" + (attr.UIIsEnable == "0" ? "readonly" : "") + "' type=\"checkbox\" name=\"CB_" + keyOfEn + "\" id=\"CB_" + keyOfEn + "\" " + checkBoxVal + " />";
        //return CB_Html;
    },
    CreateDDLEnum: function (attr, frmData, isReadonly, type, dtlIndx) {
        //下拉框和单选都使用下拉框实现
        var mustInput = attr.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + attr.KeyOfEn + '" >*</span>' : "";
        var ctrl_ID = "DDL_" + attr.KeyOfEn;
        if (type == "dtls")
            ctrl_ID = "DDL_" + dtlIndx + "_" + attr.KeyOfEn;
        var html_Select = "<label  for=\"" + ctrl_ID + "\"><p style='padding-left: 5px;'>" + attr.Name + mustInput + "</p></label>";
        html_Select += "<select name=\"" + ctrl_ID + "\" id=\"" + ctrl_ID + "\"  " + (attr.UIIsEnable == "0" || isReadonly == 1 ? "disabled" : "") + " >";

        html_Select += InitDDLOperation1(frmData, attr, "");
        html_Select += "</select>";
        return html_Select;
    },
    CreateDDLPK: function (attr, frmData, isReadonly, type, dtlIndx) {
        var ctrl_ID = "DDL_" + attr.KeyOfEn;
        if (type == "dtls")
            ctrl_ID = "DDL_" + dtlIndx + "_" + attr.KeyOfEn;

        var mustInput = attr.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + mustInput + attr.KeyOfEn + '" >*</span>' : "";
        var html_Select = "<label for=\"" + ctrl_ID + "\"><p>" + attr.Name + mustInput + "</p></label>";
        html_Select += "<select name=\"" + ctrl_ID + "\" id=\"" + ctrl_ID + "\" readonly='" + (attr.UIIsEnable == "0" || isReadonly == 1 ? "readonly" : "") + "'>";

        html_Select += InitDDLOperation1(frmData, attr, "");
        html_Select += "</select>&nbsp;&nbsp;";
        return html_Select;
    },
    CreateMapPin: function (attr, frmData, isReadonly, type, dtlIndx) {
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
    CreateMicHot: function (attr, frmData, isReadonly, type, dtlIndx) {
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
function InitDDLOperation1(frmData, mapAttr, defVal) {

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

