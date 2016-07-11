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
    /// 子线程组件控件状态
    /// </summary>
    public enum FrmThreadSta
    {
        /// <summary>
        /// 不可用
        /// </summary>
        Disable,
        /// <summary>
        /// 启用
        /// </summary>
        Enable 
    }
    /// <summary>
    /// 子线程组件
    /// </summary>
    public class FrmThreadAttr : EntityNoAttr
    {
        /// <summary>
        /// 状态
        /// </summary>
        public const string FrmThreadSta = "FrmThreadSta";
        /// <summary>
        /// X
        /// </summary>
        public const string FrmThread_X = "FrmThread_X";
        /// <summary>
        /// Y
        /// </summary>
        public const string FrmThread_Y = "FrmThread_Y";
        /// <summary>
        /// H
        /// </summary>
        public const string FrmThread_H = "FrmThread_H";
        /// <summary>
        /// W
        /// </summary>
        public const string FrmThread_W = "FrmThread_W";
    }
    /// <summary>
    /// 子线程组件
    /// </summary>
    public class FrmThread : Entity
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
        public FrmThreadSta FrmThreadSta
        {
            get
            {
                return (FrmThreadSta)this.GetValIntByKey(FrmThreadAttr.FrmThreadSta);
            }
            set
            {
                this.SetValByKey(FrmThreadAttr.FrmThreadSta, (int)value);
            }
        }
        /// <summary>
        /// Y
        /// </summary>
        public float FrmThread_Y
        {
            get
            {
                return this.GetValFloatByKey(FrmThreadAttr.FrmThread_Y);
            }
            set
            {
                this.SetValByKey(FrmThreadAttr.FrmThread_Y, value);
            }
        }
        /// <summary>
        /// X
        /// </summary>
        public float FrmThread_X
        {
            get
            {
                return this.GetValFloatByKey(FrmThreadAttr.FrmThread_X);
            }
            set
            {
                this.SetValByKey(FrmThreadAttr.FrmThread_X, value);
            }
        }
        /// <summary>
        /// W
        /// </summary>
        public float FrmThread_W
        {
            get
            {
                return this.GetValFloatByKey(FrmThreadAttr.FrmThread_W);
            }
            set
            {
                this.SetValByKey(FrmThreadAttr.FrmThread_W, value);
            }
        }
        public string FrmThread_Wstr
        {
            get
            {
                if (this.FrmThread_W == 0)
                    return "100%";
                return this.FrmThread_W + "px";
            }
        }
        /// <summary>
        /// H
        /// </summary>
        public float FrmThread_H
        {
            get
            {
                return this.GetValFloatByKey(FrmThreadAttr.FrmThread_H);
            }
            set
            {
                this.SetValByKey(FrmThreadAttr.FrmThread_H, value);
            }
        }
        public string FrmThread_Hstr
        {
            get
            {
                if (this.FrmThread_H == 0)
                    return "100%";
                return this.FrmThread_H + "px";
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
        /// 子线程组件
        /// </summary>
        public FrmThread()
        {
        }
        /// <summary>
        /// 子线程组件
        /// </summary>
        /// <param name="no"></param>
        public FrmThread(string mapData)
        {
            if (mapData.Contains("ND") == false)
            {
                this.FrmThreadSta = FrmThreadSta.Disable;
                return;
            }

            string mapdata = mapData.Replace("ND", "");
            if (DataType.IsNumStr(mapdata) == false)
            {
                this.FrmThreadSta = FrmThreadSta.Disable;
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
        /// 子线程组件
        /// </summary>
        /// <param name="no"></param>
        public FrmThread(int nodeID)
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

                Map map = new Map("WF_Node", "子线程组件");

                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(NodeAttr.Name, null, "节点名称", true, true, 0, 100, 10);

                #region 此处变更了 NodeSheet类中的，map 描述该部分也要变更.

                map.AddDDLSysEnum(FrmThreadAttr.FrmThreadSta, (int)FrmThreadSta.Disable, "组件状态",
                   true, true, FrmThreadAttr.FrmThreadSta, "@0=禁用@1=启用");

                map.AddTBFloat(FrmThreadAttr.FrmThread_X, 5, "位置X", true, false);
                map.AddTBFloat(FrmThreadAttr.FrmThread_Y, 5, "位置Y", true, false);

                map.AddTBFloat(FrmThreadAttr.FrmThread_H, 500, "高度", true, false);
                map.AddTBFloat(FrmThreadAttr.FrmThread_W, 400, "宽度", true, false);

                #endregion 此处变更了 NodeSheet类中的，map 描述该部分也要变更.

                this._enMap = map;
                return this._enMap;
            }
        }
        protected override bool beforeInsert()
        {
            GroupField gf = new GroupField();
            if (gf.IsExit(GroupFieldAttr.CtrlID, this.No) == false)
            {
                gf.EnName = "ND"+this.NodeID;
                gf.CtrlID = "FrmThread" + this.No;
                gf.CtrlType = "FrmThread";
                gf.Lab = "轨迹";
                gf.Idx = 0;
                gf.Insert(); //插入.
            }

            return base.beforeInsert();
        }
        protected override bool beforeUpdateInsertAction()
        {
            return base.beforeUpdateInsertAction();
        }
        #endregion
    }
    /// <summary>
    /// 子线程组件s
    /// </summary>
    public class FrmThreads : Entities
    {
        #region 构造
        /// <summary>
        /// 子线程组件s
        /// </summary>
        public FrmThreads()
        {
        }
        /// <summary>
        /// 子线程组件s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmThreads(string fk_mapdata)
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
                return new FrmThread();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmThread> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmThread>)this;
        }

        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmThread> Tolist()
        {
            System.Collections.Generic.List<FrmThread> list = new System.Collections.Generic.List<FrmThread>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmThread)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
