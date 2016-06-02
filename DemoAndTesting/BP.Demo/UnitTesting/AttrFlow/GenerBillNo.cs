using System;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;
using BP.UnitTesting;

namespace BP.UnitTesting.AttrFlow
{
    /// <summary>
    /// 产生单据编号
    /// </summary>
    public class GenerBillNo : TestBase
    {
        /// <summary>
        /// 产生单据编号
        /// </summary>
        public GenerBillNo()
        {
            this.Title = "产生单据编号";
            this.DescIt = "流程: 以demo 流程023 为例测试产生单据编号。";
            this.EditState = EditState.Passed;
        }

        #region 全局变量
        /// <summary>
        /// 流程编号
        /// </summary>
        public string fk_flow = "";
        /// <summary>
        /// 用户编号
        /// </summary>
        public string userNo = "";
        /// <summary>
        /// 所有的流程
        /// </summary>
        public Flow fl = null;
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 workID = 0;
        /// <summary>
        /// 发送后返回对象
        /// </summary>
        public SendReturnObjs objs = null;
        /// <summary>
        /// 工作人员列表
        /// </summary>
        public GenerWorkerList gwl = null;
        /// <summary>
        /// 流程注册表
        /// </summary>
        public GenerWorkFlow gwf = null;
        #endregion 变量

        /// <summary>
        /// 测试案例说明:
        /// 1， 不同的格式，生成不同的编号。
        /// </summary>
        public override void Do()
        {
            this.fk_flow = "023";
            fl = new Flow("023");
            string sUser = "zhoupeng";
            BP.WF.Dev2Interface.Port_Login(sUser);

            //删除数据。
            fl.DoDelData();

            //创建.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //定义单据格式.
            string billFormat = "CN{yyyy}-{MM}-{dd}IDX-{LSH4}";
            fl.BillNoFormat = billFormat;
            fl.Update();

            //执行发送.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID);

            // 应该是。
            billFormat = billFormat.Replace("{yyyy}", DateTime.Now.ToString("yyyy"));
            billFormat = billFormat.Replace("{MM}", DateTime.Now.ToString("MM"));
            billFormat = billFormat.Replace("{dd}", DateTime.Now.ToString("dd"));
            billFormat = billFormat.Replace("{LSH4}", "0001");

            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            if (gwf.BillNo != billFormat)
                throw new Exception("@应当是:" + billFormat + "现在是:" + gwf.BillNo);

            // 生成子流程。
            Flow flSub = new Flow("024");
            flSub.DoCheck();
            flSub.CheckRpt();

            // 产生子流程编号。
            for (int i = 1; i < 5; i++)
            {
                flSub.BillNoFormat = "{ParentBillNo}-{LSH4}";
                Int64 subWorkID = BP.WF.Dev2Interface.Node_CreateBlankWork(flSub.No);
                objs = BP.WF.Dev2Interface.Node_SendWork(flSub.No, subWorkID);

                //设置流程信息。
                BP.WF.Dev2Interface.SetParentInfo(flSub.No, subWorkID, "023", workID,0,null);
                if (i == 2)
                    continue;

                string subFlowBillNo = DBAccess.RunSQLReturnStringIsNull("SELECT BillNo FROM " + flSub.PTable + " WHERE OID=" + subWorkID, "");
                if (subFlowBillNo != billFormat + "-000" + i)
                    throw new Exception("@应当是:" + billFormat + "-000" + i + " , 现在是:" + subFlowBillNo);
            }

        }
    }
}
