/*
* 设计器私有的配置说明 
* 一
* UE.leipiFormDesignUrl  插件路径
* 
* 二
*UE.getEditor('myFormDesign',{
*          toolleipi:true,//是否显示，设计器的清单 tool
*/
UE.leipiFormDesignUrl = 'formdesign';
/**
 * 文本框
 * @command textfield
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'textfield');
 * ```
 */
UE.plugins['text'] = function () {
    var me = this, thePlugins = 'text';
    me.commands[thePlugins] = {
        execCommand: function (method,dataType) {
            var dialog = new UE.ui.Dialog({
                iframeUrl:'./DialogCtr/FrmTextBox.htm?FK_MapData='+pageParam.fk_mapdata+'&DataType='+dataType,
                name: thePlugins,
                editor: this,
                title: '文本框',
                cssRules: "width:600px;height:310px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'edui-cancelbutton',
                        label: '取消',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();

        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand("edit", this.anchorEl.getAttribute("data-type"), this.anchorEl);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                 //在Sys_MapAttr、Sys_MapExt中删除除控件属性
                var keyOfEn = this.anchorEl.getAttribute("data-key");
                if (keyOfEn == null || keyOfEn == undefined) {
                    alert('字段没有获取到，请联系管理员');
                    return false;
                }
                var mapAttr = new Entity("BP.Sys.MapAttr", pageParam.fk_mapdata + "_" + keyOfEn);
                mapAttr.Delete();
                var mapExt = new Entities("BP.Sys.MapExts");
                mapExt.Delete("FK_MapData", pageParam.fk_mapdata, "AttrOfOper", keyOfEn);

                //删除富文本中html
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
               
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/input/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var html = popup.formatHtml(
                '<nobr>文本框: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};

UE.plugins['edit'] = function () {
    var me = this, thePlugins = 'edit';
    me.commands[thePlugins] = {
        execCommand: function (method, datatype, obj) {
            if (obj != null) {
                var keyOfEn = obj.getAttribute("data-key");

                if (keyOfEn == null || keyOfEn == undefined || keyOfEn == "") {
                    alert('字段没有获取到，请联系管理员');
                    return false;
                }
                showFigurePropertyWin(datatype, keyOfEn, pageParam.fk_mapdata);

            }
        }
    };
}

function showFigurePropertyWin(shap, mypk,fk_mapdata) {

    if (shap == 'Text') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrString&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段String属性');
        return;
    }

    if (shap == 'Date') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段Date属性');
        return;
    }

    if (shap == 'DateTime') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段DateTime属性');
        return;
    }

    if (shap == 'Money') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段Money属性');
        return;
    }

    if (shap == 'Double') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段Double属性');
        return;
    }

    if (shap == 'Int') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段Int属性');
        return;
    }

    if (shap == 'Float') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段Float属性');
        return;
    }

    if (shap == 'Radio' || shap =='EnumSelect') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段Enum属性');
        return;
    }

    if (shap == 'CheckBox') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrBoolen&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段 Boolen 属性');
        return;
    }

    if (shap == 'BPClass' || shap == "CreateTable" || shap =="TableOrView") {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFTable&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段外键属性');
        return;
    }
    if (shap == 'SQL' || shap == "Handler" || shap == "JQuery") {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFSQL&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段外部数据源属性');
        return;
    }

    if (shap == 'Dtl') {
        var url = '../../Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=' + fk_mapdata + '&FK_MapDtl=' + mypk;
        var W = leipiEditor.body.clientWidth - 40;
        var H = leipiEditor.body.clientHeight - 40;
        CCForm_ShowDialog(url, '从表/明细表' + mypk + '属性', W, H);
        return;
    }

    if (shap == 'Image') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtImg&PKVal=' + mypk;
        CCForm_ShowDialog(url, '图片' + mypk + '属性');
        return;
    }

    if (shap == 'Button') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmBtn&PKVal=' + mypk;
        CCForm_ShowDialog(url, '按钮' + fmypk + '属性');
        return;
    }



    if (shap == 'AthMulti') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&PKVal=' + mypk;
        CCForm_ShowDialog(url, '多附件属性');
        return;
    }

    if (shap == 'AthImg') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtImgAth&PKVal='+ mypk;
        CCForm_ShowDialog(url, '图片附件');
        return;
    }

    //流程类的组件.
    if (shap == 'FlowChart') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.FrmTrack&PKVal=' + fk_mapdata.replace('ND', '') + '&tab=轨迹组件';
        CCForm_ShowDialog(url, '轨迹组件');
        return;
    }

    if (shap == 'FrmCheck') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.FrmWorkCheck&PKVal=' + fk_mapdata.replace('ND', '') + '&tab=子线程组件';
        CCForm_ShowDialog(url, '审核组件');
        return;
    }

    if (shap == 'SubFlow') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.FrmSubFlow&PKVal=' + fk_mapdata.replace('ND', '') + '&tab=子线程组件';
        CCForm_ShowDialog(url, '父子流程组件');
        return;
    }

    
    if (shap == 'HyperLink') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmLink&PKVal=' + mypk;
        CCForm_ShowDialog(url, '超链接属性');
        return;
    }


    //枚举类型.
    if (shap == 'RadioButton') {
        mypk = mypk.replace('RB_', "");
        mypk = mypk.substr(0, mypk.lastIndexOf('_'));
        mypk = mypk.replace('_0', "");
        mypk = mypk.replace('_1', "");
        mypk = mypk.replace('_2', "");
        mypk = mypk.replace('_3', "");

        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&PKVal=' +fk_mapData + "_" + mypk;
        CCForm_ShowDialog(url, '单选按钮属性');
        return;
    }

    if (shap == 'IFrame') {


        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapFrameExt&PKVal=' + mypk;
        CCForm_ShowDialog(url, '框架');
        return;
    }

    if (shap == 'HandWriting') {


        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtHandWriting&PKVal=' + mypk;
        CCForm_ShowDialog(url, '签字版');
        return;
    }
   

    alert('没有判断的双击类型:' + shap);
}


//打开窗体
function CCForm_ShowDialog(url, title, w, h) {

    if (w == undefined)
        w = 760;

    if (h == undefined)
        h = 460;

    if (plant == 'JFlow') {
        url = url.replace('.aspx', '.jsp');
        OpenEasyUiDialog(url, 'CCForm_ShowDialog', title, w, h, 'icon-library', false);
    } else {
        OpenEasyUiDialog(url, 'CCForm_ShowDialog', title, w, h, 'icon-library', false);
    }
}


/**
 * 宏控件
 * @command macros
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'macros');
 * ```
 */
UE.plugins['macros'] = function () {
    var me = this, thePlugins = 'macros';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: this.options.UEDITOR_HOME_URL + UE.leipiFormDesignUrl + '/macros.html',
                name: thePlugins,
                editor: this,
                title: '宏控件',
                cssRules: "width:600px;height:270px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'edui-cancelbutton',
                        label: '取消',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/input/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var html = popup.formatHtml(
                '<nobr>宏控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};

/**
 * 单选框组
 * @command radios
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'radio');
 * ```
 */
UE.plugins['enum'] = function () {
    var me = this, thePlugins = 'enum';
    me.commands[thePlugins] = {
        execCommand: function (method,dataType) {
            var dialog = new UE.ui.Dialog({
                iframeUrl: './DialogCtr/FrmEnumeration.htm?FK_MapData=' + pageParam.fk_mapdata+"&DataType="+dataType ,
                name: thePlugins,
                editor: this,
                title: '单选框',
                cssRules: "width:590px;height:370px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'edui-cancelbutton',
                        label: '取消',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand("edit", this.anchorEl.getAttribute("data-type"), this.anchorEl);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                 //在Sys_MapAttr、Sys_MapExt中删除除控件属性
                var keyOfEn = this.anchorEl.getAttribute("data-key");
                if (keyOfEn == null || keyOfEn == undefined) {
                    alert('字段没有获取到，请联系管理员');
                    return false;
                }
                var mapAttr = new Entity("BP.Sys.MapAttr", pageParam.fk_mapdata + "_" + keyOfEn);
                mapAttr.Delete();
                var mapExt = new Entities("BP.Sys.MapExts");
                mapExt.Delete("FK_MapData", pageParam.fk_mapdata, "AttrOfOper", keyOfEn);

                //删除富文本中html
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);

            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/span/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var type = el.getAttribute('data-type');
            var html = "";
            if (type == 'EnumSelect')
                html = popup.formatHtml(
                    '<nobr>单选下拉菜单: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            else
                html = popup.formatHtml(
                    '<nobr>单选框组: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                var elInput = el.getElementsByTagName("input");
                var rEl = elInput.length > 0 ? elInput[0] : el;
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(rEl);
            } else {
                popup.hide();
            }
        }
    });
};

/**
 * 多行文本框
 * @command textarea
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'textarea');
 * ```
 */
UE.plugins['textarea'] = function () {
    var me = this, thePlugins = 'textarea';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: this.options.UEDITOR_HOME_URL + UE.leipiFormDesignUrl + '/textarea.html',
                name: thePlugins,
                editor: this,
                title: '多行文本框',
                cssRules: "width:600px;height:330px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'edui-cancelbutton',
                        label: '取消',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        if (/textarea/ig.test(el.tagName)) {
            var html = popup.formatHtml(
                '<nobr>多行文本框: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};
/**
 * 下拉菜单
 * @command select
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'select');
 * ```
 */
UE.plugins['select'] = function () {
    var me = this, thePlugins = 'select';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: './DialogCtr/SFList.htm?FK_MapData=' + pageParam.fk_mapdata,
                name: thePlugins,
                editor: this,
                title: '下拉菜单',
                cssRules: "width:590px;height:370px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'edui-cancelbutton',
                        label: '取消',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand("edit", this.anchorEl.getAttribute("data-type"), this.anchorEl);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/select|span/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var html = popup.formatHtml(
                '<nobr>下拉菜单: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                if (el.tagName == 'SPAN') {
                    var elInput = el.getElementsByTagName("select");
                    el = elInput.length > 0 ? elInput[0] : el;
                }
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });

};
/**
 * 进度条
 * @command progressbar
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'progressbar');
 * ```
 */
UE.plugins['progressbar'] = function () {
    var me = this, thePlugins = 'progressbar';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: this.options.UEDITOR_HOME_URL + UE.leipiFormDesignUrl + '/progressbar.html',
                name: thePlugins,
                editor: this,
                title: '进度条',
                cssRules: "width:600px;height:450px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'edui-cancelbutton',
                        label: '取消',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/img/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var html = popup.formatHtml(
                '<nobr>进度条: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};
/**
 * 二维码
 * @command qrcode
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'qrcode');
 * ```
 */
UE.plugins['qrcode'] = function () {
    var me = this, thePlugins = 'qrcode';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: this.options.UEDITOR_HOME_URL + UE.leipiFormDesignUrl + '/qrcode.html',
                name: thePlugins,
                editor: this,
                title: '二维码',
                cssRules: "width:600px;height:370px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'edui-cancelbutton',
                        label: '取消',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/img/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var html = popup.formatHtml(
                '<nobr>二维码: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};
/**
 * 列表控件
 * @command listctrl
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'qrcode');
 * ```
 */
UE.plugins['dtl'] = function () {
    var me = this, thePlugins = 'dtl';
    me.commands[thePlugins] = {
        execCommand: function () {
            var val = prompt('请输入从表ID，要求表单唯一。', pageParam.fk_mapdata + 'Dtl1');

            if (val == null) {
                return;
            }

            //秦 18.11.16
            if (!CheckID(val)) {
                alert("编号不符合规则");
                return;
            }

            if (val == '') {
                alert('请输入从表ID不能为空，请重新输入！');
                NewMapDtl(pageParam.fk_mapdata);
                return;
            }
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
            handler.AddPara("FK_MapData", pageParam.fk_mapdata);
            handler.AddPara("DtlNo", val);
            handler.AddPara("FK_Node", 0); //从表为原始属性的时候FK_Node=0,设置从表权限的时候FK_Node为该节点的值

            var data = handler.DoMethodReturnString("Designer_NewMapDtl");

            if (data.indexOf('err@') == 0) {
                alert(data);
                return;
            }

            var url = '../../Comm/En.htm?EnName=BP.WF.Template.MapDtlExt&FK_MapData=' + pageParam.fk_mapdata + '&No=' + data;
            OpenEasyUiDialog(url, "eudlgframe", '从表属性', 800, 500, "icon-edit", true, null, null, null, function () {
                var _html = "<img src='../CCFormDesigner/Controls/DataView/Dtl.png' style='width:67%;height:200px'  leipiplugins='dtl' data-key='"+data+"'/>"
                leipiEditor.execCommand('insertHtml', _html);
            });

        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand("edit", "Dtl", this.anchorEl);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/img/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var html = popup.formatHtml(
                '<nobr>列表控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};

/**
 * 附件控件
 * @command ath
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'qrcode');
 * ```
 */
UE.plugins['ath'] = function () {
    var me = this, thePlugins = 'ath';
    me.commands[thePlugins] = {
        execCommand: function () {

            var val = prompt('请输入附件ID:(要求是字母数字下划线，非中文等特殊字符.)', 'Ath1');
            if (val == null) {
                return;
            }

            if (val == '') {
                alert('附件ID不能为空，请重新输入！');
                return;
            }

            //秦 18.11.16
            if (!CheckID(val)) {
                alert("编号不符合规则");
                return;
            }

            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
            handler.AddPara("FK_MapData", pageParam.fk_mapdata);
            handler.AddPara("AthNo", val);
            var data = handler.DoMethodReturnString("Designer_AthNew");

            if (data.indexOf('err@') == 0) {
                alert(data);
                return;
            }

            var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&FK_MapData=' + pageParam.fk_mapdata + '&MyPK=' + data;
            OpenEasyUiDialog(url, "eudlgframe", '附件', 800, 500, "icon-edit", true, null, null, null, function () {
                var _html = "<img src='../CCFormDesigner/Controls/DataView/AthMulti.png' style='width:67%;height:200px'  leipiplugins='ath' data-key='" + data + "' />"
               leipiEditor.execCommand('insertHtml', _html);
            });

           
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand("edit", "AthMulti", this.anchorEl);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/img/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var html = popup.formatHtml(
                '<nobr>附件控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};



/**
 *控件
 * @command ath
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @example
 * ```javascript
 * editor.execCommand( 'qrcode');
 * ```
 */
UE.plugins['component'] = function () {
    var me = this, thePlugins = 'component';
    me.commands[thePlugins] = {
        execCommand: function (methode, dataType) {
            if (dataType == "Dtl") { //从表

            }
            if (dataType == "AthMulti") { //多附件

            }
            if (dataType == "AthImg") {//图片附件
                ExtImg();
            }
            if (dataType == "IFrame") {//框架
                NewFrame();
            }
            if (dataType == "Map") {//地图控件
                ExtMap();
            }

            if (dataType == "Score") {//评分
                ExtScore();
            }

            if (dataType == "HandWriting") {//手写签字版
                ExtHandWriting();
            }
            if (dataType == "SubFlow") {//父子流程
                var mypk = GetQueryString("FK_Node");

                if (mypk == null || mypk == undefined) {
                    alert('非节点表单');
                    return;
                }
                var url = '../../Comm/En.htm?EnName=BP.WF.Template.FrmSubFlow&PKVal=' + mypk + '&tab=父子流程组件';
                OpenEasyUiDialog(url, "eudlgframe", '组件', 800, 550, "icon-property", true, null, null, null, function () {
                    //加载js
                   // $("<script type='text/javascript' src='../../WorkOpt/SubFlow.js'></script>").appendTo("head");
                    var _html = "<img src='../CCFormDesigner/Controls/DataView/SubFlowDtl.png' style='width:67%;height:200px'  leipiplugins='component' data-key='" + mypk + "'  data-type='SubFlow'/>"
                    leipiEditor.execCommand('insertHtml', _html);
                    return;

                });
            }
           
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand("edit", this.anchorEl.getAttribute("data-type"), this.anchorEl);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        var dataType = el.getAttribute("data-type");
        if (/img/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var _html;
            if (dataType == "Dtl")
                _html = popup.formatHtml(
                    '<nobr>列表控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (dataType == "AthMulti")
                _html = popup.formatHtml(
                    '<nobr>附件控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (dataType == "AthImg")
                _html = popup.formatHtml(
                    '<nobr>图片控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (dataType == "IFrame")
                _html = popup.formatHtml(
                    '<nobr>框架控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (dataType == "Map")
                _html = popup.formatHtml(
                    '<nobr>地图控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (dataType == "HandWriting")
                _html = popup.formatHtml(
                    '<nobr>手写签名版控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (dataType == "SubFlow")
                _html = popup.formatHtml(
                    '<nobr>父子流程控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');

            if (_html) {
                popup.getDom('content').innerHTML = _html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};


UE.plugins['more'] = function () {
    var me = this, thePlugins = 'more';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: this.options.UEDITOR_HOME_URL + UE.leipiFormDesignUrl + '/more.html',
                name: thePlugins,
                editor: this,
                title: '玩转表单设计器，一起参与，帮助完善',
                cssRules: "width:600px;height:200px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
};
UE.plugins['error'] = function () {
    var me = this, thePlugins = 'error';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: this.options.UEDITOR_HOME_URL + UE.leipiFormDesignUrl + '/error.html',
                name: thePlugins,
                editor: this,
                title: '异常提示',
                cssRules: "width:400px;height:130px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
};
UE.plugins['leipi'] = function () {
    var me = this, thePlugins = 'leipi';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: this.options.UEDITOR_HOME_URL + UE.leipiFormDesignUrl + '/leipi.html',
                name: thePlugins,
                editor: this,
                title: '表单设计器 - 清单',
                cssRules: "width:620px;height:220px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
};
UE.plugins['leipi_template'] = function () {
    var me = this, thePlugins = 'leipi_template';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: this.options.UEDITOR_HOME_URL + UE.leipiFormDesignUrl + '/template.html',
                name: thePlugins,
                editor: this,
                title: '表单模板',
                cssRules: "width:640px;height:380px;",
                buttons: [
                    {
                        className: 'edui-okbutton',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
};

UE.registerUI('button_leipi', function (editor, uiName) {
    if (!this.options.toolleipi) {
        return false;
    }
    //注册按钮执行时的command命令，使用命令默认就会带有回退操作
    editor.registerCommand(uiName, {
        execCommand: function () {
            editor.execCommand('leipi');
        }
    });
    //创建一个button
    var btn = new UE.ui.Button({
        //按钮的名字
        name: uiName,
        //提示
        title: "表单设计器",
        //需要添加的额外样式，指定icon图标，这里默认使用一个重复的icon
        cssRules: 'background-position: -401px -40px;',
        //点击时执行的命令
        onclick: function () {
            //这里可以不用执行命令,做你自己的操作也可
            editor.execCommand(uiName);
        }
    });
    /*
        //当点到编辑内容上时，按钮要做的状态反射
        editor.addListener('selectionchange', function () {
            var state = editor.queryCommandState(uiName);
            if (state == -1) {
                btn.setDisabled(true);
                btn.setChecked(false);
            } else {
                btn.setDisabled(false);
                btn.setChecked(state);
            }
        });
    */
    //因为你是添加button,所以需要返回这个button
    return btn;
});
UE.registerUI('button_template', function (editor, uiName) {
    if (!this.options.toolleipi) {
        return false;
    }
    //注册按钮执行时的command命令，使用命令默认就会带有回退操作
    editor.registerCommand(uiName, {
        execCommand: function () {
            try {
                leipiFormDesign.exec('leipi_template');
                //leipiFormDesign.fnCheckForm('save');
            } catch (e) {
                alert('打开模板异常');
            }

        }
    });
    //创建一个button
    var btn = new UE.ui.Button({
        //按钮的名字
        name: uiName,
        //提示
        title: "表单模板",
        //需要添加的额外样式，指定icon图标，这里默认使用一个重复的icon
        cssRules: 'background-position: -339px -40px;',
        //点击时执行的命令
        onclick: function () {
            //这里可以不用执行命令,做你自己的操作也可
            editor.execCommand(uiName);
        }
    });

    //因为你是添加button,所以需要返回这个button
    return btn;
});
UE.registerUI('button_preview', function (editor, uiName) {
    if (!this.options.toolleipi) {
        return false;
    }
    //注册按钮执行时的command命令，使用命令默认就会带有回退操作
    editor.registerCommand(uiName, {
        execCommand: function () {
            try {
                leipiFormDesign.fnReview();
            } catch (e) {
                alert('leipiFormDesign.fnReview 预览异常');
            }
        }
    });
    //创建一个button
    var btn = new UE.ui.Button({
        //按钮的名字
        name: uiName,
        //提示
        title: "预览",
        //需要添加的额外样式，指定icon图标，这里默认使用一个重复的icon
        cssRules: 'background-position: -420px -19px;',
        //点击时执行的命令
        onclick: function () {
            //这里可以不用执行命令,做你自己的操作也可
            editor.execCommand(uiName);
        }
    });

    //因为你是添加button,所以需要返回这个button
    return btn;
});

UE.registerUI('button_save', function (editor, uiName) {
    if (!this.options.toolleipi) {
        return false;
    }
    //注册按钮执行时的command命令，使用命令默认就会带有回退操作
    editor.registerCommand(uiName, {
        execCommand: function () {
            try {
                SaveForm();
            } catch (e) {
                alert('leipiFormDesign.fnCheckForm("save") 保存异常');
            }

        }
    });
    //创建一个button
    var btn = new UE.ui.Button({
        //按钮的名字
        name: uiName,
        //提示
        title: "保存表单",
        //需要添加的额外样式，指定icon图标，这里默认使用一个重复的icon
        cssRules: 'background-position: -481px -20px;',
        //点击时执行的命令
        onclick: function () {
            //这里可以不用执行命令,做你自己的操作也可
            editor.execCommand(uiName);
        }
    });

    //因为你是添加button,所以需要返回这个button
    return btn;
});

//手写签名版.
function ExtHandWriting() {

    var name = window.prompt('请输入签名版名称:\t\n比如:签字版、签名', '签字版');
    if (name == null || name == undefined)
        return;

    var frmID =pageParam.fk_mapdata;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtHandWriting();
        return;
    }

    //获得ID.
    var id = StrToPinYin(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        ExtHandWriting();
        return;
    }

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 8; //手写签名版.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; // 
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();
    var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtHandWriting&MyPK=" + mapAttr.MyPK;
    OpenEasyUiDialog(url, "eudlgframe", '签字版', 800, 500, "icon-edit", true, null, null, null, function () {
        var _html = "<img src='../../../DataUser/Siganture/admin.jpg' onerror=\"this.src='../../../DataUser/Siganture/UnName.jpg'\"  style='border:0px;height:" + mapAttr.UIHeight + "px;' id='Img" + mapAttr.KeyOfEn + "' data-type='HandWriting' data-key='" + mapAttr.MyPK +"'  leipiplugins='component'/>";
        leipiEditor.execCommand('insertHtml', _html);
    });
}
//图片附件
function ExtImg() {
    var name = window.prompt('请输入图片名称:\t\n比如:肖像、头像、ICON、地图位置', '肖像');
    if (name == null || name == undefined)
        return;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", pageParam.fk_mapdata, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
         ExtImg();
        return;
    }

    //获得ID.
    var id = StrToPinYin(name);

    var mypk = pageParam.fk_mapdata + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        return;
    }
    mapAttr.FK_MapData = pageParam.fk_mapdata;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.UIContralType = 11; //FrmImg 类型的控件.
    mapAttr.MyDataType = 1;
    mapAttr.ColSpan = 0; //单元格.
    mapAttr.LGType = 0;
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();

    var en = new Entity("BP.Sys.FrmUI.ExtImg");
    en.MyPK = pageParam.fk_mapdata + "_" + id;
    en.FK_MapData = pageParam.fk_mapdata;
    en.KeyOfEn = id;

    en.ImgAppType = 0; //图片.
    en.FK_MapData = pageParam.fk_mapdata;
    en.GroupID = mapAttr.GroupID; //设置分组列.
    en.Name = name;
    en.Insert(); //插入到数据库.

    var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtImg&MyPK=" + en.MyPK;
    OpenEasyUiDialog(url, "eudlgframe", '图片附件', 800, 500, "icon-edit", true, null, null, null, function () {
        var _html = "<img src='../CCFormDesigner/Controls/DataView/AthImg.png' style='width:" + mapAttr.UIWidth + "px;height:" + mapAttr.UIHeight + "px'  leipiplugins='athimg' data-key='" + en.MyPK + "' data-type='AthImg'/>"
        leipiEditor.execCommand('insertHtml', _html);
    });
}

///框架
function NewFrame() {
    var alert = "\t\n1.为了更好的支持应用扩展,ccform可以用iFrame的地图、流程轨迹图、轨迹表的展示。";
    alert += "\t\n2.在创建一个框架后，在框架属性里设置。";
    alert += "\t\n3.请输入框架ID,要求是字母数字下划线，非中文等特殊字符。";

    var val = prompt('新建框架:' + alert, 'Frame1');

    if (val == null) {
        return;
    }

    if (val == '') {
        alert('框架ID不能为空，请重新输入！');
        NewFrame(pageParam.fk_mapdata);
        return;
    }

    var en = new Entity("BP.Sys.FrmUI.MapFrameExt");
    en.MyPK = pageParam.fk_mapdata + "_" + val;
    if (en.IsExits() == true) {
        alert("该编号[" + val + "]已经存在");
        return;
    }

    en.FK_MapData = pageParam.fk_mapdata;
    en.Name = "我的框架" + val;
    en.FrameURL = 'MapFrameDefPage.htm';
    en.H = 200;
    en.W = 200;
    en.X = 100;
    en.Y = 100;
    en.Insert();

    var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapFrameExt&FK_MapData=' + pageParam.fk_mapdata + '&MyPK=' + en.MyPK;
    OpenEasyUiDialog(url, "eudlgframe", '框架', 800, 500, "icon-edit", true, null, null, null, function () {

        var _html = "<img src='../CCFormDesigner/Controls/DataView/iFrame.png' style='width:67%;height:200px'  leipiplugins='component' data-key='" + en.MyPK + "' data-type='IFrame'/>"
        leipiEditor.execCommand('insertHtml', _html);
    });
}
//地图
function ExtMap() {
    var name = window.prompt('请输入地图名称:\t\n比如:中国地图', '地图');
    if (name == null || name == undefined)
        return;

    var frmID = pageParam.fk_mapdata;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtMap();
        return;
    }

    //获得ID.
    var id = StrToPinYin(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        ExtMap();
        return;
    }

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 4; //地图.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; // 
    mapAttr.UIWidth = 800;//宽度
    mapAttr.UIHeight = 500;//高度
    mapAttr.Insert(); //插入字段.

    var mapAttr1 = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 0;
    mapAttr1.MyPK = frmID + "_AtPara";
    mapAttr1.FK_MapData = frmID;
    mapAttr1.KeyOfEn = "AtPara";
    mapAttr1.UIVisible = 0;
    mapAttr1.Name = "AtPara";
    mapAttr1.MyDataType = 1;
    mapAttr1.LGType = 0;
    mapAttr1.ColSpan = 1; // 
    mapAttr1.UIWidth = 100;
    mapAttr1.UIHeight = 23;
    mapAttr1.Insert(); //插入字段

    mapAttr.Retrieve();
    var url = './../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtMap&MyPK=' + mapAttr.MyPK;
    OpenEasyUiDialog(url, "eudlgframe", '地图', 800, 500, "icon-edit", true, null, null, null, function () {
        var _html = "<div style='text-align:left;padding-left:0px' id='Map_" + mapAttr.KeyOfEn + "' data-type='Map' data-key='" + mapAttr.MyPK + "' leipiplugins='component'>";
        _html += "<input type='button' name='select' value='选择'  style='background: #fff;color: #545454;font - size: 12px;padding: 4px 15px;margin: 5px 3px 5px 3px;border - radius: 3px;border: 1px solid #d2cdcd;'/>";
        _html += "<input type = text style='width:200px' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' />";
        _html += "</div>";
        leipiEditor.execCommand('insertHtml', _html);
    });
}
//评分
function ExtScore() {
    var name = window.prompt('请输入评分事项名称:\t\n比如:快递速度，服务水平', '评分事项');
    if (name == null || name == undefined)
        return;

    var frmID = pageParam.fk_mapdata;
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", frmID, "Name", name);
    if (mapAttrs.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtScore();
        return;
    }

    //获得ID.
    var id = StrToPinYin(name);
    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.MyPK = mypk;
    if (mapAttr.IsExits == true) {
        alert('名称：[' + name + "]已经存在.");
        ExtScore();
        return;
    }

    var score = window.prompt('请设定总分:\t\n比如:5，10', '5');
    if (score == null || score == undefined)
        return;

    var mypk = frmID + "_" + id;
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 101; //评分控件.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = id;
    mapAttr.Name = name;
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; //
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.Tag2 = score; // 总分
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();
    var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtScore&MyPK=" + mapAttr.MyPK;
    OpenEasyUiDialog(url, "eudlgframe", '评分', 800, 500, "icon-edit", true, null, null, null, function () {
        var _html = "<div style='text-align:left;padding-left:0px'  data-type='Score' data-key='" + mapAttr.MyPK + "' leipiplugins='component'>";
        _html += "<span class='simplestar'>";

        var num = mapAttr.Tag2;
        for (var i = 0; i < num; i++) {

            _html += "<img src='../../Style/Img/star_2.png' />";
        }
        _html += "&nbsp;&nbsp;<span class='score-tips' style='vertical-align: middle;color:#ff6600;font: 12px/1.5 tahoma,arial,\"Hiragino Sans GB\",宋体,sans-serif;'><strong>" + num + "  分</strong></span>";
        _html += "</span></div>";
        leipiEditor.execCommand('insertHtml', _html);
    });
}


//全局变量
var pageParam = {};
pageParam.fk_mapdata = GetQueryString("FK_MapData");

//保存表单的htm代码
function SaveForm() {

    if (leipiEditor.queryCommandState('source'))
        leipiEditor.execCommand('source');//切换到编辑模式才提交，否则有bug


    if (leipiEditor.hasContents()) {
        leipiEditor.sync();       //同步内容


        if (typeof type !== 'undefined') {
            type_value = type;
        }
        formeditor = leipiEditor.getContent();

        //比对Sys_MapAttr,如果html存在符合我们代码规则的保存到Sys_MapAttr中
        var strs = "FID,FK_Dept,FK_Emp,FK_NY,MyNum,OID,RDT,CDT,Rec"//默认的
        var ens = new Entities("BP.Sys.MapAttrs");
        ens.Retrieve("FK_MapData", pageParam.fk_mapdata);
        var mapAttrs = {};
        $.each(ens, function (i, en) {
            if ($.isArray(mapAttrs[en.MyPK]) == false)
                mapAttrs[en.MyPK] = [];
            mapAttrs[en.MyPK].push(en);
        })

        //获取含有data-type的元素
        var inputs = leipiEditor.document.getElementsByTagName("input");
        //遍历所有的input元素属性
        $.each(inputs, function (i, tag) {
            var dataType = tag.getAttribute("data-type");
            if (dataType != null && dataType != undefined && dataType != "") {
                //判断是否保存在Sys_MapAttr中，没有则保存
                var keyOfEn = tag.getAttribute("data-key");
                var mapAttr = mapAttrs[pageParam.fk_mapdata + "_" + keyOfEn];
                if (mapAttr == undefined || mapAttr == null) {
                    if (dataType == "Radio") {
                        var uiBindKey = tag.getAttribute("data-bindKey");
                        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
                        handler.AddPara("KeyOfEn", KeyOfEn);
                        handler.AddPara("FK_MapData", pageParam.fk_mapdata);
                        handler.AddPara("EnumKey", uiBindKey);
                        var data = handler.DoMethodReturnString("SysEnumList_SaveEnumField");
                        if (data.indexOf("err@") >= 0) {
                            alert(data);
                            return;
                        }
                    }
                    var name = tag.getAttribute("data-name");
                    mapAttr = new Entity("BP.Sys.MapAttr");

                    mapAttr.MyPK = pageParam.fk_mapdata + "_" + keyOfEn;
                    mapAttr.FK_MapData = pageParam.fk_mapdata;
                    mapAttr.KeyOfEn = keyOfEn;
                    mapAttr.Name = name;
                    if (dataType == "Text")
                        dataType = 1;
                    if (dataType == "Int")
                        dataType = 2;
                    if (dataType == "Float")
                        dataType = 3
                    if (dataType == "Money")
                        dataType = 8;
                    if (dataType == "Date")
                        dataType = 6;
                    if (dataType == "DateTime")
                        dataType = 7;
                    if (dataType == "CheckBox")
                        dataType = 4;
                    mapAttr.MyDataType = dataType;
                    if (dataType == 4) {
                        mapAttr.UIContralType = 2//checkbox
                        mapAttr.LGType = 0;
                    }
                    else if (dataType == "Radio" || dataType == "Select") {
                        mapAttr.UIContralType = 1;//下拉框
                        mapAttr.LGType = 1;//枚举
                    } else {
                        mapAttr.UIContralType = 0;//TB
                        mapAttr.LGType = 0;
                    }
                    mapAttr.Insert();
                }
            }
        });
        var selects = leipiEditor.document.getElementsByTagName("select");
        $.each(selects, function (i, tag) {
            var dataType = tag.getAttribute("data-type");
            if (dataType != null && dataType != undefined && dataType != "") {
                //找到父节点
                var ptag = $(tag).parent()[0];
                var sfTable = "";
                var keyOfEn = "";
                var uiBindKey = "";
                if (ptag.tagName.toLowerCase() == "span" && (ptag.getAttribute('leipiplugins') == "select" || ptag.getAttribute('leipiplugins') == "enum")) {
                    sfTable = ptag.getAttribute("data-sfTable");
                    keyOfEn = ptag.getAttribute("data-key");
                    uiBindKey = tag.getAttribute("data-bindKey");
                }
                var mapAttr = mapAttrs[pageParam.fk_mapdata + "_" + keyOfEn];
                if (mapAttr == undefined || mapAttr == null) {
                    if (dataType == "EnumSelect") {
                        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
                        handler.AddPara("KeyOfEn", keyOfEn);
                        handler.AddPara("FK_MapData", pageParam.fk_mapdata);
                        handler.AddPara("EnumKey", uiBindKey);
                        var data = handler.DoMethodReturnString("SysEnumList_SaveEnumField");
                        if (data.indexOf("err@") >= 0) {
                            alert(data);
                            return;
                        }
                    } else {
                        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
                        handler.AddPara("KeyOfEn", keyOfEn);
                        handler.AddPara("FK_MapData", pageParam.fk_mapdata);
                        handler.AddPara("SFTable", sfTable);
                        var data = handler.DoMethodReturnString("SFList_SaveSFField");
                        if (data.indexOf("err@") >= 0) {
                            alert(data);
                            return;
                        }
                    }
                }
            }
        });


        //保存表单的html信息
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_DevelopDesigner");
        handler.AddPara("FK_MapData", pageParam.fk_mapdata);
        handler.AddPara("HtmlCode", formeditor);

        var data = handler.DoMethodReturnString("SaveForm");
        if (data.indexOf("err@") != -1) {
            alert(data);
            return;
        }


    } else {
        alert('表单内容不能为空！')
        return false;
    }
}
//预览
function PreviewForm() {
    if (leipiEditor.queryCommandState('source'))
        leipiEditor.execCommand('source');//切换到编辑模式才提交，否则有bug

    if (leipiEditor.hasContents()) {
        leipiEditor.sync();       //同步内容
        document.saveform.target = "mywin";
        window.open('', 'mywin', "menubar=0,toolbar=0,status=0,resizable=1,left=0,top=0,scrollbars=1,width=" + (screen.availWidth - 10) + ",height=" + (screen.availHeight - 50) + "\"");
        SaveForm();
    } else {
        alert('表单内容不能为空！');
        return false;
    }
}

