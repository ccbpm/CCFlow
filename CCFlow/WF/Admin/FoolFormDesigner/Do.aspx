<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.Comm_MapDef_Do" Codebehind="Do.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title runat=server /> </title>
   <link href="../../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
   <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
   <script language="JavaScript" src="../../Comm/JScript.js" type="text/javascript" ></script>
    <script language=javascript>
        function RSize() {

            return;
            if (document.body.scrollWidth > (window.screen.availWidth - 100)) {
                window.dialogWidth = (window.screen.availWidth - 100).toString() + "px"
            } else {
                window.dialogWidth = (document.body.scrollWidth + 50).toString() + "px"
            }

            if (document.body.scrollHeight > (window.screen.availHeight - 70)) {
                window.dialogHeight = (window.screen.availHeight - 50).toString() + "px"
            } else {
                window.dialogHeight = (document.body.scrollHeight + 115).toString() + "px"
            }
            window.dialogLeft = ((window.screen.availWidth - document.body.clientWidth) / 2).toString() + "px"
            window.dialogTop = ((window.screen.availHeight - document.body.clientHeight) / 2).toString() + "px"
        }
    /* ESC Key Down  */
    function Esc()
    {
        if (event.keyCode == 27)
            window.close();
       return true;
    }
    function EditEnum(key)
    {
        var url='SysEnum.aspx?DoType=Edit&RefNo='+key;
      //  window.location.href=url;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 500px;center: yes; help: no'); 
        window.location.reload(); 
    }
    function NewEnum()
    {
        var url='SysEnum.aspx?DoType=New&EnumKey=';
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 500px; dialogWidth: 500px;center: yes; help: no'); 
       window.location.href = window.location.href;
    }
    function AddEnum(mypk, idx, key) {
        var url = '';
        url = 'EditEnum.aspx?DoType=Edit&MyPK=' + mypk + '&EnumKey=' + key + '&IDX=' + idx;
        var c = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 500px;center: yes; help: no');
        return;
    }
    function AddSFTable(mypk, idx, key) {
       
        var url = 'Do.aspx?DoType=AddSFTableAttr&MyPK=' + mypk + '&IDX=' + idx + '&RefNo=' + key;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 500px;center: yes; help: no');
    }
    function AddSFSQL(mypk, idx, key) {

        var url = 'Do.aspx?DoType=AddSFSQLAttr&MyPK=' + mypk + '&IDX=' + idx + '&RefNo=' + key;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 500px;center: yes; help: no');
    }
    function HidShowSysField() {
        var v = document.getElementById('SysField').style.display;
        if (v == 'none')
            v = 'block';
        else
            v = 'none';
        document.getElementById('SysField').style.display = v;
    }
    </script>
    <base target=_self />
    <style type="text/css">
    .FUL
    {
    }
    .FLI
    {
    }
    </style>
</head>
<body  topmargin="5" leftmargin="5" onkeypress="Esc()"  onload="RSize()" >
    <form id="form1" runat="server">
    <div align=left width="80%" >
      <uc1:Pub ID="Pub1" runat="server" />
      <uc1:Pub ID="Pub2" runat="server" />
    </div>
    </form>
</body>
</html>
 

  


  

