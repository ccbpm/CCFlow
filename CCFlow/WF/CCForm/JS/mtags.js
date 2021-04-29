(function ($) {
	/*if ("undefined" == typeof IsPopEnableSelfInput) {
		IsPopEnableSelfInput = false;
	} */
	function onUnselect(target, record) {
		var opts = getOptions(target);
		opts.onUnselect.call("", record);
		$("#TB_" + opts.KeyOfEn).val($(target).mtags("getText"));
	}

	function appendSignalNode(target, data) {
		var opts = getOptions(target);
		var containerSpan = $(target).find(".ccflow-input-span-container-span");

		var valueField = opts.valueField;
		var textField = opts.textField;
		if (!contains(target, data, valueField)) {
			
			var tag = $('<span class="ccflow-tag ccflow-label ccflow-label-primary"></span>');
			tag.data(data);
			tag.html(data[textField] + '<i class="fa fa-times" data-role="remove"></i>');
			containerSpan.append(tag);
			tag.delegate("i", "click", function (e) {
				var record = $(this).parent().data();
				$(this).parent().remove();
				opts.onUnselect.call("", record);
				$("#TB_" + opts.KeyOfEn).val($(target).mtags("getText"));
			});			
		}
    }
	function append(target, datas, remove) {
		var opts = getOptions(target);
		var container;
		if (opts.IsEnter == false)
			container = $(target).find(".ccflow-input-span-container");
		else
			container = $(target).find(".ccflow-input-span-container-span");
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
					$("#TB_" + opts.KeyOfEn).val($(target).mtags("getText"));
				});
			}
		}
	}

	function loadData(target, datas) {
		append(target, datas, true);
	}

	function clear(target) {
		if (opts.IsEnter == false)
			$(target).find(".ccflow-input-span-container span").remove();
		else
			$(target).find(".ccflow-input-span-container-span span").remove();
	}

	function setValues(target, values) {
		append(target, datas);
	}

	function getText(target) {
		var opts = getOptions(target);
		var textField = opts.textField;
		var text = [];
		if (opts.IsEnter == false)
			$(target).find(".ccflow-input-span-container span").each(function () {
				text.push($(this).data()[textField]);
			});
		else
			$(target).find(".ccflow-input-span-container-span span").each(function () {
				text.push($(this).data()[textField]);
			});
		return text.join(",");
	}

	function getValue(target) {
		var opts = getOptions(target);
		var valueField = opts.valueField;
		var text = [];
		if (opts.IsEnter == false)
			$(target).find(".ccflow-input-span-container span").each(function () {
				text.push($(this).data()[valueField]);
			});
		else
			$(target).find(".ccflow-input-span-container-span span").each(function () {
				text.push($(this).data()[valueField]);
			});
		return text.join(",");
	}

	function getOptions(target) {
		return $.data(target, "mtags").options;
	}

	function contains(target, data, valueField) {
		var flag = false;
		var opts = getOptions(target);
		if (opts.IsEnter == false)
			$(target).find(".ccflow-input-span-container span").each(function () {
				if (data[valueField] == $(this).data()[valueField]) {
					flag = true;
					return;
				}
			});
		else
			$(target).find(".ccflow-input-span-container-span span").each(function () {
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
		if (opts.IsEnter == true) {
			html += '<div style="margin-bottom:6px"><input type = "text" id = "TB_InputAuto_' + opts.KeyOfEn + '" autocomplete = "off"  style = "flex-grow: 1;width: 80%;border: none;border-bottom:1px solid #ccc !important;outline: none;padding: 0;color: #666;font-size: 14px;appearance: none;height: 28px;background-color: transparent;" />';
			html += '<button type="button" class="el-button" id="' + target.id + '_Button" ><span>' + opts.Title + '</span></button></div>';
		}
		html += '<div class="main-container" >';
		if (opts.IsEnter == false) {
			html += '<div class="ccflow-input-span-container" >';
            html += '<div id="stuff" placeholder="请双击选择" style="display: inline; border-left: 1px solid white; width: 1px;"></div>';
		} else {
			html += '<div class="ccflow-input-span-container" style="display: flex;flex-wrap: wrap;border:none">';
			html += '<div id="stuff" style="display: inline; border-left: 1px solid white; width: 1px;"></div>';
			html += '<span class="ccflow-input-span-container-span" style="display: contents;"></span>';
			
        }
		html += '</div>';
		html += '</div>';
		
		$(target).html(html);
		$("#TB_InputAuto_" + opts.KeyOfEn).on('keypress', function (event) {
			//回车事件
			if (event.keyCode == 13 && $("#TB_InputAuto_" + opts.KeyOfEn).val() != "") {
				appendSignalNode(target, { "No": new Date().getTime(), "Name": $("#TB_InputAuto_" + opts.KeyOfEn).val() });
				SaveVal_FrmEleDB(opts.FK_MapData, opts.KeyOfEn, opts.RefPKVal, new Date().getTime(), $("#TB_InputAuto_" + opts.KeyOfEn).val(), 1);
				$("#TB_InputAuto_" + opts.KeyOfEn).val("");
				$("#TB_" + opts.KeyOfEn).val($(target).mtags("getText"));
			}
		});
		$("#TB_InputAuto_" + opts.KeyOfEn).blur(function () {
			if ($("#TB_InputAuto_" + opts.KeyOfEn).val() != "") {
				appendSignalNode(target, { "No": new Date().getTime(), "Name": $("#TB_InputAuto_" + opts.KeyOfEn).val() });
				SaveVal_FrmEleDB(opts.FK_MapData, opts.KeyOfEn, opts.RefPKVal, new Date().getTime(), $("#TB_InputAuto_" + opts.KeyOfEn).val(), 1);
				$("#TB_InputAuto_" + opts.KeyOfEn).val("");
				$("#TB_" + opts.KeyOfEn).val($(target).mtags("getText"));
            }	
		});
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
		"fit": true,
		"FK_MapData":"FK_MapData",
		"KeyOfEn": "KeyOfEn",
		"RefPKVal":"RefPKVal",
		"valueField" : "No",
		"textField": "Name",
		"IsEnter": false,
		"Title":"选择",
		"onUnselect" : function (record) {
		}
	};

})(jQuery);
