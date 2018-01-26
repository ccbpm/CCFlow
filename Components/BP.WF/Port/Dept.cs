using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;

namespace BP.WF.Port
{
	/// <summary>
	/// 部门属性
	/// </summary>
    public class DeptAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 父节点编号
        /// </summary>
        public const string ParentNo = "ParentNo";
    }
	/// <summary>
	/// 部门
	/// </summary>
	public class Dept:EntityNoName
	{
		#region 属性
        /// <summary>
        /// 父节点编号
        /// </summary>
        public string ParentNo
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(DeptAttr.ParentNo, value);
            }
        }
         
		#endregion

		#region 构造函数
		/// <summary>
		/// 部门
		/// </summary>
		public Dept(){}
		/// <summary>
		/// 部门
		/// </summary>
		/// <param name="no">编号</param>
        public Dept(string no) : base(no){}
		#endregion

		#region 重写方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
		public override UAC HisUAC
		{
			get
			{
				UAC uac = new UAC();
				uac.OpenForSysAdmin();
				return uac;
			}
		}
		/// <summary>
		/// Map
		/// </summary>
		public override Map EnMap
		{
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Dept", "部门");

                map.Java_SetDepositaryOfEntity(Depositary.Application); //实体map的存放位置.
                map.Java_SetDepositaryOfMap( Depositary.Application);    // Map 的存放位置.

                map.AdjunctType = AdjunctType.None;

                map.AddTBStringPK(DeptAttr.No, null, "编号", true, false, 1, 30, 40);
                map.AddTBString(DeptAttr.Name, null,"名称", true, false, 0, 60, 200);
                map.AddTBString(DeptAttr.ParentNo, null, "父节点编号", true, false, 0, 30, 40);
              //  map.AddTBString(DeptAttr.FK_Unit, "1", "隶属单位", false, false, 0, 50, 10);

                RefMethod rm = new RefMethod();
                rm.Title = "初始化子公司二级管理员";
                rm.Warning = "设置为子公司后，系统就会在流程树上分配一个目录节点.";
                rm.ClassMethodName = this.ToString() + ".SetSubInc";
                rm.HisAttrs.AddTBString("No", null, "子公司管理员编号", true, false, 0, 100, 100);
                map.AddRefMethod(rm);
                
                this._enMap = map;
                return this._enMap;
            }
		}
		#endregion

        public string SetSubInc(string userNo)
        {
            //设置流程树权限.
            BP.WF.Template.FlowSort fs = new WF.Template.FlowSort();
            fs.No = "Inc" + this.No;
            if (fs.RetrieveFromDBSources() != 0)
            {
                //AdminEmp ae = new AdminEmp();
                //int i = ae.Retrieve(AdminEmpAttr.RootOfFlow, "Inc" + this.No);
                //if (i == 0)
                //    return "info@该部门[" + this.No + "," + this.Name + "]已经被设置过了子公司，但是没有指定管理员.";
                //else
                //    return "info@该部门[" + this.No + "," + this.Name + "]已经被设置过了子公司，管理员为:" + ae.No + "," + ae.Name;
            }

            //设置二级管理员.
            AdminEmp ad = new AdminEmp();
            ad.No = userNo;
            if (ad.RetrieveFromDBSources() == 0)
            {
                BP.Port.Emp emp = new BP.Port.Emp();
                emp.No = userNo;
                if (emp.RetrieveFromDBSources() == 0)
                    return "err@用户编号错误:" + userNo;

                ad.Copy(emp);
                ad.Insert();
            }

            ad.No = userNo;
            ad.RootOfDept = this.No;
            ad.RootOfFlow = "Inc" + this.No;
            ad.RootOfForm = "Inc" + this.No;
            ad.UserType = 1;
            ad.UseSta = 1;
            ad.Update();


            //设置流程树权限.
            fs.Name = this.Name;
            fs.ParentNo = "00";
            fs.OrgNo = this.No;
            fs.Idx = 999;
            fs.Save();

            //设置流程树权限.
            BP.Sys.FrmTree ft = new Sys.FrmTree();
            ft.No = "Inc" + this.No;
            if (ft.RetrieveFromDBSources() == 0)
            {
                ft.Name = this.Name;
                ft.ParentNo = "1";
                // ft.OrgNo = this.No;
                ft.Idx = 999;
                ft.Insert();
            }
            else
            {
                ft.Name = this.Name;
                ft.ParentNo = "1";
                //  ft.OrgNo = this.No;
                ft.Idx = 999;
                ft.Update();
            }

            return "设置成功,["+ad.No+","+ad.Name+"]重新登录就可以看到.";
        }
	}
	/// <summary>
	///部门集合
	/// </summary>
    public class Depts : EntitiesNoName
    {
        /// <summary>
        /// 查询全部。
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            if (BP.Web.WebUser.No == "admin")
                return base.RetrieveAll();

            QueryObject qo = new QueryObject(this);
            qo.AddWhere(DeptAttr.No, " = ", BP.Web.WebUser.FK_Dept);
            qo.addOr();
            qo.AddWhere(DeptAttr.ParentNo, " = ", BP.Web.WebUser.FK_Dept);
            return qo.DoQuery();
        }
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Dept();
            }
        }
        /// <summary>
        /// create ens
        /// </summary>
        public Depts()
        {
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Dept> ToJavaList()
        {
            return (System.Collections.Generic.IList<Dept>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Dept> Tolist()
        {
            System.Collections.Generic.List<Dept> list = new System.Collections.Generic.List<Dept>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Dept)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
