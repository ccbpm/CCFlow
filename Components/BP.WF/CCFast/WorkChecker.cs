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
    /// 日志审核 属性
    /// </summary>
    public class WorkCheckerAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 内容
        /// </summary>
        public const string Doc = "Doc";
        /// <summary>
        /// 分数
        /// </summary>
        public const string Cent = "Cent";
        /// <summary>
        /// 提醒时间
        /// </summary>
        public const string RefPK = "RefPK";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录人名字
        /// </summary>
        public const string RecName = "RecName";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
    }
    /// <summary>
    /// 日志审核
    /// </summary>
    public class WorkChecker : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get { return this.GetValStrByKey(WorkCheckerAttr.OrgNo); }
            set { this.SetValByKey(WorkCheckerAttr.OrgNo, value); }
        }
        public string Rec
        {
            get { return this.GetValStrByKey(WorkCheckerAttr.Rec); }
            set { this.SetValByKey(WorkCheckerAttr.Rec, value); }
        }
        public string RDT
        {
            get { return this.GetValStrByKey(WorkCheckerAttr.RDT); }
            set { this.SetValByKey(WorkCheckerAttr.RDT, value); }
        }
        public string RecName
        {
            get { return this.GetValStrByKey(WorkCheckerAttr.RecName); }
            set { this.SetValByKey(WorkCheckerAttr.RecName, value); }
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
        /// 日志审核
        /// </summary>
        public WorkChecker()
        {
        }
        public WorkChecker(string mypk)
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

                Map map = new Map("OA_WorkChecker", "日志审核");

                map.AddMyPK();

                map.AddTBString(WorkCheckerAttr.RefPK, null, "RefPK", false, false, 0, 100, 10);
                map.AddTBString(WorkCheckerAttr.Doc, null, "Doc", false, false, 0, 999, 10);

                map.AddTBInt(WorkCheckerAttr.Cent, 0, "评分", false, false);

                map.AddTBString(WorkCheckerAttr.OrgNo, null, "OrgNo", false, false, 0, 100, 10);
                map.AddTBString(WorkCheckerAttr.Rec, null, "记录人", false, false, 0, 100, 10, true);
                map.AddTBString(WorkCheckerAttr.RecName, null, "记录人", false, false, 0, 100, 10, true);

                map.AddTBDateTime(WorkCheckerAttr.RDT, null, "记录时间", false, false);

           
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.MyPK = DBAccess.GenerGUID();
            this.Rec = WebUser.No;
            this.RecName = WebUser.Name;
            this.OrgNo = WebUser.OrgNo;

            this.RDT = DataType.CurrentDataTime;

            return base.beforeInsert();
        }


    }
    /// <summary>
    /// 日志审核 s
    /// </summary>
    public class WorkCheckers : EntitiesMyPK
    {
        /// <summary>
        /// 日志审核
        /// </summary>
        public WorkCheckers() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new WorkChecker();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<WorkChecker> ToJavaList()
        {
            return (System.Collections.Generic.IList<WorkChecker>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<WorkChecker> Tolist()
        {
            System.Collections.Generic.List<WorkChecker> list = new System.Collections.Generic.List<WorkChecker>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((WorkChecker)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
