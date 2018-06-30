<%@ Page Title="" Language="C#" MasterPageFile="Single.master" AutoEventWireup="true" Inherits="CCFlow.WF.Rpt.Comm_SelectMValsWF" Codebehind="SelectMVals.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server"  >
<script type="text/javascript">
    function SelectAll() {
        var arrObj = document.all;
        if (document.forms[0].checkedAll.checked) {
            for (var i = 0; i < arrObj.length; i++) {
                if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                    arrObj[i].checked = true;
                }
            }
        } else {
            for (var i = 0; i < arrObj.length; i++) {
                if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox')
                    arrObj[i].checked = false;
            }
        }
    } 
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>

