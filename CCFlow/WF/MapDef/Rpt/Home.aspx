<%@ Page Title="报表设计" Language="C#" MasterPageFile="RptGuide.master" AutoEventWireup="true"
    Inherits="WF_MapDef_Rpt_Home" CodeBehind="Home.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <base target="_self" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%
        string rptNo = "ND" + int.Parse(this.FK_Flow) + "MyRpt"; // this.Request.QueryString["RptNo"];

        var rpt = new BP.Sys.MapData();
        rpt.No = rptNo;
        rpt.RetrieveFromDBSources();

        if (string.IsNullOrWhiteSpace(rpt.Name))
        {
    %>
    <h4>
        报表定义失效，请首先运行<a href="../../Admin/DoType.aspx?RefNo=<%=this.FK_Flow%>&DoType=FlowCheck&Lang=CH">流程检查</a>，然后再打开报表设计向导。</h4>
    <%
}
           else
           {%>
    <h4>
        报表设计向导，帮助您完成报表个性化定义：</h4>
    <ul class="navlist">
        <li>
            <div>
                <a href="S1_Edit.aspx?FK_MapData=<%=this.FK_MapData%>&FK_Flow=<%=this.FK_Flow%>&RptNo=<%=rptNo%>">
                    <span class="nav">1. 基本信息</span></a></div>
        </li>
        <li>
            <div>
                <a href="S2_ColsChose.aspx?FK_MapData=<%=this.FK_MapData%>&FK_Flow=<%=this.FK_Flow%>&RptNo=<%=rptNo%>">
                    <span class="nav">2. 设置报表显示列</span></a></div>
        </li>
        <li>
            <div>
                <a href="S3_ColsLabel.aspx?FK_MapData=<%=this.FK_MapData%>&FK_Flow=<%=this.FK_Flow%>&RptNo=<%=rptNo%>">
                    <span class="nav">3. 设置报表显示列次序</span></a></div>
        </li>
        <li>
            <div>
                <a href="S5_SearchCond.aspx?FK_MapData=<%=this.FK_MapData%>&FK_Flow=<%=this.FK_Flow%>&RptNo=<%=rptNo%>">
                    <span class="nav">4. 设置报表查询条件</span></a></div>
        </li>
        <li>
            <div>
                <a href="S6_Power.aspx?FK_MapData=<%=this.FK_MapData%>&FK_Flow=<%=this.FK_Flow%>&RptNo=<%=rptNo%>">
                    <span class="nav">5. 设置报表权限</span></a></div>
        </li>
    </ul>    
    <%
}%>
</asp:Content>
