﻿using System;
using System.IO;
using System.Collections;
using System.Data;
using System.Text;
using BP.DA;
using BP.Sys;
using BP.Port;
using BP.En;
using BP.WF.Template;
using BP.WF.Template;
using BP.WF.Data;
using BP.Web;
using Microsoft.Win32;
using System.Collections.Generic;

namespace BP.WF
{
    /// <summary>
    /// 流程
    /// 记录了流程的信息．
    /// 流程的编号，名称，建立时间．
    /// </summary>
    public class Flow : BP.En.EntityNoName
    {
        #region 参数属性.
        /// <summary>
        /// 最大值x
        /// </summary>
        public int MaxX
        {
            get
            {
                int i = this.GetParaInt("MaxX");
                if (i == 0)
                    this.GenerMaxXY();
                else
                    return i;
                return this.GetParaInt("MaxX");
            }
            set
            {
                this.SetPara("MaxX", value);
            }
        }
        /// <summary>
        /// 最大值Y
        /// </summary>
        public int MaxY
        {
            get
            {
                int i = this.GetParaInt("MaxY");
                if (i == 0)
                    this.GenerMaxXY();
                else
                    return i;

                return this.GetParaInt("MaxY");
            }
            set
            {
                this.SetPara("MaxY", value);
            }
        }
        private void GenerMaxXY()
        {
            //int x1 = DBAccess.RunSQLReturnValInt("SELECT MAX(X) FROM WF_Node WHERE FK_Flow='" + this.No + "'", 0);
            //int x2 = DBAccess.RunSQLReturnValInt("SELECT MAX(X) FROM WF_NodeLabelNode WHERE FK_Flow='" + this.No + "'", 0);
            //this.MaxY = DBAccess.RunSQLReturnValInt("SELECT MAX(Y) FROM WF_Node WHERE FK_Flow='" + this.No + "'", 0);
        }
        /// <summary>
        /// 待办的业务字段 2019-09-25 by zhoupeng
        /// </summary>
        public string BuessFields
        {
            get
            {
                return this.GetParaString(FlowAttr.BuessFields);
            }
            set
            {
                this.SetPara(FlowAttr.BuessFields, value);
            }
        }
        #endregion 参数属性.

        #region 业务数据表同步属性.
        /// <summary>
        /// 同步方式
        /// </summary>
        public DataDTSWay DTSWay
        {
            get
            {
                return (DataDTSWay)this.GetValIntByKey(FlowAttr.DataDTSWay);
            }
            set
            {
                this.SetValByKey(FlowAttr.DataDTSWay, (int)value);
            }
        }
        public FlowDTSTime DTSTime
        {
            get
            {
                return (FlowDTSTime)this.GetValIntByKey(FlowAttr.DTSTime);
            }
            set
            {
                this.SetValByKey(FlowAttr.DTSTime, (int)value);
            }
        }

        /// <summary>
        /// 数据源
        /// </summary>
        public string DTSDBSrc
        {
            get
            {
                string str = this.GetParaString(FlowAttr.DTSDBSrc);
                if (DataType.IsNullOrEmpty(str))
                    return "local";
                return str;
            }
            set
            {
                this.SetPara(FlowAttr.DTSDBSrc, value);
            }
        }
        /// <summary>
        /// 业务表
        /// </summary>
        public string DTSBTable
        {
            get
            {
                return this.GetParaString(FlowAttr.DTSBTable);
            }
            set
            {
                this.SetPara(FlowAttr.DTSBTable, value);
            }
        }
        public string DTSBTablePK
        {
            get
            {
                return this.GetParaString(FlowAttr.DTSBTablePK);
            }
            set
            {
                this.SetPara(FlowAttr.DTSBTablePK, value);
            }
        }
        /// <summary>
        /// 要同步的节点s
        /// </summary>
        public string DTSSpecNodes
        {
            get
            {
                return this.GetParaString(FlowAttr.DTSSpecNodes);
            }
            set
            {
                this.SetPara(FlowAttr.DTSSpecNodes, value);
            }
        }
        /// <summary>
        /// 同步的字段对应关系.
        /// </summary>
        public string DTSFields
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DTSFields);
            }
            set
            {
                this.SetValByKey(FlowAttr.DTSFields, value);
            }
        }
        #endregion 业务数据表同步属性.

        #region 基础属性.
        /// <summary>
        /// 消息推送.
        /// </summary>
        public PushMsgs PushMsgs
        {
            get
            {
                var ens = this.GetEntitiesAttrFromAutoNumCash(new PushMsgs(),
                  PushMsgAttr.FK_Flow, this.No);
                return ens as PushMsgs;
            }
        }
        /// <summary>
        /// 流程事件实体
        /// </summary>
        public string FlowEventEntity
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.FlowEventEntity);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowEventEntity, value);
            }
        }
        /// <summary>
        /// 流程表单类型
        /// </summary>
        public FlowFrmType FlowFrmType
        {
            get
            {
                return (FlowFrmType)this.GetValIntByKey(FlowAttr.FlowFrmType);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowFrmType, (int)value);
            }
        }
        /// <summary>
        /// url
        /// </summary>
        public string FrmUrl
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.FrmUrl);
            }
            set
            {
                this.SetValByKey(FlowAttr.FrmUrl, value);
            }
        }

        /// <summary>
        /// 流程标记
        /// </summary>
        public string FlowMark
        {
            get
            {
                string str = this.GetValStringByKey(FlowAttr.FlowMark);
                if (str == "")
                    return this.No;
                return str;
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowMark, value);
            }
        }
        public string OrgNo
        {
            get
            {
                string str = this.GetValStringByKey(FlowAttr.OrgNo);
                return str;
            }
            set
            {
                this.SetValByKey(FlowAttr.OrgNo, value);
            }
        }
        /// <summary>
        /// 节点图形类型
        /// </summary>
        public int ChartType
        {
            get
            {
                return this.GetValIntByKey(FlowAttr.ChartType);
            }
            set
            {
                this.SetValByKey(FlowAttr.ChartType, value);
            }
        }
        #endregion

        #region 发起限制.
        /// <summary>
        /// 发起限制.
        /// </summary>
        public StartLimitRole StartLimitRole
        {
            get
            {
                return (StartLimitRole)this.GetValIntByKey(FlowAttr.StartLimitRole);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartLimitRole, (int)value);
            }
        }
        /// <summary>
        /// 发起内容
        /// </summary>
        public string StartLimitPara
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.StartLimitPara);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartLimitPara, value);
            }
        }
        public string StartLimitAlert
        {
            get
            {
                string s = this.GetValStringByKey(FlowAttr.StartLimitAlert);
                if (s == "")
                    return "您已经启动过该流程，不能重复启动。";
                return s;
            }
            set
            {
                this.SetValByKey(FlowAttr.StartLimitAlert, value);
            }
        }
        /// <summary>
        /// 限制触发时间
        /// </summary>
        public StartLimitWhen StartLimitWhen
        {
            get
            {
                return (StartLimitWhen)this.GetValIntByKey(FlowAttr.StartLimitWhen);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartLimitWhen, (int)value);
            }
        }
        #endregion 发起限制.

        #region 导航模式
        /// <summary>
        /// 发起导航方式
        /// </summary>
        public StartGuideWay StartGuideWay
        {
            get
            {
                return (StartGuideWay)this.GetValIntByKey(FlowAttr.StartGuideWay);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuideWay, (int)value);
            }
        }
        /// <summary>
        /// 右侧的超链接
        /// </summary>
        public string StartGuideLink
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.StartGuideLink);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuideLink, value);
            }
        }
        /// <summary>
        /// 标签
        /// </summary>
        public string StartGuideLab
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.StartGuideLab);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuideLab, value);
            }
        }
        /// <summary>
        /// 前置导航参数1
        /// </summary>
        public string StartGuidePara1
        {
            get
            {
                if (this.StartGuideWay == Template.StartGuideWay.BySelfUrl)
                {
                    string str = this.GetValStringByKey(FlowAttr.StartGuidePara1);
                    if (str.Contains("?") == false)
                        str = str + "?1=2";
                    return str.Replace("~", "'");
                }
                else
                {
                    return this.GetValStringByKey(FlowAttr.StartGuidePara1);
                }
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuidePara1, value);
            }

        }

        /// <summary>
        /// 流程发起参数2
        /// </summary>
        public string StartGuidePara2
        {
            get
            {
                string str = this.GetValStringByKey(FlowAttr.StartGuidePara2);
                str = str.Replace("~", "'");
                if (DataType.IsNullOrEmpty(str) == null)
                {
                    if (this.StartGuideWay == BP.WF.Template.StartGuideWay.ByHistoryUrl)
                    {
                    }
                }
                return str;
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuidePara2, value);
            }
        }
        /// <summary>
        /// 流程发起参数3
        /// </summary>
        public string StartGuidePara3
        {
            get
            {
                return this.GetValStrByKey(FlowAttr.StartGuidePara3);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuidePara3, value);
            }
        }
        /// <summary>
        /// 是否启用数据重置按钮？
        /// </summary>
        public bool IsResetData
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsResetData);
            }
        }
        /// <summary>
        /// 是否启用导入历史数据按钮?
        /// </summary>
        public bool IsImpHistory
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsImpHistory);
            }
        }
        /// <summary>
        /// 是否自动装载上一笔数据?
        /// </summary>
        public bool IsLoadPriData
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsLoadPriData);
            }
        }
        #endregion

        #region 其他属性
        /// <summary>
        /// 流程删除规则
        /// </summary>
        public FlowDeleteRole FlowDeleteRole
        {
            get
            {
                return (FlowDeleteRole)this.GetValIntByKey(FlowAttr.FlowDeleteRole);
            }
        }
        /// <summary>
        /// 草稿规则
        /// </summary>
        public DraftRole DraftRole
        {
            get
            {
                return (DraftRole)this.GetValIntByKey(FlowAttr.Draft);
            }
            set
            {
                this.SetValByKey(FlowAttr.Draft, (int)value);
            }
        }
        public string Tag = null;
        /// <summary>
        /// 运行类型
        /// </summary>
        public FlowRunWay HisFlowRunWay
        {
            get
            {
                return (FlowRunWay)this.GetValIntByKey(FlowAttr.FlowRunWay);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowRunWay, (int)value);
            }
        }
        /// <summary>
        /// 运行对象
        /// </summary>
        public string RunObj
        {
            get
            {
                return this.GetValStrByKey(FlowAttr.RunObj);
            }
            set
            {
                this.SetValByKey(FlowAttr.RunObj, value);
            }
        }
        /// <summary>
        /// 时间点规则
        /// </summary>
        public SDTOfFlowRole SDTOfFlowRole
        {
            get
            {
                return (SDTOfFlowRole)this.GetValIntByKey(FlowAttr.SDTOfFlowRole);
            }
        }
        /// <summary>
        /// 按照SQL来计算流程完成时间
        /// </summary>
        public string SDTOfFlowRoleSQL
        {
            get
            {
                return this.GetValStrByKey(FlowAttr.SDTOfFlowRoleSQL);
            }
        }
        /// <summary>
        /// 流程部门数据查询权限控制方式
        /// </summary>
        public FlowDeptDataRightCtrlType HisFlowDeptDataRightCtrlType
        {
            get
            {
                return (FlowDeptDataRightCtrlType)this.GetValIntByKey(FlowAttr.DRCtrlType);
            }
            set
            {
                this.SetValByKey(FlowAttr.DRCtrlType, value);
            }
        }
        /// <summary>
        /// 流程应用类型
        /// </summary>
        public FlowAppType FlowAppType
        {
            get
            {
                return (FlowAppType)this.GetValIntByKey(FlowAttr.FlowAppType);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowAppType, (int)value);
            }
        }
        /// <summary>
        /// 流程备注的表达式
        /// </summary>
        public string FlowNoteExp
        {
            get
            {
                return this.GetValStrByKey(FlowAttr.FlowNoteExp);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowNoteExp, value);
            }
        }
        /// <summary>
        /// 是否可以在手机里启用
        /// </summary>
        public bool IsStartInMobile
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsStartInMobile);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsStartInMobile, value);
            }
        }
        #endregion 业务处理

        #region 创建新工作.
        /// <summary>
        /// 创建新工作web方式调用的
        /// </summary>
        /// <returns></returns>
        public Work NewWork()
        {
            return NewWork(WebUser.No);
        }
        /// <summary>
        /// 创建新工作.web方式调用的
        /// </summary>
        /// <param name="empNo">人员编号</param>
        /// <returns></returns>
        public Work NewWork(string empNo)
        {
            Emp emp = new Emp(empNo);
            return NewWork(emp, null);
        }
        /// <summary>
        /// 产生一个开始节点的新工作
        /// </summary>
        /// <param name="emp">发起人</param>
        /// <param name="paras">参数集合,如果是CS调用，要发起子流程，要从其他table里copy数据,就不能从request里面取,可以传递为null.</param>
        /// <returns>返回的Work.</returns>
        public Work NewWork(Emp emp, Hashtable paras)
        {
            // 检查是否可以发起该流程？
            if (Glo.CheckIsCanStartFlow_InitStartFlow(this) == false)
                throw new Exception("err@您违反了该流程的【" + this.StartLimitRole + "】限制规则。" + this.StartLimitAlert);

            //如果是bs系统.
            if (paras == null)
                paras = new Hashtable();
            if (SystemConfig.IsBSsystem == true)
            {
                foreach (string k in HttpContextHelper.RequestParamKeys)
                {
                    if (k == "OID" || k == "WorkID" || k == null)
                        continue;

                    if (paras.ContainsKey(k))
                        paras[k] = HttpContextHelper.RequestParams(k);// BP.Sys.Glo.Request.QueryString[k];
                    else
                        paras.Add(k, HttpContextHelper.RequestParams(k));
                }
            }

            //开始节点.
            BP.WF.Node nd = new BP.WF.Node(this.StartNodeID);

            //从草稿里看看是否有新工作？
            Work wk = nd.HisWork;
            try
            {
                wk.ResetDefaultVal();
            }
            catch (Exception ex)
            {
                wk.ResetDefaultVal();
            }

            string dbstr = SystemConfig.AppCenterDBVarStr;

            Paras ps = new Paras();
            GERpt rpt = this.HisGERpt;

            //是否新创建的WorkID
            bool IsNewWorkID = false;
            /*如果要启用草稿,就创建一个新的WorkID .*/
            if (this.DraftRole != Template.DraftRole.None && nd.IsStartNode)
                IsNewWorkID = true;

            string errInfo = "";
            try
            {
                //从报表里查询该数据是否存在？
                if (this.IsGuestFlow == true && DataType.IsNullOrEmpty(GuestUser.No) == false)
                {
                    /*是客户参与的流程，并且具有客户登陆的信息。*/
                    ps.SQL = "SELECT OID,FlowEndNode FROM " + this.PTable + " WHERE GuestNo=" + dbstr + "GuestNo AND WFState=" + dbstr + "WFState ";
                    ps.Add(GERptAttr.GuestNo, GuestUser.No);
                    ps.Add(GERptAttr.WFState, (int)WFState.Blank);
                    DataTable dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count > 0 && IsNewWorkID == false)
                    {
                        wk.OID = Int64.Parse(dt.Rows[0][0].ToString());
                        int nodeID = int.Parse(dt.Rows[0][1].ToString());
                        if (nodeID != this.StartNodeID)
                        {
                            string error = "@这里出现了blank的状态下流程运行到其它的节点上去了的情况。";
                            Log.DefaultLogWriteLineError(error);
                            throw new Exception(error);
                        }
                    }
                }
                else
                {

                    ps.SQL = "SELECT WorkID,FK_Node FROM WF_GenerWorkFlow WHERE WFState=0 AND Starter=" + dbstr + "FlowStarter AND FK_Flow=" + dbstr + "FK_Flow ";
                    ps.Add(GERptAttr.FlowStarter, emp.No);
                    ps.Add(GenerWorkFlowAttr.FK_Flow, this.No);
                    DataTable dt = DBAccess.RunSQLReturnTable(ps);

                    //如果没有启用草稿，并且存在草稿就取第一条
                    if (dt.Rows.Count > 0)
                    {
                        wk.OID = Int64.Parse(dt.Rows[0][0].ToString());
                        wk.RetrieveFromDBSources();
                        int nodeID = int.Parse(dt.Rows[0][1].ToString());
                        if (nodeID != this.StartNodeID)
                        {
                            string error = "err@这里出现了blank的状态下流程运行到其它的节点上去了的情况，当前停留节点:" + nodeID;
                            Log.DefaultLogWriteLineError(error);
                        }
                    }
                }

                //启用草稿或空白就创建WorkID
                if (wk.OID == 0)
                {
                    /* 说明没有空白,就创建一个空白..*/
                    wk.ResetDefaultVal();
                    wk.Rec = WebUser.No;

                    //   wk.SetValByKey(GERptAttr.RecText, emp.Name);
                    //  wk.SetValByKey(GERptAttr.Emps, emp.No);

                    //  wk.SetValByKey(WorkAttr.RDT, DataType.CurrentDataTime);
                    // wk.SetValByKey(WorkAttr.CDT, DataType.CurrentDataTime);

                    wk.SetValByKey(GERptAttr.WFState, (int)WFState.Blank);

                    wk.OID = DBAccess.GenerOID("WorkID"); /*这里产生WorkID ,这是唯一产生WorkID的地方.*/

                    //把尽量可能的流程字段放入，否则会出现冲掉流程字段属性.
                    wk.SetValByKey(GERptAttr.FK_NY, DataType.CurrentYearMonth);
                    wk.SetValByKey(GERptAttr.FK_Dept, emp.FK_Dept);
                    wk.FID = 0;

                    try
                    {
                        wk.DirectInsert();
                    }
                    catch (Exception ex)
                    {
                        wk.CheckPhysicsTable();
                        //wk.DirectInsert();
                    }

                    //设置参数.
                    foreach (string k in paras.Keys)
                    {
                        rpt.SetValByKey(k, paras[k]);
                    }

                    if (this.PTable.Equals(wk.EnMap.PhysicsTable)==true)
                    {
                        /*如果开始节点表与流程报表相等.*/
                        rpt.OID = wk.OID;
                        rpt.RetrieveFromDBSources();
                        rpt.FID = 0;
                        rpt.FlowStartRDT = DataType.CurrentDataTime;
                        rpt.Title = BP.WF.WorkFlowBuessRole.GenerTitle(this, wk);
                        //WebUser.No + "," + BP.Web.WebUser.Name + "在" + DataType.CurrentDataCNOfShort + "发起.";
                        rpt.WFState = WFState.Blank;
                        rpt.FlowStarter = emp.No;
                        rpt.FK_NY = DataType.CurrentYearMonth;
                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserNameOnly)
                            rpt.FlowEmps = "@" + emp.Name + "@";

                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                            rpt.FlowEmps = "@" + emp.No + "@";

                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                            rpt.FlowEmps = "@" + emp.No + "," + emp.Name + "@";

                        rpt.FlowEnderRDT = DataType.CurrentDataTime;
                        rpt.FlowStartRDT = DataType.CurrentDataTime;

                        rpt.FK_Dept = emp.FK_Dept;
                        rpt.FlowEnder = emp.No;
                        rpt.FlowEndNode = this.StartNodeID;
                        rpt.FlowStarter = emp.No;
                        rpt.WFState = WFState.Blank;
                        rpt.FID = 0;
                        rpt.Update();
                    }
                    else
                    {
                        rpt.OID = wk.OID;
                        rpt.FID = 0;
                        rpt.FlowStartRDT = DataType.CurrentDataTime;
                        rpt.FlowEnderRDT = DataType.CurrentDataTime;

                        rpt.Title = BP.WF.WorkFlowBuessRole.GenerTitle(this, wk);
                        // rpt.Title = WebUser.No + "," + BP.Web.WebUser.Name + "在" + DataType.CurrentDataCNOfShort + "发起.";

                        rpt.WFState = WFState.Blank;
                        rpt.FlowStarter = emp.No;

                        rpt.FlowEndNode = this.StartNodeID;
                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserNameOnly)
                            rpt.FlowEmps = "@" + emp.Name + "@";

                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                            rpt.FlowEmps = "@" + emp.No + "@";

                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                            rpt.FlowEmps = "@" + emp.No + "," + emp.Name + "@";

                        rpt.FK_NY = DataType.CurrentYearMonth;
                        rpt.FK_Dept = emp.FK_Dept;
                        rpt.FlowEnder = emp.No;
                        rpt.FlowStarter = emp.No;
                        rpt.InsertAsOID(wk.OID);
                    }

                    //调用 OnCreateWorkID的方法.  add by zhoupeng 2016.12.4 for LIMS.
                    ExecEvent.DoFlow(EventListFlow.FlowOnCreateWorkID, wk, nd, null);
                }

                if (wk.OID != 0)
                {
                    rpt.OID = wk.OID;
                    rpt.RetrieveFromDBSources();

                    rpt.FID = 0;
                    rpt.FlowStartRDT = DataType.CurrentDataTime;
                    rpt.FlowEnderRDT = DataType.CurrentDataTime;

                    //在发送的时候有更新.
                    //   rpt.DirectUpdate();
                }
            }
            catch (Exception ex)
            {
                wk.CheckPhysicsTable();

                //检查报表.
                this.CheckRpt();
                throw new Exception("@创建工作失败：请您刷新一次，如果问题仍然存在请反馈给管理员，技术信息：" + ex.StackTrace + " @ 技术信息:" + ex.Message);
            }

            //在创建WorkID的时候调用的事件.
            ExecEvent.DoFlow(EventListFlow.FlowOnCreateWorkID, wk, nd, null);

            #region copy数据.
            // 记录这个id ,不让其它在复制时间被修改。
            Int64 newOID = wk.OID;
            if (IsNewWorkID == true)
            {
                // 处理传递过来的参数。
                int i = 0;

                string expKeys = "OID,DoType,HttpHandlerName,DoMethod,t,";
                foreach (string k in paras.Keys)
                {
                    if (expKeys.IndexOf("," + k + ",") != -1)
                        continue;

                    var str = paras[k] as string;
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;
                    i++;
                    wk.SetValByKey(k, str);
                }

                if (i >= 3)
                {
                    wk.DirectUpdate();
                }
            }
            #endregion copy数据.

            #region 处理删除草稿的需求。
            if (paras.ContainsKey(StartFlowParaNameList.IsDeleteDraft) && paras[StartFlowParaNameList.IsDeleteDraft].ToString() == "1")
            {
                /*是否要删除Draft */
                Int64 oid = wk.OID;
                try
                {
                    //wk.ResetDefaultValAllAttr();
                    wk.DirectUpdate();
                }
                catch (Exception ex)
                {
                    wk.Update();
                    Log.DebugWriteError("创建新工作错误，但是屏蔽了异常,请检查默认值的问题：" + ex.Message);
                }

                MapDtls dtls = wk.HisMapDtls;
                foreach (MapDtl dtl in dtls)
                    DBAccess.RunSQL("DELETE FROM " + dtl.PTable + " WHERE RefPK='" + oid + "'");

                //删除附件数据。
                DBAccess.RunSQL("DELETE FROM Sys_FrmAttachmentDB WHERE FK_MapData='ND" + wk.NodeID + "' AND RefPKVal='" + wk.OID + "'");
                wk.OID = newOID;
            }
            #endregion 处理删除草稿的需求。

            #region 处理开始节点, 如果传递过来 FromTableName 就是要从这个表里copy数据。
            if (paras.ContainsKey("FromTableName"))
            {
                string tableName = paras["FromTableName"].ToString();
                string tablePK = paras["FromTablePK"].ToString();
                string tablePKVal = paras["FromTablePKVal"].ToString();

                DataTable dt = DBAccess.RunSQLReturnTable("SELECT * FROM " + tableName + " WHERE " + tablePK + "='" + tablePKVal + "'");
                if (dt.Rows.Count == 0)
                    throw new Exception("@利用table传递数据错误，没有找到指定的行数据，无法为用户填充数据。");

                string innerKeys = ",OID,RDT,CDT,FID,WFState,WorkID,WORKID,WFSTATE";
                foreach (DataColumn dc in dt.Columns)
                {
                    if (innerKeys.Contains("," + dc.ColumnName.ToUpper() + ","))
                        continue;

                    wk.SetValByKey(dc.ColumnName, dt.Rows[0][dc.ColumnName].ToString());
                    rpt.SetValByKey(dc.ColumnName, dt.Rows[0][dc.ColumnName].ToString());
                }
                rpt.Update();
            }
            #endregion 处理开始节点, 如果传递过来 FromTableName 就是要从这个表里copy数据。

            #region 获取特殊标记变量
            // 获取特殊标记变量.
            string PFlowNo = null;
            string PNodeIDStr = null;
            string PWorkIDStr = null;
            string PFIDStr = null;

            string CopyFormWorkID = null;
            if (paras.ContainsKey("CopyFormWorkID") == true)
            {
                CopyFormWorkID = paras["CopyFormWorkID"].ToString();
                PFlowNo = this.No;
                PNodeIDStr = paras["CopyFormNode"].ToString();
                PWorkIDStr = CopyFormWorkID;
                PFIDStr = "0";
            }

            if (paras.ContainsKey("PWorkID") == true && paras.ContainsKey("PNodeID") == true
                && Int64.Parse(paras["PWorkID"].ToString()) != 0
                && Int32.Parse(paras["PNodeID"].ToString()) != 0)
            {
                if (paras["PFlowNo"] == null)
                {
                    Int32 nodeId = Int32.Parse(paras["PNodeID"].ToString());
                    Node node = new Node(nodeId);
                    PFlowNo = node.FK_Flow;

                }
                else
                    PFlowNo = paras["PFlowNo"].ToString();

                PNodeIDStr = paras["PNodeID"].ToString();
                PWorkIDStr = paras["PWorkID"].ToString();
                PFIDStr = "0";
                if (paras.ContainsKey("PFID") == true)
                    PFIDStr = paras["PFID"].ToString(); //父流程.
            }
            #endregion 获取特殊标记变量

            #region  判断是否装载上一条数据.
            if (this.IsLoadPriData == true && this.StartGuideWay == BP.WF.Template.StartGuideWay.None)
            {
                /* 如果需要从上一个流程实例上copy数据. */
                string sql = "SELECT OID FROM " + this.PTable + " WHERE FlowStarter='" + WebUser.No + "' AND OID!=" + wk.OID + " ORDER BY OID DESC";
                string workidPri = DBAccess.RunSQLReturnStringIsNull(sql, "0");
                if (workidPri == "0")
                {
                    /*说明没有第一笔数据.*/
                }
                else
                {
                    PFlowNo = this.No;
                    PNodeIDStr = int.Parse(this.No) + "01";
                    PWorkIDStr = workidPri;
                    PFIDStr = "0";
                    CopyFormWorkID = workidPri;
                }
            }
            #endregion  判断是否装载上一条数据.

            #region 处理流程之间的数据传递1。
            if (DataType.IsNullOrEmpty(PNodeIDStr) == false && DataType.IsNullOrEmpty(PWorkIDStr) == false)
            {
                Int64 PWorkID = Int64.Parse(PWorkIDStr);
                Int64 PNodeID = 0;
                if (CopyFormWorkID != null)
                    PNodeID = Int64.Parse(PNodeIDStr);

                /* 如果是从另外的一个流程上传递过来的，就考虑另外的流程数据。*/

                #region copy 首先从父流程的NDxxxRpt copy.
                Int64 pWorkIDReal = 0;
                Flow pFlow = new Flow(PFlowNo);
                string pOID = "";
                if (DataType.IsNullOrEmpty(PFIDStr) == true || PFIDStr == "0")
                    pOID = PWorkID.ToString();
                else
                    pOID = PFIDStr;

                string sql = "SELECT * FROM " + pFlow.PTable + " WHERE OID=" + pOID;
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count != 1)
                    throw new Exception("@不应该查询不到父流程的数据[" + sql + "], 可能的情况之一,请确认该父流程的调用节点是子线程，但是没有把子线程的FID参数传递进来。");

                wk.Copy(dt.Rows[0]);
                rpt.Copy(dt.Rows[0]);

                //设置单号为空.
                wk.SetValByKey("BillNo", "");
                rpt.BillNo = "";
                #endregion copy 首先从父流程的NDxxxRpt copy.

                #region 从调用的节点上copy.
                BP.WF.Node fromNd = new BP.WF.Node(int.Parse(PNodeIDStr));
                Work wkFrom = fromNd.HisWork;
                wkFrom.OID = PWorkID;
                if (wkFrom.RetrieveFromDBSources() == 0)
                    throw new Exception("@父流程的工作ID不正确，没有查询到数据" + PWorkID);
                //wk.Copy(wkFrom);
                //rpt.Copy(wkFrom);
                #endregion 从调用的节点上copy.

                #region 获取web变量.
                foreach (string k in paras.Keys)
                {
                    if (k == "OID")
                        continue;

                    wk.SetValByKey(k, paras[k]);
                    rpt.SetValByKey(k, paras[k]);
                }
                #endregion 获取web变量.

                #region 特殊赋值.
                wk.OID = newOID;
                rpt.OID = newOID;

                // 在执行copy后，有可能这两个字段会被冲掉。
                if (CopyFormWorkID != null)
                {
                    /*如果不是 执行的从已经完成的流程copy.*/

                    wk.SetValByKey(GERptAttr.PFlowNo, PFlowNo);
                    wk.SetValByKey(GERptAttr.PNodeID, PNodeID);
                    wk.SetValByKey(GERptAttr.PWorkID, PWorkID);

                    rpt.SetValByKey(GERptAttr.PFlowNo, PFlowNo);
                    rpt.SetValByKey(GERptAttr.PNodeID, PNodeID);
                    rpt.SetValByKey(GERptAttr.PWorkID, PWorkID);

                    //忘记了增加这句话.
                    rpt.SetValByKey(GERptAttr.PEmp, WebUser.No);

                    //要处理单据编号 BillNo .
                    if (this.BillNoFormat != "")
                    {
                        rpt.SetValByKey(GERptAttr.BillNo, BP.WF.WorkFlowBuessRole.GenerBillNo(this.BillNoFormat, rpt.OID, rpt, this.PTable));

                        //设置单据编号.
                        wk.SetValByKey(GERptAttr.BillNo, rpt.BillNo);
                    }

                    rpt.SetValByKey(GERptAttr.FID, 0);
                    rpt.SetValByKey(GERptAttr.FlowStartRDT, DataType.CurrentDataTime);
                    rpt.SetValByKey(GERptAttr.FlowEnderRDT, DataType.CurrentDataTime);
                    rpt.SetValByKey(GERptAttr.WFState, (int)WFState.Blank);
                    rpt.SetValByKey(GERptAttr.FlowStarter, emp.No);
                    rpt.SetValByKey(GERptAttr.FlowEnder, emp.No);
                    rpt.SetValByKey(GERptAttr.FlowEndNode, this.StartNodeID);
                    rpt.SetValByKey(GERptAttr.FK_Dept, emp.FK_Dept);
                    rpt.SetValByKey(GERptAttr.FK_NY, DataType.CurrentYearMonth);

                    if (Glo.UserInfoShowModel == UserInfoShowModel.UserNameOnly)
                        rpt.SetValByKey(GERptAttr.FlowEmps, "@" + emp.Name + "@");

                    if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                        rpt.SetValByKey(GERptAttr.FlowEmps, "@" + emp.No + "@");

                    if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                        rpt.SetValByKey(GERptAttr.FlowEmps, "@" + emp.No + "," + emp.Name + "@");

                }

                if (rpt.EnMap.PhysicsTable != wk.EnMap.PhysicsTable)
                    wk.Update(); //更新工作节点数据.
                rpt.Update(); // 更新流程数据表.
                #endregion 特殊赋值.

                #region 复制其他数据..
                //复制明细。
                MapDtls dtls = wk.HisMapDtls;
                if (dtls.Count > 0)
                {
                    MapDtls dtlsFrom = wkFrom.HisMapDtls;
                    int idx = 0;
                    if (dtlsFrom.Count == dtls.Count)
                    {
                        foreach (MapDtl dtl in dtls)
                        {
                            if (dtl.IsCopyNDData == false)
                                continue;

                            //new 一个实例.
                            GEDtl dtlData = new GEDtl(dtl.No);

                            //删除以前的数据.
                            try
                            {
                                sql = "DELETE FROM " + dtlData.EnMap.PhysicsTable + " WHERE RefPK=" + wk.OID;
                                DBAccess.RunSQL(sql);
                            }
                            catch (Exception ex)
                            {

                            }



                            MapDtl dtlFrom = dtlsFrom[idx] as MapDtl;

                            GEDtls dtlsFromData = new GEDtls(dtlFrom.No);
                            dtlsFromData.Retrieve(GEDtlAttr.RefPK, PWorkID);
                            foreach (GEDtl geDtlFromData in dtlsFromData)
                            {
                                //是否启用多附件
                                FrmAttachmentDBs dbs = null;
                                if (dtl.IsEnableAthM == true)
                                {
                                    //根据从表的OID 获取附件信息
                                    dbs = new FrmAttachmentDBs();
                                    dbs.Retrieve(FrmAttachmentDBAttr.RefPKVal, geDtlFromData.OID);
                                }

                                dtlData.Copy(geDtlFromData);
                                dtlData.RefPK = wk.OID.ToString();
                                dtlData.FID = wk.OID;
                                if (this.No.Equals(PFlowNo) == false && (this.StartLimitRole == WF.StartLimitRole.OnlyOneSubFlow))
                                {
                                    dtlData.SaveAsOID(geDtlFromData.OID); //为子流程的时候，仅仅允许被调用1次.
                                }
                                else
                                {
                                    dtlData.InsertAsNew();
                                    if (dbs != null && dbs.Count != 0)
                                    {
                                        //复制附件信息
                                        FrmAttachmentDB newDB = new FrmAttachmentDB();
                                        foreach (FrmAttachmentDB db in dbs)
                                        {
                                            newDB.Copy(db);
                                            newDB.RefPKVal = dtlData.OID.ToString();
                                            newDB.FID = dtlData.OID;
                                            newDB.MyPK = DBAccess.GenerGUID();
                                            newDB.Insert();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //复制附件数据。
                if (wk.HisFrmAttachments.Count > 0)
                {
                    if (wkFrom.HisFrmAttachments.Count > 0)
                    {
                        int toNodeID = wk.NodeID;

                        //删除数据。
                        DBAccess.RunSQL("DELETE FROM Sys_FrmAttachmentDB WHERE FK_MapData='ND" + toNodeID + "' AND RefPKVal='" + wk.OID + "'");
                        FrmAttachmentDBs athDBs = new FrmAttachmentDBs("ND" + PNodeIDStr, PWorkID.ToString());

                        foreach (FrmAttachmentDB athDB in athDBs)
                        {
                            FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                            athDB_N.Copy(athDB);
                            athDB_N.FK_MapData = "ND" + toNodeID;
                            athDB_N.RefPKVal = wk.OID.ToString();
                            athDB_N.FK_FrmAttachment = athDB_N.FK_FrmAttachment.Replace("ND" + PNodeIDStr,
                              "ND" + toNodeID);

                            if (athDB_N.HisAttachmentUploadType == AttachmentUploadType.Single)
                            {
                                /*如果是单附件.*/
                                athDB_N.MyPK = athDB_N.FK_FrmAttachment + "_" + wk.OID;
                                if (athDB_N.IsExits == true)
                                    continue; /*说明上一个节点或者子线程已经copy过了, 但是还有子线程向合流点传递数据的可能，所以不能用break.*/
                                athDB_N.Insert();
                            }
                            else
                            {
                                athDB_N.MyPK = athDB_N.UploadGUID + "_" + athDB_N.FK_MapData + "_" + wk.OID;
                                athDB_N.Insert();
                            }
                        }
                    }
                }
                #endregion 复制表单其他数据.

                #region 复制独立表单数据.
                //求出来被copy的节点有多少个独立表单.
                FrmNodes fnsFrom = new Template.FrmNodes(fromNd.NodeID);
                if (fnsFrom.Count != 0)
                {
                    //求当前节点表单的绑定的表单.
                    FrmNodes fns = new Template.FrmNodes(nd.NodeID);
                    if (fns.Count != 0)
                    {
                        //开始遍历当前绑定的表单.
                        foreach (FrmNode fn in fns)
                        {
                            foreach (FrmNode fnFrom in fnsFrom)
                            {
                                if (fn.FK_Frm != fnFrom.FK_Frm)
                                    continue;

                                BP.Sys.GEEntity geEnFrom = new GEEntity(fnFrom.FK_Frm);
                                geEnFrom.OID = PWorkID;
                                if (geEnFrom.RetrieveFromDBSources() == 0)
                                    continue;

                                //执行数据copy , 复制到本身. 
                                geEnFrom.CopyToOID(wk.OID);
                            }
                        }
                    }
                }
                #endregion 复制独立表单数据.

            }


            #endregion 处理流程之间的数据传递1。

            #region 处理单据编号.
            //生成单据编号.
            if (this.BillNoFormat.Length > 3)
            {
                string billNoFormat = this.BillNoFormat.Clone() as string;
                string billNo = rpt.BillNo;
                if (DataType.IsNullOrEmpty(billNo) == true)
                {
                    //生成单据编号.
                    rpt.BillNo = BP.WF.WorkFlowBuessRole.GenerBillNo(billNoFormat, rpt.OID, rpt, this.PTable);
                    if (wk.Row.ContainsKey(GERptAttr.BillNo) == true)
                    {
                        wk.SetValByKey(NDXRptBaseAttr.BillNo, rpt.BillNo);
                    }
                    rpt.Update();
                }
            }
            #endregion 处理单据编号.

            #region 处理流程之间的数据传递2, 如果是直接要跳转到指定的节点上去.
            if (paras.ContainsKey("JumpToNode") == true)
            {
                wk.Rec = WebUser.No;

                //wk.SetValByKey("FK_NY", DataType.CurrentYearMonth);
                //wk.FK_Dept = emp.FK_Dept;
                //wk.SetValByKey("FK_DeptName", emp.FK_DeptText);
                //wk.SetValByKey("FK_DeptText", emp.FK_DeptText);
                wk.FID = 0;
                // wk.SetValByKey(GERptAttr.RecText, emp.Name);

                int jumpNodeID = int.Parse(paras["JumpToNode"].ToString());
                Node jumpNode = new Node(jumpNodeID);

                string jumpToEmp = paras["JumpToEmp"].ToString();
                if (DataType.IsNullOrEmpty(jumpToEmp))
                    jumpToEmp = emp.No;

                WorkNode wn = new WorkNode(wk, nd);
                wn.NodeSend(jumpNode, jumpToEmp);

                WorkFlow wf = new WorkFlow(this, wk.OID, wk.FID);

                BP.WF.GenerWorkFlow gwf = new GenerWorkFlow(rpt.OID);
                rpt.WFState = WFState.Runing;
                rpt.Update();

                return wf.GetCurrentWorkNode().HisWork;
            }
            #endregion 处理流程之间的数据传递。

            #region 最后整理wk数据.
            wk.Rec = emp.No;
            //  wk.SetValByKey(WorkAttr.RDT, DataType.CurrentDataTime);
            // wk.SetValByKey(WorkAttr.CDT, DataType.CurrentDataTime);
            wk.SetValByKey("FK_NY", DataType.CurrentYearMonth);
            wk.SetValByKey("FK_Dept", emp.FK_Dept);
            wk.SetValByKey("FK_DeptName", emp.FK_DeptText);
            wk.SetValByKey("FK_DeptText", emp.FK_DeptText);

            if (rpt.EnMap.Attrs.Contains("BillNo") == true)
                wk.SetValByKey(NDXRptBaseAttr.BillNo, rpt.BillNo);

            wk.FID = 0;
            if (wk.IsExits == false)
                wk.DirectInsert();
            else
                wk.Update();
            #endregion 最后整理参数.

            #region 给generworkflow初始化数据. add 2015-08-06
            GenerWorkFlow mygwf = new GenerWorkFlow();
            mygwf.WorkID = wk.OID;
            if (mygwf.RetrieveFromDBSources() == 0)
            {
                mygwf.FK_Flow = this.No;
                mygwf.FK_FlowSort = this.FK_FlowSort;
                mygwf.SysType = this.SysType;
                mygwf.FK_Node = nd.NodeID;
                mygwf.WorkID = wk.OID;
                mygwf.WFState = WFState.Blank;
                mygwf.FlowName = this.Name;
                mygwf.Insert();
            }
            mygwf.Starter = WebUser.No;
            mygwf.StarterName = WebUser.Name;
            mygwf.FK_Dept = BP.Web.WebUser.FK_Dept;
            mygwf.DeptName = BP.Web.WebUser.FK_DeptName;
            mygwf.RDT = DataType.CurrentDataTime;
            mygwf.Title = rpt.Title;
            mygwf.BillNo = rpt.BillNo;
            if (mygwf.Title.Contains("@") == true)
                mygwf.Title = BP.WF.WorkFlowBuessRole.GenerTitle(this, rpt);
            if (DataType.IsNullOrEmpty(PNodeIDStr) == false && DataType.IsNullOrEmpty(PWorkIDStr) == false)
            {
                if (DataType.IsNullOrEmpty(PFIDStr) == false)
                    mygwf.PFID = Int64.Parse(PFIDStr);
                mygwf.PEmp = rpt.PEmp;
                mygwf.PFlowNo = rpt.PFlowNo;
                mygwf.PNodeID = rpt.PNodeID;
                mygwf.PWorkID = rpt.PWorkID;
            }
            mygwf.DirectUpdate();
            #endregion 给 generworkflow 初始化数据.

            //更新domian.
            DBAccess.RunSQL("UPDATE wf_generworkflow  SET domain=(SELECT domain FROM wf_flowsort WHERE wf_flowsort.NO=wf_generworkflow.FK_FlowSort) where workid=" + wk.OID);


            return wk;
        }
        #endregion 创建新工作.

        #region 其他通用方法.
        public string DoBTableDTS()
        {
            if (this.DTSWay == DataDTSWay.None)
                return "执行失败，您没有设置同步方式。";

            string info = "";
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.FK_Flow, this.No);
            foreach (GenerWorkFlow gwf in gwfs)
            {
                GERpt rpt = this.HisGERpt;
                rpt.OID = gwf.WorkID;
                rpt.RetrieveFromDBSources();

                info += "@开始同步:" + gwf.Title + ",WorkID=" + gwf.WorkID;
                if (gwf.WFSta == WFSta.Complete)
                    WorkNodePlus.DTSData(this, gwf, rpt, new Node(gwf.FK_Node), true);
                else
                    WorkNodePlus.DTSData(this, gwf, rpt, new Node(gwf.FK_Node), false);
                info += "同步成功.";
            }
            return info;
        }
        /// <summary>
        /// 自动发起
        /// </summary>
        /// <returns></returns>
        public string DoAutoStartIt()
        {
            switch (this.HisFlowRunWay)
            {
                case BP.WF.FlowRunWay.SpecEmp: //指定人员按时运行。
                    string RunObj = this.RunObj;
                    string FK_Emp = RunObj.Substring(0, RunObj.IndexOf('@'));
                    BP.Port.Emp emp = new BP.Port.Emp();
                    emp.No = FK_Emp;
                    if (emp.RetrieveFromDBSources() == 0)
                        return "启动自动启动流程错误：发起人(" + FK_Emp + ")不存在。";

                    BP.Web.WebUser.SignInOfGener(emp);
                    string info_send = BP.WF.Dev2Interface.Node_StartWork(this.No, null, null, 0, null, 0, null).ToMsgOfText();
                    if (WebUser.No != "admin")
                    {
                        emp = new BP.Port.Emp();
                        emp.No = "admin";
                        emp.Retrieve();
                        BP.Web.WebUser.SignInOfGener(emp);
                        return info_send;
                    }
                    return info_send;
                case BP.WF.FlowRunWay.SelectSQLModel: //按数据集合驱动的模式执行。
                    break;
                default:
                    return "@该流程您没有设置为自动启动的流程类型。";
            }

            string msg = "";
            BP.Sys.MapExt me = new MapExt();
            me.MyPK = "ND" + int.Parse(this.No) + "01_" + MapExtXmlList.StartFlow;
            int i = me.RetrieveFromDBSources();
            if (i == 0)
            {
                Log.DefaultLogWriteLineError("没有为流程(" + this.Name + ")的开始节点设置发起数据,请参考说明书解决.");
                return "没有为流程(" + this.Name + ")的开始节点设置发起数据,请参考说明书解决.";
            }
            if (DataType.IsNullOrEmpty(me.Tag))
            {
                Log.DefaultLogWriteLineError("没有为流程(" + this.Name + ")的开始节点设置发起数据,请参考说明书解决.");
                return "没有为流程(" + this.Name + ")的开始节点设置发起数据,请参考说明书解决.";
            }

            // 获取从表数据.
            DataSet ds = new DataSet();
            string[] dtlSQLs = me.Tag1.Split('*');
            foreach (string sql in dtlSQLs)
            {
                if (DataType.IsNullOrEmpty(sql))
                    continue;

                string[] tempStrs = sql.Split('=');
                string dtlName = tempStrs[0];
                DataTable dtlTable = DBAccess.RunSQLReturnTable(sql.Replace(dtlName + "=", ""));
                dtlTable.TableName = dtlName;
                ds.Tables.Add(dtlTable);
            }

            #region 检查数据源是否正确.
            string errMsg = "";
            // 获取主表数据.
            DataTable dtMain = DBAccess.RunSQLReturnTable(me.Tag);
            if (dtMain.Rows.Count == 0)
            {
                return "流程(" + this.Name + ")此时无任务,查询语句:" + me.Tag.Replace("'", "”");
            }

            msg += "@查询到(" + dtMain.Rows.Count + ")条任务.";

            if (dtMain.Columns.Contains("Starter") == false)
                errMsg += "@配值的主表中没有Starter列.";

            if (dtMain.Columns.Contains("MainPK") == false)
                errMsg += "@配值的主表中没有MainPK列.";

            if (errMsg.Length > 2)
            {
                return "流程(" + this.Name + ")的开始节点设置发起数据,不完整." + errMsg;
            }
            #endregion 检查数据源是否正确.

            #region 处理流程发起.

            string fk_mapdata = "ND" + int.Parse(this.No) + "01";

            MapData md = new MapData(fk_mapdata);
            int idx = 0;
            foreach (DataRow dr in dtMain.Rows)
            {
                idx++;

                string mainPK = dr["MainPK"].ToString();
                string sql = "SELECT OID FROM " + md.PTable + " WHERE MainPK='" + mainPK + "'";
                if (DBAccess.RunSQLReturnTable(sql).Rows.Count != 0)
                {
                    msg += "@" + this.Name + ",第" + idx + "条,此任务在之前已经完成。";
                    continue; /*说明已经调度过了*/
                }

                string starter = dr["Starter"].ToString();
                if (WebUser.No != starter)
                {
                    BP.Web.WebUser.Exit();
                    BP.Port.Emp emp = new BP.Port.Emp();
                    emp.No = starter;
                    if (emp.RetrieveFromDBSources() == 0)
                    {
                        msg += "@" + this.Name + ",第" + idx + "条,设置的发起人员:" + emp.No + "不存在.";
                        msg += "@数据驱动方式发起流程(" + this.Name + ")设置的发起人员:" + emp.No + "不存在。";
                        continue;
                    }
                    WebUser.SignInOfGener(emp);
                }

                #region  给值.
                Work wk = this.NewWork();
                foreach (DataColumn dc in dtMain.Columns)
                    wk.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());

                if (ds.Tables.Count != 0)
                {
                    // MapData md = new MapData(nodeTable);
                    MapDtls dtls = md.MapDtls; // new MapDtls(nodeTable);
                    foreach (MapDtl dtl in dtls)
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName != dtl.No)
                                continue;

                            //删除原来的数据。
                            GEDtl dtlEn = dtl.HisGEDtl;
                            dtlEn.Delete(GEDtlAttr.RefPK, wk.OID.ToString());

                            // 执行数据插入。
                            foreach (DataRow drDtl in dt.Rows)
                            {
                                if (drDtl["RefMainPK"].ToString() != mainPK)
                                    continue;

                                dtlEn = dtl.HisGEDtl;
                                foreach (DataColumn dc in dt.Columns)
                                    dtlEn.SetValByKey(dc.ColumnName, drDtl[dc.ColumnName].ToString());

                                dtlEn.RefPK = wk.OID.ToString();
                                dtlEn.OID = 0;
                                dtlEn.Insert();
                            }
                        }
                    }
                }
                #endregion  给值.

                // 处理发送信息.
                Node nd = this.HisStartNode;
                try
                {
                    WorkNode wn = new WorkNode(wk, nd);
                    string infoSend = wn.NodeSend().ToMsgOfHtml();
                    Log.DefaultLogWriteLineInfo(msg);
                    msg += "@" + this.Name + ",第" + idx + "条,发起人员:" + WebUser.No + "-" + WebUser.Name + "已完成.\r\n" + infoSend;
                    //this.SetText("@第（" + idx + "）条任务，" + WebUser.No + " - " + WebUser.Name + "已经完成。\r\n" + msg);
                }
                catch (Exception ex)
                {
                    msg += "@" + this.Name + ",第" + idx + "条,发起人员:" + WebUser.No + "-" + WebUser.Name + "发起时出现错误.\r\n" + ex.Message;
                }
                msg += "<hr>";
            }
            return msg;
            #endregion 处理流程发起.
        }

        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No.Equals("admin") == true)
                    uac.IsUpdate = true;
                return uac;
            }
        }

        public string ClearCash()
        {
            Cash.ClearCash();
            //清空流程中的缓存
            //获取改流程中的节点数据
            Nodes nds = new Nodes(this.No);
            foreach (Node nd in nds)
            {
                //判断表单的类型
                if (nd.HisFormType == NodeFormType.FoolForm || nd.HisFormType == NodeFormType.FreeForm)
                    BP.Sys.CCFormAPI.AfterFrmEditAction("ND" + nd.NodeID);
            }

            return "清除成功.";
        }


        /// <summary>
        /// 执行检查2018
        /// </summary>
        /// <returns></returns>
        public string DoCheck2018()
        {
            FlowCheckError check = new FlowCheckError(this);
            check.DoCheck();

            return BP.Tools.Json.ToJson(check.dt);
            //return "../../Admin/Testing/FlowCheckError.htm?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 校验流程
        /// </summary>
        /// <returns></returns>
        public string DoCheck()
        {
            Cash.ClearCash();

            //删除缓存数据.
            this.ClearAutoNumCash(false);

            Nodes nds = new Nodes(this.No);

            #region 检查独立表单
            FrmNodes fns = new FrmNodes();
            fns.Retrieve(FrmNodeAttr.FK_Flow, this.No);
            string frms = "";
            string err = "";
            MapData md = new MapData();
            foreach (FrmNode item in fns)
            {
                if (frms.Contains(item.FK_Frm + ","))
                    continue;
                frms += item.FK_Frm + ",";
                md.No = item.FK_Frm;
                if (md.IsExits == false)
                    err += "@节点" + item.FK_Node + "绑定的表单:" + item.FK_Frm + ",已经被删除了.";
            }
            #endregion

            try
            {
                #region 对流程的设置做必要的检查.
                // 设置流程名称.
                DBAccess.RunSQL("UPDATE WF_Node SET FlowName = (SELECT Name FROM WF_Flow WHERE NO=WF_Node.FK_Flow) WHERE FK_Flow='" + this.No + "'");

                //设置单据编号只读格式.
                DBAccess.RunSQL("UPDATE Sys_MapAttr SET UIIsEnable=0 WHERE KeyOfEn='BillNo' AND UIIsEnable=1 ");

                //开始节点不能有会签.
                DBAccess.RunSQL("UPDATE WF_Node SET HuiQianRole=0 WHERE NodePosType=0 AND HuiQianRole !=0  AND FK_Flow='" + this.No + "'");

                //开始节点不能有退回.
                // DBAccess.RunSQL("UPDATE WF_Node SET ReturnRole=0 WHERE NodePosType=0 AND ReturnRole !=0");
                #endregion 对流程的设置做必要的检查.

                //删除垃圾,非法数据.
                string sqls = "DELETE FROM Sys_FrmSln WHERE FK_MapData NOT IN (SELECT No from Sys_MapData)";
                sqls = "";
                sqls += "@ DELETE FROM WF_Direction WHERE Node=ToNode";
                DBAccess.RunSQLs(sqls);

                //更新计算数据.
                //this.NumOfBill = DBAccess.RunSQLReturnValInt("SELECT count(*) FROM WF_BillTemplate WHERE NodeID IN (SELECT NodeID FROM WF_Flow WHERE No='" + this.No + "')");
                //this.NumOfDtl = DBAccess.RunSQLReturnValInt("SELECT count(*) FROM Sys_MapDtl WHERE FK_MapData='ND" + int.Parse(this.No) + "Rpt'");
                //this.DirectUpdate();

                string msg = "@  =======  关于《" + this.Name + " 》流程检查报告  ============";
                msg += "@信息输出分为三种: 信息  警告  错误. 如果遇到输出的错误，则必须要去修改或者设置.";
                msg += "@流程检查目前还不能覆盖100%的错误,需要手工的运行一次才能确保流程设计的正确性.";

                ////条件集合.
                //Conds conds = new Conds(this.No);

                #region 对节点进行检查
                //节点表单字段数据类型检查--begin---------
                msg += CheckFormFields(nds);
                //表单字段数据类型检查-------End-----

                //获得字段用于校验sql.
                MapAttrs mattrs = new MapAttrs("ND" + int.Parse(this.No) + "Rpt");

                //查询所有的条件.
                Conds conds = new Conds();
                conds.Retrieve(CondAttr.FK_Flow, this.No);


                //删除垃圾数据.
                string sql = "DELETE FROM WF_Direction  WHERE Node NOT IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "') AND FK_Flow='" + this.No + "' ";
                DBAccess.RunSQL(sql);
                sql = "DELETE FROM WF_Direction  WHERE ToNode NOT IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "') AND FK_Flow='" + this.No + "' ";
                DBAccess.RunSQL(sql);

                foreach (Node nd in nds)
                {
                    //设置它的位置类型.
                    // nd.RetrieveFromDBSources();
                    nd.SetValByKey(NodeAttr.NodePosType, (int)nd.GetHisNodePosType());

                    msg += "@信息: -------- 开始检查节点ID:(" + nd.NodeID + ")名称:(" + nd.Name + ")信息 -------------";

                    #region 对节点的访问规则进行检查
                    msg += "@信息:开始对节点的访问规则进行检查.";

                    switch (nd.HisDeliveryWay)
                    {
                        case DeliveryWay.ByStation:
                        case DeliveryWay.FindSpecDeptEmpsInStationlist:
                            if (nd.NodeStations.Count == 0)
                                msg += "@错误:您设置了该节点的访问规则是按岗位，但是您没有为节点绑定岗位。";
                            break;
                        case DeliveryWay.ByDept:
                            if (nd.NodeDepts.Count == 0)
                                msg += "@错误:您设置了该节点的访问规则是按部门，但是您没有为节点绑定部门。";
                            break;
                        case DeliveryWay.ByBindEmp:
                            if (nd.NodeEmps.Count == 0)
                                msg += "@错误:您设置了该节点的访问规则是按人员，但是您没有为节点绑定人员。";
                            break;
                        case DeliveryWay.BySpecNodeEmp: /*按指定的岗位计算.*/
                        case DeliveryWay.BySpecNodeEmpStation: /*按指定的岗位计算.*/
                            if (nd.DeliveryParas.Trim().Length == 0)
                                msg += "@错误:您设置了该节点的访问规则是按指定的岗位计算，但是您没有设置节点编号.";
                            break;
                        case DeliveryWay.ByDeptAndStation: /*按部门与岗位的交集计算.*/
                            string mysql = string.Empty;
                            //added by liuxc,2015.6.30.

                            mysql = "SELECT pdes.FK_Emp AS No"
                                    + " FROM   Port_DeptEmpStation pdes"
                                    + "        INNER JOIN WF_NodeDept wnd"
                                    + "             ON  wnd.FK_Dept = pdes.FK_Dept"
                                    + "             AND wnd.FK_Node = " + nd.NodeID
                                    + "        INNER JOIN WF_NodeStation wns"
                                    + "             ON  wns.FK_Station = pdes.FK_Station"
                                    + "             AND wnd.FK_Node =" + nd.NodeID
                                    + " ORDER BY"
                                    + "        pdes.FK_Emp";


                            DataTable mydt = DBAccess.RunSQLReturnTable(mysql);
                            if (mydt.Rows.Count == 0)
                                msg += "@错误:按照岗位与部门的交集计算错误，没有人员集合{" + mysql + "}";
                            break;
                        case DeliveryWay.BySQL:
                        case DeliveryWay.BySQLAsSubThreadEmpsAndData:
                            if (nd.DeliveryParas.Trim().Length == 0)
                            {
                                msg += "@错误:您设置了该节点的访问规则是按SQL查询，但是您没有在节点属性里设置查询sql，此sql的要求是查询必须包含No,Name两个列，sql表达式里支持@+字段变量，详细参考开发手册.";
                            }
                            else
                            {
                                try
                                {
                                    sql = nd.DeliveryParas;
                                    sql = Glo.DealExp(sql, this.HisGERpt, null);

                                    sql = sql.Replace("''''", "''"); //出现双引号的问题.
                                    if (sql.Contains("@"))
                                        throw new Exception("您编写的sql变量填写不正确，实际执行中，没有被完全替换下来" + sql);

                                    DataTable testDB = null;
                                    try
                                    {
                                        testDB = DBAccess.RunSQLReturnTable(sql);
                                    }
                                    catch (Exception ex)
                                    {
                                        msg += "@错误:您设置了该节点的访问规则是按SQL查询,执行此语句错误." + ex.Message;
                                    }

                                    if (testDB.Columns.Contains("no") == false
                                        || testDB.Columns.Contains("name") == false)
                                    {
                                        msg += "@错误:您设置了该节点的访问规则是按SQL查询，设置的sql不符合规则，此sql的要求是查询必须包含No,Name两个列，sql表达式里支持@+字段变量，详细参考开发手册.";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message;
                                }
                            }
                            break;
                        case DeliveryWay.ByPreviousNodeFormEmpsField:
                            //去rpt表中，查询是否有这个字段.
                            string str1 = nd.NodeID.ToString().Substring(0, nd.NodeID.ToString().Length - 2);
                            MapAttrs rptAttrs = new BP.Sys.MapAttrs();
                            rptAttrs.Retrieve(MapAttrAttr.FK_MapData, "ND" + str1 + "Rpt", MapAttrAttr.KeyOfEn);

                            if (rptAttrs.Contains(BP.Sys.MapAttrAttr.KeyOfEn, nd.DeliveryParas) == false)
                            {
                                /*检查节点字段是否有FK_Emp字段*/
                                msg += "@错误:您设置了该节点的访问规则是[06.按上一节点表单指定的字段值作为本步骤的接受人]，但是您没有在节点属性的[访问规则设置内容]里设置指定的表单字段，详细参考开发手册.";
                            }
                            //if (mattrs.Contains(BP.Sys.MapAttrAttr.KeyOfEn, "FK_Emp") == false)
                            //{
                            //    /*检查节点字段是否有FK_Emp字段*/
                            //    msg += "@错误:您设置了该节点的访问规则是按指定节点表单人员，但是您没有在节点表单中增加FK_Emp字段，详细参考开发手册 .";
                            //}
                            break;
                        case DeliveryWay.BySelected: /* 由上一步发送人员选择 */
                            if (nd.IsStartNode)
                            {
                                //msg += "@错误:开始节点不能设置指定的选择人员访问规则。";
                                break;
                            }
                            break;
                        case DeliveryWay.ByPreviousNodeEmp: /* 由上一步发送人员选择 */
                            if (nd.IsStartNode)
                            {
                                msg += "@错误:节点访问规则设置错误:开始节点，不允许设置与上一节点的工作人员相同.";
                                break;
                            }
                            break;
                        default:
                            break;
                    }
                    msg += "@对节点的访问规则进行检查完成....";
                    #endregion

                    #region 检查节点完成条件，方向条件的定义.
                    if (conds.Count != 0)
                    {
                        msg += "@信息:开始检查(" + nd.Name + ")方向条件:";
                        foreach (Cond cond in conds)
                        {
                            msg += cond.AttrKey + cond.AttrName + cond.OperatorValue + "、";
                        }
                        msg += "@(" + nd.Name + ")方向条件检查完成.....";
                    }
                    #endregion 检查节点完成条件的定义.

                    #region 如果是引用的表单库的表单，就要检查该表单是否有FID字段，没有就自动增加.
                    if (nd.HisFormType == NodeFormType.RefOneFrmTree)
                    {
                        MapAttr mattr = new MapAttr();
                        mattr.MyPK = nd.NodeFrmID + "_FID";
                        if (mattr.RetrieveFromDBSources() == 0)
                        {
                            mattr.KeyOfEn = "FID";
                            mattr.FK_MapData = nd.NodeFrmID;
                            mattr.MyDataType = DataType.AppInt;
                            mattr.UIVisible = false;
                            mattr.Name = "FID(自动增加)";
                            mattr.Insert();

                            GEEntity en = new GEEntity(nd.NodeFrmID);
                            en.CheckPhysicsTable();
                        }
                    }
                    #endregion 如果是引用的表单库的表单，就要检查该表单是否有FID字段，没有就自动增加.

                    //如果是子线城，子线程的表单必须是轨迹模式。
                    if (nd.HisRunModel == RunModel.SubThread)
                    {
                        md = new MapData("ND" + nd.NodeID);
                        if (md.PTable != "ND" + nd.NodeID)
                        {
                            md.PTable = "ND" + nd.NodeID;
                            md.Update();
                        }
                    }
                }
                #endregion

                #region 检查延续流程,子流程发起。
                SubFlowYanXus ygflows = new SubFlowYanXus();
                ygflows.Retrieve(SubFlowYanXuAttr.SubFlowNo, this.No, SubFlowYanXuAttr.SubFlowType,
                    (int)SubFlowType.YanXuFlow);
                foreach (SubFlowYanXu flow in ygflows)
                {
                    Flow fl = new Flow(flow.SubFlowNo);

                    /* 如果当前为子流程的时候，允许节点自动运行下一步骤，就要确定下一步骤的节点，必须有确定的可以计算的接收人. */
                    if (fl.SubFlowOver == SubFlowOver.SendParentFlowToNextStep)
                    {
                        Node nd = new Node(flow.FK_Node);
                        if (nd.HisToNodes.Count > 1)
                        {
                            msg += "@当前节点[" + nd.Name + "]的可以启动子流程或者延续流程.被启动的子流程设置了当子流程结束时让父流程自动运行到下一个节点，但是当前节点有分支，导致流程无法运行到下一个节点.";
                        }

                        if (nd.HisToNodes.Count == 1)
                        {
                            Node toNode = nd.HisToNodes[0] as Node;
                            if (nd.HisDeliveryWay == DeliveryWay.BySelected)
                                msg += "@当前节点[" + nd.Name + "]的可以启动子流程或者延续流程.被启动的子流程设置了当子流程结束时让父流程自动运行到下一个节点，但是当前节点有分支，导致流程无法运行到下一个节点.";
                        }

                    }
                }
                #endregion 检查越轨流程,子流程发起。


                msg += "@流程的基础信息: ------ ";
                msg += "@编号:  " + this.No + " 名称:" + this.Name + " , 存储表:" + this.PTable;

                msg += "@信息:开始检查节点流程报表.";
                this.DoCheck_CheckRpt(this.HisNodes);

                #region 检查焦点字段设置是否还有效 
                msg += "@信息:开始检查节点的焦点字段";

                //获得gerpt字段.
                GERpt rpt = this.HisGERpt;
                foreach (Attr attr in rpt.EnMap.Attrs)
                {
                    rpt.SetValByKey(attr.Key, "0");
                }

                //处理焦点字段.
                foreach (Node nd in nds)
                {
                    if (nd.FocusField.Trim() == "")
                    {
                        Work wk = nd.HisWork;
                        string attrKey = "";
                        foreach (Attr attr in wk.EnMap.Attrs)
                        {
                            if (attr.UIVisible == true && attr.UIIsDoc && attr.UIIsReadonly == false
                                && (attr.Key.Contains("Check") || attr.Key.Contains("Note")))
                                attrKey = attr.Desc + ":@" + attr.Key;
                        }
                        if (attrKey == "")
                            msg += "@警告:节点ID:" + nd.NodeID + " 名称:" + nd.Name + "属性里没有设置焦点字段，会导致信息写入轨迹表空白，为了能够保证流程轨迹是可读的请设置焦点字段.";
                        else
                        {
                            msg += "@信息:节点ID:" + nd.NodeID + " 名称:" + nd.Name + "属性里没有设置焦点字段，会导致信息写入轨迹表空白，为了能够保证流程轨迹是可读的系统自动设置了焦点字段为" + attrKey + ".";
                            nd.FocusField = attrKey;
                            nd.DirectUpdate();
                        }
                        continue;
                    }

                    string strs = nd.FocusField.Clone() as string;
                    strs = Glo.DealExp(strs, rpt, "err");
                    if (strs.Contains("@") == true)
                    {
                        // msg += "@错误:焦点字段（" + nd.FocusField + "）在节点(step:" + nd.Step + " 名称:" + nd.Name + ")属性里的设置已无效，表单里不存在该字段.";
                        //删除节点属性中的焦点字段
                        nd.FocusField = "";
                        nd.Update();
                    }
                    else
                    {
                        msg += "@提示:节点的(" + nd.NodeID + "," + nd.Name + ")焦点字段（" + nd.FocusField + "）设置完整检查通过.";
                    }

                    if (this.IsMD5)
                    {
                        if (nd.HisWork.EnMap.Attrs.Contains(WorkAttr.MD5) == false)
                            nd.RepareMap(this);
                    }
                }
                msg += "@信息:检查节点的焦点字段完成.";
                #endregion

                #region 检查质量考核点.
                msg += "@信息:开始检查质量考核点";
                foreach (Node nd in nds)
                {
                    if (nd.IsEval)
                    {
                        /*如果是质量考核点，检查节点表单是否具别质量考核的特别字段？*/
                        sql = "SELECT COUNT(*) FROM Sys_MapAttr WHERE FK_MapData='ND" + nd.NodeID + "' AND KeyOfEn IN ('EvalEmpNo','EvalEmpName','EvalEmpCent')";
                        if (DBAccess.RunSQLReturnValInt(sql) != 3)
                            msg += "@信息:您设置了节点(" + nd.NodeID + "," + nd.Name + ")为质量考核节点，但是您没有在该节点表单中设置必要的节点考核字段.";
                    }
                }
                msg += "@检查质量考核点完成.";
                #endregion

                #region 检查如果是合流节点必须不能是由上一个节点指定接受人员.
                foreach (Node nd in nds)
                {
                    //如果是合流节点.
                    if (nd.HisNodeWorkType == NodeWorkType.WorkHL || nd.HisNodeWorkType == NodeWorkType.WorkFHL)
                    {
                        if (nd.HisDeliveryWay == DeliveryWay.BySelected)
                        {
                            msg += "@错误:节点ID:" + nd.NodeID + " 名称:" + nd.Name + "是合流或者分合流节点，但是该节点设置的接收人规则为由上一步指定，这是错误的，应该为自动计算而非每个子线程人为的选择.";
                        }
                    }
                }
                #endregion 检查如果是合流节点必须不能是由上一个节点指定接受人员。

                msg += "@流程报表检查完成...";

                // 检查流程， 处理计算字段.
                Node.CheckFlow(nds, this.No);

                //创建track.
                Track.CreateOrRepairTrackTable(this.No);
                return msg;
            }
            catch (Exception ex)
            {
                throw new Exception("@检查流程错误:" + ex.Message + " @" + ex.StackTrace);
            }
        }
        /// <summary>
        /// 节点表单字段数据类型检查，名字相同的字段出现类型不同的处理方法：
        /// 依照不同于NDxxRpt表中同名字段类型为基准
        /// </summary>
        /// <returns>检查结果</returns>
        private string CheckFormFields(Nodes nds)
        {
            StringBuilder errorAppend = new StringBuilder();
            errorAppend.Append("@信息: -------- 流程节点表单的字段类型检查: ------ ");
            try
            {
                string fk_mapdatas = "'ND" + int.Parse(this.No) + "Rpt'";
                foreach (Node nd in nds)
                {
                    fk_mapdatas += ",'ND" + nd.NodeID + "'";
                }

                //筛选出类型不同的字段.
                string checkSQL = "SELECT   AA.KEYOFEN, COUNT(*) AS MYNUM FROM ("
                                    + "  SELECT A.KEYOFEN,  MYDATATYPE,  COUNT(*) AS MYNUM "
                                    + "  FROM SYS_MAPATTR A WHERE FK_MAPDATA IN (" + fk_mapdatas + ") GROUP BY KEYOFEN, MYDATATYPE"
                                    + ")  AA GROUP BY  AA.KEYOFEN HAVING COUNT(*) > 1";
                DataTable dt_Fields = DBAccess.RunSQLReturnTable(checkSQL);
                foreach (DataRow row in dt_Fields.Rows)
                {
                    string keyOfEn = row["KEYOFEN"].ToString();
                    string myNum = row["MYNUM"].ToString();
                    int iMyNum = 0;
                    int.TryParse(myNum, out iMyNum);

                    //存在2种以上数据类型，有手动进行调整
                    if (iMyNum > 2)
                    {
                        errorAppend.Append("@错误：字段名" + keyOfEn + "在此流程表(" + fk_mapdatas + ")中存在2种以上数据类型(如：int，float,varchar,datetime)，请手动修改。");
                        return errorAppend.ToString();
                    }

                    //存在2种数据类型，以不同于NDxxRpt字段类型为主
                    MapAttr baseMapAttr = new MapAttr();
                    MapAttr rptMapAttr = new MapAttr("ND" + int.Parse(this.No) + "Rpt", keyOfEn);

                    //Rpt表中不存在此字段
                    if (rptMapAttr == null || rptMapAttr.MyPK == "")
                    {
                        this.DoCheck_CheckRpt(this.HisNodes);
                        rptMapAttr = new MapAttr("ND" + int.Parse(this.No) + "Rpt", keyOfEn);
                        this.HisGERpt.CheckPhysicsTable();
                    }

                    //Rpt表中不存在此字段,直接结束
                    if (rptMapAttr == null || rptMapAttr.MyPK == "")
                        continue;

                    foreach (Node nd in nds)
                    {
                        MapAttr ndMapAttr = new MapAttr("ND" + nd.NodeID, keyOfEn);
                        if (ndMapAttr == null || ndMapAttr.MyPK == "")
                            continue;

                        //找出与NDxxRpt表中字段数据类型不同的表单
                        if (rptMapAttr.MyDataType != ndMapAttr.MyDataType)
                        {
                            baseMapAttr = ndMapAttr;
                            break;
                        }
                    }
                    errorAppend.Append("@基础表" + baseMapAttr.FK_MapData + "，字段" + keyOfEn + "数据类型为：" + baseMapAttr.MyDataTypeStr);
                    //根据基础属性类修改数据类型不同的表单
                    foreach (Node nd in nds)
                    {
                        MapAttr ndMapAttr = new MapAttr("ND" + nd.NodeID, keyOfEn);
                        //不包含此字段的进行返回,类型相同的进行返回
                        if (ndMapAttr == null || ndMapAttr.MyPK == "" || baseMapAttr.MyPK == ndMapAttr.MyPK || baseMapAttr.MyDataType == ndMapAttr.MyDataType)
                            continue;

                        ndMapAttr.Name = baseMapAttr.Name;
                        ndMapAttr.MyDataType = baseMapAttr.MyDataType;
                        ndMapAttr.UIWidth = baseMapAttr.UIWidth;
                        ndMapAttr.UIHeight = baseMapAttr.UIHeight;
                        ndMapAttr.MinLen = baseMapAttr.MinLen;
                        ndMapAttr.MaxLen = baseMapAttr.MaxLen;
                        if (ndMapAttr.Update() > 0)
                            errorAppend.Append("@修改了" + "ND" + nd.NodeID + " 表，字段" + keyOfEn + "修改为：" + baseMapAttr.MyDataTypeStr);
                        else
                            errorAppend.Append("@错误:修改" + "ND" + nd.NodeID + " 表，字段" + keyOfEn + "修改为：" + baseMapAttr.MyDataTypeStr + "失败。");
                    }
                    //修改NDxxRpt
                    rptMapAttr.Name = baseMapAttr.Name;
                    rptMapAttr.MyDataType = baseMapAttr.MyDataType;
                    rptMapAttr.UIWidth = baseMapAttr.UIWidth;
                    rptMapAttr.UIHeight = baseMapAttr.UIHeight;
                    rptMapAttr.MinLen = baseMapAttr.MinLen;
                    rptMapAttr.MaxLen = baseMapAttr.MaxLen;
                    if (rptMapAttr.Update() > 0)
                        errorAppend.Append("@修改了" + "ND" + int.Parse(this.No) + "Rpt 表，字段" + keyOfEn + "修改为：" + baseMapAttr.MyDataTypeStr);
                    else
                        errorAppend.Append("@错误:修改" + "ND" + int.Parse(this.No) + "Rpt 表，字段" + keyOfEn + "修改为：" + baseMapAttr.MyDataTypeStr + "失败。");
                }
            }
            catch (Exception ex)
            {
                errorAppend.Append("@错误:" + ex.Message);
            }
            return errorAppend.ToString();
        }
        #endregion 其他方法.

        #region 检查流程.

        #endregion 检查流程.

        #region 产生数据模板。
        readonly static string PathFlowDesc;
        static Flow()
        {
            PathFlowDesc = SystemConfig.PathOfDataUser + "FlowDesc\\";
        }
        /// <summary>
        /// 生成流程模板
        /// </summary>
        /// <returns></returns>
        public string GenerFlowXmlTemplete()
        {
            string name = this.Name;
            name = BP.Tools.StringExpressionCalculate.ReplaceBadCharOfFileName(name);

            string path = this.No + "." + name;
            path = PathFlowDesc + path + "\\";

            this.DoExpFlowXmlTemplete(path);

            name = path + name + ".xml";
            return name;
        }
        /// <summary>
        /// 生成流程模板
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DataSet DoExpFlowXmlTemplete(string path)
        {
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            DataSet ds;
            try
            {
                ds = GetFlow(path);
            }
            catch (Exception e)
            {
                throw new Exception("err@流程模板导出失败：" + e.Message);
            }

            if (ds != null)
            {
                string name = this.Name;
                name = BP.Tools.StringExpressionCalculate.ReplaceBadCharOfFileName(name);
                name = name + ".xml";
                string filePath = path + name;
                ds.WriteXml(filePath);
            }
            return ds;
        }

        //xml文件是否正在操作中
        static bool isXmlLocked;
        /// <summary>
        /// 备份当前流程到用户xml文件
        /// 用户每次保存时调用
        /// 捕获异常写入日志,备份失败不影响正常保存
        /// </summary>
        public void WriteToXml()
        {
            try
            {
                string name = this.No + "." + this.Name;
                name = BP.Tools.StringExpressionCalculate.ReplaceBadCharOfFileName(name);
                string path = PathFlowDesc + name + "\\";
                DataSet ds = GetFlow(path);
                if (ds == null)
                    return;

                string directory = this.No + "." + this.Name;
                directory = BP.Tools.StringExpressionCalculate.ReplaceBadCharOfFileName(directory);
                path = PathFlowDesc + directory + "\\";
                string xmlName = path + "Flow" + ".xml";

                if (!isXmlLocked)
                {
                    if (!DataType.IsNullOrEmpty(path))
                    {
                        if (!System.IO.Directory.Exists(path))
                            System.IO.Directory.CreateDirectory(path);
                        else if (System.IO.File.Exists(xmlName))
                        {
                            DateTime time = File.GetLastWriteTime(xmlName);
                            string xmlNameOld = path + "Flow" + time.ToString("@yyyyMMddHHmmss") + ".xml";

                            isXmlLocked = true;
                            if (File.Exists(xmlNameOld))
                                File.Delete(xmlNameOld);
                            File.Move(xmlName, xmlNameOld);
                        }
                    }

                    if (DataType.IsNullOrEmpty(xmlName) == false)
                    {
                        ds.WriteXml(xmlName);
                        isXmlLocked = false;
                    }
                }
            }
            catch (Exception e)
            {
                isXmlLocked = false;
                Log.DefaultLogWriteLineError("流程模板文件备份错误:" + e.Message);
            }
        }
        public DataSet GetFlow(string path)
        {
            // 把所有的数据都存储在这里。
            DataSet ds = new DataSet();

            // 流程信息。
            string sql = "SELECT * FROM WF_Flow WHERE No='" + this.No + "'";

            Flow fl = new Flow(this.No);
            DataTable dtFlow = fl.ToDataTableField("WF_Flow");
            dtFlow.TableName = "WF_Flow";
            ds.Tables.Add(dtFlow);

            // 节点信息
            Nodes nds = new Nodes(this.No);
            DataTable dtNodes = nds.ToDataTableField("WF_Node");
            ds.Tables.Add(dtNodes);

            //节点属性
            NodeExts ndexts = new NodeExts(this.No);
            DataTable dtNodeExts = ndexts.ToDataTableField("WF_NodeExt");
            ds.Tables.Add(dtNodeExts);

            //接收人规则
            Selectors selectors = new Selectors(this.No);
            DataTable dtSelectors = selectors.ToDataTableField("WF_Selector");
            ds.Tables.Add(dtSelectors);

            // 单据模版. 
            BillTemplates tmps = new BillTemplates(this.No);
            string pks = "";
            foreach (BillTemplate tmp in tmps)
            {
                try
                {
                    if (path != null)
                        System.IO.File.Copy(SystemConfig.PathOfDataUser + @"\CyclostyleFile\" + tmp.No + ".rtf", path + "\\" + tmp.No + ".rtf", true);
                }
                catch
                {
                    pks += "@" + tmp.PKVal;
                    tmp.Delete();
                }
            }
            tmps.Remove(pks);
            ds.Tables.Add(tmps.ToDataTableField("WF_BillTemplate"));


            string sqlin = "SELECT NodeID FROM WF_Node WHERE fk_flow='" + this.No + "'";

            // 条件信息
            Conds cds = new Template.Conds(this.No);
            ds.Tables.Add(cds.ToDataTableField("WF_Cond"));

            // 节点与表单绑定.
            FrmNodes fns = new Template.FrmNodes();
            fns.Retrieve(FrmNodeAttr.FK_Flow, this.No);
            ds.Tables.Add(fns.ToDataTableField("WF_FrmNode"));


            // 表单方案.
            FrmFields ffs = new Template.FrmFields();
            ffs.Retrieve(FrmFieldAttr.FK_Flow, this.No);
            ds.Tables.Add(ffs.ToDataTableField("Sys_FrmSln"));

            // 方向
            Directions dirs = new Directions();
            dirs.Retrieve(DirectionAttr.FK_Flow, this.No);
            ds.Tables.Add(dirs.ToDataTableField("WF_Direction"));

            // 流程标签.
            LabNotes labs = new LabNotes(this.No);
            ds.Tables.Add(labs.ToDataTableField("WF_LabNote"));

            // 可退回的节点。
            NodeReturns nrs = new NodeReturns();
            nrs.RetrieveInSQL(NodeReturnAttr.FK_Node, sqlin);
            ds.Tables.Add(nrs.ToDataTableField("WF_NodeReturn"));


            // 工具栏。
            NodeToolbars tools = new NodeToolbars();
            tools.RetrieveInSQL(NodeToolbarAttr.FK_Node, sqlin);
            ds.Tables.Add(tools.ToDataTableField("WF_NodeToolbar"));


            // 节点与部门。
            NodeDepts ndepts = new NodeDepts();
            ndepts.RetrieveInSQL(NodeDeptAttr.FK_Node, sqlin);
            ds.Tables.Add(ndepts.ToDataTableField("WF_NodeDept"));


            // 节点与岗位权限。
            NodeStations nss = new NodeStations();
            nss.RetrieveInSQL(NodeStationAttr.FK_Node, sqlin);
            ds.Tables.Add(nss.ToDataTableField("WF_NodeStation"));

            // 节点与人员。
            NodeEmps nes = new NodeEmps();
            nes.RetrieveInSQL(NodeEmpAttr.FK_Node, sqlin);
            ds.Tables.Add(nes.ToDataTableField("WF_NodeEmp"));

            // 抄送人员。
            CCEmps ces = new CCEmps();
            ces.RetrieveInSQL(CCEmpAttr.FK_Node, sqlin);
            ds.Tables.Add(ces.ToDataTableField("WF_CCEmp"));

            // 抄送部门。
            CCDepts cdds = new CCDepts();
            cdds.RetrieveInSQL(CCDeptAttr.FK_Node, sqlin);
            ds.Tables.Add(cdds.ToDataTableField("WF_CCDept"));

            //子流程。
            SubFlows fls = new SubFlows();
            fls.RetrieveInSQL(CCDeptAttr.FK_Node, sqlin);
            ds.Tables.Add(fls.ToDataTableField("WF_NodeSubFlow"));

            //表单信息，包含从表.
            sql = "SELECT No FROM Sys_MapData WHERE " + Glo.MapDataLikeKey(this.No, "No");
            MapDatas mds = new MapDatas();
            mds.RetrieveInSQL(MapDataAttr.No, sql);
            DataTable dt = mds.ToDataTableField("Sys_MapData");
            dt.Columns.Add("HtmlTemplateFile");
            foreach (DataRow dr in dt.Rows)
            {
                if (int.Parse(dr["FrmType"].ToString()) == (int)FrmType.Develop)
                {
                    string htmlCode = DBAccess.GetBigTextFromDB("Sys_MapData", "No", dr["No"].ToString(), "HtmlTemplateFile");
                    dr["HtmlTemplateFile"] = htmlCode;
                }

            }

            ds.Tables.Add(dt);

            // Sys_MapAttr.
            sql = "SELECT MyPK FROM Sys_MapAttr WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            sql += " UNION ";   //增加多附件的扩展列.
            sql += "SELECT MyPK FROM Sys_MapAttr WHERE FK_MapData IN ( SELECT MyPK FROM Sys_FrmAttachment WHERE FK_Node=0 AND " + Glo.MapDataLikeKey(this.No, "FK_MapData") + " ) ";

            MapAttrs attrs = new MapAttrs();
            attrs.RetrieveInSQL(MapAttrAttr.MyPK, sql);
            ds.Tables.Add(attrs.ToDataTableField("Sys_MapAttr"));


            // Sys_EnumMain
            sql = "SELECT No FROM Sys_EnumMain WHERE No IN (SELECT UIBindKey from Sys_MapAttr WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData") + ")";
            SysEnumMains ses = new SysEnumMains();
            ses.RetrieveInSQL(SysEnumMainAttr.No, sql);
            ds.Tables.Add(ses.ToDataTableField("Sys_EnumMain"));


            // Sys_Enum
            sql = "SELECT MyPK FROM Sys_Enum WHERE EnumKey IN ( SELECT No FROM Sys_EnumMain WHERE No IN (SELECT UIBindKey from Sys_MapAttr WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData") + " ) )";
            SysEnums sesDtl = new SysEnums();
            sesDtl.RetrieveInSQL("MyPK", sql);
            ds.Tables.Add(sesDtl.ToDataTableField("Sys_Enum"));


            // Sys_MapDtl
            sql = "SELECT No FROM Sys_MapDtl WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            MapDtls mdtls = new MapDtls();
            mdtls.RetrieveInSQL(sql);
            ds.Tables.Add(mdtls.ToDataTableField("Sys_MapDtl"));


            // Sys_MapExt
            sql = "SELECT MyPK FROM Sys_MapExt WHERE  " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            MapExts mexts = new MapExts();
            mexts.RetrieveInSQL(sql);
            ds.Tables.Add(mexts.ToDataTableField("Sys_MapExt"));



            // Sys_GroupField
            sql = "SELECT OID FROM Sys_GroupField WHERE   " + Glo.MapDataLikeKey(this.No, "FrmID"); // +" " + Glo.MapDataLikeKey(this.No, "EnName");
            GroupFields gfs = new GroupFields();
            gfs.RetrieveInSQL(sql);
            ds.Tables.Add(gfs.ToDataTableField("Sys_GroupField"));


            // Sys_MapFrame
            sql = "SELECT MyPK FROM Sys_MapFrame WHERE" + Glo.MapDataLikeKey(this.No, "FK_MapData");
            MapFrames mfs = new MapFrames();
            mfs.RetrieveInSQL("MyPK", sql);
            ds.Tables.Add(mfs.ToDataTableField("Sys_MapFrame"));



            // Sys_FrmLine.
            sql = "SELECT MyPK FROM Sys_FrmLine WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            FrmLines frmls = new FrmLines();
            frmls.RetrieveInSQL(sql);
            ds.Tables.Add(frmls.ToDataTableField("Sys_FrmLine"));


            // Sys_FrmLab.
            sql = "SELECT MyPK FROM Sys_FrmLab WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            FrmLabs frmlabs = new FrmLabs();
            frmlabs.RetrieveInSQL(sql);
            ds.Tables.Add(frmlabs.ToDataTableField("Sys_FrmLab"));


            // Sys_FrmLink.
            sql = "SELECT MyPK FROM Sys_FrmLink WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            FrmLinks frmLinks = new FrmLinks();
            frmLinks.RetrieveInSQL(sql);
            ds.Tables.Add(frmLinks.ToDataTableField("Sys_FrmLink"));

            // Sys_FrmRB.
            sql = "SELECT MyPK FROM Sys_FrmRB WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            FrmRBs frmRBs = new FrmRBs();
            frmRBs.RetrieveInSQL(sql);
            ds.Tables.Add(frmRBs.ToDataTableField("Sys_FrmRB"));

            // Sys_FrmImgAth.
            sql = "SELECT MyPK FROM Sys_FrmImgAth WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            FrmImgAths frmIs = new FrmImgAths();
            frmIs.RetrieveInSQL(sql);
            ds.Tables.Add(frmIs.ToDataTableField("Sys_FrmImgAth"));

            // Sys_FrmImg.
            sql = "SELECT MyPK FROM Sys_FrmImg WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            FrmImgs frmImgs = new FrmImgs();
            frmImgs.RetrieveInSQL(sql);
            ds.Tables.Add(frmImgs.ToDataTableField("Sys_FrmImg"));


            // Sys_FrmAttachment.
            sql = "SELECT MyPK FROM Sys_FrmAttachment WHERE FK_Node=0 AND " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            FrmAttachments frmaths = new FrmAttachments();
            frmaths.RetrieveInSQL(sql);
            ds.Tables.Add(frmaths.ToDataTableField("Sys_FrmAttachment"));

            // Sys_FrmEvent.
            sql = "SELECT MyPK FROM Sys_FrmEvent WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            FrmEvents frmevens = new FrmEvents();
            frmevens.RetrieveInSQL(sql);
            ds.Tables.Add(frmevens.ToDataTableField("Sys_FrmEvent"));
            return ds;
        }
        public DataSet GetFlow2017(string path)
        {
            // 把所有的数据都存储在这里。
            DataSet ds = new DataSet();

            // 流程信息。
            string sql = "SELECT * FROM WF_Flow WHERE No='" + this.No + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_Flow";
            ds.Tables.Add(dt);

            // 节点信息
            sql = "SELECT * FROM WF_Node WHERE FK_Flow='" + this.No + "'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_Node";
            ds.Tables.Add(dt);

            // 单据模版. 
            BillTemplates tmps = new BillTemplates(this.No);
            string pks = "";
            foreach (BillTemplate tmp in tmps)
            {
                try
                {
                    if (path != null)
                        System.IO.File.Copy(SystemConfig.PathOfDataUser + @"\CyclostyleFile\" + tmp.No + ".rtf", path + "\\" + tmp.No + ".rtf", true);
                }
                catch
                {
                    pks += "@" + tmp.PKVal;
                    tmp.Delete();
                }
            }
            tmps.Remove(pks);
            ds.Tables.Add(tmps.ToDataTableField("WF_BillTemplate"));

            string sqlin = "SELECT NodeID FROM WF_Node WHERE fk_flow='" + this.No + "'";


            // 条件信息
            sql = "SELECT * FROM WF_Cond WHERE FK_Flow='" + this.No + "'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_Cond";
            ds.Tables.Add(dt);

            // 转向规则.
            //sql = "SELECT * FROM WF_TurnTo WHERE FK_Flow='" + this.No + "'";
            //dt = DBAccess.RunSQLReturnTable(sql);
            //dt.TableName = "WF_TurnTo";
            //ds.Tables.Add(dt);

            // 节点与表单绑定.
            sql = "SELECT * FROM WF_FrmNode WHERE FK_Flow='" + this.No + "'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_FrmNode";
            ds.Tables.Add(dt);

            // 表单方案.
            sql = "SELECT * FROM Sys_FrmSln WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmSln";
            ds.Tables.Add(dt);

            // 方向
            sql = "SELECT * FROM WF_Direction WHERE Node IN (" + sqlin + ") OR ToNode In (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_Direction";
            ds.Tables.Add(dt);

            // 流程标签.
            LabNotes labs = new LabNotes(this.No);
            ds.Tables.Add(labs.ToDataTableField("WF_LabNote"));


            // 可退回的节点。
            sql = "SELECT * FROM WF_NodeReturn WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_NodeReturn";
            ds.Tables.Add(dt);

            // 工具栏。
            sql = "SELECT * FROM WF_NodeToolbar WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_NodeToolbar";
            ds.Tables.Add(dt);

            // 节点与部门。
            sql = "SELECT * FROM WF_NodeDept WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_NodeDept";
            ds.Tables.Add(dt);


            // 节点与岗位权限。
            sql = "SELECT * FROM WF_NodeStation WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_NodeStation";
            ds.Tables.Add(dt);

            // 节点与人员。
            sql = "SELECT * FROM WF_NodeEmp WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_NodeEmp";
            ds.Tables.Add(dt);

            // 抄送人员。
            sql = "SELECT * FROM WF_CCEmp WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_CCEmp";
            ds.Tables.Add(dt);

            // 抄送部门。
            sql = "SELECT * FROM WF_CCDept WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_CCDept";
            ds.Tables.Add(dt);

            // 抄送部门。
            sql = "SELECT * FROM WF_CCStation WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_CCStation";
            ds.Tables.Add(dt);

            // 延续子流程。
            sql = "SELECT * FROM WF_NodeSubFlow WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_NodeSubFlow";
            ds.Tables.Add(dt);


            int flowID = int.Parse(this.No);
            sql = "SELECT * FROM Sys_MapData WHERE " + Glo.MapDataLikeKey(this.No, "No");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapData";
            ds.Tables.Add(dt);


            // Sys_MapAttr.
            sql = "SELECT * FROM Sys_MapAttr WHERE  " + Glo.MapDataLikeKey(this.No, "FK_MapData") + " ORDER BY FK_MapData,Idx";
            //sql = "SELECT * FROM Sys_MapAttr WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData") + "  ORDER BY FK_MapData,Idx";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapAttr";
            ds.Tables.Add(dt);

            // Sys_EnumMain
            //sql = "SELECT * FROM Sys_EnumMain WHERE No IN (SELECT KeyOfEn from Sys_MapAttr WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData") +")";
            sql = "SELECT * FROM Sys_EnumMain WHERE No IN (SELECT UIBindKey from Sys_MapAttr WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData") + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_EnumMain";
            ds.Tables.Add(dt);

            // Sys_Enum
            sql = "SELECT * FROM Sys_Enum WHERE EnumKey IN ( SELECT No FROM Sys_EnumMain WHERE No IN (SELECT UIBindKey from Sys_MapAttr WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData") + " ) )";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_Enum";
            ds.Tables.Add(dt);

            // Sys_MapDtl
            sql = "SELECT * FROM Sys_MapDtl WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapDtl";
            ds.Tables.Add(dt);

            // Sys_MapExt
            //sql = "SELECT * FROM Sys_MapExt WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            sql = "SELECT * FROM Sys_MapExt WHERE  " + Glo.MapDataLikeKey(this.No, "FK_MapData");  // +Glo.MapDataLikeKey(this.No, "FK_MapData");

            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            // Sys_GroupField
            sql = "SELECT * FROM Sys_GroupField WHERE   " + Glo.MapDataLikeKey(this.No, "FrmID"); // +" " + Glo.MapDataLikeKey(this.No, "EnName");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_GroupField";
            ds.Tables.Add(dt);

            // Sys_MapFrame
            sql = "SELECT * FROM Sys_MapFrame WHERE" + Glo.MapDataLikeKey(this.No, "FK_MapData");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapFrame";
            ds.Tables.Add(dt);

            // Sys_FrmLine.
            sql = "SELECT * FROM Sys_FrmLine WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmLine";
            ds.Tables.Add(dt);

            // Sys_FrmLab.
            sql = "SELECT * FROM Sys_FrmLab WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmLab";
            ds.Tables.Add(dt);

            // Sys_FrmEle.
            sql = "SELECT * FROM Sys_FrmEle WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmEle";
            ds.Tables.Add(dt);

            // Sys_FrmLink.
            sql = "SELECT * FROM Sys_FrmLink WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmLink";
            ds.Tables.Add(dt);

            // Sys_FrmRB.
            sql = "SELECT * FROM Sys_FrmRB WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmRB";
            ds.Tables.Add(dt);

            // Sys_FrmImgAth.
            sql = "SELECT * FROM Sys_FrmImgAth WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmImgAth";
            ds.Tables.Add(dt);

            // Sys_FrmImg.
            sql = "SELECT * FROM Sys_FrmImg WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmImg";
            ds.Tables.Add(dt);

            // Sys_FrmAttachment.
            sql = "SELECT * FROM Sys_FrmAttachment WHERE FK_Node=0 AND " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmAttachment";
            ds.Tables.Add(dt);

            // Sys_FrmEvent.
            sql = "SELECT * FROM Sys_FrmEvent WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmEvent";
            ds.Tables.Add(dt);
            return ds;
        }

        #endregion 产生数据模板。

        #region 其他公用方法1
        /// <summary>
        /// 重新设置Rpt表
        /// </summary>
        public void CheckRptOfReset()
        {
            string fk_mapData = "ND" + int.Parse(this.No) + "Rpt";
            string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapData + "'";
            DBAccess.RunSQL(sql);

            sql = "DELETE FROM Sys_MapData WHERE No='" + fk_mapData + "'";
            DBAccess.RunSQL(sql);
            this.DoCheck_CheckRpt(this.HisNodes);
        }
        /// <summary>
        /// 重新装载
        /// </summary>
        /// <returns></returns>
        public string DoReloadRptData()
        {
            this.DoCheck_CheckRpt(this.HisNodes);

            // 检查报表数据是否丢失。

            if (this.HisDataStoreModel != DataStoreModel.ByCCFlow)
                return "@流程" + this.No + this.Name + "的数据存储非轨迹模式不能重新生成.";

            DBAccess.RunSQL("DELETE FROM " + this.PTable);

            string sql = "SELECT OID FROM ND" + int.Parse(this.No) + "01 WHERE  OID NOT IN (SELECT OID FROM  " + this.PTable + " ) ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            this.CheckRptData(this.HisNodes, dt);

            return "@共有:" + dt.Rows.Count + "条(" + this.Name + ")数据被装载成功。";
        }
        /// <summary>
        /// 检查与修复报表数据
        /// </summary>
        /// <param name="nds"></param>
        /// <param name="dt"></param>
        private string CheckRptData(Nodes nds, DataTable dt)
        {
            GERpt rpt = new GERpt("ND" + int.Parse(this.No) + "Rpt");
            string err = "";
            foreach (DataRow dr in dt.Rows)
            {
                rpt.ResetDefaultVal();
                int oid = int.Parse(dr[0].ToString());
                rpt.SetValByKey("OID", oid);
                Work startWork = null;
                Work endWK = null;
                string flowEmps = "@";
                foreach (Node nd in nds)
                {
                    try
                    {
                        Work wk = nd.HisWork;
                        wk.OID = oid;
                        if (wk.RetrieveFromDBSources() == 0)
                            continue;

                        rpt.Copy(wk);
                        if (nd.NodeID == int.Parse(this.No + "01"))
                            startWork = wk;

                        try
                        {
                            if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                            {
                                if (flowEmps.Contains("@" + wk.RecOfEmp.Name + "@"))
                                    continue;
                                flowEmps += wk.RecOfEmp.Name + "@";
                            }

                            if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDOnly)
                            {
                                if (flowEmps.Contains("@" + wk.Rec + "@"))
                                    continue;
                                flowEmps += wk.Rec + "@";
                            }

                            if (Glo.UserInfoShowModel == UserInfoShowModel.UserNameOnly)
                            {
                                if (flowEmps.Contains("@" + wk.Rec + ","))
                                    continue;
                                flowEmps += wk.Rec + "," + wk.RecOfEmp.Name;
                            }




                        }
                        catch
                        {
                        }
                        endWK = wk;
                    }
                    catch (Exception ex)
                    {
                        err += ex.Message;
                    }
                }

                if (startWork == null || endWK == null)
                    continue;

                rpt.SetValByKey("OID", oid);
                rpt.FK_NY = startWork.GetValStrByKey("RDT").Substring(0, 7);
                rpt.FK_Dept = startWork.GetValStrByKey("FK_Dept");
                if (DataType.IsNullOrEmpty(rpt.FK_Dept))
                {
                    string fk_dept = DBAccess.RunSQLReturnString("SELECT FK_Dept FROM Port_Emp WHERE No='" + startWork.Rec + "'");
                    rpt.FK_Dept = fk_dept;

                    startWork.SetValByKey("FK_Dept", fk_dept);
                    startWork.Update();
                }
                rpt.Title = startWork.GetValStrByKey("Title");

                string wfState = DBAccess.RunSQLReturnStringIsNull("SELECT WFState FROM WF_GenerWorkFlow WHERE WorkID=" + oid, "1");
                rpt.WFState = (WFState)int.Parse(wfState);
                rpt.FlowStarter = startWork.Rec;
                rpt.FlowStartRDT = DBAccess.RunSQLReturnStringIsNull("SELECT WFState FROM WF_GenerWorkFlow WHERE WorkID=" + oid, "1");
                rpt.FID = startWork.GetValIntByKey("FID");
                rpt.FlowEmps = flowEmps;
                rpt.FlowEnder = endWK.Rec;
                //rpt.FlowEnderRDT = endWK.RDT;
                rpt.FlowEndNode = endWK.NodeID;

                //修复标题字段。
                WorkNode wn = new WorkNode(startWork, this.HisStartNode);
                rpt.Title = BP.WF.WorkFlowBuessRole.GenerTitle(this, startWork);
                //try
                //{
                //    TimeSpan ts = endWK.RDT_DateTime - startWork.RDT_DateTime;
                //    rpt.FlowDaySpan =  ts.Days;
                //}
                //catch
                //{
                //}
                rpt.InsertAsOID(rpt.OID);
            } // 结束循环。
            return err;
        }
        /// <summary>
        /// 生成明细报表信息
        /// </summary>
        /// <param name="nds"></param>
        private void CheckRptDtl(Nodes nds)
        {
            MapDtls dtlsDtl = new MapDtls();
            dtlsDtl.Retrieve(MapDtlAttr.FK_MapData, "ND" + int.Parse(this.No) + "Rpt");
            foreach (MapDtl dtl in dtlsDtl)
            {
                dtl.Delete();
            }

            //  dtlsDtl.Delete(MapDtlAttr.FK_MapData, "ND" + int.Parse(this.No) + "Rpt");
            foreach (Node nd in nds)
            {
                if (nd.IsEndNode == false)
                    continue;

                // 取出来从表.
                MapDtls dtls = new MapDtls("ND" + nd.NodeID);
                if (dtls.Count == 0)
                    continue;

                string rpt = "ND" + int.Parse(this.No) + "Rpt";
                int i = 0;
                foreach (MapDtl dtl in dtls)
                {
                    i++;
                    string rptDtlNo = "ND" + int.Parse(this.No) + "RptDtl" + i.ToString();
                    MapDtl rtpDtl = new MapDtl();
                    rtpDtl.No = rptDtlNo;
                    if (rtpDtl.RetrieveFromDBSources() == 0)
                    {
                        rtpDtl.Copy(dtl);
                        rtpDtl.No = rptDtlNo;
                        rtpDtl.FK_MapData = rpt;
                        rtpDtl.PTable = rptDtlNo;
                        rtpDtl.Insert();
                    }

                    MapAttrs attrsRptDtl = new MapAttrs(rptDtlNo);
                    MapAttrs attrs = new MapAttrs(dtl.No);
                    foreach (MapAttr attr in attrs)
                    {
                        if (attrsRptDtl.Contains(MapAttrAttr.KeyOfEn, attr.KeyOfEn) == true)
                            continue;

                        MapAttr attrN = new MapAttr();
                        attrN.Copy(attr);
                        attrN.FK_MapData = rptDtlNo;
                        switch (attr.KeyOfEn)
                        {
                            case "FK_NY":
                                attrN.UIVisible = true;
                                attrN.Idx = 100;
                                attrN.UIWidth = 60;
                                break;
                            case "RDT":
                                attrN.UIVisible = true;
                                attrN.Idx = 100;
                                attrN.UIWidth = 60;
                                break;
                            case "Rec":
                                attrN.UIVisible = true;
                                attrN.Idx = 100;
                                attrN.UIWidth = 60;
                                break;
                            default:
                                break;
                        }

                        attrN.Save();
                    }

                    GEDtl geDtl = new GEDtl(rptDtlNo);
                    geDtl.CheckPhysicsTable();
                }
            }
        }

        /// <summary>
        /// 检查数据报表.
        /// </summary>
        /// <param name="nds"></param>
        private void DoCheck_CheckRpt(Nodes nds)
        {
            string fk_mapData = "ND" + int.Parse(this.No) + "Rpt";
            string flowId = int.Parse(this.No).ToString();


            //生成该节点的 nds 比如  "'ND101','ND102','ND103'"
            string ndsstrs = "";
            foreach (BP.WF.Node nd in nds)
            {
                ndsstrs += "'ND" + nd.NodeID + "',";
            }
            ndsstrs = ndsstrs.Substring(0, ndsstrs.Length - 1);

            #region 插入字段。
            string sql = "SELECT distinct KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN (" + ndsstrs + ")";
            if (SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "SELECT A.* FROM (" + sql + ") AS A ";
                string sql3 = "DELETE FROM Sys_MapAttr WHERE KeyOfEn NOT IN (" + sql + ") AND FK_MapData='" + fk_mapData + "' ";
                DBAccess.RunSQL(sql3); // 删除不存在的字段.
            }
            else
            {
                string sql2 = "DELETE FROM Sys_MapAttr WHERE KeyOfEn NOT IN (" + sql + ") AND FK_MapData='" + fk_mapData + "' ";
                DBAccess.RunSQL(sql2); // 删除不存在的字段.
            }

            //所有节点表单字段的合集.
            sql = "SELECT MyPK, KeyOfEn,DefVal FROM Sys_MapAttr WHERE FK_MapData IN (" + ndsstrs + ")";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //求已经存在的字段集合。
            sql = "SELECT KeyOfEn FROM Sys_MapAttr WHERE FK_MapData='ND" + flowId + "Rpt'";
            DataTable dtExits = DBAccess.RunSQLReturnTable(sql);
            string pks = "@";
            foreach (DataRow dr in dtExits.Rows)
                pks += dr[0] + "@";

            //查询出来已经有的映射.
            MapAttrs attrs = new MapAttrs(fk_mapData);

            //遍历 - 所有节点表单字段的合集
            foreach (DataRow dr in dt.Rows)
            {
                if (pks.Contains("@" + dr["KeyOfEn"].ToString() + "@") == true)
                    continue;

                string mypk = dr["MyPK"].ToString();

                pks += dr["KeyOfEn"].ToString() + "@";

                //找到这个属性.
                BP.Sys.MapAttr ma = new BP.Sys.MapAttr(mypk);
                ma.MyPK = "ND" + flowId + "Rpt_" + ma.KeyOfEn;
                ma.FK_MapData = "ND" + flowId + "Rpt";
                ma.UIIsEnable = false;

                if (ma.DefValReal.Contains("@"))
                {
                    /*如果是一个有变量的参数.*/
                    ma.DefVal = "";
                }

                //如果包含他,就说已经存在.
                if (attrs.Contains("MyPK", ma.MyPK) == true)
                    continue;

                // 如果不存在.
                ma.Insert();
            }


            // 创建mapData.
            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = "ND" + flowId + "Rpt";
            if (md.RetrieveFromDBSources() == 0)
            {
                md.Name = this.Name;
                md.PTable = this.PTable;
                md.Insert();
            }
            else
            {
                if (md.Name.Equals(this.Name) || md.PTable.Equals(this.PTable) == false)
                    md.Update();
            }
            #endregion 插入字段。

            #region 补充上流程字段到NDxxxRpt.
            int groupID = 0;
            foreach (MapAttr attr in attrs)
            {
                switch (attr.KeyOfEn)
                {
                    case GERptAttr.FK_Dept:
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.UIVisible = true;
                        attr.GroupID = groupID;// gfs[0].GetValIntByKey("OID");
                        attr.UIIsEnable = false;
                        attr.DefVal = "";
                        attr.MaxLen = 100;
                        attr.Update();
                        break;
                    case "FK_NY":
                        //  attr.UIBindKey = "BP.Pub.NYs";
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.UIVisible = true;
                        attr.UIIsEnable = false;
                        attr.GroupID = groupID;
                        attr.Update();
                        break;
                    case "FK_Emp":
                        break;
                    default:
                        break;
                }
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.Title) == false)
            {
                /* 标题 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.Title; // "FlowEmps";
                attr.Name = "标题"; //  
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 400;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.OID) == false)
            {
                /* WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.KeyOfEn = "OID";
                attr.Name = "WorkID";
                attr.MyDataType = DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.HisEditType = BP.En.EditType.Readonly;
                attr.Insert();
            }


            if (attrs.Contains(md.No + "_" + GERptAttr.FID) == false)
            {
                /* WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.KeyOfEn = "FID";
                attr.Name = "FID";
                attr.MyDataType = DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.HisEditType = BP.En.EditType.Readonly;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.WFState) == false)
            {
                /* 流程状态 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.WFState;
                attr.Name = "流程状态"; //  
                attr.MyDataType = DataType.AppInt;
                attr.UIBindKey = GERptAttr.WFState;
                attr.UIContralType = UIContralType.DDL;
                attr.LGType = FieldTypeS.Enum;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 1000;
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.WFSta) == false)
            {
                /* 流程状态Ext */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.WFSta;
                attr.Name = "状态"; //  
                attr.MyDataType = DataType.AppInt;
                attr.UIBindKey = GERptAttr.WFSta;
                attr.UIContralType = UIContralType.DDL;
                attr.LGType = FieldTypeS.Enum;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 1000;
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEmps) == false)
            {
                /* 参与人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEmps; // "FlowEmps";
                attr.Name = "参与人"; //  
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 1000;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowStarter) == false)
            {
                /* 发起人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowStarter;
                attr.Name = "发起人"; //  
                attr.MyDataType = DataType.AppString;

                //attr.UIBindKey = "BP.Port.Emps";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;

                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowStartRDT) == false)
            {
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowStartRDT; // "FlowStartRDT";
                attr.Name = "发起时间";
                attr.MyDataType = DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEnder) == false)
            {
                /* 发起人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEnder;
                attr.Name = "结束人"; //  
                attr.MyDataType = DataType.AppString;
                // attr.UIBindKey = "BP.Port.Emps";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEnderRDT) == false)
            {
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEnderRDT; // "FlowStartRDT";
                attr.Name = "结束时间";
                attr.MyDataType = DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEndNode) == false)
            {
                /* 结束节点 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEndNode;
                attr.Name = "结束节点";
                attr.MyDataType = DataType.AppInt;
                attr.DefVal = "0";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowDaySpan) == false)
            {
                /* FlowDaySpan */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowDaySpan; // "FlowStartRDT";
                attr.Name = "跨度(天)";
                attr.MyDataType = DataType.AppFloat;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.UIIsLine = false;
                attr.Idx = -101;
                attr.DefVal = "0";
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PFlowNo) == false)
            {
                /* 父流程 流程编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PFlowNo;
                attr.Name = "父流程编号"; //  父流程流程编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 3;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PNodeID) == false)
            {
                /* 父流程WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PNodeID;
                attr.Name = "父流程启动的节点";
                attr.MyDataType = DataType.AppInt;
                attr.DefVal = "0";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PWorkID) == false)
            {
                /* 父流程WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PWorkID;
                attr.Name = "父流程WorkID";
                attr.MyDataType = DataType.AppInt;
                attr.DefVal = "0";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PEmp) == false)
            {
                /* 调起子流程的人员 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PEmp;
                attr.Name = "调起子流程的人员";
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.BillNo) == false)
            {
                /* 父流程 流程编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.BillNo;
                attr.Name = "单据编号"; //  单据编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.MinLen = 0;
                attr.MaxLen = 100;
                attr.Idx = -100;
                attr.Insert();
            }




            if (attrs.Contains(md.No + "_" + GERptAttr.AtPara) == false)
            {
                /* 父流程 流程编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.AtPara;
                attr.Name = "参数"; // 单据编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.MinLen = 0;
                attr.MaxLen = 4000;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.GUID) == false)
            {
                /* 父流程 流程编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.GUID;
                attr.Name = "GUID"; // 单据编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PrjNo) == false)
            {
                /* 项目编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PrjNo;
                attr.Name = "项目编号"; //  项目编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.MinLen = 0;
                attr.MaxLen = 100;
                attr.Idx = -100;
                attr.Insert();
            }
            if (attrs.Contains(md.No + "_" + GERptAttr.PrjName) == false)
            {
                /* 项目名称 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PrjName;
                attr.Name = "项目名称"; //  项目名称
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.MinLen = 0;
                attr.MaxLen = 100;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowNote) == false)
            {
                /* 流程信息 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowNote;
                attr.Name = "流程信息"; //  父流程流程编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 500;
                attr.Idx = -100;
                attr.Insert();
            }
            #endregion 补充上流程字段。

            #region 为流程字段设置分组。
            try
            {
                string flowInfo = "流程信息";
                GroupField flowGF = new GroupField();
                int num = flowGF.Retrieve(GroupFieldAttr.FrmID, fk_mapData, GroupFieldAttr.Lab, "流程信息");
                if (num == 0)
                {
                    flowGF = new GroupField();
                    flowGF.Lab = flowInfo;
                    flowGF.FrmID = fk_mapData;
                    flowGF.Idx = -1;
                    flowGF.Insert();
                }
                sql = "UPDATE Sys_MapAttr SET GroupID='" + flowGF.OID + "' WHERE  FK_MapData='" + fk_mapData + "'  AND KeyOfEn IN('" + GERptAttr.PFlowNo + "','" + GERptAttr.PWorkID + "','" + GERptAttr.FK_Dept + "','" + GERptAttr.FK_NY + "','" + GERptAttr.FlowDaySpan + "','" + GERptAttr.FlowEmps + "','" + GERptAttr.FlowEnder + "','" + GERptAttr.FlowEnderRDT + "','" + GERptAttr.FlowEndNode + "','" + GERptAttr.FlowStarter + "','" + GERptAttr.FlowStartRDT + "','" + GERptAttr.WFState + "')";
                DBAccess.RunSQL(sql);
            }
            catch (Exception ex)
            {
                Log.DefaultLogWriteLineError(ex.Message);
            }
            #endregion 为流程字段设置分组

            #region 尾后处理.
            GERpt gerpt = this.HisGERpt;
            gerpt.CheckPhysicsTable();  //让报表重新生成.

            DBAccess.RunSQL("DELETE FROM Sys_GroupField WHERE FrmID='" + fk_mapData + "' AND OID NOT IN (SELECT GroupID FROM Sys_MapAttr WHERE FK_MapData = '" + fk_mapData + "')");

            DBAccess.RunSQL("UPDATE Sys_MapAttr SET Name='活动时间' WHERE FK_MapData='ND" + flowId + "Rpt' AND KeyOfEn='CDT'");
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET Name='参与者' WHERE FK_MapData='ND" + flowId + "Rpt' AND KeyOfEn='Emps'");
            #endregion 尾后处理.
        }
        #endregion 其他公用方法1

        #region 执行流程事件.
        private BP.WF.FlowEventBase _FDEventEntity = null;
        /// <summary>
        /// 节点实体类，没有就返回为空.
        /// </summary>
        public BP.WF.FlowEventBase FEventEntity
        {
            get
            {
                if (_FDEventEntity == null
                    && DataType.IsNullOrEmpty(this.FlowMark) == false
                    && DataType.IsNullOrEmpty(this.FlowEventEntity) == false)
                    _FDEventEntity = BP.WF.Glo.GetFlowEventEntityByEnName(this.FlowEventEntity);
                return _FDEventEntity;
            }
        }
        #endregion 执行流程事件.

        #region 基本属性
        /// <summary>
        /// 是否是MD5加密流程
        /// </summary>
        public bool IsMD5
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsMD5);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsMD5, value);
            }
        }
        /// <summary>
        /// 是否有单据
        /// </summary>
        public int NumOfBill
        {
            get
            {
                return this.GetValIntByKey(FlowAttr.NumOfBill);
            }
            set
            {
                this.SetValByKey(FlowAttr.NumOfBill, value);
            }
        }
        /// <summary>
        /// 标题生成规则
        /// </summary>
        public string TitleRole
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.TitleRole);
            }
            set
            {
                this.SetValByKey(FlowAttr.TitleRole, value);
            }
        }
        /// <summary>
        /// 明细表
        /// </summary>
        public int NumOfDtl
        {
            get
            {
                return this.GetValIntByKey(FlowAttr.NumOfDtl);
            }
            set
            {
                this.SetValByKey(FlowAttr.NumOfDtl, value);
            }
        }
        public decimal AvgDay
        {
            get
            {
                return this.GetValIntByKey(FlowAttr.AvgDay);
            }
            set
            {
                this.SetValByKey(FlowAttr.AvgDay, value);
            }
        }
        public int StartNodeID
        {
            get
            {
                return int.Parse(this.No + "01");
                //return this.GetValIntByKey(FlowAttr.StartNodeID);
            }
        }
        /// <summary>
        /// add 2013-01-01.
        /// 业务主表(默认为NDxxRpt)
        /// </summary>
        public string PTable
        {
            get
            {
                string s = this.GetValStringByKey(FlowAttr.PTable);
                if (DataType.IsNullOrEmpty(s))
                    s = "ND" + int.Parse(this.No) + "Rpt";
                return s;
            }
            set
            {
                this.SetValByKey(FlowAttr.PTable, value);
            }
        }
        /// <summary>
        /// 历史记录显示字段.
        /// </summary>
        public string HistoryFields
        {
            get
            {
                string strs = this.GetValStringByKey(FlowAttr.HistoryFields);
                if (DataType.IsNullOrEmpty(strs))
                    strs = "WFState,Title,FlowStartRDT,FlowEndNode";

                return strs;
            }
        }
        /// <summary>
        /// 是否启用？
        /// </summary>
        public bool IsGuestFlow
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsGuestFlow);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsGuestFlow, value);
            }
        }
        /// <summary>
        /// 是否可以独立启动
        /// </summary>
        public bool IsCanStart
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsCanStart);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsCanStart, value);
            }
        }
        /// <summary>
        /// 是否可以批量发起
        /// </summary>
        public bool IsBatchStart
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsBatchStart);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsBatchStart, value);
            }
        }
        /// <summary>
        /// 是否自动计算未来的处理人
        /// </summary>
        public bool IsFullSA
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsFullSA);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsFullSA, value);
            }
        }
        /// <summary>
        /// 批量发起字段
        /// </summary>
        public string BatchStartFields
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.BatchStartFields);
            }
            set
            {
                this.SetValByKey(FlowAttr.BatchStartFields, value);
            }
        }
        /// <summary>
        /// 单据格式
        /// </summary>
        public string BillNoFormat
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.BillNoFormat);
            }
            set
            {
                this.SetValByKey(FlowAttr.BillNoFormat, value);
            }
        }
        /// <summary>
        /// 流程类别
        /// </summary>
        public string FK_FlowSort
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.FK_FlowSort);
            }
            set
            {
                this.SetValByKey(FlowAttr.FK_FlowSort, value);
            }
        }
        /// <summary>
        /// 系统类别
        /// </summary>
        public string SysType
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.SysType);
            }
            set
            {
                this.SetValByKey(FlowAttr.SysType, value);
            }
        }
        /// <summary>
        /// 参数
        /// </summary>
        public string Paras
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.Paras);
            }
            set
            {
                this.SetValByKey(FlowAttr.Paras, value);
            }
        }
        /// <summary>
        /// 流程类别名称
        /// </summary>
        public string FK_FlowSortText
        {
            get
            {
                FlowSort fs = new FlowSort(this.FK_FlowSort);
                return fs.Name;
                //return this.GetValRefTextByKey(FlowAttr.FK_FlowSort);
            }
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Ver
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.Ver);
            }
            set
            {
                this.SetValByKey(FlowAttr.Ver, value);
            }
        }
        #endregion

        #region 计算属性
        /// <summary>
        /// 流程类型(大的类型)
        /// </summary>
        public int FlowType_del
        {
            get
            {
                return this.GetValIntByKey(FlowAttr.FlowType);
            }
        }
        /// <summary>
        /// (当前节点为子流程时)是否检查所有子流程完成后父流程自动发送
        /// </summary>
        public SubFlowOver SubFlowOver
        {
            get
            {
                return (SubFlowOver)this.GetValIntByKey(FlowAttr.IsAutoSendSubFlowOver);
            }
        }
        /// <summary>
        /// 是否启用数据模版？
        /// </summary>
        public bool IsDBTemplate
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsDBTemplate);
            }
        }
        public string Note
        {
            get
            {
                string s = this.GetValStringByKey("Note");
                if (s.Length == 0)
                {
                    return "无";
                }
                return s;
            }
        }
        public string NoteHtml
        {
            get
            {
                if (this.Note == "无" || this.Note == "")
                    return "流程设计人员没有编写此流程的帮助信息，请打开设计器-》打开此流程-》设计画布上点击右键-》流程属性-》填写流程帮助信息。";
                else
                    return this.Note;
            }
        }
        /// <summary>
        /// 是否多线程自动流程
        /// </summary>
        public bool IsMutiLineWorkFlow_del
        {
            get
            {
                return false;
                /*
                if (this.FlowType==2 || this.FlowType==1 )
                    return true;
                else
                    return false;
                    */
            }
        }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 应用类型
        /// </summary>
        public FlowAppType HisFlowAppType
        {
            get
            {
                return (FlowAppType)this.GetValIntByKey(FlowAttr.FlowAppType);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowAppType, (int)value);
            }
        }
        /// <summary>
        /// 数据存储模式
        /// </summary>
        public DataStoreModel HisDataStoreModel
        {
            get
            {
                return (DataStoreModel)this.GetValIntByKey(FlowAttr.DataStoreModel);
            }
            set
            {
                this.SetValByKey(FlowAttr.DataStoreModel, (int)value);
            }
        }
        /// <summary>
        /// 是否启用子流程运行结束后，主流程自动运行到下一节点
        /// </summary>
        public bool IsToParentNextNode
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsToParentNextNode);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsToParentNextNode, value);
            }
        }
        /// <summary>
        /// 节点
        /// </summary>
        private Nodes _HisNodes = null;
        /// <summary>
        /// 他的节点集合.
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                _HisNodes = new Nodes(this.No);
                return _HisNodes;
            }
            set
            {
                _HisNodes = value;
            }
        }
        /// <summary>
        /// 流程完成条件
        /// </summary>
        public Conds CondsOfFlowComplete
        {
            get
            {
                var ens = this.GetEntitiesAttrFromAutoNumCash(new Conds(),
                    CondAttr.FK_Flow, this.No, CondAttr.CondType, (int)CondType.Flow);
                return ens as Conds;
            }
        }
        /// <summary>
        /// 事件:
        /// </summary>
        public FrmEvents FrmEvents
        {
            get
            {
                var ens = this.GetEntitiesAttrFromAutoNumCash(new FrmEvents(),
                 FrmEventAttr.FK_Flow, this.No);
                return ens as FrmEvents;
            }
        }
        /// <summary>
        /// 他的 Start 节点
        /// </summary>
        public Node HisStartNode
        {
            get
            {

                foreach (Node nd in this.HisNodes)
                {
                    if (nd.IsStartNode)
                        return nd;
                }
                throw new Exception("@没有找到他的开始节点,工作流程[" + this.Name + "]定义错误.");
            }
        }
        /// <summary>
        /// 他的事务类别
        /// </summary>
        public FlowSort HisFlowSort
        {
            get
            {
                return new FlowSort(this.FK_FlowSort);
            }
        }
        /// <summary>
        /// flow data 数据
        /// </summary>
        public BP.WF.Data.GERpt HisGERpt
        {
            get
            {
                try
                {
                    BP.WF.Data.GERpt wk = new BP.WF.Data.GERpt("ND" + int.Parse(this.No) + "Rpt");
                    return wk;
                }
                catch
                {
                    this.DoCheck();
                    BP.WF.Data.GERpt wk1 = new BP.WF.Data.GERpt("ND" + int.Parse(this.No) + "Rpt");
                    return wk1;
                }
            }
        }

        public bool IsFrmEnable
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsFrmEnable);
            }
        }

        public bool IsTruckEnable
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsTruckEnable);
            }
        }

        public bool IsTimeBaseEnable
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsTimeBaseEnable);
            }
        }

        public bool IsTableEnable
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsTableEnable);
            }
        }

        public bool IsOPEnable
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsOPEnable);
            }
        }
        #endregion

        #region 构造方法

        /// <summary>
        /// 流程
        /// </summary>
        public Flow()
        {
            this.No = "";
        }
        /// <summary>
        /// 流程
        /// </summary>
        /// <param name="_No">编号</param>
        public Flow(string _No)
        {
            this.No = _No;
            if (SystemConfig.IsDebug)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("流程编号不存在");
            }
            else
            {
                this.Retrieve();
            }
        }
        protected override bool beforeUpdateInsertAction()
        {
            //获得事件实体.
            if (DataType.IsNullOrEmpty(this.FlowMark) == true)
                this.FlowEventEntity = BP.WF.Glo.GetFlowEventEntityStringByFlowMark(this.No);
            else
                this.FlowEventEntity = BP.WF.Glo.GetFlowEventEntityStringByFlowMark(this.FlowMark);

            DBAccess.RunSQL("UPDATE WF_Node SET FlowName='" + this.Name + "' WHERE FK_Flow='" + this.No + "'");
            DBAccess.RunSQL("UPDATE Sys_MapData SET Name='" + this.Name + "' WHERE No='" + this.PTable + "'");
            return base.beforeUpdateInsertAction();
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

                Map map = new Map("WF_Flow", "流程");

                //取消了缓存.
                map.DepositaryOfEntity= Depositary.Application;
                //map.DepositaryOfEntity= Depositary.Application;
                map.CodeStruct = "3";

                map.AddTBStringPK(FlowAttr.No, null, "编号", true, true, 1, 4, 3);
                map.AddTBString(FlowAttr.Name, null, "名称", true, false, 0, 200, 10);

                map.AddDDLEntities(FlowAttr.FK_FlowSort, "01", "流程类别", new FlowSorts(), false);
                map.AddTBString(FlowAttr.SysType, null, "系统类别", true, false, 0, 3, 10);

                map.AddTBInt(FlowAttr.FlowRunWay, 0, "运行方式", false, false);

                //  map.AddDDLEntities(FlowAttr.FK_FlowSort, "01", "流程类别", new FlowSorts(), false);
                //map.AddDDLSysEnum(FlowAttr.FlowRunWay, (int)FlowRunWay.HandWork, "运行方式", false,
                //    false, FlowAttr.FlowRunWay,
                //    "@0=手工启动@1=指定人员按时启动@2=数据集按时启动@3=触发式启动");

                map.AddTBString(FlowAttr.RunObj, null, "运行内容", true, false, 0, 500, 10);
                map.AddTBString(FlowAttr.Note, null, "备注", true, false, 0, 300, 10);
                map.AddTBString(FlowAttr.RunSQL, null, "流程结束执行后执行的SQL", true, false, 0, 500, 10);

                map.AddTBInt(FlowAttr.NumOfBill, 0, "是否有单据", false, false);
                map.AddTBInt(FlowAttr.NumOfDtl, 0, "NumOfDtl", false, false);
                map.AddTBInt(FlowAttr.FlowAppType, 0, "流程类型", false, false);
                map.AddTBInt(FlowAttr.ChartType, 1, "节点图形类型", false, false);

                map.AddTBInt(FlowAttr.IsFrmEnable, 1, "是否显示表单", true, true);
                map.AddTBInt(FlowAttr.IsTruckEnable, 1, "是否显示轨迹图", true, true);
                map.AddTBInt(FlowAttr.IsTimeBaseEnable, 1, "是否显示时间轴", true, true);
                map.AddTBInt(FlowAttr.IsTableEnable, 1, "是否显示时间表", true, true);
                map.AddTBInt(FlowAttr.IsOPEnable, 1, "是否显示操作", true, true);

                // map.AddBoolean(FlowAttr.IsOK, true, "是否启用", true, true);
                map.AddTBInt(FlowAttr.IsCanStart, 1, "可以独立启动否？", true, true);
                map.AddTBInt(FlowAttr.IsStartInMobile, 1, "是否可以在手机里发起？", true, true);

                map.AddTBDecimal(FlowAttr.AvgDay, 0, "平均运行用天", false, false);

                map.AddTBInt(FlowAttr.IsFullSA, 0, "是否自动计算未来的处理人？(启用后,ccflow就会为已知道的节点填充处理人到WF_SelectAccper)", false, false);
                map.AddTBInt(FlowAttr.IsMD5, 0, "IsMD5", false, false);
                map.AddTBInt(FlowAttr.Idx, 0, "显示顺序号(在发起列表中)", true, false);

                map.AddTBInt(FlowAttr.SDTOfFlowRole, 0, "流程计划完成日期计算规则", true, false);
                map.AddTBString(FlowAttr.SDTOfFlowRoleSQL, null, "流程计划完成日期计算规则SQL", false, false, 0, 200, 10);

                map.AddTBString(FlowAttr.Paras, null, "参数", false, false, 0, 1000, 10);

                // add 2013-01-01. 
                map.AddTBString(FlowAttr.PTable, null, "流程数据存储主表", true, false, 0, 30, 10);

                // add 2019.11.07  @liuqiang.
                map.AddTBInt(FlowAttr.FlowFrmType, 0, "流程表单类型", true, true);
                map.AddTBString(FlowAttr.FrmUrl, null, "表单Url", true, false, 0, 150, 10, true);

                // 草稿规则 "@0=无(不设草稿)@1=保存到待办@2=保存到草稿箱"
                map.AddTBInt(FlowAttr.Draft, 0, "草稿规则", true, false);

                // add 2013-01-01.
                map.AddTBInt(FlowAttr.DataStoreModel, 0, "数据存储模式", true, false);

                // add 2013-02-05.
                map.AddTBString(FlowAttr.TitleRole, null, "标题生成规则", true, false, 0, 90, 10, true);

                // add 2013-02-14 
                map.AddTBString(FlowAttr.FlowMark, null, "流程标记", true, false, 0, 50, 10);
                map.AddTBString(FlowAttr.FlowEventEntity, null, "FlowEventEntity", true, false, 0, 100, 10, true);
                map.AddTBString(FlowAttr.HistoryFields, null, "历史查看字段", true, false, 0, 120, 10, true);
                map.AddTBInt(FlowAttr.IsGuestFlow, 0, "是否是客户参与流程？", true, false);
                map.AddTBString(FlowAttr.BillNoFormat, null, "单据编号格式", true, false, 0, 50, 10, true);
                map.AddTBString(FlowAttr.FlowNoteExp, null, "备注表达式", true, false, 0, 90, 10, true);

                //部门权限控制类型,此属性在报表中控制的.
                map.AddTBInt(FlowAttr.DRCtrlType, 0, "部门查询权限控制方式", true, false);

                //运行主机. 这个流程运行在那个子系统的主机上.
                map.AddTBString(FlowAttr.HostRun, null, "运行主机(IP+端口)", true, false, 0, 40, 10, true);
                map.AddTBInt(FlowAttr.IsToParentNextNode, 0, "子流程运行到该节点时，让父流程自动运行到下一步", false, false);

                #region 流程启动限制
                map.AddTBInt(FlowAttr.StartLimitRole, 0, "启动限制规则", true, false);
                map.AddTBString(FlowAttr.StartLimitPara, null, "规则内容", true, false, 0, 500, 10, true);
                map.AddTBString(FlowAttr.StartLimitAlert, null, "限制提示", true, false, 0, 500, 10, false);
                map.AddTBInt(FlowAttr.StartLimitWhen, 0, "提示时间", true, false);
                #endregion 流程启动限制

                #region 导航方式。
                map.AddTBInt(FlowAttr.StartGuideWay, 0, "前置导航方式", false, false);
                map.AddTBString(FlowAttr.StartGuideLink, null, "右侧的连接", true, false, 0, 200, 10, true);
                map.AddTBString(FlowAttr.StartGuideLab, null, "连接标签", true, false, 0, 200, 10, true);

                map.AddTBString(FlowAttr.StartGuidePara1, null, "参数1", true, false, 0, 500, 10, true);
                map.AddTBString(FlowAttr.StartGuidePara2, null, "参数2", true, false, 0, 500, 10, true);
                map.AddTBString(FlowAttr.StartGuidePara3, null, "参数3", true, false, 0, 500, 10, true);
                map.AddTBInt(FlowAttr.IsResetData, 0, "是否启用数据重置按钮？", true, false);
                //    map.AddTBInt(FlowAttr.IsImpHistory, 0, "是否启用导入历史数据按钮？", true, false);
                map.AddTBInt(FlowAttr.IsLoadPriData, 0, "是否导入上一个数据？", true, false);
                #endregion 导航方式。

                map.AddTBInt(FlowAttr.IsDBTemplate, 0, "是否启用数据模版？", true, false);

                //批量发起 add 2013-12-27. 
                map.AddTBInt(FlowAttr.IsBatchStart, 0, "是否可以批量发起", true, false);
                map.AddTBString(FlowAttr.BatchStartFields, null, "批量发起字段(用逗号分开)", true, false, 0, 200, 10, true);
                map.AddTBInt(FlowAttr.IsAutoSendSubFlowOver, 0, "(当前节点为子流程时)是否检查所有子流程完成后父流程自动发送", true, true);

                map.AddTBString(FlowAttr.Ver, null, "版本号", true, true, 0, 20, 10);
                //设计类型 .
                map.AddTBInt(FlowAttr.FlowDeleteRole, 0, "流程实例删除规则", true, false);

                map.AddTBString(FlowAttr.OrgNo, null, "OrgNo", true, true, 0, 50, 10);

                //参数.
                map.AddTBAtParas(1000);

                #region 数据同步方案
                //数据同步方式.
                map.AddTBInt(FlowAttr.DataDTSWay, (int)DataDTSWay.None, "同步方式", true, true);
                map.AddTBString(FlowAttr.DTSDBSrc, null, "数据源", true, false, 0, 200, 100, false);
                map.AddTBString(FlowAttr.DTSBTable, null, "业务表名", true, false, 0, 200, 100, false);
                map.AddTBString(FlowAttr.DTSBTablePK, null, "业务表主键", false, false, 0, 32, 10);
                map.AddTBString(FlowAttr.DTSSpecNodes, null, "同步字段", false, false, 0, 4000, 10);

                map.AddTBInt(FlowAttr.DTSTime, (int)FlowDTSTime.AllNodeSend, "执行同步时间点", true, true);
                map.AddTBString(FlowAttr.DTSFields, null, "要同步的字段s,中间用逗号分开.", false, false, 0, 900, 100, false);
                #endregion 数据同步方案

                // map.AddSearchAttr(FlowAttr.FK_FlowSort);
                // map.AddSearchAttr(FlowAttr.FlowRunWay);

                RefMethod rm = new RefMethod();
                rm.Title = "设计检查报告"; // "设计检查报告";
                rm.ToolTip = "检查流程设计的问题。";
                rm.Icon = "../../WF/Img/Btn/Confirm.gif";
                rm.ClassMethodName = this.ToString() + ".DoCheck";
                rm.GroupName = "流程维护";
                map.AddRefMethod(rm);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region  公共方法
        /// <summary>
        /// 创建索引
        /// </summary>
        /// <returns></returns>
        public string CreateIndex()
        {
            // 为track表创建索引.  FID, WorkID
            string ptable = "ND" + int.Parse(this.PTable) + "Track";

            // DBAccess.CreatIndex(DBUrlType.AppCenterDSN, ptable, "my");

            return "流程[" + this.No + "." + this.Name + "]索引创建成功.";
        }
        /// <summary>
        /// 删除数据.
        /// </summary>
        /// <returns></returns>
        public string DoDelData()
        {
            #region 删除独立表单的数据.
            string mysql = "SELECT OID FROM " + this.PTable;
            FrmNodes fns = new FrmNodes();
            fns.Retrieve(FrmNodeAttr.FK_Flow, this.No);
            string strs = "";
            foreach (FrmNode nd in fns)
            {
                if (strs.Contains("@" + nd.FK_Frm) == true)
                    continue;

                strs += "@" + nd.FK_Frm + "@";
                try
                {
                    MapData md = new MapData(nd.FK_Frm);
                    DBAccess.RunSQL("DELETE FROM " + md.PTable + " WHERE OID in (" + mysql + ")");
                }
                catch
                {
                }
            }
            #endregion 删除独立表单的数据.

            string sql = "  WHERE FK_Node in (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            string sql1 = " WHERE NodeID in (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            // DBAccess.RunSQL("DELETE FROM WF_CHOfFlow WHERE FK_Flow='" + this.No + "'");

            DBAccess.RunSQL("DELETE FROM WF_Bill WHERE FK_Flow='" + this.No + "'");
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE FK_Flow='" + this.No + "'");
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FK_Flow='" + this.No + "'");

            DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FK_Flow='" + this.No + "'");

            string sqlIn = " WHERE ReturnNode IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            DBAccess.RunSQL("DELETE FROM WF_ReturnWork " + sqlIn);
            DBAccess.RunSQL("DELETE FROM WF_SelectAccper " + sql);
            DBAccess.RunSQL("DELETE FROM WF_TransferCustom " + sql);
            // DBAccess.RunSQL("DELETE FROM WF_FileManager " + sql);
            DBAccess.RunSQL("DELETE FROM WF_RememberMe " + sql);

            if (DBAccess.IsExitsObject("ND" + int.Parse(this.No) + "Track"))
                DBAccess.RunSQL("DELETE FROM ND" + int.Parse(this.No) + "Track ");

            if (DBAccess.IsExitsObject(this.PTable))
                DBAccess.RunSQL("DELETE FROM " + this.PTable);

            DBAccess.RunSQL("DELETE FROM WF_CH WHERE FK_Flow='" + this.No + "'");
            //DBAccess.RunSQL("DELETE FROM Sys_MapExt WHERE FK_MapData LIKE 'ND"+int.Parse(this.No)+"%'" );

            //删除节点数据。
            Nodes nds = new Nodes(this.No);
            foreach (Node nd in nds)
            {
                try
                {
                    Work wk = nd.HisWork;
                    DBAccess.RunSQL("DELETE FROM " + wk.EnMap.PhysicsTable);
                }
                catch
                {
                }

                MapDtls dtls = new MapDtls("ND" + nd.NodeID);
                foreach (MapDtl dtl in dtls)
                {
                    try
                    {
                        DBAccess.RunSQL("DELETE FROM " + dtl.PTable);
                    }
                    catch
                    {
                    }
                }
            }
            MapDtls mydtls = new MapDtls("ND" + int.Parse(this.No) + "Rpt");
            foreach (MapDtl dtl in mydtls)
            {
                try
                {
                    DBAccess.RunSQL("DELETE FROM " + dtl.PTable);
                }
                catch
                {
                }
            }
            return "删除成功...";
        }
        /// <summary>
        /// 装载流程模板
        /// </summary>
        /// <param name="fk_flowSort">流程类别</param>
        /// <param name="path">流程名称</param>
        /// <returns></returns>
        public static Flow DoLoadFlowTemplate(string fk_flowSort, string path, ImpFlowTempleteModel model, string SpecialFlowNo = "")
        {
            FileInfo info = new FileInfo(path);
            DataSet ds = new DataSet();

            try
            {
                ds.ReadXml(path);
            }
            catch (Exception ex)
            {
                throw new Exception("@导入流程路径:" + path + "出错：" + ex.Message);
            }


            if (ds.Tables.Contains("WF_Flow") == false)
                throw new Exception("导入错误，非流程模版文件" + path + "。");

            DataTable dtFlow = ds.Tables["WF_Flow"];
            Flow fl = new Flow();
            string oldFlowNo = dtFlow.Rows[0]["No"].ToString();
            string oldFlowName = dtFlow.Rows[0]["Name"].ToString();

            int oldFlowID = int.Parse(oldFlowNo);
            int iOldFlowLength = oldFlowID.ToString().Length;
            string timeKey = DateTime.Now.ToString("yyMMddhhmmss");

            #region 根据不同的流程模式，设置生成不同的流程编号.
            switch (model)
            {
                case ImpFlowTempleteModel.AsNewFlow: /*做为一个新流程. */
                    fl.No = fl.GenerNewNo;
                    //  fl.DoDelData();
                    //    fl.DoDelete(); /*删除可能存在的垃圾.*/
                    fl.Insert();
                    break;
                case ImpFlowTempleteModel.AsTempleteFlowNo: /*用流程模版中的编号*/
                    fl.No = oldFlowNo;
                    if (fl.IsExits)
                        throw new Exception("导入错误:流程模版(" + oldFlowName + ")中的编号(" + oldFlowNo + ")在系统中已经存在,流程名称为:" + dtFlow.Rows[0]["name"].ToString());
                    else
                    {
                        fl.No = oldFlowNo;
                        fl.DoDelData();
                        fl.DoDelete(); /*删除可能存在的垃圾.*/
                        fl.Insert();

                    }
                    break;
                case ImpFlowTempleteModel.OvrewaiteCurrFlowNo: /*覆盖当前的流程.*/
                    fl.No = oldFlowNo;
                    fl.DoDelData();
                    fl.DoDelete(); /*删除可能存在的垃圾.*/
                    fl.Insert();
                    break;
                case ImpFlowTempleteModel.AsSpecFlowNo:
                    if (SpecialFlowNo.Length <= 0)
                        throw new Exception("@您是按照指定的流程编号导入的，但是您没有传入正确的流程编号。");

                    fl.No = SpecialFlowNo.ToString();
                    fl.DoDelData();
                    fl.DoDelete(); /*删除可能存在的垃圾.*/
                    fl.Insert();
                    break;
                default:
                    throw new Exception("@没有判断");
            }
            #endregion 根据不同的流程模式，设置生成不同的流程编号.

            // string timeKey = fl.No;
            int idx = 0;
            string infoErr = "";
            string infoTable = "";
            int flowID = int.Parse(fl.No);

            #region 处理流程表数据
            foreach (DataColumn dc in dtFlow.Columns)
            {
                string val = dtFlow.Rows[0][dc.ColumnName] as string;
                switch (dc.ColumnName.ToLower())
                {
                    case "no":
                    case "fk_flowsort":
                        continue;
                    case "name":
                        // val = "复制:" + val + "_" + DateTime.Now.ToString("MM月dd日HH时mm分");
                        break;
                    default:
                        break;
                }
                fl.SetValByKey(dc.ColumnName, val);
            }
            fl.FK_FlowSort = fk_flowSort;
            if (DBAccess.IsExitsObject(fl.PTable) == true)
            {
                fl.PTable = null;
            }
            //修改成当前登陆人所在的组织
            fl.OrgNo = WebUser.OrgNo;
            fl.Update();
            //判断该流程是否是公文流程，存在BuessFields、FlowBuessType、FK_DocType=01
            Attrs attrs = fl.EnMap.Attrs;
            if (attrs.Contains("FlowBuessType") == true)
            {
                DBAccess.RunSQL("UPDATE WF_Flow Set BuessFields='" + fl.GetParaString("BuessFields") + "', FlowBuessType=" + fl.GetParaInt("FlowBuessType") + " ,FK_DocType='" + fl.GetParaString("FK_DocType") + "'");
            }

            #endregion 处理流程表数据

            #region 处理OID 插入重复的问题 Sys_GroupField, Sys_MapAttr.
            DataTable mydtGF = ds.Tables["Sys_GroupField"];
            DataTable myDTAttr = ds.Tables["Sys_MapAttr"];
            DataTable myDTAth = ds.Tables["Sys_FrmAttachment"];
            DataTable myDTDtl = ds.Tables["Sys_MapDtl"];
            DataTable myDFrm = ds.Tables["Sys_MapFrame"];

            //throw new Exception("@" + fl.No + fl.Name + ", 缺少：Sys_GroupField");
            foreach (DataRow dr in mydtGF.Rows)
            {
                Sys.GroupField gf = new Sys.GroupField();
                foreach (DataColumn dc in mydtGF.Columns)
                {
                    string val = dr[dc.ColumnName] as string;
                    gf.SetValByKey(dc.ColumnName, val);
                }
                int oldID = gf.OID;
                gf.OID = DBAccess.GenerOID();
                dr["OID"] = gf.OID; //给他一个新的OID.

                // 属性。
                if (myDTAttr != null && myDTAttr.Columns.Contains("GroupID"))
                {
                    foreach (DataRow dr1 in myDTAttr.Rows)
                    {
                        if (dr1["GroupID"] == null)
                            dr1["GroupID"] = 0;

                        if (dr1["GroupID"].ToString().Equals(oldID.ToString()))
                            dr1["GroupID"] = gf.OID;
                    }
                }

                // 附件。
                if (myDTAth != null && myDTAth.Columns.Contains("GroupID"))
                {
                    foreach (DataRow dr1 in myDTAth.Rows)
                    {
                        if (dr1["GroupID"] == null)
                            dr1["GroupID"] = 0;

                        if (dr1["GroupID"].ToString().Equals(oldID.ToString()))
                            dr1["GroupID"] = gf.OID;
                    }
                }

                if (myDTDtl != null && myDTDtl.Columns.Contains("GroupID"))
                {
                    // 从表。
                    foreach (DataRow dr1 in myDTDtl.Rows)
                    {
                        if (dr1["GroupID"] == null)
                            dr1["GroupID"] = 0;

                        if (dr1["GroupID"].ToString().Equals(oldID.ToString()))
                            dr1["GroupID"] = gf.OID;
                    }
                }

                // frm.
                if (myDFrm != null && myDFrm.Columns.Contains("GroupID"))
                {
                    foreach (DataRow dr1 in myDFrm.Rows)
                    {
                        if (dr1["GroupID"] == null)
                            dr1["GroupID"] = 0;

                        if (dr1["GroupID"].ToString().Equals(oldID.ToString()))
                            dr1["GroupID"] = gf.OID;
                    }
                }
            }
            #endregion 处理OID 插入重复的问题。 Sys_GroupField ， Sys_MapAttr.

            int timeKeyIdx = 0;
            foreach (DataTable dt in ds.Tables)
            {
                timeKeyIdx++;
                timeKey = timeKey + timeKeyIdx.ToString();

                infoTable = "@导入:" + dt.TableName + " 出现异常。";
                switch (dt.TableName)
                {
                    case "WF_Flow": //模版文件。
                        continue;
                    case "WF_NodeSubFlow": //延续子流程.
                        foreach (DataRow dr in dt.Rows)
                        {
                            SubFlowYanXu yg = new SubFlowYanXu();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "tonodeid":
                                    case "fk_node":
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_NodeSubFlow下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    default:
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                }
                                yg.SetValByKey(dc.ColumnName, val);
                            }
                            yg.Insert();
                        }
                        continue;
                    case "WF_FlowForm": //独立表单。 add 2013-12-03
                        //foreach (DataRow dr in dt.Rows)
                        //{
                        //    FlowForm cd = new FlowForm();
                        //    foreach (DataColumn dc in dt.Columns)
                        //    {
                        //        string val = dr[dc.ColumnName] as string;
                        //        if (val == null)
                        //            continue;
                        //        switch (dc.ColumnName.ToLower())
                        //        {
                        //            case "fk_flow":
                        //                val = fl.No;
                        //                break;
                        //            default:
                        //                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                        //                break;
                        //        }
                        //        cd.SetValByKey(dc.ColumnName, val);
                        //    }
                        //    cd.Insert();
                        //}
                        break;
                    case "WF_NodeForm": //节点表单权限。 2013-12-03
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeToolbar cd = new NodeToolbar();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "tonodeid":
                                    case "fk_node":
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_NodeForm下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    default:
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                }
                                cd.SetValByKey(dc.ColumnName, val);
                            }
                            cd.Insert();
                        }
                        break;
                    case "Sys_FrmSln": //表单字段权限。 2013-12-03
                        foreach (DataRow dr in dt.Rows)
                        {
                            FrmField cd = new FrmField();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "tonodeid":
                                    case "fk_node":
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点Sys_FrmSln下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    default:
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                }
                                cd.SetValByKey(dc.ColumnName, val);
                            }
                            cd.Insert();
                        }
                        break;
                    case "WF_NodeToolbar": //工具栏。
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeToolbar cd = new NodeToolbar();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "tonodeid":
                                    case "fk_node":
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_NodeToolbar下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    default:
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                }
                                cd.SetValByKey(dc.ColumnName, val);
                            }
                            cd.OID = DBAccess.GenerOID();
                            cd.DirectInsert();
                        }
                        break;
                    case "WF_BillTemplate":
                        continue; /*因为省掉了 打印模板的处理。*/
                        foreach (DataRow dr in dt.Rows)
                        {
                            BillTemplate bt = new BillTemplate();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_flow":
                                        val = flowID.ToString();
                                        break;
                                    case "nodeid":
                                    case "fk_node":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_BillTemplate下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                bt.SetValByKey(dc.ColumnName, val);
                            }
                            int i = 0;
                            string no = bt.No;
                            while (bt.IsExits)
                            {
                                bt.No = no + i.ToString();
                                i++;
                            }

                            try
                            {
                                File.Copy(info.DirectoryName + "\\" + no + ".rtf", SystemConfig.PathOfWebApp + @"\DataUser\CyclostyleFile\" + bt.No + ".rtf", true);
                            }
                            catch (Exception ex)
                            {
                                // infoErr += "@恢复单据模板时出现错误：" + ex.Message + ",有可能是您在复制流程模板时没有复制同目录下的单据模板文件。";
                            }
                            bt.Insert();
                        }
                        break;
                    case "WF_FrmNode": //Conds.xml。
                        DBAccess.RunSQL("DELETE FROM WF_FrmNode WHERE FK_Flow='" + fl.No + "'");
                        foreach (DataRow dr in dt.Rows)
                        {
                            FrmNode fn = new FrmNode();
                            fn.FK_Flow = fl.No;
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_FrmNode下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    default:
                                        break;
                                }
                                fn.SetValByKey(dc.ColumnName, val);
                            }
                            // 开始插入。
                            fn.MyPK = fn.FK_Frm + "_" + fn.FK_Node;
                            fn.Insert();
                        }
                        break;
                    case "WF_FindWorkerRole": //找人规则
                        foreach (DataRow dr in dt.Rows)
                        {
                            FindWorkerRole en = new FindWorkerRole();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_FindWorkerRole下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    default:
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                }
                                en.SetValByKey(dc.ColumnName, val);
                            }

                            //插入.
                            en.DirectInsert();
                        }
                        break;
                    case "WF_Cond": //Conds.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Cond cd = new Cond();
                            cd.FK_Flow = fl.No;
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "tonodeid":
                                    case "fk_node":
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_Cond下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    default:
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                }
                                cd.SetValByKey(dc.ColumnName, val);
                            }

                            cd.FK_Flow = fl.No;

                            //  return this.FK_MainNode + "_" + this.ToNodeID + "_" + this.HisCondType.ToString() + "_" + ConnDataFrom.Stas.ToString();
                            // ，开始插入。 
                            if (cd.MyPK.Contains("Stas"))
                            {
                                cd.MyPK = cd.FK_Node + "_" + cd.ToNodeID + "_" + cd.CondType.ToString() + "_" + ConnDataFrom.Stas.ToString();
                            }
                            else if (cd.MyPK.Contains("Dept"))
                            {
                                cd.MyPK = cd.FK_Node + "_" + cd.ToNodeID + "_" + cd.CondType.ToString() + "_" + ConnDataFrom.Depts.ToString();
                            }
                            else if (cd.MyPK.Contains("Paras"))
                            {
                                cd.MyPK = cd.FK_Node + "_" + cd.ToNodeID + "_" + cd.CondType.ToString() + "_" + ConnDataFrom.Paras.ToString();
                            }
                            else if (cd.MyPK.Contains("Url"))
                            {
                                cd.MyPK = cd.FK_Node + "_" + cd.ToNodeID + "_" + cd.CondType.ToString() + "_" + ConnDataFrom.Url.ToString();
                            }
                            else if (cd.MyPK.Contains("SQL"))
                            {
                                cd.MyPK = cd.FK_Node + "_" + cd.ToNodeID + "_" + cd.CondType.ToString() + "_" + ConnDataFrom.SQL;
                            }
                            else
                            {
                                cd.MyPK = DBAccess.GenerOID().ToString() + DateTime.Now.ToString("yyMMddHHmmss");
                            }
                            cd.DirectInsert();
                        }
                        break;
                    case "WF_CCDept"://抄送到部门。
                        foreach (DataRow dr in dt.Rows)
                        {
                            CCDept cd = new CCDept();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_CCDept下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                cd.SetValByKey(dc.ColumnName, val);
                            }

                            //开始插入。
                            try
                            {
                                cd.Insert();
                            }
                            catch
                            {
                                cd.Update();
                            }
                        }
                        break;
                    case "WF_NodeReturn"://可退回的节点。
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeReturn cd = new NodeReturn();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                    case "returnto":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_NodeReturn下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                cd.SetValByKey(dc.ColumnName, val);
                            }

                            //开始插入。
                            try
                            {
                                cd.Insert();
                            }
                            catch
                            {
                                cd.Update();
                            }
                        }
                        break;
                    case "WF_Direction": //方向。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Direction dir = new Direction();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "node":
                                    case "tonode":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_Direction下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                dir.SetValByKey(dc.ColumnName, val);
                            }
                            dir.FK_Flow = fl.No;
                            dir.Insert();
                        }
                        break;
                    case "WF_LabNote": //LabNotes.xml。
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            LabNote ln = new LabNote();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                ln.SetValByKey(dc.ColumnName, val);
                            }
                            idx++;
                            ln.FK_Flow = fl.No;
                            ln.MyPK = ln.FK_Flow + "_" + ln.X + "_" + ln.Y + "_" + idx;
                            ln.DirectInsert();
                        }
                        break;
                    case "WF_NodeDept": //FAppSets.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeDept dp = new NodeDept();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_NodeDept下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                dp.SetValByKey(dc.ColumnName, val);
                            }
                            try
                            {
                                //如果部门不属于本组织的，就要删除.  
                                if (Glo.CCBPMRunModel != CCBPMRunModel.Single)
                                {
                                    BP.WF.Port.Admin2.Dept dept = new Port.Admin2.Dept(dp.FK_Dept);
                                    if (dept.OrgNo.Equals(WebUser.OrgNo) == false)
                                        continue;
                                }
                                dp.Insert();
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        break;
                    case "WF_Node": //导入节点信息.
                        foreach (DataRow dr in dt.Rows)
                        {
                            BP.WF.Template.NodeExt nd = new BP.WF.Template.NodeExt();
                            BP.WF.Template.CC cc = new CC(); // 抄送相关的信息.
                            //cc.CheckPhysicsTable();
                            BP.WF.Template.NodeWorkCheck fwc = new NodeWorkCheck();

                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_Node下nodeid值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                    case "fk_flowsort":
                                        continue;
                                    case "showsheets":
                                    case "histonds":
                                    case "groupstands": //去除不必要的替换
                                        string key = "@" + flowID;
                                        val = val.Replace(key, "@");
                                        break;
                                    default:
                                        break;
                                }
                                nd.SetValByKey(dc.ColumnName, val);
                                cc.SetValByKey(dc.ColumnName, val);
                                fwc.SetValByKey(dc.ColumnName, val);

                            }

                            nd.FK_Flow = fl.No;
                            nd.FlowName = fl.Name;
                            try
                            {

                                if (nd.EnMap.Attrs.Contains("OfficePrintEnable"))
                                {
                                    if (nd.GetValStringByKey("OfficePrintEnable") == "打印")
                                        nd.SetValByKey("OfficePrintEnable", 0);
                                }

                                nd.DirectInsert();

                                //把抄送的信息也导入里面去.
                                cc.DirectUpdate();
                                fwc.FWCVer = 1;  //设置为2019版本. 2018版是1个节点1个人,仅仅显示1个意见.
                                fwc.DirectUpdate();
                                DBAccess.RunSQL("DELETE FROM Sys_MapAttr WHERE FK_MapData='ND" + nd.NodeID + "'");
                            }
                            catch (Exception ex)
                            {
                                cc.CheckPhysicsTable();
                                fwc.CheckPhysicsTable();

                                throw new Exception("@导入节点:FlowName:" + nd.FlowName + " nodeID: " + nd.NodeID + " , " + nd.Name + " 错误:" + ex.Message);
                            }
                            //删除mapdata.
                        }

                        // 执行update 触发其他的业务逻辑。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Node nd = new Node();
                            nd.NodeID = int.Parse(dr[NodeAttr.NodeID].ToString());
                            nd.RetrieveFromDBSources();
                            nd.FK_Flow = fl.No;
                            //获取表单类别
                            string formType = dr[NodeAttr.FormType].ToString();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "nodefrmid":
                                        //绑定表单库的表单11不需要替换表单编号
                                        if (formType.Equals("11") == false)
                                        {
                                            int iFormTypeLength = iOldFlowLength + 2;
                                            if (val.Length > iFormTypeLength)
                                            {
                                                val = "ND" + flowID + val.Substring(iFormTypeLength);
                                            }
                                        }
                                        break;
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_Node下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                    case "fk_flowsort":
                                        continue;
                                    case "showsheets":
                                    case "histonds":
                                    case "groupstands": //修复替换 
                                        string key = "@" + flowID;
                                        val = val.Replace(key, "@");
                                        break;
                                    default:
                                        break;
                                }
                                nd.SetValByKey(dc.ColumnName, val);
                            }
                            nd.FK_Flow = fl.No;
                            nd.FlowName = fl.Name;
                            nd.DirectUpdate();
                        }
                        break;
                    case "WF_NodeExt":
                        foreach (DataRow dr in dt.Rows)
                        {
                            BP.WF.Template.NodeExt nd = new BP.WF.Template.NodeExt();
                            nd.NodeID = int.Parse(flowID + dr[NodeAttr.NodeID].ToString().Substring(iOldFlowLength));
                            nd.RetrieveFromDBSources();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_Node下nodeid值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                    case "fk_flowsort":
                                        continue;
                                    case "showsheets":
                                    case "histonds":
                                    case "groupstands": //去除不必要的替换
                                        string key = "@" + flowID;
                                        val = val.Replace(key, "@");
                                        break;
                                    default:
                                        break;
                                }
                                nd.SetValByKey(dc.ColumnName, val);
                            }

                            nd.DirectUpdate();
                        }
                        break;
                    case "WF_Selector":
                        foreach (DataRow dr in dt.Rows)
                        {
                            Selector selector = new Selector();
                            foreach (DataColumn dc in dt.Columns)
                            {

                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                if (dc.ColumnName.ToLower().Equals("nodeid"))
                                {
                                    if (val.Length < iOldFlowLength)
                                    {
                                        // 节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                        throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_Node下FK_Node值错误:" + val);
                                    }
                                    val = flowID + val.Substring(iOldFlowLength);
                                }

                                selector.SetValByKey(dc.ColumnName, val);
                            }
                            selector.DirectUpdate();
                        }
                        break;
                    case "WF_NodeStation": //FAppSets.xml。
                        DBAccess.RunSQL("DELETE FROM WF_NodeStation WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + fl.No + "')");
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeStation ns = new NodeStation();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_NodeStation下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                ns.SetValByKey(dc.ColumnName, val);
                            }
                            ns.Insert();
                        }
                        break;
                    case "Sys_Enum": //RptEmps.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.SysEnum se = new Sys.SysEnum();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        break;
                                    default:
                                        break;
                                }
                                se.SetValByKey(dc.ColumnName, val);
                            }
                            se.MyPK = se.EnumKey + "_" + se.Lang + "_" + se.IntKey;
                            if (se.IsExits)
                                continue;
                            se.Insert();
                        }
                        break;
                    case "Sys_EnumMain": //RptEmps.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.SysEnumMain sem = new Sys.SysEnumMain();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                sem.SetValByKey(dc.ColumnName, val);
                            }
                            if (sem.IsExits)
                                continue;
                            sem.Insert();
                        }
                        break;
                    case "Sys_MapAttr": //RptEmps.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.MapAttr ma = new Sys.MapAttr();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_mapdata":
                                    case "keyofen":
                                    case "autofulldoc":
                                        if (val == null)
                                            continue;
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                    default:
                                        break;
                                }
                                ma.SetValByKey(dc.ColumnName, val);
                            }
                            bool b = ma.IsExit(Sys.MapAttrAttr.FK_MapData, ma.FK_MapData,
                                Sys.MapAttrAttr.KeyOfEn, ma.KeyOfEn);

                            ma.MyPK = ma.FK_MapData + "_" + ma.KeyOfEn;
                            if (b == true)
                                ma.DirectUpdate();
                            else
                                ma.DirectInsert();
                        }
                        break;
                    case "Sys_MapData": //RptEmps.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.MapData md = new Sys.MapData();
                            string htmlCode = "";
                            foreach (DataColumn dc in dt.Columns)
                            {
                                if (dc.ColumnName == "HtmlTemplateFile")
                                {
                                    htmlCode = dr[dc.ColumnName] as string;
                                    continue;
                                }

                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + int.Parse(fl.No));
                                md.SetValByKey(dc.ColumnName, val);
                            }
                            md.Save();

                            //如果是开发者表单，赋值HtmlTemplateFile数据库的值并保存到DataUser下
                            if (md.HisFrmType == FrmType.Develop)
                            {
                                //string htmlCode = DBAccess.GetBigTextFromDB("Sys_MapData", "No", "ND" + oldFlowID, "HtmlTemplateFile");
                                if (DataType.IsNullOrEmpty(htmlCode) == false)
                                {
                                    htmlCode = htmlCode.Replace("ND" + oldFlowID, "ND" + int.Parse(fl.No));
                                    //保存到数据库，存储html文件
                                    //保存到DataUser/CCForm/HtmlTemplateFile/文件夹下
                                    string filePath = SystemConfig.PathOfDataUser + "CCForm\\HtmlTemplateFile\\";
                                    if (Directory.Exists(filePath) == false)
                                        Directory.CreateDirectory(filePath);
                                    filePath = filePath + md.No + ".htm";
                                    //写入到html 中
                                    DataType.WriteFile(filePath, htmlCode);
                                    // HtmlTemplateFile 保存到数据库中
                                    DBAccess.SaveBigTextToDB(htmlCode, "Sys_MapData", "No", md.No, "HtmlTemplateFile");
                                }
                                else
                                {
                                    //如果htmlCode是空的需要删除当前节点的html文件
                                    string filePath = SystemConfig.PathOfDataUser + "CCForm\\HtmlTemplateFile\\" + md.No + ".htm";
                                    if (File.Exists(filePath) == true)
                                        File.Delete(filePath);
                                    DBAccess.SaveBigTextToDB("", "Sys_MapData", "No", md.No, "HtmlTemplateFile");
                                }
                            }
                        }

                        break;
                    case "Sys_MapDtl": //RptEmps.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.MapDtl md = new Sys.MapDtl();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                md.SetValByKey(dc.ColumnName, val);
                            }
                            md.Save();
                            md.IntMapAttrs(); //初始化他的字段属性.
                        }
                        break;
                    case "Sys_MapExt":
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.MapExt md = new Sys.MapExt();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                md.SetValByKey(dc.ColumnName, val);
                            }

                            //调整他的PK.
                            //md.InitPK();
                            md.Save(); //执行保存.
                        }
                        break;
                    case "Sys_FrmLine":
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmLine en = new FrmLine();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }

                            en.MyPK = Guid.NewGuid().ToString();
                            // DBAccess.GenerOIDByGUID(); "LIE" + timeKey + "_" + idx;
                            //if (en.IsExitGenerPK())
                            //    continue;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmImg":
                        idx = 0;
                        timeKey = DateTime.Now.ToString("yyyyMMddHHmmss");
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmImg en = new FrmImg();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }

                            en.MyPK = Guid.NewGuid().ToString();
                        }
                        break;
                    case "Sys_FrmLab":
                        idx = 0;
                        timeKey = DateTime.Now.ToString("yyyyMMddHHmmss");
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmLab en = new FrmLab();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }

                            en.MyPK = DBAccess.GenerGUID(); // "Lab" + timeKey + "_" + idx;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmLink":
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmLink en = new FrmLink();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val);
                            }
                            en.MyPK = Guid.NewGuid().ToString();
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmAttachment":
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmAttachment en = new FrmAttachment();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }

                            en.MyPK = en.FK_MapData + "_" + en.NoOfObj;
                            en.Save();
                        }
                        break;
                    case "Sys_FrmEvent": //事件.
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmEvent en = new FrmEvent();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }

                            //解决保存错误问题. 
                            try
                            {
                                en.Insert();
                            }
                            catch
                            {
                                en.Update();
                            }
                        }
                        break;
                    case "Sys_FrmRB": //Sys_FrmRB.
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmRB en = new FrmRB();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }
                            en.Insert();
                        }
                        break;
                    case "Sys_MapFrame":
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            MapFrame en = new MapFrame();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;


                                en.SetValByKey(dc.ColumnName, val.ToString().Replace("ND" + oldFlowID, "ND" + flowID));
                            }
                            en.DirectInsert();
                        }
                        break;
                    case "WF_NodeEmp": //FAppSets.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeEmp ne = new NodeEmp();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_NodeEmp下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                ne.SetValByKey(dc.ColumnName, val);
                            }
                            ne.Insert();
                        }
                        break;
                    case "Sys_GroupField": //这里需要对比一下翻译.
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.GroupField gf = new Sys.GroupField();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "enname":
                                    case "keyofen":
                                    case "ctrlid": //升级傻瓜表单的时候,新增加的字段 add by zhoupeng 2016.11.21
                                    case "frmid": //升级傻瓜表单的时候,新增加的字段 add by zhoupeng 2016.11.21
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                    default:
                                        break;
                                }
                                gf.SetValByKey(dc.ColumnName, val);
                            }
                            gf.InsertAsOID(gf.OID);

                            /*
                            string sql = "select * from Sys_GroupField where CtrlID = '" + gf.CtrlID + "' AND FrmID='" + gf.FrmID + "'";
                            int count = DBAccess.RunSQLReturnCOUNT(sql);
                            if (count > 0)
                            {
                                DBAccess.RunSQL("delete from Sys_GroupField where CtrlID = '" + gf.CtrlID + "' AND FrmID='" + gf.FrmID + "'");
                            }
                            int oid = DBAccess.GenerOID();
                            DBAccess.RunSQL("UPDATE Sys_MapAttr SET GroupID='" + oid + "' WHERE FK_MapData='" + gf.FrmID + "' AND GroupID='" + gf.OID + "'");
                            gf.InsertAsOID(oid); */
                        }
                        break;
                    case "WF_CCEmp": // 抄送.
                        foreach (DataRow dr in dt.Rows)
                        {
                            CCEmp ne = new CCEmp();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_CCEmp下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                ne.SetValByKey(dc.ColumnName, val);
                            }
                            ne.Insert();
                        }
                        break;
                    case "WF_CCStation": // 抄送.
                        string mysql = " DELETE FROM WF_CCStation WHERE   FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + flowID + "')";
                        DBAccess.RunSQL(mysql);
                        foreach (DataRow dr in dt.Rows)
                        {
                            CCStation ne = new CCStation();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_CCStation下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                ne.SetValByKey(dc.ColumnName, val);
                            }
                            ne.Save();
                        }
                        break;
                    default:
                        // infoErr += "Error:" + dt.TableName;
                        break;
                        //    throw new Exception("@unhandle named " + dt.TableName);
                }
            }

            #region 处理数据完整性。

           
            //DBAccess.RunSQL("UPDATE WF_Cond SET FK_Node=NodeID WHERE FK_Node=0");
            //DBAccess.RunSQL("UPDATE WF_Cond SET ToNodeID=NodeID WHERE ToNodeID=0");
            // DBAccess.RunSQL("DELETE FROM WF_Cond WHERE NodeID NOT IN (SELECT NodeID FROM WF_Node)");
            //  DBAccess.RunSQL("DELETE FROM WF_Cond WHERE ToNodeID NOT IN (SELECT NodeID FROM WF_Node) ");
            // DBAccess.RunSQL("DELETE FROM WF_Cond WHERE FK_Node NOT IN (SELECT NodeID FROM WF_Node) AND FK_Node > 0");

            //处理分组错误.
            Nodes nds = new Nodes(fl.No);
            foreach (Node nd in nds)
            {
                MapFrmFool cols = new MapFrmFool("ND" + nd.NodeID);
                cols.DoCheckFixFrmForUpdateVer();
            }
            #endregion

            //处理OrgNo 的导入问题.
            if (Glo.CCBPMRunModel != CCBPMRunModel.GroupInc)
            {
                fl.RetrieveFromDBSources();
                fl.OrgNo = BP.Web.WebUser.OrgNo;
                fl.DirectUpdate();
            }

            if (infoErr == "")
            {
                infoTable = "";
                return fl; // "完全成功。";
            }

            infoErr = "@执行期间出现如下非致命的错误：\t\r" + infoErr + "@ " + infoTable;
            throw new Exception(infoErr);
        }

        /// <summary>
        /// 检查报表
        /// </summary>
        public void CheckRpt()
        {
            this.DoCheck_CheckRpt(this.HisNodes);
        }
        protected override bool beforeInsert()
        {
            //删除数据库中的垃圾数据
            DoDelData();
            if (Glo.CCBPMRunModel != CCBPMRunModel.Single)
                this.OrgNo = WebUser.OrgNo;

            return base.beforeInsert();
        }
        /// <summary>
        /// 更新之前做检查
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdate()
        {
            //删除缓存数据.
            this.ClearAutoNumCash(false);

            this.Ver = DataType.CurrentDataTimess;
            return base.beforeUpdate();
        }
        /// <summary>
        /// 更新版本号
        /// </summary>
        public static void UpdateVer(string flowNo)
        {
            string sql = "UPDATE WF_Flow SET Ver='" + DataType.CurrentDataTimess + "' WHERE No='" + flowNo + "'";
            DBAccess.RunSQL(sql);
        }
        /// <summary>
        /// 删除功能.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeDelete()
        {
            // throw new Exception("err@请反馈给我们，非法的删除操作。 ");
            return base.beforeDelete();
        }
        public string DoDelete()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                throw new Exception("err@流程没有初始化，删除错误.");

            //如果是广西计算中心，不知道为什么删除了.
            if (SystemConfig.CustomerNo.Equals("GXJSZX") == true)
                throw new Exception("err@目前暂时不支持[DoDelete - " + this.No + "]请立即联系管理员.");

            //检查流程有没有版本管理？
            if (this.FK_FlowSort.Length > 1)
            {
                string str = "SELECT * FROM WF_Flow WHERE PTable='" + this.PTable + "' AND FK_FlowSort='' ";
                DataTable dt = DBAccess.RunSQLReturnTable(str);
                if (dt.Rows.Count >= 1)
                    return "err@删除流程出错，该流程下有[" + dt.Rows.Count + "]个子版本您不能删除。";
            }
            string sql = "";
            //sql = " DELETE FROM WF_chofflow WHERE FK_Flow='" + this.No + "'";
            sql += "@ DELETE FROM WF_GenerWorkerlist WHERE FK_Flow='" + this.No + "'";
            sql += "@ DELETE FROM  WF_GenerWorkFlow WHERE FK_Flow='" + this.No + "'";
            sql += "@ DELETE FROM  WF_Cond WHERE RefFlowNo='" + this.No + "'";

            //删除消息配置.
            sql += "@ DELETE FROM WF_PushMsg WHERE FK_Flow='" + this.No + "'";

            // 删除岗位节点。
            sql += "@ DELETE FROM WF_NodeStation WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            // 删除方向。
            sql += "@ DELETE FROM WF_Direction WHERE FK_Flow='" + this.No + "'";

            //删除节点绑定信息.
            sql += "@ DELETE FROM WF_FrmNode WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            sql += "@ DELETE FROM WF_NodeEmp  WHERE   FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            sql += "@ DELETE FROM WF_CCEmp WHERE   FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            sql += "@ DELETE FROM WF_CCDept WHERE   FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            sql += "@ DELETE FROM WF_CCStation WHERE   FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            sql += "@ DELETE FROM WF_NodeReturn WHERE   FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            sql += "@ DELETE FROM WF_NodeDept WHERE   FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            sql += "@ DELETE FROM WF_NodeStation WHERE   FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            sql += "@ DELETE FROM WF_NodeEmp WHERE   FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            sql += "@ DELETE FROM WF_NodeToolbar WHERE   FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            sql += "@ DELETE FROM WF_SelectAccper WHERE   FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            //sql += "@ DELETE FROM WF_TurnTo WHERE   FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            //删除侦听.
            // sql += "@ DELETE FROM WF_Listen WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            // 删除d2d数据.
            //  sql += "@GO DELETE WF_M2M WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            //// 删除配置.
            //sql += "@ DELETE FROM WF_FAppSet WHERE NodeID IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";


            //// 外部程序设置
            //sql += "@ DELETE FROM WF_FAppSet WHERE  NodeID in (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            //删除单据.
            sql += "@ DELETE FROM WF_BillTemplate WHERE  NodeID in (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            //删除权限控制.
            sql += "@ DELETE FROM Sys_FrmSln WHERE FK_Flow='" + this.No + "'";
            //考核表
            sql += "@ DELETE FROM WF_CH WHERE FK_Flow='" + this.No + "'";
            //删除抄送
            sql += "@ DELETE FROM WF_CCList WHERE FK_Flow='" + this.No + "'";
            Nodes nds = new Nodes(this.No);
            foreach (Node nd in nds)
            {
                // 删除节点所有相关的东西.
                nd.Delete();
            }

            sql += "@ DELETE  FROM WF_Node WHERE FK_Flow='" + this.No + "'";
            sql += "@ DELETE  FROM WF_LabNote WHERE FK_Flow='" + this.No + "'";

            //删除分组信息
            sql += "@ DELETE FROM Sys_GroupField WHERE FrmID NOT IN(SELECT NO FROM Sys_MapData)";

            #region 删除流程报表,删除轨迹
            MapData md = new MapData();
            md.No = "ND" + int.Parse(this.No) + "Rpt";
            md.Delete();

            //删除视图.
            if (DBAccess.IsExitsObject("V_" + this.No))
                DBAccess.RunSQL("DROP VIEW V_" + this.No);

            //删除轨迹.
            if (DBAccess.IsExitsObject("ND" + int.Parse(this.No) + "Track") == true)
                DBAccess.RunSQL("DROP TABLE ND" + int.Parse(this.No) + "Track ");

            #endregion 删除流程报表,删除轨迹.

            // 执行录制的sql scripts.
            DBAccess.RunSQLs(sql);

            //清空WF_Emp中的StartFlow 
            DBAccess.RunSQL("UPDATE  WF_Emp Set StartFlows =''");

            //删除数据的接口.
            DoDelData();

            this.Delete(); //删除需要移除缓存.

            return "执行成功.";
        }

        /// <summary>
        /// 向上移动
        /// </summary>
        public void DoUp()
        {
            this.DoOrderUp(FlowAttr.FK_FlowSort, this.FK_FlowSort, FlowAttr.Idx);
        }
        /// <summary>
        /// 向下移动
        /// </summary>
        public void DoDown()
        {
            this.DoOrderDown(FlowAttr.FK_FlowSort, this.FK_FlowSort, FlowAttr.Idx);
        }
        #endregion

        #region 版本管理.
        /// <summary>
        /// 创建新版本.
        /// </summary>
        /// <returns></returns>
        public string VerCreateNew()
        {
            try
            {
                //生成模版.
                string file = GenerFlowXmlTemplete();

                Flow newFlow = Flow.DoLoadFlowTemplate(this.FK_FlowSort, file, ImpFlowTempleteModel.AsNewFlow);

                newFlow.PTable = this.PTable;
                newFlow.FK_FlowSort = ""; //不能显示流程树上.
                newFlow.Name = this.Name;
                newFlow.Ver = DataType.CurrentDataTime;
                newFlow.IsCanStart = false; //不能发起.
                newFlow.DirectUpdate();
                return newFlow.No;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }

            //DataSet ds = this.GenerFlowXmlTemplete();
            //return "001";
        }
        /// <summary>
        /// 设置当前的版本为新版本.
        /// </summary>
        /// <returns></returns>
        public string VerSetCurrentVer()
        {
            string sql = "SELECT FK_FlowSort,No FROM WF_Flow WHERE PTable='" + this.PTable + "' AND FK_FlowSort!='' ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                return "err@没有找到主版本,请联系管理员.";
            string flowSort = dt.Rows[0][0].ToString();
            string oldFlowNo = dt.Rows[0][1].ToString();
            sql = "UPDATE WF_Flow SET FK_FlowSort='',IsCanStart=0 WHERE PTable='" + this.PTable + "' ";
            DBAccess.RunSQL(sql);

            sql = "UPDATE WF_Flow SET FK_FlowSort='" + flowSort + "', IsCanStart=1 WHERE No='" + this.No + "' ";
            DBAccess.RunSQL(sql);

            //清缓存
            Cash2019.DeleteRow("BP.WF.Flow", oldFlowNo);
            Cash2019.DeleteRow("BP.WF.Flow", this.No);
            Flow flow = new Flow(oldFlowNo);
            flow = new Flow(this.No);


            return "info@设置成功.";
        }
        /// <summary>
        /// 获得版本列表.
        /// </summary>
        /// <returns></returns>
        public string VerGenerVerList()
        {
            //if (this.FK_FlowSort.Equals("") == true)
            //    return "err@当前版本为分支版本，您无法管理，只有主版本才能管理。";

            DataTable dt = new DataTable();
            dt.Columns.Add("Ver");  //版本号
            dt.Columns.Add("No");    //内部编号
            dt.Columns.Add("Name");  //流程名称.
            dt.Columns.Add("IsRel"); //是否发布？
            dt.Columns.Add("NumOfRuning"); //运行中的流程.
            dt.Columns.Add("NumOfOK"); //运行完毕的流程.

            //如果业务表是空的，就更新它.
            string ptable = this.GetValStringByKey(FlowAttr.PTable);
            if (DataType.IsNullOrEmpty(ptable) == true)
            {
                this.SetValByKey(FlowAttr.PTable, this.PTable);
                this.DirectUpdate();
            }

            //获得所有的版本.
            // string sql = "SELECT No,Name,Ver,FK_FlowSort FROM WF_Flow WHERE PTable='"+this.PTable+"'";
            string sql = "SELECT No  FROM WF_Flow WHERE PTable='" + this.PTable + "'";
            Flows fls = new Flows();
            fls.RetrieveInSQL(sql);

            foreach (Flow item in fls)
            {
                DataRow dr = dt.NewRow();
                dr["Ver"] = item.Ver;
                dr["No"] = item.No;
                dr["Name"] = item.Name;

                if (DataType.IsNullOrEmpty(item.FK_FlowSort) == true)
                    dr["IsRel"] = "0";
                else
                    dr["IsRel"] = "1";

                dr["NumOfRuning"] = DBAccess.RunSQLReturnValInt("SELECT COUNT(WORKID) FROM WF_GenerWorkFlow WHERE FK_FLOW='" + item.No + "' AND WFState=2");
                dr["NumOfOK"] = DBAccess.RunSQLReturnValInt("SELECT COUNT(WORKID) FROM WF_GenerWorkFlow WHERE FK_FLOW='" + item.No + "' AND WFState=3");
                dt.Rows.Add(dr);
            }

            return BP.Tools.Json.ToJson(dt);
        }
        #endregion 版本管理.

    }
    /// <summary>
    /// 流程集合
    /// </summary>
    public class Flows : EntitiesNoName
    {
        #region 查询
        public static void GenerHtmlRpts()
        {
            Flows fls = new Flows();
            fls.RetrieveAll();

            foreach (Flow fl in fls)
            {
                fl.DoCheck();
                fl.GenerFlowXmlTemplete();
            }

            // 生成索引界面
            string path = SystemConfig.PathOfWorkDir + @"\VisualFlow\DataUser\FlowDesc\";
            string msg = "";
            msg += "<html>";
            msg += "\r\n<title>.net工作流程引擎设计，流程模板</title>";

            msg += "\r\n<body>";

            msg += "\r\n<h1>驰骋流程模板网</h1> <br><a href=index.htm >返回首页</a> - <a href='http://ccFlow.org' >访问驰骋工作流程管理系统，工作流引擎官方网站</a> 流程系统建设请联系:QQ:793719823,Tel:18660153393<hr>";

            foreach (Flow fl in fls)
            {
                msg += "\r\n <h3><b><a href='./" + fl.No + "/index.htm' target=_blank>" + fl.Name + "</a></b> - <a href='" + fl.No + ".gif' target=_blank  >" + fl.Name + "流程图</a></h3>";

                msg += "\r\n<UL>";
                Nodes nds = fl.HisNodes;
                foreach (Node nd in nds)
                {
                    msg += "\r\n<li><a href='./" + fl.No + "/" + nd.NodeID + "_" + nd.FlowName + "_" + nd.Name + "表单.doc' target=_blank>步骤" + nd.Step + ", - " + nd.Name + "模板</a> -<a href='./" + fl.No + "/" + nd.NodeID + "_" + nd.Name + "_表单模板.htm' target=_blank>Html版</a></li>";
                }
                msg += "\r\n</UL>";
            }
            msg += "\r\n</body>";
            msg += "\r\n</html>";

            try
            {
                string pathDef = SystemConfig.PathOfWorkDir + "\\VisualFlow\\DataUser\\FlowDesc\\" + SystemConfig.CustomerNo + "_index.htm";
                DataType.WriteFile(pathDef, msg);

                pathDef = SystemConfig.PathOfWorkDir + "\\VisualFlow\\DataUser\\FlowDesc\\index.htm";
                DataType.WriteFile(pathDef, msg);
                System.Diagnostics.Process.Start(SystemConfig.PathOfWorkDir + "\\VisualFlow\\DataUser\\FlowDesc\\");
            }
            catch
            {
            }
        }
        #endregion 查询

        #region 查询
        /// <summary>
        /// 查出来全部的自动流程
        /// </summary>
        public void RetrieveIsAutoWorkFlow()
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FlowAttr.FlowType, 1);
            qo.addOrderBy(FlowAttr.No);
            qo.DoQuery();
        }
        /// <summary>
        /// 查询出来全部的在生存期间内的流程
        /// </summary>
        /// <param name="flowSort">流程类别</param>
        /// <param name="IsCountInLifeCycle">是不是计算在生存期间内 true 查询出来全部的 </param>
        public void Retrieve(string flowSort)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FlowAttr.FK_FlowSort, flowSort);
            qo.addOrderBy(FlowAttr.No);
            qo.DoQuery();
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 工作流程
        /// </summary>
        public Flows() { }
        /// <summary>
        /// 工作流程
        /// </summary>
        /// <param name="fk_sort"></param>
        public Flows(string fk_sort)
        {
            this.Retrieve(FlowAttr.FK_FlowSort, fk_sort);
        }
        #endregion

        #region 得到实体
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Flow();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Flow> ToJavaList()
        {
            return (System.Collections.Generic.IList<Flow>)this;
        }
        /// <summary>
        /// 转化成 list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Flow> Tolist()
        {
            System.Collections.Generic.List<Flow> list = new System.Collections.Generic.List<Flow>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Flow)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

