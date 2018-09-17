using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;

namespace BP.UnitTesting
{
    /// <summary>
    /// 流程基础功能
    /// </summary>
    public  class FlowBaseFunc : TestBase
    {
        #region 变量
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
        /// 流程基础功能
        /// </summary>
        public FlowBaseFunc()
        {
            this.Title = "流程基础 API 功能测试";
            this.DescIt = "创建、删除、移交、转发、抄送。";
            this.EditState = EditState.Passed; 
        }
        /// <summary>
        /// 执行
        /// </summary>
        public override void Do()
        {
            #region 定义全局变量.
            fk_flow = "023";
            userNo = "zhanghaicheng";
            fl = new Flow(fk_flow);
            #endregion 定义全局变量.

            // 测试删除.
            this.TestDelete();

            // 测试移交.
            this.TestShift();

            // 测试抄送.
            this.TestCC();
        }
       
        /// <summary>
        /// 测试抄送
        /// </summary>
        public void TestCC()
        {
            string sUser = "zhoupeng";
            BP.WF.Dev2Interface.Port_Login(sUser);

            //创建.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //执行发送.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID);

            //执行抄送.
            BP.WF.Dev2Interface.Node_CC(fl.No, objs.VarToNodeID, workID, "zhoushengyu", "zhoupeng", "移交测试", "", null, 0);

            //让 zhoushengyu 登陆.
            BP.WF.Dev2Interface.Port_Login("zhoushengyu");

            #region 检查预期结果.
            sql = "SELECT FK_Emp FROM WF_EmpWorks WHERE WorkID=" + workID;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("@移交后待办丢失。");

            if (dt.Rows.Count != 1)
                throw new Exception("@应该只有一个人处于待办状态。");

            if (dt.Rows[0][0].ToString() != "zhoupeng")
                throw new Exception("@应该是:zhoupeng 现在是:" + dt.Rows[0][0].ToString());

            CCList list = new CCList();
            int num = list.Retrieve(CCListAttr.Rec, sUser, CCListAttr.WorkID, workID);
            if (num <= 0)
                throw new Exception("@没有写入抄送数据在 WF_CCList 表中,查询的数量是:" + num);
            #endregion 检查预期结果
        }
        /// <summary>
        /// 测试移交
        /// </summary>
        public void TestShift()
        {
            //创建.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //执行发送.
            BP.WF.Dev2Interface.Node_SendWork(fl.No, workID);

            //执行移交.
            //BP.WF.Dev2Interface.Node_Shift(fl.No, 0, workID, "zhoushengyu", "移交测试");

            #region 检查预期结果
            sql = "SELECT * FROM WF_EmpWorks WHERE WorkID="+workID;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("@移交后待办丢失。");

            if (dt.Rows.Count!=1)
                throw new Exception("@应该只有一个人处于待办状态。");

            if (dt.Rows[0]["FK_Emp"].ToString() !="zhoushengyu" )
                throw new Exception("@没有移交给 zhoushengyu");

            ShiftWork sw = new ShiftWork();
            int i = sw.Retrieve(ShiftWorkAttr.WorkID,workID, ShiftWorkAttr.FK_Node, 2302, ShiftWorkAttr.ToEmp, "zhoushengyu");
            if (i==0)
                throw new Exception("@没有写入移交数据在WF_ShiftWork表中 zhoushengyu");
            if (i != 1)
                throw new Exception("@移交数据写入了多次，在WF_ShiftWork表中 zhoushengyu");
            #endregion 检查预期结果
        }
        /// <summary>
        /// 测试删除
        /// </summary>
        public void TestDelete()
        {
            //创建.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //执行发送.
            BP.WF.Dev2Interface.Node_SendWork(fl.No, workID);

            //执行删除.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fl.No, workID,false);

            #region 检查删除功能是否符合预期.
            gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            if (gwf.RetrieveFromDBSources() != 0)
                throw new Exception("@GenerWorkFlow未删除的数据.");

            gwl = new GenerWorkerList();
            gwl.WorkID = workID;
            if (gwl.RetrieveFromDBSources() != 0)
                throw new Exception("@GenerWorkerList未删除的数据.");

            sql = "SELECT * FROM ND2301 WHERE OID="+workID;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
                throw new Exception("@ ND2301 节点数据未删除. ");

            sql = "SELECT * FROM ND2302 WHERE OID=" + workID;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
                throw new Exception("@ ND2302 节点数据未删除. ");

            sql = "SELECT * FROM ND23Rpt WHERE OID=" + workID;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
                throw new Exception("@ ND23Rpt 数据未删除. ");
            #endregion 检查删除功能是否符合预期.
        }
    }
}
