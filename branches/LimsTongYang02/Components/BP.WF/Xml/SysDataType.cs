using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.XML
{
    /// <summary>
    /// 数据类型
    /// </summary>
	public class SysDataType:XmlEn
	{
		#region 属性
        public string No
        {
            get
            {
                return this.GetValStringByKey("No");
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }
		/// <summary>
		/// 图片
		/// </summary>
		public string Desc
		{
			get
			{
                return this.GetValStringByKey("Desc");
			}
		}
		#endregion

		#region 构造
		/// <summary>
		/// 节点扩展信息
		/// </summary>
		public SysDataType()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new SysDataTypes();
			}
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	public class SysDataTypes:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
        public SysDataTypes() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new SysDataType();
			}
		}
        public override string File
        {
            get
            {
              //  return SystemConfig.PathOfWebApp + "\\WF\\MapDef\\Style\\SysDataType.xml";
                return SystemConfig.PathOfData + "\\XML\\SysDataType.xml";
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
				return null; //new BP.ZF1.AdminTools();
			}
		}
		#endregion
		 
	}
}
