<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintSample.aspx.cs" Inherits="CCFlow.WF.PrintSimple" %>

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<%@ Register Src="./UC/UCEn.ascx" TagName="UCEn" TagPrefix="uc2" %>
<%@ Register Src="./Comm/UC/ToolBar.ascx" TagName="ToolBar" TagPrefix="uc3" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <link href="/DataUser/Style/Table0.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
    </script>
    
<script src="./Comm/JS/Calendar/WdatePicker.js" type="text/javascript"></script>
<link href="./Comm/JS/Calendar/skin/WdatePicker.css" rel="stylesheet" type="text/css" />

<script language="javascript" type="text/javascript">
    $(function () {
        var screenHeight = document.documentElement.clientHeight;

        var messageHeight = $('#Message').height();
        var topBarHeight = 40;
        var childHeight = $('#childThread').height();
        var infoHeight = $('#flowInfo').height();

        var allHeight = messageHeight + topBarHeight + childHeight + childHeight + infoHeight;


        if ("<%=BtnWord %>" == "2")
            allHeight = allHeight + 30;

        var frmHeight = "<%=Height %>";
        if (screenHeight > parseFloat(frmHeight) + allHeight) {
            $("#divCCForm").height(screenHeight - allHeight);
        }

    });
    function SysCheckFrm() {
    }
    function Change() {
        var btn = document.getElementById('ContentPlaceHolder1_MyFlowUC1_MyFlow1_ToolBar1_Btn_Save');
        if (btn != null) {
            if (btn.value.valueOf('*') == -1)
                btn.value = btn.value + '*';
        }
    }
    var longCtlID = 'ContentPlaceHolder1_MyFlowUC1_MyFlow1_UCEn1_';
    function KindEditerSync() {
        try {
            if (editor1 != null) {
                editor1.sync();
            }
        }
        catch (err) {
        }
    }
    // ccform 为开发者提供的内置函数. 
    // 获取DDL值 
    function ReqDDL(ddlID) {
        var v = document.getElementById(longCtlID + 'DDL_' + ddlID).value;
        if (v == null) {
            alert('没有找到ID=' + ddlID + '的下拉框控件.');
        }
        return v;
    }
    // 获取TB值
    function ReqTB(tbID) {
        var v = document.getElementById(longCtlID + 'TB_' + tbID).value;
        if (v == null) {
            alert('没有找到ID=' + tbID + '的文本框控件.');
        }
        return v;
    }
    // 获取CheckBox值
    function ReqCB(cbID) {
        var v = document.getElementById(longCtlID + 'CB_' + cbID).value;
        if (v == null) {
            alert('没有找到ID=' + cbID + '的单选控件.');
        }
        return v;
    }
    // 获取附件文件名称,如果附件没有上传就返回null.
    function ReqAthFileName(athID) {
        var v = document.getElementById(athID);
        if (v == null) {
            return null;
        }
        var fileName = v.alt;
        return fileName;
    }

    /// 获取DDL Obj
    function ReqDDLObj(ddlID) {
        var v = document.getElementById(longCtlID + 'DDL_' + ddlID);
        if (v == null) {
            alert('没有找到ID=' + ddlID + '的下拉框控件.');
        }
        return v;
    }
    // 获取TB Obj
    function ReqTBObj(tbID) {
        var v = document.getElementById(longCtlID + 'TB_' + tbID);
        if (v == null) {
            alert('没有找到ID=' + tbID + '的文本框控件.');
        }
        return v;
    }
    // 获取CheckBox Obj值
    function ReqCBObj(cbID) {
        var v = document.getElementById(longCtlID + 'CB_' + cbID);
        if (v == null) {
            alert('没有找到ID=' + cbID + '的单选控件.');
        }
        return v;
    }
    // 设置值.
    function SetCtrlVal(ctrlID, val) {
        document.getElementById(longCtlID + 'TB_' + ctrlID).value = val;
        document.getElementById(longCtlID + 'DDL_' + ctrlID).value = val;
        document.getElementById(longCtlID + 'CB_' + ctrlID).value = val;
    }
    //执行分支流程退回到分合流节点。
    function DoSubFlowReturn(fid, workid, fk_node) {
        var url = 'ReturnWorkSubFlowToFHL.aspx?FID=' + fid + '&WorkID=' + workid + '&FK_Node=' + fk_node;
        var v = WinShowModalDialog(url, 'df');
        window.location.href = window.history.url;
    }
    function To(url) {
        //window.location.href = url;
        window.name = "dialogPage"; window.open(url, "dialogPage")
    }

    // 退回，获取配置的退回信息的字段.
    function ReturnWork(url, field) {
        var urlTemp;
        if (field == '' || field == null) {
            urlTemp = url;
        }
        else {
            // alert(field);
            //  alert(ReqTB(field));
            urlTemp = url + '&Info=' + ReqTB(field);
        }
        window.name = "dialogPage"; window.open(urlTemp, "dialogPage")
    }



    function WinOpen(url, winName) {
        var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
        newWindow.focus();
        return;
    }
    function DoDelSubFlow(fk_flow, workid) {
        if (window.confirm('您确定要终止进程吗？') == false)
            return;
        var url = 'Do.aspx?DoType=DelSubFlow&FK_Flow=' + fk_flow + '&WorkID=' + workid;
        WinShowModalDialog(url, '');
        window.location.href = window.location.href; //aspxPage + '.aspx?WorkID=';
    }
    function Do(warning, url) {
        if (window.confirm(warning) == false)
            return;
        window.location.href = url;
    }
    //设置底部工具栏
    function SetBottomTooBar() {
        var form;
        //窗口的可视高度 
        var windowHeight = document.all ? document.getElementsByTagName("html")[0].offsetHeight : window.innerHeight;
        var pageHeight = Math.max(windowHeight, document.getElementsByTagName("body")[0].scrollHeight);
        form = document.getElementById('divCCForm');

        //        if (form) {
        //            if (pageHeight > 20) pageHeight = pageHeight - 20;
        //            form.style.height = pageHeight + "px";
        //        }
        //设置toolbar
        var toolBar = document.getElementById("bottomToolBar");
        if (toolBar) {
            document.getElementById("bottomToolBar").style.display = "";
        }
    }

    window.onload = function () {
        SetBottomTooBar();

    };
</script>
<style type="text/css">
    .Bar
    {
        width: 500px;
        text-align: center;
        float: left;
    }
    #divCCForm
    {
        position: relative !important;
        margin-left:-60px;
    }
</style>


</head>
<body>
    <form id="form1" runat="server">
    <div>
        

        

<div style="width: 0px; height: 0px">
    <object id="LODOP_OB" classid="clsid:2105C259-1E0C-4534-8141-A753534CB4CA" width="0"
        height="0">
        <embed id="LODOP_EM" type="application/x-print-lodop" width="0" height="0" pluginspage="/DataUser/PrintTools/install_lodop32.exe"></embed>
    </object>
</div>
<div id="tabForm" style="<%=Width %>px; margin: 0 auto;">
</div>
<div style="margin: 0; padding: 0; width:auto" id="D">
    <uc3:ToolBar ID="ToolBar1" runat="server" />
    <div style="width: <%=Width %>px;" class="flowInfo" id="flowInfo">
        <uc1:Pub ID="Pub1" runat="server" />
    </div>
    <div style="width: <%=Width %>px;" class="Message" id='Message'>
        <uc1:Pub ID="FlowMsg" runat="server" />
    </div>
    <div style="width: <%=Width %>px;" class="childThread" id='childThread'>
        <uc1:Pub ID="Pub3" runat="server" />
    </div>
    <uc2:UCEn ID="UCEn1" runat="server" />
    <div style="width: <%=Width %>px;" class="pub2Class">
        <uc1:Pub ID="Pub2" runat="server" />
    </div>
    <div id="bottomToolBar" style="<%=Width %>px; text-align: left; display: none;" class="Bar">
        <uc3:ToolBar ID="ToolBar2" runat="server" />
    </div>
</div>


    </div>
    </form>
</body>
</html>
