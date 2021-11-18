using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.CCBill.Template;

namespace BP.CCFast
{
    /// <summary>
    /// 日志共享 属性
    /// </summary>
    public class WorkShareAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 共享人
        /// </summary>
        public const string EmpNo = "EmpNo";
        /// <summary>
        /// 共享人名称
        /// </summary>
        public const string EmpName = "EmpName";
        /// <summary>
        /// 共享给
        /// </summary>
        public const string ShareToEmpNo = "ShareToEmpNo";
        /// <summary>
        /// 共享给人员名称
        /// </summary>
        public const string ShareToEmpName = "ShareToEmpName";
        /// <summary>
        /// 状态
        /// </summary>
        public const string ShareState = "ShareState";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
    }
    /// <summary>
    /// 日志共享
    /// </summary>
    public class WorkShare : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get { return this.GetValStrByKey(WorkShareAttr.OrgNo); }
            set { this.SetValByKey(WorkShareAttr.OrgNo, value); }
        }
        public string EmpNo
        {
            get { return this.GetValStrByKey(WorkShareAttr.EmpNo); }
            set { this.SetValByKey(WorkShareAttr.EmpNo, value); }
        }
        public string EmpName
        {
            get { return this.GetValStrByKey(WorkShareAttr.EmpName); }
            set { this.SetValByKey(WorkShareAttr.EmpName, value); }
        }
        public string ShareToEmpNo
        {
            get { return this.GetValStrByKey(WorkShareAttr.ShareToEmpNo); }
            set { this.SetValByKey(WorkShareAttr.ShareToEmpNo, value); }
        }
        public string ShareToEmpName
        {
            get { return this.GetValStrByKey(WorkShareAttr.ShareToEmpName); }
            set { this.SetValByKey(WorkShareAttr.ShareToEmpName, value); }
        }
        public int ShareState
        {
            get { return this.GetValIntByKey(WorkShareAttr.ShareState); }
            set { this.SetValByKey(WorkShareAttr.ShareState, value); }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (WebUser.IsAdmin)
                {
                    uac.IsUpdate = true;
                    return uac;
                }
                return base.HisUAC;
            }
        }
        /// <summary>
        /// 日志共享
        /// </summary>
        public WorkShare()
        {
        }
        public WorkShare(string mypk)
        {
            this.MyPK = mypk;
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

                Map map = new Map("OA_WorkShare", "日志共享");

                map.AddMyPK();

                map.AddTBString(WorkShareAttr.EmpNo, null, "记录人", false, false, 0, 100, 10, true);
                map.AddTBString(WorkShareAttr.EmpName, null, "记录人名称", false, false, 0, 100, 10, true);

                map.AddTBString(WorkShareAttr.ShareToEmpNo, null, "记录人名称", false, false, 0, 100, 10, true);
                map.AddTBString(WorkShareAttr.ShareToEmpName, null, "记录人名称", false, false, 0, 100, 10, true);

                map.AddTBInt(WorkShareAttr.ShareState, 0, "状态0=关闭,1=分享", false, false);
                map.AddTBString(WorkShareAttr.OrgNo, null, "组织编号", false, false, 0, 50, 10);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行方法.
        protected override bool beforeInsert()
        {

            this.OrgNo = BP.Web.WebUser.OrgNo;

            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                this.MyPK = this.EmpNo + "_" + this.ShareToEmpNo;
            else
                this.MyPK = this.OrgNo + "_" + this.EmpNo + "_" + this.ShareToEmpNo;

            this.EmpNo = WebUser.No;
            this.EmpName = WebUser.Name;


            BP.Port.Emp emp = new Emp();
            emp.No = this.ShareToEmpNo;
            if (emp.RetrieveFromDBSources() == 0)
                throw new Exception("err@错误:人员编号不正确." + emp.No);

            this.ShareToEmpName = emp.Name;

            this.ShareState = 1;

            return base.beforeInsert();
        }
        protected override bool beforeUpdate()
        {
            return base.beforeUpdate();
        }
        #endregion 执行方法.
    }
    /// <summary>
    /// 日志共享 s
    /// </summary>
    public class WorkShares : EntitiesMyPK
    {
        public string DoAddEmp(string empNo)
        {
            return "err@执行失败.";
        }

        public string ShareTo_Init()
        {
            this.Retrieve(WorkShareAttr.EmpNo, WebUser.No);

            if (this.Count == 0)
            {
                Emps emps = new Emps();
                emps.Retrieve(EmpAttr.FK_Dept, WebUser.FK_Dept);

                foreach (Emp emp in emps)
                {
                    WorkShare en = new WorkShare();
                    en.ShareToEmpNo = emp.No;
                    en.ShareToEmpName = emp.Name;
                    en.ShareState = 1;
                    en.Insert();
                }

                this.Retrieve(WorkShareAttr.EmpNo, WebUser.No);
            }

            return this.ToJson();
        }
        /// <summary>
        /// 日志共享
        /// </summary>
        public WorkShares() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new WorkShare();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<WorkShare> ToJavaList()
        {
            return (System.Collections.Generic.IList<WorkShare>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<WorkShare> Tolist()
        {
            System.Collections.Generic.List<WorkShare> list = new System.Collections.Generic.List<WorkShare>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((WorkShare)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
