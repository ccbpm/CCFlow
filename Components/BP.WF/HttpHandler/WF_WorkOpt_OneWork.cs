using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.DA;
using BP.Sys;
using BP.WF.XML;
using BP.WF.Template;
using BP.Web;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_WorkOpt_OneWork : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_WorkOpt_OneWork(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        /// <summary>
        /// 时间轴
        /// </summary>
        /// <returns></returns>
        public string TimeBase_Init()
        {
            DataSet ds = new DataSet();

            //获取track.
            DataTable dt = BP.WF.Dev2Interface.DB_GenerTrackTable(this.FK_Flow, this.WorkID, this.FID);
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
                            msg = "<p>操作员:{" + dr[TrackAttr.EmpFromT].ToString() + "}在当前节点上，被父流程{" + mygwf.FlowName + "},<a target=b" + ap.GetValStrByKey("PWorkID") + " href='Track.aspx?WorkID=" + ap.GetValStrByKey("PWorkID") + "&FK_Flow=" + ap.GetValStrByKey("PFlowNo") + "' >" + msg + "</a></p>";
                        else
                            msg = "<p>操作员:{" + dr[TrackAttr.EmpFromT].ToString() + "}在当前节点上，被父流程调用{" + mygwf.FlowName + "}，但是该流程被删除了.</p>" + tag;

                        msg = "<a target=b" + ap.GetValStrByKey("PWorkID") + " href='Track.aspx?WorkID=" + ap.GetValStrByKey("PWorkID") + "&FK_Flow=" + ap.GetValStrByKey("PFlowNo") + "' >" + msg + "</a>";
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
                            msg = "<p>操作员:{" + dr[TrackAttr.EmpFromT].ToString() + "}在当前节点上调用了子流程{" + mygwf.FlowName + "}, <a target=b" + ap.GetValStrByKey("CWorkID") + " href='Track.aspx?WorkID=" + ap.GetValStrByKey("CWorkID") + "&FK_Flow=" + ap.GetValStrByKey("CFlowNo") + "' >" + msg + "</a></p>";
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

            //获取 WF_GenerWorkFlow
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = this.WorkID;
            gwf.RetrieveFromDBSources();
            ds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            if (gwf.WFState != WFState.Complete)
            {
                GenerWorkerLists gwls = new GenerWorkerLists();
                gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID);

                ds.Tables.Add(gwls.ToDataTableField("WF_GenerWorkerList"));
            }

            //返回结果.
            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        public string TimeBase_OpenFrm()
        {
            WF en = new WF(this.context);
            return en.Runing_OpenFrm();
        }
        /// <summary>
        /// 返回打开路径
        /// </summary>
        /// <returns></returns>
        public string FrmGuide_Init()
        {
            WF en = new WF(this.context);
            return en.Runing_OpenFrm();
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
                string str =  this.GetRequestVal("Title");
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
                string str =this.GetRequestVal("EnumKey");
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
            return BP.WF.Dev2Interface.Flow_BBSDelete(this.FK_Flow, this.MyPK, WebUser.No);
        }
        /// <summary>
        /// 执行撤销
        /// </summary>
        /// <returns></returns>
        public string OP_UnSend()
        {
            try
            {
                return BP.WF.Dev2Interface.Flow_DoUnSend(this.FK_Flow, this.WorkID);
            }
            catch(Exception ex)
            {
                return "err@"+ex.Message;
            }
        }
        protected override string DoDefaultMethod()
        {
            return "err@没有判断的执行类型：" + this.DoType+" @类 "+this.ToString();
        }

        public string OP_ComeBack()
        {
            WorkFlow wf3 = new WorkFlow(FK_Flow, WorkID);
            wf3.DoComeBackWorkFlow("无");
            return "流程已经被重新启用.";
        }

        public string OP_UnHungUp()
        {
            WorkFlow wf2 = new WorkFlow(FK_Flow, WorkID);
            //  wf2.DoUnHungUp();
            return "流程已经被解除挂起.";
        }

        public string OP_HungUp()
        {
            WorkFlow wf1 = new WorkFlow(FK_Flow, WorkID);
            //wf1.DoHungUp()
            return "流程已经被挂起.";
        }

        public string OP_DelFlow()
        {
            WorkFlow wf = new WorkFlow(FK_Flow, WorkID);
            wf.DoDeleteWorkFlowByReal(true);
            return "流程已经被删除！";
        }

        /// <summary>
        /// 获取可操作状态信息
        /// </summary>
        /// <returns></returns>
        public string OP_GetStatus()
        {
            int wfState = BP.DA.DBAccess.RunSQLReturnValInt("SELECT WFState FROM WF_GenerWorkFlow WHERE WorkID=" + WorkID, 1);
            WFState wfstateEnum = (WFState)wfState;

            Hashtable ht = new Hashtable();

            string json = "{";
            bool isCan;

            #region 文件打印的权限判断，这里为天业集团做的特殊判断，现实的应用中，都可以打印.
            bool CanPackUp = false;
            if (SystemConfig.CustomerNo == "TianYe")
            {
                bool isAdmin = false;
                if (BP.Web.WebUser.No == "admin" )
                    isAdmin = true;

                 //  if (BP.Web.WebUser.No == "admin" || BP.Web.WebUser.IsAdmin == true)

                // 判断是否可以打印.
                //string sql = "SELECT NDFrom,NDFromT,EmpFrom FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE WorkID=" + this.WorkID + " AND (EmpFrom='" + BP.Web.WebUser.No + "' OR  EmpTo='" + BP.Web.WebUser.No + "')  ";
                string sql = "SELECT Distinct NDFrom, EmpFrom FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE WorkID=" + this.WorkID;
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    //判断节点是否启用了按钮?
                    int nodeid = int.Parse(dr[0].ToString());
                    BtnLab btn = new BtnLab(nodeid);
                    if (btn.PrintPDFEnable == true || btn.PrintZipEnable == true)
                    {
                        string empFrom = dr[1].ToString();
                        if ( isAdmin == true || BP.Web.WebUser.No == empFrom)
                        {
                            CanPackUp = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                CanPackUp = true;
            }
            #endregion 文件打印的权限判断，这里为天业集团做的特殊判断，现实的应用中，都可以打印.

            ht.Add("CanPackUp", CanPackUp.ToString().ToLower());

            //是否可以打印.
            switch (wfstateEnum)
            {
                case WFState.Runing: /* 运行时*/
                    /*删除流程.*/
                    isCan = BP.WF.Dev2Interface.Flow_IsCanDeleteFlowInstance(this.FK_Flow, this.WorkID, WebUser.No);

                    ht.Add("CanFlowOverByCoercion", isCan.ToString().ToLower());

                    /*取回审批*/
                    isCan = false;
                    string para = "";
                    GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                    string sql = "SELECT NodeID FROM WF_Node WHERE CheckNodes LIKE '%" + gwf.FK_Node + "%'";
                    int myNode = DBAccess.RunSQLReturnValInt(sql, 0);

                    if (myNode != 0)
                    {
                        GetTask gt = new GetTask(myNode);
                        if (gt.Can_I_Do_It())
                        {
                            isCan = true;

                            ht.Add("TackBackFromNode", gwf.FK_Node);
                            ht.Add("TackBackToNode", myNode);
                        }
                    }

                    ht.Add("CanTackBack", isCan.ToString().ToLower() );



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
                    isCan = info.DoQuery() > 0;
                    ht.Add("CanUnSend", isCan.ToString().ToLower());
                    break;
                case WFState.Complete: // 完成.
                case WFState.Delete: // 逻辑删除..
                    /*恢复使用流程*/
                    isCan = WebUser.No == "admin";
                    ht.Add("CanRollBack", isCan.ToString().ToLower());
                    //判断是否可以打印.
                    break;
                case WFState.HungUp: // 挂起.
                    /*撤销挂起*/
                    isCan = BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(FK_Flow, FK_Node, WorkID, WebUser.No);

                    ht.Add("CanUnHungUp", isCan.ToString().ToLower());
                    break;
                default:
                    break;
            }

            return BP.Tools.Json.ToJson(ht);

            //return json + "}";
        }

        /// <summary>
        /// 获取附件列表及单据列表
        /// </summary>
        /// <returns></returns>
        public string GetAthsAndBills()
        {
            string sql = "";
            string json = "{\"Aths\":";

            if (FK_Node == 0)
                sql = "SELECT fadb.*,wn.Name NodeName FROM Sys_FrmAttachmentDB fadb INNER JOIN WF_Node wn ON wn.NodeID = fadb.NodeID WHERE fadb.FK_FrmAttachment IN (SELECT MyPK FROM Sys_FrmAttachment WHERE  " + BP.WF.Glo.MapDataLikeKey(this.FK_Flow, "FK_MapData") + "  AND IsUpload=1) AND fadb.RefPKVal='" + this.WorkID + "' ORDER BY fadb.RDT";
            else
                sql = "SELECT fadb.*,wn.Name NodeName FROM Sys_FrmAttachmentDB fadb INNER JOIN WF_Node wn ON wn.NodeID = fadb.NodeID WHERE fadb.FK_FrmAttachment IN (SELECT MyPK FROM Sys_FrmAttachment WHERE  FK_MapData='ND" + FK_Node + "' ) AND fadb.RefPKVal='" + this.WorkID + "' ORDER BY fadb.RDT";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            foreach (DataColumn col in dt.Columns)
                col.ColumnName = col.ColumnName.ToUpper();

            json += BP.Tools.Json.ToJson(dt) + ",\"Bills\":";

            Bills bills = new Bills();
            bills.Retrieve(BillAttr.WorkID, this.WorkID);

            json += bills.ToJson() + ",\"AppPath\":\"" + BP.WF.Glo.CCFlowAppPath + "\"}";

            return json;
        }
        /// <summary>
        /// 获取OneWork页面的tabs集合
        /// </summary>
        /// <returns></returns>
        public string OneWork_GetTabs()
        {
            string re = "[";

            OneWorkXmls xmls = new OneWorkXmls();
            xmls.RetrieveAll();

             
            int  nodeID = this.FK_Node;
            if (nodeID == 0)
            {
                 GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                 nodeID = this.FK_Node;
            }

            foreach (OneWorkXml item in xmls)
            {
                string url = "";
                url = string.Format("{0}?FK_Node={1}&WorkID={2}&FK_Flow={3}&FID={4}", item.URL, nodeID.ToString(), this.WorkID, this.FK_Flow, this.FID);
                re += "{" + string.Format("\"No\":\"{0}\",\"Name\":\"{1}\", \"Url\":\"{2}\"", item.No, item.Name, url) + "},";
            }

            return re.TrimEnd(',') + "]";
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
            string fk_flow = this.FK_Flow;
            Int64 workid = this.WorkID;
            Int64 fid = this.FID;


            DataSet ds = new DataSet();
            DataTable dt = null;
            string json = string.Empty;
            try
            {
                //获取流程信息
                var sql = "SELECT No \"No\", Name \"Name\", Paras \"Paras\", ChartType \"ChartType\" FROM WF_Flow WHERE No='" + fk_flow + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_FLOW";
                ds.Tables.Add(dt);

                //获取流程中的节点信息
                sql = "SELECT NodeID \"ID\", Name \"Name\", ICON \"Icon\", X \"X\", Y \"Y\", NodePosType \"NodePosType\",RunModel \"RunModel\",HisToNDs \"HisToNDs\",TodolistModel \"TodolistModel\" FROM WF_Node WHERE FK_Flow='" +
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
                sql = "SELECT Node \"Node\", ToNode \"ToNode\",DirType \"DirType\", IsCanBack \"IsCanBack\",Dots \"Dots\" FROM WF_Direction WHERE FK_Flow='" + fk_flow + "'";
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
                          + "             ON  wf.No = wgwf.FK_Flow"
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
                    var trackTable = "ND" + int.Parse(fk_flow) + "Track";
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
                    sql = "SELECT wgw.FK_Emp as \"FK_Emp\",wgw.FK_Node as \"FK_Node\",wgw.FK_EmpText as \"FK_EmpText\",wgw.RDT as \"RDT\",wgw.IsRead as \"IsRead\",wgw.IsPass as \"IsPass\" FROM WF_GenerWorkerlist wgw WHERE wgw.WorkID = " +
                          workid + (fid == 0 ? (" OR FID=" + workid) : (" OR WorkID=" + fid + " OR FID=" + fid));
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "DISPOSE";
                    ds.Tables.Add(dt);
                }
                else
                {
                    var trackTable = "ND" + int.Parse(fk_flow) + "Track";
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

                return BP.Tools.Json.DataSetToJson(ds);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }

            return json;
        }
        /// <summary>
        /// 获得发起的BBS评论.
        /// </summary>
        /// <returns></returns>
        public string FlowBBSList()
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("ActionType", (int)BP.WF.ActionType.FlowBBS);
            ps.Add("WorkID", this.WorkID);

            //转化成json
            return BP.Tools.Json.ToJson(BP.DA.DBAccess.RunSQLReturnTable(ps));
        }

        /// 查看某一用户的评论.
        public string FlowBBS_Check()
        {
            Paras pss = new Paras();
            pss.SQL = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID AND  EMPFROMT='" + this.UserName + "'";
            pss.Add("ActionType", (int)BP.WF.ActionType.FlowBBS);
            pss.Add("WorkID", this.WorkID);

            return BP.Tools.Json.ToJson(BP.DA.DBAccess.RunSQLReturnTable(pss));
        }
        /// <summary>
        /// 提交评论.
        /// </summary>
        /// <returns></returns>
        public string FlowBBS_Save()
        {
            string msg = this.GetValFromFrmByKey("TB_Msg");
            string mypk = BP.WF.Dev2Interface.Flow_BBSAdd(this.FK_Flow, this.WorkID, this.FID, msg, WebUser.No, WebUser.Name);
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE MyPK=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "MyPK";
            ps.Add("MyPK", mypk);
            return BP.Tools.Json.ToJson(BP.DA.DBAccess.RunSQLReturnTable(ps));
        }

        /// <summary>
        /// 回复评论.
        /// </summary>
        /// <returns></returns>
        public string FlowBBS_Replay()
        {
            SMS sms = new SMS();
            sms.RetrieveByAttr(SMSAttr.MyPK, MyPK);
            sms.MyPK = DBAccess.GenerGUID();
            sms.RDT = DataType.CurrentDataTime;
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
            ps.SQL = "SELECT COUNT(ActionType) FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("ActionType", (int)BP.WF.ActionType.FlowBBS);
            ps.Add("WorkID", this.WorkID);
            string count = BP.DA.DBAccess.RunSQLReturnValInt(ps).ToString();
            return count;
        }

        public string Runing_OpenFrm()
        {
            BP.WF.HttpHandler.WF wf = new WF(this.context);
            return wf.Runing_OpenFrm();
        }
    }
}
