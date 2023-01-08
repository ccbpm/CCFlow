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
//插入回收站字段.
UE.plugins['impfrmfields'] = function () {
    var me = this, thePlugins = 'impfrmfields';
    var frmID = pageParam.fk_mapdata;
    var W = document.body.clientWidth - 120;
    var H = document.body.clientHeight - 220;
    me.commands[thePlugins] = {
        execCommand: function (method, dataType) {
            var dialog = new UE.ui.Dialog({
                iframeUrl: './Fields.html?FrmID=' + frmID,
                name: thePlugins,
                editor: this,
                title: '未用(备用)字段',
                cssRules: "width:" + W + "px;height:" + H + "px;",

            });
            dialog.render();
            dialog.open();

        }
    };
}
//导入表单模板..
UE.plugins['impfrm'] = function () {
    var me = this, thePlugins = 'impfrm';
    var frmID = pageParam.fk_mapdata;
    var W = document.body.clientWidth - 120;
    var H = document.body.clientHeight - 80;
    var url = "../FoolFormDesigner/ImpExp/Imp/Default.htm?FK_MapData=" + GetQueryString("FK_MapData") + "&FrmID=" + GetQueryString("FK_MapData") + "&DoType=FunList&FK_Flow=" + GetQueryString("FK_Flow") + "&FK_Node=" + GetQueryString("FK_Node");
    me.commands[thePlugins] = {
        execCommand: function (method, dataType) {
            var dialog = new UE.ui.Dialog({
                iframeUrl: url,
                name: thePlugins,
                editor: this,
                title: '导入',
                cssRules: "width:" + W + "px;height:" + H + "px;",

            });
            dialog.render();
            dialog.open();

        }
    };
}
//手机模板..
UE.plugins['frmmobile'] = function () {
    var me = this, thePlugins = 'frmmobile';
    var frmID = pageParam.fk_mapdata;
    var W = 400;
    var H = 600;
    var url = '../MobileFrmDesigner/Default.htm?FK_Flow=' + GetQueryString("FK_Flow") + '&FK_Node=' + GetQueryString('FK_Node') + '&FK_MapData=' + GetQueryString("FK_MapData");
    me.commands[thePlugins] = {
        execCommand: function (method, dataType) {
            var dialog = new UE.ui.Dialog({
                iframeUrl: url,
                name: thePlugins,
                editor: this,
                title: '手机表单',
                cssRules: "width:" + W + "px;height:" + H + "px;",

            });
            dialog.render();
            dialog.open();

        }
    };
}
//插入模板..
UE.plugins['template'] = function () {
    var me = this, thePlugins = 'template';
    var frmID = pageParam.fk_mapdata;
    var W = 500;
    var H = 500;
    var url = '../DevelopDesigner/Template.htm?FK_Flow=' + GetQueryString("FK_Flow") + '&FK_Node=' + GetQueryString('FK_Node') + '&FK_MapData=' + GetQueryString("FK_MapData");
    me.commands[thePlugins] = {
        execCommand: function (method, dataType) {
            var dialog = new UE.ui.Dialog({
                iframeUrl: url,
                name: thePlugins,
                editor: this,
                title: '插入模板',
                cssRules: "width:" + W + "px;height:" + H + "px;",

            });
            dialog.render();
            dialog.open();

        }
    };
}
UE.plugins['text'] = function () {
    var me = this, thePlugins = 'text';
    me.commands[thePlugins] = {
        execCommand: function (method, dataType) {
            var dialog = new UE.ui.Dialog({
                iframeUrl: './DialogCtr/FrmTextBox.htm?FK_MapData=' + pageParam.fk_mapdata + '&DataType=' + dataType,
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
                var mapAttr = new Entity("BP.Sys.MapAttr");
                mapAttr.MyPK = pageParam.fk_mapdata + "_" + keyOfEn;
                mapAttr.Delete();

                var mapExt = new Entities("BP.Sys.MapExts");
                mapExt.Delete("FK_MapData", pageParam.fk_mapdata, "AttrOfOper", keyOfEn);

                //删除富文本中html
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);

            }
            this.hide();
        },
        _setwidth: function () {
            var w = prompt("请输入：比如25(数值)或者50%(百分比)", baidu.editor.dom.domUtils.getStyle(this.anchorEl, 'width').replace("px", ""));
            var percent = new RegExp(/^(100|[1-9]?\d(\.\d\d?\d?)?)%$|0$/);
            var result = percent.test(w);
            if (result && w.indexOf("%")!=-1) {
                baidu.editor.dom.domUtils.setStyle(this.anchorEl, 'width', w);
                return;
            }

            var patrn = /^(-)?\d+(\.\d+)?$/;
            if (patrn.exec(w) == null && w != "" && w != null) {
                alert("不合法的输入");
            } else {
                baidu.editor.dom.domUtils.setStyle(this.anchorEl, 'width', w + 'px');
            }
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/input|div/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var type = el.getAttribute('data-type');
            var html = "";
            if (type == "SignCheck")
                html = popup.formatHtml(
                    '<nobr>签批组件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">宽度</span></nobr>');
            else if (type == "FlowBBS")
                html = popup.formatHtml(
                    '<nobr>评论组件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">宽度</span></nobr>');

            else if (type == "Text")
                html = popup.formatHtml(
                    '<nobr>文本框: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">宽度</span></nobr>');
            else if (type == "Int")
                html = popup.formatHtml(
                    '<nobr>整数类型: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">宽度</span></nobr>');
            else if (type == "Money")
                html = popup.formatHtml(
                    '<nobr>金额类型: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">宽度</span></nobr>');
            else if (type == "Float")
                html = popup.formatHtml(
                    '<nobr>浮点类型: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">宽度</span></nobr>');
            else if (type == "Date")
                html = popup.formatHtml(
                    '<nobr>日期类型: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">宽度</span></nobr>');
            else if (type == "DateTime")
                html = popup.formatHtml(
                    '<nobr>日期时间类型: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">宽度</span></nobr>');
            else if (type == "CheckBox")
                html = popup.formatHtml(
                    '<nobr>复选框类型: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">宽度</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
    me.addListener('keydown', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/input/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            switch (evt.keyCode) {
                case 46:
                    popup.anchorEl = el;
                    cceval(baidu.editor.utils.html(popup.formatHtml('$$._delete()')));
                    break;
                default:
            }
        }
    });
};

UE.plugins['edit'] = function () {
    var me = this, thePlugins = 'edit';
    me.commands[thePlugins] = {
        execCommand: function (method, datatype, obj) {
            if (datatype == "SubFlow" || datatype == "WorkCheck") {
                showFigurePropertyWin(datatype, null, pageParam.fk_mapdata, obj);
                return;
            }
            if (obj != null) {
                var keyOfEn = obj.getAttribute("data-key");

                if (keyOfEn == null || keyOfEn == undefined || keyOfEn == "") {
                    alert('字段没有获取到，请联系管理员');
                    return false;
                }
                showFigurePropertyWin(datatype, keyOfEn, pageParam.fk_mapdata, obj);

            }
        }
    };
}

function showFigurePropertyWin(shap, mypk, fk_mapdata, anchorEl) {

    if (shap == 'Text') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrString&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段String属性', null, null, shap, fk_mapdata + '_' + mypk, anchorEl);
        return;
    }

    if (shap == 'SignCheck') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrCheck&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段签批组件的属性', null, null, shap, fk_mapdata + '_' + mypk, anchorEl);
        return;
    }

    if (shap == 'FlowBBS') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrFlowBBS&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段评论组件的属性', null, null, shap, fk_mapdata + '_' + mypk, anchorEl);
        return;
    }


    if (shap == 'Textarea') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrString&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段大文本属性', null, null, shap, fk_mapdata + '_' + mypk, anchorEl);
        return;
    }


    if (shap == 'Date') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段Date属性', null, null, shap, fk_mapdata + '_' + mypk, anchorEl);
        return;
    }

    if (shap == 'DateTime') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrDT&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段DateTime属性', null, null, shap, fk_mapdata + '_' + mypk, anchorEl);
        return;
    }

    if (shap == 'Money') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段Money属性', null, null, shap, fk_mapdata + '_' + mypk, anchorEl);
        return;
    }

    if (shap == 'Double') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段Double属性', null, null, shap, fk_mapdata + '_' + mypk, anchorEl);
        return;
    }

    if (shap == 'Int') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段Int属性', null, null, shap, fk_mapdata + '_' + mypk, anchorEl);
        return;
    }

    if (shap == 'Float') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrNum&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段Float属性', null, null, shap, fk_mapdata + '_' + mypk, anchorEl);
        return;
    }

    if (shap == 'Radio' || shap == 'EnumSelect' || shap == 'EnumCheckBox') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段Enum属性', null, null, shap, fk_mapdata + '_' + mypk, anchorEl);
        return;
    }

    if (shap == 'CheckBox') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrBoolen&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段 Boolen 属性', null, null, shap, fk_mapdata + '_' + mypk, anchorEl);
        return;
    }

    if (shap == 'BPClass' || shap == "CreateTable" || shap == "TableOrView") {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFTable&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段外键属性', null, null, shap, fk_mapdata + '_' + mypk, anchorEl);
        return;
    }
    if (shap == 'SQL' || shap == "Handler" || shap == "JQuery" || shap == "7" || shap == "SFTable" || shap == "WebApi") {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrSFSQL&PKVal=' + fk_mapdata + '_' + mypk;
        CCForm_ShowDialog(url, '字段外部数据源属性', null, null, shap, fk_mapdata + '_' + mypk, anchorEl);
        return;
    }

    if (shap == 'Dtl') {
        var url = '../../Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapData=' + fk_mapdata + '&FK_MapDtl=' + mypk;
        var W = leipiEditor.body.clientWidth - 40;
        var H = leipiEditor.body.clientHeight - 40;
        CCForm_ShowDialog(url, '从表/明细表', W, H, shap, mypk, anchorEl);
        return;
    }

    if (shap == 'Img') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtImg&PKVal=' + mypk;
        CCForm_ShowDialog(url, '图片' + mypk + '属性', null, null, shap, mypk, anchorEl);
        return;
    }

    if (shap == 'Btn') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmBtn&PKVal=' + fk_mapdata + "_" + mypk;
        CCForm_ShowDialog(url, '按钮' + mypk + '属性', null, null, shap, fk_mapdata + "_" + mypk, anchorEl);
        return;
    }

    if (shap == 'AthShow') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&PKVal=' + fk_mapdata + "_" + mypk;
        CCForm_ShowDialog(url, '附件' + mypk + '属性', null, null, shap, fk_mapdata + "_" + mypk, anchorEl);
        return;
    }


    if (shap == 'AthMulti') {
        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.FrmAttachmentExt&PKVal=' + mypk;
        CCForm_ShowDialog(url, '多附件属性', null, null, shap, mypk, anchorEl);
        return;
    }

    if (shap == 'AthImg') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmImgAth&PKVal=' + mypk;
        CCForm_ShowDialog(url, '图片附件', null, null, shap, mypk, anchorEl);
        return;
    }

    //流程类的组件.
    if (shap == 'FlowChart') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.FrmTrack&PKVal=' + fk_mapdata.replace('ND', '') + '&tab=轨迹组件';
        CCForm_ShowDialog(url, '轨迹组件', null, null, shap, fk_mapdata.replace('ND', ''), anchorEl);
        return;
    }

    if (shap == 'WorkCheck') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.NodeWorkCheck&PKVal=' + fk_mapdata.replace('ND', '') + '&tab=审核组件';
        CCForm_ShowDialog(url, '审核组件', null, null, shap, fk_mapdata.replace('ND', ''), anchorEl);

        return;
    }

    if (shap == 'SubFlow') {
        var url = '../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.SFlow.FrmSubFlow&PKVal=' + fk_mapdata.replace('ND', '') + '&tab=父子流程组件';
        CCForm_ShowDialog(url, '父子流程组件', null, null, shap, fk_mapdata.replace('ND', ''), anchorEl);
        return;
    }

    if (shap == 'HyperLink') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmLink&PKVal=' + mypk;
        CCForm_ShowDialog(url, '超链接属性', null, null, shap, mypk, anchorEl);
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

        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrEnum&PKVal=' + fk_mapdata + "_" + mypk;

        CCForm_ShowDialog(url, '单选按钮属性', null, null, shap, fk_mapdata + "_" + mypk, anchorEl);
        return;
    }

    if (shap == 'IFrame') {

        var url = '../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapFrameExt&PKVal=' + mypk;
        CCForm_ShowDialog(url, '框架组件', null, null, shap, mypk, anchorEl);
        return;
    }

    if (shap == 'HandWriting') {

        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtHandWriting&PKVal=' + mypk;
        CCForm_ShowDialog(url, '签字版组件', null, null, shap, mypk, anchorEl);
        return;
    }

    if (shap == 'Score') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtScore&PKVal=' + mypk;
        CCForm_ShowDialog(url, '评分组件', null, null, shap, mypk, anchorEl);
        return;
    }

    if (shap == 'Map') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtImg&MyPK=' + mypk;
        CCForm_ShowDialog(url, '地图组件', null, null, shap, mypk, anchorEl);
        return;
    }


    if (shap == 'GovDocFile') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrGovDocFile&MyPK=' + fk_mapdata + "_" + mypk;
        CCForm_ShowDialog(url, '公文正文组件', null, null, shap, mypk, anchorEl);
        return;
    }

    if (shap == 'DocWord') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrDocWord&MyPK=' + fk_mapdata + "_" + mypk;
        CCForm_ShowDialog(url, '发文字号组件', null, null, shap, mypk, anchorEl);
        return;
    }

    if (shap == 'DocWordReceive') {
        var url = '../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrDocWordReceive&MyPK=' + fk_mapdata + "_" + mypk;
        CCForm_ShowDialog(url, '收文字号组件', null, null, shap, mypk, anchorEl);
        return;
    }

    alert('没有判断的双击类型:' + shap);
}


//打开窗体
function CCForm_ShowDialog(url, title, w, h, shap, MyPK, anchorEl) {

    if (w == null || w == undefined)
        w = window.innerWidth* 3/4;

    if (h == null || h == undefined)
        h = 460;

    if (shap == "Dtl") {
        var self = window.open(url);
        var loop = setInterval(function () {
            if (self != null && self.closed) {
                clearInterval(loop);
                var en = new Entity("BP.Sys.MapDtl");
                en.SetPKVal(MyPK);
                if (en.RetrieveFromDBSources() == 0)
                    UE.dom.domUtils.remove(anchorEl, false);
                self = null;
            }
        }, 1000);
        return;
    }

    //弹出框编辑属性
    OpenLayuiDialog(url, title, w, 0, "r", false, false, false, null, function () {
        switch (shap) {
            case "Text":
            case 'SignCheck':
            case "FlowBBS":
            case "Textarea":
            case "Date":
            case "DateTime":
            case "Money":
            case "Double":
            case "Int":
            case "Float":
            case "CheckBox":
            case "Radio":
            case "EnumSelect":
            case "EnumCheckBox":
            case "BPClass":
            case "CreateTable":
            case "TableOrView":
            case "SQL":
            case "Handler":
            case "JQuery":
            case "Map":
            case "Score":
            case "HandWriting":
            case "Btn":
            case "AthShow":
                var en = new Entity("BP.Sys.MapAttr");
                en.SetPKVal(MyPK);
                if (en.RetrieveFromDBSources() == 0) {
                    //删除富文本中html
                    UE.dom.domUtils.remove(anchorEl, false);
                } else {
                    if (shap == "Text" || shap == "Textarea") {
                        if (en.UIContralType == 14) { //签批组件
                            //修改显示的样式
                            UE.dom.domUtils.setAttributes(anchorEl, {
                                "data-type": "SignCheck",
                                "leipiplugins": shap
                            });
                        }

                        if (en.UIContralType == 15) {//评论组件
                            //修改显示的样式
                            UE.dom.domUtils.setAttributes(anchorEl, {
                                "data-type": "FlowBBS",
                                "leipiplugins": shap
                            });
                        }
                        if (en.UIContralType == 0) {//
                            //修改显示的样式
                            UE.dom.domUtils.setAttributes(anchorEl, {
                                "placeholder": en.Tip
                            });
                        }
                    }

                    if (shap == "SignCheck" || shap == "FlowBBS") {
                        if (en.UIContralType == 0) {
                            var attributes;
                            if (en.UIHeight <= 23) {
                                attributes = { "data-type": "Text" };
                            } else {
                                attributes = {
                                    "data-type": "Textarea",
                                    "leipiplugins": 'textarea'
                                };
                                UE.dom.domUtils.setStyle(anchorEl, 'width', '528px');
                                UE.dom.domUtils.setStyle(anchorEl, 'height', '59px');
                            }
                            //修改显示的样式
                            UE.dom.domUtils.setAttributes(anchorEl, attributes);
                        }
                    }
                    if (shap == "Btn") {
                        UE.dom.domUtils.setAttributes(anchorEl, {
                            "data-name": en.Name,
                            "value": en.Name
                        });
                    }


                }
                break;
            case "Dtl":

                break;
            case "Img":
                var en = new Entity("BP.Sys.FrmUI.ExtImg");
                en.SetPKVal(MyPK);
                if (en.RetrieveFromDBSources() == 0)
                    UE.dom.domUtils.remove(anchorEl, false);
                else{
                    UE.dom.domUtils.setStyle(anchorEl, 'width', en.UIWidth+'px');
                    UE.dom.domUtils.setStyle(anchorEl, 'height', en.UIHeight+'px');
                }
                break;
            case "Button":
                break;
            case "AthMulti":
                var en = new Entity("BP.Sys.FrmAttachment");
                en.SetPKVal(MyPK);
                if (en.RetrieveFromDBSources() == 0)
                    UE.dom.domUtils.remove(anchorEl, false);
                break;
            case "AthImg":
                var en = new Entity("BP.Sys.FrmImgAth");
                en.SetPKVal(MyPK);
                if (en.RetrieveFromDBSources() == 0)
                    UE.dom.domUtils.remove(anchorEl, false);
                break;
            case "FlowChart":
                break;
            case "WorkCheck":
                var nodeID = GetQueryString("FK_Node");
                var frmCheck = new Entity("BP.WF.Template.NodeWorkCheck", nodeID);
                if (frmCheck.FWCSta == 0)
                    UE.dom.domUtils.remove(anchorEl, false);
                break;
            case "SubFlow":
                var nodeID = GetQueryString("FK_Node");
                var subFlow = new Entity("BP.WF.Template.SFlow.FrmSubFlow", nodeID);
            //if (subFlow.SFSta == 0)
            //   UE.dom.domUtils.remove(anchorEl, false);
            case "HyperLink":
                break;
            case "IFrame":
                var en = new Entity("BP.Sys.FrmUI.MapFrameExt");
                en.SetPKVal(MyPK);
                if (en.RetrieveFromDBSources() == 0)
                    UE.dom.domUtils.remove(anchorEl, false);
                break;
        }
    });
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
        execCommand: function (method, dataType) {
            var W = document.body.clientWidth - 160;
            var H = document.body.clientHeight - 220;
            if (dataType == null || dataType == undefined)
                dataType = "Select";
            var dialog = new UE.ui.Dialog({
                iframeUrl: './DialogCtr/FrmEnumeration.htm?FK_MapData=' + pageParam.fk_mapdata + "&DataType=" + dataType,
                name: thePlugins,
                editor: this,
                title: '单选框',
                cssRules: "width:" + W + "px;height:" + H + "px;",
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
            if (this.anchorEl.tagName.toLowerCase() == "label")
                this.anchorEl = this.anchorEl.parentNode;
            if (this.anchorEl.tagName.toLowerCase() == "span")
                this.anchorEl.setAttribute("data-key", this.anchorEl.id.substr(3));
            me.execCommand("edit", this.anchorEl.getAttribute("data-type"), this.anchorEl);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                //在Sys_MapAttr、Sys_MapExt中删除除控件属性
                if (this.anchorEl.tagName.toLowerCase() == "label")
                    this.anchorEl = this.anchorEl.parentNode;
                var keyOfEn = this.anchorEl.getAttribute("data-key");
                if (keyOfEn == null || keyOfEn == undefined) {
                    alert('字段没有获取到，请联系管理员');
                    return false;
                }
                var mapAttr = new Entity("BP.Sys.MapAttr");
                mapAttr.MyPK = pageParam.fk_mapdata + "_" + keyOfEn;
                mapAttr.Delete();
                var mapExt = new Entities("BP.Sys.MapExts");
                mapExt.Delete("FK_MapData", pageParam.fk_mapdata, "AttrOfOper", keyOfEn);

                //删除富文本中html
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);

            }
            this.hide();
        },
        _setwidth: function () {
            var w = prompt("请输入：比如25(数值)或者50%(百分比)", baidu.editor.dom.domUtils.getStyle(this.anchorEl, 'width').replace("px", ""));

            var percent = new RegExp(/^(100|[1-9]?\d(\.\d\d?\d?)?)%$|0$/);
            var result = percent.test(w);
            if (result && w.indexOf("%") != -1){
                baidu.editor.dom.domUtils.setStyle(this.anchorEl, 'width', w);
                return;
            }

            var patrn = /^(-)?\d+(\.\d+)?$/;
            if (patrn.exec(w) == null && w != "" && w != null) {
                alert("不合法的输入");
            } else {
                var hh = baidu.editor.dom.domUtils.getStyle(this.anchorEl, 'width');
                baidu.editor.dom.domUtils.setStyle(this.anchorEl, 'width', w + 'px');
            }

            /*var w = prompt("请输入数值：比如25", baidu.editor.dom.domUtils.getStyle(this.anchorEl, 'width').replace("px", ""));

            var patrn = /^(-)?\d+(\.\d+)?$/;
            if (patrn.exec(w) == null && w != "" && w != null) {
                alert("不合法的输入");
            } else {
                var hh = baidu.editor.dom.domUtils.getStyle(this.anchorEl, 'width');
                baidu.editor.dom.domUtils.setStyle(this.anchorEl, 'width', w + 'px');
            }*/
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (leipiPlugins == null && $(el).parent().length > 0)
            leipiPlugins = $($(el).parent()[0]).attr('leipiplugins');

        if (/select|span|label/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var type = el.getAttribute('data-type');
            var html = "";
            if (type == 'EnumSelect')
                html = popup.formatHtml(
                    '<nobr>单选下拉菜单: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">宽度</span></nobr>');
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
                iframeUrl: './DialogCtr/FrmTextBox.htm?FK_MapData=' + pageParam.fk_mapdata + '&DataType=Textarea',
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
                var mapAttr = new Entity("BP.Sys.MapAttr");
                mapAttr.MyPK = pageParam.fk_mapdata + "_" + keyOfEn;
                mapAttr.Delete();
                var mapExt = new Entities("BP.Sys.MapExts");
                mapExt.Delete("FK_MapData", pageParam.fk_mapdata, "AttrOfOper", keyOfEn);
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        },
        _setwidth: function () {
            var w = prompt("请输入：比如25(数值)或者50%(百分比)", baidu.editor.dom.domUtils.getStyle(this.anchorEl, 'width').replace("px", ""));

            var percent = new RegExp(/^(100|[1-9]?\d(\.\d\d?\d?)?)%$|0$/);
            var result = percent.test(w);
            if (result && w.indexOf("%") != -1) {
                baidu.editor.dom.domUtils.setStyle(this.anchorEl, 'width', w);
                return;
            }

            var patrn = /^(-)?\d+(\.\d+)?$/;
            if (patrn.exec(w) == null && w != "" && w != null) {
                alert("不合法的输入");
            } else {
                var hh = baidu.editor.dom.domUtils.getStyle(this.anchorEl, 'width');
                baidu.editor.dom.domUtils.setStyle(this.anchorEl, 'width', w + 'px');
            }
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        if (/textarea/ig.test(el.tagName)) {
            var type = el.getAttribute('data-type');
            var html = "";
            if (type == "SignCheck")
                html = popup.formatHtml(
                    '<nobr>签批组件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">宽度</span></nobr>');
            else if (type == "FlowBBS")
                html = popup.formatHtml(
                    '<nobr>评论组件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">宽度</span></nobr>');

            else
                html = popup.formatHtml(
                    '<nobr>多行文本框: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">宽度</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
    me.addListener('keydown', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (/textarea/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            switch (evt.keyCode) {
                case 46:
                    popup.anchorEl = el;
                    cceval(baidu.editor.utils.html(popup.formatHtml('$$._delete()')));
                    break;
                default:
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
            var W = document.body.clientWidth - 120;
            var H = document.body.clientHeight - 120;
            var dialog = new UE.ui.Dialog({
                iframeUrl: './DialogCtr/SFList.htm?FK_MapData=' + pageParam.fk_mapdata,
                name: thePlugins,
                editor: this,
                title: '下拉菜单',
                cssRules: "width:" + W + "px;height:" + H + "px;",
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
                var mapAttr = new Entity("BP.Sys.MapAttr");
                mapAttr.MyPK = pageParam.fk_mapdata + "_" + keyOfEn;
                mapAttr.Delete();
                var mapExt = new Entities("BP.Sys.MapExts");
                mapExt.Delete("FK_MapData", pageParam.fk_mapdata, "AttrOfOper", keyOfEn);
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        },
        _setwidth: function () {
            var w = prompt("请输入：比如25(数值)或者50%(百分比)", baidu.editor.dom.domUtils.getStyle(this.anchorEl, 'width').replace("px", ""));

            var percent = new RegExp(/^(100|[1-9]?\d(\.\d\d?\d?)?)%$|0$/);
            var result = percent.test(w);
            if (result && w.indexOf("%") != -1) {
                baidu.editor.dom.domUtils.setStyle(this.anchorEl, 'width', w);
                return;
            }

            var patrn = /^(-)?\d+(\.\d+)?$/;
            if (patrn.exec(w) == null && w != "" && w != null) {
                alert("不合法的输入");
            } else {
                var hh = baidu.editor.dom.domUtils.getStyle(this.anchorEl, 'width');
                baidu.editor.dom.domUtils.setStyle(this.anchorEl, 'width', w + 'px');
            }
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var leipiPlugins = el.getAttribute('leipiplugins');
        if (leipiPlugins == null && $(el).parent().length > 0)
            leipiPlugins = $($(el).parent()[0]).attr('leipiplugins');
        if (/select|span/ig.test(el.tagName) && leipiPlugins == thePlugins) {
            var html = popup.formatHtml(
                '<nobr>下拉菜单: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span>&nbsp;&nbsp;<span onclick=$$._setwidth() class="edui-clickable">宽度</span></nobr>');
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

            var en = new Entity("BP.Sys.MapDtl");
            en.No = val;
            if (en.RetrieveFromDBSources() == 1) {
                alert("已经存在:" + val);
                return;
            }
            en.FK_Node = 0;
            en.PTable = en.No;
            en.Name = "从表" + en.No;
            en.FK_MapData = pageParam.fk_mapdata;
            en.H = 300;
            en.Insert();
            var data = en.DoMethodReturnString("IntMapAttrs");


            if (data.indexOf('err@') == 0) {
                alert(data);
                return;
            }

            var url = '../../Comm/En.htm?EnName=BP.WF.Template.Frm.MapDtlExt&FK_MapData=' + pageParam.fk_mapdata + '&No=' + data;
            OpenLayuiDialog(url, '从表属性', innerWidth / 2, 0, "r", false, false, false, null, function () {
                var _html = "<img src='../CCFormDesigner/Controls/DataView/Dtl.png' style='width:67%;height:200px'  leipiplugins='dtl' data-key='" + data + "'/>"
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
                //在Sys_MapDtl中删除除控件属性
                var no = this.anchorEl.getAttribute("data-key");
                if (no == null || no == undefined) {
                    alert('从表属性没有获取到，请联系管理员');
                    return false;
                }
                var mapDtl = new Entity("BP.Sys.MapDtl", no);
                mapDtl.No = no;
                mapDtl.Delete();

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
            OpenLayuiDialog(url, '附件', window.innerWidth / 2, 0, "r", false, false, false, null, function () {
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
                //在Sys_FrmAttachment中删除除控件属性
                var mypk = this.anchorEl.getAttribute("data-key");
                if (mypk == null || mypk == undefined) {
                    alert('附件属性没有获取到，请联系管理员');
                    return false;
                }
                var ath = new Entity("BP.Sys.FrmAttachment");
                ath.MyPK = mypk;
                ath.Delete();

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
            if (dataType == "Components") {
                var dialog = new UE.ui.Dialog({
                    //iframeUrl: './DialogCtr/Components.htm?FK_MapData=' + pageParam.fk_mapdata+"&FrmType=8" ,
                    iframeUrl: '../FoolFormDesigner/Components/Default.htm?FK_MapData=' + pageParam.fk_mapdata + '&FrmType=8',
                    name: thePlugins,
                    editor: this,
                    title: '组件',
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
            if (dataType == "Dtl") { //从表

            }
            if (dataType == "AthMulti") { //多附件

            }
            if (dataType == "Img") {//图片
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

            if (dataType == "AthImg") {//图片附件
                ExtImgAth();
            }

            if (dataType == "GovDocFile") {  //公文正文组件
                ExtGovDocFile();
            }

            if (dataType == "PrintRTF") {  //RTF打印组件
                PrintRTF();
            }

            if (dataType == "PrintHtml") {  //RTF打印组件
                PrintHtml();
            }

            if (dataType == "DocWord") {  //发文字号
                ExtDocWord();
            }

            if (dataType == "DocWordReceive") { //收文字号
                ExtDocWordReceive();
            }


            if (dataType == "Btn") {//按钮
                ExtBtn();
            }

            if (dataType == "HandWriting") {//手写签字版
                ExtHandWriting();
            }

            if (dataType == "WorkCheck") { //审核组件
                var mypk = GetQueryString("FK_Node");

                if (mypk == null || mypk == undefined) {
                    alert('非节点表单,只添加审核组件标识');
                    var _html = "<img src='../CCFormDesigner/Controls/DataView/FrmCheck.png' style='width:67%;height:200px'  leipiplugins='component'  data-type='WorkCheck'/>"
                    leipiEditor.execCommand('insertHtml', _html);

                    return;
                }
                var url = '../../Comm/EnOnly.htm?EnName=BP.WF.Template.NodeWorkCheck&PKVal=' + mypk + '&tab=审核组件';
                OpenLayuiDialog(url, '组件', innerWidth / 2, 0, "r", false, false, false, null, function () {
                    //加载js
                    // $("<script type='text/javascript' src='../../WorkOpt/SubFlow.js'></script>").appendTo("head");
                    var _html = "<img src='../CCFormDesigner/Controls/DataView/FrmCheck.png' style='width:67%;height:200px'  leipiplugins='component' data-key='" + mypk + "'  data-type='WorkCheck'/>"
                    leipiEditor.execCommand('insertHtml', _html);
                    return;

                });

            }
            if (dataType == "SubFlow") { //父子流程
                var mypk = GetQueryString("FK_Node");

                if (mypk == null || mypk == undefined) {
                    alert('非节点表单,只增加父子流程标识，属性配置请在节点属性，父子流程组件中配置');
                    var _html = "<img src='../CCFormDesigner/Controls/DataView/SubFlowDtl.png' style='width:67%;height:200px'  leipiplugins='component'   data-type='SubFlow'/>"
                    leipiEditor.execCommand('insertHtml', _html);
                    return;
                }
                var url = '../../Comm/En.htm?EnName=BP.WF.Template.SFlow.FrmSubFlow&PKVal=' + mypk + '&tab=父子流程组件';
                OpenLayuiDialog(url, '父子流程', innerWidth / 2, 0, "r", false, false, false, null, function () {
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
                var dataType = this.anchorEl.getAttribute("data-type");
                if (dataType == "SubFlow") {
                    var nodeID = GetQueryString("FK_Node");
                    var subFlow = new Entity("BP.WF.Template.SFlow.FrmSubFlow", nodeID);
                    subFlow.SFSta = 0;//禁用
                    subFlow.Update();
                    baidu.editor.dom.domUtils.remove(this.anchorEl, false);
                    return;
                }

                if (dataType == "WorkCheck") {
                    var nodeID = GetQueryString("FK_Node");
                    var frmCheck = new Entity("BP.WF.Template.NodeWorkCheck", nodeID);
                    frmCheck.FWCSta = 0;//禁用
                    frmCheck.Update();
                    baidu.editor.dom.domUtils.remove(this.anchorEl, false);
                    return;
                }
                var mypk = this.anchorEl.getAttribute("data-key");
                if (mypk == null || mypk == undefined) {
                    alert('元素属性data-key丢失，请联系管理员');
                    return false;
                }

                if (dataType == "AthImg") {
                    var imgAth = new Entity("BP.Sys.FrmImgAth", mypk);
                    imgAth.MyPK = mypk;
                    imgAth.Delete();
                }
                if (dataType == "Img") {
                    var en = new Entity("BP.Sys.FrmUI.ExtImg", mypk);
                    en.MyPK = mypk;
                    en.Delete();
                }
                if (dataType == "IFrame") {
                    var en = new Entity("BP.Sys.FrmUI.MapFrameExt");
                    en.MyPK = mypk;
                    en.Delete();
                }
                if (dataType == "Map" || dataType == "Score" || dataType == "HandWriting") {
                    var mapAttr = new Entity("BP.Sys.MapAttr");
                    mapAttr.MyPK = mypk;
                    mapAttr.Delete();
                }

                if (dataType == "GovDocFile" || dataType == "DocWord" || dataType == "DocWordReceive") {
                    var mapAttr = new Entity("BP.Sys.MapAttr", pageParam.fk_mapdata + "_" + mypk);
                    mapAttr.Delete();
                }

                if (dataType == "Btn") {
                    var mapAttr = new Entity("BP.Sys.MapAttr", pageParam.fk_mapdata + "_" + mypk);
                    mapAttr.Delete();

                    //删除相关联的按钮
                    var frmBtn = new Entity("BP.Sys.FrmUI.FrmBtn", pageParam.fk_mapdata + "_" + mypk);
                    frmBtn.Delete();
                }

                
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
        if (/img|span|input/ig.test(el.tagName.toLowerCase()) && leipiPlugins == thePlugins) {
            var _html;
            if (dataType == "Dtl")
                _html = popup.formatHtml(
                    '<nobr>列表控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (dataType == "AthMulti")
                _html = popup.formatHtml(
                    '<nobr>附件控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (dataType == "AthImg")
                _html = popup.formatHtml(
                    '<nobr>图片附件控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (dataType == "Img")
                _html = popup.formatHtml(
                    '<nobr>图片控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (dataType == "IFrame")
                _html = popup.formatHtml(
                    '<nobr>框架控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (dataType == "Map")
                _html = popup.formatHtml(
                    '<nobr>地图控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (dataType == "Score")
                _html = popup.formatHtml(
                    '<nobr>评分控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');

            if (dataType == "GovDocFile")
                _html = popup.formatHtml(
                    '<nobr>公文正文组件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');


            if (dataType == "DocWord")
                _html = popup.formatHtml(
                    '<nobr>发文字号: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');

            if (dataType == "DocWordReceive")
                _html = popup.formatHtml(
                    '<nobr>收文字号: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');


            if (dataType == "Btn")
                _html = popup.formatHtml(
                    '<nobr>按钮: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (dataType == "AthShow")
                _html = popup.formatHtml(
                    '<nobr>字段附件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');

            if (dataType == "HandWriting")
                _html = popup.formatHtml(
                    '<nobr>手写签名版控件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (dataType == "WorkCheck" && pageParam.fk_node != 0)
                _html = popup.formatHtml(
                    '<nobr>审核组件: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (dataType == "SubFlow" && pageParam.fk_node != 0)
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

//UE.registerUI('button_leipi', function (editor, uiName) {
//    if (!this.options.toolleipi) {
//        return false;
//    }
//    //注册按钮执行时的command命令，使用命令默认就会带有回退操作
//    editor.registerCommand(uiName, {
//        execCommand: function () {
//            editor.execCommand('leipi');
//        }
//    });
//    //创建一个button
//    var btn = new UE.ui.Button({
//        //按钮的名字
//        name: uiName,
//        //提示
//        title: "表单设计器",
//        //需要添加的额外样式，指定icon图标，这里默认使用一个重复的icon
//        cssRules: 'background-position: -401px -40px;',
//        //点击时执行的命令
//        onclick: function () {
//            //这里可以不用执行命令,做你自己的操作也可
//            editor.execCommand(uiName);
//        }
//    });
//    /*
//        //当点到编辑内容上时，按钮要做的状态反射
//        editor.addListener('selectionchange', function () {
//            var state = editor.queryCommandState(uiName);
//            if (state == -1) {
//                btn.setDisabled(true);
//                btn.setChecked(false);
//            } else {
//                btn.setDisabled(false);
//                btn.setChecked(state);
//            }
//        });
//    */
//    //因为你是添加button,所以需要返回这个button
//    return btn;
//});
//UE.registerUI('button_template', function (editor, uiName) {
//    if (!this.options.toolleipi) {
//        return false;
//    }
//    //注册按钮执行时的command命令，使用命令默认就会带有回退操作
//    editor.registerCommand(uiName, {
//        execCommand: function () {
//            try {
//                leipiFormDesign.exec('leipi_template');
//                //leipiFormDesign.fnCheckForm('save');
//            } catch (e) {
//                alert('打开模板异常');
//            }

//        }
//    });
//    //创建一个button
//    var btn = new UE.ui.Button({
//        //按钮的名字
//        name: uiName,
//        //提示
//        title: "表单模板",
//        //需要添加的额外样式，指定icon图标，这里默认使用一个重复的icon
//        cssRules: 'background-position: -339px -40px;',
//        //点击时执行的命令
//        onclick: function () {
//            //这里可以不用执行命令,做你自己的操作也可
//            editor.execCommand(uiName);
//        }
//    });

//    //因为你是添加button,所以需要返回这个button
//    return btn;
//});
//UE.registerUI('button_preview', function (editor, uiName) {
//    if (!this.options.toolleipi) {
//        return false;
//    }
//    //注册按钮执行时的command命令，使用命令默认就会带有回退操作
//    editor.registerCommand(uiName, {
//        execCommand: function () {
//            try {
//                leipiFormDesign.fnReview();
//            } catch (e) {
//                alert('leipiFormDesign.fnReview 预览异常');
//            }
//        }
//    });
//    //创建一个button
//    var btn = new UE.ui.Button({
//        //按钮的名字
//        name: uiName,
//        //提示
//        title: "预览",
//        //需要添加的额外样式，指定icon图标，这里默认使用一个重复的icon
//        cssRules: 'background-position: -420px -19px;',
//        //点击时执行的命令
//        onclick: function () {
//            //这里可以不用执行命令,做你自己的操作也可
//            editor.execCommand(uiName);
//        }
//    });

//    //因为你是添加button,所以需要返回这个button
//    return btn;
//});

//UE.registerUI('button_save', function (editor, uiName) {
//    if (!this.options.toolleipi) {
//        return false;
//    }
//    //注册按钮执行时的command命令，使用命令默认就会带有回退操作
//    editor.registerCommand(uiName, {
//        execCommand: function () {
//            try {
//                SaveForm();
//            } catch (e) {
//                alert('leipiFormDesign.fnCheckForm("save") 保存异常');
//            }

//        }
//    });
//    //创建一个button
//    var btn = new UE.ui.Button({
//        //按钮的名字
//        name: uiName,
//        //提示
//        title: "保存表单",
//        //需要添加的额外样式，指定icon图标，这里默认使用一个重复的icon
//        cssRules: 'background-position: -481px -20px;',
//        //点击时执行的命令
//        onclick: function () {
//            //这里可以不用执行命令,做你自己的操作也可
//            editor.execCommand(uiName);
//        }
//    });

//    //因为你是添加button,所以需要返回这个button
//    return btn;
//});

//手写签名版.
function ExtHandWriting() {

    var name = promptGener('请输入签名版名称:\t\n比如:签字版、签名', '签字版');
    if (name == null || name == undefined)
        return;

    var frmID = pageParam.fk_mapdata;
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
    OpenLayuiDialog(url, '签字版', innerWidth / 2, 0, "r", false, false, false, null, function () {
        var _html = "<img src='../../../DataUser/Siganture/admin.jpg' onerror=\"this.src='../../../DataUser/Siganture/UnName.jpg'\"  style='border:0px;height:" + mapAttr.UIHeight + "px;' id='Img" + mapAttr.KeyOfEn + "' data-type='HandWriting' data-key='" + mapAttr.MyPK + "'  leipiplugins='component'/>";
        leipiEditor.execCommand('insertHtml', _html);
    });
}

//图片附件
function ExtImgAth() {
    var name = promptGener('请输入图片附件名称:\t\n比如:肖像、头像、ICON、地图位置', '肖像');
    if (name == null || name == undefined)
        return;
    var ImgAths = new Entities("BP.Sys.FrmImgAths");
    ImgAths.Retrieve("FK_MapData", pageParam.fk_mapdata, "Name", name);
    if (ImgAths.length >= 1) {
        alert('名称：[' + name + "]已经存在.");
        ExtImgAth();
        return;
    }

    //获得ID.
    var id = StrToPinYin(name);

    var imgAth = new Entity("BP.Sys.FrmImgAth");
    imgAth.FK_MapData = pageParam.fk_mapdata;
    imgAth.CtrlID = id;
    imgAth.MyPK = pageParam.fk_mapdata + "_" + id;
    imgAth.Name = name;
    imgAth.Insert();

    var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.FrmImgAth&MyPK=" + imgAth.MyPK;
    OpenLayuiDialog(url, '图片附件', innerWidth / 2, 0, "r", false, false, false, null, function () {
        var _html = "<img src='../CCFormDesigner/Controls/DataView/AthImg.png' style='width:" + imgAth.W + "px;height:" + imgAth.H + "px'  leipiplugins='component' data-key='" + imgAth.MyPK + "' data-type='AthImg'/>"
        leipiEditor.execCommand('insertHtml', _html);
    });
}

//公文正文组件
function ExtGovDocFile() {

    var en = new Entity("BP.Sys.MapAttr");
    en.SetPKVal(pageParam.fk_mapdata + "_GovDocFile");
    if (en.RetrieveFromDBSources() == 1) {
        alert("该表单 GovDocFile 字段已经存在。");
        return;
    }

    var mypk = pageParam.fk_mapdata + "_GovDocFile";
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 110; //公文正文.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = pageParam.fk_mapdata;
    mapAttr.KeyOfEn = "GovDocFile";
    mapAttr.Name = "公文组件";
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; // 
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();
    var url = "../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrGovDocFile&MyPK=" + mapAttr.MyPK;
    OpenLayuiDialog(url, '公文组件', innerWidth / 2, 0, "r", false, false, false, null, function () {
        var _Html = "<input type='text'  id='TB_GovDocFile' name='TB_GovDocFile' data-key='GovDocFile' data-name='公文正文组件' data-type='GovDocFile'   leipiplugins='component' style='width:98%'  placeholder='公文正文组件'/>";
        leipiEditor.execCommand('insertHtml', _Html);
    });
}


//发文字号
function ExtDocWord() {
    var en = new Entity("BP.Sys.MapAttr");
    en.SetPKVal(pageParam.fk_mapdata + "_DocWord");
    if (en.RetrieveFromDBSources() == 1) {
        alert("该表单 DocWord 字段已经存在，字号默认的字段 DocWord, 请确认该字段是否为发文字号字段");
        return;
    }

    var mypk = pageParam.fk_mapdata + "_DocWord";
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 17; //发文字号.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = pageParam.fk_mapdata;
    mapAttr.KeyOfEn = "DocWord";
    mapAttr.Name = "发文字号";
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; // 
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();
    var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrDocWord&MyPK=" + mapAttr.MyPK;
    OpenLayuiDialog(url, '发文字号', innerWidth / 2, 0, "r", false, false, false, null, function () {
        var _Html = "<input type='text'  id='TB_DocWord' name='TB_DocWord' data-key='DocWord' data-name='发文字号' data-type='DocWord'   leipiplugins='component' style='width:98%'  placeholder='发文字号'/>";
        leipiEditor.execCommand('insertHtml', _Html);
    });
}

//收文字号
function ExtDocWordReceive() {

    var en = new Entity("BP.Sys.MapAttr");
    en.SetPKVal(pageParam.fk_mapdata + "_DocWordReceive");
    if (en.RetrieveFromDBSources() == 1) {
        alert("该表单 DocWordReceive 字段已经存在，收文字号默认的字段 DocWordReceive,请确认该字段是否为收文字号字段");
        return;
    }

    var mypk = pageParam.fk_mapdata + "_DocWordReceive";
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 170; //收文字号.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = pageParam.fk_mapdata;
    mapAttr.KeyOfEn = "DocWordReceive";
    mapAttr.Name = "收文字号";
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; //
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 170;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();
    var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.MapAttrDocWordReceive&MyPK=" + mapAttr.MyPK;
    OpenLayuiDialog(url, '收文字号', innerWidth / 2, 0, "r", false, false, false, null, function () {
        var _Html = "<input type='text'  id='TB_DocWordReceive' name='TB_DocWordReceive' data-key='DocWordReceive' data-name='收文字号' data-type='DocWordReceive'   leipiplugins='component' style='width:98%'  placeholder='收文字号'/>";
        leipiEditor.execCommand('insertHtml', _Html);
    });
}


//图片
function ExtImg() {
    var name = promptGener('请输入图片名称:\t\n比如:肖像、头像、ICON、地图位置', '肖像');
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
    en.UIWidth = 150;
    en.UIHeight = 170;
    en.Insert(); //插入到数据库.

    var url = "../../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtImg&MyPK=" + en.MyPK;
    OpenLayuiDialog(url, '图片', innerWidth / 2, 0, "r", false, false, false, null, function () {
        var _html = "<img src='../CCFormDesigner/Controls/basic/Img.png' style='width:" + mapAttr.UIWidth + "px;height:" + mapAttr.UIHeight + "px'  leipiplugins='component' data-key='" + en.MyPK + "' data-type='Img'/>"
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

    var en = new Entity("BP.Sys.MapFrame");
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
    OpenLayuiDialog(url, '框架', innerWidth / 2, 0, "r", false, false, false, null, function () {
        var _html = "<img src='../CCFormDesigner/Controls/DataView/iFrame.png' style='width:67%;height:200px'  leipiplugins='component' data-key='" + en.MyPK + "' data-type='IFrame'/>"
        leipiEditor.execCommand('insertHtml', _html);
    });
}
//地图
function ExtMap() {
    var name = promptGener('请输入地图名称:\t\n比如:中国地图', '地图');
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
    var url = './../Comm/EnOnly.htm?EnName=BP.Sys.FrmUI.ExtMap&MyPK=' + mapAttr.MyPK;

    OpenLayuiDialog(url, '地图', innerWidth / 2, 0, "r", false, false, false, null, function () {
        var _html = "<div style='text-align:left;padding-left:0px' id='Map_" + mapAttr.KeyOfEn + "' data-type='Map' data-key='" + mapAttr.MyPK + "' leipiplugins='component'>";
        _html += "<input type='button' name='select' value='选择'  style='background: #fff;color: #545454;font - size: 12px;padding: 4px 15px;margin: 5px 3px 5px 3px;border - radius: 3px;border: 1px solid #d2cdcd;'/>";
        _html += "<input type = text style='width:200px' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' />";
        _html += "</div>";
        leipiEditor.execCommand('insertHtml', _html);
    });
}


//评分
function ExtScore() {
    var name = promptGener('请输入评分事项名称:\t\n比如:快递速度，服务水平', '评分事项');
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

    var score = promptGener('请设定总分:\t\n比如:5，10', '5');
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
    OpenLayuiDialog(url, '评分', innerWidth / 2, 0, "r", false, false, false, null, function () {
        var _html = "<span class='score-star'style='text-align:left;padding-left:0px'  data-type='Score' data-key='" + mapAttr.MyPK + "' leipiplugins='component' id='SC_" + mapAttr.KeyOfEn + "'>";
        _html += "<span class='simplestar' data-type='Score'  leipiplugins='component'  data-key='" + mapAttr.MyPK + "' id='Star_" + mapAttr.KeyOfEn + "'>";

        var num = mapAttr.Tag2;
        for (var i = 0; i < num; i++) {

            _html += "<img src='../../Style/Img/star_2.png' data-type='Score'  leipiplugins='component'  data-key='" + mapAttr.MyPK + "'/>";
        }
        _html += "&nbsp;&nbsp;<span class='score-tips' style='vertical-align: middle;color:#ff6600;font: 12px/1.5 tahoma,arial,\"Hiragino Sans GB\",宋体,sans-serif;'><strong>" + num + "  分</strong></span>";
        _html += "</span></span>";
        leipiEditor.execCommand('insertHtml', _html);
    });
}


//全局变量
var pageParam = {};
pageParam.fk_mapdata = GetQueryString("FK_MapData");
pageParam.fk_node = GetQueryString("FK_Node");
if (pageParam.fk_node == null || pageParam.fk_node == undefined)
    pageParam.fk_node = 0;

function SaveForm() {

    $("#Btn_Save").html("正在保存请稍后.");

    try {
        Save();
    } catch (e) {
        alert(e);
        return;
    }

    $("#Btn_Save").html("保存成功");
    setTimeout(function () { $("#Btn_Save").html("保存"); }, 1000);
    // alert("保存成功.");
}
var formeditor = "";
//保存表单的htm代码
function Save() {
    //清空MapData的缓存
    var en = new Entity("BP.Sys.MapData", pageParam.fk_mapdata);
    en.SetPKVal(pageParam.fk_mapdata);
    en.DoMethodReturnString("ClearCash");

    if (leipiEditor.queryCommandState('source'))
        leipiEditor.execCommand('source');//切换到编辑模式才提交，否则有bug

    if (leipiEditor.hasContents() == false) {
        alert('表单内容不能为空！');
        return false;
    }

    $("#Btn_Save").html("正在保存...");


    leipiEditor.sync();       //同步内容


    if (typeof type !== 'undefined') {
        type_value = type;
    }


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
            var bindkey = tag.getAttribute("data-bindkey");
            if (dataType == "Radio")
                keyOfEn = $($(tag).parent()[0]).parent()[0].getAttribute("data-key");//获取父级的data-key
            if (keyOfEn != null && keyOfEn != undefined && keyOfEn != "") {
                var mapAttr = mapAttrs[pageParam.fk_mapdata + "_" + keyOfEn];
                if ((mapAttr == undefined || mapAttr == null) && keyOfEn != "" && uiBindKey != "") {
                    if (dataType == "Radio") {
                        var uiBindKey = tag.getAttribute("data-bindKey");
                        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
                        handler.AddPara("KeyOfEn", keyOfEn);
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
                    if (dataType == "SignCheck") {
                        dataType = 1;
                        mapAttr.UIContralType = 14; //签批组件
                    }

                    if (dataType == "FlowBBS") {
                        dataType = 1;
                        mapAttr.UIContralType = 15; //评论组件
                    }

                    if (dataType == "GovDocFile") {
                        dataType = 1;
                        mapAttr.UIContralType = 110;//公文正文组件.
                    }

                    if (dataType == "DocWord") {
                        dataType = 1;
                        mapAttr.UIContralType = 17;//发文字号
                    }

                    if (dataType == "DocWordReceive") {
                        dataType = 1;
                        mapAttr.UIContralType = 170;//收文字号
                    }

                    if (dataType == "Btn") {
                        dataType = 1;
                        mapAttr.UIContralType = 18; //按钮
                    }


                    mapAttr.MyDataType = dataType;
                    if (dataType == 4) {
                        //枚举复选框
                        if (bindkey == null || bindkey == undefined) {
                            mapAttr.UIContralType = 2//checkbox
                            mapAttr.MyDataType = 1;
                            mapAttr.LGType = 1;
                        } else {
                            mapAttr.UIContralType = 2//checkbox
                            mapAttr.LGType = 0;
                        }

                    } else if (dataType == "Radio" || dataType == "Select") {
                        mapAttr.UIContralType = 1;//下拉框
                        mapAttr.LGType = 1;//枚举
                    } else {
                        mapAttr.LGType = 0;
                    }
                    mapAttr.Insert();
                }


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
                keyOfEn = tag.getAttribute("data-key");
                uiBindKey = tag.getAttribute("data-bindKey");
            }
            var mapAttr = mapAttrs[pageParam.fk_mapdata + "_" + keyOfEn];
            if ((mapAttr == undefined || mapAttr == null) && keyOfEn != "" && uiBindKey != "") {
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

    //补充枚举值不全的情况
    var spans = leipiEditor.document.getElementsByTagName("span");
    for (var i = 0; i < spans.length; i++) {
        var tag = spans[i];
        var uiBindKey = tag.getAttribute("data-bindKey");
        if (uiBindKey == null || uiBindKey == undefined || uiBindKey == "")
            continue;
        if (tag.getAttribute("data-type") != "Radio")
            continue;

        //获取枚举值
        var enums = GetSysEnums(uiBindKey);

        if (enums.length == 0)
            continue;
        var keyOfEn = tag.getAttribute("data-key");
        $.each(enums, function (idx, obj) {

            if (leipiEditor.document.getElementById("RB_" + keyOfEn + "_" + obj.IntKey) == null)
                $(tag).append('<input type="radio" value="' + obj.IntKey + '" id="RB_' + keyOfEn + '_' + obj.IntKey + '" name="RB_' + keyOfEn + '" data-key="' + keyOfEn + '" data-type="Radio" data-bindkey="' + uiBindKey + '" class="form-control" style="width: 15px; height: 15px;">' + obj.Lab);
        });

    }

    //获取表单的附件，从表，图片附件，审核组件
    leipiEditor.focus(true);
    var imgs = leipiEditor.document.getElementsByTagName("Img");
    var _html = ""
    var aths = new Entities("BP.Sys.FrmAttachments");
    aths.Retrieve("FK_MapData", pageParam.fk_mapdata, "FK_Node", 0);
    $.each(aths, function (i, ath) {
        document.getElementsByTagName("Im")
        var element = getElementByAttr(imgs, "data-key", ath.MyPK);
        if (element == null)
            element = leipiEditor.document.getElementById("TB_" + ath.NoOfObj);
        //增加该元素
        if (element == null) {
            _html = "<img src='../CCFormDesigner/Controls/DataView/AthMulti.png' style='width:67%;height:200px'  leipiplugins='ath' data-key='" + ath.MyPK + "' />"
            leipiEditor.execCommand('insertHtml', _html);
        }
    });

    //从表
    var dtls = new Entities("BP.Sys.MapDtls");
    dtls.Retrieve("FK_MapData", pageParam.fk_mapdata, "FK_Node", 0);
    $.each(dtls, function (i, dtl) {
        var element = getElementByAttr(imgs, "data-key", dtl.No);
        //增加该元素
        if (element == null) {
            _html = "<img src='../CCFormDesigner/Controls/DataView/Dtl.png' style='width:67%;height:200px'  leipiplugins='dtl' data-key='" + dtl.No + "'/>"
            leipiEditor.execCommand('insertHtml', _html);
        }
    });

    //图片附件
    var imgAths = new Entities("BP.Sys.FrmImgAths");
    imgAths.Retrieve("FK_MapData", pageParam.fk_mapdata);
    $.each(imgAths, function (i, imgAth) {
        var element = getElementByAttr(imgs, "data-key", imgAth.MyPK);
        //增加该元素
        if (element == null) {
            _html = "<img src='../CCFormDesigner/Controls/DataView/AthImg.png' style='width:" + imgAth.W + "px;height:" + imgAth.H + "px'  leipiplugins='component' data-key='" + imgAth.MyPK + "' data-type='AthImg'/>"
            leipiEditor.execCommand('insertHtml', _html);
        }
    });

    //审核组件  判断当前是否是节点表单，节点表单才包含审核组件
    var fk_node = GetQueryString("FK_Node");
    if (fk_node != null && fk_node != undefined && fk_node != "0" && fk_node !="undefined") {
        var element = getElementByAttr(imgs, "data-type", "WorkCheck");
        if (element == null) {
            var node = new Entity("BP.WF.Node", fk_node);
            //并且签批字段为空时增加审核组件
            if (node.FWCSta != 0 && (node.CheckField == null || node.CheckField == undefined || node.CheckField == "")) {
                var _html = "<img src='../CCFormDesigner/Controls/DataView/FrmCheck.png' style='width:67%;height:200px'  leipiplugins='component' data-key='" + fk_node + "'  data-type='WorkCheck'/>"
                leipiEditor.execCommand('insertHtml', _html);
            }
        }
    }

    //获得内容.
    formeditor = leipiEditor.getContent();

    $("#Btn_Save").html("正在保存....");

    //保存表单的html信息
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_DevelopDesigner");
    handler.AddPara("FK_MapData", pageParam.fk_mapdata);
    handler.AddPara("HtmlCode", encodeURIComponent(formeditor));

    var data = handler.DoMethodReturnString("SaveForm");
    if (data.indexOf("err@") != -1) {
        alert(data);
        return;
    }

    $("#Btn_Save").html("保存");
}

var webUser = new WebUser();
function GetSysEnums(enumKey) {

    if (webUser.CCBPMRunModel == 0 || webUser.CCBPMRunModel == 1) {
        var ses = new Entities("BP.Sys.SysEnums");
        ses.Retrieve("EnumKey", enumKey, "IntKey");
        return ses;
    }

    var ses = new Entities("BP.Cloud.Sys.SysEnums");
    ses.Retrieve("EnumKey", enumKey.replace(webUser.OrgNo + "_", ""), "OrgNo", webUser.OrgNo, "IntKey");
    return ses;
}

//根据元素自定义的属性和值获取改元素
function getElementByAttr(aElements, attr, value) {
    for (var i = 0; i < aElements.length; i++) {
        if (aElements[i].getAttribute(attr) == value)
            return aElements[i];
    }
    return null;
}





