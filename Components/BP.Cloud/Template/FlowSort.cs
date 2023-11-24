using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Cloud;
using BP.Sys;

namespace BP.Cloud.Template
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
                uac.IsDelete = true;
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

                map.AddTBStringPK(FlowSortAttr.No, null, "编号", false, true, 1, 100, 20);
                map.AddTBString(FlowSortAttr.ParentNo, null, "父节点No", false, false, 0, 100, 100);
                map.AddTBString(FlowSortAttr.Name, null, "名称", true, false, 1, 200, 400, true);
                map.AddTBString(FlowSortAttr.OrgNo, "0", "组织编号(0为系统组织)", false, false, 0, 150, 30);
                map.AddTBInt(FlowSortAttr.Idx, 0, "序号", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                this.No = DBAccess.GenerGUID();
            this.OrgNo = BP.Web.WebUser.OrgNo;
            this.ParentNo = this.OrgNo;
            this.Idx = 9999;
            return base.beforeInsert();
        }
        protected override void afterInsert()
        {
            //执行一次下移.
            this.DoDown();
            base.afterInsert();
        }

        protected override bool beforeDelete()
        {
            Flows fls = new Flows();
            int i = fls.Retrieve(FlowAttr.OrgNo, BP.Web.WebUser.OrgNo);
            if (i != 0)
                throw new Exception("err@您不能删除，该目录有流程模版.");

            return base.beforeDelete();
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
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAllFromDBSource()
        {
            return RetrieveAll();
        }
        public override int RetrieveAll()
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FlowAttr.OrgNo, BP.Web.WebUser.OrgNo);
            qo.addAnd();
            qo.AddWhere(FlowSortAttr.ParentNo, "!=","100");
            qo.addOrderBy(FlowSortAttr.Idx);
            int i = qo.DoQuery();
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
