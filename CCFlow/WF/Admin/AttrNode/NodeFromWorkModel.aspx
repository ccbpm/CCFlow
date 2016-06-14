<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true"
    CodeBehind="NodeFromWorkModel.aspx.cs" Inherits="CCFlow.WF.Admin.NodeFromWorkModel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Scripts/CommonUnite.js" type="text/javascript"></script>
    <script src="../../Comm/JScript.js" type="text/javascript"></script>
    <style type="text/css">
        .DemoImg
        {
            width: 220px;
            height:100%;
            border: 1px;
            border-color: Blue;
            margin: 0px;
        }
        .DemoImgTD
        {
            margin: 0px;
            vertical-align: top;
        }
    </style>
    <script type="text/javascript">
        function ChangeImg(val) {
            if (val == "0")
                this.BindingImg.src = "./Img/Tree.png";
            else
                this.BindingImg.src = "./Img/Tab.png";
        }
        function SelectImg(val) {
            if (val == "0")
                this.FrmImg.src = "./Img/FreeFrm.png";
            else
                this.FrmImg.src = "./Img/Col4Frm.JPG";
        }
        function SetDDLEnable(ctrl, val) {
            if (val == "enable")
                $("#" + ctrl).attr("disabled", "disabled");
            else
                $("#" + ctrl).removeAttr("disabled");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <% 
        string nodeIDstr = this.Request.QueryString["FK_Node"];
        int nodeID = int.Parse(nodeIDstr);
        BP.WF.Node nd = new BP.WF.Node(nodeID);
        BP.WF.Nodes nds = new BP.WF.Nodes(nd.FK_Flow);
    %>
    <table style="width: 100%;">
        <caption>
            <img src="../CCBPMDesigner/Img/Menu/Form.png" border="0" />节点[<%=nd.Name %>]表单解决方案</caption>
        <!-- =================================== 使用ccbpm内置的节点表单 -->
        <tr>
            <th colspan="2">
                <asp:RadioButton ID="RB_FixFrm" runat="server" Text="使用ccbpm内置的节点表单" GroupName="xxx" />
            </th>
        </tr>
        <tr>
            <td class="DemoImgTD">
                <a href="http://ccbpm.mydoc.io/?v=5404&t=17923" target="_blank">
                    <img src="./Img/FreeFrm.png" id="FrmImg" class="DemoImg" alt="点击放大" />
                </a>
            </td>
            <td>
                <table>
                    <tr>
                        <td>
                            呈现风格：
                        </td>
                        <td>
                            <asp:RadioButton ID="RB_Frm_0" runat="server" Text="自由模式" GroupName="x22xy" />
                            <asp:RadioButton ID="RB_Frm_1" runat="server" Text="傻瓜模式" GroupName="x22xy" />
                        </td>
                        <td>

                            [<a href="javascript:alert('开发中，敬请期待。');WinOpen('../CCFormDesigner/FormDesigner.aspx?FK_MapData=ND<%=nodeIDstr %>');">
                                设计自由表单(Html5)</a>]

                                 [<a href="javascript:WinOpen('../CCFormDesigner/CCFormDesignerSL.aspx?FK_Flow=<%=nd.FK_Flow %>&FK_MapData=ND<%=nodeIDstr %>&UserNo=<%=BP.Web.WebUser.No %>&SID=<%=BP.Web.WebUser.SID%>');">
                                设计自由表单(Silverlight)</a>]

                                  [<a href="javascript:WinOpen('../AttrNode/SortingMapAttrs.aspx?FK_Flow=<%=nd.FK_Flow %>&FK_MapData=ND<%=nodeIDstr %>&UserNo=<%=BP.Web.WebUser.No %>&SID=<%=BP.Web.WebUser.SID%>');">
                                手机表单</a>]
                                 - [<a href="javascript:alert('我们在以后会启用。');">设计傻瓜表单</a>]
                        </td>
                    </tr>
                    <tr>
                        <td>表单引用:</td>
                        <td>
                            <asp:RadioButton ID="RB_CurrentForm" runat="server" Text="当前节点表单" GroupName="xxy" />
                            <asp:RadioButton ID="RB_OtherForms" runat="server" Text="其他节点表单" GroupName="xxy" />
                        </td>
                        <td>
                            <font color="gray">当前节点可以设置与其他节点共用一个表单</font>
                        </td>
                    </tr>
                    <tr>
                        <td>要引用的节点:</td>
                        <td>
                            <asp:DropDownList ID="DDL_Frm" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <font color="gray">对于选择使用【其他节点表单】设置有效. </font>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <a href="javascript:WinOpen('/WF/Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmWorkCheck&PK=<%=nodeID%>')">
                                设置审核组件属性</a>

                               
                        </td>
                        <td colspan="2">
                            <font color="gray">ccbpm为您提供了一个demo流程\\流程树\\表单解决方案\\审核组件流程，使用审核组件可以方便用户设计审批类的流程。</font>
                        </td>
                    </tr>
                    <%--
             <tr>
            <td colspan="2">
             审核组件应用场景  
            <ul>
            <li>在一些流程里，通常都是开始节点填写一个表单，之后的节点做审批，这就用到了审核组件。</li>
            <li>应用步骤:</li>
            </ul>
            <%--，说明：在使用审核组件的模式下，ccbpm允许使用集成表单，比如A，B，C, D四个节点的表单，当前节点表单C,可以使用节点D的表单。  </td>
            </tr>
                    --%>
                </table>
            </td>
        </tr>
        <!-- =================================== 使用 嵌入式表单  -->
        <tr>
            <th colspan="2" class="DemoImgTD">
                <asp:RadioButton ID="RB_SelfForm" runat="server" GroupName="xxx" Text="使用嵌入式表单" />
            </th>
        </tr>
        <tr>
            <td class="DemoImgTD">
                <a href="http://ccbpm.mydoc.io/?v=5404&t=17925" target="_blank">
                    <img src="./Img/SelfFrm.png" class="DemoImg" alt="点击放大" />
                </a>
            </td>
            <td>
                请输入嵌入式表单的URL:
                <asp:TextBox ID="TB_CustomURL" runat="server" Width="424px" Height="20px"></asp:TextBox>
                <br />
                <font color="gray">
                    <ul>
                        <li>该表单必须有javascript 的 Save保存方法，该Save方法里如果需要执行存盘，并验证。</li>
                        <li>如果使用绝对路径可以使用ccbpm的全局变量@SDKFromServHost ，比如: @SDKFromServHost/MyFile.aspx </li>
                        <li>ccbpm团队为您提供了一个嵌入式表单的 demo ，位于:\\流程树\\表单解决方案\\嵌入式表单. </li>
                         <li> <a href="javascript:WinOpen('SDKComponents.aspx?DoType=FrmCheck&FK_Node=<%=nodeID%>')">组件属性</a></li>
                    </ul>
                </font>
            </td>
        </tr>
        <!-- =================================== 使用SDK表单 -->
        <tr>
            <th colspan="2" class="DemoImgTD">
                <asp:RadioButton ID="RB_SDKForm" runat="server" GroupName="xxx" Text="使用SDK表单" />
            </th>
        </tr>
        <tr>
            <td class="DemoImgTD">
                <a href="http://ccbpm.mydoc.io/?v=5404&t=18388" target="_blank">
                    <img src="./Img/SDKFrm.png" class="DemoImg" alt="点击放大" />
                </a>
            </td>
            <td>
                请输入表单的URL:
                <asp:TextBox ID="TB_FormURL" runat="server" Width="424px" Height="20px"></asp:TextBox>
                <br />
                <font color="gray">
                    <ul>
                        <li>SDK表单就是ccbpm把界面的展现完全交给了开发人员处理,开发人员只要设计一个表单,增加一个发送按钮,调用ccbpm的发送API就可以完成</li>
                        <li>如果使用绝对路径可以使用ccbpm的全局变量@SDKFromServHost ，比如: @SDKFromServHost/MyFile.aspx </li>
                        <li>ccbpm团队为您提供了一个demo流程 \\流程树\\SDK流程\\ 该目录下有很多SDK模式的流程供您参考。</li>
                    </ul>
                </font>
            </td>
        </tr>

        <!-- =================================== 绑定多表单 -->
        <tr>
            <th colspan="2" class="DemoImgTD">
                <asp:RadioButton ID="RB_SheetTree" runat="server" GroupName="xxx" Text="绑定多表单" />
            </th>
        </tr>

        <tr>
            <td class="DemoImgTD">
                <a href="http://blog.csdn.net/jflows/article/details/50160423" target="_blank">
                    <img src="./Img/Tree.png" id="BindingImg"   class="DemoImg" alt="点击放大" />
                </a>
            </td>
            <td>
                呈现风格：
                <asp:RadioButton ID="RB_tree" runat="server" Checked=true Text="表单树" GroupName="x22axy" />
                <asp:RadioButton ID="RB_tab" runat="server" Text="Tab标签页" GroupName="x22axy" />
                [<a href="javascript:WinOpen('/WF/Admin/BindFrms.aspx?FK_Node=<%=nd.NodeID%>&FK_Flow=<%=nd.FK_Flow%>&DoType=SelectedFrm')">
                    绑定/取消绑定</a> ] [ <a href="javascript:WinOpen('/WF/Admin/BindFrms.aspx?FK_Node=<%=nd.NodeID%>&FK_Flow=<%=nd.FK_Flow%>')">
                        设置表单字段控件权限</a>]
                <br />
                <font color="gray">
                    <ul>
                        <li>我们把一个节点需要绑定多个表单的节点称为多表单节点，它有两种展现方式，标签页与表单树。</li>
                        <li>对应的流程demo:\\流程树\\表单解决方案\\树形表单与多表单 </li>
                    </ul>
                </font>
            </td>
        </tr>


        
        <!-- =================================== 绑定公文表单 -->
        <tr>
            <th colspan="2" class="DemoImgTD">
                <asp:RadioButton ID="RB_WebOffice" runat="server" GroupName="xxx" Text="公文表单" />
            </th>
        </tr>
        <tr>
            <td class="DemoImgTD">
                <a href="http://blog.csdn.net/jflows/article/details/50160423" target="_blank">
                    <img src="./Img/Doc.png" id="Img1"   class="DemoImg" alt="点击放大" />
                </a>
            </td>
            <td>
                呈现风格：
                <asp:RadioButton ID="RB_WebOffice_Frm2" runat="server" Checked="true" Text="表单在前面" GroupName="RB_Doc" />
                <asp:RadioButton ID="RB_WebOffice_Frm3" runat="server" Text="公文在前面" GroupName="RB_Doc" />

                <ul>
                <li> <a href="javascript:WinOpen('/WF/Admin/BindFrms.aspx?FK_Node=<%=nd.NodeID%>&FK_Flow=<%=nd.FK_Flow%>&DoType=SelectedFrm')">
                    设置附件权限</a>  - 
                    <a href="javascript:WinOpen('/WF/Comm/UIEn.aspx?EnName=BP.WF.Template.BtnLabExtWebOffice&PK=<%=nd.NodeID%>&FK_Flow=<%=nd.FK_Flow%>')" >
                        设置公文按钮权限</a>
                        </li>
                <li>
                         [<a href="javascript:WinOpen('../CCFormDesigner/FormDesigner.aspx?FK_MapData=ND<%=nodeIDstr %>');">
                                设计自由表单(Html5)</a>]

                                 [<a href="javascript:WinOpen('../CCFormDesigner/CCFormDesignerSL.aspx?FK_Flow=<%=nd.FK_Flow %>&FK_MapData=ND<%=nodeIDstr %>&UserNo=<%=BP.Web.WebUser.No %>&SID=<%=BP.Web.WebUser.SID%>');">
                                设计自由表单(Silverlight)</a>]
                                [手机表单]-[设计傻瓜表单]
                          </li>
                </ul>
                <font color="gray">
                    <ul>
                        <li>我们把一个节点需要绑定多个表单的节点称为多表单节点，它有两种展现方式，标签页与表单树。</li>
                        <li>对应的流程demo:\\流程树\\表单解决方案\\树形表单与多表单 </li>
                    </ul>
                </font>
            </td>
        </tr>


        <tr>
            <td colspan="2" class="DemoImgTD">
                <asp:Button class="easyui-linkbutton" ID="Btn_Save" runat="server" Text="保存" OnClick="Btn_Save_Click" />
                <asp:Button class="easyui-linkbutton" ID="Btn_Cancel" runat="server" Text="保存并关闭"
                    OnClick="Btn_Cancel_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
