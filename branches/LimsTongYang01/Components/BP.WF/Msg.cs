using System;
using System.Collections;
using System.Web.Hosting;
using BP.Web;

namespace BP.WF
{
    /// <summary>
    /// MsgsManager
    /// </summary>
    public class MsgsManager
    {
        /// <summary> 
        /// 删除工作by工作ID
        /// </summary>
        /// <param name="workId"></param>
        public static void DeleteByWorkID(Int64 workId)
        {
            System.Web.HttpContext.Current.Application.Lock();
            Msgs msgs = (Msgs)System.Web.HttpContext.Current.Application["WFMsgs"];
            if (msgs == null)
            {
                msgs = new Msgs();
                System.Web.HttpContext.Current.Application["WFMsgs"] = msgs;
            }
            // 清除全部的工作ID=workid 的消息。
            msgs.ClearByWorkID(workId);
            System.Web.HttpContext.Current.Application.UnLock();
        }
        /// <summary>
        /// 增加信息
        /// </summary>
        /// <param name="wls">工作者集合</param>
        /// <param name="flowName">流程名称</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="title">标题</param>
        public static void AddMsgs(GenerWorkerLists wls, string flowName, string nodeName, string title)
        {
            return;

            System.Web.HttpContext.Current.Application.Lock();
            Msgs msgs = (Msgs)System.Web.HttpContext.Current.Application["WFMsgs"];
            if (msgs == null)
            {
                msgs = new Msgs();
                System.Web.HttpContext.Current.Application["WFMsgs"] = msgs;
            }
            // 清除全部的工作ID=workid 的消息。
            msgs.ClearByWorkID(wls[0].GetValIntByKey("WorkID"));
            foreach (GenerWorkerList wl in wls)
            {
                if (wl.FK_Emp == WebUser.No)
                    continue;
                //msgs.AddMsg(wl.WorkID,wl.FK_Node,wl.FK_Emp,"来自流程["+flowName+"]节点["+nodeName+"]工作节点标题为["+title+"]的消息。");
            }
            System.Web.HttpContext.Current.Application.UnLock();
        }
        /// <summary>
        /// sss
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public static Msgs GetMsgsByEmpID_del(int empId)
        {
            Msgs msgs = (Msgs)System.Web.HttpContext.Current.Application["WFMsgs"];
            if (msgs == null)
            {
                msgs = new Msgs();
                System.Web.HttpContext.Current.Application["WFMsgs"] = msgs;
            }
            return msgs.GetMsgsByEmpID_del(empId);
        }
        /// <summary>
        /// 取出消息的个数
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public static int GetMsgsCountByEmpID(int empId)
        {
            string sql = "select COUNT(*) from v_wf_msg WHERE FK_Emp='" + WebUser.No + "'";
            return  BP.DA.DBAccess.RunSQLReturnValInt(sql);
        }
        /// <summary>
        /// 清除信息
        /// </summary>
        /// <param name="empId"></param>
        public static void ClearMsgsByEmpID_(int empId)
        {
            System.Web.HttpContext.Current.Application.Lock();
            Msgs msgs = (Msgs)System.Web.HttpContext.Current.Application["WFMsgs"];
            msgs.ClearByEmpId_del(empId);
            System.Web.HttpContext.Current.Application.UnLock();
        }
        /// <summary>
        /// 初始化全部的消息。
        /// </summary>
        public static void InitMsgs()
        {
        }
    }
    /// <summary>
    /// Msg 的摘要说明。
    /// </summary>
    public class Msg
    {
        #region 属性
        /// <summary>
        /// 声音文件
        /// </summary>
        private string _SoundUrl = Glo.CCFlowAppPath + "WF/Sound/ring.wav";
        /// <summary>
        /// 声音文件
        /// </summary>
        public string SoundUrl
        {
            get
            {
                return _SoundUrl;
            }
            set
            {
                _SoundUrl = value;
            }
        }
        /// <summary>
        /// _IsOpenSound
        /// </summary>
        private bool _IsOpenSound = true;
        /// <summary>
        /// IsOpenSound
        /// </summary>
        public bool IsOpenSound
        {
            get
            {
                if (this._IsOpenSound == false)
                {
                    return false;
                }
                else
                {
                    this._IsOpenSound = false;
                    return true;
                }
            }
        }
        /// <summary>
        /// _WorkID
        /// </summary>
        private int _WorkID = 0;
        /// <summary>
        /// _NodeId
        /// </summary>
        private int _NodeId = 0;
        /// <summary>
        /// _Info
        /// </summary>
        private string _Info = "";
        /// <summary>
        /// _ToEmpId
        /// </summary>
        private int _ToEmpId = 0;
        /// <summary>
        /// 信息
        /// </summary>
        public string Info
        {
            get
            {
                return this._Info;
            }
            set
            {
                _Info = value;
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public int WorkID
        {
            get
            {
                return _WorkID;
            }
            set
            {
                _WorkID = value;
            }
        }
        /// <summary>
        /// NodeID
        /// </summary>
        public int NodeId
        {
            get
            {
                return _NodeId;
            }
            set
            {
                _NodeId = value;
            }
        }
        /// <summary>
        /// ToEmpId
        /// </summary>
        public int ToEmpId
        {
            get
            {
                return _ToEmpId;
            }
            set
            {
                _ToEmpId = value;
            }
        }
        #endregion

        /// <summary>
        /// 信息
        /// </summary>
        public Msg() { }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="workId"></param>
        /// <param name="nodeId"></param>
        /// <param name="toEmpId"></param>
        /// <param name="info"></param>
        public Msg(int workId, int nodeId, int toEmpId, string info)
        {
            this.WorkID = workId;
            this.NodeId = nodeId;
            this.ToEmpId = toEmpId;
            this.Info = info;
        }
    }
    /// <summary>
    /// 消息集合
    /// </summary>
    public class Msgs : ArrayList
    {

        #region 增加消息
        /// <summary>
        /// 增加消息
        /// </summary>
        /// <param name="workId"></param>
        /// <param name="nodeId"></param>
        /// <param name="toEmpId"></param>
        /// <param name="info"></param>
        public void AddMsg(int workId, int nodeId, int toEmpId, string info)
        {
            return;
            Msg msg = new Msg();
            msg.WorkID = workId;
            msg.NodeId = nodeId;
            msg.ToEmpId = toEmpId;
            msg.Info = info;
            this.Add(msg);
        }
        /// <summary>
        /// 增加消息
        /// </summary>
        /// <param name="msg">消息</param>
        public void AddMsg(Msg msg)
        {
            return;
            this.Add(msg);
        }
        #endregion

        #region 关于消息集合的操作
        /// <summary>
        /// 安工作ID 清除消息。
        /// </summary>
        /// <param name="workId"></param>
        public void ClearByWorkID(Int64 workId)
        {
            return;
            Msgs ens = this.GetMsgsByWorkID(workId);
            foreach (Msg msg in ens)
            {
                this.Remove(msg);
            }
        }
        /// <summary>
        /// 清除工作人员信息
        /// </summary>
        /// <param name="empId"></param>
        public void ClearByEmpId_del(int empId)
        {
            return;
            Msgs ens = this.GetMsgsByEmpID_del(empId);
            foreach (Msg msg in ens)
            {
                this.Remove(msg);
            }
        }
        /// <summary>
        /// 清除工作人员信息
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        public Msgs GetMsgsByWorkID(Int64 workId)
        {
            return null;
            Msgs ens = new Msgs();
            foreach (Msg msg in this)
            {
                if (msg.WorkID == workId)
                    ens.AddMsg(msg);
            }
            return ens;
        }
        /// <summary>
        /// sss
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public Msgs GetMsgsByEmpID_del(int empId)
        {
            //return ;
            Msgs ens = new Msgs();
            foreach (Msg msg in this)
            {
                if (msg.ToEmpId == empId)
                    ens.AddMsg(msg);
            }
            return ens;
        }
        /// <summary>
        /// 取得信息的数量。
        /// </summary>
        /// <param name="empId">工作人员</param>
        /// <returns>信息个数</returns>
        public int GetMsgsCountByEmpID(int empId)
        {
            return 0;
            int i = 0;
            //bool isHaveNew=false;
            int newMsgNum = 0;
            foreach (Msg msg in this)
            {
                if (msg.ToEmpId == empId)
                {
                    if (msg.IsOpenSound)
                        newMsgNum++;
                    i++;
                }
            }
            if (newMsgNum > 0)
            {
                //if (WebUser.IsSoundAlert)				 
                //    System.Web.HttpContext.Current.Response.Write("<bgsound src='"+BP.Sys.Glo.Request.ApplicationPath+Web.WebUser.NoticeSound+"' loop=1 >"  );
                //if (WebUser.IsTextAlert)
                //    BP.Sys.PubClass.ResponseWriteBlueMsg("您有["+newMsgNum+"]个新工作." );
                //System.Web.HttpContext.Current.Response.Write("<bgsound src='"+BP.Sys.Glo.Request.ApplicationPath+Web.WebUser.NoticeSound+"' loop=1 >"  );

            }
            return i;
        }
        #endregion

        /// <summary>
        /// 消息s
        /// </summary>
        public Msgs()
        {
        }

        /// <summary>
        /// 根据位置取得数据
        /// </summary>
        public new Msg this[int index]
        {
            get
            {
                return (Msg)this[index];
            }
        }

    }
    /// <summary>
    /// 用户消息
    /// </summary>
    public class UserMsgs
    {
        #region 属性
        /// <summary>
        /// _IsOpenSound
        /// </summary>
        private bool _IsOpenSound = false;
        /// <summary>
        /// _IsOpenSound
        /// </summary>
        public bool IsOpenSound
        {
            get
            {
                if (this._IsOpenSound == false)
                {
                    return false;
                }
                else
                {
                    this._IsOpenSound = false;
                    return true;
                }
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 用户消息
        /// </summary>
        public UserMsgs()
        {
        }
        /// <summary>
        /// 用户消息
        /// </summary>
        /// <param name="empId"></param>
        public UserMsgs(int empId)
        {
        }
        #endregion
    }
}
