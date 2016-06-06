<%@ Page Title="发送后转向处理规则" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master"
    AutoEventWireup="true" CodeBehind="TurnToDeal.aspx.cs" Inherits="CCFlow.WF.Admin.AttrNode.TurnToDeal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function rb_onchange(obj) {
            if (obj.checked = true) {
                if (obj.value == "RB_SpecMsg") {
                    document.getElementById("<%=TB_SpecURL.ClientID %>").value = "";
                    document.getElementById("<%=TB_SpecURL.ClientID %>").readOnly = true;
                    document.getElementById("<%=TB_SpecMsg.ClientID %>").readOnly = false;
                } else if (obj.value == "RB_SpecUrl") {
                    document.getElementById("<%=TB_SpecMsg.ClientID %>").value = "";
                    document.getElementById("<%=TB_SpecMsg.ClientID %>").readOnly = true;
                    document.getElementById("<%=TB_SpecURL.ClientID %>").readOnly = false;
                } else {
                    document.getElementById("<%=TB_SpecMsg.ClientID %>").value = "";
                    document.getElementById("<%=TB_SpecMsg.ClientID %>").readOnly = true;
                    document.getElementById("<%=TB_SpecURL.ClientID %>").value = "";
                    document.getElementById("<%=TB_SpecURL.ClientID %>").readOnly = true;
                }
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 100%">
        <caption>
            发送后转向处理规则
            <tr>
                <td valign="top">
                    <fieldset>
                        <legend>
                            <asp:RadioButton GroupName="TurnToDeal" ID="RB_CCFlowMsg" runat="server" Text="提示ccflow默认信息"
                                onClick="rb_onchange(this)" /></legend>
                        <ul style="color: Gray">
                            <li>默认为不设置，按照机器自动生成的语言提示，这是标准的信息提示。</li>
                            <li>比如：您的当前的工作已经处理完成。下一步工作自动启动，已经提交给xxx处理。 </li>
                        </ul>
                    </fieldset>
                    <fieldset>
                        <legend>
                            <asp:RadioButton ID="RB_SpecMsg" GroupName="TurnToDeal" runat="server" Text="提示指定信息"
                                onClick="rb_onchange(this)" />
                        </legend><a href="javascript:ShowHidden('SpecMsg')">输入提示的信息:</a>
                        <div id="SpecMsg" style="display: none">
                            <ul style="color: Gray">
                                <li>按照您定义的信息格式，提示给已经操作完成的用户。</li>
                                <li>比如：您的申请已经发送至XXX用户进行审批。 </li>
                                <li>该自定义信息支持ccbpm的表达式，具体可参考右侧帮助文档。 </li>
                            </ul>
                        </div>
                        <asp:TextBox ID="TB_SpecMsg" runat="server" TextMode="MultiLine" Rows=3  Width="95%"></asp:TextBox>
                    </fieldset>
                    <fieldset>
                        <legend>
                            <asp:RadioButton ID="RB_SpecUrl" GroupName="TurnToDeal" runat="server" Text="转向指定的URL"
                                onClick="rb_onchange(this)" />
                        </legend><a href="javascript:ShowHidden('SpecUrl')">输入提示的信息:</a>
                        <div id="SpecUrl" style="display: none">
                            <ul style="color: Gray">
                                <li>按照您定义的url转向，可处理较为复杂的业务逻辑处理。</li>
                                <li>比如：URL为MyFlow.aspx页面或www.baidu.com。 </li>
                                <li>该URL支持ccbpm参数形式，具体传值参考右侧帮助。 </li>
                            </ul>
                        </div>
                        <asp:TextBox ID="TB_SpecURL" runat="server" Width="95%"></asp:TextBox>
                    </fieldset>
                </td>
                <td valign="top">
                    <fieldset>
                        <legend>帮助</legend>
                        <ul style="color: Gray">
                            <li><a href="http://ccbpm.mydoc.io/?v=5404&t=17914" target="_blank">提示ccflow默认信息</a></li>
                            <li><a href="http://ccbpm.mydoc.io/?v=5404&t=17914" target="_blank">提示指定信息</a></li>
                            <li><a href="http://ccbpm.mydoc.io/?v=5404&t=17914" target="_blank">转向指定的URL</a></li>
                        </ul>
                    </fieldset>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Button ID="Btn_Save" runat="server" Text="保存" OnClick="Btn_Save_Click" />
                </td>
            </tr>
    </table>
</asp:Content>
