<%@ Page Title="" Language="C#" MasterPageFile="~/WF/FlowCheck/Site.Master" AutoEventWireup="true" CodeBehind="MyFlow.aspx.cs" Inherits="CCFlow.WF.FlowCheck.MyFlow" %>
<%@ Register src="../SDKComponents/TrackList.ascx" tagname="TrackList" tagprefix="uc1" %>
<%@ Register src="../SDKComponents/TrackChart.ascx" tagname="TrackChart" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

 <%
     BP.WF.Template.SelectAccpers sas = new BP.WF.Template.SelectAccpers();
     sas.RetrieveAll();
     BP.WF.Flow fl = new BP.WF.Flow(this.FK_Flow);
     BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
  %>

 <center>
 <table style="width:900px;">
 <caption><%=fl.Name %> - <%=nd.Name %></caption>
 
 <tr>
   <td>     
       <uc2:TrackChart ID="TrackChart1" runat="server" />
   </td>
 </tr>

 <tr>
   <td>     
       <uc1:TrackList ID="TrackList1" runat="server" />
     </td>
 </tr>

 <tr>
 <td> 
 <fieldset>
 <legend> 工作处理 -审批意见 </legend>
 
     <asp:TextBox ID="TB_Doc"  TextMode="MultiLine"  runat="server" Height="50px"  Width="100%"></asp:TextBox>
     <br />
     <asp:RadioButton ID="RB_OK"   GroupName="xxx" Checked="true" Text="同意并发送到下一步" runat="server" />
     &nbsp;<asp:DropDownList ID="DDL_ToNodes" runat="server">
     </asp:DropDownList>
     <asp:RadioButton ID="RB_UnOK"  Text="不同意退回"  GroupName="xxx" runat="server" />

     <div id="mydiv"  style="visibility:visible;float:right" > 
     <asp:DropDownList ID="DDL_Nodes" runat="server"></asp:DropDownList>
     <asp:DropDownList ID="DDL_ReturnSendModel" runat="server"></asp:DropDownList>
     </div>

     <br />
     <asp:Button ID="Btn_Save" runat="server" Text="确定并执行" onclick="Btn_Save_Click" 
         style="height: 21px" />
 </fieldset>
   </td>
 </tr>
 </table>

 </center>

 <script  type="text/javascript" >
     function Check(ctrl) {
         //alert(ctrl.id);
         var div = document.getElementById('mydiv');

        // alert(div);
        // alert(div.visibility);

         if (ctrl.id == 'ContentPlaceHolder1_RB_UnOK') {
             div.visibility = 'hidden';
         } else {
             div.visibility = 'visible';
         }
         // alert(ctrl);
     }
 </script>
</asp:Content>
