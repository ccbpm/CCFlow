using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_WorkOpt : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_WorkOpt(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 打包下载
        /// </summary>
        /// <returns></returns>
        public string Packup_Init()
        {
            try
            {
                Node nd = new Node(this.FK_Node);
                Work wk = nd.HisWork;
                return BP.WF.MakeForm2Html.MakeHtmlDocumentOfFreeFrm(wk.NodeFrmID, this.WorkID,this.FK_Flow,null);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 选择表单,发起前置导航.
        /// </summary>
        /// <returns></returns>
        public string StartGuideFrms_Init()
        {
            BP.WF.Template.FrmNodes fns = new BP.WF.Template.FrmNodes();

            QueryObject qo = new QueryObject(fns);
            qo.AddWhere(FrmNodeAttr.FK_Node, int.Parse(this.FK_Node + "01"));
            qo.addAnd();
            qo.AddWhere(FrmNodeAttr.FrmEnableRole, "!=", (int)BP.WF.Template.FrmEnableRole.WhenHaveFrmPara);
            qo.addOrderBy(FrmNodeAttr.Idx);
            qo.DoQuery();

            foreach (BP.WF.Template.FrmNode item in fns)
            {
                item.GuanJianZiDuan = item.HisFrm.Name;
            }
            return fns.ToJson();
        }

        #region 通用人员选择器.
        /// <summary>
        /// 通用人员选择器Init
        /// </summary>
        /// <returns></returns>
        public string AccepterOfGener_Init()
        {
            /* 获得上一次发送的人员列表. */
            int toNodeID = this.GetRequestValInt("ToNode");

            //查询出来,已经选择的人员.
            SelectAccpers sas = new SelectAccpers();
            int i = sas.Retrieve(SelectAccperAttr.FK_Node, toNodeID, SelectAccperAttr.WorkID, this.WorkID);
            if (i == 0)
            {
                //获得最近的一个workid.
                string trackTable = "ND" + int.Parse(this.FK_Flow) + "Track";
                string sql = "";
                if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                    sql = "SELECT TOP 1 Tag,EmpTo FROM " + trackTable + " WHERE NDTo=" + toNodeID + " AND (ActionType=0 OR ActionType=1) AND EmpFrom='" + WebUser.No + "' ORDER BY WorkID desc  ";
                else
                    sql = "SELECT Tag,EmpTo FROM  " + trackTable + " WHERE NDTo=" + toNodeID + " AND (ActionType=0 OR ActionType=1) AND EmpFrom='" + WebUser.No + "' AND ROWNUM=1 ORDER BY  WorkID desc ";

                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count != 0)
                {
                    string emps = dt.Rows[0]["Tag"].ToString();
                    if (emps == "" || emps == null)
                        emps = dt.Rows[0]["EmpTo"].ToString();

                    BP.WF.Dev2Interface.Node_AddNextStepAccepters(this.WorkID, toNodeID, emps, false);
                }

                sas.Retrieve(SelectAccperAttr.FK_Node, toNodeID, SelectAccperAttr.WorkID, this.WorkID);
            }

            return sas.ToJson();
        }
        /// <summary>
        /// 增加接收人.
        /// </summary>
        /// <returns></returns>
        public string AccepterOfGener_AddEmps()
        {
            try
            {
                //到达的节点ID.
                int toNodeID = this.GetRequestValInt("ToNode");
                string emps = this.GetRequestVal("AddEmps");


                //增加到里面去.s
                BP.WF.Dev2Interface.Node_AddNextStepAccepters(this.WorkID, toNodeID, emps, false);

                //查询出来,已经选择的人员.
                SelectAccpers sas = new SelectAccpers();
                sas.Retrieve(SelectAccperAttr.FK_Node, toNodeID, SelectAccperAttr.WorkID, this.WorkID);

                return sas.ToJson();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("INSERT") == true)
                    return "err@人员名称重复,导致部分人员插入失败.";

                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 删除.
        /// </summary>
        /// <returns></returns>
        public string AccepterOfGener_Delete()
        {
            //删除指定的人员.
            BP.DA.DBAccess.RunSQL("DELETE FROM WF_SelectAccper WHERE WorkID="+this.WorkID+" AND FK_Emp='"+this.FK_Emp+"'");

            int toNodeID = this.GetRequestValInt("ToNode");

            //查询出来,已经选择的人员.
            SelectAccpers sas = new SelectAccpers();
            sas.Retrieve(SelectAccperAttr.FK_Node, toNodeID, SelectAccperAttr.WorkID, this.WorkID);
            return sas.ToJson();
        }
        /// <summary>
        /// 执行发送.
        /// </summary>
        /// <returns></returns>
        public string AccepterOfGener_Send()
        {
            try
            {
                int toNodeID = this.GetRequestValInt("ToNode");

                /* 仅仅设置一个,检查压入的人员个数.*/
                string sql = "SELECT count(WorkID) as Num FROM WF_SelectAccper WHERE FK_Node=" + toNodeID + " AND WorkID=" + this.WorkID + " AND AccType=0";
                int num = DBAccess.RunSQLReturnValInt(sql, 0);
                if (num == 0)
                    return "err@请设置选择的人员.";
                Selector sr = new Selector(toNodeID);
                if (sr.IsSimpleSelector == true)
                {
                    if (num != 1)
                        return "err@您只能选择一个接受人,请移除其他的接受人然后执行发送.";
                }

                SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, toNodeID, null);
                string strs= objs.ToMsgOfHtml();
                strs = strs.Replace("@", "<br>@");
                return strs;
            }
            catch(Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion

        // 查询select集合
        public string AccepterOfGener_SelectEmps()
        {
            string sql = "";
            string emp = this.GetRequestVal("TB_Emps");

            #region 保障查询语句的安全.
            emp = emp.ToLower();
            emp = emp.Replace("'", "");
            emp = emp.Replace("&", "&amp");
            emp = emp.Replace("<", "&lt");
            emp = emp.Replace(">", "&gt");
            emp = emp.Replace("delete", "");
            emp = emp.Replace("update", "");
            emp = emp.Replace("insert", "");
            #endregion 保障查询语句的安全.


            bool isPinYin = DBAccess.IsExitsTableCol("Port_Emp", "PinYin");
            if (isPinYin == true)
            {
                //标识结束，不要like名字了.
                if (emp.Contains("/"))
                {
                    if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                        sql = "SELECT TOP 12 a.No,a.Name +'/'+b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR a.PinYin LIKE '%," + emp.ToLower() + "%')";
                    if (SystemConfig.AppCenterDBType == DBType.Oracle)
                        sql = "SELECT a.No,a.Name || '/' || b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR a.PinYin LIKE '%," + emp.ToLower() + "%') AND rownum<=12";
                    if (SystemConfig.AppCenterDBType == DBType.MySQL)
                        sql = "SELECT a.No,CONCAT(a.Name,'/',b.name) as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%'  OR a.PinYin LIKE '%," + emp.ToLower() + "%') LIMIT 12";
                }
                else
                {
                    if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                        sql = "SELECT TOP 12 a.No,a.Name +'/'+b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and ( a.PinYin LIKE '%," + emp.ToLower() + "%')";
                    if (SystemConfig.AppCenterDBType == DBType.Oracle)
                        sql = "SELECT a.No,a.Name || '/' || b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (  a.PinYin LIKE '%," + emp.ToLower() + "%') AND rownum<=12";
                    if (SystemConfig.AppCenterDBType == DBType.MySQL)
                        sql = "SELECT a.No,CONCAT(a.Name,'/',b.name) as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (  a.PinYin LIKE '%," + emp.ToLower() + "%' ) LIMIT 12";
                }
            }
            else
            {
                if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                    sql = "SELECT TOP 12 a.No,a.Name +'/'+b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%')";
                if (SystemConfig.AppCenterDBType == DBType.Oracle)
                    sql = "SELECT a.No,a.Name || '/' || b.name as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%') and rownum<=12";
                if (SystemConfig.AppCenterDBType == DBType.MySQL)
                    sql = "SELECT a.No,CONCAT(a.Name,'/',b.name) as Name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No like '%" + emp + "%' OR a.NAME  LIKE '%" + emp + "%') LIMIT 12";
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //  BP.DA.Log.DebugWriteError(sql);

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns[0].ColumnName = "No";
                dt.Columns[1].ColumnName = "Name";
            }

            return BP.Tools.Json.ToJson(dt);
        }

        #region 会签.
        /// <summary>
        /// 会签
        /// </summary>
        /// <returns></returns>
        public string HuiQian_Init()
        {
            //要找到主持人.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

            //查询出来集合.
            GenerWorkerLists ens = new GenerWorkerLists(this.WorkID, this.FK_Node);
            foreach (GenerWorkerList item in ens)
            {
                if (gwf.TodoEmps.Contains(item.FK_Emp + ",") == true)
                {
                    item.FK_EmpText = "<img src='../Img/zhuichiren.png' border=0 />" + item.FK_EmpText;
                    item.FK_EmpText = item.FK_EmpText;
                    item.IsPassInt = 100;
                    continue;
                }

                //标记为自己.
                if (item.FK_Emp == BP.Web.WebUser.No)
                {
                    item.FK_EmpText = "" + item.FK_EmpText;
                    item.IsPassInt = 99;
                }
            }

            //赋值部门名称。
            DataTable mydt = ens.ToDataTableField();
            mydt.Columns.Add("FK_DeptT", typeof(string));
            foreach (DataRow dr in mydt.Rows)
            {
                string fk_emp = dr["FK_Emp"].ToString();
                foreach (GenerWorkerList item in ens)
                {
                    if (item.FK_Emp == fk_emp)
                        dr["FK_DeptT"] = item.FK_DeptT;
                }
            }
            return BP.Tools.Json.ToJson(mydt);
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <returns></returns>
        public string HuiQian_Delete()
        {
            string emp = this.GetRequestVal("FK_Emp");
            if (this.FK_Emp == WebUser.No)
                return "err@您不能移除您自己";

            //要找到主持人.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (gwf.TodoEmps.Contains(BP.Web.WebUser.No + ",") == false)
                return "err@您不是主持人，您不能删除。";

            //删除该数据.
            GenerWorkerList gwlOfMe = new GenerWorkerList();
            gwlOfMe.Delete(GenerWorkerListAttr.FK_Emp, this.FK_Emp,
                GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, this.FK_Node);

            return HuiQian_Init();
        }
        /// <summary>
        /// 增加审核人员
        /// </summary>
        /// <returns></returns>
        public string HuiQian_AddEmps()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (gwf.TodoEmps.Contains(WebUser.No + ",") == false)
                return "err@您不是会签主持人，您不能执行该操作。";

            GenerWorkerList gwlOfMe = new GenerWorkerList();
            int num = gwlOfMe.Retrieve(GenerWorkerListAttr.FK_Emp, WebUser.No,
                 GenerWorkerListAttr.WorkID, this.WorkID,
                 GenerWorkerListAttr.FK_Node, this.FK_Node);

            if (num == 0)
                return "err@没有查询到当前人员的工作列表数据.";

            //是否有拼音字段？
            bool isPinYin = DBAccess.IsExitsTableCol("Port_Emp", "PinYin");
            string sql = "";

            #region 求人员集合.
            Emps emps = new Emps();
            string toEmpStrs = this.GetRequestVal("AddEmps");
            toEmpStrs = toEmpStrs.Replace(",", ";");
            string[] toEmps = toEmpStrs.Split(';');
            string infos = "";
            foreach (string empStr in toEmps)
            {
                if (string.IsNullOrEmpty(empStr) == true)
                    continue;

                if (isPinYin == true)
                {
                    if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                      sql = "SELECT TOP 12 a.No,a.Name+'/'+b.name FROM Port_Emp a,Port_Dept b  WHERE  (a.fk_dept=b.no) and (a.No = '" + empStr + "' OR a.NAME = '" + empStr + "'  OR a.PinYin LIKE '%," + empStr + "%,')";

                    if (SystemConfig.AppCenterDBType == DBType.Oracle)
                        sql = "SELECT No,Name FROM Port_Emp WHERE No='" + empStr + "' OR NAME ='" + empStr + "'  OR PinYin LIKE '%," + empStr + ",%' and ROWNUM <=12 ";

                }
                else
                {
                    if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                    sql = "SELECT TOP 12 No,Name FROM Port_Emp WHERE No='" + empStr + "' OR NAME ='" + empStr + "'";

                    if (SystemConfig.AppCenterDBType == DBType.Oracle)
                        sql = "SELECT No,Name FROM Port_Emp WHERE No='" + empStr + "' OR NAME ='" + empStr + "' AND ROWNUM  <= 12 ";


                }

                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count > 12 || dt.Rows.Count == 0)
                    continue;

                foreach (DataRow dr in dt.Rows)
                {
                    string empNo = dr[0].ToString();
                    string empName = dr[1].ToString();

                    //查查是否存在队列里？
                    num = gwlOfMe.Retrieve(GenerWorkerListAttr.FK_Emp, empNo,
                    GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, this.FK_Node);

                    if (num == 1)
                    {
                        infos += "\t\n@人员[" + empStr + "]已经在队列里.";
                        continue;
                    }

                    //增加到队列里面.
                    emps.AddEntity(new Emp(empNo));
                }
            }

            if (infos != "" && emps.Count != 0)
                return "info@" + infos;

            if (emps.Count == 0)
                return "info@你输入的人员编号错误, 执行信息:" + infos;
            #endregion 求人员集合.

            //把集合都放入到这里.
            GenerWorkerLists gwls = new GenerWorkerLists();

            //查询出来其他列的数据.
            gwlOfMe.Retrieve(GenerWorkerListAttr.FK_Emp, WebUser.No,
                GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, this.FK_Node);

            //遍历人员集合.
            foreach (Emp item in emps)
            {
                gwlOfMe.FK_Emp = item.No;
                gwlOfMe.FK_EmpText = item.Name;
                gwlOfMe.IsPassInt = 0;
                gwlOfMe.FK_Dept = item.FK_Dept;
                gwlOfMe.FK_DeptT = item.FK_DeptText; //部门名称.

                gwlOfMe.Insert(); //插入作为待办.
                infos += "\t\n@" + item.No + "  " + item.Name;

                gwlOfMe.Retrieve();

                GenerWorkerList gwl = new GenerWorkerList();
                gwl.Copy(gwlOfMe);
                gwls.AddEntity(gwl);
            }

            //赋值部门名称。
            DataTable mydt = gwls.ToDataTableField();
            mydt.Columns.Add("FK_DeptT");
            foreach (DataRow dr in mydt.Rows)
            {
                string fk_emp = dr["FK_Emp"].ToString();
                foreach (GenerWorkerList item in gwls)
                {
                    if (item.FK_Emp == fk_emp)
                        dr["FK_DeptT"] = item.FK_DeptT;
                }
            }

            return BP.Tools.Json.ToJson(mydt);
        }
        #endregion

        // 查询select集合
        public string HuiQian_SelectEmps() {
            return AccepterOfGener_SelectEmps();
        }

        #region 审核组件.
        /// <summary>
        /// 初始化审核组件数据.
        /// </summary>
        /// <returns></returns>
        public string WorkCheck_Init()
        {
            FrmWorkCheck wcDesc = new FrmWorkCheck(this.FK_Node);
            FrmWorkCheck frmWorkCheck = null;
            FrmAttachmentDBs athDBs = null;
            Nodes nds = new Nodes(this.FK_Flow);
            FrmWorkChecks fwcs = new FrmWorkChecks();
            Node nd = null;
            WorkCheck wc = null;
            Tracks tks = null;
            Track tkDoc = null;
            string nodes = "";
            bool isCanDo = false;
            bool isExitTb_doc = true;
            DataSet ds = new DataSet();
            DataRow row = null;
            string dotype = this.GetRequestVal("ShowType");
            Dictionary<int, DataTable> nodeEmps = new Dictionary<int, DataTable>(); //节点id，接收人列表
            FrmWorkCheck fwc = null;
            DataTable dt = null;
            int idx = 0;
            int noneEmpIdx = 0;

            fwcs.Retrieve(NodeAttr.FK_Flow, this.FK_Flow, NodeAttr.Step);
            ds.Tables.Add(wcDesc.ToDataTableField("wcDesc"));

            DataTable tkDt = new DataTable("Tracks");
            tkDt.Columns.Add("NodeID", typeof(int));
            tkDt.Columns.Add("NodeName", typeof(string));
            tkDt.Columns.Add("Msg", typeof(string));
            tkDt.Columns.Add("EmpFrom", typeof(string));
            tkDt.Columns.Add("EmpFromT", typeof(string));
            tkDt.Columns.Add("RDT", typeof(string));
            tkDt.Columns.Add("IsDoc", typeof(bool));
            tkDt.Columns.Add("ParentNode", typeof(int));
            tkDt.Columns.Add("T_NodeIndex", typeof(int));    //节点排列顺序，用于后面的排序
            tkDt.Columns.Add("T_CheckIndex", typeof(int));    //审核人显示顺序，用于后面的排序
            ds.Tables.Add(tkDt);

            DataTable athDt = new DataTable("Aths");
            athDt.Columns.Add("NodeID", typeof(int));
            athDt.Columns.Add("MyPK", typeof(string));
            athDt.Columns.Add("Href", typeof(string));
            athDt.Columns.Add("FileName", typeof(string));
            athDt.Columns.Add("FileExts", typeof(string));
            athDt.Columns.Add("CanDelete", typeof(bool));
            ds.Tables.Add(athDt);

            if (this.FID != 0)
                wc = new WorkCheck(this.FK_Flow, this.FK_Node, this.FID, 0);
            else
                wc = new WorkCheck(this.FK_Flow, this.FK_Node, this.WorkID, this.FID);

            isCanDo = BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Flow, this.FK_Node, this.WorkID, BP.Web.WebUser.No);
            //历史审核信息显示
            if (wcDesc.FWCListEnable)
            {
                tks = wc.HisWorkChecks;

                //已走过节点
                int empIdx = 0;
                int lastNodeId = 0;
                foreach (BP.WF.Track tk in tks)
                {
                    if (tk.HisActionType == ActionType.FlowBBS)
                        continue;

                    if (lastNodeId == 0)
                        lastNodeId = tk.NDFrom;

                    if (lastNodeId != tk.NDFrom)
                    {
                        idx++;
                        lastNodeId = tk.NDFrom;
                    }

                    tk.Row.Add("T_NodeIndex", idx);
                    
                    nd = nds.GetEntityByKey(tk.NDFrom) as Node;

                    fwc = fwcs.GetEntityByKey(tk.NDFrom) as FrmWorkCheck;
                    //求出主键
                    long pkVal = this.WorkID;
                    if (nd.HisRunModel == RunModel.SubThread)
                        pkVal = this.FID;

                    //排序，结合人员表Idx进行排序
                    if (fwc.FWCOrderModel == FWCOrderModel.SqlAccepter)
                    {
                        tk.Row["T_CheckIndex"] =
                            DBAccess.RunSQLReturnValInt(
                                string.Format("SELECT Idx FROM Port_Emp WHERE No='{0}'", tk.EmpFrom), 0);
                        noneEmpIdx++;
                    }
                    else
                    {
                        tk.Row["T_CheckIndex"] = noneEmpIdx++;
                    }

                    switch (tk.HisActionType)
                    {
                        case ActionType.WorkCheck:
                        case ActionType.StartChildenFlow:
                            if (nodes.Contains(tk.NDFrom + ",") == false)
                                nodes += tk.NDFrom + ",";
                            break;
                        default:
                            continue;
                    }
                }

                foreach (Track tk in tks)
                {
                    if (nodes.Contains(tk.NDFrom + ","))
                    {
                        if (tk.HisActionType != ActionType.WorkCheck && tk.HisActionType != ActionType.StartChildenFlow)
                            continue;

                        row = tkDt.NewRow();
                        row["NodeID"] = tk.NDFrom;
                        row["NodeName"] = (nds.GetEntityByKey(tk.NDFrom) as Node).FWCNodeName;
                        row["IsDoc"] = false;
                        row["ParentNode"] = 0;
                        row["RDT"] = string.IsNullOrWhiteSpace(tk.RDT) ? "" : tk.NDFrom == tk.NDTo && string.IsNullOrWhiteSpace(tk.Msg) ? "" : tk.RDT;
                        row["T_NodeIndex"] = tk.Row["T_NodeIndex"];
                        row["T_CheckIndex"] = tk.Row["T_CheckIndex"];

                        if (dotype != "View" && tk.EmpFrom == WebUser.No && this.FK_Node == tk.NDFrom && isExitTb_doc && (
                                            wcDesc.HisFrmWorkCheckType == FWCType.Check || (
                                            (wcDesc.HisFrmWorkCheckType == FWCType.DailyLog || wcDesc.HisFrmWorkCheckType == FWCType.WeekLog) && DateTime.Parse(tk.RDT).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd")) || (wcDesc.HisFrmWorkCheckType == FWCType.MonthLog && DateTime.Parse(tk.RDT).ToString("yyyy-MM") == DateTime.Now.ToString("yyyy-MM"))
                                            ))
                        {
                            bool isLast = true;
                            foreach (Track tk1 in tks)
                            {
                                if (tk1.HisActionType == tk.HisActionType && tk1.NDFrom == tk.NDFrom && tk1.RDT.CompareTo(tk.RDT) > 0)
                                {
                                    isLast = false;
                                    break;
                                }
                            }

                            if (isLast)
                            {
                                isExitTb_doc = false;
                                row["IsDoc"] = true;
                                row["Msg"] = Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.FK_Node) ?? "";
                                tkDoc = tk;

                                //增加默认审核意见
                                if (string.IsNullOrWhiteSpace(row["Msg"].ToString()) && wcDesc.FWCIsFullInfo)
                                    row["Msg"] = wcDesc.FWCDefInfo;
                            }
                            else
                            {
                                row["Msg"] = tk.MsgHtml;
                            }
                        }
                        else
                        {
                            row["Msg"] = tk.MsgHtml;
                        }

                        row["EmpFrom"] = tk.EmpFrom;
                        row["EmpFromT"] = tk.EmpFromT;

                        tkDt.Rows.Add(row);

                        #region //审核组件附件数据
                        athDBs = new FrmAttachmentDBs();
                        QueryObject obj_Ath = new QueryObject(athDBs);
                        obj_Ath.AddWhere(FrmAttachmentDBAttr.FK_FrmAttachment, tk.NDFrom + "_FrmWorkCheck");
                        obj_Ath.addAnd();
                        obj_Ath.AddWhere(FrmAttachmentDBAttr.RefPKVal, this.WorkID);
                        obj_Ath.addOrderBy(FrmAttachmentDBAttr.RDT);
                        obj_Ath.DoQuery();

                        foreach (FrmAttachmentDB athDB in athDBs)
                        {
                            row = athDt.NewRow();

                            row["NodeID"] = tk.NDFrom;
                            row["MyPK"] = athDB.MyPK;
                            row["Href"] = GetFileAction(athDB);
                            row["FileName"] = athDB.FileName;
                            row["FileExts"] = athDB.FileExts;
                            row["CanDelete"] = athDB.FK_MapData == this.FK_Node.ToString() && athDB.Rec == WebUser.No && dotype != "View";

                            athDt.Rows.Add(row);
                        }

                        #endregion

                        #region //子流程的审核组件数据
                        if (tk.FID != 0 && tk.HisActionType == ActionType.StartChildenFlow && tkDt.Select("ParentNode=" + tk.NDFrom).Length == 0)
                        {
                            string[] paras = tk.Tag.Split('@');
                            string[] p1 = paras[1].Split('=');
                            string fk_flow = p1[1]; //子流程编号

                            string[] p2 = paras[2].Split('=');
                            string workId = p2[1]; //子流程ID.
                            int biaoji = 0;

                            WorkCheck subwc = new WorkCheck(fk_flow, int.Parse(fk_flow + "01"), Int64.Parse(workId), 0);

                            Tracks subtks = subwc.HisWorkChecks;
                            //取出来子流程的所有的节点。
                            Nodes subNds = new Nodes(fk_flow);
                            foreach (Node item in subNds)     //主要按顺序显示
                            {
                                foreach (Track mysubtk in subtks)
                                {
                                    if (item.NodeID != mysubtk.NDFrom)
                                        continue;

                                    /*输出该子流程的审核信息，应该考虑子流程的子流程信息, 就不考虑那样复杂了.*/
                                    if (mysubtk.HisActionType == ActionType.WorkCheck)
                                    {
                                        // 发起多个子流程时，发起人只显示一次
                                        if (mysubtk.NDFrom == int.Parse(fk_flow + "01") && biaoji == 1)
                                            continue;

                                        row = tkDt.NewRow();
                                        row["NodeID"] = mysubtk.NDFrom;
                                        row["NodeName"] = string.Format("(子流程){0}", mysubtk.NDFromT);
                                        row["Msg"] = mysubtk.MsgHtml;
                                        row["EmpFrom"] = mysubtk.EmpFrom;
                                        row["EmpFromT"] = mysubtk.EmpFromT;
                                        row["RDT"] = mysubtk.RDT ?? "";
                                        row["IsDoc"] = false;
                                        row["ParentNode"] = tk.NDFrom;
                                        row["T_NodeIndex"] = idx++;
                                        row["T_CheckIndex"] = noneEmpIdx++;
                                        tkDt.Rows.Add(row);

                                        if (mysubtk.NDFrom == int.Parse(fk_flow + "01"))
                                        {
                                            biaoji = 1;
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        //todo:抄送暂未处理，不明逻辑
                        continue;
                    }

                    //判断是否显示所有步骤
                    if (wcDesc.FWCIsShowAllStep == false)
                        continue;

                    //todo:抄送暂未处理，不明逻辑
                }

                if (tkDoc != null)
                {
                    //判断可编辑审核信息是否处于最后一条，不处于最后一条，则将其移到最后一条
                    DataRow rdoc = tkDt.Select("IsDoc=True")[0];
                    if (tkDt.Rows.IndexOf(rdoc) != tkDt.Rows.Count - 1)
                    {
                        tkDt.Rows.Add(rdoc.ItemArray)["RDT"] = "";

                        rdoc["IsDoc"] = false;
                        rdoc["RDT"] = tkDoc.RDT;
                        rdoc["Msg"] = tkDoc.MsgHtml;
                    }
                    else
                    {
                        //判断刚退回时，退回接收人一打开，审核信息复制一条
                        Track lastTrack = tks[tks.Count - 1] as Track;
                        if ((lastTrack.HisActionType == ActionType.Return || lastTrack.HisActionType == ActionType.Forward) && lastTrack.NDTo == tkDoc.NDFrom)
                        {
                            tkDt.Rows.Add(rdoc.ItemArray)["RDT"] = "";

                            rdoc["IsDoc"] = false;
                            rdoc["RDT"] = tkDoc.RDT;
                            rdoc["Msg"] = tkDoc.MsgHtml;
                        }
                    }
                }
            }

            //审核意见填写
            if (isExitTb_doc && wcDesc.HisFrmWorkCheckSta == FrmWorkCheckSta.Enable && isCanDo && dotype != "View")
            {
                DataRow[] rows = null;
                nd = nds.GetEntityByKey(this.FK_Node) as Node;
                if (wcDesc.FWCOrderModel == FWCOrderModel.SqlAccepter)
                {
                    rows = tkDt.Select("NodeID=" + this.FK_Node + " AND Msg='' AND EmpFrom='" + WebUser.No + "'");

                    if (rows.Length == 0)
                        rows = tkDt.Select("NodeID=" + this.FK_Node + " AND EmpFrom='" + WebUser.No + "'", "RDT DESC");

                    if (rows.Length > 0)
                    {
                        row = rows[0];
                        row["IsDoc"] = true;
                        row["Msg"] = Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.FK_Node) ?? "";

                        if (row["Msg"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Msg"] as string))
                            row["RDT"] = "";

                        //增加默认审核意见
                        if (string.IsNullOrWhiteSpace(row["Msg"].ToString()) && wcDesc.FWCIsFullInfo)
                            row["Msg"] = wcDesc.FWCDefInfo;
                    }
                    else
                    {
                        row = tkDt.NewRow();
                        row["NodeID"] = this.FK_Node;
                        row["NodeName"] = nd.FWCNodeName;
                        row["IsDoc"] = true;
                        row["ParentNode"] = 0;
                        row["RDT"] = "";
                        row["Msg"] = Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.FK_Node) ?? "";
                        row["EmpFrom"] = WebUser.No;
                        row["EmpFromT"] = WebUser.Name;
                        row["T_NodeIndex"] = ++idx;
                        row["T_CheckIndex"] = ++noneEmpIdx;

                        //增加默认审核意见
                        if (string.IsNullOrWhiteSpace(row["Msg"].ToString()) && wcDesc.FWCIsFullInfo)
                            row["Msg"] = wcDesc.FWCDefInfo;

                        tkDt.Rows.Add(row);
                    }
                }
                else
                {
                    row = tkDt.NewRow();
                    row["NodeID"] = this.FK_Node;
                    row["NodeName"] = nd.FWCNodeName;
                    row["IsDoc"] = true;
                    row["ParentNode"] = 0;
                    row["RDT"] = "";
                    row["Msg"] = Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.FK_Node) ?? "";
                    row["EmpFrom"] = WebUser.No;
                    row["EmpFromT"] = WebUser.Name;
                    row["T_NodeIndex"] = ++idx;
                    row["T_CheckIndex"] = ++noneEmpIdx;

                    //增加默认审核意见
                    if (string.IsNullOrWhiteSpace(row["Msg"].ToString()) && wcDesc.FWCIsFullInfo)
                        row["Msg"] = wcDesc.FWCDefInfo;

                    tkDt.Rows.Add(row);
                }
            }

            #region 显示有审核组件，但还未审核的节点
            if (tks == null)
                tks = wc.HisWorkChecks;

            foreach (FrmWorkCheck item in fwcs)
            {
                if (item.FWCIsShowTruck == false)
                    continue;

                //是否已审核
                bool isHave = false;
                foreach (BP.WF.Track tk in tks)
                {
                    if (tk.NDFrom == item.NodeID || tk.NDTo == item.NodeID)
                    {
                        isHave = true;
                        continue;
                    }
                }

                if (isHave == true)
                    continue;

                row = tkDt.NewRow();
                row["NodeID"] = item.NodeID;
                row["NodeName"] = (nds.GetEntityByKey(item.NodeID) as Node).FWCNodeName;
                row["IsDoc"] = false;
                row["ParentNode"] = 0;
                row["RDT"] = "";
                row["Msg"] = "&nbsp;";
                row["EmpFrom"] = "";
                row["EmpFromT"] = "";
                row["T_NodeIndex"] = ++idx;
                row["T_CheckIndex"] = ++noneEmpIdx;

                tkDt.Rows.Add(row);
            }
            #endregion 增加空白.

            DataView dv = tkDt.DefaultView;
            dv.Sort = "T_NodeIndex ASC,T_CheckIndex ASC";
            DataTable sortedTKs = dv.ToTable("Tracks");

            ds.Tables.Remove("Tracks");
            ds.Tables.Add(sortedTKs);

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 获取审核组件中刚上传的附件列表信息
        /// </summary>
        /// <returns></returns>
        public string WorkCheck_GetNewUploadedAths()
        {
            DataRow row = null;
            string athNames = GetRequestVal("Names");
            string attachPK = GetRequestVal("AttachPK");

            DataTable athDt = new DataTable("Aths");
            athDt.Columns.Add("NodeID", typeof(int));
            athDt.Columns.Add("MyPK", typeof(string));
            athDt.Columns.Add("Href", typeof(string));
            athDt.Columns.Add("FileName", typeof(string));
            athDt.Columns.Add("FileExts", typeof(string));
            athDt.Columns.Add("CanDelete", typeof(string));

            FrmAttachmentDBs athDBs = new FrmAttachmentDBs();
            QueryObject obj_Ath = new QueryObject(athDBs);
            obj_Ath.AddWhere(FrmAttachmentDBAttr.FK_FrmAttachment, this.FK_Node + "_FrmWorkCheck");
            obj_Ath.addAnd();
            obj_Ath.AddWhere(FrmAttachmentDBAttr.RefPKVal, this.WorkID);
            obj_Ath.addOrderBy(FrmAttachmentDBAttr.RDT);
            obj_Ath.DoQuery();

            foreach (FrmAttachmentDB athDB in athDBs)
            {
                if (athNames.ToLower().IndexOf("|" + athDB.FileName.ToLower() + "|") == -1)
                    continue;

                row = athDt.NewRow();

                row["NodeID"] = this.FK_Node;
                row["MyPK"] = athDB.MyPK;
                row["Href"] = GetFileAction(athDB);
                row["FileName"] = athDB.FileName;
                row["FileExts"] = athDB.FileExts;
                row["CanDelete"] = athDB.Rec == WebUser.No;

                athDt.Rows.Add(row);
            }

            return BP.Tools.Json.ToJson(athDt);
        }
        /// <summary>
        /// 获取附件链接
        /// </summary>
        /// <param name="athDB"></param>
        /// <returns></returns>
        private string GetFileAction(FrmAttachmentDB athDB)
        {
            if (athDB == null || athDB.FileExts == "") return "#";

            FrmAttachment athDesc = new FrmAttachment(athDB.FK_FrmAttachment);
            switch (athDB.FileExts)
            {
                case "doc":
                case "docx":
                case "xls":
                case "xlsx":
                    return "javascript:AthOpenOfiice('" + athDB.FK_FrmAttachment + "','" + this.WorkID + "','" + athDB.MyPK + "','" + athDB.FK_MapData + "','" + athDB.FK_FrmAttachment + "','" + this.FK_Node + "')";
                case "txt":
                case "jpg":
                case "jpeg":
                case "gif":
                case "png":
                case "bmp":
                case "ceb":
                    return "javascript:AthOpenView('" + athDB.RefPKVal + "','" + athDB.MyPK + "','" + athDB.FK_FrmAttachment + "','" + athDB.FileExts + "','" + this.FK_Flow + "','" + athDB.FK_MapData + "','" + this.WorkID + "','false')";
                case "pdf":
                    return athDesc.SaveTo + this.WorkID + "/" + athDB.MyPK + "." + athDB.FileName;
            }

            return "javascript:AthDown('" + athDB.FK_FrmAttachment + "','" + this.WorkID + "','" + athDB.MyPK + "','" + athDB.FK_MapData + "','" + this.FK_Flow + "','" + athDB.FK_FrmAttachment + "')";
        }

        /// <summary>
        /// 审核信息保存.
        /// </summary>
        /// <returns></returns>
        public string WorkCheck_Save()
        {
            // 审核信息.
            string msg = "";
            string dotype = GetRequestVal("ShowType");
            string doc = GetRequestVal("Doc");
            bool isCC = GetRequestVal("IsCC") == "1";
            //查看时取消保存
            if (dotype != null && dotype == "View")
                return "";

            //内容为空，取消保存，20170727取消此处限制
            //if (string.IsNullOrEmpty(doc.Trim()))
            //    return "";

            string val = string.Empty;
            FrmWorkCheck wcDesc = new FrmWorkCheck(this.FK_Node);
            if (string.IsNullOrEmpty(wcDesc.FWCFields) == false)
            {
                //循环属性获取值
                Attrs fwcAttrs = new Attrs(wcDesc.FWCFields);
                foreach (Attr attr in fwcAttrs)
                {
                    if (attr.UIContralType == UIContralType.TB)
                    {
                        val = GetRequestVal("TB_" + attr.Key);

                        msg += attr.Key + "=" + val + ";";
                    }
                    else if (attr.UIContralType == UIContralType.CheckBok)
                    {
                        val = GetRequestVal("CB_" + attr.Key);

                        msg += attr.Key + "=" + Convert.ToInt32(val) + ";";
                    }
                    else if (attr.UIContralType == UIContralType.DDL)
                    {
                        val = GetRequestVal("DDL_" + attr.Key);

                        msg += attr.Key + "=" + val + ";";
                    }
                }
            }
            else
            {
                // 加入审核信息.
                msg = doc;
            }

            // 处理人大的需求，需要把审核意见写入到FlowNote里面去.
            string sql = "UPDATE WF_GenerWorkFlow SET FlowNote='" + msg + "' WHERE WorkID=" + this.WorkID;
            DBAccess.RunSQL(sql);

            // 判断是否是抄送?
            if (isCC)
            {
                // 写入审核信息，有可能是update数据。
                Dev2Interface.WriteTrackWorkCheck(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, msg, wcDesc.FWCOpLabel);

                //设置抄送状态 - 已经审核完毕.
                Dev2Interface.Node_CC_SetSta(this.FK_Node, this.WorkID, WebUser.No, CCSta.CheckOver);
            }
            else
            {
                #region 根据类型写入数据  qin
                if (wcDesc.HisFrmWorkCheckType == FWCType.Check)  //审核组件
                {
                    //判断是否审核组件中“协作模式下操作员显示顺序”设置为“按照接受人员列表先后顺序(官职大小)”，删除原有的空审核信息
                    if (wcDesc.FWCOrderModel == FWCOrderModel.SqlAccepter)
                    {
                        sql = "DELETE FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE WorkID = " + this.WorkID +
                              " AND ActionType = " + (int)ActionType.WorkCheck + " AND NDFrom = " + this.FK_Node +
                              " AND NDTo = " + this.FK_Node + " AND EmpFrom = '" + WebUser.No + "' AND (Msg='' OR Msg IS NULL)";
                        DBAccess.RunSQL(sql);
                    }

                    Dev2Interface.WriteTrackWorkCheck(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, msg,
                                                      wcDesc.FWCOpLabel);
                }
                if (wcDesc.HisFrmWorkCheckType == FWCType.DailyLog)//日志组件
                {
                    Dev2Interface.WriteTrackDailyLog(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, msg, wcDesc.FWCOpLabel);
                }
                if (wcDesc.HisFrmWorkCheckType == FWCType.WeekLog)//周报
                {
                    Dev2Interface.WriteTrackWeekLog(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, msg, wcDesc.FWCOpLabel);
                }
                if (wcDesc.HisFrmWorkCheckType == FWCType.MonthLog)//月报
                {
                    Dev2Interface.WriteTrackMonthLog(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, msg, wcDesc.FWCOpLabel);
                }
                #endregion
            }

            sql = "SELECT MyPK,RDT FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE NDFrom = " + this.FK_Node + " AND ActionType = " + (int)ActionType.WorkCheck + " AND EmpFrom = '" + WebUser.No + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql, 1, 1, "MyPK", "RDT", "DESC");

            return dt.Rows.Count > 0 ? dt.Rows[0]["RDT"].ToString() : "";
        }
        #endregion


        #region 工作分配.
        /// <summary>
        /// 分配工作
        /// </summary>
        /// <returns></returns>
        public string AllotTask_Init()
        {
            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.FK_Node, true);
            return wls.ToJson();
        }
        /// <summary>
        /// 分配工作
        /// </summary>
        /// <returns></returns>
        public string AllotTask_Save()
        {
            return "";
        }
        #endregion


        #region 执行跳转.
        /// <summary>
        /// 返回可以跳转的节点.
        /// </summary>
        /// <returns></returns>
        public string FlowSkip_Init()
        {
            Node nd = new Node(this.FK_Node);
            BP.WF.Template.BtnLab lab = new BtnLab(this.FK_Node);

            string sql = "SELECT NodeID,Name FROM WF_Node WHERE FK_Flow='" + this.FK_Flow + "'";
            switch (lab.JumpWayEnum)
            {
                case JumpWay.Previous:
                    sql = "SELECT NodeID,Name FROM WF_Node WHERE NodeID IN (SELECT FK_Node FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " )";
                    break;
                case JumpWay.Next:
                    sql = "SELECT NodeID,Name FROM WF_Node WHERE NodeID NOT IN (SELECT FK_Node FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " ) AND FK_Flow='" + this.FK_Flow + "'";
                    break;
                case JumpWay.AnyNode:
                    sql = "SELECT NodeID,Name FROM WF_Node WHERE FK_Flow='" + this.FK_Flow + "' ORDER BY STEP";
                    break;
                case JumpWay.JumpSpecifiedNodes:
                    sql = nd.JumpToNodes;
                    sql = sql.Replace("@WebUser.No", WebUser.No);
                    sql = sql.Replace("@WebUser.Name", WebUser.Name);
                    sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                    if (sql.Contains("@"))
                    {
                        Work wk = nd.HisWork;
                        wk.OID = this.WorkID;
                        wk.RetrieveFromDBSources();
                        foreach (Attr attr in wk.EnMap.Attrs)
                        {
                            if (sql.Contains("@") == false)
                                break;
                            sql = sql.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                        }
                    }
                    break;
                case JumpWay.CanNotJump:
                    return "err@此节点不允许跳转.";
                default:
                    return "err@未判断";
            }

            sql = sql.Replace("~", "'");
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //如果是oracle,就转成小写.
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["NODEID"].ColumnName = "NodeID";
                dt.Columns["NAME"].ColumnName = "Name";
            }
            return BP.Tools.Json.DataTableToJson(dt, false);
        }
        /// <summary>
        /// 执行跳转
        /// </summary>
        /// <returns></returns>
        public string FlowSkip_Do()
        {
            try
            {
                Node ndJump = new Node(this.GetRequestValInt("GoNode"));
                BP.WF.WorkNode wn = new BP.WF.WorkNode(this.WorkID, this.FK_Node);
                string msg = wn.NodeSend(ndJump, null).ToMsgOfHtml();
                return msg;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion 执行跳转.

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {

                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到.");
        }
        #endregion 执行父类的重写方法.

        /// <summary>
        /// 选择权限组
        /// </summary>
        /// <returns></returns>
        public string CC_SelectGroups()
        {
            string sql = "SELECT NO,NAME FROM GPM_Group ORDER BY IDX";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 抄送初始化.
        /// </summary>
        /// <returns></returns>
        public string CC_Init()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            Hashtable ht = new Hashtable();
            ht.Add("Title", gwf.Title);

            //计算出来曾经抄送过的人.
            string sql = "SELECT CCToName FROM WF_CCList WHERE FK_Node=" + this.FK_Node + " AND WorkID=" + this.WorkID;
            DataTable mydt = DBAccess.RunSQLReturnTable(sql);
            string toAllEmps = "";
            foreach (DataRow dr in mydt.Rows)
                toAllEmps += dr[0].ToString() + ",";

            ht.Add("CCTo", toAllEmps);

            // 根据他判断是否显示权限组。
            if (BP.DA.DBAccess.IsExitsObject("GPM_Group") == true)
                ht.Add("IsGroup", "1");
            else
                ht.Add("IsGroup", "0");

            //返回流程标题.
            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        /// <summary>
        /// 选择部门呈现信息.
        /// </summary>
        /// <returns></returns>
        public string CC_SelectDepts()
        {
            BP.Port.Depts depts = new BP.Port.Depts();
            depts.RetrieveAll();
            return depts.ToJson();
        }
        /// <summary>
        /// 选择部门呈现信息.
        /// </summary>
        /// <returns></returns>
        public string CC_SelectStations()
        {
            //岗位类型.
            string sql = "SELECT NO,NAME FROM Port_StationType ORDER BY NO";
            DataSet ds = new DataSet();
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Port_StationType";
            ds.Tables.Add(dt);

            //岗位.
            string sqlStas = "SELECT NO,NAME,FK_STATIONTYPE FROM Port_Station ORDER BY FK_STATIONTYPE,NO";
            DataTable dtSta = BP.DA.DBAccess.RunSQLReturnTable(sqlStas);
            dtSta.TableName = "Port_Station";
            ds.Tables.Add(dtSta);
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 抄送发送.
        /// </summary>
        /// <returns></returns>
        public string CC_Send()
        {
            //人员信息. 格式 zhangsan,张三;lisi,李四;
            string emps = this.GetRequestVal("Emps");

            //岗位信息. 格式:  001,002,003,
            string stations = this.GetRequestVal("Stations");
            stations = stations.Replace(";", ",");

            //权限组. 格式:  001,002,003,
            string groups = this.GetRequestVal("Groups");
            if (groups == null)
                groups = "";
            groups = groups.Replace(";", ",");

            //部门信息.  格式: 001,002,003,
            string depts = this.GetRequestVal("Depts");
            //标题.
            string title = this.GetRequestVal("TB_Title");
            //内容.
            string doc = this.GetRequestVal("TB_Doc");

            //调用抄送接口执行抄送.
            string ccRec = BP.WF.Dev2Interface.Node_CC_WriteTo_CClist(this.FK_Node, this.WorkID, title, doc, emps, depts, stations, groups);

            if (ccRec == "")
                return "没有抄送到任何人。";
            else
                return "本次抄送给如下人员：" + ccRec;

            //return "执行抄送成功.emps=(" + emps + ")  depts=(" + depts + ") stas=(" + stations + ") 标题:" + title + " ,抄送内容:" + doc;
        }

        #region 退回到分流节点处理器.
        /// <summary>
        /// 初始化.
        /// </summary>
        /// <returns></returns>
        public string DealSubThreadReturnToHL_Init()
        {
            /* 如果工作节点退回了*/
            BP.WF.ReturnWorks rws = new BP.WF.ReturnWorks();
            rws.Retrieve(BP.WF.ReturnWorkAttr.ReturnToNode, this.FK_Node,
                BP.WF.ReturnWorkAttr.WorkID, this.WorkID,
                BP.WF.ReturnWorkAttr.RDT);

            string msgInfo = "";
            if (rws.Count != 0)
            {
                foreach (BP.WF.ReturnWork rw in rws)
                {
                    msgInfo += "<fieldset width='100%' ><legend>&nbsp; 来自节点:" + rw.ReturnNodeName + " 退回人:" + rw.ReturnerName + "  " + rw.RDT + "&nbsp;<a href='./../../DataUser/ReturnLog/" + this.FK_Flow + "/" + rw.MyPK + ".htm' target=_blank>工作日志</a></legend>";
                    msgInfo += rw.BeiZhuHtml;
                    msgInfo += "</fieldset>";
                }
            }

            //把节点信息也传入过去，用于判断不同的按钮显示. 
            BP.WF.Template.BtnLab btn = new BtnLab(this.FK_Node);
            BP.WF.Node nd = new Node(this.FK_Node);

            Hashtable ht = new Hashtable();
            //消息.
            ht.Add("MsgInfo", msgInfo);

            //是否可以移交？
            if (btn.ShiftEnable)
                ht.Add("ShiftEnable", "1");
            else
                ht.Add("ShiftEnable", "0");

            //是否可以撤销？
            if (nd.HisCancelRole == CancelRole.None)
                ht.Add("CancelRole", "0");
            else
                ht.Add("CancelRole", "1");

            //是否可以删除子线程? 在分流节点上.
            if (btn.ThreadIsCanDel)
                ht.Add("ThreadIsCanDel", "1");
            else
                ht.Add("ThreadIsCanDel", "0");

            //是否可以移交子线程? 在分流节点上.
            if (btn.ThreadIsCanShift)
                ht.Add("ThreadIsCanShift", "1");
            else
                ht.Add("ThreadIsCanShift", "0");

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string DealSubThreadReturnToHL_Done()
        {
            //操作类型.
            string actionType = this.GetRequestVal("ActionType");
            string note = this.GetRequestVal("Note");


            if (actionType == "Return")
            {
                /*如果是退回. */
                BP.WF.ReturnWork rw = new BP.WF.ReturnWork();
                rw.Retrieve(BP.WF.ReturnWorkAttr.ReturnToNode, this.FK_Node,
                         BP.WF.ReturnWorkAttr.WorkID, this.WorkID);
                string info = BP.WF.Dev2Interface.Node_ReturnWork(this.FK_Flow, this.WorkID, this.FID,
                    this.FK_Node, rw.ReturnNode, note, false);
                return info;
            }


            if (actionType == "Shift")
            {
                /*如果是移交操作.*/
                string toEmps = this.GetRequestVal("ShiftToEmp");
                return BP.WF.Dev2Interface.Node_Shift(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, toEmps, note);
            }

            if (actionType == "Kill")
            {
                string msg = BP.WF.Dev2Interface.Flow_DeleteSubThread(this.FK_Flow, this.WorkID, "手工删除");
                //提示信息.
                if (msg == "" || msg == null)
                    msg = "该工作删除成功...";
                return msg;
            }

            if (actionType == "UnSend")
            {
                return BP.WF.Dev2Interface.Flow_DoUnSend(this.FK_Flow, this.FID, this.FK_Node);
            }

            return "err@没有判断的类型" + actionType;
        }
        #endregion 退回到分流节点处理器.

        public string DeleteFlowInstance_Init()
        {
            if (BP.WF.Dev2Interface.Flow_IsCanDeleteFlowInstance(this.FK_Flow,
                this.WorkID, BP.Web.WebUser.No) == false)
                return "err@您没有删除该流程的权限";
            //获取节点中配置的流程删除规则
            if(this.FK_Node != 0)
            {
                string sql = "SELECT wn.DelEnable FROM WF_Node wn WHERE wn.NodeID = " + this.FK_Node;
                return DBAccess.RunSQLReturnValInt(sql) + "";
            }

            return "";
        }

        public string DeleteFlowInstance_DoDelete()
        {
            if (BP.WF.Dev2Interface.Flow_IsCanDeleteFlowInstance(this.FK_Flow,
                this.WorkID, BP.Web.WebUser.No) == false)
                return "err@您没有删除该流程的权限.";

            string deleteWay = this.GetRequestVal("RB_DeleteWay");
            string doc = this.GetRequestVal("TB_Doc");

            //是否要删除子流程？ 这里注意变量的获取方式，你可以自己定义.
            string isDeleteSubFlow = this.GetRequestVal("CB_IsDeleteSubFlow");

            bool isDelSubFlow = false;
            if (isDeleteSubFlow == "1")
                isDelSubFlow = true;

            //按照标记删除.
            if (deleteWay == "1")
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(this.FK_Flow, this.WorkID, doc, isDelSubFlow);

            //彻底删除.
            if (deleteWay == "3")
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.FK_Flow, this.WorkID, isDelSubFlow);

            //彻底并放入到删除轨迹里.
            if (deleteWay == "2")
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByWriteLog(this.FK_Flow, this.WorkID, doc, isDelSubFlow);

            return "流程删除成功.";
        }
        /// <summary>
        /// 获得节点表单数据.
        /// </summary>
        /// <returns></returns>
        public string ViewWorkNodeFrm()
        {
            Node nd = new Node(this.FK_Node);

            Hashtable ht = new Hashtable();
            ht.Add("FormType", nd.FormType.ToString());
            ht.Add("Url", nd.FormUrl + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node);

            if (nd.FormType == NodeFormType.SDKForm)
                return BP.Tools.Json.ToJsonEntityModel(ht);

            if (nd.FormType == NodeFormType.SelfForm)
                return BP.Tools.Json.ToJsonEntityModel(ht);

            //表单模版.
            DataSet myds = BP.Sys.CCFormAPI.GenerHisDataSet(nd.NodeFrmID);
            string json = BP.WF.Dev2Interface.CCFrom_GetFrmDBJson(this.FK_Flow, this.MyPK);
            DataTable mainTable = BP.Tools.Json.ToDataTableOneRow(json);
            mainTable.TableName = "MainTable";
            myds.Tables.Add(mainTable);

            //MapExts exts = new MapExts(nd.HisWork.ToString());
            //DataTable dtMapExt = exts.ToDataTableDescField();
            //dtMapExt.TableName = "Sys_MapExt";
            //myds.Tables.Add(dtMapExt);

            return BP.Tools.Json.ToJson(myds);
        }

        /// <summary>
        /// 回复加签信息.
        /// </summary>
        /// <returns></returns>
        public string AskForRe()
        {
            string note = this.GetRequestVal("Note"); //原因.
            return BP.WF.Dev2Interface.Node_AskforReply(this.WorkID, note);
        }
        /// <summary>
        /// 执行加签
        /// </summary>
        /// <returns>执行信息</returns>
        public string Askfor()
        {
            Int64 workID = int.Parse(this.GetRequestVal("WorkID")); //工作ID
            string toEmp = this.GetRequestVal("ToEmp"); //让谁加签?
            string note = this.GetRequestVal("Note"); //原因.
            string model = this.GetRequestVal("Model"); //模式.

            BP.WF.AskforHelpSta sta = BP.WF.AskforHelpSta.AfterDealSend;
            if (model == "0")
                sta = BP.WF.AskforHelpSta.AfterDealSend;

            return BP.WF.Dev2Interface.Node_Askfor(workID, sta, toEmp, note);
        }
        /// <summary>
        /// 人员选择器
        /// </summary>
        /// <returns></returns>
        public string SelectEmps_Init()
        {
            string fk_dept = this.FK_Dept;
            if (string.IsNullOrEmpty(fk_dept)==true)
                fk_dept = BP.Web.WebUser.FK_Dept;

            DataSet ds = new DataSet();

            string sql = "SELECT No,Name,ParentNo FROM Port_Dept WHERE No='" + fk_dept + "' OR ParentNo='" + fk_dept + "' ";
            DataTable dtDept = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtDept.TableName = "Depts";
            ds.Tables.Add(dtDept);

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dtDept.Columns[0].ColumnName = "No";
                dtDept.Columns[1].ColumnName = "Name";
                dtDept.Columns[2].ColumnName = "ParentNo";
            }

            sql = "SELECT No,Name,FK_Dept FROM Port_Emp WHERE FK_Dept='" + fk_dept + "'";
            DataTable dtEmps = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtEmps.TableName = "Emps";
            ds.Tables.Add(dtEmps);
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dtEmps.Columns[0].ColumnName = "No";
                dtEmps.Columns[1].ColumnName = "Name";
                dtEmps.Columns[2].ColumnName = "FK_Dept";
            }

            //转化为 json 
            return BP.Tools.Json.DataSetToJson(ds,false);
        }

        #region 选择接受人.
        /// <summary>
        /// 初始化接受人.
        /// </summary>
        /// <returns></returns>
        public string Accepter_Init()
        {
            /*如果是协作模式, 就要检查当前是否主持人, 当前是否是会签模式. */
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (gwf.FK_Node != this.FK_Node)
                return "err@当前流程已经运动到[" + gwf.NodeName + "]上,当前处理人员为[" + gwf.TodoEmps + "]";

            //当前节点ID.
            Node nd = new Node(this.FK_Node);

            //判断当前是否是协作模式.
            if (nd.TodolistModel == TodolistModel.Teamup && nd.IsStartNode==false)
            {
                if (gwf.TodoEmps.Contains(WebUser.No + ","))
                {
                    /*说明我是主持人之一, 我就可以选择接受人,发送到下一个节点上去.*/
                }
                else
                {
                    /* 不是主持人就执行发送，返回发送结果. */
                    SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID);
                    return "info@" + objs.ToMsgOfHtml();
                }
            }

            int toNodeID = this.GetRequestValInt("ToNode");
            if (toNodeID == 0)
            {
                Nodes nds = nd.HisToNodes;
                if (nds.Count == 1)
                    toNodeID = nds[0].GetValIntByKey("NodeID");
                else
                    return "err@参数错误,必须传递来到达的节点ID ToNode .";
            }

            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();

            Selector select = new Selector(toNodeID);
            if (select.SelectorModel == SelectorModel.GenerUserSelecter)
                return "url@AccepterOfGener.htm?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + nd.FK_Flow + "&ToNode=" + toNodeID;

            //获得 部门与人员.
            DataSet ds = select.GenerDataSet(toNodeID, wk);

            #region 计算上一次选择的结果, 并把结果返回过去.
            string sql = "";
            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(string));
            dt.TableName = "Selected";
            if (select.IsAutoLoadEmps)
            {
                if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                    sql = "SELECT  top 1 Tag,EmpTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + this.FK_Node + " AND A.NDTo=" + toNodeID + " AND ActionType=1 ORDER BY WorkID DESC";
                else if (SystemConfig.AppCenterDBType == DBType.Oracle)
                    sql = "SELECT  Tag,EmpTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + this.FK_Node + " AND A.NDTo=" + toNodeID + " AND ActionType=1 AND ROWNUM =1  ORDER BY WorkID DESC ";
                else if (SystemConfig.AppCenterDBType == DBType.MySQL)
                    sql = "SELECT  Tag,EmpTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + this.FK_Node + " AND A.NDTo=" + toNodeID + " AND ActionType=1 AND  limit 1,1  ORDER BY WorkID  DESC";

                DataTable mydt = DBAccess.RunSQLReturnTable(sql);
                string emps = "";
                if (mydt.Rows.Count != 0)
                {
                    emps = mydt.Rows[0]["Tag"].ToString();
                    if (emps == "" || emps == null)
                    {
                        emps = mydt.Rows[0]["EmpTo"].ToString();
                        emps = emps + "," + emps;
                    }
                }

                string[] strs = emps.Split(';');
                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str) == true)
                        continue;

                    string[] emp = str.Split(',');
                    if (emp.Length != 2)
                        continue;

                    DataRow dr = dt.NewRow();
                    dr[0] = emp[0];
                    dt.Rows.Add(dr);
                }
            }

            //增加一个table.
            ds.Tables.Add(dt);
            #endregion 计算上一次选择的结果, 并把结果返回过去.

            //返回json.
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        /// <summary>
        /// 保存.
        /// </summary>
        /// <returns></returns>
        public string Accepter_Save()
        {
            try
            {
                //求到达的节点. 
                int toNodeID = 0;
                if (this.GetRequestVal("ToNode") != "0")
                    toNodeID = int.Parse(this.GetRequestVal("ToNode"));

                if (toNodeID == 0)
                {   //没有就获得第一个节点.
                    Node nd = new Node(this.FK_Node);
                    Nodes nds = nd.HisToNodes;
                    toNodeID = nds[0].GetValIntByKey("NodeID");
                }

                //求发送到的人员.
                // string selectEmps = this.GetValFromFrmByKey("SelectEmps");
                string selectEmps = this.GetRequestVal("SelectEmps");
                selectEmps = selectEmps.Replace(";", ",");

                //保存接受人.
                BP.WF.Dev2Interface.Node_AddNextStepAccepters(this.WorkID, toNodeID, selectEmps);
                return "SaveOK@" + selectEmps;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 执行保存并发送.
        /// </summary>
        /// <returns>返回发送的结果.</returns>
        public string Accepter_Send()
        {
            try
            {
                //求到达的节点. 
                int toNodeID = 0;
                if (this.GetRequestVal("ToNode") != "0")
                    toNodeID = int.Parse(this.GetRequestVal("ToNode"));

                if (toNodeID == 0)
                {   //没有就获得第一个节点.
                    Node nd = new Node(this.FK_Node);
                    Nodes nds = nd.HisToNodes;
                    toNodeID = nds[0].GetValIntByKey("NodeID");
                }

                //求发送到的人员.
                // string selectEmps = this.GetValFromFrmByKey("SelectEmps");
                string selectEmps = this.GetRequestVal("SelectEmps");
                selectEmps = selectEmps.Replace(";", ",");

                //执行发送.
                SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, toNodeID, selectEmps);
                return objs.ToMsgOfHtml();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion


        #region 工作退回.
        /// <summary>
        /// 获得可以退回的节点.
        /// </summary>
        /// <returns></returns>
        public string Return_Init()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(this.FK_Node, this.WorkID, this.FID);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 执行退回,返回退回信息.
        /// </summary>
        /// <returns></returns>
        public string DoReturnWork()
        {
            string[] vals = this.GetRequestVal("ReturnToNode").Split('@');
            int toNodeID = int.Parse(vals[0]);
            string reMesage = this.GetRequestVal("ReturnInfo");

            bool isBackBoolen = false;
            string isBack = this.GetRequestVal("IsBack");
            if (isBack == "1")
                isBackBoolen = true;

            return BP.WF.Dev2Interface.Node_ReturnWork(this.FK_Flow, this.WorkID, this.FID, this.FK_Node, toNodeID, reMesage, isBackBoolen);
        }
        #endregion

        /// <summary>
        /// 执行移交.
        /// </summary>
        /// <returns></returns>
        public string Shift()
        {
            string msg = this.GetRequestVal("Message");
            string toEmp = this.GetRequestVal("ToEmp");
            return BP.WF.Dev2Interface.Node_Shift(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, toEmp, msg);
        }
        /// <summary>
        /// 撤销移交
        /// </summary>
        /// <returns></returns>
        public string UnShift()
        {
            return BP.WF.Dev2Interface.Node_ShiftUn(this.FK_Flow, this.WorkID);
        }
        /// <summary>
        /// 执行催办
        /// </summary>
        /// <returns></returns>
        public string Press()
        {
            string msg = this.GetRequestVal("Msg");

            //调用API.
            return BP.WF.Dev2Interface.Flow_DoPress(this.WorkID, msg, true);
        }

      
    }
}
