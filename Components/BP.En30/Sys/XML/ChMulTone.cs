using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;


namespace BP.Sys
{
    /// <summary>
    /// 多音字
    /// </summary>
    public class ChMulToneXml : XmlEn
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
                return this.GetValStringByKey("Name");
            }
        }
        public string Desc
        {
            get
            {
                return this.GetValStringByKey("No");
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 节点扩展信息
        /// </summary>
        public ChMulToneXml()
        {
        }
        /// <summary>
        /// 获取一个实例s
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new ChMulToneXmls();
            }
        }
        #endregion
    }
    /// <summary>
    /// 多音字s
    /// </summary>
    public class ChMulToneXmls : XmlEns
    {
        #region 构造
        /// <summary>
        /// 多音字s
        /// </summary>
        public ChMulToneXmls() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new ChMulToneXml();
            }
        }
        public override string File
        {
            get
            {
                return SystemConfig.PathOfData + "\\XML\\XmlDB.xml";
            }
        }
        /// <summary>
        /// 物理表名
        /// </summary>
        public override string TableName
        {
            get
            {
                return "PinYin";
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
