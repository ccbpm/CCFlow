//. 保存嵌入式表单. add 2015-01-22 for GaoLing.
function SaveSelfFrom() {

    // 不支持火狐浏览器。
    var frm = document.getElementById('SelfForm');
    if (frm == null) {
        alert('系统错误,没有找到SelfForm的ID.');
    }
    //执行保存.
    return frm.contentWindow.Save();
}

function SendSelfFrom() {
    if (SaveSelfFrom() == false) {
        alert('表单保存失败，不能发送。');
        return false;
    }
    return true;
}

//停止流程.
function DoStop(msg, flowNo, workid) {

    if (confirm('您确定要执行 [' + msg + '] ?') == false)
        return;

    var para = 'DoType=MyFlow_StopFlow&FK_Flow=' + flowNo + '&WorkID=' + workid;

    AjaxService(para, function (msg, scope) {
        alert(msg);
        if (msg.indexOf('err@') == 0) {
            return;
        }
        SetHref( 'Todolist.htm');
    });
}

//空方法，不能删除.
function SysCheckFrm() {

}
function KindEditerSync() {
    return true;
}

function Change() {
    var btn = document.getElementById('Btn_Save');
    if (btn != null) {
        if (btn.value.valueOf('*') == -1)
            btn.value = btn.value + '*';
    }
}


// 获取附件文件名称,如果附件没有上传就返回null.
function ReqAthFileName(athID) {
    var v = document.getElementById(athID);
    if (v == null) {
        return null;
    }
    var fileName = v.alt;
    return fileName;
}

//执行分支流程退回到分合流节点。
function DoSubFlowReturn(fid, workid, fk_node) {
    var url = 'ReturnWorkSubFlowToFHL.htm?FID=' + fid + '&WorkID=' + workid + '&FK_Node=' + fk_node;
    var v = WinShowModalDialog(url, 'df');
    SetHref( window.history.url);
}
function To(url) {
    //window.location.href = filterXSS(url);
    
    window.name = "dialogPage"; window.open(filterXSS(url), "dialogPage")
}

function DoDelSubFlow(fk_flow, workid) {
    if (window.confirm('您确定要终止进程吗？') == false)
        return;

    var para = 'DoType=DelSubFlow&FK_Flow=' + fk_flow + '&WorkID=' + workid;
    AjaxService(para, function (msg, scope) {
        alert(msg);
        Reload();
    });
}



function Do(warning, url) {
    if (window.confirm(warning) == false)
        return;
    SetHref(url);
}

//关注 按钮.
function FocusBtn(btn, workid) {

    if (btn.value == '关注') {
        btn.value = '取消关注';
    }
    else {
        btn.value = '关注';
    }

    var para = "DoType=Focus&WorkID=" + workid;
    AjaxService(para, function (msg, scope) {
        // alert(msg);
    });
}

//确认 按钮.
function ConfirmBtn(btn, workid) {

    if (btn.value == '确认') {
        btn.value = '取消确认';
    }
    else {
        btn.value = '确认';
    }

    var para = "DoType=Confirm&WorkID=" + workid;
    AjaxService(para, function (msg, scope) {
        //  alert(msg);
    });
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
    pageData.PWorkID = GetQueryString("PWorkID");
    pageData.IsRead = GetQueryString("IsRead");
    pageData.T = GetQueryString("T");
    pageData.Paras = GetQueryString("Paras");
    pageData.IsReadonly = 0; //如果是IsReadOnly，就表示是查看页面，不是处理页面
    pageData.IsStartFlow = GetQueryString("IsStartFlow"); //是否是启动流程页面 即发起流程
    pageData.DoType1 = GetQueryString("DoType");
}

//将获取过来的URL参数转成URL中的参数形式  &
function pageParamToUrl() {
    var paramUrlStr = '';
    for (var param in pageData) {
        paramUrlStr += '&' + (param.indexOf('@') == 0 ? param.substring(1) : param) + '=' + pageData[param];
    }
    return paramUrlStr;
}

function addBarContent(barcount, bottombar, popoverBar, barHtml) {
    if (barcount == 4) {
        bottombar.append('<a class="mui-tab-item" href="#Popover">更多</a>');
    } else if (barcount > 4) {
        popoverBar.append(barHtml);
    } else {
        bottombar.append(barHtml);
    }
}


//结束流程.
function DoStop(msg, flowNo, workid) {

    mui.confirm('您确定要执行 [' + msg + '] ?', function (e) {
        if (e.index == 1) {
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
            handler.AddPara("FK_Flow", flowNo);
            handler.AddPara("WorkID", workid);
            var data = handler.DoMethodReturnString("MyFlow_StopFlow");
            if (data.indexOf('err@') == 0)
                return;
            SetHref( "Todolist.htm?1=2");

        }
    })

}
//删除流程.
function DeleteFlow() {
    mui.confirm('您确定要删除吗？', function (e) {
        if (e.index == 1) {
            var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_MyFlow");
            handler.AddPara("FK_Flow", GetQueryString("FK_Flow"));
            handler.AddPara("WorkID", GetQueryString('WorkID'));
            var data = handler.DoMethodReturnString("MyFlowGener_Delete");
            if (data.indexOf("err@") != -1) {
                mui.alert(data);
                return;
            }
            SetHref("Todolist.htm?1=2");

        }
    })
}



//隐藏下方的功能按钮
function setToobarDisiable() {
    //隐藏下方的功能按钮
    $('.Bar input').css('background', 'gray');
    $('.Bar input').attr('disabled', 'disabled');
}

function setToobarEnable() {
    //隐藏下方的功能按钮
    $('.Bar input').css('background', '#2884fa');
    $('.Bar input').removeAttr('disabled');
}
//设置表单元素不可用
function setFormEleDisabled() {
    //文本框等设置为不可用
    $('#divCCForm textarea').attr('disabled', 'disabled');
    $('#divCCForm select').attr('disabled', 'disabled');
    $('#divCCForm input[type!=button]').attr('disabled', 'disabled');
}

//保存
function Save(flag) {
    var iframe = document.getElementById("tbTracks");
    if (iframe)
        SaveWorkCheck();

    //保存从表
    frmData.Sys_MapDtl.forEach(function (dtl) {
        if (dtl.MobileShowModel == 2) {
            Dtl_SaveData(dtl.No, dtl.No +"form_Dtl");
        }
    })
    $("input").blur();
    if ($(".compareClass").length > 0)
        return false;
    //必填项和正则表达式检查.
    if (checkBlanks() == false) {
        mui.alert("请输入必填项！");
        return false;
    }
    if (checkReg() == false) {
        mui.alert("正则验证错误，请检查边框变红字段！");
        return false;
    }


    var url = "";

    var sys_MapData = frmData["Sys_MapData"][0];

    var params = getFormData(true, true, "divCCForm", false);

    if (sys_MapData.No.indexOf('ND') == 0) {
        var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_MyFlow");
        //$.each(params.split("&"), function (i, o) {
        //    var param = o.split("=");
        //    if (param.length == 2 && validate(param[1])) {
        //        handler.AddPara(param[0], param[1]);
        //    } else {
        //        handler.AddPara(param[0], "");
        //    }
        //});
        handler.AddUrlData();
        handler.AddJson(params);
        handler.AddPara("IsMobile", 1);
        var data = handler.DoMethodReturnString("Save");
    } else {
        var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_MyFlow");

        handler.AddUrlData();
        handler.AddJson(params);
        handler.AddPara("IsMobile", 1);
        handler.AddPara("FK_MapData", sys_MapData.No);
        handler.AddPara("OID", GetQueryString("WorkID"));
        var data = handler.DoMethodReturnString("FrmGener_Save");
    }

    if (data.indexOf('err@') == 0) {
        mui.alert(data);
        return;
    }
    if (flag == 0) {//0:保存按钮保存;1:发送时的保存

        mui.toast("保存成功！");
    }


}

//获得表单数据.
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

//点击文件名称执行的下载.
function Down2017(mypk) {

    //组织url.
    var url = Handler + "?DoType=AttachmentUpload_Down&MyPK=" + mypk + "&m=" + Math.random();

    $.ajax({
        type: 'post',
        async: true,
        url: url,
        dataType: 'html',
        success: function (data) {

            if (data.indexOf('err@') == 0) {
                alert(data); //如果是异常，就提提示.
                return;
            }

            if (data.indexOf('url@') == 0) {

                data = data.replace('url@', ''); //如果返回url，就直接转向.

                var i = data.indexOf('\DataUser');
                var str = '/' + data.substring(i);
                str = str.replace('\\\\', '\\');
                window.open(str, "_blank");
                return;
            }
            alert(data);
            return;
        }
    });
}


//刷新子流程
function refSubSubFlowIframe() {
    var iframe = $('iframe[src*="SubFlow.aspx"]');
    iframe[0].contentWindow.location.href = filterXSS(iframe[0].src);
}

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
                //多选框
                if (attr.UIContralType == UIContralType.CheckBok) {
                    _html += "<div class='mui-input-row  mui-checkbox mui-left' style='height: auto;'>";
                    _html += "<label style = 'padding: 11px 15px; float: left;width: 35%;' for=\"" + attr.KeyOfEn + "\" style='width: 35%;'><p>" + attr.Name + "</p></label>";
                    _html += '<div style="display: inline - block;float: right;">'
                    _html += Initcheckbox(frmData, attr);
                    break;
                }
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
        //位置
        if (attr.UIContralType == 16 && attr.UIIsEnable == "0") {
            loadScript("http://res.wx.qq.com/open/js/jweixin-1.2.0.js?Version=" + Math.random());
            html_string += "<div class='mui-input-row'>";
            html_string += "<button type='button' class='mui-btn' id='TB_fixed'style='margin-right:20px;width:35%' onclick='GetFixedInfoByJDWD(\"" + frmData.MainTable[0].JD + "\",\"" + frmData.MainTable[0].WD + "\")'>显示位置</button>";
            html_string += "</div>";
            return html_string;
        }
        //按钮操作
        if (attr.UIContralType == 18) { 
            html_string += "<div class='mui-input-row'>";
            html_string += "<button type='button' style='margin:3px' class='mui-btn mui-btn-primary mui-attr-btn' id='Btn_" + attr.KeyOfEn + "'>" + attr.Name + "</button>";
            html_string += "</div>";
            return html_string;
        }
        //写字板
        if (attr.UIContralType == 8) {
            var val = ConvertDefVal(frmData, attr.DefVal, attr.KeyOfEn);
            var imgSrc= "../DataUser/Siganture/UnName.jpg";
            html_string += "<div class='mui-input-row' style='min-height:37px;'>";
            html_string += "<label for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + mustInput + "</p></label>";
            var html = "<input maxlength=" + attr.MaxLen + "  id='TB_" + attr.KeyOfEn + "'  name='TB_" + attr.KeyOfEn + "' value='" + val + "' type=hidden />";

            html_string += "<img src='" + val + "' onerror=\"this.src='" + imgSrc + "'\"  style='border:0px;height:" + attr.UIHeight + "px;' id='Img" + attr.KeyOfEn + "' />" + html;
            return html_string;
        }
        //富文本
        if (attr.TextModel == 3) {
            html_string += "<div class='' style='padding:11px 15px;line-height:1.1;'>";
            html_string += "<label for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + mustInput + "</p></label>";
            if (attr.UIIsEnable == "0") {
                var val = ConvertDefVal(frmData, attr.DefVal, attr.KeyOfEn);
                val = val.replace(/white-space: nowrap;/g, "");
                return html_string + "<div style='margin: 9px 0px 9px 0px;border-bottom: 1px solid #c8c7cc59;padding-left: 15px;'>" + val + "</div>";
            }
            html_string += "<textarea wrap='virtual' onpropertychange= 'this.style.posHeight=this.scrollHeight' cols='40' style='overflow:visible;font-size:14px;width:100%;border:solid 1px gray;' rows=\"5\" placeholder=\"" + attr.Tip + "\"  onfocus =\"this.placeholder = ''\" onblur=\"this.placeholder = '" + attr.Tip + "'\" name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\"></textarea>";
            return html_string;
        }
        //多行文本
        if ((attr.UIHeight > 30 && attr.ColSpan > 1) || attr.TextModel == 2) {
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
        }else {
            html_string += "<input  style='background-color:#fff;margin: auto;position: absolute;top: 0;bottom: 0;height:auto;' type='text' name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder=\"" + attr.Tip + "\" onfocus=\"this.placeholder = ''\" onblur=\"this.placeholder = '" + attr.Tip + "'\" />";
        }

        return html_string;
    },
    CreateTBInt: function (attr) {
        var mustInput = attr.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + attr.KeyOfEn + '" >*</span>' : "";

        var inputHtml = "<label style='background-color:#fff;margin:0px;' for=\"TB_" + attr.KeyOfEn + "\"><p>" + attr.Name + mustInput + "</p></label>";

        var event = 'onblur="valitationAfter(this, \'int\')" onkeydown="valitationBefore(this, \'int\')" onkeyup="valitationAfter(this, \'int\'); if(isNaN(value) || (value%1 !== 0))execCommand(\'undo\')"  onafterpaste="valitationAfter(this, \'int\'); if(isNaN(value) || (value%1 !== 0))execCommand(\'undo\')"';
        if (attr.UIIsEnable == "1" && pageData.IsReadonly != 1) {
            var step = GetPara(attr.AtPara, "NumStepLength");
            step = step == null || step == undefined ? 1 : parseInt(step) == 0 ? 1 : parseInt(step);
            var minNum = GetPara(attr.AtPara, "NumMin") || "";
            var maxNum = GetPara(attr.AtPara, "NumMax") || "";
            var dataInfo = "";
            if (minNum != "")
                dataInfo = " data-numbox-min='" + minNum + "'";
            if (maxNum != "")
                dataInfo += " data-numbox-max='" + maxNum + "'";

            inputHtml += "<div class='mui-numbox' data-numbox-step='" + step + "'  data-numbox-bit='0'" + dataInfo +"  style='width:200px'>";
            inputHtml += "<button class='mui-btn mui-btn-numbox-minus' type='button'>-</button>";
        }
        inputHtml += "<input  class='mui-numbox-input' style='margin: auto;position: absolute;top: 0;bottom: 0;' style='background-color:#fff' type=\"number\" " + event;
        if (attr.DefValType == 0)
            inputHtml += " name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" />";
        else
            inputHtml += " name=\"TB_" + attr.KeyOfEn + "\" id=\"TB_" + attr.KeyOfEn + "\" placeholder='0' />";
        if (attr.UIIsEnable == "1" && pageData.IsReadonly != 1) {
            inputHtml += "<button class='mui-btn mui-btn-numbox-plus' type='button'>+</button>";
            inputHtml += "</div>";
        }
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

            inputHtml += "<div class='mui-numbox' data-numbox-step='" + step + "' data-numbox-bit='" + bit + "'" + dataInfo+" style='width:200px'>";
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

//AtPara  @PopValSelectModel=0@PopValFormat=0@PopValWorkModel=0@PopValShowModel=0
function GepParaByName(name, atPara) {
    var params = atPara.split('@');
    var result = $.grep(params, function (value) {
        return value != '' && value.split('=').length == 2 && value.split('=')[0] == value;
    })
    return result;
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
function ConvertDefVal(flowData, defVal, keyOfEn) {
    //防止传值为null的情况
    if (defVal == null) {
        defVal = "";
    }
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
    var mainData = flowData.MainTable == undefined ? flowData : flowData.MainTable[0];
    //通过MAINTABLE返回的参数
    for (var ele in mainData) {
        if (keyOfEn == ele) {
            result = mainData[ele];
            if (result == null) {
                result = "";
            }
            break;
        }
    }

    if (result != undefined && typeof (result) == 'string') {
        //result = result.replace(/｛/g, "{").replace(/｝/g, "}").replace(/：/g, ":").replace(/，/g, ",").replace(/【/g, "[").replace(/】/g, "]").replace(/；/g, ";").replace(/~/g, "'").replace(/‘/g, "'").replace(/‘/g, "'");
    }
    return result = unescape(result);
}


//选择发送节点
function checkRadio(id) {
    $("#" + id).prop('checked', 'true');
}

//发送
function SendIt(isHuiQian) {
    $("input").blur();
    if ($(".compareClass").length > 0)
        return;

    $("#SendBtn").addClass("mui-disabled");//禁用发送按钮
    GetWXFixed(function (isHuiQian) {
        SendForm(isHuiQian);
    });

}
function SendForm(isHuiQian) {

    var toNodeNo = 0;

    //发送前事件
    if (typeof beforeSend != 'undefined' && beforeSend instanceof Function)
        if (beforeSend() == false) {
            $("#SendBtn").removeClass("mui-disabled");
            return false;
        }

    //执行一下保存
    if (Save(1) == false) {
        $("#SendBtn").removeClass("mui-disabled");
        return false;
    }

    //判断从表数量
    var frmCheck = true;
    if (typeof dtls != 'undefined' && $("iframe[name=Dtl]").length > 0) {
        $.each($("iframe[name=Dtl]"), function (i, obj) {
            var dtlNo = obj.id;
            var item = dtls[dtlNo];
            if (item == undefined) {
                $("#HD_CurDtl_No").val(dtlNo);
                Load_DtlInit("DtlContent", dtlNo);
            }
            item = dtls[dtlNo][0];
            if (item.Count < item.NumOfDtl) {
                mui.alert("[" + item.Name + "]明细不能少于最小数量" + item.NumOfDtl);
                frmCheck = false;
            }
        })
        if (frmCheck == false)
            return false;
    }

    //如果启用了流程流转自定义，必须设置选择的游离态节点
    if ($('[name=TransferCustom]').length > 0) {
        var ens = new Entities("BP.WF.TransferCustoms");
        ens.Retrieve("WorkID", pageData.WorkID, "IsEnable", 1);
        if (ens.length == 0) {
            mui.alert("该节点启用了流程流转自定义，但是没有设置流程流转的方向，请点击流转自定义按钮进行设置");
            return false;
        }
        msg = "";
        $.each(ens, function (i, en) {
            if (en.Worker == null || en.Worker == "")
                msg += "节点[" + en.NodeName + "],";
        })
        if (msg != "") {
            msg += "没有设置接收人。";
            mui.alert(msg);
            return false;
        }
    }

    //根据workID 获取流程信息
    var gwf = new Entity("BP.WF.GenerWorkFlow", pageData.WorkID);
    var toNodeDDL = $("<div style='text-align:left'></div>");
    if (initData.ToNodes != undefined && initData.ToNodes.length > 1) {
        var isLastHuiQian = true;
        //待办人数
        var todoEmps = gwf.TodoEmps;
        if (todoEmps != null && todoEmps != undefined) {
            var huiqianSta = gwf.GetPara("HuiQianTaskSta") == 1 ? true : false;
            if (wf_node.TodolistModel == 1 && huiqianSta == true && todoEmps.split(";").length > 1)
                isLastHuiQian = false;
        }

/*
        if (gwf.GetPara("HuiQianTaskSta") == "1" && flowData.WF_GenerWorkFlow[0].HuiQianZhuChiRen != webUser.No) {
            execSend(toNodeNo);
            return false;
        }*/
        $.each(initData.ToNodes, function (i, toNode) {

            var opt = "";
            if (toNode.IsSelected == "1") {
                 opt = $("<div class='mui-input-row mui-radio mui-left' onclick='checkRadio(" + toNode.No + ")'><label>" + toNode.Name + "</label><input type='radio'  id='" + toNode.No + "' name='Radio_ToNode' value='" + toNode.No + "' checked='checked' ></div><br/>");
            } else {
                opt = $("<div class='mui-input-row mui-radio mui-left' onclick='checkRadio(" + toNode.No + ")'><label>" + toNode.Name + "</label><input type='radio'  id='" + toNode.No + "' name='Radio_ToNode' value='" + toNode.No + "' ></div><br/>");
            }
            opt.data(toNode);
            toNodeDDL.append(opt);
            
        });

        var btnArray = ['取消', '确定'];
        mui.prompt('', null, '请选择下一节点', btnArray, function (e) {
            if (e.index == 1) {
                $(".mui-popup-button-bold").css("color", "#CCCCCC");
                this.disabled = true;
                toNodeNo = $("input[name='Radio_ToNode']:checked").val();
                var selectToNode;
                if ($("#" + toNodeNo).length != 0) {
                    selectToNode = $("#" + toNodeNo).parent(0).data();

                }
                if (selectToNode.IsSelected == 2)
                    isReturnNode = 1;
                if (selectToNode.IsSelectEmps == "1" && isLastHuiQian == true) {
                    if (isHuiQian == true) {
                        initModal("HuiQian", toNodeNo);
                    } else {
                        initModal("sendAccepter", selectToNode);
                    }
                    return;
                }
                if (selectToNode.IsSelectEmps == "2") {
                    if (isHuiQian == true) {
                        initModal("HuiQian", toNodeNo);
                    } else {
                        var url = selectToNode.DeliveryParas;
                        if (url != null && url != undefined && url != "") {
                            url = url + "?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&ToNode=" + toNodeNo + "&s=" + Math.random();
                            initModal("BySelfUrl", toNodeNo, url);
                            return;
                        }
                    }
                }
                if (selectToNode.IsSelectEmps == "3") {
                   
                    if (isHuiQian == true) {
                        initModal("HuiQian", toNodeNo);
                    } else {
                        initModal("sendAccepterOfOrg", toNodeNo);
                    }
                    return;
                }

                if (selectToNode.IsSelectEmps == "4") {
                    
                    if (isHuiQian == true) {
                        initModal("HuiQian", toNodeNo);
                    } else {
                        initModal("AccepterOfDept", toNodeNo);
                    }
                    return;
                }
                if (isHuiQian == true) {
                    initModal("HuiQian", toNodeNo);
                    //$('#returnWorkModal').modal().show();
                    $("#SendBtn").removeClass("mui-disabled");
                    return;
                } else {
                    execSend(toNodeNo);
                }
            } else {
                $("#SendBtn").removeClass("mui-disabled");
            }
        }, 'div');
        var tt = $('.mui-popup-input');
        tt.empty();
        tt.append(toNodeDDL);
    } else if (initData.ToNodes != undefined && initData.ToNodes.length == 1) {
        var selectToNode = initData.ToNodes[0];
        toNodeNo = initData.ToNodes[0].No;
        if (selectToNode.IsSelectEmps == "1") { //跳到选择接收人窗口

            $('[name=SaveBtn]').trigger("click");
            isSaveOnly = false;
            if (isHuiQian == true) {
                initModal("HuiQian", toNodeNo);
            } else {
                initModal("sendAccepter", selectToNode);
            }
            return false;
        }
        if (selectToNode.IsSelectEmps == "2") {
            //Save(1); //执行保存.
            if (isHuiQian == true) {
                initModal("HuiQian", toNodeNo);
            } else {
                var url = selectToNode.DeliveryParas;
                if (url != null && url != undefined && url != "") {
                    url = url + "?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&ToNode=" + toNodeID + "&s=" + Math.random();
                    initModal("BySelfUrl", toNodeNo, url);
                    return false;
                }
            }
        }
        if (selectToNode.IsSelectEmps == "3") {

            //Save(1); //执行保存.
            if (isHuiQian == true) {
                initModal("HuiQian", toNodeNo);
            } else {
                initModal("sendAccepterOfOrg", toNodeNo);
            }
            return false;
        }

        if (selectToNode.IsSelectEmps == "4") {

            if (isHuiQian == true) {
                initModal("HuiQian", toNodeNo);
            } else {
                initModal("AccepterOfDept", toNodeNo);
            }
            return false;
        }
        if (isHuiQian == true) {
            toNodeNo = initData.ToNodes[0].No;
            initModal("HuiQian", toNodeNo);
            $("#SendBtn").removeClass("mui-disabled");
            return false;
        } else {
            execSend(toNodeNo);
        }
        return false;

    }else {
        if (isHuiQian == true) {
            toNodeNo = initData.ToNodes[0].No;
            initModal("HuiQian", toNodeNo);
            $("#SendBtn").removeClass("mui-disabled");
            return false;
        } else {
            execSend(toNodeNo);
        }
    }

}

function execSend(toNode) {

    //先设置按钮等不可用
    //setToobarDisiable();
    //Save(1);
    //判断是否启用审核组件
    var iframe = document.getElementById("tbTracks");
    if (iframe)
        if (SaveWorkCheck() == false)
            return false;


    var params = getFormData(true, true, "divCCForm",false);
    var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_MyFlow");
    handler.AddPara("IsMobile", 1);
    handler.AddPara("ToNode", toNode);
    handler.AddUrlData();
    handler.AddJson(params);
    var data = handler.DoMethodReturnString("Send"); //执行发送方法.


    if (data.indexOf('err@') == 0) { //发送时发生错误
        mui.alert(data);
        //setToobarEnable();
        $("#SendBtn").prop("disabled", false);
        $("#SendBtn").css("color", "#000");
        return;
    }
    if (data.indexOf('TurnUrl@') == 0) {  //发送成功时转到指定的URL 
        var url = data;
        url = url.replace('TurnUrl@', '');
        SetHref(url);
        return ;
    }
    if (data.indexOf('url@') >= 0) { //发送成功时转到指定的URL 
        if (data.indexOf('Accepter') != 0 && data.indexOf('AccepterGener') == -1) {
            //求出url里面的的FK_Node
            var params = data.split("&");
            var toNodeId = '';
            for (var i = 0; i < params.length; i++) {
                if (params[i].indexOf("ToNode") == -1)
                    continue;
                toNodeId = params[i].split("=")[1];
                break;
            }

            var toNode = new Entity("BP.WF.Node", toNodeId);
            initModal("sendAccepter", toNode);
            $("#returnWorkModal").modal().show();
            return;
        }
        var url = data;
        url = url.replace('url@', '');
        SetHref(url);
        return;
    }
    if (data.indexOf('SelectNodeUrl@') >= 0) { //发送成功时转到指定的URL 
        var url = data;
        url = url.replace('SelectNodeUrl@', '');
        SetHref(url);
        return;
    }

    data = data.replace('@', '<br/>@');
    data = data.replace(/@/g, '<br/>');
    data = data.replace(/null/g, '');
    OptSuc(data);

    return;

}

//发送 退回 移交等执行成功后转到  指定页面
function OptSuc(msg) {
    msg = msg.replace("@查看<img src='/WF/Img/Btn/PrintWorkRpt.gif' >", '')
    msg = msg.replace(/@/g, '<br/>').replace(/null/g, '');
    msg = msg.replace('<br/>', '').replace('<br/>', '');
    if (msg.indexOf("WorkOpt/AllotTask.htm") != -1) {
        var msgs = msg.split('<br/>');
        msg = $.grep(msgs, function (obj, i) {
            if (obj.indexOf("WorkOpt/AllotTask.htm") == -1) return obj;
        });
        msg = msg.join('<br/>');
    }

    $("#CCForm").html(msg);
    $("#divCCForm").parent().css("transform", "translate3d(0px, 0px, 0px)");


    $('#bottomToolBar').html("<a class='mui-tab-item' id='Back_Home' href='#' >返回主页 </ a><a class='mui-tab-item' id='Back_TodoList' href='#' >返回待办 </ a>");
    $("#Back_Home").on("tap", function () {
        tranfToUrl('Home.htm');

    });

    $("#CCForm").css("padding", "5px").css("line-height", "34px");
    $("#Back_TodoList").on("tap", function () {
        tranfToUrl('Todolist.htm');

    });
}


function tranfToUrl(url) {
    location.href = url;
}
//移交
//初始化发送节点下拉框
function InitToNodeDDL(flowData) {

    if (flowData.ToNodes == undefined)
        return;

    if (flowData.ToNodes.length == 0)
        return;

    //如果没有发送按钮，就让其刷新,说明加载不同步.
    var btn = $('[name=Send]');
    if (btn == null || btn == undefined) {
        Reload();
        return;
    }

    //var flowData = JSON.parse(jsonStr);

    if (flowData.ToNodes != undefined && flowData.ToNodes.length > 0) {
        // $('[value=发送]').
        var toNodeDDL = $('<select style="width:auto;" id="DDL_ToNode"></select>');
        $.each(flowData.ToNodes, function (i, toNode) {

            var opt = "";
            if (toNode.IsSelected == "1") {
                var opt = $("<option value='" + toNode.No + "' selected='true' >" + toNode.Name + "</option>");
                opt.data(toNode);
            } else {
                var opt = $("<option value='" + toNode.No + "'>" + toNode.Name + "</option>");
                opt.data(toNode);
            }

            toNodeDDL.append(opt);
        });


        $('[name=Send]').after(toNodeDDL);
    }
}

//根据下拉框选定的值，弹出提示信息  绑定那个元素显示，哪个元素不显示  
function ShowNoticeInputInfo() {
    var flowData = JSON.parse(jsonStr);
    var rbs = flowData.Sys_FrmRB;
    data = rbs;
    $("input[type=radio],select").bind('change', function (obj) {
        var needShowDDLids = [];
        var methodVal = obj.target.value;

        for (var j = 0; j < data.length; j++) {
            var value = data[j].IntKey;
            var noticeInfo = data[j].Tip;
            var drdlColName = data[j].KeyOfEn;

            if (obj.target.tagName == "SELECT") {
                drdlColName = 'DDL_' + drdlColName;
            } else {
                drdlColName = 'RB_' + drdlColName;
            }
            //if (methodVal == value &&  obj.target.name.indexOf(drdlColName) == (obj.target.name.length - drdlColName.length)) {
            if (methodVal == value && (obj.target.name == drdlColName)) {

                //根据下拉列表的值选择弹出提示信息
                if (noticeInfo == undefined || noticeInfo.trim() == '') {
                    break;
                }
                noticeInfo = noticeInfo.replace(/\\n/g, '<br/>')
                var selectText = '';
                if (obj.target.tagName.toUpperCase() == 'INPUT' && obj.target.type.toUpperCase() == 'RADIO') {//radio button
                    selectText = obj.target.nextSibling.textContent;
                } else {//select
                    selectText = $(obj.target).find("option:selected").text();
                }
                $($('#div_NoticeInputInfo .popover-title span')[0]).text(selectText);
                $('#div_NoticeInputInfo .popover-content').html(noticeInfo);

                var top = obj.target.offsetHeight;
                var left = obj.target.offsetLeft;
                var current = obj.target.offsetParent;
                while (current !== null) {
                    left += current.offsetLeft;
                    top += current.offsetTop;
                    current = current.offsetParent;
                }


                if (obj.target.tagName.toUpperCase() == 'INPUT' && obj.target.type.toUpperCase() == 'RADIO') {//radio button
                    left = left - 40;
                    top = top + 10;
                }
                if (top - $('#div_NoticeInputInfo').height() - 30 < 0) {
                    //让提示框在下方展示
                    $('#div_NoticeInputInfo').removeClass('top');
                    $('#div_NoticeInputInfo').addClass('bottom');
                    top = top;
                } else {
                    $('#div_NoticeInputInfo').removeClass('bottom');
                    $('#div_NoticeInputInfo').addClass('top');
                    top = top - $('#div_NoticeInputInfo').height() - 30;
                }
                $('#div_NoticeInputInfo').css('top', top);
                $('#div_NoticeInputInfo').css('left', left);
                $('#div_NoticeInputInfo').css('display', 'block');
                //$("#btnNoticeInfo").popover('show');
                //$('#btnNoticeInfo').trigger('click');
                break;
            }
        }

        $.each(needShowDDLids, function (i, ddlId) {
            $('#' + ddlId).change();
        });
    });


    $('#span_CloseNoticeInfo').bind('click', function () {
        $('#div_NoticeInputInfo').css('display', 'none');
    })

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

function SaveDtlAll() {
    return true;
}

//初始化多表单.
function InitFrmNodes(myFrmData) {

    //给当前的变量赋值.
    frmData = myFrmData;

    //首先判断是否是多表单?
    var frmNodes = myFrmData["FrmNodes"];

    if (frmNodes == undefined) {
        //不是多表单就绑定节点表单.
        BindFrm();
        return;
    }

    var frmNode = frmNodes[0];
    var paras = DearUrlParas();
    var handler = new HttpHandler("BP.WF.FrmGener_Init");
    handler.AddPara("FK_MapData", frmNode.FK_Frm);
    handler.AddPara("IsMobile", 1);
    handler.AddUrlData(paras);
    handler.AddJson(pageData);
    var data = handler.DoMethodReturnString("FrmGener_Init");
    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    frmData = JSON.parse(data);

    //绑定.
    BindFrm();
}

//绑定表单.
function BindFrm() {

    //分组信息.
    var Sys_GroupFields = frmData.Sys_GroupField;
    var Fk_MapData = frmData.Sys_MapData[0].No;
    var node = frmData.WF_Node[0];
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
            mapAttrsHtml+=figure_Template_FigureFrmCheck(frmData);
        }

        if (gf.CtrlType != 'Ath' && gf.CtrlType != 'FWC' && gf.CtrlType !='Dtl') {
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

    //增加列表展示的从表
    frmData.Sys_MapDtl.forEach(function (dtl) {
        if (dtl.MobileShowModel == 2) {
            $("#HD_CurDtl_No").val(dtl.No);
            Load_DtlInit(dtl.No + "DtlContent", dtl.No);
        }
    })
    
    if (isZDMobile == true) {
        $(".mui-table-view:last-child").css("margin-bottom", "0px");
        $(".mui-table-view:first-child").css("margin-top", "0px");
    }

    //为 DISABLED 的 TEXTAREA 加TITLE 
    var disabledTextAreas = $('#divCCForm textarea:disabled');
    $.each(disabledTextAreas, function (i, obj) {
        $(obj).attr('title', $(obj).val());
    })

    var trs = $(".mui-table-view-divider");
    var isHidden = false;
    $.each(trs, function (i, obj) {
        //获取所有跟随的同胞元素，其中有不隐藏的tr,就跳出循环
        var sibles = $(obj).nextAll();
        var isAllHidenField = false;
        if (sibles.length == 0) {
            $(obj).hide();
            return true;
        }
    });

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

    ////加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题
    if (frmData.Sys_MapData[0].IsEnableJs == 1) {
        var enName = frmData.Sys_MapData[0].No;
        try {
            var s = document.createElement('script');
            s.type = 'text/javascript';
            s.src = "../DataUser/JSLibData/" + enName + "_Self.js";
            var tmp = document.getElementsByTagName('script')[0];
            tmp.parentNode.insertBefore(s, tmp);
        }
        catch (err) {
        }

    }
    $.each($(".mui-attr-btn"), function (idx, item) {
        $(this).on("tap", function () {
            var keyOfEn = item.id.substring(4);
            var mapAttr = $.grep(frmData.Sys_MapAttr, function (attr) {
                return attr.KeyOfEn == keyOfEn;
            })[0];
            var tag = mapAttr.Tag || "";
            if (tag != "")
                tag = DealExp(tag);
            if (mapAttr.UIIsEnable == 1) {
                //执行js
                DBAccess.RunUrlReturnString(tag);
            }
            if (mapAttr.UIIsEnable == 2)
                DBAccess.RunFunctionReturnStr(tag);
            if (mapAttr.UIIsEnable != 0 && pageData.IsReadonly != 1)
                FullIt("", mapAttr.MyPK + "_FullData", "Btn_" + mapAttr.KeyOfEn, 0);
        })

    })
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
    if (frmData.Sys_FrmImgAth.length > 0) {
        try {
            var s = document.createElement('script');
            s.type = 'text/javascript';
            s.src = "./js/mui/js/feedback-page-img.js";
            var tmp = document.getElementsByTagName('script')[0];
            tmp.parentNode.insertBefore(s, tmp);
        }
        catch (err) {
        }
    }



    //根据下拉框、单选按钮的选择设置显示不显示、默认值
    ShowNoticeInputInfo();

    //显示tb 提示信息.
    // ShowTbNoticeInfo();

    //初始化复选下拉框
    var selectPicker = $('.selectpicker');
    $.each(selectPicker, function (i, selectObj) {
        var defVal = $(selectObj).attr('data-val');
        var defValArr = defVal.split(',');
        $(selectObj).selectpicker('val', defValArr);
    });

}

var flowData = null;
var bindDataString = "";
function GenerWorkNode() {
    var href = GetHrefUrl();
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');

    var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_MyFlow");
    handler.AddUrlData(urlParam);
    handler.AddPara("IsMobile", 1);
    handler.AddJson(pageData);
    var data = handler.DoMethodReturnString("GenerWorkNode");

    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    jsonStr = data;

    try {
        flowData = JSON.parse(data);
        frmData = flowData;
    } catch (err) {
        alert("GenerWorkNode转换JSON失败:" + jsonStr);
        return;
    }


    //获取没有解析的外部数据源
    var uiBindKeys = flowData["UIBindKey"];
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
                flowData[uiBindKeys[i].No] = operdata;
            }
        }

    }
    frmData = flowData;

    //设置标题.
    document.title = flowData.WF_Node[0].Name;
    document.getElementById("title").innerHTML = flowData.Sys_MapData[0].Name;

        
    //绑定表单.
    InitFrmNodes(flowData);

    //获得节点信息.
    var wf_node = flowData["WF_FrmNodeComponent"][0];

    //现在只有一条信息提示..
    var info = "";
    var h = $("#main").height() - 150;
    for (var i in flowData.AlertMsg) {
        var title = flowData.AlertMsg[i].Title;
        var alertMsg = flowData.AlertMsg[i].Msg;
        mui.alert(title + "\n" + alertMsg);
        if (title == "催办信息")
            $(".mui-popup-text").css("font-weight", "bold");
        if (title == "退回信息")
            $(".mui-popup-text").css("color", "red");
        var alertH = $(".mui-popup-text").height();

        if (alertH > h)
            $(".mui-popup-text").css("height", h).css("overflow-y", "auto");
        break;
    }


    window.addEventListener('touchmove', function (e) {
        var target = e.target;
        // 阻止冒泡
        target && (target.getAttribute('class') === 'mui-popup-text') && e.stopPropagation();
    }, true);


    //判断该节点是否启用了帮助提示 0 禁用 1 启用 2 强制提示 3 选择性提示
    HelpAlter();



    //循环组件 轨迹图 审核组件 子流程 子线程
    $('#CCForm').append(figure_Template_FigureFlowChart(wf_node));


    $('#CCForm').append(figure_Template_FigureSubFlowDtl(wf_node));
   
    //加载转向条件的下拉框.(优化去掉)
    //InitToNodeDDL(flowData);

    LoadFrmDataAndChangeEleStyle(flowData);

    var enName = flowData.Sys_MapData[0].No;
    //获得配置信息.
    var node = frmData.WF_Node[0];
    var frmNode = flowData["WF_FrmNode"];
    var flow = flowData.WF_Flow[0];
    if ((flow && flow.FlowDevModel == 1 || node.FormType == 11) && frmNode != null && frmNode[0].FrmSln == 1) {
        /*只读的方案.*/
        SetFrmReadonly();
        pageData.IsReadonly = 1;
    }
    if ($("#tbTracks").length != 0) {
        Skip.addJs("WorkOpt/WorkCheck.js?t=" + Math.random());
        getWorkCheck();
    }
    if (pageData.IsReadonly == 0) {
        //写字板
        var attrs = $.grep(flowData.Sys_MapAttr, function (item) {
            return item.UIVisible == 1 && item.UIContralType == 8 && item.UIIsEnable == 1;
        });
        if (attrs.length > 0) {
            $('head').append('<link href="./Scripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />');
            Skip.addJs("./Scripts/bootstrap/js/bootstrap.min.js");
            Skip.addJs("./Scripts/bootstrap/BootstrapUIDialog.js");
            attrs.forEach(function (attr) {
                $("#Img" + attr.KeyOfEn).on("tap", function () {
                    var keyOfEn = this.id.replace("Img", "");
                    var url = "WorkOpt/HandWriting.htm?WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&FK_Node=" + pageData.FK_Node + "&KeyOfEn=" + keyOfEn;
                    OpenBootStrapModal(url, "eudlgframe", "签字版", 400, 240, "icon-edit", false);
                    var $modal_dialog = $('.modal-dialog');
                    var m_top = ($(document).height() - $modal_dialog.height()) / 2;
                    var m_left = ($(document).width() - $modal_dialog.width()) / 2;
                    $('.modal').css({ 'margin': m_top + 'px ' + m_left+'px' });
                    
                });

            });
        }
       
    }
    //处理下拉框级联等扩展信息
    AfterBindEn_DealMapExt(flowData);
    OptionLinkOthers(flowData);
    if (pageData.IsReadonly != 1) {
        var pickdates = $("#CCForm .ccformdate");
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
                        var data = $("#" + id.replace("LAB_", "TB_")).data();
                        if (data && data.ReqDay != null && data.ReqDay != undefined)
                            ReqDays(data.ReqDay);
                    });
                }, false);
            }

        });
    }


    Common.MaxLengthError();



}

function HelpAlter() {
    //判断该节点是否启用了帮助提示 0 禁用 1 启用 2 强制提示 3 选择性提示
    var wf_node = flowData["WF_FrmNodeComponent"][0];
    var btnLab = new Entity("BP.WF.Template.BtnLab", wf_node.NodeID);
    if (btnLab.HelpRole != 0) {
        var count = 0;
        if (btnLab.HelpRole == 3) {
            var mypk = webUser.No + "_ND" + node.NodeID + "_HelpAlert";
            var userRegedit = new Entity("BP.Sys.UserRegedit");
            userRegedit.SetPKVal(mypk);
            count = userRegedit.RetrieveFromDBSources();
        }

        if (btnLab.HelpRole == 2 || (count == 0 && btnLab.HelpRole == 3)) {
            var filename = basePath + "/DataUser/CCForm/HelpAlert/" + wf_node.NodeID + ".htm";
            var htmlobj = $.ajax({ url: filename, async: false });
            if (htmlobj.status == 404)
                return;
            var str = htmlobj.responseText;
            if (str != null && str != "" && str != undefined) {
                mui.alert(str, '', "我知道了", function () {
                    //保存用户的帮助指引信息操作
                    var mypk = webUser.No + "_ND" + pageData.FK_Node + "_HelpAlert"
                    var userRegedit = new Entity("BP.Sys.UserRegedit");
                    userRegedit.SetPKVal(mypk);
                    var count = userRegedit.RetrieveFromDBSources();
                    if (count == 0) {
                        //保存数据
                        userRegedit.FK_Emp = webUser.No;
                        userRegedit.FK_MapData = "ND" + pageData.FK_Node;
                        userRegedit.Insert();
                    }
                });
                $(".mui-popup-title").hide();
            }
        }
    }
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
            loadScript("../DataUser/JSLibData/"+dtl.No+"_Self.js");
            if (gf.CtrlID == dtl.No) {
                if (dtl.ListShowModel == "2"){
                    if (dtl.UrlDtl == null || dtl.UrlDtl == undefined || dtl.UrlDtl == "") {
                        dtlHtml += "<div class='mui-table-view-cell' name='Dtl' id=Dtl_'" + dtl.No + "'>";
                        dtlHtml += "从表" + dtl.Name + "没有设置URL,请在" + dtl.FK_MapData + "_Self.js中解析";
                        dtlHtml += "</div>";
                        return dtlHtml;
                    }

                    var func = "TurnTo_Dtl(\"" + dtl.No + "\",\"" + dtl.UrlDtl + "\",\""+dtl.FK_MapData+"\")";
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
                    dtlHtml += "<div style='background-color:#efeff4' class='mui-table-view-cell' name='Dtl' id='" + dtl.No +"'>";
                    dtlHtml += "	<a  class='mui-navigate-right' data-title-type='native' href='javascript:" + func + "'><h5>"
                        + "<span id='" + dtl.No + "_Count'></span></h5>";
                    dtlHtml += "		<p style='display:inline;float:left'>" + gf.Lab +"</p><p style='display:inline;float:right;margin-right:20px'>点击查看详情</p>";
                    dtlHtml += "	</a>";
                    dtlHtml += "</div>";
                    return ;
                }
                 //列表模式展示
                if (dtl.MobileShowModel == 1) {
                    dtlHtml = GetDtlList(dtl.No);
                    return;
                }
                //平铺模式展示
                if (dtl.MobileShowModel == 2) {
                    dtlHtml += "<div style='background-color:#efeff4' class='mui-table-view-cell' name='Dtl' id='" + dtl.No + "'>";
                    dtlHtml += "<p>" + gf.Lab + "</p>";
                    dtlHtml += "<form id='" + dtl.No + "form_Dtl' action=''>";
                    dtlHtml += "<div id='" + dtl.No + "DtlContent'></div>";
                    dtlHtml += "</form>";
                    dtlHtml += "</div>";
                    return;
                    var src = basePath + "/CCMobile/CCForm/Dtl.htm?DtlNo=" + dtl.No + "&WorkID="+pageData.WorkID+"&RefPKVal=" + pageData.WorkID + "&FK_MapData=" + dtl.FK_MapData + "&IsReadonly=" + pageData.IsReadonly + "&FK_Node" + pageData.FK_Node + "&Version=1&FrmType=0";
                    dtlHtml += "<iframe style='width:100%;height:auto' name='Dtl' ID='Frame_" + dtl.No + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=none></iframe>";
                    dtlHtml += "</div>";
                    return;
                   
                }
            }
        });
    }
    return dtlHtml;
}

function TurnTo_Dtl(dtlNo,urlDtl, frmID) {
    //保存表单的数据
    Save(0);
    if (urlDtl.indexOf("?") == -1)
        urlDtl = urlDtl + "?1=1";
    urlDtl += "&EnsName=" + dtlNo + "&RefPKVal=" + pageData.WorkID + "&FK_MapData=" + frmID + "&IsReadonly=0&Version=1&FrmType=0";
    SetHref( basePath + "/" + urlDtl);
}
//打开明细表
function Dtl_ShowPage(dtlNo, dtlName) {
    $("#frmDtlTitle").html(dtlName);
    $("#HD_CurDtl_No").val(dtlNo);
    Load_DtlInit("DtlContent", dtlNo);
    viewApi.go("#frmDtl");
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

    //判断从表的属性，只读，可编辑、
    var isRead = false;
    var isInsert = false;
    var isDelete = false;
    if (sys_MapDtl.IsReadonly == "1" || pageData.IsReadonly == "1")
        isRead = true;
    if ((sys_MapDtl.IsInsert == "1" || sys_MapDtl.IsUpdate == "1") && pageData.IsReadonly != "1")
        isInsert = true;
    if (sys_MapDtl.IsDelete == "1" && pageData.IsReadonly != "1")
        isDelete = true;

    //存储消息

    var _Html = '<div class="mui-card" style="margin-bottom: 35px;">';
    _Html += '<ul class="mui-table-view">';
    for (var i = 0; i < dbDtl.length; i++) {
        _Html += '<li class="mui-table-view-cell mui-media">';
        _Html += '<a href="javascript:;">';
        var right = 15;
        if (isDelete == true) //删除
        {
            if (isInsert == true)
                right = 90;

            _Html += "<button type='button' class='mui-btn mui-btn-danger mui-btn-outlined' style='right:" + right + "px' onclick='DeleteDtl(\"" + dtlNo + "\"," + dbDtl[i].OID + ",this)'>";
            _Html += '删除';
            _Html += '<span class="mui-icon mui-icon-trash"></span>';
            _Html += '</button>';
        }
        if (isInsert == true) //编辑
        {
            _Html += "<button type='button' class='mui-btn mui-btn-success mui-btn-outlined' onclick='Dtl_InitPage(1,\"" + dtlNo + "\"," + dbDtl[i].OID + ")'>";
            _Html += '编辑';
            _Html += '<span class="mui-icon mui-icon-compose"></span>';
            _Html += '</button>';
        }
        if (isRead == true) //查看
        {
            _Html += "<button type='button' class='mui-btn mui-btn-success mui-btn-outlined' onclick='Dtl_InitPage(0,\"" + dtlNo + "\"," + dbDtl[i].OID + ")'>";
            _Html += '查看';
            _Html += '<span class="mui-iconmui-icon-search"></span>';
            _Html += '</button>';
        }
        _Html += '<div class="mui-media-body">';
        _Html += dbDtl[i][sys_MapDtl.MobileShowField];
        _Html += ' </div>';
        _Html += '</a>';
        _Html += '</li>';
    }


    _Html += '</ul>';

    if (isInsert == true) {

        _Html += "<button type='button' id='Dtl_" + dtlNo + "' class='mui-btn mui-btn-primary mui-btn-block' style='width:95%;margin: 15px 10px; height: 46px; padding: 0px;' onclick='Dtl_InitPage(2,\"" + dtlNo + "\",0)'>";
        _Html += "添加" + sys_MapDtl.Name;
        _Html += "</button > ";

    }

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
    if (frmData.Sys_FrmImgAthDB) {
        $.each(frmData.Sys_FrmImgAthDB, function (i, obj) {
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
    var wf_node = flowData["WF_FrmNodeComponent"][0];
    var sta = wf_node.FWCSta;
    if (sta == 0)
        return "";
    var eleHtml = "	<div class='mui-table-view-divider'><h5>" + wf_node.FWCTypeText + "</h5></div>";
    eleHtml += '<div  id="FFWC' + wf_node.NodeID + '" >' + "<div style='padding: 2px; width: 100%;'><table id='tbTracks'  style='border:1px solid #d6dde6;font-size:14px;padding: 0px; width: 100%;'></table></div>" + '</div>';
   
    return eleHtml;
}


//子流程
function figure_Template_FigureSubFlowDtl(wf_node) {
    var sta = wf_node.SFSta;
    if (sta == 0 || sta == '0')
        return $('');
    var func = "initModal(\"SubFlow\")";
    var eleHtml = "<div class='mui-table-view-cell'>";
    eleHtml += "<a class='mui-navigate-right' data-title-type='native' href='javascript:" + func + "'><h5>启动子流程</h5>";
    eleHtml += "	</a>";
    eleHtml += "</div>";
    return $(eleHtml);
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

function figure_Template_MsgAlert(msgAlert, i) {
    var eleHtml = $('<div></div>');
    var titleSpan = $('<span class="titleAlertSpan"> ' + (parseInt(i) + 1) + "&nbsp;&nbsp;&nbsp;" + msgAlert.Title + '</span>');
    var msgDiv = $('<div>' + msgAlert.Msg + '</div>');
    eleHtml.append(titleSpan).append(msgDiv);
    return eleHtml;
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
var webUser = new WebUser();

//Hide_IsShowTrack:是否隐藏该条信息,CommonShowConfig.js中定义
if ("undefined" == typeof Hide_IsShowTrack) {
    var Hide_IsShowTrack = true;
};

//从MyFlowFree2017.htm 中拿过过的.
$(function () {
    var frm = document.forms["divCCForm"];
    if (plant == "CCFlow")
        frm.action = "MyFlow.ashx?method=login&IsMobile=1";
    else
        frm.action = MyFlow + "?method=login";

    initPageParam(); //初始化参数

    InitToolBar("MyFlow"); //工具栏.ajax

    GenerWorkNode(); //表单数据.ajax

    //打开表单检查正则表达式
    if (typeof FormOnLoadCheckIsNull != 'undefined' && FormOnLoadCheckIsNull instanceof Function) {
        FormOnLoadCheckIsNull();
    }
});

function BackToHome() {
    SetHref('../CCMobilePortal/Home.htm?UserNo=' + GetQueryString('UserNo') + "&Token=" + GetQueryString("Token"));
}

function BackToTodolist() {
    SetHref('Todolist.htm?UserNo=' + GetQueryString('UserNo') + "&Token=" + GetQueryString("Token"));
}

function BackToStart() {
    SetHref('Start.htm?UserNo=' + GetQueryString('UserNo') + "&Token=" + GetQueryString("Token"));
}

//@浙商银行
function SetFrmReadonly() {
    $('#CCForm').find('input,textarea,select').attr('disabled', false);
    $('#CCForm').find('input,textarea,select').attr('readonly', true);
    $('#CCForm').find('input,textarea,select').attr('disabled', true);
    $('#Btn_Save').attr('disabled', true);
}

function setHandWriteSrc(HandWriteID, imagePath, type) {
    if (type == 0) {
        imagePath = "../" + imagePath.substring(imagePath.indexOf("DataUser"));
        document.getElementById("Img" + HandWriteID).src = "";
        $("#Img" + HandWriteID).attr("src", imagePath);
        $("#TB_" + HandWriteID).val(imagePath);
    }
    if (type == 1) {
        $("#Img_" + HandWriteID).attr("src", imagePath);
        if ("undefined" != typeof writeImg)
            writeImg = imagePath;
    }

    $('#bootStrapdlg').modal('hide');
}