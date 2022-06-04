//处理MapExt
function AfterBindEn_DealMapExt(tr, rowIndex) {
    var workNode = workNodeData;
    var mapExtArr = workNode.Sys_MapExt; // 扩展信息
    var WorkId = GetQueryString("WorkId")
    //表示从表还没有数据
    var OID = 0;
    if ($(tr).data().data == undefined)
        return;
    else
        OID = $(tr).data().data.OID;
    if (OID == 0)//数据还未保存
        OID = WorkId + "_" + rowIndex;
    for (var i = 0; i < mapExtArr.length; i++) {
        var mapExt = mapExtArr[i];
        //一起转成entity.
        mapExt = new Entity("BP.Sys.MapExt", mapExt.MyPK);
        if (mapExt.ExtType == "DtlImp"
            || mapExt.MyPK.indexOf(mapExt.FK_MapData + '_Table') >= 0
            || mapExt.MyPK.indexOf('PageLoadFull') >= 0)
            continue;

        if (mapExt.AttrOfOper == '')
            continue; //如果是不操作字段，就conntinue;

        var mapAttr = new Entity("BP.Sys.MapAttr");
        mapAttr.SetPKVal(mapExt.FK_MapData + "_" + mapExt.AttrOfOper);
        if (mapAttr.RetrieveFromDBSources() == 0) {
            mapExt.Delete();
            continue;
        }

        //处理Pop弹出框
        var PopModel = mapAttr.GetPara("PopModel");
        if (PopModel != undefined && PopModel != "" && mapExt.ExtType == mapAttr.GetPara("PopModel") && mapAttr.GetPara("PopModel") != "None") {
            var tbAuto = $(tr).find("[name=TB_" + mapExt.AttrOfOper + ']');
            var tbID = tbAuto.attr('id');

            PopDtlMapExt(mapAttr, mapExt, tbID, rowIndex, OID, tr);
            continue;
        }
        //处理文本自动填充
        var TBModel = mapAttr.GetPara("TBFullCtrl");
        if (TBModel != undefined && TBModel != "" && TBModel != "None" && (mapExt.ExtType == "FullData")) {
            var tbAuto = $(tr).find("[name=TB_" + mapExt.AttrOfOper + ']');
            if (tbAuto == null)
                continue;

            var tbID = tbAuto.attr('id');
            if (tbID == undefined)
                continue;

            tbAuto.attr("ondblclick", "ReturnValTBFullCtrl(this,'" + mapExt.MyPK + "');");
            tbAuto.attr("onkeyup", "DoAnscToFillDiv(this,this.value, '" + tbID + "', '" + mapExt.MyPK + "',\'" + TBModel + "\');");
            tbAuto.attr("AUTOCOMPLETE", "OFF");

            continue;
        }

        //下拉框填充其他控件
        var DDLFull = mapAttr.GetPara("IsFullData");
        if (DDLFull != undefined && DDLFull != "" && DDLFull == "1" && (mapExt.MyPK.indexOf("DDLFullCtrl") != -1)) {
            //枚举类型
            if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1 && mapAttr.UIContralType == 3) {
                var ddlOper = $(tr).find('input:radio[name="RB_' + mapExt.AttrOfOper + '"]');
                if (ddlOper.length == 0)
                    continue;

                ddlOper.attr("onchange", "DDLFullCtrl(this.value,\'" + "RB_" + mapExt.AttrOfOper + "\', \'" + mapExt.MyPK + "\')");

                //初始化填充数据
                var val = $('input:radio[name="RB_' + mapExt.AttrOfOper + '"]:checked').val();
                DDLFullCtrl(val, "DDL_" + mapExt.AttrOfOper + "_" + rowIndex, mapExt.MyPK);
                continue;
            }

            //外键类型
            var ddlOper = $("#DDL_" + mapExt.AttrOfOper + "_" + rowIndex);
            if (ddlOper.length == 0)
                continue;

            ddlOper.attr("onchange", "DDLFullCtrl(this.value,\'" + "DDL_" + mapExt.AttrOfOper + "_" + rowIndex + "\', \'" + mapExt.MyPK + "\')");
            //初始化填充数据
            var val = ddlOper.val();
            if (val != "" && val != undefined)
                DDLFullCtrl(val, "DDL_" + mapExt.AttrOfOper + "_" + rowIndex, mapExt.MyPK);
            continue;
        }

        switch (mapExt.ExtType) {
            case "MultipleChoiceSmall":
                var tbMulti = $(tr).find("[name=TB_" + mapExt.AttrOfOper + ']');
                var tbID = tbMulti.attr('id');
                if (mapAttr.UIIsEnable == 0 && mapExt.Tag == 0) {
                    var oid = (pageData.WorkID || pageData.OID || "");
                    var ens = new Entities("BP.Sys.FrmEleDBs");
                    ens.Retrieve("FK_MapData", mapAttr.FK_MapData, "EleID", mapAttr.KeyOfEn, "RefPKVal", oid);
                    var val = "";
                    var defaultVal = tbMulti.val();
                    for (var k = 0; k < ens.length; k++) {
                        if (defaultVal.indexOf(ens[k].Tag1) == -1)
                            continue;
                        val += ens[k].Tag2 + ",";
                    }
                    tbMulti.val(val);
                    break;
                }
                MultipleChoiceSmall(mapExt, mapAttr, workNode, tbID, rowIndex, OID); //调用 /CCForm/JS/MultipleChoiceSmall.js 的方法来完成.
                break;
            case "MultipleChoiceSearch":
                if (mapAttr.UIIsEnable == 0)
                    break;
                var tbMulti = $(tr).find("[name=TB_" + mapExt.AttrOfOper + ']');
                var tbID = tbMulti.attr('id');
                MultipleChoiceSearch(mapExt, mapAttr, tbID, rowIndex, OID); //调用 /CCForm/JS/MultipleChoiceSearch.js 的方法来完成.
                break;

            case "RegularExpression": //正则表达式  统一在保存和提交时检查
                var tb = $(tr).find('[name$=' + mapExt.AttrOfOper + ']');

                if (tb.attr('class') != undefined && tb.attr('class').indexOf('CheckRegInput') > 0) {
                    break;
                } else {
                    tb.addClass("CheckRegInput");
                    tb.data(mapExt)
                    tb.attr(mapExt.Tag, "CheckRegInput('" + tb.attr('id') + "','','" + mapExt.Tag1 + "')");
                }
                break;
            case "InputCheck": //输入检查
                if (mapExt.AttrOfOper.length == 0 || mapExt.Tag1.length == 0 || mapExt.Tag2.length == 0) {
                    continue;
                }
                //判断是否已经增加了函数
                var ctrl = $(tr).find('[name$=' + mapExt.AttrOfOper + ']');

                if (ctrl.length == 0) {
                    continue;
                }

                var changeEvent = ctrl.attr(mapExt.Tag2);
                if (changeEvent && changeEvent.indexOf(mapExt.Tag1 + '(') != -1) {
                    continue;
                }

                ctrl.attr(mapExt.Tag2, mapExt.Tag1 + '(this);');
                break;
            case "BindFunction": //绑定函数
                if (mapExt.AttrOfOper.length == 0 || mapExt.Doc.length == 0 || mapExt.Tag.length == 0) {
                    continue;
                }
                //判断是否已经增加了函数
                var ctrl = $(tr).find('[name=TB_' + mapExt.AttrOfOper + ']');

                if (ctrl.length == 0) {
                    ctrl = $(tr).find('[name=DDL_' + mapExt.AttrOfOper + ']');
                    if (ctrl.length == 0)
                        ctrl = $(tr).find('[name=RB_' + mapExt.AttrOfOper + ']');
                    if (ctrl.length == 0)
                        ctrl = $(tr).find('[name=CB_' + mapExt.AttrOfOper + ']');
                }

                if (ctrl.length == 0)
                    continue;
                //获取ctr是否包含Wdate的class,有则是时间控件
                if (ctrl.hasClass("Wdate")) {
                    var mapAttr = new Entity("BP.Sys.MapAttr", mapExt.FK_MapData + "_" + mapExt.AttrOfOper);

                    var minDate = ctrl.attr("data-info");
                    ctrl.attr("data-funcionPK", mapExt.MyPK); // 记录绑定事件的MyPK
                    ctrl.removeAttr("onfocus");
                    ctrl.unbind("focus");
                    var frmDate = mapAttr.IsSupperText; //获取日期格式
                    var dateFmt = '';
                    if (frmDate == 0) {
                        dateFmt = "yyyy-MM-dd";
                    } else if (frmDate == 1) {
                        dateFmt = "yyyy-MM-dd HH:mm";
                    } else if (frmDate == 2) {
                        dateFmt = "yyyy-MM-dd HH:mm:ss";
                    } else if (frmDate == 3) {
                        dateFmt = "yyyy-MM";
                    } else if (frmDate == 4) {
                        dateFmt = "HH:mm";
                    } else if (frmDate == 5) {
                        dateFmt = "HH:mm:ss";
                    } else if (frmDate == 6) {
                        dateFmt = "MM-dd";
                    } else if (frmDate == 7) {
                        dateFmt = "yyyy";
                    }

                    var mapextDoc = mapExt.Doc;
                    ctrl.data().MapExt = mapExt;
                    ctrl.bind("focus", function () {
                        if (minDate == "" || minDate == undefined)
                            WdatePicker({
                                dateFmt: dateFmt, onpicked: function (dp) {
                                    $(this).blur(); //失去焦点
                                    var id = ctrl[0].id;
                                    curRowIndex = id.substring(id.lastIndexOf("_") + 1);
                                    DBAccess.RunFunctionReturnStr(mapextDoc);
                                }
                            });
                        else
                            WdatePicker({
                                dateFmt: dateFmt, minDate: minDate, onpicked: function (dp) {
                                    $(this).blur(); //失去焦点
                                    var id = ctrl[0].id;
                                    curRowIndex = id.substring(id.lastIndexOf("_") + 1);
                                    DBAccess.RunFunctionReturnStr(mapextDoc);
                                }
                            });
                    });
                    break;

                }

                ctrl.data().MapExt = mapExt;
                ctrl.bind(mapExt.Tag, function (obj) {
                    var id = ctrl[0].id;
                    curRowIndex = id.substring(id.lastIndexOf("_") + 1);
                    var mapExtThis = $(obj.target).data().MapExt;
                    if (mapExtThis.Doc.indexOf("(") != -1)
                        cceval(mapExtThis.Doc);
                    else
                        cceval(mapExtThis.Doc + '(this)');
                });
                break;
            case "DataFieldInputRole": //时间限制

                //判断是否已经增加了函数
                var ctrl = $(tr).find('[name=TB_' + mapExt.AttrOfOper + ']');
                if (ctrl.length == 0)
                    break;

                if (mapExt.DoWay == 1) {
                    var tag1 = mapExt.Tag1;
                    if (tag1 == 1) {
                        var mapAttr = new Entity("BP.Sys.MapAttr", mapExt.FK_MapData + "_" + mapExt.AttrOfOper);
                        ctrl.removeAttr("onfocus");
                        var frmDate = mapAttr.IsSupperText; //获取日期格式
                        var dateFmt = '';
                        if (frmDate == 0) {
                            dateFmt = "yyyy-MM-dd";
                        } else if (frmDate == 1) {
                            dateFmt = "yyyy-MM-dd HH:mm";
                        } else if (frmDate == 2) {
                            dateFmt = "yyyy-MM-dd HH:mm:ss";
                        } else if (frmDate == 3) {
                            dateFmt = "yyyy-MM";
                        } else if (frmDate == 4) {
                            dateFmt = "HH:mm";
                        } else if (frmDate == 5) {
                            dateFmt = "HH:mm:ss";
                        } else if (frmDate == 6) {
                            dateFmt = "MM-dd";
                        } else if (frmDate == 7) {
                            dateFmt = "yyyy";
                        }

                        var minDate = '%y-%M-#{%d}';
                        ctrl.attr("data-info", minDate); //绑定时间大小限制的记录
                        var functionPK = ctrl.attr("data-funcionPK");
                        if (functionPK == null || functionPK == undefined || functionPK == "") {
                            ctrl.bind("focus", function () {
                                WdatePicker({ dateFmt: dateFmt, minDate: minDate });
                            });
                        } else {
                            ctrl.unbind("focus");
                            var bindFunctionExt = null;
                            for (var idx = 0; idx < mapExtArr.length; idx++) {
                                if (mapExtArr[idx].MyPK == functionPK) {
                                    bindFunctionExt = mapExtArr[idx];
                                    break;
                                }
                            }
                            if (bindFunctionExt == null)
                                ctrl.bind("focus", function () {
                                    var id = ctrl[0].id;
                                    curRowIndex = id.substring(id.lastIndexOf("_") + 1);
                                    WdatePicker({ dateFmt: dateFmt, minDate: minDate });
                                });
                            else
                                ctrl.bind("focus", function () {

                                    WdatePicker({
                                        dateFmt: dateFmt, minDate: minDate, onpicked: function (dp) {
                                            $(this).blur(); //失去焦点
                                            var id = ctrl[0].id;
                                            curRowIndex = id.substring(id.lastIndexOf("_") + 1);
                                            DBAccess.RunFunctionReturnStr(bindFunctionExt.Doc);
                                        }
                                    });
                                });

                        }


                    }

                }
                break;
            case "ReqDays": //配置自动计算日期天数lz
                //获取配置的字段

                var ResRDT = mapExt.AttrOfOper;//接收计算天数结果
                var StarRDT = mapExt.Tag1;//开始日期
                var EndRDT = mapExt.Tag2;//结束日期
                var RDTRadio = mapExt.Tag3;//是否包含节假日 0包含，1不包含
                var res = "";
                var result = $(tr).find("[name=TB_" + mapExt.AttrOfOper + ']');
                var end = $(tr).find("[name=TB_" + EndRDT + ']');
                var start = $(tr).find("[name=TB_" + StarRDT + ']');
                if (end == null || start == null)
                    continue;
                end.focus(function () {
                    result.val("");
                });
                //当结束日期文本框失去焦点时
                end.blur(function () {
                    //计算量日期天数
                    res = CalculateRDT(start.val(), end.val(), RDTRadio);
                    if (res == "" || res == "NaN")
                        end.val("");
                    result.val(res);
                });

                break;
            case "ActiveDDL": /*自动初始化ddl的下拉框数据. 下拉框的级联操作 已经 OK*/
                var ddlParent = $(tr).find("[name=DDL_" + mapExt.AttrOfOper + ']');
                var ddlChild = $(tr).find("[name=DDL_" + mapExt.AttrsOfActive + ']');
                if (ddlParent == null || ddlChild == null)
                    continue;
                var rowIndex = (parseInt($($(tr).find('td')[0]).text()) - 1);
                ddlParent.data().MapExt = mapExt;
                ddlParent.bind('change', function (obj) {
                    var trEle = $(obj.target).parent().parent();
                    var value = $(obj.target).val();
                    var mapExtThis = $(obj.target).data().MapExt;
                    var mapAttrOfActiveEleId = $(trEle).find("[name='DDL_" + mapExtThis.AttrsOfActive + "']").attr('id');
                    //获取这一行的数据
                    var rowIndexThis = (parseInt($($(trEle).find('td')[0]).text()) - 1);
                    setTrDataByData(rowIndexThis);
                    DDLAnsc(value, mapAttrOfActiveEleId, mapExtThis.MyPK, $(trEle).data().data);
                });

                DDLAnsc(ddlParent.val(), ddlChild.attr('id'), mapExt.MyPK, $(ddlParent.parent().parent()).data().data);

                break;
            case "AutoFullDLL": // 自动填充下拉框.
                continue; //已经处理了。
            case "AutoFull": //自动填充  //a+b=c DOC='@DanJia*@ShuLiang'  等待后续优化
                //循环  KEYOFEN
                //替换@变量
                //处理 +-*%

                //直接替换
                if (mapExt.Doc != undefined && mapExt.Doc != '') {
                    //以 + -* 、% 来分割
                    //先来计算  + -* 、%  的位置
                    if (mapExt.Doc.indexOf('+') > 0 || mapExt.Doc.indexOf('-') > 0 || mapExt.Doc.indexOf('*') > 0 || mapExt.Doc.indexOf('/') > 0) {
                        var mapExtDocArr1 = []; // 字段@field
                        var lastOperatorIndex = -1;
                        var operatorArr = []; // 计算符+-*/
                        for (var j = 0; j < mapExt.Doc.length; j++) {
                            if (mapExt.Doc[j] == "+" || mapExt.Doc[j] == "-" || mapExt.Doc[j] == "*" || mapExt.Doc[j] == "/") {
                                operatorArr.push(mapExt.Doc[j]);

                                mapExtDocArr1.push(mapExt.Doc.substring(lastOperatorIndex + 1, j));
                                lastOperatorIndex = j;
                            }
                        }
                        mapExtDocArr1.push(mapExt.Doc.substring(lastOperatorIndex + 1, mapExt.Doc.length))

                        for (var m = 0; m < mapExtDocArr1.length; m++) {
                            var extDocObj1 = mapExtDocArr1[m].replace('@', '').replace('(', '').replace(')', '');
                            //将extDocObj1转换成KeyOfEn
                            var extObjAr = $.grep(workNodeData.Sys_MapAttr, function (val) {
                                return val.Name == extDocObj1 || val.KeyOfEn == extDocObj1;
                            });

                            if (extObjAr.length == 0) {
                                // alert("mapExt:" + mapExt.AttrOfOper + "配置有误");
                            } else {
                                extDocObj1 = extObjAr[0].KeyOfEn;
                                $(tr).find('[name=TB_' + mapExt.AttrOfOper + ']').attr('disabled', true);
                                var targetObj = $(tr).find('[name=TB_' + extDocObj1 + ']');
                                if (targetObj.length == 0) {
                                    targetObj = $(tr).find('[name=DDL_' + extDocObj1 + ']');
                                    if (targetObj.length == 0) {
                                        targetObj = $(tr).find('[name=RB_' + extDocObj1 + ']')
                                        if (targetObj.length > 0) {
                                            targetObj.data().mapExt = mapExt;
                                            targetObj.bind('click', function (obj) {
                                                AutoFull(tr, obj, "RB_");
                                            });
                                        }
                                    } else {
                                        targetObj.data().mapExt = mapExt;
                                        targetObj.bind('change', function (obj) {
                                            AutoFull(tr, obj, "DDL_");
                                        });
                                    }

                                } else {
                                    targetObj.data().mapExt = mapExt;
                                    targetObj.bind('blur', function (obj) {
                                        AutoFull(tr, obj, "TB_");
                                    });
                                }





                                /**
                                * 页面初始化后触第一个计算元素的onblur事件, 让表格行统计生效
                                */
                                if (m == 0) {	// 每一行第一列触发一次
                                    //$(tr).find('[name=TB_' + extDocObj1 + ']').trigger("blur");
                                    /**
                                    * 该动作与<body onblur="SaveAll(this)">有冲突, 即:
                                    *   页面初始化InitPage()调用到AfterBindEn_DealMapExt()并运行到这里
                                    *   当input触发onblur事件后, body的onblur事件就被触发, 并调用SaveAll()方法
                                    *   SaveAll()方法又调用AjaxServiceGener()方法
                                    *   AjaxServiceGener()方法提交到后台返回后再次调用了InitPage()方法, 形成死循环
                                    *
                                    * 暂时将SaveAll()禁用
                                    * 或将blur事件改为onchange事件
                                    */
                                }

                            }
                        }

                    }
                }
                break;
        }
    }
}
