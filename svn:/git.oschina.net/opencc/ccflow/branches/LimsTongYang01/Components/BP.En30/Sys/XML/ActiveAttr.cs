using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys.XML;


namespace BP.Sys.XML
{
	/// <summary>
	/// 属性
	/// </summary>
    public class ActiveAttrAttr
    {
        /// <summary>
        /// AttrKey
        /// </summary>
        public const string AttrKey = "AttrKey";
        /// <summary>
        /// 表达式
        /// </summary>
        public const string AttrName = "AttrName";
        /// <summary>
        /// 描述
        /// </summary>
        public const string Exp = "Exp";
        /// <summary>
        /// ExpApp
        /// </summary>
        public const string ExpApp = "ExpApp";
        /// <summary>
        /// for
        /// </summary>
        public const string For = "For";
        /// <summary>
        /// 条件
        /// </summary>
        public const string Condition = "Condition";
    }
	/// <summary>
	/// ActiveAttr 的摘要说明。
	/// 考核过错行为的数据元素
	/// 1，它是 ActiveAttr 的一个明细。
	/// 2，它表示一个数据元素。
	/// </summary>
    public class ActiveAttr : XmlEn
    {
        #region 属性
        /// <summary>
        /// 选择这个属性时间需要的条件
        /// </summary>
        public string Condition
        {
            get
            {
                return this.GetValStringByKey(ActiveAttrAttr.Condition);
            }
        }
        public string AttrKey
        {
            get
            {
                return this.GetValStringByKey(ActiveAttrAttr.AttrKey);
            }
        }
        public string AttrName
        {
            get
            {
                return this.GetValStringByKey(ActiveAttrAttr.AttrName);
            }
        }
        public string Exp
        {
            get
            {
                return this.GetValStringByKey(ActiveAttrAttr.Exp);
            }
        }
        public string ExpApp
        {
            get
            {
                return this.GetValStringByKey(ActiveAttrAttr.ExpApp);
            }
        }
        public string For
        {
            get
            {
                return this.GetValStringByKey(ActiveAttrAttr.For);
            }
        }
        #endregion

        #region 构造
        public ActiveAttr()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new ActiveAttrs();
            }
        }
        #endregion
    }
	/// <summary>
	/// 
	/// </summary>
	public class ActiveAttrs:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核过错行为的数据元素
		/// </summary>
		public ActiveAttrs(){}
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new ActiveAttr();
			}
		}
		public override string File
		{
			get
			{
				return SystemConfig.PathOfXML+"\\Ens\\ActiveAttr.xml";
			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
				return "Item";
			}
		}
		public override Entities RefEns
		{
			get
			{
				return null;
			}
		}
		#endregion
		 
	}
}
