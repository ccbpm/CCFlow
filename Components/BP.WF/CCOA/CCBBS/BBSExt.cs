using System;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Sys;

namespace BP.CCOA.CCBBS
{

    /// <summary>
    /// 信息
    /// </summary>
    public class BBSExt : EntityNoName
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
                uac.IsInsert = false;
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
        public BBSExt()
        {
        }
        public BBSExt(string no)
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

                map.AddDDLEntities(BBSAttr.BBSType, null, "类型", new BBSTypes(), true);


                map.AddTBString(BBSAttr.Rec, null, "记录人", false, false, 0, 100, 10);
                map.AddTBString(BBSAttr.RecName, null, "记录人", false, true, 0, 100, 10, false);
                map.AddTBString(BBSAttr.RecDeptNo, null, "记录人部门", false, false, 0, 100, 10, false);


                map.AddTBString(BBSAttr.RelerName, null, "发布人", true, false, 0, 100, 10, false);
                map.AddTBString(BBSAttr.RelDeptName, null, "发布单位", true, false, 0, 100, 10, false);

                map.AddTBDateTime(BBSAttr.RDT, null, "发布日期", false, false);
                map.AddTBString(BBSAttr.NianYue, null, "隶属年月", false, false, 0, 10, 10);

                if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    map.AddTBString(BBSAttr.OrgNo, null, "组织", true, true, 0, 100, 10);



                //增加附件.
                map.AddMyFileS();

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行方法.
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
    public class BBSExts : EntitiesNoName
    {
        #region 构造函数.
        /// <summary>
        /// 信息
        /// </summary>
        public BBSExts() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new BBSExt();
            }
        }
        #endregion 构造函数.

        public override int RetrieveAll()
        {

            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                return this.Retrieve(BBSAttr.OrgNo, BP.Web.WebUser.OrgNo);

            return base.RetrieveAll();
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<BBSExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<BBSExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<BBSExt> Tolist()
        {
            System.Collections.Generic.List<BBSExt> list = new System.Collections.Generic.List<BBSExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((BBSExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
