<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true"
    Inherits="CCFlow.WF.WF_BatchSmall" CodeBehind="Batch.aspx.cs" %>

<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
<script src="./Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
<script language="JavaScript" src="./Comm/JScript.js" type="text/javascript"></script>
<script language="JavaScript" src="./Comm/JS/Calendar/WdatePicker.js" defer="defer"
    type="text/javascript"></script>
    <script type="text/javascript">
        function SelectAll() {
            var arrObj = document.all;
            if (document.forms[0].checkedAll.checked) {
                for (var i = 0; i < arrObj.length; i++) {
                    if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                        arrObj[i].checked = true;
                    }
                }
            } else {
                for (var i = 0; i < arrObj.length; i++) {
                    if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox')
                        arrObj[i].checked = false;
                }
            }
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
        $(function () {
            $('#ContentPlaceHolder1_Batch1_Btn_Group').hide();
        });
        function BatchGroup() {
            var btn = document.getElementById("ContentPlaceHolder1_Batch1_Btn_Group");
            if (btn) {
                btn.click();
            }
        }
    </script>
    <style type="text/css">
        .l-link
        {
            text-decoration: none;
        }
        .l-link:hover
        {
            text-decoration: underline;
        }
    </style>
    <link href="../DataUser/Style/ccbpm.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table width="100%">
    <uc1:Pub ID="Pub1" runat="server" />
    </table>
</asp:Content>
