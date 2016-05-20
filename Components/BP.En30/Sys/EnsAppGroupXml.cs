using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys.XML;


namespace BP.Sys.XML
{
    /// <summary>
    /// 属性
    /// </summary>
    public class EnsAppGroupXmlEnsName
    {
        /// <summary>
        /// 过错行为
        /// </summary>
        public const string EnsName = "EnsName";
        /// <summary>
        /// 表达式
        /// </summary>
        public const string GroupKey = "GroupKey";
        /// <summary>
        /// 数据类型
        /// </summary>
        public const string GroupName = "GroupName";
    }
    /// <summary>
    /// EnsAppGroupXml 的摘要说明，属性的配置。
    /// </summary>
    public class EnsAppGroupXml : XmlEnNoName
    {
        #region 属性
        /// <summary>
        /// 类名
        /// </summary>
        public string EnsName
        {
            get
            {
                return this.GetValStringByKey(EnsAppGroupXmlEnsName.EnsName);
            }
        }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string GroupName
        {
            get
            {
                return this.GetValStringByKey(EnsAppGroupXmlEnsName.GroupName);
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string GroupKey
        {
            get
            {
                return this.GetValStringByKey(EnsAppGroupXmlEnsName.GroupKey);
            }
        }
        #endregion

        #region 构造
        public EnsAppGroupXml()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new EnsAppGroupXmls();
            }
        }
        #endregion
    }
    /// <summary>
    /// 属性集合
    /// </summary>
    public class EnsAppGroupXmls : XmlEns
    {
        #region 构造
        /// <summary>
        /// 考核过错行为的数据元素
        /// </summary>
        public EnsAppGroupXmls()
        {
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
                return new EnsAppGroupXml();
            }
        }
        public override string File
        {
            get
            {
                return SystemConfig.PathOfXML + "\\Ens\\EnsAppXml\\";
            }
        }
        /// <summary>
        /// 物理表名
        /// </summary>
        public override string TableName
        {
            get
            {
                return "Group";
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
