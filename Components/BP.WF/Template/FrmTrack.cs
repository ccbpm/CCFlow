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
    public enum FrmTrackSta
    {
        /// <summary>
        /// 不可用
        /// </summary>
        Disable,
        /// <summary>
        /// 显示轨迹图
        /// </summary>
        Chart,
        /// <summary>
        /// 显示轨迹表
        /// </summary>
        Table
    }
    /// <summary>
    /// 轨迹图标组件
    /// </summary>
    public class FrmTrackAttr : EntityNoAttr
    {
        /// <summary>
        /// 显示标签
        /// </summary>
        public const string FrmTrackLab = "FrmTrackLab";
        /// <summary>
        /// 状态
        /// </summary>
        public const string FrmTrackSta = "FrmTrackSta";
        /// <summary>
        /// X
        /// </summary>
        public const string FrmTrack_X = "FrmTrack_X";
        /// <summary>
        /// Y
        /// </summary>
        public const string FrmTrack_Y = "FrmTrack_Y";
        /// <summary>
        /// H
        /// </summary>
        public const string FrmTrack_H = "FrmTrack_H";
        /// <summary>
        /// W
        /// </summary>
        public const string FrmTrack_W = "FrmTrack_W";
    }
    /// <summary>
    /// 轨迹图标组件
    /// </summary>
    public class FrmTrack : Entity
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
        public FrmTrackSta FrmTrackSta
        {
            get
            {
                return (FrmTrackSta)this.GetValIntByKey(FrmTrackAttr.FrmTrackSta);
            }
            set
            {
                this.SetValByKey(FrmTrackAttr.FrmTrackSta, (int)value);
            }
        }
        /// <summary>
        /// Y
        /// </summary>
        public float FrmTrack_Y
        {
            get
            {
                return this.GetValFloatByKey(FrmTrackAttr.FrmTrack_Y);
            }
            set
            {
                this.SetValByKey(FrmTrackAttr.FrmTrack_Y, value);
            }
        }
        /// <summary>
        /// X
        /// </summary>
        public float FrmTrack_X
        {
            get
            {
                return this.GetValFloatByKey(FrmTrackAttr.FrmTrack_X);
            }
            set
            {
                this.SetValByKey(FrmTrackAttr.FrmTrack_X, value);
            }
        }
        /// <summary>
        /// W
        /// </summary>
        public float FrmTrack_W
        {
            get
            {
                return this.GetValFloatByKey(FrmTrackAttr.FrmTrack_W);
            }
            set
            {
                this.SetValByKey(FrmTrackAttr.FrmTrack_W, value);
            }
        }
        public string FrmTrack_Wstr
        {
            get
            {
                if (this.FrmTrack_W == 0)
                    return "100%";
                return this.FrmTrack_W + "px";
            }
        }
        /// <summary>
        /// H
        /// </summary>
        public float FrmTrack_H
        {
            get
            {
                return this.GetValFloatByKey(FrmTrackAttr.FrmTrack_H);
            }
            set
            {
                this.SetValByKey(FrmTrackAttr.FrmTrack_H, value);
            }
        }
        public string FrmTrack_Hstr
        {
            get
            {
                if (this.FrmTrack_H == 0)
                    return "100%";
                return this.FrmTrack_H + "px";
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
        public string FrmTrackLab
        {
            get
            {
                return this.GetValStrByKey(FrmTrackAttr.FrmTrackLab);
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
        /// 轨迹图标组件
        /// </summary>
        public FrmTrack()
        {
        }
        /// <summary>
        /// 轨迹图标组件
        /// </summary>
        /// <param name="no"></param>
        public FrmTrack(string mapData)
        {
            if (mapData.Contains("ND") == false)
            {
                this.FrmTrackSta = FrmTrackSta.Disable;
                return;
            }

            string mapdata = mapData.Replace("ND", "");
            if (DataType.IsNumStr(mapdata) == false)
            {
                this.FrmTrackSta = FrmTrackSta.Disable;
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
        /// 轨迹图标组件
        /// </summary>
        /// <param name="no"></param>
        public FrmTrack(int nodeID)
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

                Map map = new Map("WF_Node", "轨迹图标组件");

                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(NodeAttr.Name, null, "节点名称", true, true, 0, 100, 10);
                map.AddTBString(FrmTrackAttr.FrmTrackLab, "轨迹", "显示标签", true, false, 0, 200, 10, false);

                #region 此处变更了 NodeSheet类中的，map 描述该部分也要变更.

                map.AddDDLSysEnum(FrmTrackAttr.FrmTrackSta, (int)FrmTrackSta.Disable, "组件状态",
                   true, true, FrmTrackAttr.FrmTrackSta, "@0=禁用@1=显示轨迹图@2=显示轨迹表");

                map.AddTBFloat(FrmTrackAttr.FrmTrack_X, 5, "位置X", false, false);
                map.AddTBFloat(FrmTrackAttr.FrmTrack_Y, 5, "位置Y", false, false);

                map.AddTBFloat(FrmTrackAttr.FrmTrack_H, 300, "高度", true, false);
                map.AddTBFloat(FrmTrackAttr.FrmTrack_W, 400, "宽度", true, false);

                #endregion 此处变更了 NodeSheet类中的，map 描述该部分也要变更.

                this._enMap = map;
                return this._enMap;
            }
        }
       
        protected override bool beforeUpdateInsertAction()
        {
            return base.beforeUpdateInsertAction();
        }
        #endregion
    }
    /// <summary>
    /// 轨迹图标组件s
    /// </summary>
    public class FrmTracks : Entities
    {
        #region 构造
        /// <summary>
        /// 轨迹图标组件s
        /// </summary>
        public FrmTracks()
        {
        }
        /// <summary>
        /// 轨迹图标组件s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmTracks(string fk_mapdata)
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
                return new FrmTrack();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmTrack> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmTrack>)this;
        }

        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmTrack> Tolist()
        {
            System.Collections.Generic.List<FrmTrack> list = new System.Collections.Generic.List<FrmTrack>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmTrack)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
