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
        #endregion 参数属性.

        #region 业务数据表同步属性.
        /// <summary>
        /// 同步方式
        /// </summary>
        public FlowDTSWay DTSWay
        {
            get
            {
                return (FlowDTSWay)this.GetValIntByKey(FlowAttr.DTSWay);
            }
            set
            {
                this.SetValByKey(FlowAttr.DTSWay, (int)value);
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
        public DTSField DTSField
        {
            get
            {
                return (DTSField)this.GetValIntByKey(FlowAttr.DTSField);
            }
            set
            {
                this.SetValByKey(FlowAttr.DTSField, (int)value);
            }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public string DTSDBSrc
        {
            get
            {
                string str = this.GetValStringByKey(FlowAttr.DTSDBSrc);
                if (string.IsNullOrEmpty(str))
                    return "local";
                return str;
            }
            set
            {
                this.SetValByKey(FlowAttr.DTSDBSrc, value);
            }
        }
        /// <summary>
        /// 业务表
        /// </summary>
        public string DTSBTable
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DTSBTable);
            }
            set
            {
                this.SetValByKey(FlowAttr.DTSBTable, value);
            }
        }
        public string DTSBTablePK
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DTSBTablePK);
            }
            set
            {
                this.SetValByKey(FlowAttr.DTSBTablePK, value);
            }
        }
        /// <summary>
        /// 要同步的节点s
        /// </summary>
        public string DTSSpecNodes
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DTSSpecNodes);
            }
            set
            {
                this.SetValByKey(FlowAttr.DTSSpecNodes, value);
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
        ///// <summary>
        ///// 发起限制文本
        ///// </summary>
        //public string StartLimitRoleText
        //{
        //    get
        //    {
        //        return  this.GetValRefTextByKey(FlowAttr.StartLimitRole);
        //    }
        //}
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
                str= str.Replace("~", "'");
                if (string.IsNullOrEmpty(str) == null)
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
        public TimelineRole HisTimelineRole
        {
            get
            {
                return (TimelineRole)this.GetValIntByKey(FlowAttr.TimelineRole);
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
        /// 延续流程方式
        /// </summary>
        public CFlowWay CFlowWay
        {
            get
            {
                return (CFlowWay)this.GetValIntByKey(FlowAttr.CFlowWay);
            }
            set
            {
                this.SetValByKey(FlowAttr.CFlowWay, (int)value);
            }
        }
        /// <summary>
        /// 延续流程参数。
        /// </summary>
        public string CFlowPara
        {
            get
            {
                return this.GetValStrByKey(FlowAttr.CFlowPara);
            }
            set
            {
                this.SetValByKey(FlowAttr.CFlowPara, value);
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
                throw new Exception("@您违反了该流程的【" + this.StartLimitRole + "】限制规则。" + this.StartLimitAlert);

            //如果是bs系统.
            if (paras == null)
                paras = new Hashtable();
            if (BP.Sys.SystemConfig.IsBSsystem == true)
            {
                foreach (string k in BP.Sys.Glo.Request.QueryString.AllKeys)
                {
                    if (paras.ContainsKey(k))
                        //  continue;
                        paras[k] = BP.Sys.Glo.Request.QueryString[k];
                    else
                        paras.Add(k, BP.Sys.Glo.Request.QueryString[k]);
                }
            }

            //开始节点.
            BP.WF.Node nd = new BP.WF.Node(this.StartNodeID);

            //从草稿里看看是否有新工作？
            StartWork wk = (StartWork)nd.HisWork;
            wk.ResetDefaultVal();

            string dbstr = SystemConfig.AppCenterDBVarStr;

            Paras ps = new Paras();
            GERpt rpt = this.HisGERpt;

            //是否新创建的WorKID
            bool IsNewWorkID = false;
            /*如果要启用草稿,就创建一个新的WorkID .*/
            if (this.DraftRole != Template.DraftRole.None && nd.IsStartNode)
                IsNewWorkID = true;

            try
            {
                //从报表里查询该数据是否存在？
                if (this.IsGuestFlow == true && string.IsNullOrEmpty(GuestUser.No) == false)
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
                    ps.SQL = "SELECT OID,FlowEndNode FROM " + this.PTable + " WHERE FlowStarter=" + dbstr + "FlowStarter AND WFState=" + dbstr + "WFState ";
                    ps.Add(GERptAttr.FlowStarter, emp.No);
                    ps.Add(GERptAttr.WFState, (int)WFState.Blank);
                    // throw new Exception(ps.SQL);
                    DataTable dt = DBAccess.RunSQLReturnTable(ps);
                    //如果没有启用草稿，并且存在草稿就取第一条 by dgq 5.28
                    if (dt.Rows.Count > 0 && IsNewWorkID == false)
                    {
                        wk.OID = Int64.Parse(dt.Rows[0][0].ToString());
                        wk.RetrieveFromDBSources();
                        int nodeID = int.Parse(dt.Rows[0][1].ToString());
                        if (nodeID != this.StartNodeID)
                        {
                            string error = "@这里出现了blank的状态下流程运行到其它的节点上去了的情况，当前停留节点:" + nodeID;
                            Log.DefaultLogWriteLineError(error);
                            //     throw new Exception(error);
                        }
                    }
                }

                //启用草稿或空白就创建WorkID
                if (wk.OID == 0 || IsNewWorkID == true)
                {
                    /* 说明没有空白,就创建一个空白..*/
                    wk.ResetDefaultVal();
                    wk.Rec = WebUser.No;

                    wk.SetValByKey(StartWorkAttr.RecText, emp.Name);
                    wk.SetValByKey(StartWorkAttr.Emps, emp.No);

                    wk.SetValByKey(WorkAttr.RDT, BP.DA.DataType.CurrentDataTime);
                    wk.SetValByKey(WorkAttr.CDT, BP.DA.DataType.CurrentDataTime);
                    wk.SetValByKey(GERptAttr.WFState, (int)WFState.Blank);

                    wk.OID = DBAccess.GenerOID("WorkID"); /*这里产生WorkID ,这是唯一产生WorkID的地方.*/

                    //把尽量可能的流程字段放入，否则会出现冲掉流程字段属性.
                    wk.SetValByKey(GERptAttr.FK_NY, BP.DA.DataType.CurrentYearMonth);
                    wk.SetValByKey(GERptAttr.FK_Dept, emp.FK_Dept);
                    wk.FID = 0;
                    wk.DirectInsert();
              

                    //设置参数.
                    foreach (string k in paras.Keys)
                        rpt.SetValByKey(k, paras[k]);

                    if (this.PTable == wk.EnMap.PhysicsTable)
                    {
                        /*如果开始节点表与流程报表相等.*/
                        rpt.OID = wk.OID;
                        rpt.RetrieveFromDBSources();
                        rpt.FID = 0;
                        rpt.FlowStartRDT = BP.DA.DataType.CurrentDataTime;
                        rpt.MyNum = 0;
                        rpt.Title = WorkNode.GenerTitle(this, wk);
                        //WebUser.No + "," + BP.Web.WebUser.Name + "在" + DataType.CurrentDataCNOfShort + "发起.";
                        rpt.WFState = WFState.Blank;
                        rpt.FlowStarter = emp.No;
                        rpt.FK_NY = DataType.CurrentYearMonth;
                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserNameOnly)
                            rpt.FlowEmps = "@" + emp.Name;

                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                            rpt.FlowEmps = "@" + emp.No;

                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                            rpt.FlowEmps = "@" + emp.No + "," + emp.Name;

                        rpt.FlowEnderRDT = BP.DA.DataType.CurrentDataTime;
                        rpt.FK_Dept = emp.FK_Dept;
                        rpt.FlowEnder = emp.No;
                        rpt.FlowEndNode = this.StartNodeID;

                        //生成单据编号.
                        string billNoFormat = this.BillNoFormat.Clone() as string;
                        if (string.IsNullOrEmpty(billNoFormat) == false)
                            rpt.BillNo = BP.WF.Glo.GenerBillNo(billNoFormat, rpt.OID, rpt, this.PTable);

                        rpt.FID = 0;
                        rpt.DirectUpdate();
                    }
                    else
                    {
                        rpt.OID = wk.OID;
                        rpt.FID = 0;
                        rpt.FlowStartRDT = BP.DA.DataType.CurrentDataTime;
                        rpt.FlowEnderRDT = BP.DA.DataType.CurrentDataTime;
                        rpt.MyNum = 0;

                        rpt.Title = WorkNode.GenerTitle(this, wk);
                        // rpt.Title = WebUser.No + "," + BP.Web.WebUser.Name + "在" + DataType.CurrentDataCNOfShort + "发起.";

                        rpt.WFState = WFState.Blank;
                        rpt.FlowStarter = emp.No;

                        rpt.FlowEndNode = this.StartNodeID;
                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserNameOnly)
                            rpt.FlowEmps = "@" + emp.Name;

                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                            rpt.FlowEmps = "@" + emp.No;

                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                            rpt.FlowEmps = "@" + emp.No + "," + emp.Name;


                        rpt.FK_NY = DataType.CurrentYearMonth;
                        rpt.FK_Dept = emp.FK_Dept;
                        rpt.FlowEnder = emp.No;
                        rpt.InsertAsOID(wk.OID);
                    }
                }
            }
            catch (Exception ex)
            {
                wk.CheckPhysicsTable();

                //检查报表.
                this.CheckRpt();
                throw new Exception("@创建工作失败：有可能是您在设计表单时候，新增加的控件，没有预览导致的，请您刷新一次应该可以解决，技术信息：" + ex.StackTrace + " @ 技术信息:" + ex.Message);
            }

            #region copy数据.
            // 记录这个id ,不让其它在复制时间被修改。
            Int64 newOID = wk.OID;
            if (IsNewWorkID == true)
            {
                // 处理传递过来的参数。
                int i = 0;
                foreach (string k in paras.Keys)
                {
                    i++;
                    wk.SetValByKey(k, paras[k].ToString());
                }

                if (i >= 3)
                {
                    wk.OID = newOID;
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
                    BP.DA.Log.DebugWriteError("创建新工作错误，但是屏蔽了异常,请检查默认值的问题：" + ex.Message);
                }

                MapDtls dtls = wk.HisMapDtls;
                foreach (MapDtl dtl in dtls)
                    DBAccess.RunSQL("DELETE FROM " + dtl.PTable + " WHERE RefPK=" + oid);

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

                string innerKeys = ",OID,RDT,CDT,FID,WFState,";
                foreach (DataColumn dc in dt.Columns)
                {
                    if (innerKeys.Contains("," + dc.ColumnName + ","))
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

            if (paras.ContainsKey("PNodeID") == true)
            {
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
            if (string.IsNullOrEmpty(PNodeIDStr) == false && string.IsNullOrEmpty(PWorkIDStr) == false)
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
                if (string.IsNullOrEmpty(PFIDStr) == true || PFIDStr == "0")
                    pOID = PWorkID.ToString();
                else
                    pOID = PFIDStr;

                string sql = "SELECT * FROM " + pFlow.PTable + " WHERE OID=" + pOID;
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count != 1)
                    throw new Exception("@不应该查询不到父流程的数据, 可能的情况之一,请确认该父流程的调用节点是子线程，但是没有把子线程的FID参数传递进来。");


                wk.Copy(dt.Rows[0]);
                rpt.Copy(dt.Rows[0]);
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
                    /*如果不是执行的从已经完成的流程copy.*/

                    wk.SetValByKey(StartWorkAttr.PFlowNo, PFlowNo);
                    wk.SetValByKey(StartWorkAttr.PNodeID, PNodeID);
                    wk.SetValByKey(StartWorkAttr.PWorkID, PWorkID);

                    rpt.SetValByKey(GERptAttr.PFlowNo, PFlowNo);
                    rpt.SetValByKey(GERptAttr.PNodeID, PNodeID);
                    rpt.SetValByKey(GERptAttr.PWorkID, PWorkID);

                    //忘记了增加这句话.
                    rpt.SetValByKey(GERptAttr.PEmp, WebUser.No);


                    rpt.SetValByKey(GERptAttr.FID, 0);
                    rpt.SetValByKey(GERptAttr.FlowStartRDT, BP.DA.DataType.CurrentDataTime);
                    rpt.SetValByKey(GERptAttr.FlowEnderRDT, BP.DA.DataType.CurrentDataTime);
                    rpt.SetValByKey(GERptAttr.MyNum, 0);
                    rpt.SetValByKey(GERptAttr.WFState, (int)WFState.Blank);
                    rpt.SetValByKey(GERptAttr.FlowStarter, emp.No);
                    rpt.SetValByKey(GERptAttr.FlowEnder, emp.No);
                    rpt.SetValByKey(GERptAttr.FlowEndNode, this.StartNodeID);
                    rpt.SetValByKey(GERptAttr.FK_Dept, emp.FK_Dept);
                    rpt.SetValByKey(GERptAttr.FK_NY, DataType.CurrentYearMonth);

                    if (Glo.UserInfoShowModel == UserInfoShowModel.UserNameOnly)
                        rpt.SetValByKey(GERptAttr.FlowEmps, "@" + emp.Name);

                    if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                        rpt.SetValByKey(GERptAttr.FlowEmps, "@" + emp.No);

                    if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                        rpt.SetValByKey(GERptAttr.FlowEmps, "@" + emp.No + "," + emp.Name);
                    //生成单据编号.
                    string billNoFormat = this.BillNoFormat.Clone() as string;
                    if (string.IsNullOrEmpty(billNoFormat) == false)
                        rpt.SetValByKey(GERptAttr.BillNo, BP.WF.Glo.GenerBillNo(billNoFormat, rpt.OID, rpt, this.PTable));
                }

                if (rpt.EnMap.PhysicsTable != wk.EnMap.PhysicsTable)
                    wk.Update(); //更新工作节点数据.
                rpt.Update(); // 更新流程数据表.
                #endregion 特殊赋值.

                #region 复制表单其他数据.
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
                            MapDtl dtlFrom = dtlsFrom[idx] as MapDtl;

                            GEDtls dtlsFromData = new GEDtls(dtlFrom.No);
                            dtlsFromData.Retrieve(GEDtlAttr.RefPK, PWorkID);
                            foreach (GEDtl geDtlFromData in dtlsFromData)
                            {
                                dtlData.Copy(geDtlFromData);
                                dtlData.RefPK = wk.OID.ToString();
                                if (this.No == PFlowNo)
                                    dtlData.InsertAsNew();
                                else
                                    dtlData.SaveAsOID(geDtlFromData.OID);
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

            }
            #endregion 处理流程之间的数据传递1。

            #region 处理单据编号.
            //生成单据编号.
            if (this.BillNoFormat.Length > 3)
            {
                string billNoFormat = this.BillNoFormat.Clone() as string;

                if (billNoFormat.Contains("@"))
                {
                    foreach (string str in paras.Keys)
                        billNoFormat = billNoFormat.Replace("@" + str, paras[str].ToString());
                }

                //生成单据编号.
                rpt.BillNo = BP.WF.Glo.GenerBillNo(billNoFormat, rpt.OID, rpt, this.PTable);
                //rpt.Update(GERptAttr.BillNo, rpt.BillNo);
                if (wk.Row.ContainsKey(GERptAttr.BillNo) == true)
                {
                    wk.SetValByKey(NDXRptBaseAttr.BillNo, rpt.BillNo);
                    // wk.Update(GERptAttr.BillNo, rpt.BillNo);
                }
            }
            #endregion 处理单据编号.

            #region 处理流程之间的数据传递2, 如果是直接要跳转到指定的节点上去.
            if (paras.ContainsKey("JumpToNode") == true)
            {
                wk.Rec = WebUser.No;
                wk.SetValByKey(StartWorkAttr.RDT, BP.DA.DataType.CurrentDataTime);
                wk.SetValByKey(StartWorkAttr.CDT, BP.DA.DataType.CurrentDataTime);
                wk.SetValByKey("FK_NY", DataType.CurrentYearMonth);
                wk.FK_Dept = emp.FK_Dept;
                wk.SetValByKey("FK_DeptName", emp.FK_DeptText);
                wk.SetValByKey("FK_DeptText", emp.FK_DeptText);
                wk.FID = 0;
                wk.SetValByKey(StartWorkAttr.RecText, emp.Name);

                int jumpNodeID = int.Parse(paras["JumpToNode"].ToString());
                Node jumpNode = new Node(jumpNodeID);

                string jumpToEmp = paras["JumpToEmp"].ToString();
                if (string.IsNullOrEmpty(jumpToEmp))
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
            wk.SetValByKey(WorkAttr.RDT, BP.DA.DataType.CurrentDataTime);
            wk.SetValByKey(WorkAttr.CDT, BP.DA.DataType.CurrentDataTime);
            wk.SetValByKey("FK_NY", DataType.CurrentYearMonth);
            wk.FK_Dept = emp.FK_Dept;
            wk.SetValByKey("FK_DeptName", emp.FK_DeptText);
            wk.SetValByKey("FK_DeptText", emp.FK_DeptText);

            wk.SetValByKey(NDXRptBaseAttr.BillNo, rpt.BillNo);
            wk.FID = 0;
            wk.SetValByKey(StartWorkAttr.RecText, emp.Name);
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
                mygwf.Starter = WebUser.No;
                mygwf.StarterName = WebUser.Name;
                mygwf.FK_Dept = BP.Web.WebUser.FK_Dept;
                mygwf.DeptName = BP.Web.WebUser.FK_DeptName;
                mygwf.FK_Flow = this.No;
                mygwf.FK_FlowSort = this.FK_FlowSort;
                mygwf.FK_Node = nd.NodeID;
                mygwf.WorkID = wk.OID;
                mygwf.WFState = WFState.Blank;
                mygwf.FlowName = this.Name;
                mygwf.RDT = BP.DA.DataType.CurrentDataTime;

                if (string.IsNullOrEmpty(PNodeIDStr) == false && string.IsNullOrEmpty(PWorkIDStr) == false)
                {
                    if (string.IsNullOrEmpty(PFIDStr) == false)
                        mygwf.PFID = Int64.Parse(PFIDStr);
                    mygwf.PEmp = rpt.PEmp;
                    mygwf.PFlowNo = rpt.PFlowNo;
                    mygwf.PNodeID = rpt.PNodeID;
                    mygwf.PWorkID = rpt.PWorkID;
                }
                mygwf.Insert();
            }
            else
            {
                if (string.IsNullOrEmpty(PNodeIDStr) == false && string.IsNullOrEmpty(PWorkIDStr) == false)
                {
                    if (string.IsNullOrEmpty(PFIDStr) == false)
                        mygwf.PFID = Int64.Parse(PFIDStr);
                    mygwf.PEmp = rpt.PEmp;
                    mygwf.PFlowNo = rpt.PFlowNo;
                    mygwf.PNodeID = rpt.PNodeID;
                    mygwf.PWorkID = rpt.PWorkID;
                    mygwf.DirectUpdate();
                }
            }
            #endregion 给generworkflow初始化数据.

            return wk;
        }

      
        #endregion 创建新工作.

        #region 初始化一个工作.
        /// <summary>
        /// 初始化一个工作
        /// </summary>
        /// <param name="workid"></param>
        /// <param name="fk_node"></param>
        /// <returns></returns>
        public Work GenerWork(Int64 workid, Node nd, bool isPostBack)
        {
            Work wk = nd.HisWork;
            wk.OID = workid;
            if (wk.RetrieveFromDBSources() == 0)
            {
                /*
                 * 2012-10-15 偶然发现一次工作丢失情况, WF_GenerWorkerlist WF_GenerWorkFlow 都有这笔数据，没有查明丢失原因。 stone.
                 * 用如下代码自动修复，但是会遇到数据copy不完全的问题。
                 * */
#warning 2011-10-15 偶然发现一次工作丢失情况.

                string fk_mapData = "ND" + int.Parse(this.No) + "Rpt";
                GERpt rpt = new GERpt(fk_mapData);
                rpt.OID = int.Parse(workid.ToString());
                if (rpt.RetrieveFromDBSources() >= 1)
                {
                    /*  查询到报表数据.  */
                    wk.Copy(rpt);
                    wk.Rec = WebUser.No;
                    wk.InsertAsOID(workid);
                }
                else
                {
                    /*  没有查询到报表数据.  */

#warning 这里不应该出现的异常信息.

                    string msg = "@不应该出现的异常.";
                    msg += "@在为节点NodeID=" + nd.NodeID + " workid:" + workid + " 获取数据时.";
                    msg += "@获取它的Rpt表数据时，不应该查询不到。";
                    msg += "@GERpt 信息: table:" + rpt.EnMap.PhysicsTable + "   OID=" + rpt.OID;

                    string sql = "SELECT count(*) FROM " + rpt.EnMap.PhysicsTable + " WHERE OID=" + workid;
                    int num = DBAccess.RunSQLReturnValInt(sql);

                    msg += " @SQL:" + sql;
                    msg += " ReturnNum:" + num;
                    if (num == 0)
                    {
                        msg += "已经用sql可以查询出来，但是不应该用类查询不出来.";
                    }
                    else
                    {
                        /*如果可以用sql 查询出来.*/
                        num = rpt.RetrieveFromDBSources();
                        msg += "@从rpt.RetrieveFromDBSources = " + num;
                    }

                    Log.DefaultLogWriteLineError(msg);

                    MapData md = new MapData("ND" + int.Parse(nd.FK_Flow) + "01");
                    sql = "SELECT * FROM " + md.PTable + " WHERE OID=" + workid;
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 1)
                    {
                        rpt.Copy(dt.Rows[0]);
                        try
                        {
                            rpt.FlowStarter = dt.Rows[0][StartWorkAttr.Rec].ToString();
                            rpt.FlowStartRDT = dt.Rows[0][StartWorkAttr.RDT].ToString();
                            rpt.FK_Dept = dt.Rows[0][StartWorkAttr.FK_Dept].ToString();
                        }
                        catch
                        {
                        }

                        rpt.OID = int.Parse(workid.ToString());
                        try
                        {
                            rpt.InsertAsOID(rpt.OID);
                        }
                        catch (Exception ex)
                        {
                            Log.DefaultLogWriteLineError("@不应该出插入不进去 rpt:" + rpt.EnMap.PhysicsTable + " workid=" + workid);
                            rpt.RetrieveFromDBSources();
                        }
                    }
                    else
                    {
                        Log.DefaultLogWriteLineError("@没有找到开始节点的数据, NodeID:" + nd.NodeID + " workid:" + workid);
                        throw new Exception("@没有找到开始节点的数据, NodeID:" + nd.NodeID + " workid:" + workid + " SQL:" + sql);
                    }

#warning 不应该出现的工作丢失.
                    Log.DefaultLogWriteLineError("@工作[" + nd.NodeID + " : " + wk.EnDesc + "], 报表数据WorkID=" + workid + " 丢失, 没有从NDxxxRpt里找到记录,请联系管理员。");

                    wk.Copy(rpt);
                    wk.Rec = WebUser.No;
                    wk.ResetDefaultVal();
                    wk.Insert();
                }
            }

            #region 判断是否有删除草稿的需求.
            if (SystemConfig.IsBSsystem == true && isPostBack == false && nd.IsStartNode && BP.Sys.Glo.Request.QueryString["IsDeleteDraft"] == "1")
            {

                /*需要删除草稿.*/
                /*是否要删除Draft */
                string title = wk.GetValStringByKey("Title");
                wk.ResetDefaultValAllAttr();
                wk.OID = workid;
                wk.SetValByKey(GenerWorkFlowAttr.Title, title);
                wk.DirectUpdate();

                MapDtls dtls = wk.HisMapDtls;
                foreach (MapDtl dtl in dtls)
                    DBAccess.RunSQL("DELETE FROM " + dtl.PTable + " WHERE RefPK=" + wk.OID);

                //删除附件数据。
                DBAccess.RunSQL("DELETE FROM Sys_FrmAttachmentDB WHERE FK_MapData='ND" + wk.NodeID + "' AND RefPKVal='" + wk.OID + "'");

            }
            #endregion


            // 设置当前的人员把记录人。
            wk.Rec = WebUser.No;
            wk.RecText = WebUser.Name;
            wk.Rec = WebUser.No;
            wk.SetValByKey(WorkAttr.RDT, BP.DA.DataType.CurrentDataTime);
            wk.SetValByKey(WorkAttr.CDT, BP.DA.DataType.CurrentDataTime);
            wk.SetValByKey(GERptAttr.WFState, WFState.Runing);
            wk.SetValByKey("FK_Dept", WebUser.FK_Dept);
            wk.SetValByKey("FK_DeptName", WebUser.FK_DeptName);
            wk.SetValByKey("FK_DeptText", WebUser.FK_DeptName);
            wk.FID = 0;
            wk.SetValByKey("RecText", WebUser.Name);

            //处理单据编号.
            if (nd.IsStartNode)
            {
                try
                {
                    string billNo = wk.GetValStringByKey(NDXRptBaseAttr.BillNo);
                    if (string.IsNullOrEmpty(billNo) && nd.HisFlow.BillNoFormat.Length > 2)
                    {
                        /*让他自动生成编号*/
                        wk.SetValByKey(NDXRptBaseAttr.BillNo,
                            BP.WF.Glo.GenerBillNo(nd.HisFlow.BillNoFormat, wk.OID, wk, nd.HisFlow.PTable));
                    }
                }
                catch
                {
                    // 可能是没有billNo这个字段,也不需要处理它.
                }
            }

            return wk;
        }
        #endregion 初始化一个工作

        #region 其他通用方法.
        public string DoBTableDTS()
        {
            if (this.DTSWay == FlowDTSWay.None)
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
                    info += this.DoBTableDTS(rpt, new Node(gwf.FK_Node), true);
                else
                    info += this.DoBTableDTS(rpt, new Node(gwf.FK_Node), false);
            }
            return info;
        }
        /// <summary>
        /// 同步当前的流程数据到业务数据表里.
        /// </summary>
        /// <param name="rpt">流程报表</param>
        /// <param name="currNode">当前节点ID</param>
        /// <param name="isStopFlow">流程是否结束</param>
        /// <returns>返回同步结果.</returns>
        public string DoBTableDTS(GERpt rpt, Node currNode, bool isStopFlow)
        {
            bool isActiveSave = false;
            // 判断是否符合流程数据同步条件.
            switch (this.DTSTime)
            {
                case FlowDTSTime.AllNodeSend:
                    isActiveSave = true;
                    break;
                case FlowDTSTime.SpecNodeSend:
                    if (this.DTSSpecNodes.Contains(currNode.NodeID.ToString()) == true)
                        isActiveSave = true;
                    break;
                case FlowDTSTime.WhenFlowOver:
                    if (isStopFlow)
                        isActiveSave = true;
                    break;
                default:
                    break;
            }
            if (isActiveSave == false)
                return "";

            #region qinfaliang, 编写同步的业务逻辑,执行错误就抛出异常.

            string[] dtsArray = this.DTSFields.Split('@');

            string[] lcArr = dtsArray[0].Split(',');//取出对应的主表字段
            string[] ywArr = dtsArray[1].Split(',');//取出对应的业务表字段


            string sql = "SELECT " + dtsArray[0] + " FROM " + this.PTable.ToUpper() + " WHERE OID=" + rpt.OID;
            DataTable lcDt = DBAccess.RunSQLReturnTable(sql);
            if (lcDt.Rows.Count == 0)//没有记录就return掉
                return "";

            BP.Sys.SFDBSrc src = new BP.Sys.SFDBSrc(this.DTSDBSrc);
            sql = "SELECT " + dtsArray[1] + " FROM " + this.DTSBTable.ToUpper();

            DataTable ywDt = src.RunSQLReturnTable(sql);

            string values = "";
            string upVal = "";


            for (int i = 0; i < lcArr.Length; i++)
            {
                switch (src.DBSrcType)
                {
                    case Sys.DBSrcType.Localhost:
                        switch (SystemConfig.AppCenterDBType)
                        {
                            case DBType.MSSQL:
                                break;
                            case DBType.Oracle:
                                if (ywDt.Columns[ywArr[i]].DataType == typeof(DateTime))
                                {
                                    if (!string.IsNullOrEmpty(lcDt.Rows[0][lcArr[i].ToString()].ToString()))
                                    {
                                        values += "to_date('" + lcDt.Rows[0][lcArr[i].ToString()] + "','YYYY-MM-DD'),";
                                    }
                                    else
                                    {
                                        values += "'',";
                                    }
                                    continue;
                                }
                                values += "'" + lcDt.Rows[0][lcArr[i].ToString()] + "',";
                                continue;
                                break;
                            case DBType.MySQL:
                                break;
                            case DBType.Informix:
                                break;
                            default:
                                throw new Exception("没有涉及到的连接测试类型...");
                        }
                        break;
                    case Sys.DBSrcType.SQLServer:
                        break;
                    case Sys.DBSrcType.MySQL:
                        break;
                    case Sys.DBSrcType.Oracle:
                        if (ywDt.Columns[ywArr[i]].DataType == typeof(DateTime))
                        {
                            if (!string.IsNullOrEmpty(lcDt.Rows[0][lcArr[i].ToString()].ToString()))
                            {
                                values += "to_date('" + lcDt.Rows[0][lcArr[i].ToString()] + "','YYYY-MM-DD'),";
                            }
                            else
                            {
                                values += "'',";
                            }
                            continue;
                        }
                        values += "'" + lcDt.Rows[0][lcArr[i].ToString()] + "',";
                        continue;
                    default:
                        throw new Exception("暂时不支您所使用的数据库类型!");
                }
                values += "'" + lcDt.Rows[0][lcArr[i].ToString()] + "',";
                //获取除主键之外的其他值
                if(i>0)
                    upVal = upVal + ywArr[i] + "='" + lcDt.Rows[0][lcArr[i].ToString()] + "',";
            }

            values = values.Substring(0, values.Length - 1);
            upVal = upVal.Substring(0,upVal.Length-1);

            //查询对应的业务表中是否存在这条记录
            sql = "SELECT * FROM " + this.DTSBTable.ToUpper() + " WHERE " + DTSBTablePK + "='" + lcDt.Rows[0][lcArr[0].ToString()] + "'";
            DataTable dt = src.RunSQLReturnTable(sql);
            //如果存在，执行更新，如果不存在，执行插入
            if (dt.Rows.Count > 0)
            {

                sql = "UPDATE " + this.DTSBTable.ToUpper() + " SET " + upVal + " WHERE " + DTSBTablePK + "='" + lcDt.Rows[0][lcArr[0].ToString()] + "'";
            }
            else
                sql = "INSERT INTO " + this.DTSBTable.ToUpper() + "(" + dtsArray[1] + ") VALUES(" + values + ")";

            try
            {
                src.RunSQL(sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            #endregion qinfaliang, 编写同步的业务逻辑,执行错误就抛出异常.

            return "同步成功.";
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
                case BP.WF.FlowRunWay.DataModel: //按数据集合驱动的模式执行。
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
                BP.DA.Log.DefaultLogWriteLineError("没有为流程(" + this.Name + ")的开始节点设置发起数据,请参考说明书解决.");
                return "没有为流程(" + this.Name + ")的开始节点设置发起数据,请参考说明书解决.";
            }
            if (string.IsNullOrEmpty(me.Tag))
            {
                BP.DA.Log.DefaultLogWriteLineError("没有为流程(" + this.Name + ")的开始节点设置发起数据,请参考说明书解决.");
                return "没有为流程(" + this.Name + ")的开始节点设置发起数据,请参考说明书解决.";
            }

            // 获取从表数据.
            DataSet ds = new DataSet();
            string[] dtlSQLs = me.Tag1.Split('*');
            foreach (string sql in dtlSQLs)
            {
                if (string.IsNullOrEmpty(sql))
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
                    BP.DA.Log.DefaultLogWriteLineInfo(msg);
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
                if (BP.Web.WebUser.No == "admin")
                    uac.IsUpdate = true;
                return uac;
            }
        }
        /// <summary>
        /// 修补流程数据视图
        /// </summary>
        /// <returns></returns>
        public static string RepareV_FlowData_View()
        {
            return null;
            //string err = "";
            //Flows fls = new Flows();
            //fls.RetrieveAllFromDBSource();

            //if (fls.Count == 0)
            //    return null;

            //string sql = "";
            //sql = "CREATE VIEW V_FlowData (FK_FlowSort,FK_Flow,OID,FID,Title,WFState,CDT,FlowStarter,FlowStartRDT,FK_Dept,FK_NY,FlowDaySpan,FlowEmps,FlowEnder,FlowEnderRDT,FlowEndNode,MyNum, PWorkID,PFlowNo,BillNo,ProjNo) ";
            ////     sql += "\t\n /*  WorkFlow Data " + DateTime.Now.ToString("yyyy-MM-dd") + " */ ";
            //sql += " AS ";
            //foreach (Flow fl in fls)
            //{
            //    if (fl.IsCanStart == false)
            //        continue;


            //    string mysql = "\t\n SELECT '" + fl.FK_FlowSort + "' AS FK_FlowSort,'" + fl.No + "' AS FK_Flow,OID,FID,Title,WFState,CDT,FlowStarter,FlowStartRDT,FK_Dept,FK_NY,FlowDaySpan,FlowEmps,FlowEnder,FlowEnderRDT,FlowEndNode,1 as MyNum,PWorkID,PFlowNo,BillNo,ProjNo FROM " + fl.PTable + " WHERE WFState >1 ";
            //    try
            //    {
            //        DBAccess.RunSQLReturnTable(mysql);
            //    }
            //    catch (Exception ex)
            //    {
            //        continue;
            //        try
            //        {
            //            fl.DoCheck();
            //            DBAccess.RunSQLReturnTable(mysql);
            //        }
            //        catch (Exception ex1)
            //        {
            //            err += ex1.Message;
            //            continue;
            //        }
            //    }

            //    if (fls.Count == 1)
            //        break;

            //    sql += mysql;
            //    sql += "\t\n UNION ";
            //}
            //if (sql.Contains("SELECT") == false)
            //    return null;

            //if (fls.Count > 1)
            //    sql = sql.Substring(0, sql.Length - 6);

            //if (sql.Length > 20)
            //{
            //    #region 删除 V_FlowData
            //    try
            //    {
            //        DBAccess.RunSQL("DROP VIEW V_FlowData");
            //    }
            //    catch
            //    {
            //        try
            //        {
            //            DBAccess.RunSQL("DROP table V_FlowData");
            //        }
            //        catch
            //        {
            //        }
            //    }
            //    #endregion 删除 V_FlowData

            //    #region 创建视图.
            //    try
            //    {
            //        DBAccess.RunSQL(sql);
            //    }
            //    catch
            //    {
            //    }
            //    #endregion 创建视图.

            //}
            //return null;
        }
        /// <summary>
        /// 校验流程
        /// </summary>
        /// <returns></returns>
        public string DoCheck()
        {
            #region 检查独立表单
            FrmNodes fns = new FrmNodes();
            fns.Retrieve(FrmNodeAttr.FK_Flow, this.No);
            string frms = "";
            string err = "";
            foreach (FrmNode item in fns)
            {
                if (frms.Contains(item.FK_Frm + ","))
                    continue;
                frms += item.FK_Frm + ",";
                try
                {
                    MapData md = new MapData(item.FK_Frm);
                    md.RepairMap();
                    Entity en = md.HisEn;
                    en.CheckPhysicsTable();
                }
                catch (Exception ex)
                {
                    err += "@节点绑定的表单:" + item.FK_Frm + ",已经被删除了.异常信息." + ex.Message;
                }
            }
            #endregion

            try
            {
                // 设置流程名称.
                DBAccess.RunSQL("UPDATE WF_Node SET FlowName = (SELECT Name FROM WF_Flow WHERE NO=WF_Node.FK_Flow)");

                //删除垃圾,非法数据.
                string sqls = "DELETE FROM Sys_FrmSln where fk_mapdata not in (select no from sys_mapdata)";
                sqls += "@ DELETE FROM WF_Direction WHERE Node=ToNode";
                DBAccess.RunSQLs(sqls);

                //更新计算数据.
                this.NumOfBill = DBAccess.RunSQLReturnValInt("SELECT count(*) FROM WF_BillTemplate WHERE NodeID IN (select NodeID from WF_Flow WHERE no='" + this.No + "')");
                this.NumOfDtl = DBAccess.RunSQLReturnValInt("SELECT count(*) FROM Sys_MapDtl WHERE FK_MapData='ND" + int.Parse(this.No) + "Rpt'");
                this.DirectUpdate();

                string msg = "@  =======  关于《" + this.Name + " 》流程检查报告  ============";
                msg += "@信息输出分为三种: 信息  警告  错误. 如果遇到输出的错误，则必须要去修改或者设置.";
                msg += "@流程检查目前还不能覆盖100%的错误,需要手工的运行一次才能确保流程设计的正确性.";

                Nodes nds = new Nodes(this.No);

                //单据模版.
                BillTemplates bks = new BillTemplates(this.No);

                //条件集合.
                Conds conds = new Conds(this.No);

                #region 对节点进行检查
                //节点表单字段数据类型检查--begin---------
                msg += CheckFormFields();
                //表单字段数据类型检查-------End-----

                foreach (Node nd in nds)
                {
                    //设置它的位置类型.
                    nd.SetValByKey(NodeAttr.NodePosType, (int)nd.GetHisNodePosType());

                    msg += "@信息: -------- 开始检查节点ID:(" + nd.NodeID + ")名称:(" + nd.Name + ")信息 -------------";

                    #region 修复数节点表单数据库.
                    msg += "@信息:开始补充&修复节点必要的字段";
                    try
                    {
                        nd.RepareMap();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@修复节点表必要字段时出现错误:" + nd.Name + " - " + ex.Message);
                    }

                    msg += "@信息:开始修复节点物理表.";
                    DBAccess.RunSQL("UPDATE Sys_MapData SET Name='" + nd.Name + "' WHERE No='ND" + nd.NodeID + "'");
                    try
                    {
                        nd.HisWork.CheckPhysicsTable();
                    }
                    catch (Exception ex)
                    {
                        msg += "@检查节点表字段时出现错误:" + "NodeID" + nd.NodeID + " Table:" + nd.HisWork.EnMap.PhysicsTable + " Name:" + nd.Name + " , 节点类型NodeWorkTypeText=" + nd.NodeWorkTypeText + "出现错误.@err=" + ex.Message;
                    }

                    // 从表检查。
                    Sys.MapDtls dtls = new BP.Sys.MapDtls("ND" + nd.NodeID);
                    foreach (Sys.MapDtl dtl in dtls)
                    {
                        msg += "@检查明细表:" + dtl.Name;
                        try
                        {
                            dtl.HisGEDtl.CheckPhysicsTable();
                        }
                        catch (Exception ex)
                        {
                            msg += "@检查明细表时间出现错误" + ex.Message;
                        }
                    }
                    #endregion 修复数节点表单数据库.

                    MapAttrs mattrs = new MapAttrs("ND" + nd.NodeID);

                    #region 对节点的访问规则进行检查

                    msg += "@信息:开始对节点的访问规则进行检查.";

                    switch (nd.HisDeliveryWay)
                    {
                        case DeliveryWay.ByStation:
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
                            {
                                msg += "@错误:您设置了该节点的访问规则是按指定的岗位计算，但是您没有设置节点编号.</font>";
                            }
                            else
                            {
                                if (DataType.IsNumStr(nd.DeliveryParas) == false)
                                {
                                    msg += "@错误:您没有设置指定岗位的节点编号，目前设置的为{" + nd.DeliveryParas + "}";
                                }
                            }
                            break;
                        case DeliveryWay.ByDeptAndStation: /*按部门与岗位的交集计算.*/
                            string mysql = string.Empty;
                            //added by liuxc,2015.6.30.
                            //区别集成与BPM模式
                            if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                            {
                                mysql =
                                    "SELECT No FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept IN ( SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" +
                                    nd.NodeID + "))AND No IN (SELECT FK_Emp FROM " + BP.WF.Glo.EmpStation +
                                    " WHERE FK_Station IN ( SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" +
                                    nd.NodeID + " )) ORDER BY No ";
                            }
                            else
                            {
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
                            }

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
                                    string sql = nd.DeliveryParas;
                                    foreach (MapAttr item in mattrs)
                                    {
                                        if (item.IsNum)
                                            sql = sql.Replace("@" + item.KeyOfEn, "0");
                                        else
                                            sql = sql.Replace("@" + item.KeyOfEn, "'0'");
                                    }

                                    sql = sql.Replace("@WebUser.No", "'ss'");
                                    sql = sql.Replace("@WebUser.Name", "'ss'");
                                    sql = sql.Replace("@WebUser.FK_Dept", "'ss'");
                                    sql = sql.Replace("@WebUser.FK_DeptName", "'ss'");

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
                            if (mattrs.Contains(BP.Sys.MapAttrAttr.KeyOfEn, nd.DeliveryParas) == false)
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
                                msg += "@错误:开始节点不能设置指定的选择人员访问规则。";
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
                    //设置它没有流程完成条件.
                    nd.IsCCFlow = false;

                    if (conds.Count != 0)
                    {
                        msg += "@信息:开始检查(" + nd.Name + ")方向条件:";
                        foreach (Cond cond in conds)
                        {
                            if (cond.FK_Node == nd.NodeID && cond.HisCondType == CondType.Flow)
                            {
                                nd.IsCCFlow = true;
                                nd.Update();
                            }

                            Node ndOfCond = new Node();
                            ndOfCond.NodeID = ndOfCond.NodeID;
                            if (ndOfCond.RetrieveFromDBSources() == 0)
                                continue;

                            try
                            {
                                if (cond.AttrKey.Length < 2)
                                    continue;
                                if (ndOfCond.HisWork.EnMap.Attrs.Contains(cond.AttrKey) == false)
                                    throw new Exception("@错误:属性:" + cond.AttrKey + " , " + cond.AttrName + " 不存在。");
                            }
                            catch (Exception ex)
                            {
                                msg += "@错误:" + ex.Message;
                                ndOfCond.Delete();
                            }
                            msg += cond.AttrKey + cond.AttrName + cond.OperatorValue + "、";
                        }
                        msg += "@(" + nd.Name + ")方向条件检查完成.....";
                    }
                    #endregion 检查节点完成条件的定义.
                }
                #endregion

                msg += "@流程的基础信息: ------ ";
                msg += "@编号:  " + this.No + " 名称:" + this.Name + " , 存储表:" + this.PTable;

                msg += "@信息:开始检查节点流程报表.";
                this.DoCheck_CheckRpt(this.HisNodes);

                #region 检查焦点字段设置是否还有效
                msg += "@信息:开始检查节点的焦点字段";

                //获得gerpt字段.
                GERpt rpt = this.HisGERpt;
                foreach (Node nd in nds)
                {
                    if (nd.FocusField.Trim() == "")
                    {
                        Work wk = nd.HisWork;
                        string attrKey = "";
                        foreach (Attr attr in wk.EnMap.Attrs)
                        {
                            if (attr.UIVisible == true && attr.UIIsDoc && attr.UIIsReadonly == false)
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
                        msg += "@错误:焦点字段（" + nd.FocusField + "）在节点(step:" + nd.Step + " 名称:" + nd.Name + ")属性里的设置已无效，表单里不存在该字段.";
                    }
                    else
                    {
                        msg += "@提示:节点的(" + nd.NodeID + "," + nd.Name + ")焦点字段（" + nd.FocusField + "）设置完整检查通过.";
                    }

                    if (this.IsMD5)
                    {
                        if (nd.HisWork.EnMap.Attrs.Contains(WorkAttr.MD5) == false)
                            nd.RepareMap();
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
                        string sql = "SELECT COUNT(*) FROM Sys_MapAttr WHERE FK_MapData='ND" + nd.NodeID + "' AND KeyOfEn IN ('EvalEmpNo','EvalEmpName','EvalEmpCent')";
                        if (DBAccess.RunSQLReturnValInt(sql) != 3)
                            msg += "@信息:您设置了节点(" + nd.NodeID + "," + nd.Name + ")为质量考核节点，但是您没有在该节点表单中设置必要的节点考核字段.";
                    }
                }
                msg += "@检查质量考核点完成.";
                #endregion

                msg += "@流程报表检查完成...";

                // 检查流程.
                Node.CheckFlow(this);

                //生成 V001 视图. del by stone 2016.03.27.
                // CheckRptViewDel(nds);
                return msg;
            }
            catch (Exception ex)
            {
                throw new Exception("@检查流程错误:" + ex.Message + " @" + ex.StackTrace);
            }
        }
        /// <summary>
        /// 节点表单字段数据类型检查，名字相同的字段出现类型不同的处理方法：依照不同于NDxxRpt表中同名字段类型为基准
        /// </summary>
        /// <returns>检查结果</returns>
        private string CheckFormFields()
        {
            StringBuilder errorAppend = new StringBuilder();
            errorAppend.Append("@信息: -------- 流程节点表单的字段类型检查: ------ ");
            try
            {
                Nodes nds = new Nodes(this.No);
                string fk_mapdatas = "'ND" + int.Parse(this.No) + "Rpt'";
                foreach (Node nd in nds)
                {
                    fk_mapdatas += ",'ND" + nd.NodeID + "'";
                }

                //筛选出类型不同的字段
                string checkSQL = "SELECT   AA.KEYOFEN, COUNT(*) AS MYNUM FROM ("
                                    + "  SELECT A.KEYOFEN,  MYDATATYPE,  COUNT(*) AS MYNUM"
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

            name = path + name + "." + this.Ver.Replace(":", "_") + ".xml";
            return name;
        }
        /// <summary>
        /// 生成流程模板
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DataSet DoExpFlowXmlTemplete(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            DataSet ds = GetFlow(path);
            if (ds != null)
            {
                string name = this.Name;
                name = BP.Tools.StringExpressionCalculate.ReplaceBadCharOfFileName(name);
                name = name + "." + this.Ver.Replace(":", "_") + ".xml";
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
                    if (!string.IsNullOrEmpty(path))
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

                    if (!string.IsNullOrEmpty(xmlName))
                    {
                        ds.WriteXml(xmlName);
                        isXmlLocked = false;
                    }
                }
            }
            catch (Exception e)
            {
                isXmlLocked = false;
                BP.DA.Log.DefaultLogWriteLineError("流程模板文件备份错误:" + e.Message);
            }
        }

        private string GenerBPMN2()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("");

            sb.Append("");

            return sb.ToString();
        }

        public DataSet GetFlow(string path)
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
            // 独立表单树
            sql = "SELECT * FROM WF_FlowFormTree WHERE FK_Flow='" + this.No + "'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_FlowFormTree";
            ds.Tables.Add(dt);

            //// 独立表单
            //sql = "SELECT * FROM WF_FlowForm WHERE FK_Flow='" + this.No + "'";
            //dt = DBAccess.RunSQLReturnTable(sql);
            //dt.TableName = "WF_FlowForm";
            //ds.Tables.Add(dt);

            //// 节点表单权限
            //sql = "SELECT * FROM WF_NodeForm WHERE FK_Node IN (" + sqlin + ")";
            //dt = DBAccess.RunSQLReturnTable(sql);
            //dt.TableName = "WF_NodeForm";
            //ds.Tables.Add(dt);

            // 条件信息
            sql = "SELECT * FROM WF_Cond WHERE FK_Flow='" + this.No + "'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_Cond";
            ds.Tables.Add(dt);

            // 转向规则.
            sql = "SELECT * FROM WF_TurnTo WHERE FK_Flow='" + this.No + "'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_TurnTo";
            ds.Tables.Add(dt);

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

            //// 应用设置 FAppSet
            //sql = "SELECT * FROM WF_FAppSet WHERE FK_Flow='" + this.No + "'";
            //dt = DBAccess.RunSQLReturnTable(sql);
            //dt.TableName = "WF_FAppSet";
            //ds.Tables.Add(dt);

            // 流程标签.
            LabNotes labs = new LabNotes(this.No);
            ds.Tables.Add(labs.ToDataTableField("WF_LabNote"));

            // 消息监听.
            Listens lts = new Listens(this.No);
            ds.Tables.Add(lts.ToDataTableField("WF_Listen"));

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

            //// 流程报表。
            //WFRpts rpts = new WFRpts(this.No);
            //// rpts.SaveToXml(path + "WFRpts.xml");
            //ds.Tables.Add(rpts.ToDataTableField("WF_Rpt"));

            //// 流程报表属性
            //RptAttrs rptAttrs = new RptAttrs();
            //rptAttrs.RetrieveAll();
            //ds.Tables.Add(rptAttrs.ToDataTableField("RptAttrs"));

            //// 流程报表访问权限。
            //RptStations rptStations = new RptStations(this.No);
            //rptStations.RetrieveAll();
            ////  rptStations.SaveToXml(path + "RptStations.xml");
            //ds.Tables.Add(rptStations.ToDataTableField("RptStations"));

            //// 流程报表人员访问权限。
            //RptEmps rptEmps = new RptEmps(this.No);
            //rptEmps.RetrieveAll();

            // rptEmps.SaveToXml(path + "RptEmps.xml");
            // ds.Tables.Add(rptEmps.ToDataTableField("RptEmps"));

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
            sql = "SELECT * FROM Sys_GroupField WHERE   " + Glo.MapDataLikeKey(this.No, "EnName"); // +" " + Glo.MapDataLikeKey(this.No, "EnName");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_GroupField";
            ds.Tables.Add(dt);

            // Sys_MapFrame
            sql = "SELECT * FROM Sys_MapFrame WHERE" + Glo.MapDataLikeKey(this.No, "FK_MapData");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapFrame";
            ds.Tables.Add(dt);

            // Sys_MapM2M
            sql = "SELECT * FROM Sys_MapM2M WHERE " + Glo.MapDataLikeKey(this.No, "FK_MapData");
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapM2M";
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
                string flowEmps = "";
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
                            if (flowEmps.Contains("@" + wk.Rec + ","))
                                continue;

                            flowEmps += "@" + wk.Rec + "," + wk.RecOfEmp.Name;
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
                if (string.IsNullOrEmpty(rpt.FK_Dept))
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
                rpt.FlowStartRDT = startWork.RDT;
                rpt.FID = startWork.GetValIntByKey("FID");
                rpt.FlowEmps = flowEmps;
                rpt.FlowEnder = endWK.Rec;
                rpt.FlowEnderRDT = endWK.RDT;
                rpt.FlowEndNode = endWK.NodeID;
                rpt.MyNum = 1;

                //修复标题字段。
                WorkNode wn = new WorkNode(startWork, this.HisStartNode);
                rpt.Title = WorkNode.GenerTitle(this, startWork);
                try
                {
                    TimeSpan ts = endWK.RDT_DateTime - startWork.RDT_DateTime;
                    rpt.FlowDaySpan = ts.Days;
                }
                catch
                {
                }
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
                        rtpDtl.GroupID = -1;
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
        /// 产生所有节点视图
        /// </summary>
        /// <param name="nds"></param>
        private void CheckRptViewDel(Nodes nds)
        {
            if (this.HisDataStoreModel == DataStoreModel.SpecTable)
                return;
         
            string viewName = "V" + this.No;
            string sql = "CREATE VIEW " + viewName + " ";
            sql += "/* CCFlow Auto Create :" + this.Name + " Date:" + DateTime.Now.ToString("yyyy-MM-dd") + " */ ";
            sql += "\r\n (MyPK,FK_Node,OID,FID,RDT,FK_NY,CDT,Rec,Emps,FK_Dept,MyNum) AS ";
            bool is1 = false;
            foreach (Node nd in nds)
            {
                //  nd.HisWork.CheckPhysicsTable();
                if (is1 == false)
                    is1 = true;
                else
                    sql += "\r\n UNION ";

                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                    case DBType.Informix:
                        sql += "\r\n SELECT '" + nd.NodeID + "' || '_'|| OID||'_'|| FID  AS MyPK, '" + nd.NodeID + "' AS FK_Node,OID,FID,RDT,SUBSTR(RDT,1,7) AS FK_NY,CDT,Rec,Emps,FK_Dept, 1 AS MyNum FROM ND" + nd.NodeID + " ";
                        break;
                    case DBType.MySQL:
                        sql += "\r\n SELECT '" + nd.NodeID + "'+'_'+CHAR(OID)  +'_'+CHAR(FID)  AS MyPK, '" + nd.NodeID + "' AS FK_Node,OID,FID,RDT," + BP.Sys.SystemConfig.AppCenterDBSubstringStr + "(RDT,1,7) AS FK_NY,CDT,Rec,Emps,FK_Dept, 1 AS MyNum FROM ND" + nd.NodeID + " ";
                        break;
                    default:
                        sql += "\r\n SELECT '" + nd.NodeID + "'+'_'+CAST(OID AS varchar(10)) +'_'+CAST(FID AS VARCHAR(10)) AS MyPK, '" + nd.NodeID + "' AS FK_Node,OID,FID,RDT," + BP.Sys.SystemConfig.AppCenterDBSubstringStr + "(RDT,1,7) AS FK_NY,CDT,Rec,Emps,FK_Dept, 1 AS MyNum FROM ND" + nd.NodeID + " ";
                        break;
                }
            }
            if (SystemConfig.AppCenterDBType != DBType.Informix)
                sql += "\r\n GO ";

            try
            {
                if (DBAccess.IsExitsObject(viewName) == true)
                    DBAccess.RunSQL("DROP VIEW " + viewName);
            }
            catch
            {
            }

            try
            {
                DBAccess.RunSQL(sql);
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex.Message);
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

            // 处理track表.
            Track.CreateOrRepairTrackTable(flowId);

            #region 插入字段。
            string sql = "";
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.Oracle:
                case DBType.MSSQL:
                    sql = "SELECT distinct  KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN ( SELECT 'ND' " + SystemConfig.AppCenterDBAddStringStr + " cast(NodeID as varchar(20)) FROM WF_Node WHERE FK_Flow='" + this.No + "')";
                    break;
                case DBType.Informix:
                    sql = "SELECT distinct  KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN ( SELECT 'ND' " + SystemConfig.AppCenterDBAddStringStr + " cast(NodeID as varchar(20)) FROM WF_Node WHERE FK_Flow='" + this.No + "')";
                    break;
                case DBType.MySQL:
                    sql = "SELECT DISTINCT KeyOfEn FROM Sys_MapAttr  WHERE FK_MapData IN (SELECT X.No FROM ( SELECT CONCAT('ND',NodeID) AS No FROM WF_Node WHERE FK_Flow='" + this.No + "') AS X )";
                    break;
                default:
                    sql = "SELECT distinct  KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN ( SELECT 'ND' " + SystemConfig.AppCenterDBAddStringStr + " cast(NodeID as varchar(20)) FROM WF_Node WHERE FK_Flow='" + this.No + "')";
                    break;
            }

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

            // 补充上没有字段。
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.Oracle:
                    sql = "SELECT MyPK, KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN ( SELECT 'ND' " + SystemConfig.AppCenterDBAddStringStr + " cast(NodeID as varchar(20)) FROM WF_Node WHERE FK_Flow='" + this.No + "')";
                    break;
                case DBType.MySQL:
                    sql = "SELECT MyPK, KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN (SELECT X.No FROM ( SELECT CONCAT('ND',NodeID) AS No FROM WF_Node WHERE FK_Flow='" + this.No + "') AS X )";
                    break;
                default:
                    sql = "SELECT MyPK, KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN ( SELECT 'ND' " + SystemConfig.AppCenterDBAddStringStr + " cast(NodeID as varchar(20)) FROM WF_Node WHERE FK_Flow='" + this.No + "')";
                    break;
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            sql = "SELECT KeyOfEn FROM Sys_MapAttr WHERE FK_MapData='ND" + flowId + "Rpt'";
            DataTable dtExits = DBAccess.RunSQLReturnTable(sql);
            string pks = "@";
            foreach (DataRow dr in dtExits.Rows)
                pks += dr[0] + "@";

            foreach (DataRow dr in dt.Rows)
            {
                string mypk = dr["MyPK"].ToString();
                if (pks.Contains("@" + dr["KeyOfEn"].ToString() + "@"))
                    continue;

                pks += dr["KeyOfEn"].ToString() + "@";

                BP.Sys.MapAttr ma = new BP.Sys.MapAttr(mypk);
                ma.MyPK = "ND" + flowId + "Rpt_" + ma.KeyOfEn;
                ma.FK_MapData = "ND" + flowId + "Rpt";
                ma.UIIsEnable = false;

                if (ma.DefValReal.Contains("@"))
                {
                    /*如果是一个有变量的参数.*/
                    ma.DefVal = "";
                }

                try
                {
                    ma.Insert();
                }
                catch
                {
                }
            }

            MapAttrs attrs = new MapAttrs(fk_mapData);

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
                md.Name = this.Name;
                md.PTable = this.PTable;
                md.Update();
            }
            #endregion 插入字段。

            #region 补充上流程字段到NDxxxRpt.
            int groupID = 0;
            foreach (MapAttr attr in attrs)
            {
                switch (attr.KeyOfEn)
                {
                    case StartWorkAttr.FK_Dept:
                        //  attr.UIBindKey = "BP.Port.Depts";
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

            if (attrs.Contains(md.No + "_" + GERptAttr.OID) == false)
            {
                /* WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.KeyOfEn = "OID";
                attr.Name = "WorkID";
                attr.MyDataType = BP.DA.DataType.AppInt;
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
                attr.MaxLen = 20;
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowStartRDT) == false)
            {
                /* MyNum */
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
                attr.MaxLen = 20;
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEnderRDT) == false)
            {
                /* MyNum */
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
                attr.MyDataType = DataType.AppMoney;
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


            if (attrs.Contains(md.No + "_MyNum") == false)
            {
                /* MyNum */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "MyNum";
                attr.Name = "条";
                attr.MyDataType = DataType.AppInt;
                attr.DefVal = "1";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.Idx = -101;
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
                int num = flowGF.Retrieve(GroupFieldAttr.EnName, fk_mapData, GroupFieldAttr.Lab, "流程信息");
                if (num == 0)
                {
                    flowGF = new GroupField();
                    flowGF.Lab = flowInfo;
                    flowGF.EnName = fk_mapData;
                    flowGF.Idx = -1;
                    flowGF.Insert();
                }
                sql = "UPDATE Sys_MapAttr SET GroupID=" + flowGF.OID + " WHERE  FK_MapData='" + fk_mapData + "'  AND KeyOfEn IN('" + GERptAttr.PFlowNo + "','" + GERptAttr.PWorkID + "','" + GERptAttr.MyNum + "','" + GERptAttr.FK_Dept + "','" + GERptAttr.FK_NY + "','" + GERptAttr.FlowDaySpan + "','" + GERptAttr.FlowEmps + "','" + GERptAttr.FlowEnder + "','" + GERptAttr.FlowEnderRDT + "','" + GERptAttr.FlowEndNode + "','" + GERptAttr.FlowStarter + "','" + GERptAttr.FlowStartRDT + "','" + GERptAttr.WFState + "')";
                DBAccess.RunSQL(sql);
            }
            catch (Exception ex)
            {
                Log.DefaultLogWriteLineError(ex.Message);
            }
            #endregion 为流程字段设置分组

            #region 尾后处理.
            GERpt sw = this.HisGERpt;
            sw.CheckPhysicsTable();  //让报表重新生成.

            DBAccess.RunSQL("DELETE FROM Sys_GroupField WHERE EnName='" + fk_mapData + "' AND OID NOT IN (SELECT GroupID FROM Sys_MapAttr WHERE FK_MapData = '" + fk_mapData + "')");
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET Name='活动时间' WHERE FK_MapData='ND" + flowId + "Rpt' AND KeyOfEn='CDT'");
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET Name='参与者' WHERE FK_MapData='ND" + flowId + "Rpt' AND KeyOfEn='Emps'");
            #endregion 尾后处理.

            #region 处理报表.
            string mapRpt = "ND" + int.Parse(No) + "MyRpt";
            MapData mapData = new MapData();
            mapData.No = mapRpt;
            if (mapData.RetrieveFromDBSources() == 0)
            {
                mapData.No = mapRpt;
                mapData.PTable = this.PTable;
                mapData.Name = this.Name + "报表";
                mapData.Note = "默认.";

                //默认的查询字段.
                mapData.Insert();

                BP.WF.Rpt.MapRpt rpt = new Rpt.MapRpt();
                rpt.No = mapRpt;
                rpt.RetrieveFromDBSources();
                rpt.FK_Flow = this.No;
                rpt.ResetIt();
                rpt.Update();
            }

            if (mapData.PTable != this.PTable)
            {
                mapData.PTable = this.PTable;
                mapData.Update();
            }

            //补充基础字段.
            attrs = new MapAttrs(mapData.No);

            #region 补充上流程字段到NDxxxRpt.
            foreach (MapAttr attr in attrs)
            {
                switch (attr.KeyOfEn)
                {
                    case StartWorkAttr.FK_Dept:
                        attr.UIBindKey = "BP.Port.Depts";
                        attr.UIContralType = UIContralType.DDL;
                        attr.LGType = FieldTypeS.FK;
                        attr.UIVisible = true;
                        attr.GroupID = groupID;// gfs[0].GetValIntByKey("OID");
                        attr.UIIsEnable = false;
                        attr.DefVal = "";
                        attr.MaxLen = 100;
                        attr.Update();
                        break;
                    case "FK_NY":
                        attr.UIBindKey = "BP.Pub.NYs";
                        attr.UIContralType = UIContralType.DDL;
                        attr.LGType = FieldTypeS.FK;
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

            if (attrs.Contains(mapData.No + "_" + GERptAttr.OID) == false)
            {
                /* WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.KeyOfEn = "OID";
                attr.Name = "WorkID";
                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.HisEditType = BP.En.EditType.Readonly;
                attr.Insert();
            }

            if (attrs.Contains(mapData.No + "_" + GERptAttr.WFSta) == false)
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

            if (attrs.Contains(mapData.No + "_" + GERptAttr.FlowEmps) == false)
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

            if (attrs.Contains(mapData.No + "_" + GERptAttr.FlowStarter) == false)
            {
                /* 发起人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowStarter;
                attr.Name = "发起人"; //  
                attr.MyDataType = DataType.AppString;

                attr.UIBindKey = "BP.Port.Emps";
                attr.UIContralType = UIContralType.DDL;
                attr.LGType = FieldTypeS.FK;

                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 20;
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(mapData.No + "_" + GERptAttr.FlowStartRDT) == false)
            {
                /* MyNum */
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

            if (attrs.Contains(mapData.No + "_" + GERptAttr.FlowEnder) == false)
            {
                /* 发起人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEnder;
                attr.Name = "结束人"; //  
                attr.MyDataType = DataType.AppString;
                attr.UIBindKey = "BP.Port.Emps";
                attr.UIContralType = UIContralType.DDL;
                attr.LGType = FieldTypeS.FK;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 20;
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(mapData.No + "_" + GERptAttr.FlowEnderRDT) == false)
            {
                /* MyNum */
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

            if (attrs.Contains(mapData.No + "_" + GERptAttr.FlowEndNode) == false)
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

            if (attrs.Contains(mapData.No + "_" + GERptAttr.FlowDaySpan) == false)
            {
                /* FlowDaySpan */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowDaySpan; // "FlowStartRDT";
                attr.Name = "跨度(天)";
                attr.MyDataType = DataType.AppMoney;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.UIIsLine = false;
                attr.Idx = -101;
                attr.DefVal = "0";
                attr.Insert();
            }

            if (attrs.Contains(mapData.No + "_" + GERptAttr.PFlowNo) == false)
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

            if (attrs.Contains(mapData.No + "_" + GERptAttr.PNodeID) == false)
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

            if (attrs.Contains(mapData.No + "_" + GERptAttr.PWorkID) == false)
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

            if (attrs.Contains(mapData.No + "_" + GERptAttr.PEmp) == false)
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


            //if (attrs.Contains(mapData.No + "_" + GERptAttr.CWorkID) == false)
            //{
            //    /* 延续流程WorkID */
            //    MapAttr attr = new BP.Sys.MapAttr();
            //    attr.FK_MapData = md.No;
            //    attr.HisEditType = EditType.UnDel;
            //    attr.KeyOfEn = GERptAttr.CWorkID;
            //    attr.Name = "延续流程WorkID";
            //    attr.MyDataType = DataType.AppInt;
            //    attr.DefVal = "0";
            //    attr.UIContralType = UIContralType.TB;
            //    attr.LGType = FieldTypeS.Normal;
            //    attr.UIVisible = true;
            //    attr.UIIsEnable = false;
            //    attr.UIIsLine = false;
            //    attr.HisEditType = EditType.UnDel;
            //    attr.Idx = -101;
            //    attr.Insert();
            //}

            //if (attrs.Contains(mapData.No + "_" + GERptAttr.CFlowNo) == false)
            //{
            //    /* 延续流程编号 */
            //    MapAttr attr = new BP.Sys.MapAttr();
            //    attr.FK_MapData = md.No;
            //    attr.HisEditType = EditType.UnDel;
            //    attr.KeyOfEn = GERptAttr.CFlowNo;
            //    attr.Name = "延续流程编号";
            //    attr.MyDataType = DataType.AppString;
            //    attr.UIContralType = UIContralType.TB;
            //    attr.LGType = FieldTypeS.Normal;
            //    attr.UIVisible = true;
            //    attr.UIIsEnable = false;
            //    attr.UIIsLine = true;
            //    attr.MinLen = 0;
            //    attr.MaxLen = 3;
            //    attr.Idx = -100;
            //    attr.Insert();
            //}

            if (attrs.Contains(mapData.No + "_" + GERptAttr.BillNo) == false)
            {
                /* 单据编号 */
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


            if (attrs.Contains(mapData.No + "_MyNum") == false)
            {
                /* MyNum */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "MyNum";
                attr.Name = "条";
                attr.MyDataType = DataType.AppInt;
                attr.DefVal = "1";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(mapData.No + "_" + GERptAttr.AtPara) == false)
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


            if (attrs.Contains(mapData.No + "_" + GERptAttr.GUID) == false)
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

            //if (attrs.Contains(mapData.No + "_" + GERptAttr.PrjNo) == false)
            //{
            //    /* 项目编号 */
            //    MapAttr attr = new BP.Sys.MapAttr();
            //    attr.FK_MapData = md.No;
            //    attr.HisEditType = EditType.UnDel;
            //    attr.KeyOfEn = GERptAttr.PrjNo;
            //    attr.Name = "项目编号"; //  项目编号
            //    attr.MyDataType = DataType.AppString;
            //    attr.UIContralType = UIContralType.TB;
            //    attr.LGType = FieldTypeS.Normal;
            //    attr.UIVisible = true;
            //    attr.UIIsEnable = false;
            //    attr.UIIsLine = false;
            //    attr.MinLen = 0;
            //    attr.MaxLen = 100;
            //    attr.Idx = -100;
            //    attr.Insert();
            //}
            //if (attrs.Contains(mapData.No + "_" + GERptAttr.PrjName) == false)
            //{
            //    /* 项目名称 */
            //    MapAttr attr = new BP.Sys.MapAttr();
            //    attr.FK_MapData = md.No;
            //    attr.HisEditType = EditType.UnDel;
            //    attr.KeyOfEn = GERptAttr.PrjName;
            //    attr.Name = "项目名称"; //  项目名称
            //    attr.MyDataType = DataType.AppString;
            //    attr.UIContralType = UIContralType.TB;
            //    attr.LGType = FieldTypeS.Normal;
            //    attr.UIVisible = true;
            //    attr.UIIsEnable = false;
            //    attr.UIIsLine = false;
            //    attr.MinLen = 0;
            //    attr.MaxLen = 100;
            //    attr.Idx = -100;
            //    attr.Insert();
            //}

            if (attrs.Contains(mapData.No + "_" + GERptAttr.FlowNote) == false)
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
            #endregion 补充上基础字段。

            #endregion 处理报表.
        }
        #endregion 其他公用方法1

        #region 执行流程事件.
        /// <summary>
        /// 执行运动事件
        /// </summary>
        /// <param name="doType">事件类型</param>
        /// <param name="currNode">当前节点</param>
        /// <param name="en">实体</param>
        /// <param name="atPara">参数</param>
        /// <param name="objs">发送对象，可选</param>
        /// <returns>执行结果</returns>
        public string DoFlowEventEntity(string doType, Node currNode, Entity en, string atPara, SendReturnObjs objs, Node jumpToNode = null, string jumpToEmps = null)
        {
            if (currNode == null)
                return null;

            string str = null;
            if (this.FEventEntity != null)
            {
                this.FEventEntity.SendReturnObjs = objs;
                str = this.FEventEntity.DoIt(doType, currNode, en, atPara, jumpToNode, jumpToEmps);
            }

            FrmEvents fes = currNode.MapData.FrmEvents;
            if (str == null)
                str = fes.DoEventNode(doType, en, atPara);


            #region 处理消息推送, edit  by zhoupeng for dengzhou gov project. 2016.05.01
            //有一些事件没有消息，直接 return ;
            switch (doType)
            {
                case EventListOfNode.WorkArrive:
                case EventListOfNode.SendSuccess:
                case EventListOfNode.ShitAfter:
                case EventListOfNode.ReturnAfter:
                case EventListOfNode.UndoneAfter:
                case EventListOfNode.AskerReAfter:
                    break;
                default:
                    return str;
            }
           
            //执行消息的发送.
            PushMsgs pms = currNode.HisPushMsgs;
            string msgAlert = ""; //生成的提示信息.
            foreach (PushMsg item in pms)
            {
                if (item.FK_Event != doType)
                    continue;

                if (item.SMSPushWay == 0 && item.MailPushWay == 0)
                    continue; /* 如果都没有消息设置，就放过.*/

                //执行发送消息.
                msgAlert += item.DoSendMessage(currNode, en, atPara, objs, jumpToNode, jumpToEmps);
            }
            return str + msgAlert;
            #endregion 处理消息推送.


            return str;
        }
        public string DoFlowEventEntity(string doType, Node currNode, Entity en, string atPara, Node jumpToNode=null, string jumpToEmp=null)
        {
            string str = this.DoFlowEventEntity(doType, currNode, en, atPara, null, jumpToNode, jumpToEmp);
            return BP.DA.DataType.PraseGB2312_To_utf8(str);
        }
        private BP.WF.FlowEventBase _FDEventEntity = null;
        /// <summary>
        /// 节点实体类，没有就返回为空.
        /// </summary>
        private BP.WF.FlowEventBase FEventEntity
        {
            get
            {
                if (_FDEventEntity == null && this.FlowMark != "" && this.FlowEventEntity != "")
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
                if (string.IsNullOrEmpty(s))
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
                if (string.IsNullOrEmpty(strs))
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
        /// 设计者编号
        /// </summary>
        public string DesignerNo1
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DesignerNo);
            }
            set
            {
                this.SetValByKey(FlowAttr.DesignerNo, value);
            }
        }
        /// <summary>
        /// 设计者名称
        /// </summary>
        public string DesignerName1
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DesignerName);
            }
            set
            {
                this.SetValByKey(FlowAttr.DesignerName, value);
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
        /// <summary>
        /// 类型
        /// </summary>
        public int DType
        {
            get
            {
                return this.GetValIntByKey(FlowAttr.DType);
            }
            set
            {
                this.SetValByKey(FlowAttr.DType, value);
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
        public bool IsAutoSendSubFlowOver
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsAutoSendSubFlowOver);
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
        /// 节点
        /// </summary>
        public Nodes _HisNodes = null;
        /// <summary>
        /// 他的节点集合.
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                if (this._HisNodes == null)
                    _HisNodes = new Nodes(this.No);
                return _HisNodes;
            }
            set
            {
                _HisNodes = value;
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
        #endregion

        #region 构造方法

        /// <summary>
        /// 流程
        /// </summary>
        public Flow()
        {
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
            this.FlowEventEntity = BP.WF.Glo.GetFlowEventEntityStringByFlowMark(this.FlowMark, this.No);

            DBAccess.RunSQL("UPDATE WF_Node SET FlowName='" + this.Name + "' WHERE FK_Flow='" + this.No + "'");
            DBAccess.RunSQL("UPDATE Sys_MapData SET  Name='" + this.Name + "' WHERE No='" + this.PTable + "'");
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
                map.Java_SetDepositaryOfEntity(Depositary.Application);
                map.Java_SetCodeStruct("3");

                map.AddTBStringPK(FlowAttr.No, null, "编号", true, true, 1, 10, 3);
                map.AddTBString(FlowAttr.Name, null, "名称", true, false, 0, 500, 10);
                map.AddDDLEntities(FlowAttr.FK_FlowSort, "01", "流程类别", new FlowSorts(), false);
                //map.AddTBString(FlowAttr.FK_FlowSort, null, "流程类别", true, false, 0, 10, 10);
                map.AddTBInt(FlowAttr.FlowRunWay, 0, "运行方式", false, false);

                //  map.AddDDLEntities(FlowAttr.FK_FlowSort, "01", "流程类别", new FlowSorts(), false);
                //map.AddDDLSysEnum(FlowAttr.FlowRunWay, (int)FlowRunWay.HandWork, "运行方式", false,
                //    false, FlowAttr.FlowRunWay,
                //    "@0=手工启动@1=指定人员按时启动@2=数据集按时启动@3=触发式启动");

                map.AddTBString(FlowAttr.RunObj, null, "运行内容", true, false, 0, 3000, 10);
                map.AddTBString(FlowAttr.Note, null, "备注", true, false, 0, 100, 10);
                map.AddTBString(FlowAttr.RunSQL, null, "流程结束执行后执行的SQL", true, false, 0, 2000, 10);

                map.AddTBInt(FlowAttr.NumOfBill, 0, "是否有单据", false, false);
                map.AddTBInt(FlowAttr.NumOfDtl, 0, "NumOfDtl", false, false);
                map.AddTBInt(FlowAttr.FlowAppType, 0, "流程类型", false, false);
                map.AddTBInt(FlowAttr.ChartType, 1, "节点图形类型", false, false);

                // map.AddBoolean(FlowAttr.IsOK, true, "是否启用", true, true);
                map.AddBoolean(FlowAttr.IsCanStart, true, "可以独立启动否？", true, true, true);
                map.AddTBDecimal(FlowAttr.AvgDay, 0, "平均运行用天", false, false);


                map.AddTBInt(FlowAttr.IsFullSA, 0, "是否自动计算未来的处理人？(启用后,ccflow就会为已知道的节点填充处理人到WF_SelectAccper)", false, false);
                map.AddTBInt(FlowAttr.IsMD5, 0, "IsMD5", false, false);
                map.AddTBInt(FlowAttr.Idx, 0, "显示顺序号(在发起列表中)", true, false);
                map.AddTBInt(FlowAttr.TimelineRole, 0, "时效性规则", true, false);
                map.AddTBString(FlowAttr.Paras, null, "参数", false, false, 0, 400, 10);

                // add 2013-01-01. 
                map.AddTBString(FlowAttr.PTable, null, "流程数据存储主表", true, false, 0, 30, 10);


                // 草稿规则 "@0=无(不设草稿)@1=保存到待办@2=保存到草稿箱"
                map.AddTBInt(FlowAttr.Draft, 0, "草稿规则", true, false);

                // add 2013-01-01.
                map.AddTBInt(FlowAttr.DataStoreModel, 0, "数据存储模式", true, false);


                // add 2013-02-05.
                map.AddTBString(FlowAttr.TitleRole, null, "标题生成规则", true, false, 0, 150, 10, true);

                // add 2013-02-14 
                map.AddTBString(FlowAttr.FlowMark, null, "流程标记", true, false, 0, 150, 10);
                map.AddTBString(FlowAttr.FlowEventEntity, null, "FlowEventEntity", true, false, 0, 100, 10, true);
                map.AddTBString(FlowAttr.HistoryFields, null, "历史查看字段", true, false, 0, 500, 10, true);
                map.AddTBInt(FlowAttr.IsGuestFlow, 0, "是否是客户参与流程？", true, false);
                map.AddTBString(FlowAttr.BillNoFormat, null, "单据编号格式", true, false, 0, 50, 10, true);
                map.AddTBString(FlowAttr.FlowNoteExp, null, "备注表达式", true, false, 0, 500, 10, true);

                //部门权限控制类型,此属性在报表中控制的.
                map.AddTBInt(FlowAttr.DRCtrlType, 0, "部门查询权限控制方式", true, false);

                #region 流程启动限制
                map.AddTBInt(FlowAttr.StartLimitRole, 0, "启动限制规则", true, false);
                map.AddTBString(FlowAttr.StartLimitPara, null, "规则内容", true, false, 0, 500, 10, true);
                map.AddTBString(FlowAttr.StartLimitAlert, null, "限制提示", true, false, 0, 500, 10, false);
                map.AddTBInt(FlowAttr.StartLimitWhen, 0, "提示时间", true, false);
                #endregion 流程启动限制

                #region 导航方式。
                map.AddTBInt(FlowAttr.StartGuideWay, 0, "前置导航方式", false, false);

                map.AddTBString(FlowAttr.StartGuidePara1, null, "参数1", true, false, 0, 500, 10, true);
                map.AddTBString(FlowAttr.StartGuidePara2, null, "参数2", true, false, 0, 500, 10, true);
                map.AddTBString(FlowAttr.StartGuidePara3, null, "参数3", true, false, 0, 500, 10, true);
                map.AddTBInt(FlowAttr.IsResetData, 0, "是否启用数据重置按钮？", true, false);
                //    map.AddTBInt(FlowAttr.IsImpHistory, 0, "是否启用导入历史数据按钮？", true, false);
                map.AddTBInt(FlowAttr.IsLoadPriData, 0, "是否导入上一个数据？", true, false);
                #endregion 导航方式。

                map.AddTBInt(FlowAttr.CFlowWay, 0, "延续流程方式", true, false);
                map.AddTBString(FlowAttr.CFlowPara, null, "延续流程参数", true, false, 0, 100, 10, true);

                //批量发起 add 2013-12-27. 
                map.AddTBInt(FlowAttr.IsBatchStart, 0, "是否可以批量发起", true, false);
                map.AddTBString(FlowAttr.BatchStartFields, null, "批量发起字段(用逗号分开)", true, false, 0, 500, 10, true);

                // map.AddTBInt(FlowAttr.IsEnableTaskPool, 0, "是否启用共享任务池", true, false);
                //map.AddDDLSysEnum(FlowAttr.TimelineRole, (int)TimelineRole.ByNodeSet, "时效性规则",
                // true, true, FlowAttr.TimelineRole, "@0=按节点(由节点属性来定义)@1=按发起人(开始节点SysSDTOfFlow字段计算)");

                map.AddTBInt(FlowAttr.IsAutoSendSubFlowOver, 0, "(当前节点为子流程时)是否检查所有子流程完成后父流程自动发送", true, true);
                map.AddTBString(FlowAttr.Ver, null, "版本号", true, true, 0, 20, 10);
                //设计类型 .
                map.AddTBInt(FlowAttr.DType, 0, "设计类型0=ccbpm,1=bpmn", true, false);

                //参数.
                map.AddTBAtParas(1000);


                #region 数据同步方案
                //数据同步方式.
                map.AddTBInt(FlowAttr.DTSWay, (int)FlowDTSWay.None, "同步方式", true, true);
                map.AddTBString(FlowAttr.DTSDBSrc, null, "数据源", true, false, 0, 200, 100, false);
                map.AddTBString(FlowAttr.DTSBTable, null, "业务表名", true, false, 0, 200, 100, false);
                map.AddTBString(FlowAttr.DTSBTablePK, null, "业务表主键", false, false, 0, 32, 10);

                map.AddTBInt(FlowAttr.DTSTime, (int)FlowDTSTime.AllNodeSend, "执行同步时间点", true, true);
                map.AddTBString(FlowAttr.DTSSpecNodes, null, "指定的节点ID", true, false, 0, 200, 100, false);
                map.AddTBInt(FlowAttr.DTSField, (int)DTSField.SameNames, "要同步的字段计算方式", true, true);
                map.AddTBString(FlowAttr.DTSFields, null, "要同步的字段s,中间用逗号分开.", false, false, 0, 2000, 100, false);
                #endregion 数据同步方案

                // map.AddSearchAttr(FlowAttr.FK_FlowSort);
                // map.AddSearchAttr(FlowAttr.FlowRunWay);

                RefMethod rm = new RefMethod();
                rm.Title = "设计检查报告"; // "设计检查报告";
                rm.ToolTip = "检查流程设计的问题。";
                rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/Btn/Confirm.gif";
                rm.ClassMethodName = this.ToString() + ".DoCheck";
                rm.GroupName = "流程维护";
                map.AddRefMethod(rm);

                //   rm = new RefMethod();
                //rm.Title = this.ToE("ViewDef", "视图定义"); //"视图定义";
                //rm.Icon = "/WF/Img/Btn/View.gif";
                //rm.ClassMethodName = this.ToString() + ".DoDRpt";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "报表运行"; // "报表运行";
                rm.Icon = SystemConfig.CCFlowWebPath + "WF/Img/Btn/View.gif";
                rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                //rm.Icon = "/WF/Img/Btn/Table.gif";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = this.ToE("FlowDataOut", "数据转出定义");  //"数据转出定义";
                ////  rm.Icon = "/WF/Img/Btn/Table.gif";
                //rm.ToolTip = "在流程完成时间，流程数据转储存到其它系统中应用。";
                //rm.ClassMethodName = this.ToString() + ".DoExp";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "删除数据";
                //rm.Warning = "您确定要执行删除流程数据吗？";
                //rm.ToolTip = "清除历史流程数据。";
                //rm.ClassMethodName = this.ToString() + ".DoExp";
                //map.AddRefMethod(rm);

                //map.AttrsOfOneVSM.Add(new FlowStations(), new Stations(), FlowStationAttr.FK_Flow,
                //    FlowStationAttr.FK_Station, DeptAttr.Name, DeptAttr.No, "抄送岗位");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region  公共方法
        /// <summary>
        /// 设计数据转出
        /// </summary>
        /// <returns></returns>
        public string DoExp()
        {
            this.DoCheck();
            return Glo.CCFlowAppPath + "WF/Admin/Exp.aspx?CondType=0&FK_Flow=" + this.No;
            //PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Admin/Exp.aspx?CondType=0&FK_Flow=" + this.No, "单据", "cdsn", 800, 500, 210, 300);
            //return null;
        }
        /// <summary>
        /// 定义报表
        /// </summary>
        /// <returns></returns>
        public string DoDRpt()
        {
            this.DoCheck();
            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Admin/WFRpt.aspx?CondType=0&FK_Flow=" + this.No, "单据", "cdsn", 800, 500, 210, 300);
            return null;
        }
        /// <summary>
        /// 运行报表
        /// </summary>
        /// <returns></returns>
        public string DoOpenRpt()
        {
            return null;
        }
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

            string sql = "  where FK_Node in (SELECT NodeID FROM WF_Node WHERE fk_flow='" + this.No + "')";
            string sql1 = " where NodeID in (SELECT NodeID FROM WF_Node WHERE fk_flow='" + this.No + "')";

            // DBAccess.RunSQL("DELETE FROM WF_CHOfFlow WHERE FK_Flow='" + this.No + "'");

            DBAccess.RunSQL("DELETE FROM WF_Bill WHERE FK_Flow='" + this.No + "'");
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE FK_Flow='" + this.No + "'");
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FK_Flow='" + this.No + "'");

            DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FK_Flow='" + this.No + "'");

            string sqlIn = " WHERE ReturnNode IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            DBAccess.RunSQL("DELETE FROM WF_ReturnWork " + sqlIn);
            DBAccess.RunSQL("DELETE FROM WF_GenerFH WHERE FK_Flow='" + this.No + "'");
            DBAccess.RunSQL("DELETE FROM WF_SelectAccper " + sql);
            DBAccess.RunSQL("DELETE FROM WF_TransferCustom " + sql);
            // DBAccess.RunSQL("DELETE FROM WF_FileManager " + sql);
            DBAccess.RunSQL("DELETE FROM WF_RememberMe " + sql);

            if (DBAccess.IsExitsObject("ND" + int.Parse(this.No) + "Track"))
                DBAccess.RunSQL("DELETE FROM ND" + int.Parse(this.No) + "Track ");

            if (DBAccess.IsExitsObject(this.PTable))
                DBAccess.RunSQL("DELETE FROM " + this.PTable);

            //DBAccess.RunSQL("DELETE FROM WF_WorkList WHERE FK_Flow='" + this.No + "'");
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
        public static Flow DoLoadFlowTemplate(string fk_flowSort, string path, ImpFlowTempleteModel model, int SpecialFlowNo = -1)
        {
            FileInfo info = new FileInfo(path);
            DataSet ds = new DataSet();
            ds.ReadXml(path);

            if (ds.Tables.Contains("WF_Flow") == false)
                throw new Exception("导入错误，非流程模版文件"+path+"。");

            DataTable dtFlow = ds.Tables["WF_Flow"];
            Flow fl = new Flow();
            string oldFlowNo = dtFlow.Rows[0]["No"].ToString();
            string oldFlowName = dtFlow.Rows[0]["Name"].ToString();

            int oldFlowID = int.Parse(oldFlowNo);
            string timeKey = DateTime.Now.ToString("yyMMddhhmmss");

            //判断流程标示.多个流程是否可以共用一个类，注释
            //if (dtFlow.Columns.Contains("FlowMark") == true)
            //{
            //    string FlowMark = dtFlow.Rows[0]["FlowMark"].ToString();
            //    if (string.IsNullOrEmpty(FlowMark) == false)
            //    {
            //        if (fl.IsExit(FlowAttr.FlowMark, FlowMark))
            //            throw new Exception("@该流程标示:" + FlowMark + "已经存在于系统中,您不能导入.");
            //    }
            //}

            switch (model)
            {
                case ImpFlowTempleteModel.AsNewFlow: /*做为一个新流程. */
                    fl.No = fl.GenerNewNo;
                    fl.DoDelData();
                    fl.DoDelete(); /*删除可能存在的垃圾.*/
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
                    }
                    break;
                case ImpFlowTempleteModel.OvrewaiteCurrFlowNo: /*覆盖当前的流程.*/
                    fl.No = oldFlowNo;
                    fl.DoDelData();
                    fl.DoDelete(); /*删除可能存在的垃圾.*/
                    break;
                case ImpFlowTempleteModel.AsSpecFlowNo:
                    if (SpecialFlowNo <= 0)
                    {
                        throw new Exception("@您是按照指定的流程编号导入的，但是您没有传入正确的流程编号。");
                    }
                    fl.No = SpecialFlowNo.ToString();
                    fl.DoDelData();
                    fl.DoDelete(); /*删除可能存在的垃圾.*/
                    break;
                default:
                    throw new Exception("@没有判断");
            }

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
            fl.Insert();
            #endregion 处理流程表数据

            #region 处理OID 插入重复的问题 Sys_GroupField, Sys_MapAttr.
            DataTable mydtGF = ds.Tables["Sys_GroupField"];
            DataTable myDTAttr = ds.Tables["Sys_MapAttr"];
            DataTable myDTAth = ds.Tables["Sys_FrmAttachment"];
            DataTable myDTDtl = ds.Tables["Sys_MapDtl"];
            DataTable myDFrm = ds.Tables["Sys_MapFrame"];
            DataTable myDM2M = ds.Tables["Sys_MapM2M"];
            if (mydtGF != null)
            {
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
                    dr["OID"] = gf.OID;

                    // 属性。
                    if (myDTAttr != null && myDTAttr.Columns.Contains("GroupID"))
                    {
                        foreach (DataRow dr1 in myDTAttr.Rows)
                        {
                            if (dr1["GroupID"] == null)
                                dr1["GroupID"] = 0;

                            if (dr1["GroupID"].ToString() == oldID.ToString())
                                dr1["GroupID"] = gf.OID;
                        }
                    }

                    if (myDTAth != null && myDTAth.Columns.Contains("GroupID"))
                    {
                        // 附件。
                        foreach (DataRow dr1 in myDTAth.Rows)
                        {
                            if (dr1["GroupID"] == null)
                                dr1["GroupID"] = 0;

                            if (dr1["GroupID"].ToString() == oldID.ToString())
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

                            if (dr1["GroupID"].ToString() == oldID.ToString())
                                dr1["GroupID"] = gf.OID;
                        }
                    }

                    if (myDFrm != null && myDFrm.Columns.Contains("GroupID"))
                    {
                        // frm.
                        foreach (DataRow dr1 in myDFrm.Rows)
                        {
                            if (dr1["GroupID"] == null)
                                dr1["GroupID"] = 0;

                            if (dr1["GroupID"].ToString() == oldID.ToString())
                                dr1["GroupID"] = gf.OID;
                        }
                    }

                    if (myDM2M != null && myDM2M.Columns.Contains("GroupID"))
                    {
                        // m2m.
                        foreach (DataRow dr1 in myDM2M.Rows)
                        {
                            if (dr1["GroupID"] == null)
                                dr1["GroupID"] = 0;

                            if (dr1["GroupID"].ToString() == oldID.ToString())
                                dr1["GroupID"] = gf.OID;
                        }
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
                    case "WF_FlowFormTree": //独立表单目录 add 2013-12-03
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
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
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
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
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
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
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
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
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
                                File.Copy(info.DirectoryName + "\\" + no + ".rtf", BP.Sys.SystemConfig.PathOfWebApp + @"\DataUser\CyclostyleFile\" + bt.No + ".rtf", true);
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
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
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
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
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
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
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
                                cd.MyPK = cd.FK_Node + "_" + cd.ToNodeID + "_" + cd.HisCondType.ToString() + "_" + ConnDataFrom.Stas.ToString();
                            }
                            else if (cd.MyPK.Contains("Dept"))
                            {
                                cd.MyPK = cd.FK_Node + "_" + cd.ToNodeID + "_" + cd.HisCondType.ToString() + "_" + ConnDataFrom.Depts.ToString();
                            }
                            else if (cd.MyPK.Contains("Paras"))
                            {
                                cd.MyPK = cd.FK_Node + "_" + cd.ToNodeID + "_" + cd.HisCondType.ToString() + "_" + ConnDataFrom.Paras.ToString();
                            }
                            else if (cd.MyPK.Contains("Url"))
                            {
                                cd.MyPK = cd.FK_Node + "_" + cd.ToNodeID + "_" + cd.HisCondType.ToString() + "_" + ConnDataFrom.Url.ToString();
                            }
                            else if (cd.MyPK.Contains("SQL"))
                            {
                                cd.MyPK = cd.FK_Node + "_" + cd.ToNodeID + "_" + cd.HisCondType.ToString() + "_" + ConnDataFrom.SQL;
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
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
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
                                    case "returnn":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
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
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
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
                    case "WF_TurnTo": //转向规则.
                        foreach (DataRow dr in dt.Rows)
                        {
                            TurnTo fs = new TurnTo();

                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    default:
                                        break;
                                }
                                fs.SetValByKey(dc.ColumnName, val);
                            }
                            fs.FK_Flow = fl.No;
                            fs.Save();
                        }
                        break;
                    case "WF_FAppSet": //FAppSets.xml。
                        continue;
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
                            NodeDept dir = new NodeDept();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    default:
                                        break;
                                }
                                dir.SetValByKey(dc.ColumnName, val);
                            }
                            dir.Insert();
                        }
                        break;
                    case "WF_Node": //导入节点信息.
                        foreach (DataRow dr in dt.Rows)
                        {
                            BP.WF.Template.NodeSheet nd = new BP.WF.Template.NodeSheet();

                            BP.WF.Template.CC cc = new CC(); // 抄送相关的信息.
                            BP.WF.Template.FrmWorkCheck fwc = new FrmWorkCheck();

                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                //  NodeAttr.NodeFrmID
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "nodefrmid":
                                        if (val.Length == 5)
                                            val = "ND" + flowID + val.Substring(3);
                                        else if (val.Length == 6)
                                            val = "ND" + flowID + val.Substring(4);
                                        else if (val.Length == 7)
                                            val = "ND" + flowID + val.Substring(5);
                                        break;
                                    case "nodeid":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    case "fk_flow":
                                    case "fk_flowsort":
                                        continue;
                                    case "showsheets":
                                    case "histonds":
                                    case "groupstands":
                                        string key = "@" + flowID;
                                        val = val.Replace(key, "");
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
                                if (nd.GetValStringByKey("OfficePrintEnable") == "打印")
                                    nd.SetValByKey("OfficePrintEnable", 0);

                                nd.DirectInsert();

                                //把抄送的信息也导入里面去.
                                cc.DirectUpdate();
                                fwc.DirectUpdate();
                                DBAccess.RunSQL("DELETE FROM Sys_MapAttr WHERE FK_MapData='ND" + nd.NodeID + "'");
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("@导入节点:FlowName:" + nd.FlowName + " nodeID: " + nd.NodeID + " , " + nd.Name + " 错误:" + ex.Message);
                            }

                            //删除mapdata.
                        }

                        foreach (DataRow dr in dt.Rows)
                        {
                            Node nd = new Node();
                            nd.NodeID = int.Parse(dr[NodeAttr.NodeID].ToString());
                            nd.RetrieveFromDBSources();
                            nd.FK_Flow = fl.No;
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "nodefrmid":
                                        if (val.Length == 5)
                                            val = "ND" + flowID + val.Substring(3);
                                        else if (val.Length == 6)
                                            val = "ND" + flowID + val.Substring(4);
                                        else if (val.Length == 7)
                                            val = "ND" + flowID + val.Substring(5);
                                        break;
                                    case "nodeid":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    case "fk_flow":
                                    case "fk_flowsort":
                                        continue;
                                    case "showsheets":
                                    case "histonds":
                                    case "groupstands":
                                        string key = "@" + flowID;
                                        val = val.Replace(key, "");
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
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    default:
                                        break;
                                }
                                ns.SetValByKey(dc.ColumnName, val);
                            }
                            ns.Insert();
                        }
                        break;
                    case "WF_Listen": // 信息侦听。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Listen li = new Listen();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "oid":
                                        continue;
                                        break;
                                    case "fk_node":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    case "nodes":
                                        string[] nds = val.Split('@');
                                        string valExt = "";
                                        foreach (string nd in nds)
                                        {
                                            if (nd == "" || nd == null)
                                                continue;
                                            string ndExt = nd.Clone() as string;
                                            if (ndExt.Length == 3)
                                                ndExt = flowID + ndExt.Substring(1);
                                            else if (val.Length == 4)
                                                ndExt = flowID + ndExt.Substring(2);
                                            else if (val.Length == 5)
                                                ndExt = flowID + ndExt.Substring(3);
                                            ndExt = "@" + ndExt;
                                            valExt += ndExt;
                                        }
                                        val = valExt;
                                        break;
                                    default:
                                        break;
                                }
                                li.SetValByKey(dc.ColumnName, val);
                            }
                            li.Insert();
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
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + int.Parse(fl.No));
                                md.SetValByKey(dc.ColumnName, val);
                            }
                            md.Save();
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
                    case "Sys_FrmEle":
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmEle en = new FrmEle();
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

                            try
                            {
                                en.MyPK = "I" + timeKey + "_" + idx;
                                en.Insert();
                            }
                            catch
                            {
                                en.MyPK = Guid.NewGuid().ToString();
                                en.Insert();
                            }
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

                            //en.MyPK = Guid.NewGuid().ToString();
                            // 出现重复的
                            try
                            {
                                en.MyPK = "Lab" + timeKey + "_" + idx;
                                en.Insert();
                            }
                            catch
                            {
                                en.MyPK = Guid.NewGuid().ToString();
                                en.Insert();
                            }
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
                            //en.MyPK = "LK" + timeKey + "_" + idx;
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
                    case "Sys_MapM2M": //Sys_MapM2M.
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            MapM2M en = new MapM2M();
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
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    default:
                                        break;
                                }
                                ne.SetValByKey(dc.ColumnName, val);
                            }
                            ne.Insert();
                        }
                        break;
                    case "Sys_GroupField": // 
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
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                    default:
                                        break;
                                }
                                gf.SetValByKey(dc.ColumnName, val);
                            }
                            //  int oid = DBAccess.GenerOID();
                            //  DBAccess.RunSQL("UPDATE Sys_MapAttr SET GroupID=" + gf.OID + " WHERE FK_MapData='" + gf.EnName + "' AND GroupID=" + gf.OID);
                            gf.InsertAsOID(gf.OID);
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
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
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
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
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
            DBAccess.RunSQL("UPDATE WF_Cond SET FK_Node=NodeID WHERE FK_Node=0");
            DBAccess.RunSQL("UPDATE WF_Cond SET ToNodeID=NodeID WHERE ToNodeID=0");

            DBAccess.RunSQL("DELETE FROM WF_Cond WHERE NodeID NOT IN (SELECT NodeID FROM WF_Node)");
            DBAccess.RunSQL("DELETE FROM WF_Cond WHERE ToNodeID NOT IN (SELECT NodeID FROM WF_Node) ");
            DBAccess.RunSQL("DELETE FROM WF_Cond WHERE FK_Node NOT IN (SELECT NodeID FROM WF_Node) AND FK_Node > 0");
            #endregion

            if (infoErr == "")
            {
                infoTable = "";
                return fl; // "完全成功。";
            }

            infoErr = "@执行期间出现如下非致命的错误：\t\r" + infoErr + "@ " + infoTable;
            throw new Exception(infoErr);

        }
        public Node DoNewNode(int x, int y)
        {
            Node nd = new Node();
            int idx = this.HisNodes.Count;
            if (idx == 0)
                idx++;

            while (true)
            {
                string strID = this.No + idx.ToString().PadLeft(2, '0');
                nd.NodeID = int.Parse(strID);
                if (!nd.IsExits)
                    break;
                idx++;
            }

            nd.HisNodeWorkType = NodeWorkType.Work;
            nd.Name = "节点" + idx;
            nd.HisNodePosType = NodePosType.Mid;
            nd.FK_Flow = this.No;
            nd.FlowName = this.Name;
            nd.X = x;
            nd.Y = y;
            nd.Step = idx;
            nd.Insert();

            nd.CreateMap();
            return nd;
        }
        /// <summary>
        /// 执行新建
        /// </summary>
        /// <param name="flowSort">类别</param>
        /// <param name="flowName">流程名称</param>
        /// <param name="model">数据存储模式</param>
        /// <param name="pTable">数据存储物理表</param>
        /// <param name="FlowMark">流程标记</param>
        public string DoNewFlow(string flowSort, string flowName,
            DataStoreModel model, string pTable, string FlowMark)
        {
            try
            {
                //检查参数的完整性.
                if (string.IsNullOrEmpty(pTable) == false && pTable.Length >= 1)
                {
                    string c = pTable.Substring(0, 1);
                    if (DataType.IsNumStr(c) == true)
                        throw new Exception("@非法的流程数据表(" + pTable + "),它会导致ccflow不能创建该表.");
                }

                this.Name = flowName;
                if (string.IsNullOrWhiteSpace(this.Name))
                    this.Name = "新建流程" + this.No; //新建流程.

                this.No = this.GenerNewNoByKey(FlowAttr.No);
                this.HisDataStoreModel = model;
                this.PTable = pTable;
                this.FK_FlowSort = flowSort;
                this.FlowMark = FlowMark;

                if (string.IsNullOrEmpty(FlowMark) == false)
                {
                    if (this.IsExit(FlowAttr.FlowMark, FlowMark))
                        throw new Exception("@该流程标示:" + FlowMark + "已经存在于系统中.");
                }

                /*给初始值*/
                //this.Paras = "@StartNodeX=10@StartNodeY=15@EndNodeX=40@EndNodeY=10";
                this.Paras = "@StartNodeX=200@StartNodeY=50@EndNodeX=200@EndNodeY=350";
                this.Save();

                #region 删除有可能存在的历史数据.
                Flow fl = new Flow(this.No);
                fl.DoDelData();
                fl.DoDelete();

                this.Save();
                #endregion 删除有可能存在的历史数据.

                Node nd = new Node();
                nd.NodeID = int.Parse(this.No + "01");
                nd.Name = "开始节点";//  "开始节点"; 
                nd.Step = 1;
                nd.FK_Flow = this.No;
                nd.FlowName = this.Name;
                nd.HisNodePosType = NodePosType.Start;
                nd.HisNodeWorkType = NodeWorkType.StartWork;
                nd.X = 200;
                nd.Y = 150;
                nd.ICON = "前台";
                nd.Insert();

                nd.CreateMap();
                nd.HisWork.CheckPhysicsTable();

                nd = new Node();
                nd.NodeID = int.Parse(this.No + "02");
                nd.Name = "节点2"; // "结束节点";
                nd.Step = 2;
                nd.FK_Flow = this.No;
                nd.FlowName = this.Name;
                nd.HisNodePosType = NodePosType.Mid;
                nd.HisNodeWorkType = NodeWorkType.Work;
                nd.X = 200;
                nd.Y = 250;
                nd.ICON = "审核";
                nd.Insert();
                nd.CreateMap();
                nd.HisWork.CheckPhysicsTable();

                BP.Sys.MapData md = new BP.Sys.MapData();
                md.No = "ND" + int.Parse(this.No) + "Rpt";
                md.Name = this.Name;
                md.Save();

                // 装载模版.
                string file = BP.Sys.SystemConfig.PathOfDataUser + "XML\\TempleteSheetOfStartNode.xml";
                if (System.IO.File.Exists(file))
                {
                    /*如果存在开始节点表单模版*/
                    DataSet ds = new DataSet();
                    ds.ReadXml(file);

                    string nodeID = "ND" + int.Parse(this.No + "01");
                    BP.Sys.MapData.ImpMapData(nodeID, ds, false);
                }
                else
                {
                    #region 生成CCForm 的装饰.
                    FrmImg img = new FrmImg();
                    img.MyPK = "Img" + DateTime.Now.ToString("yyMMddhhmmss") + WebUser.No;
                    img.FK_MapData = "ND" + int.Parse(this.No + "01");
                    img.X = (float)577.26;
                    img.Y = (float)3.45;

                    img.W = (float)137;
                    img.H = (float)40;

                    img.ImgURL = "/ccform;component/Img/LogoBig.png";
                    img.LinkURL = "http://ccflow.org";
                    img.LinkTarget = "_blank";
                    img.Insert();

                    FrmLab lab = new FrmLab();

                    lab = new FrmLab();
                    lab.MyPK = "Lab" + DateTime.Now.ToString("yyMMddhhmmss") + WebUser.No + 2;
                    lab.Text = "发起人";
                    lab.FK_MapData = "ND" + int.Parse(this.No + "01");
                    lab.X = (float)106.48;
                    lab.Y = (float)96.08;
                    lab.FontSize = 11;
                    lab.FontColor = "black";
                    lab.FontName = "Portable User Interface";
                    lab.FontStyle = "Normal";
                    lab.FontWeight = "normal";
                    lab.Insert();

                    lab = new FrmLab();
                    lab.MyPK = "Lab" + DateTime.Now.ToString("yyMMddhhmmss") + WebUser.No + 3;
                    lab.Text = "发起时间";
                    lab.FK_MapData = "ND" + int.Parse(this.No + "01");
                    lab.X = (float)307.64;
                    lab.Y = (float)95.17;

                    lab.FontSize = 11;
                    lab.FontColor = "black";
                    lab.FontName = "Portable User Interface";
                    lab.FontStyle = "Normal";
                    lab.FontWeight = "normal";
                    lab.Insert();

                    lab = new FrmLab();
                    lab.MyPK = "Lab" + DateTime.Now.ToString("yyMMddhhmmss") + WebUser.No + 4;
                    lab.Text = "新建节点(请修改标题)";
                    lab.FK_MapData = "ND" + int.Parse(this.No + "01");

                    lab.X = (float)294.67;
                    lab.Y = (float)8.27;

                    lab.FontSize = 23;
                    lab.FontColor = "Blue";
                    lab.FontName = "Portable User Interface";
                    lab.FontStyle = "Normal";
                    lab.FontWeight = "normal";
                    lab.Insert();

                    lab = new FrmLab();
                    lab.MyPK = "Lab" + DateTime.Now.ToString("yyMMddhhmmss") + WebUser.No + 5;
                    lab.Text = "说明:以上内容是ccflow自动产生的，您可以修改/删除它。@为了更方便您的设计您可以到http://ccflow.org官网下载表单模板.";
                    lab.Text += "@因为当前技术问题与silverlight开发工具使用特别说明如下:@";
                    lab.Text += "@1,改变控件位置: ";
                    lab.Text += "@  所有的控件都支持 wasd, 做为方向键用来移动控件的位置， 部分控件支持方向键. ";
                    lab.Text += "@2, 增加textbox, 从表, dropdownlistbox, 的宽度 shift+ -> 方向键增加宽度 shift + <- 减小宽度.";
                    lab.Text += "@3, 保存 windows键 + s.  删除 delete.  复制 ctrl+c   粘帖: ctrl+v.";
                    lab.Text += "@4, 支持全选，批量移动， 批量放大缩小字体., 批量改变线的宽度.";
                    lab.Text += "@5, 改变线的长度： 选择线，点绿色的圆点，拖拉它。.";
                    lab.Text += "@6, 放大或者缩小　label 的字体 , 选择一个多个label , 按 A+ 或者　A－　按钮.";
                    lab.Text += "@7, 改变线或者标签的颜色， 选择操作对象，点工具栏上的调色板.";

                    lab.X = (float)168.24;
                    lab.Y = (float)163.7;
                    lab.FK_MapData = "ND" + int.Parse(this.No + "01");
                    lab.FontSize = 11;
                    lab.FontColor = "Red";
                    lab.FontName = "Portable User Interface";
                    lab.FontStyle = "Normal";
                    lab.FontWeight = "normal";
                    lab.Insert();

                    string key = "L" + DateTime.Now.ToString("yyMMddhhmmss") + WebUser.No;
                    FrmLine line = new FrmLine();
                    line.MyPK = key + "_1";
                    line.FK_MapData = "ND" + int.Parse(this.No + "01");
                    line.X1 = (float)281.82;
                    line.Y1 = (float)81.82;
                    line.X2 = (float)281.82;
                    line.Y2 = (float)121.82;
                    line.BorderWidth = (float)2;
                    line.BorderColor = "Black";
                    line.Insert();

                    line.MyPK = key + "_2";
                    line.FK_MapData = "ND" + int.Parse(this.No + "01");
                    line.X1 = (float)360;
                    line.Y1 = (float)80.91;
                    line.X2 = (float)360;
                    line.Y2 = (float)120.91;
                    line.BorderWidth = (float)2;
                    line.BorderColor = "Black";
                    line.Insert();

                    line.MyPK = key + "_3";
                    line.FK_MapData = "ND" + int.Parse(this.No + "01");
                    line.X1 = (float)158.82;
                    line.Y1 = (float)41.82;
                    line.X2 = (float)158.82;
                    line.Y2 = (float)482.73;
                    line.BorderWidth = (float)2;
                    line.BorderColor = "Black";
                    line.Insert();

                    line.MyPK = key + "_4";
                    line.FK_MapData = "ND" + int.Parse(this.No + "01");
                    line.X1 = (float)81.55;
                    line.Y1 = (float)80;
                    line.X2 = (float)718.82;
                    line.Y2 = (float)80;
                    line.BorderWidth = (float)2;
                    line.BorderColor = "Black";
                    line.Insert();


                    line.MyPK = key + "_5";
                    line.FK_MapData = "ND" + int.Parse(this.No + "01");
                    line.X1 = (float)81.82;
                    line.Y1 = (float)40;
                    line.X2 = (float)81.82;
                    line.Y2 = (float)480.91;
                    line.BorderWidth = (float)2;
                    line.BorderColor = "Black";
                    line.Insert();

                    line.MyPK = key + "_6";
                    line.FK_MapData = "ND" + int.Parse(this.No + "01");
                    line.X1 = (float)81.82;
                    line.Y1 = (float)481.82;
                    line.X2 = (float)720;
                    line.Y2 = (float)481.82;
                    line.BorderWidth = (float)2;
                    line.BorderColor = "Black";
                    line.Insert();

                    line.MyPK = key + "_7";
                    line.FK_MapData = "ND" + int.Parse(this.No + "01");
                    line.X1 = (float)83.36;
                    line.Y1 = (float)40.91;
                    line.X2 = (float)717.91;
                    line.Y2 = (float)40.91;
                    line.BorderWidth = (float)2;
                    line.BorderColor = "Black";
                    line.Insert();

                    line.MyPK = key + "_8";
                    line.FK_MapData = "ND" + int.Parse(this.No + "01");
                    line.X1 = (float)83.36;
                    line.Y1 = (float)120.91;
                    line.X2 = (float)717.91;
                    line.Y2 = (float)120.91;
                    line.BorderWidth = (float)2;
                    line.BorderColor = "Black";
                    line.Insert();

                    line.MyPK = key + "_9";
                    line.FK_MapData = "ND" + int.Parse(this.No + "01");
                    line.X1 = (float)719.09;
                    line.Y1 = (float)40;
                    line.X2 = (float)719.09;
                    line.Y2 = (float)482.73;
                    line.BorderWidth = (float)2;
                    line.BorderColor = "Black";
                    line.Insert();
                    #endregion
                }

                //写入权限.
                WritToGPM(flowSort);

                this.DoCheck_CheckRpt(this.HisNodes);
                //  Flow.RepareV_FlowData_View();
                return this.No;
            }
            catch (Exception ex)
            {
                ///删除垃圾数据.
                this.DoDelete();

                //提示错误.
                throw new Exception("创建流程错误:" + ex.Message);
            }


        }

        /// <summary>
        /// 写入权限
        /// </summary>
        /// <param name="flowSort"></param>
        public void WritToGPM(string flowSort)
        {

            return;

            #region 写入权限管理
            if (Glo.OSModel == OSModel.OneMore)
            {
                string sql = "";

                try
                {
                    sql = "DELETE FROM GPM_Menu WHERE FK_App='" + SystemConfig.SysNo + "' AND Flag='Flow" + this.No + "'";
                    DBAccess.RunSQL(sql);
                }
                catch
                {
                }

                // 开始组织发起流程的数据.
                // 取得该流程的目录编号.
                sql = "SELECT No FROM GPM_Menu WHERE Flag='FlowSort" + flowSort + "' AND FK_App='" + BP.Sys.SystemConfig.SysNo + "'";
                string parentNoOfMenu = DBAccess.RunSQLReturnStringIsNull(sql, null);
                if (parentNoOfMenu == null)
                    throw new Exception("@没有找到该流程的(" + BP.Sys.SystemConfig.SysNo + ")目录在GPM系统中,请重新新建此目录。");

                // 取得该功能的主键编号.
                string treeNo = DBAccess.GenerOID("BP.GPM.Menu").ToString();

                // 插入流程名称.
                string url = SystemConfig.CCFlowWebPath + "WF/MyFlow.aspx?FK_Flow=" + this.No + "&FK_Node=" + int.Parse(this.No) + "01";

                sql = "INSERT INTO GPM_Menu(No,Name,ParentNo,IsDir,MenuType,FK_App,IsEnable,Flag,Url)";
                sql += " VALUES('{0}','{1}','{2}',{3},{4},'{5}',{6},'{7}','{8}')";
                sql = string.Format(sql, treeNo, this.Name, parentNoOfMenu, 0, 4, SystemConfig.SysNo, 1, "Flow" + this.No, url);
                DBAccess.RunSQL(sql);
            }
            #endregion
        }
        /// <summary>
        /// 检查报表
        /// </summary>
        public void CheckRpt()
        {
            this.DoCheck_CheckRpt(this.HisNodes);
        }
        /// <summary>
        /// 更新之前做检查
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdate()
        {
            this.Ver = BP.DA.DataType.CurrentDataTimess;
            Node.CheckFlow(this);
            return base.beforeUpdate();
        }

        /// <summary>
        /// 更新版本号
        /// </summary>
        public static void UpdateVer(string flowNo)
        {
            string sql = "UPDATE WF_Flow SET VER='" + BP.DA.DataType.CurrentDataTimess + "' WHERE No='" + flowNo + "'";
            DBAccess.RunSQL(sql);
        }
        public void DoDelete()
        {
            //删除流程数据.
            this.DoDelData();

            string sql = "";
            //sql = " DELETE FROM WF_chofflow WHERE FK_Flow='" + this.No + "'";
            sql += "@ DELETE  FROM WF_GenerWorkerlist WHERE FK_Flow='" + this.No + "'";
            sql += "@ DELETE FROM  WF_GenerWorkFlow WHERE FK_Flow='" + this.No + "'";
            sql += "@ DELETE FROM  WF_Cond WHERE FK_Flow='" + this.No + "'";

            // 删除岗位节点。
            sql += "@ DELETE  FROM  WF_NodeStation WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            // 删除方向。
            sql += "@ DELETE FROM  WF_Direction WHERE FK_Flow='" + this.No + "'";

            //删除节点绑定信息.
            sql += "@ DELETE FROM WF_FrmNode  WHERE   FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

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
            sql += "@ DELETE FROM WF_TurnTo WHERE   FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";


            //删除侦听.
            sql += "@ DELETE FROM WF_Listen WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            // 删除d2d数据.
            //  sql += "@GO DELETE WF_M2M WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            //// 删除配置.
            //sql += "@ DELETE FROM WF_FAppSet WHERE NodeID IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            // 删除配置.
            sql += "@ DELETE FROM WF_FlowEmp WHERE FK_Flow='" + this.No + "' ";

            //// 外部程序设置
            //sql += "@ DELETE FROM WF_FAppSet WHERE  NodeID in (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            //删除单据.
            sql += "@ DELETE FROM WF_BillTemplate WHERE  NodeID in (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            //删除权限控制.
            sql += "@ DELETE FROM Sys_FrmSln WHERE FK_Flow='" + this.No + "'";

            Nodes nds = new Nodes(this.No);
            foreach (Node nd in nds)
            {
                // 删除节点所有相关的东西.
                sql += "@ DELETE  FROM Sys_MapM2M WHERE FK_MapData='ND" + nd.NodeID + "'";
                nd.Delete();
            }

            sql += "@ DELETE  FROM WF_Node WHERE FK_Flow='" + this.No + "'";
            sql += "@ DELETE  FROM  WF_LabNote WHERE FK_Flow='" + this.No + "'";

            //删除分组信息
            sql += "@ DELETE FROM Sys_GroupField WHERE EnName NOT IN(SELECT NO FROM Sys_MapData)";

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
            this.Delete(); //删除需要移除缓存.

            // Flow.RepareV_FlowData_View();

            ////删除权限管理
            //if (BP.WF.Glo.OSModel == OSModel.OneMore)
            //{
            //    try
            //    {
            //        DBAccess.RunSQL("DELETE FROM GPM_Menu WHERE Flag='Flow" + this.No + "' AND FK_App='" + SystemConfig.SysNo + "'");
            //    }
            //    catch
            //    {
            //    }
            //}
        }
        #endregion
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
        /// 转化成list
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

