<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DemoBatchUrl.aspx.cs" Inherits="CCFlow.SDKFlowDemo.BatchUrlDemo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script type="text/javascript">
        function Save() {
            alert('save ok');
            return true;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    约定一个function 名字叫Save 如果保存成功，就返回true, 保存失败就返回fase. 
    </div>
    </form>
</body>
</html>
