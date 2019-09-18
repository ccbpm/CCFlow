using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程类别属性
    /// </summary>
    public class FlowSortAttr : EntityTreeAttr
    {
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 域/系统编号
        /// </summary>
        public const string Domain = "Domain";
    }
    /// <summary>
    ///  流程类别
    /// </summary>
    public class FlowSort : EntityTree
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
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 流程类别
        /// </summary>
        public FlowSort()
        {
        }
        /// <summary>
        /// 流程类别
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
        /// 流程类别Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_FlowSort", "流程类别");

                map.AddTBStringPK(FlowSortAttr.No, null, "编号", true, true, 1, 100, 20);
                map.AddTBString(FlowSortAttr.ParentNo, null, "父节点No", true, true, 0, 100, 30);
                map.AddTBString(FlowSortAttr.Name, null, "名称", true, false, 0, 200, 30,true);
                map.AddTBString(FlowSortAttr.OrgNo, "0", "组织编号(0为系统组织)", true, false, 0, 150, 30);
                map.SetHelperAlert(FlowSortAttr.OrgNo, "用于区分不同组织的的流程,比如:一个集团有多个子公司,每个子公司都有自己的业务流程.");

                map.AddTBString(FlowSortAttr.Domain, null, "域/系统编号", true, false, 0, 100, 30);
                map.SetHelperAlert(FlowSortAttr.Domain, "用于区分不同系统的流程,比如:一个集团有多个子系统每个子系统都有自己的流程,就需要标记那些流程是那个子系统的.");
                map.AddTBInt(FlowSortAttr.Idx, 0, "Idx", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }

        protected override bool beforeUpdateInsertAction()
        {
            string sql = "UPDATE WF_GenerWorkFlow SET Domain='" + this.Domain + "' WHERE FK_FlowSort='" + this.No + "'";
            DBAccess.RunSQL(sql);

            //@sly
            sql = "UPDATE WF_Emp SET StartFlows='' ";
            DBAccess.RunSQL(sql);

            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 流程类别
    /// </summary>
    public class FlowSorts : EntitiesTree
    {
        /// <summary>
        /// 流程类别s
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
        public override int RetrieveAll()
        {
            int i = base.RetrieveAll( FlowSortAttr.Idx );
            if (i == 0)
            {
                FlowSort fs = new FlowSort();
                fs.Name = "公文类";
                fs.No = "01";
                fs.Insert();

                fs = new FlowSort();
                fs.Name = "办公类";
                fs.No = "02";
                fs.Insert();
                i = base.RetrieveAll();
            }

            return i;
        }


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
