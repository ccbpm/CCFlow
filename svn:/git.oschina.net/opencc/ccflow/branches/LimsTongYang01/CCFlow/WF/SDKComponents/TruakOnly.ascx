<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TruakOnly.ascx.cs" Inherits="CCFlow.WF.App.Comm.TruakOnly" %>
<%
    string enName = "ND" + this.Request.QueryString["FK_Node"];
    string srcTrack = "/WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Track&FK_Flow=" + this.Request.QueryString["FK_Flow"];
    srcTrack += "&FK_Node=" + this.Request.QueryString["FK_Node"];
    srcTrack += "&WorkID=" + this.Request.QueryString["WorkID"];
 %>
<iframe id='F' src='<%=srcTrack%>'
 frameborder="0" style=' padding: 0px; border: 0px; margin:0px; height:700px;' width='100%'>
        </iframe>
