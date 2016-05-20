<%@ Page Title="" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true"
    CodeBehind="MessagesReplay.aspx.cs" Inherits="CCFlow.WF.MessagesReplay" %>

<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="Scripts/EasyUIUtility.js" type="text/javascript"></script>
    <script type="text/javascript">
        function openSelectEmp(hid, tid) {
            var url = 'Comm/Port/SelectUser_Jq.aspx';

            var selected = $('#' + hid).val();
            if (selected != null && selected.length > 0) {
                url += '?In=' + selected + '&tk=' + Math.random();
            }

            OpenEasyUiDialog(url, 'eudlgframe', '选择发起人', 760, 470, 'icon-user', true, function (ids) {
                var arr = ids.split(',');
                var hiddenId = arr[0];
                var tbId = arr[1];

                var innerWin = document.getElementById('eudlgframe').contentWindow;
                $('#' + tbId).val(innerWin.getReturnText());
                $('#' + hiddenId).val(innerWin.getReturnValue());
            }, hid + ',' + tid);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>
