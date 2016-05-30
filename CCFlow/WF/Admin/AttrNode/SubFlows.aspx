<%@ Page Title="节点调用父子流程" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master"
    AutoEventWireup="true" CodeBehind="SubFlows.aspx.cs" Inherits="CCFlow.WF.Admin.AttrNode.SubFlows" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../CCBPMDesigner/normalize/css/demo.css" />
    <link rel="stylesheet" type="text/css" href="../CCBPMDesigner//normalize/css/ns-default.css" />
    <link rel="stylesheet" type="text/css" href="../CCBPMDesigner//normalize/css/ns-style-bar.css" />
    <script type="text/javascript" src="../CCBPMDesigner/normalize/js/modernizr.custom.js"></script>
  
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
        }
        .btn_Save
        {
            cursor: pointer;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%
        int nodeid = int.Parse(this.Request.QueryString["FK_Node"]);
        BP.WF.Node nd = new BP.WF.Node(nodeid);
        
    %>
    <table style="width: 100%; min-width: 800px;">
        <caption>
            <img src="../CCBPMDesigner/Img/Menu/SubFlows.png" border="0" alt="子流程" />节点【<%=nd.Name %>】调用父子</caption>
        <tr>
            <td valign="top"  style=" width:20%;color:Gray">
                <fieldset>
                    <legend>父子流程定义</legend>
                    <ul>
                        <li>一些业务需求，需要多个流程协同才能完成，他们之间形成一个流程调用另外一个流程的情况，我们称为父子流程。</li>
                        <li>被吊起的成为子流程，吊起自称的节点叫做父流程节点，在子流程上，也可能吊起子流程，相对于这个当前流程来说，是子子流程(孙子流程)，ccbpm只研究两两关系，不研究跨代关系。</li>
                        <li>被吊起的成为子流程，吊起自称的节点叫做父流程节点。</li>
                    </ul>
                </fieldset>
                <fieldset>
                    <legend>关于父子流程控件</legend>
                    <ul>
                        <li>该控件的位置位于：/WF/SDKComponents/SubFlowDtl.ascx </li>
                        <li>如果您使用SDK的模式开发，仅仅需要把该控件拖入您的页面就可以了，而该控件的属性需要在右边配置。</li>
                        <li>如果您在节点表单里启用了该控件，您可以在这里设置他的属性。</li>
                    </ul>
                </fieldset>
            </td>
            <td style="width: 50%;" valign="top">

            <fieldset>
            <legend><a href="javascript:ShowHidden('state')">控件状态: </a> </legend>
            <div id="state" style=" display:none;color:Gray">
            <ul>
            <li>禁用:不使用控件，在表单上不可见。  </li>
            <li>启用:在表单上正常工作。</li>
            <li>只读:表单上可见，但是不能操作。</li>
            </ul>
            </div>

              <asp:RadioButton ID="RB_Disable" Text="禁用" GroupName="SFSta" runat="server" />
              <asp:RadioButton ID="RB_Enable" Text="启用" GroupName="SFSta" runat="server" />
              <asp:RadioButton ID="RB_Readonly" Text="只读" GroupName="SFSta" runat="server" />
            </fieldset>

             <fieldset>
            <legend><a href="javascript:ShowHidden('style')">显示方式:</a> </legend>

            
            <div id="style" style="display:none;color:Gray">
            <ul>
            <li>目前仅仅支持表格方式. </li>
            <li>我们会在以后开发更多的风格支持，满足不同的用户需要. </li>
            </ul>
            </div>


            <asp:RadioButton ID="RB_Table" Text="表格方式" GroupName="SFShowModel" runat="server" />
             <asp:RadioButton ID="RB_Free" Text="自由方式" GroupName="SFShowModel" runat="server" />
            </fieldset>

              <fieldset>
            <legend><a href="javascript:ShowHidden('title')">标题(显示控件头部):</a></legend>
             
            <div id="title" style=" display:none;color:Gray">
            <ul>
             <li>该文字显示到父子流程控件的头部，用来提示该控件的作用。</li>
             <li>默认为空。 </li>
            </ul>
            </div>

            <asp:TextBox ID="TB_SFCaption"  runat="server" Width="95%"></asp:TextBox>
            </fieldset>

          <fieldset>
            <legend><a href="javascript:ShowHidden('xx')">可手工启动的子流程</a> </legend>
            <div id="xx" style=" display:none;color:Gray">
            设置可以手工启动的流程编号:
            <ul>
            <li>单个流程设置格式为:001  </li>
            <li>多个流程用逗号分开比如: 001,002  </li>
            </ul>
            </div>
            
            <asp:TextBox ID="TB_SFDefInfo"  runat="server" Width="95%" ToolTip="节点编号,例:101,102..."></asp:TextBox>

            <br />
            <a href="../ConditionSubFlow.aspx?FK_Node=<%=nodeid %>&FK_Flow=<%=nd.FK_Flow %>" target=_blank >触发条件设置</a>
            </fieldset>

            <fieldset>
            <legend>控件的显示控制</legend>
            高度(设置0标识为100%): <asp:TextBox ID="SF_H"  runat="server" ToolTip="只可以输入数字"></asp:TextBox>
            <br />
            宽度(设置0标识为100%):<asp:TextBox ID="SF_W"  runat="server" ToolTip="只可以输入数字"></asp:TextBox>
            </fieldset>

            <asp:Button ID="BtnSave" runat="server" class="btn_Save" Text="保存" OnClick="BtnSave_Click" />
              
            </td>
        </tr>
    </table>
</asp:Content>
