<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Left.aspx.cs" Inherits="CCFlow.WF.App.Simple.Left" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
<base target="main" />
</head>
<body>
    <form id="form1" runat="server">
    <fieldset>
    <legend>流程基础功能</legend>
    <%
        if (BP.Web.WebUser.No == "Guest")
            BP.WF.Dev2Interface.Port_SigOut();

        if (BP.Web.WebUser.No == null)
            return;
           %>
  <ul>
  <li><a href="Start.aspx">发起</a></li>
  <li><a href="Todolist.aspx">待办(<%=BP.WF.Dev2Interface.Todolist_EmpWorks%>)</a></li>
  <li><a href="Runing.aspx">在途(<%=BP.WF.Dev2Interface.Todolist_Runing%>)</a></li>
  <li><a href="Search.aspx">查询</a></li>
  </ul>
  </fieldset>
  
   <fieldset>
    <legend>流程高级功能</legend>
    <ul>
  <li><a href="/WF/TaskPoolSharing.aspx">共享任务(<%=BP.WF.Dev2Interface.Todolist_Sharing%>)</a></li>
    <li><a href="CC.aspx">抄送(<%=BP.WF.Dev2Interface.Todolist_CCWorks%>)</a></li>
    <li><a href="/WF/Batch.aspx">批处理</a></li>
    <li><a href="/WF/GetTask.aspx">取回审批</a></li>
    <li><a href="/WF/Search.htm">高级查询</a></li>
    </ul>
    </fieldset>

    <% if (BP.Web.WebUser.No == "admin")
       { %>
    <fieldset>
    <legend>系统管理</legend>
    <ul>
    <li><a href="/WF/Comm/Search.aspx?EnsName=BP.Port.Depts">部门维护</a></li>
    <li><a href="/WF/Comm/Search.aspx?EnsName=BP.Port.Stations">岗位维护</a></li>
    <li><a href="/WF/Comm/Search.aspx?EnsName=BP.Port.Emps">人员维护</a></li>
    <li><a href="/WF/Comm/Sys/EnumList.aspx">枚举数据管理</a></li>
    <li><a href="/WF/Comm/Sys/SFTableList.aspx">字典表</a></li>
    </ul>
    </fieldset>

    <%} %>


     
    </form>
</body>
</html>
