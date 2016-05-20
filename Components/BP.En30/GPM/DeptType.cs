using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 部门类型
    /// </summary>
    public class DeptTypeAttr : EntityNoNameAttr
    {
    }
	/// <summary>
    ///  部门类型
	/// </summary>
	public class DeptType :EntityNoName
    {
        #region 属性
        #endregion
     
		#region 构造方法
		/// <summary>
		/// 部门类型
		/// </summary>
		public DeptType()
        {
        }
        /// <summary>
        /// 部门类型
        /// </summary>
        /// <param name="_No"></param>
        public DeptType(string _No) : base(_No) { }
		#endregion 

		/// <summary>
		/// 部门类型Map
		/// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Port_DeptType");
                map.EnDesc = "部门类型";
                map.CodeStruct = "2";

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBStringPK(DeptTypeAttr.No, null, "编号", true, true, 2, 2, 2);
                map.AddTBString(DeptTypeAttr.Name, null, "名称", true, false, 1, 50, 20);
                this._enMap = map;
                return this._enMap;
            }
        }
	}
	/// <summary>
    /// 部门类型
	/// </summary>
    public class DeptTypes : EntitiesNoName
	{
		/// <summary>
		/// 部门类型s
		/// </summary>
        public DeptTypes() { }
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
                return new DeptType();
			}
		}

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DeptType> ToJavaList()
        {
            return (System.Collections.Generic.IList<DeptType>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DeptType> Tolist()
        {
            System.Collections.Generic.List<DeptType> list = new System.Collections.Generic.List<DeptType>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DeptType)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
