using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Sys.XML;

namespace BP.GE
{
	/// <summary>
    /// 主菜单属性
	/// </summary>
    public class MainMenuXmlAttr
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
	/// 主菜单
	/// </summary>
    public class MainMenuXml : XmlMenu
    {
        #region 属性
        
        #endregion

        #region 构造
        /// <summary>
        /// 主菜单
        /// </summary>
        public MainMenuXml()
        {
        }
        /// <summary>
        /// 主菜单
        /// </summary>
        /// <param name="no">编号</param>
        public MainMenuXml(string no)
        {
            this.RetrieveByPK(MainMenuXmlAttr.No, no);
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new MainMenuXmls();
            }
        }
        #endregion
    }
	/// <summary>
    /// 主菜单s
	/// </summary>
    public class MainMenuXmls : XmlMenus
    {
        #region 构造
        /// <summary>
        /// 主菜单s
        /// </summary>
        public MainMenuXmls() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new MainMenuXml();
            }
        }
        public override string File
        {
            get
            {
               return SystemConfig.PathOfXML + "\\MainMenu.xml";
               // return SystemConfig.PathOfXML + "\\Language\\";
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
