using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using BP.WF;
using BP.DA;
using BP.Port;
using BP.Web;
using BP.En;
using BP.Sys;
using BP.WF.Data;

namespace BP.WF
{
    /// <summary>
    /// 此接口为程序员二次开发使用,在阅读代码前请注意如下事项.
    /// 1, CCFlow的对外的接口都是以静态方法来实现的.
    /// 2, 以 DB_ 开头的是需要返回结果集合的接口.
    /// 3, 以 Flow_ 是流程接口.
    /// 4, 以 Node_ 是节点接口。
    /// 5, 以 Port_ 是组织架构接口.
    /// 6, 以 DTS_ 是调度． 
    /// 7, 以 UI_ 是流程的功能窗口． 
    /// 外部用户访问接口
    /// </summary>
    public class Dev2InterfaceGuest
    {
        /// <summary>
        /// 创建WorkID
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="ht">表单参数，可以为null。</param>
        /// <param name="workDtls">明细表参数，可以为null。</param>
        /// <param name="nextWorker">操作员，如果为null就是当前人员。</param>
        /// <param name="title">创建工作时的标题，如果为null，就按设置的规则生成。</param>
        /// <returns>为开始节点创建工作后产生的WorkID.</returns>
        public static Int64 Node_CreateBlankWork(string flowNo, Hashtable ht, DataSet workDtls,
            string guestNo, string title)
        {
            return Node_CreateBlankWork(flowNo, ht, workDtls, guestNo, title, 0, null,0,null);
        }
        /// <summary>
        /// 创建WorkID
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="ht">表单参数，可以为null。</param>
        /// <param name="workDtls">明细表参数，可以为null。</param>
        /// <param name="starter">流程的发起人</param>
        /// <param name="title">创建工作时的标题，如果为null，就按设置的规则生成。</param>
        /// <param name="parentWorkID">父流程的WorkID,如果没有父流程就传入为0.</param>
        /// <param name="parentFlowNo">父流程的流程编号,如果没有父流程就传入为null.</param>
        /// <returns>为开始节点创建工作后产生的WorkID.</returns>
        public static Int64 Node_CreateBlankWork(string flowNo, Hashtable ht, DataSet workDtls,
            string guestNo, string title, Int64 parentWorkID, string parentFlowNo, int parentNodeID, string parentEmp)
        {
            //if (BP.Web.WebUser.No != "Guest")
            //    throw new Exception("@必须是Guest登陆才能发起.");

            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            //转化成编号
            parentFlowNo = TurnFlowMarkToFlowNo(parentFlowNo);

            string dbstr = SystemConfig.AppCenterDBVarStr;

            Flow fl = new Flow(flowNo);
            Node nd = new Node(fl.StartNodeID);


            //把一些其他的参数也增加里面去,传递给ccflow.
            Hashtable htPara = new Hashtable();
            if (parentWorkID != 0)
                htPara.Add(StartFlowParaNameList.PWorkID, parentWorkID);
            if (parentFlowNo != null)
                htPara.Add(StartFlowParaNameList.PFlowNo, parentFlowNo);
            if (parentNodeID != 0)
                htPara.Add(StartFlowParaNameList.PNodeID, parentNodeID);
            if (parentEmp != null)
                htPara.Add(StartFlowParaNameList.PEmp, parentEmp);


            Emp empStarter = new Emp(BP.Web.WebUser.No);
            Work wk = fl.NewWork(empStarter,htPara);
            Int64 workID = wk.OID;

            #region 给各个属性-赋值
            if (ht != null)
            {
                foreach (string str in ht.Keys)
                    wk.SetValByKey(str, ht[str]);
            }
            wk.OID = workID;
            if (workDtls != null)
            {
                //保存从表
                foreach (DataTable dt in workDtls.Tables)
                {
                    foreach (MapDtl dtl in wk.HisMapDtls)
                    {
                        if (dt.TableName != dtl.No)
                            continue;
                        //获取dtls
                        GEDtls daDtls = new GEDtls(dtl.No);
                        daDtls.Delete(GEDtlAttr.RefPK, wk.OID); // 清除现有的数据.

                        GEDtl daDtl = daDtls.GetNewEntity as GEDtl;
                        daDtl.RefPK = wk.OID.ToString();

                        // 为从表复制数据.
                        foreach (DataRow dr in dt.Rows)
                        {
                            daDtl.ResetDefaultVal();
                            daDtl.RefPK = wk.OID.ToString();

                            //明细列.
                            foreach (DataColumn dc in dt.Columns)
                            {
                                //设置属性.
                                daDtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                            }
                            daDtl.InsertAsOID(DBAccess.GenerOID("Dtl")); //插入数据.
                        }
                    }
                }
            }
            #endregion 赋值

            Paras ps = new Paras();
            // 执行对报表的数据表WFState状态的更新,让它为runing的状态.
            if (string.IsNullOrEmpty(title) == false)
            {
                if (fl.TitleRole != "@OutPara")
                {
                    fl.TitleRole = "@OutPara";
                    fl.Update();
                }

                ps = new Paras();
                ps.SQL = "UPDATE " + fl.PTable + " SET WFState=" + dbstr + "WFState,Title=" + dbstr + "Title WHERE OID=" + dbstr + "OID";
                ps.Add(GERptAttr.WFState, (int)WFState.Blank);
                ps.Add(GERptAttr.Title, title);
                ps.Add(GERptAttr.OID, wk.OID);
                DBAccess.RunSQL(ps);
            }
            else
            {
                ps = new Paras();
                ps.SQL = "UPDATE " + fl.PTable + " SET WFState=" + dbstr + "WFState,FK_Dept=" + dbstr + "FK_Dept,Title=" + dbstr + "Title WHERE OID=" + dbstr + "OID";
                ps.Add(GERptAttr.WFState, (int)WFState.Blank);
                ps.Add(GERptAttr.FK_Dept, empStarter.FK_Dept);
                ps.Add(GERptAttr.Title, WorkNode.GenerTitle(fl, wk));
                ps.Add(GERptAttr.OID, wk.OID);
                DBAccess.RunSQL(ps);
            }

            // 删除有可能产生的垃圾数据,比如上一次没有发送成功，导致数据没有清除.
            ps = new Paras();
            ps.SQL = "DELETE FROM WF_GenerWorkFlow  WHERE WorkID=" + dbstr + "WorkID1 OR FID=" + dbstr + "WorkID2";
            ps.Add("WorkID1", wk.OID);
            ps.Add("WorkID2", wk.OID);
            DBAccess.RunSQL(ps);

            ps = new Paras();
            ps.SQL = "DELETE FROM WF_GenerWorkerList  WHERE WorkID=" + dbstr + "WorkID1 OR FID=" + dbstr + "WorkID2";
            ps.Add("WorkID1", wk.OID);
            ps.Add("WorkID2", wk.OID);
            DBAccess.RunSQL(ps);

            // 设置流程信息
            if (parentWorkID != 0)
                BP.WF.Dev2Interface.SetParentInfo(flowNo, workID, parentFlowNo, parentWorkID,parentNodeID,parentEmp);

            #region 处理generworkid
            // 设置父流程信息.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = wk.OID;
            int i = gwf.RetrieveFromDBSources();

            //将流程信息提前写入wf_GenerWorkFlow,避免查询不到
            gwf.FlowName = fl.Name;
            gwf.FK_Flow = flowNo;
            gwf.FK_FlowSort = fl.FK_FlowSort;
            gwf.FK_Dept = WebUser.FK_Dept;
            gwf.DeptName = WebUser.FK_DeptName;
            gwf.FK_Node = fl.StartNodeID;
            gwf.NodeName = nd.Name;
            gwf.WFState = WFState.Runing;
            if (string.IsNullOrEmpty(title))
                gwf.Title = BP.WF.WorkNode.GenerTitle(fl, wk);
            else
                gwf.Title = title;
            gwf.Starter = WebUser.No;
            gwf.StarterName = WebUser.Name;
            gwf.RDT = DataType.CurrentDataTime;
            gwf.PWorkID = parentWorkID;
           // gwf.PFID = parentFID;
            gwf.PFlowNo = parentFlowNo;
            gwf.PNodeID = parentNodeID;
            if (i == 0)
                gwf.Insert();
            else
                gwf.Update();

            //插入待办.
            GenerWorkerList gwl = new GenerWorkerList();
            gwl.WorkID = wk.OID;
            gwl.FK_Node = nd.NodeID;
            gwl.FK_Emp = WebUser.No;
            i = gwl.RetrieveFromDBSources();

            gwl.FK_EmpText = WebUser.Name;
            gwl.FK_NodeText = nd.Name;
            gwl.FID = 0;
            gwl.FK_Flow = fl.No;
            gwl.FK_Dept = WebUser.FK_Dept;
            gwl.SDT = DataType.CurrentDataTime;
            gwl.DTOfWarning = DataType.CurrentDataTime;
            gwl.RDT = DataType.CurrentDataTime;
            gwl.IsEnable = true;
            gwl.IsPass = false;
            gwl.PRI = gwf.PRI;
            if (i == 0)
                gwl.Insert();
            else
                gwl.Update();
            #endregion

            return wk.OID;
        }

        #region 门户。
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="guestNo">客户编号</param>
        /// <param name="guestName">客户名称</param>
        public static void Port_Login(string guestNo,string guestName)
        {
            //登陆.
            BP.Web.GuestUser.SignInOfGener(guestNo, guestName, "CH", true);
        }
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="guestNo">客户编号</param>
        /// <param name="guestName">客户名称</param>
        /// <param name="deptNo">客户的部门编号</param>
        /// <param name="deptName">客户的部门名称</param>
        public static void Port_Login(string guestNo, string guestName, string deptNo, string deptName)
        {
            //登陆.
            BP.Web.GuestUser.SignInOfGener(guestNo, guestName, deptNo,deptName,"CH", true);
        }
        /// <summary>
        /// 退出登陆.
        /// </summary>
        public static void Port_LoginOunt()
        {
            //登陆.
            BP.Web.GuestUser.Exit();
        }
        #endregion 门户。


        #region 获取Guest的待办
        /// <summary>
        /// 获得可以发起的流程列表
        /// </summary>
        /// <returns>返回一个No,Name数据源，用于生成一个列表.</returns>
        public static DataTable DB_GenerCanStartFlowsOfDataTable()
        {
            return BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Flow as No, FlowName AS Name  FROM WF_Node  WHERE IsGuestNode=1 AND NodePosType=0");
        }
        /// <summary>
        /// 获取Guest的待办
        /// </summary>
        /// <param name="fk_flow">流程编号,流程编号为空表示所有的流程.</param>
        /// <param name="guestNo">客户编号</param>
        /// <returns>结果集合</returns>
        public static DataTable DB_GenerEmpWorksOfDataTable( string guestNo, string fk_flow = null)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            string sql;

            /*不是授权状态*/
            if (string.IsNullOrEmpty(fk_flow))
            {
                ps.SQL = "SELECT * FROM WF_EmpWorks WHERE GuestNo=" + dbstr + "GuestNo ORDER BY FK_Flow,ADT DESC ";
                ps.Add("GuestNo", guestNo);
            }
            else
            {
                ps.SQL = "SELECT * FROM WF_EmpWorks WHERE GuestNo=" + dbstr + "GuestNo  AND FK_Flow=" + dbstr + "FK_Flow ORDER BY  ADT DESC ";
                ps.Add("FK_Flow", fk_flow);
                ps.Add("GuestNo", guestNo);
            }
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获取未完成的流程(也称为在途流程:我参与的但是此流程未完成)
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>返回从数据视图WF_GenerWorkflow查询出来的数据.</returns>
        public static DataTable DB_GenerRuning(string fk_flow=null, string guestNo=null)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);
            if (guestNo == null)
                guestNo = BP.Web.GuestUser.No;

            string sql;
            int state = (int)WFState.Runing;

            if (string.IsNullOrEmpty(fk_flow))
                sql = "SELECT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 AND B.IsPass=1 AND A.GuestNo='" + guestNo + "' ";
            else
                sql = "SELECT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 AND B.IsPass=1  AND A.GuestNo='" + guestNo + "'";

            return BP.DA.DBAccess.RunSQLReturnTable(sql);

            //GenerWorkFlows gwfs = new GenerWorkFlows();
            //gwfs.RetrieveInSQL(GenerWorkFlowAttr.WorkID, "(" + sql + ")");
            //return gwfs.ToDataTableField();
        }
        #endregion

        #region 功能
        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="guestNo">客户编号</param>
        /// <param name="guestName">客户名称</param>
        public static void SetGuestInfo(string flowNo, Int64 workID, string guestNo, string guestName)
        {
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET GuestNo=" + dbstr + "GuestNo, GuestName=" + dbstr + "GuestName WHERE WorkID=" + dbstr + "WorkID";
            ps.Add("GuestNo", guestNo);
            ps.Add("GuestName", guestName);
            ps.Add("WorkID", workID);
            BP.DA.DBAccess.RunSQL(ps);

            Flow fl = new Flow(flowNo);
            ps = new Paras();
            ps.SQL = "UPDATE " + fl.PTable + " SET GuestNo=" + dbstr + "GuestNo, GuestName=" + dbstr + "GuestName WHERE OID=" + dbstr + "OID";
            ps.Add("GuestNo", guestNo);
            ps.Add("GuestName", guestName);
            ps.Add("OID", workID);
            BP.DA.DBAccess.RunSQL(ps);
        }
        /// <summary>
        /// 设置当前用户的待办
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="guestNo">客户编号</param>
        /// <param name="guestName">客户名称</param>
        public static void SetGuestToDoList(Int64 workID, string guestNo, string guestName)
        {
            if (guestNo == "")
                throw new Exception("@设置外部用户待办信息失败:参数guestNo不能为空.");
            if (workID == 0)
                throw new Exception("@设置外部用户待办信息失败:参数workID不能为0.");

            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerList SET GuestNo=" + dbstr + "GuestNo, GuestName=" + dbstr + "GuestName WHERE WorkID=" + dbstr + "WorkID AND IsPass=0";
            ps.Add("GuestNo", guestNo);
            ps.Add("GuestName", guestName);
            ps.Add("WorkID", workID);
            int i = BP.DA.DBAccess.RunSQL(ps);
            if (i == 0)
                throw new Exception("@设置外部用户待办信息失败:参数workID不能为空.");

            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET GuestNo=" + dbstr + "GuestNo, GuestName=" + dbstr + "GuestName WHERE WorkID=" + dbstr + "WorkID ";
            ps.Add("GuestNo", guestNo);
            ps.Add("GuestName", guestName);
            ps.Add("WorkID", workID);
            i = BP.DA.DBAccess.RunSQL(ps);
            if (i == 0)
                throw new Exception("@WF_GenerWorkFlow - 设置外部用户待办信息失败:参数WorkID不能为空.");
        }
        #endregion

        #region 通用方法
        public static string TurnFlowMarkToFlowNo(string FlowMark)
        {
            if (string.IsNullOrEmpty(FlowMark))
                return null;

            // 如果是编号，就不用转化.
            if (DataType.IsNumStr(FlowMark))
                return FlowMark;

            string s = DBAccess.RunSQLReturnStringIsNull("SELECT No FROM WF_Flow WHERE FlowMark='" + FlowMark + "'", null);
            if (s == null)
                throw new Exception("@FlowMark错误:" + FlowMark + ",没有找到它的流程编号.");
            return s;
        }
        #endregion
    }
}
