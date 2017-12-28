using System;
using System.IO;
using System.Collections;
using BP.DA;
using System.Data;
using BP.Sys;
using BP.En;

namespace BP.Sys.XML
{
    public class XmlMenuAttr
    {
        public const string No = "No";
        public const string Name = "Name";
        public const string Url = "Url";
        public const string Target = "Target";
        public const string Img = "Img";
    }
    abstract public class XmlMenu : XmlEnNoName
    {
        /// <summary>
        /// 功能编号
        /// </summary>
        public string Img
        {
            get
            {
                return this.GetValStringByKey("Img");
            }
        }
        /// <summary>
        /// 名称.
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey("Name");
            }
        }
        /// <summary>
        /// URL
        /// </summary>
        public string Url
        {
            get
            {
                return this.GetValStringByKey("Url");
            }
        }
        public string Target
        {
            get
            {
                return this.GetValStringByKey("Target");
            }
        }
        /// <summary>
        /// 菜单
        /// </summary>
        public XmlMenu()
        {
        }
        /// <summary>
        /// 菜单
        /// </summary>
        /// <param name="no"></param>
        public XmlMenu(string no)
        {
            this.RetrieveByPK("No", no);
        }
    }
    /// <summary>
    /// XmlMenus 的摘要说明。
    /// </summary>
    abstract public class XmlMenus : XmlEns
    {
        #region 构造
        /// <summary>
        /// 构造
        /// </summary>
        public XmlMenus()
        {
        }
        #endregion 构造
    }
}