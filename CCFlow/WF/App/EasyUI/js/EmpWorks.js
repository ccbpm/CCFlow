//回调函数
function GridCallBack(jsonData, scope) {
    var strTimeKey = "";
    var date = new Date();
    strTimeKey += date.getFullYear(); //年
    strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
    strTimeKey += date.getDate(); //日
    strTimeKey += date.getHours(); //HH
    strTimeKey += date.getMinutes(); //MM
    strTimeKey += date.getSeconds(); //SS
    var month = date.getMonth() + 1;
    var dateNow = date.getFullYear() + "-" + month + "-" + date.getDate();
    var pushData = eval('(' + jsonData + ')');
    var grid = $("#maingrid").datagrid({
        data: pushData,
        title: '待办列表',
        height: "auto",
        width: "auto",
        nowrap: true,
        fitColumns: true,
        autoRowHeight: false,
        singleSelect: true,
        striped: true,
        collapsible: false,
        pagination: false,
        rownumbers: true,
        columns: [[
                   { title: '标题', field: 'TITLE', width: 360, align: 'left', formatter: function (value, rec) {
                       var title = "";

                       if (rec.ISREAD == 0) {
                           title = "<a href=\"javascript:WinOpenIt('../../MyFlow.aspx?FK_Flow=" + rec.FK_FLOW + "&FK_Node=" + rec.FK_NODE
                           + "&FID=" + rec.FID + "&WorkID=" + rec.WORKID + "&AtPara=" + rec.ATPARA + "&IsRead=0&T=" + strTimeKey
                           + "','" + rec.WORKID + "','" + rec.FLOWNAME + "');\" ><img align='middle' alt='' id='" + rec.WORKID
                           + "' src='Img/Menu/Mail_UnRead.png' border=0 width=20 height=20 />" + rec.TITLE + "</a>";
                       } else {
                           title = "<a href=\"javascript:WinOpenIt('../../MyFlow.aspx?FK_Flow=" + rec.FK_FLOW + "&FK_Node=" + rec.FK_NODE
                           + "&FID=" + rec.FID + "&T=" + strTimeKey + "&WorkID=" + rec.WORKID + "&AtPara=" + rec.ATPARA + "','" + rec.WORKID
                           + "','" + rec.FLOWNAME + "');\"  ><img align='middle' border=0 width=20 height=20 id='" + rec.WORKID + "' alt='' src='Img/Menu/Mail_Read.png'/>" + rec.TITLE + "</a>";
                       }
                       return title;
                   }
                   },
                   { title: '流程名称', field: 'FLOWNAME' },
                   { title: '当前节点', field: 'NODENAME' },
                   { title: '发起人', field: 'STARTERNAME' },
                   { title: '发起日期', field: 'RDT' },
                   { title: '接受日期', field: 'ADT' },
                   { title: '应完成日期', field: 'SDT' },
                   { title: '状态', field: 'FLOWSTATE', formatter: function (value, rec) {
                       //var datePattern = /^(\d{4})-(\d{1,2})-(\d{1,2})$/;
                       if (rec.SDTOFNODE != "") {
                           var d1 = new Date();
                           var d2 = new Date(rec.SDT.replace(/-/g, "/"));
                           // if ((Date.parse(d1) > Date.parse(d2))) {

                           if (d1 > d2) {
                               return "<font color=red>逾期</font>";
                           }
                           return "正常";
                       }
                   }
                   }
                   ]]
    });
    $("#pageloading").hide();
}
//加载待办列表
function LoadGrid() {
    Application.data.getEmpWorks(GridCallBack, this);
}
//打开窗体
function WinOpenIt(url, workId, text) {
    var isReadImg = document.getElementById(workId);
    if (isReadImg) isReadImg.src = "Img/Menu/Mail_Read.png";
    if (ccflow.config.IsWinOpenEmpWorks.toUpperCase() == "TRUE") {
        var winWidth = 850;
        var winHeight = 680;
        if (screen && screen.availWidth) {
            winWidth = screen.availWidth;
            winHeight = screen.availHeight - 36;
        }
        window.open(url, "_blank", "height=" + winHeight + ",width=" + winWidth + ",top=0,left=0,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no");
    } else {
        window.parent.f_addTab(workId, text, url);
    }
}

$(function () {
    $("#pageloading").show();
    LoadGrid();
});