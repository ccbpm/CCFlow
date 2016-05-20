<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccepterCCerSelecter.ascx.cs" Inherits="CCFlow.WF.SDKComponents.AccepterCCerSelecter" %>

<script type="text/javascript">
    function Select(flowNo, nodeid, workid, fid) {
        var url = '/WF/WorkOpt/AccepterAdv.aspx?FK_Flow=' + flowNo + '&FK_Node=' + nodeid + '&WorkID=' + workid + '&FID=' + fid;
        var str = window.showModalDialog(url, '', 'dialogHeight: 600px; dialogWidth:800px; dialogTop: 50px; dialogLeft: 150px; center: no; help: no');
        if (str == undefined)
            return;
    }
</script>


