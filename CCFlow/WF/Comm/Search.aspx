<%@ Page language="c#" Inherits="CCFlow.Web.Comm.Search" Codebehind="Search.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="UCSys" Src="UC/UCSys.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<%@ Register src="UC/ToolBar.ascx" tagname="ToolBar" tagprefix="uc2" %>
<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>驰骋技术</title>
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

		<script language="javascript" type="text/javascript">
		function ShowEn(url, wName, h, w )
        {
           var s = "dialogWidth=" + parseInt(w) + "px;dialogHeight=" + parseInt(h) + "px;resizable:yes";
           var  val=window.showModalDialog( url,null,s);
           window.location.href=window.location.href;
        }
		function ImgClick()
		{

		}
		function OpenAttrs(ensName) {
	       var url= './Sys/EnsAppCfg.aspx?EnsName='+ensName + '&T=' + Math.random();
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

       function DoExp() {
           var url = window.location.href;
           //去除#号
           var lastIndex = url.lastIndexOf('#');
           if (lastIndex > -1) {
               url = url.substring(0, lastIndex) + url.substring(lastIndex + 1, url.length);
           }
           url = url + "&DoType=Exp";

           var explorer = window.navigator.userAgent;
           if (explorer.indexOf("Chrome") >= 0) {
               window.open(url, "sd", "left=200,height=500,top=150,width=600,location=yes,menubar=no,resizable=yes,scrollbars=yes,status=no,toolbar=no");
           } else {
               var reVal = window.showModalDialog(url, 'ddd', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
           }
           ReDownExpFile();
       }

       $(function () {
           document.getElementById("ToolBar1_Btn_Excel").href = "#";
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
	</HEAD>
	<body   onkeypress="Esc()"   onkeydown='DoKeyDown();' topmargin="0" leftmargin="0"  >
		<form id="Form1" method="post" runat="server">
			<table id="Table1" align="left" CellSpacing="0" CellPadding="0" border="0" width="100%">
				 <caption  class="CaptionMsg" >
						<asp:Label id="Label1" runat="server">Label</asp:Label>
                        </caption>
				<TR>
					<TD class="ToolBar"   >
                        <uc2:ToolBar ID="ToolBar1" runat="server" />
                    </TD>
				</TR>

				<TR align="justify" height="350px" valign=top  >
					<TD  width='100%'  >
						<uc1:UCSys id="UCSys1" runat="server"></uc1:UCSys>
					</TD>
				</TR>

				<TR>
					<TD ><FONT face="宋体" size=2 >
							<uc1:UCSys id="UCSys2" runat="server"></uc1:UCSys></FONT>
					</TD>
				</TR>
			</table>
		</form>
        <div id="dialogExpFile" style="width:400px; height:300px;">
            <div style="margin:20px; color:Blue;">
                提示：如果没有正常导出文件，请手动点击下方按钮进行下载。
                <br />
                <br />
                <a id="downLoad" href="" class="easyui-linkbutton" data-options="iconCls:'icon-save'">点击下载</a>
            </div>
        </div>
        <input type="hidden" id="expFileName" runat="server" />
	</body>
</HTML>
