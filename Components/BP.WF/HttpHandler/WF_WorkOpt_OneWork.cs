using System;
using System.Collections;
using System.Data;
using BP.En;
using BP.DA;
using BP.Sys;
using BP.WF.XML;
using BP.WF.Template;
using BP.Web;
using BP.WF.Template.SFlow;
using NPOI.SS.Formula.Eval;
using BP.Port;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_WorkOpt_OneWork : DirectoryPageBase
    {
        /// <summary>
        /// 进度图.
        /// </summary>
        /// <returns></returns>
        public string JobSchedule_Init()
        {
            DataSet ds = BP.WF.Dev2Interface.DB_JobSchedule(this.WorkID);
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_WorkOpt_OneWork()
        {
        }
        /// <summary>
        /// 时间轴
        /// </summary>
        /// <returns></returns>
        public string TimeSubThread_Init()
        {
            DataSet ds = new DataSet();
            string mypks = GetRequestVal("MyPKs");
            mypks = "('" + mypks.Replace(",", "','") + "')";
            string sql = "SELECT MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Exer,Tag FROM ND" + int.Parse(this.FlowNo) + "Track Where MyPK IN"+ mypks + " ORDER BY RDT ASC ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Track";
            //把列名转化成区分大小写.
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["MYPK"].ColumnName = "MyPK";
                dt.Columns["ACTIONTYPE"].ColumnName = "ActionType";
                dt.Columns["ACTIONTYPETEXT"].ColumnName = "ActionTypeText";
                dt.Columns["FID"].ColumnName = "FID";
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["NDFROM"].ColumnName = "NDFrom";
                dt.Columns["NDFROMT"].ColumnName = "NDFromT";
                dt.Columns["NDTO"].ColumnName = "NDTo";
                dt.Columns["NDTOT"].ColumnName = "NDToT";
                dt.Columns["EMPFROM"].ColumnName = "EmpFrom";
                dt.Columns["EMPFROMT"].ColumnName = "EmpFromT";
                dt.Columns["EMPTO"].ColumnName = "EmpTo";
                dt.Columns["EMPTOT"].ColumnName = "EmpToT";
                dt.Columns["RDT"].ColumnName = "RDT";
                dt.Columns["WORKTIMESPAN"].ColumnName = "WorkTimeSpan";
                dt.Columns["MSG"].ColumnName = "Msg";
                dt.Columns["NODEDATA"].ColumnName = "NodeData";
                dt.Columns["EXER"].ColumnName = "Exer";
                dt.Columns["TAG"].ColumnName = "Tag";
            }

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["mypk"].ColumnName = "MyPK";
                dt.Columns["actiontype"].ColumnName = "ActionType";
                dt.Columns["actiontypetext"].ColumnName = "ActionTypeText";
                dt.Columns["fid"].ColumnName = "FID";
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["ndfrom"].ColumnName = "NDFrom";
                dt.Columns["ndfromt"].ColumnName = "NDFromT";
                dt.Columns["ndto"].ColumnName = "NDTo";
                dt.Columns["ndtot"].ColumnName = "NDToT";
                dt.Columns["empfrom"].ColumnName = "EmpFrom";
                dt.Columns["empfromt"].ColumnName = "EmpFromT";
                dt.Columns["empto"].ColumnName = "EmpTo";
                dt.Columns["emptot"].ColumnName = "EmpToT";
                dt.Columns["rdt"].ColumnName = "RDT";
                dt.Columns["worktimespan"].ColumnName = "WorkTimeSpan";
                dt.Columns["msg"].ColumnName = "Msg";
                dt.Columns["nodedata"].ColumnName = "NodeData";
                dt.Columns["exer"].ColumnName = "Exer";
                dt.Columns["tag"].ColumnName = "Tag";
            }
            //获取track.
            ds.Tables.Add(dt);


            #region  父子流程数据存储到这里.
            Hashtable ht = new Hashtable();
            foreach (DataRow dr in dt.Rows)
            {
                ActionType at = (ActionType)int.Parse(dr[TrackAttr.ActionType].ToString());

                string tag = dr[TrackAttr.Tag].ToString(); //标识.
                string mypk = dr[TrackAttr.MyPK].ToString(); //主键.

                string msg = "";
                if (at == ActionType.CallChildenFlow)
                {
                    //被调用父流程吊起。
                    if (DataType.IsNullOrEmpty(tag) == false)
                    {
                        AtPara ap = new AtPara(tag);
                        GenerWorkFlow mygwf = new GenerWorkFlow();
                        mygwf.WorkID = ap.GetValInt64ByKey("PWorkID");
                        if (mygwf.RetrieveFromDBSources() == 1)
                            msg = "<p>操作员:{" + dr[TrackAttr.EmpFromT].ToString() + "}在当前节点上，被父流程{" + mygwf.FlowName + "},<a target=b" + ap.GetValStrByKey("PWorkID") + " href='Track.htm?WorkID=" + ap.GetValStrByKey("PWorkID") + "&FK_Flow=" + ap.GetValStrByKey("PFlowNo") + "' >" + msg + "</a></p>";
                        else
                            msg = "<p>操作员:{" + dr[TrackAttr.EmpFromT].ToString() + "}在当前节点上，被父流程调用{" + mygwf.FlowName + "}，但是该流程被删除了.</p>" + tag;

                        msg = "<a target=b" + ap.GetValStrByKey("PWorkID") + " href='Track.htm?WorkID=" + ap.GetValStrByKey("PWorkID") + "&FK_Flow=" + ap.GetValStrByKey("PFlowNo") + "' >" + msg + "</a>";
                    }

                    //放入到ht里面.
                    ht.Add(mypk, msg);
                }

                if (at == ActionType.StartChildenFlow)
                {
                    if (DataType.IsNullOrEmpty(tag) == false)
                    {
                        if (tag.Contains("Sub"))
                            tag = tag.Replace("Sub", "C");

                        AtPara ap = new AtPara(tag);
                        GenerWorkFlow mygwf = new GenerWorkFlow();
                        mygwf.WorkID = ap.GetValInt64ByKey("CWorkID");
                        if (mygwf.RetrieveFromDBSources() == 1)
                        {
                            msg = "<p>操作员:{" + dr[TrackAttr.EmpFromT].ToString() + "}在当前节点上调用了子流程{" + mygwf.FlowName + "}, <a target=b" + ap.GetValStrByKey("CWorkID") + " href='Track.htm?WorkID=" + ap.GetValStrByKey("CWorkID") + "&FK_Flow=" + ap.GetValStrByKey("CFlowNo") + "' >" + msg + "</a></p>";
                            msg += "<p>当前子流程状态：{" + mygwf.WFStateText + "}，运转到:{" + mygwf.NodeName + "}，最后处理人{" + mygwf.TodoEmps + "}，最后处理时间{" + mygwf.RDT + "}。</p>";
                        }
                        else
                            msg = "<p>操作员:{" + dr[TrackAttr.EmpFromT].ToString() + "}在当前节点上调用了子流程{" + mygwf.FlowName + "}，但是该流程被删除了.</p>" + tag;

                    }

                    //放入到ht里面.
                    ht.Add(mypk, msg);
                }
            }
            #endregion


            //把节点审核配置信息.
            NodeWorkCheck fwc = new NodeWorkCheck(this.NodeID);
            ds.Tables.Add(fwc.ToDataTableField("FrmWorkCheck"));

            //返回结果.
            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        /// <summary>
        /// 时间轴
        /// </summary>
        /// <returns></returns>
        public string TimeBase_Init()
        {
            DataSet ds = new DataSet();

            //获取干流程和子线程中的Track信息
            DataTable dt = BP.WF.Dev2Interface.DB_GenerTrackTable(this.FlowNo, this.WorkID, this.FID,false);
            ds.Tables.Add(dt);


            /*#region  父子流程数据存储到这里.
            Hashtable ht = new Hashtable();
            foreach (DataRow dr in dt.Rows)
            {
                ActionType at = (ActionType)int.Parse(dr[TrackAttr.ActionType].ToString());

                string tag = dr[TrackAttr.Tag].ToString(); //标识.
                string mypk = dr[TrackAttr.MyPK].ToString(); //主键.

                string msg = "";
                if (at == ActionType.CallChildenFlow)
                {
                    //被调用父流程吊起。
                    if (DataType.IsNullOrEmpty(tag) == false)
                    {
                        AtPara ap = new AtPara(tag);
                        GenerWorkFlow mygwf = new GenerWorkFlow();
                        mygwf.WorkID = ap.GetValInt64ByKey("PWorkID");
                        if (mygwf.RetrieveFromDBSources() == 1)
                            msg = "<p>操作员:{" + dr[TrackAttr.EmpFromT].ToString() + "}在当前节点上，被父流程{" + mygwf.FlowName + "},<a target=b" + ap.GetValStrByKey("PWorkID") + " href='Track.htm?WorkID=" + ap.GetValStrByKey("PWorkID") + "&FK_Flow=" + ap.GetValStrByKey("PFlowNo") + "' >" + msg + "</a></p>";
                        else
                            msg = "<p>操作员:{" + dr[TrackAttr.EmpFromT].ToString() + "}在当前节点上，被父流程调用{" + mygwf.FlowName + "}，但是该流程被删除了.</p>" + tag;

                        msg = "<a target=b" + ap.GetValStrByKey("PWorkID") + " href='Track.htm?WorkID=" + ap.GetValStrByKey("PWorkID") + "&FK_Flow=" + ap.GetValStrByKey("PFlowNo") + "' >" + msg + "</a>";
                    }

                    //放入到ht里面.
                    ht.Add(mypk, msg);
                }

                if (at == ActionType.StartChildenFlow)
                {
                    if (DataType.IsNullOrEmpty(tag) == false)
                    {
                        if (tag.Contains("Sub"))
                            tag = tag.Replace("Sub", "C");

                        AtPara ap = new AtPara(tag);
                        GenerWorkFlow mygwf = new GenerWorkFlow();
                        mygwf.WorkID = ap.GetValInt64ByKey("CWorkID");
                        if (mygwf.RetrieveFromDBSources() == 1)
                        {
                            msg = "<p>操作员:{" + dr[TrackAttr.EmpFromT].ToString() + "}在当前节点上调用了子流程{" + mygwf.FlowName + "}, <a target=b" + ap.GetValStrByKey("CWorkID") + " href='Track.htm?WorkID=" + ap.GetValStrByKey("CWorkID") + "&FK_Flow=" + ap.GetValStrByKey("CFlowNo") + "' >" + msg + "</a></p>";
                            msg += "<p>当前子流程状态：{" + mygwf.WFStateText + "}，运转到:{" + mygwf.NodeName + "}，最后处理人{" + mygwf.TodoEmps + "}，最后处理时间{" + mygwf.RDT + "}。</p>";
                        }
                        else
                            msg = "<p>操作员:{" + dr[TrackAttr.EmpFromT].ToString() + "}在当前节点上调用了子流程{" + mygwf.FlowName + "}，但是该流程被删除了.</p>" + tag;

                    }

                    //放入到ht里面.
                    ht.Add(mypk, msg);
                }
            }
            #endregion*/

            //获取当前流程的待办信息 WF_GenerWorkFlow
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = this.WorkID;
            gwf.RetrieveFromDBSources();
            ds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            if (gwf.WFState != WFState.Complete)
            {
                GenerWorkerLists gwls = new GenerWorkerLists();
                gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.Idx);

                //warning 补偿式的更新.  做特殊的判断，当会签过了以后仍然能够看isPass=90的错误数据.
                foreach (GenerWorkerList item in gwls)
                {
                    if (item.PassInt == 90 && gwf.NodeID != item.NodeID)
                    {
                        item.PassInt = 0;
                        item.Update();
                    }
                }
                Node nd = new Node(gwf.NodeID);
                if(nd.HisRunModel == RunModel.FL || nd.HisRunModel == RunModel.FHL)
                {
                    //获取是否存在退回的分合流点
                    GenerWorkerLists tgwls = new GenerWorkerLists();
                    tgwls.Retrieve(GenerWorkerListAttr.FID, this.WorkID,GenerWorkerListAttr.FK_Node,gwf.NodeID, GenerWorkerListAttr.IsPass,0,GenerWorkerListAttr.Idx);
                    foreach(GenerWorkerList gwl in tgwls)
                    {
                        gwls.AddEntity(gwl);
                    }
                }
                ds.Tables.Add(gwls.ToDataTableField("WF_GenerWorkerlist"));
            }

            //把节点审核配置信息.
            NodeWorkCheck fwc = new NodeWorkCheck(gwf.NodeID);
            ds.Tables.Add(fwc.ToDataTableField("FrmWorkCheck"));

            //获取启动的子流程信息
            SubFlows subFlows = new SubFlows(this.FlowNo);
            ds.Tables.Add(subFlows.ToDataTableField("WF_SubFlow"));

            //返回结果.
            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        #region 执行父类的重写方法.

        #endregion 执行父类的重写方法.

        #region 属性.
        public string Msg
        {
            get
            {
                string str = this.GetRequestVal("TB_Msg");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string UserName
        {
            get
            {
                string str = this.GetRequestVal("UserName");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string Title
        {
            get
            {
                string str = this.GetRequestVal("Title");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 字典表
        /// </summary>
        public string FK_SFTable
        {
            get
            {
                string str = this.GetRequestVal("FK_SFTable");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;

            }
        }
        public string EnumKey
        {
            get
            {
                string str = this.GetRequestVal("EnumKey");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;

            }
        }


        public string Name
        {
            get
            {
                string str = BP.Web.WebUser.Name;
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        #endregion 属性.

        public string FlowBBS_Delete()
        {
            return BP.WF.Dev2Interface.Flow_BBSDelete(this.FlowNo, this.MyPK, WebUser.No);
        }
        /// <summary>
        /// 执行撤销
        /// </summary>
        /// <returns></returns>
        public string OP_UnSend()
        {
            //获取用户当前所在的节点
            String currNode = "";
            switch (DBAccess.AppCenterDBType)
            {
                case DBType.Oracle:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    currNode = "SELECT FK_Node FROM (SELECT  FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' AND WorkID=" + this.WorkID + "  Order by RDT DESC ) WHERE rownum=1";
                    break;
                case DBType.MySQL:
                case DBType.PostgreSQL:
                case DBType.UX:
                case DBType.HGDB:
                    currNode = "SELECT  FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' AND WorkID=" + this.WorkID + "  Order by RDT DESC LIMIT 1";
                    break;
                case DBType.MSSQL:
                    currNode = "SELECT TOP 1 FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' AND WorkID="+this.WorkID+" Order by RDT DESC";
                    break;
                default:
                    break;
            }
            String unSendToNode = DBAccess.RunSQLReturnString(currNode);
            try
            {
                return BP.WF.Dev2Interface.Flow_DoUnSend(this.FlowNo, this.WorkID, int.Parse(unSendToNode), this.FID);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        protected override string DoDefaultMethod()
        {
            return "err@没有判断的执行类型：" + this.DoType + " @类 " + this.ToString();
        }

        public string OP_ComeBack()
        {
            WorkFlow wf3 = new WorkFlow(WorkID);
            wf3.DoComeBackWorkFlow("无");
            return "流程已经被重新启用.";
        }

        public string OP_UnHungup()
        {
            WorkFlow wf2 = new WorkFlow( WorkID);
            //  wf2.DoUnHungup();
            return "流程已经被解除挂起.";
        }

        public string OP_Hungup()
        {
            WorkFlow wf1 = new WorkFlow( WorkID);
            //wf1.DoHungup()
            return "流程已经被挂起.";
        }

        public string OP_DelFlow()
        {
            WorkFlow wf = new WorkFlow( WorkID);
            wf.DoDeleteWorkFlowByReal(true);
            return "流程已经被删除！";
        }

        /// <summary>
        /// 获取可操作状态信息
        /// </summary>
        /// <returns></returns>
        public string OP_GetStatus()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            Hashtable ht = new Hashtable();

            bool CanPackUp = true; //是否可以打包下载.

            #region  PowerModel权限的解析
            string psql = "SELECT A.PowerFlag,A.EmpNo,A.EmpName FROM WF_PowerModel A WHERE PowerCtrlType =1"
             + " UNION "
             + "SELECT A.PowerFlag,B.No,B.Name FROM WF_PowerModel A, Port_Emp B, Port_DeptEmpStation C WHERE A.PowerCtrlType = 0 AND B.No=C.FK_Emp AND A.StaNo = C.FK_Station";
            psql = "SELECT PowerFlag From(" + psql + ")D WHERE  D.EmpNo='" + WebUser.No + "'";

            string powers = DBAccess.RunSQLReturnStringIsNull(psql, "");

            #endregion PowerModel权限的解析

            #region 文件打印的权限判断，这里为天业集团做的特殊判断，现实的应用中，都可以打印.
            if (BP.Difference.SystemConfig.CustomerNo == "TianYe" && WebUser.No != "admin")
                CanPackUp = IsCanPrintSpecForTianYe(gwf);
            #endregion 文件打印的权限判断，这里为天业集团做的特殊判断，现实的应用中，都可以打印.
            if (CanPackUp == true)
                ht.Add("CanPackUp", 1);
            else
                ht.Add("CanPackUp", 0);

            //获取打印的方式PDF/RDF,节点打印方式
            Node nd = new Node(this.NodeID);
            if (nd.HisPrintDocEnable == true)
                ht.Add("PrintType", 1);
            else
                ht.Add("PrintType", 0);


            //是否可以打印.
            switch (gwf.WFState)
            {
                case WFState.Runing: /* 运行时*/
                    /*删除流程.*/
                    if (BP.WF.Dev2Interface.Flow_IsCanDeleteFlowInstance(this.FlowNo, this.WorkID, WebUser.No) == true)
                        ht.Add("IsCanDelete", 1);
                    else
                        ht.Add("IsCanDelete", 0);

                    if (powers.Contains("FlowDataDelete") == true)
                    {
                        //存在移除这个键值
                        if (ht.ContainsKey("IsCanDelete") == true)
                            ht.Remove("IsCanDelete");
                        ht.Add("IsCanDelete", 1);
                    }


                    /*取回审批*/
                    string para = "";
                    string sql = "SELECT NodeID FROM WF_Node WHERE CheckNodes LIKE '%" + gwf.NodeID + "%'";
                    int myNode = DBAccess.RunSQLReturnValInt(sql, 0);
                    if (myNode != 0)
                    {
                        //GetTask gt = new GetTask(myNode);
                        //if (gt.Can_I_Do_It())
                        //{
                        //    ht.Add("TackBackFromNode", gwf.NodeID);
                        //    ht.Add("TackBackToNode", myNode);
                        //    ht.Add("CanTackBack", 1);
                        //}
                    }

                    if (BP.Difference.SystemConfig.CustomerNo == "TianYe")
                    {
                        ht.Add("CanUnSend", 1);

                    }
                    else
                    {
                        /*撤销发送*/
                        GenerWorkerLists workerlists = new GenerWorkerLists();
                        QueryObject info = new QueryObject(workerlists);
                        info.AddWhere(GenerWorkerListAttr.FK_Emp, WebUser.No);
                        info.addAnd();
                        info.AddWhere(GenerWorkerListAttr.IsPass, "1");
                        info.addAnd();
                        info.AddWhere(GenerWorkerListAttr.IsEnable, "1");
                        info.addAnd();
                        info.AddWhere(GenerWorkerListAttr.WorkID, this.WorkID);

                        if (info.DoQuery() > 0)
                            ht.Add("CanUnSend", 1);
                        else
                            ht.Add("CanUnSend", 0);

                        if (powers.Contains("FlowDataUnSend") == true)
                        {
                            //存在移除这个键值
                            if (ht.ContainsKey("CanUnSend") == true)
                                ht.Remove("CanUnSend");
                            ht.Add("CanUnSend", 1);
                        }

                    }


                    //流程结束
                    if (powers.Contains("FlowDataOver") == true)
                    {
                        ht.Add("CanFlowOver", 1);
                    }

                    //催办
                    if (powers.Contains("FlowDataPress") == true)
                    {
                        ht.Add("CanFlowPress", 1);
                    }



                    //是否可以调整工时
                    sql = "SELECT CHRole \"CHRole\" From WF_GenerWorkerlist G,WF_Node N Where G.FK_Node=N.NodeID AND N.CHRole!=0 AND WorkID=" + this.WorkID + " AND FK_Emp='" + WebUser.No + "'";
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (Int32.Parse(dr["CHRole"].ToString()) == 1 || Int32.Parse(dr["CHRole"].ToString()) == 3)
                            {
                                ht.Add("CanChangCHRole", 1);
                                break;
                            }
                            else
                            {
                                ht.Add("CanChangCHRole", 2);
                            }

                        }



                    }



                    break;
                case WFState.Complete: // 完成.
                case WFState.Delete:   // 逻辑删除..
                    /*恢复使用流程*/
                    if (WebUser.No.Equals("admin")==true)
                        ht.Add("CanRollBack", 1);
                    else
                        ht.Add("CanRollBack", 0);

                    if (powers.Contains("FlowDataRollback") == true)
                    {
                        //存在移除这个键值
                        if (ht.ContainsKey("CanRollBack") == true)
                            ht.Remove("CanRollBack");
                        ht.Add("CanRollBack", 1);
                    }

                    if (nd.CHRole != 0)//0禁用 1 启用 2 只读 3 启用并可以调整流程应完成时间
                    {
                        ht.Add("CanChangCHRole", 2);

                    }


                    //判断是否可以打印.
                    break;
                case WFState.Hungup: // 挂起.
                    /*撤销挂起*/
                    if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(WorkID, WebUser.No) == false)
                        ht.Add("CanUnHungup", 0);
                    else
                        ht.Add("CanUnHungup", 1);
                    break;
                default:
                    break;
            }

            return BP.Tools.Json.ToJson(ht);

            //return json + "}";
        }
        /// <summary>
        /// 是否可以打印.
        /// </summary>
        /// <param name="gwf"></param>
        /// <returns></returns>
        private bool IsCanPrintSpecForTianYe(GenerWorkFlow gwf)
        {
            //如果已经完成了，并且节点不是最后一个节点就不能打印.
            if (gwf.WFState == WFState.Complete)
            {
                Node nd = new Node(gwf.NodeID);
                if (nd.ItIsEndNode == false)
                    return false;
            }

            // 判断是否可以打印.
            string sql = "SELECT Distinct NDFrom, EmpFrom FROM ND" + int.Parse(this.FlowNo) + "Track WHERE WorkID=" + this.WorkID;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                //判断节点是否启用了按钮?
                int nodeid = int.Parse(dr[0].ToString());
                BtnLab btn = new BtnLab(nodeid);
                if (btn.PrintPDFEnable == true || btn.PrintZipEnable == true)
                {
                    string empFrom = dr[1].ToString();
                    if (gwf.WFState == BP.WF.WFState.Complete && (BP.Web.WebUser.No == empFrom || gwf.Starter == WebUser.No))
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取OneWork页面的tabs集合
        /// </summary>
        /// <returns></returns>
        public string OneWork_GetTabs()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Url", typeof(string));
            dt.Columns.Add("IsDefault", typeof(int));
            Flow flow = new Flow(this.FlowNo);
            int nodeID = this.NodeID;
            if (nodeID == 0)
            {
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                nodeID = gwf.NodeID;
            }
            DataRow dr = null;

            string paras= string.Format("FK_Node={0}&WorkID={1}&FK_Flow={2}&FID={3}&FromWorkOpt=1&CCSta=" + this.GetRequestValInt("CCSta"),  nodeID.ToString(), this.WorkID, this.FlowNo, this.FID);
            string url = "";
            /*if (flow.IsFrmEnable == true)
            {
                
                Node nd = new Node(nodeID);
                url = "../../MyView.htm?" + paras;
                if ((nd.HisFormType == NodeFormType.SDKForm || nd.HisFormType == NodeFormType.SelfForm))
                {
                    if (nd.FormUrl.Contains("?"))
                        url = "@url=" + nd.FormUrl + "&IsReadonly=1&" + paras;
                    else
                        url = "@url=" + nd.FormUrl + "?IsReadonly=1&" + paras;
                }
                dr = dt.NewRow();
                dr["No"] = "Frm";
                dr["Name"] = "表单";
                dr["Url"] = url;
                dr["IsDefault"] = 0;
                dt.Rows.Add(dr);
            }*/

            if (flow.ItIsTruckEnable == true)
            {
                dr = dt.NewRow();
                dr["No"] = "Truck";
                dr["Name"] = "轨迹图";
                dr["Url"] = "Chart.htm?" + paras;
                dr["IsDefault"] = 0;
                dt.Rows.Add(dr);
            }

            if (flow.ItIsTimeBaseEnable == true)
            {
                dr = dt.NewRow();
                dr["No"] = "TimeBase";
                dr["Name"] = "时间轴";
                dr["Url"] = "TimeBase.htm?" + paras;
                dr["IsDefault"] = 0;
                dt.Rows.Add(dr);
            }

            if (flow.ItIsTableEnable == true)
            {
                dr = dt.NewRow();
                dr["No"] = "Table";
                dr["Name"] = "时间表";
                dr["Url"] = "Table.htm?" + paras;
                dr["IsDefault"] = 0;
                dt.Rows.Add(dr);
            }

            

            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获取流程的JSON数据，以供显示工作轨迹/流程设计
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作编号</param>
        /// <param name="fid">父流程编号</param>
        /// <returns></returns>
        public string Chart_Init2020()
        {
            //参数.
            string fk_flow = this.FlowNo;
            Int64 workid = this.WorkID;
            Int64 fid = this.FID;

            DataSet ds = new DataSet();
            DataTable dt = null;
            string json = string.Empty;
            try
            {
                //流程信息
                string sql = "SELECT No \"No\", Name \"Name\", ChartType \"ChartType\" FROM WF_Flow WHERE No='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_Flow";
                ds.Tables.Add(dt);

                //节点信息 ， 
                // NodePosType=0，开始节点， 1中间节点,2=结束节点.
                // RunModel= select * FROM sys_enums where Enumkey='RunModel' 
                // TodolistModel= select * FROM sys_enums where Enumkey='TodolistModel' ;
                sql = "SELECT NodeID \"ID\", Name \"Name\", ICON \"Icon\", X \"X\", Y \"Y\", NodePosType \"NodePosType\",RunModel \"RunModel\",HisToNDs \"HisToNDs\",TodolistModel \"TodolistModel\" FROM WF_Node WHERE FK_Flow='" +
                    fk_flow + "' ORDER BY Step";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_Node";
                ds.Tables.Add(dt);

                //标签信息
                sql = "SELECT MyPK \"MyPK\", Name \"Name\", X \"X\", Y \"Y\" FROM WF_LabNote WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_LabNote";
                ds.Tables.Add(dt);

                //线段方向信息
                sql = "SELECT Node \"Node\", ToNode \"ToNode\", 0 as  \"DirType\", 0 as \"IsCanBack\" FROM WF_Direction WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_Direction";
                ds.Tables.Add(dt);

                //如果workid=0就仅仅返回流程图数据.
                if (workid == 0)
                    return BP.Tools.Json.DataSetToJson(ds);


                //流程信息.
                GenerWorkFlow gwf = new GenerWorkFlow(workid);
                dt = gwf.ToDataTableField(); // DBAccess.RunSQLReturnTable(string.Format(sql, workid));
                dt.TableName = "WF_GenerWorkFlow";
                ds.Tables.Add(dt);

                //把节点审核配置信息.
                NodeWorkCheck fwc = new NodeWorkCheck(gwf.NodeID);
                ds.Tables.Add(fwc.ToDataTableField("FrmWorkCheck"));


                //获取工作轨迹信息
                string trackTable = "ND" + int.Parse(fk_flow) + "Track";
                sql = "SELECT FID \"FID\",NDFrom \"NDFrom\",NDFromT \"NDFromT\",NDTo  \"NDTo\", NDToT \"NDToT\", ActionType \"ActionType\",ActionTypeText \"ActionTypeText\",Msg \"Msg\",RDT \"RDT\",EmpFrom \"EmpFrom\",EmpFromT \"EmpFromT\", EmpToT \"EmpToT\",EmpTo \"EmpTo\" FROM " + trackTable +
                      " WHERE WorkID=" +
                      workid + (fid == 0 ? (" OR FID=" + workid) : (" OR WorkID=" + fid + " OR FID=" + fid)) + " ORDER BY RDT DESC";
                dt = DBAccess.RunSQLReturnTable(sql);

                DataTable newdt = new DataTable();
                newdt = dt.Clone();

                #region 判断轨迹数据中，最后一步是否是撤销或退回状态的，如果是，则删除最后2条数据
                if (dt.Rows.Count > 0)
                {
                    if (Equals(dt.Rows[0]["ActionType"], (int)ActionType.Return)
                        || Equals(dt.Rows[0]["ActionType"], (int)ActionType.UnSend))
                    {
                        if (dt.Rows.Count > 1)
                        {
                            dt.Rows.RemoveAt(1);
                            dt.Rows.RemoveAt(0);
                        }
                        else
                        {
                            dt.Rows.RemoveAt(0);
                        }
                        newdt = dt;
                    }
                    else if (dt.Rows.Count > 1 && (Equals(dt.Rows[1]["ActionType"], (int)ActionType.Return) || Equals(dt.Rows[1]["ActionType"], (int)ActionType.UnSend)))
                    {
                        //删除已发送的节点，
                        if (dt.Rows.Count > 3)
                        {
                            dt.Rows.RemoveAt(1);
                            dt.Rows.RemoveAt(1);
                        }
                        else
                        {
                            dt.Rows.RemoveAt(1);
                        }

                        string fk_node = "";
                        if (dt.Rows[0]["NDFrom"].Equals(dt.Rows[0]["NDTo"]))
                            fk_node = dt.Rows[0]["NDFrom"].ToString();
                        if (DataType.IsNullOrEmpty(fk_node) == false)
                        {
                            //如果是跳转页面，则需要删除中间跳转的节点
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (Equals(dr["ACTIONTYPE"], (int)ActionType.Skip) && dr["NDFrom"].ToString().Equals(fk_node))
                                    continue;
                                DataRow newdr = newdt.NewRow();
                                newdr.ItemArray = dr.ItemArray;
                                newdt.Rows.Add(newdr);
                            }
                        }
                        else
                        {
                            newdt = dt.Copy();
                        }
                    }
                    else
                        newdt = dt.Copy();
                }
                newdt.TableName = "Track";
                ds.Tables.Add(newdt);
                #endregion

                #region 如果流程没有完成,就把工作人员列表返回过去.
                if (gwf.WFState != WFState.Complete)
                {
                    //加入工作人员列表.
                    GenerWorkerLists gwls = new GenerWorkerLists();
                    Int64 id = this.FID;
                    if (id == 0)
                        id = this.WorkID;

                    QueryObject qo = new QueryObject(gwls);
                    qo.AddWhere(GenerWorkerListAttr.FID, id);
                    qo.addOr();
                    qo.AddWhere(GenerWorkerListAttr.WorkID, id);
                    qo.DoQuery();

                    DataTable dtGwls = gwls.ToDataTableField("WF_GenerWorkerlist");
                    ds.Tables.Add(dtGwls);
                }
                #endregion 如果流程没有完成,就把工作人员列表返回过去.

                string str= BP.Tools.Json.DataSetToJson(ds);
                 //DataType.WriteFile("c:\\GetFlowTrackJsonData_CCflow.txt", str);
                 return str;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 获得最后一个人的审批意见
        /// </summary>
        /// <returns></returns>
        public string SubFlowGuid_GenerLastOneCheckNote()
        {
            string table="ND"+int.Parse(this.FlowNo )+"Track";
            string sql = "SELECT Msg, WriteDB FROM "+table+" WHERE WorkID="+this.WorkID+ " AND ActionType=1 ORDER BY RDT DESC ";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            string info = dt.Rows[0][0].ToString();
            if (info.Contains("WorkCheck@") == true)
                info = info.Substring( info.IndexOf("WorkCheck@") + 10 );

            Hashtable ht = new Hashtable();
            ht.Add("Msg", info);
            ht.Add("WriteDB", dt.Rows[0][1]);

            return BP.Tools.Json.ToJson(ht);

        }
        

        public string Chart_Init()
        {
            string fk_flow = this.FlowNo;
            Int64 workid = this.WorkID;
            Int64 fid = this.FID;

            DataSet ds = new DataSet();
            DataTable dt = null;
            string json = string.Empty;
           
            try
            {
                //获取流程信息
                string sql = "SELECT No \"No\", Name \"Name\",  ChartType \"ChartType\" FROM WF_Flow WHERE No='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_Flow";
                ds.Tables.Add(dt);

                //获取流程中的节点信息
                sql = "SELECT NodeID \"ID\", Name \"Name\", ICON \"Icon\", X \"X\", Y \"Y\", NodePosType \"NodePosType\",RunModel \"RunModel\",HisToNDs \"HisToNDs\",TodolistModel \"TodolistModel\" ,NodeType\"NodeType\",Step\"Step\" FROM WF_Node WHERE FK_Flow='" +
                    fk_flow + "' ORDER BY Step";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_Node";
                ds.Tables.Add(dt);

                //获取流程中的标签信息
                sql = "SELECT MyPK \"MyPK\", Name \"Name\", X \"X\", Y \"Y\" FROM WF_LabNote WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_LabNote";
                ds.Tables.Add(dt);

                //获取流程中的线段方向信息
                sql = "SELECT Node \"Node\", ToNode \"ToNode\", 0 as  \"DirType\", 0 as \"IsCanBack\" FROM WF_Direction WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_Direction";
                ds.Tables.Add(dt);

                if (workid != 0)
                {
                    //获取流程信息，added by liuxc,2016-10-26
                    //sql =
                    //    "SELECT wgwf.Starter,wgwf.StarterName,wgwf.RDT,wgwf.WFSta,wgwf.WFState FROM WF_GenerWorkFlow wgwf WHERE wgwf.WorkID = " +
                    //    workid;                     
                    sql = "SELECT wgwf.Starter as \"Starter\","
                          + "        wgwf.StarterName as \"StarterName\","
                          + "        wgwf.RDT as \"RDT\","
                          + "        wgwf.WFSta as \"WFSta\","
                          + "        se.Lab  as   \"WFStaText\","
                          + "        wgwf.WFState as \"WFState\","
                          + "        wgwf.FID as \"FID\","
                          + "        wgwf.PWorkID as \"PWorkID\","
                          + "        wgwf.PFlowNo as \"PFlowNo\","
                          + "        wgwf.PNodeID as \"PNodeID\","
                          + "        wgwf.FK_Flow as \"FK_Flow\","
                          + "        wgwf.FK_Node as \"FK_Node\","
                          + "        wgwf.Title as \"Title\","
                          + "        wgwf.WorkID as \"WorkID\","
                          + "        wgwf.NodeName as \"NodeName\","
                          + "        wf.Name  as   \"FlowName\""
                          + " FROM   WF_GenerWorkFlow wgwf"
                          + "        INNER JOIN WF_Flow wf"
                          + "             ON  wf.No=wgwf.FK_Flow"
                          + "        INNER JOIN Sys_Enum se"
                          + "             ON  se.IntKey = wgwf.WFSta"
                          + "             AND se.EnumKey = 'WFSta'"
                          + " WHERE  wgwf.WorkID = {0}"
                          + "        OR  wgwf.FID = {0}"
                          + "        OR  wgwf.PWorkID = {0}"
                          + " ORDER BY"
                          + "        wgwf.RDT DESC";

                    dt = DBAccess.RunSQLReturnTable(string.Format(sql, workid));
                    dt.TableName = "FlowInfo";
                    ds.Tables.Add(dt);

                    //获得流程状态.
                    WFState wfState = (WFState)int.Parse(dt.Select("workid=" + workid + "")[0]["wfstate"].ToString());// (WFState)int.Parse(dt.Rows[0]["WFState"].ToString());

                    String fk_Node = dt.Rows[0]["FK_Node"].ToString();

                    //把节点审核配置信息.
                    NodeWorkCheck fwc = new NodeWorkCheck(fk_Node);
                    ds.Tables.Add(fwc.ToDataTableField("FrmWorkCheck"));


                    //获取工作轨迹信息
                    string trackTable = "ND" + int.Parse(fk_flow) + "Track";
                    sql = "SELECT FID \"FID\",NDFrom \"NDFrom\",NDFromT \"NDFromT\",NDTo  \"NDTo\", NDToT \"NDToT\", ActionType \"ActionType\",ActionTypeText \"ActionTypeText\",Msg \"Msg\",RDT \"RDT\",EmpFrom \"EmpFrom\",EmpFromT \"EmpFromT\", EmpToT \"EmpToT\",EmpTo \"EmpTo\",NodeData \"NodeData\" FROM " + trackTable +
                          " WHERE WorkID=" +
                          workid + (fid == 0 ? (" OR FID=" + workid) : (" OR WorkID=" + fid + " OR FID=" + fid)) + " ORDER BY RDT DESC";

                    dt = DBAccess.RunSQLReturnTable(sql);
                    DataTable newdt = new DataTable();
                    newdt = dt.Clone();

                    //判断轨迹数据中，最后一步是否是撤销或退回状态的，如果是，则删除最后2条数据
                    if (dt.Rows.Count > 0)
                    {
                        if (Equals(dt.Rows[0]["ActionType"], (int)ActionType.Return) || Equals(dt.Rows[0]["ActionType"], (int)ActionType.UnSend))
                        {
                            if (dt.Rows.Count > 1)
                            {
                                dt.Rows.RemoveAt(1);
                                dt.Rows.RemoveAt(0);
                            }
                            else
                            {
                                dt.Rows.RemoveAt(0);
                            }

                            newdt = dt;
                        }
                        else if (dt.Rows.Count > 1 && (Equals(dt.Rows[1]["ActionType"], (int)ActionType.Return) || Equals(dt.Rows[1]["ActionType"], (int)ActionType.UnSend)))
                        {
                            //删除已发送的节点，
                            if (dt.Rows.Count > 3)
                            {
                                dt.Rows.RemoveAt(1);
                                dt.Rows.RemoveAt(1);
                            }
                            else
                            {
                                dt.Rows.RemoveAt(1);
                            }

                            string fk_node = "";
                            if (dt.Rows[0]["NDFrom"].Equals(dt.Rows[0]["NDTo"]))
                                fk_node = dt.Rows[0]["NDFrom"].ToString();
                            if (DataType.IsNullOrEmpty(fk_node) == false)
                            {
                                //如果是跳转页面，则需要删除中间跳转的节点
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (Equals(dr["ACTIONTYPE"], (int)ActionType.Skip) && dr["NDFrom"].ToString().Equals(fk_node))
                                        continue;
                                    DataRow newdr = newdt.NewRow();
                                    newdr.ItemArray = dr.ItemArray;
                                    newdt.Rows.Add(newdr);
                                }
                            }
                            else
                            {
                                newdt = dt.Copy();
                            }
                        }
                        else
                            newdt = dt.Copy();
                    }

                    newdt.TableName = "Track";
                    ds.Tables.Add(newdt);

                    //获取预先计算的节点处理人，以及处理时间,added by liuxc,2016-4-15
                    sql = "SELECT wsa.FK_Node as \"FK_Node\",wsa.FK_Emp as \"FK_Emp\",wsa.EmpName as \"EmpName\",wsa.TimeLimit as \"TimeLimit\",wsa.TSpanHour as \"TSpanHour\",wsa.ADT as \"ADT\",wsa.SDT as \"SDT\" FROM WF_SelectAccper wsa WHERE wsa.WorkID = " + workid;
                    dt = DBAccess.RunSQLReturnTable(sql);
                    // dt.TableName = "POSSIBLE";
                    dt.TableName = "Possible";
                    ds.Tables.Add(dt);

                    //获取节点处理人数据，及处理/查看信息
                    sql = "SELECT wgw.FK_Emp as \"FK_Emp\",wgw.FK_Node as \"FK_Node\",wgw.EmpName as \"FK_EmpText\",wgw.RDT as \"RDT\",wgw.IsRead as \"IsRead\",wgw.IsPass as \"IsPass\" FROM WF_GenerWorkerlist wgw WHERE wgw.WorkID = " +
                          workid + (fid == 0 ? (" OR FID=" + workid) : (" OR WorkID=" + fid + " OR FID=" + fid));
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "DISPOSE";
                    ds.Tables.Add(dt);


                    //如果流程没有完成.
                    if (wfState != WFState.Complete)
                    {
                        GenerWorkerLists gwls = new GenerWorkerLists();
                        Int64 id = this.FID;
                        if (id == 0)
                            id = this.WorkID;

                        QueryObject qo = new QueryObject(gwls);
                        qo.AddWhere(GenerWorkerListAttr.FID, id);
                        qo.addOr();
                        qo.AddWhere(GenerWorkerListAttr.WorkID, id);
                        qo.addOrderBy(GenerWorkerListAttr.Idx);
                        qo.DoQuery();
                        
                       
                        DataTable dtGwls = gwls.ToDataTableField("WF_GenerWorkerlist");
                        ds.Tables.Add(dtGwls);
                    }

                }
                else
                {
                    string trackTable = "ND" + int.Parse(fk_flow) + "Track";
                    sql = "SELECT NDFrom \"NDFrom\", NDTo \"NDTo\",ActionType \"ActionType\",ActionTypeText \"ActionTypeText\",Msg \"Msg\",RDT \"RDT\",EmpFrom \"EmpFrom\",EmpFromT \"EmpFromT\",EmpToT \"EmpToT\",EmpTo \"EmpTo\",NodeData \"NodeData\" FROM " + trackTable +
                          " WHERE WorkID=0 ORDER BY RDT ASC";
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "TRACK";
                    ds.Tables.Add(dt);
                }

                //for (int i = 0; i < ds.Tables.Count; i++)
                //{
                //    dt = ds.Tables[i];
                //    dt.TableName = dt.TableName.ToUpper();
                //    for (int j = 0; j < dt.Columns.Count; j++)
                //    {
                //        dt.Columns[j].ColumnName = dt.Columns[j].ColumnName.ToUpper();
                //    }
                //}

                //获取子流程
                SubFlows subFlows = new SubFlows(this.FlowNo);
                ds.Tables.Add(subFlows.ToDataTableField("WF_NodeSubFlow"));

                //获取发起的子流程
                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID, "WorkID");
                ds.Tables.Add(gwfs.ToDataTableField("WF_GenerWorkFlow"));
                string str = BP.Tools.Json.DataSetToJson(ds);
                //  DataType.WriteFile("c:\\GetFlowTrackJsonData_CCflow.txt", str);
                return str;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return json;
        }
        private WebUserCopy _webUserCopy = null;
        public WebUserCopy WebUser
        {
            get
            {
                if (_webUserCopy == null)
                {
                    _webUserCopy = new WebUserCopy();
                    _webUserCopy.LoadWebUser();
                }
                return _webUserCopy;
            }
            set
            {
                _webUserCopy = value;
            }
        }
        /// <summary>
        /// 只获取未来的节点集合
        /// </summary>
        /// <returns></returns>
        public string GetFutureNodeNotContainsEmps()
        {
            string nodeIDs = "";
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (gwf.WFState == WFState.Complete)
                return nodeIDs;

           
            int currNodeID = gwf.NodeID;
            //获取未来处理人的节点
            Node nd = new Node(currNodeID);
            GERpt rptGe = nd.HisFlow.HisGERpt;
            rptGe.OID = this.WorkID;
            rptGe.RetrieveFromDBSources();
            Node hisNode = nd;
            while (nd.ItIsEndNode == false)
            {
                nd = NodeSend_GenerNextStepNode_Ext(nd, rptGe, this.WebUser, hisNode);
                if (nd == null)
                    break;
                nodeIDs += nd.NodeID + ",";
                if (nd.HisNodeType == NodeType.UserNode)
                    hisNode = nd;
            }   
            return nodeIDs;
        }
        public string GetFutureNodes()
        {
            DataSet ds = new DataSet();
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (gwf.WFState == WFState.Complete)
                return BP.Tools.Json.ToJson(ds);

            SelectAccpers accpers = new SelectAccpers();
            accpers.Retrieve(SelectAccperAttr.WorkID, this.WorkID);
            SelectAccpers selectAccpers = new SelectAccpers();
            int currNodeID = gwf.NodeID;
            //获取未来处理人的节点
            Node nd = new Node(currNodeID);
            GERpt rptGe = nd.HisFlow.HisGERpt;
            rptGe.OID = this.WorkID;
            rptGe.RetrieveFromDBSources();
            SelectAccpers curaccpers = null;
            SelectAccper accper = null;
            Node hisNode = nd;
            Nodes nds = new Nodes();
            while (nd.ItIsEndNode==false)
            {
                nd = NodeSend_GenerNextStepNode_Ext(nd, rptGe, this.WebUser, hisNode);
                if (nd == null)
                    break;
                nds.AddEntity(nd);
                curaccpers = accpers.GetEntitiesByKey(SelectAccperAttr.WorkID, this.WorkID, SelectAccperAttr.FK_Node, nd.NodeID) as SelectAccpers;
                if (curaccpers != null)
                {
                    selectAccpers.AddEntities(curaccpers);
                    accper = curaccpers[0] as SelectAccper;
                    this.WebUser.No = accper.EmpNo;
                    this.WebUser.Name = accper.EmpName;
                    this.WebUser.DeptNo = accper.DeptNo;
                    this.WebUser.DeptName = accper.DeptName;
                }
                if (nd.HisNodeType == NodeType.UserNode)
                    hisNode = nd;
                if (nd.HisNodeType == NodeType.RouteNode && accper == null)
                {
                    accper = new SelectAccper();
                    accper.NodeID = nd.NodeID;
                    accper.WorkID = this.WorkID;
                    selectAccpers.AddEntity(accper);
                }
            }
            ds.Tables.Add(nds.ToDataTableField("Nodes"));
            ds.Tables.Add(selectAccpers.ToDataTableField("SelectAccpers"));
            return BP.Tools.Json.ToJson(ds);
        }

        private Node NodeSend_GenerNextStepNode_Ext(Node currNode,GERpt rptGe,WebUserCopy webUser,Node hisNode)
        {
            Nodes nds = currNode.HisToNodes;
            if (nds.Count == 1)
            {
                Node toND = (Node)nds[0];
                return toND;
            }
            if (nds.Count == 0)
                return null;
            //获得所有的方向,按照优先级, 按照条件处理方向，如果成立就返回.
            Directions dirs = new Directions(currNode.NodeID);
            Node nd = null;
            if (dirs.Count == 1)
            {
                nd = new Node(dirs[0].GetValIntByKey(DirectionAttr.ToNode));
                return nd;
            }
            //定义没有条件的节点集合.
            Directions dirs0Cond = new Directions();
            
            foreach (Direction dir in dirs)
            {
                //查询出来他的条件.
                Conds conds = new Conds();
                conds.Retrieve(CondAttr.FK_Node, currNode.NodeID,
                    CondAttr.ToNodeID, dir.ToNode, CondAttr.CondType,
                    (int)CondType.Dir,
                    CondAttr.Idx);

                //可以到达的节点.
                if (conds.Count == 0)
                {
                    dirs0Cond.AddEntity(dir); //把他加入到里面.
                    continue;
                }

                //按条件计算.
                if (conds.GenerResult(rptGe, webUser, hisNode) == true)
                {
                    nd = new Node(dir.ToNode);
                    return nd;
                }
            }

            if (dirs0Cond.Count == 0)
                return null;

            if (dirs0Cond.Count != 1)
                return null;

            int toNodeID = dirs0Cond[0].GetValIntByKey(DirectionAttr.ToNode);
            nd = new Node(toNodeID);
            return nd;
        }
        /// <summary>
        /// 获取流程的JSON数据，以供显示工作轨迹/流程设计
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作编号</param>
        /// <param name="fid">父流程编号</param>
        /// <returns></returns>
        public string GetFlowTrackJsonData()
        {
            string fk_flow = this.FlowNo;
            Int64 workid = this.WorkID;
            Int64 fid = this.FID;


            DataSet ds = new DataSet();
            DataTable dt = null;
            try
            {
                //获取流程信息
                string sql = "SELECT No \"No\", Name \"Name\",  ChartType \"ChartType\" FROM WF_Flow WHERE No='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_FLOW";
                ds.Tables.Add(dt);

                //获取流程中的节点信息
                sql = "SELECT NodeID \"ID\", Name \"Name\", ICON \"Icon\", X \"X\", Y \"Y\", NodePosType \"NodePosType\", RunModel \"RunModel\",HisToNDs \"HisToNDs\",TodolistModel \"TodolistModel\" FROM WF_Node WHERE FK_Flow='" +
                    fk_flow + "' ORDER BY Step";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_NODE";
                ds.Tables.Add(dt);

                //获取流程中的标签信息
                sql = "SELECT MyPK \"MyPK\", Name \"Name\", X \"X\", Y \"Y\" FROM WF_LabNote WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_LABNOTE";
                ds.Tables.Add(dt);

                //获取流程中的线段方向信息
                sql = "SELECT Node \"Node\", ToNode \"ToNode\", 0 as  \"DirType\", 0 as \"IsCanBack\" FROM WF_Direction WHERE FK_Flow='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_DIRECTION";
                ds.Tables.Add(dt);

                if (workid != 0)
                {
                    //获取流程信息，added by liuxc,2016-10-26
                    //sql =
                    //    "SELECT wgwf.Starter,wgwf.StarterName,wgwf.RDT,wgwf.WFSta,wgwf.WFState FROM WF_GenerWorkFlow wgwf WHERE wgwf.WorkID = " +
                    //    workid;
                    sql = "SELECT wgwf.Starter as \"Starter\","
                          + "        wgwf.StarterName as \"StarterName\","
                          + "        wgwf.RDT as \"RDT\","
                          + "        wgwf.WFSta as \"WFSta\","
                          + "        se.Lab  as   \"WFStaText\","
                          + "        wgwf.WFState as \"WFState\","
                          + "        wgwf.FID as \"FID\","
                          + "        wgwf.PWorkID as \"PWorkID\","
                          + "        wgwf.PFlowNo as \"PFlowNo\","
                          + "        wgwf.PNodeID as \"PNodeID\","
                          + "        wgwf.FK_Flow as \"FK_Flow\","
                          + "        wgwf.FK_Node as \"FK_Node\","
                          + "        wgwf.Title as \"Title\","
                          + "        wgwf.WorkID as \"WorkID\","
                          + "        wgwf.NodeName as \"NodeName\","
                          + "        wf.Name  as   \"FlowName\""
                          + " FROM   WF_GenerWorkFlow wgwf"
                          + "        INNER JOIN WF_Flow wf"
                          + "             ON  wf.No=wgwf.FK_Flow"
                          + "        INNER JOIN Sys_Enum se"
                          + "             ON  se.IntKey = wgwf.WFSta"
                          + "             AND se.EnumKey = 'WFSta'"
                          + " WHERE  wgwf.WorkID = {0}"
                          + "        OR  wgwf.FID = {0}"
                          + "        OR  wgwf.PWorkID = {0}"
                          + " ORDER BY"
                          + "        wgwf.RDT DESC";

                    dt = DBAccess.RunSQLReturnTable(string.Format(sql, workid));
                    dt.TableName = "FLOWINFO";
                    ds.Tables.Add(dt);

                    //获取工作轨迹信息
                    string trackTable = "ND" + int.Parse(fk_flow) + "Track";
                    sql = "SELECT NDFrom \"NDFrom\",NDFromT \"NDFromT\",NDTo  \"NDTo\", NDToT \"NDToT\", ActionType \"ActionType\",ActionTypeText \"ActionTypeText\",Msg \"Msg\",RDT \"RDT\",EmpFrom \"EmpFrom\",EmpFromT \"EmpFromT\", EmpToT \"EmpToT\",EmpTo \"EmpTo\" FROM " + trackTable +
                          " WHERE WorkID=" +
                          workid + (fid == 0 ? (" OR FID=" + workid) : (" OR WorkID=" + fid + " OR FID=" + fid)) + " ORDER BY RDT ASC";

                    dt = DBAccess.RunSQLReturnTable(sql);

                    //判断轨迹数据中，最后一步是否是撤销或退回状态的，如果是，则删除最后2条数据
                    if (dt.Rows.Count > 0)
                    {
                        if (Equals(dt.Rows[0]["ACTIONTYPE"], (int)ActionType.Return) || Equals(dt.Rows[0]["ACTIONTYPE"], (int)ActionType.UnSend))
                        {
                            if (dt.Rows.Count > 1)
                            {
                                dt.Rows.RemoveAt(0);
                                dt.Rows.RemoveAt(0);
                            }
                            else
                            {
                                dt.Rows.RemoveAt(0);
                            }
                        }
                    }

                    dt.TableName = "TRACK";
                    ds.Tables.Add(dt);

                    //获取预先计算的节点处理人，以及处理时间,added by liuxc,2016-4-15
                    sql = "SELECT wsa.FK_Node as \"FK_Node\",wsa.FK_Emp as \"FK_Emp\",wsa.EmpName as \"EmpName\",wsa.TimeLimit as \"TimeLimit\",wsa.TSpanHour as \"TSpanHour\",wsa.ADT as \"ADT\",wsa.SDT as \"SDT\" FROM WF_SelectAccper wsa WHERE wsa.WorkID = " + workid;
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "POSSIBLE";
                    ds.Tables.Add(dt);

                    //获取节点处理人数据，及处理/查看信息
                    sql = "SELECT wgw.FK_Emp as \"FK_Emp\",wgw.FK_Node as \"FK_Node\",wgw.EmpName as \"EmpName\",wgw.RDT as \"RDT\",wgw.IsRead as \"IsRead\",wgw.IsPass as \"IsPass\" FROM WF_GenerWorkerlist wgw WHERE wgw.WorkID = " +
                          workid + (fid == 0 ? (" OR FID=" + workid) : (" OR WorkID=" + fid + " OR FID=" + fid));
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "DISPOSE";
                    ds.Tables.Add(dt);
                }
                else
                {
                    string trackTable = "ND" + int.Parse(fk_flow) + "Track";
                    sql = "SELECT NDFrom \"NDFrom\", NDTo \"NDTo\",ActionType \"ActionType\",ActionTypeText \"ActionTypeText\",Msg \"Msg\",RDT \"RDT\",EmpFrom \"EmpFrom\",EmpFromT \"EmpFromT\",EmpToT \"EmpToT\",EmpTo \"EmpTo\" FROM " + trackTable +
                          " WHERE WorkID=0 ORDER BY RDT ASC";
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "TRACK";
                    ds.Tables.Add(dt);
                }

                //for (int i = 0; i < ds.Tables.Count; i++)
                //{
                //    dt = ds.Tables[i];
                //    dt.TableName = dt.TableName.ToUpper();
                //    for (int j = 0; j < dt.Columns.Count; j++)
                //    {
                //        dt.Columns[j].ColumnName = dt.Columns[j].ColumnName.ToUpper();
                //    }
                //}

                string str = BP.Tools.Json.DataSetToJson(ds);
                //  DataType.WriteFile("c:\\GetFlowTrackJsonData_CCflow.txt", str);
                return str;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 获得发起的BBS评论.
        /// </summary>
        /// <returns></returns>
        public string FlowBBSList()
        {
            BP.CCBill.Tracks tracks = new BP.CCBill.Tracks();
            BP.En.QueryObject qo = new En.QueryObject(tracks);
            qo.AddWhere(TrackAttr.ActionType, BP.CCBill.FrmActionType.BBS);
            qo.addAnd();
            qo.addLeftBracket();

            if (this.FID != 0)
            {
                qo.AddWhere(TrackAttr.WorkID, this.FID);
                qo.addOr();
                qo.AddWhere(TrackAttr.FID, this.FID);
            }
            else
            {
                qo.AddWhere(TrackAttr.WorkID, this.WorkID);

                if (this.WorkID != 0)
                {
                    qo.addOr();
                    qo.AddWhere(TrackAttr.FID, this.WorkID);
                }
            }
            qo.addRightBracket();
            qo.addOrderBy(TrackAttr.RDT);
            qo.DoQuery();
            //转化成json
            return BP.Tools.Json.ToJson(tracks.ToDataTableField());
        }

        /// 查看某一用户的评论.
        public string FlowBBS_Check()
        {
            Paras pss = new Paras();
            pss.SQL = "SELECT * FROM ND" + int.Parse(this.FlowNo) + "Track WHERE ActionType=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID AND  EMPFROMT='" + this.UserName + "'";
            pss.Add("ActionType", (int)BP.WF.ActionType.FlowBBS);
            pss.Add("WorkID", this.WorkID);

            return BP.Tools.Json.ToJson(DBAccess.RunSQLReturnTable(pss));
        }
        /// <summary>
        /// 提交评论.
        /// </summary>
        /// <returns></returns>
        public string FlowBBS_Save()
        {
            string msg = this.GetValFromFrmByKey("FlowBBS_Doc");
            string fk_mapData = this.GetRequestVal("FK_MapData");
            Node nd = new Node(this.NodeID);
            if (DataType.IsNullOrEmpty(fk_mapData) == true)
            {
                fk_mapData = nd.NodeFrmID;
            }
            MapData mapData = new MapData(fk_mapData);
            BP.WF.Dev2Interface.Track_WriteBBS(fk_mapData, mapData.Name, this.WorkID, msg,
           this.FID, this.FlowNo, null, this.NodeID, nd.Name);
            return "评论信息保存成功";
        }

        /// <summary>
        /// 回复评论.
        /// </summary>
        /// <returns></returns>
        public string FlowBBS_Replay()
        {
            SMS sms = new SMS();
            sms.RetrieveByAttr(SMSAttr.MyPK, MyPK);
            sms.setMyPK(DBAccess.GenerGUID());
            sms.RDT = DataType.CurrentDateTime;
            sms.SendToEmpNo = this.UserName;
            sms.Sender = WebUser.No;
            sms.Title = this.Title;
            sms.DocOfEmail = this.Msg;
            sms.Insert();
            return null;
        }
        /// <summary>
        /// 统计评论条数.
        /// </summary>
        /// <returns></returns>
        public string FlowBBS_Count()
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT COUNT(ActionType) FROM ND" + int.Parse(this.FlowNo) + "Track WHERE ActionType=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("ActionType", (int)BP.WF.ActionType.FlowBBS);
            ps.Add("WorkID", this.WorkID);
            string count = DBAccess.RunSQLReturnValInt(ps).ToString();
            return count;
        }

        
    }
}
