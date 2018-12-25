using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;
using BP.WF.Data;

namespace BP.WF
{
	/// <summary>
	/// 附件未读日志
	/// </summary>
	public class AthUnReadLogAttr 
	{
		#region 基本属性
		/// <summary>
		/// 工作ID
		/// </summary>
		public const  string WorkID="WorkID";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 流程类别
        /// </summary>
        public const string FlowName = "FlowName";
		/// <summary>
		/// 删除人员
		/// </summary>
		public const  string FK_Emp="FK_Emp";
		/// <summary>
		/// 删除原因
		/// </summary>
		public const  string BeiZhu="BeiZhu";
        /// <summary>
        /// 删除日期
        /// </summary>
        public const string SendDT = "SendDT";
        /// <summary>
        /// 删除人员
        /// </summary>
        public const string FK_EmpDept = "FK_EmpDept";
        /// <summary>
        /// 删除人员名称
        /// </summary>
        public const string FK_EmpDeptName = "FK_EmpDeptName";
        /// <summary>
        /// 第几周？
        /// </summary>
        public const string WeekNum = "WeekNum";
        /// <summary>
        /// 隶属年月？
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 节点名称
        /// </summary>
        public const string NodeName = "NodeName";
		#endregion

        #region 流程属性
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 参与人员
        /// </summary>
        public const string FlowEmps = "FlowEmps";
        /// <summary>
        /// 流程ID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// 发起年月
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// 发起人ID
        /// </summary>
        public const string FlowStarter = "FlowStarter";
        /// <summary>
        /// 发起日期
        /// </summary>
        public const string FlowStartSendDT = "FlowStartSendDT";
        /// <summary>
        /// 发起人部门ID
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 数量
        /// </summary>
        public const string MyNum = "MyNum";
        /// <summary>
        /// 结束人
        /// </summary>
        public const string FlowEnder = "FlowEnder";
        /// <summary>
        /// 最后活动日期
        /// </summary>
        public const string FlowEnderSendDT = "FlowEnderSendDT";
        /// <summary>
        /// 跨度
        /// </summary>
        public const string FlowDaySpan = "FlowDaySpan";
        /// <summary>
        /// 结束节点
        /// </summary>
        public const string FlowEndNode = "FlowEndNode";
        /// <summary>
        /// 父流程WorkID
        /// </summary>
        public const string PWorkID = "PWorkID";
        /// <summary>
        /// 父流程编号
        /// </summary>
        public const string PFlowNo = "PFlowNo";
        #endregion 
    }
	/// <summary>
	/// 附件未读日志
	/// </summary>
    public class AthUnReadLog : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(AthUnReadLogAttr.WorkID);
            }
            set
            {
                SetValByKey(AthUnReadLogAttr.WorkID, value);
            }
        }
        /// <summary>
        /// 操作人
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(AthUnReadLogAttr.FK_Emp);
            }
            set
            {
                SetValByKey(AthUnReadLogAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 删除人员
        /// </summary>
        public string FK_EmpDept
        {
            get
            {
                return this.GetValStringByKey(AthUnReadLogAttr.FK_EmpDept);
            }
            set
            {
                SetValByKey(AthUnReadLogAttr.FK_EmpDept, value);
            }
        }
        public string FK_EmpDeptName
        {
            get
            {
                return this.GetValStringByKey(AthUnReadLogAttr.FK_EmpDeptName);
            }
            set
            {
                SetValByKey(AthUnReadLogAttr.FK_EmpDeptName, value);
            }
        }
        public string BeiZhu
        {
            get
            {
                return this.GetValStringByKey(AthUnReadLogAttr.BeiZhu);
            }
            set
            {
                SetValByKey(AthUnReadLogAttr.BeiZhu, value);
            }
        }
        public string BeiZhuHtml
        {
            get
            {
                return this.GetValHtmlStringByKey(AthUnReadLogAttr.BeiZhu);
            }
        }
        /// <summary>
        /// 记录日期
        /// </summary>
        public string SendDT
        {
            get
            {
                return this.GetValStringByKey(AthUnReadLogAttr.SendDT);
            }
            set
            {
                SetValByKey(AthUnReadLogAttr.SendDT, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(AthUnReadLogAttr.FK_Flow);
            }
            set
            {
                SetValByKey(AthUnReadLogAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 流程类别
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(AthUnReadLogAttr.FlowName);
            }
            set
            {
                SetValByKey(AthUnReadLogAttr.FlowName, value);
            }
        }
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(AthUnReadLogAttr.FK_Node);
            }
            set
            {
                SetValByKey(AthUnReadLogAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStringByKey(AthUnReadLogAttr.NodeName);
            }
            set
            {
                SetValByKey(AthUnReadLogAttr.NodeName, value);
            }
        }
        #endregion

        #region 构造函数
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
        /// 附件未读日志
        /// </summary>
        public AthUnReadLog() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_AthUnReadLog", "附件未读日志");
                 
                // 流程基础数据。
                map.AddMyPK(false);
                map.AddDDLEntities(GenerWorkFlowAttr.FK_Dept, null, "部门", new Port.Depts(), false);
                map.AddTBString(GenerWorkFlowAttr.Title, null, "标题", true, true, 0, 100, 100);
                map.AddTBInt(GenerWorkFlowAttr.WorkID, 0, "WorkID", false, false);
                map.AddTBString(GERptAttr.FlowStarter, null, "发起人", true, true, 0, 100, 100);
                map.AddTBDateTime(GERptAttr.FlowStartRDT, null, "发起时间", true, true);
                map.AddDDLEntities(GenerWorkFlowAttr.FK_NY, null, "年月", new BP.Pub.NYs(), false);
                map.AddDDLEntities(GenerWorkFlowAttr.FK_Flow, null, "流程", new Flows(), false);


                map.AddTBInt(AthUnReadLogAttr.FK_Node, 0, "节点ID", true, true);
                map.AddTBString(AthUnReadLogAttr.NodeName, null, "节点名称", true, true, 0, 20, 10);

                //删除信息.
                map.AddTBString(AthUnReadLogAttr.FK_Emp, null, "人员", true, true, 0, 20, 10);
                map.AddTBString(AthUnReadLogAttr.FK_EmpDept, null, "人员部门", true, true, 0, 20, 10);
                map.AddTBString(AthUnReadLogAttr.FK_EmpDeptName, null, "人员名称", true, true, 0, 200, 10);
                map.AddTBString(AthUnReadLogAttr.BeiZhu, "", "内容", true, true, 0, 4000, 10);
                map.AddTBDateTime(AthUnReadLogAttr.SendDT, null, "日期", true, true);

                //查询.
                map.AddSearchAttr(GenerWorkFlowAttr.FK_Dept);
                map.AddSearchAttr(GenerWorkFlowAttr.FK_NY);
                map.AddSearchAttr(GenerWorkFlowAttr.FK_Flow);

               // map.AddHidden(FlowDataAttr.FlowEmps, " LIKE ", "'%@@WebUser.No%'");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// 附件未读日志s 
	/// </summary>
	public class AthUnReadLogs : EntitiesMyPK
	{	 
		#region 构造
		/// <summary>
		/// 附件未读日志s
		/// </summary>
		public AthUnReadLogs()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new AthUnReadLog();
			}
		}
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<AthUnReadLog> ToJavaList()
        {
            return (System.Collections.Generic.IList<AthUnReadLog>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<AthUnReadLog> Tolist()
        {
            System.Collections.Generic.List<AthUnReadLog> list = new System.Collections.Generic.List<AthUnReadLog>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((AthUnReadLog)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
	
}
