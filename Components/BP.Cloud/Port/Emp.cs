using System;
using System.Text.RegularExpressions;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.En30.Utility.Web;
using BP.Web;
using BP.Difference;

namespace BP.Cloud
{
    /// <summary>
    /// 操作员 属性
    /// </summary>
    public class EmpAttr : BP.En.EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 用户ID
        /// </summary>
        public const string UserID = "UserID";
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
        public const string Leader = "Leader";
        #endregion

        /// <summary>
        /// QQAppID
        /// </summary>
        public const string QQAppID = "QQAppID";
        /// <summary>
        /// QQ号
        /// </summary>
        public const string QQ = "QQ";
        /// <summary>
        /// 组织解构编码
        /// </summary>
        public const string OrgNo = "OrgNo";
        public const string OrgName = "OrgName";
        public const string OpenID = "OpenID";
        public const string OpenID2 = "OpenID2";
        /// <summary>
        /// 连接的ID
        /// </summary>
        public const string unionid = "unionid";
        public const string EmpSta = "EmpSta";
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
        /// 用户ID.
        /// </summary>
        public string UserID
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.UserID);
            }
            set
            {
                this.SetValByKey(EmpAttr.UserID, value);
            }
        }
        public string OpenID
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.OpenID);
            }
            set
            {
                this.SetValByKey(EmpAttr.OpenID, value);
            }
        }
        public string OpenID2
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.OpenID2);
            }
            set
            {
                this.SetValByKey(EmpAttr.OpenID2, value);
            }
        }
        public string unionid
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.unionid);
            }
            set
            {
                this.SetValByKey(EmpAttr.unionid, value);
            }
        }
        public string SID
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.SID);
            }
            set
            {
                this.SetValByKey(EmpAttr.SID, value);
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
        /// 直属领导
        /// </summary>
        public string Leader
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.Leader);
            }
            set
            {
                this.SetValByKey(EmpAttr.Leader, value);
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
        public string QQ
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.QQ);
            }
            set
            {
                this.SetValByKey(EmpAttr.QQ, value);
            }
        }
        public string QQAppID
        {
            get
            {
                return this.GetValStringByKey(EmpAttr.QQAppID);
            }
            set
            {
                this.SetValByKey(EmpAttr.QQAppID, value);
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

        public int EmpSta
        {
            get
            {
                return this.GetValIntByKey(EmpAttr.EmpSta);
            }
            set
            {
                this.SetValByKey(EmpAttr.EmpSta, value);
            }
        }
        /// <summary>
        /// 组织结构编码
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(EmpAttr.OrgNo, value);
            }
        }
        public string OrgName
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.OrgName);
            }
            set
            {
                this.SetValByKey(EmpAttr.OrgName, value);
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
            //启用加密
            if (SystemConfig.isEnablePasswordEncryption == true)
                pass = BP.Tools.Cryptography.EncryptString(pass);

            /*使用数据库校验.*/
            if (this.Pass.Equals(pass) == true)
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
            try
            {
                //如果小于11位估计不是一个手机号.
                /* if (BP.DA.DataType.IsMobile(no) == false)
                     this.No = BP.Web.WebUser.OrgNo + "_" + no;
                 else
                     this.No = no;*/
                this.No = no;
                this.Retrieve();
              //  this.No = no;
            }
            catch (Exception ex)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("err@用户账号[" + this.No + "]错误:" + ex.Message);
            }
        }

        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.IsAdmin == true)
                    uac.OpenAll();
                else
                    uac.Readonly();
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

                Map map = new Map("Port_Emp", "用户");
                map.EnType = EnType.App;
                map.IndexField = EmpAttr.FK_Dept;

                #region 字段

                /*关于字段属性的增加 */
                map.AddTBStringPK(EmpAttr.No, null, "手机号/ID", true, false, 1, 500, 90);
                map.AddTBString(EmpAttr.UserID, null, "用户ID", true, false, 0, 100, 10);
              
                map.AddTBString(EmpAttr.Name, null, "姓名", true, false, 0, 500, 130);
                map.AddTBString(EmpAttr.Pass, null, "密码", false, false, 0, 100, 10);

                map.AddDDLEntities(EmpAttr.FK_Dept, null, "当前登录的部门",
                    new BP.Port.Depts(), false);

                //状态. 0=启用，1=禁用.
                map.AddTBInt(EmpAttr.EmpSta, 0, "EmpSta", false, false);

                map.AddTBString(EmpAttr.Leader, null, "部门领导", false, false, 0, 100, 10);

                map.AddTBString(EmpAttr.SID, null, "安全校验码", false, false, 0, 36, 36);
                map.AddTBString(EmpAttr.Tel, null, "电话", true, false, 0, 20, 130);
                map.AddTBString(EmpAttr.Email, null, "邮箱", true, false, 0, 100, 132, true);
                map.AddTBString(EmpAttr.PinYin, null, "拼音", true, false, 0, 1000, 132, true);

                map.AddTBString(EmpAttr.OrgNo, null, "OrgNo", true, false, 0, 500, 132, true);
                map.AddTBString(EmpAttr.OrgName, null, "OrgName", true, false, 0, 500, 132, true);


                map.AddTBString(EmpAttr.QQ, null, "QQ", true, false, 0, 500, 132, true);
                map.AddTBString(EmpAttr.QQAppID, null, "QQAppID", true, false, 0, 500, 132, true);

                map.AddTBString(EmpAttr.OpenID, null, "OpenID", false, false, 0, 200, 36);//小程序的openID
                map.AddTBString(EmpAttr.OpenID2, null, "OpenID2", false, false, 0, 200, 36);//公众号的openID
                map.AddTBString(EmpAttr.unionid, null, "unionid", false, false, 0, 200, 36);//公众号的openID



                //map.AddDDLEntities(EmpAttr.OrgNo, null, "组织", new BP.Cloud.Orgs(), false);
                //map.AddTBString(EmpAttr.OrgNo, null, "OrgNo", false, false, 0, 36, 36);
                //map.AddTBString(EmpAttr.OrgName, null, "OrgName", false, false, 0, 36, 36);
                map.AddTBInt(EmpAttr.Idx, 0, "序号", true, false);
                #endregion 字段

                this._enMap = map;
                return this._enMap;
            }
        }

        public string DoEditMainDept()
        {
            return SystemConfig.CCFlowWebPath + "GPM/EmpDeptMainDept.htm?FK_Emp=" + this.No + "&FK_Dept=" + this.FK_Dept;
        }

        public string DoEmpDepts()
        {
            return SystemConfig.CCFlowWebPath + "GPM/EmpDepts.htm?FK_Emp=" + this.No;
        }

        public string DoSinger()
        {
            //路径
            return SystemConfig.CCFlowWebPath + "GPM/Siganture.htm?EmpNo=" + this.No;
        }
        protected override bool beforeInsert()
        {
            if (SystemConfig.isEnablePasswordEncryption == true)
            {
                if (this.Pass == "")
                {
                    this.Pass = "123";
                }
                this.Pass = BP.Tools.Cryptography.EncryptString(this.Pass);
            }

            string sid = BP.Tools.SecurityUnit.EncryptByAes(WebUser.OrgNo + "_" + WebUser.No + DBAccess.GenerGUID()); 
            this.SID = sid;
            
            ////当前人员所在的部门.
            //  this.OrgNo = BP.Web.WebUser.OrgNo;

            ////处理主部门的问题.
            //DeptEmp de = new DeptEmp();
            //de.FK_Dept = this.FK_Dept;
            //de.FK_Emp = this.No;
            //de.IsMainDept = true;
            //de.OrgNo = this.OrgNo;
            //de.Save();
            BP.Sys.Base.Glo.WriteUserLog("新建人员:" + this.ToJson(), "人员数据操作");
            return base.beforeInsert();
        }

        protected override bool beforeUpdateInsertAction()
        {
            //增加拼音，以方便查找.
            if (DataType.IsNullOrEmpty(this.Name) == true)
                throw new Exception("err@名称不能为空.");

            string pinyinQP = BP.DA.DataType.ParseStringToPinyin(this.Name).ToLower();
            string pinyinJX = BP.DA.DataType.ParseStringToPinyinJianXie(this.Name).ToLower();
            this.PinYin = "," + pinyinQP + "," + pinyinJX + ",";

            ////处理岗位信息.
            //DeptEmpStations des = new DeptEmpStations();
            //des.Retrieve(DeptEmpStationAttr.FK_Emp, this.No);

            //string depts = "";
            //string stas = "";

            //foreach (DeptEmpStation item in des)
            //{
            //    BP.GPM.Dept dept = new BP.GPM.Dept();
            //    dept.No = item.FK_Dept;
            //    if (dept.RetrieveFromDBSources() == 0)
            //    {
            //        item.Delete();
            //        continue;D:\CCFlowCloud\BP.Cloud\Port\Emp.cs
            //    }

            //    //给拼音重新定义值,让其加上部门的信息.
            //    this.PinYin = this.PinYin + pinyinJX + "/" + BP.DA.DataType.ParseStringToPinyinJianXie(dept.Name).ToLower()+",";

            //    BP.Port.Station sta = new Port.Station();
            //    sta.No = item.FK_Station;
            //    if (sta.RetrieveFromDBSources() == 0)
            //    {
            //        item.Delete();
            //        continue;
            //    }

            //    stas += "@" + dept.NameOfPath + "|" + sta.Name;
            //    depts += "@" + dept.NameOfPath;
            //}
            BP.Sys.Base.Glo.WriteUserLog("新建/修改人员:" + this.ToJson(), "人员数据操作");
            return base.beforeUpdateInsertAction();
        }

        /// <summary>
        /// 保存后修改WF_Emp中的邮箱
        /// </summary>
        protected override void afterInsertUpdateAction()
        {
            string sql = "Select Count(*) From WF_Emp Where No='" + this.No + "'";
            int count = DBAccess.RunSQLReturnValInt(sql);
            if (count == 0)
            { 
                sql = "INSERT INTO WF_Emp (No,Name,Email,UseSta,FK_Dept,OrgNo) VALUES('" + this.No + "','" + this.Name + "','" + this.Email + "',1,'"+this.FK_Dept+"','"+this.OrgNo+"')";
                DBAccess.RunSQL(sql); 
            }

            //修改Port_Emp中的缓存                  
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = this.No;
            if (emp.RetrieveFromDBSources() != 0)
            {
                emp.DeptNo = this.FK_Dept;
                emp.Update();
            }
            BP.Sys.Base.Glo.WriteUserLog("新建/修改人员:" + this.ToJson(), "人员数据操作");
            base.afterInsertUpdateAction();
        }
        protected override bool beforeDelete()
        {
            //if (this.OrgNo != BP.Web.WebUser.OrgNo)
            //    throw new Exception("err@您不能删除别人的数据.");

            //DeptEmps ens = new DeptEmps();
            //ens.Delete(DeptEmpAttr.FK_Emp, this.No, DeptEmpAttr.FK_Dept,this.FK_Dept, DeptEmpAttr.OrgNo,this.OrgNo);

            //DeptEmpStations ensD = new DeptEmpStations();
            //ensD.Delete(DeptEmpStationAttr.FK_Emp, this.No, DeptEmpStationAttr.FK_Dept, this.FK_Dept, DeptEmpStationAttr.OrgNo, this.OrgNo);


            //BP.Sys.Base.Glo.WriteUserLog("删除人员:" + this.ToJson(), "人员数据操作");
            return base.beforeDelete();
        }
        /// <summary>
        /// 删除之后要做的事情
        /// </summary>
        protected override void afterDelete()
        {
            base.afterDelete();
        }

        public static string GenerPinYin(string no, string name)
        {
            //增加拼音，以方便查找.
            string pinyinQP = BP.DA.DataType.ParseStringToPinyin(name).ToLower();
            string pinyinJX = BP.DA.DataType.ParseStringToPinyinJianXie(name).ToLower();
            string py = "," + pinyinQP + "," + pinyinJX + ",";

            ////处理岗位信息.
            //DeptEmpStations des = new DeptEmpStations();
            //des.Retrieve(DeptEmpStationAttr.FK_Emp, no);

            //string depts = "";
            //string stas = "";

            //foreach (DeptEmpStation item in des)
            //{
            //    BP.GPM.Dept dept = new BP.GPM.Dept();
            //    dept.No = item.FK_Dept;
            //    if (dept.RetrieveFromDBSources() == 0)
            //    {
            //        item.Delete();
            //        continue;
            //    }

            //    //给拼音重新定义值,让其加上部门的信息.
            //    py = py + pinyinJX + "/" + BP.DA.DataType.ParseStringToPinyinJianXie(dept.Name).ToLower() + ",";

            //    BP.Port.Station sta = new Port.Station();
            //    sta.No = item.FK_Station;
            //    if (sta.RetrieveFromDBSources() == 0)
            //    {
            //        item.Delete();
            //        continue;
            //    }

            //    stas += "@" + dept.NameOfPath + "|" + sta.Name;
            //    depts += "@" + dept.NameOfPath;
            //}

            return py;
        }

        /// <summary>
        /// 向上移动
        /// </summary>
        public string DoUp()
        {
            this.DoOrderUp(EmpAttr.FK_Dept, this.FK_Dept, EmpAttr.Idx);
            return "执行成功.";
        }
        /// <summary>
        /// 向下移动
        /// </summary>
        public string DoDown()
        {
            this.DoOrderDown(EmpAttr.FK_Dept, this.FK_Dept, EmpAttr.Idx);
            return "执行成功.";
        }

        public string DoResetpassword(string pass1, string pass2)
        {
            if (pass1.Equals(pass2) == false)
                return "两次密码不一致";

            if (SystemConfig.isEnablePasswordEncryption == true)
                pass1 = BP.Tools.Cryptography.EncryptString(pass1);

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
