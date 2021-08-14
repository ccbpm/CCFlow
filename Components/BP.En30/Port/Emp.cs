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
        public const string SID = "SID";
        /// <summary>
        /// 手机号
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// 邮箱
        /// </summary>
        public const string Email = "Email";

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
                return this.GetValStringByKey(EntityNoNameAttr.No);
            }
            set
            {
                this.SetValByKey(EmpAttr.No, value);
            }
        }
        /// <summary>
        /// 用户ID:SAAS模式下UserID是可以重复的.
        /// </summary>
        public string UserID
        {
            get
            {
                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    return this.GetValStringByKey(EmpAttr.UserID);

                return this.GetValStringByKey(EmpAttr.No);
            }
            set
            {
                this.SetValByKey(EmpAttr.UserID, value);

                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    this.SetValByKey(EmpAttr.No, BP.Web.WebUser.OrgNo + "_" + value);
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
                    throw new Exception("@获取操作员" + this.UserID + "部门[" + this.FK_Dept + "]出现错误,可能是系统管理员没有给他维护部门.@" + ex.Message);
                }
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
            //string gePass = SystemConfig.AppSettings["GenerPass"];
            //if (gePass == pass && DataType.IsNullOrEmpty(gePass) == false)
            //    return true;

            //启用加密
            if (SystemConfig.IsEnablePasswordEncryption == true)
            {
                pass = BP.Tools.Cryptography.EncryptString(pass);
                /*使用数据库校验.*/
                if (this.Pass == pass)
                    return true;

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
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
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
                if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    map.AddTBString(EmpAttr.UserID, null, "用户ID", true, false, 0, 50, 30);

                map.AddTBString(EmpAttr.OrgNo, null, "OrgNo", true, false, 0, 50, 30);
                map.AddTBString(EmpAttr.Name, null, "名称", true, false, 0, 200, 30);
                map.AddTBString(EmpAttr.Pass, "123", "密码", false, false, 0, 20, 10);
                map.AddDDLEntities(EmpAttr.FK_Dept, null, "部门", new Port.Depts(), true);
                map.AddTBString(EmpAttr.SID, null, "安全校验码", false, false, 0, 36, 36);
                map.AddTBString(EmpAttr.Tel, null, "手机号", false, false, 0, 36, 36);
                map.AddTBString(EmpAttr.Email, null, "邮箱", false, false, 0, 36, 36);

                // map.AddTBString("docs", null, "安全校33验码", false, false, 0, 4000, 36);
                #endregion 字段

                map.AddSearchAttr(EmpAttr.FK_Dept);


                this._enMap = map;
                return this._enMap;
            }

        }
        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new Emps(); }
        }
        #endregion 构造函数

        #region 重写方法
        protected override bool beforeDelete()
        {
            if (this.No.Equals("admin") == true)
                throw new Exception("err@管理员账号不能删除.");

            return base.beforeDelete();
        }
        #endregion 重写方法

        #region 重写查询.
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public override int Retrieve()
        {

            return base.Retrieve();
        }
        /// <summary>
        /// 查询.
        /// </summary>
        /// <returns></returns>
        public override int RetrieveFromDBSources()
        {

            return base.RetrieveFromDBSources();

        }
        #endregion


        #region 方法测试代码.
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
