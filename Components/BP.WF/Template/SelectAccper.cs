using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
	/// <summary>
	/// 选择接受人属性
	/// </summary>
    public class SelectAccperAttr
    {
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 到人员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 操作员名称
        /// </summary>
        public const string EmpName = "EmpName";
        /// <summary>
        /// 部门名称
        /// </summary>
        public const string DeptName = "DeptName";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 办理意见  信息
        /// </summary>
        public const string Info = "Info";
        /// <summary>
        /// 以后发送是否按此计算
        /// </summary>
        public const string IsRemember = "IsRemember";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 类型(@0=接受人@1=抄送人)
        /// </summary>
        public const string AccType = "AccType";
        /// <summary>
        /// 维度标记
        /// </summary>
        public const string Tag = "Tag";
        /// <summary>
        /// 时限天
        /// </summary>
        public const string TimeLimit = "TimeLimit";
        /// <summary>
        /// 时限小时
        /// </summary>
        public const string TSpanHour = "TSpanHour";
        /// <summary>
        /// 接受日期(计划)
        /// </summary>
        public const string PlanADT = "ADT";
        /// <summary>
        /// 应完成日期(计划)
        /// </summary>
        public const string PlanSDT = "SDT";
    }
	/// <summary>
	/// 选择接受人
	/// 节点的到人员有两部分组成.	 
	/// 记录了从一个节点到其他的多个节点.
	/// 也记录了到这个节点的其他的节点.
	/// </summary>
    public class SelectAccper : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenAll();
                return uac;
            }
        }
        /// <summary>
        ///工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(SelectAccperAttr.WorkID);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.WorkID, value);
            }
        }
        /// <summary>
        ///节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(SelectAccperAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.FK_Node, value);
            }
        }
       
        /// <summary>
        /// 到人员
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(SelectAccperAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 标记
        /// </summary>
        public string Tag
        {
            get
            {
                return this.GetValStringByKey(SelectAccperAttr.Tag);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.Tag, value);
            }
        }
        /// <summary>
        /// 人员名称
        /// </summary>
        public string EmpName
        {
            get
            {
                string s= this.GetValStringByKey(SelectAccperAttr.EmpName);
                if (s == "" || s == null)
                    s = this.FK_Emp;
                return s;
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.EmpName, value);
            }
        }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName
        {
            get
            {
                return this.GetValStringByKey(SelectAccperAttr.DeptName);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.DeptName, value);
            }
        }
        /// <summary>
        /// 接收人
        /// </summary>
        public string Rec
        {
            get
            {
                return this.GetValStringByKey(SelectAccperAttr.Rec);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.Rec, value);
            }
        }
        /// <summary>
        /// 办理意见  信息
        /// </summary>
        public string Info
        {
            get
            {
                return this.GetValStringByKey(SelectAccperAttr.Info);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.Info, value);
            }
        }
        /// <summary>
        /// 是否记忆
        /// </summary>
        public bool IsRemember
        {
            get
            {
                return this.GetValBooleanByKey(SelectAccperAttr.IsRemember);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.IsRemember, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(SelectAccperAttr.Idx);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.Idx, value);
            }
        }
        /// <summary>
        ///  类型(@0=接受人@1=抄送人)
        /// </summary>
        public int AccType
        {
            get
            {
                return this.GetValIntByKey(SelectAccperAttr.AccType);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.AccType, value);
            }
        }
        /// <summary>
        /// 时限
        /// </summary>
        public float TSpanHour
        {
            get
            {
                return this.GetValFloatByKey(SelectAccperAttr.TSpanHour);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.TSpanHour, value);
            }
        }

        /// <summary>
        /// 工作到达日期(计划)
        /// </summary>
        public string PlanADT
        {
            get
            {
                return this.GetValStringByKey(SelectAccperAttr.PlanADT);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.PlanADT, value);
            }
        }
        /// <summary>
        /// 工作应完成日期(计划)
        /// </summary>
        public string PlanSDT
        {
            get
            {
                return this.GetValStringByKey(SelectAccperAttr.PlanSDT);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.PlanSDT, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 选择接受人
        /// </summary>
        public SelectAccper()
        {

        }
        public SelectAccper(string mypk)
        {
            this.MyPK = mypk;
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

                Map map = new Map("WF_SelectAccper", "选择接受/抄送人信息");
                map.AddMyPK();

                map.AddTBInt(SelectAccperAttr.FK_Node, 0, "接受人节点", true, false);
                
                map.AddTBInt(SelectAccperAttr.WorkID, 0, "WorkID", true, false);
                map.AddTBString(SelectAccperAttr.FK_Emp, null, "FK_Emp", true, false, 0, 20, 10);
                map.AddTBString(SelectAccperAttr.EmpName, null, "EmpName", true, false, 0, 20, 10);
                map.AddTBString(SelectAccperAttr.DeptName, null, "部门名称", true, false, 0, 400, 10);
                map.AddTBInt(SelectAccperAttr.AccType, 0, "类型(@0=接受人@1=抄送人)", true, false);
                map.AddTBString(SelectAccperAttr.Rec, null, "记录人", true, false, 0, 20, 10);
                map.AddTBString(SelectAccperAttr.Info, null, "办理意见信息", true, false, 0, 200, 10);

                map.AddTBInt(SelectAccperAttr.IsRemember, 0, "以后发送是否按本次计算", true, false);
                map.AddTBInt(SelectAccperAttr.Idx, 0, "顺序号(可以用于流程队列审核模式)", true, false);
                /*
                 *  add 2015-1-12.
                 * 为了解决多维度的人员问题.
                 * 在分流点向下发送时, 一个人可以分配两次任务，但是这个任务需要一个维度来区分。
                 * 这个维度，有可能是一个类别，批次。
                 */
                map.AddTBString(SelectAccperAttr.Tag, null, "维度信息Tag", true, false, 0, 200, 10);

                map.AddTBInt(SelectAccperAttr.TimeLimit, 0, "时限-天", true, false);
                map.AddTBFloat(SelectAccperAttr.TSpanHour, 0, "时限-小时", true, false);

                //应该完成日期，为了自动计算未来的日期.
                map.AddTBDateTime(SelectAccperAttr.PlanADT, null, "到达日期(计划)", true, false);
                map.AddTBDateTime(SelectAccperAttr.PlanSDT, null, "应完成日期(计划)", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


        protected override bool beforeInsert()
        {
            this.ResetPK();

            return base.beforeInsert();
        }

        public void ResetPK()
        {
            //注释掉了.
            // this.MyPK = this.FK_Node + "_" + this.WorkID + "_" + this.FK_Emp+"_"+this.Idx;
            this.MyPK = this.FK_Node + "_" + this.WorkID + "_" + this.FK_Emp;
        }
        protected override bool beforeUpdateInsertAction()
        {
            if (this.DeptName.Length == 0)
            {
                bool isHavePathName = DBAccess.IsExitsTableCol("Port_Dept", "NameOfpath");
                if (isHavePathName == true)
                {
                    this.DeptName = DBAccess.RunSQLReturnStringIsNull("select a.NameOfPath from port_dept a,port_emp b where a.No=b.fk_dept and b.no='" + this.FK_Emp + "'", "无");
                    if (this.DeptName == "无")
                        this.DeptName = DBAccess.RunSQLReturnStringIsNull("select a.name from port_dept a,port_emp b where a.No=b.fk_dept and b.no='" + this.FK_Emp + "'", "无");
                }
                else
                    this.DeptName = DBAccess.RunSQLReturnStringIsNull("select a.name from port_dept a,port_emp b where a.No=b.fk_dept and b.no='" + this.FK_Emp + "'", "无");
            }

            this.ResetPK();
            this.Rec = BP.Web.WebUser.No;
            return base.beforeUpdateInsertAction();
        }
        //protected override bool beforeUpdateInsertAction()
        //{
        //    this.Rec = BP.Web.WebUser.No;
        //    return base.beforeUpdateInsertAction();
        //}
    }
	/// <summary>
	/// 选择接受人
	/// </summary>
    public class SelectAccpers : EntitiesMyPK
    {
        /// <summary>
        /// 是否记忆下次选择
        /// </summary>
        public bool IsSetNextTime
        {
            get
            {
                if (this.Count == 0)
                    return false;

                foreach (SelectAccper item in this)
                {
                    if (item.IsRemember == true)
                        return item.IsRemember;
                }
                return false;
            }
        }
        /// <summary>
        /// 查询接收人,如果没有设置就查询历史记录设置的接收人.
        /// </summary>
        /// <param name="fk_node"></param>
        /// <param name="Rec"></param>
        /// <returns></returns>
        public int QueryAccepter(int fk_node, string rec, Int64 workid)
        {
            //查询出来当前的数据.
            int i = this.Retrieve(SelectAccperAttr.FK_Node, fk_node,
                 SelectAccperAttr.WorkID, workid);
            if (i != 0)
                return i; /*如果没有就找最大的workid.*/

            //找出最近的工作ID
            int maxWorkID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT Max(WorkID) FROM WF_SelectAccper WHERE Rec='" + rec + "' AND FK_Node=" + fk_node, 0);
            if (maxWorkID == 0)
                return 0;

            //查询出来该数据.
            this.Retrieve(SelectAccperAttr.FK_Node, fk_node,
           SelectAccperAttr.WorkID, maxWorkID);

            //返回查询结果.
            return this.Count;
        }
        /// <summary>
        /// 查询上次的设置
        /// </summary>
        /// <param name="fk_node">节点编号</param>
        /// <param name="rec">当前人员</param>
        /// <param name="workid">工作ID</param>
        /// <returns></returns>
        public int QueryAccepterPriSetting(int fk_node)
        {
            //找出最近的工作ID.
            int maxWorkID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT Max(WorkID) FROM WF_SelectAccper WHERE " + SelectAccperAttr.IsRemember + "=1 AND Rec='" + BP.Web.WebUser.No + "' AND FK_Node=" + fk_node, 0);
            if (maxWorkID == 0)
                return 0;

            //查询出来该数据.
            this.Retrieve(SelectAccperAttr.FK_Node, fk_node,
           SelectAccperAttr.WorkID, maxWorkID);

            //返回查询结果.
            return this.Count;
        }
        /// <summary>
        /// 他的到人员
        /// </summary>
        public Emps HisEmps
        {
            get
            {
                Emps ens = new Emps();
                foreach (SelectAccper ns in this)
                {
                    ens.AddEntity(new Emp(ns.FK_Emp));
                }
                return ens;
            }
        }
        /// <summary>
        /// 他的工作节点
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                Nodes ens = new Nodes();
                foreach (SelectAccper ns in this)
                {
                    ens.AddEntity(new Node(ns.FK_Node));
                }
                return ens;
            }
        }
        /// <summary>
        /// 选择接受人
        /// </summary>
        public SelectAccpers() { }
        /// <summary>
        /// 查询出来选择的人员
        /// </summary>
        /// <param name="fk_flow"></param>
        /// <param name="workid"></param>
        public SelectAccpers( Int64 workid)
        {
            BP.En.QueryObject qo = new QueryObject(this);
            qo.AddWhere(SelectAccperAttr.WorkID, workid);
            qo.addOrderByDesc(SelectAccperAttr.FK_Node,SelectAccperAttr.Idx);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SelectAccper();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SelectAccper> ToJavaList()
        {
            return (System.Collections.Generic.IList<SelectAccper>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SelectAccper> Tolist()
        {
            System.Collections.Generic.List<SelectAccper> list = new System.Collections.Generic.List<SelectAccper>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SelectAccper)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
