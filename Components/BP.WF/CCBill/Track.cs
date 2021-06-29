using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF.Template;
using BP.WF;
using BP.CCBill.Template;


namespace BP.CCBill
{
    /// <summary>
    ///  轨迹-属性
    /// </summary>
    public class TrackAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// WorkID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 活动类型
        /// </summary>
        public const string ActionType = "ActionType";
        /// <summary>
        /// 活动类型名称
        /// </summary>
        public const string ActionTypeText = "ActionTypeText";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录人名称
        /// </summary>
        public const string RecName = "RecName";
        /// <summary>
        /// 部门No
        /// </summary>
        public const string DeptNo = "DeptNo";
        /// <summary>
        /// 部门名称
        /// </summary>
        public const string DeptName = "DeptName";
        /// <summary>
        /// 参数信息
        /// </summary>
        public const string Tag = "Tag";
        /// <summary>
        /// 表单数据
        /// </summary>
        public const string FrmDB = "FrmDB";
        /// <summary>
        /// 消息
        /// </summary>
        public const string Msg = "Msg";
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 表单名称
        /// </summary>
        public const string FrmName = "FrmName";

        #region 流程相关信息.
        /// <summary>
        /// FID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// 流程ID
        /// </summary>
        public const string FlowNo = "FlowNo";
        /// <summary>
        /// 流程名称
        /// </summary>
        public const string FlowName = "FlowName";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 节点名字
        /// </summary>
        public const string NodeName = "NodeName";
        #endregion 流程相关信息.
    }
    /// <summary>
    /// 轨迹
    /// </summary>
    public class Track : EntityMyPK
    {
        #region 字段属性.
        /// <summary>
        /// 参数数据.
        /// </summary>
        public string Tag
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.Tag);
            }
            set
            {
                this.SetValByKey(TrackAttr.Tag, value);
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.FrmID);
            }
            set
            {
                this.SetValByKey(TrackAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 表单名称
        /// </summary>
        public string FrmName
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.FrmName);
            }
            set
            {
                this.SetValByKey(TrackAttr.FrmName, value);
            }
        }
        /// <summary>
        /// 记录日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.RDT);
            }
            set
            {
                this.SetValByKey(TrackAttr.RDT, value);
            }
        }
         
        /// <summary>
        /// fid
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(TrackAttr.FID);
            }
            set
            {
                this.SetValByKey(TrackAttr.FID, value);
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(TrackAttr.WorkID);
            }
            set
            {
                this.SetValByKey(TrackAttr.WorkID, value);
            }
        }
        public string ActionType
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.ActionType);
            }
            set
            {
                this.SetValByKey(TrackAttr.ActionType, value);
            }
        }
        /// <summary>
        /// 获取动作文本
        /// </summary>
        /// <param name="at"></param>
        /// <returns></returns>
        public static string GetActionTypeT(string at)
        {
            switch (at)
            {
                case BP.CCBill.FrmActionType.Save:
                    return "保存";
                case BP.CCBill.FrmActionType.Create:
                    return "提交";
                case BP.CCBill.FrmActionType.BBS:
                    return "评论";
                case BP.CCBill.FrmActionType.View:
                    return "打开";
                default:
                    return "信息" + at.ToString();
            }
        }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActionTypeText
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.ActionTypeText);
            }
            set
            {
                this.SetValByKey(TrackAttr.ActionTypeText, value);
            }
        }
        /// <summary>
        /// 记录人
        /// </summary>
        public string Rec
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.Rec);
            }
            set
            {
                this.SetValByKey(TrackAttr.Rec, value);
            }
        }
        /// <summary>
        /// 记录人名字
        /// </summary>
        public string RecName
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.RecName);
            }
            set
            {
                this.SetValByKey(TrackAttr.RecName, value);
            }
        }
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.Msg);
            }
            set
            {
                this.SetValByKey(TrackAttr.Msg, value);
            }
        }
        /// <summary>
        /// 消息
        /// </summary>
        public string MsgHtml
        {
            get
            {
                return this.GetValHtmlStringByKey(TrackAttr.Msg);
            }
        }
        #endregion attrs

        #region 流程属性.
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.FlowNo);
            }
            set
            {
                this.SetValByKey(TrackAttr.FlowNo, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.FlowName);
            }
            set
            {
                this.SetValByKey(TrackAttr.FlowName, value);
            }
        }
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(TrackAttr.NodeID);
            }
            set
            {
                this.SetValByKey(TrackAttr.NodeID, value);
            }
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.NodeName);
            }
            set
            {
                this.SetValByKey(TrackAttr.NodeName, value);
            }
        }
        public string DeptNo
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.DeptNo);
            }
            set
            {
                this.SetValByKey(TrackAttr.DeptNo, value);
            }
        }
        public string DeptName
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.DeptName);
            }
            set
            {
                this.SetValByKey(TrackAttr.DeptName, value);
            }
        }
        #endregion 流程属性.


        #region 构造.
        /// <summary>
        /// 表单轨迹表
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Frm_Track", "表单轨迹表");

                #region 基本字段.
                map.AddMyPK();  //组合主键.

                map.AddTBString(TrackAttr.FrmID, null, "表单ID", true, false, 0, 50, 200);
                map.AddTBString(TrackAttr.FrmName, null, "表单名称(可以为空)", true, false, 0, 200, 200);

               // map.AddTBInt(TrackAttr.ActionType, 0, "类型", true, false);
                map.AddTBString(TrackAttr.ActionType, null, "类型", true, false, 0, 30, 100);
                map.AddTBString(TrackAttr.ActionTypeText, null, "类型(名称)", true, false, 0, 30, 100);

                map.AddTBInt(TrackAttr.WorkID, 0, "工作ID/OID", true, false);
                map.AddTBString(TrackAttr.Msg, null, "消息", true, false, 0, 300, 3000);

                map.AddTBString(TrackAttr.Rec, null, "记录人", true, false, 0, 200, 100);
                map.AddTBString(TrackAttr.RecName, null, "名称", true, false, 0, 200, 100);
                map.AddTBDateTime(TrackAttr.RDT, null, "记录日期时间", true, false);

                map.AddTBString(TrackAttr.DeptNo, null, "部门编号", true, false, 0, 200, 100);
                map.AddTBString(TrackAttr.DeptName, null, "名称", true, false, 0, 200, 100);
                #endregion 基本字段

                #region 流程信息.
                //流程信息.
                map.AddTBInt(TrackAttr.FID, 0, "FID", true, false);

                map.AddTBString(TrackAttr.FlowNo, null, "流程ID", true, false, 0, 20, 20);
                map.AddTBString(TrackAttr.FlowName, null, "流程名称", true, false, 0, 200, 200);

                map.AddTBInt(TrackAttr.NodeID, 0, "节点ID", true, false);
                map.AddTBString(TrackAttr.NodeName, null, "节点名称", true, false, 0, 200, 200);
                #endregion 流程信息.

                map.AddTBAtParas(3999); //参数.

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 轨迹
        /// </summary>
        public Track()
        {
        }
        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.MyPK) == true)
                this.MyPK = DBAccess.GenerGUID();
            return base.beforeInsert();
        }
        #endregion 构造.
    }
    /// <summary>
    /// 轨迹集合s
    /// </summary>
    public class Tracks : EntitiesMyPK
    {
        #region 构造方法.
        /// <summary>
        /// 轨迹集合
        /// </summary>
        public Tracks()
        {
        }
        public override Entity GetNewEntity
        {
            get
            {
                return new Track();
            }
        }
        #endregion 构造方法.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Track> ToJavaList()
        {
            return (System.Collections.Generic.IList<Track>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Track> Tolist()
        {
            System.Collections.Generic.List<Track> list = new System.Collections.Generic.List<Track>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Track)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}