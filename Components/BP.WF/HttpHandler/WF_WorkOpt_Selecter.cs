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

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_WorkOpt_Selecter : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_WorkOpt_Selecter()
        {

        }

        public string ByStation_ShowEmps()
        {
            string staNo = this.GetRequestVal("StaNo");
            string sql = "";
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "SELECT A.No, A.Name,A.FK_Dept FROM Port_Emp A, Port_DeptEmpStation B WHERE A.No=B.FK_Emp AND B.FK_Station='" + staNo + "'";
            else
                sql = "SELECT A.No, A.Name,A.FK_Dept FROM Port_Emp A, Port_DeptEmpStation B WHERE A.No=B.FK_Emp AND A.OrgNo='" + BP.Web.WebUser.OrgNo + "' AND B.FK_Station='" + staNo + "'";

            DataTable db = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(db);
            //return "方法未完成";
        }

        #region  界面 .
        public string SelectEmpsByTeamStation_Init()
        {
            string TeamNo = this.GetRequestVal("TeamNo");
            string sql = "";
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "SELECT A.No, A.Name,A.FK_Dept FROM Port_Emp A, Port_TeamEmp B WHERE A.No=B.FK_Emp AND B.FK_Team='" + TeamNo + "'";
            else
                sql = "SELECT A.No, A.Name,A.FK_Dept FROM Port_Emp A, Port_TeamEmp B WHERE A.No=B.FK_Emp AND A.OrgNo='" + BP.Web.WebUser.OrgNo + "' AND B.FK_Team='" + TeamNo + "'";

            DataTable db = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(db);
        }
        #endregion 界面方法.
        public string AddSelectEmp()
        {
            //获得前台传来的参数
            string FK_Node = this.GetRequestVal("FK_Node");
            string WorkID = this.GetRequestVal("WorkID");
            string FK_Emp = this.GetRequestVal("FK_Emp");
            string EmpName = this.GetRequestVal("EmpName");
            string FK_Dept = this.GetRequestVal("FK_Dept");
            //得到部门名称
            Dept dept = new Dept(FK_Dept);
            string DeptName = dept.Name;
            SelectAccper selectAccper = new SelectAccper();
            selectAccper.MyPK = FK_Node + "_" + WorkID + "_" + FK_Emp;
            if (selectAccper.RetrieveFromDBSources() == 0)
            {
                selectAccper.FK_Node =int.Parse(FK_Node);
                selectAccper.WorkID =long.Parse(WorkID);
                selectAccper.FK_Emp = FK_Emp;
                selectAccper.EmpName = EmpName;
                selectAccper.DeptName = DeptName;
                selectAccper.Insert();
                return "";
            }
            return "err@添加人员失败";
        }
        public string DelSelectEmp()
        {
            string MyPK = this.GetRequestVal("MyPK");
            SelectAccper selectAccper = new SelectAccper(MyPK);
            if (selectAccper.Delete()== 0)
                return "err@删除失败";
            return "删除成功";
        }
        /// <summary>
        /// 关键字查询
        /// </summary>
        /// <returns></returns>
        public string Selecter_SearchByKey()
        {
            
            string key = this.GetRequestVal("Key"); //查询关键字.

            string ensOfM = this.GetRequestVal("EnsOfM"); //多的实体.
            Entities ensMen = ClassFactory.GetEns(ensOfM);
            QueryObject qo = new QueryObject(ensMen); //集合.
            qo.AddWhere("No", " LIKE ", "%" + key + "%");
            qo.addOr();
            qo.AddWhere("Name", " LIKE ", "%" + key + "%");
            qo.DoQuery();

            return ensMen.ToJson();
        }
    }
}
