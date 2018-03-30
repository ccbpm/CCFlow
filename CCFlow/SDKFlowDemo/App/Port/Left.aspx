<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Left.aspx.cs" Inherits="CCFlow.App.AppDemo_Left" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>导航</title>
    <link href="/DataUser/Style/Table0.css" rel="stylesheet" type="text/css" /><link href="/DataUser/Style/Table.css" rel="stylesheet" type="text/css" />

    <script language="JavaScript" type="text/javascript">
        function myrefresh() {
            window.location.reload();
        }
    </script>
</head>
<body >
    <form id="form1" runat="server">

    <!------------------------- BP组件  -------------------->
    <table>
    <tr>
    <th>BP组件</th>
    </tr>

    <tr>
    <td>
     <ul>
          <li> <a href="/WF/Comm/Ens.aspx?EnsName=BP.Demo.BPFramework.BanJis" target="main">班级维护</a></li>
          <li><a href="/WF/Comm/En.htm?EnName=BP.Demo.BPFramework.Student" target="main">新建学生</a></li>
          <li> <a href="/WF/Comm/Search.htm?EnName=BP.Demo.BPFramework.Student" target="main">学生查询</a></li>
          <li> <a href="/WF/Comm/Group.htm?EnName=BP.Demo.BPFramework.Student" target="main">学生统计</a></li>
       </ul>
     </td>
    </tr>


    <!------------------------- JQuery 与Entity的结合使用.  -------------------->
    <tr> <th>JQuery与Entity</th>  </tr>
    <tr>
    <td>
     <ul>
          <li><a href="/SDKFlowDemo/BPFramework/DataInputJQ/Student.htm" target="main">新建学生</a></li>
       </ul>
     </td>
    </tr>

     <!------------------------- 图标显示.  -------------------->
    <tr> <th>图标显示</th>  </tr>
    <tr>
    <td>
     <ul>
          <li><a href="/WF/Comm/Rpt/Pie.htm?RptName=BP.Demo.RptStudent" target="main">男女比例(饼图)</a></li>
       </ul>
     </td>
    </tr>




    <!------------------------- 数据采集  -------------------->
    <tr>
    <th>数据采集</th>
    </tr>

     <tr>
    <td>
     <ul>
          <li> <a href="/SDKFlowDemo/BPFramework/DataInput/DemoRegUserWithRequest.aspx" target="main">通用界面</a></li>
          <li><a href="/SDKFlowDemo/BPFramework/DataInput/DemoRegUserWithUserContral.aspx" target="main">用户控件</a></li>
          <li> <a href="/SDKFlowDemo/BPFramework/DataInput/Cell3DLeft.aspx" target="main">3维2维度在左边</a></li>
          <li><a href="/SDKFlowDemo/BPFramework/DataInput/Cell3DLeft.aspx" target="main">3维2维度在上边</a></li>
       </ul>
     </td>
    </tr>

    <!------------------------- 输出  -------------------->
    <tr>
    <th>输出</th>
    </tr>

    <tr>
    <td>
     <ul>
          <li> <a href="/SDKFlowDemo/BPFramework/Output/DemoJZOut.aspx" target="main">矩阵输出</a></li>
          <li> <a href="/SDKFlowDemo/BPFramework/Output/DemoTurnPage.aspx" target="main">翻页</a></li>
          <li> <a href="/SDKFlowDemo/BPFramework/Output/DemoUserList.aspx" target="main">不分页</a></li>
       </ul>
     </td>
    </tr>


    </table>
  
                    
    </form>
</body>
</html>
