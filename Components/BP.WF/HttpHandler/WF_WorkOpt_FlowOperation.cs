using System.Data;
using BP.DA;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_WorkOpt_FlowOperation : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_WorkOpt_FlowOperation()
        {

        }

        public string FlowTrackDateByWorkID()
        {
            string trackTable = "ND" + int.Parse(this.FK_Flow) + "Track";
            string sql = "SELECT NDFrom,NDFromT,EmpFrom,EmpFromT,RDT From " + trackTable + " Where WorkID=" + this.WorkID + " AND ActionType IN(0,1,6,7,8,11,27) Order By RDT";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase
                || BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns[0].ColumnName = "NDFrom";
                dt.Columns[1].ColumnName = "NDFromT";
                dt.Columns[2].ColumnName = "EmpFrom";
                dt.Columns[3].ColumnName = "EmpFromT";
                dt.Columns[4].ColumnName = "RDT";
            }
            return BP.Tools.Json.ToJson(dt);
        }

        /// <summary>
        /// 流程调整
        /// </summary>
        /// <returns></returns>
        public string ReSend()
        {
            int toNodeID = this.GetRequestValInt("ToNodeID");
            string toEmps = this.GetRequestVal("Emps");
            string note = this.GetRequestVal("Note");
            return BP.WF.Dev2Interface.Flow_ReSend(this.WorkID,toNodeID,toEmps,note);
        }


    }
}
