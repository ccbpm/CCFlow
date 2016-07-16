<%@ Page Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.WF_MapDef_GroupField" Title="未命名頁面" Codebehind="GroupField.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server"  >
<script language="javascript" >
   function Del( refNo, refOID )
	{
	 if ( window.confirm('您确定要删除吗？ ') == false ) 
	      return false;
	   window.location.href='GroupField.aspx?RefNo='+ refNo +'&DoType=DelGF&RefOID=' + refOID;
    }
</script>
<base target=_self />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>

