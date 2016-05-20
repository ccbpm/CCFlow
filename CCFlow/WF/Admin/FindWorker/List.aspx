<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="CCFlow.WF.Admin.FindWorker.UIFindWorkerRoles" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>找人规则</title>
     <link href="../../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function Edit(sort0, fk_flow, fk_node, refOID) {
            var h = 500;
            var w = 400;
            var url = sort0 + '.aspx?FK_Flow=' + fk_flow + '&FK_Node=' + fk_node + '&RefOID=' + refOID;
          //  window.showModalDialog(url, 'N2', 'dialogHeight: ' + h + 'px; dialogWidth: ' + w + 'px; center: yes; help: no');
            //window.location.href = window.location.href;
            window.location.href = url;
        }
        function New(fk_flow, fk_node) {
            var h = 500;
            var w = 400;
            var url = 'Leader.aspx?FK_Flow=' + fk_flow + '&FK_Node=' + fk_node;
           // window.showModalDialog(url, 'N2', 'dialogHeight: ' + h + 'px; dialogWidth: ' + w + 'px; center: yes; help: no');
            //window.location.href = window.location.href;
            window.location.href = url;

        }
        function Up(oid) {
            var h = 30;
            var w = 40;
            var url = 'List.aspx?DoType=Up&RefOID=' + oid;
            window.showModalDialog(url, 'N2', 'dialogHeight: ' + h + 'px; dialogWidth: ' + w + 'px; center: yes; help: no');
            window.location.href = window.location.href;
        }
        function Down(oid) {
            var h = 30;
            var w = 40;
            var url = 'List.aspx?DoType=Down&RefOID=' + oid;
            window.showModalDialog(url, 'N2', 'dialogHeight: ' + h + 'px; dialogWidth: ' + w + 'px; center: yes; help: no');
            window.location.href = window.location.href;
        }
        function Del(oid) {
            var h = 30;
            var w = 40;
            var url = 'List.aspx?DoType=Del&RefOID=' + oid;
            window.showModalDialog(url, 'N2', 'dialogHeight: ' + h + 'px; dialogWidth: ' + w + 'px; center: yes; help: no');
            window.location.href = window.location.href;
        }
        function Enable(oid) {
            var h = 30;
            var w = 40;
            var url = 'List.aspx?DoType=Enable&RefOID=' + oid;
            window.showModalDialog(url, 'N2', 'dialogHeight: ' + h + 'px; dialogWidth: ' + w + 'px; center: yes; help: no');
            window.location.href = window.location.href;
        }
        function UnEnable(oid) {
            var h = 30;
            var w = 40;
            var url = 'List.aspx?DoType=UnEnable&RefOID=' + oid;
            window.showModalDialog(url, 'N2', 'dialogHeight: ' + h + 'px; dialogWidth: ' + w + 'px; center: yes; help: no');
            window.location.href = window.location.href;
        }
    </script>
</head>
<body leftMargin=0  topMargin=0>
    <form id="form1" runat="server">
    <div>
    
        <uc1:Pub ID="Pub1" runat="server" />
    
    </div>
    </form>
</body>
</html>
