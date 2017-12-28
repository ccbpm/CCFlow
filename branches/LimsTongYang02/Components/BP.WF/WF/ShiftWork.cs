using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port; 
using BP.En;

namespace BP.WF
{
	/// <summary>
	/// 移交记录
	/// </summary>
    public class ShiftWorkAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 移交原因
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// 移交人
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 移交人名成
        /// </summary>
        public const string FK_EmpName = "FK_EmpName";
        /// <summary>
        /// 移交时间
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 是否读取？
        /// </summary>
        public const string IsRead = "IsRead";
        /// <summary>
        /// 移交给
        /// </summary>
        public const string ToEmp = "ToEmp";
        /// <summary>
        /// 移交给人员名称
        /// </summary>
        public const string ToEmpName = "ToEmpName";
        #endregion
    }
	/// <summary>
	/// 移交记录
	/// </summary>
	public class ShiftWork : EntityMyPK
	{		
		#region 基本属性
		/// <summary>
		/// 工作ID
		/// </summary>
        public Int64 WorkID
		{
			get
			{
				return this.GetValInt64ByKey(ShiftWorkAttr.WorkID);
			}
			set
			{
				SetValByKey(ShiftWorkAttr.WorkID,value);
			}
		}
		/// <summary>
		/// 工作节点
		/// </summary>
		public int FK_Node
		{
			get
			{
				return this.GetValIntByKey(ShiftWorkAttr.FK_Node);
			}
			set
			{
				SetValByKey(ShiftWorkAttr.FK_Node,value);
			}
		}
        /// <summary>
        /// 是否读取？
        /// </summary>
        public bool IsRead
        {
            get
            {
                return this.GetValBooleanByKey(ShiftWorkAttr.IsRead);
            }
            set
            {
                SetValByKey(ShiftWorkAttr.IsRead, value);
            }
        }
        /// <summary>
        /// ToEmpName
        /// </summary>
        public string ToEmpName
        {
            get
            {
                return this.GetValStringByKey(ShiftWorkAttr.ToEmpName);
            }
            set
            {
                SetValByKey(ShiftWorkAttr.ToEmpName, value);
            }
        }
        /// <summary>
        /// 移交人名称.
        /// </summary>
        public string FK_EmpName
        {
            get
            {
                return this.GetValStringByKey(ShiftWorkAttr.FK_EmpName);
            }
            set
            {
                SetValByKey(ShiftWorkAttr.FK_EmpName, value);
            }
        }
        /// <summary>
        /// 移交时间
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(ShiftWorkAttr.RDT);
            }
            set
            {
                SetValByKey(ShiftWorkAttr.RDT, value);
            }
        }
        /// <summary>
        /// 移交意见
        /// </summary>
		public string Note
		{
			get
			{
				return this.GetValStringByKey(ShiftWorkAttr.Note);
			}
			set
			{
				SetValByKey(ShiftWorkAttr.Note,value);
			}
		}
        /// <summary>
        /// 移交意见html格式
        /// </summary>
        public string NoteHtml
        {
            get
            {
                return this.GetValHtmlStringByKey(ShiftWorkAttr.Note);
            }
        }
        /// <summary>
        /// 移交人
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(ShiftWorkAttr.FK_Emp);
            }
            set
            {
                SetValByKey(ShiftWorkAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 移交给
        /// </summary>
        public string ToEmp
        {
            get
            {
                return this.GetValStringByKey(ShiftWorkAttr.ToEmp);
            }
            set
            {
                SetValByKey(ShiftWorkAttr.ToEmp, value);
            }
        }
		#endregion

		#region 构造函数
		/// <summary>
		/// 移交记录
		/// </summary>
		public ShiftWork()
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

                Map map = new Map("WF_ShiftWork", "移交记录");

                map.AddMyPK();

                map.AddTBInt(ShiftWorkAttr.WorkID, 0, "工作ID", true, true);
                map.AddTBInt(ShiftWorkAttr.FK_Node, 0, "FK_Node", true, true);
                map.AddTBString(ShiftWorkAttr.FK_Emp, null, "移交人", true, true, 0, 40, 10);
                map.AddTBString(ShiftWorkAttr.FK_EmpName, null, "移交人名称", true, true, 0, 40, 10);

                map.AddTBString(ShiftWorkAttr.ToEmp, null, "移交给", true, true, 0, 40, 10);
                map.AddTBString(ShiftWorkAttr.ToEmpName, null, "移交给名称", true, true, 0, 40, 10);

                map.AddTBDateTime(ShiftWorkAttr.RDT, null, "移交时间", true, true);
                map.AddTBString(ShiftWorkAttr.Note, null, "移交原因", true, true, 0, 2000, 10);

                map.AddTBInt(ShiftWorkAttr.IsRead, 0, "是否读取？", true, true);
                this._enMap = map;
                return this._enMap;
            }
        }
        protected override bool beforeInsert()
        {
            this.MyPK = BP.DA.DBAccess.GenerOIDByGUID().ToString();
            this.RDT = DataType.CurrentDataTime;
            return base.beforeInsert();
        }
		#endregion	 
	}
	/// <summary>
	/// 移交记录s 
	/// </summary>
	public class ShiftWorks : Entities
	{	 
		#region 构造
		/// <summary>
		/// 移交记录s
		/// </summary>
		public ShiftWorks()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new ShiftWork();
			}
		}
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ShiftWork> ToJavaList()
        {
            return (System.Collections.Generic.IList<ShiftWork>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ShiftWork> Tolist()
        {
            System.Collections.Generic.List<ShiftWork> list = new System.Collections.Generic.List<ShiftWork>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ShiftWork)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
	
}
