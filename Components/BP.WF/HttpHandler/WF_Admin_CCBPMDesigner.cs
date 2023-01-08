using System;
using System.Collections;
using System.Data;
using System.Text;
using BP.DA;
using BP.Web;
using BP.WF.Template;
using BP.WF.XML;
using BP.WF.Port.Admin2Group;
using BP.Difference;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 初始化函数 
    /// </summary>
    public class WF_Admin_CCBPMDesigner : DirectoryPageBase
    {
        /// <summary>
        /// 选择器
        /// </summary>
        /// <returns></returns>
        public string SelectFlows_Init()
        {
            string fk_flowsort = this.GetRequestVal("FK_FlowSort").Substring(1);

            if (DataType.IsNullOrEmpty(fk_flowsort) == true || fk_flowsort.Equals("undefined") == true)
                fk_flowsort = "99";
            DataSet ds = new DataSet();

            string sql = "";
            if (DBAccess.AppCenterDBType == DBType.MySQL)
                sql = "SELECT CONCAT('F' , No) as No,Name, CONCAT('F' ,ParentNo) as ParentNo FROM WF_FlowSort WHERE No='" + fk_flowsort + "' OR ParentNo='" + fk_flowsort + "' ORDER BY Idx";
            else
                sql = "SELECT 'F' + No as No,Name, 'F' + ParentNo as ParentNo FROM WF_FlowSort WHERE No='" + fk_flowsort + "' OR ParentNo='" + fk_flowsort + "' ORDER BY Idx";

            DataTable dtFlowSorts = DBAccess.RunSQLReturnTable(sql);
            //if (dtFlowSort.Rows.Count == 0)
            //{
            //    fk_dept = BP.Web.WebUser.FK_Dept;
            //    sql = "SELECT No,Name,ParentNo FROM Port_Dept WHERE No='" + fk_dept + "' OR ParentNo='" + fk_dept + "' ORDER BY Idx ";
            //    dtDept = DBAccess.RunSQLReturnTable(sql);
            //}

            dtFlowSorts.TableName = "FlowSorts";
            ds.Tables.Add(dtFlowSorts);

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dtFlowSorts.Columns[0].ColumnName = "No";
                dtFlowSorts.Columns[1].ColumnName = "Name";
                dtFlowSorts.Columns[2].ColumnName = "ParentNo";
            }

            //sql = "SELECT No,Name, FK_Dept FROM Port_Emp WHERE FK_Dept='" + fk_dept + "' ";

            if (DBAccess.AppCenterDBType == DBType.MySQL)

                sql = "SELECT  No,CONCAT(NO ,'.',NAME) as Name, CONCAT('F',FK_FlowSort) as ParentNo, Idx FROM WF_Flow where FK_FlowSort='" + fk_flowsort + "' ";
            else
                sql = "SELECT  No,(NO + '.' + NAME) as Name, 'F' + FK_FlowSort as ParentNo, Idx FROM WF_Flow where FK_FlowSort='" + fk_flowsort + "' ";

            sql += " ORDER BY Idx ";

            DataTable dtFlows = DBAccess.RunSQLReturnTable(sql);
            dtFlows.TableName = "Flows";
            ds.Tables.Add(dtFlows);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dtFlows.Columns[0].ColumnName = "No";
                dtFlows.Columns[1].ColumnName = "Name";
                dtFlows.Columns[2].ColumnName = "FK_FlowSort";
            }

            //转化为 json 
            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        /// <summary>
        /// 按照管理员登录.
        /// </summary>
        /// <param name="userNo">管理员编号</param>
        /// <returns>登录信息</returns>
        public string AdminerChang_LoginAs()
        {
            string orgNo = this.GetRequestVal("OrgNo");
            WebUser.OrgNo = this.OrgNo;
            return "info@登录成功, 如果系统不能自动刷新，请手工刷新。";
        }

        public string Flows_Init()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("FlowNo");
            dt.Columns.Add("FlowName");

            dt.Columns.Add("NumOfRuning"); //运行中的.
            dt.Columns.Add("NumOfOK"); //已经完成的.
            dt.Columns.Add("NumOfEtc"); //其他.

            Flows fls = new Flows();
            fls.RetrieveAll();

            foreach (Flow fl in fls)
            {
                DataRow dr = dt.NewRow();
                dr["FlowNo"] = fl.No;
                dr["FlowName"] = fl.Name;
                dr["NumOfRuning"] = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM  WF_GenerWorkFlow WHERE FK_Flow='" + fl.No + "' AND WFState in (2,5)", 0);
                dr["NumOfOK"] = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM  WF_GenerWorkFlow WHERE FK_Flow='" + fl.No + "' AND WFState = 3 ", 0);
                dr["NumOfEtc"] = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM  WF_GenerWorkFlow WHERE FK_Flow='" + fl.No + "' AND WFState in (4,5,6,7,8) ", 0);

                dt.Rows.Add(dr);
            }
            return BP.Tools.Json.ToJson(dt);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_CCBPMDesigner()
        {
        }
        /// <summary>
        /// 执行流程设计图的保存.
        /// </summary>
        /// <returns></returns>
        public string Designer_Save()
        {

            if (BP.Web.WebUser.IsAdmin == false)
                return "err@当前您【" + WebUser.No + "," + WebUser.Name + "】不是管理员,请重新登录.造成这种原因是您在测试容器没有正常退回造成的.";

            string sql = "";
            try
            {
                Flow flow = new Flow(this.FK_Flow);

                StringBuilder sBuilder = new StringBuilder();

                //保存方向.
                sBuilder = new StringBuilder();
                string[] dirs = this.GetRequestVal("Dirs").Split('@');

                Direction mydir = new Direction();
                foreach (string item in dirs)
                {
                    if (item == "" || item == null)
                        continue;
                    string[] strs = item.Split(',');
                    mydir.setMyPK(strs[0]);
                    if (mydir.IsExits == true)
                        continue;

                    sBuilder.Append("DELETE FROM WF_Direction WHERE MyPK='" + strs[0] + "';");

                    sBuilder.Append("INSERT INTO WF_Direction (MyPK,FK_Flow,Node,ToNode) VALUES ('" + strs[0] + "','" + strs[1] + "','" + strs[2] + "','" + strs[3] + "');");
                }
                DBAccess.RunSQLs(sBuilder.ToString());

                //保存label位置.
                sBuilder = new StringBuilder();
                string[] labs = this.GetRequestVal("Labs").Split('@');
                foreach (string item in labs)
                {
                    if (DataType.IsNullOrEmpty(item) == true)
                        continue;
                    string[] strs = item.Split(',');

                    sBuilder.Append("UPDATE WF_LabNote SET X=" + strs[1] + ",Y=" + strs[2] + " WHERE MyPK='" + strs[0] + "';");
                }

                string sqls = sBuilder.ToString();
                DBAccess.RunSQLs(sqls);

                //更新节点 HisToNDs，不然就需要检查一遍.
                BP.WF.Nodes nds = new Nodes();
                nds.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, this.FK_Flow);

                //获得方向集合处理toNodes
                Directions mydirs = new Directions(this.FK_Flow);

                string mystrs = "";
                foreach (Node item in nds)
                {
                    string strs = "";
                    foreach (Direction dir in mydirs)
                    {
                        if (dir.Node != item.NodeID)
                            continue;

                        strs += "@" + dir.ToNode;
                    }

                    int nodePosType = 0;
                    if (item.IsStartNode == true)
                        nodePosType = 0;
                    else if (DataType.IsNullOrEmpty(strs) == true)
                        nodePosType = 2;
                    else
                        nodePosType = 1;

                    DBAccess.RunSQL("UPDATE WF_Node SET HisToNDs='" + strs + "',NodePosType=" + nodePosType + "  WHERE NodeID=" + item.NodeID);

                    DBAccess.RunSQL("UPDATE Sys_MapData SET Name='"+item.Name+"' WHERE No='ND"+item.NodeID+"'");
                }

                //获取所有子流程
                string subs = this.GetRequestVal("SubFlows");
                int subFlowShowType = flow.GetValIntByKey(FlowAttr.SubFlowShowType);
                if (DataType.IsNullOrEmpty(subs) == false && subFlowShowType == 0)
                {
                    string[] subFlows = subs.Split('@');
                    foreach (string item in subFlows)
                    {
                        if (DataType.IsNullOrEmpty(item) == true)
                            continue;
                        String[] strs = item.Split(',');
                        sBuilder.Append("UPDATE WF_NodeSubFlow SET X=" + strs[1] + ",Y=" + strs[2] + " WHERE MyPK=" + strs[0] + ";");
                    }
                }
                //保存节点位置. @101,2,30@102,3,1
                string[] nodes = this.GetRequestVal("Nodes").Split('@');
                foreach (string item in nodes)
                {
                    if (DataType.IsNullOrEmpty(item) == true)
                        continue;

                    string[] strs = item.Split(',');
                    string nodeID = strs[0]; //获得nodeID.
                    if (subFlowShowType == 1 && subs.IndexOf(nodeID) != -1)
                    {
                        string sub = subs.Substring(subs.IndexOf("@\"" + nodeID) + 1);
                        if (sub.Contains("@") == true)
                            sub = sub.Substring(0, sub.IndexOf("@"));
                        string[] subInfo = sub.Split(',');
                        sBuilder.Append("UPDATE WF_Node SET X=" + strs[1] + ",Y=" + strs[2] + ",Name='" + strs[3] + "',SubFlowX=" + subInfo[1] + ", SubFlowY=" + subInfo[2] + " WHERE NodeID=" + strs[0] + ";");
                    }
                    else
                    {
                        sBuilder.Append("UPDATE WF_Node SET X=" + strs[1] + ",Y=" + strs[2] + ",Name='" + strs[3] + "' WHERE NodeID=" + strs[0] + ";");
                    }
                }

                DBAccess.RunSQLs(sBuilder.ToString());

                // DBAccess.RunSQL("update WF_Direction set ToNodeName=WF_Node.Name from WF_Node where //WF_Direction.ToNode=WF_Node.NodeID AND WF_Direction.FK_FlOW='" + this.FK_Flow+"'");

                #region 更新节点名称.
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                        sql = " UPDATE WF_Direction SET ToNodeName = WF_Node.Name FROM WF_Node  ";
                        sql += " WHERE WF_Direction.ToNode = WF_Node.NodeID AND WF_Direction.FK_Flow='" + this.FK_Flow + "'";
                        break;
                    default:
                        sql = "UPDATE WF_Direction A, WF_Node B SET A.ToNodeName=B.Name WHERE A.ToNode=B.NodeID AND A.FK_Flow='" + this.FK_Flow + "' ";
                        break;
                }
                DBAccess.RunSQL(sql);
                #endregion 更新节点名称.


                //清楚缓存.
               Cash.ClearCash();
                // Node nd = new Node(102);
                // throw new Exception(nd.Name);

                return "保存成功.";

            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 下载流程模版
        /// </summary>
        /// <returns></returns>
        public string ExpFlowTemplete()
        {
            Flow flow = new Flow(this.FK_Flow);
            string fileXml = flow.GenerFlowXmlTemplete();
            string docs = DataType.ReadTextFile(fileXml);
            return docs;
        }
        /// <summary>
        /// 返回临时文件.
        /// </summary>
        /// <returns></returns>
        public string DownFormTemplete()
        {
            DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet_AllEleInfo(this.FK_MapData);

            string file = BP.Difference.SystemConfig.PathOfTemp + this.FK_MapData + ".xml";
            ds.WriteXml(file);
            string docs = DataType.ReadTextFile(file);
            return docs;
        }

        /// <summary>
        /// 使管理员登录使管理员登录    /// </summary>
        /// <returns></returns>
        public string LetLogin()
        {
            LetAdminLogin(this.GetRequestVal("UserNo"), true);
            return "登录成功.";
        }
        /// <summary>
        /// 获得枚举列表的JSON.
        /// </summary>
        /// <returns></returns>
        public string Logout()
        {
            BP.WF.Dev2Interface.Port_SigOut();
            return "您已经安全退出,欢迎使用ccbpm.";
        }

        /// <summary>
        /// 根据部门、角色获取人员列表
        /// </summary>
        /// <returns></returns>
        public string GetEmpsByStationTable()
        {
            string deptid = this.GetRequestVal("DeptNo");
            string stid = this.GetRequestVal("StationNo");

            if (string.IsNullOrWhiteSpace(deptid) || string.IsNullOrWhiteSpace(stid))
                return "[]";

            DataTable dt = new DataTable();
            dt.Columns.Add("NO", typeof(string));
            dt.Columns.Add("PARENTNO", typeof(string));
            dt.Columns.Add("NAME", typeof(string));
            dt.Columns.Add("TTYPE", typeof(string));


            BP.Port.Emp emp = null;
            BP.Port.Emps emps = new BP.Port.Emps();
            emps.RetrieveAll();

            BP.Port.DeptEmpStations dess = new BP.Port.DeptEmpStations();
            dess.Retrieve(BP.Port.DeptEmpStationAttr.FK_Dept, deptid, BP.Port.DeptEmpStationAttr.FK_Station, stid);

            foreach (BP.Port.DeptEmpStation des in dess)
            {
                emp = emps.GetEntityByKey(des.FK_Emp) as BP.Port.Emp;

                dt.Rows.Add(emp.No, deptid + "|" + stid, emp.Name, "EMP");
            }

            return BP.Tools.Json.ToJson(dt);
        }

        public string GetStructureTreeRootTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NO", typeof(string));
            dt.Columns.Add("PARENTNO", typeof(string));
            dt.Columns.Add("NAME", typeof(string));
            dt.Columns.Add("TTYPE", typeof(string));

            string parentrootid = this.GetRequestVal("parentrootid");  // context.Request.QueryString["parentrootid"];
            string newRootId = "";

            if (WebUser.No != "admin")
            {
                newRootId = WebUser.OrgNo;
            }


            BP.Port.Dept dept = new BP.Port.Dept();

            if (!string.IsNullOrWhiteSpace(newRootId))
            {
                if (dept.Retrieve(BP.Port.DeptAttr.No, newRootId) == 0)
                {
                    dept.No = "-1";
                    dept.Name = "无部门";
                    dept.ParentNo = "";
                }
            }
            else
            {
                if (dept.Retrieve(BP.Port.DeptAttr.ParentNo, parentrootid) == 0)
                {
                    dept.No = "-1";
                    dept.Name = "无部门";
                    dept.ParentNo = "";
                }
            }

            dt.Rows.Add(dept.No, dept.ParentNo, dept.Name, "DEPT");


            return BP.Tools.Json.ToJson(dt);
        }
        #region 主页.
        /// <summary>
        /// 初始化登录界面.
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            try
            {
                //如果登录信息丢失了,就让其重新登录一次.
                if (DataType.IsNullOrEmpty(BP.Web.WebUser.NoOfRel) == true)
                {
                    string userNo = this.GetRequestVal("UserNo");
                    string sid = this.GetRequestVal("Token");
                    BP.WF.Dev2Interface.Port_LoginByToken(sid);
                }

                if (BP.Web.WebUser.IsAdmin == false)
                    return "url@Login.htm?DoType=Logout&Err=NoAdminUsers";

                //如果没有流程表，就执行安装.
                if (DBAccess.IsExitsObject("WF_Flow") == false)
                    return "url@../DBInstall.htm";

                Hashtable ht = new Hashtable();

                ht.Add("OSModel", "1");

                //把系统信息加入里面去.
                ht.Add("SysNo", BP.Difference.SystemConfig.SysNo);
                ht.Add("SysName", BP.Difference.SystemConfig.SysName);

                ht.Add("CustomerNo", BP.Difference.SystemConfig.CustomerNo);
                ht.Add("CustomerName", BP.Difference.SystemConfig.CustomerName);

                //集成的平台.
                ht.Add("RunOnPlant", BP.Difference.SystemConfig.RunOnPlant);

                try
                {
                    // 执行升级
                    string str = BP.WF.Glo.UpdataCCFlowVer();
                    if (str == null)
                        str = "";
                    ht.Add("Msg", str);
                }
                catch (Exception ex)
                {
                    return "err@" + ex.Message;
                }

                //生成Json.
                return BP.Tools.Json.ToJsonEntityModel(ht);
            }
            catch (Exception ex)
            {
                return "err@初始化界面期间出现如下错误:" + ex.Message;
            }
        }
        #endregion

        #region 登录窗口.
        public string Login_InitInfo()
        {
            Hashtable ht = new Hashtable();
            ht.Add("SysNo", BP.Difference.SystemConfig.SysNo);
            ht.Add("SysName", BP.Difference.SystemConfig.SysName);

            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 初始化登录界面.
        /// </summary>
        /// <returns></returns>
        public string Login_Init()
        {
            //检查数据库连接.
            try
            {
                DBAccess.TestIsConnection();
            }
            catch (Exception ex)
            {
                return "err@异常信息:" + ex.Message;
            }

            //检查是否缺少Port_Emp 表，如果没有就是没有安装.
            if (DBAccess.IsExitsObject("Port_Emp") == false && DBAccess.IsExitsObject("WF_Flow") == false)
                return "url@../DBInstall.htm";

            ////让admin登录
            //if (DataType.IsNullOrEmpty(BP.Web.WebUser.No) || BP.Web.WebUser.IsAdmin == false)
            //    return "url@Login.htm?DoType=Logout";

            //如果没有流程表，就执行安装.
            if (DBAccess.IsExitsObject("WF_Flow") == false)
                return "url@../DBInstall.htm";

            //是否需要自动登录。 这里都把cookeis的数据获取来了.
            string userNo = this.GetRequestVal("UserNo");
            string sid = this.GetRequestVal("Token");

            if (String.IsNullOrEmpty(sid) == false && String.IsNullOrEmpty(userNo) == false)
            {
                /*  如果都有值，就需要他登录。 */
                try
                {
                    string str = BP.WF.Glo.UpdataCCFlowVer();
                    BP.WF.Dev2Interface.Port_LoginByToken(sid);
                    if (this.FK_Flow == null)
                        return "url@Default.htm?UserNo=" + userNo + "&OrgNo=" + WebUser.OrgNo + "&Key=" + DateTime.Now.ToBinary() + "&Token=" + sid;
                    else
                        return "url@Designer.htm?UserNo=" + userNo + "&OrgNo=" + WebUser.OrgNo + "&FK_Flow=" + this.FK_Flow + "&Key=" + DateTime.Now.ToBinary() + "&Token=" + sid;
                }
                catch (Exception ex)
                {
                    return "err@登录失败" + ex.Message;
                }
            }

            try
            {
                // 执行升级
                string str = BP.WF.Glo.UpdataCCFlowVer();
                if (str == null)
                    str = "准备完毕,欢迎登录,当前小版本号为:" + BP.WF.Glo.Ver;
                return str;
            }
            catch (Exception ex)
            {
                string msg = "err@升级失败(ccbpm有自动修复功能,您可以刷新一下系统会自动创建字段,刷新多次扔解决不了问题,请反馈给我们)";
                msg += "@系统信息:" + ex.Message;
                return msg;
            }
        }
        //流程设计器登陆前台，转向规则，判断是否为天业BPM
        public string Login_Redirect()
        {
            if (BP.Difference.SystemConfig.CustomerNo == "TianYe")
                return "url@../../../BPM/pages/login.html";

            return "url@../../AppClassic/Login.htm?DoType=Logout";
        }
        /// <summary>
        ///初始化当前登录人的下的所有组织
        /// </summary>
        /// <returns></returns>
        public string SelectOneOrg_Init()
        {
            Orgs orgs = new Orgs();
            //            orgs.Retrieve("Adminer", WebUser.No);
            orgs.RetrieveInSQL("SELECT OrgNo FROM Port_OrgAdminer WHERE FK_Emp='" + WebUser.No + "'");
            return orgs.ToJson();
        }
        /// <summary>
        ///选择一个组织
        /// </summary>
        /// <returns></returns>
        public string SelectOneOrg_Selected()
        {
            WebUser.OrgNo = this.OrgNo;

            //找到管理员所在的部门.
            string sql = "SELECT a.No FROM Port_Dept A,Port_DeptEmp B WHERE A.No=B.FK_Dept AND B.FK_Emp='" + WebUser.No + "'  AND A.OrgNo='" + this.OrgNo + "'";
            string deptNo = DBAccess.RunSQLReturnStringIsNull(sql, this.OrgNo);

            WebUser.FK_Dept = deptNo;

            //执行更新到用户表信息.
            WebUser.UpdateSIDAndOrgNoSQL();

            return "url@Default.htm?Token=" + WebUser.Token + "&UserNo=" + WebUser.No + "&OrgNo=" + WebUser.OrgNo;
            // return "登录成功.";
        }
        #endregion 登录窗口.


        #region 流程相关 Flow
        /// <summary>
        /// 获取流程所有元素
        /// </summary>
        /// <returns>json data</returns>
        public string Flow_AllElements_ResponseJson()
        {
            BP.WF.Flow flow = new BP.WF.Flow();
            flow.No = this.FK_Flow;
            flow.RetrieveFromDBSources();

            DataSet ds = new DataSet();
            DataTable dtNodes = DBAccess.RunSQLReturnTable("SELECT NODEID,NAME,X,Y,RUNMODEL FROM WF_NODE WHERE FK_FLOW='" + this.FK_Flow + "'");
            dtNodes.TableName = "Nodes";
            ds.Tables.Add(dtNodes);

            DataTable dtDirection = DBAccess.RunSQLReturnTable("SELECT NODE,TONODE FROM WF_DIRECTION WHERE FK_FLOW='" + this.FK_Flow + "'");
            dtDirection.TableName = "Direction";
            ds.Tables.Add(dtDirection);

            DataTable dtLabNote = DBAccess.RunSQLReturnTable("SELECT MYPK,NAME,X,Y FROM WF_LABNOTE WHERE FK_FLOW='" + this.FK_Flow + "'");
            dtLabNote.TableName = "LabNote";
            ds.Tables.Add(dtLabNote);


            // return BP.Tools.Json.DataSetToJson(ds, false);
            return BP.Tools.Json.ToJson(ds);
        }
        #endregion end Flow

        #region 节点相关 Nodes
        /// <summary>
        /// 根据节点编号删除流程节点
        /// </summary>
        /// <returns>执行结果</returns>
        public string DeleteNode()
        {
            try
            {
                BP.WF.Node node = new BP.WF.Node();
                node.NodeID = this.FK_Node;
                if (node.RetrieveFromDBSources() == 0)
                    return "err@删除失败,没有删除到数据，估计该节点已经别删除了.";

                if (node.IsStartNode == true)
                    return "err@开始节点不允许被删除。";

                node.Delete();
                return "删除成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 修改节点名称
        /// </summary>
        /// <returns></returns>
        public string Node_EditNodeName()
        {
            string FK_Node = this.GetValFromFrmByKey("NodeID");
            //string NodeName = System.Web.HttpContext.Current.Server.UrlDecode(this.GetValFromFrmByKey("NodeName"));
            string NodeName = HttpContextHelper.UrlDecode(this.GetValFromFrmByKey("NodeName"));

            BP.WF.Node node = new BP.WF.Node();
            node.NodeID = int.Parse(FK_Node);
            int iResult = node.RetrieveFromDBSources();
            if (iResult > 0)
            {
                node.Name = NodeName;
                node.Update();
                return "@修改成功.";
            }

            return "err@修改节点失败，请确认该节点是否存在？";
        }

        ///// <summary>
        ///// 修改节点运行模式
        ///// </summary>
        ///// <returns></returns>
        //public string Node_ChangeRunModel()
        //{
        //    string runModel = GetValFromFrmByKey("RunModel");
        //    BP.WF.Node node = new BP.WF.Node(this.FK_Node);
        //    //节点运行模式
        //    switch (runModel)
        //    {
        //        case "NodeOrdinary":
        //            node.HisRunModel = BP.WF.RunModel.Ordinary;
        //            break;
        //        case "NodeFL":
        //            node.HisRunModel = BP.WF.RunModel.FL;
        //            break;
        //        case "NodeHL":
        //            node.HisRunModel = BP.WF.RunModel.HL;
        //            break;
        //        case "NodeFHL":
        //            node.HisRunModel = BP.WF.RunModel.FHL;
        //            break;
        //        case "NodeSubThread":
        //            node.HisRunModel = BP.WF.RunModel.SubThread;
        //            break;
        //    }
        //    node.Update();

        //    return "设置成功.";
        //}
        #endregion end Node

        #region CCBPMDesigner

        StringBuilder sbJson = new StringBuilder();
        public void GenerChildRows(DataTable dt, DataTable newDt, DataRow parentRow)
        {
            DataRow[] rows = dt.Select("ParentNo='" + parentRow["NO"] + "'");
            foreach (DataRow r in rows)
            {
                newDt.Rows.Add(r.ItemArray);
                GenerChildRows(dt, newDt, r);
            }
        }

        public string GetBindingFormsTable()
        {
            string fk_flow = GetValFromFrmByKey("fk_flow");
            if (string.IsNullOrWhiteSpace(fk_flow))
                return "[]";

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT wfn.FK_Frm NO,");
            sql.AppendLine("       smd.NAME,");
            sql.AppendLine("       NULL PARENTNO,");
            sql.AppendLine("       'FORM' TTYPE,");
            sql.AppendLine("       -1 DTYPE,");
            sql.AppendLine("       0 ISPARENT");
            sql.AppendLine("FROM   WF_FrmNode wfn");
            sql.AppendLine("       INNER JOIN Sys_MapData smd");
            sql.AppendLine("            ON  smd.No=wfn.FK_Frm");
            sql.AppendLine("WHERE  wfn.FK_Flow = '{0}'");
            sql.AppendLine("       AND wfn.FK_Node = (");
            sql.AppendLine("               SELECT wn.NodeID");
            sql.AppendLine("               FROM   WF_Node wn");
            sql.AppendLine("               WHERE  wn.FK_Flow = '{0}' AND wn.NodePosType = 0");
            sql.AppendLine("           )");

            DataTable dt = DBAccess.RunSQLReturnTable(string.Format(sql.ToString(), fk_flow));
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获取设计器 - 系统维护菜单数据
        /// 系统维护管理员菜单 需要翻译
        /// </summary>
        /// <returns></returns>
        public string GetTreeJson_AdminMenu()
        {
            string treeJson = string.Empty;
            //查询全部.
            AdminMenus groups = new AdminMenus();
            groups.RetrieveAll();

            return BP.Tools.Json.ToJson(groups.ToDataTable());
            return treeJson;
        }

        /// <summary>
        /// 根据DataTable生成Json树结构
        /// </summary>
        public string GetTreeJsonByTable(DataTable tabel, object pId, string rela = "ParentNo", string idCol = "No", string txtCol = "Name", string IsParent = "IsParent", string sChecked = "", string[] attrFields = null)
        {
            string treeJson = string.Empty;

            if (tabel.Rows.Count > 0)
            {
                sbJson.Append("[");
                string filer = string.Empty;
                if (pId.ToString() == "")
                {
                    filer = string.Format("{0} is null or {0}='-1' or {0}='0' or {0}='F0'", rela);
                }
                else
                {
                    filer = string.Format("{0}='{1}'", rela, pId);
                }

                DataRow[] rows = tabel.Select(filer, idCol);
                if (rows.Length > 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        DataRow row = rows[i];

                        string jNo = row[idCol] as string;
                        string jText = row[txtCol] as string;
                        if (jText.Length > 25)
                            jText = jText.Substring(0, 25) + "<img src='../Scripts/easyUI/themes/icons/add2.png' onclick='moreText(" + jNo + ")'/>";

                        string jIsParent = row[IsParent].ToString();
                        string jState = "1".Equals(jIsParent) ? "open" : "closed";
                        jState = "open".Equals(jState) && i == 0 ? "open" : "closed";

                        DataRow[] rowChild = tabel.Select(string.Format("{0}='{1}'", rela, jNo));
                        string tmp = "{\"id\":\"" + jNo + "\",\"text\":\"" + jText;

                        //增加自定义attributes列，added by liuxc,2015-10-6
                        var attrs = string.Empty;
                        if (attrFields != null && attrFields.Length > 0)
                        {
                            foreach (var field in attrFields)
                            {
                                if (!tabel.Columns.Contains(field)) continue;
                                if (DataType.IsNullOrEmpty(row[field].ToString()))
                                {
                                    attrs += ",\"" + field + "\":\"\"";
                                    continue;
                                }
                                attrs += ",\"" + field + "\":" +
                                         (tabel.Columns[field].DataType == typeof(string)
                                              ? string.Format("\"{0}\"", row[field])
                                              : row[field]);
                            }
                        }

                        if ("0".Equals(pId.ToString()) || row[rela].ToString() == "F0")
                        {
                            tmp += "\",\"attributes\":{\"IsParent\":\"" + jIsParent + "\",\"IsRoot\":\"1\"" + attrs + "}";
                        }
                        else
                        {
                            tmp += "\",\"attributes\":{\"IsParent\":\"" + jIsParent + "\"" + attrs + "}";
                        }

                        if (rowChild.Length > 0)
                        {
                            tmp += ",\"checked\":" + sChecked.Contains("," + jNo + ",").ToString().ToLower()
                                + ",\"state\":\"" + jState + "\"";
                        }
                        else
                        {
                            tmp += ",\"checked\":" + sChecked.Contains("," + jNo + ",").ToString().ToLower();
                        }

                        sbJson.Append(tmp);
                        if (rowChild.Length > 0)
                        {
                            sbJson.Append(",\"children\":");
                            GetTreeJsonByTable(tabel, jNo, rela, idCol, txtCol, IsParent, sChecked, attrFields);
                        }

                        sbJson.Append("},");
                    }
                    sbJson = sbJson.Remove(sbJson.Length - 1, 1);
                }
                sbJson.Append("]");
                treeJson = sbJson.ToString();
            }
            return treeJson;
        }
        /// <summary>
        /// 上移流程类别
        /// </summary>
        /// <returns></returns>
        public String MoveUpFlowSort()
        {
            String fk_flowSort = this.GetRequestVal("FK_FlowSort").Replace("F", "");
            FlowSort fsSub = new FlowSort(fk_flowSort); //传入的编号多出F符号，需要替换掉
            fsSub.DoUp();
            return "F" + fsSub.No;
        }
        /// <summary>
        /// 下移流程类别
        /// </summary>
        /// <returns></returns>
        public String MoveDownFlowSort()
        {
            String fk_flowSort = this.GetRequestVal("FK_FlowSort").Replace("F", "");
            FlowSort fsSub = new FlowSort(fk_flowSort); //传入的编号多出F符号，需要替换掉
            fsSub.DoDown();
            return "F" + fsSub.No;
        }
        /// <summary>
        /// 上移流程
        /// </summary>
        /// <returns></returns>
        public string MoveUpFlow()
        {
            Flow flow = new Flow(this.FK_Flow);
            flow.DoUp();
            return flow.No;
        }
        /// <summary>
        /// 下移流程
        /// </summary>
        /// <returns></returns>
        public string MoveDownFlow()
        {
            Flow flow = new Flow(this.FK_Flow);
            flow.DoDown();
            return flow.No;
        }
        /// <summary>
        /// 删除流程类别.
        /// </summary>
        /// <returns></returns>
        public string DelFlowSort()
        {
            string fk_flowSort = this.GetRequestVal("FK_FlowSort").Replace("F", "");

            FlowSort fs = new FlowSort();
            fs.No = fk_flowSort;

            //检查是否有流程？
            Paras ps = new Paras();
            ps.SQL = "SELECT COUNT(*) FROM WF_Flow WHERE FK_FlowSort=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "fk_flowSort";
            ps.Add("fk_flowSort", fk_flowSort);
            //string sql = "SELECT COUNT(*) FROM WF_Flow WHERE FK_FlowSort='" + fk_flowSort + "'";
            if (DBAccess.RunSQLReturnValInt(ps) != 0)
                return "err@该目录下有流程，您不能删除。";

            //检查是否有子目录？
            ps = new Paras();
            ps.SQL = "SELECT COUNT(*) FROM WF_FlowSort WHERE ParentNo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "ParentNo";
            ps.Add("ParentNo", fk_flowSort);
            //sql = "SELECT COUNT(*) FROM WF_FlowSort WHERE ParentNo='" + fk_flowSort + "'";
            if (DBAccess.RunSQLReturnValInt(ps) != 0)
                return "err@该目录下有子目录，您不能删除。";

            fs.Delete();

            return "删除成功.";
        }
        /// <summary>
        /// 新建同级流程类别 对照需要翻译
        /// </summary>
        /// <returns></returns>
        public string NewSameLevelFlowSort()
        {
            FlowSort fs = null;
            fs = new FlowSort(this.No.Replace("F", "")); //传入的编号多出F符号，需要替换掉.

            string orgNo = fs.OrgNo; //记录原来的组织结构编号. 对照需要翻译

            string sameNodeNo = fs.DoCreateSameLevelNode().No;
            fs = new FlowSort(sameNodeNo);
            fs.Name = this.Name;
            fs.OrgNo = orgNo; // 组织结构编号. 对照需要翻译
            fs.Update();
            return "F" + fs.No;
        }
        /// <summary>
        /// 新建下级类别. 
        /// </summary>
        /// <returns></returns>
        public string NewSubFlowSort()
        {
            FlowSort fsSub = new FlowSort(this.No.Replace("F", ""));//传入的编号多出F符号，需要替换掉.
            string orgNo = fsSub.OrgNo; //记录原来的组织结构编号. 对照需要翻译

            string subNodeNo = fsSub.DoCreateSubNode().No;
            FlowSort subFlowSort = new FlowSort(subNodeNo);
            subFlowSort.Name = this.Name;
            subFlowSort.OrgNo = orgNo; // 组织结构编号. 对照需要翻译.
            subFlowSort.Update();
            return "F" + subFlowSort.No;
        }
        /// <summary>
        /// 表单树 - 删除表单类别
        /// </summary>
        /// <returns></returns>
        public string CCForm_DelFormSort()
        {
            SysFormTree formTree = new SysFormTree(this.No);

            //检查是否有子类别？
            Paras ps = new Paras();
            ps.SQL = "SELECT COUNT(*) FROM Sys_FormTree WHERE ParentNo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "ParentNo";
            ps.Add("ParentNo", this.No);
            //string sql = "SELECT COUNT(*) FROM Sys_FormTree WHERE ParentNo='" + this.No + "'";
            if (DBAccess.RunSQLReturnValInt(ps) != 0)
                return "err@该目录下有子类别，您不能删除。";

            //检查是否有表单？
            ps = new Paras();
            ps.SQL = "SELECT COUNT(*) FROM Sys_MapData WHERE FK_FormTree=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_FormTree";
            ps.Add("FK_FormTree", this.No);
            //sql = "SELECT COUNT(*) FROM Sys_MapData WHERE FK_FormTree='" + this.No + "'";
            if (DBAccess.RunSQLReturnValInt(ps) != 0)
                return "err@该目录下有表单，您不能删除。";

            formTree.Delete();
            return "删除成功";
        }
        /// <summary>
        /// 让admin登录
        /// </summary>
        /// <returns></returns>
        public string LetAdminLoginByToken()
        {
            try
            {
                string userNo = this.GetRequestVal("UserNo");
                string sid = this.GetRequestVal("Token");

                BP.WF.Dev2Interface.Port_LoginByToken(sid);

                return "info@登录成功";
            }
            catch (Exception ex)
            {
                return "err@登录失败:" + ex.Message;
            }
        }
        /// <summary>
        /// 让admin 登陆
        /// </summary>
        /// <param name="lang">当前的语言</param>
        /// <returns>成功则为空，有异常时返回异常信息</returns>
        public string LetAdminLogin(string empNo, bool islogin)
        {
            try
            {
                if (islogin)
                {
                    BP.Port.Emp emp = new BP.Port.Emp(empNo);
                    WebUser.SignInOfGener(emp);
                }
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            return "@登录成功.";
        }
        #endregion

    }
}
