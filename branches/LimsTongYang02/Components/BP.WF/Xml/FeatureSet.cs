using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.XML
{
	/// <summary>
	/// 个性化设置
	/// </summary>
	public class FeatureSet:XmlEnNoName
	{
		#region 属性
        public new string Name
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }
		#endregion

		#region 构造
		/// <summary>
		/// 节点扩展信息
		/// </summary>
		public FeatureSet()
		{
		}
        public FeatureSet(string no)
        {
            
        }
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new FeatureSets();
			}
		}
		#endregion
	}
	/// <summary>
    /// 个性化设置s
	/// </summary>
	public class FeatureSets:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
        public FeatureSets() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new FeatureSet();
			}
		}
		public override string File
		{
			get
			{
                return SystemConfig.PathOfXML + "FeatureSet.xml";
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
