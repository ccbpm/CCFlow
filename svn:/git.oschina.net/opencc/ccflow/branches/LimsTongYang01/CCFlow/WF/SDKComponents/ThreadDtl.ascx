<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ThreadDtl.ascx.cs" Inherits="CCFlow.WF.SDKComponents.ThreadDtl" %>
<script type="text/javascript" language="javascript">
    function DoDelSubFlow(fk_flow, workid) {
        if (window.confirm('您确定要终止进程吗？') == false)
            return;
        var url = '/WF/Do.aspx?DoType=DelSubFlow&FK_Flow=' + fk_flow + '&WorkID=' + workid;
        WinShowModalDialogT(url, 's');
        window.location.href = window.location.href;  
    }
    function WinOpen(url, winName) {
        var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
        newWindow.focus();
        return;
    }
    function WinShowModalDialogT(url, winName) {
        var v = window.showModalDialog(url, winName, 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
        return;
    }
 </script>