using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Difference;
using BP.WF.Port;
using BP.WF.Template;
using BP.WF.Template.SFlow;
using System;
using System.Collections;
using System.Data;
using System.IO;
using BP.Tools;
using Newtonsoft.Json.Linq;

namespace BP.WF.HttpHandler
{

    public class WF : DirectoryPageBase
    {
        public string DealErrInfo_Save()
        {
            return "";
        }

        #region 单表单查看.
        /// <summary>
        /// 流程单表单查看
        /// </summary>
        /// <returns></returns>
        public string FrmView_Init()
        {
            Node nd = new Node(this.NodeID);
            nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.

            MapData md = new MapData();
            md.No = nd.NodeFrmID;
            if (md.RetrieveFromDBSources() == 0)
            {
                throw new Exception("装载错误，该表单ID=" + md.No + "丢失，请修复一次流程重新加载一次.");
            }

            //获得表单模版.
            DataSet myds = BP.Sys.CCFormAPI.GenerHisDataSet(md.No);

            #region 把主从表数据放入里面.
            //.工作数据放里面去, 放进去前执行一次装载前填充事件.
            BP.WF.Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.RetrieveFromDBSources();

            //重设默认值.
            //wk.ResetDefaultVal();

            DataTable mainTable = wk.ToDataTableField("MainTable");
            mainTable.TableName = "MainTable";
            myds.Tables.Add(mainTable);
            #endregion

            //加入WF_Node.
            DataTable WF_Node = nd.ToDataTableField("WF_Node");
            myds.Tables.Add(WF_Node);

            #region 加入组件的状态信息, 在解析表单的时候使用.
            BP.WF.Template.FrmNodeComponent fnc = new FrmNodeComponent(nd.NodeID);
            myds.Tables.Add(fnc.ToDataTableField("WF_FrmNodeComponent").Copy());
            #endregion 加入组件的状态信息, 在解析表单的时候使用.

            #region 把外键表加入DataSet
            DataTable dtMapAttr = myds.Tables["Sys_MapAttr"];
            DataTable dt = new DataTable();
            MapExts mes = md.MapExts;
            MapExt me = new MapExt();
            DataTable ddlTable = new DataTable();
            ddlTable.Columns.Add("No");
            foreach (DataRow dr in dtMapAttr.Rows)
            {
                string lgType = dr["LGType"].ToString();
                string uiBindKey = dr["UIBindKey"].ToString();

                if (DataType.IsNullOrEmpty(uiBindKey) == true)
                    continue; //为空就continue.

                if (lgType.Equals("1") == true)
                    continue; //枚举值就continue;

                string uiIsEnable = dr["UIIsEnable"].ToString();
                if (uiIsEnable.Equals("0") == true && lgType.Equals("1") == true)
                    continue; //如果是外键，并且是不可以编辑的状态.

                if (uiIsEnable.Equals("1") == true && lgType.Equals("0") == true)
                    continue; //如果是外部数据源，并且是不可以编辑的状态.



                // 检查是否有下拉框自动填充。
                string keyOfEn = dr["KeyOfEn"].ToString();
                string fk_mapData = dr["FK_MapData"].ToString();

                #region 处理下拉框数据范围. for 小杨.
                me = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL, MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
                if (me != null)
                {
                    string fullSQL = me.Doc.Clone() as string;
                    if (DataType.IsNullOrEmpty(fullSQL) == true)
                        throw new Exception("err@没有给AutoFullDLL配置SQL：MapExt：=" + me.MyPK + ",原始的配置SQL为:" + me.Doc);
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
                {
                    continue;
                }

                DataTable mydt = BP.Pub.PubClass.GetDataTableByUIBineKey(uiBindKey);
                if (mydt == null)
                {
                    DataRow ddldr = ddlTable.NewRow();
                    ddldr["No"] = uiBindKey;
                    ddlTable.Rows.Add(ddldr);
                }
                else
                {
                    myds.Tables.Add(mydt);
                }
            }
            ddlTable.TableName = "UIBindKey";
            myds.Tables.Add(ddlTable);
            #endregion End把外键表加入DataSet


            return BP.Tools.Json.ToJson(myds);
        }
        #endregion

        #region 综合查询
        /// <summary>
        /// 综合查询
        /// </summary>
        /// <returns></returns>
        public string SearchZongHe_Init()
        {
            string key = this.GetRequestVal("Key");
            string dtFrom = this.GetRequestVal("DTFrom");
            string dtTo = this.GetRequestVal("DTTo");
            string flowNo = this.GetRequestVal("FlowNo");
            string wfState = this.GetRequestVal("WFState");
            Paras ps = new Paras();
            ps.SQL = "SELECT WorkID,FlowName,NodeName,StarterName,RDT,SendDT,WFState,Title,SDTOfNode FROM WF_GenerWorkFlow ";
            string dbstr = SystemConfig.AppCenterDBVarStr;

            ps.SQL += "  WHERE (Starter=" + dbstr + "Starter OR ";
            if (SystemConfig.AppCenterDBVarStr == "@" || SystemConfig.AppCenterDBVarStr == "?")
            {
                if (SystemConfig.AppCenterDBType == DBType.MySQL)
                    ps.SQL += " Emps like CONCAT('%'," + dbstr + "Emps" + ",'%'))";
                else
                    ps.SQL += " Emps like '%' +" + dbstr + "Emps" + "+'%')";
            }
            else
            {
                ps.SQL += "  Emps like '%'||" + dbstr + "Emps" + "||'%')";
            }

            ps.Add("Starter", WebUser.No);
            ps.Add("Emps", WebUser.No);
            //如果关键字key.
            if (DataType.IsNullOrEmpty(key) == false)
            {
                if (SystemConfig.AppCenterDBVarStr == "@" || SystemConfig.AppCenterDBVarStr == "?")
                {
                    if (SystemConfig.AppCenterDBType == DBType.MySQL)
                        ps.SQL += " AND Title like CONCAT('%'," + dbstr + "Title" + ",'%')";
                    else
                        ps.SQL += "  AND Title like '%' +" + dbstr + "Title" + "+'%'";
                }
                else
                {
                    ps.SQL += " AND Title like '%'||" + dbstr + "Title" + "||'%'";
                }
                ps.Add("Title", key);
            }

            //如果有日期从到.
            if (DataType.IsNullOrEmpty(dtFrom) == false)
            {
                ps.SQL += " AND ( RDT >= " + dbstr + "DtFrom AND RDT <=" + dbstr + "DtTo ) ";
                ps.Add("DtFrom", dtFrom);
                ps.Add("DtTo", dtTo);
            }


            //如果有流程编号.
            if (DataType.IsNullOrEmpty(flowNo) == false)
            {
                ps.SQL += " AND  FK_Flow= " + dbstr + "FK_Flow ";
                ps.Add("FK_Flow", flowNo);
            }

            //如果有流程状态.
            if (DataType.IsNullOrEmpty(wfState) == false && wfState.Equals("all") == false)
            {
                ps.SQL += " AND  WFState= " + dbstr + "WFState ";
                ps.Add("WFState", wfState);
            }
            else
                ps.SQL += " AND  WFState >1 ";

            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dt.Columns[0].ColumnName = "WorkID";
                dt.Columns[1].ColumnName = "FlowName";
                dt.Columns[2].ColumnName = "NodeName";
                dt.Columns[3].ColumnName = "StarterName";
                dt.Columns[4].ColumnName = "RDT";
                dt.Columns[5].ColumnName = "SendDT";
                dt.Columns[6].ColumnName = "WFState";
                dt.Columns[7].ColumnName = "Title";
                dt.Columns[8].ColumnName = "SDTOfNode";
            }
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion

        /// <summary>
        /// 流程数据
        /// </summary>
        /// <returns></returns>
        public string Watchdog_Init()
        {
            string sql = " SELECT FK_Flow,FlowName, COUNT(workid) as Num FROM V_MyFlowData WHERE MyEmpNo='" + WebUser.No + "' ";
            sql += " GROUP BY  FK_Flow,FlowName ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Group";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dt.Columns[0].ColumnName = "FK_Flow";
                dt.Columns[1].ColumnName = "FlowName";
                dt.Columns[2].ColumnName = "Num";
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 流程数据初始化
        /// </summary>
        /// <returns></returns>
        public string Watchdog_InitFlows()
        {
            string sql = " SELECT *  FROM V_MyFlowData WHERE MyEmpNo='" + WebUser.No + "' AND FK_Flow='" + this.FlowNo + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Flows";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                string columnName = "";
                foreach (DataColumn col in dt.Columns)
                {
                    columnName = col.ColumnName.ToUpper();
                    switch (columnName)
                    {
                        case "WORKID":
                            col.ColumnName = "WorkID";
                            break;
                        case "FID":
                            col.ColumnName = "FID";
                            break;
                        case "FK_FLOWSORT":
                            col.ColumnName = "FK_FlowSort";
                            break;
                        case "SYSTYPE":
                            col.ColumnName = "SysType";
                            break;
                        case "FK_FLOW":
                            col.ColumnName = "FK_Flow";
                            break;
                        case "FLOWNAME":
                            col.ColumnName = "FlowName";
                            break;
                        case "TITLE":
                            col.ColumnName = "Title";
                            break;
                        case "WFSTA":
                            col.ColumnName = "WFSta";
                            break;
                        case "WFSTATE":
                            col.ColumnName = "WFState";
                            break;
                        case "STARTER":
                            col.ColumnName = "Starter";
                            break;
                        case "STARTERNAME":
                            col.ColumnName = "StarterName";
                            break;
                        case "RDT":
                            col.ColumnName = "RDT";
                            break;
                        case "PFLOWNO":
                            col.ColumnName = "PFlowNo";
                            break;
                        case "PWORKID":
                            col.ColumnName = "PWorkID";
                            break;
                        case "PNODEID":
                            col.ColumnName = "PNodeID";
                            break;
                        case "TODOEMPS":
                            col.ColumnName = "TodoEmps";
                            break;
                        case "EMPS":
                            col.ColumnName = "Emps";
                            break;
                        case "BILLNO":
                            col.ColumnName = "BillNo";
                            break;
                        case "SENDDT":
                            col.ColumnName = "SendDT";
                            break;
                        case "FK_NODE":
                            col.ColumnName = "FK_Node";
                            break;
                        case "NODENAME":
                            col.ColumnName = "NodeName";
                            break;
                        case "FK_DEPT":
                            col.ColumnName = "FK_Dept";
                            break;
                        case "DEPTNAME":
                            col.ColumnName = "DeptName";
                            break;
                        case "PRI":
                            col.ColumnName = "PRI";
                            break;
                        case "SDTOFNODE":
                            col.ColumnName = "SDTOfNode";
                            break;

                    }

                }
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF()
        {

        }
        /// <summary>
        /// 为宁海特殊处理
        /// </summary>
        /// <returns></returns>
        public string GoToMyFlow_Init()
        {
            string userNo = this.GetRequestVal("UserNo");
            BP.WF.Dev2Interface.Port_Login(userNo);
            return "登录成功";
        }
        protected override string DoDefaultMethod()
        {
            return base.DoDefaultMethod();
        }
        public string HasSealPic()
        {
            string no = GetRequestVal("No");
            if (DataType.IsNullOrEmpty(no))
            {
                return "";
            }

            string path = "/DataUser/Siganture/" + no + ".jpg";
            //如果文件存在

            if (File.Exists(BP.Difference.SystemConfig.PathOfWebApp + (path)) == false)
            {
                path = "/DataUser/Siganture/" + no + ".JPG";
                if (File.Exists(BP.Difference.SystemConfig.PathOfWebApp + (path)) == true)
                {
                    return "";
                }

                //如果不存在，就返回名称
                BP.Port.Emp emp = new BP.Port.Emp(no);
                return emp.Name;
            }
            return "";
        }
        /// <summary>
        /// 执行的方法.
        /// </summary>
        /// <returns></returns>
        public string Do_Init()
        {
            string at = this.GetRequestVal("ActionType");
            if (DataType.IsNullOrEmpty(at))
            {
                at = this.GetRequestVal("DoType");
            }

            if (DataType.IsNullOrEmpty(at) && this.SID != null)
            {
                at = "Track";
            }
            string sid = this.SID;
            try
            {
                switch (at)
                {

                    case "Focus": //把任务放入任务池.
                        BP.WF.Dev2Interface.Flow_Focus(this.WorkID);
                        return "info@Close";
                    case "PutOne": //把任务放入任务池.
                        BP.WF.Dev2Interface.Node_TaskPoolPutOne(this.WorkID);
                        return "info@Close";
                        break;
                    case "DoAppTask": // 申请任务.
                        BP.WF.Dev2Interface.Node_TaskPoolTakebackOne(this.WorkID);
                        return "info@Close";
                    case "DoOpenCC":
                    case "WFRpt":
                        string Sta = this.GetRequestVal("Sta");
                        if (Sta == "0")
                        {
                            CCList cc1 = new CCList();
                            cc1.setMyPK(this.MyPK);
                            cc1.Retrieve();
                            cc1.HisSta = CCSta.Read;
                            cc1.Update();
                        }

                        if (DataType.IsNullOrEmpty(sid) == false)
                        {
                            string[] strss = sid.Split('_');
                            GenerWorkFlow gwfl = new GenerWorkFlow(Int64.Parse(strss[1]));
                            return "url@./MyCC.htm?WorkID=" + gwfl.WorkID + "&FK_Node=" + gwfl.NodeID + "&FK_Flow=" + gwfl.FlowNo + "&FID=" + gwfl.FID + "&CCSta=1";
                        }
                        return "url@./MyCC.htm?WorkID=" + this.WorkID + "&FK_Node=" + this.NodeID + "&FK_Flow=" + this.FlowNo + "&FID=" + this.FID + "&CCSta=1";
                    case "DelCC": //删除抄送.
                        CCList cc = new CCList();
                        cc.setMyPK(this.MyPK);
                        cc.Retrieve();
                        cc.HisSta = CCSta.Del;
                        cc.Update();
                        return "info@Close";
                    case "DelSubFlow": //删除进程。
                        try
                        {
                            BP.WF.Dev2Interface.Flow_DeleteSubThread(this.WorkID, "手工删除");
                            return "info@Close";
                        }
                        catch (Exception ex)
                        {
                            return "err@" + ex.Message;
                        }
                        break;

                    case "DelDtl":
                        GEDtls dtls = new GEDtls(this.EnsName);
                        GEDtl dtl = (GEDtl)dtls.GetNewEntity;
                        dtl.OID = this.RefOID;
                        if (dtl.RetrieveFromDBSources() == 0)
                        {
                            return "info@Close";
                        }
                        FrmEvents fes = new FrmEvents(this.EnsName); //获得事件.

                        // 处理删除前事件.
                        try
                        {
                            fes.DoEventNode(EventListFrm.DtlRowDelBefore, dtl);
                        }
                        catch (Exception ex)
                        {
                            return "err@" + ex.Message;
                        }
                        dtl.Delete();

                        // 处理删除后事件.
                        try
                        {
                            fes.DoEventNode(EventListFrm.DtlRowDelAfter, dtl);
                        }
                        catch (Exception ex)
                        {
                            return "err@" + ex.Message;
                            break;
                        }
                        return "info@Close";
                        break;
                    case "EmpDoUp":
                        BP.WF.Port.WFEmp ep = new BP.WF.Port.WFEmp(this.GetRequestVal("RefNo"));
                        ep.DoUp();

                        BP.WF.Port.WFEmps emps111 = new BP.WF.Port.WFEmps();
                        //  emps111.RemoveCache();
                        emps111.RetrieveAll();
                        return "info@Close";
                        break;
                    case "EmpDoDown":
                        BP.WF.Port.WFEmp ep1 = new BP.WF.Port.WFEmp(this.GetRequestVal("RefNo"));
                        ep1.DoDown();

                        BP.WF.Port.WFEmps emps11441 = new BP.WF.Port.WFEmps();
                        //  emps11441.RemoveCache();
                        emps11441.RetrieveAll();
                        return "info@Close";
                    case "Track": //通过一个串来打开一个工作.
                        string mySid = this.SID; // this.Request.QueryString["Token"];
                        string[] mystrs = mySid.Split('_');

                        Int64 myWorkID = int.Parse(mystrs[1]);
                        string fk_emp = mystrs[0];
                        int fk_node = int.Parse(mystrs[2]);
                        Node mynd = new Node();
                        mynd.NodeID = fk_node;
                        mynd.RetrieveFromDBSources();

                        string fk_flow = mynd.FlowNo;
                        string myurl = "./WorkOpt/OneWork/OneWork.htm?CurrTab=Track&FK_Node=" + mynd.NodeID + "&WorkID=" + myWorkID + "&FK_Flow=" + fk_flow;
                        Web.WebUser.SignInOfGener(new BP.Port.Emp(fk_emp));

                        return "url@" + myurl;
                    case "OF": //通过一个串来打开一个工作.
                        //sid 格式为: guid+"_"+workid+"_"+empNo+"_"+nodeID;
                        string[] strs = sid.Split('_');

                        string workID = strs[1];
                        string empNo = strs[2];
                        string u = "";

                        GenerWorkerList wl = new GenerWorkerList();
                        int i = wl.Retrieve(GenerWorkerListAttr.FK_Emp, empNo,
                            GenerWorkerListAttr.WorkID, workID,
                            GenerWorkerListAttr.IsPass, 0);

                        //如果已处理就打开在途查看
                        GenerWorkFlow gwf = new GenerWorkFlow(int.Parse(workID));
                        int gwfNum = gwf.Retrieve();
                        if (i == 0 && gwfNum != 0)
                        {
                            u = "MyViewGener.htm?WorkID=" + workID + "&FK_Flow=" + gwf.FlowNo + "&FK_Node=" + gwf.NodeID + "&FID=" + gwf.FID + "&PWorkID=" + gwf.PWorkID;
                            return "url@" + u;
                        }
                        else if (i == 0 && gwfNum == 0) {
                            return "info@你好，未查询到此工作流程数据或者已经被删除。";
                        }
                            

                     

                        //设置他的组织.
                        BP.Web.WebUser.OrgNo = gwf.OrgNo;

                        BP.Port.Emp empOF = new BP.Port.Emp(wl.EmpNo);
                        Web.WebUser.SignInOfGener(empOF);
                        u = "MyFlow.htm?FK_Flow=" + wl.FlowNo + "&WorkID=" + wl.WorkID + "&FK_Node=" + wl.NodeID + "&FID=" + wl.FID + "&PWorkID=" + gwf.PWorkID;

                        return "url@" + u;
                    case "ExitAuth":
                        BP.Port.Emp emp = new BP.Port.Emp(this.EmpNo);
                        //首先退出，再进行登录
                        BP.Web.WebUser.Exit();
                        BP.Web.WebUser.SignInOfGener(emp, WebUser.SysLang);
                        return "info@Close";

                    case "UnSend": //执行撤消发送。
                        string url = "./WorkOpt/UnSend.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FlowNo;
                        return "url@" + url;
                    case "SetBillState":
                        break;
                    case "WorkRpt":
                        break;
                    case "PrintBill":
                        break;
                    //删除流程中第一个节点的数据，包括待办工作
                    case "DeleteFlow":
                        //调用DoDeleteWorkFlowByReal方法
                        WorkFlow wf = new WorkFlow(new Flow(this.FlowNo), this.WorkID);
                        wf.DoDeleteWorkFlowByReal(true);
                        return "流程删除成功";
                        break;
                    case "DownFlowSearchExcel":    //下载流程查询结果，转到下面的逻辑，不放在此try..catch..中
                        break;
                    case "DownFlowSearchToTmpExcel":    //导出到模板
                        break;
                    default:
                        throw new Exception("没有判断的at标记:" + at);
                }
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            //此处之所以再加一个switch，是因为在下载文件逻辑中，调用Response.End()方法，如果此方法放在try..catch..中，会报线程中止异常
            switch (at)
            {
                case "DownFlowSearchExcel":
                    //  DownMyStartFlowExcel();
                    break;
                case "DownFlowSearchToTmpExcel":    //导出到模板
                    // DownMyStartFlowToTmpExcel();
                    break;
            }
            return "";
        }

        /// <summary>
        /// 获取设置的PC端和移动端URL
        /// </summary>
        /// <returns></returns>
        public string Do_PCAndMobileUrl()
        {
            Hashtable ht = new Hashtable();
            ht.Add("PCUrl", BP.Difference.SystemConfig.HostURL);
            ht.Add("MobileUrl", BP.Difference.SystemConfig.MobileURL);
            return BP.Tools.Json.ToJson(ht);
        }

        /// <summary>
        /// 页面调整移动端OR手机端
        /// </summary>
        /// <returns></returns>
        public string Do_Direct()
        {
            //获取地址
            string baseUrl = this.GetRequestVal("DirectUrl");
            ////判断是移动端还是PC端打开的页面
            //Regex RegexMobile = new Regex(@"(iemobile|iphone|ipod|android|nokia|sonyericsson|blackberry|samsung|sec\-|windows ce|motorola|mot\-|up.b|midp\-)",
            //RegexOptions.IgnoreCase | RegexOptions.Compiled);
            //移动端打开
            if (HttpContextHelper.RequestIsFromMobile)
                return BP.Difference.SystemConfig.MobileURL + baseUrl;
            else
                return BP.Difference.SystemConfig.HostURL + baseUrl;
        }

        #region 我的关注流程.
        /// <summary>
        /// 我的关注流程
        /// </summary>
        /// <returns></returns>
        public string Focus_Init()
        {
            string flowNo = this.GetRequestVal("FK_Flow");
            string domain = this.GetRequestVal("Domain");

            int idx = 0;
            //获得关注的数据.
            DataTable dt = BP.WF.Dev2Interface.DB_Focus(flowNo, BP.Web.WebUser.No, domain);
            SysEnums stas = new SysEnums("WFSta");
            string[] tempArr;
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                int wfsta = int.Parse(dr["WFSta"].ToString());
                //edit by liuxc,2016-10-22,修复状态显示不正确问题
                string wfstaT = (stas.GetEntityByKey(SysEnumAttr.IntKey, wfsta) as SysEnum).Lab;
                string currEmp = string.Empty;

                if (wfsta != (int)BP.WF.WFSta.Complete)
                {
                    //edit by liuxc,2016-10-24,未完成时，处理当前处理人，只显示处理人姓名
                    foreach (string emp in dr["ToDoEmps"].ToString().Split(';'))
                    {
                        tempArr = emp.Split(',');

                        currEmp += tempArr.Length > 1 ? tempArr[1] : tempArr[0] + ",";
                    }

                    currEmp = currEmp.TrimEnd(',');

                    //currEmp = dr["ToDoEmps"].ToString();
                    //currEmp = currEmp.TrimEnd(';');
                }
                dr["ToDoEmps"] = currEmp;
                dr["FlowNote"] = wfstaT;
                dr["AtPara"] = (wfsta == (int)BP.WF.WFSta.Complete ? dr["Sender"].ToString().TrimStart('(').TrimEnd(')').Split(',')[1] : "");
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 取消关注
        /// </summary>
        /// <returns></returns>
        public string Focus_Delete()
        {
            BP.WF.Dev2Interface.Flow_Focus(this.WorkID);
            return "执行成功";
        }
        #endregion 我的关注.

        /// <summary>
        /// 方法
        /// </summary>
        /// <returns></returns>
        public string HandlerMapExt()
        {
            WF_CCForm wf = new WF_CCForm();
            return wf.HandlerMapExt();
        }
        /// <summary>
        /// 最近发起的流程.
        /// </summary>
        /// <returns></returns>
        public string StartEaryer_Init()
        {
            //定义容器.
            DataSet ds = new DataSet();

            //获得能否发起的流程.
            string sql = "SELECT FK_Flow as No,FlowName as Name, FK_FlowSort,B.Name as FK_FlowSortText,B.Domain, COUNT(WorkID) as Num ";
            sql += " FROM WF_GenerWorkFlow A, WF_FlowSort B  ";
            sql += " WHERE Starter='" + BP.Web.WebUser.No + "'  AND A.FK_FlowSort=B.No  ";
            if (DataType.IsNullOrEmpty(this.FlowNo) == false)
                sql += " AND A.FK_Flow='" + this.FlowNo + "'";
            sql += " GROUP BY FK_Flow, FlowName, FK_FlowSort, B.Name,B.Domain ";

            if (SystemConfig.AppCenterDBType == DBType.DM)
            {
                sql = "SELECT FK_Flow as \"No\",FlowName as Name, FK_FlowSort,B.Name as FK_FlowSortText,B.\"Domain\", COUNT(WorkID) as Num ";
                sql += " FROM WF_GenerWorkFlow A, WF_FlowSort B  ";
                sql += " WHERE Starter='" + BP.Web.WebUser.No + "'  AND A.FK_FlowSort=B.No  ";
                if (DataType.IsNullOrEmpty(this.FlowNo) == false)
                    sql += " AND A.FK_Flow='" + this.FlowNo + "'";
                sql += " GROUP BY FK_Flow, FlowName, FK_FlowSort, B.Name,B.\"Domain\" ";
            }

            DataTable dtStart = DBAccess.RunSQLReturnTable(sql);
            dtStart.TableName = "Start";
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dtStart.Columns[0].ColumnName = "No";
                dtStart.Columns[1].ColumnName = "Name";
                dtStart.Columns[2].ColumnName = "FK_FlowSort";
                dtStart.Columns[3].ColumnName = "FK_FlowSortText";
                dtStart.Columns[4].ColumnName = "Domain";
                dtStart.Columns[5].ColumnName = "Num";
            }
            ds.Tables.Add(dtStart);

            DataTable dtSort = new DataTable("Sort");
            dtSort.Columns.Add("No", typeof(string));
            dtSort.Columns.Add("Name", typeof(string));
            dtSort.Columns.Add("Domain", typeof(string));

            string nos = "";
            foreach (DataRow dr in dtStart.Rows)
            {
                string no = dr["FK_FlowSort"].ToString();
                if (nos.Contains(no) == true)
                    continue;

                string name = dr["FK_FlowSortText"].ToString();
                string domain = dr["Domain"].ToString();

                nos += "," + no;

                DataRow mydr = dtSort.NewRow();
                mydr[0] = no;
                mydr[1] = name;
                mydr[2] = domain;
                dtSort.Rows.Add(mydr);
            }

            dtSort.TableName = "Sort";
            ds.Tables.Add(dtSort);

            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        /// <summary>
        /// 获得发起列表 
        /// </summary>
        /// <returns></returns>
        public string Start_Init()
        {
            string json = "";

            BP.WF.Port.WFEmp em = new WFEmp();
            string userNo = WebUser.No;
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                userNo = WebUser.OrgNo + "_" + WebUser.No;
            em.No = userNo;
            if (DataType.IsNullOrEmpty(em.No) == true)
                return "err@登录信息丢失,请重新登录.";

            if (em.RetrieveFromDBSources() == 0)
            {
                em.DeptNo = BP.Web.WebUser.DeptNo;
                em.Name = BP.Web.WebUser.Name;
                if(DataType.IsNullOrEmpty(BP.Web.WebUser.OrgNo) == false)
                {
                    em.OrgNo = BP.Web.WebUser.OrgNo;
                }
                em.Insert();
            }
            
            json = DBAccess.GetBigTextFromDB("WF_Emp", "No", userNo, "StartFlows");
            if (DataType.IsNullOrEmpty(json) == false)
                return json;

            //定义容器.
            DataSet ds = new DataSet();

            //获得能否发起的流程.
            DataTable dtStart = Dev2Interface.DB_StarFlows(userNo);
            dtStart.TableName = "Start";
            ds.Tables.Add(dtStart);

            #region 动态构造 流程类别.
            DataTable dtSort = new DataTable("Sort");
            dtSort.Columns.Add("No", typeof(string));
            dtSort.Columns.Add("Name", typeof(string));
            dtSort.Columns.Add("Domain", typeof(string));

            string nos = "";
            foreach (DataRow dr in dtStart.Rows)
            {
                string no = dr["FK_FlowSort"].ToString();
                if (nos.Contains(no) == true)
                    continue;

                string name = dr["FK_FlowSortText"].ToString();
                string domain = dr["Domain"].ToString();

                nos += "," + no;

                DataRow mydr = dtSort.NewRow();
                mydr[0] = no;
                mydr[1] = name;
                mydr[2] = domain;
                dtSort.Rows.Add(mydr);
            }

            dtSort.TableName = "Sort";
            ds.Tables.Add(dtSort);
            #endregion 动态构造 流程类别.

            //返回组合
            json = BP.Tools.Json.DataSetToJson(ds, false);

            //把json存入数据表，避免下一次再取.
            if (dtStart.Rows.Count > 0)
                DBAccess.SaveBigTextToDB(json, "WF_Emp", "No", userNo, "StartFlows");

            //返回组合
            return json;
        }
        /// <summary>
        /// 获得发起列表
        /// </summary>
        /// <returns></returns>
        public string FlowSearch_Init()
        {
            DataSet ds = new DataSet();

            //流程类别.
            FlowSorts fss = new FlowSorts();
            fss.RetrieveAll();

            DataTable dtSort = fss.ToDataTableField("Sort");
            dtSort.TableName = "Sort";
            ds.Tables.Add(dtSort);

            //获得能否发起的流程.
            DataTable dtStart = DBAccess.RunSQLReturnTable("SELECT No,Name, FK_FlowSort FROM WF_Flow ORDER BY FK_FlowSort,Idx");
            dtStart.TableName = "Start";

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dtStart.Columns["NO"].ColumnName = "No";
                dtStart.Columns["NAME"].ColumnName = "Name";
                dtStart.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
            }

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dtStart.Columns["no"].ColumnName = "No";
                dtStart.Columns["name"].ColumnName = "Name";
                dtStart.Columns["fk_flowsort"].ColumnName = "FK_FlowSort";
            }

            ds.Tables.Add(dtStart);



            //返回组合
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        #region 获得列表.
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="UserNo">人员编号</param>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>运行中的流程</returns>
        public string Runing_Init()
        {
            DataTable dt = null;
            bool isContainFuture = this.GetRequestValBoolen("IsContainFuture");
            dt = BP.WF.Dev2Interface.DB_GenerRuning(WebUser.No, this.FlowNo, false, this.Domain, isContainFuture); //获得指定域的在途.
            return BP.Tools.Json.ToJson(dt);
        }
        //近期工作
        public string RecentWork_Init()
        {
            /* 近期工作. */
            string sql = "";
            string empNo = BP.Web.WebUser.No;
            sql += "SELECT  * FROM WF_GenerWorkFlow  WHERE ";
            sql += " (Emps LIKE '%@" + empNo + "@%' OR Emps LIKE '%@" + empNo + ",%' OR Emps LIKE '%," + empNo + "@%')";
            // sql += " AND Starter!='" + empNo + "'"; //不能是我发起的.

            if (DataType.IsNullOrEmpty(this.FlowNo) == false)
                sql += " AND FK_Flow='" + this.FlowNo + "'"; //如果有流程编号,就按他过滤.

            sql += " AND WFState >1 ORDER BY RDT DESC ";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            //添加oracle的处理
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["PRI"].ColumnName = "PRI";
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["FID"].ColumnName = "FID";
                dt.Columns["WFSTATE"].ColumnName = "WFState";
                dt.Columns["WFSTA"].ColumnName = "WFSta";
                dt.Columns["WEEKNUM"].ColumnName = "WeekNum";
                dt.Columns["TSPAN"].ColumnName = "TSpan";
                dt.Columns["TODOSTA"].ColumnName = "TodoSta";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["TODOEMPSNUM"].ColumnName = "TodoEmpsNum";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["TASKSTA"].ColumnName = "TaskSta";
                dt.Columns["SYSTYPE"].ColumnName = "SysType";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["SENDDT"].ColumnName = "SendDT";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["SDTOFFLOW"].ColumnName = "SDTOfFlow";
                dt.Columns["RDT"].ColumnName = "RDT";
                dt.Columns["PWORKID"].ColumnName = "PWorkID";
                dt.Columns["PFLOWNO"].ColumnName = "PFlowNo";
                dt.Columns["PFID"].ColumnName = "PFID";
                dt.Columns["PEMP"].ColumnName = "PEmp";
                dt.Columns["NODENAME"].ColumnName = "NodeName";

                dt.Columns["GUID"].ColumnName = "Guid";
                dt.Columns["GUESTNO"].ColumnName = "GuestNo";
                dt.Columns["GUESTNAME"].ColumnName = "GuestName";
                dt.Columns["FLOWNOTE"].ColumnName = "FlowNote";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["FK_NY"].ColumnName = "FK_NY";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FK_DEPT"].ColumnName = "FK_Dept";
                dt.Columns["EMPS"].ColumnName = "Emps";
                dt.Columns["DOMAIN"].ColumnName = "Domain";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["BILLNO"].ColumnName = "BillNo";
            }

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["pri"].ColumnName = "PRI";
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["fid"].ColumnName = "FID";
                dt.Columns["wfstate"].ColumnName = "WFState";
                dt.Columns["wfsta"].ColumnName = "WFSta";
                dt.Columns["weeknum"].ColumnName = "WeekNum";
                dt.Columns["tspan"].ColumnName = "TSpan";
                dt.Columns["todosta"].ColumnName = "TodoSta";
                dt.Columns["deptname"].ColumnName = "DeptName";
                dt.Columns["todoempsnum"].ColumnName = "TodoEmpsNum";
                dt.Columns["todoemps"].ColumnName = "TodoEmps";
                dt.Columns["title"].ColumnName = "Title";
                dt.Columns["tasksta"].ColumnName = "TaskSta";
                dt.Columns["systype"].ColumnName = "SysType";
                dt.Columns["startername"].ColumnName = "StarterName";
                dt.Columns["starter"].ColumnName = "Starter";
                dt.Columns["sender"].ColumnName = "Sender";
                dt.Columns["senddt"].ColumnName = "SendDT";
                dt.Columns["sdtofnode"].ColumnName = "SDTOfNode";
                dt.Columns["sdtofflow"].ColumnName = "SDTOfFlow";
                dt.Columns["rdt"].ColumnName = "RDT";
                dt.Columns["pworkid"].ColumnName = "PWorkID";
                dt.Columns["pflowno"].ColumnName = "PFlowNo";
                dt.Columns["pfid"].ColumnName = "PFID";
                dt.Columns["pemp"].ColumnName = "PEmp";
                dt.Columns["nodename"].ColumnName = "NodeName";
                dt.Columns["guid"].ColumnName = "Guid";
                dt.Columns["guestno"].ColumnName = "GuestNo";
                dt.Columns["guestname"].ColumnName = "GuestName";
                dt.Columns["flownote"].ColumnName = "FlowNote";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["fk_ny"].ColumnName = "FK_NY";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
                dt.Columns["fk_flowsort"].ColumnName = "FK_FlowSort";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["fk_dept"].ColumnName = "FK_Dept";
                dt.Columns["emps"].ColumnName = "Emps";
                dt.Columns["domain"].ColumnName = "Domain";
                dt.Columns["deptname"].ColumnName = "DeptName";
                dt.Columns["billno"].ColumnName = "BillNo";
            }
            return BP.Tools.Json.ToJson(dt);
        }

        /// <summary>
        /// 我参与的已经完成的工作.
        /// </summary>
        /// <returns></returns>
        public string Complete_Init()
        {
            /* 如果不是删除流程注册表. */
            Paras ps = new Paras();
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            ps.SQL = "SELECT  * FROM WF_GenerWorkFlow  WHERE (Emps LIKE '%@" + WebUser.No + "@%' OR Emps LIKE '%@" + WebUser.No + ",%' OR Emps LIKE '%," + WebUser.No + "@%') and WFState=" + (int)WFState.Complete + " ORDER BY  RDT DESC";
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            //添加oracle的处理
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["PRI"].ColumnName = "PRI";
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["FID"].ColumnName = "FID";
                dt.Columns["WFSTATE"].ColumnName = "WFState";
                dt.Columns["WFSTA"].ColumnName = "WFSta";
                dt.Columns["WEEKNUM"].ColumnName = "WeekNum";
                dt.Columns["TSPAN"].ColumnName = "TSpan";
                dt.Columns["TODOSTA"].ColumnName = "TodoSta";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["TODOEMPSNUM"].ColumnName = "TodoEmpsNum";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["TASKSTA"].ColumnName = "TaskSta";
                dt.Columns["SYSTYPE"].ColumnName = "SysType";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["SENDDT"].ColumnName = "SendDT";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["SDTOFFLOW"].ColumnName = "SDTOfFlow";
                dt.Columns["RDT"].ColumnName = "RDT";
                dt.Columns["PWORKID"].ColumnName = "PWorkID";
                dt.Columns["PFLOWNO"].ColumnName = "PFlowNo";
                dt.Columns["PFID"].ColumnName = "PFID";
                dt.Columns["PEMP"].ColumnName = "PEmp";
                dt.Columns["NODENAME"].ColumnName = "NodeName";


                dt.Columns["GUID"].ColumnName = "Guid";
                dt.Columns["GUESTNO"].ColumnName = "GuestNo";
                dt.Columns["GUESTNAME"].ColumnName = "GuestName";
                dt.Columns["FLOWNOTE"].ColumnName = "FlowNote";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["FK_NY"].ColumnName = "FK_NY";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FK_DEPT"].ColumnName = "FK_Dept";
                dt.Columns["EMPS"].ColumnName = "Emps";
                dt.Columns["DOMAIN"].ColumnName = "Domain";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["BILLNO"].ColumnName = "BillNo";
                dt.Columns["ATPARA"].ColumnName = "AtPara";
                dt.Columns["HUNGUPTIME"].ColumnName = "HungupTime";
                dt.Columns["ORGNO"].ColumnName = "OrgNo";
                dt.Columns["PNODEID"].ColumnName = "PNodeID";
                dt.Columns["PRJNAME"].ColumnName = "PrjName";
                dt.Columns["PRJNO"].ColumnName = "PrjNo";
                dt.Columns["SDTOFFLOWWARNING"].ColumnName = "SDTOfFlowWarning";
            }

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["pri"].ColumnName = "PRI";
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["fid"].ColumnName = "FID";
                dt.Columns["wfstate"].ColumnName = "WFState";
                dt.Columns["wfsta"].ColumnName = "WFSta";
                dt.Columns["weeknum"].ColumnName = "WeekNum";
                dt.Columns["tspan"].ColumnName = "TSpan";
                dt.Columns["todosta"].ColumnName = "TodoSta";
                dt.Columns["deptname"].ColumnName = "DeptName";
                dt.Columns["todoempsnum"].ColumnName = "TodoEmpsNum";
                dt.Columns["todoemps"].ColumnName = "TodoEmps";
                dt.Columns["title"].ColumnName = "Title";
                dt.Columns["tasksta"].ColumnName = "TaskSta";
                dt.Columns["systype"].ColumnName = "SysType";
                dt.Columns["startername"].ColumnName = "StarterName";
                dt.Columns["starter"].ColumnName = "Starter";
                dt.Columns["sender"].ColumnName = "Sender";
                dt.Columns["senddt"].ColumnName = "SendDT";
                dt.Columns["sdtofnode"].ColumnName = "SDTOfNode";
                dt.Columns["sdtofflow"].ColumnName = "SDTOfFlow";
                dt.Columns["rdt"].ColumnName = "RDT";
                dt.Columns["pworkid"].ColumnName = "PWorkID";
                dt.Columns["pflowno"].ColumnName = "PFlowNo";
                dt.Columns["pfid"].ColumnName = "PFID";
                dt.Columns["pemp"].ColumnName = "PEmp";
                dt.Columns["nodename"].ColumnName = "NodeName";
                dt.Columns["guid"].ColumnName = "Guid";
                dt.Columns["guestno"].ColumnName = "GuestNo";
                dt.Columns["guestname"].ColumnName = "GuestName";
                dt.Columns["flownote"].ColumnName = "FlowNote";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["fk_ny"].ColumnName = "FK_NY";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
                dt.Columns["fk_flowsort"].ColumnName = "FK_FlowSort";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["fk_dept"].ColumnName = "FK_Dept";
                dt.Columns["emps"].ColumnName = "Emps";
                dt.Columns["domain"].ColumnName = "Domain";
                dt.Columns["deptname"].ColumnName = "DeptName";
                dt.Columns["billno"].ColumnName = "BillNo";
                dt.Columns["atpara"].ColumnName = "AtPara";
                dt.Columns["hunguptime"].ColumnName = "HungupTime";
                dt.Columns["orgno"].ColumnName = "OrgNo";
                dt.Columns["pnodeid"].ColumnName = "PNodeID";
                dt.Columns["prjname"].ColumnName = "PrjName";
                dt.Columns["prjno"].ColumnName = "PrjNo";
                dt.Columns["sdtofflowwarning"].ColumnName = "SDTOfFlowWarning";
            }

            return BP.Tools.Json.ToJson(dt);
        }

        /// <summary>
        /// 执行撤销
        /// </summary>
        /// <returns></returns>
        public string Runing_UnSend()
        {
            try
            {
                //获取撤销到的节点
                int unSendToNode = this.GetRequestValInt("UnSendToNode");
                return BP.WF.Dev2Interface.Flow_DoUnSend(this.FlowNo, this.WorkID, unSendToNode, this.FID);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 撤销发送
        /// </summary>
        /// <returns></returns>
        public string Runing_UnSendCC()
        {
            string checkboxs = GetRequestVal("CCPKs");
            CCLists ccs = new CCLists();
            ccs.RetrieveIn("MyPK", "'" + checkboxs.Replace(",", "','") + "'");
            ccs.Delete();
            return "撤销抄送成功";
        }
        /// <summary>
        /// 执行催办
        /// </summary>
        /// <returns></returns>
        public string Runing_Press()
        {
            try
            {
                return BP.WF.Dev2Interface.Flow_DoPress(this.WorkID, this.GetRequestVal("Msg"), false);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 打开表单
        /// </summary>
        /// <returns></returns>
        public string Runing_OpenFrm()
        {
            int nodeID = this.NodeID;
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (nodeID == 0)
            {
                gwf = new GenerWorkFlow(this.WorkID);
                nodeID = gwf.NodeID;
            }

            string appPath = BP.WF.Glo.CCFlowAppPath;
            Node nd = null;
            Track tk = new Track();

            tk.WorkID = this.WorkID;
            if (this.MyPK != null)
            {
                tk = new Track(this.FlowNo, this.MyPK);
                nd = new Node(tk.NDFrom);
            }
            else
            {
                nd = new Node(nodeID);
            }

            Flow fl = new Flow(this.FlowNo);
            Int64 workid = 0;
            if (nd.ItIsSubThread == true)
            {
                if (tk.FID == 0)
                {
                    if (gwf == null)
                    {
                        gwf = new GenerWorkFlow(this.WorkID);
                    }

                    workid = gwf.FID;
                }
                else
                {
                    workid = tk.FID;
                }
            }
            else
            {
                workid = tk.WorkID;
            }

            Int64 fid = this.FID;
            if (this.FID == 0)
            {
                fid = tk.FID;
            }

            if (fid > 0)
            {
                workid = fid;
            }

            if (workid == 0)
            {
                workid = this.WorkID;
            }

            string urlExt = "";

            // gwf.atPara.HisHT

            DataTable ndrpt = DBAccess.RunSQLReturnTable("SELECT PFlowNo,PWorkID FROM " + fl.PTable + " WHERE OID=" + workid);
            if (ndrpt.Rows.Count == 0)
            {
                urlExt = "&PFlowNo=0&PWorkID=0&IsToobar=0&IsHidden=true&CCSta=" + this.GetRequestValInt("CCSta");
            }
            else
            {
                urlExt = "&PFlowNo=" + ndrpt.Rows[0]["PFlowNo"] + "&PWorkID=" + ndrpt.Rows[0]["PWorkID"] + "&IsToobar=0&IsHidden=true&CCSta=" + this.GetRequestValInt("CCSta");
            }

            urlExt += "&From=CCFlow&TruckKey=" + tk.GetValStrByKey("MyPK") + "&DoType=" + this.DoType + "&UserNo=" + WebUser.No ?? string.Empty + "&Token=" + WebUser.Token ?? string.Empty;

            urlExt = urlExt.Replace("PFlowNo=null", "");
            urlExt = urlExt.Replace("PWorkID=null", "");

            if (gwf.atPara.HisHT.Count > 0)
            {
                foreach (string item in gwf.atPara.HisHT.Keys)
                {
                    urlExt += "&" + item + "=" + gwf.atPara.HisHT[item];
                }
            }


            if (nd.HisFormType == NodeFormType.SDKForm || nd.HisFormType == NodeFormType.SelfForm)
            {
                //added by liuxc,2016-01-25
                if (nd.FormUrl.Contains("?"))
                    return "urlForm@" + nd.FormUrl + "&IsReadonly=1&WorkID=" + workid + "&FK_Node=" + nd.NodeID + "&FK_Flow=" + nd.FlowNo + "&FID=" + fid + urlExt;

                return "urlForm@" + nd.FormUrl + "?IsReadonly=1&WorkID=" + workid + "&FK_Node=" + nd.NodeID + "&FK_Flow=" + nd.FlowNo + "&FID=" + fid + urlExt;
            }

            if (nd.HisFormType == NodeFormType.SheetTree || nd.HisFormType == NodeFormType.SheetAutoTree)
            {
                if (Glo.Platform == Platform.CCFlow)
                    return "url@/WF/MyViewTree.htm?3=4&WorkID=" + this.WorkID + "&FID=" + this.FID + "&OID=" + this.WorkID + "&FK_Flow=" + this.FlowNo + "&FK_Node=" + nd.NodeID + "&PK=OID&PKVal=" + this.WorkID + "&IsEdit=0&IsLoadData=0&IsReadonly=1" + urlExt;
                else
                    return "url@/jflow-web/MyViewTree.htm?3=4&WorkID=" + this.WorkID + "&FID=" + this.FID + "&OID=" + this.WorkID + "&FK_Flow=" + this.FlowNo + "&FK_Node=" + nd.NodeID + "&PK=OID&PKVal=" + this.WorkID + "&IsEdit=0&IsLoadData=0&IsReadonly=1" + urlExt;
            }

            Work wk = nd.HisWork;
            wk.OID = workid;
            if (wk.RetrieveFromDBSources() == 0)
            {
                GERpt rtp = nd.HisFlow.HisGERpt;
                rtp.OID = workid;
                if (rtp.RetrieveFromDBSources() == 0)
                {
                    string info = "打开(" + nd.Name + ")错误";
                    info += "当前的节点数据已经被删除！！！<br> 造成此问题出现的原因如下。";
                    info += "1、当前节点数据被非法删除。";
                    info += "2、节点数据是退回人与被退回人中间的节点，这部分节点数据查看不支持。";
                    info += "技术信息:表" + wk.EnMap.PhysicsTable + " WorkID=" + workid;
                    return "err@" + info;
                }
                wk.Row = rtp.Row;
            }

            if (nd.HisFlow.ItIsMD5 && wk.ItIsPassCheckMD5() == false)
            {
                string err = "打开(" + nd.Name + ")错误";
                err += "当前的节点数据已经被篡改，请报告管理员。";
                return "err@" + err;
            }
            nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.
            if (nd.HisFormType == NodeFormType.Develop)
            {
                MapData md = new MapData(nd.NodeFrmID);
                if (md.HisFrmType != FrmType.Develop)
                {
                    md.HisFrmType = FrmType.Develop;
                    md.Update();
                }
            }
            else if (nd.HisFormType == NodeFormType.FoolForm)
            {
                nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.
                MapData md = new MapData(nd.NodeFrmID);
                if (md.HisFrmType != FrmType.FoolForm)
                {
                    md.HisFrmType = FrmType.FoolForm;
                    md.Update();
                }
            }
            string endUrl = "";

            if (gwf.atPara.HisHT.Count > 0)
            {
                foreach (string item in gwf.atPara.HisHT.Keys)
                {
                    endUrl += "&" + item + "=" + gwf.atPara.HisHT[item];
                }
            }

            //加入是累加表单的标志，目的是让附件可以看到.

            if (nd.HisFormType == NodeFormType.FoolTruck)
            {
                endUrl = "&FormType=10&FromWorkOpt=" + this.GetRequestVal("FromWorkOpt");
            }

            //return "url@./CCForm/Frm.htm?FK_MapData=" + nd.NodeFrmID + "&OID=" + wk.OID + "&FK_Flow=" + this.FlowNo + "&FK_Node=" + nd.NodeID + "&PK=OID&PKVal=" + wk.OID + "&IsEdit=0&IsLoadData=0&IsReadonly=1" + endUrl+"&CCSta="+this.GetRequestValInt("CCSta");

            if (Glo.Platform == Platform.CCFlow)
                return "url@/WF/MyView.htm?FK_MapData=" + nd.NodeFrmID + "&OID=" + wk.OID + "&FK_Flow=" + this.FlowNo + "&FK_Node=" + nd.NodeID + "&PK=OID&PKVal=" + wk.OID + "&IsEdit=0&IsLoadData=0&IsReadonly=1" + endUrl + "&CCSta=" + this.GetRequestValInt("CCSta");
            else
                return "url@/jflow-web/WF/MyView.htm?FK_MapData=" + nd.NodeFrmID + "&OID=" + wk.OID + "&FK_Flow=" + this.FlowNo + "&FK_Node=" + nd.NodeID + "&PK=OID&PKVal=" + wk.OID + "&IsEdit=0&IsLoadData=0&IsReadonly=1" + endUrl + "&CCSta=" + this.GetRequestValInt("CCSta");
        }
        /// <summary>
        /// 草稿
        /// </summary>
        /// <returns></returns>
        public string Draft_Init()
        {
            DataTable dt = null;
            string domain = this.GetRequestVal("Domain");
            string flowNo = this.GetRequestVal("FK_Flow");

            dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable(flowNo, domain);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 删除草稿.
        /// </summary>
        /// <returns></returns>
        public string Draft_Delete()
        {
            return BP.WF.Dev2Interface.Flow_DoDeleteDraft(this.FlowNo, this.WorkID, false);
        }
        /// <summary>
        /// 获得会签列表
        /// </summary>
        /// <returns></returns>
        public string HuiQianList_Init()
        {
            string sql = "SELECT A.WorkID, A.Title,A.FK_Flow, A.FlowName, A.Starter, A.StarterName, A.Sender, A.Sender,A.FK_Node,A.NodeName,A.SDTOfNode,A.TodoEmps";
            sql += " FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B ";
            sql += " WHERE A.WorkID=B.WorkID and a.FK_Node=b.FK_Node ";
            sql += " AND (B.IsPass=90 OR A.AtPara LIKE '%HuiQianZhuChiRen=" + WebUser.No + "%') ";
            sql += " AND B.FK_Emp=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Emp";

            Paras ps = new Paras();
            ps.Add("FK_Emp", WebUser.No);
            ps.SQL = sql;
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";

                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";

                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["title"].ColumnName = "Title";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["flowname"].ColumnName = "FlowName";

                dt.Columns["starter"].ColumnName = "Starter";
                dt.Columns["startername"].ColumnName = "StarterName";

                dt.Columns["sender"].ColumnName = "Sender";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
                dt.Columns["nodename"].ColumnName = "NodeName";
                dt.Columns["sdtofnode"].ColumnName = "SDTOfNode";
                dt.Columns["todoemps"].ColumnName = "TodoEmps";
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 协作模式待办
        /// </summary>
        /// <returns></returns>
        public string TeamupList_Init()
        {
            string sql = "SELECT A.WorkID, A.Title,A.FK_Flow, A.FlowName, A.Starter, A.StarterName, A.Sender, A.Sender,A.FK_Node,A.NodeName,A.SDTOfNode,A.TodoEmps";
            sql += " FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B, WF_Node C ";
            sql += " WHERE A.WorkID=B.WorkID and a.FK_Node=b.FK_Node AND A.FK_Node=C.NodeID AND C.TodolistModel=1 ";
            sql += " AND B.IsPass=0 AND B.FK_Emp=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Emp";
            //   sql += " AND B.IsPass=0 AND B.FK_Emp=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Emp";

            Paras ps = new Paras();
            ps.Add("FK_Emp", WebUser.No);
            ps.SQL = sql;
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";

                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";

                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["title"].ColumnName = "Title";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["flowname"].ColumnName = "FlowName";

                dt.Columns["starter"].ColumnName = "Starter";
                dt.Columns["startername"].ColumnName = "StarterName";

                dt.Columns["sender"].ColumnName = "Sender";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
                dt.Columns["nodename"].ColumnName = "NodeName";
                dt.Columns["sdtofnode"].ColumnName = "SDTOfNode";
                dt.Columns["todoemps"].ColumnName = "TodoEmps";
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获得加签人的待办
        /// @LQ 
        /// </summary>
        /// <returns></returns>
        public string HuiQianAdderList_Init()
        {
            string sql = "SELECT A.WorkID, A.Title,A.FK_Flow, A.FlowName, A.Starter, A.StarterName, A.Sender, A.Sender,A.FK_Node,A.NodeName,A.SDTOfNode,A.TodoEmps";
            sql += " FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B, WF_Node C ";
            sql += " WHERE A.WorkID=B.WorkID and a.FK_Node=b.FK_Node AND B.IsPass=0 AND B.FK_Emp=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Emp";
            sql += " AND B.AtPara LIKE '%IsHuiQian=1%' ";
            sql += " AND A.FK_Node=C.NodeID ";
            sql += " AND C.TodolistModel= 4";

            Paras ps = new Paras();
            ps.Add("FK_Emp", WebUser.No);
            ps.SQL = sql;
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";

                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";

                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
            }

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["title"].ColumnName = "Title";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["flowname"].ColumnName = "FlowName";

                dt.Columns["starter"].ColumnName = "Starter";
                dt.Columns["startername"].ColumnName = "StarterName";

                dt.Columns["sender"].ColumnName = "Sender";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
                dt.Columns["nodename"].ColumnName = "NodeName";
                dt.Columns["sdtofnode"].ColumnName = "SDTOfNode";
                dt.Columns["todoemps"].ColumnName = "TodoEmps";
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 初始化待办.
        /// </summary>
        /// <returns></returns>
        public string Todolist_Init()
        {
            string wfState = this.GetRequestVal("ShowWhat"); //比如：WFSTate=1,状态.
            string orderBy = this.GetRequestVal("OrderBy");
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(WebUser.No, this.NodeID,
                wfState, this.Domain, this.FlowNo, orderBy);
            return BP.Tools.Json.ToJson(dt);
        }
        public string Todolist_ABC()
        {
            // BP.WF.Dev2Interface.
            return "xxxx";
        }
        /// <summary>
        /// 逾期工作
        /// </summary>
        /// <returns></returns>
        public string Timeout_Init()
        {
            //string wfState = this.GetRequestVal("ShowWhat"); //比如：WFSTate=1,状态.
            //string orderBy = this.GetRequestVal("OrderBy");
            DataTable dt = BP.WF.Dev2Interface.DB_Timeout();
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 近期发起
        /// </summary>
        /// <returns></returns>
        public string RecentStart_Init()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_RecentStart(this.FlowNo);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获得授权人的待办.
        /// </summary>
        /// <returns></returns>
        public string Todolist_Author()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(this.No, this.NodeID);

            //转化大写的toJson.
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string TodolistOfAuth_Init()
        {
            return "err@尚未重构完成.";
        }
        /// <summary>
        /// 获得挂起列表
        /// </summary>
        /// <returns></returns>
        public string HungupList_Init()
        {
            return BP.WF.Dev2Interface.DB_GenerHungupList();
        }
        /// <summary>
        /// 同意挂起
        /// </summary>
        /// <returns></returns>
        public string HungupList_Agree()
        {
            return BP.WF.Dev2Interface.Node_HungupWorkAgree(this.WorkID);
        }
        /// <summary>
        /// 拒绝挂起
        /// </summary>
        /// <returns></returns>
        public string HungupList_Reject()
        {
            return BP.WF.Dev2Interface.Node_HungupWorkReject(this.WorkID, this.GetRequestVal("Msg"));
        }

        public string FutureTodolist_Init()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_FutureTodolist();

            //转化大写的toJson.
            return BP.Tools.Json.ToJson(dt);
        }


        #endregion 获得列表.


        #region 共享任务池.
        /// <summary>
        /// 初始化共享任务
        /// </summary>
        /// <returns></returns>
        public string TaskPoolSharing_Init()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_TaskPool();

            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 申请任务.
        /// </summary>
        /// <returns></returns>
        public string TaskPoolSharing_Apply()
        {
            bool b = BP.WF.Dev2Interface.Node_TaskPoolTakebackOne(this.WorkID);
            if (b == true)
            {
                return "申请成功.";
            }
            else
            {
                return "err@申请失败...";
            }
        }
        /// <summary>
        /// 我申请下来的任务
        /// </summary>
        /// <returns></returns>
        public string TaskPoolApply_Init()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_TaskPoolOfMyApply();

            return BP.Tools.Json.ToJson(dt);
        }
        public string TaskPoolApply_PutOne()
        {
            BP.WF.Dev2Interface.Node_TaskPoolPutOne(this.WorkID);
            return "放入成功,其他的同事可以看到这件工作.您可以在任务池里看到它并重新申请下来.";
        }
        #endregion

        #region 登录相关.
        /// <summary>
        /// 返回当前会话信息.
        /// 1. token 用于ccflow内部访问.
        /// 2. sid 用于集成.
        /// </summary>
        /// <returns></returns>
        public string Login_Init()
        {
            #region 检查一下是否有 token、sid ? 如果有就直接登录.
            string token = this.GetRequestVal("Token");
            if (DataType.IsNullOrEmpty(token) == false)
            {
                BP.WF.Dev2Interface.Port_LoginByToken(token);
                return "url@Home.htm?Token=" + token;
            }
            #endregion 检查一下是否有token ?

            Hashtable ht = new Hashtable();
            if (BP.Web.WebUser.NoOfRel == null)
            {
                ht.Add("UserNo", "");
            }
            else
            {
                ht.Add("UserNo", BP.Web.WebUser.No);
            }
            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 执行登录.
        /// </summary>
        /// <returns></returns>
        public string LoginSubmit()
        {
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.UserID = this.GetValFromFrmByKey("TB_No");

            if (emp.RetrieveFromDBSources() == 0)
                return "err@用户名或密码错误.";

            string pass = this.GetValFromFrmByKey("TB_PW");
            if (emp.Pass.Equals(pass) == false)
                return "err@用户名或密码错误.";

            //让其登录.
            BP.WF.Dev2Interface.Port_Login(emp.UserID);

            return "登录成功.";
        }
        /// <summary>
        /// 执行授权登录
        /// </summary>
        /// <returns></returns>
        public string AuthorList_LoginAs()
        {
            BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp(this.No);

            //if (wfemp.AuthorIsOK == false)
            //   return "err@授权登录失败！";

            BP.Port.Emp emp1 = new BP.Port.Emp(this.No);
            BP.Web.WebUser.SignInOfGener(emp1, "CH", false, false, BP.Web.WebUser.No, BP.Web.WebUser.Name);

            return "授权登录成功！";
        }
        /// <summary>
        /// 批处理审批
        /// </summary>
        /// <returns></returns>
        public string Batch_Init()
        {
            string sql = "SELECT a.NodeID, a.Name,a.FlowName, a.BatchRole, COUNT(WorkID) AS Num  FROM  WF_Node a, WF_EmpWorks b ";
            sql += " WHERE A.NodeID=b.FK_Node AND B.FK_Emp='" + WebUser.No + "' AND a.BatchRole!=0 ";
            sql += " AND b.WFState!=7 GROUP BY A.NodeID, a.Name,a.FlowName,a.BatchRole ";
            //sql += " AND b.WFState NOT IN (7) AND a.BatchRole!=0 GROUP BY A.NodeID, a.Name,a.FlowName,a.BatchRole ";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dt.Columns[0].ColumnName = "NodeID";
                dt.Columns[1].ColumnName = "Name";
                dt.Columns[2].ColumnName = "FlowName";
                dt.Columns[3].ColumnName = "BatchRole";
                dt.Columns[4].ColumnName = "Num";
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 授权列表
        /// </summary>
        /// <returns></returns>
        public string AuthorTodolist_Init()
        {

            return "";
        }

        /// <summary>
        /// 授权列表
        /// </summary>
        /// <returns></returns>
        public string AuthorTodolist_Todolist()
        {

            return "";
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="UserNo"></param>
        /// <param name="Author"></param>
        /// <returns></returns>
        public string AuthExitAndLogin(string UserNo, string Author)
        {
            string msg = "suess@退出成功！";
            try
            {
                BP.Port.Emp emp = new BP.Port.Emp(UserNo);
                //首先退出
                BP.Web.WebUser.Exit();
                //再进行登录
                BP.Port.Emp emp1 = new BP.Port.Emp(Author);
                BP.Web.WebUser.SignInOfGener(emp1, "CH", false, false, null, null);
            }
            catch (Exception ex)
            {
                msg = "err@退出时发生错误:" + ex.Message;
            }
            return msg;
        }
        /// <summary>
        /// 获取授权人列表
        /// </summary>
        /// <returns></returns>
        public string AuthorList_Init()
        {
            try
            {
                Auths ens = new Auths();
                ens.Retrieve(AuthAttr.AutherToEmpNo, WebUser.No);
                return ens.ToJson();

                //Paras ps = new Paras();
                //ps.SQL = "SELECT No,Name,AuthorDate FROM WF_Emp WHERE Author=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Author";
                //ps.Add("Author", BP.Web.WebUser.No);
                //DataTable dt = DBAccess.RunSQLReturnTable(ps);

                //if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
                //{
                //    dt.Columns["NO"].ColumnName = "No";
                //    dt.Columns["NAME"].ColumnName = "Name";
                //    dt.Columns["AUTHORDATE"].ColumnName = "AuthorDate";
                //}
                //if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
                //{
                //    dt.Columns["no"].ColumnName = "No";
                //    dt.Columns["name"].ColumnName = "Name";
                //    dt.Columns["authordate"].ColumnName = "AuthorDate";
                //}
                //return BP.Tools.Json.ToJson(dt);
            }
            catch (Exception ex)
            {
                WFEmp en = new WFEmp();
                en.CheckPhysicsTable();
                throw new Exception("err@系统异常，请在执行一次:" + ex.Message);
            }
        }

        public string AuthorListMy_Init()
        {
		    try

            {
                    Auths ens = new Auths();
                    ens.Retrieve(AuthAttr.Auther, WebUser.No, null);
                    return ens.ToJson("dt");
            }
		    catch (Exception ex)
		    {
			    WFEmp en = new WFEmp();
            en.CheckPhysicsTable();
			    throw new Exception("err@系统异常，请在执行一次:" + ex.Message);
            }
        }
/// <summary>
/// 当前登陆人是否有授权
/// </summary>
/// <returns></returns>
public string IsHaveAuthor()
        {
            Auths ens = new Auths();
            ens.Retrieve(AuthAttr.Auther, BP.Web.WebUser.No);
            if (ens.Count > 0)
                return "suess@有授权";
            return "err@没有授权";
        }
        /// <summary>
        /// 退出.
        /// </summary>
        /// <returns></returns>
        public string LoginExit()
        {
            BP.WF.Dev2Interface.Port_SigOut();
            return null;
        }
        /// <summary>
        /// 授权退出.
        /// </summary>
        /// <returns></returns>
        public string AuthExit()
        {
            return this.AuthExitAndLogin(this.No, BP.Web.WebUser.Auth);
        }
        #endregion 登录相关.

        /// <summary>
        /// 获得抄送列表
        /// </summary>
        /// <returns></returns>
        public string CC_Init()
        {
            string sta = this.GetRequestVal("Sta");
            if (sta == null || sta == "")
                sta = "-1";

            int pageSize = 6; // int.Parse(pageSizeStr);

            string pageIdxStr = this.GetRequestVal("PageIdx");
            if (pageIdxStr == null)
                pageIdxStr = "1";

            int pageIdx = int.Parse(pageIdxStr);

            //实体查询.
            //BP.WF.SMSs ss = new BP.WF.SMSs();
            //BP.En.QueryObject qo = new BP.En.QueryObject(ss);

            DataTable dt = null;
            if (sta.Equals("-1"))
                dt = BP.WF.Dev2Interface.DB_CCList();

            if (sta == "0")
                dt = BP.WF.Dev2Interface.DB_CCList_UnRead(BP.Web.WebUser.No);

            if (sta == "1")
                dt = BP.WF.Dev2Interface.DB_CCList_Read();

            if (sta == "2")
                dt = BP.WF.Dev2Interface.DB_CCList_Delete(BP.Web.WebUser.No);

            //int allNum = qo.GetCount();
            //qo.DoQuery(BP.WF.SMSAttr.MyPK, pageSize, pageIdx);

            return BP.Tools.Json.ToJson(dt);
        }

        /// <summary>
        /// 我的流程的查询条件
        /// </summary>
        /// <returns></returns>
        public string Search_Conds()
        {
            if (WebUser.No == null)
                throw new Exception("err@登录信息丢失.");
            DataSet ds = new DataSet();
            string tSpan = this.GetRequestVal("TSpan");
            string keyWord = this.GetRequestVal("KeyWord");

            #region 1、获取时间段枚举/总数.
            SysEnums ses = new SysEnums("TSpan");
            DataTable dtTSpan = ses.ToDataTableField();
            dtTSpan.TableName = "TSpan";
            ds.Tables.Add(dtTSpan);
            string sqlWhere = "";
            if (DataType.IsNullOrEmpty(keyWord) == false)
                sqlWhere += " AND Title like '%" + keyWord + "%' ";
            if (DataType.IsNullOrEmpty(this.FlowNo) == false)
                sqlWhere += " AND FK_Flow='" + this.FlowNo + "'";
            string sql = " SELECT  TSpan as No, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE (Emps LIKE '%" + WebUser.No + "%' OR Starter='" + WebUser.No + "') AND FID = 0 AND WFState > 1 " + sqlWhere + " GROUP BY TSpan ";

            DataTable dtTSpanNum = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow drEnum in dtTSpan.Rows)
            {
                string no = drEnum["IntKey"].ToString();
                foreach (DataRow dr in dtTSpanNum.Rows)
                {
                    if (dr["No"].ToString() == no)
                    {
                        drEnum["Lab"] = drEnum["Lab"].ToString() + "(" + dr["Num"] + ")";
                        break;
                    }
                }
            }
            #endregion

            #region 2、处理流程类别列表.
            sqlWhere = "";
            if (DataType.IsNullOrEmpty(keyWord) == false)
                sqlWhere += " AND Title like '%" + keyWord + "%' ";
            if (tSpan != "-1")
                sqlWhere += " AND TSpan=" + tSpan;

            sql = "SELECT  FK_Flow as No, FlowName as Name, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE (Emps LIKE '%" + WebUser.No + "%' OR TodoEmps LIKE '%" + BP.Web.WebUser.No + ",%' OR Starter='" + WebUser.No + "')  AND WFState > 1 AND FID = 0 " + sqlWhere + " GROUP BY FK_Flow, FlowName";

            DataTable dtFlows = DBAccess.RunSQLReturnTable(sql);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dtFlows.Columns[0].ColumnName = "No";
                dtFlows.Columns[1].ColumnName = "Name";
                dtFlows.Columns[2].ColumnName = "Num";
            }


            dtFlows.TableName = "Flows";
            ds.Tables.Add(dtFlows);
            #endregion
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 查询总条数
        /// </summary>
        /// <returns></returns>
        public string Search_Count()
        {
            if (WebUser.No == null)
                return "";
            string tSpan = this.GetRequestVal("TSpan");
            if (DataType.IsNullOrEmpty(tSpan) == true)
                tSpan = null;
            //查询关键字
            string keyWord = this.GetRequestVal("KeyWord");
            if (DataType.IsNullOrEmpty(keyWord) == true)
                keyWord = null;

            string sqlWhere = "(Emps LIKE '%" + WebUser.No + "%' OR TodoEmps LIKE '%" + WebUser.No + "%' OR Starter = '" + WebUser.No + "') AND FID = 0 AND WFState > 1 ";
            if (tSpan != "-1")
                sqlWhere += "AND TSpan = '" + tSpan + "' ";
            if (keyWord != null)
                sqlWhere += "AND Title like '%" + keyWord + "%' ";
            if (this.FlowNo != null)
                sqlWhere += "AND FK_Flow = '" + this.FlowNo + "' ";

            //获取总条数
            string totalNumSql = "SELECT count(*) from WF_GenerWorkFlow where " + sqlWhere;
            int totalNum = DBAccess.RunSQLReturnValInt(totalNumSql);
            return totalNum.ToString();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string Search_Data()
        {
            if (WebUser.No == null)
                return "";
            string sql = "";

            string tSpan = this.GetRequestVal("TSpan");
            if (DataType.IsNullOrEmpty(tSpan) == true)
                tSpan = null;
            //查询关键字
            string keyWord = this.GetRequestVal("KeyWord");
            if (DataType.IsNullOrEmpty(keyWord) == true)
                keyWord = null;


            #region 处理查询
            //当前页
            int pageIdx = this.PageIdx;
            //每页条数
            int pageSize = this.PageSize;

            int startIndex = (pageIdx - 1) * pageSize;
            int num = pageSize * (pageIdx - 1);
            string sqlWhere = "(Emps LIKE '%" + WebUser.No + "%' OR TodoEmps LIKE '%" + WebUser.No + "%' OR Starter = '" + WebUser.No + "') AND FID = 0 AND WFState > 1 ";
            if (tSpan != "-1")
                sqlWhere += "AND TSpan = '" + tSpan + "' ";
            if (keyWord != null)
                sqlWhere += "AND Title like '%" + keyWord + "%' ";
            if (this.FlowNo != null)
                sqlWhere += "AND FK_Flow = '" + this.FlowNo + "' ";

            sqlWhere += " ORDER BY RDT DESC ";
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle
                || SystemConfig.AppCenterDBType == DBType.KingBaseR3
                || SystemConfig.AppCenterDBType == DBType.KingBaseR6
                || SystemConfig.AppCenterDBType == DBType.DM)
                sql = "SELECT NVL(WorkID, 0) WorkID,NVL(FID, 0) FID ,FK_Flow,FlowName,Title, NVL(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,NVL(RDT, '2018-05-04 19:29') RDT,NVL(FK_Node, 0) FK_Node,NodeName, TodoEmps " +
                    "FROM (select A.*, rownum r from (select * from WF_GenerWorkFlow where " + sqlWhere + ") A) where r between " + (pageIdx * pageSize - pageSize + 1) + " and " + (pageIdx * pageSize);
            else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MSSQL)
                sql = "SELECT  TOP " + pageSize + " ISNULL(WorkID, 0) WorkID,ISNULL(FID, 0) FID ,FK_Flow,FlowName,Title, ISNULL(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,ISNULL(RDT, '2018-05-04 19:29') RDT,ISNULL(FK_Node, 0) FK_Node,NodeName, TodoEmps FROM WF_GenerWorkFlow " +
                    "where WorkID not in (select top(" + num + ") WorkID from WF_GenerWorkFlow where " + sqlWhere + ") AND" + sqlWhere;
            else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
                sql = "SELECT IFNULL(WorkID, 0) WorkID,IFNULL(FID, 0) FID ,FK_Flow,FlowName,Title, IFNULL(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,IFNULL(RDT, '2018-05-04 19:29') RDT,IFNULL(FK_Node, 0) FK_Node,NodeName, TodoEmps FROM WF_GenerWorkFlow where (1=1) AND " + sqlWhere + " LIMIT " + startIndex + "," + pageSize;
            else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || BP.Difference.SystemConfig.AppCenterDBType == DBType.UX)
                sql = "SELECT COALESCE(WorkID, 0) WorkID,COALESCE(FID, 0) FID ,FK_Flow,FlowName,Title, COALESCE(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,COALESCE(RDT, '2018-05-04 19:29') RDT,COALESCE(FK_Node, 0) FK_Node,NodeName, TodoEmps FROM WF_GenerWorkFlow where (1=1) AND " + sqlWhere + " LIMIT " + pageSize + " offset " + startIndex;
            DataTable mydt = DBAccess.RunSQLReturnTable(sql);

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                mydt.Columns[0].ColumnName = "WorkID";
                mydt.Columns[1].ColumnName = "FID";
                mydt.Columns[2].ColumnName = "FK_Flow";
                mydt.Columns[3].ColumnName = "FlowName";
                mydt.Columns[4].ColumnName = "Title";
                mydt.Columns[5].ColumnName = "WFSta";
                mydt.Columns[6].ColumnName = "WFState";
                mydt.Columns[7].ColumnName = "Starter";
                mydt.Columns[8].ColumnName = "StarterName";
                mydt.Columns[9].ColumnName = "Sender";
                mydt.Columns[10].ColumnName = "RDT";
                mydt.Columns[11].ColumnName = "FK_Node";
                mydt.Columns[12].ColumnName = "NodeName";
                mydt.Columns[13].ColumnName = "TodoEmps";


            }
            mydt.TableName = "WF_GenerWorkFlow";
            if (mydt != null)
            {
                mydt.Columns.Add("TDTime");
                foreach (DataRow dr in mydt.Rows)
                {
                    dr["TDTime"] = GetTraceNewTime(dr["FK_Flow"].ToString(), int.Parse(dr["WorkID"].ToString()), int.Parse(dr["FID"].ToString()));
                }
            }
            #endregion

            return BP.Tools.Json.ToJson(mydt);
        }
        public static string GetTraceNewTime(string fk_flow, Int64 workid, Int64 fid)
        {
            #region 获取track数据.
            string sqlOfWhere2 = "";
            string sqlOfWhere1 = "";
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            if (fid == 0)
            {
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "WorkID11 OR WorkID=" + dbStr + "WorkID12 )  ";
                ps.Add("WorkID11", workid);
                ps.Add("WorkID12", workid);
            }
            else
            {
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "FID11 OR WorkID=" + dbStr + "FID12 ) ";
                ps.Add("FID11", fid);
                ps.Add("FID12", fid);
            }

            string sql = "";
            sql = "SELECT MAX(RDT) FROM ND" + int.Parse(fk_flow) + "Track " + sqlOfWhere1;
            sql = "SELECT RDT FROM  ND" + int.Parse(fk_flow) + "Track  WHERE RDT=(" + sql + ")";
            ps.SQL = sql;

            try
            {
                return DBAccess.RunSQLReturnString(ps);
            }
            catch
            {
                // 处理track表.
                Track.CreateOrRepairTrackTable(fk_flow);
                return DBAccess.RunSQLReturnString(ps);
            }
            #endregion 获取track数据.
        }
        #region 处理page接口.
        /// <summary>
        /// 执行的内容
        /// </summary>
        public string DoWhat
        {
            get
            {
                string str = this.GetRequestVal("DoWhat");
                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("DoType");
                return str;
            }
        }
        /// <summary>
        /// 当前的用户
        /// </summary>
        public string UserNo
        {
            get
            {

                string str = this.GetRequestVal("UserNo");
                if (DataType.IsNullOrEmpty(str) == true)
                    return this.GetRequestVal("UserID");
                return str;
            }
        }
        /// <summary>
        /// 用户的安全校验码(请参考集成章节)
        /// </summary>
        public string SID
        {
            get
            {
                string str = this.GetRequestVal("Token");
                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("Token");
                return str;
            }
        }
        public Int64 GenerWorkIDByEntityPK()
        {
            if (this.WorkID != 0)
                return this.WorkID;

            string entityPK = this.GetRequestVal("EntityPK");
            if (DataType.IsNullOrEmpty(entityPK) == true)
                return 0;

            string entityPKVal = this.GetRequestVal(entityPK);
            if (DataType.IsNullOrEmpty(entityPK) == true)
                throw new Exception("err@参数值[" + entityPK + "]没有传递过来,无法获取实体流程的数据.");

            GERpt rpt = new GERpt("ND" + int.Parse(this.FlowNo) + "Rpt");

            if (rpt.EnMap.Attrs.Contains(entityPK) == false)
            {
                string frmID = "ND" + int.Parse(this.FlowNo) + "Rpt";
                MapAttr attr = new MapAttr();
                attr.MyPK = frmID + "_" + entityPK;
                attr.setKeyOfEn(entityPK);
                attr.FrmID = frmID;
                attr.Name = entityPK;
                attr.MyDataType = DataType.AppString;
                attr.setMaxLen(50);
                attr.setUIVisible(false);
                attr.Save();

                //节点表单
                frmID = "ND" + int.Parse(this.FlowNo) + "01";
                attr.MyPK = frmID + "_" + entityPK;
                attr.setKeyOfEn(entityPK);
                attr.FrmID = frmID;
                attr.MyDataType = DataType.AppString;
                attr.setMaxLen(50);
                attr.setUIVisible(false);
                attr.Save();

                //从新查询.
                rpt = new GERpt("ND" + int.Parse(this.FlowNo) + "Rpt");
            }
            int i = rpt.Retrieve(entityPK, entityPKVal);

            if (rpt.OID == 0)
            {
                rpt.OID = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FlowNo, BP.Web.WebUser.No);
                rpt.Retrieve();

                rpt.PrjNo = entityPK;
                rpt.PrjName = entityPKVal;
                rpt.SetValByKey(entityPK, entityPKVal);
                rpt.Update();

                GenerWorkFlow gwf = new GenerWorkFlow(rpt.OID);
                gwf.PrjNo = entityPK;
                gwf.PrjName = entityPKVal;
                gwf.SetPara("EntityPK", entityPK);
                gwf.SetPara(entityPK, entityPKVal);
                gwf.Update();
            }

            return rpt.OID;
        }
        /// <summary>
        /// 用户登录.
        /// </summary>
        /// <returns></returns>
        public string Port_Login()
        {

            string userNo = this.UserNo;
            if (DataType.IsNullOrEmpty(userNo) == false)
            {
                BP.WF.Dev2Interface.Port_Login(userNo);
                return BP.WF.Dev2Interface.Port_GenerToken();
            }

            string token = this.GetRequestVal("Token");
            return BP.WF.Dev2Interface.Port_LoginByToken(token);
        }
        public string DevType
        {
            get
            {
                string val = this.GetRequestVal("DevType");
                if (DataType.IsNullOrEmpty(val) == true || val == "PC")
                    return "WF";
                else
                    return "CCMobile";
            }
        }
        public string PortSaaS_Init()
        {
            if (this.DoWhat == null)
                return "err@必要的参数没有传入，请参考接口规则。DoWhat";

            string token = "";

            #region 根据用户的token, 调用配置的ssoURL ，获得用户名，并登录.
            string userNo = "ccs";
            BP.WF.Port.AdminGroup.Org org = new BP.WF.Port.AdminGroup.Org();
            org.No = this.OrgNo;
            if (org.RetrieveFromDBSources() == 0)
                return "err@组织编号为" + this.OrgNo + "在数据库中不存在";
            string url = org.GetValStringByKey("SSOUrl", "");  // 格式: https:/xxxx.xxx.xxx.xx/xx.do?xxxxx={$Token}
            if (DataType.IsNullOrEmpty(url) == true || url.Contains("{$Token}") == false)
                return "err@组织信息配置错误,没有配置回调访问验证的token的服务地址，请让组织管理员登陆系统在=》组织管理=》组织属性=》SSOUrl进行设置。";
            string ticket = this.GetRequestVal("ticket");
            //根据配置的地址，获得token.
            url = url.Replace("{$ticket}", ticket).Replace("{$Token}", ticket);
            string result = DataType.ReadURLContext(url);  //执行回调：url返回用户编号.
            if (DataType.IsNullOrEmpty(result) == true)
                return "err@执行URL没有返回结果值";
            //数据序列化
            JObject jsonData = result.ToJObject();
            //code=200，表示请求成功，否则失败
            if (!jsonData["code"].ToString().Equals("0000"))
                return "err@执行SSOUrl回调URL返回结果失败";

            //获取返回的数据
            JObject data = jsonData["result"].ToString().ToJObject();
            //
            userNo = data["mobilePhone"] != null ? data["mobilePhone"].ToString() : "";
            if (DataType.IsNullOrEmpty(userNo) == true)
                return "err@读取url错误:" + userNo;

            string empID = this.OrgNo + "_" + userNo;
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = empID;
            if (emp.RetrieveFromDBSources() == 0)
                return "err@用户错误:" + emp.No;

            //执行登陆.
            BP.WF.Dev2Interface.Port_Login(userNo, this.OrgNo);
            #endregion 获得token.

            if (this.DoWhat.Equals("PortLogin") == true)
                return "登陆成功";

            #region 生成参数串.
            string paras = "";
            foreach (string str in HttpContextHelper.RequestParamKeys)
            {
                string val = this.GetRequestVal(str);
                if (val.IndexOf('@') != -1)
                    return "err@您没有能参数: [ " + str + " ," + val + " ] 给值 ，URL 将不能被执行。";
                switch (str)
                {
                    case DoWhatList.DoNode:
                    case DoWhatList.Emps:
                    case DoWhatList.EmpWorks:
                    case DoWhatList.FlowSearch:
                    case DoWhatList.Login:
                    case DoWhatList.MyFlow:
                    case DoWhatList.MyWork:
                    case DoWhatList.Start:
                    case DoWhatList.Start5:
                    case DoWhatList.StartSimple:
                    case DoWhatList.FlowFX:
                    case DoWhatList.DealWork:
                    case "StartFlow":
                    case "FK_Flow":
                    case "WorkID":
                    case "FK_Node":
                    case "Token":
                    case "DoType":
                    case "DoMethod":
                    case "HttpHandlerName":
                    case "t":
                    case "FrmID":
                    case "FK_MapData":
                    case "MyFrm":
                    case "MyView":
                    case "DoWhat":
                    case "EntityPK":
                        break;
                    default:
                        paras += "&" + str + "=" + val;
                        break;
                }
            }
            paras += "&Token=" + token;
            string nodeID = int.Parse(this.FlowNo + "01").ToString();
            #endregion 生成参数串.

            //发起流程.
            if (this.DoWhat.Equals("StartClassic") == true)
            {
                if (this.FlowNo == null)
                {
                    return "url@./AppClassic/Home.htm";
                }
                else
                {
                    return "url@./AppClassic/Home.htm?FK_Flow=" + this.FlowNo + paras + "&FK_Node=" + nodeID;
                }
            }

            //打开工作轨迹。
            if (this.DoWhat.Equals(DoWhatList.OneWork) == true)
            {
                if (this.FlowNo == null || this.WorkID == null)
                    throw new Exception("@参数 FK_Flow 或者 WorkID 为 Null 。");
                return "url@/" + this.DevType + "/MyView.htm?FK_Flow=" + this.FlowNo + "&WorkID=" + this.WorkID + "&o2=1" + paras;
            }

            //查看表单不需要FK_Node参数。
            if (this.DoWhat.Equals("MyView") == true)
            {
                //if (this.NodeID != 0)
                //    return "err@执行MyView不需要NodeID/FK_Node参数.";
                Int64 workID = this.GenerWorkIDByEntityPK();
                if (workID == 0)
                    return "err@执行MyView不需要NodeID/FK_Node参数.";

                if (this.NodeID == 0)
                {
                    GenerWorkFlow gwf = new GenerWorkFlow(workID);
                    paras += "&FK_Node=" + gwf.NodeID;
                }

                return "url@/" + this.DevType + "/MyView.htm?FK_Flow=" + this.FlowNo + "&WorkID=" + workID + "&o2=1" + paras;
            }

            //查看指定的节点表单需要FK_Node参数。
            if (this.DoWhat.Equals("MyFrm") == true)
            {
                if (this.NodeID == 0)
                    return "err@执行 MyFrm 需要NodeID/FK_Node参数.";

                Int64 workID = this.GenerWorkIDByEntityPK();
                if (workID == 0)
                    return "err@执行MyView不需要NodeID/FK_Node参数.";

                return "url@/" + this.DevType + "/MyFrm.htm?FK_Flow=" + this.FlowNo + "&FK_Node=" + this.NodeID + "&WorkID=" + workID + "&o2=1" + paras;
            }

            //发起页面,或者调用发起流程.
            if (this.DoWhat.Equals(DoWhatList.Start) == true || this.DoWhat.Equals("StartFlow"))
            {
                if (this.FlowNo == null)
                    return "url@Start.htm";

                //实体编号,启动指定实体编号隶属的工作流程ID.
                Int64 workID = this.GenerWorkIDByEntityPK();
                if (workID == 0)
                    return "url@MyFlow.htm?FK_Flow=" + this.FlowNo + paras + "&FK_Node=" + nodeID;

                // 是否可以执行当前的工作.
                if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(workID, WebUser.No) == true)
                {
                    return "url@/" + this.DevType + "/MyFlow.htm?FK_Flow=" + this.FlowNo + paras + "&WorkID=" + workID;
                }

                return "url@/" + this.DevType + "/MyView.htm?FK_Flow=" + this.FlowNo + paras + "&WorkID=" + workID;
            }

            //发起表单.
            if (this.DoWhat.Equals("StartFrm"))
            {
                int oid = this.OID;
                if (oid == 0)
                {
                    GEEntity gn = new GEEntity(this.FrmID);
                    oid = DBAccess.GenerOIDByGUID();
                    try
                    {
                        gn.SaveAsOID(oid);
                    }
                    catch (Exception ex)
                    {
                        gn.CheckPhysicsTable();
                        gn.SaveAsOID(oid);
                    }
                }
                return "url@/" + this.DevType + "/CCForm/Frm.htm?FrmID=" + this.FrmID + "&OID=" + oid + "" + paras;
            }


            //处理工作.
            if (this.DoWhat.Equals(DoWhatList.DealWork) == true || this.DoWhat.Equals(DoWhatList.MyFlow) == true)
            {
                if (DataType.IsNullOrEmpty(this.FlowNo) || this.WorkID == 0)
                    return "err@参数 FK_Flow 或者 WorkID 为Null 。";
                return "url@MyFlow.htm?FK_Flow=" + this.FlowNo + "&WorkID=" + this.WorkID + "&o2=1" + paras;
            }
            //我的抄送.
            if (this.DoWhat.Equals(DoWhatList.MyCC) == true)
            {
                if (DataType.IsNullOrEmpty(this.FlowNo) || this.WorkID == 0)
                    return "err@参数 FK_Flow 或者 WorkID 为Null 。";
                return "url@MyCC.htm?FK_Flow=" + this.FlowNo + "&WorkID=" + this.WorkID + "&o2=1" + paras + "&FK_Node=" + nodeID;
            }
            //流程设计器
            if (this.DoWhat.Equals(DoWhatList.Flows) == true)
                return "url@Portal/Flows.htm";
            //表单设计器
            if (this.DoWhat.Equals(DoWhatList.Frms) == true)
                return "url@Portal/Frms.htm";

            //请求在途.
            if (this.DoWhat.Equals(DoWhatList.Runing) == true)
                return "url@/" + this.DevType + "/Runing.htm?FK_Flow=" + this.FlowNo;

            //请求首页.
            if (this.DoWhat.Equals("Home") == true)
            {
                if (this.DevType.Equals("CCMobile"))
                    return "url@/CCMobilePortal/SaaS/Home.htm?FK_Flow=" + this.FlowNo;
                return "url@/Portal/Standard/Default.htm?FK_Flow=" + this.FlowNo;
            }

            //请求待办。
            if (this.DoWhat.Equals(DoWhatList.EmpWorks) == true || this.DoWhat.Equals("Todolist") == true)
            {
                if (DataType.IsNullOrEmpty(this.FlowNo))
                {
                    return "url@/" + this.DevType + "/Todolist.htm";
                }
                else
                {
                    return "url@/" + this.DevType + "/Todolist.htm?FK_Flow=" + this.FlowNo;
                }
            }
            //草稿
            if (this.DoWhat.Equals(DoWhatList.Draft) == true)
                return "url@Draft.htm?FK_Flow=" + this.FlowNo;
            //请求流程查询。
            if (this.DoWhat.Equals(DoWhatList.FlowSearch) == true)
            {
                if (DataType.IsNullOrEmpty(this.FlowNo))
                {
                    return "url@./RptSearch/Default.htm";
                }
                else
                {
                    return "url@./RptDfine/Search.htm?2=1&FK_Flow=001&EnsName=ND" + int.Parse(this.FlowNo) + "Rpt" + paras;
                }
            }
            //我发起的。
            if (this.DoWhat.Equals(DoWhatList.MyStartFlows) == true)
            {

                return "url@./Comm/Search.htm?EnsName=BP.WF.Data.MyStartFlows&Token=" + token;

            }
            //当前登录人处理的所有流程。
            if (this.DoWhat.Equals(DoWhatList.MyJoinFlows) == true)
            {

                return "url@./Comm/Search.htm?EnsName=BP.WF.Data.MyJoinFlows&WFSta=all&Token=" + token;

            }
            //admin查看处理的所有流程。
            if (this.DoWhat.Equals(DoWhatList.GenerWorkFlowViews) == true)
            {

                return "url@./Comm/Search.htm?EnsName=BP.WF.Data.GenerWorkFlowViews&WFSta=all&Token=" + token;

            }
            //流程查询小页面.
            if (this.DoWhat.Equals(DoWhatList.FlowSearchSmall) == true)
            {
                if (this.FlowNo == null)
                    return "url@./RptSearch/Default.htm";
                else
                    return "url./Comm/Search.htm?EnsName=ND" + int.Parse(this.FlowNo) + "Rpt" + paras;
            }

            //打开消息.
            if (this.DoWhat.Equals(DoWhatList.DealMsg) == true)
            {
                string guid = this.GetRequestVal("GUID");
                BP.WF.SMS sms = new SMS();
                sms.setMyPK(guid);
                sms.Retrieve();

                //判断当前的登录人员.
                if (BP.Web.WebUser.No != sms.SendToEmpNo)
                {
                    BP.WF.Dev2Interface.Port_Login(sms.SendToEmpNo);
                }

                AtPara ap = new AtPara(sms.AtPara);
                switch (sms.MsgType)
                {
                    case SMSMsgType.SendSuccess: // 发送成功的提示.

                        if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(ap.GetValInt64ByKey("WorkID"), BP.Web.WebUser.No) == true)
                        {
                            return "url@/" + this.DevType + "/MyFlow.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras;
                        }
                        else
                        {
                            return "url@/" + this.DevType + "/MyView.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras;
                        }

                    default: //其他的情况都是查看工作报告.
                        return "url@/" + this.DevType + "/MyView.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras;
                }
            }
            return "url@/" + this.DevType + "/AppClassic/Home.htm?Token=" + paras;
            //  return "err@没有判断的标记.";
            //  return "warning@没有约定的标记:DoWhat=" + this.DoWhat;
        }

        /// <summary>
        /// 调用页面入口
        /// </summary>
        /// <returns></returns>
        public string Port_Init()
        {
            if (this.DoWhat == null)
                return "err@必要的参数没有传入，请参考接口规则, DoWhat";


            #region 安全性校验. SID 模式.
            string token = this.GetRequestVal("Token");
            if (DataType.IsNullOrEmpty(token) == false)
            {
                Dev2Interface.Port_LoginByToken(token);
            }
            else if (DataType.IsNullOrEmpty(this.UserNo) == false)
            {
                Dev2Interface.Port_Login(this.UserNo);
                //token = Dev2Interface.Port_GenerToken("PC");
            }
            #endregion 安全性校验. SID 模式.

            if (this.DoWhat.Equals("PortLogin") == true)
                return "登陆成功";

            #region 生成参数串.
            string paras = "";
            foreach (string str in HttpContextHelper.RequestParamKeys)
            {
                string val = this.GetRequestVal(str);
                if (val.IndexOf('@') != -1)
                    return "err@您没有能参数: [ " + str + " ," + val + " ] 给值 ，URL 将不能被执行。";
                switch (str)
                {
                    case DoWhatList.DoNode:
                    case DoWhatList.Emps:
                    case DoWhatList.EmpWorks:
                    case DoWhatList.FlowSearch:
                    case DoWhatList.Login:
                    case DoWhatList.MyFlow:
                    case DoWhatList.MyWork:
                    case DoWhatList.Start:
                    case DoWhatList.Start5:
                    case DoWhatList.StartSimple:
                    case DoWhatList.FlowFX:
                    case DoWhatList.DealWork:
                    case "StartFlow":
                    case "FK_Flow":
                    case "WorkID":
                    case "FK_Node":
                    case "Token":
                    case "DoType":
                    case "DoMethod":
                    case "HttpHandlerName":
                    case "t":
                    case "FrmID":
                    case "FK_MapData":
                    case "MyFrm":
                    case "MyView":
                    case "DoWhat":
                    case "EntityPK":
                        break;
                    default:
                        paras += "&" + str + "=" + val;
                        break;
                }
            }
            paras += "&Token=" + token;
            string nodeID = int.Parse(this.FlowNo + "01").ToString();
            #endregion 生成参数串.

            //发起流程.
            if (this.DoWhat.Equals("StartClassic") == true)
            {
                if (this.FlowNo == null)
                    return "url@./AppClassic/Home.htm";
                else
                    return "url@./AppClassic/Home.htm?FK_Flow=" + this.FlowNo + paras + "&FK_Node=" + nodeID;
            }

            //打开工作轨迹。
            if (this.DoWhat.Equals(DoWhatList.OneWork) == true)
            {
                if (this.FlowNo == null || this.WorkID == null)
                    throw new Exception("@参数 FK_Flow 或者 WorkID 为 Null 。");
                return "url@MyView.htm?FK_Flow=" + this.FlowNo + "&WorkID=" + this.WorkID + "&o2=1" + paras;
            }

            //查看表单不需要FK_Node参数。
            if (this.DoWhat.Equals("MyView") == true)
            {
                //if (this.NodeID != 0)
                //    return "err@执行MyView不需要NodeID/FK_Node参数.";
                Int64 workID = this.GenerWorkIDByEntityPK();
                if (workID == 0)
                    return "err@执行MyView不需要NodeID/FK_Node参数.";

                if (this.NodeID == 0)
                {
                    GenerWorkFlow gwf = new GenerWorkFlow(workID);
                    paras += "&FK_Node=" + gwf.NodeID;
                }

                return "url@MyView.htm?FK_Flow=" + this.FlowNo + "&WorkID=" + workID + "&o2=1" + paras;
            }

            //查看指定的节点表单需要FK_Node参数。
            if (this.DoWhat.Equals("MyFrm") == true)
            {
                if (this.NodeID == 0)
                    return "err@执行 MyFrm 需要NodeID/FK_Node参数.";

                Int64 workID = this.GenerWorkIDByEntityPK();
                if (workID == 0)
                    return "err@执行MyView不需要NodeID/FK_Node参数.";

                return "url@MyFrm.htm?FK_Flow=" + this.FlowNo + "&FK_Node=" + this.NodeID + "&WorkID=" + workID + "&o2=1" + paras;
            }

            //发起页面,或者调用发起流程.
            if (this.DoWhat.Equals(DoWhatList.Start) == true || this.DoWhat.Equals("StartFlow"))
            {
                if (this.FlowNo == null)
                    return "url@Start.htm";

                //实体编号,启动指定实体编号隶属的工作流程ID.
                Int64 workID = this.GenerWorkIDByEntityPK();
                if (workID == 0)
                    return "url@MyFlow.htm?FK_Flow=" + this.FlowNo + paras + "&FK_Node=" + nodeID;

                // 是否可以执行当前的工作.
                if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(workID, WebUser.No) == true)
                    return "url@MyFlow.htm?FK_Flow=" + this.FlowNo + paras + "&WorkID=" + workID;

                return "url@MyView.htm?FK_Flow=" + this.FlowNo + paras + "&WorkID=" + workID;
            }

            //发起表单.
            if (this.DoWhat.Equals("StartFrm"))
            {
                int oid = this.OID;
                if (oid == 0)
                {
                    GEEntity gn = new GEEntity(this.FrmID);
                    oid = DBAccess.GenerOIDByGUID();
                    try
                    {
                        gn.SaveAsOID(oid);
                    }
                    catch (Exception ex)
                    {
                        gn.CheckPhysicsTable();
                        gn.SaveAsOID(oid);
                    }
                }
                return "url@/WF/CCForm/Frm.htm?FrmID=" + this.FrmID + "&OID=" + oid + "" + paras + "&Token=" + token + "&UserNo=" + UserNo;
            }

            //处理工作.
            if (this.DoWhat.Equals(DoWhatList.DealWork) == true || this.DoWhat.Equals(DoWhatList.MyFlow) == true)
            {
                if (this.WorkID == 0)
                    return "err@参数  WorkID 为Null 。";
                return "url@MyFlow.htm?WorkID=" + this.WorkID + "&o2=1" + paras;
            }

            //我的抄送.
            if (this.DoWhat.Equals(DoWhatList.MyCC) == true)
            {
                if (this.WorkID == 0)
                    return "err@参数  WorkID 为 Null 。";
                return "url@MyCC.htm?WorkID=" + this.WorkID + "&o2=1" + paras + "&FK_Node=" + nodeID;
            }

            // 抄送列表
            if (this.DoWhat.Equals(DoWhatList.CCList) == true)
                return "url@CC.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;

            //已完成
            if (this.DoWhat.Equals(DoWhatList.Complete) == true)
                return "url@Complete.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;

            // 最近发起
            if (this.DoWhat.Equals(DoWhatList.StartEarlyer) == true)
                return "url@StartEarlyer.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;

            //监控
            if (this.DoWhat.Equals(DoWhatList.Watchdog) == true)
            {
                if (DataType.IsNullOrEmpty(this.EnsName))
                    return "url@Watchdog.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;
                else
                    return "url@Watchdog.htm?" + paras.Substring(1);
            }

            //流程设计器
            if (this.DoWhat.Equals(DoWhatList.Flows) == true)
                return "url@Portal/FlowTree.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;
            //表单设计器
            if (this.DoWhat.Equals(DoWhatList.Frms) == true)
                return "url@Portal/FrmTree.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;

            //请求在途.
            if (this.DoWhat.Equals(DoWhatList.Runing) == true)
                return "url@Runing.htm?FK_Flow=" + this.FlowNo;

            //请求首页.
            if (this.DoWhat.Equals("Home") == true)
                return "url@Home.htm?FK_Flow=" + this.FlowNo;

            //请求待办。
            if (this.DoWhat.Equals(DoWhatList.EmpWorks) == true || this.DoWhat.Equals("Todolist") == true)
            {
                if (DataType.IsNullOrEmpty(this.FlowNo))
                    return "url@Todolist.htm";
                else
                    return "url@Todolist.htm?FK_Flow=" + this.FlowNo;
            }

            //草稿
            if (this.DoWhat.Equals(DoWhatList.Draft) == true)
                return "url@Draft.htm?FK_Flow=" + this.FlowNo;

            //请求流程查询。
            if (this.DoWhat.Equals(DoWhatList.FlowSearch) == true)
            {
                if (DataType.IsNullOrEmpty(this.FlowNo))
                    return "url@./RptSearch/Default.htm";
                else
                    return "url@./RptDfine/Search.htm?2=1&FK_Flow=" + this.FlowNo + "&EnsName=ND" + int.Parse(this.FlowNo) + "Rpt" + paras;
            }

            //我发起的
            if (this.DoWhat.Equals(DoWhatList.MyStartFlows) == true)
                return "url@./Comm/Search.htm?EnsName=BP.WF.Data.MyStartFlows&Token=" + token;

            //我审批的（当前登录人处理的所有流程）。
            if (this.DoWhat.Equals(DoWhatList.MyJoinFlows) == true)
                return "url@./Comm/Search.htm?EnsName=BP.WF.Data.MyJoinFlows&WFSta=all&Token=" + token;

            //我的流程分布
            if (this.DoWhat.Equals(DoWhatList.DistributedOfMy) == true)
                return "url@./RptSearch/DistributedOfMy.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;

            //我的流程
            if (this.DoWhat.Equals(DoWhatList.SearchDataGrid) == true)
                return "url@SearchDataGrid.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;

            //逾期流程
            if (this.DoWhat.Equals(DoWhatList.SearchDelays) == true)
                return "url@./Comm/Search.htm?EnsName=BP.WF.Data.Delays&Token=" + token;

            //会签主持人待办
            if (this.DoWhat.Equals(DoWhatList.HuiQianList) == true)
                return "url@HuiQianList.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;

            //协作待办 
            if (this.DoWhat.Equals(DoWhatList.TeamupList) == true)
                return "url@TeamupList.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;

            //授权待办 
            if (this.DoWhat.Equals(DoWhatList.AuthorList) == true)
                return "url@AuthorList.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;

            //工作委托2019  
            if (this.DoWhat.Equals(DoWhatList.AuthorTodolist2019) == true)
                return "url@AuthorTodolist2019.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;

            //共享任务 
            if (this.DoWhat.Equals(DoWhatList.TaskPoolSharing) == true)
                return "url@TaskPoolSharing.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;

            //申请下来的任务 
            if (this.DoWhat.Equals(DoWhatList.TaskPoolApply) == true)
                return "url@TaskPoolApply.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;

            //未来任务 
            if (this.DoWhat.Equals(DoWhatList.FutureTodolist) == true)
                return "url@FutureTodolist.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;

            //我的设置
            if (this.DoWhat.Equals(DoWhatList.MySetting) == true)
                return "url@./Comm/RefFunc/En.htm?EnName=BP.Port.Emp&No=" + BP.Web.WebUser.No + "&Token=" + token;

            //我的关注
            if (this.DoWhat.Equals(DoWhatList.MyFocus) == true)
                return "url@Focus.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;

            //挂起的工作
            if (this.DoWhat.Equals(DoWhatList.HungupList) == true)
                return "url@HungupList.htm?Token=" + token + "&UserNo=" + BP.Web.WebUser.No;

            //admin查看处理的所有流程。
            if (this.DoWhat.Equals(DoWhatList.GenerWorkFlowViews) == true)
                return "url@./Comm/Search.htm?EnsName=BP.WF.Data.GenerWorkFlowViews&WFSta=all&Token=" + token;

            //流程查询小页面.
            if (this.DoWhat.Equals(DoWhatList.FlowSearchSmall) == true)
            {
                if (this.FlowNo == null)
                    return "url@./RptSearch/Default.htm";
                else
                    return "url@./Comm/Search.htm?EnsName=ND" + int.Parse(this.FlowNo) + "Rpt" + paras;
            }

            //打开消息.
            if (this.DoWhat.Equals(DoWhatList.DealMsg) == true)
            {
                string guid = this.GetRequestVal("GUID");
                BP.WF.SMS sms = new SMS();
                sms.setMyPK(guid);
                sms.Retrieve();

                //判断当前的登录人员.
                if (BP.Web.WebUser.No != sms.SendToEmpNo)
                {
                    BP.WF.Dev2Interface.Port_Login(sms.SendToEmpNo);
                }

                AtPara ap = new AtPara(sms.AtPara);
                switch (sms.MsgType)
                {
                    case SMSMsgType.SendSuccess: // 发送成功的提示.

                        if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(ap.GetValInt64ByKey("WorkID"), BP.Web.WebUser.No) == true)
                            return "url@MyFlow.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras;
                        else
                            return "url@MyView.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras;

                    default: //其他的情况都是查看工作报告.
                        return "url@MyView.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras;
                }
            }
            //  return "err@没有判断的标记.";
            return "warning@没有约定的标记:DoWhat=" + this.DoWhat;
        }
        #endregion 处理page接口.
        /**
        Author_InitLeft
        @return
        */
        public string Author_InitLeft()
        {
            string sql = "SELECT Auther,AuthName, FK_Flow,FlowName, COUNT(FK_Flow) as Num   FROM V_WF_AuthTodolist ";
            sql += "WHERE AutherToEmpNo = '" + WebUser.No + "'";
            sql += " AND TakeBackDT >= '" + this.GetRequestVal("nowDate") + "'";
            sql += "     GROUP BY Auther, AuthName, FK_Flow, FlowName  ";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dt.Columns[0].ColumnName = "Auther";
                dt.Columns[1].ColumnName = "AuthName";
                dt.Columns[2].ColumnName = "FK_Flow";
                dt.Columns[3].ColumnName = "FlowName";
                dt.Columns[4].ColumnName = "Num";
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /**
         Author_InitDocs
         @return
         */
        public string Author_InitDocs()
        {
            string sql = "SELECT *  FROM V_WF_AuthTodolist ";
            sql += "WHERE AutherToEmpNo = '" + WebUser.No + "' and Auther ='" + this.GetRequestVal("author")
                    + "' and FK_Flow ='" + this.GetRequestVal("flowNo") + "'";
            sql += "  AND TakeBackDT >= '" + this.GetRequestVal("nowDate") + "'";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                string columnName = "";
                foreach (DataColumn col in dt.Columns)
                {
                    columnName = col.ColumnName.ToUpper();
                    switch (columnName)
                    {
                        case "AUTHER":
                            col.ColumnName = "Auther";
                            break;
                        case "AUTHNAME":
                            col.ColumnName = "AutherName";
                            break;
                        case "PWORKID":
                            col.ColumnName = "PWorkID";
                            break;
                        case "FK_NODE":
                            col.ColumnName = "FK_Node";
                            break;
                        case "FID":
                            col.ColumnName = "FID";
                            break;
                        case "WORKID":
                            col.ColumnName = "WorkID";
                            break;
                        case "AUTHERTOEMPNO":
                            col.ColumnName = "AutherToEmpNo";
                            break;
                        case "TAKEBACKDT":
                            col.ColumnName = "TakeBackDT";
                            break;
                        case "FK_FLOW":
                            col.ColumnName = "FK_Flow";
                            break;
                        case "FLOWNAME":
                            col.ColumnName = "FlowName";
                            break;
                        case "TITLE":
                            col.ColumnName = "Title";
                            break;
                    }
                }
            }
            return BP.Tools.Json.ToJson(dt);
        }
    }
}