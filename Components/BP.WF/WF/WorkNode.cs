using System;
using BP.En;
using BP.DA;
using System.Collections;
using System.Data;
using BP.Port;
using BP.Web;
using BP.Sys;
using BP.WF.Template;
using BP.WF.Data;

namespace BP.WF
{
    /// <summary>
    /// WF 的摘要说明.
    /// 工作流.
    /// 这里包含了两个方面
    /// 工作的信息．
    /// 流程的信息.
    /// </summary>
    public class WorkNode
    {
        #region 权限判断
        /// <summary>
        /// 判断一个人能不能对这个工作节点进行操作。
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        private bool IsCanOpenCurrentWorkNode(string empId)
        {
            WFState stat = this.HisGenerWorkFlow.WFState;
            if (stat == WFState.Runing)
            {
                if (this.HisNode.IsStartNode)
                {
                    /*如果是开始工作节点，从工作岗位判断他有没有工作的权限。*/
                    return WorkFlow.IsCanDoWorkCheckByEmpStation(this.HisNode.NodeID, empId);
                }
                else
                {
                    /* 如果是初始化阶段,判断他的初始化节点 */
                    GenerWorkerList wl = new GenerWorkerList();
                    wl.WorkID = this.HisWork.OID;
                    wl.FK_Emp = empId;

                    Emp myEmp = new Emp(empId);
                    wl.FK_EmpText = myEmp.Name;

                    wl.FK_Node = this.HisNode.NodeID;
                    wl.FK_NodeText = this.HisNode.Name;
                    return wl.IsExits;
                }
            }
            else
            {
                /* 如果是初始化阶段 */
                return false;
            }
        }
        #endregion

        #region 属性/变量.
        /// <summary>
        /// 子线程是否有分组标志.
        /// </summary>
        private bool IsHaveSubThreadGroupMark = false;


        private string _execer = null;
        /// <summary>
        /// 实际执行人，执行工作发送时，有时候当前 WebUser.No 并非实际的执行人。
        /// </summary>
        public string Execer
        {
            get
            {
                if (_execer == null || _execer == "")
                    _execer = WebUser.No;
                return _execer;
            }
            set
            {
                _execer = value;
            }
        }
        private string _execerName = null;
        /// <summary>
        /// 实际执行人名称(请参考实际执行人)
        /// </summary>
        public string ExecerName
        {
            get
            {
                if (_execerName == null || _execerName == "")
                    _execerName = WebUser.Name;
                return _execerName;
            }
            set
            {
                _execerName = value;
            }
        }
        private string _execerDeptName = null;
        /// <summary>
        /// 实际执行人名称(请参考实际执行人)
        /// </summary>
        public string ExecerDeptName
        {
            get
            {
                if (_execerDeptName == null)
                    _execerDeptName = WebUser.FK_DeptName;
                return _execerDeptName;
            }
            set
            {
                _execerDeptName = value;
            }
        }
        private string _execerDeptNo = null;
        /// <summary>
        /// 实际执行人名称(请参考实际执行人)
        /// </summary>
        public string ExecerDeptNo
        {
            get
            {
                if (_execerDeptNo == null)
                    _execerDeptNo = WebUser.FK_Dept;
                return _execerDeptNo;
            }
            set
            {
                _execerDeptNo = value;
            }
        }
        /// <summary>
        /// 虚拟目录的路径
        /// </summary>
        private string _VirPath = null;
        /// <summary>
        /// 虚拟目录的路径 
        /// </summary>
        public string VirPath
        {
            get
            {
                if (_VirPath == null && BP.Sys.SystemConfig.IsBSsystem)
                    _VirPath = Glo.CCFlowAppPath;//BP.Sys.Glo.Request.ApplicationPath;
                return _VirPath;
            }
        }
        private string _AppType = null;
        /// <summary>
        /// 虚拟目录的路径
        /// </summary>
        public string AppType
        {
            get
            {
                if (BP.Sys.SystemConfig.IsBSsystem == false)
                {
                    return "CCFlow";
                }

                if (_AppType == null && BP.Sys.SystemConfig.IsBSsystem)
                {
                    if (BP.Web.WebUser.IsWap)
                        _AppType = "WF/WAP";
                    else
                    {
                        bool b = BP.Sys.Glo.Request.RawUrl.ToLower().Contains("oneflow");
                        if (b)
                            _AppType = "WF/OneFlow";
                        else
                            _AppType = "WF";
                    }
                }
                return _AppType;
            }
        }
        private string nextStationName = "";
        public WorkNode town = null;
        private bool IsFindWorker = false;
        public bool IsSubFlowWorkNode
        {
            get
            {
                if (this.HisWork.FID == 0)
                    return false;
                else
                    return true;
            }
        }
        #endregion 属性/变量.

        #region GenerWorkerList 相关方法.
        //查询出每个节点表里的接收人集合（Emps）。
        public string GenerEmps(Node nd)
        {
            string str = "";
            foreach (GenerWorkerList wl in this.HisWorkerLists)
                str = wl.FK_Emp + ",";
            return str;
        }
        /// <summary>
        /// 产生它的工作者
        /// </summary>
        /// <param name="town">WorkNode</param>
        /// <returns>产生的工作人员</returns>
        public GenerWorkerLists Func_GenerWorkerLists(WorkNode town)
        {
            this.town = town;
            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(string));
            string sql;
            string FK_Emp;

            // 如果指定特定的人员处理。
            if (DataType.IsNullOrEmpty(JumpToEmp) == false)
            {
                string[] emps = JumpToEmp.Split(',');
                foreach (string emp in emps)
                {
                    if (DataType.IsNullOrEmpty(emp))
                        continue;
                    DataRow dr = dt.NewRow();
                    dr[0] = emp;
                    dt.Rows.Add(dr);
                }

                /*如果是抢办或者共享.*/

                // 如果执行了两次发送，那前一次的轨迹就需要被删除,这里是为了避免错误。
                ps = new Paras();
                ps.Add("WorkID", this.HisWork.OID);
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FK_Node =" + dbStr + "FK_Node";
                DBAccess.RunSQL(ps);

                return InitWorkerLists(town, dt);
            }

            // 如果执行了两次发送，那前一次的轨迹就需要被删除,这里是为了避免错误,
            ps = new Paras();
            ps.Add("WorkID", this.HisWork.OID);
            ps.Add("FK_Node", town.HisNode.NodeID);
            ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FK_Node =" + dbStr + "FK_Node";
            DBAccess.RunSQL(ps);

            if (this.town.HisNode.HisDeliveryWay == DeliveryWay.ByCCFlowBPM
                || 1 == 1)
            {
                /*如果设置了安ccbpm的BPM模式*/
                while (true)
                {
                    FindWorker fw = new FindWorker();
                    dt = fw.DoIt(this.HisFlow, this, town);
                    if (dt == null)
                        throw new Exception("@没有找到接收人.");

                    return InitWorkerLists(town, dt);
                }
            }

            throw new Exception("@此部分代码已经移除了.");
        }
        #endregion GenerWorkerList 相关方法.

        /// <summary>
        /// 生成一个 word
        /// </summary>
        public void DoPrint()
        {
            string tempFile = SystemConfig.PathOfTemp + "\\" + this.WorkID + ".doc";
            Work wk = this.HisNode.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();
            Glo.GenerWord(tempFile, wk);
            PubClass.OpenWordDocV2(tempFile, this.HisNode.Name + ".doc");
        }
        string dbStr = SystemConfig.AppCenterDBVarStr;
        public Paras ps = new Paras();
        /// <summary>
        /// 递归删除两个节点之间的数据
        /// </summary>
        /// <param name="nds">到达的节点集合</param>
        public void DeleteToNodesData(Nodes nds)
        {
            if (this.HisFlow.HisDataStoreModel == DataStoreModel.SpecTable)
                return;

            /*开始遍历到达的节点集合*/
            foreach (Node nd in nds)
            {
                Work wk = nd.HisWork;
                if (wk.EnMap.PhysicsTable == this.HisFlow.PTable)
                    continue;

                wk.OID = this.WorkID;
                if (wk.Delete() == 0)
                {
                    wk.FID = this.WorkID;
                    if (wk.EnMap.PhysicsTable == this.HisFlow.PTable)
                        continue;
                    if (wk.Delete(WorkAttr.FID, this.WorkID) == 0)
                        continue;
                }

                #region 删除当前节点数据，删除附件信息。
                // 删除明细表信息。
                MapDtls dtls = new MapDtls("ND" + nd.NodeID);
                foreach (MapDtl dtl in dtls)
                {
                    ps = new Paras();
                    ps.SQL = "DELETE FROM " + dtl.PTable + " WHERE RefPK=" + dbStr + "WorkID";
                    ps.Add("WorkID", this.WorkID.ToString());
                    BP.DA.DBAccess.RunSQL(ps);
                }

                // 删除表单附件信息。
                BP.DA.DBAccess.RunSQL("DELETE FROM Sys_FrmAttachmentDB WHERE RefPKVal=" + dbStr + "WorkID AND FK_MapData=" + dbStr + "FK_MapData ",
                    "WorkID", this.WorkID.ToString(), "FK_MapData", "ND" + nd.NodeID);
                // 删除签名信息。
                BP.DA.DBAccess.RunSQL("DELETE FROM Sys_FrmEleDB WHERE RefPKVal=" + dbStr + "WorkID AND FK_MapData=" + dbStr + "FK_MapData ",
                    "WorkID", this.WorkID.ToString(), "FK_MapData", "ND" + nd.NodeID);
                #endregion 删除当前节点数据。

                /*说明:已经删除该节点数据。*/
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE (WorkID=" + dbStr + "WorkID1 OR FID=" + dbStr + "WorkID2 ) AND FK_Node=" + dbStr + "FK_Node",
                    "WorkID1", this.WorkID, "WorkID2", this.WorkID, "FK_Node", nd.NodeID);

                if (nd.IsFL)
                {
                    /* 如果是分流 */
                    GenerWorkerLists wls = new GenerWorkerLists();
                    QueryObject qo = new QueryObject(wls);
                    qo.AddWhere(GenerWorkerListAttr.FID, this.WorkID);
                    qo.addAnd();

                    string[] ndsStrs = nd.HisToNDs.Split('@');
                    string inStr = "";
                    foreach (string s in ndsStrs)
                    {
                        if (s == "" || s == null)
                            continue;
                        inStr += "'" + s + "',";
                    }
                    inStr = inStr.Substring(0, inStr.Length - 1);
                    if (inStr.Contains(",") == true)
                        qo.AddWhere(GenerWorkerListAttr.FK_Node, int.Parse(inStr));
                    else
                        qo.AddWhereIn(GenerWorkerListAttr.FK_Node, "(" + inStr + ")");

                    qo.DoQuery();
                    foreach (GenerWorkerList wl in wls)
                    {
                        Node subNd = new Node(wl.FK_Node);
                        Work subWK = subNd.GetWork(wl.WorkID);
                        subWK.Delete();

                        //删除分流下步骤的节点信息.
                        DeleteToNodesData(subNd.HisToNodes);
                    }

                    DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FID=" + dbStr + "WorkID",
                        "WorkID", this.WorkID);
                    DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE FID=" + dbStr + "WorkID",
                        "WorkID", this.WorkID);
                }
                DeleteToNodesData(nd.HisToNodes);
            }
        }

        #region 根据工作岗位生成工作者
        private Node _ndFrom = null;
        private Node ndFrom
        {
            get
            {
                if (_ndFrom == null)
                    _ndFrom = this.HisNode;
                return _ndFrom;
            }
            set
            {
                _ndFrom = value;
            }
        }
        /// <summary>
        /// 初始化工作人员
        /// </summary>
        /// <param name="town">到达的wn</param>
        /// <param name="dt">数据源</param>
        /// <param name="fid">FID</param>
        /// <returns>GenerWorkerLists</returns>
        private GenerWorkerLists InitWorkerLists(WorkNode town, DataTable dt, Int64 fid = 0)
        {
            if (dt.Rows.Count == 0)
                throw new Exception("接受人员列表为空"); // 接受人员列表为空

            this.HisGenerWorkFlow.TodoEmpsNum = -1;

            #region 判断发送的类型，处理相关的FID.
            // 定义下一个节点的接受人的 FID 与 WorkID.
            Int64 nextUsersWorkID = this.WorkID;
            Int64 nextUsersFID = this.HisWork.FID;

            // 是否是分流到子线程。
            bool isFenLiuToSubThread = false;
            if (this.HisNode.IsFLHL == true
                && town.HisNode.HisRunModel == RunModel.SubThread)
            {
                isFenLiuToSubThread = true;
                nextUsersWorkID = 0;
                nextUsersFID = this.HisWork.OID;
            }


            // 子线程 到 合流点or 分合流点.
            bool isSubThreadToFenLiu = false;
            if (this.HisNode.HisRunModel == RunModel.SubThread
                && town.HisNode.IsFLHL == true)
            {
                nextUsersWorkID = this.HisWork.FID;
                nextUsersFID = 0;
                isSubThreadToFenLiu = true;
            }

            // 子线程到子线程.
            bool isSubthread2Subthread = false;
            if (this.HisNode.HisRunModel == RunModel.SubThread && town.HisNode.HisRunModel == RunModel.SubThread)
            {
                nextUsersWorkID = this.HisWork.OID;
                nextUsersFID = this.HisWork.FID;
                isSubthread2Subthread = true;
            }
            #endregion 判断发送的类型，处理相关的FID.

            int toNodeId = town.HisNode.NodeID;
            this.HisWorkerLists = new GenerWorkerLists();
            this.HisWorkerLists.Clear();

            #region 限期时间  town.HisNode.TSpan-1
            // 2008-01-22 之前的东西。
            //int i = town.HisNode.TSpan;
            //dtOfShould = DataType.AddDays(dtOfShould, i);
            //if (town.HisNode.WarningHour > 0)
            //    dtOfWarning = DataType.AddDays(dtOfWarning, i - town.HisNode.WarningHour);
            // edit at 2008-01-22 , 处理预警日期的问题。

            DateTime dtOfShould = DateTime.Now;
            if (this.HisFlow.HisTimelineRole == BP.WF.Template.TimelineRole.ByFlow)
            {
                /*如果整体流程是按流程设置计算。*/
                dtOfShould = DataType.ParseSysDateTime2DateTime(this.HisGenerWorkFlow.SDTOfFlow);
            }

            //如果按照节点设置.
            if (this.HisFlow.HisTimelineRole == BP.WF.Template.TimelineRole.ByNodeSet)
            {
                if (town.HisNode.HisCHWay == CHWay.ByTime)
                {
                    int day = 0;
                    int hh = 0;
                    //增加天数. 考虑到了节假日.                
                    dtOfShould = Glo.AddDayHoursSpan(DateTime.Now, this.town.HisNode.TimeLimit,
                        this.town.HisNode.TimeLimitHH, this.town.HisNode.TimeLimitMM, this.town.HisNode.TWay);
                }
            }


            //求警告日期.
            DateTime dtOfWarning = DateTime.Now;
            if (this.town.HisNode.WarningDay == 0)
            {
                dtOfWarning = dtOfShould;
            }
            else
            {
                //计算警告日期.
                //增加小时数. 考虑到了节假日.
                dtOfWarning = Glo.AddDayHoursSpan(DateTime.Now, this.town.HisNode.WarningDay, 0, 0, this.town.HisNode.TWay);
            }

            switch (this.HisNode.HisNodeWorkType)
            {
                case NodeWorkType.StartWorkFL:
                case NodeWorkType.WorkFHL:
                case NodeWorkType.WorkFL:
                case NodeWorkType.WorkHL:
                    break;
                default:
                    this.HisGenerWorkFlow.FK_Node = town.HisNode.NodeID;
                    this.HisGenerWorkFlow.SDTOfNode = dtOfShould.ToString(DataType.SysDataTimeFormat);
                    this.HisGenerWorkFlow.TodoEmpsNum = dt.Rows.Count;
                    break;
            }
            #endregion 限期时间  town.HisNode.TSpan-1

            #region 处理 人员列表 数据源。
            // 定义是否有分组mark. 如果有三列，就说明该集合中有分组 mark. 就是要处理一个人多个子线程的情况.
            if (dt.Columns.Count == 3 && town.HisNode.HisFormType == NodeFormType.SheetAutoTree)
                this.IsHaveSubThreadGroupMark = false;
            else
                this.IsHaveSubThreadGroupMark = true;

            //如果有4个列并且下一个节点是动态表单树节点.No,Name,BatchNo,FrmIDs 这样的四个列，就是子线程分组.
            if (dt.Columns.Count == 4 && town.HisNode.HisFormType == NodeFormType.SheetAutoTree)
                this.IsHaveSubThreadGroupMark = true;

            if (dt.Columns.Count <= 2 && town.HisNode.HisFormType == NodeFormType.SheetAutoTree)
                throw new Exception("@组织的数据源不正确，到达的节点" + town.HisNode.Name + ",表单类型是动态表单树，至少返回3列来标识表单ID.");

            if (dt.Columns.Count <= 2)
                this.IsHaveSubThreadGroupMark = false;

            if (dt.Rows.Count == 1)
            {
                /* 如果只有一个人员 */
                GenerWorkerList wl = new GenerWorkerList();
                if (isFenLiuToSubThread)
                {
                    /*  说明这是分流点向下发送
                     *  在这里产生临时的workid.
                     */
                    wl.WorkID = DBAccess.GenerOIDByGUID();
                }
                else
                {
                    wl.WorkID = nextUsersWorkID;
                }

                wl.FID = nextUsersFID;
                wl.FK_Node = toNodeId;
                wl.FK_NodeText = town.HisNode.Name;

                wl.FK_Emp = dt.Rows[0][0].ToString();

                Emp emp = new Emp();
                emp.No = wl.FK_Emp;
                if (emp.RetrieveFromDBSources() == 0)
                    throw new Exception("@接收人规则设置错误，求出来的接收人[" + wl.FK_Emp + "]不存在或者被停用。");

                wl.FK_EmpText = emp.Name;
                wl.FK_Dept = emp.FK_Dept;
                wl.FK_DeptT = emp.FK_DeptText;

                //应完成日期.
                if (town.HisNode.HisCHWay == CHWay.None)
                {
                    wl.SDT = "无";
                }
                else
                {
                    wl.SDT = dtOfShould.ToString(DataType.SysDataTimeFormat);
                }


                //警告日期.
                wl.DTOfWarning = dtOfWarning.ToString(DataType.SysDataTimeFormat);

                wl.FK_Flow = town.HisNode.FK_Flow;
                //  wl.Sender = this.Execer;

                // and 2015-01-14 , 如果有三列，就约定为最后一列是分组标志， 有标志就把它放入标志里 .
                if (this.IsHaveSubThreadGroupMark == true)
                {
                    wl.GroupMark = dt.Rows[0][2].ToString(); //第3个列是分组标记.
                    if (DataType.IsNullOrEmpty(wl.GroupMark))
                        throw new Exception("@｛" + wl.FK_Emp + "｝分组标记中没有值,会导致无法按照分组标记去生成子线程,请检查配置的信息是否正确.");
                }

                if (this.IsHaveSubThreadGroupMark == true && town.HisNode.HisFormType == NodeFormType.SheetAutoTree)
                {
                    /*是分组标记，并且是自动表单树.*/
                    wl.GroupMark = dt.Rows[0][2].ToString(); //第3个列是分组标记.
                    wl.FrmIDs = dt.Rows[0][3].ToString(); //第4个列是可以执行的表单IDs.
                }

                if (dt.Columns.Count == 3 && town.HisNode.HisFormType == NodeFormType.SheetAutoTree)
                {
                    /*是自动表单树,只有3个列，说明最后一列就是表单IDs.*/
                    wl.FrmIDs = dt.Rows[0][2].ToString(); //第3个列是可以执行的表单IDs.
                }

                //设置发送人.
                if (BP.Web.WebUser.No == "Guest")
                {
                    wl.Sender = GuestUser.No + "," + GuestUser.Name;
                    wl.GuestNo = GuestUser.No;
                    wl.GuestName = GuestUser.Name;
                }
                else
                {
                    wl.Sender = WebUser.No + "," + WebUser.Name;
                }

                //判断下一个节点是否是外部用户处理人节点？
                if (town.HisNode.IsGuestNode)
                {
                    if (this.HisGenerWorkFlow.GuestNo != "")
                    {
                        wl.GuestNo = this.HisGenerWorkFlow.GuestNo;
                        wl.GuestName = this.HisGenerWorkFlow.GuestName;
                    }
                    else
                    {
                        /*这种情况是，不是外部用户发起的流程。*/
                        if (town.HisNode.HisDeliveryWay == DeliveryWay.BySQL)
                        {
                            string mysql = town.HisNode.DeliveryParas.Clone() as string;
                            DataTable mydt = BP.DA.DBAccess.RunSQLReturnTable(Glo.DealExp(mysql, this.rptGe, null));

                            wl.GuestNo = mydt.Rows[0][0].ToString();
                            wl.GuestName = mydt.Rows[0][1].ToString();

                            this.HisGenerWorkFlow.GuestNo = wl.GuestNo;
                            this.HisGenerWorkFlow.GuestName = wl.GuestName;
                        }
                        else if (town.HisNode.HisDeliveryWay == DeliveryWay.ByPreviousNodeFormEmpsField)
                        {
                            wl.GuestNo = this.HisWork.GetValStrByKey(town.HisNode.DeliveryParas);
                            wl.GuestName = "外部用户";
                            this.HisGenerWorkFlow.GuestNo = wl.GuestNo;
                            this.HisGenerWorkFlow.GuestName = wl.GuestName;
                        }
                        else
                        {
                            throw new Exception("@当前节点[" + this.town.HisNode.Name + "]是中间节点，并且是外部用户处理节点，您需要正确的设置，这个外部用户接受人规则。");
                        }
                    }

                    //wl.FK_Emp = wl.GuestNo;
                    //wl.GuestName = wl.GuestName;
                }

                wl.Insert();
                this.HisWorkerLists.AddEntity(wl);

                RememberMe rm = new RememberMe(); // this.GetHisRememberMe(town.HisNode);
                rm.Objs = "@" + wl.FK_Emp + "@";
                rm.ObjsExt += BP.WF.Glo.DealUserInfoShowModel(wl.FK_Emp, wl.FK_EmpText);
                rm.Emps = "@" + wl.FK_Emp + "@";
                rm.EmpsExt = BP.WF.Glo.DealUserInfoShowModel(wl.FK_Emp, wl.FK_EmpText);
                this.HisRememberMe = rm;
            }
            else
            {
                /* 如果有多个人员，就要考虑接受人是否记忆属性的问题。 */
                RememberMe rm = this.GetHisRememberMe(town.HisNode);

                #region 是否需要清空记忆属性.
                // 如果按照选择的人员处理，就设置它的记忆为空。2011-11-06 处理电厂需求 .
                if (this.town.HisNode.HisDeliveryWay == DeliveryWay.BySelected
                    || this.town.HisNode.IsAllowRepeatEmps == true  /*允许接受人员重复*/
                    || town.HisNode.IsRememberMe == false)
                {
                    if (rm != null)
                        rm.Objs = "";
                }

                if (this.HisNode.IsFL)
                {
                    if (rm != null)
                        rm.Objs = "";
                }

                if (this.IsHaveSubThreadGroupMark == false && rm != null && rm.Objs != "")
                {
                    /*检查接受人列表是否发生了变化,如果变化了，就要把有效的接受人清空，让其重新生成.*/
                    string emps = "@";
                    foreach (DataRow dr in dt.Rows)
                        emps += dr[0].ToString() + "@";

                    if (rm.Emps != emps)
                    {
                        // 列表发生了变化.
                        rm.Emps = emps;
                        rm.Objs = ""; //清空有效的接受人集合.
                    }
                }
                #endregion 是否需要清空记忆属性.

                string myemps = "";
                Emp emp = new Emp();
                foreach (DataRow dr in dt.Rows)
                {
                    string fk_emp = dr[0].ToString();
                    if (this.IsHaveSubThreadGroupMark == true)
                    {
                        /*如果有分组 Mark ,就不处理重复人员的问题.*/
                    }
                    else
                    {
                        // 处理人员重复的，不然会导致generworkerlist的pk错误。
                        if (myemps.IndexOf("@" + dr[0].ToString() + ",") != -1)
                            continue;
                        myemps += "@" + dr[0].ToString() + ",";
                    }

                    GenerWorkerList wl = new GenerWorkerList();

                    #region 根据记忆是否设置该操作员可用与否。
                    if (rm != null)
                    {
                        if (rm.Objs == "")
                        {
                            /*如果是空的.*/
                            wl.IsEnable = true;
                        }
                        else
                        {
                            if (rm.Objs.Contains("@" + fk_emp + "@") == true)
                                wl.IsEnable = true; /* 如果包含，就说明他已经有了*/
                            else
                                wl.IsEnable = false;
                        }
                    }
                    else
                    {
                        wl.IsEnable = false;
                    }
                    #endregion 根据记忆是否设置该操作员可用与否。

                    wl.FK_Node = toNodeId;
                    wl.FK_NodeText = town.HisNode.Name;
                    wl.FK_Emp = fk_emp;

                    emp.No = wl.FK_Emp;
                    if (emp.RetrieveFromDBSources() == 0)
                        continue;

                    wl.FK_EmpText = emp.Name;
                    wl.FK_Dept = emp.FK_Dept;
                    //wl.WarningHour = town.HisNode.WarningHour;
                    if (town.HisNode.HisCHWay == CHWay.None)
                        wl.SDT = "无";
                    else
                        wl.SDT = dtOfShould.ToString(DataType.SysDataTimeFormat);

                    wl.DTOfWarning = dtOfWarning.ToString(DataType.SysDataTimeFormat);

                    wl.FK_Flow = town.HisNode.FK_Flow;
                    if (this.IsHaveSubThreadGroupMark == true)
                    {
                        //设置分组信息.
                        object val = dr[2];
                        if (val == null)
                            throw new Exception("@分组标志不能为Null.");

                        if (DataType.IsNullOrEmpty(val.ToString()) == null)
                            throw new Exception("@分组标志不能为Null.");

                        wl.GroupMark = val.ToString();
                        if (dt.Columns.Count == 4 && this.town.HisNode.HisFormType == NodeFormType.SheetAutoTree)
                        {
                            wl.FrmIDs = dr[3].ToString();
                            if (DataType.IsNullOrEmpty(dr[3].ToString()))
                                throw new Exception("@组织的接受人数据源不正确,表单IDs不能为空.");
                        }
                    }
                    else
                    {
                        if (dt.Columns.Count == 3 && this.town.HisNode.HisFormType == NodeFormType.SheetAutoTree)
                        {
                            /*如果只有三列, 并且是动态表单树.*/
                            wl.FrmIDs = dr[2].ToString(); //
                            if (DataType.IsNullOrEmpty(dr[2].ToString()))
                                throw new Exception("@组织的接受人数据源不正确,表单IDs不能为空.");
                        }
                    }

                    wl.FID = nextUsersFID;
                    if (isFenLiuToSubThread)
                    {
                        /* 说明这是分流点向下发送
                         *  在这里产生临时的workid.
                         */
                        wl.WorkID = DBAccess.GenerOIDByGUID();
                    }
                    else
                    {
                        wl.WorkID = nextUsersWorkID;
                    }

                    try
                    {
                        wl.Insert();
                        this.HisWorkerLists.AddEntity(wl);
                    }
                    catch (Exception ex)
                    {
                        Log.DefaultLogWriteLineError("不应该出现的异常信息：" + ex.Message);
                    }
                }

                //执行对rm的更新。
                if (rm != null)
                {
                    string empExts = "";
                    string objs = "@"; // 有效的工作人员.
                    string objsExt = "@"; // 有效的工作人员.
                    foreach (GenerWorkerList wl in this.HisWorkerLists)
                    {
                        if (wl.IsEnable == false)
                            empExts += "<strike><font color=red>" + BP.WF.Glo.DealUserInfoShowModel(wl.FK_Emp, wl.FK_EmpText) + "</font></strike>、";
                        else
                            empExts += BP.WF.Glo.DealUserInfoShowModel(wl.FK_Emp, wl.FK_EmpText);

                        if (wl.IsEnable == true)
                        {
                            objs += wl.FK_Emp + "@";
                            objsExt += BP.WF.Glo.DealUserInfoShowModel(wl.FK_Emp, wl.FK_EmpText);
                        }
                    }
                    rm.EmpsExt = empExts;

                    rm.Objs = objs;
                    rm.ObjsExt = objsExt;
                    //  rm.Save(); //保存.
                    this.HisRememberMe = rm;
                }
            }

            if (this.HisWorkerLists.Count == 0)
                throw new Exception("@根据[" + this.town.HisNode.HisRunModel + "]部门产生工作人员出现错误，流程[" + this.HisWorkFlow.HisFlow.Name + "],中节点[" + town.HisNode.Name + "]定义错误,没有找到接受此工作的工作人员.");
            #endregion 处理 人员列表 数据源。

            #region 设置流程数量,其他的信息为任务池提供数据。
            string hisEmps = "";
            int num = 0;
            foreach (GenerWorkerList wl in this.HisWorkerLists)
            {
                if (wl.IsPassInt == 0 && wl.IsEnable == true)
                {
                    num++;
                    hisEmps += wl.FK_Emp + "," + wl.FK_EmpText + ";";
                }
            }

            if (num == 0)
                throw new Exception("@不应该产生的结果错误.");

            this.HisGenerWorkFlow.TodoEmpsNum = num;
            this.HisGenerWorkFlow.TodoEmps = hisEmps;
            #endregion

            #region  求出日志类型，并加入变量中。
            ActionType at = ActionType.Forward;
            switch (town.HisNode.HisNodeWorkType)
            {
                case NodeWorkType.StartWork:
                case NodeWorkType.StartWorkFL:
                    at = ActionType.Start;
                    break;
                case NodeWorkType.Work:
                    if (this.HisNode.HisNodeWorkType == NodeWorkType.WorkFL
                        || this.HisNode.HisNodeWorkType == NodeWorkType.WorkFHL)
                        at = ActionType.ForwardFL;
                    else
                        at = ActionType.Forward;
                    break;
                case NodeWorkType.WorkHL:
                    at = ActionType.ForwardHL;
                    break;
                case NodeWorkType.SubThreadWork:
                    at = ActionType.SubThreadForward;
                    break;
                default:
                    break;
            }
            #endregion  求出日志类型，并加入变量中。

            //把工作的当时信息存入数据库.
            // string json = this.HisWork.ToJson();

            #region 如果是子线城前进.
            if (at == ActionType.SubThreadForward)
            {
                string emps = "";
                foreach (GenerWorkerList wl in this.HisWorkerLists)
                {
                    this.AddToTrack(at, wl, "子线程", this.town.HisWork.OID);
                }
                //写入到日志.
            }
            #endregion 如果是子线城前进.

            #region 如果是非子线城前进.
            if (at != ActionType.SubThreadForward)
            {
                if (this.HisWorkerLists.Count == 1)
                {
                    GenerWorkerList wl = this.HisWorkerLists[0] as GenerWorkerList;
                    this.AddToTrack(at, wl.FK_Emp, wl.FK_EmpText, wl.FK_Node, wl.FK_NodeText, null, this.ndFrom, null, null);
                }
                else
                {
                    string info = "共(" + this.HisWorkerLists.Count + ")人接收\t\n";

                    string emps = "";
                    foreach (GenerWorkerList wl in this.HisWorkerLists)
                    {
                        info += BP.WF.Glo.DealUserInfoShowModel(wl.FK_DeptT, wl.FK_EmpText) + "\t\n";

                        emps += wl.FK_Emp + "," + wl.FK_EmpText + ";";
                    }

                    //写入到日志.
                    this.AddToTrack(at, this.Execer, "多人接受(见信息栏)", town.HisNode.NodeID, town.HisNode.Name, info, this.ndFrom, null, emps);
                }
            }
            #endregion 如果是非子线城前进.

            #region 把数据加入变量中.
            string ids = "";
            string names = "";
            string idNames = "";
            if (this.HisWorkerLists.Count == 1)
            {
                GenerWorkerList gwl = (GenerWorkerList)this.HisWorkerLists[0];
                ids = gwl.FK_Emp;
                names = gwl.FK_EmpText;

                //设置状态。
                this.HisGenerWorkFlow.TaskSta = TaskSta.None;
            }
            else
            {
                foreach (GenerWorkerList gwl in this.HisWorkerLists)
                {
                    ids += gwl.FK_Emp + ",";
                    names += gwl.FK_EmpText + ",";
                }

                //设置状态, 如果该流程使用了启用共享任务池。
                if (town.HisNode.IsEnableTaskPool && this.HisNode.HisRunModel == RunModel.Ordinary)
                    this.HisGenerWorkFlow.TaskSta = TaskSta.Sharing;
                else
                    this.HisGenerWorkFlow.TaskSta = TaskSta.None;
            }

            this.addMsg(SendReturnMsgFlag.VarAcceptersID, ids, ids, SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarAcceptersName, names, names, SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarAcceptersNID, idNames, idNames, SendReturnMsgType.SystemMsg);
            #endregion

            return this.HisWorkerLists;
        }
        #endregion


        #region 条件
        private Conds _HisNodeCompleteConditions = null;
        /// <summary>
        /// 节点完成任务的条件
        /// 条件与条件之间是or 的关系, 就是说,如果任何一个条件满足,这个工作人员在这个节点上的任务就完成了.
        /// </summary>
        public Conds HisNodeCompleteConditions
        {
            get
            {
                if (this._HisNodeCompleteConditions == null)
                {
                    _HisNodeCompleteConditions = new Conds(CondType.Node, this.HisNode.NodeID, this.WorkID, this.rptGe);

                    return _HisNodeCompleteConditions;
                }
                return _HisNodeCompleteConditions;
            }
        }
        private Conds _HisFlowCompleteConditions = null;
        /// <summary>
        /// 他的完成任务的条件,此节点是完成任务的条件集合
        /// 条件与条件之间是or 的关系, 就是说,如果任何一个条件满足,这个任务就完成了.
        /// </summary>
        public Conds HisFlowCompleteConditions
        {
            get
            {
                if (this._HisFlowCompleteConditions == null)
                {
                    _HisFlowCompleteConditions = new Conds(CondType.Flow, this.HisNode.NodeID, this.WorkID, this.rptGe);
                }
                return _HisFlowCompleteConditions;
            }
        }
        #endregion

        #region 关于质量考核
        ///// <summary>
        ///// 得到以前的已经完成的工作节点.
        ///// </summary>
        ///// <returns></returns>
        //public WorkNodes GetHadCompleteWorkNodes()
        //{
        //    WorkNodes mywns = new WorkNodes();
        //    WorkNodes wns = new WorkNodes(this.HisNode.HisFlow, this.HisWork.OID);
        //    foreach (WorkNode wn in wns)
        //    {
        //        if (wn.IsComplete)
        //            mywns.Add(wn);
        //    }
        //    return mywns;
        //}
        #endregion

        #region 流程公共方法
        private Flow _HisFlow = null;
        public Flow HisFlow
        {
            get
            {
                if (_HisFlow == null)
                    _HisFlow = this.HisNode.HisFlow;
                return _HisFlow;
            }
        }
        private Node JumpToNode = null;
        private string JumpToEmp = null;


        #region NodeSend 的附属功能.
        /// <summary>
        /// 获得下一个节点.
        /// </summary>
        /// <returns></returns>
        public Node NodeSend_GenerNextStepNode()
        {
            //如果要是跳转到的节点，自动跳转规则规则就会失效。
            if (this.JumpToNode != null)
                return this.JumpToNode;
            // 被zhoupeng注释，因为有可能遇到跳转.
            //if (this.HisNode.HisToNodes.Count == 1)
            //    return (Node)this.HisNode.HisToNodes[0];

            // 判断是否有用户选择的节点.
            if (this.HisNode.CondModel == CondModel.ByUserSelected)
            {
                // 获取用户选择的节点.
                string nodes = this.HisGenerWorkFlow.Paras_ToNodes;
                if (DataType.IsNullOrEmpty(nodes))
                    throw new Exception("@用户没有选择发送到的节点.");

                string[] mynodes = nodes.Split(',');
                foreach (string item in mynodes)
                {
                    if (DataType.IsNullOrEmpty(item))
                        continue;

                    //排除到达自身节点.
                    if (this.HisNode.NodeID.ToString() == item)
                        continue;

                    return new Node(int.Parse(item));
                }

                //设置他为空,以防止下一次发送出现错误.
                this.HisGenerWorkFlow.Paras_ToNodes = "";
            }

            Node nd = NodeSend_GenerNextStepNode_Ext1();
            return nd;
        }
        /// <summary>
        /// 知否执行了跳转.
        /// </summary>
        public bool IsSkip = false;
        /// <summary>
        /// 获取下一步骤的工作节点.
        /// </summary>
        /// <returns></returns>
        public Node NodeSend_GenerNextStepNode_Ext1()
        {
            //如果要是跳转到的节点，自动跳转规则规则就会失效。
            if (this.JumpToNode != null)
                return this.JumpToNode;

            Node mynd = this.HisNode;
            Work mywork = this.HisWork;
            var beforeSkipNodeID = 0;   //added by liuxc,2015-7-13,标识自动跳转之前的节点ID


            #region (最后)判断是否有延续流程.
            NodeYGFlows ygflows = new NodeYGFlows();
            ygflows.Retrieve(NodeYGFlowAttr.FK_Node, this.HisNode.NodeID);
            if (ygflows.Count != 0 && 1 == 2)
            {
                foreach (NodeYGFlow item in ygflows)
                {
                    bool isPass = false;

                    if (item.ExpType == ConnDataFrom.Paras)
                        isPass = BP.WF.Glo.CondExpPara(item.CondExp, this.rptGe.Row);

                    if (item.ExpType == ConnDataFrom.SQL)
                        isPass = BP.WF.Glo.CondExpSQL(item.CondExp, this.rptGe.Row);

                    if (isPass == true)
                        return new Node(int.Parse(item.FK_Flow + "01"));
                }
            }
            #endregion (最后)判断是否有延续流程.


            #region 计算到达的节点.
            this.ndFrom = this.HisNode;
            while (true)
            {
                //上一步的工作节点.
                int prvNodeID = mynd.NodeID;
                if (mynd.IsEndNode)
                {
                    /*如果是最后一个节点了,仍然找不到下一步节点...*/
                    if (this.HisGenerWorkFlow.TransferCustomType == TransferCustomType.ByCCBPMDefine)
                        this.IsStopFlow = true;
                    return mynd;
                }

                // 获取它的下一步节点.
                Node nd = this.NodeSend_GenerNextStepNode_Ext(mynd);
                nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.

                mynd = nd;
                Work skipWork = null;
                if (mywork.NodeFrmID != nd.NodeFrmID)
                {
                    /* 跳过去的节点也要写入数据，不然会造成签名错误。*/
                    skipWork = nd.HisWork;

                    if (skipWork.EnMap.PhysicsTable != this.rptGe.EnMap.PhysicsTable)
                    {
                        skipWork.Copy(this.rptGe);
                        skipWork.Copy(mywork);

                        skipWork.OID = this.WorkID;
                        if (nd.HisRunModel == RunModel.SubThread)
                            skipWork.FID = mywork.FID;

                        skipWork.Rec = this.Execer;
                        skipWork.SetValByKey(WorkAttr.RDT, DataType.CurrentDataTimess);
                        skipWork.SetValByKey(WorkAttr.CDT, DataType.CurrentDataTimess);
                        skipWork.ResetDefaultVal();

                        // 把里面的默认值也copy报表里面去.
                        rptGe.Copy(skipWork);

                        //如果存在就修改
                        if (skipWork.IsExit(skipWork.PK, this.WorkID) == true)
                        {//@袁丽娜
                            int count = skipWork.RetrieveFromDBSources();
                            if (count == 1)
                                skipWork.DirectUpdate();
                            else
                                skipWork.DirectInsert();
                        }
                        else
                            skipWork.InsertAsOID(this.WorkID);
                    }

                    #region  初始化发起的工作节点。

                    if (1 == 2)
                    {

#warning 被 zhoupeng 删除 2014-06-20, 不应该存在这里.
                        if (this.HisWork.EnMap.PhysicsTable == nd.HisWork.EnMap.PhysicsTable)
                        {
                            /*这是数据合并模式, 就不执行 copy */
                        }
                        else
                        {

                            /* 如果两个数据源不想等，就执行copy。 */

                            #region 复制附件。
                            FrmAttachments athDesc = this.HisNode.MapData.FrmAttachments;
                            if (athDesc.Count > 0)
                            {
                                FrmAttachmentDBs athDBs = new FrmAttachmentDBs("ND" + this.HisNode.NodeID,
                                      this.WorkID.ToString());
                                int idx = 0;
                                if (athDBs.Count > 0)
                                {
                                    athDBs.Delete(FrmAttachmentDBAttr.FK_MapData, "ND" + nd.NodeID,
                                        FrmAttachmentDBAttr.RefPKVal, this.WorkID);

                                    /*说明当前节点有附件数据*/
                                    foreach (FrmAttachmentDB athDB in athDBs)
                                    {
                                        idx++;
                                        FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                                        athDB_N.Copy(athDB);
                                        athDB_N.FK_MapData = "ND" + nd.NodeID;
                                        athDB_N.RefPKVal = this.WorkID.ToString();

                                        // athDB_N.MyPK = this.WorkID + "_" + idx + "_" + athDB_N.FK_MapData;
                                        // if (athDB.dbt
                                        // athDB_N.MyPK = athDB_N.FK_FrmAttachment.Replace("ND" + this.HisNode.NodeID, "ND" + nd.NodeID) + "_" + this.WorkID;

                                        athDB_N.MyPK = DBAccess.GenerGUID(); // athDB_N.FK_FrmAttachment.Replace("ND" + this.HisNode.NodeID, "ND" + nd.NodeID) + "_" + this.WorkID;

                                        athDB_N.FK_FrmAttachment = athDB_N.FK_FrmAttachment.Replace("ND" + this.HisNode.NodeID,
                                           "ND" + nd.NodeID);

                                        athDB_N.Save();
                                    }
                                }
                            }
                            #endregion 复制附件。

                            #region 复制图片上传附件。
                            if (this.HisNode.MapData.FrmImgAths.Count > 0)
                            {
                                FrmImgAthDBs athDBs = new FrmImgAthDBs("ND" + this.HisNode.NodeID,
                                      this.WorkID.ToString());
                                int idx = 0;
                                if (athDBs.Count > 0)
                                {
                                    athDBs.Delete(FrmAttachmentDBAttr.FK_MapData, "ND" + nd.NodeID,
                                        FrmAttachmentDBAttr.RefPKVal, this.WorkID);

                                    /*说明当前节点有附件数据*/
                                    foreach (FrmImgAthDB athDB in athDBs)
                                    {
                                        idx++;
                                        FrmImgAthDB athDB_N = new FrmImgAthDB();
                                        athDB_N.Copy(athDB);
                                        athDB_N.FK_MapData = "ND" + nd.NodeID;
                                        athDB_N.RefPKVal = this.WorkID.ToString();
                                        athDB_N.MyPK = this.WorkID + "_" + idx + "_" + athDB_N.FK_MapData;
                                        athDB_N.FK_FrmImgAth = athDB_N.FK_FrmImgAth.Replace("ND" + this.HisNode.NodeID, "ND" + nd.NodeID);
                                        athDB_N.Save();
                                    }
                                }
                            }
                            #endregion 复制图片上传附件。

                            #region 复制Ele
                            if (this.HisNode.MapData.FrmEles.Count > 0)
                            {
                                FrmEleDBs eleDBs = new FrmEleDBs("ND" + this.HisNode.NodeID,
                                      this.WorkID.ToString());
                                if (eleDBs.Count > 0)
                                {
                                    eleDBs.Delete(FrmEleDBAttr.FK_MapData, "ND" + nd.NodeID,
                                        FrmEleDBAttr.RefPKVal, this.WorkID);

                                    /*说明当前节点有附件数据*/
                                    foreach (FrmEleDB eleDB in eleDBs)
                                    {
                                        FrmEleDB eleDB_N = new FrmEleDB();
                                        eleDB_N.Copy(eleDB);
                                        eleDB_N.FK_MapData = "ND" + nd.NodeID;
                                        eleDB_N.GenerPKVal();
                                        eleDB_N.Save();
                                    }
                                }
                            }
                            #endregion 复制Ele

                            #region 复制明细数据
                            // int deBugDtlCount=
                            Sys.MapDtls dtls = this.HisNode.MapData.MapDtls;
                            string recDtlLog = "@记录测试明细表Copy过程,从节点ID:" + this.HisNode.NodeID + " WorkID:" + this.WorkID + ", 到节点ID=" + nd.NodeID;
                            if (dtls.Count > 0)
                            {
                                Sys.MapDtls toDtls = nd.MapData.MapDtls;
                                recDtlLog += "@到节点明细表数量是:" + dtls.Count + "个";

                                Sys.MapDtls startDtls = null;
                                bool isEnablePass = false; /*是否有明细表的审批.*/
                                foreach (MapDtl dtl in dtls)
                                {
                                    if (dtl.IsEnablePass)
                                        isEnablePass = true;
                                }

                                if (isEnablePass) /* 如果有就建立它开始节点表数据 */
                                    startDtls = new BP.Sys.MapDtls("ND" + int.Parse(nd.FK_Flow) + "01");

                                recDtlLog += "@进入循环开始执行逐个明细表copy.";
                                int i = -1;
                                foreach (Sys.MapDtl dtl in dtls)
                                {
                                    if (dtl.IsCopyNDData == false)
                                        continue;

                                    recDtlLog += "@进入循环开始执行明细表(" + dtl.No + ")copy.";

                                    i++;
                                    //if (toDtls.Count <= i)
                                    //    continue;
                                    Sys.MapDtl toDtl = null;
                                    foreach (MapDtl todtl in toDtls)
                                    {
                                        if (todtl.Name.Substring(6, todtl.Name.Length - 6).Equals(dtl.Name.Substring(6, dtl.Name.Length - 6)))
                                        {
                                            if (todtl.PTable == dtl.PTable)
                                                break;

                                            toDtl = todtl;
                                            break;
                                        }
                                    }

                                    if (toDtl == null)
                                        continue;

                                    if (dtl.IsEnablePass == true)
                                    {
                                        /*如果启用了是否明细表的审核通过机制,就允许copy节点数据。*/
                                        toDtl.IsCopyNDData = true;
                                    }

                                    if (toDtl.IsCopyNDData == false)
                                        continue;

                                    //获取明细数据。
                                    GEDtls gedtls = new GEDtls(dtl.No);
                                    QueryObject qo = null;
                                    qo = new QueryObject(gedtls);
                                    switch (dtl.DtlOpenType)
                                    {
                                        case DtlOpenType.ForEmp:
                                            qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                                            break;
                                        case DtlOpenType.ForWorkID:
                                            qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                                            break;
                                        case DtlOpenType.ForFID:
                                            qo.AddWhere(GEDtlAttr.FID, this.WorkID);
                                            break;
                                    }
                                    qo.DoQuery();

                                    recDtlLog += "@查询出来从明细表:" + dtl.No + ",明细数据:" + gedtls.Count + "条.";

                                    int unPass = 0;
                                    // 是否启用审核机制。
                                    isEnablePass = dtl.IsEnablePass;
                                    if (isEnablePass && this.HisNode.IsStartNode == false)
                                        isEnablePass = true;
                                    else
                                        isEnablePass = false;

                                    if (isEnablePass == true)
                                    {
                                        /*判断当前节点该明细表上是否有，isPass 审核字段，如果没有抛出异常信息。*/
                                        if (gedtls.Count != 0)
                                        {
                                            GEDtl dtl1 = gedtls[0] as GEDtl;
                                            if (dtl1.EnMap.Attrs.Contains("IsPass") == false)
                                                isEnablePass = false;
                                        }
                                    }

                                    recDtlLog += "@删除到达明细表:" + dtl.No + ",数据, 并开始遍历明细表,执行一行行的copy.";
                                    DBAccess.RunSQL("DELETE FROM " + toDtl.PTable + " WHERE RefPK=" + dbStr + "RefPK", "RefPK", this.WorkID.ToString());

                                    // copy数量.
                                    int deBugNumCopy = 0;
                                    foreach (GEDtl gedtl in gedtls)
                                    {
                                        if (isEnablePass)
                                        {
                                            if (gedtl.GetValBooleanByKey("IsPass") == false)
                                            {
                                                /*没有审核通过的就 continue 它们，仅复制已经审批通过的.*/
                                                continue;
                                            }
                                        }

                                        BP.Sys.GEDtl dtCopy = new GEDtl(toDtl.No);
                                        dtCopy.Copy(gedtl);
                                        dtCopy.FK_MapDtl = toDtl.No;
                                        dtCopy.RefPK = this.WorkID.ToString();
                                        dtCopy.InsertAsOID(dtCopy.OID);
                                        dtCopy.RefPKInt64 = this.WorkID;
                                        deBugNumCopy++;

                                        #region  复制明细表单条 - 附件信息
                                        if (toDtl.IsEnableAthM)
                                        {
                                            /*如果启用了多附件,就复制这条明细数据的附件信息。*/
                                            FrmAttachmentDBs athDBs = new FrmAttachmentDBs(dtl.No, gedtl.OID.ToString());
                                            if (athDBs.Count > 0)
                                            {
                                                i = 0;
                                                foreach (FrmAttachmentDB athDB in athDBs)
                                                {
                                                    i++;
                                                    FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                                                    athDB_N.Copy(athDB);
                                                    athDB_N.FK_MapData = toDtl.No;
                                                    athDB_N.MyPK = toDtl.No + "_" + dtCopy.OID + "_" + i.ToString();
                                                    athDB_N.FK_FrmAttachment = athDB_N.FK_FrmAttachment.Replace("ND" + this.HisNode.NodeID,
                                                        "ND" + nd.NodeID);
                                                    athDB_N.RefPKVal = dtCopy.OID.ToString();
                                                    athDB_N.DirectInsert();
                                                }
                                            }
                                        }
                                        #endregion  复制明细表单条 - 附件信息

                                    }
#warning 记录日志.
                                    if (gedtls.Count != deBugNumCopy)
                                    {
                                        recDtlLog += "@从明细表:" + dtl.No + ",明细数据:" + gedtls.Count + "条.";
                                        //记录日志.
                                        Log.DefaultLogWriteLineInfo(recDtlLog);
                                        throw new Exception("@系统出现错误，请将如下信息反馈给管理员,谢谢。: 技术信息:" + recDtlLog);
                                    }

                                    #region 如果启用了审核机制
                                    if (isEnablePass)
                                    {
                                        /* 如果启用了审核通过机制，就把未审核的数据copy到第一个节点上去 
                                         * 1, 找到对应的明细点.
                                         * 2, 把未审核通过的数据复制到开始明细表里.
                                         */
                                        string fk_mapdata = "ND" + int.Parse(nd.FK_Flow) + "01";
                                        MapData md = new MapData(fk_mapdata);
                                        string startUser = "SELECT Rec FROM " + md.PTable + " WHERE OID=" + this.WorkID;
                                        startUser = DBAccess.RunSQLReturnString(startUser);

                                        MapDtl startDtl = (MapDtl)startDtls[i];
                                        foreach (GEDtl gedtl in gedtls)
                                        {
                                            if (gedtl.GetValBooleanByKey("IsPass"))
                                                continue; /* 排除审核通过的 */

                                            BP.Sys.GEDtl dtCopy = new GEDtl(startDtl.No);
                                            dtCopy.Copy(gedtl);
                                            dtCopy.OID = 0;
                                            dtCopy.FK_MapDtl = startDtl.No;
                                            dtCopy.RefPK = gedtl.OID.ToString(); //this.WorkID.ToString();
                                            dtCopy.SetValByKey("BatchID", this.WorkID);
                                            dtCopy.SetValByKey("IsPass", 0);
                                            dtCopy.SetValByKey("Rec", startUser);
                                            dtCopy.SetValByKey("Checker", this.ExecerName);
                                            dtCopy.RefPKInt64 = this.WorkID;
                                            dtCopy.SaveAsOID(gedtl.OID);
                                        }
                                        DBAccess.RunSQL("UPDATE " + startDtl.PTable + " SET Rec='" + startUser + "',Checker='" + this.Execer + "' WHERE BatchID=" + this.WorkID + " AND Rec='" + this.Execer + "'");
                                    }
                                    #endregion 如果启用了审核机制
                                }
                            }
                            #endregion 复制明细数据
                        }
                    }
                    #endregion 初始化发起的工作节点.

                    IsSkip = true;
                    mywork = skipWork;
                }

                //如果没有设置跳转规则，就返回他们.
                if (nd.AutoJumpRole0 == false && nd.AutoJumpRole1 == false && nd.AutoJumpRole2 == false && nd.HisWhenNoWorker == false)
                    return nd;

                DataTable dt = null;
                FindWorker fw = new FindWorker();
                WorkNode toWn = new WorkNode(this.WorkID, nd.NodeID);
                if (skipWork == null)
                    skipWork = toWn.HisWork;

                dt = fw.DoIt(this.HisFlow, this, toWn); // 找到下一步骤的接受人.
                string Executor = "";//实际执行人
                string ExecutorName = "";//实际执行人名称
                Emp emp = new Emp();
                if (dt == null || dt.Rows.Count == 0)
                {
                    if (nd.HisWhenNoWorker == true)
                    {
                        this.AddToTrack(ActionType.Skip, this.Execer, this.ExecerName,
                            nd.NodeID, nd.Name, "自动跳转.(当没有找到处理人时)", ndFrom);
                        ndFrom = nd;
                        continue;
                    }
                    else
                    {
                        //抛出异常.
                        throw new Exception("@没有找到节点[" + nd.Name + "]的工作处理人.");
                    }
                }

                if (dt.Rows.Count == 0)
                    throw new Exception("@没有找到下一个节点(" + nd.Name + ")的处理人");

                if (nd.AutoJumpRole0)
                {
                    /*处理人就是发起人*/
                    bool isHave = false;
                    foreach (DataRow dr in dt.Rows)
                    {
                        // 如果出现了 处理人就是发起人的情况.
                        if (dr[0].ToString() == this.HisGenerWorkFlow.Starter)
                        {
                            #region 处理签名，让签名的人是发起人。


                            Attrs attrs = skipWork.EnMap.Attrs;
                            bool isUpdate = false;
                            foreach (Attr attr in attrs)
                            {
                                if (attr.UIIsReadonly && attr.UIVisible == true
                                    )
                                {
                                    if (attr.DefaultValOfReal == "@WebUser.No")
                                    {
                                        skipWork.SetValByKey(attr.Key, this.HisGenerWorkFlow.Starter);
                                        isUpdate = true;
                                    }
                                    if (attr.DefaultValOfReal == "@WebUser.Name")
                                    {
                                        skipWork.SetValByKey(attr.Key, this.HisGenerWorkFlow.StarterName);
                                        isUpdate = true;
                                    }
                                    if (attr.DefaultValOfReal == "@WebUser.FK_Dept")
                                    {
                                        skipWork.SetValByKey(attr.Key, this.HisGenerWorkFlow.FK_Dept);
                                        isUpdate = true;
                                    }
                                    if (attr.DefaultValOfReal == "@WebUser.DeptName")
                                    {
                                        skipWork.SetValByKey(attr.Key, this.HisGenerWorkFlow.DeptName);
                                        isUpdate = true;
                                    }
                                }
                            }
                            if (isUpdate)
                                skipWork.DirectUpdate();
                            #endregion 处理签名，让签名的人是发起人。

                            isHave = true;
                            Executor = dr[0].ToString();
                            emp = new Emp(Executor);
                            ExecutorName = emp.Name;
                            break;
                        }
                    }

                    if (isHave == true)
                    {
                        /*如果发现了，当前人员包含处理人集合. */
                        this.AddToTrack(ActionType.Skip, Executor, ExecutorName, nd.NodeID, nd.Name, "自动跳转,(处理人就是提交人)", ndFrom);
                        this.HisFlow.DoFlowEventEntity(EventListOfNode.SendWhen, nd, skipWork, null);
                        ndFrom = nd;
                        this.HisFlow.DoFlowEventEntity(EventListOfNode.SendSuccess, nd, skipWork, null, this.HisMsgObjs);
                        continue;
                    }

                    //如果没有跳转,判断是否,其他两个条件是否设置.
                    if (nd.AutoJumpRole1 == false && nd.AutoJumpRole2 == false)
                        return nd;
                }

                if (nd.AutoJumpRole1)
                {
                    /*处理人已经出现过*/
                    bool isHave = false;
                    foreach (DataRow dr in dt.Rows)
                    {
                        // 如果出现了处理人就是提交人的情况.
                        string sql = "SELECT COUNT(*) FROM WF_GenerWorkerList WHERE FK_Emp='" + dr[0].ToString() + "' AND WorkID=" + this.WorkID;
                        if (DBAccess.RunSQLReturnValInt(sql) == 1)
                        {
                            /*这里不处理签名.*/
                            isHave = true;
                            Executor = dr[0].ToString();
                            emp = new Emp(Executor);
                            ExecutorName = emp.Name;
                            break;
                        }
                    }

                    if (isHave == true)
                    {
                        this.AddToTrack(ActionType.Skip, Executor, ExecutorName, nd.NodeID, nd.Name, "自动跳转.(处理人已经出现过)", ndFrom);
                        this.HisFlow.DoFlowEventEntity(EventListOfNode.SendWhen, nd, skipWork, null);
                        ndFrom = nd;
                        this.HisFlow.DoFlowEventEntity(EventListOfNode.SendSuccess, nd, skipWork, null, this.HisMsgObjs);
                        continue;
                    }

                    //如果没有跳转,判断是否,其他两个条件是否设置.
                    if (nd.AutoJumpRole2 == false)
                        return nd;
                }

                if (nd.AutoJumpRole2)
                {
                    /* 处理人与上一步相同 */
                    bool isHave = false;
                    foreach (DataRow dr in dt.Rows)
                    {
                        string sql = "SELECT COUNT(*) FROM WF_GenerWorkerList WHERE FK_Emp='" + dr[0] +
                                     "' AND WorkID=" + this.WorkID + " AND FK_Node=" +
                                     (beforeSkipNodeID > 0 ? beforeSkipNodeID : prvNodeID); //edited by liuxc,2015-7-13
                        if (DBAccess.RunSQLReturnValInt(sql) == 1)
                        {
                            /*这里不处理签名.*/
                            isHave = true;
                            Executor = dr[0].ToString();
                            emp = new Emp(Executor);
                            ExecutorName = emp.Name;
                            break;
                        }
                    }

                    if (isHave == true)
                    {
                        //added by liuxc,2015-7-13,生成跳过的节点数据
                        //记录最开始相同处理人的节点ID，用来上面查找SQL判断
                        if (beforeSkipNodeID == 0)
                            beforeSkipNodeID = prvNodeID;

                        Work wk = nd.HisWork;
                        wk.Copy(this.rptGe);
                        wk.Rec = WebUser.No;
                        wk.OID = this.WorkID;
                        wk.ResetDefaultVal();

                        //执行表单填充，如果有，修改新昌方面同时修改本版本，added by liuxc,2015-10-16
                        MapExt item = nd.MapData.MapExts.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull) as MapExt;
                        BP.WF.Glo.DealPageLoadFull(wk, item, nd.MapData.MapAttrs, nd.MapData.MapDtls);

                        wk.DirectSave();

                        //added by liuxc,2015-10-16
                        #region  //此处时，跳转的节点如果有签章，则签章路径会计算错误，需要重新计算一下签章路径，暂时没找到好法子，将UCEn.ascx.cs中的计算签章的逻辑挪过来使用
                        FrmImgs imgs = new FrmImgs();
                        imgs.Retrieve(FrmImgAttr.FK_MapData, "ND" + nd.NodeID, FrmImgAttr.ImgAppType, 1, FrmImgAttr.IsEdit, 1);

                        foreach (FrmImg img in imgs)
                        {
                            //获取登录人岗位
                            string stationNo = "";
                            //签章对应部门
                            string fk_dept = WebUser.FK_Dept;
                            //部门来源类别
                            string sealType = "0";
                            //签章对应岗位
                            string fk_station = img.Tag0;
                            //表单字段
                            string sealField = "";
                            string imgSrc = "";
                            string sql = "";

                            //如果设置了部门与岗位的集合进行拆分
                            if (!DataType.IsNullOrEmpty(img.Tag0) && img.Tag0.Contains("^") && img.Tag0.Split('^').Length == 4)
                            {
                                fk_dept = img.Tag0.Split('^')[0];
                                fk_station = img.Tag0.Split('^')[1];
                                sealType = img.Tag0.Split('^')[2];
                                sealField = img.Tag0.Split('^')[3];
                                //如果部门没有设定，就获取部门来源
                                if (fk_dept == "all")
                                {
                                    //发起人
                                    if (sealType == "1")
                                    {
                                        sql = "SELECT FK_Dept FROM WF_GenerWorkFlow WHERE WorkID=" + wk.OID;
                                        fk_dept = BP.DA.DBAccess.RunSQLReturnString(sql);
                                    }
                                    //表单字段
                                    if (sealType == "2" && !DataType.IsNullOrEmpty(sealField))
                                    {
                                        //判断字段是否存在
                                        foreach (MapAttr attr in nd.MapData.MapAttrs)
                                        {
                                            if (attr.KeyOfEn == sealField)
                                            {
                                                fk_dept = wk.GetValStrByKey(sealField);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            sql = string.Format(" select FK_Station from Port_DeptStation where FK_Dept ='{0}' and FK_Station in (select FK_Station from " + BP.WF.Glo.EmpStation + " where FK_Emp='{1}')", fk_dept, WebUser.No);
                            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (fk_station.Contains(dr[0] + ","))
                                {
                                    stationNo = dr[0].ToString();
                                    break;
                                }
                            }

                            try
                            {
                                imgSrc = BP.WF.Glo.CCFlowAppPath + "DataUser/Seal/" + fk_dept + "_" + stationNo + ".jpg";
                                //设置主键
                                string myPK = DataType.IsNullOrEmpty(img.EnPK) ? "seal" : img.EnPK;
                                myPK = myPK + "_" + wk.OID + "_" + img.MyPK;

                                FrmEleDB imgDb = new FrmEleDB();
                                QueryObject queryInfo = new QueryObject(imgDb);
                                queryInfo.AddWhere(FrmEleAttr.MyPK, myPK);
                                queryInfo.DoQuery();
                                //判断是否存在
                                if (imgDb != null && !DataType.IsNullOrEmpty(imgDb.FK_MapData))
                                {
                                    imgDb.FK_MapData = DataType.IsNullOrEmpty(img.EnPK) ? "seal" : img.EnPK;
                                    imgDb.EleID = wk.OID.ToString();
                                    imgDb.RefPKVal = img.MyPK;
                                    imgDb.Tag1 = imgSrc;
                                    imgDb.Update();
                                }
                                else
                                {
                                    imgDb.FK_MapData = DataType.IsNullOrEmpty(img.EnPK) ? "seal" : img.EnPK;
                                    imgDb.EleID = wk.OID.ToString();
                                    imgDb.RefPKVal = img.MyPK;
                                    imgDb.Tag1 = imgSrc;
                                    imgDb.Insert();
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        this.AddToTrack(ActionType.Skip, Executor, ExecutorName, nd.NodeID, nd.Name, "自动跳转.(处理人与上一步相同)", ndFrom);
                        this.HisFlow.DoFlowEventEntity(EventListOfNode.SendWhen, nd, wk, null);
                        ndFrom = nd;
                        this.HisFlow.DoFlowEventEntity(EventListOfNode.SendSuccess, nd, wk, null, this.HisMsgObjs);
                        continue;
                    }

                    //没有跳出转的条件，就返回本身.
                    return nd;
                }

                mynd = nd;
                ndFrom = nd;
            }//结束循环。

            #endregion 计算到达的节点.


            throw new Exception("@找到下一步节点.");
        }
        /// <summary>
        /// 处理OrderTeamup退回模式
        /// </summary>
        public void DealReturnOrderTeamup()
        {
            /*如果协作，顺序方式.*/
            GenerWorkerList gwl = new GenerWorkerList();
            gwl.FK_Emp = WebUser.No;
            gwl.FK_Node = this.HisNode.NodeID;
            gwl.WorkID = this.WorkID;
            if (gwl.RetrieveFromDBSources() == 0)
                throw new Exception("@没有找到自己期望的数据.");

            gwl.IsPass = true;
            gwl.Update();

            gwl.FK_Emp = this.JumpToEmp;
            gwl.FK_Node = this.JumpToNode.NodeID;
            gwl.WorkID = this.WorkID;
            if (gwl.RetrieveFromDBSources() == 0)
                throw new Exception("@没有找到接受人期望的数据.");

            #region 要计算当前人员的应完成日期
            // 计算出来 退回到节点的应完成时间. 
            DateTime dtOfShould;
            if (this.HisFlow.HisTimelineRole == Template.TimelineRole.ByFlow)
            {
                /*如果整体流程是按流程设置计算 */
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                dtOfShould = DataType.ParseSysDateTime2DateTime(gwf.SDTOfFlow);
            }
            else
            {
                //增加天数. 考虑到了节假日.             
                dtOfShould = Glo.AddDayHoursSpan(DateTime.Now, this.HisNode.TimeLimit,
                    this.HisNode.TimeLimitHH, this.HisNode.TimeLimitMM, this.HisNode.TWay);
            }
            // 应完成日期.
            string sdt = dtOfShould.ToString(DataType.SysDataTimeFormat);
            #endregion

            //更新日期，为了考核. 
            if (this.HisNode.HisCHWay == CHWay.None)
                gwl.SDT = "无";
            else
                gwl.SDT = sdt;

            //  gwl.RDT = DataType.CurrentDataTime;

            gwl.IsPass = false;
            gwl.Update();

            GenerWorkerLists ens = new GenerWorkerLists();
            ens.AddEntity(gwl);
            this.HisWorkerLists = ens;

            this.addMsg(SendReturnMsgFlag.VarAcceptersID, gwl.FK_Emp, gwl.FK_Emp, SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarAcceptersName, gwl.FK_EmpText, gwl.FK_EmpText, SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.OverCurr, "@当前工作已经发送给退回人(" + gwl.FK_Emp + "," + gwl.FK_EmpText + ").", null, SendReturnMsgType.Info);

            this.HisGenerWorkFlow.WFState = WFState.Runing;
            this.HisGenerWorkFlow.FK_Node = gwl.FK_Node;
            this.HisGenerWorkFlow.NodeName = gwl.FK_NodeText;

            this.HisGenerWorkFlow.TodoEmps = gwl.FK_Emp + "," + gwl.FK_EmpText + ";";
            this.HisGenerWorkFlow.TodoEmpsNum = 0;
            this.HisGenerWorkFlow.TaskSta = TaskSta.None;
            this.HisGenerWorkFlow.Update();
        }
        /// <summary>
        /// 获取下一步骤的工作节点
        /// </summary>
        /// <returns></returns>
        private Node NodeSend_GenerNextStepNode_Ext(Node currNode)
        {
            // 检查当前的状态是是否是退回，.
            if (this.SendNodeWFState == WFState.ReturnSta)
            {

            }

            Nodes nds = currNode.HisToNodes;
            if (nds.Count == 1)
            {
                Node toND = (Node)nds[0];
                return toND;
            }

            if (nds.Count == 0)
                throw new Exception("没有找到它的下了步节点.");

            Conds dcsAll = new Conds();
            dcsAll.Retrieve(CondAttr.NodeID, currNode.NodeID, CondAttr.PRI);
            //if (dcsAll.Count == 0)
            //    throw new Exception("@没有为节点(" + currNode.NodeID + " , " + currNode.Name + ")设置方向条件.");

            #region 获取能够通过的节点集合，如果没有设置方向条件就默认通过.
            Nodes myNodes = new Nodes();
            int toNodeId = 0;
            int numOfWay = 0;
            foreach (Node nd in nds)
            {
                Conds dcs = new Conds();
                foreach (Cond dc in dcsAll)
                {
                    if (dc.ToNodeID != nd.NodeID)
                        continue;

                    dc.WorkID = this.WorkID;
                    dc.FID = this.HisWork.FID;

                    dc.en = this.rptGe;

                    dcs.AddEntity(dc);
                }

                if (dcs.Count == 0)
                {
                    continue;
                    //throw new Exception("@worknode 流程设计错误：流程{" + currNode.FlowName + "}从节点(" + currNode.Name + ")到节点(" + nd.Name + ")，没有设置方向条件，有分支的节点必须有方向条件。");
                }

                if (dcs.IsPass) // 如果通过了.
                    myNodes.AddEntity(nd);
            }
            #endregion 获取能够通过的节点集合，如果没有设置方向条件就默认通过.


            // 如果没有找到,就找到没有设置方向条件的节点,没有设置方向条件的节点是默认同意的.
            if (myNodes.Count == 0)
            {
                foreach (Node nd in nds)
                {
                    Conds dcs = new Conds();
                    foreach (Cond dc in dcsAll)
                    {
                        if (dc.ToNodeID != nd.NodeID)
                            continue;
                        dcs.AddEntity(dc);
                    }

                    if (dcs.Count == 0)
                        return nd;
                }
            }

            // 如果没有找到.
            if (myNodes.Count == 0)
                throw new Exception("@当前用户(" + this.ExecerName + "),定义节点的方向条件错误:从{" + currNode.NodeID + currNode.Name + "}节点到其它节点,定义的所有转向条件都不成立.");

            //如果找到1个.
            if (myNodes.Count == 1)
            {
                Node toND = myNodes[0] as Node;
                return toND;
            }

            //如果找到了多个.
            foreach (Cond dc in dcsAll)
            {
                foreach (Node myND in myNodes)
                {
                    if (dc.ToNodeID == myND.NodeID)
                    {
                        return myND;
                    }
                }
            }
            throw new Exception("@不应该出现的异常,不应该运行到这里.");
        }
        /// <summary>
        /// 获取下一步骤的节点集合
        /// </summary>
        /// <returns></returns>
        public Nodes Func_GenerNextStepNodes()
        {
            //如果跳转节点已经有了变量.
            if (this.JumpToNode != null)
            {
                Nodes myNodesTo = new Nodes();
                myNodesTo.AddEntity(this.JumpToNode);
                return myNodesTo;
            }

            if (this.HisNode.HisToNodes.Count == 1)
                return this.HisNode.HisToNodes;

            #region 如果使用户选择的.
            if (this.HisNode.CondModel == CondModel.ByUserSelected)
            {
                // 获取用户选择的节点.
                string nodes = this.HisGenerWorkFlow.Paras_ToNodes;
                if (DataType.IsNullOrEmpty(nodes))
                    throw new Exception("@用户没有选择发送到的节点.");

                Nodes nds = new Nodes();
                string[] mynodes = nodes.Split(',');
                foreach (string item in mynodes)
                {
                    if (DataType.IsNullOrEmpty(item))
                        continue;
                    nds.AddEntity(new Node(int.Parse(item)));
                }
                return nds;
            }
            #endregion 如果使用户选择的.




            Nodes toNodes = this.HisNode.HisToNodes;
            // 如果只有一个转向节点, 就不用判断条件了,直接转向他.
            if (toNodes.Count == 1)
                return toNodes;
            Conds dcsAll = new Conds();
            dcsAll.Retrieve(CondAttr.NodeID, this.HisNode.NodeID, CondAttr.PRI);

            #region 获取能够通过的节点集合，如果没有设置方向条件就默认通过.
            Nodes myNodes = new Nodes();
            int toNodeId = 0;
            int numOfWay = 0;

            foreach (Node nd in toNodes)
            {
                Conds dcs = new Conds();
                foreach (Cond dc in dcsAll)
                {
                    if (dc.ToNodeID != nd.NodeID)
                        continue;

                    dc.WorkID = this.HisWork.OID;
                    dc.en = this.rptGe;
                    dcs.AddEntity(dc);
                }

                if (dcs.Count == 0)
                {
                    myNodes.AddEntity(nd);
                    continue;
                }

                if (dcs.IsPass) // 如果多个转向条件中有一个成立.
                {
                    myNodes.AddEntity(nd);
                    continue;
                }
            }
            #endregion 获取能够通过的节点集合，如果没有设置方向条件就默认通过.

            #region 走到最后，发现一个条件都不符合，就找没有设置方向条件的节点. （@杜翻译）
            if (myNodes.Count == 0)
            {
                /*如果没有找到其他节点，就找没有设置方向条件的节点.*/
                foreach (Node nd in toNodes)
                {
                    Conds conds = new Conds();
                    int i = conds.Retrieve(CondAttr.NodeID, nd.NodeID, CondAttr.CondType, 2);
                    if (i == 0)
                        continue;

                    //增加到节点集合.
                    myNodes.AddEntity(nd);
                }

                //如果没有设置方向条件的节点有多个，就清除在后面抛出异常.
                if (myNodes.Count != 1)
                    myNodes.Clear();
            }
            #endregion 走到最后，发现一个条件都不符合，就找没有设置方向条件的节点.


            if (myNodes.Count == 0)
                throw new Exception(string.Format("@定义节点的方向条件错误:没有给从{0}节点到其它节点,定义转向条件或者您定义的所有转向条件都不成立.",
                    this.HisNode.NodeID + this.HisNode.Name));


            return myNodes;
        }
        /// <summary>
        /// 检查一下流程完成条件.
        /// </summary>
        /// <returns></returns>
        private void Func_CheckCompleteCondition()
        {
            if (this.HisNode.HisRunModel == RunModel.SubThread)
                throw new Exception("@流程设计错误：不允许在子线程上设置流程完成条件。");

            this.IsStopFlow = false;
            this.addMsg("CurrWorkOver", string.Format("当前工作[{0}]已经完成", this.HisNode.Name));

            #region 判断流程条件.
            try
            {
                if (this.HisNode.HisToNodes.Count == 0 && this.HisNode.IsStartNode)
                {
                    /* 如果流程完成 */
                    string overMsg = this.HisWorkFlow.DoFlowOver(ActionType.FlowOver, "符合流程完成条件", this.HisNode, this.rptGe);

                    if (this.HisGenerWorkFlow.TransferCustomType == TransferCustomType.ByCCBPMDefine)
                        this.IsStopFlow = true;
                    this.addMsg("OneNodeFlowOver", "@工作已经成功处理(一个流程的工作)。");
                }

                if (this.HisNode.IsCCFlow && this.HisFlowCompleteConditions.IsPass)
                {
                    /*如果有流程完成条件，并且流程完成条件是通过的。*/
                    string stopMsg = this.HisFlowCompleteConditions.ConditionDesc;
                    /* 如果流程完成 */
                    string overMsg = this.HisWorkFlow.DoFlowOver(ActionType.FlowOver, "符合流程完成条件:" + stopMsg, this.HisNode, this.rptGe);
                    this.IsStopFlow = true;

                    // string path = BP.Sys.Glo.Request.ApplicationPath;
                    string mymsg = "@符合工作流程完成条件" + stopMsg + "" + overMsg;
                    this.addMsg(SendReturnMsgFlag.FlowOver, mymsg, mymsg, SendReturnMsgType.Info);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("@判断流程{0}完成条件出现错误." + ex.StackTrace, this.HisNode.Name));
            }
            #endregion
        }
        /// <summary>
        /// 设置当前工作已经完成.
        /// </summary>
        /// <returns></returns>
        private string Func_DoSetThisWorkOver()
        {
            //设置结束人.  
            this.rptGe.SetValByKey(GERptAttr.FK_Dept, this.HisGenerWorkFlow.FK_Dept); //此值不能变化.
            this.rptGe.SetValByKey(GERptAttr.FlowEnder, this.Execer);
            this.rptGe.SetValByKey(GERptAttr.FlowEnderRDT, DataType.CurrentDataTime);
            if (this.town == null)
                this.rptGe.SetValByKey(GERptAttr.FlowEndNode, this.HisNode.NodeID);
            else
            {
                if (this.HisNode.HisRunModel == RunModel.FL || this.HisNode.HisRunModel == RunModel.FHL)
                    this.rptGe.SetValByKey(GERptAttr.FlowEndNode, this.HisNode.NodeID);
                else
                    this.rptGe.SetValByKey(GERptAttr.FlowEndNode, this.town.HisNode.NodeID);
            }

            this.rptGe.SetValByKey(GERptAttr.FlowDaySpan,
                DataType.GeTimeLimits(rptGe.FlowStartRDT, DataType.CurrentDataTime));

            //如果两个物理表不想等.
            if (this.HisWork.EnMap.PhysicsTable != this.rptGe.EnMap.PhysicsTable)
            {
                // 更新状态。
                this.HisWork.SetValByKey("CDT", DataType.CurrentDataTime);
                this.HisWork.Rec = this.Execer;

                //判断是不是MD5流程？
                if (this.HisFlow.IsMD5)
                    this.HisWork.SetValByKey("MD5", Glo.GenerMD5(this.HisWork));

                if (this.HisNode.IsStartNode)
                    this.HisWork.SetValByKey(StartWorkAttr.Title, this.HisGenerWorkFlow.Title);

                this.HisWork.DirectUpdate();
            }

            #region 2014-08-02 删除了其他人员的待办，增加了 IsPass=0 参数.
            if (this.town != null && this.town.HisNode.NodeID == this.HisNode.NodeID)
            {
                // 清除其他的工作者.
                //ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE IsPass=0 AND FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND FK_Emp <> " + dbStr + "FK_Emp";
                //ps.Clear();
                //ps.Add("FK_Node", this.HisNode.NodeID);
                //ps.Add("WorkID", this.WorkID);
                //ps.Add("FK_Emp", this.Execer);
                //DBAccess.RunSQL(ps);
            }
            else
            {
                // 清除其他的工作者.
                ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE IsPass=0 AND FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND FK_Emp <> " + dbStr + "FK_Emp";
                ps.Clear();
                ps.Add("FK_Node", this.HisNode.NodeID);
                ps.Add("WorkID", this.WorkID);
                ps.Add("FK_Emp", this.Execer);
                DBAccess.RunSQL(ps);
            }
            #endregion 2014-08-02 删除了其他人员的待办，增加了 IsPass=0 参数.

            if (this.town != null && this.town.HisNode.NodeID == this.HisNode.NodeID)
            {
                /*如果不是当前节点发给当前节点，就更新上一个节点全部完成。 */
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=1 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND FK_Emp=" + dbStr + "FK_Emp AND IsPass=0";
                ps.Add("FK_Node", this.HisNode.NodeID);
                ps.Add("WorkID", this.WorkID);
                ps.Add("FK_Emp", this.Execer);
                DBAccess.RunSQL(ps);
            }
            else
            {
                /*如果不是当前节点发给当前节点，就更新上一个节点全部完成。 */
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=1 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND IsPass=0";
                ps.Add("FK_Node", this.HisNode.NodeID);
                ps.Add("WorkID", this.WorkID);
                DBAccess.RunSQL(ps);
            }

            // 给generworkflow赋值。
            if (this.IsStopFlow == true)
                this.HisGenerWorkFlow.WFState = WFState.Complete;
            else
                this.HisGenerWorkFlow.WFState = WFState.Runing;

            // 流程应完成时间。
            if (this.HisWork.EnMap.Attrs.Contains(WorkSysFieldAttr.SysSDTOfFlow))
                this.HisGenerWorkFlow.SDTOfFlow = this.HisWork.GetValStrByKey(WorkSysFieldAttr.SysSDTOfFlow);

            // 下一个节点应完成时间。
            if (this.HisWork.EnMap.Attrs.Contains(WorkSysFieldAttr.SysSDTOfNode))
                this.HisGenerWorkFlow.SDTOfFlow = this.HisWork.GetValStrByKey(WorkSysFieldAttr.SysSDTOfNode);

            //执行更新。
            if (this.IsStopFlow == false)
                this.HisGenerWorkFlow.Update();

            return "@流程已经完成.";
        }
        #endregion 附属功能
        /// <summary>
        /// 普通节点到普通节点
        /// </summary>
        /// <param name="toND">要到达的下一个节点</param>
        /// <returns>执行消息</returns>
        private void NodeSend_11(Node toND)
        {
            string sql = "";
            string errMsg = "";
            Work toWK = toND.HisWork;
            toWK.OID = this.WorkID;
            toWK.FID = this.HisWork.FID;

            // 如果执行了跳转.
            if (this.IsSkip == true)
                toWK.RetrieveFromDBSources(); //有可能是跳转.

            #region 执行数据初始化
            // town.
            WorkNode town = new WorkNode(toWK, toND);

            errMsg = "初试化他们的工作人员 - 期间出现错误.";

            // 初试化他们的工作人员．
            current_gwls = this.Func_GenerWorkerLists(town);
            if (town.HisNode.TodolistModel == TodolistModel.Order && current_gwls.Count > 1)
            {
                /*如果到达的节点是队列流程节点，就要设置他们的队列顺序.*/
                int idx = 0;
                foreach (GenerWorkerList gwl in current_gwls)
                {
                    idx++;
                    if (idx == 1)
                        continue;
                    gwl.IsPassInt = idx + 100;
                    gwl.Update();
                }
            }

            if ((town.HisNode.TodolistModel == TodolistModel.Teamup || town.HisNode.TodolistModel == TodolistModel.TeamupGroupLeader) && current_gwls.Count > 1)
            {
                /*如果是协作模式 */
                if (town.HisNode.FWCOrderModel == 1)
                {
                    /* 如果是协作模式，并且显示排序按照官职大小排序. */
                    DateTime dt = DateTime.Now;
                    foreach (GenerWorkerList gwl in current_gwls)
                    {
                        dt = dt.AddMinutes(5);
                        string rdt = dt.ToString("yyyy-MM-dd HH:mm:ss");

                        BP.WF.Dev2Interface.WriteTrack(this.HisFlow.No, town.HisNode.NodeID, town.HisNode.Name, this.WorkID, town.HisWork.FID, "",
                            ActionType.WorkCheck, null, null, null, gwl.FK_Emp, gwl.FK_EmpText, gwl.FK_Emp, gwl.FK_EmpText, rdt);
                    }
                }
            }


            #region 保存目标节点数据.
            if (this.HisWork.EnMap.PhysicsTable != toWK.EnMap.PhysicsTable)
            {
                errMsg = "保存目标节点数据 - 期间出现错误.";

                //为下一步骤初始化数据.
                GenerWorkerList gwl = current_gwls[0] as GenerWorkerList;
                toWK.Rec = gwl.FK_Emp;
                string emps = gwl.FK_Emp;
                if (current_gwls.Count != 1)
                {
                    foreach (GenerWorkerList item in current_gwls)
                        emps += item.FK_Emp + ",";
                }
                toWK.Emps = emps;

                try
                {
                    if (this.IsSkip == true)
                    {
                        int count = toWK.RetrieveFromDBSources();
                        if(count >0)
                        toWK.DirectUpdate(); // 如果执行了跳转.
                        else
                            toWK.DirectInsert();
                    }
                    else
                        toWK.DirectInsert();
                }
                catch (Exception ex)
                {
                    Log.DefaultLogWriteLineInfo("@出现SQL异常有可能是没有修复表，或者重复发送. Ext=" + ex.Message);
                    try
                    {
                        toWK.CheckPhysicsTable();
                        toWK.DirectUpdate();
                    }
                    catch (Exception ex1)
                    {
                        Log.DefaultLogWriteLineInfo("@保存工作错误：" + ex1.Message);
                        throw new Exception("@保存工作错误：" + toWK.EnDesc + ex1.Message);
                    }
                }
            }
            #endregion 保存目标节点数据.

            //@加入消息集合里。
            this.SendMsgToThem(current_gwls);

            if (toND.IsGuestNode == true)
            {
                string htmlInfo = string.Format("@发送给如下{0}位处理人,{1}.", this.HisRememberMe.NumOfObjs.ToString(), this.HisGenerWorkFlow.GuestNo + " " + this.HisGenerWorkFlow.GuestName);

                string textInfo = string.Format("@发送给如下{0}位处理人,{1}.", this.HisRememberMe.NumOfObjs.ToString(), this.HisGenerWorkFlow.GuestName);

                this.addMsg(SendReturnMsgFlag.ToEmps, textInfo, htmlInfo);
            }
            else
            {
                string htmlInfo = string.Format("@发送给如下{0}位处理人,{1}.", this.HisRememberMe.NumOfObjs.ToString(), this.HisRememberMe.EmpsExt);

                string textInfo = string.Format("@发送给如下{0}位处理人,{1}.", this.HisRememberMe.NumOfObjs.ToString(), this.HisRememberMe.ObjsExt);

                this.addMsg(SendReturnMsgFlag.ToEmps, textInfo, htmlInfo);

            }

            #region 处理审核问题,更新审核组件插入的审核意见中的 到节点，到人员。
            try
            {
                Paras ps = new Paras();
                ps.SQL = "UPDATE ND" + int.Parse(toND.FK_Flow) + "Track SET NDTo=" + dbStr + "NDTo,NDToT=" + dbStr + "NDToT,EmpTo=" + dbStr + "EmpTo,EmpToT=" + dbStr + "EmpToT WHERE NDFrom=" + dbStr + "NDFrom AND EmpFrom=" + dbStr + "EmpFrom AND WorkID=" + dbStr + "WorkID AND ActionType=" + (int)ActionType.WorkCheck;
                ps.Add(TrackAttr.NDTo, toND.NodeID);
                ps.Add(TrackAttr.NDToT, toND.Name);
                ps.Add(TrackAttr.EmpTo, this.HisRememberMe.EmpsExt);
                ps.Add(TrackAttr.EmpToT, this.HisRememberMe.EmpsExt);
                ps.Add(TrackAttr.NDFrom, this.HisNode.NodeID);
                ps.Add(TrackAttr.EmpFrom, WebUser.No);
                ps.Add(TrackAttr.WorkID, this.WorkID);
                BP.DA.DBAccess.RunSQL(ps);
            }
            catch (Exception ex)
            {
                #region  如果更新失败，可能是由于数据字段大小引起。
                Flow flow = new Flow(toND.FK_Flow);

                string updateLengthSql = string.Format("  alter table {0} alter column {1} varchar(2000) ", "ND" + int.Parse(toND.FK_Flow) + "Track", "EmpFromT");
                BP.DA.DBAccess.RunSQL(updateLengthSql);

                updateLengthSql = string.Format("  alter table {0} alter column {1} varchar(2000) ", "ND" + int.Parse(toND.FK_Flow) + "Track", "EmpFrom");
                BP.DA.DBAccess.RunSQL(updateLengthSql);

                updateLengthSql = string.Format("  alter table {0} alter column {1} varchar(2000) ", "ND" + int.Parse(toND.FK_Flow) + "Track", "EmpTo");
                BP.DA.DBAccess.RunSQL(updateLengthSql);
                updateLengthSql = string.Format("  alter table {0} alter column {1} varchar(2000) ", "ND" + int.Parse(toND.FK_Flow) + "Track", "EmpToT");
                BP.DA.DBAccess.RunSQL(updateLengthSql);


                Paras ps = new Paras();
                ps.SQL = "UPDATE ND" + int.Parse(toND.FK_Flow) + "Track SET NDTo=" + dbStr + "NDTo,NDToT=" + dbStr + "NDToT,EmpTo=" + dbStr + "EmpTo,EmpToT=" + dbStr + "EmpToT WHERE NDFrom=" + dbStr + "NDFrom AND EmpFrom=" + dbStr + "EmpFrom AND WorkID=" + dbStr + "WorkID AND ActionType=" + (int)ActionType.WorkCheck;
                ps.Add(TrackAttr.NDTo, toND.NodeID);
                ps.Add(TrackAttr.NDToT, toND.Name);
                ps.Add(TrackAttr.EmpTo, this.HisRememberMe.EmpsExt);
                ps.Add(TrackAttr.EmpToT, this.HisRememberMe.EmpsExt);
                ps.Add(TrackAttr.NDFrom, this.HisNode.NodeID);
                ps.Add(TrackAttr.EmpFrom, WebUser.No);
                ps.Add(TrackAttr.WorkID, this.WorkID);
                BP.DA.DBAccess.RunSQL(ps);

                #endregion
            }
            #endregion 处理审核问题.

            //string htmlInfo = string.Format("@任务自动发送给{0}如下处理人{1}.", this.nextStationName,this._RememberMe.EmpsExt);
            //string textInfo = string.Format("@任务自动发送给{0}如下处理人{1}.", this.nextStationName,this._RememberMe.ObjsExt);

            if (this.HisWorkerLists.Count >= 2 && this.HisNode.IsTask)
            {
                if (WebUser.IsWap)
                    this.addMsg(SendReturnMsgFlag.AllotTask, null, "<a href='./WorkOpt/AllotTask.htm?WorkID=" + this.WorkID + "&FK_Node=" + toND.NodeID + "&FK_Flow=" + toND.FK_Flow + "'  target=_self ><img src='./Img/AllotTask.gif' border=0/>指定特定的处理人处理</a>。", SendReturnMsgType.Info);
                else
                    this.addMsg(SendReturnMsgFlag.AllotTask, null, "<a href='./WorkOpt/AllotTask.htm?WorkID=" + this.WorkID + "&FK_Node=" + toND.NodeID + "&FK_Flow=" + toND.FK_Flow + "'  target=_self><img src='./Img/AllotTask.gif' border=0/>指定特定的处理人处理</a>。", SendReturnMsgType.Info);
            }

            //if (WebUser.IsWap == false)
            //    this.addMsg(SendReturnMsgFlag.ToEmpExt, null, "@<a href=\"javascript:WinOpen('" + VirPath + "WF/Msg/SMS.aspx?WorkID=" + this.WorkID + "&FK_Node=" + toND.NodeID + "');\" ><img src='" + VirPath + "WF/Img/SMS.gif' border=0 />发手机短信提醒他(们)</a>", SendReturnMsgType.Info);


            if (this.HisNode.HisFormType != NodeFormType.SDKForm || 1 == 1)
            {
                if (this.HisNode.IsStartNode)
                    // this.addMsg(SendReturnMsgFlag.ToEmpExt, null, "@<a href='./WorkOpt/UnSend.htm?DoType=UnSend&WorkID=" + this.HisWork.OID + "&FK_Flow=" + toND.FK_Flow + "' ><img src='./Img/Action/UnSend.png' border=0/>撤销本次发送</a><a href='MyFlow.htm?FK_Flow=" + toND.FK_Flow + "&FK_Node=" + int.Parse(toND.FK_Flow) + "01'><img src='./Img/New.gif' border=0/>新建流程</a>.", SendReturnMsgType.Info);
                    this.addMsg(SendReturnMsgFlag.ToEmpExt, null, "@<a href='./WorkOpt/UnSend.htm?DoType=UnSend&UserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&WorkID=" + this.HisWork.OID + "&FK_Flow=" + toND.FK_Flow + "' ><img src='./Img/Action/UnSend.png' border=0/>撤销本次发送</a>", SendReturnMsgType.Info);
                else
                    this.addMsg(SendReturnMsgFlag.ToEmpExt, null, "@<a href='./WorkOpt/UnSend.htm?DoType=UnSend&UserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&WorkID=" + this.HisWork.OID + "&FK_Flow=" + toND.FK_Flow + "' ><img src='./Img/Action/UnSend.png' border=0 />撤销本次发送</a> ", SendReturnMsgType.Info);
            }

            this.HisGenerWorkFlow.FK_Node = toND.NodeID;
            this.HisGenerWorkFlow.NodeName = toND.Name;

            //ps = new Paras();
            //ps.SQL = "UPDATE WF_GenerWorkFlow SET WFState=" + (int)WFState.Runing + ", FK_Node=" + dbStr + "FK_Node,NodeName=" + dbStr + "NodeName WHERE WorkID=" + dbStr + "WorkID";
            //ps.Add("FK_Node", toND.NodeID);
            //ps.Add("NodeName", toND.Name);
            //ps.Add("WorkID", this.HisWork.OID);
            //DBAccess.RunSQL(ps);

            if (this.HisNode.HisFormType == NodeFormType.SDKForm || this.HisNode.HisFormType == NodeFormType.SelfForm)
            {
            }
            else
            {
                //this.addMsg(SendReturnMsgFlag.WorkRpt, null, "@<img src='./Img/Btn/PrintWorkRpt.gif' ><a href='WFRpt.htm?WorkID=" + this.HisWork.OID + "&FID=" + this.HisWork.FID + "&FK_Flow=" + toND.FK_Flow + "' target='_self' >工作轨迹</a>。");
            }
            this.addMsg(SendReturnMsgFlag.WorkStartNode, "@下一步[" + toND.Name + "]工作成功启动.", "@下一步<font color=blue>[" + toND.Name + "]</font>工作成功启动.");
            #endregion

            #region  初始化发起的工作节点。
            if (this.HisWork.EnMap.PhysicsTable == toWK.EnMap.PhysicsTable)
            {
                /*这是数据合并模式, 就不执行copy*/
                this.CopyData(toWK, toND, true);
            }
            else
            {
                /* 如果两个数据源不想等，就执行copy。 */
                this.CopyData(toWK, toND, false);
            }
            #endregion 初始化发起的工作节点.

            #region 判断是否是质量评价。
            if (toND.IsEval)
            {
                /*如果是质量评价流程*/
                toWK.SetValByKey(WorkSysFieldAttr.EvalEmpNo, this.Execer);
                toWK.SetValByKey(WorkSysFieldAttr.EvalEmpName, this.ExecerName);
                toWK.SetValByKey(WorkSysFieldAttr.EvalCent, 0);
                toWK.SetValByKey(WorkSysFieldAttr.EvalNote, "");
            }
            #endregion

        }

        /// <summary>
        /// 处理分流点向下发送 to 异表单.
        /// </summary>
        /// <returns></returns>
        private void NodeSend_24_UnSameSheet(Nodes toNDs)
        {
            //NodeSend_2X_GenerFH();

            /*分别启动每个节点的信息.*/
            string msg = "";

            #region 查询出来当前流程节点数据，为子线程的节点copy数据所用。
            //查询出来上一个节点的附件信息来。
            FrmAttachmentDBs athDBs = new FrmAttachmentDBs("ND" + this.HisNode.NodeID, this.WorkID.ToString());

            //查询出来上一个Ele信息来。
            FrmEleDBs eleDBs = new FrmEleDBs("ND" + this.HisNode.NodeID, this.WorkID.ToString());
            #endregion

            //定义系统变量.
            string workIDs = "";
            string empIDs = "";
            string empNames = "";
            string toNodeIDs = "";

            foreach (Node nd in toNDs)
            {
                msg += "@" + nd.Name + "工作已经启动，处理工作者：";

                //产生一个工作信息。
                Work wk = nd.HisWork;
                wk.Copy(this.HisWork);
                wk.FID = this.HisWork.OID;
                wk.OID = BP.DA.DBAccess.GenerOID("WorkID");
                wk.BeforeSave();
                wk.DirectInsert();

                //获得它的工作者。
                WorkNode town = new WorkNode(wk, nd);
                current_gwls = this.Func_GenerWorkerLists(town);
                if (current_gwls.Count == 0)
                {
                    msg += "@" + nd.Name + "工作已经启动，处理工作者：";
                    msg += "节点:" + nd.Name + "，没有找到可处理的人员，此线程节点无法启动。";
                    wk.Delete();
                    continue;
                }

                #region 执行数据copy.
                if (athDBs.Count > 0)
                {
                    //插入之前先删除.
                    FrmAttachmentDB mydb = new FrmAttachmentDB();
                    mydb.Delete(FrmAttachmentDBAttr.FK_MapData, "ND" + nd.NodeID, FrmAttachmentDBAttr.RefPKVal, wk.OID);

                    /*说明当前节点有附件数据*/
                    int idx = 0;
                    foreach (FrmAttachmentDB athDB in athDBs)
                    {
                        idx++;
                        FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                        athDB_N.Copy(athDB);
                        athDB_N.FK_MapData = "ND" + nd.NodeID;
                        athDB_N.MyPK = BP.DA.DBAccess.GenerGUID();
                        athDB_N.FK_FrmAttachment = athDB_N.FK_FrmAttachment.Replace("ND" + this.HisNode.NodeID, "ND" + nd.NodeID);
                        athDB_N.RefPKVal = wk.OID.ToString();
                        athDB_N.Insert();
                    }
                }

                if (eleDBs.Count > 0)
                {
                    /*说明当前节点有附件数据*/
                    int idx = 0;
                    foreach (FrmEleDB eleDB in eleDBs)
                    {
                        idx++;
                        FrmEleDB eleDB_N = new FrmEleDB();
                        eleDB_N.Copy(eleDB);
                        eleDB_N.FK_MapData = "ND" + nd.NodeID;
                        eleDB_N.Insert();
                    }
                }
                #endregion 执行数据copy.

                foreach (GenerWorkerList wl in current_gwls)
                {
                    msg += wl.FK_Emp + "，" + wl.FK_EmpText + "、";
                    // 产生工作的信息。
                    GenerWorkFlow gwf = new GenerWorkFlow();
                    gwf.WorkID = wk.OID;
                    if (gwf.IsExits == false)
                    {
                        gwf.FID = this.WorkID;

                        //#warning 需要修改成标题生成规则。
                        //#warning 让子流程的Titlte与父流程的一样.

                        gwf.Title = this.HisGenerWorkFlow.Title; // WorkNode.GenerTitle(this.rptGe);
                        gwf.WFState = WFState.Runing;
                        gwf.RDT = DataType.CurrentDataTime;
                        gwf.Starter = this.Execer;
                        gwf.StarterName = this.ExecerName;
                        gwf.FK_Flow = nd.FK_Flow;
                        gwf.FlowName = nd.FlowName;
                        gwf.FK_FlowSort = this.HisNode.HisFlow.FK_FlowSort;
                        gwf.SysType = this.HisNode.HisFlow.SysType;

                        gwf.FK_Node = nd.NodeID;
                        gwf.NodeName = nd.Name;
                        gwf.FK_Dept = wl.FK_Dept;
                        gwf.DeptName = wl.FK_DeptT;
                        gwf.TodoEmps = wl.FK_Emp + "," + wl.FK_EmpText + ";";
                        gwf.DirectInsert();
                    }

                    ps = new Paras();
                    ps.SQL = "UPDATE WF_GenerWorkerlist SET WorkID=" + dbStr + "WorkID1,FID=" + dbStr + "FID WHERE FK_Emp=" + dbStr + "FK_Emp AND WorkID=" + dbStr + "WorkID2 AND FK_Node=" + dbStr + "FK_Node ";
                    ps.Add("WorkID1", wk.OID);
                    ps.Add("FID", this.WorkID);

                    ps.Add("FK_Emp", wl.FK_Emp);
                    ps.Add("WorkID2", wl.WorkID);
                    ps.Add("FK_Node", wl.FK_Node);
                    DBAccess.RunSQL(ps);

                    //设置当前的workid.
                    wl.WorkID = wk.OID;

                    //记录变量.
                    workIDs += wk.OID.ToString() + ",";
                    empIDs += wl.FK_Emp + ",";
                    empNames += wl.FK_EmpText + ",";
                    toNodeIDs += gwf.FK_Node + ",";

                    //更新工作信息.
                    wk.Rec = wl.FK_Emp;
                    wk.Emps = "@" + wl.FK_Emp;
                    //wk.RDT = DataType.CurrentDataTimess;
                    wk.DirectUpdate();
                }
            }

            //加入分流异表单，提示信息。
            this.addMsg("FenLiuUnSameSheet", msg);

            //加入变量。
            this.addMsg(SendReturnMsgFlag.VarTreadWorkIDs, workIDs, workIDs, SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarAcceptersID, empIDs, empIDs, SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarAcceptersName, empNames, empNames, SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarToNodeIDs, toNodeIDs, toNodeIDs, SendReturnMsgType.SystemMsg);

            //写入日志. @yuanlina
            if (this.HisNode.IsStartNode == true)
                this.AddToTrack(ActionType.Start, empIDs, empNames, this.HisNode.NodeID, this.HisNode.Name, msg);
            else
                this.AddToTrack(ActionType.Forward, empIDs, empNames, this.HisNode.NodeID, this.HisNode.Name, msg);

        }
        /// <summary>
        /// 产生分流点
        /// </summary>
        /// <param name="toWN"></param>
        /// <returns></returns>
        private GenerWorkerLists NodeSend_24_SameSheet_GenerWorkerList(WorkNode toWN)
        {
            return null;
        }
        /// <summary>
        /// 当前产生的接受人员列表.
        /// </summary>
        private GenerWorkerLists current_gwls = null;
        /// <summary>
        /// 处理分流点向下发送 to 同表单.
        /// </summary>
        /// <param name="toNode">到达的分流节点</param>
        private void NodeSend_24_SameSheet(Node toNode)
        {
            if (this.HisGenerWorkFlow.Title == "未生成")
                this.HisGenerWorkFlow.Title = BP.WF.WorkFlowBuessRole.GenerTitle(this.HisFlow, this.HisWork);

            #region 删除到达节点的子线程如果有，防止退回信息垃圾数据问题,如果退回处理了这个部分就不需要处理了.
            ps = new Paras();
            ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE FID=" + dbStr + "FID  AND FK_Node=" + dbStr + "FK_Node";
            ps.Add("FID", this.HisWork.OID);
            ps.Add("FK_Node", toNode.NodeID);
            #endregion 删除到达节点的子线程如果有，防止退回信息垃圾数据问题，如果退回处理了这个部分就不需要处理了.


            #region 产生下一步骤的工作人员
            // 发起.
            Work wk = toNode.HisWork;
            wk.Copy(this.rptGe);
            wk.Copy(this.HisWork);  //复制过来主表基础信息。
            wk.FID = this.HisWork.OID; // 把该工作FID设置成干流程上的工作ID.

            // 到达的节点.
            town = new WorkNode(wk, toNode);

            // 产生下一步骤要执行的人员.
            current_gwls = this.Func_GenerWorkerLists(town);

            //给当前工作人员增加已经处理的历史步骤. add 2015-01-14,这样做的目的就是，可以让分流节点的发送人员看到每个子线程的在途工作.
            current_gwls.Delete(GenerWorkerListAttr.FK_Node, this.HisNode.NodeID, GenerWorkerListAttr.FID, this.WorkID); //首先清除.

            //清除以前的数据，比如两次发送。
            if (this.HisFlow.HisDataStoreModel == DataStoreModel.ByCCFlow)
                wk.Delete(WorkAttr.FID, this.HisWork.OID);

            // 判断分流的次数.是不是历史记录里面有分流。
            bool IsHaveFH = false;
            if (this.HisNode.IsStartNode == false)
            {
                ps = new Paras();
                ps.SQL = "SELECT COUNT(WorkID) FROM WF_GenerWorkerlist WHERE FID=" + dbStr + "OID";
                ps.Add("OID", this.HisWork.OID);
                if (DBAccess.RunSQLReturnValInt(ps) != 0)
                    IsHaveFH = true;
            }
            #endregion 产生下一步骤的工作人员

            #region 复制数据.
            //获得当前流程数节点数据.
            FrmAttachmentDBs athDBs = new FrmAttachmentDBs("ND" + this.HisNode.NodeID,
                                            this.WorkID.ToString());

            MapDtls dtlsFrom = new MapDtls("ND" + this.HisNode.NodeID);
            if (dtlsFrom.Count > 1)
            {
                foreach (MapDtl d in dtlsFrom)
                    d.HisGEDtls_temp = null;
            }
            MapDtls dtlsTo = null;
            if (dtlsFrom.Count >= 1)
                dtlsTo = new MapDtls("ND" + toNode.NodeID);

            ///定义系统变量.
            string workIDs = "";

            DataTable dtWork = null;
            if (toNode.HisDeliveryWay == DeliveryWay.BySQLAsSubThreadEmpsAndData)
            {
                /*如果是按照查询ＳＱＬ，确定明细表的接收人与子线程的数据。*/
                string sql = toNode.DeliveryParas;
                sql = Glo.DealExp(sql, this.HisWork, null);
                dtWork = BP.DA.DBAccess.RunSQLReturnTable(sql);
            }
            if (toNode.HisDeliveryWay == DeliveryWay.ByDtlAsSubThreadEmps)
            {
                /*如果是按照明细表，确定明细表的接收人与子线程的数据。*/
                foreach (MapDtl dtl in dtlsFrom)
                {
                    //加上顺序，防止变化，人员编号变化，处理明细表中接受人重复的问题。
                    string sql = "SELECT * FROM " + dtl.PTable + " WHERE RefPK=" + this.WorkID + " ORDER BY OID";
                    dtWork = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    if (dtWork.Columns.Contains("UserNo"))
                        break;
                    else
                        dtWork = null;
                }
            }

            string groupMark = "";
            int idx = -1;
            foreach (GenerWorkerList wl in current_gwls)
            {
                if (this.IsHaveSubThreadGroupMark == true)
                {
                    /*如果启用了批次处理,子线程的问题..*/
                    if (groupMark.Contains("@" + wl.FK_Emp + "," + wl.GroupMark) == false)
                        groupMark += "@" + wl.FK_Emp + "," + wl.GroupMark;
                    else
                    {
                        wl.Delete(); //删除该条垃圾数据.
                        continue;
                    }
                }

                idx++;
                Work mywk = toNode.HisWork;

                #region 复制数据.
                mywk.Copy(this.rptGe);
                //  mywk.Copy(this.HisWork);  // 复制过来信息。
                if (dtWork != null)
                {
                    /*用IDX处理是为了解决，人员重复出现在数据源并且还能根据索引对应的上。*/
                    DataRow dr = dtWork.Rows[idx];
                    if (dtWork.Columns.Contains("UserNo")
                        && dr["UserNo"].ToString() == wl.FK_Emp)
                    {
                        mywk.Copy(dr);
                    }

                    if (dtWork.Columns.Contains("No")
                       && dr["No"].ToString() == wl.FK_Emp)
                    {
                        mywk.Copy(dr);
                    }
                }
                #endregion 复制数据.

                bool isHaveEmp = false;

                //是否有分流的明细表?
                bool isHaveFLDtl = false;
                foreach (MapDtl dtl in dtlsFrom)
                {
                    if (dtl.IsFLDtl == true)
                    {
                        isHaveFLDtl = true;
                        break;
                    }
                }


                //是否是分组工作流程, 定义变量是为了，不让其在重复插入work数据。
                bool isGroupMarkWorklist = false;

                if (IsHaveFH)
                {
                    /* 如果曾经走过分流合流，就找到同一个人员同一个FID下的OID ，做这当前线程的ID。*/
                    ps = new Paras();
                    ps.SQL = "SELECT WorkID,FK_Node FROM WF_GenerWorkerlist WHERE FK_Node!=" + dbStr + "FK_Node AND FID=" + dbStr + "FID AND FK_Emp=" + dbStr + "FK_Emp ORDER BY RDT DESC";
                    ps.Add("FK_Node", toNode.NodeID);
                    ps.Add("FID", this.WorkID);
                    ps.Add("FK_Emp", wl.FK_Emp);
                    DataTable dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                    {
                        /*没有发现，就说明以前分流节点中没有这个人的分流信息. */
                        mywk.OID = DBAccess.GenerOID("WorkID");
                    }
                    else
                    {
                        int workid_old = (int)dt.Rows[0][0];
                        int fk_Node_nearly = (int)dt.Rows[0][1];
                        Node nd_nearly = new Node(fk_Node_nearly);
                        Work nd_nearly_work = nd_nearly.HisWork;
                        nd_nearly_work.OID = workid_old;
                        if (nd_nearly_work.RetrieveFromDBSources() != 0)
                        {
                            mywk.Copy(nd_nearly_work);
                            mywk.OID = workid_old;
                        }
                        else
                        {
                            mywk.OID = DBAccess.GenerOID("WorkID");
                        }

                        // 明细表数据汇总表，要复制到子线程的主表上去.
                        foreach (MapDtl dtl in dtlsFrom)
                        {
                            if (dtl.IsHLDtl == false)
                                continue;

                            string sql = "SELECT * FROM " + dtl.PTable + " WHERE Rec='" + wl.FK_Emp + "' AND RefPK='" + this.WorkID + "'";
                            DataTable myDT = DBAccess.RunSQLReturnTable(sql);
                            if (myDT.Rows.Count == 1)
                            {
                                Attrs attrs = mywk.EnMap.Attrs;
                                foreach (Attr attr in attrs)
                                {

                                    switch (attr.Key)
                                    {
                                        case GEDtlAttr.FID:
                                        case GEDtlAttr.OID:
                                        case GEDtlAttr.Rec:
                                        case GEDtlAttr.RefPK:
                                            continue;
                                        default:
                                            break;
                                    }

                                    if (attr.IsRefAttr == true)
                                        continue;
                                    if (attr.Field == null)
                                        continue; //

                                    if (myDT.Columns.Contains(attr.Field) == true)
                                        mywk.SetValByKey(attr.Key, myDT.Rows[0][attr.Field]);
                                }
                            }
                        }
                        isHaveEmp = true;
                    }
                }
                else
                {
                    //为子线程产生WorkID.
                    /* edit by zhoupeng 2015.12.24 平安夜. 处理国机的需求，判断是否有分组的情况，如果有就要找到分组的workid
                     * 让其同一个分组，只能生成一个workid。 
                     * */
                    if (this.IsHaveSubThreadGroupMark == true)
                    {
                        //查询该GroupMark 是否已经注册到流程引擎主表里了.
                        string sql = "SELECT WorkID FROM WF_GenerWorkFlow WHERE AtPara LIKE '%GroupMark=" + wl.GroupMark + "%' AND FID=" + this.WorkID;
                        DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                        if (dt.Rows.Count == 0)
                        {
                            mywk.OID = DBAccess.GenerOID("WorkID");  //BP.DA.DBAccess.GenerOID();
                        }
                        else
                        {
                            mywk.OID = Int64.Parse(dt.Rows[0][0].ToString()); //使用该分组下的，已经注册过的分组的WorkID，而非产生一个新的WorkID。
                            isGroupMarkWorklist = true; //是分组数据，让其work 就不要在重复插入了.
                        }
                    }
                    else
                    {
                        mywk.OID = DBAccess.GenerOID("WorkID");  //BP.DA.DBAccess.GenerOID();
                    }
                }

                //非分组工作人员.
                if (isGroupMarkWorklist == false)
                {
                    if (this.HisWork.FID == 0)
                        mywk.FID = this.HisWork.OID;

                    mywk.Rec = wl.FK_Emp;
                    mywk.Emps = wl.FK_Emp;
                    mywk.BeforeSave();

                    //判断是不是MD5流程？
                    if (this.HisFlow.IsMD5)
                        mywk.SetValByKey("MD5", Glo.GenerMD5(mywk));
                }


                #region 处理烟台（安检模式的流程）需求, 需要合流节点有一个明细表，根据明细表启动子线程任务,并要求把明细表的一行数据copy到下一个子线程的主表上去。
                if (isHaveFLDtl == true)
                {
                    /* 如果设置了当前节点是分流节点，并且有明细表是分流明细表，我们就要从这里copy数据。*/
                    foreach (MapDtl dtl in this.HisWork.HisMapDtls)
                    {
                        if (dtl.IsFLDtl == false)
                            continue; /*如果不是分流数据。*/

                        //获取明细数据。
                        GEDtls gedtls = null;
                        if (dtl.HisGEDtls_temp == null)
                        {
                            gedtls = new GEDtls(dtl.No);
                            QueryObject qo = null;
                            qo = new QueryObject(gedtls);
                            switch (dtl.DtlOpenType)
                            {
                                case DtlOpenType.ForEmp:
                                    qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                                    break;
                                case DtlOpenType.ForWorkID:
                                    qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                                    break;
                                case DtlOpenType.ForFID:
                                    qo.AddWhere(GEDtlAttr.FID, this.WorkID);
                                    break;
                            }
                            qo.DoQuery();
                            dtl.HisGEDtls_temp = gedtls;
                        }
                        gedtls = dtl.HisGEDtls_temp;

                        //找到数据，并copy到里面去.
                        foreach (GEDtl gedtl in gedtls)
                        {
                            string worker = gedtl.GetValStringByKey(dtl.SubThreadWorker);
                            if (worker.Contains(wl.FK_Emp) == false)
                                continue;

                            if (DataType.IsNullOrEmpty(dtl.SubThreadGroupMark) == false)
                            {
                                string groupMarkDtl = gedtl.GetValStringByKey(dtl.SubThreadGroupMark);
                                if (wl.GroupMark != groupMarkDtl)
                                    continue;
                            }

                            //开始执行数据copy,
                            mywk.Copy(gedtl);
                            //  wk.DirectUpdate();
                            break;
                        }
                    }
                }
                #endregion 处理烟台需求, 需要合流节点有一个明细表，根据明细表启动子线程任务,并要求把明细表的一行数据copy到下一个子线程的主表上去。

                //非分组工作人员.
                if (isGroupMarkWorklist == false)
                {
                    //保存主表数据.
                    mywk.InsertAsOID(mywk.OID);
                    //给系统变量赋值，放在发送后返回对象里.
                    workIDs += mywk.OID + ",";

                    #region  复制附件信息
                    if (athDBs.Count > 0)
                    {
                        /* 说明当前节点有附件数据 */
                        athDBs.Delete(FrmAttachmentDBAttr.FK_MapData, "ND" + toNode.NodeID,
                            FrmAttachmentDBAttr.RefPKVal, mywk.OID);

                        foreach (FrmAttachmentDB athDB in athDBs)
                        {
                            FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                            athDB_N.Copy(athDB);
                            athDB_N.FK_MapData = "ND" + toNode.NodeID;
                            athDB_N.RefPKVal = mywk.OID.ToString();
                            athDB_N.FK_FrmAttachment = athDB_N.FK_FrmAttachment.Replace("ND" + this.HisNode.NodeID,
                              "ND" + toNode.NodeID);

                            if (athDB_N.HisAttachmentUploadType == AttachmentUploadType.Single)
                            {
                                //注意如果是单附件主键的命名规则不能变化，否则会导致与前台约定获取数据错误。
                                athDB_N.MyPK = athDB_N.FK_FrmAttachment + "_" + mywk.OID;
                                try
                                {
                                    athDB_N.DirectInsert();
                                }
                                catch
                                {
                                    athDB_N.MyPK = BP.DA.DBAccess.GenerGUID();
                                    athDB_N.Insert();
                                }
                            }
                            else
                            {
                                try
                                {
                                    // 多附件就是: FK_MapData+序列号的方式, 替换主键让其可以保存,不会重复.
                                    athDB_N.MyPK = athDB_N.UploadGUID + "_" + athDB_N.FK_MapData + "_" + athDB_N.RefPKVal;
                                    athDB_N.DirectInsert();
                                }
                                catch
                                {
                                    athDB_N.MyPK = BP.DA.DBAccess.GenerGUID();
                                    athDB_N.Insert();
                                }
                            }
                        }
                    }
                    #endregion  复制附件信息

                    #region  复制签名信息
                    if (this.HisNode.MapData.FrmImgs.Count > 0)
                    {
                        foreach (FrmImg img in this.HisNode.MapData.FrmImgs)
                        {
                            //排除图片
                            if (img.HisImgAppType == ImgAppType.Img)
                                continue;
                            //获取数据
                            FrmEleDBs eleDBs = new FrmEleDBs(img.MyPK, this.WorkID.ToString());
                            if (eleDBs.Count > 0)
                            {
                                eleDBs.Delete(FrmEleDBAttr.FK_MapData, img.MyPK.Replace("ND" + this.HisNode.NodeID, "ND" + toNode.NodeID)
                                    , FrmEleDBAttr.EleID, this.WorkID);

                                /*说明当前节点有附件数据*/
                                foreach (FrmEleDB eleDB in eleDBs)
                                {
                                    FrmEleDB eleDB_N = new FrmEleDB();
                                    eleDB_N.Copy(eleDB);
                                    eleDB_N.FK_MapData = img.EnPK.Replace("ND" + this.HisNode.NodeID, "ND" + toNode.NodeID);
                                    eleDB_N.RefPKVal = img.EnPK.Replace("ND" + this.HisNode.NodeID, "ND" + toNode.NodeID);
                                    eleDB_N.EleID = mywk.OID.ToString();
                                    eleDB_N.GenerPKVal();
                                    eleDB_N.Save();
                                }
                            }
                        }
                    }
                    #endregion  复制附件信息

                    #region 复制图片上传附件。
                    if (this.HisNode.MapData.FrmImgAths.Count > 0)
                    {
                        FrmImgAthDBs frmImgAthDBs = new FrmImgAthDBs("ND" + this.HisNode.NodeID,
                              this.WorkID.ToString());
                        if (frmImgAthDBs.Count > 0)
                        {
                            frmImgAthDBs.Delete(FrmAttachmentDBAttr.FK_MapData, "ND" + toNode.NodeID,
                                FrmAttachmentDBAttr.RefPKVal, mywk.OID);

                            /*说明当前节点有附件数据*/
                            foreach (FrmImgAthDB imgAthDB in frmImgAthDBs)
                            {
                                FrmImgAthDB imgAthDB_N = new FrmImgAthDB();
                                imgAthDB_N.Copy(imgAthDB);
                                imgAthDB_N.FK_MapData = "ND" + toNode.NodeID;
                                imgAthDB_N.RefPKVal = mywk.OID.ToString();
                                imgAthDB_N.FK_FrmImgAth = imgAthDB_N.FK_FrmImgAth.Replace("ND" + this.HisNode.NodeID, "ND" + toNode.NodeID);
                                imgAthDB_N.Save();
                            }
                        }
                    }
                    #endregion 复制图片上传附件。

                    #region  复制从表信息.
                    if (dtlsFrom.Count > 0 && dtlsTo.Count > 0)
                    {
                        int i = -1;
                        foreach (Sys.MapDtl dtl in dtlsFrom)
                        {
                            i++;
                            if (dtlsTo.Count <= i)
                                continue;

                            Sys.MapDtl toDtl = (Sys.MapDtl)dtlsTo[i];
                            if (toDtl.IsCopyNDData == false)
                                continue;

                            if (toDtl.PTable == dtl.PTable)
                                continue;

                            //获取明细数据。
                            GEDtls gedtls = null;
                            if (dtl.HisGEDtls_temp == null)
                            {
                                gedtls = new GEDtls(dtl.No);
                                QueryObject qo = null;
                                qo = new QueryObject(gedtls);
                                switch (dtl.DtlOpenType)
                                {
                                    case DtlOpenType.ForEmp:
                                        qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                                        break;
                                    case DtlOpenType.ForWorkID:
                                        qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                                        break;
                                    case DtlOpenType.ForFID:
                                        qo.AddWhere(GEDtlAttr.FID, this.WorkID);
                                        break;
                                }
                                qo.DoQuery();
                                dtl.HisGEDtls_temp = gedtls;
                            }
                            gedtls = dtl.HisGEDtls_temp;

                            int unPass = 0;
                            DBAccess.RunSQL("DELETE FROM " + toDtl.PTable + " WHERE RefPK=" + dbStr + "RefPK", "RefPK", mywk.OID);
                            foreach (GEDtl gedtl in gedtls)
                            {
                                BP.Sys.GEDtl dtCopy = new GEDtl(toDtl.No);
                                dtCopy.Copy(gedtl);
                                dtCopy.FK_MapDtl = toDtl.No;
                                dtCopy.RefPK = mywk.OID.ToString();
                                dtCopy.OID = 0;
                                dtCopy.Insert();

                                #region  复制从表单条 - 附件信息 - M2M- M2MM
                                if (toDtl.IsEnableAthM)
                                {
                                    /*如果启用了多附件,就复制这条明细数据的附件信息。*/
                                    athDBs = new FrmAttachmentDBs(dtl.No, gedtl.OID.ToString());
                                    if (athDBs.Count > 0)
                                    {
                                        i = 0;
                                        foreach (FrmAttachmentDB athDB in athDBs)
                                        {
                                            i++;
                                            FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                                            athDB_N.Copy(athDB);
                                            athDB_N.FK_MapData = toDtl.No;
                                            athDB_N.MyPK = toDtl.No + "_" + dtCopy.OID + "_" + i.ToString();
                                            athDB_N.FK_FrmAttachment = athDB_N.FK_FrmAttachment.Replace("ND" + this.HisNode.NodeID,
                                                "ND" + toNode.NodeID);
                                            athDB_N.RefPKVal = dtCopy.OID.ToString();
                                            athDB_N.DirectInsert();
                                        }
                                    }
                                }
                                #endregion  复制从表单条 - 附件信息
                            }
                        }
                    }
                    #endregion  复制附件信息
                }

                #region (循环最后处理)产生工作的信息
                // 产生工作的信息。
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = mywk.OID;
                if (gwf.RetrieveFromDBSources() == 0)
                {
                    gwf.FID = this.WorkID;
                    gwf.FK_Node = toNode.NodeID;

                    if (this.HisNode.IsStartNode)
                        gwf.Title = BP.WF.WorkFlowBuessRole.GenerTitle(this.HisFlow, this.HisWork) + "(" + wl.FK_EmpText + ")";
                    else
                        gwf.Title = this.HisGenerWorkFlow.Title + "(" + wl.FK_EmpText + ")";

                    gwf.WFState = WFState.Runing;
                    gwf.RDT = DataType.CurrentDataTime;
                    gwf.Starter = this.Execer;
                    gwf.StarterName = this.ExecerName;
                    gwf.FK_Flow = toNode.FK_Flow;
                    gwf.FlowName = toNode.FlowName;

                    gwf.FID = this.WorkID;
                    gwf.FK_FlowSort = toNode.HisFlow.FK_FlowSort;
                    gwf.NodeName = toNode.Name;
                    gwf.FK_Dept = wl.FK_Dept;
                    gwf.DeptName = wl.FK_DeptT;
                    gwf.TodoEmps = wl.FK_Emp + "," + wl.FK_EmpText;
                    if (wl.GroupMark != "")
                        gwf.Paras_GroupMark = wl.GroupMark;

                    gwf.Sender = BP.WF.Glo.DealUserInfoShowModel(WebUser.No, WebUser.Name);

                    gwf.DirectInsert();
                }
                else
                {
                    if (wl.GroupMark != "")
                        gwf.Paras_GroupMark = wl.GroupMark;

                    gwf.Sender = BP.WF.Glo.DealUserInfoShowModel(WebUser.No, WebUser.Name);
                    gwf.FK_Node = toNode.NodeID;
                    gwf.NodeName = toNode.Name;
                    gwf.Update();
                }


                // 插入当前分流节点的处理人员,让其可以在在途里看到工作.
                //非分组工作人员.
                if (isGroupMarkWorklist == false)
                {
                    GenerWorkerList flGwl = new GenerWorkerList();
                    flGwl.Copy(wl);

                    //flGwl.WorkID = this.WorkID;

                    flGwl.FK_Emp = WebUser.No;
                    flGwl.FK_EmpText = WebUser.Name;
                    flGwl.FK_Node = this.HisNode.NodeID;
                    flGwl.Sender = WebUser.No + "," + WebUser.Name;
                    // flGwl.GroupMark = "";
                    flGwl.IsPassInt = -2; // -2; //标志该节点是干流程人员处理的节点.
                    //  wl.FID = 0; //如果是干流，
                    flGwl.Insert();
                }

                //把临时的workid 更新到.
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerlist SET WorkID=" + dbStr + "WorkID1 WHERE WorkID=" + dbStr + "WorkID2";
                ps.Add("WorkID1", mywk.OID);
                ps.Add("WorkID2", wl.WorkID); //临时的ID,更新最新的workid.
                int num = DBAccess.RunSQL(ps);
                if (num == 0)
                    throw new Exception("@不应该更新不到它。");

                //设置当前的workid. 临时的id有变化.
                wl.WorkID = mywk.OID;
                #endregion 产生工作的信息.
            }
            #endregion 复制数据.

            #region 处理消息提示
            string info = "@分流节点:{0}已经发起,@发送给 (" + this.HisRememberMe.NumOfObjs + ") 位处理人{1}.";
            this.addMsg("FenLiuInfo",
                string.Format(info, toNode.Name, this.HisRememberMe.EmpsExt));

            //把子线程的 WorkIDs 加入系统变量.
            this.addMsg(SendReturnMsgFlag.VarTreadWorkIDs, workIDs, workIDs, SendReturnMsgType.SystemMsg);

            // 如果是开始节点，就可以允许选择接受人。
            if (this.HisNode.IsStartNode)
            {
                if (current_gwls.Count >= 2 && this.HisNode.IsTask)
                    this.addMsg("AllotTask", "@<img src='./Img/AllotTask.gif' border=0 /><a href='./WorkOpt/AllotTask.htm?WorkID=" + this.WorkID + "&FID=" + this.WorkID + "&NodeID=" + toNode.NodeID + "' target=_self >修改接受对象</a>.");
            }

            if (this.HisNode.IsStartNode)
            {
                //this.addMsg("UnDoNew", "@<a href='./WorkOpt/UnSend.htm?DoType=UnSend&UserNo="+WebUser.No+"&SID="+WebUser.SID+"&WorkID=" + this.WorkID + "&FK_Flow=" + toNode.FK_Flow + "' ><img src='./Img/Action/UnSend.png' border=0/>撤销本次发送</a>， <a href='MyFlow.htm?FK_Flow=" + toNode.FK_Flow + "&FK_Node=" + int.Parse(toNode.FK_Flow) + "01' ><img src='./Img/New.gif' border=0/>新建流程</a>。");
                this.addMsg("UnDo", "@<a href='./WorkOpt/UnSend.htm?DoType=UnSend&UserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&WorkID=" + this.WorkID + "&FK_Flow=" + toNode.FK_Flow + "' ><img src='./Img/Action/UnSend.png' border=0/>撤销本次发送</a>.");
            }
            else
            {
                this.addMsg("UnDo", "@<a href='./WorkOpt/UnSend.htm?DoType=UnSend&UserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&WorkID=" + this.WorkID + "&FK_Flow=" + toNode.FK_Flow + "' ><img src='./Img/Action/UnSend.png' border=0/>撤销本次发送</a>.");
            }

            //  this.addMsg("Rpt", "@<a href='WFRpt.htm?WorkID=" + this.WorkID + "&FID=" + wk.FID + "&FK_Flow=" + this.HisNode.FK_Flow + "' target='_self' >工作轨迹</a>");
            #endregion 处理消息提示
        }
        /// <summary>
        /// 合流点到普通点发送
        /// 1. 首先要检查完成率.
        /// 2, 按普通节点向普通节点发送.
        /// </summary>
        /// <returns></returns>
        private void NodeSend_31(Node nd)
        {
            //检查完成率.

            // 与1-1一样的逻辑处理.
            this.NodeSend_11(nd);
        }
        /// <summary>
        /// 子线程向下发送
        /// </summary>
        /// <returns></returns>
        private string NodeSend_4x()
        {
            return null;
        }
        /// <summary>
        /// 子线程向合流点
        /// </summary>
        /// <returns></returns>
        private void NodeSend_53_SameSheet_To_HeLiu(Node toNode)
        {
            Work toNodeWK = toNode.HisWork;
            toNodeWK.Copy(this.HisWork);
            toNodeWK.OID = this.HisWork.FID;
            toNodeWK.FID = 0;
            this.town = new WorkNode(toNodeWK, toNode);

            // 获取到达当前合流节点上 与上一个分流点之间的子线程节点的集合。
            string spanNodes = this.SpanSubTheadNodes(toNode);

            #region 处理FID.
            Int64 fid = this.HisWork.FID;
            if (fid == 0)
            {
                if (this.HisNode.HisRunModel != RunModel.SubThread)
                    throw new Exception("@当前节点非子线程节点.");

                string strs = BP.DA.DBAccess.RunSQLReturnStringIsNull("SELECT FID FROM WF_GenerWorkFlow WHERE WorkID=" + this.HisWork.OID, "0");
                if (strs == "0")
                    throw new Exception("@丢失FID信息");
                fid = Int64.Parse(strs);

                this.HisWork.FID = fid;
            }
            #endregion FID

            // 先查询一下是否有人员，在合流节点上，如果没有就让其初始化人员. 
            current_gwls = new GenerWorkerLists();
            current_gwls.Retrieve(GenerWorkerListAttr.WorkID, this.HisWork.FID, GenerWorkerListAttr.FK_Node, toNode.NodeID);
            if (current_gwls.Count == 0)
                current_gwls = this.Func_GenerWorkerLists(this.town);// 初试化他们的工作人员．

            string FK_Emp = "";
            string toEmpsStr = "";
            string emps = "";
            foreach (GenerWorkerList wl in current_gwls)
            {
                toEmpsStr += BP.WF.Glo.DealUserInfoShowModel(wl.FK_Emp, wl.FK_EmpText);

                if (current_gwls.Count == 1)
                    emps = wl.FK_Emp;
                else
                    emps += "@" + FK_Emp;
            }
            //增加变量.
            this.addMsg(SendReturnMsgFlag.VarAcceptersID, emps.Replace("@", ","), SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarAcceptersName, toEmpsStr, SendReturnMsgType.SystemMsg);

            /* 
            * 更新它的节点 worklist 信息, 说明当前节点已经完成了.
            * 不让当前的操作员能看到自己的工作。
            */

            #region 设置父流程状态 设置当前的节点为:

            Work mainWK = town.HisWork;
            mainWK.OID = this.HisWork.FID;
            mainWK.RetrieveFromDBSources();

            // 复制报表上面的数据到合流点上去。
            DataTable dt = DBAccess.RunSQLReturnTable("SELECT * FROM " + this.HisFlow.PTable + " WHERE OID=" + dbStr + "OID",
                "OID", this.HisWork.FID);
            foreach (DataColumn dc in dt.Columns)
                mainWK.SetValByKey(dc.ColumnName, dt.Rows[0][dc.ColumnName]);

            mainWK.Rec = FK_Emp;
            mainWK.Emps = emps;
            mainWK.OID = this.HisWork.FID;
            mainWK.Save();

            // 产生合流汇总从表数据.
            this.GenerHieLiuHuiZhongDtlData_2013(toNode);

            //设置当前子线程已经通过.
            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=1  WHERE WorkID=" + dbStr + "WorkID AND FID=" + dbStr + "FID AND IsPass=0";
            ps.Add("WorkID", this.WorkID);
            ps.Add("FID", this.HisWork.FID);
            DBAccess.RunSQL(ps);

            /* 合流点需要等待各个分流点全部处理完后才能看到它。*/
            string mysql = "";

#warning 对于多个分合流点可能会有问题。
            mysql = "SELECT COUNT(distinct WorkID) AS Num FROM WF_GenerWorkerList WHERE IsEnable=1 AND FID=" + this.HisWork.FID + " AND FK_Node IN (" + spanNodes + ")";
            decimal numAll = (decimal)DBAccess.RunSQLReturnValInt(mysql);

            mysql = "SELECT COUNT(distinct WorkID) AS Num FROM WF_GenerWorkerList WHERE IsPass=1 AND FID=" + this.HisWork.FID + " AND FK_Node IN (" + spanNodes + ")";
            decimal numPassed = (decimal)DBAccess.RunSQLReturnValInt(mysql);

            decimal passRate = numPassed / numAll * 100;
            if (toNode.PassRate <= passRate)
            {
                /* 这时已经通过,可以让主线程看到待办. */
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=0 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID";
                ps.Add("FK_Node", toNode.NodeID);
                ps.Add("WorkID", this.HisWork.FID);
                int num = DBAccess.RunSQL(ps);
                if (num == 0)
                    throw new Exception("@不应该更新不到它.");
            }
            else
            {
#warning 为了不让其显示在途的工作需要， =3 不是正常的处理模式。
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=3 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID";
                ps.Add("FK_Node", toNode.NodeID);
                ps.Add("WorkID", this.HisWork.FID);
                int num = DBAccess.RunSQL(ps);
                if (num == 0)
                    throw new Exception("@不应该更新不到它.");
            }

            this.HisGenerWorkFlow.FK_Node = toNode.NodeID;
            this.HisGenerWorkFlow.NodeName = toNode.Name;

            //改变当前流程的当前节点.
            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET WFState=" + (int)WFState.Runing + ",  FK_Node=" + dbStr + "FK_Node,NodeName=" + dbStr + "NodeName WHERE WorkID=" + dbStr + "WorkID";
            ps.Add("FK_Node", toNode.NodeID);
            ps.Add("NodeName", toNode.Name);
            ps.Add("WorkID", this.HisWork.FID);
            DBAccess.RunSQL(ps);


            #endregion 设置父流程状态

            this.addMsg("InfoToHeLiu", "@流程已经运行到合流节点[" + toNode.Name + "]。@您的工作已经发送给如下人员[" + toEmpsStr + "]，@您是第一个到达此节点的处理人.");

            #region 处理国机的需求, 把最后一个子线程的主表数据同步到合流节点的Rpt里面去.(不是很合理) 2015.12.30
            Work towk = town.HisWork;
            towk.OID = this.HisWork.FID;
            towk.RetrieveFromDBSources();
            towk.Copy(this.HisWork);
            towk.DirectUpdate();
            #endregion 处理国机的需求, 把最后一个子线程的主表数据同步到合流节点的Rpt里面去.

        }
        private string NodeSend_55(Node toNode)
        {
            return null;
        }
        /// <summary>
        /// 节点向下运动
        /// </summary>
        private void NodeSend_Send_5_5()
        {
            //执行设置当前人员的完成时间. for: anhua 2013-12-18.
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET CDT=" + dbstr + "CDT WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node AND FK_Emp=" + dbstr + "FK_Emp";
            ps.Add(GenerWorkerListAttr.CDT, DataType.CurrentDataTimess);
            ps.Add(GenerWorkerListAttr.WorkID, this.WorkID);
            ps.Add(GenerWorkerListAttr.FK_Node, this.HisNode.NodeID);
            ps.Add(GenerWorkerListAttr.FK_Emp, this.Execer);
            BP.DA.DBAccess.RunSQL(ps);

            #region 检查当前的状态是是否是退回,如果是退回的状态，就给他赋值.
            // 检查当前的状态是是否是退回，.
            if (this.SendNodeWFState == WFState.ReturnSta)
            {
                /*检查该退回是否是原路返回?*/
                ps = new Paras();
                ps.SQL = "SELECT ReturnNode,Returner,IsBackTracking FROM WF_ReturnWork WHERE WorkID=" + dbStr + "WorkID AND IsBackTracking=1 ORDER BY RDT DESC";
                ps.Add(ReturnWorkAttr.WorkID, this.WorkID);
                DataTable dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count != 0)
                {
                    //有可能查询出来多个，因为按时间排序了，只取出最后一次退回的，看看是否有退回并原路返回的信息。

                    /*确认这次退回，是退回并原路返回 ,  在这里初始化它的工作人员, 与将要发送的节点. */
                    this.JumpToNode = new Node(int.Parse(dt.Rows[0]["ReturnNode"].ToString()));
                    this.JumpToEmp = dt.Rows[0]["Returner"].ToString();
                    this.IsSkip = true; //如果不设置为true, 将会删除目标数据.
                    //  this.NodeSend_11(this.JumpToNode);
                }
            }
            #endregion.

            switch (this.HisNode.HisRunModel)
            {
                case RunModel.Ordinary: /* 1： 普通节点向下发送的*/
                    Node toND = this.NodeSend_GenerNextStepNode();
                    if (this.IsStopFlow)
                        return;

                    if (this.HisNode.FK_Flow != toND.FK_Flow)
                    {
                        NodeSendToYGFlow(toND, JumpToEmp);
                        return;
                    }

                    //写入到达信息.
                    this.addMsg(SendReturnMsgFlag.VarToNodeID, toND.NodeID.ToString(), toND.NodeID.ToString(), SendReturnMsgType.SystemMsg);
                    this.addMsg(SendReturnMsgFlag.VarToNodeName, toND.Name, toND.Name, SendReturnMsgType.SystemMsg);
                    switch (toND.HisRunModel)
                    {
                        case RunModel.Ordinary:   /*1-1 普通节to普通节点 */
                            this.NodeSend_11(toND);
                            break;
                        case RunModel.FL:  /* 1-2 普通节to分流点 */
                            this.NodeSend_11(toND);
                            break;
                        case RunModel.HL:  /*1-3 普通节to合流点   */
                            this.NodeSend_11(toND);
                            // throw new Exception("@流程设计错误:请检查流程获取详细信息, 普通节点下面不能连接合流节点(" + toND.Name + ").");
                            break;
                        case RunModel.FHL: /*1-4 普通节点to分合流点 */
                            this.NodeSend_11(toND);
                            break;
                        // throw new Exception("@流程设计错误:请检查流程获取详细信息, 普通节点下面不能连接分合流节点(" + toND.Name + ").");
                        case RunModel.SubThread: /*1-5 普通节to子线程点 */
                            throw new Exception("@流程设计错误:请检查流程获取详细信息, 普通节点下面不能连接子线程节点(" + toND.Name + ").");
                        default:
                            throw new Exception("@没有判断的节点类型(" + toND.Name + ")");
                            break;
                    }
                    break;
                case RunModel.FL: /* 2: 分流节点向下发送的*/
                    Nodes toNDs = this.Func_GenerNextStepNodes();
                    if (toNDs.Count == 1)
                    {
                        Node toND2 = toNDs[0] as Node;
                        //加入系统变量.
                        this.addMsg(SendReturnMsgFlag.VarToNodeID, toND2.NodeID.ToString(), toND2.NodeID.ToString(), SendReturnMsgType.SystemMsg);
                        this.addMsg(SendReturnMsgFlag.VarToNodeName, toND2.Name, toND2.Name, SendReturnMsgType.SystemMsg);

                        switch (toND2.HisRunModel)
                        {
                            case RunModel.Ordinary:    /*2.1 分流点to普通节点 */
                                this.NodeSend_11(toND2); /* 按普通节点到普通节点处理. */
                                break;
                            case RunModel.FL:  /*2.2 分流点to分流点  */
                            //  throw new Exception("@流程设计错误:请检查流程获取详细信息, 分流点(" + this.HisNode.Name + ")下面不能连接分流节点(" + toND2.Name + ").");
                            case RunModel.HL:  /*2.3 分流点to合流点,分合流点. */
                            case RunModel.FHL:
                                this.NodeSend_11(toND2); /* 按普通节点到普通节点处理. */
                                break;
                            // throw new Exception("@流程设计错误:请检查流程获取详细信息, 分流点(" + this.HisNode.Name + ")下面不能连接合流节点(" + toND2.Name + ").");
                            case RunModel.SubThread: /* 2.4 分流点to子线程点   */
                                if (toND2.HisSubThreadType == SubThreadType.SameSheet)
                                    NodeSend_24_SameSheet(toND2);
                                else
                                    NodeSend_24_UnSameSheet(toNDs); /*可能是只发送1个异表单*/
                                break;
                            default:
                                throw new Exception("@没有判断的节点类型(" + toND2.Name + ")");
                                break;
                        }
                    }
                    else
                    {
                        /* 如果有多个节点，检查一下它们必定是子线程节点否则，就是设计错误。*/
                        bool isHaveSameSheet = false;
                        bool isHaveUnSameSheet = false;
                        foreach (Node nd in toNDs)
                        {
                            switch (nd.HisRunModel)
                            {
                                case RunModel.Ordinary:
                                    NodeSend_11(nd); /*按普通节点到普通节点处理.*/
                                    break;
                                case RunModel.FL:
                                case RunModel.FHL:
                                case RunModel.HL:
                                    NodeSend_11(nd); /*按普通节点到普通节点处理.*/
                                    break;
                                //    throw new Exception("@流程设计错误:请检查流程获取详细信息, 分流点(" + this.HisNode.Name + ")下面不能连接分流节点(" + nd.Name + ").");
                                //case RunModel.FHL:
                                //    throw new Exception("@流程设计错误:请检查流程获取详细信息, 分流点(" + this.HisNode.Name + ")下面不能连接分合流节点(" + nd.Name + ").");
                                //case RunModel.HL:
                                //    throw new Exception("@流程设计错误:请检查流程获取详细信息, 分流点(" + this.HisNode.Name + ")下面不能连接合流节点(" + nd.Name + ").");
                                default:
                                    break;
                            }
                            if (nd.HisSubThreadType == SubThreadType.SameSheet)
                                isHaveSameSheet = true;

                            if (nd.HisSubThreadType == SubThreadType.UnSameSheet)
                                isHaveUnSameSheet = true;
                        }

                        if (isHaveUnSameSheet && isHaveSameSheet)
                            throw new Exception("@不支持流程模式: 分流节点同时启动了同表单的子线程与异表单的子线程.");

                        if (isHaveSameSheet == true)
                            throw new Exception("@不支持流程模式: 分流节点同时启动了多个同表单的子线程.");

                        //启动多个异表单子线程节点.
                        this.NodeSend_24_UnSameSheet(toNDs);
                    }
                    break;
                case RunModel.HL:  /* 3: 合流节点向下发送 */
                    Node toND3 = this.NodeSend_GenerNextStepNode();
                    if (this.IsStopFlow)
                        return;

                    //加入系统变量.
                    this.addMsg(SendReturnMsgFlag.VarToNodeID, toND3.NodeID.ToString(), toND3.NodeID.ToString(), SendReturnMsgType.SystemMsg);
                    this.addMsg(SendReturnMsgFlag.VarToNodeName, toND3.Name, toND3.Name, SendReturnMsgType.SystemMsg);

                    switch (toND3.HisRunModel)
                    {
                        case RunModel.Ordinary: /*3.1 普通工作节点 */
                            this.NodeSend_31(toND3); /* 让它与普通点点普通点一样的逻辑. */
                            break;
                        case RunModel.FL: /*3.2 分流点 */
                            this.NodeSend_31(toND3); /* 让它与普通点点普通点一样的逻辑. */
                            break;
                        //throw new Exception("@流程设计错误:请检查流程获取详细信息, 合流点(" + this.HisNode.Name + ")下面不能连接分流节点(" + toND3.Name + ").");
                        case RunModel.HL: /*3.3 合流点 */
                        case RunModel.FHL:
                            this.NodeSend_31(toND3); /* 让它与普通点点普通点一样的逻辑. */
                            break;
                        //throw new Exception("@流程设计错误:请检查流程获取详细信息, 合流点(" + this.HisNode.Name + ")下面不能连接合流节点(" + toND3.Name + ").");
                        case RunModel.SubThread:/*3.4 子线程*/
                            throw new Exception("@流程设计错误:请检查流程获取详细信息, 合流点(" + this.HisNode.Name + ")下面不能连接子线程节点(" + toND3.Name + ").");
                        default:
                            throw new Exception("@没有判断的节点类型(" + toND3.Name + ")");
                    }
                    break;
                case RunModel.FHL:  /* 4: 分流节点向下发送的 */
                    Node toND4 = this.NodeSend_GenerNextStepNode();
                    if (this.IsStopFlow)
                        return;

                    //加入系统变量.
                    this.addMsg(SendReturnMsgFlag.VarToNodeID, toND4.NodeID.ToString(), toND4.NodeID.ToString(), SendReturnMsgType.SystemMsg);
                    this.addMsg(SendReturnMsgFlag.VarToNodeName, toND4.Name, toND4.Name, SendReturnMsgType.SystemMsg);

                    switch (toND4.HisRunModel)
                    {
                        case RunModel.Ordinary: /*4.1 普通工作节点 */
                            this.NodeSend_11(toND4); /* 让它与普通点点普通点一样的逻辑. */
                            break;
                        case RunModel.FL: /*4.2 分流点 */
                            throw new Exception("@流程设计错误:请检查流程获取详细信息, 合流点(" + this.HisNode.Name + ")下面不能连接分流节点(" + toND4.Name + ").");
                        case RunModel.HL: /*4.3 合流点 */
                        case RunModel.FHL:
                            this.NodeSend_11(toND4); /* 让它与普通点点普通点一样的逻辑. */
                            break;
                        //  throw new Exception("@流程设计错误:请检查流程获取详细信息, 合流点(" + this.HisNode.Name + ")下面不能连接合流节点(" + toND4.Name + ").");
                        case RunModel.SubThread:/*4.5 子线程*/
                            if (toND4.HisSubThreadType == SubThreadType.SameSheet)
                            {
                                NodeSend_24_SameSheet(toND4);
                            }
                            else
                            {
                                Nodes toNDs4 = this.Func_GenerNextStepNodes();
                                NodeSend_24_UnSameSheet(toNDs4); /*可能是只发送1个异表单*/
                            }
                            break;
                        //throw new Exception("@流程设计错误:请检查流程获取详细信息, 合流点(" + this.HisNode.Name + ")下面不能连接子线程节点(" + toND4.Name + ").");
                        default:
                            throw new Exception("@没有判断的节点类型(" + toND4.Name + ")");
                    }
                    break;

                case RunModel.SubThread:  /* 5: 子线程节点向下发送的 */
                    Node toND5 = this.NodeSend_GenerNextStepNode();
                    if (this.IsStopFlow)
                        return;

                    //加入系统变量.
                    this.addMsg(SendReturnMsgFlag.VarToNodeID, toND5.NodeID.ToString(), toND5.NodeID.ToString(), SendReturnMsgType.SystemMsg);
                    this.addMsg(SendReturnMsgFlag.VarToNodeName, toND5.Name, toND5.Name, SendReturnMsgType.SystemMsg);

                    switch (toND5.HisRunModel)
                    {
                        case RunModel.Ordinary: /*5.1 普通工作节点 */
                            throw new Exception("@流程设计错误:请检查流程获取详细信息, 子线程点(" + this.HisNode.Name + ")下面不能连接普通节点(" + toND5.Name + ").");
                            break;
                        case RunModel.FL: /*5.2 分流点 */
                            throw new Exception("@流程设计错误:请检查流程获取详细信息, 子线程点(" + this.HisNode.Name + ")下面不能连接分流节点(" + toND5.Name + ").");
                        case RunModel.HL: /*5.3 合流点 */
                        case RunModel.FHL: /*5.4 分合流点 */
                            if (this.HisNode.HisSubThreadType == SubThreadType.SameSheet)
                                this.NodeSend_53_SameSheet_To_HeLiu(toND5);
                            else
                                this.NodeSend_53_UnSameSheet_To_HeLiu(toND5);

                            //把合流点设置未读.
                            ps = new Paras();
                            ps.SQL = "UPDATE WF_GenerWorkerList SET IsRead=0 WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND  FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node";
                            ps.Add("WorkID", this.HisWork.FID);
                            ps.Add("FK_Node", toND5.NodeID);
                            BP.DA.DBAccess.RunSQL(ps);
                            break;
                        case RunModel.SubThread: /*5.5 子线程*/
                            if (toND5.HisSubThreadType == this.HisNode.HisSubThreadType)
                            {
                                #region 删除到达节点的子线程如果有，防止退回信息垃圾数据问题,如果退回处理了这个部分就不需要处理了.
                                ps = new Paras();
                                ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE FID=" + dbStr + "FID  AND FK_Node=" + dbStr + "FK_Node";
                                ps.Add("FID", this.HisWork.FID);
                                ps.Add("FK_Node", toND5.NodeID);
                                #endregion 删除到达节点的子线程如果有，防止退回信息垃圾数据问题，如果退回处理了这个部分就不需要处理了.

                                this.NodeSend_11(toND5); /*与普通节点一样.*/
                            }
                            else
                                throw new Exception("@流程设计模式错误：连续两个子线程的子线程模式不一样，从节点(" + this.HisNode.Name + ")到节点(" + toND5.Name + ")");
                            break;
                        default:
                            throw new Exception("@没有判断的节点类型(" + toND5.Name + ")");
                    }
                    break;
                default:
                    throw new Exception("@没有判断的执行节点类型:" + this.HisNode.HisRunModelT);
            }
        }

        #region 执行数据copy.
        public void CopyData(Work toWK, Node toND, bool isSamePTable)
        {
            //如果存储模式为, 合并模式.
            if (this.HisFlow.HisDataStoreModel == DataStoreModel.SpecTable)
                return;

            string errMsg = "如果两个数据源不想等，就执行 copy - 期间出现错误.";
            if (isSamePTable == true)
                return;

            #region 主表数据copy.
            if (isSamePTable == false)
            {
                toWK.SetValByKey("OID", this.HisWork.OID); //设定它的ID.
                if (this.HisNode.IsStartNode == false)
                    toWK.Copy(this.rptGe);

                toWK.Copy(this.HisWork); // 执行 copy 上一个节点的数据。
                toWK.Rec = this.Execer;

                //要考虑FID的问题.
                if (this.HisNode.HisRunModel == RunModel.SubThread
                    && toND.HisRunModel == RunModel.SubThread)
                    toWK.FID = this.HisWork.FID;

                try
                {
                    //判断是不是MD5流程？
                    if (this.HisFlow.IsMD5)
                        toWK.SetValByKey("MD5", Glo.GenerMD5(toWK));

                    if (toWK.IsExits)
                        toWK.Update();
                    else
                        toWK.Insert();
                }
                catch (Exception ex)
                {
                    toWK.CheckPhysicsTable();
                    try
                    {
                        toWK.Copy(this.HisWork); // 执行 copy 上一个节点的数据。
                        toWK.Rec = this.Execer;
                        toWK.SaveAsOID(toWK.OID);
                    }
                    catch (Exception ex11)
                    {
                        if (toWK.Update() == 0)
                            throw new Exception(ex.Message + " == " + ex11.Message);
                    }
                }
            }
            #endregion 主表数据copy.

            #region 复制附件。
            if (this.HisNode.MapData.FrmAttachments.Count > 0)
            {
                //删除上一个节点可能有的数据，有可能是发送退回来的产生的垃圾数据.
                Paras ps = new Paras();
                ps.SQL = "DELETE FROM Sys_FrmAttachmentDB WHERE FK_MapData=" + dbStr + "FK_MapData AND RefPKVal=" + dbStr + "RefPKVal";
                ps.Add(FrmAttachmentDBAttr.FK_MapData, "ND" + toND.NodeID);
                ps.Add(FrmAttachmentDBAttr.RefPKVal, this.WorkID);
                DBAccess.RunSQL(ps);

                FrmAttachmentDBs athDBs = new FrmAttachmentDBs("ND" + this.HisNode.NodeID,
                      this.WorkID.ToString());

                int idx = 0;
                if (athDBs.Count > 0)
                {
                    /*说明当前节点有附件数据*/
                    foreach (FrmAttachmentDB athDB in athDBs)
                    {
                        FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                        athDB_N.Copy(athDB);
                        athDB_N.FK_MapData = "ND" + toND.NodeID;
                        athDB_N.RefPKVal = this.HisWork.OID.ToString();
                        athDB_N.FK_FrmAttachment = athDB_N.FK_FrmAttachment.Replace("ND" + this.HisNode.NodeID,
                          "ND" + toND.NodeID);

                        if (athDB_N.HisAttachmentUploadType == AttachmentUploadType.Single)
                        {
                            /*如果是单附件.*/
                            athDB_N.MyPK = athDB_N.FK_FrmAttachment + "_" + this.HisWork.OID;
                            //if (athDB_N.IsExits == true)
                            //    continue; /*说明上一个节点或者子线程已经copy过了, 但是还有子线程向合流点传递数据的可能，所以不能用break.*/
                            try
                            {
                                athDB_N.Insert();
                            }
                            catch
                            {
                                athDB_N.MyPK = BP.DA.DBAccess.GenerGUID();
                                athDB_N.Insert();
                            }
                        }
                        else
                        {
                            ////判断这个guid 的上传文件是否被其他的线程copy过去了？
                            //if (athDB_N.IsExit(FrmAttachmentDBAttr.UploadGUID, athDB_N.UploadGUID,
                            //    FrmAttachmentDBAttr.FK_MapData, athDB_N.FK_MapData) == true)
                            //    continue; /*如果是就不要copy了.*/

                            athDB_N.MyPK = athDB_N.UploadGUID + "_" + athDB_N.FK_MapData + "_" + toWK.OID;
                            try
                            {
                                athDB_N.Insert();
                            }
                            catch
                            {
                                athDB_N.MyPK = BP.DA.DBAccess.GenerGUID();
                                athDB_N.Insert();
                            }
                        }
                    }
                }
            }
            #endregion 复制附件。

            #region 复制图片上传附件。
            if (this.HisNode.MapData.FrmImgAths.Count > 0)
            {
                FrmImgAthDBs athDBs = new FrmImgAthDBs("ND" + this.HisNode.NodeID,
                      this.WorkID.ToString());
                int idx = 0;
                if (athDBs.Count > 0)
                {
                    athDBs.Delete(FrmAttachmentDBAttr.FK_MapData, "ND" + toND.NodeID,
                        FrmAttachmentDBAttr.RefPKVal, this.WorkID);

                    /*说明当前节点有附件数据*/
                    foreach (FrmImgAthDB athDB in athDBs)
                    {
                        idx++;
                        FrmImgAthDB athDB_N = new FrmImgAthDB();
                        athDB_N.Copy(athDB);
                        athDB_N.FK_MapData = "ND" + toND.NodeID;
                        athDB_N.RefPKVal = this.WorkID.ToString();
                        athDB_N.MyPK = this.WorkID + "_" + idx + "_" + athDB_N.FK_MapData;
                        athDB_N.FK_FrmImgAth = athDB_N.FK_FrmImgAth.Replace("ND" + this.HisNode.NodeID, "ND" + toND.NodeID);
                        athDB_N.Save();
                    }
                }
            }
            #endregion 复制图片上传附件。

            #region 复制Ele
            if (this.HisNode.MapData.FrmImgs.Count > 0)
            {
                foreach (FrmImg img in this.HisNode.MapData.FrmImgs)
                {
                    //排除图片
                    if (img.HisImgAppType == ImgAppType.Img)
                        continue;
                    //获取数据
                    FrmEleDBs eleDBs = new FrmEleDBs(img.EnPK, this.WorkID.ToString());
                    if (eleDBs.Count > 0)
                    {
                        eleDBs.Delete(FrmEleDBAttr.FK_MapData, img.EnPK.Replace("ND" + this.HisNode.NodeID, "ND" + toND.NodeID)
                            , FrmEleDBAttr.EleID, this.WorkID);

                        /*说明当前节点有附件数据*/
                        foreach (FrmEleDB eleDB in eleDBs)
                        {
                            FrmEleDB eleDB_N = new FrmEleDB();
                            eleDB_N.Copy(eleDB);
                            eleDB_N.FK_MapData = img.EnPK.Replace("ND" + this.HisNode.NodeID, "ND" + toND.NodeID);
                            eleDB_N.RefPKVal = img.EnPK.Replace("ND" + this.HisNode.NodeID, "ND" + toND.NodeID);
                            eleDB_N.GenerPKVal();
                            eleDB_N.Save();
                        }
                    }
                }
            }
            #endregion 复制Ele

            #region 复制明细数据
            // int deBugDtlCount=
            Sys.MapDtls dtls = this.HisNode.MapData.MapDtls;
            string recDtlLog = "@记录测试明细表Copy过程,从节点ID:" + this.HisNode.NodeID + " WorkID:" + this.WorkID + ", 到节点ID=" + toND.NodeID;
            if (dtls.Count > 0)
            {
                Sys.MapDtls toDtls = toND.MapData.MapDtls;
                recDtlLog += "@到节点明细表数量是:" + dtls.Count + "个";

                Sys.MapDtls startDtls = null;
                bool isEnablePass = false; /*是否有明细表的审批.*/
                foreach (MapDtl dtl in dtls)
                {
                    if (dtl.IsEnablePass)
                        isEnablePass = true;
                }

                if (isEnablePass) /* 如果有就建立它开始节点表数据 */
                    startDtls = new BP.Sys.MapDtls("ND" + int.Parse(toND.FK_Flow) + "01");

                recDtlLog += "@进入循环开始执行逐个明细表copy.";
                int i = -1;

                foreach (Sys.MapDtl dtl in dtls)
                {
                    recDtlLog += "@进入循环开始执行明细表(" + dtl.No + ")copy.";

                    //如果当前的明细表，不需要copy.
                    if (dtl.IsCopyNDData == false)
                        continue;

                    //i++;
                    //if (toDtls.Count <= i)
                    //    continue;
                    //Sys.MapDtl toDtl = (Sys.MapDtl)toDtls[i];

                    i++;
                    //if (toDtls.Count <= i)
                    //    continue;
                    Sys.MapDtl toDtl = null;
                    foreach (MapDtl todtl in toDtls)
                    {
                        if (todtl.PTable == dtl.PTable)
                            continue;

                        string toDtlName = "";
                        string dtlName = "";
                        try
                        {
                            toDtlName = todtl.HisGEDtl.FK_MapDtl.Substring(todtl.HisGEDtl.FK_MapDtl.IndexOf("Dtl"), todtl.HisGEDtl.FK_MapDtl.Length - todtl.HisGEDtl.FK_MapDtl.IndexOf("Dtl"));
                            dtlName = dtl.HisGEDtl.FK_MapDtl.Substring(dtl.HisGEDtl.FK_MapDtl.IndexOf("Dtl"), dtl.HisGEDtl.FK_MapDtl.Length - dtl.HisGEDtl.FK_MapDtl.IndexOf("Dtl"));
                        }
                        catch
                        {
                            continue;
                        }

                        if (toDtlName == dtlName)
                        {
                            toDtl = todtl;
                            break;
                        }
                    }

                    if (dtl.IsEnablePass == true)
                    {
                        /*如果启用了是否明细表的审核通过机制,就允许copy节点数据。*/
                        toDtl.IsCopyNDData = true;
                    }

                    if (toDtl == null || toDtl.IsCopyNDData == false)
                        continue;

                    if (dtl.PTable == toDtl.PTable)
                        continue;


                    //获取明细数据。
                    GEDtls gedtls = new GEDtls(dtl.No);
                    QueryObject qo = null;
                    qo = new QueryObject(gedtls);
                    switch (dtl.DtlOpenType)
                    {
                        case DtlOpenType.ForEmp:
                            qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                            break;
                        case DtlOpenType.ForWorkID:
                            qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                            break;
                        case DtlOpenType.ForFID:
                            qo.AddWhere(GEDtlAttr.FID, this.WorkID);
                            break;
                    }
                    qo.DoQuery();

                    recDtlLog += "@查询出来从明细表:" + dtl.No + ",明细数据:" + gedtls.Count + "条.";

                    int unPass = 0;
                    // 是否启用审核机制。
                    isEnablePass = dtl.IsEnablePass;
                    if (isEnablePass && this.HisNode.IsStartNode == false)
                        isEnablePass = true;
                    else
                        isEnablePass = false;

                    if (isEnablePass == true)
                    {
                        /*判断当前节点该明细表上是否有，isPass 审核字段，如果没有抛出异常信息。*/
                        if (gedtls.Count != 0)
                        {
                            GEDtl dtl1 = gedtls[0] as GEDtl;
                            if (dtl1.EnMap.Attrs.Contains("IsPass") == false)
                                isEnablePass = false;
                        }
                    }

                    recDtlLog += "@删除到达明细表:" + dtl.No + ",数据, 并开始遍历明细表,执行一行行的copy.";

                    if (BP.DA.DBAccess.IsExitsObject(toDtl.PTable))
                        DBAccess.RunSQL("DELETE FROM " + toDtl.PTable + " WHERE RefPK=" + dbStr + "RefPK", "RefPK", this.WorkID.ToString());

                    // copy数量.
                    int deBugNumCopy = 0;
                    foreach (GEDtl gedtl in gedtls)
                    {
                        if (isEnablePass)
                        {
                            if (gedtl.GetValBooleanByKey("IsPass") == false)
                            {
                                /*没有审核通过的就 continue 它们，仅复制已经审批通过的.*/
                                continue;
                            }
                        }

                        BP.Sys.GEDtl dtCopy = new GEDtl(toDtl.No);
                        dtCopy.Copy(gedtl);
                        dtCopy.FK_MapDtl = toDtl.No;
                        dtCopy.RefPK = this.WorkID.ToString();
                        dtCopy.InsertAsOID(dtCopy.OID);
                        dtCopy.RefPKInt64 = this.WorkID;
                        deBugNumCopy++;

                        #region  复制明细表单条 - 附件信息
                        if (toDtl.IsEnableAthM)
                        {
                            /*如果启用了多附件,就复制这条明细数据的附件信息。*/
                            FrmAttachmentDBs athDBs = new FrmAttachmentDBs(dtl.No, gedtl.OID.ToString());
                            if (athDBs.Count > 0)
                            {
                                i = 0;
                                foreach (FrmAttachmentDB athDB in athDBs)
                                {
                                    i++;
                                    FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                                    athDB_N.Copy(athDB);
                                    athDB_N.FK_MapData = toDtl.No;
                                    athDB_N.MyPK = athDB.MyPK + "_" + dtCopy.OID + "_" + i.ToString();
                                    athDB_N.FK_FrmAttachment = athDB_N.FK_FrmAttachment.Replace("ND" + this.HisNode.NodeID,
                                        "ND" + toND.NodeID);
                                    athDB_N.RefPKVal = dtCopy.OID.ToString();
                                    try
                                    {
                                        athDB_N.DirectInsert();
                                    }
                                    catch
                                    {
                                        athDB_N.DirectUpdate();
                                    }

                                }
                            }
                        }
                        #endregion  复制明细表单条 - 附件信息

                    }
#warning 记录日志.
                    if (gedtls.Count != deBugNumCopy)
                    {
                        recDtlLog += "@从明细表:" + dtl.No + ",明细数据:" + gedtls.Count + "条.";
                        //记录日志.
                        Log.DefaultLogWriteLineInfo(recDtlLog);
                        throw new Exception("@系统出现错误，请将如下信息反馈给管理员,谢谢。: 技术信息:" + recDtlLog);
                    }

                    #region 如果启用了审核机制
                    if (isEnablePass)
                    {
                        /* 如果启用了审核通过机制，就把未审核的数据copy到第一个节点上去 
                         * 1, 找到对应的明细点.
                         * 2, 把未审核通过的数据复制到开始明细表里.
                         */
                        string fk_mapdata = "ND" + int.Parse(toND.FK_Flow) + "01";
                        MapData md = new MapData(fk_mapdata);
                        string startUser = "SELECT Rec FROM " + md.PTable + " WHERE OID=" + this.WorkID;
                        startUser = DBAccess.RunSQLReturnString(startUser);

                        MapDtl startDtl = (MapDtl)startDtls[i];
                        foreach (GEDtl gedtl in gedtls)
                        {
                            if (gedtl.GetValBooleanByKey("IsPass"))
                                continue; /* 排除审核通过的 */

                            BP.Sys.GEDtl dtCopy = new GEDtl(startDtl.No);
                            dtCopy.Copy(gedtl);
                            dtCopy.OID = 0;
                            dtCopy.FK_MapDtl = startDtl.No;
                            dtCopy.RefPK = gedtl.OID.ToString(); //this.WorkID.ToString();
                            dtCopy.SetValByKey("BatchID", this.WorkID);
                            dtCopy.SetValByKey("IsPass", 0);
                            dtCopy.SetValByKey("Rec", startUser);
                            dtCopy.SetValByKey("Checker", this.ExecerName);
                            dtCopy.RefPKInt64 = this.WorkID;
                            dtCopy.SaveAsOID(gedtl.OID);
                        }
                        DBAccess.RunSQL("UPDATE " + startDtl.PTable + " SET Rec='" + startUser + "',Checker='" + this.Execer + "' WHERE BatchID=" + this.WorkID + " AND Rec='" + this.Execer + "'");
                    }
                    #endregion 如果启用了审核机制
                }
            }
            #endregion 复制明细数据
        }
        #endregion

        #region 返回对象处理.
        private SendReturnObjs HisMsgObjs = null;
        public void addMsg(string flag, string msg)
        {
            addMsg(flag, msg, null, SendReturnMsgType.Info);
        }
        public void addMsg(string flag, string msg, SendReturnMsgType msgType)
        {
            addMsg(flag, msg, null, msgType);
        }
        public void addMsg(string flag, string msg, string msgofHtml, SendReturnMsgType msgType)
        {
            if (HisMsgObjs == null)
                HisMsgObjs = new SendReturnObjs();
            this.HisMsgObjs.AddMsg(flag, msg, msgofHtml, msgType);
        }
        public void addMsg(string flag, string msg, string msgofHtml)
        {
            addMsg(flag, msg, msgofHtml, SendReturnMsgType.Info);
        }
        #endregion 返回对象处理.

        #region 方法
        /// <summary>
        /// 发送失败是撤消数据。
        /// </summary>
        public void DealEvalUn()
        {

            //数据发送。
            BP.WF.Data.Eval eval = new Eval();
            if (this.HisNode.IsFLHL == false)
            {
                eval.MyPK = this.WorkID + "_" + this.HisNode.NodeID;
                eval.Delete();
            }

            // 分合流的情况，它是明细表产生的质量评价。
            MapDtls dtls = this.HisNode.MapData.MapDtls;
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsHLDtl == false)
                    continue;

                //获取明细数据。
                GEDtls gedtls = new GEDtls(dtl.No);
                QueryObject qo = null;
                qo = new QueryObject(gedtls);
                switch (dtl.DtlOpenType)
                {
                    case DtlOpenType.ForEmp:
                        qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                        break;
                    case DtlOpenType.ForWorkID:
                        qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                        break;
                    case DtlOpenType.ForFID:
                        qo.AddWhere(GEDtlAttr.FID, this.WorkID);
                        break;
                }
                qo.DoQuery();

                foreach (GEDtl gedtl in gedtls)
                {
                    eval = new Eval();
                    eval.MyPK = gedtl.OID + "_" + gedtl.Rec;
                    eval.Delete();
                }
            }
        }
        /// <summary>
        /// 处理质量考核
        /// </summary>
        public void DealEval()
        {
            if (this.HisNode.IsEval == false)
                return;

            BP.WF.Data.Eval eval = new Eval();
            eval.CheckPhysicsTable();

            if (this.HisNode.IsFLHL == false)
            {
                eval.MyPK = this.WorkID + "_" + this.HisNode.NodeID;
                eval.Delete();

                eval.Title = this.HisGenerWorkFlow.Title;

                eval.WorkID = this.WorkID;
                eval.FK_Node = this.HisNode.NodeID;
                eval.NodeName = this.HisNode.Name;

                eval.FK_Flow = this.HisNode.FK_Flow;
                eval.FlowName = this.HisNode.FlowName;

                eval.FK_Dept = this.ExecerDeptNo;
                eval.DeptName = this.ExecerDeptName;

                eval.Rec = this.Execer;
                eval.RecName = this.ExecerName;

                eval.RDT = DataType.CurrentDataTime;
                eval.FK_NY = DataType.CurrentYearMonth;

                eval.EvalEmpNo = this.HisWork.GetValStringByKey(WorkSysFieldAttr.EvalEmpNo);
                eval.EvalEmpName = this.HisWork.GetValStringByKey(WorkSysFieldAttr.EvalEmpName);
                eval.EvalCent = this.HisWork.GetValStringByKey(WorkSysFieldAttr.EvalCent);
                eval.EvalNote = this.HisWork.GetValStringByKey(WorkSysFieldAttr.EvalNote);

                eval.Insert();
                return;
            }

            // 分合流的情况，它是明细表产生的质量评价。
            Sys.MapDtls dtls = this.HisNode.MapData.MapDtls;
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsHLDtl == false)
                    continue;

                //获取明细数据。
                GEDtls gedtls = new GEDtls(dtl.No);
                QueryObject qo = null;
                qo = new QueryObject(gedtls);
                switch (dtl.DtlOpenType)
                {
                    case DtlOpenType.ForEmp:
                        qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                        break;
                    case DtlOpenType.ForWorkID:
                        qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                        break;
                    case DtlOpenType.ForFID:
                        qo.AddWhere(GEDtlAttr.FID, this.WorkID);
                        break;
                }
                qo.DoQuery();

                foreach (GEDtl gedtl in gedtls)
                {
                    eval = new Eval();
                    eval.MyPK = gedtl.OID + "_" + gedtl.Rec;
                    eval.Delete();

                    eval.Title = this.HisGenerWorkFlow.Title;

                    eval.WorkID = this.WorkID;
                    eval.FK_Node = this.HisNode.NodeID;
                    eval.NodeName = this.HisNode.Name;

                    eval.FK_Flow = this.HisNode.FK_Flow;
                    eval.FlowName = this.HisNode.FlowName;

                    eval.FK_Dept = this.ExecerDeptNo;
                    eval.DeptName = this.ExecerDeptName;

                    eval.Rec = this.Execer;
                    eval.RecName = this.ExecerName;

                    eval.RDT = DataType.CurrentDataTime;
                    eval.FK_NY = DataType.CurrentYearMonth;

                    eval.EvalEmpNo = gedtl.GetValStringByKey(WorkSysFieldAttr.EvalEmpNo);
                    eval.EvalEmpName = gedtl.GetValStringByKey(WorkSysFieldAttr.EvalEmpName);
                    eval.EvalCent = gedtl.GetValStringByKey(WorkSysFieldAttr.EvalCent);
                    eval.EvalNote = gedtl.GetValStringByKey(WorkSysFieldAttr.EvalNote);
                    eval.Insert();
                }
            }
        }
        private void CallSubFlow()
        {
            // 获取配置信息.
            string[] paras = this.HisNode.SubFlowStartParas.Split('@');
            foreach (string item in paras)
            {
                if (DataType.IsNullOrEmpty(item))
                    continue;

                string[] keyvals = item.Split(';');

                string FlowNo = ""; //流程编号
                string EmpField = ""; // 人员字段.
                string DtlTable = ""; //明细表.
                foreach (string keyval in keyvals)
                {
                    if (DataType.IsNullOrEmpty(keyval))
                        continue;

                    string[] strs = keyval.Split('=');
                    switch (strs[0])
                    {
                        case "FlowNo":
                            FlowNo = strs[1];
                            break;
                        case "EmpField":
                            EmpField = strs[1];
                            break;
                        case "DtlTable":
                            DtlTable = strs[1];
                            break;
                        default:
                            throw new Exception("@流程设计错误,获取流程属性配置的发起参数时，未指明的标记: " + strs[0]);
                    }

                    if (this.HisNode.SubFlowStartWay == SubFlowStartWay.BySheetField)
                    {
                        string emps = this.HisWork.GetValStringByKey(EmpField) + ",";
                        string[] empStrs = emps.Split(',');

                        string currUser = this.Execer;
                        Emps empEns = new Emps();
                        string msgInfo = "";
                        foreach (string emp in empStrs)
                        {
                            if (DataType.IsNullOrEmpty(emp))
                                continue;

                            //以当前人员的身份登录.
                            Emp empEn = new Emp(emp);
                            BP.Web.WebUser.SignInOfGener(empEn);

                            // 把数据复制给它.
                            Flow fl = new Flow(FlowNo);
                            Work sw = fl.NewWork();

                            Int64 workID = sw.OID;
                            sw.Copy(this.HisWork);
                            sw.OID = workID;
                            sw.Update();

                            WorkNode wn = new WorkNode(sw, new Node(int.Parse(FlowNo + "01")));
                            wn.NodeSend(null, this.Execer);
                            msgInfo += BP.WF.Dev2Interface.Node_StartWork(FlowNo, null, null, 0, emp, this.WorkID, FlowNo);
                        }
                    }

                }
            }


            //BP.WF.Dev2Interface.Flow_NewStartWork(
            DataTable dt;

        }
        #endregion


        /// <summary>
        /// 工作流发送业务处理
        /// </summary>
        public SendReturnObjs NodeSend()
        {
            return NodeSend(null, null);
        }

        /// <summary>
        /// 1变N,用于分流节点，向子线程copy数据。
        /// </summary>
        /// <returns></returns>
        public void CheckFrm1ToN()
        {
            //只有分流，合流才能执行1ToN.
            if (this.HisNode.HisRunModel == RunModel.Ordinary
                || this.HisNode.HisRunModel == RunModel.HL
                || this.HisNode.HisRunModel == RunModel.SubThread)
            {
                return;
            }

            //初始化变量.
            if (frmNDs == null)
                frmNDs = new FrmNodes(this.HisNode.FK_Flow, this.HisNode.NodeID);

            foreach (FrmNode fn in frmNDs)
            {
                if (fn.Is1ToN == false)
                    continue;

                #region 获得实体主键.
                // 处理主键.
                long pk = 0;// this.WorkID;
                switch (fn.WhoIsPK)
                {
                    case WhoIsPK.FID:
                        pk = this.HisWork.FID;
                        break;
                    case WhoIsPK.OID:
                        pk = this.HisWork.OID;
                        break;
                    case WhoIsPK.PWorkID:
                        pk = this.rptGe.PWorkID;
                        break;
                    default:
                        throw new Exception("@未判断的类型.");
                }

                if (pk == 0)
                    throw new Exception("@未能获取表单主键.");
                #endregion 获得实体主键.


                //初始化这个实体.
                GEEntity geEn = new GEEntity(fn.FK_Frm, pk);

                //首先删除垃圾数据.
                geEn.Delete("FID", this.WorkID);

                //循环子线程，然后插入数据.
                foreach (GenerWorkerList item in current_gwls)
                {
                    geEn.PKVal = item.WorkID; //子线程的WorkID作为.
                    geEn.SetValByKey("FID", this.WorkID);

                    #region 处理默认变量.
                    //foreach (Attr attr in geEn.EnMap.Attrs)
                    //{
                    //    if (attr.DefaultValOfReal == "@RDT")
                    //    {
                    //        geEn.SetValByKey(attr.Key, BP.DA.DataType.CurrentDataTime);
                    //        continue;
                    //    }

                    //    if (attr.DefaultValOfReal == "@WebUser.No")
                    //    {
                    //        geEn.SetValByKey(attr.Key, item.FK_Emp);
                    //        continue;
                    //    }

                    //    if (attr.DefaultValOfReal == "@WebUser.Name")
                    //    {
                    //        geEn.SetValByKey(attr.Key, item.FK_EmpText);
                    //        continue;
                    //    }

                    //    if (attr.DefaultValOfReal == "@WebUser.FK_Dept")
                    //    {
                    //        Emp emp = new Emp(item.FK_Emp);
                    //        geEn.SetValByKey(attr.Key, emp.FK_Dept);
                    //        continue;
                    //    }

                    //    if (attr.DefaultValOfReal == "@WebUser.FK_DeptName")
                    //    {
                    //        Emp emp = new Emp(item.FK_Emp);
                    //        geEn.SetValByKey(attr.Key, emp.FK_DeptText);
                    //        continue;
                    //    }
                    //}
                    #endregion 处理默认变量.

                    geEn.DirectInsert();
                }
            }
        }
        /// <summary>
        /// 当前节点的-表单绑定.
        /// </summary>
        private BP.WF.Template.FrmNodes frmNDs = null;
        /// <summary>
        /// 汇总子线程的表单到合流节点上去
        /// </summary>
        /// <returns></returns>
        public void CheckFrmHuiZongToDtl()
        {
            //只有分流，合流才能执行1ToN.
            if (this.HisNode.HisRunModel != RunModel.SubThread)
                return;

            //初始化变量.
            if (frmNDs == null)
                frmNDs = new FrmNodes(this.HisNode.FK_Flow, this.HisNode.NodeID);

            foreach (FrmNode fn in frmNDs)
            {
                //如果该表单不需要汇总，就不处理他.
                if (fn.HuiZong == "0" || fn.HuiZong == "")
                    continue;

                #region 获得实体主键.
                // 处理主键.
                long pk = 0;// this.WorkID;
                switch (fn.WhoIsPK)
                {
                    case WhoIsPK.FID:
                        pk = this.HisWork.FID;
                        break;
                    case WhoIsPK.OID:
                        pk = this.HisWork.OID;
                        break;
                    case WhoIsPK.PWorkID:
                        pk = this.rptGe.PWorkID;
                        break;
                    default:
                        throw new Exception("@未判断的类型.");
                }

                if (pk == 0)
                    throw new Exception("@未能获取表单主键.");
                #endregion 获得实体主键.


                //初始化这个实体,获得这个实体的数据.
                GEEntity rpt = new GEEntity(fn.FK_Frm, pk);
                //
                string[] strs = fn.HuiZong.Trim().Split('@');

                //实例化这个数据.
                MapDtl dtl = new MapDtl(strs[1].ToString());

                //把数据汇总到指定的表里.
                GEDtl dtlEn = dtl.HisGEDtl;
                dtlEn.OID = (int)this.WorkID;
                int i = dtlEn.RetrieveFromDBSources();
                dtlEn.Copy(rpt);

                dtlEn.OID = (int)this.WorkID;
                dtlEn.RDT = BP.DA.DataType.CurrentDataTime;
                dtlEn.Rec = BP.Web.WebUser.No;

                dtlEn.RefPK = this.HisWork.FID.ToString();
                dtlEn.FID = 0;

                if (i == 0)
                    dtlEn.SaveAsOID((int)this.WorkID);
                else
                    dtlEn.Update();
            }
        }
        /// <summary>
        /// 检查是否填写审核意见
        /// </summary>
        /// <returns></returns>
        private bool CheckFrmIsFullCheckNote()
        {
            //检查是否写入了审核意见.
            if (this.HisNode.FrmWorkCheckSta == FrmWorkCheckSta.Enable)
            {
                /*检查审核意见 */
                string sql = "SELECT Msg,EmpToT FROM ND" + int.Parse(this.HisNode.FK_Flow) + "Track WHERE  EmpFrom='" + WebUser.No + "' AND NDFrom=" + this.HisNode.NodeID + " AND WorkID=" + this.WorkID + " AND ActionType=" + (int)ActionType.WorkCheck;
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Exception("err@请填写审核意见.");

                if (dt.Rows[0][0].ToString() == "")
                    throw new Exception("err@审核意见不能为空.");
            }
            return true;
        }
        /// <summary>
        /// 检查独立表单上必须填写的项目.
        /// </summary>
        /// <returns></returns>
        public bool CheckFrmIsNotNull()
        {
            //if (this.HisNode.HisFormType != NodeFormType.SheetTree)
            //    return true;




            //增加节点表单的必填项判断.
            string err = "";
            if (this.HisNode.HisFormType == NodeFormType.FreeForm || this.HisNode.HisFormType == NodeFormType.FoolForm)
            {
                MapAttrs attrs = this.HisNode.MapData.MapAttrs;
                Row row = this.HisWork.Row;
                foreach (MapAttr attr in attrs)
                {
                    if (attr.UIIsInput == false)
                        continue;

                    string str = row[attr.KeyOfEn] == null ? string.Empty : row[attr.KeyOfEn].ToString();
                    /*如果是检查不能为空 */
                    if (str == null || DataType.IsNullOrEmpty(str) == true || str.Trim() == "")
                        err += "@字段{" + attr.KeyOfEn + " ; " + attr.Name + "}，不能为空。";
                }

                #region 检查附件个数的完整性. - 该部分代码稳定后，移动到独立表单的检查上去。
                foreach (FrmAttachment ath in this.HisWork.HisFrmAttachments)
                {
                    if (ath.UploadFileNumCheck == UploadFileNumCheck.None)
                        continue;

                    if (ath.UploadFileNumCheck == UploadFileNumCheck.NotEmpty)
                    {
                        Paras ps = new Paras();
                        ps.SQL = "SELECT COUNT(MyPK) as Num FROM Sys_FrmAttachmentDB WHERE FK_MapData=" + SystemConfig.AppCenterDBVarStr + "FK_MapData AND FK_FrmAttachment=" + SystemConfig.AppCenterDBVarStr + "FK_FrmAttachment AND RefPKVal=" + SystemConfig.AppCenterDBVarStr + "RefPKVal";
                        ps.Add("FK_MapData", "ND" + this.HisNode.NodeID);
                        ps.Add("FK_FrmAttachment", ath.MyPK);
                        ps.Add("RefPKVal", this.WorkID);
                        if (DBAccess.RunSQLReturnValInt(ps) == 0)
                            err += "@您没有上传附件:" + ath.Name;
                    }

                    if (ath.UploadFileNumCheck == UploadFileNumCheck.EverySortNoteEmpty)
                    {
                        Paras ps = new Paras();
                        ps.SQL = "SELECT COUNT(MyPK) as Num, MyNote FROM Sys_FrmAttachmentDB WHERE FK_MapData=" + SystemConfig.AppCenterDBVarStr + "FK_MapData AND FK_FrmAttachment=" + SystemConfig.AppCenterDBVarStr + "FK_FrmAttachment AND RefPKVal=" + SystemConfig.AppCenterDBVarStr + "RefPKVal Group BY MyNote";
                        ps.Add("FK_MapData", "ND" + this.HisNode.NodeID);
                        ps.Add("FK_FrmAttachment", ath.MyPK);
                        ps.Add("RefPKVal", this.WorkID);

                        DataTable dt = DBAccess.RunSQLReturnTable(ps);
                        if (dt.Rows.Count == 0)
                            err += "@您没有上传附件:" + ath.Name + "，请为每个类别下上传附件。";

                        string sort = ath.Sort.Replace(";", ",");
                        string[] strs = sort.Split(',');
                        foreach (string str in strs)
                        {
                            bool isHave = false;
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["MyNote"].ToString() == str)
                                {
                                    isHave = true;
                                    break;
                                }
                            }
                            if (isHave == false)
                                err += "@你需要上传(" + str + ")";
                        }
                    }
                }
                #endregion 检查附件个数的完整性.

                #region 检查图片附件的必填，added by liuxc,2016-11-1
                foreach (FrmImgAth imgAth in this.HisNode.MapData.FrmImgAths)
                {
                    if (!imgAth.IsRequired)
                        continue;

                    Paras ps = new Paras();
                    ps.SQL = "SELECT COUNT(MyPK) as Num FROM Sys_FrmImgAthDB WHERE FK_MapData=" + SystemConfig.AppCenterDBVarStr + "FK_MapData AND FK_FrmImgAth=" + SystemConfig.AppCenterDBVarStr + "FK_FrmImgAth AND RefPKVal=" + SystemConfig.AppCenterDBVarStr + "RefPKVal";
                    ps.Add("FK_MapData", "ND" + this.HisNode.NodeID);
                    ps.Add("FK_FrmImgAth", imgAth.MyPK);
                    ps.Add("RefPKVal", this.WorkID);
                    if (DBAccess.RunSQLReturnValInt(ps) == 0)
                        err += "@您没有上传图片附件:" + imgAth.CtrlID;
                }
                #endregion 检查图片附件的必填，added by liuxc,2016-11-1

                if (err != "")
                    throw new Exception("err@在提交前检查到如下必输字段填写不完整:" + err);


                CheckFrmIsFullCheckNote();
            }

            //查询出来所有的设置。
            FrmFields ffs = new FrmFields();

            QueryObject qo = new QueryObject(ffs);
            qo.AddWhere(FrmFieldAttr.FK_Node, this.HisNode.NodeID);
            qo.addAnd();
            qo.addLeftBracket();
            qo.AddWhere(FrmFieldAttr.IsNotNull, 1);
            qo.addOr();
            qo.AddWhere(FrmFieldAttr.IsWriteToFlowTable, 1);
            qo.addRightBracket();
            qo.DoQuery();

            if (ffs.Count == 0)
                return true;

            BP.WF.Template.FrmNodes frmNDs = new FrmNodes(this.HisNode.FK_Flow, this.HisNode.NodeID);
            err = "";
            foreach (FrmNode item in frmNDs)
            {
                MapData md = new MapData(item.FK_Frm);

                //可能是url.
                if (md.HisFrmType == FrmType.Url)
                    continue;

                //如果使用默认方案,就return出去.
                if (item.FrmSln == 0)
                    continue;

                //检查是否有？
                bool isHave = false;
                foreach (FrmField myff in ffs)
                {
                    if (myff.FK_MapData != item.FK_Frm)
                        continue;
                    isHave = true;
                    break;
                }
                if (isHave == false)
                    continue;

                // 处理主键.
                long pk = 0;// this.WorkID;

                switch (item.WhoIsPK)
                {
                    case WhoIsPK.FID:
                        pk = this.HisWork.FID;
                        break;
                    case WhoIsPK.OID:
                        pk = this.HisWork.OID;
                        break;
                    case WhoIsPK.PWorkID:
                        pk = this.rptGe.PWorkID;
                        break;
                    default:
                        throw new Exception("@未判断的类型.");
                }

                if (pk == 0)
                    throw new Exception("@未能获取表单主键.");

                //获取表单值
                ps = new Paras();
                ps.SQL = "SELECT * FROM " + md.PTable + " WHERE OID=" + ps.DBStr + "OID";
                ps.Add(WorkAttr.OID, pk);
                DataTable dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                {
                    err += "@表单{" + md.Name + "}没有输入数据。";
                    continue;
                }

                // 检查数据是否完整.
                foreach (FrmField ff in ffs)
                {
                    if (ff.FK_MapData != item.FK_Frm)
                        continue;

                    //获得数据.
                    string val = string.Empty;
                    val = dt.Rows[0][ff.KeyOfEn].ToString();

                    if (ff.IsNotNull == true)
                    {
                        /*如果是检查不能为空 */
                        if (DataType.IsNullOrEmpty(val) == true || val.Trim() == "")
                            err += "@表单{" + md.Name + "}字段{" + ff.KeyOfEn + " ; " + ff.Name + "}，不能为空。";
                    }

                    //判断是否需要写入流程数据表.
                    if (ff.IsWriteToFlowTable == true)
                    {
                        this.HisWork.SetValByKey(ff.KeyOfEn, val);
                        //this.rptGe.SetValByKey(ff.KeyOfEn, val);
                    }
                }
            }
            if (err != "")
                throw new Exception("@在提交前检查到如下必输字段填写不完整:" + err);

            return true;
        }
        /// <summary>
        /// copy表单树的数据
        /// </summary>
        /// <returns></returns>
        public Work CopySheetTree()
        {
            if (this.HisNode.HisFormType != NodeFormType.SheetTree)
                return null;

            //查询出来所有的设置。
            FrmFields ffs = new FrmFields();

            

            QueryObject qo = new QueryObject(ffs);
            qo.AddWhere(FrmFieldAttr.FK_Node, this.HisNode.NodeID);
            qo.DoQuery();


            BP.WF.Template.FrmNodes frmNDs = new FrmNodes(this.HisNode.FK_Flow, this.HisNode.NodeID);
            string err = "";
            foreach (FrmNode item in frmNDs)
            {
                MapData md = new MapData(item.FK_Frm);

                //可能是url.
                if (md.HisFrmType == FrmType.Url)
                    continue;

                //检查是否有？
                bool isHave = false;
                foreach (FrmField myff in ffs)
                {
                    if (myff.FK_MapData != item.FK_Frm)
                        continue;
                    isHave = true;
                    break;
                }

                if (isHave == false)
                    continue;

                MapAttrs attrs = new MapAttrs(item.FK_Frm);
                string attrstr = "";
                foreach (MapAttr attr in attrs)
                {
                    attrstr += attr.KeyOfEn + ",";
                }

                // 处理主键.
                long pk = 0;// this.WorkID;

                switch (item.WhoIsPK)
                {
                    case WhoIsPK.FID:
                        pk = this.HisWork.FID;
                        break;
                    case WhoIsPK.OID:
                        pk = this.HisWork.OID;
                        break;
                    case WhoIsPK.PWorkID:
                        pk = this.rptGe.PWorkID;
                        break;
                    default:
                        throw new Exception("@未判断的类型.");
                }

                if (pk == 0)
                    throw new Exception("@未能获取表单主键.");

                //获取表单值
                ps = new Paras();
                ps.SQL = "SELECT * FROM " + md.PTable + " WHERE OID=" + ps.DBStr + "OID";
                ps.Add(WorkAttr.OID, pk);
                DataTable dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                    continue;

                // 检查数据是否完整.
                foreach (FrmField ff in ffs)
                {
                    if (ff.FK_MapData != item.FK_Frm)
                        continue;

                        if (!attrstr.Contains(ff.KeyOfEn))
                            continue;

                        //获得数据.
                        string val = string.Empty;
                        val = dt.Rows[0][ff.KeyOfEn].ToString();

                    this.HisWork.SetValByKey(ff.KeyOfEn, val);
                }
            }

            return this.HisWork;
        }
        /// <summary>
        /// 执行抄送
        /// </summary>
        public void DoCC()
        {
        }
        /// <summary>
        /// 通知主持人 @整体需要重新翻译.
        /// </summary>
        /// <returns></returns>
        private string DealAlertZhuChiRen()
        {

            /*有两个待办，就说明当前人员是最后一个会签人，就要把主持人的状态设置为 0 */
            //获得主持人信息.
            GenerWorkerList gwl = new GenerWorkerList();
            int i = gwl.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.IsPass, 90);
            if (i != 1)
                return "@您已经会签完毕.";

            gwl.IsPassInt = 0; //从会签列表里移动到待办.
            gwl.IsRead = false; //设置为未读.

            BP.WF.Dev2Interface.Port_SendMsg(gwl.FK_Emp,
                "工作会签完毕", this.HisGenerWorkFlow.Title + " 工作会签完毕,请到待办查看.",
                "HuiQian" + this.WorkID + "_" + WebUser.No, "HuiQian", HisGenerWorkFlow.FK_Flow, this.HisGenerWorkFlow.FK_Node, this.WorkID, 0);

            //设置为未读.
            BP.WF.Dev2Interface.Node_SetWorkUnRead(this.HisGenerWorkFlow.WorkID);


            //设置最后处理人.
            this.HisGenerWorkFlow.TodoEmps = gwl.FK_Emp + "," + gwl.FK_EmpText + ";";
            this.HisGenerWorkFlow.Update();

            #region 处理天业集团对主持人的考核.
            /*
             * 对于会签人的时间计算
             * 1, 从主持人接受工作时间点起，到最后一个一次分配会签人止，作为第一时间段。
             * 2，所有会签人会签完毕后到会签人执行发送时间点止作为第2个时间段。
             * 3，第1个时间端+第2个时间段为主持人所处理该工作的时间，时效考核的内容按照这个两个时间段开始计算。
             */
            if (this.HisNode.HisCHWay == CHWay.ByTime)
            {
                /*如果是按照时效考核.*/

                //获得最后一次执行会签的时间点.
                string sql = "SELECT RDT FROM ND" + int.Parse(this.HisNode.FK_Flow) + "TRACK WHERE WorkID=" + this.WorkID + " AND ActionType=30 ORDER BY RDT";
                string lastDTOfHuiQian = DBAccess.RunSQLReturnStringIsNull(sql, null);

                //取出来下达给主持人的时间点.
                string dtOfToZhuChiRen = gwl.RDT;

                //获得两个时间间隔.
                DateTime t_lastDTOfHuiQian = DataType.ParseSysDate2DateTime(lastDTOfHuiQian);
                DateTime t_dtOfToZhuChiRen = DataType.ParseSysDate2DateTime(dtOfToZhuChiRen);

                TimeSpan ts = t_lastDTOfHuiQian - t_dtOfToZhuChiRen;

                //生成该节点设定的 时间范围.
                int hour = this.HisNode.TimeLimit * 24 + this.HisNode.TimeLimitHH;
                // int.Parse(this.HisNode.TSpanHour.ToString());
                TimeSpan tsLimt = new TimeSpan(hour, this.HisNode.TimeLimitMM, 0);

                //获得剩余的时间范围.
                TimeSpan myLeftTS = tsLimt - ts;

                //计算应该完成的日期.
                DateTime dtNow = DateTime.Now;
                dtNow = dtNow.AddHours(myLeftTS.TotalHours);

                //设置应该按成的日期.
                if (this.HisNode.HisCHWay == CHWay.None)
                    gwl.SDT = "无";
                else
                    gwl.SDT = dtNow.ToString(DataType.SysDataTimeFormat);

                //设置预警日期, 为了方便提前1天预警.
                dtNow = dtNow.AddDays(-1);
                gwl.DTOfWarning = dtNow.ToString(DataType.SysDataTimeFormat);
            }
            #endregion 处理天业集团对会签人的考核.

            gwl.Update();

            return "您是最后一个会签该工作的处理人，已经提醒主持人(" + gwl.FK_Emp + "," + gwl.FK_EmpText + ")处理当前工作。";
        }
        /// <summary>
        /// 如果是协作.
        /// </summary>
        /// <returns>是否执行到最后一个人？</returns>
        public bool DealTeamUpNode()
        {
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, this.HisNode.NodeID);

            if (gwls.Count == 1)
                return false; /*让其向下执行,因为只有一个人,就没有顺序的问题.*/

            //查看是否我是最后一个？
            int num = 0;
            string todoEmps = ""; //记录没有处理的人.
            foreach (GenerWorkerList item in gwls)
            {
                if (item.IsPassInt == 0 || item.IsPassInt == 90)
                {
                    if (item.FK_Emp.Equals(WebUser.No) == false)
                        todoEmps += BP.WF.Glo.DealUserInfoShowModel(item.FK_Emp, item.FK_EmpText) + " ";
                    num++;
                }
            }

            if (num == 1)
            {
                if (this.HisGenerWorkFlow.HuiQianTaskSta == HuiQianTaskSta.None)
                {
                    this.HisGenerWorkFlow.Sender = BP.WF.Glo.DealUserInfoShowModel(BP.Web.WebUser.No, BP.Web.WebUser.Name);
                    this.HisGenerWorkFlow.TodoEmpsNum = 1;
                    this.HisGenerWorkFlow.TodoEmps = WebUser.Name + ";";
                }
                else
                {
                    string huiqianNo = this.HisGenerWorkFlow.HuiQianZhuChiRen;
                    string huiqianName = this.HisGenerWorkFlow.HuiQianZhuChiRenName;

                    this.HisGenerWorkFlow.Sender = BP.WF.Glo.DealUserInfoShowModel(huiqianNo, huiqianName);
                    this.HisGenerWorkFlow.TodoEmpsNum = 1;
                    this.HisGenerWorkFlow.TodoEmps = WebUser.Name + ";";


                }
                return false; /*只有一个待办,说明自己就是最后的一个人.*/
            }

            //把当前的待办设置已办，并且提示未处理的人。
            foreach (GenerWorkerList gwl in gwls)
            {
                if (gwl.FK_Emp.Equals(WebUser.No) == false)
                    continue;

                //设置当前不可以用.
                gwl.IsPassInt = 1;
                gwl.Update();

                // 检查完成条件。
                if (this.HisNode.IsEndNode == false)
                    this.CheckCompleteCondition();

                //写入日志.
                if (this.HisGenerWorkFlow.HuiQianTaskSta != HuiQianTaskSta.None)
                    this.AddToTrack(ActionType.TeampUp, gwl.FK_Emp, todoEmps, this.HisNode.NodeID, this.HisNode.Name, "会签");
                else
                    this.AddToTrack(ActionType.TeampUp, gwl.FK_Emp, todoEmps, this.HisNode.NodeID, this.HisNode.Name, "协作发送");

                //替换人员信息.
                string emps = this.HisGenerWorkFlow.TodoEmps;

                emps = emps.Replace(WebUser.No + "," + WebUser.Name + ";", "");
                emps = emps.Replace(WebUser.No + "," + WebUser.Name, "");

                this.HisGenerWorkFlow.TodoEmps = emps;

                //处理会签问题
                this.addMsg(SendReturnMsgFlag.OverCurr, "@会签工作已完成@当前工作未处理的会签人有: " + todoEmps + " .", null, SendReturnMsgType.Info);
                return true;
            }

            throw new Exception("@不应该运行到这里，DealTeamUpNode。");
        }
        /// <summary>
        /// 如果是协作
        /// </summary>
        public bool DealTeamupGroupLeader()
        {
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, this.HisNode.NodeID, GenerWorkerListAttr.IsPass);

            if (gwls.Count == 1)
                return false; /*让其向下执行,因为只有一个人,就没有顺序的问题.*/

            #region  判断自己是否是组长？如果是组长，就让返回false, 让其运动到最后一个节点，因为组长同意了，就全部同意了。
            if (this.HisNode.TeamLeaderConfirmRole == TeamLeaderConfirmRole.ByDeptFieldLeader)
            {
                string sql = "SELECT COUNT(No) AS num FROM Port_Dept WHERE Leader='" + WebUser.No + "'";
                if (BP.DA.DBAccess.RunSQLReturnValInt(sql, 0) == 1)
                    return false;
            }

            if (this.HisNode.TeamLeaderConfirmRole == TeamLeaderConfirmRole.BySQL)
            {
                string sql = this.HisNode.TeamLeaderConfirmDoc;
                sql = Glo.DealExp(sql, this.HisWork, null);
                sql = sql.Replace("~", "'");
                sql = sql.Replace("@WorkID", this.WorkID.ToString());
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                string userNo = WebUser.No;
                foreach (DataRow dr in dt.Rows)
                {
                    string str = dr[0] as string;
                    if (str == userNo)
                        return false;
                }
            }

            if (this.HisNode.TeamLeaderConfirmRole == TeamLeaderConfirmRole.HuiQianLeader)
            {
                if (this.HisGenerWorkFlow.TodoEmps.Contains(BP.Web.WebUser.No + "," + BP.Web.WebUser.Name + ";") == true)
                {
                    /*当前人是组长，检查是否可以可以发送,检查自己是否是最后一个人 ？*/
                    string todoEmps = ""; //记录没有处理的人.
                    int num = 0;
                    foreach (GenerWorkerList item in gwls)
                    {
                        if (item.IsPassInt == 0 || item.IsPassInt == 90)
                        {
                            if (item.FK_Emp != WebUser.No)
                                todoEmps += BP.WF.Glo.DealUserInfoShowModel(item.FK_Emp, item.FK_EmpText) + " ";
                            num++;
                        }
                    }

                    if (num == 1)
                    {
                        this.HisGenerWorkFlow.Sender = BP.WF.Glo.DealUserInfoShowModel(BP.Web.WebUser.No, BP.Web.WebUser.Name);
                        this.HisGenerWorkFlow.HuiQianTaskSta = HuiQianTaskSta.None;
                        return false; /*只有一个待办,说明自己就是最后的一个人.*/
                    }

                    if (SystemConfig.CustomerNo == "LIMS")
                    {
                        this.HisGenerWorkFlow.Sender = BP.WF.Glo.DealUserInfoShowModel(BP.Web.WebUser.No, BP.Web.WebUser.Name);
                        this.HisGenerWorkFlow.HuiQianTaskSta = HuiQianTaskSta.None;
                        return false; /*不处理，未完成的会签人，没有执行会签的人，忽略.*/
                    }

                    this.addMsg(SendReturnMsgFlag.CondInfo, "@当前工作未处理的会签人有: " + todoEmps + ",您不能执行发送.", null, SendReturnMsgType.Info);
                    return true;
                }
            }
            #endregion

            //查看是否我是最后一个？
            int mynum = 0;
            string todoEmps1 = ""; //记录没有处理的人.
            foreach (GenerWorkerList item in gwls)
            {
                if (item.IsPassInt == 0 || item.IsPassInt == 90)
                {
                    if (item.FK_Emp != WebUser.No)
                        todoEmps1 += BP.WF.Glo.DealUserInfoShowModel(item.FK_Emp, item.FK_EmpText) + " ";
                    mynum++;
                }
            }

            if (mynum == 1)
            {
                this.HisGenerWorkFlow.Sender = BP.WF.Glo.DealUserInfoShowModel(BP.Web.WebUser.No, BP.Web.WebUser.Name);
                return false; /*只有一个待办,说明自己就是最后的一个人.*/
            }

            //把当前的待办设置已办，并且提示未处理的人。
            foreach (GenerWorkerList gwl in gwls)
            {
                if (gwl.FK_Emp != WebUser.No)
                    continue;

                //设置当前已经完成.
                gwl.IsPassInt = 1;
                gwl.Update();

                // 检查完成条件。
                if (this.HisNode.IsEndNode == false)
                    this.CheckCompleteCondition();

                //执行时效考核.
                if (this.rptGe == null)
                    Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID, this.rptGe.Title, gwl);
                else
                    Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, 0, this.HisGenerWorkFlow.Title, gwl);

                this.AddToTrack(ActionType.TeampUp, gwl.FK_Emp, todoEmps1, this.HisNode.NodeID, this.HisNode.Name, "协作发送");

                //cut 当前的人员.
                string emps = this.HisGenerWorkFlow.TodoEmps;
                emps = emps.Replace(WebUser.Name + ";", "");
                emps = emps.Replace(WebUser.Name, "");

                this.HisGenerWorkFlow.TodoEmps = emps;
                this.HisGenerWorkFlow.DirectUpdate();

                //处理会签问题，
                if (mynum == 2)
                {
                    string msg = this.DealAlertZhuChiRen();
                    this.addMsg(SendReturnMsgFlag.OverCurr, msg, null, SendReturnMsgType.Info);
                }
                else
                {
                    this.addMsg(SendReturnMsgFlag.OverCurr, "@会签工作已完成@当前工作未处理的会签人有:" + todoEmps1 + " .", null, SendReturnMsgType.Info);
                }
                return true;
            }
            throw new Exception("@不应该运行到这里。");
        }
        /// <summary>
        /// 处理队列节点
        /// </summary>
        /// <returns>是否可以向下发送?</returns>
        public bool DealOradeNode()
        {
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, this.HisNode.NodeID, GenerWorkerListAttr.IsPass);

            if (gwls.Count == 1)
                return false; /*让其向下执行,因为只有一个人。就没有顺序的问题.*/

            int idx = -100;
            foreach (GenerWorkerList gwl in gwls)
            {
                idx++;
                if (gwl.FK_Emp != WebUser.No)
                    continue;

                //设置当前不可以用.
                gwl.IsPassInt = idx;
                gwl.Update();
            }

            foreach (GenerWorkerList gwl in gwls)
            {
                if (gwl.IsPassInt > 10)
                {
                    /*就开始发到这个人身上. */
                    gwl.IsPassInt = 0;
                    gwl.Update();

                    // 检查完成条件。
                    if (this.HisNode.IsEndNode == false)
                    {
                        this.CheckCompleteCondition();
                    }
                    //写入日志.
                    this.AddToTrack(ActionType.Order, gwl.FK_Emp, gwl.FK_EmpText, this.HisNode.NodeID,
                        this.HisNode.Name, "队列发送");

                    this.addMsg(SendReturnMsgFlag.VarAcceptersID, gwl.FK_Emp, gwl.FK_Emp, SendReturnMsgType.SystemMsg);
                    this.addMsg(SendReturnMsgFlag.VarAcceptersName, gwl.FK_EmpText, gwl.FK_EmpText, SendReturnMsgType.SystemMsg);
                    this.addMsg(SendReturnMsgFlag.OverCurr, "当前工作已经发送给(" + gwl.FK_Emp + "," + gwl.FK_EmpText + ").", null, SendReturnMsgType.Info);

                    //执行更新.
                    if (this.HisGenerWorkFlow.Emps.Contains("@" + WebUser.No + "@") == false)
                        this.HisGenerWorkFlow.Emps = this.HisGenerWorkFlow.Emps + WebUser.No + "@";

                    this.rptGe.FlowEmps = this.HisGenerWorkFlow.Emps;
                    this.rptGe.WFState = WFState.Runing;

                    this.rptGe.Update(GERptAttr.FlowEmps, this.rptGe.FlowEmps, GERptAttr.WFState, (int)WFState.Runing);


                    this.HisGenerWorkFlow.WFState = WFState.Runing;
                    this.HisGenerWorkFlow.Update();
                    return true;
                }
            }

            // 如果是最后一个，就要他向下发送。
            return false;
        }
        /// <summary>
        /// 检查阻塞模式
        /// </summary>
        private void CheckBlockModel()
        {
            if (this.HisNode.BlockModel == BlockModel.None)
                return;

            string blockMsg = this.HisNode.BlockAlert;

            if (string.IsNullOrWhiteSpace(this.HisNode.BlockAlert))
                blockMsg = "@符合发送阻塞规则，不能向下发送！";

            if (this.HisNode.BlockModel == BlockModel.CurrNodeAll)
            {
                /*如果设置检查是否子流程结束.*/
                GenerWorkFlows gwls = new GenerWorkFlows();
                if (this.HisNode.HisRunModel == RunModel.SubThread)
                {
                    /*如果是子线程,仅仅检查自己子线程上发起的workid.*/
                    QueryObject qo = new QueryObject(gwls);
                    qo.AddWhere(GenerWorkFlowAttr.PWorkID, this.WorkID);
                    qo.addAnd();
                    qo.AddWhere(GenerWorkFlowAttr.PNodeID, this.HisNode.NodeID);
                    qo.addAnd();
                    qo.AddWhere(GenerWorkFlowAttr.PFlowNo, this.HisFlow.No);
                    qo.addAnd();
                    qo.AddWhere(GenerWorkFlowAttr.WFSta, (int)WFSta.Runing);
                    qo.DoQuery();
                    if (gwls.Count == 0)
                        return;
                }
                else
                {
                    /*检查，以前的子线程是否发起过流程 与以前的分子线程节点是否发起过子流程。 */
                    QueryObject qo = new QueryObject(gwls);

                    qo.addLeftBracket();
                    qo.AddWhere(GenerWorkFlowAttr.PFID, this.WorkID);
                    qo.addOr();
                    qo.AddWhere(GenerWorkFlowAttr.PWorkID, this.WorkID);
                    qo.addRightBracket();

                    qo.addAnd();

                    qo.addLeftBracket();
                    qo.AddWhere(GenerWorkFlowAttr.PNodeID, this.HisNode.NodeID);
                    qo.addAnd();
                    qo.AddWhere(GenerWorkFlowAttr.PFlowNo, this.HisFlow.No);
                    qo.addAnd();
                    qo.AddWhere(GenerWorkFlowAttr.WFSta, (int)WFSta.Runing);
                    qo.addRightBracket();

                    qo.DoQuery();
                    if (gwls.Count == 0)
                        return;
                }

                string err = "";
                err += "@如下子流程没有完成，你不能向下发送。@---------------------------------";
                foreach (GenerWorkFlow gwf in gwls)
                    err += "@流程ID=" + gwf.WorkID + ",标题:" + gwf.Title + ",当前执行人:" + gwf.TodoEmps + ",运行到节点:" + gwf.NodeName;

                err = Glo.DealExp(blockMsg, this.rptGe, null) + err;
                throw new Exception(err);
            }

            if (this.HisNode.BlockModel == BlockModel.SpecSubFlow)
            {
                /*如果按照特定的格式判断阻塞*/
                string exp = this.HisNode.BlockExp;
                if (exp.Contains("@") == false)
                    throw new Exception("@设置错误，该节点的阻塞配置格式错误，请参考帮助来解决。");

                string[] strs = exp.Split('@');
                string err = "";
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    if (str.Contains("=") == false)
                        throw new Exception("@阻塞设置的格式不正确:" + str);

                    string[] nodeFlow = str.Split('=');
                    int nodeid = int.Parse(nodeFlow[0]); //启动子流程的节点.
                    string subFlowNo = nodeFlow[1];

                    GenerWorkFlows gwls = new GenerWorkFlows();

                    if (this.HisNode.HisRunModel == RunModel.SubThread)
                    {
                        /* 如果是子线程，就不需要管，主干节点的问题。*/
                        QueryObject qo = new QueryObject(gwls);
                        qo.AddWhere(GenerWorkFlowAttr.PWorkID, this.WorkID);
                        qo.addAnd();
                        qo.AddWhere(GenerWorkFlowAttr.PNodeID, nodeid);
                        qo.addAnd();
                        qo.AddWhere(GenerWorkFlowAttr.PFlowNo, this.HisFlow.No);
                        qo.addAnd();
                        qo.AddWhere(GenerWorkFlowAttr.FK_Flow, subFlowNo);
                        qo.addAnd();
                        qo.AddWhere(GenerWorkFlowAttr.WFSta, (int)WFSta.Runing);

                        qo.DoQuery();
                        if (gwls.Count == 0)
                            continue;
                    }
                    else
                    {
                        /* 非子线程，就需要考虑，从该节点上，发起的子线程的 ，主干节点的问题。*/
                        QueryObject qo = new QueryObject(gwls);

                        qo.addLeftBracket();
                        qo.AddWhere(GenerWorkFlowAttr.PFID, this.WorkID);
                        qo.addOr();
                        qo.AddWhere(GenerWorkFlowAttr.PWorkID, this.WorkID);
                        qo.addRightBracket();

                        qo.addAnd();

                        qo.addLeftBracket();
                        qo.AddWhere(GenerWorkFlowAttr.PNodeID, nodeid);
                        qo.addAnd();
                        qo.AddWhere(GenerWorkFlowAttr.PFlowNo, this.HisFlow.No);
                        qo.addAnd();
                        qo.AddWhere(GenerWorkFlowAttr.WFSta, (int)WFSta.Runing);
                        qo.addAnd();
                        qo.AddWhere(GenerWorkFlowAttr.FK_Flow, subFlowNo);
                        qo.addRightBracket();

                        qo.DoQuery();
                        if (gwls.Count == 0)
                            continue;
                    }

                    err += "@如下子流程没有完成，你不能向下发送。@---------------------------------";
                    foreach (GenerWorkFlow gwf in gwls)
                        err += "@子流程ID=" + gwf.WorkID + ",子流程名称" + gwf.FlowName + ",标题:" + gwf.Title + ",当前执行人:" + gwf.TodoEmps + ",运行到节点:" + gwf.NodeName;
                }

                if (DataType.IsNullOrEmpty(err) == true)
                    return;

                err = Glo.DealExp(blockMsg, this.rptGe, null) + err;
                throw new Exception(err);
            }

            if (this.HisNode.BlockModel == BlockModel.BySQL)
            {
                /*按 sql 判断阻塞*/
                decimal d = DBAccess.RunSQLReturnValDecimal(Glo.DealExp(this.HisNode.BlockExp, this.rptGe, null), 0, 1);
                //如果值大于0进行阻塞
                if (d > 0)
                    throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe, null));

                return;
            }

            if (this.HisNode.BlockModel == BlockModel.ByExp)
            {
                /*按表达式阻塞. 格式为: @ABC=123 */
                //this.MsgOfCond = "@以表单值判断方向，值 " + en.EnDesc + "." + this.AttrKey + " (" + en.GetValStringByKey(this.AttrKey) + ") 操作符:(" + this.FK_Operator + ") 判断值:(" + this.OperatorValue.ToString() + ")";
                string exp = this.HisNode.BlockExp;
                string[] strs = exp.Trim().Split(' ');

                string key = strs[0].Trim().TrimStart('@');
                string oper = strs[1].Trim();
                string val = strs[2].Trim();
                val = val.Replace("'", "");
                val = val.Replace("%", "");
                val = val.Replace("~", "");

                BP.En.Row row = this.rptGe.Row;

                string valPara = null;
                if (row.ContainsKey(key) == false)
                {
                    try
                    {
                        /*如果不包含指定的关键的key, 就到公共变量里去找. */
                        if (Glo.SendHTOfTemp.ContainsKey(key) == false)
                            throw new Exception("@判断条件时错误,请确认参数是否拼写错误,没有找到对应的表达式:" + exp + " Key=(" + key + ") oper=(" + oper + ")Val=(" + val + ")");
                        valPara = Glo.SendHTOfTemp[key].ToString().Trim();
                    }
                    catch
                    {
                        //有可能是常量. 
                        valPara = key;
                    }
                }
                else
                {
                    valPara = row[key].ToString().Trim();
                }

                #region 开始执行判断.
                if (oper == "=")
                {
                    if (valPara != val)
                        return;

                    throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe, null));
                }

                if (oper.ToUpper() == "LIKE")
                {
                    if (valPara.Contains(val) == false)
                        return;

                    throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe, null));
                }

                if (oper == ">")
                {
                    if (float.Parse(valPara) > float.Parse(val))
                        throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe, null));

                    return;
                }

                if (oper == ">=")
                {
                    if (float.Parse(valPara) >= float.Parse(val))
                        throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe, null));

                    return;
                }

                if (oper == "<")
                {
                    if (float.Parse(valPara) < float.Parse(val))
                        throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe, null));

                    return;
                }

                if (oper == "<=")
                {
                    if (float.Parse(valPara) <= float.Parse(val))
                        throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe, null));

                    return;
                }

                if (oper == "!=")
                {
                    if (float.Parse(valPara) != float.Parse(val))
                        throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe, null));

                    return;
                }

                throw new Exception("@阻塞模式参数配置格式错误:" + exp + " Key=" + key + " oper=" + oper + " Val=" + val);
                #endregion 开始执行判断.
            }

            throw new Exception("@该阻塞模式没有实现...");
        }
        /// <summary>
        /// 发送到延续子流程.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="toEmps"></param>
        /// <returns></returns>
        private SendReturnObjs NodeSendToYGFlow(Node node, string toEmpIDs)
        {
            string sql = "";
            if (DataType.IsNullOrEmpty(toEmpIDs))
            {
                toEmpIDs = "";

                DataTable dt = null;

                #region 按照人员选择
                if (node.HisDeliveryWay == DeliveryWay.BySelected)
                {
                    sql = "SELECT FK_Emp No, EmpName Name FROM WF_SelectAccper WHERE FK_Node=" + node.NodeID + " AND WorkID=" + this.WorkID + " AND AccType=0";
                    dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 0)
                        throw new Exception("@没有为延续子流程设置接受人.");
                }
                #endregion 按照人员选择.

                #region 按照岗位与部门的交集.
                if (node.HisDeliveryWay == DeliveryWay.ByDeptAndStation)
                {

                    sql = "SELECT pdes.FK_Emp AS No"
                          + " FROM   Port_DeptEmpStation pdes"
                          + " INNER JOIN WF_NodeDept wnd ON wnd.FK_Dept = pdes.FK_Dept"
                          + " AND wnd.FK_Node = " + node.NodeID
                          + " INNER JOIN WF_NodeStation wns ON  wns.FK_Station = pdes.FK_Station"
                          + " AND wns.FK_Node =" + node.NodeID
                          + " ORDER BY pdes.FK_Emp";

                    dt = DBAccess.RunSQLReturnTable(sql);


                    if (dt.Rows.Count == 0)
                        throw new Exception("@节点访问规则(" + node.HisDeliveryWay.ToString() + ")错误:节点(" + node.NodeID + "," + node.Name + "), 按照岗位与部门的交集确定接受人的范围错误，没有找到人员:SQL=" + sql);
                }
                #endregion 按照岗位与部门的交集

                #region 仅按岗位计算
                if (node.HisDeliveryWay == DeliveryWay.ByStationOnly)
                {
                    sql = "SELECT A.FK_Emp No FROM " + BP.WF.Glo.EmpStation + " A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node ORDER BY A.FK_Emp";
                    ps = new Paras();
                    ps.Add("FK_Node", node.NodeID);
                    ps.SQL = sql;
                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                        throw new Exception("@节点访问规则错误:节点(" + node.NodeID + "," + node.Name + "), 仅按岗位计算，没有找到人员:SQL=" + ps.SQLNoPara);
                }
                #endregion

                #region 按绑定的人计算
                if (node.HisDeliveryWay == DeliveryWay.ByBindEmp)
                {
                    ps = new Paras();
                    ps.Add("FK_Node", node.NodeID);
                    ps.SQL = "SELECT FK_Emp No FROM WF_NodeEmp WHERE FK_Node=" + dbStr + "FK_Node ORDER BY FK_Emp";
                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                        throw new Exception("@流程设计错误:下一个节点(" + town.HisNode.Name + ")没有绑定工作人员 . ");
                }
                #endregion

                if (dt == null)
                    throw new Exception("err@您启动的子流程或者延续流程开始节点没有明确的设置接受人.");

                //组装到达的人员.
                foreach (DataRow dr in dt.Rows)
                    toEmpIDs += dr["No"].ToString();
            }

            if (toEmpIDs == "")
                throw new Exception("@延续子流程目前仅仅支持选择接受人方式.");

            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(node.FK_Flow, null, null,
                toEmpIDs, null, this.WorkID, 0, this.HisNode.FK_Flow, this.HisNode.NodeID, BP.Web.WebUser.No, 0, null);

            //复制当前信息.
            Work wk = node.HisWork;
            wk.OID = workid;
            wk.RetrieveFromDBSources();
            wk.Copy(this.HisWork);
            wk.Update();

            //为接受人显示待办.
            BP.WF.Dev2Interface.Node_SetDraft2Todolist(node.FK_Flow, workid);

            //设置变量.
            this.addMsg(SendReturnMsgFlag.VarToNodeID, node.NodeID.ToString(), workid.ToString(), SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarAcceptersID, toEmpIDs, toEmpIDs, SendReturnMsgType.SystemMsg);

            //设置消息.
            this.addMsg("Msg1", "子流程(" + node.FlowName + ")已经启动,发送给{" + toEmpIDs + "}处理人.");
            //this.addMsg(SendReturnMsgFlag.MsgOfText, "需要等待子流程完成后，该流程才能向下运动。");
            this.addMsg("Msg2", "当前您的待办已经消失,需要等待子流程完成后您的待办才能显示,您可以从在途里查看工作进度.");

            //设置当前工作操作员不可见.
            sql = "UPDATE WF_GenerWorkerList SET IsPass=80 WHERE WorkID=" + this.WorkID + " AND IsPass=0";
            BP.DA.DBAccess.RunSQL(sql);

            return HisMsgObjs;
        }

        /// <summary>
        /// 工作流发送业务处理.
        /// 升级日期:2012-11-11.
        /// 升级原因:代码逻辑性不清晰,有遗漏的处理模式.
        /// 修改人:zhoupeng.
        /// 修改地点:厦门.
        /// ----------------------------------- 说明 -----------------------------
        /// 1，方法体分为三大部分: 发送前检查\5*5算法\发送后的业务处理.
        /// 2, 详细请参考代码体上的说明.
        /// 3, 发送后可以直接获取它的
        /// </summary>
        /// <param name="jumpToNode">要跳转的节点,可以为空.</param>
        /// <param name="jumpToEmp">要跳转的人,可以为空.</param>
        /// <returns>返回执行结果</returns>
        public SendReturnObjs NodeSend(Node jumpToNode, string jumpToEmp)
        {
            if (this.HisNode.IsGuestNode)
            {
                if (this.Execer != "Guest")
                    throw new Exception("@当前节点（" + this.HisNode.Name + "）是客户执行节点，所以当前登录人员应当是Guest,现在是:" + this.Execer);
            }

            #region 安全性检查.
            //   第1: 检查是否可以处理当前的工作.
            if (this.HisNode.IsStartNode == false
                && BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.HisNode.FK_Flow, this.HisNode.NodeID,
                this.WorkID, this.Execer) == false)
                throw new Exception("@当前工作您已经处理完成，或者您(" + this.Execer + " " + this.ExecerName + ")没有处理当前工作的权限。");

            // 第1.2: 调用发起前的事件接口,处理用户定义的业务逻辑.
            string sendWhen = this.HisFlow.DoFlowEventEntity(EventListOfNode.SendWhen, this.HisNode,
                this.HisWork, null);

            //返回格式. @Info=xxxx@ToNodeID=xxxx@ToEmps=xxxx
            if (sendWhen != null && sendWhen.IndexOf("@") >= 0)
            {
                AtPara ap = new AtPara(sendWhen);
                int nodeid = ap.GetValIntByKey("ToNodeID", 0);
                if (nodeid != 0)
                {
                    if (jumpToNode != null && jumpToNode.NodeID != nodeid)
                        throw new Exception("在该流程的FEE中您指定了跳转到的节点,但是在外部您有传入了发送到的节点，两者相矛盾,系统不知道要转向:" + jumpToNode.NodeID + ",还是转向:" + nodeid);

                    jumpToNode = new Node(nodeid);
                }

                //监测是否有停止流程的标志？
                this.IsStopFlow = ap.GetValBoolenByKey("IsStopFlow", false);

                string toEmps = ap.GetValStrByKey("ToEmps");
                if (DataType.IsNullOrEmpty(toEmps) == true)
                    jumpToEmp = toEmps;

                //处理str信息.
                sendWhen = sendWhen.Replace("@Info=", "");
                sendWhen = sendWhen.Replace("@IsStopFlow=1", "");
                sendWhen = sendWhen.Replace("@ToNodeID=" + nodeid.ToString(), "");
                sendWhen = sendWhen.Replace("@ToEmps=" + toEmps, "");
            }

            if (sendWhen != null)
            {
                /*说明有事件要执行,把执行后的数据查询到实体里*/
                this.HisWork.RetrieveFromDBSources();
                this.HisWork.ResetDefaultVal();
                this.HisWork.Rec = this.Execer;
                this.HisWork.RecText = this.ExecerName;
                if (DataType.IsNullOrEmpty(sendWhen) == false)
                {
                    sendWhen = System.Web.HttpUtility.UrlDecode(sendWhen);
                    if (sendWhen.StartsWith("false") || sendWhen.StartsWith("False") || sendWhen.StartsWith("error") || sendWhen.StartsWith("Error"))
                    {
                        this.addMsg(SendReturnMsgFlag.SendWhen, sendWhen);
                        sendWhen = sendWhen.Replace("false", "");
                        sendWhen = sendWhen.Replace("False", "");

                        throw new Exception("@执行发送前事件失败:" + sendWhen);
                    }
                }

                //把发送sendWhen 消息提示给用户.
                this.addMsg("SendWhen", sendWhen, sendWhen, SendReturnMsgType.Info);
            }
            #endregion 安全性检查.

            //加入系统变量.
            this.addMsg(SendReturnMsgFlag.VarCurrNodeID, this.HisNode.NodeID.ToString(), this.HisNode.NodeID.ToString(), SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarCurrNodeName, this.HisNode.Name, this.HisNode.Name, SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarWorkID, this.WorkID.ToString(), this.WorkID.ToString(), SendReturnMsgType.SystemMsg);

            if (this.IsStopFlow == true)
            {
                /*在检查完后，反馈来的标志流程已经停止了。*/

                //查询出来当前节点的工作报表.
                this.rptGe = this.HisFlow.HisGERpt;
                this.rptGe.SetValByKey("OID", this.WorkID);
                this.rptGe.RetrieveFromDBSources();

                this.Func_DoSetThisWorkOver();
                this.rptGe.WFState = WFState.Complete;
                this.rptGe.Update();
                this.HisGenerWorkFlow.Update(); //added by liuxc,2016-10=24,最后节点更新Sender字段
                //执行考核
                Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, 0, this.HisGenerWorkFlow.Title);
                return this.HisMsgObjs;
            }


            //设置跳转节点，如果有可以为null.
            this.JumpToNode = jumpToNode;
            this.JumpToEmp = jumpToEmp;

            string sql = null;
            DateTime dt = DateTime.Now;
            this.HisWork.Rec = this.Execer;
            this.WorkID = this.HisWork.OID;


            #region 第一步: 检查当前操作员是否可以发送: 共分如下 3 个步骤.
            //第1.2.1: 如果是开始节点，就要检查发起流程限制条件.
            if (this.HisNode.IsStartNode)
            {
                if (Glo.CheckIsCanStartFlow_SendStartFlow(this.HisFlow, this.HisWork) == false)
                    throw new Exception("@违反了流程发起限制条件:" + Glo.DealExp(this.HisFlow.StartLimitAlert, this.HisWork, null));
            }

            // 第1.3: 判断当前流程状态.
            if (this.HisNode.IsStartNode == false
                && this.HisGenerWorkFlow.WFState == WFState.Askfor)
            {
                /*如果是加签状态, 就判断加签后，是否要返回给执行加签人.*/
                GenerWorkerLists gwls = new GenerWorkerLists();
                gwls.Retrieve(GenerWorkerListAttr.FK_Node, this.HisNode.NodeID,
                    GenerWorkerListAttr.WorkID, this.WorkID);

                bool isDeal = false;
                AskforHelpSta askForSta = AskforHelpSta.AfterDealSend;
                foreach (GenerWorkerList item in gwls)
                {
                    if (item.IsPassInt == (int)AskforHelpSta.AfterDealSend)
                    {
                        /*如果是加签后，直接发送就不处理了。*/
                        isDeal = true;
                        askForSta = AskforHelpSta.AfterDealSend;

                        // 更新workerlist, 设置所有人员的状态为已经处理的状态,让它走到下一步骤去.
                        DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=1 WHERE FK_Node=" + this.HisNode.NodeID + " AND WorkID=" + this.WorkID);

                        //写入日志.
                        this.AddToTrack(ActionType.ForwardAskfor, item.FK_Emp, item.FK_EmpText,
                            this.HisNode.NodeID, this.HisNode.Name, "加签后向下发送，直接发送给下一步处理人。");
                    }

                    if (item.IsPassInt == (int)AskforHelpSta.AfterDealSendByWorker)
                    {
                        /*如果是加签后，在由我直接发送。*/
                        item.IsPassInt = 0;

                        isDeal = true;
                        askForSta = AskforHelpSta.AfterDealSendByWorker;

                        // 更新workerlist, 设置所有人员的状态为已经处理的状态.
                        DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=1 WHERE FK_Node=" + this.HisNode.NodeID + " AND WorkID=" + this.WorkID);

                        // 把发起加签人员的状态更新过来，让他可见待办工作.
                        item.IsPassInt = 0;
                        item.Update();

                        // 更新流程状态.
                        this.HisGenerWorkFlow.WFState = WFState.AskForReplay;
                        this.HisGenerWorkFlow.Update();

                        //让加签人，设置成工作未读。
                        BP.WF.Dev2Interface.Node_SetWorkUnRead(this.WorkID, item.FK_Emp);

                        // 从临时变量里获取回复加签意见.
                        string replyInfo = this.HisGenerWorkFlow.Paras_AskForReply;

                        ////写入日志.
                        //this.AddToTrack(ActionType.ForwardAskfor, item.FK_Emp, item.FK_EmpText,
                        //    this.HisNode.NodeID, this.HisNode.Name,
                        //    "加签后向下发送，并转向加签人发起人（" + item.FK_Emp + "，" + item.FK_EmpText + "）。<br>意见:" + replyInfo);

                        //写入日志.
                        this.AddToTrack(ActionType.ForwardAskfor, item.FK_Emp, item.FK_EmpText,
                            this.HisNode.NodeID, this.HisNode.Name, "回复意见:" + replyInfo);

                        //加入系统变量。
                        this.addMsg(SendReturnMsgFlag.VarToNodeID, this.HisNode.NodeID.ToString(), SendReturnMsgType.SystemMsg);
                        this.addMsg(SendReturnMsgFlag.VarToNodeName, this.HisNode.Name, SendReturnMsgType.SystemMsg);
                        this.addMsg(SendReturnMsgFlag.VarAcceptersID, item.FK_Emp, SendReturnMsgType.SystemMsg);
                        this.addMsg(SendReturnMsgFlag.VarAcceptersName, item.FK_EmpText, SendReturnMsgType.SystemMsg);

                        //加入提示信息.
                        this.addMsg(SendReturnMsgFlag.SendSuccessMsg, "已经转给，加签的发起人(" + item.FK_Emp + "," + item.FK_EmpText + ")", SendReturnMsgType.Info);

                        //删除当前操作员临时增加的工作列表记录, 如果不删除就会导致第二次加签失败.
                        GenerWorkerList gwl = new GenerWorkerList();
                        gwl.Delete(GenerWorkerListAttr.FK_Node, this.HisNode.NodeID,
                            GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Emp, this.Execer);

                        //执行时效考核.
                        Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, 0, this.HisGenerWorkFlow.Title);

                        //返回发送对象.
                        return this.HisMsgObjs;
                    }
                }

                if (isDeal == false)
                    throw new Exception("@流程引擎错误，不应该找不到加签的状态.");
            }


            // 第3: 如果是是合流点，有子线程未完成的情况.
            if (this.HisNode.IsHL || this.HisNode.HisRunModel == RunModel.FHL)
            {
                /*   如果是合流点 检查当前是否是合流点如果是，则检查分流上的子线程是否完成。*/
                /*检查是否有子线程没有结束*/
                Paras ps = new Paras();
                ps.SQL = "SELECT WorkID,FK_Emp,FK_EmpText,FK_NodeText FROM WF_GenerWorkerList WHERE FID=" + ps.DBStr + "FID AND IsPass=0 AND IsEnable=1";
                ps.Add(WorkAttr.FID, this.WorkID);

                DataTable dtWL = DBAccess.RunSQLReturnTable(ps);
                string infoErr = "";
                if (dtWL.Rows.Count != 0)
                {
                    if (this.HisNode.ThreadKillRole == ThreadKillRole.None
                        || this.HisNode.ThreadKillRole == ThreadKillRole.ByHand)
                    {
                        infoErr += "@您不能向下发送，有如下子线程没有完成。";
                        foreach (DataRow dr in dtWL.Rows)
                        {
                            infoErr += "@操作员编号:" + dr["FK_Emp"] + "," + dr["FK_EmpText"] + ",停留节点:" + dr["FK_NodeText"];
                        }

                        if (this.HisNode.ThreadKillRole == ThreadKillRole.ByHand)
                            infoErr += "@请通知他们处理完成,或者强制删除子流程您才能向下发送.";
                        else
                            infoErr += "@请通知他们处理完成,您才能向下发送.";

                        //抛出异常阻止它向下运动。
                        throw new Exception(infoErr);
                    }

                    if (this.HisNode.ThreadKillRole == ThreadKillRole.ByAuto)
                    {
                        //删除每个子线程，然后向下运动。
                        foreach (DataRow dr in dtWL.Rows)
                            BP.WF.Dev2Interface.Flow_DeleteSubThread(this.HisFlow.No, Int64.Parse(dr[0].ToString()), "合流点发送时自动删除");
                    }
                }
            }
            #endregion 第一步: 检查当前操作员是否可以发送

            //查询出来当前节点的工作报表.
            this.rptGe = this.HisFlow.HisGERpt;
            this.rptGe.SetValByKey("OID", this.WorkID);
            this.rptGe.RetrieveFromDBSources();

            //检查阻塞模式.
            this.CheckBlockModel();

            // 检查FormTree必填项目,如果有一些项目没有填写就抛出异常.
            this.CheckFrmIsNotNull();

            // 处理自动运行 - 预先设置未来的运行节点.
            this.DealAutoRunEnable();


            //把数据更新到数据库里.
            this.HisWork.DirectUpdate();
            if (this.HisWork.EnMap.PhysicsTable != this.rptGe.EnMap.PhysicsTable)
            {
                // 有可能外部参数传递过来导致，rpt表数据没有发生变化。
                this.rptGe.Copy(this.HisWork);
                //首先执行保存，不然会影响条件的判断 by dgq 2016-1-14
                this.rptGe.Update();
            }

            //如果是队列节点, 就判断当前的队列人员是否走完。
            if (this.TodolistModel == TodolistModel.Order)
            {
                if (this.DealOradeNode() == true)
                {
                    //执行时效考核.
                    Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID, this.rptGe.Title);
                    return this.HisMsgObjs;
                }
            }

            //如果是协作模式节点, 就判断当前的队列人员是否走完.
            if (this.TodolistModel == TodolistModel.Teamup)
            {

                // @fanleiwei ,增加了此部分.
                string todoEmps = this.HisGenerWorkFlow.TodoEmps;
                todoEmps = todoEmps.Replace(WebUser.No + "," + WebUser.Name + ";", "");
                todoEmps = todoEmps.Replace(WebUser.No + "," + WebUser.Name, "");

                this.HisGenerWorkFlow.TodoEmps = todoEmps;
                this.HisGenerWorkFlow.Update(GenerWorkFlowAttr.TodoEmps, todoEmps);


                /* 如果是协作*/
                if (this.DealTeamUpNode() == true)
                {
                    /*
                     * 1. 判断是否传递过来到达节点，到达人员信息，如果传递过来，就可能是主持人在会签之后执行的发送.
                     * 2. 会签之后执行的发送，就要把到达节点，到达人员存储到数据表里.
                     */

                    if (jumpToNode != null)
                    {
                        /*如果是就记录下来发送到达的节点ID,到达的人员ID.*/
                        this.HisGenerWorkFlow.HuiQianSendToNodeIDStr = this.HisNode.NodeID + "," + jumpToNode.NodeID;
                        if (jumpToEmp == null)
                            this.HisGenerWorkFlow.HuiQianSendToEmps = "";
                        else
                            this.HisGenerWorkFlow.HuiQianSendToEmps = jumpToEmp;

                        this.HisGenerWorkFlow.Update();
                    }

                    //执行时效考核.
                    Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID, this.rptGe.Title);
                    return this.HisMsgObjs;
                }

                //取出来已经存储的到达节点，节点人员信息. 在tempUp模式的会签时，主持人发送会把发送到节点，发送给人员的信息
                // 存储到wf_generworkflow里面.
                if (this.JumpToNode == null)
                {
                    /*如果是就记录下来发送到达的节点ID,到达的人员ID.*/
                    string strs = this.HisGenerWorkFlow.HuiQianSendToNodeIDStr;

                    if (strs.Contains(",") == true)
                    {
                        string[] nds = strs.Split(',');
                        int fromNodeID = int.Parse(nds[0]);
                        int toNodeID = int.Parse(nds[1]);
                        if (fromNodeID == this.HisNode.NodeID)
                        {
                            JumpToNode = new Node(toNodeID);
                            JumpToEmp = this.HisGenerWorkFlow.HuiQianSendToEmps;
                        }
                    }
                    //this.HisGenerWorkFlow.SendToEmps = jumpToEmp;
                    //this.HisGenerWorkFlow.Update();
                }
            }

            //如果是协作组长模式节点, 就判断当前的队列人员是否走完.
            if (this.TodolistModel == TodolistModel.TeamupGroupLeader)
            {
                /* 如果是协作组长模式.*/
                if (this.DealTeamupGroupLeader() == true)
                {
                    //执行时效考核.
                    //Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID, this.rptGe.Title);
                    return this.HisMsgObjs;
                }
            }

            // 启动事务, 这里没有实现.
            DBAccess.DoTransactionBegin();
            try
            {
                if (this.HisNode.IsStartNode)
                    InitStartWorkDataV2(); // 初始化开始节点数据, 如果当前节点是开始节点.

                //处理发送人，把发送人的信息放入wf_generworkflow 2015-01-14. 原来放入WF_GenerWorkerList.
                oldSender = this.HisGenerWorkFlow.Sender; //旧发送人,在回滚的时候把该发送人赋值给他.
                this.HisGenerWorkFlow.Sender = BP.WF.Glo.DealUserInfoShowModel(WebUser.No, WebUser.Name);

                #region 处理退回的情况.
                if (this.HisGenerWorkFlow.WFState == WFState.ReturnSta)
                {
                    /* 检查该退回是否是原路返回 ? */
                    Paras ps = new Paras();
                    ps.SQL = "SELECT ReturnNode,Returner,ReturnerName,IsBackTracking FROM WF_ReturnWork WHERE WorkID=" + dbStr + "WorkID AND IsBackTracking=1 ORDER BY RDT DESC";
                    ps.Add(ReturnWorkAttr.WorkID, this.WorkID);
                    DataTable mydt = DBAccess.RunSQLReturnTable(ps);
                    if (mydt.Rows.Count != 0)
                    {
                        //有可能查询出来多个，因为按时间排序了，只取出最后一次退回的，看看是否有退回并原路返回的信息。

                        /*确认这次退回，是退回并原路返回 ,  在这里初始化它的工作人员, 与将要发送的节点. */
                        this.JumpToNode = new Node(int.Parse(mydt.Rows[0]["ReturnNode"].ToString()));
                        this.JumpToEmp = mydt.Rows[0]["Returner"].ToString();
                        string toEmpName = mydt.Rows[0]["ReturnerName"].ToString();

                        #region 如果当前是退回, 并且当前的运行模式是按照流程图运行.
                        if (this.HisGenerWorkFlow.TransferCustomType == TransferCustomType.ByCCBPMDefine)
                        {
                            if (this.JumpToNode.TodolistModel == TodolistModel.Order
                                || this.JumpToNode.TodolistModel == TodolistModel.TeamupGroupLeader
                                || this.JumpToNode.TodolistModel == TodolistModel.Teamup)
                            {
                                /*如果是多人处理节点.*/
                                this.DealReturnOrderTeamup();

                                //写入track.
                                this.AddToTrack(ActionType.Forward, this.JumpToEmp, toEmpName, this.JumpToNode.NodeID, this.JumpToNode.Name, "退回后发送");

                                //执行时效考核.
                                Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID, this.rptGe.Title);
                                return this.HisMsgObjs;
                            }
                        }
                        #endregion 如果当前是退回, 并且当前的运行模式是按照流程图运行.*/

                        #region  如果当前是退回. 并且当前的运行模式按照自由流程设置方式运行
                        if (this.HisGenerWorkFlow.TransferCustomType == TransferCustomType.ByWorkerSet)
                        {
                            if (this.HisGenerWorkFlow.TodolistModel == TodolistModel.Order
                                || this.JumpToNode.TodolistModel == TodolistModel.TeamupGroupLeader
                                || this.HisGenerWorkFlow.TodolistModel == TodolistModel.Teamup)
                            {
                                /*如果是多人处理节点.*/
                                this.DealReturnOrderTeamup();

                                //写入track.
                                this.AddToTrack(ActionType.Forward, this.JumpToEmp, toEmpName, this.JumpToNode.NodeID, this.JumpToNode.Name, "退回后发送(按照自定义运行模式)");

                                //执行时效考核.
                                Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID, this.rptGe.Title);
                                return this.HisMsgObjs;
                            }
                        }
                        #endregion  如果当前是退回. 并且当前的运行模式按照自由流程设置方式运行
                    }
                }
                #endregion 处理退回的情况.

                //做了不可能性的判断.
                if (this.HisGenerWorkFlow.FK_Node != this.HisNode.NodeID)
                    throw new Exception("@流程出现的错误,工作ID=" + this.WorkID + ",当前活动点(" + this.HisGenerWorkFlow.FK_Node + "" + this.HisGenerWorkFlow.NodeName + ")与发送点(" + this.HisNode.NodeID + this.HisNode.Name + ")不一致");

                // 检查完成条件。
                if (jumpToNode != null && this.HisNode.IsEndNode)
                {
                    /*是跳转的情况，并且是最后的节点，就不检查流程完成条件。*/
                }
                else
                {
                    //检查流程完成条件.
                    this.CheckCompleteCondition();
                }

                // 处理自由流程. add by stone. 2014-11-23.
                if (jumpToNode == null && this.HisGenerWorkFlow.TransferCustomType == TransferCustomType.ByWorkerSet)
                {
                    //如果没有指定要跳转到的节点，并且当前处理手工干预的运行状态.
                    _transferCustom = TransferCustom.GetNextTransferCustom(this.WorkID, this.HisNode.NodeID);
                    if (_transferCustom == null)
                    {
                        /* 表示执行到这里结束流程. */
                        this.IsStopFlow = true;

                        this.HisGenerWorkFlow.WFState = WFState.Complete;
                        this.rptGe.WFState = WFState.Complete;
                        string msg1 = this.HisWorkFlow.DoFlowOver(ActionType.FlowOver,
                            "流程已经按照设置的步骤成功结束。", this.HisNode, this.rptGe);
                        this.addMsg(SendReturnMsgFlag.End, msg1);
                    }
                    else
                    {
                        this.JumpToNode = new Node(_transferCustom.FK_Node);
                        this.JumpToEmp = _transferCustom.Worker;
                        this.HisGenerWorkFlow.TodolistModel = _transferCustom.TodolistModel;
                    }
                }

                // 处理质量考核，在发送前。
                this.DealEval();

                // 加入系统变量.
                if (this.IsStopFlow)
                    this.addMsg(SendReturnMsgFlag.IsStopFlow, "1", "流程已经结束", SendReturnMsgType.Info);
                else
                    this.addMsg(SendReturnMsgFlag.IsStopFlow, "0", "流程未结束", SendReturnMsgType.SystemMsg);

                //  string mymsgHtml = "@查看<img src='./Img/Btn/PrintWorkRpt.gif' ><a href='WFRpt.htm?WorkID=" + this.HisWork.OID + "&FID=" + this.HisWork.FID + "&FK_Flow=" + this.HisNode.FK_Flow + "' target='_self' >工作轨迹</a>";
                // this.addMsg(SendReturnMsgFlag.MsgOfText, mymsgHtml);

                if (this.IsStopFlow == true)
                {
                    /*在检查完后，反馈来的标志流程已经停止了。*/
                    // 这里应该去掉，不然事件就不起作用. 翻译.
                    /*
                    this.Func_DoSetThisWorkOver();
                    this.rptGe.WFState = WFState.Complete;
                    this.rptGe.Update();
                    this.HisGenerWorkFlow.Update(); //added by liuxc,2016-10=24,最后节点更新Sender字段
                     * */
                    //执行考核
                    Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, 0, this.HisGenerWorkFlow.Title);
                    return HisMsgObjs;
                }

                //@增加发送到子流程的判断.
                if (jumpToNode != null && this.HisNode.FK_Flow != jumpToNode.FK_Flow)
                {
                    /*判断是否是延续子流程. */
                    return NodeSendToYGFlow(jumpToNode, jumpToEmp);
                }

                #region 第二步: 进入核心的流程运转计算区域. 5*5 的方式处理不同的发送情况.

                // 执行节点向下发送的25种情况的判断.
                this.NodeSend_Send_5_5();

                //通过 55 之后要判断是否要结束流程，如果结束流程就执行相关的更新。
                if (this.IsStopFlow)
                {
                    this.rptGe.WFState = WFState.Complete;
                    this.Func_DoSetThisWorkOver();
                    this.HisGenerWorkFlow.Update(); //added by liuxc,2016-10=24,最后节点更新Sender字段
                }
                else
                {
                    //如果是退回状态，就把是否原路返回的轨迹去掉.
                    if (this.HisGenerWorkFlow.WFState == WFState.ReturnSta)
                        BP.DA.DBAccess.RunSQL("UPDATE WF_ReturnWork SET IsBackTracking=0 WHERE WorkID=" + this.WorkID);

                    this.Func_DoSetThisWorkOver();
                    if (town != null && town.HisNode.HisBatchRole == BatchRole.Group)
                    {
                        this.HisGenerWorkFlow.WFState = WFState.Batch;
                        this.HisGenerWorkFlow.Update();
                    }
                }

                //计算从发送到现在的天数.
                this.rptGe.FlowDaySpan = DataType.GeTimeLimits(this.HisGenerWorkFlow.RDT);
                Int64 fid = this.rptGe.FID;
                this.rptGe.Update();
                #endregion 第二步: 5*5 的方式处理不同的发送情况.

                #region 第三步: 处理发送之后的业务逻辑.
                //把当前节点表单数据copy的流程数据表里.
                this.DoCopyCurrentWorkDataToRpt();

                //处理合理节点的1变N的问题.
                this.CheckFrm1ToN();

                //处理子线程的独立表单向合流节点的独立表单明细表的数据汇总.
                this.CheckFrmHuiZongToDtl();

                #endregion 第三步: 处理发送之后的业务逻辑.

                #region 处理子流程
                if (this.HisNode.IsStartNode && this.HisNode.SubFlowStartWay != SubFlowStartWay.None)
                    CallSubFlow();

                #endregion 处理子流程

                #region 生成单据
                if (this.HisNode.HisPrintDocEnable == PrintDocEnable.PrintRTF && this.HisNode.BillTemplates.Count > 0)
                {

                    BillTemplates reffunc = this.HisNode.BillTemplates;

                    #region 生成单据信息
                    Int64 workid = this.HisWork.OID;
                    int nodeId = this.HisNode.NodeID;
                    string flowNo = this.HisNode.FK_Flow;
                    #endregion

                    DateTime dtNow = DateTime.Now;
                    Flow fl = this.HisNode.HisFlow;
                    string year = dt.Year.ToString();
                    string billInfo = "";
                    foreach (BillTemplate func in reffunc)
                    {
                        if (func.HisBillFileType != BillFileType.RuiLang)
                        {
                            string file = year + "_" + this.ExecerDeptNo + "_" + func.No + "_" + workid + ".doc";
                            BP.Pub.RTFEngine rtf = new BP.Pub.RTFEngine();

                            Works works;
                            string[] paths;
                            string path;
                            try
                            {
                                #region 把数据放入 单据引擎。
                                rtf.HisEns.Clear(); //主表数据。
                                rtf.EnsDataDtls.Clear(); // 明细表数据.

                                // 找到主表数据.
                                rtf.HisGEEntity = new GEEntity(this.rptGe.ClassID);
                                rtf.HisGEEntity.Row = rptGe.Row;

                                // 把每个节点上的工作加入到报表引擎。
                                rtf.AddEn(this.HisWork);
                                rtf.ensStrs += ".ND" + this.HisNode.NodeID;

                                //把当前work的Dtl 数据放进去了。
                                System.Collections.Generic.List<Entities> al = this.HisWork.GetDtlsDatasOfList();

                                foreach (Entities ens in al)
                                    rtf.AddDtlEns(ens);
                                #endregion 把数据放入 单据引擎。

                                #region 生成单据

                                paths = file.Split('_');
                                path = paths[0] + "/" + paths[1] + "/" + paths[2] + "/";
                                string billUrl = VirPath + "DataUser/Bill/" + path + file;
                                if (func.HisBillFileType == BillFileType.PDF)
                                {
                                    billUrl = billUrl.Replace(".doc", ".pdf");
                                    billInfo += "<img src='./Img/FileType/PDF.gif' /><a href='" + billUrl + "' target=_blank >" + func.Name + "</a>";
                                }
                                else
                                {
                                    billInfo += "<img src='./Img/FileType/doc.gif' /><a href='" + billUrl + "' target=_blank >" + func.Name + "</a>";
                                }

                                path = BP.WF.Glo.FlowFileBill + year + "\\" + this.ExecerDeptNo + "\\" + func.No + "\\";
                                // path = AppDomain.CurrentDomain.BaseDirectory + path;
                                if (System.IO.Directory.Exists(path) == false)
                                    System.IO.Directory.CreateDirectory(path);

                                rtf.MakeDoc(func.TempFilePath + ".rtf",
                                    path, file, func.ReplaceVal, false);
                                #endregion

                                #region 转化成pdf.
                                if (func.HisBillFileType == BillFileType.PDF)
                                {
                                    string rtfPath = path + file;
                                    string pdfPath = rtfPath.Replace(".doc", ".pdf");
                                    try
                                    {
                                        Glo.Rtf2PDF(rtfPath, pdfPath);
                                    }
                                    catch (Exception ex)
                                    {
                                        this.addMsg("RptError", "产生报表数据错误:" + ex.Message);
                                    }
                                }
                                #endregion

                                #region 保存单据
                                Bill bill = new Bill();
                                bill.MyPK = this.HisWork.FID + "_" + this.HisWork.OID + "_" + this.HisNode.NodeID + "_" + func.No;
                                bill.FID = this.HisWork.FID;
                                bill.WorkID = this.HisWork.OID;
                                bill.FK_Node = this.HisNode.NodeID;
                                bill.FK_Dept = this.ExecerDeptNo;
                                bill.FK_Emp = this.Execer;
                                bill.Url = billUrl;
                                bill.RDT = DataType.CurrentDataTime;
                                bill.FullPath = path + file;
                                bill.FK_NY = DataType.CurrentYearMonth;
                                bill.FK_Flow = this.HisNode.FK_Flow;
                                bill.FK_BillType = func.FK_BillType;
                                bill.FK_Flow = this.HisNode.FK_Flow;
                                bill.Emps = this.rptGe.GetValStrByKey("Emps");
                                bill.FK_Starter = this.rptGe.GetValStrByKey("Rec");
                                bill.StartDT = this.rptGe.GetValStrByKey("RDT");
                                bill.Title = this.rptGe.GetValStrByKey("Title");
                                bill.FK_Dept = this.rptGe.GetValStrByKey("FK_Dept");
                                try
                                {
                                    bill.Save();
                                }
                                catch
                                {
                                    bill.Update();
                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                BP.WF.DTS.InitBillDir dir = new BP.WF.DTS.InitBillDir();
                                dir.Do();
                                path = BP.WF.Glo.FlowFileBill + year + "\\" + this.ExecerDeptNo + "\\" + func.No + "\\";
                                string msgErr = "@" + string.Format("生成单据失败，请让管理员检查目录设置") + "[" + BP.WF.Glo.FlowFileBill + "]。@Err：" + ex.Message + " @File=" + file + " @Path:" + path;
                                billInfo += "@<font color=red>" + msgErr + "</font>@<hr>@系统已经做了可能性的修复，请您在发送一次，如果问题仍然存在请联系管理员。";
                                throw new Exception(msgErr + "@其它信息:" + ex.Message);
                            }
                        }

                    } // end 生成循环单据。

                    if (billInfo != "")
                        billInfo = "@" + billInfo;
                    this.addMsg(SendReturnMsgFlag.BillInfo, billInfo);
                }
                #endregion

                #region 执行抄送.
                //执行抄送.
                if (this.HisNode.IsEndNode == false)
                {
                    //执行自动抄送.
                    string ccMsg1 = WorkFlowBuessRole.DoCCAuto(this.HisNode, this.rptGe, this.WorkID, this.HisWork.FID);
                    //按照指定的字段抄送.
                    string ccMsg2 = WorkFlowBuessRole.DoCCByEmps(this.HisNode, this.rptGe, this.WorkID, this.HisWork.FID);
                    string ccMsg = ccMsg1 + ccMsg2;

                    if (DataType.IsNullOrEmpty(ccMsg) == false)
                    {
                        this.addMsg("CC", "@自动抄送给:" + ccMsg);
                        this.AddToTrack(ActionType.CC, WebUser.No, WebUser.Name, this.HisNode.NodeID, this.HisNode.Name, ccMsg1 + ccMsg2, this.HisNode);
                    }
                }

                DBAccess.DoTransactionCommit(); //提交事务.
                #endregion 处理主要业务逻辑.

                #region 执行启动子流程.
                string msg = "";
                if (this.HisNode.SFActiveFlows.Length > 2)
                {
                    string[] fls = this.HisNode.SFActiveFlows.Split(',');
                    foreach (string flowNo in fls)
                    {
                        if (DataType.IsNullOrEmpty(flowNo) == true)
                            continue;

                        //检查是否有发起限制子流程的条件.
                        Flow fl = new Flow(flowNo);

                        #region 检查流程启动条件.
                        bool isPass = false;
                        //检查是否有
                        Conds cds = new Conds();
                        cds.Retrieve(CondAttr.FK_Node, this.HisNode.NodeID, CondAttr.NodeID, int.Parse(flowNo));
                        foreach (Cond cd in cds)
                        {
                            cd.WorkID = this.HisWork.OID;
                            cd.NodeID = this.HisNode.NodeID;
                        }

                        if (cds.Count == 0)
                            isPass = true;
                        else
                            isPass = cds.IsPass;

                        if (isPass == false)
                        {
                            msg += "@不符合子流程[" + fl.Name + "]启动条件。";
                            continue;
                        }
                        #endregion 检查流程启动条件.

                        #region 启动子流程.
                        //组织从表数据，把它copy到子流程里面.
                        DataSet dsDtl = new DataSet();
                        MapDtls dtls = new MapDtls();
                        dtls.Retrieve(MapDtlAttr.FK_MapData, this.HisWork.ClassID);
                        foreach (MapDtl dtl in dtls)
                        {
                            string sqlDtl = "SELECT * FROM " + dtl.PTable + " WHERE RefPK='" + this.WorkID + "'";

                            DataTable dtDtl = DBAccess.RunSQLReturnTable(sqlDtl);

                            dtDtl.TableName = dtl.No.Replace("ND" + this.HisNode.NodeID, "ND" + int.Parse(fl.No + "01"));

                            dsDtl.Tables.Add(dtDtl);
                        }

                        SendReturnObjs sendObjs = BP.WF.Dev2Interface.Node_StartWork(fl.No, this.HisWork.Row, dsDtl, 0, null, this.WorkID, this.HisFlow.No);
                        msg += "@子流程[" + fl.Name + "]已经启动，已经发送给：" + sendObjs.VarAcceptersName + "，发送到：" + sendObjs.VarToNodeName + "。";
                        #endregion 启动子流程.

                    }
                    if (msg != "")
                        this.addMsg(SendReturnMsgFlag.MsgOfText, msg);

                }
                #endregion 执行启动子流程.

                #region 处理流程数据与业务表的数据同步.
                if (this.HisFlow.DTSWay != FlowDTSWay.None)
                    this.HisFlow.DoBTableDTS(this.rptGe, this.HisNode, this.IsStopFlow);

                #endregion 处理流程数据与业务表的数据同步.

                #region 处理发送成功后的消息提示
                if (this.HisNode.HisTurnToDeal == TurnToDeal.SpecMsg)
                {
                    string htmlInfo = "";
                    string textInfo = "";
                    #region 判断当前处理人员，可否处理下一步工作.
                    if (this.town != null
                        && this.HisRememberMe != null
                        && this.HisRememberMe.Emps.Contains("@" + WebUser.No + "@") == true)
                    {
                        string url = "MyFlow.htm?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + town.HisNode.NodeID + "&FID=" + this.rptGe.FID;
                        //   htmlInfo = "@<a href='" + url + "' >下一步工作您仍然可以处理，点击这里现在处理。</a>.";
                        textInfo = "@下一步工作您仍然可以处理。";
                        this.addMsg(SendReturnMsgFlag.MsgOfText, textInfo, null);
                    }
                    #endregion 判断当前处理人员，可否处理下一步工作.

                    string msgOfSend = this.HisNode.TurnToDealDoc;
                    if (msgOfSend.Contains("@"))
                    {
                        Attrs attrs = this.HisWork.EnMap.Attrs;
                        foreach (Attr attr in attrs)
                        {
                            if (msgOfSend.Contains("@") == false)
                                continue;
                            msgOfSend = msgOfSend.Replace("@" + attr.Key, this.HisWork.GetValStrByKey(attr.Key));
                        }
                    }

                    if (msgOfSend.Contains("@") == true)
                    {
                        /*说明有一些变量在系统运行里面.*/
                        string msgOfSendText = msgOfSend.Clone() as string;
                        foreach (SendReturnObj item in this.HisMsgObjs)
                        {
                            if (DataType.IsNullOrEmpty(item.MsgFlag))
                                continue;

                            if (msgOfSend.Contains("@") == false)
                                break;

                            msgOfSendText = msgOfSendText.Replace("@" + item.MsgFlag, item.MsgOfText);

                            if (item.MsgOfHtml != null)
                                msgOfSend = msgOfSend.Replace("@" + item.MsgFlag, item.MsgOfHtml);
                            else
                                msgOfSend = msgOfSend.Replace("@" + item.MsgFlag, item.MsgOfText);
                        }

                        this.HisMsgObjs.OutMessageHtml = msgOfSend + htmlInfo;
                        this.HisMsgObjs.OutMessageText = msgOfSendText + textInfo;
                    }
                    else
                    {
                        this.HisMsgObjs.OutMessageHtml = msgOfSend;
                        this.HisMsgObjs.OutMessageText = msgOfSend;
                    }

                    //return msgOfSend;
                }
                #endregion 处理发送成功后事件.

                #region 如果需要跳转.
                if (town != null)
                {
                    if (this.town.HisNode.HisRunModel == RunModel.SubThread && this.town.HisNode.HisRunModel == RunModel.SubThread)
                    {
                        this.addMsg(SendReturnMsgFlag.VarToNodeID, town.HisNode.NodeID.ToString(), town.HisNode.NodeID.ToString(), SendReturnMsgType.SystemMsg);
                        this.addMsg(SendReturnMsgFlag.VarToNodeName, town.HisNode.Name, town.HisNode.Name, SendReturnMsgType.SystemMsg);
                    }

#warning 如果这里设置了自动跳转，现在去掉了. 2014-11-07.
                    //if (town.HisNode.HisDeliveryWay == DeliveryWay.ByPreviousOperSkip)
                    //{
                    //    town.NodeSend();
                    //    this.HisMsgObjs = town.HisMsgObjs;
                    //}
                }
                #endregion 如果需要跳转.

                #region 删除已经发送的消息，那些消息已经成为了垃圾信息.
                if (Glo.IsEnableSysMessage == true)
                {
                    Paras ps = new Paras();
                    ps.SQL = "DELETE FROM Sys_SMS WHERE MsgFlag=" + SystemConfig.AppCenterDBVarStr + "MsgFlag";
                    ps.Add("MsgFlag", "WKAlt" + this.HisNode.NodeID + "_" + this.WorkID);
                    BP.DA.DBAccess.RunSQL(ps);
                }
                #endregion

                #region 设置流程的标记.
                if (this.HisNode.IsStartNode)
                {
                    if (this.rptGe.PWorkID != 0 && this.HisGenerWorkFlow.PWorkID == 0)
                    {
                        BP.WF.Dev2Interface.SetParentInfo(this.HisFlow.No, this.WorkID, this.rptGe.PFlowNo, this.rptGe.PWorkID, this.rptGe.PNodeID, this.rptGe.PEmp);

                        //写入track, 调用了父流程.
                        Node pND = new Node(rptGe.PNodeID);
                        fid = 0;
                        if (pND.HisNodeWorkType == NodeWorkType.SubThreadWork)
                        {
                            GenerWorkFlow gwf = new GenerWorkFlow(this.rptGe.PWorkID);
                            fid = gwf.FID;
                        }

                        string paras = "@CFlowNo=" + this.HisFlow.No + "@CWorkID=" + this.WorkID;

                        Glo.AddToTrack(ActionType.StartChildenFlow, rptGe.PFlowNo, rptGe.PWorkID, fid, pND.NodeID, pND.Name,
                            WebUser.No, WebUser.Name,
                            pND.NodeID, pND.Name, WebUser.No, WebUser.Name,
                            "<a href='WFRpt.htm?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "' target=_blank >打开子流程</a>", paras);
                    }
                    else if (SystemConfig.IsBSsystem == true)
                    {
                        /*如果是BS系统*/
                        string pflowNo = BP.Sys.Glo.Request.QueryString["PFlowNo"];
                        if (DataType.IsNullOrEmpty(pflowNo) == false)
                        {
                            string pWorkID = BP.Sys.Glo.Request.QueryString["PWorkID"];
                            string pNodeID = BP.Sys.Glo.Request.QueryString["PNodeID"];
                            string pEmp = BP.Sys.Glo.Request.QueryString["PEmp"];

                            // 设置成父流程关系.
                            BP.WF.Dev2Interface.SetParentInfo(this.HisFlow.No, this.WorkID, pflowNo, Int64.Parse(pWorkID), int.Parse(pNodeID), pEmp);

                            //写入track, 调用了父流程.
                            Node pND = new Node(pNodeID);
                            fid = 0;
                            if (pND.HisNodeWorkType == NodeWorkType.SubThreadWork)
                            {
                                GenerWorkFlow gwf = new GenerWorkFlow(Int64.Parse(pWorkID));
                                fid = gwf.FID;
                            }
                            string paras = "@CFlowNo=" + this.HisFlow.No + "@CWorkID=" + this.WorkID;
                            Glo.AddToTrack(ActionType.StartChildenFlow, pflowNo, Int64.Parse(pWorkID), Int64.Parse(fid.ToString()), pND.NodeID, pND.Name, WebUser.No, WebUser.Name,
                                pND.NodeID, pND.Name, WebUser.No, WebUser.Name,
                                "<a href='WFRpt.htm?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "' target=_blank >打开子流程</a>", paras);
                        }
                    }
                }
                #endregion 设置流程的标记.

                //执行时效考核.
                Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID, this.rptGe.Title);

                #region 触发下一个节点的自动发送, 处理国机的需求.
                if (this.HisMsgObjs.VarToNodeID != null
                    && this.town != null
                    && this.town.HisNode.WhoExeIt != 0)
                {
                    string currUser = BP.Web.WebUser.No;
                    string[] emps = this.HisMsgObjs.VarAcceptersID.Split(',');
                    foreach (string emp in emps)
                    {
                        if (DataType.IsNullOrEmpty(emp))
                            continue;

                        try
                        {
                            //让这个人登录.
                            BP.Port.Emp empEn = new Emp(emp);
                            BP.WF.Dev2Interface.Port_Login(emp);
                            if (this.HisNode.HisRunModel == RunModel.SubThread
                                && this.town.HisNode.HisRunModel != RunModel.SubThread)
                            {
                                /*如果当前的节点是子线程，并且发送到的节点非子线程。
                                 * 就是子线程发送到非子线程的情况。
                                 */
                                this.HisMsgObjs = BP.WF.Dev2Interface.Node_SendWork(this.HisNode.FK_Flow, this.HisWork.FID);
                            }
                            else
                            {
                                this.HisMsgObjs = BP.WF.Dev2Interface.Node_SendWork(this.HisNode.FK_Flow, this.HisWork.OID);
                            }
                        }
                        catch
                        {
                            // 可能是正常的阻挡发送，操作不必提示。
                            //this.HisMsgObjs.AddMsg("Auto"
                        }
                        BP.WF.Dev2Interface.Port_Login(currUser);
                        //使用一个人处理就可以了.
                        break;
                    }
                }
                #endregion 触发下一个节点的自动发送。

                #region 计算未来处理人.
                if (this.town == null)
                {
                    FullSA fsa1 = new FullSA(this);
                }
                else
                {
                    FullSA fsa = new FullSA(this.town);
                }

                #endregion 计算未来处理人.

                #region 判断当前处理人员，可否处理下一步工作.
                if (this.town != null
                    && this.HisRememberMe != null
                    && this.HisRememberMe.Emps.Contains("@" + WebUser.No + "@") == true)
                {
                    string url = "MyFlow.htm?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + town.HisNode.NodeID + "&FID=" + this.rptGe.FID;
                    //    string htmlInfo = "@<a href='" + url + "' >下一步工作您仍然可以处理，点击这里现在处理。</a>.";
                    string textInfo = "@下一步工作您仍然可以处理。";

                    // this.addMsg(SendReturnMsgFlag.MsgOfText, textInfo, null);
                }
                #endregion 判断当前处理人员，可否处理下一步工作.

                //处理事件.
                this.Deal_Event();

                //返回这个对象.
                return this.HisMsgObjs;
            }
            catch (Exception ex)
            {
                this.WhenTranscactionRollbackError(ex);
                DBAccess.DoTransactionRollback();

                throw new Exception(ex.Message);
                //  throw new Exception(ex.Message + "  tech@info:" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        private void Deal_Event()
        {
            #region 处理节点到达事件..
            //执行发送到达事件.
            if (this.town != null)
            {
                string sendSuccess = this.HisFlow.DoFlowEventEntity(EventListOfNode.WorkArrive,
                    this.town.HisNode, this.rptGe, null, null);
            }
            #endregion 处理节点到达事件.

            #region 处理发送成功后事件.
            try
            {
                //调起发送成功后的事件，把参数传入进去。
                if (this.SendHTOfTemp != null)
                {
                    foreach (string key in this.SendHTOfTemp.Keys)
                    {
                        if (rptGe.Row.ContainsKey(key) == true)
                            this.rptGe.Row[key] = this.SendHTOfTemp[key].ToString();
                        else
                            this.rptGe.Row.Add(key, this.SendHTOfTemp[key].ToString());
                    }
                }

                //执行发送.
                string sendSuccess = this.HisFlow.DoFlowEventEntity(EventListOfNode.SendSuccess,
                    this.HisNode, this.rptGe, null, this.HisMsgObjs);

                //string SendSuccess = this.HisNode.MapData.FrmEvents.DoEventNode(EventListOfNode.SendSuccess, this.HisWork);
                if (sendSuccess != null)
                    this.addMsg(SendReturnMsgFlag.SendSuccessMsg, sendSuccess);
            }
            catch (Exception ex)
            {
                this.addMsg(SendReturnMsgFlag.SendSuccessMsgErr, ex.Message);
            }
            #endregion 处理发送成功后事件.
        }

        /// <summary>
        /// 执行业务表数据同步.
        /// </summary>
        private void DTSBTable()
        {

        }
        /// <summary>
        /// 手工的回滚提交失败信息，补偿没有事务的缺陷。
        /// </summary>
        /// <param name="ex"></param>
        private void WhenTranscactionRollbackError(Exception ex)
        {
            /*在提交错误的情况下，回滚数据。*/

            #region 如果是分流点下同表单发送失败再次发送就出现错误
            if (this.town != null
                && this.town.HisNode.HisNodeWorkType == NodeWorkType.SubThreadWork
                && this.town.HisNode.HisSubThreadType == SubThreadType.SameSheet)
            {
                /*如果是子线程*/
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE FID=" + this.WorkID + " AND FK_Node=" + this.town.HisNode.NodeID);
                //删除子线程数据.
                if (BP.DA.DBAccess.IsExitsObject(this.town.HisWork.EnMap.PhysicsTable) == true)
                    DBAccess.RunSQL("DELETE FROM " + this.town.HisWork.EnMap.PhysicsTable + " WHERE FID=" + this.WorkID);
            }
            #endregion 如果是分流点下同表单发送失败再次发送就出现错误

            try
            {
                //删除发生的日志.
                DBAccess.RunSQL("DELETE FROM ND" + int.Parse(this.HisFlow.No) + "Track WHERE WorkID=" + this.WorkID +
                                " AND NDFrom=" + this.HisNode.NodeID + " AND ActionType=" + (int)ActionType.Forward);

                // 删除考核信息。
                this.DealEvalUn();

                // 把工作的状态设置回来。
                if (this.HisNode.IsStartNode)
                {
                    ps = new Paras();
                    ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET WFState=" + (int)WFState.Runing + " WHERE OID=" +
                             dbStr + "OID ";
                    ps.Add(GERptAttr.OID, this.WorkID);
                    DBAccess.RunSQL(ps);
                    //  this.HisWork.Update(GERptAttr.WFState, (int)WFState.Runing);
                }

                // 把流程的状态设置回来。
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = this.WorkID;
                if (gwf.RetrieveFromDBSources() == 0)
                    return;

                if (gwf.WFState != 0 || gwf.FK_Node != this.HisNode.NodeID)
                {
                    /* 如果这两项其中有一项有变化。*/
                    gwf.FK_Node = this.HisNode.NodeID;
                    gwf.NodeName = this.HisNode.Name;
                    gwf.WFState = WFState.Runing;
                    this.HisGenerWorkFlow.Sender = BP.WF.Glo.DealUserInfoShowModel(oldSender, oldSender);
                    gwf.Update();
                }

                //执行数据.
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE FK_Emp=" + dbStr + "FK_Emp AND WorkID=" + dbStr +
                         "WorkID AND FK_Node=" + dbStr + "FK_Node ";
                ps.AddFK_Emp();
                ps.Add("WorkID", this.WorkID);
                ps.Add("FK_Node", this.HisNode.NodeID);
                DBAccess.RunSQL(ps);

                Node startND = this.HisNode.HisFlow.HisStartNode;
                StartWork wk = startND.HisWork as StartWork;
                switch (startND.HisNodeWorkType)
                {
                    case NodeWorkType.StartWorkFL:
                    case NodeWorkType.WorkFL:
                        break;
                    default:
                        /*
                         要考虑删除WFState 节点字段的问题。
                         */
                        //// 把开始节点的装态设置回来。
                        //DBAccess.RunSQL("UPDATE " + wk.EnMap.PhysicsTable + " SET WFState=0 WHERE OID="+this.WorkID+" OR OID="+this);
                        //wk.OID = this.WorkID;
                        //int i =wk.RetrieveFromDBSources();
                        //if (wk.WFState == WFState.Complete)
                        //{
                        //    wk.Update("WFState", (int)WFState.Runing);
                        //}
                        break;
                }
                Nodes nds = this.HisNode.HisToNodes;
                foreach (Node nd in nds)
                {
                    if (nd.NodeID == this.HisNode.NodeID)
                        continue;

                    Work mwk = nd.HisWork;
                    if (mwk.EnMap.PhysicsTable == this.HisFlow.PTable
                        || mwk.EnMap.PhysicsTable == this.HisWork.EnMap.PhysicsTable)
                        continue;

                    mwk.OID = this.WorkID;
                    try
                    {
                        mwk.DirectDelete();
                    }
                    catch
                    {
                        mwk.CheckPhysicsTable();
                        mwk.DirectDelete();
                    }
                }
                this.HisFlow.DoFlowEventEntity(EventListOfNode.SendError, this.HisNode, this.rptGe, null);

            }
            catch (Exception ex1)
            {
                if (this.town != null && this.town.HisWork != null)
                    this.town.HisWork.CheckPhysicsTable();

                if (this.rptGe != null)
                    this.rptGe.CheckPhysicsTable();
                throw new Exception(ex.Message + "@回滚发送失败数据出现错误：" + ex1.StackTrace + "@有可能系统已经自动修复错误，请您在重新执行一次。");
            }
        }
        #endregion

        #region 用户到的变量
        public GenerWorkerLists HisWorkerLists = null;
        private GenerWorkFlow _HisGenerWorkFlow;
        public GenerWorkFlow HisGenerWorkFlow
        {
            get
            {
                if (_HisGenerWorkFlow == null)
                {
                    _HisGenerWorkFlow = new GenerWorkFlow(this.WorkID);
                    SendNodeWFState = _HisGenerWorkFlow.WFState; //设置发送前的节点状态。
                }
                return _HisGenerWorkFlow;
            }
            set
            {
                _HisGenerWorkFlow = value;
            }
        }
        private Int64 _WorkID = 0;
        /// <summary>
        /// 工作ID.
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return _WorkID;
            }
            set
            {
                _WorkID = value;
            }
        }
        /// <summary>
        /// 原来的发送人.
        /// </summary>
        private string oldSender = null;
        #endregion


        public GERpt rptGe = null;
        private void InitStartWorkDataV2()
        {
            /*如果是开始流程判断是不是被吊起的流程，如果是就要向父流程写日志。*/
            if (SystemConfig.IsBSsystem)
            {
                string fk_nodeFrom = BP.Sys.Glo.Request.QueryString["FromNode"];
                if (DataType.IsNullOrEmpty(fk_nodeFrom) == false)
                {
                    Node ndFrom = new Node(int.Parse(fk_nodeFrom));
                    string PWorkID = BP.Sys.Glo.Request.QueryString["PWorkID"];
                    if (DataType.IsNullOrEmpty(PWorkID))
                        PWorkID = BP.Sys.Glo.Request.QueryString["PWorkID"];

                    string pTitle = DBAccess.RunSQLReturnStringIsNull("SELECT Title FROM  ND" + int.Parse(ndFrom.FK_Flow) + "01 WHERE OID=" + PWorkID, "");

                    ////记录当前流程被调起。
                    //  this.AddToTrack(ActionType.StartSubFlow, WebUser.No,
                    //  WebUser.Name, ndFrom.NodeID, ndFrom.FlowName + "\t\n" + ndFrom.FlowName, "被父流程(" + ndFrom.FlowName + ":" + pTitle + ")调起.");

                    //记录父流程被调起。
                    BP.WF.Dev2Interface.WriteTrack(this.HisFlow.No, this.HisNode.NodeID, this.HisNode.Name, this.WorkID, 0,
                        "被{" + ndFrom.FlowName + "}发起,发起人:" + this.ExecerName, ActionType.CallChildenFlow,
                        "@PWorkID=" + PWorkID + "@PFlowNo=" + ndFrom.HisFlow.No, "发起子流程:" + this.HisFlow.Name, null);
                }
            }

            /* 产生开始工作流程记录. */
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = this.HisWork.OID;
            int srcNum = gwf.RetrieveFromDBSources();
            if (srcNum == 0)
            {
                gwf.WFState = WFState.Runing;
            }
            else
            {
                if (gwf.WFState == WFState.Blank)
                    gwf.WFState = WFState.Runing;

                SendNodeWFState = gwf.WFState; //设置发送前的节点状态。
            }

            #region 设置流程标题.
            if (this.title == null)
            {
                if (this.HisFlow.TitleRole == "@OutPara" || DataType.IsNullOrEmpty(this.HisFlow.TitleRole) == true)
                {
                    /*如果是外部参数,*/
                    gwf.Title = DBAccess.RunSQLReturnString("SELECT Title FROM " + this.HisFlow.PTable + " WHERE OID=" + this.WorkID);
                    if (DataType.IsNullOrEmpty(gwf.Title))
                        gwf.Title = this.Execer + "," + this.ExecerName + "在:" + DataType.CurrentDataTimeCN + "发起.";
                    //throw new Exception("您设置的流程标题生成规则为外部传来的参数，但是您岋料在创建空白工作时，流程标题值为空。");
                }
                else
                {
                    gwf.Title = BP.WF.WorkFlowBuessRole.GenerTitle(this.HisFlow, this.HisWork);
                }
            }
            else
            {
                gwf.Title = this.title;
            }

            //流程标题.
            this.rptGe.Title = gwf.Title;
            #endregion 设置流程标题.

            if (DataType.IsNullOrEmpty(rptGe.BillNo))
            {
                //处理单据编号.
                string billNoTemplate = this.HisFlow.BillNoFormat.Clone() as string;
                if (DataType.IsNullOrEmpty(billNoTemplate) == false)
                {
                    string billNo = BP.WF.WorkFlowBuessRole.GenerBillNo(billNoTemplate, this.WorkID, this.rptGe, this.HisFlow.PTable);
                    gwf.BillNo = billNo;
                    this.rptGe.BillNo = billNo;
                }
            }
            else
            {
                gwf.BillNo = rptGe.BillNo;
            }

            this.HisWork.SetValByKey("Title", gwf.Title);
            gwf.RDT = DataType.CurrentDataTime;  // this.HisWork.RDT;
            gwf.Starter = this.Execer;
            gwf.StarterName = this.ExecerName;
            gwf.FK_Flow = this.HisNode.FK_Flow;
            gwf.FlowName = this.HisNode.FlowName;
            gwf.FK_FlowSort = this.HisNode.HisFlow.FK_FlowSort;
            gwf.SysType = this.HisNode.HisFlow.SysType;
            gwf.FK_Node = this.HisNode.NodeID;
            gwf.NodeName = this.HisNode.Name;
            gwf.FK_Dept = this.HisWork.RecOfEmp.FK_Dept;
            gwf.DeptName = this.HisWork.RecOfEmp.FK_DeptText;

            if (this.HisFlow.HisTimelineRole == TimelineRole.ByFlow)
            {
                try
                {
                    gwf.SDTOfFlow = this.HisWork.GetValStrByKey(WorkSysFieldAttr.SysSDTOfFlow);
                }
                catch (Exception ex)
                {
                    Log.DefaultLogWriteLineError("可能是流程设计错误,获取开始节点{" + gwf.Title + "}的整体流程应完成时间有错误,是否包含SysSDTOfFlow字段? 异常信息:" + ex.Message);
                    /*获取开始节点的整体流程应完成时间有错误,是否包含SysSDTOfFlow字段? .*/
                    if (this.HisWork.EnMap.Attrs.Contains(WorkSysFieldAttr.SysSDTOfFlow) == false)
                        throw new Exception("流程设计错误，您设置的流程时效属性是｛按开始节点表单SysSDTOfFlow字段计算｝，但是开始节点表单不包含字段 SysSDTOfFlow , 系统错误信息:" + ex.Message);
                    throw new Exception("初始化开始节点数据错误:" + ex.Message);
                }
            }

            //加入两个参数. 2013-02-17
            if (gwf.PWorkID != 0)
            {
                this.rptGe.PWorkID = gwf.PWorkID;
                this.rptGe.PFlowNo = gwf.PFlowNo;
                this.rptGe.PNodeID = gwf.PNodeID;
                this.rptGe.PEmp = gwf.PEmp;
            }
            else
            {
                try
                {
                    gwf.PWorkID = this.rptGe.PWorkID;
                }
                catch (Exception e)
                {
                    gwf.PWorkID = 0;
                }

                try
                {
                    gwf.PFlowNo = this.rptGe.PFlowNo;
                }
                catch (Exception e)
                {
                    gwf.PFlowNo = "";
                }
                try
                {
                    gwf.PNodeID = this.rptGe.PNodeID;
                }
                catch (Exception e)
                {
                    gwf.PNodeID = 0;
                }


                try
                {
                    gwf.PEmp = this.rptGe.PEmp;
                }
                catch (Exception e)
                {
                    gwf.PEmp = WebUser.No;
                }
            }

            // 生成FlowNote
            string note = this.HisFlow.FlowNoteExp.Clone() as string;
            if (DataType.IsNullOrEmpty(note) == false)
                note = BP.WF.Glo.DealExp(note, this.rptGe, null);
            this.rptGe.FlowNote = note;
            gwf.FlowNote = note;


            if (srcNum == 0)
                gwf.DirectInsert();
            else
                gwf.DirectUpdate();

            StartWork sw = (StartWork)this.HisWork;

            #region 设置  HisGenerWorkFlow

            this.HisGenerWorkFlow = gwf;

            #endregion HisCHOfFlow

            #region  产生开始工作者,能够执行他们的人员.
            GenerWorkerList wl = new GenerWorkerList();
            wl.WorkID = this.HisWork.OID;
            wl.FK_Node = this.HisNode.NodeID;
            wl.FK_Emp = this.Execer;
            wl.Delete();

            wl.FK_NodeText = this.HisNode.Name;
            wl.FK_EmpText = this.ExecerName;
            wl.FK_Flow = this.HisNode.FK_Flow;
            wl.FK_Dept = this.ExecerDeptNo;
            // wl.WarningHour = this.HisNode.WarningHour;
            wl.SDT = "无";
            wl.DTOfWarning = DataType.CurrentDataTime;

            try
            {
                wl.Save();
            }
            catch
            {
                wl.CheckPhysicsTable();
                wl.Update();
            }
            #endregion

            this.rptGe.FlowStartRDT = DataType.CurrentDataTime;
            this.rptGe.FlowEnderRDT = DataType.CurrentDataTime;
        }

        /// <summary>
        /// 执行将当前工作节点的数据copy到Rpt里面去.
        /// </summary>
        public void DoCopyCurrentWorkDataToRpt()
        {
            /* 如果两个表一致就返回..*/
            // 把当前的工作人员增加里面去.
            string str = rptGe.GetValStrByKey(GERptAttr.FlowEmps);
            if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDOnly)
            {
                if (str.Contains("@" + this.Execer) == false)
                    rptGe.SetValByKey(GERptAttr.FlowEmps, str + "@" + this.Execer);
            }

            if (Glo.UserInfoShowModel == UserInfoShowModel.UserNameOnly)
            {
                if (str.Contains("@" + WebUser.Name) == false)
                    rptGe.SetValByKey(GERptAttr.FlowEmps, str + "@" + this.ExecerName);
            }

            if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
            {
                if (str.Contains("@" + this.Execer + "," + this.ExecerName) == false)
                    rptGe.SetValByKey(GERptAttr.FlowEmps, str + "@" + this.Execer + "," + this.ExecerName);
            }

            rptGe.FlowEnder = this.Execer;
            rptGe.FlowEnderRDT = DataType.CurrentDataTime;

            //设置当前的流程所有的用时.
            rptGe.FlowDaySpan = DataType.GeTimeLimits(this.rptGe.GetValStringByKey(GERptAttr.FlowStartRDT), DataType.CurrentDataTime);

            if (this.HisNode.IsEndNode || this.IsStopFlow)
                rptGe.WFState = WFState.Complete;
            else
                rptGe.WFState = WFState.Runing;

            if (this.HisWork.EnMap.PhysicsTable == this.HisFlow.PTable)
            {
                rptGe.DirectUpdate();
            }
            else
            {
                /*将当前的属性复制到rpt表里面去.*/
                DoCopyRptWork(this.HisWork);
                rptGe.DirectUpdate();
            }
        }
        /// <summary>
        /// 执行数据copy.
        /// </summary>
        /// <param name="fromWK"></param>
        public void DoCopyRptWork(Work fromWK)
        {
            foreach (Attr attr in fromWK.EnMap.Attrs)
            {
                switch (attr.Key)
                {
                    case BP.WF.Data.GERptAttr.FK_NY:
                    case BP.WF.Data.GERptAttr.FK_Dept:
                    case BP.WF.Data.GERptAttr.FlowDaySpan:
                    case BP.WF.Data.GERptAttr.FlowEmps:
                    case BP.WF.Data.GERptAttr.FlowEnder:
                    case BP.WF.Data.GERptAttr.FlowEnderRDT:
                    case BP.WF.Data.GERptAttr.FlowEndNode:
                    case BP.WF.Data.GERptAttr.FlowStarter:
                    case BP.WF.Data.GERptAttr.Title:
                    case BP.WF.Data.GERptAttr.WFSta:
                        continue;
                    default:
                        break;
                }

                object obj = fromWK.GetValByKey(attr.Key);
                if (obj == null)
                    continue;
                this.rptGe.SetValByKey(attr.Key, obj);
            }
            if (this.HisNode.IsStartNode)
                this.rptGe.SetValByKey("Title", fromWK.GetValByKey("Title"));
        }
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="at">类型</param>
        /// <param name="toEmp">到人员</param>
        /// <param name="toEmpName">到人员名称</param>
        /// <param name="toNDid">到节点</param>
        /// <param name="toNDName">到节点名称</param>
        /// <param name="msg">消息</param>
        public void AddToTrack(ActionType at, string toEmp, string toEmpName, int toNDid, string toNDName, string msg)
        {
            AddToTrack(at, toEmp, toEmpName, toNDid, toNDName, msg, this.HisNode);
        }
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="at"></param>
        /// <param name="gwl"></param>
        /// <param name="msg"></param>
        public void AddToTrack(ActionType at, GenerWorkerList gwl, string msg, Int64 subTreadWorkID)
        {
            Track t = new Track();

            if (this.HisGenerWorkFlow.FID == 0)
            {
                t.WorkID = subTreadWorkID;
                t.FID = this.HisWork.OID;
            }
            else
            {
                t.WorkID = this.HisWork.OID;
                t.FID = this.HisGenerWorkFlow.FID;
            }

            t.RDT = DataType.CurrentDataTimess;
            t.HisActionType = at;

            t.NDFrom = ndFrom.NodeID;
            t.NDFromT = ndFrom.Name;

            t.EmpFrom = this.Execer;
            t.EmpFromT = this.ExecerName;
            t.FK_Flow = this.HisNode.FK_Flow;

            //     t.Tag = tag + "@SendNode=" + this.HisNode.NodeID;

            t.NDTo = gwl.FK_Node;
            t.NDToT = gwl.FK_NodeText;

            t.EmpTo = gwl.FK_Emp;
            t.EmpToT = gwl.FK_EmpText;
            t.Msg = msg;
            //t.FrmDB = frmDBJson; //表单数据Json.

            switch (at)
            {
                case ActionType.Forward:
                case ActionType.ForwardAskfor:
                case ActionType.Start:
                case ActionType.UnSend:
                case ActionType.ForwardFL:
                case ActionType.ForwardHL:
                case ActionType.TeampUp:
                    //判断是否有焦点字段，如果有就把它记录到日志里。
                    if (this.HisNode.FocusField.Length > 1)
                    {
                        string exp = this.HisNode.FocusField;
                        if (this.rptGe != null)
                            exp = Glo.DealExp(exp, this.rptGe, null);
                        else
                            exp = Glo.DealExp(exp, this.HisWork, null);

                        t.Msg += exp;
                        if (t.Msg.Contains("@"))
                            Log.DebugWriteError("@在节点(" + this.HisNode.NodeID + " ， " + this.HisNode.Name + ")焦点字段被删除了,表达式为:" + this.HisNode.FocusField + " 替换的结果为:" + t.Msg);
                    }
                    break;
                default:
                    break;
            }


            if (at == ActionType.SubThreadForward
                || at == ActionType.StartChildenFlow
                || at == ActionType.Start
                || at == ActionType.Forward
                || at == ActionType.SubThreadForward
                || at == ActionType.ForwardHL
                || at == ActionType.FlowOver)
            {
                if (this.HisNode.IsFL)
                    at = ActionType.ForwardFL;
                t.FrmDB = this.HisWork.ToJson();
            }

            try
            {
                // t.MyPK = t.WorkID + "_" + t.FID + "_"  + t.NDFrom + "_" + t.NDTo +"_"+t.EmpFrom+"_"+t.EmpTo+"_"+ DateTime.Now.ToString("yyMMddHHmmss");
                t.Insert();
            }
            catch
            {
                t.CheckPhysicsTable();
                t.Insert();
            }

            if (at == ActionType.SubThreadForward
              || at == ActionType.StartChildenFlow
              || at == ActionType.Start
              || at == ActionType.Forward
              || at == ActionType.SubThreadForward
              || at == ActionType.ForwardHL
              || at == ActionType.FlowOver)
            {
                this.HisGenerWorkFlow.Paras_LastSendTruckID = t.MyPK;
            }
        }
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="at">类型</param>
        /// <param name="toEmp">到人员</param>
        /// <param name="toEmpName">到人员名称</param>
        /// <param name="toNDid">到节点</param>
        /// <param name="toNDName">到节点名称</param>
        /// <param name="msg">消息</param>
        public void AddToTrack(ActionType at, string toEmp, string toEmpName, int toNDid, string toNDName, string msg, Node ndFrom, string frmDBJson = null, string tag = null)
        {
            Track t = new Track();

            t.WorkID = this.HisWork.OID;
            t.FID = this.HisWork.FID;

            t.RDT = DataType.CurrentDataTimess;
            t.HisActionType = at;

            t.NDFrom = ndFrom.NodeID;
            t.NDFromT = ndFrom.Name;

            t.EmpFrom = this.Execer;
            t.EmpFromT = this.ExecerName;
            t.FK_Flow = this.HisNode.FK_Flow;
            t.Tag = tag + "@SendNode=" + this.HisNode.NodeID;

            if (toNDid == 0)
            {
                toNDid = this.HisNode.NodeID;
                toNDName = this.HisNode.Name;
            }


            t.NDTo = toNDid;
            t.NDToT = toNDName;

            t.EmpTo = toEmp;
            t.EmpToT = toEmpName;
            t.Msg = msg;
            t.FrmDB = frmDBJson; //表单数据Json.

            switch (at)
            {
                case ActionType.Forward:
                case ActionType.ForwardAskfor:
                case ActionType.Start:
                case ActionType.UnSend:
                case ActionType.ForwardFL:
                case ActionType.ForwardHL:
                case ActionType.TeampUp:
                    //判断是否有焦点字段，如果有就把它记录到日志里。
                    if (this.HisNode.FocusField.Length > 1)
                    {
                        string exp = this.HisNode.FocusField;
                        if (this.rptGe != null)
                            exp = Glo.DealExp(exp, this.rptGe, null);
                        else
                            exp = Glo.DealExp(exp, this.HisWork, null);

                        t.Msg += exp;
                        if (t.Msg.Contains("@"))
                            Log.DebugWriteError("@在节点(" + this.HisNode.NodeID + " ， " + this.HisNode.Name + ")焦点字段被删除了,表达式为:" + this.HisNode.FocusField + " 替换的结果为:" + t.Msg);
                    }
                    break;
                default:
                    break;
            }


            if (at == ActionType.SubThreadForward
                || at == ActionType.StartChildenFlow
                || at == ActionType.Start
                || at == ActionType.Forward
                || at == ActionType.SubThreadForward
                || at == ActionType.ForwardHL
                || at == ActionType.FlowOver)
            {
                if (this.HisNode.IsFL)
                    at = ActionType.ForwardFL;
                t.FrmDB = this.HisWork.ToJson();
            }

            try
            {
                // t.MyPK = t.WorkID + "_" + t.FID + "_"  + t.NDFrom + "_" + t.NDTo +"_"+t.EmpFrom+"_"+t.EmpTo+"_"+ DateTime.Now.ToString("yyMMddHHmmss");
                t.Insert();
            }
            catch
            {
                t.CheckPhysicsTable();
            }

            if (at == ActionType.SubThreadForward
              || at == ActionType.StartChildenFlow
              || at == ActionType.Start
              || at == ActionType.Forward
              || at == ActionType.SubThreadForward
              || at == ActionType.ForwardHL
              || at == ActionType.FlowOver)
            {
                this.HisGenerWorkFlow.Paras_LastSendTruckID = t.MyPK;
            }
        }
        /// <summary>
        /// 向他们发送消息
        /// </summary>
        /// <param name="gwls">接受人</param>
        public void SendMsgToThem(GenerWorkerLists gwls)
        {
            //if (BP.WF.Glo.IsEnableSysMessage == false)
            //    return;

            //求到达人员的IDs
            string toEmps = "";
            foreach (GenerWorkerList gwl in gwls)
            {
                toEmps += gwl.FK_Emp + ",";
            }

            //处理工作到达事件.
            PushMsgs pms = this.town.HisNode.HisPushMsgs;
            foreach (PushMsg pm in pms)
            {
                if (pm.FK_Event != EventListOfNode.WorkArrive)
                    continue;

                string msg = pm.DoSendMessage(this.town.HisNode, this.town.HisWork, null, null, null, toEmps);
                this.addMsg("alert" + pm.MyPK, msg, msg, SendReturnMsgType.Info);
                // this.addMsg(SendReturnMsgFlag.SendSuccessMsg, "已经转给，加签的发起人(" + item.FK_Emp + "," + item.FK_EmpText + ")", SendReturnMsgType.Info);
            }
            return;
        }
        /// <summary>
        /// 发送前的流程状态。
        /// </summary>
        private WFState SendNodeWFState = WFState.Blank;
        /// <summary>
        /// 合流节点是否全部完成？
        /// </summary>
        private bool IsOverMGECheckStand = false;
        private bool _IsStopFlow = false;
        private bool IsStopFlow
        {
            get
            {
                return _IsStopFlow;
            }
            set
            {
                _IsStopFlow = value;
                if (_IsStopFlow == true)
                {
                    if (this.rptGe != null)
                    {
                        this.rptGe.WFState = WFState.Complete;
                        this.rptGe.Update("WFState", (int)WFState.Complete);
                    }
                }
            }
        }
        /// <summary>
        /// 检查
        /// </summary>
        private void CheckCompleteCondition_IntCompleteEmps()
        {
            string sql = "SELECT FK_Emp,FK_EmpText FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " AND IsPass=1";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            string emps = "@";
            string flowEmps = "@";
            foreach (DataRow dr in dt.Rows)
            {
                if (emps.Contains("@" + dr[0].ToString() + "@"))
                    continue;

                emps = emps + dr[0].ToString() + "@";
                flowEmps = flowEmps + dr[1] + "," + dr[0].ToString() + "@";
            }
            //追加当前操作人
            if (emps.Contains("@" + WebUser.No + "@") == false)
            {
                emps = emps + WebUser.No + "@";
                flowEmps = flowEmps + WebUser.Name + "," + WebUser.No + "@";
            }
            // 给他们赋值.
            this.rptGe.FlowEmps = flowEmps;
            this.HisGenerWorkFlow.Emps = emps;
        }
        /// <summary>
        /// 处理自动运行 - 预先设置未来的运行节点.
        /// </summary>
        private void DealAutoRunEnable()
        {
            //检查当前节点是否是自动运行的.
            if (this.HisNode.AutoRunEnable == false)
                return;

            /*如果是自动运行就要设置自动运行参数.*/
            string exp = this.HisNode.AutoRunParas.Clone() as string;
            if (exp == null || exp == "")
                throw new Exception("@您设置当前是自动运行，但是没有在该节点上设置参数。");

            exp = exp.Replace("@OID", this.WorkID.ToString());
            exp = exp.Replace("@WorkID", this.WorkID.ToString());

            exp = exp.Replace("@NodeID", this.HisNode.NodeID.ToString());
            exp = exp.Replace("@FK_Node", this.HisNode.NodeID.ToString());

            exp = exp.Replace("@WebUser.No", BP.Web.WebUser.No);
            exp = exp.Replace("@WebUser.Name", BP.Web.WebUser.Name);
            exp = exp.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
            exp = exp.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);

            if (exp.Contains("@") == true)
                exp = Glo.DealExp(exp, this.HisWork, null);

            if (exp.Contains("@") == true)
                throw new Exception("@您配置的表达式没有没被完全的解析下来:" + exp);

            //没有查询到就不设置了.
            string strs = DBAccess.RunSQLReturnStringIsNull(exp, null);
            if (strs == null)
                return;

            //把约定的参数写入到引擎
            Dev2Interface.Flow_SetFlowTransferCustom(this.HisFlow.No, this.WorkID,
                BP.WF.TransferCustomType.ByWorkerSet, strs);

            //重新执行查询.
            this.HisGenerWorkFlow.RetrieveFromDBSources();
        }
        /// <summary>
        /// 检查流程、节点的完成条件
        /// </summary>
        /// <returns></returns>
        private void CheckCompleteCondition()
        {
            // 执行初始化人员.
            this.CheckCompleteCondition_IntCompleteEmps();

            // 如果结束流程，就增加如下信息 翻译.
            this.HisGenerWorkFlow.Sender = BP.Web.WebUser.No;
            this.HisGenerWorkFlow.SendDT = DataType.CurrentDataTime;

            this.rptGe.FlowEnder = BP.Web.WebUser.No;
            this.rptGe.FlowEnderRDT = DataType.CurrentDataTime;

            this.IsStopFlow = false;
            if (this.HisNode.IsEndNode)
            {
                /* 如果流程完成 */
                //   CCWork cc = new CCWork(this);
                // 在流程完成锁前处理消息收听，否则WF_GenerWorkerlist就删除了。

                if (this.HisGenerWorkFlow.TransferCustomType == TransferCustomType.ByCCBPMDefine)
                {
                    this.IsStopFlow = true;
                    this.HisGenerWorkFlow.WFState = WFState.Complete;
                    this.rptGe.WFState = WFState.Complete;

                    string msg = this.HisWorkFlow.DoFlowOver(ActionType.FlowOver, "流程已经走到最后一个节点，流程成功结束。", this.HisNode, this.rptGe);
                    this.addMsg(SendReturnMsgFlag.End, msg);
                }

                return;
            }

            #region 判断节点完成条件
            this.addMsg(SendReturnMsgFlag.OverCurr, string.Format("当前工作[{0}]已经完成", this.HisNode.Name));
            #endregion

            #region 判断流程条件.
            try
            {
                if (this.HisNode.HisToNodes.Count == 0 && this.HisNode.IsStartNode)
                {
                    // 在流程完成锁前处理消息收听，否则WF_GenerWorkerlist就删除了。

                    /* 如果流程完成 */
                    this.HisWorkFlow.DoFlowOver(ActionType.FlowOver, "符合流程完成条件", this.HisNode, this.rptGe);
                    this.IsStopFlow = true;
                    this.addMsg(SendReturnMsgFlag.OneNodeSheetver, "工作已经成功处理(一个流程的工作)。",
                        "工作已经成功处理(一个流程的工作)。 @查看<img src='./Img/Btn/PrintWorkRpt.gif' >", SendReturnMsgType.Info);
                    return;
                }

                if (this.HisNode.IsCCFlow && this.HisFlowCompleteConditions.IsPass)
                {
                    string stopMsg = this.HisFlowCompleteConditions.ConditionDesc;
                    /* 如果流程完成 */
                    string overMsg = this.HisWorkFlow.DoFlowOver(ActionType.FlowOver, "符合流程完成条件:" + stopMsg, this.HisNode, this.rptGe);
                    this.IsStopFlow = true;

                    // string path = BP.Sys.Glo.Request.ApplicationPath;
                    this.addMsg(SendReturnMsgFlag.MacthFlowOver, "@符合工作流程完成条件" + stopMsg + "" + overMsg,
                        "@符合工作流程完成条件" + stopMsg + "" + overMsg, SendReturnMsgType.Info);
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("@判断流程{0}完成条件出现错误." + ex.StackTrace, this.HisNode.Name));
            }
            #endregion

        }

        #region 启动多个节点
        /// <summary>
        /// 生成为什么发送给他们
        /// </summary>
        /// <param name="fNodeID"></param>
        /// <param name="toNodeID"></param>
        /// <returns></returns>
        public string GenerWhySendToThem(int fNodeID, int toNodeID)
        {
            return "";
            //return "@<a href='WhySendToThem.aspx?NodeID=" + fNodeID + "&ToNodeID=" + toNodeID + "&WorkID=" + this.WorkID + "' target=_blank >" + this.ToE("WN20", "为什么要发送给他们？") + "</a>";
        }
        /// <summary>
        /// 工作流程ID
        /// </summary>
        public static Int64 FID = 0;
        /// <summary>
        /// 没有FID
        /// </summary>
        /// <param name="nd"></param>
        /// <returns></returns>
        private string StartNextWorkNodeHeLiu_WithOutFID(Node nd)
        {
            throw new Exception("未完成:StartNextWorkNodeHeLiu_WithOutFID");
        }
        /// <summary>
        /// 异表单子线程向合流点运动
        /// </summary>
        /// <param name="nd"></param>
        private void NodeSend_53_UnSameSheet_To_HeLiu(Node nd)
        {

            Work heLiuWK = nd.HisWork;
            heLiuWK.OID = this.HisWork.FID;
            if (heLiuWK.RetrieveFromDBSources() == 0) //查询出来数据.
                heLiuWK.DirectInsert();

            heLiuWK.Copy(this.HisWork); // 执行copy.

            heLiuWK.OID = this.HisWork.FID;
            heLiuWK.FID = 0;

            this.town = new WorkNode(heLiuWK, nd);

            //合流节点上的工作处理者。
            GenerWorkerLists gwls = new GenerWorkerLists(this.HisWork.FID, nd.NodeID);
            current_gwls = gwls;

            if (gwls.Count == 0)
            {
                // 说明第一次到达河流节点。
                current_gwls = this.Func_GenerWorkerLists(this.town);
                gwls = current_gwls;

                GenerWorkFlow gwf = new GenerWorkFlow(this.HisWork.FID);
                gwf.FK_Node = nd.NodeID;
                gwf.NodeName = nd.Name;
                gwf.TodoEmpsNum = gwls.Count;

                string todoEmps = "";
                foreach (GenerWorkerList item in gwls)
                    todoEmps += item.FK_Emp + "," + item.FK_EmpText + ";";

                gwf.TodoEmps = todoEmps;
                gwf.WFState = WFState.Runing;
                gwf.Update();
            }

            string FK_Emp = "";
            string toEmpsStr = "";
            string emps = "";
            foreach (GenerWorkerList wl in gwls)
            {
                toEmpsStr += BP.WF.Glo.DealUserInfoShowModel(wl.FK_Emp, wl.FK_EmpText);
                if (gwls.Count == 1)
                    emps = toEmpsStr;
                else
                    emps += "@" + toEmpsStr;
            }


            /* 
            * 更新它的节点 worklist 信息, 说明当前节点已经完成了.
            * 不让当前的操作员能看到自己的工作。
            */
            #region 处理合流节点表单数据。


            #region 复制主表数据. edit 2014-11-20 向合流点汇总数据.
            //复制当前节点表单数据.
            heLiuWK.FID = 0;
            heLiuWK.Rec = FK_Emp;
            heLiuWK.Emps = emps;
            heLiuWK.OID = this.HisWork.FID;
            heLiuWK.DirectUpdate(); //在更新一次.

            /* 把数据复制到rpt数据表里. */
            this.rptGe.OID = this.HisWork.FID;
            this.rptGe.RetrieveFromDBSources();
            this.rptGe.Copy(this.HisWork);
            this.rptGe.DirectUpdate();
            #endregion 复制主表数据.

            // 产生合流汇总明细表数据.
            //this.GenerHieLiuHuiZhongDtlData_2013(nd);

            #endregion 处理合流节点表单数据
            /* 合流点需要等待各个分流点全部处理完后才能看到它。*/
            string info = "";
            string sql1 = "";
#warning 对于多个分合流点可能会有问题。
            ps = new Paras();
            ps.SQL = "SELECT COUNT(distinct WorkID) AS Num FROM WF_GenerWorkerList WHERE  FID=" + dbStr + "FID AND FK_Node IN (" + this.SpanSubTheadNodes(nd) + ")";
            ps.Add("FID", this.HisWork.FID);
            decimal numAll1 = (decimal)DBAccess.RunSQLReturnValInt(ps);
            decimal passRate1 = 1 / numAll1 * 100;
            if (nd.PassRate <= passRate1)
            {
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=0,FID=0 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID";
                ps.Add("FK_Node", nd.NodeID);
                ps.Add("WorkID", this.HisWork.FID);
                DBAccess.RunSQL(ps);

                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkFlow SET FK_Node=" + dbStr + "FK_Node,NodeName=" + dbStr + "NodeName WHERE WorkID=" + dbStr + "WorkID";
                ps.Add("FK_Node", nd.NodeID);
                ps.Add("NodeName", nd.Name);
                ps.Add("WorkID", this.HisWork.FID);
                DBAccess.RunSQL(ps);

                info = "@下一步合流点(" + nd.Name + ")已经启动。";
            }
            else
            {
#warning 为了不让其显示在途的工作需要， =3 不是正常的处理模式。
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=3,FID=0 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID";
                ps.Add("FK_Node", nd.NodeID);
                ps.Add("WorkID", this.HisWork.OID);
                DBAccess.RunSQL(ps);
            }

            this.HisGenerWorkFlow.FK_Node = nd.NodeID;
            this.HisGenerWorkFlow.NodeName = nd.Name;

            //ps = new Paras();
            //ps.SQL = "UPDATE WF_GenerWorkFlow SET  WFState=" + (int)WFState.Runing + ", FK_Node=" + nd.NodeID + ",NodeName='" + nd.Name + "' WHERE WorkID=" + this.HisWork.FID;
            //ps.Add("FK_Node", nd.NodeID);
            //ps.Add("NodeName", nd.Name);
            //ps.Add("WorkID", this.HisWork.FID);
            //DBAccess.RunSQL(ps);

            this.addMsg(SendReturnMsgFlag.VarAcceptersID, emps, SendReturnMsgType.SystemMsg);
            this.addMsg("HeLiuInfo", "@下一步的工作处理人[" + emps + "]" + info, SendReturnMsgType.Info);
        }
        /// <summary>
        /// 产生合流汇总数据
        /// 把子线程的子表主表数据放到合流点的从表上去
        /// </summary>
        /// <param name="nd"></param>
        private void GenerHieLiuHuiZhongDtlData_2013(Node ndOfHeLiu)
        {
            #region 汇总明细表.
            MapDtls mydtls = ndOfHeLiu.HisWork.HisMapDtls;
            foreach (MapDtl dtl in mydtls)
            {
                if (dtl.IsHLDtl == false)
                    continue;

                GEDtl geDtl = dtl.HisGEDtl;
                geDtl.Copy(this.HisWork);
                geDtl.RefPK = this.HisWork.FID.ToString(); // RefPK 就是当前子线程的FID.
                geDtl.Rec = this.Execer;
                geDtl.RDT = DataType.CurrentDataTime;

                #region 判断是否是质量评价
                if (ndOfHeLiu.IsEval)
                {
                    /*如果是质量评价流程*/
                    geDtl.SetValByKey(WorkSysFieldAttr.EvalEmpNo, this.Execer);
                    geDtl.SetValByKey(WorkSysFieldAttr.EvalEmpName, this.ExecerName);
                    geDtl.SetValByKey(WorkSysFieldAttr.EvalCent, 0);
                    geDtl.SetValByKey(WorkSysFieldAttr.EvalNote, "");
                }
                #endregion

                #region 执行插入数据.
                try
                {
                    geDtl.InsertAsOID(this.HisWork.OID);
                }
                catch
                {
                    geDtl.Update();
                }
                #endregion 执行插入数据.


                #region 还要处理附件的 copy 汇总. 如果子线程上有附件组件.
                if (dtl.IsEnableAthM == true)
                {
                    /*如果启用了多附件。*/
                    //取出来所有的上个节点的数据集合.
                    FrmAttachments athSLs = this.HisWork.HisFrmAttachments;
                    if (athSLs.Count == 0)
                        break; /*子线程上没有附件组件.*/

                    //求子线程的汇总附件集合 (处理如果子线程上有多个附件，其中一部分附件需要汇总另外一部分不需要汇总的模式)
                    string strs = "";
                    foreach (FrmAttachment item in athSLs)
                    {
                        if (item.IsToHeLiuHZ == true)
                            strs += "," + item.MyPK + ",";
                    }

                    //如果没有找到，并且附件集合只有1个，就设置他为子线程的汇总附件，可能是设计人员忘记了设计.
                    if (strs == "" && athSLs.Count == 1)
                    {
                        FrmAttachment athT = athSLs[0] as FrmAttachment;
                        athT.IsToHeLiuHZ = true;
                        athT.Update();
                        strs = "," + athT.MyPK + ",";
                    }

                    // 没有找到要执行的附件.
                    if (strs == "")
                        break;

                    //取出来所有的上个节点的数据集合.
                    FrmAttachmentDBs athDBs = new FrmAttachmentDBs();
                    athDBs.Retrieve(FrmAttachmentDBAttr.FK_MapData, this.HisWork.NodeFrmID,
                        FrmAttachmentDBAttr.RefPKVal, this.HisWork.OID);

                    if (athDBs.Count == 0)
                        break; /*子线程没有上传附件.*/


                    /*说明当前节点有附件数据*/
                    foreach (FrmAttachmentDB athDB in athDBs)
                    {
                        if (strs.Contains("," + athDB.FK_FrmAttachment + ",") == false)
                            continue;

                        FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                        athDB_N.Copy(athDB);
                        athDB_N.FK_MapData = dtl.No;
                        athDB_N.RefPKVal = geDtl.OID.ToString();
                        athDB_N.FK_FrmAttachment = dtl.No + "_AthM";
                        athDB_N.UploadGUID = "";
                        athDB_N.FID = this.HisWork.FID;

                        //生成新的GUID.
                        athDB_N.MyPK = BP.DA.DBAccess.GenerGUID();
                        athDB_N.Insert();
                    }

                }
                #endregion 还要处理附件的copy 汇总.
                break;
            }
            #endregion 汇总明细表.

            #region 复制附件。
            FrmAttachments aths = ndOfHeLiu.HisWork.HisFrmAttachments;  // new FrmAttachments("ND" + this.HisNode.NodeID);
            if (aths.Count == 0)
                return;
            foreach (FrmAttachment ath in aths)
            {
                if (ath.IsHeLiuHuiZong == false)
                    continue; //如果是合流的汇总的多附件数据。

                //取出来所有的上个节点的数据集合.
                FrmAttachments athSLs = this.HisWork.HisFrmAttachments;
                if (athSLs.Count == 0)
                    break; /*子线程上没有附件组件.*/

                //求子线程的汇总附件集合 (处理如果子线程上有多个附件，其中一部分附件需要汇总另外一部分不需要汇总的模式)
                string strs = "";
                foreach (FrmAttachment item in athSLs)
                {
                    if (item.IsToHeLiuHZ == true)
                        strs += "," + item.MyPK + ",";
                }

                //如果没有找到，并且附件集合只有1个，就设置他为子线程的汇总附件，可能是设计人员忘记了设计.
                if (strs == "" && athSLs.Count == 1)
                {
                    FrmAttachment athT = athSLs[0] as FrmAttachment;
                    athT.IsToHeLiuHZ = true;
                    athT.Update();
                    strs = "," + athT.MyPK + ",";
                }

                // 没有找到要执行的附件.
                if (strs == "")
                    break;

                //取出来所有的上个节点的数据集合.
                FrmAttachmentDBs athDBs = new FrmAttachmentDBs();
                athDBs.Retrieve(FrmAttachmentDBAttr.FK_MapData, this.HisWork.NodeFrmID,
                    FrmAttachmentDBAttr.RefPKVal, this.HisWork.OID);

                if (athDBs.Count == 0)
                    break; /*子线程没有上传附件.*/

                /*说明当前节点有附件数据*/
                foreach (FrmAttachmentDB athDB in athDBs)
                {
                    if (strs.Contains("," + athDB.FK_FrmAttachment + ",") == false)
                        continue;

                    //判断是否已经存在附件，避免重复上传
                    FrmAttachmentDB athNDB = new FrmAttachmentDB();
                    int num = athNDB.Retrieve(FrmAttachmentDBAttr.FK_MapData, "ND" + ndOfHeLiu.NodeID, FrmAttachmentDBAttr.RefPKVal, this.HisWork.FID.ToString(), FrmAttachmentDBAttr.UploadGUID, athDB.UploadGUID);
                    if (num > 0)
                        continue;

                    FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                    athDB_N.Copy(athDB);
                    athDB_N.FK_MapData = "ND" + ndOfHeLiu.NodeID;
                    athDB_N.RefPKVal = this.HisWork.FID.ToString();
                    athDB_N.FK_FrmAttachment = ath.MyPK;

                    //生成新的GUID.
                    athDB_N.MyPK = BP.DA.DBAccess.GenerGUID();
                    athDB_N.Insert();
                }
                break;
            }
            #endregion 复制附件。

            #region 复制Ele。
            FrmEleDBs eleDBs = new FrmEleDBs("ND" + this.HisNode.NodeID,
                  this.WorkID.ToString());
            if (eleDBs.Count > 0)
            {
                /*说明当前节点有附件数据*/
                int idx = 0;
                foreach (FrmEleDB eleDB in eleDBs)
                {
                    idx++;
                    FrmEleDB eleDB_N = new FrmEleDB();
                    eleDB_N.Copy(eleDB);
                    eleDB_N.FK_MapData = "ND" + ndOfHeLiu.NodeID;
                    eleDB_N.MyPK = eleDB_N.MyPK.Replace("ND" + this.HisNode.NodeID, "ND" + ndOfHeLiu.NodeID);
                    eleDB_N.RefPKVal = this.HisWork.FID.ToString();
                    eleDB_N.Save();
                }
            }
            #endregion 复制Ele。


        }
        /// <summary>
        /// 子线程节点
        /// </summary>
        private string _SpanSubTheadNodes = null;
        /// <summary>
        /// 获取分流与合流之间的子线程节点集合.
        /// </summary>
        /// <param name="toNode"></param>
        /// <returns></returns>
        private string SpanSubTheadNodes(Node toHLNode)
        {
            _SpanSubTheadNodes = "";
            SpanSubTheadNodes_DiGui(toHLNode.FromNodes);
            if (_SpanSubTheadNodes == "")
                throw new Exception("获取分合流之间的子线程节点集合为空，请检查流程设计，在分合流之间的节点必须设置为子线程节点。");
            _SpanSubTheadNodes = _SpanSubTheadNodes.Substring(1);
            return _SpanSubTheadNodes;

        }
        private void SpanSubTheadNodes_DiGui(Nodes subNDs)
        {
            foreach (Node nd in subNDs)
            {
                if (nd.HisNodeWorkType == NodeWorkType.SubThreadWork)
                {
                    //判断是否已经包含，不然可能死循环
                    if (_SpanSubTheadNodes.Contains("," + nd.NodeID))
                        continue;

                    _SpanSubTheadNodes += "," + nd.NodeID;
                    SpanSubTheadNodes_DiGui(nd.FromNodes);
                }
            }
        }
        #endregion

        #region 基本属性
        /// <summary>
        /// 工作
        /// </summary>
        private Work _HisWork = null;
        /// <summary>
        /// 工作
        /// </summary>
        public Work HisWork
        {
            get
            {
                return this._HisWork;
            }
        }
        /// <summary>
        /// 节点
        /// </summary>
        private Node _HisNode = null;
        /// <summary>
        /// 节点
        /// </summary>
        public Node HisNode
        {
            get
            {
                return this._HisNode;
            }
        }
        private RememberMe HisRememberMe = null;
        public RememberMe GetHisRememberMe(Node nd)
        {
            if (HisRememberMe == null || HisRememberMe.FK_Node != nd.NodeID)
            {
                HisRememberMe = new RememberMe();
                HisRememberMe.FK_Emp = this.Execer;
                HisRememberMe.FK_Node = nd.NodeID;
                HisRememberMe.RetrieveFromDBSources();
            }
            return this.HisRememberMe;
        }
        private WorkFlow _HisWorkFlow = null;
        /// <summary>
        /// 工作流程
        /// </summary>
        public WorkFlow HisWorkFlow
        {
            get
            {
                if (_HisWorkFlow == null)
                    _HisWorkFlow = new WorkFlow(this.HisNode.HisFlow, this.HisWork.OID, this.HisWork.FID);
                return _HisWorkFlow;
            }
        }
        /// <summary>
        /// 当前节点的工作是不是完成。
        /// </summary>
        public bool IsComplete
        {
            get
            {
                if (this.HisGenerWorkFlow.WFState == WFState.Complete)
                    return true;
                else
                    return false;
            }
        }
        public TransferCustom _transferCustom = null;
        public TodolistModel TodolistModel
        {
            get
            {
                //如果当前的节点是按照ccbpm定义的方式运行的，就返回当前节点的多人待办模式，否则就返回自定义的模式。
                if (this.HisGenerWorkFlow.TransferCustomType == TransferCustomType.ByCCBPMDefine)
                    return this.HisNode.TodolistModel;
                return this.HisGenerWorkFlow.TodolistModel;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 建立一个工作节点事例.
        /// </summary>
        /// <param name="workId">工作ID</param>
        /// <param name="nodeId">节点ID</param>
        public WorkNode(Int64 workId, int nodeId)
        {
            this.WorkID = workId;
            Node nd = new Node(nodeId);
            Work wk = nd.HisWork;
            wk.OID = workId;
            int i = wk.RetrieveFromDBSources();
            if (i == 0)
            {
                this.rptGe = nd.HisFlow.HisGERpt;
                if (wk.FID != 0)
                    this.rptGe.OID = wk.FID;
                else
                    this.rptGe.OID = this.WorkID;

                this.rptGe.RetrieveFromDBSources();
                wk.Row = rptGe.Row;
            }
            this._HisWork = wk;
            this._HisNode = nd;
        }
        public Hashtable SendHTOfTemp = null;
        public string title = null;
        /// <summary>
        /// 建立一个工作节点事例
        /// </summary>
        /// <param name="wk">工作</param>
        /// <param name="nd">节点</param>
        public WorkNode(Work wk, Node nd)
        {
            this.WorkID = wk.OID;
            this._HisWork = wk;
            this._HisNode = nd;
        }
        #endregion

        #region 运算属性
        private void Repair()
        {
        }
        public WorkNode GetPreviousWorkNode_FHL(Int64 workid)
        {
            Nodes nds = this.HisNode.FromNodes;
            foreach (Node nd in nds)
            {
                if (nd.HisRunModel == RunModel.SubThread)
                {
                    Work wk = nd.HisWork;
                    wk.OID = workid;
                    if (wk.RetrieveFromDBSources() != 0)
                    {
                        WorkNode wn = new WorkNode(wk, nd);
                        return wn;
                    }
                }
            }
            return null;
        }
        public WorkNodes GetPreviousWorkNodes_FHL()
        {
            // 如果没有找到转向他的节点,就返回,当前的工作.
            if (this.HisNode.IsStartNode)
                throw new Exception("@此节点是开始节点,没有上一步工作"); //此节点是开始节点,没有上一步工作.

            if (this.HisNode.HisNodeWorkType == NodeWorkType.WorkHL
               || this.HisNode.HisNodeWorkType == NodeWorkType.WorkFHL)
            {
            }
            else
            {
                throw new Exception("@当前工作节 - 非是分合流节点。");
            }

            WorkNodes wns = new WorkNodes();
            Nodes nds = this.HisNode.FromNodes;
            foreach (Node nd in nds)
            {
                Works wks = (Works)nd.HisWorks;
                wks.Retrieve(WorkAttr.FID, this.HisWork.OID);

                if (wks.Count == 0)
                    continue;

                foreach (Work wk in wks)
                {
                    WorkNode wn = new WorkNode(wk, nd);
                    wns.Add(wn);
                }
            }
            return wns;
        }
        /// <summary>
        /// 得当他的上一步工作
        /// 1, 从当前的找到他的上一步工作的节点集合.		 
        /// 如果没有找到转向他的节点,就返回,当前的工作.
        /// </summary>
        /// <returns>得当他的上一步工作</returns>
        public WorkNode GetPreviousWorkNode()
        {
            // 如果没有找到转向他的节点,就返回,当前的工作.
            if (this.HisNode.IsStartNode)
                throw new Exception("@此节点是开始节点,没有上一步工作."); //此节点是开始节点,没有上一步工作.

            string sql = "";
            int nodeid = 0;
            string truckTable = "ND" + int.Parse(this.HisNode.FK_Flow) + "Track";
            sql = "SELECT NDFrom,Tag FROM " + truckTable + " WHERE WorkID=" + this.WorkID + " AND NDTo='" + this.HisNode.NodeID + "' AND (ActionType=1 OR ActionType=" + (int)ActionType.Skip + ") ORDER BY RDT DESC";
            //首先获取实际发送节点，不存在时再使用from节点
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                nodeid = int.Parse(dt.Rows[0]["NDFrom"].ToString());
                if (dt.Rows[0]["Tag"] != null && dt.Rows[0]["Tag"].ToString().Contains("SendNode=") == true)
                {
                    string tag = dt.Rows[0]["Tag"].ToString();
                    string[] strs = tag.Split('@');
                    foreach (string str in strs)
                    {
                        if (str == null || str == "" || str.Contains("SendNode=") == false)
                            continue;
                        string[] mystr = str.Split('=');
                        if (mystr.Length == 2)
                        {
                            string sendNode = mystr[1];
                            if (string.IsNullOrEmpty(sendNode) == false && sendNode.Equals("0") == false)
                            {
                                nodeid = int.Parse(sendNode);
                            }
                        }
                    }
                }
            }

            if (nodeid == 0)
            {
                switch (this.HisNode.HisRunModel)
                {
                    case RunModel.HL:
                    case RunModel.FHL:
                        sql = "SELECT NDFrom FROM " + truckTable + " WHERE WorkID=" + this.WorkID
                                                                                       + " ORDER BY RDT DESC";
                        break;
                    case RunModel.SubThread:
                        sql = "SELECT NDFrom FROM " + truckTable + " WHERE WorkID=" + this.WorkID
                                                                                       + " AND NDTo=" + this.HisNode.NodeID + " "
                                                                                       + " AND ActionType=" + (int)ActionType.SubThreadForward + " ORDER BY RDT DESC";
                        if (DBAccess.RunSQLReturnCOUNT(sql) == 0)
                            sql = "SELECT NDFrom FROM " + truckTable + " WHERE WorkID=" + this.HisWork.FID
                                                                                      + " AND NDTo=" + this.HisNode.NodeID + " "
                                                                                      + " AND ActionType=" + (int)ActionType.SubThreadForward + " ORDER BY RDT DESC";

                        break;
                    default:
                        sql = "SELECT FK_Node FROM WF_GenerWorkerList WHERE WorkID=" + this.WorkID + " AND FK_Node!='" + this.HisNode.NodeID + "' ORDER BY RDT,FK_Node ";
                        //throw new Exception("err@没有判断的类型:"+this.HisNode.HisRunModel);
                        //根据当前节点获取上一个节点，不用管那个人发送的
                        break;
                }
                nodeid = DBAccess.RunSQLReturnValInt(sql, 0);
            }
            if (nodeid == 0)
                throw new Exception("@错误，没有找到上一步节点。" + sql);

            Node nd = new Node(nodeid);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.RetrieveFromDBSources();

            WorkNode wn = new WorkNode(wk, nd);
            return wn;


            //WorkNodes wns = new WorkNodes();
            //Nodes nds = this.HisNode.FromNodes;
            //foreach (Node nd in nds)
            //{
            //    switch (this.HisNode.HisNodeWorkType)
            //    {
            //        case NodeWorkType.WorkHL: /* 如果是合流 */
            //            if (this.IsSubFlowWorkNode == false)
            //            {
            //                /* 如果不是线程 */
            //                Node pnd = nd.HisPriFLNode;
            //                if (pnd == null)
            //                    throw new Exception("@没有取道它的上一步骤的分流节点，请确认设计是否错误？");

            //                Work wk1 = (Work)pnd.HisWorks.GetNewEntity;
            //                wk1.OID = this.HisWork.OID;
            //                if (wk1.RetrieveFromDBSources() == 0)
            //                    continue;
            //                WorkNode wn11 = new WorkNode(wk1, pnd);
            //                return wn11;
            //                break;
            //            }
            //            break;
            //        default:
            //            break;
            //    }

            //    Work wk = (Work)nd.HisWorks.GetNewEntity;
            //    wk.OID = this.HisWork.OID;
            //    if (wk.RetrieveFromDBSources() == 0)
            //        continue;

            //    string table = "ND" + int.Parse(this.HisNode.FK_Flow) + "Track";
            //    string actionSQL = "SELECT EmpFrom,EmpFromT,RDT FROM " + table + " WHERE WorkID=" + this.WorkID + " AND NDFrom=" + nd.NodeID + " AND ActionType=" + (int)ActionType.Forward;
            //    DataTable dt = DBAccess.RunSQLReturnTable(actionSQL);
            //    if (dt.Rows.Count == 0)
            //        continue;

            //    wk.Rec = dt.Rows[0]["EmpFrom"].ToString();
            //    wk.RecText = dt.Rows[0]["EmpFromT"].ToString();
            //    wk.SetValByKey("RDT", dt.Rows[0]["RDT"].ToString());

            //    WorkNode wn = new WorkNode(wk, nd);
            //    wns.Add(wn);
            //}
            //switch (wns.Count)
            //{
            //    case 0:
            //        throw new Exception("没有找到他的上一步工作，系统错误，请通知管理员来处理，请上让上一步处理人撤消发送、或者用本区县管理员用户登陆=》待办工作=》流程查询=》在关键字中输入Workid其它条件选择全部，查询到该流程删除它。 @WorkID=" + this.WorkID);
            //    case 1:
            //        return (WorkNode)wns[0];
            //    default:
            //        break;
            //}
            //Node nd1 = wns[0].HisNode;
            //Node nd2 = wns[1].HisNode;
            //if (nd1.FromNodes.Contains(NodeAttr.NodeID, nd2.NodeID))
            //{
            //    return wns[0];
            //}
            //else
            //{
            //    return wns[1];
            //}
        }
        #endregion
    }
    /// <summary>
    /// 工作节点集合.
    /// </summary>
    public class WorkNodes : CollectionBase
    {
        #region 构造
        /// <summary>
        /// 他的工作s
        /// </summary> 
        public Works GetWorks
        {
            get
            {
                if (this.Count == 0)
                    throw new Exception("@初始化失败，没有找到任何节点。");

                Works ens = this[0].HisNode.HisWorks;
                ens.Clear();

                foreach (WorkNode wn in this)
                {
                    ens.AddEntity(wn.HisWork);
                }
                return ens;
            }
        }
        /// <summary>
        /// 工作节点集合
        /// </summary>
        public WorkNodes()
        {
        }

        public int GenerByFID(Flow flow, Int64 fid)
        {
            this.Clear();

            Nodes nds = flow.HisNodes;
            foreach (Node nd in nds)
            {
                if (nd.HisRunModel == RunModel.SubThread)
                    continue;

                Work wk = nd.GetWork(fid);
                if (wk == null)
                    continue;


                this.Add(new WorkNode(wk, nd));
            }
            return this.Count;
        }
        /// <summary>
        /// 这个方法有问题的
        /// </summary>
        /// <param name="flow"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public int GenerByWorkID2014_01_06(Flow flow, Int64 oid)
        {
            Nodes nds = flow.HisNodes;
            foreach (Node nd in nds)
            {
                Work wk = nd.GetWork(oid);
                if (wk == null)
                    continue;
                string table = "ND" + int.Parse(flow.No) + "Track";
                string actionSQL = "SELECT EmpFrom,EmpFromT,RDT FROM " + table + " WHERE WorkID=" + oid + " AND NDFrom=" + nd.NodeID + " AND ActionType=" + (int)ActionType.Forward;
                DataTable dt = DBAccess.RunSQLReturnTable(actionSQL);
                if (dt.Rows.Count == 0)
                    continue;

                wk.Rec = dt.Rows[0]["EmpFrom"].ToString();
                wk.RecText = dt.Rows[0]["EmpFromT"].ToString();
                wk.SetValByKey("RDT", dt.Rows[0]["RDT"].ToString());
                this.Add(new WorkNode(wk, nd));
            }
            return this.Count;
        }
        public int GenerByWorkID(Flow flow, Int64 oid)
        {
            string table = "ND" + int.Parse(flow.No) + "Track";
            string actionSQL = "SELECT EmpFrom,EmpFromT,RDT,NDFrom FROM " + table + " WHERE WorkID=" + oid + " AND (ActionType=" + (int)ActionType.Forward + " OR ActionType=" + (int)ActionType.ForwardFL + " OR ActionType=" + (int)ActionType.ForwardHL + " OR ActionType=" + (int)ActionType.SubThreadForward + " ) ORDER BY RDT";
            DataTable dt = DBAccess.RunSQLReturnTable(actionSQL);

            string nds = "";
            foreach (DataRow dr in dt.Rows)
            {
                Node nd = new Node(int.Parse(dr["NDFrom"].ToString()));
                Work wk = nd.GetWork(oid);
                if (wk == null)
                    wk = nd.HisWork;

                // 处理重复的问题.
                if (nds.Contains(nd.NodeID.ToString() + ",") == true)
                    continue;
                nds += nd.NodeID.ToString() + ",";


                wk.Rec = dr["EmpFrom"].ToString();
                wk.RecText = dr["EmpFromT"].ToString();
                wk.SetValByKey("RDT", dr["RDT"].ToString());
                this.Add(new WorkNode(wk, nd));
            }
            return this.Count;
        }
        /// <summary>
        /// 删除工作流程
        /// </summary>
        public void DeleteWorks()
        {
            foreach (WorkNode wn in this)
            {
                if (wn.HisFlow.HisDataStoreModel != DataStoreModel.ByCCFlow)
                    return;
                wn.HisWork.Delete();
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 增加一个WorkNode
        /// </summary>
        /// <param name="wn">工作 节点</param>
        public void Add(WorkNode wn)
        {
            this.InnerList.Add(wn);
        }
        /// <summary>
        /// 根据位置取得数据
        /// </summary>
        public WorkNode this[int index]
        {
            get
            {
                return (WorkNode)this.InnerList[index];
            }
        }
        #endregion
    }
}
