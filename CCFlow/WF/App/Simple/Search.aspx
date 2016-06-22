<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="CCFlow.WF.App.Simple.Search" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流程查询</title>

    <%--<link href="../../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
--%>
    <script type="text/javascript">
        function WinOpen(url) {
            var newWindow = window.open(url, 'z',
             'scroll:1;status:1;help:1;resizable:1;dialogWidth:680px;dialogHeight:420px');
            newWindow.focus();
            return;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <table width='90%' align=left >
    <caption>
    <div style='float:left'><img src='/WF/Img/Search.gif' />流程查询/分析</div>
    <div style='float:right'><a href="javascript:WinOpen('/WF/KeySearch.aspx',900,900);">关键字查询</a>|<a href="javascript:WinOpen('/WF/Comm/Search.aspx?EnsName=BP.WF.Data.GenerWorkFlowViews',900,900); ">综合查询</a>|<a href="javascript:WinOpen('/WF/Comm/Group.aspx?EnsName=BP.WF.Data.GenerWorkFlowViews',900,900); ">综合分析</a>|<a href="javascript:WinOpen('/WF/Comm/Search.aspx?EnsName=BP.WF.WorkFlowDeleteLogs');">删除日志</a></div>
    </caption>
    <tr>
    <th>序</th>
    <th>流程名称</th>
    <th>流程查询-分析</th>
    </tr>
<%
            int colspan = 5;
            string sql = "";
            BP.WF.Flows fls = new BP.WF.Flows();
            fls.RetrieveAll();
            BP.WF.Template.FlowSorts fss = new BP.WF.Template.FlowSorts();
            fss.RetrieveAll();
            int idx = 0;
            int gIdx = 0;
            foreach (BP.WF.Template.FlowSort fs in fss)
            {
                if (fs.ParentNo == "0" 
                    || fs.ParentNo=="" 
                    || fs.No=="0" )
                    continue;
                gIdx++;
            %>
            <tr>
            <th colspan=5><%=fs.Name %></th>
            </tr>
            <%
                foreach (BP.WF.Flow fl in fls)
                {
                    if (fl.FK_FlowSort != fs.No)
                        continue;
                    idx++;
                        %>
                        <tr>
                         <td class="Idx"> <%=idx %></td> 
                         <td ><%=fl.Name %></td> 

                         <td>
                    <%
                    string src2 = BP.WF.Glo.CCFlowAppPath + "WF/Rpt/Search.aspx?RptNo=ND" + int.Parse(fl.No) + "MyRpt&FK_Flow=" + fl.No;
                    
                    %>
                    <a href="javascript:WinOpen('<%=src2%>');" >查询</a>
                    <%    src2 = BP.WF.Glo.CCFlowAppPath + "WF/Rpt/Group.aspx?FK_Flow=" + fl.No + "&DoType=Dept";
                    %>
                     - <a href="javascript:WinOpen('<%=src2 %>');" >分析</a>
                     </td>
                     </tr>
                   <%
                }
            }
            %>
    </table>
    </form>
</body>
</html>
