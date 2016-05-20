<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" CodeBehind="BatchStart.aspx.cs" Inherits="CCFlow.WF.BatchStartMy" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
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
 <script language="JavaScript" src="./../Comm/JScript.js" type="text/javascript" ></script>
<script language="JavaScript" src="./../Comm/JS/Calendar/WdatePicker.js" defer="defer" type="text/javascript" ></script>
<script language="javascript" type="text/javascript" >
　　    function SelectAllBS(ctrl) {
        var arrObj = document.all;
        if (ctrl.checked) {
            for (var i = 0; i < arrObj.length; i++) {
                if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                    if (arrObj[i].name.indexOf('IDX_') > 0)
                        arrObj[i].checked = true;
                }
            }
        } else {
            for (var i = 0; i < arrObj.length; i++) {
                if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                    if (arrObj[i].name.indexOf('IDX_') > 0)
                        arrObj[i].checked = false;
                }
            }
        }
    }
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     
    <uc1:Pub ID="Pub1" runat="server" />
      
</asp:Content>
