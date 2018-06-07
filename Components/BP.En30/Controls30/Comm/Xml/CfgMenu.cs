using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;


namespace BP.Web.Comm
{
    public class CfgMenuAttr
    {
        /// <summary>
        /// 编号
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        public const string HelpFile = "HelpFile";
        public const string Img = "Img";
    }
    /// <summary>
    /// 配置菜单
    /// </summary>
    public class CfgMenu : XmlMenu
    {
        #region 构造
        public CfgMenu()
        {
        }
        /// <summary>
        /// 编号
        /// </summary>
        /// <param name="no"></param>
        public CfgMenu(string no)
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new CfgMenus();
            }
        }
        #endregion
    }
    /// <summary>
    /// 配置菜单s
    /// </summary>
    public class CfgMenus : XmlMenus
    {
        #region 构造
        /// <summary>
        /// 配置菜单
        /// </summary>
        public CfgMenus()
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
                return new CfgMenu();
            }
        }
        public override string File
        {
            get
            {
                return SystemConfig.PathOfWebApp + "\\WF\\Comm\\Sys\\CfgMenu.xml";
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
                return null; //new BP.ZF1.Helps();
            }
        }
        #endregion
    }
}
