using BP.DA;
using BP.Web;
using BP.En;
using BP.Sys;

namespace BP.CCOA.KnowledgeManagement
{
    /// <summary>
    /// 知识点 属性
    /// </summary>
    public class KMDtlAttr : EntityTreeAttr
    {
        /// <summary>
        /// 模式
        /// </summary>
        public const string KMDtlPRI = "KMDtlPRI";
        /// <summary>
        /// 内容1
        /// </summary>
        public const string Docs = "Docs";


        public const string Title = "Title";

        /// <summary>
        /// 内容2
        /// </summary>
        public const string KMDtlSta = "KMDtlSta";
        /// <summary>
        /// 内容3
        /// </summary>
        public const string RefTreeNo = "RefTreeNo";
        public const string KnowledgeNo = "KnowledgeNo";
        /// <summary>
        /// 负责人
        /// </summary>
        public const string ManagerEmpName = "ManagerEmpName";

        public const string RefLabelNo = "RefLabelNo";
        public const string RefLabelName = "RefLabelName";

        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录人名称
        /// </summary>
        public const string RecName = "RecName";
        /// <summary>
        /// 记录关注者
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 隶属关注者
        /// </summary>
        public const string RiQi = "RiQi";
        /// <summary>
        /// 年月
        /// </summary>
        public const string Foucs = "Foucs";
        /// <summary>
        /// 第几周
        /// </summary>
        public const string RefEmpsName = "RefEmpsName";
        /// <summary>
        /// 年度
        /// </summary>
        public const string DTTo = "DTTo";
        /// <summary>
        /// 是否删除.
        /// </summary>
        public const string IsDel = "IsDel";
    }
    /// <summary>
    /// 知识点
    /// </summary>
    public class KMDtl : EntityNoName
    {
        #region 基本属性
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get { return this.GetValStrByKey(KMDtlAttr.OrgNo); }
            set { this.SetValByKey(KMDtlAttr.OrgNo, value); }
        }
        public string Rec
        {
            get { return this.GetValStrByKey(KMDtlAttr.Rec); }
            set { this.SetValByKey(KMDtlAttr.Rec, value); }
        }
        public string RecName
        {
            get { return this.GetValStrByKey(KMDtlAttr.RecName); }
            set { this.SetValByKey(KMDtlAttr.RecName, value); }
        }
        public string RDT
        {
            get { return this.GetValStrByKey(KMDtlAttr.RDT); }
            set { this.SetValByKey(KMDtlAttr.RDT, value); }
        }
        /// <summary>
        /// 关注者
        /// </summary>
        public string RiQi
        {
            get { return this.GetValStrByKey(KMDtlAttr.RiQi); }
            set { this.SetValByKey(KMDtlAttr.RiQi, value); }
        }
        /// <summary>
        /// 年月
        /// </summary>
        public string Foucs
        {
            get { return this.GetValStrByKey(KMDtlAttr.Foucs); }
            set { this.SetValByKey(KMDtlAttr.Foucs, value); }
        }
        public string DTTo
        {
            get { return this.GetValStrByKey(KMDtlAttr.DTTo); }
            set { this.SetValByKey(KMDtlAttr.DTTo, value); }
        }
        /// <summary>
        /// 项目数
        /// </summary>
        public string RefTreeNo
        {
            get { return this.GetValStrByKey(KMDtlAttr.RefTreeNo); }
            set { this.SetValByKey(KMDtlAttr.RefTreeNo, value); }
        }
        public string KnowledgeNo
        {
            get { return this.GetValStrByKey(KMTreeAttr.KnowledgeNo); }
            set { this.SetValByKey(KMTreeAttr.KnowledgeNo, value); }
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
        /// 知识点
        /// </summary>
        public KMDtl()
        {
        }
        public KMDtl(string mypk)
        {
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

                Map map = new Map("OA_KMDtl", "知识点");

                map.AddTBStringPK(KMDtlAttr.No, null, "编号", false, false, 0, 50, 10);

                map.AddTBString(KMDtlAttr.Name, null, "名称", true, false, 0, 500, 10,true);
                map.AddTBStringDoc(KMDtlAttr.Docs, "Docs", null, "内容", true, false, 0, 4000, 20, true, true);


                map.AddTBString(KMDtlAttr.RefTreeNo, null, "关联树编号", false, false, 0, 50, 10);
                map.AddTBString(KMDtlAttr.KnowledgeNo, null, "知识编号", false, false, 0, 50, 10);
                map.AddTBString(KMDtlAttr.Foucs, null, "关注者(多个人用都好分开)", false, false, 0, 4000, 10);

                // map.AddTBString(KMDtlAttr.Docs, null, "内容", false, false, 0, 4000, 10);
                // map.AddDDLSysEnum(KMDtlAttr.KMDtlPRI, 0, "优先级", true, false, "KMDtlPRI", "@0=高@1=中@2=低");
                //   map.AddDDLSysEnum(KMDtlAttr.KMDtlSta, 0, "状态", true, false, "KMDtlSta", "@0=未完成@1=删除");
                // map.AddTBInt(KMDtlAttr.KMDtlSta, 0, "状态", false, false);

                //
                map.AddTBInt(KMDtlAttr.IsDel, 0, "IsDel", false, false);


                map.AddTBString(KMDtlAttr.OrgNo, null, "组织编号", false, false, 0, 100, 10);
                map.AddTBString(KMDtlAttr.Rec, null, "记录人", false, false, 0, 100, 10);
                map.AddTBString(KMDtlAttr.RecName, null, "记录人名称", false, false, 0, 100, 10, true);
                map.AddTBDateTime(KMDtlAttr.RDT, null, "记录时间", false, false);
                map.AddTBInt(KMTreeAttr.Idx, 0, "Idx", false, false);

                map.AddMyFileS("附件");




                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行方法.
        protected override bool beforeInsert()
        {
            this.No = DBAccess.GenerGUID();
            this.Rec = WebUser.No;
            this.RecName = WebUser.Name;
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                this.OrgNo = WebUser.OrgNo;

            //设置日期.
            this.SetValByKey(KMDtlAttr.RDT, DataType.CurrentDateTime);

            return base.beforeInsert();
        }
        protected override bool beforeUpdate()
        {
            ////计算条数.
            //this.RefTreeNo = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS N FROM OA_KMDtlDtl WHERE RefPK='" + this.MyPK + "'");

            ////计算合计工作小时..
            //this.Manager = DBAccess.RunSQLReturnValInt("SELECT SUM(Hour) + Sum(Minute)/60.00 AS N FROM OA_KMDtlDtl WHERE RefPK='" + this.MyPK + "'");

            return base.beforeUpdate();
        }
        #endregion 执行方法.
    }
    /// <summary>
    /// 知识点 s
    /// </summary>
    public class KMDtls : EntitiesNoName
    {
        #region 查询.
        /// <summary>
        /// 所有的知识点
        /// </summary>
        /// <returns></returns>
        public string KMDtl_AllKMDtls_del()
        {
            QueryObject qo = new QueryObject(this);

            qo.addLeftBracket();
            qo.AddWhere(KMDtlAttr.Rec, WebUser.No);
            qo.addOr();
            qo.AddWhere(KMDtlAttr.RefTreeNo, " like ", "%," + WebUser.No + ",%");
            qo.addRightBracket();

            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                qo.addAnd();
                qo.AddWhere(KMDtlAttr.OrgNo, " = ", WebUser.OrgNo);
            }
            qo.DoQuery();
            return this.ToJson();
        }
        #endregion 重写.

        #region 重写.
        /// <summary>
        /// 知识点
        /// </summary>
        public KMDtls() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new KMDtl();
            }
        }
        #endregion 重写.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<KMDtl> ToJavaList()
        {
            return (System.Collections.Generic.IList<KMDtl>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<KMDtl> Tolist()
        {
            System.Collections.Generic.List<KMDtl> list = new System.Collections.Generic.List<KMDtl>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((KMDtl)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
