using System;
using System.Data;
using BP.DA;
using BP.Difference;
using BP.En;
using BP.Sys;

namespace BP.Cloud
{
    /// <summary>
    /// 操作员 的摘要说明。
    /// </summary>
    public class EmpExt : EntityNoName
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

                string sql = "SELECT COUNT(FK_EmpExt) FROM Port_DeptEmpExtStation WHERE FK_EmpExt='" + this.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                    return false;

                sql = "SELECT COUNT(FK_EmpExt) FROM Port_DeptEmpExt WHERE FK_EmpExt='" + this.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                    return false;

                return true;
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
        #endregion

        #region 构造函数
        /// <summary>
        /// 操作员
        /// </summary>
        public EmpExt()
        {
        }
       
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No.Equals("admin"))
                {
                    uac.IsInsert = false;
                    uac.IsUpdate = false;
                    uac.IsDelete = false;
                    uac.IsView = true;
                    return uac;
                }
                uac.OpenForAppAdmin();
                uac.IsInsert = false;
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
                map.AddTBStringPK(EmpAttr.No, null, "编号", false, false, 1, 50, 90);

                map.AddTBString(EmpAttr.UserID, null, "手机号", true, true, 1, 50, 90);
                map.AddTBString(EmpAttr.Name, null, "姓名", true, false, 0, 200, 130);

                map.AddDDLEntities(EmpAttr.FK_Dept, null, "主部门", new BP.Cloud.Depts(), false);

                map.AddTBString(EmpAttr.Tel, null, "电话", true, false, 0, 20, 130);
                map.AddTBString(EmpAttr.Email, null, "邮箱", true, false, 0, 100, 132, true);
                map.AddTBString(EmpAttr.QQ, null, "QQ", true, false, 0, 500, 132, true);

                map.AddTBString(EmpAttr.SID, null, "安全校验码", false, false, 0, 36, 36);
                map.AddTBString(EmpAttr.PinYin, null, "拼音", false, false, 0, 500, 132, true);
                map.AddTBString(EmpAttr.QQAppID, null, "QQAppID", false, false, 0, 500, 132, false);
                map.AddTBInt(EmpAttr.Idx, 0, "序号", false, false);
                #endregion 字段

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

                rm = new RefMethod();
                rm.Title = "修改密码";
                rm.ClassMethodName = this.ToString() + ".DoResetpassword";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.HisAttrs.AddTBString("pass1", null, "输入密码", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("pass2", null, "再次输入", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "修改主部门";
                rm.ClassMethodName = this.ToString() + ".DoEditMainDept";
                rm.RefAttrKey = EmpAttr.FK_Dept;
                rm.RefMethodType = RefMethodType.LinkModel;
                map.AddRefMethod(rm);

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
            //if (this.Pass == "")
            //    this.Pass = "123";

            //if (SystemConfig.IsEnablePasswordEncryption == true)
            //    this.Pass = BP.Tools.Cryptography.EncryptString(this.Pass);

            ////当前人员所在的部门.
            //this.OrgNo = BP.Web.WebUser.FK_Dept;
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
            //DeptEmpExtStations des = new DeptEmpExtStations();
            //des.Retrieve(DeptEmpExtStationAttr.FK_EmpExt, this.No);

            //string depts = "";
            //string stas = "";

            //foreach (DeptEmpExtStation item in des)
            //{
            //    BP.GPM.Dept dept = new BP.GPM.Dept();
            //    dept.No = item.FK_Dept;
            //    if (dept.RetrieveFromDBSources() == 0)
            //    {
            //        item.Delete();
            //        continue;
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
            return base.beforeUpdateInsertAction();
        }

        /// <summary>
        /// 保存后修改WF_EmpExt中的邮箱
        /// </summary>
        protected override void afterInsertUpdateAction()
        {
            string sql = "Select Count(*) From WF_Emp Where No='" + this.No + "'";
            int count = DBAccess.RunSQLReturnValInt(sql);
            if (count == 0)
                sql = "INSERT INTO WF_Emp (No,Name,Email) VALUES('" + this.No + "','" + this.Name + "','" + this.Email + "')";
            else
                sql = "UPDATE WF_Emp SET Email='" + this.Email + "' where No='"+this.No+"'";

            DBAccess.RunSQL(sql);

            base.afterInsertUpdateAction();
        }
        protected override bool beforeDelete()
        {
            if (this.OrgNo != BP.Web.WebUser.OrgNo)
                throw new  Exception("err@您不能删除别人的数据.");

            return base.beforeDelete();
        }
        /// <summary>
        /// 删除之后要做的事情
        /// </summary>
        protected override void afterDelete()
        {
            DeptEmps ens = new DeptEmps();
            ens.Delete(DeptEmpAttr.FK_Emp, this.UserID);

            DeptEmpStations ensD = new DeptEmpStations();
            ensD.Delete(DeptEmpAttr.FK_Emp, this.UserID);

            base.afterDelete();
        }

        public static string GenerPinYin(string no, string name)
        {
            //增加拼音，以方便查找.
            string pinyinQP = BP.DA.DataType.ParseStringToPinyin(name).ToLower();
            string pinyinJX = BP.DA.DataType.ParseStringToPinyinJianXie(name).ToLower();
            string py = "," + pinyinQP + "," + pinyinJX + ",";

            ////处理岗位信息.
            //DeptEmpExtStations des = new DeptEmpExtStations();
            //des.Retrieve(DeptEmpExtStationAttr.FK_EmpExt, no);

            //string depts = "";
            //string stas = "";

            //foreach (DeptEmpExtStation item in des)
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
            get { return new EmpExts(); }
        }
        #endregion 构造函数
    }
    /// <summary>
    /// 操作员s
    // </summary>
    public class EmpExts : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EmpExt();
            }
        }
        /// <summary>
        /// 操作员s
        /// </summary>
        public EmpExts()
        {
        }
        #endregion 构造方法

        public override int RetrieveAll()
        {
            if (BP.Web.WebUser.No.Equals("admin"))
                return base.RetrieveAll();

            return this.Retrieve(EmpAttr.OrgNo, BP.Web.WebUser.OrgNo);
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EmpExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<EmpExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EmpExt> Tolist()
        {
            System.Collections.Generic.List<EmpExt> list = new System.Collections.Generic.List<EmpExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EmpExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
