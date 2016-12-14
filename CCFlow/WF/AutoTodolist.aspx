<%@ Page Title="授权人待办" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" CodeBehind="AutoTodolist.aspx.cs" Inherits="CCFlow.WF.AuthTodolist" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script type="text/javascript">

    function LogAs(fk_emp) {
        if (window.confirm('您确定要以[' + fk_emp + ']授权方式登陆处理工作吗？') == false)
            return;

        var url = 'Do.aspx?DoType=LogAs&FK_Emp=' + fk_emp;
        WinShowModalDialog(url, '');
        alert('登陆成功，现在您可以以[' + fk_emp + ']处理工作。');
        window.location.href = 'EmpWorks.aspx';
    }

</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<%
    if (BP.Web.WebUser.IsAuthorize == true)
    {
        %>
        <h2>当前已经是授权状态，您不能查看授权人的被授权数据。</h2>
        <%
        return;
    }

    BP.WF.Port.WFEmps ems = new BP.WF.Port.WFEmps();
    ems.Retrieve(BP.WF.Port.WFEmpAttr.Author, BP.Web.WebUser.No);
            
    if (ems.Count == 0)
    {
        BP.WF.Glo.ToMsg("没有人授权给您，授权待办工作为空，<a href='EmpWorks.aspx'>点击这里返回我的待办</a>。");
          %>
          
        <h2></h2>
        <%
            return;
    }
%>
              <table style="width:99%"> 
                <caption ><div class=CaptionMsg>授权待办</div></caption>
                <tr>
                <th>序 </th>
                <th>授权人 </th>
                <th>标题 </th>
                <th>流程 </th>
                <th>停留节点 </th>
                <th>发送时间</th>
                <th>授权人</th>
                </tr>
<%
    int idx = 0;
    foreach (BP.WF.Port.WFEmp em in ems)
    {
        System.Data.DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(em.No, null);
        foreach (System.Data.DataRow dr in dt.Rows)
        {
%>
<tr>
<td class="Idx" ><%=idx++ %></td>
<td><%=em.Name%></td>

<td> <a href="javascript:LogAs('<%=em.No %>')" > <%=dr["Title"] %> </a></td>
<td><%=dr["FlowName"]%></td>
<td><%=dr["NodeName"]%></td>
<td><%=BP.DA.DataType.ParseSysDate2DateTimeFriendly(dr["ADT"].ToString())%></td>
<td><%=em.Name%> </td>
</tr>

<%
        }
%>
<%
    }
%>

                </table>

</asp:Content>
