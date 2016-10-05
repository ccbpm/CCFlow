<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.Comm_MapDef_MapDtl"
    CodeBehind="MapDtl.aspx.cs" %>

<%@ Register Src="../../UC/UCEn.ascx" TagName="UCEn" TagPrefix="uc2" %>
<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>表单设计</title>
    <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <script language="JavaScript" src="../../Comm/JScript.js" type="text/javascript" ></script>
    <base target="_self" />
    <script language="javascript">
        //	    function AutoFull(fullID) {
        //	        alert(id);
        //	    }

        function Insert(fk_mapdata, IDX) {
            var url = 'FieldTypeList.aspx?FK_MapData=' + fk_mapdata + '&IDX=' + IDX + '&s=' + Math.random();
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function AddF(fk_mapdata) {
            var url = 'FieldTypeList.aspx?FK_MapData=' + fk_mapdata + '&s=' + Math.random();
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function Edit(fk_mapdata, mypk, ftype) {
            var url = 'EditF.aspx?DoType=Edit&FK_MapData=' + fk_mapdata + '&MyPK=' + mypk + '&FType=' + ftype + '&s=' + Math.random();
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function EditEnum(mypk, refno) {
            var url = 'EditEnum.aspx?DoType=Edit&MyPK=' + mypk + '&RefNo=' + refno + '&s=' + Math.random();
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function EditTable(mypk, refno) {
            var url = 'EditTable.aspx?DoType=Edit&MyPK=' + mypk + '&RefNo=' + refno + '&s=' + Math.random();
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function Up(mypk, refoid) {
            var url = 'Do.aspx?DoType=Up&MyPK=' + mypk + '&RefOID=' + refoid + '&s=' + Math.random();
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');
            //window.location.href ='Designer.aspx?PK='+mypk+'&IsOpen=1';
            window.location.href = window.location.href;
        }
        function Down(mypk, refoid) {
            var url = 'Do.aspx?DoType=Down&MyPK=' + mypk + '&RefOID=' + refoid + '&s=' + Math.random();
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
            //   window.location.href ='Designer.aspx?PK='+mypk+'&IsOpen=1';
            //  window.location.href ='Designer.aspx?PK='+mypk+'&IsOpen=1';
        }
        function Del(mypk, refoid) {
            if (window.confirm('您确定要删除吗？') == false)
                return;

            var url = 'Do.aspx?DoType=Del&MyPK=' + mypk + '&RefOID=' + refoid + '&s=' + Math.random();
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function Esc() {
            if (event.keyCode == 27)
                window.close();
            return true;
        }
    </script>
</head>
<body topmargin="0" leftmargin="0" onkeypress="Esc()">
    <form id="form1" runat="server">
    <div align="left">
        <uc1:Pub ID="Pub1" runat="server" />
    </div>
    </form>
</body>
</html>
