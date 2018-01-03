(function ($) {

	function loadData(target, datas) {
		var opts = getOptions(target);
		var container = $(target).find(".ccflow-input-span-container");
		var valueField = opts.valueField;
		var textField = opts.textField;
		for (var i = 0; i < datas.length; i++) {
			var data = datas[i];
			if (!contains(target, data, valueField) && typeof data[valueField] !== "undefined") {
				var tag = $('<span class="ccflow-tag ccflow-label ccflow-label-primary"></span>');
				tag.data(data);
				tag.html(data[textField] + '&nbsp;<i class="fa fa-times" data-role="remove"></i>');
				container.append(tag);
				tag.delegate("i", "click", function (e) {
					var record = $(this).parent().data();
					$(this).parent().remove();
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
		return $.data(target, "mtags").options;
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
		html += 	'<div class="ccflow-input-span-container"></div>';
		html += '</div>';
		$(target).html(html);

		var valueField = opts.valueField;
		var textField = opts.textField;

		//var container = $(target).find(".ccflow-input-span-container");

		/*
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

		search.keyup(function (e) {
			var text = search.val();
			var datas = [];
			if ($.isArray(opts.data) && opts.data.length > 0) {
				datas = opts.data;
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
			});
		}
		*/
 
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
		t._outerWidth(opts.width);
		resize(target);
		$(window).bind("resize", function () {
			resize(target);
		});
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
					options : $.extend({}, $.fn.mtags.defaults, $.fn.mtags.parseOptions(this), options)
				});
				create(this);
			}
			setSize(this);
		});
	};

	$.fn.mtags.methods = {
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

	$.fn.mtags.parseOptions = function (target) {
		var t = $(target);
		return $.extend({}, $.parser.parseOptions(target, [ "width", "data", {
			"fit" : "boolean",
			"valueField" : "string",
			"textField" : "string"
		}]));
	};

	$.fn.mtags.defaults = {
		"width" : "100%",
		"fit" : true,
		"valueField" : "No",
		"textField" : "Name"
	};

})(jQuery);
