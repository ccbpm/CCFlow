﻿using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.WF.Template;
using BP.WF;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 子流程模式
    /// </summary>
    public enum SubFlowModel
    {
        /// <summary>
        /// 下级
        /// </summary>
        SubLevel,
        /// <summary>
        /// 同级
        /// </summary>
        SameLevel
    }
    /// <summary>
    /// 子流程类型
    /// </summary>
    public enum SubFlowType
    {
        /// <summary>
        /// 手动的子流程
        /// </summary>
        HandSubFlow = 0,
        /// <summary>
        /// 自动触发的子流程
        /// </summary>
        AutoSubFlow = 1,
        /// <summary>
        /// 延续子流程
        /// </summary>
        YanXuFlow = 2
    }
    /// <summary>
    /// 父子流程控件状态
    /// </summary>
    public enum FrmSubFlowSta
    {
        /// <summary>
        /// 不可用
        /// </summary>
        Disable,
        /// <summary>
        /// 可用
        /// </summary>
        Enable,
        /// <summary>
        /// 只读
        /// </summary>
        Readonly
    }
    /// <summary>
    /// 父子流程
    /// </summary>
    public class FrmSubFlowAttr : EntityNoAttr
    {
        /// <summary>
        /// 标签
        /// </summary>
        public const string SFLab = "SFLab";
        /// <summary>
        /// 状态
        /// </summary>
        public const string SFSta = "SFSta";
        /// <summary>
        /// X
        /// </summary>
        public const string SF_X = "SF_X";
        /// <summary>
        /// Y
        /// </summary>
        public const string SF_Y = "SF_Y";
        /// <summary>
        /// H
        /// </summary>
        public const string SF_H = "SF_H";
        /// <summary>
        /// W
        /// </summary>
        public const string SF_W = "SF_W";
        /// <summary>
        /// 应用类型
        /// </summary>
        public const string SFType = "SFType";
        /// <summary>
        /// 附件
        /// </summary>
        public const string SFAth = "SFAth";
        /// <summary>
        /// 显示方式.
        /// </summary>
        public const string SFShowModel = "SFShowModel";
        /// <summary>
        /// 轨迹图是否显示?
        /// </summary>
        public const string SFTrackEnable = "SFTrackEnable";
        /// <summary>
        /// 历史审核信息是否显示?
        /// </summary>
        public const string SFListEnable = "SFListEnable";
        /// <summary>
        /// 是否显示所有的步骤？
        /// </summary>
        public const string SFIsShowAllStep = "SFIsShowAllStep";
        /// <summary>
        /// 默认审核信息
        /// </summary>
        public const string SFDefInfo = "SFDefInfo";
        /// <summary>
        /// 触发的流程
        /// </summary>
        public const string SFActiveFlows = "SFActiveFlows";
        /// <summary>
        /// 标题
        /// </summary>
        public const string SFCaption = "SFCaption";
        /// <summary>
        /// 如果用户未审核是否按照默认意见填充？
        /// </summary>
        public const string SFIsFullInfo = "SFIsFullInfo";
        /// <summary>
        /// 操作名词(审核，审定，审阅，批示)
        /// </summary>
        public const string SFOpLabel = "SFOpLabel";
        /// <summary>
        /// 操作人是否显示数字签名
        /// </summary>
        public const string SigantureEnabel = "SigantureEnabel";
        /// <summary>
        /// 操作字段
        /// </summary>
        public const string SFFields = "SFFields";
        /// <summary>
        /// 显示控制方式
        /// </summary>
        public const string SFShowCtrl = "SFShowCtrl";
        /// <summary>
        /// 查看类型
        /// </summary>
        public const string SFOpenType = "SFOpenType";
    }
    /// <summary>
    /// 父子流程
    /// </summary>
    public class FrmSubFlow : Entity
    {
        #region 属性
        /// <summary>
        /// 标签
        /// </summary>
        public string SFLab
        {
            get
            {
                return this.GetValStringByKey(FrmSubFlowAttr.SFLab);
            }
        }
        /// <summary>
        /// 编号
        /// </summary>
        public string No
        {
            get
            {
                return "ND" + this.NodeID;
            }
            set
            {
                string nodeID = value.Replace("ND", "");
                this.NodeID = int.Parse(nodeID);
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
        /// 可触发的子流程
        /// </summary>
        public string SFActiveFlows
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.SFActiveFlows);
            }
            set
            {
                this.SetValByKey(NodeAttr.SFActiveFlows, value);
            }
        }
        /// <summary>
        /// 字段列
        /// </summary>
        public string SFFields
        {
            get
            {
                return this.GetValStringByKey(FrmSubFlowAttr.SFFields);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFFields, value);
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public FrmSubFlowSta HisFrmSubFlowSta
        {
            get
            {
                return (FrmSubFlowSta)this.GetValIntByKey(FrmSubFlowAttr.SFSta);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFSta, (int)value);
            }
        }
        /// <summary>
        /// 显示控制方式
        /// </summary>
        public SFShowCtrl SFShowCtrl
        {
            get
            {
                return (SFShowCtrl)this.GetValIntByKey(FrmSubFlowAttr.SFShowCtrl);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFShowCtrl, (int)value);
            }
        }
        /// <summary>
        /// 显示格式(0=表格,1=自由.)
        /// </summary>
        public FrmWorkShowModel HisFrmWorkShowModel
        {
            get
            {
                return (FrmWorkShowModel)this.GetValIntByKey(FrmSubFlowAttr.SFShowModel);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFShowModel, (int)value);
            }
        }
        /// <summary>
        /// 控件状态
        /// </summary>
        public FrmSubFlowSta SFSta
        {
            get
            {
                return (FrmSubFlowSta)this.GetValIntByKey(FrmSubFlowAttr.SFSta);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFSta, (int)value);
            }
        }
        /// <summary>
        /// 显示方式
        /// </summary>
        public FrmWorkShowModel SFShowModel
        {
            get
            {
                return (FrmWorkShowModel)this.GetValIntByKey(FrmSubFlowAttr.SFShowModel);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFShowModel, (int)value);
            }
        }
        /// <summary>
        /// Y
        /// </summary>
        public float SF_Y
        {
            get
            {
                return this.GetValFloatByKey(FrmSubFlowAttr.SF_Y);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SF_Y, value);
            }
        }
        /// <summary>
        /// X
        /// </summary>
        public float SF_X
        {
            get
            {
                return this.GetValFloatByKey(FrmSubFlowAttr.SF_X);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SF_X, value);
            }
        }
        /// <summary>
        /// 打开类型
        /// </summary>
        public int SFOpenType
        {
            get
            {
                return this.GetValIntByKey(FrmSubFlowAttr.SFOpenType);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFOpenType, value);
            }
        }
        /// <summary>
        /// W
        /// </summary>
        public float SF_W
        {
            get
            {
                return this.GetValFloatByKey(FrmSubFlowAttr.SF_W);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SF_W, value);
            }
        }
        public string SF_Wstr
        {
            get
            {
                if (this.SF_W == 0)
                    return "100%";
                return this.SF_W + "px";
            }
        }
        /// <summary>
        /// H
        /// </summary>
        public float SF_H
        {
            get
            {
                return this.GetValFloatByKey(FrmSubFlowAttr.SF_H);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SF_H, value);
            }
        }
        public string SF_Hstr
        {
            get
            {
                if (this.SF_H == 0)
                    return "100%";
                return this.SF_H + "px";
            }
        }
        /// <summary>
        /// 轨迹图是否显示?
        /// </summary>
        public bool SFTrackEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmSubFlowAttr.SFTrackEnable);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFTrackEnable, value);
            }
        }
        /// <summary>
        /// 历史审核信息是否显示?
        /// </summary>
        public bool SFListEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmSubFlowAttr.SFListEnable);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFListEnable, value);
            }
        }
        /// <summary>
        /// 在轨迹表里是否显示所有的步骤？
        /// </summary>
        public bool SFIsShowAllStep
        {
            get
            {
                return this.GetValBooleanByKey(FrmSubFlowAttr.SFIsShowAllStep);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFIsShowAllStep, value);
            }
        }
        /// <summary>
        /// 如果用户未审核是否按照默认意见填充?
        /// </summary>
        public bool SFIsFullInfo
        {
            get
            {
                return this.GetValBooleanByKey(FrmSubFlowAttr.SFIsFullInfo);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFIsFullInfo, value);
            }
        }
        /// <summary>
        /// 默认审核信息
        /// </summary>
        public string SFDefInfo
        {
            get
            {
                return this.GetValStringByKey(FrmSubFlowAttr.SFDefInfo);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFDefInfo, value);
            }
        }
        /// <summary>
        /// 节点名称.
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey("Name");
            }
        }
        /// <summary>
        /// 标题，如果为空则取节点名称.
        /// </summary>
        public string SFCaption
        {
            get
            {
                string str = this.GetValStringByKey(FrmSubFlowAttr.SFCaption);
                if (str == "")
                    str = "启动子流程";
                return str;
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFCaption, value);
            }
        }
        /// <summary>
        /// 操作名词(审核，审定，审阅，批示)
        /// </summary>
        public string SFOpLabel
        {
            get
            {
                return this.GetValStringByKey(FrmSubFlowAttr.SFOpLabel);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFOpLabel, value);
            }
        }
        /// <summary>
        /// 是否显示数字签名？
        /// </summary>
        public bool SigantureEnabel
        {
            get
            {
                return this.GetValBooleanByKey(FrmSubFlowAttr.SigantureEnabel);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SigantureEnabel, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsDelete = false;
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 重写主键
        /// </summary>
        public override string PK
        {
            get
            {
                return "NodeID";
            }
        }
        /// <summary>
        /// 父子流程
        /// </summary>
        public FrmSubFlow()
        {
        }
        /// <summary>
        /// 父子流程
        /// </summary>
        /// <param name="no"></param>
        public FrmSubFlow(string mapData)
        {
            if (mapData.Contains("ND") == false)
            {
                this.HisFrmSubFlowSta = FrmSubFlowSta.Disable;
                return;
            }

            string mapdata = mapData.Replace("ND", "");
            if (DataType.IsNumStr(mapdata) == false)
            {
                this.HisFrmSubFlowSta = FrmSubFlowSta.Disable;
                return;
            }

            try
            {
                this.NodeID = int.Parse(mapdata);
            }
            catch
            {
                return;
            }
            this.Retrieve();
        }
        /// <summary>
        /// 父子流程
        /// </summary>
        /// <param name="no"></param>
        public FrmSubFlow(int nodeID)
        {
            this.NodeID = nodeID;
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Node", "父子流程");

                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(NodeAttr.Name, null, "节点名称", true, true, 0, 100, 10);
                map.AddTBString(FrmSubFlowAttr.SFLab, "子流程", "显示标签", true, false, 0, 200, 10, true);

                #region 此处变更了 NodeSheet类中的，map 描述该部分也要变更.

                map.AddDDLSysEnum(FrmSubFlowAttr.SFSta, (int)FrmSubFlowSta.Disable, "组件状态",
                   true, true, FrmSubFlowAttr.SFSta, "@0=禁用@1=启用@2=只读");

                map.AddDDLSysEnum(FrmSubFlowAttr.SFShowModel, (int)FrmWorkShowModel.Free, "显示方式",
                    true, true, FrmSubFlowAttr.SFShowModel, "@0=表格方式@1=自由模式"); //此属性暂时没有用.

                map.AddTBString(FrmSubFlowAttr.SFCaption, "启动子流程", "连接标题", true, false, 0, 100, 10, true);
                map.AddTBString(FrmSubFlowAttr.SFDefInfo, null, "可启动的子流程编号(多个用逗号分开)", false, false, 0, 50, 10, true);
                map.AddTBString(FrmSubFlowAttr.SFActiveFlows, null, "可触发的子流程编号(多个用逗号分开)", false, false, 0, 50, 10, true);

                map.AddTBFloat(FrmSubFlowAttr.SF_X, 5, "位置X", true, false);
                map.AddTBFloat(FrmSubFlowAttr.SF_Y, 5, "位置Y", true, false);

                map.AddTBFloat(FrmSubFlowAttr.SF_H, 300, "高度", true, false);
                map.AddTBFloat(FrmSubFlowAttr.SF_W, 400, "宽度", true, false);

                map.AddTBString(FrmSubFlowAttr.SFFields, null, "审批格式字段", true, false, 0, 50, 10, true);

                map.AddDDLSysEnum(FrmSubFlowAttr.SFShowCtrl, (int)SFShowCtrl.All, "显示控制方式",
                  true, true, FrmSubFlowAttr.SFShowCtrl, "@0=可以看所有的子流程@1=仅仅可以看自己发起的子流程"); //此属性暂时没有用.

                map.AddDDLSysEnum(FrmSubFlowAttr.SFOpenType, 0, "打开子流程显示",
                 true, true, FrmSubFlowAttr.SFOpenType, "@0=工作查看器@1=傻瓜表单轨迹查看器"); //此属性暂时没有用.


                #endregion 此处变更了 NodeSheet类中的，map 描述该部分也要变更.

                RefMethod rm = new RefMethod();
                rm.Title = "手动启动子流程"; 
                rm.ClassMethodName = this.ToString() + ".DoSubFlowHand";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "自动触发子流程"; 
                rm.ClassMethodName = this.ToString() + ".DoSubFlowAuto";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "延续子流程";  
                rm.ClassMethodName = this.ToString() + ".DoSubFlowYanXu";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 子流程。
        /// <summary>
        /// 自动触发
        /// </summary>
        /// <returns></returns>
        public string DoSubFlowAuto()
        {
            return "../../Admin/AttrNode/SubFlow/SubFlowAuto.htm?FK_Node=" + this.NodeID + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 手动启动子流程
        /// </summary>
        /// <returns></returns>
        public string DoSubFlowHand()
        {
            return "../../Admin/AttrNode/SubFlow/SubFlowHand.htm?FK_Node=" + this.NodeID + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 延续子流程
        /// </summary>
        /// <returns></returns>
        public string DoSubFlowYanXu()
        {
            return "../../Admin/AttrNode/SubFlow/SubFlowYanXu.htm?FK_Node=" + this.NodeID + "&tk=" + new Random().NextDouble();
        }
        #endregion 子流程。

        #region 重写方法.
        protected override bool beforeUpdateInsertAction()
        {
            return base.beforeUpdateInsertAction();
        }
        protected override void afterUpdate()
        {
            //清空缓存，重新查数据
            Node nd = new Node(this.NodeID);
            nd.RetrieveFromDBSources();
            Cash2019.UpdateRow(nd.ToString(),this.NodeID.ToString(),nd.Row);

            GroupField gf = new GroupField();
            if (this.SFSta == FrmSubFlowSta.Disable)
            {
                gf.Delete(GroupFieldAttr.CtrlID, "SubFlow" + this.No);
            }
            else
            {
                if (gf.IsExit(GroupFieldAttr.CtrlID, "SubFlow" + this.No) == false)
                {
                    gf = new GroupField();
                    gf.FrmID = "ND" + this.NodeID;
                    gf.CtrlID = "SubFlow" + this.No;
                    gf.CtrlType = GroupCtrlType.SubFlow;
                    gf.Lab = "父子流程组件";
                    gf.Idx = 0;
                    gf.Insert(); //插入.
                }
            }

            base.afterUpdate();
        }
        #endregion 重写方法.
    }
    /// <summary>
    /// 父子流程s
    /// </summary>
    public class FrmSubFlows : Entities
    {
        #region 构造
        /// <summary>
        /// 父子流程s
        /// </summary>
        public FrmSubFlows()
        {
        }
        /// <summary>
        /// 父子流程s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmSubFlows(string fk_mapdata)
        {
            if (SystemConfig.IsDebug)
                this.Retrieve("No", fk_mapdata);
            else
                this.RetrieveFromCash("No", (object)fk_mapdata);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmSubFlow();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmSubFlow> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmSubFlow>)this;
        }

        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmSubFlow> Tolist()
        {
            System.Collections.Generic.List<FrmSubFlow> list = new System.Collections.Generic.List<FrmSubFlow>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmSubFlow)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
