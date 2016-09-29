using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.En;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.Port;
using System.Drawing.Imaging;
using System.Drawing;
using System.Configuration;
using BP.Tools;


namespace BP.WF
{
    public class CCFlowAPI
    {
        /// <summary>
        /// 产生一个WorkNode
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="fid">FID</param>
        /// <param name="userNo">用户编号</param>
        /// <returns>返回dataset</returns>
        public static DataSet GenerWorkNode(string fk_flow, int fk_node, Int64 workID, Int64 fid, string userNo)
        {
            if (fk_node == 0)
                fk_node = int.Parse(fk_flow + "01");

            if (workID == 0)
                workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow, null, null, userNo, null);

            try
            {
                Emp emp = new Emp(userNo);
                BP.Web.WebUser.SignInOfGener(emp);

                MapData md = new MapData();
                md.No = "ND" + fk_node;
                if (md.RetrieveFromDBSources() == 0)
                    throw new Exception("装载错误，该表单ID=" + md.No + "丢失，请修复一次流程重新加载一次.");

                //获得表单模版.
                DataSet myds = BP.Sys.CCFormAPI.GenerHisDataSet(md.No,true);

                #region 流程设置信息.
                Node nd = new Node(fk_node);
                if (nd.IsStartNode == false)
                    BP.WF.Dev2Interface.Node_SetWorkRead(fk_node, workID);

                //// 节点数据.
                //string sql = "SELECT * FROM WF_Node WHERE NodeID=" + fk_node;
                //DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                //dt.TableName = "WF_NodeBar";
                //myds.Tables.Add(dt);

                //// 流程数据.
                //Flow fl = new Flow(fk_flow);
                //myds.Tables.Add(fl.ToDataTableField("WF_Flow"));
                #endregion 流程设置信息.
                

                #region 把主从表数据放入里面.
                //.工作数据放里面去, 放进去前执行一次装载前填充事件.
                BP.WF.Work wk = nd.HisWork;
                wk.OID = workID;
                wk.RetrieveFromDBSources();

                // 处理传递过来的参数。
                foreach (string k in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
                {
                    wk.SetValByKey(k, System.Web.HttpContext.Current.Request.QueryString[k]);
                }

                // 执行表单事件..
                string msg = md.FrmEvents.DoEventNode(FrmEventList.FrmLoadBefore, wk);
                if (string.IsNullOrEmpty(msg) == false)
                    throw new Exception("err@错误:" + msg);

                //重设默认值.
                wk.ResetDefaultVal();

                //执行装载填充》
                MapExt me = new MapExt();
                me.MyPK = wk.ToString() + "_" + MapExtXmlList.PageLoadFull;
                if (me.RetrieveFromDBSources() == 1)
                {
                    //执行通用的装载方法.
                    MapAttrs attrs = new MapAttrs("ND" + fk_node);
                    MapDtls dtls=new MapDtls( "ND"+fk_node);
                    wk = BP.WF.Glo.DealPageLoadFull(wk, me, attrs, dtls) as Work;
                }

                DataTable mainTable = wk.ToDataTableField(md.No);
                mainTable.TableName = "MainTable";
                myds.Tables.Add(mainTable);

                string sql = "";
                DataTable dt = null;

                //把附件的数据放入.
                if (md.FrmAttachments.Count > 0)
                {
                    sql = "SELECT * FROM Sys_FrmAttachmentDB where RefPKVal=" + workID + " AND FK_MapData='ND" + fk_node + "'";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "Sys_FrmAttachmentDB";
                    myds.Tables.Add(dt);
                }
                // 图片附件数据放入
                if (md.FrmImgAths.Count > 0)
                {
                    sql = "SELECT * FROM Sys_FrmImgAthDB where RefPKVal=" + workID + " AND FK_MapData='ND" + fk_node + "'";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "Sys_FrmImgAthDB";
                    myds.Tables.Add(dt);
                }

                //把从表的数据放入.
                if (md.MapDtls.Count > 0)
                {
                    foreach (MapDtl dtl in md.MapDtls)
                    {
                        GEDtls dtls = new GEDtls(dtl.No);
                        QueryObject qo = null;
                        try
                        {
                            qo = new QueryObject(dtls);
                            switch (dtl.DtlOpenType)
                            {
                                case DtlOpenType.ForEmp:  // 按人员来控制.
                                    qo.AddWhere(GEDtlAttr.RefPK, workID);
                                    qo.addAnd();
                                    qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                                    break;
                                case DtlOpenType.ForWorkID: // 按工作ID来控制
                                    qo.AddWhere(GEDtlAttr.RefPK, workID);
                                    break;
                                case DtlOpenType.ForFID: // 按流程ID来控制.
                                    qo.AddWhere(GEDtlAttr.FID, workID);
                                    break;
                            }
                        }
                        catch
                        {
                            dtls.GetNewEntity.CheckPhysicsTable();
                        }
                        DataTable dtDtl = qo.DoQueryToTable();

                        // 为明细表设置默认值.
                        MapAttrs dtlAttrs = new MapAttrs(dtl.No);
                        foreach (MapAttr attr in dtlAttrs)
                        {
                            //处理它的默认值.
                            if (attr.DefValReal.Contains("@") == false)
                                continue;

                            foreach (DataRow dr in dtDtl.Rows)
                                dr[attr.KeyOfEn] = attr.DefVal;
                        }

                        dtDtl.TableName = dtl.No; //修改明细表的名称.
                        myds.Tables.Add(dtDtl); //加入这个明细表, 如果没有数据，xml体现为空.
                    }
                }
                #endregion


                #region 把外键表加入DataSet
                DataTable dtMapAttr = myds.Tables["Sys_MapAttr"];

                MapExts mes = md.HisMapExts;

                foreach (DataRow dr in dtMapAttr.Rows)
                {
                    string lgType = dr["LGType"].ToString();
                    if (lgType != "2")
                        continue;

                    string UIIsEnable = dr["UIIsEnable"].ToString();
                    if (UIIsEnable == "0")
                        continue;

                    string uiBindKey = dr["UIBindKey"].ToString();
                    if (string.IsNullOrEmpty(uiBindKey) == true)
                    {
                        string myPK = dr["MyPK"].ToString();
                        /*如果是空的*/
                     //   throw new Exception("@属性字段数据不完整，流程:" + fl.No + fl.Name + ",节点:" + nd.NodeID + nd.Name + ",属性:" + myPK + ",的UIBindKey IsNull ");
                    }

                    // 判断是否存在.
                    if (myds.Tables.Contains(uiBindKey) == true)
                        continue;

                    // 检查是否有下拉框自动填充。
                    string keyOfEn = dr["KeyOfEn"].ToString();
                    string fk_mapData = dr["FK_MapData"].ToString();

                    #region 处理下拉框数据范围. for xiaoyang.
                    me = mes.GetEntityByKey(MapExtAttr.ExtType,  MapExtXmlList.AutoFullDLL, MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
                    if (me != null)
                    {
                        string fullSQL = me.Doc.Clone() as string;
                        fullSQL = fullSQL.Replace("~", ",");
                        fullSQL = BP.WF.Glo.DealExp(fullSQL, wk, null);
                        dt = DBAccess.RunSQLReturnTable(fullSQL);
                        dt.TableName = uiBindKey; //可能存在隐患，如果多个字段，绑定同一个表，就存在这样的问题.
                        myds.Tables.Add(dt);
                        continue;
                    }
                    #endregion 处理下拉框数据范围.

                    myds.Tables.Add(BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey));
                }
                #endregion End把外键表加入DataSet


                #region 把流程信息放入里面.
                //把流程信息表发送过去.
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = workID;
                gwf.RetrieveFromDBSources();

                //myds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

                //if (gwf.WFState == WFState.Shift)
                //{
                //    //如果是转发.
                //    BP.WF.ShiftWorks fws = new ShiftWorks();
                //    fws.Retrieve(ShiftWorkAttr.WorkID, workID, ShiftWorkAttr.FK_Node, fk_node);
                //    myds.Tables.Add(fws.ToDataTableField("WF_ShiftWork"));
                //}

                //if (gwf.WFState == WFState.ReturnSta)
                //{
                //    //如果是退回.
                //    ReturnWorks rts = new ReturnWorks();
                //    rts.Retrieve(ReturnWorkAttr.WorkID, workID,
                //        ReturnWorkAttr.ReturnToNode, fk_node,
                //        ReturnWorkAttr.RDT);
                //    myds.Tables.Add(rts.ToDataTableField("WF_ReturnWork"));
                //}

                //if (gwf.WFState == WFState.HungUp)
                //{
                //    //如果是挂起.
                //    HungUps hups = new HungUps();
                //    hups.Retrieve(HungUpAttr.WorkID, workID, HungUpAttr.FK_Node, fk_node);
                //    myds.Tables.Add(hups.ToDataTableField("WF_HungUp"));
                //}
               
                //Int64 wfid = workID;
                //if (fid != 0)
                //    wfid = fid;

                ////放入track信息.
                //Paras ps = new Paras();
                //ps.SQL = "SELECT * FROM ND" + int.Parse(fk_flow) + "Track WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
                //ps.Add("WorkID", wfid);
                //DataTable dtNode = DBAccess.RunSQLReturnTable(ps);
                //dtNode.TableName = "Track";
                //myds.Tables.Add(dtNode);

                ////工作人员列表，用于审核组件.
                //ps = new Paras();
                //ps.SQL = "SELECT * FROM  WF_GenerWorkerlist WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
                //ps.Add("WorkID", wfid);
                //DataTable dtGenerWorkerlist = DBAccess.RunSQLReturnTable(ps);
                //dtGenerWorkerlist.TableName = "WF_GenerWorkerlist";
                //myds.Tables.Add(dtGenerWorkerlist);

                ////放入CCList信息. 用于审核组件.
                //ps = new Paras();
                //ps.SQL = "SELECT * FROM WF_CCList WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
                //ps.Add("WorkID", wfid);
                //DataTable dtCCList = DBAccess.RunSQLReturnTable(ps);
                //dtCCList.TableName = "WF_CCList";
                //myds.Tables.Add(dtCCList);

                ////放入WF_SelectAccper信息. 用于审核组件.
                //ps = new Paras();
                //ps.SQL = "SELECT * FROM WF_SelectAccper WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
                //ps.Add("WorkID", wfid);
                //DataTable dtSelectAccper = DBAccess.RunSQLReturnTable(ps);
                //dtSelectAccper.TableName = "WF_SelectAccper";
                //myds.Tables.Add(dtSelectAccper);

                ////放入所有的节点信息. 用于审核组件.
                //ps = new Paras();
                //ps.SQL = "SELECT * FROM WF_Node WHERE FK_Flow=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "FK_Flow ORDER BY " + NodeAttr.Step;
                //ps.Add("FK_Flow", fk_flow);
                //DataTable dtNodes = DBAccess.RunSQLReturnTable(ps);
                //dtNodes.TableName = "Nodes";
                //myds.Tables.Add(dtNodes);
                #endregion 把流程信息放入里面.

                #region 处理流程-消息提示.
                DataTable dtAlert = new DataTable();
                dtAlert.TableName = "AlertMsg";
                dtAlert.Columns.Add("Title", typeof(string));
                dtAlert.Columns.Add("Msg", typeof(string));

              //  string msg = "";
                switch (gwf.WFState)
                {
                    case WFState.AskForReplay: // 返回加签的信息.
                        string mysql = "SELECT * FROM ND" + int.Parse(fk_flow) + "Track WHERE WorkID=" + workID + " AND " + TrackAttr.ActionType + "=" + (int)ActionType.ForwardAskfor;
                        DataTable mydt = BP.DA.DBAccess.RunSQLReturnTable(mysql);
                        foreach (DataRow dr in mydt.Rows)
                        {
                            string msgAskFor = dr[TrackAttr.Msg].ToString();
                            string worker = dr[TrackAttr.EmpFrom].ToString();
                            string workerName = dr[TrackAttr.EmpFromT].ToString();
                            string rdt = dr[TrackAttr.RDT].ToString();

                            DataRow drMsg = dtAlert.NewRow();
                            drMsg["Title"] = worker + "," + workerName + "回复信息:";
                            drMsg["Msg"] = DataType.ParseText2Html(msgAskFor) + "<br>" + rdt;
                            dtAlert.Rows.Add(drMsg);
                        }
                        break;
                    case WFState.Askfor: //加签.

                        sql = "SELECT * FROM ND" + int.Parse(fk_flow) + "Track WHERE WorkID=" + workID + " AND " + TrackAttr.ActionType + "=" + (int)ActionType.AskforHelp;
                        dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                        foreach (DataRow dr in dt.Rows)
                        {
                            string msgAskFor = dr[TrackAttr.Msg].ToString();
                            string worker = dr[TrackAttr.EmpFrom].ToString();
                            string workerName = dr[TrackAttr.EmpFromT].ToString();
                            string rdt = dr[TrackAttr.RDT].ToString();

                            DataRow drMsg = dtAlert.NewRow();
                            drMsg["Title"] = worker + "," + workerName + "请求加签:";
                            drMsg["Msg"] = DataType.ParseText2Html(msgAskFor) + "<br>" + rdt + "<a href='./WorkOpt/AskForRe.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workID + "&FID=" + fid + "' >回复加签意见</a> --";
                            dtAlert.Rows.Add(drMsg);

                            //提示信息.
                            // this.FlowMsg.AlertMsg_Info(worker + "," + workerName + "请求加签:",
                            //   DataType.ParseText2Html(msgAskFor) + "<br>" + rdt + " --<a href='./WorkOpt/AskForRe.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "' >回复加签意见</a> --");
                        }
                        // isAskFor = true;
                        break;
                    case WFState.ReturnSta:
                        /* 如果工作节点退回了*/
                        ReturnWorks rws = new ReturnWorks();
                        rws.Retrieve(ReturnWorkAttr.ReturnToNode, fk_node,
                            ReturnWorkAttr.WorkID, workID,
                            ReturnWorkAttr.RDT);
                        if (rws.Count != 0)
                        {
                            string msgInfo = "";
                            foreach (BP.WF.ReturnWork rw in rws)
                            {
                                DataRow drMsg = dtAlert.NewRow();
                                drMsg["Title"] = "来自节点:" + rw.ReturnNodeName + " 退回人:" + rw.ReturnerName + "  " + rw.RDT + "&nbsp;<a href='/DataUser/ReturnLog/" + fk_flow + "/" + rw.MyPK + ".htm' target=_blank>工作日志</a>";
                                drMsg["Msg"] = rw.NoteHtml;
                                dtAlert.Rows.Add(drMsg);
                            }

                            string str = nd.ReturnAlert;
                            if (str != "")
                            {
                                str = str.Replace("~", "'");
                                str = str.Replace("@PWorkID", workID.ToString());
                                str = str.Replace("@PNodeID", nd.NodeID.ToString());
                                str = str.Replace("@FK_Node", nd.NodeID.ToString());

                                str = str.Replace("@PFlowNo", fk_flow);
                                str = str.Replace("@FK_Flow", fk_flow);
                                str = str.Replace("@PWorkID", workID.ToString());

                                str = str.Replace("@WorkID", workID.ToString());
                                str = str.Replace("@OID", workID.ToString());

                                DataRow drMsg = dtAlert.NewRow();
                                drMsg["Title"] = "退回信息";
                                drMsg["Msg"] = str;
                                dtAlert.Rows.Add(drMsg);
                            }
                            else
                            {
                                DataRow drMsg = dtAlert.NewRow();
                                drMsg["Title"] = "退回信息";
                                drMsg["Msg"] = msgInfo;
                                dtAlert.Rows.Add(drMsg);
                            }
                            //gwf.WFState = WFState.Runing;
                            //gwf.DirectUpdate();
                        }
                        break;
                    case WFState.Shift:
                        /* 判断移交过来的。 */
                        ShiftWorks fws = new ShiftWorks();
                        BP.En.QueryObject qo = new QueryObject(fws);
                        qo.AddWhere(ShiftWorkAttr.WorkID, workID);
                        qo.addAnd();
                        qo.AddWhere(ShiftWorkAttr.FK_Node, fk_node);
                        qo.addOrderBy(ShiftWorkAttr.RDT);
                        qo.DoQuery();
                        if (fws.Count >= 1)
                        {
                            DataRow drMsg = dtAlert.NewRow();
                            drMsg["Title"] = "移交历史信息";
                            msg = "";
                            foreach (ShiftWork fw in fws)
                            {
                                string temp = "@移交人[" + fw.FK_Emp + "," + fw.FK_EmpName + "]。@接受人：" + fw.ToEmp + "," + fw.ToEmpName + "。<br>移交原因：-------------" + fw.NoteHtml;
                                if (fw.FK_Emp == WebUser.No)
                                    temp = "<b>" + temp + "</b>";

                                temp = temp.Replace("@", "<br>@");
                                msg += temp + "<hr/>";
                            }
                            drMsg["Msg"] = msg;
                            dtAlert.Rows.Add(drMsg);
                        }
                        break;
                    default:
                        break;
                }
                #endregion

                myds.Tables.Add(dtAlert);

                return myds;
            }
            catch (Exception ex)
            {
                Log.DebugWriteError(ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 产生一个WorkNode
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="fid">FID</param>
        /// <param name="userNo">用户编号</param>
        /// <returns>返回dataset</returns>
        public static DataSet GenerWorkNodeForAndroid(string fk_flow, int fk_node, Int64 workID, Int64 fid, string userNo)
        {
            if (fk_node == 0)
                fk_node = int.Parse(fk_flow + "01");

            if (workID == 0)
                workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow, null, null, userNo, null);

            try
            {
                Emp emp = new Emp(userNo);
                BP.Web.WebUser.SignInOfGener(emp);

                MapData md = new MapData();
                md.No = "ND" + fk_node;
                if (md.RetrieveFromDBSources() == 0)
                    throw new Exception("装载错误，该表单ID=" + md.No + "丢失，请修复一次流程重新加载一次.");

                //表单模版.
                DataSet myds = BP.Sys.CCFormAPI.GenerHisDataSet(md.No);
                return myds;

                #region 流程设置信息.
                Node nd = new Node(fk_node);

                if (nd.IsStartNode == false)
                    BP.WF.Dev2Interface.Node_SetWorkRead(fk_node, workID);

                // 节点数据.
                string sql = "SELECT * FROM WF_Node WHERE NodeID=" + fk_node;
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_NodeBar";
                myds.Tables.Add(dt);

                // 流程数据.
                Flow fl = new Flow(fk_flow);
                myds.Tables.Add(fl.ToDataTableField("WF_Flow"));
                #endregion 流程设置信息.

                #region 把主从表数据放入里面.
                //.工作数据放里面去, 放进去前执行一次装载前填充事件.
                BP.WF.Work wk = nd.HisWork;
                wk.OID = workID;
                wk.RetrieveFromDBSources();

                // 处理传递过来的参数。
                foreach (string k in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
                {
                    wk.SetValByKey(k, System.Web.HttpContext.Current.Request.QueryString[k]);
                }

                // 执行一次装载前填充.
                string msg = md.FrmEvents.DoEventNode(FrmEventList.FrmLoadBefore, wk);
                if (string.IsNullOrEmpty(msg) == false)
                    throw new Exception("错误:" + msg);

                wk.ResetDefaultVal();
                myds.Tables.Add(wk.ToDataTableField(md.No));

                //把附件的数据放入.
                if (md.FrmAttachments.Count > 0)
                {
                    sql = "SELECT * FROM Sys_FrmAttachmentDB where RefPKVal=" + workID + " AND FK_MapData='ND" + fk_node + "'";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "Sys_FrmAttachmentDB";
                    myds.Tables.Add(dt);
                }
                // 图片附件数据放入
                if (md.FrmImgAths.Count > 0)
                {
                    sql = "SELECT * FROM Sys_FrmImgAthDB where RefPKVal=" + workID + " AND FK_MapData='ND" + fk_node + "'";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "Sys_FrmImgAthDB";
                    myds.Tables.Add(dt);
                }

                //把从表的数据放入.
                if (md.MapDtls.Count > 0)
                {
                    foreach (MapDtl dtl in md.MapDtls)
                    {
                        GEDtls dtls = new GEDtls(dtl.No);
                        QueryObject qo = null;
                        try
                        {
                            qo = new QueryObject(dtls);
                            switch (dtl.DtlOpenType)
                            {
                                case DtlOpenType.ForEmp:  // 按人员来控制.
                                    qo.AddWhere(GEDtlAttr.RefPK, workID);
                                    qo.addAnd();
                                    qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                                    break;
                                case DtlOpenType.ForWorkID: // 按工作ID来控制
                                    qo.AddWhere(GEDtlAttr.RefPK, workID);
                                    break;
                                case DtlOpenType.ForFID: // 按流程ID来控制.
                                    qo.AddWhere(GEDtlAttr.FID, workID);
                                    break;
                            }
                        }
                        catch
                        {
                            dtls.GetNewEntity.CheckPhysicsTable();
                        }
                        DataTable dtDtl = qo.DoQueryToTable();

                        // 为明细表设置默认值.
                        MapAttrs dtlAttrs = new MapAttrs(dtl.No);
                        foreach (MapAttr attr in dtlAttrs)
                        {
                            //处理它的默认值.
                            if (attr.DefValReal.Contains("@") == false)
                                continue;

                            foreach (DataRow dr in dtDtl.Rows)
                                dr[attr.KeyOfEn] = attr.DefVal;
                        }

                        dtDtl.TableName = dtl.No; //修改明细表的名称.
                        myds.Tables.Add(dtDtl); //加入这个明细表, 如果没有数据，xml体现为空.
                    }
                }
                #endregion

                #region 把外键表加入DataSet
                DataTable dtMapAttr = myds.Tables["Sys_MapAttr"];
                foreach (DataRow dr in dtMapAttr.Rows)
                {
                    string lgType = dr["LGType"].ToString();
                    if (lgType != "2")
                        continue;

                    string UIIsEnable = dr["UIIsEnable"].ToString();
                    if (UIIsEnable == "0")
                        continue;

                    string uiBindKey = dr["UIBindKey"].ToString();
                    if (string.IsNullOrEmpty(uiBindKey) == true)
                    {
                        string myPK = dr["MyPK"].ToString();
                        /*如果是空的*/
                        throw new Exception("@属性字段数据不完整，流程:" + fl.No + fl.Name + ",节点:" + nd.NodeID + nd.Name + ",属性:" + myPK + ",的UIBindKey IsNull ");
                    }

                    // 判断是否存在.
                    if (myds.Tables.Contains(uiBindKey) == true)
                        continue;

                    myds.Tables.Add(BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey));
                }
                #endregion End把外键表加入DataSet

                #region 把流程信息放入里面.
                //把流程信息表发送过去.
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = workID;
                gwf.RetrieveFromDBSources();

                myds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

                if (gwf.WFState == WFState.Shift)
                {
                    //如果是转发.
                    BP.WF.ShiftWorks fws = new ShiftWorks();
                    fws.Retrieve(ShiftWorkAttr.WorkID, workID, ShiftWorkAttr.FK_Node, fk_node);
                    myds.Tables.Add(fws.ToDataTableField("WF_ShiftWork"));
                }

                if (gwf.WFState == WFState.ReturnSta)
                {
                    //如果是退回.
                    ReturnWorks rts = new ReturnWorks();
                    rts.Retrieve(ReturnWorkAttr.WorkID, workID,
                        ReturnWorkAttr.ReturnToNode, fk_node,
                        ReturnWorkAttr.RDT);
                    myds.Tables.Add(rts.ToDataTableField("WF_ReturnWork"));
                }

                if (gwf.WFState == WFState.HungUp)
                {
                    //如果是挂起.
                    HungUps hups = new HungUps();
                    hups.Retrieve(HungUpAttr.WorkID, workID, HungUpAttr.FK_Node, fk_node);
                    myds.Tables.Add(hups.ToDataTableField("WF_HungUp"));
                }

                //if (gwf.WFState == WFState.Askfor)
                //{
                //    //如果是加签.
                //    BP.WF.ShiftWorks fws = new ShiftWorks();
                //    fws.Retrieve(ShiftWorkAttr.WorkID, workID, ShiftWorkAttr.FK_Node, fk_node);
                //    myds.Tables.Add(fws.ToDataTableField("WF_ShiftWork"));
                //}

                Int64 wfid = workID;
                if (fid != 0)
                    wfid = fid;


                //放入track信息.
                Paras ps = new Paras();
                ps.SQL = "SELECT * FROM ND" + int.Parse(fk_flow) + "Track WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
                ps.Add("WorkID", wfid);
                DataTable dtNode = DBAccess.RunSQLReturnTable(ps);
                dtNode.TableName = "Track";
                myds.Tables.Add(dtNode);

                //工作人员列表，用于审核组件.
                ps = new Paras();
                ps.SQL = "SELECT * FROM  WF_GenerWorkerlist WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
                ps.Add("WorkID", wfid);
                DataTable dtGenerWorkerlist = DBAccess.RunSQLReturnTable(ps);
                dtGenerWorkerlist.TableName = "WF_GenerWorkerlist";
                myds.Tables.Add(dtGenerWorkerlist);

                //放入CCList信息. 用于审核组件.
                ps = new Paras();
                ps.SQL = "SELECT * FROM WF_CCList WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
                ps.Add("WorkID", wfid);
                DataTable dtCCList = DBAccess.RunSQLReturnTable(ps);
                dtCCList.TableName = "WF_CCList";
                myds.Tables.Add(dtCCList);

                //放入WF_SelectAccper信息. 用于审核组件.
                ps = new Paras();
                ps.SQL = "SELECT * FROM WF_SelectAccper WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
                ps.Add("WorkID", wfid);
                DataTable dtSelectAccper = DBAccess.RunSQLReturnTable(ps);
                dtSelectAccper.TableName = "WF_SelectAccper";
                myds.Tables.Add(dtSelectAccper);

                //放入所有的节点信息. 用于审核组件.
                ps = new Paras();
                ps.SQL = "SELECT * FROM WF_Node WHERE FK_Flow=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "FK_Flow ORDER BY " + NodeAttr.Step;
                ps.Add("FK_Flow", fk_flow);
                DataTable dtNodes = DBAccess.RunSQLReturnTable(ps);
                dtNodes.TableName = "Nodes";
                myds.Tables.Add(dtNodes);

                #endregion 把流程信息放入里面.

                return myds;
            }
            catch (Exception ex)
            {
                Log.DebugWriteError(ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
    }
}
