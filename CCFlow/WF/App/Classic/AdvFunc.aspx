<%@ Page Language="C#" AutoEventWireup="true" Inherits="AppDemo_AdvFunc" Codebehind="AdvFunc.aspx.cs" %>
<%@ Register src="../../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>导航</title>
    <link href="../../../DataUser/Style/Table0.css" rel="stylesheet" 
        type="text/css" />

        <style  type="text/css" >
        
.CaptionMsg
{
    vertical-align: middle;
    text-align: left;
    height: 32px;
    background-position: left center;
    line-height: 34px;
    font-size: 12px;
    text-align: center;
    vertical-align: middle;
    height: 30px;
    font-family: 黑体;
    font-weight: bolder;
    color: #FFFFFF;
    background: url('/DataUser/Style/Img/TitleMsg.png');
    width: 138px;
    margin-left: -10px;
    padding-left: 0px;
    background-repeat: no-repeat;
    background-attachment: fixed;
}

        </style>
</head>
<body>
    <form id="form1" runat="server">
    <uc1:Pub ID="Pub1" runat="server" />
    </form>
</body>
</html>

