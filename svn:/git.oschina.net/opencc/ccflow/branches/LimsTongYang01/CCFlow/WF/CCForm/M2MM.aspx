<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true"
    Inherits="CCFlow.WF.CCForm.WF_M2MM" CodeBehind="M2MM.aspx.cs" %>

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <meta http-equiv="Page-Enter" content="revealTrans(duration=0.5, transition=8)" />
    <style type="text/css">
        body
        {
            font-size: smaller;
        }
    </style>
    <script language="javascript">
        var isChange = false;
        function SaveM2M() {

            if (isChange == false)
                return;
            var btn = document.getElementById('Button1');
            btn.click();
            isChange = false;
        }
        function TROver(ctrl) {
            ctrl.style.backgroundColor = 'LightSteelBlue';
        }
        function TROut(ctrl) {
            ctrl.style.backgroundColor = 'white';
        }
        function Del(id, ens) {
            if (window.confirm('您确定要执行删除吗？') == false)
                return;
            var url = 'Do.aspx?DoType=DelDtl&OID=' + id + '&EnsName=' + ens;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
    </script>
    <style type="text/css">
        .HBtn
        {
            width: 1px;
            height: 1px;
            display: none;
        }
        
        UL
        {
            padding-left: 12px;
        }
        
        li
        {
            font-size: 16px;
        }
        
        table
        {
            border: none;
        }
        
        .Left
        {
            border: none;
            background-color: Silver;
        }
    </style>
    </style>
    <script language="JavaScript" src="../Comm/JScript.js"></script>
    <script language="JavaScript" src="../Comm/JS/Calendar.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table>
        <tr>
            <td valign="top" style='width: 30%' class="Left">
                <uc1:Pub ID="Left" runat="server" />
            </td>
            <td valign="top">
                <uc1:Pub ID="Pub1" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td colspan="1">
                <asp:Button ID="Button1" CssClass="Btn" runat="server" Text="Save" OnClick="Button1_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
