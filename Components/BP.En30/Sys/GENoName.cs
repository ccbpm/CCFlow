using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.En
{
	/// <summary>
	/// 属性
	/// </summary>
	public class GENoNameAttr : EntityNoNameAttr
	{

    }
	/// <summary>
	/// 
	/// </summary>
    public class GENoName : EntityNoName
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
        public GENoName()
        {

        }
        /// <summary>
        /// 编号
        /// </summary>
        /// <param name="no">编号</param>
        public GENoName(string no)  :base(no)
        {

        }
        public GENoName(string sftable, string tableDesc)
        {
            this.PhysicsTable = sftable;
            this.Desc = tableDesc;
        }
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null) 
                    return this._enMap;

                Map map = new Map(this.PhysicsTable,this.Desc);
                map.IsAutoGenerNo = true;

                map.Java_SetDepositaryOfEntity( Depositary.Application);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.App);
                map.IsAutoGenerNo = true;

                map.AddTBStringPK(GENoNameAttr.No, null, "编号", true, true, 1, 30, 3);
                map.AddTBString(GENoNameAttr.Name, null, "名称", true, false, 1, 60, 500);
                //return map;
                this._enMap = map;
                return this._enMap;
            }
        }
        public string PhysicsTable = null;
        public string Desc = null;

        #endregion
    }
	/// <summary>
	/// GENoNames
	/// </summary>
	public class GENoNames : EntitiesNoName
	{
        /// <summary>
        /// 物理表
        /// </summary>
        public string SFTable = null;
        public string Desc = null;

		/// <summary>
		/// GENoNames
		/// </summary>
        public GENoNames()
		{
		}
        public GENoNames(string sftable, string tableDesc)
        {
            this.SFTable = sftable;
            this.Desc = tableDesc;
        }
        public override Entity GetNewEntity
        {
            get 
            {
                return new GENoName(this.SFTable, this.Desc);
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
        public System.Collections.Generic.IList<GENoName> ToJavaList()
        {
            return (System.Collections.Generic.IList<GENoName>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GENoName> Tolist()
        {
            System.Collections.Generic.List<GENoName> list = new System.Collections.Generic.List<GENoName>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GENoName)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
