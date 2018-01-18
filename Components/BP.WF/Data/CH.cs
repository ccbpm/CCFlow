using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.Port;
using BP.Web;
using BP.Sys;

namespace BP.WF.Data
{
    /// <summary>
    /// 完成状态
    /// </summary>
    public enum CHSta
    {
        /// <summary>
        /// 及时完成
        /// </summary>
        JiShi,
        /// <summary>
        /// 按期完成
        /// </summary>
        AnQi,
        /// <summary>
        /// 预期完成
        /// </summary>
        YuQi,
        /// <summary>
        /// 超期完成
        /// </summary>
        ChaoQi
    }
	/// <summary>
	/// 时效考核属性
	/// </summary>
    public class CHAttr
    {
        #region 属性
        public const string MyPK = "MyPK";
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_FlowT = "FK_FlowT";

        /// <summary>
        /// 发送人
        /// </summary>
        public const string Sender = "Sender";
        /// <summary>
        /// 发送人名称
        /// </summary>
        public const string SenderT = "SenderT";

        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 节点编号
        /// </summary>
        public const string FK_NodeT = "FK_NodeT";

        /// <summary>
        /// 部门编号
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 部门编号
        /// </summary>
        public const string FK_DeptT = "FK_DeptT";
        /// <summary>
        /// 当事人
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 当事人名称
        /// </summary>
        public const string FK_EmpT = "FK_EmpT";
        /// <summary>
        /// 相关当事人
        /// </summary>
        public const string GroupEmps = "GroupEmps";
        /// <summary>
        /// 相关当事人名称
        /// </summary>
        public const string GroupEmpsNames = "GroupEmpsNames";
        /// <summary>
        /// 相关当事人数量
        /// </summary>
        public const string GroupEmpsNum = "GroupEmpsNum";
        /// <summary>
        /// 限期
        /// </summary>
        public const string TimeLimit = "TimeLimit";
        /// <summary>
        /// 实际期限
        /// </summary>
        public const string UseDays = "UseDays";
        /// <summary>
        /// 使用时间
        /// </summary>
        public const string UseHours = "UseHours";
        /// <summary>
        /// 逾期
        /// </summary>
        public const string OverDays = "OverDays";
        /// <summary>
        /// 预期
        /// </summary>
        public const string OverHours = "OverHours";
        /// <summary>
        /// 用时（分钟）
        /// </summary>
        public const string UseMinutes = "UseMinutes";
        /// <summary>
        /// 超时（分钟）
        /// </summary>
        public const string OverMinutes = "OverMinutes";
        /// <summary>
        /// 状态
        /// </summary>
        public const string CHSta = "CHSta";
        /// <summary>
        /// 年月
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// 考核方式
        /// </summary>
        public const string DTSWay = "DTSWay";
        /// <summary>
        /// 周
        /// </summary>
        public const string WeekNum = "WeekNum";
        /// <summary>
        /// FID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 时间从
        /// </summary>
        public const string DTFrom = "DTFrom";
        /// <summary>
        /// 时间到
        /// </summary>
        public const string DTTo = "DTTo";
        /// <summary>
        /// 应完成日期
        /// </summary>
        public const string SDT = "SDT";
        /// <summary>
        /// 总扣分
        /// </summary>
        public const string Points = "Points";
        #endregion
    }
	/// <summary>
	/// 时效考核
	/// </summary> 
    public class CH : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 发送人
        /// </summary>
        public string Sender
        {
            get
            {
                return this.GetValStringByKey(CHAttr.Sender);
            }
            set
            {
                this.SetValByKey(CHAttr.Sender, value);
            }
        }
        /// <summary>
        /// 发送人名称
        /// </summary>
        public string SenderT
        {
            get
            {
                return this.GetValStringByKey(CHAttr.SenderT);
            }
            set
            {
                this.SetValByKey(CHAttr.SenderT, value);
            }
        }
        /// <summary>
        /// 考核状态
        /// </summary>
        public CHSta CHSta
        {
            get
            {
                return (CHSta)this.GetValIntByKey(CHAttr.CHSta);
            }
            set
            {
                this.SetValByKey(CHAttr.CHSta, (int)value);
            }
        }
        /// <summary>
        /// 时间到
        /// </summary>
        public string DTTo
        {
            get
            {
                return this.GetValStringByKey(CHAttr.DTTo);
            }
            set
            {
                this.SetValByKey(CHAttr.DTTo, value);
            }
        }
        /// <summary>
        /// 时间从
        /// </summary>
        public string DTFrom
        {
            get
            {
                return this.GetValStringByKey(CHAttr.DTFrom);
            }
            set
            {
                this.SetValByKey(CHAttr.DTFrom, value);
            }
        }
        /// <summary>
        /// 应完成日期
        /// </summary>
        public string SDT
        {
            get
            {
                return this.GetValStringByKey(CHAttr.SDT);
            }
            set
            {
                this.SetValByKey(CHAttr.SDT, value);
            }
        }
        /// <summary>
        /// 流程标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStringByKey(CHAttr.Title);
            }
            set
            {
                this.SetValByKey(CHAttr.Title, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(CHAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(CHAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 流程
        /// </summary>
        public string FK_FlowT
        {
            get
            {
                return this.GetValStringByKey(CHAttr.FK_FlowT);
            }
            set
            {
                this.SetValByKey(CHAttr.FK_FlowT, value);
            }
        }
        /// <summary>
        /// 限期
        /// </summary>
        public int TimeLimit
        {
            get
            {
                return this.GetValIntByKey(CHAttr.TimeLimit);
            }
            set
            {
                this.SetValByKey(CHAttr.TimeLimit, value);
            }
        }
        /// <summary>
        /// 实际完成用时.
        /// </summary>
        public float UseDays
        {
            get
            {
                return this.GetValFloatByKey(CHAttr.UseDays);
            }
            set
            {
                this.SetValByKey(CHAttr.UseDays, value);
            }
        }
        /// <summary>
        /// 逾期时间
        /// </summary>
        public float OverDays
        {
            get
            {
                return this.GetValFloatByKey(CHAttr.OverDays);
            }
            set
            {
                this.SetValByKey(CHAttr.OverDays, value);
            }
        }
        /// <summary>
        /// 用时（分钟）
        /// </summary>
        public float UseMinutes
        {
            get
            {
                return this.GetValFloatByKey(CHAttr.UseMinutes);
            }
            set
            {
                this.SetValByKey(CHAttr.UseMinutes, value);
            }
        }
        /// <summary>
        /// 超时（分钟）
        /// </summary>
        public float OverMinutes
        {
            get
            {
                return this.GetValFloatByKey(CHAttr.OverMinutes);
            }
            set
            {
                this.SetValByKey(CHAttr.OverMinutes, value);
            }
        }
        /// <summary>
        /// 操作人员
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(CHAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(CHAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 人员
        /// </summary>
        public string FK_EmpT
        {
            get
            {
                return this.GetValStringByKey(CHAttr.FK_EmpT);
            }
            set
            {
                this.SetValByKey(CHAttr.FK_EmpT, value);
            }
        }
        /// <summary>
        /// 相关当事人
        /// </summary>
        public string GroupEmps
        {
            get
            {
                return this.GetValStringByKey(CHAttr.GroupEmps);
            }
            set
            {
                this.SetValByKey(CHAttr.GroupEmps, value);
            }
        }
        /// <summary>
        /// 相关当事人名称
        /// </summary>
        public string GroupEmpsNames
        {
            get
            {
                return this.GetValStringByKey(CHAttr.GroupEmpsNames);
            }
            set
            {
                this.SetValByKey(CHAttr.GroupEmpsNames, value);
            }
        }
        /// <summary>
        /// 相关当事人数量
        /// </summary>
        public int GroupEmpsNum
        {
            get
            {
                return this.GetValIntByKey(CHAttr.GroupEmpsNum);
            }
            set
            {
                this.SetValByKey(CHAttr.GroupEmpsNum, value);
            }
        }
        /// <summary>
        /// 部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStrByKey(CHAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(CHAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string FK_DeptT
        {
            get
            {
                return this.GetValStrByKey(CHAttr.FK_DeptT);
            }
            set
            {
                this.SetValByKey(CHAttr.FK_DeptT, value);
            }
        }
        /// <summary>
        /// 年月
        /// </summary>
        public string FK_NY
        {
            get
            {
                return this.GetValStrByKey(CHAttr.FK_NY);
            }
            set
            {
                this.SetValByKey(CHAttr.FK_NY, value);
            }
        }
        /// <summary>
        /// 考核方式
        /// </summary>
        public int DTSWay
        {
            get
            {
                return this.GetValIntByKey(CHAttr.DTSWay);
            }
            set
            {
                this.SetValByKey(CHAttr.DTSWay, value);
            }
        }
        /// <summary>
        /// 周
        /// </summary>
        public int WeekNum
        {
            get
            {
                return this.GetValIntByKey(CHAttr.WeekNum);
            }
            set
            {
                this.SetValByKey(CHAttr.WeekNum, value);
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(CHAttr.WorkID);
            }
            set
            {
                this.SetValByKey(CHAttr.WorkID, value);
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(CHAttr.FID);
            }
            set
            {
                this.SetValByKey(CHAttr.FID, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(CHAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(CHAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string FK_NodeT
        {
            get
            {
                return this.GetValStrByKey(CHAttr.FK_NodeT);
            }
            set
            {
                this.SetValByKey(CHAttr.FK_NodeT, value);
            }
        }
        /// <summary>
        /// 总扣分
        /// </summary>
        public float Points
        {
            get
            {
                return this.GetValFloatByKey(CHAttr.Points);
            }
            set
            {
                this.SetValByKey(CHAttr.Points, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = false;
                uac.IsView = true;
                return uac;
            }
        }
        /// <summary>
        /// 时效考核
        /// </summary>
        public CH() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pk"></param>
        public CH(string pk)
            : base(pk)
        {
        }
        #endregion

        #region Map
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_CH", "时效考核");

                map.AddMyPK();

                #region 基本属性.
                map.AddTBInt(CHAttr.WorkID, 0, "工作ID", false, true);
                map.AddTBInt(CHAttr.FID, 0, "FID", false, true);
                map.AddTBString(CHAttr.Title, null, "标题", false, false, 0, 900, 5);

                map.AddTBString(CHAttr.FK_Flow, null, "流程", false, false, 3, 3, 3);
                map.AddTBString(CHAttr.FK_FlowT, null, "流程名称", true, true, 0, 200, 5);

                map.AddTBInt(CHAttr.FK_Node, 0, "节点", false, false);
                map.AddTBString(CHAttr.FK_NodeT, null, "节点名称", true, true, 0, 200, 5);


                map.AddTBString(CHAttr.Sender, null, "发送人", false, false, 0, 200, 3);
                map.AddTBString(CHAttr.SenderT, null, "发送人名称", true, true, 0, 200, 5);


                map.AddTBString(CHAttr.FK_Emp, null, "当事人", true, true, 0, 30, 3);
                map.AddTBString(CHAttr.FK_EmpT, null, "当事人名称", true, true, 0, 200, 5);

                //为邓州增加的属性. 解决多人处理一个节点的工作的问题.
                map.AddTBString(CHAttr.GroupEmps, null, "相关当事人", true, true, 0, 400, 3);
                map.AddTBString(CHAttr.GroupEmpsNames, null, "相关当事人名称", true, true, 0, 900, 3);
                map.AddTBInt(CHAttr.GroupEmpsNum, 1, "相关当事人数量", false, false);


                map.AddTBString(CHAttr.DTFrom, null, "任务下达时间", true, true, 0, 50, 5);
                map.AddTBString(CHAttr.DTTo, null, "任务处理时间", true, true, 0, 50, 5);
                map.AddTBString(CHAttr.SDT, null, "应完成日期", true, true, 0, 50, 5);

                map.AddTBString(CHAttr.FK_Dept, null, "隶属部门", true, true, 0, 50, 5);
                map.AddTBString(CHAttr.FK_DeptT, null, "部门名称", true, true, 0, 500, 5);
                map.AddTBString(CHAttr.FK_NY, null, "隶属月份", true, true, 0, 10, 10);
                map.AddDDLSysEnum(CHAttr.DTSWay, 0, "考核方式", true, true, CHAttr.DTSWay, "@0=不考核@1=按照时效考核@2=按照工作量考核");
                #endregion 基本属性.

                #region 计算属性.
                map.AddTBString(CHAttr.TimeLimit, null, "规定限期", true, true, 0, 50, 5);
                map.AddTBFloat(CHAttr.OverMinutes, 0, "逾期分钟", false, true);
                map.AddTBFloat(CHAttr.UseDays, 0, "实际使用天", false, true);
                map.AddTBFloat(CHAttr.OverDays, 0, "逾期天", false, true);
                map.AddTBInt(CHAttr.CHSta, 0, "状态", true, true);
                map.AddTBInt(CHAttr.WeekNum, 0, "第几周", false, true);
                map.AddTBFloat(CHAttr.Points, 0, "总扣分", true, true);
                map.AddTBIntMyNum();
                #endregion 计算属性.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
       
    }
	/// <summary>
	/// 时效考核s
	/// </summary>
	public class CHs :Entities
	{
		#region 构造方法属性
		/// <summary>
        /// 时效考核s
		/// </summary>
		public CHs(){}
		#endregion 

		#region 属性
		/// <summary>
        /// 时效考核
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new CH();
			}
		}
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CH> ToJavaList()
        {
            return (System.Collections.Generic.IList<CH>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CH> Tolist()
        {
            System.Collections.Generic.List<CH> list = new System.Collections.Generic.List<CH>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CH)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
