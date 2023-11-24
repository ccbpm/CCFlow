//初始化信息
var ensName = GetQueryString("EnsName");
var webUser = new WebUser();
//当前用户查询信息.
var ur = new Entity("BP.Sys.UserRegedit");
ur.MyPK = webUser.No + "_" + ensName + "_SearchAttrs";
ur.RetrieveFromDBSources();

var cfg = new Entity("BP.Sys.EnCfg");
cfg.No = ensName;
cfg.RetrieveFromDBSources();

function InitToolBar() {
    //创建处理器.
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Comm");
    handler.AddUrlData();  //增加参数.
    //获得map基本信息.
    mapBase = handler.DoMethodReturnJSON("Search_MapBaseInfo");

    pkFiled = mapBase.EntityPK;
    $("#title").text(mapBase.EnDesc);
    document.title = mapBase.EnDesc;

    var data = handler.DoMethodReturnJSON("Search_SearchAttrs");
    //绑定外键枚举查询条件.
    var attrs = data["Attrs"];

    var html = "";
    var searchFields = mapBase.SearchFields;
    //关键字查询
    if (searchFields == null || searchFields == "" || searchFields == undefined) {
        searchFields = "";
        if (mapBase.IsShowSearchKey == "1") {
            var keyLabel = cfg.GetPara("KeyLabel");
            if (keyLabel == null || keyLabel == undefined || keyLabel == "")
                keyLabel = "关键字";
            html += '<div class="mui-input-row">';
            html += '<label>' + keyLabel + '</label>';
            html += '<input type="text" class="mui-input-clear" placeholder="输入关键字" data-input-clear="2" id="TB_Key" name="TB_Key" value="' + ur.SearchKey + '">';
            html += '</div>';
        }
        //String字段查询
    } else {
        var strs = searchFields.split("@");
        var str;
        var fieldV = "";
        for (var i = 0; i < strs.length; i++) {
            if (strs[i] == "")
                continue;

            str = strs[i].split("=");
            if (str.length < 2 || str[0] == "" || str[1] == "")
                continue;
            fields.push(str[1]);
            fieldV = ur.GetPara(str[1]);
            if (fieldV == null || fieldV == undefined)
                fieldV = "";

            html += '<div class="mui-input-row">';
            html += '<label>' + str[0] + '</label>';
            html += '<input type="text" class="mui-input-clear"  data-input-clear="2" id="TB_' + str[1] + '" name="TB_' + str[1] + '" value="' + fieldV + '">';
            html += '</div>';

        }
    }

    //数值型的查询增加
    var searchFieldsOfNum = mapBase.SearchFieldsOfNum;
    var val1 = "";
    var val2 = "";
    if (searchFieldsOfNum != null && searchFieldsOfNum != undefined && searchFieldsOfNum != "") {
        var strs = searchFieldsOfNum.split("@");
        var str;
        var fieldV = "";
        for (var i = 0; i < strs.length; i++) {
            if (strs[i] == "")
                continue;

            str = strs[i].split("=");
            if (str.length < 2 || str[0] == "" || str[1] == "")
                continue;
            fields.push(str[1]);
            fieldV = ur.GetPara(str[1]);
            if (fieldV == null || fieldV == undefined || fieldV == "") {
                val1 = "";
                val2 = "";
            } else {
                val1 = fieldV.split(',')[0];
                val2 = fieldV.split(',')[1];
            }


            html += '<div class="mui-input-row">';
            html += '<label>' + str[0] + '</label>';
            html += '<input type="text" class="mui-input-clear congDao"  data-input-clear="2" id="TB_' + str[1] + '_0" name="TB_' + str[1] + '" value="' + val1 + '">';
            html += "<label style='width:5%;padding:11px 0px'>～&nbsp;&nbsp;</label>";
            html += '<input type="text" class="mui-input-clear congDao"  data-input-clear="2" id="TB_' + str[1] + '_1" name="TB_' + str[1] + '" value="' + val2 + '">';
            html += '</div>';

        }
    }


    if (mapBase.DTSearchWay != "0") {
        var dateType = "date";
        if (mapBase.DTSearchWay != "1")
            dateType = "datatime";
        html += '<div class="mui-input-row">';
        html += '<label>' + mapBase.DTSearchLabel + '</label>';
        html += "<a class='mui-navigate-right'>";
        html += "<span name='LAB_DTFrom' id='LAB_DTFrom' data-options='{\"type\":\"" + dateType + "\"}' class='mui-pull-right ccformdate' style='min-width:180px;padding-top:10px;'><p>请选择日期</p></span>";
        html += "</a>";
        html += "<input  type='hidden' name='TB_DTFrom' id='TB_DTFrom' value='" + ur.DTFrom + "'/>";
        html += "</div>";

        html += '<div class="mui-input-row">';
        html += '<label>到</label>';
        html += "<a class='mui-navigate-right'>";
        html += "<span name='LAB_DTTo' id='LAB_DTTo' data-options='{\"type\":\"" + dateType + "\"}' class='mui-pull-right ccformdate' style='min-width:180px;padding-top:10px;'><p>请选择日期</p></span>";
        html += "</a>";
        html += "<input  type='hidden' name='TB_DTTo' id='TB_DTTo' value='" + ur.DTTo + "'/>";

        html += "</div>";
    }

    $("#toolBar").append(html); //设置基础信息.

    //格式为: @WFSta=0@FK_Dept=02
    var json = AtParaToJson(ur.Vals);

    for (var i = 0; i < attrs.length; i++) {
        var attr = attrs[i];
        var str = "";
        str += '<div class="mui-input-row">';
        str += "<label>" + attr.Name + "</label>";

        str += "<select class='form-control' style='margin-top:5px;width:" + attr.Width + "px' name='DDL_" + attr.Field + "' ID='DDL_" + attr.Field + "'>" + InitDDLOperation(data, attr, "all") + "</select>";
        str += "</div>"
        str = $(str);
        $("#toolBar").append(str); //设置基础信息.
    }

    html = "";
    //为查询外键赋值.
    for (var i = 0; i < attrs.length; i++) {
        var attr = attrs[i];
        var selectVal = json[attr.Field];

        if (selectVal == undefined || selectVal == "")
            selectVal = "all";

        //判断是否有级联关系
        var myPK = "ActiveDDL_" + ensName + "_" + attr.Field;
        var mapExt = new Entity("BP.Sys.MapExt");
        mapExt.SetPKVal(myPK);
        var isExist = mapExt.RetrieveFromDBSources();
        //处理级联关系
        if (isExist == 1) {
            var ddlPerant = $("#DDL_" + mapExt.AttrOfOper);
            var ddlChild = $("#DDL_" + mapExt.AttrsOfActive);
            if (ddlPerant != null && ddlChild != null) {
                ddlPerant.attr("onchange", "DDLRelation(this.value,\'" + "DDL_" + mapExt.AttrsOfActive + "\', \'" + mapExt.MyPK + "\',\'" + ddlPerant.val() + "\')");

            }
        }

        $("#DDL_" + attr.Field).val(selectVal);
    }
    html = "";
    html += '<div class="mui-button-row">';
    html += '<button type="button" class="mui-btn mui-btn-primary" onclick="Search()">查询</button>&nbsp;&nbsp;';
    html += '</div>';
    $("#toolBar").append(html);

    var pickdates = $(".ccformdate");
    if (pickdates.length > 0) {
        $("#LAB_DTFrom").html(ur.DTFrom);
        $("#LAB_DTTo").html(ur.DTTo);
        $("#TB_DTFrom").val(ur.DTFrom);
        $("#TB_DTTo").val(ur.DTTo);
    }
    pickdates.each(function (i, pickdate) {
        var id = this.getAttribute('id');
        if ($("#" + id).html() == '') {
            $("#" + id).html("<p>请选择时间<p>");
        }

        pickdate.addEventListener('tap', function () {
            var _self = this;
            var optionsJson = this.getAttribute('data-options') || '{}';
            var options = JSON.parse(optionsJson);
            var id = this.getAttribute('id');
            _self.picker = new mui.DtPicker(options);
            _self.picker.show(function (rs) {
                $("#" + id).html(rs.text);
                $("#TB_" + id.substr(4)).val(rs.text);
                _self.picker.dispose();
                _self.picker = null;

            });
        }, false);


    });



}
//初始化下拉列表框的OPERATION
function InitDDLOperation(frmData, mapAttr, defVal) {

    var operations = "";
    operations += "<option value='all' >全部</option>";

    var ens = frmData[mapAttr.Field];
    if (ens == null) {
        ens = [{ 'IntKey': 0, 'Lab': '否' }, { 'IntKey': 1, 'Lab': '是' }];
    }
    for (var i = 0; i < ens.length; i++) {

        var en = ens[i];

        if (en.No == undefined)
            operations += "<option value='" + en.IntKey + "'>" + en.Lab + "</option>";
        else
            operations += "<option value='" + en.No + "'>" + en.Name + "</option>";
    }
    return operations;
}

function SearchCondition() {
    //保存查询条件.
    var ensName = GetQueryString("EnsName");
    var ur = new Entity("BP.Sys.UserRegedit");
    ur.MyPK = webUser.No + "_" + ensName + "_SearchAttrs";
    ur.FK_Emp = webUser.No;

    if ($("#TB_Key") != null && $("#TB_Key").val() != "")
        ur.SearchKey = $("#TB_Key").val();
    else
        ur.SearchKey = "";

    //增加字段查询
    var val = "";
    for (var i = 0; i < fields.length; i++) {
        var field = fields[i];
        var strs = $("input[name='TB_" + field + "']");
        if (strs.length == 1) {
            ur.SetPara(field, $("#TB_" + field).val());
        } else {
            if ($("#TB_" + field + "_0").val() == "" && $("#TB_" + field + "_1").val() == "")
                ur.SetPara(field, "");
            else
                ur.SetPara(field, $("#TB_" + field + "_0").val() + "," + $("#TB_" + field + "_1").val());
        }


    }

    //设置查询时间.
    if ($("#TB_DTFrom").length == 1)
        ur.DTFrom = $("#TB_DTFrom").val();

    if ($("#TB_DTTo").length == 1)
        ur.DTTo = $("#TB_DTTo").val();

    //获得外键的查询条件,存储里面去.
    var str = "";
    $("select[name^='DDL_']").each(function () {
        var id = $(this).attr("id");
        id = id.replace("DDL_", "");
        str += "@" + id + "=" + $(this).val();
    });

    ur.FK_Emp = webUser.No;
    ur.CfgKey = "SearchAttrs";
    ur.Vals = str;
    ur.FK_MapData = ensName;
    ur.Save();
}

/**
        *初始化数据信息
        */
function InitData() {
    //创建处理器.
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Comm");
    handler.AddUrlData()
    handler.AddPara("PageIdx", pageIdx);
    handler.AddPara("PageSize", pageSize);
    //查询集合
    var data = handler.DoMethodReturnString("Search_SearchIt");
    if (data.indexOf('err@') == 0) {
        mui.alert(data);
        return;
    }

    //当前用户页面信息.
    var ur = new Entity("BP.Sys.UserRegedit");
    ur.MyPK = webUser.No + "_" + ensName + "_SearchAttrs";
    ur.RetrieveFromDBSources();

    var count = ur.GetPara("RecCount");
    if (count % pageSize != 0)
        pages = parseInt(count / pageSize) + 1;
    else
        pages = parseInt(count / pageSize);

    if (pages == 0) pages = 1;


    data = JSON.parse(data);
    mapAttrs = data.Attrs;
    return data["DT"];
}

function GetShowFields() {
    var idx = 0;
    if (mobileShowModel == 0 || mobileShowContent=="") {
        //默认显示map的可见的前四个字段
        $.each(mapAttrs, function (i, mapAttr) {
            if (mapAttr.UIVisible == 0
                || mapAttr.KeyOfEn == "OID"
                || mapAttr.KeyOfEn == "WorkID"
                || mapAttr.KeyOfEn == "NodeID"
                || mapAttr.KeyOfEn == "MyNum"
                || mapAttr.KeyOfEn == "MyPK") 
                return true;
            if (idx < 4)
                showField.push(mapAttr);
            idx++;
        });
    }
    if (mobileShowModel == 1 && mobileShowContent != "") {
        mobileShowContent = mobileShowContent + ",";
        mobileShowContent = replaceAll(mobileShowContent, " ", "");
        $.each(mapAttrs, function (i, mapAttr) {
            if (mapAttr.UIVisible == 0
                || mapAttr.KeyOfEn == "OID"
                || mapAttr.KeyOfEn == "WorkID"
                || mapAttr.KeyOfEn == "NodeID"
                || mapAttr.KeyOfEn == "MyNum"
                || mapAttr.KeyOfEn == "MyPK")
                return true;
            //记录换行的字段

            if (mobileShowContent.indexOf("@" + mapAttr.KeyOfEn + ",") >= 0
                || mobileShowContent.indexOf("@" + mapAttr.KeyOfEn + "@")>=0) {
                mapAttr.IsCR =1;
                showField.push(mapAttr);
                return true;
            }
            if (mobileShowContent.indexOf(mapAttr.KeyOfEn + ",") != -1
                || mobileShowContent.indexOf(mapAttr.KeyOfEn + "@") != -1) {
                mapAttr.IsCR = 0;
                showField.push(mapAttr);
            }
           
        });
    }
    if (mobileShowModel == 2 && mobileShowContent != "") {
        $.each(mapAttrs, function (i, mapAttr) {
            if (mapAttr.UIVisible == 0
                || mapAttr.KeyOfEn == "OID"
                || mapAttr.KeyOfEn == "WorkID"
                || mapAttr.KeyOfEn == "NodeID"
                || mapAttr.KeyOfEn == "MyNum"
                || mapAttr.KeyOfEn == "MyPK")
                return true;
            if (mobileShowContent.indexOf("{"+mapAttr.KeyOfEn + "}") != -1)
                showField.push(mapAttr);

        });
    }
    
}
function ShowPageInfo(pageType) {
    pageType = pageType || "search";
    var pageData = InitData();
    GetShowFields();

    //判断字段中存在的主键
    var table = document.body.querySelector('.mui-table-view');
    //加载数据
    var _html = "";
    var val = "";
    $.each(pageData, function (i, item) {
        var li = document.createElement('li');
        if (pageType == "batch") {
            li.className = 'mui-table-view-cell';
            li.style.paddingRight = "0px";
        }
           
        else
            li.className = 'mui-table-view-cell  mui-collapse';
        li.id = item[pkFiled];
        _html = "";
        if (pageType == "batch") {
            _html += '<div class="mui-row mui-checkbox mui-left " >';
            _html += '<label>';
            _html += GetRowInfo(item);
            _html += '</label>';
            _html += '<input name="checkbox" id="CB_' + item[pkFiled]+'" type="checkbox">';
            _html += '</div > ';
            _html += '<div class="mui-row mui-left" >';
            _html += "<button type='button' class='mui-btn' style='float:right;margin-right:20px;padding:7px 15px;margin-top:5px;line-height: 0.9' onclick='OpenEn(\"" + item[pkFiled] + "\")'>查看</button>";
            _html += '</div>';
            //_html += '</a>';
        } else {
            _html = '<a class="mui-navigate-right" href="javascript:void(0)">';
            _html += '<div class="mui-row">';
            _html += GetRowInfo(item);
            _html += '</div > ';
            _html += '</a>';
            _html += '<ul  class="mui-table-view mui-table-view-chevron">';
            _html += '<li class="mui-table-view-cell">';
            _html += '<form class="mui-input-group">';
            _html += ShowInfoByMapAttr(mapAttrs, item);
            _html += '</form>';
            _html += '</li>';
            _html += '</ul>';
        }
       
        li.innerHTML = _html;
        table.appendChild(li);
    });
}
function GetRowInfo(row) {
    var _html = "";
    if (mobileShowModel == 0 || mobileShowContent == "") {
        showField.forEach(attr => {
            val = GetFieldValue(attr, row);
            _html += '<div style="color:#8f8f94">' + attr.Name + ':' + val + '</div>';
        })
    }
    if (mobileShowModel == 1 && mobileShowContent != "") {
        //mobileShowContent遇到@就换行
        showField.forEach(attr => {
            val = GetFieldValue(attr, row);
            if (attr.IsCR == 1)
                _html += "<br/>";
            _html += '<div style="display:inline;color:#8f8f94;padding-right:20px">' + attr.Name + ':' + val + '</div>';
           
        })
    }
    if (mobileShowModel == 2 && mobileShowContent != "") {
        showField.forEach(attr => {
            val = GetFieldValue(attr, row);
            mobileShowContent = mobileShowContent.replace(attr.KeyOfEn, val);
        })
        _html += mobileShowContent;
    }
    return _html;
}
function GetFieldValue(attr, row) {
    val = row[attr.KeyOfEn] || "";
    if (attr.UIContralType == 1) {
        val = row[attr.KeyOfEn + "T"] || "";
        if (val == "")
            val = row[attr.KeyOfEn + "Text"] || "";
    } if (attr.UIContralType == 2) {
        if (val == "0") val = "否";
        if (val == "1") val = "是";
    }
    if (attr.MyDataType == "6") {
        if (val != "")
            val = FormatDate(new Date(val), "yyyy-MM-dd");
    }
    return val;
}
/**
 * 数据显示
 * @param {any} mapAttrs
 * @param {any} data
 */
function ShowInfoByMapAttr(mapAttrs, data) {
    var lab = "";
    var val = "";
    _html = "";
    var i = 0;
    $.each(mapAttrs, function (idx, mapAttr) {
        if (mapAttr.UIVisible == 0
            || mapAttr.KeyOfEn == "OID"
            || mapAttr.KeyOfEn == "WorkID"
            || mapAttr.KeyOfEn == "NodeID"
            || mapAttr.KeyOfEn == "MyNum"
            || mapAttr.KeyOfEn == "MyPK") {
            return true;
        }
        lab = mapAttr.Name;
        val = data[mapAttr.KeyOfEn] || "";
        if (mapAttr.UIContralType == 1) {
            val = data[mapAttr.KeyOfEn + "T"] || "";
            if (val == "")
                val = data[mapAttr.KeyOfEn + "Text"] || "";
        }
        if (mapAttr.UIContralType == 2) {
            if (val == "0") val = "否";
            if (val == "1") val = "是";
        }
        if (mapAttr.MyDataType == "6") {
            if (val != "")
                val = FormatDate(new Date(val), "yyyy-MM-dd");
        }
        if (mapAttr.IsRichText == "1") {
            _html += '<div class="mui-input-row">';
            _html += '<label><p>' + lab + '</p></label>';
            _html += '</div >';
            _html += '<div class="mui-input-row">';
            _html += '<textarea id="textarea" rows="5" readonly="readonly"  disabled="disabled">' + val + '</textarea>';
            _html += '</div >';
            i++;
            return true;
        }
        _html += '<div class="mui-input-row">';
        _html += '<label><p>' + lab + '</p></label>';
        if (i == 0) {
            _html += "<button type='button' class='mui-btn' style='float:right;width:20%;padding:7px 15px;margin-top:5px;;line-height: 0.9' onclick='OpenEn(\"" + data[pkFiled] +"\")'>更多</button>";
            _html += '<input style="width:45%"  value="' + val + '" readonly="readonly" type="text"  disabled="disabled"/>';
        } else {
            _html += '<input style="background-color:#fff " value="' + val + '" readonly="readonly" type="text"  disabled="disabled"/>';
        }
        _html += '</div >';
        i++;
    })
    return _html;
}

function OpenEn(pkval) {
    url = cfg.UrlExt;
    var urlOpenType = cfg.GetPara("SearchUrlOpenType");

    if (urlOpenType == 0 || urlOpenType == undefined)
        url = "./RefFunc/En.htm?EnName=" + ensName.substr(0, ensName.length - 1) + "&PKVal=" + pkval;

    if (urlOpenType == 1)
        url = "./RefFunc/EnOnly.htm?EnName=" + ensName.substr(0, ensName.length - 1) + "&PKVal=" + pkval;

    if (urlOpenType == 2)
        url = "../FrmViw.htm?FK_MapData=" + GetQueryString("EnsName") + "&PKVal=" + pkval;

    if (urlOpenType == 3)
        url = "../FrmViw.htm?FK_MapData=" + GetQueryString("EnsName") + "&PKVal=" + pkval;

    if (urlOpenType == 9) {
        if (url.indexOf('?') == -1)
            url = url + "?1=1";
        if (url.indexOf('FrmID') != -1)
            url = url + "&WorkID=" + pkval + "&OID=" + pkval;
        else
            url = url + "&EnsName=" + ensName + "&EnName=" + enName + "&PKVal=" + pkval;
    }

    window.parent.location.href = url;
}