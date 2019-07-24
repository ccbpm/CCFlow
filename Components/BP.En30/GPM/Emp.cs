using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.GPM
{
    /// <summary>
    /// 操作员属性
    /// </summary>
    public class EmpAttr : BP.En.EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 密码
        /// </summary>
        public const string Pass = "Pass";
        /// <summary>
        /// sid
        /// </summary>
        public const string SID = "SID";
        /// <summary>
        /// 电话
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// 邮箱
        /// </summary>
        public const string Email = "Email";
        /// <summary>
        /// 序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 拼音
        /// </summary>
        public const string PinYin = "PinYin";
        #endregion
        /// <summary>
        /// 签字类型
        /// </summary>
        public const string SignType = "SignType";
    }
    /// <summary>
    /// 操作员 的摘要说明。
    /// </summary>
    public class Emp : EntityNoName
    {
        #region 扩展属性
        /// <summary>
        /// 该人员是否被禁用.
        /// </summary>
        public bool IsEnable
        {
            get
            {
                if (this.No == "admin")
                    return true;

                string sql = "SELECT COUNT(FK_Emp) FROM Port_DeptEmpStation WHERE FK_Emp='" + this.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                    return false;

                sql = "SELECT COUNT(FK_Emp) FROM Port_DeptEmp WHERE FK_Emp='" + this.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                    return false;

                return true;
            }
        }
       
        /// <summary>
        /// 拼音
        /// </summary>
        public string PinYin
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.PinYin);
            }
            set
            {
                this.SetValByKey(EmpAttr.PinYin, value);
            }
        }
        /// <summary>
        /// 主要的部门。
        /// </summary>
        public Dept HisDept
        {
            get
            {
                try
                {
                    return new Dept(this.FK_Dept);
                }
                catch (Exception ex)
                {
                    throw new Exception("@获取操作员" + this.No + "部门[" + this.FK_Dept + "]出现错误,可能是系统管理员没有给他维护部门.@" + ex.Message);
                }
            }
        }
        /// <summary>
        /// 部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(EmpAttr.FK_Dept, value);
            }
        }
        public string FK_DeptText
        {
            get
            {
                return this.GetValRefTextByKey(EmpAttr.FK_Dept);
            }
        }
        public string Tel
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.Tel);
            }
            set
            {
                this.SetValByKey(EmpAttr.Tel, value);
            }
        }
        public string Email
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.Email);
            }
            set
            {
                this.SetValByKey(EmpAttr.Email, value);
            }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string Pass
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.Pass);
            }
            set
            {
                this.SetValByKey(EmpAttr.Pass, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(EmpAttr.Idx);
            }
            set
            {
                this.SetValByKey(EmpAttr.Idx, value);
            }
        }
        /// <summary>
        /// 签字类型
        /// </summary>
        public int SignType
        {
            get
            {
                return this.GetValIntByKey(EmpAttr.SignType);
            }
            set
            {
                this.SetValByKey(EmpAttr.SignType, value);
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 检查密码(可以重写此方法)
        /// </summary>
        /// <param name="pass">密码</param>
        /// <returns>是否匹配成功</returns>
        public bool CheckPass(string pass)
        {
            if (this.Pass == pass)
                return true;
            return false;
        }
        #endregion 公共方法

        #region 构造函数
        /// <summary>
        /// 操作员
        /// </summary>
        public Emp()
        {
        }
        /// <summary>
        /// 操作员
        /// </summary>
        /// <param name="no">编号</param>
        public Emp(string no)
        {
            this.No = no.Trim();
            if (this.No.Length == 0)
                throw new Exception("@要查询的操作员编号为空。");
            try
            {
                this.Retrieve();
            }
            catch (Exception ex)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("@用户或者密码错误：[" + no + "]，或者帐号被停用。@技术信息(从内存中查询出现错误)：ex1=" + ex.Message);
            }
        }
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAppAdmin();
                return uac;
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

                Map map = new Map();

                #region 基本属性
                map.EnDBUrl =
                    new DBUrl(DBUrlType.AppCenterDSN); //要连接的数据源（表示要连接到的那个系统数据库）。
                map.PhysicsTable = "Port_Emp"; // 要物理表。
                map.Java_SetDepositaryOfMap( Depositary.Application);    //实体map的存放位置.
                map.Java_SetDepositaryOfEntity( Depositary.Application); //实体存放位置
                map.EnDesc = "用户"; // "用户"; // 实体的描述.
                map.Java_SetEnType(EnType.App);   //实体类型。
                map.EnType = EnType.App;
                #endregion

                #region 字段
                /*关于字段属性的增加 */
                map.AddTBStringPK(EmpAttr.No, null, "登陆账号", true, false, 1, 50, 90);
                map.AddTBString(EmpAttr.Name, null, "名称", true, false, 0, 200, 130);
                map.AddTBString(EmpAttr.Pass, "123", "密码", false, false, 0, 100, 10);

                map.AddDDLEntities(EmpAttr.FK_Dept, null, "主部门", new BP.Port.Depts(), true);

                map.AddTBString(EmpAttr.SID, null, "安全校验码", false, false, 0, 36, 36);
                map.AddTBString(EmpAttr.Tel, null, "电话", true, false, 0, 20, 130);
                map.AddTBString(EmpAttr.Email, null, "邮箱", true, false, 0, 100, 132, true);
                map.AddTBString(EmpAttr.PinYin, null, "拼音", true, false, 0, 500, 132, true);

                // 0=不签名 1=图片签名, 2=电子签名.
                map.AddDDLSysEnum(EmpAttr.SignType, 0, "签字类型", true, true, EmpAttr.SignType,
                    "@0=不签名@1=图片签名@2=电子签名");

                map.AddTBInt(EmpAttr.Idx, 0, "序号", true, false);
                #endregion 字段

                map.AddSearchAttr(EmpAttr.SignType);

                 RefMethod rm = new RefMethod();
                 rm.Title = "设置图片签名";
                 rm.ClassMethodName = this.ToString() + ".DoSinger";
                 rm.RefMethodType = RefMethodType.RightFrameOpen;
                 map.AddRefMethod(rm);

                 rm = new RefMethod();
                 rm.Title = "部门岗位";
                 rm.ClassMethodName = this.ToString() + ".DoEmpDepts";
                 rm.RefMethodType = RefMethodType.RightFrameOpen;
                 map.AddRefMethod(rm);

                 //节点绑定部门. 节点绑定部门.
                 map.AttrsOfOneVSM.AddBranches(new DeptEmps(), new BP.GPM.Depts(),
                    BP.GPM.DeptEmpAttr.FK_Emp,
                    BP.GPM.DeptEmpAttr.FK_Dept, "部门维护", EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");

                 ////他的部门权限
                 //map.AttrsOfOneVSM.Add(new DeptEmps(), new Depts(), DeptEmpAttr.FK_Emp, DeptEmpAttr.FK_Dept,
                 //    DeptAttr.Name, DeptAttr.No, "部门权限", Dot2DotModel.TreeDept);

                rm = new RefMethod();
                rm.Title = "修改密码";
                rm.ClassMethodName = this.ToString() + ".DoResetpassword";
                rm.HisAttrs.AddTBString("pass1", null, "输入密码", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("pass2", null, "再次输入", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "修改主部门";
                rm.ClassMethodName = this.ToString() + ".DoEditMainDept";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
              //  rm.RefAttrKey = EmpAttr.FK_Dept;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }

        public string DoEditMainDept()
        {
            return "../../../GPM/EmpDeptMainDept.htm?FK_Emp=" + this.No;
        }
        

        public string DoEmpDepts()
        {
            return "../../../GPM/EmpDepts.htm?FK_Emp=" + this.No;
        }

        public string DoSinger()
        {
            return "../../../GPM/Siganture.htm?EmpNo=" + this.No;
        }


        protected override bool beforeUpdateInsertAction()
        {
            //增加拼音，以方便查找.
            string pinyinQP = BP.DA.DataType.ParseStringToPinyin(this.Name).ToLower();
            string pinyinJX = BP.DA.DataType.ParseStringToPinyinJianXie(this.Name).ToLower();
            this.PinYin = "," + pinyinQP + "," + pinyinJX + ",";

            //处理岗位信息.
            DeptEmpStations des = new DeptEmpStations();
            des.Retrieve(DeptEmpStationAttr.FK_Emp, this.No);

            string depts = "";
            string stas = "";

            foreach (DeptEmpStation item in des)
            {
                BP.GPM.Dept dept = new BP.GPM.Dept();
                dept.No = item.FK_Dept;
                if (dept.RetrieveFromDBSources() == 0)
                {
                    item.Delete();
                    continue;
                }

                //给拼音重新定义值,让其加上部门的信息.
                this.PinYin = this.PinYin + pinyinJX + "/" + BP.DA.DataType.ParseStringToPinyinJianXie(dept.Name).ToLower()+",";

                BP.Port.Station sta = new Port.Station();
                sta.No = item.FK_Station;
                if (sta.RetrieveFromDBSources() == 0)
                {
                    item.Delete();
                    continue;
                }

                stas += "@" + dept.NameOfPath + "|" + sta.Name;
                depts += "@" + dept.NameOfPath;
            }


            return base.beforeUpdateInsertAction();
        }
        public static string GenerPinYin(string no,string name)
        {
            //增加拼音，以方便查找.
            string pinyinQP = BP.DA.DataType.ParseStringToPinyin(name).ToLower();
            string pinyinJX = BP.DA.DataType.ParseStringToPinyinJianXie(name).ToLower();
            string py = "," + pinyinQP + "," + pinyinJX + ",";

            //处理岗位信息.
            DeptEmpStations des = new DeptEmpStations();
            des.Retrieve(DeptEmpStationAttr.FK_Emp, no);

            string depts = "";
            string stas = "";

            foreach (DeptEmpStation item in des)
            {
                BP.GPM.Dept dept = new BP.GPM.Dept();
                dept.No = item.FK_Dept;
                if (dept.RetrieveFromDBSources() == 0)
                {
                    item.Delete();
                    continue;
                }

                //给拼音重新定义值,让其加上部门的信息.
                py = py + pinyinJX + "/" + BP.DA.DataType.ParseStringToPinyinJianXie(dept.Name).ToLower() + ",";

                BP.Port.Station sta = new Port.Station();
                sta.No = item.FK_Station;
                if (sta.RetrieveFromDBSources() == 0)
                {
                    item.Delete();
                    continue;
                }

                stas += "@" + dept.NameOfPath + "|" + sta.Name;
                depts += "@" + dept.NameOfPath;
            }

            return py;
        }

        /// <summary>
        /// 向上移动
        /// </summary>
        public void DoUp()
        {
            this.DoOrderUp(EmpAttr.FK_Dept, this.FK_Dept, EmpAttr.Idx);
        }
        /// <summary>
        /// 向下移动
        /// </summary>
        public void DoDown()
        {
            this.DoOrderDown(EmpAttr.FK_Dept, this.FK_Dept, EmpAttr.Idx);
        }

        public string DoResetpassword(string pass1, string pass2)
        {
            if (pass1.Equals(pass2) == false)
                return "两次密码不一致";


            this.Pass = pass1;

            this.Update();
            return "密码设置成功";
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new Emps(); }
        }
        #endregion 构造函数
    }
    /// <summary>
    /// 操作员s
    // </summary>
    public class Emps : EntitiesNoName
    {
        #region 构造方法
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
        /// <summary>
        /// 操作员s
        /// </summary>
        public Emps()
        {
        }
        public override int RetrieveAll()
        {
            return base.RetrieveAll("Name");
        }
        #endregion 构造方法

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
