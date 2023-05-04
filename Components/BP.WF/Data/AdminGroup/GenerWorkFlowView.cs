using System;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;

namespace BP.WF.Data.AdminGroup
{
    /// <summary>
    /// 流程实例
    /// </summary>
    public class GenerWorkFlowView : Entity
    {
        #region 基本属性
        /// <summary>
        /// 主键
        /// </summary>
        public override string PK
        {
            get
            {
                return GenerWorkFlowAttr.WorkID;
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string FlowNote
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.FlowNote);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.FlowNote, value);
            }
        }
        /// <summary>
        /// 工作流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.FK_Flow);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// BillNo
        /// </summary>
        public string BillNo
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.BillNo);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.BillNo, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.FlowName);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.FlowName, value);
            }
        }
        /// <summary>
        /// 优先级
        /// </summary>
        public int PRI
        {
            get
            {
                return this.GetValIntByKey(GenerWorkFlowAttr.PRI);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.PRI, value);
            }
        }
        /// <summary>
        /// 待办人员数量
        /// </summary>
        public int TodoEmpsNum
        {
            get
            {
                return this.GetValIntByKey(GenerWorkFlowAttr.TodoEmpsNum);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.TodoEmpsNum, value);
            }
        }
        /// <summary>
        /// 待办人员列表
        /// </summary>
        public string TodoEmps
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.TodoEmps);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.TodoEmps, value);
            }
        }
        /// <summary>
        /// 参与人
        /// </summary>
        public string Emps
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.Emps);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.Emps, value);
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public TaskSta TaskSta
        {
            get
            {
                return (TaskSta)this.GetValIntByKey(GenerWorkFlowAttr.TaskSta);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.TaskSta, (int)value);
            }
        }
        /// <summary>
        /// 类别编号
        /// </summary>
        public string FK_FlowSort
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.FK_FlowSort);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.FK_FlowSort, value);
            }
        }
        /// <summary>
        /// 部门编号
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.FK_Dept);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.Title);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.Title, value);
            }
        }
        /// <summary>
        /// 客户编号
        /// </summary>
        public string GuestNo
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.GuestNo);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.GuestNo, value);
            }
        }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string GuestName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.GuestName);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.GuestName, value);
            }
        }
        /// <summary>
        /// 产生时间
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.RDT);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.RDT, value);
            }
        }
        /// <summary>
        /// 节点应完成时间
        /// </summary>
        public string SDTOfNode
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.SDTOfNode);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.SDTOfNode, value);
            }
        }
        /// <summary>
        /// 流程应完成时间
        /// </summary>
        public string SDTOfFlow
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.SDTOfFlow);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.SDTOfFlow, value);
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(GenerWorkFlowAttr.WorkID);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.WorkID, value);
            }
        }
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(GenerWorkFlowAttr.FID);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.FID, value);
            }
        }
        /// <summary>
        /// 父节点流程编号.
        /// </summary>
        public Int64 PWorkID
        {
            get
            {
                return this.GetValInt64ByKey(GenerWorkFlowAttr.PWorkID);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.PWorkID, value);
            }
        }
        /// <summary>
        /// 父流程调用的节点
        /// </summary>
        public int PNodeID
        {
            get
            {
                return this.GetValIntByKey(GenerWorkFlowAttr.PNodeID);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.PNodeID, value);
            }
        }
        /// <summary>
        /// PFlowNo
        /// </summary>
        public string PFlowNo
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.PFlowNo);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.PFlowNo, value);
            }
        }
        /// <summary>
        /// 吊起子流程的人员
        /// </summary>
        public string PEmp
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.PEmp);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.PEmp, value);
            }
        }
        /// <summary>
        /// 发起人
        /// </summary>
        public string Starter
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.Starter);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.Starter, value);
            }
        }
        /// <summary>
        /// 发起人名称
        /// </summary>
        public string StarterName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.StarterName);
            }
            set
            {
                this.SetValByKey(GenerWorkFlowAttr.StarterName, value);
            }
        }
        /// <summary>
        /// 发起人部门名称
        /// </summary>
        public string DeptName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.DeptName);
            }
            set
            {
                this.SetValByKey(GenerWorkFlowAttr.DeptName, value);
            }
        }
        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.NodeName);
            }
            set
            {
                this.SetValByKey(GenerWorkFlowAttr.NodeName, value);
            }
        }
        /// <summary>
        /// 当前工作到的节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(GenerWorkFlowAttr.FK_Node);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 工作流程状态
        /// </summary>
        public WFState WFState
        {
            get
            {
                return (WFState)this.GetValIntByKey(GenerWorkFlowAttr.WFState);
            }
            set
            {
                if (value == WF.WFState.Complete)
                    SetValByKey(GenerWorkFlowAttr.WFSta, (int)WFSta.Complete);
                else if (value == WF.WFState.Delete)
                    SetValByKey(GenerWorkFlowAttr.WFSta, (int)WFSta.Etc);
                else
                    SetValByKey(GenerWorkFlowAttr.WFSta, (int)WFSta.Runing);

                SetValByKey(GenerWorkFlowAttr.WFState, (int)value);
            }
        }
        /// <summary>
        /// 状态(简单)
        /// </summary>
        public WFSta WFSta
        {
            get
            {
                return (WFSta)this.GetValIntByKey(GenerWorkFlowAttr.WFSta);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.WFSta, (int)value);
            }
        }
        public string WFStateText
        {
            get
            {
                BP.WF.WFState ws = (WFState)this.WFState;
                switch (ws)
                {
                    case WF.WFState.Complete:
                        return "已完成";
                    case WF.WFState.Runing:
                        return "在运行";
                    case WF.WFState.Hungup:
                        return "挂起";
                    case WF.WFState.Askfor:
                        return "加签";
                    case WF.WFState.ReturnSta:
                        return "退回";
                    case WF.WFState.Draft:
                        return "草稿";
                    default:
                        return "未判断";
                }
            }
        }
        /// <summary>
        /// GUID
        /// </summary>
        public string GUID
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.GUID);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.GUID, value);
            }
        }
        #endregion

        #region 参数属性.
        public string Paras_ToNodes
        {

            get
            {
                return this.GetParaString("ToNodes");
            }

            set
            {
                this.SetPara("ToNodes", value);
            }
        }
        /// <summary>
        /// 加签信息
        /// </summary>
        public string Paras_AskForReply
        {

            get
            {
                return this.GetParaString("AskForReply");
            }

            set
            {
                this.SetPara("AskForReply", value);
            }
        }
        #endregion 参数属性.

        #region 构造函数
        /// <summary>
        /// 访问权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                return uac;
            }
        }
        /// <summary>
        /// 产生的工作流程
        /// </summary>
        public GenerWorkFlowView()
        {
        }
        /// <summary>
        /// 产生的工作流程
        /// </summary>
        /// <param name="workId"></param>
        public GenerWorkFlowView(Int64 workId)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(GenerWorkFlowAttr.WorkID, workId);
            if (qo.DoQuery() == 0)
                throw new Exception("工作 GenerWorkFlowView [" + workId + "]不存在。");
        }

        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_GenerWorkFlow", "流程查询");

                map.AddTBIntPK(GenerWorkFlowAttr.WorkID, 0, "WorkID", true, true);
                map.AddTBString(GenerWorkFlowAttr.StarterName, null, "发起人", true, true, 0, 30, 10);
                map.AddTBString(GenerWorkFlowAttr.Title, null, "标题", true, true, 0, 300, 10, true);

                map.AddDDLSysEnum(GenerWorkFlowAttr.WFSta, 0, "流程状态", true, false,GenerWorkFlowAttr.WFSta,
                    "@0=运行中@1=已完成@2=其他");

                map.AddTBString(GenerWorkFlowAttr.NodeName, null, "当前节点", true, true, 0, 100, 10);
                map.AddTBDateTime(GenerWorkFlowAttr.RDT, "发起日期", true, true);
                map.AddTBString(GenerWorkFlowAttr.BillNo, null, "单据编号", true, true, 0, 100, 10);
                map.AddTBString(GenerWorkFlowAttr.FlowName, null, "流程名", true, true, 0, 100, 10, true);

                map.AddDDLEntities("OrgNo", null, "组织", new BP.WF.Port.AdminGroup.Orgs(), false);
                map.AddTBString(GenerWorkFlowAttr.FK_NY, null, "发起月份", true, true, 0, 100, 10);


                map.AddSearchAttr(GenerWorkFlowAttr.WFSta);
                map.DTSearchKey = GenerWorkFlowAttr.RDT;
                map.DTSearchWay = DTSearchWay.ByDate;
                map.DTSearchLabel = "发起日期";

                //把不等于 0 的去掉.
                if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    map.AddHidden("OrgNo", "=", "@WebUser.OrgNo");

                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "轨迹查看";
                rm.ClassMethodName = this.ToString() + ".DoTrack";
                // rm.Icon = "../../WF/Img/Track.png";
                rm.Icon = "icon-graph";
                //        rm.IsForEns = true;
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                // rm.Icon = "../../WF/Img/Btn/Back.png";
                rm.Icon = "icon-reload";
                rm.Title = "回滚";
                rm.ClassMethodName = this.ToString() + ".DoRollback";

                rm.HisAttrs.AddDDLSQL("NodeID", "0", "回滚到节点",
                   "SELECT NodeID+'' as No,Name FROM WF_Node WHERE FK_Flow='@FK_Flow'", true);
                rm.HisAttrs.AddTBString("Note", null, "回滚原因", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "轨迹修改";
                rm.Icon = "icon-graph";
                //   rm.IsForEns = false;
                rm.ClassMethodName = this.ToString() + ".DoEditTrack";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "调整流程";
                rm.Icon = "icon-target";
                rm.HisAttrs.AddTBString("RenYuan", null, "调整到人员", true, false, 0, 100, 100);
                //rm.HisAttrs.AddTBInt("shuzi", 0, "调整到节点", true, false);
                rm.HisAttrs.AddDDLSQL("nodeID", "0", "调整到节点",
                    "SELECT NodeID as No,Name FROM WF_Node WHERE FK_Flow='@FK_Flow'", true);
                rm.ClassMethodName = this.ToString() + ".DoTest";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "调整数据";
                rm.IsForEns = false;
                rm.Icon = "icon-target";
                rm.ClassMethodName = this.ToString() + ".DoEditFrm";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "彻底删除";
                rm.ClassMethodName = this.ToString() + ".DoDelete";
                rm.Warning = "您确定要删除吗？包括该流程的所有数据。";
                // rm.Icon = "../../WF/Img/Btn/Delete.gif";
                rm.Icon = "icon-close";
                rm.IsForEns = false;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "逻辑删除";
                rm.ClassMethodName = this.ToString() + ".DoDeleteFlag";
                rm.HisAttrs.AddTBString("Note", null, "删除原因", true, false, 0, 100, 100);
                //   rm.Warning = "您确定要删除吗？";
                // rm.Icon = "../../WF/Img/Btn/Delete.gif";
                rm.Icon = "icon-close";
                rm.IsForEns = false;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = "icon-wrench";
                rm.Title = "修复数据";
                rm.IsForEns = false;
                rm.ClassMethodName = this.ToString() + ".RepairDataIt";
                rm.RefMethodType = RefMethodType.Func;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = "icon-key";
                rm.Title = "移交";
                rm.ClassMethodName = this.ToString() + ".DoFlowShift";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 修改表单
        /// </summary>
        /// <returns></returns>
        public string DoEditFrm()
        {
            Node nd = new Node(this.FK_Node);
            if (nd.FormType == NodeFormType.SelfForm
                || nd.FormType == NodeFormType.SDKForm
                || nd.FormType == NodeFormType.SheetAutoTree
                || nd.FormType == NodeFormType.SheetTree
                || nd.FormType == NodeFormType.WebOffice
                || nd.FormType == NodeFormType.WordForm)
                return "err@当前节点表单类型不同.";

            string frmID = nd.NodeFrmID;
            return "../../Admin/AttrFlow/AdminFrmList.htm?FK_Flow=" + this.FK_Flow + "&FrmID=" + frmID + "&WorkID=" + this.WorkID;
        }

        #region 执行功能.
        //,string isOK, int wfstate, string fk_emp
        public string DoTest(string toEmpNo, string toNodeID)
        {
            try
            {
                return BP.WF.Dev2Interface.Flow_ReSend(this.WorkID, int.Parse(toNodeID),
                    toEmpNo, BP.Web.WebUser.Name + ":调整.");
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }

        }
        /// <summary>
        /// 修复数据
        /// 1. 当前的节点的数据不小心丢失.
        /// 2. 从轨迹里把数据找到溯源到业务表里.
        /// </summary>
        /// <returns></returns>
        public string RepairDataIt()
        {
            string infos = "";

            Flow fl = new Flow(this.FK_Flow);
            Node nd = new Node(int.Parse(fl.No + "01"));
            Work wk = nd.HisWork;

            string trackTable = "ND" + int.Parse(fl.No) + "Track";
            string sql = "SELECT MyPK FROM " + trackTable + " WHERE WorkID=" + this.WorkID + " AND ACTIONTYPE=1 and NDFrom=" + nd.NodeID;
            string mypk = DBAccess.RunSQLReturnString(sql);
            if (DataType.IsNullOrEmpty(mypk) == true)
                return "err@没有找到track主键。";

            wk.OID = this.WorkID;
            wk.RetrieveFromDBSources();

            string file = "c:/temp/" + this.WorkID + ".txt";
            try
            {
                DBAccess.GetFileFromDB(file, trackTable, "MyPK", mypk, "FrmDB");
            }
            catch (Exception ex)
            {
                infos += "@ 错误:" + fl.No + " - Rec" + wk.Rec + " db=" + wk.OID + " - " + fl.Name;
            }

            string json = DataType.ReadTextFile(file);
            DataTable dtVal = BP.Tools.Json.ToDataTable(json);

            DataRow mydr = dtVal.Rows[0];

            Attrs attrs = wk.EnMap.Attrs;
            bool isHave = false;
            foreach (Attr attr in attrs)
            {
                string jsonVal = mydr[attr.Key].ToString();
                string enVal = wk.GetValStringByKey(attr.Key);
                if (DataType.IsNullOrEmpty(enVal) == true)
                {
                    wk.SetValByKey(attr.Key, jsonVal);
                    isHave = true;
                }
            }

            if (isHave == true)
            {
                wk.DirectUpdate();
                return "不需要更新数据.";
            }
            infos += "@WorkID=" + wk.OID + " =" + wk.Rec + "  被修复.";

            return infos;
        }
        public string RepairDataAll()
        {
            string infos = "";

            Flows fls = new Flows();
            fls.RetrieveAll();

            foreach (Flow fl in fls)
            {
                string sql = "SELECT OID FROM " + fl.PTable + " WHERE BillNo IS NULL AND OID=" + this.WorkID;
                DataTable dt = DBAccess.RunSQLReturnTable(sql);

                Node nd = new Node(int.Parse(fl.No + "01"));
                Work wk = nd.HisWork;

                string trackTable = "ND" + int.Parse(fl.No) + "Track";
                foreach (DataRow dr in dt.Rows)
                {
                    Int64 workid = Int64.Parse(dr["OID"].ToString());

                    sql = "SELECT MyPK FROM " + trackTable + " WHERE WorkID=" + workid + " AND ACTIONTYPE=1 and NDFrom=" + nd.NodeID;
                    string mypk = DBAccess.RunSQLReturnString(sql);
                    if (DataType.IsNullOrEmpty(mypk) == true)
                        continue;

                    wk.OID = workid;
                    wk.RetrieveFromDBSources();

                    string file = "c:/temp/" + mypk + ".txt";
                    try
                    {
                        DBAccess.GetFileFromDB(file, trackTable, "MyPK", mypk, "FrmDB");
                    }
                    catch (Exception ex)
                    {
                        infos += "@ 错误:" + fl.No + " - Rec" + wk.Rec + " db=" + wk.OID + " - " + fl.Name;
                    }

                    string json = DataType.ReadTextFile(file);
                    DataTable dtVal = BP.Tools.Json.ToDataTable(json);

                    DataRow mydr = dtVal.Rows[0];

                    Attrs attrs = wk.EnMap.Attrs;
                    bool isHave = false;
                    foreach (Attr attr in attrs)
                    {
                        string jsonVal = mydr[attr.Key].ToString();
                        string enVal = wk.GetValStringByKey(attr.Key);
                        if (DataType.IsNullOrEmpty(enVal) == true)
                        {
                            wk.SetValByKey(attr.Key, jsonVal);
                            isHave = true;
                        }
                    }

                    if (isHave == true)
                    {
                        wk.DirectUpdate();
                        continue;
                    }
                    infos += "@WorkID=" + wk.OID + " =" + wk.Rec + "   被修复.";
                }
            }
            return infos;
        }
        /// <summary>
        /// 回滚
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        /// <param name="note">回滚原因</param>
        /// <returns>回滚的结果</returns>
        public string DoRollback(string nodeID, string note)
        {
            try
            {
                return BP.WF.Dev2Interface.Flow_DoRebackWorkFlow(this.FK_Flow, this.WorkID,
                    int.Parse(nodeID), note);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 修改轨迹
        /// </summary>
        /// <returns></returns>
        public string DoEditTrack()
        {
            return "../../Admin/AttrFlow/EditTrack.htm?WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string DoTrack()
        {
            return "../../WFRpt.htm?WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 执行移交
        /// </summary>
        /// <param name="ToEmp"></param>
        /// <param name="Note"></param>
        /// <returns></returns>
        public string DoShift(string ToEmp, string Note)
        {
            if (BP.WF.Dev2Interface.Flow_IsCanViewTruck(this.FK_Flow, this.WorkID) == false)
                return "您没有操作该流程数据的权限.";

            try
            {
                BP.WF.Dev2Interface.Node_Shift(this.WorkID, ToEmp, Note);
                return "移交成功";
            }
            catch (Exception ex)
            {
                return "移交失败@" + ex.Message;
            }
        }
        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <returns></returns>
        public string DoDeleteFlag(string msg)
        {
            if (BP.WF.Dev2Interface.Flow_IsCanViewTruck(this.FK_Flow, this.WorkID) == false)
                return "您没有操作该流程数据的权限.";

            try
            {
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(this.WorkID, msg, true);
                return "删除成功";
            }
            catch (Exception ex)
            {
                return "删除失败@" + ex.Message;
            }
        }
        /// <summary>
        /// 执行删除
        /// </summary>
        /// <returns></returns>
        public string DoDelete()
        {
            if (BP.WF.Dev2Interface.Flow_IsCanViewTruck(this.FK_Flow, this.WorkID) == false)
                return "您没有操作该流程数据的权限.";

            try
            {
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.WorkID, true);
                return "删除成功";
            }
            catch (Exception ex)
            {
                return "删除失败@" + ex.Message;
            }
        }
        /// <summary>
        /// 移交
        /// </summary>
        /// <returns></returns>
        public string DoFlowShift()
        {
            return "../../WorkOpt/Shift.htm?WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node;
        }
        /// <summary>
        /// 回滚流程
        /// </summary>
        /// <returns></returns>
        public string Rollback()
        {
            return "../../WorkOpt/Rollback.htm?WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node;
        }
        /// <summary>
        /// 执行跳转
        /// </summary>
        /// <returns></returns>
        public string DoFlowSkip()
        {
            return "../../WorkOpt/FlowSkip.htm?WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node;
        }
        #endregion
    }
    /// <summary>
    /// 流程实例s
    /// </summary>
    public class GenerWorkFlowViews : Entities
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GenerWorkFlowView();
            }
        }
        /// <summary>
        /// 流程实例集合
        /// </summary>
        public GenerWorkFlowViews() { }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GenerWorkFlowView> ToJavaList()
        {
            return (System.Collections.Generic.IList<GenerWorkFlowView>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GenerWorkFlowView> Tolist()
        {
            System.Collections.Generic.List<GenerWorkFlowView> list = new System.Collections.Generic.List<GenerWorkFlowView>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GenerWorkFlowView)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }

}
