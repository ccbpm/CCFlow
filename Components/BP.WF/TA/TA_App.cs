using System;
using System.Collections.Generic;
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
using System.Runtime.InteropServices.WindowsRuntime;

namespace BP.TA
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class TA_App : BP.WF.HttpHandler.DirectoryPageBase
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public TA_App()
        {
        }

        #region 属性.
        public Int64 TaskID
        {
            get
            {
                return this.GetRequestValInt64("TaskID");
            }
        }
        public string Msg
        {
            get
            {
                return this.GetRequestVal("Msg");
            }
        }
        public string TemplateNo
        {
            get
            {
                return this.GetRequestVal("TemplateNo");
            }
        }
        public string PrjNo
        {
            get
            {
                return this.GetRequestVal("PrjNo");
            }
        }
        #endregion 属性.


        #region  菜单 .
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GL_TATrack_Init()
        {
            string sql = "SELECT B.MyPK,B.TaskID, A.PrjNo, A.PrjName, a.Title, B.ActionType, B.ActionName, B.EmpNo,B.EmpName, B.RDT, B.Docs ";
            sql += " FROM TA_Task A, TA_Track B WHERE A.OID=B.TaskID ORDER BY A.PrjNo,A.OID,B.RDT ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        public string GL_TAMyTrack_Init()
        {
            string sql = "SELECT B.MyPK,B.TaskID, A.PrjNo, A.PrjName, a.Title, B.ActionType, B.ActionName, B.EmpNo,B.EmpName, B.RDT , B.Docs";
            sql += " FROM TA_Task A, TA_Track B WHERE A.OID =B.TaskID AND B.EmpNo='" + WebUser.No+ "' ORDER BY  A.PrjNo,A.OID,B.RDT  ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        public string Start_Init()
        {
            return TaskAPI.DB_Start();
        }
        public string Todolist_Init()
        {
            return TaskAPI.DB_Todolist();
        }

        #endregion 菜单.

        #region  工作处理器 .
        /// <summary>
        /// 我的项目初始化
        /// </summary>
        /// <returns></returns>
        public string MyPrj_Init()
        {
            //如果没有PrjNo = 就创建.
            string prjNo = this.PrjNo;
            if (DataType.IsNullOrEmpty(prjNo) == true)
                prjNo = TaskAPI.Prj_CreateNo(this.TemplateNo);

            return "执行成功.";
        }
        public string Prj_CreateNo()
        {
            return TaskAPI.Prj_CreateNo(this.TemplateNo);
        }
        public string Prj_Start()
        {
            //发起
            return TaskAPI.Prj_Start(this.PrjNo);
        }
        public string Prj_Complete()
        {
            //发起
            return TaskAPI.Prj_Complete(this.PrjNo);
        }
        public string Prj_DeleteByRel()
        {
            //发起
            return TaskAPI.Prj_DeleteByRel(this.PrjNo);
        }
        public string Prj_DeleteByFlag()
        {
            //发起
            return TaskAPI.Prj_DeleteByFlag(this.PrjNo);
        }
        #endregion 工作处理器.


        #region  工作处理部件 .
        /// <summary>
        /// 退回
        /// </summary>
        /// <returns></returns>
        public string Task_Return()
        {
            //提交工作.
            return TaskAPI.Task_Return(this.TaskID, this.Msg);
        }
        public string Task_CheckReturn()
        {
            int result = this.GetRequestValInt("CheckedResult"); //审核结果.
            string shiftEmpNo = this.GetRequestVal("ShiftEmpNo"); //要移交的人员.

            //提交工作.
            return TaskAPI.Task_CheckReturn(this.TaskID, result, this.Msg, shiftEmpNo);
        }
        public string Task_CheckSubmit()
        {
            int result = this.GetRequestValInt("CheckedResult");
            //提交工作.
            return TaskAPI.Task_CheckSubmit(this.TaskID, result, this.Msg);
        }
        public string Task_Shift()
        {
            string toEmpNo= this.GetRequestVal("ShiftToEmpNo");

            //提交工作.
            return TaskAPI.Task_Shift(this.TaskID, toEmpNo, this.Msg);
        }
        public string Task_HuiBao()
        {
            //提交工作.
            return TaskAPI.Task_HuiBao(this.TaskID, this.GetRequestVal("Msg1"), this.GetRequestVal("Msg2"),this.GetRequestValInt("WCL"),this.GetRequestValInt("UseHH"),this.GetRequestValInt("UseMM"));
        }
        #endregion 工作处理部件.


    }
}
