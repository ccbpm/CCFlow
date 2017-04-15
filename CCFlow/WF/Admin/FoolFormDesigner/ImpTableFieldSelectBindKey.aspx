<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/FoolFormDesigner/WinOpen.master" AutoEventWireup="true" CodeBehind="ImpTableFieldSelectBindKey.aspx.cs" Inherits="CCFlow.WF.MapDef.ImpTableFieldSelectBindKey" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
    function SetIt(val) {
        window.returnValue = val;
        window.close();
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<fieldset>
<legend>枚举类型</legend>
<ul>
<% 
    
   BP.Sys.SysEnumMains ens = new BP.Sys.SysEnumMains();
   ens.RetrieveAll();
    
       foreach (BP.Sys.SysEnumMain en in ens)
       {
           %> <li> <a href="javascript:SetIt('<%=en.No %>');"> <%= en.Name %> </a> <font color=green> <%=en.CfgVal %> </font>  </li>
      <% } %>
</ul>
</fieldset>

<fieldset>
<legend>外键类型</legend>

<ul>
<% 
    BP.Sys.SFTables tabs = new BP.Sys.SFTables();
    tabs.RetrieveAll();

    foreach (BP.Sys.SFTable en in tabs)
       {
           %> <li> <a href="javascript:SetIt('<%=en.No %>');"> <%= en.Name %> </a> <font color=green> <%=en.TableDesc %> </font>  </li>
      <% } %>
</ul>
 

</fieldset>

</asp:Content>
