﻿using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port; 
using BP.Port; 
using BP.En;
using BP.Web;
using System.Drawing;
using System.Text;
using System.IO;

namespace BP.WF.Port
{
	/// <summary>
	/// 管理员
	/// </summary>
    public class AdminEmpAttr
    {
        #region 基本属性
        /// <summary>
        /// No
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 申请人
        /// </summary>
        public const string Name = "Name";
        public const string LoginData = "LoginData";
        public const string Tel = "Tel";
        /// <summary>
        /// 授权人
        /// </summary>
        public const string Author = "Author";
        /// <summary>
        /// 授权日期
        /// </summary>
        public const string AuthorDate = "AuthorDate";
        /// <summary>
        /// 是否处于授权状态
        /// </summary>
        public const string AuthorWay = "AuthorWay";
        /// <summary>
        /// 授权自动收回日期
        /// </summary>
        public const string AuthorToDate = "AuthorToDate";
        public const string Email = "Email";
        public const string AlertWay = "AlertWay";
        public const string Stas = "Stas";
        public const string Depts = "Depts";
        public const string FK_Dept = "FK_Dept";
        public const string Idx = "Idx";
        public const string FtpUrl = "FtpUrl";
        public const string Style = "Style";
        public const string Msg = "Msg";
        public const string TM = "TM";
        public const string UseSta = "UseSta";
        /// <summary>
        /// 授权的人员
        /// </summary>
        public const string AuthorFlows = "AuthorFlows";
        /// <summary>
        /// 用户状态
        /// </summary>
        public const string UseType = "UseType";
        /// <summary>
        /// 流程根目录
        /// </summary>
        public const string RootOfFlow = "RootOfFlow";
        /// <summary>
        /// 表单根目录
        /// </summary>
        public const string RootOfForm = "RootOfForm";
        /// <summary>
        /// 部门根目录
        /// </summary>
        public const string RootOfDept = "RootOfDept";
        #endregion
    }
	/// <summary>
	/// 管理员
	/// </summary>
    public class AdminEmp : EntityNoName
    {
        #region 基本属性
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(AdminEmpAttr.FK_Dept);
            }
            set
            {
                SetValByKey(AdminEmpAttr.FK_Dept, value);
            }
        }
        public string RootOfDept
        {
            get
            {
                return this.GetValStringByKey(AdminEmpAttr.RootOfDept);
            }
            set
            {
                SetValByKey(AdminEmpAttr.RootOfDept, value);
            }
        }
        public string RootOfFlow
        {
            get
            {
                return this.GetValStrByKey(AdminEmpAttr.RootOfFlow);
            }
            set
            {
                SetValByKey(AdminEmpAttr.RootOfFlow, value);
            }
        }
        public string RootOfForm
        {
            get
            {
                return this.GetValStringByKey(AdminEmpAttr.RootOfForm);
            }
            set
            {
                SetValByKey(AdminEmpAttr.RootOfForm, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 管理员
        /// </summary>
        public AdminEmp() { }
        /// <summary>
        /// 管理员
        /// </summary>
        /// <param name="no"></param>
        public AdminEmp(string no)
        {
            this.No = no;
            try
            {
                if (this.RetrieveFromDBSources() == 0)
                {
                    Emp emp = new Emp(no);
                    this.Copy(emp);
                    this.Insert();
                }
            }
            catch
            {
                this.CheckPhysicsTable();
            }
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Emp", "管理员");

                map.AddTBStringPK(AdminEmpAttr.No, null, "帐号", true, true, 1, 50, 36);
                map.AddTBString(AdminEmpAttr.Name, null, "名称", true,false, 0, 50, 20);
                map.AddDDLEntities(AdminEmpAttr.FK_Dept, null, "主部门", new BP.Port.Depts(), false);

                map.AddDDLSysEnum(AdminEmpAttr.UseSta, 3, "用户状态", true, true, AdminEmpAttr.UseSta, "@0=禁用@1=启用");
                map.AddDDLSysEnum(AdminEmpAttr.UseType, 3, "用户状态", true, true, AdminEmpAttr.UseType, "@0=普通用户@1=管理员用户");

                map.AddDDLEntities(AdminEmpAttr.RootOfFlow, null, "流程权限节点", new BP.WF.Template.FlowSorts(), true);
                map.AddDDLEntities(AdminEmpAttr.RootOfForm, null, "表单权限节点", new BP.WF.Template.SysFormTrees(), true);
                map.AddDDLEntities(AdminEmpAttr.RootOfDept, null, "组织结构权限节点", new BP.GPM.Depts(), true);

                //查询条件.
                map.AddSearchAttr(AdminEmpAttr.UseSta);
                map.AddSearchAttr(AdminEmpAttr.UseType);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 方法
        #endregion
    }
	/// <summary>
	/// 管理员s 
	/// </summary>
	public class AdminEmps : EntitiesNoName
	{	 
		#region 构造
		/// <summary>
		/// 管理员s
		/// </summary>
		public AdminEmps()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new AdminEmp();
			}
		}

        public override int RetrieveAll()
        {
            return base.RetrieveAll("FK_Dept","Idx");
        }
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<AdminEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<AdminEmp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<AdminEmp> Tolist()
        {
            System.Collections.Generic.List<AdminEmp> list = new System.Collections.Generic.List<AdminEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((AdminEmp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

	}
	
}
