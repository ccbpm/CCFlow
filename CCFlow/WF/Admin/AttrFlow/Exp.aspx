<%@ Page Title="导出" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true"
    CodeBehind="Exp.aspx.cs" Inherits="CCFlow.WF.Admin.AttrFlow.Exp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%
        BP.WF.Flow fl = new BP.WF.Flow(this.Request.QueryString["FK_Flow"]);
    %>
    <table style="width: 100%">
        <caption>
            流程[
            <%=fl.Name%>
            ]模版导出
        </caption>
        <tr>
            <td valign="top" style="width: 30%;">
                <fieldset>
                    <legend>
                        <img src="../../Img/Btn/Help.gif" />关于流程模版</legend>
                    <ol>
                        <li>ccbpm生成的流程模版是一个特定格式的xml文件。</li>
                        <li>它是流程引擎模版与表单引擎模版的完整的组合体。</li>
                        <li>ccbpm的jflow与ccflow的流程引擎导出的流程模版通用。</li>
                        <li>流程模版用于流程设计者的作品交换。</li>
                        <li>在实施的过程中，我们可以把一个系统上的流程模版导入到另外一个系统中去。</li>
                    </ol>
                </fieldset>
                <fieldset>
                    <legend>
                        <img src="../../Img/Btn/Help.gif" />关于流程模版云</legend>
                    <ol>
                        <li>ccbpm团队为各位爱好者提供了云储存</li>
                        <li>它是流程引擎模版与表单引擎模版的完整的组合体。</li>
                        <li>ccbpm的jflow与ccflow的流程引擎导出的流程模版通用。</li>
                        <li>流程模版用于流程设计者的作品交换。</li>
                        <li>在实施的过程中，我们可以把一个系统上的流程模版导入到另外一个系统中去。</li>
                    </ol>
                </fieldset>
            </td>
            <td valign="top">
                <fieldset>
                    <legend>下载到本机 </legend>
                    <ul>
                        <li>我们已经为您生成了流程模版文件，<a href="javascript:WinOpen('/WF/Admin/XAP/DoPort.htm?DoType=ExpFlowTemplete&FK_Flow=<%=fl.No%>&Lang=CH');">请点击这里下载到本机</a>。
                        </li>
                        <li>该xml格式的流程模版文件可以通过，软盘交换到其它ccbpm系统中去。</li>
                    </ul>
                </fieldset>
                <fieldset>
                    <legend>
                        <img src="../CCBPMDesigner/Img/FlowPublic.png" />共享到共有云服务器</legend>
                    <%
                        
                        BP.WF.CloudWS.WSSoapClient ccflowCloud = BP.WF.Cloud.Glo.GetSoap();

                        bool isExitUser = false;

                        try
                        {
                            string userNo = BP.WF.Cloud.CCLover.UserNo;
                        }
                        catch (Exception)
                        {
                            isExitUser = true;//不存在用户,需要注册
                        }
                        try
                        {
                            ccflowCloud.GetNetState();

                            if (isExitUser)
                            {
                                this.BtnPub.Enabled = false;
                    %>
                    <div style='color: red; margin-top: 10px; margin-bottom: 10px'>
                        请先注册ccbpm私有云账号<a href='javascript: window.parent.closeTab("用户注册");window.parent.addTab("RegUser", "用户注册", "../../../WF/Admin/Clound/RegUser.aspx","");'>注册</a><div />
                        <%}
                            else
                            {%>
                        <ul>
                            <li>感谢您共享该文件.</li>
                            <li>导出到公有云的流程模版需要经过审核才能发布。 </li>
                            <li>选择模版类别：<asp:DropDownList ID="DropDownList1" runat="server" Width="214px">
                            </asp:DropDownList>
                            </li>
                        </ul>
                        <%}%>
                        <%}
                        catch (Exception)
                        {
                        %>
                        <div style='color: red; margin-top: 10px; margin-bottom: 10px'>
                            您没有连接到互联网,请连接互联网可以共享到ccbpm的服务器上 点击<a href='javascript:window.location.reload();'>刷新</a><div />
                            <%}
                            %>
                            <asp:Button ID="BtnPub" runat="server" Text="保存至共有云" OnClick="BtnPub_Click" />
                </fieldset>
                <fieldset>
                    <legend>
                        <img src="../CCBPMDesigner/Img/FlowPrivate.png" />上传到私云服务器</legend>
                    <%
                        try
                        {
                            ccflowCloud.GetNetState();

                            if (isExitUser)
                            {
                                this.BtnPri.Enabled = false;
                    %>
                    <div style='color: red; margin-top: 10px; margin-bottom: 10px'>
                        请先注册ccbpm私有云账号<a href='javascript: window.parent.closeTab("用户注册");window.parent.addTab("RegUser", "用户注册", "../../../WF/Admin/Clound/RegUser.aspx","");'>注册</a><div />
                        <%}
                            else
                            {%>
                        <ul>
                            <li>您可以把此模版放入到您的私有云里，我们会好好的为您永久的保管。.</li>
                            <li>选择模版类别：<asp:DropDownList ID="DropDownList2" runat="server" Width="214px">
                            </asp:DropDownList>
                            </li>
                        </ul>
                        <%}%>
                        <%}
                        catch (Exception)
                        {
                        %>
                        <div style='color: red; margin-top: 10px; margin-bottom: 10px'>
                            您没有连接到互联网,请连接互联网可以共享到ccbpm的服务器上 点击<a href='javascript:window.location.reload();'>刷新</a><div />
                            <%}
                            %>
                            <asp:Button ID="BtnPri" runat="server" Text="保存至私有云" OnClick="BtnPri_Click" />
                </fieldset>
            </td>
    </table>
</asp:Content>
