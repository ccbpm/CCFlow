/// <reference path="easyUI/jquery-1.8.0.min.js" />
/// <reference path="easyUI/jquery.easyui.min.js" />

//added by liuxc,2014-12-1
//此文件可用于存放EasyUI的公用JS方法，建议都给JS方法加上注释，Demo如下


function InitOKCancelText() {

    if ($.messager == null || $.messager == undefined)
        return;
    if ($.messager.defaults == null && $.messager.defaults == undefined)
        $.messager.defaults.ok = "确定";

    if ($.messager.defaults == null && $.messager.defaults == undefined)
        $.messager.defaults.cancel = "取消";
}

function OpenEasyUiDialog(url, iframeId, dlgTitle, dlgWidth, dlgHeight, dlgIcon, showBtns, okBtnFunc, okBtnFuncArgs, dockObj, dlgClosedFunc) {

    InitOKCancelText();
    ///<summary>使用EasyUiDialog打开一个页面</summary>
    ///<param name="url" type="String">页面链接</param>
    ///<param name="iframeId" type="String">嵌套url页面的iframe的id，在okBtnFunc中，可以通过document.getElementById('eudlgframe').contentWindow获取该页面，然后直接调用该页面的方法，比如获取选中值</param>
    ///<param name="dlgTitle" type="String">Dialog标题</param>
    ///<param name="dlgWidth" type="int">Dialog宽度</param>
    ///<param name="dlgHeight" type="int">Dialog高度</param>
    ///<param name="dlgIcon" type="String">Dialog图标，必须是一个样式class</param>
    ///<param name="showBtns" type="Boolean">Dialog下方是否显示“确定”“取消”按钮，如果显示，则后面的okBtnFunc参数要填写</param>
    ///<param name="okBtnFunc" type="Function">点击“确定”按钮调用的方法</param>
    ///<param name="okBtnFuncArgs" type="Object">okBtnFunc方法使用的参数</param>
    ///<param name="dockObj" type="Object">Dialog绑定的dom对象，随此dom对象有尺寸变化而变化，如：document.getElementById('mainDiv')</param>
    ///<param name="dlgClosedFunc" type="Function">窗体关闭调用的方法（注意：此方法中不能再调用dialog中页面的内容）</param>

    //    var inIframe = window.frameElement != null && window.frameElement.nodeName == 'IFRAME',
    //        doc = inIframe ? window.parent.document : window.document;
    var dlg = $('#eudlg');
    var isTheFirst;

    if (dlg.length == 0) {
        isTheFirst = true;
        var divDom = document.createElement('div');
        divDom.setAttribute('id', 'eudlg');
        document.body.appendChild(divDom);
        dlg = $('#eudlg');
        //dlg.append("<iframe frameborder='0' src='' scrolling='auto' id='" + iframeId + "' style='width:100%;height:100%'></iframe>");
    }

	if (dlg.length == 1 && $('#' + iframeId).length == 0) {
		dlg.children().remove();
		dlg.empty();
		dlg.html("");
		dlg.append("<iframe frameborder='0' src='' scrolling='auto' id='" + iframeId + "' style='width:100%;height:97%'></iframe>");
    }
	if (dlg.length == 1 && $('#' + iframeId).length == 0) {
		console.log("never happend");
		dlg.html("<iframe frameborder='0' src='' scrolling='auto' id='" + iframeId + "' style='width:100%;height:97%'></iframe>");
    }

	if (typeof window.doCloseDialog != "function") {
		window.doCloseDialog = function () {
	        dlg.dialog("close");
	    };
		// 当弹出框未获得焦点时触发
		$(document).bind("keyup", function (e) {
			e = e || window.event;    
			var key = e.keyCode || e.which || e.charCode; 
			if (key == 27) {
				if (typeof doCloseDialog === 'function') {
					doCloseDialog.call();
				}
			}
		});
	}

    //此处为防止在一个页面使用多次此方法时，传进的iframeId不同，造成找不到非第一次创建的iframe的错误而设置的
    //todo:此处暂时有问题，同一个页面调用此方法，传进的iframeId必须相同，否则会出现问题，此问题有待以后解决
//    if ($('#' + iframeId).length == 0) {
//        dlg.empty();
//        dlg.append("<iframe frameborder='0' src='' scrolling='auto' id='" + iframeId + "' style='width:100%;height:100%'></iframe>");
//    }

    //处理定位外层容器尺寸变化事件
    if (dockObj != null && dockObj != undefined) {
        var dobj = $(dockObj);

        dlgWidth = dobj.innerWidth() - 20;
        dlgHeight = dobj.innerHeight() - 20;

        if (isTheFirst) {
            $(dockObj).resize(function () {
                var obj = $(this);

                $('#eudlg').dialog('resize', {
                    width: obj.innerWidth() - 20,
                    height: obj.innerHeight() - 40
                });
            });
        }
    }

    dlg.dialog({
        title: dlgTitle,
        width: dlgWidth,
        height: dlgHeight,
        iconCls: dlgIcon,
        resizable: true,
        modal: true,
        onClose: function () {
            /*防止缓存，切换页面不能显示问题*/
            $("#eudlg").remove();

            if (dlgClosedFunc) {
                dlgClosedFunc();
            }
        },
        cache: false
    });

    if (showBtns && okBtnFunc) {
        dlg.dialog({
            buttons: [{
                text: '确定',
                handler: function () {
                    if (okBtnFunc(okBtnFuncArgs) == false) {
                        return;
                    }

                    dlg.dialog('close');
                    $('#' + iframeId).attr('src', '');
                }
            }, {
                text: '取消',
                handler: function () {
                    dlg.dialog('close');
                    $('#' + iframeId).attr('src', '');
                }
            }]
        });
    }
    else {
        dlg.dialog({
            buttons: null
        });
    }

    dlg.dialog('open');
    $('#' + iframeId).attr('src', url);
}

function OpenEasyUiSampleEditDialog(editPropertyName, editType, oldValue, okBtnFunc, okBtnFuncArgs, isMultiLine, dlgIcon, dlgWidth, dlgHeight) {

    InitOKCancelText();

    ///<summary>使用EasyUiDialog打开一个含有1个字段文本框的编辑页面</summary>
    ///<param name="editPropertyName" type="String">当前编辑的字段中文名称</param>
    ///<param name="editType" type="String">编辑类型，比如：新建、编辑等</param>
    ///<param name="oldValue" type="String">编辑时，字段的旧文本值</param>
    ///<param name="okBtnFunc" type="Function">点击“确定”按钮调用的方法</param>
    ///<param name="okBtnFuncArgs" type="Object">okBtnFunc方法使用的参数</param>
    ///<param name="isMultiLine" type="Boolean">是否显示多行输入文本框</param>
    ///<param name="dlgIcon" type="String">Dialog图标，必须是一个样式class</param>
    ///<param name="dlgWidth" type="int">Dialog宽度</param>
    ///<param name="dlgHeight" type="int">Dialog高度</param>
    var dlgId = isMultiLine ? 'eumeditdlg' : 'eueditdlg',
        dlgLableId = dlgId + 'label',
        dlgTxtId = dlgId + 'txt',
        dlg = $('#' + dlgId),
        dw = dlgWidth || 380,
        dh = dlgHeight || 160 + (isMultiLine ? 24 * 3 : 24),
        tw = dw - 56,
        th = dh - 160;

    if (dlg.length == 0) {
        var divDom = document.createElement('div');
        divDom.setAttribute('id', dlgId);
        document.body.appendChild(divDom);
        dlg = $('#' + dlgId);
        dlg.append("<div style='padding:10px'><span id='" + dlgLableId + "'>请输入" + editPropertyName + "</span>:<br />" +
            "<" + (isMultiLine ? "textarea" : "input type='text'") + " id='" + dlgTxtId + "' style='width:" + tw + "px;height:" + th + "px;line-height:24px;' /></div>");
        //选中处理
        $('#' + dlgTxtId).focus(function () {
            this.select();
        });
    }
    else {
        $('#' + dlgLableId).text('请输入' + editPropertyName);
        $('#' + dlgTxtId).css('width', tw + 'px').css('height', th + 'px');
    }

    if (oldValue) {
        $('#' + dlgTxtId).val(oldValue);
    }

    dlg.dialog({
        title: editType + editPropertyName,
        width: dw,
        height: dh,
        iconCls: dlgIcon,
        resizable: false,
        modal: true,
        buttons: [{
            text: '确定',
            handler: function () {
                if (okBtnFunc) {
                    if (okBtnFunc($('#' + dlgTxtId).val(), okBtnFuncArgs) == false) {
                        $('#' + dlgTxtId).focus();
                        return;
                    }

                    dlg.dialog('close');
                    $('#' + dlgTxtId).val('');
                }
            }
        }, {
            text: '取消',
            handler: function () {
                dlg.dialog('close');
                $('#' + dlgTxtId).val('');
            }
        }]
    });

    dlg.dialog('open');
    $('#' + dlgTxtId).focus();
}

function OpenEasyUiDialogForSingleHtml(url, dlgTitle, dlgWidth, dlgHeight, dlgIcon, showBtns, okBtnFunc, okBtnFuncArgs) {

    InitOKCancelText();

    ///<summary>使用EasyUiDialog打开一个页面</summary>
    ///<param name="url" type="String">页面链接</param>
    ///<param name="dlgTitle" type="String">Dialog标题</param>
    ///<param name="dlgWidth" type="int">Dialog宽度</param>
    ///<param name="dlgHeight" type="int">Dialog高度</param>
    ///<param name="dlgIcon" type="String">Dialog图标，必须是一个样式class</param>
    ///<param name="showBtns" type="Boolean">Dialog下方是否显示“确定”“取消”按钮，如果显示，则后面的okBtnFunc参数要填写</param>
    ///<param name="okBtnFunc" type="Function">点击“确定”按钮调用的方法</param>
    ///<param name="okBtnFuncArgs" type="Object">okBtnFunc方法使用的参数</param>

    var dlg = $('#euhtmldlg');
    var isTheFirst;

    if (dlg.length == 0) {
        isTheFirst = true;
        var divDom = document.createElement('div');
        divDom.setAttribute('id', 'euhtmldlg');
        document.body.appendChild(divDom);
        dlg = $('#euhtmldlg');
    }

    dlg.dialog({
        title: dlgTitle,
        width: dlgWidth || 800,
        height: dlgHeight || 495,
        iconCls: dlgIcon,
        resizable: true,
        modal: true,
        href: url,
        cache: false
    });

    if (showBtns && okBtnFunc) {
        dlg.dialog({
            buttons: [{
                text: '确定',
                iconCls: 'icon-save',
                handler: function () {
                    okBtnFunc(okBtnFuncArgs)
                    dlg.dialog('close');
                    //dlg.dialog({ href: '' });
                }
            }, {
                text: '取消',
                iconCls: 'icon-cancel',
                handler: function () {
                    dlg.dialog('close');
                    //dlg.dialog({ href: '' });
                }
            }]
        });
    }
    else {
        dlg.dialog({
            buttons: null,
            onClose: function () {
                //dlg.dialog({ href: '' });
            }
        });
    }

    dlg.dialog('open');
}

//打开.
function OpenEasyUiDialogExt(url, title, w, h, isReload) {

    OpenEasyUiDialog(url, "eudlgframe", title, w, h, "icon-property", true, null, null, null, function () {
        if (isReload == true) {
            Reload();
        }
    });
}

function OpenEasyUiDialogExtCloseFunc(url, title, w, h, closeFunc) {
    OpenEasyUiDialog(url, "eudlgframe", title, w, h, "icon-property", true, null, null, null, closeFunc);
}

function OpenEasyUiConfirm(msg, okBtnFunc, okBtnFuncArgs) {
    InitOKCancelText();

    ///<summary>打开EasyUiConfirm确认框</summary>
    ///<param name="msg" type="String">确认消息</param>
    ///<param name="okBtnFunc" type="Function">点击“确定”按钮调用的方法</param>
    ///<param name="okBtnFuncArgs" type="Object">okBtnFunc方法使用的参数</param>
    $.messager.confirm('询问', msg, function (r) {
        if (r && okBtnFunc) {
            okBtnFunc(okBtnFuncArgs);
        }
    });
}

function EasyUiMenuShowForCheckedItems(menuid, itemChecks) {

    InitOKCancelText();

    ///<summary>EasyUi Menu中的checkbox项的显示菜单状态处理方法</summary>
    ///<param name="menuid" type="String">menu的id</param>
    ///<param name="itemChecks" type="Array">各checkbox item的选中信息，格式[{id:'各checkbox item id',checked:true},...]</param>
    var menu = $('#' + menuid)[0];

    $.each(itemChecks, function () {
        $('#' + menuid).menu('setIcon', { target: $('#' + this.id)[0], iconCls: this.checked ? 'icon-ok' : 'icon-xxxx' });
    });
}

function EasyUiCheckedMenuItemClick(menuid, checkitemid) {

    InitOKCancelText();

    ///<summary>EasyUi Menu中的checkbox项的按钮选中/取消选中单击处理方法，此处checkbox项是一个虚拟的，本身easyui中不存在</summary>
    ///<param name="menuid" type="String">menu的id</param>
    ///<param name="checkitemid" type="String">复选项的itemid</param>
    var item = $('#' + menuid).menu('getItem', $('#' + checkitemid)[0]),
        newIconCls = 'icon-xxxx';

    if (!item.iconCls || item.iconCls != 'icon-ok') {
        newIconCls = 'icon-ok';
    }

    $('#' + menuid).menu('setIcon', { target: $('#' + checkitemid)[0], iconCls: newIconCls });
}

function EasyUiMenuItemsCheckOnlyOne(menuid, submenuid, checkitemid, groupItemIdPrefix) {

    InitOKCancelText();

    ///<summary>操作EasyUi Menu中的多checkbox项选一项的处理方法，要求这多个item的id前缀一致</summary>
    ///<param name="menuid" type="String">menu的id</param>
    ///<param name="submenuid" type="String">如果这些项处于menu的一个子级菜单中，请填写这个子级菜单项的项级item的id</param>
    ///<param name="checkitemid" type="String">中选中项的itemid</param>
    ///<param name="groupItemIdPrefix" type="String">id前缀</param>
    var menu = submenuid ? $('#' + submenuid)[0] : $('#' + menuid)[0];

    for (var i = 0, j = menu.children.length; i < j; i++) {
        if (menu.children[i].className.indexOf('menu-item') == -1) continue;

        if (menu.children[i].id == checkitemid) {
            $('#' + menuid).menu('setIcon', { target: menu.children[i], iconCls: 'icon-ok' });
        }
        else if(groupItemIdPrefix && menu.children[i].id.length > groupItemIdPrefix.length && menu.children[i].id.substr(0, groupItemIdPrefix.length) == groupItemIdPrefix) {
            $('#' + menuid).menu('setIcon', { target: menu.children[i], iconCls: 'icon-xxxx' });    //此处的icon-xxxx本身是不存在的，但如果此处不写的话，easyui的设置图标方法会神经性的失效，原因不明
        }
    }
}

