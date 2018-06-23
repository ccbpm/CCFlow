using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;

namespace BP.GPM
{
    /// <summary>
    /// 部门属性
    /// </summary>
    public class DeptAttr : EntityTreeAttr
    {
        /// <summary>
        /// 部门负责人
        /// </summary>
        public const string Leader = "Leader";
        /// <summary>
        /// 联系电话
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// 单位全名
        /// </summary>
        public const string NameOfPath = "NameOfPath";
    }
    /// <summary>
    /// 部门
    /// </summary>
    public class Dept : EntityTree
    {
        #region 属性
        /// <summary>
        /// 全名
        /// </summary>
        public string NameOfPath
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.NameOfPath);
            }
            set
            {
                this.SetValByKey(DeptAttr.NameOfPath, value);
            }
        }
        /// <summary>
        /// 父节点的ID
        /// </summary>
        public new string ParentNo
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
        /// <summary>
        /// 领导
        /// </summary>
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
        private Depts _HisSubDepts = null;
        /// <summary>
        /// 它的子节点
        /// </summary>
        public Depts HisSubDepts
        {
            get
            {
                if (_HisSubDepts == null)
                    _HisSubDepts = new Depts(this.No);
                return _HisSubDepts;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 部门
        /// </summary>
        public Dept() { }
        /// <summary>
        /// 部门
        /// </summary>
        /// <param name="no">编号</param>
        public Dept(string no) : base(no) { }
        #endregion

        #region 重写方法
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

                Map map = new Map();
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN); //连接到的那个数据库上. (默认的是: AppCenterDSN )
                map.PhysicsTable = "Port_Dept";
                map.Java_SetEnType(EnType.Admin);

                map.EnDesc = "部门"; //  实体的描述.
                map.Java_SetDepositaryOfEntity(Depositary.Application); //实体map的存放位置.
                map.Java_SetDepositaryOfMap(Depositary.Application);    // Map 的存放位置.

                map.AddTBStringPK(DeptAttr.No, null, "编号", true, true, 1, 50, 20);

                //比如xx分公司财务部
                map.AddTBString(DeptAttr.Name, null, "名称", true, false, 0, 100, 30);

                //比如:\\驰骋集团\\南方分公司\\财务部
                map.AddTBString(DeptAttr.NameOfPath, null, "部门路径", true, true, 0, 300, 30, true);

                map.AddTBString(DeptAttr.ParentNo, null, "父节点编号", true, false, 0, 100, 30);

                // 01,0101,010101.
                map.AddTBString(DeptAttr.TreeNo, null, "树编号", false, false, 0, 100, 30);

                //部门领导.
                map.AddTBString(DeptAttr.Leader, null, "部门领导", true, false, 0, 100, 30);
                map.AddTBString(DeptAttr.Tel, null, "联系电话", true, false, 0, 100, 30);

                //顺序号.
                map.AddTBInt(DeptAttr.Idx, 0, "顺序号", true, false);

                //是否是目录
                map.AddTBInt(DeptAttr.IsDir, 0, "是否是目录", true, true);

                //  map.AddDDLEntities(DeptAttr. null, "部门类型", new DeptTypes(), true);

                RefMethod rm = new RefMethod();
                rm.Title = "重置该部门一下的部门路径";
                rm.ClassMethodName = this.ToString() + ".DoResetPathName";
                rm.RefMethodType = RefMethodType.Func;

                string msg = "当该部门名称变化后,该部门与该部门的子部门名称路径(Port_Dept.NameOfPath)将发生变化.";
                msg += "\t\n 该部门与该部门的子部门的人员路径也要发生变化Port_Emp列DeptDesc.StaDesc.";
                msg += "\t\n 您确定要执行吗?";
                rm.Warning = msg;

                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "增加同级部门";
                rm.ClassMethodName = this.ToString() + ".DoSameLevelDept";
                rm.HisAttrs.AddTBString("No", null, "同级部门编号", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("Name", null, "部门名称", true, false, 0, 100, 100); 
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "增加下级部门";
                rm.ClassMethodName = this.ToString() + ".DoSubDept";
                rm.HisAttrs.AddTBString("No", null, "同级部门编号", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("Name", null, "部门名称", true, false, 0, 100, 100);
                map.AddRefMethod(rm);


                //平铺模式.
                map.AttrsOfOneVSM.AddGroupPanelModel(new DeptStations(), new Stations(),
                    DeptStationAttr.FK_Dept,
                    DeptStationAttr.FK_Station, "对应岗位(平铺)", StationAttr.FK_StationType);

                map.AttrsOfOneVSM.AddGroupListModel(new DeptStations(), new Stations(),
                  DeptStationAttr.FK_Dept,
                  DeptStationAttr.FK_Station, "对应岗位(树)", StationAttr.FK_StationType);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 创建下级节点.
        /// </summary>
        /// <returns></returns>
        public string DoMyCreateSubNode()
        {
            Entity en = this.DoCreateSubNode();
            return en.ToJson();
        }

        /// <summary>
        /// 创建同级节点.
        /// </summary>
        /// <returns></returns>
        public string DoMyCreateSameLevelNode()
        {
            Entity en = this.DoCreateSameLevelNode();
            return en.ToJson();
        }

        public string DoSameLevelDept(string no,string name)
        {
            Dept en = new Dept();
            en.No = no;
            if (en.RetrieveFromDBSources() == 1)
                return "err@编号已经存在";

            en.Name = name;
            en.ParentNo = this.ParentNo;
            en.Insert();
             
            return "增加成功..";
        }
        public string DoSubDept(string no, string name)
        {
            Dept en = new Dept();
            en.No = no;
            if (en.RetrieveFromDBSources() == 1)
                return "err@编号已经存在";

            en.Name = name;
            en.ParentNo = this.No;
            en.Insert();

            return "增加成功..";
        }
        /// <summary>
        /// 重置部门
        /// </summary>
        /// <returns></returns>
        public string DoResetPathName()
        {
            this.GenerNameOfPath();
            return "重置成功.";
        }

        /// <summary>
        /// 生成部门全名称.
        /// </summary>
        public void GenerNameOfPath()
        {
            string name = this.Name;

            //根目录不再处理
            if (this.IsRoot == true)
            {
                this.NameOfPath = name;
                this.DirectUpdate();
                this.GenerChildNameOfPath(this.No);
                return;
            }

            Dept dept = new Dept();
            dept.No = this.ParentNo;
            if (dept.RetrieveFromDBSources() == 0)
                return;

            while (true)
            {
                if (dept.IsRoot)
                    break;

                name = dept.Name + "\\" + name;
                dept = new Dept(dept.ParentNo);
            }
            //根目录
            name = dept.Name + "\\" + name;
            this.NameOfPath = name;
            this.DirectUpdate();

            this.GenerChildNameOfPath(this.No);

            //更新人员路径信息.
            BP.GPM.Emps emps = new Emps();
            emps.Retrieve(EmpAttr.FK_Dept, this.No);
            foreach (BP.GPM.Emp emp in emps)
                emp.Update();
        }

        /// <summary>
        /// 处理子部门全名称
        /// </summary>
        /// <param name="FK_Dept"></param>
        public void GenerChildNameOfPath(string deptNo)
        {
            Depts depts = new Depts(deptNo);
            if (depts != null && depts.Count > 0)
            {
                foreach (Dept dept in depts)
                {
                    dept.GenerNameOfPath();
                    GenerChildNameOfPath(dept.No);


                    //更新人员路径信息.
                    BP.GPM.Emps emps = new Emps();
                    emps.Retrieve(EmpAttr.FK_Dept, this.No);
                    foreach (BP.GPM.Emp emp in emps)
                        emp.Update();
                }
            }
        }
    }
    /// <summary>
    ///部门集合
    /// </summary>
    public class Depts : EntitiesTree
    {
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
        /// 部门集合
        /// </summary>
        public Depts()
        {
        }
        /// <summary>
        /// 部门集合
        /// </summary>
        /// <param name="parentNo">父部门No</param>
        public Depts(string parentNo)
        {
            this.Retrieve(DeptAttr.ParentNo, parentNo);
        }

        #region 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。
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
        #endregion 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。
    }
}
