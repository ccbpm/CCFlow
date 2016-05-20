<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowDB.aspx.cs" Inherits="CCFlow.App.FlowDB.FlowDB" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Src="/WF/Admin/Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流程数据管理</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="../css/cssTab.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript">


        //工作删除.
        function DelIt(fk_flow, workid) {
            if (window.confirm('Are you sure?') == false)
                return;
            var url = 'FlowDB.aspx?DoType=DelIt&FK_Flow=' + fk_flow + '&WorkID=' + workid;
            WinShowModalDialog(url, '');
            window.location.href = window.location.href;
        }

        //流程移交.
        function FlowShift(fk_flow, workid) {

            var url = 'FlowShift.aspx?FK_Flow=' + fk_flow + '&WorkID=' + workid;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px; dialogTop: 100px; dialogLeft: 110px; center: yes; help: no');
            if (b != null)
                window.location.href = window.location.href;
        }

        //跳转.
        function FlowSkip(fk_flow, workid) {
            var url = 'FlowSkip.aspx?FK_Flow=' + fk_flow + '&WorkID=' + workid;
            var v = window.showModalDialog(url, 'sd', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
            if (v != null)
                window.location.href = window.location.href;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc1:Pub ID="Pub1" runat="server" />
    </div>
    </form>
</body>
</html>
