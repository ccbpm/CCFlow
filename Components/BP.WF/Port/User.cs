using System;
using System.Text.RegularExpressions;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.WF.Port
{
    /// <summary>
    /// 操作员 属性
    /// </summary>
    public class UserAttr : BP.En.EntityNoNameAttr
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
        #endregion

        #region 组织属性.
        /// <summary>
        /// 组织结构编码
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 组织名称
        /// </summary>
        public const string OrgName = "OrgName";
        /// <summary>
        /// 微信ID
        /// </summary>
        public const string unionid = "unionid";
        public const string OpenID = "OpenID";
        public const string OpenID2 = "OpenID2";
        #endregion 组织属性.

        #region 权限.
        /// <summary>
        /// 组织结构编码
        /// </summary>
        public const string IsSaller = "IsSaller";
        #endregion 组织属性.
    }
    /// <summary>
    /// 操作员 的摘要说明。
    /// </summary>
    public class User : EntityNoName
    {
        #region 扩展属性
        
            public string unionid
        {
            get
            {
                return this.GetValStrByKey(UserAttr.unionid);
            }
            set
            {
                this.SetValByKey(UserAttr.unionid, value);
            }
        }
        public string OpenID
        {
            get
            {
                return this.GetValStrByKey(UserAttr.OpenID);
            }
            set
            {
                this.SetValByKey(UserAttr.OpenID, value);
            }
        }
        public string OpenID2
        {
            get
            {
                return this.GetValStrByKey(UserAttr.OpenID2);
            }
            set
            {
                this.SetValByKey(UserAttr.OpenID2, value);
            }
        }
        
        public string SID
        {
            get
            {
                return this.GetValStrByKey(UserAttr.SID);
            }
            set
            {
                this.SetValByKey(UserAttr.SID, value);
            }
        }
        /// <summary>
        /// 拼音
        /// </summary>
        public string PinYin
        {
            get
            {
                return this.GetValStrByKey(UserAttr.PinYin);
            }
            set
            {
                this.SetValByKey(UserAttr.PinYin, value);
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
                return this.GetValStrByKey(UserAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(UserAttr.FK_Dept, value);
            }
        }
        public string FK_DeptText
        {
            get
            {
                return this.GetValRefTextByKey(UserAttr.FK_Dept);
            }
        }
        public string Tel
        {
            get
            {
                return this.GetValStrByKey(UserAttr.Tel);
            }
            set
            {
                this.SetValByKey(UserAttr.Tel, value);
            }
        }
        public string Email
        {
            get
            {
                return this.GetValStrByKey(UserAttr.Email);
            }
            set
            {
                this.SetValByKey(UserAttr.Email, value);
            }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string Pass
        {
            get
            {
                return this.GetValStrByKey(UserAttr.Pass);
            }
            set
            {
                this.SetValByKey(UserAttr.Pass, value);
            }
        } 
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(UserAttr.Idx);
            }
            set
            {
                this.SetValByKey(UserAttr.Idx, value);
            }
        }
        /// <summary>
        /// 组织结构编码
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(UserAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(UserAttr.OrgNo, value);
            }
        }
        public string OrgName
        {
            get
            {
                return this.GetValStrByKey(UserAttr.OrgName);
            }
            set
            {
                this.SetValByKey(UserAttr.OrgName, value);
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
            if (SystemConfig.IsEnablePasswordEncryption == true)
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
        public User()
        {
        }
        /// <summary>
        /// 操作员
        /// </summary>
        /// <param name="no">编号</param>
        public User(string no)
        {
            try
            {
                this.No = no;
                this.Retrieve();
            }
            catch (Exception ex)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("err@用户账号[" + this.No + "]错误:" + ex.Message);
                throw ex;
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

                Map map = new Map("Port_User", "用户");
              // map.setEnType(EnType.App);
                map.IndexField = UserAttr.FK_Dept;

                #region 字段。
                /*关于字段属性的增加 */
                map.AddTBStringPK(UserAttr.No, null, "手机号/ID", true, false, 1, 150, 90);
                map.AddTBString(UserAttr.Name, null, "姓名", true, false, 0, 500, 130);
                map.AddTBString(UserAttr.Pass, null, "密码", false, false, 0, 100, 10);
                map.AddTBString(UserAttr.FK_Dept, null, "部门", false, false, 0, 100, 10);
                map.AddTBString(UserAttr.SID, null, "SID", false, false, 0, 36, 36);
                map.AddTBString(UserAttr.Tel, null, "电话", true, false, 0, 20, 130);
                map.AddTBString(UserAttr.Email, null, "邮箱", true, false, 0, 100, 132, true);
                map.AddTBString(UserAttr.PinYin, null, "拼音", true, false, 0, 1000, 132, true);

                map.AddTBString(UserAttr.OrgNo, null, "OrgNo", true, false, 0, 500, 132, true);
                map.AddTBString(UserAttr.OrgName, null, "OrgName", true, false, 0, 500, 132, true);
                map.AddTBString(UserAttr.unionid, null, "unionid", true, false, 0, 500, 132, true);
                map.AddTBString(UserAttr.OpenID, null, "小程序的OpenID", true, false, 0, 500, 132, true);
                map.AddTBString(UserAttr.OpenID2, null, "公众号的OpenID", true, false, 0, 500, 132, true);

                map.AddTBInt(UserAttr.Idx, 0, "序号", true, false);
                #endregion 字段

                this._enMap = map;
                return this._enMap;
            }
        }

         
        protected override bool beforeInsert()
        {
            if (SystemConfig.IsEnablePasswordEncryption == true)
            {
                if (this.Pass == "")
                {
                    this.Pass = "123";
                }
                this.Pass = BP.Tools.Cryptography.EncryptString(this.Pass);
            }
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
            return base.beforeUpdateInsertAction();
        }
        protected override bool beforeDelete()
        {
            if (this.OrgNo != BP.Web.WebUser.OrgNo)
                throw new Exception("err@您不能删除别人的数据.");

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
            //DeptUserStations des = new DeptUserStations();
            //des.Retrieve(DeptUserStationAttr.FK_User, no);

            //string depts = "";
            //string stas = "";

            //foreach (DeptUserStation item in des)
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

            //    BP.Port.Station sta = new BP.Port.Station();
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
            this.DoOrderUp(UserAttr.FK_Dept, this.FK_Dept, UserAttr.Idx);
            return "执行成功.";
        }
        /// <summary>
        /// 向下移动
        /// </summary>
        public string DoDown()
        {
            this.DoOrderDown(UserAttr.FK_Dept, this.FK_Dept, UserAttr.Idx);
            return "执行成功.";
        }

        public string DoResetpassword(string pass1, string pass2)
        {
            if (pass1.Equals(pass2) == false)
                return "两次密码不一致";

            if (SystemConfig.IsEnablePasswordEncryption == true)
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
            get { return new Users(); }
        }
        #endregion 构造函数
    }
    /// <summary>
    /// 操作员s
    // </summary>
    public class Users : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new User();
            }
        }
        /// <summary>
        /// 操作员s
        /// </summary>
        public Users()
        {
        }

        #endregion 构造方法

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<User> ToJavaList()
        {
            return (System.Collections.Generic.IList<User>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<User> Tolist()
        {
            System.Collections.Generic.List<User> list = new System.Collections.Generic.List<User>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((User)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
