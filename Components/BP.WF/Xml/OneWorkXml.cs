using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.XML
{
    /// <summary>
    /// 工作一户式
    /// </summary>
    public class OneWorkXml : XmlEnNoName
    {
        #region 属性.
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
                if (BP.WF.Glo.Plant == Plant.CCFlow)
                    return this.GetValStringByKey("UrlCCFlow");
                return this.GetValStringByKey("UrlJFlow");
            }
        }

        public new string IsDefault
        {
            get
            {
                return this.GetValStringByKey("IsDefault");
            }
        }

 
        #endregion 属性.

        #region 构造
        /// <summary>
        /// 节点扩展信息
        /// </summary>
        public OneWorkXml()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new OneWorkXmls();
            }
        }
        #endregion
    }
    /// <summary>
    /// 工作一户式s
    /// </summary>
    public class OneWorkXmls : XmlEns
    {
        #region 构造
        /// <summary>
        /// 工作一户式s
        /// </summary>
        public OneWorkXmls() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new OneWorkXml();
            }
        }
        /// <summary>
        /// 文件
        /// </summary>
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
                return "OneWork";
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
