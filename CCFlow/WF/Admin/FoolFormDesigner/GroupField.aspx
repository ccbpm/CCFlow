<%@ Page Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.WF_MapDef_GroupField" Title="未命名頁面" Codebehind="GroupField.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server"  >
<script language="javascript" type="text/javascript" >
   function Del(refNo, refOID )
	{
	 if ( window.confirm('您确定要删除吗？ ') == false ) 
	      return false;
	   window.location.href='GroupField.aspx?RefNo='+ refNo +'&DoType=DelGF&RefOID=' + refOID;
    }
</script>
<base target="_self" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<%
    int gfOID = 0;
    string str = this.Request.QueryString["GroupField"];
    if (str == "" || str == null)
        str = "0";
    gfOID= int.Parse(str);
    %>

   <% if (gfOID == 0)
      { %>
    <fieldset>
    <legend>新建空白分组</legend>

    <table>
    <tr> 
    <td>分组名称</td>
    <td> 
        <asp:TextBox ID="TB_Blank_Name" runat="server" Text="新建分组" Width="390px"></asp:TextBox>
        </td>
    </tr>

    <tr>
    <td colspan="2"> <font color="gray"> 空白字段分组，建立后可以把相关的字段放入此分组里。</font></td>
    </tr>

      <tr> 
    <td colspan="2" > 
        <asp:Button ID="Btn_SaveBlank" runat="server" Text="保存"   onclick="btn_SaveBlank_Click" />
          </td>
    </tr>
    </table>

    </fieldset>



    <fieldset>
    <legend>新建审核分组</legend>

    <table>
    <tr> 
    <td>分组名称</td>
    <td> 
        <asp:TextBox ID="TB_Check_Name" runat="server" Width="390px" 
            AutoPostBack="True" ontextchanged="TB_Check_Name_TextChanged"></asp:TextBox>
        </td>
    </tr>

     <tr> 
    <td>前缀</td>
    <td> 
        <asp:TextBox ID="TB_Check_Pix" runat="server" Width="390px"></asp:TextBox>
        </td>
    </tr>

    <tr> 
    <td colspan=2> 
        <asp:Button ID="Btn_SaveCheck" runat="server" Text="保存" 
            onclick="Btn_Save_Check_Click" />
        </td>
    </tr>
    </table>
    </fieldset>
    

    <fieldset>
    <legend> <a href="GroupField.aspx?DoType=NewEvalGroup&FK_MapData=<%=this.FK_MapData %>" > 创建工作质量考核字段分组</a></legend>
     
     <ul>
      <li> 创建质量考核: EvalEmpNo,EvalEmpName,EvalCent,EvalNote 4个必要的字段。</li>
     </ul>

    </fieldset>

    <%}else { %>

    <%
          
        BP.Sys.GroupField gf = new BP.Sys.GroupField();
        gf.OID = gfOID;
        gf.RetrieveFromDBSources();
          
        this.TB_Edit_Name.Text = gf.Lab;
        this.Btn_Edit_Del.Attributes["onclick"] = "return window.confirm('您确定要删除吗？');";
        this.Btn_Edit_DelAll.Attributes["onclick"] = "return window.confirm('您确定要删除吗？');";
     %>

    <fieldset>
    <legend>修改分组</legend>

    <table>
    <tr> 
    <td>分组名称</td>
    <td> 
        <asp:TextBox ID="TB_Edit_Name"   runat="server" Width="390px"></asp:TextBox>
        </td>
    </tr>

      <tr> 
    <td colspan="2" > 
        <asp:Button ID="Btn_Edit_Save" runat="server" Text="保存" 
            onclick="Btn_Edit_Save_Click"  />
        <asp:Button ID="Btn_Edit_SaveAndClose" runat="server" Text="保存并关闭" 
            onclick="Btn_Edit_SaveAndClose_Click"  />

        <asp:Button ID="Btn_Edit_Del" runat="server" Text="删除" 
            onclick="Btn_Edit_Del_Click"  />
        <asp:Button ID="Btn_Edit_DelAll" runat="server" Text="删除并删除该分组下的字段" 
            onclick="Btn_Edit_DelAll_Click"  />


          </td>
    </tr>
    </table>

    </fieldset>


    <% } %>


</asp:Content>

