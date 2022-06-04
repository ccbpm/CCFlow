//逻辑类型
var FieldTypeS = { Normal: '0', Enum: '1', FK: '2', WinOpen: '3' };
//数据类型
var FormDataType = { AppString: '1', AppInt: '2', AppFloat: '3', AppBoolean: '4', AppDouble: '5', AppDate: '6', AppDateTime: '7', AppMoney: '8', AppRate: '9' };
//控件类型
var UIContralType = { TB: '0', DDL: '1', CheckBok: '2', RadioBtn: '3', MapPin: '4', MicHot: '5' };
//表单扩展函数
var Form_Ext_Function = "";
//表单整体是否只读
var Form_ReadOnly = false;
//审核组件是否启用
var WorkCheck_Enable = false;
//当前表单
var curFK_MapData = null;

//加载节点表单控件
function GenerFormElement() {
    var args = new RequestArgs();
    var nodeId = args.FK_Node;
    if (nodeId) {
        while (nodeId.substring(0, 1) == '0') {
            nodeId = nodeId.substring(1);
        }
        nodeId = nodeId.replace('#', '');
    }
    curFK_MapData = "ND" + nodeId;
    //只读
    if (args.IsReadonly && args.IsReadonly == "1") {
        Form_ReadOnly = true;
    }
    //只读
    if (args.IsEdit && args.IsEdit == "0") {
        Form_ReadOnly = true;
    }
    AjaxMobileService({
        action: "nodeformelements",
        FK_Flow: args.FK_Flow,
        FK_Node: args.FK_Node,
        WorkID: args.WorkID,
        FID: args.FID,
        CWorkID: args.CWorkID,
        PWorkID: args.PWorkID
    }, function (scope) {
        var pushData = cceval('(' + scope + ')');

        //有错误消息
        if (pushData.error) {
            MsgHidenLoader();
            var _Html = "<button class='ui-btn ui-state-disabled ui-icon-alert ui-btn-icon-top'>" + pushData.error + "</button>";
            //清空面板
            $("#Controls_view").empty();
            //隐藏按钮
            $("#Btn_Send").hide();
            $("#Btn_Save").hide();
            $("#Btn_Return").hide();
            $("#Btn_Other").hide();
            $(_Html).appendTo('#Controls_view');
            //刷新
            $("#Controls_view").trigger("create");
            $("#Controls_view").listview('refresh');
            return;
        }

        //装载控件
        var transControl = new TransControlFromJson(pushData);
        var _html = transControl.To_Html();
        //添加审核组件
        if (pushData.WorkCheck) {
            //_html += "<li data-role=\"list-divider\">" + pushData.WorkCheck.WCText + "</li>";
            _html += WorkCheck_InitHtml();
            //_html += "<li data-icon=\"false\"><a href='#page_workcheck'><h2 style=\"color:#096BC1;font-size: 0.8em;\">点击查看审批详细信息</h2></a></li>";
        }
        //公文文件
        if (pushData.OfficeFile) {
            _html += "<li data-role=\"list-divider\">公文</li>";
            if (pushData.OfficeFile.FileExit == "false") {
                _html += "<li data-icon=\"false\">文件不存在</li>";
            } else {
                _html += "<li data-icon=\"false\"><a href='" + pushData.OfficeFile.FileWebPath + "' target=\"_blank\"><h2 style=\"color:#096BC1;font-size: 0.8em;\">下载公文</h2></a></li>";
            }
        }
        //展显
        $(_html).appendTo('#Controls_view');
        //刷新
        $("#Controls_view").trigger("create");
        $("#Controls_view").listview('refresh');
        //日期控件
        $('input:jqmData(role="datebox")').mobiscroll(optDate);
        $('input:jqmData(role="datetimebox")').mobiscroll(optDateTime);
        //加载自定义脚本
        LoadFormSelfJavaScript(curFK_MapData);
        //执行扩展函数
        if (Form_Ext_Function.length > 0) {
            cceval(Form_Ext_Function);
        }
        //存在扩展设置
        if (pushData.MapExts) {
            CCForm_DealMapExt(pushData.MapExts);
        }
        MsgHidenLoader();
    }, this);
}

//独立表单初始化控件
function Frm_InitControls(FK_MapData) {
    MessageShow("正在加载...", false);
    $("#Controls_view").empty();
    var args = new RequestArgs();
    curFK_MapData = args.FK_MapData;

    var curFormIsEdit = "1";
    Form_ReadOnly = false;
    if (FK_MapData && FK_MapData != "") {
        curFK_MapData = FK_MapData;
    }

    //只读
    if (args.IsReadonly && args.IsReadonly == "1") {
        Form_ReadOnly = true;
        curFormIsEdit = "0";
    }
    //只读
    if (args.IsEdit && args.IsEdit == "0") {
        Form_ReadOnly = true;
        curFormIsEdit = "0";
    }
    AjaxMobileService({
        action: "ccformelements",
        FK_Flow: args.FK_Flow,
        FK_Node: args.FK_Node,
        FK_MapData: curFK_MapData,
        WorkID: args.WorkID,
        FID: args.FID,
        CWorkID: args.CWorkID,
        PWorkID: args.PWorkID,
        IsTest: args.IsTest,
        IsEdit: curFormIsEdit
    }, function (scope) {
        var pushData = cceval('(' + scope + ')');
        //只读
        if (pushData.IsReadOnly == "1") {
            Form_ReadOnly = true;
            curFormIsEdit = "0";
        }
        var transControl = new TransControlFromJson(pushData);
        var _html = transControl.To_Html();
        //展显
        $(_html).appendTo('#Controls_view');
        //刷新
        $("#Controls_view").trigger("create");
        $("#Controls_view").listview('refresh');
        //日期控件
        $('input:jqmData(role="datebox")').mobiscroll(optDate);
        $('input:jqmData(role="datetimebox")').mobiscroll(optDateTime);

        //加载自定义脚本
        LoadFormSelfJavaScript(curFK_MapData);
        //执行扩展函数
        if (Form_Ext_Function.length > 0) {
            cceval(Form_Ext_Function);
        }
        //存在扩展设置
        if (pushData.MapExts) {
            CCForm_DealMapExt(pushData.MapExts);
        }
        MsgHidenLoader();
    }, this);
}

//根据控件原型返回相应控件编码
function TransControlFromJson(controls) {
    this.Form_Controls = controls;
    this.control = null;
    this.Ctrl_Class = "";
    //控件是否可用
    this.Enable = true;
}
//控件属性
TransControlFromJson.prototype = {
    To_Html: function () {
        var _html = "";
        var groupFields = this.Form_Controls.GroupField;
        //数据分组字段
        for (var i = 0, j = groupFields.length; i < j; i++) {
            var groupField = groupFields[i];
            _html += "<li data-icon=\"bars\" data-role=\"list-divider\">" + groupField.Lab + "</li>";
            //数据普通字段    
            var fields = groupField.Fields;
            for (var k = 0, m = fields.length; k < m; k++) {
                this.Ctrl_Class = "";
                this.control = fields[k];
                this.Enable = true;
                //判断控件是否可用
                if (this.control.UIIsEnable == "0" || Form_ReadOnly == true) {
                    this.Enable = false;
                    //this.Ctrl_Class = "readonly = \"readonly\" ";
                    this.Ctrl_Class = "disabled=\"disabled\" ";
                }
                //图片签名
                if (this.control.IsSigan == "1") {
                    _html += "<li class=\"ui-field-contain\">";
                    _html += this.CreateSignPicture();
                    _html += "</li>";
                    continue;
                }
                _html += "<li class=\"ui-field-contain\">";
                //加载其他数据控件
                switch (this.control.LGType) {
                    case FieldTypeS.Normal: //输出普通类型字段
                        if (this.control.UIContralType == UIContralType.DDL) {
                            //判断外部数据或WS类型.
                            if (this.Enable == false) {
                                this.Ctrl_Class = "disabled=\"disabled\" ";
                            }
                            _html += this.CreateDDLPK();
                            break;
                        }
                        switch (this.control.MyDataType) {
                            case FormDataType.AppString:
                                _html += this.CreateTBString();
                                break;
                            case FormDataType.AppInt:
                                _html += this.CreateTBInt();
                                break;
                            case FormDataType.AppFloat:
                            case FormDataType.AppDouble:
                            case FormDataType.AppMoney:
                                _html += this.CreateTBFloat();
                                break;
                            case FormDataType.AppDate:
                                //日期\boolen型的不允许获取焦点，所以只能禁用
                                if (this.Enable == false) {
                                    this.Ctrl_Class = "disabled=\"disabled\" ";
                                }
                                _html += this.CreateTBDate();
                                break;
                            case FormDataType.AppDateTime:
                                //日期\boolen型的不允许获取焦点，所以只能禁用
                                if (this.Enable == false) {
                                    this.Ctrl_Class = "disabled=\"disabled\" ";
                                }
                                _html += this.CreateTBDateTime();
                                break;
                            case FormDataType.AppBoolean:
                                //日期\boolen型的不允许获取焦点，所以只能禁用
                                if (this.Enable == false) {
                                    this.Ctrl_Class = "disabled=\"disabled\" ";
                                }
                                _html += this.CreateCBBoolean();
                                break;
                        }
                        break;
                    case FieldTypeS.Enum: //枚举值下拉框
                        //日期\boolen型的不允许获取焦点，所以只能禁用
                        if (this.Enable == false) {
                            this.Ctrl_Class = "disabled=\"disabled\" ";
                        }
                        _html += this.CreateDDLEnum();
                        break;
                    case FieldTypeS.FK: //外键表下拉框    
                        //日期\boolen型的不允许获取焦点，所以只能禁用
                        if (this.Enable == false) {
                            this.Ctrl_Class = "disabled=\"disabled\" ";
                        }
                        _html += this.CreateDDLPK();
                        break;
                    case FieldTypeS.WinOpen: //打开系统页面
                        switch (this.control.UIContralType) {
                            case UIContralType.MapPin: //地图定位
                                _html += this.CreateMapPin();
                                break;
                            case UIContralType.MicHot: //语音控件
                                _html += this.CreateMicHot();
                                break;
                        }
                        break;
                }
                _html += "</li>";
            }
            //按钮
            if (groupField.FrmBtns && groupField.FrmBtns.length > 0) {
                var btn_css = "style=\"color:#096BC1;font-size: 0.8em;\"";
                for (var iFrmBtn = 0; iFrmBtn < groupField.FrmBtns.length; iFrmBtn++) {
                    var frmBtnID = groupField.FrmBtns[iFrmBtn].MyPK;
                    var frmBtnText = groupField.FrmBtns[iFrmBtn].Text;
                    var frmBtnEventType = groupField.FrmBtns[iFrmBtn].EventType;
                    var frmBtnEventContext = groupField.FrmBtns[iFrmBtn].EventContext;
                    var frmBtnIsEnable = groupField.FrmBtns[iFrmBtn].IsEnable;
                    var disabled = "disabled";
                    if (frmBtnIsEnable == 1) {

                    }
                    //js事件
                    Form_Ext_Function += "$('#Btn_" + frmBtnID + "').on('tap', function () { FrmBtnEventFactory('" + frmBtnID + "','" + frmBtnEventType + "','" + frmBtnEventContext + "'); });"
                    //生成页面
                    _html += "<li data-icon=\"false\">";
                    _html += "   <input " + btn_css + " type=\"button\" id=\"Btn_" + frmBtnID + "\" value=\"" + frmBtnText + "\"/>";
                    _html += "</li>";
                }
            }
            //多附件
            if (groupField.MapAths && groupField.MapAths.length > 0) {
                //_html += "<li data-icon=\"bars\" data-role=\"list-divider\">附件</li>";
                for (var iAth = 0; iAth < groupField.MapAths.length; iAth++) {
                    _html += "<li data-icon=\"false\">";
                    _html += "   <a href='#' onclick='SelectedAthMents(\"" + groupField.MapAths[iAth].MyPK + "\",\"" + groupField.MapAths[iAth].Name + "\")'>";
                    _html += "     <h2 style=\"color:#096BC1;font-size: 0.8em;\">" + groupField.MapAths[iAth].Name;
                    _html += "        <span id='" + groupField.MapAths[iAth].MyPK + "_Count' class=\"athment_filecount\">（" + groupField.MapAths[iAth].AthMentDBs + "）个</span>";
                    _html += "     </h2>";
                    _html += "     <p>点击查看详细</p>";
                    _html += "   </a>";
                    _html += "</li>";
                }
            }
            //明细表
            if (groupField.MapDtls && groupField.MapDtls.length > 0) {
                //_html += "<li data-icon=\"bars\" data-role=\"list-divider\">表格</li>";
                for (var iDtl = 0; iDtl < groupField.MapDtls.length; iDtl++) {
                    _html += "<li data-icon=\"false\">";
                    _html += "   <a href='#' onclick='SelectedDtlNo(\"" + groupField.MapDtls[iDtl].No + "\",\"" + groupField.MapDtls[iDtl].Name + "\")'>";
                    _html += "     <h2 style=\"color:#096BC1;font-size: 0.8em;\">" + groupField.MapDtls[iDtl].Name;
                    _html += "        <span id='" + groupField.MapDtls[iDtl].No + "_Count' class=\"athment_filecount\">（" + groupField.MapDtls[iDtl].Dtl_DBCount + "）条记录</span>";
                    _html += "     </h2>";
                    _html += "     <p>点击查看详细</p>";
                    _html += "   </a>";
                    _html += "</li>";
                }
            }
        }
        return _html;
    },
    CreateSignPicture: function () {
        //图片签名
        var html_Sign = "<label for=\"Sign_" + this.control.KeyOfEn + "\">" + this.control.Name + "</label>";
        html_Sign += "<div align=\"left\">";
        html_Sign += "<img name=\"Sign_" + this.control.KeyOfEn + "\" id=\"Sign_" + this.control.KeyOfEn + "\" src='" + this.control.FieldRelValue + "' border=0 onerror=\"this.src='" + this.control.ImgErrorValue + "'\"/>";
        html_Sign += "</div>";
        return html_Sign;
    },
    CreateTBString: function () {
        var html_string = "";
        var strPlaceholder = "";
        //启用二维码
        if (this.control.IsEnableQrCode && this.control.IsEnableQrCode == "1") {
            strPlaceholder = "通过扫一扫添加";
            Form_Ext_Function += "$('#Btn_" + this.control.KeyOfEn + "').on('tap', function () { QrCodeToInput('TB_" + this.control.KeyOfEn + "'); });"
            html_string = "<label for=\"TB_" + this.control.KeyOfEn + "\">" + this.control.Name + "</label>";
            html_string += "<div class=\"QrCodeBar ui-grid-a\">";
            html_string += "  <div class=\"ui-block-a\">";
            html_string += "      <input " + this.Ctrl_Class + " type=\"text\" name=\"TB_" + this.control.KeyOfEn + "\" id=\"TB_" + this.control.KeyOfEn + "\" placeholder=\"" + strPlaceholder + "\" value=\"" + this.control.FieldRelValue + "\" />";
            html_string += "  </div>";
            html_string += "  <div class=\"ui-block-b\">";
            html_string += "      <div style='margin-top:12px;'>";
            html_string += "         <img id='Btn_" + this.control.KeyOfEn + "' src='image/Field/scanQbar.png' width='29' height='24'/>";
            html_string += "      </div>";
            html_string += "  </div>";
            html_string += "</div>";
            return html_string;
        }
        //多行文本
        if (this.control.UIHeight > 40) {
            html_string = "<label for=\"TB_" + this.control.KeyOfEn + "\">" + this.control.Name + "</label>";
            html_string += "<textarea " + this.Ctrl_Class + " cols=\"40\" rows=\"5\" placeholder=\"" + strPlaceholder + "\" name=\"TB_" + this.control.KeyOfEn + "\" id=\"TB_" + this.control.KeyOfEn + "\">" + this.control.FieldRelValue + "</textarea>";
            return html_string;
        }
        //单行文本
        html_string = "<label for=\"TB_" + this.control.KeyOfEn + "\">" + this.control.Name + "</label>";
        html_string += "<input " + this.Ctrl_Class + " type=\"text\" name=\"TB_" + this.control.KeyOfEn + "\" id=\"TB_" + this.control.KeyOfEn + "\" placeholder=\"" + strPlaceholder + "\" value=\"" + this.control.FieldRelValue + "\" />";
        return html_string;
    },
    CreateTBInt: function () {
        var inputHtml = "<label for=\"TB_" + this.control.KeyOfEn + "\">" + this.control.Name + "</label>";
        inputHtml += "<input " + this.Ctrl_Class + " type=\"number\" pattern=\"[0 - 9] * \"";
        inputHtml += " name=\"TB_" + this.control.KeyOfEn + "\" id=\"TB_" + this.control.KeyOfEn + "\" placeholder=\"0\"";
        inputHtml += " value=\"" + this.control.FieldRelValue + "\" />";
        return inputHtml;
    },
    CreateTBFloat: function () {
        return "<label for=\"TB_" + this.control.KeyOfEn + "\">" + this.control.Name + "</label><input " + this.Ctrl_Class + " type=\"number\" name=\"TB_" + this.control.KeyOfEn + "\" id=\"TB_" + this.control.KeyOfEn + "\" placeholder=\"0.00\" value=\"" + this.control.FieldRelValue + "\" />";
    },
    CreateTBDate: function () {
        var inputHtml = "<label for=\"TB_" + this.control.KeyOfEn + "\">" + this.control.Name + "</label>";
        inputHtml += "<input " + this.Ctrl_Class + " type=\"text\" data-role=\"datebox\" name=\"TB_" + this.control.KeyOfEn + "\" id=\"TB_" + this.control.KeyOfEn + "\" value=\"" + this.control.FieldRelValue + "\" />";
        return inputHtml;
    },
    CreateTBDateTime: function () {
        //Form_Ext_Function += "$('#TB_" + this.control.KeyOfEn + "').datetimepicker({lang:'ch'});";
        var inputHtml = "<label for=\"TB_" + this.control.KeyOfEn + "\">" + this.control.Name + "</label>";
        inputHtml += "<input " + this.Ctrl_Class + " type=\"text\" data-role=\"datetimebox\" ";
        inputHtml += "name=\"TB_" + this.control.KeyOfEn + "\" id=\"TB_" + this.control.KeyOfEn + "\" ";
        inputHtml += "value=\"" + this.control.FieldRelValue + "\" />";
        return inputHtml;
    },
    CreateCBBoolean: function () {
        var checkBoxVal = "";
        var keyOfEn = this.control.KeyOfEn;
        var CB_Html = "<fieldset data-role=\"controlgroup\">";
        if (this.control.FieldRelValue == "1")
            checkBoxVal = "checked='checked'";

        CB_Html += "  <legend>" + this.control.Name + "</legend>";
        CB_Html += "  <label for=\"CB_" + keyOfEn + "\">" + this.control.Name + "</label>";
        CB_Html += "  <input type='hidden' name='CB_" + keyOfEn + "' value='0'/>";
        CB_Html += "  <input " + this.Ctrl_Class + " type=\"checkbox\" name=\"CB_" + keyOfEn + "\" id=\"CB_" + keyOfEn + "\" " + checkBoxVal + " />";
        CB_Html += "</fieldset>";
        return CB_Html;
    },
    CreateDDLEnum: function () {
        var selectedVal = this.control.FieldRelValue;
        //下拉框和单选都使用下拉框实现
        var ctrl_ID = "RB_" + this.control.KeyOfEn;
        if (this.control.UIContralType == UIContralType.DDL) {
            ctrl_ID = "DDL_" + this.control.KeyOfEn;
        }

        var html_Select = "<label for=\"" + ctrl_ID + "\">" + this.control.Name + "</label>";
        html_Select += "<select name=\"" + ctrl_ID + "\" id=\"" + ctrl_ID + "\" " + this.Ctrl_Class + ">";

        //获取枚举数据
        $.ajax({
            type: "POST",
            url: "common/action.ashx",
            dataType: "text", //返回json格式的数据
            async: false,
            cache: false,
            data: {
                action: "formenumdata",
                EnumKey: this.control.UIBindKey
            },
            success: function (scope) {
                var pushData = cceval('(' + scope + ')');
                for (var i = 0; i < pushData.length; i++) {
                    if (selectedVal == pushData[i].IntKey) {
                        html_Select += "<option value=\"" + pushData[i].IntKey + "\" selected='selected'>" + pushData[i].Lab + "</option>";
                    } else {
                        html_Select += "<option value=\"" + pushData[i].IntKey + "\">" + pushData[i].Lab + "</option>";
                    }
                }
            }
        });
        html_Select += "</select>";
        return html_Select;
    },
    CreateDDLPK: function () {
        var args = new RequestArgs();
        var selectedVal = this.control.FieldRelValue;
        var html_Select = "<label for=\"DDL_" + this.control.KeyOfEn + "\">" + this.control.Name + "</label>";
        html_Select += "<select name=\"DDL_" + this.control.KeyOfEn + "\" id=\"DDL_" + this.control.KeyOfEn + "\" " + this.Ctrl_Class + ">";
        var isEnable = this.Enable == true ? 1 : 0;
        var WorkID = args.WorkID;
        //获取外键表数据
        $.ajax({
            type: "POST",
            url: "common/action.ashx",
            dataType: "text", //返回json格式的数据
            async: false,
            cache: false,
            data: {
                action: "formddlpkdata",
                MyPK: this.control.MyPK,
                PKValue: selectedVal,
                WorkID: WorkID,
                IsEnable: isEnable
            },
            success: function (scope) {
                var pushData = cceval('(' + scope + ')');
                for (var i = 0; i < pushData.length; i++) {
                    if (selectedVal == pushData[i].No) {
                        html_Select += "<option value=\"" + pushData[i].No + "\" selected='selected'>" + pushData[i].Name + "</option>";
                    } else {
                        html_Select += "<option value=\"" + pushData[i].No + "\">" + pushData[i].Name + "</option>";
                    }
                }
            }
        });
        html_Select += "</select>";
        return html_Select;
    },
    CreateMapPin: function () {
        var html_MapPin = "<label for=\"TB_" + this.control.KeyOfEn + "\">" + this.control.Name + "</label>";
        //展示内容
        html_MapPin += "<div align=\"left\">";
        if (this.Enable == false) {
            html_MapPin += "<img name=\"MapPin_" + this.control.KeyOfEn + "\" id=\"MapPin_" + this.control.KeyOfEn + "\" src='image/Field/ic_pindisabled.png' border=0  width=\"" + this.control.UIWidth + "\" height=\"" + this.control.UIHeight + "\" align='middle'/>";
        } else {
            html_MapPin += "<img onclick=\"GetMapLocationAddress('" + this.control.KeyOfEn + "')\" name=\"MapPin_" + this.control.KeyOfEn + "\" id=\"MapPin_" + this.control.KeyOfEn + "\" src='image/Field/ic_pin.png' border=0 width=\"" + this.control.UIWidth + "\" height=\"" + this.control.UIHeight + "\" align='middle'/>";
        }
        html_MapPin += "<span onclick=\"OpenMapView('" + this.control.KeyOfEn + "')\" style=\"margin-left:5px;\" name=\"LBL_" + this.control.KeyOfEn + "\" id=\"LBL_" + this.control.KeyOfEn + "\">" + this.control.FieldRelValue + "</span>";
        html_MapPin += "</div>";
        //数据控件
        html_MapPin += "<input type='hidden' name=\"TB_" + this.control.KeyOfEn + "\" id=\"TB_" + this.control.KeyOfEn + "\" value=\"" + this.control.FieldRelValue + "\" />";
        //地图定位
        return html_MapPin;
    },
    CreateMicHot: function () {
        var html_MicHot = "<label for=\"TB_" + this.control.KeyOfEn + "\">" + this.control.Name + "</label>";
        var bDelete = this.Enable;
        //展示内容
        html_MicHot += "<div>";
        if (this.Enable == false) {
            html_MicHot += "<img align=\"left\" name=\"MicHot_" + this.control.KeyOfEn + "\" id=\"MicHot_" + this.control.KeyOfEn + "\" src='image/Field/microphonedisabled.png' border=0  width=\"" + this.control.UIWidth + "\" height=\"" + this.control.UIHeight + "\"/>";
        } else {
            html_MicHot += "<img align=\"left\" onclick=\"StartOpenRecord('" + this.control.KeyOfEn + "')\" name=\"MicHot_" + this.control.KeyOfEn + "\" ";
            html_MicHot += "id=\"MicHot_" + this.control.KeyOfEn + "\" src='image/Field/microphonehot.png' border=0 width=\"" + this.control.UIWidth + "\" height=\"" + this.control.UIHeight + "\"/>";
        }
        html_MicHot += "<img src='image/Field/wx_startplay.gif' align='middle' style='display:none;' />";
        html_MicHot += "<div align=\"left\" style=\"margin-left:15px;float:left;\" name=\"Recorde_" + this.control.KeyOfEn + "\" id=\"Recorde_" + this.control.KeyOfEn + "\"></div>";
        html_MicHot += "</div><br /><br />";
        html_MicHot += "<div id=\"PanelRecords_" + this.control.KeyOfEn + "\">";

        //获取历史语音
        var args = new RequestArgs();
        var keyOfEn = this.control.KeyOfEn;
        $.ajax({
            url: "common/DingDingWebApi.ashx",
            type: 'GET',
            async: false,
            cache: false,
            data: {
                action: "GenerMedias",
                FK_MapData: curFK_MapData,
                FK_Flow: args.FK_Flow,
                FK_Node: args.FK_Node,
                WorkID: args.WorkID,
                FID: args.FID,
                CWorkID: args.CWorkID,
                PWorkID: args.PWorkID,
                KeyOfEn: keyOfEn
            },
            success: function (data) {
                var pushData = cceval("(" + data + ")");
                for (var i = 0; i < pushData.length; i++) {
                    var mediaId = pushData[i].RefPKVal;
                    var duration = pushData[i].Tag2;

                    html_MicHot += "<div class='wx_content' id='Content_" + mediaId.replace("@", "") + "'>";
                    html_MicHot += "  <div class='wx_playSound' id='Record_" + mediaId + "' onclick=\"StartPlayRecord('','" + mediaId + "')\">";
                    html_MicHot += "     <img id='imgSrc_" + mediaId.replace("@", "") + "' src='image/Field/wx_stopplay.png' align='middle' /><div>" + duration + "\"</div>";
                    html_MicHot += " </div>";
                    if (bDelete == true) {
                        html_MicHot += " <div class='float_div' onclick=\"DeleteRecord('" + mediaId + "')\"></div>";
                    }
                    html_MicHot += "</div>";
                }
            },
            error: function (xhr, errorType, error) {
                alert(errorType + ', ' + error);
            }
        });
        //        html_MicHot += " <div class='wx_content'  id='Content_lATOd-J6K85m0tAHzkk616Y'>";
        //        html_MicHot += "  <div class='wx_playSound' id='Record_123' onclick=\"StartPlayRecord('1234','123')\">";
        //        html_MicHot += "     <img id='imgSrc_123' src='image/Field/wx_stopplay.png' align='middle' /><div>12\"</div>";
        //        html_MicHot += "   </div>";
        //        html_MicHot += "   <div class='float_div' onclick=\"DeleteRecord('@lATOd-J6K85m0tAHzkk616Y')\"></div>";
        //        html_MicHot += " </div>";
        html_MicHot += "</div>";
        //语音
        return html_MicHot;
    }
}

//初始化审核组件,主意：async 必须要同步
function WorkCheck_InitHtml() {
    var args = new RequestArgs();
    var _Html = "";
    //获取审核数据
    $.ajax({
        type: "POST",
        url: "common/action.ashx",
        dataType: "text", //返回json格式的数据
        async: false,
        cache: false,
        data: {
            action: "formworkchecktracks",
            FK_Flow: args.FK_Flow,
            FK_Node: args.FK_Node,
            WorkID: args.WorkID,
            FID: args.FID
        },
        success: function (scorp) {
            var pushData = cceval('(' + scorp + ')');
            if (pushData.Msg) {
                _Html = "<li><div style='color: Red;font-size: 0.9em;font-family: Arial;'>" + pushData.Msg + "</div></li>";
            } else {
                //历史审核信息
                var sigantureEnabel = pushData.WorkCheck.SigantureEnabel;
                for (var i = 0; i < pushData.WorkCheck.tracks.length; i++) {
                    var nodeID = pushData.WorkCheck.tracks[i].NodeID;
                    var nodeName = pushData.WorkCheck.tracks[i].NodeName;
                    var msgHtml = pushData.WorkCheck.tracks[i].MsgHtml;
                    var signHtml = pushData.WorkCheck.tracks[i].SigantureHtml;
                    var rdt = pushData.WorkCheck.tracks[i].RDT;

                    _Html += "<li data-role=\"list-divider\">" + nodeName + "</li>";
                    _Html += "<li>";
                    _Html += "<h1>" + msgHtml + "</h1>";
                    if (sigantureEnabel == "true" && signHtml.indexOf("DataUser") > -1) {
                        signHtml = "<img src='" + signHtml + "'/>";
                    }
                    _Html += "<p>签名：<span>" + signHtml + "</span></p>";
                    _Html += "<p>日期：<span>" + rdt + "</span></p>";
                    _Html += "</li>";
                }
                //是否添加审核意见框
                if (pushData.WorkCheck.WCState == "Enable") {
                    WorkCheck_Enable = true;
                    _Html += "<li data-role=\"list-divider\">" + pushData.CurrNode.Name + "</li>";
                    _Html += "<li>";
                    _Html += "<label for=\"WorkCheck_Remark\" style=\"color:#69ca02;font-size: 0.95em;\">填写审核意见:</label>";
                    _Html += "<textarea cols=\"40\" rows=\"5\" name=\"textarea\" id=\"WorkCheck_Remark\">" + pushData.CurrNode.MsgHtml + "</textarea>";
                    _Html += "</li>";
                }
            }
            //如果没有信息则提示无信息
            if (_Html == "") {
                _Html = "<li><button class='ui-btn ui-state-disabled ui-icon-alert ui-btn-icon-top'>记录为空</button></li>";
            }
            return _Html;
        }
    });
    return _Html;
}

//提交审核意见
function WorkCheck_Submit() {
    //如果启用审核意见则执行保存
    if (WorkCheck_Enable == true) {
        var rsVal = $("#WorkCheck_Remark").val();
        if (rsVal == undefined || rsVal == "") {
            MessageShow("请填写审核意见。", true);
            return false;
        }
        //执行发送
        var args = new RequestArgs();
        AjaxMobileService({ action: "saveworkcheck",
            FK_Flow: args.FK_Flow,
            FK_Node: args.FK_Node,
            WorkID: args.WorkID,
            FID: args.FID,
            WorkCheckMsg: rsVal
        }, function (scorp) {
            if (scorp != "true") {
                MessageShow("审核意见保存失败" + scorp, true);
                return false;
            }
        }, this);
    }
    return true;
}

//多附件
function SelectedAthMents(MyPK, name, OID) {
    //中文名
    $("#Header_Ath").html(name);
    //编号
    $("#HD_Ath_MyPK").val(MyPK);
    if (OID) {
        $("#HD_Ath_OID").val(OID);
    } else {
        $("#HD_Ath_OID").val("0");
    }
    $.mobile.changePage($("#page_athment"));
}

//打开明细表
function SelectedDtlNo(dtlNo, name) {
    //明细表中文名
    $("#Header_Dtl").html(name);
    //明细表编号
    $("#HD_CurDtl_No").val(dtlNo);
    $.mobile.changePage($("#page_dtl"));
}

//打开定位地图
function GetMapLocationAddress(keyOfen) {
    if (typeof GetMapLocationCoords != "undefined") {
        //获取当前定位坐标
        GetMapLocationCoords(function (result) {
            if (result.latitude) {
                //定位，打开微调地图，范围放大到2000
                OpenMapSearch(result.latitude, result.longitude, 2000, function (poi) {
                    if (poi.city) {

                        var address = poi.city + poi.adName + poi.snippet;
                        var args = new RequestArgs();
                        $.ajax({
                            type: "GET",
                            url: "common/DingDingWebApi.ashx",
                            dataType: "text", //返回json格式的数据
                            async: false,
                            cache: false,
                            data: {
                                action: "SaveMapCoords",
                                FK_MapData: curFK_MapData,
                                FK_Flow: args.FK_Flow,
                                FK_Node: args.FK_Node,
                                WorkID: args.WorkID,
                                FID: args.FID,
                                CWorkID: args.CWorkID,
                                PWorkID: args.PWorkID,
                                KeyOfEn: keyOfen,
                                latitude: poi.latitude,
                                longitude: poi.longitude,
                                address: address
                            },
                            success: function (scope) {
                                if (scope == "true") {
                                    $("#LBL_" + keyOfen).html(address);
                                    $("#TB_" + keyOfen).val(address);
                                } else {
                                    MessageShow("保存失败" + scope, true);
                                }
                            }
                        });

                    } else {
                        MessageShow(JSON.stringify(poi), true);
                    }
                });
            } else {
                MessageShow("获取定位失败" + JSON.stringify(result), true);
            }
        });
    } else {
        MessageShow("调用高德地图控件失败，请在钉钉打开。", true);
    }
}

//地图显示
function OpenMapView(keyOfen) {
    //打开高德地图
    if (typeof OpenDDBizMapView != "undefined") {
        var args = new RequestArgs();
        $.ajax({
            type: "GET",
            url: "common/DingDingWebApi.ashx",
            dataType: "text", //返回json格式的数据
            async: false,
            cache: false,
            data: {
                action: "openmapbycoords",
                FK_MapData: curFK_MapData,
                FK_Flow: args.FK_Flow,
                FK_Node: args.FK_Node,
                WorkID: args.WorkID,
                FID: args.FID,
                CWorkID: args.CWorkID,
                PWorkID: args.PWorkID,
                KeyOfEn: keyOfen
            },
            success: function (scope) {
                var pushData = cceval('(' + scope + ')');
                if (pushData.address) {
                    OpenDDBizMapView(pushData.latitude, pushData.longitude, pushData.address);
                }
            }
        });
    }
    //打开其他地图

}

//开始录音
function StartOpenRecord(keyOfEn) {
    //配置进度条
    var progressManage = null;
    progressManage = new ProgressManage({
        contentId: 'Recorde_' + keyOfEn,
        totalCount: 60,
        proModel: ProgressModel.sencond,
        timing: 1000,
        clickFun: function () {
            if (ProgressGlo.TimeOut != null) {
                clearTimeout(ProgressGlo.TimeOut);
                ProgressGlo.TimeOut = null;
                if (typeof DingDing_StopRecorder != "undefined") {
                    DingDing_StopRecorder();
                }
                //progressManage.ReStartProgress();
            }
        },
        ComplateFun: function () {
            //progressManage.DoProgress();
        }
    });
    //录音开始
    var StartRecordFun = function () {
        progressManage.DoProgress();
    }
    //录音成功
    var RecordSucessFun = function (downLoadRes, recordRes) {
        //recordRes.mediaId; // 停止播放音频MediaID
        //recordRes.duration; // 返回音频的时长，单位：秒
        //downLoadRes.localAudioId
        //创建语音图标
        if (downLoadRes) {
            var duration = 0;
            if (recordRes.duration) {
                duration = Math.ceil(recordRes.duration);
            }
            var html_MicHot = "<div class='wx_content' id='Content_" + recordRes.mediaId.replace("@", "") + "'>";
            html_MicHot += "  <div class='wx_playSound' id='Record_" + downLoadRes.localAudioId + "' onclick=\"StartPlayRecord('" + downLoadRes.localAudioId + "','" + recordRes.mediaId + "')\">";
            html_MicHot += "     <img id='imgSrc_" + recordRes.mediaId.replace("@", "") + "' src='image/Field/wx_stopplay.png' align='middle' /><div>" + duration + "\"</div>";
            html_MicHot += "  </div>";
            html_MicHot += "  <div class='float_div' onclick=\"DeleteRecord('" + recordRes.mediaId + "')\"></div>";
            html_MicHot += "</div>";
            $(html_MicHot).appendTo("#PanelRecords_" + keyOfEn);
        }
        //移除暂停录音
        progressManage.RemoveLoading();
        if (ProgressGlo.TimeOut != null) {
            clearTimeout(ProgressGlo.TimeOut);
            ProgressGlo.TimeOut = null;
        }
    }
    //录音失败
    var RecordFaildFun = function (err) {
        progressManage.RemoveLoading();
    }

    //启动录音
    if (typeof DingDing_StartRecorder != "undefined") {
        var args = new RequestArgs();
        DingDing_StartRecorder(keyOfEn, StartRecordFun, RecordSucessFun, RecordFaildFun);
    }
}

//开始播放录音
function StartPlayRecord(localAudioId, mediaId) {
    //var src = $("#imgSrc_" + mediaId).attr("src")
    //if (src.indexOf("wx_startplay.gif") > 0) {
    //    if (typeof DingDingStopPaly != undefined) {
    //        $("#imgSrc_" + mediaId).attr("src", "image/Field/wx_stopplay.png");
    //    }
    //    return;
    //}
    //$("#imgSrc_" + mediaId).attr("src", "image/Field/wx_startplay.gif");
    //return;
    //开始播放
    var onPlayStart = function () {
        $("#imgSrc_" + mediaId.replace("@", "")).attr("src", "image/Field/wx_startplay.gif");
    }
    //播放停止
    var onPlayEnd = function () {
        $("#imgSrc_" + mediaId.replace("@", "")).attr("src", "image/Field/wx_stopplay.png");
    }
    //播放出现错误
    var onPlayFaild = function (err) {
        $("#imgSrc_" + mediaId.replace("@", "")).attr("src", "image/Field/wx_stopplay.png");
    }

    if (typeof DingDing_PlayAudio != "undefined") {
        DingDing_PlayAudio(localAudioId, mediaId, onPlayStart, onPlayEnd, onPlayFaild);
    }
}

//删除语音
function DeleteRecord(mediaId) {
    if (confirm("确定要删除所选语音吗？")) {
        var args = new RequestArgs();
        $.ajax({
            type: "GET",
            url: "common/DingDingWebApi.ashx",
            dataType: "text", //返回json格式的数据
            async: false,
            cache: false,
            data: {
                action: "deleterecordes",
                FK_MapData: curFK_MapData,
                FK_Flow: args.FK_Flow,
                FK_Node: args.FK_Node,
                WorkID: args.WorkID,
                FID: args.FID,
                CWorkID: args.CWorkID,
                PWorkID: args.PWorkID,
                mediaId: mediaId
            },
            success: function (scope) {
                $("#Content_" + mediaId.replace("@", "")).remove();
            }
        });
    }
}

//扫描二维码
function QrCodeToInput(ctrlId) {
    DingDing_OpenBarCode(function (scope) {
        $("#" + ctrlId).val(scope.text);
    });
}