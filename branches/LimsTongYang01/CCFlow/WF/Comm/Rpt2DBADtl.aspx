<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Comm/MasterPage.master" AutoEventWireup="true" CodeBehind="Rpt2DBADtl.aspx.cs" Inherits="CCFlow.WF.Comm.Rpt2DBADtl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<%
    string rptName = this.Request.QueryString["Rpt2Name"];
    BP.Rpt.Rpt2Base rpt = BP.En.ClassFactory.GetRpt2Base(rptName);
    string idx = this.Request.QueryString["Idx"];
    BP.Rpt.Rpt2Attr rptAttr = rpt.AttrsOfGroup.GetD2(int.Parse(idx));

    // 执行SQL.
    string sql = rptAttr.DBSrcOfDtl;
    foreach (string k in this.Request.QueryString.Keys)
        sql = sql.Replace("@"+k, this.Request[k]);
    sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
    sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
    sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);

    foreach (string key in this.Request.QueryString.Keys)
        sql = sql.Replace("@" + key, this.Request.QueryString[key]);

    if (sql.Contains("@") == true)
    {
        BP.DA.AtPara ap = new BP.DA.AtPara(rptAttr.DefaultParas);
        foreach (string k in ap.HisHT.Keys)
        {
            sql = sql.Replace("@" + k, ap.HisHT[k].ToString());
        }
    }
    
    System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
 %>

 <table class="Table" cellpadding="0" cellspacing="0" border="0" style="width:100%;">
    <tr>
    <td class="GroupTitle" style="text-align:center">序</td>
    <%
  // 标题.
  foreach (System.Data.DataColumn dc in dt.Columns)
  {
      %> <td class="GroupTitle" style="text-align:center"> <%=dc.ColumnName %> </td> <%
  }
%>
</tr>
   <%   //数据表.
       int i = 0;
  foreach (System.Data.DataRow dr  in dt.Rows)
  {
    %>
 <tr >
     <%
         i++;
      %>
       <td class='Idx'><%=i %></td>
      <%
      foreach (System.Data.DataColumn dc in dt.Columns)
        {
           %>
           <td> <%=dr[dc.ColumnName] %> </td>
       <%  }  %>
</tr>
 <%}%>
 </table>

</asp:Content>
