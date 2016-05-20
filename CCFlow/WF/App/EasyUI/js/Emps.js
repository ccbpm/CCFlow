
//打电话
function WinOpen(url) {
    window.showModalDialog(url, '_blank', 'height=600,width=950,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');

}

//回调函数
function callBack(jsonData, scope) {
    if (jsonData) {
        var pushData = eval('(' + jsonData + ')');
        var grid = $("#maingrid").ligerGrid({
            columns: [
                      { display: '部门', name: 'DeptName', width: 140, align: 'left' },
                      { display: '编号', name: 'No', width: 120 },
                      { display: '姓名', name: 'Name', width: 100 },
                      { display: '职位', name: 'DutyName', width: 100 },
                      { display: '直属领导', name: 'Leader', width: 100 },
                      { display: 'Tel', name: 'Tel', width: 150, align: 'center', render: function (rowdata, rowindex, value) {
                          if (rowdata.Tel == "") {
                              return "";
                          }
                          else {
                              var h = "../../Msg/SMS.aspx?Tel=" + rowdata.Tel + "&T=" + dateNow;
                              return "<a href='javascript:void(0)' onclick=WinOpen('" + h + "') ><img src='Img/Menu/mail.png' border=0  align=middle />" + rowdata.Tel + "</a>";
                          }

                      }
                      },
                      { display: 'Email', name: 'Email', width: 160, align: 'center', render: function (rowdata, rowindex, value) {
                          if (rowdata.Email == "") {
                              return "";
                          }
                          else {
                              var h = "";
                              return "<a href='Mailto:" + rowdata.Email + "'><img src='Img/Menu/tel.png' border=0 align=middle/>" + rowdata.Email + "</a>";

                          }
                      }
                      },
                        { display: '签名', name: 'QianMing', render: function (rowdata, rowindex, value) {
                            if (rowdata.QianMing == "") {
                                return "";
                            }
                            else {
                                return "<img src='../.." + rowdata.QianMing + "' border=0/>";
                            }

                        }
                        }
                   ],
            usePager: false,
            data: pushData,
            rownumbers: true,
            alternatingRow: false,
            tree: { columnName: 'DeptName' },
            columnWidth: 120,
            onReload: LoadGrid
        });
    }
    else {
        $.ligerDialog.warn('加载数据出错，请关闭后重试！');
    }
    $("#pageloading").hide();
}
//加载 通讯录  列表
function LoadGrid() {
    $("#pageloading").show();
    Application.data.getEmps(callBack, this);
}
var dateNow = "";
$(function () {

    var date = new Date();
    dateNow += date.getFullYear(); //年
    dateNow += date.getMonth() + 1; //月 月比实际月份要少1
    dateNow += date.getDate(); //日
    dateNow += date.getHours(); //HH
    dateNow += date.getMinutes(); //MM
    dateNow += date.getSeconds(); //SS

    LoadGrid();
});