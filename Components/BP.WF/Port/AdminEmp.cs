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
	/// 管理员
	/// </summary>
    public class AdminEmp : EntityNoName
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
                return this.GetValIntByKey(AdminEmpAttr.UseSta);
            }
            set
            {
                SetValByKey(AdminEmpAttr.UseSta, value);
            }
        }
        /// <summary>
        /// 用户类型
        /// </summary>
        public int UserType
        {
            get
            {
                return this.GetValIntByKey(AdminEmpAttr.UserType);
            }
            set
            {
                SetValByKey(AdminEmpAttr.UserType, value);
            }
        }
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
        /// <summary>
        /// 组织结构
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(AdminEmpAttr.OrgNo);
            }
            set
            {
                SetValByKey(AdminEmpAttr.OrgNo, value);
            }
        }
        public string RootOfDept
        {
            get
            {
                if (this.No == "admin")
                    return "0";

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
                if (this.No == "admin")
                    return "0";

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
                if (this.No == "admin")
                    return "0";

                return this.GetValStringByKey(AdminEmpAttr.RootOfForm);
            }
            set
            {
                SetValByKey(AdminEmpAttr.RootOfForm, value);
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
                map.AddDDLEntities(AdminEmpAttr.OrgNo, null, "组织", new BP.WF.Port.Incs(), true);


                map.AddDDLSysEnum(AdminEmpAttr.UseSta, 3, "用户状态", true, true, AdminEmpAttr.UseSta, "@0=禁用@1=启用");
                map.AddDDLSysEnum(AdminEmpAttr.UserType, 3, "用户状态", true, true, AdminEmpAttr.UserType, "@0=普通用户@1=管理员用户");

                map.AddDDLEntities(AdminEmpAttr.RootOfFlow, null, "流程权限节点", new BP.WF.Template.FlowSorts(), true);
                map.AddDDLEntities(AdminEmpAttr.RootOfForm, null, "表单权限节点", new BP.WF.Template.SysFormTrees(), true);
                map.AddDDLEntities(AdminEmpAttr.RootOfDept, null, "组织结构权限节点", new BP.WF.Port.Incs(), true);
                 
                map.AddTBMyNum();

                //查询条件.
                map.AddSearchAttr(AdminEmpAttr.UseSta);
                map.AddSearchAttr(AdminEmpAttr.UserType);
                map.AddSearchAttr(AdminEmpAttr.OrgNo);


                RefMethod rm = new RefMethod();
                rm.Title = "增加管理员";
                //  rm.GroupName = "高级设置";
                rm.HisAttrs.AddTBString("FrmID", null, "管理员编号ID", true, false, 0, 100, 100);

                rm.ClassMethodName = this.ToString() + ".DoAddAdminer";
                rm.Icon = "../../WF/Img/Btn/Copy.GIF";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置加密密码";
                rm.HisAttrs.AddTBString("FrmID", null, "输入密码", true, false, 0, 100, 100);
                rm.Warning = "您确定要执行设置改密码吗？";
                rm.ClassMethodName = this.ToString() + ".DoSetPassword";
               // rm.Icon = "../../WF/Img/Btn/Copy.GIF";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "测试";
                rm.HisAttrs.AddTBString("wenben", null, "文本", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBInt("shuzi", 0, "数字", true, false);
                rm.HisAttrs.AddTBDate("riqi", null, "日期", true, false);
                rm.HisAttrs.AddTBDate("riqishijian", null, "日期时间", true, false);
                rm.HisAttrs.AddBoolen("boolen", false, "Boolen");
                rm.HisAttrs.AddDDLSysEnum("WFState", 0, "枚举", true, true);
                rm.HisAttrs.AddDDLEntities("FK_Emp", null, "外键", new BP.Port.Depts(), true);
                rm.ClassMethodName = this.ToString() + ".DoTest";
                map.AddRefMethod(rm);



                rm = new RefMethod();
                rm.Title = "测试Boolen";
                rm.HisAttrs.AddDDLEntities("FK_Emp", null, "外键", new BP.Port.Depts(), true);

              //  rm.HisAttrs.AddDDLSysEnum("WFState", 0, "枚举", true, true);
           //     rm.HisAttrs.AddBoolen("boolenField", false, "Boolen字段测试");
             //   rm.HisAttrs.AddTBInt("intType", 0, "数字", true, false);

                rm.ClassMethodName = this.ToString() + ".DoTestBoolen";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        //,string isOK, int wfstate, string fk_emp
        public string DoTestBoolen(string intType )
        {
            return " WFState=" + intType;
           // return "boolen=" + boolen + " intType=" + intType;
        }

        //,string isOK, int wfstate, string fk_emp
        public string DoTest(string wenben, int shuzi, string riqi, string dateTime, bool boolen, int meiJu, string waiJian)
        {
            string str="";
            str += "@text=" + wenben;
            str += "@int=" + shuzi;
            str += "@data=" + riqi;
            str += "@dateTime=" + dateTime;
            str += "@boolen=" + boolen;
            str += "@meiJu=" + meiJu;
            str += "@waiJian=" + waiJian;
            //str += "isOK=" + isOK;
            //str += "wfstate=" + wfstate;
            //str += "fk_emp=" + fk_emp;
            return str;
        }

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
        /// 增加二级管理员.
        /// </summary>
        /// <param name="empID"></param>
        /// <returns></returns>
        public string DoAddAdminer(string empID)
        {
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = empID;
            if (emp.RetrieveFromDBSources() == 0)
                return "err@管理员增加失败，ID="+empID+"不存在用户表，您增加的管理员必须存在与Port_Emp用户表.";

            AdminEmp adminEmp = new AdminEmp();
            adminEmp.No = empID;
            if (adminEmp.RetrieveFromDBSources() == 1)
                return "err@管理员【" + adminEmp.Name + "】已经存在，您不需要在增加.";

            adminEmp.Copy(emp);
            adminEmp.FK_Dept = WebUser.FK_Dept;
            adminEmp.RootOfDept = WebUser.FK_Dept;
            adminEmp.UserType = 1;
            adminEmp.Insert();

            return "增加成功,请关闭当前窗口查询到该管理员，设置他的权限。";

        }
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
