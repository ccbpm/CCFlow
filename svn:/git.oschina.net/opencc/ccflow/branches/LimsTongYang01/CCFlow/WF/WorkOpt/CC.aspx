<%@ Page Title="抄送" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WorkOpt.WF_WorkOpt_CC" Codebehind="CC.aspx.cs" %>
<%@ Register src="./../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <script language="JavaScript" src="../Comm/JScript.js" type="text/javascript"></script>
    <script  type="text/javascript">
        function ShowIt(tb, hid) {
            var url = 'SelectUser.aspx?OID=123&CtrlVal=' + hid.value;
            var v = window.showModalDialog(url, 'dfg', 'dialogHeight: 450px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
            if (v == null || v == '' || v == 'NaN') {
                return;
            }

            var arr = v.split('~');
            hid.value = arr[0];
            tb.value = arr[1];
        }
        function NoSubmit(ev) {
            if (window.event.srcElement.tagName == "TEXTAREA")
                return true;
            if (ev.keyCode == 13) {
                window.event.keyCode = 9;
                ev.keyCode = 9;
                return true;
            }
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>