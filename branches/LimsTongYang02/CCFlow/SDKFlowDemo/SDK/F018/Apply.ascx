<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Apply.ascx.cs" Inherits="CCFlow.SDKFlowDemo.SDK.F018.Apply" %>
<%@ Register Src="../../../WF/SDKComponents/Toolbar.ascx" TagName="Toolbar" TagPrefix="uc1" %>

<link href="../../../WF/Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
<link href="../../../WF/Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
<script src="../../../WF/Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
<script src="../../../WF/Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
<script src="../../../WF/Scripts/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
<script src="../../../WF/SDKComponents/Base/SDKData.js" type="text/javascript"></script>

<script type="text/javascript">

    var doType = "";
    var paras = "";

    function Send() {
        if (confirm("确定要发送吗？") == false)
            return;
        doType = "Send";
        LoadFrm();
        //表单提交.
        $('#form1').submit();
        // LockPage();
    }
    function Save() {
        doType = "Save";
        LoadFrm();
        //表单提交.
        $('#form1').submit();
        // LockPage();
    }
    //锁定页面功能.
    function LockPage() {
        if (doType != "Send")
            return;
        var arrObj = document.all;
        for (var i = 0; i < arrObj.length; i++) {
            if (typeof arrObj[i].type == "undefined")
                continue;

            if (arrObj[i].type == "button")
                arrObj[i].disabled = 'disabled';

            if (arrObj[i].type == "text")
                arrObj[i].disabled = 'disabled';
        }
    }

    function LoadFrm() {
        GetParas();
        $('#form1').form({
            url: "Serv18.ashx?DoType=" + doType + paras,
            data: paras,
            onSubmit: function () {
            },
            success: function (data) {
                //                window.parent.Reflash();
                ShowMsg('发送成功', data);
            }
        });
    }

    //生成url Para.
    function GetParas() {
        paras = "";
        //获取其他参数
        var sHref = window.location.href;
        var args = sHref.split("?");
        var retval = "";
        if (args[0] != sHref) /*参数不为空*/
        {
            var str = args[1];
            args = str.split("&");
            for (var i = 0; i < args.length; i++) {
                str = args[i];
                var arg = str.split("=");
                if (arg.length <= 1)
                    continue;
                //不包含就添加
                if (paras.indexOf(arg[0]) == -1) {
                    paras += "&" + arg[0] + "=" + arg[1];
                }
            }
        }
    }
    function ShowMsg(title, msg) {
        $('#Msg').dialog(
        {
            closeable: false,
            title: title,
            modal: true,
            width: 500,
            height: 300,
            content: msg,
            buttons: [{ text: '关闭', handler: function () {
                $('#Msg').dialog("close");
            }
            }]
        }
        );
    }
</script>
<div id="WFInfo">
</div>
<div id="Msg" style="font-size:15px;"/>

<table border="1" width="90%">
<tr>
    <td colspan="3" valign="top">
    <uc1:Toolbar ID="Toolbar1" runat="server" /></td>
</tr>

<tr>
<td>请假人帐号:</td>
<td>
    <asp:TextBox ID="TB_No" runat="server" ></asp:TextBox>
    </td>
<td>当前登录人登录帐号BP.Web.WebUser.No</td>
</tr>


<tr>
<td>请假人名称:</td>
<td>
    <asp:TextBox ID="TB_Name" runat="server" > </asp:TextBox>
    </td>
<td>当前登录人名称BP.Web.WebUser.Name</td>
</tr>


<tr>
<td>请假人部门编号:</td>
<td>
    <asp:TextBox ID="TB_DeptNo" runat="server"  ></asp:TextBox>
    </td>
<td>当前登录人部门编号BP.Web.WebUser.FK_Dept</td>
</tr>

<tr>
<td>请假人部门名称:</td>
<td>
    <asp:TextBox ID="TB_DeptName" runat="server"  ></asp:TextBox>
    </td>
<td>当前登录人部门名称BP.Web.WebUser.FK_DeptName</td>
</tr>


<tr>
<td>请假天数:</td>
<td>
    <asp:TextBox ID="TB_QingJiaTianShu" runat="server" Text="0"  ></asp:TextBox>
    </td>
<td></td>
</tr>

<tr>
<td>请假原因:</td>
<td>
    <asp:TextBox ID="TB_QingJiaYuanYin" runat="server"></asp:TextBox>
    </td>
<td></td>
</tr>
<%Int64 fk_node = Int64.Parse(this.Request.QueryString["FK_Node"]);
  if (fk_node != 1801)
  { %> 
  <tr>
<td>部门经理审批意见:</td>
<td>
    <asp:TextBox ID="TB_NoteBM" runat="server"></asp:TextBox>
    </td>
<td></td>
</tr>
  <%}
    if (fk_node !=1801 && fk_node!=1802)
  { %> 
  <tr>
<td>总经理审批意见:</td>
<td>
    <asp:TextBox ID="TB_NoteZJL" runat="server"></asp:TextBox>
    </td>
<td></td>
</tr>
  <%}
      if (fk_node != 1801 && fk_node != 1802 && fk_node!=1803)
  { %> 
  <tr>
<td>人力资源审批意见:</td>
<td>
    <asp:TextBox ID="TB_NoteRL" runat="server"></asp:TextBox>
    </td>
<td></td>
</tr>
  <%}
   %>
</table>