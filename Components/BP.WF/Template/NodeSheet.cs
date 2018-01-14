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
        /// 步骤
        /// </summary>
        public int Step
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.Step);
            }
            set
            {
                this.SetValByKey(NodeAttr.Step, value);
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

                map.AddTBString(NodeAttr.Tip, null, "操作提示", true, false, 0, 100, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=18084");

                string str = "";
                // str += "@0=01.按岗位智能计算";
                str += "@0=01.按岗位智能计算";
                str += "@1=02.按节点绑定的部门计算";
                str += "@2=03.按设置的SQL获取接受人计算";
                str += "@3=04.按节点绑定的人员计算";
                str += "@4=05.由上一节点发送人通过“人员选择器”选择接受人";
                str += "@5=06.按上一节点表单指定的字段值作为本步骤的接受人";
                str += "@6=07.与上一节点处理人员相同";
                str += "@7=08.与开始节点处理人相同";
                str += "@8=09.与指定节点处理人相同";
                str += "@9=10.按绑定的岗位与部门交集计算";
                str += "@10=11.按绑定的岗位计算并且以绑定的部门集合为纬度";
                str += "@11=12.按指定节点的工作人员或者指定字段人员的岗位计算";
                str += "@12=13.按SQL确定子线程接受人与数据源";
                str += "@13=14.由上一节点的明细表来决定子线程的接受人";
                str += "@14=15.仅按绑定的岗位计算";
                str += "@15=16.由FEE来决定";
                str += "@16=17.按绑定部门计算,该部门一人处理标识该工作结束(子线程).";
                str += "@100=18.按ccflow的BPM模式处理";
                map.AddDDLSysEnum(NodeAttr.DeliveryWay, 0, "节点访问规则", true, true, NodeAttr.DeliveryWay, str);
                map.SetHelperUrl(NodeAttr.DeliveryWay, "http://ccbpm.mydoc.io/?v=5404&t=17907");
                map.AddBoolean(NodeAttr.IsExpSender, true, "本节点接收人不允许包含上一步发送人?", true, true, true, "http://ccbpm.mydoc.io/?v=5404&t=17904");

                map.AddTBString(NodeAttr.DeliveryParas, null, "访问规则设置内容",
                    true, false, 0, 500, 10, true, "http://ccbpm.mydoc.io/?v=5404&t=17907");

                map.AddDDLSysEnum(NodeAttr.WhoExeIt, 0, "谁执行它", true, true, NodeAttr.WhoExeIt, "@0=操作员执行@1=机器执行@2=混合执行");
                map.SetHelperUrl(NodeAttr.WhoExeIt, "http://ccbpm.mydoc.io/?v=5404&t=17913");

                map.AddDDLSysEnum(NodeAttr.TurnToDeal, 0, "发送后转向",
                 true, true, NodeAttr.TurnToDeal, "@0=提示ccflow默认信息@1=提示指定信息@2=转向指定的url@3=按照条件转向");
                map.SetHelperUrl(NodeAttr.TurnToDeal, "http://ccbpm.mydoc.io/?v=5404&t=17914");
                map.AddTBString(NodeAttr.TurnToDealDoc, null, "转向处理内容", true, false, 0, 1000, 10, true, "http://ccbpm.mydoc.io/?v=5404&t=17914");
                map.AddDDLSysEnum(NodeAttr.ReadReceipts, 0, "已读回执", true, true, NodeAttr.ReadReceipts,
                    "@0=不回执@1=自动回执@2=由上一节点表单字段决定@3=由SDK开发者参数决定");
                map.SetHelperUrl(NodeAttr.ReadReceipts, "http://ccbpm.mydoc.io/?v=5404&t=17915");


                map.AddDDLSysEnum(NodeAttr.CondModel, 0, "方向条件控制规则", true, true, NodeAttr.CondModel,
                 "@0=由连接线条件控制@1=让用户手工选择@2=发送按钮旁下拉框选择");
                map.SetHelperUrl(NodeAttr.CondModel, "http://ccbpm.mydoc.io/?v=5404&t=17917"); //增加帮助

                // 撤销规则.
                map.AddDDLSysEnum(NodeAttr.CancelRole, (int)CancelRole.OnlyNextStep, "撤销规则", true, true,
                    NodeAttr.CancelRole, "@0=上一步可以撤销@1=不能撤销@2=上一步与开始节点可以撤销@3=指定的节点可以撤销");
                map.SetHelperUrl(NodeAttr.CancelRole, "http://ccbpm.mydoc.io/?v=5404&t=17919");

                // 节点工作批处理. edit by peng, 2014-01-24. 
                map.AddDDLSysEnum(NodeAttr.BatchRole, (int)BatchRole.None, "工作批处理", true, true, NodeAttr.BatchRole, "@0=不可以批处理@1=批量审核@2=分组批量审核");
                map.AddTBInt(NodeAttr.BatchListCount, 12, "批处理数量", true, false);
                //map.SetHelperUrl(NodeAttr.BatchRole, this[SYS_CCFLOW, "节点工作批处理"]); //增加帮助
                map.SetHelperUrl(NodeAttr.BatchRole, "http://ccbpm.mydoc.io/?v=5404&t=17920");
                map.SetHelperUrl(NodeAttr.BatchListCount, "http://ccbpm.mydoc.io/?v=5404&t=17920");

                map.AddTBString(NodeAttr.BatchParas, null, "批处理参数", true, false, 0, 300, 10, true);
                map.SetHelperUrl(NodeAttr.BatchParas, "http://ccbpm.mydoc.io/?v=5404&t=17920");

                map.AddBoolean(NodeAttr.IsTask, true, "允许分配工作否?", true, true, false, "http://ccbpm.mydoc.io/?v=5404&t=17904");
                map.AddBoolean(NodeAttr.IsRM, true, "是否启用投递路径自动记忆功能?", true, true, false, "http://ccbpm.mydoc.io/?v=5404&t=17905");

                map.AddTBDateTime("DTFrom", "生命周期从", true, true);
                map.AddTBDateTime("DTTo", "生命周期到", true, true);

                map.AddBoolean(NodeAttr.IsBUnit, false, "是否是节点模版（业务单元）?", true, true, true, "http://ccbpm.mydoc.io/?v=5404&t=17904");
                #endregion  基础属性

                #region 对应关系

                // 相关功能。
                if (BP.WF.Glo.OSModel == OSModel.OneOne)
                {
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
                    }
                }

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
