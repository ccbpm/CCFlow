using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.Admin
{
    /// <summary>
    /// 流程目录属性
    /// </summary>
    public class FlowSortAttr : EntityTreeAttr
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
    ///  流程目录
    /// </summary>
    public class FlowSort : EntityNoName
    {
        #region 属性.
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(FlowSortAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(FlowSortAttr.OrgNo, value);
            }
        }
        public string Domain
        {
            get
            {
                return this.GetValStrByKey(FlowSortAttr.Domain);
            }
            set
            {
                this.SetValByKey(FlowSortAttr.Domain, value);
            }
        }
        public string ParentNo
        {
            get
            {
                return this.GetValStrByKey(FlowSortAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(FlowSortAttr.ParentNo, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 流程目录
        /// </summary>
        public FlowSort()
        {
        }
        /// <summary>
        /// 流程目录
        /// </summary>
        /// <param name="_No"></param>
        public FlowSort(string _No) : base(_No) { }
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
        /// 流程目录Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_FlowSort", "流程目录");

                map.AddTBStringPK(FlowSortAttr.No, null, "编号", false, false, 1, 100, 20);
                map.AddTBString(FlowSortAttr.ParentNo, null, "父节点No", false, false, 0, 100, 30);

                map.AddTBString(FlowSortAttr.Name, null, "名称", true, false, 0, 200, 30, true);
                map.AddTBString(FlowSortAttr.ShortName, null, "简称", true, false, 0, 200, 30, true);

                map.AddTBString(FlowSortAttr.OrgNo, "0", "组织编号(0为系统组织)", false, false, 0, 150, 30);
                map.SetHelperAlert(FlowSortAttr.OrgNo, "用于区分不同组织的的流程,比如:一个集团有多个子公司,每个子公司都有自己的业务流程.");
                map.AddTBString(FlowSortAttr.Domain, null, "域/系统编号", true, false, 0, 100, 30);
                map.SetHelperAlert(FlowSortAttr.Domain, "用于区分不同系统的流程,比如:一个集团有多个子系统每个子系统都有自己的流程,就需要标记那些流程是那个子系统的.");
                map.AddTBInt(FlowSortAttr.Idx, 0, "Idx", false, false);

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
            if (Glo.CCBPMRunModel != CCBPMRunModel.Single)
                this.OrgNo = BP.Web.WebUser.OrgNo;

            return base.beforeInsert();
        }
        protected override bool beforeUpdate()
        {
            //更新流程引擎控制表.
            string sql = "UPDATE WF_GenerWorkFlow SET Domain='" + this.Domain + "' WHERE FK_FlowSort='" + this.No + "'";
            DBAccess.RunSQL(sql);

            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "UPDATE WF_Emp SET StartFlows='' ";
            else
                sql = "UPDATE WF_Emp SET StartFlows='' ";

            DBAccess.RunSQL(sql);
            return base.beforeUpdate();
        }
        /// <summary>
        /// 删除之前的逻辑
        /// </summary>
        /// <returns></returns>
        protected override bool beforeDelete()
        {
            //检查是否有流程？
            Paras ps = new Paras();
            ps.SQL = "SELECT COUNT(*) FROM WF_Flow WHERE FK_FlowSort=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "fk_flowSort";
            ps.Add("fk_flowSort", this.No);
            if (DBAccess.RunSQLReturnValInt(ps) != 0)
                throw new Exception("err@该目录下有流程，您不能删除。");

            //检查是否有子目录？
            ps = new Paras();
            ps.SQL = "SELECT COUNT(*) FROM WF_FlowSort WHERE ParentNo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "ParentNo";
            ps.Add("ParentNo", this.No);
            //sql = "SELECT COUNT(*) FROM WF_FlowSort WHERE ParentNo='" + fk_flowSort + "'";
            if (DBAccess.RunSQLReturnValInt(ps) != 0)
                throw new Exception("err@该目录下有子目录，您不能删除...");

            return base.beforeDelete();
        }
    }
    /// <summary>
    /// 流程目录
    /// </summary>
    public class FlowSorts : EntitiesNoName
    {
        #region 构造.
        /// <summary>
        /// 流程目录s
        /// </summary>
        public FlowSorts() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowSort();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            if (Glo.CCBPMRunModel != CCBPMRunModel.Single)
                return this.Retrieve(FlowSortAttr.OrgNo, BP.Web.WebUser.OrgNo, FlowSortAttr.Idx);

            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FlowSortAttr.ParentNo, "!=", "0");
            qo.addOrderBy("Idx");
            int i = qo.DoQuery();

            if (i == 0)
            {
                FlowSort fs = new FlowSort();
                fs.Name = "流程树";
                fs.No = "100";
                fs.ParentNo = "0";
                fs.Insert();

                fs = new FlowSort();
                fs.Name = "公文类";
                fs.No = "01";
                fs.ParentNo = "100";
                fs.Insert();

                fs = new FlowSort();
                fs.Name = "办公类";
                fs.No = "02";
                fs.ParentNo = "100";
                fs.Insert();

                qo = new QueryObject(this);
                qo.AddWhere(FlowSortAttr.ParentNo, "!=", "");
                qo.addOrderBy("Idx");
                i = qo.DoQuery();
            }
            return i;
        }
        #endregion 构造.


        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowSort> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowSort>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowSort> Tolist()
        {
            System.Collections.Generic.List<FlowSort> list = new System.Collections.Generic.List<FlowSort>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowSort)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
