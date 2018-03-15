using System;
using System.Collections.Generic;
using System.Text;
using BP.En;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.WF.Data;
using BP.WF.Template;

namespace BP.WF
{
    /// <summary>
    ///  属性
    /// </summary>
    public class GetTaskAttr
    {
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 审核节点
        /// </summary>
        public const string MainNode = "MainNode";
        /// <summary>
        /// 要审核的节点
        /// </summary>
        public const string CheckNodes = "CheckNodes";
    }
    /// <summary>
    /// 取回任务
    /// </summary>
    public class GetTask : BP.En.Entity
    {
        /// <summary>
        /// 我可以处理当前的工作吗？
        /// </summary>
        /// <returns></returns>
        public bool Can_I_Do_It()
        {
            /* 判断我是否可以处理当前点数据？ */
            switch (this.HisDeliveryWay)
            {
                case DeliveryWay.ByPreviousNodeFormEmpsField:
                    NodeEmps ndemps = new NodeEmps(this.NodeID);
                    if (ndemps.Contains(NodeEmpAttr.FK_Emp, WebUser.No) == false)
                        return false;
                    else
                        return true;
                case DeliveryWay.ByStation:
                    Stations sts = WebUser.HisStations;
                    string myStaStrs = "@";
                    foreach (Station st in sts)
                        myStaStrs += "@" + st.No;
                    myStaStrs = myStaStrs + "@";

                    NodeStations ndeStas = new NodeStations(this.NodeID);
                    bool isHave = false;
                    foreach (NodeStation ndS in ndeStas)
                    {
                        if (myStaStrs.Contains("@" + ndS.FK_Station + "@") == true)
                        {
                            isHave = true;
                            break;
                        }
                    }
                    if (isHave == false)
                        return false;
                    return true;
                default: /* 其它的情况则不与判断。*/
                    // jc.Delete(); // 设置是非法的，直接删除。
                    return false;
                    break;
            }
        }

        #region attrs
        /// <summary>
        /// 投递方式
        /// </summary>
        public DeliveryWay HisDeliveryWay
        {
            get
            {
                return (DeliveryWay)this.GetValIntByKey(NodeAttr.DeliveryWay);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
            }
        }
        /// <summary>
        /// 名称
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
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(GetTaskAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(GetTaskAttr.FK_Flow, value);
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
        }
        public string CheckNodes
        {
            get
            {
                string s= this.GetValStringByKey(GetTaskAttr.CheckNodes);
                s = s.Replace("~", "'");
                return s;
            }
            set
            {
                this.SetValByKey(GetTaskAttr.CheckNodes, value);
            }
        }
        #endregion attrs

        #region 属性
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Node", "取回任务");

                #region 字段
                map.AddTBIntPK(NodeAttr.NodeID, 0,"NodeID", true, true);
                map.AddTBString(NodeAttr.Name, null,"节点名称", true, false, 0, 100, 10);
                map.AddTBInt(NodeAttr.Step,0, "步骤", true, false);
                map.AddTBString(NodeAttr.FK_Flow, null, "流程编号", true, false, 0, 10, 10);
                map.AddTBString(GetTaskAttr.CheckNodes, null, "工作节点s", true, false, 0, 50, 100);

                map.AddTBInt(NodeAttr.DeliveryWay, 0, "访问规则", true, true);

                #endregion 字段

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 取回任务
        /// </summary>
        public GetTask()
        {
        }
        public GetTask(int nodeId)
        {
            this.NodeID = nodeId;
            this.Retrieve();
        }
        #endregion attrs
    }
    /// <summary>
    /// 取回任务集合
    /// </summary>
    public class GetTasks : BP.En.Entities
    {
        public GetTasks()
        {
        }
        /// <summary>
        /// 取回任务集合
        /// </summary>
        public GetTasks(string fk_flow)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(GetTaskAttr.FK_Flow, fk_flow);
            qo.addAnd();
            qo.AddWhereLen(GetTaskAttr.CheckNodes, " >= ", 3, SystemConfig.AppCenterDBType);
            qo.DoQuery();
        }
        /// <summary>
        /// 获得实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GetTask();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GetTask> ToJavaList()
        {
            return (System.Collections.Generic.IList<GetTask>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GetTask> Tolist()
        {
            System.Collections.Generic.List<GetTask> list = new System.Collections.Generic.List<GetTask>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GetTask)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
