using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.CCOA
{
    /// <summary>
    /// 任务 属性
    /// </summary>
    public class TaskAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 模式
        /// </summary>
        public const string TaskPRI = "TaskPRI";
        /// <summary>
        /// 内容1
        /// </summary>
        public const string Docs = "Docs";
        public const string Title = "Title";

        /// <summary>
        /// 内容2
        /// </summary>
        public const string TaskSta = "TaskSta";
        /// <summary>
        /// 内容3
        /// </summary>
        public const string ManagerEmpNo = "ManagerEmpNo";
        /// <summary>
        /// 负责人
        /// </summary>
        public const string ManagerEmpName = "ManagerEmpName";

        public const string RefLabelNo = "RefLabelNo";
        public const string RefLabelName = "RefLabelName";

        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录人名称
        /// </summary>
        public const string RecName = "RecName";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 隶属日期
        /// </summary>
        public const string RiQi = "RiQi";
        /// <summary>
        /// 年月
        /// </summary>
        public const string DTFrom = "DTFrom";
        /// <summary>
        /// 项目数
        /// </summary>
        public const string RefEmpsNo = "RefEmpsNo";
        /// <summary>
        /// 第几周
        /// </summary>
        public const string RefEmpsName = "RefEmpsName";
        /// <summary>
        /// 年度
        /// </summary>
        public const string DTTo = "DTTo";
        /// <summary>
        /// 负责人.
        /// </summary>
        public const string Manager = "Manager";

        public const string ParentNo = "ParentNo";
        public const string IsSubTask = "IsSubTask";
    }
    /// <summary>
    /// 任务
    /// </summary>
    public class Task : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get { return this.GetValStrByKey(TaskAttr.OrgNo); }
            set { this.SetValByKey(TaskAttr.OrgNo, value); }
        }
        public string Rec
        {
            get { return this.GetValStrByKey(TaskAttr.Rec); }
            set { this.SetValByKey(TaskAttr.Rec, value); }
        }
        public string RecName
        {
            get { return this.GetValStrByKey(TaskAttr.RecName); }
            set { this.SetValByKey(TaskAttr.RecName, value); }
        }
        public string RDT
        {
            get { return this.GetValStrByKey(TaskAttr.RDT); }
            set { this.SetValByKey(TaskAttr.RDT, value); }
        }
        /// <summary>
        /// 日期
        /// </summary>
        public string RiQi
        {
            get { return this.GetValStrByKey(TaskAttr.RiQi); }
            set { this.SetValByKey(TaskAttr.RiQi, value); }
        }
        /// <summary>
        /// 年月
        /// </summary>
        public string DTFrom
        {
            get { return this.GetValStrByKey(TaskAttr.DTFrom); }
            set { this.SetValByKey(TaskAttr.DTFrom, value); }
        }
        public string DTTo
        {
            get { return this.GetValStrByKey(TaskAttr.DTTo); }
            set { this.SetValByKey(TaskAttr.DTTo, value); }
        }
        /// <summary>
        /// 项目数
        /// </summary>
        public int RefEmpsNo
        {
            get { return this.GetValIntByKey(TaskAttr.RefEmpsNo); }
            set { this.SetValByKey(TaskAttr.RefEmpsNo, value); }
        }
        /// <summary>
        /// 第几周？
        /// </summary>
        public int RefEmpsName
        {
            get { return this.GetValIntByKey(TaskAttr.RefEmpsName); }
            set { this.SetValByKey(TaskAttr.RefEmpsName, value); }
        }
        /// <summary>
        /// 负责人
        /// </summary>
        public float Manager
        {
            get { return this.GetValFloatByKey(TaskAttr.Manager); }
            set { this.SetValByKey(TaskAttr.Manager, value); }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// 权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (WebUser.IsAdmin)
                {
                    uac.IsUpdate = true;
                    return uac;
                }
                return base.HisUAC;
            }
        }
        /// <summary>
        /// 任务
        /// </summary>
        public Task()
        {
        }
        public Task(string mypk)
        {
            this.setMyPK(mypk);
            this.Retrieve();
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

                Map map = new Map("OA_Task", "任务");

                map.AddMyPK();

                map.AddTBString(TaskAttr.Title, null, "标题", false, false, 0, 500, 10);
                map.AddTBString(TaskAttr.Docs, null, "内容", false, false, 0, 4000, 10);

                map.AddTBString(TaskAttr.ParentNo, null, "父节点ID", false, false, 0, 50, 10);
                map.AddTBInt(TaskAttr.IsSubTask, 0, "是否是子任务", true, false);


                map.AddDDLSysEnum(TaskAttr.TaskPRI, 0, "优先级", true, false, "TaskPRI", "@0=高@1=中@2=低");
                map.AddDDLSysEnum(TaskAttr.TaskSta, 0, "状态", true, false, "TaskSta", "@0=未完成@1=已完成");

                map.AddTBDateTime(TaskAttr.DTFrom, null, "日期从", false, false);
                map.AddTBDateTime(TaskAttr.DTTo, null, "到", false, false);

                map.AddTBString(TaskAttr.ManagerEmpNo, null, "负责人", false, false, 0, 30, 10);
                map.AddTBString(TaskAttr.ManagerEmpName, null, "负责人名称", false, false, 0, 40, 10);

                map.AddTBString(TaskAttr.RefEmpsNo, null, "参与人编号", false, false, 0, 3000, 10);
                map.AddTBString(TaskAttr.RefEmpsName, null, "参与人名称", false, false, 0, 3000, 10);

                map.AddTBString(TaskAttr.RefLabelNo, null, "标签标号", false, false, 0, 3000, 10);
                map.AddTBString(TaskAttr.RefLabelName, null, "标签名称", false, false, 0, 3000, 10);

                map.AddTBString(TaskAttr.OrgNo, null, "组织编号", false, false, 0, 100, 10);
                map.AddTBString(TaskAttr.Rec, null, "记录人", false, false, 0, 100, 10);
                map.AddTBString(TaskAttr.RecName, null, "记录人名称", false, false, 0, 100, 10, true);
                map.AddTBDateTime(TaskAttr.RDT, null, "记录时间", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行方法.
        protected override bool beforeInsert()
        {
            this.setMyPK(DBAccess.GenerGUID());
            this.Rec = WebUser.No;
            this.RecName = WebUser.Name;
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                this.OrgNo = WebUser.OrgNo;

            this.SetValByKey("RDT", DataType.CurrentDateTime);

            return base.beforeInsert();
        }
        protected override bool beforeUpdate()
        {
            ////计算条数.
            //this.RefEmpsNo = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS N FROM OA_TaskDtl WHERE RefPK='" + this.MyPK + "'");

            ////计算合计工作小时..
            //this.Manager = DBAccess.RunSQLReturnValInt("SELECT SUM(Hour) + Sum(Minute)/60.00 AS N FROM OA_TaskDtl WHERE RefPK='" + this.MyPK + "'");

            return base.beforeUpdate();
        }
        #endregion 执行方法.
    }
    /// <summary>
    /// 任务 s
    /// </summary>
    public class Tasks : EntitiesMyPK
    {

        #region 查询.
        /// <summary>
        /// 所有的任务
        /// </summary>
        /// <returns></returns>
        public string Task_AllTasks()
        {
            QueryObject qo = new QueryObject(this);

            qo.addLeftBracket();
            qo.AddWhere(TaskAttr.Rec, WebUser.No);
            qo.addOr();
            qo.AddWhere(TaskAttr.RefEmpsNo, " like ", "%," + WebUser.No + ",%");
            qo.addOr();
            qo.AddWhere(TaskAttr.ManagerEmpNo, " like ", "%," + WebUser.No + ",%");
            qo.addRightBracket();
            qo.addAnd();
            qo.AddWhere(TaskAttr.IsSubTask,  0);

            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                qo.addAnd();
                qo.AddWhere(TaskAttr.OrgNo, " = ", WebUser.OrgNo);
            }
            qo.DoQuery();
            return this.ToJson();
        }
        
        public string TextBox_EmpPinYin(string key)
        {
            string whereSQL = " AND OrgNo='"+BP.Web.WebUser.OrgNo+"'";
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                whereSQL = "";

            string sql = "";
            sql = "SELECT No, Name FROM Port_Emp WHERE (No like '%" + key+ "%' or Name like '%" + key+ "%' or PinYin like '%" + key+"%' )  " + whereSQL;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }

        public string Selecter_DeptEmps()
        {
            DataSet ds = new DataSet();

            Depts depts = new Depts();
            depts.RetrieveAll();

            Emps emps = new Emps();
            emps.RetrieveAll();

            ds.Tables.Add(depts.ToDataTableField("Depts"));
            ds.Tables.Add(emps.ToDataTableField("Emps"));

            return BP.Tools.Json.ToJson(ds);

        }
        #endregion 重写.

        #region 重写.
        /// <summary>
        /// 任务
        /// </summary>
        public Tasks() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Task();
            }
        }
        #endregion 重写.


        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Task> ToJavaList()
        {
            return (System.Collections.Generic.IList<Task>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Task> Tolist()
        {
            System.Collections.Generic.List<Task> list = new System.Collections.Generic.List<Task>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Task)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
