<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TrackUC.ascx.cs" Inherits="CCFlow.WF.WorkOpt.OneWork.TrackUC" %>
<%@ Register Src="../../UC/UCEn.ascx" TagName="UCEn" TagPrefix="uc1" %>
<script type="text/javascript">
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
        width: 16px;
        height: 16px;
    }
</style>
<link href="css/style.css" rel="stylesheet" type="text/css" />
<uc1:UCEn ID="UCEn1" runat="server" />
<asp:HiddenField ID="HiddenField1" runat="server"></asp:HiddenField>

<script type="text/javascript">
    window.onload = function () {
        var op = $("#<%=HiddenField1.ClientID%>").val();
        $('#flowNote').append(op);

        $(".main .year .list").each(function (e, target) {
            var $target = $(target),
	        $ul = $target.find("ul");
            $target.height($ul.outerHeight()), $ul.css("position", "absolute");
        });
        $(".main .year>h2>a").click(function (e) {
            e.preventDefault();
            $(this).parents(".year").toggleClass("close");
        });
    }
</script>
