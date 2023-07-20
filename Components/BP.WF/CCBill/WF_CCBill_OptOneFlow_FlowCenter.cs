using System.Data;
using BP.DA;
using BP.WF;
using BP.WF.HttpHandler;


namespace BP.CCBill
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill_OptOneFlow_FlowCenter : DirectoryPageBase
    {
        #region 构造方法.
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill_OptOneFlow_FlowCenter()
        {
        }
        #endregion 构造方法.

        /// <summary>
        /// 单个实体流程记录.
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            DataSet ds = new DataSet();

            string sql = "SELECT DISTINCT FK_Flow as No, FlowName as Name, '' as Icon  FROM WF_GenerWorkFlow WHERE PFlowNo='" + this.PFlowNo + "' AND PWorkID=" + this.WorkID;
            DataTable dtGroup = DBAccess.RunSQLReturnTable(sql);
            dtGroup.TableName = "Flows";
            if (SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dtGroup.Columns[0].ColumnName = "No";
                dtGroup.Columns[0].ColumnName = "Name";
            }
            ds.Tables.Add(dtGroup);

            //获得所有的子流程数据.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID);

            DataTable mydt = gwfs.ToDataTableField("GenerWorkFlows");
            mydt.Columns.Add("Icon");
            ds.Tables.Add(mydt);

            return BP.Tools.Json.ToJson(ds);
        }

    }
}
