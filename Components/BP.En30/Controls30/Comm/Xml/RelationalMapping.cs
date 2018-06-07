using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;


namespace BP.Web.Comm
{
    public class RelationalMappingAttr
    {
        /// <summary>
        /// 编号
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        public const string HelpFile = "HelpFile";
        public const string Img = "Img";
    }
	/// <summary>
	/// 
	/// </summary>
	public class RelationalMapping:XmlEn
	{

		#region 构造
		public RelationalMapping()
		{
		}
		/// <summary>
		/// 编号
		/// </summary>
		/// <param name="no"></param>
		public RelationalMapping(string no)
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new RelationalMappings();
            }
        }
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	public class RelationalMappings:XmlEns
	{
		#region 构造
		/// <summary>
		/// 关系映射
		/// </summary>
        public RelationalMappings() 
        {
        }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new RelationalMapping();
			}
		}
		public override string File
		{
			get
			{
                return SystemConfig.PathOfWebApp + "\\Helper\\RelationalMapping.xml";
			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
				return "RelationalMapping";
			}
		}
		public override Entities RefEns
		{
			get
			{
				return null; //new BP.ZF1.Helps();
			}
		}
		#endregion
		 
	}
}
