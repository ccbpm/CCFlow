<%@ Page Language="C#" MasterPageFile="../SDKComponents/Site.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_Forward_UI" Title="移交" Codebehind="Forward.aspx.cs" %>
<%@ Register Src="../Comm/UC/ToolBar.ascx" TagName="ToolBar" TagPrefix="uc1" %>
<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />

     <script type="text/javascript" >
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
<script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
<script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
<script type="text/javascript">
    function TBHelp(ctrl, enName) {
        var explorer = window.navigator.userAgent;
        var url = "../Comm/HelperOfTBEUI.aspx?EnsName=" + enName + "&AttrKey=" + ctrl + "&WordsSort=2" + "&FK_MapData=" + enName + "&id=" + ctrl;
        var str = "";
        if (explorer.indexOf("Chrome") >= 0) {//谷歌的
            window.open(url, "sd", "left=200,height=500,top=150,width=600,location=yes,menubar=no,resizable=yes,scrollbars=yes,status=no,toolbar=no");
        }
        else {//IE,火狐的
            str = window.showModalDialog(url, "sd", "dialogHeight:500px;dialogWidth:600px;dialogTop:150px;dialogLeft:200px;center:no;help:no");
            if (str == undefined) return;
            ctrl = ctrl.replace("Forward", "TB");
            $("*[id$=" + ctrl + "]").focus().val(str);
        }
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   
   
<table border="0" style="width: 100%; height: 100%" align="center">
    <caption>   您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %></caption>
    <tr>
        <td valign="top" style=" text-align:center;border:0px;">

        <center>
<div  style="align:center;"> 
              <table border="0" >
                <tr>
                    <td colspan="1" valign="top"   style="text-align: left; border:0px;">
                        <uc1:ToolBar ID="ToolBar1" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td bgcolor="#FFFFFF" style="text-align: left;border:0px;" valign="top">
                        <uc2:Pub ID="Top" runat="server" />
                    </td>
                </tr>
            </table>
            </div>
            </center>
           
        </td>
    </tr>
</table>

</asp:Content>
