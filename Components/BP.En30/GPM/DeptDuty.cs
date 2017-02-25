using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.GPM
{
	/// <summary>
	/// 部门职务
	/// </summary>
	public class DeptDutyAttr  
	{
		#region 基本属性
		/// <summary>
		/// 部门
		/// </summary>
		public const  string FK_Dept="FK_Dept";
		/// <summary>
		/// 职务
		/// </summary>
		public const  string FK_Duty="FK_Duty";		 
		#endregion	
	}
	/// <summary>
    /// 部门职务 的摘要说明。
	/// </summary>
    public class DeptDuty : Entity
    {
        #region 基本属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;

            }
        }
        /// <summary>
        /// 部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(DeptDutyAttr.FK_Dept);
            }
            set
            {
                SetValByKey(DeptDutyAttr.FK_Dept, value);
            }
        }
        public string FK_DutyT
        {
            get
            {
                return this.GetValRefTextByKey(DeptDutyAttr.FK_Duty);
            }
        }
        /// <summary>
        ///职务
        /// </summary>
        public string FK_Duty
        {
            get
            {
                return this.GetValStringByKey(DeptDutyAttr.FK_Duty);
            }
            set
            {
                SetValByKey(DeptDutyAttr.FK_Duty, value);
            }
        }
        #endregion

        #region 扩展属性

        #endregion

        #region 构造函数
        /// <summary>
        /// 部门职务
        /// </summary> 
        public DeptDuty() { }
        /// <summary>
        /// 工作人员职务对应
        /// </summary>
        /// <param name="_empoid">部门</param>
        /// <param name="wsNo">职务编号</param> 	
        public DeptDuty(string _empoid, string wsNo)
        {
            this.FK_Dept = _empoid;
            this.FK_Duty = wsNo;
            if (this.Retrieve() == 0)
                this.Insert();
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

                Map map = new Map("Port_DeptDuty");
                map.EnDesc = "部门职务";
                map.Java_SetEnType(EnType.Dot2Dot); //实体类型，admin 系统管理员表，PowerAble 权限管理表,也是用户表,你要想把它加入权限管理里面请在这里设置。。

                map.AddTBStringPK(DeptDutyAttr.FK_Dept, null, "部门", false, false, 1, 15, 1);
                map.AddDDLEntitiesPK(DeptDutyAttr.FK_Duty, null, "职务", new Dutys(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// 部门职务 
	/// </summary>
	public class DeptDutys : Entities
	{
		#region 构造
		/// <summary>
		/// 部门职务
		/// </summary>
		public DeptDutys()
		{
		}
		/// <summary>
		/// 工作人员与职务集合
		/// </summary>
		public DeptDutys(string DutyNo)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(DeptDutyAttr.FK_Duty, DutyNo);
			qo.DoQuery();
		}		 
		#endregion

		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new DeptDuty();
			}
		}	
		#endregion 

		#region 查询方法
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DeptDuty> ToJavaList()
        {
            return (System.Collections.Generic.IList<DeptDuty>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DeptDuty> Tolist()
        {
            System.Collections.Generic.List<DeptDuty> list = new System.Collections.Generic.List<DeptDuty>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DeptDuty)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
