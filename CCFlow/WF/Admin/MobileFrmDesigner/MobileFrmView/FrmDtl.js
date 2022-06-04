var Form_ReadOnly = false;
var mainData;
var gfs; //明细表分组
var dtlSize = 0; //明细表条数
var sys_mapAttr;
//加载明细表数据
function Load_DtlInit() {
    var dtl_No = $("#HD_CurDtl_No").val();
    $("#DtlContent").empty();
    var args = new RequestArgs();

    //获得mapdtl实体的基本信息.
    var hand = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    hand.AddPara("EnsName", dtl_No);
    hand.AddPara("RefPKVal", args.WorkID);
    hand.AddPara("FK_Node", args.FK_Node);
    hand.AddPara("IsReadonly", args.IsReadonly);
    mainData = hand.DoMethodReturnJSON("Dtl_Init");

    //获取正真含有的分组
    //var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_MyFlow");
    //handler.AddPara("FK_Node", args.FK_Node);
    //handler.AddPara("FK_MapData", dtl_No);
    //var gfs = handler.DoMethodReturnJSON("GetGroupFields");

    gfs = new Entities("BP.Sys.GroupFields");
    gfs.Retrieve("FrmID", dtl_No);


    //主表数据，用于变量替换.
    var mainTable = mainData["MainTable"]; //主表数据.
    //从表信息.
    sys_MapDtl = mainData["Sys_MapDtl"][0]; //从表描述.
    sys_mapAttr = mainData["Sys_MapAttr"]; //从表字段.
    var sys_mapExtDtl = mainData["Sys_MapExt"]; //扩展信息.
    var dbDtl = mainData["DBDtl"]; //从表数据.
    var mapDtls = mainData["MapDtls"]; //从表的从表集合.

    dtlSize = dbDtl.length;

    var _Html = "";
    //判断是否有数据
    if(dbDtl.length == 0){
        _Html = "<div class='mui-indexed-list-inner empty'>";
        _Html += " <div class='mui-indexed-list-empty-alert'>没有数据</div>";
        _Html += "</div>";
    }
    if (Form_ReadOnly == true || (sys_MapDtl.IsInsert == "0" && sys_MapDtl.IsUpdate == "0")) {
        Form_ReadOnly = true;
    }


    //加载表单元素\数据
    var dtl_Idx = 1;
    for (var j = 0; j < dbDtl.length; j++) {
        _Html += "<ul class='mui-table-view'>";
        _Html += "  <li class='mui-table-view-divider'>序号:" + dtl_Idx;
        if (sys_MapDtl.IsDelete == "1") {
            _Html += "   <div class='dtl_deleterow' id='" + dbDtl[j].OID + "'>删除</div>";
        }
        _Html += "	</li>";
        //循环分组
        for (var g = 0; g < gfs.length; g++) {
            //收个分组标签不进行添加
            if (g != 0) {
                _Html += "<div class=\"mui-table-view-divider\"><h5 style='color:black;'>" + gfs[g].Lab + "</h5></div>";
            }
            //明细表的控件.
            if (gfs[g].CtrlType == 'Dtl') {
                if (mapDtls) {
                    $.each(mapDtls, function (i, dtl) {
                        if (gfs[g].CtrlID == dtl.No) {
                            var func = "DtlChild_ShowPage(\"" + dtl.No + "\",\"" + dtl.Name + "\",\"" + dbDtl[j].OID + "\")";
                            _Html += "<div class='mui-table-view-cell'>";
                            _Html += "	<a class='mui-navigate-right' data-title-type='native' href='javascript:" + func + "'><h5>" + dtl.Name
                            + "<span id='" + dtl.No + "_Count'></span></h5>";
                            _Html += "		<p>点击查看详细</p>";
                            _Html += "	</a>";
                            _Html += "</div>";
                        }
                    });
                }
                continue;
            }
            //字段生成
            for (var k = 0; k < sys_mapAttr.length; k++) {
//                if (sys_mapAttr[k].GroupID != gfs[g].OID)
//                    continue;
                if (sys_mapAttr[k].UIVisible == "0")
                    continue;
                if (sys_mapAttr[k].KeyOfEn == "OID")
                    continue;

                var transControl = new TranseDtlControl(sys_mapAttr[k], dbDtl[j]);
                _Html += transControl.To_Html();
            }
        }
        _Html += "</ul>";
        dtl_Idx++;
    }

    //启用新增按钮
    if (Form_ReadOnly == false && sys_MapDtl.IsInsert == "1") {
        _Html += "<ul class='mui-table-view' id='AddInfo'>";
        _Html += "<li class='mui-table-view-cell'>";
        _Html += " <div class='dtl_addpanel'>";
        _Html += "    <a><b class='dtl_addpanel_b'>+</b> 增加" + sys_MapDtl.Name + "明细</a>";
        _Html += " </div>";
        _Html += "</li>";
        _Html += "</ul>";
    }
    //添加保存按钮
    if (Form_ReadOnly == false && dbDtl.length > 0 && (sys_MapDtl.IsInsert == "1" || sys_MapDtl.IsUpdate == "1")) {
        $("#dtlDone").html("提交");
        $("#dtlDone").off("tap").on("tap", function () {
            Dtl_SaveData();
            // function (data) {
                // var pushData = cceval('(' + data + ')');
                // if (pushData.Msg) {
                //     mui.toast(pushData.Msg);
                //     return;
                // }
                viewApi.back();
                mui.toast("保存成功！");
                Load_DtlInit();
            // });
        });
    }

    //更新明细表记录数量
    //$("#" + dtl_No + "_Count").html("(" + dbDtl.length + ")条记录");

    //生成页面
    $(_Html).appendTo('#DtlContent');

    //解析扩展设置,MapExt
    for (var y = 0; y < dbDtl.length; y++) {
        AfterBindDtl_DealMapExt(mainData, dbDtl[y].OID);
    }

    if (Form_ReadOnly == false && dbDtl.length > 0 && (sys_MapDtl.IsInsert == "1" || sys_MapDtl.IsUpdate == "1")) {
        //日期控件
        mui(".mui-input-row").off("tap").on("tap", ".ccformdate", function () {
            var dDate = new Date();
            var optionsJson = this.getAttribute('data-options') || '{}';
            var ctrID = this.getAttribute('id');
            var options = JSON.parse(optionsJson);
            var picker = new mui.DtPicker(options);
            picker.show(function (rs) {
                var timestr = rs.text;
                $("#" + ctrID).html(timestr);
                $("#TB_" + ctrID.substr(4)).val(timestr);
                picker.dispose();
            });
        });
    }
    //添加行事件
    $(".dtl_addpanel a").on("click", function () {
        //先保存后新增行
        Dtl_SaveData();
        Dtl_InsertRow();
        Load_DtlInit();

    });
    //删除事件
    $(".dtl_deleterow").on("click", function () {
        var target = $(this);
        var oid = target.attr("id");
        //先保存后删除行
        Dtl_SaveData();
        Dtl_DeleteByOID(oid);

    });
}

//保存数据
function Dtl_SaveData(CallBack) {
    var urlExt = urlExtFrm();
    var args = new RequestArgs();
    var dtl_No = $("#HD_CurDtl_No").val();

    var url = GetHrefUrl();
    if (url.indexOf('/jflow-web/') >= 0) {
        var index = url.indexOf('/jflow-web');
        url = url.substring(index);
    }
    var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_CCForm");
    handler.AddPara("EnsName", dtl_No);
    handler.AddPara("RefPKVal", args.WorkID);
    handler.AddFormData();
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("Dtl_SaveRow");
    if (data.indexOf("err@") == 0) {
        mui.toast(data);
        return;
    }

}

//添加行
function Dtl_InsertRow() {
    var dtl_No = $("#HD_CurDtl_No").val();
    var args = new RequestArgs();
    var dtl = new Entity(dtl_No);
    dtl.RefPK = args.WorkID;
    dtl.FID = args.FID;
    dtl = dtl.Insert();
    Load_DtlForm(JSON.parse(dtl));
}

function Load_DtlForm(dbDtl) {
    var _Html = "";
    var index = parseInt(dtlSize)+1;
    _Html += "<ul class='mui-table-view'>";
    _Html += "  <li class='mui-table-view-divider'>序号:" + index;
    _Html += "	</li>";
    //循环分组
    for (var g = 0; g < gfs.length; g++) {
        //收个分组标签不进行添加
        if (g != 0) {
            _Html += "<div class=\"mui-table-view-divider\"><h5 style='color:black;'>" + gfs[g].Lab + "</h5></div>";
        }
        //明细表的控件.
        if (gfs[g].CtrlType == 'Dtl') {

            continue;
        }
        //字段生成
        for (var k = 0; k < sys_mapAttr.length; k++) {
//            if (sys_mapAttr[k].GroupID != gfs[g].OID)
//                continue;
            if (sys_mapAttr[k].UIVisible == "0")
                continue;
            if (sys_mapAttr[k].KeyOfEn == "OID")
                continue;

            var transControl = new TranseDtlControl(sys_mapAttr[k],dbDtl);
            _Html += transControl.To_Html();
        }
    }
    _Html += "</ul>";
    $(_Html).appendTo('#DtlContent');
}

//删除记录通过主键OID
function Dtl_DeleteByOID(oid) {
    var btnArray = ['否', '是'];
    mui.confirm('确定要删除所选记录吗？', '提示', btnArray, function (e) {
        if (e.index == 1) {
            var dtl_No = $("#HD_CurDtl_No").val();
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
            handler.AddPara("FK_MapDtl", dtl_No);
            handler.AddPara("OID", oid);
            handler.DoMethodReturnString("Dtl_DeleteRow");
            Load_DtlInit();
        }
    });
}

//打开明细表的明细表
function DtlChild_ShowPage(dtlNo, dtlName, OID) {
    $("#HD_CurChildDtl_No").val(dtlNo);
    $("#HD_CurDtl_OID").val(OID);
    $("#frmChildDtlTitle").html(dtlName);
    Load_ChildDtlForm();
    viewApi.go("#frmChildDtl");
}

//加载明细表的字表数据
function Load_ChildDtlForm() {
    var cdtl_No = $("#HD_CurChildDtl_No").val();
    $("#ChildDtlContent").empty();
    var OID = $("#HD_CurDtl_OID").val();
    var args = new RequestArgs();

    //获得mapdtl实体的基本信息.
    var hand = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    hand.AddPara("EnsName", cdtl_No);
    hand.AddPara("RefPKVal", OID);
    hand.AddPara("FK_Node", args.FK_Node);
    hand.AddPara("IsReadonly", args.IsReadonly);
    mainData = hand.DoMethodReturnJSON("Dtl_Init");

    //获取正真含有的分组
    var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_MyFlow");
    handler.AddPara("FK_Node", args.FK_Node);
    handler.AddPara("FK_MapData", cdtl_No);
    var gfs = handler.DoMethodReturnJSON("GetGroupFields");


    //主表数据，用于变量替换.
    var mainTable = mainData["MainTable"]; //主表数据.
    //从表信息.
    sys_MapDtl = mainData["Sys_MapDtl"][0]; //从表描述.
    var sys_mapAttr = mainData["Sys_MapAttr"]; //从表字段.
    var sys_mapExtDtl = mainData["Sys_MapExt"]; //扩展信息.
    var dbDtl = mainData["DBDtl"]; //从表数据.
    //var mapDtls = mainData["MapDtls"]; //从表的从表集合.

    var _Html = "";
    //判断是否有数据
    if (dbDtl.length == 0) {
        if (Form_ReadOnly == false && sys_MapDtl.Insert == "1") {
            Dtl_InsertRow();
            return;
        }
        _Html = "<div class='mui-indexed-list-inner empty'>";
        _Html += " <div class='mui-indexed-list-empty-alert'>没有数据</div>";
        _Html += "</div>";
    } else if (Form_ReadOnly == true || (sys_MapDtl.IsInsert == "0" && sys_MapDtl.IsUpdate == "0")) {
        //只读
        Form_ReadOnly = true;
    }

    //加载表单元素\数据
    var dtl_Idx = 1;
    for (var j = 0; j < dbDtl.length; j++) {
        _Html += "<ul class='mui-table-view'>";
        _Html += "  <li class='mui-table-view-divider'>序号:" + dtl_Idx;
        if (sys_MapDtl.IsDelete == "1") {
            _Html += "   <div class='dtlchild_deleterow' id='" + dbDtl[j].OID + "'>删除</div>";
        }
        _Html += "	</li>";
        //循环分组
        for (var g = 0; g < gfs.length; g++) {
            //收个分组标签不进行添加
            if (g != 0) {
                _Html += "<div class=\"mui-table-view-divider\"><h5 style='color:black;'>" + gfs[g].Lab + "</h5></div>";
            }
            //明细表的控件.
            if (gfs[g].CtrlType == 'Dtl') {
                if (mapDtls) {
                    $.each(mapDtls, function (i, dtl) {
                        if (gfs[g].CtrlID == dtl.No) {
                            var func = "DtlChild_ShowPage(\"" + dtl.No + "\",\"" + dtl.Name + "\")";
                            _Html += "<div class='mui-table-view-cell'>";
                            _Html += "	<a class='mui-navigate-right' data-title-type='native' href='javascript:" + func + "'><h5>" + dtl.Name
                            + "<span id='" + dtl.No + "_Count'></span></h5>";
                            _Html += "		<p>点击查看详细</p>";
                            _Html += "	</a>";
                            _Html += "</div>";
                        }
                    });
                }
                continue;
            }
            //字段生成
            for (var k = 0; k < sys_mapAttr.length; k++) {
                if (sys_mapAttr[k].GroupID != gfs[g].OID)
                    continue;
                if (sys_mapAttr[k].UIVisible == "0")
                    continue;
                if (sys_mapAttr[k].KeyOfEn == "OID")
                    continue;
                var transControl = new TranseDtlControl(sys_mapAttr[k],null);
                _Html += transControl.To_Html();
            }
        }
        _Html += "</ul>";

    }



    //生成页面
    $("#AddInfo").before(_Html);

    if (Form_ReadOnly == false && dbDtl.length > 0 && (sys_MapDtl.IsInsert == "1" || sys_MapDtl.IsUpdate == "1")) {
        //日期控件
        mui(".mui-input-row").off("tap").on("tap", ".ccformdate", function () {
            var dDate = new Date();
            var optionsJson = this.getAttribute('data-options') || '{}';
            var ctrID = this.getAttribute('id');
            var options = JSON.parse(optionsJson);
            var picker = new mui.DtPicker(options);
            picker.show(function (rs) {
                var timestr = rs.text;
                $("#" + ctrID).html(timestr);
                $("#TB_" + ctrID.substr(4)).val(timestr);
                picker.dispose();
            });
        });
    }

}

//保存子表数据
function DtlChild_SaveData(CallBack) {
    var urlExt = urlExtFrm();
    var args = new RequestArgs();
    var dtl_No = $("#HD_CurChildDtl_No").val();
    var revpk = $("#HD_CurDtl_OID").val();

    var url = GetHrefUrl();
    if (url.indexOf('/jflow-web/') >= 0) {
        var index = url.indexOf('/jflow-web');
        url = url.substring(index);
    }
    var ccmobile = url.substring(0, url.lastIndexOf('/') + 1) + "CCForm/ProcessRequest.do";

    $("#form_ChildDtl").ajaxSubmit({
        type: 'post',
        url: ccmobile + "?DoType=SaveDtl&EnsName=" + dtl_No + "&RefPKVal=" + revpk,
        success: function (data) {
            CallBack(data);
        },
        error: function (XmlHttpRequest, textStatus, errorThrown) {
            mui.toast("保存失败，请检查表单！");
        }
    });
}

//子表添加行
function DtlChild_InsertRow() {
    var dtl_No = $("#HD_CurChildDtl_No").val();
    var revpk = $("#HD_CurDtl_OID").val();
    var dtl = new Entity(dtl_No);
    dtl.RefPK = revpk;
    dtl.FID = 0;
    dtl = dtl.Insert();
    Load_ChildDtlForm();
}

//删除记录通过主键OID
function DtlChild_DeleteByOID(oid) {
    var btnArray = ['否', '是'];
    mui.confirm('确定要删除所选记录吗？', '提示', btnArray, function (e) {
        if (e.index == 1) {
            var dtl_No = $("#HD_CurChildDtl_No").val();
            var dtl = new Entity(dtl_No);
            dtl.OID = oid;
            dtl.SetPKVal(oid);
            var dtl = dtl.Delete();
            Load_ChildDtlForm();
        }
    });
}

//转控件
function TranseDtlControl(dtlColumn, DataRow) {
    this.control = dtlColumn;
    this.DataRow = DataRow;
    this.Ctrl_Class = "";
    //控件是否可用
    this.Enable = true;
}
//控件属性
TranseDtlControl.prototype = {
    To_Html: function () {
        var _html = "";
        this.Ctrl_Class = "";
        this.Enable = true;
        //判断控件是否可用
        if (this.control.UIIsEnable == "0" || Form_ReadOnly == true) {
            this.Enable = false;
            this.Ctrl_Class = "disabled = \"disabled\" ";
        }
        _html = "<li class='mui-input-row'>";
        //标签
        _html += "	<label><p>" + this.control.Name + "</P></label>";
        //图片签名
        if (this.control.IsSigan == "1") {
            _html += this.CreateSignPicture();
            _html += "</li>"
            return _html;
        }
        var Ctrl_Val = "";
        if (this.DataRow != null)
            Ctrl_Val = this.DataRow[this.control.KeyOfEn];

        if (Ctrl_Val == undefined)
            Ctrl_Val = "";

        //加载其他数据控件
        switch (this.control.LGType) {
            case FieldTypeS.Normal: //输出普通类型字段
                if (this.control.UIContralType == UIContralType.DDL) {
                    //判断外部数据或WS类型.
                    if (this.Enable == false) {
                        this.Ctrl_Class = "disabled = \"disabled\" ";
                    }
                    _html += this.CreateDDLPK(Ctrl_Val);
                    break;
                }
                switch (this.control.MyDataType) {
                    case FormDataType.AppString:
                        _html += this.CreateTBString(Ctrl_Val);
                        break;
                    case FormDataType.AppInt:
                        _html += this.CreateTBInt(Ctrl_Val);
                        break;
                    case FormDataType.AppFloat:
                    case FormDataType.AppDouble:
                    case FormDataType.AppMoney:
                        _html += this.CreateTBFloat(Ctrl_Val);
                        break;
                    case FormDataType.AppDate:
                        if (this.Enable == false) {
                            this.Ctrl_Class = "readonly = \"readonly\" ";
                        }
                        _html += this.CreateTBDate(Ctrl_Val);
                        break;
                    case FormDataType.AppDateTime:
                        if (this.Enable == false) {
                            this.Ctrl_Class = "readonly = \"readonly\" ";
                        }
                        _html += this.CreateTBDateTime(Ctrl_Val);
                        break;
                    case FormDataType.AppBoolean:
                        if (this.Enable == false) {
                            this.Ctrl_Class = "readonly = \"readonly\" ";
                        }
                        _html += this.CreateCBBoolean(Ctrl_Val);
                        break;
                }
                break;
            case FieldTypeS.Enum: //枚举值下拉框
                if (this.Enable == false) {
                    this.Ctrl_Class = "disabled = \"disabled\" ";
                }
                _html += this.CreateDDLEnum(Ctrl_Val);
                break;
            case FieldTypeS.FK: //外键表下拉框    
                if (this.Enable == false) {
                    this.Ctrl_Class = "disabled = \"disabled\" ";
                }
                _html += this.CreateDDLPK(Ctrl_Val);
                break;
        }
        _html += "</li>"
        return _html;
    },
    CreateSignPicture: function (Ctrl_Val) {
        //图片签名
        var sign_id = this.control.KeyOfEn;
        var val = this.DataRow[sign_id];
        var errorVal = this.DataRow["ImgErrorValue"];
        return "<img name=\"Sign_Dtl_" + sign_id + "\" id=\"Sign_Dtl_" + sign_id + "\" src='" + val + "' border=0 onerror=\"this.src='" + errorVal + "'\"/>";
    },
    CreateTBString: function (Ctrl_Val) {
        var Ctrl_Id = "TB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        //var Ctrl_Val = this.DataRow[this.control.KeyOfEn];
        //多行文本
        if (this.control.UIHeight > 40) {
            return "<textarea " + this.Ctrl_Class + " cols=\"40\" rows=\"8\" name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\">" + Ctrl_Val + "</textarea>";
        }
        //单行文本
        return "<input " + this.Ctrl_Class + " type=\"text\" name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" value=\"" + Ctrl_Val + "\" />";
    },
    CreateTBInt: function (Ctrl_Val) {
        var Ctrl_Id = "TB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        // var Ctrl_Val = this.DataRow[this.control.KeyOfEn];
        var inputHtml = "<input " + this.Ctrl_Class + " type=\"number\" pattern=\"[0 - 9] * \"";
        inputHtml += " name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" placeholder=\"0\" value=\"" + Ctrl_Val + "\" />";
        return inputHtml;
    },
    CreateTBFloat: function (Ctrl_Val) {
        var Ctrl_Id = "TB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        // var Ctrl_Val = this.DataRow[this.control.KeyOfEn];
        return "<input " + this.Ctrl_Class + " type=\"number\" name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" placeholder=\"0.00\" value=\"" + Ctrl_Val + "\" />";
    },
    CreateTBDate: function (Ctrl_Val) {
        var Ctrl_Id = "TB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        var LAB_Id = "LAB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        //var Ctrl_Val = this.DataRow[this.control.KeyOfEn];
        var Ctrl_Text = Ctrl_Val;
        if (this.Enable == false) {
            return "<input " + this.Ctrl_Class + " type=\"text\" data-role=\"datebox\" name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" value=\"" + Ctrl_Val + "\" />";
        }
        var inputHtml = "";
        if (Ctrl_Val == "") {
            Ctrl_Text = "<p>请选择日期</p>";
        }
        inputHtml += "<a class='mui-navigate-right'>";
        inputHtml += "  <span name=\"" + LAB_Id + "\" id=\"" + LAB_Id + "\" data-options='{\"type\":\"date\"}' class='mui-pull-right ccformdate' style='margin-right:30px;margin-top:10px;min-width:160px;text-align:right;'>" + Ctrl_Text + "</span>";
        inputHtml += "</a>";
        inputHtml += "<input  type='hidden' name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" value='" + Ctrl_Val + "' />";
        return inputHtml;
    },
    CreateTBDateTime: function (Ctrl_Val) {
        var Ctrl_Id = "TB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        var LAB_Id = "LAB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        //var Ctrl_Val = this.DataRow[this.control.KeyOfEn];
        var Ctrl_Text = Ctrl_Val;
        if (this.Enable == false) {
            return "<input " + this.Ctrl_Class + " type=\"text\" data-role=\"datetimebox\" name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" value=\"" + Ctrl_Val + "\" />";
        }
        var inputHtml = "";
        if (Ctrl_Val == "") {
            Ctrl_Text = "<p>请选择时间</p>";
        }
        inputHtml += "<a class='mui-navigate-right'>";
        inputHtml += "  <span name=\"" + LAB_Id + "\" id=\"" + LAB_Id + "\" data-options='{\"type\":\"datetime\"}' class='mui-pull-right ccformdate' style='margin-right:30px;margin-top:10px;min-width:160px;text-align:right;'>" + Ctrl_Text + "</span>";
        inputHtml += "</a>";
        inputHtml += "<input  type='hidden' name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" value='" + Ctrl_Val + "' />";
        return inputHtml;
    },
    CreateCBBoolean: function (Ctrl_Val) {
        var checkBoxVal = "";
        var Ctrl_Id = "CB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        //var Ctrl_Val = this.DataRow[this.control.KeyOfEn];
        var CB_Html = "<fieldset data-role=\"controlgroup\">";
        if (Ctrl_Val == "1" || Ctrl_Val == "是")
            checkBoxVal = "checked='checked'";
        CB_Html += "  <label style='font-size:15px;width:120px;padding-top:4px' for=\"" + Ctrl_Id + "\">" + this.control.Name + "</label>";
        CB_Html += "  <input type='hidden' name='" + Ctrl_Id + "' value='0'/>";
        CB_Html += "  <input style='width:75px;margin-top:8px' " + this.Ctrl_Class + " type='checkbox' name='" + Ctrl_Id + "'  id='" + Ctrl_Id + "'" + checkBoxVal +  "/>";
        CB_Html += "</fieldset>";
        return CB_Html;
    },
    CreateDDLEnum: function (Ctrl_Val) {
        //下拉框和单选都使用下拉框实现

        var Ctrl_Id = "RB_" + this.control.KeyOfEn + "_" + this.DataRow.OID
        if (this.control.UIContralType == UIContralType.DDL) {
            Ctrl_Id = "DDL_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        }

        //获取枚举数据
        var enums = new Entities("BP.Sys.SysEnums");
        enums.Retrieve("EnumKey", this.control.UIBindKey);

        var html_Select = "";
        html_Select += "<select name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" " + this.Ctrl_Class + ">";

        for (var i = 0; i < enums.length; i++) {
            if (Ctrl_Val == enums[i].IntKey) {
                html_Select += "<option value=\"" + enums[i].IntKey + "\" selected='selected'>" + enums[i].Lab + "</option>";
            } else {
                html_Select += "<option value=\"" + enums[i].IntKey + "\">" + enums[i].Lab + "</option>";
            }
        }
        html_Select += "</select>";
        return html_Select;
    },
    CreateDDLPK: function (Ctrl_Val) {
        var args = new RequestArgs();
        // var Ctrl_Val = this.DataRow[this.control.KeyOfEn];
        var Ctrl_Id = "DDL_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        var dtl_OID = this.DataRow.OID;
        var isEnable = this.Enable == true ? 1 : 0;
        var WorkID = args.WorkID;
        var html_Select = "";

        html_Select += "<select name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" " + this.Ctrl_Class + ">";
        if(isEnable == 0){
            if( this.DataRow[this.control.KeyOfEn+"Text"]!=null&&this.DataRow[this.control.KeyOfEn+"Text"]!=undefined )
                html_Select += "<option value=\"" + this.DataRow[this.control.KeyOfEn] + "\">" + this.DataRow[this.control.KeyOfEn+"Text"] + "</option>";
            else
                html_Select += "<option value=\"" + this.DataRow[this.control.KeyOfEn] + "\">" + this.DataRow[this.control.KeyOfEn+"T"] + "</option>";
        }
        var pushData = mainData[this.control.UIBindKey];
        //获取外键表数据,不存在表及sql方法
        var sfTable = new Entity("BP.Sys.SFTable", this.control.UIBindKey);
        if (sfTable != null && sfTable != "") {
            var selectStatement = sfTable.SelectStatement;
            var srcType = sfTable.SrcType;
            //Handler 获取外部数据源
            if (srcType == 5)
                pushData = DBAccess.RunDBSrc(selectStatement, 1);
            //JavaScript获取外部数据源
            if (srcType == 6)
                pushData = DBAccess.RunDBSrc(sfTable.FK_Val, 2);

        }
        if(pushData!=null) {
            for (var i = 0; i < pushData.length; i++) {
                if (Ctrl_Val == pushData[i].No) {
                    html_Select += "<option value=\"" + pushData[i].No + "\" selected='selected'>" + pushData[i].Name + "</option>";
                } else {
                    html_Select += "<option value=\"" + pushData[i].No + "\">" + pushData[i].Name + "</option>";
                }
            }
        }
        html_Select += "</select>";
        return html_Select;
    }
}

//处理 MapExt 的扩展. 工作处理器，独立表单都要调用他.
function AfterBindDtl_DealMapExt(dtlForm, OID) {
    var mapExts = dtlForm.Sys_MapExt;
    var mapAttrs = dtlForm.Sys_MapAttr;
    // 主表扩展(统计从表)
    var detailExt = {};
    wxh: for (var i = 0; i < mapExts.length; i++) {
        var mapExt = mapExts[i];

        //一起转成entity.
        var mapExt = new Entity("BP.Sys.MapExt", mapExt);
        var mapAttr = null;
        for (var j = 0; j < mapAttrs.length; j++) {
            
            if (mapAttrs[j].FK_MapData == mapExt.FK_MapData && mapAttrs[j].KeyOfEn == mapExt.AttrOfOper) {
                if (mapAttrs[j].UIIsEnable == 0 && (mapExt.ExtType == 'PopBranchesAndLeaf' || mapExt.ExtType == 'PopBranches' || mapExt.ExtType == 'PopTableSearch' || mapExt.ExtType == 'PopGroupList'))
                    continue wxh;//如果控件只读，并且是pop弹出形式，就continue到外循环
                mapAttr = mapAttrs[j];
                break;
            }
        }
        
        switch (mapExt.ExtType) {
            case "MultipleChoiceSmall":
                MultipleChoiceSmall(mapExt, mapAttr); //调用 /CCForm/JS/MultipleChoiceSmall.js 的方法来完成.
                break;
            case "MultipleChoiceSearch":
                MultipleChoiceSearch(mapExt); //调用 /CCForm/JS/MultipleChoiceSmall.js 的方法来完成.
                break;
            case "PopBranchesAndLeaf": //树干叶子模式.
                Dtl_PopBranchesAndLeaf(mapExt, mapAttr, OID); //调用 /CCForm/JS/Pop.js 的方法来完成.
                break;
            case "PopBranches": //树干简单模式.           	
                Dtl_PopBranches(mapExt, mapAttr, OID); //调用 /CCForm/JS/Pop.js 的方法来完成.
                break;
            case "PopGroupList": //分组模式.
                PopGroupList(mapExt, mapAttr); //调用 /CCForm/JS/Pop.js 的方法来完成.
                break;
            case "PopSelfUrl": //自定义url.
                SelfUrl(mapExt, mapAttr); //调用 /CCForm/JS/MultipleChoiceSmall.js 的方法来完成.
                break;
            case "PopTableSearch": //表格查询.
                Dtl_PopTableSearch(mapExt, mapAttr, OID); //调用 /CCForm/TableSearch.js 的方法来完成.
                break;
            case "PopVal": //PopVal窗返回值.
                var tb = $('[name$=' + mapExt.AttrOfOper + ']');
                tb.attr("onclick", "ShowHelpDiv('TB_" + mapExt.AttrOfOper + "','','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "','returnvalccformpopval');");
                tb.attr("ondblclick", "ReturnValCCFormPopValGoogle(this,'" + mapExt.MyPK + "','" + mapExt.FK_MapData + "', " + mapExt.W + "," + mapExt.H + ",'" + GepParaByName("Title", mapExt.AtPara) + "');");

                tb.attr('readonly', 'true');
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

                if (tb.attr('class') != undefined && tb.attr('class').indexOf('CheckRegInput') > 0) {
                    break;
                } else {
                    tb.addClass("CheckRegInput");
                    tb.data(mapExt)

                }
                break;
            case "InputCheck": //输入检查
                break;
            case "TBFullCtrl": //自动填充
                TBFullCtrl(mapExt, mapAttr);
                break;
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
                    }
                }

                break;
            case "ActiveDDL": /*自动初始化ddl的下拉框数据. 下拉框的级联操作 已经 OK*/
                var ddlPerant = $("#DDL_" + mapExt.AttrOfOper + "_" + OID);
                var ddlChild = $("#DDL_" + mapExt.AttrsOfActive + "_" + OID);
                if (ddlPerant == null || ddlChild == null)
                    continue;

                ddlPerant.attr("onchange", "DDLAnsc(this.value,\'" + "DDL_" + mapExt.AttrsOfActive + "_" + OID + "\', \'" + mapExt.MyPK + "\',\'" + ddlPerant.val() + "\')");

                var valClient = ConvertDefVal(frmData, '', mapExt.AttrsOfActive); // ddlChild.SelectedItemStringVal;

                //初始化页面时方法加载

                DDLAnsc(ddlPerant.val(), "DDL_" + mapExt.AttrsOfActive + "_" + OID, mapExt.MyPK, dbSrc, mapExt.DBType);

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
            case "DDLFullCtrl": // 自动填充其他的控件..

                var ddlOper = $("#DDL_" + mapExt.AttrOfOper);
                if (ddlOper.length == 0)
                    continue;

                var enName = frmData.Sys_MapData[0].No;
                var dbSrc = mapExt.Doc;

                //SQL 数据源获取
                if (mapExt.DBType == 0)
                    dbSrc = "";

                ddlOper.attr("onchange", "Change('" + enName + "');DDLFullCtrl(this.value,\'" + "DDL_" + mapExt.AttrOfOper + "\', \'" + mapExt.MyPK + "\')");


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
                        var operations = '';

                        ddlC1.children().remove();
                        ddlC1.html(operations);
                        //ddlC1.SetSelectItem(valC1);
                    }
                }
                break;
        }
    }
}

/******************************************  树干枝叶模式 **********************************/
function Dtl_PopBranchesAndLeaf(mapExt, mapAttr, OID) {
    if (mapAttr.UIVisible == 0) {
        return;
    }
    var attrOfOper = mapExt.AttrOfOper + "_" + OID;
    var ctrlID = "TB_" + mapExt.AttrOfOper + "_" + OID;
    var target = $("#" + ctrlID);
    target.hide();
    var parentTarget = target.parent();

    if (mapAttr.UIIsEnable != 0 && mapAttr.UIVisible != 0) {
        //增加a标签
        var aLink = $('<a class="mui-navigate-right"></a>');
        aLink.on('tap', function () {
            viewApi.go("#branchesAndLeaf");
            initBranchesLPage(mapExt, OID, attrOfOper);
        });
        aLink.append($('<p style="margin-right:30px;margin-top:10px;min-width:160px;text-align:right;">请选择</p>'));
        parentTarget.append(aLink);
    }

    //增加显示选择结果行
    var width = target.width();
    var height = target.height();
    var container = $("<div class='mui-table-view-cell'></div>");
    parentTarget.after(container);
    container.attr("id", attrOfOper + "_mtags");

    $("#" + attrOfOper + "_mtags").mtags({
        "fit": true,
        "onUnselect": function (record) {
            DeleteFrmEleDB(attrOfOper, OID, record.No);
            var mtags = $("#" + ctrlID + "_mtags i");
            var len = mtags.length;
            var RemoveFunc = mapExt.GetPara("RemoveFunc");

            if (RemoveFunc) {
                if (RemoveFunc.indexOf("(") == -1) {
                    RemoveFunc = RemoveFunc + "('" + record.No + "','" + len + "')";
                } else {
                    var para = record.No + "','" + len;
                    RemoveFunc = replaceAll(RemoveFunc, "Key", para);
                    RemoveFunc = replaceAll(RemoveFunc, "~", "'");
                }
                //调用移除函数
                DBAccess.RunDBSrc(RemoveFunc, mapExt.DBType,mapExt.FK_DBSrc);
            }
            console.log("unselect: " + JSON.stringify(record));
        }
    });

    //初始加载
    Refresh_Mtags(mapExt.FK_MapData, attrOfOper, OID, mapAttr);
    return;
}

/******************************************  树干模式 **********************************/
function Dtl_PopBranches(mapExt, mapAttr, OID) {
    if (mapAttr.UIVisible == 0) {
        return;
    }
    var attrOfOper = mapExt.AttrOfOper + "_" + OID;
    var ctrlID = "TB_" + mapExt.AttrOfOper + "_" + OID;

    var target = $("#" + ctrlID);
    target.hide();
    var parentTarget = target.parent();
    var oid = GetPKVal();
    if (mapAttr.UIIsEnable != 0 && mapAttr.UIVisible != 0) {
        //增加a标签
        var aLink = $('<a class="mui-navigate-right"></a>');
        aLink.on('tap', function () {
            viewApi.go("#branches");
            initBranchesPage(mapExt, OID, attrOfOper);
        });
        aLink.append($('<p style="margin-right:30px;margin-top:10px;min-width:160px;text-align:right;">请选择</p>'));
        parentTarget.append(aLink);
    }
    //增加显示选择结果行
    var width = target.width();
    var height = target.height();
    var container = $("<div class='mui-table-view-cell'></div>");
    parentTarget.after(container);
    container.attr("id", attrOfOper + "_mtags");

    $("#" + attrOfOper + "_mtags").mtags({
        "fit": true,
        "onUnselect": function (record) {
            console.log("unselect: " + JSON.stringify(record));
            DeleteFrmEleDB(attrOfOper, OID, record.No);

            var mtags = $("#" + attrOfOper + "_mtags i");
            var len = mtags.length;
            var RemoveFunc = mapExt.GetPara("RemoveFunc");

            if (RemoveFunc) {
                if (RemoveFunc.indexOf("(") == -1) {
                    RemoveFunc = RemoveFunc + "('" + record.No + "','" + len + "')";
                } else {
                    var para = record.No + "','" + len;
                    RemoveFunc = replaceAll(RemoveFunc, "Key", para);
                    RemoveFunc = replaceAll(RemoveFunc, "~", "'");
                }
                //调用移除函数
                DBAccess.RunDBSrc(RemoveFunc, mapExt.DBType, mapExt.FK_DBSrc);
            }
        }
    });
    //初始加载
    Refresh_Mtags(mapExt.FK_MapData, attrOfOper, OID, mapAttr);
}

/******************************************  表格查询 **********************************/
function Dtl_PopTableSearch(mapExt, mapAttr, OID) {
    if (mapAttr.UIVisible == 0) {
        return;
    }
    var attrOfOper = mapExt.AttrOfOper + "_" + OID;
    var ctrlID = "TB_" + mapExt.AttrOfOper + "_" + OID;

    var target = $("#" + ctrlID);
    target.hide();
    var parentTarget = target.parent();
    var oid = OID;
    if (mapAttr.UIIsEnable != 0 && mapAttr.UIVisible != 0) {
        //增加a标签
        var aLink = $('<a class="mui-navigate-right"></a>');
        aLink.on('tap', function () {
            viewApi.go("#tableSearch");
            initTableSPage(mapExt, oid, attrOfOper);
        });
        aLink.append($('<p style="margin-right:30px;margin-top:10px;min-width:160px;text-align:right;">请选择</p>'));
        parentTarget.append(aLink);
    }
    //增加显示选择结果行
    var width = target.width();
    var height = target.height();
    var container = $("<div class='mui-table-view-cell'></div>");
    parentTarget.after(container);
    container.attr("id", attrOfOper + "_mtags");


    $("#" + attrOfOper + "_mtags").mtags({
        "fit": true,
        "onUnselect": function (record) {
            DeleteFrmEleDB(attrOfOper, oid, record.No);
        }
    });
    //初始加载
    Refresh_Mtags(mapExt.FK_MapData, attrOfOper, oid, mapAttr);

}