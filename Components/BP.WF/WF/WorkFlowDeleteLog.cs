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
	/// 流程删除日志
	/// </summary>
	public class WorkFlowDeleteLogAttr 
	{
		#region 基本属性
		/// <summary>
		/// 工作ID
		/// </summary>
		public const  string OID="OID";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 流程类别
        /// </summary>
        public const string FK_FlowSort = "FK_FlowSort";
		/// <summary>
		/// 删除人员
		/// </summary>
		public const  string Oper="Oper";
		/// <summary>
		/// 删除原因
		/// </summary>
		public const  string DeleteNote="DeleteNote";
        /// <summary>
        /// 删除日期
        /// </summary>
        public const string DeleteDT = "DeleteDT";
        /// <summary>
        /// 删除人员
        /// </summary>
        public const string OperDept = "OperDept";
        /// <summary>
        /// 删除人员名称
        /// </summary>
        public const string OperDeptName = "OperDeptName";
        /// <summary>
        /// 删除节点节点
        /// </summary>
        public const string DeleteNode = "DeleteNode";
        /// <summary>
        /// 删除节点节点名称
        /// </summary>
        public const string DeleteNodeName = "DeleteNodeName";        
        /// <summary>
        /// 删除节点后是否需要原路返回？
        /// </summary>
        public const string IsBackTracking = "IsBackTracking";
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
        public const string FlowStartDeleteDT = "FlowStartDeleteDT";
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
        public const string FlowEnderDeleteDT = "FlowEnderDeleteDT";
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
	/// 流程删除日志
	/// </summary>
    public class WorkFlowDeleteLog : EntityOID
    {
        #region 基本属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 OID
        {
            get
            {
                return this.GetValInt64ByKey(WorkFlowDeleteLogAttr.OID);
            }
            set
            {
                SetValByKey(WorkFlowDeleteLogAttr.OID, value);
            }
        }
        /// <summary>
        /// 操作人
        /// </summary>
        public string Oper
        {
            get
            {
                return this.GetValStringByKey(WorkFlowDeleteLogAttr.Oper);
            }
            set
            {
                SetValByKey(WorkFlowDeleteLogAttr.Oper, value);
            }
        }
        /// <summary>
        /// 删除人员
        /// </summary>
        public string OperDept
        {
            get
            {
                return this.GetValStringByKey(WorkFlowDeleteLogAttr.OperDept);
            }
            set
            {
                SetValByKey(WorkFlowDeleteLogAttr.OperDept, value);
            }
        }
        public string OperDeptName
        {
            get
            {
                return this.GetValStringByKey(WorkFlowDeleteLogAttr.OperDeptName);
            }
            set
            {
                SetValByKey(WorkFlowDeleteLogAttr.OperDeptName, value);
            }
        }
        public string DeleteNote
        {
            get
            {
                return this.GetValStringByKey(WorkFlowDeleteLogAttr.DeleteNote);
            }
            set
            {
                SetValByKey(WorkFlowDeleteLogAttr.DeleteNote, value);
            }
        }
        public string DeleteNoteHtml
        {
            get
            {
                return this.GetValHtmlStringByKey(WorkFlowDeleteLogAttr.DeleteNote);
            }
        }
        /// <summary>
        /// 记录日期
        /// </summary>
        public string DeleteDT
        {
            get
            {
                return this.GetValStringByKey(WorkFlowDeleteLogAttr.DeleteDT);
            }
            set
            {
                SetValByKey(WorkFlowDeleteLogAttr.DeleteDT, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(WorkFlowDeleteLogAttr.FK_Flow);
            }
            set
            {
                SetValByKey(WorkFlowDeleteLogAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 流程类别
        /// </summary>
        public string FK_FlowSort
        {
            get
            {
                return this.GetValStringByKey(WorkFlowDeleteLogAttr.FK_FlowSort);
            }
            set
            {
                SetValByKey(WorkFlowDeleteLogAttr.FK_FlowSort, value);
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
        /// 流程删除日志
        /// </summary>
        public WorkFlowDeleteLog() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_WorkFlowDeleteLog", "流程删除日志");
                 
                // 流程基础数据。
                map.AddTBIntPKOID(); 
                map.AddTBInt(GenerWorkFlowAttr.FID, 0, "FID", false, false);
                map.AddDDLEntities(GenerWorkFlowAttr.FK_Dept, null, "部门", new Port.Depts(), false);
                map.AddTBString(GenerWorkFlowAttr.Title, null, "标题", true, true, 0, 100, 100);
                map.AddTBString(GERptAttr.FlowStarter, null, "发起人", true, true, 0, 100, 100);
                map.AddTBDateTime(GERptAttr.FlowStartRDT, null, "发起时间", true, true);
                map.AddDDLEntities(GenerWorkFlowAttr.FK_NY, null, "年月", new BP.Pub.NYs(), false);
                map.AddDDLEntities(GenerWorkFlowAttr.FK_Flow, null, "流程", new Flows(), false);
                map.AddTBDateTime(GERptAttr.FlowEnderRDT, null, "最后处理时间", true, true);
                map.AddTBInt(GERptAttr.FlowEndNode, 0, "停留节点", true, true);
                map.AddTBFloat(GERptAttr.FlowDaySpan, 0, "跨度(天)", true, true);
                map.AddTBString(GERptAttr.FlowEmps, null, "参与人", false, false, 0, 100, 100);

                //删除信息.
                map.AddTBString(WorkFlowDeleteLogAttr.Oper, null, "删除人员", true, true, 0, 20, 10);
                map.AddTBString(WorkFlowDeleteLogAttr.OperDept, null, "删除人员部门", true, true, 0, 20, 10);
                map.AddTBString(WorkFlowDeleteLogAttr.OperDeptName, null, "删除人员名称", true, true, 0, 200, 10);
                map.AddTBString(WorkFlowDeleteLogAttr.DeleteNote, "", "删除原因", true, true, 0, 4000, 10);
                map.AddTBDateTime(WorkFlowDeleteLogAttr.DeleteDT, null, "删除日期", true, true);

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
	/// 流程删除日志s 
	/// </summary>
	public class WorkFlowDeleteLogs : Entities
	{	 
		#region 构造
		/// <summary>
		/// 流程删除日志s
		/// </summary>
		public WorkFlowDeleteLogs()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new WorkFlowDeleteLog();
			}
		}
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<WorkFlowDeleteLog> ToJavaList()
        {
            return (System.Collections.Generic.IList<WorkFlowDeleteLog>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<WorkFlowDeleteLog> Tolist()
        {
            System.Collections.Generic.List<WorkFlowDeleteLog> list = new System.Collections.Generic.List<WorkFlowDeleteLog>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((WorkFlowDeleteLog)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
	
}
