<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/FoolFormDesigner/WinOpen.master" AutoEventWireup="true" CodeBehind="SysEnum.aspx.cs" Inherits="CCFlow.WF.Admin.FoolFormDesigner.UISysEnum" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <script language="javascript" type="text/javascript" >
     /* ESC Key Down  */
     function Esc() {
         if (event.keyCode == 27)
             window.close();
         return true;
     }
     function RSize() {

         if (document.body.scrollWidth > (window.screen.availWidth - 100)) {
             window.dialogWidth = (window.screen.availWidth - 100).toString() + "px";
         } else {
             window.dialogWidth = (document.body.scrollWidth + 50).toString() + "px";
         }

         if (document.body.scrollHeight > (window.screen.availHeight - 70)) {
             window.dialogHeight = (window.screen.availHeight - 50).toString() + "px";
         } else {
             window.dialogHeight = (document.body.scrollHeight + 115).toString() + "px";
         }
         window.dialogLeft = ((window.screen.availWidth - document.body.clientWidth) / 2).toString() + "px";
         window.dialogTop = ((window.screen.availHeight - document.body.clientHeight) / 2).toString() + "px";
     }
    </script>
    <base target=_self /> 

    <style type="text/css">
        .style1
        {
            width: 437px;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table  style="width:100%">
<tr>
<td valign="top"  > 

<table style="width:100%;">
<caption> 枚举属性 </caption>
<tr>
<td nowarp=true >中文名:</td>
<td> <asp:TextBox ID="TB_Name" runat="server" AutoPostBack="True" 
        ontextchanged="TextBox1_TextChanged"></asp:TextBox></td>
</tr>

<tr>
<td  nowarp=true >编号:</td>
<td> <asp:TextBox ID="TB_No" runat="server"></asp:TextBox></td>
</tr>

<tr>
<td colspan="2"   >
    <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click"   />
    <asp:Button ID="Btn_SaveAndClose" runat="server" Text="保存&关闭" onclick="Btn_SaveAndClose_Click" 
            />
    <asp:Button ID="Btn_SaveAndAddFrm" runat="server" Text="保存并放到表单里" onclick="Btn_SaveAndAddFrm_Click" 
          />
    <asp:Button ID="Btn_Del" runat="server" Text="删除" onclick="Btn_Del_Click"    />

    </td>
</tr>
</table>
 
 </td>

<td valign="top"  style="width:70%;"> 
    <uc1:Pub ID="Pub1" runat="server" />
    </td>
</tr>
</table>

</asp:Content>
