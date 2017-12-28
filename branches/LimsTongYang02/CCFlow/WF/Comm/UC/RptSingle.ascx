<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RptSingle.ascx.cs"
 Inherits="CCFlow.WF.Comm.UC.RptSingle" %>
<%
    BP.En.Rpt2Base rpt = BP.En.ClassFactory.GetRpt2Base(this.Rpt2Name);
        string rptName = this.Rpt2Name;
        //求出来要显示的报表.
        BP.En.Rpt2Attr myattr = rpt.AttrsOfGroup.GetD2(int.Parse(this.Idx));
    %>

  <% =this.Rpt2Name %>   <%=this.ChartType %>