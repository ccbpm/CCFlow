<%@ Page Language="c#" Inherits="CCFlow.Web.WF.Comm.UIContrastDtl" CodeBehind="ContrastDtl.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="UCSys" Src="UC/UCSys.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="Head1" runat="server">
    <title>详细信息</title>
    <link href="Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script language="JavaScript" src="JScript.js"></script>
    <script language="javascript" type="text/javascript" >
        function ShowEn(url, wName) {
            val = window.showModalDialog(url, wName, 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
            // window.location.href=window.location.href;
        }
        function DoExp() {
            var url = window.location.href;
            url = url + "&DoType=Exp";
            //var v = window.showModalDialog(url, 'ddd', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
            var v = WinOpen(url, 'wincommgroup', 900, 900);
        }
        function Esc() {
            if (event.keyCode == 27)
                window.close();
            return true;
        }
    </script>
    <base target="_self" />
</head>
<body onkeypress="Esc()" class="easyui-layout">
    <form id="Form1" method="post" runat="server">

    <table>
    <caption  > <%=this.ShowTitle %>; <a href=javascript:DoExp(); >导出到Excel</a></caption>
    <tr>
    <td>
    <%--<div data-options="region:'center',border:false,title:'<%=this.ShowTitle %>; <a href=javascript:DoExp(); >导出到Excel</a>'">--%>
        <uc1:UCSys ID="UCSys1" runat="server"></uc1:UCSys>
    <%--</div> --%>
    </td>
    </tr>
    </table>
    </form>
</body>
</html>
