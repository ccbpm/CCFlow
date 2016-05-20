<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BatchSelfFrm.aspx.cs" Inherits="CCFlow.SDKFlowDemo.TestCase.BatchSelfFrm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script type="text/javascript">

        function Save() {
            alert('已经触发了 嵌入式表单 的Save方法');
            return true;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>

    <fieldset>
    <legend> 自定义批处理表单</legend>

    <ul>
    <li>开发人员可以自定义一个批量处理的保存的表单。</li>
    <li>该表单必须有javascript  Save方法，如果保存成功就返回true, 否则返回false 就阻止用户向下发送。</li>
    <li>当前的URL:<font color=red><b> <%=this.Request.RawUrl %> </b></font></li>
    </ul>
    </fieldset>


    </div>
    </form>
</body>
</html>
