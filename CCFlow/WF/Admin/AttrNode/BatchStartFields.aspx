<%@ Page Title="" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true"
    CodeBehind="BatchStartFields.aspx.cs" Inherits="CCFlow.WF.Admin.BatchStartFields" %>

<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="txtRole" runat="server" />
    <table style="width: 70%;">
        <caption>批量发起规则设置</caption>
        <tr>
            <td colspan="1">规则设置：</td>
            <td colspan="2"> 
                <asp:DropDownList ID="DDL_BRole" runat="server">              
                </asp:DropDownList>
            </td>
            <td><a href="http://ccbpm.mydoc.io/?v=5404&t=17920" target="_blank">帮助</a> </td>
        </tr>
         <tr>
            <td colspan="1">显示的行数：</td>

            <td colspan="2">
                <asp:TextBox ID="TB_Num" runat="server" Width="56px" ></asp:TextBox>
            </td>

            <td> 0表示显示所有</td>
            </tr>
        <tr>
            <td class="Sum" colspan="4">
                批量发起字段(需要填写的字段.)
            </td>
        </tr>


        <tr> 
        <td colspan="4"> 

        <table style=" width:100%;">
         <tr>
            <th>序</th>
            <th>字段</th>
            <th>名称 </th>
            <th>类型</th>
        </tr>
        <%
            int nodeID = int.Parse(this.Request.QueryString["FK_Node"]);
            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs("ND" + nodeID);
            BP.WF.Node nd = new BP.WF.Node(nodeID);

            //显示字段..
            int idx = 0;
            foreach (BP.Sys.MapAttr item in attrs)
            {
        %>
        <tr>
            <td class="Idx">
                <%=idx++ %>
            </td>
            <td>
                <%
//查看是否选中
if (nd.BatchParas.Contains(item.KeyOfEn))
{ 
                %>
                <input name="CB_Node" type="checkbox" value="<%=item.KeyOfEn %>" checked="checked" /><%=item.KeyOfEn %>
                <% }
else
{
                %>
                <input name="CB_Node" type="checkbox" value="<%=item.KeyOfEn %>" /><%=item.KeyOfEn %>
                <%} %>
            </td>
            <td>
                <%=item.Name %>
            </td>
            <td>
                <%=item.LGTypeT %>
            </td>
        </tr>
        <%}  %>

        </table>

        </td>
        </tr>
       
        <tr>
            <td colspan="4">
                <asp:Button ID="Btn_Save" runat="server" Text="保存" OnClick="Btn_Save_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
