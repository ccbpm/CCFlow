<%@ Page Title="关键字查询" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_KeySearch" Codebehind="KeySearch.aspx.cs" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    function OpenIt(fk_flow, fk_node, workid) {
        var url = './WFRpt.aspx?WorkID=' + workid + '&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node;
        var newWindow = window.open(url, 'card', 'width=700,top=50,left=50,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false');
        newWindow.focus();
        return;
    }
    function NoSubmit(ev) {
        if (window.event.srcElement.tagName == "TEXTAREA")
            return true;
        if (ev.keyCode == 13) {
            window.event.keyCode = 9;
            ev.keyCode = 9;
            return true;
        }
        return true;
    }
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
<table width="100%" border="0">
<caption class="CaptionMsgLong">全文检索</caption>
<tr>
<td style= "text-align:center"></br></br>
<b>&nbsp;输入任何关键字:</b><asp:TextBox ID="TextBox1" runat="server" BorderStyle=Inset
 BorderColor=AliceBlue
 Font-Bold="True" 
        Font-Size="Large" Width="259px"></asp:TextBox> 
<asp:CheckBox ID="CheckBox1" runat="server" Font-Bold="True" 
    ForeColor="#0033CC" Text="仅查询我参与的流程" />
        <br />
    &nbsp;<asp:Button ID="Btn_ByWorkID" runat="server" Text="按工作ID查"   
         onclick="Button1_Click"  />

        <asp:Button ID="Btn_ByTitle" runat="server" Text="按流程标题字段关键字查"  
          onclick="Button1_Click"/>

        <asp:Button ID="Btn_ByAll" runat="server" Text="全部字段关键字查" Visible=false Font-Bold="True" 
         onclick="Button1_Click" />
<%--说明:为了提高查询效率请正确的选择查询方式.--%>
<br>
    
</td>
</tr>
</table>
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>

