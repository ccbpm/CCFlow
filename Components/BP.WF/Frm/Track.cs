using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF.Template;

namespace BP.Frm
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
        /// 完成日期
        /// </summary>
        public const string CDT = "CDT";
        /// <summary>
        /// FID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// WorkID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// CWorkID
        /// </summary>
        public const string CWorkID = "CWorkID";
        /// <summary>
        /// 活动类型
        /// </summary>
        public const string ActionType = "ActionType";
        /// <summary>
        /// 活动类型名称
        /// </summary>
        public const string ActionTypeText = "ActionTypeText";
        /// <summary>
        /// 时间跨度
        /// </summary>
        public const string WorkTimeSpan = "WorkTimeSpan";
        /// <summary>
        /// 节点数据
        /// </summary>
        public const string NodeData = "NodeData";
        /// <summary>
        /// 轨迹字段
        /// </summary>
        public const string TrackFields = "TrackFields";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// 从节点
        /// </summary>
        public const string NDFrom = "NDFrom";
        /// <summary>
        /// 到节点
        /// </summary>
        public const string NDTo = "NDTo";
        /// <summary>
        /// 从人员
        /// </summary>
        public const string EmpFrom = "EmpFrom";
        /// <summary>
        /// 到人员
        /// </summary>
        public const string EmpTo = "EmpTo";
        /// <summary>
        /// 审核
        /// </summary>
        public const string Msg = "Msg";
        /// <summary>
        /// EmpFromT
        /// </summary>
        public const string EmpFromT = "EmpFromT";
        /// <summary>
        /// NDFromT
        /// </summary>
        public const string NDFromT = "NDFromT";
        /// <summary>
        /// NDToT
        /// </summary>
        public const string NDToT = "NDToT";
        /// <summary>
        /// EmpToT
        /// </summary>
        public const string EmpToT = "EmpToT";
        /// <summary>
        /// 实际执行人员
        /// </summary>
        public const string Exer = "Exer";
        /// <summary>
        /// 参数信息
        /// </summary>
        public const string Tag = "Tag";
        /// <summary>
        /// 表单数据
        /// </summary>
        public const string FrmDB = "FrmDB";
    }
    /// <summary>
    /// 轨迹
    /// </summary>
    public class Track : BP.En.EntityMyPK
    {
        #region 基本属性.
        /// <summary>
        /// 表单数据
        /// </summary>
        public string FrmDB = null;
        public string FK_Flow = null;
        #endregion 基本属性.

        #region 字段属性.
        /// <summary>
        /// 节点从
        /// </summary>
        public int NDFrom
        {
            get
            {
                return this.GetValIntByKey(TrackAttr.NDFrom);
            }
            set
            {
                this.SetValByKey(TrackAttr.NDFrom, value);
            }
        }
        /// <summary>
        /// 节点到
        /// </summary>
        public int NDTo
        {
            get
            {
                return this.GetValIntByKey(TrackAttr.NDTo);
            }
            set
            {
                this.SetValByKey(TrackAttr.NDTo, value);
            }
        }
        /// <summary>
        /// 从人员
        /// </summary>
        public string EmpFrom
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.EmpFrom);
            }
            set
            {
                this.SetValByKey(TrackAttr.EmpFrom, value);
            }
        }
        /// <summary>
        /// 到人员
        /// </summary>
        public string EmpTo
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.EmpTo);
            }
            set
            {
                this.SetValByKey(TrackAttr.EmpTo, value);
            }
        }
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
        /// <summary>
        /// CWorkID
        /// </summary>
        public Int64 CWorkID
        {
            get
            {
                return this.GetValInt64ByKey(TrackAttr.CWorkID);
            }
            set
            {
                this.SetValByKey(TrackAttr.CWorkID, value);
            }
        }
        /// <summary>
        /// 活动类型
        /// </summary>
        public ActionType HisActionType
        {
            get
            {
                return (ActionType)this.GetValIntByKey(TrackAttr.ActionType);
            }
            set
            {
                this.SetValByKey(TrackAttr.ActionType, (int)value);
            }
        }
        /// <summary>
        /// 获取动作文本
        /// </summary>
        /// <param name="at"></param>
        /// <returns></returns>
        public static string GetActionTypeT(ActionType at)
        {
            switch (at)
            {
                case ActionType.Forward:
                    return "发送";
                case ActionType.Return:
                    return "退回";
                case ActionType.ReturnAndBackWay:
                    return "退回并原路返回";
                case ActionType.Shift:
                    return "移交";
                case ActionType.UnShift:
                    return "撤消移交";
                case ActionType.Start:
                    return "发起";
                case ActionType.UnSend:
                    return "撤消发送";
                case ActionType.ForwardFL:
                    return " -前进(分流点)";
                case ActionType.ForwardHL:
                    return " -向合流点发送";
                case ActionType.FlowOver:
                    return "流程结束";
                case ActionType.CallChildenFlow:
                    return "子流程调用";
                case ActionType.StartChildenFlow:
                    return "子流程发起";
                case ActionType.SubThreadForward:
                    return "线程前进";
                case ActionType.RebackOverFlow:
                    return "恢复已完成的流程";
                case ActionType.FlowOverByCoercion:
                    return "强制结束流程";
                case ActionType.HungUp:
                    return "挂起";
                case ActionType.UnHungUp:
                    return "取消挂起";
                case ActionType.Press:
                    return "催办";
                case ActionType.CC:
                    return "抄送";
                case ActionType.WorkCheck:
                    return "审核";
                case ActionType.ForwardAskfor:
                    return "加签发送";
                case ActionType.AskforHelp:
                    return "加签";
                case ActionType.Skip:
                    return "跳转";
                case ActionType.HuiQian:
                    return "主持人执行会签";
                case ActionType.DeleteFlowByFlag:
                    return "逻辑删除";
                case ActionType.Order:
                    return "队列发送";
                case ActionType.FlowBBS:
                    return "评论";
                case ActionType.TeampUp:
                    return "协作";
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
        /// 节点数据
        /// </summary>
        public string NodeData
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.NodeData);
            }
            set
            {
                this.SetValByKey(TrackAttr.NodeData, value);
            }
        }
        /// <summary>
        /// 实际执行人
        /// </summary>
        public string Exer
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.Exer);
            }
            set
            {
                this.SetValByKey(TrackAttr.Exer, value);
            }
        }
        /// <summary>
        /// 审核意见
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
        /// <summary>
        /// 人员到
        /// </summary>
        public string EmpToT
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.EmpToT);
            }
            set
            {
                this.SetValByKey(TrackAttr.EmpToT, value);
            }
        }
        /// <summary>
        /// 人员从
        /// </summary>
        public string EmpFromT
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.EmpFromT);
            }
            set
            {
                this.SetValByKey(TrackAttr.EmpFromT, value);
            }
        }
        /// <summary>
        /// 节点从
        /// </summary>
        public string NDFromT
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.NDFromT);
            }
            set
            {
                this.SetValByKey(TrackAttr.NDFromT, value);
            }
        }
        /// <summary>
        /// 节点到
        /// </summary>
        public string NDToT
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.NDToT);
            }
            set
            {
                this.SetValByKey(TrackAttr.NDToT, value);
            }
        }
        #endregion attrs

        #region 构造.
        public string RptName = null;
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Frm_Track", "轨迹表");

                #region 字段
                map.AddMyPK();  //增加一个自动增长的列.

                map.AddTBInt(TrackAttr.ActionType, 0, "类型", true, false);
                map.AddTBString(TrackAttr.ActionTypeText, null, "类型(名称)", true, false, 0, 30, 100);
                map.AddTBInt(TrackAttr.FID, 0, "流程ID", true, false);
                map.AddTBInt(TrackAttr.WorkID, 0, "工作ID", true, false);

                map.AddTBInt(TrackAttr.NDFrom, 0, "从节点", true, false);
                map.AddTBString(TrackAttr.NDFromT, null, "从节点(名称)", true, false, 0, 300, 100);

                map.AddTBInt(TrackAttr.NDTo, 0, "到节点", true, false);
                map.AddTBString(TrackAttr.NDToT, null, "到节点(名称)", true, false, 0, 999, 900);

                map.AddTBString(TrackAttr.EmpFrom, null, "从人员", true, false, 0, 20, 100);
                map.AddTBString(TrackAttr.EmpFromT, null, "从人员(名称)", true, false, 0, 30, 100);

                map.AddTBString(TrackAttr.EmpTo, null, "到人员", true, false, 0, 2000, 100);
                map.AddTBString(TrackAttr.EmpToT, null, "到人员(名称)", true, false, 0, 2000, 100);

                map.AddTBString(TrackAttr.RDT, null, "日期", true, false, 0, 20, 100);
                map.AddTBFloat(TrackAttr.WorkTimeSpan, 0, "时间跨度(天)", true, false);
                map.AddTBStringDoc(TrackAttr.Msg, null, "消息", true, false);
                map.AddTBStringDoc(TrackAttr.NodeData, null, "节点数据(日志信息)", true, false);
                map.AddTBString(TrackAttr.Tag, null, "参数", true, false, 0, 300, 3000);
                map.AddTBString(TrackAttr.Exer, null, "执行人", true, false, 0, 200, 100);
                #endregion 字段

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
        /// <summary>
        /// 增加授权人
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            if (BP.Web.WebUser.No == "Guest")
            {
                this.Exer = BP.Web.GuestUser.No + "," + BP.Web.GuestUser.Name;
            }
            else
            {
                if (BP.Web.WebUser.IsAuthorize)
                    this.Exer = BP.WF.Glo.DealUserInfoShowModel(BP.Web.WebUser.Auth, BP.Web.WebUser.AuthName);
                else
                    this.Exer = BP.WF.Glo.DealUserInfoShowModel(BP.Web.WebUser.No, BP.Web.WebUser.Name);
            }

            DateTime d;
            if (string.IsNullOrWhiteSpace(RDT) || DateTime.TryParse(this.RDT, out d) == false)
                this.RDT = BP.DA.DataType.CurrentDataTimess;

            return base.beforeInsert();
        }
        #endregion 构造.
    }
    /// <summary>
    /// 轨迹集合s
    /// </summary>
    public class Tracks : BP.En.EntitiesMyPK
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