using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.Sys.XML;

namespace BP.WF.XML
{
    /// <summary>
    /// 事件源
    /// </summary>
	public class EventSource:XmlEn
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
		#endregion

		#region 构造
		/// <summary>
        /// 事件源
		/// </summary>
		public EventSource()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new EventSources();
			}
		}
		#endregion
	}
	/// <summary>
    /// 事件源s
	/// </summary>
    public class EventSources : XmlEns
    {
        #region 构造
        /// <summary>
        /// 事件源s
        /// </summary>
        public EventSources() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new EventSource();
            }
        }
        public override string File
        {
            get
            {
                return SystemConfig.PathOfXML + "\\EventList.xml";
            }
        }
        /// <summary>
        /// 物理表名
        /// </summary>
        public override string TableName
        {
            get
            {
                return "Source";
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
