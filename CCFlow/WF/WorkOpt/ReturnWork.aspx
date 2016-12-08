<%@ Page Title="工作退回" Language="C#" MasterPageFile="../SDKComponents/Site.Master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_ReturnWorkSmall" Codebehind="ReturnWork.aspx.cs" %>
<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<%@ Register Src="./../Comm/UC/ToolBar.ascx" TagName="ToolBar" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="JavaScript" src="../Comm/JScript.js" type="text/javascript"></script>
    <script type="text/javascript">
        function NoSubmit(ev) {
            if (window.event.srcElement.tagName == "TEXTAREA")
                return true;
            if (ev.keyCode == 13) {
                window.event.keyCode = 9;
                ev.keyCode = 9;
                return true;
            }
            return true;
        }
</script>
    <link href="../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
<script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
<script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
<script type="text/javascript">
    function OnChange(ctrl) {
        var text = ctrl.options[ctrl.selectedIndex].text;
        var user = text.substring(0, text.indexOf('='));
        var nodeName = text.substring(text.indexOf('>') + 1, 1000);
        var objVal = '您好' + user + ':';
        objVal += "  \t\n ";
        objVal += "  \t\n ";
        objVal += "   您处理的 “" + nodeName + "” 工作有错误，需要您重新办理． ";
        objVal += "\t\n   \t\n 礼! ";
        objVal += "  \t\n ";

        try {
            document.getElementById("<%=TBClientID %>").value = objVal;
        } catch (e) {
        }
    }

    function TBHelp(ctrl, enName) {
        var explorer = window.navigator.userAgent;
        var url = "../Comm/HelperOfTBEUI.aspx?EnsName=" + enName + "&AttrKey=" + ctrl + "&WordsSort=1" + "&FK_MapData=" + enName + "&id=" + ctrl;
        var str;
        if (explorer.indexOf("Chrome") >= 0) {
            window.open(url, "sd", "left=200,height=500,top=150,width=600,location=yes,menubar=no,resizable=yes,scrollbars=yes,status=no,toolbar=no");
        } else {
            str = window.showModalDialog(url, "sd", "dialogHeight:500px;dialogWidth:600px;dialogTop:150px;dialogLeft:200px;center:no;help:no");
            if (str == undefined)
                return;
            ctrl = ctrl.replace("Return", "TB");
            $("*[id$=" + ctrl + "]").focus().val(str);
        }
    }
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

 <table style=" text-align:left; width:100%">
<caption>您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %></caption>
<tr>
<td style=" text-align:center"> 

<center>
<div  style="align:center;"> 
<table >
<tr>
<td style=" border:0px;">
      <uc3:ToolBar ID="ToolBar1" runat="server" />
    </td>
</tr>
<tr>
    <td  style=" border:0px;">
     <uc1:Pub ID="Pub1" runat="server" />
    </td>
</tr>
</table>
</div>
</center>

</td>
</tr>
</table>

</asp:Content>






