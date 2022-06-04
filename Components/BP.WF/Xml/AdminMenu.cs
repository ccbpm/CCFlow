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
    public class AdminMenu : XmlEn
    {
        #region 属性
        /// <summary>
        /// 菜单编号
        /// </summary>
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
        /// <summary>
        /// 分组编号
        /// </summary>
        public string GroupNo
        {
            get
            {
                return this.GetValStringByKey("GroupNo");
            }
            set
            {
                this.SetVal("GroupNo", value);
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
            set
            {
                this.SetVal("Name", value);
            }
        }
        /// <summary>
        /// 应用范围
        /// </summary>
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
        /// <summary>
        /// Url菜单
        /// </summary>
        public string Url
        {
            get
            {
                return this.GetValStringByKey("Url");
            }
            set
            {
                this.SetVal("Url", value);
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 节点扩展信息
        /// </summary>
        public AdminMenu()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new AdminMenus();
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
            return true;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class AdminMenus : XmlEns
    {
        #region 构造
        /// <summary>
        /// 考核率的数据元素
        /// </summary>
        public AdminMenus() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new AdminMenu();
            }
        }
        public override string File
        {
            get
            {
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                    return BP.Difference.SystemConfig.PathOfWebApp + "DataUser/XML/AdminMenu.xml";

                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    if (BP.Web.WebUser.No.Equals("admin") == true)
                        return BP.Difference.SystemConfig.PathOfWebApp + "DataUser/XML/AdminMenuSAAS.xml";
                    else
                        return BP.Difference.SystemConfig.PathOfWebApp + "DataUser/XML/Admin2MenuSAAS.xml";
                }

                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.GroupInc)
                {
                    if (BP.Web.WebUser.No.Equals("admin") == true)
                        return BP.Difference.SystemConfig.PathOfWebApp + "DataUser/XML/AdminMenuGroup.xml";
                    else
                        return BP.Difference.SystemConfig.PathOfWebApp + "DataUser/XML/Admin2MenuGroup.xml";
                }
                throw new Exception("err@系统错误....");
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
                return null; //new BP.ZF1.AdminAdminMenus();
            }
        }
        #endregion

    }
}
