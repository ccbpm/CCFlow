using System;
using System.Collections.Generic;
using System.Text;
using BP.En;
using BP.Sys;

namespace BP.WF.Data
{
    /// <summary>
    ///  属性 
    /// </summary>
    public class FlowDataAttr  
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 参与成员
        /// </summary>
        public const string FlowEmps = "FlowEmps";
        /// <summary>
        /// 流程ID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string OID = "OID";
        /// <summary>
        /// 年月
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// 流程发起人
        /// </summary>
        public const string FlowStarter = "FlowStarter";
        /// <summary>
        /// 流程发起时间
        /// </summary>
        public const string FlowStartRDT = "FlowStartRDT";
        /// <summary>
        /// 部门发起部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 流程数量
        /// </summary>
        public const string WFState = "WFState";
        /// <summary>
        /// 个数(统计分析用)
        /// </summary>
        public const string MyNum = "MyNum";
        /// <summary>
        /// 结束人
        /// </summary>
        public const string FlowEnder = "FlowEnder";
        /// <summary>
        /// 流程最后处理时间
        /// </summary>
        public const string FlowEnderRDT = "FlowEnderRDT";
        /// <summary>
        /// 跨度
        /// </summary>
        public const string FlowDaySpan = "FlowDaySpan";
        /// <summary>
        /// 流程结束节点
        /// </summary>
        public const string FlowEndNode = "FlowEndNode";
    }
    /// <summary>
    /// 报表
    /// </summary>
    public class FlowData : BP.En.EntityOID
    {
        #region attrs
        /// <summary>
        /// 流程发起人
        /// </summary>
        public string FlowStarter
        {
            get
            {
                return this.GetValStringByKey(FlowDataAttr.FlowStarter);
            }
            set
            {
                this.SetValByKey(FlowDataAttr.FlowStarter, value);
            }
        }
        /// <summary>
        /// 流程发起时间
        /// </summary>
        public string FlowStartRDT
        {
            get
            {
                return this.GetValStringByKey(FlowDataAttr.FlowStartRDT);
            }
            set
            {
                this.SetValByKey(FlowDataAttr.FlowStartRDT, value);
            }
        }
        /// <summary>
        /// 流程结束者
        /// </summary>
        public string FlowEnder
        {
            get
            {
                return this.GetValStringByKey(FlowDataAttr.FlowEnder);
            }
            set
            {
                this.SetValByKey(FlowDataAttr.FlowEnder, value);
            }
        }
        /// <summary>
        /// 流程最后处理时间
        /// </summary>
        public string FlowEnderRDT
        {
            get
            {
                return this.GetValStringByKey(FlowDataAttr.FlowEnderRDT);
            }
            set
            {
                this.SetValByKey(FlowDataAttr.FlowEnderRDT, value);
            }
        }
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(FlowDataAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(FlowDataAttr.FK_Dept, value);
            }
        }
        public int WFState
        {
            get
            {
                return this.GetValIntByKey(FlowDataAttr.WFState);
            }
            set
            {
                this.SetValByKey(FlowDataAttr.WFState, value);
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(FlowDataAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(FlowDataAttr.FK_Flow, value);
            }
        }
        #endregion attrs

        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                return uac;
            }
        }

        #region attrs - attrs 
        public string RptName = null;
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("V_FlowData", "流程数据");
                map.Java_SetEnType(EnType.View);

                map.AddTBIntPKOID(FlowDataAttr.OID, "WorkID");
                map.AddTBInt(FlowDataAttr.FID, 0, "FID", false, false);

                map.AddDDLEntities(FlowDataAttr.FK_Dept, null, "部门", new Port.Depts(), false);
                map.AddTBString(FlowDataAttr.Title, null, "标题", true, true, 0, 100, 100,true);
                map.AddTBString(FlowDataAttr.FlowStarter, null, "发起人", true, true, 0, 100, 100);
                map.AddTBDateTime(FlowDataAttr.FlowStartRDT, null, "发起时间", true, true);
                map.AddDDLSysEnum(FlowDataAttr.WFState, 0, "流程状态", true, true, "WFStateApp");
                map.AddDDLEntities(FlowDataAttr.FK_NY, null, "年月", new BP.Pub.NYs(), false);
                map.AddDDLEntities(FlowDataAttr.FK_Flow, null, "流程", new Flows(), false);
                map.AddTBDateTime(FlowDataAttr.FlowEnderRDT, null, "最后处理时间", true, true);
                map.AddTBInt(FlowDataAttr.FlowEndNode, 0, "结束节点", true, true);
                map.AddTBInt(FlowDataAttr.FlowDaySpan, 0, "跨度(天)", true, true);
                map.AddTBInt(FlowDataAttr.MyNum, 1, "个数", true, true);
                map.AddTBString(FlowDataAttr.FlowEmps, null, "参与人", false, false, 0, 100, 100);

                map.AddSearchAttr(FlowDataAttr.FK_NY);
                map.AddSearchAttr(FlowDataAttr.WFState);
                map.AddSearchAttr(FlowDataAttr.FK_Flow);

                map.AddHidden(FlowDataAttr.FlowEmps, " LIKE ", "'%@@WebUser.No%'");

                RefMethod rm = new RefMethod();
                rm.Title = "流程运行轨迹";
                rm.ClassMethodName = this.ToString() + ".DoOpen";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion attrs

        public string DoOpen()
        {
            PubClass.WinOpen(Glo.CCFlowAppPath+"WF/WorkOpt/OneWork/Track.aspx?WorkID=" + this.OID + "&FK_Flow=" + this.FK_Flow, 900, 600);
            return null;
        }
    }
    /// <summary>
    /// 报表集合
    /// </summary>
    public class FlowDatas : BP.En.EntitiesOID
    {
        /// <summary>
        /// 报表集合
        /// </summary>
        public FlowDatas()
        {
        }

        public override Entity GetNewEntity
        {
            get
            {
                return new FlowData();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowData> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowData>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowData> Tolist()
        {
            System.Collections.Generic.List<FlowData> list = new System.Collections.Generic.List<FlowData>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowData)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
