<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubFlowDtl.ascx.cs" Inherits="CCFlow.WF.SDKComponents.SubFlowDtl" %>
<script type="text/javascript">
    function Del(fk_flow, workid) {
        if (window.confirm('您确定要删除吗？') == false)
            return;
    }
    function OpenIt(url) {
        var newWindow = window.open(url, 'card', 'width=700,top=50,left=50,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false');
        newWindow.focus();
        return;
    }
</script>
<div id="DelMsg" />