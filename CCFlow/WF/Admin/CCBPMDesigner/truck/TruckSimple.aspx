<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TruckSimple.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.truck.TruckSimple" %>
<%@ Import Namespace="BP.WF.Template" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流程轨迹</title>
</head>
<body>
    <form id="form1" runat="server">
    <style type="text/css">
        .flowstep
        {
            float: left;
            width: 980px;
            margin: 0 auto;
            padding: 0px;
        }
        .flowstep-1
        {
            margin: 0 auto;
            padding: 0px;
            width: 980px;
        }
        .flowstep-1 li
        {
            list-style: none;
            text-align: center;
            float: left;
            width: 100px;
        }
        .step-name
        {
            padding: 3px 0px;
            font-weight: bold;
            color: #888888;
            height: 20px;
        }
        .step-name1
        {
            padding: 3px 0px;
            font-weight: bold;
            color: #0375D4;
            height: 20px;
        }
        .step-first1
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("../Img/process.png") no-repeat scroll 50% -34px transparent;
        }
        .step-first2
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("../Img/process.png") no-repeat scroll 50% -204px transparent;
        }
        .step-first3
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("../Img/process.png") no-repeat scroll 50% -272px transparent;
        }
        .step-flow1
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("../Img/process.png") no-repeat scroll 50% 0px transparent;
        }
        .step-flow0
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("../Img/process.png") no-repeat scroll 50% -102px transparent;
        }
        .step-flow2
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("../Img/process.png") no-repeat scroll 50% -170px transparent;
        }
        .step-flow3
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("../Img/process.png") no-repeat scroll 50% -238px transparent;
        }
        .step-last0
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("../Img/process.png") no-repeat scroll 50% -136px transparent;
        }
        .step-last1
        {
            height: 34px;
            line-height: 34px;
            font-size: 18px;
            background: url("../Img/process.png") no-repeat scroll 50% -68px transparent;
        }
        .step-time
        {
            color: #999999;
            margin-top: 10px;
        }
    </style>
    <div class="flowstep">
        <ul class="flowstep-1">

        <%
            string flowNo = Request.QueryString["flowNo"];
            string workid = Request.QueryString["workId"];
            string sql = "";
            
            
             %>

        </ul>
    </div>
    </form>
</body>
</html>
