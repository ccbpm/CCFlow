<%@ Page Title="授权" Language="C#" MasterPageFile="~/WF/App/Setting/Site.Master" AutoEventWireup="true"
    CodeBehind="Auto.aspx.cs" Inherits="CCFlow.WF.App.Setting.Auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <asp:Repeater runat="server"  ID="repList">
    <HeaderTemplate>
     <table cellpadding='0' cellspacing='0' bordercolor="#3399FF" border="1">
            <caption class="CaptionMsgLong">
                请选择您要授权的人员</caption>
            <tr>
                <td style="width: 200px">
                    编号
                </td>
                <td style="width: 200px">
                    部门
                </td>
                <td style="width: 200px">
                    要执行授权的人员
                </td>
            </tr>
    </HeaderTemplate>
    <ItemTemplate>
    <tr> 
   
    <td><%#Eval("No") %></td>
        <td><%#Eval("DeptName")%></td>
              <td> <a href="AutoDtl.aspx?FK_emp=<%#Eval("No") %>"> <%#Eval("Name") %> </a></td>
    </tr>
    
    </ItemTemplate>
    <FooterTemplate></table></FooterTemplate>
       
    
    </asp:Repeater>
</asp:Content>
