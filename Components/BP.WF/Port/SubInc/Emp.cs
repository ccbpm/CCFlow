using System;
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

namespace BP.WF.Port.SubInc
{
	/// <summary>
	/// 子公司人员
	/// </summary>
    public class EmpAttr
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
        /// <summary>
        /// 所在组织
        /// </summary>
        public const string OrgNo = "OrgNo";
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
        public const string UserType = "UserType";
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
	/// 子公司人员
	/// </summary>
    public class Emp : EntityNoName
    {
        #region 基本属性
        public bool IsAdmin
        {
            get
            {
                if (this.No == "admin")
                    return true;

                if (this.UserType == 1 && this.UseSta == 1)
                    return true;

                return false;
            }
        }
        
        /// <summary>
        /// 用户状态
        /// </summary>
        public int UseSta
        {
            get
            {
                return this.GetValIntByKey(EmpAttr.UseSta);
            }
            set
            {
                SetValByKey(EmpAttr.UseSta, value);
            }
        }
        /// <summary>
        /// 用户类型
        /// </summary>
        public int UserType
        {
            get
            {
                return this.GetValIntByKey(EmpAttr.UserType);
            }
            set
            {
                SetValByKey(EmpAttr.UserType, value);
            }
        }
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(EmpAttr.FK_Dept);
            }
            set
            {
                SetValByKey(EmpAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 组织结构
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(EmpAttr.OrgNo);
            }
            set
            {
                SetValByKey(EmpAttr.OrgNo, value);
            }
        }
        public string RootOfDept
        {
            get
            {
                if (this.No == "admin")
                    return "0";

                return this.GetValStringByKey(EmpAttr.RootOfDept);
            }
            set
            {
                SetValByKey(EmpAttr.RootOfDept, value);
            }
        }
        public string RootOfFlow
        {
            get
            {
                if (this.No == "admin")
                    return "0";

                return this.GetValStrByKey(EmpAttr.RootOfFlow);
            }
            set
            {
                SetValByKey(EmpAttr.RootOfFlow, value);
            }
        }
        public string RootOfForm
        {
            get
            {
                if (this.No == "admin")
                    return "0";

                return this.GetValStringByKey(EmpAttr.RootOfForm);
            }
            set
            {
                SetValByKey(EmpAttr.RootOfForm, value);
            }
        }
        #endregion

        #region 构造函数
        public override En.UAC HisUAC
        {
            get
            {
                UAC uac = new En.UAC();
                uac.OpenForSysAdmin();
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 子公司人员
        /// </summary>
        public Emp() { }
        /// <summary>
        /// 子公司人员
        /// </summary>
        /// <param name="no"></param>
        public Emp(string no)
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

                Map map = new Map("WF_Emp", "子公司人员");

                map.AddTBStringPK(EmpAttr.No, null, "帐号", true, true, 1, 50, 36);
                map.AddTBString(EmpAttr.Name, null, "名称", true,false, 0, 50, 20);
                map.AddDDLEntities(EmpAttr.FK_Dept, null, "主部门", new BP.Port.Depts(), false);
                map.AddDDLEntities(EmpAttr.OrgNo, null, "组织", new BP.WF.Port.Incs(), true);
                map.AddDDLSysEnum(EmpAttr.UseSta, 3, "用户状态", true, true, EmpAttr.UseSta, "@0=禁用@1=启用");

                //map.AddDDLEntities(EmpAttr.RootOfFlow, null, "流程权限节点", new BP.WF.Template.FlowSorts(), true);
                //map.AddDDLEntities(EmpAttr.RootOfForm, null, "表单权限节点", new BP.WF.Template.SysFormTrees(), true);
                //map.AddDDLEntities(EmpAttr.RootOfDept, null, "组织结构权限节点", new BP.WF.Port.Incs(), true);
                map.AddTBMyNum();


                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "设置加密密码";
                rm.HisAttrs.AddTBString("FrmID", null, "输入密码", true, false, 0, 100, 100);
                rm.Warning = "您确定要执行设置改密码吗？";
                rm.ClassMethodName = this.ToString() + ".DoSetPassword";
                map.AddRefMethod(rm);
                    

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

      

        #region 方法
        protected override bool beforeUpdateInsertAction()
        {
            if (this.No == "admin")
            {
                this.RootOfDept = "0";
                this.RootOfFlow = "0";
                this.RootOfForm = "0";
            }
            else
            {
                if (this.UserType == 1)
                {
                    //为树目录更新OrgNo编号.
                    BP.WF.Template.FlowSort fs = new Template.FlowSort();
                    fs.No = this.RootOfFlow;  //周朋需要对照翻译.
                    if (fs.RetrieveFromDBSources() == 1)
                    {
                        fs.OrgNo = this.RootOfDept;
                        fs.Update();

                        //更新本级目录.
                        BP.WF.Template.FlowSorts fsSubs = new Template.FlowSorts();
                        fsSubs.Retrieve(BP.WF.Template.FlowSortAttr.ParentNo, fs.No);
                        foreach (BP.WF.Template.FlowSort item in fsSubs)
                        {
                            item.OrgNo = this.RootOfDept;
                            item.Update();
                        }
                    }
                    BP.DA.DBAccess.RunSQL("UPDATE WF_FlowSort SET OrgNo='0' WHERE OrgNo NOT IN (SELECT RootOfDept FROM WF_Emp WHERE UserType=1 )");
                }
            }

            return base.beforeUpdateInsertAction();
        }
        #endregion

        /// <summary>
        /// 设置加密密码存储
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string DoSetPassword(string password)
        {
            string str = BP.Tools.Cryptography.EncryptString(password);
            DBAccess.RunSQLReturnVal("UPDATE Port_Emp SET Pass='" + str + "' WHERE No='" + this.No + "'");
            return "设置成功..";
        }
        /// <summary>
        /// 增加二级子公司人员.
        /// </summary>
        /// <param name="empID"></param>
        /// <returns></returns>
        public string DoAddAdminer(string empID)
        {
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = empID;
            if (emp.RetrieveFromDBSources() == 0)
                return "err@子公司人员增加失败，ID="+empID+"不存在用户表，您增加的子公司人员必须存在与Port_Emp用户表.";

            Emp Emp = new Emp();
            Emp.No = empID;
            if (Emp.RetrieveFromDBSources() == 1)
                return "err@子公司人员【" + Emp.Name + "】已经存在，您不需要在增加.";

            Emp.Copy(emp);
            Emp.FK_Dept = WebUser.FK_Dept;
            Emp.RootOfDept = WebUser.FK_Dept;
            Emp.UserType = 1;
            Emp.Save();

            return "增加成功,请关闭当前窗口查询到该子公司人员，设置他的权限。";

        }
    }
	/// <summary>
	/// 子公司人员s 
	/// </summary>
	public class Emps : EntitiesNoName
	{	 
		#region 构造
		/// <summary>
		/// 子公司人员s
		/// </summary>
		public Emps()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new Emp();
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
        public System.Collections.Generic.IList<Emp> ToJavaList()
        {
            return (System.Collections.Generic.IList<Emp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Emp> Tolist()
        {
            System.Collections.Generic.List<Emp> list = new System.Collections.Generic.List<Emp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Emp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

	}
	
}
