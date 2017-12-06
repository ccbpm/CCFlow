
function testExpression(exp) {
	if (exp == null || typeof exp == "undefined" || typeof exp != "string") {
		return false;
	}
	exp = exp.replace(/\s/g, "");
	if (exp == "" || exp.length == 0) {
		return false;
	}
	if (/[\+\-\*\/]{2,}/.test(exp)) {
		return false;
	}
	if (/\(\)/.test(exp)) {
		return false;
	}
	var stack = [];
	for (var i = 0; i < exp.length; i++) {
		var c = exp.charAt(i);
		if (c == "(") {
			stack.push("(");
		} else if (c == ")") {
			if (stack.length > 0) {
				stack.pop();
			} else {
				return false;
			}
		}
	}
	if (stack.length != 0) {
		return false;
	}
	if (/^[\+\-\*\/]|[\+\-\*\/]$/.test(exp)) {
		return false;
	}
	if (/\([\+\-\*\/]|[\+\-\*\/]\)|[\+\-\*\/]\(|\)[\+\-\*\/]/.test(exp)) {
		return false;
	}
	return true;
}

/**
 * 表单计算(包括普通表单以及从表弹出页表单)
 */
function calculator(Sys_MapExt) {
	$.each(Sys_MapExt, function (i, o) {
		if (o.ExtType == "AutoFull") {
			if (!testExpression(o.Doc)) {
				console.log("MyPk: " + o.MyPK + ", 表达式: '" + o.Doc + "'格式错误");
				return false;
			}
			var targets = [];
			var index = -1;
			for (var i = 0; i < o.Doc.length; i++) {
				var c = o.Doc.charAt(i);
				if (/[\+\-|*\/\(\)]/.test(c)) {
					targets.push(o.Doc.substring(index + 1, i));
					index = i;
				}
			}
			targets.push(o.Doc.substring(index + 1, o.Doc.length));
			//
			var expression = {
				"judgement" : [],
				"execute_judgement" : [],
				"calculate" : o.Doc
			};
			$.each(targets, function (i, o) {
				var target = o.replace("@", "");
				var element = "$(':input[name=TB_" + target + "]')";
				expression.judgement.push(element + ".length == 0");
				expression.execute_judgement.push("!isNaN(parseFloat(" + element + ".val()))");
				expression.calculate = expression.calculate.replace(o, "parseFloat(" + element + ".val())");
			});
			(function (targets, expression, resultTarget, pk, expDefined) {
				$.each(targets, function (i, o) {
					var target = o.replace("@", "");
					$(":input[name=TB_" + target + "]").bind("change", function () {
						var evalExpression = " var result = ''; ";
						if (expression.judgement.length > 0) {
							evalExpression += " if ( " + expression.judgement.join(" || ") + " ) { ";
							evalExpression += " 	console.log(\"MyPk: " + pk + ", 表达式: '" + expDefined + "' " + "中有对象在当前页面不存在\");"
							evalExpression += " } ";
						}
						if (expression.execute_judgement.length > 0) {
							evalExpression += " else if ( " + expression.execute_judgement.join(" && ") + " ) { ";
						}
						if (expression.calculate.length > 0) {
							evalExpression += " 	result = " + expression.calculate + "; ";
						}
						if (expression.execute_judgement.length > 0) {
							evalExpression += " } ";
						}
						eval(evalExpression);
						$(":input[name=TB_" + resultTarget + "]").val(result);
					});
					if (i == 0) {
						$(":input[name=TB_" + target + "]").trigger("change");
					}
				});
			})(targets, expression, o.AttrOfOper, o.MyPK, o.Doc);
			$(":input[name=TB_" + o.AttrOfOper + "]").attr("disabled", true);
		}
	});
}

function GenerFreeFrm(mapData, frmData) {

    //循环MapAttr
    for (var mapAtrrIndex in frmData.Sys_MapAttr) {
        var mapAttr = frmData.Sys_MapAttr[mapAtrrIndex];
        var eleHtml = figure_MapAttr_Template(mapAttr);
        $('#CCForm').append(eleHtml);
    }

	calculator(frmData.Sys_MapExt);

    //循环FrmLab
    for (var i in frmData.Sys_FrmLab) {
        var frmLab = frmData.Sys_FrmLab[i];
        var label = figure_Template_Label(frmLab);
        $('#CCForm').append(label);
    }
    //循环FrmRB
    for (var i in frmData.Sys_FrmRB) {
        var frmLab = frmData.Sys_FrmRB[i];
        var label = figure_Template_Rb(frmLab);
        $('#CCForm').append(label);
    }

    //循环FrmBtn
    for (var i in frmData.Sys_FrmBtn) {
        var frmBtn = frmData.Sys_FrmBtn[i];
        var btn = figure_Template_Btn(frmBtn);
        $('#CCForm').append(btn);
    }

    //循环Image
    for (var i in frmData.Sys_FrmImg) {
        var frmImg = frmData.Sys_FrmImg[i];
        var createdFigure = figure_Template_Image(frmImg);
        $('#CCForm').append(createdFigure);
    }

    //循环 Link
    for (var i in frmData.Sys_FrmLink) {
        var frmLink = frmData.Sys_FrmLink[i];
        var createdFigure = figure_Template_HyperLink(frmLink);
        $('#CCForm').append(createdFigure);
    }

    //循环 图片附件
    for (var i in frmData.Sys_FrmImgAth) {
        var frmImgAth = frmData.Sys_FrmImgAth[i];
        var createdFigure = figure_Template_ImageAth(frmImgAth);
        $('#CCForm').append(createdFigure);
    }
    //循环 附件
    for (var i in frmData.Sys_FrmAttachment) {
        var frmAttachment = frmData.Sys_FrmAttachment[i];
        var createdFigure = figure_Template_Attachment(frmAttachment);
        $('#CCForm').append(createdFigure);
    }

	// 主表扩展(统计从表)
	var detailExt = {};
	// 装载公式 从表id -> 公式
	$.each(frmData.Sys_MapExt, function (i, o) {
		if (o.ExtType == "AutoFullDtlField") {	// 明细统计公式
			// 从表id.列.[Sum|Avg|Max|Min] -> CCFrm_CongBiaoCeShiDtl1.ShanJia.Avg
			var docs = o.Doc.split("\.");
			if (docs.length == 3) {
				var ext = {
					"DtlNo" : docs[0],
					"FK_MapData" : o.FK_MapData,
					"AttrOfOper" : o.AttrOfOper,
					"Doc" : o.Doc,
					"DtlColumn" : docs[1],
					"exp" : docs[2]
				};
				detailExt[ext.DtlNo] = ext;
				$(":input[name=TB_" + ext.AttrOfOper + "]").attr("disabled", true);
			}
		}
	});

    //循环 从表
    for (var i in frmData.Sys_MapDtl) {
        var frmMapDtl = frmData.Sys_MapDtl[i];
        var createdFigure = figure_Template_Dtl(frmMapDtl, detailExt[frmMapDtl.No]);	// 根据从表id获取公式 从表id -> 公式
        $('#CCForm').append(createdFigure);
    }

    //循环线
    for (var i in frmData.Sys_FrmLine) {
        var frmLine = frmData.Sys_FrmLine[i];
        var createdConnector = connector_Template_Line(frmLine);
        $('#CCForm').append(createdConnector);
    }

    //循环Sys_MapFrame
    for (var i in frmData.Sys_MapFrame) {
        var frame = frmData.Sys_MapFrame[i];
        var alertMsgEle = figure_Template_IFrame(frame);
        $('#CCForm').append(alertMsgEle);
    }
}


//初始化从表
function figure_Template_Dtl(frmDtl, ext) {

    var eleHtml = $("<DIV id='Fd" + frmDtl.No + "' style='position:absolute; left:" + frmDtl.X + "px; top:" + frmDtl.Y + "px; width:" + frmDtl.W + "px; height:" + frmDtl.H + "px;text-align: left;' >");
    var paras = this.pageData;
    var strs = "";
    for (var str in paras) {
        if (str == "EnsName" || str == "RefPKVal" || str == "IsReadonly")
            continue
        else
            strs += "&" + str + "=" + paras[str];
    }
    var src = "";

    if (frmDtl.RowShowModel == "0"  ) {
        if (pageData.IsReadOnly) {
            src = "Dtl.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.OID + "&IsReadonly=1" + strs;
        } else {
            src = "Dtl.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.OID + "&IsReadonly=0" + strs;
        }
    }
    else if (frmDtl.RowShowModel == "1") {
        if (pageData.IsReadOnly)
            src = "DtlCard.aspx?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.OID + "&IsReadonly=1" + strs;
        else
            src = "DtlCard.aspx?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.OID + "&IsReadonly=0" + strs;
    }
    var eleIframe = '<iframe></iframe>';
    eleIframe = $("<iframe ID='F" + frmDtl.No + "' src='" + src +
                 "' frameborder=0  style='position:absolute;width:" + frmDtl.W + "px; height:" + frmDtl.H +
                 "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
    if (pageData.IsReadOnly) {

    } else {
        if (frmDtl.DtlSaveModel == 0) {
            eleHtml.append(addLoadFunction(frmDtl.No, "blur", "SaveDtl"));
            eleIframe.attr('onload', frmDtl.No + "load()");
        }
    }
    eleHtml.append(eleIframe);

	if (ext) {	// 表达式传入iframe
		eleIframe.load(function () {
			$(this).contents().find(":input[id=formExt]").val(JSON.stringify(ext));
			if (this.contentWindow && typeof this.contentWindow.parentStatistics === "function") {
				this.contentWindow.parentStatistics(ext);
			}
		});
	}

    //added by liuxc,2017-1-10,此处前台JS中增加变量DtlsLoadedCount记录明细表的数量，用于加载完全部明细表的判断
    var js = "";
    if (!pageData.IsReadonly) {
        js = "<script type='text/javascript' >";
        js += " function SaveDtl(dtl) { ";
        js += "   GenerPageKVs(); //调用产生kvs ";
        js += "\n   var iframe = document.getElementById('F' + dtl );";
        js += "   if(iframe && iframe.contentWindow){ ";
        js += "      iframe.contentWindow.SaveDtlData(); ";
        js += "   } ";
        js += " } ";
        js += " function SaveM2M(dtl) { ";
        js += "   document.getElementById('F' + dtl ).contentWindow.SaveM2M();";
        js += "} ";
        js += "</script>";
        eleHtml.append($(js));
    }
    return eleHtml;
}

//初始化框架
function figure_Template_IFrame(fram) {

    var eleHtml = $("<DIV id='Fd" + fram.MyPK + "' style='position:absolute; left:" + fram.X + "px; top:" + fram.Y + "px; width:" + fram.W + "px; height:" + fram.H + "px;text-align: left;' >");
    var paras = this.pageData;
    var strs = "";
    for (var str in paras) {
        if (str == "EnsName" || str == "RefPKVal" || str == "IsReadonly")
            continue
        else
            strs += "&" + str + "=" + paras[str];
    }

    var src = dealWithUrl(fram.URL) + "&IsReadOnly=0";
    alert(src);

    var eleIframe = '<iframe></iframe>';
    eleIframe = $("<iframe ID='Fdg" + fram.MyPK + "' src='" + src +
                 "' frameborder=0  style='position:absolute;width:" + fram.W + "px; height:" + fram.H +
                 "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");

    eleHtml.append(eleIframe);

    return eleHtml;
}



//升级表单元素 初始化文本框、日期、时间
function figure_MapAttr_Template(mapAttr) {

    var eleHtml = '';
    if (mapAttr.UIVisible == 1) { //是否显示.

        var str = '';
        var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);

        var isInOneRow = false; //是否占一整行
        var islabelIsInEle = false; //

        eleHtml += '';

        if (mapAttr.UIContralType != 6) {

            if (mapAttr.LGType == 2) {

                eleHtml +=
                        "<select data-val='" + ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn) + "'   name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
            } else {
                //添加文本框 ，日期控件等
                //AppString   
                if (mapAttr.MyDataType == "1" && mapAttr.LGType != "2") {//不是外键
                    if (mapAttr.UIContralType == "1") {//DDL 下拉列表框

                        eleHtml +=
                                "<select data-val='" + ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' name='DDL_" + mapAttr.KeyOfEn + "' value='" + ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + ">" +
                            (frmData, mapAttr, defValue) + "</select>";
                    } else { //文本区域

                        if (mapAttr.UIHeight <= 23) {
                            eleHtml +=
                                "<input maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "' type='text' placeholder='" + (mapAttr.Tip || '') + "' " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + "/>"
                            ;
                        }
                        else {

                            if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {
                                //如果是富文本就使用百度 UEditor

                                if (mapAttr.UIIsEnable == "0") {
                                    //只读状态直接 div 展示富文本内容
                                    //eleHtml += "<script id='" + editorPara.id + "' name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";
                                    eleHtml += "<div class='richText' style='width:" + mapAttr.UIWidth + "px'>" + defValue + "</div>";
                                } else {
                                    document.BindEditorMapAttr = mapAttr; //存到全局备用

                                    //设置编辑器的默认样式
                                    var styleText = "text-align:left;font-size:12px;";
                                    styleText += "width:100%;";
                                    styleText += "height:" + mapAttr.UIHeight + "px;";
                                    //注意这里 name 属性是可以用来绑定表单提交时的字段名字的
                                    eleHtml += "<script id='editor' name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";
                                }
                            } else {
                                eleHtml +=
                                "<textarea maxlength=" + mapAttr.MaxLen + " style='height:" + mapAttr.UIHeight + "px;' name='TB_" + mapAttr.KeyOfEn + "' type='text' " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + "/>"
                            }
                        }
                    }
                } //AppDate
                else if (mapAttr.MyDataType == 6) {//AppDate
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {
                        enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += "<input maxlength=" + mapAttr.MaxLen + "  type='text' class='TBcalendar'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
                }
                else if (mapAttr.MyDataType == 7) {// AppDateTime = 7
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {
                        enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
                        //enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += "<input maxlength=" + mapAttr.MaxLen / 2 + "  type='text' class='TBcalendar'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' />";
                }
                else if (mapAttr.MyDataType == 4) {// AppBoolean = 7
                    if (mapAttr.UIIsEnable == 1) {

                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    //CHECKBOX 默认值
                    var checkedStr = '';
                    if (checkedStr != "true" && checkedStr != '1') {
                        checkedStr = ' checked="checked" '
                    }
                    checkedStr = ConvertDefVal(frmData, '', mapAttr.KeyOfEn);
                    eleHtml += "<div><input " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + "/>";
                    eleHtml += '<label class="labRb" for="CB_' + mapAttr.KeyOfEn + '">' + mapAttr.Name + '</label></div>';
                    //return eleHtml;
                }

                if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
                    if (mapAttr.UIContralType == 1) {//DDL
                        //多选下拉框
                        eleHtml +=
                                "<select data-val='" + ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
                    }
                }


                // AppDouble  AppFloat 
                if (mapAttr.MyDataType == 5 || mapAttr.MyDataType == 3) {
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {

                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += "<input style='text-align:right;' onkeyup=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' " + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
                }
                if ((mapAttr.MyDataType == 2 && mapAttr.LGType != 1)) {//AppInt
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {

                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += "<input style='text-align:right;' onkeyup=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' " + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
                }
                //AppMoney  AppRate
                if (mapAttr.MyDataType == 8) {
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {

                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += "<input style='text-align:right;' onkeyup=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' " + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
                }
            }
        }

        if (mapAttr.UIContralType == 6) {

            var atParamObj = AtParaToJson(mapAttr.AtPara);
            if (atParamObj.AthRefObj != undefined) {//扩展设置为附件展示
                eleHtml += "<input type='hidden' class='tbAth' data-target='" + mapAttr.AtPara + "' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' >" + "</input>";
                defValue = defValue != undefined && defValue != '' ? defValue : '&nbsp;';
                if (defValue.indexOf('@AthCount=') == 0) {
                    defValue = "附件" + "<span class='badge'>" + defValue.substring('@AthCount='.length, defValue.length) + "</span>个";
                } else {
                    defValue = defValue;
                }
                eleHtml += "<div class='divAth' data-target='" + mapAttr.KeyOfEn + "'  id='DIV_" + mapAttr.KeyOfEn + "'>" + defValue + "</div>";
            }
        }

        if (islabelIsInEle == false) {
            //eleHtml = '<div style="text-align:right;padding:0px;margin:0px; ' + (isInOneRow ? "clear:left;" : "") + '"  class="col-lg-1 col-md-1 col-sm-2 col-xs-4"><label>' + mapAttr.Name + "</label>" +
            //(mapAttr.UIIsInput == 1 ? '<span style="color:red" class="mustInput" data-keyofen="' + mapAttr.KeyOfEn + '">*</span>' : "")
            //+ "</div>" + eleHtml;
            //先把 必填项的 * 写到元素后面 可能写到标签后面更合适
            eleHtml +=
           mapAttr.UIIsInput == 1 ? '<span style="color:red" class="mustInput" data-keyofen="' + mapAttr.KeyOfEn + '">*</span>' : "";
        }

    } else {

        var value = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
        if (value == undefined) {
            value = '';
        } else {
            //value = value.toString().replace(/：/g, ':').replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}');
        }

        //hiddenHtml += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + " value='" + ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' name='TB_" + mapAttr.KeyOfEn + "></input>";
        eleHtml += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "'></input>";
    }
    eleHtml = $('<div>' + eleHtml + '</div>');
    eleHtml.children(0).css('width', mapAttr.UIWidth).css('height', mapAttr.UIHeight);
    eleHtml.css('position', 'absolute').css('top', mapAttr.Y).css('left', mapAttr.X);

    if (mapAttr.UIIsEnable == "0") {
        enableAttr = eleHtml.find('[name=TB_' + mapAttr.KeyOfEn + ']').attr('disabled', true);
        enableAttr = eleHtml.find('[name=DDL_' + mapAttr.KeyOfEn + ']').attr('disabled', true);
    }
    return eleHtml;
}

//将#FF000000 转换成 #FF0000
function TranColorToHtmlColor(color) {
    if (color != undefined && color.indexOf('#') == 0 && color.length == 9) {
        color = color.substring(0, 7);
    }
    return color;
}

//FontStyle, FontWeight, IsBold, IsItalic
//fontStyle font-size:19;font-family:"Portable User Interface";font-weight:bolder;color:#FF0051; 为H5设计的，不用解析后面3个
function analysisFontStyle(ele, fontStyle, isBold, isItalic) {
    if (fontStyle != undefined && fontStyle.indexOf(':') > 0) {
        var fontStyleArr = fontStyle.split(';');
        $.each(fontStyleArr, function (i, fontStyleObj) {
            ele.css(fontStyleObj.split(':')[0], fontStyleObj.split(':')[1]);
        });
    }
    else {
        if (isBold == 1) {
            ele.css('font-weight', 'bold');
        }
        if (isItalic == 1) {
            ele.css('font-style', 'italic')
        }
    }
}

//升级表单元素 初始化Label
function figure_Template_Label(frmLab) {
    var eleHtml = '';
    eleHtml = '<label></label>'
    eleHtml = $(eleHtml);
    var text = frmLab.Text.replace(/@/g, "<br>");
    eleHtml.html(text);
    eleHtml.css('position', 'absolute').css('top', frmLab.Y).css('left', frmLab.X).css('font-size', frmLab.FontSize)
        .css('padding-top', '5px').css('color', TranColorToHtmlColor(frmLab.FontColr));
    analysisFontStyle(eleHtml, frmLab.FontStyle, frmLab.isBold, frmLab.IsItalic);
    return eleHtml;
}

//初始化按钮
function figure_Template_Btn(frmBtn) {
    var eleHtml = $('<div></div>');
    var btnHtml = $('<input type="button" value="">');
    btnHtml.val(frmBtn.Text).width(frmBtn.W).height(frmBtn.H).addClass('btn');
    var doc = frmBtn.EventContext;
    doc = doc.replace("~", "'");
    var eventType = frmBtn.EventType;
    if (eventType == 0) {//禁用
        btnHtml.attr('disabled', 'disabled').css('background', 'gray');
    } else if (eventType == 5 || eventType == 6) {//运行Exe文件. 运行JS
        btnHtml.attr('onclick', doc);

    }
    eleHtml.append(btnHtml);
    //别的一些属性先不加
    eleHtml.css('position', 'absolute').css('top', frmBtn.Y).css('left', frmBtn.X).width(frmBtn.W).height(frmBtn.H);
    return eleHtml;
}

//初始化单选按钮
function figure_Template_Rb(frmRb) {
    var eleHtml = '<div></div>';
    eleHtml = $(eleHtml);
    var childRbEle = $('<input id="RB_ChuLiFangShi2" type="radio"/>');
    var childLabEle = $('<label class="labRb"></label>');
    childLabEle.html(frmRb.Lab).attr('for', 'RB_' + frmRb.KeyOfEn + frmRb.IntKey).attr('name', 'RB_' + frmRb.KeyOfEn);

    childRbEle.val(frmRb.IntKey).attr('id', 'RB_' + frmRb.KeyOfEn + frmRb.IntKey).attr('name', 'RB_' + frmRb.KeyOfEn);
    if (frmRb.UIIsEnable == false)
        childRbEle.attr('disabled', 'disabled');
    var defVal = ConvertDefVal(frmData, '', frmRb.KeyOfEn);
    if (defVal == frmRb.IntKey) {
        childRbEle.attr("checked", "checked");
    }

    eleHtml.append(childRbEle).append(childLabEle);
    eleHtml.css('position', 'absolute').css('top', frmRb.Y).css('left', frmRb.X);
    return eleHtml;
}

//初始化超链接
function figure_Template_HyperLink(frmLin) {
    //URL @ 变量替换
    var url = frmLin.URL;
    $.each(frmData.Sys_MapAttr, function (i, obj) {
        if (url.indexOf('@' + obj.KeyOfEn) > 0) {
            //替换
            //url=  url.replace(new RegExp(/(：)/g), ':');
            //先这样吧
            url = url.replace('@' + obj.KeyOfEn, frmData.MainTable[0][obj.KeyOfEn]);
        }
    });

    var eleHtml = '<span></span>';
    eleHtml = $(eleHtml);

    var a = $("<a></a>");
    a.attr('href', url).attr('target', frmLin.Target).html(frmLin.Text);
    eleHtml.append(a);
    eleHtml.css('position', 'absolute')
        .css('top', frmLin.Y)
        .css('left', frmLin.X)
        .css('color', frmLin.FontColr)
        .css('fontsize', frmLin.FontSize)
        .css('font-family', frmLin.FontName);
    return eleHtml;
}


//初始化 IMAGE  只初始化了图片类型
function figure_Template_Image(frmImage) {
    var eleHtml = '';
    var imgSrc = "";
    if (frmImage.ImgAppType == 0) {//图片类型
        //数据来源为本地.
        if (frmImage.ImgSrcType == 0) {
            if (frmImage.ImgPath.indexOf(";") < 0)
                imgSrc = frmImage.ImgPath;
        }
        //数据来源为指定路径.
        if (frmImage.ImgSrcType == 1) {
            //图片路径不为默认值
            imgSrc = frmImage.ImgURL;
            if (imgSrc.indexOf("@") == 0) {
                /*如果有变量 此处可能已经处理过    和周总商量*/
                //imgSrc = BP.WF.Glo.DealExp(imgSrc, en, "");
                imgSrc = imgSrc;
            }

        }
        // 由于火狐 不支持onerror 所以 判断图片是否存在放到服务器端
        if (imgSrc == "")//|| !File.Exists(Server.MapPath("~/" + imgSrc)))  //
            imgSrc = "../../DataUser/ICON/CCFlow/LogBig.png";
        eleHtml = $('<div></div>');
        var a = $("<a></a>");
        var img = $("<img/>")
        img.attr("src", imgSrc).css('width', frmImage.W).css('height', frmImage.H).attr('onerror', "this.src='../../DataUser/ICON/CCFlow/LogBig.png'");
        if (frmImage.LinkURL != undefined && frmImage.LinkURL != '') {
            a.attr('href', frmImage.LinkTarget).attr('target', frmImage.LinkTarget).css('width', frmImage.W).css('height', frmImage.H);
            a.append(img);
            eleHtml.append(a);
        } else {
            eleHtml.append(img);
        }

        eleHtml.attr("id", frmImage.MyPK);
        eleHtml.css('position', 'absolute').css('top', frmImage.Y).css('left', frmImage.X).css('width', frmImage.W).css('height', frmImage.H); ;
    } else if (frmImage.ImgAppType == 3)//二维码  手机
    {


    } else if (frmImage.ImgAppType == 1) {//暂不解析
        //电子签章  写后台
    }
    return eleHtml;
}

//初始化 IMAGE附件   L4418  问下周总
function figure_Template_ImageAth(frmImageAth) {
    return "";
}

//初始化 附件
function figure_Template_Attachment(frmAttachment) {
    var eleHtml = '';
    var ath = frmAttachment;
    if (ath.UploadType == 0) {//单附件上传 L4204
        return $('');
    }
    var src = "";
    if (pageData.IsReadonly)
        src = "AttachmentUpload.htm?PKVal=" + pageData.OID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";
    else
        src = "AttachmentUpload.htm?PKVal=" + pageData.OID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK;

    eleHtml += '<div>' + "<iframe style='width:" + ath.W + "px;height:" + ath.H + "px;' ID='Attach_" + ath.MyPK + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', ath.Y).css('left', ath.X).css('width', ath.W).css('height', ath.H);

    return eleHtml;
}

function connector_Template_Line(frmLine) {
    var eleHtml = '';
    eleHtml = '<table><tr><td></td></tr></table>';
    eleHtml = $(eleHtml).css('position', 'absolute').css('top', frmLine.Y1).css('left', frmLine.X1);
    eleHtml.find('td').css('padding', '0px')
    //css('top',parseFloat(frmLine.Y1)>parseFloat( frmLine.Y2)?frmLine.Y2:frmLine.Y1).
    //css('left', parseFloat(frmLine.X1) > parseFloat(frmLine.X2 )? frmLine.X2 : frmLine.X1).
        .css('width', Math.abs(frmLine.X1 - frmLine.X2) == 0 ? frmLine.BorderWidth : Math.abs(frmLine.X1 - frmLine.X2))
    .css('height', Math.abs(frmLine.Y1 - frmLine.Y2) == 0 ? frmLine.BorderWidth : Math.abs(frmLine.Y1 - frmLine.Y2))
        .css("background", frmLine.BorderColor);

    return eleHtml;
}
