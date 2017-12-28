using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Sys.XML;

namespace BP.WF.XML
{
    /// <summary>
    /// 移动菜单
    /// </summary>
    public class Mobile : XmlEn
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
                return this.GetValStringByKey("Name");
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 节点扩展信息
        /// </summary>
        public Mobile()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new Mobiles();
            }
        }
        #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    public class Mobiles : XmlEns
    {
        #region 构造
        /// <summary>
        /// 考核率的数据元素
        /// </summary>
        public Mobiles() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new Mobile();
            }
        }
        public override string File
        {
            get
            {
                return SystemConfig.CCFlowAppPath + "DataUser\\XML\\Mobiles.xml";
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
