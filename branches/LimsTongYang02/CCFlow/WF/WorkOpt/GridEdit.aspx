<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridEdit.aspx.cs" Inherits="CCFlow.WF.WorkOpt.GridEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/CreateControl.js" type="text/javascript"></script>
    <script type="text/javascript">
        function OnOpenReport() {
            //设置 DefaultAction 属性为 false，不执行控件本身的默认打开行为
            ReportDesigner.DefaultAction = false;
 
            var LoadURL  =location.href+"&method=load";
            var success = ReportDesigner.Report.LoadFromURL(encodeURI(LoadURL));
            if (success == true) {
                ReportDesigner.Reload();
            }
            else {
                alert("载入报表失败!");
            }
        }

        function OnSaveReport() {
            //设置 DefaultAction 属性为 false，不执行控件本身的默认保存行为
            ReportDesigner.DefaultAction = false;

            ReportDesigner.Post();  //将设计器中的设计数据提交到报表对象

            var LoadURL = location.href+"&method=save";
            var success = ReportDesigner.Report.SaveToURL(encodeURI(LoadURL));
            if (success)
                alert("保存报表成功!");
            else
                alert("保存报表失败!");
        }
    </script>
</head>
<body style="margin:0px;height:800px">
    <script type="text/javascript">
        var file = "../../DataUser/CyclostyleFile/<%=Request.QueryString["grf"] %>";
        //修改一个报表，在完成报表设计后，将报表保存在web服务器上
        //前面两个参数分别指定模板载入与保存的URL，
        //第三个参数指定报表数据的URL，以便在设计时载入数据及时查看效果
        //这里不指定任何参数，在 OpenReport 与 SaveReport 中进行具体的参数化处理
        CreateDesignerEx("100%", "100%", file, "", "", "<param name='OnOpenReport' value='OnOpenReport'><param name='OnSaveReport' value='OnSaveReport'>");
    </script>
</body>
</html>
