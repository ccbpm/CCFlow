//扩展设置信息
var mapExt = null;
var objID = null;
//初始化页面
function initTBFullCtrlPage(obj,mapAttr,oid,objID1,type) {
    objID = objID1;
    mapExt = obj;
    var webUser = new WebUser();

    var global = window;
    global.selectedRows = [];
    //设置变量
    global.FK_MapData = mapExt.FK_MapData;
    global.AttrOfOper = mapExt.AttrOfOper;
    global.oid = oid;

    //设置标题.
    $("#TBFCTitle").text(mapAttr.Name);

    //设置查询提示
    var span = $("#TB_TBFC_Key").siblings().eq(1).children().eq(1);
    span = span.html("输入"+mapAttr.Name+"的值");

    //点击关键字查询的操作
    $("#TB_TBFC_Key").on("keyup", function () {
        //debugger
        loadTBFullCtrlData();
    });

    //获取表格的数据源
    //获取表格的数据源
    dbSrc = mapExt.Doc;
    if (dbSrc == null || dbSrc == "")
        dbSrc = mapExt.Tag4;

    if (dbSrc == "" || dbSrc == "") {
        alert('配置错误:查询数据源，初始化数据源必须都不能为空。');
        return;
    }

    // 初始化加载
    var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
    frmEleDBs.Retrieve("FK_MapData", FK_MapData, "EleID", AttrOfOper, "RefPKVal", oid);
    $.each(frmEleDBs, function (i, o) {
        global.selectedRows.push({
            "No": o.Tag1,
            "Name": o.Tag2,
        });
    });


    global.count = frmEleDBs.length;
    //改变完成初始状态
    changeDoneState(count,"TBFCDone");

    global.searchTableColumns = [];
    //显示行号的添加
    global.searchTableColumns = [{
        formatter:function(value,row,index){
            return index+1;
        }
    }];

    var columns = mapExt.Tag3;
    //设置bootstrapTable显示列的中文名称.
    if (typeof columns == "string" && columns!=null && columns!="") {

        $.each(columns.split(","), function (i, o) {
            var exp = o.split("=");
            var field;
            var title;
            if (exp.length == 1) {
                field = title = exp[0];
            } else if (exp.length == 2) {
                field = exp[0];
                title = exp[1];
            }
            if (!isLegalName(field)) {
                return true;
            }
            searchTableColumns.push({
                field: field,
                title: title
            });
        });
    } else {
        // by default
        searchTableColumns.push({
            field: 'No',
            title: '编号'
        });
        searchTableColumns.push({
            field: 'Name',
            title: '名称'
        });
    }

    //设置bootstrapTable 表格选项
    var options = {
        striped: true,
        cache: false,
        showHeader:true,
        sortOrder: "asc",
        strictSearch: true,
        minimumCountColumns: 2,
        clickToSelect: true,
        sortable: false,
        cardView: false,
        detailView: false,
        uniqueId: "No",
        columns: searchTableColumns
    };

    //选中行的操作
    options.onClickRow = function (row, element) {
        $(".success").removeClass('success');
        $(element).addClass('success');
        $("#" + objID).show();
        $("#" + objID).val(row.No);
       
        changeDoneState(1, "TBFCDone");

        // 填充.
        FullIt(row.No, mapExt.MyPK, objID,type);

        //填充主表数据源
        //TableFullCtrl(dataObj, objID);
        //执行个性化填充下拉框，比如填充ddl下拉框的范围.
        //FullCtrlDDL(row.No, objID, mapExt);
        //执行填充从表.
        //FullDtl(row.No, mapExt.MyPK, mapExt);

    }

    $('#TBFCtreeGrid').bootstrapTable(options);

    //加载表格数据
    loadTBFullCtrlData();

}

function isLegalName(name) {
    if (!name) {
        return false;
    }
    return name.match(/^[a-zA-Z\$_][a-zA-Z\d\$_]*$/);
}

//输入关键字进行查询
function loadTBFullCtrlData() {
    var UserNo = GetQueryString("UserNo");
    var RefPKVal = GetQueryString("RefPKVal");
    var keyWord = $("#TB_TBFC_Key").val();
    var dataObj = GenerDB(mapExt.Tag4, keyWord, mapExt.DBType, mapExt.FK_DBSrc);
    $('#TBFCtreeGrid').bootstrapTable("load",dataObj);
}
		
 