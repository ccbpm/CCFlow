/**
 * 开发者表单的解析
 * @param {any} mapData 表单属性
 * @param {any} fk_mapData 表单数据
 */
var currentURL = GetHrefUrl();
var frmData;
var isShowMustInput = getConfigByKey("FrmDevelop_IsShowStar", '1');

//图片签名
var isButtonShowSignature = getConfigByKey("IsButtonShowSignature", false);
var UserICon = getConfigByKey("UserICon", '../DataUser/Siganture/'); //获取签名图片的地址
var UserIConExt = getConfigByKey("UserIConExt", '.jpg');  //签名图片的默认后缀

function GenerDevelopFrm(wn, fk_mapData, isComPare) {
    if (isComPare == null || isComPare == undefined || isComPare == "")
        isComPare = false;
    $("head").append("<style>.layui-form-radio{margin:0px;padding-right:0px}</style>")
    frmData = wn;
    var htmlContent = "";
    //数据库中查找
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_DevelopDesigner");
    handler.AddPara("FK_MapData", fk_mapData);
    htmlContent = handler.DoMethodReturnString("Designer_Init");

    if (htmlContent == "") {
        layer.alert("开发者设计的表单内容丢失，请联系管理员");
        return;
    }
    if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1) {
        htmlContent = htmlContent.replace(new RegExp("../../../", 'gm'), "../../");
    } else if (currentURL.indexOf("AdminFrm.htm") != -1) {
        //不做替换
    } else {
        htmlContent = htmlContent.replace(new RegExp("../../../", 'gm'), "../");
    }

    //获取版本
    var ver = GetPara(frmData.MainTable[0].AtPara, "FrmVer");
    ver = ver == null || ver == undefined || ver == "" ? 0 : parseInt(ver);
    var mainFrmID = GetPara(frmData.Sys_MapData[0].AtPara, "MainFrmID");
    var isSameVer = mainFrmID == fk_mapData ? true : false;
    if (isSameVer == false) {
        htmlContent = replaceAll(htmlContent, fk_mapData, mainFrmID);
    }
    $("#CCForm").html(htmlContent);

    //解析表单中的数据

    var mapAttrs = frmData.Sys_MapAttr;
    var html = "";
    for (var i = 0; i < mapAttrs.length; i++) {
        var mapAttr = mapAttrs[i];
        //1.加载隐藏字段，
        if (mapAttr.UIVisible == 0) {
            if ($("#TB_" + mapAttr.KeyOfEn).length == 1)
                SetDevelopCtrlHidden(mapAttr.KeyOfEn);
            $("#TB_" + mapAttr.KeyOfEn).hide();
            $("#DDL_" + mapAttr.KeyOfEn).hide();
            $("input[name=CB_" + mapAttr.KeyOfEn + "]").hide();
            $("input[name=RB_" + mapAttr.KeyOfEn + "]").hide();
            if (mapAttr.UIVisible == 0 && mapAttr.UIIsEnable == 0)
                $("input[name=RB_" + mapAttr.KeyOfEn + "]").attr("disabled", "disabled");
            continue;
        }

        //必填属性
        if (mapAttr.UIIsEnable == 1 && mapAttr.UIIsInput && isShowMustInput == 1) {
            var mustInput = "<span style='color:red' class='mustInput' data-keyofen='" + mapAttr.KeyOfEn + "' >*</span>";
            $("#TB_" + mapAttr.KeyOfEn).after(mustInput);
            $("#DDL_" + mapAttr.KeyOfEn).after(mustInput);
            $("#CB_" + mapAttr.KeyOfEn).parent().append(mustInput);
        }

        //设置字段的样式属性
        $('#TB_' + mapAttr.KeyOfEn).addClass(mapAttr.CSS);
        $('#RB_' + mapAttr.KeyOfEn).addClass(mapAttr.CSS);
        $('#DDL_' + mapAttr.KeyOfEn).addClass(mapAttr.CSS);
        $('#CB_' + mapAttr.KeyOfEn).addClass(mapAttr.CSS);
        //富文本编辑器
        if (mapAttr.TextModel == 3) {
            var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
            if (mapAttr.UIIsEnable == "0" || isReadonly == true) {
                //只读状态直接 div 展示富文本内容
                defValue = defValue.replace(/white-space: nowrap;/g, "");
                var eleHtml = "<div style='margin:9px 0px 9px 15px'>" + defValue + "</div>";
                var element = $("#TB_" + mapAttr.KeyOfEn);
                element.after(eleHtml);
                element.remove(); //移除节点
            } else {
                if (richTextType == "tinymce") {
                    var element = $("#TB_" + mapAttr.KeyOfEn);
                    element.addClass("rich");
                    element.css("height", mapAttr.UIHeight).css("width", "100%");
                } else {
                    if (mapAttr.UIHeight < 180) {
                        mapAttr.UIHeight = 180;
                    }
                    //设置编辑器的默认样式
                    var styleText = "text-align:left;font-size:12px;";
                    styleText += "width:100%;";
                    var height = parseInt(mapAttr.UIHeight) - 54;

                    styleText += "height:" + height + "px;";
                    //注意这里 name 属性是可以用来绑定表单提交时的字段名字的 id 是特殊约定的.
                    var eleHtml = "<script class='EditorClass' id='editor_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";
                    var element = $("#TB_" + mapAttr.KeyOfEn);
                    element.after(eleHtml);
                    element.remove(); //移除节点
                }
            }
        }
        //审核组件、评论组件
        if (mapAttr.UIContralType == 14 || mapAttr.UIContralType == 15)
            $('#TB_' + mapAttr.KeyOfEn).removeAttr("placeholder");
        //公文
        if (mapAttr.UIContralType == 17)
            $('#TB_' + mapAttr.KeyOfEn).attr("placeholder", "请单击进行编辑");

        //如果是时间控件
        if (mapAttr.MyDataType == 6 || mapAttr.MyDataType == 7) {
            var frmDate = mapAttr.IsSupperText; //获取日期格式
            var dateFmt = "";
            var dateType = "";
            if (frmDate == 0) {
                dateFmt = "yyyy-MM-dd";
                dateType = "date"
            } else if (frmDate == 1) {
                dateFmt = "yyyy-MM-dd HH:mm";
                dateType = "datetime";
            } else if (frmDate == 2) {
                dateFmt = "yyyy-MM-dd HH:mm:ss";
                dateType = "datetime";
            } else if (frmDate == 3) {
                dateFmt = "yyyy-MM";
                dateType = "month";
            } else if (frmDate == 4) {
                dateFmt = "HH:mm";
                dateType = "time";
            } else if (frmDate == 5) {
                dateFmt = "HH:mm:ss";
                dateType = "time";
            } else if (frmDate == 6) {
                dateFmt = "MM-dd";
                dateType = "date";
            } else if (frmDate == 6) {
                dateFmt = "yyyy";
                dateType = "year";
            }

            var element = $('#TB_' + mapAttr.KeyOfEn);
            element.wrap("<div style=' position: relative;display:inline-block'></div>");
            element.before("<i class='input-icon layui-icon layui-icon-date'></i>");
            element.attr("data-info", dateFmt);
            element.attr("data-type", dateType);
            element.addClass("ccdate")
            if (mapAttr.UIIsEnable == 0 || isReadonly == true)
                element.attr("disabled", "disabled");
            continue;
        }


        //数值型的字段
        if (mapAttr.UIIsEnable != 0 && isReadonly == false
            //浮点型，双精度，整型，金额类型
            && (mapAttr.MyDataType == 5 || mapAttr.MyDataType == 3 || (mapAttr.MyDataType == 2 && mapAttr.LGType == 0) || mapAttr.MyDataType == 8)) {
            var obj = $("#TB_" + mapAttr.KeyOfEn);
            if (mapAttr.IsSecret)
                obj.attr("type", password);
            var bit = 0;
            var defVal = mapAttr.DefVal;
            if (defVal != null && defVal !== "" && defVal.indexOf(".") >= 0)
                bit = defVal.substring(defVal.indexOf(".") + 1).length;

            if (bit == null || bit == undefined || bit == "")
                bit = 0;
            var minNum = GetPara(mapAttr.AtPara, "NumMin") || "";
            var maxNum = GetPara(mapAttr.AtPara, "NumMax") || "";

            obj.attr("data-bit", bit);
            if (minNum != "")
                obj.attr("data-min", minNum);
            if (maxNum != "")
                obj.attr("data-max", maxNum);

            obj.bind('focus', function () {
                removeplaceholder(this, parseInt($(this).attr("data-bit")));
            });

            obj.bind('blur', function () {
                addplaceholder(this, parseInt($(this).attr("data-bit")));
                if (this.getAttribute("data-type") == "Money") {
                    numberFormat(this, parseInt($(this).attr("data-bit")));
                    FormatMoney(this, parseInt($(this).attr("data-bit")), ',', 1);
                }

            });


            obj.bind('keyup', function () {
                limitLength(this, parseInt($(this).attr("data-bit")));
                if (this.getAttribute("data-type") == "Int")
                    valitationAfter(this, 'int');

                if (this.getAttribute("data-type") == "Float" || this.getAttribute("data-type") == "Money") {
                    valitationAfter(this, 'float');
                    limitLength(this, parseInt($(this).attr("data-bit")));
                    if (this.getAttribute("data-type") == "Money")
                        FormatMoney(this, parseInt($(this).attr("data-bit")), ',', 0);
                }


            });

            continue;
        }

        //外部数据源、外键的选择列表
        if ((mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1)
            || (mapAttr.LGType == "2" && mapAttr.MyDataType == "1")) {
            var _html = InitDDLOperation(frmData, mapAttr, null);
            $("#DDL_" + mapAttr.KeyOfEn).empty();
            $("#DDL_" + mapAttr.KeyOfEn).append(_html);
            $("#DDL_" + mapAttr.KeyOfEn).attr("lay-filter", mapAttr.KeyOfEn);
            $("#DDL_" + mapAttr.KeyOfEn).addClass("ddl-ext");
            continue;
        }
        //图片签名、盖章
        if (mapAttr.MyDataType == "1" && (mapAttr.IsSigan == "1" || mapAttr.IsSigan == '4')) {
            //隐藏该字段值
            debugger
            $("#TB_" + mapAttr.KeyOfEn).hide();
            var html = "";
            var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
            var localHref = GetLocalWFPreHref();
            if (mapAttr.UIIsEnable == 1 && pageData.IsReadonly == 0) {
                //显示按钮
                if (isButtonShowSignature == true && mapAttr.IsSigan == '1') {
                    html = '<input type="button" value="点击签名" id="Btn_' + mapAttr.KeyOfEn + '" class="Btn" onclick="ChangeSigantureState(\'' + mapAttr.KeyOfEn + '\',1)"/>';
                    if (val != "")
                        html += "<img src='" + UserICon + "/" + val + UserIConExt + "' alt='" + val + "'   style='border:0px;width:105px;height:45px;padding-left:20px;position: absolute;mix-blend-mode:normal;' id='Img_" + mapAttr.KeyOfEn + "' />";

                }
                else if (isButtonShowSignature == true && mapAttr.IsSigan == '4') {
                    html = '<input type="button" value="点击盖章" id="Btn_' + mapAttr.KeyOfEn + '" class="Btn" onclick="ChangeSigantureState(\'' + mapAttr.KeyOfEn + '\',4)"/>';
                    if (val != "")
                        html += "<img src='" + UserICon + "/" + val + UserIConExt + "' alt='" + val + "'   style='border:0px;width:105px;height:45px;' id='Img_" + mapAttr.KeyOfEn + "' />";

                } else {
                    if (val != "")
                        val = webUser.No;
                    html = "<img src='" + UserICon + "/" + val + UserIConExt + "' alt='" + val + "'   style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />";
                }
            } else {
                if (val != "") {
                    if (mapAttr.IsSigan == '1')
                        html = "<img src='" + UserICon + "/" + val + UserIConExt + "' alt='" + val + "'   style='border:0px;width:105px;height:45px;position: absolute;mix-blend-mode:normal;' id='Img" + mapAttr.KeyOfEn + "' />";
                    if (mapAttr.IsSigan == '4')
                        html = "<img src='" + UserICon + "/" + val + UserIConExt + "' alt='" + val + "'   style='border:0px;width:175px;height:50px;' id='Img" + mapAttr.KeyOfEn + "' />";
                }
            }

            $("#TB_" + mapAttr.KeyOfEn).after(html);
            continue;

        }

        //为复选框高级设置绑定事件
        if (mapAttr.MyDataType == 4) {
            var obj = $("#CB_" + mapAttr.KeyOfEn);
            $("#CB_" + mapAttr.KeyOfEn).attr("value", "1");
            $("#CB_" + mapAttr.KeyOfEn).attr("lay-skin", "primary");
            $("#CB_" + mapAttr.KeyOfEn).attr("lay-filter", mapAttr.KeyOfEn);
            continue;
        }
        //为单选按钮高级设置绑定事件
        if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {
            //枚举下拉框
            if (mapAttr.UIContralType == 1) {
                //重新生成枚举下拉框的值
                var _html = InitDDLOperation(frmData, mapAttr, null);
                $("#DDL_" + mapAttr.KeyOfEn).empty();
                $("#DDL_" + mapAttr.KeyOfEn).append(_html);
                $("#DDL_" + mapAttr.KeyOfEn).attr("lay-filter", mapAttr.KeyOfEn);
            }
            //枚举单选
            if (mapAttr.UIContralType == 3) {
                //重新设置单选按钮的值
                var _html = "";

                //显示方式,默认为横向展示.
                var RBShowModel = 3;
                if (mapAttr.AtPara.indexOf('@RBShowModel=0') > 0)
                    RBShowModel = 0;
                var oldChildren = $("#SR_" + mapAttr.KeyOfEn).children();
                $("#SR_" + mapAttr.KeyOfEn).empty();

                $.each(frmData.Sys_Enum, function (i, obj) {
                    if (obj.EnumKey == mapAttr.UIBindKey) {
                        var $_oldEle = oldChildren[0];
                        var $_cloneEle = $($_oldEle).clone(true);//复制一份

                        //属性更新
                        $_cloneEle.find("input").attr("id", "RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey);
                        $_cloneEle.find("input").attr("name", "RB_" + mapAttr.KeyOfEn);
                        $_cloneEle.find("input").val(obj.IntKey);
                        $_cloneEle.find("input").attr("data-key", mapAttr.KeyOfEn);
                        $_cloneEle.find("input").attr("lay-filter", mapAttr.KeyOfEn);

                        $("#SR_" + mapAttr.KeyOfEn).append($_cloneEle.find("input")).append(obj.Lab + "&nbsp;");
                    }

                });

                if (mapAttr.UIIsEnable == 1 && mapAttr.UIIsInput && isShowMustInput == 1) {
                    var mustInput = "<span style='color:red' class='mustInput' data-keyofen='" + mapAttr.KeyOfEn + "' >*</span>";
                    $("#SR_" + mapAttr.KeyOfEn).after(mustInput);
                }

            }
            continue;
        }
        //枚举多选复选框
        if (mapAttr.MyDataType == "1" && mapAttr.UIContralType == "2") {
            //显示方式,默认为横向展示.
            var RBShowModel = 3;
            if (mapAttr.AtPara.indexOf('@RBShowModel=0') > 0)
                RBShowModel = 0;
            var _html = "";
            $.each(frmData.Sys_Enum, function (i, obj) {
                if (obj.EnumKey == mapAttr.UIBindKey) {
                    var br = "";
                    if (RBShowModel == 0)
                        br = "<br>";
                    _html += "<input style='vertical-align:-1px;' class='mcheckbox' type=checkbox lay-skin='primary' name='CB_" + mapAttr.KeyOfEn + "' id='CB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "' title='" + obj.Lab + "' />";
                }
            });
            $("#SC_" + mapAttr.KeyOfEn).empty();
            $("#SC_" + mapAttr.KeyOfEn).append(_html);
            if (mapAttr.UIIsEnable == 1 && mapAttr.UIIsInput && isShowMustInput == 1) {
                var mustInput = "<span style='color:red' class='mustInput' data-keyofen='" + mapAttr.KeyOfEn + "' >*</span>";
                $("#SC_" + mapAttr.KeyOfEn).after(mustInput);
            }
            continue;
        }

        if (mapAttr.MyDataType == 1) {
            if (mapAttr.UIContralType == 8)//手写签字版
            {
                var element = $("#Img" + mapAttr.KeyOfEn);
                var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
                var ondblclick = mapAttr.UIIsEnable == 1 && isReadonly == false ? " ondblclick='figure_Template_HandWrite(\"" + mapAttr.KeyOfEn + "\",\"" + defValue + "\")'" : "";
                if (defValue && defValue != "")
                    defValue = defValue.substring(defValue.indexOf("DataUser") + 8);
                //路径的判断
                var baseUrl = "";
                if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
                    baseUrl = "../../DataUser";
                else if (currentURL.indexOf("AdminFrm.htm") != -1)
                    baseUrl = "../../../DataUser";
                else
                    baseUrl = "../DataUser";

                var eleHtml = "<img src='" + baseUrl + defValue + "' " + ondblclick + " onerror=\"this.src='" + baseUrl + "/Siganture/UnName.jpg'\"  style='border:0px;width:100px;height:30px;' id='Img" + mapAttr.KeyOfEn + "' />";
                eleHtml += "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "'  value='" + defValue + "' type=hidden />";
                element.after(eleHtml);
                element.remove(); //移除Imge节点
                continue;
            }
            if (mapAttr.UIContralType == 4)//地图
            {
                var obj = $("#TB_" + mapAttr.KeyOfEn);
                //获取兄弟节点
                $(obj.prev()).attr("onclick", "figure_Template_Map('" + mapAttr.KeyOfEn + "','" + mapAttr.UIIsEnable + "')");
                continue;
            }

            if (mapAttr.UIContralType == 6) {//字段附件
                var _html = getFieldAth(mapAttr,frmData.Sys_FrmAttachment);
                $("#TB_" + mapAttr.KeyOfEn).hide();
                $("#TB_" + mapAttr.KeyOfEn).after(_html);

                continue;
            }
            if (mapAttr.UIContralType == 101)//评分
            {
                var scores = $(".simplestar");//获取评分的类
                $.each(scores, function (idx, score) {
                    if (score.id != "Star_" + mapAttr.KeyOfEn)
                        return true;
                    var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
                    $.each($(this).children("Img"), function (index) {
                        if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
                            $(this).attr("src", $(this).attr("src").replace("../../", "../"));
                        else if (currentURL.indexOf("AdminFrm.htm") != -1) {
                            //不做处理
                        } else
                            $(this).attr("src", $(this).attr("src").replace("../../", "./"));
                        if (defValue != "" && index < parseInt(defValue))
                            $(this).attr("src", $(this).attr("src").replace("star_1.png", "star_2.png"));
                    });
                });
                continue;
            }
        }
    }



    //2.解析控件 从表、附件、附件图片、框架、地图、签字版、父子流程
    var frmDtls = frmData.Sys_MapDtl;
    if (frmDtls && frmDtls.length > 0) {
        for (var i = 0; i < frmDtls.length; i++) {
            var frmDtl = frmDtls[i];
            var dtlNo = frmDtl.No;
            if (isSameVer == false)
                dtlNo = dtlNo.replace(fk_mapData, mainFrmID);

            //根据data-key获取从表元素
            var element = $("Img[data-key=" + dtlNo + "]");
            if (element.length == 0)
                continue;
            var prev = $(element).parent().prev();
            if (prev.length > 0 && prev[0].innerHTML.indexOf(frmDtl.Name) != -1)
                $(prev[0]).attr("id", "Lab_" + frmDtl.No);

            if (frmDtl.IsView == 0) {
                $(element).hide();
                $("#Lab_" + frmDtl.No).hide();
                continue;
            }
            if (isComPare == true) {
                var eleHtml = $("<div id='Dtl_" + frmDtl.No + "' name='Dtl' style='height:auto;margin:5px 10px;border-top:1px solid #D0D0D0' ></div>");
                $(element).after(eleHtml);
                $(element).remove();
            } else {
                figure_Develop_Dtl(element, frmDtl);
            }
        }
    }

    var aths = frmData.Sys_FrmAttachment;//附件

    //表格附件
    if (aths && aths.length > 0) {
        $.each(aths, function (idex, ath) {
            var mypk = ath.MyPK;
            if (isSameVer == false)
                mypk = mypk.replace(fk_mapData, mainFrmID);
            var element = $("Img[data-key=" + mypk + "]");
            if (element.length != 0) {
                var prev = $(element).parent().prev();
                if (prev.length > 0 && prev[0].innerHTML.indexOf(ath.Name) != -1)
                    $(prev[0]).attr("id", "Lab_" + ath.No);

                var eleHtml = $("<div id='Div_" + ath.MyPK + "' name='Ath' style=' height:auto;margin:5px 10px' ></div>");
                $(element).after(eleHtml);
                $(element).remove(); //移除Imge节点
                AthTable_Init(ath, "Div_" + ath.MyPK);
            }
        });
    }


    //图片附件
    var athImgs = frmData.Sys_FrmImgAth;
    if (athImgs && athImgs.length > 0) {
        var imgSrc = "<input type='hidden' id='imgSrc'/>";
        $('#CCForm').append(imgSrc);
        for (var i = 0; i < athImgs.length; i++) {
            var athImg = athImgs[i];
            var athImg = athImgs[i];
            var mypk = athImg.MyPK;
            if (isSameVer == false)
                mypk = mypk.replace(fk_mapData, mainFrmID);
            //根据data-key获取从表元素
            var element = $("img[data-key=" + mypk + "]");
            if (element.length == 0)
                continue;
            figure_Develop_ImageAth(element, athImg, fk_mapData);

        }
    }


    //图片
    var imgs = frmData.Sys_FrmImg;
    if (imgs && imgs.length > 0) {
        for (var i = 0; i < imgs.length; i++) {
            var img = imgs[i];
            var mypk = img.MyPK;
            if (isSameVer == false)
                mypk = mypk.replace(fk_mapData, mainFrmID);
            //根据data-key获取从表元素
            var element = $("Img[data-key=" + mypk + "]");
            if (element.length == 0)
                continue;
            figure_Develop_Image(element, img);

        }
    }

    var iframes = frmData.Sys_MapFrame;//框架
    if (iframes && iframes.length > 0) {
        for (var i = 0; i < iframes.length; i++) {
            var iframe = iframes[i];
            //根据data-key获取从表元素
            var mypk = iframe.MyPK;
            if (isSameVer == false)
                mypk = mypk.replace(fk_mapData, mainFrmID);
            var element = $("Img[data-key=" + mypk + "]");
            if (element.length == 0)
                continue;
            figure_Develop_IFrame(element, iframe);

        }
    }

    if (frmData.WF_FrmNodeComponent == null || frmData.WF_FrmNodeComponent == undefined) {
        var element = $("Img[data-type=WorkCheck]");
        if (element.length != 0)
            $(element).remove();
        element = $("Img[data-type=SubFlow]");
        if (element.length != 0)
            $(element).remove();
    }

    //审核组件的判断
    if (frmData.WF_FrmNodeComponent != null && frmData.WF_FrmNodeComponent != undefined) {
        var nodeComponents = frmData.WF_FrmNodeComponent[0];//节点组件
        if (nodeComponents != null) {
            var element = $("Img[data-type=WorkCheck]");
            if (element.length != 0)
                figure_Develop_FigureFrmCheck(nodeComponents, element, frmData);

            element = $("Img[data-type=SubFlow]");
            if (element.length != 0)
                figure_Develop_FigureSubFlowDtl(nodeComponents, element);


        }
    }

    if ($("#SubFlow").length == 1 && frmData.WF_Node != undefined) {
        Skip.addJs(ccbpmPath + "/WF/WorkOpt/SubFlow.js");
        var html = SubFlow_Init(frmData.WF_Node[0]);
        $("#SubFlow").html(html);
    }
}

/**
 * 从表解析
 * @param {any} element
 * @param {any} frmDtl
 * @param {any} ext
 */
function figure_Develop_Dtl(element, frmDtl) {
    var urlParam = location.href.substring(location.href.indexOf('?') + 1, location.href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');

    //在Image元素下引入IFrame文件
    var src = "";
    var baseUrl = "./CCForm/";
    if (currentURL.indexOf("AdminFrm.htm") != -1)
        baseUrl = "../../CCForm/";
    if (currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
        baseUrl = "../CCForm/";
    if (currentURL.indexOf("CCForm/") != -1)
        baseUrl = "./";
    //表格模式
    if (frmDtl.ListShowModel == "0")
        src = baseUrl + "Dtl2017.htm?1=1";
    if (frmDtl.ListShowModel == "1")
        src = baseUrl + "DtlCard.htm?1=1";
    if (frmDtl.ListShowModel == "2") {
        if (frmDtl.UrlDtl == null || frmDtl.UrlDtl == undefined || frmDtl.UrlDtl == "")
            return "从表" + frmDtl.Name + "没有设置URL,请在" + frmDtl.FK_MapData + "_Self.js中解析";
        src = basePath + "/" + frmDtl.UrlDtl;
        if (src.indexOf("?") == -1)
            src += "?1=1";
    }
    var isRead = isReadonly == true ? 1 : 0
    src += "&EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=" + isRead + "&" + urlParam + "&Version=1&FrmType=0";

    var eleHtml = $("<div id='Dtl_" + frmDtl.No + "' name='Dtl' style='height:auto;margin:5px 10px;border-top:1px solid #D0D0D0' ></div>");

    var eleIframe = $("<iframe style='width:100%;height:100%' name='Dtl' ID='Frame_" + frmDtl.No + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>");
    eleHtml.append(eleIframe);
    $(element).after(eleHtml);
    $(element).remove(); //移除Imge节点
}


//图片附件
function figure_Develop_ImageAth(element, frmImageAth, fk_mapData) {
    var isEdit = frmImageAth.IsEdit;
    var img = $("<img class='pimg' onclick='imgShow(this)'/>");

    var imgSrc = basePath + "/WF/Data/Img/LogH.PNG";

    //获取数据
    if (fk_mapData.indexOf("ND") != -1)
        imgSrc = basePath + "/DataUser/ImgAth/Data/" + frmImageAth.CtrlID + "_" + pageData.WorkID + ".png";
    else
        imgSrc = basePath + "/DataUser/ImgAth/Data/" + fk_mapData + "_" + frmImageAth.CtrlID + "_" + pageData.WorkID + ".png";

    //设计属性
    img.attr('id', 'Img' + frmImageAth.MyPK).attr('name', 'Img' + frmImageAth.MyPK);
    img.attr("src", imgSrc).attr('onerror', "this.src='" + basePath + "/WF/Admin/CCFormDesigner/Controls/DataView/AthImg.png'");
    img.css('width', element.width()).css('height', element.height()).css('padding', "0px").css('margin', "0px").css('border-width', "0px");

    //可编辑
    if (isEdit == "1" && isReadonly == false) {
        var url = dynamicHandler + "?DoType=HttpHandler&DoMethod=FrmImgAthDB_Upload&HttpHandlerName=BP.WF.HttpHandler.WF_CCForm&FK_MapData=" + fk_mapData + "&CtrlID=" + frmImageAth.CtrlID + "&RefPKVal=" + pageData.WorkID
        var fieldSet = $("<fieldset style='display:inline'></fieldset>");
        var length = $("<legend><div class='layui-btn layui-btn-primary' id ='editimg' data-ref='" + frmImageAth.MyPK + "' data-info='" + url + "'>修改图片</div></legend>");
        fieldSet.append(length);
        fieldSet.append(img);
        $(element).after(fieldSet);

    } else {
        $(element).after(img);
    }
    $(element).remove(); //移除Imge节点
}

//初始化 IMAGE  只初始化了图片类型
function figure_Develop_Image(element, frmImage) {
    //解析图片
    if (frmImage.ImgAppType == 0) { //图片类型
        //数据来源为本地.
        var imgSrc = '';
        if (frmImage.ImgSrcType == 0) {
            //替换参数
            var frmPath = frmImage.ImgPath;
            frmPath = frmPath.replace('＠', '@');
            frmPath = frmPath.replace('@basePath', basePath);
            frmPath = frmPath.replace('@basePath', basePath);
            imgSrc = DealJsonExp(frmData.MainTable[0], frmPath);
        }

        //数据来源为指定路径.
        if (frmImage.ImgSrcType == 1) {
            var url = frmImage.ImgURL;
            url = url.replace('＠', '@');
            url = url.replace('@basePath', basePath);
            imgSrc = DealJsonExp(frmData.MainTable[0], url);
        }
        var errorImg = "../DataUser/ICON/CCFlow/LogBig.png";
        if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
            errorImg = "../../DataUser/ICON/CCFlow/LogBig.png";
        else if (currentURL.indexOf("AdminFrm.htm") != -1)
            errorImg = "../../../DataUser/ICON/CCFlow/LogBig.png";
        // 由于火狐 不支持onerror 所以 判断图片是否存在放到服务器端
        if (imgSrc == "" || imgSrc == null)
            imgSrc = errorImg;

        var a = $("<a></a>");
        var img = $("<img/>")

        img.attr("src", imgSrc).css('width', frmImage.W).css('height', frmImage.H).attr('onerror', "this.src='" + errorImg + "'");

        if (frmImage.LinkURL != undefined && frmImage.LinkURL != '') {
            a.attr('href', frmImage.LinkTarget).attr('target', frmImage.LinkTarget).css('width', frmImage.W).css('height', frmImage.H);
            a.append(img);
            $(element).after(a);
        } else {
            $(element).after(img);
        }

        $(element).remove(); //移除Imge节点
    } else if (frmImage.ImgAppType == 3)//二维码  手机
    {


    } else if (frmImage.ImgAppType == 1) {//暂不解析
        //电子签章  写后台
    }
}

function figure_Develop_Btn(frmBtn) {
    var element;
    if ($("#TB_" + frmBtn.BtnID).length == 0)
        return;
    element = $("#TB_" + frmBtn.BtnID);

    var doc = frmBtn.EventContext;
    doc = doc.replace("~", "'");
    var eventType = frmBtn.EventType;
    if (eventType == 0) {//禁用
        element.attr('disabled', 'disabled').css('background', 'gray');
    } else if (eventType == 1) {//运行URL
        $.each(frmData.Sys_MapAttr, function (i, obj) {
            if (doc.indexOf('@' + obj.KeyOfEn) > 0) {
                doc = doc.replace('@' + obj.KeyOfEn, frmData.MainTable[0][obj.KeyOfEn]);
            }
        });
        var OID = GetQueryString("OID");
        if (OID == undefined || OID == "");
        OID = GetQueryString("OID");
        var FK_Node = GetQueryString("FK_Node");
        var FK_Flow = GetQueryString("FK_Flow");
        var webUser = new WebUser();
        var userNo = webUser.No;
        var SID = webUser.Token;
        if (SID == undefined)
            SID = "";
        if (doc.indexOf("?") == -1)
            doc = doc + "?1=1";
        doc = doc + "&OID=" + pageData.WorkID + "&FK_Node=" + FK_Node + "&FK_Flow=" + FK_Flow + "&UserNo=" + userNo + "&Token=" + SID;
        element.attr('onclick', "window.open('" + doc + "')");

    } else {//运行JS
        if (doc.indexOf("(") == -1)
            doc = doc + "()";
        element.attr('onclick', doc);
    }
}




//初始化框架
function figure_Develop_IFrame(element, frame) {

    //获取框架的类型 0 自定义URL 1 地图开发 2流程轨迹表 3流程轨迹图
    var urlType = frame.UrlSrcType;
    var url = "";
    if (urlType == 0) {
        url = frame.URL;
        if (url.indexOf('?') == -1)
            url += "?1=2";

        if (url.indexOf("@basePath") == 0)
            url = url.replace("@basePath", basePath);

        //1.处理URL需要的参数
        var pageParams = getQueryString();
        $.each(pageParams, function (i, pageParam) {
            var pageParamArr = pageParam.split('=');
            url = url.replace("@" + pageParamArr[0], pageParamArr[1]);
        });

        var src = url.replace(new RegExp(/(：)/g), ':');
        if (src.indexOf("?") > 0) {
            var params = getQueryStringFromUrl(src);
            if (params != null && params.length > 0) {
                $.each(params, function (i, param) {
                    if (param.indexOf('@') != -1) {//是需要替换的参数
                        paramArr = param.split('=');
                        if (paramArr.length == 2 && paramArr[1].indexOf('@') == 0) {
                            if (paramArr[1].indexOf('@WebUser.') == 0)
                                url = url.replace(paramArr[1], frmData.MainTable[0][paramArr[1].substr('@WebUser.'.length)]);
                            else
                                url = url.replace(paramArr[1], frmData.MainTable[0][paramArr[1].substr(1)]);
                        }
                    }
                });
            }
        }


        //1.拼接参数
        var paras = this.pageData;
        var strs = "";
        for (var str in paras) {
            if (str == "EnsName" || str == "RefPKVal" || str == "IsReadonly")
                continue
            else
                strs += "&" + str + "=" + paras[str];
        }

        url = url + strs + "&IsReadonly=0";

        //4.追加GenerWorkFlow AtPara中的参数
        var gwf = frmData.WF_GenerWorkFlow[0];
        if (gwf != null) {
            var atPara = gwf.AtPara;
            if (atPara != null && atPara != "") {
                atPara = atPara.replace(/@/g, '&');
                url = url + atPara;
            }
        }
    }
    if (urlType == 2 || urlType == 3) {
        if (urlType == 2) { //轨迹表
            if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
                url = "../WorkOpt/OneWork/Table.htm";
            else
                url = "./WorkOpt/OneWork/Table.htm";
        }
        if (urlType == 3) {//轨迹图
            if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
                url = "../WorkOpt/OneWork/TimeBase.htm";
            else if (currentURL.indexOf("AdminFrm.htm") != -1)
                url = "../../WorkOpt/OneWork/TimeBase.htm";
            else
                url = "./WorkOpt/OneWork/TimeBase.htm";
        }
        url = url + "?FK_Node=" + pageData.FK_Node + "&FK_Flow=" + pageData.FK_Flow + "&WorkID=" + pageData.OID + "&FID=" + pageData.FID;

    }

    var eleHtml = $("<div id='Frame" + frame.MyPK + "' style='width:" + frame.W + "px; height:auto;' ></div>");

    var eleIframe = $("<iframe class= 'Fdtl' ID = 'Frame_" + frame.MyPK + "' src = '" + url + "' frameborder=0  style='width:" + frame.W + "px;"
        + "height: auto; text-align: left; '  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");


    eleHtml.append(eleIframe);
    $(element).after(eleHtml);
    $(element).remove(); //移除Frame节点

}


//子流程
function figure_Develop_FigureSubFlowDtl(wf_node, element) {

    Skip.addJs(ccbpmPath + "/WF/WorkOpt/SubFlow.js");
    var html = SubFlow_Init(wf_node);

    var eleHtml = $("<div id='SubFlow' style='width:100% height:auto;' >" + html + "</div>");

    $(element).after(eleHtml);
    $(element).remove(); //移除SubFlow节点
}


//审核组件
function figure_Develop_FigureFrmCheck(wf_node, element, frmData) {

    var currentURL = GetHrefUrl();
    //这个修改数据的位置
    if (currentURL != undefined && currentURL.indexOf("AdminFrm.htm") != -1) {
        $(element).remove();
        return;
    }

    var sta = wf_node.FWCSta;
    if (sta == 0 || sta == undefined) {
        $(element).remove();  //移除审核组件图片
        return $('还未开始审核');
    }


    var node = frmData.WF_Node;
    if (node != undefined)
        node = node[0];

    var frmNode = frmData.WF_FrmNode;
    if (frmNode != undefined)
        frmNode = frmNode[0];

    if (node == null) {
        $(element).remove();
        return $('');
    }

    if (frmNode != null && node.FormType == 5 && frmNode.IsEnableFWC == 0) {
        $(element).remove();
        return $('');
    }




    var url = "./WorkOpt/";
    if (currentURL.indexOf("FrmGener.htm") != -1 || currentURL.indexOf("MyBill.htm") != -1 || currentURL.indexOf("MyDict.htm") != -1)
        url = '../WorkOpt/';
    else if (currentURL.indexOf("AdminFrm.htm") != -1)
        url = '../../WorkOpt/';
    if (wf_node.FWCSta != 0) {
        if (wf_node.FWCVer == 0 || wf_node.FWCVer == "" || wf_node.FWCVer == undefined)
            pageData.FWCVer = 0;
        else
            pageData.FWCVer = 1;
    }

    var eleHtml = $("<div id='WorkCheck'></div>");

    $(element).after(eleHtml);
    $(element).remove(); //移除SubFlow节点

}

var isSigantureChecked = false;
var isStampChecked = false;
function ChangeSigantureState(keyOfEn, sigantureType) {
    if (typeof ChangeSigantureStateSelf == "function") {
        ChangeSigantureStateSelf(keyOfEn, sigantureType);
        return;
    }
    if (sigantureType == 1) {
        if (isSigantureChecked == false) {
            isSigantureChecked = true;
            //修改按钮名称
            $('#Btn_' + keyOfEn).val("取消签名");
            $('#TB_' + keyOfEn).val(webUser.No);
            var _html = "<img src='" + UserICon + webUser.No + UserIConExt + "' alt='" + webUser.No + "'   style='border:0px;width:105px;height:45px;padding-left:20px;position: absolute;mix-blend-mode:normal;' id='Img_" + keyOfEn + "' />";
            if ($("#stamp").length == 0)
                $('#Btn_' + keyOfEn).after("<div id='stamp'style='display:inline'>" + _html + "</div>");
            else
                $("#stamp").prepend(_html);
            return;
        }
        if (isSigantureChecked == true) {
            isSigantureChecked = false;
            //修改按钮名称
            $('#Btn_' + keyOfEn).val("点击签名");
            $('#TB_' + keyOfEn).val("");
            $('#Img_' + keyOfEn).remove();
            return;
        }
    }
    if (sigantureType == 4) {
        if (isStampChecked == false) {
            isStampChecked = true;
            //修改按钮名称
            $('#Btn_' + keyOfEn).val("取消盖章");
            $('#TB_' + keyOfEn).val(webUser.FK_Dept);
            var _html = " <img src= '" + UserICon + webUser.FK_Dept + UserIConExt + "' alt= '" + webUser.FK_Dept + "'   style='border:0px;width:175px;height:50px;' id = 'Img_" + keyOfEn + "'/>";
            if ($("#stamp").length == 0)
                $('#Btn_' + keyOfEn).after("<div id='stamp'style='display:inline'>" + _html + "</div>");
            else
                $("#stamp").append(_html);
            //$('#Btn_' + keyOfEn).after();
            return;
        }
        if (isStampChecked == true) {
            isStampChecked = false;
            //修改按钮名称
            $('#Btn_' + keyOfEn).val("点击盖章");
            $('#TB_' + keyOfEn).val("");
            $('#Img_' + keyOfEn).remove();
            return;
        }
    }
}

//双击签名
function figure_Develop_Siganture(SigantureID, val, type) {
    //先判断，是否存在签名图片
    var handler = new HttpHandler("BP.WF.HttpHandler.WF");
    handler.AddPara('no', val);
    data = handler.DoMethodReturnString("HasSealPic");

    //如果不存在，就显示当前人的姓名
    if (data.length > 0 && type == 0) {
        $("#TB_" + SigantureID).before(data);
        var obj = document.getElementById("Img" + SigantureID);
        var impParent = obj.parentNode; //获取img的父对象
        impParent.removeChild(obj);
    }
    else {
        var src = UserICon + val + UserIConExt;    //新图片地址

        document.getElementById("Img" + SigantureID).src = src;
    }
    isSigantureChecked = true;


}


//地图
function figure_Develop_Map(MapID, UIIsEnable) {
    var mainTable = frmData.MainTable[0];
    var AtPara = "";
    //通过MAINTABLE返回的参数
    for (var ele in mainTable) {
        if (ele == "AtPara" && mainTable != '') {
            AtPara = mainTable[ele];
            break;
        }
    }

    var url = basePath + "/WF/CCForm/Map.htm?WorkID=" + pageData.WorkID + "&FK_Node=" + pageData.FK_Node + "&KeyOfEn=" + MapID + "&UIIsEnable=" + UIIsEnable + "&Paras=" + AtPara;
    OpenBootStrapModal(url, "eudlgframe", "地图", 800, 500, null, false, function () { }, null, function () {

    });
}


