using System.Data;
using BP.DA;
using BP.WF;
using BP.WF.HttpHandler;
using BP.CCBill.Template;


namespace BP.CCBill
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill_OptOneFlow : DirectoryPageBase
    {
        #region 构造方法.
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill_OptOneFlow()
        {
        }
        #endregion 构造方法.

        /// <summary>
        /// 基础资料修改流程
        /// </summary>
        /// <returns></returns>
        public string FlowBaseData_Init()
        {
            BP.CCBill.Template.MethodFlowBaseData method = new MethodFlowBaseData();
            return "";
        }
        /// <summary>
        /// 单个实体流程记录.
        /// </summary>
        /// <returns></returns>
        public string SingleDictGenerWorkFlows_Init()
        {
            DataSet ds = new DataSet();

            string sql = "SELECT DISTINCT A.FK_Flow as No, A.FlowName as Name, B.Icon  FROM WF_GenerWorkFlow A, WF_Flow B  WHERE  A.FK_Flow=B.No AND A.PWorkID=" + this.WorkID;
            DataTable dtGroup = DBAccess.RunSQLReturnTable(sql);
            dtGroup.TableName = "Flows";
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dtGroup.Columns[0].ColumnName = "No";
                dtGroup.Columns[1].ColumnName = "Name";
                dtGroup.Columns[2].ColumnName = "Icon";
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
