using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Collections;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 节点属性.
    /// </summary>
    public class NodeSheet : Entity
    {
        #region 属性.
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID,value);
            }
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.Name);
            }
            set
            {
                this.SetValByKey(NodeAttr.Name, value);
            }
        }
        #endregion 属性.

        #region 构造函数
        /// <summary>
        /// 节点
        /// </summary>
        public NodeSheet() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                //map 的基 础信息.
                Map map = new Map("WF_Node", "节点");

                #region  基础属性
                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.SetHelperUrl(NodeAttr.NodeID, "http://ccbpm.mydoc.io/?v=5404&t=17901");
                map.AddTBInt(NodeAttr.Step, 0, "步骤(无计算意义)", true, false);
                map.SetHelperUrl(NodeAttr.Step, "http://ccbpm.mydoc.io/?v=5404&t=17902");
                //map.SetHelperAlert(NodeAttr.Step, "它用于节点的排序，正确的设置步骤可以让流程容易读写."); //使用alert的方式显示帮助信息.
                map.AddTBString(NodeAttr.FK_Flow, null, "流程编号", false, false, 3, 3, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=17023");
                map.AddTBString(NodeAttr.Name, null, "名称", true, true, 0, 100, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=17903");
                #endregion  基础属性

                #region 对应关系
                // 相关功能。
                if (BP.WF.Glo.OSModel == OSModel.OneOne)
                {
                    //平铺模式.
                    map.AttrsOfOneVSM.AddGroupPanelModel(new BP.WF.Template.NodeStations(), new BP.WF.Port.Stations(),
                        BP.WF.Template.NodeStationAttr.FK_Node,
                        BP.WF.Template.NodeStationAttr.FK_Station, "节点绑定岗位", StationAttr.FK_StationType);



                    map.AttrsOfOneVSM.AddGroupListModel(new BP.WF.Template.NodeStations(), new BP.WF.Port.Stations(),
                      BP.WF.Template.NodeStationAttr.FK_Node,
                      BP.WF.Template.NodeStationAttr.FK_Station, "节点绑定岗位", StationAttr.FK_StationType);

                    //判断是否为集团使用，集团时打开新页面以树形展示
                    if (BP.WF.Glo.IsUnit == true)
                    {
                        RefMethod rmDept = new RefMethod();
                        rmDept.Title = "节点绑定部门";
                        rmDept.ClassMethodName = this.ToString() + ".DoDepts";
                        rmDept.Icon = "../../Img/Btn/DTS.gif";
                        map.AddRefMethod(rmDept);
                    }
                    else
                    {
                        map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeDepts(), new BP.Port.Depts(), NodeDeptAttr.FK_Node, NodeDeptAttr.FK_Dept, DeptAttr.Name,
            DeptAttr.No, "节点绑定部门", Dot2DotModel.TreeDept);
                    }
                }
                else
                {
                    //节点岗位.
                    map.AttrsOfOneVSM.AddGroupPanelModel(new BP.WF.Template.NodeStations(),
                        new BP.GPM.Stations(),
                      NodeStationAttr.FK_Node, NodeStationAttr.FK_Station,
                       "节点绑定岗位", BP.GPM.StationAttr.FK_StationType);

                    //判断是否为集团使用，集团时打开新页面以树形展示
                    if (BP.WF.Glo.IsUnit == true)
                    {
                        RefMethod rmDept = new RefMethod();
                        rmDept.Title = "节点绑定部门";
                        rmDept.ClassMethodName = this.ToString() + ".DoDepts";
                        rmDept.Icon = "../../Img/Btn/DTS.gif";
                        map.AddRefMethod(rmDept);
                    }
                    else
                    {
                        //节点部门.
                        map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeDepts(), new BP.GPM.Depts(),
                            NodeDeptAttr.FK_Node, NodeDeptAttr.FK_Dept, DeptAttr.Name,
            DeptAttr.No, "节点绑定部门", Dot2DotModel.TreeDept);

                        //map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeDepts(), new BP.GPM.Depts(),
                        //NodeDeptAttr.FK_Node, NodeDeptAttr.FK_Dept, DeptAttr.Name,
                        //DeptAttr.No, "节点绑定部门", Dot2DotModel.TreeDept);
                    }
                }


                //节点绑定人员. 使用树杆与叶子的模式绑定.
                map.AttrsOfOneVSM.AddBranches(new BP.WF.Template.NodeDepts(), new BP.Port.Depts(),
                   BP.WF.Template.NodeDeptAttr.FK_Node,
                   BP.WF.Template.NodeDeptAttr.FK_Dept, "节点绑定部门AddBranches", EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");


                //节点绑定人员. 使用树杆与叶子的模式绑定.
                map.AttrsOfOneVSM.AddBranchesAndLeaf(new BP.WF.Template.NodeEmps(), new BP.Port.Emps(),
                   BP.WF.Template.NodeEmpAttr.FK_Node,
                   BP.WF.Template.NodeEmpAttr.FK_Emp, "节点绑定接受人", EmpAttr.FK_Dept, EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");

                //map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeEmps(),
                //new BP.Port.Emps(), NodeEmpAttr.FK_Node, NodeEmpAttr.FK_Emp, DeptAttr.Name,
                //DeptAttr.No, "节点绑定接受人");

                map.AddDtl(new NodeToolbars(), NodeToolbarAttr.FK_Node);

                // 傻瓜表单可以调用的子流程. 2014.10.19 去掉.
                //map.AttrsOfOneVSM.Add(new BP.WF.NodeFlows(), new Flows(), NodeFlowAttr.FK_Node, NodeFlowAttr.FK_Flow, DeptAttr.Name, DeptAttr.No,
                //    "傻瓜表单可调用的子流程");
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 节点集合
    /// </summary>
    public class NodeSheets : Entities
    {
        #region 构造方法
        /// <summary>
        /// 节点集合
        /// </summary>
        public NodeSheets()
        {
        }
        #endregion

        public override Entity GetNewEntity
        {
            get { return new NodeSheet(); }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<NodeSheet> ToJavaList()
        {
            return (System.Collections.Generic.IList<NodeSheet>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<NodeSheet> Tolist()
        {
            System.Collections.Generic.List<NodeSheet> list = new System.Collections.Generic.List<NodeSheet>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((NodeSheet)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
