/// <reference path="jquery-1.7.2.min.js" />
function CtrlFactory(sId) {
    ///<summary>HTML对象辅助操作类</summary>
    ///<param name="sId" type="String">对象id</param>
    this.id = sId;
    this.jq = $('#' + sId);
    this.lastCtrl = null;
    this.tag = this.jq[0].localName;

    //公共方法
    if (typeof CtrlFactory._initialized == "undefined") {
        CtrlFactory.prototype.getQueryString = function (sName) {
            ///<summary>获取URL中的参数值</summary>
            ///<param name="sName" type="String">url参数名</param>
            var reg = new RegExp("(^|&)" + sName + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return encodeURIComponent(r[2]); return null;
        }

        CtrlFactory.prototype.ajax = function (url, data, async, successFunction, errorFunction) {
            /// <summary>与后台交互</summary>
            /// <param name="url" type="String">地址</param>
            /// <param name="data" type="Object">向后台发送的参数对象，格式如：{ method: 'save', FK_Flow: '002',...}</param>
            /// <param name="async" type="Boolean">是否使用异步</param>
            /// <param name="successFunction" type="Function">交互成功后，运行的函数</param>
            /// <param name="errorFunction" type="Function">交互失败后，运行的函数</param> 
            $.ajax({
                type: "GET", //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "text/plain; charset=utf-8",
                url: url, //要访问的后台地址
                data: data, //要发送的数据
                xhrFields: {
                    withCredentials: true
                },
                crossDomain: true,
                async: async,
                cache: false,
                error: function (XMLHttpRequest, errorThrown) {
                    if (errorFunction) {
                        errorFunction(XMLHttpRequest);
                    }
                },
                success: function (msg) {//msg为返回的数据，在这里做数据绑定
                    var d = msg;
                    if (successFunction) {
                        successFunction(d);
                    }
                }
            });
        }

        CtrlFactory.prototype.disabled = function (sCtrlId) {
            ///<summary>将指定控件设置为不可编辑</summary>
            ///<param name="sCtrlId" type="String">对象ID，必须是准确的ID，包含DDL_/TB_等</param>
            $('#' + sCtrlId).attr('disabled', 'disabled');
        }

        CtrlFactory.prototype.getValue = function (sCtrlId) {
            ///<summary>获取控件的值</summary>
            ///<param name="sCtrlId" type="String">对象ID，必须是准确的ID，包含DDL_/TB_等</param>
            var ctrl = $('#' + sCtrlId);

            if (ctrl.length == 0) {
                alert(sCtrlId + " 控件不存在！");
                return null;
            }

            return ctrl.val();
        }

        CtrlFactory.prototype.getText = function (sCtrlId) {
            ///<summary>获取控件的文本</summary>
            ///<param name="sCtrlId" type="String">对象ID，必须是准确的ID，包含DDL_/TB_等</param>
            var ctrl = $('#' + sCtrlId);

            if (ctrl.length == 0) {
                alert(sCtrlId + " 控件不存在！");
                return null;
            }

            if (sCtrlId.indexOf("DDL_") == 0) {
                return ctrl[0].selectedIndex != -1 ? ctrl[0].options[ctrl[0].selectedIndex].text : '';
            }

            return ctrl.text();
        }

        //增加控件
        CtrlFactory.prototype.addSelect = function (sParentCtrlId, sCtrlId, aItemSource, sValue, fnOnChange) {
            ///<summary>增加Select</summary>
            ///<param name="sParentCtrlId" type="String">父级对象ID</param>
            ///<param name="sCtrlId" type="String">对象ID</param>
            ///<param name="aItemSource" type="Array">填充的数组，对象含有NO,NAME属性</param>
            ///<param name="sValue" type="String">当前选中值</param>
            ///<param name="fnOnChange" type="Function">选择项变更时触发的事件：function(val,text){}</param>
            var ctrlId = 'DDL_' + sCtrlId;
            var html = '<select id="' + ctrlId + '">';

            $.each(aItemSource, function (idx, item) {
                if (item.NO == undefined || item.NAME == undefined) {
                    return true;
                }

                html += '<option value="' + item.NO + '"' + (sValue && sValue.toString() == item.NO ? ' selected' : '') + '>' + item.NAME + '</option>';
            });

            html += '</select>';

            if (sParentCtrlId) {
                $('#' + sParentCtrlId).append(html);
            }
            else {
                this.jq.append(html);
            }

            if (fnOnChange) {
                $('#' + ctrlId).change(function () {
                    fnOnChange(this.value, this.options[this.selectedIndex].text);
                });
            }

            this.lastCtrl = new CtrlFactory(ctrlId);

            return this;
        }

        CtrlFactory.prototype.addTR = function (sParentCtrlId, sCtrlId, sAttr, sBeforeSelector) {
            ///<summary>增加表格行TR对象</summary>
            ///<param name="sParentCtrlId" type="String">父级对象ID</param>
            ///<param name="sCtrlId" type="String">对象ID</param>
            ///<param name="sAttr" type="String">TR的属性</param>
            ///<param name="sBeforeSelector" type="String">在此过滤表达式代表的TR对象之后增加TR对象</param>
            var ctrlId = sCtrlId;
            var haveId = sCtrlId && sCtrlId.length > 0;
            var html = '<tr' + (haveId ? (' id="' + ctrlId + '"') : '') + (sAttr ? (' ' + sAttr) : '') + '></tr>';

            if (sBeforeSelector) {
                $(sBeforeSelector).after(html);
            }
            else {
                if (sParentCtrlId) {
                    $('#' + sParentCtrlId).append(html);
                }
                else {
                    this.jq.append(html);
                }
            }

            if (haveId) {
                this.lastCtrl = new CtrlFactory(ctrlId);
            }

            return this;
        }

        CtrlFactory.prototype.addTD = function (sParentCtrlId, sCtrlId, sAttr, sText) {
            ///<summary>增加表格行TD对象</summary>
            ///<param name="sParentCtrlId" type="String">父级对象ID</param>
            ///<param name="sCtrlId" type="String">对象ID</param>
            ///<param name="sAttr" type="String">TD的属性</param>
            ///<param name="sText" type="String">TD中的html</param>
            var ctrlId = sCtrlId;
            var haveId = sCtrlId && sCtrlId.length > 0;
            var html = '<td' + (haveId ? (' id="' + ctrlId + '"') : '') + (sAttr ? (' ' + sAttr) : '') + '>' + (sText ? sText : '') + '</td>';

            if (sParentCtrlId) {
                $('#' + sParentCtrlId).append(html);
            }
            else {
                this.jq.append(html);
            }

            if (haveId) {
                this.lastCtrl = new CtrlFactory(ctrlId);
            }

            return this;
        }

        CtrlFactory.prototype.addTextbox = function (sParentCtrlId, sCtrlId, sAttr, sValue) {
            ///<summary>增加文本框</summary>
            ///<param name="sParentCtrlId" type="String">父级对象ID</param>
            ///<param name="sCtrlId" type="String">对象ID</param>
            ///<param name="sAttr" type="String">文本框的属性</param>
            ///<param name="sValue" type="String">文本框的值</param>
            var ctrlId = 'TB_' + sCtrlId;
            var html = '<input type="text" id="' + ctrlId + '"' + (sAttr ? (' ' + sAttr) : '') + ' value="' + sValue + '" />';

            if (sParentCtrlId) {
                $('#' + sParentCtrlId).append(html);
            }
            else {
                this.jq.append(html);
            }

            this.lastCtrl = new CtrlFactory(ctrlId);

            return this;
        }

        CtrlFactory.prototype.addBtn = function (sParentSelector, sCtrlId, sAttr, sHref, sText, fnOnClick, sBeforeSelector) {
            ///<summary>增加按钮</summary>
            ///<param name="sParentSelector" type="String">父级对象ID</param>
            ///<param name="sCtrlId" type="String">对象ID</param>
            ///<param name="sAttr" type="String">按钮的属性</param>
            ///<param name="sHref" type="String">按钮跳转的URL</param>
            ///<param name="sText" type="String">按钮显示的文本</param>
            ///<param name="fnOnClick" type="Function">按钮单击事件：function(btn){}</param>
            ///<param name="sBeforeSelector" type="String">在此过滤表达式代表的对象之后增加按钮对象</param>
            var ctrlId = 'Btn_' + sCtrlId;
            var html = '<a href="' + (fnOnClick ? 'javascript:void(0)' : sHref) + '" id="' + ctrlId + '"' + (sAttr ? (' ' + sAttr) : '') + '>' + sText + '</a>';

            if (sBeforeSelector) {
                $(sBeforeSelector).after(html);
            }
            else {
                if (sParentSelector) {
                    $(sParentSelector).append(html);
                }
                else {
                    this.jq.append(html);
                }
            }

            if (fnOnClick) {
                $('#' + ctrlId).click(function () {
                    fnOnClick(this);
                });
            }

            if (sAttr && sAttr.indexOf('easyui-linkbutton') != -1) {
                $.parser.parse();
            }

            this.lastCtrl = new CtrlFactory(ctrlId);

            return this;
        }

        CtrlFactory.prototype.add = function (sParentSelector, sHtml, sBeforeSelector) {
            ///<summary>增加html代码</summary>
            ///<param name="sParentSelector" type="String">父级对象ID</param>
            ///<param name="sHtml" type="String">要添加的html代码</param>
            ///<param name="sBeforeSelector" type="String">在此过滤表达式代表的对象之后增加html</param>
            if (sBeforeSelector) {
                $(sBeforeSelector).after(sHtml);
            }
            else {
                if (sParentSelector) {
                    $(sParentSelector).append(sHtml);
                }
                else {
                    this.jq.append(sHtml);
                }
            }

            return this;
        }

        CtrlFactory.prototype.addBR = function (sParentSelector, iCount, sBeforeSelector) {
            ///<summary>增加换行</summary>
            ///<param name="sParentSelector" type="String">父级对象检索表达式</param>
            ///<param name="iCount" type="Int">要添加的换行的个数</param>
            ///<param name="sBeforeSelector" type="String">在此过滤表达式代表的对象之后增加换行</param>
            var html = '';

            for (var i = 0; i < iCount; i++) {
                html += '<br />';
            }

            if (sBeforeSelector) {
                $(sBeforeSelector).after(html);
            }
            else {
                if (sParentSelector) {
                    $(sParentSelector).append(html);
                }
                else {
                    this.jq.append(html);
                }
            }

            return this;
        }

        CtrlFactory.prototype.addSpace = function (sParentSelector, iCount, sBeforeSelector) {
            ///<summary>增加空格</summary>
            ///<param name="sParentSelector" type="String">父级对象ID</param>
            ///<param name="iCount" type="Int">要添加的空格的个数</param>
            ///<param name="sBeforeSelector" type="String">在此过滤表达式代表的对象之后增加空格</param>
            var html = '';

            for (var i = 0; i < iCount; i++) {
                html += '&nbsp;';
            }

            if (sBeforeSelector) {
                $(sBeforeSelector).after(html);
            }
            else {
                if (sParentSelector) {
                    $(sParentSelector).append(html);
                }
                else {
                    this.jq.append(html);
                }
            }

            return this;
        }
    }
}