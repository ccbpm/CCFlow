using System;
using System.Data;
using BP.DA;
using BP.WF;
using BP.Port ;
using BP.Sys;
using BP.En;
using BP.WF.Template;

namespace BP.WF.Data
{
	/// <summary>
    /// 流程实例
	/// </summary>
    public class GenerWorkFlowViewAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 工作流
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 流程状态
        /// </summary>
        public const string WFState = "WFState";
        /// <summary>
        /// 流程状态(简单)
        /// </summary>
        public const string WFSta = "WFSta";
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 发起人
        /// </summary>
        public const string Starter = "Starter";
        /// <summary>
        /// 产生时间
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 完成时间
        /// </summary>
        public const string CDT = "CDT";
        /// <summary>
        /// 得分
        /// </summary>
        public const string Cent = "Cent";
        /// <summary>
        /// 当前工作到的节点.
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 当前工作岗位
        /// </summary>
        public const string FK_Station = "FK_Station";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 年月
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// 流程ID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// 是否启用
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// 流程名称
        /// </summary>
        public const string FlowName = "FlowName";
        /// <summary>
        /// 发起人名称
        /// </summary>
        public const string StarterName = "StarterName";
        /// <summary>
        /// 节点名称
        /// </summary>
        public const string NodeName = "NodeName";
        /// <summary>
        /// 部门名称
        /// </summary>
        public const string DeptName = "DeptName";
        /// <summary>
        /// 流程类别
        /// </summary>
        public const string FK_FlowSort = "FK_FlowSort";
        /// <summary>
        /// 优先级
        /// </summary>
        public const string PRI = "PRI";
        /// <summary>
        /// 流程应完成时间
        /// </summary>
        public const string SDTOfFlow = "SDTOfFlow";
        /// <summary>
        /// 节点应完成时间
        /// </summary>
        public const string SDTOfNode = "SDTOfNode";
        /// <summary>
        /// 父流程ID
        /// </summary>
        public const string PWorkID = "PWorkID";
        /// <summary>
        /// 父流程编号
        /// </summary>
        public const string PFlowNo = "PFlowNo";
        /// <summary>
        /// 父流程节点
        /// </summary>
        public const string PNodeID = "PNodeID";
        /// <summary>
        /// 子流程的调用人.
        /// </summary>
        public const string PEmp = "PEmp";
        /// <summary>
        /// 客户编号(对于客户发起的流程有效)
        /// </summary>
        public const string GuestNo = "GuestNo";
        /// <summary>
        /// 客户名称
        /// </summary>
        public const string GuestName = "GuestName";
        /// <summary>
        /// 单据编号
        /// </summary>
        public const string BillNo = "BillNo";
        /// <summary>
        /// 备注
        /// </summary>
        public const string FlowNote = "FlowNote";
        /// <summary>
        /// 待办人员
        /// </summary>
        public const string TodoEmps = "TodoEmps";
        /// <summary>
        /// 待办人员数量
        /// </summary>
        public const string TodoEmpsNum = "TodoEmpsNum";
        /// <summary>
        /// 任务状态
        /// </summary>
        public const string TaskSta = "TaskSta";
        /// <summary>
        /// 临时存放的参数
        /// </summary>
        public const string AtPara = "AtPara";
        /// <summary>
        /// 参与人
        /// </summary>
        public const string Emps = "Emps";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
        #endregion
    }
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
                return GenerWorkFlowViewAttr.WorkID;
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string FlowNote
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.FlowNote);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.FlowNote, value);
            }
        }
        /// <summary>
        /// 工作流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.FK_Flow);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// BillNo
        /// </summary>
        public string BillNo
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.BillNo);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.BillNo, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.FlowName);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.FlowName, value);
            }
        }
        /// <summary>
        /// 优先级
        /// </summary>
        public int PRI
        {
            get
            {
                return this.GetValIntByKey(GenerWorkFlowViewAttr.PRI);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.PRI, value);
            }
        }
        /// <summary>
        /// 待办人员数量
        /// </summary>
        public int TodoEmpsNum
        {
            get
            {
                return this.GetValIntByKey(GenerWorkFlowViewAttr.TodoEmpsNum);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.TodoEmpsNum, value);
            }
        }
        /// <summary>
        /// 待办人员列表
        /// </summary>
        public string TodoEmps
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.TodoEmps);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.TodoEmps, value);
            }
        }
        /// <summary>
        /// 参与人
        /// </summary>
        public string Emps
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.Emps);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.Emps, value);
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public TaskSta TaskSta
        {
            get
            {
                return (TaskSta)this.GetValIntByKey(GenerWorkFlowViewAttr.TaskSta);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.TaskSta, (int)value);
            }
        }
        /// <summary>
        /// 类别编号
        /// </summary>
        public string FK_FlowSort
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.FK_FlowSort);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.FK_FlowSort, value);
            }
        }
        /// <summary>
        /// 部门编号
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.FK_Dept);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.Title);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.Title, value);
            }
        }
        /// <summary>
        /// 客户编号
        /// </summary>
        public string GuestNo
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.GuestNo);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.GuestNo, value);
            }
        }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string GuestName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.GuestName);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.GuestName, value);
            }
        }
        /// <summary>
        /// 产生时间
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.RDT);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.RDT, value);
            }
        }
        /// <summary>
        /// 节点应完成时间
        /// </summary>
        public string SDTOfNode
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.SDTOfNode);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.SDTOfNode, value);
            }
        }
        /// <summary>
        /// 流程应完成时间
        /// </summary>
        public string SDTOfFlow
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.SDTOfFlow);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.SDTOfFlow, value);
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(GenerWorkFlowViewAttr.WorkID);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.WorkID, value);
            }
        }
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(GenerWorkFlowViewAttr.FID);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.FID, value);
            }
        }
        /// <summary>
        /// 父节点流程编号.
        /// </summary>
        public Int64 PWorkID
        {
            get
            {
                return this.GetValInt64ByKey(GenerWorkFlowViewAttr.PWorkID);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.PWorkID, value);
            }
        }
        /// <summary>
        /// 父流程调用的节点
        /// </summary>
        public int PNodeID
        {
            get
            {
                return this.GetValIntByKey(GenerWorkFlowViewAttr.PNodeID);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.PNodeID, value);
            }
        }
        /// <summary>
        /// PFlowNo
        /// </summary>
        public string PFlowNo
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.PFlowNo);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.PFlowNo, value);
            }
        }
        /// <summary>
        /// 吊起子流程的人员
        /// </summary>
        public string PEmp
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.PEmp);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.PEmp, value);
            }
        }
        /// <summary>
        /// 发起人
        /// </summary>
        public string Starter
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.Starter);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.Starter, value);
            }
        }
        /// <summary>
        /// 发起人名称
        /// </summary>
        public string StarterName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.StarterName);
            }
            set
            {
                this.SetValByKey(GenerWorkFlowViewAttr.StarterName, value);
            }
        }
        /// <summary>
        /// 发起人部门名称
        /// </summary>
        public string DeptName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.DeptName);
            }
            set
            {
                this.SetValByKey(GenerWorkFlowViewAttr.DeptName, value);
            }
        }
        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowViewAttr.NodeName);
            }
            set
            {
                this.SetValByKey(GenerWorkFlowViewAttr.NodeName, value);
            }
        }
        /// <summary>
        /// 当前工作到的节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(GenerWorkFlowViewAttr.FK_Node);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 工作流程状态
        /// </summary>
        public WFState WFState
        {
            get
            {
                return (WFState)this.GetValIntByKey(GenerWorkFlowViewAttr.WFState);
            }
            set
            {
                if (value == WF.WFState.Complete)
                    SetValByKey(GenerWorkFlowViewAttr.WFSta, (int)WFSta.Complete);
                else if (value == WF.WFState.Delete)
                    SetValByKey(GenerWorkFlowViewAttr.WFSta, (int)WFSta.Etc);
                else
                    SetValByKey(GenerWorkFlowViewAttr.WFSta, (int)WFSta.Runing);

                SetValByKey(GenerWorkFlowViewAttr.WFState, (int)value);
            }
        }
        /// <summary>
        /// 状态(简单)
        /// </summary>
        public WFSta WFSta
        {
            get
            {
                return (WFSta)this.GetValIntByKey(GenerWorkFlowViewAttr.WFSta);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.WFSta, (int)value);
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
                    case WF.WFState.HungUp:
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
                return this.GetValStrByKey(GenerWorkFlowViewAttr.GUID);
            }
            set
            {
                SetValByKey(GenerWorkFlowViewAttr.GUID, value);
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
            qo.AddWhere(GenerWorkFlowViewAttr.WorkID, workId);
            if (qo.DoQuery() == 0)
                throw new Exception("工作 GenerWorkFlowView [" + workId + "]不存在。");
        }
        /// <summary>
        /// 执行修复
        /// </summary>
        public void DoRepair()
        {
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

                // map.AddTBIntPK("WorkID", 0, "WorkID", true, true);
                map.AddTBIntPK(GenerWorkFlowViewAttr.WorkID, 0, "WorkID", true, true);


                map.AddTBString(GenerWorkFlowViewAttr.StarterName, null, "发起人", true, false, 0, 30, 10);
                map.AddTBString(GenerWorkFlowViewAttr.Title, null, "标题", true, false, 0, 100, 10, true);
                map.AddDDLSysEnum(GenerWorkFlowViewAttr.WFSta, 0, "流程状态", true, false, GenerWorkFlowViewAttr.WFSta,
                    "@0=运行中@1=已完成@2=其他");

                map.AddDDLSysEnum(GenerWorkFlowViewAttr.WFState, 0, "流程状态", true, false, MyStartFlowAttr.WFState);
                map.AddTBString(GenerWorkFlowViewAttr.NodeName, null, "当前节点名称", true, false, 0, 100, 10);
                map.AddTBDateTime(GenerWorkFlowViewAttr.RDT, "记录日期", true, true);
                map.AddTBString(GenerWorkFlowViewAttr.BillNo, null, "单据编号", true, false, 0, 100, 10);
                map.AddTBStringDoc(GenerWorkFlowViewAttr.FlowNote, null, "备注", true, false, true);

                map.AddDDLEntities(GenerWorkFlowViewAttr.FK_FlowSort, null, "类别", new FlowSorts(), false);
                map.AddDDLEntities(GenerWorkFlowViewAttr.FK_Flow, null, "流程", new Flows(), false);
                map.AddDDLEntities(GenerWorkFlowViewAttr.FK_Dept, null, "部门", new BP.Port.Depts(), false);

                map.AddTBInt(GenerWorkFlowViewAttr.FID, 0, "FID", false, false);
                map.AddTBInt(GenerWorkFlowViewAttr.FK_Node, 0, "FK_Node", false, false);

                map.AddDDLEntities(GenerWorkFlowViewAttr.FK_NY, null, "月份", new GenerWorkFlowViewNYs(), false);

                map.AddTBMyNum();

                //map.AddSearchAttr(GenerWorkFlowViewAttr.FK_Dept);
                map.AddSearchAttr(GenerWorkFlowViewAttr.FK_Flow);
                map.AddSearchAttr(GenerWorkFlowViewAttr.WFSta);
                map.AddSearchAttr(GenerWorkFlowViewAttr.FK_NY);

                //把不等于 0 的去掉.
                map.AddHidden(GenerWorkFlowViewAttr.WFState, "!=", "0");


                RefMethod rm = new RefMethod();
                rm.Title = "轨迹";
                rm.ClassMethodName = this.ToString() + ".DoTrack";
                rm.Icon = "../../WF/Img/Track.png";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "删除";
                rm.ClassMethodName = this.ToString() + ".DoDelete";
                rm.Warning = "您确定要删除吗？";
                rm.Icon = "../../WF/Img/Btn/Delete.gif";
                rm.IsForEns = false;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/CC.gif";
                rm.Title = "移交";
                rm.ClassMethodName = this.ToString() + ".DoFlowShift";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/Back.png";
                rm.Title = "回滚";
                rm.ClassMethodName =  this.ToString()+ ".Rollback";
                //rm.HisAttrs.AddTBInt("NodeID", 0, "回滚到节点", true, false);
               // rm.HisAttrs.AddTBInt("NodeID", 0, "回滚到节点", true, false);
                rm.HisAttrs.AddTBString("NodeID", null, "NodeID", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("EmpNo", null, "回滚到人员编号",true,false,0,100,100);
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/CC.gif";
                rm.Title = "跳转";
                rm.IsForEns = false;
                rm.ClassMethodName = this.ToString() + ".DoFlowSkip";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/CC.gif";
                rm.Title = "修复该流程数据实例";
                rm.IsForEns = false;
                rm.ClassMethodName = this.ToString() + ".RepairDataIt";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "调整";
                rm.HisAttrs.AddTBString("wenben", null, "调整到人员", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBInt("shuzi", 0, "调整到节点", true, false);
                 
                rm.ClassMethodName = this.ToString() + ".DoTest";
                map.AddRefMethod(rm);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行功能.
        //,string isOK, int wfstate, string fk_emp
        public string DoTest(string toEmpNo, int toNodeID)
        {
           return BP.WF.Dev2Interface.Flow_ReSend(this.WorkID, toNodeID, toEmpNo,"admin调整");
        }
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

            string file = "c:\\temp\\" + this.WorkID + ".txt";
            try
            {
                BP.DA.DBAccess.GetFileFromDB(file, trackTable, "MyPK", mypk, "FrmDB");
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
                if ( DataType.IsNullOrEmpty(enVal)==true)
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
            infos += "@WorkID=" + wk.OID + " =" + wk.Rec + "  dt=" + wk.RDT + "被修复.";

            return infos;
        }
        public string RepairDataAll()
        {
            string infos = "";

            Flows fls = new Flows();
            fls.RetrieveAll();

            foreach (Flow fl in fls)
            {


                string sql = "SELECT OID FROM " + fl.PTable + " WHERE BillNo IS NULL AND OID="+this.WorkID;
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

                    string file = "c:\\temp\\" + mypk + ".txt";
                    try
                    {
                        BP.DA.DBAccess.GetFileFromDB(file, trackTable, "MyPK", mypk, "FrmDB");
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
                        if ( DataType.IsNullOrEmpty(enVal) ==true)
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
                    infos += "@WorkID=" + wk.OID + " =" + wk.Rec + "  dt=" + wk.RDT + "被修复.";
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
        public string Rollback(string nodeid, string note)
        {
            BP.WF.Template.FlowSheet fl = new Template.FlowSheet(this.FK_Flow);
            return fl.DoRebackFlowData(this.WorkID, int.Parse(nodeid), note);
        }

        public string DoTrack()
        {
            PubClass.WinOpen("../../WF/WFRpt.htm?WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow, 900, 800);
            return null;
        }
        /// <summary>
        /// 执行移交
        /// </summary>
        /// <param name="ToEmp"></param>
        /// <param name="Note"></param>
        /// <returns></returns>
        public string DoShift(string ToEmp, string Note)
        {
            if (BP.WF.Dev2Interface.Flow_IsCanViewTruck(this.FK_Flow, this.WorkID, this.FID) == false)
                return "您没有操作该流程数据的权限.";

            try
            {
                BP.WF.Dev2Interface.Node_Shift(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, ToEmp, Note);
                return "移交成功";
            }
            catch (Exception ex)
            {
                return "移交失败@" + ex.Message;
            }
        }
        /// <summary>
        /// 执行删除
        /// </summary>
        /// <returns></returns>
        public string DoDelete()
        {
            if (BP.WF.Dev2Interface.Flow_IsCanViewTruck(this.FK_Flow, this.WorkID, this.FID) == false)
                return "您没有操作该流程数据的权限.";

            try
            {
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.FK_Flow, this.WorkID, true);
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
            return "../../WorkOpt/Forward.htm?WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node;
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
		/// <summary>
		/// 根据工作流程,工作人员 ID 查询出来他当前的能做的工作.
		/// </summary>
		/// <param name="flowNo">流程编号</param>
		/// <param name="empId">工作人员ID</param>
		/// <returns></returns>
		public static DataTable QuByFlowAndEmp(string flowNo, int empId)
		{
			string sql="SELECT a.WorkID FROM WF_GenerWorkFlowView a, WF_GenerWorkerlist b WHERE a.WorkID=b.WorkID   AND b.FK_Node=a.FK_Node  AND b.FK_Emp='"+empId.ToString()+"' AND a.FK_Flow='"+flowNo+"'";
			return DBAccess.RunSQLReturnTable(sql);
		}


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
		public GenerWorkFlowViews(){}
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
