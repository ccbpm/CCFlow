using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Sys.XML;

namespace BP.WF.XML
{
    /// <summary>
    /// 事件列表
    /// </summary>
    public class EventListDtlList
    {
        /// <summary>
        /// 保存前
        /// </summary>
        public const string DtlSaveBefore = "DtlSaveBefore";
        /// <summary>
        /// 保存后
        /// </summary>
        public const string DtlSaveEnd = "DtlSaveEnd";
        /// <summary>
        /// 记录保存前
        /// </summary>
        public const string DtlItemSaveBefore = "DtlItemSaveBefore";
        /// <summary>
        /// 记录保存后
        /// </summary>
        public const string DtlItemSaveAfter = "DtlItemSaveAfter";
        /// <summary>
        /// 记录删除前
        /// </summary>
        public const string DtlItemDelBefore = "DtlItemDelBefore";
        /// <summary>
        /// 记录删除后
        /// </summary>
        public const string DtlItemDelAfter = "DtlItemDelAfter";
    }
    /// <summary>
    /// 从表事件
    /// </summary>
    public class EventListDtl : XmlEn
    {
        #region 属性
        /// <summary>
        /// 编号
        /// </summary>
        public string No
        {
            get
            {
                return this.GetValStringByKey("No");
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string EventDesc
        {
            get
            {
                return this.GetValStringByKey("EventDesc");
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 从表事件
        /// </summary>
        public EventListDtl()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new EventListDtls();
            }
        }
        #endregion
    }
    /// <summary>
    /// 从表事件s
    /// </summary>
    public class EventListDtls : XmlEns
    {
        #region 构造
        /// <summary>
        /// 从表事件s
        /// </summary>
        public EventListDtls() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new EventListDtl();
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
                return "ItemDtl";
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
