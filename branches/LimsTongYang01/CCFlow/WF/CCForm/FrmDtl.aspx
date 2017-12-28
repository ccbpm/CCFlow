<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.CCForm.WF_FrmDtl" Codebehind="FrmDtl.aspx.cs" %>
<%@ Register src="../UC/UCEn.ascx" tagname="UCEn" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <style type="text/css">
        .HBtn
        {
        	/* display:none; */
        	visibility:visible;
        }
    </style>
	<script language="JavaScript" src="../Comm/JScript.js"></script>
    <script language="JavaScript" src="../Comm/JS/Calendar/WdatePicker.js" defer="defer" ></script>
    <script src="../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script language="javascript" >
        function SaveDtlData() {
            var btn = document.getElementById("<%=Btn_Save.ClientID %>");
            if (btn)
                btn.click();
        }
        function Change() { 
        
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:Button ID="Btn_Save" runat="server" Text="保存"  CssClass="Btn" Visible=true 
        onclick="Btn_Save_Click"  />
    <uc1:UCEn ID="UCEn1" runat="server" />
</asp:Content>

