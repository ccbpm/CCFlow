using System;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Port
{
    /// <summary>
    /// 操作员属性
    /// </summary>
    public class EmpAttr : EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// UserID
        /// </summary>
        public const string UserID = "UserID";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
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
        public const string Token = "Token";
        /// <summary>
        /// 手机号
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// 邮箱
        /// </summary>
        public const string Email = "Email";
        /// <summary>
        /// 直属部门领导
        /// </summary>
        public const string Leader = "Leader";
        /// <summary>
        /// 部门领导编号
        /// </summary>
        public const string LeaderName = "LeaderName";
        /// <summary>
        /// 排序
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 检索拼音
        /// </summary>
        public const string PinYin = "PinYin";

        public const string EmpSta = "EmpSta";

        #endregion
    }
    /// <summary>
    /// Emp 的摘要说明。
    /// </summary>
    public class Emp : EntityNoName
    {
        #region 扩展属性
        public new string No
        {
            get
            {
                return this.GetValStringByKey(EmpAttr.No);
            }
            set
            {
                this.SetValByKey(EmpAttr.No, value);
            }
        }
        public string PinYin
        {
            get
            {
                return this.GetValStringByKey(EmpAttr.PinYin);
            }
            set
            {
                this.SetValByKey(EmpAttr.PinYin, value);
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
        /// 用户ID:SAAS模式下UserID是可以重复的.
        /// </summary>
        public string UserID
        {
            get
            {
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    return this.GetValStringByKey(EmpAttr.UserID);

                return this.GetValStringByKey(EmpAttr.No);
            }
            set
            {
                this.SetValByKey(EmpAttr.UserID, value);

                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    if (value.StartsWith(BP.Web.WebUser.OrgNo + "_") == true)
                    {
                        this.SetValByKey(EmpAttr.UserID, value.Replace(BP.Web.WebUser.OrgNo + "_", ""));
                        this.SetValByKey(EmpAttr.No, value);
                    }
                    else
                        this.SetValByKey(EmpAttr.No, BP.Web.WebUser.OrgNo + "_" + value);
                }

                else
                    this.SetValByKey(EmpAttr.No, value);
            }
        }
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(EmpAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(EmpAttr.OrgNo, value);
            }
        }

        /// <summary>
        /// 部门编号
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
        /// <summary>
        /// 部门编号
        /// </summary>
        public string FK_DeptText
        {
            get
            {
                return this.GetValRefTextByKey(EmpAttr.FK_Dept);
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
        public string Token
        {
            get
            {
                return this.GetValStrByKey(EmpAttr.Token);
            }
            set
            {
                this.SetValByKey(EmpAttr.Token, value);
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
        #endregion

        #region 公共方法
        /// <summary>
        /// 权限管理.
        /// </summary>
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
        /// 检查密码(可以重写此方法)
        /// </summary>
        /// <param name="pass">密码</param>
        /// <returns>是否匹配成功</returns>
        public bool CheckPass(string pass)
        {
            //检查是否与通用密码相符.
            //string gePass =  BP.Difference.SystemConfig.AppSettings["GenerPass"];
            //if (gePass == pass && DataType.IsNullOrEmpty(gePass) == false)
            //    return true;

            //启用加密
            if (BP.Difference.SystemConfig.IsEnablePasswordEncryption == true)
            {
                //pass = BP.Tools.Cryptography.EncryptString(pass);
                ///*使用数据库校验.*/
                //if (this.Pass == pass)
                //    return true;

                pass = BP.Tools.Cryptography.MD5_Encrypt(pass);
                /*使用数据库校验.*/
                if (this.Pass == pass)
                    return true;
            }

            if (this.Pass.Equals(pass) == true)
                return true;
            return false;
        }

        private static byte[] Keys = { 0x12, 0xCD, 0x3F, 0x34, 0x78, 0x90, 0x56, 0x7B };

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static string EncryptPass(string pass)
        {
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象   

            byte[] data = Encoding.Unicode.GetBytes(pass);//定义字节数组，用来存储要加密的字符串  
            MemoryStream MStream = new MemoryStream(); //实例化内存流对象      
            //使用内存流实例化加密流对象   
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(Keys, Keys), CryptoStreamMode.Write);
            CStream.Write(data, 0, data.Length);  //向加密流中写入数据      
            CStream.FlushFinalBlock();              //释放加密流      
            return Convert.ToBase64String(MStream.ToArray());//返回加密后的字符串  
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static string DecryptPass(string pass)
        {
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象    
            byte[] data = Convert.FromBase64String(pass);//定义字节数组，用来存储要解密的字符串  
            MemoryStream MStream = new MemoryStream(); //实例化内存流对象      
            //使用内存流实例化解密流对象       
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(Keys, Keys), CryptoStreamMode.Write);
            CStream.Write(data, 0, data.Length);      //向解密流中写入数据     
            CStream.FlushFinalBlock();               //释放解密流      
            return Encoding.Unicode.GetString(MStream.ToArray());       //返回解密后的字符串 
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
        /// <param name="userID">编号</param>
        public Emp(string userID)
        {
            if (userID == null || userID.Length == 0)
                throw new Exception("@要查询的操作员编号为空。");

            userID = userID.Trim();
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                if (userID.Equals("admin") == true)
                    this.SetValByKey("No", userID);
                else
                    this.SetValByKey("No", BP.Web.WebUser.OrgNo + "_" + userID);
            }
            else
            {
                this.SetValByKey("No", userID);
            }
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

                Map map = new Map("Port_Emp", "用户");

                #region 基本属性
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN); //要连接的数据源（表示要连接到的那个系统数据库）。
                map.IndexField = EmpAttr.FK_Dept;
                #endregion

                #region 字段
                /* 关于字段属性的增加 .. */
                //map.IsEnableVer = true;
                map.AddTBStringPK(EmpAttr.No, null, "编号", true, false, 1, 50, 30);

                //如果是集团模式或者是SAAS模式.
                if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    map.AddTBString(EmpAttr.UserID, null, "用户ID", true, false, 0, 50, 30);

                map.AddTBString(EmpAttr.Name, null, "名称", true, false, 0, 200, 30);
                map.AddTBString(EmpAttr.PinYin, null, "拼音", false, false, 0, 200, 30);
                map.AddTBString(EmpAttr.Pass, "123", "密码", false, false, 0, 20, 10);

                map.AddDDLEntities(EmpAttr.FK_Dept, null, "部门", new BP.Port.Depts(), false);
                map.AddTBString(EmpAttr.Token, null, "Token", false, false, 0, 36, 36);
                map.AddTBString(EmpAttr.Tel, null, "手机号", false, false, 0, 36, 36);
                map.AddTBString(EmpAttr.Email, null, "邮箱", false, false, 0, 36, 36);

                map.AddTBString(EmpAttr.Leader, null, "直属部门领导", false, false, 0, 20, 130);
                map.SetHelperAlert(EmpAttr.Leader, "这里是领导的登录帐号，不是中文名字，用于流程的接受人规则中。");
                map.AddTBString(EmpAttr.LeaderName, null, "直属部门领导", true, true, 0, 20, 130);

                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                    map.AddTBString(EmpAttr.OrgNo, null, "组织编号", false, false, 0, 50, 50);
                else
                    map.AddTBString(EmpAttr.OrgNo, null, "组织编号", true, false, 0, 50, 50);

                map.AddTBInt(EmpAttr.EmpSta, 0, "EmpSta", false, false);
                map.AddTBInt(EmpAttr.Idx, 0, "Idx", false, false);
                #endregion 字段

                map.AddSearchAttr(EmpAttr.FK_Dept);

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
                map.AttrsOfOneVSM.AddBranches(new DeptEmps(), new BP.Port.Depts(),
                   BP.Port.DeptEmpAttr.FK_Emp,
                   BP.Port.DeptEmpAttr.FK_Dept, "部门维护", EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");

                //用户组
                map.AttrsOfOneVSM.Add(new TeamEmps(), new Teams(), TeamEmpAttr.FK_Emp, TeamEmpAttr.FK_Team,
                    TeamAttr.Name, TeamAttr.No, "用户组", Dot2DotModel.Default);

                rm = new RefMethod();
                rm.Title = "修改密码";
                rm.ClassMethodName = this.ToString() + ".DoResetpassword";
                rm.HisAttrs.AddTBString("pass1", null, "输入密码", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("pass2", null, "再次输入", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "修改主部门";
                rm.ClassMethodName = this.ToString() + ".DoEditMainDept";
                rm.RefAttrKey = EmpAttr.FK_Dept;
                rm.RefMethodType = RefMethodType.LinkModel;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置部门直属领导";
                rm.ClassMethodName = this.ToString() + ".DoEditLeader";
                rm.RefAttrKey = EmpAttr.LeaderName;
                rm.RefMethodType = RefMethodType.LinkModel;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }

        public string DoEditMainDept()
        {
            return BP.Difference.SystemConfig.CCFlowWebPath + "GPM/EmpDeptMainDept.htm?FK_Emp=" + this.No + "&FK_Dept=" + this.FK_Dept;
        }

        public string DoEditLeader()
        {
            return BP.Difference.SystemConfig.CCFlowWebPath + "GPM/EmpLeader.htm?FK_Emp=" + this.No + "&FK_Dept=" + this.FK_Dept;
        }

        public string DoEmpDepts()
        {
            return BP.Difference.SystemConfig.CCFlowWebPath + "GPM/EmpDepts.htm?FK_Emp=" + this.No;
        }

        public string DoSinger()
        {
            //路径
            return BP.Difference.SystemConfig.CCFlowWebPath + "GPM/Siganture.htm?EmpNo=" + this.No;
        }
        protected override bool beforeInsert()
        {
            if (BP.Difference.SystemConfig.IsEnablePasswordEncryption == true)
                this.Pass = BP.Tools.Cryptography.EncryptString(this.Pass);
            return base.beforeInsert();
        }

        protected override bool beforeUpdateInsertAction()
        {
            //增加拼音，以方便查找.
            if (DataType.IsNullOrEmpty(this.Name) == true)
                throw new Exception("err@名称不能为空.");

            if (BP.Web.WebUser.IsAdmin == false)
                throw new Exception("err@非管理员无法操作.");

            if (DataType.IsNullOrEmpty(this.Email) == false)
            {
                //邮箱格式
                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
                if (r.IsMatch(this.Email) == false)
                    throw new Exception("邮箱格式不正确.");
            }

            string pinyinQP = DataType.ParseStringToPinyin(this.Name).ToLower();
            string pinyinJX = DataType.ParseStringToPinyinJianXie(this.Name).ToLower();
            this.PinYin = "," + pinyinQP + "," + pinyinJX + ",";

            //处理岗位信息.
            DeptEmpStations des = new DeptEmpStations();
            des.Retrieve(DeptEmpStationAttr.FK_Emp, this.No);

            string depts = "";
            string stas = "";

            foreach (DeptEmpStation item in des)
            {
                Dept dept = new Dept();
                dept.No = item.FK_Dept;
                if (dept.RetrieveFromDBSources() == 0)
                {
                    item.Delete();
                    continue;
                }

                //给拼音重新定义值,让其加上部门的信息.
                this.PinYin = this.PinYin + pinyinJX + "/" + DataType.ParseStringToPinyinJianXie(dept.Name).ToLower() + ",";

                BP.Port.Station sta = new BP.Port.Station();
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
        protected override bool beforeDelete()
        {
            if (this.No.ToLower().Equals("admin") == true)
                throw new Exception("err@管理员账号不能删除.");

            return base.beforeDelete();
        }

        /// <summary>
        /// 保存后修改WF_Emp中的邮箱
        /// </summary>
        protected override void afterInsertUpdateAction()
        {
            string sql = "Select Count(*) From WF_Emp Where No='" + this.No + "'";
            int count = DBAccess.RunSQLReturnValInt(sql);
            if (count == 0)
                sql = "INSERT INTO WF_Emp (No,Name,Email) VALUES('" + this.No + "','" + this.Name + "','" + this.Email + "')";
            else
                sql = "UPDATE WF_Emp SET Email='" + this.Email + "'";
            DBAccess.RunSQL(sql);

            DeptEmp deptEmp = new DeptEmp();
            deptEmp.FK_Dept = this.FK_Dept;
            deptEmp.FK_Emp = this.No;
            deptEmp.setMyPK(this.FK_Dept + "_" + this.No);
            if (deptEmp.IsExit("MyPK", deptEmp.MyPK) == false)
                deptEmp.Insert();


            base.afterInsertUpdateAction();
        }
        /// <summary>
        /// 删除之后要做的事情
        /// </summary>
        protected override void afterDelete()
        {
            DeptEmps des = new DeptEmps();
            des.Delete(DeptEmpAttr.FK_Emp, this.No);

            DeptEmpStations stas = new DeptEmpStations();
            stas.Delete(DeptEmpAttr.FK_Emp, this.No);

            base.afterDelete();
        }

        public static string GenerPinYin(string no, string name)
        {
            //增加拼音，以方便查找.
            string pinyinQP = DataType.ParseStringToPinyin(name).ToLower();
            string pinyinJX = DataType.ParseStringToPinyinJianXie(name).ToLower();
            string py = "," + pinyinQP + "," + pinyinJX + ",";

            //处理岗位信息.
            DeptEmpStations des = new DeptEmpStations();
            des.Retrieve(DeptEmpStationAttr.FK_Emp, no);

            string depts = "";
            string stas = "";

            foreach (DeptEmpStation item in des)
            {
                Dept dept = new Dept();
                dept.No = item.FK_Dept;
                if (dept.RetrieveFromDBSources() == 0)
                {
                    item.Delete();
                    continue;
                }

                //给拼音重新定义值,让其加上部门的信息.
                py = py + pinyinJX + "/" + DataType.ParseStringToPinyinJianXie(dept.Name).ToLower() + ",";

                BP.Port.Station sta = new BP.Port.Station();
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

            if (BP.Difference.SystemConfig.IsEnablePasswordEncryption == true)
                this.Pass = BP.Tools.Cryptography.EncryptString(this.Pass);

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


        #region 方法测试代码.
        /// <summary>
        /// 禁用、启用用户
        /// </summary>
        /// <returns></returns>
        public string DoUnEnable()
        {
            string userNo = this.No;
            //判断当前人员是否有待办
            string wfSql = "";
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                wfSql = " AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";
                userNo = this.UserID;
            }
            string sql = "";
            /*不是授权状态*/
            if (BP.Difference.SystemConfig.GetValByKeyBoolen("IsEnableTaskPool", false) == true)
                sql = "SELECT count(WorkID) as Num FROM WF_EmpWorks WHERE FK_Emp='" + userNo + "' AND TaskSta!=1 " + wfSql;
            else
                sql = "SELECT count(WorkID) as Num FROM WF_EmpWorks WHERE  FK_Emp='" + userNo + "' " + wfSql;

            int count = DBAccess.RunSQLReturnValInt(sql);
            if (count != 0)
                return this.Name + "还存在待办，不能禁用该用户";
            sql = "UPDATE WF_Emp SET UseSta=0 WHERE No='" + this.No + "'";
            DBAccess.RunSQL(sql);
            //this.Delete();
            this.EmpSta = 1;
            this.Update();
            return this.Name + "已禁用";

        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        public string ResetPass()
        {
            return "执行成功.";
        }
        /// <summary>
        /// ChangePass
        /// </summary>
        /// <param name="oldpass"></param>
        /// <param name="pass1"></param>
        /// <param name="pass2"></param>
        /// <returns></returns>
        public string ChangePass(string oldpass, string pass1, string pass2)
        {
            if (BP.Web.WebUser.No.Equals(this.UserID) == false)
                return "err@sss";

            return "执行成功.";
        }

        #endregion 方法测试代码.

    }
    /// <summary>
    /// 操作员
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
        /// <summary>
        /// 操作员s
        /// </summary>
        public Emps(string deptNo)
        {

            this.Retrieve(EmpAttr.FK_Dept, deptNo);

        }
        #endregion 构造方法

        public string reseet()
        {
            return "ceshi";
        }

        #region 重写查询,add by zhoupeng 2015.09.30 为了适应能够从 webservice 数据源查询数据.
        /// <summary>
        /// 重写查询全部适应从WS取数据需要
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            //if (BP.Web.WebUser.No != "admin")
            //    throw new Exception("@您没有查询的权限.");


            return base.RetrieveAll();

        }
        /// <summary>
        /// 重写重数据源查询全部适应从WS取数据需要
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAllFromDBSource()
        {

            return base.RetrieveAllFromDBSource();

        }
        #endregion 重写查询.

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
