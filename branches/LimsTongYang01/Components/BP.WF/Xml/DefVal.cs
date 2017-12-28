using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.XML
{
    /// <summary>
    /// 默认值
    /// </summary>
	public class DefVal:XmlEnNoName
	{
		#region 属性
        public string Val
        {
            get
            {
                return this.GetValStringByKey("Val");
            }
        }
		#endregion

		#region 构造
		/// <summary>
		/// 节点扩展信息
		/// </summary>
		public DefVal()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new DefVals();
			}
		}
		#endregion
	}
	/// <summary>
    /// 默认值s
	/// </summary>
	public class DefVals:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
        public DefVals() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new DefVal();
			}
		}
        public override string File
        {
            get
            {
                return SystemConfig.PathOfData + "\\Xml\\DefVal.xml";
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
