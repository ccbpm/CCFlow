<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowManagerEUI.aspx.cs"
    Inherits="CCFlow.WF.App.EasyUI.FlowManagerEUI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流程调度</title>
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
       
        function OpenIt(url) {
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px; dialogTop: 100px; dialogLeft: 110px; center: yes; help: no');
            if (b != null)
                loadData();
        }

        function winOpen(url) {

            window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');

        }

        //删除
           function DelIt(queryType, oid, fk_flow) {
            $("#pageloading").show();
            if (window.confirm('你确定要删除吗?') == false) {
                $("#pageloading").hide();
                return;
            }

            $.post(location.href, { 'method': 'delete', 'key': oid, 'FK_Flow': fk_flow }, function (jsonData) {
                if (jsonData == "True") {
                    $("#pageloading").hide();
                  $.messager.alert('提示','删除成功！');
                  LoadGridData(1,20);

                }
                else {
                    $.Dialog.warn(jsonData);
                    $("#pageloading").hide();
                }
            });

        }

        //加载grid后回调函数
        function LoadDataGridCallBack(js, scorp) {
            $("#pageloading").hide();
            if (js == "") js = "[]";
            //系统错误
            if (js.status && js.status == 500) {
                $("body").html("<b>访问页面出错，请联系管理员。<b>");
                return;
            }
            var pushData = eval('(' + js + ')');
            //时间格式处理
            var strTimeKey = "";
            var date = new Date();
            strTimeKey += date.getFullYear(); //年
            strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
            strTimeKey += date.getDate(); //日
            strTimeKey += date.getHours(); //HH
            strTimeKey += date.getMinutes(); //MM
            strTimeKey += date.getSeconds(); //SS
            var dateNow = date.getFullYear() + "" + date.getMonth() + 1 + "" + date.getDate();
            $('#newsGrid').datagrid({
                columns: [[
          
                     { checkbox: true },
                      { field: 'TITLE', title: '标题', width: 340, align: 'left', formatter: function (rowdata, row, index) {
                          var title = "";
                          if (rowdata.ISREAD == 0) {
                       
                              var h = "/WF/WFRpt.aspx?WorkID=" + row.WORKID + "&FK_Flow=" + row.FK_FLOW + "&FID=" + row.FID + "&T=" + dateNow;
                              title = "<a href='javascript:void(0);' onclick=winOpen('" + h + "')><img align='middle' alt='' id='" + row.OID + "' src='/WF/App/EasyUI/Img/Menu/Mail_UnRead.png' border=0 />" + row.TITLE + "</a>";
                          } else {
                              var h = "/WF/WFRpt.aspx?WorkID=" + row.WORKID + "&FK_Flow=" + row.FK_FLOW + "&FID=" + row.FID + "&T=" + dateNow;
                              title = "<a href='javascript:void(0);' onclick=winOpen('" + h + "')><img align='middle' alt='' id='" + row.OID + "' src='/WF/App/EasyUI/Img/Menu/Mail_Read.png'/>" + row.TITLE + "</a>";

                          }
                          return title;
                      } 
                      },
                       { field: 'FLOWNAME', title: '类型', align: 'left' },
                     { field: 'NODENAME', title: '当前节点',  align: 'left' },
                    { field: 'STARTERNAME', title: '发起人', align: 'center' },
                    { field: 'WFSTATE', title: '状态',  align: 'center', formatter: function (rowdata, row, index) {
                        if (row.WFSTATE == "0") {
                            return "空白";
                        } else if (row.WFSTATE == "3") {
                            return "已完成";
                        } else if (row.WFSTATE == "2") {
                            return "运行中";
                        } else if (row.WFSTATE == "1") {
                            return "草稿";
                        } else if (row.WFSTATE == "4") {
                            return "挂起";
                        } else if (row.WFSTATE == "5") {
                            return "退回";
                        } else if (row.WFSTATE == "6") {
                            return "移交";
                        } else if (row.WFSTATE == "7") {
                            return "删除(逻辑)";
                        } else if (row.WFSTATE == "8") {
                            return "加签";
                        } else if (row.WFSTATE == "9") {
                            return "冻结";
                        } else {
                            return row.WFSTATE;
                        }
                    }
                    },
                         { field: 'RDT', title: '发起日期',  align: 'center' },
                          { field: 'NODE', title: '轨迹图', formatter: function (rowdata, row, index) {
                              var op = null;
                              op = "<a href=\"javascript:OpenIt('/WF/Chart.aspx?WorkID=" + row.WORKID + "&FK_Flow=" + row.FK_FLOW + "&FID=" + row.FID + "'); \" >打开</a>";

                              return op;
                          }
                          },
                             { field: 'BILLNO', title: '报告', formatter: function (rowdata, row, index) {
                                 var op = null;
                                 op = "<a href=\"javascript:OpenIt('/WF/WFRpt.aspx?WorkID=" + row.WORKID + "&FK_Flow=" + row.FK_FLOW + "&FID=" + row.FID + "'); \" >打开</a>";

                                 return op;
                             }
                             },
                              { field: 'WORKID', title: '操作', align: 'center',formatter: function (rowdata, row, index) {
                                  var op = null;
                                  if (rowdata.WFState == "3") {
                                      op = "<a href=\"javascript:OpenIt('FlowRollBack.aspx?FK_Flow=" + row.FK_FLOW + "&WorkID=" + row.WORKID + "&FK_Node=" + row.FK_NODE + "&FID=" + row.FID + "');\">回滚</a>";
                                  }
                                  else {
                                      op = "<a href=\"javascript:DelIt('Delete','" + row.WORKID + "','" + row.FK_FLOW + "')\">删除</a> - <a href=\"javascript:OpenIt('FlowShift.aspx?FK_Flow=" + row.FK_FLOW + "&WorkID=" + row.WORKID + "&FK_Node=" + row.FK_NODE + "&FID=" + row.FID + "');\">调度</a> - <a href=\"javascript:OpenIt('FlowSkip.aspx?FK_Flow=" + row.FK_FLOW + "&WorkID=" + row.WORKID + "');\">跳转</a>";
                                  }
                                  return op;
                              }
                              }

                  ]],

                selectOnCheck: false,
                checkOnSelect: true,
                singleSelect: true,
                data: pushData,
                width: 'auto',
                height: 'auto',
                striped: true,
                rownumbers: true,
                pagination:true,
                remoteSort: false,
                fitColumns: true,
                pageNumber: scorp.pageNumber,
                pageSize: scorp.pageSize,
                pageList: [20, 30, 40, 50],
                onDblClickCell: function (index, field, value) {
                },
            
            });
            //分页
            var pg = $("#newsGrid").datagrid("getPager");
            if (pg) {
                $(pg).pagination({
                    onRefresh: function (pageNumber, pageSize) {
                        LoadGridData(pageNumber, pageSize);
                    },
                    onSelectPage: function (pageNumber, pageSize) {
                        LoadGridData(pageNumber, pageSize);
                    }
                });
            }
        }



        //加载grid
        function LoadGridData(pageNumber, pageSize) {
            this.pageNumber = pageNumber;
            this.pageSize = pageSize;
            var flow = $("#sh_FlowName").combobox('getValue');
          

             
                var startDate = $("#sh_StartDate").datebox('getValue');

            var endData = $("#sh_EndDate").datebox('getValue')
            var state = $("#sh_State").combobox('getValue');
            var title = $("#sh_Title").val();


            var key = '';
            if (flow != '') {
                key += "FK_Flow='" + flow + "'";
            }
            if (title != '') {
                if (key == '') {
                    key += encodeURI(" title like '%" + title + "%'");
                } else {
                    key += encodeURI(" and title like '%" + title + "%' ");
                }
            }


            if (startDate != '') {
                if (key == '') {
                    key += " RDT>='" + startDate + "'";
                } else {
                    key += " and RDT>='" + startDate + "'";
                }
            }

            if (endData != '') {
                if (key == '') {
                    key += " RDT<='" + endData + "'";
                } else {
                    key += " and RDT<='" + endData + "'";
                }
            }
            if (state != null) {
                if (state > 0) {
                    if (key == '') {
                        key += " WFState='" + state + "'";
                    } else {
                        key += " and WFState='" + state + "'";
                    }
                }
            }

            var params = {
                method: "worklist",
                pageNumber: pageNumber,
                pageSize: pageSize,
                key: key
            };
            queryData(params, LoadDataGridCallBack, this);
        }
   
        //初始化
        $(function () {
        $("#sh_FlowName").combobox(
                {
                    url: 'Base/DataServices.ashx?action=GetFlow',
                    valueField: 'No',
                    textField: 'Name'
                });
            LoadGridData(1, 20);
        });

        //公共方法
        function queryData(param, callback, scope, method, showErrMsg) {
            if (!method) method = 'GET';
            $.ajax({
                type: method, //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: "FlowManagerEUI.aspx", //要访问的后台地址
                data: param, //要发送的数据
                async: false,
                cache: false,
                complete: function () { }, //AJAX请求完成时隐藏loading提示
                error: function (XMLHttpRequest, errorThrown) {
                    callback(XMLHttpRequest);
                },
                success: function (msg) {//msg为返回的数据，在这里做数据绑定
                    var data = msg;
                    callback(data, scope);
                }
            });
        }


        //刷新
        function RefreshGrid() {
            var grid = $('#newsGrid');
            var options = grid.datagrid('getPager').data("pagination").options;
            var curPage = options.pageNumber;
            var pageSize = options.pageSize;
            LoadGridData(curPage, pageSize);
        }
        var commonWin = null;

        function ReFreashRoomGrid() {
            commonWin.window('close');
            RefreshGrid();
        } 
        
          

    </script>
</head>
<body>
    <div id="pageloading">
    </div>
    <div id="tb">
        <table width="90%" cellpadding="2" cellspacing="2">
            <tr style="float: left">
                <td>
                    类型:
                </td>
                <td>
                    <input type="text" id="sh_FlowName" class="easyui-combobox" />
                </td>
                <td>
                    关键字:<input type="text" id="sh_Title" class="easyui-textbox" />
                </td>
                <td>
                    发起时间:<input type="text" id="sh_StartDate" class="easyui-datebox" />
                </td>
                <td>
                    到:<input type="text" id="sh_EndDate" class="easyui-datebox" />
                </td>
                <td style="width: 35px">
                    状态:
                </td>
                <td>
                    <select name="sh_State" id="sh_State" class="easyui-combobox">
                        <option value="0">全部</option>
                        <option value="1">草稿</option>
                        <option value="2">运行中</option>
                        <option value="3">已完成</option>
                        <option value="4">挂起</option>
                        <option value="5">退回</option>
                        <option value="6">转发(移交)</option>
                        <option value="7">删除(逻辑)</option>
                        <option value="8">加签</option>
                        <option value="9">冻结</option>
                    </select>
                </td>
                <td tyle="width: 30px">
                    <input type="button" id="btnReturn" runat="server" value="查询" onclick=" LoadGridData(1, 20)" />
                </td>
            </tr>
        </table>
    </div>
    <table id="newsGrid" fit="true" fitcolumns="true" toolbar="#tb" class="easyui-datagrid">
    </table>
</body>
</html>
