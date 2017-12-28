<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TruakUC.ascx.cs" Inherits="CCFlow.WF.WorkOpt.OneWork.TruakUC" %>
<%@ Register src="../../UC/UCEn.ascx" tagname="UCEn" tagprefix="uc1" %>
<script  type="text/javascript">
    function ReinitIframe(frmID, tdID) {
        try {

            var iframe = document.getElementById(frmID);
            var tdF = document.getElementById(tdID);
            iframe.height = iframe.contentWindow.document.body.scrollHeight;
            iframe.width = iframe.contentWindow.document.body.scrollWidth;
            if (tdF.width < iframe.width) {
                tdF.width = iframe.width;
            } else {
                iframe.width = tdF.width;
            }

            tdF.height = iframe.height;
            return;

        } catch (ex) {

            return;
        }
        return;
    }
</script>
<style type="text/css">
.ActionType
{
    width:16px;
    height:16px;
}
</style>
<uc1:UCEn ID="UCEn1" runat="server" />
