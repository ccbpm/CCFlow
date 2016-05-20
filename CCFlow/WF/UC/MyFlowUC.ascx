<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.UC.MyFlowUC" Codebehind="MyFlowUC.ascx.cs" %>
<%@ Register src="../Comm/UC/UCSys.ascx" tagname="UCSys" tagprefix="uc3" %>
<%@ Register src="../Comm/UC/ToolBar.ascx" tagname="ToolBar" tagprefix="uc4" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc5" %>
<%@ Register src="UCEn.ascx" tagname="UCEn" tagprefix="uc6" %>
    <%@ Register src="MyFlow.ascx" tagname="MyFlow" tagprefix="uc2" %>
    <style type="text/css">
        .style1
        {
            width: 225px;
        }
        .left
        {
            float:left;
        }
        .right
        {
            float:right;
        }
        .main
        {
            text-align:left;
        }
    </style>
	<script language="JavaScript" src="/WF/Comm/JScript.js"></script>
    <div align="center">
 <%--   <span >
    <uc1:FlowInfoSimple ID="FlowInfoSimple1" runat="server" />
    </span>--%>

    <span align="center" >

    <%--  <table style=" text-align:center"  >
      <%
          BP.WF.Flow fl = new BP.WF.Flow(this.Request.QueryString["FK_Flow"]);
          
           %>
    <caption><%=fl.Name %></caption>
    <tr>
    <td>--%>

    <uc2:MyFlow ID="MyFlow1" runat="server" />
<%--
     </td>
    </tr>
    </table>--%>

    </span>
    </div>