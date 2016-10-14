<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true"
    CodeBehind="BatchStart.aspx.cs" Inherits="CCFlow.WF.BatchStartMy" %>

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
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
    <script language="JavaScript" src="./../Comm/JScript.js" type="text/javascript"></script>
    <script language="JavaScript" src="./../Comm/JS/Calendar/WdatePicker.js" defer="defer"
        type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
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
    <uc1:Pub ID="Pub2" runat="server" />
    <fieldset width='100%'>
        <legend>&nbsp;通过Excel导入方式发起:<a href="../DataUser/BatchStartFlowTemplete/<%=this.FK_Flow %>.xls"><img
            src='/WF/Img/FileType/xls.gif' />下载Excel模版</a>&nbsp;</legend>文件名:
        <asp:FileUpload ID="File1" runat="server" />&nbsp;
        <asp:Button ID="Btn_Imp" runat="server" Text="导入" OnClick="btn_Upload_Click" />
    </fieldset>
</asp:Content>
