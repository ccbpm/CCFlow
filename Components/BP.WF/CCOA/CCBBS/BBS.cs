using System;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Sys;

namespace BP.CCOA.CCBBS
{
    /// <summary>
    /// 信息 属性
    /// </summary>
    public class BBSAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 信息内容
        /// </summary>
        public const string Docs = "Docs";
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

        public const string RecDeptNo = "RecDeptNo";
        public const string RecDeptName = "RecDeptName";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 年月
        /// </summary>
        public const string NianYue = "NianYue";
        /// <summary>
        /// 读取人数
        /// </summary>
        public const string ReadTimes = "ReadTimes";
        /// <summary>
        /// 读取人
        /// </summary>
        public const string Reader = "Reader";
        /// <summary>
        /// 状态
        /// </summary>
        public const string BBSSta = "BBSSta";

        public const string RelerName = "RelerName";
        public const string RelDeptName = "RelDeptName";

        public const string BBSType = "BBSType";
        public const string BBSPRI = "BBSPRI";

    }
    /// <summary>
    /// 信息
    /// </summary>
    public class BBS : EntityNoName
    {
        #region 基本属性
        public string Docs
        {
            get
            {
                return this.GetValStrByKey(BBSAttr.Docs);
            }
            set
            {
                this.SetValByKey(BBSAttr.Docs, value);
            }
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
                uac.IsUpdate = true;
                uac.IsInsert = true;
                return uac;

                ////if (WebUser.IsAdmin)
                ////{
                ////    uac.IsUpdate = true;
                ////    return uac;
                ////}
                //return base.HisUAC;
            }
        }
        /// <summary>
        /// 信息
        /// </summary>
        public BBS()
        {
        }
        public BBS(string no)
        {
            this.SetValByKey(BBSAttr.No, no);
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

                Map map = new Map("OA_BBS", "信息");

                map.AddTBStringPK(BBSAttr.No, null, "编号", false, true, 1, 59, 59);
                map.AddTBString(BBSAttr.Name, null, "标题", true, false, 0, 100, 10, true);

                map.AddTBStringDoc(BBSAttr.Docs, "Docs", null, "内容", true, false, 0, 4000, 20, true, true);


                map.AddDDLSysEnum(BBSAttr.BBSPRI, 0, "重要性", true, true, "BBSPRI", "@0=普通@1=紧急@2=火急");

                map.AddDDLSysEnum(BBSAttr.BBSSta, 0, "状态", true, true, "BBSSta", "@0=发布中@1=禁用");
                map.AddDDLEntities(BBSAttr.BBSType, null, "类型", new BBSTypes(), true);


                map.AddTBString(BBSAttr.Rec, null, "记录人", false, false, 0, 100, 10);
                map.AddTBString(BBSAttr.RecName, null, "记录人", true, true, 0, 100, 10, false);
                map.AddTBString(BBSAttr.RecDeptNo, null, "记录人部门", false, false, 0, 100, 10, false);


                map.AddTBString(BBSAttr.RelerName, null, "发布人", true, false, 0, 100, 10, false);
                map.AddTBString(BBSAttr.RelDeptName, null, "发布单位", true, false, 0, 100, 10, false);

                map.AddTBDateTime(BBSAttr.RDT, null, "发布日期", true, true);
                map.AddTBString(BBSAttr.NianYue, null, "隶属年月", false, false, 0, 10, 10);

                map.AddTBInt(BBSAttr.ReadTimes, 0, "读取次数", true, true);
                map.AddTBStringDoc(BBSAttr.Reader, null, "读取人", false, false, false);

                if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    map.AddTBString(BBSAttr.OrgNo, null, "组织", true, true, 0, 100, 10);

                //增加附件.
                map.AddMyFileS();

                #region 设置查询条件.
                map.DTSearchKey = BBSAttr.RDT;
                map.DTSearchWay = DTSearchWay.ByDate;
                map.DTSearchLabel = "发布日期";

                if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    map.AddHidden("OrgNo", "=", BP.Web.WebUser.OrgNo);

                map.AddSearchAttr(BBSAttr.BBSSta);
                map.AddSearchAttr(BBSAttr.BBSType);
                #endregion 设置查询条件.


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行方法.
        public string DoRead()
        {
            string reader = this.GetValStrByKey("Reader");
            if (reader.Contains(WebUser.No + ",") == false)
                return "";

            reader += "@" + WebUser.Name + ",";
            this.SetValByKey("Reader", reader);
            int t = this.GetValIntByKey(BBSAttr.ReadTimes);
            this.SetValByKey("ReadTimes", t + 1);
            try
            {
                this.Update();
            }
            catch (Exception ex)
            {
             //  BP.DA.Log.DebugWriteBBS("读取人数太多." + ex.Message);
            }
            return "";
        }
        protected override bool beforeInsert()
        {

            this.SetValByKey(BBSAttr.No, DBAccess.GenerGUID());

            this.SetValByKey(BBSAttr.Rec, BP.Web.WebUser.No);
            this.SetValByKey(BBSAttr.RecName, BP.Web.WebUser.Name);
            this.SetValByKey(BBSAttr.RecDeptNo, BP.Web.WebUser.FK_Dept);

            this.SetValByKey(BBSAttr.RDT, DataType.CurrentDateTime); //记录日期.
            this.SetValByKey(BBSAttr.NianYue, DataType.CurrentYearMonth);//隶属年月.

            this.SetValByKey(BBSAttr.RelerName, BP.Web.WebUser.Name);
            this.SetValByKey(BBSAttr.RelDeptName, BP.Web.WebUser.FK_DeptName);

            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                this.SetValByKey(BBSAttr.OrgNo, BP.Web.WebUser.OrgNo);


            return base.beforeInsert();
        }
        #endregion 执行方法.
    }
    /// <summary>
    /// 信息 s
    /// </summary>
    public class BBSs : EntitiesNoName
    {
        #region 构造函数.
        /// <summary>
        /// 信息
        /// </summary>
        public BBSs() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new BBS();
            }
        }
        #endregion 构造函数.

        public override int RetrieveAll()
        {
            return base.RetrieveAll();
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<BBS> ToJavaList()
        {
            return (System.Collections.Generic.IList<BBS>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<BBS> Tolist()
        {
            System.Collections.Generic.List<BBS> list = new System.Collections.Generic.List<BBS>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((BBS)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
