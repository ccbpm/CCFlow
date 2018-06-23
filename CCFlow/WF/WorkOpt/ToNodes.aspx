<%@ Page Title="选择节点转向" Language="C#" MasterPageFile="../SDKComponents/Site.Master" AutoEventWireup="true" CodeBehind="ToNodes.aspx.cs"
 Inherits="CCFlow.Plug_in.CCFlow.WF.WorkOpt.UIToNodes" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <script language="JavaScript" src="../Comm/JScript.js" type="text/javascript"></script>
    <script type="text/javascript">
        function SetUnEable(ctrl) {
            SetEnable(true);
        }

        function RBSameSheet(ctrl) {
            if (ctrl.checked) {
                SetEnable(false);
            }
            else {
                SetEnable(true);
            }
        }

        function SetEnable(enable) {
            var arrObj = document.all;
            for (var i = 0; i < arrObj.length; i++) {
                if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                    arrObj[i].disabled = enable;
                }
            }
        }
        function SetRBSameSheetCheck() {
            var arrObj = document.all;
            for (var i = 0; i < arrObj.length; i++) {
                if (typeof arrObj[i].type != "undefined" && arrObj[i].id.valueOf('RB_SameSheet') != -1) {
                    arrObj[i].checked = true;
                    break;
                }
            }
//            var arrObj = document.all;
//            RB_SameSheet
//            ctrl.checked = true;
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div align=center>
<table width="100%" align="center" >
<caption>您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %>   --   请选择到达的节点</caption>
<tr>
<td width="20%">
</td>

<td>
    <uc1:Pub ID="Pub1" runat="server" />
</td>
</tr>
</table>
</div>

</asp:Content>
