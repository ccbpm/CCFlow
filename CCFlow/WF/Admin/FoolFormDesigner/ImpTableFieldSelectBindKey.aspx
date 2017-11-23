<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/FoolFormDesigner/WinOpen.master" AutoEventWireup="true" CodeBehind="ImpTableFieldSelectBindKey.aspx.cs" Inherits="CCFlow.WF.MapDef.ImpTableFieldSelectBindKey" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script src="../../Scripts/QueryString.js" type="text/javascript"></script>
<script type="text/javascript">
    function SetIt(val) {
        if (parent && parent.window && typeof parent.window.doCloseDialog === 'function') {
            var ctl_id = GetQueryString("ctl_id");
            parent.$("#" + ctl_id).val(val);
            parent.window.doCloseDialog.call();
        } else {
            window.returnValue = val;
        	window.close();
        }
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
