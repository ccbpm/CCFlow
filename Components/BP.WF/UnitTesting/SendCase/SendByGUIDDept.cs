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
    /// 部门编号为GUID模式下的发送
    /// </summary>
    public class SendByGUIDDept : TestBase
    {
        /// <summary>
        /// 部门编号为GUID模式下的发送
        /// </summary>
        public SendByGUIDDept()
        {
            this.Title = "部门编号为GUID模式下的发送";
            this.DescIt = "以send024,send023,send005为基础测试，部门编号是GUID模式下的数据存储问题.";
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
        /// 部门编号为GUID模式下的发送
        /// </summary>
        public override void Do()
        {
            //重新装载演示环境.
            ReLoadDept();

            try
            {
                #region 把数据换成guid模式.
                BP.Port.Depts depts = new Port.Depts();
                depts.RetrieveAll();

                string guid1 = "";
                foreach (BP.Port.Dept item in depts)
                {
                    string deptNo = item.No;
                    string guid = DBAccess.GenerGUID();
                    if (item.No == "1")
                        guid1 = guid;

                    sql = "UPDATE Port_Dept SET No='" + guid + "' WHERE No='" + deptNo + "'";
                    DBAccess.RunSQL(sql);

                    sql = "UPDATE Port_Emp SET FK_Dept='" + guid + "' WHERE FK_Dept='" + deptNo + "'";
                    DBAccess.RunSQL(sql);

                }

                sql = "UPDATE Port_Dept SET ParentNo='" + guid1 + "' WHERE ParentNo='1'";
                DBAccess.RunSQL(sql);
                #endregion

                string err = "";
                Flow fl = new Flow("023");
                fl.CheckRpt();
                fl.DoDelData();

                BP.Sys.SystemConfig.DoClearCash();

                err = "@第Send023 错误.";
                Send023 se = new Send023();
                se.Do();

                fl = new Flow("024");
                fl.CheckRpt();
                err = "@第Send024 错误.";
                Send024 s2e = new Send024();
                s2e.Do();


                fl = new Flow("005");
                fl.CheckRpt();
                err = "@第Send005 错误.";
                Send005 s5 = new Send005();
                s5.Do();

                //重新装载演示环境.
                ReLoadDept();
            }
            catch (Exception ex)
            {
                //重新装载演示环境.
                ReLoadDept();

                throw ex;
            }
        }
        /// <summary>
        /// 重新装载环境.
        /// </summary>
        public void ReLoadDept()
        {
            string sqls = "";
            sqls += "@DROP VIEW Port_DeptEmpStation";
            BP.DA.DBAccess.RunSQLs(sqls);

            string sqlscript = "";
            sqlscript = BP.Sys.SystemConfig.PathOfData + "\\Install\\SQLScript\\Port_Inc_CH_BMP.sql";
            BP.DA.DBAccess.RunSQLScript(sqlscript);
            BP.Sys.SystemConfig.DoClearCash();
        }
    }
}
