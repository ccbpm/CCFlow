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
using BP.UnitTesting;

namespace BP.UnitTesting.SendCase
{
    public class Send009 : TestBase
    {
        /// <summary>
        /// 异表单分合流的发送
        /// </summary>
        public Send009()
        {
            this.Title = "异表单分合流的发送";
            this.DescIt = "流程:009部门年计划流程(异表单分合流),执行发送后的数据是否符合预期要求.";
            this.EditState = EditState.UnOK;
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
        /// 说明:
        /// 1, zhoupeng 启动.
        /// 2, 发送给三个人 zhoupeng，张海成 qifenglin，祁凤林、guoxiangbin，郭祥斌、。
        /// 3，zhoupeng 结束和合流点。
        /// </summary>
        public override void Do()
        {
            //初始化变量.
            fk_flow = "009";
            userNo = "zhoupeng";
            fl = new Flow(fk_flow);

            //执行第1步检查，创建工作与发送.
            this.Step1();

            //执行第2_1步检查，zhanghaicheng的发送结果.
            this.Step2_1();

            //执行第2_2步检查，qifenglin 的发送结果.
            this.Step2_2();

            //执行第2_3步检查，guoxiangbin 的发送结果.
            this.Step2_3();

            //最后的检查.
            this.Step3();
        }
        /// <summary>
        /// 创建流程，发送分流点第1步.
        /// </summary>
        public void Step1()
        {
            // 让zhoupeng 登录.
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
            sql = "SELECT * FROM ND901 WHERE OID=" + workid;
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

            //组织参数.
            Hashtable ht = new Hashtable();
            ht.Add("KeFuBu",1); 
            ht.Add("ShiChangBu", 1);
            ht.Add("YanFaBu", 1);

            //开始节点:执行发送,并获取发送对象. 主线程向子线程发送.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid,ht);

            #region 第1步: 检查【开始节点】发送对象返回的信息是否完整？
            //从获取的发送对象里获取到下一个工作者: zhanghaicheng,qifenglin,guoxiangbin .
            if (objs.VarAcceptersID != "zhanghaicheng,qifenglin,guoxiangbin,")
                throw new Exception("@下一步的接受人不正确,  zhanghaicheng,qifenglin,guoxiangbin, .现在是:" + objs.VarAcceptersID);

            if (objs.VarToNodeIDs != "902,903,904,")
                throw new Exception("@应该是 902,903,904,  现在是:" + objs.VarToNodeIDs);

            if (objs.VarWorkID != workid)
                throw new Exception("@主线程的workid不应该变化:" + objs.VarWorkID);

            if (objs.VarCurrNodeID != 901)
                throw new Exception("@当前节点的编号不能变化:" + objs.VarCurrNodeID);

            if (objs.VarTreadWorkIDs == null)
                throw new Exception("@没有获取到三条子线程ID.");

            if (objs.VarTreadWorkIDs.Contains(",") == false)
                throw new Exception("@没有获取到三条子线程的WorkID:" + objs.VarTreadWorkIDs);

            #endregion  检查【开始节点】发送对象返回的信息是否完整？

            #region 第2步: 检查流程引擎控制系统表是否符合预期.
            gwf = new GenerWorkFlow(workid);
            if (gwf.FK_Node != 901)
                throw new Exception("@主线程向子线程发送时，主线程的FK_Node应该不变化，现在：" + gwf.FK_Node);

            if (gwf.WFState != WFState.Runing)
                throw new Exception("@主线程向子线程发送时，主线程的 WFState 应该 WFState.Runing ：" + gwf.WFState.ToString());

            if (gwf.Starter != WebUser.No)
                throw new Exception("@应该是发起人员，现在是:" + gwf.Starter);

            //找出发起人的工作列表.
            gwl = new GenerWorkerList(workid, 901, WebUser.No);
            if (gwl.IsPass == true)
                throw new Exception("@干流上的pass状态应该是通过,此人已经没有他的待办工作了.");

            //找出子线程上的工作人员.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkerListAttr.FID, workid);
            if (gwfs.Count != 3)
                throw new Exception("@应该有两个流程注册，现在是："+gwfs.Count+"个.");

            //检查它们的注册数据是否完整.
            foreach (GenerWorkFlow item in gwfs)
            {
                if (item.Starter != WebUser.No)
                    throw new Exception("@当前的人员应当是发起人,现在是:" + item.Starter);

                //Node nd = new Node(item.FK_Node);
                //if (nd.iss

                //if (item.FK_Node == 901)
                //    throw new Exception("@当前节点应当是 902 ,现在是:" + item.FK_Node);

                if (item.WFState != WFState.Runing)
                    throw new Exception("@当前 WFState 应当是 Runing ,现在是:" + item.WFState.ToString());
            }

            //找出子线程工作处理人员的工作列表.
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FID, workid);
            if (gwls.Count != 3)
                throw new Exception("@应该在子线程上查询出来 3 个待办，现在只有(" + gwls.Count + ")个。");

            //检查子线程的待办完整性.
            foreach (GenerWorkerList item in gwls)
            {
                if (item.IsPass)
                    throw new Exception("@不应该是已经通过，因为他们没有处理。");

                if (item.IsEnable == false)
                    throw new Exception("@应该是：IsEnable ");

                //if (item.Sender.Contains(WebUser.No) == false)
                //    throw new Exception("@发送人，应该是当前人员。现在是:" + item.Sender);

                if (item.FK_Flow != "009")
                    throw new Exception("@应该是 009 现在是:" + item.FK_Flow);

                //if (item.FK_Node != 902)
                //    throw new Exception("@应该是 902 现在是:" + item.FK_Node);
            }

            //取主线程的待办工作.
            sql = "SELECT * FROM WF_EmpWorks WHERE WorkID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
                throw new Exception("@不应当出现主线程的待办在 WF_EmpWorks 视图中. " + sql);

            //取待办子线程的待办工作.
            sql = "SELECT * FROM WF_EmpWorks WHERE FID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 3)
                throw new Exception("@应该取出来两个子线程的 WF_EmpWorks 视图中. " + sql);

            #endregion end 检查流程引擎控制系统表是否符合预期.

            #region 第3步: 检查【开始节点】发送节点表单-数据信息否完整？
            //检查节点表单表是否有数据？
            sql = "SELECT * FROM ND901 WHERE OID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@应该找到开始节点表单数据，但是没有。");

            if (dt.Rows[0]["Rec"].ToString() != WebUser.No)
                throw new Exception("@没有向主线程开始节点表里写入Rec字段，现在是：" + dt.Rows[0]["Rec"].ToString() + "应当是:" + WebUser.No);

            //找出子线程工作处理人员的工作列表.
            gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FID, workid);

            //检查子线程节点数据表是否正确。
            foreach (GenerWorkerList item in gwls)
            {
                sql = "SELECT * FROM ND" + item.FK_Node + " WHERE OID=" + item.WorkID;
                dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count != 1)
                    throw new Exception("@不应该发现不到子线程节点数据。");

                foreach (DataColumn dc in dt.Columns)
                {
                    string val = dt.Rows[0][dc.ColumnName].ToString();
                    switch (dc.ColumnName)
                    {
                        case WorkAttr.FID:
                            if (val != workid.ToString())
                                throw new Exception("@不应当不等于workid.");
                            break;
                        case WorkAttr.Rec:
                            if (val != item.FK_Emp)
                                throw new Exception("@不应当不等于:" + item.FK_Emp);
                            break;
                        case WorkAttr.MyNum:
                            if (DataType.IsNullOrEmpty(val))
                                throw new Exception("@不应当为空:" + dc.ColumnName);
                            break;
                        case WorkAttr.RDT:
                            if (DataType.IsNullOrEmpty(val))
                                throw new Exception("@ RDT 不应当为空:" + dc.ColumnName);
                            break;
                        case WorkAttr.CDT:
                            if (DataType.IsNullOrEmpty(val))
                                throw new Exception("@ CDT 不应当为空:" + dc.ColumnName);
                            break;
                        case WorkAttr.Emps:
                            if (DataType.IsNullOrEmpty(val) || val.Contains(item.FK_Emp) == false)
                                throw new Exception("@ Emps 不应当为空，或者不包含发起人.");
                            break;
                        default:
                            break;
                    } //结束列的col判断。
                }
            }

            // 检查报表数据是否完整。
            sql = "SELECT * FROM ND9Rpt WHERE OID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataColumn dc in dt.Columns)
            {
                string val = dt.Rows[0][dc.ColumnName].ToString();
                switch (dc.ColumnName)
                {
                    case GERptAttr.FID:
                        if (int.Parse(val) != 0)
                            throw new Exception("@应该是 FID =0 ");
                        break;
                    case GERptAttr.FK_Dept:
                        if (val!=WebUser.FK_Dept)
                            throw new Exception("@ FK_Dept 字段填充错误,应当是:"+WebUser.FK_Dept+",现在是:"+val);
                        break;
                    case GERptAttr.FK_NY:
                        if (val != DataType.CurrentYearMonth)
                            throw new Exception("@ FK_NY 字段填充错误. ");
                        break;
                    case GERptAttr.FlowDaySpan:
                        if (val != "0")
                            throw new Exception("@ FlowDaySpan 应当是 0 . ");
                        break;
                    case GERptAttr.FlowEmps:
                        if (BP.WF.Glo.UserInfoShowModel != UserInfoShowModel.UserNameOnly)
                        {
                            if (val.Contains(WebUser.No) == false)
                                throw new Exception("@ 应该包含 zhoupeng , 现在是: " + val);
                        }
                        break;
                    case GERptAttr.FlowEnder:
                        if ( val != "zhoupeng")
                            throw new Exception("@应该是 zhoupeng 是 FlowEnder ,现在是:"+val );
                        break;
                    case GERptAttr.FlowEnderRDT:
                        break;
                    case GERptAttr.FlowEndNode:
                        if ( val != "901")
                            throw new Exception("@应该是 901 是 FlowEndNode,现在是:"+val);
                        break;
                    case GERptAttr.FlowStarter:
                        if ( val != "zhoupeng")
                            throw new Exception("@应该是 zhoupeng 是 FlowStarter, 现在是:"+val);
                        break;
                    case GERptAttr.MyNum:
                        if (val != "1")
                            throw new Exception("@ MyNum 应当是1, 现在是:"+val);
                        break;
                    case GERptAttr.PFlowNo:
                        if (val != "")
                            throw new Exception("@ PFlowNo 应当是 '' 现在是:"+val);
                        break;
                    case GERptAttr.PWorkID:
                        if (val != "0")
                            throw new Exception("@ PWorkID 应当是 '0' 现在是:" + val);
                        break;
                    case GERptAttr.Title:
                        if (DataType.IsNullOrEmpty(val))
                            throw new Exception("@ Title 不应当是空 " + val);
                        break;
                    case GERptAttr.WFState:
                        if (int.Parse(val) != (int)WFState.Runing)
                            throw new Exception("@应该是 WFState.Runing 是当前的状态,现在是："+val);
                        break;
                    default:
                        break;
                }
            }
            #endregion  检查【开始节点】发送数据信息否完整？
        }
        /// <summary>
        /// 让子线程中的一个人 zhoushengyu 登录, 然后执行向下发起.
        /// 检查业务逻辑是否正确？
        /// </summary>
        public void Step2_1()
        {
            //子线程中的接受人员, 分别是 zhanghaicheng,qifenglin,guoxiangbin

            // 让子线程中的一个人 zhanghaicheng 登录, 然后执行向下发起,
            BP.WF.Dev2Interface.Port_Login("zhanghaicheng");

            //获得此人的 009 的待办工作.
            dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(WebUser.No, WFState.Runing, "009");
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
            ht.Add("XianYouRenShu", 90);
            ht.Add("XinZengRenShu", 20);//把数据放里面去,让它保存到子线程的主表，以检查数据是否汇总到合流节点上。
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, threahWorkID, ht);

            #region 第1步: 检查发送后的变量.
            if (objs.VarWorkID != threahWorkID)
                throw new Exception("@应当是 VarWorkID=" + threahWorkID + " ，现在是:" + objs.VarWorkID);
            
            if (objs.VarCurrNodeID != 902)
                throw new Exception("@应当是 VarCurrNodeID=902 是，现在是:" + objs.VarCurrNodeID);

            if (objs.VarToNodeID != 999)
                throw new Exception("@应当是 VarToNodeID= 999 是，现在是:" + objs.VarToNodeID);

            if (objs.VarAcceptersID != "zhoupeng")
                throw new Exception("@应当是 VarAcceptersID= zhoupeng 是，现在是:" + objs.VarAcceptersID);
            #endregion 第1步: 检查发送后的变量.

            #region 第2步: 检查引擎控制系统表.
            //先检查干流数据.
            gwf = new GenerWorkFlow(workid);
            if (gwf.WFState != WFState.Runing)
                throw new Exception("@应当是 Runing, 现在是:" + gwf.WFState);

            if (gwf.FID != 0)
                throw new Exception("@应当是 0, 现在是:" + gwf.FID);

            if (gwf.FK_Node != 901)
                throw new Exception("@应当是 901, 现在是:" + gwf.FK_Node);

            if (gwf.Starter != "zhoupeng")
                throw new Exception("@应当是 zhoupeng, 现在是:" + gwf.Starter);

            // 干流的工作人员表是否有变化？
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, workid);
            foreach (GenerWorkerList item in gwls)
            {
                if (item.FK_Emp != "zhoupeng")
                    throw new Exception("@应当是 zhoupeng, 现在是:" + item.FK_Emp);

                //如果是开始节点.
                if (item.FK_Node == 901)
                {
                    if (item.IsPass == false)
                        throw new Exception("@pass状态错误了，应该是已通过。");
                }

                //如果是结束节点.
                if (item.FK_Node == 999)
                {
                    //检查子线程完成率. 
                    Node nd = new Node(999);
                    if (nd.PassRate > 50)
                    {
                        // 检查主线程数据是否正确.
                         sql = "SELECT COUNT(*) FROM WF_EmpWorks WHERE WorkID="+workid;
                        if (DBAccess.RunSQLReturnValInt(sql)!=0)
                            throw new Exception("@因为完成率大于 50, 所以一个通过了，主线程的工作人员不能看到.");

                        //if (item.IsPassInt != 3)
                        //    throw new Exception("@因为完成率大于 50, 所以一个通过了，主线程的工作人员不能看到,现在是:"+item.IsPassInt);
                    }
                    else
                    {
                        if (item.IsPassInt != 0)
                            throw new Exception("@因为小于50，所以只要有一个通过了，主线程的zhoupeng 工作人员应该可以看到待办，但是没有查到。 ");
                    }
                }
            }

            //检查子线程的工作人员列表表 。
            gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FID, workid);
            if (gwls.Count != 3)
                throw new Exception("@不是期望的两条子线程上的工作人员列表数据.");
            foreach (GenerWorkerList item in gwls)
            {
                if (item.FK_Emp == "zhanghaicheng")
                {
                    if (item.IsPass == false)
                        throw new Exception("@此人应该是处理通过了，现在没有通过。");
                }

                if (item.FK_Emp == "qifenglin")
                {
                    if (item.IsPass == true)
                        throw new Exception("@此人应该有待办，结果不符合预期。");
                }

                if (item.FK_Emp == "guoxiangbin")
                {
                    if (item.IsPass == true)
                        throw new Exception("@此人应该有待办，结果不符合预期。");
                }
            }
            #endregion 第2步: 检查引擎控制系统表.

            #region 第3步: 检查 节点表单表数据.
            sql = "SELECT * FROM ND901 WHERE OID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows[0]["Rec"].ToString() != "zhoupeng")
                throw new Exception("@开始节点的Rec 字段写入错误。");

            ////检查节点表单表是否有数据，以及数据是否正确？
            //sql = "SELECT * FROM ND902 WHERE FID=" + workid;
            //dt = DBAccess.RunSQLReturnTable(sql);
            //if (dt.Rows.Count != 2)
            //    throw new Exception("@应该在第一个子线程节点上找到两个数据");

            //foreach (DataRow dr in dt.Rows)
            //{
            //    if (dr["Rec"].ToString() == "zhangyifan")
            //        continue;
            //    if (dr["Rec"].ToString() == "zhoushengyu")
            //        continue;
            //    throw new Exception("@子线程表单数据没有正确的写入Rec字段.");
            //}

            ////检查参数是否存储到子线程的主表上了？
            //sql = "SELECT * FROM ND902 WHERE OID=" + threahWorkID;
            //dt = DBAccess.RunSQLReturnTable(sql);
            //if (dt.Rows.Count != 1)
            //    throw new Exception("@没有找到子线程期望的数据。");

            //if (dt.Rows[0]["FuWuQi"].ToString() != "90")
            //    throw new Exception("没有存储到指定的位置.");

            //if (dt.Rows[0]["ShuMaXiangJi"].ToString() != "20")
            //    throw new Exception("没有存储到指定的位置.");

            //// 检查汇总的明细表数据是否copy正确？
            //  sql = "SELECT * FROM ND999Dtl1 WHERE OID=" + threahWorkID;
            //  dt = DBAccess.RunSQLReturnTable(sql);
            //  if (dt.Rows.Count != 1)
            //      throw new Exception("@子线程的数据没有copy到汇总的明细表里.");
            //  dt = DBAccess.RunSQLReturnTable(sql);
            //  if (dt.Rows.Count != 1)
            //      throw new Exception("@没有找到子线程期望的数据。");

            //  if (dt.Rows[0]["FuWuQi"].ToString() != "90")
            //      throw new Exception("没有存储到指定的位置.");

            //  if (dt.Rows[0]["ShuMaXiangJi"].ToString() != "20")
            //      throw new Exception("没有存储到指定的位置.");

            //检查报表数据是否正确？
            sql = "SELECT * FROM  ND9Rpt WHERE OID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            string val = dt.Rows[0][GERptAttr.FlowEnder].ToString();
            //if (dt.Rows[0][GERptAttr.FlowEnder].ToString() != "zhoupeng")
            //    throw new Exception("@应该是 zhoupeng 是 FlowEnder, 现在是:"+val);

            if (dt.Rows[0][GERptAttr.FlowStarter].ToString() != "zhoupeng")
                throw new Exception("@应该是 zhoupeng 是 FlowStarter .");

            if (dt.Rows[0][GERptAttr.FlowEndNode].ToString() != "999")
                throw new Exception("@应该是 999 是 FlowEndNode，而现在是:" + dt.Rows[0][GERptAttr.FlowEndNode].ToString());

            if (int.Parse(dt.Rows[0][GERptAttr.WFState].ToString()) != (int)WFState.Runing)
                throw new Exception("@应该是 WFState.Runing 是 WFState .");

            if (int.Parse(dt.Rows[0][GERptAttr.FID].ToString()) != 0)
                throw new Exception("@应该是 FID =0 ");

            if (dt.Rows[0]["FK_NY"].ToString() != DataType.CurrentYearMonth)
                throw new Exception("@ FK_NY 字段填充错误. ");
            #endregion 第3步: 检查 节点表单表数据.
        }
        /// <summary>
        /// 每个子线程向下发送
        /// </summary>
        public void Step2_2()
        {
            // 让子线程中的一个人 zhoushengyu 登录, 然后执行向下发起,
            BP.WF.Dev2Interface.Port_Login("qifenglin");

            //获得此人的 009 的待办工作.
            dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(WebUser.No, WFState.Runing, "009");
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
            ht.Add("FuWuQi", 100);
            ht.Add("ShuMaXiangJi", 30);//把数据放里面去,让它保存到子线程的主表，以检查数据是否汇总到合流节点上。
    
            // 执行 子线程向合流点发送.
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, threahWorkID, ht);

            #region 第1步: 检查发送后的变量.
            if (objs.VarWorkID != threahWorkID)
                throw new Exception("@应当是 VarWorkID=" + threahWorkID + " ，现在是:" + objs.VarWorkID);

            if (objs.VarCurrNodeID != 903)
                throw new Exception("@应当是 VarCurrNodeID=903 是，现在是:" + objs.VarCurrNodeID);

            if (objs.VarToNodeID != 999)
                throw new Exception("@应当是 VarToNodeID= 999 是，现在是:" + objs.VarToNodeID);

            //if (objs.VarAcceptersID != "zhoupeng")
            //    throw new Exception("@应当是 VarAcceptersID= zhoupeng 是，现在是:" + objs.VarAcceptersID);
            #endregion 第1步: 检查发送后的变量.

            #region 第2步: 检查引擎控制系统表.
            //先检查干流数据.
            gwf = new GenerWorkFlow(workid);
            if (gwf.WFState != WFState.Runing)
                throw new Exception("@应当是 Runing, 现在是:" + gwf.WFState);

            if (gwf.FID != 0)
                throw new Exception("@应当是 0, 现在是:" + gwf.FID);

            if (gwf.FK_Node != 901)
                throw new Exception("@应当是 901, 现在是:" + gwf.FK_Node);

            if (gwf.Starter != "zhoupeng")
                throw new Exception("@应当是 zhoupeng, 现在是:" + gwf.Starter);

            // 干流的工作人员表是否有变化？
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, workid);
            foreach (GenerWorkerList item in gwls)
            {
                if (item.FK_Emp != "zhoupeng")
                    throw new Exception("@应当是 zhoupeng, 现在是:" + item.FK_Emp);

                //如果是开始节点.
                if (item.FK_Node == 901)
                {
                    if (item.IsPass == false)
                        throw new Exception("@pass状态错误了，应该是已通过。");
                }

                //如果是结束节点.
                if (item.FK_Node == 999)
                {
                    //检查子线程完成率.
                    Node nd = new Node(999);
                    if (nd.PassRate > 50)
                    {
                        if (item.IsPassInt != 0)
                            throw new Exception("@因为完成率大于 50, 现在两个都通过了，所以这合流点上也应该是通过的状态。");
                    }
                    else
                    {
                        if (item.IsPassInt != 0)
                            throw new Exception("@因为小于50，所以只要有一个通过了，主线程的zhoupeng 工作人员应该可以看到待办，但是没有查到。 ");
                    }
                }
            }

            //检查子线程的工作人员列表表。
            gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FID, workid);
            if (gwls.Count != 3)
                throw new Exception("@不是期望的两条子线程上的工作人员列表数据, 应该是3,现在是:"+gwls.Count);
            foreach (GenerWorkerList item in gwls)
            {
                if (item.FK_Emp == "zhanghaicheng")
                {
                    if (item.IsPass == false)
                        throw new Exception("@此人应该是处理通过了，现在没有通过。");
                }

                if (item.FK_Emp == "qifenglin")
                {
                    if (item.IsPass == false)
                        throw new Exception("@此人应该是处理通过了，现在没有通过。");
                }

                if (item.FK_Emp == "guoxiangbin")
                {
                    if (item.IsPass == true)
                        throw new Exception("@此人应该有待办，结果不符合预期。");
                }
            }
            #endregion 第2步: 检查引擎控制系统表.

            #region 第3步: 检查 节点表单表数据.
            sql = "SELECT * FROM ND901 WHERE OID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows[0]["Rec"].ToString() != "zhoupeng")
                throw new Exception("@开始节点的Rec 字段写入错误。");

            ////检查节点表单表是否有数据，以及数据是否正确？
            //sql = "SELECT * FROM ND902 WHERE FID=" + workid;
            //dt = DBAccess.RunSQLReturnTable(sql);
            //if (dt.Rows.Count != 3)
            //    throw new Exception("@应该在第一个子线程节点上找到 3 个数据,现在是:"+dt.Rows.Count);]

            //foreach (DataRow dr in dt.Rows)
            //{
            //    if (dr["Rec"].ToString() == "zhangyifan")
            //    {
            //        continue;
            //    }
            //    if (dr["Rec"].ToString() == "zhoushengyu")
            //    {
            //        continue;
            //    }
            //    throw new Exception("@子线程表单数据没有正确的写入Rec字段.");
            //}

            ////检查参数是否存储到子线程的主表上了？
            //sql = "SELECT * FROM ND902 WHERE OID=" + threahWorkID;
            //dt = DBAccess.RunSQLReturnTable(sql);
            //if (dt.Rows.Count != 1)
            //    throw new Exception("@没有找到子线程期望的数据。");

            //if (dt.Rows[0]["FuWuQi"].ToString() != "100")
            //    throw new Exception("没有存储到指定的位置.");

            //if (dt.Rows[0]["ShuMaXiangJi"].ToString() != "30")
            //    throw new Exception("没有存储到指定的位置.");


            //// 检查汇总的明细表数据是否copy正确？
            //sql = "SELECT * FROM ND999Dtl1 WHERE OID=" + threahWorkID;
            //dt = DBAccess.RunSQLReturnTable(sql);
            //if (dt.Rows.Count != 1)
            //    throw new Exception("@子线程的数据没有copy到汇总的明细表里.");
            //dt = DBAccess.RunSQLReturnTable(sql);
            //if (dt.Rows.Count != 1)
            //    throw new Exception("@没有找到子线程期望的数据。");

            //if (dt.Rows[0]["FuWuQi"].ToString() != "100")
            //    throw new Exception("没有存储到指定的位置.");

            //if (dt.Rows[0]["ShuMaXiangJi"].ToString() != "30")
            //    throw new Exception("没有存储到指定的位置.");
             

            //检查报表数据是否正确？
            sql = "SELECT * FROM  ND9Rpt WHERE OID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            //if (dt.Rows[0][GERptAttr.FlowEnder].ToString() != "zhoupeng")
            //    throw new Exception("@应该是 zhoupeng 是 FlowEnder .");

            if (dt.Rows[0][GERptAttr.FlowStarter].ToString() != "zhoupeng")
                throw new Exception("@应该是 zhoupeng 是 FlowStarter .");

            if (dt.Rows[0][GERptAttr.FlowEndNode].ToString() != "999")
                throw new Exception("@应该是 999 是 FlowEndNode，现在是:" + dt.Rows[0][GERptAttr.FlowEndNode].ToString());

            if (int.Parse(dt.Rows[0][GERptAttr.WFState].ToString()) != (int)WFState.Runing)
                throw new Exception("@应该是 WFState.Runing 是 WFState .");

            if (int.Parse(dt.Rows[0][GERptAttr.FID].ToString()) != 0)
                throw new Exception("@应该是 FID =0 ");

            if (dt.Rows[0]["FK_NY"].ToString() != DataType.CurrentYearMonth)
                throw new Exception("@ FK_NY 字段填充错误. ");
            #endregion 第3步: 检查 节点表单表数据.
        }
        /// <summary>
        /// 每个子线程向下发送
        /// </summary>
        public void Step2_3()
        {
            // 让子线程中的一个人 zhoushengyu 登录, 然后执行向下发起,
            BP.WF.Dev2Interface.Port_Login("guoxiangbin");

            //获得此人的 009 的待办工作.
            dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(WebUser.No, WFState.Runing, "009");
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
            ht.Add("FuWuQi", 100);
            ht.Add("ShuMaXiangJi", 30);//把数据放里面去,让它保存到子线程的主表，以检查数据是否汇总到合流节点上。

            // 执行 子线程向合流点发送.
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, threahWorkID, ht);

            #region 第1步: 检查发送后的变量.
            if (objs.VarWorkID != threahWorkID)
                throw new Exception("@应当是 VarWorkID=" + threahWorkID + " ，现在是:" + objs.VarWorkID);

            if (objs.VarCurrNodeID != 904)
                throw new Exception("@应当是 VarCurrNodeID=904 是，现在是:" + objs.VarCurrNodeID);

            if (objs.VarToNodeID != 999)
                throw new Exception("@应当是 VarToNodeID= 999 是，现在是:" + objs.VarToNodeID);

            //if (objs.VarAcceptersID != "zhoupeng")
            //    throw new Exception("@应当是 VarAcceptersID= zhoupeng 是，现在是:" + objs.VarAcceptersID);
            #endregion 第1步: 检查发送后的变量.

            #region 第2步: 检查引擎控制系统表.
            //先检查干流数据.
            gwf = new GenerWorkFlow(workid);
            if (gwf.WFState != WFState.Runing)
                throw new Exception("@应当是 Runing, 现在是:" + gwf.WFState);

            if (gwf.FID != 0)
                throw new Exception("@应当是 0, 现在是:" + gwf.FID);

            if (gwf.FK_Node != 999)
                throw new Exception("@应当是 999, 现在是:" + gwf.FK_Node);

            if (gwf.Starter != "zhoupeng")
                throw new Exception("@应当是 zhoupeng, 现在是:" + gwf.Starter);

            // 干流的工作人员表是否有变化？
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, workid);
            foreach (GenerWorkerList item in gwls)
            {
                if (item.FK_Emp != "zhoupeng")
                    throw new Exception("@应当是 zhoupeng, 现在是:" + item.FK_Emp);

                //如果是开始节点.
                if (item.FK_Node == 901)
                {
                    if (item.IsPass == false)
                        throw new Exception("@pass状态错误了，应该是已通过。");
                }

                //如果是结束节点.
                if (item.FK_Node == 999)
                {
                    //检查子线程完成率.
                    Node nd = new Node(999);
                    if (nd.PassRate > 50)
                    {
                        if (item.IsPassInt != 0)
                            throw new Exception("@因为完成率大于 50, 现在两个都通过了，所以这合流点上也应该是通过的状态。");
                    }
                    else
                    {
                        if (item.IsPassInt != 0)
                            throw new Exception("@因为小于50，所以只要有一个通过了，主线程的zhoupeng 工作人员应该可以看到待办，但是没有查到。 ");
                    }
                }
            }

            //检查子线程的工作人员列表表。
            gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FID, workid);
            if (gwls.Count != 3)
                throw new Exception("@不是期望的 3 条子线程上的工作人员列表数据.");

            foreach (GenerWorkerList item in gwls)
            {
                if (item.FK_Emp == "zhanghaicheng")
                {
                    if (item.IsPass == false)
                        throw new Exception("@ zhanghaicheng 应该是处理通过了，现在没有通过。IsPass=" + item.IsPassInt);
                }

                if (item.FK_Emp == "qifenglin")
                {
                    if (item.IsPass == false)
                        throw new Exception("@ qifenglin 应该是处理通过了，现在没有通过。IsPass=" + item.IsPassInt);
                }

                if (item.FK_Emp == "guoxiangbin")
                {
                    if (item.IsPass == false)
                        throw new Exception("@ guoxiangbin 应该是处理通过了，现在没有通过。IsPass=" + item.IsPassInt);
                }
            }
            #endregion 第2步: 检查引擎控制系统表.

            #region 第3步: 检查 节点表单表数据.
            sql = "SELECT * FROM ND901 WHERE OID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows[0]["Rec"].ToString() != "zhoupeng")
                throw new Exception("@开始节点的Rec 字段写入错误。");


            //// 检查汇总的明细表数据是否copy正确？
            //sql = "SELECT * FROM ND999Dtl1 WHERE OID=" + threahWorkID;
            //dt = DBAccess.RunSQLReturnTable(sql);
            //if (dt.Rows.Count != 1)
            //    throw new Exception("@子线程的数据没有copy到汇总的明细表里.");
            //dt = DBAccess.RunSQLReturnTable(sql);
            //if (dt.Rows.Count != 1)
            //    throw new Exception("@没有找到子线程期望的数据。");

            //if (dt.Rows[0]["FuWuQi"].ToString() != "100")
            //    throw new Exception("没有存储到指定的位置.");

            //if (dt.Rows[0]["ShuMaXiangJi"].ToString() != "30")
            //    throw new Exception("没有存储到指定的位置.");

            //检查报表数据是否正确？
            sql = "SELECT * FROM  ND9Rpt WHERE OID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            //if (dt.Rows[0][GERptAttr.FlowEnder].ToString() != "zhoupeng")
            //    throw new Exception("@应该是 zhoupeng 是 FlowEnder ." +);

            if (dt.Rows[0][GERptAttr.FlowStarter].ToString() != "zhoupeng")
                throw new Exception("@应该是 zhoupeng 是 FlowStarter .");

            if (dt.Rows[0][GERptAttr.FlowEndNode].ToString() != "999")
                throw new Exception("@应该是 999 是 FlowEndNode .");

            if (int.Parse(dt.Rows[0][GERptAttr.WFState].ToString()) != (int)WFState.Runing)
                throw new Exception("@应该是 WFState.Runing 是 WFState .");

            if (int.Parse(dt.Rows[0][GERptAttr.FID].ToString()) != 0)
                throw new Exception("@应该是 FID =0 ");

            if (dt.Rows[0]["FK_NY"].ToString() != DataType.CurrentYearMonth)
                throw new Exception("@ FK_NY 字段填充错误. ");

            #endregion 第3步: 检查 节点表单表数据.
        }
        /// <summary>
        /// 执行zhoupeng的 发送。
        /// 1，检查发送的对象。
        /// 2，检查流程引擎控制表。
        /// 3，检查节点表。
        /// </summary>
        public void Step3()
        {
            // 让主线程上的发起人登录。
            BP.WF.Dev2Interface.Port_Login("zhoupeng");

            // 执行向最后一个节点发送
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);

            #region 第1步: 检查发送后的变量.
            if (objs.VarWorkID != workid)
                throw new Exception("@应当是 VarWorkID=" + workid + " ，现在是:" + objs.VarWorkID);

            if (objs.VarCurrNodeID != 999)
                throw new Exception("@应当是 VarCurrNodeID=999 是，现在是:" + objs.VarCurrNodeID);

            if (objs.VarToNodeID != 0)
                throw new Exception("@应当是 VarToNodeID= 0 是，现在是:" + objs.VarToNodeID);

            if (objs.VarAcceptersID != null)
                throw new Exception("@应当是 VarAcceptersID= null 是，现在是:" + objs.VarAcceptersID);
            #endregion 第1步: 检查发送后的变量.

            #region 第2步: 检查引擎控制系统表.
            //检查主线程.
            sql = "SELECT * FROM WF_GenerWorkFlow WHERE WorkID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            

            sql = "SELECT * FROM WF_GenerWorkerlist WHERE WorkID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
                throw new Exception("@流程结束了，引擎表的数据没有删除。");

            //检查子线程.
            sql = "SELECT * FROM WF_GenerWorkFlow WHERE FID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);

            sql = "SELECT * FROM WF_GenerWorkerlist WHERE FID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
                throw new Exception("@流程结束了，引擎表的数据没有删除。");
            #endregion 第2步: 检查引擎控制系统表.

            #region 第3步: 检查 节点表单表数据.
            //检查明细表合流点上的汇总数据。
            sql = "SELECT * FROM  ND999Dtl1 WHERE RefPK=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 3)
                throw new Exception("@丢失了明细表的汇总数据");

            //int sum = 0;
            //foreach (DataRow dr in dt.Rows)
            //{
            //    sum += int.Parse(dr["FuWuQi"].ToString());
            //}
            //if (sum!=190)
            //    throw new Exception("@明细表的汇总数据错误了");

            //检查报表数据是否正确？
            sql = "SELECT * FROM  ND9Rpt WHERE OID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows[0][GERptAttr.FlowEnder].ToString() != "zhoupeng")
                throw new Exception("@应该是 zhoupeng 是 FlowEnder .");

               if ( DataType.IsNullOrEmpty(dt.Rows[0][GERptAttr.Title].ToString()) )
                            throw new Exception("@流程走完后标题丢失了");

            if (dt.Rows[0][GERptAttr.FlowStarter].ToString() != "zhoupeng")
                throw new Exception("@应该是 zhoupeng 是 FlowStarter .");

            if (dt.Rows[0][GERptAttr.FlowEndNode].ToString() != "999")
                throw new Exception("@应该是 999 是 FlowEndNode .");

            if (dt.Rows[0][GERptAttr.FlowEnder].ToString() != "zhoupeng")
                throw new Exception("@应该是 zhoupeng 是 FlowEnder .");

            if (int.Parse(dt.Rows[0][GERptAttr.WFState].ToString()) != (int)WFState.Complete)
                throw new Exception("@应该是 WFState.Complete 是当前的状态 .");

            if (int.Parse(dt.Rows[0][GERptAttr.FID].ToString()) != 0)
                throw new Exception("@应该是 FID =0 ");

            if (dt.Rows[0]["FK_NY"].ToString() != DataType.CurrentYearMonth)
                throw new Exception("@ FK_NY 字段填充错误. ");

            // 检查子线程节点表里的数据是否存在？
            sql = "SELECT * FROM ND902 WHERE FID=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@应该在第一个子线程节点上找到1个数据。");
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Rec"].ToString() == "zhanghaicheng")
                    continue;
                if (dr["Rec"].ToString() == "qifenglin")
                    continue;
                if (dr["Rec"].ToString() == "guoxiangbin")
                    continue;
                throw new Exception("@子线程表单数据没有正确的写入Rec字段.");
            }
            #endregion 第3步: 检查 节点表单表数据.
           
        }
    }
}
