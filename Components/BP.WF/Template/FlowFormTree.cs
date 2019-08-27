using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
    public class FlowFormTreeAttr : EntityTreeAttr
    {
        /// <summary>
        /// 流程
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 节点类型
        /// </summary>
        public const string NodeType = "NodeType";
        /// <summary>
        /// url
        /// </summary>
        public const string Url = "Url";
    }
    /// <summary>
    /// 独立表单树-用于数据解构构造
    /// </summary>
    public class FlowFormTree : EntityTree
    {
        #region 扩展属性，不做数据操作
        /// <summary>
        /// 节点类型
        /// </summary>
        public string NodeType { get; set; }
        /// <summary>
        /// 是否可编辑
        /// </summary>
        public string IsEdit { get; set; }
        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 打开时是否关闭其它的页面？
        /// </summary>
        public string IsCloseEtcFrm { get; set; }
        #endregion

        #region 属性
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(FrmNodeAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FK_Flow, value);
            }
        }
        #endregion 属性

        #region 构造方法
        /// <summary>
        /// 独立表单树
        /// </summary>
        public FlowFormTree()
        {
        }
        /// <summary>
        /// 独立表单树
        /// </summary>
        /// <param name="_No"></param>
        public FlowFormTree(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// 独立表单树Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_FormTree", "独立表单树-用于数据解构构造");
                map.Java_SetCodeStruct("2");;
                map.Java_SetDepositaryOfEntity(Depositary.Application);


                map.AddTBStringPK(FlowFormTreeAttr.No, null, "编号", true, true, 1, 10, 20);
                map.AddTBString(FlowFormTreeAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(FlowFormTreeAttr.ParentNo, null, "父节点No", false, false, 0, 100, 30);
                map.AddTBInt(FlowFormTreeAttr.Idx, 0, "Idx", false, false);

                // 隶属的流程编号.
                map.AddTBString(FlowFormTreeAttr.FK_Flow, null, "流程编号", true, true, 1, 20, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
    }
    /// <summary>
    /// 独立表单树
    /// </summary>
    public class FlowFormTrees : EntitiesTree
    {
        /// <summary>
        /// 独立表单树s
        /// </summary>
        public FlowFormTrees()
        {
        }
        /// <summary>
        /// 独立表单树
        /// </summary>
        public FlowFormTrees(string flowNo)
        {
           int i= this.Retrieve(FlowFormTreeAttr.FK_Flow, flowNo);
           if (i == 0)
           {
               FlowFormTree tree = new FlowFormTree();
               tree.No = "100";
               tree.FK_Flow = flowNo;
               tree.Name = "根目录";
              // tree.IsDir = false;
               tree.ParentNo = "0";
               tree.Insert();

               //创建一个节点.
               tree.DoCreateSubNode();
           }
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowFormTree();
            }

        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowFormTree> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowFormTree>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowFormTree> Tolist()
        {
            System.Collections.Generic.List<FlowFormTree> list = new System.Collections.Generic.List<FlowFormTree>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowFormTree)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
