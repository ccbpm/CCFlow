using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.XML
{
    /// <summary>
    /// 消息类型属性
    /// </summary>
    public class MsgTypeAttr
    {
        /// <summary>
        /// 编号
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
    }
    /// <summary>
    /// 消息类型
    /// </summary>
	public class MsgType:XmlEnNoName
	{
		#region 构造
		/// <summary>
		/// 节点扩展信息
		/// </summary>
		public MsgType()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new MsgTypes();
			}
		}
		#endregion
	}
	/// <summary>
	/// 消息类型s
	/// </summary>
	public class MsgTypes:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
        public MsgTypes() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new MsgType();
			}
		}
        /// <summary>
        /// XML文件位置.
        /// </summary>
		public override string File
		{
			get
			{
                return SystemConfig.CCFlowAppPath + "WF\\Data\\Xml\\SysDataType.xml";
			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
				return "MsgType";
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
