<%@ Page Language="c#" Inherits="CCFlow.Web.Comm.Search" CodeBehind="Search.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="UCSys" Src="UC/UCSys.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<%@ Register Src="UC/ToolBar.ascx" TagName="ToolBar" TagPrefix="uc2" %>
<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title>驰骋技术</title>
    <link href='./Style/Table0.css' rel='stylesheet' type='text/css' />
    <link href="../Scripts/easyUI15/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI15/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI15/jquery.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI15/jquery.easyui.min.js" type="text/javascript"></script>
    <script language="JavaScript" src="JScript.js" type="text/javascript"></script>
    <script language="JavaScript" src="Menu.js" type="text/javascript"></script>
    <script language="JavaScript" src="ShortKey.js" type="text/javascript"></script>
    <script src="./JS/Calendar/WdatePicker.js" type="text/javascript"></script>
    <script src="Gener.js" type="text/javascript"></script>
    <link href="./JS/Calendar/skin/WdatePicker.css" rel="stylesheet" type="text/css" />
    <link href='./Style/Table0.css' rel='stylesheet' type='text/css' />
    <script language="javascript" type="text/javascript">
        function ShowEn(url, wName, h, w) {
            OpenDialogAndCloseRefresh(url, "编辑", w, h, "icon-edit");
        }
        function ImgClick() {

        }
        function OpenAttrs(ensName) {
            var url = './Sys/EnsAppCfg.aspx?EnsName=' + ensName + '&inlayer=1&t=' + Math.random();
            OpenDialogAndCloseRefresh(url, "设置", 720, 500, "icon-edit");
        }
        function closeDlg() {
            $("#eudlg").dialog("close");
        }

        function DDL_mvals_OnChange(ctrl, ensName, attrKey) {
            var idx_Old = ctrl.selectedIndex;
            if (ctrl.options[ctrl.selectedIndex].value != 'mvals')
                return;
            if (attrKey == null)
                return;

            var url = 'SelectMVals.aspx?EnsName=' + ensName + '&AttrKey=' + attrKey;
            var val = window.showModalDialog(url, 'dg', 'dialogHeight: 450px; dialogWidth: 450px; center: yes; help: no');
            if (val == '' || val == null) {
                // if (idx_Old==ctrl.options.cont
                ctrl.selectedIndex = 0;
                //    ctrl.options[0].selected = true;
            }
        }

        function DoExp() {
            var url = window.location.href;
            //去除#号
            var lastIndex = url.lastIndexOf('#');
            if (lastIndex > -1) {
                url = url.substring(0, lastIndex) + url.substring(lastIndex + 1, url.length);
            }
            url = url + "&DoType=Exp";

            var explorer = window.navigator.userAgent;
            window.open(url, "sd", "left=200,height=500,top=150,width=600,location=yes,menubar=no,resizable=yes,scrollbars=yes,status=no,toolbar=no");
            ReDownExpFile();
        }

        $(function () {
            try {
                document.getElementById("ToolBar1_Btn_Excel").href = "#";
            } catch (e) { }
            $("#dialogExpFile").hide();
        });

        function ReDownExpFile() {
            var fileName = $("*[id$=expFileName]").val();
            document.getElementById("downLoad").href = "../../DataUser/Temp/" + fileName;

            $("#dialogExpFile").show();
            $("#dialogExpFile").dialog({
                height: 300,
                width: 400,
                showMax: false,
                isResize: true,
                modal: true,
                title: "手动下载文件",
                slide: false,
                close: function () {
                },
                buttons: [{ text: '关闭', handler: function () {
                    $('#dialogExpFile').dialog('close');
                }
                }]
            });
        }
    </script>
</head>
<body onkeypress="Esc()" onkeydown='DoKeyDown();' topmargin="0" leftmargin="0">
    <form id="Form1" method="post" runat="server">
    <table id="Table1" align="left" cellspacing="0" cellpadding="0" border="0" topmargin="0"
        leftmargin="0" width="100%">
        <tr>
            <td class="ToolBar" topmargin="0" leftmargin="0">
                <uc2:ToolBar ID="ToolBar1" runat="server" />
            </td>
        </tr>
        <tr align="justify" height="350px" valign="top">
            <td width='100%'>
                <uc1:UCSys ID="UCSys1" runat="server"></uc1:UCSys>
            </td>
        </tr>
        <tr>
            <td>
                <font face="宋体" size="2">
                    <uc1:UCSys ID="UCSys2" runat="server"></uc1:UCSys>
                </font>
            </td>
        </tr>
    </table>
    </form>
    <div id="dialogExpFile" style="width: 400px; height: 300px;">
        <div style="margin: 20px; color: Blue;">
            提示：如果没有正常导出文件，请手动点击下方按钮进行下载。
            <br />
            <br />
            <a id="downLoad" href="" class="easyui-linkbutton" data-options="iconCls:'icon-save'">
                点击下载</a>
        </div>
    </div>
    <input type="hidden" id="expFileName" runat="server" />
</body>
</html>
