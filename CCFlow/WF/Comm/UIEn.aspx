<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>

<%@ Page Language="c#" Inherits="BP.Web.Comm.UIEn" ValidateRequest="false" CodeBehind="UIEn.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" >
<html>
<head runat="server">
    <title>驰骋软件,值得信赖.</title>
    <meta http-equiv="pragma" content="no-cache">
    <meta http-equiv="Cache-Control" content="no-cache,   must-revalidate">
    <meta http-equiv="expires" content="Wed,   26   Feb   1978   08:21:57   GMT">
    <script language="javascript" type="text/javascript">
        function calendar(ctrl) {
            var url = './Pub/CalendarHelp.htm';
            val = window.showModalDialog(url, '', 'dialogHeight: 335px; dialogWidth: 340px; center: yes; help: no');
            if (val == undefined)
                return;
            ctrl.value = val;
        }
        function closeEvent() {
            if (location.href.indexOf("closereload=1") == -1) {
                return;
            }

            if (window.opener) {
                try {
                    window.opener.location.reload();
                }
                catch (e) {
                }
            }
        }
    </script>
    <base target="_self" />
</head>
<body onkeypress="Esc()" onunload="closeEvent()" leftmargin="0" topmargin="0" onkeydown='DoKeyDown();'>
    <form id="Form1" method="post" runat="server">
    </form>
</body>
</html>
