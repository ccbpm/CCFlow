(function ($) {

    function onSelect(target, record) {
        var opts = getOptions(target);
        opts.onSelect.call("", record);
    }

    function onUnselect(target, record) {
        var opts = getOptions(target);
        opts.onUnselect.call("", record);
    }

    function loadData(target, datas) {
        var opts = getOptions(target);
        var search = $(target).find(".ccflow-search");
        var valueField = opts.valueField;
        var textField = opts.textField;
        for (var i = 0; i < datas.length; i++) {
            var data = datas[i];
            if (!contains(target, data, valueField) && data[valueField] != empty && typeof data[valueField] !== "undefined") {
                var tag = $('<span class="ccflow-tag ccflow-label ccflow-label-primary"></span>');
                tag.data(data);
                tag.html( data[textField] + '<i class="fa fa-times" data-role="remove"></i>');
                search.before(tag);
                onSelect(target, data);
                tag.delegate("i", "click", function (e) {
                    var record = $(this).parent().data();
                    $(this).parent().remove();
                    opts.onUnselect.call("", record);
                });
            }
        }
    }

    function clear(target) {
        $(target).find(".ccflow-input-span-container span").remove();
    }

    function setValues(target, values) {
        var opts = getOptions(target);
        var valueField = opts.valueField;
        var textField = opts.textField;
        var datas = [];
        if ($.isArray(opts.data) && opts.data.length > 0) {
            datas = opts.data;
        } else if ($.trim(opts.sql) != "") {

            datas = exeSrc(opts.sql, opts.valueField, opts.textField, values);

        } else if ($.trim(opts.url) != "") {
            datas = requestUrl(opts.url, valueField, textField, text);
        }
        loadData(target, datas);
    }

    function getText(target) {
        var opts = getOptions(target);
        var textField = opts.textField;
        var text = [];
        $(target).find(".ccflow-input-span-container span").each(function () {
            text.push($(this).data()[textField]);
        });
        return text.join(",");
    }

    function getValue(target) {
        var opts = getOptions(target);
        var valueField = opts.valueField;
        var text = [];
        $(target).find(".ccflow-input-span-container span").each(function () {
            text.push($(this).data()[valueField]);
        });
        return text.join(",");
    }

    function getOptions(target) {
        return $.data(target, "mselector").options;
    }

    var empty = "__empty__";

    function requestUrl(url, valueField, textField, key) {
        var datas = [];
        return datas;
        if (url && $.trim(url) != "") {
            //var _url = url.replace(/@Key/g, key).replace(/~/g, "'");
            $.ajax({
                type: 'post',
                async: false,
                url: url + "&t=" + new Date().getTime(),
                dataType: 'html',
                xhrFields: {
                    withCredentials: true
                },
                crossDomain: true,
                success: function (data) {
                    if (data.indexOf("err@") != -1) {
                        alert(data);
                        return;
                    }
                    var ja = JSON.parse(data);
                    if ($.isArray(ja)) {
                        $.each(ja, function (i, o) {
                            var option = {};
                            option[valueField] = o[valueField];
                            option[textField] = o[textField];
                            datas.push(option);
                        });
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
                }
            });
        }
        return datas;
    }

    function exeSrc(sql, valueField, textField, key) {
        var datas = [];
        //var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCFrom");
        //if (sql && $.trim(key) != "") {
        //    key = key.replace("'", "");
        //    var _sql = sql.replace(/@Key/g, key).replace(/~/g, "'");
        //    handler.AddPara("SQL", _sql);
        //    var dt = handler.DoMethodReturnString("RunSQL_Init");
        //    if ($.isArray(dt)) {
        //        $.each(dt, function (i, o) {
        //            var option = {};
        //            option[valueField] = o[valueField];
        //            option[textField] = o[textField];
        //            datas.push(option);
        //        });
        //    }
        //    /*
        //    if (values) {
        //    var machesData = [];
        //    $.each(values.split(","), function (i, o) {
        //    $.each(datas, function (index, object) {
        //    if (o == object[valueField]) {
        //    var option = {};
        //    option[valueField] = object[valueField];
        //    option[textField] = object[textField];
        //    machesData.push(option);
        //    }
        //    });
        //    });
        //    datas = machesData;
        //    }
        //    */
        //}
        return datas;
    }

    function contains(target, data, valueField) {
        var flag = false;
        $(target).find(".ccflow-input-span-container span").each(function () {
            if (data[valueField] == $(this).data()[valueField]) {
                flag = true;
                return;
            }
        });
        return flag;
    }

    function create(target) {
        var opts = getOptions(target);
        var html = "";
        html += '<div class="col-xs-10 main-container">';
        html += '<label>' + opts.label + '</label>';
        html += '<div class="ccflow-input-span-container">';
        html += '<input type="text" class="ccflow-search" value="" placeholder="' + opts.tip + '" id="MultipleChoiceSearch">';
        html += '</div>';
        html += '<div class="ccflow-block">';
        html += '<ul class="ccflow-ul">';
        html += '</ul>';
        html += '</div>';
        html += '</div>';
        $(target).html(html);

        var valueField = opts.valueField;
        var textField = opts.textField;
        var emptyOption = {};
        emptyOption[valueField] = empty;
        emptyOption[textField] = "无匹配项";

        var search = $(target).find(".ccflow-search");
        var container = $(target).find(".ccflow-input-span-container");
        var block = $(target).find(".ccflow-block");
        var ul = $(target).find(".ccflow-ul");

        search.focus(function () {
            animteDown();
        });

        ul.blur(function () {
            setTimeout(function () {
                animateUp(container, block);
            }, 200);
        });

        //点击任意地方隐藏下拉
        $(document).click(function (event) {
            if (document.activeElement.id != "MultipleChoiceSearch")
                animateUp(container, block);
        });

        function addDictionary(datas, callback) {
            for (var i = 0; i < datas.length; i++) {
                var data = datas[i];
                var li = $("<li></li>");
                li.text(data[textField]);
                li.data(datas[i]);
				li.attr("tabindex", i);
                ul.append(li);
            }
            callback(data, valueField);
        }

        function updateDictionary(datas, callback) {
            ul.empty();
            addDictionary(datas, callback);
        }

        function animteDown() {
            block.slideDown("fast").css({
                "border": "1px solid #96C8DA",
                "border-top": "0px",
                "box-shadow": "0 2px 3px 0 rgba(34,36,38,.15)"
            });
            container.css({
                "border": "1px solid #96C8DA",
                "border-bottom": "0px",
                "box-shadow": "0 2px 3px 0 rgba(34,36,38,.15)"
            });
        }

        search.keyup(function (e) {
            var text = search.val();
            var datas = [];
            var src = opts.dbSrc;
            text = text.replace("'", "");
            //增加数据源的访问.
            src = src.replace(/@Key/g, text).replace(/~/g, "'");
            var dt = DBAccess.RunDBSrc(src);

            if ($.isArray(dt)) {
                $.each(dt, function (i, o) {
                    var option = {};
                    option[valueField] = o[valueField];
                    option[textField] = o[textField];
                    datas.push(option);
                });
            }

            if (opts.filter) {
                datas = $.grep(datas, function (o) {
                    return o[textField].indexOf(text) != -1;
                });
                if (datas.length === 0) {
                    datas.push(emptyOption);
                }
            }
            updateDictionary(datas, addListener);
			
			/*
			e = e || window.event;
			var key = e.keyCode || e.which || e.charCode;
			switch (key) {
				case 38:
					$(this).blur();
					ul.children("li").removeClass("hover");
					var scrollHeight = block.prop('scrollHeight');
					block.scrollTop(scrollHeight);
					ul.children("li:last").addClass("hover");
					ul.children("li:last").focus();
					ul.data({
						"currentIndex" : 0
					});
					break;
				case 40:
					$(this).blur();
					ul.children("li").removeClass("hover");
					block.scrollTop(0);
					ul.children("li:first").addClass("hover");
					ul.children("li:first").focus();
					//
					var totalCount = ul.children("li").length;
					ul.data({
						"totalCount" : totalCount,
						"currentIndex" : ul.children("li").length - 1
					});
					break;
			}
			*/
        });

        function addListener() {
            ul.delegate("li", "click", function () {
                var data = $(this).data();
                if (!contains(target, data, valueField) && data[valueField] != empty) {
                    var tag = $('<span class="ccflow-tag ccflow-label ccflow-label-primary"></span>');
                    tag.data(data);
                    tag.html($(this).text() + '<i class="fa fa-times" data-role="remove"></i>');
                    search.before(tag);
                    onSelect(target, data);
                    tag.delegate("i", "click", function () {
                        var record = $(this).parent().data();
                        $(this).parent().remove();
                        opts.onUnselect.call("", record);
                    });
                }
                search.val("");
                animateUp();
            });

			ul.children("li").on("mouseover", function () {
				ul.children("li").removeClass("hover");
				$(this).addClass("hover");
			});

			/*
			ul.on("keyup", function (e) {
				e = e || window.event;
				var key = e.keyCode || e.which || e.charCode;
				switch (key) {
					case 13:
						var currentIndex = ul.data().currentIndex;
						ul.children("li").eq(currentIndex).trigger("click");
						break;
					case 38:
						ul.children("li").removeClass("hover");
						//
						var totalCount = ul.data().totalCount;
						var currentIndex = ul.data().currentIndex;
						currentIndex--;
						if (currentIndex < 0) {
							currentIndex = totalCount - 1;
						}
						ul.children("li").eq(currentIndex).addClass("hover");
						block.scrollTop(currentIndex * 24);
						ul.data({
							"currentIndex" : currentIndex
						});
						break;
					case 40:
						ul.children("li").removeClass("hover");
						//
						var totalCount = ul.data().totalCount;
						var currentIndex = ul.data().currentIndex;
						currentIndex++;
						if (currentIndex >= totalCount) {
							currentIndex = 0;
						}
						ul.children("li").eq(currentIndex).addClass("hover");
						block.scrollTop(currentIndex * 24);
						ul.data({
							"currentIndex" : currentIndex
						});
						break;
				}
			});
			*/
        }

        function animateUp() {
            block.slideUp("fast", function () {
                container.css({
                    "border": "1px solid #ccc"
                });
            });
        }
    }

    function resize(target) {
        var c = $(target).find(".ccflow-input-span-container");
        //
        var width = c.width();
        //
        var pl = c.css("padding-left").match(/[0-9]+/);
        if ($.isArray(pl) && pl.length > 0) {
            width += 2 * parseInt(pl[0]);
        } else {
            width += 8;
        }
        //
        var bl = c.css("border-left").match(/[0-9]+/);
        if ($.isArray(bl) && bl.length > 0) {
            width += 2 * parseInt(bl[0]);
        } else {
            width += 2;
        }
        //
        $(target).find(".ccflow-block").css({
            //"width" : c.width() + 2 * parseInt(c.css("padding-left").match(/[0-9]+/)[0]) + 2 * parseInt(c.css("border-left").match(/[0-9]+/)[0])
            "width": width
        });
    }

    function setSize(target) {
        var opts = getOptions(target);
        var t = $(target);
        if (opts.fit == true) {
            var p = t.parent();
            opts.width = p.width();
        }
        var header = t.find('.main-container');
        //t._outerWidth(opts.width);
        resize(target);
        $(window).bind("resize", function () {
            resize(target);
        });
    }

    $.fn.mselector = function (options, params) {
        if (typeof options == 'string') {
            return $.fn.mselector.methods[options](this, params);
        }
        options = options || {};
        return this.each(function () {
            var state = $.data(this, "mselector");
            if (state) {
                $.extend(state.options, options);
            } else {
                state = $.data(this, 'mselector', {
                    options: $.extend({}, $.fn.mselector.defaults, $.fn.mselector.parseOptions(this), options)
                });
                create(this);
            }
            setSize(this);
        });
    };

    $.fn.mselector.methods = {
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

    $.fn.mselector.parseOptions = function (target) {
        var t = $(target);
        return $.extend({}, parseOptions(target, ["width", "data", {
            "fit": "boolean",
            "valueField": "string",
            "textField": "string",
            "label": "string",
            "filter": "boolean",
            "dbSrc": "string"
        }]));
    };

    $.fn.mselector.defaults = {
        "width": "100%",
        "fit": true,
        "valueField": "No",
        "textField": "Name",
        "label": "",
        "filter": true,
        "tip": "请输入关键字",
        "dbSrc": "",
        "onSelect": function (record) {
        },
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
