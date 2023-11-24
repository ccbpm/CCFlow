var frmData;
var bindDataString = "";
function GenerFrm() {
    var href = GetHrefUrl();
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');
    var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_CCForm");
    handler.AddUrlData(urlParam);
    var data = handler.DoMethodReturnString("FrmGener_Init");
    if (data.indexOf('err@') == 0) {
        alert(data);
        console.log(data);
        return;
    }


    try {
        frmData = JSON.parse(data);
    } catch (err) {
        alert("GenerFrm转换JSON失败:" + data);
        console.log(data);
        return;
    }

   
    //设置标题.
    document.title = frmData.Sys_MapData[0].Name;
    document.getElementById("title").innerHTML = document.title;
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
    

    //分组信息.
    var Sys_GroupFields = frmData.Sys_GroupField;
    var Fk_MapData = frmData.Sys_MapData[0].No;
    var node = frmData.WF_Node && frmData.WF_Node.length > 0 ? frmData.WF_Node[0]:null;
    //遍历循环生成 li
    var mapAttrsHtml = "";
    //加入隐藏字段
    var html = "";
    $.grep(frmData.Sys_MapAttr, function (item) {
        return item.UIVisible == 0;

    }).forEach(function (attr) {
        var defval = ConvertDefVal(frmData, attr.DefVal, attr.KeyOfEn);
        html += "<input type='hidden' id='TB_" + attr.KeyOfEn + "' name='TB_" + attr.KeyOfEn + "' value='" + defval + "' />";


    });
    $('#CCForm').append(html);
    var isZDMobile = $.grep(Sys_GroupFields, function (gf) {
        return gf.IsZDMobile == 1;
    }).length > 0 ? true : false;
    if (isZDMobile == true)
        mapAttrsHtml += ' <ul class="mui-table-view mui-table-view-chevron" style="margin-top:0px">';

    for (var i = 0; i < Sys_GroupFields.length; i++) {

        var gf = Sys_GroupFields[i];

        if (gf.CtrlType == 'FWC' && node.FWCSta == 0) {
            continue;
        }


        if (gf.CtrlType == 'FWC' && node.FWCSta != 0) {
            Skip.addJs("WorkOpt/WorkCheck.js?t=" + Math.random());
            $('#CCForm').append(figure_Template_FigureFrmCheck(frmData));
            getWorkCheck();
        }

        if (gf.CtrlType != 'Ath' && gf.CtrlType != 'FWC') {
            if (isZDMobile == false)
                mapAttrsHtml += "<div class=\"mui-table-view-divider\"><h5 style='color:black;'>" + gf.Lab + "</h5></div>";
            else {
                mapAttrsHtml += "<li class='mui-table-view-cell mui-collapse mui-active'><a class='mui-navigate-right' href='#'>" + gf.Lab + "</a>";
                mapAttrsHtml += "<div class='mui-collapse-content' style='margin-right:-65px'>";
            }
        }


        //附件类的控件.
        if (gf.CtrlType == 'Ath') {

            mapAttrsHtml += InitAth(frmData, gf, isZDMobile);
            if (isZDMobile == true) {
                mapAttrsHtml += "   </div>";
                mapAttrsHtml += "</li>";
            }

            continue;
        }

        //明细表的控件.
        if (gf.CtrlType == 'Dtl') {
            mapAttrsHtml += InitDtl(frmData, gf, isZDMobile);
            if (isZDMobile == true) {
                mapAttrsHtml += "   </div>";
                mapAttrsHtml += "</li>";
            }
            continue;
        }

        //字段类的控件.
        if (gf.CtrlType == '' || gf.CtrlType == null) {
            mapAttrsHtml += InitMapAttr(frmData.Sys_MapAttr, gf.OID, frmData);
            if (isZDMobile == true) {
                mapAttrsHtml += "   </div>";
                mapAttrsHtml += "</li>";
            }
            continue;
        }

    }
    if (isZDMobile == true)
        mapAttrsHtml += "</ul>";
    //展显
    $(mapAttrsHtml).appendTo('#CCForm');

    if (isZDMobile == true) {
        $(".mui-table-view:last-child").css("margin-bottom", "0px");
        $(".mui-table-view:first-child").css("margin-top", "0px");
    }

    //为 DISABLED 的 TEXTAREA 加TITLE 
    var disabledTextAreas = $('#divCCForm textarea:disabled');
    $.each(disabledTextAreas, function (i, obj) {
        $(obj).attr('title', $(obj).val());
    })

    mui(".mui-switch").switch();
    //监听开关事件
    var SW = $('.mui-switch');
    $.each(SW, function (i, obj) {
        var KeyOfEn = $(obj).attr("id");

        document.getElementById(KeyOfEn).addEventListener("toggle", function (event) {
            KeyOfEn = KeyOfEn.substring(3);
            if (event.detail.isActive) {
                $("#TB_" + KeyOfEn).val("1");
            } else {
                $("#TB_" + KeyOfEn).val("0");
            }
        })
    })

    //根据NAME 设置ID的值
    var inputs = $('[name]');
    $.each(inputs, function (i, obj) {
        if ($(obj).attr("id") == undefined || $(obj).attr("id") == '') {
            $(obj).attr("id", $(obj).attr("name"));
        }
    });

    var enName = frmData.Sys_MapData[0].No;

    try {
        var src = "../../DataUser/JSLibData/" + enName + "_Self.js";
        loadScript(src);
        src = "../../DataUser/JSLibData/" + enName + ".js";
        loadScript(src);
        if (frmData.Sys_FrmAttachment.length > 0) {
            src = "../js/mui/js/feedback-page.js";
            loadScript(src);
        }
        if (frmData.Sys_FrmImgAth.length > 0) {
            src = "../js/mui/js/feedback-page.js";
            loadScript(src);
        }
    }
    catch (err) {
    }

    //根据下拉框、单选按钮的选择设置显示不显示、默认值
    //ShowNoticeInputInfo();

    //显示tb 提示信息.
    // ShowTbNoticeInfo();

    //初始化复选下拉框
    var selectPicker = $('.selectpicker');
    $.each(selectPicker, function (i, selectObj) {
        var defVal = $(selectObj).attr('data-val');
        var defValArr = defVal.split(',');
        $(selectObj).selectpicker('val', defValArr);
    });
    LoadFrmDataAndChangeEleStyle(frmData);
    //处理下拉框级联等扩展信息
    AfterBindEn_DealMapExt(frmData);
    if (pageData.IsReadonly != 1) {
        var pickdates = $(".ccformdate");
        pickdates.each(function (i, pickdate) {
            var id = this.getAttribute('id');
            if ($("#" + id).html() == '') {
                $("#" + id).html("<p style='margin-bottom:0px;'>请选择时间</p>");
            }
            var funcionPK = "";
            if ($('#TB_' + id.substr(4)).length != 0) {
                funcionPK = $('#TB_' + id.substr(4)).attr("data-funcionPK");
            }

            if (bindDataString.indexOf(id.replace('LAB_', '')) == -1) {
                pickdate.addEventListener('tap', function () {
                    var _self = this;
                    var optionsJson = this.getAttribute('data-options') || '{}';

                    var options = JSON.parse(optionsJson);
                    if (options.type == 0) {
                        options.type = "date";//yyyy-MM-dd
                    } else if (options.type == 1) {
                        options.type = "date-time";//yyyy-MM-dd HH:mm
                    } else if (options.type == 2) {
                        options.type = "datetime";//yyyy-MM-dd HH:mm:ss
                    } else if (options.type == 3) {
                        options.type = "month";//yyyy-MM
                    } else if (options.type == 4) {
                        options.type = "time-min";//HH:mm
                    } else if (options.type == 5) {
                        options.type = "time";//HH:mm:ss
                    } else if (options.type == 6) {
                        options.type = "month-day";//MM-dd
                    } else if (options.type == 7) {
                        options.type = "year";//yyyy
                    }
                    var id = this.getAttribute('id');
                    /*
                        * 首次显示时实例化组件
                        * 示例为了简洁，将 options 放在了按钮的 dom 上
                        * 也可以直接通过代码声明 optinos 用于实例化 DtPicker
                        */
                    _self.picker = new mui.DtPicker(options);
                    _self.picker.show(function (rs) {
                        /*
                            * rs.value 拼合后的 value
                            * rs.text 拼合后的 text
                            * rs.y 年，可以通过 rs.y.vaue 和 rs.y.text 获取值和文本
                            * rs.m 月，用法同年
                            * rs.d 日，用法同年
                            * rs.h 时，用法同年
                            * rs.i 分（minutes 的第二个字母），用法同年
                            */

                        /* 
                            * 返回 false 可以阻止选择框的关闭
                            * return false;
                            */
                        /*
                            * 释放组件资源，释放后将将不能再操作组件
                            * 通常情况下，不需要示放组件，new DtPicker(options) 后，可以一直使用。
                            * 当前示例，因为内容较多，如不进行资原释放，在某些设备上会较慢。
                            * 所以每次用完便立即调用 dispose 进行释放，下次用时再创建新实例。
                            */
                        $("#" + id).html(rs.text);
                        $("#TB_" + id.substr(4)).val(rs.text);
                        _self.picker.dispose();
                        _self.picker = null;
                        if (funcionPK != null && funcionPK != "") {
                            var bindFunctionExt = new Entity("BP.Sys.MapExt", funcionPK);
                            doc = bindFunctionExt.Doc;
                            if (doc != null && doc != "") {
                                DBAccess.RunFunctionReturnStr(doc);
                            }
                        }
                    });
                }, false);
            }

        });
    }
    $.each($(".mui-attr-btn"), function (idx, item) {
        $(this).on("tap", function () {
            var keyOfEn = item.id.substring(4);
            var mapAttr = $.grep(frmData.Sys_MapAttr, function (attr) {
                return attr.KeyOfEn == keyOfEn;
            })[0];
            var tag = mapAttr.Tag||"";
            if (tag != "")
                tag = DealExp(tag);
            if (mapAttr.UIIsEnable == 1) {
                //执行js
                DBAccess.RunUrlReturnString(tag);
            }
            if (mapAttr.UIIsEnable == 2)
                DBAccess.RunFunctionReturnStr(tag);
            if (mapAttr.UIIsEnable != 0 && pageData.IsReadonly!=1)
                FullIt("", mapAttr.MyPK + "_FullData", "Btn_" + mapAttr.KeyOfEn, 0);
        })
        
    })
   
}

    
function Change() {
    var btn = document.getElementById('Btn_Save');
    if (btn != null) {
        if (btn.value.valueOf('*') == -1)
            btn.value = btn.value + '*';
    }
}
  


//以下是软通写的
//初始化网页URL参数
function initPageParam() {
    //新建独有
    pageData.UserNo = GetQueryString("UserNo");
    pageData.DoWhat = GetQueryString("DoWhat");
    pageData.IsMobile = GetQueryString("IsMobile");

    pageData.FK_Flow = GetQueryString("FK_Flow");
    pageData.FK_Node = GetQueryString("FK_Node");
    pageData.FID = GetQueryString("FID") == null ? 0 : GetQueryString("FID");
    pageData.WorkID = GetQueryString("WorkID");
    pageData.IsRead = GetQueryString("IsRead");
    pageData.T = GetQueryString("T");
    pageData.Paras = GetQueryString("Paras");
    pageData.IsReadonly = GetQueryString("IsReadOnly"); //如果是IsReadOnly，就表示是查看页面，不是处理页面
    pageData.IsStartFlow = GetQueryString("IsStartFlow"); //是否是启动流程页面 即发起流程

    pageData.DoType1 = GetQueryString("DoType")//View
}

//将获取过来的URL参数转成URL中的参数形式  &
function pageParamToUrl() {
    var paramUrlStr = '';
    for (var param in pageData) {
        paramUrlStr += '&' + (param.indexOf('@') == 0 ? param.substring(1) : param) + '=' + pageData[param];
    }
    return paramUrlStr;
} 
//设置附件为只读
function setAttachDisabled() {
    //附件设置
    var attachs = $('iframe[src*="AttachmentUpload.htm"]');
    $.each(attachs, function (i, attach) {
        if (attach.src.indexOf('IsReadOnly') == -1) {
            $(attach).attr('src', $(attach).attr('src') + "&IsReadOnly=1");
        }
    })
}
 
//设置表单元素不可用
function setFormEleDisabled() {
    //文本框等设置为不可用
    $('#divCCForm textarea').attr('disabled', 'disabled');
    $('#divCCForm select').attr('disabled', 'disabled');
    $('#divCCForm input[type!=button]').attr('disabled', 'disabled');
}

//初始化附件内容.
function InitAth(workNodeData, gf) {

    var athDBs = workNodeData.Sys_FrmAttachmentDB;
    if (athDBs.length == 0)
        return "无";

    var html = "<ul>";
    $.each(athDBs, function (i, obj) {
        html += "<li><a href='./CCForm/AttachmentUpload.aspx?DoType=Down&MyPK=" + obj.MyPK + "' target=_blank > <img src='./Img/FileType/" + obj.FileExts + ".gif' onerror=\"src='./Img/FileType/tmp.gif'\" border=0 />" + obj.FileName + "</li>";
    })
    html += "</ul>";
    return html;
}

//明细表
function InitDtl(frmData, gf, isZDMobile) {
    var dtlHtml = "";
    if (isZDMobile == false) {

    }
    //mapAttrsHtml += "<div class=\"mui-table-view-divider\"><h5 style='color:black;'>" + gf.Lab + "</h5></div>";
    else {
        dtlHtml += "<li class='mui-table-view-cell mui-collapse mui-active'><a class='mui-navigate-right' href='#'>" + gf.Lab + "</a>";
        dtlHtml += "<div class='mui-collapse-content' style='margin-right:-65px'>";
    }


    if (frmData.Sys_MapDtl) {
        $.each(frmData.Sys_MapDtl, function (i, dtl) {
            loadScript("../DataUser/JSLibData/" + dtl.No + "_Self.js");
            if (gf.CtrlID == dtl.No) {
                if (dtl.ListShowModel == "2") {
                    if (dtl.UrlDtl == null || dtl.UrlDtl == undefined || dtl.UrlDtl == "") {
                        dtlHtml += "<div class='mui-table-view-cell' name='Dtl' id=Dtl_'" + dtl.No + "'>";
                        dtlHtml += "从表" + dtl.Name + "没有设置URL,请在" + dtl.FK_MapData + "_Self.js中解析";
                        dtlHtml += "</div>";
                        return dtlHtml;
                    }

                    var func = "TurnTo_Dtl(\"" + dtl.No + "\",\"" + dtl.UrlDtl + "\",\"" + dtl.FK_MapData + "\")";
                    dtlHtml += "<div style='background-color:#efeff4' class='mui-table-view-cell' name='Dtl' id='" + dtl.No + "'>";
                    dtlHtml += "	<a  class='mui-navigate-right' data-title-type='native' href='javascript:" + func + "'><h5>"
                        + "<span id='" + dtl.No + "_Count'></span></h5>";
                    dtlHtml += "		<p style='display:inline;float:left'>" + gf.Lab + "</p><p style='display:inline;float:right;margin-right:20px'>点击查看详情</p>";
                    dtlHtml += "	</a>";
                    dtlHtml += "</div>";
                    return;
                }
                if (dtl.MobileShowModel == undefined || dtl.MobileShowModel == 0) {
                    var func = "Dtl_ShowPage(\"" + dtl.No + "\",\"" + dtl.Name + "\")";
                    dtlHtml += "<div style='background-color:#efeff4' class='mui-table-view-cell' name='Dtl' id='" + dtl.No + "'>";
                    dtlHtml += "	<a  class='mui-navigate-right' data-title-type='native' href='javascript:" + func + "'><h5>"
                        + "<span id='" + dtl.No + "_Count'></span></h5>";
                    dtlHtml += "		<p style='display:inline;float:left'>" + gf.Lab + "</p><p style='display:inline;float:right;margin-right:20px'>点击查看详情</p>";
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

window.onresize = function () {
    if (pageData.Col == 8) {
        if (jsonStr != undefined && jsonStr != '') {
            var workNodeData = JSON.parse(jsonStr);
            //设置CCFORM的表格宽度  
            if (document.body.clientWidth > 992) {//处于中屏时设置宽度最小值
                $('#CCForm').css('min-width', workNodeData.Sys_MapData[0].TableWidth);
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

//逻辑类型、数据类型、控件类型
var FieldTypeS = { Normal: 0, Enum: 1, FK: 2, WinOpen: 3 },
    FormDataType = { AppString: 1, AppInt: 2, AppFloat: 3, AppBoolean: 4, AppDouble: 5, AppDate: 6, AppDateTime: 7, AppMoney: 8, AppRate: 9 },
    UIContralType = { TB: 0, DDL: 1, CheckBok: 2, RadioBtn: 3, MapPin: 4, MicHot: 5 };

//解析表单字段 MapAttr
function InitMapAttr(Sys_MapAttr, groupID) {

    var _html = "";

    $.grep(Sys_MapAttr, function (item) {
        return ((item.IsEnableInAPP != 0 && item.UIVisible != 0)) && item.GroupID == groupID;

    }).forEach(function (attr) {

        //图片签名
        if (attr.IsSigan == "1") {
            _html += "<div class='mui-input-row'>";
            _html += FormUtils.CreateSignPicture(attr);
            _html += "</div>";
            return;
        }
        if (attr.UIContralType == 16 && attr.UIIsEnable == "1")
            return;

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
                        _html += "<div class='mui-input-row' style='min-height:37px'>";
                        _html += FormUtils.CreateTBInt(attr);
                        break;
                    case FormDataType.AppFloat:
                    case FormDataType.AppDouble:
                    case FormDataType.AppMoney:
                        _html += "<div class='mui-input-row'style='min-height:37px'>";
                        _html += FormUtils.CreateTBFloat(attr);
                        break;
                    case FormDataType.AppDate:
                        //日期\boolen型的不允许获取焦点，所以只能禁用
                        _html += "<div class='mui-input-row' style='min-height:37px'>";
                        _html += FormUtils.CreateTBDate(attr);
                        break;
                    case FormDataType.AppDateTime:
                        //日期\boolen型的不允许获取焦点，所以只能禁用
                        _html += "<div class='mui-input-row' style='min-height:37px'>";
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

                    var ctrl_ID = "DDL_" + attr.KeyOfEn;

                    _html += "<div class='mui-input-row' style='height: auto;'>";
                    var mustInput = attr.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + attr.KeyOfEn + '" >*</span>' : "";
                    _html += "<label style = 'margin:0px;' for=\"" + ctrl_ID + "\" style='width: 35%;'><p>" + attr.Name + mustInput + "</p></label>";
                    _html += "<select  style='margin: auto;position: absolute;top: 0;bottom: 0;' name=\"" + ctrl_ID + "\" id=\"" + ctrl_ID + "\"  " + (attr.UIIsEnable == "0" ? "disabled" : "") + "onchange='changeEnable(this,\"" + attr.FK_MapData + "\",\"" + attr.KeyOfEn + "\",\"" + attr.AtPara + "\")'" + " >";

                    _html += InitDDLOperation(frmData, attr, "");
                    _html += "</select>";

                } else {
                    _html += "<div class='mui-input-row' style='height: auto;'>";
                    _html += FormUtils.CreateDDLEnum(attr);
                }

                break;
            case FieldTypeS.FK: //外键表下拉框
                if (attr.Name.length >= 10) {
                    _html += FormUtils.CreateDDLPK(attr);
                } else {
                    _html += "<div class='mui-input-row'>";
                    _html += FormUtils.CreateDDLPK(attr);
                }

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

        var val = ConvertDefVal(frmData, attr.DefVal, attr.KeyOfEn);
        var html_Sign = "<label for=\"Sign_" + attr.KeyOfEn + "\"><p>" + attr.Name + "</p></label>";
        html_Sign += "<div align=\"left\">";
        if (webUser.CCBPMRunModel == 2)
            html_Sign += "<img name=\"Sign_" + attr.KeyOfEn + "\" id=\"Sign_" + attr.KeyOfEn + "\" src='../DataUser/Siganture/" + webUser.OgNo + "/" + val + ".jpg' border=0  style='height:40px;' onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\"/>";
        else
            html_Sign += "<img name=\"Sign_" + attr.KeyOfEn + "\" id=\"Sign_" + attr.KeyOfEn + "\" src='../DataUser/Siganture/" + val + ".jpg' border=0  style='height:40px;' onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\"/>";

        html_Sign += "</div>";
        return html_Sign;
    },
    CreateTBString: function (attr) {
        var html_string = "";
        if (attr.Tip == "")
            attr.Tip = "请输入";

        if (attr.UIIsEnable == "0")
            strPlaceholder = "";
        var mustInput = attr.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + attr.KeyOfEn + '" >*</span>' : "";
        //启用二维码
        if (attr.IsEnableQrCode && attr.IsEnableQrCode == "1") {
            html_string += "<div class='mui-input-row'>";
            strPlaceholder = "通过扫一扫添加";
            Form_Ext_Function += "$('#Btn_" + attr.KeyOfEn + "').on('tap', function () { QrCodeToInput('TB_" + attr.KeyOfEn + "'); });"
            html_string += "<label for=\"TB_" + attr.KeyOfEn + "\">" + attr.Name + "</label>";
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
                str = "";
            html_string += str;
            html_string += "</div>";
            return html_string;
        }
        if (attr.UIContralType == 16 && attr.UIIsEnable == "0") {
            loadScript("http://res.wx.qq.com/open/js/jweixin-1.2.0.js?Version=" + Math.random());
            html_string += "<div class='mui-input-row'>";
            html_string += "<button type='button' class='mui-btn' id='TB_fixed'style='margin-right:20px;width:35%' onclick='GetFixedInfoByJDWD(\"" + frmData.MainTable[0].JD + "\",\"" + frmData.MainTable[0].WD + "\")'>显示位置</button>";
            html_string += "</div>";
            return html_string;
        }
        if (attr.UIContralType == 18) {
            //按钮操作
            html_string += "<div class='mui-input-row'>";
            html_string += "<button type='button' style='margin:3px' class='mui-btn mui-btn-primary mui-attr-btn' id='Btn_"+attr.KeyOfEn+"'>" + attr.Name + "</button>";
            html_string += "</div>";
            return html_string;
        }

        //多行文本
        if (attr.UIHeight > 30 && attr.ColSpan > 1) {
            html_string += "<div class='' style='padding:11px 15px;line-height:1.1;'>";
            html_string += "<label for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + mustInput + "</p></label>";
            if (attr.UIIsEnable == "0")
                html_string += "<div name='TB_" + attr.KeyOfEn + "' id='TB_" + attr.KeyOfEn + "' style='padding:5px;border:1px solid #d6dde6;font-size: 14px;line-height:22px;'></div>";
            else
                html_string += "<textarea wrap='virtual' onpropertychange= 'this.style.posHeight=this.scrollHeight' cols='40' style='overflow:visible;font-size:14px;width:100%;border:solid 1px gray;' rows=\"5\" placeholder=\"" + attr.Tip + "\"  onfocus =\"this.placeholder = ''\" onblur=\"this.placeholder = '" + attr.Tip + "'\" name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\"></textarea>";
            return html_string;
        }

        

        //单行文本
        if (attr.UIIsInput == 1 && attr.UIIsEnable == 1 && attr.UIContralType != 12) {
            html_string += "<div class='mui-input-row' style='min-height:37px;'>";
            html_string += "<label style='margin:0px;' for=\"TB_" + attr.KeyOfEn + "\"  class='mustInput' ><p>" + attr.Name + mustInput + "</p></label>";
        } else if (attr.UIIsEnable == 1 && attr.UIContralType != 12) {
            html_string += "<div class='mui-input-row' style='min-height:37px;'>";
            html_string += "<label style='margin:0px;' for=\"TB_" + attr.KeyOfEn + "\" ><p>" + attr.Name + "</p></label>";
        } else if (attr.UIIsEnable == "0" && attr.UIContralType != 12) {
            html_string += "<div class='mui-input-row' style='min-height:37px;'>";
            html_string += "<label style='margin:0px;' for=\"TB_" + attr.KeyOfEn + "\" style='margin: 0px;'><p>" + attr.Name + "</p></label>";
        }

        //身份证件解析增加一个
        if (attr.UIContralType == 13 && attr.KeyOfEn == "IDCardName") {
            html_string += "<div style='float:right'><div  style='background-color:#fff;font-size:14px;float:left;display:inline;padding:10px 15px;'  name='TB_" + attr.KeyOfEn + "' id='TB_" + attr.KeyOfEn + "'></div><label class='image-local'><img src='./js/mui/images/vcard.png' style='width:35px'/><input type='file' accept='image/png,image/bmp,image/jpg,image/jpeg' onchange='GetIDCardInfo(this)' style='display: none;'/></label></div>";
        } else if (attr.UIContralType == 12) {
            html_string += "<div class=\"mui-table-view-divider\"><h5 style='color:black;'>" + attr.Name + "</h5></div>";
            html_string += InitEleAth(frmData, attr.Name, attr.FK_MapData, attr.KeyOfEn);
        } else if (attr.UIIsEnable == "0") {
            html_string += "<input  style='background-color:#fff;margin: auto;position: absolute;top: 0;bottom: 0;height:auto;' type='text' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder=\"" + strPlaceholder + "\" />";
        } else {
            html_string += "<input  style='background-color:#fff;margin: auto;position: absolute;top: 0;bottom: 0;height:auto;' type='text' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder=\"" + attr.Tip + "\" onfocus=\"this.placeholder = ''\" onblur=\"this.placeholder = '" + attr.Tip + "'\" />";
        }

        return html_string;
    },
    CreateTBInt: function (attr) {
        var mustInput = attr.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + attr.KeyOfEn + '" >*</span>' : "";

        var inputHtml = "<label style='background-color:#fff;margin:0px;' for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + mustInput + "</p></label>";

        inputHtml += "<input style='margin: auto;position: absolute;top: 0;bottom: 0;' style='background-color:#fff' type=\"number\" pattern=\"/^\d+$/\"";
        if (attr.DefValType == 0)
            inputHtml += " name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" />";
        else
            inputHtml += " name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder='0' />";

        return inputHtml;
    },
    CreateTBFloat: function (attr) {
        var attrdefVal = attr.DefVal;
        var bit;
        if (attrdefVal != null && attrdefVal !== "" && attrdefVal.indexOf(".") >= 0)
            bit = attrdefVal.substring(attrdefVal.indexOf(".") + 1).length;
        else
            bit = 2;
        var event = "";
        if (attr.UIIsEnable == "1" && pageData.IsReadonly != 1) {
            if (attr.MyDataType == 8)
                event = " onkeyup=\"valitationAfter(this, 'money');limitLength(this," + bit + "); FormatMoney(this, " + bit + ", ',')\"";
            else
                event = " onkeyup=\"valitationAfter(this, 'float');if(isNaN(value)) execCommand('undo');limitLength(this," + bit + ");\"" + " onafterpaste=\"valitationAfter(this, 'float');if(isNaN(value))execCommand('undo');\"";
        }
        var mustInput = attr.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + attr.KeyOfEn + '" >*</span>' : "";
        var inputHtml = "<label style='background-color: #fff; margin: 0px;' for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + mustInput + "</p></label>";
        if (attr.UIIsEnable == "1" && pageData.IsReadonly != 1) {
            var step = GetPara(attr.AtPara, "NumStepLength");
            step = step == null || step == undefined ? 0.1 : parseFloat(step);
            var minNum = GetPara(attr.AtPara, "NumMin") || "";
            var maxNum = GetPara(attr.AtPara, "NumMax") || "";
            var dataInfo = "";
            if (minNum != "")
                dataInfo = " data-numbox-min='" + minNum + "'";
            if (maxNum != "")
                dataInfo += " data-numbox-max='" + maxNum + "'";

            inputHtml += "<div class='mui-numbox' data-numbox-step='" + step + "' data-numbox-bit='" + bit + "'" + dataInfo + " style='width:200px'>";
            inputHtml += "<button class='mui-btn mui-btn-numbox-minus' type='button'>-</button>";
        }
        if (attr.DefValType == 0)
            inputHtml += "<input style='backgroud-color:#fff' class='mui-numbox-input' type=\"number\" " + event + "  name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" />";
        else
            inputHtml += "<input style='backgroud-color:#fff' class='mui-numbox-input' type=\"number\" " + event + "  name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder=\"0.00\" />";
        if (attr.UIIsEnable == "1" && pageData.IsReadonly != 1) {
            inputHtml += "<button class='mui-btn mui-btn-numbox-plus' type='button'>+</button>";
            inputHtml += "</div>";
        }

        return inputHtml;
    },
    CreateTBDate: function (attr) {
        var mustInput = attr.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + attr.KeyOfEn + '" >*</span>' : "";
        var inputHtml = "<label style='margin:0px;' for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + mustInput + "</p></label>";
        if (attr.UIIsEnable == "0") {
            inputHtml += "<input readonly='readonly' type='text' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" />";
        }
        else {
            inputHtml += "<a class='mui-navigate-right'>";
            inputHtml += "  <label style='margin-bottom:0px;' name=\"LAB_" + attr.KeyOfEn + "\" id=\"LAB_" + attr.KeyOfEn + "\" data-options='{\"type\":" + attr.IsSupperText + ",\"beginYear\":\"1960\"}' class='mui-pull-right ccformdate' style='padding-top:10px;width:50%'><p>请选择日期</p></label>";
            inputHtml += "</a>";
            inputHtml += "<input type='hidden' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" />";
        }
        return inputHtml;
    },
    CreateTBDateTime: function (attr) {
        var mustInput = attr.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + attr.KeyOfEn + '" >*</span>' : "";
        var inputHtml = "<label  for=\"TB_" + attr.KeyOfEn + "\" style = 'margin:0px;'><p>" + attr.Name + mustInput + "</p></label>";

        if (attr.UIIsEnable == "0") {
            inputHtml += "<input name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" readonly='readonly' type='text' />";
        }
        else {
            inputHtml += "<a class='mui-navigate-right'>";
            inputHtml += " <label style='margin-bottom:0px;width:50%;' name=\"LAB_" + attr.KeyOfEn + "\" id=\"LAB_" + attr.KeyOfEn + "\" data-options='{\"type\":" + attr.IsSupperText + ",\"beginYear\":\"1960\"}' class='mui-pull-right ccformdate' style='padding-top:10px;width:50%'><p style='margin-bottom: 0px;'>请选择时间</p></label>";
            inputHtml += "</a>";
            inputHtml += "<input style='margin: auto;position: absolute;top: 0;bottom: 0;' type='hidden' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" />";
        }
        return inputHtml;
    },
    CreateCBBoolean: function (attr) {
        var checkBoxVal = "";
        var keyOfEn = attr.KeyOfEn;
        var CB_Html = "";
        CB_Html += "  <label><p>" + attr.Name + "</p></label>";
        CB_Html += "  <input type='hidden'  id='CB_" + keyOfEn + "' name='CB_" + keyOfEn + "' value='" + attr.DefVal + "'/>";
        if (attr.UIIsEnable == "0")
            CB_Html += "  <div class='mui-switch mui-switch-blue mui-switch-mini  mui-disabled' id='SW_" + attr.KeyOfEn + "'>";
        else
            CB_Html += "  <div class='mui-switch mui-switch-blue mui-switch-mini' id='SW_" + attr.KeyOfEn + "'>";

        CB_Html += "      <div class='mui-switch-handle'></div>";
        CB_Html += "  </div>";

        //CB_Html += "  <input readonly='" + (attr.UIIsEnable == "0" ? "readonly" : "") + "' type=\"checkbox\" name=\"CB_" + keyOfEn + "\" id=\"CB_" + keyOfEn + "\" " + checkBoxVal + " />";
        return CB_Html;
    },
    CreateDDLEnum: function (attr) {
        //下拉框和单选都使用下拉框实现
        var mustInput = attr.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + attr.KeyOfEn + '" >*</span>' : "";
        var ctrl_ID = "DDL_" + attr.KeyOfEn;

        var html_Select = "<label style = 'margin:0px;'  for=\"" + ctrl_ID + "\"><p>" + attr.Name + mustInput + "</p></label>";
        html_Select += "<select name=\"" + ctrl_ID + "\" id=\"" + ctrl_ID + "\"  " + (attr.UIIsEnable == "0" ? "disabled" : "") + " onchange='changeEnable(this,\"" + attr.FK_MapData + "\",\"" + attr.KeyOfEn + "\",\"" + attr.AtPara + "\")'>";

        html_Select += InitDDLOperation(frmData, attr, "");
        html_Select += "</select>";
        return html_Select;
    },
    CreateDDLPK: function (attr) {
        var mustInput = attr.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + mustInput + attr.KeyOfEn + '" >*</span>' : "";
        var html_Select = "<label for=\"DDL_" + attr.KeyOfEn + "\"><p>" + attr.Name + mustInput + "</p></label>";
        html_Select += "<select name=\"DDL_" + attr.KeyOfEn + "\" id=\"DDL_" + attr.KeyOfEn + "\" readonly='" + (attr.UIIsEnable == "0" ? "readonly" : "") + "' >";

        html_Select += InitDDLOperation(frmData, attr, "");
        html_Select += "</select>&nbsp;&nbsp;";
        return html_Select;
    },
    CreateMapPin: function (attr) {
        loadScript("http://api.map.baidu.com/api?v=2.0&ak=rgwS2tQzfT9dX21CvZkyTE2eQ1D0vDWh&Version=" + Math.random());
        loadScript("http://developer.baidu.com/map/jsdemo/demo/convertor.js?Version=" + Math.random());
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



function InitMapAttrOfCtrl(mapAttr) {

    var eleHtml = '';
    if (mapAttr.UIVisible == 1) {//是否显示

        var str = '';
        var defValue = ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn);

        var isInOneRow = false; //是否占一整行
        var islabelIsInEle = false; //

        eleHtml += '';

        if (mapAttr.UIContralType != 6) {

            if (mapAttr.LGType == 2) {
                //多选下拉框
                var isMultiSele = "";
                var isMultiSeleClass = "";
                //                if (mapAttr.UIIsMultiple != undefined && mapAttr.UIIsMultiple == 1) {
                //                    isMultiSele = ' multiple data-live-search="false" ';
                //                    isMultiSeleClass = " selectpicker show-tick form-control ";
                //                }
                eleHtml += "<select my='1' data-val='" + ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' class='" + isMultiSeleClass + "' " + isMultiSele + " name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(workNodeData, mapAttr, defValue) + "</select>";
            } else {

                //添加文本框 ，日期控件等
                //AppString
                if (mapAttr.MyDataType == "1" && mapAttr.LGType != "2") {//不是外键
                    if (mapAttr.UIContralType == "1") {//DDL 下拉列表框

                        //多选下拉框
                        var isMultiSele = "";
                        var isMultiSeleClass = "";

                        //                        if (mapAttr.UIIsMultiple != undefined && mapAttr.UIIsMultiple == 1) {
                        //                            isMultiSele = ' multiple data-live-search="false" ';
                        //                            isMultiSeleClass = " selectpicker show-tick form-control ";
                        //                        }


                        eleHtml +=
                            "<select my='2' data-val='" + ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' class='" + isMultiSeleClass + "' " + isMultiSele + " name='DDL_" + mapAttr.KeyOfEn + "' value='" + ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + " >" +
                            (workNodeData, mapAttr, defValue) + "</select>";
                    } else { //文本区域
                        if (mapAttr.UIHeight <= 23 || 1 == 1) {
                            eleHtml +=
                                "<input maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "' style='width:" + mapAttr.UIWidth + ";height:" + mapAttr.UIHeight + ";' type='text' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + " />"
                            ;
                        }
                    }
                } //AppDate
                else if (mapAttr.MyDataType == 6) {//AppDate
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 0) {
                        enableAttr = "disabled='disabled'";
                    }

                    var inputHtml = "<input " + enableAttr + " type='text'  ";
                    inputHtml += "name=\"TB_" + mapAttr.KeyOfEn + "\" id=\"TB_" + mapAttr.KeyOfEn + "\" ";
                    inputHtml += "value=\"" + mapAttr.FieldRelValue + "\" />";
                    //inputHtml += "<script>$('#TB_" + mapAttr.KeyOfEn + "').datepicker({inline: true,dateFormat: 'yy-mm-dd'});</script>";
                    eleHtml += inputHtml;
                }
                else if (mapAttr.MyDataType == 7) {// AppDateTime = 7
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {
                        //  enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
                        enableAttr = '';
                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += "<input  type='date'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' />";
                }
                else if (mapAttr.MyDataType == 4) {  // AppBoolean = 7
                    var CB_Html = "<input type='checkbox' name='CB_" + mapAttr.KeyOfEn + "' id='CB_" + mapAttr.KeyOfEn + "'>";

                    eleHtml += CB_Html;
                }


                if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
                    if (mapAttr.UIContralType == 1) { //DDL
                        eleHtml += "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(workNodeData, mapAttr, defValue) + "</select>";
                    }
                }

                // AppDouble  AppFloat 
                if (mapAttr.MyDataType == 5 || mapAttr.MyDataType == 3) {
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {

                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += "<input style='text-align:right;' onkeyup=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
                }
                if ((mapAttr.MyDataType == 2 && mapAttr.LGType != 1)) {//AppInt
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {

                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += "<input style='text-align:right;' onkeyup=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
                }
                //AppMoney  AppRate
                if (mapAttr.MyDataType == 8) {
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {

                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += "<input style='text-align:right;' onkeyup=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
                }
            }
        } else {
            //展示附件信息  FREE 不需要
            return;

            //            var atParamObj = AtParaToJson(mapAttr.AtPara);
            //            if (atParamObj.AthRefObj != undefined) {//扩展设置为附件展示
            //                eleHtml += "<input type='hidden' class='tbAth' data-target='" + mapAttr.AtPara + "' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' >" + "</input>";
            //                defValue = defValue != undefined && defValue != '' ? defValue : '&nbsp;';
            //                if (defValue.indexOf('@AthCount=') == 0) {
            //                    defValue = "附件" + "<span class='badge'>" + defValue.substring('@AthCount='.length, defValue.length) + "</span>个";
            //                } else {
            //                    defValue = defValue;
            //                }
            //                eleHtml += "<div class='divAth' data-target='" + mapAttr.KeyOfEn + "'  id='DIV_" + mapAttr.KeyOfEn + "'>" + defValue + "</div>";
            //            }
        }

        if (islabelIsInEle == false) {
            //eleHtml = '<div style="text-align:right;padding:0px;margin:0px; ' + (isInOneRow ? "clear:left;" : "") + '"  class="col-lg-1 col-md-1 col-sm-2 col-xs-4"><label>' + mapAttr.Name + "</label>" +
            //(mapAttr.UIIsInput == 1 ? '<span style="color:red" class="mustInput" data-keyofen="' + mapAttr.KeyOfEn + '">*</span>' : "")
            //+ "</div>" + eleHtml;
            //先把 必填项的 * 写到元素后面 可能写到标签后面更合适
            //eleHtml += mapAttr.UIIsInput == 1 ? '<span style="color:red" class="mustInput" data-keyofen="' + mapAttr.KeyOfEn + '">*</span>' : "";
        }
    } else {
        var value = ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn);
        if (value == undefined) {
            value = '';
        } else {
            //value = value.toString().replace(/：/g, ':').replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}');
        }

        //hiddenHtml += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + " value='" + ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' name='TB_" + mapAttr.KeyOfEn + "></input>";
        eleHtml += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "' />";
    }


    //eleHtml = "<div style='width:" + mapAttr.UIWidth + ";height:" + mapAttr.UIHeight + ";float:right'>" + eleHtml + "</div>";

    return eleHtml;

    // alert(eleHtml);

    eleHtml = $('<div>' + eleHtml + '</div>');
    eleHtml.children(0).css('width', mapAttr.UIWidth).css('height', mapAttr.UIHeight);

    //eleHtml.css('position', 'absolute').css('top', mapAttr.Y - 10).css('left', mapAttr.X);

    if (mapAttr.UIIsEnable == "0") {
        enableAttr = eleHtml.find('[name=TB_' + mapAttr.KeyOfEn + ']').attr('disabled', true);
        enableAttr = eleHtml.find('[name=DDL_' + mapAttr.KeyOfEn + ']').attr('disabled', true);
    }

    return eleHtml;
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

            if (mapAttr.DefVal == -1)
                operations += "<option " + (defVal == -1 ? " selected = 'selected' " : "") + " value='" + mapAttr.DefVal + "'>-无(不选择)-</option>";

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
//填充默认数据
function ConvertDefVal(workNodeData, defVal, keyOfEn) {
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
    for (var ele in workNodeData.MainTable[0]) {
        if (keyOfEn == ele && workNodeData.MainTable[0] != '') {
            result = workNodeData.MainTable[0][ele];
            break;
        }
    }

   
    var result = unescape(result);

    if (result == "null")
        result = "";

    return result;
}
 

var workNodeData = {};

//将#FF000000 转换成 #FF0000
function TranColorToHtmlColor(color) {
    if (color != undefined && color.indexOf('#') == 0 && color.length == 9) {
        color = color.substring(0, 7);
    }
    return color;
}

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

    var imgSrc = "/WF/Data/Img/LogH.PNG";
    //获取数据
    if (workNodeData.Sys_FrmImgAthDB) {
        $.each(workNodeData.Sys_FrmImgAthDB, function (i, obj) {
            if (obj.FK_FrmImgAth == frmImageAth.MyPK) {
                imgSrc = obj.FileFullName;
            }
        });
    }
    //设计属性
    img.attr('id', 'Img' + frmImageAth.MyPK).attr('name', 'Img' + frmImageAth.MyPK);
    img.attr("src", imgSrc).attr('onerror', "this.src='/WF/Data/Img/LogH.PNG'");
    img.css('width', frmImageAth.W).css('height', frmImageAth.H).css('padding', "0px").css('margin', "0px").css('border-width', "0px");
    //不可编辑
    if (isEdit == "1") {
        var fieldSet = $("<fieldset></fieldset>");
        var length = $("<legend></legend>");
        var a = $("<a></a>");
        var url = "/WF/CCForm/ImgAth.aspx?W=" + frmImageAth.W + "&H=" + frmImageAth.H + "&FK_MapData=ND" + pageData.FK_Node + "&MyPK=" + pageData.WorkID + "&ImgAth=" + frmImageAth.MyPK;

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
        if (pageData.IsReadonly) {

            src = "./CCForm/Dtl.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=1&" + urlParam + "&Version=" + load.Version;
        } else {
            src = "./CCForm/Dtl.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=0&" + urlParam + "&Version=" + load.Version;
        }
    }
    else if (frmDtl.DtlShowModel == "1") {
        if (pageData.IsReadonly)
            src = appPath + "WF/CCForm/DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=1" + strs;
        else
            src = appPath + "WF/CCForm/DtlCard.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=0" + strs;

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
    var sta = wf_node.FWCSta;
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

    var eleHtml = '<div id="FFWC' + wf_node.NodeID + '">' + "<iframe style='width:100%;height:500px;' id='FFWC" + wf_node.NodeID + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
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
    var eleHtml = '<div id=DIVWC' + wf_node.NodeID + '>' + "<iframe id=FSF" + wf_node.NodeID + " style='width:100%;height:" + h + "px'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);

    return eleHtml;
}

//初始化框架
function figure_Template_IFrame(fram) {
    var eleHtml = '';
    var src = dealWithUrl(fram.src) + "IsReadOnly=0";
    eleHtml = $('<div id="iframe' + fram.MyPK + '">' + '</div>');
    var iframe = $("<iframe  style='width:" + fram.W + "px; height:" + fram.H + "'     src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling='no'></iframe>");
    eleHtml.append(iframe);
    return eleHtml;
}
 
var colVisibleJsonStr = ''
var jsonStr = '';

$(function () {

    initPageParam(); //初始化参数

    GenerFrm(); //表单数据.ajax

});

function BackToHome() {
    SetHref('../CCMobilePortal/Home.htm?UserNo=' + GetQueryString('UserNo') + "&Token=" + GetQueryString("Token"));
}

function BackToTodolist() {
    SetHref('../CCMobilePortal/Todolist.htm?UserNo=' + GetQueryString('UserNo') + "&Token=" + GetQueryString("Token"));
}

function BackToStart() {
    SetHref('../CCMobilePortal/Start.htm?UserNo=' + GetQueryString('UserNo') + "&Token=" + GetQueryString("Token"));
}
