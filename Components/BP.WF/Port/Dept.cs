using BP.En;
using BP.Sys;
using BP.WF.Template;

namespace BP.WF.Port
{
	/// <summary>
	/// 部门属性(即将弃用)
	/// </summary>
    public class DeptAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 父节点编号
        /// </summary>
        public const string ParentNo = "ParentNo";
        /// <summary>
        /// 隶属组织
        /// </summary>
        public const string OrgNo = "OrgNo";
        public const string Leader = "Leader";
        public const string LeaderName = "LeaderName";
        public const string Idx = "Idx";
    }
    /// <summary>
    /// 部门(即将弃用)
    /// </summary>
    public class Dept:EntityNoName
	{
        #region 属性
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(DeptAttr.Idx);
            }
            set
            {
                this.SetValByKey(DeptAttr.Idx, value);
            }
        }
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
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(DeptAttr.OrgNo, value);
            }
        }
        public string Leader
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.Leader);
            }
            set
            {
                this.SetValByKey(DeptAttr.Leader, value);
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
                uac.IsInsert = false;
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


                map.AddTBStringPK(DeptAttr.No, null, "编号", true, false, 1, 30, 40);
                map.AddTBString(DeptAttr.Name, null,"名称", true, false, 0, 60, 200);
                map.AddTBString(DeptAttr.ParentNo, null, "父节点编号", true, true, 0, 30, 40);
                map.AddTBString(DeptAttr.OrgNo, null, "隶属组织", true, true, 0, 50, 250);

                map.AddDDLEntities(DeptAttr.Leader, null, "部门领导", new BP.Port.Emps(), true);
                map.AddTBInt(DeptAttr.Idx, 0, "序号", true, false);

                //map.AddTBString(DeptAttr.FK_Unit, "1", "隶属单位", false, false, 0, 50, 10);
                //设置二级管理员
                RefMethod rm = new RefMethod();
                rm.Title = "设置二级管理员";
                rm.ClassMethodName = this.ToString() + ".ToSetAdminer";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ItIsCanBatch = false; //是否可以批处理？
                map.AddRefMethod(rm);
                this._enMap = map;
                return this._enMap;
            }
		}
        #endregion


        protected override bool beforeDelete()
        {
            //检查是否可以删除.
            BP.Port.Dept dept = new BP.Port.Dept(this.No);
            dept.CheckIsCanDelete();

            return base.beforeDelete();
        }
        /// <summary>
        /// 跳转到单机版二级管理员设置
        /// </summary>
        /// <returns></returns>
        public string ToSetAdminer()
        {
            return "../../../GPM/SetAdminer.htm?FK_Dept=" + this.No;
        }
        /// <summary>
        /// 设置单机版二级管理员
        /// </summary>
        /// <param name="adminer"></param>
        /// <returns></returns>
        public string DoSetAdminer(string adminer, string userName)
        {
            GloVar gloVar = new GloVar();
            gloVar.No = this.No + "_" + adminer + "_Adminer";
            if (gloVar.RetrieveFromDBSources() == 1)
                return userName + "已经设置成部门[" + this.Name + "]的二级管理员";
            gloVar.Name = userName;
            gloVar.Val = adminer;
            gloVar.Note = this.No;
            gloVar.GroupKey = "Adminer";
            gloVar.Insert();

            //设置流程目录、表单目录
            #region 检查流程树.
            BP.WF.Template.FlowSort fs = new BP.WF.Template.FlowSort();
            fs.No = this.No;
            if (fs.RetrieveFromDBSources() == 0)
            {
                //获得根目录节点.
                BP.WF.Template.FlowSort root = new Template.FlowSort();
                int i = root.Retrieve(BP.WF.Template.FlowSortAttr.ParentNo, "0");

                //设置流程树权限.
                fs.No = this.No;
                fs.Name = "流程树";
                fs.ParentNo = root.No;
                fs.Idx = 999;
                fs.DirectInsert();

                //创建下一级目录.
                BP.WF.Template.FlowSort en = fs.DoCreateSubNode() as BP.WF.Template.FlowSort;
                en.Name = "日常办公类";
                en.Domain = "";
                en.DirectUpdate();
            }
            #endregion 检查流程树.

            #region 检查表单树.
            //表单根目录.
            SysFormTree ftRoot = new SysFormTree();
            int val = ftRoot.Retrieve(BP.WF.Template.FlowSortAttr.ParentNo, "0");
            if (val == 0)
            {
                val = ftRoot.Retrieve(BP.WF.Template.FlowSortAttr.No, "100");
                if (val == 0)
                {
                    ftRoot.No = "100";
                    ftRoot.Name = "表单库";
                    ftRoot.ParentNo = "0";
                    ftRoot.Insert();
                }
                else
                {
                    ftRoot.ParentNo = "0";
                    ftRoot.Name = "表单库";
                    ftRoot.Update();
                }
            }

            //设置表单树权限.
            SysFormTree ft = new SysFormTree();
            ft.No = this.No;
            if (ft.RetrieveFromDBSources() == 0)
            {
                ft.Name = "表单树";
                ft.ParentNo = ftRoot.No;
                ft.Idx = 999;
                ft.DirectInsert();

                //创建两个目录.
                SysFormTree mySubFT = ft.DoCreateSubNode() as SysFormTree;
                mySubFT.Name = "日常办公类";
                mySubFT.DirectUpdate();
            }
           
            #endregion 检查表单树.
            return "设置成功";
        }
    }
    /// <summary>
    ///部门集合(即将弃用)
    /// </summary>
    public class Depts : EntitiesNoName
    {
        /// <summary>
        /// 查询全部。
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            if (BP.Web.WebUser.No.Equals("admin")==true)
                return base.RetrieveAll();

            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
            {
                QueryObject qo = new QueryObject(this);
                qo.AddWhere(DeptAttr.No, " = ", BP.Web.WebUser.DeptNo);
                qo.addOr();
                qo.AddWhere(DeptAttr.ParentNo, " = ", BP.Web.WebUser.DeptNo);
                return qo.DoQuery();
            }

            return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo);
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
