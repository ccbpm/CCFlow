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
    public class EnsAppXmlEnsName
    {
        /// <summary>
        /// 过错行为
        /// </summary>
        public const string EnsName = "EnsName";
        /// <summary>
        /// 表达式
        /// </summary>
        public const string Desc = "Desc";
        /// <summary>
        /// 数据类型
        /// </summary>
        public const string DBType = "DBType";
        /// <summary>
        /// 默认值
        /// </summary>
        public const string DefVal = "DefVal";
        /// <summary>
        /// 值
        /// </summary>
        public const string EnumKey = "EnumKey";
        /// <summary>
        /// 值
        /// </summary>
        public const string EnumVals = "EnumVals";
    }
    /// <summary>
    /// EnsAppXml 的摘要说明，属性的配置。
    /// </summary>
    public class EnsAppXml : XmlEnNoName
    {
        #region 属性
        /// <summary>
        /// 枚举值
        /// </summary>
        public string EnumKey
        {
            get
            {
                return this.GetValStringByKey(EnsAppXmlEnsName.EnumKey);
            }
        }
        public string EnumVals
        {
            get
            {
                return this.GetValStringByKey(EnsAppXmlEnsName.EnumVals);
            }
        }
        /// <summary>
        /// 类名
        /// </summary>
        public string EnsName
        {
            get
            {
                return this.GetValStringByKey(EnsAppXmlEnsName.EnsName);
            }
        }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DBType
        {
            get
            {
                return this.GetValStringByKey(EnsAppXmlEnsName.DBType);
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc
        {
            get
            {
                return this.GetValStringByKey(EnsAppXmlEnsName.Desc);
            }
        }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefVal
        {
            get
            {
                return this.GetValStringByKey(EnsAppXmlEnsName.DefVal);
            }
        }
        public bool DefValBoolen
        {
            get
            {
                return this.GetValBoolByKey(EnsAppXmlEnsName.DefVal);
            }
        }
        public int DefValInt
        {
            get
            {
                return this.GetValIntByKey(EnsAppXmlEnsName.DefVal);
            }
        }
        #endregion

        #region 构造
        public EnsAppXml()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new EnsAppXmls();
            }
        }
        #endregion
    }
    /// <summary>
    /// 属性集合
    /// </summary>
    public class EnsAppXmls : XmlEns
    {
        #region 构造
        /// <summary>
        /// 考核过错行为的数据元素
        /// </summary>
        public EnsAppXmls()
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
                return new EnsAppXml();
            }
        }
        public override string File
        {
            get
            {
                return SystemConfig.PathOfXML + "\\Ens\\EnsAppXml\\GE.xml";
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
