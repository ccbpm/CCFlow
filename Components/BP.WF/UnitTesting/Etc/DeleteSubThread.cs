using System;
using System.Collections.Generic;
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
    public class DeleteSubThread : TestBase
    {
        /// <summary>
        /// 子线程的删除
        /// </summary>
        public DeleteSubThread()
        {
            this.Title = "子线程的删除";
            this.DescIt = "流程:005月销售总结(同表单分合流),删除一个子线程是否符合预期的要求.";
            this.EditState = EditState.Passed;
        }

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
        public Int64 workid = 0;
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
        /// 1, 此流程针对于最简单的分合流程进行， zhanghaicheng发起，zhoushengyu,zhangyifan,两个人处理子线程，
        ///    zhanghaicheng 接受子线程汇总数据.
        /// 2, 测试方法体分成三大部分. 发起，子线程处理，合流点执行，分别对应: Step1(), Step2_1(), Step2_2()，Step3() 方法。
        /// 3，针对发送测试，不涉及到其它的功能.
        /// </summary>
        public override void Do()
        {
            //初始化变量.
            fk_flow = "005";
            userNo = "zhanghaicheng";

            fl = new Flow(fk_flow);

            //执行第1步检查，创建工作与发送.
            this.Step1();

            //执行第2_1步检查，zhoushengyu的发送结果.
            this.Step2_1();

            ////执行第2_2步检查，zhangyifan的发送结果.
            //this.Step2_2();

            //删除 zhangyifan 的进程.
            this.Step3();
        }
        /// <summary>
        /// 创建流程，发送分流点第1步.
        /// </summary>
        public void Step1()
        {
            // 让zhanghaicheng 登录.
            BP.WF.Dev2Interface.Port_Login(userNo);

            //创建空白工作, 发起开始节点.
            workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            #region 检查 创建流程后的数据是否完整 ？
            // "检查创建这个空白是否有数据完整?;
            sql = "SELECT * FROM " + fl.PTable + " WHERE OID=" + workid;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("@发起流程出错误,不应该找不到报表数据.");

            // 检查节点表单表是否有数据?;
            sql = "SELECT * FROM ND501 WHERE OID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("@不应该在开始节点表单表中找不到数据，");

            if (dt.Rows[0]["Rec"].ToString() != WebUser.No)
                throw new Exception("@记录人应该是当前人员.");

            // 检查创建这个空白是否有数据完整?;
            sql = "SELECT * FROM WF_EmpWorks WHERE WorkID=" + workid + " AND FK_Emp='" + WebUser.No + "'";
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count !=0 )
                throw new Exception("@找到当前人员的待办就是错误的.");
            #endregion 检查发起流程后的数据是否完整？

            //开始节点:执行发送,并获取发送对象. 主线程向子线程发送.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);

            #region 第1步: 检查【开始节点】发送对象返回的信息是否完整？
            //从获取的发送对象里获取到下一个工作者. zhangyifan(张一帆)、zhoushengyu(周升雨).
            if (objs.VarAcceptersID != "zhangyifan,zhoushengyu,")
                throw new Exception("@下一步的接受人不正确,  zhangyifan,zhoushengyu, 现在是:" + objs.VarAcceptersID);

            if (objs.VarToNodeID != 502)
                throw new Exception("@应该是 502节点. 现在是:" + objs.VarToNodeID);

            if (objs.VarWorkID != workid)
                throw new Exception("@主线程的workid不应该变化:" + objs.VarWorkID);

            if (objs.VarCurrNodeID != 501)
                throw new Exception("@当前节点的编号不能变化:" + objs.VarCurrNodeID);

            if (objs.VarTreadWorkIDs == null)
                throw new Exception("@没有获取到两条子线程ID.");

            if (objs.VarTreadWorkIDs.Contains(",") == false)
                throw new Exception("@没有获取到两条子线程的WorkID:" + objs.VarTreadWorkIDs);
            #endregion  检查【开始节点】发送对象返回的信息是否完整？

            #region 第2步: 检查流程引擎控制系统表是否符合预期.
            gwf = new GenerWorkFlow(workid);
            if (gwf.FK_Node != 501)
                throw new Exception("@主线程向子线程发送时，主线程的FK_Node应该不变化，现在：" + gwf.FK_Node);

            if (gwf.WFState != WFState.Runing)
                throw new Exception("@主线程向子线程发送时，主线程的 WFState 应该 WFState.Runing ：" + gwf.WFState.ToString());

            if (gwf.Starter != WebUser.No)
                throw new Exception("@应该是发起人员，现在是:" + gwf.Starter );

            //找出发起人的工作列表.
            gwl = new GenerWorkerList(workid, 501, WebUser.No);
            if (gwl.IsPass == true)
                throw new Exception("@干流上的pass状态应该是通过,此人已经没有他的待办工作了.");

            //找出子线程上的工作人员.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkerListAttr.FID, workid);
            if (gwfs.Count != 2)
                throw new Exception("@应该有两个流程注册，现在是："+gwfs.Count+"个.");

            //检查它们的注册数据是否完整.
            foreach (GenerWorkFlow item in gwfs)
            {
                if (item.Starter != WebUser.No)
                    throw new Exception("@当前的人员应当是发起人,现在是:" + item.Starter);

                if (item.FK_Node != 502)
                    throw new Exception("@当前节点应当是 502 ,现在是:" + item.FK_Node);

                if (item.WFState != WFState.Runing)
                    throw new Exception("@当前 WFState 应当是 Runing ,现在是:" + item.WFState.ToString());
            }

            //找出子线程工作处理人员的工作列表.
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FID, workid);
            if (gwls.Count != 2)
                throw new Exception("@应该在子线程上查询出来两个待办，现在只有(" + gwls.Count + ")个。");

            //检查子线程的待办完整性.
            foreach (GenerWorkerList item in gwls)
            {
                if (item.IsPass)
                    throw new Exception("@不应该是已经通过，因为他们没有处理。");

                if (item.IsEnable == false)
                    throw new Exception("@应该是：IsEnable ");

                //if (item.Sender.Contains(WebUser.No) == false)
                //    throw new Exception("@发送人，应该是当前人员。现在是:" + item.Sender);

                if (item.FK_Flow != "005")
                    throw new Exception("@应该是 005 现在是:" + item.FK_Flow);

                if (item.FK_Node != 502)
                    throw new Exception("@应该是 502 现在是:" + item.FK_Node);
            }

            //取主线程的待办工作.
            sql = "SELECT * FROM WF_EmpWorks WHERE WorkID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
                throw new Exception("@不应当出现主线程的待办在 WF_EmpWorks 视图中. " + sql);

            //取待办子线程的待办工作.
            sql = "SELECT * FROM WF_EmpWorks WHERE FID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 2)
                throw new Exception("@应该取出来两个子线程的 WF_EmpWorks 视图中. " + sql);

            #endregion end 检查流程引擎控制系统表是否符合预期.

            #region 第3步: 检查【开始节点】发送节点表单-数据信息否完整？
            //检查节点表单表是否有数据？
            sql = "SELECT * FROM ND501 WHERE OID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@应该找到开始节点表单数据，但是没有。");

            if (dt.Rows[0]["Rec"].ToString() != WebUser.No)
                throw new Exception("@没有向主线程开始节点表里写入Rec字段，现在是：" + dt.Rows[0]["Rec"].ToString() + "应当是:" + WebUser.No);

            //检查节点表单表是否有数据，以及数据是否正确？
            sql = "SELECT * FROM ND502 WHERE FID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 2)
                throw new Exception("@应该在第一个子线程节点上找到两个数据。");
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Rec"].ToString() == "zhangyifan")
                {
                    continue;
                }
                if (dr["Rec"].ToString() == "zhoushengyu")
                {
                    continue;
                }
                throw new Exception("@子线程表单数据没有正确的写入Rec字段.");
            }


            sql = "SELECT * FROM  ND5Rpt WHERE OID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows[0][GERptAttr.FlowEnder].ToString() != "zhanghaicheng")
                throw new Exception("@应该是 zhanghaicheng 是 FlowEnder .");

            if (dt.Rows[0][GERptAttr.FlowStarter].ToString() != "zhanghaicheng")
                throw new Exception("@应该是 zhanghaicheng 是 FlowStarter .");

            if (dt.Rows[0][GERptAttr.FlowEndNode].ToString() != "502")
                throw new Exception("@应该是 502 是 FlowEndNode .");

            if (int.Parse(dt.Rows[0][GERptAttr.WFState].ToString()) != (int)WFState.Runing)
                throw new Exception("@应该是 WFState.Runing 是当前的状态。");

            if (int.Parse(dt.Rows[0][GERptAttr.FID].ToString()) != 0)
                throw new Exception("@应该是 FID =0 ");

            if (dt.Rows[0]["FK_NY"].ToString() != DataType.CurrentYearMonth)
                throw new Exception("@ FK_NY 字段填充错误. ");
            #endregion  检查【开始节点】发送数据信息否完整？
        }
        /// <summary>
        /// 让子线程中的一个人 zhoushengyu 登录, 然后执行向下发起.
        /// 检查业务逻辑是否正确？
        /// </summary>
        public void Step2_1()
        {
            //子线程中的接受人员, 分别是 zhoushengyu,zhangyifan

            // 让子线程中的一个人 zhoushengyu 登录, 然后执行向下发起,
            BP.WF.Dev2Interface.Port_Login("zhoushengyu");

            //获得此人的 005 的待办工作.
            dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(WebUser.No,WFState.Runing, "005");
            if (dt.Rows.Count == 0)
                throw new Exception("@不应该获取不到它的待办数据.");

            //获取子线程的workID.
            int threahWorkID = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (int.Parse(dr["FID"].ToString()) == workid)
                {
                    threahWorkID = int.Parse(dr["WorkID"].ToString());
                    break;
                }
            }
            if (threahWorkID == 0)
                throw new Exception("@不应当找不到它的待办。");

            // 执行 子线程向合流点发送.
            Hashtable ht = new Hashtable();
            ht.Add("FuWuQi",90);
            ht.Add("ShuMaXiangJi", 20);//把数据放里面去,让它保存到子线程的主表，以检查数据是否汇总到合流节点上。
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, threahWorkID, ht);

            #region 第1步: 检查发送后的变量.
            if (objs.VarWorkID != threahWorkID)
                throw new Exception("@应当是 VarWorkID=" + threahWorkID + " ，现在是:" + objs.VarWorkID);

            if (objs.VarCurrNodeID != 502)
                throw new Exception("@应当是 VarCurrNodeID=502 是，现在是:" + objs.VarCurrNodeID);

            if (objs.VarToNodeID != 599)
                throw new Exception("@应当是 VarToNodeID= 599 是，现在是:" + objs.VarToNodeID);

            if (objs.VarAcceptersID != "zhanghaicheng")
                throw new Exception("@应当是 VarAcceptersID= zhanghaicheng 是，现在是:" + objs.VarAcceptersID);
            #endregion 第1步: 检查发送后的变量.

            #region 第2步: 检查引擎控制系统表.
            //先检查干流数据.
            gwf = new GenerWorkFlow(workid);
            if (gwf.WFState != WFState.Runing)
                throw new Exception("@应当是 Runing, 现在是:" + gwf.WFState);

            if (gwf.FID != 0)
                throw new Exception("@应当是 0, 现在是:" + gwf.FID);

            if (gwf.FK_Node != 599)
                throw new Exception("@应当是 599, 现在是:" + gwf.FK_Node);

            if (gwf.Starter != "zhanghaicheng")
                throw new Exception("@应当是 zhanghaicheng, 现在是:" + gwf.Starter);

            // 干流的工作人员表是否有变化？
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, workid);
            foreach (GenerWorkerList item in gwls)
            {
                if (item.FK_Emp != "zhanghaicheng")
                    throw new Exception("@应当是 zhanghaicheng, 现在是:" + item.FK_Emp);

                //如果是开始节点.
                if (item.FK_Node == 501)
                {
                    if (item.IsPass == false)
                        throw new Exception("@pass状态错误了，应该是已通过。");
                }

                //如果是结束节点.
                if (item.FK_Node == 599)
                {
                    //检查子线程完成率. 
                    Node nd = new Node(599);
                    if (nd.PassRate > 50)
                    {
                        if (item.IsPassInt != 3)
                            throw new Exception("@因为完成率大于 50, 所以一个通过了，主线程的工作人员不能看到,现在是:"+item.IsPassInt);
                    }
                    else
                    {
                        if (item.IsPassInt != 0)
                            throw new Exception("@因为小于50，所以只要有一个通过了，主线程的zhanghaicheng 工作人员应该可以看到待办，但是没有查到。 ");
                    }
                }
            }

            //检查子线程的工作人员列表表。
            gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FID, workid);
            if (gwls.Count != 2)
                throw new Exception("@不是期望的两条子线程上的工作人员列表数据.");
            foreach (GenerWorkerList item in gwls)
            {
                if (item.FK_Emp == "zhoushengyu")
                {
                    if (item.IsPass == false)
                        throw new Exception("@此人应该是处理通过了，现在没有通过。");
                }

                if (item.FK_Emp == "zhangyifan")
                {
                    if (item.IsPass == true)
                        throw new Exception("@此人应该有待办，结果不符合预期。");
                }
            }
            #endregion 第2步: 检查引擎控制系统表.

            #region 第3步: 检查 节点表单表数据.
            sql = "SELECT * FROM ND501 WHERE OID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows[0]["Rec"].ToString() != "zhanghaicheng")
                throw new Exception("@开始节点的Rec 字段写入错误。");

            //检查节点表单表是否有数据，以及数据是否正确？
            sql = "SELECT * FROM ND502 WHERE FID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 2)
                throw new Exception("@应该在第一个子线程节点上找到两个数据。");
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Rec"].ToString() == "zhangyifan")
                    continue;
                if (dr["Rec"].ToString() == "zhoushengyu")
                    continue;
                throw new Exception("@子线程表单数据没有正确的写入Rec字段.");
            }

            //检查参数是否存储到子线程的主表上了？
            sql = "SELECT * FROM ND502 WHERE OID=" + threahWorkID;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@没有找到子线程期望的数据。");

            if (dt.Rows[0]["FuWuQi"].ToString()!="90")
                throw new Exception("没有存储到指定的位置.");

              if (dt.Rows[0]["ShuMaXiangJi"].ToString()!="20")
                throw new Exception("没有存储到指定的位置.");

              

            // 检查汇总的明细表数据是否copy正确？
              sql = "SELECT * FROM ND599Dtl1 WHERE OID=" + threahWorkID;
              dt = DBAccess.RunSQLReturnTable(sql);
              if (dt.Rows.Count != 1)
                  throw new Exception("@子线程的数据没有copy到汇总的明细表里.");
              dt = DBAccess.RunSQLReturnTable(sql);
              if (dt.Rows.Count != 1)
                  throw new Exception("@没有找到子线程期望的数据。");

              if (dt.Rows[0]["FuWuQi"].ToString() != "90")
                  throw new Exception("没有存储到指定的位置.");

              if (dt.Rows[0]["ShuMaXiangJi"].ToString() != "20")
                  throw new Exception("没有存储到指定的位置.");


            //检查报表数据是否正确？
            sql = "SELECT * FROM  ND5Rpt WHERE OID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows[0][GERptAttr.FlowEnder].ToString() != "zhanghaicheng")
                throw new Exception("@应该是 zhanghaicheng 是 FlowEnder .");

            if (dt.Rows[0][GERptAttr.FlowStarter].ToString() != "zhanghaicheng")
                throw new Exception("@应该是 zhanghaicheng 是 FlowStarter .");

            if (dt.Rows[0][GERptAttr.FlowEndNode].ToString() != "502")
                throw new Exception("@应该是 502 是 FlowEndNode .");

            if (int.Parse(dt.Rows[0][GERptAttr.WFState].ToString()) != (int)WFState.Runing)
                throw new Exception("@应该是 WFState.Runing 是 WFState .");

            if (int.Parse(dt.Rows[0][GERptAttr.FID].ToString()) != 0)
                throw new Exception("@应该是 FID =0 ");

            if (dt.Rows[0]["FK_NY"].ToString() != DataType.CurrentYearMonth)
                throw new Exception("@ FK_NY 字段填充错误. ");
            #endregion 第3步: 检查 节点表单表数据.
        }
        /// <summary>
        /// 执行zhanghaicheng的 发送。
        /// 1，检查发送的对象。
        /// 2，检查流程引擎控制表。
        /// 3，检查节点表。
        /// </summary>
        public void Step3()
        {
            // 让主线程上的发起人登录.
            BP.WF.Dev2Interface.Port_Login("zhanghaicheng");

            // 查询出来 zhangyifan 的workid.
            string sql = "SELECT WorkID FROM WF_GenerWorkerList WHERE FK_Emp='zhangyifan' AND FID=" + workid;
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

            if (dt.Rows.Count != 1)
                throw new Exception("@不应该找不到子线程的.");

            // 取得子线程ID.
            Int64 threakWorkID = Int64.Parse(dt.Rows[0][0].ToString());

            // 执行删除子线程. 删除子线程与删除流程是同一个方法。
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.fk_flow, threakWorkID, false);

            #region 第1步: 检查发送后的变量.
            // 检查 zhangyifan 是否有待办工作？
            sql = "SELECT COUNT(*) FROM WF_EmpWorks WHERE WorkID=" + threakWorkID;
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
                throw new Exception("@不应该找到该线程的待办工作.");

            //检查子流程数据是否还在?
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = threakWorkID;
            if (gwf.IsExits == true)
                throw new Exception("@子线程数据还存在，这是错误的.");


            //检查子流程数据是否还在?
            GenerWorkerList gwl = new GenerWorkerList();
            gwl.WorkID = threakWorkID;
            gwl.FID = workid;
            gwl.FK_Emp = "zhangyifan";
            if (gwl.IsExits == true)
                throw new Exception("@子线程数据的待办还存在，这是错误的.");



            //检查主流程数据是否还在?
              gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            if (gwf.IsExits == false)
                throw new Exception("@主线程数据已经不存在，这是错误的.");



            #endregion 第1步: 检查发送后的变量.


            // 删除测试数据.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.fk_flow, workid, false);

        }
    }
}
