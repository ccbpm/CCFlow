using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.WF.Port;

namespace BP.Frm
{
	/// <summary>
	/// 单据可创建的人员属性
	/// </summary>
	public class EmpCreateAttr
	{
		/// <summary>
		/// 表单ID
		/// </summary>
		public const string FrmID="FrmID";
		/// <summary>
		/// 人员
		/// </summary>
		public const string FK_Emp="FK_Emp";
	}
	/// <summary>
	/// 单据可创建的人员
	/// 表单ID的到人员有两部分组成.	 
	/// 记录了从一个表单ID到其他的多个表单ID.
	/// 也记录了到这个表单ID的其他的表单ID.
	/// </summary>
	public class EmpCreate :EntityMM
	{
		#region 基本属性
		/// <summary>
		///表单ID
		/// </summary>
		public int  FrmID
		{
			get
			{
				return this.GetValIntByKey(EmpCreateAttr.FrmID);
			}
			set
			{
				this.SetValByKey(EmpCreateAttr.FrmID,value);
			}
		}
		/// <summary>
		/// 到人员
		/// </summary>
		public string FK_Emp
		{
			get
			{
				return this.GetValStringByKey(EmpCreateAttr.FK_Emp);
			}
			set
			{
				this.SetValByKey(EmpCreateAttr.FK_Emp,value);
			}
		}
        public string FK_EmpT
        {
            get
            {
                return this.GetValRefTextByKey(EmpCreateAttr.FK_Emp);
            }
        }
		#endregion 

		#region 构造方法
		/// <summary>
		/// 单据可创建的人员
		/// </summary>
		public EmpCreate()
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

                Map map = new Map("Frm_EmpCreate", "单据可创建的人员");

                map.AddTBStringPK(EmpCreateAttr.FrmID,null,"表单",true,true,1,100,100 );
                map.AddDDLEntitiesPK(EmpCreateAttr.FK_Emp, null, "人员", new Emps(), true);

                this._enMap = map;
                return this._enMap;
            }
		}
		#endregion
	}
	/// <summary>
	/// 单据可创建的人员
	/// </summary>
    public class EmpCreates : EntitiesMM
    {
        #region 构造函数.
        /// <summary>
        /// 单据可创建的人员
        /// </summary>
        public EmpCreates() { }
        /// <summary>
        /// 单据可创建的人员
        /// </summary>
        /// <param name="NodeID">表单IDID</param>
        public EmpCreates(int NodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(EmpCreateAttr.FrmID, NodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 单据可创建的人员
        /// </summary>
        /// <param name="EmpNo">EmpNo </param>
        public EmpCreates(string EmpNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(EmpCreateAttr.FK_Emp, EmpNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EmpCreate();
            }
        }
        #endregion 构造函数.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EmpCreate> ToJavaList()
        {
            return (System.Collections.Generic.IList<EmpCreate>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EmpCreate> Tolist()
        {
            System.Collections.Generic.List<EmpCreate> list = new System.Collections.Generic.List<EmpCreate>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EmpCreate)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
