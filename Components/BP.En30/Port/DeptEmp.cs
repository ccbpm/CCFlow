using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.Port
{
    /// <summary>
    /// 部门人员信息
    /// </summary>
    public class DeptEmpAttr
    {
        #region 基本属性
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 人员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 组织编码
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 岗位编码
        /// </summary>
        public const string StationNo = "StationNo";
        public const string StationNoT = "StationNoT";
        public const string DeptName = "DeptName";
        #endregion
    }
    /// <summary>
    /// 部门人员信息 的摘要说明。
    /// </summary>
    public class DeptEmp : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
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
        /// 人员
        /// </summary>
        public string EmpNo
        {
            get
            {
                return this.GetValStringByKey(DeptEmpAttr.FK_Emp);
            }
            set
            {
                SetValByKey(DeptEmpAttr.FK_Emp, value);
                this.setMyPK(this.DeptNo + "_" + this.EmpNo);
            }
        }
        /// <summary>
        /// 部门
        /// </summary>
        public string DeptNo
        {
            get
            {
                return this.GetValStringByKey(DeptEmpAttr.FK_Dept);
            }
            set
            {
                SetValByKey(DeptEmpAttr.FK_Dept, value);
                this.setMyPK(this.DeptNo + "_" + this.EmpNo);
            }
        }
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(DeptEmpAttr.OrgNo);
            }
            set
            {
                SetValByKey(DeptEmpAttr.OrgNo, value);
            }
        }
        public string StationNo
        {
            get
            {
                return this.GetValStringByKey(DeptEmpAttr.StationNo);
            }
            set
            {
                SetValByKey(DeptEmpAttr.StationNo, value);
            }
        }
        public string StationNoT
        {
            get
            {
                return this.GetValStringByKey(DeptEmpAttr.StationNoT);
            }
            set
            {
                SetValByKey(DeptEmpAttr.StationNoT, value);
            }
        }
        public string DeptName
        {
            get
            {
                return this.GetValStringByKey(DeptEmpAttr.DeptName);
            }
            set
            {
                SetValByKey(DeptEmpAttr.DeptName, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 工作部门人员信息
        /// </summary> 
        public DeptEmp() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_DeptEmp", "部门人员信息");
                map.IndexField = DeptEmpAttr.FK_Dept;

                map.AddMyPK();
                map.AddTBString(DeptEmpAttr.FK_Dept, null, "部门", false, false, 1, 50, 1);
                map.AddDDLEntities(DeptEmpAttr.FK_Emp, null, "操作员", new BP.Port.Emps(), false);
                map.AddTBString(DeptEmpAttr.OrgNo, null, "组织编码", false, false, 0, 50, 50);

                //For Vue3版本.
                map.AddTBString(DeptEmpAttr.DeptName, null, "部门名称(Vue3)", false, false, 0, 500, 36);
                map.AddTBString(DeptEmpAttr.StationNo, null, "岗位编号(Vue3)", false, false, 0, 500, 36);
                map.AddTBString(DeptEmpAttr.StationNoT, null, "岗位名称(Vue3)", false, false, 0, 500, 36);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeDelete()
        {
            BP.Sys.Base.Glo.WriteUserLog("删除:" + this.ToJson(), "组织数据操作");
            return base.beforeDelete();
        }
        protected override bool beforeInsert()
        {
            BP.Sys.Base.Glo.WriteUserLog("新建:" + this.ToJson(), "组织数据操作");
            return base.beforeInsert();
        }

        protected override void afterDelete()
        {
            DeptEmpStations des = new DeptEmpStations();
            des.Delete("FK_Dept", this.GetValByKey("FK_Dept"), "FK_Emp", this.GetValByKey("FK_Emp"));
            base.afterDelete();
        }

        /// <summary>
        /// 更新前做的事情
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdateInsertAction()
        {
			if (BP.Difference.SystemConfig.CCBPMRunModel != BP.Sys.CCBPMRunModel.Single &&  DataType.IsNullOrEmpty(this.OrgNo))
                this.OrgNo = BP.Web.WebUser.OrgNo;            
			if (DataType.IsNullOrEmpty(this.MyPK) == true)
            {
                if (BP.Difference.SystemConfig.CCBPMRunModel == BP.Sys.CCBPMRunModel.SAAS)
                {
                    this.setMyPK(this.DeptNo + "_" + this.EmpNo.Replace(this.OrgNo+"_",""));
                }
                else
                {
                    this.setMyPK(this.DeptNo + "_" + this.EmpNo);
                }
                    
            }
            
            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 部门人员信息 
    /// </summary>
    public class DeptEmps : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 工作部门人员信息
        /// </summary>
        public DeptEmps()
        {
        }
        #endregion

        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DeptEmp();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DeptEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<DeptEmp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DeptEmp> Tolist()
        {
            System.Collections.Generic.List<DeptEmp> list = new System.Collections.Generic.List<DeptEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DeptEmp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

        #region 删除方法
        public string DelteNotInEmp()
        {
            string sql = "DELETE FROM Port_DeptEmp WHERE FK_Emp NOT IN (SELECT No FROM Port_Emp)";
            DBAccess.RunSQL(sql);
            return "删除成功";
        }
        #endregion

    }
}
