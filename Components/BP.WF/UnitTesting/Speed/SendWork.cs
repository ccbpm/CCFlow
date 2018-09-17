using System;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;
 

namespace BP.UnitTesting
{
    /// <summary>
    /// 测试名称
    /// </summary>
    public class SendWork : TestBase
    {
        /// <summary>
        /// 测试名称
        /// </summary>
        public SendWork()
        {
            this.Title = "效率测试：SendWork";
            this.DescIt = "流程: 005月销售总结(同表单分合流),执行发送后的数据是否符合预期要求.";
            this.EditState = EditState.UnOK;
        }
        /// <summary>
        /// 测试案例说明:
        /// 1, . 这是一个标准的发送效率测试.
        /// 2, . 执行了createworkid 之后进行发送.
        /// </summary>
        public override void Do()
        {
            this.AddNote("开始执行发送.....");

            String userNo = "admin";
            BP.Port.Emp emp = new BP.Port.Emp(userNo);
            BP.Web.WebUser.SignInOfGener(emp);

            BP.WF.UnitTesting.TestAPI api = new BP.WF.UnitTesting.TestAPI();
            api.No = "SendWork";
            api.Name = "标准的工作发送";
            api.Save();

            BP.WF.UnitTesting.TestVer apiVer = new BP.WF.UnitTesting.TestVer();
            apiVer.No = "SendWork"+BP.DA.DataType.CurrentDataTime;
            apiVer.Name = "版本" + apiVer.No;             

            try
            {
                //定义了10个样本. 对该过程执行10次。
                for (int idx = 0; idx < 5; idx++)
                {
                    DateTime startTime = System.DateTime.Now;
                    for (int i = 0; i <= 1000; i++)
                    {
                        long workid = BP.WF.Dev2Interface.Node_CreateBlankWork("230");
                        BP.WF.SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork("230", workid, 0, "admin");
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
                    dtl.TimesPerSecond = ts.TotalMilliseconds / 1000;
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

            this.AddNote("查看数据： <a href=''></a>");
        }
    }
}
