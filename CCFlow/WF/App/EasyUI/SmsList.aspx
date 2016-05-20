<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SmsList.aspx.cs" Inherits="CCFlow.AppDemoLigerUI.SmsList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="jquery/lib/ligerUI/skins/Aqua/css/ligerui-all.css" rel="stylesheet"  type="text/css" />
    <link href="jquery/lib/ligerUI/skins/Gray/css/all.css" rel="stylesheet" type="text/css" />
    <link href="jquery/tablestyle.css" rel="stylesheet" type="text/css" />
    <link href="jquery/lib/ligerUI/skins/ligerui-icons.css" rel="stylesheet" type="text/css" />
  <%--  <script src="jquery/lib/jquery/jquery-1.5.2.min.js" type="text/javascript"></script>--%>
    <script src="jquery/lib/jquery/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="jquery/lib/ligerUI/js/core/base.js" type="text/javascript"></script>
    <script src="jquery/lib/ligerUI/js/ligerui.all.js" type="text/javascript"></script>
    <script src="jquery/lib/ligerUI/js/plugins/ligerGrid.js" type="text/javascript"></script>
    <script src="jquery/lib/ligerUI/js/plugins/ligerDialog.js" type="text/javascript"></script>
    <script src="jquery/lib/ligerUI/js/plugins/ligerDrag.js" type="text/javascript"></script>
    <script src="jquery/lib/ligerUI/js/plugins/ligerResizable.js" type="text/javascript"></script>
    <script src="js/AppData.js" type="text/javascript"></script>
    <script src="js/Smslist.js" type="text/javascript" ></script>
   <style type="text/css">
      .divcls
      {
          margin-left:5px;
          }
   </style>
 
</head>
<body >
    <form id="form1" runat="server">
      <div id="pageloading">
    </div>
    <div id="maingrid" style="margin: 0; padding: 0;">
    </div>
      <div id="toptoolbar">
    </div>
    <div  id="divDoc"  style="position:absolute !important;left:25%;z-index:3;border:1px solid #A3C0E8; background-color:#fff; margin :0px auto;
        padding:0px ; display:none;width:400px;height:200px;text-align:left">
    <div style=" height:20px;text-align:left; width:100%; background-color:#A3C0E8;">
     <font style="font:15px; font-weight:bold; text-align:left;">详细信息:</font> 
    </div>
    <div id="divDocContent" style="width:100%; text-align:left; margin:2px auto">
    
    </div>
    </div</form>
</body>
</html>
