<%@ Page Title="" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.WF_MapDef_FrmAttachment" Codebehind="Attachment.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script language="JavaScript" src="../Comm/JScript.js"></script>
    <script language=javascript>
        /* ESC Key Down */
        function Esc() {
            if (event.keyCode == 27)
                window.close();
            return true;
        }
        function NoSubmit() {
            
        }
    </script>
    <base target=_self /> 
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<center>
    <uc1:Pub ID="Pub1" runat="server" />
    </center>
</asp:Content>

