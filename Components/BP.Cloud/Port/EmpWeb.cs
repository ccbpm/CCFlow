using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Port;
using BP.En30.Utility.Web;
using BP.Cloud.HttpHandler;
using BP.Difference;

namespace BP.Cloud
{
    /// <summary>
    /// 操作员 属性
    /// </summary>
    public class EmpWebAttr : BP.En.EntityNoNameAttr
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
        /// <summary>
        /// 人员状态
        /// </summary>
        public const string EmpSta = "EmpSta";
        public const string Sex = "Sex";
    }
    /// <summary>
    /// 操作员 的摘要说明。
    /// </summary>
    public class EmpWeb : EntityNoName
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

                string sql = "SELECT COUNT(FK_EmpWeb) FROM Port_DeptEmpWebStation WHERE FK_EmpWeb='" + this.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                    return false;

                sql = "SELECT COUNT(FK_EmpWeb) FROM Port_DeptEmpWeb WHERE FK_EmpWeb='" + this.No + "'";
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
                return this.GetValStrByKey(EmpWebAttr.UserID);
            }
            set
            {
                this.SetValByKey(EmpWebAttr.UserID, value);
            }
        }
        public string SID
        {
            get
            {
                return this.GetValStrByKey(EmpWebAttr.SID);
            }
            set
            {
                this.SetValByKey(EmpWebAttr.SID, value);
            }
        }
        /// <summary>
        /// 拼音
        /// </summary>
        public string PinYin
        {
            get
            {
                return this.GetValStrByKey(EmpWebAttr.PinYin);
            }
            set
            {
                this.SetValByKey(EmpWebAttr.PinYin, value);
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
                return this.GetValStrByKey(EmpWebAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(EmpWebAttr.FK_Dept, value);
            }
        }
        public string FK_DeptText
        {
            get
            {
                return this.GetValRefTextByKey(EmpWebAttr.FK_Dept);
            }
        }
        public string Tel
        {
            get
            {
                return this.GetValStrByKey(EmpWebAttr.Tel);
            }
            set
            {
                this.SetValByKey(EmpWebAttr.Tel, value);
            }
        }
        public string Email
        {
            get
            {
                return this.GetValStrByKey(EmpWebAttr.Email);
            }
            set
            {
                this.SetValByKey(EmpWebAttr.Email, value);
            }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string Pass
        {
            get
            {
                return this.GetValStrByKey(EmpWebAttr.Pass);
            }
            set
            {
                this.SetValByKey(EmpWebAttr.Pass, value);
            }
        }
        public string QQ
        {
            get
            {
                return this.GetValStrByKey(EmpWebAttr.QQ);
            }
            set
            {
                this.SetValByKey(EmpWebAttr.QQ, value);
            }
        }
        public string QQAppID
        {
            get
            {
                return this.GetValStringByKey(EmpWebAttr.QQAppID);
            }
            set
            {
                this.SetValByKey(EmpWebAttr.QQAppID, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(EmpWebAttr.Idx);
            }
            set
            {
                this.SetValByKey(EmpWebAttr.Idx, value);
            }
        }
        /// <summary>
        /// 组织结构编码
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(EmpWebAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(EmpWebAttr.OrgNo, value);
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
        public EmpWeb()
        {
        }
        /// <summary>
        /// 操作员
        /// </summary>
        /// <param name="no">编号</param>
        public EmpWeb(string no)
        {
            try
            {
                //如果小于11位估计不是一个手机号.
                if (BP.DA.DataType.IsMobile(no) == false)
                    this.No = BP.Web.WebUser.OrgNo + "_" + no;
                else
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
                {
                    uac.IsDelete = false;
                    uac.IsInsert = false;
                    uac.IsUpdate = true;
                    uac.IsView = true;

                }
                else
                {
                    uac.Readonly();
                    uac.IsView = false;
                }
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
                map.IndexField = EmpWebAttr.FK_Dept;

                #region 字段
                /*关于字段属性的增加 */
                map.AddTBStringPK(EmpAttr.No, null, "登陆账号", false, false, 1, 50, 90);
                map.AddTBString(EmpAttr.UserID, null, "用户ID", true, true, 0, 100, 10);
                map.AddTBString(EmpAttr.Name, null, "名称", true, false, 0, 200, 130);

                //密码.
                map.AddTBString(EmpAttr.Pass, null, "Pass", false, false, 0, 200, 130);

                map.AddDDLEntities(EmpAttr.FK_Dept, null, "主部门", new BP.Port.Depts(), false);

                map.AddTBString(EmpAttr.SID, null, "安全校验码", false, false, 0, 36, 36);
                map.AddTBString(EmpAttr.Tel, null, "电话", true, false, 0, 20, 130);
                map.AddTBString(EmpAttr.Email, null, "邮箱", true, false, 0, 100, 132, true);
                map.AddTBString(EmpAttr.PinYin, null, "拼音", false, false, 0, 500, 132, true);
               
               // map.AddDDLSysEnum(EmpWebAttr.Sex, 1, "性别", true, true, "XBXT", "@1=男@2=女");
                map.AddTBString(EmpAttr.Leader, null, "直属主管", true, false, 0, 500, 132, false);

                //状态.
                map.AddDDLSysEnum(EmpWebAttr.EmpSta, 0, "状态", false, false, EmpWebAttr.EmpSta,
                "@0=正常@1=禁止登陆");

                //// 0=不签名 1=图片签名, 2=电子签名.
                //map.AddDDLSysEnum(EmpAttr.SignType, 0, "签字类型", true, true, EmpAttr.SignType,
                //    "@0=不签名@1=图片签名@2=电子签名");
                map.AddTBString(EmpAttr.OrgNo, null, "组织编号", true, true, 0, 50, 50);

                map.AddTBInt(EmpAttr.Idx, 0, "序号", false, false);
                #endregion 字段

                //字段查询.
                map.AddSearchAttr(EmpAttr.FK_Dept);


                //增加隐藏查询条件.
                if (BP.Web.WebUser.No.Equals("admin") == true)
                {
                    map.AddDDLEntities(EmpAttr.OrgNo, null, "组织", new BP.Cloud.Orgs(), false);
                }
                else
                {
                    map.AddTBString(EmpAttr.OrgNo, null, "OrgNo", false, false, 0, 36, 36);
                    map.AddHidden(StationTypeAttr.OrgNo, "=", BP.Web.WebUser.OrgNo);
                }


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
                map.AttrsOfOneVSM.AddBranches(new BP.Cloud.DeptEmps(), new BP.Cloud.Depts(),
                   BP.Cloud.DeptEmpAttr.EmpNo,
                   BP.Cloud.DeptEmpAttr.FK_Dept, "部门维护", EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");

                ////用户组
                //map.AttrsOfOneVSM.Add(new TeamEmps(), new Teams(), TeamEmpAttr.FK_Emp, TeamEmpAttr.FK_Team,
                //    TeamAttr.Name, TeamAttr.No, "用户组", Dot2DotModel.Default);

                rm = new RefMethod();
                rm.Title = "修改密码";
                rm.ClassMethodName = this.ToString() + ".DoResetpassword";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.HisAttrs.AddTBString("pass1", null, "输入密码", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("pass2", null, "再次输入", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "修改主部门";
                //rm.ClassMethodName = this.ToString() + ".DoEditMainDept";
                //rm.RefAttrKey = EmpAttr.FK_Dept;
                //rm.RefMethodType = RefMethodType.LinkModel;
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }

        public string DoEditMainDept()
        {
            return "/App/Org/EmpDeptMainDept.htm?FK_Emp=" + this.No + "&FK_Dept=" + this.FK_Dept + "&UserID=" + this.UserID;
        }
        public string DoEmpDepts()
        {
            return "/App/Org/EmpDepts.htm?FK_Emp=" + this.No;
        }
        public string DoSinger()
        {
            //路径
            return "/App/Org/Siganture.htm?EmpNo=" + this.UserID+"&OrgNo="+this.OrgNo;
        }
        protected override bool beforeInsert()
        {
            if (this.OrgNo != BP.Web.WebUser.OrgNo)
                throw new Exception("err@没有权限.");

            string pass1 = "123";
            if (SystemConfig.isEnablePasswordEncryption == true)
                pass1 = BP.Tools.Cryptography.EncryptString(pass1);
            this.Pass = pass1;
            BP.Sys.Base.Glo.WriteUserLog("新增人员:" + this.ToJson(), "岗位数据操作");
            return base.beforeInsert();
        }
        protected override bool beforeUpdateInsertAction()
        {
            if (this.OrgNo != BP.Web.WebUser.OrgNo)
                throw new Exception("err@没有权限.");
            //增加拼音，以方便查找.
            if (DataType.IsNullOrEmpty(this.Name) == true)
                throw new Exception("err@名称不能为空.");

            string pinyinQP = BP.DA.DataType.ParseStringToPinyin(this.Name).ToLower();
            string pinyinJX = BP.DA.DataType.ParseStringToPinyinJianXie(this.Name).ToLower();
            this.PinYin = "," + pinyinQP + "," + pinyinJX + ",";


            ////处理岗位信息.
            //DeptEmpWebStations des = new DeptEmpWebStations();
            //des.Retrieve(DeptEmpWebStationAttr.FK_EmpWeb, this.No);

            //string depts = "";
            //string stas = "";

            //foreach (DeptEmpWebStation item in des)
            //{
            //    BP.GPM.Dept dept = new BP.GPM.Dept();
            //    dept.No = item.FK_Dept;
            //    if (dept.RetrieveFromDBSources() == 0)
            //    {
            //        item.Delete();
            //        continue;D:\CCFlowCloud\BP.Cloud\Port\EmpWeb.cs
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
            BP.Sys.Base.Glo.WriteUserLog("新增/修改人员:" + this.ToJson(), "岗位数据操作");
            return base.beforeUpdateInsertAction();
        }

        protected override void afterInsertUpdateAction()
        {
            base.afterInsertUpdateAction();
        }
        protected override bool beforeUpdate()
        {
            return base.beforeUpdate();
        }
        protected override void afterUpdate()
        {

            base.afterUpdate();
        }
        protected override void afterDelete()
        {
            DeptEmps des = new DeptEmps();
            des.Retrieve(DeptEmpAttr.FK_Emp, this.UserID, DeptEmpAttr.OrgNo, this.OrgNo);
            des.Delete();

            DeptEmpStations ensD = new DeptEmpStations();
            ensD.Delete(DeptEmpAttr.FK_Emp, this.UserID, DeptEmpAttr.OrgNo, this.OrgNo);

            base.afterDelete();
        }
        protected override bool beforeDelete()
        {
            if (this.OrgNo != BP.Web.WebUser.OrgNo)
                throw new Exception("err@没有权限.");

            DeptEmps des = new DeptEmps();
            des.Retrieve(DeptEmpAttr.FK_Emp, this.UserID, DeptEmpAttr.OrgNo, this.OrgNo);
            des.Delete();

            DeptEmpStations ensD = new DeptEmpStations();
            ensD.Delete(DeptEmpAttr.FK_Emp, this.UserID, DeptEmpAttr.OrgNo, this.OrgNo);
            BP.Sys.Base.Glo.WriteUserLog("删除人员:" + this.ToJson(), "岗位数据操作");
            return base.beforeDelete();
        }

        public static string GenerPinYin(string no, string name)
        {
            //增加拼音，以方便查找.
            string pinyinQP = BP.DA.DataType.ParseStringToPinyin(name).ToLower();
            string pinyinJX = BP.DA.DataType.ParseStringToPinyinJianXie(name).ToLower();
            string py = "," + pinyinQP + "," + pinyinJX + ",";
            return py;
        }

        /// <summary>
        /// 向上移动
        /// </summary>
        public string DoUp()
        {
            if (this.OrgNo != BP.Web.WebUser.OrgNo)
                throw new Exception("err@没有权限.");

            this.DoOrderUp(EmpWebAttr.FK_Dept, this.FK_Dept, EmpWebAttr.Idx);
            return "执行成功.";
        }
        /// <summary>
        /// 向下移动
        /// </summary>
        public string DoDown()
        {
            if (this.OrgNo != BP.Web.WebUser.OrgNo)
                throw new Exception("err@没有权限.");

            this.DoOrderDown(EmpWebAttr.FK_Dept, this.FK_Dept, EmpWebAttr.Idx);
            return "执行成功.";
        }

        public string DoResetpassword(string pass1, string pass2)
        {
            if (this.OrgNo != BP.Web.WebUser.OrgNo)
                throw new Exception("err@没有权限.");

            if (pass1.Equals(pass2) == false)
                return "两次密码不一致";

            if (SystemConfig.isEnablePasswordEncryption == true)
                pass1 = BP.Tools.Cryptography.EncryptString(pass1);

            this.Pass = pass1;
            this.Update("Pass", pass1);
            return "密码设置成功";
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new EmpWebs(); }
        }
        #endregion 构造函数
    }
    /// <summary>
    /// 操作员s
    // </summary>
    public class EmpWebs : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EmpWeb();
            }
        }
        /// <summary>
        /// 操作员s
        /// </summary>
        public EmpWebs()
        {
        }

        #endregion 构造方法

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EmpWeb> ToJavaList()
        {
            return (System.Collections.Generic.IList<EmpWeb>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EmpWeb> Tolist()
        {
            System.Collections.Generic.List<EmpWeb> list = new System.Collections.Generic.List<EmpWeb>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EmpWeb)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
