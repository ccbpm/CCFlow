using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using BP.DA;
using BP.Port;
using BP.Web;
using BP.En;
using BP.WF.Data;
using BP.Sys;

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
    /// </summary>
    public class Dev2InterfaceAnonymous
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

            string dbstr = SystemConfig.AppCenterDBVarStr;

            Flow fl = new Flow(flowNo);
            Node nd = new Node(fl.StartNodeID);

            Emp empStarter = new Emp(BP.Web.WebUser.No);


            //把一些其他的参数也增加里面去,传递给ccflow.
            Hashtable htPara = new Hashtable();
            if (parentWorkID != 0)
                htPara.Add(StartFlowParaNameList.PWorkID, parentWorkID);
            if (parentFlowNo != null)
                htPara.Add(StartFlowParaNameList.PFlowNo, parentFlowNo);
            if (parentNodeID != 0)
                htPara.Add(StartFlowParaNameList.PNodeID, parentNodeID);
            if (parentEmp !=null)
                htPara.Add(StartFlowParaNameList.PEmp, parentEmp);


            Work wk = fl.NewWork(empStarter, htPara);
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
            if (DataType.IsNullOrEmpty(title) == false)
            {
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
                ps.Add(GERptAttr.Title, BP.WF.WorkFlowBuessRole.GenerTitle(fl, wk));
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
                BP.WF.Dev2Interface.SetParentInfo(flowNo, workID, parentWorkID);
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
        /// 获取Guest的待办
        /// </summary>
        /// <param name="fk_flow">流程编号,流程编号为空表示所有的流程.</param>
        /// <param name="guestNo">客户编号</param>
        /// <returns>结果集合</returns>
        public static DataTable DB_GenerEmpWorksOfDataTable(string fk_flow, string guestNo)
        {
         

            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            string sql;

            /*不是授权状态*/
            if (DataType.IsNullOrEmpty(fk_flow))
            {
                ps.SQL = "SELECT * FROM WF_EmpWorks WHERE GuestNo=" + dbstr + "GuestNo AND FK_Emp='Guest' ORDER BY FK_Flow,ADT DESC ";
                ps.Add("GuestNo", guestNo);
            }
            else
            {
                ps.SQL = "SELECT * FROM WF_EmpWorks WHERE GuestNo=" + dbstr + "GuestNo AND FK_Emp='Guest' AND FK_Flow=" + dbstr + "FK_Flow ORDER BY  ADT DESC ";
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
        public static DataTable DB_GenerRuning(string fk_flow, string guestNo)
        {
           
            string sql;
            int state = (int)WFState.Runing;

            if (DataType.IsNullOrEmpty(fk_flow))
                sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 AND B.IsPass=1 AND A.GuestNo='" + guestNo + "' ";
            else
                sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 AND B.IsPass=1  AND A.GuestNo='" + guestNo + "'";

            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.RetrieveInSQL(GenerWorkFlowAttr.WorkID, "(" + sql + ")");
            return gwfs.ToDataTableField();
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
            if (DataType.IsNullOrEmpty(FlowMark))
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
