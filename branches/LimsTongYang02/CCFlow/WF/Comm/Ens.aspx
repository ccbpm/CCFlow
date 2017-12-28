<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow_Comm_UIEnsV10" CodeBehind="Ens.aspx.cs" %>

<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<%@ Register Src="UC/ucsys.ascx" TagName="ucsys" TagPrefix="uc1" %>
<%@ Register Src="UC/ToolBar.ascx" TagName="ToolBar" TagPrefix="uc2" %>
<%@ Register Src="UC/Pub.ascx" TagName="Pub" TagPrefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Page-Enter" content="revealTrans(duration=0.5, transition=8)" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script language="JavaScript" src="JScript.js"></script>
    <script language="JavaScript" src="ShortKey.js"></script>
    <script language="JavaScript" src="./JS/Calendar/WdatePicker.js" defer="defer"></script>
    <script language="javascript">

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

        function OpenAttrs(ensName) {
            var url = './Sys/EnsAppCfg.aspx?EnsName=' + ensName;
            var s = 'dialogWidth=680px;dialogHeight=480px;status:no;center:1;resizable:yes'.toString();
            val = window.showModalDialog(url, null, s);
            window.location.href = window.location.href;
        }

        function ShowEn(url, wName, h, w) {
            var s = "dialogWidth=" + parseInt(w) + "px;dialogHeight=" + parseInt(h) + "px;resizable:yes";
            var val = window.showModalDialog(url, null, s);
            window.location.href = window.location.href;
        }
        
    </script>
    <style type="text/css">
        .Style1
        {
            width: 100%;
        }
        .Idx
        {
            width: 10px;
        }
    </style>
</head>
<body topmargin="0" leftmargin="0" onkeypress="Esc()" onkeydown='DoKeyDown();'>
    <form id="aspnetForm" runat="server" name="aspnetForm">
    <table id="Table1" align="left" width="100%">
        <tr>
            <td>
                <uc3:Pub ID="Pub1" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="ToolBar">
                <uc2:ToolBar ID="ToolBar1" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="left">
                <uc1:ucsys ID="ucsys1" runat="server" />
                <uc1:ucsys ID="ucsys2" runat="server" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
