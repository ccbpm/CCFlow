using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo
{
	/// <summary>
	/// 目录属性
	/// </summary>
    public class SortAttr:EntityTreeAttr
    {
        #region 基本属性
        /// <summary>
        /// 简称
        /// </summary>
        public const string Abbr = "Abbr";
        #endregion
    }
	/// <summary>
	/// 目录
	/// </summary>
    public class Sort : EntityTree
    {
        #region 属性

        public string SortAbbr
        {
            get
            {
                return this.GetValStringByKey(SortAttr.Abbr);
            }
            set
            {
                this.SetValByKey(SortAttr.Abbr, value);
            }
        }
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (Web.WebUser.No != "admin")
                {
                    uac.IsView = false;
                    return uac;
                }
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// Sort
        /// </summary>
        public Sort()
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
                Map map = new Map("Demo_Sort", "目录");
                map.Java_SetEnType(EnType.Admin);

                map.AddTBStringPK(SortAttr.No, null, "编号", true, true, 4, 4, 4);
                map.AddTBString(SortAttr.Name, null, "名称", true, true, 0, 200, 4);
                map.AddTBString(SortAttr.ParentNo, null, "父编号", true, true, 0, 4, 4);
                map.AddTBString(SortAttr.Abbr, null, "简称", true, true, 0, 60, 400);
                map.AddTBInt(SortAttr.Idx, 0, "序号", true, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// 目录s
	/// </summary>
	public class Sorts: EntitiesTree
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new Sort();
			}
		}
		/// <summary>
        /// 目录
		/// </summary>
		public Sorts(){} 		 
		#endregion
	}
}
