using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.XML
{
    /// <summary>
    /// 事件
    /// </summary>
    public class EventList : XmlEn
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
        /// 扩展名称
        /// </summary>
        public string NameHtml
        {
            get
            {
                if (this.IsHaveMsg)
                    return "<img src='../Img/Message24.png' border=0 width='17px'/>" + this.GetValStringByKey(BP.Web.WebUser.SysLang);
                else
                    return  this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }
        /// <summary>
        /// 输入描述
        /// </summary>
        public string EventDesc
        {
            get
            {
                return this.GetValStringByKey("EventDesc");
            }
        }
        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType
        {
            get
            {
                return this.GetValStringByKey("EventType");
            }
        }
        /// <summary>
        /// 是否有消息
        /// </summary>
        public bool IsHaveMsg
        {
            get
            {
                return this.GetValBoolByKey("IsHaveMsg");
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 事件
        /// </summary>
        public EventList()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new EventLists();
            }
        }
        #endregion
    }
    /// <summary>
    /// 事件s
    /// </summary>
    public class EventLists : XmlEns
    {
        #region 构造
        /// <summary>
        /// 事件s
        /// </summary>
        public EventLists() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new EventList();
            }
        }
        /// <summary>
        /// 存放路径
        /// </summary>
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
