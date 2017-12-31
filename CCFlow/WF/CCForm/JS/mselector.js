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
				tag.html(data[textField] + '&nbsp;<i class="fa fa-times" data-role="remove"></i>');
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
			datas = executeSql(opts.sql, opts.valueField, opts.textField, values);
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

	function executeSql(sql, valueField, textField, key) {
		var datas = [];
		if (sql && $.trim(key) != "") {
			var dt = DBAccess.RunSQLReturnTable(sql.replace("@Key", key).replace(/~/g, "'"));
			if ($.isArray(dt)) {
				$.each(dt, function (i, o) {
					var option = {};
					option[valueField] = o[valueField];
					option[textField] = o[textField];
					datas.push(option);
				});
			}
			/*
			if (values) {
				var machesData = [];
				$.each(values.split(","), function (i, o) {
					$.each(datas, function (index, object) {
						if (o == object[valueField]) {
							var option = {};
							option[valueField] = object[valueField];
							option[textField] = object[textField];
							machesData.push(option);
						}
					});
				});
				datas = machesData;
			}
			*/
		}
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
		html += 	'<label>' + opts.label + '</label>';
		html += 	'<div class="ccflow-input-span-container">';
		html += 		'<input type="text" class="ccflow-search" value="" placeholder="' + opts.tip + '">';
		html += 	'</div>';
		html += 	'<div class="ccflow-block">';
		html += 		'<ul class="ccflow-ul">';
		html += 		'</ul>';
		html += 	'</div>';
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

		search.focus(function (){
			animteDown();
		});

		search.blur(function () {
			setTimeout(function () {
				animateUp(container, block);
			}, 200);
		});

		function addDictionary(datas, callback) {
			for (var i = 0; i < datas.length; i++) {
				var data = datas[i];
				var li = $("<li></li>");
				li.text(data[textField]);
				li.data(datas[i]);
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
				"border" : "1px solid #96C8DA",
				"border-top" : "0px",
				"box-shadow" : "0 2px 3px 0 rgba(34,36,38,.15)"
			});
			container.css({
				"border" : "1px solid #96C8DA",
				"border-bottom" : "0px",
				"box-shadow" : "0 2px 3px 0 rgba(34,36,38,.15)"
			});
		}

		search.keyup(function (e) {
			var text = search.val();
			var datas = [];
			if ($.isArray(opts.data) && opts.data.length > 0) {
				datas = opts.data;
			} else if ($.trim(opts.sql) != "") {
				datas = executeSql(opts.sql, valueField, textField, text);
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
		});

		function addListener() {
			ul.delegate("li", "click", function () {
				var data = $(this).data();
				if (!contains(target, data, valueField) && data[valueField] != empty) {
					var tag = $('<span class="ccflow-tag ccflow-label ccflow-label-primary"></span>');
					tag.data(data);
					tag.html($(this).text() + '&nbsp;<i class="fa fa-times" data-role="remove"></i>');
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
		}

		function animateUp() {
			block.slideUp("fast", function () {
				container.css({
					"border" : "1px solid #ccc"
				});
			});
		}
	}

	function resize(target) {
		var c = $(target).find(".ccflow-input-span-container");
		$(target).find(".ccflow-block").css({
			"width" : c.width() + 2 * parseInt(c.css("padding-left").match(/[0-9]+/)[0]) +  2 * parseInt(c.css("border-left").match(/[0-9]+/)[0])
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
		t._outerWidth(opts.width);
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
					options : $.extend({}, $.fn.mselector.defaults, $.fn.mselector.parseOptions(this), options)
				});
				create(this);
			}
			setSize(this);
		});
	};

	$.fn.mselector.methods = {
		setValues : function (jq, values) {
			return jq.each(function(){
				setValues(this, values);
			});
		},
		getValue : function (jq) {
			return getValue(jq[0]);
		},
		getText : function (jq) {
			return getText(jq[0]);
		},
		clear : function (jq) {
			return jq.each(function () {
				clear(this);
			});
		},
		loadData : function (jq, values) {
			return jq.each(function(){
				loadData(this, values);
			});
		}
	};

	$.fn.mselector.parseOptions = function (target) {
		var t = $(target);
		return $.extend({}, $.parser.parseOptions(target, [ "width", "data", {
			"fit" : "boolean",
			"valueField" : "string",
			"textField" : "string",
			"label" : "string",
			"filter" : "boolean",
			"tip" : "string",
			"sql" : "string"
		}]));
	};

	$.fn.mselector.defaults = {
		"width" : "100%",
		"fit" : true,
		"valueField" : "No",
		"textField" : "Name",
		"label" : "",
		"filter" : true,
		"tip" : "请输入关键字",
		"data" : [],	// [{ "No" : "2102", "Name" : "大连" }, { "No" : "3701", "Name" : "济南" }]
		"sql" : "",	// "SELECT No, Name FROM CN_CITY WHERE Name Like '%@Key%'"

		"onSelect" : function (record) {
		},
		"onUnselect" : function (record) {
		}
	};

})(jQuery);
