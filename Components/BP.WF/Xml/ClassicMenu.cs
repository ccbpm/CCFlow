using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.XML
{
    /// <summary>
    /// 经典模式左侧菜单
    /// </summary>
    public class ClassicMenu : XmlEn
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
                switch (this.No)
                {
                    case "EmpWorks":
                        return this.GetValStringByKey(BP.Web.WebUser.SysLang)+"("+BP.WF.Dev2Interface.Todolist_EmpWorks+")";
                    case "Sharing":
                        return this.GetValStringByKey(BP.Web.WebUser.SysLang) + "(" + BP.WF.Dev2Interface.Todolist_Sharing + ")";
                    case "CC":
                        return this.GetValStringByKey(BP.Web.WebUser.SysLang) + "(" + BP.WF.Dev2Interface.Todolist_CCWorks + ")";
                    default:
                        return this.GetValStringByKey(BP.Web.WebUser.SysLang);
                }
            }
        }
        /// <summary>
        /// 图片
        /// </summary>
        public string Img
        {
            get
            {
                return this.GetValStringByKey("Img");
            }
        }
        public string Title
        {
            get
            {
                return this.GetValStringByKey("Title");
            }
        }
        public string Url
        {
            get
            {
                return this.GetValStringByKey("Url");
            }
        }
        public bool Enable
        {
            get
            {
                return this.GetValBoolByKey("Enable");
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 节点扩展信息
        /// </summary>
        public ClassicMenu()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new ClassicMenus();
            }
        }
        #endregion
    }
    /// <summary>
    /// 经典模式左侧菜单s
    /// </summary>
    public class ClassicMenus : XmlEns
    {
        #region 构造
        /// <summary>
        /// 考核率的数据元素
        /// </summary>
        public ClassicMenus() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new ClassicMenu();
            }
        }
        public override string File
        {
            get
            {
                return SystemConfig.PathOfDataUser + "XML\\Menu.xml";
            }
        }
        /// <summary>
        /// 物理表名
        /// </summary>
        public override string TableName
        {
            get
            {
                return "ClassicMenu";
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
