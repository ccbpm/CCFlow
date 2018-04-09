using System;
using System.Data;
using BP.DA;
using BP.En;
using System.Collections;
using BP.Web;
using BP.GPM;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 查找类型
    /// </summary>
    public enum FindColleague
    {
        /// <summary>
        /// 所有 
        /// </summary>
        All,
        /// <summary>
        /// 指定职务
        /// </summary>
        SpecDuty,
        /// <summary>
        /// 指定岗位
        /// </summary>
        SpecStation      
    }
    /// <summary>
    /// 找人规则属性
    /// </summary>
    public class FindWorkerRoleAttr : BP.En.EntityOIDNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 类型0值
        /// </summary>
        public const string SortVal0 = "SortVal0";
        /// <summary>
        /// 类型0标签
        /// </summary>
        public const string SortText0 = "SortText0";
        /// <summary>
        /// 类型1值
        /// </summary>
        public const string SortVal1 = "SortVal1";
        /// <summary>
        /// 类型1标签
        /// </summary>
        public const string SortText1 = "SortText1";
        /// <summary>
        /// 类型2值
        /// </summary>
        public const string SortVal2 = "SortVal2";
        /// <summary>
        /// 类型2标签
        /// </summary>
        public const string SortText2 = "SortText2";


        /// 类型2值
        /// </summary>
        public const string SortVal3 = "SortVal3";
        /// <summary>
        /// 类型2标签
        /// </summary>
        public const string SortText3 = "SortText3";

        /// 类型4值
        /// </summary>
        public const string SortVal4 = "SortVal4";
        /// <summary>
        /// 类型4标签
        /// </summary>
        public const string SortText4 = "SortText4";


        /// <summary>
        /// Tag1值
        /// </summary>
        public const string TagVal0 = "TagVal0";
        /// <summary>
        /// Tag1标签
        /// </summary>
        public const string TagText0 = "TagText0";

       
        /// <summary>
        /// Tag1值
        /// </summary>
        public const string TagVal1 = "TagVal1";
        /// <summary>
        /// Tag1标签
        /// </summary>
        public const string TagText1 = "TagText1";
        /// <summary>
        /// Tag1值
        /// </summary>
        public const string TagVal2 = "TagVal2";
        /// <summary>
        /// Tag1标签
        /// </summary>
        public const string TagText2 = "TagText2";
        
        /// <summary>
        /// Tag1值
        /// </summary>
        public const string TagVal3 = "TagVal3";
        /// <summary>
        /// Tag1标签
        /// </summary>
        public const string TagText3 = "TagText3";
       

        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 是否可用？
        /// </summary>
        public const string IsEnable = "IsEnable";
        #endregion
    }
    /// <summary>
    /// 找人规则
    /// </summary>
    public class FindWorkerRole : EntityOIDName
    {
        #region  找同事
        /// <summary>
        /// 找同事规则
        /// </summary>
        public FindColleague HisFindColleague
        {
            get
            {
                return (FindColleague)int.Parse(this.TagVal3);
            }
        }
        #endregion  找同事

        #region  找领导类型
        /// <summary>
        /// 寻找领导类型
        /// </summary>
        public FindLeaderType HisFindLeaderType
        {
            get
            {
                return (FindLeaderType)int.Parse(this.SortVal1);
            }
        }
        /// <summary>
        /// 模式
        /// </summary>
        public FindLeaderModel HisFindLeaderModel
        {
            get
            {
                return (FindLeaderModel)int.Parse(this.SortVal2);
            }
        }
        #endregion

        #region 基本属性
        public bool IsEnable
        {
            get
            {
                return this.GetValBooleanByKey(FindWorkerRoleAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.IsEnable, value);
            }
        }
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsUpdate = true;
                return uac;
            }
        }
        /// <summary>
        /// 找人规则的事务编号
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(FindWorkerRoleAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.FK_Node, value);
            }
        }

        /// <summary>
        /// 类别0值
        /// </summary>
        public string SortVal0
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.SortVal0);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.SortVal0, value);
            }
        }
        /// <summary>
        /// 类别0Text
        /// </summary>
        public string SortText0
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.SortText0);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.SortText0, value);
            }
        }

        public string SortText3
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.SortText3);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.SortText3, value);
            }
        }

        /// <summary>
        /// 类别1值
        /// </summary>
        public string SortVal1
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.SortVal1);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.SortVal1, value);
            }
        }
        /// <summary>
        /// 类别1Text
        /// </summary>
        public string SortText1
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.SortText1);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.SortText1, value);
            }
        }

        /// <summary>
        /// 类别2值
        /// </summary>
        public string SortVal2
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.SortVal2);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.SortVal2, value);
            }
        }
        /// <summary>
        /// 类别2Text
        /// </summary>
        public string SortText2
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.SortText2);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.SortText2, value);
            }
        }
        /// <summary>
        /// 类别3值
        /// </summary>
        public string SortVal3
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.SortVal3);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.SortVal3, value);
            }
        }
        /// <summary>
        /// 类别3Text
        /// </summary>
        public string SortText4
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.SortText4);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.SortText4, value);
            }
        }
        /// <summary>
        /// 数据0
        /// </summary>
        public string TagVal0
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.TagVal0);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.TagVal0, value);
            }
        }
        /// <summary>
        /// 数据1
        /// </summary>
        public string TagVal1
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.TagVal1);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.TagVal1, value);
            }
        }
        /// <summary>
        /// TagVal2
        /// </summary>
        public string TagVal2
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.TagVal2);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.TagVal2, value);
            }
        }
        /// <summary>
        /// TagVal3
        /// </summary>
        public string TagVal3
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.TagVal3);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.TagVal3, value);
            }
        }
        /// <summary>
        /// 数据0
        /// </summary>
        public string TagText0
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.TagText0);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.TagText0, value);
            }
        }
        /// <summary>
        /// TagText1
        /// </summary>
        public string TagText1
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.TagText1);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.TagText1, value);
            }
        }

        /// <summary>
        /// 数据1
        /// </summary>
        public string TagText2
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.TagText2);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.TagText2, value);
            }
        }
        /// <summary>
        /// TagText3
        /// </summary>
        public string TagText3
        {
            get
            {
                return this.GetValStringByKey(FindWorkerRoleAttr.TagText3);
            }
            set
            {
                this.SetValByKey(FindWorkerRoleAttr.TagText3, value);
            }
        }
        #endregion

        #region 变量
        public WorkNode town = null;
        public WorkNode currWn = null;
        public Flow fl = null;
        string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
        public Paras ps = null;
        public Int64 WorkID = 0;
        public Node HisNode = null;
        #endregion 变量

        #region 构造函数
        /// <summary>
        /// 找人规则
        /// </summary>
        public FindWorkerRole() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_FindWorkerRole", "找人规则");
                 

                map.AddTBIntPKOID();

                map.AddTBString(FindWorkerRoleAttr.Name, null, "Name", true, false, 0, 200, 0);

                map.AddTBInt(FindWorkerRoleAttr.FK_Node, 0, "节点ID", false, false);

                // 规则存储.
                map.AddTBString(FindWorkerRoleAttr.SortVal0, null, "SortVal0", true, false, 0, 200, 0);
                map.AddTBString(FindWorkerRoleAttr.SortText0, null, "SortText0", true, false, 0, 200, 0);

                map.AddTBString(FindWorkerRoleAttr.SortVal1, null, "SortVal1", true, false, 0, 200, 0);
                map.AddTBString(FindWorkerRoleAttr.SortText1, null, "SortText1", true, false, 0, 200, 0);

                map.AddTBString(FindWorkerRoleAttr.SortVal2, null, "SortText2", true, false, 0, 200, 0);
                map.AddTBString(FindWorkerRoleAttr.SortText2, null, "SortText2", true, false, 0, 200, 0);

                map.AddTBString(FindWorkerRoleAttr.SortVal3, null, "SortVal3", true, false, 0, 200, 0);
                map.AddTBString(FindWorkerRoleAttr.SortText3, null, "SortText3", true, false, 0, 200, 0);


                // 规则采集信息值存储.
                map.AddTBString(FindWorkerRoleAttr.TagVal0, null, "TagVal0", true, false, 0, 1000, 0);
                map.AddTBString(FindWorkerRoleAttr.TagVal1, null, "TagVal1", true, false, 0, 1000, 0);
                map.AddTBString(FindWorkerRoleAttr.TagVal2, null, "TagVal2", true, false, 0, 1000, 0);
                map.AddTBString(FindWorkerRoleAttr.TagVal3, null, "TagVal3", true, false, 0, 1000, 0);

                // TagText
                map.AddTBString(FindWorkerRoleAttr.TagText0, null, "TagText0", true, false, 0, 1000, 0);
                map.AddTBString(FindWorkerRoleAttr.TagText1, null, "TagText1", true, false, 0, 1000, 0);
                map.AddTBString(FindWorkerRoleAttr.TagText2, null, "TagText2", true, false, 0, 1000, 0);
                map.AddTBString(FindWorkerRoleAttr.TagText3, null, "TagText3", true, false, 0, 1000, 0);

                map.AddTBInt(FindWorkerRoleAttr.IsEnable, 1, "是否可用", false, false);
                map.AddTBInt(FindWorkerRoleAttr.Idx, 0, "IDX", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 内部方法.
        /// <summary>
        /// 上移
        /// </summary>
        public void DoUp()
        {
            this.DoOrderUp(FindWorkerRoleAttr.FK_Node, this.FK_Node.ToString(), FindWorkerRoleAttr.Idx);
        }
        /// <summary>
        /// 下移
        /// </summary>
        public void DoDown()
        {
            this.DoOrderDown(FindWorkerRoleAttr.FK_Node, this.FK_Node.ToString(), FindWorkerRoleAttr.Idx);
        }
        private string sql = "";
        #endregion 内部方法
 
        /// <summary>
        /// 生成数据
        /// </summary>
        /// <returns></returns>
        public DataTable GenerWorkerOfDataTable()
        {
            DataTable dt = new DataTable();
            // 首先判断第一类别
            switch (this.SortVal0)
            {
                case "ByDept":
                    return this.GenerByDept();
                case "Leader":
                case "SpecEmps":

                    #region   首先找到2级参数，就是当事人是谁？
                    string empNo = null;
                    string empDept = null;
                    switch (this.HisFindLeaderType)
                    {
                        case FindLeaderType.Submiter: // 当前提交人的直线领导
                            empNo = BP.Web.WebUser.No;
                            empDept = BP.Web.WebUser.FK_Dept;
                            break;
                        case FindLeaderType.SpecNodeSubmiter: // 指定节点提交人的直线领导.
                            sql = "SELECT FK_Emp,FK_Dept FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " AND FK_Node=" + this.TagVal1;
                            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                            if (dt.Rows.Count == 0)
                                throw new Exception("@没有找到指定节点数据，请反馈给系统管理员，技术信息:" + sql);
                            empNo = dt.Rows[0][0] as string;
                            empDept = dt.Rows[0][1] as string;
                            break;
                        case FindLeaderType.BySpecField: //指定节点字段人员的直接领导..
                            sql = " SELECT " + this.TagVal1 + " FROM " + this.HisNode.HisFlow.PTable + " WHERE OID=" + this.WorkID;
                            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                            empNo = dt.Rows[0][0] as string;
                            if (DataType.IsNullOrEmpty(empNo))
                                throw new Exception("@指定的节点字段(" + this.TagVal1 + ")的值为空.");
                            //指定它
                            Emp emp = new Emp();
                            emp.No = empNo;
                            if (emp.RetrieveFromDBSources() == 0)
                                throw new Exception("@指定的节点字段(" + this.TagVal1 + ")的值(" + empNo + ")是非法的人员编号...");
                            empDept = emp.FK_Dept;
                            break;
                        default:
                            throw new Exception("@尚未处理的Case:" + this.HisFindLeaderType);
                            break;
                    }
                    if (DataType.IsNullOrEmpty(empNo))
                        throw new Exception("@遗漏的判断步骤，没有找到指定的工作人员.");
                    #endregion

                    if (this.SortVal0 == "Leader")
                        return GenerHisLeader(empNo, empDept); // 产生他的领导并返回.
                    else
                        return GenerHisSpecEmps(empNo, empDept); // 产生他的特定的同事并返回.
                default:
                    break;
            }
            return null;
        }

        #region 按部门查找
        private DataTable GenerByDept()
        {
            //部门编号.
            string deptNo = this.TagVal1;

            //职务-岗位。
            string objVal = this.TagVal2;

            string way = this.SortVal1;

            string sql = "";
            switch (way)
            {
                case "0": //按职务找.
                    sql = "SELECT B.No,B.Name FROM Port_DeptEmp A, Port_Emp B WHERE A.FK_Dept='"+deptNo+"'  AND A.FK_Duty='"+objVal+"' AND B.No=A.FK_Emp";
                    break;
                case "1": //按岗位找.
                    sql = "SELECT B.No,B.Name FROM Port_DeptEmpStation A, Port_Emp B WHERE A.FK_Dept='" + deptNo + "'  AND A.FK_Station='" + objVal + "' AND B.No=A.FK_Emp";
                    break;
                case "2": //所有该部门的人员.
                    sql = "SELECT B.No,B.Name FROM Port_DeptEmp A, Port_Emp B WHERE A.FK_Dept='" + deptNo + "' AND B.No=A.FK_Emp";
                    break;
                default:
                    break;
            }
            return DBAccess.RunSQLReturnTable(sql);
        }
        #endregion

        #region 找同事
        /// <summary>
        /// 当前提交人的直线领导
        /// </summary>
        /// <returns></returns>
        private DataTable GenerHisSpecEmps(string empNo, string empDept)
        {
            DeptEmp de = new DeptEmp();

            DataTable dt = new DataTable();
            string leader = null;
            string tempDeptNo = "";

            switch (this.HisFindColleague)
            {
                case FindColleague.All: // 所有该部门性质下的人员.
                    sql = "SELECT Leader FROM Port_DeptEmp WHERE FK_Emp='" + empNo + "' AND FK_Dept='" + empDept + "'";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    leader = dt.Rows[0][0] as string;
                    if (DataType.IsNullOrEmpty(leader))
                        throw new Exception("@系统管理员没有给(" + empNo + ")在部门(" + empDept + ")中设置直接领导.");

                    break;
                case FindColleague.SpecDuty: // 特定职务级别的领导.
                    tempDeptNo = empDept.Clone() as string;
                    while (true)
                    {
                        sql = "SELECT FK_Emp FROM Port_DeptEmp WHERE DutyLevel='" + this.TagVal2 + "' AND FK_Dept='" + tempDeptNo + "'";
                        DataTable mydt = DBAccess.RunSQLReturnTable(sql);
                        if (mydt.Rows.Count != 0)
                            return mydt; /*直接反回.*/

                        Dept d = new Dept(tempDeptNo);
                        if (d.ParentNo == "0")
                            return null; /*如果到了跟节点.*/
                        tempDeptNo = d.ParentNo;
                    }
                    break;
                case FindColleague.SpecStation: // 特定岗位的领导.
                    tempDeptNo = empDept.Clone() as string;
                    while (true)
                    {
                        sql = "SELECT FK_Emp FROM Port_DeptEmpStation WHERE FK_Station='" + this.TagVal2 + "' AND FK_Dept='" + tempDeptNo + "'";
                        DataTable mydt = DBAccess.RunSQLReturnTable(sql);
                        if (mydt.Rows.Count != 0)
                            return mydt; /*直接反回.*/

                        Dept d = new Dept(tempDeptNo);
                        if (d.ParentNo == "0")
                        {
                            /* 在直线领导中没有找到 */
                            return null; /*如果到了跟节点.*/
                        }
                        tempDeptNo = d.ParentNo;
                    }
                    break;
                default:
                    break;
            }

            // 增加列.
            dt.Columns.Add(new DataColumn("No", typeof(string)));
            DataRow dr = dt.NewRow();
            dr[0] = leader;
            dt.Rows.Add(dr);
            return dt;
        }
        public string ErrMsg = null;
        #endregion 直线领导

        #region 直线领导
        /// <summary>
        /// 当前提交人的直线领导
        /// </summary>
        /// <returns></returns>
        private DataTable GenerHisLeader(string empNo,string empDept)
        {
            DeptEmp de = new DeptEmp();

            DataTable dt=new DataTable();
            string leader=null;
            string tempDeptNo = "";

            switch (this.HisFindLeaderModel)
            {
                case FindLeaderModel.DirLeader: // 直接领导.
                    sql = "SELECT Leader FROM Port_DeptEmp WHERE FK_Emp='" + empNo + "' AND FK_Dept='" + empDept + "'";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    leader = dt.Rows[0][0] as string;
                    if (DataType.IsNullOrEmpty(leader))
                        throw new Exception("@系统管理员没有给(" + empNo + ")在部门(" + empDept + ")中设置直接领导.");
                    break;
                case FindLeaderModel.SpecDutyLevelLeader: // 特定职务级别的领导.
                    tempDeptNo = empDept.Clone() as string;
                    while (true)
                    {
                        sql = "SELECT FK_Emp FROM Port_DeptEmp WHERE DutyLevel='" + this.TagVal2 + "' AND FK_Dept='" + tempDeptNo + "'";
                        DataTable mydt = DBAccess.RunSQLReturnTable(sql);
                        if (mydt.Rows.Count != 0)
                            return mydt; /*直接反回.*/

                        Dept d = new Dept(tempDeptNo);
                        if (d.ParentNo == "0")
                            return null; /*如果到了跟节点.*/
                        tempDeptNo = d.ParentNo;
                    }
                    break;
                case FindLeaderModel.DutyLeader: // 特定职务的领导.
                    tempDeptNo = empDept.Clone() as string;
                    while (true)
                    {
                          sql = "SELECT FK_Emp FROM Port_DeptEmp WHERE FK_Duty='" + this.TagVal2 + "' AND FK_Dept='" + tempDeptNo + "'";
                          DataTable mydt = DBAccess.RunSQLReturnTable(sql);
                        if (mydt.Rows.Count != 0)
                            return mydt; /*直接反回.*/

                        Dept d = new Dept(tempDeptNo);
                        if (d.ParentNo == "0")
                            return null; /*如果到了跟节点.*/
                        tempDeptNo = d.ParentNo;
                    }
                    break;
                case FindLeaderModel.SpecStation: // 特定岗位的领导.
                    tempDeptNo = empDept.Clone() as string;
                    while (true)
                    {
                        sql = "SELECT FK_Emp FROM Port_DeptEmpStation WHERE FK_Station='" + this.TagVal2 + "' AND FK_Dept='" + tempDeptNo + "'";
                        DataTable mydt = DBAccess.RunSQLReturnTable(sql);
                        if (mydt.Rows.Count != 0)
                            return mydt; /*直接反回.*/

                        Dept d = new Dept(tempDeptNo);
                        if (d.ParentNo == "0")
                        {
                            /* 在直线领导中没有找到 */
                            return null; /*如果到了跟节点.*/
                        }
                        tempDeptNo = d.ParentNo;
                    }
                    break;
                default:
                    break;
            }

            // 增加列.
            dt.Columns.Add(new DataColumn("No", typeof(string)));
            DataRow dr = dt.NewRow();
            dr[0] = leader;
            dt.Rows.Add(dr);
            return dt;
        }
        #endregion 直线领导

        public string DBStr
        {
            get
            {
                return SystemConfig.AppCenterDBVarStr;
            }
        }
    }
    /// <summary>
    /// 找人规则集合
    /// </summary>
    public class FindWorkerRoles : EntitiesOID
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FindWorkerRole();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 找人规则集合
        /// </summary>
        public FindWorkerRoles()
        {
        }
        /// <summary>
        /// 找人规则集合
        /// </summary>
        /// <param name="nodeID"></param>
        public FindWorkerRoles(int nodeID)
        {
            this.Retrieve(FindWorkerRoleAttr.FK_Node, nodeID, FindWorkerRoleAttr.Idx);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FindWorkerRole> ToJavaList()
        {
            return (System.Collections.Generic.IList<FindWorkerRole>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FindWorkerRole> Tolist()
        {
            System.Collections.Generic.List<FindWorkerRole> list = new System.Collections.Generic.List<FindWorkerRole>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FindWorkerRole)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
