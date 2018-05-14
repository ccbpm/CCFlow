function LoadFrmDataAndChangeEleStyle(frmData) {

    //加入隐藏控件.
    var mapAttrs = frmData.Sys_MapAttr;
    var html = "";
    for (var mapAttr in mapAttrs) {
        if (mapAttr.UIVisable == 0) {
            var defval = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
            html += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + defval + "' />";
            html = $(html);
            $('#CCFrom').append(html);
        }
    }

    //设置为只读的字段.
    for (var i = 0; i < mapAttrs.length; i++) {
        var mapAttr = mapAttrs[i];
        //设置文本框只读.
        if (mapAttr.UIIsEnable == false || mapAttr.UIIsEnable == 0) {
            var tb = $('#TB_' + mapAttr.KeyOfEn);
            $('#TB_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#CB_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#RB_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#DDL_' + mapAttr.KeyOfEn).attr('disabled', true);
        }
    }

    //为控件赋值.
    for (var i = 0; i < mapAttrs.length; i++) {

        var mapAttr = mapAttrs[i];

        $('#TB_' + mapAttr.KeyOfEn).attr("name", "TB_" + mapAttr.KeyOfEn);
        $('#DDL_' + mapAttr.KeyOfEn).attr("name", "DDL_" + mapAttr.KeyOfEn);
        $('#CB_' + mapAttr.KeyOfEn).attr("name", "CB_" + mapAttr.KeyOfEn);

        var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);

        // alert(val);

        $('#TB_' + mapAttr.KeyOfEn).val(val);

        //文本框.
        if (mapAttr.UIContralType == 0) {
            if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {
                $('#editor').val(val);
            } else {
                $('#TB_' + mapAttr.KeyOfEn).val(val);
            }
        }

        //枚举下拉框.
        if (mapAttr.UIContralType == 1) {

            // InitDDLOperation(flowData, mapAttr, val);
            $('#DDL_' + mapAttr.KeyOfEn).val(val);

        }

        //checkbox.
        if (mapAttr.UIContralType == 2) {
            if (val == "1")
                $('#CB_' + mapAttr.KeyOfEn).attr("checked", "true");
        }
    }

    var mapAttrs = frmData.Sys_MapAttr;
    //解析设置表单字段联动显示与隐藏.
    for (var i = 0; i < mapAttrs.length; i++) {

        var mapAttr = mapAttrs[i];
        if (mapAttr.UIVisible == 0)
            continue;

        if (mapAttr.LGType != 1)
            continue;

        if (mapAttr.UIIsEnable ==0)
            continue;


        if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {  // AppInt Enum
            if (mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
                if (mapAttr.UIContralType == 1) {
                    /*启用了显示与隐藏.*/
                    var ddl = $("#DDL_" + mapAttr.KeyOfEn);
                    //初始化页面的值
                    var nowKey = ddl.val();
                                        

                    setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey);

                }
                if (mapAttr.UIContralType == 3) {
                    /*启用了显示与隐藏.*/
                    var rb = $("#RB_" + mapAttr.KeyOfEn);
                    var nowKey = $('input[name="RB_' + mapAttr.KeyOfEn + '"]:checked').val();
                    setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey);

                }
            }
        }

    }


}

//处理 MapExt 的扩展. 工作处理器，独立表单都要调用他.
function AfterBindEn_DealMapExt(frmData) {

    var mapExts = frmData.Sys_MapExt;
    var mapAttrs = frmData.Sys_MapAttr;
    // 主表扩展(统计从表)
    var detailExt = {};

    for (var i = 0; i < mapExts.length; i++) {
        var mapExt = mapExts[i];

        //一起转成entity.
        var mapExt = new Entity("BP.Sys.MapExt", mapExt);
        var mapAttr = null;
        for (var j = 0; j < mapAttrs.length; j++) {
            if (mapAttrs[j].FK_MapData == mapExt.FK_MapData && mapAttrs[j].KeyOfEn == mapExt.AttrOfOper) {
                mapAttr = mapAttrs[j];
                break;
            }
        }

        //debugger;
        switch (mapExt.ExtType) {
            case "MultipleChoiceSmall":
                MultipleChoiceSmall(mapExt, mapAttr); //调用 /CCForm/JS/MultipleChoiceSmall.js 的方法来完成.
                break;
            case "MultipleChoiceSearch":
                MultipleChoiceSearch(mapExt); //调用 /CCForm/JS/MultipleChoiceSmall.js 的方法来完成.
                break;
            case "PopBranchesAndLeaf": //树干叶子模式.
                PopBranchesAndLeaf(mapExt); //调用 /CCForm/JS/Pop.js 的方法来完成.
                break;
            case "PopBranches": //树干简单模式.
                PopBranches(mapExt); //调用 /CCForm/JS/Pop.js 的方法来完成.
                break;
            case "PopGroupList": //分组模式.
                PopGroupList(mapExt); //调用 /CCForm/JS/Pop.js 的方法来完成.
                break;
            case "PopSelfUrl": //自定义url.
                SelfUrl(mapExt); //调用 /CCForm/JS/MultipleChoiceSmall.js 的方法来完成.
                break;
            case "PopTableSearch": //表格查询.
                PopTableSearch(mapExt); //调用 /CCForm/JS/Pop.js 的方法来完成.
                break;
            case "PopVal": //PopVal窗返回值.
                var tb = $('[name$=' + mapExt.AttrOfOper + ']');
                //tb.attr("placeholder", "请双击选择。。。");
                tb.attr("onclick", "ShowHelpDiv('TB_" + mapExt.AttrOfOper + "','','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "','returnvalccformpopval');");
                tb.attr("ondblclick", "ReturnValCCFormPopValGoogle(this,'" + mapExt.MyPK + "','" + mapExt.FK_MapData + "', " + mapExt.W + "," + mapExt.H + ",'" + GepParaByName("Title", mapExt.AtPara) + "');");

                tb.attr('readonly', 'true');
                //tb.attr('disabled', 'true');
                var icon = '';
                var popWorkModelStr = '';
                var popWorkModelIndex = mapExt.AtPara != undefined ? mapExt.AtPara.indexOf('@PopValWorkModel=') : -1;
                if (popWorkModelIndex >= 0) {
                    popWorkModelIndex = popWorkModelIndex + '@PopValWorkModel='.length;
                    popWorkModelStr = mapExt.AtPara.substring(popWorkModelIndex, popWorkModelIndex + 1);
                }
                switch (popWorkModelStr) {
                    /// <summary>       
                    /// 自定义URL       
                    /// </summary>       
                    //SelfUrl =1,       
                    case "1":
                        icon = "glyphicon glyphicon-th";
                        break;
                    /// <summary>       
                    /// 表格模式       
                    /// </summary>       
                    // TableOnly,       
                    case "2":
                        icon = "glyphicon glyphicon-list";
                        break;
                    /// <summary>       
                    /// 表格分页模式       
                    /// </summary>       
                    //TablePage,       
                    case "3":
                        icon = "glyphicon glyphicon-list-alt";
                        break;
                    /// <summary>       
                    /// 分组模式       
                    /// </summary>       
                    // Group,       
                    case "4":
                        icon = "glyphicon glyphicon-list-alt";
                        break;
                    /// <summary>       
                    /// 树展现模式       
                    /// </summary>       
                    // Tree,       
                    case "5":
                        icon = "glyphicon glyphicon-tree-deciduous";
                        break;
                    /// <summary>       
                    /// 双实体树       
                    /// </summary>       
                    // TreeDouble       
                    case "6":
                        icon = "glyphicon glyphicon-tree-deciduous";
                        break;
                    default:
                        break;
                }
                tb.width(tb.width() - 40);
                tb.height('auto');
                var eleHtml = ' <div class="input-group form_tree" style="width:' + tb.width() + 'px;height:' + tb.height() + 'px">' + tb.parent().html() +
                '<span class="input-group-addon" onclick="' + "ReturnValCCFormPopValGoogle(document.getElementById('TB_" + mapExt.AttrOfOper + "'),'" + mapExt.MyPK + "','" + mapExt.FK_MapData + "', " + mapExt.W + "," + mapExt.H + ",'" + GepParaByName("Title", mapExt.AtPara) + "');" + '"><span class="' + icon + '"></span></span></div>';
                tb.parent().html(eleHtml);
                break;
            case "BindFunction": //控件绑定函数.

                if ($('#TB_' + mapExt.AttrOfOper).length == 1) {
                    $('#TB_' + mapExt.AttrOfOper).bind(DynamicBind(mapExt, "TB_"));
                    break;
                }
                if ($('#DDL_' + mapExt.AttrOfOper).length == 1) {
                    $('#DDL_' + mapExt.AttrOfOper).bind(DynamicBind(mapExt, "DDL_"));
                    break;
                }
                if ($('#CB_' + mapExt.AttrOfOper).length == 1) {
                    $('#CB_' + mapExt.AttrOfOper).bind(DynamicBind(mapExt, "CB_"));
                    break;
                }
                if ($('#RB_' + mapExt.AttrOfOper).length == 1) {
                    $('#RB_' + mapExt.AttrOfOper).bind(DynamicBind(mapExt, "RB_"));
                    break;
                }
                break;
            case "RegularExpression": //正则表达式  统一在保存和提交时检查

                if (mapExt.Tag == "onblur") {
                    var tb = $('[name$=' + "TB_" + mapExt.AttrOfOper + ']');
                    tb.blur(function () {  // 失去焦点 
                        //    SetQingJiaTianShu();
                    });
                    return;
                }
                var tb = $('[name$=' + mapExt.AttrOfOper + ']');

                //tb.attr(mapExt.Tag, "CheckRegInput('" + tb.attr('name') + "'," + mapExt.Doc.replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}') + ",'" + mapExt.Tag1 + "')");

                if (tb.attr('class') != undefined && tb.attr('class').indexOf('CheckRegInput') > 0) {
                    break;
                } else {
                    tb.addClass("CheckRegInput");
                    tb.data(mapExt)
                    //tb.data().name = tb.attr('name');
                    //tb.data().Doc = mapExt.Doc;
                    //tb.data().Tag1 = mapExt.Tag1;
                    //tb.attr("data-name", tb.attr('name'));
                    //tb.attr("data-Doc", tb.attr('name'));
                    //tb.attr("data-checkreginput", "CheckRegInput('" + tb.attr('name') + "'," + mapExt.Doc.replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}') + ",'" + mapExt.Tag1 + "')");
                }
                break;
            case "InputCheck": //输入检查
                //var tbJS = $("#TB_" + mapExt.AttrOfOper);
                //if (tbJS != undefined) {
                //    tbJS.attr(mapExt.Tag2, mapExt.Tag1 + "(this)");
                //}
                //else {
                //    tbJS = $("#DDL_" + mapExt.AttrOfOper);
                //    if (ddl != null)
                //        ddl.attr(mapExt.Tag2, mapExt.Tag1 + "(this);");
                //}
                break;
            case "TBFullCtrl": //自动填充
                var tbAuto = $("#TB_" + mapExt.AttrOfOper);
                if (tbAuto == null)
                    continue;

                tbAuto.attr("ondblclick", "ReturnValTBFullCtrl(this,'" + mapExt.MyPK + "');");
                tbAuto.attr("onkeyup", "DoAnscToFillDiv(this,this.value,\'TB_" + mapExt.AttrOfOper + "\', \'" + mapExt.MyPK + "\');");
                tbAuto.attr("AUTOCOMPLETE", "OFF");
                if (mapExt.Tag != "") {
                    /* 处理下拉框的选择范围的问题 */
                    var strs = mapExt.Tag.split('$');
                    for (var str in strs) {
                        var str = strs[k];
                        if (str = "") {
                            continue;
                        }

                        var myCtl = str.split(':');
                        var ctlID = myCtl[0];
                        var ddlC1 = $("#DDL_" + ctlID);
                        if (ddlC1 == null) {
                            continue;
                        }

                        //如果文本库数值为空，就让其返回.
                        var txt = tbAuto.val();
                        if (txt == '')
                            continue;

                        //获取要填充 ddll 的SQL.
                        var sql = myCtl[1].replace(/~/g, "'");
                        sql = sql.replace("@Key", txt);
                        //sql = BP.WF.Glo.DealExp(sql, en, null);  怎么办

                        //try
                        //{
                        //    dt = DBAccess.RunSQLReturnTable(sql);
                        //}
                        //catch (Exception ex)
                        //{
                        //    this.Clear();
                        //    this.AddFieldSet("配置错误");
                        //    this.Add(me.ToStringAtParas() + "<hr>错误信息:<br>" + ex#MessageDiv);
                        //    this.AddFieldSetEnd();
                        //    return;
                        //}

                        //if (dt.Rows.Count != 0)
                        //{
                        //    string valC1 = ddlC1.SelectedItemStringVal;
                        //    foreach (DataRow dr in dt.Rows)
                        //{
                        //        ListItem li = ddlC1.Items.FindByValue(dr[0].ToString());
                        //    if (li == null)
                        //    {
                        //        ddlC1.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                        //    }
                        //    else
                        //    {
                        //        li.Attributes["enable"] = "false";
                        //        li.Attributes["display"] = "false";

                        //    }
                        //}
                        //ddlC1.SetSelectItem(valC1);
                    }
                }

                break;
            case "ActiveDDL": /*自动初始化ddl的下拉框数据. 下拉框的级联操作 已经 OK*/
                var ddlPerant = $("#DDL_" + mapExt.AttrOfOper);
                var ddlChild = $("#DDL_" + mapExt.AttrsOfActive);
                if (ddlPerant == null || ddlChild == null)
                    continue;
                ddlPerant.attr("onchange", "DDLAnsc(this.value,\'" + "DDL_" + mapExt.AttrsOfActive + "\', \'" + mapExt.MyPK + "\')");
                // 处理默认选择。
                //string val = ddlPerant.SelectedItemStringVal;
                var valClient = ConvertDefVal(frmData, '', mapExt.AttrsOfActive); // ddlChild.SelectedItemStringVal;

                //ddlChild.select(valClient);  未写
                break;
            case "AutoFullDLL": // 自动填充下拉框.
                continue; //已经处理了。
            case "AutoFull": //自动填充  //a+b=c DOC='@DanJia*@ShuLiang'  等待后续优化
                //循环  KEYOFEN
                //替换@变量
                //处理 +-*%

                if (mapExt.Doc == undefined || mapExt.Doc == '')
                    continue;
                calculator(mapExt);
                break;
            case "AutoFullDtlField": //主表扩展(统计从表)
                var docs = mapExt.Doc.split("\.");
                if (docs.length == 3) {
                    var ext = {
                        "DtlNo": docs[0],
                        "FK_MapData": mapExt.FK_MapData,
                        "AttrOfOper": mapExt.AttrOfOper,
                        "Doc": mapExt.Doc,
                        "DtlColumn": docs[1],
                        "exp": docs[2]
                    };
                    if (!$.isArray(detailExt[ext.DtlNo])) {
                        detailExt[ext.DtlNo] = [];
                    }
                    detailExt[ext.DtlNo].push(ext);
                    var iframeDtl = $("#F" + ext.DtlNo);
                    iframeDtl.load(function () {
                        $(this).contents().find(":input[id=formExt]").val(JSON.stringify(detailExt[ext.DtlNo]));
                        if (this.contentWindow && typeof this.contentWindow.parentStatistics === "function") {
                            this.contentWindow.parentStatistics(detailExt[ext.DtlNo]);
                        }
                    });
                    $(":input[name=TB_" + ext.AttrOfOper + "]").attr("disabled", true);
                }
                break;
            case "DDLFullCtrl": // 自动填充其他的控件..  先不做

                var ddlOper = $("#DDL_" + mapExt.AttrOfOper);
                if (ddlOper.length == 0)
                    continue;

                var enName = frmData.Sys_MapData[0].No;

                ddlOper.attr("onchange", "Change('" + enName + "');DDLFullCtrl(this.value,\'" + "DDL_" + mapExt.AttrOfOper + "\', \'" + mapExt.MyPK + "\')");

                // alert(enName + " " + ddlOper.length + " " + mapExt.AttrOfOper + " " + document.getElementById("DDL_" + mapExt.AttrOfOper));
                //ddlOper.bind("change", function () {
                //alert('sss');
                // Change('" + enName + "');
                //DDLFullCtrl(this.value, "DDL_" + mapExt.AttrOfOper, mapExt.MyPK);
                //s  });
                //  ddlOper.attr("onchange", "alert('sss');Change('" + enName + "');DDLFullCtrl(this.value,\'" + "DDL_" + mapExt.AttrOfOper + "\', \'" + mapExt.MyPK + "\')");


                if (mapExt.Tag != null && mapExt.Tag != "") {
                    /* 下拉框填充范围. */
                    var strs = mapExt.Tag.split('$');
                    for (var k = 0; k < strs.length; k++) {
                        var str = strs[k];
                        if (str == "")
                            continue;

                        var myCtl = str.split(':');
                        var ctlID = myCtl[0];
                        var ddlC1 = $("#DDL_" + ctlID);
                        if (ddlC1 == null) {
                            //me.Tag = "";
                            //me.Update();
                            continue;
                        }

                        //如果触发的dll 数据为空，则不处理.
                        if (ddlOper.val() == "")
                            continue;

                        var sql = myCtl[1].replace(/~/g, "'");
                        sql = sql.replace("@Key", ddlOper.val());

                        //需要执行SQL语句
                        //sql = BP.WF.Glo.DealExp(sql, en, null);

                        //dt = DBAccess.RunSQLReturnTable(sql);
                        //string valC1 = ddlC1.SelectedItemStringVal;
                        //if (dt.Rows.Count != 0)
                        //{
                        //    foreach (DataRow dr in dt.Rows)
                        //{
                        //        ListItem li = ddlC1.Items.FindByValue(dr[0].ToString());
                        //    if (li == null)
                        //    {
                        //        ddlC1.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                        //    }
                        //    else
                        //    {
                        //        li.Attributes["visable"] = "false";
                        //    }
                        //}

                        //var items = [{ No: 1, Name: '测试1' }, { No: 2, Name: '测试2' }, { No: 3, Name: '测试3' }, { No: 4, Name: '测试4' }, { No: 5, Name: '测试5'}];
                        var operations = '';
                        //                        $.each(items, function (i, item) {
                        //                            operations += "<option  value='" + item.No + "'>" + item.Name + "</option>";
                        //                        });
                        ddlC1.children().remove();
                        ddlC1.html(operations);
                        //ddlC1.SetSelectItem(valC1);
                    }
                }
                break;
        }
    }
}

function DynamicBind(mapExt, ctrlType) {

    $('#' + ctrlType + mapExt.AttrOfOper).on(mapExt.Tag, function () {
        DBAccess.RunFunctionReturnStr(mapExt.Doc);
    });
}

/**
* 表单计算(包括普通表单以及从表弹出页表单)
*/
function calculator(o) {
    if (!testExpression(o.Doc)) {
        console.log("MyPk: " + o.MyPK + ", 表达式: '" + o.Doc + "'格式错误");
        return false;
    }
    var targets = [];
    var index = -1;
    for (var i = 0; i < o.Doc.length; i++) {	// 对于复杂表达式需要重点测试
        var c = o.Doc.charAt(i);
        if (c == "(") {
            index++;
        } else if (c == ")") {
            targets.push(o.Doc.substring(index + 1, i));
            i++;
            index = i;
        } else if (/[\+\-|*\/]/.test(c)) {
            targets.push(o.Doc.substring(index + 1, i));
            index = i;
        }
    }
    if (index + 1 < o.Doc.length) {
        targets.push(o.Doc.substring(index + 1, o.Doc.length));
    }
    //
    var expression = {
        "judgement": [],
        "execute_judgement": [],
        "calculate": o.Doc
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
                $(":input[name=TB_" + resultTarget + "]").val(typeof result == "undefined" ? "" : result);
            });
            if (i == 0) {
                $(":input[name=TB_" + target + "]").trigger("change");
            }
        });
    })(targets, expression, o.AttrOfOper, o.MyPK, o.Doc);
    $(":input[name=TB_" + o.AttrOfOper + "]").attr("disabled", true);
}