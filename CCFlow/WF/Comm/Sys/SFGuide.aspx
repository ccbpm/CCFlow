<%@ Page Title="创建字典表向导" Language="C#" MasterPageFile="~/WF/Comm/MasterPage.master"
    AutoEventWireup="true" CodeBehind="SFGuide.aspx.cs" Inherits="CCFlow.WF.Comm.Sys.SFGuide" %>

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Style/CommStyle.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script language="JavaScript" src="../JScript.js"></script>
    <script type="text/javascript">
        function generateSQL(dblink, dbname, dbtype, islocal) {
            var table = $('#ContentPlaceHolder1_Pub1_LB_Table').val();
            var no = $('#ContentPlaceHolder1_Pub1_DDL_ColValue').val();
            var name = $('#ContentPlaceHolder1_Pub1_DDL_ColText').val();
            var parentno = $('#ContentPlaceHolder1_Pub1_DDL_ColParentNo').val();
            var type = $('#ContentPlaceHolder1_Pub1_DDL_SFTableType').val();
            var txtsql = $('#ContentPlaceHolder1_Pub1_TB_SelectStatement');
            var sql;
            switch (dbtype) {
                case 'SQLServer':
                    sql = 'SELECT ' + no + ' AS No, ' + name + ' AS [Name]' + (type == '1' ? (', ' + parentno + ' AS ParentNo') : '') + ' FROM ' + (islocal ? '' : (dblink + '.' + dbname + '.')) + 'dbo.' + table;
                    break;
                case 'Oracle':
                case 'MySQL':
                    sql = 'SELECT ' + no + ' AS No, ' + name + ' AS \'Name\'' + (type == '1' ? (', ' + parentno + ' AS ParentNo') : '') + ' FROM OPENQUERY(' + dblink + ',\'SELECT * FROM ' + dbname + '.' + table + '\')';
                    break;
                case 'Informix':
                    sql = '';
                    break;
            }

            txtsql.text(sql);
        }

        function showInfo(title, msg, autoHiddenMillionSeconds) {
            $.messager.show({
                title: title,
                msg: msg,
                timeout: autoHiddenMillionSeconds,
                showType: 'slide',
                style: {
                    right: '',
                    top: document.body.scrollTop + document.documentElement.scrollTop,
                    bottom: ''
                }
            });
        }

        function showInfoAndGo(title, msg, icon, url) {
            if (url == undefined || url == null || url.length == 0) {
                $.messager.alert(title, msg, icon);
            }
            else {
                $.messager.alert(title, msg, icon, function () {
                    self.location = url;
                });
            }
        }

        function showInfoAndBack(title, msg, icon) {
            $.messager.alert(title, msg, icon, function () {
                history.back();
            });
        }
    </script>
    <base target=_self />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>
