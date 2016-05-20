<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.UC.Tools" Codebehind="Tools.ascx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<%@ Register src="ToolsWap.ascx" tagname="ToolsWap" tagprefix="uc2" %>

<style   type="text/css" >

 
 
</style>
<table border=0 width='100%' align='left'  >

<caption class="CaptionMsg" >系统设置</caption>

<tr>
<td  valign=top width='20%' align='center' style="font-size:14px;  height:100%;color:#026ac1" >

 <ul style="line-height:28px;">
  
 <%
     
     BP.WF.XML.Tools tools = new BP.WF.XML.Tools();
     tools.RetrieveAll();
     if (tools.Count == 0)
         return;
     
     string refno = this.RefNo;
     if (refno == null)
         refno = "Per";

     foreach (BP.WF.XML.Tool tool in tools)
     {
         string msg = "";
         if (tool.No == refno)
             msg = "<li><b>" + tool.Name + "</b> </li>";
         else
             msg = "<li><a href='?RefNo=" + tool.No + "' >" + tool.Name + "</a></li>";
        
         %> <%=msg%> <%
     }
        if (BP.Web.WebUser.No == "admin")
        {
              %> 

              <li> <a href='?RefNo=AdminSet' >网站设置</a></li>
              <li> <a href='/WF/Comm/Sys/Holiday.aspx' >节假日设置</a></li>

               <%
        }
       %>
     </ul>
   
    </td>
<td  valign=top  align='left'  width='80%' >
    <uc2:ToolsWap ID="ToolsWap1" runat="server" />
    </td>
</tr>
</table>
