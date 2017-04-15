<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewFrmGuide.aspx.cs" Inherits="CCFlow.WF.Admin.CCFormDesigner.NewFrmGuide" %>

<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ccbpm表单创建向导</title>
    <link href="../../../DataUser/Style/Table0.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function Back() {
            window.location.href = 'NewFrmGuide.aspx';
        }
    </script>
    <style type="text/css">
    .title{
	text-decoration: none;
  font-size: 16px;
  color: #ffffff;
  background: cornflowerblue;
  padding: 5px 10px;
  margin: 0 5px;
  border-radius: 3px;
  box-shadow: 0px 1px 2px;
}
.con-list{
  line-height: 30px;
  font-size: 13px;
}
fieldset{
	  border: 1px solid #c7ced3;
	    margin-bottom: 20px;
}
.link-img{
	  float: right;
  padding-bottom: 10px;
  margin-right: 10px;
}

table caption{
	  border: 1px solid #C2D5E3;
  border-bottom: none;
line-height: 30px !important;
}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <uc1:Pub ID="Pub1" runat="server" />
    </form>
    <script type="text/javascript">
        window.onload = function () {
            var tableWidth = document.getElementsByTagName("table")[0].offsetWidth;
            document.getElementsByTagName("table")[0].Width = tableWidth;
        }
    </script>
</body>
</html>
