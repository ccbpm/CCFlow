<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.Comm_MapDef_SFTableEditData"
    CodeBehind="SFTableEditData.aspx.cs" %>

<%@ Register Src="../UC/Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <title runat="server" />
    <script language="JavaScript" src="../../Comm/JScript.js"></script>
    <script language="javascript">
        /* ESC Key Down  */
        function Esc() {
            if (event.keyCode == 27)
                window.close();
            return true;
        }
        function Del(refno, pageidx, enpk) {
            if (window.confirm('您确定要删除字段[' + enpk + ']吗？') == false)
                return;
            var url = 'SFTableEditData.aspx?FK_SFTable=' + refno + '&PageIdx=' + pageidx + '&EnPK=' + enpk;
            window.location.href = url;
        }
        function ToUrl(url) {
            window.location.href = url;
        }
        function RSize() {

            if (document.body.scrollWidth > (window.screen.availWidth - 100)) {
                window.dialogWidth = (window.screen.availWidth - 100).toString() + "px"
            } else {
                window.dialogWidth = (document.body.scrollWidth + 50).toString() + "px"
            }

            if (document.body.scrollHeight > (window.screen.availHeight - 70)) {
                window.dialogHeight = (window.screen.availHeight - 50).toString() + "px"
            } else {
                window.dialogHeight = (document.body.scrollHeight + 115).toString() + "px"
            }
            window.dialogLeft = ((window.screen.availWidth - document.body.clientWidth) / 2).toString() + "px"
            window.dialogTop = ((window.screen.availHeight - document.body.clientHeight) / 2).toString() + "px"
        }
    </script>
    <base target="_self" />
</head>
<body onkeypress="Esc()" class="easyui-layout">
    <div data-options="region:'center',title:'<%=this.Title %>'" style="padding: 5px;">
        <form id="form1" runat="server">
        <uc1:Pub ID="Pub1" runat="server" />
        <uc1:Pub ID="Pub2" runat="server" />
        <uc1:Pub ID="Pub3" runat="server" />
        </form>
    </div>
</body>
</html>
