using System;
using System.Data;
using BP.DA;
using BP.WF;
using BP.Port;
using BP.En;

namespace BP.WF
{
	/// <summary>
	/// 产生分合流程控制
	/// </summary>
    public class GenerFHAttr
    {
        #region 基本属性
        /// <summary>
        /// FID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// 工作流
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 流程状态
        /// </summary>
        public const string WFState = "WFState";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string GroupKey = "GroupKey";
        /// <summary>
        /// 当前工作到的节点.
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 发送到人员
        /// </summary>
        public const string ToEmpsMsg = "ToEmpsMsg";
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 记录时间
        /// </summary>
        public const string RDT = "RDT";
        #endregion
    }
	/// <summary>
	/// 产生分合流程控制
	/// </summary>
    public class GenerFH : Entity
    {
        #region 基本属性
        public override string PK
        {
            get
            {
                return "FID";
            }
        }
        /// <summary>
        /// HisFlow
        /// </summary>
        public Flow HisFlow
        {
            get
            {
                return new Flow(this.FK_Flow);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(GenerFHAttr.RDT);
            }
            set
            {
                SetValByKey(GenerFHAttr.RDT, value);
            }
        }
        public string Title
        {
            get
            {
                return this.GetValStringByKey(GenerFHAttr.Title);
            }
            set
            {
                SetValByKey(GenerFHAttr.Title, value);
            }
        }
        /// <summary>
        /// 工作流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(GenerFHAttr.FK_Flow);
            }
            set
            {
                SetValByKey(GenerFHAttr.FK_Flow, value);
            }
        }
        public string ToEmpsMsg
        {
            get
            {
                return this.GetValStringByKey(GenerFHAttr.ToEmpsMsg);
            }
            set
            {
                SetValByKey(GenerFHAttr.ToEmpsMsg, value);
            }
        }
        
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(GenerFHAttr.FID);
            }
            set
            {
                SetValByKey(GenerFHAttr.FID, value);
            }
        }
        public string GroupKey
        {
            get
            {
                return this.GetValStringByKey(GenerFHAttr.GroupKey);
            }
            set
            {
                this.SetValByKey(GenerFHAttr.GroupKey, value);
            }
        }
        public string FK_NodeText
        {
            get
            {
                Node nd = new Node(this.FK_Node);
                return nd.Name;
            }
        }
        /// <summary>
        /// 当前工作到的节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(GenerFHAttr.FK_Node);
            }
            set
            {
                SetValByKey(GenerFHAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 工作流程状态( 0, 未完成,1 完成, 2 强制终止 3, 删除状态,) 
        /// </summary>
        public int WFState
        {
            get
            {
                return this.GetValIntByKey(GenerFHAttr.WFState);
            }
            set
            {
                SetValByKey(GenerFHAttr.WFState, value);
            }
        }
        #endregion  

        #region 构造函数
        /// <summary>
        /// 产生分合流程控制流程
        /// </summary>
        public GenerFH()
        {
        }
        /// <summary>
        /// 产生分合流程控制流程
        /// </summary>
        /// <param name="FID"></param>
        public GenerFH(Int64 FID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(GenerFHAttr.FID, FID);
            if (qo.DoQuery() == 0)
                throw new Exception("查询 GenerFH 工作[" + FID + "]不存在，可能是已经完成。");
        }
        /// <summary>
        /// 产生分合流程控制流程
        /// </summary>
        /// <param name="FID">工作流程ID</param>
        /// <param name="flowNo">流程编号</param>
        public GenerFH(Int64 FID, string flowNo)
        {
            try
            {
                this.FID = FID;
                this.FK_Flow = flowNo;
                this.Retrieve();
            }
            catch (Exception ex)
            {
                //WorkFlow wf = new WorkFlow(new Flow(flowNo), FID, FID);
                //StartWork wk = wf.HisStartWork;
                //if (wf.WFState == BP.WF.WFState.Complete)
                //{
                //    throw new Exception("@已经完成流程，不存在于当前工作集合里，如果要得到此流程的详细信请查看历史工作。技术信息:" + ex.Message);
                //}
                //else
                //{
                //    this.Copy(wk);
                //    //string msg = "@流程内部错误，给您带来的不便，深表示抱歉，请把此情况通知给系统管理员。error code:0001更多的信息:" + ex.Message;
                //    string msg = "@流程内部错误，给您带来的不便，深表示抱歉，请把此情况通知给系统管理员。error code:0001更多的信息:" + ex.Message;
                //    Log.DefaultLogWriteLine(LogType.Error, "@工作完成后在使用它抛出的异常：" + msg);
                //    //throw new Exception(msg);
                //}
            }
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
                Map map = new Map("WF_GenerFH", "分合流程控制");
                
                map.AddTBIntPK(GenerFHAttr.FID, 0, "流程ID", true, true);
                map.AddTBIntPK("OID", 0, "OID", true, true);

                map.AddTBString(GenerFHAttr.Title, null, "标题", true, false, 0, 4000, 10);
                map.AddTBString(GenerFHAttr.GroupKey, null, "分组主键", true, false, 0, 3000, 10);
                map.AddTBString(GenerFHAttr.FK_Flow, null, "流程", true, false, 0, 500, 10);
                map.AddTBString(GenerFHAttr.ToEmpsMsg, null, "接受人员", true, false, 0, 4000, 10);
                map.AddTBInt(GenerFHAttr.FK_Node, 0, "停留节点", true, false);
                map.AddTBInt(GenerFHAttr.WFState, 0, "WFState", true, false);
                map.AddTBDate(GenerFHAttr.RDT, null, "RDT", true, false);

                //RefMethod rm = new RefMethod();
                //rm.Title = "工作报告";  // "工作报告" ;
                //rm.ClassMethodName = this.ToString() + ".DoRpt";
                //rm.Icon = "../WF/Img/Btn/doc.gif";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = this.ToE("FlowSelfTest", "流程自检"); // "流程自检";
                //rm.ClassMethodName = this.ToString() + ".DoSelfTestInfo";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "流程自检并修复";
                //rm.ClassMethodName = this.ToString() + ".DoRepare";
                //rm.Warning = "您确定要执行此功能吗？ \t\n 1)如果是断流程，并且停留在第一个节点上，系统为执行删除它。\t\n 2)如果是非地第一个节点，系统会返回到上次发起的位置。";
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 重载基类方法
        protected override void afterDelete()
        {
            base.afterDelete();
        }
        #endregion
    }
	/// <summary>
	/// 产生分合流程控制s
	/// </summary>
	public class GenerFHs : Entities
	{
		/// <summary>
		/// 根据工作流程,工作人员ID 查询出来他当前的能做的工作.
		/// </summary>
		/// <param name="flowNo">流程编号</param>
		/// <param name="empId">工作人员ID</param>
		/// <returns></returns>
		public static DataTable QuByFlowAndEmp(string flowNo, int empId)
		{
			string sql="SELECT a.FID FROM WF_GenerFH a, WF_GenerWorkerlist b WHERE a.FID=b.FID   AND b.FK_Node=a.FK_Node  AND b.FK_Emp='"+empId.ToString()+"' AND a.FK_Flow='"+flowNo+"'";
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
				return new GenerFH();
			}
		}
		/// <summary>
		/// 产生工作流程集合
		/// </summary>
		public GenerFHs(){}
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GenerFH> ToJavaList()
        {
            return (System.Collections.Generic.IList<GenerFH>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GenerFH> Tolist()
        {
            System.Collections.Generic.List<GenerFH> list = new System.Collections.Generic.List<GenerFH>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GenerFH)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
	
}
