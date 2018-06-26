using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.XML
{
    /// <summary>
    /// 管理员
    /// </summary>
    public class AdminMenuGroup : XmlEn
    {
        #region 属性
        public string No
        {
            get
            {
                return this.GetValStringByKey("No");
            }
            set
            {
                this.SetVal("No", value);
            }
        }
        public string ParentNo
        {
            get
            {
                return this.GetValStringByKey("ParentNo");
            }
            set
            {
                this.SetVal("ParentNo", value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey("Name");
            }
            set
            {
                this.SetVal("Name", value);
            }
        }
        public string For
        {
            get
            {
                return this.GetValStringByKey("For");
            }
            set
            {
                this.SetVal("For", value);
            }
        }
        
        #endregion

        #region 构造
        /// <summary>
        /// 节点扩展信息
        /// </summary>
        public AdminMenuGroup()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new AdminMenuGroups();
            }
        }
        #endregion

        /// <summary>
        /// 是否可以使用？
        /// </summary>
        /// <param name="no">操作员编号</param>
        /// <returns></returns>
        public bool IsCanUse(string no)
        {
            if (string.IsNullOrEmpty(this.For))
                return true;

            if (this.For == no)
                return true;

            if (this.For == "SecondAdmin")
                return true;

            return false;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class AdminMenuGroups : XmlEns
    {
        #region 构造
        /// <summary>
        /// 考核率的数据元素
        /// </summary>
        public AdminMenuGroups() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new AdminMenuGroup();
            }
        }
        public override string File
        {
            get
            {
                return SystemConfig.PathOfWebApp + "\\DataUser\\XML\\AdminMenu.xml";
            }
        }
        /// <summary>
        /// 物理表名
        /// </summary>
        public override string TableName
        {
            get
            {
                return "Group";
            }
        }
        public override Entities RefEns
        {
            get
            {
                return null; //new BP.ZF1.AdminAdminMenus();
            }
        }
        #endregion

    }
}
