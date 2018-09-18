using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum SendReturnMsgType
    {
        /// <summary>
        /// 消息
        /// </summary>
        Info,
        /// <summary>
        /// 系统消息
        /// </summary>
        SystemMsg
    }
    /// <summary>
    /// 消息标记
    /// </summary>
    public class SendReturnMsgFlag
    {
        /// <summary>
        /// 符合工作流程完成条件
        /// </summary>
        public const string MacthFlowOver = "MacthFlowOver";
        /// <summary>
        /// 当前工作[{0}]已经完成
        /// </summary>
        public const string CurrWorkOver = "CurrWorkOver";
        /// <summary>
        /// 符合完成条件,流程完成
        /// </summary>
        public const string FlowOverByCond = "FlowOverByCond";
        /// <summary>
        /// 到人员
        /// </summary>
        public const string ToEmps = "ToEmps";
        /// <summary>
        /// 到人员的扩展信息
        /// </summary>
        public const string ToEmpExt = "ToEmpExt";
        /// <summary>
        /// 分配任务
        /// </summary>
        public const string AllotTask = "AllotTask";
        /// <summary>
        /// 合流结束
        /// </summary>
        public const string HeLiuOver = "HeLiuOver";
        /// <summary>
        /// 工作报告
        /// </summary>
        public const string WorkRpt = "WorkRpt";
        /// <summary>
        /// 启动节点
        /// </summary>
        public const string WorkStartNode = "WorkStartNode";
        /// <summary>
        /// 工作启动
        /// </summary>
        public const string WorkStart = "WorkStart";
        /// <summary>
        /// 流程结束
        /// </summary>
        public const string FlowOver = "FlowOver";
        /// <summary>
        /// 发送成功后的事件异常
        /// </summary>
        public const string SendSuccessMsgErr = "SendSuccessMsgErr";
        /// <summary>
        /// 发送成功信息
        /// </summary>
        public const string SendSuccessMsg = "SendSuccessMsg";
        /// <summary>
        /// 分流程信息
        /// </summary>
        public const string FenLiuInfo = "FenLiuInfo";
        /// <summary>
        /// 抄送消息
        /// </summary>
        public const string CCMsg = "CCMsg";
        /// <summary>
        /// 编辑接受者
        /// </summary>
        public const string EditAccepter = "EditAccepter";
        /// <summary>
        /// 新建流程
        /// </summary>
        public const string NewFlowUnSend = "NewFlowUnSend";
        /// <summary>
        /// 撤销发送
        /// </summary>
        public const string UnSend = "UnSend";
        /// <summary>
        /// 报表
        /// </summary>
        public const string Rpt = "Rpt";
        /// <summary>
        /// 发送时
        /// </summary>
        public const string SendWhen = "SendWhen";
        /// <summary>
        /// 当前流程结束
        /// </summary>
        public const string End = "End";
        /// <summary>
        /// 当前流程完成
        /// </summary>
        public const string OverCurr = "OverCurr";
        /// <summary>
        /// 流程方向信息
        /// </summary>
        public const string CondInfo = "CondInfo";
        /// <summary>
        /// 一个节点完成
        /// </summary>
        public const string OneNodeSheetver = "OneNodeSheetver";
        /// <summary>
        /// 单据信息
        /// </summary>
        public const string BillInfo = "BillInfo";
        /// <summary>
        /// 文本信息(系统不会生成)
        /// </summary>
        public const string MsgOfText = "MsgOfText";
        /// <summary>
        /// 消息收听信息
        /// </summary>
        public const string ListenInfo = "ListenInfo";
        /// <summary>
        /// 流程是否结束？
        /// </summary>
        public const string IsStopFlow = "IsStopFlow";

        #region 系统变量
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string VarWorkID = "VarWorkID";
        /// <summary>
        /// 当前节点ID
        /// </summary>
        public const string VarCurrNodeID = "VarCurrNodeID";
        /// <summary>
        /// 当前节点名称
        /// </summary>
        public const string VarCurrNodeName = "VarCurrNodeName";
        /// <summary>
        /// 到达节点ID
        /// </summary>
        public const string VarToNodeID = "VarToNodeID";
        /// <summary>
        /// 到达的节点集合
        /// </summary>
        public const string VarToNodeIDs = "VarToNodeIDs";
        /// <summary>
        /// 到达节点名称
        /// </summary>
        public const string VarToNodeName = "VarToNodeName";
        /// <summary>
        /// 接受人集合的名称(用逗号分开)
        /// </summary>
        public const string VarAcceptersName = "VarAcceptersName";
        /// <summary>
        /// 接受人集合的ID(用逗号分开)
        /// </summary>
        public const string VarAcceptersID = "VarAcceptersID";
        /// <summary>
        /// 接受人集合的ID Name(用逗号分开)
        /// </summary>
        public const string VarAcceptersNID = "VarAcceptersNID";
        /// <summary>
        /// 子线程的WorkIDs
        /// </summary>
        public const string VarTreadWorkIDs = "VarTreadWorkIDs";
        #endregion 系统变量
    }
    /// <summary>
    /// 工作发送返回对象
    /// </summary>
    public class SendReturnObj
    {
        /// <summary>
        /// 消息标记
        /// </summary>
        public string MsgFlag = null;
        /// <summary>
        /// 消息标记描述
        /// </summary>
        public string MsgFlagDesc
        {
            get
            {
                if (MsgFlag == null)
                    throw new Exception("@没有标记");

                switch (MsgFlag)
                {
                    case SendReturnMsgFlag.VarAcceptersID:
                        return "接受人ID";
                    case SendReturnMsgFlag.VarAcceptersName:
                        return "接受人名称";
                    case SendReturnMsgFlag.VarAcceptersNID:
                        return "接受人ID集合";
                    case SendReturnMsgFlag.VarCurrNodeID:
                        return "当前节点ID";
                    case SendReturnMsgFlag.VarCurrNodeName:
                        return "接受人集合的名称(用逗号分开)";
                    case SendReturnMsgFlag.VarToNodeID:
                        return "到达节点ID";
                    case SendReturnMsgFlag.VarToNodeName:
                        return "到达节点名称";
                    case SendReturnMsgFlag.VarTreadWorkIDs:
                        return "子线程的WorkIDs";
                    case SendReturnMsgFlag.BillInfo:
                        return "单据信息";
                    case SendReturnMsgFlag.CCMsg:
                        return "抄送信息";
                    case SendReturnMsgFlag.CondInfo:
                        return "条件信息";
                    case SendReturnMsgFlag.CurrWorkOver:
                        return "当前的工作已经完成";
                    case SendReturnMsgFlag.EditAccepter:
                        return "编辑接受者";
                    case SendReturnMsgFlag.End:
                        return "当前的流程已经结束";
                    case SendReturnMsgFlag.FenLiuInfo:
                        return "分流信息";
                    case SendReturnMsgFlag.FlowOver:
                        return "当前流程已经完成";
                    case SendReturnMsgFlag.FlowOverByCond:
                        return "符合完成条件，流程完成.";
                    case SendReturnMsgFlag.HeLiuOver:
                        return "分流完成";
                    case SendReturnMsgFlag.MacthFlowOver:
                        return "符合工作流程完成条件";
                    case SendReturnMsgFlag.NewFlowUnSend:
                        return "新建流程";
                    case SendReturnMsgFlag.OverCurr:
                        return "当前流程完成";
                    case SendReturnMsgFlag.Rpt:
                        return "报表";
                    case SendReturnMsgFlag.SendSuccessMsg:
                        return "发送成功信息";
                    case SendReturnMsgFlag.SendSuccessMsgErr:
                        return "发送错误";
                    case SendReturnMsgFlag.SendWhen:
                        return "发送时";
                    case SendReturnMsgFlag.ToEmps:
                        return "到达人员";
                    case SendReturnMsgFlag.UnSend:
                        return "撤消发送";
                    case SendReturnMsgFlag.ToEmpExt:
                        return "到人员的扩展信息";
                    case SendReturnMsgFlag.VarWorkID:
                        return "工作ID";
                    case SendReturnMsgFlag.IsStopFlow:
                        return "流程是否结束";
                    case SendReturnMsgFlag.WorkRpt:
                        return "工作报告";
                    case SendReturnMsgFlag.WorkStartNode:
                        return "启动节点";
                    case SendReturnMsgFlag.AllotTask:
                        return "分配任务";
                    default:
                        return "信息:" + MsgFlag;
                    //  throw new Exception("@没有判断的标记...");
                }
            }
        }
        /// <summary>
        /// 消息类型
        /// </summary>
        public SendReturnMsgType HisSendReturnMsgType = SendReturnMsgType.Info;
        /// <summary>
        /// 消息内容
        /// </summary>
        public string MsgOfText = null;
        /// <summary>
        /// 消息内容Html
        /// </summary>
        public string MsgOfHtml = null;
        /// <summary>
        /// 发送消息
        /// </summary>
        public SendReturnObj()
        {
        }
    }
    /// <summary>
    /// 工作发送返回对象集合.
    /// </summary>
    public class SendReturnObjs:System.Collections.CollectionBase
    {
        #region 获取系统变量.
        public Int64 VarWorkID
        {
            get
            {
                foreach (SendReturnObj item in this)
                {
                    if (item.MsgFlag == SendReturnMsgFlag.VarWorkID)
                        return Int64.Parse(item.MsgOfText);
                }
                return 0;
            }
        }
        public bool IsStopFlow
        {
             get
            {
                foreach (SendReturnObj item in this)
                {
                    if (item.MsgFlag == SendReturnMsgFlag.IsStopFlow)
                    {
                        if (item.MsgOfText == "1")
                            return true;
                        else
                            return false;
                    }
                }
                throw new Exception("@没有找到系统变量IsStopFlow");
            }
        }

        /// <summary>
        /// 到达节点ID
        /// </summary>
        public int VarToNodeID
        {
            get
            {
                foreach (SendReturnObj item in this)
                {
                    if (item.MsgFlag == SendReturnMsgFlag.VarToNodeID)
                        return int.Parse( item.MsgOfText);
                }
                return 0;
            }
        }
        /// <summary>
        /// 到达节点IDs
        /// </summary>
        public string VarToNodeIDs
        {
            get
            {
                foreach (SendReturnObj item in this)
                {
                    if (item.MsgFlag == SendReturnMsgFlag.VarToNodeIDs)
                        return item.MsgOfText;
                }
                return null;
            }
        }
        /// <summary>
        /// 到达节点名称
        /// </summary>
        public string VarToNodeName
        {
            get
            {
                foreach (SendReturnObj item in this)
                {
                    if (item.MsgFlag == SendReturnMsgFlag.VarToNodeName)
                        return item.MsgOfText;
                }
                return "没有找到变量.";
            }
        }
        /// <summary>
        /// 到达的节点名称
        /// </summary>
        public string VarCurrNodeName
        {
            get
            {
                foreach (SendReturnObj item in this)
                {
                    if (item.MsgFlag == SendReturnMsgFlag.VarCurrNodeName)
                        return  item.MsgOfText;
                }
                return null;
            }
        }
        public int VarCurrNodeID
        {
            get
            {
                foreach (SendReturnObj item in this)
                {
                    if (item.MsgFlag == SendReturnMsgFlag.VarCurrNodeID)
                        return int.Parse( item.MsgOfText);
                }
                return 0;
            }
        }
        /// <summary>
        /// 接受人
        /// </summary>
        public string VarAcceptersName
        {
            get
            {
                foreach (SendReturnObj item in this)
                {
                    if (item.MsgFlag == SendReturnMsgFlag.VarAcceptersName)
                        return item.MsgOfText;
                }
                return null;
            }
        }
        /// <summary>
        /// 接受人IDs
        /// </summary>
        public string VarAcceptersID
        {
            get
            {
                foreach (SendReturnObj item in this)
                {
                    if (item.MsgFlag == SendReturnMsgFlag.VarAcceptersID)
                        return item.MsgOfText;
                }
                return null;
            }
        }
        /// <summary>
        /// 文本提示信息.
        /// </summary>
        public string MsgOfText
        {
            get
            {
                foreach (SendReturnObj item in this)
                {
                    if (item.MsgFlag == SendReturnMsgFlag.MsgOfText)
                        return item.MsgOfText;
                }
                return null;
            }
        }
        
        /// <summary>
        /// 分流向子线程发送时产生的子线程的WorkIDs, 多个有逗号分开.
        /// </summary>
        public string VarTreadWorkIDs
        {
            get
            {
                foreach (SendReturnObj item in this)
                {
                    if (item.MsgFlag == SendReturnMsgFlag.VarTreadWorkIDs)
                        return item.MsgOfText;
                }
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        public SendReturnObjs()
        {
        }
        /// <summary>
        /// 根据指定格式的字符串生成一个事例获取相关变量
        /// </summary>
        /// <param name="specText">指定格式的字符串</param>
        public SendReturnObjs(string specText)
        {
            this.LoadSpecText(specText);
        }
        /// <summary>
        /// 输出text消息
        /// </summary>
        public string OutMessageText = null;
        /// <summary>
        /// 输出html信息
        /// </summary>
        public string OutMessageHtml = null;
        /// <summary>
        /// 增加消息
        /// </summary>
        /// <param name="msgFlag">消息标记</param>
        /// <param name="msg">文本消息</param>
        /// <param name="msgOfHtml">html消息</param>
        /// <param name="type">消息类型</param>
        public void AddMsg(string msgFlag, string msg, string msgOfHtml, SendReturnMsgType type)
        {
            SendReturnObj obj = new SendReturnObj();
            obj.MsgFlag = msgFlag;
            obj.MsgOfText = msg;
            obj.MsgOfHtml = msgOfHtml;
            obj.HisSendReturnMsgType = type;
            foreach (SendReturnObj item in this)
            {
                if (item.MsgFlag == msgFlag)
                {
                    item.MsgFlag = msgFlag;
                    item.MsgOfText = msg;
                    item.MsgOfHtml = msgOfHtml;
                    item.HisSendReturnMsgType = type;
                    return;
                }
            }
            this.InnerList.Add(obj);
        }
        /// <summary>
        /// 转化成特殊的格式
        /// </summary>
        /// <returns></returns>
        public string ToMsgOfSpecText()
        {
            string msg = "";
            foreach (SendReturnObj item in this)
            {
                if (item.MsgOfText != null)
                {
                    msg += "$" + item.MsgFlag + "^" + (int)item.HisSendReturnMsgType + "^" + item.MsgOfText;
                }
            }

            //增加上 text信息。
            msg += "$MsgOfText^"+(int)SendReturnMsgType.Info +"^" + this.ToMsgOfText();

            msg.Replace("@@", "@");
            return msg;
        }
        /// <summary>
        /// 装载指定的文本，生成这个对象。
        /// </summary>
        /// <param name="text">指定格式的文本</param>
        public void LoadSpecText(string text)
        {
            string[] strs = text.Split('$');
            foreach (string str in strs)
            {
                string[] sp=str.Split('^');
                this.AddMsg(sp[0], sp[2], null, (SendReturnMsgType)int.Parse(sp[1]));
            }
        }
        /// <summary>
        /// 转化成text方式的消息，以方便识别不出来html的设备输出.
        /// </summary>
        /// <returns></returns>
        public string ToMsgOfText()
        {
            if (this.OutMessageText != null)
                return this.OutMessageText;

            string msg = "";
            foreach (SendReturnObj item in this)
            {
                if (item.HisSendReturnMsgType == SendReturnMsgType.SystemMsg)
                    continue;

                //特殊判断.
                if (item.MsgFlag == SendReturnMsgFlag.IsStopFlow)
                {
                    msg += "@" + item.MsgOfHtml;
                    continue; 
                }


                if (item.MsgOfText != null   )
                {
                    if (item.MsgOfText.Contains("<"))
                    {
#warning 不应该出现.
                      //  BP.DA.Log.DefaultLogWriteLineWarning("@文本信息里面有html标记:" + item.MsgOfText);
                        continue;
                    }
                    msg += "@" + item.MsgOfText;
                    continue; 
                }

            }
            msg.Replace("@@", "@");
            return msg;
        }
        /// <summary>
        /// 转化成html方式的消息，以方便html的信息输出.
        /// </summary>
        /// <returns></returns>
        public string ToMsgOfHtml()
        {
            if (this.OutMessageHtml != null)
                return this.OutMessageHtml;

            string msg = "";
            foreach (SendReturnObj item in this)
            {
                if (item.HisSendReturnMsgType != SendReturnMsgType.Info)
                    continue;

                if (item.MsgOfHtml != null)
                {
                    msg += "@" + item.MsgOfHtml;
                    continue;
                }

                if (item.MsgOfText != null)
                {
                    msg += "@" + item.MsgOfText;
                    continue;
                }
            }
            msg = msg.Replace("@@", "@");
            msg = msg.Replace("@@", "@");
            if (msg == "@")
                return "@流程已经完成.";

            return msg;
        }
    }
}
