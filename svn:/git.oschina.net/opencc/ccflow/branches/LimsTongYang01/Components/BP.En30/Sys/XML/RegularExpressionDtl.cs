using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.Sys.XML;


namespace BP.Sys.XML
{
    /// <summary>
    ///  RegularExpressionDtl 正则表达模版
    /// </summary>
	public class RegularExpressionDtl : XmlEn
	{
		#region 属性
        /// <summary>
        /// 编号
        /// </summary>
        public string ItemNo
        {
            get
            {
                return this.GetValStringByKey("ItemNo");
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey("Name");
            }
        }
        public string Note
        {
            get
            {
                return this.GetValStringByKey("Note");
            }
        }
        public string Exp
        {
            get
            {
                return this.GetValStringByKey("Exp");
            }
        }
        public string ForEvent
        {
            get
            {
                return this.GetValStringByKey("ForEvent");
            }
        }
        public string Msg
        {
            get
            {
                return this.GetValStringByKey("Msg");
            }
        }
		#endregion

		#region 构造
		/// <summary>
		/// 节点扩展信息
		/// </summary>
        public RegularExpressionDtl()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new RegularExpressionDtls();
			}
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	public class RegularExpressionDtls:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
        public RegularExpressionDtls() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new RegularExpressionDtl();
			}
		}
		public override string File
		{
			get
			{
                return SystemConfig.PathOfData + "\\XML\\RegularExpression.xml";
			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
				return "Dtl";
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
