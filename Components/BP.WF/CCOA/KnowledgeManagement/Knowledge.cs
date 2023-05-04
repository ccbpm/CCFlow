using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.Difference;

namespace BP.CCOA.KnowledgeManagement
{
    /// <summary>
    /// 知识管理 属性
    /// </summary>
    public class KnowledgeAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 参与人s
        /// </summary>
        public const string Emps = "Emps";
        /// <summary>
      
        /// 关注人
        /// </summary>
        public const string Foucs = "Foucs";
        /// <summary>
        /// 内容1
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 内容1
        /// </summary>
        public const string Docs = "Docs";
        /// <summary>
        /// 创建字段
        /// </summary>
        public const string ImgUrl = "ImgUrl";
        /// <summary>
        /// 内容2
        /// </summary>
        public const string KnowledgeSta = "KnowledgeSta";
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
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
    }
    /// <summary>
    /// 知识管理
    /// </summary>
    public class Knowledge : EntityNoName
    {
        #region 基本属性
        public int KnowledgeSta
        {
            get { return this.GetValIntByKey(KnowledgeAttr.KnowledgeSta); }
            set { this.SetValByKey(KnowledgeAttr.KnowledgeSta, value); }
        }
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get { return this.GetValStrByKey(KnowledgeAttr.OrgNo); }
            set { this.SetValByKey(KnowledgeAttr.OrgNo, value); }
        }
        public string Rec
        {
            get { return this.GetValStrByKey(KnowledgeAttr.Rec); }
            set { this.SetValByKey(KnowledgeAttr.Rec, value); }
        }
        public string RecName
        {
            get { return this.GetValStrByKey(KnowledgeAttr.RecName); }
            set { this.SetValByKey(KnowledgeAttr.RecName, value); }
        }
        public string RDT
        {
            get { return this.GetValStrByKey(KnowledgeAttr.RDT); }
            set { this.SetValByKey(KnowledgeAttr.RDT, value); }
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
        /// 知识管理
        /// </summary>
        public Knowledge()
        {
        }
        public Knowledge(string mypk)
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

                Map map = new Map("OA_Knowledge", "知识管理");

                map.AddTBStringPK(KnowledgeAttr.No, null, "编号", false, false, 0, 50, 10);
                map.AddTBString(KnowledgeAttr.Name, null, "名称", true, true, 0, 500, 10);

                map.AddTBString(KnowledgeAttr.ImgUrl, null, "路径", true, true, 0, 500, 10);
                map.AddTBString(KnowledgeAttr.Title, null, "标题", true, true, 0, 4000, 10);
                map.AddTBString(KnowledgeAttr.Docs, null, "描述", true, true, 0, 4000, 10);
                //map.AddTBString(KnowledgeAttr.KnowledgeSta, null, "状态", true, true, 0, 4000, 10);

                map.AddDDLSysEnum(KnowledgeAttr.KnowledgeSta, 0, "状态", true, false, "KnowledgeSta", "@0=公开@1=私有");

                //zhoupeng@周朋;liping@李萍;
                map.AddTBString(KnowledgeAttr.Emps, null, "参与人", false, false, 0, 4000, 10);

                //,zhoupeng,liping,
                map.AddTBString(KnowledgeAttr.Foucs, null, "关注的人(多个人用逗号分开)", false, false, 0, 4000, 10);


                map.AddTBString(KnowledgeAttr.OrgNo, null, "组织编号", false, false, 0, 100, 10);
                map.AddTBString(KnowledgeAttr.Rec, null, "记录人", false, false, 0, 100, 10);
                map.AddTBString(KnowledgeAttr.RecName, null, "记录人名称", false, false, 0, 100, 10, true);
                map.AddTBDateTime(KnowledgeAttr.RDT, null, "记录时间", false, false);

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

            #region 初始化目录数据
            //初始化目录数据.

            KMTree en = new KMTree();
            en.No = this.No;
            en.Name = "根目录";
            en.ParentNo = "0";
            en.KnowledgeNo = this.No;
            en.DirectInsert();

            en = new KMTree();
            en.Name = "目录1";
            en.ParentNo = this.No;
            en.KnowledgeNo = this.No;
            en.Insert();

            KMDtl dtl = new KMDtl();
            dtl.Name = "文件1";
            dtl.RefTreeNo = en.No;
            dtl.KnowledgeNo = this.No;         
            dtl.Insert();

            dtl = new KMDtl();
            dtl.Name = "文件2";
            dtl.RefTreeNo = en.No;
            dtl.KnowledgeNo = this.No;
            dtl.Insert();


            en = new KMTree();
            en.Name = "目录2";
            en.ParentNo = this.No;
            en.KnowledgeNo = this.No;
            en.Insert();

            dtl = new KMDtl();
            dtl.Name = "文件1";
            dtl.RefTreeNo = en.No;
            dtl.KnowledgeNo = this.No;
            dtl.Insert();

            dtl = new KMDtl();
            dtl.Name = "文件2";
            dtl.RefTreeNo = en.No;
            dtl.KnowledgeNo = this.No;
            dtl.Insert();
            #endregion 初始化目录数据

            return base.beforeInsert();
        }
        protected override bool beforeUpdate()
        {
            ////计算条数.
            //this.RefEmpsNo = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS N FROM OA_KnowledgeDtl WHERE RefPK='" + this.MyPK + "'");

            ////计算合计工作小时..
            //this.Manager = DBAccess.RunSQLReturnValInt("SELECT SUM(Hour) + Sum(Minute)/60.00 AS N FROM OA_KnowledgeDtl WHERE RefPK='" + this.MyPK + "'");

            return base.beforeUpdate();
        }
        #endregion 执行方法.
    }
    /// <summary>
    /// 知识管理 s
    /// </summary>
    public class Knowledges : EntitiesNoName
    {
        #region 查询.
        /// <summary>
        /// 所有的知识管理
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {

            //初始化实体.
            Knowledges ens = new Knowledges();

            QueryObject qo = new QueryObject(ens);
            //  qo.addLeftBracket();
            //qo.AddWhere(KnowledgeAttr.KnowledgeSta, 0);

            //qo.addOr();
            //qo.AddWhere(KnowledgeAttr.KnowledgeSta, 1);

            //qo.addLeftBracket();
            //qo.AddWhere(KnowledgeAttr.KnowledgeSta, 1);
            //qo.addAnd();
            //qo.AddWhere(KnowledgeAttr.Emps, " like ", "%," + WebUser.No + ",%");
            //qo.addRightBracket();

            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
            {
                qo.addAnd();
                qo.AddWhere(KnowledgeAttr.OrgNo, " = ", WebUser.OrgNo);
            }
            qo.DoQuery();

            return ens.ToJson();
        }

        public string Default_Init11()
        {

            //初始化实体.
            Knowledges ens = new Knowledges();

            QueryObject qo = new QueryObject(ens);
            //  qo.addLeftBracket();
            qo.AddWhere(KnowledgeAttr.KnowledgeSta, 0);

            //   qo.addOr();

            //qo.addLeftBracket();
            //qo.AddWhere(KnowledgeAttr.KnowledgeSta, 1);
            //qo.addAnd();
            //qo.AddWhere(KnowledgeAttr.Emps, " like ", "%," + WebUser.No + ",%");
            //qo.addRightBracket();

            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.SAAS)
            {
                qo.addAnd();
                qo.AddWhere(KnowledgeAttr.OrgNo, " = ", WebUser.OrgNo);
            }
            qo.DoQuery();

            return ens.ToJson();
        }
        /// <summary>
        /// 关注的知识点
        /// </summary>
        /// <returns></returns>
        public string Default_KMDtlFoucs()
        {
            KMDtls dtls = new KMDtls();
            dtls.Retrieve(KMDtlAttr.Foucs, " LIKE ", "%" + WebUser.No + "%");
            return dtls.ToJson();
        }
        #endregion 重写.

        #region 重写.
        /// <summary>
        /// 知识管理
        /// </summary>
        public Knowledges() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Knowledge();
            }
        }
        #endregion 重写.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Knowledge> ToJavaList()
        {
            return (System.Collections.Generic.IList<Knowledge>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Knowledge> Tolist()
        {
            System.Collections.Generic.List<Knowledge> list = new System.Collections.Generic.List<Knowledge>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Knowledge)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.


        public string Selecter_DeptEmps(string deptNo)
        {
            DataSet ds = new DataSet();

            Depts depts = new Depts();
            QueryObject qo = new QueryObject(depts);
            qo.AddWhere(DeptAttr.ParentNo, " = ", deptNo);
            if(SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
            {
                qo.addAnd();
                qo.AddWhere(DeptAttr.OrgNo, WebUser.OrgNo);

            }
            qo.addOrderBy(DeptAttr.Idx);
            qo.DoQuery();

            //获取这个部门下的人员
            Emps emps = new Emps();
            emps.Retrieve(EmpAttr.FK_Dept, deptNo);

            ds.Tables.Add(depts.ToDataTableField("Depts"));
            ds.Tables.Add(emps.ToDataTableField("Emps"));

            return BP.Tools.Json.ToJson(ds);

        }
        public string SelectEmpByKey(string searchKey)
        {
            string dbStr =SystemConfig.AppCenterDBVarStr;
            string sql = " SELECT A.No,A.Name,B.NameOfPath,B.Name AS DeptName From Port_Emp A ,Port_Dept B WHERE A.FK_Dept=B.No AND (A.No like";
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.MySQL:
                case DBType.PostgreSQL:
                    sql += " CONCAT('%'," + SystemConfig.AppCenterDBVarStr + "No,'%') OR A.Name like CONCAT('%'," + SystemConfig.AppCenterDBVarStr + "Name,'%'))";
                    break;
                case DBType.MSSQL:
                    sql += " '%'+" + SystemConfig.AppCenterDBVarStr + "No+'%' OR A.Name like '%'+" + SystemConfig.AppCenterDBVarStr + "Name+'%')";
                    break;
                case DBType.Oracle:
                    sql += " '%'||" + SystemConfig.AppCenterDBVarStr + "No||'%' OR A.Name like '%'||" + SystemConfig.AppCenterDBVarStr + "Name||'%')";
                    break;
                default:
                    throw new System.Exception("err@数据据" + SystemConfig.AppCenterDBType + "还未解析");

            }
            if (SystemConfig.CCBPMRunModel!=CCBPMRunModel.Single)
                sql += " AND A.OrgNo='" + WebUser.OrgNo + "' AND B.OrgNo='" + WebUser.OrgNo + "'";
            Paras paras = new Paras();
            paras.SQL = sql;
            paras.Add("No", searchKey);
            paras.Add("Name", searchKey);
            DataTable dt = DBAccess.RunSQLReturnTable(paras);
            if(SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dt.Columns[0].ColumnName = "No";
                dt.Columns[1].ColumnName = "Name";
                dt.Columns[2].ColumnName = "NameOfPath";
                dt.Columns[3].ColumnName = "DeptName";
            }
            return BP.Tools.Json.ToJson(dt);
        }
    }


}
