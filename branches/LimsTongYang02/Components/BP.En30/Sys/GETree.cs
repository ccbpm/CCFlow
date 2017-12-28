using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.En
{
	/// <summary>
	/// 属性
	/// </summary>
	public class GETreeAttr : EntityNoNameAttr
	{
    }
	/// <summary>
	/// 树结构实体
	/// </summary>
    public class GETree : EntityNoName
    {
        #region 构造
        public override string ToString()
        {
            return this.PhysicsTable;
        }
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        public GETree()
        {

        }
        /// <summary>
        /// 编号
        /// </summary>
        /// <param name="no">编号</param>
        public GETree(string no)  :base(no)
        {

        }
        public GETree(string sftable, string tableDesc)
        {
            this.PhysicsTable = sftable;
            this.Desc = tableDesc;
        }
        public override Map EnMap
        {
            get
            {
               // if (this._enMap != null) return this._enMap;
                Map map = new Map(this.PhysicsTable,this.Desc);
                map.IsAutoGenerNo = true;

                map.Java_SetDepositaryOfEntity( Depositary.Application);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.App);
                map.IsAutoGenerNo = true;

                map.AddTBStringPK(GETreeAttr.No, null, "编号", true, true, 1, 30, 3);
                map.AddTBString(GETreeAttr.Name, null, "名称", true, false, 1, 60, 500);
                return map;
            //    this._enMap = map;
                //return this._enMap;
            }
        }
        public string PhysicsTable = null;
        public string Desc = null;

        #endregion
    }
	/// <summary>
    /// 树结构实体s
	/// </summary>
	public class GETrees : EntitiesNoName
	{
        /// <summary>
        /// 物理表
        /// </summary>
        public string SFTable = null;
        public string Desc = null;

		/// <summary>
		/// GETrees
		/// </summary>
        public GETrees()
		{
		}
        public GETrees(string sftable, string tableDesc)
        {
            this.SFTable = sftable;
            this.Desc = tableDesc;
        }
        public override Entity GetNewEntity
        {
            get 
            {
                return new GETree(this.SFTable, this.Desc);
            }
        }
        public override int RetrieveAll()
        {
            return this.RetrieveAllFromDBSource();
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GETree> ToJavaList()
        {
            return (System.Collections.Generic.IList<GETree>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GETree> Tolist()
        {
            System.Collections.Generic.List<GETree> list = new System.Collections.Generic.List<GETree>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GETree)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
