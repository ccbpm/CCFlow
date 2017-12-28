<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="S15901.aspx.cs" Inherits="CCFlow.SDKFlowDemo.SDK.SDKComponents.F159.S15901" %>
<%@ Register src="../../../../WF/SDKComponents/Toolbar.ascx" tagname="Toolbar" tagprefix="uc1" %>

<%@ Register src="../../../../WF/SDKComponents/FrmCheck.ascx" tagname="FrmCheck" tagprefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>工具栏组件:发起请假申请单</title>
    <script type="text/javascript" >

        // 校验数据,是否完整？
        function CheckData() {

            var msg = "";
            //执行数据输入安全性校验.
            if (document.getElementById("TB_DTFrom").value == "")
                msg += "\t\n@请输入请见时间从.";

            if (document.getElementById("TB_TianShu").value == "")
                msg += "\t\n@请输入请假天数.";

            if (msg == "") {
                return true;
            }

            //提示保存失败的信息。
            alert(msg);
            return false;
        }

        //重写执行保存的方法.
        function Save() {

            if (CheckData() == false)
                return;

            try {

                document.getElementById('Btn_SaveOK').click(); //调用btn_save事件.
                //document.getElementById('Btn_Save').click(); //调用btn_save事件.
                return true; //保存成功，用户可以发送.
            } catch (e) {
                alert(e.name + " :  " + e.message);
                return false; // 保存失败不能发送.
            }

//            // 调用隐藏的保存按钮.
//            var btn = document.getElementById('Btn_Save').click(); //执行服务器的保存.
//            alert(btn);
//            btn.click()
        }

        //重写执行发送的方法.
        function Send() {

            if (CheckData() == false) {
                return;
            }
            //执行发送,调用服务器的隐藏发送按钮.
            document.getElementById('Btn_SendOK').click(); //Btn_Send.
        }
    </script>
    <link href="../../../../DataUser/Style/Table0.css" rel="stylesheet" 
        type="text/css" />
</head>
<body>

    <form id="form1" runat="server">
    <%
        BP.Demo.SDK.QingJia qingjia = new BP.Demo.SDK.QingJia();
        qingjia.OID = (int)this.WorkID;
        qingjia.RetrieveFromDBSources();
        
      %>
    <table style=" text-align:center;width:80%; ">
    <caption>工具栏组件toolbar演示</caption>
    <tr>
    <td>
    <!-- 工具栏组件: 每个按钮是否显示，以及显示的内容在节点属性里控制。 -->
        <uc1:Toolbar ID="Toolbar1" runat="server" />
        </td>
        </tr>
         <tr>
    <td>

    <!-- 表单部分 strat  **************************  -->
     <table style=" width:100%;">
     <tr>
     <td>请假日期从： </td>
     <td><input  type="text" id="TB_DTFrom"   /> </td>
     </tr>
     <tr>
     <td>到： </td>
     <td><input  type="text" id="TB_DTTo"  /> </td>
     </tr>

      <tr>
     <td>请假天数： </td>
     <td><input  type="text" id="TB_TianShu"  /> </td>
     </tr>

     <tr>
      <td>请假人： </td>
      <td><%=BP.Web.WebUser.Name %> </td>
     </tr>



     <tr>
     <th colspan=2>请假原因</th>
     </tr>

     <tr>
     <td colspan=2> 
         <uc2:FrmCheck ID="FrmCheck1" runat="server" />
         </td>
     </tr>



     </table>

    <!-- 表单部分 end  **************************  -->
      </td>
        </tr>
        </table>


    <div  style=" display:none">
    <!-- 我们创建两个服务器按钮让其保存与发送.-->
    <asp:Button ID="Btn_SendOK" runat="server" Text="发送" onclick="Btn_Send_Click"  />
    <asp:Button ID="Btn_SaveOK" runat="server" Text="保存" onclick="Btn_Save_Click" />
    </div>


    </form>
</body>
</html>
