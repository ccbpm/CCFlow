using System;
using System.Collections.Generic;
using System.Data;
using BP.DA;
using BP.Sys;

namespace BP.En
{
    /// <summary>
    /// 访问控制
    /// </summary>
    public class UAC
    {
        #region 常用方法
        
        public void Readonly()
        {
            this.IsUpdate = false;
            this.IsDelete = false;
            this.IsInsert = false;
            this.IsAdjunct = false;
            this.IsView = true;
        }
        /// <summary>
        /// 全部开放
        /// </summary>
        public void OpenAll()
        {
            this.IsUpdate = true;
            this.IsDelete = true;
            this.IsInsert = true;
            this.IsAdjunct = false;
            this.IsView = true;
        }
        /// <summary>
        /// 为一个角色设置全部的权限
        /// </summary>
        /// <param name="fk_station"></param>
        public void OpenAllForStation(string fk_station)
        {
            Paras ps = new Paras();
            ps.Add("FK_Emp",BP.Web.WebUser.No);
            ps.Add("st", fk_station);

            bool bl;

            bl = DBAccess.IsExits("SELECT FK_Emp FROM Port_DeptEmpStation WHERE FK_Emp=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Emp AND FK_Station=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "st", ps);

            if (bl)
                this.OpenAll();
            else
                this.Readonly();
        }
        /// <summary>
        /// 仅仅对管理员
        /// </summary>
        public UAC OpenForSysAdmin()
        {
           // if (BP.Web.WebUser.No.Equals("admin") == true)
            if (BP.Web.WebUser.IsAdmin)
                this.OpenAll();
             
            return this;
        }
        public UAC OpenForAppAdmin()
        {
            /*if (BP.Web.WebUser.No != null
                && BP.Web.WebUser.No.Contains("admin") == true)*/
            if (BP.Web.WebUser.IsAdmin)
            {
                this.OpenAll();
            }
            return this;
        }

        public UAC OpenForAdmin()
        {
            /*if (BP.Web.WebUser.No != null
               && BP.Web.WebUser.IsAdmin == true)*/
            if (BP.Web.WebUser.IsAdmin)
            {
                this.OpenAll();
            }
            else
            {
                this.Readonly();
            }
            return this;
        }
        #endregion

        #region 控制属性
        /// <summary>
        /// 是否插入
        /// </summary>
        public bool IsInsert = false;
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete = false;
        /// <summary>
        /// 是否更新
        /// </summary>
        public bool IsUpdate = false;
        /// <summary>
        /// 是否查看
        /// </summary>
        public bool IsView = true;
        /// <summary>
        /// 附件
        /// </summary>
        public bool IsAdjunct = false;
        /// <summary>
        /// 是否可以导出
        /// <para>注意：要启用导出权限控制，请使用uac.IsExp = UserRegedit.HaveRoleForExp(this.ToString());</para>
        /// </summary>
        public bool IsExp = false;
        /// <summary>
        /// 是否可以导入
        /// <para>注意：要启用导入权限控制，请使用uac.IsImp = UserRegedit.HaveRoleForImp(this.ToString());</para>
        /// </summary>
        public bool IsImp = false;
        #endregion

        #region 构造
        /// <summary>
        /// 用户访问
        /// </summary>
        public UAC()
        {

        }
        #endregion
    }
}
