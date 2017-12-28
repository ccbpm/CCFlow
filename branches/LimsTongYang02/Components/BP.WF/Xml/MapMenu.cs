using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Sys.XML;

namespace BP.WF.XML
{
    /// <summary>
    /// 映射菜单
    /// </summary>
    public class MapMenu : XmlEn
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
        public string JS
        {
            get
            {
                return this.GetValStringByKey("JS");
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
        /// <summary>
        /// 说明
        /// </summary>
        public string Note
        {
            get
            {
                return this.GetValStringByKey("Note");
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 节点扩展信息
        /// </summary>
        public MapMenu()
        {
        }
        /// <summary>
        /// 获取一个实例s
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new MapMenus();
            }
        }
        #endregion
    }
    /// <summary>
    /// 映射菜单s
    /// </summary>
    public class MapMenus : XmlEns
    {
        #region 构造
        /// <summary>
        /// 映射菜单s
        /// </summary>
        public MapMenus() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new MapMenu();
            }
        }
        public override string File
        {
            get
            {
               // return SystemConfig.PathOfWebApp + "\\WF\\MapDef\\Style\\XmlDB.xml";
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
                return "MapMenu";
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
