using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.BPMN
{
    /// <summary>
    /// 元素
    /// </summary>
    public class EleListXml : XmlEn
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
        /// <summary>
        /// 描述
        /// </summary>
        public string EventDesc
        {
            get
            {
                return this.GetValStringByKey("EleDesc");
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string EleType
        {
            get
            {
                return this.GetValStringByKey("EleType");
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 事件
        /// </summary>
        public EleListXml()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new EleListXmls();
            }
        }
        #endregion
    }
    /// <summary>
    /// 元素s
    /// </summary>
    public class EleListXmls : XmlEns
    {
        #region 构造
        /// <summary>
        /// 事件s
        /// </summary>
        public EleListXmls() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new EleListXml();
            }
        }
        /// <summary>
        /// 存放路径
        /// </summary>
        public override string File
        {
            get
            {
                return SystemConfig.PathOfWebApp + "\\WF\\Admin\\CCFlowDesigner\\Designer.xml";
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
