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
                //超级用户必须是admin.
                if (BP.Web.WebUser.No == "admin")
                    this.SetValByKey(FlowSortAttr.OrgNo, "0");
                else
                    this.SetValByKey(FlowSortAttr.OrgNo, value);
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
                map.AddTBString(FlowSortAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(FlowSortAttr.ParentNo, null, "父节点No", false, false, 0, 100, 30);
                map.AddTBString(FlowSortAttr.TreeNo, null, "树节点编号", false, false, 0, 100, 30);
                map.AddTBString(FlowSortAttr.OrgNo, "0", "组织编号(0为系统组织)", false, false, 0, 50, 30);

                map.AddTBInt(FlowSortAttr.Idx, 0, "Idx", false, false);
                map.AddTBInt(FlowSortAttr.IsDir, 0, "IsDir", false, false);

                this._enMap = map;
                return this._enMap;
            }
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
