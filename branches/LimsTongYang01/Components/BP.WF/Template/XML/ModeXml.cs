using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Sys.XML;

namespace BP.WF.Template.XML
{
    /// <summary>
    /// 模式
    /// </summary>
	public class ModeXml:XmlEnNoName
    {
        #region 属性
        /// <summary>
        /// 设置描述
        /// </summary>
        public string SetDesc
        {
            get
            {
                return this.GetValStringByKey("SetDesc");
            }
        }
        /// <summary>
        /// 类别
        /// </summary>
        public string FK_ModeSort
        {
            get
            {
                return this.GetValStringByKey("FK_ModeSort");
            }
        }
        /// <summary>
        /// 类别
        /// </summary>
        public string Note
        {
            get
            {
                return this.GetValStringByKey("Note");
            }
        }
        public string ParaType
        {
            get
            {
                return this.GetValStringByKey("ParaType");
            }
        }
        #endregion
      
		#region 构造
		/// <summary>
		/// 模式
		/// </summary>
        public ModeXml()
		{
		}
        /// <summary>
        /// 模式
        /// </summary>
        public ModeXml(string no):base(no)
        {
        }
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
                return new ModeXmls();
			}
		}
		#endregion
	}
	/// <summary>
    /// 模式s
	/// </summary>
	public class ModeXmls:XmlEns
	{
		#region 构造
		/// <summary>
        /// 皮肤s
		/// </summary>
        public ModeXmls() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new ModeXml();
			}
		}
        /// <summary>
        /// 文件路径
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
                return "Model";
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
