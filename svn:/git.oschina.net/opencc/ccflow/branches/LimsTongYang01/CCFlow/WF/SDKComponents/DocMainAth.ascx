<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocMainAth.ascx.cs" Inherits="CCFlow.WF.App.Comm.DocMainAth" %>
<%
    string enName = "ND" + this.Request.QueryString["FK_Node"];
    string src = "/WF/CCForm/DocMainAth.aspx?FID=" + this.Request.QueryString["FID"];
    src += "&WorkID=" + this.Request.QueryString["WorkID"];
    src += "&FK_Node=" + this.Request.QueryString["FK_Node"];
    src += "&FK_Flow=" + this.Request.QueryString["FK_Flow"];
    src += "&FK_FrmAttachment=ND" + this.Request.QueryString["FK_Node"] + "_DocMultiAth";
    src += "&RefPKVal=" + this.Request.QueryString["WorkID"];
    src += "&PKVal=" + this.Request.QueryString["WorkID"];
    src += "&Ath=DocMainAth" ;
    src += "&FK_MapData=" + enName;
    src += "&Paras=" + this.Request.QueryString["Paras"];
%>
  <iframe id='Fa' src='<%=src%>' frameborder="0" leftmargin='0' topmargin='0' height="50" scrolling="no"
   style="align:left;width:100%; margin-left:1px;" scrolling="auto"></iframe>



