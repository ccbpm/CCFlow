<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.CCForm.Comm_M2M"
    CodeBehind="M2M.aspx.cs" %>

<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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

        function GroupBarClick(gID) {
            var alt = document.getElementById('I' + gID).alert;
             if (alt==null) {
		            alt = 'Max';
		        }
		        var sta = 'block';
		        if (alt == 'Max') {
		            sta = 'block';
		            alt = 'Min';
		        } else {
		            sta = 'none';
		            alt = 'Max';
		        }
		        document.getElementById('I' + gID).src = '../Img/' + alt + '.gif';
		        document.getElementById('I' + gID).alert = alt;
		        document.getElementById('TR' + gID).style.display = sta;
        }
    </script>
    <style type="text/css">
        .HBtn
        {
            width: 1px;
            height: 1px;
            display: none;
        }
        
        td
        {
            border: 0.5px solid #D6DDE6;
            padding: 0px;
            text-align: left;
            background-color: #FFFFFF;
            color: #333333;
            margin: 0px;
            font-size: 12px;
        }
        th, .Title
        {
            font-size: 12px;
            border: 1px solid #C2D5E3;
            text-align: center;
            color: #336699;
            white-space: nowrap;
            padding: 0px;
            background-color: InfoBackground;
        }
        .Title:hover
        {
            cursor:pointer;
        }
        .ckbgroup
        {
            cursor: default;
        }
        .ckbgroup:hover
        {
            cursor:default;
        }
    </style>
    <script language="JavaScript" src="../Comm/JScript.js"></script>
    <base target="_self" />
</head>
<body topmargin="0" leftmargin="0" onkeypress="Esc()" style="font-size: smaller">
    <form id="form1" runat="server">
    <uc2:Pub ID="Pub1" runat="server" />
    <asp:Button ID="Button1" runat="server" Text="保存" CssClass="Btn" Visible="true" OnClick="Button1_Click" />
    </form>
</body>
</html>
