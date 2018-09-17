using System;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.En;
using BP.DA;
using BP.Web;
using BP.Port;
using System.Data;
using System.Collections;

namespace BP.UnitTesting
{
    /// <summary>
    /// 父子流程
    /// </summary>
    public  class CallSubFlow : TestBase
    {
        /// <summary>
        /// 父子流程
        /// </summary>
        public CallSubFlow()
        {
            this.Title = "父子流程";
            this.DescIt = "测试call 子流程,以023 与024流程为实例.";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 说明 ：父子流程
        /// 涉及到了:  等功能。
        /// </summary>
        public override void Do()
        {
            //测试子流程只有一个人.
            Test1();

            //子流程的开始节点有一组人.
            Test2();
        }
       
        /// <summary>
        /// 测试子流程只有一个人
        /// </summary>
        private void Test1()
        {
            string fk_flow = "023";
            string userNo = "zhanghaicheng";
            Flow fl = new Flow(fk_flow);

            // zhanghaicheng 登录.
            BP.WF.Dev2Interface.Port_Login(userNo);

            //创建空白工作, 发起开始节点.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);


            //创建第一个子流程，调用只有一个人的接口.
            Emp empSubFlowStarter = new Emp("zhoupeng");
            Int64 subFlowWorkID = BP.WF.Dev2Interface.Node_CreateStartNodeWork("024", null, null, empSubFlowStarter.No,
                "子流程发起测试", workid,"023",0);

            #region 检查发起的子流程 流程引擎表 是否完整？
            GenerWorkFlow gwf = new GenerWorkFlow(subFlowWorkID);
            if (gwf.PFlowNo != "023")
                throw new Exception("@父流程编号错误,应当是023现在是" + gwf.PFlowNo);

            if (gwf.PWorkID != workid)
                throw new Exception("@父流程WorkID错误,应当是" + workid + "现在是" + gwf.PWorkID);

            if (gwf.Starter != empSubFlowStarter.No)
                throw new Exception("@流程发起人编号错误,应当是" + empSubFlowStarter.No + "现在是" + gwf.Starter);

            if (gwf.StarterName != empSubFlowStarter.Name)
                throw new Exception("@流程发起人 Name 错误,应当是" + empSubFlowStarter.Name + "现在是" + gwf.StarterName);

            if (gwf.FK_Dept != empSubFlowStarter.FK_Dept)
                throw new Exception("@流程隶属部门错误,应当是" + empSubFlowStarter.FK_Dept + "现在是" + gwf.FK_Dept);

            if (gwf.Title != "子流程发起测试")
                throw new Exception("@流程标题 子流程发起测试 错误,应当是 子流程发起测试 现在是" + gwf.Title);

            if (gwf.WFState != WFState.Runing)
                throw new Exception("@流程 WFState 错误,应当是 Runing 现在是" + gwf.WFState);

            if (gwf.FID != 0)
                throw new Exception("@FID错误,应当是0现在是" + gwf.FID);

            if (gwf.FK_Flow != "024")
                throw new Exception("@FK_Flow错误,应当是024现在是" + gwf.FK_Flow);

            if (gwf.FK_Node != 2401)
                throw new Exception("@停留的当前节点错误,应当是2401现在是" + gwf.FK_Flow);

            GenerWorkerLists gwls = new GenerWorkerLists(subFlowWorkID, 2401);
            if (gwls.Count != 1)
                throw new Exception("@待办列表个数应当1,现在是" + gwls.Count);


            // 检查发起人列表是否完整？
            GenerWorkerList gwl = (GenerWorkerList)gwls[0];
            if (gwl.FK_Emp != empSubFlowStarter.No)
                throw new Exception("@处理人错误，现在是:" + empSubFlowStarter.No);

            if (gwl.IsPassInt != 0)
                throw new Exception("@通过状态应当是未通过，现在是:" + gwl.IsPassInt);

            if (gwl.FID != 0)
                throw new Exception("@流程ID 应当是0 ，现在是:" + gwl.FID);

            if (gwl.FK_EmpText != empSubFlowStarter.Name)
                throw new Exception("@FK_EmpText  错误, 现在是:" + gwl.FK_EmpText);

            #endregion 检查发起的子流程 流程引擎表 是否完整？

            #region 检查发起的子流程数据是否完整？
            //检查报表数据是否完整?
            sql = "SELECT * FROM ND24Rpt WHERE OID=" + subFlowWorkID;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("@发起流程出错误,不应该找不到子流程的报表数据.");

            foreach (DataColumn dc in dt.Columns)
            {
                string val = dt.Rows[0][dc.ColumnName].ToString();
                switch (dc.ColumnName)
                {
                    case GERptAttr.FID:
                        if (val != "0")
                            throw new Exception("@应当是0");
                        break;
                    case GERptAttr.FK_Dept:
                        if (val != empSubFlowStarter.FK_Dept)
                            throw new Exception("@应当是" + empSubFlowStarter.FK_Dept + ", 现在是:" + val);
                        break;
                    case GERptAttr.FK_NY:
                        if (val != DataType.CurrentYearMonth)
                            throw new Exception("@应当是" + DataType.CurrentYearMonth + ", 现在是:" + val);
                        break;
                    case GERptAttr.FlowDaySpan:
                        if (val != "0")
                            throw new Exception("@应当是 0 , 现在是:" + val);
                        break;
                    case GERptAttr.FlowEmps:
                        if (val.Contains(empSubFlowStarter.No) == false)
                            throw new Exception("@应当是包含当前人员, 现在是:" + val);
                        break;
                    case GERptAttr.FlowEnder:
                        if (val != empSubFlowStarter.No)
                            throw new Exception("@应当是 empSubFlowStarter.No, 现在是:" + val);
                        break;
                    case GERptAttr.FlowEnderRDT:
                        if (val.Contains(DataType.CurrentData) == false)
                            throw new Exception("@应当是 当前日期, 现在是:" + val);
                        break;
                    case GERptAttr.FlowEndNode:
                        if (val != "2401")
                            throw new Exception("@应当是 2401, 现在是:" + val);
                        break;
                    case GERptAttr.FlowStarter:
                        if (val != empSubFlowStarter.No)
                            throw new Exception("@应当是  WebUser.No, 现在是:" + val);
                        break;
                    case GERptAttr.FlowStartRDT:
                        if (DataType.IsNullOrEmpty(val))
                            throw new Exception("@应当不能为空,现在是:" + val);
                        break;
                    case GERptAttr.Title:
                        if (DataType.IsNullOrEmpty(val))
                            throw new Exception("@不能为空title" + val);
                        break;
                    case GERptAttr.WFState:
                        WFState sta = (WFState)int.Parse(val);
                        if (sta != WFState.Runing)
                            throw new Exception("@应当是  WFState.Runing 现在是" + sta.ToString());
                        break;
                    default:
                        break;
                }
            }
            #endregion 检查是否完整？

            // 测试以子流程向下发送，是否成功？
            BP.WF.Dev2Interface.Port_Login(empSubFlowStarter.No);
            objs = BP.WF.Dev2Interface.Node_SendWork("024", subFlowWorkID);
            if (objs.VarToNodeID != 2402)
                throw new Exception("@子流程向下发送时不成功.");
        }

        /// <summary>
        /// 测试子流程只有一个人
        /// </summary>
        private void Test2()
        {
            string fk_flow = "023";
            string userNo = "zhanghaicheng";
            Flow fl = new Flow(fk_flow);

            // zhanghaicheng 登录.
            BP.WF.Dev2Interface.Port_Login(userNo);

            //创建空白工作, 发起开始节点.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid); /*发送到第二个节点上去*/

            /*创建子流程. 指定可以处理子流程的处理人员是一个集合。 
             *
             * 此api多了两个参数：
             * 1，该流程隶属于那个部门. 
             * 2，该流程的参与人集合，用逗号分开.
             */

            Emp flowStarter = new Emp(WebUser.No);

            Int64 subFlowWorkID=0; // = new Emp(WebUser.No);

            //Int64 subFlowWorkID = BP.WF.Dev2Interface.Node_CreateStartNodeWork("024", null, null, "zhanghaicheng",
            //    "1", "zhoupeng,zhoushengyu", "子流程发起测试(为开始节点创建多人的工作处理)", "023", workid);


            #region 检查发起的子流程 流程引擎表 是否完整？
            GenerWorkFlow gwf = new GenerWorkFlow(subFlowWorkID);
            if (gwf.PFlowNo != "023")
                throw new Exception("@父流程编号错误,应当是023现在是" + gwf.PFlowNo);

            if (gwf.PWorkID != workid)
                throw new Exception("@父流程WorkID错误,应当是" + workid + "现在是" + gwf.PWorkID);

            if (gwf.Starter != flowStarter.No)
                throw new Exception("@流程发起人编号错误,应当是" + flowStarter.No + "现在是" + gwf.Starter);

            if (gwf.StarterName != flowStarter.Name)
                throw new Exception("@流程发起人 Name 错误,应当是" + flowStarter.Name + "现在是" + gwf.StarterName);

            if (gwf.FK_Dept != "1")
                throw new Exception("@流程隶属部门错误,应当是  1 现在是" + gwf.FK_Dept);

            if (gwf.Title != "子流程发起测试(为开始节点创建多人的工作处理)")
                throw new Exception("@流程标题 子流程发起测试 错误,应当是 子流程发起测试 现在是" + gwf.Title);

            if (gwf.WFState != WFState.Runing)
                throw new Exception("@流程 WFState 错误,应当是 Runing 现在是" + gwf.WFState);

            if (gwf.FID != 0)
                throw new Exception("@FID错误,应当是0现在是" + gwf.FID);

            if (gwf.FK_Flow != "024")
                throw new Exception("@FK_Flow错误,应当是024现在是" + gwf.FK_Flow);

            if (gwf.FK_Node != 2401)
                throw new Exception("@停留的当前节点错误,应当是2401现在是" + gwf.FK_Flow);

            GenerWorkerLists gwls = new GenerWorkerLists(subFlowWorkID, 2401);
            if (gwls.Count != 2)
                throw new Exception("@待办列表个数应当2,现在是" + gwls.Count);

            // 检查发起人列表是否完整？
            foreach (GenerWorkerList gwl in gwls)
            {
                if (gwl.IsPassInt != 0)
                    throw new Exception("@通过状态应当是未通过，现在是:" + gwl.IsPassInt);

                if (gwl.FID != 0)
                    throw new Exception("@流程ID 应当是0 ，现在是:" + gwl.FID);

                if (gwl.FK_Emp == "zhoupeng")
                {
                    Emp tempEmp = new Emp(gwl.FK_Emp);
                    if (gwl.FK_Dept != tempEmp.FK_Dept)
                        throw new Exception("@FK_Dept  错误, 现在是:" + gwl.FK_Dept);
                }
            }
            #endregion 检查发起的子流程 流程引擎表 是否完整？

            #region 检查发起的子流程数据是否完整？
            //检查报表数据是否完整?
            sql = "SELECT * FROM ND24Rpt WHERE OID=" + subFlowWorkID;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("@发起流程出错误,不应该找不到子流程的报表数据.");

            foreach (DataColumn dc in dt.Columns)
            {
                string val = dt.Rows[0][dc.ColumnName].ToString();
                switch (dc.ColumnName)
                {
                    case GERptAttr.PWorkID:
                        if (val != gwf.PWorkID.ToString() )
                            throw new Exception("@应当是父流程的workid,现是在:"+val);
                        break;
                    case GERptAttr.PFlowNo:
                        if (val != "023")
                            throw new Exception("@应当是023");
                        break;
                    case GERptAttr.FID:
                        if (val != "0")
                            throw new Exception("@应当是0");
                        break;
                    case GERptAttr.FK_Dept:
                        if (val != "1")
                            throw new Exception("@应当是  1, 现在是:" + val);
                        break;
                    case GERptAttr.FK_NY:
                        if (val != DataType.CurrentYearMonth)
                            throw new Exception("@应当是" + DataType.CurrentYearMonth + ", 现在是:" + val);
                        break;
                    case GERptAttr.FlowDaySpan:
                        if (val != "0")
                            throw new Exception("@应当是 0 , 现在是:" + val);
                        break;
                    //case GERptAttr.FlowEmps:
                    //    if (val.Contains(empSubFlowStarter.No) == false)
                    //        throw new Exception("@应当是包含当前人员, 现在是:" + val);
                    //    break;
                    //case GERptAttr.FlowEnder:
                    //    if (val != empSubFlowStarter.No)
                    //        throw new Exception("@应当是 empSubFlowStarter.No, 现在是:" + val);
                    //    break;
                    case GERptAttr.FlowEnderRDT:
                        if (val.Contains(DataType.CurrentData) == false)
                            throw new Exception("@应当是 当前日期, 现在是:" + val);
                        break;
                    case GERptAttr.FlowEndNode:
                        if (val != "2401")
                            throw new Exception("@应当是 2401, 现在是:" + val);
                        break;
                    //case GERptAttr.FlowStarter:
                    //    if (val != empSubFlowStarter.No)
                    //        throw new Exception("@应当是  WebUser.No, 现在是:" + val);
                    //    break;
                    case GERptAttr.FlowStartRDT:
                        if (DataType.IsNullOrEmpty(val))
                            throw new Exception("@应当不能为空,现在是:" + val);
                        break;
                    case GERptAttr.Title:
                        if (DataType.IsNullOrEmpty(val))
                            throw new Exception("@不能为空title" + val);
                        break;
                    case GERptAttr.WFState:
                        WFState sta = (WFState)int.Parse(val);
                        if (sta != WFState.Runing)
                            throw new Exception("@应当是  WFState.Runing 现在是" + sta.ToString());
                        break;
                    default:
                        break;
                }
            }
            #endregion 检查是否完整？

            // 测试以子流程向下发送，是否成功？
            BP.WF.Dev2Interface.Port_Login("zhoupeng");
            objs = BP.WF.Dev2Interface.Node_SendWork("024", subFlowWorkID);

            #region 检查返回来的变量数据完整性。
            if (objs.VarToNodeID != 2402)
                throw new Exception("@子流程向下发送时不成功.");

            #endregion 检查数据完整性。

            #region 检查其它的人在开始节点上是否还有待办工作？
            //检查报表数据是否完整?
            sql = "SELECT * FROM WF_EmpWorks WHERE WorkID=" + subFlowWorkID + " AND FK_Emp='zhoushengyu'";
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
                throw new Exception("@在开始节点一个人处理完成后，其它人还有待办.");

            #endregion 检查其它的人在开始节点上是否还有待办工作。
        }
    }
}
