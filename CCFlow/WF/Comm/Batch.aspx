<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" 
Inherits="CCFlow.WF.Comm.Comm_Batch" Title="批处理" Codebehind="Batch.aspx.cs" %>
<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<%@ Register Src="./UC/UCSys.ascx" TagName="UCSys" TagPrefix="uc1" %>
<%@ Register src="./UC/ToolBar.ascx" tagname="ToolBar" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
        <link href='./Style/Table0.css' rel='stylesheet' type='text/css' />
        <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
        <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
        <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
        <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
		<script language="JavaScript" src="JScript.js" type="text/javascript"></script>
		<script language="JavaScript" src="Menu.js" type="text/javascript"></script>
		<script language="JavaScript" src="ShortKey.js" type="text/javascript"></script>
        <script src="./JS/Calendar/WdatePicker.js" type="text/javascript"></script>
        <link href="./JS/Calendar/skin/WdatePicker.css" rel="stylesheet" type="text/css" />
        <link href='./Style/Table0.css' rel='stylesheet' type='text/css' />
        <script src="../Scripts/EasyUIUtility.js" type="text/javascript"></script>

    <script language="JavaScript" src="JScript.js"></script>
    <script language="javascript">
        function ShowEn(url, wName, h, w) {

            OpenEasyUiDialog(url, "eudlgframe", "详细", 900, 550, "icon-edit", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }


        function selectAll() {
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
　　 	function OpenAttrs(ensName)
		{
	       var url= './Sys/EnsAppCfg.aspx?EnsName='+ensName;
           var s =  'dialogWidth=680px;dialogHeight=480px;status:no;center:1;resizable:yes'.toString() ;
		   val=window.showModalDialog( url , null ,  s);
           window.location.href=window.location.href;
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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<div align=left>
    <table id="Table1" align="left"  border=0 width='100%'>
        <tr>
            <td class='ToolBar'  border=0 >
                <uc2:ToolBar ID="ToolBar1" runat="server" />
                </td>
        </tr>
                
                <tr>
         <td align=left  border=0>
                <uc1:UCSys ID="UCSys1" runat="server" />
            </td>
        </tr>
        <tr class='TRSum'  border=0>
            <td>
                <uc1:UCSys ID="UCSys3" runat="server" />
            </td>
        </tr>
        <tr>
            <td  Class='ToolBar'  border=0 >
                    <uc1:UCSys ID="UCSys2" runat="server" />
            </td>
        </tr>
    </table>
    </div>
</asp:Content>
