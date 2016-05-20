<%@ Page Title="" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true"
    CodeBehind="DTSBTable.aspx.cs" Inherits="CCFlow.WF.Admin.AttrFlow.DTSBTable" %>

<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%
        string flowNo = this.Request.QueryString["FK_Flow"];
        if (flowNo == null || flowNo == "")
            flowNo = "001";
        BP.WF.Flow fl = new BP.WF.Flow(flowNo);
    %>
    <table>
        <caption>与业务表数据同步</caption>
        <tr>
            <td valign="top" style="width: 20%; color:Gray">
                <fieldset>
                    <legend>应用场景</legend>
                    <ol>
                        <li>在稍大的应用中，流程系统与业务系统的数据库是分开的。比如：业务流程系统、固定资产系统、客户关系管理系统、财务系统。</li>
                        <li>在固定资产管理系统中，流程审批数据库与固定资产数据库是分开的，一个固定资产的采购申请走完后，需要把该固定资产采购的信息同步到固定资产系统中去，或者一个固定资产的报废需要把该审批结果需要更新固定资产状态。</li>
                        <li>流程走完一个订单审批后，需要把订单的信息同步到仓库管理系统中去。</li>
                    </ol>
                </fieldset>
                <fieldset>
                    <legend>帮助</legend>
                    <ol>
                        <li>ccbpm在运转的过程中会把节点表单的数据存储到ccbpm数据库的数据表 <span style="color: Blue;">[
                            <%=fl.PTable%>] </span>里，这个表的名称可以在流程属性里定义。 </li>
                        <li>流程数据存储表可以自定义，定义路径：流程属性=》 基本属性=》流程数据表。</li>
                        <li>有的应用场景下，会把该表的业务数据同步到其他系统中去，ccbpm提供了事件可以使用编程的方式实现这样的需求。</li>
                        <li>为了更好的适应开发者的需求，我们提供了界面化的定义工具。 </li>
                        <li>该功能可以把流程数据转出到指定的数据库上，指定的数据表里。</li>
                    </ol>
                </fieldset>
            </td>
            <td valign="top" style="text-align: center;">
                <fieldset>
                    <legend>
                        <asp:RadioButton ID="RB_DTSWay0" Text="不执行同步" GroupName="Way22" runat="server" />
                    </legend>
                    <ul style="color:Gray">
                        <li>流程运行的数据存储到<span style="color: Blue;">[<%=fl.PTable %>]</span> ,不与其他系统交换数据。</li>
                        <li>其他系统可以读写这个表<span style="color: Blue;"> [<%=fl.PTable %>] </span>的数据，完成相关的业务操作。</li>
                        <li>该表名<span style="color: Blue;"> [<%=fl.PTable %>] </span>是可以在流程属性配置。</li>
                    </ul>
                </fieldset>
                <fieldset>
                    <legend>
                        <asp:RadioButton ID="RB_DTSWay1" Text="执行同步" GroupName="Way22" runat="server" />
                    </legend>
                    <table style="text-align: center;">
                        <tr>
                            <td>
                                请选择要导出的数据源
                            </td>
                            <td>
                                <asp:DropDownList ID="DDL_DBSrc" runat="server" OnSelectedIndexChanged="DDL_DBSrc_SelectedIndexChanged">
                                </asp:DropDownList>
                                &nbsp;
                                <%--</td>
                            <td>--%>
                                你可以在表单库中, <a href="javascript:OpenDBASource();">
                                    设置数据源</a>。
                                <%--<a href="javascript:WinOpen('../../Comm/Sys/SFDBSrcNewGuide.aspx','_blank');">设置数据源</a>。--%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                指定的表
                            </td>
                            <td>
                                <asp:DropDownList ID="DDL_Table" runat="server" OnSelectedIndexChanged="DDL_Table_SelectedIndexChanged">
                                </asp:DropDownList>
                                &nbsp;
                                <%-- </td>
                            <td>--%>
                                要把数据同步到那个数据表里去？
                            </td>
                        </tr>
                        <tr>
                            <td>
                                同步的计算方式
                            </td>
                            <td>
                                <asp:RadioButton ID="RB_DTSField0" Text="字段名相同" GroupName="Way" runat="server" />
                                <asp:RadioButton ID="RB_DTSField1" Text="按设置字段" GroupName="Way" runat="server" />
                                &nbsp; [<a href="javascript:OpenFields()">设置字段匹配</a>]
                            </td>
                        </tr>
                        <script type="text/javascript">
                        function OpenDBASource(){
                            if(window.parent && window.parent.addTab){
                            window.parent.addTab('dtsbTable','设置数据源','../../Comm/Sys/SFDBSrcNewGuide.aspx','');
                            }else{
                                window.open('../../Comm/Sys/SFDBSrcNewGuide.aspx','newwindow', 'height=600, width=800, top=0, left=0, toolbar=no, menubar=no, scrollbars=yes, resizable=no, location=no, status=no');
                            }
                        }
   //设置字段匹配
   function OpenFields() {
      var dllSrc = document.all.<%= DDL_DBSrc.ClientID %>;
      var src = dllSrc.options[dllSrc.selectedIndex].value; 
    
      var dllTable = document.all.<%= DDL_Table.ClientID %>;
      var tableName = dllTable.options[dllTable.selectedIndex].value; 
      var fk_flow='<%=FK_Flow %>';

      var url='../../Admin/AttrFlow/DTSBTableExt.aspx?FK_Flow='+fk_flow+'&FK_DBSrc=' + src + '&TableName=' + tableName + '&';
      if(window.parent && window.parent.addTab){
        window.parent.addTab('dtsbTable','设置字段匹配',url,'');
      }else{
        window.open(url,'_blank');
      }
   }
   function OpenDTSNodes() {
   var toggleStyle= document.getElementById("dis").style.display;
   if (toggleStyle=="none") {
     document.getElementById("dis").style.display="block";
      }else {
     document.getElementById("dis").style.display="none";
}
   }
function WinOpen(url, winName) {
    var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
    newWindow.focus();
    return;
}
   //必须初始化
   window.onload=function(){
      checkNodes();
   };
 
   function checkNodes(){
        var  input = document.getElementsByTagName("input");
         var   value='';
        for (var i = 0; i < input.length; i++) {
            if (input[i].type == "checkbox") {
                if (input[i].checked) {
                    value += input[i].id+",";
                }
            }
        }
    
      document.all.<%= HiddenField.ClientID %>.value=value;
    }
                        </script>
                        <tr>
                            <td style="border-bottom: none;">
                                同步的时间
                            </td>
                            <td style="border-bottom: none;">
                                <asp:RadioButton ID="RB_DTSTime0" Text="所有的节点发送后" GroupName="xx" runat="server" />
                                <br />
                                <asp:RadioButton ID="RB_DTSTime2" Text="流程结束时" GroupName="xx" runat="server" />
                                <br />
                                <asp:RadioButton ID="RB_DTSTime1" Text="指定的节点发送后" GroupName="xx" runat="server" />
                                [<a href="javascript:OpenDTSNodes()">设置同步的节点</a>]
                                <div id="dis" style="display: none;">
                                    <fieldset>
                                        <legend>选择节点</legend>
                                        <%
                                            BP.WF.Nodes nds = fl.HisNodes;
                                            string html = "<table><tr>";
                                            int count = 1;

                                            string[] checkNodes = fl.DTSSpecNodes.Split(',');
                                            foreach (BP.WF.Node nd in nds)
                                            {
                                                bool isChecked = false;
                                                foreach (string cn in checkNodes)
                                                {
                                                    if (cn == nd.NodeID.ToString())
                                                    {
                                                        isChecked = true;
                                                    }
                                                }
                                                if (count == 2)//一行两列
                                                {
                                                    if (isChecked)
                                                        html += "<td ><input onclick=\"checkNodes();\"  class=\"checkNode\"   checked=\"checked\" id=\"" + nd.NodeID + "\" type=\"checkbox\" />[" + nd.NodeID + "]" + nd.Name + "</td></tr>";
                                                    else
                                                        html += "<td><input onclick=\"checkNodes();\"  class=\"checkNode\" id=\"" + nd.NodeID + "\" type=\"checkbox\" />[" + nd.NodeID + "]" + nd.Name + "</td></tr>";

                                                    count = 0;
                                                }
                                                else
                                                {
                                                    if (isChecked)
                                                        html += "<td><input onclick=\"checkNodes();\"  class=\"checkNode\"  checked=\"checked\"  id=\"" + nd.NodeID + "\" type=\"checkbox\" />[" + nd.NodeID + "]" + nd.Name + "</td>";
                                                    else
                                                        html += "<td><input onclick=\"checkNodes();\"  class=\"checkNode\"  id=\"" + nd.NodeID + "\" type=\"checkbox\" />[" + nd.NodeID + "]" + nd.Name + "</td>";
                                                }
                                                count += 1;
                                            }
                                            html += "</table>";
                                        %>
                                        <%=html %>
                                        <asp:HiddenField ID="HiddenField" runat="server" />
                                    </fieldset>
                                </div>
                                <br />
                            </td>
                            <td style="border-bottom: none;">
                            </td>
                        </tr>
                    </table>
                </fieldset>
                <asp:Button ID="BtnSave" runat="server" OnClick="BtnSave_Click" Text=" 保 存 " />
            </td>
        </tr>
    </table>
</asp:Content>
