using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;


namespace BP.Sys
{
    /// <summary>
    /// 分组内容
    /// </summary>
    public class FieldGroupXml : XmlEn
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
        public string Desc
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang+"Desc");
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 节点扩展信息
        /// </summary>
        public FieldGroupXml()
        {
        }
        /// <summary>
        /// 获取一个实例s
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new FieldGroupXmls();
            }
        }
        #endregion
    }
    /// <summary>
    /// 分组内容s
    /// </summary>
    public class FieldGroupXmls : XmlEns
    {
        #region 构造
        /// <summary>
        /// 分组内容s
        /// </summary>
        public FieldGroupXmls() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new FieldGroupXml();
            }
        }
        public override string File
        {
            get
            {
               // return SystemConfig.PathOfWebApp + "\\WF\\MapDef\\Style\\XmlDB.xml";
                                return SystemConfig.PathOfData + "\\XML\\XmlDB.xml";
                //\MapDef\\Style\
            }
        }
        /// <summary>
        /// 物理表名
        /// </summary>
        public override string TableName
        {
            get
            {
                return "FieldGroup";
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
