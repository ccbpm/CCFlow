(function ($) {

	function onUnselect(target, record) {
		var opts = getOptions(target);
		opts.onUnselect.call("", record);
	}

	function append(target, datas, remove) {
		var opts = getOptions(target);
		var container = $(target).find(".ccflow-input-span-container");
		if (remove) {
			container.children("span").remove();
		}
		var valueField = opts.valueField;
		var textField = opts.textField;
		for (var i = 0; i < datas.length; i++) {
			var data = datas[i];
			if (!contains(target, data, valueField)) {
				var tag = $('<span class="ccflow-tag ccflow-label ccflow-label-primary"></span>');
				tag.data(data);
				tag.html(data[textField] + '<i class="fa fa-times" data-role="remove"></i>');
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
		$(target).find(".ccflow-input-span-container span").remove();
	}

	function setValues(target, values) {
		append(target, datas);
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
		html += '<div class="main-container">';
		html += 	'<div class="ccflow-input-span-container">';
		html += 		'<div id="stuff" style="display: inline; border-left: 1px solid white; width: 1px;"></div>';
		html += 	'</div>';
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
		var c = t.find('.main-container');
		c._outerWidth(opts.width);
		c._outerHeight(opts.height);
		t.find("#stuff")._outerHeight(opts.height);
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
			return jq.each(function () {
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
			return jq.each(function () {
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
		"textField" : "Name",
		"onUnselect" : function (record) {
		}
	};

})(jQuery);
