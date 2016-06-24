<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.CCForm.Comm_Dtl"
    CodeBehind="Dtl.aspx.cs" %>

<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Comm/JS/Calendar/WdatePicker.js" type="text/javascript"></script>
    <script src="MapExt.js" type="text/javascript"></script>
    <meta http-equiv="Page-Enter" content="revealTrans(duration=0.5, transition=8)" />
    <link href="../../DataUser/Style/Table0.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript">


        var isChange = false;
        function SaveDtlData() {

            //var changeVal = $("#<%=isChange.ClientID %>").val();
            //alert(isChange + "--" + changeVal);
            if (isChange == false)
                return;
            var btn = document.getElementById('Button1');
            btn.click();
            isChange = false;
        }
        function SaveDtlDataTo(url) {

            if (isChange == true) {
                alert('请先执行保存,在退出.');
                return true;
            }
            //SaveDtlData();
            window.location.href = url;
        }

        function TROver(ctrl) {
            ctrl.style.backgroundColor = 'LightSteelBlue';
        }

        function TROut(ctrl) {
            ctrl.style.backgroundColor = 'white';
        }

        function Del(id, ens, refPk, pageIdx) {
            if (window.confirm('您确定要执行删除吗？') == false)
                return;

            var url = 'Do.aspx?DoType=DelDtl&OID=' + id + '&EnsName=' + ens;
            //		        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');

            $.post(url, null, function () {

                window.location.href = 'Dtl.aspx?EnsName=' + ens + '&RefPKVal=' + refPk + '&PageIdx=' + pageIdx;

            }
		        );
        }
        function SetChange(value) {

            isChange = value;


            //$("#<%=isChange.ClientID %>").val(value);

        }

        function DtlOpt(workId, fk_mapdtl, FID) {
            var url = 'DtlOpt.aspx?WorkID=' + workId + '&FK_MapDtl=' + fk_mapdtl + '&FID=' + FID;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = 'Dtl.aspx?EnsName=' + fk_mapdtl + '&RefPKVal=' + workId;
        }
    </script>
    <%-- <style type="text/css">
        .HBtn
        {
            width: 1px;
            height: 1px;
            display: none;
        }
    </style>--%>
    <script language="JavaScript" src="../Comm/JScript.js"></script>
    <script language="JavaScript" src="../Comm/JS/Calendar/WdatePicker.js" defer="defer"></script>
    <script src="MapExt.js" type="text/javascript"></script>
    <script type="text/javascript">
        function SetVal() {
            // document.getElementById('KVs').value = this.GenerPageKVs();
            //  kvs = this.GenerPageKVs();
        }
    </script>
    <script language="javascript" type="text/javascript">
        // row主键信息 .
        var rowPK = null;
        // ccform 为开发者提供的内置函数.
        // 获取DDL值.
        function ReqDDL(ddlID) {
            var v = document.getElementById('Pub1_DDL_' + ddlID + "_" + rowPK).value;
            if (v == null) {
                alert('没有找到ID=' + ddlID + '的下拉框控件.');
            }
            return v;
        }
        // 获取TB值
        function ReqTB(tbID) {
            var v = document.getElementById('Pub1_TB_' + tbID + "_" + rowPK).value;
            if (v == null) {
                alert('没有找到ID=' + tbID + '的文本框控件.');
            }
            return v;
        }
        // 获取CheckBox值
        function ReqCB(cbID) {
            var v = document.getElementById('Pub1_CB_' + cbID + "_" + rowPK).value;
            if (v == null) {
                alert('没有找到ID=' + cbID + '的单选控件.');
            }
            return v;
        }

        /// 获取DDL Obj
        function ReqDDLObj(ddlID) {
            var v = document.getElementById('Pub1_DDL_' + ddlID + "_" + rowPK);
            if (v == null) {
                alert('没有找到ID=' + ddlID + '的下拉框控件.');
            }
            return v;
        }
        // 获取TB Obj
        function ReqTBObj(tbID) {
            var v = document.getElementById('Pub1_TB_' + tbID + "_" + rowPK);
            if (v == null) {
                alert('没有找到ID=' + tbID + '的文本框控件.');
            }
            return v;
        }
        // 获取CheckBox Obj值
        function ReqCBObj(cbID) {
            var v = document.getElementById('Pub1_CB_' + cbID + "_" + rowPK);
            if (v == null) {
                alert('没有找到ID=' + cbID + '的单选控件.');
            }
            return v;
        }

        // 设置控件值.
        function SetCtrlVal(ctrlID, val) {
            document.getElementById('Pub1_TB_' + ctrlID + "_" + rowPK).value = val;
            document.getElementById('Pub1_DDL_' + ctrlID + "_" + rowPK).value = val;
            document.getElementById('Pub1_CB_' + ctrlID + "_" + rowPK).value = val;
        }
        function Esc() {
            if (event.keyCode == 27)
                window.close();
            return true;
        }
    </script>
</head>
<body onkeypress="Esc()" style="font-size: smaller;" class="easyui-layout" onblur="SetVal();"
    onload="SetVal();" topmargin="0" leftmargin="0">
    <div style="display: none;" id="msgTitle">
        请稍后....
    </div>
    <form id="form1" runat="server">
    <div id="mainPanle" region="center" border="false" style="position: fixed">
        <asp:HiddenField Value="true" ID="isChange" runat="server" />
        <asp:Button ID="Button1" runat="server" Text="" CssClass="HBtn" Visible="true" OnClick="Button1_Click" />
        <uc2:Pub ID="Pub1" runat="server" />
        <uc2:Pub ID="Pub2" runat="server" />
    </div>
    </form>
</body>
</html>
