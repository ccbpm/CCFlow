<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="SearchFlow.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.SearchFlow" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<%--标签页显示页面--%>
    <script type="text/javascript">

        function OpenTab(id, name, url, icon) {
            if (window.parent != null && window.parent.closeTab != null) {
                window.parent.closeTab(name);
                window.parent.addTab(id, name, url, icon);
            }
        }


    </script>
<center>
<div style="text-align;center">
<br />
<br />
<br />
<br />
<h2>请输入流程名称、节点名称关键字点查找按钮，直达属性设置。</h2>
    <asp:Label ID="Label1" runat="server" Text="查询类型:" Font-Size="15px" ></asp:Label>
   <asp:RadioButton ID="RBT_BD" runat="server" GroupName="xzgz"  Text=" 本地" Checked="true" />
    <asp:RadioButton ID="RBT_Y" runat="server" GroupName="xzgz"  Text=" 云" />



    <asp:TextBox ID="key" runat="server" ></asp:TextBox>
    <asp:Button ID="BtnSava" runat="server" Text="关键字查询" OnClick="Button1_Click" />
    <p></p>
    <asp:Repeater ID="RepList" runat="server">
        <HeaderTemplate><table>
            <tr>
                <th style=" width:50px; color:gray; background-color:#F0FFFF">序</th>
                <th style=" width:200px;color:gray;background-color:#F0FFFF"">流程名称</th>
                 <th style=" width:200px;color:gray;background-color:#F0FFFF"">流程类别</th>

            </tr>
            </HeaderTemplate>
        <ItemTemplate>
                <tr>
                    <td style="width:50px; text-align:center"><%#Container.ItemIndex+1%></td>

                    <% if (this.RBT_BD.Checked==true)
                       {%>

                     <td  style="width:200px;"><a style="color:gray" href="javascript:OpenTab('','流程查询','../CCBPMDesigner/Designer.aspx?FK_Flow=<%#Eval("No")%>&UserNo=<%=BP.Web.WebUser.No %>&SID=<%=BP.Web.WebUser.SID %>&Flow_V=2','icon-search');">  <image src="../CCBPMDesigner/Img/Flow.png"></image>&nbsp&nbsp<%#Eval("flowname") %></a> </td>        
                    <td style="width:200px;"><%#Eval("Name") %></td>      
                  
                       <%}
                         
                       else
                       
                       {%>
                            <td  style="width:200px;"><a style="color:gray">  <image src="../Clound/imgs/Flow.png"></image>&nbsp&nbsp<%#Eval("Name") %></a> </td>
                       <%}
                       
                        %>
                </tr>
        </ItemTemplate>  
        <FooterTemplate> </table></FooterTemplate>
    </asp:Repeater>
</div>
</center>
</asp:Content>
