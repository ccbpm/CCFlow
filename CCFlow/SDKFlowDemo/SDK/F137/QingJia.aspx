<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QingJia.aspx.cs" Inherits="CCFlow.SDKFlowDemo.SDK.F137.QingJia" %>

<%@ Register src="../../../WF/SDKComponents/FrmCheck.ascx" tagname="FrmCheck" tagprefix="uc1" %>
<%@ Register src="../../../WF/SDKComponents/DocMultiAth.ascx" tagname="DocMultiAth" tagprefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    
<!-- 重写toolbar 预留的send 与save 两个方法。-->
<script type="text/javascript">

    function Save() {
        alert('这是调用 嵌入式表单 的 Save 方法激活的的存盘。');
        return true;
    }
</script>
    
</head>
<body>
    <form id="form1" runat="server">
    <%
        string fk_flow = this.Request.QueryString["FK_Flow"];
        string fk_node = this.Request.QueryString["FK_Node"];
        string workID = this.Request.QueryString["WorkID"];
        string fid = this.Request.QueryString["FID"];
      %>
    <div>


    <table>
    <tr>
    <td>请假人 </td>
    <td> </td>

    <td>所在部门 </td>
    <td> </td>
    </tr>

        <td>日期从 </td>
    <td> </td>

    <td>到 </td>
    <td> </td>
    </tr>

    </table>

        <uc1:FrmCheck ID="FrmCheck1" runat="server" />
    </div>

    <% if (fk_node == "102")
       {
           %>


           <%
       } %>
    
    <uc2:DocMultiAth ID="DocMultiAth1" runat="server" />
    
    </form>
</body>
</html>
