using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;
using BP.UnitTesting;

namespace BP.UnitTesting.SendCase
{
    /// <summary>
    /// 为多人创建开始工作节点
    /// </summary>
    public class StarterFlowByERS : TestBase
    {
        /// <summary>
        /// 部门编号为GUID模式下的发送
        /// </summary>
        public StarterFlowByERS()
        {
            this.Title = "为多人创建开始工作节点";
            this.DescIt = "以send024,send023,send005为基础测试，为多人创建开始工作节点.";
            this.EditState = EditState.Passed;
        }

        #region 全局变量
        /// <summary>
        /// 流程编号
        /// </summary>
        public string fk_flow = "";
        /// <summary>
        /// 用户编号
        /// </summary>
        public string userNo = "";
        /// <summary>
        /// 所有的流程
        /// </summary>
        public Flow fl = null;
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 workid = 0;
        /// <summary>
        /// 发送后返回对象
        /// </summary>
        public SendReturnObjs objs = null;
        /// <summary>
        /// 工作人员列表
        /// </summary>
        public GenerWorkerList gwl = null;
        /// <summary>
        /// 流程注册表
        /// </summary>
        public GenerWorkFlow gwf = null;
        #endregion 变量

        /// <summary>
        /// 
        /// </summary>
        public override void Do()
        {
            // string 
            this.fk_flow = "023";
            this.userNo = "zhoupeng";
            string toEmps = "zhanghaicheng,zhangyifan";
            string belontToDept = "2";

            #region 检查接受人的待办工作.
            sql = "SELECT * FROM WF_EmpWorks WHERE WorkID=" + this.workid;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 2)
                throw new Exception("@应当有两个人的待办工作,现在是:" + dt.Rows.Count + "个.");

            foreach (DataRow dr in dt.Rows)
            {
                string fk_emp = dr["FK_Emp"].ToString();

                if (fk_emp == "zhanghaicheng")
                {
                }

                if (fk_emp == "zhangyifan")
                {
                }

            }
            #endregion 检查接受人的待办工作.
        }
    }
}
