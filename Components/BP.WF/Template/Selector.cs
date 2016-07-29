using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 方向条件控制规则
    /// </summary>
    public enum CondModel
    {
        /// <summary>
        /// 按照用户设置的方向条件计算
        /// </summary>
        ByLineCond,
        /// <summary>
        /// 按照用户选择计算
        /// </summary>
        ByUserSelected
    }
    /// <summary>
    /// 关系类型
    /// </summary>
    public enum CondOrAnd
    {
        /// <summary>
        /// 关系集合里面的所有条件都成立.
        /// </summary>
        ByAnd,
        /// <summary>
        /// 关系集合里的只有一个条件成立.
        /// </summary>
        ByOr
    }
    /// <summary>
    /// 待办工作超时处理方式
    /// </summary>
    public enum OutTimeDeal
    {
        /// <summary>
        /// 不处理
        /// </summary>
        None,
        /// <summary>
        /// 自动的转向下一步骤
        /// </summary>
        AutoTurntoNextStep,
        /// <summary>
        /// 自动跳转到指定的点
        /// </summary>
        AutoJumpToSpecNode,
        /// <summary>
        /// 自动移交到指定的人员
        /// </summary>
        AutoShiftToSpecUser,
        /// <summary>
        /// 向指定的人员发送消息
        /// </summary>
        SendMsgToSpecUser,
        /// <summary>
        /// 删除流程
        /// </summary>
        DeleteFlow,
        /// <summary>
        /// 执行SQL
        /// </summary>
        RunSQL
        ///// <summary>
        ///// 到达指定的日期，仍未处理，自动向下发送.
        ///// </summary>
        //WhenToSpecDataAutoSend
    }
    /// <summary>
    /// 显示方式
    /// </summary>
    public enum SelectorDBShowWay
    {
        /// <summary>
        /// 表格
        /// </summary>
        Table,
        /// <summary>
        /// 树
        /// </summary>
        Tree
    }
    public enum SelectorModel
    {
        /// <summary>
        /// 表格
        /// </summary>
        Station,
        /// <summary>
        /// 树
        /// </summary>
        Dept,
        /// <summary>
        /// 操作员
        /// </summary>
        Emp,
        /// <summary>
        /// SQL
        /// </summary>
        SQL,
        /// <summary>
        /// 自定义链接
        /// </summary>
        Url
    }
    /// <summary>
    /// Selector属性
    /// </summary>
    public class SelectorAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 流程
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 接受模式
        /// </summary>
        public const string SelectorModel = "SelectorModel";

        public const string SelectorP1 = "SelectorP1";
        public const string SelectorP2 = "SelectorP2";
        /// <summary>
        /// 数据显示方式(表格与树)
        /// </summary>
        public const string SelectorDBShowWay = "SelectorDBShowWay";
    }
    /// <summary>
    /// 选择器
    /// </summary>
    public class Selector : Entity
    {
        #region 基本属性
        public override string PK
        {
            get
            {
                return "NodeID";
            }
        }
        /// <summary>
        /// 显示方式
        /// </summary>
        public SelectorDBShowWay SelectorDBShowWay
        {
            get
            {
                return (SelectorDBShowWay)this.GetValIntByKey(SelectorAttr.SelectorDBShowWay);
            }
            set
            {
                this.SetValByKey(SelectorAttr.SelectorDBShowWay, (int)value);
            }
        }
        /// <summary>
        /// 选择模式
        /// </summary>
        public SelectorModel SelectorModel
        {
            get
            {
                return (SelectorModel)this.GetValIntByKey(SelectorAttr.SelectorModel);
            }
            set
            {
                this.SetValByKey(SelectorAttr.SelectorModel, (int)value);
            }
        }

        public string SelectorP1
        {
            get
            {
                string s= this.GetValStringByKey(SelectorAttr.SelectorP1);
                s = s.Replace("~", "'");
                return s;
            }
            set
            {
                this.SetValByKey(SelectorAttr.SelectorP1, value);
            }
        }
        public string SelectorP2
        {
            get
            {
                string s = this.GetValStringByKey(SelectorAttr.SelectorP2);
                s = s.Replace("~", "'");
                return s;
                //return this.GetValStringByKey(SelectorAttr.SelectorP2);
            }
            set
            {
                this.SetValByKey(SelectorAttr.SelectorP2, value);
            }
        }
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(SelectorAttr.NodeID);
            }
            set
            {
                this.SetValByKey(SelectorAttr.NodeID, value);
            }
        }
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsView = false;
                if (BP.Web.WebUser.No == "admin")
                {
                    uac.IsUpdate = true;
                    uac.IsView = true;
                }
                return uac;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// Accpter
        /// </summary>
        public Selector() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeid"></param>
        public Selector(int nodeid)
        {
            this.NodeID = nodeid;
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

                Map map = new Map("WF_Node", "选择器");

                map.Java_SetDepositaryOfEntity(Depositary.Application);


                map.AddTBIntPK(SelectorAttr.NodeID, 0, "NodeID", true, true);
                map.AddTBString(SelectorAttr.Name, null, "节点名称", true, true, 0,100,100);

                map.AddDDLSysEnum(SelectorAttr.SelectorDBShowWay, 0, "数据显示方式", true, true,
                SelectorAttr.SelectorDBShowWay, "@0=表格显示@1=树形显示");

                map.AddDDLSysEnum(SelectorAttr.SelectorModel, 0, "窗口模式", true, true, SelectorAttr.SelectorModel,
                    "@0=按岗位@1=按部门@2=按人员@3=按SQL@4=自定义Url");


                //map.AddTBString(SelectorAttr.SelectorP1, null, "参数1", true, false, 0, 500, 10, true);
                //map.AddTBString(SelectorAttr.SelectorP2, null, "参数2", true, false, 0, 500, 10, true);

                map.AddTBStringDoc(SelectorAttr.SelectorP1, null, "参数1", true, false, true);
                map.AddTBStringDoc(SelectorAttr.SelectorP2, null, "参数2", true, false, true);

            

                // 相关功能。
                map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeStations(), new BP.WF.Port.Stations(),
                    NodeStationAttr.FK_Node, NodeStationAttr.FK_Station,
                    DeptAttr.Name, DeptAttr.No, "节点岗位");

                map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeDepts(), new BP.WF.Port.Depts(), NodeDeptAttr.FK_Node, NodeDeptAttr.FK_Dept, DeptAttr.Name,
                DeptAttr.No, "节点部门");

                map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeEmps(), new BP.WF.Port.Emps(), NodeEmpAttr.FK_Node, NodeEmpAttr.FK_Emp, DeptAttr.Name,
                    DeptAttr.No, "接受人员");


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// Accpter
    /// </summary>
    public class Selectors : Entities
    {
        /// <summary>
        /// Accpter
        /// </summary>
        public Selectors()
        {
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Selector();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Selector> ToJavaList()
        {
            return (System.Collections.Generic.IList<Selector>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Selector> Tolist()
        {
            System.Collections.Generic.List<Selector> list = new System.Collections.Generic.List<Selector>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Selector)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
