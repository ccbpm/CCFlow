<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SysManager.aspx.cs" Inherits="CCFlow.App.AppDemo_WelcomeApp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    <fieldset>
    <legend> 组织结构管理</legend>
    <ul>
   <li> <a href="/WF/Comm/Search.aspx?EnsName=BP.Port.Emps"> 人员</a> </li>
  <li>  <a href="/WF/Comm/Search.aspx?EnsName=BP.Port.Depts"> 部门</a></li>
  <li>  <a href="/WF/Comm/Search.aspx?EnsName=BP.Port.Stations"> 岗位</a></li>
    </ul>
    </fieldset>

     <fieldset>
    <legend> 故障编码配置管理</legend>
    <ul>
   <li> <a href="/WF/Comm/Search.aspx?EnsName=BP.Port.Emps"> 人员</a> </li>
  <li>  <a href="/WF/Comm/Search.aspx?EnsName=BP.Port.Depts"> 部门</a></li>
  <li>  <a href="/WF/Comm/Search.aspx?EnsName=BP.Port.Stations"> 岗位</a> </li>
    </ul>
    </fieldset>

    
     <fieldset>
    <legend> 基础数据配置管理</legend>
    <ul>
  <li>  <a href="/WF/Comm/Search.aspx?EnsName=BP.Port.Emps"> 人员</a> </li>
  <li>  <a href="/WF/Comm/Search.aspx?EnsName=BP.Port.Depts"> 部门</a> </li>
  <li>  <a href="/WF/Comm/Search.aspx?EnsName=BP.Port.Stations"> 岗位</a> </li>
    </ul>
    </fieldset>


    </div>
    </form>
</body>
</html>
