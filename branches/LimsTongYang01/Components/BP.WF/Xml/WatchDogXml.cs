using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.XML
{
    /// <summary>
    /// 流程监控菜单
    /// </summary>
    public class WatchDogXml : XmlEnNoName
    {
        public new string Name
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }

        #region 构造
        /// <summary>
        /// 流程监控菜单
        /// </summary>
        public WatchDogXml()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new WatchDogXmls();
            }
        }
        #endregion
    }
    /// <summary>
    /// 流程监控菜单s
    /// </summary>
    public class WatchDogXmls : XmlEns
    {
        #region 构造
        /// <summary>
        /// 流程监控菜单s
        /// </summary>
        public WatchDogXmls() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new WatchDogXml();
            }
        }
        public override string File
        {
            get
            {
                return SystemConfig.PathOfWebApp + "\\WF\\Admin\\Sys\\Sys.xml";
            }
        }
        /// <summary>
        /// 表
        /// </summary>
        public override string TableName
        {
            get
            {
                return "WatchDog";
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
