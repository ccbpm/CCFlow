(function ($) {

    function onUnselect(target, record) {
        var opts = getOptions(target);
        opts.onUnselect.call("", record);
    }

    function append(target, datas, remove) {
        var opts = getOptions(target);
        var container = $(target).find(".ccflow-input-span-container");
        if (remove) {
            container.children(".mtags-tag").remove();
        }
        var valueField = opts.valueField;
        var textField = opts.textField;
        for (var i = 0; i < datas.length; i++) {
            var data = datas[i];
            if (!contains(target, data, valueField)) {
                var tag = $('<div class="mtags-tag"></div>');
                tag.data(data);
                var _html = '<span class="mtags-span">' + data[textField] + '</span><i class="layui-icon layui-icon-close" style="margin-left:8px;font-size:12px" data-role="remove"></i>';
                tag.html(_html);
                container.append(tag);
                tag.delegate("i", "click", function (e) {
                    var record = $(this).parent().data();
                    $(this).parent().remove();
                    opts.onUnselect.call("", record);
                });
            }
        }
    }

    function loadData(target, datas) {
        append(target, datas, true);
    }

    function clear(target) {
        $(target).find(".ccflow-input-span-container .mtags-tag").remove();
    }

    function setValues(target, values) {
        append(target, datas);
    }

    function getText(target) {
        var opts = getOptions(target);
        var textField = opts.textField;
        var text = [];
        $(target).find(".ccflow-input-span-container .mtags-tag").each(function () {
            text.push($(this).data()[textField]);
        });
        return text.join(",");
    }

    function getValue(target) {
        var opts = getOptions(target);
        var valueField = opts.valueField;
        var text = [];
        $(target).find(".ccflow-input-span-container .mtags-tag").each(function () {
            text.push($(this).data()[valueField]);
        });
        return text.join(",");
    }

    function getOptions(target) {
        return $.data(target, "mtags").options;
    }

    function contains(target, data, valueField) {
        var flag = false;
        $(target).find(".ccflow-input-span-container .mtags-tag").each(function () {
            if (data[valueField] == $(this).data()[valueField]) {
                flag = true;
                return;
            }
        });
        return flag;
    }

    function create(target) {
        //var opts = getOptions(target);
        var html = "";
        html += '<div class="layui-col-xs12 main-container">';
        html += '<div class="ccflow-input-span-container" style="display:flex;padding:3px 10px; flex-wrap:wrap;">';
        html += '<div id="stuff" style="display: inline; border-left: 0px solid white; width: 0px;"></div>';
        html += '</div>';
        html += '</div>';
        $(target).html(html);
    }

    function setSize(target) {
        var opts = getOptions(target);
        var t = $(target);
        if (opts.fit == true) {
            var p = t.parent();
            opts.width = p.width();
        }
        //var c = t.find('.main-container');
        //var c_width = opts.width == "100%";
        //c._outerWidth(c_width);
        //c._outerHeight(opts.height);
        //t.find("#stuff")._outerHeight(opts.height);
    }

    $.fn.mtags = function (options, params) {
        if (typeof options == 'string') {
            return $.fn.mtags.methods[options](this, params);
        }
        options = options || {};
        return this.each(function () {
            var state = $.data(this, "mtags");
            if (state) {
                $.extend(state.options, options);
            } else {
                state = $.data(this, 'mtags', {
                    options: $.extend({}, $.fn.mtags.defaults, $.fn.mtags.parseOptions(this), options)
                });
                create(this);
            }
            setSize(this);
        });
    };

    $.fn.mtags.methods = {
        setValues: function (jq, values) {
            return jq.each(function () {
                setValues(this, values);
            });
        },
        getValue: function (jq) {
            return getValue(jq[0]);
        },
        getText: function (jq) {
            return getText(jq[0]);
        },
        clear: function (jq) {
            return jq.each(function () {
                clear(this);
            });
        },
        loadData: function (jq, values) {
            return jq.each(function () {
                loadData(this, values);
            });
        }
    };

    $.fn.mtags.parseOptions = function (target) {
        var t = $(target);
        return $.extend({}, parseOptions(target, ["width", "data", {
            "fit": "boolean",
            "valueField": "string",
            "textField": "string"
        }]));
    };

    $.fn.mtags.defaults = {
        "width": "100%",
        "fit": true,
        "valueField": "No",
        "textField": "Name",
        "onUnselect": function (record) {
        }
    };

})(jQuery);

function parseOptions(target, properties) {
    var t = $(target);
    var options = {};

    var s = $.trim(t.attr('data-options'));
    if (s) {
        if (s.substring(0, 1) != '{') {
            s = '{ ' + s + ' } ';
        }
        options = (new Function('return ' + s))();
    }
    $.map(['width', 'height', 'left', 'top', 'minWidth', 'maxWidth', 'minHeight', 'maxHeight'], function (p) {
        var pv = $.trim(target.style[p] || '');
        if (pv) {
            if (pv.indexOf('%') == -1) {
                pv = parseInt(pv) || undefined;
            }
            options[p] = pv;
        }
    });

    if (properties) {
        var opts = {};
        for (var i = 0; i < properties.length; i++) {
            var pp = properties[i];
            if (typeof pp == 'string') {
                opts[pp] = t.attr(pp);
            } else {
                for (var name in pp) {
                    var type = pp[name];
                    if (type == 'boolean') {
                        opts[name] = t.attr(name) ? (t.attr(name) == 'true') : undefined;
                    } else if (type == 'number') {
                        opts[name] = t.attr(name) == '0' ? 0 : parseFloat(t.attr(name)) || undefined;
                    }
                }
            }
        }
        $.extend(options, opts);
    }
    return options;
}