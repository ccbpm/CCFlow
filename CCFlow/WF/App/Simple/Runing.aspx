<%@ Page Title="在途列表" Language="C#" MasterPageFile="SiteMenu.Master" AutoEventWireup="true"
    CodeBehind="Runing.aspx.cs" Inherits="CCFlow.App.Runing" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        // 撤销。
        function UnSend(appPath, fk_flow, workid) {
            if (window.confirm('您确定要撤销本次发送吗？') == false)
                return;
            var url = appPath + 'WF/Do.aspx?DoType=UnSend&WorkID=' + workid + '&FK_Flow=' + fk_flow;
            window.location.href = url;
            return;
        }
        function Press(appPath, fk_flow, workid) {
            var url = appPath + 'WF/WorkOpt/Press.htm?WorkID=' + workid + '&FK_Flow=' + fk_flow;
            var v = window.showModalDialog(url, 'sd', 'dialogHeight: 220px; dialogWidth: 430px;center: yes; help: no');
        }
        function CopyAndStart(appPath, fk_flow, CopyFormNode, CopyFormWorkID) {
            var url = appPath + 'WF/MyFlow.aspx?CopyFormWorkID=' + CopyFormWorkID + '&CopyFormNode=' + CopyFormNode + '&FK_Flow=' + fk_flow;
            var v = window.open(url, 'sd', 'dialogHeight: 220px; dialogWidth: 430px;center: yes; help: no');
        }

        function WinOpenCC(ccid, fk_flow, fk_node, workid, fid, sta) {
            var url = '';
            if (sta == '0') {
                url = 'WF/Do.aspx?DoType=DoOpenCC&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node + '&WorkID=' + workid + '&FID=' + fid + '&Sta=' + sta + '&MyPK=' + ccid + "&T=" + dateNow;
            }
            else {
                url = 'WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Track&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node + '&WorkID=' + workid + '&FID=' + fid + '&Sta=' + sta + '&MyPK=' + ccid + "&T=" + dateNow;
            }
            //window.parent.f_addTab("cc" + fk_flow + workid, "抄送" + fk_flow + workid, url);
            var newWindow = window.open(url, 'z');
            newWindow.focus();
        }
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <%
        //获取在途工作。
        System.Data.DataTable dt = null;
            if (BP.Sys.SystemConfig.AppSettings["IsAddCC"] == "1")
                dt = BP.WF.Dev2Interface.DB_GenerRuning();
            else
                dt = BP.WF.Dev2Interface.DB_GenerRuningAndCC();
        
        string path = this.Request.ApplicationPath;
        // 输出结果
    %>
    <table style="width:100%; border:1px">
    <caption>在途 (<%=BP.WF.Dev2Interface.Todolist_Runing %>)</caption>
        <tr>
            <th>序</th>
            <th>标题</th>
            <th>流程</th>
            <th>发起时间</th>
            <th>发起人</th>
            <th>停留节点</th>
            <th>当前处理人</th>
            <th>操作</th>
        </tr>
        <%
            int idx = 0;
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                string workid = dr["WorkID"].ToString();
                string fk_flow = dr["FK_Flow"].ToString();
                string nodeID = dr["FK_Node"].ToString();
                idx++;
        %>
        <tr>
        <td class="Idx"><%= idx%></td>

            <td>
                <% if ( 1==1 || dr["Type"] + "" == "RUNNING")
                   { %>
                <a href="../../WFRpt.aspx?FK_Flow=<%= dr["FK_Flow"] %>&WorkID=<%= dr["WorkID"] %>"
                    target="_blank">
                    <%= dr["Title"] %>
                </a>
                <% }
                   else
                   { %>
                <a href='javascript:WinOpenCC("<%=dr["MyPk"] %> "," <%=dr["FK_Flow"] %>  ","<%=dr["FK_Node"] %> "," <%=dr["WorkID"] %> ","<%=dr["FID"] %>","<%=dr["Sta"] %>")'>
                    <%=dr["Title"] %></a>
                <% } %>
            </td>

            <td nowarp=true><%= dr["FlowName"] %></td>
            <td nowarp=true><%= dr["RDT"] %></td>
            <td nowarp=true><%= dr["StarterName"] %></td>
            <td nowarp=true><%= dr["NodeName"] %></td>
            <td nowarp=true><%= dr["TodoEmps"] %></td>
            <% if (1 == 1 ||  dr["Type"] + "" == "RUNNING")
               { %>
            <td nowarp=true>
               <% if (dr["FID"].ToString() != "0")
                  { %>
                [<a href="../../MyFlowInfo.aspx?FK_Flow=<%= fk_flow %>&WorkID=<%= workid %>&DoType=DeleteFlow" target=_blank >删除</a>]
                <% }
                  else
                  { %>

                 [<a href="../../WorkOpt/UnSend.aspx?FK_Flow=<%= fk_flow %>&WorkID=<%= workid %>" target=_blank >撤销发送</a>]
                -[<a href="javascript:CopyAndStart('<%= path %>','<%= fk_flow %>','<%= nodeID %>','<%= workid %>')" >Copy发起</a>]
                <%} %>
                -[<a href="javascript:Press('<%= path %>','<%= fk_flow %>','<%= workid %>')" >催办</a>]

            </td>
            <% } else  {%>
            <td>  </td>
            <% } %>
        </tr>
        <% } %>
    </table>
</asp:Content>
