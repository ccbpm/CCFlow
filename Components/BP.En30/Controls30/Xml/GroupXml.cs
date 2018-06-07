using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.Sys.XML;

namespace BP.Web
{
	/// <summary>
    /// 分组菜单属性
	/// </summary>
    public class GroupXmlAttr
    {
        /// <summary>
        /// 编号  
        /// </summary>    
        public const string No = "No";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
    }
	/// <summary>
	/// 分组菜单
	/// </summary>
    public class GroupXml : XmlEnNoName
    {
        #region 属性
        #endregion

        #region 构造
        /// <summary>
        /// 分组菜单
        /// </summary>
        public GroupXml()
        {

        }
        /// <summary>
        /// 分组菜单
        /// </summary>
        /// <param name="no">编号</param>
        public GroupXml(string no)
        {
            this.RetrieveByPK(GroupXmlAttr.No, no);
        }
        
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new GroupXmls();
            }
        }
        #endregion
    }
	/// <summary>
    /// 分组菜单s
	/// </summary>
    public class GroupXmls : XmlMenus
    {
        #region 构造
        /// <summary>
        /// 分组菜单s
        /// </summary>
        public GroupXmls()
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
                return new GroupXml();
            }
        }
        public override string File
        {
            get
            {
                return SystemConfig.PathOfXML + "\\Ens\\Group.xml";
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
