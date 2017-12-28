using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;

namespace BP.Sys
{
    public class ExpFucnXmlList
    {
        /// <summary>
        /// 活动菜单
        /// </summary>
        public const string ActiveDDL = "ActiveDDL";
        /// <summary>
        /// 输入验证
        /// </summary>
        public const string InputCheck = "InputCheck";
        /// <summary>
        /// 自动完成
        /// </summary>
        public const string AutoFull = "AutoFull";
        /// <summary>
        /// Pop返回值
        /// </summary>
        public const string PopVal = "PopVal";
        /// <summary>
        /// Func
        /// </summary>
        public const string Func = "Func";
    }
	public class ExpFucnXml: BP.Sys.XML.XmlEnNoName
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
		public ExpFucnXml()
		{
		}
        public ExpFucnXml(string no)
        {
            
        }
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new ExpFucnXmls();
			}
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	public class ExpFucnXmls:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
        public ExpFucnXmls() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new ExpFucnXml();
			}
		}
        public override string File
        {
            get
            {
                return SystemConfig.PathOfXML + "MapExt.xml";
            }
        }
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
                return "ExpFunc";
			}
		}
		public override Entities RefEns
		{
			get
			{
				return null; //new BP.ZF1.AdminTools();
			}
		}
		#endregion
		 
	}
}
