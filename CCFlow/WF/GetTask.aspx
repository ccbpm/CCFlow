<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_JumpCheck" Codebehind="GetTask.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<script language="JavaScript" src="./Comm/JScript.js"></script>
   <script   type="text/javascript">
       function Tackback(fk_flow, fk_node, toNode, workid) {
           if (window.confirm('您确定要执行取回操作吗？') == false)
               return;
           var url = 'GetTask.aspx?DoType=Tackback&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node + '&ToNode=' + toNode + '&WorkID=' + workid;
           window.location.href = url;
       }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<div style="float:none;text-align:center">
    
    <uc1:Pub ID="Pub1" runat="server" />
    
    </div>
</asp:Content>



