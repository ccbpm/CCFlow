<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_LoginSmall" Codebehind="Login.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

<script  language="javascript" type="text/javascript" >
    function ExitAuth(fk_emp) {
        if (window.confirm('您确定要退出授权登陆模式吗？') == false)
            return;

        var url = 'Do.aspx?DoType=ExitAuth&FK_Emp=' + fk_emp;
        WinShowModalDialog(url, '');
        window.location.href = 'Tools.aspx';
    }

    function NoSubmit(fk_emp) {

    }
</script>

<link rel="shortcut icon" href="./Img/ccbpm.ico" type="image/x-icon" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<table width="100%; border:0px;">
<caption class=CaptionMsg >切换用户</caption>
<tr>
<td>
<center>
    <uc1:Pub ID="Pub1" runat="server" />
    </center>
    </td>
    </tr>
    </table>
</asp:Content>

