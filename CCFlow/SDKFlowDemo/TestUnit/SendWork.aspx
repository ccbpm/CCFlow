<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendWork.aspx.cs" Inherits="CCFlow.SDKFlowDemo.TestUnit.Test111" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

    <div>
    <%
        String userNo = "admin";
        BP.Port.Emp emp = new BP.Port.Emp(userNo);
        BP.Web.WebUser.SignInOfGener(emp);

        BP.WF.UnitTesting.TestAPI api = new BP.WF.UnitTesting.TestAPI();
        api.No = "SendWork";
        api.Name = "标准的工作发送";
        api.Save();

        BP.WF.UnitTesting.TestVer apiVer = new BP.WF.UnitTesting.TestVer();
        apiVer.No = "SendWork002";
        apiVer.Name = "版本" + apiVer.No;

        try
        {
            //定义了5个样本. 对该过程执行5次。
            for (int idx = 0; idx < 5; idx++)
            {

                DateTime startTime = System.DateTime.Now;
                for (int i = 0; i <= 1000; i++)
                {
                    long workid = BP.WF.Dev2Interface.Node_CreateBlankWork("065");
                    BP.WF.SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork("065", workid, 0, "admin");
                }
                //doSomeThing();   //要运行的java程序
                DateTime endTime = System.DateTime.Now;
                TimeSpan ts = endTime - startTime;

                BP.WF.UnitTesting.TestSample dtl = new BP.WF.UnitTesting.TestSample();
                dtl.MyPK = BP.DA.DBAccess.GenerGUID();
                dtl.FK_API = api.No;
                dtl.FK_Ver = apiVer.No;
                dtl.DTFrom = startTime.ToShortTimeString();
                dtl.DTTo = endTime.ToShortTimeString();
                dtl.Name = api.Name + "-" + apiVer.Name;

                dtl.TimeUse = ts.TotalMilliseconds;
                //运行时间间隔长度（毫秒）/1000 (换算成秒)/1000(每次需要多少秒) 
                //
                dtl.TimesPerSecond = 1/(ts.TotalMilliseconds / 1000/1000);
                dtl.Insert();
            }

            apiVer.Insert(); //执行成功后，版本号在插入里面.
        }
        catch (Exception ex)
        {
            BP.WF.UnitTesting.TestSample dtl = new BP.WF.UnitTesting.TestSample();
            dtl.Delete(BP.WF.UnitTesting.TestSampleAttr.FK_Ver, apiVer.No);
            throw ex;
        }

        this.Response.Redirect("/WF/Comm/Group.htm?EnsName=BP.WF.UnitTesting.TestSamples", true);

     %>
    </div>

    
    <fieldset>
    <legend>关于发送的标准测试</legend>
    <ul>
    <li> 该api执行了两个方法。</li>
    <li> 创建workid,然后执行的发送.</li>
    <li> 测试的流程无表单字段.</li>
    </ul>
    </fieldset>

    <fieldset>
    <legend>历史测试报告-徐淑浩-sqlserver  2018-9-16</legend>
    <ul>
    <li>执行了5个批次：每批次执行1000次发起. 每秒跑出了19次发起.</li>
    </ul>
    </fieldset>
    <fieldset>
    <legend>历史测试报告-徐淑浩-oracle  2018-9-16</legend>
    <ul>
    <li>执行了5个批次：每批次执行1000次发起. 每秒跑出了10次发起.</li>
    </ul>
    </fieldset>

    <fieldset>
    <legend>历史测试报告-徐淑浩-oracle  2018-9-16</legend>
    <ul>
    <li>执行了5个批次：每批次执行1000次发起. 每秒跑出了10次发起.</li>
    </ul>
    </fieldset>

    </form>
</body>
</html>
