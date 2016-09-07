<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_ToolsSmall" Codebehind="Tools.aspx.cs" %>

<%@ Register src="UC/Tools.ascx" tagname="Tools" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">    
        a
        {
            color:#0066CC;
            text-decoration:none;
        }
        a:hover
        {
            color:#0084C5;
            text-decoration:underline;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Tools ID="Tools1" runat="server" />
    <script src="Comm/JS/Calendar/WdatePicker.js" type="text/javascript"></script>
    
<script language="JavaScript" src="./Comm/JScript.js"></script>
<script language=javascript >

    function DoAutoTo(fk_emp, empName) {
        if (window.confirm('您确定要把您的工作授权给[' + fk_emp + ']吗？') == false)
            return;
        var url = 'Do.aspx?DoType=AutoTo&FK_Emp=' + fk_emp;
        WinShowModalDialog(url, '');
        alert('授权成功，请别忘记收回。');
        window.location.href = 'Tools.aspx';
    }

    function ExitAuth(fk_emp) {
        if (window.confirm('您确定要退出授权登陆模式吗？') == false)
            return;

        var url = 'Do.aspx?DoType=ExitAuth&FK_Emp=' + fk_emp;
        WinShowModalDialog(url, '');
        window.location.href = 'Tools.aspx';
    }

    function TakeBack(fk_emp) {
        if (window.confirm('您确定要取消对[' + fk_emp + ']的授权吗？') == false)
            return;

        var url = 'Do.aspx?DoType=TakeBack';
        WinShowModalDialog(url, '');
        alert('您已经成功的取消。');
        window.location.reload();
    }

    function LogAs(fk_emp) {
        if (window.confirm('您确定要以[' + fk_emp + ']授权方式登陆吗？') == false)
            return;

        var url = 'Do.aspx?DoType=LogAs&FK_Emp=' + fk_emp;
        WinShowModalDialog(url, '');
        alert('登陆成功，现在您可以以[' + fk_emp + ']处理工作。');
        window.location.href = 'EmpWorks.aspx';
    }

    function CHPass() {
        var url = 'Do.aspx?DoType=TakeBack';
        // WinShowModalDialog(url,'');
        alert('密码修改成功，请牢记您的新密码。');
    }


</script>
</asp:Content>

