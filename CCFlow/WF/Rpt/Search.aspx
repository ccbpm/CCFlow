<%@ Page Title="查询" Language="C#" MasterPageFile="Single.Master" AutoEventWireup="true"
    Inherits="CCFlow.WF.Rpt.WF_Rpt_Search" CodeBehind="Search.aspx.cs" %>

<%@ Register Src="UC/Search.ascx" TagName="Search" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Comm/JS/Calendar/WdatePicker.js" type="text/javascript"></script>
    <script language="JavaScript" src="../Comm/JScript.js" type="text/javascript" />
    <script language="javascript" type="text/javascript">
        function ShowEn(url, wName, h, w) {
            h = 700;
            w = 900;
            var s = "dialogWidth=" + parseInt(w) + "px;dialogHeight=" + parseInt(h) + "px;resizable:yes";
            var val = window.showModalDialog(url, null, s);
            window.location.href = window.location.href;
        }
        function ImgClick() {
        }
        function OpenAttrs(ensName) {
            var url = './../../Sys/EnsAppCfg.aspx?EnsName=' + ensName;
            var s = 'dialogWidth=680px;dialogHeight=480px;status:no;center:1;resizable:yes'.toString();
            val = window.showModalDialog(url, null, s);
            window.location.href = window.location.href;
        }
        function DDL_mvals_OnChange(ctrl, ensName, attrKey) {

            var idx_Old = ctrl.selectedIndex;
            if (ctrl.options[ctrl.selectedIndex].value != 'mvals')
                return;
            if (attrKey == null)
                return;
            var timestamp = Date.parse(new Date());

            var url = 'SelectMVals.aspx?EnsName=' + ensName + '&AttrKey=' + attrKey + '&D=' + timestamp;
            var val = window.showModalDialog(url, 'dg', 'dialogHeight: 450px; dialogWidth: 450px; center: yes; help: no');
            if (val == '' || val == null) {
                ctrl.selectedIndex = 0;
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc2:Search ID="Search1" runat="server" />
</asp:Content>
