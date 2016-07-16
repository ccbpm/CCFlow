<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" ValidateRequest="false"  
AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.WF_MapDef_MapDtlMTR" Codebehind="MapDtlMTR.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
     <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
     <link href="../../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
	<script language="JavaScript" src="../../Comm/JScript.js" ></script>
    
    <script type="text/javascript">
        function Rep() {
            var mytb = document.getElementById('ContentPlaceHolder1_Pub1_TB_Doc');
            var s = mytb.value;
            s = s.replace(/</g, "《");
            s = s.replace(/>/g, "》");
            s = s.replace(/'/g, "‘");
            alert(s);
            mytb.value = s;
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
      <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>