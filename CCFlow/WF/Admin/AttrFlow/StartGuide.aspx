<%@ Page Title="" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" CodeBehind="StartGuide.aspx.cs" Inherits="CCFlow.WF.Admin.AttrFlow.StartGuide1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width:98%">
        <caption>发起前置导航</caption>
        <tr>
            <td valign="top" style="width: 20%;">
                <fieldset>
                    <legend>帮助</legend>

                    <ul>
                        <li>我们经常会遇到用户发起流程前，首先进入一个实体列表界面（比如项目列表，成员列表、供应商列表。），选择一个实体后把该实体的信息带入开始节点的表单字段里，我们把这个应用场景叫做发起前置导航方式。发起前置导航方式有如下几种应用场景，开发者根据需要进行配置。</li>
                        <p />
                        <li>比如：流程发起前，先列出所有纳税人列表，用户选中一条，会将纳税人信息直接填充到表单内。</li>

                    </ul>
                </fieldset>
            </td>

            <td valign="top">
                <fieldset>
                    <legend>
                        <asp:RadioButton ID="RB_None" Text="无（默认）" GroupName="xzgz" runat="server" Checked="true" /></legend>
                    <font color="gray">不设置任何前置导航，发起流程直接进入开始节点表单。</font>
                </fieldset>

                <%--  <fieldset>
                        <legend>
                            <asp:RadioButton ID="RB_1" Text="按系统的URL-(父子流程)单条模式" GroupName="xzgz" runat="server" /></legend>
                        参数1：<br />
                        <Textarea id="TB1" runat="server" style="width:100%;height:51px" > </Textarea><br />
                    </fieldset> 
                    <fieldset>
                        <legend>
                            <asp:RadioButton ID="RB_2" Text="按系统的URL-(子父流程)多条模式" GroupName="xzgz" runat="server" /></legend>
                        参数1：<br />

                        <Textarea id="TB4" runat="server" style="width:100%;height:51px"></Textarea><br />
                    </fieldset>--%>

                <fieldset>
                    <legend>
                        <asp:RadioButton ID="RB_ByHistoryUrl" Text="从开始节点Copy数据(查询历史记录)" runat="server" GroupName="xzgz" /></legend>
                      
                    <a href="javascript:ShowHidden('ByHistoryUrl')">请设置SQL</a>: 
                    <div id="ByHistoryUrl" style="display:none;color:gray">
                    <ul>
                    <li>用户希望出现一个历史发起的流程列表，选择一条流程并把该流程的数据copy到新建的流程上。</li>
                    <li>您需要在这里配置一个SQL, 并且该SQL必须有一个OID列。</li>
                    <li>比如：SELECT  Title ,OID FROM ND1001 WHERE No LIKE '%@key%' OR Name LIKE '%@key%'</li>
                    </ul>
                    </div>
                    
                    <Textarea id="TB_ByHistoryUrl" runat="server" style="width:98%; height: 24px"></Textarea><br />
                </fieldset>


                <fieldset>
                    <legend>
                        <asp:RadioButton ID="RB_SelfUrl" Text="按自定义的Url" runat="server" GroupName="xzgz" /></legend>
                    
                    <a href="javascript:ShowHidden('ByUrl')">请设置URL:</a>
                    <div id="ByUrl" style="display:none;color:gray">
                   
                    <ul>
                    <li>请设置URL在下面的文本框里。</li>
                    <li>该URL是一个列表，在每一行的数据里有一个连接链接到工作处理器上（/WF/MyFlow.aspx）</li>
                    <li>连接到工作处理器（ WF/MyFlow.aspx）必须有2个参数FK_Flow=xxx&IsCheckGuide=1</li>
                    <li>您可以打开Demo: /SDKFlowDemo/TestCase/StartGuideSelfUrl.aspx 详细的说明了该功能如何开发。</li>
                    </ul>
                   
                    </div>

                    <Textarea id="TB_SelfURL" runat="server" style="width:98%; height: 24px"></Textarea><br />
                </fieldset>

                <fieldset>
                    <legend>
                        <asp:RadioButton ID="RB_BySQLOne" Text="按设置的SQL-单条模式" runat="server" GroupName="xzgz" /></legend>
                    
                    <a href="javascript:ShowHidden('Paras1')">查询参数:</a>
                    <div id="Paras1" style="display:none;color:gray">
                    <ul >
                      <li>比如:SELECT No, Name, No as EmpNo,Name as EmpName,Email FROM WF_Emp WHERE No LIKE '%@key%' </li>
                      <li>初始化列表参数，该查询语句必须有No,Name两个列，注意显示数量限制。</li>
                      <li>很多场合下需要用到父子流程，在启动子流程的时候需要选择一个父流程。</li>
                      <li>实例:SELECT a.WorkID as No, a.Title as Name, a.Starter, a.WorkID As PWorkID, '011' as PFlowNo, a.FK_Node as PNodeID FROM WF_GenerWorkflow a, WF_GenerWorkerlist b WHERE A.WorkID=b.WorkID  AND B.FK_Emp='@WebUser.No' AND B.IsPass=0 AND A.FK_Flow='011' AND a.Title Like '%@Key%'</li>
                    </ul>
                    </div>

                    <Textarea id="TB_BySQLOne1"  rows="3"  runat="server" style="width:98%; height: 51px"></Textarea><br />

                     <a href="javascript:ShowHidden('ByParas2')">初始化列表参数:</a>
                    <div id="ByParas2" style="display:none;color:gray">
                    <ul>
                     <li>比如:SELECT top 15 No,Name ,No as EmpNo,Name as EmpName ,Email FROM WF_Emp  </li>
                    <li>或者:SELECT  No,Name ,No as EmpNo,Name as EmpName ,Email FROM WF_Emp WHERE ROWID < 15  </li>
                    <li>该数据源必须有No,Name两个列, 其他的列要与开始节点表单字段对应。</li>
                    <li>注意查询的数量，避免太多影响效率。</li>
                    </ul>
                    </div>
 
                    </font><Textarea id="TB_BySQLOne2" rows="3"  runat="server" style="width:98%; height: 51px"></Textarea><br />
                </fieldset>

                 <fieldset>
                    <legend>
                        <asp:RadioButton ID="RB_SubFlow" Text="子流程实例列表模式-多条" runat="server" GroupName="xzgz" /></legend>
                    
                    <a href="javascript:ShowHidden('SubFlowParas1')">查询参数:</a>
                    <div id="SubFlowParas1" style="display:none;color:gray">
                    <ul >
                      <li>比如:SELECT No, Name, No as EmpNo,Name as EmpName,Email FROM WF_Emp WHERE No LIKE '%@key%' </li>
                      <li>初始化列表参数，该查询语句必须有No,Name两个列，注意显示数量限制。</li>
                      <li>很多场合下需要用到父子流程，在启动子流程的时候需要选择一个父流程。</li>
                      <li>实例:SELECT a.WorkID as No, a.Title as Name, a.Starter, a.WorkID As PWorkID, '011' as PFlowNo, a.FK_Node as PNodeID FROM WF_GenerWorkflow a, WF_GenerWorkerlist b WHERE A.WorkID=b.WorkID  AND B.FK_Emp='@WebUser.No' AND B.IsPass=0 AND A.FK_Flow='011' AND a.Title Like '%@Key%'</li>
                    </ul>
                    </div>

                    <Textarea id="TB_SubFlow1"  rows="3"  runat="server" style="width:98%; height: 51px"></Textarea><br />

                    <a href="javascript:ShowHidden('subflow2')">初始化列表参数:</a>
                    <div id="subflow2" style="display:none;color:gray">
                    <ul>
                     <li>比如:SELECT top 15 No,Name ,No as EmpNo,Name as EmpName ,Email FROM WF_Emp  </li>
                    <li>或者:SELECT  No,Name ,No as EmpNo,Name as EmpName ,Email FROM WF_Emp WHERE ROWID < 15  </li>
                    <li>该数据源必须有No,Name两个列, 其他的列要与开始节点表单字段对应。</li>
                    <li>注意查询的数量，避免太多影响效率。</li>
                    </ul>
                    </div>
 
                    </font><Textarea id="TB_SubFlow2" rows="3"  runat="server" style="width:98%; height: 51px"></Textarea><br />
                </fieldset>


                <% 
                    BP.WF.Template.FrmNodes fns=new BP.WF.Template.FrmNodes( int.Parse(  this.Request.QueryString["FK_Flow"] +"01"));
                    if (fns.Count >=2 )
                    {
                  %>
                 <fieldset>
                    <legend>
                        <asp:RadioButton ID="RB_FrmList" Text="开始节点绑定的独立表单列表" runat="server"  GroupName="xzgz" /></legend>
                        <font color="gray">
                        <ul>
                        <li> 流程启动的时候，系统会把流程引擎绑定的开始节点表单列出来让操作员去选择。 </li>
                        <li> 选择一个或者n个表单后，系统就会把参数 Frms 带入到工作处理器里，让工作处理器启用这个表单。 </li>
                        <li> 这种工作方式适应的环境是一个流程可以挂接多个表单。 </li>
                        </ul>
                        </font>
                </fieldset>
                <%} %>
            

            </td>
        </tr>

        <tr>
        <td>
        </td>
            <td>
                <asp:Button class="easyui-linkbutton" ID="Btn_Save" runat="server" Text="保存" OnClick="BtnSave_Click" />
                | <a href="../TestFlow.aspx?FK_Flow=<%=this.FK_Flow %>&SID=<%=BP.Web.WebUser.SID %>&Lang=CH" target="_blank" >运行测试</a>
            </td>

        </tr>
    </table>
</asp:Content>
