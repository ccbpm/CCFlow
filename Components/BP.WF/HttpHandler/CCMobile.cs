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
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class CCMobile : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CCMobile()
        {
        }

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
            throw new Exception("@标记["+this.DoType+"]，没有找到.");
        }
        #endregion 执行父类的重写方法.

        public string Login_Init()
        {
            BP.WF.HttpHandler.WF ace = new WF();
            return ace.Login_Init();
        }

        public string Login_Submit()
        {
            string userNo = this.GetRequestVal("TB_No");
            string pass = this.GetRequestVal("TB_PW");

            BP.Port.Emp emp = new Emp();
            emp.No = userNo;
            if (emp.RetrieveFromDBSources() == 0)
            {
                if (DBAccess.IsExitsTableCol("Port_Emp", "NikeName") == true)
                {
                    /*如果包含昵称列,就检查昵称是否存在.*/
                    Paras ps = new Paras();
                    ps.SQL = "SELECT No FROM Port_Emp WHERE NikeName=" + SystemConfig.AppCenterDBVarStr +"userNo";
                    ps.Add("userNo", userNo);
                    //string sql = "SELECT No FROM Port_Emp WHERE NikeName='" + userNo + "'";
                    string no = DBAccess.RunSQLReturnStringIsNull(ps, null);
                    if (no == null)
                        return "err@用户名或者密码错误.";

                    emp.No = no;
                    int i = emp.RetrieveFromDBSources();
                    if (i == 0)
                        return "err@用户名或者密码错误.";
                }
                else
                {
                    return "err@用户名或者密码错误.";
                }
            }

            if (emp.CheckPass(pass) == false)
                return "err@用户名或者密码错误.";

            //调用登录方法.
            BP.WF.Dev2Interface.Port_Login(emp.No);

            return "登录成功.";
        }
        /// <summary>
        /// 会签列表
        /// </summary>
        /// <returns></returns>
        public string HuiQianList_Init()
        {
            WF wf = new WF();
            return wf.HuiQianList_Init();
        }

        public string GetUserInfo()
        {
            if (WebUser.No == null)
                return "{err:'nologin'}";

            StringBuilder append = new StringBuilder();
            append.Append("{");
            string userPath = HttpContextHelper.PhysicalApplicationPath + "/DataUser/UserIcon/";
            string userIcon = userPath + BP.Web.WebUser.No + "Biger.png";
            if (System.IO.File.Exists(userIcon))
            {
                append.Append("UserIcon:'" + BP.Web.WebUser.No + "Biger.png'");
            }
            else
            {
                append.Append("UserIcon:'DefaultBiger.png'");
            }
            append.Append(",UserName:'" + BP.Web.WebUser.Name + "'");
            append.Append(",UserDeptName:'" + BP.Web.WebUser.FK_DeptName + "'");
            append.Append("}");
            return append.ToString();
        }
        public string StartGuide_MulitSend()
        {
            WF_MyFlow en = new WF_MyFlow();
            return en.StartGuide_MulitSend();
        }
        public string Home_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("UserNo", BP.Web.WebUser.No);
            ht.Add("UserName", BP.Web.WebUser.Name);

            //系统名称.
            ht.Add("SysName", BP.Sys.SystemConfig.SysName);
            ht.Add("CustomerName", BP.Sys.SystemConfig.CustomerName);

            ht.Add("Todolist_EmpWorks", BP.WF.Dev2Interface.Todolist_EmpWorks);
            ht.Add("Todolist_Runing", BP.WF.Dev2Interface.Todolist_Runing);
            ht.Add("Todolist_Complete", BP.WF.Dev2Interface.Todolist_Complete);
            //ht.Add("Todolist_Sharing", BP.WF.Dev2Interface.Todolist_Sharing);
            ht.Add("Todolist_CCWorks", BP.WF.Dev2Interface.Todolist_CCWorks);
            //ht.Add("Todolist_Apply", BP.WF.Dev2Interface.Todolist_Apply); //申请下来的任务个数.
            //ht.Add("Todolist_Draft", BP.WF.Dev2Interface.Todolist_Draft); //草稿数量.

            ht.Add("Todolist_HuiQian", BP.WF.Dev2Interface.Todolist_HuiQian); //会签数量.

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public string Home_Init_WorkCount()
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT  TSpan as No, '' as Name, COUNT(WorkID) as Num, FROM WF_GenerWorkFlow WHERE Emps LIKE '%" + SystemConfig.AppCenterDBVarStr + "Emps%' GROUP BY TSpan";
            ps.Add("Emps", WebUser.No);
            //string sql = "SELECT  TSpan as No, '' as Name, COUNT(WorkID) as Num, FROM WF_GenerWorkFlow WHERE Emps LIKE '%" + WebUser.No + "%' GROUP BY TSpan";
            DataSet ds = new DataSet();
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
            ds.Tables.Add(dt);
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns[0].ColumnName = "TSpan";
                dt.Columns[1].ColumnName = "Num";
            }

            string sql = "SELECT IntKey as No, Lab as Name FROM Sys_Enum WHERE EnumKey='TSpan'";
            DataTable dt1 = BP.DA.DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataRow mydr in dt1.Rows)
                {
                    
                }
            }

            return BP.Tools.Json.ToJson(dt);
        }
        public string MyFlow_Init()
        {
            BP.WF.HttpHandler.WF_MyFlow wfPage = new WF_MyFlow();
            return wfPage.MyFlow_Init();
        }
        
        public string Runing_Init()
        {
            BP.WF.HttpHandler.WF wfPage = new WF();
          return  wfPage.Runing_Init();
        }
        
        /// <summary>
        /// 新版本.
        /// </summary>
        /// <returns></returns>
        public string Todolist_Init()
        {
            string fk_node = this.GetRequestVal("FK_Node");
            DataTable dt = BP.WF.Dev2Interface.DB_Todolist(WebUser.No, this.FK_Node);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 查询已完成.
        /// </summary>
        /// <returns></returns>
        public string Complete_Init()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_FlowComplete();
            return BP.Tools.Json.ToJson(dt);
        }
        public string DB_GenerReturnWorks()
        {
            /* 如果工作节点退回了*/
            BP.WF.ReturnWorks rws = new BP.WF.ReturnWorks();
            rws.Retrieve(BP.WF.ReturnWorkAttr.ReturnToNode, this.FK_Node, BP.WF.ReturnWorkAttr.WorkID, this.WorkID, BP.WF.ReturnWorkAttr.RDT);
            StringBuilder append = new StringBuilder();
            append.Append("[");
            if (rws.Count != 0)
            {
                foreach (BP.WF.ReturnWork rw in rws)
                {
                    append.Append("{");
                    append.Append("ReturnNodeName:'" + rw.ReturnNodeName + "',");
                    append.Append("ReturnerName:'" + rw.ReturnerName + "',");
                    append.Append("RDT:'" + rw.RDT + "',");
                    append.Append("NoteHtml:'" + rw.BeiZhuHtml + "'");
                    append.Append("},");
                }
                append.Remove(append.Length - 1, 1);
            }
            append.Append("]");
            return BP.Tools.Entitis2Json.Instance.ReplaceIllgalChart(append.ToString());
        }

        public string Start_Init()
        {
            BP.WF.HttpHandler.WF wfPage = new WF();
            return wfPage.Start_Init();
        }

        public string HandlerMapExt()
        {
            WF_CCForm en = new WF_CCForm();
            return en.HandlerMapExt();
        }

        /// <summary>
        /// 打开手机端
        /// </summary>
        /// <returns></returns>
        public string Do_OpenFlow()
        {
            string sid = this.GetRequestVal("SID");
            string[] strs = sid.Split('_');
            GenerWorkerList wl = new GenerWorkerList();
            int i = wl.Retrieve(GenerWorkerListAttr.FK_Emp, strs[0],
                GenerWorkerListAttr.WorkID, strs[1],
                GenerWorkerListAttr.IsPass, 0);

            if (i == 0)
            {
                return "err@提示:此工作已经被别人处理或者此流程已删除。";
            }

            BP.Port.Emp empOF = new BP.Port.Emp(wl.FK_Emp);
            Web.WebUser.SignInOfGener(empOF);
            return "MyFlow.htm?FK_Flow=" + wl.FK_Flow + "&WorkID=" + wl.WorkID + "&FK_Node=" + wl.FK_Node + "&FID=" + wl.FID;
        }
        /// <summary>
        /// 流程单表单查看.
        /// </summary>
        /// <returns>json</returns>
        public string FrmView_Init()
        {
            BP.WF.HttpHandler.WF wf = new WF();
            return wf.FrmView_Init();
        }
        /// <summary>
        /// 撤销发送
        /// </summary>
        /// <returns></returns>
        public string FrmView_UnSend()
        {
            BP.WF.HttpHandler.WF_WorkOpt_OneWork en = new WF_WorkOpt_OneWork();
            return en.OP_UnSend();
        }

        public string AttachmentUpload_Down()
        {
            WF_CCForm ccform = new WF_CCForm();
            return ccform.AttachmentUpload_Down();
        }

        public string AttachmentUpload_DownByStream()
        {
            WF_CCForm ccform = new WF_CCForm();
            return ccform.AttachmentUpload_DownByStream();
        }

        #region 关键字查询.
        /// <summary>
        /// 打开表单
        /// </summary>
        /// <returns></returns>
        public string SearchKey_OpenFrm()
        {
            BP.WF.HttpHandler.WF_RptSearch search = new WF_RptSearch();
            return search.KeySearch_OpenFrm();
        }
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <returns></returns>
        public string SearchKey_Query()
        {
            BP.WF.HttpHandler.WF_RptSearch search = new WF_RptSearch();
            return search.KeySearch_Query();
        }
        #endregion 关键字查询.

        #region 查询.
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string Search_Init()
        {
            DataSet ds = new DataSet();
            string sql = "";

            string tSpan = this.GetRequestVal("TSpan");
            if (tSpan == "")
                tSpan = null;

            #region 1、获取时间段枚举/总数.
            SysEnums ses = new SysEnums("TSpan");
            DataTable dtTSpan = ses.ToDataTableField();
            dtTSpan.TableName = "TSpan";
            ds.Tables.Add(dtTSpan);

            if (this.FK_Flow == null)
                sql = "SELECT  TSpan as No, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE (Emps LIKE '%" + WebUser.No + "%' OR Starter='" + WebUser.No + "') AND WFState > 1 GROUP BY TSpan";
            else
                sql = "SELECT  TSpan as No, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE FK_Flow='" + this.FK_Flow + "' AND (Emps LIKE '%" + WebUser.No + "%' OR Starter='" + WebUser.No + "')  AND WFState > 1 GROUP BY TSpan";

            DataTable dtTSpanNum = BP.DA.DBAccess.RunSQLReturnTable(sql);
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
            if (tSpan == "-1")
                sql = "SELECT  FK_Flow as No, FlowName as Name, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE (Emps LIKE '%" + WebUser.No + "%' OR TodoEmps LIKE '%" + BP.Web.WebUser.No + ",%' OR Starter='" + WebUser.No + "')  AND WFState > 1 AND FID = 0 GROUP BY FK_Flow, FlowName";
            else
                sql = "SELECT  FK_Flow as No, FlowName as Name, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE TSpan=" + tSpan + " AND (Emps LIKE '%" + WebUser.No + "%' OR TodoEmps LIKE '%"+BP.Web.WebUser.No+",%' OR Starter='" + WebUser.No + "')  AND WFState > 1 AND FID = 0 GROUP BY FK_Flow, FlowName";

            DataTable dtFlows = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dtFlows.Columns[0].ColumnName = "No";
                dtFlows.Columns[1].ColumnName = "Name";
                dtFlows.Columns[2].ColumnName = "Num";
            }
            dtFlows.TableName = "Flows";
            ds.Tables.Add(dtFlows);
            #endregion

            #region 3、处理流程实例列表.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            String sqlWhere = "";
            sqlWhere = "(1 = 1)AND (((Emps LIKE '%" + WebUser.No + "%')OR(TodoEmps LIKE '%" + WebUser.No + "%')OR(Starter = '" + WebUser.No + "')) AND (WFState > 1)";
            if (tSpan != "-1")
            {
                sqlWhere += "AND (TSpan = '" + tSpan + "') ";
            }

            if (this.FK_Flow != null)
            {
                sqlWhere += "AND (FK_Flow = '" + this.FK_Flow + "')) ";
            }
            else
            {
                sqlWhere += ")";
            }
            sqlWhere += "ORDER BY RDT DESC";

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
                sql = "SELECT NVL(WorkID, 0) WorkID,NVL(FID, 0) FID ,FK_Flow,FlowName,Title, NVL(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,NVL(RDT, '2018-05-04 19:29') RDT,NVL(FK_Node, 0) FK_Node,NodeName, TodoEmps FROM (select * from WF_GenerWorkFlow where " + sqlWhere + ") where rownum <= 500";
            else if(SystemConfig.AppCenterDBType == DBType.MSSQL)
                sql = "SELECT  TOP 500 ISNULL(WorkID, 0) WorkID,ISNULL(FID, 0) FID ,FK_Flow,FlowName,Title, ISNULL(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,ISNULL(RDT, '2018-05-04 19:29') RDT,ISNULL(FK_Node, 0) FK_Node,NodeName, TodoEmps FROM WF_GenerWorkFlow where " + sqlWhere;
            else if (SystemConfig.AppCenterDBType == DBType.MySQL || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                sql = "SELECT IFNULL(WorkID, 0) WorkID,IFNULL(FID, 0) FID ,FK_Flow,FlowName,Title, IFNULL(WFSta, 0) WFSta,WFState,  Starter, StarterName,Sender,IFNULL(RDT, '2018-05-04 19:29') RDT,IFNULL(FK_Node, 0) FK_Node,NodeName, TodoEmps FROM WF_GenerWorkFlow where " + sqlWhere +" LIMIT 500";

            DataTable mydt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
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
            

            ds.Tables.Add(mydt);

            return BP.Tools.Json.ToJson(ds);
        }
         public static string  GetTraceNewTime(string fk_flow, Int64 workid, Int64 fid)
        {
            #region 获取track数据.
            string sqlOfWhere2 = "";
            string sqlOfWhere1 = "";
            string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
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
            sql = "SELECT MAX(RDT) FROM ND" + int.Parse(fk_flow) + "Track " + sqlOfWhere1 ;
             sql = "SELECT RDT FROM  ND" + int.Parse(fk_flow) + "Track  WHERE RDT=("+sql+")";
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
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public string Search_Search()
        {
            string TSpan = this.GetRequestVal("TSpan");
            string FK_Flow = this.GetRequestVal("FK_Flow");

            GenerWorkFlows gwfs = new GenerWorkFlows();
            QueryObject qo = new QueryObject(gwfs);
            qo.AddWhere(GenerWorkFlowAttr.Emps, " LIKE ", "%" + BP.Web.WebUser.No + "%");
            if (!DataType.IsNullOrEmpty(TSpan))
            {
                qo.addAnd();
                qo.AddWhere(GenerWorkFlowAttr.TSpan, this.GetRequestVal("TSpan"));
            }
            if (!DataType.IsNullOrEmpty(FK_Flow))
            {
                qo.addAnd();
                qo.AddWhere(GenerWorkFlowAttr.FK_Flow, this.GetRequestVal("FK_Flow"));
            }
            qo.Top = 50;

            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                qo.DoQuery();
                DataTable dt = gwfs.ToDataTableField("Ens");
                return BP.Tools.Json.ToJson(dt);
            }
            else
            {
                DataTable dt = qo.DoQueryToTable();
                return BP.Tools.Json.ToJson(dt);
            }
        }

        #endregion

    }
}
