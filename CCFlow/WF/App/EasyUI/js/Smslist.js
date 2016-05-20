//获取从上一个url传递来的参数列表
function QueryString() {
    var name, value, i;

    var str = location.href;
    var num = str.indexOf("?")

    str = str.substr(num + 1);
    var arrtmp = str.split("&");

    for (i = 0; i < arrtmp.length; i++) {
        num = arrtmp[i].indexOf("=");
        if (num > 0) {
            name = arrtmp[i].substring(0, num);
            value = arrtmp[i].substr(num + 1);
            this[name] = value;
        }
    }
}


//弹出层  
function WinOpenIt(url) {
    $.ligerDialog.open({ height: 400, url: url, width: 450, showMax: true, showToggle: true, showMin: true, isResize: true, modal: false, title: '邮件详细信息' });
}
var pushData = "";
//回调函数
function callBack(jsonData, scope) {
    if (jsonData) {
        if (jsonData.length > 17) {
            pushData = eval('(' + jsonData + ')');
            var grid = $("#maingrid").ligerGrid({
                columns: [
                   { display: '主题', name: 'EMAILTITLE', width: 300, align: 'left', render: function (rowdata, rowindex) {
                       if (rowdata.IsRead == "1") {
                           return "<img align='middle' id='" + rowdata.MYPK + "' border=0 width='20' height='20' src='Img/Menu/Mail_Read.png'/><a  href='javascript:void(0)' onmouseover=OpenDocDiv('" + escape(rowdata.EMAILDOC) + "',event)  onmouseout=CloseDocDiv() >" + rowdata.EMAILTITLE + "</a>";
                       }
                       else {
                           return "<img align='middle' id='" + rowdata.MYPK + "' border=0 width='20' height='20' src='Img/Menu/Mail_UnRead.png'/><a  href='javascript:void(0)' onmouseover=OpenDocDiv('" + escape(rowdata.EMAILDOC) + "',event)   onmouseout=CloseDocDiv() >" + rowdata.EMAILTITLE + "</a>";
                       }
                   }
                   },
                   { display: '发件人', name: 'SENDER' },
                   { display: '发送日期', name: 'RDT', width: 150, render: function (rowdata, rowindex) {
                       var week = "";
                       if (rowdata.WeekRDT != "") {
                           switch (rowdata.WEEKRDT) {
                               case "1":
                                   week = "周一";
                                   break;
                               case "2":
                                   week = "周二";
                                   break;
                               case "3":
                                   week = "周三";
                                   break;
                               case "4":
                                   week = "周四";
                                   break;
                               case "5":
                                   week = "周五";
                                   break;
                               case "6":
                                   week = "周六";
                                   break;
                               case "7":
                                   week = "周日";
                                   break;
                           }
                       }
                       var date = new Date(Date.parse(rowdata.RDT.replace(/\-/g, "/")));
                       var month = (date.getMonth() + 1) < 10 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1);
                       var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
                       var hour = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
                       var minu = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
                       return date.getFullYear() + "/" + month + "/" + day + "(" + week + ")" + hour + ":" + minu;
                   }
                   }
                   ],
                pageSize: 20,
                data: pushData,
                rownumbers: true,
                height: "99%",
                width: "99%",
                columnWidth: 120,
                onReload: LoadGrid,
                detail: { onShowDetail: GetDetail },
                groupColumnName: 'GROUPBY',
                groupColumnDisplay: '日期'
            });
        }
    }
    else {
        $.ligerDialog.warn('无系统消息！');
    }
    $("#pageloading").hide();
}

//浮上  打开
function OpenDocDiv(row, e) {
    var x = e.clientX;
    var y = e.clientY + 10;
    document.getElementById('divDoc').style.top = y + "px";
    document.getElementById('divDoc').style.position = "absolute";
    document.getElementById('divDocContent').innerHTML = unescape(row).replace(/~/g, "'");
    document.getElementById('divDoc').style.display = 'block';
    return false;
}
// 离开 关闭
function CloseDocDiv() {
    document.getElementById('divDoc').style.display = 'none';
    return false;
}

//显示 详细信息
function GetDetail(row, detailPanel) {
    if ($('#griddetail1') != null && $('#griddetail1').length > 0) {
        $('#griddetail1').css('display', 'none');
        $('#griddetail1').remove();
    }
    var grid = document.createElement('div');
    $(".l-grid-detailpanel").css('height', 100);
    $(grid).css('margin', 20);
    grid.innerHTML = row.EMAILDOC.replace(/~/g, "'");
    $(detailPanel).append(grid);

    var isReadImg = document.getElementById(row.MYPK);
    if (isReadImg) isReadImg.src = "Img/Menu/Mail_Read.png";
    Application.data.upMsgSta(row.MYPK, upMsgSta, this);
}
//加载 收件箱 列表
function LoadGrid() {
    $("#pageloading").show();
    Application.data.popAlert(type, callBack, this);
}
var strTimeKey = "";
var type = "";
$(function () {

    strTimeKey = "";
    var date = new Date();
    strTimeKey += date.getFullYear(); //年
    strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
    strTimeKey += date.getDate(); //日
    strTimeKey += date.getHours(); //HH
    strTimeKey += date.getMinutes(); //MM
    strTimeKey += date.getSeconds(); //SS

    var Request = new QueryString();
    type = Request["type"];

    LoadGrid();
});
//修改数据状态  2013.05.23 H
function upMsgSta(my_PK, jsonData, scope) {
    if (jsonData) { }
}
