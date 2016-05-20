<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FilesView.aspx.cs" Inherits="CCFlow.WF.CCForm.FilesView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <base target="_self"" />
</head>
<body>
    <form id="form1" runat="server">
    <div  style=" text-align:center">
     <uc1:Pub ID="Pub1" runat="server" />
     </div>
    </form>
</body>
</html>
