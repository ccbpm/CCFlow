<%@ Page Title="" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true" CodeBehind="FTC.aspx.cs" Inherits="CCFlow.WF.WorkOpt.FTC" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

 <script type="text/javascript">
     function Save() {
     
         try {
             document.getElementById('Btn_Save').click(); //调用btn_save事件.
             alert('保存成功.');
             return true; //保存成功，用户可以发送.
         } catch (e) {
             alert(e.name + " :  " + e.message);
             return false; // 保存失败不能发送.
         }
     }
 </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <uc1:Pub ID="Pub1" runat="server" />

   <%--<div style="display:none ">--%>
   <div  >

    <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click" />
    </div>
  
</asp:Content>
