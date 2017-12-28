<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="MasterPageSingle.Master"
    CodeBehind="Dot2DotSingle.aspx.cs" Inherits="CCFlow.WF.Comm.RefFunc.Dot2DotSingle" %>

<%@ Register Src="Dot2Dot.ascx" TagName="Dot2Dot" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="javascript" type="text/javascript">
        function SelectAll(cb_selectAll) {
            var arrObj = document.all;
            if (cb_selectAll.checked) {
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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:Dot2Dot ID="Dot2Dot1" runat="server" />
</asp:Content>
