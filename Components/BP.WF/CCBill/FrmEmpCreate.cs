﻿using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.WF.Port;

namespace BP.WF.CCBill
{
	/// <summary>
	/// 单据可创建的人员属性
	/// </summary>
	public class FrmEmpCreateAttr
	{
		/// <summary>
		/// 节点
		/// </summary>
		public const string FK_Bill="FK_Bill";
		/// <summary>
		/// 人员
		/// </summary>
		public const string FK_Emp="FK_Emp";
	}
	/// <summary>
	/// 单据可创建的人员
	/// 节点的到人员有两部分组成.	 
	/// 记录了从一个节点到其他的多个节点.
	/// 也记录了到这个节点的其他的节点.
	/// </summary>
	public class FrmEmpCreate :EntityMM
	{
		#region 基本属性
		/// <summary>
		///节点
		/// </summary>
		public int  FK_Bill
		{
			get
			{
				return this.GetValIntByKey(FrmEmpCreateAttr.FK_Bill);
			}
			set
			{
				this.SetValByKey(FrmEmpCreateAttr.FK_Bill,value);
			}
		}
		/// <summary>
		/// 到人员
		/// </summary>
		public string FK_Emp
		{
			get
			{
				return this.GetValStringByKey(FrmEmpCreateAttr.FK_Emp);
			}
			set
			{
				this.SetValByKey(FrmEmpCreateAttr.FK_Emp,value);
			}
		}
        public string FK_EmpT
        {
            get
            {
                return this.GetValRefTextByKey(FrmEmpCreateAttr.FK_Emp);
            }
        }
		#endregion 

		#region 构造方法
		/// <summary>
		/// 单据可创建的人员
		/// </summary>
		public FrmEmpCreate()
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

                Map map = new Map("_FrmEmpCreate", "单据可创建的人员");

                map.AddTBStringPK(FrmEmpCreateAttr.FK_Bill,null,"表单",true,true,1,100,100 );
                map.AddDDLEntitiesPK(FrmEmpCreateAttr.FK_Emp, null, "人员", new Emps(), true);

                this._enMap = map;
                return this._enMap;
            }
		}
		#endregion
	}
	/// <summary>
	/// 单据可创建的人员
	/// </summary>
    public class FrmEmpCreates : EntitiesMM
    {
        #region 构造函数.
        /// <summary>
        /// 单据可创建的人员
        /// </summary>
        public FrmEmpCreates() { }
        /// <summary>
        /// 单据可创建的人员
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public FrmEmpCreates(int NodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmEmpCreateAttr.FK_Bill, NodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 单据可创建的人员
        /// </summary>
        /// <param name="EmpNo">EmpNo </param>
        public FrmEmpCreates(string EmpNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmEmpCreateAttr.FK_Emp, EmpNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmEmpCreate();
            }
        }
        #endregion 构造函数.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmEmpCreate> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmEmpCreate>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmEmpCreate> Tolist()
        {
            System.Collections.Generic.List<FrmEmpCreate> list = new System.Collections.Generic.List<FrmEmpCreate>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmEmpCreate)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
