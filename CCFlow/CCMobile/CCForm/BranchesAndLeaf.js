var mapExt = null;
var webUser;
var selectJSON;
var jsonArray;
var treeClickUrl;
var global = window;
function initBranchesLPage(obj, oid, dtlKeyOfen, type) {
    global.selectedRows = [];

    mapExt = obj;
    //登录用户信息
    webUser = new WebUser();
    var list = document.getElementById('list');
    window.indexedList = new mui.IndexedList(list);

    //设置变量.
    global.FK_MapData = mapExt.FK_MapData;
    global.AttrOfOper = dtlKeyOfen ? dtlKeyOfen : mapExt.AttrOfOper;
    global.selectType = mapExt.GetPara("SelectType");
    global.oid = oid;

    //跟节点编号.
    var rootNo = mapExt.Doc;
    if (rootNo == "@WebUser.FK_Dept") {
        rootNo = webUser.FK_Dept;
    }


    var treeUrl = mapExt.Tag2;      //初始化树
    treeClickUrl = mapExt.Tag3; //树选择.
    if (treeUrl == "" || treeClickUrl == "") {
        alert('配置错误:查询数据源，初始化树的数据源，点击树节点返回数据的数据源，必须都不能为空。');
        return;
    }
    global.TreeUrl = treeUrl;
    global.IsLazy = treeUrl.substr(treeUrl.toLowerCase().indexOf("where")).indexOf("ParentNo") != -1 ? true : false;
    treeUrl = treeUrl.replace(/@Key/g, rootNo);
    jsonArray = DBAccess.RunDBSrc(treeUrl, mapExt.DBType, mapExt.FK_DBSrc);

    //获取已选择的项
    var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
    frmEleDBs.Retrieve("FK_MapData", FK_MapData, "EleID", AttrOfOper, "RefPKVal", oid);
    $.each(frmEleDBs, function (i, o) {
        global.selectedRows.push({
            "No": o.Tag1,
            "Name": o.Tag2,
            "POP_Value": o.Tag3
        });
    });

    global.count = frmEleDBs.length;
    //改变完成初始状态
    changeDoneState(count, "done");


    //搜索框提示
    Search(global,type);

    global.rootNo = rootNo;
    //根节点名称
    global.rootName = "根目录";
    for (var i = 0; i < jsonArray.length; i++) {
        var data = jsonArray[i];
        if (data.No == rootNo) {
            rootName = data.Name;
            break;
        }
    }
    //解析树形结构
    loadTree(rootNo, rootNo, rootName, global.selectType, type);

    mui('.mui-indexed-list-inner').on('change', 'input', function () {
        var obj = this;
        //获取方法
        cceval(this.attributes.getNamedItem('onclick').textContent);
    });
};

//搜索框的设置
function Search(global, type) {
    //设置标题.
    var title = mapExt.GetPara("Title");
    $("#BLTitle").text(title);
    //设置搜索提示框
    var tip = mapExt.GetPara("SearchTip");
    //var input=$('<input id="TB_Key" type="search" class="mui-input-clear mui-indexed-list-search-input" placeholder="'+tip+'" >');
    //$("#Search").append(input);
    var span = $("#TB_Key").siblings().eq(1).children().eq(1);
    span = span.html(tip);

    //查询
    var searchUrl = mapExt.Tag1;
    $("#TB_Key").on("keyup", function () {
        var keyVal = $(this).val();
        if (searchUrl != "") {
            var mySrc = replaceAll(searchUrl, "@Key", keyVal);
            mySrc = replaceAll(mySrc, "~", "'");
            //获得json.
            var json = DBAccess.RunDBSrc(mySrc, mapExt.DBType, mapExt.FK_DBSrc);
            selectJSON = json;
        }

        var newjson = $.grep(selectJSON, function (item) {
            return item.No.indexOf(keyVal) != -1 || item.Name.indexOf(keyVal) != -1;
        });
        loadData(newjson, null, 0, global.selectType, type);
    });
}



//加载结构
function loadTree(currentNo, parentNo, parentName, selectType,type) {
    var slider = $("#LevelIdx");
    if (currentNo == global.rootNo) {
        slider.attr("href", "javaScript:void(0);loadTree(\"" + parentNo + "\",\"" + parentNo + "\",\"" + parentName + "\",\"" + selectType + "\")");
        slider.text(global.rootName);
    } else {
        slider.text(parentName);
        slider.attr("href", "javaScript:void(0);changeParentNo(\"" + parentNo + "\")");
    }
    var childJson;
    if (global.IsLazy == true)
        childJson = DBAccess.RunDBSrc(global.TreeUrl.replace(/@Key/g, currentNo), mapExt.DBType);
    else
        childJson = getJsonByParentNo(jsonArray, currentNo);
    var infoJson = getInfoJsonByParentNo(treeClickUrl, currentNo);
    loadData(infoJson, childJson, 1, global.selectType, type);
}

//返回上一级
function changeParentNo(rootNo, id) {
    //获取该节点的上一节点
    var parentNo = rootNo;
    var rootName;
    for (var i = 0; i < jsonArray.length; i++) {
        var data = jsonArray[i];
        if (data.No == rootNo) {
            parentNo = data.ParentNo;
            rootName = data.Name;
            break;
        }
    }
    if (id == "branches") {
        loadBranchesTree(rootNo, parentNo, rootName);
    } else {
        loadTree(rootNo, parentNo, rootName, global.selectType);
    }
}
//获取父节点下的所有节点
function getJsonByParentNo(jsonArray, parentNo) {
    var treeJson = [];
    for (var i = 0; i < jsonArray.length; i++) {
        var data = jsonArray[i];
        if (data.ParentNo == parentNo) {
            treeJson.push(data);
        }
    }
    return treeJson;
}

//获取节点关联的信息
function getInfoJsonByParentNo(treeClickUrl, rootNo) {
    var myurl = replaceAll(treeClickUrl, "@Key", rootNo);
    myurl = replaceAll(myurl, "~", "'");
    //获得json.
    var json = DBAccess.RunDBSrc(myurl, mapExt.DBType, mapExt.FK_DBSrc);
    return json;
}
//加载数据
function loadData(infoJson, childJson, showType, selectType, type) {
    $("#list").children('.mui-indexed-list-inner').removeClass('empty');
    var ul = $("#treeView");
    ul.html('');
    if (showType == 1) {
        //解析树形结构
        if ($.isArray(childJson) && childJson.length > 0) {
            $.each(childJson, function (i, o) {
                var li = '<li data-value="' + o.No + '" data-tags="' + o.Name + '" class="mui-table-view-cell mui-indexed-list-item  mui-left">';
                li += "<a class='mui-navigate-right' href='javaScript:void(0);loadTree(\"" + o.No + "\",\"" + o.ParentNo + "\",\"" + o.Name + "\",\"" + global.selectType + "\")'>" + o.Name + "</a></li>";
                ul.append(li);
            });
        }
    }

    if (!$.isArray(infoJson) || infoJson.length == 0) {
        var list = document.getElementById('list');

        //        $("#list").children('.mui-indexed-list-inner').addClass('empty');
        //$("#alert").show();
        return false;
    }

    //解析数据信息
    var globalSelectedRows = global.selectedRows;
    $.each(infoJson, function (i, o) {
        var isEqual = false;
        for (var index = 0; index < globalSelectedRows.length; index++) {
            if (o.No == globalSelectedRows[index].No) {
                isEqual = true;
                break;
            }
        }
        if (selectType == 0) {
            var li = '<li data-value="' + o.No + '" data-tags="' + o.Name + '" class="mui-table-view-cell mui-indexed-list-item mui-radio mui-left">';
            if (isEqual) {
                li += "<input name='radio1' type='radio' checked='chedked' onclick='changeRadioStatus(this,\"" + o.No + "\",\"" + o.Name + "\",\"\"" + type + ")'/>" + o.Name + "</li>";
            } else {
                li += "<input name='radio1' type='radio' onclick='changeRadioStatus(this,\"" + o.No + "\",\"" + o.Name + "\",\"\"" + type + ")'/>" + o.Name + "</li>";
            }
        } else {
            var li = '<li data-value="' + o.No + '" data-tags="' + o.Name + '" class="mui-table-view-cell mui-indexed-list-item mui-checkbox mui-left">';
            if (isEqual)
                li += "<input type='checkbox' checked='chedked' onclick='changeSeltStatus(this,\"" + o.No + "\",\"" + o.Name + "\",\"\")'/>" + o.Name + "</li>";
            else
                li += "<input type='checkbox' onclick='changeSeltStatus(this,\"" + o.No + "\",\"" + o.Name + "\",\"\")'/>" + o.Name + "</li>";
        }

        ul.append(li);
    });
    return true;
}
//点击单选框事件
function changeRadioStatus(obj, No, Name, Tag3, type) {
    var globalSelectedRows = global.selectedRows;
    if (obj.checked == true) {

        changeDoneState(globalSelectedRows.length, "done");
        //清空数据库中所有数据
        if (globalSelectedRows.length != 0) {
            for (var i = 0; i < globalSelectedRows.length; i++) {
                DeleteFrmEleDB(global.AttrOfOper, global.oid, globalSelectedRows[i].No);
            }

        }
        //存入数据
        SaveFrmEleDB(global.FK_MapData, global.AttrOfOper, global.oid, No, Name, "");
        globalSelectedRows.length = 0;//清空数组
        globalSelectedRows.push({
            "No": No,
            "Name": Name
        });
        var mtags = $("#" + global.AttrOfOper + "_mtags")
        mtags.mtags("loadData", globalSelectedRows); //给框赋值
        var text = mtags.mtags("getText");
        $("#TB_" + global.AttrOfOper).val(text); //给隐藏的input赋值
        //填充其他控件
        FullIt(globalSelectedRows[0].No, mapExt.MyPK, "TB_" + global.AttrOfOper, type);
    }


}
//点击复选框事件
function changeSeltStatus(obj, No, Name, Tag3) {
    var globalSelectedRows = global.selectedRows;
    if (obj.checked == true) {
        var sel = $.grep(globalSelectedRows, function (obj) {
            return obj.No == No;
        });
        if (sel.length == 0) {
            //选择取消复选框时改变状态
            global.count++;
            changeDoneState(global.count, "done");

            SaveFrmEleDB(global.FK_MapData, global.AttrOfOper, global.oid, No, Name, "");
            globalSelectedRows.push({
                "No": No,
                "Name": Name
            });
            var mtags = $("#" + global.AttrOfOper + "_mtags")
            mtags.mtags("loadData", globalSelectedRows); //给框赋值
            var text = mtags.mtags("getText");
            $("#TB_" + global.AttrOfOper).val(text); //给隐藏的input赋值
        }
    }

    else {
        for (var index = 0; index < globalSelectedRows.length; index++) {
            if (No == globalSelectedRows[index].No) {
                global.count--;
                changeDoneState(global.count, "done");
                DeleteFrmEleDB(global.AttrOfOper, global.oid, No);
                globalSelectedRows.splice(index, 1);
                var mtags = $("#" + global.AttrOfOper + "_mtags")
                mtags.mtags("loadData", global.selectedRows); //给框赋值
                var text = mtags.mtags("getText");
                $("#TB_" + global.AttrOfOper).val(text); //给隐藏的input赋值
                break;
            }
        }
    }

}