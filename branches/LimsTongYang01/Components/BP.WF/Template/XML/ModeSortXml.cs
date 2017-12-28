using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;
using BP.Sys;

namespace BP.WF.Template.XML
{
    /// <summary>
    /// 模式
    /// </summary>
	public class ModeSortXml:XmlEnNoName
	{
		#region 构造
		/// <summary>
		/// 节点扩展信息
		/// </summary>
        public ModeSortXml()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
                return new ModeSortXmls();
			}
		}
		#endregion
	}
	/// <summary>
    /// 模式s
	/// </summary>
	public class ModeSortXmls:XmlEns
	{
		#region 构造
		/// <summary>
        /// 模式s
		/// </summary>
        public ModeSortXmls() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new ModeSortXml();
			}
		}
        /// <summary>
        /// 文件位置
        /// </summary>
        public override string File
        {
            get
            {
                return SystemConfig.PathOfWebApp + "\\WF\\Admin\\AccepterRole\\AccepterRole.xml";

            }
        }
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
                return "ModelSort";
			}
		}
        /// <summary>
        /// 关联的实体
        /// </summary>
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
