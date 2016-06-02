<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowManager.aspx.cs" Inherits="CCFlow.AppDemoLigerUI.FlowManager" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流程调度</title>
    <link href="/WF/App/EasyUI/jquery/tablestyle.css" rel="stylesheet" type="text/css" />
    <link href="/WF/App/EasyUI/jquery/lib/ligerUI/skins/Aqua/css/ligerui-all.css"
        rel="stylesheet" type="text/css" />
    <link href="/WF/App/EasyUI/jquery/lib/ligerUI/skins/ligerui-icons.css" rel="stylesheet"
        type="text/css" />
    <script src="/WF/App/EasyUI/jquery/lib/jquery/jquery-1.5.2.min.js" type="text/javascript"></script>
    <script src="/WF/App/EasyUI/jquery/lib/ligerUI/js/core/base.js" type="text/javascript"></script>
    <script src="/WF/App/EasyUI/jquery/lib/ligerUI/js/ligerui.all.js" type="text/javascript"></script>
    <script src="/WF/App/EasyUI/jquery/lib/ligerUI/js/plugins/ligerGrid.js" type="text/javascript"></script>
    <script src="/WF/App/EasyUI/jquery/lib/ligerUI/js/plugins/ligerDialog.js" type="text/javascript"></script>
    <script src="/WF/App/EasyUI/jquery/lib/ligerUI/js/plugins/ligerButton.js" type="text/javascript"></script>
    <script src="/WF/App/EasyUI/jquery/lib/ligerUI/js/plugins/ligerTextBox.js" type="text/javascript"></script>
    <%-- <script src="../AppDemoLigerUI/js/QiYeList.js" type="text/javascript"></script>
    --%>
    <script type="text/javascript">
        function OpenIt(url) {
            //            window.open(url, 'card', 'width=700,top=50,left=50,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false');
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px; dialogTop: 100px; dialogLeft: 110px; center: yes; help: no');
            if (b != null)
                loadData();
        }

        function DelIt(queryType, oid, fk_flow) {
            $("#pageloading").show();
            if (window.confirm('你确定要删除吗?') == false) {
                $("#pageloading").hide();
                return;
            }

            $.post(location.href, { 'action': 'Delete', 'key': oid, 'FK_Flow': fk_flow }, function (jsonData) {
                if (jsonData == "True") {
                    $("#pageloading").hide();
                    $.ligerDialog.success("删除成功!");
                    loadData();
                }
                else {
                    $.ligerDialog.warn(jsonData);
                    $("#pageloading").hide();
                }
            });

        }

        $(function () {
            initSearch();
            loadData();

        

        });
        //初始化查询
        function initSearch() {
            $("#sh_FlowName").ligerComboBox(
                {
                    autocomplete: true,
                    url: 'Base/DataServices.ashx?action=GetFlow',
                    valueField: 'No',
                    textField: 'Name',
                    selectBoxWidth: 120,
                    width: 120,
                    cancelable: true,
                    label: '类型:'
                }
            );
            $("#sh_State").ligerComboBox(
                {
                    autocomplete: true,
                    selectBoxWidth: 120,
                    width: 120,
                    cancelable: true
                }
            );


            var d = new Date();
            var year = d.getFullYear();
            var month = d.getMonth() + 1; // 记得当前月是要+1的
            var today = year + "-" + month + "-01";

            $("#sh_StartDate").ligerDateEditor({ format: "yyyy-MM-dd", width: 120,  label: '发起时间', labelWidth: 65 });
            $("#sh_EndDate").ligerDateEditor({ format: "yyyy-MM-dd", width: 120, label: '到' });
        }
        //加载数据
        function loadData() {
            $("#pageloading").show();
            var strTimeKey = "";
            var date = new Date();
            strTimeKey += date.getFullYear(); //年
            strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
            strTimeKey += date.getDate(); //日
            strTimeKey += date.getHours(); //HH
            strTimeKey += date.getMinutes(); //MM
            strTimeKey += date.getSeconds(); //SS
            var dateNow = date.getFullYear()  +""+ date.getMonth() + 1 +""+ date.getDate();

            var flow = $("#sh_FlowName").ligerComboBox().getValue();
             
            var startDate = $("#sh_StartDate").val();
            var endData = $("#sh_EndDate").val();
            var state = $("#sh_State").val();
            var title = $("#sh_Title").val();
            var param = '';
            if (flow != '') {
                param += "FK_Flow='" + flow + "'";
            }
            if (title != '') {
                if (param == '') {
                    param += encodeURI(" title like '%" + title + "%'");
                } else {
                    param += encodeURI(" and title like '%" + title + "%' ");
                }

            }


            if (startDate != '') {
                if (param == '') {
                    param += " RDT>='" + startDate + "'";
                } else {
                    param += " and RDT>='" + startDate + "'";
                }
            }

            if (endData != '') {
                if (param == '') {
                    param += " RDT<='" + endData + "'";
                } else {
                    param += " and RDT<='" + endData + "'";
                }
            }
            if (state != null) {
                if (state > 0) {
                    if (param == '') {
                        param += " WFState='" + state + "'";
                    } else {
                        param += " and WFState='" + state + "'";
                    }
                }
            }
         


            $.post(location.href, { 'action': 'loadData', 'key': param }, function (jsonData) {
                if (jsonData) {
                    var pushData = eval('(' + jsonData + ')');
                    var grid = $("#maingrid").ligerGrid({
                        columns: [
                      { display: '标题', name: 'TITLE', width: 340, align: 'left', render: function (rowdata, rowindex) {
                          var title = "";
                          if (rowdata.ISREAD == 0) {
                              //                                           title = "<a href=\"javascript:WinOpenIt('../WF/MyFlowSmall.aspx?FK_Flow=" + rowdata.FK_Flow + "&FK_Node=" + rowdata.FlowEndNode
                              //                           + "&FID=" + rowdata.FID + "&WorkID=" + rowdata.OID + "&IsRead=0&T=" + strTimeKey
                              //                           + "','" + rowdata.OID + "','" + rowdata.FlowName + "');\" ><img align='middle' alt='' id='" + rowdata.OID
                              //                           + "' src='Img/Menu/Mail_UnRead.png'/>" + rowdata.Title + "</a>";


                              var h = "/WF/WFRpt.aspx?WorkID=" + rowdata.WORKID + "&FK_Flow=" + rowdata.FK_FLOW + "&FID=" + rowdata.FID + "&T=" + dateNow;
                              title = "<a href='javascript:void(0);' onclick=winOpen('" + h + "')><img align='middle' alt='' id='" + rowdata.OID + "' src='/WF/App/EasyUI/Img/Menu/Mail_UnRead.png' border=0 />" + rowdata.TITLE + "</a>";
                          } else {
                              //                                           title = "<a href=\"javascript:WinOpenIt('../WF/MyFlowSmall.aspx?FK_Flow=" + rowdata.FK_Flow + "&FK_Node=" + rowdata.FlowEndNode
                              //                           + "&FID=" + rowdata.FID + "&T=" + strTimeKey + "&WorkID=" + rowdata.OID + "','" + rowdata.OID
                              //                           + "','" + rowdata.FlowName + "');\"  ><img align='middle' id='" + rowdata.OID + "' alt='' src='Img/Menu/Mail_Read.png'/>" + rowdata.Title + "</a>";
                              var h = "/WF/WFRpt.aspx?WorkID=" + rowdata.WORKID + "&FK_Flow=" + rowdata.FK_FLOW + "&FID=" + rowdata.FID + "&T=" + dateNow;
                              title = "<a href='javascript:void(0);' onclick=winOpen('" + h + "')><img align='middle' alt='' id='" + rowdata.OID + "' src='/WF/App/EasyUI/Img/Menu/Mail_Read.png'/>" + rowdata.TITLE + "</a>";

                          }
                          return title;
                      }
                      }, { display: '类型', name: 'FLOWNAME' },
                   { display: '当前节点', name: 'NODENAME' },
                   { display: '发起人', name: 'STARTERNAME' },
                            { display: '状态', name: 'WFSTATE', render: function (rowdata, rowindex) {
                                if (rowdata.WFSTATE == "0") {
                                    return "空白";
                                } else if (rowdata.WFSTATE == "3") {
                                    return "已完成";
                                } else if (rowdata.WFSTATE == "2") {
                                    return "运行中";
                                } else if (rowdata.WFSTATE == "1") {
                                    return "草稿";
                                } else if (rowdata.WFSTATE == "4") {
                                    return "挂起";
                                } else if (rowdata.WFSTATE == "5") {
                                    return "退回";
                                } else if (rowdata.WFSTATE == "6") {
                                    return "移交";
                                } else if (rowdata.WFSTATE == "7") {
                                    return "删除(逻辑)";
                                } else if (rowdata.WFSTATE == "8") {
                                    return "加签";
                                } else if (rowdata.WFSTATE == "9") {
                                    return "冻结";
                                } else {
                                    return rowdata.WFSTATE;

                                }
                            }
                            },
                   { display: '发起日期', name: 'RDT' },
                   { display: '轨迹图', name: 'NODE', render: function (rowdata, rowindex) {
                       var op = null;
                       op = "<a href=\"javascript:OpenIt('../WF/WorkOpt/OneWork/ChartTrack.aspx?WorkID=" + rowdata.WORKID + "&FK_Flow=" + rowdata.FK_FLOW + "&FID=" + rowdata.FID + "'); \" >打开</a>";

                       return op;
                   }
                   },
               { display: '报告', name: 'BILLNO', render: function (rowdata, rowindex) {
                   var op = null;
                   op = "<a href=\"javascript:OpenIt('../WF/WFRpt.aspx?WorkID=" + rowdata.WORKID + "&FK_Flow=" + rowdata.FK_FLOW + "&FID=" + rowdata.FID + "'); \" >打开</a>";

                   return op;
               }
               },
                { display: '操作', name: 'WORKID', render: function (rowdata, rowindex) {
                    var op = null;
					 if (rowdata.WFState == "3")
                        op = "<a href=\"javascript:OpenIt('FlowRollBack.aspx?FK_Flow=" + rowdata.FK_FLOW + "&WorkID=" + rowdata.WORKID + "&FK_Node=" + rowdata.FK_NODE + "&FID=" + rowdata.FID + "');\">回滚</a>";

                    else
                    op = "<a href=\"javascript:DelIt('Delete','" + rowdata.WORKID + "','" + rowdata.FK_FLOW + "')\">删除</a> - <a href=\"javascript:OpenIt('FlowShift.aspx?FK_Flow=" + rowdata.FK_FLOW + "&WorkID=" + rowdata.WORKID + "&FK_Node="+rowdata.FK_NODE+"&FID="+rowdata.FID+"');\">调度</a> - <a href=\"javascript:OpenIt('FlowSkip.aspx?FK_Flow=" + rowdata.FK_FLOW + "&WorkID=" + rowdata.WORKID + "');\">跳转</a>";

                    return op;
                }
                }
                   ],

                        pageSize: 20,
                        data: pushData,
                        rownumbers: true,
                        height: "99%",
                        width: "99%",
                        columnWidth: 120,
                        onDblClickRow: function (rowdata, rowindex) {

                        }
                    });
                    $("#pageloading").hide();
                }
                else {
                    $.ligerDialog.warn('加载数据出错，请关闭后重试！');
                }
            });
        }

        function winOpen(url) {
            // window.showModalDialog(url, '_blank', 'dialogHeight: 500px; dialogWidth: 850px;center: yes; help: no;scroll:no');
            window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="pageloading">
    </div>
    <div id="search">
        <table width="90%" cellpadding="0" cellspacing="0">
            <tr>
                 <td >
                    类型:
                </td>
                 <td>
                     <input type="text" id="sh_FlowName" />
                 </td>
                <td>
                    关键字:<input type="text" id="sh_Title" />
                </td>
                <td>
                    <input type="text" id="sh_StartDate" />
                </td>
                <td style="width: 125px">
                    <input type="text" id="sh_EndDate" />
                </td>
                <td style="width: 35px">
                    状态:
                </td>
                <td style="width: 75px">
                    <select name="sh_State" id="sh_State">
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
                <td style="width: 30px">
                    <input type="button" id="btnReturn" runat="server" value="查询" onclick="loadData()" />
                </td>
            </tr>
        </table>
    </div>
    <div id="maingrid" style="margin: 0; padding: 0">
    </div>
    <%--<div id="maingrid" style="margin: 0; padding: 0">
    </div>
    <div style="display: none;">
    </div>--%>
    </form>
    <%--关键子:<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
&nbsp;<asp:Button ID="Btn_Search" runat="server" onclick="Btn_Search_Click"
        Text="查询" />
&nbsp;

<table>
<tr>
<th>序号</th>
<th>编号</th>
<th>企业名称</th>
<th>操作</th>
</tr>

 <asp:DataList ID="dtNews" runat="server" Height="148px" Width="692px">
                                            <ItemTemplate>
                                                <table border="0" cellpadding="0" cellspacing="0" class="frmNewTable">
                                                    <tr>
                                                        <td>
                                                        </td>
                                                        <td>
                                                             <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("no") %>'></asp:Label></a>
                                                        </td>
                                                        <td>
                                                            <a href='QiYeInfo.aspx?QYBH=<%# Eval("no") %>'>
                                                            <asp:Label ID="lblDate" runat="server" Text='<%# Eval("name") %>'></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="line_01" colspan="3">
                                                        </td>
                                                    </tr>
                                                </table>
                                                </div>
                                            </ItemTemplate>
                                        </asp:DataList>
</tr>

<%--<%
    int idx = 0;
    foreach (System.Data.DataRow dr in dt.Rows)
    {
        idx++;
        string qybh = dr["No"].ToString();
        %><TR><%="<TD>" + idx + "</TD><TD>" + dr["No"] + "</TD><TD><a href='QiYeInfo.aspx?QYBH=" + qybh + "'>" + dr["Name"] + "</a></TD>"
        %></TR><%
    }
</table>--%>
</body>
</html>
