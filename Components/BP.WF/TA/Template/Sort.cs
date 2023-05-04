using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.Difference;
namespace BP.TA
{
    /// <summary>
    /// 流程类别属性
    /// </summary>
    public class SortAttr : EntityTreeAttr
    {
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 简称
        /// </summary>
        public const string ShortName = "ShortName";
        /// <summary>
        /// 域/系统编号
        /// </summary>
        public const string Domain = "Domain";
    }
    /// <summary>
    ///  流程类别
    /// </summary>
    public class Sort : EntityTree
    {
        #region 属性.
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(SortAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(SortAttr.OrgNo, value);
            }
        }
        public string Domain
        {
            get
            {
                return this.GetValStrByKey(SortAttr.Domain);
            }
            set
            {
                this.SetValByKey(SortAttr.Domain, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 流程类别
        /// </summary>
        public Sort()
        {
        }
        /// <summary>
        /// 流程类别
        /// </summary>
        /// <param name="_No"></param>
        public Sort(string _No) : base(_No) { }
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        #endregion

        /// <summary>
        /// 流程类别Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("TA_Sort", "流程类别");

                map.AddTBStringPK(SortAttr.No, null, "编号", false, false, 1, 100, 20);
                map.AddTBString(SortAttr.ParentNo, null, "父节点No", false, false, 0, 100, 30);

                map.AddTBString(SortAttr.Name, null, "名称", true, false, 0, 200, 30, true);
                map.AddTBString(SortAttr.ShortName, null, "简称", true, false, 0, 200, 30, true);
                map.AddTBString(SortAttr.OrgNo, "0", "组织编号(0为系统组织)", false, false, 0, 150, 30);
                map.SetHelperAlert(SortAttr.OrgNo, "用于区分不同组织的的流程,比如:一个集团有多个子公司,每个子公司都有自己的业务流程.");

                map.AddTBString(SortAttr.Domain, null, "域/系统编号", true, false, 0, 100, 30);
                map.SetHelperAlert(SortAttr.Domain, "用于区分不同系统的流程,比如:一个集团有多个子系统每个子系统都有自己的流程,就需要标记那些流程是那个子系统的.");
                map.AddTBInt(SortAttr.Idx, 0, "目录显示顺序(发起列表)", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }

        /// <summary>
        /// 创建的时候，给他增加一个OrgNo。
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                this.OrgNo = BP.Web.WebUser.OrgNo;

            return base.beforeInsert();
        }
        protected override bool beforeUpdate()
        {
            ////更新流程引擎控制表.
            //string sql = "UPDATE WF_GenerWorkFlow SET Domain='" + this.Domain + "' WHERE FK_Sort='" + this.No + "'";
            //DBAccess.RunSQL(sql);

            //// sql = "UPDATE WF_Flow SET Domain='" + this.Domain + "' WHERE FK_Sort='" + this.No + "'";
            ////DBAccess.RunSQL(sql);

            //if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
            //    sql = "UPDATE WF_Emp SET StartFlows='' ";
            //else
            //    sql = "UPDATE WF_Emp SET StartFlows='' ";
           // DBAccess.RunSQL(sql);
            return base.beforeUpdate();
        }
        /// <summary>
        /// 删除之前的逻辑
        /// </summary>
        /// <returns></returns>
        protected override bool beforeDelete()
        {
            ////检查是否有流程？
            //Paras ps = new Paras();
            //ps.SQL = "SELECT COUNT(*) FROM TA_Template WHERE Sort=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "fk_Sort";
            //ps.Add("SortNo", this.No);
            ////string sql = "SELECT COUNT(*) FROM WF_Flow WHERE FK_Sort='" + fk_Sort + "'";
            //if (DBAccess.RunSQLReturnValInt(ps) != 0)
            //    throw new Exception("err@该目录下有流程，您不能删除。");

            ////检查是否有子目录？
            //ps = new Paras();
            //ps.SQL = "SELECT COUNT(*) FROM WF_Sort WHERE ParentNo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "ParentNo";
            //ps.Add("ParentNo", this.No);
            ////sql = "SELECT COUNT(*) FROM WF_Sort WHERE ParentNo='" + fk_Sort + "'";
            //if (DBAccess.RunSQLReturnValInt(ps) != 0)
            //    throw new Exception("err@该目录下有子目录，您不能删除...");

            return base.beforeDelete();
        }
    }
    /// <summary>
    /// 流程类别
    /// </summary>
    public class Sorts : EntitiesTree
    {
        /// <summary>
        /// 流程类别s
        /// </summary>
        public Sorts() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Sort();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                return this.Retrieve(SortAttr.OrgNo, BP.Web.WebUser.OrgNo, SortAttr.Idx);

            int i = base.RetrieveAll(SortAttr.Idx);
            if (i == 0)
            {
                Sort fs = new Sort();
                fs.Name = "流程树";
                fs.No = "1";
                fs.ParentNo = "0";
                fs.Insert();

                fs = new Sort();
                fs.Name = "公文类";
                fs.No = "01";
                fs.ParentNo = "1";
                fs.Insert();

                fs = new Sort();
                fs.Name = "办公类";
                fs.No = "02";
                fs.ParentNo = "1";
                fs.Insert();
                i = base.RetrieveAll(SortAttr.Idx);
            }
            return i;
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Sort> ToJavaList()
        {
            return (System.Collections.Generic.IList<Sort>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Sort> Tolist()
        {
            System.Collections.Generic.List<Sort> list = new System.Collections.Generic.List<Sort>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Sort)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
