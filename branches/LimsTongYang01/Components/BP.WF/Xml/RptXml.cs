using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.XML
{
    /// <summary>
    /// 流程一户式
    /// </summary>
    public class RptXml : XmlEnNoName
    {
        public new string Name
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }
        public new string URL
        {
            get
            {
                return this.GetValStringByKey("URL");
            }
        }
        public new string ICON
        {
            get
            {
                return this.GetValStringByKey("ICON");
            }
        }

        #region 构造
        /// <summary>
        /// 节点扩展信息
        /// </summary>
        public RptXml()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new RptXmls();
            }
        }
        #endregion
    }
    /// <summary>
    /// 流程一户式s
    /// </summary>
    public class RptXmls : XmlEns
    {
        #region 构造
        /// <summary>
        /// 流程一户式s
        /// </summary>
        public RptXmls() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new RptXml();
            }
        }
        public override string File
        {
            get
            {
                return SystemConfig.PathOfData + "\\Xml\\WFAdmin.xml";
            }
        }
        /// <summary>
        /// 物理表名
        /// </summary>
        public override string TableName
        {
            get
            {
                return "RptFlow";
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
