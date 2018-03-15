using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.WF.Template;
using BP.WF;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 轨迹图标组件控件状态
    /// </summary>
    public enum FTCSta
    {
        /// <summary>
        /// 不可用
        /// </summary>
        Disable,
        /// <summary>
        /// 只读
        /// </summary>
        ReadOnly,
        /// <summary>
        /// 可以设置人员
        /// </summary>
        SetWorkers
    }
    /// <summary>
    /// 流转自定义组件
    /// </summary>
    public class FTCAttr : EntityNoAttr
    {
        /// <summary>
        /// 显示标签
        /// </summary>
        public const string FTCLab = "FTCLab";
        /// <summary>
        /// 状态
        /// </summary>
        public const string FTCSta = "FTCSta";
        /// <summary>
        /// 工作模式
        /// </summary>
        public const string FTCWorkModel = "FTCWorkModel";
        /// <summary>
        /// X
        /// </summary>
        public const string FTC_X = "FTC_X";
        /// <summary>
        /// Y
        /// </summary>
        public const string FTC_Y = "FTC_Y";
        /// <summary>
        /// H
        /// </summary>
        public const string FTC_H = "FTC_H";
        /// <summary>
        /// W
        /// </summary>
        public const string FTC_W = "FTC_W";
    }
    /// <summary>
    /// 流转自定义组件
    /// </summary>
    public class FrmTransferCustom : Entity
    {
        #region 属性
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
        /// 控件状态
        /// </summary>
        public FTCSta FTCSta
        {
            get
            {
                return (FTCSta)this.GetValIntByKey(FTCAttr.FTCSta);
            }
            set
            {
                this.SetValByKey(FTCAttr.FTCSta, (int)value);
            }
        }
        /// <summary>
        /// 工作模式
        /// </summary>
        public int FTCWorkModel
        {
            get
            {
                return this.GetValIntByKey(FTCAttr.FTCWorkModel);
            }
            set
            {
                this.SetValByKey(FTCAttr.FTCWorkModel, value);
            }
        }
        /// <summary>
        /// Y
        /// </summary>
        public float FTC_Y
        {
            get
            {
                return this.GetValFloatByKey(FTCAttr.FTC_Y);
            }
            set
            {
                this.SetValByKey(FTCAttr.FTC_Y, value);
            }
        }
        /// <summary>
        /// X
        /// </summary>
        public float FTC_X
        {
            get
            {
                return this.GetValFloatByKey(FTCAttr.FTC_X);
            }
            set
            {
                this.SetValByKey(FTCAttr.FTC_X, value);
            }
        }
        /// <summary>
        /// W
        /// </summary>
        public float FTC_W
        {
            get
            {
                return this.GetValFloatByKey(FTCAttr.FTC_W);
            }
            set
            {
                this.SetValByKey(FTCAttr.FTC_W, value);
            }
        }
        public string FTC_Wstr
        {
            get
            {
                if (this.FTC_W == 0)
                    return "100%";
                return this.FTC_W + "px";
            }
        }
        /// <summary>
        /// H
        /// </summary>
        public float FTC_H
        {
            get
            {
                return this.GetValFloatByKey(FTCAttr.FTC_H);
            }
            set
            {
                this.SetValByKey(FTCAttr.FTC_H, value);
            }
        }
        public string FTC_Hstr
        {
            get
            {
                if (this.FTC_H == 0)
                    return "100%";
                return this.FTC_H + "px";
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
        /// 显示标签
        /// </summary>
        public string FTCLab
        {
            get
            {
                return this.GetValStrByKey(FTCAttr.FTCLab);
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
        /// 流转自定义组件
        /// </summary>
        public FrmTransferCustom()
        {
        }
        /// <summary>
        /// 流转自定义组件
        /// </summary>
        /// <param name="no"></param>
        public FrmTransferCustom(string mapData)
        {
            if (mapData.Contains("ND") == false)
            {
                this.FTCSta = FTCSta.Disable;
                return;
            }

            string mapdata = mapData.Replace("ND", "");
            if (DataType.IsNumStr(mapdata) == false)
            {
                this.FTCSta = FTCSta.Disable;
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
        /// 流转自定义组件
        /// </summary>
        /// <param name="no"></param>
        public FrmTransferCustom(int nodeID)
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

                Map map = new Map("WF_Node", "流转自定义组件");

                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(NodeAttr.Name, null, "节点名称", true, true, 0, 100, 10);
                map.AddTBString(FTCAttr.FTCLab, "流转自定义", "显示标签", true, false, 0, 50, 10, true);


                #region 此处变更了 NodeSheet类中的，map 描述该部分也要变更.

                map.AddDDLSysEnum(FTCAttr.FTCSta, (int)FTCSta.Disable, "组件状态",
                   true, true, FTCAttr.FTCSta, "@0=禁用@1=只读@2=可设置人员");

                map.AddDDLSysEnum(FTCAttr.FTCWorkModel,0, "工作模式",
                  true, true, FTCAttr.FTCWorkModel, "@0=简洁模式@1=高级模式");

                map.AddTBFloat(FTCAttr.FTC_X, 5, "位置X", false, false);
                map.AddTBFloat(FTCAttr.FTC_Y, 5, "位置Y", false, false);

                map.AddTBFloat(FTCAttr.FTC_H, 300, "高度", true, false);
                map.AddTBFloat(FTCAttr.FTC_W, 400, "宽度", true, false);

                #endregion 此处变更了 NodeSheet类中的，map 描述该部分也要变更.

                this._enMap = map;
                return this._enMap;
            }
        }
       
     
        #endregion
    }
    /// <summary>
    /// 流转自定义组件s
    /// </summary>
    public class FrmTransferCustoms : Entities
    {
        #region 构造
        /// <summary>
        /// 流转自定义组件s
        /// </summary>
        public FrmTransferCustoms()
        {
        }
        /// <summary>
        /// 流转自定义组件s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmTransferCustoms(string fk_mapdata)
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
                return new FrmTransferCustom();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmTransferCustom> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmTransferCustom>)this;
        }

        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmTransferCustom> Tolist()
        {
            System.Collections.Generic.List<FrmTransferCustom> list = new System.Collections.Generic.List<FrmTransferCustom>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmTransferCustom)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
