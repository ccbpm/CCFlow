<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StartGuideSelfUrl.aspx.cs" Inherits="CCFlow.SDKFlowDemo.TestCase.StartGuideSelfUrl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <%
        BP.WF.Port.WFEmps emps = new BP.WF.Port.WFEmps();
        emps.RetrieveAll();
      %>

      <table style="border:1px;">
      <caption> 发起前置导航自定义URL测试案例</caption>
      <tr>
      <th>序</th>
      <th>名称</th>
      <th>电话 </th>
      <th>地址 </th>
      <th>邮件 </th>
      </tr>
      <%
          int idx = 0;
          foreach (BP.WF.Port.WFEmp item in emps)
          {
              idx++;

              //注意必须有FK_Flow 与 IsCheckGuide=1 的参数.
              string url = "../../WF/MyFlow.aspx?FK_Flow=163&IsCheckGuide=1&Tel=" + item.Tel + "&Email=" + item.Email + "&EmpNo=" + item.No + "&EmpName=" + item.Name;
      %>
      <tr>
      <td><%=idx %></td>
      <td> <a href="<%=url%>" > <%=item.Name %> </a> </td>
      <td><%=item.Tel%> </td>
      <td><%=item.Author%> </td>
      <td><%=item.Email%> </td>
      </tr>
      <%} %>
      </table>

    </div>
    </form>
</body>
</html>
