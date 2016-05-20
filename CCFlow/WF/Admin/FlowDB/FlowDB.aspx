<%@ Page Title="流程监控" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true"
    Inherits="CCFlow.WF.Admin.WF_Admin_FlowDB" CodeBehind="FlowDB.aspx.cs" %>

<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Comm/JScript.js" type="text/javascript"></script>
    <script src="../../Comm/JS/Calendar/WdatePicker.js" type="text/javascript"></script>
    <script src="../../Scripts/EasyUIUtility.js" type="text/javascript"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            $("table.Table tr:gt(0)").hover(
                function () { $(this).addClass("tr_hover"); },
                function () { $(this).removeClass("tr_hover"); });
        });

        //工作删除.
        function DelIt(fk_flow, workid) {
            if (window.confirm('Are you sure?') == false)
                return;
            var url = 'FlowDB.aspx?DoType=DelIt&FK_Flow=' + fk_flow + '&WorkID=' + workid;
            WinShowModalDialog(url, '');
            window.location.href = window.location.href;
        }

        //流程移交.
        function FlowShift(fk_flow, workid) {

            var url = 'FlowShift.aspx?FK_Flow=' + fk_flow + '&WorkID=' + workid;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px; dialogTop: 100px; dialogLeft: 110px; center: yes; help: no');
            if (b != null)
                window.location.href = window.location.href;
        }

        //跳转.
        function FlowSkip(fk_flow, workid) {
            var url = 'FlowSkip.aspx?FK_Flow=' + fk_flow + '&WorkID=' + workid;
            var v = window.showModalDialog(url, 'sd', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
            if (v != null)
                window.location.href = window.location.href;
        }

        function openSelectEmp(hid, tid) {
            var url = '../../Comm/Port/SelectUser_Jq.aspx';
            var selected = $('#' + hid).val();
            if (selected != null && selected.length > 0) {
                url += '?In=' + selected + '&tk=' + Math.random();
            }

            OpenEasyUiDialog(url, 'eudlgframe', '选择发起人', 760, 470, 'icon-user', true, function (ids) {
                var arr = ids.split(',');
                var hiddenId = arr[0];
                var tbId = arr[1];

                var innerWin = document.getElementById('eudlgframe').contentWindow;
                $('#' + tbId).val(innerWin.getReturnText());
                $('#' + hiddenId).val(innerWin.getReturnValue());
            }, hid + ',' + tid);
        }

        function openSelectDept(hid, tid) {
            var url = '../../Comm/Port/SelectDepts_Jq.aspx';
            var selected = $('#' + hid).val();
            if (selected != null && selected.length > 0) {
                url += '?In=' + selected + '&tk=' + Math.random();
            }

            OpenEasyUiDialog(url, 'eudlgframe', '选择部门', 520, 360, 'icon-department', true, function (ids) {
                var arr = ids.split(',');
                var hiddenId = arr[0];
                var tbId = arr[1];
                
                var innerWin = document.getElementById('eudlgframe').contentWindow;
                $('#' + tbId).val(innerWin.getReturnText());
                $('#' + hiddenId).val(innerWin.getReturnValue());
            }, hid + ',' + tid);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:Pub ID="Pub3" runat="server" />
    <uc1:Pub ID="Pub1" runat="server" />
    <uc1:Pub ID="Pub2" runat="server" />
</asp:Content>
