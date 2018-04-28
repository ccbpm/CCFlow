<%@ Page Title="子流程" Language="C#" MasterPageFile="../WinOpen.Master" AutoEventWireup="true" CodeBehind="SubFlow.aspx.cs" Inherits="CCFlow.WF.WorkOpt.SubFlow" %>
<%@ Register src="../SDKComponents/SubFlowDtl.ascx" tagname="SubFlowDtl" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.7.2.min.js"></script>
  <script language="JavaScript" src="../Comm/JScript.js"  type="text/javascript"></script>
    <script type="text/javascript">

        $(function () {
            //当IsShowTitle 不为0时，不显示标题
            if (GetQueryString("IsShowTitle") != undefined && GetQueryString("IsShowTitle") == 0) {
                $('#DelMsg table caption').hide();
            }
        })
</script>
    <script type="text/javascript" src="../Scripts/QueryString2016.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:SubFlowDtl ID="SubFlowDtl1" runat="server" />
</asp:Content>
