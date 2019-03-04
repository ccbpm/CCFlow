using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using BP.WF.Template;

namespace BP.WF.CCBill
{
    /// <summary>
    /// 接口调用
    /// </summary>
    public class Dev2Interface
    {
        /// <summary>
        /// 创建工作ID
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="userNo">用户编号</param>
        /// <param name="htParas">参数</param>
        /// <returns>一个新的WorkID</returns>
        public static Int64 CreateBlankWork(string frmID, string userNo, Hashtable htParas)
        {
            GenerBill gb = new GenerBill();
            int i = gb.Retrieve(GenerBillAttr.FrmID, frmID, GenerBillAttr.Starter, userNo, GenerBillAttr.BillState, 0);
            if (i == 1)
                return gb.WorkID;

            FrmBill fb = new FrmBill(frmID);

            gb.Title = "单据:" + fb.Name + "," + BP.Web.WebUser.FK_DeptName + "," + WebUser.Name;
            gb.WorkID = BP.DA.DBAccess.GenerOID("WorkID");
            gb.BillState = BillState.None; //初始化状态.
            gb.Starter = BP.Web.WebUser.No;
            gb.StarterName = BP.Web.WebUser.Name;
            gb.FrmName = fb.Name; //单据名称.
            gb.FrmID = fb.No; //单据ID

            gb.FK_FrmTree = fb.FK_FormTree; //单据类别.
            gb.RDT = BP.DA.DataType.CurrentDataTime;
            gb.NDStep = 1;
            gb.NDStepName = "启动";
            gb.DirectInsert();

            return gb.WorkID;
        }
        /// <summary>
        /// 发送工作
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        public static string SendWork(string frmID, Int64 workID, string sendToEmpID)
        {
            return "发送chenggong.";
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="workID">工作ID</param>
        /// <returns>返回保存结果</returns>
        public static string SaveWork(string frmID, Int64 workID)
        {
            GenerBill gb = new GenerBill(workID);

            gb.BillState = BillState.Editing;
            gb.Update();

            return "保存成功...";
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="workID">工作ID</param>
        /// <returns>返回保存结果</returns>
        public static string SaveAsDraft(string frmID, Int64 workID)
        {
            GenerBill gb = new GenerBill(workID);
            if (gb.BillState != BillState.None)
                return "err@只有在None的模式下才能保存草稿。";

            if (gb.BillState != BillState.Editing)
            {
                gb.BillState = BillState.Editing;
                gb.Update();
            }
            return "保存成功...";
        }
        /// <summary>
        /// 删除单据
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        public static string MyBill_Delete(string frmID, Int64 workID)
        {
            FrmBill fb = new FrmBill(frmID);
            string sqls = "DELETE FROM WF_CCBill WHERE WorkID="+workID;
            sqls += "@DELETE FROM "+fb.PTable+" WHERE OID=" + workID;
            DBAccess.RunSQLs(sqls);
             
            return "删除成功.";
        }
        
        /// <summary>
        /// 工作退回
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <param name="returnToEmpID"></param>
        /// <param name="returnMsg"></param>
        /// <returns></returns>
        public static string ReturnWork(string frmID, Int64 workID, string returnToEmpID, string returnMsg)
        {
            return "发送chenggong.";
        }
        /// <summary>
        /// 获得发起列表
        /// </summary>
        /// <param name="empID"></param>
        /// <returns></returns>
        public static DataSet DB_StartFlows(string empID)
        {
            //定义容器.
            DataSet ds = new DataSet();

            //单据类别.
            BP.Sys.FrmTrees ens = new BP.Sys.FrmTrees();
            ens.RetrieveAll();

            DataTable dtSort = ens.ToDataTableField("Sort");
            dtSort.TableName = "Sort";
            ds.Tables.Add(dtSort);

            //查询出来单据运行模式的.
            FrmBills bills = new FrmBills();
            bills.Retrieve(FrmBillAttr.FrmBillWorkModel, 1);

            DataTable dtStart = bills.ToDataTableField();
            dtStart.TableName = "Start";
            ds.Tables.Add(dtStart);
            return ds;
        }
        /// <summary>
        /// 获得待办列表
        /// </summary>
        /// <param name="empID"></param>
        /// <returns></returns>
        public static DataTable DB_Todolist(string empID)
        {
            return new DataTable();
        }
        /// <summary>
        /// 草稿列表
        /// </summary>
        /// <param name="frmID">单据ID</param>
        /// <param name="empID">操作员</param>
        /// <returns></returns>
        public static DataTable DB_Draft(string frmID, string empID)
        {
            if (DataType.IsNullOrEmpty(empID) == true)
                empID = BP.Web.WebUser.No;

            GenerBills bills = new GenerBills();
            bills.Retrieve(GenerBillAttr.FrmID, frmID, GenerBillAttr.Starter, empID);

            return bills.ToDataTableField();
        }


    }
}
