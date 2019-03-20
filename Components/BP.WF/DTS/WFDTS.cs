using System;
using System.Data;
using BP.DA ; 
using System.Collections;
using BP.En;
using BP.WF;
using BP.Port ; 
using BP.En;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Template;
using BP.DTS;

namespace BP.WF.DTS
{
    

    public class UserPort : DataIOEn2
    {
        /// <summary>
        /// 调度人员,岗位,部门
        /// </summary>
        public UserPort()
        {
            this.HisDoType = DoType.UnName;
            this.Title = "生成流程部门(运行在系统第一次安装时或者部门变化时)";
            this.HisRunTimeType = RunTimeType.UnName;
            this.FromDBUrl = DBUrlType.AppCenterDSN;
            this.ToDBUrl = DBUrlType.AppCenterDSN;
        }
        public override void Do()
        {
            //执行调度部门。
            //BP.Port.DTS.GenerDept gd = new BP.Port.DTS.GenerDept();
            //gd.Do();
            // 调度人员信息。
            // Emp emp = new Emp(Web.WebUser.No);
            // emp.DoDTSEmpDeptStation();
        }
    }
    public class DelWorkFlowData : DataIOEn
    {
        public DelWorkFlowData()
        {
            this.HisDoType = DoType.UnName;
            this.Title = "<font color=red><b>清除流程数据</b></font>";
            //this.HisRunTimeType = RunTimeType.UnName;
            //this.FromDBUrl = DBUrlType.AppCenterDSN;
            //this.ToDBUrl = DBUrlType.AppCenterDSN;
        }
        public override void Do()
        {
            if (BP.Web.WebUser.No != "admin")
                throw new Exception("非法用户。");

          //  DBAccess.RunSQL("DELETE FROM WF_CHOfFlow");
            DBAccess.RunSQL("DELETE FROM WF_Bill");
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist");
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow");
          //  DBAccess.RunSQL("DELETE FROM WF_WORKLIST");
            DBAccess.RunSQL("DELETE FROM WF_ReturnWork");
            DBAccess.RunSQL("DELETE FROM WF_GECheckStand");
            DBAccess.RunSQL("DELETE FROM WF_GECheckMul");
        //    DBAccess.RunSQL("DELETE FROM WF_ForwardWork");
            DBAccess.RunSQL("DELETE FROM WF_SelectAccper");

            // 删除.
            CCLists ens = new CCLists();
            ens.ClearTable();

            Nodes nds = new Nodes();
            nds.RetrieveAll();

            string msg = "";
            foreach (Node nd in nds)
            {
                
                Work wk =  null;
                try
                {
                    wk = nd.HisWork;
                    DBAccess.RunSQL("DELETE FROM " + wk.EnMap.PhysicsTable);
                }
                catch (Exception ex)
                {
                    wk.CheckPhysicsTable();
                    msg += "@" + ex.Message;
                }
            }

            if (msg != "")
                throw new Exception(msg);
        }
    }
    public class InitBillDir : DataIOEn
    {
        /// <summary>
        /// 流程时效考核
        /// </summary>
        public InitBillDir()
        {
            this.HisDoType = DoType.UnName;
            this.Title = "<font color=green><b>创建单据目录(运行在每次更改单据文号或每年一天)</b></font>";
            this.HisRunTimeType = RunTimeType.UnName;
            this.FromDBUrl = DBUrlType.AppCenterDSN;
            this.ToDBUrl = DBUrlType.AppCenterDSN;
        }
        /// <summary>
        /// 创建单据目录
        /// </summary>
        public override void Do()
        {
            if (true)//此方法暂时排除，不需要创建目录。
                return;
            Depts Depts = new Depts();
            QueryObject qo = new QueryObject(Depts);
      //      qo.AddWhere("Grade", " < ", 4);
            qo.DoQuery();

            BillTemplates funcs = new BillTemplates();
            funcs.RetrieveAll();


            string path = BP.WF.Glo.FlowFileBill  ;
            string year = DateTime.Now.Year.ToString();

            if (System.IO.Directory.Exists(path) == false)
                System.IO.Directory.CreateDirectory(path);

            if (System.IO.Directory.Exists(path + "\\\\" + year) == false)
                System.IO.Directory.CreateDirectory(path + "\\\\" + year);


            foreach (Dept Dept in Depts)
            {
                if (System.IO.Directory.Exists(path + "\\\\" + year + "\\\\" + Dept.No) == false)
                    System.IO.Directory.CreateDirectory(path + "\\\\" + year + "\\\\" + Dept.No);

                foreach (BillTemplate func in funcs)
                {
                    if (System.IO.Directory.Exists(path + "\\\\" + year + "\\\\" + Dept.No + "\\\\" + func.No) == false)
                        System.IO.Directory.CreateDirectory(path + "\\\\" + year + "\\\\" + Dept.No + "\\\\" + func.No);
                }
            }
        }
    }

    public class OutputSQLs : DataIOEn
    {
        /// <summary>
        /// 流程时效考核
        /// </summary>
        public OutputSQLs()
        {
            this.HisDoType = DoType.UnName;
            this.Title = "OutputSQLs for produces DTSCHofNode";
            this.HisRunTimeType = RunTimeType.UnName;
            this.FromDBUrl = DBUrlType.AppCenterDSN;
            this.ToDBUrl = DBUrlType.AppCenterDSN;
        }
        public override void Do()
        {
            string sql = this.GenerSqls();
        }
        public string GenerSqls()
        {
            Log.DefaultLogWriteLine(LogType.Info, BP.Web.WebUser.Name + "开始调度考核信息:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            string infoMsg = "", errMsg = "";

            Nodes nds = new Nodes();
            nds.RetrieveAll();

            string fromDateTime = DateTime.Now.Year + "-01-01";
            fromDateTime = "2004-01-01 00:00";
            //string fromDateTime=DateTime.Now.Year+"-01-01 00:00";
            //string fromDateTime=DateTime.Now.Year+"-01-01 00:00";
            string insertSql = "";
            string delSQL = "";
            string updateSQL = "";

            string sqls = "";
            int i = 0;
            foreach (Node nd in nds)
            {
                if (nd.IsPCNode)  /* 如果是计算机节点.*/
                    continue;
                i++;
                Map map = nd.HisWork.EnMap;
                delSQL = "\n DELETE FROM " + map.PhysicsTable + " WHERE  OID  NOT IN (SELECT WORKID FROM WF_GenerWorkFlow ) AND WFState= " + (int)WFState.Runing;

                sqls += "\n\n\n -- NO:" + i + "、" + nd.FK_Flow + nd.FlowName + " :  " + map.EnDesc + " \n" + delSQL + "; \n" + insertSql + "; \n" + updateSQL + ";";
            }
            Log.DefaultLogWriteLineInfo(sqls);
            return sqls;
        }
    }
    public class OutputSQLOfDeleteWork : DataIOEn
    {
        /// <summary>
        /// 流程时效考核
        /// </summary>
        public OutputSQLOfDeleteWork()
        {
            this.HisDoType = DoType.UnName;
            this.Title = "生成删除节点数据的sql.";
            this.HisRunTimeType = RunTimeType.UnName;
            this.FromDBUrl = DBUrlType.AppCenterDSN;
            this.ToDBUrl = DBUrlType.AppCenterDSN;
        }
        public override void Do()
        {
            string sql = this.GenerSqls();
        }
        public string GenerSqls()
        {
            Nodes nds = new Nodes();
            nds.RetrieveAll();
            string delSQL = "";
            foreach (Node nd in nds)
            {
                delSQL += "\n DELETE FROM " + nd.PTable + "  ; ";
            }
            return delSQL;
        }
    }
}
