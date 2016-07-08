<%@ Page Title="" Language="C#" MasterPageFile="~/Prj/MasterPage.master" AutoEventWireup="true" CodeFile="DocTree.aspx.cs" Inherits="ExpandingApplication_PRJ_DocTree" %>

<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script type="text/javascript">
        function Upload(refNo, idx) {
           
            var url = 'DocTree.aspx?DoType=Upload&RefNo=' + refNo + '&IDX=' + idx;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 200px; dialogWidth: 300px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function DoDown(refNo, idx) {
           
            var url = 'DocTree.aspx?DoType=DoDown&RefNo=' + refNo + '&IDX=' + idx;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 200px; dialogWidth: 300px;center: yes; help: no');
        }
        function DoDelete(refNo, idx) {

            if (window.confirm('您确定要删除吗？') == false)
                return;
            var url = 'DocTree.aspx?DoType=DoDelete&RefNo=' + refNo + '&IDX=' + idx;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 200px; dialogWidth: 300px;center: yes; help: no');
            window.location.href = window.location.href;
        }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>

