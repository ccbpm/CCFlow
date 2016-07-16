<%@ Page Title="" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="CCFlow.WF.MapDef.MapExtUI.ListUI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<%
    string fk_mapdata=this.Request.QueryString["FK_MapData"];
    BP.Sys.MapExts ens = new BP.Sys.MapExts();
    ens.Retrieve(BP.Sys.MapExtAttr.FK_MapData, fk_mapdata);

    BP.Sys.MapExtXmls xmls =new BP.Sys.MapExtXmls();
    xmls.RetrieveAll();
  %>

  <table>
  <%
      foreach (BP.Sys.MapExtXml xml in xmls)
      {
          int num = ens.GetCountByKey(BP.Sys.MapExtAttr.ExtType, xml.No);
          if (num == 0)
              continue;
          %>
          <tr>
          <td class="Title"> <%=xml.Name %> <%=xml.No %></td>
          </tr>
          
           <tr>
           <ul>
          <%
              foreach (BP.Sys.MapExt ext in ens)
              {
                 %> <li> <%=ext.AttrOfOper %> </li><% 
              }
           %>
           </ul>
          </tr>

    <%
      }
     %>
       </table>
</asp:Content>
