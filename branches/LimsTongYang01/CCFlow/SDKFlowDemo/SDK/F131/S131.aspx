<%@ Page Title="" Language="C#" MasterPageFile="~/WF/SDKComponents/Site.Master" AutoEventWireup="true" CodeBehind="S131.aspx.cs" Inherits="CCFlow.SDKFlowDemo.SDK.F131.S131" %>
<%@ Register src="../../../WF/SDKComponents/Toolbar.ascx" tagname="Toolbar" tagprefix="uc1" %>
<%@ Register src="../../../WF/SDKComponents/FrmCheck.ascx" tagname="FrmCheck" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../../WF/Comm/Style/Table.css" rel="stylesheet" 
        type="text/css" />
    <link href="../../../WF/Comm/Style/Table0.css" rel="stylesheet" 
        type="text/css" />

        <script type="text/javascript">
            var btnSave = document.getElementsByName('Save');
            var btnSend = document.getElementsByName('Send');
           // document.getElementsByName('Save').style.display ='none';
            //alert(btnSave);
           // alert(btnSave.style.display);
          //  btnSave.display = "none";
//            var btnSend = document.getElementsByName('ContentPlaceHolder1_BtnSend');
//            btnSend.style.visibility = "visible";

            function Save() {
                btnSave.onclick();
            }
            function Send() {
                btnSend.onclick();
            }
        </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style=" text-align:left; width:90% " >
<tr>
<td class="ToolBar">
    <asp:Button text="发送" runat="server" ID="BtnSend"   name="Send"
        onclick="Btn_Send_Click"/>
    <asp:Button text="保存" runat="server" ID="BtnSave" name="Save"
        onclick="Btn_Save_Click" />

    <uc1:Toolbar ID="Toolbar1" runat="server" />
    </td>
</tr>

<tr>
<td>请假申请单</td>
</tr>

<tr>
<td></td>
</tr>


<tr>
<th>审核信息</th>
</tr>

<tr>
<td>
    <uc2:FrmCheck ID="FrmCheck1" runat="server" />
    </td>
</tr>

</table>


</asp:Content>
