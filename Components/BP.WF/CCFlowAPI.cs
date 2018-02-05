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
            //让其登录. ??? 为什么需要登录？
            if (WebUser.No != userNo)
            {
                Emp emp = new Emp(userNo);
                BP.Web.WebUser.SignInOfGener(emp);
            }

            //节点.
            if (fk_node == 0)
                fk_node = int.Parse(fk_flow + "01");

            if (workID == 0)
                workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow, null, null, userNo, null);

            Node nd = new Node(fk_node);
            try
            {

                MapData md = new MapData();
                md.No = nd.NodeFrmID;
                if (md.RetrieveFromDBSources() == 0)
                    throw new Exception("装载错误，该表单ID=" + md.No + "丢失，请修复一次流程重新加载一次.");

                //获得表单模版.
                DataSet myds = BP.Sys.CCFormAPI.GenerHisDataSet_2017(md.No);

                //把流程信息表发送过去.
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = workID;
                gwf.RetrieveFromDBSources();


                //加入WF_Node.
                DataTable WF_Node = nd.ToDataTableField("WF_Node");
                myds.Tables.Add(WF_Node);

                #region 加入组件的状态信息, 在解析表单的时候使用.
                BP.WF.Template.FrmNodeComponent fnc = new FrmNodeComponent(nd.NodeID);
                if (nd.NodeFrmID != "ND" + nd.NodeID && nd.HisFormType != NodeFormType.RefOneFrmTree)
                {
                    /*说明这是引用到了其他节点的表单，就需要把一些位置元素修改掉.*/
                    int refNodeID = int.Parse(nd.NodeFrmID.Replace("ND", ""));

                    BP.WF.Template.FrmNodeComponent refFnc = new FrmNodeComponent(refNodeID);

                    fnc.SetValByKey(FrmWorkCheckAttr.FWC_H, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_H));
                    fnc.SetValByKey(FrmWorkCheckAttr.FWC_W, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_W));
                    fnc.SetValByKey(FrmWorkCheckAttr.FWC_X, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_X));
                    fnc.SetValByKey(FrmWorkCheckAttr.FWC_Y, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_Y));

                    if (fnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_H) <= 10)
                        fnc.SetValByKey(FrmWorkCheckAttr.FWC_H, 500);

                    if (fnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_W) <= 10)
                        fnc.SetValByKey(FrmWorkCheckAttr.FWC_W, 600);

                    if (fnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_X) <= 10)
                        fnc.SetValByKey(FrmWorkCheckAttr.FWC_X, 200);

                    if (fnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_Y) <= 10)
                        fnc.SetValByKey(FrmWorkCheckAttr.FWC_Y, 200);


                    fnc.SetValByKey(FrmSubFlowAttr.SF_H, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_H));
                    fnc.SetValByKey(FrmSubFlowAttr.SF_W, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_W));
                    fnc.SetValByKey(FrmSubFlowAttr.SF_X, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_X));
                    fnc.SetValByKey(FrmSubFlowAttr.SF_Y, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_Y));

                    fnc.SetValByKey(FrmThreadAttr.FrmThread_H, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_H));
                    fnc.SetValByKey(FrmThreadAttr.FrmThread_W, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_W));
                    fnc.SetValByKey(FrmThreadAttr.FrmThread_X, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_X));
                    fnc.SetValByKey(FrmThreadAttr.FrmThread_Y, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_Y));

                    fnc.SetValByKey(FrmTrackAttr.FrmTrack_H, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_H));
                    fnc.SetValByKey(FrmTrackAttr.FrmTrack_W, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_W));
                    fnc.SetValByKey(FrmTrackAttr.FrmTrack_X, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_X));
                    fnc.SetValByKey(FrmTrackAttr.FrmTrack_Y, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_Y));

                    fnc.SetValByKey(FTCAttr.FTC_H, refFnc.GetValFloatByKey(FTCAttr.FTC_H));
                    fnc.SetValByKey(FTCAttr.FTC_W, refFnc.GetValFloatByKey(FTCAttr.FTC_W));
                    fnc.SetValByKey(FTCAttr.FTC_X, refFnc.GetValFloatByKey(FTCAttr.FTC_X));
                    fnc.SetValByKey(FTCAttr.FTC_Y, refFnc.GetValFloatByKey(FTCAttr.FTC_Y));
                }

                myds.Tables.Add(fnc.ToDataTableField("WF_FrmNodeComponent"));
                #endregion 加入组件的状态信息, 在解析表单的时候使用.

                #region 流程设置信息.
                if (nd.IsStartNode == false)
                    BP.WF.Dev2Interface.Node_SetWorkRead(fk_node, workID);
                 
                //增加转向下拉框数据.
                if (nd.CondModel == CondModel.SendButtonSileSelect)
                {
                    if (nd.IsStartNode==true || gwf.TodoEmps.Contains(WebUser.No + ",") == true )
                    {
                        /*如果当前不是主持人,如果不是主持人，就不让他显示下拉框了.*/

                        /*如果当前节点，是可以显示下拉框的.*/
                        Nodes nds = nd.HisToNodes;

                        DataTable dtToNDs = new DataTable();
                        dtToNDs.TableName = "ToNodes";
                        dtToNDs.Columns.Add("No", typeof(string));   //节点ID.
                        dtToNDs.Columns.Add("Name", typeof(string)); //到达的节点名称.
                        dtToNDs.Columns.Add("IsSelectEmps", typeof(string)); //是否弹出选择人的对话框？
                        dtToNDs.Columns.Add("IsSelected", typeof(string));  //是否选择？

                        #region 增加到达延续子流程节点。
                        NodeYGFlows ygflows = new NodeYGFlows(fk_node.ToString());
                        if (ygflows.Count > 1 && SystemConfig.CustomerNo.Equals("CZBank")==true)
                            dtToNDs.Rows.Clear();  //为浙商银行做的特殊判断，如果配置了延续流程，就不让其走分支节点.

                        foreach (NodeYGFlow item in ygflows)
                        {
                            DataRow dr = dtToNDs.NewRow();
                            dr["No"] = item.FK_Flow + "01";
                            dr["Name"] = "启动:" + item.FlowName;
                            //if (item.HisDeliveryWay == DeliveryWay.BySelected)
                            dr["IsSelectEmps"] = "1";
                            //else
                            //  dr["IsSelectEmps"] = "0";  //是不是，可以选择接受人.

                            //设置默认选择的节点.
                            //if (defalutSelectedNodeID == item.NodeID)
                            //    dr["IsSelected"] = "1";
                            //else
                            //    dr["IsSelected"] = "0";

                            dr["IsSelected"] = "0";
                            dtToNDs.Rows.Add(dr);
                        }
                        #endregion 增加到达延续子流程节点。

                        #region 到达其他节点.
                        if (ygflows.Count==0)
                        {
                            //上一次选择的节点.
                            int defalutSelectedNodeID = 0;
                            if (nds.Count > 1)
                            {
                                string mysql = "";
                                // 找出来上次发送选择的节点.
                                if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                                    mysql = "SELECT  top 1 NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + fk_node + " AND ActionType=1 ORDER BY WorkID DESC";
                                else if (SystemConfig.AppCenterDBType == DBType.Oracle)
                                    mysql = "SELECT * FROM ( SELECT  NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + fk_node + " AND ActionType=1 ORDER BY WorkID DESC ) WHERE ROWNUM =1";
                                else if (SystemConfig.AppCenterDBType == DBType.MySQL)
                                    mysql = "SELECT  NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + fk_node + " AND ActionType=1 ORDER BY WorkID  DESC limit 1,1";

                                //获得上一次发送到的节点.
                                defalutSelectedNodeID = DBAccess.RunSQLReturnValInt(mysql, 0);
                            }

                            #region 为天业集团做一个特殊的判断.
                            if (SystemConfig.CustomerNo == "TianYe" && nd.Name.Contains("董事长")==true)
                            {
                                /*如果是董事长节点, 如果是下一个节点默认的是备案. */
                                foreach (Node item in nds)
                                {
                                    if (item.Name.Contains("备案") == true && item.Name.Contains("待") == false)
                                    {
                                        defalutSelectedNodeID = item.NodeID;
                                        break;
                                    }
                                }
                            }
                            #endregion 为天业集团做一个特殊的判断.



                            foreach (Node item in nds)
                            {
                                DataRow dr = dtToNDs.NewRow();
                                dr["No"] = item.NodeID;
                                dr["Name"] = item.Name;
                                //if (item.hissel

                                if (item.HisDeliveryWay == DeliveryWay.BySelected)
                                    dr["IsSelectEmps"] = "1";
                                else
                                    dr["IsSelectEmps"] = "0";  //是不是，可以选择接受人.

                                //设置默认选择的节点.
                                if (defalutSelectedNodeID == item.NodeID)
                                    dr["IsSelected"] = "1";
                                else
                                    dr["IsSelected"] = "0";

                                dtToNDs.Rows.Add(dr);
                            }
                        }
                        #endregion 到达其他节点。


                        //增加一个下拉框, 对方判断是否有这个数据.
                        myds.Tables.Add(dtToNDs);
                    }
                }

                // 节点数据.
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

                //重设默认值.
                wk.ResetDefaultVal();

                if (BP.Sys.SystemConfig.IsBSsystem == true)
                {
                    // 处理传递过来的参数。
                    foreach (string k in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
                    {
                        if (string.IsNullOrEmpty(k) == true)
                            continue;

                        wk.SetValByKey(k, System.Web.HttpContext.Current.Request.QueryString[k]);
                    }

                    // 处理传递过来的frm参数。
                    foreach (string k in System.Web.HttpContext.Current.Request.Form.AllKeys)
                    {
                        if (string.IsNullOrEmpty(k) == true)
                            continue;
                        wk.SetValByKey(k, System.Web.HttpContext.Current.Request.Form[k]);
                    }
                }

                // 执行表单事件..
                string msg = md.DoEvent(FrmEventList.FrmLoadBefore, wk);
                if (string.IsNullOrEmpty(msg) == false)
                    throw new Exception("err@错误:" + msg);

                // 执行FEE事件.
                string msgOfLoad = nd.HisFlow.DoFlowEventEntity(EventListOfNode.FrmLoadBefore, nd,
                    wk, null);
                if (msgOfLoad != null)
                    wk.RetrieveFromDBSources();


                //执行装载填充.
                MapExt me = new MapExt();
                if (me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull, MapExtAttr.FK_MapData, wk.NodeFrmID) == 1)
                {
                    //执行通用的装载方法.
                    MapAttrs attrs = new MapAttrs(wk.NodeFrmID);
                    MapDtls dtls = new MapDtls(wk.NodeFrmID);
                    wk = BP.WF.Glo.DealPageLoadFull(wk, me, attrs, dtls) as Work;
                }

                DataTable mainTable = wk.ToDataTableField(md.No);
                mainTable.TableName = "MainTable";
                myds.Tables.Add(mainTable);

                string sql = "";
                DataTable dt = null;
                #endregion

                #region 图片附件
                FrmImgAthDBs imgAthDBs = new FrmImgAthDBs(nd.NodeFrmID, workID.ToString());
                if (imgAthDBs != null && imgAthDBs.Count > 0)
                {
                    DataTable dt_ImgAth = imgAthDBs.ToDataTableField("Sys_FrmImgAthDB");
                    myds.Tables.Add(dt_ImgAth);
                }
                #endregion

                #region 增加附件信息.
                BP.Sys.FrmAttachments athDescs = new FrmAttachments();
                athDescs.Retrieve(FrmAttachmentAttr.FK_MapData, nd.NodeFrmID);
                if (athDescs.Count != 0)
                {
                    FrmAttachment athDesc = athDescs[0] as FrmAttachment;

                    //查询出来数据实体.
                    BP.Sys.FrmAttachmentDBs dbs = new BP.Sys.FrmAttachmentDBs();
                    if (athDesc.HisCtrlWay == AthCtrlWay.PWorkID)
                    {
                        string pWorkID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + workID, 0).ToString();
                        if (pWorkID == null || pWorkID == "0")
                            pWorkID = workID.ToString();

                        if (athDesc.AthUploadWay == AthUploadWay.Inherit)
                        {
                            /* 继承模式 */
                            BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
                            qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, pWorkID);
                            qo.addOr();
                            qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, workID);
                            qo.addOrderBy("RDT");
                            qo.DoQuery();
                        }

                        if (athDesc.AthUploadWay == AthUploadWay.Interwork)
                        {
                            /*共享模式*/
                            dbs.Retrieve(FrmAttachmentDBAttr.RefPKVal, pWorkID);
                        }
                    }
                    else if (athDesc.HisCtrlWay == AthCtrlWay.WorkID)
                    {
                        /* 继承模式 */
                        BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
                        qo.AddWhere(FrmAttachmentDBAttr.NoOfObj, athDesc.NoOfObj);
                        qo.addAnd();
                        qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, workID);
                        qo.addOrderBy("RDT");
                        qo.DoQuery();
                    }

                    //增加一个数据源.
                    myds.Tables.Add(dbs.ToDataTableField("Sys_FrmAttachmentDB"));
                }
                #endregion

                #region 把外键表加入DataSet
                DataTable dtMapAttr = myds.Tables["Sys_MapAttr"] ;
                MapExts mes = md.MapExts;
                foreach (DataRow dr in dtMapAttr.Rows)
                {
                    string lgType = dr["LGType"].ToString();
                    if (lgType != "2")
                        continue;

                    string UIIsEnable = dr["UIVisible"].ToString();
                    if (UIIsEnable == "0")
                        continue;

                    string uiBindKey = dr["UIBindKey"].ToString();
                    if (string.IsNullOrEmpty(uiBindKey) == true)
                    {
                        string myPK = dr["MyPK"].ToString();
                        /*如果是空的*/
                     //   throw new Exception("@属性字段数据不完整，流程:" + fl.No + fl.Name + ",节点:" + nd.NodeID + nd.Name + ",属性:" + myPK + ",的UIBindKey IsNull ");
                    }

                    // 检查是否有下拉框自动填充。
                    string keyOfEn = dr["KeyOfEn"].ToString();
                    string fk_mapData = dr["FK_MapData"].ToString();

                    #region 处理下拉框数据范围. for 小杨.
                    me = mes.GetEntityByKey(MapExtAttr.ExtType,  MapExtXmlList.AutoFullDLL, MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
                    if (me != null)
                    {
                        string fullSQL = me.Doc.Clone() as string;
                        fullSQL = fullSQL.Replace("~", ",");
                        fullSQL = BP.WF.Glo.DealExp(fullSQL, wk, null);
                        dt = DBAccess.RunSQLReturnTable(fullSQL);
                        //重构新表
                        DataTable dt_FK_Dll = new DataTable();
                        dt_FK_Dll.TableName = keyOfEn;//可能存在隐患，如果多个字段，绑定同一个表，就存在这样的问题.
                        dt_FK_Dll.Columns.Add("No", typeof(string));
                        dt_FK_Dll.Columns.Add("Name", typeof(string));
                        foreach (DataRow dllRow in dt.Rows)
                        {
                            DataRow drDll = dt_FK_Dll.NewRow();
                            drDll["No"] = dllRow["No"];
                            drDll["Name"] = dllRow["Name"];
                            dt_FK_Dll.Rows.Add(drDll);
                        }
                        myds.Tables.Add(dt_FK_Dll);
                        continue;
                    }
                    #endregion 处理下拉框数据范围.

                    // 判断是否存在.
                    if (myds.Tables.Contains(uiBindKey) == true)
                        continue;

                    myds.Tables.Add(BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey));
                }
                #endregion End把外键表加入DataSet

                #region 处理流程-消息提示.
                DataTable dtAlert = new DataTable();
                dtAlert.TableName = "AlertMsg";

                dtAlert.Columns.Add("Title", typeof(string));
                dtAlert.Columns.Add("Msg", typeof(string));
                dtAlert.Columns.Add("URL", typeof(string));

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
                            drMsg["Msg"] = DataType.ParseText2Html(msgAskFor) + "<br>" + rdt + "<a href='./WorkOpt/AskForRe.htm?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workID + "&FID=" + fid + "' >回复加签意见</a> --";
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
                            //string msgInfo = "";
                            //foreach (BP.WF.ReturnWork rw in rws)
                            //{
                            //    DataRow drMsg = dtAlert.NewRow();
                            //    //drMsg["Title"] = "来自节点:" + rw.ReturnNodeName + " 退回人:" + rw.ReturnerName + "  " + rw.RDT + "&nbsp;<a href='/DataUser/ReturnLog/" + fk_flow + "/" + rw.MyPK + ".htm' target=_blank>工作日志</a>";
                            //    drMsg["Title"] = "来自节点:" + rw.ReturnNodeName + " 退回人:" + rw.ReturnerName + "  " + rw.RDT;
                            //    drMsg["Msg"] = rw.BeiZhuHtml;
                            //    dtAlert.Rows.Add(drMsg);
                            //}

                             string msgInfo = "";
                             foreach (BP.WF.ReturnWork rw in rws)
                             {
                                 //drMsg["Title"] = "来自节点:" + rw.ReturnNodeName + " 退回人:" + rw.ReturnerName + "  " + rw.RDT + "&nbsp;<a href='/DataUser/ReturnLog/" + fk_flow + "/" + rw.MyPK + ".htm' target=_blank>工作日志</a>";
                                 msgInfo += "\t\n来自节点:" + rw.ReturnNodeName + " 退回人:" + rw.ReturnerName + "  " + rw.RDT;
                                 msgInfo += rw.BeiZhuHtml;
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
                                drMsg["Msg"] = msgInfo + "\t\n" + str;
                                dtAlert.Rows.Add(drMsg);
                            }
                            else
                            {
                                DataRow drMsg = dtAlert.NewRow();
                                drMsg["Title"] = "退回信息";
                                drMsg["Msg"] = msgInfo + "\t\n" + str;
                                dtAlert.Rows.Add(drMsg);
                            }
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

                #region 增加流程节点表单绑定信息.
                if (nd.HisFormType == NodeFormType.RefOneFrmTree)
                {
                    /* 独立流程节点表单. */

                    FrmNode fn = new FrmNode();
                    fn.MyPK = nd.NodeFrmID + "_" + nd.NodeID + "_" + nd.FK_Flow;
                    fn.Retrieve();
                    myds.Tables.Add(fn.ToDataTableField("FrmNode"));
                }
                #endregion 增加流程节点表单绑定信息.


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
                string msg = md.DoEvent(FrmEventList.FrmLoadBefore, wk);
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
