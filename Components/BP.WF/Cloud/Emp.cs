using System;
using System.Text.RegularExpressions;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Port;
using BP.Port.WeiXin;

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
        /// 组织结构编码
        /// </summary>
        public const string OrgNo = "OrgNo";
        public const string OrgName = "OrgName";
        /// <summary>
        /// 连接的ID
        /// </summary>
        public const string unionid = "unionid";
        public const string EmpSta = "EmpSta";
        public const string OpenID = "OpenID";
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
        public bool ItIsEnable
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
        /// 部门
        /// </summary>
        public string DeptNo
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
        public string DeptText
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

        #region 构造函数
        /// <summary>
        /// 操作员
        /// </summary>
        public Emp()
        {
        }
        /// <summary>
        /// 初始化人员
        /// </summary>
        /// <param name="orgNo"></param>
        /// <param name="userID"></param>
        public Emp(string orgNo, string userID)
        {
            int i = this.Retrieve(EmpAttr.OrgNo, orgNo, EmpAttr.UserID, userID);
            if (i == 0)
                throw new Exception("@组织编号:" + orgNo + " 或者UserID:" + UserID + "错误.");
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
                uac.IsInsert = false; //@hongyan.
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
                map.setEnType(EnType.App);
                map.IndexField = EmpAttr.FK_Dept;

                #region 字段
                /*关于字段属性的增加 */
                map.AddTBStringPK(EmpAttr.No, null, "手机号/ID", false, false, 1, 500, 90);
                map.AddTBString(EmpAttr.UserID, null, "登陆ID", true, true, 0, 100, 10);
                map.AddTBString(EmpAttr.Name, null, "姓名", true, false, 0, 500, 130);
                map.AddTBString(EmpAttr.FK_Dept, null, "当前登录的部门", true, false, 0, 500, 130);

                //状态. 0=启用，1=禁用.
                map.AddTBInt(EmpAttr.EmpSta, 0, "EmpSta", false, false);
                map.AddTBString(EmpAttr.Leader, null, "部门领导", false, false, 0, 100, 10);
                map.AddTBString(EmpAttr.Tel, null, "电话", true, false, 0, 20, 130, true);
                map.AddTBString(EmpAttr.Email, null, "邮箱", true, false, 0, 100, 132, true);
                map.AddTBString(EmpAttr.PinYin, null, "拼音", false, false, 0, 1000, 132, false);
                map.AddTBString(EmpAttr.OrgNo, null, "OrgNo", true, true, 0, 500, 132, false);

                map.AddTBString(EmpAttr.OpenID, null, "微信OpenID", false, false, 0, 200, 36);


                //map.AddDDLEntities(EmpAttr.OrgNo, null, "组织", new BP.Cloud.Orgs(), false);
                //map.AddTBString(EmpAttr.OrgNo, null, "OrgNo", false, false, 0, 36, 36);
                //map.AddTBString(EmpAttr.OrgName, null, "OrgName", false, false, 0, 36, 36);
                map.AddTBInt(EmpAttr.Idx, 0, "序号", true, false);
                #endregion 字段

                #region 相关方法.
                RefMethod rm = new RefMethod();
                rm.Title = "设置图片签名";
                rm.ClassMethodName = this.ToString() + ".DoSinger";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "部门角色";
                rm.ClassMethodName = this.ToString() + ".DoEmpDepts";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                ////节点绑定部门. 节点绑定部门.
                //map.AttrsOfOneVSM.AddBranches(new DeptEmps(), new BP.Port.Depts(),
                //   BP.Port.DeptEmpAttr.FK_Emp,
                //   BP.Port.DeptEmpAttr.FK_Dept, "部门维护", EmpAttr.Name, EmpAttr.No, "@OrgNo");

                rm = new RefMethod();
                rm.Title = "修改密码";
                rm.ClassMethodName = this.ToString() + ".DoResetpassword";
                rm.HisAttrs.AddTBString("pass1", null, "输入密码", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("pass2", null, "再次输入", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "设置部门直属领导";
                //rm.ClassMethodName = this.ToString() + ".DoEditLeader";
                //rm.RefAttrKey = "LeaderName";
                //rm.RefMethodType = RefMethodType.LinkModel;
                //map.AddRefMethod(rm);
                #endregion 相关方法.

                this._enMap = map;
                return this._enMap;
            }
        }

        #region 方法执行.
        public string DoEditMainDept()
        {
            return "../../../GPM/EmpDeptMainDept.htm?FK_Emp=" + this.No + "&FK_Dept=" + this.DeptNo;
        }

        public string DoEditLeader()
        {
            return "../../../GPM/EmpLeader.htm?FK_Emp=" + this.No + "&FK_Dept=" + this.DeptNo;
        }

        public string DoEmpDepts()
        {
            return "/GPM/EmpDepts.htm?FK_Emp=" + this.No;
        }

        public string DoSinger()
        {
            //路径
            return "../../../GPM/Siganture.htm?EmpNo=" + this.No;
        }
        #endregion 方法执行.

        protected override bool beforeInsert()
        {

            ////当前人员所在的部门.
            //  this.OrgNo = BP.Web.WebUser.OrgNo;
            ////处理主部门的问题.
            //DeptEmp de = new DeptEmp();
            //de.DeptNo = this.DeptNo;
            //de.EmpNo = this.No;
            //de.IsMainDept = true;
            //de.OrgNo = this.OrgNo;
            //de.Save();
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

        /// <summary>
        /// 保存后修改WF_Emp中的邮箱
        /// </summary>
        protected override void afterInsertUpdateAction()
        {
            string sql = "Select Count(*) From WF_Emp Where No='" + this.No + "'";
            int count = DBAccess.RunSQLReturnValInt(sql);
            if (count == 0)
            {
                sql = "INSERT INTO WF_Emp (No,Name) VALUES('" + this.No + "','" + this.Name + "')";
                DBAccess.RunSQL(sql);
            }
            //修改Port_Emp中的缓存                  
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = this.No;
            if (emp.RetrieveFromDBSources() != 0)
            {
                emp.DeptNo = this.DeptNo;
                emp.DirectUpdate();
            }

            base.afterInsertUpdateAction();
        }
        protected override bool beforeDelete()
        {
            if (this.OrgNo.Equals(BP.Web.WebUser.OrgNo) == false)
                throw new Exception("err@您不能删除别人的数据.");

            DeptEmps ens = new DeptEmps();
            ens.Delete(DeptEmpAttr.FK_Emp, this.No);

            DeptEmpStations ensD = new DeptEmpStations();
            ensD.Delete(DeptEmpAttr.FK_Emp, this.No);

            return base.beforeDelete();
        }
        /// <summary>
        /// 删除之后要做的事情
        /// </summary>
        protected override void afterDelete()
        {
            base.afterDelete();
        }

        public static string GenerPinYin(string name)
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
            this.DoOrderUp(EmpAttr.FK_Dept, this.DeptNo, EmpAttr.Idx);
            return "执行成功.";
        }
        /// <summary>
        /// 向下移动
        /// </summary>
        public string DoDown()
        {
            this.DoOrderDown(EmpAttr.FK_Dept, this.DeptNo, EmpAttr.Idx);
            return "执行成功.";
        }
        /// <summary>
        /// 重置密码.
        /// </summary>
        /// <param name="pass1"></param>
        /// <param name="pass2"></param>
        /// <returns></returns>
        public string DoResetpassword(string pass1, string pass2)
        {
            BP.Port.Emp emp=new BP.Port.Emp();
            emp.No = this.No;
            emp.Retrieve();

            return emp.DoResetpassword(pass1,pass2);

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
