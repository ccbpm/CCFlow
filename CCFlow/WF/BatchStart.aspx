<%@ Page Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" CodeBehind="BatchStart.aspx.cs"
    Inherits="CCFlow.WF.BatchStartMy" %>

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Scripts/easyUI15/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="Scripts/easyUI15/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/easyUI15/jquery.min.js" type="text/javascript"></script>
    <script src="Scripts/easyUI15/jquery.easyui.min.js" type="text/javascript"></script>
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
    <script language="JavaScript" src="./Comm/JScript.js" type="text/javascript"></script>
    <script language="JavaScript" src="./Comm/JS/Calendar/WdatePicker.js" defer="defer"
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

        $(function () {
            var showTabIndex = <%=this.ShowTabIndex %>;
            $('#tt').tabs('select', showTabIndex);
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="caption" style="width: 100%">
        <div class='CapLeft'>
        </div>
        <div class='CapCenter' style='width: <%=(this.Page.Title.Length * 20).ToString()%>px'>
            <%=this.Page.Title%></div>
        <div class='CapRight'>
        </div>
        <div style='clear: both'>
        </div>
    </div>
    <div id="tt" class="easyui-tabs" data-options="fit:true">
        <div title="手工录入方式">
            <uc1:Pub ID="Pub2" runat="server" />
            <br />
            <br />
            <br />
            <br />
        </div>
        <div title="Excel导入方式">
            <uc1:Pub ID="Pub1" runat="server" />
            <fieldset width='100%' id="fsexcel" runat="server">
                <legend>&nbsp;通过Excel批量导入方式发起</legend>Excel模板：<a href="../DataUser/BatchStartFlowTemplete/<%=this.FK_Flow %>.xls"><img
                    src='/WF/Img/FileType/xls.gif' />下载</a><br />
                Excel文件：
                <asp:FileUpload ID="File1" runat="server" />&nbsp;
                <asp:Button ID="Btn_Imp" runat="server" Text="批量导入发起" OnClick="btn_Upload_Click" />
            </fieldset>
            <br />
            <br />
            <br />
            <br />
        </div>
    </div>
</asp:Content>
