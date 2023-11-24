using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;
using BP.En;
using System.Collections.Generic;
using BP.Web;

namespace BP.WF
{
    /// <summary>
    /// 工作人员集合
    /// </summary>
    public class GenerWorkerListAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作节点
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 处罚单据编号
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 流程
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 征管软件是不是罚款
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 使用的部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 部门名称
        /// </summary>
        public const string DeptName = "DeptName";
        /// <summary>
        /// 应该完成时间
        /// </summary>
        public const string SDT = "SDT";
        /// <summary>
        /// 警告日期
        /// </summary>
        public const string DTOfWarning = "DTOfWarning";
        /// <summary>
        /// 记录时间
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 完成时间
        /// </summary>
        public const string CDT = "CDT";
        /// <summary>
        /// 是否可用
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// IsPass
        /// </summary>
        public const string IsPass = "IsPass";
        /// <summary>
        /// 流程ID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// 人员名称
        /// </summary>
        public const string EmpName = "EmpName";
        /// <summary>
        /// 节点名称
        /// </summary>
        public const string NodeName = "NodeName";
        /// <summary>
        /// 发送人
        /// </summary>
        public const string Sender = "Sender";
        /// <summary>
        /// 谁执行它?
        /// </summary>
        public const string WhoExeIt = "WhoExeIt";
        /// <summary>
        /// 优先级
        /// </summary>
        public const string PRI = "PRI";
        /// <summary>
        /// 是否读取？
        /// </summary>
        public const string IsRead = "IsRead";
        /// <summary>
        /// 催办次数
        /// </summary>
        public const string PressTimes = "PressTimes";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Tag = "Tag";
        /// <summary>
        /// 参数
        /// </summary>
        public const string Paras = "Paras";
        /// <summary>
        /// 挂起时间
        /// </summary>
        public const string DTOfHungup = "DTOfHungup";
        /// <summary>
        /// 解除挂起时间
        /// </summary>
        public const string DTOfUnHungup = "DTOfUnHungup";
        /// <summary>
        /// 挂起次数
        /// </summary>
        public const string HungupTimes = "HungupTimes";
        /// <summary>
        /// 外部用户编号
        /// </summary>
        public const string GuestNo = "GuestNo";
        /// <summary>
        /// 外部用户名称
        /// </summary>
        public const string GuestName = "GuestName";
        #endregion

        /// <summary>
        /// 分组标记
        /// </summary>
        public const string GroupMark = "GroupMark";
        /// <summary>
        /// 表单IDs
        /// </summary>
        public const string FrmIDs = "FrmIDs";
        /// <summary>
        /// 是否会签？
        /// </summary>
        public const string IsHuiQian = "IsHuiQian";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 岗位编号
        /// </summary>
        public const string StaNo = "StaNo";
    }
    /// <summary>
    /// 工作者列表
    /// </summary>
    public class GenerWorkerList : Entity
    {
        #region 参数属性.
        /// <summary>
        /// 是否会签
        /// </summary>
        public bool ItIsHuiQian
        {
            get
            {
                return this.GetParaBoolen(GenerWorkerListAttr.IsHuiQian);
            }
            set
            {
                this.SetPara(GenerWorkerListAttr.IsHuiQian, value);
            }
        }
        /// <summary>
        /// 分组标记
        /// </summary>
        public string GroupMark
        {
            get
            {
                return this.GetParaString(GenerWorkerListAttr.GroupMark);
            }
            set
            {
                this.SetPara(GenerWorkerListAttr.GroupMark, value);
            }
        }
        /// <summary>
        /// 表单ID(对于动态表单树有效.)
        /// </summary>
        public string FrmIDs
        {
            get
            {
                return this.GetParaString(GenerWorkerListAttr.FrmIDs);
            }
            set
            {
                this.SetPara(GenerWorkerListAttr.FrmIDs, value);
            }
        }
        #endregion 参数属性.

        #region 基本属性
        /// <summary>
        /// 谁来执行它
        /// </summary>
        public int WhoExeIt
        {
            get
            {
                return this.GetValIntByKey(GenerWorkerListAttr.WhoExeIt);
            }
            set
            {
                SetValByKey(GenerWorkerListAttr.WhoExeIt, value);
            }
        }
        public int PressTimes
        {
            get
            {
                return this.GetParaInt(GenerWorkerListAttr.PressTimes);
            }
            set
            {
                SetPara(GenerWorkerListAttr.PressTimes, value);
            }
        }
        /// <summary>
        /// 优先级
        /// </summary>
        public int PRI
        {
            get
            {
                return this.GetValIntByKey(GenerWorkerListAttr.PRI);
            }
            set
            {
                SetValByKey(GenerWorkerListAttr.PRI, value);
            }
        }

        /// <summary>
        /// Idx
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(GenerWorkerListAttr.Idx);
            }
            set
            {
                SetValByKey(GenerWorkerListAttr.Idx, value);
            }
        }
        /// <summary>
        /// WorkID
        /// </summary>
        public override string PK
        {
            get
            {
                return "WorkID,FK_Emp,FK_Node";
            }
        }
        /// <summary>
        /// 是否可用(在分配工作时有效)
        /// </summary>
        public bool ItIsEnable
        {
            get
            {
                return this.GetValBooleanByKey(GenerWorkerListAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.IsEnable, value);
            }
        }
        /// <summary>
        /// 是否通过(对审核的会签节点有效)
        /// </summary>
        public bool ItIsPass
        {
            get
            {
                return this.GetValBooleanByKey(GenerWorkerListAttr.IsPass);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.IsPass, value);
            }
        }
        /// <summary>
        /// 0=未处理.
        /// 1=已经通过.
        /// -2=  标志该节点是干流程人员处理的节点，目的为了让分流节点的人员可以看到待办.
        /// </summary>
        public int PassInt
        {
            get
            {
                return this.GetValIntByKey(GenerWorkerListAttr.IsPass);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.IsPass,value);
            }
        }
        public bool ItIsRead
        {
            get
            {
                return this.GetValBooleanByKey(GenerWorkerListAttr.IsRead);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.IsRead, value);
            }
        }
        /// <summary>
        /// WorkID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(GenerWorkerListAttr.WorkID);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.WorkID, value);
            }
        }
        /// <summary>
        /// Node
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(GenerWorkerListAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName
        {
            get
            {
                return this.GetParaString(GenerWorkerListAttr.DeptName);
            }
            set
            {
                this.SetPara(GenerWorkerListAttr.DeptName, value);
            }
        }
        public string StaName
        {
            get
            {
                return this.GetParaString("StaName");
            }
            set
            {
                this.SetPara("StaName", value);
            }
        }
        public string DeptNo
        {
            get
            {

                return this.GetValStrByKey(GenerWorkerListAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 发送人
        /// </summary>
        public string Sender
        {
            get
            {
                return this.GetValStrByKey(GenerWorkerListAttr.Sender);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.Sender, value);
            }
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkerListAttr.NodeName);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.NodeName, value);
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(GenerWorkerListAttr.FID);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.FID, value);
            }
        }
        /// <summary>
        /// 工作人员
        /// </summary>
        public Emp HisEmp
        {
            get
            {
                return new Emp(this.EmpNo);
            }
        }
        /// <summary>
        /// 发送日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(GenerWorkerListAttr.RDT);
            }
            set
            {
                //this.SetValByKey(GenerWorkerListAttr.RDT, value);
                this.SetValByKey(GenerWorkerListAttr.RDT, DataType.CurrentDateTimess);

            }
        }
        /// <summary>
        /// 完成时间
        /// </summary>
        public string CDT
        {
            get
            {
                return this.GetValStringByKey(GenerWorkerListAttr.CDT);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.CDT, value);
            }
        }
        /// <summary>
        /// 应该完成日期
        /// </summary>
        public string SDT
        {
            get
            {
                return this.GetValStringByKey(GenerWorkerListAttr.SDT);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.SDT, value);
            }
        }
        /// <summary>
        /// 警告日期
        /// </summary>
        public string DTOfWarning
        {
            get
            {
                return this.GetValStringByKey(GenerWorkerListAttr.DTOfWarning);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.DTOfWarning, value);
            }
        }
        /// <summary>
        /// 人员
        /// </summary>
        public string EmpNo
        {
            get
            {
                return this.GetValStringByKey(GenerWorkerListAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 人员名称
        /// </summary>
        public string EmpName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkerListAttr.EmpName);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.EmpName, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>		 
        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(GenerWorkerListAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.FK_Flow, value);
            }
        }

        public string GuestNo
        {
            get
            {
                return this.GetValStringByKey(GenerWorkerListAttr.GuestNo);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.GuestNo, value);
            }
        }
        public string GuestName
        {
            get
            {
                return this.GetValStringByKey(GenerWorkerListAttr.GuestName);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.GuestName, value);
            }
        }
        #endregion

        #region 挂起属性
        /// <summary>
        /// 挂起时间
        /// </summary>
        public string DTOfHungup
        {
            get
            {
                return this.GetValStringByKey(GenerWorkerListAttr.DTOfHungup);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.DTOfHungup, value);
            }
        }
        /// <summary>
        /// 解除挂起时间
        /// </summary>
        public string DTOfUnHungup
        {
            get
            {
                return this.GetValStringByKey(GenerWorkerListAttr.DTOfUnHungup);
            }
            set
            {
                this.SetValByKey(GenerWorkerListAttr.DTOfUnHungup, value);
            }
        }
        /// <summary>
        /// 挂起次数
        /// </summary>
        public int HungupTimes
        {
            get
            {
                return this.GetParaInt(GenerWorkerListAttr.HungupTimes);
            }
            set
            {
                this.SetPara(GenerWorkerListAttr.HungupTimes, value);
            }
        }
        #endregion 

        #region 构造函数
        /// <summary>
        /// 主键
        /// </summary>
        public override string PKField
        {
            get
            {
                return "WorkID,FK_Emp,FK_Node";
            }
        }
        /// <summary>
        /// 工作者
        /// </summary>
        public GenerWorkerList()
        {
        }
        public GenerWorkerList(Int64 workid, int nodeID, string empNo)
        {
            this.WorkID = workid;
            this.NodeID = nodeID;
            this.EmpNo = empNo;
            this.Retrieve();
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

                Map map = new Map("WF_GenerWorkerlist", "工作者");

                map.IndexField = GenerWorkerListAttr.WorkID;

                //三个主键: WorkID，FK_Emp，FK_Node
                map.AddTBIntPK(GenerWorkerListAttr.WorkID, 0, "工作ID", true, true);
                map.AddTBStringPK(GenerWorkerListAttr.FK_Emp, null, "人员", true, false, 0, 100, 100);
                map.AddTBIntPK(GenerWorkerListAttr.FK_Node, 0, "节点ID", true, false);

                //干流程WorkID.
                map.AddTBInt(GenerWorkerListAttr.FID, 0, "流程ID", true, false);

                map.AddTBString(GenerWorkerListAttr.EmpName, null, "人员名称", true, false, 0, 30, 100);
                map.AddTBString(GenerWorkerListAttr.NodeName, null, "节点名称", true, false, 0, 100, 100);

                map.AddTBString(GenerWorkerListAttr.FK_Flow, null, "流程", true, false, 0, 5, 100);
                //为tianyu集团，处理工作的岗位编号.
                map.AddTBString(GenerWorkerListAttr.StaNo, null, "岗位编号", true, false, 0, 100, 100);
                //为tianyu集团，处理工作的部门编号.
                map.AddTBString(GenerWorkerListAttr.FK_Dept, null, "部门", true, false, 0, 100, 100);
                
                //如果是流程属性来控制的就按流程属性来计算。
                map.AddTBDateTime(GenerWorkerListAttr.SDT, "应完成日期", false, false);
                map.AddTBDateTime(GenerWorkerListAttr.DTOfWarning, "警告日期", false, false);
                map.AddTBDateTime(GenerWorkerListAttr.RDT, "记录时间(接受工作日期)", false, false);

                //完成日期.
                map.AddTBDateTime(GenerWorkerListAttr.CDT, "完成时间", false, false);

                //用于分配工作.
                map.AddTBInt(GenerWorkerListAttr.IsEnable, 1, "是否可用", true, true);

                //add for 上海 2012-11-30 
                map.AddTBInt(GenerWorkerListAttr.IsRead, 0, "是否读取", true, true);

                //是否通过？ 0=未通过 1=已经完成 , xxx标识其他状态.
                map.AddTBInt(GenerWorkerListAttr.IsPass, 0, "是否通过(对合流节点有效)", false, false);

                // 谁执行它？ 0=手工执行，1=机器执行。2=混合执行.
                map.AddTBInt(GenerWorkerListAttr.WhoExeIt, 0, "谁执行它", false, false);

                //发送人. 2011-11-12 为天津用户增加。
                map.AddTBString(GenerWorkerListAttr.Sender, null, "发送人", true, false, 0, 200, 100);

                //优先级，2012-06-15 为青岛用户增加。
                map.AddTBInt(GenerWorkFlowAttr.PRI, 1, "优先级", true, true);

                // 挂起
                map.AddTBDateTime(GenerWorkerListAttr.DTOfHungup,null, "挂起时间", false, false);
                map.AddTBDateTime(GenerWorkerListAttr.DTOfUnHungup,null, "预计解除挂起时间", false, false);

                //外部用户. 203-08-30
                map.AddTBString(GenerWorkerListAttr.GuestNo, null, "外部用户编号", true, false, 0, 30, 100);
                map.AddTBString(GenerWorkerListAttr.GuestName, null, "外部用户名称", true, false, 0, 100, 100);

                map.AddTBInt(GenerWorkerListAttr.Idx, 1, "Idx", true, true);

                //参数标记 2014-04-05.
                map.AddTBAtParas(4000); 

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 重写基类的方法.
        protected override bool beforeInsert()
        {
            if (this.FID != 0)
            {
                if (this.FID == this.WorkID)
                    this.FID = 0;
            }

            if (this.EmpNo.Equals("Guest"))
            {
                this.EmpName = BP.Web.GuestUser.Name;
                this.GuestName = this.EmpName;
                this.GuestNo = BP.Web.GuestUser.No;
            }

            if (DataType.IsNullOrEmpty(this.DeptNo))
                this.DeptNo = WebUser.DeptNo;

            if (DataType.IsNullOrEmpty(this.DeptName))
                this.DeptName = WebUser.DeptName;

            //增加记录日期.
            this.SetValByKey(GenerWorkerListAttr.RDT,  DataType.CurrentDateTimess);

            return base.beforeInsert();
        }
        protected override bool beforeUpdate()
        {
            if (DataType.IsNullOrEmpty(this.DeptNo))
                this.DeptNo = WebUser.DeptNo;

            if (DataType.IsNullOrEmpty(this.DeptName))
                this.DeptName = WebUser.DeptName;

            return base.beforeUpdate();

        }
        #endregion 重写基类的方法.

    }
    /// <summary>
    /// 工作人员集合
    /// </summary>
    public class GenerWorkerLists : Entities
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GenerWorkerList();
            }
        }
        /// <summary>
        /// GenerWorkerList
        /// </summary>
        public GenerWorkerLists() { }
        public GenerWorkerLists(Int64 workId)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(GenerWorkerListAttr.WorkID, workId);
            qo.addOrderBy(GenerWorkerListAttr.RDT);
            qo.DoQuery();
            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workId"></param>
        /// <param name="nodeId"></param>
        public GenerWorkerLists(Int64 workId, int nodeId)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(GenerWorkerListAttr.WorkID, workId);
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.FK_Node, nodeId);
            qo.DoQuery();
            return;
        }
        public GenerWorkerLists(Int64 workId, int nodeId,string FK_Emp)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(GenerWorkerListAttr.WorkID, workId);
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.FK_Node, nodeId);
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.FK_Emp, FK_Emp);
            qo.DoQuery();
            return;
        }
        /// <summary>
        /// 构造工作人员集合
        /// </summary>
        /// <param name="workId">工作ID</param>
        /// <param name="nodeId">节点ID</param>
        /// <param name="isWithEmpExts">是否包含为分配的人员</param>
        public GenerWorkerLists(Int64 workId, int nodeId, bool isWithEmpExts)
        {
            QueryObject qo = new QueryObject(this);
            qo.addLeftBracket();
            qo.AddWhere(GenerWorkerListAttr.WorkID, workId);
            qo.addOr();
            qo.AddWhere(GenerWorkerListAttr.FID, workId);
            qo.addRightBracket();
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.FK_Node, nodeId);
            int i = qo.DoQuery();

            if (isWithEmpExts == false)
                return;

            if (i == 0)
                throw new Exception("@系统错误，工作人员丢失请与管理员联系。NodeID=" + nodeId + " WorkID=" + workId);

            RememberMe rm = new RememberMe();
            rm.EmpNo = BP.Web.WebUser.No;
            rm.NodeID = nodeId;
            if (rm.RetrieveFromDBSources() == 0)
                return;

            GenerWorkerList wl = (GenerWorkerList)this[0];
            string[] myEmpStrs = rm.Emps.Split('@');
            foreach (string emp in myEmpStrs)
            {
                if (emp==null || emp=="")
                    continue;

                if (this.GetCountByKey(GenerWorkerListAttr.FK_Emp, emp) >= 1)
                    continue;

                GenerWorkerList mywl = new GenerWorkerList();
                mywl.Copy(wl);
                mywl.ItIsEnable = false;
                mywl.EmpNo = emp;
                WF.Port.WFEmp myEmp = new BP.WF.Port.WFEmp(emp);
                mywl.EmpName = myEmp.Name;
                try
                {
                    mywl.Insert();
                }
                catch
                {
                    mywl.Update();
                    continue;
                }
                this.AddEntity(mywl);
            }
            return;
        }
        /// <summary>
        /// 工作者
        /// </summary>
        /// <param name="workId">工作者ID</param>
        /// <param name="flowNo">流程编号</param>
        public GenerWorkerLists(Int64 workId, string flowNo)
        {
            if (workId == 0)
                return;

            Flow fl = new Flow(flowNo);
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(GenerWorkerListAttr.WorkID, workId);
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.FK_Flow, flowNo);
            qo.DoQuery();
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GenerWorkerList> ToJavaList()
        {
            return (System.Collections.Generic.IList<GenerWorkerList>)this;
        }
        /// <summary>
        /// 转化成list 为了翻译成java的需要
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<BP.WF.GenerWorkerList> Tolist()
        {
            System.Collections.Generic.List<BP.WF.GenerWorkerList> list = new System.Collections.Generic.List<BP.WF.GenerWorkerList>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((BP.WF.GenerWorkerList)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
