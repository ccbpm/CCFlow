<%@ Page Title="加签" Language="C#" MasterPageFile="../SDKComponents/Site.Master" AutoEventWireup="true"
    CodeBehind="Askfor.aspx.cs" Inherits="CCFlow.WF.WorkOpt.Askfor" %>
<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

        function ShowIt(tb, hid) {
         
            var url = 'SelectUser.aspx?OID=123&CtrlVal=' + hid.value;
            var v = window.showModalDialog(url, 'dfg', 'dialogHeight: 450px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
            if (v == null || v == '' || v == 'NaN') {
                return;
            }

            var arr = v.split('~');
            var emparr = arr[0].split(',');
            hid.value = arr[0];

            if (emparr.length > 1) {
                alert('输入的加签人（' + arr[1] + '）不正确，加签人只能选择一个，请重新选择！');
                return;
            }

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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 <table style=" text-align:left; width:100%; border:0px;">
<caption>您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %></caption>
<tr>
<td valign=top style="text-align:center">
        <uc1:Pub ID="Pub2" runat="server" />
 </td>
</tr>
</table>
</asp:Content>