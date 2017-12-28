using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys.XML
{
    /// <summary>
    /// 文本框事件属性
    /// </summary>
    public class TBEventXmlList
    {
        /// <summary>
        /// 功能
        /// </summary>
        public const string Func = "Func";
        /// <summary>
        /// 事件名称
        /// </summary>
        public const string EventName = "EventName";
        /// <summary>
        /// 为
        /// </summary>
        public const string DFor = "DFor";
    }
    /// <summary>
    /// 文本框事件
    /// </summary>
	public class TBEventXml:XmlEn
	{
		#region 属性
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName
        {
            get
            {
                return this.GetValStringByKey(TBEventXmlList.EventName);
            }
        }
        /// <summary>
        /// 功能
        /// </summary>
        public string Func
        {
            get
            {
                return this.GetValStringByKey(TBEventXmlList.Func);
            }
        }
        /// <summary>
        /// 数据为
        /// </summary>
        public string DFor
        {
            get
            {
                return this.GetValStringByKey(TBEventXmlList.DFor);
            }
        }
		#endregion

		#region 构造
		/// <summary>
		/// 文本框事件
		/// </summary>
		public TBEventXml()
		{
		}
        public TBEventXml(string no)
        {
        }
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new TBEventXmls();
			}
		}
		#endregion
	}
	/// <summary>
    /// 文本框事件s
	/// </summary>
	public class TBEventXmls:XmlEns
	{
		#region 构造
		/// <summary>
        /// 文本框事件s
		/// </summary>
        public TBEventXmls() { }
        /// <summary>
        /// 文本框事件s
        /// </summary>
        /// <param name="dFor"></param>
        public TBEventXmls(string dFor)
        {
            this.Retrieve(TBEventXmlList.DFor, dFor);
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
				return new TBEventXml();
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
                return "TBEvent";
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
